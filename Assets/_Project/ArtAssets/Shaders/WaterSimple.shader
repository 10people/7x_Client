Shader "Custom/Water Simple" {
   Properties {
        _MainTex ("Water Texture", 2D) = "white" {}
        _FoamTex ("Foam Texture", 2D) = "black" {}
        _Alpha ("Water alpha", Range (0.0, 1.0)) = 0.7
        _FoamAddAlpha ("Foam add alpha", Range (0.0, 1.0)) = 0.7
        _FoamFactor ("Foam factor", Range (0.0, 3.0)) = 0.3
        _FoamScale ("Foam scale", Range (0.1, 5.0)) = 2
        _TimeFactor ("Time Factor", Range(0.1, 10)) = 3
   }

   Category {
       Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" }
       
       Blend SrcAlpha OneMinusSrcAlpha
       Cull Back Lighting Off Fog { Color (0,0,0,0) }
       ZWrite On
       
       SubShader {
           // draw after all opaque geometry has been drawn
           Pass {
              Name "BASE"
       	   
              CGPROGRAM
              #pragma fragmentoption ARB_precision_hint_fastest
              #pragma vertex vert 
              #pragma fragment frag
       	   
              #include "UnityCG.cginc"
              
              struct appdata_t {
                 float4 vertex : POSITION;
                 float3 normal : NORMAL;
                 fixed4 color : COLOR;
                 half2 texcoord : TEXCOORD0;
              };
       	   
              struct v2f {
                 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 half3 texcoord : TEXCOORD0;
                 half3 texcoord1 : TEXCOORD1;
              };
              
              sampler2D _MainTex;
              sampler2D _FoamTex;
              float4 _MainTex_ST;
              fixed _Alpha;
              fixed _FoamAddAlpha;
              fixed _FoamFactor;
              half _FoamScale;
              half _TimeFactor;
       	   
              inline fixed triangle(half t, half a) {
                return abs(2 * (t/a - floor(t/a + 0.5f)));
              }
       	   
              v2f vert (appdata_t v)
              {
                 v2f o;
                 o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
       	   
                 o.color.xyz = v.color.xyz;
                 o.color.w = _FoamAddAlpha + clamp(length(v.normal.xz) * _FoamFactor, 0, 1);
                 
                 o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                 o.texcoord.z = _Alpha + o.color.w;
       	   
                 o.texcoord1.xy = o.texcoord * _FoamScale;	
                 // Texture animation
                 o.texcoord1.z = triangle(_Time.y, _TimeFactor);
       	   
                 return o;
              }
              
              fixed4 frag (v2f i) : COLOR
              {
                 fixed4 texColor;
                 texColor.xyz = tex2D(_MainTex, 1f - i.texcoord.xy).xyz * (1f - i.color.w) * i.texcoord1.z + 
                                tex2D(_MainTex, i.texcoord.xy).xyz * (1f - i.color.w) * (1f - i.texcoord1.z) + 
                                tex2D(_FoamTex, i.texcoord1.xy).xyz * i.color.w;
                 texColor.w = i.texcoord.z;
       	   
                 return texColor;
              }
       	   
              ENDCG  
          }
       }
   }
}