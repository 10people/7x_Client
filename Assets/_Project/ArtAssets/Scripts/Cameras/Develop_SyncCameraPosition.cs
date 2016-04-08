using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Develop_SyncCameraPosition : MonoBehaviour {

	public float m_velocity = 1.0f;

	public Transform m_target_camera;

	private CharacterController m_controller;

	private Vector3 m_cam_offset;

	private Vector3 m_cached_pos;

	#region Mono

	void Start () {
		Init();
	}

	void Update () {
		if( m_target_camera == null ){
			Debug.LogError( "Target Camera is Null." );

			return;
		}

		if( Input.GetKey( KeyCode.W ) ){
			Move( m_target_camera.transform.forward, true );
		}
		else if( Input.GetKey( KeyCode.S ) ){
			Move( -m_target_camera.transform.forward, true );
		}
		else if( Input.GetKey( KeyCode.A ) ){
			Move( -m_target_camera.transform.right, true );
		}
		else if( Input.GetKey( KeyCode.D ) ){
			Move( m_target_camera.transform.right, true );
		}

		{
			Move( Vector3.down * 3, false );
		}
	}

	#endregion



	#region Utilities

	private void Init(){
		m_cached_pos = transform.position;

		if( m_target_camera != null ){
			m_cam_offset = m_target_camera.transform.position - m_cached_pos;
		}

		{
			m_controller = GetComponent<CharacterController>();
		}
	}

	private void Move( Vector3 p_vec_3, bool p_is_use_control ){
		if( m_target_camera == null ){
			Debug.LogError( "Target Camera is Null." );

			return;
		}

		if( p_is_use_control ){
			p_vec_3.y = 0;

			p_vec_3.Normalize();
		}

		Vector3 t_offset = p_vec_3 * m_velocity * Time.deltaTime;

		{
			Vector3 t_origin = transform.position;

			m_controller.Move( t_offset );

			m_target_camera.transform.position = m_target_camera.transform.position + ( transform.position - t_origin );
		}
	}

	#endregion
}
