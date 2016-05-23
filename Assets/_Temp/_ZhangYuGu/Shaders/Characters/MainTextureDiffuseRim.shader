Shader "Custom/Characters/Main Texture Diffuse Rim"{
	Properties {
		_FxColor( "Fx Color", Color ) = ( 0, 0, 0, 0 )
        _MainTex( "Base (RGB)", 2D ) = "white" {}
        _MainColor( "Main Color", Color ) = ( 1, 1, 1, 1 )
        _MainWeight( "Brightness", Range( 0, 2 ) ) = 1.0
        _RimColor( "Rim", Color) = ( 0, 0, 0, 1 )
        _RimWeight( "Rim Weight", Range( 8, 1.0 ) ) = 8
    }
	
	Category {
		Tags {
			"Queue"="AlphaTest"
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
					"LightMode"="ForwardBase"
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"
				#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
				
				struct v2f{
					float4 pos : SV_POSITION;
					fixed3 color_d : COLOR0;
					fixed4 color_a : COLOR1;
					float2 uv : TEXCOORD0;
					float3 normal : TEXCOORD1;
					float4 vertex : TEXCOORD2;
					SHADOW_COORDS(3)
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;

				uniform fixed4 _FxColor;
				uniform fixed4 _RimColor;
				uniform fixed4 _MainColor;

				float _MainWeight;
				float _RimWeight;

				v2f vert (appdata_base v) {
					v2f o;
					
					o.pos = mul( UNITY_MATRIX_MVP, v.vertex );

					o.uv = v.texcoord.xy;

					fixed3 t_v = UnityObjectToWorldNormal(v.normal);

					o.color_d = max(0, dot(t_v, _WorldSpaceLightPos0.xyz)) * _LightColor0;

					o.color_a.xyz = ShadeSH9(half4(t_v,1));

					o.color_a.w = 0;

					TRANSFER_SHADOW(o)

					o.normal = v.normal;

					o.vertex.xyz = v.vertex;

					o.vertex.w = 0;

					return o;
				}
				
				fixed4 frag(v2f i) : SV_Target {
					fixed4 t_c = tex2D(_MainTex, i.uv);

					t_c.rgb = t_c.rgb * _MainWeight * ( i.color_d * SHADOW_ATTENUATION(i) + i.color_a ) * _MainColor;

					if( _RimWeight == 8 ){
						i.color_a = 0;
					}
					else{
						i.color_a.w = pow( 1 - saturate( abs( dot( i.normal, normalize(ObjSpaceViewDir(i.vertex)) ) ) ), _RimWeight );
					}

					t_c.rgb = t_c.rgb * ( 1 - i.color_a.w ) + i.color_a.w * _RimColor;

					return t_c;
				}
				ENDCG
			}
			
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
	}
}