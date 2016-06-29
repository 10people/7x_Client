﻿//#define DEBUG_UI_2D_TOOL



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
		UpdateAll();
	}

	void LateUpdate(){
		UpdateAll();
	}

	void OnDestroy(){
		ClearAll();

		base.OnDestroy();
	}

	#endregion



	#region All

	private void UpdateAll(){
		Update2DUI();

		UpdateBackgroundScaler();

		UpdateLog();
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
		// remove if exist
		{
//			#if DEBUG_UI_2D_TOOL
//			Debug.Log( "Remove pre Added." );
//			#endif

			UI2DToolItem t_ui = GetUI( p_manager.GetRootGameObject() );

			if( t_ui != null ){
				m_2d_manager_list.Remove( t_ui );
			}
		}

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

	private bool m_hide_origin_top_ui = false;

	/// Desc:
	/// Manual add the Toppest UI, and hide MainCityUI.
	/// 
	/// Params:
	/// 1.p_gb, the UI's root gameObject which is tried to be added here;
	/// 2.p_hide_origin_top_ui, control whether or not hide the origin UI(except 0 UI);
	/// 
	/// Notice:
	/// 1.p_gb MUST be Function's UI.
	public void AddTopUI( GameObject p_gb, bool p_hide_origin_top_ui = true ){
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
				#if DEBUG_UI_2D_TOOL
				Debug.Log( "UI2D Already Added." );

				Debug.Log( "GetTopUI().GetRootGameObject():" + GetTopUI().GetRootGameObject() );
				#endif

				return;
			}
		}

		UI2DToolItem t_manager = new UI2DToolItem( p_gb );

		if( t_manager == null ){
			Debug.LogError( "Error when initialize." );

			return;
		}

		#if DEBUG_UI_2D_TOOL
		Debug.Log( " ------------------------- Add Top Function UI " + GameObjectHelper.GetGameObjectHierarchy( p_gb ) + " - " + p_hide_origin_top_ui +
			"   Cur.Total: " + m_2d_manager_list.Count );
		#endif

		// hide origin top
		#if ENABLE_OPTIMIZE
		HideOriginTopUI();
		#endif

		{
			m_hide_origin_top_ui = p_hide_origin_top_ui;

			if( p_hide_origin_top_ui ){
				if( m_2d_manager_list.Count > 1 ){
					HideOriginTopUI();
				}
			}

//			{
//				for( int i = 1; i < m_2d_manager_list.Count - 1; i++ ){
//					UI2DToolItem t_item = m_2d_manager_list[ i ];
//
//					if( t_item != null ){
//						#if DEBUG_UI_2D_TOOL
//						Debug.Log( "Making old UI Invisible: " + t_item.GetRootGameObject() );
//						#endif
//
//						t_item.SetActive( false );
//					}
//				}
//			}
		}

		{
			AddToTop( t_manager );
		}

		{
			ForceUpdateAllUI();
		}

		{
			// refresh new top
			ForceUpdateTopUI();
		}
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

	private void UpdateBackgroundScaler(){
		if( m_top_background_scaler != null ){
			if( GetPreTopBackgroundScalerVisibility() != m_top_background_scaler.gameObject.activeInHierarchy ){
				#if DEBUG_UI_2D_TOOL
				Debug.Log( "----------------------- Top Scaler Changed. ----------------------------" );
				#endif

				{
					ForceUpdateAllUI();
				}

				{
					ForceUpdateTopUI();
				}
			}
		}
	}

	private void Update2DUI(){
		if( m_2d_manager_list.Count <= 0 ){
			return;
		}

		UI2DToolItem t_top = GetTopUI();

		if( m_cached_gb == null ){
			{
				ForceUpdateTopUI( true );
			}

			return;
		}

		if( t_top.GetRootGameObject() == null ){
			RemoveTopUI();

			return;
		}

		if( m_cached_gb != t_top.GetRootGameObject() ){
			{
				ForceUpdateTopUI( true );
			}

			return;
		}

		if( m_cached_visibility != t_top.GetVisibility() ){
			{
				ForceUpdateTopUI( true );
			}

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

		if( m_2d_manager_list.Count == 1 ){
			UtilityTool.CheckGCTimer();
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

	// Update all UI
	private static void ForceUpdateAllUI(){
		#if DEBUG_UI_2D_TOOL
		Debug.Log( "---------------- ForceUpdateAllUI()" );

		LogInfo();
		#endif

		bool t_cover_found = false;

		for( int i = m_2d_manager_list.Count - 1; i >= 0; i-- ){
			UI2DToolItem t_item = m_2d_manager_list[ i ];

			if( t_item != null ){
				if( t_cover_found ){
					#if DEBUG_UI_2D_TOOL
					Debug.Log( "Making old UI Invisible: " + t_item.GetRootGameObject() );
					#endif

					t_item.SetActive( false );

					continue;
				}
				else{
					t_item.SetActive( true );
				}

				if( !t_cover_found && t_item.HaveActiveBackgroundScaler() ){
					t_cover_found = true;

					continue;
				}
			}
		}
	}

	/// Update Top UI, if Top UI is invisible, Remove it.
	private static void ForceUpdateTopUI( bool p_force_update_all = false ){
		#if DEBUG_UI_2D_TOOL
		Debug.Log( "---------------- ForceUpdateTopUI()" );

		LogInfo();
		#endif

		if( m_2d_manager_list.Count <= 0 ){
			return;
		}
		
		UpdateCachedGameObject();

		if( !m_cached_visibility && m_2d_manager_list.Count > 1 ){
			RemoveTopUI();
		}

		if( p_force_update_all ){
			ForceUpdateAllUI();
		}

		{
			UpdateUIBackground();
		}
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
				Debug.Log( "TopUI not visible." );

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

	private static BackgroundScaler m_top_background_scaler = null;

	private static bool m_pre_top_background_scaler_visibility = false;

	private static bool GetPreTopBackgroundScalerVisibility(){
		return m_pre_top_background_scaler_visibility;
	}

	private static void SetPreTopBackgroundScalerVisibility( bool p_cur_visibility ){
		m_pre_top_background_scaler_visibility = p_cur_visibility;
	}

	public static GameObject GetTopBackgroundScalerGameObject(){
		if( m_top_background_scaler == null ){
			return null;
		}

		return m_top_background_scaler.gameObject;
	}

	private static void UpdateUIBackground(){
		#if UNITY_IOS
		return;
		#endif

		#if DEBUG_UI_2D_TOOL
		Debug.Log( "-------------- UpdateUIBackground( Count: " + m_2d_manager_list.Count + " )" );

		LogInfo();

		#endif


		#if ENABLE_OPTIMIZE 
		if( m_2d_manager_list.Count > 1 ){
			Console_Effect.SetUIBackground( true );
		}
		else{
			Console_Effect.SetUIBackground( false );
		}
		#else
//		if( m_2d_manager_list.Count > 1 ){
//			Console_Effect.SetUIBackground( true );
//		}
//		else{
//			Console_Effect.SetUIBackground( false );
//		}

		bool t_root_bg_scaled = false;

		BackgroundScaler t_active_background_scaler = null;

		bool t_camera_fx_setted = false;

		int t_camera_fx_index = -1;

		for( int i = m_2d_manager_list.Count - 1; i >= 0; i-- ){
			GameObject t_gb = m_2d_manager_list[ i ].GetRootGameObject();

            if( t_gb == null ) {
                continue;
            }

			if( i == m_2d_manager_list.Count - 1 ){
				m_2d_manager_list[i].SetCameraEffect( false );

				t_active_background_scaler = m_2d_manager_list[ i ].GetActiveBackgroundScaler();

				m_top_background_scaler = m_2d_manager_list[ i ].GetBackgroundScaler();

				if( m_top_background_scaler != null ){
					SetPreTopBackgroundScalerVisibility( m_top_background_scaler.gameObject.activeInHierarchy );
				}

				if( t_active_background_scaler != null ){
					#if DEBUG_UI_2D_TOOL
					Debug.Log( "Scaler Found: " + GameObjectHelper.GetGameObjectHierarchy( t_active_background_scaler.gameObject ) );
					#endif

					t_root_bg_scaled = true;
				}
				else{
//					Debug.Log( "No Active Background Scaler Found: " + m_2d_manager_list[ i ].GetRootGameObject() );
//
//					Debug.Log( "Scaler: " + GameObjectHelper.GetGameObjectHierarchy( m_2d_manager_list[ i ].GetBackgroundScaler() ) );
				}
			}
			else{
				if( !m_2d_manager_list[i].GetVisibility() ){
					continue;
				}

				if( t_root_bg_scaled ){
					continue;
				}

				if( t_camera_fx_setted ){
					m_2d_manager_list[i].SetCameraEffect( false );
					continue;
				}
					
				if( m_2d_manager_list[i].SetCameraEffect( true ) ){
					t_camera_fx_setted = true;

					t_camera_fx_index = i;
				}
			}
		}

//		Debug.Log( "m_2d_manager_list.Count: " + m_2d_manager_list.Count );
//
//		Debug.Log( "t_root_bg_scaled: " + t_root_bg_scaled );


		{
			if( m_2d_manager_list.Count > 1 && t_root_bg_scaled ){
				m_2d_manager_list[ 0 ].SetActive( false );

				CameraHelper.SetMainCamera( false );
			}
			else if( t_camera_fx_setted && t_camera_fx_index > 0 ){
				m_2d_manager_list[ 0 ].SetActive( false );

				CameraHelper.SetMainCamera( false );
			}
			else{
				if( m_2d_manager_list[ 0 ] != null ){
					m_2d_manager_list[ 0 ].SetActive( true );

					if( !m_2d_manager_list[ 0 ].IsCameraEffectOpen() ){
						CameraHelper.SetMainCamera( true );
					}
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

		Console_Effect.SetUIBackground( true );
	}

	#endregion



	#region Utilities

	public bool m_log_info = false;

	private void UpdateLog(){
		if( !m_log_info ){
			return;
		}

		m_log_info = false;

		LogInfo();
	}

	private static void LogInfo(){
		UI2DTool m_2d_tool = UI2DTool.GetInstanceWithOutCreate();

		int t_count = m_2d_tool.GetUICount();

		for( int i = t_count - 1; i >= 0; i-- ){
			UI2DTool.UI2DToolItem t_ui = m_2d_tool.GetUI( i );

			if( t_ui.GetRootGameObject() != null ){
				Debug.Log( i + 
					" visible: " + t_ui.GetVisibility() + 
					" Scaler: " + t_ui.HaveActiveBackgroundScaler() +  
					"   - " + t_ui.GetRootGameObject().name + " - " + t_ui.GetFlagItemCount() );
			}
			else{
				Debug.Log( i + ": 2D UI GameObject = null" );
			}
		}
	}

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

	private void HideOriginTopUI(){
		if( GetTopUI() != null ){
			if( GetTopUI().GetRootGameObject() != null ){
				GetTopUI().SetActive( false );
			}
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

	public UI2DToolItem GetUI( GameObject p_gb ){
		for( int i = 0; i < m_2d_manager_list.Count; i++ ){
			if( m_2d_manager_list[ i ].GetRootGameObject() == p_gb ){
				return m_2d_manager_list[ i ];
			}
		}
			
		return null;
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
//				Debug.LogError( "Error, m_ui_root_gb is null." );

				return;
			}

			m_is_active = p_is_active;

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
					Debug.Log( "-- " + m_ui_root_gb + 
						".Manul Set Cam: " + m_is_active + 
						"   --- " + GameObjectHelper.GetGameObjectHierarchy( t_cams[ i ].gameObject ) );
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

		public BackgroundScaler GetActiveBackgroundScaler(){
			GameObject t_gb = GetRootGameObject();

			if( t_gb == null ){
				return null;
			}

			BackgroundScaler[] t_scalers = t_gb.GetComponentsInChildren<BackgroundScaler>();

			for( int k = 0; k < t_scalers.Length; k++ ){
				if( t_scalers[ k ] != null ){
					if( t_scalers[ k ].gameObject.activeInHierarchy ){
						if( t_scalers[ k ].IsFullScreenUI() ){
							return t_scalers[ k ];	
						}
					}
				}
			}

			return null;
		}

		public BackgroundScaler GetBackgroundScaler(){
			GameObject t_gb = GetRootGameObject();

			if( t_gb == null ){
				return null;
			}

			BackgroundScaler[] t_scalers = t_gb.GetComponentsInChildren<BackgroundScaler>( true );

			for( int k = 0; k < t_scalers.Length; k++ ){
				if( t_scalers[ k ] != null ){
					return t_scalers[ k ];	
				}
			}

			return null;
		}

		public bool HaveActiveBackgroundScaler(){
			BackgroundScaler t_scaler = GetActiveBackgroundScaler();

			if( t_scaler != null ){
				return true;
			}

			return false;
		}

		// true, if UIBackgroundEffect found.
		public bool SetCameraEffect( bool p_enable_effect ){
			GameObject t_gb = GetRootGameObject();

			bool t_found = false;

			if( t_gb == null ) {
				return false;
			}

			Camera[] t_cams = t_gb.GetComponentsInChildren<Camera>();

			for( int j = 0; j < t_cams.Length; j++ ){
				UIBackgroundEffect t_fx = EffectTool.SetUIBackgroundEffect( t_cams[ j ].gameObject, p_enable_effect );

				if( t_fx != null ){
					t_found = true;
				}
			}

			return t_found;
		}

		public bool IsCameraEffectOpen(){
			GameObject t_gb = GetRootGameObject();

			if( t_gb == null ) {
				return false;
			}

			Camera[] t_cams = t_gb.GetComponentsInChildren<Camera>();

			for( int j = 0; j < t_cams.Length; j++ ){
				if( UIBackgroundEffect.IsUIBackgroundEffectOpen( t_cams[ j ].gameObject ) ){
					return true;
				}
			}

			return false;
		}
	}
}
