Shader "KY/Distortion"
{
	Properties 
	{
_Distortion("_Distortion", Range(0,0.1) ) = 0
_Tex("_Tex", 2D) = "white" {}

	}
	
	SubShader 
	{
		Tags{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Opaque"
}
GrabPass { }
		
Cull Back
ZWrite Off
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma exclude_renderers d3d11 d3d11_9x
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


float _Distortion;
sampler2D _Tex;
sampler2D _GrabTexture;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float4 screenPos;
float2 uv_Tex;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D1=tex2D(_Tex,(IN.uv_Tex.xyxy).xy);
float4 UnpackNormal0=float4(UnpackNormal(Tex2D1).xyz, 1.0);
float4 Abs0=abs(UnpackNormal0);
float4 Multiply1=Abs0 * _Distortion.xxxx;
float4 Add0=((IN.screenPos.xy/IN.screenPos.w).xyxy) + Multiply1;
float4 Tex2D0=tex2D(_GrabTexture,Add0.xy);
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = Tex2D0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}