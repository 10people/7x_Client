


using UnityEngine;
using System.Collections;



public class RuntimeGuideChecker : MonoBehaviour {

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateGuide();
	}

	#endregion



	#region Guide

	public int m_guide_id = 0;

	public bool m_open_guide = false;

	public bool m_close_guide = false;

	public bool m_log_guide = false;

	private void UpdateGuide(){
		if( m_open_guide ){
			m_open_guide = false;

			UIYindao.m_UIYindao.setOpenYindao( m_guide_id );

			return;
		}

		if( m_close_guide ){
			m_close_guide = false;

			UIYindao.m_UIYindao.CloseUI();

			return;
		}

		if( m_log_guide ){
			m_log_guide = false;

			Debug.Log( "Guide.State: " +  UIYindao.m_UIYindao.m_isOpenYindao + " - " + UIYindao.m_UIYindao.m_iCurId + "   - " +
				GuideInfoTemplate.GetWindowId_By_GuideId( UIYindao.m_UIYindao.m_iCurId ) );
		}
	}

	#endregion



	#region Utilities



	#endregion

}
