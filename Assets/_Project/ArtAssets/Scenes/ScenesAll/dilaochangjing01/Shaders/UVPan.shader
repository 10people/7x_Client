Shader "M/UVPan" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpeedU("SpeedU", float) = 1
		_SpeedV("SpeedV", float) = 1
		
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _SpeedU;
		float _SpeedV;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex.xy + float2(_SpeedU, _SpeedV ) * _Time.x);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG  
	} 
	FallBack "Diffuse"
}
