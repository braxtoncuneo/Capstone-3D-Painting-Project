﻿Shader "Unlit/RayTracer"
{
	Properties
	{
		_Data ("DataTex", 3D) = "" {}
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

			sampler3D _Data;
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

			float4 step(float4 samp, float4 curr,float stepSize, inout bool hit) {
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
				float stepSize = max(length(i.dir) * 1.0 / 64, 1.0/texWidth);
				float level = 0.0;// floor(log2(length(i.dir) / 4.0));
				float3 pos = (i.color-0.5)*2.0;
				float3 vel = normalize(i.dir);
				float3 low = (float3(-1, -1, -1) - pos) / vel;
				float3 hie = (float3( 1,  1,  1) - pos) / vel;
				float3 beg = min(low, hie);
				float3 end = max(low, hie);
				float entr = max(beg.x, max(beg.y, beg.z));
				float exit = min(end.x, min(end.y, end.z));
				float distLeft = exit - entr;
				float4 samp = float4(0.0,0.0,0.0,0.0);
				float4 vox;
				bool hit = false;

				while (distLeft >= 0.0 && !hit) {
					vox = tex3Dlod(_Data,float4(pos*0.5 + 0.5,level));
					samp = step(samp, vox, stepSize, hit);
					if (samp.w >= 0.99) {
						break;
					}
					pos -= stepSize * vel;
					distLeft -= stepSize;
				}

				fixed4 col;
				if (samp.w > 0.0) {
					col = float4(samp.xyz / samp.w, samp.w); // float4(i.color,1.0);
				}
				else {
					col = float4(0.0, 0.0, 0.0, 0.0);
				}
				return col;
			}
			ENDHLSL
		}
	}
}
