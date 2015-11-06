using UnityEngine;
using System.Collections;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.6.25
 * @since:		Unity 4.5.3
 * Function:	Make SkyBox Flow.
 * 
 * Notes:
 * Use this to Update Shader: "Custom/Effects/Flow SkyBox".
 */
public class FlowSkyBox : MonoBehaviour {

	#region Mono

	// speed
	public float m_vps = 3.0f;

	private Material m_flow_sky_box = null;

	private Shader m_flow_sky_box_sh = null;

	void Start () {
		m_flow_sky_box = RenderSettings.skybox;

		if( m_flow_sky_box != null ){
			m_flow_sky_box_sh = Shader.Find( "Custom/Effects/Flow SkyBox" );

			if( m_flow_sky_box_sh != null ){
				m_flow_sky_box.shader = m_flow_sky_box_sh;
			}
		}
	}

	void Update () {
		UpdateSkyBox();
	}

	#endregion



	#region Utilities

	// Update Mat's Shader.
	private void UpdateSkyBox(){
		if( m_flow_sky_box_sh == null ){
			return;
		}

		if( m_flow_sky_box == null ){
			return;
		}

		m_flow_sky_box.SetFloat( "_Coef", m_flow_sky_box.GetFloat( "_Coef" ) + Time.deltaTime * m_vps );
	}

	#endregion
}
