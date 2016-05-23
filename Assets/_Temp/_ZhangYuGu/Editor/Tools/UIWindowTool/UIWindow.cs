

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class UIWindow : EditorWindow {

	private UIWindowTool m_window_tool;
	


	private Vector2 m_scroll = Vector2.zero;
	

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );

		m_window_tool = UIWindowTool.GetInstanceWithOutCreate();

		{
			GUICommon();
		}

		if( m_window_tool == null ){
			GUIEmpty();
		}
		else{
			GUIWindowTool();
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}	

	void GUICommon(){
		EditorGUILayout.BeginVertical();
		
		EditorGUILayout.EndVertical();
	}

	void GUIEmpty(){
		EditorGUILayout.LabelField( "WindowTool is null", GUILayout.Width( 120f ) );
	}

	void GUIWindowTool(){
		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			int t_count = m_window_tool.GetWindowCount();

			for( int i = t_count - 1; i >= 0; i-- ){
				UIWindowTool.WindowState t_window = m_window_tool.GetWindow( i );

				if( t_window.GetTrigger() != null ){
					EditorGUILayout.LabelField( i + 
						" Id: " + t_window.GetUIId() +
						" Trigger: " + t_window.GetTrigger() + 
						" Event: " + t_window.GetWindowEvent(),
					    GUILayout.Width( 500f ) );
				}
				else{
					EditorGUILayout.LabelField( i + ": Window is null", GUILayout.Width( 500f ) );
				}
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
