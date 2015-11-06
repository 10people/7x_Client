using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugAtlasMemory : MonoBehaviour {

	public GameObject m_ui_prefab;

	public GameObject m_ui_instance;

	#region Mono
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
		int t_btn_index = 0;
		
		int t_left_offset = 0;
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Load UI" ) ){
			if( m_ui_instance == null ){
				m_ui_instance = (GameObject)Instantiate( m_ui_prefab );
			}
		}
				
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Destroy UI" ) ){
			if( m_ui_instance != null ){
				Destroy( m_ui_instance );
			}
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Unload Unused" ) ){
			Resources.UnloadUnusedAssets();
		}
				
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Switch Scene" ) ){
			Application.LoadLevel( 0 );
		}
	}
	
	#endregion
	
	
	
	#region Interaction

	
	#endregion
	
	
	
	#region Utilities
	
	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 50 * p_index_y, 150, 50 );
	}
	
	#endregion
}