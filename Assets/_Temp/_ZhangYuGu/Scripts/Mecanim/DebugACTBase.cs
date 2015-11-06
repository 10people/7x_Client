using UnityEngine;
using System.Collections;

public class DebugACTBase : MonoBehaviour {

	public string m_act_name;

	public float m_delay = 1.0f;
	
	public Vector3 m_move_self = new Vector3( 0, 0, 1f );

	public Vector3 m_hit_move_self = new Vector3( 0, 0, 0.5f );
	
	public float m_move_duration = 1.0f;

	public iTween.EaseType m_ease_type = iTween.EaseType.easeInOutQuint;

	
	public float m_clip_duration = 1.0f;

	public float m_clip_speed = 1.0f;

	public float m_clip_addition = 1.0f;

	public float GetTotalDuration(){
		return m_clip_duration / m_clip_speed * m_clip_addition;
	}
}