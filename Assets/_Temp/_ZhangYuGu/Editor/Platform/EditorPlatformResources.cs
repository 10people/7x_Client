#define DEBUG_RESOURCES


using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorPlatformResources : MonoBehaviour {

	#region Common
	
	public const string BUILD_MENU_PLATFORM_RESOURCES_PREFIX		= "Build/Resources/";

	#endregion



	#region Mono

	#endregion


	#region Build Android Resources

	/// Replace with iOS Resources.
	[MenuItem( BUILD_MENU_PLATFORM_RESOURCES_PREFIX + "Use iOS Resources", false, 1 )]
	public static void BuildiOSResources(){
		#if DEBUG_RESOURCES
		Debug.Log( "EditorPlatformResources.BuildiOSResources()" );
		#endif
		
		ExecResources( IOS_CACHED_RESOURCES_PATH );
	}

	#endregion



	#region Build iOS Resources

	/// Replace with Android Resources.
	[MenuItem( BUILD_MENU_PLATFORM_RESOURCES_PREFIX + "Use Android Resources", false, 1 )]
	public static void BuildAndroidResources(){
		#if DEBUG_RESOURCES
		Debug.Log( "EditorPlatformResources.BuildAndroidResources()" );
		#endif
		
		ExecResources( ANDROID_CACHED_RESOURCES_PATH );
	}

	#endregion



	#region Utilities

	// use this folder as Resources
	private const string ANDROID_CACHED_RESOURCES_PATH	= "ResourcesCache/Android";

	// use this folder as Resources
	private const string IOS_CACHED_RESOURCES_PATH		= "ResourcesCache/iOS";

	// final used resources
	private const string TARGET_RESOURCES_PATH			= "Resources";

	private static void ExecResources( string p_source_resources_path ){
		#if DEBUG_RESOURCES
		Debug.Log( "ExecResources( " + p_source_resources_path + " )" );
		#endif

		string t_full_src_path = PathHelper.GetFullPath_WithRelativePath( p_source_resources_path );

		string t_full_dst_path = PathHelper.GetFullPath_WithRelativePath( TARGET_RESOURCES_PATH );

		#if DEBUG_RESOURCES

		Debug.Log( "Src Path: " + t_full_src_path );

		Debug.Log( "Dst Path: " + t_full_dst_path );

		#endif

		// process
		{
			FileHelper.DirectoryCopy( t_full_src_path, t_full_dst_path );
		}

		{
			AssetDatabase.Refresh();
		}
	}

	#endregion


}
