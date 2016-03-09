#define ALWAYS_RETAIN



//#define DEBUG_UI_EFFECT

//#define FX_ART_USE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.x.x
 * @since:		Unity 4.5.3
 * Function:	Help to manage 3d ui effect.
 * 
 * Notes:
 * None.
 */ 
public class UI3DEffectTool : MonoBehaviour {

	/// Desc:
	/// UIType.MainUI: 
	///       MainCity.UI/BattleField.UI/House.UI;
	/// 
	/// UIType.FunctionUI: 
	///       Other Function UIs;
	/// 
	/// UIType.PopUI:
	///       For All Top-Level UIs;
	public enum UIType{
		None,
		MainUI_0,
		FunctionUI_1,
		PopUI_2,
	}

	public GameObject[] m_ngui_layer_root;

	public GameObject[] m_top_layer_root;

	public GameObject[] m_mid_layer_root;

	public GameObject[] m_bot_layer_root;



	public Vector2 m_design_size = new Vector2( 960, 640 );

	public float m_c_factor = 1.0f;


	public UILabel[] m_lb_debug;


	private static UI3DEffectTool m_instance = null;

	public static bool HaveInstance(){
		return m_instance != null;
	}
		
	public static UI3DEffectTool Instance(){
		if( m_instance == null ){
			#if DEBUG_UI_EFFECT
			Debug.Log( "UI3DEffectTool.Instance( " + Res2DTemplate.GetResPath( Res2DTemplate.Res.UI_3D_EFFECT ) + " )" );
			#endif

			string t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.UI_3D_EFFECT );
//			string t_ui_path = "_UIs/Utilities/UI_3D_Effect_Root";

#if FX_ART_USE && UNITY_EDITOR
			Object t_obj = Resources.Load( t_ui_path );

			WWW t_www = null;

			ResourceLoadCallback( ref t_www, "", t_obj );
#else
			Global.ResourcesDotLoad( t_ui_path, ResourceLoadCallback );
#endif
		}
		
		return m_instance;
	}
	
	public static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );
		
		if( t_gb == null ){
			Debug.LogError( "Instantiate to null." );
			
			return;
		}

		DontDestroyOnLoad( t_gb );
	}
	
	#region Mono

	void Awake(){
		#if DEBUG_UI_EFFECT
		Debug.Log( "UI3DEffectTool.Make.Instance()" );
		#endif

		m_instance = this;
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
#if FX_ART_USE && UNITY_EDITOR
		CheckArtFx();
#else
#endif

		{
			FxHelper.UpdateFx();
		}

		{
			UpdateToLoad();
			
			UpdateFxWatcher();
			
			UpdateCameras();
			
			DynamicSet();
		}
	}

	void OnDestroy(){
		#if DEBUG_UI_EFFECT
		Debug.Log( "UI3DEffectTool.OnDestroy()" );
		#endif

		{
			for( int i = 0; i < m_ngui_layer_root.Length; i++ ){
				m_ngui_layer_root[ i ] = null;
			}
			
			m_ngui_layer_root = null;
			
			for( int i = 0; i < m_top_layer_root.Length; i++ ){
				m_top_layer_root[ i ] = null;
			}
			
			m_top_layer_root = null;
			
			for( int i = 0; i < m_mid_layer_root.Length; i++ ){
				m_mid_layer_root[ i ] = null;
			}
			m_mid_layer_root = null;
			
			
			for( int i = 0; i < m_bot_layer_root.Length; i++ ){
				m_bot_layer_root[ i ] = null;
			}
			
			m_bot_layer_root = null;
		}
	}

	#endregion



	#region Dynamic Set

	private int t_dyset = 0;

	private void ForceDynamicSetNow(){
		t_dyset = -1;

		DynamicSet();
	}

	/// Dynamic active and deactive cameras.
	private void DynamicSet(){
		{
			t_dyset++;

			if( t_dyset %10 != 0 ){
				return;
			}

			{
				t_dyset = 0;
			}
		}

		DynamicSet( m_ngui_layer_root );

		DynamicSet( m_top_layer_root );

		DynamicSet( m_mid_layer_root );

		DynamicSet( m_bot_layer_root );
	}

	private void DynamicSet( GameObject[] p_gbs ){
		if( p_gbs == null ){
			return;
		}

		for( int i = 0; i < p_gbs.Length; i++ ){
			if( p_gbs[ i ] == null ){
				continue;
			}

			p_gbs[ i ].transform.parent.gameObject.SetActive( p_gbs[ i ].transform.childCount > 0 ? true : false );
		}
	}

	#endregion



#if FX_ART_USE && UNITY_EDITOR

	public GameObject fx_ngui_gb = null;

	public GameObject fx_top_prefab = null;

	public GameObject fx_mid_prefab = null;

	public GameObject fx_bot_prefab = null;

	private void CheckArtFx(){
		if( fx_ngui_gb == null ){
			return;
		}

		WWW t_www = null;

		if( fx_top_prefab != null ){
			TopEffectLoadCallback( fx_ngui_gb, ( GameObject )Instantiate( fx_top_prefab ) );
		}
		else if( fx_mid_prefab != null ){
			MidEffectLoadCallback( fx_ngui_gb, ( GameObject )Instantiate( fx_mid_prefab ) );
		}
		else if( fx_bot_prefab != null ){
			BottomEffectLoadCallback( fx_ngui_gb, ( GameObject )Instantiate( fx_bot_prefab ) );
		}
		else{
			return;
		}

		{
			fx_ngui_gb = null;

			fx_top_prefab = null;

			fx_mid_prefab = null;

			fx_bot_prefab = null;
		}
	}

	private static void SetGameObjectLayer( GameObject p_target_layer_gb, GameObject p_gameobject ){
		int t_child_count = p_gameobject.transform.childCount;
		
		{
			for( int i = 0; i < t_child_count; i++ ){
				Transform t_child = p_gameobject.transform.GetChild( i );
				
				GameObjectHelper.SetGameObjectLayer( t_child.gameObject, p_target_layer_gb.layer );
			}
			
			GameObjectHelper.SetGameObjectLayer( p_gameobject, p_target_layer_gb.layer );
		}
	}

#endif



	#region Clean 3D Effect

	public static void ClearAllUI3DEffect(){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );

			return;
		}

		ClearAllUIFx();

		ClearAllFxInToLoad();
	}

	#endregion



	#region Utilities

	/// Whether p_ngui_gb have any fx on it.
	public static bool HaveAnyFx( GameObject p_ngui_gb  ){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );
			
			return false;
		}

		FxWatcher t_watcher = GetFxWatcher( p_ngui_gb );

		if( t_watcher != null ){
			if( t_watcher.HaveFx() ){
				return true;
			}
			else{
				return false;
			}
		}
		else{
			return false;
		}
	}

	#endregion



	#region Top Layer Effect

	/// Desc:
	/// Show 3D UI Effect on top of all UIs.
	/// 
	/// Params:
	/// 1.p_ui_type: 
	///       MainUI for MainCity.UI/BattleField.UI/House.UI;
	///       FunctionUI for other popout Function.UI;
	/// 	  PopUI for All Top-Level UIs;
	/// 
	/// 2.p_target_ngui_gb: 
	///       Target NGUI GameObject;
	/// 
	/// 3.p_fx_local_pos:
	///       Local Vector3 Position(Inspector.Transform.Position);
	/// 
	/// 4.p_3d_effect_path: 
	///       "_3D/Fx/_To_Sort/beibao";
	public static void ShowTopLayerEffect( UIType p_ui_type, GameObject p_target_ngui_gb, string p_3d_effect_path, GameObject p_target_ngui_center_gb = null  ){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );
			
			return;
		}

		#if DEBUG_UI_EFFECT
		Debug.Log( "ShowTopLayerEffect: " + p_ui_type + " - " + p_target_ngui_gb );

//		DebugNGUIObject( p_target_ngui_gb );
//
//		Debug.Log( "path: " + p_3d_effect_path + " - " + p_target_ngui_center_gb );
		#endif

		Instance().AddToLoadList( p_ui_type, p_target_ngui_gb, p_3d_effect_path, Instance().TopEffectLoadCallback, p_target_ngui_center_gb );
	}

	public void TopEffectLoadCallback( UIType p_ui_type, GameObject p_ngui_gb, GameObject p_effect_object, GameObject p_target_ngui_center_gb = null  ){
		GameObject t_gb = p_effect_object;

		{
			#if FX_ART_USE && UNITY_EDITOR
			SetGameObjectLayer( GetTopRoot( p_ui_type ), t_gb );
			#else
			GameObjectHelper.SetGameObjectLayerRecursive( t_gb, GetTopRoot( p_ui_type ).layer );
			#endif

			#if DEBUG_UI_EFFECT
			Debug.Log( "TopEffectLoadCallback.SetParent: " + t_gb + " - " + GetTopRoot( p_ui_type ) );
			#endif

			t_gb.transform.parent = GetTopRoot( p_ui_type ).transform;
		}

		{
			AddFxWatcher( p_ngui_gb, t_gb, true, p_target_ngui_center_gb );
		}
	}

	#endregion



	#region Bottom Layer Effect

	/// Desc:
	/// Show 3D UI Effect bellow all UIs.
	/// 
	/// Params:
	/// 1.p_ui_type: 
	///       MainUI for MainCity.UI/BattleField.UI/House.UI;
	///       FunctionUI for other popout Function.UI;
	/// 	  PopUI for All Top-Level UIs;
	/// 
	/// 2.p_target_ngui_gb: 
	///       Target NGUI GameObject;
	/// 
	/// 3.p_fx_local_pos: 
	///       Local Vector3 Position(Inspector.Transform.Position);
	/// 
	/// 4.p_3d_effect_path: 
	///       "_3D/Fx/_To_Sort/beibao";
	public static void ShowBottomLayerEffect( UIType p_ui_type, GameObject p_target_ngui_gb, string p_3d_effect_path, GameObject p_target_ngui_center_gb = null ){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );

			return;
		}

		#if DEBUG_UI_EFFECT
		Debug.Log( "ShowBottomLayerEffect: " + p_ui_type + " - " + p_target_ngui_gb );

//		DebugNGUIObject( p_target_ngui_gb );
//
//		Debug.Log( "path: " + p_3d_effect_path + " - " + p_target_ngui_center_gb );
		#endif

		Instance().AddToLoadList( p_ui_type, p_target_ngui_gb, p_3d_effect_path, Instance().BottomEffectLoadCallback, p_target_ngui_center_gb );
	}
	
	public void BottomEffectLoadCallback( UIType p_ui_type, GameObject p_ngui_gb, GameObject p_effect_object, GameObject p_target_ngui_center_gb = null ){
		GameObject t_gb = p_effect_object;
		
		{
			#if FX_ART_USE && UNITY_EDITOR
			SetGameObjectLayer( GetBotRoot( p_ui_type ), t_gb );
			#else
			GameObjectHelper.SetGameObjectLayerRecursive( t_gb, GetBotRoot( p_ui_type ).layer );
			#endif

			#if DEBUG_UI_EFFECT
			Debug.Log( "BottomEffectLoadCallback.SetParent: " + t_gb + " - " + GetBotRoot( p_ui_type ) );
			#endif

			t_gb.transform.parent = GetBotRoot( p_ui_type ).transform;
		}

		{
			AddFxWatcher( p_ngui_gb, t_gb, true, p_target_ngui_center_gb );
		}
	}
	
	#endregion



	#region Mid Layer Effect

	/// Desc:
	/// Show 3D UI Effect bellow common UIs.
	/// 
	/// Params:
	/// 1.p_ui_type: 
	///       MainUI for MainCity.UI/BattleField.UI/House.UI;
	///       FunctionUI for other popout Function.UI;
	/// 	  PopUI for All Top-Level UIs;
	/// 
	/// 2.p_target_ngui_gb: 
	///       Target NGUI GameObject;
	/// 
	/// 3.p_fx_local_pos: 
	///       Local Vector3 Position(Inspector.Transform.Position);
	/// 
	/// 4.p_3d_effect_path: 
	///       "_3D/Fx/_To_Sort/beibao";
	public static void ShowMidLayerEffect( UIType p_ui_type, GameObject p_target_ngui_gb, string p_3d_effect_path, GameObject p_target_ngui_center_gb = null  ){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );
			
			return;
		}

		#if DEBUG_UI_EFFECT
		Debug.Log( "ShowMidLayerEffect: " + p_ui_type + " - " + p_target_ngui_gb );

//		DebugNGUIObject( p_target_ngui_gb );
//		
//		Debug.Log( "path: " + p_3d_effect_path + " - " + p_target_ngui_center_gb );
		#endif

		Instance().AddToLoadList( p_ui_type, p_target_ngui_gb, p_3d_effect_path, Instance().MidEffectLoadCallback, p_target_ngui_center_gb );
	}

	/// Desc:
	/// Show other NGUI overlaying.
	/// 
	/// Notes:
	/// 1.These NGUI compoents must be static which means not moving or scaling.
	/// 
	/// Params:
	/// 1.p_ui_type: 
	///       MainUI for MainCity.UI/BattleField.UI/House.UI;
	///       FunctionUI for other popout Function.UI;
	/// 	  PopUI for All Top-Level UIs;
	/// 
	/// 2.p_target_ngui_gb: 
	///       Target NGUI GameObject, same as ShowMidLayerEffect.
	/// 
	/// 3.p_overlay_ngui_gb: 
	///       Another ngui gb need to be overlaying.
	public static void ShowMidLayerOverLayNGUI( UIType p_ui_type, GameObject p_target_ngui_gb, GameObject p_overlay_ngui_gb, GameObject p_target_ngui_center_gb = null  ){
		if( !HaveInstance() ){
			Debug.LogError( "No UI3DInstance Exist." );
			
			return;
		}

		#if DEBUG_UI_EFFECT
		Debug.Log( "ShowMidLayerOverLayNGUI: " + p_ui_type + " - " + p_target_ngui_gb );
		
		Debug.Log( "path: " + p_overlay_ngui_gb + " - " + p_target_ngui_center_gb );
		#endif

		if( p_target_ngui_gb == null ){
			Debug.LogError( "p_target_ngui_gb = null." );
			
			return;
		}

		if( p_overlay_ngui_gb == null ){
			Debug.LogError( "p_overlay_ngui_gb = null." );
			
			return;
		}

		GameObject t_obj = GameObjectHelper.AddChild( Instance().GetNGUIRoot( p_ui_type ), p_overlay_ngui_gb );

		{
			Instance().ClearShadow( t_obj );
		}

		{
			Vector3 t_local_pos = TransformHelper.GetLocalPositionInUIRoot( p_overlay_ngui_gb );
			
			t_obj.transform.localPosition = t_local_pos;
		}
		
		Instance().AddFxWatcher( p_overlay_ngui_gb, t_obj, true, p_target_ngui_center_gb );
	}
	
	public void MidEffectLoadCallback( UIType p_ui_type, GameObject p_ngui_gb, GameObject p_effect_object, GameObject p_target_ngui_center_gb = null  ){
		GameObject t_gb = p_effect_object;
		
		{
			#if FX_ART_USE && UNITY_EDITOR
			SetGameObjectLayer( GetMidRoot( p_ui_type ), t_gb );
			#else
			GameObjectHelper.SetGameObjectLayerRecursive( t_gb, GetMidRoot( p_ui_type ).layer );
			#endif

			#if DEBUG_UI_EFFECT
			Debug.Log( "MidEffectLoadCallback.SetParent: " + t_gb + " - " + GetMidRoot( p_ui_type ) );
			#endif

			t_gb.transform.parent = GetMidRoot( p_ui_type ).transform;
		}

		{
			SetOverLayNGUI( p_ui_type, p_ngui_gb, p_target_ngui_center_gb );
		}

		{
			AddFxWatcher( p_ngui_gb, t_gb, true, p_target_ngui_center_gb );
		}
	}

	/// add a overlaying ngui shadow object.
	private void SetOverLayNGUI( UIType p_ui_type, GameObject p_overlay_ngui_gb, GameObject p_target_ngui_center_gb = null ){
		#if DEBUG_UI_EFFECT
		Debug.Log( "SetOverLayNGUI: " + p_ui_type + " - " + p_overlay_ngui_gb + " - " + p_target_ngui_center_gb );
		#endif

		if( p_overlay_ngui_gb == null ){
			Debug.LogError( "p_overlay_ngui_gb = null." );

			return;
		}

		GameObject t_obj = GameObjectHelper.AddChild( GetNGUIRoot( p_ui_type ), p_overlay_ngui_gb );

		{
			ClearShadow( t_obj );
		}

		{
			Vector3 t_local_pos = Vector3.zero;

			Vector3 t_local_scale = Vector3.one;

			if( p_target_ngui_center_gb == null ){
				t_local_pos = TransformHelper.GetLocalPositionInUIRoot( p_overlay_ngui_gb );

				t_local_scale = TransformHelper.GetLocalScaleInUIRoot( p_overlay_ngui_gb );
			}
			else{
				t_local_pos = TransformHelper.GetLocalPositionInUIRoot( p_target_ngui_center_gb );

				t_local_scale = TransformHelper.GetLocalScaleInUIRoot( p_target_ngui_center_gb );
			}

			t_obj.transform.localPosition = t_local_pos;

			t_obj.transform.localScale = t_local_scale;
		}

		AddFxWatcher( p_overlay_ngui_gb, t_obj, true, p_target_ngui_center_gb );
	}

	private void ClearShadow( GameObject p_gb ){
		ComponentHelper.DisableColliders( p_gb );
		
		ComponentHelper.StopITweens( p_gb );
		
		ComponentHelper.ClearMonosWithoutNGUI( p_gb );
	}
	
	#endregion



	#region High Low

	private GameObject GetNGUIRoot( UIType p_ui_type ){
		switch( p_ui_type ){
		case UIType.MainUI_0:
			return m_ngui_layer_root[ 0 ];

		case UIType.FunctionUI_1:
			return m_ngui_layer_root[ 1 ];

		case UIType.PopUI_2:
			return m_ngui_layer_root[ 2 ];
		}

		Debug.LogError( "Error Type: " + p_ui_type );

		return null;
	}

	private GameObject GetTopRoot( UIType p_ui_type ){
		switch( p_ui_type ){
		case UIType.MainUI_0:
			return m_top_layer_root[ 0 ];
			
		case UIType.FunctionUI_1:
			return m_top_layer_root[ 1 ];

		case UIType.PopUI_2:
			return m_top_layer_root[ 2 ];
		}
		
		Debug.LogError( "Error Type: " + p_ui_type );
		
		return null;
	}

	private GameObject GetMidRoot( UIType p_ui_type ){
		switch( p_ui_type ){
		case UIType.MainUI_0:
			return m_mid_layer_root[ 0 ];
			
		case UIType.FunctionUI_1:
			return m_mid_layer_root[ 1 ];

		case UIType.PopUI_2:
			return m_mid_layer_root[ 2 ];
		}
		
		Debug.LogError( "Error Type: " + p_ui_type );
		
		return null;
	}

	private GameObject GetBotRoot( UIType p_ui_type ){
		switch( p_ui_type ){
		case UIType.MainUI_0:
			return m_bot_layer_root[ 0 ];
			
		case UIType.FunctionUI_1:
			return m_bot_layer_root[ 1 ];

		case UIType.PopUI_2:
			return m_bot_layer_root[ 2 ];
		}
		
		Debug.LogError( "Error Type: " + p_ui_type );
		
		return null;
	}
	
	#endregion



	#region Cameras

	private void UpdateCameras(){
		float t_a = Screen.width / m_design_size.x;
		
		float t_b = Screen.height / m_design_size.y;
		
		float t_c_factor = t_a / t_b;
		
		if( t_c_factor < 1 ){
			t_c_factor = 1 / t_c_factor;
		}
		else{
			t_c_factor = 1.0f;
		}

		if( Mathf.Approximately( m_c_factor, t_c_factor ) ){
			return;
		}

		m_c_factor = t_c_factor;

		for( int i = 0; i < m_top_layer_root.Length; i++ ){
			UpdateCamera( m_top_layer_root[ i ].transform.parent.gameObject.GetComponent<Camera>() );
		}
		
		for( int i = 0; i < m_mid_layer_root.Length; i++ ){
			UpdateCamera( m_mid_layer_root[ i ].transform.parent.gameObject.GetComponent<Camera>() );
		}
		
		for( int i = 0; i < m_bot_layer_root.Length; i++ ){
			UpdateCamera( m_bot_layer_root[ i ].transform.parent.gameObject.GetComponent<Camera>() );
		}
	}
	
	private void UpdateCamera( Camera p_camera ){
		if( p_camera == null ){
			Debug.LogError( "Error, No Camera Here." );

			return;
		}

		p_camera.orthographicSize = m_c_factor;
	}

	#endregion



	#region Fx Watcher

	public List<FxWatcher> m_fx_watcher_list = new List<FxWatcher>();

	private void AddFxWatcher( GameObject p_ngui_gb, GameObject p_param_gb, bool p_sync_ngui_tran = true, GameObject p_target_ngui_center_gb = null ){
		if( p_ngui_gb == null ){
			Debug.LogError( "Error, p_ngui_gb = null." );

			return;
		}

		FxWatcher t_watcher = GetOrCreateFxWatcher( p_ngui_gb );

		if( t_watcher == null ){
			Debug.Log( "Watcher is Null." );
		}

		t_watcher.AddWatchItem( p_param_gb, p_sync_ngui_tran, p_target_ngui_center_gb );

		t_watcher.ForceUpdateVisibility();
	}

	private static FxWatcher GetFxWatcher( GameObject p_ngui_gb ){
		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return null;
		}

		for( int i = 0; i < Instance().m_fx_watcher_list.Count; i++ ){
			FxWatcher t_watcher = Instance().m_fx_watcher_list[ i ];

			if( t_watcher.m_target_ngui_gb == p_ngui_gb ){
				return t_watcher;
			}
		}

		return null;
	}

	private static FxWatcher GetOrCreateFxWatcher( GameObject p_ngui_gb ){
		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return null;
		}

		FxWatcher t_watcher = GetFxWatcher( p_ngui_gb );

		if( t_watcher != null ){
			return t_watcher;
		}

		t_watcher = new FxWatcher( p_ngui_gb );

		Instance().m_fx_watcher_list.Add( t_watcher );

		return t_watcher;
	}

	public void UpdateFxWatcher(){
		int t_count = m_fx_watcher_list.Count;

		for( int i = t_count - 1; i >= 0; i-- ){
			FxWatcher t_watcher = m_fx_watcher_list[ i ];

			t_watcher.Update();
		}
	}

	private static void ClearAllUIFx(){
		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return;
		}

		for( int i = Instance().m_fx_watcher_list.Count - 1; i >= 0; i-- ){
			FxWatcher t_watcher = Instance().m_fx_watcher_list[ i ];
			
			t_watcher.Clear();
		}
	}

	public static void ClearUIFx( GameObject p_ngui_gb ){
		#if DEBUG_UI_EFFECT
		Debug.Log( "ClearUIFx: " + p_ngui_gb );
		#endif

		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return;
		}

		{
			ClearFxInToLoad( p_ngui_gb );
		}

		FxWatcher t_watcher = GetFxWatcher( p_ngui_gb );

		if( t_watcher == null ){
//			Debug.Log( "No UI fx Exist for: " + p_ngui_gb );

			return;
		}

		t_watcher.Clear();
	}

	public class FxWatcherShadow{
		public GameObject m_shadow_gb = null;

		private GameObject m_target_ngui_center_gb = null;

		public UIWidget m_shadow_widget = null;

		private Transform m_shadow_widget_tran = null;

		public bool m_sync_ngui_tran = false;

		public FxWatcherShadow( GameObject p_target_gb, bool p_sync_ngui_tran, GameObject p_target_ngui_center_gb = null ){
			UIWidget t_target_widget = p_target_gb.GetComponent<UIWidget>();
			
			InitConstructor( p_target_gb, t_target_widget, p_sync_ngui_tran, p_target_ngui_center_gb );
		}

//		public FxWatcherShadow( GameObject p_target_gb, UIWidget p_target_widget, bool p_sync_ngui_tran ){
//			InitConstructor( p_target_gb, p_target_widget, p_sync_ngui_tran );
//		}

		private void InitConstructor( GameObject p_target_gb, UIWidget p_target_widget, bool p_sync_ngui_tran, GameObject p_target_ngui_center_gb = null ){
			if( p_target_gb == null ){
				Debug.LogError( "Error, No Shadow." );
				
				return;
			}
			
			m_shadow_gb = p_target_gb;

			m_target_ngui_center_gb = p_target_ngui_center_gb;

			m_shadow_widget = p_target_widget;
			
			if( m_shadow_widget != null ){
				m_shadow_widget_tran = m_shadow_widget.gameObject.transform;
			}
			
			m_sync_ngui_tran = p_sync_ngui_tran;
		}

		public void UpdateVisibility( bool p_visible ){
			if( m_shadow_gb == null ){
				return;
			}

			m_shadow_gb.SetActive( p_visible );
		}

		public string GetShadowVisibility(){
			if( m_shadow_gb == null ){
				return "Null";
			}

			return m_shadow_gb.activeInHierarchy + "";
		}

		/// Update Fx GameObject.
		public void UpdateFxPosition( Vector3 p_local_pos ){
			if( m_shadow_gb == null ){
				return;
			}

			if( m_target_ngui_center_gb == null ){
				m_shadow_gb.transform.localPosition = p_local_pos * UI3DEffectTool.Instance().m_c_factor;
			}
			else{
				Vector3 t_local_pos = TransformHelper.GetLocalPositionInUIRoot( m_target_ngui_center_gb );

				m_shadow_gb.transform.localPosition = t_local_pos * UI3DEffectTool.Instance().m_c_factor;
			}
		}

		/// Update NGUI Shadow GameObject.
		public void UpdateNGUITran( Vector3 p_position, Vector3 p_rot, Vector3 p_local_scale ){
			if( !m_sync_ngui_tran ){
				return;
			}

			m_shadow_widget_tran.localPosition = p_position;

			m_shadow_widget_tran.eulerAngles = p_rot;
			
			m_shadow_widget_tran.localScale = p_local_scale;
			
//			Debug.Log( "Find Widget: " + t_widget.gameObject.name );
		}
	}

	public class FxWatcher{
		public GameObject m_target_ngui_gb = null;

		private GameObject m_target_ngui_ui_root_gb = null;

		private List<Camera> m_target_ngui_ui_root_cams = new List<Camera>();



		private Vector3 m_cached_target_local_ngui_pos = Vector3.zero;

		private Vector3 m_cached_target_ngui_rot = Vector3.zero;

		private Vector3 m_cached_target_local_ngui_scale = Vector3.one;

		private bool m_cached_visibility	= false;

		public List<FxWatcherShadow> m_shadow_list = new List<FxWatcherShadow>();

		public FxWatcher( GameObject p_target_ngui_gb ){
			m_target_ngui_gb = p_target_ngui_gb;

			{
				m_target_ngui_ui_root_gb = NGUIHelper.GetUIRoot( m_target_ngui_gb );

				Camera[] t_cams = m_target_ngui_ui_root_gb.GetComponentsInChildren<Camera>();
				
				for( int i = 0; i < t_cams.Length; i++ ){
					m_target_ngui_ui_root_cams.Add( t_cams[ i ] );
				}
			}

			UpdateCachedVisibility();

			UpdateCachedTransform();
		}

		/// if we have fx for this target.
		public bool HaveFx(){
			return m_shadow_list.Count > 0 ? true : false;
		}

		public void AddWatchItem( GameObject p_watch_shadow, bool p_sync_ngui_tran, GameObject p_target_ngui_center_gb ){
			if( ContainWatchItem( p_watch_shadow ) ){
				Debug.LogError( "Item Already been Watched: " + p_watch_shadow );
				
				return;
			}

			FxWatcherShadow t_shadow = new FxWatcherShadow( p_watch_shadow, p_sync_ngui_tran, p_target_ngui_center_gb );
			
			m_shadow_list.Add( t_shadow );
		}

		private bool ContainWatchItem( GameObject p_watch_item ){
			for( int i = 0; i < m_shadow_list.Count; i++ ){
				FxWatcherShadow t_item = m_shadow_list[ i ];

				if( t_item.m_shadow_gb == p_watch_item ){
					return true;
				}
			}

			return false;
		}

		public void Clear(){
#if DEBUG_UI_EFFECT
            Debug.Log("Clear() target --->" + m_target_ngui_gb );
#endif

			int t_count = m_shadow_list.Count;

			for( int i = t_count - 1; i >= 0; i-- ){
				GameObject t_fx = m_shadow_list[ i ].m_shadow_gb;

				if( t_fx != null ){
					t_fx.SetActive( false );
					
					Destroy( t_fx );
				}
				
				m_shadow_list.Remove( m_shadow_list[ i ] );
			}

			UI3DEffectTool.Instance().m_fx_watcher_list.Remove( this );
		}

		public void Update(){
			if( ShouldClear() ){
                Clear();

				return;
			}

			UpdateVisiblility();

			UpdateTransform();
		}

		private void UpdateVisiblility(){
			if( m_target_ngui_gb == null ){
				return;
			}

			if( m_cached_visibility == GetTargetNGUIVisibility() ){
				return;
			}
			else{
				m_cached_visibility = GetTargetNGUIVisibility();
			}

			{
				ForceUpdateVisibility();
			}
		}

		public string GetTargetDesc(){
			if( m_target_ngui_gb == null ){
				return "";
			}
			else{
				return GameObjectHelper.GetGameObjectHierarchy( m_target_ngui_gb );
			}
		}

		public bool GetCachedVisibility(){
			return m_cached_visibility;
		}

		public bool GetTargetNGUIVisibility(){
//			return m_target_ngui_gb.activeInHierarchy;

			if( m_target_ngui_gb == null ){
				return false;
			}

			if( m_target_ngui_ui_root_cams.Count > 0 ){
				bool t_cam_gb_visible = m_target_ngui_ui_root_cams[ 0 ].gameObject.activeInHierarchy;

				bool t_cam_visible = m_target_ngui_ui_root_cams[ 0 ].enabled;
				
				if( !t_cam_gb_visible ){
					return false;
				}
				else{
					if( t_cam_visible ){
//						return m_target_ngui_gb.activeInHierarchy;	

						if( m_target_ngui_ui_root_cams[ 0 ].cullingMask == 0 ){
							return false;
						}
						else{
							return m_target_ngui_gb.activeInHierarchy;	
						}

//						if( ( m_target_ngui_ui_root_cams[ 0 ].cullingMask & m_target_ngui_gb.layer ) > 0 ){
//							return m_target_ngui_gb.activeInHierarchy;	
//						}
//						else{
//							return false;
//						}
					}
					else{
						return false;
					}
				}
			}
			else{
				Debug.LogError( "Error, no cam exist." );
				
				return m_target_ngui_gb.activeInHierarchy;
			}
		}

		public void ForceUpdateVisibility(){
			int t_count = m_shadow_list.Count;
			
			for( int i = t_count - 1; i >= 0; i-- ){
				FxWatcherShadow t_shadow = m_shadow_list[ i ];
				
//				#if DEBUG_UI_EFFECT
//				Debug.Log( "Update Visibility to: " + m_cached_visibility );						
//				#endif
				
				t_shadow.UpdateVisibility( m_cached_visibility );
			}
		}

		private void UpdateTransform(){
			if( m_target_ngui_gb == null ){
				return;
			}

			if( m_cached_target_local_ngui_pos == m_target_ngui_gb.transform.position &&
			   m_cached_target_local_ngui_scale == m_target_ngui_gb.transform.lossyScale &&
			   m_cached_target_ngui_rot == m_target_ngui_gb.transform.eulerAngles ){
				return;
			}

			#if DEBUG_UI_EFFECT
//			Debug.Log( "FxWatcher.UpdateTransform( " + m_target_ngui_gb + " - " + 
//			          m_shadow_list.Count + " )" );
			#endif

			{
				m_cached_target_local_ngui_pos = TransformHelper.GetLocalPositionInUIRoot( m_target_ngui_gb );

				m_cached_target_local_ngui_scale = TransformHelper.GetLocalScaleInUIRoot( m_target_ngui_gb );
								
//				Debug.Log( "Local Pos Updated: " + m_cached_local_ngui_pos );
//
//				Debug.Log( "Local Scale Updated: " + m_cached_local_ngui_scale );

				int t_count = m_shadow_list.Count;

				for( int i = t_count - 1; i >= 0; i-- ){
					FxWatcherShadow t_shadow = m_shadow_list[ i ];

					if( t_shadow.m_shadow_widget != null ){
						t_shadow.UpdateNGUITran( m_cached_target_local_ngui_pos, m_cached_target_ngui_rot, m_cached_target_local_ngui_scale );

						#if DEBUG_UI_EFFECT
//						Debug.Log( "Update NGUI Widget: " + t_shadow.m_shadow_gb.name + " - " +
//						          m_cached_target_local_ngui_pos + " - " + 
//						          m_cached_target_local_ngui_scale );
						#endif
					}
					else{
						t_shadow.UpdateFxPosition( m_cached_target_local_ngui_pos );

						#if DEBUG_UI_EFFECT
//						Debug.Log( "Update Fx Pos: " + t_shadow.m_shadow_gb.name + " - " +
//						          m_cached_target_local_ngui_pos );
						#endif
					}
				}
			}

			{
				UpdateCachedTransform();
			}
		}

		private void UpdateCachedVisibility(){
			m_cached_visibility = GetTargetNGUIVisibility();

			ForceUpdateVisibility();
		}

		private void UpdateCachedTransform(){
			m_cached_target_local_ngui_pos = m_target_ngui_gb.transform.position;

			m_cached_target_ngui_rot = m_target_ngui_gb.transform.eulerAngles;
			
			m_cached_target_local_ngui_scale = m_target_ngui_gb.transform.lossyScale;
		}

		public bool ShouldClear(){
			if( m_target_ngui_gb == null ){
#if DEBUG_UI_EFFECT
                Debug.Log( "Target_ngui_gb = null." );
#endif

				return true;
			}

			#if ALWAYS_RETAIN
			return false;
			#endif

			
			if( !m_target_ngui_gb.activeSelf ){
#if DEBUG_UI_EFFECT
                Debug.Log("m_target_ngui_gb.activeSelf = false.");
#endif

				return true;
			}

			if( !GetTargetNGUIVisibility() ){
#if DEBUG_UI_EFFECT
				Debug.Log("GetTargetNGUIVisibility() = false.");
#endif

				return true;
			}

			return false;
		}
	}

	#endregion



	#region Fx Load List

	private List<FxToLoad> m_fx_to_load_list = new List<FxToLoad>();

	private void AddToLoadList( UIType p_ui_type, GameObject p_ngui_gb, string p_fx_path, UI3DEffectLoadDelegate p_call_back, GameObject p_target_ngui_center_gb = null ){
		FxToLoad t_task = new FxToLoad( p_ui_type, p_ngui_gb, p_fx_path, p_call_back, p_target_ngui_center_gb );

		m_fx_to_load_list.Add( t_task );
	}

	private void UpdateToLoad(){
		if( m_fx_to_load_list.Count <= 0 ){
			return;
		}

		while( true ){
			FxToLoad t_task = m_fx_to_load_list[ 0 ];
			
			if( t_task.IsReadyToLoad() ){
				t_task.ExeLoad();
			}
			
			if( !t_task.IsDone() ){
				#if DEBUG_UI_EFFECT
				Debug.Log( "Waiting For Fx to Load." );
				#endif

				return;
			}

			m_fx_to_load_list.Remove( t_task );

			if( m_fx_to_load_list.Count == 0 ){
				break;
			}
		}
	}

	private static void ClearAllFxInToLoad(){
		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return;
		}

		Instance().m_fx_to_load_list.Clear();
	}

	private static void ClearFxInToLoad( GameObject p_gb ){
		if( !HaveInstance() ){
			Debug.Log( "UI3DEffectTool.Not.Exist." );

			return;
		}

		for (int i = Instance().m_fx_to_load_list.Count - 1; i >= 0; i--) {
			FxToLoad t_task = Instance().m_fx_to_load_list[ i ];

			if( t_task.GetNGUIGameObject() == p_gb ){
//				Debug.Log( "Remove loading and not loaded 3d fx." );

				Instance().m_fx_to_load_list.Remove( t_task );
			}
		}
	}

	public delegate void UI3DEffectLoadDelegate( UIType p_ui_type, GameObject p_ngui_gb, GameObject p_3d_effect_gb, GameObject p_ngui_center_gb );

	private class FxToLoad{
		private enum LoadState{
			Ready_To_Load = 0,
			Loading,
			Done,
		}

		private UIType m_ui_type = UIType.None;

		private LoadState m_load_state = LoadState.Ready_To_Load;

		private GameObject m_ngui_gb;

		private GameObject m_target_ngui_center_gb;

		private string m_fx_path;

		private UI3DEffectLoadDelegate m_call_back;

		public FxToLoad( UIType p_ui_type, GameObject p_ngui_gb, string p_fx_path, UI3DEffectLoadDelegate p_call_back, GameObject p_target_ngui_center_gb ){
			if( p_ngui_gb == null ){
				Debug.LogError( "Error NGUI GameObject = null" );
			}

			if( string.IsNullOrEmpty( p_fx_path ) ){
				Debug.LogError( "Error Fx Path IsNullOrEmpty." );
			}

			if( p_call_back == null ){
				Debug.LogError( "Error Res Loaded Callback is Null." );
			}

			if( p_ui_type == UIType.None ){
				Debug.LogError( "Error In UIType." );
			}

			m_ui_type = p_ui_type;

			m_load_state = LoadState.Ready_To_Load;

			m_ngui_gb = p_ngui_gb;

			m_target_ngui_center_gb = p_target_ngui_center_gb;

			m_fx_path = p_fx_path;

			m_call_back = p_call_back;
		}

		public GameObject GetNGUIGameObject(){
			return m_ngui_gb;
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
			GameObject t_gb = (GameObject)Instantiate( p_object );

			// ui effect auto release
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

			if( m_ngui_gb == null ){
				Debug.LogWarning( "FxToLoad.EffectLoadCallback()" );
				return;
			}

//			#if DEBUG_UI_EFFECT
//			Debug.Log( "EffectLoadCallback.NGUI.GB: " + m_ngui_gb );
//			#endif

			try{
				if(m_call_back != null ){
					m_call_back( m_ui_type, m_ngui_gb, t_gb, m_target_ngui_center_gb );
				}
				else{
					Debug.LogError( "call back should not be null." );
				}
			}
			catch( Exception p_e ){
				Debug.LogError( "Error In: " + p_e );
			}

			{
				UI3DEffectTool.Instance().ForceDynamicSetNow();
			}

			{
				Vector3 t_delta_pos = Vector3.zero;

				if( m_target_ngui_center_gb == null ){
					t_delta_pos = TransformHelper.GetLocalPositionInUIRoot( m_ngui_gb );
				}
				else{
					t_delta_pos = TransformHelper.GetLocalPositionInUIRoot( m_target_ngui_center_gb );
				}

//				Debug.Log( "Init Fx Pos: " + t_delta_pos );

				t_gb.transform.localPosition = t_delta_pos * UI3DEffectTool.Instance().m_c_factor;

				{
					CleanFx( t_gb );

					SoundHelper.PlayFxSound( t_gb, m_fx_path );
				}
			}
		}

		public bool IsReadyToLoad(){
			return ( m_load_state == LoadState.Ready_To_Load );
		}

		public bool IsDone(){
			return ( m_load_state == LoadState.Done );
		}

		private void CleanFx( GameObject p_gb ){
			if( p_gb == null ){
				Debug.LogWarning( "Error, p_gb = null." );
				
				return;
			}
			
			int t_child_count = p_gb.transform.childCount;
			
			{
				for( int i = 0; i < t_child_count; i++ ){
					Transform t_child = p_gb.transform.GetChild( i );
					
					CleanFx( t_child.gameObject );
				}
				
				{
					ParticleSystem t_ps = p_gb.GetComponent<ParticleSystem>();

					if( t_ps != null ){
						return;
					}

//					Renderer t_renderer = p_gb.GetComponent<Renderer>();
//
//					if( t_renderer != null ){
//						t_renderer.gameObject.transform.localScale = t_renderer.gameObject.transform.localScale * UI3DEffectTool.Instance().m_c_factor;
//					}
				}
			}
		}
	}

	#endregion



	#region Utilities

	private static void DebugNGUIObject( GameObject p_ngui_gb ){
		GameObjectHelper.LogGameObjectHierarchy( p_ngui_gb );

		TransformHelper.LogPosition( p_ngui_gb, "UI3DEffectTool" );
	}

	#endregion



//	#region Debug Labels
//
//	public static void SetDebugInfo( int p_lb_index, string p_info ){
//		if (p_lb_index < 0 || p_lb_index >= Instance().m_lb_debug.Length) {
//			Debug.Log( "Error, str: " + p_lb_index + " - " + p_info );
//
//			return;
//		}
//
//		Instance().m_lb_debug [p_lb_index].text = p_info;
//
//		{
//			for( int i = Instance().m_lb_debug.Length - 1; i >= 0; i-- ){
//				Instance().m_lb_debug[ i ].gameObject.SetActive( ConfigTool.GetBool (ConfigTool.CONST_SHOW_DEBUG_INFO ) );
//			}
//		}
//	}
//
//	#endregion
}
