//#define DEBUG_ENTER_NEXT_SCENE

//#define DEBUG_SHOW_LOADING_INFO

//#define DEBUG_SHOW_LOADING_TIME



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.1
 * @since:		Unity 4.5.3
 * Function:	Exist in Loading Scene, prepare loading, exes scene switches.
 * 
 * Notes:
 * None.
 */ 
public class EnterNextScene : MonoBehaviour{
	
	/// 加载进度条
	public UISlider m_slider_progress;

	public UILabel m_lb_debug;

	/// Background Image
	private UITexture m_background_image;

	// wait frame count when loading done
	private static int m_loading_done_waiting_frame	= 3;

	private static EnterNextScene m_instance;

	public static EnterNextScene Instance(){
		return m_instance; 
	}

	#region Mono
	
	void Awake(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Awake()" );
		#endif

		m_instance = this;

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Awake.Done()" );
		#endif
	}

	void Start(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Start()" );
		#endif

		{
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_TOTAL_LOADING_TIME, true ) ){
				Debug.Log( "------------------" +
				          "Reset Loading Time( " + GetSceneToLoad() + " )" + 
				          "------------------" +
				          Time.realtimeSinceStartup );
			}

			LoadingHelper.ResetLoadingInfo();
		}

		// Find Real Background
		{
			m_background_image = StaticLoading.Instance().m_loading_bg;
		}

		{
			DontDestroyOnLoad( gameObject );

			DontDestroyOnLoad( StaticLoading.Instance().m_loading_bg_root );
		}

		//SocketTool.RegisterSocketListener( this );

		PrepareToLoadScene();

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Start.Done()" );
		#endif
	}

	void Update(){
		{
			float t_percentage = LoadingHelper.GetLoadingPercentage( StaticLoading.m_loading_sections );

			#if DEBUG_ENTER_NEXT_SCENE
			Debug.Log( "EnterNextScene.Update.Loading.Bar( " + t_percentage + " )" );
			#endif

			if( t_percentage < m_preserve_percentage ){
				t_percentage = m_preserve_percentage;
			}

			if( t_percentage < m_slider_progress.value ){
//				Debug.Log( "Percentage error: " + t_percentage + " / " + m_slider_progress.value );
			}
			else{
				m_slider_progress.value = t_percentage;
			}

			{
				UpdateFx();
			}
		}

        if ( ConfigTool.GetBool( ConfigTool.CONST_SHOW_CURRENT_LOADING ) ){
			if( m_loading_asset_changed ){
				{
					SetLoadingAssetChanged( false );
					
					m_lb_debug.gameObject.SetActive( true );
				}
				
				if( m_lb_debug != null ){
					m_lb_debug.text = StaticLoading.GetCurLoading();
				}
			}
		}

//		#if DEBUG_SHOW_LOADING_INFO
//		Debug.Log( "EnterNextScene.Update.Done()" );
//		#endif
	}

	/** Notes:
	 * bug fixed, NEVER destroy here.
	 * destroy in ManualDestroy.
	 */
	void OnDestroy(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.OnDestroy()" );
		#endif

		Clear();
	}

	void Clear(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Clear()" );
		#endif

		m_instance = null;

		m_background_image = null;
	}


	#endregion



	#region Prepare To Load Scene

	/// Called when Start(), then try to start loading.
	void PrepareToLoadScene(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.PrepareToLoadScene()" );
		#endif

		if( SocketTool.IsConnected() ){
//			Debug.Log( "--- Scene Tag --- EnterNextScene.PrepareToLoadScene --- State_LOADINGSCENE" );
			
			PlayerState t_state = new PlayerState();
			
			t_state.s_state = State.State_LOADINGSCENE;
			
			SocketHelper.SendQXMessage( t_state, ProtoIndexes.PLAYER_STATE_REPORT );
		}
		else{
//			Debug.LogError( "Error, Socket not Connected." );
		}

		if ( LoadingHelper.IsLoadingLogin() ){
            Prepare_For_Login();
        }
		else if ( LoadingHelper.IsLoadingMainCity() || LoadingHelper.IsLoadingMainCityYeWan() ){
            // loading MainCity
            //  Prepare_For_MainCity();
     
			{
				PrepareForCityLoad t_com = gameObject.AddComponent<PrepareForCityLoad>();

				t_com.Prepare_For_MainCity();
			}
        }
		else if ( LoadingHelper.IsLoadingAllianceCity() || LoadingHelper.IsLoadingAllianceCityYeWan()) {
            // Prepare_For_AllianceCity();
        }
		else if ( LoadingHelper.IsLoadingAllianceCityYeWan() || LoadingHelper.IsLoadingAllianceTenentsCity())
        {
          	
        }
		else if ( LoadingHelper.IsLoadingAllianceTenentsCity() ){
          //  Prepare_For_AllianceCity();
        }
		else if ( LoadingHelper.IsLoadingBattleField() ) {
            // BattleField
			gameObject.AddComponent<PrepareForBattleField>();
        }
		else if ( LoadingHelper.IsLoadingAllianceBattle() ) {
            // PrepareForAllianceBattle
		    gameObject.AddComponent<PrepareForAllianceBattle>();
		}
		else if ( LoadingHelper.IsLoadingCarriage() ) {
            // PrepareForCarriage
            gameObject.AddComponent<PrepareForCarriage>();
		}
		else if ( LoadingHelper.IsLoadingTreasureCity() )
		{
			// PrepareForTreasureCity
			gameObject.AddComponent<PrepareForTreasureCity>();
		}
        else {
            // load scenes
            DirectLoadLevel();
        }
	}

	#endregion


	
	#region Direct Load Level

	private static float m_load_level_time = 0.0f;

	private void ShowLoadLevelTime(){
		Debug.Log( "--- Load Level Time: " + m_load_level_time );
	}
	
	/// Load the Scene Directly.
	public static void DirectLoadLevel(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.DirectLoadLevel( " + m_to_load_scene_name + " )" );
		#endif

		Global.LoadLevel( GetSceneToLoad(), Instance().LoadLevelDone );
	}

	public void LoadLevelDone( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.LoadLevelDone()" );
		#endif

		StartCoroutine( LoadLevelNow() );
	}

	/// Load the Scene Directly.
	IEnumerator LoadLevelNow(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "---------------- LoadLevelNow( " + GetSceneToLoad() + " ) ---------------" );
		#endif

		#if DEBUG_SHOW_LOADING_TIME
		LoadingHelper.LogTimeSinceLoading( "EnterNextScene.LoadLevelNow()" );
		#endif

		string t_to_load_scene_name = GetSceneToLoad();

		{
			m_load_level_time = Time.realtimeSinceStartup;

			SceneManager.LoadLevel( t_to_load_scene_name );

			m_load_level_time = Time.realtimeSinceStartup - m_load_level_time;

			{
				ClearSceneToLoad();
			}
		}

		#if DEBUG_SHOW_LOADING_TIME
		LoadingHelper.LogTimeSinceLoading( "LoadLevel - EnterNextScene.LoadLevelNow.Done()" );
		#endif

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "LoadLevelAsync.Level.Load.Done()" );
		#endif

//		float t_wait_start = Time.realtimeSinceStartup;
//
//		while( Application.loadedLevelName != t_to_load_scene_name ){
//			#if DEBUG_ENTER_NEXT_SCENE
//			Debug.Log( "Waiting For Loading." );
//			#endif
//
//			yield return new WaitForEndOfFrame();
//		}
//
//		Debug.Log( "Wait For ApplicationLevelNameChange: " + ( Time.realtimeSinceStartup - t_wait_start ) );

		{
			CityGlobalData.m_PlayerInCity = true;

            PrepareWhenSceneLoaded();
		}

		{
			SceneManager.UpdateSceneStateByLevel( t_to_load_scene_name );
		}

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "------------------------Level Change: " + Application.loadedLevelName );
		#endif

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.Destroy()" );
		#endif

		ManualDestroy();

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.After.Destroy()" );
		#endif

		yield return null;
	}

	#endregion



	#region Destroy UI

	private static float m_preserve_percentage = 0.0f;

	// TODO, check if is for new player use.
	private void DestroyForNextLoading(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.DestroyForNextLoading()" );
		#endif

		// only for loading & loading
		{
			m_preserve_percentage = LoadingHelper.GetLoadingPercentage( StaticLoading.m_loading_sections );
		}

		UnRegister();

		DestroyUI();
	}

	// for double loading use.
	public void ManualDestroyImmediate(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log ( "ManualDestroyImmediate()" );
		#endif

		DestroyUIImmediately();
	}

	// unregister and destroy UI if needed.
	private void ManualDestroy(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.ManualDestroy( " +  IsDestroyUIWhenLevelLoaded() + " )" );
		#endif

		{
			m_preserve_percentage = 0.0f;
		}
		
		{
			UnRegister();
		}

		if( IsDestroyUIWhenLevelLoaded() ){
			StartCoroutine( DelayDestroy() );
		}
		else{
			ExecuteLoadingDoneCallback();
		}
	}

	IEnumerator DelayDestroy(){
		if( m_loading_done_waiting_frame <= 0 ){
			m_loading_done_waiting_frame = 1;
		}

		for( int i = 0; i < m_loading_done_waiting_frame; i++ ){
			#if DEBUG_ENTER_NEXT_SCENE
			Debug.Log( "Waiting Frame Count: " + i );
			#endif

			yield return new WaitForEndOfFrame();	
		}

		{
			DestroyUIImmediately();
		}
	}

	private void UnRegister(){
		//SocketTool.UnRegisterSocketListener(this);

		// move to destroy UI
//		m_instance = null;
	}

	/// call this to destroy loading UI when loading is done.
	public void DestroyUI(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.DestroyUI()" );
		#endif

		StartCoroutine( DelayDestroy() );
	}

	private void DestroyUIImmediately(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.DestroyUIImmediately()" );
		#endif

		{
			Clear();
		}

		if ( StaticLoading.Instance() != null ) {
			StaticLoading.Instance().ManualDestroy ();
		}
		else {
			Debug.LogError( "Never Should be Here." );
		}
		
		{
			gameObject.SetActive( false );
			
			Destroy( gameObject );
		}

		{
			ExecuteLoadingDoneCallback();
		}

		{
//			UtilityTool.Instance.DelayedUnloadUnusedAssets();

			UtilityTool.UnloadUnusedAssets();
		}
	}

	#endregion



	#region Prepare After Scene Loaded

	private List<EventDelegate> m_loading_done_callback_list = new List<EventDelegate>();

	public void AddLoadingDoneCallback( EventDelegate.Callback p_callback ){
		EventDelegate.Add( m_loading_done_callback_list, p_callback );
	}

	public void ExecuteLoadingDoneCallback(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.Callback()" );
		#endif

		//		LogDelegate();

		EventDelegate.Execute( m_loading_done_callback_list );
	}

	/// Called when Next Scene Loaded.
	private void PrepareWhenSceneLoaded(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded()" );
		#endif

		#if DEBUG_SHOW_LOADING_TIME
		LoadingHelper.LogTimeSinceLoading( "EnterNextScene.PrepareWhenSceneLoaded()" );
		#endif

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_TOTAL_LOADING_TIME, true ) ){
			Debug.Log( "------------------" + 
			          " EnterNextScene: " + GetSceneToLoad() + " - " +
			          LoadingHelper.GetTimeSinceLoading() + 
			          " ------------------" );

			#if DEBUG_SHOW_LOADING_INFO
			LoadingHelper.ShowTotalLoadingInfo();

			ShowLoadLevelTime();

			LoadingHelper.ShowDetailLoadingInfo();

//			Bundle_Loader.LogCoroutineInfo();
			#endif

//			#if DEBUG_SHOW_LOADING_INFO
//			LoadingHelper.LogLoadingInfo( StaticLoading.m_loading_sections );
//			#endif
		}

		if( Global.m_is_loading_from_bundle ){
			Bundle_Loader.CheckAllShaders();
		}

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.Light()" );
		#endif

		#if DEBUG_SHOW_LOADING_TIME
		LoadingHelper.LogTimeSinceLoading( "EnterNextScene.PrepareWhenSceneLoaded.After.Check()" );
		#endif

		#if DEBUG_SHOW_LOADING_TIME
		LoadingHelper.LogTimeSinceLoading( "EnterNextScene.PrepareWhenSceneLoaded.Done()" );
		#endif
	}

	private void LogDelegate(){
		Debug.Log( "delegate count: " + m_loading_done_callback_list.Count );

		for( int i = 0; i <= m_loading_done_callback_list.Count - 1; i++ ){
			Debug.Log( i + " : " + m_loading_done_callback_list[ i ].ToString() );
		}
	}

	#endregion



	#region Prepare For Login

	private int m_prepare_data_for_login = 0;

	private const int PREPARE_DATA_COUNT_FOR_LOGIN	= 1;

	private void Prepare_For_Login(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Prepare_For_Login()" );
		#endif

		// reset count
		{
			m_prepare_data_for_login = 0;

			StartCoroutine( CheckingDataForLogin() );
		}

		// load res data
		{
			Res2DTemplate.LoadTemplates( LoginDataLoaded );

			LanguageTemplate.LoadTemplates();
		}
	}

	public void LoginDataLoaded(){
		m_prepare_data_for_login++;
	}
	
	IEnumerator CheckingDataForLogin(){
		while( m_prepare_data_for_login < PREPARE_DATA_COUNT_FOR_LOGIN ){
			yield return new WaitForEndOfFrame();
		}

		{
//			SetAutoActivation( true );
			
			DirectLoadLevel();
		}
	}

	#endregion



	#region Prepare For Main City

	/// 是否加载完PVE信息
	private int m_received_data_for_main_city = 0;

	private const int REQUEST_DATA_COUNT_FOR_MAINCITY = 4;

	private void InitMainCityLoading(){
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_MAINCITY_NETWORK, 1, REQUEST_DATA_COUNT_FOR_MAINCITY );
	}
 
    private bool _isEnterMainCity = true;
    IEnumerator CheckingDataForMainCity(){
		while( m_received_data_for_main_city < REQUEST_DATA_COUNT_FOR_MAINCITY ){
			yield return new WaitForEndOfFrame();
		}

		// enter pve for 1st battle.
		if( Global.m_iScreenID == 100101 || Global.m_iScreenID == 100102 || Global.m_iScreenID == 100103)
		{
			Debug.Log( "CheckingDataForMainCity.EnterBattlePve()" );
            _isEnterMainCity = false;
            DestroyForNextLoading();

			EnterBattleField.EnterBattlePve( 1, Global.m_iScreenID % 10, LevelType.LEVEL_NORMAL );
		}
		else{
//			SetAutoActivation( true );

			DirectLoadLevel();
		}

        if (m_received_data_for_main_city == REQUEST_DATA_COUNT_FOR_MAINCITY && _isEnterMainCity){
   
        }
	}

	#endregion



	#region Prepare For Alliance City

	private int m_received_data_for_alliance_city = 0;
	
	private const int REQUEST_DATA_COUNT_FOR_ALLIANCECITY = 4;
	
	private void InitAllianceCityLoading(){
		LoadingHelper.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_MAINCITY_NETWORK, 1, REQUEST_DATA_COUNT_FOR_ALLIANCECITY );
	}

	private void Prepare_For_AllianceCity(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.Prepare_For_AllianceCity()" );
		#endif

		InitAllianceCityLoading();

		// reset info
		{
			m_received_data_for_main_city = 0;
			
			StartCoroutine( CheckingDataForMainCity() );
		}
 		
		// request PVE Info
		{
			JunZhuDiaoLuoManager.RequestMapInfo( -1 );
		}
		
		// request JunZhu Info
		{
 
			JunZhuData.Instance();
			JunZhuData.RequestJunZhuInfo();
		}

		{
//			Debug.Log ("AllianceCityTask");
			TaskData.Instance.RequestData();
		}

		//request Alliance Info
		{
			AllianceData.Instance.RequestData ();
		}

		{			
			TenementData.Instance.RequestData();
		}

        //  request Friend Info
        {
            FriendOperationData.Instance.RequestData();
        }
    }

	#endregion



	#region Common Network

	#endregion



	#region Network Listeners

	#endregion



	#region Config To Load Scene
	
	private static string m_to_load_scene_name = "";

	private static bool m_destroy_ui_when_level_loaded = true;
	
	/// Set Params For Next Scene to Load.
	/// 
	/// Params:
	/// p_to_load_scene_name: Scene name, as in build setting.
	public static void SetSceneToLoad( string p_to_load_scene_name, bool p_destroy_ui_when_level_loaded = true ){
		//		Debug.Log( "-------------- EnterNextScene.SetSceneToLoad( " + 
		//						p_to_load_scene_name + " - " + p_auto_activation + 
		//						" ) --------------" );
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.SetSceneToLoad( " + p_to_load_scene_name + " )" );
		#endif

		if( string.IsNullOrEmpty( p_to_load_scene_name ) ){
			Debug.LogError( "Error, Scene Name is NullorEmpty." );

			return;
		}

		m_to_load_scene_name = p_to_load_scene_name;

		m_destroy_ui_when_level_loaded = p_destroy_ui_when_level_loaded;
	}

	public static void ClearSceneToLoad(){
		m_to_load_scene_name = "";
	}
	
	public static string GetSceneToLoad(){
		return m_to_load_scene_name;
	}

	public static bool IsDestroyUIWhenLevelLoaded(){
		return m_destroy_ui_when_level_loaded;
	}
	
	#endregion



	#region Utilities

	private void UpdateFx(){
		UnityEngine.Object[] t_objs = GameObject.FindObjectsOfType(typeof(UILoadingFx));

		for( int i = 0; i < t_objs.Length; i++ ){
			if( t_objs[ i ] == null ){
				continue;
			}

			UILoadingFx t_fx = (UILoadingFx)t_objs[ i ];

			t_fx.UpdatePos();
		}
	}

	// get background image for optimize useage.
	public static UITexture GetBackgroundImage(){
		return Instance().m_background_image;
	}

	private static bool m_loading_asset_changed = false;

	public static void SetLoadingAssetChanged( bool p_changed ){
		m_loading_asset_changed = p_changed;
	}

	#endregion

}
