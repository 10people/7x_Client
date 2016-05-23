//#define DEBUG_BUILD

//#define DELETE_CACHE_BAT



//#define OPEN_DEVELOPMENT_BUILD



using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.4.x
 * @since:		Unity 5.1.3
 * Function:	Sice Payment built-in, android use this to build.
 * 
 * Notes:
 * None.
 */
public class EditorBuildAndroid : MonoBehaviour {



	#region Mono

	/** Path: E:/WorkSpace_Eclipse/qixiong
	 */
	[PostProcessBuild(1000)]
	public static void OnThirdPlatformPostBuild( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log( "OnThirdPlatformPostBuild( " + p_target + " - " + p_path_to_built_project + " )" );
		#endif

		if( p_path_to_built_project.EndsWith( ".apk" ) ){
			#if DEBUG_BUILD
			UnityEngine.Debug.Log( "Build apk, return." );
			#endif

			return;
		}
		else{
			SetBuiltProjectPath( p_path_to_built_project );

			m_android_build_properties = PropertyHelper.LoadProperty( GetAndroidPropertyPath(), false );
		}

		#if UNITY_ANDROID
		switch ( ThirdPlatform.GetPlatformType() ) {
		case ThirdPlatform.PlatformType.MyApp_Android_Platform:
//			Debug.Log ( "MyApp Platform Build." );

			UpdateProject();
			break;

		default:
//			Debug.Log ( "Default Platform Build." );

			UpdateProject();
			break;
		}
		#endif
	}

	#endregion



	#region Build

	[MenuItem( "Build/Apk for Released", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_APK )]
	public static void BuildApkForRelease(){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log( "BuildApkForRelease()" );
		#endif
		
		{
			m_android_build_properties = PropertyHelper.LoadProperty( GetAndroidPropertyPath(), false );
		}

		{
			CleanCacheProject();
		}

		{
			#if OPEN_DEVELOPMENT_BUILD
			BuildOptions t_option = BuildOptions.AcceptExternalModificationsToPlayer | BuildOptions.Development;
			#else
			BuildOptions t_option = BuildOptions.AcceptExternalModificationsToPlayer;
			#endif

			string t_full_path = GetCacheProjectPath();

//			UnityEngine.Debug.Log( "Build Path: " + t_full_path );

			UnityEditor.BuildTarget t_target = BuildTarget.Android;

			string[] t_levels = GetLevelPath().ToArray();

//			{
//				for( int i = 0; i < t_levels.Length; i++ ){
//					UnityEngine.Debug.Log( i + " : " + t_levels[ i ] );
//				}
//			}

			BuildPipeline.BuildPlayer( t_levels,
				t_full_path,
				t_target,
				t_option );
		}

		{
			ProcessApk();
		}

		{
			CheckIfCleanCache();
		}
	}

	[MenuItem( "Build/", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_APK_SEPERATOR )]
	public static void BuildApkReleaseSeperator(){

	}

	#endregion



	#region Update Project

	private static void UpdateProject(){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log( "UpdateProject()" );
		#endif

		{
			ProcessFolder ( GetMyAppRootPath(), "src" );
		}

		{
			ProcessFolder ( GetMyAppRootPath(), "assets" );
		}

		{
			ProcessProject( GetBuiltProjectPath() );
		}
	}

	public static void ProcessFolder( string p_src_project_path, string p_relative_path ){
		//		#if DEBUG_THIRD_PLATFORM
		//		Debug.Log( "ProcessFolder( " + p_relative_path + " )" ) ;
		//		#endif

		string t_src = Path.Combine ( p_src_project_path, p_relative_path );

		string t_des = Path.Combine ( Path.Combine( GetBuiltProjectPath(), PlayerSettings.productName ), p_relative_path );

//		string t_des = Path.Combine ( "E:\\WorkSpace_Eclipse\\qixiong\\七雄无双", p_relative_path );

//		Debug.Log ( "Copy: " + t_src );
//		
//		Debug.Log ( "To: " + t_des );

		FileHelper.DirectoryCopy( t_src, t_des );
	}


	private static void ProcessProject( string p_path_to_built_project ){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log ( "ProcessProject.OnProcessManifest()" );
		#endif

		{
			FileHelper.FileBackUp( GetDesManifestPath() );

			{
				FileHelper.FileCopy( GetMyAppManifestPath(), GetDesManifestPath() );

				//FileHelper.FileCopy( GetMyAppProjectPath(), EditorBuildAndroid3rd.GetDesProjectPath() );

				//FileHelper.FileCopy( GetMyAppProjectPropertyPath(), EditorBuildAndroid3rd.GetDesProjectPropertyPath() );

				//FileHelper.FileCopy( GetMyAppClassPath(), EditorBuildAndroid3rd.GetDesClassPath() );
			}
		}
	}

	#endregion



	#region Process Apk

	private static void ProcessApk(){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log( "ProcessApk()" );
		#endif

		{
			ExecuteInWindows( 
				"call android update project -n " + PlayerSettings.productName + " -p " + GetMainAndroidProject(), 
				GetCacheFileRelativePath( "1.bat" ) );
		}

		{
			ExecuteInWindows( 
				"android update lib-project -p " + GetBuiltLibraryPath(), 
				GetCacheFileRelativePath( "2.bat" ) );
		}

		{
			FileHelper.FileCopy( GetMyAppBuildPath(), GetDesBuildPath() );

//			FileHelper.FileCopy( GetMyAppPropertiesPath(), GetDesPropertiesPath() );

			FileHelper.FileCopy( GetAndroidPropertyPath(), GetDesPropertiesPath() );
		}

		{
			FileHelper.DirectoryCreate( GetBuiltLibrarySrcPath() );
		}

		{
			ExecuteInWindows( 
				"call ant -f " + GetBuildFilePath() + " clean", 
				GetCacheFileRelativePath( "3.bat" ) );
		}

		{
			ExecuteInWindows( 
				"call ant -f " + GetBuiltLibraryBuildFilePath() + " clean", 
				GetCacheFileRelativePath( "4.bat" ) );
		}

		{
			ExecuteInWindows( 
				"call ant -f " + GetBuildFilePath() + " release", 
				GetCacheFileRelativePath( "5.bat" ) );
		}

		{
			FileHelper.FileCopy( GetBuiltApkPath(), PathHelper.GetBuiltApkPath() );

			#if DEBUG_BUILD
			UnityEngine.Debug.Log( "Src Apk: " + GetBuiltApkPath() );

			UnityEngine.Debug.Log( "Dst Apk: " + PathHelper.GetBuiltApkPath() );
			#endif
		}
	}

	#endregion



	#region Clean Cache

	private static void CheckIfCleanCache(){
		if( !bool.Parse( m_android_build_properties[ CLEAN_CACHE ] ) ){
			return;
		}

		CleanCacheProject();
	}

	private static void CleanCacheProject(){
		DirectoryInfo t_dir = new DirectoryInfo( GetCacheProjectPath() );

		if( t_dir.Exists ){
//			UnityEngine.Debug.Log( "Delete Cache Folder: " + t_dir.FullName );

			t_dir.Delete( true );
		}
	}

	#endregion



	#region Path

	private static string GetMyAppRootPath(){
		return m_android_build_properties[ MY_APP_ROOT_PATH_KEY ];
	}

	private static string GetAndroidPropertyPath(){
		return Application.dataPath + "/Build/Android/AndroidBuild.properties";
	}

	private static string GetMyAppManifestPath(){
		return Path.Combine( GetMyAppRootPath(), "AndroidManifest.xml" );
	}

	public static string GetDesManifestPath(){
		return Path.Combine( GetMainAndroidProject(), "AndroidManifest.xml" );
	}

	private static string GetMyAppBuildPath(){
		return Application.dataPath + "/Build/Android/MyAppbuild.xml";
	}

	public static string GetDesBuildPath(){
		return Path.Combine( GetMainAndroidProject(), "build.xml" );
	}

	private static string GetMyAppPropertiesPath(){
		return Path.Combine( GetMyAppRootPath(), "custom.properties" );
	}

	public static string GetDesPropertiesPath(){
		return Path.Combine( GetMainAndroidProject(), "custom.properties" );
	}

	public static string GetBuiltApkPath(){
		string t_path = Path.Combine( GetMainAndroidProject(), "bin" );

		t_path = Path.Combine( t_path, PlayerSettings.productName + "-release.apk" );

		return t_path;
	}

	/** Path: "E:\\WorkSpace_Eclipse\\qixiong\\七雄无双"
	 */
	public static string GetMainAndroidProject(){
		return Path.Combine( GetBuiltProjectPath(), PlayerSettings.productName );
	}

	/** Path: "E:\WorkSpace_Eclipse\qixiong\七雄无双\bin\七雄无双-release.apk"
	 */
	public static string GetFinalApkPath(){
		return Path.Combine( GetMainAndroidProject(), "bin\\" + PlayerSettings.productName + "-release.apk" );
	}

	public static string GetOutputApkPath(){
		return "";
	}

	/** 
	 * D:/AndroidProject
	 */
	private static string GetCacheProjectPath(){
		return "D:/AndroidProject";
	}

	private static List<string> GetLevelPath(){
		List<string> t_level_names = new List<string>();

		foreach( UnityEditor.EditorBuildSettingsScene t_scene in UnityEditor.EditorBuildSettings.scenes ){
			if( t_scene.enabled ){
				t_level_names.Add( t_scene.path );

//				UnityEngine.Debug.Log( "Level: " + t_scene.path );
			}
		}

		return t_level_names;
	}

	private static string GetTempBatchPath(){
		return Path.Combine( Application.dataPath, "_temp.bat" );
	}

	public static string GetBuildFilePath(){
		return Path.Combine( GetMainAndroidProject(), "build.xml" );
	}

	public static string GetBuiltLibraryBuildFilePath(){
		return Path.Combine( GetBuiltLibraryPath(), "build.xml" );
	}

	public static string GetBuiltLibraryPath(){
		return Path.Combine( GetBuiltProjectPath(), "unity-android-resources" );
	}

	public static string GetBuiltLibrarySrcPath(){
		return Path.Combine( GetBuiltLibraryPath(), "src" );
	}

	/** Path: E:/WorkSpace_Eclipse/qixiong
	 */
	private static string m_built_project_path = "E:/WorkSpace_Eclipse/qixiong";

	/** Path: E:/WorkSpace_Eclipse/qixiong
	 */
	public static string GetBuiltProjectPath(){
		if( string.IsNullOrEmpty( m_built_project_path ) ){
			UnityEngine.Debug.LogError( "Error, built project path is not setted." );
		}

		return m_built_project_path;
	}

	private static void SetBuiltProjectPath( string p_built_project_path ){
		m_built_project_path = p_built_project_path;
	}

	#endregion



	#region Utilities

	/**
	 * Build\Android\1.bat
	 */ 
	private static string GetCacheFileRelativePath( string p_file_name ){
		return Path.Combine( "_Temp/_ZhangYuGu/_Cache", p_file_name );
	}

	/** p_file_relative_path = "Resources/abc.bat"
	 */
	private static void ExecuteInWindows( string p_content, string p_file_relative_path = "" ){
		string t_file_full_path = "";

		if( string.IsNullOrEmpty( p_file_relative_path ) ){
			t_file_full_path = GetTempBatchPath();
		}
		else{
			t_file_full_path = Path.Combine( Application.dataPath, p_file_relative_path );
		}

		{
			BuildTempBatch( t_file_full_path, p_content );
		}

		try {
			Process t_process = Process.Start( t_file_full_path );

			t_process.WaitForExit();

			int t_exit_code = t_process.ExitCode;

			//			UnityEngine.Debug.Log( "ExitCode: " + t_exit_code );
		} catch (Exception e){
			UnityEngine.Debug.Log( e );        
		}

		#if DELETE_CACHE_BAT
		{
			FileHelper.FileDelete( t_file_full_path );
		}
		#endif
	}

	private static void BuildTempBatch( string p_path, string p_text ){
		#if DEBUG_BUILD
		UnityEngine.Debug.Log( "BuildTempBatch: " + p_path );

		UnityEngine.Debug.Log( "Content: " + p_text );
		#endif

		FileHelper.WriteFile( p_path, p_text );
	}

	#endregion



	#region Properties

	private static Dictionary<string, string> m_android_build_properties = new Dictionary<string, string>();

	private const string MY_APP_ROOT_PATH_KEY		= "android.myapp";

	private const string CLEAN_CACHE				= "clean.cache";

	#endregion
}
