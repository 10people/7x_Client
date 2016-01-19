//#define DEBUG_THIRD_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorBuildAndroid3rd {

	#region Common

	public const string BUILD_MENU_ANDROID_3RD_PREFIX		= "Build/Third Platform/Android/";

	public const string BUILD_MENU_SETTING_ANDROID_PREFIX	= "Build/Settings/Android/";

	#endregion



	#region Project Config

	public const string ANDROID_SRC_FOLDER_NAME		= "src";

	#endregion



	#region XG

	public const string XG_PROJECT_PATH		 		= "E:\\WorkSpace_Eclipse\\Build3rdProject\\Android\\XG";

	public const string XG_LIB_FOLDER_NAME 			= "libs";

	public static void ProcessXG(){
		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "ProcessXG()" );
		#endif

		ProcessFolder( XG_PROJECT_PATH, XG_LIB_FOLDER_NAME );

		AssetDatabase.Refresh();
	}

	#endregion



//	#region GameGoo
//
//	[MenuItem( BUILD_MENU_SETTING_ANDROID_PREFIX + "GameGoo", false, 1)]
//	public static void BuildSettingsMyApp(){
//		EditorBuild3rd.BuildSettings( "_Android/GameGoo" );
//	}
//
//	#endregion


	
	#region Build 3rd

	[PostProcessBuild(1000)]
	public static void OnThirdPlatformPostBuild( BuildTarget p_target, string p_path_to_built_project ){
//		if ( true ) {
//			Debug.Log( "3rd platform not working." );
//
//			return;
//		}

		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "OnThirdPlatformPostBuild( " + p_path_to_built_project + " )" );

		Debug.Log( "PlatformType: " + ThirdPlatform.GetPlatformType() );
		#endif

		#if UNITY_ANDROID
		switch ( ThirdPlatform.GetPlatformType() ) {
		case ThirdPlatform.PlatformType.MyApp_Android_Platform:
			EditorMyApp_Android.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;
			
		default:
//			Debug.Log ( "Default Platform Build." );
			break;
		}
		#endif
	}

	#endregion



	#region Path

	public static string GetDesManifestPath(){
		string t_path = PathHelper.GetAndroidProjectFullPath();

		string t_manifest_path = Path.Combine( t_path, "AndroidManifest.xml" );

		return t_manifest_path;
	}

	public static string GetDesProjectPath(){
		string t_path = PathHelper.GetAndroidProjectFullPath();
		
		string t_desc_path = Path.Combine( t_path, ".project" );
		
		return t_desc_path;
	}
	
	public static string GetDesClassPath(){
		string t_path = PathHelper.GetAndroidProjectFullPath();
		
		string t_desc_path = Path.Combine( t_path, ".classpath" );
		
		return t_desc_path;
	}

	public static string GetDesProjectPropertyPath(){
		string t_path = PathHelper.GetAndroidProjectFullPath();
		
		string t_desc_path = Path.Combine( t_path, "project.properties" );
		
		return t_desc_path;
	}

	#endregion



	#region Utilities

	public static void ProcessFile( string p_src_project_path, string p_relative_path ){
		string t_des = Path.Combine( PathHelper.GetAndroidProjectFullPath(), p_relative_path );
		
		string t_src = Path.Combine( p_src_project_path, p_relative_path );
		
		if( File.Exists ( t_des ) ) {
//			#if DEBUG_THIRD_PLATFORM
//			Debug.Log ( "Delete: " + t_des );
//			#endif
			
			File.Delete( t_des );
		}
		
//		#if DEBUG_THIRD_PLATFORM
//		Debug.Log ( "Copy: " + t_src );
//		
//		Debug.Log ( "To: " + t_des );
//		#endif
		
		File.Copy( t_src, t_des );
	}

	public static void ProcessFolder( string p_src_project_path, string p_relative_path ){
//		#if DEBUG_THIRD_PLATFORM
//		Debug.Log( "ProcessFolder( " + p_relative_path + " )" ) ;
//		#endif

		string t_src = Path.Combine ( p_src_project_path, p_relative_path );

		string t_des = Path.Combine ( PathHelper.GetAndroidProjectFullPath(), p_relative_path );
		
//		#if DEBUG_THIRD_PLATFORM
//		Debug.Log ( "Copy: " + t_src );
//		
//		Debug.Log ( "To: " + t_des );
//		#endif
		
		FileHelper.DirectoryCopy( t_src, t_des );
	}

	#endregion




}
