Shader "Hidden/Crossfade" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex ("Texture 2", 2D) = "white" {}
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
				float4 m = tex2D(_MainTex, i.uv);
				float4 s = tex2D(_SecondTex, i.uv);

				float4 result = m;
				result.rgb = lerp(m.rgb, s.rgb, _t);

				return result;
			}
			ENDCG
		}
	}
}
