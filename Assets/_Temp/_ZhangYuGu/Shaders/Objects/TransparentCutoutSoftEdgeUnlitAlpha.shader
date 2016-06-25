Shader "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit Alpha" {
	Properties {
		_Tran( "Transparency", Range ( 0.0, 1.0 ) ) = 1.0
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
	}

	SubShader {
		Tags { 
			"Queue"="AlphaTest" 
			"IgnoreProjector"="True" 
			"RenderType"="TransparentCutout" 
		}
		
		Lighting off
		Cull Off
		
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _Cutoff;
				fixed _Tran;
				
				v2f vert( appdata_t v ){
					v2f o;
					
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					
					o.color = v.color;
					
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					UNITY_TRANSFER_FOG(o,o.vertex);
					
					return o;
				}
				
				fixed4 _Color;
				fixed4 frag (v2f i) : SV_Target{
					half4 col = _Color * tex2D(_MainTex, i.texcoord);
					
					if( col.a <= ( 1 - _Tran ) || col.a <= 0 ){
						discard;
					}
					
					clip(col.a - _Cutoff);
					
					UNITY_APPLY_FOG(i.fogCoord, col);
					
					return col;
				}
			ENDCG
		}

		Pass {
			Tags { "RequireOption" = "SoftVegetation" }
			
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _Cutoff;
				fixed _Tran;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					
					o.color = v.color;
					
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					UNITY_TRANSFER_FOG(o,o.vertex);
					
					return o;
				}
				
				fixed4 _Color;
				fixed4 frag (v2f i) : SV_Target
				{
					half4 col = _Color * tex2D(_MainTex, i.texcoord);
					
					if( col.a <= ( 1 - _Tran ) || col.a <= 0 ){
						discard;
					}
					
					clip(-(col.a - _Cutoff));
					
					UNITY_APPLY_FOG(i.fogCoord, col);
					
					return col;
				}
			ENDCG
		}
	}

}
