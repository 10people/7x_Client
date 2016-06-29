

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class UI2DWindow : EditorWindow {

	private UI2DTool m_2d_tool;
	


	private Vector2 m_scroll = Vector2.zero;
	

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );

		m_2d_tool = UI2DTool.GetInstanceWithOutCreate();

		{
			GUICommon();
		}

		if( m_2d_tool == null ){
			GUIEmpty();
		}
		else{
			GUI2DTool();
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}	

	void GUICommon(){
		EditorGUILayout.BeginVertical();
		
		EditorGUILayout.ObjectField( "Cached GameObject", UI2DTool.GetCachedGameObject(), typeof(GameObject) );

		EditorGUILayout.ObjectField( "Scaler GameObject", UI2DTool.GetTopBackgroundScalerGameObject(), typeof(GameObject) );

		EditorGUILayout.EndVertical();
	}

	void GUIEmpty(){
		EditorGUILayout.LabelField( "UI2DTool is null", GUILayout.Width( 120f ) );
	}

	void GUI2DTool(){
		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			int t_count = m_2d_tool.GetUICount();

			for( int i = t_count - 1; i >= 0; i-- ){
				UI2DTool.UI2DToolItem t_ui = m_2d_tool.GetUI( i );

				if( t_ui.GetRootGameObject() != null ){
					EditorGUILayout.LabelField( i + 
						" visible: " + t_ui.GetVisibility() + 
						" Scaler: " + t_ui.HaveActiveBackgroundScaler() +  
						"   - " + t_ui.GetRootGameObject().name + " - " + t_ui.GetFlagItemCount(),
					    GUILayout.Width( 500f ) );
				}
				else{
					EditorGUILayout.LabelField( i + ": 2D UI GameObject = null", GUILayout.Width( 500f ) );
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
