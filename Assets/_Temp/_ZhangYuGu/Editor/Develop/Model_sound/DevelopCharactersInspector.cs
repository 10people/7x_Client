//#define OPEN_SAVE_FILE

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DevelopCharacters))]
public class DevelopCharactersInspector : Editor {

	#region Mono

	public override void OnInspectorGUI (){
		DevelopCharacters t_model_sound = target as DevelopCharacters;
		
		GUILayout.Space( 3f );
		
		serializedObject.Update();

		// Develop
		{
			DevelopInfo( t_model_sound );
		}

		// Model
		{
			ModelInfo( t_model_sound );
		}

		// Animation
		{
			AnimationInfo( t_model_sound );
		}

		// Pause
		{
			PauseModel( t_model_sound );
		}

		#if OPEN_SAVE_FILE
		// save
		{
			SaveInfo( t_model_sound );
		}
		#endif

		// fx time out
		{
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_fx_time_out" ) , new GUIContent( "Fx TimeOut" ) );
		}
		
		serializedObject.ApplyModifiedProperties();
	}

	#endregion



	#region Develop Info

	private void DevelopInfo( DevelopCharacters p_model_sound ){


		if( !NGUIEditorTools.DrawHeader( "Char Info" ) ){
			return;
		}
		
		NGUIEditorTools.BeginContents();
		
		{
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_name" ), new GUIContent( "Name" ) );

//			GameObject t_gb = p_model_sound.m_cached_gb;
			
//			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_cached_gb" ), new GUIContent( "Character" ) );
			
//			p_model_sound.m_cached_gb = t_gb;
		}
		
		NGUIEditorTools.EndContents();
	}
	 
	#endregion



	#region Model Info

	private void ModelInfo( DevelopCharacters p_model_sound ){
		// Id
		{
			GUILayout.BeginHorizontal();
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_model_id" ), new GUIContent( "Model Id" ) );
			
			GUILayout.EndHorizontal();
		}
		
		if( GUILayout.Button( "Load Model" ) ){
			p_model_sound.LoadModel();
		}

		{
			ModelDetail(p_model_sound  );
		}
	}

	private void ModelDetail( DevelopCharacters p_model_sound ){
		if( !p_model_sound.HaveTemplateData() ){
			return;
		}
		
		if( !NGUIEditorTools.DrawHeader( "Model Detail" ) ){
			return;
		}

		NGUIEditorTools.BeginContents();
		
		{
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_path" ), new GUIContent( "Path (Read Only)" ) );
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_effect" ), new GUIContent( "Effects (Read Only)" )  );
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_sound" ), new GUIContent( "Sounds (Read Only)" ) );
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_radius" ), new GUIContent( "Radius (Read Only)" ) );
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_target_height" ), new GUIContent( "Height (Read Only)" ) );
		}
		
		NGUIEditorTools.EndContents();
		
		{
			WeaponInfo( p_model_sound );
		}
	}

	private void WeaponInfo( DevelopCharacters p_model_sound ){
		if( !p_model_sound.IsPlayer() ){
			return;
		}

		// Weapon Detail
		if( !NGUIEditorTools.DrawHeader( "Weapon Detail" ) ){
			return;
		}

		NGUIEditorTools.BeginContents();
		
		{
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_weapon_type" ), new GUIContent( "Weapon Type" ) );
			
			if( p_model_sound.m_weapon_type == DevelopCharacters.WeaponType.HEAVY ){
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_weapon_0_id" ), new GUIContent( "Heavy Weapon Id" ) );
			}
			else if( p_model_sound.m_weapon_type == DevelopCharacters.WeaponType.BOW ){
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_weapon_0_id" ), new GUIContent( "Range Weapon Id" ) );
			}
			else if( p_model_sound.m_weapon_type == DevelopCharacters.WeaponType.LIGHT ){
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_weapon_0_id" ), new GUIContent( "Left Weapon Id" ) );
				
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_weapon_1_id" ), new GUIContent( "Right Weapon Id" ) );
			}
		}
		
		if( p_model_sound.m_weapon_type != DevelopCharacters.WeaponType.None ){
			if( GUILayout.Button( "Load Weapon" ) ){
				p_model_sound.LoadWeaponModel();
			}
		}
		
		NGUIEditorTools.EndContents();
	}
	
	#endregion



	#region Animation

	private void AnimationInfo( DevelopCharacters p_model_sound ){
		if( !p_model_sound.HaveModelGameObject() ){
			return;
		}

		if( p_model_sound.IsPlayer() ){
		   if( p_model_sound.m_weapon_type == DevelopCharacters.WeaponType.None ){
				EditorGUILayout.HelpBox( "Please Select A WeaponType First.", MessageType.Info );

				return;
			}
		}

		if( !NGUIEditorTools.DrawHeader( "Animation Detail" ) ){
			return;
		}

		NGUIEditorTools.BeginContents();

		GUILayout.BeginVertical();

		// PreBuild
		{
			GUILayout.BeginHorizontal();

			switch( p_model_sound.m_char_type ){
			case DevelopCharacters.CharType.NONE:
				Debug.LogError( "Error, Char Have No Type." );
				break;

			case DevelopCharacters.CharType.PLAYER:
				switch( p_model_sound.m_weapon_type ){
				case DevelopCharacters.WeaponType.None:
					EditorGUILayout.HelpBox( "Please Select a WeaponType First.", MessageType.Info );
					break;
					
				case DevelopCharacters.WeaponType.LIGHT:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_player_light_animation_type" ), new GUIContent( "Player Clip" ) );
					break;
					
				case DevelopCharacters.WeaponType.HEAVY:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_player_heavy_animation_type" ), new GUIContent( "Player Clip" ) );
					break;
					
				case DevelopCharacters.WeaponType.BOW:
					EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_player_range_animation_type" ), new GUIContent( "Player Clip" ) );
					break;
				}
				break;

			case DevelopCharacters.CharType.ENEMY:
				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_enemy_animation_type" ), new GUIContent( "Enemy Clip" ) );
				break;

			case DevelopCharacters.CharType.NPC:
//				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_npc_animation_type" ), new GUIContent( "NPC Clip" ) );
				break;

			case DevelopCharacters.CharType.ITEM:
//				EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_item_animation_type" ), new GUIContent( "Enemy Clip" ) );
				break;
			}

			if( p_model_sound.IsPlayer() || p_model_sound.IsEnemy() ){
				if ( GUILayout.Button( "Play", GUILayout.Width( 60f ) ) ){
					p_model_sound.PlayPreBuildAnimation();
				}
			}

			GUILayout.EndHorizontal();
		}

		// custom
		{
			GUILayout.BeginHorizontal();
			
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_custom_animation_clip_name" ), new GUIContent( "Custom Clip" ) );
			
			if ( GUILayout.Button( "Play", GUILayout.Width( 60f ) ) ){
				p_model_sound.PlayCustomAnimation( p_model_sound.m_custom_animation_clip_name );
			}
			
			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();

		NGUIEditorTools.EndContents();
	}
	
	#endregion



	#region Pause Model

	private void PauseModel( DevelopCharacters p_model_sound ){
		if( !p_model_sound.HaveTemplateData() ){
			return;
		}

		if( !p_model_sound.HaveModelGameObject() ){
			return;
		}

		float t_speed = EditorGUILayout.Slider( "Speed", p_model_sound.m_play_speed, 0f, 1f );
		
		if( !Mathf.Approximately( t_speed, p_model_sound.m_play_speed ) ){
			p_model_sound.UpdateSpeed( t_speed );
		}
	}

	#endregion



	#region Save Info

	private void SaveInfo( DevelopCharacters p_model_sound ){
		if( p_model_sound.m_target_model_template == null ){
			return;
		}

		if( GUILayout.Button( "Save txt" ) ){
			p_model_sound.SaveFiles();
		}
	}
	
	#endregion
}
