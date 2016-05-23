//#define OPEN_DEBUG_UTILITIES



using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using UnityEditor.Callbacks;
using System.Data;
using Mono.Data.Sqlite;
using System.Linq;  
using System.Text; 



[InitializeOnLoad]
public class EditorUtilityOpen : MonoBehaviour{

    public enum MenuItemPriority{
		OPEN_DRAMA_DIRECTOR = 100,
		OPEN_UI_2D,
		OPEN_WINDOW_2D,
		OPEN_LOAD_TASK,
		OPEN_FX_CACHE,
		OPEN_SPOT_VIEW,
		OPEN_FX_VIEW,
		OPEN_PROTO,
		OPEN_MODEL_ACTIVATOR,
		OPEN_OTHER,
    }

    static EditorUtilityOpen()
    {
        //		Debug.Log( "EditorUtilities()" );

        //		AssetDatabase.ImportAsset( "Assets/Resources/_Data/Design/BaiZhan.xml" );

        AssetDatabase.Refresh();
    }


	#region Open Menu

	#if OPEN_DEBUG_UTILITIES && UNITY_ANDROID

	[MenuItem("Utility/Open/Drama Director", false, (int)MenuItemPriority.OPEN_DRAMA_DIRECTOR)]
	static void OpenDramaDirector(){
		EditorWindow.GetWindow<DramaDirectorWindow>( false, "Drama Director", true );	
	}

	[MenuItem("Utility/Open/2D UI", false, (int)MenuItemPriority.OPEN_UI_2D)]
	static void OpenUI2DTool(){
		EditorWindow.GetWindow<UI2DWindow>( false, "2D UI", true );
	}

	[MenuItem("Utility/Open/Window UI", false, (int)MenuItemPriority.OPEN_WINDOW_2D)]
	static void OpenUIWindowTool(){
		EditorWindow.GetWindow<UIWindow>( false, "UI Window", true );
	}

	[MenuItem("Utility/Open/Load Task", false, (int)MenuItemPriority.OPEN_LOAD_TASK)]
	static void OpenUILoadTaskTool(){
		EditorWindow.GetWindow<UILoadTaskWindow>( false, "Load Task", true );
	}

	[MenuItem("Utility/Open/Fx Cache", false, (int)MenuItemPriority.OPEN_FX_CACHE)]
	static void OpenFxCacheWindowTool(){
		EditorWindow.GetWindow<FxCacheWindow>( false, "Fx Cache", true );
	}

	[MenuItem("Utility/Open/Fx View", false, (int)MenuItemPriority.OPEN_FX_VIEW)]
	static void OpenFxViewWindowTool(){
		EditorWindow.GetWindow<FxViewWindow>( false, "Fx View", true );
	}

	[MenuItem("Utility/Open/Proto View", false, (int)MenuItemPriority.OPEN_PROTO)]
	static void OpenViewProtoWindowTool(){
		EditorWindow.GetWindow<ViewProtoWindow>( false, "Proto View", true );
	}

	[MenuItem("Utility/Open/View Spot", false, (int)MenuItemPriority.OPEN_SPOT_VIEW)]
	static void OpenViewSpotWindowTool(){
		EditorWindow.GetWindow<ViewSpotWindow>( false, "View Spot", true );
	}


	[MenuItem("Utility/Open/Model Activator", false, (int)MenuItemPriority.OPEN_MODEL_ACTIVATOR)]
	static void OpenModelActivatorWindowTool(){
		EditorWindow.GetWindow<ModelActivatorWindow>( false, "Model Activator", true );
	}

	[MenuItem("Utility/Open/Open Other", false, (int)MenuItemPriority.OPEN_OTHER)]
	static void OpenOther(){
		Debug.Log( "MacHome: " + PathHelper.GetMacHome() );
	}

	[MenuItem("Utility/", false, (int)MenuItemPriority.OPEN_OTHER + 1)]
	static void OpenBreaker(){

	}
	#endif

	#endregion



	#region Utilities

	

	#endregion
}
