#define SHOW_SERVER_SELECTOR

#define SKIP_BUNDLE_UPDATE



//#define DEBUG_LOADING

//#define LOCAL_RESOURCES

//#define LOCAL_OFFLINE_BUNDLE



//#define SKIP_BASE_BUNDLES


using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.3
 * @since:		Unity 4.5.3
 * Function:	Init Bundle files.
 * 
 * Notes:
 * 1.Update Bundle Info.
 * 2.Update Bundle Items.
 * 3.Prepare Basic Bundles & Assets.
 * 4.When Release New Version, Please Update These:
 *       A.CONFIG_SMALL_VERSION_NEWEST for small version update;
 *       B.CONFIG_SMALL_VERSION_NEWEST & CONFIG_BIG_VESION & CONFIG_BASE_VERSION for big version update;
 *       C.Create a new Folder like "2015_0617_1641" under "StreamingArchived" Folder, and copy iOS/Android Assets to here;
 *       D.Pack "2015_0617_1641" Folder, and send it to update Server;
 * 5.CONFIG_SMALL_VERSION_NEWEST must be the same as folder name "2015_0617_1641";
 * 6.When Build Big Version, Please Delete All Foldes Under "StreamingArchived";
 */ 
public class Prepare_Bundle_Config : MonoBehaviour {

	private enum BundleState{
		NotExist,
		ToUpdate,
		Normal,
	}

	public Font m_font;

	public UISlider m_slider_progress;

	public UILabel m_lb_title;

	public UILabel m_lb_tips;

	public string[] m_to_load_bundles;

	private static BundleState m_bundle_state = BundleState.NotExist;



	public UIPopupList m_pop_update_server;

	public UIButton m_bt_update;


	private bool m_is_big_version_update = false;


	private static Prepare_Bundle_Config m_instance = null;

	#region Mono

	public static Prepare_Bundle_Config Instance(){
		if( m_instance == null ){
			Debug.LogError( "Error, Prepare_Bundle_Config.m_instance = null." );
		}

		return m_instance;
	}

	void Awake(){
		m_instance = this;

		if ( !ShowServerSelector ()) {
			if( m_pop_update_server != null ){
				m_pop_update_server.gameObject.SetActive( false );
			}

			if( m_bt_update != null ){
				m_bt_update.gameObject.SetActive( false );
			}

			if( m_lb_tips != null ){
				m_lb_tips.gameObject.SetActive( false );
			}
		}

		// set default http server prefix
		if( ShowServerSelector() ){
			SetCeshiServer();

//			SetDefaultServer();
		}
		else{
//			SetTiYanServer();

			SetCeshiServer();
		}

		// init and clean
		{
			VersionTool.Instance().Init();

			UtilityTool.LoadBox();

			Bundle_Loader.m_bundle_list_info = null;
			
			StaticLoading.ClearLoadingInfo( m_loading_sections );
		}

		// 1st & 2nd test in third platform
		{
			GameObject t_gb = UtilityTool.GetDontDestroyOnLoadGameObject();

			ThirdPlatform t_3rd = t_gb.GetComponent<ThirdPlatform>();

			if( t_3rd == null ){
				t_3rd = t_gb.AddComponent<ThirdPlatform>();
			}
		}

		{
			PathHelper.LogPath();
		}

		{
			DeviceHelper.LogDeviceInfo( null );
		}

		// log
		{
			FileHelper.DeleteLogFile();

			Application.logMessageReceived += FileHelper.LogFile;
		}

		// report when game launched
		{
			OperationSupport.ReportClientAction( OperationSupport.ClientAction.LAUNCH_GAME );
		}

		// TODO, Debug Use, remove after lzma.
//		Prepare_Bundle_Cleaner.CleanCache();
	}

	// Use this for initialization
	void Start () {
		// waiting for 3rd login or direct update bundle
		if( !ThirdPlatform.IsThirdPlatform() ){
			UpdateServerSelected( null );
		}

		// bt update
		{
			UIEventListener t_listener = m_bt_update.gameObject.GetComponent<UIEventListener>();

			if( t_listener != null ){
				t_listener.onClick = UpdateServerSelected;
			}
			else{
				Debug.LogError( "No UIEventListener Exist." );
			}
		}

		// popup list update server
		{
			m_pop_update_server.onChange.Add( new EventDelegate( OnPopupListChange ) );
		}

		#if UNITY_STANDALONE
			Debug.Log( "Direct Enter Login." );

			PreLoadBaseBundlesDone();
		#endif

//		ShowErrorBox( POPUP_TIPS_CONNECTING_TO_UPDATE_SERVER_FAIL, ConnectToUpdateServerFail );

	}

	// Update is called once per frame
	void Update () {
		if( m_loading_asset_changed ){
			SetLoadingAssetChanged( false );

			if( m_lb_title != null ){
				m_lb_title.text = GetCurLoadingTitle();
			}
			
			if( m_lb_tips != null ){
				m_lb_tips.text = GetCurLoading();
			}
		}

		{
			float t_percentage = StaticLoading.GetLoadingPercentage( m_loading_sections );
			
//			Debug.Log( "StaticLoading.Percentage: " + t_percentage );
			
			m_slider_progress.value = t_percentage;
		}
	}

//	void OnGUI(){
//		#if DEBUG_LOADING
//		if( m_is_big_version_update ){
//			int lb_w = Screen.width / 5;
//			int lb_h = Screen.height / 10;
//			
//			GUI.Label( new Rect( Screen.width / 2 - lb_w / 2, Screen.height / 2 - lb_h / 2, lb_w, lb_h ), 
//			          "Big Version Update,Please Try To Get Newest Client." );
//		}
//
//		if( !m_start_update ){
//			return;
//		}
//
//		int t_label_index = 0;
//		
//		GUI.Label( GetLabelRect( t_label_index++ ), 
//		          "Loading: " + GetCurLoading() );
//		
//		GUI.Label( GetLabelRect( t_label_index++ ), 
//		          "Cur Prepare Time: " + ( Time.realtimeSinceStartup - t_time_since_start_up ) );
//
//		#endif
//	}


	private float t_time_since_start_up = 0;

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Select Update Server

	private bool m_start_update = false;

	public void UpdateServerSelected( GameObject p_gb ){
//		#if DEBUG_LOADING
		Debug.Log( "UpdateServerSelected()" );
//		#endif

		SetBundleUpdateState( BundleUpdateState.CHECKING_UPDATE_INFO );

		#if UNITY_STANDALONE
		return;
		#endif

		{
			m_start_update = true;

			t_time_since_start_up = Time.realtimeSinceStartup;
			
			StartCoroutine( UpdateBundles() );
		}
	}

	private void SetDefaultServer(){
		ConfigTool.ServerType t_server_type = SelectUrl.GetServerType();

		switch( t_server_type ){
		case ConfigTool.ServerType.CeShi:
			m_pop_update_server.value = "测试服";
			break;

		case ConfigTool.ServerType.NeiWang:
			m_pop_update_server.value = "内网服";
			break;

		case ConfigTool.ServerType.TiYan:
			m_pop_update_server.value = "体验服";
			break;
		}

		Debug.Log ( "Default Server: " + t_server_type );

		OnPopupListChange ();
	}

	private void SetTiYanServer(){
		HttpRequest.SetUpdateUrl( HttpRequest.UPDATE_URL_TIYAN );
		
		ConfigTool.SetServerType( ConfigTool.ServerType.TiYan );
		
		SelectUrl.SetUrlServeType( ConfigTool.ServerType.TiYan );
	}

	private void SetCeshiServer(){
		HttpRequest.SetUpdateUrl( HttpRequest.UPDATE_URL_CESHI );
		
		ConfigTool.SetServerType( ConfigTool.ServerType.CeShi );
		
		SelectUrl.SetUrlServeType( ConfigTool.ServerType.CeShi );
	}

	public void OnPopupListChange(){
		#if DEBUG_LOADING
//		Debug.Log( "UpdateServerSelected( " + m_pop_update_server.value + " )" );
		#endif

		switch( m_pop_update_server.value ){
		case "测试服":
			HttpRequest.SetUpdateUrl( HttpRequest.UPDATE_URL_CESHI );

			ConfigTool.SetServerType( ConfigTool.ServerType.CeShi );

			SelectUrl.SetUrlServeType( ConfigTool.ServerType.CeShi );
			break;

		case "体验服":
			SetTiYanServer();
			break;

		case "内网服":
			// default
			HttpRequest.SetUpdateUrl( HttpRequest.UPDATE_URL_NEIWANG );

			ConfigTool.SetServerType( ConfigTool.ServerType.NeiWang );

			SelectUrl.SetUrlServeType( ConfigTool.ServerType.NeiWang );
			break;
		}


	}

	#endregion



	#region Update Bundles

	public static string m_config_cached_small_version = "";

	private string m_config_package_small_version = "";

	private IEnumerator UpdateBundles(){
		{
			#if SKIP_BUNDLE_UPDATE
			if( ThirdPlatform.IsThirdPlatform() ){
				for( int i = 0; i < 10; i++ ){
					Debug.Log( "Skipping Update Now, Please Confirm." );

					Debug.LogError( "Skipping Update Now, Please Confirm." );
				}

				Application.Quit();
			}
			
			PreLoadBaseBundlesDone();

			yield break;
			#endif
		}


		// check
		{
			{
				LoadCachedBundleVersions();
			}
			
			#if LOCAL_OFFLINE_BUNDLE || LOCAL_RESOURCES
			SetBundleState( BundleState.NotExist );

			{
				InitLoadingSections();
			}
			#else
			{
				UpdateServerBundleInfo();
			}
			
			// process
			{
				while( Bundle_Loader.m_bundle_list_info == null ){
//					Debug.Log( "Waiting For Response." );
					SetLoadingTitle( LOADING_TIPS_CONNECTING_TO_UPDATE_SERVER );

					yield return new WaitForEndOfFrame();
				}
				
				if( !Bundle_Loader.m_bundle_list_info.m_is_success ){
					Debug.LogError( "Error, Now ReConnect." );

					ShowErrorBox( POPUP_TIPS_CONNECTING_TO_UPDATE_SERVER_FAIL, ConnectToUpdateServerFail );

					yield break;
				}
			}
			
			// check
			{
				{
					UpdateBundleStatus();
				}

				{
					InitLoadingSections();
				}
				
				if( IsBundleNotExist() ){
					Debug.Log( "Clear Bundle & WWW-Path." );
					
					Prepare_Bundle_Cleaner.CleanCache();
					
					CleanBundleUpdateList();
				}

				if( IsToUpdateBundle() ){
					if( Bundle_Loader.m_bundle_list_info.m_big_version != VersionTool.GetBigVersion() ){
						Debug.LogError( "Big Version Update." );

						StaticLoading.ClearLoadingInfo( m_loading_sections );

						m_is_big_version_update = true;

						ShowErrorBox( POPUP_TIPS_BIG_VERSION_UPDATE, BigVersionTipsCallback );

						// TODO, Show Update Button, then goto app store.
						while( true ){
							yield return new WaitForEndOfFrame();
						}
					}
				}

				#if DEBUG_LOADING
				Debug.Log( "State: " + m_bundle_state );
				#endif
			}
			
			// Not Exist
			if( IsBundleNotExist() ){
				// write
				{
					System.IO.FileStream t_stream = FileHelper.GetPersistentFileStream( ConstInGame.CONST_PERSISTENT_DATA_BUNDLE_LIST );
					
					FileHelper.WriteString( t_stream, Bundle_Loader.m_bundle_list_info.m_json.ToString( "" ) );

					t_stream.SetLength( t_stream.Position );

					t_stream.Close();
				}
			}
			#endif
			
			#if LOCAL_OFFLINE_BUNDLE || LOCAL_RESOURCES
			
			#else
			SetBundleUpdateState( BundleUpdateState.UPDATEING_BUNDLES );

			// update
			if( IsToUpdateBundle() ){
				{
					SetLoadingTitle( LOADING_TIPS_UPDAING_ASSESTS );
				}

				{
					UpdateBundleItemsVersions();
				}
				
				// update bundles
				{
					{
						for( int i = 0; i < Bundle_Loader.m_bundle_list_info.m_item_list.Count; i++ ){
							Bundle_Loader.BundleListInfo.BundleItem t_item = Bundle_Loader.m_bundle_list_info.m_item_list[ i ];

							Debug.Log( "Updating Bundle: " + Bundle_Loader.m_bundle_list_info.m_item_list[ i ].GetUrl() + " - " +
							          int.Parse( t_item.m_version ) + " - " +
							          t_item.m_key + " )" );

							Bundle_Loader.Instance().LoadBundleFromUrl(
								Bundle_Loader.m_bundle_list_info.m_item_list[ i ].GetUrl(),
								int.Parse( t_item.m_version ),
								t_item.m_key,
								UpdateBundleCallback,
								null,
								null );
						}
					}
					
					Bundle_Loader.Instance().DownloadTheToLoad();
				}
				
				while( !Bundle_Loader.m_bundle_list_info.IsUpdateDone() ){
					//					Debug.Log( "Waiting For Update." );
					
					yield return new WaitForEndOfFrame();
				}
				
				// clean
				{
					Bundle_Loader.CleanData();
				}
			}
			#endif
			
			#if !LOCAL_RESOURCES
			// load www-path
			{
				ReloadBundleItemsVersions();
			}
			#endif
		}

		#if !LOCAL_RESOURCES
		// configs
		{
			{
				Bundle_Loader.Instance().LoadAllBundleConfigs();
			}

			// waiting for config to load
			while( Bundle_Loader.Instance().m_config_loaded_count < Bundle_Loader.m_all_bundle_config_files.Length ){
				#if DEBUG_LOADING
//				Debug.Log( "Waiting For Bundle Configs Loaded." );
				#endif
				
				yield return new WaitForEndOfFrame();
			}

			{
				SetBundleUpdateState( BundleUpdateState.EXTRACT_BUNDLES );

				CheckExtractBundles();

				while( m_bundle_config_to_extract_loaded < m_bundle_config_to_extract_count ){
					#if DEBUG_LOADING
					Debug.Log( "Waiting For Bundle Config To Be Loaded." );
					#endif
					
					yield return new WaitForEndOfFrame();
				}

				while( m_bundles_extracted_count < m_bundles_to_extract_count ){
					#if DEBUG_LOADING
					Debug.Log( "Waiting For Bundle To Be Extracted." );
					#endif

					yield return new WaitForEndOfFrame();
				}
			}

			{
				Global.m_is_loading_from_bundle = true;
			}
		}
		
		// prepare
		if( IsBundleRefreshed() ){
			float t_cur = Time.realtimeSinceStartup;

			// update state
			{
//				Debug.Log( "Save Version: " + Bundle_Loader.m_bundle_list_info.m_small_version );
				
				PlayerPrefs.SetString( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION, Bundle_Loader.m_bundle_list_info.m_small_version );
				
				PlayerPrefs.SetString( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_BIG_VRESION, VersionTool.GetBigVersion() );
				
				PlayerPrefs.Save();
			}
			
//			Debug.Log( " PrepareBundles.Time: " + ( Time.realtimeSinceStartup - t_cur ) );
		}
		
		// preload
		{
			PreLoadBaseBundles();
		}
		#else
		{
			PreLoadBaseBundlesDone();
			
			yield return null;
		}
		#endif
	}

	private void UpdateServerBundleInfo(){
		string t_param = "";
		
		if( m_config_cached_small_version == "" ){
			t_param = m_config_package_small_version;
		}
		else{
			t_param = m_config_cached_small_version;
		}
		
		{
			Dictionary< string,string > t_request_params = new Dictionary<string,string>();
			
			{
				t_request_params.Add( "platform", PlatformHelper.GetPlatformTag() );
				
				t_request_params.Add( "version", t_param );
				
				t_request_params.Add( "bigVersion", VersionTool.GetBigVersion() );
				
				t_request_params.Add( "baseVersion", t_param );
			}

			#if DEBUG_LOADING
			Debug.Log( "Get Update Info: " + HttpRequest.GetUpdateUrl() );
			#endif
			
			HttpRequest.Instance().Connect( HttpRequest.GetUpdateUrl(), 
			                               t_request_params, 
			                               UpdateSuccessCallback, 
			                               UpdateFailCallback ); 
		}
	}

	private void UpdateBundleStatus(){
		#if DEBUG_LOADING
		Debug.Log( "local: " + m_config_cached_small_version );
		
		Debug.Log( "package: " + m_config_package_small_version );
		
		Debug.Log( "server: " + Bundle_Loader.m_bundle_list_info.m_small_version );
		
		Debug.Log( "local.big: " + VersionTool.GetBigVersion() );
		
		Debug.Log( "server.big: " + Bundle_Loader.m_bundle_list_info.m_big_version );
		#endif
		
		if( m_config_cached_small_version == "" ){
			if( m_config_package_small_version != Bundle_Loader.m_bundle_list_info.m_small_version ){
				SetBundleState( BundleState.ToUpdate );
			}
			else{
				SetBundleState( BundleState.NotExist );
			}
		}
		else{
			if( m_config_cached_small_version != Bundle_Loader.m_bundle_list_info.m_small_version ){
				if( m_config_package_small_version != Bundle_Loader.m_bundle_list_info.m_small_version ){
					SetBundleState( BundleState.ToUpdate );
				}
				else{
					SetBundleState( BundleState.NotExist );
				}
			}
			else{
				SetBundleState( BundleState.Normal );
			}
		}
	}

	// update local bundle config
	private void UpdateBundleItemsVersions(){
		// delta
		{
			Bundle_Loader.BundleListInfo t_local_info = new Bundle_Loader.BundleListInfo();
			
			System.IO.FileStream t_stream = FileHelper.GetPersistentFileStream( ConstInGame.CONST_PERSISTENT_DATA_BUNDLE_LIST );
			
			string t_content = FileHelper.ReadString( t_stream );
			
			Debug.Log( "Local Bundle List: " + t_content );
			
			{
				t_stream.Position = 0;

				ParseBundleListInfo( t_local_info, t_content );
			}
			
			if( string.IsNullOrEmpty( t_content ) ){
				FileHelper.WriteString( t_stream, Bundle_Loader.m_bundle_list_info.m_json.ToString( "" ) );
			}
			else{
				t_local_info.m_json[ CONST_URL_PREFIX_TAG ] = Bundle_Loader.m_bundle_list_info.m_url_prefix;
				
				t_local_info.m_json[ CONST_SERVER_VERSION_TAG ] = Bundle_Loader.m_bundle_list_info.m_small_version;
				
				// items
				for( int i = 0; i < Bundle_Loader.m_bundle_list_info.m_item_list.Count; i++ ){
					bool t_found = false;
					
					for( int j = 0; j < t_local_info.m_json[ CONST_UPDATE_LIST_TAG ].Count; j++ ){
						string t_local_key = t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ j ][ CONST_ITEM_KEY_TAG ];
						
						if( Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_key == t_local_key ){
							t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ j ][ CONST_ITEM_URL_PREFIX_TAG ] = Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_url_prefix;
							
							t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ j ][ CONST_ITEM_VERSION_TAG ] = Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_version;
							
							t_found = true;
							
							break;
						}
					}
					
					if( !t_found ){
						int t_index = t_local_info.m_json[ CONST_UPDATE_LIST_TAG ].Count;
						
						t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ t_index ][ CONST_ITEM_URL_PREFIX_TAG ] = Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_url_prefix;
						
						t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ t_index ][ CONST_ITEM_KEY_TAG ] = Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_key;
						
						t_local_info.m_json[ CONST_UPDATE_LIST_TAG ][ t_index ][ CONST_ITEM_VERSION_TAG ] = Bundle_Loader.m_bundle_list_info.m_item_list[ i ].m_version;
					}
				}
				
				FileHelper.WriteString( t_stream, t_local_info.m_json.ToString( "" ) );
				
				Debug.Log( "Updated Bundle List: " + t_local_info.m_json.ToString( "" ) );
			}

			t_stream.SetLength( t_stream.Position );

			t_stream.Close();
		}
	}

	// reload newest bundle config
	private void ReloadBundleItemsVersions(){
		Bundle_Loader.m_bundle_list_info = new Bundle_Loader.BundleListInfo();
		
		System.IO.FileStream t_stream = FileHelper.GetPersistentFileStream( ConstInGame.CONST_PERSISTENT_DATA_BUNDLE_LIST );
		
		{
			string t_content = FileHelper.ReadString( t_stream );
			
			ParseBundleListInfo( Bundle_Loader.m_bundle_list_info, t_content );
			
//			Debug.Log( "Reload From Local Bundle List: " + t_content );
		}
		
		t_stream.Close();
	}

	public void UpdateBundleCallback( ref WWW p_www, string p_path, Object p_object ){
		{
			Bundle_Loader.m_bundle_list_info.m_downloaded_count++;
			
			StaticLoading.ItemLoaded( m_loading_sections, CONST_UPDATE_BUNDLES );

			// TODO
//			SetCurLoading( p_path );
		}

		Debug.Log( "UpdateBundleCallback( " + 
		          Bundle_Loader.m_bundle_list_info.m_downloaded_count + " / " + 
		          Bundle_Loader.m_bundle_list_info.m_item_list.Count + " - " +
		          p_path + " )" );
	}

	public static void CleanBundleUpdateList(){
//		Debug.Log( "CleanBundleUpdateList()" );

		FileHelper.DeletePersistentFileStream( ConstInGame.CONST_PERSISTENT_DATA_BUNDLE_LIST );
	}
	
	/// Server Response:
	/// "platform": "Android",
	/// "version":"2015_0127_1502", 
	/// "BigVersion":"0.93",
	/// "url":"http://192.168.0.176:8080/wsRes/rep/201501281419/Android"
	/// "updateList":[
	/// 		{"key":"Resources/_UIs/Login","version":"1"},
	/// 		{"key":"Resources/_UIs/City","version":"2"},
	/// 		{"key":"Resources/_UIs/City","version":"3"}]
	///}
	public void UpdateSuccessCallback( string p_response ){
		{
			Bundle_Loader.m_bundle_list_info = new Bundle_Loader.BundleListInfo();

			Bundle_Loader.m_bundle_list_info.m_is_success = true;
		}

		#if DEBUG_LOADING
		Debug.Log( "UpdateSuccessCallback: " + p_response );
		#endif

		ParseBundleListInfo( Bundle_Loader.m_bundle_list_info, p_response );
	}
	
	public void UpdateFailCallback( string p_response ){
		{
			Bundle_Loader.m_bundle_list_info = new Bundle_Loader.BundleListInfo();
		}

		Debug.LogError( "UpdateFailCallback: " + p_response );

		{
			UtilityTool.SetCommonCodeError( "Update", p_response );
		}
	}

	public void ParseBundleListInfo( Bundle_Loader.BundleListInfo p_list_info, string p_response ){
		if( string.IsNullOrEmpty( p_response ) ){
			return;
		}

		p_list_info.m_json = JSON.Parse( p_response );
		
		p_list_info.m_small_version = p_list_info.m_json[ CONST_SERVER_VERSION_TAG ].Value;

		p_list_info.m_big_version = p_list_info.m_json[ CONST_BIG_VERSION_TAG ].Value;

		p_list_info.m_url_prefix = p_list_info.m_json[ CONST_URL_PREFIX_TAG ].Value;

		#if DEBUG_LOADING
		Debug.Log( "Url.Prefix: " + p_list_info.m_url_prefix );

		Debug.Log( "Version: " + p_list_info.m_small_version );
		#endif
		
		for( int i = 0; i < p_list_info.m_json[ CONST_UPDATE_LIST_TAG ].Count; i++ ){
			Bundle_Loader.BundleListInfo.BundleItem t_item = new Bundle_Loader.BundleListInfo.BundleItem();

			{
				string t_url_prefix = p_list_info.m_json[ CONST_UPDATE_LIST_TAG ][ i ][ CONST_ITEM_URL_PREFIX_TAG ];
				
				if( string.IsNullOrEmpty( t_url_prefix ) ){
					t_item.m_url_prefix = p_list_info.m_url_prefix;
				}
				else{
					t_item.m_url_prefix = t_url_prefix;
				}
			}

			t_item.m_key = p_list_info.m_json[ CONST_UPDATE_LIST_TAG ][ i ][ CONST_ITEM_KEY_TAG ];
			
			t_item.m_version = p_list_info.m_json[ CONST_UPDATE_LIST_TAG ][ i ][ CONST_ITEM_VERSION_TAG ];
			
			p_list_info.m_item_list.Add( t_item );
		}
	}

	#endregion



	#region PopUp Window

	private static void ShowErrorBox( string p_error_string, UIBox.onclick p_click ){
		Global.CreateBox( POPUP_TIPS_TITLE,
		                 p_error_string,
		                 "",
		                 null,
		                 BUTTON_TXT_OK, 
		                 null, 
		                 p_click,
		                 null,
		                 null,
		                 null,
		                 false,
		                 false,
		                 false );
	}

	public void BigVersionTipsCallback( int p_int ){
		Debug.Log( "BigVersionTipsCallback()" );

		UtilityTool.QuitGame();
	}

	public void ConnectToUpdateServerFail( int p_int ){
		Debug.Log( "ConnectToUpdateServerFail()" );

		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_BUNDLE );

		UtilityTool.ClearCommonCodeError();
	}

	#endregion



	#region Extract Common Bundles

	private int m_bundle_config_to_extract_loaded = 0;

	private int m_bundle_config_to_extract_count = 0;

	private int m_bundles_extracted_count = 0;

	private int m_bundles_to_extract_count = 0;

	private List<string> m_bundles_to_extract_list = new List<string>();

	private float m_extract_start_time = 0;

	private void CheckExtractBundles(){
		#if DEBUG_LOADING
		Debug.Log( "CheckExtractBundles()" );
		#endif

		{
			m_bundle_config_to_extract_loaded = 0;
			
			m_bundle_config_to_extract_count = 0;
			
			m_bundles_to_extract_list.Clear();
		}

		{
			m_extract_start_time = Time.realtimeSinceStartup;
		}

		{
			m_bundles_extracted_count = 0;
			
			m_bundles_to_extract_count = 0;
		}

		bool t_exec_extract = false;

		{
			int t_value = PlayerPrefs.GetInt( ConstInGame.CONST_EXTRACT_BUNBLES_KEY, 1 );

			#if DEBUG_LOADING
			Debug.Log( "PlayerPrefs.CONST_EXTRACT_BUNBLES_KEY: " + t_value );
			#endif

			t_exec_extract = ( t_value == 1 ? true : false );
		}
		
		if( t_exec_extract ){
			{
				SetLoadingTitle( LOADING_TIPS_EXTRACTING_ASSETS );
			}

			#if DEBUG_LOADING
			Debug.Log( "CheckExtractBundles.Loading.Common.Bundle.Configs()" );
			#endif

			m_bundle_config_to_extract_count = m_extract_bundle_config_files.Length;

			for( int i = 0; i < m_extract_bundle_config_files.Length; i++ ){
				Bundle_Loader.LoadAssetFromBundle(
					Bundle_Loader.GetConfigBundleKey(),
					LoadCommonBundleConfigDone,
					m_extract_bundle_config_files[ i ] );
			}
		}
		else{
			#if DEBUG_LOADING
			Debug.Log( "CheckExtractBundles.Skip.Loading.Common.Bundle.Configs()" );
			#endif
		}
	}

	public void LoadCommonBundleConfigDone( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		#if DEBUG_LOADING
		Debug.Log( "LoadCommonBundleConfigDone( " + p_path + " )" );
		#endif

		if( p_object == null ){
			Debug.LogError( "Res Not Found.: " + p_path );
			
			return;
		}
		
		TextAsset t_text = ( TextAsset )p_object;
		
		string t_json_string = t_text.text;
		
		// json node
		{
			JSONNode t_bundle_node = JSONNode.Parse( t_json_string );
			
			for( int i = 0; i < t_bundle_node.Count; i++ ){
				string t_key = ( (JSONClass)t_bundle_node ).GetKey( i );

				m_bundles_to_extract_list.Add( t_key );
			}
		}
		
		{
			m_bundle_config_to_extract_loaded++;
			
			if( m_bundle_config_to_extract_loaded == m_bundle_config_to_extract_count ){
				ExecExtractBundles();
			}
		}
	}

	private void ExecExtractBundles(){
		#if DEBUG_LOADING
		Debug.Log( "ExecExtractBundles()" );

		float t_time = Time.realtimeSinceStartup;
		#endif

		{
			m_bundles_to_extract_count = m_bundles_to_extract_list.Count;

			StaticLoading.InitSectionInfo( m_loading_sections, CONST_EXTRACT_ALL_BUNDLES, 100, m_bundles_to_extract_count );
		}
		
		for( int i = 0; i < m_bundles_to_extract_list.Count; i++ ){
			Bundle_Loader.LoadAssetFromBundle( m_bundles_to_extract_list[ i ],
			                                  BundleExtractedCallback,
			                                  null,
			                                  null,
			                                  null,
			                                  false );
		}
		
		#if DEBUG_LOADING
		Debug.Log( "Time Cost When Add All Bundles To Extract List: " + ( Time.realtimeSinceStartup - t_time ) );
		#endif
	}

	public void BundleExtractedCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		{
			m_bundles_extracted_count++;
			
			StaticLoading.ItemLoaded( m_loading_sections, CONST_EXTRACT_ALL_BUNDLES );

			// TODO
//			SetCurLoading( m_bundles_extracted_count + ": " + p_path );
		}

		#if DEBUG_LOADING
		Debug.Log( "BundleExtractedCallback( " + m_bundles_extracted_count + " - " + " - " +
		          ( Time.realtimeSinceStartup - m_extract_start_time ) + " - " +
		          p_path + " )" );
		#endif

		{
			if( m_bundles_extracted_count == m_bundles_to_extract_count ){
				#if DEBUG_LOADING
				Debug.Log( "All Bundles Extracted." );
				#endif

				PlayerPrefs.SetInt( ConstInGame.CONST_EXTRACT_BUNBLES_KEY, 0 );
				
				PlayerPrefs.Save();
			}
		}
	}

	private int GetAllBundlesCount(){
		return Bundle_Loader.GetBundleInfoDict().Count;
	}

	#endregion

	
	
	#region Update Bundle Utilities

	private void SetBundleState( BundleState p_state ){
		m_bundle_state = p_state;

		switch( p_state ){
		case BundleState.Normal:
//			SetLoadingTitle( "Entering Login.." );
			break;

		case BundleState.NotExist:
//			SetLoadingTitle( "Extracting Assets.." );
			break;

		case BundleState.ToUpdate:
//			SetLoadingTitle( "Update Assets.." );
			break;
		}
	}
	
	private bool IsBundleNotExist(){
		return m_bundle_state == BundleState.NotExist;
	}
	
	private bool IsToUpdateBundle(){
		return m_bundle_state == BundleState.ToUpdate;
	}
	
	private bool IsBundleRefreshed(){
		return ( m_bundle_state == BundleState.NotExist || 
		        m_bundle_state == BundleState.ToUpdate );
	}

	// load local bundles
	private void LoadCachedBundleVersions(){
		m_config_cached_small_version = PlayerPrefs.GetString( 
		                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION, 
		                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT );
		
		{
			string t_cached_big_version = PlayerPrefs.GetString( 
			                                                    ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_BIG_VRESION, 
			                                                    ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT );
			
			string t_cur_big_version = VersionTool.GetBigVersion();
			
			if( t_cur_big_version != t_cached_big_version ){
				// clear
				Prepare_Bundle_Cleaner.CleanBundleConfigs();
				
				m_config_cached_small_version = PlayerPrefs.GetString( 
				                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION,
				                                                      ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT );
			}
		}

		m_config_package_small_version = VersionTool.GetSmallVersion();
	}

	#endregion



	#region PreLoad Base Bundles

	private int m_preloaded_base_bundles_count = 0;

	/// Exec Preload.
	private void PreLoadBaseBundles(){
		SetBundleUpdateState( BundleUpdateState.PRELOAD_BASE_BUNDLES );

		#if DEBUG_LOADING
		Debug.Log( "PreLoadBaseBundles()" );
		#endif

#if SKIP_BASE_BUNDLES
		{
			PreLoadBaseBundlesDone();
			
			return;
		}
#endif

		if( m_to_load_bundles.Length <= 0 ){
			PreLoadBaseBundlesDone();

			return;
		}

		// prepare
		{
			StartCoroutine( CheckingBaseBundles() );
		}

		for( int i = 0; i < m_to_load_bundles.Length; i++ ){
			if( string.IsNullOrEmpty( m_to_load_bundles[ i ] ) ){
				Debug.LogError( "Error, " + i + " slot is NullOrEmpty." );
				
				continue;
			}
			
			string t_bundle_path = m_to_load_bundles[ i ];
			
			Bundle_Loader.LoadAssetFromBundle( t_bundle_path,
			                                  PreLoadBaseBundlesCallback,
			                                  null );
		}
	}

	public void PreLoadBaseBundlesCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		m_preloaded_base_bundles_count++;
	}

	IEnumerator CheckingBaseBundles(){
		{
			m_preloaded_base_bundles_count = 0;
		}

		while( m_preloaded_base_bundles_count < m_to_load_bundles.Length ){
			yield return new WaitForEndOfFrame();
		}

		PreLoadBaseBundlesDone();
	}

	private void PreLoadBaseBundlesDone(){
		PreLoadResources();
	}

	#endregion



	#region PreLoad Resources

	private int m_prepare_data_for_loading 				= 0;
	
	private const int PREPARE_DATA_COUNT_FOR_LOADING	= 5;

	public void PreLoadResources(){
		SetBundleUpdateState( BundleUpdateState.PRELOAD_RESOURCES );

		#if DEBUG_LOADING
		Debug.Log( Time.realtimeSinceStartup + "PreLoadResources()" );
		#endif

		// reset count
		{
			m_prepare_data_for_loading = 0;
			
			StartCoroutine( CheckingResources() );
		}

		// load data
		{
			ConfigTool.Instance.LoadConfigs( LoadingDataDone );

			Res2DTemplate.LoadTemplates( LoadingDataDone );

			LanguageTemplate.LoadTemplates( LoadingDataDone );

			MyColorData.LoadTemplates( LoadingDataDone );

			QualityTool.Instance.LoadQualities( LoadingDataDone );
		}
	}

	public void LoadingDataDone(){
		{
			m_prepare_data_for_loading++;
			
			StaticLoading.ItemLoaded( m_loading_sections, CONST_LOADING_PRELOAD_RESOURCES );

			// TODO
//			SetCurLoading( "Template: " + m_prepare_data_for_loading );
		}

//		Debug.Log( "LoadingDataDone( " + m_prepare_data_for_loading + " )" );
	}

	IEnumerator CheckingResources(){
		{
			m_prepare_data_for_loading = 0;
		}

		while( m_prepare_data_for_loading < PREPARE_DATA_COUNT_FOR_LOADING ){
			#if DEBUG_LOADING
			Debug.Log( Time.realtimeSinceStartup + "CheckingResources( " + m_prepare_data_for_loading + " / " + PREPARE_DATA_COUNT_FOR_LOADING + " )" );
			#endif

			yield return new WaitForEndOfFrame();
		}

		// load utilities
//		{
//			NetworkWaiting.Instance( true );
//		}

		{
			Bundle_Loader.Instance().SetShowTime( false );
		}

		{
			BundleUpdateDone ();
		}
	}

	#endregion



	#region Bundle Update Done

	public static void BundleUpdateDone(){
		Debug.Log ( "BundleUpdateDone()" );

		SetBundleUpdateState( BundleUpdateState.PREPARE_START_GAME );

		{
			bool t_check_device = false;
			
			#if UNITY_ANDROID || UNITY_IOS
			t_check_device = true;
			#endif
			
			#if UNITY_EDITOR
			t_check_device = true;
			#endif
			
			if( t_check_device ){
				if( !DeviceHelper.CheckIsDeviceSupported() ){
					return;
				}
			}
		}

		if ( !ThirdPlatform.IsThirdPlatform () ) {
			StartGame ();
		}
		else {
			Debug.Log( "PrepareBundleConfig( wait for third to start game )" );
			
			ThirdPlatform.Instance().UploadToken();
		}
	}

	public static void StartGame(){
		Debug.Log( "PrepareBundleConfig.StartGame()" );

		SceneManager.EnterLogin();
	}

	#endregion



	#region Bundle Checker

	private static string m_level_name = "";

	public static void LoadLoadinglDone( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		Debug.Log( "LoadLoadinglDone( " + p_path + " )" );

		Debug.LogError( "Never Use This." );

		Application.LoadLevel( m_level_name );
	}

	#endregion



	#region Bundle Update State

	public enum BundleUpdateState{
		SELECT_UPDATE_SERVER = 0,
		CHECKING_UPDATE_INFO,
		UPDATEING_BUNDLES,
		EXTRACT_BUNDLES,
		PRELOAD_BASE_BUNDLES,
		PRELOAD_RESOURCES,
		PREPARE_START_GAME,
	}
	
	private static BundleUpdateState m_bundle_update_state = BundleUpdateState.SELECT_UPDATE_SERVER;

	private static void SetBundleUpdateState( BundleUpdateState p_state ){
		m_bundle_update_state = p_state;
	}

	public static BundleUpdateState GetBundleUpdateState(){
		return m_bundle_update_state;
	}

	#endregion


	
	#region GUI Utility
	
	private bool m_show_time = true;
	
	public void SetShowTime( bool p_will_show ){
		m_show_time = p_will_show;
	}
	
	private static Rect GetButtonRect( int p_index ){
		return new Rect( 100, p_index * 30, 300, 35 );	
	}
	
	private static Rect GetLabelRect( int p_index ){
		return new Rect( 50, Screen.height - 20 - p_index * 40, 650, 30 );
	}
	
	#endregion



	#region Loading Utilities

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

	public static List<StaticLoading.LoadingSection> m_loading_sections = new List<StaticLoading.LoadingSection>();

	private static void InitLoadingSections(){
		switch( m_bundle_state ){
		case BundleState.ToUpdate:
			StaticLoading.InitSectionInfo( m_loading_sections, CONST_UPDATE_BUNDLES, 1, GetUpdateBundleCount() );
			break;
		}

		{
			StaticLoading.InitSectionInfo( m_loading_sections, CONST_LOADING_BUNDLE_CONFIG, 1, GetLoadingBundleConfigCount() );
			
			StaticLoading.InitSectionInfo( m_loading_sections, CONST_LOADING_PRELOAD_RESOURCES, 1, PREPARE_DATA_COUNT_FOR_LOADING );
		}
	}

	private static int GetLoadingBundleConfigCount(){
		return Bundle_Loader.m_all_bundle_config_files.Length;
	}

	private static int GetUpdateBundleCount(){
		return Bundle_Loader.m_bundle_list_info.m_item_list.Count;
	}

	#endregion



	#region Utilities

	public static bool ShowServerSelector(){
		#if SHOW_SERVER_SELECTOR
		return true;
		#else
		return false;
		#endif
	}

	#endregion



	#region Loading Sections

	public const string CONST_EXTRACT_ALL_BUNDLES		= "ExtractAllBundles";



	public const string CONST_UPDATE_BUNDLES			= "UpdateBundles";

	public const string CONST_LOADING_BUNDLE_CONFIG		= "LoadingBundleInfo";

	public const string CONST_LOADING_PRELOAD_RESOURCES	= "LoadingPreloadRes";

	#endregion



	#region Load Common Bundle

	/// Pre Extract Bundle Configs.
	public static string[] m_extract_bundle_config_files = {
//		Bundle_Loader.Ui_Common_Config_Path_Name,
//		Bundle_Loader.Ui_Images_Config_Path_Name,
		Bundle_Loader.Data_Config_Path_Name,
//		Bundle_Loader.Sound_Config_Path_Name,
//		Bundle_Loader.D3D_Config_Path_Name,
		
//		Bundle_Loader.Fx_Config_Path_Name,
//		Bundle_Loader.New_Config_Path_Name,
//		Bundle_Loader.Scenes_Config_Path_Name
	};

	#endregion



	#region Const Values

	private const string CONST_SERVER_VERSION_TAG		= "version";
	
	private const string CONST_BIG_VERSION_TAG			= "BigVersion";
	
	private const string CONST_URL_PREFIX_TAG			= "url";
	
	private const string CONST_UPDATE_LIST_TAG			= "updateList";
	
	
	private const string CONST_ITEM_URL_PREFIX_TAG		= "url";
	
	private const string CONST_ITEM_KEY_TAG				= "key";
	
	private const string CONST_ITEM_VERSION_TAG			= "version";

	#endregion



	#region Const String

	public const string LOADING_TIPS_CONNECTING_TO_UPDATE_SERVER	= "连接更新服务器中，请稍后......";

	public const string LOADING_TIPS_EXTRACTING_ASSETS				= "提取系统资源中，请稍后......";

	public const string LOADING_TIPS_UPDAING_ASSESTS				= "更新资源中，请稍后......";



	public const string POPUP_TIPS_TITLE							= "版本更新";

	public const string POPUP_TIPS_BIG_VERSION_UPDATE				= "大版本更新，请下载新版本进入游戏。";

	public const string POPUP_TIPS_CONNECTING_TO_UPDATE_SERVER_FAIL	= "对不起，更新服务器连接失败！";



	public const string BUTTON_TXT_OK								= "确定";

	#endregion

}