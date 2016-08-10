using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyPageManager : MonoBehaviour 
{
	public List<MyPageButtonManager> m_listMyPageButtonManager;
	public int m_iBY;
	public GameObject m_spriteSelect;
	private int m_iPageIndex;
	private int m_iSelectEndY = -99999;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_spriteSelect.transform.position.y != m_iSelectEndY && m_iSelectEndY != -99999)
		{
			float tempY = 0;
			if(Math.Abs((m_iSelectEndY - m_spriteSelect.transform.localPosition.y)) < 1)
			{
				tempY = m_iSelectEndY;
			}
			else
			{
				tempY = m_spriteSelect.transform.localPosition.y + (m_iSelectEndY - m_spriteSelect.transform.localPosition.y) / 2;
			}
			m_spriteSelect.transform.localPosition = new Vector3(m_spriteSelect.transform.localPosition.x, tempY, m_spriteSelect.transform.localPosition.z);
		}	
	}

	public void setPageIndex(int index)
	{
		m_iPageIndex = index;

		setAnimation(m_iPageIndex);
		for(int i = 0; i < m_listMyPageButtonManager.Count; i ++)
		{
			if(i == m_iPageIndex)
			{
				m_listMyPageButtonManager[i].m_spritePageButton.spriteName = "pagebutton_" + i +"_1";
				//					m_pageButton[i].color = Color.white;
			}
			else
			{
				m_listMyPageButtonManager[i].m_spritePageButton.spriteName = "pagebutton_" + i +"_0";
				//					m_pageButton[i].color = Color.grey;
			}
		}
	}

	public void setAnimation(int index)
	{
		m_iSelectEndY = 128 - index * 74;
	}
}
