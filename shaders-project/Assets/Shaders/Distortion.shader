Shader "Hidden/Distortion" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Strength ("Strength", Range (0, 1)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float _Strength;
			
			fixed4 frag (v2f_img i) : COLOR {

				float2 d = (i.uv - 0.5) * 2;
				d *= abs(d) ;
				d = (d + 1) / 2;

				i.uv = lerp(i.uv, d, _Strength);

				float4 c = tex2D(_MainTex, i.uv);
				return c;
			}
			ENDCG
		}
	}
}
