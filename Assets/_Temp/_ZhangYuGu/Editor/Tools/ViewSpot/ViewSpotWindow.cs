using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ViewSpotWindow : EditorWindow {

	private ViewSpotWindow m_view_spot;
	


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
		if( FunctionOpenTemp.templates == null ){
			return;
		}

		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			for( int i = 0; i < FunctionOpenTemp.templates.Count; i++ ){
				FunctionOpenTemp t_item = FunctionOpenTemp.templates[ i ];

				EditorGUILayout.LabelField( "id: " + t_item.m_iID + "      " + 
					"Show: " + t_item.m_show_red_alert + "   " + 
					"UsePushData: " + t_item.m_use_red_push_data + "   " + 
					"MenuParent: " + t_item.m_parent_menu_id,
					GUILayout.Width( 500f ) );
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
