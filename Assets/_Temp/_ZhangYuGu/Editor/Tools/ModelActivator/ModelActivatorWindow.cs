

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ModelActivatorWindow : EditorWindow {

	private ModelAutoActivator m_model_activator;
	


	private Vector2 m_scroll = Vector2.zero;
	

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );

		m_model_activator = ModelAutoActivator.GetInstanceWithOutCreate();

		{
			GUICommon();
		}

		if( m_model_activator == null ){
			GUIEmpty();
		}
		else{
			GUIModelActivator();
		}
	}

	void OnInspectorUpdate() {
		Repaint();
	}	

	void GUICommon(){
		EditorGUILayout.BeginVertical();
		
		//EditorGUILayout.ObjectField( "Cached GameObject", UI2DTool.GetCachedGameObject(), typeof(GameObject) );

		//EditorGUILayout.ObjectField( "Scaler GameObject", UI2DTool.GetActiveBackgroundScalerGameObject(), typeof(GameObject) );

		EditorGUILayout.EndVertical();
	}

	void GUIEmpty(){
		EditorGUILayout.LabelField( "Model Activator is null", GUILayout.Width( 120f ) );
	}

	void GUIModelActivator(){
		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			List<ModelAutoActivator.ActivatorContainer> t_list = ModelAutoActivator.GetActiveActivator();

			for( int i = t_list.Count - 1; i >= 0; i-- ){
				ModelAutoActivator.ActivatorContainer t_container = t_list[ i ];

				if( t_container.GetRootGameObject() != null ){
					EditorGUILayout.LabelField( i + 
						" visible: " + t_container.IsVisible() + 
						" Hierarchy: " + GameObjectHelper.GetGameObjectHierarchy( t_container.GetRootGameObject() ),
					    GUILayout.Width( 500f ) );
				}
				else{
					EditorGUILayout.LabelField( i + ": Container GameObject = null", GUILayout.Width( 500f ) );
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
