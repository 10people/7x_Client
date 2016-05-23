Shader "effect/Unlit" 
{ 
 Properties 
 { 
 _MainTex ("Main Texture", 2D) = "white" {} 
 _QOffset ("Offset", Vector) = (0,0,0,0) 
 _Dist ("Distance", Float) = 100.0 
 } 
 SubShader 
 { 
 Tags { "Queue" = "Geometry" } 
 Pass 
 { 
 CGPROGRAM 
 #pragma exclude_renderers ps3 xbox360 flash 
 #pragma fragmentoption ARB_precision_hit_fastest 
  
 #pragma vertex vert 
 #pragma fragment frag 
  
 #include "UnityCG.cginc" 


  
 uniform sampler2D _MainTex; 
 uniform float4 _MainTex_ST; 
  
 uniform float4 _QOffset; 
 uniform float _Dist; 
  
 struct vertexInput 
 { 
 float4 vertex : POSITION; 
 float4 texcoord : TEXCOORD0; 
  
 }; 
  
 struct fragmentInput 
 { 
 float4 pos : SV_POSITION; 
 half2 uv : TEXCOORD0; 
  
 }; 
  
 fragmentInput vert( vertexInput i ) 
 { 
  
 fragmentInput o; 
     float4 vPos = mul (UNITY_MATRIX_MV, i.vertex); 
     float zOff = vPos.z*_Dist; 
     vPos += _QOffset*zOff*zOff; 
     o.pos = mul (UNITY_MATRIX_P, vPos); 
     
  
 o.uv = TRANSFORM_TEX(i.texcoord, _MainTex); 
  
  
 return o; 
 } 
  
 half4 frag( fragmentInput i ):COLOR 
 { 
 return tex2D( _MainTex,i.uv); 
 } 
  
 ENDCG 
 } 
 } 
 FallBack "Diffuse" 
}