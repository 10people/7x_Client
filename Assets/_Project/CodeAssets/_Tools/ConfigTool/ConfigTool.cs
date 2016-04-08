//#define DEBUG_CONFIG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.11.28
 * @since:		Unity 4.5.3
 * Function:	Manage config files to help develop.
 * 
 * Notes:
 * 1. All Config Key MUST be listed here.
 */ 
public class ConfigTool : Singleton<ConfigTool>{
	
	/// Config values dict.
	public static Dictionary<string, ConfigValue> m_config_value_dict = new Dictionary<string, ConfigValue>();
	
	/// Config txt dict.
	private static Dictionary<string, string> m_config_xml_dict = new Dictionary<string, string>();
	
	public const char CONST_LINE_SPLITTER		= ':';



	public class ConfigValue{
		public enum ValueType{
			EmptyValue = 0,
			BoolValue,
			IntValue,
			FloatValue,
			StringValue,
		}

		private ValueType m_value_type = ValueType.EmptyValue;

		public ValueType m_type{
			set{
				m_value_type = value;
			}
			get{
				return m_value_type;
			}
		}


		private bool m_bool_value = false;

		public bool m_bool{
			set{
				m_bool_value = value;
				
				m_value_type = ValueType.BoolValue;
			}
			get{
				return m_bool_value;
			}
		}


		private int m_int_value = 0;

		public int m_int{
			set{
				m_int_value = value;
				
				m_value_type = ValueType.IntValue;
			}
			get{
				return m_int_value;
			}
		}


		private float m_float_value = 0f;

		public float m_float{
			set{
				m_float_value = value;
				
				m_value_type = ValueType.FloatValue;
			}
			get{
				return m_float_value;
			}
		}


		private string m_string_value = "";

		public string m_string{
			set{
				m_string_value = value;
				
				m_value_type = ValueType.StringValue;
			}
			get{
				return m_string_value;
			}
		}


		public void AutoSet( string p_container ){
			if( m_type == ValueType.BoolValue ){
				m_bool = bool.Parse( p_container );
			}
			else if( m_type == ValueType.FloatValue ){
				m_float = float.Parse( p_container );
			}
			else if( m_type == ValueType.IntValue ){
				m_int = int.Parse( p_container );
			}
			else if( m_type == ValueType.StringValue ){
				m_string = p_container;
			}
			else{
				Debug.LogError( "ErrorType: " + p_container );
			}
		}

		public string ValueToString(){
			switch( m_type ){
			case ValueType.EmptyValue:
				return "EmptyType";

			case ValueType.BoolValue:
				return m_bool_value.ToString();

			case ValueType.FloatValue:
				return m_float_value.ToString();

			case ValueType.IntValue:
				return m_int_value.ToString();

			case ValueType.StringValue:
				return m_string_value.ToString();
			}

			return "Error.Type";
		}
	}

	#region Instance
	
	public FPSCounter_CS m_fps_counter = null;
	
	#endregion



	#region Mono

	void Awake()
    {
	}

	void Start(){

	}

	void Update(){
		if( GetBool( CONST_SHOW_FPS ) ){
			ComponentHelper.AddIfNotExist( gameObject, typeof(FPSCounter_CS) );
		}

		{
			int t_fps = GetInt( CONST_TARGET_FPS );

			if( Application.targetFrameRate != t_fps ){
				ResetFPS();
			}
		}
		
//		{
//			if( NetworkWaiting.GetShowWaiting() != GetBool( CONST_NETWORK_SHOW_STATUS ) ){
//				ResetNetworkShowWaiting();
//			}
//		}
	}

	void OnDestroy(){
		CleanData();

		base.OnDestroy();
	}

	#endregion



	#region GUI

	#if UNITY_IOS || UNITY_EDITOR || UNITY_ANDROID
	public void OnGUI(){
		{
			// init and common
			Config_Common.OnGUI();
		}

		{
			Config_Camera.OnGUI();
		}

		{
			Config_ParticleSystem.OnGUI();
		}

		{
			Config_Quality.OnGUI();
		}
	}
	#endif

	#endregion



	#region Load

	void CleanData(){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigTool.CleanData()" );
		#endif

		m_config_xml_dict.Clear();
		
		m_config_value_dict.Clear();
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

		if ( m_config_xml_dict.Count > 0 && m_config_value_dict.Count > 0 ) {
			return;
		}

		{
			TextAsset t_text = ( TextAsset )p_object;

			UtilityTool.LoadStringStringDict( m_config_xml_dict, t_text, CONST_LINE_SPLITTER );
		}

		// Load Items
		{
			LoadConfigItems();
		}

		// Exe Items
		{
			ExeConfigItems();
		}

		// Log Items
		{
			LogConfigItems();
		}
	}
	
	private void LoadConfigItems(){
//		Debug.Log( "ConfigTool.LoadConfigItems." );

		// version
		if( ContainsKey( m_config_xml_dict, CONST_SHOW_VERSION ) ){
			LoadValues( m_config_value_dict, CONST_SHOW_VERSION, LoadBoolValue( m_config_xml_dict, CONST_SHOW_VERSION ) );
		}

		{
			LoadValues( m_config_value_dict, CONST_INVESTIGATION_USE, LoadBoolValue( m_config_xml_dict, CONST_INVESTIGATION_USE ) );
		}

		// debug
		{
			LoadValues( m_config_value_dict, CONST_SHOW_SCREEN_LOG, LoadBoolValue( m_config_xml_dict, CONST_SHOW_SCREEN_LOG ) );

			LoadValues( m_config_value_dict, CONST_SHOW_CONSOLE, LoadBoolValue( m_config_xml_dict, CONST_SHOW_CONSOLE ) );

			LoadValues( m_config_value_dict, CONST_COMMON_CODE_EXCEPTION, LoadBoolValue( m_config_xml_dict, CONST_COMMON_CODE_EXCEPTION ) );

			LoadValues( m_config_value_dict, CONST_NETWORK_CLOSE_SWITCHER, LoadBoolValue( m_config_xml_dict, CONST_NETWORK_CLOSE_SWITCHER ) );

			LoadValues( m_config_value_dict, CONST_MANUAL_CLEAN, LoadBoolValue( m_config_xml_dict, CONST_MANUAL_CLEAN ) );

			LoadValues( m_config_value_dict, CONST_SHOW_MAIN_CAMERA_INFO, LoadBoolValue( m_config_xml_dict, CONST_SHOW_MAIN_CAMERA_INFO ) );

			LoadValues( m_config_value_dict, CONST_SHOW_PARTICLE_CONTROLLERS, LoadBoolValue( m_config_xml_dict, CONST_SHOW_PARTICLE_CONTROLLERS ) );

			LoadValues( m_config_value_dict, CONST_SHOW_CAMERA_SUPERIOR, LoadBoolValue( m_config_xml_dict, CONST_SHOW_CAMERA_SUPERIOR ) );

			LoadValues( m_config_value_dict, CONST_SHOW_QUALITY_SWITCH, LoadBoolValue( m_config_xml_dict, CONST_SHOW_QUALITY_SWITCH ) );

			LoadValues( m_config_value_dict, CONST_SHOW_CURRENT_LOADING, LoadBoolValue( m_config_xml_dict, CONST_SHOW_CURRENT_LOADING ) );

			LoadValues( m_config_value_dict, CONST_SHOW_DEVICE_INFO, LoadBoolValue( m_config_xml_dict, CONST_SHOW_DEVICE_INFO ) );

			//LoadValues( m_config_value_dict, CONST_OPEN_CHECK_XML_TOOL, LoadBoolValue( m_config_dict, CONST_OPEN_CHECK_XML_TOOL ) );

			LoadValues( m_config_value_dict, CONST_QUICK_PAUSE, LoadBoolValue( m_config_xml_dict, CONST_QUICK_PAUSE ) );
			
			LoadValues( m_config_value_dict, CONST_QUICK_FX, LoadBoolValue( m_config_xml_dict, CONST_QUICK_FX ) );
		}

		// fps
		{
			// show fps
			{
				LoadValues( m_config_value_dict, CONST_SHOW_FPS, LoadBoolValue( m_config_xml_dict, CONST_SHOW_FPS ) );
			}
			
			// target fps
			if( ContainsKey( m_config_xml_dict, CONST_TARGET_FPS ) ){
				LoadValues( m_config_value_dict, CONST_TARGET_FPS, LoadIntValue( m_config_xml_dict, CONST_TARGET_FPS ) );

				ResetFPS();
			}
			else{
				// default
				QualitySettings.vSyncCount = 1;
				
				Application.targetFrameRate = 60;
			}

			LoadValues( m_config_value_dict, CONST_LOADING_INTERVAL, LoadFloatValue( m_config_xml_dict, CONST_LOADING_INTERVAL ) );

			LoadValues( m_config_value_dict, CONST_MAINCITY_UI_GC, LoadFloatValue( m_config_xml_dict, CONST_MAINCITY_UI_GC ) );

			LoadValues( m_config_value_dict, CONST_MAX_CHAR_COUNT, LoadIntValue( m_config_xml_dict, CONST_MAX_CHAR_COUNT ) );

			LoadValues( m_config_value_dict, CONST_MAX_CHAR_IN_TAN_BAO_COUNT, LoadIntValue( m_config_xml_dict, CONST_MAX_CHAR_IN_TAN_BAO_COUNT ) );

			LoadValues( m_config_value_dict, CONST_CHAR_UPDATE_INTERVAL, LoadFloatValue( m_config_xml_dict, CONST_CHAR_UPDATE_INTERVAL ) );
		}

		// guide
		{
			LoadValues( m_config_value_dict, CONST_OPEN_GUIDE_EDITOR, LoadBoolValue( m_config_xml_dict, CONST_OPEN_GUIDE_EDITOR ) );

			LoadValues( m_config_value_dict, CONST_SHOW_GUIDE_SWITCHER, LoadBoolValue( m_config_xml_dict, CONST_SHOW_GUIDE_SWITCHER ) );

			LoadValues( m_config_value_dict, CONST_OPEN_ALLTHE_FUNCTION, LoadBoolValue( m_config_xml_dict, CONST_OPEN_ALLTHE_FUNCTION ) );
		}
		
		// battle field
		{
			LoadValues( m_config_value_dict, CONST_QUICK_CHOOSE_LEVEL, LoadBoolValue( m_config_xml_dict, CONST_QUICK_CHOOSE_LEVEL ) );

			LoadValues( m_config_value_dict, CONST_QUICK_FIGHT, LoadBoolValue( m_config_xml_dict, CONST_QUICK_FIGHT ) );

			LoadValues( m_config_value_dict, CONST_SHOW_BATTLE_CAMERA_OPS, LoadBoolValue( m_config_xml_dict, CONST_SHOW_BATTLE_CAMERA_OPS ) );
		}
		
		// network emulate
		{
			LoadValues( m_config_value_dict, CONST_NETWORK_CHECK_TIME, LoadFloatValue( m_config_xml_dict, CONST_NETWORK_CHECK_TIME ) );

			LoadValues( m_config_value_dict, CONST_NETWORK_PING_TIME, LoadFloatValue( m_config_xml_dict, CONST_NETWORK_PING_TIME ) );

			LoadValues( m_config_value_dict, CONST_NETOWRK_SOCKET_TIME_OUT, LoadFloatValue( m_config_xml_dict, CONST_NETOWRK_SOCKET_TIME_OUT ) );

			#if UNITY_EDITOR || UNITY_STANDALONE
			m_is_emulating_latency = IsEmulatingNetworkLatency();
			
			m_emulate_network_latency = GetEmulatingNetworkLatency();
			
			#elif UNITY_IPHONE || UNITY_ANDROID
			m_is_emulating_latency = false;
			
			m_emulate_network_latency = 0;
			
			#else
			Debug.Log( "#else()" );
			#endif

			LoadValues( m_config_value_dict, CONST_NETWORK_SHOW_STATUS, LoadBoolValue( m_config_xml_dict, CONST_NETWORK_SHOW_STATUS ) );
		}
		
		// logs
		{
			LoadValues( m_config_value_dict, CONST_LOG_HTTP_STATUS, LoadBoolValue( m_config_xml_dict, CONST_LOG_HTTP_STATUS ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_SEND, LoadBoolValue( m_config_xml_dict, CONST_LOG_SOCKET_SEND ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_SEND_DETIAL, LoadBoolValue( m_config_xml_dict, CONST_LOG_SOCKET_SEND_DETIAL ) );


			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_RECEIVE, LoadBoolValue( m_config_xml_dict, CONST_LOG_SOCKET_RECEIVE ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_RECEIVE_DETAIL, LoadBoolValue( m_config_xml_dict, CONST_LOG_SOCKET_RECEIVE_DETAIL ) );


			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER, LoadBoolValue( m_config_xml_dict, CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER ) );

			LoadValues( m_config_value_dict, CONST_LOG_MAINCITY_SPRITE_MOVE, LoadBoolValue( m_config_xml_dict, CONST_LOG_MAINCITY_SPRITE_MOVE ) );

			LoadValues( m_config_value_dict, CONST_LOG_ASSET_LOADING, LoadBoolValue( m_config_xml_dict, CONST_LOG_ASSET_LOADING ) );


			LoadValues( m_config_value_dict, CONST_LOG_TOTAL_LOADING_TIME, LoadBoolValue( m_config_xml_dict, CONST_LOG_TOTAL_LOADING_TIME ) );

			LoadValues( m_config_value_dict, CONST_LOG_ITEM_LOADING_TIME, LoadBoolValue( m_config_xml_dict, CONST_LOG_ITEM_LOADING_TIME ) );

			LoadValues( m_config_value_dict, CONST_LOG_BUNDLE_DOWNLOADING, LoadBoolValue( m_config_xml_dict, CONST_LOG_BUNDLE_DOWNLOADING ) );

			LoadValues( m_config_value_dict, CONST_LOG_DIALOG_BOX, LoadBoolValue( m_config_xml_dict, CONST_LOG_DIALOG_BOX ) );


			LoadValues( m_config_value_dict, CONST_LOG_QUALITY_CONFIG, LoadBoolValue( m_config_xml_dict, CONST_LOG_QUALITY_CONFIG ) );
		}
		
		// bundle
		{
			LoadValues( m_config_value_dict, CONST_CLEAN_EDITOR_CACHE, LoadBoolValue( m_config_xml_dict, CONST_CLEAN_EDITOR_CACHE ) );
		}
	}

	private void ExeConfigItems(){
		// network
//		{
//			ResetNetworkShowWaiting();
//		}
		
		// bundle
		{
			#if UNITY_EDITOR
			if( GetBool( CONST_CLEAN_EDITOR_CACHE ) ){
				BundleHelper.CleanCache();
			}
			#endif
		}
	}

	private void LogConfigItems(){
		// log all
		{
//			LogConfigs();
		}

		{
			
		}
	}

//	private void ResetNetworkShowWaiting(){
//		NetworkWaiting.SetShowWaiting( GetBool( CONST_NETWORK_SHOW_STATUS ) );
//	}

	private void ResetFPS(){
		int t_fps = GetInt( CONST_TARGET_FPS );

		if( t_fps == 60 ){
			QualitySettings.vSyncCount = 1;
		}
		else if( t_fps == 30 ){
			QualitySettings.vSyncCount = 2;
		}
		else{
			QualitySettings.vSyncCount = 0;
		}
		
		Application.targetFrameRate = t_fps;
	}

	public static void LoadValues( Dictionary<string, ConfigValue> p_dict, string p_key, bool p_bool ){
//		Debug.Log( "Config.LoadValue: " + p_key + " - " + p_bool );

		ConfigValue t_value = new ConfigValue();

		t_value.m_bool = p_bool;

		p_dict.Add( p_key, t_value );
	}

	public static void LoadValues( Dictionary<string, ConfigValue> p_dict, string p_key, int p_int ){
//		Debug.Log( "Config.LoadValue: " + p_key + " - " + p_int );

		ConfigValue t_value = new ConfigValue();
		
		t_value.m_int = p_int;

		p_dict.Add( p_key, t_value );
	}

	public static void LoadValues( Dictionary<string, ConfigValue> p_dict, string p_key, float p_float ){
//		Debug.Log( "Config.LoadValue: " + p_key + " - " + p_float );

		ConfigValue t_value = new ConfigValue();
		
		t_value.m_float = p_float;
		
		p_dict.Add( p_key, t_value );
	}

	public static void LoadValues( Dictionary<string, ConfigValue> p_dict, string p_key, string p_string ){
//		Debug.Log( "Config.LoadValue: " + p_key + " - " + p_string );

		ConfigValue t_value = new ConfigValue();
		
		t_value.m_string = p_string;
		
		p_dict.Add( p_key, t_value );
	}

	/** Desc:
	 * Get Value with Key.
	 * 
	 * Return:
	 * exist, then return;
	 * if not, return "";
	 */
	public static string LoadStringValue( Dictionary<string, string> p_dict, string p_key ){
		if( !ContainsKey( p_dict, p_key ) ){
			Debug.LogWarning( "Config.Key not contained: " + p_key );
			
			return "";
		}
		
		string t_value = p_dict[ p_key ];
		
		if( string.IsNullOrEmpty( t_value ) ){
			Debug.LogError( "value.IsNullOrEmpty: " + p_key );
			
			return "";
		}
		
		return t_value;
	}
	
	/** Desc:
	 * Get Value with Key.
	 * 
	 * Return:
	 * exist, then return;
	 * if not, return false;
	 */
	public static bool LoadBoolValue( Dictionary<string, string> p_dict, string p_key ){
		if( !ContainsKey( p_dict, p_key ) ){
			Debug.LogWarning( "Config.Key not contained: " + p_key );
			
			return false;
		}
		
		string t_value = p_dict[ p_key ];
		
		if( string.IsNullOrEmpty( t_value ) ){
			Debug.LogError( "value.IsNullOrEmpty: " + p_key );
			
			return false;
		}
		
		if( t_value.ToLower() == "true" ||
		   t_value == "1" ){
			return true;
		}
		
		if( t_value.ToLower() == "false" ||
		   t_value == "0" ){
			return false;
		}
		
		Debug.LogError( "Error,LoadBoolValue: " + p_key );
		
		return false;
	}
	
	/** Desc:
	 * Get Value with Key.
	 * 
	 * Return:
	 * exist, then return;
	 * if not, return 0;
	 */
	public static int LoadIntValue( Dictionary<string, string> p_dict, string p_key ){
		if( !ContainsKey( p_dict, p_key ) ){
			Debug.LogWarning( "Config.Key not contained: " + p_key );
			
			return 0;
		}
		
		string t_value = p_dict[ p_key ];
		
		if( string.IsNullOrEmpty( t_value ) ){
			Debug.LogError( "value.IsNullOrEmpty: " + p_key );
			
			return 0;
		}
		
		int t_int = int.Parse( t_value );
		
		return t_int;
	}
	
	/** Desc:
	 * Get Value with Key.
	 * 
	 * Return:
	 * exist, then return;
	 * if not, return 0;
	 */
	public static float LoadFloatValue( Dictionary<string, string> p_dict, string p_key ){
		if( !ContainsKey( p_dict, p_key ) ){
			Debug.LogWarning( "Config.Key not contained: " + p_key );
			
			return 0;
		}
		
		string t_value = p_dict[ p_key ];
		
		if( string.IsNullOrEmpty( t_value ) ){
			Debug.LogError( "value.IsNullOrEmpty: " + p_key );
			
			return 0;
		}
		
		float t_float = float.Parse( t_value );
		
		return t_float;
	}

	#endregion



	#region Checker

	public static bool ContainsKey( Dictionary<string, string> p_dict, string p_key ){
		return p_dict.ContainsKey( p_key );
	}

	public static bool IsEmulatingNetworkLatency(){
		bool t_emulating = false;

		if( ContainsKey( m_config_xml_dict, CONST_NETWORK_LATENCY ) ){
			float t_latency = LoadFloatValue( m_config_xml_dict, CONST_NETWORK_LATENCY );

			if( t_latency > 0 ){
				t_emulating = true;
			}
		}

		return t_emulating;
	}

	#endregion


	#region Get Target Value

	public static float GetEmulatingNetworkLatency(){
		if( ContainsKey( m_config_xml_dict, CONST_NETWORK_LATENCY ) ){
			return LoadFloatValue( m_config_xml_dict, CONST_NETWORK_LATENCY );
		}

		return 0;
	}


	#endregion



	#region Get Config Value

	public static bool GetBool( string p_key, bool p_default_value = false ){
		return UtilityTool.GetBool( m_config_value_dict, p_key, p_default_value );
	}

	public static int GetInt( string p_key, int p_default_value = 0 ){
		return UtilityTool.GetInt( m_config_value_dict, p_key, p_default_value );
	}

	public static float GetFloat( string p_key, float p_default_value = 0f ){
		return UtilityTool.GetFloat( m_config_value_dict, p_key, p_default_value );
	}

	public static string GetString( string p_key, string p_default_value = "" ){
		return UtilityTool.GetString( m_config_value_dict, p_key, p_default_value );
	}

	public static string ValueToString( string p_key, string p_default_value = "" ){
		return UtilityTool.ValueToString( m_config_value_dict, p_key, p_default_value );
	}

	#endregion



	#region Utilities

	public static void LogConfigs(){
		foreach( KeyValuePair<string, ConfigTool.ConfigValue> t_pair in ConfigTool.m_config_value_dict ){
			Debug.Log( "Config.( " + t_pair.Key + " - " + t_pair.Value.ValueToString() + " )" );
		}
	}

	#endregion


	
	#region File Path
	
	private const string CONST_CONFIG_FILE_PATH		= "_Data/Config/Config";
	
	#endregion


	
	#region Quick Config Variables
	
	public static bool m_is_emulating_latency 		= false;
	
	public static float m_emulate_network_latency 	= 0.0f;
	
	#endregion



	#region Version Keys

	public const string CONST_SHOW_VERSION			= "ShowVersion";

	public const string CONST_INVESTIGATION_USE		= "InVestigateVersion";

	#endregion



	#region Debug Keys

	public const string CONST_SHOW_SCREEN_LOG			= "ShowScreenLog";

	public const string CONST_SHOW_CONSOLE				= "ShowConsole";

	public const string CONST_COMMON_CODE_EXCEPTION		= "ShowCodeException";

	public const string CONST_NETWORK_CLOSE_SWITCHER	= "NetworkCloseSwitcher";
	
	public const string CONST_MANUAL_CLEAN				= "ManualClean";

	public const string CONST_SHOW_MAIN_CAMERA_INFO		= "ShowMainCameraInfo";

	public const string CONST_SHOW_PARTICLE_CONTROLLERS	= "ShowParticleControllers";

	public const string CONST_SHOW_CAMERA_SUPERIOR		= "ShowCameraSuperior";

	public const string CONST_SHOW_QUALITY_SWITCH		= "ShowQualitySwitch";

	public const string CONST_SHOW_CURRENT_LOADING		= "ShowCurrentLoading";

	public const string CONST_SHOW_DEVICE_INFO			= "ShowDeviceInfo";

	//public const string CONST_OPEN_CHECK_XML_TOOL		= "CheckXmlTool";

	public const string CONST_QUICK_PAUSE 				= "QuickPause";
	
	public const string CONST_QUICK_FX					= "QuickFx";

	#endregion



	#region FPS Keys

	public const string CONST_TARGET_FPS			= "TargetFPS";
	
	public const string CONST_SHOW_FPS				= "ShowFPS";

	public const string CONST_LOADING_INTERVAL		= "LoadingInterval";

	public const string CONST_MAINCITY_UI_GC		= "MainCityUIGC";

	public const string CONST_MAX_CHAR_COUNT		= "MaxCarriageChar";

	public const string CONST_MAX_CHAR_IN_TAN_BAO_COUNT		= "MaxCharInTanBao";

	public const string CONST_CHAR_UPDATE_INTERVAL	= "CharUpdateInterval";

	public const string CONST_MAX_EFFECT_COUNT = "MaxEffect";

	public const string CONST_EFFECT_UPDATE_INTERVAL = "EffectUpdateInterval";

	#endregion



	#region Guide Keys
	
	public const string CONST_OPEN_GUIDE_EDITOR			= "OpenGuideEditor";

	public const string CONST_SHOW_GUIDE_SWITCHER		= "ShowGuideSwitcher";

	public const string CONST_OPEN_ALLTHE_FUNCTION  	= "OpenAllTheFunction";

	#endregion



	#region BattleField Keys

	public const string CONST_QUICK_CHOOSE_LEVEL		= "QuickChooseLevel";

	public const string CONST_QUICK_FIGHT				= "QuickFight";

	public const string CONST_SHOW_BATTLE_CAMERA_OPS	= "ShowBattleCameraOps";

	#endregion



	#region Network Keys

	public const string CONST_NETWORK_CHECK_TIME		= "SocketCheckTime";

	public const string CONST_NETWORK_PING_TIME			= "SocketPingTime";

	public const string CONST_NETOWRK_SOCKET_TIME_OUT	= "SocketTimeOut";
	
	public const string CONST_NETWORK_LATENCY			= "NetworkLatency";

	public const string CONST_NETWORK_SHOW_STATUS		= "ShowNetworkStatus";

	#endregion



	#region Log Keys

	public const string CONST_LOG_HTTP_STATUS			= "LogHttpStatus";

	public const string CONST_LOG_SOCKET_SEND			= "LogSocketSend";
	public const string CONST_LOG_SOCKET_SEND_DETIAL	= "LogSocketSendDetail";

	public const string CONST_LOG_SOCKET_RECEIVE		= "LogSocketReceive";
	public const string CONST_LOG_SOCKET_RECEIVE_DETAIL	= "LogSocketReceiveDetail";

	public const string CONST_LOG_SOCKET_WAITING		= "LogSocketWaiting";

	public const string CONST_LOG_SOCKET_PROCESSOR_AND_LISTENER		= "LogSocketProcessorListener";

	public const string CONST_LOG_MAINCITY_SPRITE_MOVE	= "LogMainCitySpriteMove";


	public const string CONST_LOG_ASSET_LOADING			= "LogAssetsLoading";


	public const string CONST_LOG_ITEM_LOADING_TIME		= "LogItemLoadingTime";

	public const string CONST_LOG_TOTAL_LOADING_TIME	= "LogTotalLoadingTime";


	public const string CONST_LOG_BUNDLE_DOWNLOADING	= "LogBundleDownLoading";


	public const string CONST_LOG_DIALOG_BOX			= "LogDialogBox";


	public const string CONST_LOG_QUALITY_CONFIG		= "LogQuality";
			
	#endregion



	#region Bundle Keys

	public const string CONST_CLEAN_EDITOR_CACHE		= "CleanEditorCache";

	#endregion
}
