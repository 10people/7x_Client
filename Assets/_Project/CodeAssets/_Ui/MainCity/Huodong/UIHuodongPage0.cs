using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage0 : MYNGUIPanel , SocketListener 
{
	public List<IconSampleManager> m_listIconSample;
	public UILabel m_labelDes;
	public GameObject m_objButton;
	public UILabel m_labelButton;
	public ExploreResp m_ExploreResp;
	public GameObject m_Eff;
	// Use this for initialization
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_Eff, EffectTemplate.getEffectTemplateByEffectId(620243).path);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}

	public void setData(ExploreResp exploreResp)
	{
		m_ExploreResp = exploreResp;
		MainCityUI.SetRedAlert(1422 ,false);
		switch(m_ExploreResp.success)
		{
		case 0:
			m_objButton.SetActive(true);
			m_labelButton.text = "前往充值";
			SparkleEffectItem.OpenSparkle( m_objButton, SparkleEffectItem.MenuItemStyle.Common_Icon, -1);
			break;
		case 1:
			m_objButton.SetActive(true);
			m_labelButton.text = "领 取";
			MainCityUI.SetRedAlert(1422 ,true);
			break;
		case 2:
			m_objButton.SetActive(false);
			break;
		}
		for(int i = 0; i < m_listIconSample.Count; i ++)
		{
			m_listIconSample[i].SetIconByID(m_ExploreResp.awardsList[i].itemId, "x" + m_ExploreResp.awardsList[i].itemNumber, 8);
			m_listIconSample[i].SetIconPopText(m_ExploreResp.awardsList[i].itemId);
		}
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
			switch(m_ExploreResp.success)
			{
			case 0:
				RechargeData.Instance.RechargeDataReq ();
				break;
			case 1:
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FIRST_CHARGE_GETREWARD_REQ);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_FIRST_CHARGE_REWARD_REQ);
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
