using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ViewProtoWindow : EditorWindow {

	private Vector2 m_scroll = Vector2.zero;
	

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );

		{
			GUICommon();
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}	

	void GUICommon(){
		if( SocketTool.GetSendingMessages() == null ){
			return;
		}

		if( SocketTool.GetReceivedMessages() == null ){
			return;
		}

		QXBuffer[] t_sending = SocketTool.GetSendingMessages().ToArray();

		QXBuffer[] t_receiving = SocketTool.GetReceivedMessages().ToArray();

		int t_sending_len = t_sending.Length;

		int t_received_len = t_receiving.Length;

		int t_max_len = t_sending_len > t_received_len ? t_sending_len : t_received_len;

		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			for( int i = 0; i < t_max_len; i++ ){
				string t_received_str = "";

				string t_sending_str = "";

				if( i >= t_sending_len ){
					t_sending_str = "-";
				}
				else{
					if( t_sending[ i ] != null ){
						t_sending_str = t_sending[ i ].m_protocol_index + "";	
					}
					else{
						t_sending_str = "null";
					}
				}

				if( i >= t_received_len ){
					t_received_str = "-";
				}
				else{
					if( t_receiving[ i ] != null ){
						t_received_str = t_receiving[ i ].m_protocol_index + "";	
					}
					else{
						t_received_str = "null";
					}
				}

				EditorGUILayout.LabelField( i + " --->   " + 
					"Received: " + t_received_str + "   " +
					"Sending: " + t_sending_str,
					GUILayout.Width( 300f ) );
			}

			EditorGUILayout.EndVertical();
		}

		{
			EditorGUILayout.EndScrollView();
		}
	}

	#endregion



	#region Utilities



	#endregion
}
