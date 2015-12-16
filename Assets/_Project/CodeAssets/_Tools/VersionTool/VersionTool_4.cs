//#define DEBUG_VERSION

using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.7.10
 * @since:		Unity 4.5.3
 * Function:	Manage Versions to Help Build.
 * 
 * Notes:
 * None.
 */ 
public class VersionTool_4 {

	public enum VersionType{
		None,
		SmallVersion,
		BigVersion,
	}

	/// Config values dict.
	public static Dictionary<string, ConfigTool.ConfigValue> m_version_value_dict = new Dictionary<string, ConfigTool.ConfigValue>();
	
	/// Config txt dict.
	private static Dictionary<string, string> m_version_dict = new Dictionary<string, string>();
	
	public const char CONST_LINE_SPLITTER		= '=';



	private static VersionTool_4 m_instance = null;

	public static VersionTool_4 Instance(){
		if( m_instance == null ){
			m_instance = new VersionTool_4();
		}

		return m_instance;
	}

	public VersionTool_4(){

	}



	#region Init

	public void Init(){
		#if DEBUG_VERSION
		Debug.Log( "VersionTool.Init()" );
		#endif

		{
			Clear();
		}

		#if UNITY_EDITOR
		InitEditorAsset();
		#else
		InitBuildAsset();
		#endif
	}

	private void Clear(){
		#if DEBUG_VERSION
		Debug.Log( "Clear()" );
		#endif

		m_version_value_dict.Clear();

		m_version_dict.Clear();
	}

	private void InitEditorAsset(){
		#if DEBUG_VERSION
		Debug.Log( "InitEditorAsset()" );
		#endif

		#if UNITY_EDITOR
		TextAsset t_text = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/Resources/_Data/Config/Version.txt", typeof(TextAsset) );

		LoadAsset( t_text );
		#endif
	}

	private void InitBuildAsset(){
		#if DEBUG_VERSION
		Debug.Log( "InitBuildAsset()" );
		#endif

		TextAsset t_text = (TextAsset)Resources.Load( "_Data/Config/Version" );

		LoadAsset( t_text );
	}

	private void LoadAsset( TextAsset p_text ){
		if (p_text == null) {
			Debug.LogError( "Error, p_text = null." );

			return;
		}

		#if DEBUG_VERSION
		Debug.Log( "LoadAsset( " + p_text.text + " )" );
		#endif

		{
			UtilityTool.LoadStringStringDict( m_version_dict, p_text, CONST_LINE_SPLITTER );
		}

		{
			LoadVersionItems();
		}
	}

	private void LoadVersionItems(){
		ConfigTool.LoadValues( m_version_value_dict, CONST_BUILD_TIME, ConfigTool.LoadStringValue( m_version_dict, CONST_BUILD_TIME ) );

		ConfigTool.LoadValues( m_version_value_dict, CONST_SMALL_VERSION, ConfigTool.LoadStringValue( m_version_dict, CONST_SMALL_VERSION ) );

		ConfigTool.LoadValues( m_version_value_dict, CONST_BIG_VERSION, ConfigTool.LoadStringValue( m_version_dict, CONST_BIG_VERSION ) );

		ConfigTool.LoadValues( m_version_value_dict, CONST_BASE_VERSION, ConfigTool.LoadStringValue( m_version_dict, CONST_BASE_VERSION ) );

		#if DEBUG_VERSION
		foreach( KeyValuePair<string, ConfigTool.ConfigValue> t_kv in m_version_value_dict ){
			Debug.Log( t_kv.Key + " = " + t_kv.Value.ValueToString() );
		}
		#endif
	}

	#endregion

	
	
	#region Versions
	
	
	/// NOTICE:
	/// All small version build should update this value, archived files must be under 20XX_XXXX_XXXX\Platform.
	/// All big version MUST DELETE ALL Archived bundles, only leave the newest.
	/// 
	/// Version Changes:
	/// 1.Update Version Time To 0617-1725;
	public const string CONFIG_SMALL_VERSION_NEWEST		= "2015_0813_1000";
	
	/// Version Changes:
	//	public const string CONFIG_SMALL_VERSION_NEWEST		= "2015_0707_2116";
	
	//	public const string CONFIG_SMALL_VERSION_NEWEST		= "2015_0707_2101";
	
	//	public const string CONFIG_SMALL_VERSION_NEWEST		= "2015_0707_2013";
	
	
	
	/// NOTICE:
	/// Update this each big version build.
	/// 
	/// Version Changes:
	/// 1.2nd Big Version for test use.
	public const string CONFIG_BIG_VESION			= "0.99.0813.1000";
	
	// Big Version Update.
	//	public const string CONFIG_BIG_VESION			= "0.97.0704.1727";
	
	/// Init.
	//	public const string CONFIG_BIG_VESION		= "0.98.0704.1455";
	
	
	
	/// NOTICE:
	/// Update this when each big version build.
	public const string CONFIG_BASE_VERSION			= "2015_0813_1000";
	
	/// Big Version Update.
	//	public const string CONFIG_BASE_VERSION			= "2015_0704_1727";
	
	/// Init.
	//	public const string CONFIG_BASE_VERSION			= "2015_0707_1936";


	public static string GetBuildTime(){
		return UtilityTool.GetString( m_version_value_dict, CONST_BUILD_TIME, "0000-0000" );
	}
	
	public static string GetSmallVersion(){
		return UtilityTool.GetString( m_version_value_dict, CONST_SMALL_VERSION, "2000_0000_0000" );
	}
	
	public static string GetBigVersion(){
		return UtilityTool.GetString( m_version_value_dict, CONST_BIG_VERSION, "1.0.0000.0000" );
	}
	
	public static string GetBaseVersion(){
		return UtilityTool.GetString( m_version_value_dict, CONST_BASE_VERSION, "0000_0000" );;
	}
	
	#endregion



	#region Const Keys

	public const string CONST_BUILD_TIME		= "BuildTime";

	public const string CONST_SMALL_VERSION		= "SmallVersion";

	public const string CONST_BIG_VERSION		= "BigVersion";

	public const string CONST_BASE_VERSION		= "BaseVersion";

	#endregion
}
