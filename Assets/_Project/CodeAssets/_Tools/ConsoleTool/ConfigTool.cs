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
public class ConfigTool : Singleton<ConfigTool>
{

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

	/// Config values dict.
	public static Dictionary<string, ConfigValue> m_config_value_dict = new Dictionary<string, ConfigValue>();

	/// Config txt dict.
	private static Dictionary<string, string> m_config_dict = new Dictionary<string, string>();

	public const char CONST_LINE_SPLITTER		= ':';



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
		
		{
			if( NetworkWaiting.GetShowWaiting() != GetBool( CONST_NETWORK_SHOW_STATUS ) ){
				ResetNetworkShowWaiting();
			}
		}
	}

	void OnDestroy(){
		CleanData();
	}

	#endregion



	#region GUI

	public GUIStyle m_gui_lb_style;

	public GUIStyle m_gui_btn_style;

	public GUIStyle m_gui_text_field_style;

	public void InitGUI(){
		{
			if( m_gui_lb_style == null ){
//				m_gui_lb_style = new GUIStyle( GUI.skin.label );
				m_gui_lb_style = GUI.skin.label;
			}
			
			if( m_gui_btn_style == null ){
//				m_gui_btn_style = new GUIStyle( GUI.skin.button );

				m_gui_btn_style = GUI.skin.button;
			}
			
			if( m_gui_text_field_style == null ){
//				m_gui_text_field_style = new GUIStyle( GUI.skin.textField );

				m_gui_text_field_style = GUI.skin.textField;
			}
		}
		
		{
			m_gui_lb_style.fontSize = 18;
			
			m_gui_lb_style.normal.textColor = Color.white;
		}
		
		{
			m_gui_btn_style.fontSize = 18;
		}
		
		{
			m_gui_text_field_style.fontSize = 18;
		}
	}

	public void OnGUI(){
		{
			InitGUI();
		}

		{
			OnGUI_Common();
		}

		{
			OnGUI_CameraInfo();
		}

		{
			OnGUI_Particles();
		}

		{
			OnGUI_CameraSuperior();
		}

		{
			OnGUI_Quality();
		}
	}

	#endregion



	#region GUI Common

	public float[] m_btn_rect_params = new float[ 6 ];

	public float[] m_lb_rect_params = new float[ 6 ];

	public float[] m_slider_rect_params = new float[ 6 ];

	public float[] m_toggle_rect_params = new float[ 6 ];

	public float[] m_text_field_rect_params = new float[ 6 ];

	void OnGUI_Common(){

		{
			OnGUI_Common_Info();
		}

		{
			OnGUI_Common_Debug();
		}
	}

	private static bool m_device_info_logged = false;

	void OnGUI_Common_Info(){
		if( GetBool( CONST_SHOW_VERSION ) ){
			GUI.Label( new Rect( 0, Screen.height * 0.9f, 250, 35 ), GetString( CONST_VERSION ), m_gui_lb_style );

//			GUI.Label( new Rect( 0, 25, 250, 35 ), "BV: " + Prepare_Bundle_Config.m_config_cached_small_version, m_gui_lb_style );
//
//			GUI.Label( new Rect( 0, 50, 250, 35 ), "SV: " + Prepare_Bundle_Config.CONFIG_BIG_VESION, m_gui_lb_style );
		}
		
		if( GetBool( CONST_SHOW_DEVICE_INFO ) ){
			int t_info_index = 0;

			m_lb_rect_params[ 0 ] = 0;

			m_lb_rect_params[ 1 ] = 40;

			m_lb_rect_params[ 2 ] = Screen.width;

			m_lb_rect_params[ 3 ] = 35;

			m_lb_rect_params[ 4 ] = 0;

			m_lb_rect_params[ 5 ] = 40;

			#if UNITY_IOS
			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "iGen: " + UnityEngine.iOS.Device.generation.ToString(), m_gui_lb_style );
			#endif

			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "Model: " + SystemInfo.deviceModel + " - Name: " + SystemInfo.deviceName, m_gui_lb_style );

			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "G.Name: " + SystemInfo.graphicsDeviceName, m_gui_lb_style );

			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "G.Mem: " + SystemInfo.graphicsMemorySize + " - P.FR: " + SystemInfo.graphicsPixelFillrate, m_gui_lb_style );

			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "S.Mem: " + SystemInfo.systemMemorySize + " - G.DV: " + SystemInfo.graphicsDeviceVersion, m_gui_lb_style );

			GUI.Label( UtilityTool.GetGUIRect( t_info_index++, m_lb_rect_params ), "G: " + SystemInfo.processorType + " - G.Num: " + SystemInfo.processorCount, m_gui_lb_style );

			if( !m_device_info_logged ){
				m_device_info_logged = true;

				ConsoleTool.LogDeviceInfo();
			}
		}
	}

	void OnGUI_Common_Debug(){
		{
			m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.2f;
			
			m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			m_btn_rect_params[ 4 ] = 0;
			
			m_btn_rect_params[ 5 ] = Screen.height * 0.095f;
		}

		int t_button_index = 0;

		if( GetBool( CONST_NETWORK_CLOSE_SWITCHER ) ){
			if( GUI.Button( UtilityTool.GetGUIRect( t_button_index++, m_btn_rect_params ), "Close Socket", m_gui_btn_style ) ){
				SocketTool.Instance().SetSocketLost();
			}
		}

		if( GetBool( CONST_MANUAL_CLEAN ) ){
			if( GUI.Button( UtilityTool.GetGUIRect( t_button_index++, m_btn_rect_params ), "Manual Clean", m_gui_btn_style ) ){
				Debug.Log( "Manual Clean." );

				Resources.UnloadUnusedAssets();
			}
		}

		if( GetBool( CONST_COMMON_CODE_EXCEPTION ) ){
			string t_error = UtilityTool.GetCommonCodeError();
			
			if( !string.IsNullOrEmpty( t_error ) ){
				// scroll view
				{
					UtilityTool.m_common_code_scroll_pos = GUI.BeginScrollView( 
					                                                           new Rect( 0, 110, Screen.width * 0.8f, Screen.height * 0.5f ),
					                                                           UtilityTool.m_common_code_scroll_pos,
					                                                           new Rect( 0, 0, Screen.width, Screen.height ) );
					
					GUI.Label( new Rect( 0, 0, Screen.width, Screen.height ), UtilityTool.GetCommonCodeError() );
					
					GUI.EndScrollView();
				}
				
				// control
				{
					if( GUI.Button( new Rect( 300, 0, 100, 50 ), "Clear" ) ){
						UtilityTool.ClearCommonCodeError();
					}
				}
			}
		}
	}

	#endregion



	#region GUI Camera Info

	void OnGUI_CameraInfo(){
		if( !GetBool( CONST_SHOW_MAIN_CAMERA_INFO ) ){
			return;
		}

		if( Camera.main == null ){
			return;
		}

		{
			m_lb_rect_params[ 0 ] = Screen.width * 0.05f;
			
			m_lb_rect_params[ 1 ] = Screen.height * 0.85f;
			
			m_lb_rect_params[ 2 ] = Screen.width * 0.075f;
			
			m_lb_rect_params[ 3 ] = Screen.height * 0.06f;
			
			m_lb_rect_params[ 4 ] = Screen.width * 0.075f;
			
			m_lb_rect_params[ 5 ] = 0;
		}

		{
			Vector3 m_pos = Camera.main.transform.position;
			
			GUI.Label( UtilityTool.GetGUIRect( 0, 
			                                  0, m_lb_rect_params[ 1 ],
			                                  m_lb_rect_params[ 2 ], m_lb_rect_params[ 3 ],
			                                  m_lb_rect_params[ 4 ], m_lb_rect_params[ 5 ] ), 
			          "T: ", 
			          m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 0, m_lb_rect_params ), m_pos.x.ToString( "f2" ), m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 1, m_lb_rect_params ), m_pos.y.ToString( "f2" ), m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 2, m_lb_rect_params ), m_pos.z.ToString( "f2" ), m_gui_lb_style );
		}
		
		{
			m_lb_rect_params[ 1 ] = m_lb_rect_params[ 1 ] + Screen.height * 0.075f;
			
			Quaternion m_quaternion = Camera.main.transform.rotation;
			
			GUI.Label( UtilityTool.GetGUIRect( 0, 
			                                  0, m_lb_rect_params[ 1 ],
			                                  m_lb_rect_params[ 2 ], m_lb_rect_params[ 3 ],
			                                  m_lb_rect_params[ 4 ], m_lb_rect_params[ 5 ] ), 
			          "R: ", 
			          m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 0, m_lb_rect_params ), m_quaternion.eulerAngles.x.ToString( "f2" ), m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 1, m_lb_rect_params ), m_quaternion.eulerAngles.y.ToString( "f2" ), m_gui_lb_style );
			
			GUI.Label( UtilityTool.GetGUIRect( 2, m_lb_rect_params ), m_quaternion.eulerAngles.z.ToString( "f2" ), m_gui_lb_style );
		}
	}

	#endregion



	#region Quality Set

	private bool m_light_on = false;

	void OnGUI_Quality(){
		if( !GetBool( CONST_SHOW_QUALITY_SWITCH ) ){
			return;
		}

		int t_btn_index = 0;

		{
			m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.1f;
			
			m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 3 ] = Screen.height * 0.11f;
			
			m_btn_rect_params[ 4 ] = 0;
			
			m_btn_rect_params[ 5 ] = Screen.height * 0.125f;
		}

		bool t_fog = RenderSettings.fog;

		if( GUI.Button( UtilityTool.GetGUIRect( t_btn_index++, m_btn_rect_params ), 
		               m_light_on ? "L On" : "L Off" ) ){
			m_light_on = !m_light_on;

			QualityTool.ConfigLights( m_light_on );
		}

		if( GUI.Button( UtilityTool.GetGUIRect( t_btn_index++, m_btn_rect_params ), 
		               m_config_value_dict[ CONST_SHOW_CAMERA_SUPERIOR ].m_bool ? "Cam On" : "Cam Off" ) ){
			m_config_value_dict[ CONST_SHOW_CAMERA_SUPERIOR ].m_bool = !m_config_value_dict[ CONST_SHOW_CAMERA_SUPERIOR ].m_bool;
		}

		if( GUI.Button( UtilityTool.GetGUIRect( t_btn_index++, m_btn_rect_params ), 
		               m_bloom_on ? "B On" : "B Off" ) ){
			m_bloom_on = !m_bloom_on;
			
			QualityTool.ConfigBloom( m_bloom_on );
		}

		if( GUI.Button( UtilityTool.GetGUIRect( t_btn_index++, m_btn_rect_params ), 
		               "AA " + QualitySettings.antiAliasing + "" ) ){
			int t_anti = 0;

			if( QualitySettings.antiAliasing == 0 ){
				t_anti = QualitySettings.antiAliasing + 2;
			}
			else{
				t_anti = QualitySettings.antiAliasing * 2;
			}

			if( t_anti > 8 ){
				t_anti = 0;
			}

			QualitySettings.antiAliasing = t_anti;
		}


	}

	private bool m_bloom_on = false;



	#endregion



	#region Camera Superior

	private static float[] m_camera_superior_info = new float[ 5 ];

	private static Camera m_main_camera;

	private static bool m_enable_camera_superior = false;

	void OnGUI_CameraSuperior(){
		if( !GetBool( CONST_SHOW_CAMERA_SUPERIOR ) ){
			return;
		}

		if( Camera.main == null ){
			return;
		}

		if( m_main_camera != Camera.main ){
			m_main_camera = Camera.main;

			CameraSuperiorForCamera t_camera = m_main_camera.gameObject.GetComponent<CameraSuperiorForCamera>();

			if( t_camera == null ){
				m_main_camera.gameObject.AddComponent<CameraSuperiorForCamera>();
			}
		}

		float t_slider_width_factor = 0.075f;

		float t_slider_offet_factor = 0.1f;

		{
			m_slider_rect_params[ 0 ] = Screen.width * ( 0.5f + t_slider_width_factor - t_slider_offet_factor * 3 );
			
			m_slider_rect_params[ 1 ] = Screen.height * 0.05f;
			
			m_slider_rect_params[ 2 ] = Screen.width * t_slider_width_factor;
			
			m_slider_rect_params[ 3 ] = Screen.height * 0.5f;
			
			m_slider_rect_params[ 4 ] = Screen.width * t_slider_offet_factor;
			
			m_slider_rect_params[ 5 ] = 0;
		}

		// check box
		{
			m_toggle_rect_params[ 0 ] = Screen.width * 0.5f;

			m_toggle_rect_params[ 1 ] = m_slider_rect_params[ 1 ] + m_slider_rect_params[ 3 ];

			m_toggle_rect_params[ 2 ] = Screen.width * 0.075f;

			m_toggle_rect_params[ 3 ] = Screen.height * 0.075f;

			m_toggle_rect_params[ 4 ] = m_slider_rect_params[ 4 ];

			m_toggle_rect_params[ 5 ] = m_slider_rect_params[ 5 ];

			bool t_m_new_enable = GUI.Toggle( UtilityTool.GetGUIRect( 0, m_toggle_rect_params ), 
			                                      m_enable_camera_superior, "SP" );

			if( t_m_new_enable && !m_enable_camera_superior ){
				m_main_camera_local_position = m_main_camera.gameObject.transform.localPosition;
					
				m_main_camera_local_rotation = m_main_camera.gameObject.transform.localEulerAngles;
			}

//			if( t_m_new_enable != m_enable_camera_superior ){
//				Debug.Log( "Camera Superior Toggled." );
//			}

			m_enable_camera_superior = t_m_new_enable;
		}

		if( !m_enable_camera_superior ){
			return;
		}

		// sliders
		{
			int t_index = 0;
			
			bool m_value_changed = false;
			
			for( int i = 0; i < m_camera_superior_info.Length; i++ ){
				float t_max = i <= 2 ? 0.75f : 1.5f;
				
				float t_min = i <= 2 ? -0.75f : -1.5f;
				
				float t_slider_value = GUI.VerticalSlider( UtilityTool.GetGUIRect( t_index++, m_slider_rect_params ),
				                                          m_camera_superior_info[ i ],
				                                          t_max, t_min );
				
				if( Mathf.Abs( t_slider_value - ( t_max + t_min ) / 2.0f ) <= ( t_max - t_min ) / 10.0f ){
					m_camera_superior_info[ i ] = 0;
				}
				else{
					m_camera_superior_info[ i ] = t_slider_value;
				}
			}
		}
	}

	private static Vector3 m_main_camera_local_position;
	
	private static Vector3 m_main_camera_local_rotation;

	public static void CameraSuperior(){
		if( !m_enable_camera_superior ){
			return;
		}

		if( !GetBool( CONST_SHOW_CAMERA_SUPERIOR ) ){
			return;
		}

		if( m_main_camera == null ){
			return;
		}

//		Debug.Log( "CameraSuperior()" );

		{
			m_main_camera_local_position = m_main_camera_local_position + new Vector3( m_camera_superior_info[ 0 ], m_camera_superior_info[ 1 ], m_camera_superior_info[ 2 ] );
			
			m_main_camera.gameObject.transform.localPosition = m_main_camera_local_position;
		}

		{
			m_main_camera_local_rotation = m_main_camera_local_rotation + new Vector3( m_camera_superior_info[ 3 ], m_camera_superior_info[ 4 ], 0 );

			m_main_camera.gameObject.transform.localEulerAngles = m_main_camera_local_rotation;
		}
	}

	#endregion



	#region GUI Particles

	private int m_particle_item_count = 3;
	
	private string[] m_particle_item_names = { 
		"",
		"",
		"",
	};

	void OnGUI_Particles(){
		if( !GetBool( CONST_SHOW_PARTICLE_CONTROLLERS ) ){
			return;
		}

		Vector2 t_offset = new Vector2( Screen.width * 0.1f, Screen.height * 0.0f );
		
		float[] m_btn_enable_params = {
			Screen.width * 0.0f + t_offset.x, Screen.height * 0.05f + t_offset.y,
			Screen.width * 0.095f, Screen.height * 0.06f,
			0, Screen.height * 0.1f };
		
		float[] m_btn_disable_params = {
			m_btn_enable_params[ 0 ] + Screen.width * 0.1f, m_btn_enable_params[ 1 ],
			m_btn_enable_params[ 2 ], m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ], };
		
		float[] m_text_field_params = {
			Screen.width * 0.2f + t_offset.x, m_btn_enable_params[ 1 ] + t_offset.y,
			Screen.width * 0.2f, m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ] };
		
		int t_row = 0;
		
		// all
		{
			if( GUI.Button( UtilityTool.GetGUIRect( t_row, m_btn_enable_params ), "En All", m_gui_btn_style ) ){
//				Debug.Log( "Enable All." );

				DebugParticles( true, "" );
			}
			
			if( GUI.Button( UtilityTool.GetGUIRect( t_row, m_btn_disable_params ), "Dis All", m_gui_btn_style ) ){
//				Debug.Log( "Disable All." );

				DebugParticles( false, "" );
			}
			
			t_row++;
		}
		
		// sub
		{
			for( int i = 0; i < m_particle_item_count; i++ ){
				string t_particle_name = GUI.TextField( UtilityTool.GetGUIRect( t_row, m_text_field_params ), 
				                                       m_particle_item_names[ i ], 
				                                       m_gui_text_field_style );

				m_particle_item_names[ i ] = t_particle_name;
				
				if( GUI.Button( UtilityTool.GetGUIRect( t_row, m_btn_enable_params ), "En", m_gui_btn_style ) ){
//					Debug.Log( "Enable: " + i + " - " + t_particle_name );

					DebugParticles( true, t_particle_name );
				}
				
				if( GUI.Button( UtilityTool.GetGUIRect( t_row, m_btn_disable_params ), "Dis", m_gui_btn_style ) ){
//					Debug.Log( "Disable: " + i + " - " + t_particle_name );

					DebugParticles( false, t_particle_name );
				}
				
				t_row++;
			}
		}
	}

	/// params:
	/// 
	/// p_particle_name: "" means all.
	private void DebugParticles( bool p_enable, string p_particle_name ){
		Object[] t_ps_array = GameObject.FindObjectsOfType( typeof(ParticleSystem) );

		for( int i = 0; i < t_ps_array.Length; i++ ){
			ParticleSystem t_ps = (ParticleSystem)t_ps_array[ i ];

			if( string.IsNullOrEmpty( p_particle_name ) ){
				t_ps.GetComponent<Renderer>().enabled = p_enable;
				
//				Debug.Log( "Set " + t_ps.gameObject.name + ": " + p_enable );

				continue;
			}

			if( t_ps.gameObject.name.ToLowerInvariant() == p_particle_name.ToLowerInvariant() ){
				t_ps.GetComponent<Renderer>().enabled = p_enable;

//				Debug.Log( "Set " + t_ps.gameObject.name + ": " + p_enable );
			}
		}

		// debug
//		{
//			Object[] t_audio_listeners = GameObject.FindObjectsOfType( typeof(AudioListener) );
//
//			for( int i = 0; i < t_audio_listeners.Length; i++ ){
//				AudioListener t_as = (AudioListener)t_audio_listeners[ i ];
//
//				Debug.Log( "Audio Listener " + i + ": " + t_as.gameObject.name );
//			}
//		}
	}

	#endregion



	#region Load

	void CleanData(){
		#if DEBUG_CONFIG
		Debug.Log( "ConfigTool.CleanData()" );
		#endif

		m_config_dict.Clear();
		
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
		Debug.Log( "ConfigTool.ResourceLoadCallback()" );
		#endif

		if ( m_config_dict.Count > 0 && m_config_value_dict.Count > 0 ) {
			return;
		}

		{
			TextAsset t_text = ( TextAsset )p_object;

			UtilityTool.LoadStringStringDict( m_config_dict, t_text, CONST_LINE_SPLITTER );
		}

		// Load Items
		{
			LoadConfigItems();
		}

		// Exe Items
		{
			ExeConfigItems();
		}
	}
	
	private void LoadConfigItems(){
//		Debug.Log( "ConfigTool.LoadConfigItems." );

		// version
		if( ContainsKey( m_config_dict, CONST_VERSION ) ){
			LoadValues( m_config_value_dict, CONST_VERSION, LoadStringValue( m_config_dict, CONST_VERSION ) );

			LoadValues( m_config_value_dict, CONST_SHOW_VERSION, LoadBoolValue( m_config_dict, CONST_SHOW_VERSION ) );
		}

		// debug
		{
			LoadValues( m_config_value_dict, CONST_SHOW_CONSOLE, LoadBoolValue( m_config_dict, CONST_SHOW_CONSOLE ) );

			LoadValues( m_config_value_dict, CONST_COMMON_CODE_EXCEPTION, LoadBoolValue( m_config_dict, CONST_COMMON_CODE_EXCEPTION ) );

			LoadValues( m_config_value_dict, CONST_NETWORK_CLOSE_SWITCHER, LoadBoolValue( m_config_dict, CONST_NETWORK_CLOSE_SWITCHER ) );

			LoadValues( m_config_value_dict, CONST_MANUAL_CLEAN, LoadBoolValue( m_config_dict, CONST_MANUAL_CLEAN ) );

			LoadValues( m_config_value_dict, CONST_SHOW_MAIN_CAMERA_INFO, LoadBoolValue( m_config_dict, CONST_SHOW_MAIN_CAMERA_INFO ) );

			LoadValues( m_config_value_dict, CONST_SHOW_PARTICLE_CONTROLLERS, LoadBoolValue( m_config_dict, CONST_SHOW_PARTICLE_CONTROLLERS ) );

			LoadValues( m_config_value_dict, CONST_SHOW_CAMERA_SUPERIOR, LoadBoolValue( m_config_dict, CONST_SHOW_CAMERA_SUPERIOR ) );

			LoadValues( m_config_value_dict, CONST_SHOW_QUALITY_SWITCH, LoadBoolValue( m_config_dict, CONST_SHOW_QUALITY_SWITCH ) );

			LoadValues( m_config_value_dict, CONST_SHOW_CURRENT_LOADING, LoadBoolValue( m_config_dict, CONST_SHOW_CURRENT_LOADING ) );

			LoadValues( m_config_value_dict, CONST_SHOW_DEVICE_INFO, LoadBoolValue( m_config_dict, CONST_SHOW_DEVICE_INFO ) );

			//LoadValues( m_config_value_dict, CONST_OPEN_CHECK_XML_TOOL, LoadBoolValue( m_config_dict, CONST_OPEN_CHECK_XML_TOOL ) );

			LoadValues( m_config_value_dict, CONST_QUICK_PAUSE, LoadBoolValue( m_config_dict, CONST_QUICK_PAUSE ) );
			
			LoadValues( m_config_value_dict, CONST_QUICK_FX, LoadBoolValue( m_config_dict, CONST_QUICK_FX ) );
		}

		// fps
		{
			// show fps
			{
				LoadValues( m_config_value_dict, CONST_SHOW_FPS, LoadBoolValue( m_config_dict, CONST_SHOW_FPS ) );
			}
			
			// target fps
			if( ContainsKey( m_config_dict, CONST_TARGET_FPS ) ){
				LoadValues( m_config_value_dict, CONST_TARGET_FPS, LoadIntValue( m_config_dict, CONST_TARGET_FPS ) );

				ResetFPS();
			}
			else{
				// default
				QualitySettings.vSyncCount = 1;
				
				Application.targetFrameRate = 60;
			}

			LoadValues( m_config_value_dict, CONST_LOADING_INTERVAL, LoadFloatValue( m_config_dict, CONST_LOADING_INTERVAL ) );
		}

		// guide
		{
			LoadValues( m_config_value_dict, CONST_OPEN_GUIDE_EDITOR, LoadBoolValue( m_config_dict, CONST_OPEN_GUIDE_EDITOR ) );

			LoadValues( m_config_value_dict, CONST_SHOW_GUIDE_SWITCHER, LoadBoolValue( m_config_dict, CONST_SHOW_GUIDE_SWITCHER ) );

			LoadValues( m_config_value_dict, CONST_OPEN_ALLTHE_FUNCTION, LoadBoolValue( m_config_dict, CONST_OPEN_ALLTHE_FUNCTION ) );
		}
		
		// battle field
		{
			LoadValues( m_config_value_dict, CONST_QUICK_CHOOSE_LEVEL, LoadBoolValue( m_config_dict, CONST_QUICK_CHOOSE_LEVEL ) );

			LoadValues( m_config_value_dict, CONST_QUICK_FIGHT, LoadBoolValue( m_config_dict, CONST_QUICK_FIGHT ) );

			LoadValues( m_config_value_dict, CONST_SHOW_BATTLE_CAMERA_OPS, LoadBoolValue( m_config_dict, CONST_SHOW_BATTLE_CAMERA_OPS ) );
		}
		
		// network emulate
		{
			LoadValues( m_config_value_dict, CONST_NETWORK_CHECK_TIME, LoadFloatValue( m_config_dict, CONST_NETWORK_CHECK_TIME ) );

			LoadValues( m_config_value_dict, CONST_NETOWRK_SOCKET_TIME_OUT, LoadFloatValue( m_config_dict, CONST_NETOWRK_SOCKET_TIME_OUT ) );

			#if UNITY_EDITOR || UNITY_STANDALONE
			m_is_emulating_latency = IsEmulatingNetworkLatency();
			
			m_emulate_network_latency = GetEmulatingNetworkLatency();
			
			#elif UNITY_IPHONE || UNITY_ANDROID
			m_is_emulating_latency = false;
			
			m_emulate_network_latency = 0;
			
			#else
			Debug.Log( "#else()" );
			#endif

			LoadValues( m_config_value_dict, CONST_NETWORK_SHOW_STATUS, LoadBoolValue( m_config_dict, CONST_NETWORK_SHOW_STATUS ) );

			LoadValues( m_config_value_dict, CONST_SHOW_NETWORK_SELECTOR, LoadBoolValue( m_config_dict, CONST_SHOW_NETWORK_SELECTOR ) );
		}
		
		// logs
		{
			LoadValues( m_config_value_dict, CONST_LOG_HTTP_STATUS, LoadBoolValue( m_config_dict, CONST_LOG_HTTP_STATUS ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_SEND, LoadBoolValue( m_config_dict, CONST_LOG_SOCKET_SEND ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_SEND_DETIAL, LoadBoolValue( m_config_dict, CONST_LOG_SOCKET_SEND_DETIAL ) );


			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_RECEIVE, LoadBoolValue( m_config_dict, CONST_LOG_SOCKET_RECEIVE ) );

			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_RECEIVE_DETAIL, LoadBoolValue( m_config_dict, CONST_LOG_SOCKET_RECEIVE_DETAIL ) );


			LoadValues( m_config_value_dict, CONST_LOG_SOCKET_PROCESSOR, LoadBoolValue( m_config_dict, CONST_LOG_SOCKET_PROCESSOR ) );

			LoadValues( m_config_value_dict, CONST_LOG_MAINCITY_SPRITE_MOVE, LoadBoolValue( m_config_dict, CONST_LOG_MAINCITY_SPRITE_MOVE ) );

			LoadValues( m_config_value_dict, CONST_LOG_ASSET_LOADING, LoadBoolValue( m_config_dict, CONST_LOG_ASSET_LOADING ) );


			LoadValues( m_config_value_dict, CONST_LOG_TOTAL_LOADING_TIME, LoadBoolValue( m_config_dict, CONST_LOG_TOTAL_LOADING_TIME ) );

			LoadValues( m_config_value_dict, CONST_LOG_ITEM_LOADING_TIME, LoadBoolValue( m_config_dict, CONST_LOG_ITEM_LOADING_TIME ) );

			LoadValues( m_config_value_dict, CONST_LOG_BUNDLE_DOWNLOADING, LoadBoolValue( m_config_dict, CONST_LOG_BUNDLE_DOWNLOADING ) );

			LoadValues( m_config_value_dict, CONST_LOG_DIALOG_BOX, LoadBoolValue( m_config_dict, CONST_LOG_DIALOG_BOX ) );


			LoadValues( m_config_value_dict, CONST_LOG_QUALITY_CONFIG, LoadBoolValue( m_config_dict, CONST_LOG_QUALITY_CONFIG ) );
		}
		
		// bundle
		{
			LoadValues( m_config_value_dict, CONST_CLEAN_EDITOR_CACHE, LoadBoolValue( m_config_dict, CONST_CLEAN_EDITOR_CACHE ) );
		}
	}

	private void ExeConfigItems(){
		// network
		{
			ResetNetworkShowWaiting();
		}
		
		// bundle
		{
			#if UNITY_EDITOR
			
			if( GetBool( CONST_CLEAN_EDITOR_CACHE ) ){
				Prepare_Bundle_Cleaner.CleanCache();
			}
			
			#endif
		}
	}

	private void ResetNetworkShowWaiting(){
		NetworkWaiting.SetShowWaiting( GetBool( CONST_NETWORK_SHOW_STATUS ) );
	}

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
			Debug.Log( "Config.Key not contained: " + p_key );
			
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
			//			Debug.Log( "Config.Key not contained: " + p_key );
			
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
			Debug.Log( "Config.Key not contained: " + p_key );
			
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
			Debug.Log( "Config.Key not contained: " + p_key );
			
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

		if( ContainsKey( m_config_dict, CONST_NETWORK_LATENCY ) ){
			float t_latency = LoadFloatValue( m_config_dict, CONST_NETWORK_LATENCY );

			if( t_latency > 0 ){
				t_emulating = true;
			}
		}

		return t_emulating;
	}

	#endregion


	#region Get Target Value

	public static float GetEmulatingNetworkLatency(){
		if( ContainsKey( m_config_dict, CONST_NETWORK_LATENCY ) ){
			return LoadFloatValue( m_config_dict, CONST_NETWORK_LATENCY );
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



	#region ServerType
	
	public enum ServerType{
		NeiWang = 0,
		TiYan,
		CeShi,
	}

	private static ServerType m_server_type_enum = ServerType.CeShi;

	// login use
	public static ServerType GetDefaultServerType(){
		return ServerType.CeShi;
	}

	// login use
	public static SelectUrl.UrlSeclect GetDefaultLoginServerType(){
		return SelectUrl.UrlSeclect.CeShi;
	}

	// login use
	public static string GetDefaultServerName(){
		return "测试服";
	}

	public static ServerType GetServerType(){
		return m_server_type_enum;
	}

	public static void SetServerType( ServerType p_type ){
		m_server_type_enum = p_type;

//		Debug.Log ( "ServerType.Setted: " + p_type );
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

	public const string CONST_VERSION				= "Version";

	#endregion



	#region Debug Keys

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

	public const string CONST_NETOWRK_SOCKET_TIME_OUT	= "SocketTimeOut";
	
	public const string CONST_NETWORK_LATENCY			= "NetworkLatency";

	public const string CONST_NETWORK_SHOW_STATUS		= "ShowNetworkStatus";

	public const string CONST_SHOW_NETWORK_SELECTOR 	= "ShowServerSelector";

	#endregion



	#region Log Keys

	public const string CONST_LOG_HTTP_STATUS			= "LogHttpStatus";

	public const string CONST_LOG_SOCKET_SEND			= "LogSocketSend";
	public const string CONST_LOG_SOCKET_SEND_DETIAL	= "LogSocketSendDetail";

	public const string CONST_LOG_SOCKET_RECEIVE		= "LogSocketReceive";
	public const string CONST_LOG_SOCKET_RECEIVE_DETAIL	= "LogSocketReceiveDetail";

	public const string CONST_LOG_SOCKET_PROCESSOR		= "LogSocketProcessor";

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



	#region Camera Superior For Camera

	public class CameraSuperiorForCamera : MonoBehaviour{

		void Awake(){

		}

		void Start(){

		}

		void Update(){

		}

		void OnPreRender() {
			ConfigTool.CameraSuperior();
		}

	}

	#endregion
}
