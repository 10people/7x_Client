Shader "Custom/Effects/StrEffect" {
	Properties {
		_SKColor( "Str Color", Color ) = ( 0.3647, 0.8353, 0.866, 1 )
		
		_Coef( "Coefficient", Range (0.0, 2.0 ) ) = 0.74
	}

	Category {
		SubShader {
			Tags {
				"Queue"="Transparent-8"
				"IgnoreProjector"="True"
				"RenderType"="Opaque"
			}
			
			Pass {
				ZWrite on
				Cull back
				Alphatest Greater 0
				Blend SrcAlpha OneMinusSrcAlpha 
				
				CGPROGRAM  
	           
	            #include "UnityCG.cginc"
	            
	            #pragma vertex vert  
	            #pragma fragment frag  
	            
		        struct v4f {  
		            float4 m_pos : POSITION;  
		            float4 m_color : COLOR;
		            float2 m_uv : TEXCOORD0;
		        };
		        
				uniform float4 _FxColor;
			    
				uniform float4 _Color;
				
				uniform float4 _SKColor;
				
				float _Coef = 1.35f;
				
				sampler2D _MainTex;
				
				static const half m_offset = 0.5f;
		        
		        v4f vert( appdata_base p_v ){  
		            v4f t_out;
		            
		            t_out.m_uv = p_v.texcoord;
		            
		            t_out.m_pos = mul( UNITY_MATRIX_MVP, p_v.vertex );
		            
					float3 t_normal = mul( (float3x3)UNITY_MATRIX_IT_MV, p_v.normal );
					
					t_normal = t_normal / length( t_normal );
				
					float t_dot = length( TransformViewToProjection( t_normal ).xy );
		            
		            if( t_dot < 0 ){
		            	t_dot = -t_dot;
		            }
		            
		            t_dot = t_dot - _Coef;
		            
		            if( t_dot < 0 ){
		            	t_dot = 0;
		            }
		            
					t_out.m_color = t_dot * _SKColor;
					
		            return t_out;  
		        }
		        
		        half4 frag( v4f p_v ) : COLOR{
					return p_v.m_color;
	            }
	            
	            ENDCG
			}
		}
	}
	
	Fallback "Custom/Characters/Main Texture High Light"
}