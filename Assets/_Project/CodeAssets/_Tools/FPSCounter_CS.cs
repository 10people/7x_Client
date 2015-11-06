using UnityEngine;
using System.Collections;

public class FPSCounter_CS : MonoBehaviour {

	public float m_update_interval = 0.5f;

	public Vector2 m_pos_vector = new Vector2( 0.8f, 0.8f );

	public int m_font_size 		= 18;

	public int m_fps_warnning	= 20;

	public Color m_normal_color		= new Color( 55.0f / 255, 213f / 255, 7f / 255 );

	public Color m_warning_color 	= new Color( 255.0f / 255, 25f / 255, 25f / 255 );
	
	private float m_last_interval = 0f;

	private int m_frame_count 	= 0;

	private float m_fps 		= 0.0f;

	private string m_fps_str	= "";

	private GUIStyle m_gui_style = new GUIStyle();


	#region Mono

	void  Awake (){
		useGUILayout = false;

//		Application.targetFrameRate = 60;
	}
	
	
	void  OnGUI (){
		if( !ConfigTool.GetBool( ConfigTool.CONST_SHOW_FPS ) ){
			return;
		}

		if( m_fps > m_fps_warnning ){
			m_gui_style.normal.textColor = m_normal_color;
		}
		else{
			m_gui_style.normal.textColor = m_warning_color;
		}

		m_gui_style.fontSize = m_font_size;

		GUI.Label ( new Rect( Screen.width * m_pos_vector.x, Screen.height * m_pos_vector.y, 100, 30 ), 
		           "FPS: " + m_fps_str,
		           m_gui_style );
	}

	void  Start (){
		m_last_interval = Time.realtimeSinceStartup;

		m_frame_count = 0;
	}
	
	private float m_cur_time = 0.0f;

	void Update(){
		++m_frame_count;

		m_cur_time = Time.realtimeSinceStartup;

		if( m_cur_time > m_last_interval + m_update_interval ){
			m_fps = m_frame_count / ( m_cur_time - m_last_interval );

			m_fps_str = m_fps.ToString( "f2" );

			m_frame_count = 0;

			m_last_interval = m_cur_time;
		}
	}

	#endregion



	#region Utilities

	public string GetFPSString(){
		return m_fps_str;
	}

	public float GetFPSFloat(){
		return m_fps;
	}

	public int GetFPSInt(){
		return (int)m_fps;
	}

	#endregion
}