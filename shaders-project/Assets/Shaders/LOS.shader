Shader "Custom/LOS" {
	Properties {
		_MainTex ("MainTex", 2D) = "white" {}
		_SecondTex ("SecondTex", 2D) = "white" {} 
		_t ("T", Range (0, 1)) = 0
	}

	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _SecondTex;
			uniform float _t;
			
			fixed4 frag (v2f_img i) : COLOR {
				float4 tex1 = tex2D(_MainTex, i.uv);
				float4 tex2 = tex2D(_SecondTex, i.uv);
				float4 result = (0,0,0,1);
				result.rgb = lerp(tex1.rgb, tex2.rgb, _t);
				return result;
			}
			ENDCG
		}
	}
}
