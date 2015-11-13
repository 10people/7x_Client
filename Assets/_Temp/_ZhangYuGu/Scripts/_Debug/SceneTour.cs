using UnityEngine;
using System.Collections;

public class SceneTour : MonoBehaviour {

	public float m_move_xz_unit = 10.0f;

	public float m_move_y_unit = 8.0f;

	public Camera m_camera = null;

	public int m_scene_index = 0;

	private float[] m_btn_rect_params = new float[ 6 ];

	private static SceneTour m_instance = null;

	void Awake(){
		DontDestroyOnLoad (gameObject);

		if (m_instance == null) {
			m_instance = this;
		}
		else {
			Destroy( gameObject );
		}
	}

	// Use this for initialization
	void Start () {
		UpdateCamera ();
	}

	void UpdateCamera(){
		m_camera = gameObject.GetComponent<Camera>();

		if (m_camera == null) {
			Debug.LogError( "No Camera Found." );
		}
	}

	void OnGUI(){
		GUIScene ();

		GUICamera ();
	}

	void GUIScene(){
		{
			m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.2f;
			
			m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			m_btn_rect_params[ 4 ] = 0;
			
			m_btn_rect_params[ 5 ] = Screen.height * 0.095f;
		}
		
		int t_button_index = 0;

		if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "Next" ) ){
			m_scene_index = ( m_scene_index + 1 ) % Application.levelCount;

			Application.LoadLevel( m_scene_index );

			UpdateCamera();
		}

		if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "Previous" ) ){
			m_scene_index = ( m_scene_index - 1 + Application.levelCount ) % Application.levelCount;

			Application.LoadLevel( m_scene_index );
			
			UpdateCamera();
		}

		GUI.Label (new Rect (0, 0, Screen.width / 2, Screen.height * 0.1f), Application.loadedLevelName );
	}

	void GUICamera(){
		{
			m_btn_rect_params[ 0 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.2f;
			
			m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			m_btn_rect_params[ 4 ] = 0;
			
			m_btn_rect_params[ 5 ] = Screen.height * 0.095f;
		}
		
		int t_button_index = 0;



		if (m_camera == null) {
			return;
		}

		{
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "+X" ) ){
				m_camera.gameObject.transform.position += new Vector3( m_move_xz_unit, 0, 0 );
			}
			
			t_button_index++;
			
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "-X" ) ){
				m_camera.gameObject.transform.position += new Vector3( -m_move_xz_unit, 0, 0 );
			}
		}

		{
			t_button_index = 0;
			
			m_btn_rect_params[ 0 ] = Screen.width * 0.0f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.2f + m_btn_rect_params[ 5 ];
			
			m_btn_rect_params[ 4 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 5 ] = 0;
			
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "-Z" ) ){
				m_camera.gameObject.transform.position += new Vector3( 0, 0, -m_move_xz_unit );
			}
			
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "Reset" ) ){
				m_camera.gameObject.transform.position = new Vector3( 0, m_camera.gameObject.transform.position.y, 0 );
			}
			
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "+Z" ) ){
				m_camera.gameObject.transform.position += new Vector3( 0, 0, m_move_xz_unit );
			}
		}

		{
			m_btn_rect_params[ 0 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 1 ] = Screen.height * 0.8f;
			
			m_btn_rect_params[ 2 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
			
			m_btn_rect_params[ 4 ] = Screen.width * 0.2f;
			
			m_btn_rect_params[ 5 ] = 0;
		}
		
		t_button_index = 0;

		{
			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "-Y" ) ){
				m_camera.gameObject.transform.position += new Vector3( 0, -m_move_y_unit, 0 );
			}

			t_button_index++;

			if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "+Y" ) ){
				m_camera.gameObject.transform.position += new Vector3( 0, m_move_y_unit, 0 );
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
