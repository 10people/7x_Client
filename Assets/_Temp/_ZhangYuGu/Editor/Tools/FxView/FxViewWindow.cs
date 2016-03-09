using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FxViewWindow : EditorWindow {

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

		if( UI3DEffectTool.HaveInstance() ){
			EditorGUILayout.BeginVertical();

			List<UI3DEffectTool.FxWatcher> t_list = UI3DEffectTool.Instance().m_fx_watcher_list;

			int t_index = 0;

			for( int i = 0; i < t_list.Count; i++ ){
				UI3DEffectTool.FxWatcher t_item = t_list[ i ];

				List<UI3DEffectTool.FxWatcherShadow> t_shadow_list = t_item.m_shadow_list;

				for( int j = 0; j < t_shadow_list.Count; j++ ){
					UI3DEffectTool.FxWatcherShadow t_shadow = t_shadow_list[ j ];

					EditorGUILayout.LabelField( i + " - " + j + " :   " + 
						"V: " + t_shadow.GetShadowVisibility() + "    " +
						"Shadow: " + GameObjectHelper.GetGameObjectHierarchy( t_shadow.m_shadow_gb ) + "   " +
						" Target: " + t_item.GetTargetDesc()  + "   " + 
						" CachV: " + t_item.GetCachedVisibility() + "   " + 
						" TargetV: " + t_item.GetTargetNGUIVisibility() + "   ",
						GUILayout.Width( 2500f ) );
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
