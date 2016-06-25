using UnityEngine;
using System.Collections;

public class Config_Camera {

	#region Mono

	public static void OnGUI(){
		{
			OnGUI_CameraInfo();
		}
		
		{
			OnGUI_CameraSuperior();
		}

	}

	#endregion



	#region GUI Camera Info
	
	private static void OnGUI_CameraInfo(){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_MAIN_CAMERA_INFO ) ){
			return;
		}
		
		if( Camera.main == null ){
			return;
		}
		
		{
			GUIHelper.m_lb_rect_params[ 0 ] = Screen.width * 0.05f;
			
			GUIHelper.m_lb_rect_params[ 1 ] = Screen.height * 0.85f;
			
			GUIHelper.m_lb_rect_params[ 2 ] = Screen.width * 0.075f;
			
			GUIHelper.m_lb_rect_params[ 3 ] = Screen.height * 0.06f;
			
			GUIHelper.m_lb_rect_params[ 4 ] = Screen.width * 0.075f;
			
			GUIHelper.m_lb_rect_params[ 5 ] = 0;
		}
		
		{
			Vector3 m_pos = Camera.main.transform.position;
			
			GUI.Label( GUIHelper.GetGUIRect( 0, 
			                                0, GUIHelper.m_lb_rect_params[ 1 ],
			                                GUIHelper.m_lb_rect_params[ 2 ], GUIHelper.m_lb_rect_params[ 3 ],
			                                GUIHelper.m_lb_rect_params[ 4 ], GUIHelper.m_lb_rect_params[ 5 ] ), 
			          "T: ", 
			          GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 0, GUIHelper.m_lb_rect_params ), m_pos.x.ToString( "f2" ), GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 1, GUIHelper.m_lb_rect_params ), m_pos.y.ToString( "f2" ), GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 2, GUIHelper.m_lb_rect_params ), m_pos.z.ToString( "f2" ), GUIHelper.m_gui_lb_style );
		}
		
		{
			GUIHelper.m_lb_rect_params[ 1 ] = GUIHelper.m_lb_rect_params[ 1 ] + Screen.height * 0.075f;
			
			Quaternion m_quaternion = Camera.main.transform.rotation;
			
			GUI.Label( GUIHelper.GetGUIRect( 0, 
			                                0, GUIHelper.m_lb_rect_params[ 1 ],
			                                GUIHelper.m_lb_rect_params[ 2 ], GUIHelper.m_lb_rect_params[ 3 ],
			                                GUIHelper.m_lb_rect_params[ 4 ], GUIHelper.m_lb_rect_params[ 5 ] ), 
			          "R: ", 
			          GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 0, GUIHelper.m_lb_rect_params ), m_quaternion.eulerAngles.x.ToString( "f2" ), GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 1, GUIHelper.m_lb_rect_params ), m_quaternion.eulerAngles.y.ToString( "f2" ), GUIHelper.m_gui_lb_style );
			
			GUI.Label( GUIHelper.GetGUIRect( 2, GUIHelper.m_lb_rect_params ), m_quaternion.eulerAngles.z.ToString( "f2" ), GUIHelper.m_gui_lb_style );
		}
	}
	
	#endregion



	#region Camera Superior
	
	private static float[] m_camera_superior_info = new float[ 5 ];
	
	private static Camera m_main_camera;
	
	private static bool m_enable_camera_superior = false;
	
	private static void OnGUI_CameraSuperior(){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_CAMERA_SUPERIOR ) ){
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
			GUIHelper.m_slider_rect_params[ 0 ] = Screen.width * ( 0.5f + t_slider_width_factor - t_slider_offet_factor * 3 );
			
			GUIHelper.m_slider_rect_params[ 1 ] = Screen.height * 0.05f;
			
			GUIHelper.m_slider_rect_params[ 2 ] = Screen.width * t_slider_width_factor;
			
			GUIHelper.m_slider_rect_params[ 3 ] = Screen.height * 0.5f;
			
			GUIHelper.m_slider_rect_params[ 4 ] = Screen.width * t_slider_offet_factor;
			
			GUIHelper.m_slider_rect_params[ 5 ] = 0;
		}
		
		// check box
		{
			GUIHelper.m_toggle_rect_params[ 0 ] = Screen.width * 0.5f;
			
			GUIHelper.m_toggle_rect_params[ 1 ] = GUIHelper.m_slider_rect_params[ 1 ] + GUIHelper.m_slider_rect_params[ 3 ];
			
			GUIHelper.m_toggle_rect_params[ 2 ] = Screen.width * 0.075f;
			
			GUIHelper.m_toggle_rect_params[ 3 ] = Screen.height * 0.075f;
			
			GUIHelper.m_toggle_rect_params[ 4 ] = GUIHelper.m_slider_rect_params[ 4 ];
			
			GUIHelper.m_toggle_rect_params[ 5 ] = GUIHelper.m_slider_rect_params[ 5 ];
			
			bool t_m_new_enable = GUI.Toggle( GUIHelper.GetGUIRect( 0, GUIHelper.m_toggle_rect_params ), 
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
				
				float t_slider_value = GUI.VerticalSlider( GUIHelper.GetGUIRect( t_index++, GUIHelper.m_slider_rect_params ),
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
		
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_CAMERA_SUPERIOR ) ){
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



	#region Utilities

	#endregion



	#region Camera Superior For Camera
	
	public class CameraSuperiorForCamera : MonoBehaviour{
		
		void Awake(){
			
		}
		
		void Start(){
			
		}

		void OnPreRender() {
			CameraSuperior();
		}
		
	}
	
	#endregion
}
