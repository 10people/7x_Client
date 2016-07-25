Shader "M/Water" {
	Properties {
		_Color1 ("Color1", Color) = (0.0824, 0.219,0.116,1)
		_Color2 ("Color2", Color) = (0.05, 0.171, 0.219, 1)
		_RefMap("Reflection Map", 2D) = "white" {}
		_NormalMap("Normal map", 2D) = "white" {}
		_RippleAmount("Ripple amount", Float) = 0.5
		_Speed("Speed", Vector) = (0.02, 0.02, 0.02, 0.02)
		_FresnelPower("Fresnel Power", Float) = 2
	}
	SubShader {
		Tags { "Queue"="Geometry+1" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 200
		
		Pass {
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			
			sampler2D _NormalMap;
			sampler2D _RefMap;

			float4 _Color1;
			float4 _Color2;
			float4 _RefMap_ST;
			float4 _NormalMap_ST;
			float _RippleAmount;
			float4 _Speed;
			float _FresnelPower;

 
			struct appdata {
			   float4 vertex : POSITION0;
			   float3 Normal: NORMAL;
			   float2 Texcoord : TEXCOORD0;
			};
			
			struct v2f {
			   float4 pos : SV_POSITION;
			   float4 WorldPos : TEXCOORD0;
			   float3 WorldNormal : TEXCOORD1;
			   float2 Texcoord : TEXCOORD2;
			};

			
			float fresnel (float3 light, float3 normal, float R0, float p)
			 {
 
				 float cosAngle = 1 - saturate(dot(light, normal));

				 float result = cosAngle * cosAngle;
						 result = result * result;
						 result = result * cosAngle;
						 result = saturate(result * (1 - saturate(R0)) + R0);


				 return pow(result, p);
			 }

			float4 desaturation(float4 c, float k)
			{
				float f = dot(c.xyz, float3(0.3, 0.59, 0.11));
				float3 cc = lerp(c.xyz, f.xxx, k);
     
				return float4(cc, c.w);
			}

			float3 calcNormal(float3 worldNormal, float2 texcoord)
			{
				
			   float2 p1 = texcoord + float2(0.484, 0.867);
			   float2 p2 = texcoord + float2(0.685, 0.447);
			   float2 p3 = texcoord + float2(0.0954, 0.04);
			   float2 p4 = texcoord;
   
				
				
   
			   p1.y -= _Speed.x * _Time.y;
			   p2.y += _Speed.y * _Time.y;
			   p3.x -= _Speed.z * _Time.y;
			   p4.x -= _Speed.w * _Time.y;
   
			   p1 *= 2;
			   p2 *= 2;
			   p3 *= 2;
			   p4 *= 2;

			   float3 n1 = tex2D(_NormalMap, p1);
			   float3 n2 = tex2D(_NormalMap, p2);
			   //float3 n3 = tex2D(_NormalMap, p3);
			   //float3 n4 = tex2D(_NormalMap, p4); 
   
			   float3 n = n1 + n2;// + n3 + n4;
			   
			   //n = n * 2 - 4;
			   n = n * 2 - 2;

			   //n *= 0.5;
				
    

   
   
			   n = lerp(n, float3(0, 1, 0), _RippleAmount);
   
    
			   return n;
			}

 
			v2f vert (appdata v)
			{ 
			   v2f Output;

			   Output.pos = mul( UNITY_MATRIX_MVP, v.vertex  );
			   Output.WorldPos = mul(_Object2World, v.vertex);
			   Output.WorldNormal = normalize(mul(_Object2World, float4(v.Normal, 0)));
			   Output.Texcoord = TRANSFORM_TEX(v.Texcoord, _NormalMap);
			   return( Output );
			} 

			float3 mReflect(float3 eye, float3 norm)
			{
				return eye - 2 * norm * dot(eye, norm);
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				float3 n = normalize(i.WorldNormal);
				float3 v = normalize(i.WorldPos.xyz - _WorldSpaceCameraPos.xyz);
				float3 norm = calcNormal(n, i.Texcoord);
				float3 ref = normalize(mReflect(v, norm));
				//float4 c = texCUBE(_Env, ref.xzy);
				float2 texCoord = ref.xz * 0.5 + 0.5;
				float4 c = tex2D(_RefMap, TRANSFORM_TEX(texCoord, _RefMap));
				 
				return c;
				c = desaturation(c, 0.7);
				
				float4 c1 = _Color1 + c;
				float4 c2 = _Color2 + c;
				float fres = fresnel(-v, n, 0.018, _FresnelPower);
				c = lerp(c1, c2, fres);
     

				return c;

			}
			 
			ENDCG 
		}
	} 
}
