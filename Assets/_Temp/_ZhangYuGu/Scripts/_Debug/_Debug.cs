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
//		{
//			DateTime t_time = DateTime.Parse( "2008-05-01 21:34:42" );
//
//			Debug.Log( t_time.ToString( "yyyy MM dd   HH mm ss" ) );
//		}
//
//		{
//			TimeHelper.SetServerDateTime( "2016-02-28 21:16:21" );
//
//			PlayerInfoCache.SetRegisterTime( "2016-02-27 21:16:21" );
//		}

//		Debug.Log ( "_Debug.Awake()" );

//		m_instance = this;

//		DontDestroyOnLoad( gameObject );

//		ComponentHelper.HideComponent( transform );

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

//		GameObjectHelper.LogComponents( gameObject );
	}

	// Use this for initialization
	void Start () {
//		Debug.Log ( "_Debug.Start()" );

//		NavTest();
	}

	public bool m_log_button_status = false;
	
	public bool m_execute = false;
	
	public bool m_button_status = false;

	public bool m_debug_log = false;

	[Range(0, 1)]
	public float m_anim_normalized_time = 0.0f;

	public Vector3 m_delta_move = new Vector3( 0.001f, 0, 0 );

	void Update () {
		{
			Debug.Log( "Server Time: " + TimeHelper.GetCurServerDateTimeString() + "   IsFirstDay: " + LoadingTemplate.IsFirstDay() );
		}

//		{
//			CharacterController t_control = GetComponent<CharacterController>();
//
//			if( t_control != null ){
//				t_control.Move( m_delta_move );
//			}
//		}

//		{
//			Animator t_anim = gameObject.GetComponent<Animator>();
//
//			if( t_anim == null ){
//				return;
//			}
//
//			t_anim.Play( t_anim.GetCurrentAnimatorStateInfo( 0 ).shortNameHash, 
//				-1, 
//				m_anim_normalized_time );
//			
//			t_anim.Update( 0.0f );
//		}

//		TraceAudioSource();

//		AutoLogin();

//		if( m_debug_log ){
//			Debug.Log( "HaveInstance: " + UI3DEffectTool.HaveInstance() );
//
//			m_debug_log = false;
//		}

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

//		if (m_execute) {
//			m_execute = false;
//			
//			UIButton t_btn = GetComponent<UIButton>();
//			
//			t_btn.isEnabled = m_button_status;
//		}
//		
//		if (m_log_button_status) {
//			m_log_button_status = false;
//			
//			UIButton t_btn = GetComponent<UIButton>();
//			
//			Debug.Log( "Log btn status: " + t_btn.isEnabled );
//		}
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
//		Debug.Log ( "_Debug.OnDestroy()" );

		m_instance = null;
	}

	#endregion



	#region AudioSource Trace

	private void TraceAudioSource(){
		AudioSource[] t_sources = GetComponents<AudioSource>();

		for( int i = t_sources.Length - 1; i >= 0; i-- ){
			if( SoundHelper.IsRemovable( t_sources[ i ] ) ){
				Debug.Log( "AudioSoource Removable: " + t_sources[ i ] );
			}
			else{
				ComponentHelper.LogAudioSource( 
					t_sources[ i ], 
					"",
					i + ": " + GameObjectHelper.GetGameObjectHierarchy( t_sources[ i ] ) );
			}
		}
	}

	#endregion



	#region Auto Login

	private int m_login_count = 0;

	private enum AutoLoginFlow{
		LogOut_Confirmed,
		Notice_Confirmed,
		AC_PN_Confirmed,
		Login_Confirmed,
		Enter_Game_Confirmed,
	}

	private AutoLoginFlow m_flow = AutoLoginFlow.LogOut_Confirmed;

	private void AutoLogin(){
		if( Time.frameCount % 3 != 0 ){
			return;
		}

		if( SceneManager.IsInLoginScene() &&
		   m_flow == AutoLoginFlow.LogOut_Confirmed ){
			NoticeManager t_notice = Component.FindObjectOfType<NoticeManager>();
		
			if( t_notice != null ){
				if( t_notice.gameObject.activeInHierarchy ){
					t_notice.gameObject.SendMessage( "CloseBtn" );
					
//					Debug.Log( "Notice Confirmed." );
					
					m_flow = AutoLoginFlow.Notice_Confirmed;
				}
			}
		}

		if( SceneManager.IsInLoginScene() &&
		   m_flow == AutoLoginFlow.Notice_Confirmed ){
			AccountRequest t_cs = Component.FindObjectOfType<AccountRequest>();

			if( t_cs != null ){
//				t_cs.userName.value = "110";
//				
//				t_cs.password.value = "110";

				t_cs.userName.value = "q123";
				
				t_cs.password.value = "q123";
				
//				Debug.Log( "Account&Password Confirmed." );
				
				m_flow = AutoLoginFlow.AC_PN_Confirmed;
			}
		}

		if( SceneManager.IsInLoginScene() &&
		   m_flow == AutoLoginFlow.AC_PN_Confirmed ){
			AccountRequest t_cs = Component.FindObjectOfType<AccountRequest>();
			
			t_cs.SendMessage( "DengLuRequestSend" );
				
//			Debug.Log( "Login Confirmed." );

			m_flow = AutoLoginFlow.Login_Confirmed;
		}

		if( SceneManager.IsInLoginScene() &&
		   m_flow == AutoLoginFlow.Login_Confirmed ){
			EnterGame t_cs = Component.FindObjectOfType<EnterGame>();

			if( t_cs != null ){
				t_cs.SendMessage( "EnterGameReq" );
				
//				Debug.Log( "Enter Game Confirmed." );
				
				m_flow = AutoLoginFlow.Enter_Game_Confirmed;
				
				{
					m_login_count++;
					
					Debug.Log( "LoginCount: " + m_login_count );
				}
			}
		}

		if( SceneManager.IsInMainCityScene() &&
		   m_flow == AutoLoginFlow.Enter_Game_Confirmed ){
			if( EnterNextScene.Instance() == null && !StaticLoading.HaveInstance() ){
				SettingUpLayerManangerment.SwitchAccountController();
		
//				Debug.Log( "Account LogOut." );

				m_flow = AutoLoginFlow.LogOut_Confirmed;
			}
		}
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
