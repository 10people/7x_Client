using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugAnimEvent : MonoBehaviour {

	public float m_time_scale = 1.0f;

	public bool m_loop = false;

	public int m_loop_count = 100;

	public List<Texture2D> m_tex_list = new List<Texture2D>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Time.timeScale = m_time_scale;

		if( m_loop ){
			m_loop = false;

			long t_num = 0;


			Debug.Log( "Time Before: " + Time.realtimeSinceStartup );

			for( int i = 0; i < m_loop_count; i++ ){
				for( int j = 0; j < m_loop_count; j++ ){
					for( int k = 0; k < m_loop_count; k++ ){
						t_num++;
					}
				}
			}

			Debug.Log( "Time After: " + Time.realtimeSinceStartup + "   - " + t_num );
		}
	}

	public void AnimEventCallback( int p_param ){
		Debug.Log( "AnimEventCallback( " + p_param + " )" );
	}

	void OnGUI(){
		int t_button_index = 0;
				
		if (GUI.Button (GetButton (t_button_index++), "Load Res")) {
			Debug.Log( "Time Before: " + Time.realtimeSinceStartup );

			LoadRes( "heximap" );
			LoadRes( "jinyangzhaojiu" );
			LoadRes( "kangjixionglu" );
			LoadRes( "kangjizhongshan" );
			LoadRes( "leyifaqi" );

			LoadRes( "miezuzhihuo" );
			LoadRes( "muhouheishou" );
			LoadRes( "sanjiafenjing" );
			LoadRes( "suibingtaoqi" );
			LoadRes( "tiandanfuqi" );


			Debug.Log( "Time After: " + Time.realtimeSinceStartup );
		}
		
//		int t_label_index = 0;
//		GUI.Label( GetLabelRect ( t_label_index++ ), "Dependencies" );

		int t_tex_index = 0;

		for( int i = 0; i < m_tex_list.Count; i++ ){
			GUI.DrawTexture ( GetTexRect( t_tex_index++ ), m_tex_list[ i ] );
		}
	}

	private void LoadRes( string p_path ){
		string t_prefix = "_UIs/PVE/Images/PVE_Maps/";

		p_path = t_prefix + p_path;

		Texture2D t_tex = ( Texture2D )Resources.Load( p_path );

		Debug.Log( p_path + ": " + t_tex.texelSize + " - " + t_tex.width + ", " + t_tex.height );

		m_tex_list.Add( t_tex );
	}

	private static Rect GetButton( int p_index ){
		return new Rect( 100, p_index * 30, 300, 35 );	
	}
	
	private static Rect GetLabelRect( int p_index ){
		return new Rect (0, p_index * 40, 100, 50);
	}

	private static Rect GetTexRect( int p_index ){
		return new Rect (0, p_index * 40, 100, 50);
	}



}
