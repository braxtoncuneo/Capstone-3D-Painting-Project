Shader "Unlit/RayTracer"
{
	Properties
	{
		// None of the 3D textures used should have default values
		ColorData("ColorData", 3D) = "" {}
	SurfaceData("SurfaceData", 3D) = "" {}
	SkipData("SkipData", 3D) = "" {}
	}
		SubShader
	{
		// These tags ensure blocks are ordered from back to front
		// and given full alpha and depth testing
		Tags{ "Queue" = "Transparent"  "RenderType" = "AlphaTest" }
		// Standard LOD hint for unlit shaders
		LOD 100
		// Ensure the right alpha blending equation is being used
		Blend SrcAlpha OneMinusSrcAlpha
		// Enable depth to be manipulated and tested
		ZWrite On
		ZTest LEqual

		Pass
	{

		HLSLPROGRAM

		// Hint to sacrifice precision for speed
#pragma fragmentoption ARB_precision_hint_fastest
		// Make sure this shader builds for a high-tier graphics model
#pragma target 5.0

#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		// Datatype for vertex shader input
		struct appdata
	{
		float4 vertex : POSITION;
		float3 color : COLOR0;
	};

	// Datatype for vertex shader output and fragment shader input
	struct v2f
	{
		float3 color : COLOR0;
		float4 vertex : SV_POSITION;
		float4 world: POSITION2;
		float3 camera: O_DIRECTION;
	};

	// Datatype for fragment shader output
	struct fOut {
		float4 color : SV_TARGET;
		float depth : SV_DEPTH;
	};

	// Holds the color data for voxels in the rendering block
	texture3D ColorData;
	// Holds the normal data for voxels in the rendering block
	texture3D SurfaceData;
	// Holds the metadata required for accellerated ray tracing
	texture3D<int> SkipData;
	// How many voxels wide the block is
	int texWidth;
	// How many Unity units wide the block is
	float blockWidth;

	// The current transform of the brush
	//float4x4 brush_transform;
	// The current color of the brush
	//float4 brush_color;


	/*
	** The main vertex shader function
	** Inputs are :
	**	-  model-space position ( as vertex )
	**	-  block-space position ( as color )
	** Outputs are:
	**  -  block-space position ( as color )
	**  -  projection-space position ( as color )
	**  -  world-space position ( as world )
	**  -  object-space camera position ( as camera )
	*/
	v2f vert(appdata v)
	{
		v2f result;
		// Block-space coordinates encoded in color
		result.color = v.color;
		// Actual projection-space vertex position
		result.vertex = UnityObjectToClipPos(v.vertex);
		// World-space vertex position
		result.world = mul(unity_ObjectToWorld, v.vertex);
		// Object-space offset from camera
		result.camera = ObjSpaceViewDir(v.vertex);
		return result;
	}


	/*
	** The function responsible for handling volumetric transparency
	** sampleAcc: the present value of the ongoing sample accumulator
	** newSamp: the color value of the newest sample (from the current voxel)
	** stepSize: the distance the ray is traversing over the voxel
	*/
	float4 stepsample(float4 sampleAcc, float4 newSamp,float stepSize) {
		// Convert basic transparency to effective transparency based
		// on how much space was traversed by the ray
		float adjTrans = 1.0 - pow(1.0 - newSamp.w, stepSize);
		// Calculate new ongoing sample value
		float4 result = float4(sampleAcc.xyz + newSamp.xyz * (1 - sampleAcc.w) * adjTrans, sampleAcc.w + (1 - sampleAcc.w)* adjTrans);
		// Clamp sampleAcc alpha to normalized range
		result.w = clamp(result.w,0.0, 1.0);
		return result;
	}

	/*
	** Scans the SkipGrid in a top-down manner and retrieves required metadata
	** coord: the coordinates of the voxel the trace is currently in
	** sCoord: the coordinates of the voxel the trace should sample given its
	**         current position
	** mag: the size of the voxel being traversed through ( a power of two )
	** covered: whether or not the current section of voxels being sampled is
	**          enclosed by voxels of identical color
	*/
	void findLevel(in int3 coord, inout int3 sCoord, out float mag, out bool covered) {
		// The level in the hierarchy being checked
		uint level;
		// The mask used to check if a section/level of the SkipGrid is covered
		uint covItr = 0x80000000;
		// Stores samples from the SkipGrid
		int skDatum;
		mag = texWidth;
		covered = false;
		// Checking levels by powers of two, highest to lowest
		for (level = texWidth; level > 1; level >>= 1) {
			skDatum = SkipData[coord / level];
			covered = ((skDatum & covItr) != 0);
			// Once similarity flag is found, escape the loop
			if ((skDatum & (level >> 1)) != 0) {
				break;
			}
			mag = mag / 2;
			covItr >>= 1;
		}
		// If no similarity was found, simply return input coordinates
		if (level == 0) {
			sCoord = coord;
		}
		// Mask out bits less than the level if similarity is found
		else {
			sCoord = coord & (~(level - 1));
		}
	}

	/*
	** Samples the ColorGrid and SurfaceGrid and processes the results prior to calculating
	** effects upon the sample accumulator
	**
	** sampleAcc: the ongoing sample accumulator for the trace
	** sCoord: the coordinates to use for sampling into the ColorGrid and SurfaceGrid
	** dist: the distance traveled by the trace during the current step
	** covered: whether or not the region of voxels being sampled is considered covered
	*/
	void processStep(inout float4 sampleAcc, in float3 sCoord, in float dist, in bool covered) {
		float4 color = ColorData[sCoord];
		float4 surface;
		// Count all covered voxels as having a zero normal
		if (covered) {
			surface = float4(0, 0, 0, 0);
		}
		else {
			surface = SurfaceData[sCoord];
		}
		// Brighten/darken the color of the sample by the normal (a cheap diffuse shading)
		if (color.w > 0) {
			color.xyz *= surface.y*0.5 + 0.5;
		}
		sampleAcc = stepsample(sampleAcc, color, dist);
	}



	/*
	** The main function of the fragment shader
	** Inputs are:
	**  -  block-space position ( as color )
	**  -  projection-space position ( as color )
	**  -  world-space position ( as world )
	**  -  object-space camera position ( as camera )
	** Outputs are:
	**  - fragment color (as color)
	**  - fragment depth (as depth)
	*/
	fOut frag(v2f i)
	{

		// The hard limit for what distance traces may travel
		float stepMax = sqrt(texWidth*texWidth * 3);
		// The distance accumulator
		float stepTotal = 0;

		// The hard limit for how many steps a trace may take
		int iterMax = texWidth * 3;
		// Counts the number of steps made by a trace
		int iter = 0;

		// Normalized direction of travel
		float3 dirNorm = abs(normalize(i.camera));

		// Sign of the axial components of the direction of travel
		float3 fSign = 0 - sign(i.camera);
		int3 iSign = int3(fSign);

		// Floating-point representation of where in the block the
		// trace is
		float3 fCoord = i.color * texWidth * 0.9999999;

		// The normalized offset ( or difference ) in position between the
		// trace and the next voxel boundaries (along each axial plane)
		// that will trigger a new step
		float3 diff = 0.5 + fSign * (0.5 - fmod(fCoord, 1.0));

		// Sampling coordinates
		int3 sCoord;

		// Integer position of trace (for referencing specific voxels)
		int3 coord = fCoord;

		// Axially decomposed representation of the parametric time required
		// to hit the next axial plane indicated by diff
		float3 left;
		// The parametric time left to the soonest step transition
		float deltaTime;

		// Accumulates samples made through the life of the trace
		float4 sampleAcc = float4(0.0, 0.0, 0.0, 0.0);

		// Unit conversion between parametric time and world space
		float tToZ = blockWidth / texWidth;

		// The magnetude of the voxel being traversed
		float mag = 1;

		// Indicates whether or not the current voxel is covered
		bool covered;

		while ((stepTotal < stepMax) && (iter<iterMax)) {

			findLevel(coord, sCoord, mag,covered);

			diff = 0.5 + fSign * (0.5 - fmod(fCoord, 1.0*mag) / mag);
			left = diff / dirNorm;
			deltaTime = min(left.x, min(left.y, left.z));

			processStep(sampleAcc, sCoord, deltaTime*mag, covered);

			if (sampleAcc.w >= 0.99) {
				break;
			}

			stepTotal += deltaTime * mag + 0.01;
			fCoord = fCoord + dirNorm * (deltaTime*mag + 0.01) * fSign;
			coord = fCoord;

			if (any(fCoord < float3(0, 0, 0)) || any(fCoord > float3(texWidth, texWidth, texWidth))) {
				break;
			}
			iter++;
		}


		// The below block of commented code is used for debugging purposes and should not
		// be uncommented for release builds

		//sampleAcc = float4(float3(fCoord)/texWidth,1.0);
		//sampleAcc = float4(coord % 2, 1.0);
		//sampleAcc = float4(diff ,1.0);
		//sampleAcc = float4(fmod(fCoord, 1.0),1.0);
		//sampleAcc = float4(left, 1.0);
		//sampleAcc = float4( float3(t, t, t) / stepMax, 1.0);
		//sampleAcc = float4(dirNorm*deltaTime, 1.0);
		//sampleAcc = float4((float3(1,1,1)*stepCount)/stepMax,1.0);
		//cAvg = normalize(cAvg); sampleAcc = float4(cAvg, 1.0);
		//sampleAcc = float4(fmod(dirNorm * best,1.0), 1.0);
		//sampleAcc = float4(/*log2(iter + 0.001) / 8.0*/ ((iter/(1.0*texWidth))), 0.0, 0.0, 1.0);


		// The variable to be used as output
		fOut result;

		// The world-space offset between the default fragment position and the camera
		float3 fragCamOffset = i.world - _WorldSpaceCameraPos;

		// Calculates the depth of the terminal position of the trace
		float3 rayHit = i.world + normalize(fragCamOffset) * stepTotal * tToZ;
		float rayZ = mul(UNITY_MATRIX_VP, float4(rayHit, 1.0)).z;


		result.color = float4(0, 0, 0, 0); 

		// Applies the voxel sample to the result
		if (sampleAcc.w > 0.0) {
			result.depth = rayZ;
			result.color = float4(result.color.xyz + (1 - result.color.w) * sampleAcc.xyz / sampleAcc.w,
				result.color.w + (1 - result.color.w) * sampleAcc.w);
		}

		// If the result is still transparent, simply discard
		if (result.color.w <= 0) {
			discard;
		}

		// A debug statement for checking the depth written to fragments
		// result.color = float4(result.depth, result.depth, result.depth,1.0);

		return result;

	}
	ENDHLSL
	}
	}
}

