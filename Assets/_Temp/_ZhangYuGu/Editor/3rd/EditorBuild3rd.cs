

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorBuild3rd {

	#region Settings
	
	public static string GetSettingsPath_WithRelativePath( string p_res_relative_path )
	{
		// check first '/'
		if (!p_res_relative_path.StartsWith("/"))
		{
			p_res_relative_path = "/" + p_res_relative_path;
		}
		
		return Application.dataPath.Substring( 0, Application.dataPath.Length - "/Assets".Length ) + p_res_relative_path;
	}
	
	public static void BuildSettings( string p_platform_desc = "KuaiYong" ){
		string t_src = GetSettingsPath_WithRelativePath ("ProjectSettings/3rd/" + p_platform_desc + "/ProjectSettings.asset");
		
		string t_des = GetSettingsPath_WithRelativePath ( "ProjectSettings/ProjectSettings.asset" );
		
		FileHelper.FileCopy ( t_src,
		                     t_des );

		AssetDatabase.Refresh();
	}
		
	#endregion



}

