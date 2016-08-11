using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodong :  MYNGUIPanel , SocketListener
{
	private int m_iIndexPanel = 0;
	public static UIHuodong m_UIHuodong;
	public ActivityFunctionResp m_Info;
	public GameObject m_objTitle;
	public GameObject m_MonetParentObj;
	public UISprite m_spriteSelect;

	public MyPageButtonManager m_UIHuodongData;
	public List<MyPageButtonManager> m_listUIHuodongData = new List<MyPageButtonManager>();

	public UIHuodongPage0 m_UIHuodongPage0;
	public UIHuodongPage1 m_UIHuodongPage1;
	public UIHuodongPage2 m_UIHuodongPage2;
	public UIHuodongPage3 m_UIHuodongPage3;
	public UIHuodongPage4 m_UIHuodongPage4;
	public UIHuodongPage5 m_UIHuodongPage5;
	public UIHuodongPage6 m_UIHuodongPage6;
	public UIHuodongPage7 m_UIHuodongPage7;

	public GameObject m_objShow;

	public List<GameObject> m_listObj;

	public bool m_isAdd = false;

	private int m_iPageIndex;
	private int m_iSelectEndY = -99999;
	
	void Awake()
	{
		SocketTool.RegisterSocketListener(this);
		Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
//		ActivityFunctionResp temp = new ActivityFunctionResp();
//		temp.functionList = new List<int>();

//		temp.functionList.Add(1422);
//		temp.functionList.Add(1393);
//		temp.functionList.Add(1394);
//		temp.functionList.Add(1391);
//		temp.functionList.Add(600200);
//		temp.functionList.Add(144);

//		setData(temp);

//		Global.ScendNull(ProtoIndexes.C_ACTIVITY_FIRST_CHARGE_REWARD_REQ);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	
	void Start ()
	{
		MainCityUI.setGlobalTitle(m_objTitle, "活动", 0, 0);
		MainCityUI.setGlobalBelongings(m_MonetParentObj, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);

		m_UIHuodong = this;
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_Info != null)
		{
			for(int i = 0; i < m_Info.functionList.Count; i ++)
			{
				if(FunctionOpenTemp.GetTemplateById(m_Info.functionList[i]) != null)
				{
					m_listUIHuodongData[i].m_spriteRed.gameObject.SetActive(FunctionOpenTemp.GetTemplateById(m_Info.functionList[i]).m_show_red_alert);
				}
			}
		}
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

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityFunctionResp tempInfo = new ActivityFunctionResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				if(m_Info != null)
				{
					if(tempInfo.functionList.Count == m_Info.functionList.Count)
					{
						return false;
					}
				}
				setData(tempInfo);

				if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
				{
					bool tempHaveShouchong = false;
					for(int i = 0; i < m_Info.functionList.Count; i ++)
					{
						if(m_Info.functionList[i] == 1422)
						{
							tempHaveShouchong = true;
						}
					}
				}
				break;
			}
			case ProtoIndexes.S_ACTIVITY_FIRST_CHARGE_REWARD_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ExploreResp tempInfo = new ExploreResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				m_UIHuodongPage0.setData(tempInfo);
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 0;
				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
				{
					//if(!UIYindao.m_UIYindao.m_isOpenYindao)
					{
						TaskData.Instance.m_iCurMissionIndex = 100173;
						ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
						tempTaskData.m_iCurIndex = 1;
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
					}
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.S_ACTIVITY_MONTH_CARD_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityCardResp tempInfo = new ActivityCardResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage1.setData(tempInfo);

				m_listObj[m_iIndexPanel].SetActive(false);

				m_iIndexPanel = 1;

				m_listObj[m_iIndexPanel].SetActive(true);

				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
				{
					//if(!UIYindao.m_UIYindao.m_isOpenYindao)
					{
						TaskData.Instance.m_iCurMissionIndex = 100173;
						ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
						tempTaskData.m_iCurIndex = 1;
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
					}
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.S_ACTIVITY_GROWTHFUND_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityGrowthFundResp tempInfo = new ActivityGrowthFundResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage2.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 2;

				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.S_ACTIVITY_STRENGTH_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityGetStrengthResp tempInfo = new ActivityGetStrengthResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage3.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 3;
				
				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.S_ACTIVITY_LEVEL_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivitLevelGiftResp tempInfo = new ActivitLevelGiftResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage4.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 4;
				
				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.S_ACTIVITY_ACHIEVEMENT_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityAchievementResp tempInfo = new ActivityAchievementResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage5.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 5;

				if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
				{
					//if(!UIYindao.m_UIYindao.m_isOpenYindao)
					{
						TaskData.Instance.m_iCurMissionIndex = 100173;
						ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
						tempTaskData.m_iCurIndex = 2;
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
					}
				}

				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FUNCTIONLIST_INFO_REQ, ProtoIndexes.S_ACTIVITY_FUNCTIONLIST_INFO_RESP);
				break;
			}
			case ProtoIndexes.GET_QQ_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ErrorMessage tempInfo = new ErrorMessage();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage6.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 6;
				
				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				break;
			}
			case ProtoIndexes.GET_REQ_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ErrorMessage tempInfo = new ErrorMessage();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_UIHuodongPage7.setData(tempInfo);
				
				m_listObj[m_iIndexPanel].SetActive(false);
				
				m_iIndexPanel = 7;
				
				m_listObj[m_iIndexPanel].SetActive(true);
				m_objShow.SetActive(true);
				if(!m_isAdd)
				{
					MainCityUI.TryAddToObjectList(gameObject);
					UIYindao.m_UIYindao.CloseUI();
					m_isAdd = true;
				}
				break;
			}
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}

	
	void OnCloseWindow()
	{
	}
	
	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("PageButton") != -1)
		{
			int index = int.Parse(ui.name.Substring(10, ui.name.Length - 10));
			for(int i = 0; i < m_Info.functionList.Count; i ++)
			{
				if(index == m_Info.functionList[i])
				{
					m_iPageIndex = i;
					m_iSelectEndY = 128 - i * 48;
					m_listUIHuodongData[i].m_spritePageButton.spriteName = "Function_" + m_Info.functionList[i] + "_1";
				}
				else
				{
					m_listUIHuodongData[i].m_spritePageButton.spriteName = "Function_" + m_Info.functionList[i] + "_0";
				}
			}
//			m_spriteSelect.gameObject.transform.parent = m_listUIHuodongData[getIndex(index)].gameObject.transform;
//			m_spriteSelect.gameObject.transform.localPosition = Vector3.zero;
			switch(index)
			{
			case 1422:
			case 0:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FIRST_CHARGE_REWARD_REQ, ProtoIndexes.S_ACTIVITY_FIRST_CHARGE_REWARD_RESP);
				break;
			case 1393:
			case 1:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_MONTH_CARD_REQ, ProtoIndexes.S_ACTIVITY_MONTH_CARD_RESP);
				break;
			case 1394:
			case 2:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_INFO_REQ, ProtoIndexes.S_ACTIVITY_GROWTHFUND_INFO_RESP);
				break;
			case 1391:
			case 3:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_STRENGTH_INFO_REQ, ProtoIndexes.S_ACTIVITY_STRENGTH_INFO_RESP);
				break;
			case 600200:
			case 4:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_LEVEL_INFO_REQ, ProtoIndexes.S_ACTIVITY_LEVEL_INFO_RESP);
				break;
			case 144:
			case 5:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_ACHIEVEMENT_INFO_REQ, ProtoIndexes.S_ACTIVITY_ACHIEVEMENT_INFO_RESP);
				break;
			case 1395:
				Global.ScendNull(ProtoIndexes.GET_QQ_INFO, ProtoIndexes.GET_QQ_INFO);
				break;
			case 1396:
				Global.ScendNull(ProtoIndexes.GET_REQ_INFO, ProtoIndexes.GET_REQ_INFO);
				break;
			}
		}
		else if(ui.name.IndexOf("Close") != -1)
		{
			MainCityUI.TryRemoveFromObjectList(gameObject);
			TreasureCityUI.TryRemoveFromObjectList(gameObject);
			Destroy(gameObject);
		}
	}

	public int getIndex(int id)
	{
		for(int i = 0; i < m_listUIHuodongData.Count; i ++)
		{
			int tempid = int.Parse(m_listUIHuodongData[i].m_spritePageButton.gameObject.name.Substring(10, m_listUIHuodongData[i].gameObject.name.Length - 10));
			if(tempid == id)
			{
				return i;
			}
		}
		return 0;
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

	public void setData(ActivityFunctionResp data)
	{
		for(int i = 0; i < m_listUIHuodongData.Count; i ++)
		{
			GameObject.Destroy(m_listUIHuodongData[i].gameObject);
		}
		m_listUIHuodongData = new List<MyPageButtonManager>();
		m_Info = data;

		for(int i = 0; i < m_Info.functionList.Count; i ++)
		{
			GameObject tempObj = GameObject.Instantiate(m_UIHuodongData.gameObject);
			tempObj.SetActive(true);
			tempObj.name = "PageButton" + m_Info.functionList[i];
			tempObj.transform.parent = m_UIHuodongData.gameObject.transform.parent;
			tempObj.transform.localScale = Vector3.one;
			tempObj.transform.localPosition = new Vector3(-32, 128 - 48 * i, 0);
			MyPageButtonManager tempEnemtData = tempObj.GetComponent<MyPageButtonManager>();
			tempEnemtData.gameObject.name = "Page" + m_Info.functionList[i];
			tempEnemtData.m_spritePageButton.spriteName = "Function_" + m_Info.functionList[i] + "_0";
			tempEnemtData.m_spritePageButton.name = "PageButton" + m_Info.functionList[i];
			tempEnemtData.m_spritePageButton.MakePixelPerfect();
			if(FunctionOpenTemp.GetTemplateById(m_Info.functionList[i]) != null)
			{
				tempEnemtData.m_spriteRed.gameObject.SetActive(FunctionOpenTemp.GetTemplateById(m_Info.functionList[i]).m_show_red_alert);
			}
			m_listUIHuodongData.Add(tempEnemtData);
		}

		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			GameObject tempButtonName = new GameObject();
			tempButtonName.name = Global.m_sPanelWantRun;

			MYClick(tempButtonName);
			if(Global.m_sPanelWantRun == "PageButton144")
			{
				bool tempHaveShouchong = false;
				for(int i = 0; i < m_Info.functionList.Count; i ++)
				{
					if(m_Info.functionList[i] == 1422)
					{
						tempHaveShouchong = true;
					}
				}
			}
			Global.m_sPanelWantRun = "";
			GameObject.Destroy(tempButtonName);
		}
		else
		{
			MYClick(m_listUIHuodongData[0].m_spritePageButton.gameObject);
		}
	}
}
