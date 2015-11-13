//#define LOG_QUALITY

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
	public static Dictionary<string, string> m_config_xml_dict = new Dictionary<string, string>();


	public const char CONST_SECTION_SPLITTER		= '=';

	public const char CONST_DEVICE_SPLITTER		= '/';


	private Shader m_blade_effect = null;

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
		if( !DeviceHelper.Is_iOS_Target_Device() ){
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
		if( !DeviceHelper.Is_iOS_Target_Device() ){
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
		if( !DeviceHelper.Is_iOS_Target_Device() ){
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

	#endregion



	#region Scene Fx Level

	public const string CONST_SCENE_FX_LEVEL_NONE		= "None";

	public const string CONST_SCENE_FX_LEVEL_LOW		= "Low";

	public const string CONST_SCENE_FX_LEVEL_MEDIUM		= "Medium";

	public const string CONST_SCENE_FX_LEVEL_HIGH		= "High";

	#endregion
}