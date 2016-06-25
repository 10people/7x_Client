//#define DEBUG_EFFECT

//#define DEBUG_EFFECT_DETAIL



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.5.26
 * @since:		Unity 5.1.3
 * Function:	Boss's crush and normal effect.
 * 
 * Notes:
 * 1.None.
 */ 
public class BossEffect : MonoBehaviour {

	public Material m_normal_mat;

	public Material m_crush_mat;

	private Renderer[] m_renderers;



	#region Mono

	void Start () {
		Init();
	}

	#endregion



	#region Init

	private List<BossEffect> m_boss_effects = new List<BossEffect>();

	private void Init(){
		if( m_normal_mat == null ){
			Debug.LogError( "Error, normal mat is null." );
		}

		if( m_crush_mat == null ){
			Debug.LogError( "Error, crush mat is null." );
		}

		{
			m_renderers = gameObject.GetComponentsInChildren<Renderer>();
		}

		{
			m_boss_effects.Clear();

			BossEffect[] t_effect = gameObject.GetComponentsInChildren<BossEffect>();

			m_boss_effects.AddRange( t_effect );
		}
	}

	#endregion



	#region Use

	public void UseNormalMat(){
		#if DEBUG_EFFECT
		Debug.Log( "UseNormalMat() " + Time.realtimeSinceStartup );
		#endif

		if( m_normal_mat == null ){
			Debug.LogError( "Error, normal mat is null." );
		}

		if( m_crush_mat == null ){
			Debug.LogError( "Error, crush mat is null." );
		}

		{
			UpdateMat( m_normal_mat );

			LeanTween.cancel( gameObject );
		}
	}

	private float m_crush_time = 0.0f;

	private List<Renderer> m_target_renderers = new List<Renderer>();

	private const float COSNT_LAUNCH_TIME		= 1.5f;

	public void UseCrushMat( float p_time ){
		#if DEBUG_EFFECT
		Debug.Log( "UseCrushMat( " + p_time + " ) " + Time.realtimeSinceStartup );
		#endif

		if( m_normal_mat == null ){
			Debug.LogError( "Error, normal mat is null." );
		}

		if( m_crush_mat == null ){
			Debug.LogError( "Error, crush mat is null." );
		}

		{
			m_crush_time = p_time;

			Material t_target_mat = new Material( m_normal_mat );

			t_target_mat.name = t_target_mat.name + " - Auto";
		
			UpdateMat( t_target_mat );

			LeanTween.value( gameObject, 0.0f, 1.0f, COSNT_LAUNCH_TIME ).
				setOnUpdate( UpdateTarget ).
			setEase(LeanTweenType.easeInOutBack ).
				setIgnoreTimeScale( false );

			LeanTween.value( gameObject, 1.0f, 0.0f, p_time ).
				setDelay( COSNT_LAUNCH_TIME ).
				setOnUpdate( UpdateTarget ).
				setEase(LeanTweenType.easeInCirc ).
				setIgnoreTimeScale( false );
		}
	}

	public void UpdateTarget( float p_p ){
		#if DEBUG_EFFECT_DETAIL
		Debug.Log( "------- UpdateTarget( " + p_p + " ) -------" );

		Debug.Log( "Mat: " + m_target_renderer + "" );
		#endif

		{
			Color t_color = m_crush_mat.GetColor( "_MainColor" ) * p_p + m_normal_mat.GetColor( "_MainColor" ) * ( 1 - p_p ) ;

			t_color.r = Mathf.Clamp( t_color.r, 0.0f, 1.0f );

			t_color.g = Mathf.Clamp( t_color.g, 0.0f, 1.0f );

			t_color.b = Mathf.Clamp( t_color.b, 0.0f, 1.0f );

			t_color.a = Mathf.Clamp( t_color.a, 0.0f, 1.0f );

			MaterialHelper.UpdateColor( m_target_renderers, "_MainColor", t_color );
		}

		{
			Color t_color = m_crush_mat.GetColor( "_RimColor" ) * p_p + m_normal_mat.GetColor( "_RimColor" ) * ( 1 - p_p ) ;

			t_color.r = Mathf.Clamp( t_color.r, 0.0f, 1.0f );

			t_color.g = Mathf.Clamp( t_color.g, 0.0f, 1.0f );

			t_color.b = Mathf.Clamp( t_color.b, 0.0f, 1.0f );

			t_color.a = Mathf.Clamp( t_color.a, 0.0f, 1.0f );
				
			MaterialHelper.UpdateColor( m_target_renderers, "_RimColor", t_color );
		}

		{
			float t_v = m_crush_mat.GetFloat( "_RimWeight" ) * p_p + m_normal_mat.GetFloat( "_RimWeight" ) * ( 1 - p_p );

			MaterialHelper.UpdateFloat( m_target_renderers, "_RimWeight", Mathf.Clamp( t_v, 0.0f, 1.0f ) );
		}

		{
			float t_v = m_crush_mat.GetFloat( "_RimWidth" ) * p_p + m_normal_mat.GetFloat( "_RimWidth" ) * ( 1 - p_p );

			MaterialHelper.UpdateFloat( m_target_renderers, "_RimWidth", Mathf.Clamp( t_v, 1.0f, 8.0f ) );
		}
	}

	#endregion



	#region Utilities

	private void UpdateMat( Material p_mat ){
		if( m_renderers == null ){
			return;
		}

		m_target_renderers.Clear();

		Renderer t_found = null;

		for( int i = 0; i < m_renderers.Length; i++ ){
			Renderer t_render = m_renderers[ i ];

			if( t_render == null ){
				continue;
			}

			Material t_mat = t_render.material;

			if( t_mat == null ){
				continue;
			}

			if( t_mat.mainTexture == p_mat.mainTexture ){
				#if DEBUG_EFFECT
				Debug.Log( "Use new mat at: " + GameObjectHelper.GetGameObjectHierarchy( t_render.gameObject  ) );

				Debug.Log( "Rednerer: " + t_render );

				Debug.Log( "New mat: " + p_mat );
				#endif

				t_render.material = p_mat;

				t_found = t_render;

				m_target_renderers.Add( t_render );
			}
		}

		if( t_found == null ){
			#if DEBUG_EFFECT
			Debug.LogError( "Target not found: " + p_mat.mainTexture );

			ComponentHelper.LogTexture( p_mat.mainTexture );
			#endif
		}
	}

	#endregion
}
