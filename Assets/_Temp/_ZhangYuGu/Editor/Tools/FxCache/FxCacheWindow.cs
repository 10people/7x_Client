using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FxCacheWindow : EditorWindow {

	private FxCacheWindow m_fx_cache;
	


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
		{
			m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );
		}

		{
			EditorGUILayout.BeginVertical();

			Dictionary<string, FxHelper.FxCacheContainer> t_dict = FxHelper.GetFreedFxGbDict();

			int t_index = 0;

			foreach( KeyValuePair<string, FxHelper.FxCacheContainer> t_kv in t_dict ){
				EditorGUILayout.LabelField( t_index + ":   " + t_kv.Value.m_free_gb_list.Count + " - " + t_kv.Value.m_created_count + " : " + t_kv.Key, GUILayout.Width( 500f ) );

				t_index++;
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
