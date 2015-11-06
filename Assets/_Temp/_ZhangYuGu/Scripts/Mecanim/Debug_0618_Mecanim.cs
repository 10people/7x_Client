using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_0618_Mecanim : MonoBehaviour {

	public GameObject m_act_range_root;


	private Animator m_animator;

	private Vector3 m_collider_origin_size;

	private Vector3 m_collider_origin_center;
		
	private Vector3 m_joystick_speed;

	
	private int m_pre_combo_action_count = 0;
	
	private string m_pre_combo_action_name = "";
	
//	private float m_pre_combo_action_0_time = 0.0f;
	
	private string pre_clip_name = "";
	
	private Vector3 m_pre_combo_action_des = new Vector3( 0, 0,0 );
	
	
	private Dictionary< string, Debug_ACT_Movement > m_act_move_data = new Dictionary< string, Debug_ACT_Movement >();

	private Dictionary< string, DebugScreenShake > m_act_shake_data = new Dictionary< string, DebugScreenShake >();

	private Dictionary< string, Vector3 > m_act_dir_data = new Dictionary< string, Vector3 >();

	private List<Debug_Attack_Range> m_act_ranges = new List<Debug_Attack_Range>();


	private Dictionary< string, Debug_Fx_Config > m_fx_data = new Dictionary< string, Debug_Fx_Config >();

	#region Mono
	
	void Awake(){
		{
			Debug_ACT_Movement[] t_acts = GetComponents<Debug_ACT_Movement>();
			
			foreach( Debug_ACT_Movement t_act in t_acts ){
				m_act_move_data.Add( t_act.m_act_name, t_act );
			}
			
			Debug.Log( "Act Data Count: " + m_act_move_data.Count );
		}

		{
			DebugScreenShake[] t_shakes = GetComponents<DebugScreenShake>();
			
			foreach( DebugScreenShake t_shake in t_shakes ){
				m_act_shake_data.Add( t_shake.m_act_name, t_shake );
			}
			
			Debug.Log( "Shake Data Count: " + m_act_shake_data.Count );
		}

		{
			m_act_dir_data.Clear();
		}

		{
			
			int t_child_count = m_act_range_root.transform.childCount;
			
			for( int i = 0; i < t_child_count; i++ ){
				GameObject t_gb = m_act_range_root.transform.GetChild( i ).gameObject;
				
				Debug_Attack_Range t_range = t_gb.GetComponent<Debug_Attack_Range>();

				m_act_ranges.Add( t_range );
			}
		}

		{
			Debug_Fx_Config[] t_fxs = GetComponentsInChildren<Debug_Fx_Config>();
			
			foreach( Debug_Fx_Config t_fx in t_fxs ){
				m_fx_data.Add( t_fx.m_fx_name, t_fx );
			}
			
			Debug.Log( "Fxs Data Count: " + m_fx_data.Count );
		}
	}
	
	// Use this for initialization
	void Start () {
		m_animator = GetComponent<Animator>();

		m_collider_origin_size = GetComponent<BoxCollider>().size;

		m_collider_origin_center = GetComponent<BoxCollider>().center;
		
		Debug.Log( "0: " +  m_animator.GetLayerName( 0 ) );
	}
	
	void OnDestroy(){
		m_act_move_data.Clear();

		m_act_shake_data.Clear();

		m_act_dir_data.Clear();

		m_act_ranges.Clear();
	}

	void LateUpdate(){

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



	#endregion
		

	
	#region UI Interaction
		
	public void Action_As(){
		Debug.Log( "Action_A()" );
		
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
			
			//ActionMovement( "attack_1", new string[]{ "attack_0" } );
			
			UpdateCombo( "attack_1", m_joystick_speed );
			
			return;
		}
		else if( IsComboing( 2, "attack_1" ) ){
			Debug.Log( "Stage 2" );
			
			m_animator.SetTrigger( "t_attack_2" );
			
			//ActionMovement( "attack_2", new string[]{ "attack_0", "attack_1" } );
			
			UpdateCombo( "attack_2", m_joystick_speed );
			
			return;
		}
		
		Debug.Log( "Action_B not clickable: " + m_pre_combo_action_count + " - " + m_pre_combo_action_name );
		
		LogAnimationClipState();
	}
	
	public void Action_B(){
		//Debug.Log( "Action_B()" );
		
		if( !IsIdlingOrRunning() ){
			return;
		}
		
		StopMove();


		m_animator.SetTrigger( "t_attack_0" );

		//ActionMovement( "attack_0", null );

		UpdateCombo( "attack_0" );


		m_animator.SetTrigger( "t_attack_1" );

		//ActionMovement( "attack_1", new string[]{ "attack_0" } );

		UpdateCombo( "attack_1" );


		m_animator.SetTrigger( "t_attack_2" );

		//ActionMovement( "attack_2", new string[]{ "attack_0", "attack_1" } );

		UpdateCombo( "attack_2" );
	}
		
	#endregion



	#region Collide

	void OnTriggerEnter(Collider other) {
		//Debug.Log( gameObject + ".OnTriggerEnter( " + other.gameObject + " )" );
		
		string t_cur_act = GetCurSoloActionName();

		Debug.Log( "Cur Act: " + t_cur_act );

		if( m_act_move_data.ContainsKey( t_cur_act ) ){
			Debug_ACT_Movement t_move = m_act_move_data[ t_cur_act ];

			if( t_move.m_stop_if_collide ){
				Debug.Log( "Stop Move: " + t_move.m_act_name );

				iTween.StopByName( gameObject, t_cur_act );
			}
		}
	}

	#endregion



	#region Action Callbacks

	private void Attack_Open_Cal( string p_act ){
		//Debug.Log( "Attack_Open_Cal( " + p_act + " )" );

		for( int i = 0; i < m_act_ranges.Count; i++ ){
			if( m_act_ranges[ i ].m_act_name == p_act ){
				m_act_ranges[ i ].gameObject.SetActive( true );
			}
		}
	}

	private void Attack_Close_Cal( string p_act ){
		//Debug.Log( "Attack_Close_Cal( " + p_act + " )" );

		for( int i = 0; i < m_act_ranges.Count; i++ ){
			m_act_ranges[ i ].gameObject.SetActive( false );
		}
	}

	private void ActionRotation( string p_act_name ){
		//Debug.Log( "ActionRotation( " + p_act_name + " ) " );

		PlayFx( p_act_name );

		if( !m_act_dir_data.ContainsKey( p_act_name ) ){
			Debug.Log( "dir data not containing: " + p_act_name );
			
			return;
		}
		
		if( m_act_dir_data[ p_act_name ].magnitude <= 0 ){
			return;
		}
		
		Vector3 t_look_target = gameObject.transform.position + m_act_dir_data[ p_act_name ] * 10;
		
		gameObject.transform.LookAt( t_look_target );
	}
	
	private void ActionMovement( string p_act_name ){
		//Debug.Log( "ActionMovement( " + p_act_name + " ) " );
		
		if( !m_act_move_data.ContainsKey( p_act_name ) ){
			Debug.LogWarning( "move data not containing: " + p_act_name );
			
			return;
		}
		
		Vector3 t_dir_global = gameObject.transform.TransformDirection( m_act_move_data[ p_act_name ].m_move_self );
		
		m_pre_combo_action_des = gameObject.transform.position + t_dir_global;
		
		iTween.MoveTo(
			gameObject, 
			iTween.Hash( 
		            "name", p_act_name,
		            "position", m_pre_combo_action_des, 
		            "delay", m_act_move_data[ p_act_name ].m_delay,
		            "time", m_act_move_data[ p_act_name ].m_move_duration, 
		            "easeType", m_act_move_data[ p_act_name ].m_ease_type ) );

		GetComponent<BoxCollider>().size = m_collider_origin_size * 2;

		GetComponent<BoxCollider>().center = m_collider_origin_center * 2;
	}
	
	private void ActionShake( string p_act_name ){
		//Debug.Log( "ActionShake( " + p_act_name + " ) " );
		
		if( m_act_shake_data.ContainsKey( p_act_name ) ){
			m_act_shake_data[ p_act_name ].Shake();
		}
	}

	#endregion



	#region Actions
	
	private void StopMove(){
		UpdateSpeed( new Vector3( 0, 0, 0 ) );
	}
	
	private void UpdateCombo( string p_combo_name ){
		if( m_pre_combo_action_count == 0 ){
//			m_pre_combo_action_0_time = Time.time;
		}
		
		m_pre_combo_action_count++;
		
		m_pre_combo_action_name = p_combo_name;
	}

	private void UpdateCombo( string p_combo_name, Vector3 p_joystick ){
		UpdateCombo( p_combo_name );
		
		if( p_joystick.magnitude > 0 ){
			Debug.Log( "UpdateCombo( " + p_combo_name + " - " + p_joystick + " )" );

			m_act_dir_data.Add( p_combo_name, p_joystick );
		}
	}
	
	private bool IsAttacking(){
		if( m_pre_combo_action_count > 0 ){
			return true;
		}
		
		return false;
	}
	
	private void ClearComboAction(){
		if( m_pre_combo_action_count > 0 && IsIdling() && pre_clip_name != "idle" && pre_clip_name != "run" ){
			Debug.Log( "ClearComboAction()" );
			
			m_pre_combo_action_count = 0;
			
			m_pre_combo_action_name = "";
			
//			m_pre_combo_action_0_time = 0.0f;

			m_act_dir_data.Clear();

			GetComponent<BoxCollider>().size = m_collider_origin_size;

			GetComponent<BoxCollider>().center = m_collider_origin_center;
		}
		
		pre_clip_name = GetCurSoloActionName();
	}
	
	#endregion
	
	
	
	#region Utilities

	private void PlayFx( string p_name ){
		if( m_fx_data.ContainsKey( p_name ) ){
			m_fx_data[ p_name ].PlayFx();
		}
		else{
			Debug.Log( "Fx not contained:" + p_name );
		}
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
		
		Debug.Log( "LogAnimationClipState( " + t_states.Length + " )" );
		
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
