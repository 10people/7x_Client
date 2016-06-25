//#define DEBUG_CONFIG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.5.21
 * @since:		Unity 5.1.3
 * Function:	Manage property file.
 * 
 * Notes:
 */ 
public class PropertyTool : Singleton<PropertyTool>{
	
	/// Config txt dict.
	private static Dictionary<string, string> m_property_dict = new Dictionary<string, string>();
	
	public const char CONST_LINE_SPLITTER		= '=';



	#region Mono

	void Awake()
    {
	}

	void Start(){

	}

	void OnDestroy(){
		CleanData();

		base.OnDestroy();
	}

	#endregion



	#region Load

	void CleanData(){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigTool.CleanData()" );
		#endif

		m_property_dict.Clear();
	}
	
	public void LoadConfigs( EventDelegate.Callback p_callback = null ){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigTool.LoadConfigs()" );
		#endif

		Global.ResourcesDotLoad( CONST_CONFIG_FILE_PATH, ResourceLoadCallback, UtilityTool.GetEventDelegateList( p_callback ) );
	}
	
	public void ResourceLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigTool.ResourceLoadCallback( " + ((TextAsset)p_object).text + " )" );
		#endif

		if ( m_property_dict.Count > 0 ) {
			return;
		}

		{
			TextAsset t_text = ( TextAsset )p_object;

			UtilityTool.LoadStringStringDict( m_property_dict, t_text, CONST_LINE_SPLITTER );
		}

//		foreach( KeyValuePair<string, string> t_pair in m_property_dict ){
//			Debug.Log( "Property.( " + t_pair.Key + " - " + t_pair.Value + " )" );
//		}
	}

	#endregion



	#region Get

	public static bool GetBool( string p_key, bool p_default_value = false ){
		return PropertyHelper.GetBool( m_property_dict, p_key, p_default_value );
	}

	public static int GetInt( string p_key, int p_default_value = 0 ){
		return PropertyHelper.GetInt( m_property_dict, p_key, p_default_value );
	}

	public static float GetFloat( string p_key, float p_default_value = 0f ){
		return PropertyHelper.GetFloat( m_property_dict, p_key, p_default_value );
	}

	public static string GetString( string p_key, string p_default_value = "" ){
		return PropertyHelper.GetString( m_property_dict, p_key, p_default_value );
	}

	#endregion



	#region Utilities

	#endregion


	
	#region File Path
	
	private const string CONST_CONFIG_FILE_PATH		= "_Data/Config/Property";
	
	#endregion




	#region Version Keys

	public const string CONST_SHOW_VERSION			= "ShowVersion";

	public const string CONST_VERSION_CODE			= "VersionCode";

	public const string CONST_FLOAT_TEST			= "FloatTest";

	#endregion

}
