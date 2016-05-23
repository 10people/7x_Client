Shader "Custom/Characters/Check Normal" {
	Properties {
		_FxColor( "Fx Color", Color ) = ( 0, 0, 0, 0 )
        _MainTex( "Base (RGB)", 2D ) = "white" {}
        _MainWeight( "Brightness", Range( 0, 2 ) ) = 1.0
        _RimColor( "Rim", Color) = ( 1, 1, 1, 1 )
        _RimWeight( "Rim Weight", Range( 15, 1.5 ) ) = 3
    }
	
	Category {
		Tags {
			"Queue"="Geometry+1"
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
					float2 uv : TEXCOORD0;
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				
				uniform fixed4 _FxColor;
				uniform fixed4 _RimColor;

				float _MainWeight;
				float _RimWeight;

				v2f vert (appdata_base v) {
					v2f o;
					
					o.pos = mul( UNITY_MATRIX_MVP, v.vertex );

					o.color_d = UnityObjectToWorldNormal(v.normal);

					o.uv = v.texcoord.xy;
					
					return o;
				}
				
				fixed4 frag(v2f i) : SV_Target {
					fixed4 t_c = tex2D(_MainTex, i.uv);

					t_c.rgb = i.color_d;

					return t_c;
				}
				ENDCG
			}
			
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
	}
}