using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

public class EditorHelper {

	#region Selection

	public static UnityEngine.Object[] GetSelectionObjects(){
		UnityEngine.Object[] t_objs = Selection.GetFiltered( typeof(UnityEngine.Object), SelectionMode.DeepAssets );

		return t_objs;
	}

	#endregion



	#region Object Type

	public static bool IsFolderObject( UnityEngine.Object p_object ){
		string t_path = GetAssetPath( (UnityEngine.Object)p_object );

		string t_folder_path = PathHelper.GetFullPath_WithRelativePath( t_path );

		return Directory.Exists( t_folder_path );
	}

	#endregion



	#region Path

	/// "Assets/Resources"
	public static string GetAssetResourcesPrefix(){
		return "Assets/Resources";
	}

	/// "Resources"
	public static string GetResourcesPrefix(){
		return "Resources";
	}

	/// "StreamingAssets"
	public static string GetStreamingPrefix(){
		return "StreamingAssets";
	}

	/// "StreamingArchived"
	public static string GetStreamingArchivedPrefix(){
		return "StreamingArchived";
	}

	/// return "Assets/MyTextures/hello.png"
	public static string GetAssetPath( UnityEngine.Object p_object ){
		return AssetDatabase.GetAssetPath( (UnityEngine.Object)p_object );
	}

	/// Convert
	/// "Assets/Resources/_Data/Design/Action.xml"
	/// to
	/// "_Data/Design/Action.xml"
	public static string RemoveAssetResourcesPrefix( string p_asset_path ){
		if( p_asset_path.IndexOf( GetAssetResourcesPrefix() ) != 0 ){
			Debug.LogWarning( "Warning, no such prefix exist: " + p_asset_path );

			return p_asset_path;
		}

		return p_asset_path.Substring( GetAssetResourcesPrefix().Length + 1 );
	}

	#endregion



	#region Log

	public static void LogSelectionObjects(){
		UnityEngine.Object[] t_objs = GetSelectionObjects();
		
		int t_asset_count = 1;
		
		for( int i = 0; i < t_objs.Length; i++ ){
			if( !IsFolderObject( t_objs[ i ] ) ){
				LogObject( t_objs[ i ], t_asset_count + " " );
				
				t_asset_count++;
			}
		}
	}

	public static void LogObject( UnityEngine.Object p_object, string p_prefix = "", string p_surfix = "" ){
		if( p_object == null ){
			Debug.Log( "Object is null." );

			return;
		}

		string t_path = GetAssetPath( (UnityEngine.Object)p_object );

		Debug.Log( p_prefix + " " + t_path + " " + p_surfix );
	}

	#endregion



	#region Utilities

	public static void Refresh(){
		AssetDatabase.Refresh();
	}

	#endregion
}
