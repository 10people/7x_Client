

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class PathHelper : MonoBehaviour {

	#region Mac

	/// return "/Users/dastan"
	public static string GetMacHome(){

		#if UNITY_EDITOR
		string t_process = Application.dataPath;

		int t_dst_index = -1;

		{
			t_process = t_process.Substring( 1 );

			t_process = t_process.Substring( t_process.IndexOf( '/' ) + 1 );

			t_process = t_process.Substring( t_process.IndexOf( '/' ) );
		}

		string t_target_path = Application.dataPath.Substring( 0, Application.dataPath.IndexOf( t_process ) );

		return t_target_path;

		#endif

		Debug.LogError( "Only Should be called under Editor." );

		return "";
	} 

	private const string XcodeProjectRelativePath = "/workspace/xcode_workspace/xcode_qixiong";

	public static string GetXCodeProjectFullPath(){
		return PathHelper.GetMacHome() + XcodeProjectRelativePath;
	}

	#endregion
}
