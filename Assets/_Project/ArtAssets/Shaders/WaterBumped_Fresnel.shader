Shader "Custom/Water Reflective Bumped (Fog)" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" { }
		_FogFactor ("Fog Factor", Range(100, 3000)) = 100
		_FresnelFactor ("Fresnel Factor", Range(0.1, 100)) = 6
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite on Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert alpha approxview
		#pragma target 3.0
		
		sampler2D _BumpMap;
		samplerCUBE _Cube;
		
		fixed4 _Color;
		fixed4 _ReflectColor;
		half _FogFactor;
		half _FresnelFactor;
		
		struct Input {
			float2 uv_BumpMap;
			float3 viewDir;
			float3 worldRefl; 
			float3 worldPos;
			INTERNAL_DATA
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;	
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		
			// Calculating reflection vector
			half3 worldRefl = WorldReflectionVector (IN, o.Normal);
			fixed4 reflcol = texCUBE (_Cube, worldRefl);
		
			// Fresnel
			half rim = pow(1.0 - saturate(dot (normalize(IN.viewDir), o.Normal)), _FresnelFactor);
			//  Fog
			half fogDepth = distance(_WorldSpaceCameraPos, mul((float3x3) _Object2World, IN.worldPos)) / _FogFactor;
		
			o.Emission = reflcol.rgb * _ReflectColor.rgb * _ReflectColor.rgb;
			o.Alpha = length(reflcol.rgb) * _ReflectColor.a * _Color.a + rim + fogDepth;
		}
		ENDCG
	}
	
	FallBack "Reflective/Bumped Specular"
}
