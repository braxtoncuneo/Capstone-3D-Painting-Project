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
		Tags{ "Queue" = "AlphaTest"  "RenderType" = "AlphaTest" }
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


			float4 stepSamp(float4 samp, float4 curr,float stepSize, inout bool hit) {
				float4 result;
				if (curr.w >= 127) {
					hit = true;
					result = float4(samp.xyz + curr.xyz * (1 - samp.w), 1);
				}
				else {
					result = float4(samp.xyz + curr.xyz * (1 - samp.w) * curr.w  * stepSize, samp.w + (1 - samp.w)* curr.w  * stepSize );
					result.w = clamp(result.w,0.0, 1.0);
				}
				return result;
			}
			
			
			fOut frag (v2f i)
			{

				int stepMax = /* 1;// */texWidth * 3;
				int3 pos = i.color * texWidth;
				float3 dir = abs(normalize(i.dir));
				float dist = length(i.dir);
				float3 fSign = 0 - sign(i.dir);
				int3 iSign = int3(fSign);
				float3 diff = 0.5 + fSign * (0.5 - fmod(i.color * texWidth * 0.9999999 , 1.0) );
				float3 rem =  (0.5 + fSign * (0.5 - i.color) ) * texWidth;
				int3 stepsLeft = ceil(rem);
				int stepCount = min(stepsLeft.x + stepsLeft.y + stepsLeft.z,stepMax);
				int3 coord = i.color * texWidth * 0.9999999;
				int step = 0;
				float3 left;
				float3 best;
				int3 crosses;
				int3 nocross;
				float3 cAvg = float3(0,0,0);
				float4 samp = float4(0.0, 0.0, 0.0, 0.0);
				float4 color;
				float4 surface;
				float t = 0;

				float nearZ = _ProjectionParams.y;
				float farZ	= _ProjectionParams.z;
				float tToZ = blockWidth / texWidth;

				bool hit = false;
					
				while (step < stepCount) {
					//break;

					left = diff / dir;
					best = min(left.x, min(left.y, left.z));
					crosses = int3(left == best);
					nocross = int3(left != best);
					cAvg += crosses;

					color = ColorData[coord];
					surface = SurfaceData[coord];
					if (color.w > 0) {
						color.xyz *= surface.y*0.5 + 0.5;
					}
					samp = stepSamp(samp, color, best / texWidth, hit);
					if (samp.w >= 0.99) {
						break;
					}
					

					t += best;
					coord += iSign * crosses;
					step  += crosses.x + crosses.y + crosses.z;
					diff  = (diff - dir * best) + crosses;
					if (any(coord < int3(0,0,0)) || any(coord > int3(texWidth,texWidth,texWidth))) {
						break;
					}
				}
				//samp = float4(float3(coord)/texWidth,1.0);
				//samp = float4(coord % 2, 1.0);
				//samp = float4(diff ,1.0);
				//samp = float4(rem, 1.0);
				//samp = float4(left, 1.0);
				//samp = float4( float3(t, t, t) / stepMax, 1.0);
				//samp = float4(dir, 1.0);
				//samp = float4((float3(1,1,1)*stepCount)/stepMax,1.0);
				//cAvg = normalize(cAvg); samp = float4(cAvg, 1.0);
				//samp = float4(fmod(dir * best,1.0), 1.0);
				fOut result;
				float outZ = i.vertex.z - (t * tToZ) / (farZ-nearZ);// t * blockWidth;
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

