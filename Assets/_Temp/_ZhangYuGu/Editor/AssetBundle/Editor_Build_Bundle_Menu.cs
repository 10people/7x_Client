using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.3
 * @since:		Unity 4.5.3
 * Function:	Add Unity's MenuItems.
 * 
 * Notes:
 * None.
 */ 
public class Editor_Build_Bundle_Menu : MonoBehaviour {

	public enum MenuItemPriority{
		BUILD_APK = 0,
		BUILD_APK_SEPERATOR,
		BUILD_ALL,
		BUILD_MANIFEST,

		BUILD_DEBUG = 100,

		BUILD_SUB_UI_ATLAS_PREFABS,
		BUILD_SUB_UI_IMAGES,
		BUILD_SUB_DATA,

		BUILD_SUB_SOUND,
		BUILD_SUB_3D_Models,
		BUILD_SUB_FX,
		BUILD_SUB_TEMP_FOLDERS,
		BUILD_SUB_SCENES,

		BUILD_SUB_CONFIG,
		BUILD_SUB_BUNDLE_LIST,
	}
	
	#region Build All

	#if UNITY_ANDROID
	[MenuItem("Build/Bundles/Build All 4/Android", false, (int)MenuItemPriority.BUILD_ALL ) ]
	public static void Debug_Editor_Build_All_Android() {
		Editor_Build_Bundle.Build_All( BuildTarget.Android );
	}
	#endif

//	#if UNITY_ANDROID || UNITY_STANDALONE
//	[MenuItem("Build/Bundles/Build All 4/Windows", false, (int)MenuItemPriority.BUILD_ALL )]
//	static void Debug_Editor_Build_All_Windows() {
//		Editor_Build_Bundle.Build_All( BuildTarget.StandaloneWindows );
//	}
//	#endif

	#if UNITY_IOS
	[MenuItem("Build/Bundles/Build All 4/iOS", false, (int)MenuItemPriority.BUILD_ALL )]
	public static void Debug_Editor_Build_All_iOS() {
		Editor_Build_Bundle.Build_All( BuildTarget.iOS );
	}
	#endif

	#endregion



	#region Build Sub/Build UI Atlases&Prefabs
		
	[MenuItem("Build/Bundles/Build Sub/Build UI Atlases n Prefabs/Android", false, (int)MenuItemPriority.BUILD_SUB_UI_ATLAS_PREFABS )]
	static void Debug_Editor_Build_UI_Atlases_Prefabs_Android() {
		Editor_Build_Bundle.Build_UI_Atlas_Prefab_Bundles( BuildTarget.Android );
	}

	[MenuItem("Build/Bundles/Build Sub/Build UI Atlases n Prefabs/Windows", false, (int)MenuItemPriority.BUILD_SUB_UI_ATLAS_PREFABS )]
	static void Debug_Editor_Build_UI_Atlases_Prefabs_Windows() {
		Editor_Build_Bundle.Build_UI_Atlas_Prefab_Bundles( BuildTarget.StandaloneWindows );
	}

	[MenuItem("Build/Bundles/Build Sub/Build UI Atlases n Prefabs/iOS", false, (int)MenuItemPriority.BUILD_SUB_UI_ATLAS_PREFABS )]
	static void Debug_Editor_Build_UI_Atlases_Prefabs_iOS() {
		Editor_Build_Bundle.Build_UI_Atlas_Prefab_Bundles( BuildTarget.iOS );
	}

	#endregion



	#region Build Sub/Build UI Images

	[MenuItem("Build/Bundles/Build Sub/Build UI Images/Android", false, (int)MenuItemPriority.BUILD_SUB_UI_IMAGES )]
	static void Debug_Editor_Build_UI_Images_Android() {
		Editor_Build_Bundle.Build_UI_Image_Bundles( BuildTarget.Android );
	}

	[MenuItem("Build/Bundles/Build Sub/Build UI Images/Windows", false, (int)MenuItemPriority.BUILD_SUB_UI_IMAGES )]
	static void Debug_Editor_Build_UI_Images_Windows() {
		Editor_Build_Bundle.Build_UI_Image_Bundles( BuildTarget.StandaloneWindows );
	}

	[MenuItem("Build/Bundles/Build Sub/Build UI Images/iOS", false, (int)MenuItemPriority.BUILD_SUB_UI_IMAGES )]
	static void Debug_Editor_Build_UI_Images_iOS() {
		Editor_Build_Bundle.Build_UI_Image_Bundles( BuildTarget.iOS );
	}

	#endregion



//	#region Build Sub/Build Unfiled Images
//
//	[MenuItem("Build/Bundles/Build Sub/Build Unfiled Images/Android", false, (int)MenuItemPriority.BUILD_SUB_UNFILED_IMAGES )]
//	static void Debug_Editor_Build_Unfiled_Images_Android() {
//		Editor_Build_Bundle.Build_Unfiled_Images( BuildTarget.Android );
//	}
//
//	[MenuItem("Build/Bundles/Build Sub/Build Unfiled Images/Windows", false, (int)MenuItemPriority.BUILD_SUB_UNFILED_IMAGES )]
//	static void Debug_Editor_Build_Unfiled_Images_Windows() {
//		Editor_Build_Bundle.Build_Unfiled_Images( BuildTarget.StandaloneWindows );
//	}
//
//	[MenuItem("Build/Bundles/Build Sub/Build Unfiled Images/iOS", false, (int)MenuItemPriority.BUILD_SUB_UNFILED_IMAGES )]
//	static void Debug_Editor_Build_Unfiled_Images_iOS() {
//		Editor_Build_Bundle.Build_Unfiled_Images( BuildTarget.iPhone );
//	}
//
//	#endregion



	#region Build Sub/Build Data
	
	[MenuItem("Build/Bundles/Build Sub/Build Data/Android", false, (int)MenuItemPriority.BUILD_SUB_DATA )]
	static void Debug_Editor_Build_Data_Android() {
		Editor_Build_Bundle.Build_Data( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Data/Windows", false, (int)MenuItemPriority.BUILD_SUB_DATA )]
	static void Debug_Editor_Build_Data_Windows() {
		Editor_Build_Bundle.Build_Data( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Data/iOS", false, (int)MenuItemPriority.BUILD_SUB_DATA )]
	static void Debug_Editor_Build_Data_iOS() {
		Editor_Build_Bundle.Build_Data( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build Sounds
	
	[MenuItem("Build/Bundles/Build Sub/Build Sound/Android", false, (int)MenuItemPriority.BUILD_SUB_SOUND )]
	static void Debug_Editor_Build_Sounds_Android() {
		Editor_Build_Bundle.Build_Sounds( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Sound/Windows", false, (int)MenuItemPriority.BUILD_SUB_SOUND )]
	static void Debug_Editor_Build_Sounds_Windows() {
		Editor_Build_Bundle.Build_Sounds( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Sound/iOS", false, (int)MenuItemPriority.BUILD_SUB_SOUND )]
	static void Debug_Editor_Build_Sounds_iOS() {
		Editor_Build_Bundle.Build_Sounds( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build 3D Models Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build 3D Models Bundle/Android", false, (int)MenuItemPriority.BUILD_SUB_3D_Models )]
	static void Debug_Editor_Build_3D_Models_Bundle_Android() {
		Editor_Build_Bundle.Build_3D_Models( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build 3D Models Bundle/Windows", false, (int)MenuItemPriority.BUILD_SUB_3D_Models )]
	static void Debug_Editor_Build_3D_Models_Bundle_Windows() {
		Editor_Build_Bundle.Build_3D_Models( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build 3D Models Bundle/iOS", false, (int)MenuItemPriority.BUILD_SUB_3D_Models )]
	static void Debug_Editor_Build_3D_Models_Bundle_iOS() {
		Editor_Build_Bundle.Build_3D_Models( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build fx Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build Fx Bundle/Android", false, (int)MenuItemPriority.BUILD_SUB_FX )]
	static void Debug_Editor_Build_Fx_Bundle_Android() {
		Editor_Build_Bundle.Build_Fx( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Fx Bundle/Windows", false, (int)MenuItemPriority.BUILD_SUB_FX )]
	static void Debug_Editor_Build_Fx_Bundle_Windows() {
		Editor_Build_Bundle.Build_Fx( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Fx Bundle/iOS", false, (int)MenuItemPriority.BUILD_SUB_FX )]
	static void Debug_Editor_Build_Fx_Bundle_iOS() {
		Editor_Build_Bundle.Build_Fx( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build New Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build New Bundle/Android", false, (int)MenuItemPriority.BUILD_SUB_TEMP_FOLDERS )]
	static void Debug_Editor_Build_Temp_Folders_Bundle_Android() {
		Editor_Build_Bundle.Build_Temp_Folders( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build New Bundle/Windows", false, (int)MenuItemPriority.BUILD_SUB_TEMP_FOLDERS )]
	static void Debug_Editor_Build_Temp_Folders_Bundle_Windows() {
		Editor_Build_Bundle.Build_Temp_Folders( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build New Bundle/iOS", false, (int)MenuItemPriority.BUILD_SUB_TEMP_FOLDERS )]
	static void Debug_Editor_Build_Temp_Folders_Bundle_iOS() {
		Editor_Build_Bundle.Build_Temp_Folders( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build Scenes Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build Scenes Bundle/Android", false, (int)MenuItemPriority.BUILD_SUB_SCENES )]
	static void Debug_Editor_Build_Scenes_Bundle_Android() {
		Editor_Build_Bundle.Build_Scenes( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Scenes Bundle/Windows", false, (int)MenuItemPriority.BUILD_SUB_SCENES )]
	static void Debug_Editor_Build_Scenes_Bundle_Windows() {
		Editor_Build_Bundle.Build_Scenes( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Scenes Bundle/iOS", false, (int)MenuItemPriority.BUILD_SUB_SCENES )]
	static void Debug_Editor_Build_Scenes_Bundle_iOS() {
		Editor_Build_Bundle.Build_Scenes( BuildTarget.iOS );
	}
	
	#endregion


	
	#region Build Sub/Build Configs Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build Configs Bundle/Android", false, (int)MenuItemPriority.BUILD_SUB_CONFIG )]
	static void Debug_Editor_Build_Configs_Bundle_Android() {
		Editor_Build_Bundle.Build_Configs( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Configs Bundle/Windows", false, (int)MenuItemPriority.BUILD_SUB_CONFIG )]
	static void Debug_Editor_Build_Configs_Bundle_Windows() {
		Editor_Build_Bundle.Build_Configs( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Configs Bundle/iOS", false, (int)MenuItemPriority.BUILD_SUB_CONFIG )]
	static void Debug_Editor_Build_Configs_Bundle_iOS() {
		Editor_Build_Bundle.Build_Configs( BuildTarget.iOS );
	}
	
	#endregion



	#region Build Sub/Build Configs Bundle
	
	[MenuItem("Build/Bundles/Build Sub/Build Bundle List/Android", false, (int)MenuItemPriority.BUILD_SUB_BUNDLE_LIST )]
	static void Debug_Editor_Build_Bundle_List_Android() {
		Editor_Build_Bundle.Rebuild_Bundle_List( BuildTarget.Android );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Bundle List/Windows", false, (int)MenuItemPriority.BUILD_SUB_BUNDLE_LIST )]
	static void Debug_Editor_Build_Bundle_List_Windows() {
		Editor_Build_Bundle.Rebuild_Bundle_List( BuildTarget.StandaloneWindows );
	}
	
	[MenuItem("Build/Bundles/Build Sub/Build Bundle List/iOS", false, (int)MenuItemPriority.BUILD_SUB_BUNDLE_LIST )]
	static void Debug_Editor_Build_Bundle_List_iOS() {
		Editor_Build_Bundle.Rebuild_Bundle_List( BuildTarget.iOS );
	}
	
	#endregion
}
