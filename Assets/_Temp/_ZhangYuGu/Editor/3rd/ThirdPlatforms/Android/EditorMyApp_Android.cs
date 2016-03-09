//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class EditorMyApp_Android : MonoBehaviour {

	#region Config

	private const string Configured_MYAPP_Project = "E:\\WorkSpace_Eclipse\\Build3rdProject\\Android\\MyApp";

	#endregion



	#region My App

	[MenuItem( EditorBuildAndroid3rd.BUILD_MENU_ANDROID_3RD_PREFIX + "My App Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_src_project_path = Configured_MYAPP_Project;

//		{
//			EditorBuildAndroid3rd.ProcessXG ();
//		}

		{
			EditorBuildAndroid3rd.ProcessFolder ( t_src_project_path, EditorBuildAndroid3rd.ANDROID_SRC_FOLDER_NAME );
		}
	}

	[MenuItem( EditorBuildAndroid3rd.BUILD_MENU_ANDROID_3RD_PREFIX + "My App Platform/Third Platform Project", false, 100)]
	private static void BuildMyAppProject(){
		string t_built_project_path = PathHelper.GetAndroidProjectFullPath();

		{
			OnProcessProject( BuildTarget.Android, t_built_project_path );
		}
	}
	
	[MenuItem( EditorBuildAndroid3rd.BUILD_MENU_SETTING_ANDROID_PREFIX + "MyApp", false, 1)]
	public static void BuildSettingsMyApp(){
		EditorBuild3rd.BuildSettings( "_Android/MyApp" );
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnPostBuildPlatform()" );
		#endif

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep project the same." );
//			#endif
//
//			return;
//		}

		{
			BuildPlatform();
		}

		{
			OnProcessProject( p_target, p_path_to_built_project );
		}
	}

	private static void OnProcessProject( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnProcessManifest()" );
		#endif

		{
			FileHelper.FileBackUp( EditorBuildAndroid3rd.GetDesManifestPath() );

			{
				FileHelper.FileCopy( GetMyAppManifestPath(), EditorBuildAndroid3rd.GetDesManifestPath() );
				
				FileHelper.FileCopy( GetMyAppProjectPath(), EditorBuildAndroid3rd.GetDesProjectPath() );

				FileHelper.FileCopy( GetMyAppProjectPropertyPath(), EditorBuildAndroid3rd.GetDesProjectPropertyPath() );
				
				FileHelper.FileCopy( GetMyAppClassPath(), EditorBuildAndroid3rd.GetDesClassPath() );
			}
		}
	}
	
	#endregion



	#region Path

	private static string GetMyAppRootPath(){
		return "E:\\WorkSpace_Eclipse\\Build3rdProject\\Android\\MyApp";
	}

	private static string GetMyAppManifestPath(){
		return Path.Combine( GetMyAppRootPath(), "AndroidManifest.xml" );
	}

	public static string GetMyAppProjectPath(){
		string t_path = GetMyAppRootPath();
		
		string t_desc_path = Path.Combine( t_path, ".project" );
		
		return t_desc_path;
	}

	public static string GetMyAppClassPath(){
		string t_path = GetMyAppRootPath();
		
		string t_desc_path = Path.Combine( t_path, ".classpath" );
		
		return t_desc_path;
	}

	public static string GetMyAppProjectPropertyPath(){
		string t_path = GetMyAppRootPath();
		
		string t_desc_path = Path.Combine( t_path, "project.properties" );
		
		return t_desc_path;
	}

	#endregion



	#region Utilities


	#endregion
}
