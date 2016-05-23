using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RechargePage : GeneralInstance<RechargePage> {

	public delegate void RechargeDelegate ();
	public RechargeDelegate M_RechargeDelegate;

	public GameObject m_rechargeObj;
	public GameObject m_vipDesObj;
	public GameObject m_vipItemListObj;

	private bool m_firstPage = true;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		Debug.Log ("Start");
		m_triggerHandler.m_handler += OnTriggerEnterCallBack;
	}

	public void ChangePage (bool firstPage)
	{
		m_vipBtnLabel.text = firstPage ? "特  权" : "充  值";
		m_rechargeObj.SetActive (firstPage);
		m_vipDesObj.SetActive (!firstPage);
		m_vipItemListObj.SetActive (!firstPage);
	}

	#region RechargePage

	public VipInfoResp M_VipResp;

	public UIScrollView m_sc;
	public UIGrid m_grid;
	public UIScrollBar m_sb;

	public GameObject m_rechargeItem;
	private List<GameObject> m_rechargeItemList = new List<GameObject> ();

	public GameObject m_nextVipObj;
	public UILabel m_vipBtnLabel;
	public UILabel m_vip1;
	public UIProgressBar m_expPb;
	public UILabel m_expLabel;
	public UILabel m_need;
	public UILabel m_vip2;

	public void InItRechargePage (VipInfoResp tempResp)
	{
		M_VipResp = tempResp;

		InItVipInfo ();

		m_rechargeItemList = QXComData.CreateGameObjectList (m_rechargeItem,m_grid,tempResp.infos.Count,m_rechargeItemList);

		for (int i = 0;i < m_rechargeItemList.Count;i ++)
		{
			RechargeItem recharge = m_rechargeItemList[i].GetComponent<RechargeItem> ();
			recharge.InItRechargeItem (tempResp.infos[i]);
		}
	}

	void InItVipInfo ()
	{
		m_nextVipObj.SetActive (M_VipResp.isMax ? false : true);
		m_vip1.text = "[b]VIP " + M_VipResp.vipLevel + "[-]";
		if (M_VipResp.isMax)
		{
			m_expPb.value = 1;
			m_expLabel.text = "";
		}
		else
		{
			m_vip2.text = "[b]VIP " + (M_VipResp.vipLevel + 1).ToString() + "[-]";
			m_need.text = "再充" + (M_VipResp.needYb - M_VipResp.hasYb).ToString();
			m_expLabel.text = M_VipResp.hasYb.ToString() + "/" + M_VipResp.needYb.ToString();
			m_expPb.value = M_VipResp.hasYb / float.Parse (M_VipResp.needYb.ToString ());
		}
	}

	#endregion

	#region VipPage

	public UILabel m_vipTitle;

	public TriggerHandler m_triggerHandler;

	public GameObject m_rewardItem;
	private List<GameObject> m_rewardList = new List<GameObject>();

	void InItVipPage ()
	{
		List<VipTemplate> vipTemplates = VipTemplate.templates;
		for (int i = 0;i < vipTemplates.Count;i ++)
		{
			if (vipTemplates[i].lv == 0)
			{
				vipTemplates.RemoveAt (i);
				break;
			}
		}
		m_rewardList = QXComData.CreateGameObjectList (m_rewardItem,vipTemplates.Count,m_rewardList);

		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			m_rewardList[i].transform.localPosition = new Vector3(i * 880,0,0);
			RechargeVipItem vipItem = m_rewardList[i].GetComponent<RechargeVipItem> ();
			vipItem.InItVipItem (vipTemplates[i]);
		}
	}

	public void ChangeVipTitle (int tempVip)
	{
		m_vipTitle.text = "VIP" + tempVip + "    等级特权";
	}

	/// <summary>
	/// 刷新vip特权奖励领取状态
	/// </summary>
	/// <param name="tempVip">Temp vip.</param>
	public void RefreshVipReward (int tempVip)
	{
		M_VipResp.getRewardVipList.Add (tempVip);
		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			RechargeVipItem vipItem = m_rewardList[i].GetComponent<RechargeVipItem> ();
			if (tempVip == vipItem.M_VipTemp.lv)
			{
				vipItem.SetGetBtnState (false);
			}
		}
	}

	/// <summary>
	/// Determines whether this instance is reward get the specified tempVip.
	/// </summary>
	/// <returns><c>true</c> if this instance is reward get the specified tempVip; otherwise, <c>false</c>.</returns>
	/// <param name="tempVip">Temp vip.</param>
	public bool IsRewardGet (int tempVip)
	{
		foreach (int vipId in M_VipResp.getRewardVipList)
		{
			if (tempVip == vipId)
			{
				return true;
			}
		}
		return false;
	}

	void OnTriggerEnterCallBack (Collider col)
	{
		Debug.Log ("col.name:" + col.name);
		
		RechargeVipItem vipItem = col.GetComponent<RechargeVipItem> ();
		if (vipItem)
		{
			ChangeVipTitle (vipItem.M_VipTemp.lv);
		}
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "VipBtn":

			if (m_firstPage)
			{
				m_firstPage = false;
				ChangePage (m_firstPage);
				InItVipPage ();
			}
			else
			{
				m_firstPage = true;
				ChangePage (m_firstPage);
				InItRechargePage (M_VipResp);
			}

			break;

		case "GerRewardBtn":

			RechargeVipItem vipItem = ui.GetComponentInParent <RechargeVipItem> ();
			RechargeData.Instance.GetVipRewardReq (vipItem.M_VipTemp.lv,vipItem.M_RewardDataList);

			break;

		case "LeftBtn":
			break;

		case "RightBtn":
			break;

		case "CloseBtn":

			if (M_RechargeDelegate != null)
			{
				M_RechargeDelegate ();
			}

			break;

		default:

			RechargeItem recharge = ui.GetComponent<RechargeItem> ();
			if (recharge.M_ChongTimes != null)
			{
				if (recharge.M_ChongTimes.id == 0)//月卡
				{
					string text = "当前月卡剩余" + MyColorData.getColorString (4,M_VipResp.yueKaLeftDays) + "天（充值可累计）\n是否购买？";
					QXComData.CreateBoxDiy (text,false,YueKaBuyCallBack);
				}
				else if (recharge.M_ChongTimes.id == 1)//终生卡
				{
					if (recharge.M_ChongTimes.times > 0)
					{
						string text = "终生卡只能购买一次！";
						QXComData.CreateBoxDiy (text,true,null);
					}
					else
					{
						#if UNITY_EDITOR
						RechargeData.Instance.RechargeReq (recharge.M_ChongTimes.id,ChongZhiTemplate.GetChongZhiTempById (recharge.M_ChongTimes.id).needNum);
						#endif

						#if UNITY_ANDROID
						RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,recharge.M_ChongTimes.id,recharge.M_RechargeTemp.addNum);
						Bonjour.BuyYuanBao (recharge.M_RechargeTemp.addNum + recharge.M_RechargeTemp.extraYuanbao);
						#endif
					}
				}
				else
				{
					#if UNITY_EDITOR
					RechargeData.Instance.RechargeReq (recharge.M_ChongTimes.id,ChongZhiTemplate.GetChongZhiTempById (recharge.M_ChongTimes.id).needNum);
					#endif

					#if UNITY_ANDROID
					RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,recharge.M_ChongTimes.id,recharge.M_RechargeTemp.addNum);
					Bonjour.BuyYuanBao (recharge.M_RechargeTemp.addNum + (recharge.M_ChongTimes.times > 0 ? recharge.M_RechargeTemp.extraYuanbao : recharge.M_RechargeTemp.addNum));
					#endif
				}
			}

			break;
		}
	}

	void YueKaBuyCallBack (int i)
	{
		if (i == 2)
		{
			#if UNITY_EDITOR
			RechargeData.Instance.RechargeReq (M_VipResp.infos[0].id,ChongZhiTemplate.GetChongZhiTempById (M_VipResp.infos[0].id).needNum);
			#endif

			#if UNITY_ANDROID
			RechargeItem vipItem = m_rechargeItemList[0].GetComponent<RechargeItem> ();
			RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,vipItem.M_ChongTimes.id,vipItem.M_RechargeTemp.addNum);
			Bonjour.BuyYuanBao (vipItem.M_RechargeTemp.addNum + (vipItem.M_ChongTimes.times > 0 ? vipItem.M_RechargeTemp.extraYuanbao : vipItem.M_RechargeTemp.addNum));
			#endif
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
