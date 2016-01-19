//#define SKIP_BUNDLE_UPDATE

//#define LOCAL_BUNDLE_UPDATE

//#define DEBUG_BUNDLE



using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.12.4
 * @since:		Unity 5.1.3
 * Function:	Update Bundle files.
 * 
 * Notes:
 * 1.None.
 */ 
public class PrepareBundles : MonoBehaviour {

	public Font m_font;

	public UISlider m_slider_progress;

	public UILabel m_lb_title;

	public UILabel m_lb_tips;

	public UIPopupList m_pop_update_server;

	public UIButton m_bt_update;


	#region Instance

	private static PrepareBundles m_instance = null;

	public static PrepareBundles Instance(){
		if( m_instance == null ){
			Debug.LogError( "Error, Prepare_Bundle_Config.m_instance = null." );
		}

		return m_instance;
	}

	#endregion



	#region Mono
	
	void Awake(){
//		Debug.Log( "PrepareBundles.Awake()" );
		
		m_instance = this;
		
		PrepareBundleHelper.PrepareBundles_Init_In_Awake();

//		Debug.Log( "PrepareBundles.Awake.Done()" );
	}

	// Use this for initialization
	void Start () {
//		Debug.Log( "PrepareBundles.Start()" );

		if( ThirdPlatform.IsThirdPlatform() ){
			// waiting for 3rd login or direct update bundle
			SetUpdateState( UpdateState.SELECT_UPDATE_SERVER );
		}
		else{
			if( !PrepareBundleHelper.ShowServerSelector() ){
				// direct upate
				UpdateServerSelected( null );
			}
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
			m_pop_update_server.onChange.Add( new EventDelegate( PrepareBundleHelper.OnPopupListChange ) );
		}

		#if UNITY_STANDALONE
			Debug.Log( "Direct Enter Login." );

			BundleHelper.Instance().PreLoadResources();
		#endif

//		Debug.Log( "PrepareBundles.Start.Done()" );
	}

	// Update is called once per frame
	void Update () {
		PrepareBundleHelper.UpdateUI();
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Update Bundles

	public void UpdateServerSelected( GameObject p_gb ){
//		#if DEBUG_BUNDLE
		Debug.Log( "UpdateServerSelected()" );
//		#endif
		
		SetUpdateState( UpdateState.CHECKING_UPDATE_INFO );
		
		#if UNITY_STANDALONE
		return;
		#endif
		
		{
			StartUpdate();
		}
	}

	private void StartUpdate(){
		{
			#if SKIP_BUNDLE_UPDATE
			if( ThirdPlatform.IsThirdPlatform() ){
				for( int i = 0; i < 5; i++ ){
					Debug.Log( "Skipping Update Now, Please Confirm." );

					Debug.LogError( "Skipping Update Now, Please Confirm." );
				}

				Application.Quit();
			}

			BundleHelper.Instance().PreLoadResources();

			return;
			#endif
		}

		{
			PrepareBundleHelper.LoadCachedBundleVersions();

			PrepareBundleHelper.SetLoadingTitle( PrepareBundleHelper.LOADING_TIPS_CONNECTING_TO_UPDATE_SERVER );

			#if LOCAL_BUNDLE_UPDATE
			GetLocalInfo();
			#else
			GetServerInfo();
			#endif
		}
	}

	#endregion



	#region Local Update

	private void GetLocalInfo(){
		string t_file_path = PathHelper.GetFullPath_WithRelativePath( "Assets/StreamingArchived/" + 
		                                                             PlatformHelper.GetPlatformTag() + "/" +
		                                                             ManifestHelper.COSNT_UPDATE_FILE_NAME );

		string t_content = FileHelper.ReadString( t_file_path );

		if( string.IsNullOrEmpty( t_content ) ){
			UpdateFailCallback( "Fail in Load Local." );
		}
		else{
			JSONNode t_json = JSON.Parse( t_content );
			
			t_json[ PrepareBundleHelper.CONST_URL_PREFIX_TAG ] = PathHelper.GetLocalFileWWWPath_U5_Test_Use( "" );

			UpdateSuccessCallback( t_json.ToString( "" ) );
		}
	}

	#endregion



	#region HTTP

	private void GetServerInfo(){
		Dictionary< string,string > t_request_params = new Dictionary<string,string>();
		
		{
			t_request_params.Add( "platform", PlatformHelper.GetPlatformTag() );
			
			t_request_params.Add( "version", PrepareBundleHelper.GetLocalSmallVersion() );
			
			t_request_params.Add( "bigVersion", VersionTool.GetPackageBigVersion() );
		}
		
		#if DEBUG_BUNDLE
		Debug.Log( "Get Update Info: " + NetworkHelper.GetUpdateUrl() );
		#endif
		
		HttpRequest.Instance().Connect( NetworkHelper.GetUpdateUrl(), 
		                               t_request_params, 
		                               UpdateSuccessCallback, 
		                               UpdateFailCallback ); 
	}

	public void UpdateFailCallback( string p_response ){
		DebugHelper.SetCommonCodeError( "Update", p_response );
		
		Debug.LogError( "UpdateFailCallback, Error, Now ReConnect: " + p_response );
		
		ShowErrorBox( PrepareBundleHelper.POPUP_TIPS_CONNECTING_TO_UPDATE_SERVER_FAIL, ConnectToUpdateServerFail );
	}

	/// Server Response:
	/// {
	/// 	"SmallVersion":"2015_1016_1525", 
	/// 	"BigVersion":"2015_1014_1935",
	/// 	"url":"http://192.168.0.176:8080/wsRes/rep/201501281419/Android"
	/// }
	public void UpdateSuccessCallback( string p_response ){
		#if DEBUG_BUNDLE
		Debug.Log( "UpdateSuccessCallback: " + p_response );
		#endif

		if( string.IsNullOrEmpty( p_response ) ){
			Debug.LogError( "Error, response null or empty: " + p_response );

			return;
		}

		/// parse
		{
			JSONNode t_json = JSON.Parse( p_response );

			if( t_json == null ){
				Debug.LogError( "JSON Parse Error: " + p_response );

				return;
			}
			
			PrepareBundleHelper.SetServerSmallVersion( t_json[ PrepareBundleHelper.CONST_SERVER_SMALL_VERSION_TAG ].Value );
			
			PrepareBundleHelper.SetServerBigVersion( t_json[ PrepareBundleHelper.CONST_SERVER_BIG_VERSION_TAG ].Value );
			
			PrepareBundleHelper.SetServerBundlePrefix( t_json[ PrepareBundleHelper.CONST_URL_PREFIX_TAG ].Value );
		}

		{
			PrepareBundleHelper.UpdateBundleState();
			
			#if DEBUG_BUNDLE
			Debug.Log( "State: " + PrepareBundleHelper.GetBundleState() );
			#endif
		}

		if( VersionTool.GetPackageBigVersion() != PrepareBundleHelper.GetServerBigVersion() ){
			BigUpdate();
		}
		else{
			SmallUpdateCheck();
		}
	}

	#endregion



	#region Version Update

	private void BigUpdate(){
		// big update
		Debug.LogError( "Big Version Update." );
		
		LoadingHelper.ClearLoadingInfo( PrepareBundleHelper.GetLoadingSections() );
		
		// Show Update Button, then goto App Store.
		ShowErrorBox( PrepareBundleHelper.POPUP_TIPS_BIG_VERSION_UPDATE, BigVersionTipsCallback );
	}
	
	private void SmallUpdateCheck(){
		{
			PrepareBundleHelper.InitLoadingSections();

			SetUpdateState( UpdateState.UPDATEING_BUNDLES );
		}

		{
			PrepareBundleHelper.SetLoadingTitle( PrepareBundleHelper.LOADING_TIPS_UPDAING_ASSESTS );
		}
		
		{
			int t_root_version = PrepareBundleHelper.GetCachedRootBundleVersion();

			if( PrepareBundleHelper.IsToUpdateBundle() ){
				#if DEBUG_BUNDLE
				Debug.Log( "SmallUpdate." );
				#endif

				t_root_version = PrepareBundleHelper.GetNewRootBundleVersion();
			}

			// update bundles
			{
				string t_root_bundle_url = BundleHelper.GetRootBundleUrl();

				string t_file_url = BundleHelper.GetFileUrl();

				#if LOCAL_BUNDLE_UPDATE
				t_root_bundle_url = PathHelper.GetLocalFileWWWPath_U5_Test_Use( PlatformHelper.GetPlatformTag() );

				t_file_url = PathHelper.GetLocalFileWWWPath_U5_Test_Use( ManifestHelper.CONST_MANIFEST_FOLDER_NAME + "/" + ManifestHelper.CONST_FILE_NAME );
				#endif

				{
					if( !PrepareBundleHelper.IsBigVersionUpdated() ){
						BundleHelper.LoadAsset( t_root_bundle_url, t_root_version,
						                       "", ManifestHelper.CONST_ROOT_BUNDLE_ASSET_NAME,
						                       RootBundleCallback );
					}
					
					BundleHelper.LoadAsset( t_file_url, t_root_version,
					                       "", ManifestHelper.CONST_FILE_NAME,
					                       FileBundleCallback );
				}
			}
		}
	}

	public void RootBundleCallback( ref WWW p_www, string p_path, Object p_object ){
		#if DEBUG_BUNDLE
		Debug.Log( "RootBundleCallback()" );
		#endif
		{
			BundleHelper.SetRootBundle( p_object );
		}
	}

	public void FileBundleCallback( ref WWW p_www, string p_path, Object p_object ){
		#if DEBUG_BUNDLE
		Debug.Log( "FileBundleCallback()" );
		#endif

		{
			LoadingHelper.ItemLoaded( PrepareBundleHelper.GetLoadingSections(), PrepareBundleHelper.CONST_LOADING_BUNDLE_CONFIG );
		}

		{
			ManifestHelper.SetManifest( p_object );
		}

		if( PrepareBundleHelper.IsToUpdateBundle() ){
			BundleHelper.UpdateBundles();
		}
		else{
			#if DEBUG_BUNDLE
			Debug.Log( "All going well." );
			#endif

			// enter game
			BundleHelper.Instance().PreLoadResources();
		}
	}

	#endregion



	#region PopUp Window

	private static void ShowErrorBox( string p_error_string, UIBox.onclick p_click ){
		Global.CreateBox( PrepareBundleHelper.POPUP_TIPS_TITLE,
		                 p_error_string,
		                 "",
		                 null,
		                 PrepareBundleHelper.BUTTON_TXT_OK, 
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

		DebugHelper.ClearCommonCodeError();
	}

	#endregion



	#region Bundle Update Done

	public static void BundleUpdateDone(){
		Debug.Log ( "BundleUpdateDone()" );

		SetUpdateState( UpdateState.PREPARE_START_GAME );

		if( PrepareBundleHelper.IsDeviceCheckOpen() ){
			if( !DeviceHelper.CheckIsDeviceSupported() ){
				return;
			}
		}

		if ( ThirdPlatform.IsThirdPlatform () ) {
			Debug.Log( "PrepareBundleConfig( wait for third to start game )" );
			
			ThirdPlatform.Instance().UploadToken();
		}
		else {
			StartGame ();
		}
	}

	public static void StartGame(){
		Debug.Log( "PrepareBundleConfig.StartGame()" );

		SceneManager.EnterLogin();
	}

	#endregion



	#region Bundle Update State

	public enum UpdateState{
		SELECT_UPDATE_SERVER = 0,
		CHECKING_UPDATE_INFO,
		UPDATEING_BUNDLES,
		EXTRACT_BUNDLES,
		PRELOAD_BASE_BUNDLES,
		PRELOAD_RESOURCES,
		PREPARE_START_GAME,
	}
	
	private static UpdateState m_bundle_update_state = UpdateState.SELECT_UPDATE_SERVER;

	public static void SetUpdateState( UpdateState p_state ){
		Debug.Log( "SetUpdateState( " + p_state + " )" );

		m_bundle_update_state = p_state;
	}

	public static UpdateState GetBundleUpdateState(){
		return m_bundle_update_state;
	}

	#endregion
}