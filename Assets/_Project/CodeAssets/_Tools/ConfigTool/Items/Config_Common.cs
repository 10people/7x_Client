using UnityEngine;
using System.Collections;

public class Config_Common {

	#region Mono

	public static void OnGUI(){
		{
			InitGUI();
		}
		
		{
			OnGUI_Common();
		}
		

	}

	#endregion



	#region GUI Common

	private static void OnGUI_Common(){
		
		{
			OnGUI_Common_Info();
		}
		
		{
			OnGUI_Common_Debug();
		}
	}
	
	private static bool m_device_info_logged = false;
	
	private static void OnGUI_Common_Info(){
		if( ConfigTool.GetBool( ConfigTool.CONST_SHOW_VERSION ) ){
			if( ConfigTool.GetBool( ConfigTool.CONST_SHOW_CONSOLE ) ) {
				{
					SetPingColor();

					GUI.Label( new Rect( ScreenHelper.GetX( 0.1f ), ScreenHelper.GetY( 0.8f ), 250, 35 ), 
					          NetworkHelper.GetPingMS() + " ms", GUIHelper.m_gui_lb_style );
					
					RestoreColor();
				}

				GUI.Label( new Rect( 0, ScreenHelper.GetY( 0.9f ), 250, 35 ), 
				          ConfigTool.GetString( ConfigTool.CONST_VERSION ), GUIHelper.m_gui_lb_style );
			}
			else{
				GUI.Label( new Rect( ScreenHelper.GetX( 0.0f ), ScreenHelper.GetY( 0.0f ), 250, 35 ), 
				          ConfigTool.GetString( ConfigTool.CONST_VERSION ), GUIHelper.m_gui_lb_style );

				{
					SetPingColor();

					GUI.Label( new Rect( ScreenHelper.GetX( 0.1f ), ScreenHelper.GetY( 0.9f ), 250, 35 ), 
					          NetworkHelper.GetPingMS() + " ms", GUIHelper.m_gui_lb_style );

					RestoreColor();
				}
			}

//			GUI.Label( new Rect( 0, 25, 250, 35 ), "BV: " + Prepare_Bundle_Config.m_config_cached_small_version, m_gui_lb_style );
//
//			GUI.Label( new Rect( 0, 50, 250, 35 ), "SV: " + Prepare_Bundle_Config.CONFIG_BIG_VESION, m_gui_lb_style );
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_SHOW_DEVICE_INFO ) ){
			int t_info_index = 0;
			
			GUIHelper.m_lb_rect_params[ 0 ] = 0;
			
			GUIHelper.m_lb_rect_params[ 1 ] = 40;
			
			GUIHelper.m_lb_rect_params[ 2 ] = Screen.width;
			
			GUIHelper.m_lb_rect_params[ 3 ] = 35;
			
			GUIHelper.m_lb_rect_params[ 4 ] = 0;
			
			GUIHelper.m_lb_rect_params[ 5 ] = 40;
			
			#if UNITY_IOS
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "iGen: " + UnityEngine.iOS.Device.generation.ToString(), GUIHelper.m_gui_lb_style );
			#endif
			
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "Model: " + SystemInfo.deviceModel + " - Name: " + SystemInfo.deviceName, GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "G.Name: " + SystemInfo.graphicsDeviceName, GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "G.Mem: " + SystemInfo.graphicsMemorySize + " - P.FR: " + SystemInfo.graphicsPixelFillrate, GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "S.Mem: " + SystemInfo.systemMemorySize + " - G.DV: " + SystemInfo.graphicsDeviceVersion, GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( t_info_index++, GUIHelper.m_lb_rect_params ), "G: " + SystemInfo.processorType + " - G.Num: " + SystemInfo.processorCount, GUIHelper.m_gui_lb_style );
			
			if( !m_device_info_logged ){
				m_device_info_logged = true;
				
				Console_SetConfig.LogDeviceInfo();
			}
		}
	}
	
	private static void OnGUI_Common_Debug(){
		{
			GUIHelper.m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			GUIHelper.m_btn_rect_params[ 1 ] = Screen.height * 0.2f;
			
			GUIHelper.m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			GUIHelper.m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			GUIHelper.m_btn_rect_params[ 4 ] = 0;
			
			GUIHelper.m_btn_rect_params[ 5 ] = Screen.height * 0.095f;
		}
		
		int t_button_index = 0;
		
		if( ConfigTool.GetBool( ConfigTool.CONST_NETWORK_CLOSE_SWITCHER ) ){
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, GUIHelper.m_btn_rect_params ), "Close Socket", GUIHelper.m_gui_btn_style ) ){
				SocketTool.Instance().SetSocketLost();
			}
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_MANUAL_CLEAN ) ){
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, GUIHelper.m_btn_rect_params ), "Manual Clean", GUIHelper.m_gui_btn_style ) ){
				Debug.Log( "Manual Clean." );
				
				Resources.UnloadUnusedAssets();
			}
		}
		
		if( ConfigTool.GetBool( ConfigTool.CONST_COMMON_CODE_EXCEPTION ) ){
			string t_error = DebugHelper.GetCommonCodeError();
			
			if( !string.IsNullOrEmpty( t_error ) ){
				// scroll view
				{
					DebugHelper.m_common_code_scroll_pos = GUI.BeginScrollView( 
					                                                           new Rect( 0, 110, Screen.width * 0.8f, Screen.height * 0.5f ),
					                                                           DebugHelper.m_common_code_scroll_pos,
					                                                           new Rect( 0, 0, Screen.width, Screen.height ) );
					
					GUI.Label( new Rect( 0, 0, Screen.width, Screen.height ), DebugHelper.GetCommonCodeError() );
					
					GUI.EndScrollView();
				}
				
				// control
				{
					if( GUI.Button( new Rect( 300, 0, 100, 50 ), "Clear" ) ){
						DebugHelper.ClearCommonCodeError();
					}
				}
			}
		}
	}
	
	#endregion



	#region GUI ping

	private const long CONST_MAX_LOW_PING		= 150;

	private const long CONST_MAIN_HIGH_PING		= 300;

	private static Color m_low_color		= new Color( 55.0f / 255, 213f / 255, 7f / 255 );

	private static Color m_mid_color		= new Color( 249.0f / 255, 196f / 255, 20f / 255 );

	private static Color m_high_color		= new Color( 255.0f / 255, 25f / 255, 25f / 255 );

	private static Color m_pre_color		= new Color( 55.0f / 255, 213f / 255, 7f / 255 );

	private static void SetPingColor(){
		m_pre_color = GUIHelper.m_gui_lb_style.normal.textColor;

		if( NetworkHelper.GetPingMS() >= CONST_MAIN_HIGH_PING ){
			GUIHelper.m_gui_lb_style.normal.textColor = m_high_color;
		}
		else if( NetworkHelper.GetPingMS() >= CONST_MAX_LOW_PING ){
			GUIHelper.m_gui_lb_style.normal.textColor = m_mid_color;
		}
		else{
			GUIHelper.m_gui_lb_style.normal.textColor = m_low_color;
		}
	}

	private static void RestoreColor(){
		GUIHelper.m_gui_lb_style.normal.textColor = m_pre_color;;
	}

	#endregion



	#region Utilities

	private static void InitGUI(){
		{
			if( GUIHelper.m_gui_lb_style == null ){
				//				m_gui_lb_style = new GUIStyle( GUI.skin.label );
				GUIHelper.m_gui_lb_style = GUI.skin.label;
			}
			
			if( GUIHelper.m_gui_btn_style == null ){
				//				m_gui_btn_style = new GUIStyle( GUI.skin.button );
				
				GUIHelper.m_gui_btn_style = GUI.skin.button;
			}
			
			if( GUIHelper.m_gui_text_field_style == null ){
				//				m_gui_text_field_style = new GUIStyle( GUI.skin.textField );
				
				GUIHelper.m_gui_text_field_style = GUI.skin.textField;
			}
		}
		
		{
			GUIHelper.m_gui_lb_style.fontSize = 18;
			
			GUIHelper.m_gui_lb_style.normal.textColor = Color.white;
		}
		
		{
			GUIHelper.m_gui_btn_style.fontSize = 18;
		}
		
		{
			GUIHelper.m_gui_text_field_style.fontSize = 18;
		}
	}

	#endregion

}
