//#define DEBUG_ENTER_NEXT_SCENE

//#define DEBUG_SHOW_LOADING_INFO

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

//	private AsyncOperation m_async;


	private static EnterNextScene m_instance;

	public static EnterNextScene Instance(){
		return m_instance; 
	}

	#region Mono
	
	void Awake(){
//		Debug.Log( "EnterNextScene.Awake()" );

		m_instance = this;
	}

	void Start(){
//		Debug.Log( "EnterNextScene.Start()" );

		{
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_TOTAL_LOADING_TIME, true ) ){
				Debug.Log( "------------------" +
				          "Reset Loading Time" +
				          "------------------" );
			}

			ResetLoadingInfo();
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
	}

	/** Notes:
	 * bug fixed, NEVER destroy here.
	 * destroy in ManualDestroy.
	 */
	void OnDestroy(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "EnterNextScene.OnDestroy()" );
		#endif
	}

	void Update(){
//		if( m_async != null ){
//			Debug.Log( "m_async.progress: " + m_async.progress );
//
//			m_slider_progress.value = m_async.progress * 100;
//
//			StaticLoading.UpdatePercentag( StaticLoading.m_loading_sections,
//			                              StaticLoading.CONST_COMMON_LOADING_SCENE, m_async.progress );
//		}

		{
			float t_percentage = StaticLoading.GetLoadingPercentage( StaticLoading.m_loading_sections );

//			Debug.Log( "StaticLoading.Percentage: " + t_percentage );

			if( t_percentage < m_preserve_percentage ){
				t_percentage = m_preserve_percentage;
			}

			if( t_percentage < m_slider_progress.value ){
//				Debug.Log( "Percentage error: " + t_percentage + " / " + m_slider_progress.value );
			}
			else{
				m_slider_progress.value = t_percentage;
			}
		}

       // progress log
        //{
        //    StaticLoading.LogLoadingInfo(StaticLoading.m_loading_sections);
        //}

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

		if ( LoadingHelper.IsLoadingLogin()) {
            Prepare_For_Login();
        }
		else if ( LoadingHelper.IsLoadingMainCity() || LoadingHelper.IsLoadingMainCityYeWan() ){
            // loading MainCity
            //  Prepare_For_MainCity();
     
            PrepareForCityLoad.Instance.Prepare_For_MainCity();
        }
		else if ( LoadingHelper.IsLoadingAllianceCity() || LoadingHelper.IsLoadingAllianceCityYeWan()) {
            // Prepare_For_AllianceCity();
    
            PrepareForCityLoad.Instance.Prepare_For_AllianceCity();
        }
		else if ( LoadingHelper.IsLoadingAllianceCityYeWan() || LoadingHelper.IsLoadingAllianceTenentsCity())
        {
            PrepareForCityLoad.Instance.Prepare_For_AllianceCity();
        }
		else if ( LoadingHelper.IsLoadingAllianceTenentsCity() ){
          //  Prepare_For_AllianceCity();
        }
		else if ( LoadingHelper.IsLoadingBattleField() ) {
            // BattleField
			gameObject.AddComponent<PrepareForBattleField>();
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
		Debug.Log( "Load Level Time: " + m_load_level_time );
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

		m_load_level_time = Time.realtimeSinceStartup;

		{
			SceneManager.LoadLevel( GetSceneToLoad() );
		}

		m_load_level_time = Time.realtimeSinceStartup - m_load_level_time;

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "LoadLevelAsync.Level.Load.Done()" );
		#endif

		while( Application.loadedLevelName != GetSceneToLoad() ){
			#if DEBUG_ENTER_NEXT_SCENE
			Debug.Log( "Waiting For Loading." );
			#endif

			yield return new WaitForEndOfFrame();
		}

		{
            CityGlobalData.m_PlayerInCity = true;
            PrepareWhenSceneLoaded();
		}

		{
			SceneManager.UpdateSceneStateByLevel();
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
//		Debug.Log( "EnterNextScene.DestroyForNextLoading()" );

		// only for loading & loading
		{
			m_preserve_percentage = StaticLoading.GetLoadingPercentage( StaticLoading.m_loading_sections );
		}

		UnRegister();

		DestroyUI();
	}

	// for double loading use.
	public void ManualDestroyImmediate(){
		Debug.Log ( "ManualDestroyImmediate()" );

		{
			m_preserve_percentage = 0.0f;
		}
		
		{
			UnRegister();
		}
		
		{
			DestroyUI();
		}
	}

	// unregister and destroy UI if needed.
	private void ManualDestroy(){
//		Debug.Log( "EnterNextScene.ManualDestroy()" );

		{
			m_preserve_percentage = 0.0f;
		}
		
		{
			UnRegister();
		}

		if( IsDestroyUIWhenLevelLoaded() ){
			StartCoroutine( DelayDestroy() );
		}
	}

	IEnumerator DelayDestroy(){
		yield return new WaitForEndOfFrame();
		
		{
			DestroyUI();
		}
	}

	private void UnRegister(){
		//SocketTool.UnRegisterSocketListener(this);

		// move to destroy UI
//		m_instance = null;
	}

	/// call this to destroy loading UI when loading is done.
	public void DestroyUI(){
		{
			m_instance = null;
		}

		if ( StaticLoading.Instance () != null ) {
			StaticLoading.Instance ().ManualDestroy ();
		}
		else {
			Debug.LogError( "Never Should be Here." );
		}
		
		{
			gameObject.SetActive( false );
			
			Destroy( gameObject );
		}

		{
			UtilityTool.Instance.DelayedUnloadUnusedAssets();
		}
	}

	#endregion



	#region Prepare After Scene Loaded

	private List<EventDelegate> m_loading_done_callback_list = new List<EventDelegate>();

	public void AddLoadingDoneCallback( EventDelegate.Callback p_callback ){
		EventDelegate.Add( m_loading_done_callback_list, p_callback );
	}

	/// Called when Next Scene Loaded.
	private void PrepareWhenSceneLoaded(){
		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded()" );
		#endif

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_TOTAL_LOADING_TIME, true ) ){
			Debug.Log( "------------------" + 
			          " EnterNextScene: " + GetSceneToLoad() + " - " +
			          GetTimeSinceLoading() + 
			          " ------------------" );
		
			#if DEBUG_SHOW_LOADING_INFO
			ShowTotalLoadingInfo();

			Bundle_Loader.LogCoroutineInfo();
			
			ShowLoadLevelTime();
			#endif
		}

		if( Global.m_is_loading_from_bundle ){
			Bundle_Loader.CheckAllShaders();
		}

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.Light()" );
		#endif

		{
			LoadingHelper.ConfigBloomAndLight();
		}

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.AA()" );
		#endif

		#if DEBUG_ENTER_NEXT_SCENE
		Debug.Log( "PrepareWhenSceneLoaded.Before.Callback()" );
		#endif

		EventDelegate.Execute( m_loading_done_callback_list );

		#if DEBUG_ENTER_NEXT_SCENE
		EnterNextScene.LogTimeSinceLoading( "PrepareWhenSceneLoaded" );
		#endif
	}

	#endregion



	#region Common Init

	private static float m_begin_loading_time	= 0.0f;

	private static float m_last_tagged_time		= 0.0f;

	private static int m_asset_load_count 		= 0;

	private static void ResetLoadingInfo(){
		// reset local info
		{
			m_begin_loading_time = Time.realtimeSinceStartup;
			
			m_last_tagged_time = Time.realtimeSinceStartup;
			
			m_asset_load_count = 0;
			
			Bundle_Loader.ResetCoroutineInfo();
		}

		// reset detail info
		{
			LoadingHelper.ClearLoadingItemInfo();
		}
	}

	public static int GetAssetLoadedCount(){
		return m_asset_load_count;
	}

	public static void AssetLoaded(){
		m_asset_load_count++;
	}

	public static float GetTimeSinceLoading(){
		return Time.realtimeSinceStartup - m_begin_loading_time;
	}

	public static float GetTimesinceLastTimeTag( bool p_retag = true ){
		float t_delta = Time.realtimeSinceStartup - m_last_tagged_time;

		if( p_retag ){
			m_last_tagged_time = Time.realtimeSinceStartup;
		}

		return t_delta;
	}

	public static void LogTimeSinceLoading( string p_loading_tag ){
		Debug.Log( MathHelper.FloatPrecision( GetTimesinceLastTimeTag(), 5 ) + 
		          " / " + 
		          MathHelper.FloatPrecision( GetTimeSinceLoading(), 5 ) + " - " + 
		          p_loading_tag );
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
//		StaticLoading.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_COMMON_LOADING_SCENE, 2, -1 );

		StaticLoading.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_MAINCITY_NETWORK, 1, REQUEST_DATA_COUNT_FOR_MAINCITY );
	}
 
    private bool _isEnterMainCity = true;
    IEnumerator CheckingDataForMainCity(){
		while( m_received_data_for_main_city < REQUEST_DATA_COUNT_FOR_MAINCITY ){
			yield return new WaitForEndOfFrame();
		}

		// enter pve for 1st battle.
		if( Global.m_iScreenID == 100101 || Global.m_iScreenID == 100102 || Global.m_iScreenID == 100103){
            //			Debug.Log( "CheckingDataForMainCity.EnterBattlePve()" );
            _isEnterMainCity = false;
            DestroyForNextLoading();

			EnterBattleField.EnterBattlePve( 1, Global.m_iScreenID % 10, LevelType.LEVEL_NORMAL );
		}
		else{
//			SetAutoActivation( true );

			DirectLoadLevel();
		}

        if (m_received_data_for_main_city == REQUEST_DATA_COUNT_FOR_MAINCITY && _isEnterMainCity)
        {
   
        }
	}

	#endregion



	#region Prepare For Alliance City

	private int m_received_data_for_alliance_city = 0;
	
	private const int REQUEST_DATA_COUNT_FOR_ALLIANCECITY = 4;
	
	private void InitAllianceCityLoading(){
//		StaticLoading.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_COMMON_LOADING_SCENE, 2, -1 );
		
		StaticLoading.InitSectionInfo( StaticLoading.m_loading_sections, StaticLoading.CONST_MAINCITY_NETWORK, 1, REQUEST_DATA_COUNT_FOR_ALLIANCECITY );
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

	///	Every Proto Only Could Have 1 Processor, but Many Listener.
	/// MainCity MAY Have Processors.
//	public bool OnSocketEvent( QXBuffer p_message ){
////		Debug.Log ("p_message:" + p_message);
//		if( p_message == null ){
//			return false;
//		}

//		switch( p_message.m_protocol_index ){
////			case ProtoIndexes.S_MIBAO_INFO_RESP:{
////
////			    MiBaoGlobleData.Instance().OnProcessSocketMessage(p_message);
////				Debug.Log ("秘宝info：" + ProtoIndexes.S_MIBAO_INFO_RESP);
////				m_received_data_for_main_city++;
////
////			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
////			                         StaticLoading.CONST_MAINCITY_NETWORK, "S_MIBAO_INFO_RESP" );
////
////				return true;
////			}

//			case ProtoIndexes.PVE_PAGE_RET:{
////				Debug.Log( "PveInfoResp:" + ProtoIndexes.PVE_PAGE_RET );

//				ProcessPVEPageReturn( p_message );

//				m_received_data_for_main_city++;

//			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
//			                         StaticLoading.CONST_MAINCITY_NETWORK, "PVE_PAGE_RET" );
				
//				return true;
//			}
				
////			case ProtoIndexes.S_EquipInfo:{
////				Debug.Log( "OnSocketEvent: TanBaoYinDaoCol()" );
////
////				EquipsOfBody.Instance().ProcessEquipInfo( p_message );
////
////				m_received_data_for_main_city++;
////
////			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
////			                         StaticLoading.CONST_MAINCITY_NETWORK, "S_EquipInfo" );
////
////				return true;
////			}

//			case ProtoIndexes.JunZhuInfoRet:{

////				Debug.Log( "获得君主数据: " + Global.m_iScreenID );
				
//				m_received_data_for_main_city++;

//			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
//			                         StaticLoading.CONST_MAINCITY_NETWORK, "JunZhuInfoRet" );
				
//				return true;
//			}

//		    case ProtoIndexes.ALLIANCE_HAVE_RESP:{
				
////				Debug.Log ("获得有联盟信息");
				
//				m_received_data_for_main_city++;

//			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
//			                         StaticLoading.CONST_MAINCITY_NETWORK, "ALLIANCE_HAVE_RESP" );
				
//				return true;
//			}

//			case ProtoIndexes.ALLIANCE_NON_RESP:{
			
////				Debug.Log ("获得无联盟信息");
			
//				m_received_data_for_main_city++;
			
//			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
//			                         StaticLoading.CONST_MAINCITY_NETWORK, "ALLIANCE_NON_RESP" );
			
//			return true;
//			}

//			case ProtoIndexes.S_TaskList:{
			
////				Debug.Log( "获得主线任务: " + Global.m_iScreenID );
			
//				m_received_data_for_main_city++;
			
//			StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
//			                         StaticLoading.CONST_MAINCITY_NETWORK, "S_TaskList" );
			
//			return true;
//		}

//			default:{
//				return false;
//			}
//		}
//	}

	#endregion



	#region Network Listeners

	/// Refresh PVE Data
	private void ProcessPVEPageReturn( QXBuffer p_buffer ){
//		Debug.Log( "ProcessPVEPageReturn( 获得了管卡数据 )" );

		MemoryStream t_stream = new MemoryStream( p_buffer.m_protocol_message, 0, p_buffer.position );
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		Section tempInfo = new Section();
		
		t_qx.Deserialize( t_stream, tempInfo, tempInfo.GetType() );

		if(tempInfo.s_allLevel == null)
		{
			Debug.Log( "tempInfo.s_allLevel == null" );
		}
		if(tempInfo.maxCqPassId > CityGlobalData.m_temp_CQ_Section)
		{
			CityGlobalData.m_temp_CQ_Section = tempInfo.maxCqPassId;
		}

		foreach( Level tempLevel in tempInfo.s_allLevel ){
			if( !tempLevel.s_pass ){

				Global.m_iScreenID = tempLevel.guanQiaId;
				
				break;
			}
		}
	}

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
		
		m_to_load_scene_name = p_to_load_scene_name;

		m_destroy_ui_when_level_loaded = p_destroy_ui_when_level_loaded;
	}
	
	public static string GetSceneToLoad(){
		return m_to_load_scene_name;
	}

	public static bool IsDestroyUIWhenLevelLoaded(){
		return m_destroy_ui_when_level_loaded;
	}
	
	#endregion



	#region Utilities

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
