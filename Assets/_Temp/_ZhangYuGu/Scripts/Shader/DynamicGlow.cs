using UnityEngine;
using System.Collections;

public class DynamicGlow : MonoBehaviour {

	public Material m_mat = null;

	private Vector3 m_target_vec3 = new Vector3( 90, 90, 90 );


	private Vector3 m_from_vec3 = Vector3.up;

	private Vector3 m_des_vec3 = Vector3.down;

	private Vector3 m_dura_time_vec3 = Vector3.zero;


	private Vector2 m_time_begin_end = new Vector2( 1f, 2.5f );

	private Vector3 m_total_time_vec3 = Vector3.one;


	private Vector3 m_sin_vec3 = Vector3.zero;

	private Vector3 m_record_vec3 = Vector3.up;

	void Start () {
		StartCoroutine( "ChangeVec" );
	}

	IEnumerator ChangeVec(){
		while( true ){
			{
				m_dura_time_vec3.x += Time.deltaTime;

				m_dura_time_vec3.y += Time.deltaTime;

				m_dura_time_vec3.z += Time.deltaTime;
			}

			{
				m_target_vec3.x = Mathf.Lerp( m_from_vec3.x, m_des_vec3.x, m_dura_time_vec3.x / m_total_time_vec3.x );

				m_target_vec3.y = Mathf.Lerp( m_from_vec3.y, m_des_vec3.y, m_dura_time_vec3.y / m_total_time_vec3.y );

				m_target_vec3.z = Mathf.Lerp( m_from_vec3.z, m_des_vec3.z, m_dura_time_vec3.z / m_total_time_vec3.z );
			}
			


			{
				if( Mathf.Approximately( m_target_vec3.x, m_des_vec3.x ) ){
					m_from_vec3.x = m_des_vec3.x;

					m_des_vec3.x = UtilityTool.GetRandom( 0, 360 );

					m_dura_time_vec3.x = 0;

					m_total_time_vec3.x = UtilityTool.GetRandom( m_time_begin_end.x, m_time_begin_end.y );
				}

				if( Mathf.Approximately( m_target_vec3.y, m_des_vec3.y ) ){
					m_from_vec3.y = m_des_vec3.y;
					
					m_des_vec3.y = UtilityTool.GetRandom( 0, 360 );

					m_dura_time_vec3.y = 0;

					m_total_time_vec3.y = UtilityTool.GetRandom( m_time_begin_end.x, m_time_begin_end.y );
				}

				if( Mathf.Approximately( m_target_vec3.z, m_des_vec3.z ) ){
					m_from_vec3.z = m_des_vec3.z;
					
					m_des_vec3.z = UtilityTool.GetRandom( 0, 360 );
					
					m_dura_time_vec3.z = 0;
					
					m_total_time_vec3.z = UtilityTool.GetRandom( m_time_begin_end.x, m_time_begin_end.y );
				}

			}

			yield return new WaitForEndOfFrame();
		}
	}
	
	void Update () {
		if( m_record_vec3 != m_target_vec3 ){
			if( m_mat == null ){
				Debug.LogError( "Error, Mat = null." );

				return;
			}

			m_record_vec3 = m_target_vec3;

			{
				m_sin_vec3.x = Mathf.Sin( m_target_vec3.x * Mathf.Deg2Rad );

				m_sin_vec3.y = Mathf.Sin( m_target_vec3.y * Mathf.Deg2Rad );

				m_sin_vec3.z = Mathf.Sin( m_target_vec3.z * Mathf.Deg2Rad );

				m_sin_vec3 = Vector3.Normalize( m_sin_vec3 );

				m_mat.SetVector( "_Vec3", m_sin_vec3 );
			}
		}
	}
}
