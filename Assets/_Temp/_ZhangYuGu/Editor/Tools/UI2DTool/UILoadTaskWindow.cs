

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class UILoadTaskWindow : EditorWindow {

	private BundleHelper m_bundle_helper;
	


	private Vector2 m_scroll = Vector2.zero;
	

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );

		{
			GUICommon();
		}

		m_bundle_helper = BundleHelper.GetRef();

		if( m_bundle_helper == null ){
			GUIEmpty();
		}
		else{
			GUILoadTask();
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}	

	void GUICommon(){
		EditorGUILayout.BeginVertical();

		EditorGUILayout.LabelField( "IsDownloading: " + BundleHelper.IsDownloading(), GUILayout.Width( 600f ) );

		EditorGUILayout.EndVertical();
	}

	void GUIEmpty(){
		EditorGUILayout.LabelField( "Load Task is null", GUILayout.Width( 120f ) );
	}

	void GUILoadTask(){
		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			int t_count = m_bundle_helper.GetLoadTaskList().Count;

			for( int i = t_count - 1; i >= 0; i-- ){
				BundleHelper.LoadTask t_task = m_bundle_helper.GetLoadTaskList()[ i ];

				EditorGUILayout.LabelField( i + " : " + t_task.GetDescription(), GUILayout.Width( 1200f ) );
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
