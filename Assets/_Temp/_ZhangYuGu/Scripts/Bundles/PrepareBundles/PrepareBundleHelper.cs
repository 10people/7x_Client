#define DEBUG_BUNDLE

#define SHOW_SERVER_SELECTOR



using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class PrepareBundleHelper {

	#region Init for PrepareBundle

	/// Init for prepare bundle class.
	public static void PrepareBundles_Init_In_Awake(){
//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake()" );
//		#endif

		if ( !ShowServerSelector ()) {
			if( PrepareBundles.Instance().m_pop_update_server != null ){
				PrepareBundles.Instance().m_pop_update_server.gameObject.SetActive( false );
			}
			
			if( PrepareBundles.Instance().m_bt_update != null ){
				PrepareBundles.Instance().m_bt_update.gameObject.SetActive( false );
			}
			
			if( PrepareBundles.Instance().m_lb_tips != null ){
				PrepareBundles.Instance().m_lb_tips.gameObject.SetActive( false );
			}
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.1()" );
//		#endif
		
		// set default http server prefix
		if( ShowServerSelector() ){
			PrepareBundleHelper.SetCeshiServer();
			
			//			PrepareBundleHelper.SetDefaultServer();
		}
		else{
			//			PrepareBundleHelper.SetTiYanServer();
			
			PrepareBundleHelper.SetCeshiServer();
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.2()" );
//		#endif

		// init and clean
		{
			VersionTool.Instance().Init();

//			#if DEBUG_BUNDLE
//			Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.After.VersionTool()" );
//			#endif
			
			UtilityTool.LoadBox();

//			#if DEBUG_BUNDLE
//			Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.After.UtilityTool()" );
//			#endif
			
			Bundle_Loader.m_bundle_list_info = null;

//			#if DEBUG_BUNDLE
//			Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.After.Bundle_Loader()" );
//			#endif
			
			PrepareBundleHelper.ClearLoadingSections();
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.3()" );
//		#endif
		
		// 1st & 2nd test in third platform
		{
			{
				GameObject t_gb = GameObjectHelper.GetDontDestroyOnLoadGameObject();
				
				ComponentHelper.AddIfNotExist( t_gb, typeof(ThirdPlatform) );
			}

			GameObjectHelper.RegisterGlobalComponents();
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.4()" );
//		#endif
		
		{
			PathHelper.LogPath();
		}
		
		{
			DeviceHelper.LogDeviceInfo( null );
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.5()" );
//		#endif

		// log
		{
			FileHelper.DeleteLogFile();
			
			Application.logMessageReceived += FileHelper.LogFile;
		}
		
		// report when game launched
		{
			OperationSupport.ReportClientAction( OperationSupport.ClientAction.LAUNCH_GAME );
		}

//		#if DEBUG_BUNDLE
//		Debug.Log( "PrepareBundleHelper.PrepareBundles_Init_In_Awake.Done()" );
//		#endif
	}

	#endregion



	#region Update UI
	
	/// Update UI for preparing and updaing bundles.
	public static void UpdateUI(){
		if( m_loading_asset_changed ){
			SetLoadingAssetChanged( false );
			
			if( PrepareBundles.Instance().m_lb_title != null ){
				PrepareBundles.Instance().m_lb_title.text = GetCurLoadingTitle();
			}
			
			if( PrepareBundles.Instance().m_lb_tips != null ){
				PrepareBundles.Instance().m_lb_tips.text = GetCurLoading();
			}
		}
		
		{
			float t_percentage = LoadingHelper.GetLoadingPercentage( m_loading_sections );
			
			//			Debug.Log( "StaticLoading.Percentage: " + t_percentage );
			
			PrepareBundles.Instance().m_slider_progress.value = t_percentage;
		}
	}
	
	#endregion



	#region Update Bundle Info

	public static void UpdateLocalBundleInfo(){
		if( IsBundleNotExist() || IsToUpdateBundle() ){
			Debug.Log( "Save Version: " + PrepareBundleHelper.GetServerSmallVersion() );
			
			PlayerPrefs.SetString( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION, 
			                      PrepareBundleHelper.GetServerSmallVersion() );
			
			PlayerPrefs.SetInt( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_ROOT_BUNDLE_VERSION, 
			                      PrepareBundleHelper.GetNewRootBundleVersion() );
			
			PlayerPrefs.Save();
		}
	}

	#endregion
	
	

	#region Bundle Update Progress

	private static List<LoadingSection> m_loading_sections = new List<LoadingSection>();

	public static List<LoadingSection> GetLoadingSections(){
		return m_loading_sections;
	}

	/// Clears the loading sections.
	public static void ClearLoadingSections(){
		LoadingHelper.ClearLoadingInfo( m_loading_sections );
	}
	
	public static void InitLoadingSections(){
		LoadingHelper.InitSectionInfo( m_loading_sections, CONST_LOADING_BUNDLE_CONFIG, 1, 1 );

		LoadingHelper.InitSectionInfo( m_loading_sections, CONST_UPDATE_BUNDLES, 10, 0 );

		LoadingHelper.InitSectionInfo( m_loading_sections, CONST_LOADING_PRELOAD_RESOURCES, 1, BundleHelper.PREPARE_DATA_COUNT_FOR_LOADING );
	}

	#endregion



	#region Local Version

	public static string m_config_cached_small_version 		= "";
	
	// load local bundle version
	public static void LoadCachedBundleVersions(){
		m_config_cached_small_version = PlayerPrefs.GetString( 
		                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION, 
		                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT );
		
		if( string.Compare( GetLocalSmallVersion(), ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT ) == 0 ){
			m_config_cached_small_version = VersionTool.GetPackageSmallVersion();

//			#if DEBUG_BUNDLE
//			Debug.Log( "Use Local Resources Small Version: " + m_config_cached_small_version );
//			#endif
		}
	}
	
	public static string GetLocalSmallVersion(){
		return m_config_cached_small_version;
	}

	// not used
	public static void SetLocalSmallVersion( string p_local_small_version ){
		m_config_cached_small_version = p_local_small_version;
	}

	#endregion



	#region Server Version

	private static string m_server_small_version 		= "";

	private static string m_server_big_version			= "";

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android
	private static string m_server_bundle_prefix		= "";

	public static string GetServerSmallVersion(){
		return m_server_small_version;
	}

	public static void SetServerSmallVersion( string p_server_small_version ){
		#if DEBUG_BUNDLE
		Debug.Log( "SetServerSmallVersion( " + p_server_small_version + " )" );
		#endif

		m_server_small_version = p_server_small_version;
	}

	public static string GetServerBigVersion(){
		return m_server_big_version;
	}

	public static void SetServerBigVersion( string p_server_big_version ){
		#if DEBUG_BUNDLE
		Debug.Log( "SetServerBigVersion( " + p_server_big_version + " )" );
		#endif

		m_server_big_version = p_server_big_version;
	}

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android
	/// OS.dataPath/StreamingArchived/Platform
	public static string GetServerBundleUrlPrefix(){
		return m_server_bundle_prefix;
	}

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android
	/// OS.dataPath/StreamingArchived/Platform
	public static void SetServerBundlePrefix( string p_server_url_prefix ){
		#if DEBUG_BUNDLE
		Debug.Log( "SetServerBundlePrefix( " + p_server_url_prefix + " )" );
		#endif

		m_server_bundle_prefix = p_server_url_prefix;
	}

	#endregion



	#region Cached Root Bundle Version

	/// Get cached root bundle version.
	public static int GetCachedRootBundleVersion(){
		int t_cached_root_bundle_version = PlayerPrefs.GetInt( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_ROOT_BUNDLE_VERSION, 
		                                             			ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_ROOT_BUNDLE_VRESION_DEFAULT );

		return t_cached_root_bundle_version;
	}

	private static int m_current_bundle_version	= 0;

	public static int GetCurrentBundleVersion(){
		return m_current_bundle_version;
	}

	public static void SetCurrentBundleVersion( int p_bundle_version ){
		#if DEBUG_BUNDLE
		Debug.Log( "SetCurrentBundleVersion( " + p_bundle_version + " )" );
		#endif

		m_current_bundle_version = p_bundle_version;
	}

	/// Get New Root Bundle's Version
	public static int GetNewRootBundleVersion(){
		return GetCachedRootBundleVersion() + 1;
	}

	#endregion

	
	
	#region Bundle State

	public enum BundleState{
		NotExist,
		ToUpdate,
		Normal,
	}

	private static BundleState m_bundle_state = BundleState.NotExist;
	
	private static void SetBundleState( BundleState p_state ){
		m_bundle_state = p_state;
		
		#if DEBUG_BUNDLE
		Debug.Log( "SetBundleState( " + p_state + " )" );
		#endif
	}
	
	public static bool IsBundleNotExist(){
		return m_bundle_state == BundleState.NotExist;
	}
	
	public static bool IsToUpdateBundle(){
		return m_bundle_state == BundleState.ToUpdate;
	}
	
	public static BundleState GetBundleState(){
		return m_bundle_state;
	}

	public static void UpdateBundleState(){
		#if DEBUG_BUNDLE
		Debug.Log( "local small: " + PrepareBundleHelper.GetLocalSmallVersion() );
		
		Debug.Log( "server small: " + PrepareBundleHelper.GetServerSmallVersion() );
		
		Debug.Log( "local big: " + VersionTool.GetPackageBigVersion() );
		
		Debug.Log( "server big: " + PrepareBundleHelper.GetServerBigVersion() );
		#endif
		
		if( IsBigVersionUpdated() ){
			SetBundleState( BundleState.NotExist );
			
			// clean bundles
			{
				BundleHelper.CleanBundleConfigs();
			}
			
			return;
		}
		
		if( PrepareBundleHelper.GetLocalSmallVersion() != PrepareBundleHelper.GetServerSmallVersion() ){
			// small or big update
			SetBundleState( BundleState.ToUpdate );
		}
		else{
			// already updated
			SetBundleState( BundleState.Normal );
		}
	}
	
	#endregion



	#region Loading
	
	private static string m_cur_loading_title = "";
	
	private static string m_cur_loading_asset = "";
	
	private static bool m_loading_asset_changed = false;
	
	public static void SetLoadingTitle( string p_loading_title ){
		m_cur_loading_title = p_loading_title;
		
		SetLoadingAssetChanged( true );
	}
	
	public static string GetCurLoadingTitle(){
		return m_cur_loading_title;
	}
	
	public static void SetCurLoading( string p_cur_loading_name ){
		m_cur_loading_asset = p_cur_loading_name;
		
		SetLoadingAssetChanged( true );
	}
	
	public static void SetLoadingAssetChanged( bool p_changed ){
		m_loading_asset_changed = p_changed;
	}
	
	public static string GetCurLoading(){
		return m_cur_loading_asset;
	}
	
	private static int GetLoadingBundleConfigCount(){
		return Bundle_Loader.m_all_bundle_config_files.Length;
	}
		
	#endregion
	
	
	
	#region Utilities

	public static bool IsDeviceCheckOpen(){
		bool t_check_device = false;
		
		#if UNITY_ANDROID || UNITY_IOS
		t_check_device = true;
		#endif
		
		#if UNITY_EDITOR
		t_check_device = true;
		#endif

		return t_check_device;
	}

	public static bool IsBigVersionUpdated(){
		return VersionTool.GetPackageSmallVersion() == PrepareBundleHelper.GetServerSmallVersion();
	}
	
	public static bool ShowServerSelector(){
		#if SHOW_SERVER_SELECTOR
		return true;
		#else
		return false;
		#endif
	}
	
	#endregion



	#region Configs
	
	private static void SetDefaultServer(){
		NetworkHelper.ServerType t_server_type = SelectUrl.GetServerType();
		
		switch( t_server_type ){
		case NetworkHelper.ServerType.CeShi:
			PrepareBundles.Instance().m_pop_update_server.value = "测试服";
			break;
			
		case NetworkHelper.ServerType.NeiWang:
			PrepareBundles.Instance().m_pop_update_server.value = "内网服";
			break;
			
		case NetworkHelper.ServerType.TiYan:
			PrepareBundles.Instance().m_pop_update_server.value = "体验服";
			break;
		}
		
		Debug.Log ( "Default Server: " + t_server_type );
		
		OnPopupListChange ();
	}
	
	public static void SetTiYanServer(){
		NetworkHelper.SetUpdateUrl( NetworkHelper.UPDATE_URL_TIYAN );

		NetworkHelper.SetServerType( NetworkHelper.ServerType.TiYan );
		
		SelectUrl.SetUrlServeType( NetworkHelper.ServerType.TiYan );
	}
	
	public static void SetCeshiServer(){
		NetworkHelper.SetUpdateUrl( NetworkHelper.UPDATE_URL_CESHI );

		NetworkHelper.SetServerType( NetworkHelper.ServerType.CeShi );
		
		SelectUrl.SetUrlServeType( NetworkHelper.ServerType.CeShi );
	}
	
	public static void OnPopupListChange(){
		#if DEBUG_BUNDLE
		Debug.Log( "UpdateServerSelected( " + PrepareBundles.Instance().m_pop_update_server.value + " )" );
		#endif
		
		switch( PrepareBundles.Instance().m_pop_update_server.value ){
		case "测试服":
			NetworkHelper.SetUpdateUrl( NetworkHelper.UPDATE_URL_CESHI );
			
			NetworkHelper.SetServerType( NetworkHelper.ServerType.CeShi );
			
			SelectUrl.SetUrlServeType( NetworkHelper.ServerType.CeShi );
			break;
			
		case "体验服":
			SetTiYanServer();
			break;
			
		case "内网服":
			// default
			NetworkHelper.SetUpdateUrl( NetworkHelper.UPDATE_URL_NEIWANG );
			
			NetworkHelper.SetServerType( NetworkHelper.ServerType.NeiWang );
			
			SelectUrl.SetUrlServeType( NetworkHelper.ServerType.NeiWang );
			break;
		}
	}
	
	#endregion
	
	
	
	#region Const HTTP Keys
	
	public const string CONST_SERVER_SMALL_VERSION_TAG		= "SmallVersion";
	
	public const string CONST_SERVER_BIG_VERSION_TAG		= "BigVersion";
	
	public const string CONST_URL_PREFIX_TAG				= "url";
	
	public const string CONST_UPDATE_LIST_TAG				= "updateList";
	
	
	public const string CONST_ITEM_URL_PREFIX_TAG		= "url";
	
	public const string CONST_ITEM_KEY_TAG				= "key";
	
	public const string CONST_ITEM_VERSION_TAG			= "version";
	
	#endregion



	#region Const Loading Sections

	public const string CONST_LOADING_BUNDLE_CONFIG			= "LoadingBundleInfo";

	public const string CONST_UPDATE_BUNDLES				= "UpdateBundles";
	
	public const string CONST_LOADING_PRELOAD_RESOURCES		= "LoadingPreloadRes";
	
	#endregion
	
	
	
	#region Const Tip Strings
	
	public const string LOADING_TIPS_CONNECTING_TO_UPDATE_SERVER	= "连接更新服务器中，请稍后......";
	
	public const string LOADING_TIPS_EXTRACTING_ASSETS				= "提取系统资源中，请稍后......";
	
	public const string LOADING_TIPS_UPDAING_ASSESTS				= "更新资源中，请稍后......";
	
	
	
	public const string POPUP_TIPS_TITLE							= "版本更新";
	
	public const string POPUP_TIPS_BIG_VERSION_UPDATE				= "大版本更新，请下载新版本进入游戏。";
	
	public const string POPUP_TIPS_CONNECTING_TO_UPDATE_SERVER_FAIL	= "对不起，更新服务器连接失败！";
	
	
	
	public const string BUTTON_TXT_OK								= "确定";
	
	#endregion
}
