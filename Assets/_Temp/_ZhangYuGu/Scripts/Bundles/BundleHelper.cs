//#define DEBUG_LOADING_DETAIL

//#define DEBUG_BUNDLE_LEVEL

//#define DEBUG_BUNDLE_HELPER

//#define DEBUG_BUNDLE_HELPER_DETAIL



//#define DEBUG_DELAY_LOAD

//#define SKIP_NULL_ERROR



using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.11.30
 * @since:		Unity 5.1.3
 * Function:	Helper class for Use Bundles.
 * 
 * Notes:
 * 1.Help to manager Bundles in Unity 5.
 * 2.1st BigVersion is the same with SmallVersion, and without any bundle exist.
 * 3.2nd SmallVersion build with the same BigVersion, bundle will exist.
 */ 
public class BundleHelper : MonoBehaviour{

	#region Delay Load

	#if DEBUG_DELAY_LOAD
	public float m_delay_load_time = 0.05f;
	#else
	public float m_delay_load_time = 0.0f;
	#endif

	#endregion



	#region Instance

	private static BundleHelper m_instance = null;

	public static BundleHelper GetRef(){
		return m_instance;
	}

	public static BundleHelper Instance(){
		if( m_instance == null ){
			m_instance = GameObjectHelper.GetDontDestroyOnLoadGameObject().AddComponent<BundleHelper>();
		}

		return m_instance;
	}

	#endregion



	#region Mono

	void Awake(){
		UpdateLastFrameTime();
	}

	void OnGUI(){
		UpdateLastFrameTime();
	}

	void OnDestroy(){
//		Debug.LogError( "BundleHelper.OnDestroy()" );

		m_instance = null;

		m_manifest = null;
			
		m_bundle_dict.Clear();
				
		m_to_load_list.Clear();
	}

	#endregion



	#region Clean
	
	public static void CleanBundleConfigs(){
//		Debug.Log( "Clear PlayerPrefs & Caching." );
		
		{
			CleanBundleVersionPrefs();
		}
		
		{
			CleanCache();
		}
	}

	private static void CleanBundleVersionPrefs(){
		Debug.Log( "CleanBundleVersionPrefs()" );
		
		PlayerPrefs.DeleteKey( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION );

		PlayerPrefs.DeleteKey( ConstInGame.CONST_PLAYER_PREFS_KEY_CACHED_ROOT_BUNDLE_VERSION );

		PlayerPrefs.DeleteKey( ConstInGame.CONST_FIRST_TIME_TO_PLAY_VIDEO );
		
		PlayerPrefs.DeleteKey( ConstInGame.CONST_EXTRACT_BUNBLES_KEY );
		
		PlayerPrefs.Save();
	}

	public static void CleanCache(){
		Debug.Log( "Clean Cache." );
		
		Caching.CleanCache();
	}

	public static void CleanToLoad(){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "BundleHelper.CleanToLoad()" );
		#endif
		
		{
			if( m_to_load_list.Count > 0 ){
				Debug.LogError( "m_to_load_list.Count: " + m_to_load_list.Count );
				
				for( int i = m_to_load_list.Count - 1; i >= 0; i-- ){
					Debug.LogError( i + ": " + m_to_load_list[ i ].m_res_asset_path + " - " + m_to_load_list[ i ].m_url );
				}
			}
			
			{
				m_to_load_list.Clear();
			}
		}
	}

	#endregion



	#region Load Bundle

	/// static to keep data
	private static Dictionary<string, BundleContainer> m_bundle_dict	= new Dictionary<string, BundleContainer>();

	private static List<LoadTask> m_to_load_list = new List<LoadTask>();

	public List<LoadTask> GetLoadTaskList(){
		return m_to_load_list;
	}

	public static void LoadLevelAsset( string p_scene_path, Bundle_Loader.LoadResourceDone p_delegate ){
		string t_bundle_key = ManifestHelper.GetBundleKey( p_scene_path );
		
		if( !string.IsNullOrEmpty( t_bundle_key ) ){
			if( ManifestHelper.IsSceneAsset( p_scene_path ) ){
				p_scene_path = "";
			}
		}
		else{
			p_scene_path = "";
		}
		
		#if DEBUG_BUNDLE_HELPER || DEBUG_BUNDLE_LEVEL
		Debug.Log( "BundleHelper.LoadLevelAsset( " + p_scene_path + " )" );
		#endif
		
		LoadAsset( p_scene_path, null, p_delegate );
	}

	/// Most Common use.
	public static void LoadAsset( string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list = null ){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "BundleHelper.LoadAsset( " + p_resource_path + " )" );
		#endif

		string t_url = "";

		string t_bundle_key = ManifestHelper.GetBundleKey( p_resource_path );

		#if DEBUG_BUNDLE_HELPER_DETAIL
		Debug.Log( "t_bundle_key( " + t_bundle_key + " )" );
		#endif

		if( !string.IsNullOrEmpty( t_bundle_key ) ){
			t_url = GetUrlWithBundleKey( t_bundle_key );

			if( ManifestHelper.IsSceneAsset( p_resource_path ) ){
				p_resource_path = "";
			}
		}

		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "BundleHelper.LoadAsset( " + p_resource_path + " - " + t_bundle_key + " - " + 
		          t_url + PrepareBundleHelper.GetCurrentBundleVersion() + " )" );
		#endif

		BundleHelper.LoadAsset( t_url, PrepareBundleHelper.GetCurrentBundleVersion(),
		                       t_bundle_key, p_resource_path,
		                       p_delegate, p_callback_list );
	}

	/// Load Asset with Detail Info.
	public static void LoadAsset( string p_url, int p_version, 
									string p_bundle_key, string p_asset_path, 
									Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list = null,
									System.Type p_type = null, bool p_active_dependence_load = true ){
//		Debug.Log( m_to_load_list.Count + " Add_To_Load_List: " + p_asset_path + " - " + p_bundle_key );

		if( p_active_dependence_load ){
			if( !string.IsNullOrEmpty( p_bundle_key ) ){
				AddAssetDependence( p_bundle_key );
			}
		}

		{
			AddLoadTask( p_url, p_version, p_asset_path, p_delegate, p_callback_list, p_type );
		}

		{
			DownloadTheToLoad();
		}
	}

	private static void AddLoadTask( string p_url, int p_version, 
	                                string p_asset_path, Bundle_Loader.LoadResourceDone p_delegate, 
	                                List<EventDelegate> p_callback_list = null, 
	                                System.Type p_type = null ){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "AddLoadTask: " + p_asset_path + " , " + p_type + " , " + p_url + " , " + p_version );
		#endif

		LoadTask t_task = new LoadTask( p_url, p_version, p_asset_path, p_delegate, p_callback_list, p_type );
		
		m_to_load_list.Add( t_task );
	}

	private static void AddAssetDependence( string p_bundle_key ){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "AddAssetDependence: " + p_bundle_key );
		#endif

		string[] t_dependence = GetBundleDependence( p_bundle_key );

		if( t_dependence == null ){
//			Debug.LogWarning( "No Dependence Found: " + p_bundle_key );

			return;
		}

		for( int i = 0; i < t_dependence.Length; i++ ){
			#if DEBUG_BUNDLE_HELPER
			Debug.Log( "AddAssetDependence Item: " + i + " : " + t_dependence[ i ] );
			#endif

			AddLoadTask( GetUrlWithBundleKey( t_dependence[ i ] ), PrepareBundleHelper.GetCurrentBundleVersion(),
			            "", null );
		}
	}

	#endregion



	#region Inner Load

	private static WWW m_www = null;

	private static void DownloadTheToLoad(){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "DownloadTheToLoad()" );
		#endif

		if( IsDownloading() ){
			#if DEBUG_BUNDLE_HELPER_DETAIL
			Debug.Log( "IsDownloading." );
			#endif

			return;
		}
		
		if( m_to_load_list.Count > 0 ){
			#if DEBUG_BUNDLE_HELPER
			Debug.Log( "Try DownloadTheToLoad: " + m_to_load_list.Count );
			#endif

			{
				LoadingHelper.BeforeStartCoroutine();
				
				BundleHelper.Instance().StartCoroutine( BundleHelper.Instance().DownloadAndCache() );
			}
		}
	}

	IEnumerator DownloadAndCache (){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "DownloadAndCache()" + Time.realtimeSinceStartup );
		#endif

		if( m_to_load_list.Count <= 0 ){
			yield break;
		}

//		#if DEBUG_LOADING_DETAIL
//		LoadingHelper.LogTimeSinceLoading( "BundleHelper.DownloadAndCache()" );
//		#endif

		{
			SetIsDownloading( true );
		}

		do{
			LoadTask p_bundle_to_load = m_to_load_list[ 0 ];
			
			{
				m_to_load_list.RemoveAt( 0 );
			}

			bool t_is_local_res = string.IsNullOrEmpty( p_bundle_to_load.m_url ) ? true : false;
			
			string p_url = p_bundle_to_load.m_url;
			
			int p_version = p_bundle_to_load.m_version;
			
			AssetBundle t_bundle = null;
			
			bool load_bundle_success = false;
			
			bool t_contains = m_bundle_dict.ContainsKey( p_url );
			
			#if DEBUG_BUNDLE_HELPER_DETAIL
			Debug.Log( "t_is_local_res: " + t_is_local_res );
			
			Debug.Log( "t_contains: " + t_contains );
			#endif
			
			#if DEBUG_DELAY_LOAD
			while( true ){
				float t_wait = UtilityTool.GetRandom( 0.0f, m_delay_load_time );
				
				yield return new WaitForSeconds( t_wait );
				
				break;
			}
			#endif
			
			LoadingHelper.BeforeLoadRes();
			
			if( !t_is_local_res ){
				if( !t_contains ){
					{
						//				Debug.Log( "Load From Server Bundle: " + p_url );
						
						if( Caching.enabled ) { 
							while ( !Caching.ready ){
								Debug.LogError( "Caching is not ready." );
								
								yield return null;
							}
							
							m_www = WWW.LoadFromCacheOrDownload( p_url, p_version );
						}
						else {
							m_www = new WWW( p_url );
						}
						
						yield return m_www;
						
						if( ConfigTool.GetBool( ConfigTool.CONST_LOG_BUNDLE_DOWNLOADING, false ) ){
							Debug.Log( "WWW.LoadFromCacheOrDownload( " + " - " + 
							          PathHelper.GetFileNameFromPath( p_url ) + " - " +
							          p_version + " - " + p_url + " )" );
						}
						
						if( m_www.error != null ) {
							Debug.LogError( m_www.error + " : " + p_version + ", " + p_url );
							
							m_www.Dispose();
						}
						else if( m_www.assetBundle == null ){
							Debug.LogError ("t_www.assetBundle = null.");
						}
						else{
							load_bundle_success = true;
							
							t_bundle = m_www.assetBundle;
							
							{
								BundleContainer t_container = new BundleContainer( p_url, p_version, t_bundle );
								
								m_bundle_dict.Add( p_url, t_container );
								
	//							#if DEBUG_BUNDLE_HELPER
	//							Debug.Log( "-------- Bundle Loaded: " + t_container.GetBundleDescription() );
	//
	//							t_container.LogAllAssetNames();
	//							
	//							t_container.LogAllScenePaths();
	//							#endif
							}
						}
					}
				}
				else{
					load_bundle_success = true;
					
					t_bundle = m_bundle_dict[ p_url ].GetBundle();
				}
				
				if( !load_bundle_success ){
					Debug.LogError( "DownloadAndCache fail." );
					
					m_www = null;
				}
			}
			
			{
				LoadingHelper.BeforeReadyToLoadNextAsset();
				
				while( !IsReadyToLoadNextAsset() ){
					#if DEBUG_BUNDLE_HELPER
					Debug.Log( "WaitingToLoad: " + p_bundle_to_load.m_res_asset_path );
					#endif
					
					yield return new WaitForEndOfFrame();
				}
				
				LoadingHelper.AfterReadyToLoadNextAsset();
			}
			
			float t_start_time = Time.realtimeSinceStartup;
			
			UnityEngine.Object t_object = null;
			
			if( t_is_local_res ){
				if( !string.IsNullOrEmpty( p_bundle_to_load.m_res_asset_path ) ){
					if ( p_bundle_to_load.m_type != null){
						#if DEBUG_BUNDLE_HELPER
						Debug.Log( "Loading: " + p_bundle_to_load.m_res_asset_path + " - " + p_bundle_to_load.m_type );
						#endif
						
						t_object = Resources.Load( p_bundle_to_load.m_res_asset_path, p_bundle_to_load.m_type );
					}
					else{
						#if DEBUG_BUNDLE_HELPER
						Debug.Log( "Loading: " + p_bundle_to_load.m_res_asset_path );
						#endif
						
						t_object = Resources.Load( p_bundle_to_load.m_res_asset_path );
						
						//					#if DEBUG_BUNDLE_HELPER
						//					Debug.Log( "Loading Done: " + p_bundle_to_load.m_res_asset_path );
						//					#endif
					}
					
					if( ConfigTool.GetBool( ConfigTool.CONST_LOG_ASSET_LOADING, false ) ){
						Debug.Log( "Resources.Loaded: " + p_bundle_to_load.m_res_asset_path );
					}
					
					if( t_object == null ){
						#if !SKIP_NULL_ERROR
						Debug.LogError( "Resources.Load null: " + 
						               p_bundle_to_load.m_res_asset_path + " - " +
						               p_url + " - " + p_version );
						#endif
					}
				}
			}
			else if( load_bundle_success ){
				string t_asset_name = PathHelper.GetFileNameFromPath( p_bundle_to_load.m_res_asset_path );
				
				if( !string.IsNullOrEmpty( t_asset_name ) ){
					if( p_bundle_to_load.m_type != null ){
						t_object = m_bundle_dict[ p_url ].LoadBundleAsset( t_asset_name, p_bundle_to_load.m_type );
					}
					else{
						t_object = m_bundle_dict[ p_url ].LoadBundleAsset( t_asset_name );
					}
					
					if( ConfigTool.GetBool( ConfigTool.CONST_LOG_ASSET_LOADING, false ) )
					{
						Debug.Log( "BundleHelper.Load: " + t_asset_name + " - " + 
						          p_bundle_to_load.m_res_asset_path + " - " +
						          p_url + " - " + p_version );
					}
					
					if( t_object == null ){
						#if !SKIP_NULL_ERROR
						Debug.LogError( "BundleHelper.Load null: " + t_asset_name + " - " + 
						               p_bundle_to_load.m_res_asset_path + " - " +
						               p_url + " - " + p_version );
						#endif
					}
				}
			}

			float t_load_done_time = Time.realtimeSinceStartup;

			float t_delegate_done_time = 0;

			float t_watcher_done_time = 0;

			{
				try{
					if( p_bundle_to_load.m_delegate != null ){
						if( string.IsNullOrEmpty( p_bundle_to_load.m_res_asset_path ) ){
//						#if DEBUG_BUNDLE_HELPER
//						Debug.Log( "Executing delegate send url: " + p_bundle_to_load.m_res_asset_path );
//						#endif
							
							p_bundle_to_load.m_delegate( ref m_www, p_url, t_object );
						} 
						else{
//						#if DEBUG_BUNDLE_HELPER
//						Debug.Log( "Executing delegate send res path: " + p_bundle_to_load.m_res_asset_path );
//						#endif
							
							p_bundle_to_load.m_delegate( ref m_www, p_bundle_to_load.m_res_asset_path, t_object );
						}
					}
					
//				#if DEBUG_BUNDLE_HELPER
//				Debug.Log( "Before Execute callback list." );
//				#endif
					
					{
						EventDelegate.Execute( p_bundle_to_load.m_callback_list );
					}
					
//					#if DEBUG_BUNDLE_HELPER
//					Debug.Log( "After Execute callback list." );
//					#endif
				}
				catch( Exception t_e ){
					Debug.LogError( "Exception In Exception: " + t_e );
				}
				
//				#if DEBUG_BUNDLE_HELPER
//				Debug.Log( "Before Execute Global.ResoucesLoaded." );
//				#endif

				t_delegate_done_time = Time.realtimeSinceStartup;

				{
					Global.ResoucesLoaded( p_bundle_to_load.m_res_asset_path, t_object );
				}

				t_watcher_done_time = Time.realtimeSinceStartup;
				
//				#if DEBUG_BUNDLE_HELPER
//				Debug.Log( "After Execute Global.ResoucesLoaded." );
//				#endif
			}

			{
				if ( ConfigTool.GetBool( ConfigTool.CONST_LOG_ITEM_LOADING_TIME, false ) ){
					#if DEBUG_LOADING_DETAIL
					Debug.Log( "Load Cost: " + ( t_load_done_time - t_start_time ) + " - " + 
								"Delegate Cost: " + ( t_delegate_done_time - t_load_done_time ) + " - " + 
					          	"Watcher Cost: " + ( t_watcher_done_time - t_delegate_done_time ) + " - " +
								LoadingHelper.GetTimeSinceLoading() + " - " + 
								p_bundle_to_load.m_res_asset_path + " - " + 
								"still waiting: " + m_to_load_list.Count +
								" - " + Time.realtimeSinceStartup );
					#else
					Debug.Log( "Total Time: " + ( t_watcher_done_time - t_start_time ) + " - " + 
								LoadingHelper.GetTimeSinceLoading() + " - " + 
								p_bundle_to_load.m_res_asset_path );
					#endif
				}
			}

			{
				LoadingHelper.AssetLoaded();
				
				LoadingHelper.AddLoadingItemInfo(
					p_bundle_to_load.m_res_asset_path,
					LoadingHelper.GetAssetLoadedCount(),
					t_load_done_time - t_start_time,
					t_watcher_done_time - t_load_done_time );
			}
			
			{
				m_www = null;
				
//				#if DEBUG_BUNDLE_HELPER
//				Debug.Log( "Before DownLoad Next." );
//				#endif
			}
		}
		while( m_to_load_list.Count > 0 );

		{
			SetIsDownloading( false );
		}

//		#if DEBUG_LOADING_DETAIL
//		LoadingHelper.LogTimeSinceLoading( "BundleHelper.DownloadAndCache() Done." );
//		#endif

//		#if DEBUG_BUNDLE_HELPER
//		Debug.Log( "DownloadAndCache.Finish( " + p_bundle_to_load.GetDescription() + " )" );
//		#endif
		
		yield break;
	}

	private static bool m_is_downloading = false;
	
	private static void SetIsDownloading( bool p_is_downloading ){
		m_is_downloading = p_is_downloading;
	}
	
	public static bool IsDownloading(){
		return m_is_downloading;
	}

	private static float m_last_frame_time = -1.0f;

	private static void UpdateLastFrameTime(){
		m_last_frame_time = Time.realtimeSinceStartup;
	}
	
	public static bool IsReadyToLoadNextAsset(){
		if( m_instance == null ){
			Debug.Log( "Not In Loading Scene." );
			
			return true;
		}
		else{
			float t_delta = Time.realtimeSinceStartup - m_last_frame_time;

			float t_target_time = ConfigTool.GetFloat( ConfigTool.CONST_LOADING_INTERVAL, 1.0f );

			if( t_delta > t_target_time && m_last_frame_time > 0 && Time.realtimeSinceStartup > 0 ){
				#if DEBUG_BUNDLE_HELPER
				Debug.Log( "t_delta: " + t_delta );

				Debug.Log( "t_target_time: " + t_target_time );

				Debug.Log( "Time.realtimeSinceStartup: " + Time.realtimeSinceStartup );

				Debug.Log( "m_last_frame_time: " + m_last_frame_time );
				#endif

				return false;
			} 
			else{
//				Debug.Log( Time.realtimeSinceStartup + " ---> " + m_last_frame_time );

				return true;
			}
		}
	}

	#endregion



	#region Root Bundle

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android/Android
	public static string GetRootBundleUrl(){

		return PrepareBundleHelper.GetServerBundleUrlPrefix() + "/" + PlatformHelper.GetPlatformTag();
	}

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android/platform/file
	public static string GetFileUrl(){
		return PrepareBundleHelper.GetServerBundleUrlPrefix() + "/" + ManifestHelper.CONST_MANIFEST_FOLDER_NAME + "/" + ManifestHelper.CONST_FILE_NAME;
	}

	/// http://192.168.0.176:8080/wsRes/rep/201501281419/Android/assets/resources/_data/config/config
	public static string GetUrlWithBundleKey( string p_bundle_key ){
		return PrepareBundleHelper.GetServerBundleUrlPrefix() + "/" + p_bundle_key;
	}

	private static AssetBundleManifest m_manifest	= null;

	public static void SetRootBundle( UnityEngine.Object p_object ){
		m_manifest = (AssetBundleManifest)p_object;

		if( m_manifest == null ){
			Debug.LogError( "Error in SetRootBundle(), manifest is null." );

			return;
		}

		#if DEBUG_BUNDLE_HELPER
		string[] t_bundles = m_manifest.GetAllAssetBundles();

		for( int i = 0; i < t_bundles.Length; i++ ){
			Debug.Log( "Bundle " + i + " : " + t_bundles[ i ] );

			string[] t_dependence = m_manifest.GetAllDependencies( t_bundles[ i ] );

			for( int j = 0; j < t_dependence.Length; j++ ){
				Debug.Log( "Dependence " + j + " : " + t_dependence[ j ] );
			}
		}
		#endif
	}

	public static string[] GetAllBundles(){
		if( m_manifest == null ){
			return null;
		}

		return m_manifest.GetAllAssetBundles();
	}

	public static string[] GetBundleDependence( string p_bundle ){
		if( m_manifest == null ){
			return null;
		}

		return m_manifest.GetAllDependencies( p_bundle );
	}


	#endregion



	#region Update

	private static int m_bundle_updated_count = 0;

	public static void UpdateBundles(){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "UpdateBundles()" );
		#endif

		{
			m_bundle_updated_count = 0;
		}

		PrepareBundleHelper.SetCurrentBundleVersion( PrepareBundleHelper.GetNewRootBundleVersion() );
		
		string[] t_bundles = GetAllBundles();

		{
			LoadingHelper.UpdateSectionInfo( PrepareBundleHelper.GetLoadingSections(), PrepareBundleHelper.CONST_UPDATE_BUNDLES, t_bundles.Length );
		}
		
		for( int i = 0; i < t_bundles.Length; i++ ){
			string t_url = GetUrlWithBundleKey( t_bundles[ i ] );

//			#if DEBUG_BUNDLE_HELPER
//			Debug.Log( "GetBundleUrl " + i + " : " + t_bundles[ i ] + "      " + t_url );
//			#endif

			LoadAsset( t_url, PrepareBundleHelper.GetCurrentBundleVersion(),
			          	t_bundles[ i ], "",
			          	BundleHelper.Instance().BundleUpdateCallback, null,
			          	null, 
			          	false );
		}
	}

	public void BundleUpdateCallback( ref WWW p_www, string p_path, System.Object p_object ){
		{
			LoadingHelper.ItemLoaded( PrepareBundleHelper.GetLoadingSections(), PrepareBundleHelper.CONST_UPDATE_BUNDLES );
		}

		{
			m_bundle_updated_count++;

			#if DEBUG_BUNDLE_HELPER
			Debug.Log( "UpdateBundles( " + m_bundle_updated_count + " / " + GetAllBundles().Length + ")" );
			#endif

			if( m_bundle_updated_count == GetAllBundles().Length ){
				#if DEBUG_BUNDLE_HELPER
				Debug.Log( "Bundle Updated Done." );
				#endif

				{
					BundleHelper.CleanToLoad();
					
					PrepareBundleHelper.UpdateLocalBundleInfo();
				}
				
				{
					BundleHelper.Instance().PreLoadResources();
				}
			}
		}
	}
	
	#endregion



	#region Preload Bundle For Login

	public void PreLoadResources(){
		#if DEBUG_BUNDLE_HELPER
		Debug.Log( "PreLoadResources()" );
		#endif

		PrepareBundles.SetUpdateState( PrepareBundles.UpdateState.PRELOAD_RESOURCES );

		// reset count
		{
			m_data_for_loading_loaded = 0;
		}
		
		// load data
		{
			ConfigTool.Instance.LoadConfigs( LoadingDataDone );
			
			Res2DTemplate.LoadTemplates( LoadingDataDone );
			
			LanguageTemplate.LoadTemplates( LoadingDataDone );
			
			MyColorData.LoadTemplates( LoadingDataDone );
			
			QualityTool.Instance.LoadQualities( LoadingDataDone );



			// remember to update data count after you add somthing here: PREPARE_DATA_COUNT_FOR_LOADING
			LocalCacheTool.Instance().LoadConfig();
		}
	}

	private int m_data_for_loading_loaded 				= 0;
	
	public const int PREPARE_DATA_COUNT_FOR_LOADING		= 5;

	public void LoadingDataDone(){
		{
			m_data_for_loading_loaded++;

			LoadingHelper.ItemLoaded( PrepareBundleHelper.GetLoadingSections(), PrepareBundleHelper.CONST_LOADING_PRELOAD_RESOURCES );
			
//			SetCurLoading( "Template: " + m_data_for_loading_loaded );
		}
		
//		Debug.Log( "LoadingDataDone( " + m_data_for_loading_loaded + " )" );

		CheckLoadingResources();
	}
	
	void CheckLoadingResources(){
//		#if DEBUG_LOADING_DETAIL
//		Debug.Log( "CheckLoadingResources( " + m_data_for_loading_loaded + PREPARE_DATA_COUNT_FOR_LOADING + " )" );
//		#endif

		if( m_data_for_loading_loaded < PREPARE_DATA_COUNT_FOR_LOADING ){
			return;
		}

// load utilities
//		{
//			NetworkWaiting.Instance( true );
//		}
		
		{
			Bundle_Loader.Instance().SetShowTime( false );
		}
		
		{ 
			PrepareBundles.BundleUpdateDone ();
		}
	}
	
	#endregion



	/// Container class for runtime bundle.
	private class BundleContainer{
		private string m_url;
		
		private int m_version;
		
		private AssetBundle m_bundle;
		
		public BundleContainer( string p_url, int p_version, AssetBundle p_bundle ){
			if( string.IsNullOrEmpty( p_url ) ){
				Debug.LogError( "Url not specified." );
				
				return;
			}
			
			if( p_version < 0 ){
				Debug.LogError( "version error." );
				
				return;
			}
			
			if( p_bundle == null ){
				Debug.LogError( "bundle is null." );
				
				return;
			}
			
			m_url = p_url;
			
			m_version = p_version;
			
			m_bundle = p_bundle;
		}
		
		public AssetBundle GetBundle(){
			return m_bundle;
		}

		/// Sync Load.
		public UnityEngine.Object LoadBundleAsset( string p_asset_name ){
			if( string.IsNullOrEmpty( p_asset_name ) ){
				return m_bundle.mainAsset;
			}
			else{
				return m_bundle.LoadAsset( p_asset_name );
			}
		}

		public UnityEngine.Object LoadBundleAsset( string p_asset_name, System.Type p_type ){
			if( string.IsNullOrEmpty( p_asset_name ) ){
				return m_bundle.mainAsset;
			}
			else{
				return m_bundle.LoadAsset( p_asset_name, p_type );
			}
		}

		public string GetBundleDescription(){
			return m_url + "   - " + m_version;
		}
		
		public void LogAllAssetNames(){
			string[] t_assets = m_bundle.GetAllAssetNames();
			
			if( t_assets == null ){
				Debug.LogError( "Error, no asset exist: " + GetBundleDescription() );
				
				return;
			}
			
			for( int i = 0; i < t_assets.Length; i++ ){
				Debug.Log( "Asset " + i + " : " + t_assets[ i ] );
			}
		}

		public void LogAllScenePaths(){
			string[] t_paths = m_bundle.GetAllScenePaths();

			if( t_paths == null ){
				Debug.LogError( "Error, no path exist: " + GetBundleDescription() );

				return;
			}

			for( int i = 0; i < t_paths.Length; i++ ){
				Debug.Log( "Scene " + i + " : " + t_paths[ i ] );
			}
		}
	}

	public class LoadTask{
		public string m_url;
		
		public int m_version;

		public Bundle_Loader.LoadResourceDone m_delegate;
		
		public List<EventDelegate> m_callback_list;
		
		public string m_res_asset_path;
		
		public System.Type m_type;

		public LoadTask( string p_url, int p_version, 
		                	string p_asset_path, Bundle_Loader.LoadResourceDone p_delegate, 
		                    List<EventDelegate> p_callback_list = null, 
		                	System.Type p_type = null ){
			m_url = p_url;
			
			m_version = p_version;
			
			m_delegate = p_delegate;
			
			m_res_asset_path = p_asset_path;
			
			m_type = p_type;
			
			m_callback_list = p_callback_list;
		}

		/// asset_path + url + version
		public string GetDescription(){
			return m_res_asset_path + " - " + m_url + " - " + m_version;
		}
	}
}