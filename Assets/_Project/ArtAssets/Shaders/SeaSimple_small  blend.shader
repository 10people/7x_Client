Shader "Custom/SeaSimple_small_blend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorTex ("color (RGB)", 2D) = "white" {}
		_wave_length("_wave_length",float) =1
		_wave_height("_wave_height",float) =1
		_Emission("_Emisson",float) =1
 
	}
	SubShader {
		Tags {"Queue" = "Transparent+200"}
 		zwrite off
 		ztest less
//		blend srcalpha one
		blend srcalpha oneminussrcalpha
		pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			uniform float4 _MainTex_ST; 
			uniform float4 _ColorTex_ST; 

			sampler2D _ColorTex;
			half _wave_height;
			half _wave_length;
			half _Emission;
 
			
			struct vertexinput
			{
				half4 texcoord:TEXCOORD0;
				half4 vertex:POSITION;
			};
			struct v2f
			{
			 	half4  pos: SV_POSITION;
			 	half4  uv: TEXCOORD0;
			 	float4  uv2: TEXCOORD1;
 
			};
			v2f vert (vertexinput v)
			{
				v2f o;
				o.uv = v.texcoord;
				o.uv.w = clamp((-abs(o.uv.x-0.5)*2+1)*5,0,1);
				float cc = frac(_Time.y*0.01);
				o.uv2 = (v.texcoord+float4(cc,cc,0,0))*5;
 				o.uv2.xy *= _MainTex_ST.xy;	
				o.uv.z = sin(v.vertex.x *_wave_length+frac(_Time.x*4)*6.2831852);
				  
				v.vertex.z += o.uv.z*_wave_height;
//				o.uv.z = o.uv.z*0.5+0.5;
//				o.uv.z*= o.uv.w;
				o.uv.xy *= _ColorTex_ST.xy;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
  
				return o;
			}
			half4 frag(v2f i) : COLOR
			{
				half4 tex1 = tex2D(_MainTex, i.uv2.xy);
				half4 tex2 = tex2D(_ColorTex, i.uv.xy);
//				tex1.rgb*=tex1.a;
				
				tex1.a = tex1.r;
				tex1.rgb =0.86;
				tex1.a = (tex1.a * tex2.r*_Emission + tex2.r*0.2)*i.uv.w;
 

				return tex1;
			}			
			ENDCG
		}
	} 
}
