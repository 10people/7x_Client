#define DEBUG_PLATFORM

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

		{
			EditorBuildAndroid3rd.ProcessFolder ( t_src_project_path, EditorBuildAndroid3rd.ANDROID_SRC_FOLDER_NAME );
		}

		{
			EditorBuildAndroid3rd.ProcessXG ();
		}
	}

	[MenuItem( EditorBuildAndroid3rd.BUILD_MENU_ANDROID_3RD_PREFIX + "My App Platform/Third Platform Project", false, 100)]
	private static void BuildMyAppProject(){
		string t_built_project_path = PathHelper.GetAndroidProjectFullPath();

		{
			OnProcessManifest( BuildTarget.Android, t_built_project_path );
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

		{
			BuildPlatform();
		}

		if ( true ) {
			#if DEBUG_PLATFORM
			Debug.Log( "keep manifest the same." );
			#endif

			return;
		}

		{
			OnProcessManifest( p_target, p_path_to_built_project );
		}
	}

	private static void OnProcessManifest( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnProcessPbx()" );
		#endif

		{
			FileHelper.FileBackUp( EditorBuildAndroid3rd.GetManifestPath() );
		}

		{


			FileHelper.FileCopy( GetMyAppManifestPath(), EditorBuildAndroid3rd.GetManifestPath() );
		}


		if( true ){
			return;
		}

		string t_pbx = EditorBuildiOS3rd.GetPBXContent( p_target, p_path_to_built_project );

		// backup
		{
			EditorBuildiOS3rd.BackUpPbx( p_path_to_built_project );
		}

		{
			// flag
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "OTHER_LDFLAGS = (\n" +
			                 	 	 "					\"-weak_framework\",\n" +
			                  		 "					CoreMotion,\n" +
			                  		 "					\"-weak-lSystem\",\n" + 
			                  		 "				);",
			                         
			                         "OTHER_LDFLAGS = (\n" +
			                         "\"-ObjC\",\n" +
			                         "\"-lz\",\n" +
			                         ");"
			                         );


			//Debug.Log( "Processed t_pbx: " + t_pbx );
		}


		
		{
			// save
			EditorBuildiOS3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}
	
	#endregion



	#region Utilities

	private static string GetMyAppManifestPath(){
		return "E:\\WorkSpace_Eclipse\\Build3rdProject\\Android\\MyApp\\AndroidManifest.xml";
	}

	#endregion
}
