using UnityEngine;
using System.Collections;

public class Debug_ACT_Movement : MonoBehaviour {

	public string m_act_name;
	
	public float m_delay = 0.0f;
	
	public Vector3 m_move_self = new Vector3( 0, 0, 1f );

	public float m_move_duration = 1.0f;
	
	public iTween.EaseType m_ease_type = iTween.EaseType.easeInOutQuint;

	public bool m_stop_if_collide = false;
}
