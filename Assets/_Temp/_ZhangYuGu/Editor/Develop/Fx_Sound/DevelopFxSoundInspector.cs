using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DevelopFxSound))]
public class DevelopFxSoundInspector : Editor {

	public override void OnInspectorGUI (){
		DevelopFxSound t_fx_sound = target as DevelopFxSound;

		GUILayout.Space( 3f );

		serializedObject.Update();

		// Fx
		{
			GUILayout.BeginHorizontal();

			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_fx_id" ) );

			if( GUILayout.Button( "Get Info" ) ){
				t_fx_sound.GetFxInfo();
			}

			GUILayout.EndHorizontal();
		}

		if( GUILayout.Button( "Play Fx" ) ){
			t_fx_sound.PlayFx();
		}

		// Fx Detail
		if( NGUIEditorTools.DrawHeader( "Fx Detail" ) ){

			NGUIEditorTools.BeginContents();

			{
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_fx_path" ), new GUIContent( "Path (Read Only)" ) );

				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_fx_sound" ), new GUIContent( "Sounds (Read Only)" )  );

				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_fx_position" ), new GUIContent( "Position?" ) );
			}

			NGUIEditorTools.EndContents();
		}

		// save
		{
			if( GUILayout.Button( "Save txt" ) ){
				t_fx_sound.SaveFiles();
			}
		}

		// fx time out
		{
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_fx_time_out" ) , new GUIContent( "Fx TimeOut" ) );
		}

		serializedObject.ApplyModifiedProperties();
	}
}
