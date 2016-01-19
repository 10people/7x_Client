
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;



public class Console_SetConfig {

	#region Mono

	public static void OnGUI(){
		ShowPause();

		ShowQuickFx();
	}
	
	private static void ShowPause(){
		bool t_show = ConfigTool.GetBool( ConfigTool.CONST_QUICK_PAUSE );
		
		if ( !t_show ) {
			return;
		}
		
		if( GUI.Button( new Rect( 0, ScreenHelper.GetY( 0.1f ), ScreenHelper.GetX( 0.15f ), ScreenHelper.GetY( 0.1f ) ), 
		               ( Time.timeScale > 0 ? "Pause" : "Resume" ) ) ){
			ExecPause();
		}
	}
	
	private static bool m_show_fx = true;
	
	private static void ShowQuickFx(){
		bool t_show = ConfigTool.GetBool( ConfigTool.CONST_QUICK_FX );
		
		if ( !t_show ) {
			return;
		}
		
		if( GUI.Button( new Rect( ScreenHelper.GetX( 0.2f ), ScreenHelper.GetY( 0.1f ), ScreenHelper.GetX( 0.2f ), ScreenHelper.GetY( 0.1f ) ), 
		               m_show_fx ? "Fx.Hide" : "Fx.Show" ) ){
			m_show_fx = !m_show_fx;
			
			string[] t_params = { "", ConsoleTool.COM_TYPE_PARTICLE_SYSTEM };
			
			if( m_show_fx ){
				t_params[ 0 ] = ConsoleTool.ENABLE_COMPONENT;
				Console_Component.EnableComponent( t_params );
			}
			else{
				t_params[ 0 ] = ConsoleTool.DISABLE_COMPONENT;
				
				Console_Component.DisableComponent( t_params );
			}
		}
	}
	
	#endregion
	


	#region Pause

	private static void ExecPause(){
		if ( Time.timeScale > 0 ) {
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}
		
		//		Debug.Log ( "Switch TimeScale to: " + Time.timeScale );
	}
	

	#endregion


	
	#region Set Config Tool
	
	/// Emp:
	/// SetConfigTool ShowConsole false
	public static void SetConfigTool( string[] p_params ){
		if( p_params.Length < 3 ){
			Debug.Log( "Length Not Enough." );
			
			return;
		}
		
		string t_target_key = p_params[ 1 ].ToLowerInvariant();
		
		foreach( KeyValuePair<string, ConfigTool.ConfigValue> t_pair in ConfigTool.m_config_value_dict ){
			string t_key = t_pair.Key.ToLowerInvariant();
			
			if( t_key == t_target_key ){
				Debug.Log( "Config.Set( " + t_key + " - " + p_params[ 2 ] + " )" );
				
				t_pair.Value.AutoSet( p_params[ 2 ] );
				
				return;
			}
		}
		
		{
			Debug.LogError( "Key Not Found: " + p_params[ 1 ] );
		}
	}
	
	#endregion
	
	
	
	#region Log Device Info
	
	public static void LogDeviceInfo(){
		DeviceHelper.LogDeviceInfo( null );
	}
	
	#endregion
	
	
	
	#region Log Fps
	
	public static void LogFps( string[] p_params ){
		ComponentHelper.AddIfNotExist( GameObjectHelper.GetDontDestroyOnLoadGameObject(), 
		                              typeof(FPSCounter_CS) );
		
		Debug.Log( "TargetFrameRate: " + Application.targetFrameRate );
		
		Debug.Log( "vSyncCount: " + QualitySettings.vSyncCount );
	}
	
	#endregion
	
	
	
	#region Log Config
	
	public static void LogConfig( string[] p_params ){
		ConfigTool.LogConfigs();
	}
	
	#endregion
}
