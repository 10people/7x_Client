// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;
public class Editor_Doc_Export_Bundle {


	static void Trunk_1(){

	}

	static void Branch_2(){

	}

	static void Branch_3(){

	}

	#region Build Android Common

	//[MenuItem("Debug/Android/Build Selected Assets - Track dependencies")]
	static void ExportResource () {
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
		if (path.Length != 0) {
			// Build the resource file from the active selection.
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

			Debug.Log( "---streaming to: " + path );

			for( int i = 0; i < selection.Length; i++ ){
				Debug.Log( "Streaming " + i + ": " + selection[ i ] );
			}

			BuildPipeline.BuildAssetBundle(Selection.activeObject, 
			                               selection, 
			                               path, 
			                               BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
			                               BuildTarget.Android );

			Selection.objects = selection;
		}
	}

	#endregion





}