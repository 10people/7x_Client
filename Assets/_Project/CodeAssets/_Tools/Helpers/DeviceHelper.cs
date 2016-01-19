//#define DEBUG_DEVICE_INFO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeviceHelper {

	/// Device values dict.
	public static Dictionary<string, ConfigTool.ConfigValue> m_device_dict = new Dictionary<string, ConfigTool.ConfigValue>();
	
	/// Device txt dict.
	public static Dictionary<string, string> m_device_xml_dict = new Dictionary<string, string>();

	private const char CONST_GDN_SPLITTER			= '/';



	#region Data

	public static void CleanDeviceData(){
		m_device_dict.Clear();
		
		m_device_xml_dict.Clear();
	}

	public static void DeviceLoadCallback( ref WWW p_www, string p_path, Object p_object ){
		if( DeviceHelper.m_device_dict.Count > 0 && DeviceHelper.m_device_xml_dict.Count > 0) {
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

	#endregion


	#region Check Device
	
	private static string m_nonsupport_reason = "";
	
	public static bool CheckIsDeviceSupported(){
		bool t_is_supported = IsSupported ();

		#if DEBUG_DEVICE_INFO
		Debug.Log( "CheckIsDeviceSupported( " + t_is_supported + " )" );

		LogDeviceInfo();
		#endif

		if( !t_is_supported ){
			if( AccountRequest.account != null && AccountRequest.account.loginObj != null ){
				AccountRequest.account.loginObj.SetActive( false );
			}
			
			Global.CreateBox( LanguageTemplate.GetText( LanguageTemplate.Text.CHAT_UIBOX_INFO ),
			                 LanguageTemplate.GetText( LanguageTemplate.Text.MACHINE_TIPS_1 ),
			                 "",
			                 null,
			                 LanguageTemplate.GetText( LanguageTemplate.Text.CONFIRM ), 
			                 
			                 null, 
			                 DeviceNotSupported,
			                 null,
			                 null,
			                 null,
			                 
			                 false,
			                 false,
			                 true );
			
			{
				Debug.Log( "Nonsupport: " + m_nonsupport_reason );
			}
			
			{
				Dictionary<string,string> tempUrl = new Dictionary<string,string>();
				
				tempUrl.Add ( "UnSupport" , m_nonsupport_reason + " - " + GetDeviceInfo() );
				
				HttpRequest.Instance ().Connect ( NetworkHelper.GetPrefix() + NetworkHelper.REPORT_UNSUPPORT_DEVICE_URL, 
				                                 tempUrl, 
				                                 ReportSuccess, 
				                                 ReportFail );
			}
		}
		
		return t_is_supported;
	}
	
	private static void ReportSuccess (string p_resp ){
		Debug.Log( "ReportSuccess（ " + p_resp + " )" );
	}
	
	private static void ReportFail (string p_resp ){
		Debug.Log( "ReportFail（ " + p_resp + " )" );
	}
	
	public static void DeviceNotSupported( int p_int ){
		Debug.Log( "DeviceNotSupported()" );
		
		Debug.Log( "NonSupport Reason: " + m_nonsupport_reason );
		
		UtilityTool.QuitGame();
	}
	
	public static bool IsSupported(){
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

		// Remove CPU Check, for Emulator use
//		if( SystemInfo.processorCount < UtilityTool.GetInt( m_device_dict, CONST_MIN_CCOUNT ) ){
//			m_nonsupport_reason = SystemInfo.processorCount + " : " + 
//				CONST_MIN_CCOUNT + " = " + UtilityTool.GetInt( m_device_dict, CONST_MIN_CCOUNT );
//			
//			return false;
//		}
		
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
	
	public static string GetDeviceModelOrGen(){
		#if UNITY_ANDROID
		return SystemInfo.deviceModel;
		#endif
		
		#if UNITY_IOS
		return UnityEngine.iOS.Device.generation.ToString();
		#endif
		
		return "Default";
	}

	public static string GetDeviceCompany(){
		#if UNITY_EDITOR || UNITY_STANDALONE
		return "PC";
		#endif

		string[] t_items = SystemInfo.deviceModel.Split( ' ' );

		#if UNITY_ANDROID
		return t_items[ 0 ];
		#endif

		#if UNITY_IOS
		return "iPhone";
		#endif
		
		return "Default";
	}

	public static string GetDeviceInfo(){
		string t_info = CONST_DEVICE_NAME + "-" + SystemInfo.deviceName + " " +
			CONST_DEVICE_MODEL + "-" + GetDeviceModelOrGen() + " " +
				QualityTool.CONST_GRAPHICS_SECTION + "-" + SystemInfo.graphicsDeviceName + " " +
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

	#endregion



	#region Log

	public static void LogDeviceInfo( string[] p_params = null ){
//		#if !DEBUG_DEVICE_INFO
//		return;
//		#endif

		#if UNITY_IOS
		Debug.Log( "iGen: " + UnityEngine.iOS.Device.generation.ToString() );
		#endif
		
		Debug.Log( "Model: " + SystemInfo.deviceModel + " - Name: " + SystemInfo.deviceName );

		Debug.Log( "ModelOrGen: " + GetDeviceModelOrGen() );

		Debug.Log( "G.Name: " + SystemInfo.graphicsDeviceName );

		Debug.Log( "G.Mem: " + SystemInfo.graphicsMemorySize + " - P.FR: " + SystemInfo.graphicsPixelFillrate );
		
		Debug.Log( "S.Mem: " + SystemInfo.systemMemorySize + " - G.DV: " + SystemInfo.graphicsDeviceVersion );
		
		Debug.Log( "C.Type: " + SystemInfo.processorType + " - C.Count: " + SystemInfo.processorCount );

		Debug.Log( "SL: " + SystemInfo.graphicsShaderLevel );

		Debug.Log( "IF: " + SystemInfo.supportsImageEffects );
				
		Debug.Log( "RT: " + SystemInfo.supportsRenderTextures );

		Debug.Log( "Platform: " + ThirdPlatform.GetPlatformTag () );
	}

	#endregion



	#region Find GDN
	
	/// Desc:
	/// GDNs is splitted by '/'.
	/// 
	/// Params:
	/// p_devices_tags: PowerVR SGX 544/Adreno (TM) 320?/Adreno (TM) 330?
	public static bool FindCurrentGDN( string p_gnd_tags ){
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
	
	public static bool IsGDNFound( string p_gdn_tag ){
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
	public static bool FindCurrentDevice( string p_devices_tags, bool p_device_found, bool p_update_device_tag ){
		if( string.IsNullOrEmpty( p_devices_tags ) ){
			return false;
		}
		
		if( !p_device_found ){
			#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
			if( p_devices_tags == QualityTool.CONST_DEVICES_ANDROID_OTHERS ){
				// Android Default Devices Configs
				SetDeviceTag( p_devices_tags );
				
				return true;
			}
			#endif
			
			#if UNITY_IOS
			if( p_devices_tags == QualityTool.CONST_DEVICES_IOS_OTHERS ){
				// iOS Default Devices Configs
				SetDeviceTag( QualityTool.CONST_DEVICES_IOS_OTHERS );
				
				return true;
			}
			#endif
		}
		
		string[] t_devices = p_devices_tags.Split( QualityTool.CONST_DEVICE_SPLITTER );
		
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
	
	public static bool IsExactDeviceFound( string p_device_tag ){
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
	
	public static void SetDeviceTag( string p_device_tag ){
		if( QualityTool.m_config_xml_dict.ContainsKey( QualityTool.CONST_DEVICE_TAG ) ){
			Debug.LogError( "Already Contained." );
			
			return;
		}
		
		QualityTool.m_config_xml_dict[ QualityTool.CONST_DEVICE_TAG ] = p_device_tag;
		
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



	#region Utilities

	public static bool Is_iOS_Target_Device(){
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
	
	public static bool Is_iOS_Low_Device(){
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

	
	
	#region Device Keys
	
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
}
