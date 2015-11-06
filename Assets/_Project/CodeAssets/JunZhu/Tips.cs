using UnityEngine;
using System.Collections;

public class Tips : MonoBehaviour {

	private bool m_pressState;

	public GameObject m_tipsObject;

	public enum DataType
	{
		attack = 0,
		defense,
		life,
		tongshuai,
		wuli,
		zhimou
	};

	public DataType m_type;

	void OnPress(bool tempPress) //按住0.5f后 显示tips
	{
		m_pressState = tempPress;

		if(m_pressState)
		{
			StartCoroutine(ShowTips());
		}else
		{
			m_tipsObject.SetActive(false);
		}
	}

	IEnumerator ShowTips()
	{
		yield return new WaitForSeconds(0.0f);

		if(m_pressState)
		{
			m_tipsObject.SetActive(true);
			if(m_tipsObject.GetComponent<TipsData>())
			m_tipsObject.GetComponent<TipsData>().ShowData((int)m_type);
		}
	}
}
