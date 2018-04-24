Shader "Unlit/RayTracer"
{
	Properties
	{
		ColorData("ColorData", 3D) = "" {}
		SurfaceData("SurfaceData", 3D) = "" {}
		SkipData("SkipData", 3D) = "" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent"  "RenderType" = "AlphaTest" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		ZTest LEqual

		Pass
	{

		HLSLPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 5.0
#pragma vertex vert
#pragma fragment frag
		// make fog work

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 color : COLOR0;
		//float2 st : TEXCOORD1;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float3 color : COLOR0;
		//float2 st : TEXCOORD1;
		float4 vertex : SV_POSITION;
		float3 dir: O_DIRECTION;
	};

	struct fOut {
		float4 color : SV_TARGET;
		float depth : SV_DEPTH;
	};

	texture3D ColorData;
	texture3D SurfaceData;
	texture3D<int4> SkipData;
	int texWidth;
	float blockWidth;


	float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f result;
				result.vertex = UnityObjectToClipPos(v.vertex);
				result.dir = ObjSpaceViewDir(v.vertex);
				result.color = v.color;
				return result;
			}


			float4 stepSamp(float4 samp, float4 curr,float stepSize) {
				float4 result;
				if (curr.w >= 1024) {
					result = float4(samp.xyz + curr.xyz * (1 - samp.w), 1);
				}
				else {
					float adjTrans = 1.0 - pow(1.0 - curr.w, stepSize);
					//float adjTrans = curr.w*stepSize;
					result = float4(samp.xyz + curr.xyz * (1 - samp.w) * adjTrans, samp.w + (1 - samp.w)* adjTrans );
					result.w = clamp(result.w,0.0, 1.0);
				}
				return result;
			}
			
			void findLevel(in int3 coord, inout int3 sCoord, out float mag) {
				int i;
				mag = texWidth;
				for (i = texWidth; i > 1; i >>= 1) {
					if ((SkipData[coord/i].x & (i>>1)) != 0) {
						break;
					}
					mag = mag / 2;
				}
				if (i == 0) {
					sCoord = coord;
				}
				else {
					sCoord = coord & (~(i-1));
				}
			}

			void processStep(inout float4 samp, in float3 sCoord, in float dist) {
				float4 color = ColorData[sCoord];
				float4 surface = SurfaceData[sCoord];
				if (color.w > 0) {
					color.xyz *= surface.y*0.5 + 0.5;
				}
				samp = stepSamp(samp, color, dist / texWidth);
			}
			
			fOut frag (v2f i)
			{

				float stepMax = /* 1;// */sqrt(texWidth*texWidth * 3);
				float stepTotal = 0;
				int iterMax = texWidth * 3;
				int iter = 0;

				float3 dir = abs(normalize(i.dir));
				float3 fSign = 0 - sign(i.dir);
				int3 iSign = int3(fSign);
				float3 fCoord = i.color * texWidth * 0.9999999;
				float3 diff = 0.5 + fSign * (0.5 - fmod(fCoord, 1.0));
				int3 sCoord;
				int3 coord = fCoord;
				float3 left;
				float3 deltaTime;
				float3 cAvg = float3(0,0,0);
				float4 samp = float4(0.0, 0.0, 0.0, 0.0);
				int sSamp;

				float nearZ = _ProjectionParams.y;
				float farZ	= _ProjectionParams.z;
				float tToZ = blockWidth / texWidth;
				int vMin = 128;
				float mag = 1;

				bool hit = false;

				while ( (stepTotal < stepMax) && (iter<iterMax) ) {
					
					findLevel(coord, sCoord, mag);

					diff = 0.5 + fSign * (0.5 - fmod(fCoord, 1.0*mag) / mag);
					left = diff / dir;
					deltaTime = min(left.x, min(left.y, left.z));

					processStep(samp, sCoord, deltaTime*mag);

					if (samp.w >= 0.99) {
						break;
					}
					
					stepTotal += deltaTime*mag + 0.01;
					fCoord = fCoord + dir * (deltaTime*mag+0.01) * fSign;
					coord = fCoord;

					if (any(fCoord < float3(0, 0, 0)) || any(fCoord > float3(texWidth, texWidth, texWidth))) {
						break;
					}
					iter++;
				}

				//samp = float4(float3(fCoord)/texWidth,1.0);
				//samp = float4(coord % 2, 1.0);
				//samp = float4(diff ,1.0);
				//samp = float4(fmod(fCoord, 1.0),1.0);
				//samp = float4(left, 1.0);
				//samp = float4( float3(t, t, t) / stepMax, 1.0);
				//samp = float4(dir*deltaTime, 1.0);
				//samp = float4((float3(1,1,1)*stepCount)/stepMax,1.0);
				//cAvg = normalize(cAvg); samp = float4(cAvg, 1.0);
				//samp = float4(fmod(dir * best,1.0), 1.0);
				//samp = float4(log2(iter + 0.001) / 8.0, 0.0, 0.0, 1.0);
				/*
				if (iter > iterMax * 0.6) {
					samp = float4(1.0, 0.0, 0.0, 1.0);
				}
				else {
					samp = float4(0.0, 0.0, 0.0, 1.0);
				}
				*/

				fOut result;
				float outZ = i.vertex.z - (stepTotal * tToZ) / (farZ-nearZ);// t * blockWidth;
				result.depth = outZ;
				if (samp.w > 0.0) {
					result.color = float4(samp.xyz / samp.w, samp.w); // */ float4(float3(1,1,1)*outZ,1.0);
				}
				else {
					discard;
				}
					
				return result;
				
			}
			ENDHLSL
		}
	}
}

