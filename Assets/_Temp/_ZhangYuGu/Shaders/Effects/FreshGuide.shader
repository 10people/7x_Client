Shader "Custom/Effects/Fresh Guide"{
	Properties{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		
		_Tex_ST("Vector4",Vector) = (1, 1, 0, 0)
	}
	
	SubShader{
		LOD 100

		Tags{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZTest Less
		Fog { Mode Off }
		//Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float4 color : COLOR;
				};
	
				struct v2f{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};
	
				sampler2D _MainTex;
				
				float4 _MainTex_ST;
				
				uniform float4 _Tex_ST;
				
				uniform float4 _TintColor;
				
		        float _Coef;
				
				float _T;
				
				v2f vert (appdata_t v){
					v2f o;
					
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					
					o.texcoord = TRANSFORM_TEX(v.texcoord, _Tex);
					
					return o;
				}
				
				static const half OGW = 1.0f;
				
				half4 frag (v2f i) : COLOR{
					half4 tex = tex2D(_MainTex, i.texcoord);
							
					return tex;
				}
			ENDCG
		}
	}
}