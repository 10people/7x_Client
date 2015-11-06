using UnityEngine;
using System.Collections;

public class Debug_MainCity_FPS_0924 : MonoBehaviour {

	public GameObject m_player_root;

	public GameObject m_camera_root;

	public Vector3 m_delta;



	public Camera m_camera;

	public float m_cam_delta = 5f;


	//public MobileBloom t_bloom;

	// Use this for initialization
	void Start () {
//		Application.targetFrameRate = 60;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log( "TargetFrame: " + Application.targetFrameRate );
	}

	void OnGUI(){
		int t_btn_index = 0;
		
		int t_left_offset = 0;

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Add Player" ) ){
//			AddPlayer( 1 );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Add 5 Player" ) ){
//			AddPlayer(5 );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Remove Player" ) ){
			RemovePlayer( 1 );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Remove 5 Player" ) ){
			RemovePlayer( 5 );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Bloom" ) ){
			//t_bloom.SwitchAble();
		}


		t_left_offset = (int)(Screen.width * 0.2f);

		t_btn_index = 0;

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "z++" ) ){
			m_camera_root.transform.position = m_camera_root.transform.position + new Vector3( 0, 0, m_delta.z );
		}
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "z--" ) ){
			m_camera_root.transform.position = m_camera_root.transform.position + new Vector3( 0, 0, -m_delta.z );
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "y++" ) ){
			m_camera_root.transform.position = m_camera_root.transform.position + new Vector3( 0, m_delta.y, 0 );
		}
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "y--" ) ){
			m_camera_root.transform.position = m_camera_root.transform.position + new Vector3( 0, -m_delta.y, 0 );
		}


		t_left_offset = (int)(Screen.width * 0.4f);
		
		t_btn_index = 0;
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Cam++" ) ){
			m_camera.transform.Rotate( new Vector3( m_cam_delta, 0, 0 ) );
		}
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Cam--" ) ){
			m_camera.transform.Rotate( new Vector3( -m_cam_delta, 0, 0 ) );
		}



		int t_lb_index = 0;
		
		int t_lb_left_offset = 20;

		GUI.contentColor = Color.red;

		GUI.Label( GetLabelRect( t_lb_left_offset, t_lb_index++ ), GetActiveChild() );

		GUI.Label( GetLabelRect( t_lb_left_offset, t_lb_index++ ), m_camera.transform.rotation.x.ToString() );
	}

	private void AddPlayer( int p_num ){
		if( p_num <= 0 ){
			return;
		}

		int t_count = m_player_root.transform.childCount;

		for( int i = 0; i < p_num; i++ ){
			for( int j = 0; j < t_count; j++ ){
				GameObject t_gb = m_player_root.transform.GetChild( j ).gameObject;

				if( !t_gb.activeSelf ){
					t_gb.SetActive( true );

					break;
				}
			}
		}
	}

	private void RemovePlayer( int p_num ){
		if( p_num <= 0 ){
			return;
		}

		int t_count = m_player_root.transform.childCount;
		
		for( int i = 0; i < p_num; i++ ){
			for( int j = 0; j < t_count; j++ ){
				GameObject t_gb = m_player_root.transform.GetChild( j ).gameObject;
				
				if( t_gb.activeSelf ){
					t_gb.SetActive( false );

					break;
				}
			}
		}
	}

	private string GetActiveChild(){
		int t_count = m_player_root.transform.childCount;

		int t_active = 0;

		for( int j = 0; j < t_count; j++ ){
			GameObject t_gb = m_player_root.transform.GetChild( j ).gameObject;
				
			if( t_gb.activeSelf ){
				t_active++;
			}
		}

		return t_active.ToString();
	}

	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, Screen.height * 0.15f * p_index_y, Screen.width * 0.2f, Screen.height * 0.15f );
	}

	private Rect GetLabelRect( int p_x, int p_index_y ){
		return new Rect( p_x + Screen.width * 0.2f * p_index_y, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.2f );
	}
}
