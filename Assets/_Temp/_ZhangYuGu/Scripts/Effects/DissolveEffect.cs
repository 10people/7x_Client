using UnityEngine;
using System.Collections;

public class DissolveEffect : MonoBehaviour {

	#region Mono

	public Material m_mat = null;

	private float m_c = 0.0f;

	public const float DISSOLVE_DUR	= 2.0f;

	private Shader m_origin_shader = null;

	private Texture m_origin_tex = null;

	// Use this for initialization
	void Start () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMat();
	}

	#endregion



	#region Set

	public void Restore(){
		m_mat.shader = m_origin_shader;
	}

	public void Init(){
		if( m_mat == null ){
			Debug.LogError( "Mat = null, return." );

			return;
		}

		{
			m_origin_shader = m_mat.shader;

			m_origin_tex = m_mat.mainTexture;
		}

		{
			Shader t_s = Shader.Find( "Custom/Effects/Dissolve Colored" );

			if( t_s != null ){
				m_mat.shader = t_s;
			}
			else{
				Debug.LogError( "Error, shader = null." );
			}
		}

		{
			m_mat.mainTexture = m_origin_tex;
		}

		iTween.ValueTo( gameObject, iTween.Hash( 
		                                              	"from", 0,
		                                              	"to", 1f,
		                                        		"time", DISSOLVE_DUR,
		                                        		"easetype", iTween.EaseType.linear,
		                                             	"onupdate", "OnUpdate",
		                                              	"oncomplete", "Done" ) );
	}

	private void UpdateMat(){
		if( m_mat == null ){
			return;
		}

		m_mat.SetFloat( "_Dissolve", m_c );
	}

	public void OnUpdate( float p_value ){
		m_c = p_value;
	}

	public void Done(){
		m_c = 0;
		
		UpdateMat();

		m_mat = null;
	}

	#endregion
}
