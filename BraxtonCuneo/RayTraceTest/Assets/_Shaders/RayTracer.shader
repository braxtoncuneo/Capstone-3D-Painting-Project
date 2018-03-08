Shader "Unlit/RayTracer"
{
	Properties
	{
		//_Data ("DataTex", 3D) = "" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			HLSLPROGRAM
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
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f result;
				result.vertex = UnityObjectToClipPos(v.vertex);
				result.dir = ObjSpaceViewDir(v.vertex);
				result.color = v.color;
				return result;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float stepSize = 1.0 / 256;
				float3 pos = (i.color-0.5)*2.0;
				float3 vel = normalize(i.dir);
				float3 low = (float3(-1, -1, -1) - pos) / vel;
				float3 hie = (float3( 1,  1,  1) - pos) / vel;
				float3 beg = min(low, hie);
				float3 end = max(low, hie);
				float entr = max(beg.x, max(beg.y, beg.z));
				float exit = min(end.x, min(end.y, end.z));
				float distLeft = exit - entr;
				float samp = 0.0;
				float3 vox;
				while (distLeft >= 0.0) {
					if (length(pos) < 0.75) {
						//vox = = _Data.sample(pos);
						samp += stepSize*0.5;
					}
					pos -= stepSize * vel;
					distLeft -= stepSize;
				}

				fixed4 col = float4(samp,float2(1.0,1.0)*0.25*(exit - entr),1.0); // float4(i.color,1.0);
				return col;
			}
			ENDHLSL
		}
	}
}
