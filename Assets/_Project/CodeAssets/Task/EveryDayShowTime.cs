using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EveryDayShowTime : MYNGUIPanel{
	// Use this for initialization
	public List<GameObject> m_listObjPage;
	public static bool m_isLoad0 = false;
	public static bool m_isLoad1 = false;
	public static bool m_isLoad2 = false;
	private int m_iPageIndex = 0;
	public MyPageManager m_myPageManager;
	void Start () 
	{
		m_isLoad0 = false;
		m_isLoad1 = false;
		m_isLoad2 = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_isLoad0 && m_isLoad1 && m_isLoad2)
		{
			if(!m_listObjPage[3].activeSelf)
			{
				m_listObjPage[0].SetActive(true);
				m_listObjPage[3].SetActive(true);
			}
		}
	}

	public override void MYClick(GameObject ui)
	{
//		m_iPageIndex = m_iWantPageIndex;
//		m_PageObj[m_iPageIndex].SetActive(true);
//		for(int i = 0; i < m_pageButton.Count; i ++)
//		{
//			if(m_isOpenFunction[i])
//			{
//				if(i == m_iPageIndex)
//				{
//					m_iSelecteEndY = 128 - 74 * i;
//					m_pageButton[i].spriteName = "pagebutton" + i +"_1";
//					//					m_pageButton[i].color = Color.white;
//				}
//				else
//				{
//					m_pageButton[i].spriteName = "pagebutton" + i +"_0";
//					//					m_pageButton[i].color = Color.grey;
//				}
//			}
//		}

		if(ui.name.IndexOf("Page") != -1)
		{
			int temp = int.Parse(ui.name.Substring(4, 1));
			if(temp != m_iPageIndex)
			{
				m_listObjPage[m_iPageIndex].gameObject.SetActive(false);
			}
			m_iPageIndex = temp;
			m_listObjPage[m_iPageIndex].gameObject.SetActive(true);

			m_myPageManager.setPageIndex(m_iPageIndex);
		}
		if(ui.name.IndexOf("Close") != -1)
		{
			MainCityUI.TryRemoveFromObjectList(gameObject);
			GameObject.Destroy(gameObject);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}
