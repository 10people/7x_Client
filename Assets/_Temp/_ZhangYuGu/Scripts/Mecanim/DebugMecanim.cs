using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugMecanim : MonoBehaviour {



	public float m_time_scale = 1.0f;



	public float t_com_0_delay = 0.2f;

	public Vector3 t_com_0_offset = new Vector3( 0, 0, 1.5f );

	public float t_com_0_duration = 0.3f;


	public float t_com_1_delay = 0.1f;
	
	public Vector3 t_com_1_offset = new Vector3( 0, 0, 1.0f );
	
	public float t_com_1_duration = 0.2f;


	public float t_com_2_delay = 0.1f;
	
	public Vector3 t_com_2_offset = new Vector3( 0, 0, 2.5f );
	
	public float t_com_2_duration = 0.2f;



	private Animator m_animator;


	private Vector3 m_joystick_speed;


	private int m_pre_combo_action_count = 0;

	private string m_pre_combo_action_name = "";

	private float m_pre_combo_action_0_time = 0.0f;

	private string pre_clip_name = "";

	private Vector3 m_pre_combo_action_des = new Vector3( 0, 0,0 );


	private Dictionary< string, DebugACTBase > m_act_datas = new Dictionary< string, DebugACTBase >();


	#region Mono

	void Awake(){


		{
			DebugACTBase[] t_acts = GetComponents<DebugACTBase>();

			foreach( DebugACTBase t_act in t_acts ){
				m_act_datas.Add( t_act.m_act_name, t_act );
			}
		}
	}

	// Use this for initialization
	void Start () {

		Time.timeScale = m_time_scale;

		m_animator = GetComponent<Animator>();
	}

	void OnDestroy(){
		m_act_datas.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		if( Time.timeScale != m_time_scale ){
			Time.timeScale = m_time_scale;
		}

		UpdateMovement();

		//LogAnimationClipState();

		{
			ClearComboAction();
		}
	}

	void UpdateMovement(){
		if( IsPlaying( "idle" ) || IsPlaying( "run" ) ){
			gameObject.transform.position = gameObject.transform.position + Time.deltaTime * m_joystick_speed;

			UpdateRotation( gameObject );
		}
	}

	void UpdateRotation( GameObject p_gb ){
		if( m_joystick_speed.magnitude > 0 ){
			p_gb.transform.LookAt( p_gb.transform.position + m_joystick_speed * 10 );
		}
	}

	#endregion



	#region UI Interaction

	public void Action_A(){
		if( !IsIdlingOrRunning() ){
			return;
		}

		StopMove();

		m_animator.SetTrigger( "t_attack" );

		ActionMovement( "attack",  null );
	}

	public void Action_B(){
		if( IsIdlingOrRunning() && m_pre_combo_action_count == 0 ){
			StopMove();

			m_animator.SetTrigger( "t_attack_com_p_0" );
						
			m_animator.SetBool( "b_attack_com_p_1", false );
			
			m_animator.SetBool( "b_attack_com_p_2", false );

			ActionMovement( "attack_com_0", null );

			UpdateCombo( "attack_com_0" );

			return;
		}
		else if( IsComboing( 1, "attack_com_0" ) ){
			m_animator.SetBool( "b_attack_com_p_1", true );

			ActionMovement( "attack_com_1", new string[]{ "attack_com_0" } );

			UpdateCombo( "attack_com_1" );

			return;
		}
		else if( IsComboing( 2, "attack_com_1" ) ){
			m_animator.SetBool( "b_attack_com_p_2", true );

			ActionMovement( "attack_com_2", new string[]{ "attack_com_0", "attack_com_1" } );

			UpdateCombo( "attack_com_2" );

			return;
		}

		LogAnimationClipState();
	}

	public void Action_C(){
		if( !IsIdlingOrRunning() ){
			return;
		}

		StopMove();

		m_animator.SetTrigger( "t_attack_com_p_0" );

		m_animator.SetBool( "b_attack_com_p_1", true );

		m_animator.SetBool( "b_attack_com_p_2", true );

		{
			Vector3 t_dir_global = gameObject.transform.TransformDirection( t_com_0_offset );
			
			iTween.MoveTo(
				gameObject, 
				iTween.Hash( "position", gameObject.transform.position + t_dir_global, 
			            "time", t_com_0_duration, 
			            "easeType", iTween.EaseType.easeInOutQuint,
			            "delay", t_com_0_delay ) );
		}

		{
			Vector3 t_dir_global = gameObject.transform.TransformDirection( t_com_0_offset + t_com_1_offset );
			
			iTween.MoveTo(
				gameObject, 
				iTween.Hash( "position", gameObject.transform.position + t_dir_global, 
			            "time", t_com_1_duration, 
			            "easeType", iTween.EaseType.easeInOutQuint,
			            "delay", t_com_0_delay + t_com_0_duration + t_com_1_delay ) );
		}

		{
			Vector3 t_dir_global = gameObject.transform.TransformDirection( t_com_0_offset + t_com_1_offset + t_com_2_offset );
			
			iTween.MoveTo(
				gameObject, 
				iTween.Hash( "position", gameObject.transform.position + t_dir_global, 
			            "time", t_com_2_duration, 
			            "easeType", iTween.EaseType.easeInOutQuint,
			            "delay", t_com_0_delay + t_com_0_duration + t_com_1_delay + t_com_1_duration + t_com_2_delay ) );
		}
	}

	private void ActionMovement( string p_act_name, params string[] p_pre_acts ){
		bool t_is_combo = ( p_pre_acts != null && p_pre_acts.Length > 0 );

		if( t_is_combo ){
			TransformHelper.StoreTransform( gameObject );
			
			gameObject.transform.position = m_pre_combo_action_des;
			
			UpdateRotation( gameObject );
		}

		Vector3 t_dir_global = gameObject.transform.TransformDirection( m_act_datas[ p_act_name ].m_move_self );

		m_pre_combo_action_des = gameObject.transform.position + t_dir_global;

		if( t_is_combo ){
			TransformHelper.RestoreTransform( gameObject );
		}

		float t_pre_duration_total = 0.0f;

		if( t_is_combo ){
			foreach( string t_act_name in p_pre_acts ){
				t_pre_duration_total += m_act_datas[ t_act_name ].GetTotalDuration();
			}
		}

		if( t_dir_global.magnitude > 0 ){
			iTween.LookTo( 
				gameObject,
				iTween.Hash( "looktarget", m_pre_combo_action_des,
		        	    "delay", t_pre_duration_total - ( Time.time - m_pre_combo_action_0_time ),
		            	"time", 0 ) );
		}

		iTween.MoveTo(
			gameObject, 
			iTween.Hash( "position", m_pre_combo_action_des, 
		            "delay", ( t_pre_duration_total + m_act_datas[ p_act_name ].m_delay ) - ( t_is_combo ? ( Time.time - m_pre_combo_action_0_time ) : 0 ),
		            "time", m_act_datas[ p_act_name ].m_move_duration, 
		            "easeType", m_act_datas[ p_act_name ].m_ease_type ) );
	}

	#endregion


	#region Actions

	private void StopMove(){
		UpdateSpeed( new Vector3( 0, 0, 0 ) );
	}

	private void UpdateCombo( string p_combo_name ){
		if( m_pre_combo_action_count == 0 ){
			m_pre_combo_action_0_time = Time.time;
		}

		m_pre_combo_action_count++;
		
		m_pre_combo_action_name = p_combo_name;
	}

	private bool IsComboing(){
		if( m_pre_combo_action_count > 0 && GetCurSoloActionName() != "idle" ){
			return true;
		}

		return false;
	}

	private void ClearComboAction(){
		if( m_pre_combo_action_count > 0 && IsIdling() && pre_clip_name != "idle" && pre_clip_name != "run" ){
			m_pre_combo_action_count = 0;
			
			m_pre_combo_action_name = "";

			m_pre_combo_action_0_time = 0.0f;

		}

		pre_clip_name = GetCurSoloActionName();
	}

	#endregion



	#region Utilities

	public float GetAnimationStateDuration( string p_animation_state_name ){
		switch( p_animation_state_name ){
		case "attack":
			return 1f;

		case "attack_com_0":
			return 0.5238097f * 1.12f;

		case "attack_com_1":
			return 0.2666674f * 1.2f;

		case "attack_com_2":
			return 0.8444455f;

		default:

			break;
		}

		Debug.LogError( "Error, State Not Exist: " + p_animation_state_name );

		return 0.0f;
	}

	private bool IsComboing( int p_pre_count, string p_pre_name ){
		return ( m_pre_combo_action_count == p_pre_count && m_pre_combo_action_name == p_pre_name );
	}
		
	public void UpdateSpeed( Vector3 p_speed ){
		m_joystick_speed = p_speed;
		
		m_animator.SetFloat( "f_speed", m_joystick_speed.magnitude ); 
	}

	private bool IsPlaying( string p_action_name ){
		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo( 0 );
		
		if( t_states.Length > 1 ){
			return false;
		}
		else if( t_states.Length == 0 ){
			return false;
		}
		
		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];
			
			if( t_item.clip.name == p_action_name ){
				return true;
			}
		}
		
		return false;
	}

	public string GetCurSoloActionName(){
		string t_default_name = "";

		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo( 0 );
		
		if( t_states.Length > 1 ){
			return t_default_name;
		}
		else if( t_states.Length == 0 ){
			return t_default_name;
		}
		
//		for( int i = 0; i < t_states.Length; i++ ){
//			AnimationInfo t_item = t_states[ i ];
//			
//			return t_item.clip.name;
//		}
		
		return t_default_name;
	}

	private bool IsIdlingOrRunning(){
		if( IsIdling() ){
			return true;
		}

		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo( 0 );
		
		if( t_states.Length > 1 ){
			return false;
		}
		else if( t_states.Length == 0 ){
			return false;
		}
		
		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];
			
			if( t_item.clip.name == "run" ){
				return true;
			}
		}
		
		return false;
	}

	private bool IsIdling(){
		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo( 0 );

		if( t_states.Length > 1 ){
			return false;
		}
		else if( t_states.Length == 0 ){
			return false;
		}

		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];
			
			if( t_item.clip.name == "idle" ){
				return true;
			}
		}

		return false;
	}

	private void LogAnimationClipState(){
		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo( 0 );

		for( int i = 0; i < t_states.Length; i++ ){
			AnimatorClipInfo t_item = t_states[ i ];

			Debug.Log( t_item.clip.name + ": " + t_item.clip.length );

			AnimatorStateInfo t_state = m_animator.GetCurrentAnimatorStateInfo( 0 );
	
			Debug.Log( i + ":( " + 
			          t_item.clip.name + " - " + 
			          t_item.clip.length + "   -   " + 
			          t_state.loop + ": " + 
			          t_state.normalizedTime + " - " + 
			          t_state.length + " )" );

		}
	}

	#endregion
}
