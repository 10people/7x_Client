using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameActivityManager : MonoBehaviour {

	public GameObject m_everyDay;

	public GameObject m_countDay;
	public bool isGuideMove = false;

	public List<EventHandler> m_eventHanderList = new List<EventHandler>();

	public EventHandler m_EhLQ;

	void Start()
	{
		m_EhLQ.m_handler += LingQu;
		foreach(EventHandler tempHand in m_eventHanderList)
		{
			tempHand.m_handler += ClickEvent;
		}
	}

	void ButtonState(GameObject tempObject)
	{
		foreach(EventHandler tempHand in m_eventHanderList)
		{
			tempHand.transform.FindChild("Background").GetComponentInChildren<UISprite>().spriteName = "backGround2_GameActibity";
			tempHand.transform.FindChild("Background").GetComponentInChildren<UISprite>().SetDimensions(167,66);
		}
		tempObject.transform.FindChild("Background").GetComponentInChildren<UISprite>().spriteName = "backGround1_GameActibity";
		tempObject.transform.FindChild("Background").GetComponentInChildren<UISprite>().SetDimensions(177,76);
	}

	void ClickEvent(GameObject tempOjbect)
	{
		if(tempOjbect.name == "Button_1")
		{
			m_everyDay.SetActive(true);
			m_countDay.SetActive(false);
			
		}
		else if(tempOjbect.name == "Button_2")
		{
			m_countDay.SetActive(true);
			m_everyDay.SetActive(false);
		}
		else 
		{
			m_countDay.SetActive(false);
			m_everyDay.SetActive(false);

		}
		ButtonState(tempOjbect);
	}

	void LingQu(GameObject tempOjbect)
	{
	   isGuideMove = true;
	}
}
