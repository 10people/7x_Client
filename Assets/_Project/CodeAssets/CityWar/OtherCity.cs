using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class OtherCity : GeneralInstance<OtherCity> {

	public delegate void OtherCityDelegate ();
	public OtherCityDelegate M_OtherCityDelegate;

	private CityFightInfoResp m_OtherCityResp;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;

	public GameObject m_otherCity;
	private List<GameObject> m_otherCityList = new List<GameObject> ();

	private OtherCityItem m_otherCityItem;

	private int curCityId;		//当前挑战难度郡城id

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItOtherCity (CityFightInfoResp tempResp)
	{
		m_OtherCityResp = tempResp;

		curCityId = 0;
		for (int i = 0;i < tempResp.cityList.Count;i ++)
		{
			if (tempResp.cityList[i].cityState == 0)
			{
				curCityId = tempResp.cityList[i].cityId;
				break;
			}
		}

		m_otherCityList = QXComData.CreateGameObjectList (m_otherCity,tempResp.cityList.Count,m_otherCityList);

		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
			m_otherCityList[i].transform.localPosition = new Vector3(i * 155,0,0);
			m_sc.UpdateScrollbars (true);
			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			cityItem.InItOtherCity (tempResp.cityList[i],curCityId);
		}

		SetPos ();

		m_sc.enabled = m_otherCityList.Count < 4 ? false : true;
//		m_sb.gameObject.SetActive (false);
	}

	void SetPos ()
	{
		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
//			if (i == m_otherCityList.Count - 1)
//			{
//				UIWidget widget = m_otherCityList[i].GetComponent<UIWidget>();
//				
//				float widgetValue = m_sc.GetWidgetValueRelativeToScrollView (widget).x;
//				if (widgetValue < 0 || widgetValue > 1)
//				{
//					m_sc.SetWidgetValueRelativeToScrollView(widget, 0);
//					float scrollValue = m_sc.GetSingleScrollViewValue();
//					if (scrollValue >= 1) m_sb.value = 0.99f;
//					if (scrollValue <= 0) m_sb.value = 0.01f;
//				}
//			}

			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			if (cityItem.M_CityInfo.cityState2 == 1)
			{
				UIWidget widget = m_otherCityList[i].GetComponent<UIWidget>();
				
				float widgetValue = m_sc.GetWidgetValueRelativeToScrollView (widget).x;
				if (widgetValue < 0 || widgetValue > 1)
				{
					m_sc.SetWidgetValueRelativeToScrollView(widget, 0);
					float scrollValue = m_sc.GetSingleScrollViewValue();
					if (scrollValue >= 1) m_sb.value = 0.99f;
					if (scrollValue <= 0) m_sb.value = 0.01f;
				}
			}
		}
	}

	public void RefreshOtherCity (int cityId)
	{
		for (int i = 0;i < m_otherCityList.Count;i ++)
		{
			OtherCityItem cityItem = m_otherCityList[i].GetComponent<OtherCityItem> ();
			if (cityId == cityItem.M_CityInfo.cityId)
			{
				cityItem.M_CityInfo.cityState2 = 1;
				cityItem.InItOtherCity (cityItem.M_CityInfo,curCityId);
			}
		}
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			if (M_OtherCityDelegate != null)
			{
				M_OtherCityDelegate ();
			}

			break;
		case "EnterFightBtn":

			OtherCityItem cityItem = ui.transform.parent.GetComponent<OtherCityItem> ();
			if (cityItem.M_CityInfo != null)
			{
				m_otherCityItem = cityItem;
				if (cityItem.M_canJoin)
				{
					if (cityItem.M_CityInfo.cityState2 == 1)
					{
						CityWarOperateReq operateReq = new CityWarOperateReq();
						operateReq.operateType = CityOperateType.ENTER_FIGHT;
						operateReq.cityId = cityItem.M_CityInfo.cityId;
						CityWarData.Instance.CityOperate (operateReq);
					}
					else
					{
						string text = "您的联盟拥有虎符" + MyColorData.getColorString (4,CityWarPage.m_instance.CityResp.haveHufu.ToString ()) + "\n是否花费[e5e205]" + JCZCityTemplate.GetJCZCityTemplateById (cityItem.M_CityInfo.cityId).cost + "[-]虎符对该城池宣战？";
						QXComData.CreateBoxDiy (text,false,OtherCityBid);
					}
				}
			}

			break;
		default:
			break;
		}
	}

	void OtherCityBid (int i)
	{
		if (i == 2)
		{
			CityWarOperateReq operateReq = new CityWarOperateReq();
			operateReq.operateType = CityOperateType.BID;
			operateReq.cityId = m_otherCityItem.M_CityInfo.cityId;
			operateReq.price = m_otherCityItem.M_Template.cost;
			CityWarData.Instance.CityOperate (operateReq);
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
