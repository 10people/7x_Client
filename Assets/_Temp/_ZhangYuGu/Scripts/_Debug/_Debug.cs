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

	public Texture2D m_tex;

	public Texture2D m_self_copy_tex;

	#region Mono

	private void SetUITexture( Texture2D p_tex ){
		UITexture t_tex = GetComponent<UITexture>();

		if( t_tex == null ){
			return;
		}

		if( p_tex == null ){
			return;
		}

		{
			t_tex.mainTexture = p_tex;
		}
	}

	_Debug(){
//		Debug.Log ( "_Debug()" );
	}

	~_Debug(){
//		Debug.Log ( "~_Debug()" );
	}

	void FindAllUISprite(){
		UnityEngine.Object[] t_objects = Resources.FindObjectsOfTypeAll( typeof(UISprite) );

		int t_count = 0;

		for( int i = t_objects.Length - 1; i >= 0; i-- ){
			UISprite t_sprite = (UISprite)t_objects[ i ];

			if( t_sprite == null ){
				continue;
			}

			if( !t_sprite.gameObject.activeInHierarchy ){
				ComponentHelper.LogUISprite( t_sprite );
			}
		}
	}

	void Awake(){
		Debug.Log ( "_Debug.Awake()" );

		ModelAutoActivator.Instance();

//		FindAllUISprite();

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

	void OnEnable(){
		Debug.Log( "_Debug.OnEnable()" );


	}

	public GameObject m_gb_destroy;

	public GameObject m_gb_destroy_object;

	// Use this for initialization
	void Start () {

//		Debug.Log( "------ Destroy Info ------" );
//
//		if( m_gb_destroy != null ){
//			Destroy( m_gb_destroy );	
//		}
//
//		Debug.Log( "Gb after destroy: " + m_gb_destroy );
//
//		if( m_gb_destroy_object != null ){
//			DestroyObject( m_gb_destroy_object );	
//		}
//
//		Debug.Log( "Gb after destroy object: " + m_gb_destroy_object );


//		Debug.Log ( "_Debug.Start()" );

//		NavTest();

//		iTween.ValueTo(gameObject, iTween.Hash(
//			"from", 0.0f,
//			"to", 5.0f,
//			"delay", 0,
//			"time", 5.0f,
//			"easetype", iTween.EaseType.linear,
//			"onupdate", "onFloat1stUpdate",
//			"oncomplete", "onFloat1stComplete"
//		));
//
//		iTween.ValueTo(gameObject, iTween.Hash(
//			"from", 0.0f,
//			"to", 5.0f,
//			"delay", 0,
//			"time", 5.0f,
//			"easetype", iTween.EaseType.linear,
//			"onupdate", "onFloat2ndUpdate",
//			"oncomplete", "onFloat2ndComplete"
//		));

//		Time.timeScale = 0;

//		Debug.Log( TimeHelper.GetFrameAndTime() + " iTween.Set." );
//
//		iTween t_itween = iTween.ValueTo(gameObject, iTween.Hash(
//			"from", 0.0f,
//			"to", 5.0f,
//			"delay", 3.0f,
//			"ignoretimescale", true,
//			"time", 3.0f,
//			"easetype", iTween.EaseType.linear,
//			"onstart", "onFloatRealTimeStart",
//			"onupdate", "onFloatRealTimeUpdate",
//			"oncomplete", "onFloatRealTimeComplete"
//		));
//
//		t_itween.SetDebug( true );
	}

	public void onFloatRealTimeStart(){
		Debug.Log( TimeHelper.GetFrameAndTime() + " onFloatRealTimeStart()" );
	}

	public void onFloatRealTimeUpdate( float p_float ){
		Debug.Log( TimeHelper.GetFrameAndTime() + " onFloatRealTimeUpdate( " + p_float + " ) " );
	}

	public void onFloatRealTimeComplete(){
		Debug.Log( TimeHelper.GetFrameAndTime() + " onFloatRealTimeComplete() " );
	}

	public void onFloat1stUpdate( float p_float ){
		Debug.Log( "onFloat1stUpdate( " + p_float + " ) " + TimeHelper.GetFrameAndTime() );
	}

	public void onFloat1stComplete(){
		Debug.Log( "onFloat1stComplete() " + TimeHelper.GetFrameAndTime() );
	}

	public void onFloat2ndUpdate( float p_float ){
		Debug.Log( "onFloat2ndUpdate( " + p_float + " ) " + TimeHelper.GetFrameAndTime() );
	}

	public void onFloat2ndComplete(){
		Debug.Log( "onFloat2ndComplete() " + TimeHelper.GetFrameAndTime() );
	}

	public bool m_log_button_status = false;
	
	public bool m_execute = false;
	
	public bool m_button_status = false;

	public bool m_debug_log = false;

	[Range(0, 1)]
	public float m_anim_normalized_time = 0.0f;

	public Vector3 m_delta_move = new Vector3( 0.001f, 0, 0 );

	void Update () {
		ModelAutoActivator.Instance().ManualUpdate();

//		UILabel t_label = GetComponent<UILabel>();
//
//		if( t_label == null ){
//			return;
//		}
//
//		NGUIHelper.GetTextWidth( t_label, t_label.text );

//		if( m_tex != null ){
//			Debug.Log( "Update UITexture: " + m_tex );
//
//			SetUITexture( m_tex );
//
//			m_tex = null;
//		}
//
//		if( m_self_copy_tex != null ){
//			Debug.Log( "Update Self Copy UITexture: " + m_tex );
//
//			Texture2D t_tex = null;
//
//			{
//				
//			}
//
//			SetUITexture( m_self_copy_tex );
//
//			m_self_copy_tex = null;
//		}

//		{
//			Debug.Log( "Server Time: " + TimeHelper.GetCurServerDateTimeString() + "   IsFirstDay: " + LoadingTemplate.IsFirstDay() );
//		}

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
