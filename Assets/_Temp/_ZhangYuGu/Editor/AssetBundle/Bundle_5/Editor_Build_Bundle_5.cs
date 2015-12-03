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
	[MenuItem("Build/Bundles/Debug Bundle", false, (int)Editor_Build_Bundle_Menu.MenuItemPriority.BUILD_DEBUG ) ]
	public static void Debug_Editor_Debug_Bundle() {
		// Build All
		{
			Editor_Build_Bundle_5.BuildAll();
		}
		
		// Log All
//		{
//			BundleHelper.LogAllBundleConfigs();
//		}
	}
	#endif
	
	#endregion


	#region Build All

	/// Build All Bundles into destination folder.
	/// 
	/// Notes:
	/// 1.Folder MUST exist.
	public static void BuildAll(){
		string t_bundle_root_path = "Assets/StreamingAssets/" + PlatformHelper.GetAndroidTag();

		{
			BundleHelper.BuildAll( t_bundle_root_path );
		}

		{
			string t_src_path = Path.Combine( Application.dataPath, t_bundle_root_path.Substring( "Assets/".Length ) );

			string t_target_path = Path.Combine( Application.dataPath, "_Temp/_ZhangYuGu/_Debug/" + PlatformHelper.GetAndroidTag() );

			FileHelper.DirectoryCopy( t_src_path, t_target_path );
		}
	}

	#endregion




	#region Utilities


	#endregion
}
