#define DEBUG_BUNDLE_HELPER

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.12.3
 * @since:		Unity 5.1.3
 * Function:	Helper class for Build Bundles.
 * 
 * Notes:
 * 1.Created in Unity 5
 */ 
public class EditorBundleHelper{

	#region Build
	
	// Build All Bundles in Unity5.
	public static void BuildAll( string p_path ){
		if( string.IsNullOrEmpty( p_path ) ){
			Debug.LogError( "path is null or empty." );
			
			return;
		} 
		
		BuildPipeline.BuildAssetBundles( p_path,
		                                BuildAssetBundleOptions.None,
		                                GetBuildTarget() );
	}
	
	#endregion



	#region Logs
	
	/// Log all configged bundles
	public static void LogAllBundleConfigs(){
		string[] t_names = AssetDatabase.GetAllAssetBundleNames();
		foreach( string t_name in t_names ){
			Debug.Log( "Asset Bundle: " + t_name );
		}
	}

	#endregion


	
	#region Utilities
	
	/// Get Build Target in current os.
	public static BuildTarget GetBuildTarget(){
		#if UNITY_ANDROID
		return BuildTarget.Android;
		#elif UNITY_IOS
		return BuildTarget.iOS;
		#endif
		
		return BuildTarget.StandaloneWindows;
	}

	public static void CleanCachedBundles(){
		string t_full_target_path = PathHelper.GetFullPath_WithRelativePath( Editor_Build_Bundle_5.GetBuildTargetPath() );
		
		FileHelper.DeleteDirectoryAndCreate( t_full_target_path );
		
		EditorHelper.Refresh();
	}

	#endregion

}