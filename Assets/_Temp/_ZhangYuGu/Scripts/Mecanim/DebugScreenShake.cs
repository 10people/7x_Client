using UnityEngine;
using System.Collections;

public class DebugScreenShake : MonoBehaviour {

	public string m_act_name;

	public float m_delay;

	public float m_duration;

	public Vector3 m_max_offset;

	public iTween.EaseType m_ease_type = iTween.EaseType.easeInOutQuint;


	private Vector3 m_cam_origin;

	private float m_tween_start_time;


	private bool m_late_shake = false;

	private Vector3 m_late_shake_offset;

	void LateUpdate(){
		if( m_late_shake ){
			Camera.main.transform.position = Camera.main.transform.position + m_late_shake_offset;

			m_late_shake = false;
		}
	}

	public void Shake( float p_pre_delay ){
		Shake( p_pre_delay );
	}

	public void Shake(){
		m_cam_origin = Camera.main.transform.position;
		
		m_tween_start_time = Time.time;
		
		iTween.ValueTo(
			gameObject,
			iTween.Hash( "from", new Vector3( 0, 0, 0 ),
		            "to", m_max_offset,
		            "delay", m_delay,
		            "time", m_duration,
		            "easetype", m_ease_type,
		            "onupdate", "OniTween",
		            "oncomplete", "OniTweenComplete" ) );
	}

	public void OniTween( Vector3 p_offset ){
		//Debug.Log( "OniTween: " + p_offset );

		//Camera.main.transform.Translate( GetScreenShake( p_offset ), Space.Self );

		m_late_shake = true;

		m_late_shake_offset = GetScreenShake( p_offset );
	}

	public void OniTweenComplete(){
		//Debug.Log( "OniTweenComplete()" );

		Camera.main.transform.position = m_cam_origin;
	}

	private Vector3 GetBaseOffset(){
		return ( Time.time - m_tween_start_time - m_delay ) / m_duration * m_max_offset;
	}

	private Vector3 GetScreenShake( Vector3 p_offset ){
		return new Vector3( ( Random.value - 0.5f ) * 2 * p_offset.x, 
		                   ( Random.value - 0.5f ) * 2 * p_offset.y, 
		                   ( Random.value - 0.5f ) * 2 * p_offset.z ); 
	}
}
