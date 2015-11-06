using UnityEngine;
using System.Collections;

public class Debug_Lock_Camera_Relative_Pos : MonoBehaviour {

	public GameObject m_target;

	public bool m_lock_x;

	public bool m_lock_y;

	public bool m_lock_z;

	private Vector3 m_relative_to_target;

	// Use this for initialization
	void Start () {
		m_relative_to_target = gameObject.transform.position - m_target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = m_target.transform.position + m_relative_to_target;
	}
}
