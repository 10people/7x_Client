using UnityEngine;
using System.Collections;

public class ShaderHelper {

	#region Replace

	public static void Replace<T>( GameObject p_object, Shader p_origin_shader, Shader p_new_shader ) where T : Renderer{
		if( p_object == null ){
//			Debug.Log( "p_object is null." );

			return;
		}

		if( p_origin_shader == null ){
//			Debug.Log( "p_origin_shader is null." );

			return;
		}

		if( p_new_shader == null ){
//			Debug.Log( "p_new_shader is null." );

			return;
		}

		T[] t_renderers = p_object.GetComponentsInChildren<T>();
		
		for (int i = 0; i < t_renderers.Length; i++)
		{
			T t_renderer = t_renderers[i];
			
			Material[] t_mats = t_renderer.materials;
			
			for ( int j = 0; j < t_renderer.materials.Length; j++ ){
				Material t_mat = t_renderer.materials[ j ];
				
				if ( t_mat == null ){
					continue;
				}
				
//				ComponentHelper.LogMat( t_mat );
				
				if ( t_mat.shader == p_origin_shader ){
//					Debug.Log( "Find and Replace." );
					
					t_mat.shader = p_new_shader;
						
					break;
				}
			}
		}
	}

	#endregion



	#region Const

	#endregion
}
