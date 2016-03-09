//#define DEBUG_CONFIG



using System;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
using System.Collections;
using System.Collections.Generic;



public class ConfigHelper{
	private Dictionary<string, string> m_config_dict = new Dictionary<string, string>();
	
	public const char CONST_LINE_SPLITTER		= ':';

	protected void LoadConfig( string p_res_path ){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigHelper.LoadConfig( " + p_res_path  + " )" );
		#endif

		Global.ResourcesDotLoad( p_res_path, ResourceLoadCallback );
	}
	
	protected void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigHelper.ResourceLoadCallback( " + ((TextAsset)p_object).text + " )" );
		#endif

		if ( m_config_dict.Count > 0 ) {
			return;
		}

		{
			TextAsset t_text = ( TextAsset )p_object;

			UtilityTool.LoadStringStringDict( m_config_dict, t_text, CONST_LINE_SPLITTER );
		}
	}
	
	public string GetStringValue( string p_key ){
		if( m_config_dict.ContainsKey( p_key ) ){
			return m_config_dict[ p_key ];
		}
		else{
			return "";
		}
	}
	
	// default false
	public bool GetBoolValue( string p_key ){
		if( m_config_dict.ContainsKey( p_key ) ){
			return bool.Parse( m_config_dict[ p_key ] );
		}
		else{
			return false;
		}
	}
}