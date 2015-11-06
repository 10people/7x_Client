using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_0616_Mecanim : MonoBehaviour {

		
	private Animator m_animator;
		
	private Vector3 m_joystick_speed;

	
	private int m_pre_combo_action_count = 0;
	
	private string m_pre_combo_action_name = "";
	
	private float m_pre_combo_action_0_time = 0.0f;
	
	private string pre_clip_name = "";
	
	private Vector3 m_pre_combo_action_des = new Vector3( 0, 0,0 );
	
	
	private Dictionary< string, DebugACTBase > m_act_data = new Dictionary< string, DebugACTBase >();

	private Dictionary< string, DebugScreenShake > m_shake_data = new Dictionary< string, DebugScreenShake >();

	
	#region Mono
	
	void Awake(){
		{
			DebugACTBase[] t_acts = GetComponents<DebugACTBase>();
			
			foreach( DebugACTBase t_act in t_acts ){
				m_act_data.Add( t_act.m_act_name, t_act );
			}
			
			Debug.Log( "Act Data Count: " + m_act_data.Count );
		}

		{
			DebugScreenShake[] t_shakes = GetComponents<DebugScreenShake>();
			
			foreach( DebugScreenShake t_shake in t_shakes ){
				m_shake_data.Add( t_shake.m_act_name, t_shake );
			}
			
			Debug.Log( "Shake Data Count: " + m_shake_data.Count );
		}
	}
	
	// Use this for initialization
	void Start () {
		m_animator = GetComponent<Animator>();
		
		Debug.Log( "0: " +  m_animator.GetLayerName( 0 ) );
	}
	
	void OnDestroy(){
		m_act_data.Clear();

		m_shake_data.Clear();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement();
		
		//LogAnimationClipState();
		
		{
			ClearComboAction();
		}
	}
	
	void UpdateMovement(){
		//Debug.Log( "UpdateMovement()" );

		if( !IsAttacking() ){
			gameObject.transform.position = gameObject.transform.position + Time.deltaTime * m_joystick_speed;
			
			UpdateRotation( gameObject );
		}
	}
	
	void UpdateRotation( GameObject p_gb ){
		//Debug.Log( "UpdateRotation( " + m_joystick_speed + " )" );

		if( m_joystick_speed.magnitude > 0 ){
			p_gb.transform.LookAt( p_gb.transform.position + m_joystick_speed * 10 );
		}
	}
	
	#endregion


	#region Animation callback

	public void On_Attack_0_Move( AnimationEvent p_event ){
		Debug.Log( "On_Attack_0_Move E(" + p_event + " )" + gameObject.name );
	}

	/*
	public void On_Attack_0_Move( float p_float ){
		Debug.Log( "On_Attack_0_Move F( " + p_float + " )" );
		
	}

	public void On_Attack_0_Move(  int p_int ){
		Debug.Log( "On_Attack_0_Move I( " + p_int + " )" );
		
	}

	public void On_Attack_0_Move( string p_name ){
		Debug.Log( gameObject.name +  ".On_Attack_0_Move S( " + p_name + " )" );

		m_is_callbacking = true;
	}
	*/

	public bool m_is_callbacking = false;


	#endregion
	
	
	
	#region UI Interaction
	
	public void Action_A(){
		//Debug.Log( "Action_A()" );

		if( IsIdlingOrRunning() && m_pre_combo_action_count == 0 ){
			Debug.Log( "Stage 0" );
			
			StopMove();
			
			m_animator.SetTrigger( "t_attack_0" );
			
			UpdateCombo( "attack_0" );
			
			return;
		}
		else if( IsComboing( 1, "attack_0" ) ){
			Debug.Log( "Stage 1" );
			
			m_animator.SetTrigger( "t_attack_1" );
			
			UpdateCombo( "attack_1" );
			
			return;
		}
		else if( IsComboing( 2, "attack_1" ) ){
			Debug.Log( "Stage 2" );
			
			m_animator.SetTrigger( "t_attack_2" );
			
			UpdateCombo( "attack_2" );
			
			return;
		}
		
		Debug.Log( "Action_A not clickable: " + m_pre_combo_action_count + " - " + m_pre_combo_action_name );
		
		LogAnimationClipState();
	}
	
	public void Action_B(){
		if( IsIdlingOrRunning() && m_pre_combo_action_count == 0 ){
			StopMove();
			
			m_animator.SetTrigger( "t_attack_0" );
						
			ActionMovement( "attack_0", null );
			
			UpdateCombo( "attack_0" );
			
			return;
		}
		else if( IsComboing( 1, "attack_0" ) ){
			m_animator.SetTrigger( "t_attack_1" );
			
			ActionMovement( "attack_1", new string[]{ "attack_0" } );
			
			UpdateCombo( "attack_1" );
			
			return;
		}
		else if( IsComboing( 2, "attack_1" ) ){
			m_animator.SetTrigger( "t_attack_2" );
			
			ActionMovement( "attack_2", new string[]{ "attack_0", "attack_1" } );
			
			UpdateCombo( "attack_2" );
			
			return;
		}
		
		LogAnimationClipState();
	}
	
	public void Action_C(){
		if( !IsIdlingOrRunning() ){
			return;
		}
		
		StopMove();


		m_animator.SetTrigger( "t_attack_0" );

		ActionMovement( "attack_0", null );

		UpdateCombo( "attack_0" );


		m_animator.SetTrigger( "t_attack_1" );

		ActionMovement( "attack_1", new string[]{ "attack_0" } );

		UpdateCombo( "attack_1" );


		m_animator.SetTrigger( "t_attack_2" );

		ActionMovement( "attack_2", new string[]{ "attack_0", "attack_1" } );

		UpdateCombo( "attack_2" );
	}
	
	private void ActionMovement( string p_act_name, params string[] p_pre_acts ){
		bool t_is_combo = ( p_pre_acts != null && p_pre_acts.Length > 0 );

		if( t_is_combo ){
			TransformHelper.StoreTransform( gameObject );
			
			gameObject.transform.position = m_pre_combo_action_des;
			
			UpdateRotation( gameObject );
		}

		Vector3 t_dir_global = gameObject.transform.TransformDirection( m_act_data[ p_act_name ].m_move_self );
		
		m_pre_combo_action_des = gameObject.transform.position + t_dir_global;
		
		if( t_is_combo ){
			TransformHelper.RestoreTransform( gameObject );
		}
		
		float t_pre_duration_total = 0.0f;
		
		if( t_is_combo ){
			foreach( string t_act_name in p_pre_acts ){
				t_pre_duration_total += m_act_data[ t_act_name ].GetTotalDuration();
			}
		}

		if( t_dir_global.magnitude > 0 ){
			iTween.LookTo( 
			              gameObject,
			              iTween.Hash( "looktarget", m_pre_combo_action_des,
			            "delay", t_pre_duration_total - ( t_is_combo ? ( Time.time - m_pre_combo_action_0_time ) : 0 ),
			            "time", 0 ) );
		}
		
		iTween.MoveTo(
			gameObject, 
			iTween.Hash( "position", m_pre_combo_action_des, 
		            "delay", ( t_pre_duration_total + m_act_data[ p_act_name ].m_delay ) - ( t_is_combo ? ( Time.time - m_pre_combo_action_0_time ) : 0 ),
		            "time", m_act_data[ p_act_name ].m_move_duration, 
		            "easeType", m_act_data[ p_act_name ].m_ease_type ) );

		if( m_shake_data.ContainsKey( p_act_name ) ){
			m_shake_data[ p_act_name ].Shake( t_pre_duration_total - ( t_is_combo ? ( Time.time - m_pre_combo_action_0_time ) : 0 ) );
		}
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
	
	private bool IsAttacking(){
		if( m_pre_combo_action_count > 0 ){
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
		
//		for( int i = 0; i < t_states.Length; i++ ){
//			AnimationInfo t_item = t_states[ i ];
//			
//			if( t_item.clip.name == "run" ){
//				return true;
//			}
//		}
		
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
