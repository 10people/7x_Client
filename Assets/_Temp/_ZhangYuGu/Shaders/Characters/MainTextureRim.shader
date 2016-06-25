Shader "Custom/Characters/Main Texture Rim"{
   Properties {
		_FxColor( "Fx Color", Color ) = ( 0, 0, 0, 0 )
        _MainTex( "Base (RGB)", 2D ) = "white" {}
        _MainWeight( "Brightness", Range( 0, 2 ) ) = 1.2
        _Light("Light",Vector) =( 0.4, -0.9, 0.1 )
        _MainColor( "Main", Color ) = ( 0, 0, 0, 0 )
        _DiffuseColor( "Diffuse", Color ) = ( 0, 0, 0, 0 )
        _DiffuseWeight( "Diffuse Weight", Range( 0, 1 ) ) = 0
		_RimColor( "Rim", Color) = ( 1, 1, 1, 1 )
        _RimWidth( "Rim Width", Range( 0, 1 ) ) = 0.35
        _RimWeight( "Rim Weight", Range( 0, 1 ) ) = 0
    }
	
	Category {
		Tags {
			"Queue"="Transparent-8"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
		
		ZWrite On
		Cull Off
		Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha 
		
		SubShader {
			Pass {
				Lighting Off
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata 
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed3 color : COLOR;
					fixed3 color_d : TEXCOORD1;
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				
				uniform fixed4 _FxColor;
				uniform fixed4 _MainColor;
				uniform fixed4 _DiffuseColor;
				uniform fixed4 _RimColor;
				float3 _Light;

				float _MainWeight;
				float _DiffuseWeight;
				float _RimWidth;
				float _RimWeight;

				v2f vert (appdata_base v) {
					v2f o;
					
					o.pos = mul( UNITY_MATRIX_MVP, v.vertex );

					float3 viewDir = normalize( ObjSpaceViewDir( v.vertex ) );

					float3 t_normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal );

					t_normal = t_normal / length( t_normal );

					o.color_d = max( 0, dot( t_normal, -normalize( _Light ) ) ) * _DiffuseColor.xyz * _DiffuseWeight;
					
					o.color = smoothstep( 1 - _RimWidth * _RimWidth, 1.0, 1 - dot( v.normal, viewDir ) ) * _RimWeight * _RimColor;

					o.uv = v.texcoord.xy;
					
					return o;
				}
				
				fixed4 frag(v2f i) : COLOR {
					fixed4 t_c = tex2D(_MainTex, i.uv);

					t_c.xyz = ( t_c.xyz * _MainColor + t_c.xyz ) * _MainWeight + i.color_d + i.color + _FxColor.xyz;
				   
					return t_c;
				}
				ENDCG
			}
		}
	}
}