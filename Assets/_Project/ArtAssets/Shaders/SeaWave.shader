Shader "Custom/SeaWave" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_WaveP("wave: x= height,y=speed,z=size",vector) = (1,1,1,1)
	}
	SubShader {
		Tags {"Queue" = "Transparent +200"}
 		zwrite off
//		ztest less
		blend srcalpha oneminussrcalpha
		pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			half4  _WaveP;
			
			struct vertexinput
			{
				half4 texcoord:TEXCOORD0;
				half4 vertex:POSITION;
			};
			struct v2f
			{
			 	half4  pos: SV_POSITION;
			 	float4  uv: TEXCOORD0;
			};
			v2f vert (vertexinput v)
			{
				v2f o;
 
				o.uv = (v.texcoord+float4(0,frac(_Time.y*_WaveP.y*0.005),0,0))*3;
		 		o.uv .x*=50;
				o.uv.z = 0.75*sin(v.texcoord.y *_WaveP.z-_Time.y*2);
				v.vertex.z += o.uv.z*_WaveP.x;
 
				half cc = (1-abs(v.texcoord.y -0.5)*2);
				half dd = (1-abs(v.texcoord.x -0.5)*2);
// 				half mm =  clamp(o.uv.z,0,1) ;
 				half mm =   o.uv.z*0.5+0.5; 
				o.uv.z = mm *cc*dd;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			half4 frag(v2f i) : COLOR
			{
				
				half tex2 = tex2D(_MainTex, i.uv.xy).r;
				half4 tex1 = half4(1,1,1,1);
				
				
//				tex1.a *=tex2 * i.uv.z*7.6;
				tex1.a *=tex2 * i.uv.z*1.25;

				return tex1;
			}			
			ENDCG
		}
	} 
}
