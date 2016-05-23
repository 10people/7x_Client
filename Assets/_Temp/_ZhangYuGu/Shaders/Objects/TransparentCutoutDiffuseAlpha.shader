Shader "Legacy Shaders/Transparent/Cutout/Diffuse Alpha" {
	Properties {
		_Tran( "Transparency", Range ( 0.0, 1.0 ) ) = 1.0
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader {
		Tags {"Queue" = "Transparent" "IgnoreProjector"="True"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf CustomLam alpha

		float _Tran;
		float _Cutoff;
		sampler2D _MainTex;
		fixed4 _Color;

		half4 LightingCustomLam(SurfaceOutput s, half3 lightDir, half atten) {
		  half NdotL = dot (s.Normal, lightDir);

		  half4 c;

		  c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);

		  if( s.Alpha < _Cutoff ){
		  	c.a = 0;
		  }
		  else{
		  	c.a = s.Albedo * _Tran;
		  }

		  return c;
		}

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;

			if( c.a < _Cutoff ){
				o.Alpha = 0;
			}
			else{
				o.Alpha = c.a * _Tran;
			}
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
