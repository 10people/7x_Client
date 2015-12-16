//#define DEBUG_UPDATE

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using SimpleJSON;

public class EditorBuildUpdate {

	#region Build Update

	private static JSONClass m_all_bundle_json = new JSONClass();

	public static void BuildUpdate( string p_relative_path ){
		{
			Clean( p_relative_path );
		}

		{
			BuildInfo();
		}

		{
			BuildFile( p_relative_path );
		}
	}

	private static void BuildInfo(){
		// update info
		{
			VersionTool.Instance().Init();

			m_all_bundle_json[ VersionTool.CONST_SMALL_VERSION ] = VersionTool.GetPackageSmallVersion();
			
			m_all_bundle_json[ VersionTool.CONST_BIG_VERSION ] = VersionTool.GetPackageBigVersion();
		}
	}

	private static void BuildFile( string p_relative_path ){
		// write file
		{
			string t_path = PathHelper.GetFullPath_WithRelativePath( GetUpdatePath( p_relative_path ) );

			#if DEBUG_UPDATE
			Debug.Log( "BuildFile( " + t_path + " )" );

			Debug.Log( "Update Content: " + m_all_bundle_json.ToString( "" ) );
			#endif


			FileHelper.OutputFile( t_path, m_all_bundle_json.ToString( "" ) );
		}
		
		// refresh
		{
			AssetDatabase.Refresh();
		}
	}

	#endregion



	#region Clean

	private static void Clean( string p_relative_path ){
		{
			ClearAllBundleJSON();
		}

		{
			string t_path = PathHelper.GetFullPath_WithRelativePath( GetUpdatePath( p_relative_path ) );

			FileHelper.FileDelete( t_path );
		}
	}

	private static void ClearAllBundleJSON(){
		m_all_bundle_json = new JSONClass();
	}

	#endregion



	#region Utilities

	private static string GetUpdatePath( string p_relative_path ){
		return p_relative_path + "/" + ManifestHelper.COSNT_UPDATE_FILE_NAME;
	}

	#endregion



	#region Const



	#endregion
}
