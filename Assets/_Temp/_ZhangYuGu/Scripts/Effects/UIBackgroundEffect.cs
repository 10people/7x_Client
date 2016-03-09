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

	private RenderTexture m_cached_rt = null;

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

	void OnEnable(){
		
	}

	void OnDisable(){
		RevertCam();

		if( m_cached_rt != null ){
			RenderTexture.ReleaseTemporary( m_cached_rt );

			m_cached_rt = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		
	}

	void OnRenderImage( RenderTexture p_src, RenderTexture p_dest ) {
		bool t_enable = true;

		if( !m_is_supported ){
			t_enable = false;
		}

		if( !t_enable ){
			Graphics.Blit ( p_src, p_dest );
		}

		if( m_cached_rt != null ){
			Graphics.Blit ( m_cached_rt, p_dest );

			return;
		}

		{
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

			if( m_cached_rt == null ){
				m_cached_rt = RenderTexture.GetTemporary( t_rt_temp.width, t_rt_temp.height, 0 );

				Graphics.Blit( t_rt_temp, m_cached_rt );

				InitCam();
			}

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



	#region Camera Ops

	private int m_cam_mask = 0;

	private CameraClearFlags m_cam_flag;

	private void InitCam(){
		Camera t_cam = GetComponent<Camera>();

		if( t_cam == null ){
			return;
		}

		#if DEBUG_UI_BG_EF
		Debug.Log( "UIBackground.InitCam( " + GameObjectHelper.GetGameObjectHierarchy( gameObject ) + " )" );
		#endif

		{
			m_cam_flag = t_cam.clearFlags;

			m_cam_mask = t_cam.cullingMask;
		}

		{
			t_cam.clearFlags = CameraClearFlags.Depth;

			t_cam.cullingMask = 0;
		}
	}

	private void RevertCam(){
		Camera t_cam = GetComponent<Camera>();

		if( t_cam == null ){
			return;
		}

		{
			t_cam.clearFlags = m_cam_flag;

			t_cam.cullingMask = m_cam_mask;
		}
	}

	#endregion



	#region Use

	public static UIBackgroundEffect SetUIBackgroundEffect( GameObject p_gb, bool p_open ){
		if( p_gb == null ){
			Debug.Log( "gameobjet is null: " + p_gb );

			return null;
		}

		Camera t_cam = p_gb.GetComponent<Camera>();

		if( t_cam == null ){
			Debug.Log( "No Camera found in gb: " + p_gb );

			return null;
		}

		if( p_open ){
			UIBackgroundEffect t_effect = (UIBackgroundEffect)ComponentHelper.AddIfNotExist( p_gb, typeof(UIBackgroundEffect) );

			return t_effect;
		}
		else{
			ComponentHelper.RemoveIfExist( p_gb, typeof(UIBackgroundEffect) );

			return null;
		}
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
