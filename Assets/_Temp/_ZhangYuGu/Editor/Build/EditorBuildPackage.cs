#define BUILD_BUNDLES

//#define BUILD_PACKAGE

#define BUILD_PROJECT



#define DEBUG_BUILD

using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.7.10
 * @since:		Unity 4.5.3
 * Function:	Manage Versions to Help Build.
 * 
 * Setup:
 * 1.Config Unity path.
 * 
 * Build:
 * 1.Update Version.txt.
 * 2.Run Build.
 */ 
public class EditorBuildPackage : MonoBehaviour {

	private enum MenuItemPriority{
		BUILD_PACKAGE_ANDROID = 1,
		BUILD_PACKAGE_IOS,
		BUILD_PROJECT,

		BUILD_TEST
	}


	#region Mono
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	#endregion
	
	
	
	#region Build Menu Items

	#if BUILD_PACKAGE && ( UNITY_ANDROID || UNITY_STANDALONE )
	[MenuItem("Build/Project/Android", false, (int)MenuItemPriority.BUILD_PACKAGE_ANDROID )]
	#endif
	public static void BuildAndroid(){
		#if DEBUG_BUILD
		Debug.Log( "BuildAndroid()" );
		#endif

		ExecBuild( BuildOptions.Development );
	}

	#if BUILD_PACKAGE && UNITY_IOS
	[MenuItem("Build/Project/iOS", false, (int)MenuItemPriority.BUILD_PACKAGE_IOS )]
	#endif
	public static void BuildiOS(){
		#if DEBUG_BUILD
		Debug.Log( "BuildiOS()" );
		#endif
		
		ExecBuild( BuildOptions.Development );
	}

	#if BUILD_PROJECT
	[MenuItem("Build/Project/Project", false, (int)MenuItemPriority.BUILD_PROJECT )]
	#endif
	public static void BuildProject(){
		#if DEBUG_BUILD
		Debug.Log( "BuildProject()" );
		#endif

		ExecBuild( BuildOptions.Development | BuildOptions.AcceptExternalModificationsToPlayer );
	}

	#endregion



	#region Build

	#if BUILD_BUNDLES
	[MenuItem("Build/Bundles/Auto Build", false, (int)MenuItemPriority.BUILD_TEST )]
	#endif
	/// Automatic build big&small versions of bundles.
	public static void BuildBundles(){
		#if DEBUG_BUILD
		Debug.Log( "EditorBuildPackage.BuildBundles()" );
		#endif

		VersionTool.Instance().Init();

		VersionTool.VersionType t_type = GetVersionType();

		switch( t_type ){
		case VersionTool.VersionType.None:
			return;

		case VersionTool.VersionType.SmallVersion:
			BuildSmallVersion();
			break;

		case VersionTool.VersionType.BigVersion:
			BuildBigVersion();
			break;
		}
	}

	private static VersionTool.VersionType GetVersionType(){
		BuildTarget t_target = BuildTarget.Android;

		#if UNITY_IOS
		t_target = BuildTarget.iOS;
		#endif

		JSONNode t_node = Editor_Build_Bundle.GetArchivedJsonNode( t_target );

		string t_node_big = "";
		
		string t_node_base = "";
		
		string t_node_small = "";

		if( t_node != null ){
			t_node_big = t_node[ Editor_Build_Bundle.CONFIG_BUNDLE_BIG_VERSION_TAG ];
			
			t_node_base = t_node[ Editor_Build_Bundle.CONFIG_BUNDLE_BASE_VERSION_TAG ];
			
			t_node_small = t_node[ Editor_Build_Bundle.CONFIG_BUNDLE_SMALL_VERSION_TAG ];
		}

		if( t_node == null ){
			#if DEBUG_BUILD
			Debug.Log( "t_node = null." );
			#endif

			return VersionTool.VersionType.BigVersion;
		}
		else if( t_node_big != VersionTool.GetBigVersion() ){
			#if DEBUG_BUILD
			Debug.Log( "Archive Big: " + t_node_big );

			Debug.Log( "Cur: " + VersionTool.GetBigVersion() );
			#endif

			if( t_node_base == VersionTool.GetBaseVersion() ){
				Debug.LogError( "Error: " + t_node_base + " - " + VersionTool.GetBaseVersion() );
				
				return VersionTool.VersionType.None;
			}
			
			return VersionTool.VersionType.BigVersion;
		}
		else if( t_node_small != VersionTool.GetSmallVersion() ){
			#if DEBUG_BUILD
			Debug.Log( "Archive Small: " + t_node_small );
			
			Debug.Log( "Cur: " + VersionTool.GetSmallVersion() );
			#endif

			return VersionTool.VersionType.SmallVersion;
		}
		else if( t_node_small == VersionTool.GetSmallVersion() ){
			Debug.LogError( "Error: " + t_node_small + " - " + VersionTool.GetSmallVersion() );
			
			return VersionTool.VersionType.None;
		}
		else{
			Debug.LogError( "Error: Nothing To Build." );
			
			return VersionTool.VersionType.None;
		}
	}

	#endregion



	#region Build

	private static void BuildSmallVersion(){
		#if DEBUG_BUILD
		Debug.Log( "BuildSmallVersion()" );
		#endif

		{
			BuildBundle();
		}

		{
			BuildVersionDone();
		}
	}

	private static void BuildBundle(){
		#if DEBUG_BUILD
		Debug.Log( "BuildBundle()" );
		#endif

		#if UNITY_ANDROID
		Editor_Build_Bundle_Menu.Debug_Editor_Build_All_Android();
		#elif UNITY_IOS
		Editor_Build_Bundle_Menu.Debug_Editor_Build_All_iOS();
		#else
		Debug.LogError( "Error In Target." );
		
		return;
		#endif
	}

	private static void BuildBigVersion(){
		#if DEBUG_BUILD
		Debug.Log( "BuildBigVersion()" );
		#endif

		{
			CleanForBigBuild();
		}

		{
			BuildBundle();
		}

		{
			BuildVersionDone();
		}

		Debug.Log ( "Big Version Build, Bundles Built already, manual build project please." );

		return;

		#if UNITY_ANDROID
		BuildAndroid();
		#elif UNITY_IOS
		Debug.LogError( "Error, to Test iOS." );

		return;

		BuildiOS();
		#else
		Debug.LogError( "Error In Target." );
		
		return;
		#endif


	}

	private static void BuildVersionDone(){
		#if DEBUG_BUILD
		Debug.Log( "BuildVersionDone()" );
		#endif

		{
			#if UNITY_ANDROID
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetSmallVersion() + "/Android" );
			#elif UNITY_IOS
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetSmallVersion() + "/iOS" );
			#else
			string t_full_path = "";
			#endif

			#if DEBUG_BUILD
			Debug.Log( "target path: " + t_full_path );
			#endif

			#if UNITY_ANDROID
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingAssets/" + "Android" );
			#elif UNITY_IOS
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingAssets/" + "iOS" );
			#else
			string t_src_path = "";

			Debug.LogError( "Error In Target." );
			
			return;
			#endif

			#if DEBUG_BUILD
			Debug.Log( "src path: " + t_src_path );
			#endif

			FileHelper.DirectoryCopy( t_src_path, t_full_path );
		}

		{
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetSmallVersion() );
			
			string t_dest_path = Path.Combine( GetBuildTargetFolder(), VersionTool.GetSmallVersion() );

			FileHelper.DirectoryCopy( t_src_path, t_dest_path );
		}

		{
			AssetDatabase.Refresh();
		}
	}

	#endregion



	#region Utilities

	private static void CleanForBigBuild(){
		#if DEBUG_BUILD
		Debug.Log( "CleanForBigBuild()" );
		#endif

		// archive
		{
			CleanArchive();
		}

		// stream
		{
			CleanStream();
		}

		{
			AssetDatabase.Refresh();
		}
	}

	private static void CleanArchive(){
		#if DEBUG_BUILD
		Debug.Log( "CleanArchive()" );
		#endif

		string t_target = "StreamingArchived";

		string t_full_path = PathHelper.GetFullPath_WithRelativePath( t_target );
		
		DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
		
		if( !t_dir.Exists ){
			Debug.LogError( "Error, Archive Not Exist." );

			return;
		}

		DirectoryInfo[] t_dirs = t_dir.GetDirectories();
		
		for( int i = 0; i < t_dirs.Length; i++ ){
			DirectoryInfo t_sub_dir = t_dirs[ i ];

			string t_path = t_sub_dir.FullName;

			#if DEBUG_BUILD
			Debug.Log( "Delete: " + t_path );
			#endif

			Directory.Delete( t_path, true );

			File.Delete( t_path + ".meta" );
		}
	}

	private static void CleanStream(){
		#if DEBUG_BUILD
		Debug.Log( "CleanStream()" );
		#endif

		string[] t_targets = {
			"StreamingAssets/Android",
			"StreamingAssets/iOS"
		};

		for( int i = 0; i < t_targets.Length; i++ ){
			string t_target = t_targets[ i ];

			string t_full_path = PathHelper.GetFullPath_WithRelativePath( t_target );

			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );

			if( !t_dir.Exists ){
				continue;
			}

			string t_path = t_dir.FullName;

			#if DEBUG_BUILD
			Debug.Log( "Delete: " + t_path );
			#endif

			Directory.Delete( t_path, true );

			File.Delete( t_path + ".meta" );
		}
	}

	private static List<string> GetLevelPath(){
		List<string> t_level_names = new List<string>();
		
		foreach( UnityEditor.EditorBuildSettingsScene t_scene in UnityEditor.EditorBuildSettings.scenes ){
			if( t_scene.enabled ){
				t_level_names.Add( t_scene.path );
				
				Debug.Log( "Level: " + t_scene.path );
			}
		}
		
		return t_level_names;
	}

	private static void ExecBuild( BuildOptions p_options ){
		Debug.Log( "ExecBuild( " + p_options + " )" );

		VersionTool.Instance().Init();

		bool t_is_project = false;

		{
			int t_project = (int)BuildOptions.AcceptExternalModificationsToPlayer;

			int t_option = (int)p_options;

			if( ( t_option & t_project ) == t_project ){
				t_is_project = true;
			}

			Debug.Log( "Is Building Project: " + t_is_project );
		}
		
		string t_file_name = VersionTool.GetSmallVersion();

		if( !t_is_project ){
			#if UNITY_ANDROID
			t_file_name = t_file_name + ".apk";
			#elif UNITY_IOS
			t_file_name = t_file_name + ".ipa";
			#else
			Debug.LogError( "Error In Target." );
			
			return;
			#endif
		}
		
		string t_full_path = GetBuildTargetFolder() +
			"/" + t_file_name;

		{
			#if UNITY_IOS
			t_full_path = PathHelper.GetXCodeProjectFullPath();
			#endif
		}
		
		Debug.Log( "Build Path: " + t_full_path );

		#if UNITY_IOS
		BuildTarget t_target = BuildTarget.iOS;
		#elif UNITY_ANDROID
		BuildTarget t_target = BuildTarget.Android;
		#elif UNITY_STANDALONE
		BuildTarget t_target = BuildTarget.StandaloneWindows;
		#else
		BuildTarget t_target = BuildTarget.StandaloneWindows;
		#endif

		{
			string[] t_levels = GetLevelPath().ToArray();

//			foreach( string t_level in t_levels ){
//				Debug.Log( t_level );
//			}

//			string t_file_path = EditorUtility.SaveFilePanel( "Save", "", "SimpleBuilder", "" );

//			Debug.Log( "path: " + t_file_path );

			BuildPipeline.BuildPlayer( t_levels,
			                          t_full_path,
			                          t_target,
			                          p_options );
		}

	}

	private static string GetBuildTargetFolder(){
		return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ), VersionTool.GetBuildTime() );
	}

	#endregion
}
