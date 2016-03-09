#define DEBUG_BUNDLE

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using SimpleJSON;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.11.30
 * @since:		Unity 5.1.3
 * Function:	Build bundles in Unity 5.
 * 
 * Notes:
 * 1.Must be executed in Unity 5.
 */ 
public class Editor_Build_Bundle_5 {

	#region Debug Bundle
	
	#if DEBUG_BUNDLE
	/// 1st.
	[MenuItem("Build/Bundles/Build All", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_DEBUG - 2 ) ]
	public static void Debug_Editor_Build_All() {
		{
			EditorBundleHelper.CleanCachedBundles();
		}

		// Build All
		{
			Editor_Build_Bundle_5.BuildAll();
		}

		{
			EditorBuildUpdate.BuildUpdate( GetBuildTargetPath() );
		}
	}

	/// 1st.
	[MenuItem("Build/Bundles/Build Bundle", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_DEBUG - 1 ) ]
	public static void Debug_Editor_Debug_Bundle() {
		{
			EditorBundleHelper.CleanCachedBundles();
		}
		
		// Log All
//		{
//			EditorBundleHelper.LogAllBundleConfigs();
//		}

		// Selections
		{
			BuildSelections();
		}

		{
			EditorBuildUpdate.BuildUpdate( GetBuildTargetPath() );
		}
	}

	/// 2nd.
	[MenuItem("Build/Bundles/Build Manifest", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_DEBUG ) ]
	public static void Debug_Editor_Debug_Bundle_2nd() {
		{
			EditorBuildManifest.BuildManifest( GetBuildTargetPath() );
		}

		{
			EditorBuildUpdate.BuildUpdate( GetBuildTargetPath() );
		}

		{
			BuildUpdateDone();
		}
	}
	#endif
	
	#endregion


	#region Build All

	/// Build All Bundles into destination folder.
	/// 
	/// Notes:
	/// 1.Folder MUST exist.
	public static void BuildAll(){
		{
			EditorBundleHelper.BuildAll( GetBuildTargetPath() );
		}

//		{
//			string t_src_path = Path.Combine( Application.dataPath, GetBuildTargetPath().Substring( "Assets/".Length ) );
//
//			string t_target_path = Path.Combine( Application.dataPath, "_Temp/_ZhangYuGu/_Debug/" + PlatformHelper.GetAndroidTag() );
//
//			FileHelper.DirectoryCopy( t_src_path, t_target_path );
//		}

		{
			EditorHelper.Refresh();
		}
	}

	#endregion



	#region Build Selection

	#if DEBUG_BUNDLE

	public static void BuildSelections(){
		UnityEngine.Object[] t_objs = EditorHelper.GetSelectionObjects();

		List<AssetBundleBuild> t_build_list = new List<AssetBundleBuild>();

		int t_asset_count = 1;

		for( int i = 0; i < t_objs.Length; i++ ){
			if( EditorHelper.IsFolderObject( t_objs[ i ] ) ){
				continue;
			}

//			{
//				EditorHelper.LogObject( t_objs[ i ], t_asset_count + " " );
//				
//				t_asset_count++;
//			}

			{
				AssetBundleBuild t_build = new AssetBundleBuild();

				t_build.assetBundleName = StringHelper.RemoveExtension( EditorHelper.GetAssetPath( t_objs[ i ] ) );

				t_build.assetNames = new string[]{ EditorHelper.GetAssetPath( t_objs[ i ] ) };

//				{
//					Debug.Log( i + " bundle name: " + t_build.assetBundleName );
//
//					Debug.Log( i + " asset name: " + t_build.assetNames[ 0 ] );
//				}

				t_build_list.Add( t_build );
			}
		}

		BuildPipeline.BuildAssetBundles( GetBuildTargetPath(), 
		                                t_build_list.ToArray(),
		                                BuildAssetBundleOptions.None,
		                                EditorBundleHelper.GetBuildTarget() );

		EditorHelper.Refresh();
	}

	#endif

	#endregion



	#region Utilities

	private static void BuildUpdateDone(){
		#if DEBUG_BUNDLE
		Debug.Log( "BuildUpdateDone()" );
		#endif

		{
			VersionTool.Instance().Init();
		}
		
		{
			#if UNITY_ANDROID
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetPackageSmallVersion() + "/Android" );
			#elif UNITY_IOS
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetPackageSmallVersion() + "/iOS" );
			#else
			string t_full_path = "";
			#endif
			
			#if DEBUG_BUNDLE
			Debug.Log( "target path: " + t_full_path );
			#endif
			
			#if UNITY_ANDROID
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + "Android" );
			#elif UNITY_IOS
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + "iOS" );
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
			string t_src_path = PathHelper.GetFullPath_WithRelativePath( "StreamingArchived/" + VersionTool.GetPackageSmallVersion() );
			
//			string t_dest_path = Path.Combine( GetBuildTargetFolder(), VersionTool.GetPackageSmallVersion() );
			string t_dest_path = GetBuildTargetFolder();
			
			FileHelper.DirectoryCopy( t_src_path, t_dest_path );
		}
		
		{
			AssetDatabase.Refresh();
		}
	}

	private static string GetBuildTargetFolder(){
		return Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Desktop ), VersionTool.GetPackageSmallVersion() );
	}

	/// "Assets/StreamingArchived/" + PlatformHelper.GetPlatformTag()
	public static string GetBuildTargetPath(){
		return "Assets/StreamingArchived/" + PlatformHelper.GetPlatformTag();
	}

	#endregion
}
