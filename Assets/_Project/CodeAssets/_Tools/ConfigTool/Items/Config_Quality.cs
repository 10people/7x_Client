using UnityEngine;
using System.Collections;

public class Config_Quality {

	#region Mono

	public static void OnGUI(){
		OnGUI_Quality();
	}

	#endregion


	#region Quality Set
	
	private static bool m_light_on = false;
	
	private static void OnGUI_Quality(){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_QUALITY_SWITCH ) ){
			return;
		}

		int t_btn_index = 0;
		
		{
			GUIHelper.m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			GUIHelper.m_btn_rect_params[ 1 ] = Screen.height * 0.1f;
			
			GUIHelper.m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			GUIHelper.m_btn_rect_params[ 3 ] = Screen.height * 0.11f;
			
			GUIHelper.m_btn_rect_params[ 4 ] = 0;
			
			GUIHelper.m_btn_rect_params[ 5 ] = Screen.height * 0.125f;
		}
		
		bool t_fog = RenderSettings.fog;
		
		if( GUI.Button( GUIHelper.GetGUIRect( t_btn_index++, GUIHelper.m_btn_rect_params ), 
		               m_light_on ? "L On" : "L Off" ) ){
			m_light_on = !m_light_on;
			
			Quality_Shadow.ConfigLights( m_light_on );
		}
		
		if( GUI.Button( GUIHelper.GetGUIRect( t_btn_index++, GUIHelper.m_btn_rect_params ), 
		               ConfigTool.m_config_value_dict[ ConfigTool.CONST_SHOW_CAMERA_SUPERIOR ].m_bool ? "Cam On" : "Cam Off" ) ){
			ConfigTool.m_config_value_dict[ ConfigTool.CONST_SHOW_CAMERA_SUPERIOR ].m_bool = !ConfigTool.m_config_value_dict[ ConfigTool.CONST_SHOW_CAMERA_SUPERIOR ].m_bool;
		}
		
		if( GUI.Button( GUIHelper.GetGUIRect( t_btn_index++, GUIHelper.m_btn_rect_params ), 
		               m_bloom_on ? "B On" : "B Off" ) ){
			m_bloom_on = !m_bloom_on;
			
			Quality_Common.ConfigBloom( m_bloom_on );
		}
		
		if( GUI.Button( GUIHelper.GetGUIRect( t_btn_index++, GUIHelper.m_btn_rect_params ), 
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
	
	private static bool m_bloom_on = false;
	
	#endregion
	

}
