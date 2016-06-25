//#define DEBUG_HITTED_EFFECT

//#define DEBUG_BOSS_EFFECT

//#define USE_ITWEEN

//#define USE_NEW_EFFECT



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HittedEffect : MonoBehaviour {

	#region Mono

	public bool m_is_debug = false;

	public Renderer[] m_renderers;

	private const float DELAY_MAX	= 0.15f;

	private const float BLINK_DUR	= 0.25f;



	// Use this for initialization
	void Start () {
		m_target_sh = Shader.Find( "Custom/Characters/Main Texture Hight Light Rim" );

		{
			InitOriginParams();
		}

		if( m_is_debug ){
//			InitAnim();

			UnityEngine.Object t_ob = Resources.Load( GetHittedAnimPath( IsSoldierHitted() ) );

			WWW t_www = null;

			AnimLoadedCallback( ref t_www, GetHittedAnimPath( IsSoldierHitted() ), t_ob );
		}
		else{
			if( GetHittedAnimObject( GetHittedAnimPath( IsSoldierHitted() ) ) == null ){
				Global.ResourcesDotLoad( GetHittedAnimPath( IsSoldierHitted() ), AnimLoadedCallback );
			}
			else{
				WWW t_www = null;

				AnimLoadedCallback( ref t_www, "", GetHittedAnimObject( GetHittedAnimPath( IsSoldierHitted() ) ) );
			}
		}
	}

	private static string GetHittedAnimPath( bool p_is_soldier_hitted ){
		string t_res_path = "";
		#if USE_NEW_EFFECT
		t_res_path = "_3D/Anims/BattleField/HittedAnim";
		#else
		if( p_is_soldier_hitted ){
			t_res_path = "_3D/Anims/BattleField/OriginHittedAnim";
		}
		else{
			t_res_path = "_3D/Anims/BattleField/OriginHittedAnim_Boss";
		}

		#endif

		return t_res_path;
	}

	private void AnimLoadedCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "AnimLoadedCallback.path: " + p_path );

		Debug.Log( "AnimLoadedCallback.ob: " + p_object );
		#endif

		Animation t_anim = (Animation)ComponentHelper.AddIfNotExist( gameObject, typeof(Animation) );

		t_anim.AddClip( (AnimationClip)p_object, GetHittedAnimPath( IsSoldierHitted() ) );

		t_anim.clip = (AnimationClip)p_object;

		{
			InitAnim();
		}
	}

	// Update is called once per frame
	void Update () {
		if( m_playing_anim ){
			UpdateMat();
		}
	}

	#endregion



	#region Anim

	public bool m_playing_anim = false;

	#if USE_NEW_EFFECT
//	public Color m_main_color = Color.white;

	public Color m_rim_color = Color.white;

	public float m_anim_width = 8.0f;

	public float m_anim_weight = 1.0f;
	#else
	public Color m_fx_color = Color.black;
	#endif

	public void HittedAnimDone(){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "HittedAnimDone()" );
		#endif

		m_playing_anim = false;

		Done();
	}

	#endregion



	#region Hitted

	private Shader m_target_sh = null;

	private bool m_is_soldier_hitted = true;

	public void SetIsSoldierHitted( bool p_is_soldier_hitted ){
		m_is_soldier_hitted = p_is_soldier_hitted;
	}

	private bool IsSoldierHitted(){
		return m_is_soldier_hitted;
	}

	public void InitAnim(){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "InitAnim()" );
		#endif

		{
			m_playing_anim = true;

			Animation t_anim = GetComponent<Animation>();

			if( t_anim != null ){
				t_anim.Play();
			}
			else{
				#if UNITY_EDITOR
				Debug.Log( "Anim not exist." );
				#endif
			}
		}

//		if( m_renderers.Length == 0 ){
//			return;
//		}

//		float t_delay = Random.value * DELAY_MAX;

//		#if USE_ITWEEN
//		iTween.ValueTo( gameObject, iTween.Hash( 
//		                                              "from", 0,
//		                                              "to", 0.5f,
//														"delay", t_delay,
//		                                              "time", BLINK_DUR / 2,
//		                                        		"easetype", iTween.EaseType.easeOutQuart,
//		                                              "onupdate", "OnUpdate" ) );
//
//		iTween.ValueTo( gameObject, iTween.Hash( 
//		                                              "from", 1,
//		                                              "to", 0.5f,
//														"delay", BLINK_DUR /2 + t_delay,
//		                                              "time", BLINK_DUR / 2,
//		                                        		"easetype", iTween.EaseType.easeOutQuart,
//		                                              "onupdate", "OnUpdate",
//		                                              "oncomplete", "Done" ) );
//		#else
//		LeanTween.value( gameObject, 0f, 1.0f, BLINK_DUR / 2 )
//			.setDelay( t_delay )
//			.setEase( LeanTweenType.easeOutQuart )
//			.setOnUpdate( OnUpdate );
//
//		LeanTween.value( gameObject, 1.0f, 0f, BLINK_DUR / 2 )
//			.setDelay( BLINK_DUR /2 + t_delay )
//			.setEase( LeanTweenType.easeOutQuart )
//			.setOnUpdate( OnUpdate )
//			.setOnComplete( Done );
//		#endif

//		{
//			UpdateColor( 1.0f );
//
//			m_cached_width = 1.0f;
//
//			m_cached_weight = 0;
//		}

//		UpdateMat();
	}

	private void UpdateMat(){
		if( m_renderers == null ){
			return;
		}

		for( int i = 0; i < m_renderers.Length; i++ ){
			Renderer t_render = m_renderers[ i ];

			if( t_render == null ){
				continue;
			}

			for( int j = 0; j < t_render.materials.Length; j++ ){
				Material t_mat = t_render.material;

//				if( t_mat.shader != m_target_sh ){
//					#if DEBUG_BOSS_EFFECT
//					Debug.Log( "sh not the same: " );
//
//					Debug.Log( "mat.sh: " + t_mat.shader );
//
//					Debug.Log( "target sh: " + m_target_sh );
//					#endif
//
//					continue;
//				}

//				if( t_mat.HasProperty( "_MainColor" ) ){
//					t_mat.SetColor( "_RimColor", m_cached_color );
//					t_mat.SetColor( "_MainColor", m_main_color );
//				}
//				else{
//					#if UNITY_EDITOR
//					Debug.Log( "No Property found: " + "_MainColor" );
//					#endif
//				}

				#if USE_NEW_EFFECT
				if( t_mat.HasProperty( "_RimColor" ) ){
//					t_mat.SetColor( "_RimColor", m_cached_color );
					t_mat.SetColor( "_RimColor", m_rim_color );
				}
				else{
					#if UNITY_EDITOR && DEBUG_HITTED_EFFECT
					Debug.Log( "No Property found: " + "_RimColor" );
					#endif
				}

				if( t_mat.HasProperty( "_RimWeight" ) ){
//					t_mat.SetFloat( "_RimWeight", m_cached_weight );
					t_mat.SetFloat( "_RimWeight", m_anim_weight );
				}
				else{
					#if UNITY_EDITOR && DEBUG_HITTED_EFFECT
					Debug.Log( "No Property found: " + "_RimWeight" );
					#endif
				}

				if( t_mat.HasProperty( "_RimWidth" ) ){
//					t_mat.SetFloat( "_RimWidth", m_cached_width );
					t_mat.SetFloat( "_RimWidth", m_anim_width );
				}
				else{
					#if UNITY_EDITOR && DEBUG_HITTED_EFFECT
					Debug.Log( "No Property found: " + "_RimWidth" );
					#endif
				}
				#else
				if( t_mat.HasProperty( "_FxColor" ) ){
//					t_mat.SetColor( "_FxColor", m_fx_color );
					t_mat.SetColor( "_FxColor", m_fx_color );
				}
				else{
					#if UNITY_EDITOR && DEBUG_HITTED_EFFECT
					//Debug.Log( "No Property found: " + "_FxColor" );
					#endif
				#endif
				}
			}
		}
	}

	private Color m_cached_color = new Color( 0.0f, 0.0f, 0.0f );

	private float m_cached_weight = 0;

	private float m_cached_width = 2.0f;

	private void UpdateColor( float p_c ){
		m_cached_color.r = p_c;

		m_cached_color.g = p_c;

		m_cached_color.b = p_c;
	}


	public void OnUpdate( float p_value ){
//		UpdateColor( p_value );

		m_cached_weight = p_value;
	}

	public void Done(){
		if( m_boss_found ){
			{
				#if DEBUG_BOSS_EFFECT
				Debug.Log( "Restore Boss Param." );
				#endif

				LogBossParam();
			}

			m_cached_color = m_boss_color;

			m_cached_width = m_boss_width;

			m_cached_weight = m_boss_weight;
		}
		else{
			UpdateColor( 0 );

			m_cached_width = 8.0f;

			m_cached_weight = 0.0f;
		}

		{
			m_fx_color = Color.black;
		}
		
		UpdateMat();

//		m_renderers = null;
	}

	#endregion



	#region Boss

	private Color m_boss_color = Color.white;

	private float m_boss_width = 0.0f;

	private float m_boss_weight = 0.0f;

	private bool m_boss_found = false;

	private void InitOriginParams(){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "InitOriginParams()" );
		#endif

		m_renderers = gameObject.GetComponentsInChildren<Renderer>();

		Material[] t_mats = ComponentHelper.GetMaterialsWithShader<SkinnedMeshRenderer>( gameObject, m_target_sh );

		if( t_mats.Length >= 1 ){
			SaveBossEffect( m_target_sh );
		}

		if( t_mats.Length > 1 ){
//			#if UNITY_EDITOR
//			Debug.LogError( "More than 1 mat." );
//			#endif
		}
	}

	public void SaveBossEffect( Shader p_target_sh ){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "SaveBossEffect()" );
		#endif

		Renderer[] t_renderers = gameObject.GetComponentsInChildren<Renderer>();

		for( int i = 0; i < t_renderers.Length; i++ ){
			Renderer t_render = t_renderers[ i ];

			if( t_render == null ){
				continue;
			}

			for( int j = 0; j < t_render.materials.Length; j++ ){
				Material t_mat = t_render.materials[ j ];

				if( t_mat.shader == p_target_sh ){
					#if DEBUG_BOSS_EFFECT
					Debug.Log( "Boss sh found." );
					#endif

					{
						m_boss_found = true;

						m_boss_color = t_mat.GetColor( "_RimColor" );

						m_boss_width = t_mat.GetFloat( "_RimWidth" );

						m_boss_weight = t_mat.GetFloat( "_RimWeight" );
					}

					{
						LogBossParam();
					}

					return;
				}
			}
		}
	}

	private void LogBossParam(){
		#if DEBUG_BOSS_EFFECT
		Debug.Log( "Boss Color: " + m_boss_color );

		Debug.Log( "Boss Width: " + m_boss_width );

		Debug.Log( "Boss Weight: " + m_boss_weight );
		#endif
	}

	#endregion



	#region Utilities

	private static UnityEngine.Object GetHittedAnimObject( string p_anim_name ){
		return ObjectHelper.GetLoadedObject( p_anim_name );
	}

	public static void PreloadAnims(){
		ObjectHelper.LoadObject( GetHittedAnimPath( true ) );

		ObjectHelper.LoadObject( GetHittedAnimPath( false ) );
	}

	#endregion
}
