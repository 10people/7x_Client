using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//#if UNITY_EDITOR 
//using UnityEditor;
//using UnityEditorInternal;
//#endif

public class _Debug : MonoBehaviour {

	public Animator m_animator = null;

	private static _Debug m_instance = null;

	public Color m_temp_color;

	public _Empty empty_target = null;

	#region Mono

	void Awake(){
		Debug.Log ( "_Debug.Awake()" );

		m_instance = this;

		DontDestroyOnLoad( gameObject );

		ComponentHelper.HideComponent( transform );

//		{
//			Debug.Log( "01:01:01: " + TimeHelper.GetUniformedTimeString( 1 *60 * 60 + 1 * 60 + 1 ) );
//
//			Debug.Log( "01:01: " + TimeHelper.GetUniformedTimeString( 1 * 60 + 1 ) );
//		}

//		{
//			GameObject t_gb = new GameObject( "_Empty_GameObject" );
//
//			empty_target = (_Empty)ComponentHelper.AddIfNotExist( t_gb, typeof(_Empty) );
//			
//			Destroy( t_gb );
//
//			DestroyObject( _EmptyScript.Instance() );
//		}

		GameObjectHelper.LogComponents( gameObject );
	}

	// Use this for initialization
	void Start () {
		Debug.Log ( "_Debug.Start()" );

//		NavTest();
	}

	public bool m_log_button_status = false;
	
	public bool m_execute = false;
	
	public bool m_button_status = false;

	void Update () {
		//		Debug.Log ( "Update.TimeScale: " + Time.timeScale );
		
		//		ConTest();

//		if( empty_target != null ){
//			Debug.Log( "_Empty mono still here: " + empty_target.GetFrameCount() );
//		}
//		else{
//			Debug.Log( "_Empty mono is null." );
//		}
//
//		if( _Empty.m_instance != null ){
//			Debug.Log( "_Empty's instance is still there." );
//		}
//		else{
//			Debug.Log( "_Empty's instance is null." );
//		}

		if (m_execute) {
			m_execute = false;
			
			UIButton t_btn = GetComponent<UIButton>();
			
			t_btn.isEnabled = m_button_status;
		}
		
		if (m_log_button_status) {
			m_log_button_status = false;
			
			UIButton t_btn = GetComponent<UIButton>();
			
			Debug.Log( "Log btn status: " + t_btn.isEnabled );
		}
	}
	
	void LateUpdate() {
		
	}

	void OnGUI(){
//		{
//			GUIHelper.GUILayoutVerticalSpace( 0.1f );
//		}
//		
//		GUILayout.BeginVertical();
//		
//		{
//			string t_path = "_3D/Fx/Prefabs/BattleEffect/BOSSwudi";
//
//			if( GUILayout.Button( "Global Fx" ) ){
//				FxTool.PlayGlobalFx( t_path, FxLoadDelegate );
//			}
//
//			if( GUILayout.Button( "Global Fx X" ) ){
//				FxTool.PlayGlobalFx( t_path, FxLoadDelegate, new Vector3( 10, 10, 10 ), new Vector3( 30, 30, 30 ) );
//			}
//			
//			if( GUILayout.Button( "Local Fx" ) ){
//				FxTool.PlayLocalFx( t_path, gameObject, FxLoadDelegate );
//			}
//
//			if( GUILayout.Button( "Local Fx" ) ){
//				FxTool.PlayLocalFx( t_path, gameObject, FxLoadDelegate, new Vector3( -10, -10, -10 ), new Vector3( -30, -30, -30 ) );
//			}
//		}
//		
//		GUILayout.EndVertical();
	}

	public void FxLoadDelegate( GameObject p_fx ){
		Debug.Log( "FxLoadDelegate( " + p_fx + " )" );

		GameObjectHelper.LogGameObjectInfo( p_fx );
	}

	void OnDestroy(){
		Debug.Log ( "_Debug.OnDestroy()" );

		m_instance = null;
	}

	#endregion



	#region Utilities

	public NavMeshAgent t_agent_a;

	public Vector3 t_a_dest = new Vector3( 0, 0, 0 );

	public NavMeshAgent t_agent_b;

	public Vector3 t_b_dest = new Vector3( 0, 0, 0 );
	
	private void NavTest(){
		t_agent_a.SetDestination( t_a_dest );
		
		t_agent_b.SetDestination( t_b_dest );
	}


	public CharacterController t_con_a;

	public Vector3 t_a_delta = new Vector3( 0, 0, 0 );

	public CharacterController t_con_b;

	public Vector3 t_b_delta = new Vector3( 0, 0, 0 );

	private void ConTest(){
		t_con_a.Move( t_a_delta );

		t_con_b.Move( t_b_delta );
	}

	#endregion
}
