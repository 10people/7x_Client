using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_0618_Soldier : MonoBehaviour {

	public Vector3 m_max_move_offset;

	public Vector3 m_max_move_speed = new Vector3( 0, 0, 1 );

	public float m_max_idle_time;

	private Animator m_animator;

	private Dictionary< string, Debug_Fx_Config > m_fx_data = new Dictionary< string, Debug_Fx_Config >();


	private Vector3 m_origin_pos;

	private bool m_attacked = false;

	#region Mono
		
	void Awake(){
		{
			Debug_Fx_Config[] t_fxs = GetComponentsInChildren<Debug_Fx_Config>();
			
			foreach( Debug_Fx_Config t_fx in t_fxs ){
				m_fx_data.Add( t_fx.m_fx_name, t_fx );
			}
			
			Debug.Log( "Fxs Data Count: " + m_fx_data.Count );
		}

		m_origin_pos = transform.position;
	}

	// Use this for initialization
	void Start () {
		m_animator = GetComponent<Animator>();

		RandomIdle();
	}

	#endregion



	#region Action

	private void PlayIdle(){
		//Debug.Log( "PlayIdle()" );

		if( m_attacked ){
			RandomIdle();

			m_attacked = false;
		}
	}

	private void StopMove(){
		UpdateSpeed( new Vector3( 0, 0, 0 ) );
	}

	#endregion


	#region Attack Interaction

	public void OnAttacked( Debug_Attack_Range p_attack ){
		Debug.Log( "OnAttacked( " + p_attack + " )" );

		StopMove();

		iTween.Stop( gameObject );

		m_animator.SetTrigger( "t_attacked" );

		gameObject.transform.LookAt( p_attack.transform.position );

		PlayFx( "hitted" );

		m_attacked = true;
	}


	#endregion



	#region Actions

	private void RandomIdle(){
		float t_time = UtilityTool.GetRandom( 1, m_max_idle_time );

		//Debug.Log( "RandomIdle( " + t_time + " )" );

		StopMove();

		//iTween.Stop( gameObject );

		iTween.ValueTo(
			gameObject,
			iTween.Hash( "from", t_time,
		            "to", 0,
		            "time", t_time,
		            "easetype", iTween.EaseType.linear,
		            "onupdate", "OniTween",
		            "oncomplete", "OnIdleDone" ) );
	}

	public void OniTween( float p_time ){

	}

	public void OnIdleDone(){
		//Debug.Log( "OnIdleDone()" );

		iTween.Stop( gameObject );

		RandomMove();
	}

	private void RandomMove(){
		TransformHelper.StoreTransform( gameObject );

		gameObject.transform.position = m_origin_pos;

		gameObject.transform.Rotate( 0, UtilityTool.GetRandom( 0, 360 ), 0, Space.Self );

		Vector3 t_dest = gameObject.transform.TransformPoint( new Vector3(
			UtilityTool.GetRandom( 0, m_max_move_offset.x ),
			UtilityTool.GetRandom( 0, m_max_move_offset.y ),
			UtilityTool.GetRandom( 0, m_max_move_offset.z ) ) );

		TransformHelper.RestoreTransform( gameObject );

		//Debug.Log( "RandomMove( " + t_dest + " )" );

		m_animator.SetFloat( "f_speed", m_max_move_speed.magnitude );

		iTween.MoveTo(
			gameObject, 
			iTween.Hash( 
		            "position", t_dest, 
		            "time", ( t_dest - gameObject.transform.position ).magnitude / m_max_move_speed.magnitude, 
		            "easeType", iTween.EaseType.linear,
		            "oncomplete", "OnMoveDone" ) );

		gameObject.transform.LookAt( t_dest );
	}

	private void OnMoveDone(){
		//Debug.Log( "OnMoveDone()" );
		
		iTween.Stop( gameObject );
		
		RandomIdle();
	}

	#endregion


	#region Fxs



	#endregion



	#region Utilities

	public void UpdateSpeed( Vector3 p_speed ){
		m_animator.SetFloat( "f_speed", p_speed.magnitude ); 
	}

	private void PlayFx( string p_name ){
		if( m_fx_data.ContainsKey( p_name ) ){
			m_fx_data[ p_name ].PlayFx();
		}
		else{
			Debug.Log( "Fx not contained:" + p_name );
		}
	}

	#endregion
}