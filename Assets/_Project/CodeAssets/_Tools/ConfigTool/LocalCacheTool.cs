#define DEBUG_CONFIG

using System;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using System.Collections;
using System.Collections.Generic;



public class LocalCacheTool : ConfigHelper{

	private static LocalCacheTool m_instance = null;
	
	public static LocalCacheTool Instance(){
		if( m_instance == null ){
			m_instance = new LocalCacheTool();
		}
		
		return m_instance;
	} 

	public void LoadConfig(){
		LoadConfig( CONST_CONFIG_FILE_PATH );
	}
	
	#region File Path
	
	private const string CONST_CONFIG_FILE_PATH		= "_Data/Config/LocalCache";
	
	#endregion



	#region Const

	public const string CONST_VERSION				= "Version";

	public const string CONST_VERSION_CODE			= "VersionCode";

	#endregion
}