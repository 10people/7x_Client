Shader "Custom/Objects/Main Texture Cutout" {
	Properties {
		_Color( "Main Color", Color ) = ( 0.537, 0.537, 0.537, 1 )
		_MainTex( "Base (RGB) Trans (A)", 2D ) = "white" {}
	}

	SubShader {
		Tags {
            "RenderType"="Opaque"
        }
        
        Pass {
            
            Tags {
                "LightMode"="ForwardBase"
            }
            
//            AlphaTest Greater 0.01
			Blend SrcAlpha OneMinusSrcAlpha 
				
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
			#if defined(SHADER_API_D3D9)
			#pragma target 3.0
			#else
			
			#endif
            uniform sampler2D _MainTex;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
	            #endif
	            o.normalDir = UnityObjectToWorldNormal(v.normal);
	            o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
	            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
	            o.posWorld = mul(_Object2World, v.vertex);
	            float3 lightColor = _LightColor0.xyz;
	            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	            UNITY_TRANSFER_FOG(o,o.pos);
	            TRANSFER_VERTEX_TO_FRAGMENT(o)
	            return o;
	        }
	        
	        float4 frag(VertexOutput i) : COLOR {
	            i.normalDir = normalize(i.normalDir);
	            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
	            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	            float3 normalDirection = i.normalDir;
	            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	            float3 lightColor = _LightColor0.xyz;
	            float attenuation = LIGHT_ATTENUATION(i);
	            float3 attenColor = attenuation * _LightColor0.xyz;
	            UnityLight light;
	            #ifdef LIGHTMAP_OFF
	                light.color = lightColor;
	                light.dir = lightDirection;
	                light.ndotl = LambertTerm (normalDirection, light.dir);
	            #else
	                light.color = half3(0.f, 0.f, 0.f);
	                light.ndotl = 0.0f;
	                light.dir = half3(0.f, 0.f, 0.f);
	            #endif
	            UnityGIInput d;
	            d.light = light;
	            d.worldPos = i.posWorld.xyz;
	            d.worldViewDir = viewDirection;
	            d.atten = attenuation;
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
	            UnityGI gi = UnityGlobalIllumination (d, 1, 0, normalDirection);
	            
	            float4 t_tex = tex2D(_MainTex, i.uv0);
	            
	            float4 t_color = float4( gi.indirect.diffuse, 1 ) * t_tex;
	            
	            return t_color;
	        }
	        ENDCG
	    }
	}
	
	Fallback "Diffuse"
}