#define DEBUG_DRAMA_DIRECTOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DramaDirectorWindow : EditorWindow {

	#region Unity

	private DramaDirector m_drama_director = null;

	void OnGUI(){
		GUILayout.Space( 3f );

		{
			DramaTargetGUI();
		}

		if ( DramaDirector.HaveInstance() && m_drama_director == null ) {
//			#if DEBUG_DRAMA_DIRECTOR
//			Debug.Log( "Auto Set DramaDirector Instance." );
//			#endif
			
			m_drama_director = DramaDirector.Instance();
		}

		if ( m_drama_director == null ) {
			return;
		}

		{
			DramaDetailGUI ();
		}

		{
			DramaControlGUI ();
		}
	}
	
	#endregion
	
	
	
	#region Drama Target

	private void DramaTargetGUI(){
		m_drama_director = (DramaDirector)EditorGUILayout.ObjectField( m_drama_director, typeof(DramaDirector), true );

		if ( !EditorApplication.isPlaying ) {
			return;
		}
		
		if ( GUILayout.Button( "Save Drama", GUILayout.Width( 120f ) ) ){
			OnSaveDrama();
		}
	}

	private void OnSaveDrama(){
		#if DEBUG_DRAMA_DIRECTOR
		Debug.Log( "OnSaveDrama()" );
		#endif
		
		if( DramaDirector.HaveInstance() ){
			DramaDirector.Instance().SaveStoryBoard();
		}
		else{
			Debug.LogError( "DramaDirector have no instance." );
		}
	}
	
	#endregion



	#region Drama Detail

	private void DramaDetailGUI(){
		if( !NGUIEditorTools.DrawHeader( "Drama Detail" ) ) {
			return;		
		}

		// drama info 1st row
		{
			GUILayout.BeginHorizontal();

			// story board
			{
				EditorGUILayout.LabelField( "StoryBoard id: " , GUILayout.Width( 100f ) );
				
				m_drama_director.m_story_board_id = EditorGUILayout.TextField( m_drama_director.m_story_board_id, GUILayout.Width( 100f ) );
			}

			// battle field id
			{
				EditorGUILayout.LabelField( "BattleField id: " , GUILayout.Width( 100f ) );
				
				m_drama_director.m_battle_field_id = EditorGUILayout.TextField( m_drama_director.m_battle_field_id, GUILayout.Width( 100f ) );
			}

			GUILayout.EndHorizontal();
		}

		// drama info 2nd row
		{
			GUILayout.BeginHorizontal();

			// boss stand pos
//			{
//				EditorGUILayout.LabelField( "Position id: " , GUILayout.Width( 100f ) );
//				
//				m_drama_director.m_boss_stand_pos_id = EditorGUILayout.TextField( m_drama_director.m_boss_stand_pos_id, GUILayout.Width( 100f ) );
//			}

			if ( GUILayout.Button( "Preview Drama", GUILayout.Width( 120f ) ) ){
				OnPreviewDrama();
			}
			
			GUILayout.EndHorizontal();
		}
	}

	private void OnPreviewDrama(){
//		#if DEBUG_DRAMA_DIRECTOR
//		Debug.Log( "OnPreviewDrama()" );
//		#endif

		AssetDatabase.Refresh ();

		Resources.UnloadUnusedAssets ();

		#if UNITY_EDITOR
		if( !EditorApplication.isPlaying ){
			EditorApplication.isPlaying = true;
		}
		else{
			if( DramaDirector.HaveInstance() ){
				DramaDirector.Instance().OnPreviewDrama();
			}
			else{
				Debug.LogError( "DramaDirector have no instance." );
			}
		}
		#endif

	}

	#endregion



	#region Drama Control
	
	private float m_drama_speed = 1.0f;

	private bool m_pause_drama = false;

	private void DramaControlGUI(){
		if( !NGUIEditorTools.DrawHeader( "Drama Control" ) ) {
			return;		
		}
		
		// drama info 1st row
		{
			GUILayout.BeginHorizontal();

			// speed
			{
				if ( GUILayout.Button( m_pause_drama ? "Resume" : "Pause", GUILayout.Width( 80f ) ) ){
					m_pause_drama = !m_pause_drama;
					
					if( m_pause_drama ){
						Time.timeScale = 0.0f;
					}
					else{
						ResetTimeScale();
					}
				}

				if( !m_pause_drama ){
					EditorGUILayout.LabelField( "Speed:" , GUILayout.Width( 80f ) );

					float t_drama_speed = EditorGUILayout.Slider( "", m_drama_speed, 0f, 1f );
					
					if( !Mathf.Approximately( t_drama_speed, m_drama_speed ) ){
						m_drama_speed = t_drama_speed;
						
						ResetTimeScale();
					}
				}
			}

			GUILayout.EndHorizontal();
		}
	}

	private void ResetTimeScale(){
		Time.timeScale = m_drama_speed;
	}

	#endregion
}
