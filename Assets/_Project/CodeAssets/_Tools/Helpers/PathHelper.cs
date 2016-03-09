//#define DEBUG_PATH

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class PathHelper : MonoBehaviour {

	#region Common

	/**
	 * UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/Resources/_Data/Config/Version.txt", typeof(TextAsset) );
	 * 
	 * Application.dataPath --> Unity Editor: <path to project folder>/Assets
	 * 
	 * Application.persistentDataPath --> 
	 * 
	 */

	/** Params:
	* p_file_name: Local_File.bin
	*/
	public static string GetPersistentFilePath( string p_file_name ){
		string t_local_file_name = Application.persistentDataPath + "/" + p_file_name;
		
		return t_local_file_name;
	}

	/// Params:
	/// p_path: file://E:/WorkSpace_External/DynastyMobile_2014/Assets/StreamingAssets/Android/Resources/Data/BattleField/BattleFlags/BattleFlags_-22f14f9d
	/// 
	/// return: BattleFlags_-22f14f9d
	public static string GetFileNameFromPath( string p_path ){
		if( string.IsNullOrEmpty( p_path ) ){
			return "";
		}

		int t_index = p_path.LastIndexOf( '/' );
		
		if ( t_index < 0 ){
			return p_path;
		}
		else{
			return p_path.Substring( t_index + 1 );
		}
	}
	
	/** Get full path for OS to access files and folders.
     * 
     * Params:
     * 1.p_relative_path:
     *   "StreamingAssets/UIResources/MemoryTrace/MemoryTrace"
     *   "Assets/StreamingAssets/UIResources/MemoryTrace/MemoryTrace"
     */
	public static string GetFullPath_WithRelativePath( string p_res_relative_path ){
		// check first '/'
		if( !p_res_relative_path.StartsWith( "/" ) ){
			p_res_relative_path = "/" + p_res_relative_path;
		}
		
		while( p_res_relative_path.StartsWith( "/Assets" ) ){
			p_res_relative_path = StringHelper.RemovePrefix( p_res_relative_path, "/Assets" );
		}
		
		return Application.dataPath + p_res_relative_path;
	}
	
	/* Params:
     * 1.p_res_relative_path: 	"Unfiled";
     * 
     * Return:			"Assets/StreamingAssets/PlatformX/Unfiled";
     */
	public static string GetConfigRelativePath_WithRelativePath(string p_config_relative_path_name)
	{
		string t_config_prefix = Application.streamingAssetsPath;
		
		// Add platform prefix
		{
			string t_platform = "/";
			
			t_platform = t_platform + PlatformHelper.GetPlatformTag();
			
			t_config_prefix = t_config_prefix + t_platform;
		}
		
		return t_config_prefix + "/" + p_config_relative_path_name;
	}

	/// Desc:
	/// Asset Path for WWW.Load.
	///
	/// Params:
	/// p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	/// 
	/// return:
	/// OS.dataPath/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
	public static string GeStreamingAssetWWWPath( string p_res_asset_path ){
		{
			p_res_asset_path = StringHelper.RemovePrefix(p_res_asset_path, "/");
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		if ( Application.platform == RuntimePlatform.WindowsEditor ){
			return "file://" + Application.streamingAssetsPath + p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.OSXEditor ){
			return "file://" + Application.dataPath + "/StreamingAssets" + p_res_asset_path;
		}
		#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return "file://" + Application.streamingAssetsPath + p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// Android
			return "jar:file://" + Application.dataPath + "!/assets" + p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return "file://" + Application.dataPath + "/Raw" + p_res_asset_path;
		}
		
		#endif
		
		return null;
	}

	#endregion



	#region Mac

	/// return "/Users/dastan"
	public static string GetMacHome(){

		#if UNITY_EDITOR
		string t_process = Application.dataPath;

		int t_dst_index = -1;

		{
			t_process = t_process.Substring( 1 );

			t_process = t_process.Substring( t_process.IndexOf( '/' ) + 1 );

			t_process = t_process.Substring( t_process.IndexOf( '/' ) );
		}

		string t_target_path = Application.dataPath.Substring( 0, Application.dataPath.IndexOf( t_process ) );

		return t_target_path;

		#endif

		Debug.LogError( "Only Should be called under Editor." );

		return "";
	} 

	private const string XcodeProjectRelativePath = "/workspace/xcode_workspace/xcode_qixiong";

	public static string GetXCodeProjectFullPath(){
		return PathHelper.GetMacHome() + XcodeProjectRelativePath;
	}

	#endregion



	#region PC

	private const string AndroidProjectFullPath = "E:\\WorkSpace_Eclipse\\qixiong\\七雄无双";

	/// "E:\\WorkSpace_Eclipse\\qixiong\\七雄无双"
	public static string GetAndroidProjectFullPath(){
		return AndroidProjectFullPath;
	}

	#endregion



	#region Bundle

	/** Desc:
	 * Bundle Path for local www.Load, Only Should be used in Editor's Debug Mode(No Bundle exist in real package).
	 * 
	 * Params(U5):
	 * p_bundle_key:	"assets/resources/_data/config/config"
	 * p_bundle_key:	""
	 * 
	 * return:
	 * OS.dataPath/StreamingArchived/Platform/assets/resources/_data/config/config
	 * OS.dataPath/StreamingArchived/Platform
	 */
	public static string GetLocalFileWWWPath_U5_Test_Use( string p_res_asset_path ){
		if( !string.IsNullOrEmpty( p_res_asset_path ) ){
			p_res_asset_path = StringHelper.RemovePrefix( p_res_asset_path, "/" );
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		return "file://" + Application.dataPath + "/StreamingArchived" + 
					"/" + PlatformHelper.GetPlatformTag() + p_res_asset_path;
		#else
		Debug.LogError( "Never Should be Used In Real Package, only could be run under editor mode." );		
		#endif
		
		return null;
	}

	/** Desc:
	 * Bundle Path for WWW.Load.
	 * 
	 * 
	 * Params(U4):
	 * p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 
	 * Params(U5):
	 * p_bundle_key:	"assets/resources/_data/config/config"
	 * 
	 * return:
	 * OS.dataPath/StreamingAssets/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
	 */
	public static string GetLocalBundleWWWPath( string p_res_asset_path ){
		{
			p_res_asset_path = StringHelper.RemovePrefix( p_res_asset_path, "/" );
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		if( Application.platform == RuntimePlatform.WindowsEditor ){
			return "file://" + Application.streamingAssetsPath + 
				"/" + PlatformHelper.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.OSXEditor ){
			return "file://" + Application.dataPath + "/StreamingAssets" + 
				"/" + PlatformHelper.GetiOSTag() +
					p_res_asset_path;
		}
		#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return "file://" + Application.streamingAssetsPath + 
				"/" + PlatformHelper.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// Android
			return "jar:file://" + Application.dataPath + "!/assets" + 
				"/" + PlatformHelper.GetAndroidTag() + 	
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return "file://" + Application.dataPath + "/Raw" + 
				"/" + PlatformHelper.GetiOSTag() + 
					p_res_asset_path;
		}
		
		#endif
		
		return null;
	}
	
	/** Desc:
	 * Bundle Path for FileStream, use with caution.
	 * 
	 * Params:
	 * p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 
	 * return:
	 * OS.dataPath/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
	 */
	public static string GetLocalBundlePath( string p_res_asset_path ){
		{
			p_res_asset_path = StringHelper.RemovePrefix( p_res_asset_path, "/" );
			
			p_res_asset_path = "/" + p_res_asset_path;
		}
		
		#if UNITY_EDITOR
		if( Application.platform == RuntimePlatform.WindowsEditor ){
			return Application.streamingAssetsPath + 
				"/" + PlatformHelper.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.OSXEditor ){
			return Application.dataPath + "/StreamingAssets" + 
				"/" + PlatformHelper.GetiOSTag() +
					p_res_asset_path;
		}
		#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return Application.streamingAssetsPath + 
				"/" + PlatformHelper.GetAndroidTag() + 
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// TODO, Recheck
			// Android
			return "jar:" + Application.dataPath + "!/assets" + 
				"/" + PlatformHelper.GetAndroidTag() + 	
					p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return Application.dataPath + "/Raw" + 
				"/" + PlatformHelper.GetiOSTag() + 
					p_res_asset_path;
		}
		
		#endif
		
		return null;
	}

	#endregion



	#region Logs
	
	public static void LogPath(){
		#if UNITY_ANDROID && DEBUG_PATH
		Debug.Log( "dataPath: " + Application.dataPath );
		
		Debug.Log( "persistentDataPath: " + Application.persistentDataPath );
		
		Debug.Log( "streamingAssetsPath: " + Application.streamingAssetsPath );
		
		Debug.Log( "temporaryCachePath: " + Application.temporaryCachePath );
		#endif
	}

	#endregion



	#region Utilities

	public static string GetParentPath( string p_path ){
		if( string.IsNullOrEmpty( p_path ) ){
			return "";
		}

		int t_index = p_path.LastIndexOf( "/" );

		if( t_index > 0 ){
			return p_path.Substring( 0, t_index );
		}
		else{
			return p_path;
		}
	}

	#endregion
}
