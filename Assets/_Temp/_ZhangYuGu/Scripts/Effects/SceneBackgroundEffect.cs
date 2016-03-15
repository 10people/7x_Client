//#define DEBUG_UI_BG_EF



using UnityEngine;
using System.Collections;



public class SceneBackgroundEffect : MonoBehaviour {

	#region Mono

	public Color m_light_color = Color.black;

	public Color m_dark_color = Color.black;

	private Shader m_shader = null;

	private Material m_mat = null;

	private bool m_is_supported = false;
	
	void Awake(){
		#if DEBUG_UI_BG_EF
		Debug.Log( "SceneBackgroundEffect.Awake()" );
		#endif

		InitEffect();
	}

	// Use this for initialization
	void Start () {
	
	}

	void OnEnable(){
		
	}

	void OnDisable(){
		
	}
	
	// Update is called once per frame
	void Update () {
		if( !m_is_supported ){
			return;
		}
		
		m_mat.SetColor( "_LightColor", m_light_color );

		m_mat.SetColor( "_DarkColor", m_dark_color );
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

			return;
		}

		{
			Graphics.Blit( p_src, p_dest, m_mat );
		}
	}

	#endregion



	#region Init

	private void InitEffect(){
		#if DEBUG_UI_BG_EF
		Debug.Log( "SceneBackgroundEffect.InitEffect()" );
		#endif

		m_shader = Shader.Find( "Custom/Effects/SceneBackground" );

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
			Debug.Log( "SceneBackgroundEffect.( new mat )" );
			#endif

			m_mat = new Material( m_shader );

			m_is_supported = true;
		}

//		ComponentHelper.HideComponent( this );
	}

	#endregion



	#region Camera Ops

	#endregion



	#region Use

	#endregion



	#region Utilities

	#endregion
}
