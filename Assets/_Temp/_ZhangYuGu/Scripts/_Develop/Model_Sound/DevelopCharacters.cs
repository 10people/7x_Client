#define DEBUG_MODEL_SOUND

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class DevelopCharacters : MonoBehaviour {

	public string m_name = "Develop Characters";



	// Model Template Info
	public int m_target_model_id = 0;

	public ModelTemplate m_target_model_template;

	public string m_target_path = "";

	public string m_target_effect = "";

	public string m_target_sound = "";

	public float m_target_radius = 0.0f;
	
	public float m_target_height = 0.0f;



	// cached info
	private string m_cached_name = "";

	private int m_cached_model_id = 0;

	private string m_cached_model_sound = "";

	public GameObject m_cached_gb = null;

	private Animator m_cached_animator = null;



	private List<DevelopUtility.DevelopFxTarget> m_fx_info_list = new List<DevelopUtility.DevelopFxTarget>();

	public float m_fx_time_out	= 300.0f;



	private static DevelopCharacters m_instance = null;



	#region

	void Awake(){
		{
			m_instance = this;
		}

		LoadFiles();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateDevelopInfo();

		TryGetModelnfo();

		UpdateModelInfo();

		UpdateFxTargets();
	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Model Callbacks
	
	private void UpdateCallbacks( GameObject p_gb ){
		DevelopUtility.DevelopAnimationCallback t_callback = p_gb.AddComponent<DevelopUtility.DevelopAnimationCallback>();

		t_callback.OnResetHitCount = OnResetHitCount;

		t_callback.OnPlaySound = OnPlaySound;
		
		t_callback.OnAttackMove = OnAttackMove;
		
		t_callback.OnPlayAtttackEffect = OnPlayAttackEffect;
		
		t_callback.OnResetXuanFengCount = OnResetXuanFengCount;
		
		t_callback.OnAddXuanFengCount = OnAddXuanFengCount;
	}

	public void OnResetHitCount(){
		if( !HaveAnimator() ){
			return;
		}

		for( int i = 0; i < 5; i++ ){
			string t_trigger = "attack_" + i;
			
			m_cached_animator.ResetTrigger( t_trigger );
		}
	}

	public void OnPlaySound( string p_path ){
		PlaySound( p_path, m_cached_gb );
	}
	
	public void OnAttackMove( int p_action_id ){
		if( !KingControllor.AttackMoveWillPlayEffect( p_action_id ) ){
			return;
		}
		
		#if DEBUG_MODEL_SOUND
		Debug.Log( "TOCHECK, OnAttackMove( " + p_action_id + " )" );
		#endif
		
		PlayFx(KingControllor.GetAttackMoveEffectId() );
	}
	
	public void OnPlayAttackEffect( int p_attack_id ){
		#if DEBUG_MODEL_SOUND
		//		Debug.Log( "OnPlayAttackEffect( " + p_attack_id + " )" );
		#endif
		
		DevelopUtility.PlayAttackEffectReturn m_play_attack_effect = KingControllor.GetPlayAttackEffectParams( p_attack_id );
		
		//		Debug.Log( "OnPlayAttackEffect( " + p_attack_id + " - " + m_play_attack_effect.m_effect_id + " )" );
		
		PlayFx( m_play_attack_effect );
	}
	
	public void OnResetXuanFengCount(){
		#if DEBUG_MODEL_SOUND
		//		Debug.Log( "TOCHECK, OnResetXuanFengCount()" );
		#endif
		
		KingSkillXuanFengZhan.ResetXuanFengCountCallback( m_cached_animator );
	}
	
	public void OnAddXuanFengCount(){
		#if DEBUG_MODEL_SOUND
		Debug.Log( "TOCHECK, OnAddXuanFengCount()" );
		#endif
		
		KingSkillXuanFengZhan.AddXuanFengCountCallback( m_cached_animator );
	}

	#endregion



	#region Reset Params

	private void ResetParams(){
		m_custom_animation_clip_name = "";
	}

	#endregion



	#region Develop Info

	private void UpdateDevelopInfo(){
		if( m_cached_name == m_name ){
			return;
		}

		m_cached_name = m_name;

		gameObject.name = m_name;
	}

	#endregion



	#region Develop Model

	private void TryGetModelnfo(){
		if( m_cached_model_id == m_target_model_id ){
			return;
		}
		
		ModelTemplate t_model_template = ModelTemplate.getModelTemplateByModelId( m_target_model_id, false );
		
		if( t_model_template == null ){
			m_target_model_template = null;

			return;
		}
		
		GetModelInfo();
	}

	public void GetModelInfo(){
		ModelTemplate t_model_template = ModelTemplate.getModelTemplateByModelId( m_target_model_id, false );

		if( t_model_template == null ){
			Debug.LogError( "Error In Getting Info." );
			
			return;
		}
		
		{
			m_target_model_template = t_model_template;

			m_target_model_id = m_target_model_template.modelId;

			m_target_path = m_target_model_template.path;
			
			m_target_effect = m_target_model_template.effect;
			
			m_target_sound = m_target_model_template.sound;
			
			m_target_radius = m_target_model_template.radius;
			
			m_target_height = m_target_model_template.height;
		}
		
		{
			m_cached_model_id = m_target_model_id;

			m_cached_model_sound = m_target_sound;
		}
	}

	public void LoadModel(){
		ModelTemplate t_model_template = ModelTemplate.getModelTemplateByModelId( m_target_model_id );

		if( t_model_template == null ){
			Debug.LogError( "Error In Getting Info." );
			
			return;
		}

		if( HaveModelGameObject() ){
			m_cached_gb.SetActive( false );

			Destroy( m_cached_gb );
		}

		{
			ResetParams();
		}

		{
			m_cached_gb = ( GameObject )Instantiate( Resources.Load( t_model_template.path ) );

			m_cached_gb.transform.parent = gameObject.transform;

			TransformHelper.ResetLocalPosAndLocalRot( m_cached_gb );

			m_cached_animator = m_cached_gb.GetComponent<Animator>();
		}

		{
			UpdateCharType( m_cached_gb );

			MonoBehaviour[] t_scripts = m_cached_gb.GetComponents<MonoBehaviour>();

			for( int i = t_scripts.Length - 1; i >= 0; i-- ){
				MonoBehaviour t_script = t_scripts[ i ];

				#if DEBUG_MODEL_SOUND
//				Debug.Log( "Destroy( " + t_script + " )" );
				#endif
				
				Destroy( t_script );
			}
		}

		{
			NavMeshAgent t_agent = m_cached_gb.GetComponent<NavMeshAgent>();

			if( t_agent != null ){
				Destroy( t_agent );
			}
		}

		{
			ComponentHelper.ClearColliders( m_cached_gb );
		}

		{
			UpdateCallbacks( m_cached_gb );
		}
	}

	#endregion



	#region Fx

	private GameObject m_temp_gb_root = null;

	public void PlayFx( DevelopUtility.PlayAttackEffectReturn p_attack_effect ){
		if( !p_attack_effect.m_will_play_effect ){
			return;
		}

		PlayFx( p_attack_effect.m_effect_id );
	}

	public void PlayFx( int p_fx_id ){
		#if DEBUG_MODEL_SOUND
//		Debug.Log( "PlayFx( " + m_target_model_id + " - " + p_fx_id + " )" );
		#endif

		EffectIdTemplate t_effect_id_template = EffectIdTemplate.getEffectTemplateByEffectId( p_fx_id );
		
		if( t_effect_id_template == null ){
			Debug.LogError( "Error In Getting Info." );
			
			return;
		}
		
//		Debug.Log( "PlayFx " + t_effect_id_template.effectId + " - " + t_effect_id_template.path + " - " + t_effect_id_template.sound );

		UnityEngine.Object t_res_gb = Resources.Load( t_effect_id_template.path );

		if( t_res_gb == null ){
			Debug.LogError( "Error, Nothing To Load: " + t_effect_id_template.path );

			return;
		}

		GameObject t_gb = ( GameObject )Instantiate( t_res_gb );

		{
			t_gb.transform.parent = GetTempGameObject().transform;
			
			TransformHelper.ResetLocalPosAndLocalRot( t_gb );
			
			UpdateFxSpeed( t_gb );
		}

		PlaySound( t_effect_id_template.sound, t_gb );
		
		{
			DevelopUtility.DevelopFxTarget t_info = new DevelopUtility.DevelopFxTarget( t_gb );
			
			m_fx_info_list.Add( t_info );
		}
	}

	private void PlaySound( string p_config_path, GameObject p_gb ){
		int t_target_sound_id = SoundPlayEff.GetTargetSoundId( p_config_path );

		if( t_target_sound_id == SoundPlayEff.GetSoundNullId() ){
			return;
		}

		if( p_gb == null ){
			Debug.LogError( "PlaySound.p_gb = null." );

			return;
		}

		#if DEBUG_MODEL_SOUND
//		Debug.Log( "PlaySound( " + t_target_sound_id + " in " + p_config_path + " )" );
		#endif
		
		DevelopUtility.PlaySound( t_target_sound_id, p_gb );
	}

	private void UpdateFxTargets(){
		for( int i = m_fx_info_list.Count - 1; i >= 0; i-- ){
			DevelopUtility.DevelopFxTarget t_target = m_fx_info_list[ i ];
			
			if( t_target.IsDone( DevelopCharacters.m_instance.m_fx_time_out ) ){
				t_target.FxDone();
				
				m_fx_info_list.Remove( t_target );
			}
		}
	}

	private GameObject GetTempGameObject(){
		if( m_temp_gb_root == null ){
			m_temp_gb_root = new GameObject();

			m_temp_gb_root.transform.parent = gameObject.transform;

			TransformHelper.ResetLocalPosAndLocalRotAndLocalScale( m_temp_gb_root );
		}

		return m_temp_gb_root;
	}

	#endregion



	#region Char Type

	public enum CharType{
		NONE,
		PLAYER,
		ENEMY,
		NPC,
		ITEM,
	}
	
	public CharType m_char_type = CharType.NONE;


	public GameObject m_gb_pos_weapon_Heavy;
			
	public GameObject m_gb_pos_weapon_Light_left;
		
	public GameObject m_gb_pos_weapon_Light_right;

	public GameObject m_gb_pos_weapon_Ranged;



	public RuntimeAnimatorController m_acl_light;
	
	public RuntimeAnimatorController m_acl_heavy;
	
	public RuntimeAnimatorController m_acl_range;

	private void UpdateCharType( GameObject p_gb ){
		{
			m_char_type = CharType.NONE;
		}

		KingControllor t_player = p_gb.GetComponent<KingControllor>();

		// Player
		if( t_player != null ){
			m_char_type = CharType.PLAYER;

			{
				m_gb_pos_weapon_Heavy = t_player.m_weapon_Heavy;

				DevelopUtility.MakeSureObjectExist( m_gb_pos_weapon_Heavy, "m_gb_pos_weapon_Heavy" );
				
				m_gb_pos_weapon_Light_left = t_player.m_weapon_Light_left;

				DevelopUtility.MakeSureObjectExist( m_gb_pos_weapon_Light_left, "m_gb_pos_weapon_Light_left" );
				
				m_gb_pos_weapon_Light_right = t_player.m_weapon_Light_right;

				DevelopUtility.MakeSureObjectExist( m_gb_pos_weapon_Light_right, "m_gb_pos_weapon_Light_right" );
				
				m_gb_pos_weapon_Ranged = t_player.m_weapon_Ranged;

				DevelopUtility.MakeSureObjectExist( m_gb_pos_weapon_Ranged, "m_gb_pos_weapon_Ranged" );
			}

			{
				m_acl_heavy = t_player.heavyConrollor;

				DevelopUtility.MakeSureObjectExist( m_acl_heavy, "m_acl_heavy" );

				m_acl_light = t_player.lightControllor;

				DevelopUtility.MakeSureObjectExist( m_acl_light, "m_acl_light" );

				m_acl_range = t_player.rangedConrollor;

				DevelopUtility.MakeSureObjectExist( m_acl_range, "m_acl_range" );
			}
		}



		BaseAI t_enemy = p_gb.GetComponent<BaseAI>();
		
		// ENEMY
		if( t_enemy != null && m_char_type == CharType.NONE ){
			m_char_type = CharType.ENEMY;
		}



		NpcObjectItem t_npc = p_gb.GetComponent<NpcObjectItem>();

		// NPC
		if( t_npc != null && m_char_type == CharType.NONE ){
			m_char_type = CharType.NPC;
		}



		GearAI t_gear = p_gb.GetComponent<GearAI>();

		DroppenAI t_drop = p_gb.GetComponent<DroppenAI>();
		// ITEM
		if( ( t_gear != null || t_drop != null ) && m_char_type == CharType.NONE ){
			m_char_type = CharType.ITEM;
		}
	}

	public bool IsPlayer(){
		return m_char_type == CharType.PLAYER;
	}

	public bool IsEnemy(){
		return m_char_type == CharType.ENEMY;
	}

	public bool IsNPC(){
		return m_char_type == CharType.NPC;
	}

	public bool IsItem(){
		return m_char_type == CharType.ITEM;
	}

	#endregion



	#region Player Weapon

	public enum WeaponType{
		None,
		LIGHT,
		HEAVY,
		BOW,
	}
	
	public WeaponType m_weapon_type = WeaponType.None;

	// 0 or left
	public int m_weapon_0_id = 0;

	// 1 or right
	public int m_weapon_1_id = 0;




	private WeaponType m_cached_weapon_type = WeaponType.None;

	private void UpdateWeaponInfo(){
		if( m_cached_weapon_type == m_weapon_type ){
			return;
		}
		
		{
			ClearWeapons();

			m_cached_weapon_type = m_weapon_type;

			m_weapon_0_id = 0;

			m_weapon_1_id = 0;
		}

		switch( m_cached_weapon_type ){
		case WeaponType.HEAVY:
			m_cached_animator.runtimeAnimatorController = m_acl_heavy;
			break;

		case WeaponType.LIGHT:
			m_cached_animator.runtimeAnimatorController = m_acl_light;
			break;

		case WeaponType.BOW:
			m_cached_animator.runtimeAnimatorController = m_acl_range;
			break;
		}
	}

	public void LoadWeaponModel(){
		{
			ClearWeapons();
		}
		
		ModelTemplate t_model_0_template = ModelTemplate.getModelTemplateByModelId( m_weapon_0_id, false );
		
		ModelTemplate t_model_1_template = ModelTemplate.getModelTemplateByModelId( m_weapon_1_id, false );


		UnityEngine.Object t_gb = null;

		GameObject t_gb_weapon_0 = null;

		GameObject t_gb_weapon_1 = null;

		switch( m_weapon_type ){
		case WeaponType.LIGHT:
			if( t_model_0_template != null ){
				t_gb = Resources.Load( t_model_0_template.path );

				if( t_gb != null ){
					t_gb_weapon_0 = ( GameObject )Instantiate( t_gb );
					
					t_gb_weapon_0.transform.parent = m_gb_pos_weapon_Light_left.transform;
				}
				else{
					Debug.LogError( "Nothing to Load: " + t_model_0_template.path );
				}
			}

			if( t_model_1_template != null ){
				t_gb = Resources.Load( t_model_1_template.path );

				if( t_gb != null ){
					t_gb_weapon_1 = ( GameObject )Instantiate( t_gb );
					
					t_gb_weapon_1.transform.parent = m_gb_pos_weapon_Light_right.transform;
				}
				else{
					Debug.LogError( "Nothing to Load: " + t_model_1_template.path );
				}
			}
			break;

		case WeaponType.HEAVY:
			if( t_model_0_template != null ){
				t_gb = Resources.Load( t_model_0_template.path );

				if( t_gb != null ){
					t_gb_weapon_0 = ( GameObject )Instantiate( t_gb );
					
					t_gb_weapon_0.transform.parent = m_gb_pos_weapon_Heavy.transform;
				}
				else{
					Debug.LogError( "Nothing to Load: " + t_model_0_template.path );
				}
			}
			break;

		case WeaponType.BOW:
			if( t_model_0_template != null ){
				t_gb = Resources.Load( t_model_0_template.path );

				if( t_gb != null ){
					t_gb_weapon_0 = ( GameObject )Instantiate( t_gb );
					
					t_gb_weapon_0.transform.parent = m_gb_pos_weapon_Ranged.transform;
				}
				else{
					Debug.LogError( "Nothing to Load: " + t_model_0_template.path );
				}
			}
			break;
		}

		TransformHelper.ResetLocalPosAndLocalRot( t_gb_weapon_0 );

		TransformHelper.ResetLocalPosAndLocalRot( t_gb_weapon_1 );
	}

	public void ClearWeapons(){
		if( m_char_type != CharType.PLAYER ){
			return;
		}

		#if DEBUG_MODEL_SOUND
//		Debug.Log( "ClearWeapons()" );
		#endif

		{
			GameObjectHelper.RemoveAllChildrenDeeply( m_gb_pos_weapon_Heavy );
			
			GameObjectHelper.RemoveAllChildrenDeeply( m_gb_pos_weapon_Light_left );
			
			GameObjectHelper.RemoveAllChildrenDeeply( m_gb_pos_weapon_Light_right );
			
			GameObjectHelper.RemoveAllChildrenDeeply( m_gb_pos_weapon_Ranged );
		}
	}

	#endregion



	#region Animation Type

	private const string COMBO_ATTACK_TAG	= "_COMBO";

	// Light
	public enum PlayerLightAnimationType{
		None,
		
		Stand0,
		Run,
		DODGE,

		attack_1,
		attack_2_COMBO,
		attack_3_COMBO,
		attack_4_COMBO,
		
		skill_1,
		skill_mibao,
		yixinghuanying,
		
		BATC,
		BATCDown,
		
		Dead,
		
		zhuchengdile,
		
		OnTheStage,
	}

	public PlayerLightAnimationType m_player_light_animation_type = PlayerLightAnimationType.None;



	// Heavy
	public enum PlayerHeavyAnimationType{
		None,

		Stand0,
		Run,
		DODGE,

		attack_1,
		attack_2_COMBO,
		attack_3_COMBO,
		attack_4_COMBO,
		
		skill_1,
		skill_2,
		skill_mibao,

		BATC,
		BATCDown,
		
		Dead,
		
		zhuchengdile,
		
		OnTheStage,
	}
			
	public PlayerHeavyAnimationType m_player_heavy_animation_type = PlayerHeavyAnimationType.None;



	// Range
	public enum PlayerRangeAnimationType{
		None,
		
		Stand0,
		Run,
		DODGE,

		attack_1,
		attack_2_COMBO,
		attack_3_COMBO,
		attack_4_COMBO,
		
		skill_1,
		skill_2,
		skill_mibao,
		
		BATC,
		BATCDown,
		
		Dead,
		
		zhuchengdile,

		OnTheStage,
	}

	public PlayerRangeAnimationType m_player_range_animation_type = PlayerRangeAnimationType.None;



	// All Enemy Have the Same Animator Structure.
	public enum EnemyAnimationType{
		None,

		Stand0,
		Stand,

		Run,

		Attack_0,
		Attack_1,
		Attack_2,

		Skill_0,
		Skill_1,
		Skill_2,
		Skill_3,
		Skill_4,
		Skill_5,
		Skill_6,
		Skill_7,
		Skill_8,

		BATC,
		BATCDown,

		Dead,

		Dialog,
		OnTheStage,
	}

	public EnemyAnimationType m_enemy_animation_type = EnemyAnimationType.None;



	/// Different from each other.
	public enum NPCAnimationType{
		None,
	}

	public NPCAnimationType m_npc_animation_type = NPCAnimationType.None;



	/// Different from each other.
	public enum ItemAnimationType{
		None,
	}

	public ItemAnimationType m_item_animation_type = ItemAnimationType.None;



	// custom animations
	public string m_custom_animation_clip_name = "";

	public void PlayCustomAnimation( string p_custom_animation_name ){
		#if DEBUG_MODEL_SOUND
//		Debug.Log( "PlayCustomAnimation( " + p_custom_animation_name + " )" );
		#endif

		if( string.IsNullOrEmpty( p_custom_animation_name ) ){
			return;
		}

		if( !HaveModelGameObject() ){
			Debug.LogError( "No GameObject Exist." );

			return;
		}

		if( !HaveAnimator() ){
			Debug.LogError( "No Animator Exist." );

			return;
		}

		// run
		{
			bool t_is_run = ( p_custom_animation_name.ToLowerInvariant() == "run" );

			m_cached_animator.SetFloat( "move_speed", t_is_run ? 10.0f : 0.0f );
		}

		bool t_is_combo = p_custom_animation_name.Contains( COMBO_ATTACK_TAG );

		if( !t_is_combo ){
			m_cached_animator.Play( p_custom_animation_name );
			
			return;
		}

		// combo
		{
			{
				m_cached_animator.Play( PlayerLightAnimationType.Stand0.ToString() );
			}

			// attack combo
			{
				string t_end_anim = p_custom_animation_name.Substring( 0, p_custom_animation_name.Length - COMBO_ATTACK_TAG.Length );
				
				int t_stop_index = int.Parse( t_end_anim.Substring( t_end_anim.Length - 1, 1 ) );
				
				for( int i = 0; i < t_stop_index; i++ ){
					string t_trigger = p_custom_animation_name.Substring( 0, 7 ) + i;
					
					m_cached_animator.SetTrigger( t_trigger );
				}
			}
		}
	}

	public void PlayPreBuildAnimation(){
		string t_anim = "";

		switch( m_char_type ){
		case DevelopCharacters.CharType.NONE:
			Debug.LogError( "Error, Char Have No Type." );
			break;
			
		case DevelopCharacters.CharType.PLAYER:
			switch( m_weapon_type ){
			case WeaponType.None:
				t_anim = "";
				break;

			case WeaponType.LIGHT:
				t_anim = m_player_light_animation_type.ToString();
				break;

			case WeaponType.HEAVY:
				t_anim = m_player_heavy_animation_type.ToString();
				break;

			case WeaponType.BOW:
				t_anim = m_player_range_animation_type.ToString();
				break;
			}
			break;
			
		case DevelopCharacters.CharType.NPC:
			t_anim = m_npc_animation_type.ToString();
			break;
			
		case DevelopCharacters.CharType.ENEMY:
			t_anim = m_enemy_animation_type.ToString();
			break;
			
		case DevelopCharacters.CharType.ITEM:
			t_anim = m_item_animation_type.ToString();
			break;
		}

		#if DEBUG_MODEL_SOUND
//		Debug.Log( "PlayPreBuildAnimation( " + t_anim + " )" );
		#endif

		PlayCustomAnimation( t_anim );
	}

	#endregion



	#region Pause Model

	public float m_play_speed = 1.0f;

	public void UpdateSpeed( float p_speed ){
		if( !HaveModelGameObject() ){
			Debug.LogError( "No ModelGameObject Exist." );
			
			return;
		}
		
		if( !HaveAnimator() ){
			Debug.LogError( "No Animator Exist." );
			
			return;
		}

		m_play_speed = p_speed;

		{
			m_cached_animator.speed = m_play_speed;
		}

		{
			for( int i = 0; i < m_fx_info_list.Count; i++ ){
				DevelopUtility.DevelopFxTarget t_fx = m_fx_info_list[ i ];

				UpdateFxSpeed( t_fx.m_fx_target );
			}
		}
	}

	private void UpdateFxSpeed( GameObject p_gb ){
		ParticleSystem[] t_pss = p_gb.GetComponentsInChildren<ParticleSystem>();
		
		for( int j = 0; j < t_pss.Length; j++ ){
			ParticleSystem t_ps = t_pss[ j ];
			
			t_ps.playbackSpeed = m_play_speed;
		}
	}

	#endregion



	#region File
	
	private void LoadFiles(){
		ModelTemplate.LoadTemplates();

		EffectIdTemplate.LoadTemplates();
		
		SoundManager.PureLoadTemplates();

		SkillTemplate.LoadTemplates();
	}
	
	public void SaveFiles(){
		#if DEBUG_MODEL_SOUND
		Debug.Log( "SaveFiles will be made when necessary." );
		#endif

	}

	#endregion



	#region Animation Callbacks



	#endregion


	#region Utilities
	
	/// Check Info Changes.
	private void UpdateModelInfo(){
		bool m_changed = false;
		
		if( m_cached_model_sound != m_target_sound ){
			m_changed = true;
		}
		
		if( m_changed ){
			UpdateCachedInfo();
		}

		UpdateWeaponInfo();
	}
	
	/// Update Template Info.
	private void UpdateCachedInfo(){
		#if DEBUG_MODEL_SOUND
		Debug.Log( "UpdateCachedInfo()" );
		#endif
		
		{
			m_cached_model_sound = m_target_sound;
		}
		
		ModelTemplate t_model_template = ModelTemplate.getModelTemplateByModelId( m_target_model_id, false );

		// Save Field
		{
			t_model_template.sound = m_target_sound;
		}
	}

	public bool HaveTemplateData(){
		return m_target_model_template != null;
	}
	
	public bool HaveModelGameObject(){
		return m_cached_gb != null;
	}


	public bool HaveAnimator(){
		return m_cached_animator != null;
	}
	
	#endregion
}
