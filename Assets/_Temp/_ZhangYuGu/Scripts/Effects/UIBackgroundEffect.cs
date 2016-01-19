//#define DEBUG_UI_BG_EF

using UnityEngine;
using System.Collections;

public class UIBackgroundEffect : MonoBehaviour {

	#region Mono

	private Shader m_shader = null;

	private Material m_mat = null;

	private float m_rt_size_p = 0.25f;

	private float m_coef = 1.0f;

	private float m_iter_offset = 0.6f;

	private bool m_is_supported 	= false;

	
	private float m_default_delay = 0.0f;

	private float m_start_time = 0;

	void Awake(){
		#if DEBUG_UI_BG_EF
		Debug.Log( "UIBackgroundEffect.Awake()" );
		#endif

		InitEffect();
	}

	// Use this for initialization
	void Start () {
		m_start_time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnRenderImage( RenderTexture p_src, RenderTexture p_dest ) {
		bool t_enable = true;

		if( !m_is_supported ){
			t_enable = false;
		}

		if( !t_enable ){
			Graphics.Blit ( p_src, p_dest );
		}
		else{
			RenderTexture t_rt = RenderTexture.GetTemporary( (int)( p_src.width * m_rt_size_p ), (int)( p_src.height * m_rt_size_p ), 0 );
			
			{
				m_mat.SetVector( "_offset_xy", new Vector4( 0, GetCoef(), 0, 0 ) );
				
				Graphics.Blit( p_src, t_rt, m_mat );
			}
			
			RenderTexture t_rt_temp = RenderTexture.GetTemporary( (int)( p_src.width * m_rt_size_p ), (int)( p_src.height * m_rt_size_p ), 0 );
			
			{
				m_mat.SetVector( "_offset_xy", new Vector4( GetCoef(), 0, 0, 0 ) );
				
				Graphics.Blit( t_rt, t_rt_temp, m_mat );
			}
			
			Graphics.Blit( t_rt_temp, p_dest, m_mat );
			
			RenderTexture.ReleaseTemporary( t_rt );
			
			RenderTexture.ReleaseTemporary( t_rt_temp );
		}
	}

	#endregion



	#region Init

	private void InitEffect(){
		#if DEBUG_UI_BG_EF
		Debug.Log( "UIBackgroundEffect.InitEffect()" );
		#endif

		m_shader = Shader.Find( "Custom/Effects/UIBackground" );

		if( m_shader == null || !m_shader.isSupported ){
			m_is_supported = false;

			if( m_shader == null ){
				Debug.Log( "Shader = null." );
			}

			if( !m_shader.isSupported ){
				Debug.Log( "Shader not supported." );
			}
		}
		else{
			#if DEBUG_UI_BG_EF
			Debug.Log( "UIBackgroundEffect.( new mat )" );
			#endif

			m_mat = new Material( m_shader );

			m_is_supported = true;
		}

//		ComponentHelper.HideComponent( this );
	}

	public void SetRtSize( float p_rt_size ){
		m_rt_size_p = p_rt_size;
	}

	#endregion



	#region Utilities

	private float GetCoef(){
		if( m_default_delay <= 0 ){
			return ( m_coef + m_iter_offset );
		}
		else{
			return ( m_coef + m_iter_offset ) * Mathf.Clamp01( ( Time.time - m_start_time ) / m_default_delay );
		}

	}

	#endregion
}
