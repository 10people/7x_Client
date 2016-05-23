//#define DEBUG_FX_CACHE

//#define DEBUG_FX_CREATE



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
public class FxHelper {

	#region Fx Cache

	private static Dictionary<string, FxCacheContainer> m_freed_fx_gb_dict = new Dictionary<string, FxCacheContainer>();

	#if UNITY_EDITOR
	public static Dictionary<string, FxCacheContainer> GetFreedFxGbDict(){
		return m_freed_fx_gb_dict;
	}
	#endif

	public static void CleanCachedFx(){
		#if DEBUG_FX_CACHE
		Debug.Log( "CleanCachedFx" );
		#endif

		foreach( KeyValuePair<string, FxCacheContainer> t_kv in m_freed_fx_gb_dict ){
			FxCacheContainer t_list = t_kv.Value;

			for( int i = t_list.m_free_gb_list.Count - 1; i >= 0; i-- ){
				GameObject t_gb = t_list.m_free_gb_list[ i ];

				if( t_gb != null ){
					GameObject.Destroy( t_gb );
				}
			}

			t_list.Clear();
		}

		m_freed_fx_gb_dict.Clear();
	}

	public static GameObject InstantiateFxGameObject( string p_fx_path, GameObject p_gb ){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_gb );	

		#if DEBUG_FX_CACHE || DEBUG_FX_CREATE
		Debug.Log( "no freed Fx, instantiate One: " + p_fx_path + " - " + t_gb );
		#endif

		FxCacheContainer t_container = GetOrCreateContainer( p_fx_path );

		t_container.Created();

		FxCacheItem.AutoFxCache( p_fx_path, t_gb );

		return t_gb;
	}

	/// May return null, is no free fx exist.
	private static GameObject ObtainFreeFxGameObject( string p_fx_path ){
		if( string.IsNullOrEmpty( p_fx_path ) ){
			Debug.LogError( "fx path is null: " + p_fx_path );

			return null;
		}

		if( !m_freed_fx_gb_dict.ContainsKey( p_fx_path ) ){
			return null;
		}

		FxCacheContainer t_container = m_freed_fx_gb_dict[ p_fx_path ];

		if( t_container.m_free_gb_list.Count <= 0 ){
			return null;
		}

		GameObject t_gb = t_container.m_free_gb_list[ 0 ];

		t_container.m_free_gb_list.RemoveAt( 0 );

		#if DEBUG_FX_CACHE
		Debug.Log( "Obtain Fx Cache: " + p_fx_path + " - " + t_gb );
		#endif

		return t_gb;
	}

	/// get cached or create new fx.gb.
	public static GameObject ObtainFreeFxGameObject( string p_fx_path, GameObject p_gb ){
		{
			if( string.IsNullOrEmpty( p_fx_path ) ){
				Debug.LogError( "fx path is null: " + p_fx_path );

				return null;
			}

			if( p_gb == null ){
				Debug.LogError( "GameObject is null: " + p_gb );

				return null;
			}
		}

		GameObject t_gb = FxHelper.ObtainFreeFxGameObject( p_fx_path );

		if( t_gb == null ){
			t_gb = InstantiateFxGameObject( p_fx_path, p_gb );
		}

		{
			FxCacheItem t_fx_cache = t_gb.GetComponent<FxCacheItem>();

			if( t_fx_cache == null ){
				Debug.LogError( "ObtainFreeFxGameObject.Error, Not a complete Cached Item: " + p_gb );
			}
		}

		return t_gb;
	}

	public static FxCacheContainer GetOrCreateContainer( string p_fx_path ){
		if( !m_freed_fx_gb_dict.ContainsKey( p_fx_path ) ){
			FxCacheContainer t_temp = new FxCacheContainer();

			m_freed_fx_gb_dict.Add( p_fx_path, t_temp );
		}

		FxCacheContainer t_container = m_freed_fx_gb_dict[ p_fx_path ];

		return t_container;
	}

	public static void FreeFxGameObject( string p_fx_path, GameObject p_gb ){
		{
			if( p_gb == null ){
				Debug.LogError( "GameObject is null: " + p_gb );

				return;
			}

			if( string.IsNullOrEmpty( p_fx_path ) ){
				Debug.LogError( "fx path is null: " + p_fx_path );

				return;
			}
		}

		{
			GetOrCreateContainer( p_fx_path );
		}

		{
			FxCacheItem t_fx_cache = p_gb.GetComponent<FxCacheItem>();

			if( t_fx_cache == null ){
				Debug.LogError( "FreeFxGameObject.Error, Not a complete Cached Item: " + p_gb );
			}
		}

		FxCacheContainer t_list = m_freed_fx_gb_dict[ p_fx_path ];

		if( t_list.m_free_gb_list.Contains( p_gb ) ){
//			#if UNITY_EDITOR
//			Debug.Log( "Already contained: " + p_gb );
//			#endif

			return;
		}

		p_gb.transform.parent = GameObjectHelper.GetTempFxGameObjectRoot().transform;

		p_gb.SetActive( false );

		t_list.m_free_gb_list.Add( p_gb );

		#if DEBUG_FX_CACHE
		Debug.Log( "Free Fx: " + p_fx_path + " - " + p_gb );
		#endif
	}

	public class FxCacheContainer{
		public List<GameObject> m_free_gb_list = new List<GameObject>();

		public int m_created_count = 0;

		public void Created(){
			m_created_count++;
		}

		public void Clear(){
			m_free_gb_list.Clear();
		}
	}

	#endregion



	#region Play Global

	public delegate void FxLoadDelegate( GameObject p_fx );

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



	#region Utilities

	public static bool IsFxAutoRelease( GameObject p_gb ){
		return ParticleAutoRelease.IsAutoReleaseEnabled( p_gb );
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

		int t_count = m_fx_to_load_list.Count;

		for( int i = t_count - 1; i >= 0; i-- ){
			FxToLoad t_task = m_fx_to_load_list[ i ];

			if( t_task.IsReadyToLoad() ){
				t_task.ExeLoad();

				return;
			}

			if( !t_task.IsDone() ){
				return;
			}

			m_fx_to_load_list.Remove( t_task );
		}
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