#define SYNC_LOAD

#define SIMULATE_BUNDLE_LOAD

//#define CUSTOM_7ZIP_LOAD

//#define DEBUG_LOADING


using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.3
 * @since:		Unity 4.5.3
 * Function:	Manage Bundle files.
 * 
 * Notes:
 * None.
 */ 
public class Bundle_Loader : MonoBehaviour {
	
	//	public delegate void LoadResourceDone( UnityEngine.Object p_object );
	//	public delegate void LoadResourceDone( ref WWW p_www, string p_path, ref UnityEngine.Object p_object );
	/// Params:
	/// p_www:		www component.
	/// p_path:		asset path first, or bundle path if asset path not exist.
	/// p_object:	returned object.
	public delegate void LoadResourceDone( ref WWW p_www, string p_path, UnityEngine.Object p_object );
	
	public delegate void LoadAllBundlesDone();
	
	
	private class BundleToLoadTask{

		public enum BundleType{
			SERVER_BUNDLE,
			LOCAL_BUNDLE,
			LOCAL_RESOURCES,
		}

		public BundleType m_bundle_type;

		public string m_url;
		
		public int m_version;
		
		public string m_bundle_key;
		
		public LoadResourceDone m_delegate;

		public List<EventDelegate> m_callback_list;
		
		public string m_res_asset_path;

		public System.Type m_type;

		public bool m_open_simulate = false;

		public BundleToLoadTask( BundleType p_bundle_type, 
		                        string p_url, int p_version, 
		                        string p_bundle_key, 
		                        LoadResourceDone p_delegate, 
		                        string p_res_asset_path, System.Type p_type, 
		                        List<EventDelegate> p_callback_list, 
		                        bool p_open_simulate = false ){
			m_bundle_type = p_bundle_type;

			m_url = p_url;
			
			m_version = p_version;
			
			m_bundle_key = p_bundle_key;
			
			m_delegate = p_delegate;

			m_res_asset_path = p_res_asset_path;

			m_type = p_type;

			m_callback_list = p_callback_list;

			m_open_simulate = p_open_simulate;
		}

		public bool IsServerBundle(){
			return m_bundle_type == BundleType.SERVER_BUNDLE;
		}

		public bool IsLocalBundle(){
			return m_bundle_type == BundleType.LOCAL_BUNDLE;
		}
	}

	/// All Loaded Bundles.
	private static Dictionary<string, AssetBundle> m_bundle_dict = new Dictionary<string, AssetBundle> ();

	/// All Local Bundles Info, To Acc Bundles' Loading.
	/// 
	/// key: bundle's short name
	/// value: bundle info
	private static Dictionary<string, BundleInfo> m_bundle_info_dict = new Dictionary<string, BundleInfo>();

	/// Local Bundle Info.
	public class BundleInfo{
		public string m_full_name;

		// full path
		public List<string> m_item_list = new List<string>();

		// short name
		public List<string> m_depend_list = new List<string>();

		public void AddItemContained( string p_item_path ){
			m_item_list.Add( p_item_path );
		}

		public void AddItemDepended( string p_depended_short_name ){
			m_depend_list.Add( p_depended_short_name );
		}
	}

	public class BundleListInfo{
		public bool m_resp_returned = false;

		public JSONNode m_json = null;



		public bool m_is_success = false;

		public string m_big_version = "";

		public int m_downloaded_count = 0;

		public string m_url_prefix = "";

		public string m_small_version = "";

		public class BundleItem{
			public string m_url_prefix;

			public string m_key;
			
			public string m_version;

			public string GetUrl(){
				string t_url_prefix = UtilityTool.RemoveSurfix( m_url_prefix, "/" );
				
				return t_url_prefix + "/" + m_key;
			}
		}

		public List<BundleItem> m_item_list = new List<BundleItem>();

		
		public bool IsUpdateDone(){
			return m_downloaded_count >= m_item_list.Count;
		}
	}

	/// Server Bundle List.
	public static BundleListInfo m_bundle_list_info = null;

	private static float m_downloading_time_since_start_up = 0;
	
	private static float m_bundle_loader_awake_time = 0;
	
	
	private static List<BundleToLoadTask> m_to_load_list = new List<BundleToLoadTask>();
	
	
	
	#region Instance
	
	private static Bundle_Loader m_instance;

	private static WWW m_www = null;

	/** Desc:
	 * Load & Init Bundle Config.
	 */
	public static Bundle_Loader Instance(){
		if( m_instance == null ){
			GameObject t_gb = UtilityTool.GetDontDestroyOnLoadGameObject();
			
			m_instance = t_gb.AddComponent<Bundle_Loader>();
		}
		
		return m_instance;
	}
	
	#endregion
	
	
	
	#region Mono
	
	void Awake(){
		#if UNITY_EDITOR
//		Debug.Log( "Debug To Clean Cache." );

//		Prepare_Bundle_Cleaner.CleanCache();
		#else

		if (Application.platform == RuntimePlatform.WindowsPlayer ||
		    Application.platform == RuntimePlatform.OSXPlayer ){
			Prepare_Bundle_Cleaner.CleanCache();
		}
		
		#endif
	}
	
	// Use this for initialization
	void Start () {
		// Init
		{
			m_bundle_loader_awake_time = Time.realtimeSinceStartup;
		}
	}
	
	// Update is called once per frame
	void Update () {
//		if( m_www != null ){
//			Debug.Log( "m_www: " + m_www.progress + " - " + m_www.uploadProgress );
//		}
	}

	void OnGUI(){
		OnGUITime();
	}


	void OnGUITime(){
		if( !m_show_time ){
			return;
		}
		
		int t_label_index = 0;
		
		GUI.Label( GetLabelRect( t_label_index++ ), 
		          "Loader Duration: " + ( Time.realtimeSinceStartup - m_bundle_loader_awake_time ) );
		
		GUI.Label( GetLabelRect( t_label_index++ ), 
		          "DownLoad Time: " + GetDownLoadTime() );
	}

	void OnDestroy(){
		#if DEBUG_LOADING
		Debug.Log( "BundleLoader.OnDestroy()" );
		#endif

		ManualDestroy();
	}
	
	#endregion
	
	
	
	#region Clean
	
	/// Clean data.
	public static void CleanData(){
		#if DEBUG_LOADING
		Debug.Log( "Bundle_Loader.CleanData()" );
		#endif
		
		m_bundle_list_info = null;
		
		if( m_to_load_list.Count > 0 ){
			Debug.LogError( "m_to_load_list.Count: " + m_to_load_list.Count );
			
			for( int i = m_to_load_list.Count - 1; i >= 0; i-- ){
				Debug.LogError( i + ": " + m_to_load_list[ i ].m_res_asset_path + " - " + m_to_load_list[ i ].m_bundle_key );
			}
		}
		
		{
			m_to_load_list.Clear();
		}
	}
	
	void ManualDestroy(){
		#if DEBUG_LOADING
		Debug.Log( "BundleLoader.ManualDestroy()" );
		#endif
		
		CleanData();
		
		{
			m_bundle_dict.Clear();
			
			m_bundle_info_dict.Clear();
		}
	}
	
	#endregion
	
	
	
	#region GUI Utility
	
	private bool m_show_time = false;
	
	public void SetShowTime( bool p_will_show ){
		m_show_time = p_will_show;
	}
	
	private static Rect GetButtonRect( int p_index ){
		return new Rect( 100, p_index * 30, 300, 35 );	
	}
	
	private static Rect GetLabelRect( int p_index ){
		return new Rect( 30, p_index * 60, 250, 50);
	}
	
	#endregion
	
	
	
	#region Load Bundle Config
		
	/// Param:	ui_atlas_prefabs
	/// return: StreamingAssets/Android/ui_atlas_prefabs
	public static string GetConfigAssetPath( string p_asset_name ){
		string t_res_path_prefix = "StreamingAssets/" + UtilityTool.GetPlatformTag() + "/";
		
		return t_res_path_prefix + p_asset_name;
	}
	
	public static string GetConfigBundleKey(){
		string t_bundle_key = UtilityTool.GetPlatformTag() + Config_Bundle_Surfix;
		
		return t_bundle_key;
	}

	private float m_load_bundle_config_time	= 0;

	/// Loads all bundle configs.
	public void LoadAllBundleConfigs(){
		#if DEBUG_LOADING
		Debug.Log( "LoadAllBundleConfigs()" );
		#endif

		m_load_bundle_config_time = Time.realtimeSinceStartup;

		string t_bundle_key = GetConfigBundleKey();
		
//		string t_bundle_url = GetLocalBundlePath( t_bundle_name );
		
		{
			// set to prepare
			m_config_loaded_count = 0;
		}
		
		{
			float t_load_time = Time.realtimeSinceStartup;
			
			for(int i = 0; i < m_all_bundle_config_files.Length; i++ ){
				LoadAssetFromBundle(
					t_bundle_key,
					ConfigBundleLoaded,
					m_all_bundle_config_files[ i ] );
//					GetConfigAssetPath( m_preload_config_files[ i ] ) );
			}
		}
	}
	
	private void ConfigBundleLoaded( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		if( p_object == null ){
			Debug.LogError( "ConfigBundleLoaded.Res Not Found: " + p_path );
		}
		else{
			TextAsset t_text = ( TextAsset )p_object;
			
			string t_json_string = t_text.text;
			
			// json node
			{
				JSONNode t_bundle_node = JSONNode.Parse( t_json_string );
				
				PreProcessBundleInfo( t_bundle_node );
			}
		}

		{
			m_config_loaded_count++;

			StaticLoading.ItemLoaded( Prepare_Bundle_Config.m_loading_sections, Prepare_Bundle_Config.CONST_LOADING_BUNDLE_CONFIG );

			if( p_object != null ){
				Prepare_Bundle_Config.SetCurLoading( p_object.name );
			}
			else{
				Prepare_Bundle_Config.SetCurLoading( p_path );
			}
		}

//		Debug.Log( "ConfigBundleLoaded: " + m_config_loaded_count + " / " + m_preload_config_files.Length + 
//		          " - " + p_path );
	}
	
	public int m_config_loaded_count = 0;

	#endregion



	#region PreProcess Bundle Info

	private void PreProcessBundleInfo( JSONNode p_json_node ){
		// pre calc bundle name
		{
			JSONClass t_class = (JSONClass)p_json_node;

			int t_count = p_json_node.Count;

			string t_key = "";
			
			string t_short_name = "";
			
			char[] t_splitter = { CONFIG_DEPENDENCE_SPLITTER_TAG };

//			Debug.Log( "Count: " + t_count );
			
			for( int i = 0; i < t_count; i++ ){
				BundleInfo t_info = new BundleInfo();

//				Debug.Log( "------ Bundle Info ------" );

				// key & short name
				{
					{
						t_key = t_class.GetKey( i );
						
						t_info.m_full_name = t_key;

//						Debug.Log( "Full Name: " + t_key );
					}
					
					{
						if( t_key.LastIndexOf( "/" ) < 0 ){
							Debug.LogError( "Error: " + t_key );
						}
						
						t_short_name = UtilityTool.GetFileNameFromPath( t_key );

//						Debug.Log( "Short Name: " + t_short_name );
					}
				}

				// items
				{
					JSONNode t_node = p_json_node[ i ][ CONFIG_ITEMS_TAG ];
					
					for( int j = 0; j < t_node.Count; j++ ){
						string t_res_path = t_node[ j ][ CONFIG_ASSET_PATH_TAG ];

						t_info.AddItemContained( t_res_path );

//						Debug.Log( "Item " + j + " : " + t_res_path );
					}
				}

				// dependence
				{
					string t_depend = p_json_node[ i ][ CONFIG_DEPENDENCE_OPTIMIZED_TAG ];

					if( !string.IsNullOrEmpty( t_depend ) ){
						string[] t_depends = t_depend.Split( t_splitter );
						
						for( int j = 0; j < t_depends.Length; j++ ){
							if( !string.IsNullOrEmpty( t_depends[ j ] ) ){
								
								t_info.AddItemDepended( t_depends[ j ] );

//								Debug.Log( "Depend " + j + " : " + t_depends[ j ] );
							}
						}
					}
				}
				
				m_bundle_info_dict.Add( t_short_name, t_info );
			}
		}
	}

	#endregion


	
	#region Load Bundle
	
	/** Desc:
	 * 1. Add Task to load list;
	 * 2. if just bundle without callback, then check if loaded or if in list;
	 * 3. if bundle with callback, then still add to list;
	 * 
	 * Params:
	 * 1.p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 */
	private void Add_To_Load_List_With_Dependency( string p_bundle_key, LoadResourceDone p_delegate, string p_res_asset_path, System.Type p_type = null, List<EventDelegate> p_callback_list = null ){
		//string t_path = "file://" + Application.streamingAssetsPath + "/iOS/UIResources/MemoryTrace/Textures.unity3d";
		//string t_path = Application.streamingAssetsPath + "/Android/UIResources/MemoryTrace/Textures.unity3d";

		// check if exist
		{
			if( !NeedTo_AddTo_ToLoadList( p_bundle_key, p_delegate, p_res_asset_path, p_callback_list ) ){
				return;
			}
		}

		List<string> t_depend_list = GetBundleDependency( p_bundle_key );

		if( t_depend_list != null ){
			for( int i = 0; i < t_depend_list.Count; i++ ){
				if( !string.IsNullOrEmpty( t_depend_list[ i ] ) ){
					string t_full_path = Get_Bundle_Full_Path( t_depend_list[ i ] );
					
					Add_To_Load_List_With_Dependency( t_full_path, null, null, null );
				}
			}
		}

		// load bundle
		{
			// server
			for( int i = 0; i < m_bundle_list_info.m_item_list.Count; i++ ){
				if( m_bundle_list_info.m_item_list[ i ].m_key == p_bundle_key ){
					Debug.Log( "Loading From Server Bundle: " + m_bundle_list_info.m_item_list[ i ].GetUrl() );

					Add_To_Load_List(
						BundleToLoadTask.BundleType.SERVER_BUNDLE,
						m_bundle_list_info.m_item_list[ i ].GetUrl(),
						int.Parse( m_bundle_list_info.m_item_list[ i ].m_version ),
						m_bundle_list_info.m_item_list[ i ].m_key,
						p_delegate,
						p_res_asset_path,
						p_type,
						p_callback_list );

					return;
				}
			}

			{
				// all local bundle's version is 0.

#if CUSTOM_7ZIP_LOAD
				string t_bundle_url = GetLocalBundlePath( p_bundle_key );
#else
				string t_bundle_url = GetLocalBundleWWWPath( p_bundle_key );
#endif

//				Debug.Log( "Loading From Local Bundle: " + t_bundle_url );

				Add_To_Load_List(
					BundleToLoadTask.BundleType.LOCAL_BUNDLE,
					t_bundle_url,
					0,
					p_bundle_key,
					p_delegate,
					p_res_asset_path,
					p_type,
					p_callback_list );
			}
		}
	}

	private void Add_To_Load_List( BundleToLoadTask.BundleType p_bundle_type, string p_url, int p_version, string p_bundle_key, LoadResourceDone p_delegate, string p_res_asset_path, System.Type p_type = null, List<EventDelegate> p_callback_list = null, bool p_open_simulate = false ){
		// add to to-load list
		{
//			Debug.Log( m_to_load_list.Count + " Add_To_Load_List: " + p_res_asset_path + " - " + p_bundle_key );

			m_to_load_list.Add( new BundleToLoadTask( p_bundle_type, p_url, p_version, p_bundle_key, p_delegate, p_res_asset_path, p_type, p_callback_list, p_open_simulate ) );
		}
	}
	
	/** Desc:
	 * 1. if need to Add Task to load list;
	 * 2. if just bundle without callback, then check if loaded or if in list;
	 * 3. if bundle with callback, then still add to list;
	 */
	private bool NeedTo_AddTo_ToLoadList( string p_bundle_key, LoadResourceDone p_delegate, string p_res_asset_path, List<EventDelegate> p_callback_list ){
		if( ( p_delegate == null ) && 
		   	( p_callback_list == null ) && 
		   	string.IsNullOrEmpty( p_res_asset_path ) ){
			// check if exist in to-load list
			if( IsExistInToLoadList( p_bundle_key ) ){
				return false;
			}
			
			if( m_bundle_dict.ContainsKey( p_bundle_key ) ){
				return false;
			}
			
			return true;
		}
		else{
			return true;
		}
	}
	
	#endregion



	#region Unload Bundle

	/** Desc:
	 * Unload target bundle.
	 * CALL THIS FUNCTION CAREFULLY, DEPENDENCE LINK IS EASILY TO BREAK.
	 * 
	 * Params:
	 * 1.p_bundle_Key:	"Resources/Data/Design/Design_-fff7420a"
	 */
	public static void UnloadBundle( string p_bundle_key, bool p_unload_all_loaded_objects = true ){
		if( m_bundle_dict.ContainsKey( p_bundle_key ) ){
			AssetBundle t_bundle = m_bundle_dict[ p_bundle_key ];

			t_bundle.Unload( p_unload_all_loaded_objects );

			t_bundle = null;

			m_bundle_dict.Remove( p_bundle_key );
		}
		else{
			Debug.LogError( "Bundle_Loader.UnloadBundle Not Exist: " + p_bundle_key + " : " + p_unload_all_loaded_objects );
		}
	}

	#endregion


	
	#region To-Load List
	
	private bool IsExistInToLoadList( string p_bundle_key ){
		for( int i = 0; i < m_to_load_list.Count; i++ ){
			if( m_to_load_list[ i ].m_bundle_key == p_bundle_key ){
				return true;
			}
		}
		
		return false;
	}

	private void RemoveFromToLoadList( BundleToLoadTask p_task ){
		m_to_load_list.Remove( p_task );
	}
	
	#endregion
	
	
	
	#region Asset Loader

	/** Returns:
	 * true, if found in bundle;
	 * false, if not.
	 */
	public static bool LoadLevelBundle( string p_level_name, LoadResourceDone p_delegate ){
		string t_bundle_key = Find_Level_Bundle_Key( p_level_name );
		
		if( t_bundle_key == null ||
		   t_bundle_key == "" ){
//			Debug.LogError( "Level Not In Bundle: " + p_level_name );
			
			return false;
		}

		#if DEBUG_LOADING
		Debug.Log( "LoadLevelBundle( " + p_level_name + " : " + 
		          t_bundle_key + " )" );
		#endif
		
		LoadAssetFromBundle( t_bundle_key, p_delegate, null, null, null );
		
		return true;
	}

	public static float m_find_bundle_key_time	= 0.0f;

	/** Returns:
	 * true, if found in bundle;
	 * false, if not.
	 */
	public static bool LoadAssetFromBundle( string p_res_asset_path, System.Type p_type, LoadResourceDone p_delegate, List<EventDelegate> p_callback_list ){
		string t_bundle_key = Find_Asset_Bundle_Key( p_res_asset_path );

		if( t_bundle_key == null ||
		   t_bundle_key == "" ){
//			Debug.LogError( "Asset Not In Bundle: " + p_res_asset_path );
			
			return false;
		}
		
		LoadAssetFromBundle( t_bundle_key, p_delegate, p_res_asset_path, p_type, p_callback_list );
		
		return true;
	}

	/** Params:
	 * 1.p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 2.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 */
	public static void LoadAssetFromBundle( string p_bundle_key, LoadResourceDone p_delegate, string p_res_asset_path, System.Type p_type = null, List<EventDelegate> p_callback_list = null, bool p_log_loading = true ){
		#if DEBUG_LOADING
		if( p_log_loading ){
			string t_asset_name = GetAssetName( p_res_asset_path );
			
			Debug.Log( "LoadAssetFromBundle( " + UtilityTool.GetFileNameFromPath( t_asset_name ) + " - " +
			          t_asset_name + " - " + 
			          p_bundle_key + " )" );
		}
		#endif

		Bundle_Loader.Instance().Add_To_Load_List_With_Dependency( p_bundle_key, p_delegate, p_res_asset_path, p_type, p_callback_list );

		{
			Bundle_Loader.Instance().DownloadTheToLoad();
		}
	}

	public static void LoadAssetFromResources( string p_resource_path, System.Type p_type, LoadResourceDone p_delegate, List<EventDelegate> p_callback_list, bool p_open_simulate ){
		{
			Bundle_Loader.Instance().Add_To_Load_List(
				BundleToLoadTask.BundleType.LOCAL_RESOURCES,
				null,
				-1,
				null,
				p_delegate,
				p_resource_path,
				p_type,
				p_callback_list,
				p_open_simulate );
		}

		{
			Bundle_Loader.Instance().DownloadTheToLoad();
		}
	}

	/// Update Bundles Only.
	public void LoadBundleFromUrl( string p_url, int p_version, string p_bundle_key, LoadResourceDone p_delegate, string p_res_asset_path, List<EventDelegate> p_callback_list ){
		Add_To_Load_List(
			BundleToLoadTask.BundleType.SERVER_BUNDLE,
			p_url,
			p_version,
			p_bundle_key,
			p_delegate,
			p_res_asset_path,
			null,
			p_callback_list );
	}

	#endregion

	
	
	#region WWW.Download
	
	private static int m_bundle_loaded_count = 0;
	
	private static bool m_is_downloading = false;
	
	private void SetIsDownloading( bool p_is_downloading ){
		m_is_downloading = p_is_downloading;
	}
	
	private bool IsDownloading(){
		return m_is_downloading;
	}
	
	public void DownloadTheToLoad(){
		if( IsDownloading() ){
//			Debug.Log( "IsDownloading." );

			return;
		}

		if( m_to_load_list.Count > 0 ){
//			Debug.Log( "Try DownloadTheToLoad: " + m_to_load_list.Count );

			BundleToLoadTask t_load_task = m_to_load_list[ 0 ];
			
			m_bundle_loaded_count++;
			
			// remove from to-load list
			RemoveFromToLoadList( t_load_task );
			
			// set downloading flag
			SetIsDownloading( true );

			switch(  t_load_task.m_bundle_type ){
			case BundleToLoadTask.BundleType.SERVER_BUNDLE:
			case BundleToLoadTask.BundleType.LOCAL_BUNDLE:
				StartCoroutine( DownloadAndCache( t_load_task ) );
				break;

			case BundleToLoadTask.BundleType.LOCAL_RESOURCES:

				BeforeStartCoroutine();

				StartCoroutine( ResourcesDotLoad( t_load_task ) );
				break;
			}


		}
//		else{
//			Debug.Log( "List Empty: " + m_to_load_list.Count );
//		}
	}

	IEnumerator ResourcesDotLoad( BundleToLoadTask p_load_task ){
		#if DEBUG_LOADING
		Debug.Log( "ResourcesDotLoad: " + p_load_task.m_res_asset_path );
		#endif


		UnityEngine.Object t_object = null;
		
		TimeHelper.SignetTime();

		BeforeResDotLoad();

		{
			BeforeReadyToLoadNextAsset();

			while( !StaticLoading.IsReadyToLoadNextAsset() ){
//			Debug.Log( "WaitingToLoad: " + p_resource_path );
				
				yield return new WaitForEndOfFrame();
			}

			AfterReadyToLoadNextAsset();
		}

		#if SYNC_LOAD

		if ( p_load_task.m_type != null){
//			Debug.Log( "Loading: " + p_load_task.m_res_asset_path + " - " + p_load_task.m_type );

			t_object = Resources.Load( p_load_task.m_res_asset_path, p_load_task.m_type );
		}
		else{
//			Debug.Log( "Loading: " + p_load_task.m_res_asset_path );

			t_object = Resources.Load( p_load_task.m_res_asset_path );
		}

		{
			AfterResDotLoad();

			EnterNextScene.AssetLoaded();
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_ASSET_LOADING ) ){
			Debug.Log( "Resources.Loaded: " + p_load_task.m_res_asset_path );
		}

		{
			LoadingHelper.AddLoadingItemInfo(
				p_load_task.m_res_asset_path,
				EnterNextScene.GetAssetLoadedCount(),
				TimeHelper.GetDeltaTimeSinceSignet() );
		}
		
		if ( ConfigTool.GetBool( ConfigTool.CONST_LOG_ITEM_LOADING_TIME, false ) ){
			TimeHelper.LogDeltaTimeSinceSignet( EnterNextScene.GetAssetLoadedCount() + " - " + 
			                                    EnterNextScene.GetTimeSinceLoading() + " - " + 
			                                    p_load_task.m_res_asset_path );
		}
		#else
		ResourceRequest t_request = null;
		
		if( p_load_task.m_type != null ){
			t_request = Resources.LoadAsync( p_load_task.m_res_asset_path, p_load_task.m_type );
		}
		else{
			t_request = Resources.LoadAsync( p_load_task.m_res_asset_path );
		}
		
		yield return t_request;
		
		if( ConfigTool.m_is_log_asset_loading ){
			Debug.Log( "Resources.LoadAsync: " + p_load_task.m_res_asset_path );
		}
		
		if( ConfigTool.m_is_log_item_loading_time ){
			ConfigTool.LogItemLoadingTime( p_load_task.m_res_asset_path );
		}
		
		t_object = t_request.asset;
		#endif
		
		if ( t_object == null )
		{
			Debug.LogError("Resource Not Found: " + p_load_task.m_res_asset_path );
		}
		
//		LoadCallback(p_delegate, t_object, p_resource_path, p_callback_list, p_open_simulate);
		
		#if SIMULATE_BUNDLE_LOAD && UNITY_EDITOR
		if ( p_load_task.m_open_simulate ){
			int t_delay_count = UnityEngine.Random.Range( 0, 2 );
			
			for( int i = 0; i < t_delay_count; i++ ){
				Debug.Log( "SimulateBundleLoad.delay: " + 
				          i + " / " + t_delay_count + 
				          " - " + p_load_task.m_res_asset_path );
				
				yield return new WaitForEndOfFrame();
			}
		}
		#endif

		try
		{	
			{
				p_load_task.m_delegate( ref m_www, p_load_task.m_res_asset_path, t_object );
			}
			
			{
				EventDelegate.Execute( p_load_task.m_callback_list );
			}
		}
		catch( Exception t_e ){
			Debug.LogError( "Exception In Exception: " + t_e );
		}

		{
			Global.ResoucesLoaded( p_load_task.m_res_asset_path, t_object );
		}

		// set downloading flag
		{
			SetIsDownloading( false );
		}

		// download the next bundle
		{
//			Debug.Log( "DownloadAndCache.Ready To Load Next." );
			
			DownloadTheToLoad();
		}

		yield break;
	}

	IEnumerator DownloadAndCache ( BundleToLoadTask p_bundle_to_load ){
		string p_url = p_bundle_to_load.m_url;
		int p_version = p_bundle_to_load.m_version;
		string p_bundle_dict_key = p_bundle_to_load.m_bundle_key;

		ResetDownLoadTime();
		
		AssetBundle t_bundle = null;
		
		bool load_bundle_success = false;
		
		bool t_contains = m_bundle_dict.ContainsKey( p_bundle_dict_key );
		
		// load bundle from www, if not loaded
		if( !t_contains ){
#if CUSTOM_7ZIP_LOAD
			if( p_bundle_to_load.IsServerBundle() )
#else
#endif
			{
//				Debug.Log( "Load From Server Bundle: " + p_url );

				if( Caching.enabled ) { 
					while ( !Caching.ready ){
						yield return null;
					}
					
					m_www = WWW.LoadFromCacheOrDownload( p_url, p_version );
				}
				else {
					m_www = new WWW( p_url );
				}
				
				yield return m_www;
				
				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_BUNDLE_DOWNLOADING, false ) ){
					Debug.Log( "WWW.LoadFromCacheOrDownload( " + GetDownLoadTime() + " - " + 
					          UtilityTool.GetFileNameFromPath( p_url ) + " - " +
					          p_version + " - " +
					          p_url + " )" );
				}
				
				if( m_www.error != null ) {
					Debug.LogError( m_www.error + " : " + 
					               p_bundle_dict_key + ", " +
					               p_version + ", " +
					               p_url );
					
					m_www.Dispose();
				}
				else if( m_www.assetBundle == null ){
					Debug.LogError ("t_www.assetBundle = null.");
				}
				else{
//				Debug.Log( "WWW.LoadFromCacheOrDownload.Done." );
					
					load_bundle_success = true;
					
					t_bundle = m_www.assetBundle;
				}
			}
#if CUSTOM_7ZIP_LOAD
			else
			{
//				Debug.Log( "Load From Local Bundle: " + p_bundle_dict_key + " - " + p_url );

				string t_local_file_path = UtilityTool.GetPersistentFilePath( p_bundle_dict_key );

				t_local_file_path = t_local_file_path + "_un";

				{
					FileInfo t_out_file = new FileInfo( t_local_file_path );

					if( !t_out_file.Exists ){
//						Debug.Log( "Try To UnZipping the Bundle." );

						SevenZip.BundleEncoder.Decode7Zip( p_url, t_local_file_path );
					}
					else{
//						Debug.Log( "Already UnZipped, Load Directly." );
					}
				}

//				Debug.Log( "Unzipped File: " + t_local_file_path );

				load_bundle_success = true;
				
				t_bundle = AssetBundle.CreateFromFile( t_local_file_path );
			}
#endif

			// add to bundle dict
			{
				m_bundle_dict[ p_bundle_dict_key ] = t_bundle;
			}
		}
		else{
			load_bundle_success = true;
			
			t_bundle = m_bundle_dict[ p_bundle_dict_key ];
		}
		
		if( !load_bundle_success ){
			Debug.LogError( "DownloadAndCache fail." );

			m_www = null;
			
//			yield break;
		}

		{
			while( !StaticLoading.IsReadyToLoadNextAsset() ){
//						Debug.Log( "WaitingToLoad: " + p_bundle_to_load.m_res_asset_path );
				
				yield return new WaitForEndOfFrame();
			}
		}
		
		string t_asset_name = GetAssetName( p_bundle_to_load.m_res_asset_path );

		UnityEngine.Object t_object = null;

		// load res asset delegate
		if( load_bundle_success ){
			if( !string.IsNullOrEmpty( t_asset_name ) ){
				TimeHelper.SignetTime();

#if SYNC_LOAD
					if( p_bundle_to_load.m_type != null ){
						t_object = m_bundle_dict[ p_bundle_dict_key ].LoadAsset( t_asset_name, p_bundle_to_load.m_type );
					}
					else{
						t_object = m_bundle_dict[ p_bundle_dict_key ].LoadAsset( t_asset_name );
					}

				{
					EnterNextScene.AssetLoaded();
				}

				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_ASSET_LOADING, false ) ){
//					if( BattleNet.IsLogNodeLoading() ){
					Debug.Log( "BundleLoader.Load: " + t_asset_name + " - " + 
					          p_bundle_to_load.m_res_asset_path + " - " +
					          p_bundle_dict_key );
				}

				{
					LoadingHelper.AddLoadingItemInfo(
						p_bundle_to_load.m_res_asset_path,
						EnterNextScene.GetAssetLoadedCount(),
						TimeHelper.GetDeltaTimeSinceSignet() );
				}

				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_ITEM_LOADING_TIME, false ) ){
					TimeHelper.LogDeltaTimeSinceSignet( EnterNextScene.GetAssetLoadedCount() + " - " + 
					                                    EnterNextScene.GetTimeSinceLoading() + " - " + 
					                                    p_bundle_to_load.m_res_asset_path );
				}
#else
				AssetBundleRequest t_request = null;
				
				if( p_bundle_to_load.m_type != null ){
					t_request = m_bundle_dict[ p_bundle_dict_key ].LoadAsync( t_asset_name, p_bundle_to_load.m_type );
				}
				else{
					t_request = m_bundle_dict[ p_bundle_dict_key ].LoadAsync( t_asset_name, typeof(Object) );
				}
				
				yield return t_request;

				if( ConfigTool.m_is_log_asset_loading ){
					Debug.Log( "BundleLoader.LoadAsync: " + t_asset_name + " - " + 
					          p_bundle_to_load.m_res_asset_path + " - " +
					          p_bundle_dict_key );
				}

				if( ConfigTool.m_is_log_item_loading ){
					ConfigTool.LogItemLoadingTime( p_bundle_to_load.m_res_asset_path );
				}

				t_object = t_request.asset;
#endif

				if( t_object == null ){
					Debug.LogError( "BundleLoader.Load: " + t_asset_name + " - " + 
					               p_bundle_to_load.m_res_asset_path + " - " +
					               p_bundle_dict_key );
				}
			}
		}

		{
			try{
				if( p_bundle_to_load.m_delegate != null ){
					if( string.IsNullOrEmpty( p_bundle_to_load.m_res_asset_path ) ){
						p_bundle_to_load.m_delegate( ref m_www, p_bundle_dict_key, t_object );
					} 
					else{
						p_bundle_to_load.m_delegate( ref m_www, p_bundle_to_load.m_res_asset_path, t_object );
					}
				}
				
				{
					EventDelegate.Execute( p_bundle_to_load.m_callback_list );
				}
			}
			catch( Exception t_e ){
				Debug.LogError( "Exception In Exception: " + t_e );
			}
			
			{
				Global.ResoucesLoaded( p_bundle_to_load.m_res_asset_path, t_object );
			}
		}
		
		{
			UpdateDownLoadTime();
		}
		
		// set downloading flag
		{
			m_www = null;

			SetIsDownloading( false );
		}

		// download the next bundle
		{
//			Debug.Log( "DownloadAndCache.Ready To Load Next." );

			DownloadTheToLoad();
		}
		
		yield break;
	}
	
	#endregion



	#region Coroutine Use

	private static float m_coroutine_time_tag = 0.0f;

	private static float m_total_coroutine_time = 0.0f;

	private static float m_total_prepare_load_next_asset_time = 0.0f;

	private static void ResetCoroutineTimeTag(){
		m_coroutine_time_tag = Time.realtimeSinceStartup;
	}

	public static void ResetCoroutineInfo(){
		ResetCoroutineTimeTag();

		m_total_coroutine_time = 0.0f;

		m_total_prepare_load_next_asset_time = 0.0f;
	}

	private void BeforeStartCoroutine(){
		ResetCoroutineTimeTag();
	}

	private void BeforeResDotLoad(){
		m_total_coroutine_time = m_total_coroutine_time + ( Time.realtimeSinceStartup - m_coroutine_time_tag );
	}

	private void AfterResDotLoad(){
		ResetCoroutineTimeTag();
	}

	private void BeforeReadyToLoadNextAsset(){
		ResetCoroutineTimeTag();
	}

	private void AfterReadyToLoadNextAsset(){
		m_total_prepare_load_next_asset_time = m_total_prepare_load_next_asset_time + ( Time.realtimeSinceStartup - m_coroutine_time_tag );
	}

	public static void LogCoroutineInfo(){
		Debug.Log( "Total Coroutine: " + m_total_coroutine_time );

		Debug.Log( "Total Prepare Next Asset: " + m_total_prepare_load_next_asset_time );
	}

	#endregion


	
	#region Load Bundle Utilities

	public static Dictionary<string, BundleInfo> GetBundleInfoDict(){
		return m_bundle_info_dict;
	}

	/** Params:
	 * 1.p_res_path:	"CreateRole";
	 * 
	 * return:
	 * "Project/ArtAssets/Scenes/CreateRole/CreateRole_unity_-12edcd52"
	 */
	private static string Find_Level_Bundle_Key( string p_level_name ){
		BundleInfo t_info = null;

		int t_count = 0;
		
		string t_item = "";
		
		string t_level_name = "";

		foreach( KeyValuePair<string, BundleInfo> t_key_value_pair in m_bundle_info_dict ){
			t_info = t_key_value_pair.Value;
			
			t_count = t_info.m_item_list.Count;
			
			for( int i = 0; i < t_count; i++ ){
				t_item = t_info.m_item_list[ i ];

				t_level_name = GetShortName( t_item );
				
				if( string.CompareOrdinal( t_level_name, p_level_name ) == 0 ){
					return t_info.m_full_name;
				}
			}
		}

//		Debug.LogError( "Error, can't find config: " + p_level_name );

		return "";
	}
	
	/** Params:
	 * 1.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 * 
	 * return:
	 * "Project/ArtAssets/Scenes/CreateRole/CreateRole_unity_-12edcd52"
	 */
	private static string Find_Asset_Bundle_Key( string p_res_asset_path ){
		string t_relative_bundle_path = GetResourceRelativePath( p_res_asset_path );

		BundleInfo t_info = null;

		foreach( KeyValuePair<string, BundleInfo> t_key_value_pair in m_bundle_info_dict ){
			t_info = t_key_value_pair.Value;

			int t_count = t_info.m_item_list.Count;

			string t_item = "";

			for( int i = 0; i < t_count; i++ ){
				t_item = t_info.m_item_list[ i ];

				if( string.CompareOrdinal( t_item, t_relative_bundle_path ) == 0 ){
					return t_info.m_full_name;
				}
			}
		}
		
//		Debug.LogError( "FindBundleKey.Error Bundle Not Found: " + t_relative_bundle_path );
		
		return "";
	}

	/** Desc:
	 * Bundle Path for FileStream.
	 * 
	 * Params:
	 * p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 
	 * return:
	 * OS.dataPath/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
	 */
	private static string GetLocalBundlePath( string p_res_asset_path ){
		{
			p_res_asset_path = UtilityTool.RemovePrefix( p_res_asset_path, "/" );
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		if( Application.platform == RuntimePlatform.WindowsEditor ){
			return Application.streamingAssetsPath + 
				"/" + UtilityTool.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.OSXEditor ){
			return Application.dataPath + "/StreamingAssets" + 
				"/" + UtilityTool.GetiOSTag() +
					p_res_asset_path;
		}
		#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return Application.streamingAssetsPath + 
				"/" + UtilityTool.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// TODO, Recheck
			// Android
			return "jar:" + Application.dataPath + "!/assets" + 
				"/" + UtilityTool.GetAndroidTag() + 	
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return Application.dataPath + "/Raw" + 
				"/" + UtilityTool.GetiOSTag() + 
					p_res_asset_path;
		}
		
		#endif
		
		return null;
	}
	
	/** Desc:
	 * Bundle Path for WWW.Load.
	 * 
	 * Params:
	 * p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 
	 * return:
	 * OS.dataPath/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
	 */
	private static string GetLocalBundleWWWPath( string p_res_asset_path ){
		{
			p_res_asset_path = UtilityTool.RemovePrefix( p_res_asset_path, "/" );
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		if( Application.platform == RuntimePlatform.WindowsEditor ){
			return "file://" + Application.streamingAssetsPath + 
				"/" + UtilityTool.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.OSXEditor ){
			return "file://" + Application.dataPath + "/StreamingAssets" + 
				"/" + UtilityTool.GetiOSTag() +
					p_res_asset_path;
		}
		#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return "file://" + Application.streamingAssetsPath + 
				"/" + UtilityTool.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// Android
			return "jar:file://" + Application.dataPath + "!/assets" + 
				"/" + UtilityTool.GetAndroidTag() + 	
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return "file://" + Application.dataPath + "/Raw" + 
				"/" + UtilityTool.GetiOSTag() + 
					p_res_asset_path;
		}
		
		#endif
		
		return null;
	}
	
	private static List<string> GetBundleDependency( string p_bundle_key ){
		float t_cur = Time.realtimeSinceStartup;

		BundleInfo t_info = null;
		
		foreach( KeyValuePair<string, BundleInfo> t_key_value_pair in m_bundle_info_dict ){
			t_info = t_key_value_pair.Value;
			
			int t_count = t_info.m_item_list.Count;
			
			string t_item = "";
			
			if( string.CompareOrdinal( t_info.m_full_name, p_bundle_key ) == 0 ){
				return t_info.m_depend_list;
			}
		}
		
		return null;
	}
	
	/** Params:
	 * 1.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 * 
	 * 
	 * return:			"Resources/_UIs/_CommonAtlas/Network/NetworkReConnectProcessor"
	 */
	private static string GetResourceRelativePath( string p_res_asset_path ){
		string t_relative_bundle_path = "Resources/" + p_res_asset_path;
		
		return t_relative_bundle_path;
	}
	
	/** Params:
	 * 1.p_file_name:	"NetworkReConnectProcessor_17";
	 */
	private static string Get_Bundle_Full_Path( string p_file_name ){
		// if it's a path, then return
		if( p_file_name.Contains( "/" ) ){
			return p_file_name;
		}

		{
			string t_value = "";
			BundleInfo t_info = null;

			m_bundle_info_dict.TryGetValue( p_file_name, out t_info );

			if( t_info != null ){
				return t_info.m_full_name;
			}
		}

		Debug.LogError( "Get_Bundle_Full_Path.Error, Bundle not found: " + p_file_name );
		
		return "";
	}
	
	#endregion
	
	
	
	#region Load Utilities
	
	private static void ResetDownLoadTime(){
		m_downloading_time_since_start_up = Time.realtimeSinceStartup;
	}
	
	private static float GetDownLoadTime(){
		return ( Time.realtimeSinceStartup - m_downloading_time_since_start_up );
	}
	
	private static void UpdateDownLoadTime(){
		Global.UpdateTotalDownloadTime( GetDownLoadTime() );
	}
	
	private static void LogDownLoadTime( string p_asset_name, string p_bundle_key ){
		Debug.Log( "Load Time: " + GetDownLoadTime() + " / " + Global.GetTotalDownloadTime() + " " +
		          p_asset_name + " - " + 
		          p_bundle_key );
	}
	
	private string GetConfigBundlePath(){
		// "Android/Android_Config"
		string t_bundle_path =	UtilityTool.GetPlatformTag() + Bundle_Loader.Config_Bundle_Surfix;
		
		return t_bundle_path;
	}
	
	/// Return just res name.
	private static string GetAssetName( string p_asset_path ){
		if( string.IsNullOrEmpty( p_asset_path ) ){
			return "";
		}
		
		string t_name = GetShortName( p_asset_path );
		
		return t_name;
	}

	/// Param: Project/ArtAssets/Scenes/Login/Login_unity_-7f7b6d66
	/// Return: Login_unity_-7f7b6d66
	private static string GetShortName( string p_path ){
		return p_path.Substring( p_path.LastIndexOf( "/" ) + 1 );
	}
	
	#endregion



	#region Shaders Utilities

	public static void CheckAllShaders(){
//		Debug.Log( "CheckAllShaders()" );

		TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_CHECK_SHADER );

		object[] t_renderers = GameObject.FindObjectsOfType( typeof(Renderer) );

		for( int i = 0; i < t_renderers.Length; i++ ){
			Renderer t_renderer = (Renderer)t_renderers[ i ];

			Material[] t_mats = t_renderer.sharedMaterials;

			for( int j = 0; j < t_mats.Length; j++ ){
				Material t_mat = t_mats[ j ];

				if( t_mat == null ){
					continue;
				}

				Shader t_shader = t_mat.shader;
				
//				Debug.Log( t_mat.name + " - " + t_shader.name );

				{
					#if DEBUG_LOADING
//					Debug.Log( "Reset Shader: " + t_shader.name + " - " + t_renderer.gameObject.name );
					#endif

					Shader t_new_shader = Shader.Find( t_shader.name );

					t_mat.shader = t_new_shader;
				}
			}
		}

		{
			TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_CHECK_SHADER );
		}
	}

	#endregion



	#region All Bundle Configs

	public static string[] m_all_bundle_config_files = {
//		Ui_Common_Config_Path_Name,
//		Ui_Images_Config_Path_Name,
		Data_Config_Path_Name,
//		Sound_Config_Path_Name,
//		D3D_Config_Path_Name,

//		Fx_Config_Path_Name,
//		New_Config_Path_Name,
//		Scenes_Config_Path_Name,
	};

	#endregion


	
	#region Config Keys
	
	public const string CONFIG_DEPENDENCE_TREE_TAG		= "Dependence";
	
	public const string CONFIG_DEPENDENCE_OPTIMIZED_TAG	= "D-O";
	
	public const string CONFIG_ITEMS_TAG				= "Items";
	
	public const string CONFIG_ASSET_PATH_TAG			= "asset";
	
	public const string CONFIG_CONTAINED_ASSET_TAG		= "ContainedAsset";
	
	public const char CONFIG_DEPENDENCE_SPLITTER_TAG	= ';';
	
	
	
	public const string Ui_Common_Config_Path_Name		= "ui_commons";
	
	public const string Ui_Images_Config_Path_Name		= "ui_images";
	
	public const string Ui_Unfiled_Config_Path_Name		= "ui_unfiled";
	
	public const string Data_Config_Path_Name			= "data";
	
	public const string Sound_Config_Path_Name			= "sound";

	public const string Scenes_Config_Path_Name			= "scenes";

	public const string D3D_Config_Path_Name			= "d3d";

	public const string Fx_Config_Path_Name				= "fx";

	public const string New_Config_Path_Name			= "new";


	public const string Config_Path_Ext					= ".txt";
	
	
	public const string Config_Bundle_Surfix			= "_Config";
	
	#endregion

}