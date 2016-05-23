using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage1 : MYNGUIPanel , SocketListener 
{
	public List<UIHuodongPage1EnemtData> m_listEnemt;
	public ActivityCardResp m_Info;
	// Use this for initialization
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void setData(ActivityCardResp data)
	{
		m_Info = data;
		for(int i = 0; i < m_Info.monthCard.Count; i ++)
		{
			if(m_Info.monthCard[i].rmDays == -1)
			{
				m_listEnemt[i].m_labelDes.gameObject.SetActive(false);
			}
			else
			{
				m_listEnemt[i].m_labelDes.gameObject.SetActive(true);
				m_listEnemt[i].m_labelDes.text = "剩余天数：[ff0000]" + m_Info.monthCard[0].rmDays + "[-]";
			}
			m_listEnemt[i].m_labelNum.text = "" + m_Info.monthCard[i].giveNum;
			switch(m_Info.monthCard[i].result)
			{
			case 0:
				m_listEnemt[i].m_objButton.SetActive(true);
				m_listEnemt[i].m_labelTime.gameObject.SetActive(false);
				m_listEnemt[i].m_labelButtonDes.text = "购 买";
				break;
			case 1:
				m_listEnemt[i].m_objButton.SetActive(true);
				m_listEnemt[i].m_labelTime.gameObject.SetActive(false);
				m_listEnemt[i].m_labelButtonDes.text = "领 取";
				break;
			case 2:
				m_listEnemt[i].m_objButton.SetActive(false);
				m_listEnemt[i].m_labelTime.gameObject.SetActive(true);
				TimeLabelHelper.Instance.setTimeLabel(m_listEnemt[i].m_labelTime, "", m_Info.monthCard[i].cd, EndTime);
				break;
			}
		}
//		m_ExploreResp = exploreResp;
//		Debug.Log(m_ExploreResp.success);
//		switch(m_ExploreResp.success)
//		{
//		case 0:
//			m_objButton.SetActive(true);
//			m_labelButton.text = "前往充值";
//			break;
//		case 1:
//			m_objButton.SetActive(true);
//			m_labelButton.text = "领 取";
//			break;
//		case 2:
//			m_objButton.SetActive(false);
//			break;
//		}
//		for(int i = 0; i < m_listIconSample.Count; i ++)
//		{
//			m_listIconSample[i].SetIconByID(m_ExploreResp.awardsList[i].itemId, "x" + m_ExploreResp.awardsList[i].itemNumber);
//		}
	}

	public void EndTime()
	{

	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ACTIVITY_MONTH_CARD_REWARD_RESP:
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
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("ButtonLingqu") != -1)
		{
			Debug.Log(ui.name);
			int index = int.Parse(ui.name.Substring(12, 1));
			switch(m_Info.monthCard[index].result)
			{
			case 0:
				TopUpLoadManagerment.LoadPrefab(false);
				break;
			case 1:
				Global.ScendID(ProtoIndexes.C_ACTIVITY_MONTH_CARD_REWARD_REQ, index);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_MONTH_CARD_REQ);
				break;
			case 2:
				break;
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
