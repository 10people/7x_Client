Shader "Custom/Characters/Main Texture Hight Light Rim"{
	Properties {
		_MainTex( "Base (RGB)", 2D ) = "white" {}
        _MainColor( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
        _RimColor( "Rim", Color) = ( 0, 0, 0, 1 )
        _RimWidth( "Rim Width", Range( 8, 1.0 ) ) = 8
		_RimWeight( "Rim Weight", Range( 0, 1.0 ) ) = 1
    }
	
	Category {
		Tags {
			"Queue"="AlphaTest+100"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
		
		ZWrite On
		Cull Off
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		
		SubShader {
			Pass {
				Tags {
					
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				struct v2f{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float4 vertex : TEXCOORD2;
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;

				uniform fixed4 _FxColor;
				uniform fixed4 _RimColor;
				uniform fixed4 _MainColor;

				float _RimWidth;
				float _RimWeight;

				v2f vert (appdata_base v) {
					v2f o;
					
					o.pos = mul( UNITY_MATRIX_MVP, v.vertex );

					o.uv = v.texcoord.xy;

					fixed3 t_v = UnityObjectToWorldNormal(v.normal);

					o.normal = v.normal;

					o.vertex.xyz = v.vertex;

					o.vertex.w = 0;

					return o;
				}
				
				fixed4 frag(v2f i) : SV_Target {
					fixed4 t_c = tex2D(_MainTex, i.uv);

					fixed4 t_w = 0;

					if( _RimWidth == 8 ){
						t_w = 0;
					}
					else{
						t_w = pow( 1 - saturate( abs( dot( i.normal, normalize(ObjSpaceViewDir(i.vertex)) ) ) ), _RimWidth ) * _RimWeight;
					}

					t_c.rgb = t_c.rgb * _MainColor * 2.2f * ( 1 - t_w ) + t_w * _RimColor;

					return t_c;
				}
				ENDCG
			}
			
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
	}
}