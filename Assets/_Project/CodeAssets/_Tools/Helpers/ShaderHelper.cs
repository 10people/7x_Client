//#define DEBUG_SHADER

using UnityEngine;
using System.Collections;

public class ShaderHelper {

	#region Replace

	public static void ReplaceAll<T>( GameObject p_object, Shader p_origin_shader, Shader p_new_shader ) where T : Renderer{
		if( p_object == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_object is null." );
			#endif
			
			return;
		}
		
		if( p_origin_shader == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_origin_shader is null." );
			#endif
			
			return;
		}
		
		if( p_new_shader == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_new_shader is null." );
			#endif
			
			return;
		}
		
		T[] t_renderers = p_object.GetComponentsInChildren<T>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}
				
				#if DEBUG_SHADER
				ComponentHelper.LogMaterial( t_mat );
				#endif
				
				if ( t_mat.shader == p_origin_shader ){
					#if DEBUG_SHADER
					Debug.Log( "Find and Replace " + j + " : " + t_mat.shader );
					#endif
					
					t_mat.shader = p_new_shader;
				}
				else{
					#if DEBUG_SHADER
					Debug.Log( "Skip " + j + " : " + t_mat.shader );

					Debug.Log( "target: " + p_origin_shader );
					#endif
				}
			}
		}
	}

	public static Material Replace<T>( GameObject p_object, Shader p_origin_shader, Shader p_new_shader ) where T : Renderer{
		if( p_object == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_object is null." );
			#endif

			return null;
		}

		if( p_origin_shader == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_origin_shader is null." );
			#endif

			return null;
		}

		if( p_new_shader == null ){
			#if DEBUG_SHADER
			Debug.Log( "p_new_shader is null." );
			#endif

			return null;
		}

		T[] t_renderers = p_object.GetComponentsInChildren<T>();
		
		for( int i = 0; i < t_renderers.Length; i++ ){
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}

				#if DEBUG_SHADER
				ComponentHelper.LogMaterial( t_mat );
				#endif

				if ( t_mat.shader == p_origin_shader ){
					#if DEBUG_SHADER
					Debug.Log( "Find and Replace." );
					#endif

					t_mat.shader = p_new_shader;
						
					return t_mat;
				}
			}
		}

		return null;
	}

	#endregion



	#region Const

	#endregion
}
