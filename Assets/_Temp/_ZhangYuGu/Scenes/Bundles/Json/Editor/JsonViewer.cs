//#define DEBUG_JSON_VIEWER

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class JsonViewer : EditorWindow {

	public TextAsset m_txt;

	private TextAsset m_cached_txt;




	private Vector2 m_scroll = Vector2.zero;

	#region Unity

	void OnGUI(){
		GUILayout.Space( 3f );
		
		m_cached_txt = (TextAsset)EditorGUILayout.ObjectField( "Text File", m_txt, typeof(TextAsset) );

		if( m_txt != m_cached_txt ){
			UpdateJsonText( m_cached_txt );
		}

		m_scroll = EditorGUILayout.BeginScrollView( m_scroll, false, true );

		EditorGUILayout.BeginVertical();

		EditorGUILayout.TextField( m_formatted_json_text, GUILayout.Height( EditorGUIUtility.singleLineHeight * m_line_count ) );	

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndScrollView();
	}

	#endregion



	#region Utilities

	private string m_formatted_json_text = "";

	private int m_line_count = 0;

	private void UpdateJsonText( TextAsset p_txt ){
		m_cached_txt = p_txt;

		m_txt = m_cached_txt;

		if( p_txt == null ){
			m_formatted_json_text = "";

			UpdateLine( m_formatted_json_text );

			return;
		}

		JSONNode t_json_node = JSONNode.Parse( p_txt.text );

		{
			m_formatted_json_text = t_json_node.ToString( "" );

			UpdateLine( m_formatted_json_text );
		}
	}

	private void UpdateLine( string p_line ){
		m_line_count = m_formatted_json_text.Split( '\n' ).Length;
	}

	#endregion
}
