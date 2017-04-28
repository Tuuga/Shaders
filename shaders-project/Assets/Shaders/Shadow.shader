Shader "Hidden/Shadow" {
	Properties {
		_Color ("COLOR", COLOR) = (0, 0, 0, 1)
		_MainTex ("MainTex", 2D) = "white" {}
	}

	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _Color;
			sampler2D _MainTex;
			
			fixed4 frag (v2f_img i) : COLOR {
				float4 result = tex2D(_MainTex, i.uv);

				result.rgb = lerp(result.rgb, _Color.rgb, _Color.a);

				return result;
			}
			ENDCG
		}
	}
}
