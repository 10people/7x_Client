Shader "Custom/Effects/UIBackground" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader { 
 		Tags { 
	 		"QUEUE"="Transparent-20" 
	 		"IGNOREPROJECTOR"="true" 
	 		"RenderType"="Transparent" 
	 	}
 		
 		Pass {
  			Tags { 
  				"QUEUE"="Transparent"
  				"IGNOREPROJECTOR"="true"
  				"RenderType"="Transparent"
  			}
  			
  			ZTest Off 
  			Cull Off 
  			ZWrite Off 
  			Blend Off
	  		Fog { 
	  			Mode off 
	  		}  
  			
  			CGPROGRAM  
           
            #include "UnityCG.cginc"
            
            #pragma vertex vert  
            #pragma fragment frag  
            
	        struct v4f {  
	            float4 m_pos : POSITION;
	            float2 m_coord : TEXCOORD0;
	        };
	        
	        sampler2D _MainTex;
	        
	        uniform half4 _MainTex_TexelSize;
	        
	        uniform half4 _offset_xy;
	        
	        v4f vert( appdata_base p_v ){  
	            v4f t_out;
	            
	            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
	            
	            t_out.m_coord = p_v.texcoord;
				
	            return t_out;  
	        }
	        
	        static const half COEF[5] = { 0.08, 0.25, 0.35, 0.25, 0.08 };
	        
			half4 frag( v4f p_v ) : COLOR{
				half4 t_tex_frag = 0;
					
				for( int i = 0; i < 5; i++ ){
					t_tex_frag += tex2D( _MainTex, p_v.m_coord.xy + ( i - 2 ) * _offset_xy * _MainTex_TexelSize.xy ) * COEF[ i ];
				}

				return t_tex_frag;
            }  
            
            ENDCG  
  		}
  	}

	FallBack Off
}
