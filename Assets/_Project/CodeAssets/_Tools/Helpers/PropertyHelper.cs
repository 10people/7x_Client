using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.4.29
 * @since:		Unity 5.1.3
 * Function:	Load properties.
 * 
 * 
 * 
 * Notes:
 * 
 * 
 * 
 * Example:
 * target=android-23
 * android.library.reference.1=../unity-android-resources
 */
public class PropertyHelper {

	#region Load

	private const char PROPERTY_SPLITTER				= '=';

	public static Dictionary<string, string> LoadProperty( string p_full_property_path, bool p_log = false ){
		string t_content = FileHelper.ReadString( p_full_property_path );

		Dictionary<string, string> t_dict = new Dictionary<string, string>();

		UtilityTool.LoadStringStringDict( t_dict, t_content, PROPERTY_SPLITTER );

		if( p_log ){
			int t_index = 0;

			foreach( KeyValuePair<string, string> t_kv in t_dict ){
				Debug.Log( t_index + " : " + t_kv.Key + " = " + t_kv.Value );

				t_index++;
			}
		}

		return t_dict;
	}

	#endregion



	#region Get

	public static bool GetBool( Dictionary<string, string> p_dict, string p_key, bool p_default_value = false ){
		if( !p_dict.ContainsKey( p_key ) ){
			Debug.Log( "Key not contained: " + p_key );

			return p_default_value; 
		}

		string t_str = p_dict[ p_key ];

		try{
			return bool.Parse( t_str );
		}
		catch( Exception t_e ){
			Debug.LogError( "Exception: " + t_e );

			return p_default_value;
		}
	}

	public static int GetInt( Dictionary<string, string> p_dict, string p_key, int p_default_value = 0 ){
		if( !p_dict.ContainsKey( p_key ) ){
			Debug.Log( "Key not contained: " + p_key );

			return p_default_value; 
		}

		string t_str = p_dict[ p_key ];

		try{
			return int.Parse( t_str );
		}
		catch( Exception t_e ){
			Debug.LogError( "Exception: " + t_e );

			return p_default_value;
		}
	}

	public static float GetFloat( Dictionary<string, string> p_dict, string p_key, float p_default_value = 0f ){
		if( !p_dict.ContainsKey( p_key ) ){
			Debug.Log( "Key not contained: " + p_key );

			return p_default_value; 
		}

		string t_str = p_dict[ p_key ];

		try{
			return float.Parse( t_str );
		}
		catch( Exception t_e ){
			Debug.LogError( "Exception: " + t_e );

			return p_default_value;
		}
	}

	public static string GetString( Dictionary<string, string> p_dict, string p_key, string p_default_value = "" ){
		if( !p_dict.ContainsKey( p_key ) ){
			Debug.Log( "Key not contained: " + p_key );

			return p_default_value; 
		}

		return p_dict[ p_key ];
	}

	#endregion
}
