using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainCityListButtonManager
{
	public GameObject m_objThis;
	public int m_iBX;
	public int m_iBY;
	public int m_iTX;
	public int m_iTY;
	public GameObject m_Obj;
	public int m_iState;
	public delegate void AnimEnd();
	public AnimEnd m_DelegateAnimEnd;
	public bool m_isAnim = false;
	public List<FunctionButtonManager> m_listFunctionButtonManager = new List<FunctionButtonManager>();

	public MainCityListButtonManager(GameObject parent, string name, int state)
	{
		m_objThis = new GameObject();
		m_objThis.name = name;
		m_objThis.transform.parent = parent.transform;
		m_objThis.transform.localPosition = Vector3.zero;
		m_objThis.transform.localScale = Vector3.one;
		m_iState = state;
		switch(m_iState)
		{
		case 0:
			m_iBX = 50;
			m_iBY = -100;
			m_iTX = 0;
			m_iTY = -75;
			break;
		case 1:
			m_iBX = 960 + ClientMain.m_iMoveX * 2 - 50;
			m_iBY = -100;
			m_iTX = -75;
			m_iTY = 0;
			break;
		case 2:
			m_iBX = 960 + ClientMain.m_iMoveX * 2 - 150;
			m_iBY = -(640 + ClientMain.m_iMoveY * 2 - 45);
			m_iTX = -75;
			m_iTY = 0;
			break;
		case 3:
			m_iBX = 960 + ClientMain.m_iMoveX * 2 - 50;
			m_iBY = -(640 + ClientMain.m_iMoveY * 2 - 150);
			m_iTX = 0;
			m_iTY = 75;
			break;
		}
//		if(state == 2)
//		{
//			UIPanel temp = m_objThis.AddComponent<UIPanel>();
//			temp.clipping = UIDrawCall.Clipping.SoftClip;
//			temp.clipOffset = new Vector2((960 + ClientMain.m_iMoveX * 2) / 2 - 90 , -(640 + ClientMain.m_iMoveY * 2 - 50));
//			temp.baseClipRegion = new Vector4(0,0,960 + ClientMain.m_iMoveX * 2,100);
//			temp.clipSoftness = new Vector2(20,0);
//			temp.depth = 10;
//		}
//		m_MainCityListButton_L = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_L", 0);
//		m_MainCityListButton_RT = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_RT", 1);
//		m_MainCityListButton_RB = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_RB", 2);
//		m_MainCityListButton_R = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_R", 3);
	}

	public void UpData()
	{
		if(m_isAnim)
		{
			bool tempAnim = true;
			for(int i = 0; i < m_listFunctionButtonManager.Count; i ++)
			{
				if(!m_listFunctionButtonManager[i].Move())
				{
					tempAnim = false;
				}
//				if(m_iState == 2)
//				{
////					Debug.Log(m_FunctionButtonManager[i].transform.localPosition.x);
//					if(m_FunctionButtonManager[i].transform.localPosition.x >= 960 + ClientMain.m_iMoveX * 2 - 50)
//					{
//						m_FunctionButtonManager[i].m_ButtonSprite.alpha = 0;
//					}
//					else if(m_FunctionButtonManager[i].transform.localPosition.x <= 960 + ClientMain.m_iMoveX * 2 - 50 - 300)
//					{
//						m_FunctionButtonManager[i].m_ButtonSprite.alpha = 1;
//					}
//					else
//					{
//						float alp = (m_FunctionButtonManager[i].transform.localPosition.x - (960 + ClientMain.m_iMoveX * 2 - 50 - 300)) / 300;
//						m_FunctionButtonManager[i].m_ButtonSprite.alpha = 1 - alp;
//					}
//				}
			}
			if(tempAnim)
			{
				for(int i = 0; i < m_listFunctionButtonManager.Count; i ++)
				{
					if(FunctionOpenTemp.GetTemplateById(m_listFunctionButtonManager[i].m_index).m_parent_menu_id != -1)
					{
						GameObject.Destroy(m_listFunctionButtonManager[i].gameObject);
						m_listFunctionButtonManager.RemoveAt(i);
						i --;
					}
				}
				if(m_DelegateAnimEnd != null)
				{
					m_DelegateAnimEnd();
				}
				m_isAnim = false;
			}
		}
	}

	public void setBPos(int x, int y, int tx, int ty)
	{
		m_iBX = x;
		m_iBY = y;
		m_iTX = tx;
		m_iTY = ty;
	}

	public void addButton(FunctionOpenTemp button)
	{
		int index = button.m_iID;
		GameObject tempObject = GameObject.Instantiate(MainCityUI.m_MainCityUI.ButtonPrefab) as GameObject;
		tempObject.transform.parent = m_objThis.transform;
		tempObject.transform.localScale = Vector3.one;

		tempObject.transform.name = "MainCityUIButton_" + index;

		FunctionButtonManager tempButtonManager = tempObject.GetComponent<FunctionButtonManager>();
		tempObject.SetActive(true);
		tempButtonManager.SetData(button);
		m_listFunctionButtonManager.Add(tempButtonManager);
		if(button.m_show_red_alert)
		{
			tempButtonManager.ShowRedAlert();
		}
		tempButtonManager.Teshu();
	}

	public void addButton(FunctionButtonManager tempObject)
	{
		tempObject.transform.parent = m_objThis.transform;
		tempObject.transform.localPosition = new Vector3(480 + ClientMain.m_iMoveX, -(320 + ClientMain.m_iMoveY), 0);
		tempObject.transform.localScale = Vector3.one;
		tempObject.transform.name = "MainCityUIButton_" + tempObject.m_index;
		m_listFunctionButtonManager.Add(tempObject);
		if(tempObject.m_MYNGUIButtonMessage.panel == null || tempObject.m_MYNGUIButtonMessage.panel != MainCityUI.m_MainCityUI)
		{
			tempObject.m_MYNGUIButtonMessage.panel = MainCityUI.m_MainCityUI;
		}
	}

	public void reMoveButton(int id)
	{
		FunctionButtonManager temp = getButtonManagerByID(id);
		m_listFunctionButtonManager.Remove(temp);
		GameObject.Destroy(temp.gameObject);
	}
	
	public void sortButtonS()
	{
		m_listFunctionButtonManager.Sort();
	}

	public void setPos()
	{
		for(int i = 0; i < m_listFunctionButtonManager.Count; i ++)
		{
			m_listFunctionButtonManager[i].m_iWantToX = m_iBX + i * m_iTX;
			m_listFunctionButtonManager[i].m_iWantToY = m_iBY + i * m_iTY;
			m_listFunctionButtonManager[i].setMoveDis();
		}
	}

	public void setEndPos()
	{
		for(int i = 0; i < m_listFunctionButtonManager.Count; i ++)
		{
			m_listFunctionButtonManager[i].GoToPos();
		}
	}

	public void setMove(AnimEnd animEnd)
	{
		m_DelegateAnimEnd = animEnd;
		m_isAnim = true;
	}

	public FunctionButtonManager getButtonManagerByID(int id)
	{
		for(int i = 0; i < m_listFunctionButtonManager.Count; i ++)
		{
			if(m_listFunctionButtonManager[i].m_index == id)
			{
				return m_listFunctionButtonManager[i];
			}
		}
		return null;
	}
}
