using UnityEngine;
using System.Collections;

public class MecJoyStick : MonoBehaviour {

	public GameObject m_mec_target;
		
	public float m_time_scale = 1.0f;

	public UILabel m_lb_time_scale;


	public string m_message_name = "UpdateSpeed";


	public float m_max_velocity = 1.0f;

	public Vector3 m_speed;

	public bool m_stop_if_release = true;


	private float m_max_speed_screen_len = 0;

	private float m_time_scale_unit = 0.1f;

	#region Mono

	// Use this for initialization
	void Start () {
		Time.timeScale = m_time_scale;

		BoxCollider t_collider = GetComponent<BoxCollider>();

		m_max_speed_screen_len = t_collider.size.x / 2;

		Debug.Log( "m_max_speed_screen_len: " + m_max_speed_screen_len );
	}
	
	// Update is called once per frame
	void Update () {
		if( Time.timeScale != m_time_scale ){
			Time.timeScale = m_time_scale;

			m_lb_time_scale.text = m_time_scale.ToString( "0.00" );
		}
		

	}

	#endregion


	#region UI Interaction

	public void Accelerate(){
		m_time_scale += m_time_scale_unit;
	}

	public void Decelerate(){
		m_time_scale -= m_time_scale_unit;

		if( m_time_scale < 0 ){
			m_time_scale = 0.05f;
		}
	}


	void OnPress( bool p_pressed ){
		if( p_pressed ){
			UpdateSpeed();
		}
		else if( m_stop_if_release ){
			m_speed.x = 0;

			m_speed.y = 0;

			m_speed.z = 0;

			m_mec_target.SendMessage( m_message_name, m_speed );
		}
	}

	void OnDrag (Vector2 delta){
		//Debug.Log( "OnDrag( " + delta + " )" );
		UpdateSpeed();
	}


	#endregion


	#region Utilities

	void UpdateSpeed(){
		Vector2 t_local_pos = new Vector2( transform.localPosition.x + ScreenTool.DesignWidth * 0.5f, 
		                                  transform.localPosition.y + ScreenTool.DesignHeight * 0.5f );
		
		Vector2 t_design_pos = ScreenTool.ScreenToDesign( UICamera.currentTouch.pos );
		
		Vector2 t_delta = t_design_pos - t_local_pos;
		
		//Debug.Log( t_delta + " - " + UICamera.currentTouch.pos + " - " + t_design_pos + " - " + t_local_pos + " - " + transform.localPosition );
		
		{
			float t_len = t_delta.magnitude;
			
			//float t_speed = t_len / m_max_speed_screen_len * m_max_velocity;

			float t_speed = m_max_velocity;

			if( t_speed > m_max_velocity ){
				t_speed = m_max_velocity;
			}
			
			m_speed.x = t_delta.x / t_len * t_speed;

			m_speed.y = 0;

			m_speed.z = t_delta.y / t_len * t_speed;
		}

		m_mec_target.SendMessage( m_message_name, m_speed );
	}

	#endregion
}
