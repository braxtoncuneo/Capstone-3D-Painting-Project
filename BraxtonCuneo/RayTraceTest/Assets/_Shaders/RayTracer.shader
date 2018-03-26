Shader "Unlit/RayTracer"
{
	Properties
	{
		ColorData ("ColorData", 3D) = "" {}
		SurfaceData("SurfaceData", 3D) = "" {}
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			HLSLPROGRAM
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

			texture3D ColorData;
			texture3D SurfaceData;

			int texWidth;


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
			
			
			fixed4 frag (v2f i) : SV_Target
			{


				if (true) {
					int stepMax = /* 2;// */texWidth * 3;
					int3 pos = i.color * texWidth;
					float3 dir = abs(normalize(i.dir));
					float dist = length(i.dir);
					float3 fSign = 0 - sign(i.dir);
					int3 iSign = int3(fSign);
					float3 diff = 0.5 + fSign * (0.5 - fmod(i.color * texWidth * 0.9999999, 1.0) );
					float3 rem =  (0.5 + fSign * (0.5 - i.color)) * texWidth;
					int3 stepsLeft = ceil(rem);
					int stepCount = min(stepsLeft.x + stepsLeft.y + stepsLeft.z,stepMax);
					int3 coord = i.color * texWidth;
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

					bool hit = false;
					
					while (step < stepCount) {
						//break;

						left = diff / dir;
						//left = left + int3(isnan(left)) * 2;
						best = min(left.x, min(left.y, left.z));
						crosses = int3(left == best);
						nocross = int3(left != best);
						cAvg += crosses;

						// /*
						color = ColorData[coord];
						surface = SurfaceData[coord];
						if (color.w > 0) {
							color.xyz *= surface.y*0.5 + 0.5;
						}
						samp = stepSamp(samp, color, best / texWidth, hit);
						if (samp.w >= 0.99) {
							break;
						}
						// */

						t += best;
						coord += iSign * crosses;
						step  += crosses.x + crosses.y + crosses.z;
						diff  = (diff - dir * best) + crosses;
						if (any(coord < int3(0,0,0)) && any(coord >= int3(texWidth,texWidth,texWidth))) {
							break;
						}
					}
					//samp = float4(float3(coord)/texWidth,1.0);
					//samp = float4(diff ,1.0);
					//samp = float4(rem, 1.0);
					//samp = float4(left, 1.0);
					//samp = float4( float3(t, t, t) / stepMax, 1.0);
					//samp = float4(dir, 1.0);
					//samp = float4((float3(1,1,1)*stepCount)/stepMax,1.0);
					//cAvg = normalize(cAvg); samp = float4(cAvg, 1.0);
					//samp = float4(fmod(dir * best,1.0), 1.0);
					fixed4 result;
					if (samp.w > 0.0) {
						result = float4(samp.xyz / samp.w, samp.w); // float4(i.color,1.0);
					}
					else {
						result = float4(0.0, 0.0, 0.0, 0.0);
					}
					return result;
				}
				else {
					float stepSize = max(length(i.dir), 0.5) / texWidth;
					float3 pos = (i.color - 0.5)*2.0;
					float3 vel = normalize(i.dir);
					float3 low = (float3(-1, -1, -1) - pos) / vel;
					float3 hie = (float3(1,  1,  1) - pos) / vel;
					float3 beg = min(low, hie);
					float3 end = max(low, hie);
					float entr = max(beg.x, max(beg.y, beg.z));
					float exit = min(end.x, min(end.y, end.z));
					float distLeft = exit - entr;
					float4 samp = float4(0.0,0.0,0.0,0.0);
					float4 color;
					float4 surface;

					bool hit = false;
					float dist = length(i.dir);

					while (distLeft >= 0.0 && !hit) {
						stepSize = /*max(dist, 0.5)*/ 0.125 / texWidth;
						color = ColorData[(pos*0.5 + 0.5)*texWidth];

						surface = SurfaceData[(pos*0.5 + 0.5)*texWidth];
						if (color.w > 0) {
							color.xyz *= surface.y*0.5 + 0.5;
						}
						samp = stepSamp(samp, color, stepSize, hit);
						if (samp.w >= 0.99) {
							break;
						}
						pos -= stepSize * vel;
						distLeft -= stepSize;
						dist += stepSize;
					}

					fixed4 result;
					if (samp.w > 0.0) {
						result = float4(samp.xyz / samp.w, samp.w); // float4(i.color,1.0);
					}
					else {
						result = float4(0.0, 0.0, 0.0, 0.0);
					}
					return result;
				}
			}
			ENDHLSL
		}
	}
}

