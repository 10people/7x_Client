//#define DEBUG_QUALITY

//#define SHOW_REAL_SHADOW

//#define IGNORE_EDITOR

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

	private enum AA{
		None,
		Low,	// 2
		Medium,	// 4
		High,	// 8
	}

	/// Quality values dict.
	public static Dictionary<string, ConfigTool.ConfigValue> m_quality_dict = new Dictionary<string, ConfigTool.ConfigValue>();

	/// Config txt dict.
	private static Dictionary<string, string> m_config_xml_dict = new Dictionary<string, string>();


	public const char CONST_SECTION_SPLITTER		= '=';

	private const char CONST_DEVICE_SPLITTER		= '/';

	private const char CONST_GDN_SPLITTER			= '/';


	private Shader m_blade_effect = null;


	/// Device values dict.
	public static Dictionary<string, ConfigTool.ConfigValue> m_device_dict = new Dictionary<string, ConfigTool.ConfigValue>();
	
	/// Device txt dict.
	private static Dictionary<string, string> m_device_xml_dict = new Dictionary<string, string>();

	#region Mono

	void OnDestroy(){
		CleanQualityData();

		CleanDeviceData();
	}

	#endregion



	#region Load

	public void LoadQualities( EventDelegate.Callback p_callback = null ){
		Global.ResourcesDotLoad( CONST_DEVICE_FILE_PATH, DeviceLoadCallback, null );

		Global.ResourcesDotLoad( CONST_QUALITY_FILE_PATH, QualityLoadCallback, UtilityTool.GetEventDelegateList( p_callback ) );
	}

	private void CleanDeviceData(){
		m_device_dict.Clear();
		
		m_device_xml_dict.Clear();
	}

	public void DeviceLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		if (m_device_dict.Count > 0 && m_device_xml_dict.Count > 0) {
			return;
		}

		// load xml
		{
			TextAsset t_text = ( TextAsset )p_object;
			
			string[] t_lines = t_text.text.Split( '\n' );
			
			foreach( string t_line in t_lines ){
				string[] t_pair = t_line.Split( ConfigTool.CONST_LINE_SPLITTER );
				
				if( t_pair.Length == 2 ){
					if( !m_device_xml_dict.ContainsKey( t_pair[ 0 ].Trim() ) ){
						m_device_xml_dict.Add( t_pair[ 0 ].Trim(), t_pair[ 1 ].Trim() );
					}
				}
				else{
//					Debug.LogWarning( "Parse Error: " + t_line );
				}
			}
		}

		// load items
		{
			ConfigTool.LoadValues( m_device_dict, CONST_MIN_G_MEM, ConfigTool.LoadIntValue( m_device_xml_dict, CONST_MIN_G_MEM ) );

			ConfigTool.LoadValues( m_device_dict, CONST_MIN_MEM, ConfigTool.LoadIntValue( m_device_xml_dict, CONST_MIN_MEM ) );

			ConfigTool.LoadValues( m_device_dict, CONST_MIN_SL, ConfigTool.LoadIntValue( m_device_xml_dict, CONST_MIN_SL ) );

			ConfigTool.LoadValues( m_device_dict, CONST_MIN_CCOUNT, ConfigTool.LoadIntValue( m_device_xml_dict, CONST_MIN_CCOUNT ) );

			ConfigTool.LoadValues( m_device_dict, CONST_IE, ConfigTool.LoadBoolValue( m_device_xml_dict, CONST_IE ) );

			ConfigTool.LoadValues( m_device_dict, CONST_RT, ConfigTool.LoadBoolValue( m_device_xml_dict, CONST_RT ) );

			ConfigTool.LoadValues( m_device_dict, CONST_UNGDN, ConfigTool.LoadStringValue( m_device_xml_dict, CONST_UNGDN ) );

			ConfigTool.LoadValues( m_device_dict, CONST_UNDVC, ConfigTool.LoadStringValue( m_device_xml_dict, CONST_UNDVC ) );
		}
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

		#if DEBUG_QUALITY
		LogQualityItems();
		#endif

		{
			ExecQualityItems();
		}
	}

	#endregion

	
	
	#region Check Device

	private string m_nonsupport_reason = "";

	public bool CheckIsDeviceSupported(){
		Debug.Log( "CheckIsDeviceSupported()" );

		bool t_is_supported = IsSupported ();

		if( !t_is_supported ){
			{
				AccountRequest.account.loginObj.SetActive( false );
			}

			Global.CreateBox( LanguageTemplate.GetText( LanguageTemplate.Text.CHAT_UIBOX_INFO ),
			                 LanguageTemplate.GetText( LanguageTemplate.Text.MACHINE_TIPS_1 ),
			                 "",
			                 null,
			                 LanguageTemplate.GetText( LanguageTemplate.Text.CONFIRM ), 
			                 null, 
			                 DeviceNotSupported,
			                 null );

			{
				Debug.Log( "Nonsupport: " + m_nonsupport_reason );
			}

			{
				Dictionary<string,string> tempUrl = new Dictionary<string,string>();
				
				tempUrl.Add ( "UnSupport" , m_nonsupport_reason + " - " + GetDeviceInfo() );
				
				HttpRequest.Instance ().Connect ( HttpRequest.GetPrefix() + HttpRequest.REPORT_UNSUPPORT_DEVICE_URL, 
				                                 tempUrl, 
				                                 ReportSuccess, 
				                                 ReportFail );
			}
		}

		return t_is_supported;
	}

	private void ReportSuccess (string p_resp ){
		Debug.Log( "ReportSuccess（ " + p_resp + " )" );
	}

	private void ReportFail (string p_resp ){
		Debug.Log( "ReportFail（ " + p_resp + " )" );
	}

	public void DeviceNotSupported( int p_int ){
		Debug.Log( "DeviceNotSupported()" );

		Debug.Log( "NonSupport Reason: " + m_nonsupport_reason );

		UtilityTool.QuitGame();
	}
	
	private bool IsSupported(){
		if( SystemInfo.graphicsMemorySize < UtilityTool.GetInt( m_device_dict, CONST_MIN_G_MEM ) ){
			m_nonsupport_reason = SystemInfo.graphicsMemorySize + " : " + 
				CONST_MIN_G_MEM + " = " + UtilityTool.GetInt( m_device_dict, CONST_MIN_G_MEM );

			return false;
		}

		if( SystemInfo.systemMemorySize < UtilityTool.GetInt( m_device_dict, CONST_MIN_MEM ) ){
			m_nonsupport_reason = SystemInfo.systemMemorySize + " : " + 
				CONST_MIN_MEM + " = " + UtilityTool.GetInt( m_device_dict, CONST_MIN_MEM );
			
			return false;
		}

		if( SystemInfo.graphicsShaderLevel < UtilityTool.GetInt( m_device_dict, CONST_MIN_SL ) ){
			m_nonsupport_reason = SystemInfo.graphicsShaderLevel + " : " + 
				CONST_MIN_SL + " = " + UtilityTool.GetInt( m_device_dict, CONST_MIN_SL );
			
			return false;
		}

		if( SystemInfo.processorCount < UtilityTool.GetInt( m_device_dict, CONST_MIN_CCOUNT ) ){
			m_nonsupport_reason = SystemInfo.processorCount + " : " + 
				CONST_MIN_CCOUNT + " = " + UtilityTool.GetInt( m_device_dict, CONST_MIN_CCOUNT );
			
			return false;
		}

		if( SystemInfo.supportsImageEffects != UtilityTool.GetBool( m_device_dict, CONST_IE ) ){
			m_nonsupport_reason = SystemInfo.supportsImageEffects + " : " + 
				CONST_IE + " = " + UtilityTool.GetBool( m_device_dict, CONST_IE );
			
			return false;
		}

		if( SystemInfo.supportsRenderTextures != UtilityTool.GetBool( m_device_dict, CONST_RT ) ){
			m_nonsupport_reason = SystemInfo.supportsRenderTextures + " : " + 
				CONST_RT + " = " + UtilityTool.GetBool( m_device_dict, CONST_RT );
			
			return false;
		}

		if( FindCurrentDevice( UtilityTool.GetString( m_device_dict, CONST_UNDVC ), false, false ) ){
			m_nonsupport_reason = GetDeviceModelOrGen() + " : " + 
				CONST_UNDVC + " = " + UtilityTool.GetString( m_device_dict, CONST_UNDVC );
			
			return false;
		}

		if( FindCurrentGDN( UtilityTool.GetString( m_device_dict, CONST_UNGDN ) ) ){
			m_nonsupport_reason = SystemInfo.graphicsDeviceName + " : " + 
				CONST_UNGDN + " = " + UtilityTool.GetString( m_device_dict, CONST_UNGDN );

			return false;
		}
		                      
		return true;
	}

	public static string GetDeviceInfo(){
		string t_info = CONST_DEVICE_NAME + "-" + SystemInfo.deviceName + " " +
			CONST_DEVICE_MODEL + "-" + GetDeviceModelOrGen() + " " +
			CONST_GRAPHICS_SECTION + "-" + SystemInfo.graphicsDeviceName + " " +
			CONST_MIN_G_MEM + "-" + SystemInfo.graphicsMemorySize + " " +
			CONST_MIN_MEM + "-" + SystemInfo.systemMemorySize + " " +
			CONST_MIN_SL + "-" + SystemInfo.graphicsShaderLevel + " " +
			CONST_MIN_CCOUNT + "-" + SystemInfo.processorCount + " " +
			CONST_IE + "-" + SystemInfo.supportsImageEffects + " " +
			CONST_RT + "-" + SystemInfo.supportsRenderTextures + " " +
			CONST_CHANNEL_TAG + "-" + ThirdPlatform.GetPlatformTag () + " " +
			CONST_SESSION_TAG + "-" + ThirdPlatform.GetPlatformSession ();

        return t_info;
	}

	public static string GetDeviceModelOrGen(){
		#if UNITY_ANDROID
		return SystemInfo.deviceModel;
		#endif
		
		#if UNITY_IOS
		return UnityEngine.iOS.Device.generation.ToString();
		#endif

		return "Default";
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

						if( !FindCurrentDevice( t_cur_section_value, t_device_found, true ) ){
							t_skip_to_next_section = true;

							continue;
						}
						else{
							t_device_found = true;
						}
					}
					else if( t_cur_section_key == CONST_GRAPHICS_SECTION ){
						// gdn check

						if( !FindCurrentGDN( t_cur_section_value ) ){
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
		
		ConfigTool.LoadValues( m_quality_dict, CONST_IN_CITY_SHADOW, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_IN_CITY_SHADOW ) );
		
		ConfigTool.LoadValues( m_quality_dict, CONST_BATTLE_FIELD_SHADOW, ConfigTool.LoadBoolValue( m_config_xml_dict, CONST_BATTLE_FIELD_SHADOW ) );
		
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
		// AA
		{
			#if UNITY_IOS
			QualityTool.ConfigAA();
			#endif
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

		m_config_xml_dict[ CONST_IN_CITY_SHADOW ] = "false";
		
		m_config_xml_dict[ CONST_BATTLE_FIELD_SHADOW ] = "true";
		
		m_config_xml_dict[ CONST_BLADE_EFFECT ] = "false";
		
		m_config_xml_dict[ CONST_BOSS_EFFECT ] = "true";

		m_config_xml_dict[ CONST_AA ] = "None";

		m_config_xml_dict[ CONST_BLOOM ] = "false";

		m_config_xml_dict[ CONST_CHARACTER_HITTED_FX ] = "false";

		m_config_xml_dict[ CONST_SCNE_FX_LEVEL ] = CONST_SCENE_FX_LEVEL_NONE;
	}

	/// Highest quality for all platform.
	private void SetToHighest(){
		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			Debug.Log( "SetToHighest()" );
		}

		m_config_xml_dict[ CONST_IN_CITY_SHADOW ] = "true";
		
		m_config_xml_dict[ CONST_BATTLE_FIELD_SHADOW ] = "true";
		
		m_config_xml_dict[ CONST_BLADE_EFFECT ] = "true";
		
		m_config_xml_dict[ CONST_BOSS_EFFECT ] = "true";

		m_config_xml_dict[ CONST_AA ] = "Low";

		m_config_xml_dict[ CONST_BLOOM ] = "true";

		m_config_xml_dict[ CONST_CHARACTER_HITTED_FX ] = "true";

		m_config_xml_dict[ CONST_SCNE_FX_LEVEL ] = CONST_SCENE_FX_LEVEL_HIGH;
	}

	#endregion



	#region Find GDN

	/// Desc:
	/// GDNs is splitted by '/'.
	/// 
	/// Params:
	/// p_devices_tags: PowerVR SGX 544/Adreno (TM) 320?/Adreno (TM) 330?
	private bool FindCurrentGDN( string p_gnd_tags ){
		if( string.IsNullOrEmpty( p_gnd_tags ) ){
			return false;
		}

		string[] t_gnds = p_gnd_tags.Split( CONST_GDN_SPLITTER );
		
		for( int i = 0; i < t_gnds.Length; i++ ){
			string t_gdn_tag = t_gnds[ i ];

			if( IsGDNFound( t_gdn_tag ) ){
				return true;
			}
		}
		
		return false;
	}
	
	private bool IsGDNFound( string p_gdn_tag ){
		if( string.IsNullOrEmpty( p_gdn_tag ) ){
			return false;
		}

		bool t_found = SystemInfo.graphicsDeviceName.Trim().ToLowerInvariant() == p_gdn_tag.Trim().ToLowerInvariant();

		if( t_found ){
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
				Debug.Log( "GDN.Found: " + SystemInfo.graphicsDeviceName );
			}
		}

		return t_found;
	}

	#endregion



	#region Find Device
	
	/// Desc:
	/// Devices is splitted by '/'.
	/// 
	/// Params:
	/// p_devices_tags: iPhone5S/iPhone6/iPhone6Plus
	private bool FindCurrentDevice( string p_devices_tags, bool p_device_found, bool p_update_device_tag ){
		if( string.IsNullOrEmpty( p_devices_tags ) ){
			return false;
		}

		if( !p_device_found ){
			#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
			if( p_devices_tags == CONST_DEVICES_ANDROID_OTHERS ){
				// Android Default Devices Configs
				SetDeviceTag( p_devices_tags );
				
				return true;
			}
			#endif

			#if UNITY_IOS
			if( p_devices_tags == CONST_DEVICES_IOS_OTHERS ){
				// iOS Default Devices Configs
				SetDeviceTag( CONST_DEVICES_IOS_OTHERS );
				
				return true;
			}
			#endif
		}
		
		string[] t_devices = p_devices_tags.Split( CONST_DEVICE_SPLITTER );

		for( int i = 0; i < t_devices.Length; i++ ){
			string t_device_tag = t_devices[ i ];

			if( IsExactDeviceFound( t_device_tag ) ){
				if( p_update_device_tag ){
					SetDeviceTag( t_device_tag );
				}
				
				return true;
			}
		}
		
		return false;
	}

	private bool IsExactDeviceFound( string p_device_tag ){
		if( string.IsNullOrEmpty( p_device_tag ) ){
			return false;
		}
		bool t_found = false;

		#if UNITY_ANDROID
		{
//			Debug.Log( "p_device_tag: " + p_device_tag + " --- " + SystemInfo.deviceModel );

			t_found = SystemInfo.deviceModel == p_device_tag;
		}
		#endif

		#if UNITY_IOS
		{
			t_found = UnityEngine.iOS.Device.generation.ToString() == p_device_tag;
		}
		#endif

		return t_found;
	}
	
	private void SetDeviceTag( string p_device_tag ){
		if( m_config_xml_dict.ContainsKey( CONST_DEVICE_TAG ) ){
			Debug.LogError( "Already Contained." );
			
			return;
		}
		
		m_config_xml_dict[ CONST_DEVICE_TAG ] = p_device_tag;

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			#if UNITY_ANDROID
			Debug.Log( "Device.Found: " + p_device_tag );
			#endif
			
			#if UNITY_IOS
			Debug.Log( "Device.Found: " + UnityEngine.iOS.Device.generation.ToString() );
			#endif
		}
	}

	#endregion



	#region InCity Shadow

	/// Show simple plane shadow incity or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public bool InCity_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;

		#if UNITY_EDITOR && !IGNORE_EDITOR
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return !GetBool( CONST_IN_CITY_SHADOW );
		#elif UNITY_IOS
		return InCity_iOS_ShowSimpleShadow();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );

		return true;
		#endif

		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif
		
		return t_show_simple_shadow;
	}

	private bool InCity_iOS_ShowSimpleShadow(){
		if( !Is_iOS_Target_Device() ){
			return true;
		}

		#if UNITY_IOS
		return !GetBool( CONST_IN_CITY_SHADOW );
		#endif

		return false;
	}

	#endregion



	#region BattleField Shadow
	
	/// Show simple plane shadow in battle field or not.
	/// 
	/// Notes:
	/// 1.if not showed, never create the shadow.
	public bool BattleField_ShowSimpleShadow(){
		bool t_show_simple_shadow = true;

		#if UNITY_EDITOR && !IGNORE_EDITOR
		t_show_simple_shadow = true;
		#elif UNITY_STANDALONE
		t_show_simple_shadow = true;
		#elif UNITY_ANDROID
		return !GetBool( CONST_BATTLE_FIELD_SHADOW );
		#elif UNITY_IOS
		return BattleField_iOS_ShowSimpleShadow();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return true;
		#endif

		#if SHOW_REAL_SHADOW
		t_show_simple_shadow = false;
		#endif

		return t_show_simple_shadow;
	}

	private bool BattleField_iOS_ShowSimpleShadow(){
		if( !Is_iOS_Target_Device() ){
			return true;
		}
		
		#if UNITY_IOS
		return !GetBool( CONST_BATTLE_FIELD_SHADOW );
		#endif
		
		return false;
	}
	
	#endregion



	#region Shadow

	/// Add Shadows:
	/// 
	/// 1.Add Directional light;
	/// 2.Edit Light Color and Intensity, Edit Light Direction(transform);
	/// 3.Set Shadow Type: Hard Shadow, and Edit Shadow Strength;
	/// 4.Set Culling Mask: 3DLayer, 3D Shadow Ground;
	/// 5.Enable the GameObject, Disable the Light Components;
	public static void ConfigLights( bool p_active_light ){
		#if DEBUG_QUALITY
		Debug.Log( "ConfigLights( " + p_active_light + " )" );
		#endif

		Object[] t_objs = GameObject.FindObjectsOfType( typeof(Light) );
		
		//		Debug.Log( "Active Light's GameObject Count: " + t_objs.Length );

		for( int i = 0; i < t_objs.Length; i++ ){
			Light t_light = (Light)t_objs[ i ];
			
			t_light.enabled = p_active_light;

			#if DEBUG_QUALITY
			GameObjectHelper.LogGameObjectHierarchy( t_light.gameObject, i + " Light " );
			#endif

			int t_mask = 0;

			{
				int t_index = LayerMask.NameToLayer( "3D Shadow Ground" );
				
				t_mask += 1 << t_index;
			}

			{
				int t_index = LayerMask.NameToLayer( "3D Layer" );
				
				t_mask += 1 << t_index;
			}

			
			t_light.cullingMask = t_mask;
		}

		#if DEBUG_QUALITY
		Debug.Log( "ConfigLights Done." );
		#endif
	}
	
	#endregion



	#region AA

	public static void ConfigAA(){
		string t_aa = GetString( CONST_AA );

		if( string.IsNullOrEmpty( t_aa ) ){
			Debug.LogError( "Error, No AA Setted." );

			return;
		}

		switch( t_aa ){
		case "None":
			QualitySettings.antiAliasing = 0;

			break;

		case "Low":
			QualitySettings.antiAliasing = 2;

			break;

		case "Medium":
			QualitySettings.antiAliasing = 4;

			break;

		case "High":
			QualitySettings.antiAliasing = 8;

			break;

		default:
			Debug.LogError( "Error, No Value Exist." );

			return;
		}

		if( ConfigTool.GetBool( ConfigTool.CONST_LOG_QUALITY_CONFIG, false ) ){
			Debug.Log( "AA: " + QualitySettings.antiAliasing );
		}
	}

	#endregion



	#region Bloom

//	private static FastBloom m_fast_bloom = null;
	
	public static void ConfigBloom( bool p_enable_bloom ){
		#if UNITY_IOS && !ENABLE_BLOOM
		return;
		#endif

		#if UNITY_ANDROID
		return;
		#endif

		/*
		#if DEBUG_QUALITY
		Debug.Log( "ConfigBloom( " + p_enable_bloom + " )" );
		#endif

		FastBloom t_pre_bloom = null; 

		{
			Object[] t_objs = GameObject.FindObjectsOfType( typeof(FastBloom) );

			if( t_objs.Length > 1 || t_objs.Length < 0 ){
				Debug.LogError( "Error for Bloom Config." );

				return;
			}

			if( t_objs.Length == 1 ){
				t_pre_bloom = (FastBloom)t_objs[ 0 ];
			}
		}

		if( Camera.main == null ){
			Debug.LogError( "Error, No Main Camera Setted." );

//			#if UNITY_EDITOR
//			Debug.Log( "Scene: " + Application.loadedLevelName );
//
//			UnityEditor.EditorApplication.isPaused = true;
//			#endif

			return;
		}

		if( p_enable_bloom ){
			if( QualitySettings.antiAliasing != 0 ){
				QualitySettings.antiAliasing = 0;
			}
		}

		FastBloom t_bloom = Camera.main.GetComponent<FastBloom>();

		if( t_bloom == null ){
			t_bloom = Camera.main.gameObject.AddComponent<FastBloom>();
		}
		else{

		}
		
		if( t_bloom != m_fast_bloom ){
			m_fast_bloom = t_bloom;
		}

//		Shader t_shader = Shader.Find( "Custom/Effects/MobileBloom" );

		Shader t_shader = Shader.Find( "Hidden/FastBloom" );

		if( t_shader != null ){
			t_bloom.fastBloomShader = t_shader;
		}
		else{
			Debug.LogError( "Error, Bloom Shader Not Found." );
		}

		if( m_fast_bloom != null ){
			m_fast_bloom.enabled = p_enable_bloom;
		}
		else{
			Debug.LogError( "Error, Bloom Not Found." );
		}

		if( t_pre_bloom != null && m_fast_bloom != null ){
			m_fast_bloom.threshhold = t_pre_bloom.threshhold;

			m_fast_bloom.intensity = t_pre_bloom.intensity;

			m_fast_bloom.blurSize = t_pre_bloom.blurSize;

			m_fast_bloom.blurIterations = t_pre_bloom.blurIterations;
		}
		else{
			m_fast_bloom.threshhold = 0.3f;
			
			m_fast_bloom.intensity = 0.5f;
			
			m_fast_bloom.blurSize = 0.5f;
			
			m_fast_bloom.blurIterations = 1;
		}

		if( !p_enable_bloom ){
//			Debug.Log( "Destroy.FB." );

			Destroy( t_bloom );
		}

		#if DEBUG_QUALITY
		Debug.Log( "ConfigBloom Done." );
		#endif

		*/
	}

	#endregion



	#region Blade Effect

	private bool ShowCoolBlade(){
		#if UNITY_EDITOR
		return false;
		#elif UNITY_STANDALONE
		return true;
		#elif UNITY_ANDROID
		return GetBool( CONST_BLADE_EFFECT );
		#elif UNITY_IOS
		return iOS_ShowCoolBlade();
		#else
		Debug.LogError( "TargetPlatform Error: " + Application.platform );
		
		return false;
		#endif
	}

	private bool iOS_ShowCoolBlade(){
		if( !Is_iOS_Target_Device() ){
			return false;
		}
		
		#if UNITY_IOS
		return GetBool( CONST_BLADE_EFFECT );
		#else
		return false;
		#endif
	}

	public void UpdateBladeEffect( GameObject p_gb ){
		if( !ShowCoolBlade() ){
			return;
		}

//		Debug.Log( "Set New Blade Effect." );

		if( p_gb == null ){
			Debug.LogError( "Error, GameObject Is Null." );

			return;
		}

		ParticleSystem t_ps = p_gb.GetComponent<ParticleSystem>();

		if( t_ps == null ){
			Debug.LogError( "Error, GameObject.PS Is Null." );
			
			return;
		}

		Renderer t_renderer = t_ps.GetComponent<Renderer>();

		if( t_renderer == null ){
			Debug.LogError( "Error, GameObject.PS.Renderer Is Null." );
			
			return;
		}

		Material t_mat = t_renderer.sharedMaterial;

		if( t_mat == null ){
			Debug.LogError( "Error, GameObject.PS.Renderer.Mat Is Null." );
			
			return;
		}

		if( m_blade_effect == null ){
			m_blade_effect = Shader.Find( "Custom/Effects/Blade Effect" );

			if( m_blade_effect == null ){
				Debug.LogError( "Error, Blade Effect not found." );

				return;
			}
		}

		t_mat.shader = m_blade_effect;
	}

	#endregion



	#region Scene Fx

	/// None Scene Fx
	public static bool IsSceneFxNone(){
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_SCNE_FX_LEVEL ), QualityTool.CONST_SCENE_FX_LEVEL_NONE );
	}

	public static bool IsSceneFxLow(){
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_SCNE_FX_LEVEL ), QualityTool.CONST_SCENE_FX_LEVEL_LOW );
	}

	public static bool IsSceneFxMedium(){
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_SCNE_FX_LEVEL ), QualityTool.CONST_SCENE_FX_LEVEL_MEDIUM );
	}

	public static bool IsSceneFxHigh(){
		return StringHelper.IsLowerEqual( QualityTool.GetString( QualityTool.CONST_SCNE_FX_LEVEL ), QualityTool.CONST_SCENE_FX_LEVEL_HIGH );
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


	
	#region Utilities

	public bool IsLowQuality(){
		return GetString( CONST_DEFAULT_QUALITY ) == CONST_QUALITY_SECTION_LOW;
	}
	
	public bool IsMediumQuality(){
		return GetString( CONST_DEFAULT_QUALITY ) == CONST_QUALITY_SECTION_MEDIUM;
	}
	
	public bool IsHighQuality(){
		return GetString( CONST_DEFAULT_QUALITY ) == CONST_QUALITY_SECTION_HIGH;
	}

	private bool Is_iOS_Target_Device(){
		#if UNITY_IOS
		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || 
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen || 
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ){
			return false;
		}

		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone || 
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G || 
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS ||
		   UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 ){
			return false;
		}

		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad1Gen ){
			return false;
		}
		#endif

		return true;
	}

	private bool Is_iOS_Low_Device(){
		#if UNITY_IOS
		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || 
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen || 
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen ||
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen ||
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen ){
			return true;
		}
		
		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone || 
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G || 
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3GS ||
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4 ||
		   	UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone4S ){
			return true;
		}
		
		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad1Gen ||
			UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPad2Gen ){
			return true;
		}

		if( UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPadMini1Gen ){
			return true;
		}

		return false;
		#else
		return true;
		#endif
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



	public const string CONST_QUALITY_SECTION_LOW		= "Low";
	
	public const string CONST_QUALITY_SECTION_MEDIUM	= "Medium";
	
	public const string CONST_QUALITY_SECTION_HIGH		= "High";

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



	public const string CONST_MIN_G_MEM					= "GMem";

	public const string CONST_MIN_MEM					= "Mem";

	public const string CONST_MIN_SL					= "SL";

	public const string CONST_MIN_CCOUNT				= "CCount";

	public const string CONST_IE						= "IE";

	public const string CONST_RT						= "RT";

	public const string CONST_UNGDN						= "UNGDN";

	public const string CONST_UNDVC						= "UNDVC";

	public const string CONST_CHANNEL_TAG 				= "CNTG";

	public const string CONST_SESSION_TAG				= "SID";

	#endregion



	#region Device Info

	public const string CONST_DEVICE_NAME				= "DN";

	public const string CONST_DEVICE_MODEL				= "DM";

	#endregion



	#region Scene Fx Level

	public const string CONST_SCENE_FX_LEVEL_NONE		= "None";

	public const string CONST_SCENE_FX_LEVEL_LOW		= "Low";

	public const string CONST_SCENE_FX_LEVEL_MEDIUM		= "Medium";

	public const string CONST_SCENE_FX_LEVEL_HIGH		= "High";

	#endregion
}