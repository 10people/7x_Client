#define DEBUG_FX

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.12.10
 * @since:		Unity 5.1.3
 * Function:	Help to manage 3d fx.
 * 
 * Notes:
 * None.
 */ 
public class FxTool {

	public delegate void FxLoadDelegate( GameObject p_fx );

	#region Play Global

	/// Play Fx under Global Coordinate, set pos to Vector3.Zero, and rotation to Identity.
	public static void PlayGlobalFx( string p_fx_path, FxLoadDelegate p_callback = null ){
		PlayGlobalFx( p_fx_path, p_callback, Vector3.zero, Vector3.zero );
	}

	/// Play Fx under Global Coordinate, set pos to p_global_pos, and rotation to p_rot.
	public static void PlayGlobalFx( string p_fx_path, FxLoadDelegate p_callback, Vector3 p_global_pos, Vector3 p_global_rot ){
		FxToLoad t_task = new FxToLoad( p_fx_path, null, p_callback, p_global_pos, p_global_rot );
		
		m_fx_to_load_list.Add( t_task );

		{
			UpdateFx();
		}
	}

	#endregion



	#region Play Local

	/// Play Fx under Local Coordinate, set pos to Vector3.Zero, and rotation to Identity.
	public static void PlayLocalFx( string p_fx_path, GameObject p_parent_gb, FxLoadDelegate p_callback = null ){
		PlayLocalFx( p_fx_path, p_parent_gb, p_callback, Vector3.zero, Vector3.zero );
	}

	/// Play Fx under Local Coordinate, set pos to p_local_pos, and rotation to p_rot.
	public static void PlayLocalFx( string p_fx_path, GameObject p_parent_gb, FxLoadDelegate p_callback, Vector3 p_local_pos, Vector3 p_local_rot ){
		FxToLoad t_task = new FxToLoad( p_fx_path, p_parent_gb, p_callback, p_local_pos, p_local_rot );
		
		m_fx_to_load_list.Add( t_task );

		{
			UpdateFx();
		}
	}

	#endregion



	#region Update

	public static void UpdateFx(){
		UpdateToLoad();
	}

	#endregion



	#region Fx Load List
	
	private static List<FxToLoad> m_fx_to_load_list = new List<FxToLoad>();

	public static void CleanFxToLoad(){
		m_fx_to_load_list.Clear();
	}

	private static void UpdateToLoad(){
		if( m_fx_to_load_list.Count <= 0 ){
			return;
		}
		
		FxToLoad t_task = m_fx_to_load_list[ 0 ];
		
		if( t_task.IsReadyToLoad() ){
			t_task.ExeLoad();
			
			return;
		}
		
		if( !t_task.IsDone() ){
			return;
		}
		
		m_fx_to_load_list.Remove( t_task );
	}

	private class FxToLoad{
		private enum LoadState{
			Ready_To_Load = 0,
			Loading,
			Done,
		}

		private LoadState m_load_state = LoadState.Ready_To_Load;

		private string m_fx_path;

		private GameObject m_parent_gb;

		private Vector3 m_pos;

		private Vector3 m_rot;
		
		private FxLoadDelegate m_call_back;
		
		public FxToLoad( string p_fx_path, GameObject p_parent_gb, FxLoadDelegate p_call_back, Vector3 p_pos, Vector3 p_rot ){
			if( string.IsNullOrEmpty( p_fx_path ) ){
				Debug.LogError( "Error Fx Path IsNullOrEmpty." );
			}
			
			//if( p_call_back == null ){
			//	Debug.LogError( "Error Res Loaded Callback is Null." );
			//}
			
			m_load_state = LoadState.Ready_To_Load;
			
			m_fx_path = p_fx_path;

			m_parent_gb = p_parent_gb;

			m_call_back = p_call_back;

			m_pos = p_pos;
			
			m_rot = p_rot;
		}
		
		public bool IsReadyToLoad(){
			return ( m_load_state == LoadState.Ready_To_Load );
		}
		
		public bool IsDone(){
			return ( m_load_state == LoadState.Done );
		}

		public void ExeLoad(){
			m_load_state = LoadState.Loading;
			
			#if FX_ART_USE && UNITY_EDITOR
			#else
			Global.ResourcesDotLoad( m_fx_path, EffectLoadCallback );
			#endif
		}
		
		public void EffectLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
//			Debug.Log(p_path);

			GameObject t_res_gb = (GameObject)p_object;

			GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );
			
			// effect auto release
			{
				t_gb.AddComponent<ParticleAutoRelease>();
			}
			
			{
				Animator[] t_animators = t_gb.GetComponentsInChildren<Animator>();
				
				for( int i = 0; i < t_animators.Length; i++ ){
					Animator t_anim = t_animators[ i ];
					
					t_anim.applyRootMotion = false;
				}
			}
			
			m_load_state = LoadState.Done;

			{
				Vector3 t_delta_pos = Vector3.zero;
				
				if( m_parent_gb != null ){
					t_gb.transform.parent = m_parent_gb.transform;
				}

				{
					t_gb.transform.localScale = t_res_gb.transform.localScale;

					t_gb.transform.localPosition = m_pos;

					{
						Quaternion t_rot = Quaternion.identity;
						
						t_rot.eulerAngles = m_rot;
						
						t_gb.transform.localRotation = t_rot;
					}
				}
				
				{	
					SoundHelper.PlayFxSound( t_gb, m_fx_path );
				}
			}

			try{
				if(m_call_back != null ){
					m_call_back( t_gb );
				}
				else{
					//Debug.LogError( "call back should not be null." );
				}
			}
			catch( Exception p_e ){
				Debug.LogError( "Error In: " + p_e );
			}
		}
	}

	#endregion
}