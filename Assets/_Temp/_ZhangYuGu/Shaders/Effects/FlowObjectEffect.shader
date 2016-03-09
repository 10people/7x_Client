Shader "Custom/Effects/Flow Object Effect" {
	
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color( "Main Color", Color ) = ( 1, 1, 1, 1 )
		_Str("Strength X, Y, Z, XYZ",Vector) =(0,0,0,0)
		_Pos("pos center",Vector) =(0,0,0,0)
	}
	
	SubShader {

		Tags {
			"Queue"="AlphaTest+1"
			"RenderType"="TransparentCutout"
		}

		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			zwrite on

			CGPROGRAM
			
//			#pragma exclude_renderers xbox360
//			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Pos;
			float4 _Str;
			uniform float4 _Color;

			struct v2f {
				float4 pos:SV_POSITION;
//				#ifdef LIGHTMAP_ON
//				float2  uv[2] : TEXCOORD0;
//				#endif
//				#ifdef LIGHTMAP_OFF
				float2  uv[1] : TEXCOORD0;
//				#endif
			};

			v2f vert( appdata_full v ){
				v2f o;

				float dis = distance( v.vertex ,_Pos );

				if( _Str.x > 0 ){
					v.vertex.x = v.vertex.x + sin(dis * _Str.x + _Time.y) * _Str.w;
				}

				if( _Str.y > 0 ){
					v.vertex.y = v.vertex.y + sin(dis * _Str.y + _Time.y) * _Str.w;
				}

				if( _Str.z > 0 ){
					v.vertex.z = v.vertex.z + sin(dis * _Str.z + _Time.y) * _Str.w;
				}

				o.pos = mul( UNITY_MATRIX_MVP, v.vertex );

				o.uv[0] = v.texcoord;

//				#ifdef LIGHTMAP_ON
//				o.uv[1] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//				#endif

				return o;
			}  
						
			half4 frag( v2f i ) : COLOR{
			 	half4 t_tex = tex2D( _MainTex, i.uv[0] );

			 	t_tex *= _Color;

			 	if( t_tex.w <= 0.01 ){
			 		discard;
			 	}

//			 	#ifdef LIGHTMAP_ON
//				t_tex.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv[1]));
//            	#endif

			 	return t_tex;
			}
			ENDCG 
		}
	} 
}
