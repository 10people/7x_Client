using UnityEngine;
using System.Collections;

public class Debug_BB_Cloud : MonoBehaviour {

	public bool m_is_hide = true;

	public GameObject m_gb_t;

	public GameObject m_gb_b;

	public GameObject m_gb_l;

	public GameObject m_gb_r;


	public float m_duration = 1.0f;

	public Vector2 m_move_offset;


	public Vector2 m_cam_size;


	private bool m_changable = true;

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region UI Interaction

	void OnGUI(){
		if( !m_changable ){
			return;
		}

		int t_btn_index = 0;
		
		int t_left_offset = 0;
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Switch" ) ){
			Debug.Log( "Switch" );
			
			Switch();
		}
	}

	#endregion



	#region Interaction

	private void Switch(){
		m_changable = false;

		iTween.ValueTo(
			gameObject,
			iTween.Hash( "from", Camera.main.orthographicSize,
		            "to", m_is_hide ? m_cam_size.x : m_cam_size.y,
		            "time", m_duration,
		            "easetype", iTween.EaseType.easeInQuint,
		            "onupdate", "OniTween",
		            "oncomplete", "OniTweenComplete" ) );

		Vector3 m_offset_r = new Vector3( m_move_offset.x , 0, 0 );

		m_offset_r = m_is_hide ? m_offset_r : -m_offset_r;

		Vector3 m_offset_t = new Vector3( 0, 0, m_move_offset.y );

		m_offset_t = m_is_hide ? -m_offset_t : m_offset_t;

		iTween.MoveBy( m_gb_r, 
		              iTween.Hash( 
		            "x", m_offset_r.x, 
		            "time", m_duration, 
		            "easeType", iTween.EaseType.easeInQuint ) );

		iTween.MoveBy( m_gb_l, 
		              iTween.Hash( 
		            "x", -m_offset_r.x, 
		            "time", m_duration, 
		            "easeType", iTween.EaseType.easeInQuint ) );

		iTween.MoveBy( m_gb_t, 
		              iTween.Hash( 
		            "z", m_offset_t.z, 
		            "time", m_duration, 
		            "easeType", iTween.EaseType.easeInQuint ) );

		iTween.MoveBy( m_gb_b, 
		              iTween.Hash( 
		            "z", -m_offset_t.z, 
		            "time", m_duration, 
		            "easeType", iTween.EaseType.easeInQuint ) );

		m_is_hide = !m_is_hide;
	}

	private void OniTween( float p_value ){
		Camera.main.orthographicSize = p_value;
	}

	private void OniTweenComplete(){
		m_changable = true;
	}

	#endregion



	#region Utilities
	
	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 75 * p_index_y, 200, 75 );
	}
	
	#endregion
}
