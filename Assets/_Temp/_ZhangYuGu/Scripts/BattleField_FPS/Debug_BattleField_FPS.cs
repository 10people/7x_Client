using UnityEngine;
using System.Collections;

public class Debug_BattleField_FPS : MonoBehaviour {

	public GameObject m_smoke_root;

	public GameObject m_fire_root;


	public UILabel m_lb_pos;

	public Vector3[] m_test_pos;

	public GameObject m_target_gb;


	public int m_cur_pos_index = 0;


	#region Mono

	#endregion


	#region Interaction

	public void SwtichSmoke(){
		m_smoke_root.SetActive( !m_smoke_root.activeSelf );
	}

	public void SwitchFire(){
		m_fire_root.SetActive( !m_fire_root.activeSelf );
	}

	public void NextPos(){
		m_cur_pos_index = ( m_cur_pos_index + 1 ) % m_test_pos.Length;

		UpdatePos();
	}

	public void PrePos(){
		m_cur_pos_index = ( m_cur_pos_index - 1 + m_test_pos.Length ) % m_test_pos.Length;

		UpdatePos();
	}

	#endregion


	#region Utilities

	private void UpdatePos(){
		m_target_gb.transform.position = m_test_pos[ m_cur_pos_index ];
		
		m_lb_pos.text = m_cur_pos_index + "";
	}

	#endregion
}
