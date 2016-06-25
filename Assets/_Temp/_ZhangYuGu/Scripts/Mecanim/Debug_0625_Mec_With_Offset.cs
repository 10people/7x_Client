using UnityEngine;
using System.Collections;

public class Debug_0625_Mec_With_Offset : MonoBehaviour {

	public Transform m_root_tran;

	private Vector3 m_origin_offset;

	private Vector3 m_origin_pos;

	void Awake(){
		Debug.Log( "Awake()" );

		m_origin_offset = m_root_tran.position;

		m_origin_pos = transform.position;
	}

	public void MecAnimSync(){
		Debug.Log( "MecAnimSync()" );

		Vector3 t_delta = m_root_tran.position - m_origin_offset;

		Debug.Log( "delta: " + t_delta );

		transform.position = m_origin_pos + t_delta;

		GetComponent<Animator>().SetTrigger( "t_attack" );
	}
}