//#define DEBUG_UI_2D_TOOL


#define ENABLE_OPTIMIZE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.11.26
 * @since:		Unity 4.5.3
 * Function:	Help to manage 2d ui with NGUI.
 * 
 * Notes:
 * 1.UI at 0 is designed for MainCity.UI or any other lowest 2d UIs.
 * 2.for cur design and requirement, all the other layers' ui will be removed if it's the toppest and not visible.
 * 3.if UI.0 is the toppest one, make it visible, if not make it invisible.
 * 4.only update function UI, without pop window.
 */ 
public class UI2DTool : Singleton<UI2DTool>{

	// 0: MainMenu
	// 1: Pop UI
	// 2: Pop UI's sub UI or redirect UI
	private static List<UI2DToolItem> m_2d_manager_list = new List<UI2DToolItem>();


	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		#if ENABLE_OPTIMIZE
		Update2DUI();
		#endif
	}

	void OnDestroy(){
		ClearAll();
	}

	#endregion



	#region 2D UI Management

	/// Clear all references, called when enter new scene
	public static void ClearAll(){
		m_2d_manager_list.Clear();

		ClearCache();
	}

	/// Desc:
	/// Manual add the Toppest UI, and hide MainCityUI.
	/// 
	/// Notice:
	/// 1.p_gb MUST be Function's UI.
	public void AddTopUI( GameObject p_gb ){
		#if !ENABLE_OPTIMIZE
		return;
		#endif

		if( p_gb == null ){
			Debug.LogError( "Error, p_gb is null." );

			return;
		}

		if( !p_gb.activeSelf ){
			Debug.LogError( "Error, p_gb must be active." );

			return;
		}

		if( GetTopUI() != null ){
			if( GetTopUI().GetRootGameObject() == p_gb ){
				Debug.Log( "Already Added." );

				return;
			}
		}

		UI2DToolItem t_manager = new UI2DToolItem( p_gb );

		if( t_manager == null ){
			Debug.LogError( "Error when initialize." );

			return;
		}

		#if DEBUG_UI_2D_TOOL
		Debug.Log( "Show Function UI " + m_2d_manager_list.Count + ": " + 
		          GameObjectHelper.GetGameObjectHierarchy( p_gb ) );
		#endif

		// hide origin top
		if( GetTopUI() != null ){
			if( GetTopUI().GetRootGameObject() != null ){
				GetTopUI().SetActive( false );
			}
		}

		// add new top
		m_2d_manager_list.Add( t_manager );

		// refresh new top
		UpdateTopUI();
	}

//	public static void RemoveUI(){
//		RemoveTopUI();
//	}

	#endregion



	#region Update 2D UI

	// cached top ui
	private static GameObject m_cached_gb		= null;

	private static bool m_cached_visibility		= false;

	private static void ClearCache(){
		m_cached_gb = null;
	}

	public static GameObject GetCachedGameObject(){
		return m_cached_gb;
	}

	private void Update2DUI(){
		if( m_2d_manager_list.Count <= 0 ){
			return;
		}

		UI2DToolItem t_top = GetTopUI();

		if( m_cached_gb == null ){
			UpdateTopUI();

			return;
		}

		if( t_top.GetRootGameObject() == null ){
			RemoveTopUI();

			return;
		}

		if( m_cached_gb != t_top.GetRootGameObject() ){
			UpdateTopUI();

			return;
		}

		if( m_cached_visibility != t_top.GetRootGameObject().activeSelf ){
			UpdateTopUI();

			return;
		}
	}

	/// Remove Top UI, then update new Toppest UI.
	private static void RemoveTopUI(){
		if( m_2d_manager_list.Count <= 0 ){
			Debug.LogError( "Error, out of bound." );

			return;
		}
		else if( m_2d_manager_list.Count == 1 ){
			Debug.LogError( "Never try to remove MainUI." );

			return;
		}

		#if DEBUG_UI_2D_TOOL
		{
			UI2DToolItem t_item = m_2d_manager_list[m_2d_manager_list.Count - 1];
			
			Debug.Log( "Remove Top UI: " + GameObjectHelper.GetGameObjectHierarchy( t_item.GetRootGameObject() ) );
		}
		#endif

		m_2d_manager_list.RemoveAt( m_2d_manager_list.Count - 1 );

		// make new toppest visible
		MakeTopUIVisible();
	}

	private static UI2DToolItem GetTopUI(){
		if( m_2d_manager_list.Count <= 0 ){
//			Debug.LogError( "Error, Top UI is null." );

			return null;
		}

		return m_2d_manager_list[ m_2d_manager_list.Count - 1 ];
	}

	private static void MakeTopUIVisible(){
		if( m_2d_manager_list.Count <= 0 ){
			Debug.LogError( "Error, Top UI doesn't exist." );

			return;
		}

		UI2DToolItem t_ui = GetTopUI();

		if( t_ui.GetRootGameObject() != null ){
			t_ui.SetActive( true );
		}
	}

	/// Update Top UI, if Top UI is invisible, Remove it.
	private static void UpdateTopUI(){
		if( m_2d_manager_list.Count <= 0 ){
			return;
		}
		
		UpdateCachedGameObject();

		if( !m_cached_visibility && m_2d_manager_list.Count > 1 ){
			RemoveTopUI();
		}
	}
	
	private static void UpdateCachedGameObject(){
		// update cached gb
		{
			m_cached_gb = GetTopUI().GetRootGameObject();
		}

		// update cached visibility
		if( m_cached_gb != null ){
			m_cached_visibility = m_cached_gb.activeSelf;
		}
		else{
			m_cached_visibility = false;
		}
	}

	#endregion



	#region Update Main UI

	private static void UpdateMainUI(){
		if( m_2d_manager_list.Count <= 0 ){
			return;
		}

		if( m_2d_manager_list[ 0 ] == null ){
			return;
		}

		m_2d_manager_list[ 0 ].SetActive( !( m_2d_manager_list.Count > 1 ) );
	}

	#endregion



	#region Utilities

	public int GetUICount(){
		return m_2d_manager_list.Count;
	}

	public UI2DToolItem GetUI( int p_index ){
		if( p_index < 0 || p_index >= m_2d_manager_list.Count ){
			Debug.LogError( "Error, out of bound." );

			return null;
		}

		return m_2d_manager_list[ p_index ];
	}

	#endregion

	public class UI2DToolItem{
		private GameObject m_ui_root_gb;

		/// Init 2D Manager with ngui root game object.
		public UI2DToolItem( GameObject p_gb ){
			if( p_gb == null ){
				Debug.LogError( "Error, p_gb = null." );

				return;
			}

			if( p_gb.transform.parent != null ){
				Debug.LogError( "Error, not the root gameobject." );

				return;
			}

			m_ui_root_gb = GameObjectHelper.GetRootGameObject( p_gb );
		}

		public GameObject GetRootGameObject(){
			return m_ui_root_gb;
		}

		public void SetActive( bool p_is_active ){
			if( m_ui_root_gb == null ){
				Debug.LogError( "Error, m_ui_root_gb is null." );

				return;
			}

			m_ui_root_gb.SetActive( p_is_active );
		}
	}
}
