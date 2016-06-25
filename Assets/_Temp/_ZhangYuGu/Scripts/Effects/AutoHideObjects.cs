//#define DEBUG_AUTO



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.5.27
 * @since:		Unity 5.1.3
 * Function:	Attach it to the player.
 * 
 * Notes:
 * 1.None.
 */ 
public class AutoHideObjects : MonoBehaviour {
	
	#region Mono

	void Awake(){
	
	}

	void Update(){
		if( Camera.main == null ){
			return;
		}

		CheckCollider();

		UpdateMat();
	}

	#endregion



	#region Check

	private GameObject m_cur_hit_gb = null;

	private void CheckCollider(){
		RaycastHit t_hit;

		int t_mask = LayerMask.GetMask( "Grounded", "3D Layer" );

		GameObject t_pre_hit_gb = m_cur_hit_gb;

		if( !Physics.Raycast( Camera.main.transform.position, 
				gameObject.transform.position - Camera.main.transform.position, 
				out t_hit, 
				100, 
				t_mask ) ){
			m_cur_hit_gb = null;
		}
		else{
			m_cur_hit_gb = t_hit.collider.gameObject;
		}

		if( t_pre_hit_gb != m_cur_hit_gb ){
			#if DEBUG_AUTO
			Debug.Log( "Set New Cur Hitted Gb( " + m_cur_hit_gb + " )" );
			#endif	
		}

		if( m_cur_hit_gb == null ){
			return;
		}

		if( TransformHelper.IsParentOrChild( m_cur_hit_gb, gameObject ) ){
			return;
		}

//		#if DEBUG_AUTO
//		Debug.Log( "Hitted: " + t_hit.collider.gameObject + " - " + t_hit.point );
//		#endif

		AddHittedInfo( t_hit.collider.gameObject );
	}

	public float m_alpha_duration = 1.0f;

	#if DEBUG_AUTO
	public bool m_log_list = false;
	#else
	private bool m_log_list = false;
	#endif

	private void UpdateMat(){
		for( int i = m_hitted_info_list.Count - 1; i >= 0; i-- ){
			HittedInfo t_info = m_hitted_info_list[ i ];

			float t_delta_alpha_progress = Time.deltaTime * 1.0f / m_alpha_duration;

			float t_pow = 1.0f;

			float t_pre_alpha = t_info.m_hitted_alpha_progress;

			if( t_info.m_hitted_gb == m_cur_hit_gb ){
				t_delta_alpha_progress = -t_delta_alpha_progress;
			}

			t_info.m_hitted_alpha_progress += t_delta_alpha_progress;

			t_info.m_hitted_alpha_progress = Mathf.Clamp( t_info.m_hitted_alpha_progress, 0.0f, 1.0f );

			if( t_info.m_hitted_alpha_progress == t_pre_alpha ){
				continue;
			}

			t_info.SetTran( t_info.m_hitted_alpha_progress );

//			#if DEBUG_AUTO
//			Debug.Log( "delta progress: " + t_delta_alpha_progress + 
//				" - progress: " + t_info.m_hitted_alpha_progress + 
//				"   - " + t_info.m_hitted_gb );
//			#endif

			if( t_info.m_hitted_alpha_progress == 1 &&
				t_info.m_hitted_gb != m_cur_hit_gb ){
				#if DEBUG_AUTO
				Debug.Log( "Remove Hitted Info( " + t_info.m_hitted_gb + " )" );
				#endif

				m_hitted_info_list.Remove( t_info );
			}
		}

		if( m_log_list ){
			m_log_list = false;

			Log();
		}
	}

	#endregion



	#region Log

	private void Log(){
		Debug.Log( "------------------------ Logging Hitted Info List -------------------------" );

		for( int i = 0; i < m_hitted_info_list.Count; i++ ){
			HittedInfo t_info = m_hitted_info_list[ i ];

			Debug.Log( "Hitted Info: " + i );

			t_info.Log();
		}
	}

	#endregion
	
	

	#region Hitted Info

	private List<HittedInfo> m_hitted_info_list = new List<HittedInfo>();

	private void AddHittedInfo( GameObject p_gb ){
		for( int i = 0; i < m_hitted_info_list.Count; i++ ){
			HittedInfo t_info = m_hitted_info_list[ i ];

			if( t_info.m_hitted_gb == p_gb ){
				#if DEBUG_AUTO
				Debug.Log( "Hitted GameObject already exist: " + p_gb );

				Log();

				#endif

				return;
			}
		}

		Material t_mat = ComponentHelper.GetFirstMaterial( p_gb );

		//  (UnityEngine.Shader)
//		string t_name = t_mat.shader.name.Substring( 0, t_mat.shader.name.Length - 11 );

		if (t_mat == null) return;

		string t_name = t_mat.shader.name;
			
		if( !t_name.EndsWith( "Alpha" ) ){
			#if DEBUG_AUTO
			Debug.Log( "Mat not using the targe shader: " + t_name + " - " + t_mat.shader.ToString() );
			#endif

			return;
		}

		HittedInfo t_new_info = new HittedInfo();

		t_new_info.m_hitted_gb = p_gb;

		t_new_info.m_hitted_mat = t_mat;

		t_new_info.m_hitted_mat_list = ComponentHelper.GetMaterials( p_gb );

		t_new_info.m_hitted_alpha_progress = t_new_info.m_hitted_mat.GetFloat( "_Tran" );

		m_hitted_info_list.Add( t_new_info );

		#if DEBUG_AUTO
		Debug.Log( "Add Hitted Info( " + p_gb + " )" );
		#endif
	}

	#endregion



	#region Utilities

	#endregion


	private class HittedInfo{
		public GameObject m_hitted_gb;

		public Material m_hitted_mat;

		public List<Material> m_hitted_mat_list;

		public float m_hitted_alpha_progress;

		public void Log(){
			Debug.Log( "Hitted gb: " + m_hitted_gb );

			Debug.Log( "Hitted mat: " + m_hitted_mat );

			Debug.Log( "Hitted progress: " + m_hitted_alpha_progress );
		}

		public void SetTran( float p_tran ){
			for( int i = 0; i < m_hitted_mat_list.Count; i++ ){
				Material t_mat = m_hitted_mat_list[ i ];

				if( t_mat == null ){
					continue;
				}

				t_mat.SetFloat( "_Tran", p_tran );
			}
		}
	}
}