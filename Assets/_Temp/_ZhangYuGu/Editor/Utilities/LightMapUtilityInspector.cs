
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(LightMapUtility))]
public class LightMapUtilityInspector : Editor {

	#region Mono
	
	public override void OnInspectorGUI (){
		LightMapUtility t_target = target as LightMapUtility;

		if( t_target.m_light_params_dict == null ){
			t_target.ComposeDict();
		}
		
		GUILayout.Space( 3f );
		
		serializedObject.Update();
		
		// Develop
		{
			DevelopInfo( t_target );
		}

		serializedObject.ApplyModifiedProperties();
	}
	
	#endregion



	#region Develop Info
	
	private void DevelopInfo( LightMapUtility t_target ){
		NGUIEditorTools.BeginContents();

		NGUIEditorTools.BeginContents();
		
		GUILayout.BeginVertical();
	
		List<float> t_list = t_target.m_light_params_dict.Keys.ToList();

		t_list.Sort();

		foreach( float t_key_float in t_list ){
			float t_value_float = t_target.m_light_params_dict[ t_key_float ];

			GUILayout.BeginHorizontal();
			
			string t_key = t_key_float.ToString();
			
			t_key = EditorGUILayout.TextField("Origin Intensity: ", t_key );
			
			string t_value = t_value_float.ToString();
			
			string t_percent = EditorGUILayout.TextField("new Intensity: ", t_value );
			
			{
				float t_new_key = float.Parse( t_key );
				
				float t_new_value = float.Parse( t_percent );
				
				if( !Mathf.Approximately( t_new_key, t_key_float ) ){
					t_target.m_light_params_dict.Remove( t_key_float );
					
					t_target.m_light_params_dict.Add( t_new_key, t_value_float );

					t_target.GenerateList();

					break;
				}
				
				if( !Mathf.Approximately( t_new_value, t_value_float ) ){
					t_target.m_light_params_dict[ t_new_key ] = t_new_value;

					t_target.GenerateList();

					break;
				}
			}
			
			GUILayout.EndHorizontal();
		}

		if( GUILayout.Button( "Add" ) ){
			float t_key = 0;

			while( t_target.m_light_params_dict.ContainsKey( t_key ) ){
				t_key = t_key + Random.value / 10;
			}

			t_target.m_light_params_dict.Add( t_key, 0 );

			t_target.GenerateList();
		}

		if( GUILayout.Button( "Process Spot Group" ) ){
			t_target.OnProcess();
		}

		GUILayout.EndVertical();
		
		NGUIEditorTools.EndContents();
		
		NGUIEditorTools.EndContents();
	}
	
	#endregion
}