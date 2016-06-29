using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage5 : MYNGUIPanel , SocketListener 
{
	public ActivityAchievementResp m_Info;
	public UIHuodongPage2EnemtData m_UIHuodongPage2EnemtData;
	public List<UIHuodongPage2EnemtData> m_listUIHuodongPage2EnemtData = new List<UIHuodongPage2EnemtData>();
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}

	public void setData(ActivityAchievementResp data)
	{
		m_Info = data;
		for(int i = 0; i < m_listUIHuodongPage2EnemtData.Count; i ++)
		{
			GameObject.Destroy(m_listUIHuodongPage2EnemtData[i].gameObject);
		}
		m_listUIHuodongPage2EnemtData = new List<UIHuodongPage2EnemtData>();

		int tempIndex = 0;
		for(int i = 0; i < m_Info.leveList.Count; i ++)
		{
			GameObject tempObj = GameObject.Instantiate(m_UIHuodongPage2EnemtData.gameObject);
			tempObj.SetActive(true);
			tempObj.name = "Enemt" + i;
			tempObj.transform.parent = m_UIHuodongPage2EnemtData.gameObject.transform.parent;
			tempObj.transform.localScale = Vector3.one;
			tempObj.transform.localPosition = new Vector3(69, 40 - 90 * i, 0);
			UIHuodongPage2EnemtData tempEnemtData = tempObj.GetComponent<UIHuodongPage2EnemtData>();
			for(int q = 0; q < m_Info.leveList[i].awardList.Count; q ++)
			{
				GameObject tempIconObj = GameObject.Instantiate(tempEnemtData.m_IconSampleManager.gameObject);
				tempIconObj.SetActive(true);
				tempIconObj.name = "Icon" + q;
				tempIconObj.transform.parent = tempEnemtData.m_IconSampleManager.gameObject.transform.parent;
				tempIconObj.transform.localPosition = new Vector3(-185 + q * 50, -16, 0);
				IconSampleManager tempIconSampleManager = tempIconObj.GetComponent<IconSampleManager>();
				tempIconSampleManager.SetIconByID(m_Info.leveList[i].awardList[q].itemId, "x" + m_Info.leveList[i].awardList[q].itemNumber, 3);
				tempIconSampleManager.SetIconPopText(m_Info.leveList[i].awardList[q].itemId);
				tempIconObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				tempEnemtData.m_listIconSampleManager.Add(tempIconSampleManager);
			}
			tempEnemtData.m_sName.text = m_Info.leveList[i].des;
			if(m_Info.leveList[i].process < m_Info.leveList[i].maxProcess)
			{
				tempEnemtData.m_sJindu.text = "[ff0000]" + m_Info.leveList[i].process + "[-]/" + m_Info.leveList[i].maxProcess;
			}
			else
			{
				tempEnemtData.m_sJindu.text = m_Info.leveList[i].process + "/" + m_Info.leveList[i].maxProcess;
			}
			if(m_Info.leveList[i].process < m_Info.leveList[i].maxProcess)
			{
				tempEnemtData.m_BoxCollider.enabled = false;
				tempEnemtData.m_spriteButtonBG.color = Color.gray;
				tempEnemtData.m_UILabelType.setType(11);
			}
			else
			{
				tempEnemtData.m_BoxCollider.enabled = true;
				tempEnemtData.m_spriteButtonBG.color = Color.white;
				tempEnemtData.m_UILabelType.setType(10);
			}
			tempEnemtData.m_spriteButtonBG.name = "ButtonLingqu" + i;
			m_listUIHuodongPage2EnemtData.Add(tempEnemtData);
		}
		MainCityUI.SetRedAlert(144 ,false);
		for(int i = 0; i < m_Info.leveList.Count; i ++)
		{
			if(m_Info.leveList[i].process >= m_Info.leveList[i].maxProcess)
			{
				m_listUIHuodongPage2EnemtData[i].gameObject.transform.localPosition = new Vector3(69, 40 - 90 * tempIndex, 0);
				tempIndex ++;
				MainCityUI.SetRedAlert(144 ,true);
			}
		}
		for(int i = 0; i < m_Info.leveList.Count; i ++)
		{
			if(m_Info.leveList[i].process < m_Info.leveList[i].maxProcess)
			{
				m_listUIHuodongPage2EnemtData[i].gameObject.transform.localPosition = new Vector3(69, 40 - 90 * tempIndex, 0);
				tempIndex ++;
			}
		}
		//		for(int i = 0; i < m_listIconSample.Count; i ++)
		//		{
		//			m_listIconSample[i].SetIconByID(m_ExploreResp.awardsList[i].itemId, "x" + m_ExploreResp.awardsList[i].itemNumber);
		//		}
	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ACTIVITY_FIRST_CHARGE_GETREWARD_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityGetRewardResp tempInfo = new ActivityGetRewardResp();

				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				switch(tempInfo.result)
				{
				case 1:
					Global.CreateBox("领奖失败", "您还没有充值", "", null, "确定", null, null);
					break;
				case 2:
					Global.CreateBox("领奖失败", "您已领取过奖励", "", null, "确定", null, null);
					break;
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
	int m_iPid;
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("ButtonLingqu") != -1)
		{
			int index = int.Parse(ui.name.Substring(12, ui.name.Length - 12));
			if(m_iPid != m_Info.leveList[index].id)
			{
				m_iPid = m_Info.leveList[index].id;
				Global.ScendID(ProtoIndexes.C_ACTIVITY_ACHIEVEMENT_GET_REQ, m_Info.leveList[index].id, ProtoIndexes.S_ACTIVITY_ACHIEVEMENT_GET_RESP);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_ACHIEVEMENT_INFO_REQ, ProtoIndexes.S_ACTIVITY_ACHIEVEMENT_INFO_RESP);
				if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
				{
					UIHuodong.m_UIHuodong.m_UIScrollView.enabled = true;
				}
			}
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
