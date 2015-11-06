//#define DEBUG_ANIMATION_CALLBACKS

using UnityEngine;
using System.Collections;

public class DevelopUtility : MonoBehaviour {

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Utilites

	public static void MakeSureObjectExist( Object p_gb, string p_log_if_error ){
		if( p_gb == null ){
			Debug.LogError( "Object is Null: " + p_log_if_error );
		}
	}

	/// Play Sound On p_gb With Sound_Id in "SoundId.xml".
	public static void PlaySound( int p_sound_id, GameObject p_gb ){
//		Debug.Log( "PlaySound( " + p_sound_id + " on " + p_gb + " )" );

		AudioClip t_sound_clip = ( AudioClip )Resources.Load( SoundManager.GetSoundInfo( p_sound_id ).sPath );

		AudioSource[] t_sources = p_gb.GetComponents<AudioSource>();

		AudioSource t_target_source = null;

		for( int i = 0; i < t_sources.Length; i++ ){
			AudioSource t_source = t_sources[ i ];

			if( !t_source.isPlaying ){
//				Debug.Log( "Find Stopped Source." );

				t_target_source = t_source;

				break;
			}
		}

		if( t_target_source == null ){
//			Debug.Log( "Create New Source." );

			t_target_source = p_gb.AddComponent<AudioSource>();
		}
		
		t_target_source.rolloffMode = AudioRolloffMode.Linear;
		
		t_target_source.clip = (AudioClip)t_sound_clip;
		
		t_target_source.Play();
	}

	#endregion

	public class DevelopFxTarget{
		public GameObject m_fx_target;
		
		public float m_create_time;
		
		public DevelopFxTarget( GameObject p_gb ){
			m_fx_target = p_gb;
			
			m_create_time = Time.realtimeSinceStartup;
		}
		
		public bool IsDone( float p_time_out ){
			if( Time.realtimeSinceStartup - m_create_time > p_time_out ){
				return true;
			}
			
			//			Debug.Log( m_fx_target.name + ": " + ( Time.realtimeSinceStartup - m_create_time ) );
			
			return false;
		}
		
		public void FxDone(){
			if( m_fx_target == null ){
				return;
			}
			
			m_fx_target.gameObject.SetActive( false );
			
			Destroy( m_fx_target );
		}
	}

	/// Reference DramaStorySimulation.cs.
	public class DevelopAnimationCallback : MonoBehaviour{
		public delegate void VoidDelegateString ( string p_string );

		public delegate void VoidDelegateInt( int p_int );

		public delegate void VoidDelegateVoid();



		public VoidDelegateVoid OnResetHitCount;

		public VoidDelegateString OnPlaySound;

		public VoidDelegateInt OnAttackMove;

		public VoidDelegateInt OnPlayAtttackEffect;

		public VoidDelegateVoid OnResetXuanFengCount;

		public VoidDelegateVoid OnAddXuanFengCount;


		public void ResetHitCount(){
			#if DEBUG_ANIMATION_CALLBACKS
			Debug.Log( "DevelopAnimationCallback.ResetHitCount()" );
			#endif

			if( OnResetHitCount != null ){
				OnResetHitCount();
			}
		}

		public void PlaySound( string p_path ){
			if( OnPlaySound != null ){
				OnPlaySound( p_path );
			}
		}

		public void attackDone( int actionId ){}

		public void playAttackEffect( int p_attack_id ){
			#if DEBUG_ANIMATION_CALLBACKS
//			Debug.Log( "TOCHECK, DevelopAnimationCallback.playAttackEffect( " + p_attack_id + " )" );
			#endif

			if( OnPlayAtttackEffect != null ){
				OnPlayAtttackEffect( p_attack_id );
			}
		}

		public void setWeaponTriggerTrue( int _aid ){

		}

		public void setWeaponTriggerFalse( int hand ){

		}

		public void createShadows(){}

		public void refreshShadows( int actionId ){}

		public void attackDone(){
			#if DEBUG_ANIMATION_CALLBACKS
			Debug.Log( "DevelopAnimationCallback.attackDone()" );
			#endif
		}

		public void dieActionDone(){}

		public void attackedActionStart(){
			#if DEBUG_ANIMATION_CALLBACKS
			Debug.Log( "DevelopAnimationCallback.attackedActionStart()" );
			#endif
		}

		public void attackedActionEnd(){
			#if DEBUG_ANIMATION_CALLBACKS
			Debug.Log( "DevelopAnimationCallback.attackedActionEnd()" );
			#endif
		}

		public void clearTrails(){}

		public void openShow(){}

		public void activeSkillStart(){}

		public void refreshRupt(){}

		public void playPre(){
			#if DEBUG_ANIMATION_CALLBACKS
			Debug.Log( "TODO, DevelopAnimationCallback.playPre()" );
			#endif

			// TODO, Play excel fx.
		}

		public void setStand(){}

		public void attackStart(){}

		public void attackMove( int actionId ){
			if( OnAttackMove != null ){
				OnAttackMove( actionId );
			}
		}

		public void firstAtt(){}

		public void updateAttackSpeed(){}

		public void chooseTarget_skill_1( int index ){}

		public void checkSkillDrama( int skillId ){}

		public void winActionStart(){}

		public void skill_1_resetPosition(){}

		public void WudizhanDone(){}

		public void resetXuanFengCount(){
			if( OnResetXuanFengCount != null ){
				OnResetXuanFengCount();
			}
		}

		public void addXuanFengCount(){
			if( OnAddXuanFengCount != null ){
				OnAddXuanFengCount();
			}
		}

		public void HeavySkill_2_End(){}

		public void createArrow( int p_action_id ){}

		public void attackForward(){}

		public void winActionDone(){}

		public void checkDieInKnockdown(){}
	}



	public class PlayAttackEffectReturn{
		public enum PositionType{
			ZERO,
			TRANSFORM_POSITION,
			TRNASFORM_POSITION_PLUS_TRANSFORM_FORWARD,
		}

		public enum ForwardType{
			ZERO,
			TRANSFORM_FORWARD,
			CUSTOM,
		}

		public enum GameObjectType{
			NONE,
			GAMEOBJECT,
			WEAPON_RANGE_TRANSFORM_PARENT_GAMEOBJECT,
		}

		public bool m_will_play_effect = false;

		public int m_effect_id;

		public PositionType m_position_type;

		public ForwardType m_forward_type;

		public Vector3 m_custom_forward;

		public GameObjectType m_gb_type;

		public float m_time;


		public void Set( int p_effect_id, PositionType p_pos_type, ForwardType p_forward_type ){
			Set( p_effect_id, p_pos_type, p_forward_type, Vector3.zero, GameObjectType.NONE, BattleEffectControllor.GetDefaultEffectTime() );
		}

		public void Set( int p_effect_id, PositionType p_pos_type, ForwardType p_forward_type, Vector3 p_custom_forward ){
			Set( p_effect_id, p_pos_type, p_forward_type, p_custom_forward, GameObjectType.NONE, BattleEffectControllor.GetDefaultEffectTime() );
		}

		public void Set( int p_effect_id, GameObjectType p_gb_type ){
			Set( p_effect_id, PositionType.ZERO, ForwardType.ZERO, Vector3.zero, p_gb_type, BattleEffectControllor.GetDefaultEffectTime() );
		}

		public void Set( int p_effect_id, GameObjectType p_gb_type, float p_time ){
			Set( p_effect_id, PositionType.ZERO, ForwardType.ZERO, Vector3.zero, p_gb_type, p_time );
		}

		private void Set( int p_effect_id, PositionType p_pos_type, ForwardType p_forward_type, Vector3 p_custom_forward, GameObjectType p_gb_type, float p_time ){
			m_will_play_effect = true;
			
			m_effect_id = p_effect_id;
			
			m_position_type = p_pos_type;
			
			m_forward_type = p_forward_type;

			m_custom_forward = p_custom_forward;
			
			m_gb_type = p_gb_type;

			m_time = p_time;
		}
	}
}
