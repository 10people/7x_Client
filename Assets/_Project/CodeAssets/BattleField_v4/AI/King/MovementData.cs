using UnityEngine;
using System.Collections;

public class MovementData : MonoBehaviour
{
	public int actionId;

	public Vector3 m_move_self;

	public float m_move_time;
	
	public iTween.EaseType m_ease_type;

	public bool m_stop_if_collide;
}
