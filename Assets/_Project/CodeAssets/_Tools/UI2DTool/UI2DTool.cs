//#define DEBUG_UI_2D_TOOL



#define SHOW_UIBGEF_ALL_THE_TIME

//#define ENABLE_OPTIMIZE



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

	/// cached ui gbs.
	private static List<GameObject> m_2d_ui_gb_list = new List<GameObject>();

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Update2DUI();
	}

	void OnDestroy(){
		ClearAll();

		base.OnDestroy();
	}

	#endregion



	#region 2D UI Management

	/// Clear all references, called when enter new scene
	public static void ClearAll(){
		m_2d_manager_list.Clear();

		m_2d_ui_gb_list.Clear();

		ClearCache();
	}

	private void AddToTop( UI2DToolItem p_manager ){
		// add new top
		{
			m_2d_manager_list.Add( p_manager );
			
			p_manager.GetRootGameObject().transform.position = GetUIRootPos( m_2d_manager_list.Count );

		}

		// cache ui gb
		{
			if( !m_2d_ui_gb_list.Contains( p_manager.GetRootGameObject() ) ){
				m_2d_ui_gb_list.Add( p_manager.GetRootGameObject() );
			}
		}
	}

	private static void UpdateCachedUIGameObject(){
		for( int i = m_2d_ui_gb_list.Count - 1; i >= 0; i-- ){
			if( m_2d_ui_gb_list[ i ] == null ){
				m_2d_ui_gb_list.RemoveAt( i );
			}
		}
	}

	public static GameObject GetCachedUIGameObject( UnityEngine.Object p_gb ){
		{
			UpdateCachedUIGameObject();
		}

		int t_count = 0;

		GameObject t_gb = null;

		for( int i = 0; i < m_2d_ui_gb_list.Count; i++ ){
			if( m_2d_ui_gb_list[ i ].name.IndexOf( p_gb.name ) == 0 ){
				t_count++;

				t_gb = m_2d_ui_gb_list[ i ];
			}
		}

		if( t_count > 1 ){
			Debug.LogError( "Error, multi ui name found." );

			for( int i = 0; i < m_2d_ui_gb_list.Count; i++ ){
				if( m_2d_ui_gb_list[ i ].name.IndexOf( p_gb.name ) == 0 ){

					GameObjectHelper.LogGameObjectHierarchy( m_2d_ui_gb_list[ i ], i + " : " );
				}
			}
		}

		return t_gb;
	}


	/// Desc:
	/// Manual add the Toppest UI, and hide MainCityUI.
	/// 
	/// Notice:
	/// 1.p_gb MUST be Function's UI.
	public void AddTopUI( GameObject p_gb ){
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
//				Debug.Log( "UI2D Already Added." );

//				Debug.Log( "GetTopUI().GetRootGameObject():" + GetTopUI().GetRootGameObject() );

				return;
			}
		}

		UI2DToolItem t_manager = new UI2DToolItem( p_gb );

		if( t_manager == null ){
			Debug.LogError( "Error when initialize." );

			return;
		}

		#if DEBUG_UI_2D_TOOL
		Debug.Log( "Add Top Function UI " + m_2d_manager_list.Count + ": " + 
		          GameObjectHelper.GetGameObjectHierarchy( p_gb ) );
		#endif

		// hide origin top
		#if ENABLE_OPTIMIZE
		if( GetTopUI() != null ){
			if( GetTopUI().GetRootGameObject() != null ){
				GetTopUI().SetActive( false );
			}
		}
		#endif

		{
			AddToTop( t_manager );
		}

		// refresh new top
		ForceUpdateTopUI();
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
			ForceUpdateTopUI();

			return;
		}

		if( t_top.GetRootGameObject() == null ){
			RemoveTopUI();

			return;
		}

		if( m_cached_gb != t_top.GetRootGameObject() ){
			ForceUpdateTopUI();

			return;
		}

		if( m_cached_visibility != t_top.GetVisibility() ){
			ForceUpdateTopUI();

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
		#if ENABLE_OPTIMIZE
		MakeTopUIVisible();
		#else
		MakeTopUIVisible();
		#endif

		if( Quality_MemLevel.IsMemLevelLow() ){
			UtilityTool.UnloadUnusedAssets();
		}
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
	private static void ForceUpdateTopUI(){
		if( m_2d_manager_list.Count <= 0 ){
			return;
		}
		
		UpdateCachedGameObject();

		if( !m_cached_visibility && m_2d_manager_list.Count > 1 ){
			RemoveTopUI();
		}

		#if SHOW_UIBGEF_ALL_THE_TIME
		UpdateUIBackground();
		#elif !UNITY_EDITOR
		UpdateUIBackground();
		#endif
	}
	
	private static void UpdateCachedGameObject(){
		// update cached gb
		{
			m_cached_gb = GetTopUI().GetRootGameObject();
		}

		// update cached visibility
		if( m_cached_gb != null ){
			m_cached_visibility = GetTopUI().GetVisibility();

			#if DEBUG_UI_2D_TOOL
			if( !m_cached_visibility ){
				Debug.Log( "not visible." );

				GetTopUI().LogVisibility();
			}

			#endif
			
		}
		else{
			#if DEBUG_UI_2D_TOOL
			Debug.Log( "m_cached_gb = null." );
			#endif

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

	private static void UpdateUIBackground(){
		#if UNITY_IOS
		return;
		#endif

		#if DEBUG_UI_2D_TOOL
		Debug.Log( "UpdateUIBackground() : " + m_2d_manager_list.Count );
		#endif

		#if ENABLE_OPTIMIZE 
		if( m_2d_manager_list.Count > 1 ){
			Console_Effect.SetUIBackground( true );
		}
		else{
			Console_Effect.SetUIBackground( false );
		}
		#else
		if( m_2d_manager_list.Count > 1 ){
			Console_Effect.SetUIBackground( true );
		}
		else{
			Console_Effect.SetUIBackground( false );
		}

		for( int i = 0; i < m_2d_manager_list.Count; i++ ){
			GameObject t_gb = m_2d_manager_list[ i ].GetRootGameObject();

            if (t_gb == null) {
                continue;
            }

			Camera[] t_cams = t_gb.GetComponentsInChildren<Camera>();

			for( int j = 0; j < t_cams.Length; j++ ){
				if( i == m_2d_manager_list.Count - 1 ){
					EffectTool.SetUIBackgroundEffect( t_cams[ j ].gameObject, false );
				}
				else{
					EffectTool.SetUIBackgroundEffect( t_cams[ j ].gameObject, true );
				}
			}
		}
		#endif
	}

	#endregion



	#region Special Case

	/// Battle v4 done.
	public static void OnBattleV4ResultShow(){
		#if UNITY_IOS
		return;
		#endif

		#if SHOW_UIBGEF_ALL_THE_TIME
		Console_Effect.SetUIBackground( true );
		#elif !UNITY_EDITOR
		Console_Effect.SetUIBackground( true );
		#endif
	}

	#endregion



	#region Utilities

	/// Return:
	/// True, if p_gb already exist, and make the ui visible.
	/// False, if p_gb not exist.
	/// 
	/// Tips:
	/// 1.Open this function to show Sample Code.
	/*
	 * Sample Code:
	if( UI2DTool.ShowHidedUI( p_object ) ){
		// do init&operation here
		
		GameObject t_ui_gb = UI2DTool.GetCachedUIGameObject( p_object );
		
		// emp 1
		{
			MainCityUI.TryAddToObjectList( t_ui_gb, false );
		}
		
		// emp 2
		//			{
		//				t_ui_gb.GetComponentInChildren<ScaleEffectController>().OnOpenWindowClick();
		//
		//				MainCityUI.TryAddToObjectList( t_ui_gb );
		//			}
		
		return;
	}
	*/
	public static bool ShowHidedUI( UnityEngine.Object p_gb ){
		GameObject t_gb = GetCachedUIGameObject( p_gb );

		if( t_gb != null ){
			#if DEBUG_UI_2D_TOOL
			Debug.Log( "Found Cached UI, Now Show it." );
			#endif

			t_gb.SetActive( true );

			return true;
		}
		else{
			return false;
		}
	}


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

	private static Vector3 GetUIRootPos( int p_count ){
//		Vector3 t_offset = new Vector3( -100, -100, -100 );

		Vector3 t_offset = new Vector3( -100, -100, 0 );

		Vector3 t_origin = new Vector3( 0, 1000, 0 );

		return t_origin + t_offset * p_count;
	}

	#endregion

	public class UI2DToolItem{
		private GameObject m_ui_root_gb;

		private bool m_is_active = true;

		private List<Camera> m_cached_cam = new List<Camera>();

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

			// fix potential bug
//			{
//				MonoBehaviour[] t_monos = m_ui_root_gb.GetComponentsInChildren<MonoBehaviour>();
//
//				bool t_found = false;
//
//				for( int i = 0; i < t_monos.Length; i++ ){
//					if( t_monos[ i ] is IUIRootAutoActivator ){
//						Debug.Log( "Activator found " + i + ": " + t_monos[ i ] );
//
//						UIRootAutoActivator.Instance().ManualUpdate();
//
//						t_found = true;
//
//						break;
//					}
//				}
//
//				if( !t_found ){
//					Debug.Log( "Activator not found." );
//				}
//			}

			{
				SetActive( true );
			}
		}

		public void LogVisibility(){
			bool t_gb_visible = m_cached_cam[ 0 ].gameObject.activeInHierarchy;
			
			bool t_cam_visible = m_cached_cam[ 0 ].enabled;
			
			Debug.Log ( "t_gb_visible: " + t_gb_visible );

			Debug.Log ( "t_cam_visible: " + t_cam_visible );
		}

		public bool GetVisibility(){
			{
//				return GetRootGameObject().activeSelf;
			}

			{
				if( m_cached_cam.Count > 0 ){
					bool t_gb_visible = m_cached_cam[ 0 ].gameObject.activeInHierarchy;

					bool t_cam_visible = m_cached_cam[ 0 ].enabled;

					if( !t_gb_visible ){
						return false;
					}
					else{
						return t_cam_visible;
					}
				}
				else{
					Debug.LogError( "Error, no cam exist." );

					return false;
				}
			}
		}

		public GameObject GetRootGameObject(){
			return m_ui_root_gb;
		}

		public void SetActive( bool p_is_active ){
			if( m_ui_root_gb == null ){
				Debug.LogError( "Error, m_ui_root_gb is null." );

				return;
			}

			m_is_active = p_is_active;

			#if DEBUG_UI_2D_TOOL
			Debug.Log( m_ui_root_gb + ".SetActive( " + m_is_active + " )" );
			#endif

			ExecuteSetActive();
		}

		public int GetFlagItemCount(){
			return m_cached_cam.Count;
		}

		private void ExecuteSetActive(){
//			{
//				m_ui_root_gb.SetActive( p_is_active );
//			}

			bool t_should_add = m_cached_cam.Count <= 0 ? true : false;

			{
				Camera[] t_cams = m_ui_root_gb.GetComponentsInChildren<Camera>();

				for( int i = 0; i < t_cams.Length; i++ ){
					t_cams[ i ].enabled = m_is_active;

					#if DEBUG_UI_2D_TOOL
					Debug.Log( "Manul Set Cam: " + m_is_active );
					
					GameObjectHelper.LogGameObjectHierarchy( t_cams[ i ].gameObject );
					#endif

					{
						UICamera t_ui_cam = t_cams[ i ].gameObject.GetComponent<UICamera>();

						if( t_ui_cam != null ){
							t_ui_cam.enabled = m_is_active;
						}
					}

					if( t_should_add ){
						m_cached_cam.Add( t_cams[ i ] );
					}
				}
			}

			{
				UI2DEventListener[] t_listeners = m_ui_root_gb.GetComponentsInChildren<UI2DEventListener>();

				for( int i = 0; i < t_listeners.Length; i++ ){
					t_listeners[ i ].OnUI2DShow();
				}
			}
		}
	}
}
