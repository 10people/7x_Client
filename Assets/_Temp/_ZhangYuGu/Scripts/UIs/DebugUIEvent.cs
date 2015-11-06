using UnityEngine;
using System.Collections;

public class DebugUIEvent : MonoBehaviour {

	public bool m_press_log		= false;

	public bool m_drag_log		= false;

	public bool m_click_log		= true;

	public bool m_drop_log		= true;

	void OnPress( bool p_pressed ){
		if( m_press_log ){
			Debug.Log( gameObject + ".OnPress( " + p_pressed + " )" );
		}
	}


	void OnDrag( Vector2 p_delta ){
		if( m_drag_log ){
			Debug.Log( gameObject + ".OnDrag( " + p_delta + " )" );
		}
	}

	void OnClick(){
		if( m_click_log ){
			Debug.Log( gameObject + ".OnClick()" );
		}
	}

	void OnDrop (GameObject go){
		if( m_drop_log ){
			Debug.Log( gameObject + ".OnDrop( " + go + " )" );
		}
	}
}
