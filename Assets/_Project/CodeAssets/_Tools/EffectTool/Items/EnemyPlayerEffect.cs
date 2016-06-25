//#define DEBUG_EFFECT



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.6.3
 * @since:		Unity 5.1.3
 * Function:	Enemy player's effect.
 * 
 * Notes:
 * 1.None.
 */ 
public class EnemyPlayerEffect : MonoBehaviour {

	public Material m_enemy_mat;

	private Renderer[] m_renderers;



	#region Mono

	void Awake(){
		Init();
	}

	#endregion



	#region Init

	private void Init(){
		if( m_enemy_mat == null ){
			Debug.LogError( "Error, crush mat is null." );
		}

		{
			m_renderers = gameObject.GetComponentsInChildren<Renderer>();
		}
	}

	#endregion



	#region Use

	private Renderer m_target_renderer = null;

	public void UseEnemyEffect(){
		#if DEBUG_EFFECT
		Debug.Log( "UseCrushMat()" );
		#endif

		if( m_enemy_mat == null ){
			Debug.LogError( "Error, crush mat is null." );
		}

		{
			Material t_target_mat = new Material( m_enemy_mat );
		
			m_target_renderer = UpdateMat( t_target_mat );
		}
	}

	#endregion



	#region Utilities

	private Renderer UpdateMat( Material p_mat ){
		if( m_renderers == null ){
			return null;
		}

		Renderer t_target = null;

		for( int i = 0; i < m_renderers.Length; i++ ){
			Renderer t_render = m_renderers[ i ];

			if( t_render == null ){
				continue;
			}

			Material[] t_mats = MaterialHelper.CloneMaterials( t_render );

			for( int j = 0; j < t_mats.Length; j++ ){
				Material t_mat = t_mats[ j ];

				if( t_mat == null ){
					continue;
				}

				if( t_mat.mainTexture == p_mat.mainTexture ){
					#if DEBUG_EFFECT
					Debug.Log( "Use new mat at: " + GameObjectHelper.GetGameObjectHierarchy( t_render.gameObject  ) );

					Debug.Log( "Rednerer: " + t_render );

					Debug.Log( "New mat: " + p_mat );
					#endif

					t_mats[ j ] = p_mat;

					t_target = t_render;
				}
			}

			t_render.materials = t_mats;
		}

		return t_target;
	}

	#endregion
}
