Shader "Custom/Effects/Sparkle Effect"{
	Properties{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		
		_Tex_ST("Vector4",Vector) = (1, 1, 0, 0)
		
		_TintColor( "Tint Color", Color ) = ( 0, 0, 0, 0 )
		
		_Coef( "Coefficient", Range (0.0, 1.0 ) ) = 1
		
		_T( "T", Range (-2, 2 ) ) = 0
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
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"
	
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float4 color : COLOR;
				};
	
				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 m_color : COLOR;
					half2 texcoord : TEXCOORD0;
					half2 texcoordOg : TEXCOORD1;
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
										
					o.texcoordOg = v.texcoord;
					
					o.texcoord = TRANSFORM_TEX(v.texcoord, _Tex);
					
					o.m_color = _TintColor * _Coef;
					
					return o;
				}
				
				static const half OGW = 1.0f;
				
				half4 frag (v2f i) : COLOR{
					half t_og_x = i.texcoordOg - _T;
					
					if( abs( i.texcoordOg.y - t_og_x ) < OGW ){
						t_og_x = abs( abs( i.texcoordOg.y - t_og_x ) - OGW ) / OGW;
					}
					else{
						t_og_x = 0;
					}
					
					half4 tex = tex2D(_MainTex, i.texcoord);
										
					half4 col =  lerp( tex, i.m_color * tex.w, clamp( t_og_x * 1.1, 0.0, 1.0 ) );

					return col;
				}
			ENDCG
		}
	}
}