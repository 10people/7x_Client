//#define LOG_QUALITY

//#define DEBUG_QUALITY

//#define ENABLE_BLOOM

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.5.12
 * @since:		Unity 4.5.3
 * Function:	Manage Quality for different devices.
 * 
 * Notes:
 * 1. All Quality Policy must be here.
 */ 
public class QualityTool : Singleton<QualityTool>{

	public enum QualityLevel{
		Low = 0,
		Medium,
		High,
	}

	/// Quality values dict.
	public static Dictionary<string, ConfigTool.ConfigValue> m_quality_dict = new Dictionary<string, ConfigTool.ConfigValue>();

	/// Config txt dict.
	public static Dictionary<string, string> m_config_xml_dict = new Dictionary<string, string>();


	public const char CONST_SECTION_SPLITTER		= '=';

	public const char CONST_DEVICE_SPLITTER		= '/';


	#region Mono

	void OnDestroy(){
		CleanQualityData();

		DeviceHelper.CleanDeviceData();
	}

	#endregion



	#region Load

	public void LoadQualities( EventDelegate.Callback p_callback = null ){
		Global.ResourcesDotLoad( CONST_DEVICE_FILE_PATH, DeviceHelper.DeviceLoadCallback, null );

		Global.ResourcesDotLoad( CONST_QUALITY_FILE_PATH, QualityLoadCallback, UtilityTool.GetEventDelegateList( p_callback ) );
	}

	private void CleanQualityData(){
		m_quality_dict.Clear();
		
		m_config_xml_dict.Clear();
	}
	
	public void QualityLoadCallback( ref WWW p_www, string p_path, Object p_object ){
//		Debug.Log( "QualityTool.ResourceLoadCallback()" );

//		if( true ){
//			return;
//		}
		
		if (m_quality_dict.Count > 0 && m_config_xml_dict.Count > 0) {
			return;
		}

		// load dict
		{
			LoadConfigItems( p_object );
		}

		// Update Other Devices
		{
			UpdateQualityForOtherDevices();
		}

		// make easy dict
		{
			LoadQualityItems();
		}

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			LogQualityItems();
		}
		else{
			#if LOG_QUALITY
			LogQualityItems();
			#endif
		}

		{
			ExecQualityItems();
		}
	}

	#endregion



	#region Load Detail

	private void LoadConfigItems( Object p_object ){
//		Debug.Log( "QualityTool.LoadQualityItems()" );

		{
			TextAsset t_text = ( TextAsset )p_object;
			
			string[] t_lines = t_text.text.Split( '\n' );

			bool t_skip_to_next_section = false;

			bool t_device_found = false;

			bool t_quality_found = false;

			string t_cur_section_key = "";

			string t_cur_section_value = "";

			for( int i = 0; i < t_lines.Length; i++ ){
				string t_line = t_lines[ i ];

				string[] t_item_pair = null;
				
				string[] t_section_pair = null;

				t_section_pair = t_line.Split( CONST_SECTION_SPLITTER );

				if( t_section_pair.Length == 2 ){
					// section found

					{
						t_skip_to_next_section = false;
					}

					t_cur_section_key = t_section_pair[ 0 ].Trim();

					t_cur_section_value = t_section_pair[ 1 ].Trim();

					if( t_cur_section_key == CONST_DEVICE_SECTION ){
						// device check

						if( !DeviceHelper.FindCurrentDevice( t_cur_section_value, t_device_found, true ) ){
							t_skip_to_next_section = true;

							continue;
						}
						else{
							t_device_found = true;
						}
					}
					else if( t_cur_section_key == CONST_GRAPHICS_SECTION ){
						// gdn check

						if( !DeviceHelper.FindCurrentGDN( t_cur_section_value ) ){
							t_skip_to_next_section = true;
							
							continue;
						}
					}
					else if( t_cur_section_key == CONST_QUALITY_SECTION ){
						// fill quality

						if( m_config_xml_dict[ CONST_DEFAULT_QUALITY ] != t_cur_section_value ){
							t_skip_to_next_section = true;
							
							continue;
						}
						else{
							t_quality_found = true;
						}
					}


					// next line
					continue;
				}

				if( t_skip_to_next_section ){
					continue;
				}

				{
					t_item_pair = t_line.Split( ConfigTool.CONST_LINE_SPLITTER );
					
					if( t_item_pair.Length != 2 ){
						// nothing
						
						continue;
					}
					
					// item pair
					{
						string t_item_key = t_item_pair[ 0 ].Trim();

						string t_item_value = t_item_pair[ 1 ].Trim();

						if( !m_config_xml_dict.ContainsKey( t_item_key ) ){
//							Debug.Log( "Add Item: " + t_item_key + " - " + t_item_value );
							
							m_config_xml_dict.Add( t_item_key, t_item_value );
						}
						else if( OverrideQualityItems( t_cur_section_key, t_item_key, t_cur_section_value, t_item_value ) ){
							m_config_xml_dict[ t_item_key ] = t_item_value;
						}
					}
				}
			}

			if( !t_device_found ){
				Debug.LogError( "Error, Device Not Found." );
			}

			if( !t_quality_found ){
				Debug.LogError( "Error, Quality Not Found." );
			}
		}
	}

	private bool OverrideQualityItems( string p_section_key, string p_item_key, string p_section_value = "", string p_item_value = "" ){
		if( p_section_key == CONST_GRAPHICS_SECTION ){
			if( p_item_key == CONST_DEFAULT_QUALITY ){
				if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
					Debug.Log( "OverrideQualityItems: " + p_section_key + " - " + p_section_value + "   " + 
					          p_item_key + " - " + p_item_value );
				}

				return true;
			}
		}

		return false;
	}

	private void LoadQualityItems(){
		ConfigTool.LoadValues( m_quality_dict, CONST_DEFAULT_QUALITY, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_DEFAULT_QUALITY ) );
		
		ConfigTool.LoadValues( m_quality_dict, CONST_DEVICE_TAG, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_DEVICE_TAG ) );

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			#if UNITY_ANDROID
			Debug.Log( "Mobile.Device.Model: " + SystemInfo.deviceModel );
			#endif
			
			#if UNITY_IOS
			Debug.Log( "Mobile.Device.Gen: " + UnityEngine.iOS.Device.generation.ToString() );
			#endif

			Debug.Log( "Mobile.Device.G.Name: " + SystemInfo.graphicsDeviceName );
		}
		
		ConfigTool.LoadValues( m_quality_dict, CONST_IN_CITY_SHADOW, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_IN_CITY_SHADOW ) );
		
		ConfigTool.LoadValues( m_quality_dict, CONST_BATTLE_FIELD_SHADOW, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_BATTLE_FIELD_SHADOW ) );
		
		ConfigTool.LoadValues( m_quality_dict, CONST_BLADE_EFFECT, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_BLADE_EFFECT ) );
		
		ConfigTool.LoadValues( m_quality_dict, CONST_BOSS_EFFECT, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_BOSS_EFFECT ) );

		ConfigTool.LoadValues( m_quality_dict, CONST_AA, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_AA ) );

		ConfigTool.LoadValues( m_quality_dict, CONST_BLOOM, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_BLOOM ) );

		ConfigTool.LoadValues( m_quality_dict, CONST_CHARACTER_HITTED_FX, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_CHARACTER_HITTED_FX ) );

		ConfigTool.LoadValues( m_quality_dict, CONST_SCNE_FX_LEVEL, ConfigTool.LoadStringValue( m_config_xml_dict, CONST_SCNE_FX_LEVEL ) );
	}

	private static void LogQualityItem( string p_quality_item ){
		Debug.Log( p_quality_item + ": " + ValueToString( p_quality_item ) );
	}

	public static void LogQualityItems(){
		LogQualityItem( CONST_DEVICE_TAG );

		LogQualityItem( CONST_DEFAULT_QUALITY );



		LogQualityItem( CONST_IN_CITY_SHADOW );
		
		LogQualityItem( CONST_BATTLE_FIELD_SHADOW );
		
		LogQualityItem( CONST_BLADE_EFFECT );
		
		LogQualityItem( CONST_BOSS_EFFECT );

		LogQualityItem( CONST_AA );

		LogQualityItem( CONST_BLOOM );

		LogQualityItem( CONST_CHARACTER_HITTED_FX );

		LogQualityItem( CONST_SCNE_FX_LEVEL );
	}

	private void ExecQualityItems(){
		// quality level
		{
			string t_quality_set = GetString( CONST_DEFAULT_QUALITY );
			if( t_quality_set == CONST_QUALITY_SECTION_LOW ){
				SetQualityLevel( QualityLevel.Low );
			}

			if( t_quality_set == CONST_QUALITY_SECTION_MEDIUM ){
				SetQualityLevel( QualityLevel.Medium );
			}

			if( t_quality_set == CONST_QUALITY_SECTION_HIGH ){
				SetQualityLevel( QualityLevel.High );
			}
		}

		// AA
		{
			#if UNITY_IOS
			Quality_Common.ConfigAA();
			#endif
		}

		{
			Quality_SceneFx.LoadSceneFxLevel( GetString( QualityTool.CONST_SCNE_FX_LEVEL ) );
		}
	}

	#endregion



	#region Update Other Devices

	private void UpdateQualityForOtherDevices(){
//		Debug.Log( "UpdateQualityForOtherDevices()" );
		
		// set to lowest for lowest device
		{
			CheckLowest();
		}
		
		// set to highest for highest device
		{
			CheckHighest();
		}
	}

	private void CheckLowest(){
		#if UNITY_IOS
		{
			bool t_set_to_lowest = false;
			
			if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || 
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen || 
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ){
				t_set_to_lowest = true;
			}
			
			if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone || 
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G || 
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS ||
			   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 ){
				t_set_to_lowest = true;
			}
			
			if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad1Gen ){
				t_set_to_lowest = true;
			}
			
			if( t_set_to_lowest ){
				SetToLowest();
			}
		}
		#endif
		
		#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
		{
			// Needn't to Find Lowest, Android Is Always Lowest.



			// try to find lowest
			
//			bool t_set_to_lowest = true;
//			
//			if( t_set_to_lowest ){
//				SetToLowest();
//			}
		}
		#endif
	}

	private void CheckHighest(){
		#if UNITY_IOS
		{
			bool t_set_to_highest = false;

			if( m_config_xml_dict[ CONST_DEVICE_TAG ] == CONST_DEVICES_IOS_OTHERS ){
				t_set_to_highest = true;
			}

			if( t_set_to_highest ){
				SetToHighest();
			}
		}
		#endif
		
		#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
		{
			// Needn't to Find Highest, gdn will.

			// try to find highest
			
//			bool t_set_to_highest = false;
//			
//			if( t_set_to_highest ){
//				SetToHighest();
//			}
		}
		#endif
	}

	/// Lowest quality for all platform.
	private void SetToLowest(){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			Debug.Log( "SetToLowest()" );
		}

		m_config_xml_dict[ CONST_IN_CITY_SHADOW ] = Quality_Shadow.SHADOW_NONE;
		
		m_config_xml_dict[ CONST_BATTLE_FIELD_SHADOW ] = Quality_Shadow.SHADOW_NONE;
		
		m_config_xml_dict[ CONST_BLADE_EFFECT ] = "false";
		
		m_config_xml_dict[ CONST_BOSS_EFFECT ] = "true";

		m_config_xml_dict[ CONST_AA ] = "None";

		m_config_xml_dict[ CONST_BLOOM ] = "false";

		m_config_xml_dict[ CONST_CHARACTER_HITTED_FX ] = "false";

		m_config_xml_dict[ CONST_SCNE_FX_LEVEL ] = Quality_SceneFx.CONST_SCENE_FX_LEVEL_NONE;
	}

	/// Highest quality for all platform.
	private void SetToHighest(){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			Debug.Log( "SetToHighest()" );
		}

		m_config_xml_dict[ CONST_IN_CITY_SHADOW ] = Quality_Shadow.SHADOW_HIGH;
		
		m_config_xml_dict[ CONST_BATTLE_FIELD_SHADOW ] = Quality_Shadow.SHADOW_HIGH;
		
		m_config_xml_dict[ CONST_BLADE_EFFECT ] = "true";
		
		m_config_xml_dict[ CONST_BOSS_EFFECT ] = "true";

		m_config_xml_dict[ CONST_AA ] = "Low";

		m_config_xml_dict[ CONST_BLOOM ] = "true";

		m_config_xml_dict[ CONST_CHARACTER_HITTED_FX ] = "true";

		m_config_xml_dict[ CONST_SCNE_FX_LEVEL ] = Quality_SceneFx.CONST_SCENE_FX_LEVEL_HIGH;
	}

	#endregion



	#region Quality Level

	private static QualityLevel m_quality_level = QualityLevel.Low;

	public static QualityLevel GetQualityLevel(){
		return m_quality_level;
	}

	public static void SetQualityLevel( QualityLevel p_quality_level ){
		m_quality_level = p_quality_level;
	}

	public bool IsLowQuality(){
		return m_quality_level == QualityLevel.Low;
	}
	
	public bool IsMediumQuality(){
		return m_quality_level == QualityLevel.Medium;
	}
	
	public bool IsHighQuality(){
		return m_quality_level == QualityLevel.High;
	}
	
	#endregion



	#region Get Config Value
	
	public static bool GetBool( string p_key, bool p_default_value = false ){
		return UtilityTool.GetBool( m_quality_dict, p_key, p_default_value );
	}
	
	public static int GetInt( string p_key, int p_default_value = 0 ){
		return UtilityTool.GetInt( m_quality_dict, p_key, p_default_value );
	}
	
	public static float GetFloat( string p_key, float p_default_value = 0f ){
		return UtilityTool.GetFloat( m_quality_dict, p_key, p_default_value );
	}
	
	public static string GetString( string p_key, string p_default_value = "" ){
		return UtilityTool.GetString( m_quality_dict, p_key, p_default_value );
	}
	
	public static string ValueToString( string p_key, string p_default_value = "" ){
		return UtilityTool.ValueToString( m_quality_dict, p_key, p_default_value );
	}
	
	#endregion



	#region File Path

	#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
	private const string CONST_DEVICE_FILE_PATH			= "_Data/Config/AndroidDevice";
	#elif UNITY_IOS
	private const string CONST_DEVICE_FILE_PATH			= "_Data/Config/iOSDevice";
	#endif

	private const string CONST_QUALITY_FILE_PATH		= "_Data/Config/Quality";
	
	#endregion



	#region Section Keys

	public const string CONST_DEVICE_SECTION		= "Device";

	/// Low, Medium, High
	public const string CONST_QUALITY_SECTION		= "Quality";

	public const string CONST_GRAPHICS_SECTION		= "GDN";

	#endregion



	#region Section Values

	public const string CONST_DEVICES_ANDROID_OTHERS	= "Android.Others";

	public const string CONST_DEVICES_IOS_OTHERS		= "iOS.Others";

	#endregion



	#region Config Keys

	public const string CONST_DEFAULT_QUALITY			= "Quality";

	public const string CONST_DEVICE_TAG				= "DeviceTag";


	
	public const string CONST_IN_CITY_SHADOW			= "InCityShadow";

	public const string CONST_BATTLE_FIELD_SHADOW		= "BattleFieldShadow";

	public const string CONST_BLADE_EFFECT				= "BladeEffect";

	public const string CONST_BOSS_EFFECT				= "BossEffect";

	public const string CONST_AA						= "AntiAlias";

	// diabled
	public const string CONST_BLOOM						= "Bloom";

	public const string CONST_CHARACTER_HITTED_FX		= "HittedPS";

	public const string CONST_SCNE_FX_LEVEL				= "SceneFx";

	#endregion



	#region Quality Level Values
	
	public const string CONST_QUALITY_SECTION_LOW		= "Low";
	
	public const string CONST_QUALITY_SECTION_MEDIUM	= "Medium";
	
	public const string CONST_QUALITY_SECTION_HIGH		= "High";
	
	#endregion
}