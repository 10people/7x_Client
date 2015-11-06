Shader "T4MShaders/ShaderModel2/Diffuse/T4M 4 Textures" {
    Properties {
        _Splat0 ("_Splat0", 2D) = "white" {}
        _Splat1 ("_Splat1", 2D) = "white" {}
        _Splat2 ("_Splat2", 2D) = "white" {}
        _Splat3 ("_Splat3", 2D) = "white" {}
        _Control ("_Control", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
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
            uniform sampler2D _Control; uniform float4 _Control_ST;
            uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
            uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
            uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
            uniform sampler2D _Splat3; uniform float4 _Splat3_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
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
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
	            #endif
	            #ifdef DYNAMICLIGHTMAP_ON
	                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	            #endif
	            o.normalDir = UnityObjectToWorldNormal(v.normal);
	            o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
	            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
	            o.posWorld = mul(_Object2World, v.vertex);
	            float3 lightColor = _LightColor0.rgb;
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
	            float3 lightColor = _LightColor0.rgb;
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
	            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	                d.ambient = 0;
	                d.lightmapUV = i.ambientOrLightmapUV;
	            #else
	                d.ambient = i.ambientOrLightmapUV;
	            #endif
	            UnityGI gi = UnityGlobalIllumination (d, 1, 0, normalDirection);
	            lightDirection = gi.light.dir;
	            lightColor = gi.light.color;
	            float NdotL = max(0.0,dot( normalDirection, lightDirection ));
	            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
	            float3 indirectDiffuse = float3(0,0,0);
	            indirectDiffuse += gi.indirect.diffuse;
	            float4 _Splat0_var = tex2D(_Splat0,TRANSFORM_TEX(i.uv0, _Splat0));
	            float4 _Control_var = tex2D(_Control,TRANSFORM_TEX(i.uv0, _Control));
	            float4 _Splat1_var = tex2D(_Splat1,TRANSFORM_TEX(i.uv0, _Splat1));
	            float4 _Splat2_var = tex2D(_Splat2,TRANSFORM_TEX(i.uv0, _Splat2));
	            float4 _Splat3_var = tex2D(_Splat3,TRANSFORM_TEX(i.uv0, _Splat3));
	            float3 diffuseColor = (((_Splat0_var.rgb*_Control_var.r)+(_Control_var.g*_Splat1_var.rgb))+((_Control_var.b*_Splat2_var.rgb)+(_Control_var.a*_Splat3_var.rgb)));
	            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
	            float3 finalColor = diffuse;
	            fixed4 finalRGBA = fixed4(finalColor,1);
	            UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
	            return finalRGBA;
	        }
	        ENDCG
	    }
	    
	    Pass {
	        Name "FORWARD_DELTA"
	        Tags {
	            "LightMode"="ForwardAdd"
	        }
	        Blend One One
	        
	        
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #define UNITY_PASS_FORWARDADD
	        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
	        #include "UnityCG.cginc"
	        #include "AutoLight.cginc"
	        #include "Lighting.cginc"
	        #include "UnityPBSLighting.cginc"
	        #include "UnityStandardBRDF.cginc"
	        #pragma multi_compile_fwdadd_fullshadows
	        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
	        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
	        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
	        #pragma multi_compile_fog
	        #if defined(SHADER_API_D3D9)
			#pragma target 3.0
			#else
				
			#endif
	        uniform sampler2D _Control; uniform float4 _Control_ST;
	        uniform sampler2D _Splat0; uniform float4 _Splat0_ST;
	        uniform sampler2D _Splat1; uniform float4 _Splat1_ST;
	        uniform sampler2D _Splat2; uniform float4 _Splat2_ST;
	        uniform sampler2D _Splat3; uniform float4 _Splat3_ST;
	        struct VertexInput {
	            float4 vertex : POSITION;
	            float3 normal : NORMAL;
	            float4 tangent : TANGENT;
	            float2 texcoord0 : TEXCOORD0;
	            float2 texcoord1 : TEXCOORD1;
	            float2 texcoord2 : TEXCOORD2;
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
	        };
	        VertexOutput vert (VertexInput v) {
	            VertexOutput o = (VertexOutput)0;
	            o.uv0 = v.texcoord0;
	            o.uv1 = v.texcoord1;
	            o.uv2 = v.texcoord2;
	            o.normalDir = UnityObjectToWorldNormal(v.normal);
	            o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
	            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
	            o.posWorld = mul(_Object2World, v.vertex);
	            float3 lightColor = _LightColor0.rgb;
	            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	            TRANSFER_VERTEX_TO_FRAGMENT(o)
	            return o;
	        }
	        float4 frag(VertexOutput i) : COLOR {
	            i.normalDir = normalize(i.normalDir);
	            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
	            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	            float3 normalDirection = i.normalDir;
	            float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
	            float3 lightColor = _LightColor0.rgb;
	            float attenuation = LIGHT_ATTENUATION(i);
	            float3 attenColor = attenuation * _LightColor0.xyz;
	            float NdotL = max(0.0,dot( normalDirection, lightDirection ));
	            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
	            float4 _Splat0_var = tex2D(_Splat0,TRANSFORM_TEX(i.uv0, _Splat0));
	            float4 _Control_var = tex2D(_Control,TRANSFORM_TEX(i.uv0, _Control));
	            float4 _Splat1_var = tex2D(_Splat1,TRANSFORM_TEX(i.uv0, _Splat1));
	            float4 _Splat2_var = tex2D(_Splat2,TRANSFORM_TEX(i.uv0, _Splat2));
	            float4 _Splat3_var = tex2D(_Splat3,TRANSFORM_TEX(i.uv0, _Splat3));
	            float3 diffuseColor = (((_Splat0_var.rgb*_Control_var.r)+(_Control_var.g*_Splat1_var.rgb))+((_Control_var.b*_Splat2_var.rgb)+(_Control_var.a*_Splat3_var.rgb)));
	            float3 diffuse = directDiffuse * diffuseColor;
	            float3 finalColor = diffuse;
	            return fixed4(finalColor * 1,0);
	        }
	        ENDCG
	    }
	}

Fallback "Diffuse"
}
