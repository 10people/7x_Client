using UnityEngine;
using System.Collections;

public class HittedEffect : MonoBehaviour {

	#region Mono

	private Renderer[] m_renderers;

	private float m_c = 0.0f;

	private const float BLINK_DUR	= 0.25f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		UpdateMat();
	}

	#endregion



	#region Hitted

	public void Init(){
		m_renderers = gameObject.GetComponentsInChildren<Renderer>();

		if( m_renderers.Length == 0 ){
			return;
		}

		iTween.ValueTo( gameObject, iTween.Hash( 
		                                              "from", 0,
		                                              "to", 0.5f,
		                                              "time", BLINK_DUR / 2,
		                                        		"easetype", iTween.EaseType.easeOutQuart,
		                                              "onupdate", "OnUpdate" ) );

		iTween.ValueTo( gameObject, iTween.Hash( 
		                                              "from", 1,
		                                              "to", 0.5f,
		                                              "delay", BLINK_DUR /2,
		                                              "time", BLINK_DUR / 2,
		                                        		"easetype", iTween.EaseType.easeOutQuart,
		                                              "onupdate", "OnUpdate",
		                                              "oncomplete", "Done" ) );
	}

	private void UpdateMat(){
		if( m_renderers == null ){
			return;
		}
		
		Color t_c = new Color( m_c, m_c, m_c, m_c );
		
		for( int i = 0; i < m_renderers.Length; i++ ){
			Renderer t_render = m_renderers[ i ];

			if( t_render == null ){
				continue;
			}

			for( int j = 0; j < t_render.materials.Length; j++ ){
				Material t_mat = t_render.materials[ j ];
				
				t_mat.SetColor( "_FxColor", t_c );
			}
		}
	}

	public void OnUpdate( float p_value ){
		m_c = p_value;
	}

	public void Done(){
		m_c = 0;
		
		UpdateMat();

		m_renderers = null;
	}

	#endregion
}
