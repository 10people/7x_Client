using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITextManager
{
	private static GameObject m_ObjText;
	private List<string> m_listData = new List<string>();
	public List<UIText> m_listUIText = new List<UIText>();

	public void createText(string data)
	{
		m_listData.Add(data);
		if(m_ObjText == null)
		{
			Global.ResourcesDotLoad("New/Text", CachedTextCallback);
		}
		else
		{
			initText();
		}
	}

	private void CachedTextCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		m_ObjText = p_object as GameObject;
		initText();
	}

	private void initText()
	{
		UIText tempText = GameObject.Instantiate(m_ObjText).GetComponent<UIText>();
		tempText.m_UILabel.text = m_listData[0];
		m_listData.RemoveAt(0);
		m_listUIText.Add(tempText);
		for(int i = 0; i < m_listUIText.Count ; i ++)
		{
			m_listUIText[i].gameObject.transform.position = new Vector3(i * 10, 0, 0);
			if(m_listUIText[i].m_panelMove.transform.localPosition.y < ((m_listUIText.Count - 1) - i)  * 40 + 80)
			{
				m_listUIText[i].m_panelMove.transform.localPosition = new Vector3(0,((m_listUIText.Count - 1) - i) * 40 + 80, 0);
			}
			setPosC(m_listUIText[i]);
		}
	}

	// Update is called once per frame
	public void Update ()
	{
		for(int i = 0; i < m_listUIText.Count ; i ++)
		{
			if(m_listUIText[i] == null)
			{
				m_listUIText.RemoveAt(0);
				i--;
				continue;
			}
			if(m_listUIText[i].m_iNum > 40)
			{
				m_listUIText[i].m_panelMove.transform.localPosition = new Vector3(0,m_listUIText[i].m_panelMove.transform.localPosition.y + 10, 0);
				setPosC(m_listUIText[i]);
			}
			else
			{
				m_listUIText[i].m_iNum++;
			}
			if(m_listUIText[i].m_panelMove.transform.localPosition.y >= 320)
			{
				GameObject.Destroy(m_listUIText[i].gameObject);
				m_listUIText.RemoveAt(0);
				i--;
			}
		}
	}

	public void setPosC(UIText uiText)
	{
		float tempF = 1 - ((uiText.m_panelMove.transform.localPosition.y - 80)/ 240f);
		uiText.m_UIPanel.alpha = tempF;
		tempF += 0.5f;
		if(tempF > 1)
		{
			tempF = 1f;
		}
		uiText.m_panelMove.transform.localScale = new Vector3(tempF, tempF, tempF);
	}
}
