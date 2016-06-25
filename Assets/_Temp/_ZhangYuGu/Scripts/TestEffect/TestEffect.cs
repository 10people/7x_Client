using UnityEngine;
using System.Collections;

public class TestEffect : MonoBehaviour {

	void Awake(){
//		EffectTemplate.LoadTemplates();

		EffectIdTemplate.LoadTemplates();
	}

	private GUIStyle m_gui_lb_style;
	
	private GUIStyle m_gui_btn_style;
	
	private GUIStyle m_gui_text_field_style;

	private string m_particle_id = "";

	void OnGUI(){
		if( m_gui_lb_style == null ){
			m_gui_lb_style = new GUIStyle( GUI.skin.label );
		}
		
		if( m_gui_btn_style == null ){
			m_gui_btn_style = new GUIStyle( GUI.skin.button );
		}
		
		if( m_gui_text_field_style == null ){
			m_gui_text_field_style = new GUIStyle( GUI.skin.textField );
		}

		Vector2 t_offset = new Vector2( Screen.width * 0.1f, Screen.height * 0.0f );

		float[] m_btn_enable_params = {
			Screen.width * 0.0f + t_offset.x, Screen.height * 0.05f + t_offset.y,
			Screen.width * 0.125f, Screen.height * 0.08f,
			0, Screen.height * 0.1f };
		
		float[] m_btn_disable_params = {
			m_btn_enable_params[ 0 ] + Screen.width * 0.1f, m_btn_enable_params[ 1 ],
			m_btn_enable_params[ 2 ], m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ], };
		
		float[] m_text_field_params = {
			Screen.width * 0.2f + t_offset.x, m_btn_enable_params[ 1 ] + t_offset.y,
			Screen.width * 0.2f, m_btn_enable_params[ 3 ],
			0, m_btn_enable_params[ 5 ] };

		m_particle_id = GUI.TextField( GUIHelper.GetGUIRect( 0, m_text_field_params ), 
		                                m_particle_id, 
		               					m_gui_text_field_style );

		if( GUI.Button( GUIHelper.GetGUIRect( 0, m_btn_enable_params ), "Launch", m_gui_btn_style ) ){
			LaunchParticle( m_particle_id );
		}
	}

	private void LaunchParticle( string p_particle_id ){
		int t_id = int.Parse( p_particle_id );

		EffectIdTemplate t_temp = EffectTemplate.getEffectTemplateByEffectId( t_id );

		if( t_temp == null ){
			return;
		}

		Object t_obj = Resources.Load( t_temp.path );

		GameObject t_gb = (GameObject)Instantiate( t_obj ); 

		t_gb.SetActive( true );
	}
}
