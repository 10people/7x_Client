Shader "Custom/Effects/SceneBackground" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightColor( "Light Color", Color ) = ( 0, 0, 0, 0 )
		_DarkColor( "Dark Color", Color ) = ( 0, 0, 0, 0 )
	}
	
	SubShader { 
 		Pass {
  			
  			CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct v4f {
	        	float4 m_pos : POSITION;
	            float2 m_coord : TEXCOORD0;
	        };
	        
	        sampler2D _MainTex;
	        
	        uniform float4 _LightColor;

	        uniform float4 _DarkColor;
			
	        v4f vert( appdata_base p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );

	            t_out.m_coord = p_v.texcoord;
				
	            return t_out;  
	        }
	        
	       half4 frag( v4f p_v ) : COLOR{
				half4 t_tex_frag = 0;
				
				t_tex_frag = tex2D( _MainTex, p_v.m_coord.xy );

				t_tex_frag.xyz = t_tex_frag.xyz + _LightColor.xyz - _DarkColor.xyz;

				return t_tex_frag;
            }  
            
            ENDCG  
  		}
  	}

	FallBack Off
}
