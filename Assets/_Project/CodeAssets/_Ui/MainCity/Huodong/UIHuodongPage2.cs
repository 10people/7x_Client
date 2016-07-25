using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage2 : MYNGUIPanel , SocketListener 
{
	public ActivityGrowthFundResp m_Info;
	public UIHuodongPage2EnemtData m_UIHuodongPage2EnemtData;
	public GameObject m_objButtonGoumai;
	public GameObject m_objButtonYiGou;
	public UISprite m_spriteVip;
	public List<UIHuodongPage2EnemtData> m_listUIHuodongPage2EnemtData = new List<UIHuodongPage2EnemtData>();
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
		
	public void setData(ActivityGrowthFundResp data)
	{
		m_Info = data;
		if(m_Info.leveList == null)
		{
			return;
		}
		m_spriteVip.spriteName = "v" + (int)CanshuTemplate.GetValueByKey (CanshuTemplate.CHENGZHANGJIJIN_VIP);
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
		for(int i = 0; i < m_listUIHuodongPage2EnemtData.Count; i ++)
		{
			GameObject.Destroy(m_listUIHuodongPage2EnemtData[i].gameObject);
		}
		m_listUIHuodongPage2EnemtData = new List<UIHuodongPage2EnemtData>();
		MainCityUI.SetRedAlert(1394 ,false);
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
				if(m_Info.result == 2)
				{
					MainCityUI.SetRedAlert(1394 ,true);
				}
			}
			tempEnemtData.m_spriteButtonBG.name = "ButtonLingqu" + i;
			m_listUIHuodongPage2EnemtData.Add(tempEnemtData);
		}
		if(m_Info.result == 2)
		{
			m_objButtonGoumai.SetActive(false);
			m_objButtonYiGou.SetActive(true);
		}
		else
		{
			m_objButtonGoumai.SetActive(true);
			SparkleEffectItem.OpenSparkle( m_objButtonGoumai, SparkleEffectItem.MenuItemStyle.Common_Icon, -1);
			m_objButtonYiGou.SetActive(false);
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
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("ButtonLingqu") != -1)
		{
			if(m_Info.result == 2)
			{
				int index = int.Parse(ui.name.Substring(12, ui.name.Length - 12));
				Global.ScendID(ProtoIndexes.C_ACTIVITY_GROWTHFUND_GETREWARD_REQ, m_Info.leveList[index].id);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_INFO_REQ);
			}
			else
			{
				ClientMain.m_UITextManager.createText("请购买基金");
			}

		}
		else if(ui.name.IndexOf("ButtonGoumai") != -1)
		{
			if(m_Info.result == 0)
			{
				Global.CreateFunctionIcon(1901);
			}
			else if(m_Info.result == 1)
			{
				if(JunZhuData.Instance().m_junzhuInfo.yuanBao >= m_Info.costNum)
				{
					Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_BUY_REQ);
					Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_INFO_REQ);
				}
				else
				{
					Global.CreateFunctionIcon(101);
				}
			}
			else
			{
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_BUY_REQ);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_GROWTHFUND_INFO_REQ);
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
