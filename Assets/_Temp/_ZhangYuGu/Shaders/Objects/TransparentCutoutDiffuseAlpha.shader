Shader "Legacy Shaders/Transparent/Cutout/Diffuse Alpha" {
	Properties {
		_Tran( "Transparency", Range ( 0.0, 1.0 ) ) = 1.0
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff

		float _Tran;
		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.xyz;

			o.Alpha = c.a;

			if( o.Albedo.x + o.Albedo.y + o.Albedo.z - ( 1 - _Tran ) * 3 < 0 || c.a <= 0 ){
				discard;
			}
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
