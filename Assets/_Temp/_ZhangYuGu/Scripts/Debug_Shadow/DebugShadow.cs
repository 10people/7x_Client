using UnityEngine;
using System.Collections;

public class DebugShadow : MonoBehaviour {

	public GameObject m_enemy_root;

	public GameObject m_enemy_template;

	public Light m_light;

	public Material[] m_mats;

	public string[] m_shader_names;


	public Material[] m_reference_mats_shader;


	public float m_shadow_dis_delta = 0.2f;

	private bool m_gui_hide = false;

	#region Mono

	void Awake(){
//		Application.targetFrameRate = 60;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		int t_btn_index = 0;
		
		int t_left_offset = 0;

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Hide GUI" ) ){
			Debug.Log( "Hide GUI." );
			
			m_gui_hide = !m_gui_hide;
		}
	
		if( m_gui_hide ){
			return;
		}
		
		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Switch Light" ) ){
			m_light.gameObject.SetActive( !m_light.gameObject.activeSelf );
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Change Shader" ) ){
			for( int i = 0; i < m_mats.Length; i++ ){
				Shader t_shader = m_mats[ i ].shader;

				for( int j = 0; j < m_shader_names.Length; j++ ){
					if( t_shader.name == m_shader_names[ j ] ){
						string t_new_shader = m_shader_names[ ( j + 1 ) % m_shader_names.Length ];

						m_mats[ i ].shader = Shader.Find( t_new_shader );
					}
				}
			}
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Log Shader" ) ){
			for( int i = 0; i < m_mats.Length; i++ ){
				Shader t_shader = m_mats[ i ].shader;
			}
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Add Enemy" ) ){
			GameObject t_gb = ( GameObject )Instantiate( m_enemy_template );

			t_gb.SetActive( true );

			t_gb.transform.parent = m_enemy_root.transform;
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Remove Enemy" ) ){
			if( m_enemy_root.transform.childCount > 0 ){
				Destroy( m_enemy_root.transform.GetChild( 0 ).gameObject );
			}
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Inc Dis" ) ){
			QualitySettings.shadowDistance += m_shadow_dis_delta;
		}

		if( GUI.Button( GetButtonRect( t_left_offset, t_btn_index++ ), "Dec Dis" ) ){
			QualitySettings.shadowDistance -= m_shadow_dis_delta;
		}

		int t_btn_label_index = 0;

		int t_left_label_offset = 250;

		GUI.Label( GetLabelRect( t_left_label_offset, t_btn_label_index++ ), 
		               m_light.gameObject.activeSelf + "" );

		GUI.Label( GetLabelRect( t_left_label_offset, t_btn_label_index++ ), 
		               m_mats[ 0 ].shader.name );

		GUI.Label( GetLabelRect( t_left_label_offset, t_btn_label_index++ ), 
		          QualitySettings.shadowDistance + "" );
	}
	
	#endregion



	#region Utilities
	
	private Rect GetButtonRect( int p_x, int p_index_y ){
		return new Rect( p_x, 50 * p_index_y, 200, 50 );
	}

	private Rect GetLabelRect( int p_x, int p_index_y ){
		return new Rect( p_x, 50 * p_index_y, 250, 50 );
	}

	
	#endregion
}
