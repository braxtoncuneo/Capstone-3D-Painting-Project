Shader "_Shaders/BlitShader" {
	SubShader {
		Pass {
			Cull Off
			ZTest Always
			ZWrite Off

			HLSLPROGRAM
			#pragma target 3.5
			#pragma vertex CopyPassVertex
			#pragma fragment CopyPassFragment


			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

			//TEXTURE2D(color_texture);
			//SAMPLER(samplercolor_texture);
			Texture2D color_texture;
			SamplerState sampler_point_repeat;
			
			//TEXTURE2D(_DepthTex);
			//SAMPLER(sampler_DepthTex);

			float4 _ProjectionParams;

			struct VertexInput {
				float4 pos : POSITION;
			};

			struct VertexOutput {
				float4 clipPos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			/*
			struct PixelOutput {
				float4 Color: COLOR;
				float  Depth: DEPTH;
			};
			*/

			VertexOutput CopyPassVertex (VertexInput input) {
				VertexOutput output;
				output.clipPos = float4(input.pos.xy, 0.0, 1.0);
				output.uv = input.pos.xy * 0.5 + 0.5;
				if (_ProjectionParams.x < 0.0) {
					output.uv.y = 1.0 - output.uv.y;
				}
				return output;
			}

			float4 CopyPassFragment (VertexOutput input) : SV_TARGET {
				//PixelOutput output;
				//output.Color = SAMPLE_TEXTURE2D(color_texture,  samplercolor_texture,  input.uv);
				//output.Depth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, input.uv);
				//float4 b = SAMPLE_TEXTURE2D(color_texture,  samplercolor_texture,  input.uv);
				float4 b = color_texture.Sample(sampler_point_repeat,input.uv);
				//b.x = input.uv.x;
				//b.y = input.uv.y;
				return b;
			}

			ENDHLSL
		}
	}
}
