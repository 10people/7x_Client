using UnityEngine;
using System.Collections;

public class Debug_MainCity_FPS : MonoBehaviour {
	
	void OnGUI(){
		int t_btn_index = 0;
		
		int t_left_offset = 0;
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Delayed Unload" ) ){
			Debug.Log( "Delayed Unload." );
			
			UtilityTool.Instance.DelayedUnloadUnusedAssets();
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Log Socket" ) ){
			Debug.Log( "Log Socket." );
			
//			SocketTool.Instance().m_log_socket_condition = true;
		}
	}

	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 75 * p_index_y, 200, 75 );
	}
}
