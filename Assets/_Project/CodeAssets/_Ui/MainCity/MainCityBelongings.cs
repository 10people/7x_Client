using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI controller in main city ui right top.
/// </summary>
public class MainCityBelongings : MYNGUIPanel , SocketListener
{
	public UILabel m_MoneyNum;
	public UILabel m_yuanbaoNuM;
	public UILabel m_energyNuM;

	public List<MYNGUIButtonMessage> m_listButtonMessage;

	
	void Start()
	{
		SocketTool.RegisterSocketListener(this);
		//money info
		m_MoneyNum.text = JunZhuData.Instance().m_junzhuInfo.jinBi.ToString();
		if (JunZhuData.Instance().m_junzhuInfo.jinBi > 100000000)
		{
			m_MoneyNum.text = JunZhuData.Instance().m_junzhuInfo.jinBi / 10000 + "万";
		}
		
		//ingot info
		m_yuanbaoNuM.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
		if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 100000000)
		{
			m_yuanbaoNuM.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000) + "万";
		}
		
		//energy info
		string energyText = JunZhuData.Instance().m_junzhuInfo.tili > 10000 ? JunZhuData.Instance().m_junzhuInfo.tili / 10000 + "万" : JunZhuData.Instance().m_junzhuInfo.tili.ToString();
		string energyMaxText = JunZhuData.Instance().m_junzhuInfo.tiLiMax > 10000 ? JunZhuData.Instance().m_junzhuInfo.tiLiMax / 10000 + "万" : JunZhuData.Instance().m_junzhuInfo.tiLiMax.ToString();
		m_energyNuM.text = energyText + "/" + energyMaxText;

	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message == null)
		{
			return false;
		}
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.JunZhuInfoRet:
		{
			Start();
			return true;
		}
		default:
			return false;
		}
	}
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("RT_BuyMoney") != -1)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
		}
		else if(ui.name.IndexOf("RT_BuyRecharge") != -1)
		{
            EquipSuoData.TopUpLayerTip();
        }
		else if(ui.name.IndexOf("RT_BuyEnergy") != -1)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
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
