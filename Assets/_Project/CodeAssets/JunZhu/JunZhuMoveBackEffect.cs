using UnityEngine;
using System.Collections;

public class JunZhuMoveBackEffect : MonoBehaviour 
{
	public GameObject m_ZhuangBeiInfo;
	public GameObject m_TweenLayer;
	public bool m_IsOn = false;
	
	void OnClick()
	{
		if(!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
		{
			PlayerPrefs.DeleteKey("JunZhu");
		}

		m_TweenLayer.GetComponent<TweenPosition> ().from = Vector3.zero;
	//	m_TweenLayer.GetComponent<TweenPosition> ().to = m_ZhuangBeiInfo.GetComponent<JunZhuZhuangBeiInfo> ().m_PositionSend;
	//	m_TweenLayer.GetComponent<TweenPosition> ().duration = 20.0f;
		m_TweenLayer.GetComponent<TweenPosition> ().enabled = true;
//
		m_TweenLayer.GetComponent<TweenScale> ().from = new Vector3 (1, 1, 1);
		m_TweenLayer.GetComponent<TweenScale> ().to = new Vector3 (0.1f, 0.1f, 0.1f);
	//	m_TweenLayer.GetComponent<TweenScale> ().duration = 30.0f;

		m_TweenLayer.GetComponent<TweenScale> ().enabled = true;

		StartCoroutine (WaitFor());
 		
	}

     IEnumerator WaitFor()
	{ 
		//if (m_TweenLayer.GetComponent<TweenScale> ().from.x == 1)
		
//		
//		{
//			m_ZhuangBeiInfo.SetActive(false);
//
//			m_TweenLayer.transform.localPosition = Vector3.zero;
//			m_TweenLayer.transform.localScale = Vector3.one;
//		
//		}
		yield return new WaitForSeconds (0.1f);
	 	m_ZhuangBeiInfo.SetActive(false);
	}
}
