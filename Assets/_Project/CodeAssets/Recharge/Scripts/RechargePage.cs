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

	public GameObject m_anchorTR;
	public GameObject m_anchorTL;

	public UIAtlas m_rechargeAtlas;
	public UIAtlas m_comAtlas;

	private bool m_firstPage = true;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		Debug.Log ("Start");
		m_triggerHandler.m_handler += OnTriggerEnterCallBack;
		QXComData.LoadYuanBaoInfo (m_anchorTR);
		QXComData.LoadTitleObj (m_anchorTL,"充值");
	}

	public void ChangePage (bool firstPage)
	{
		m_firstPage = firstPage;
		m_vipBtn.atlas = firstPage ? m_rechargeAtlas : m_comAtlas;
		m_vipBtn.spriteName = firstPage ? "TeQuanBtn" : "btn_orange_219x74";
		m_vipBtnLabel.text = firstPage ? "" : "返回充值";
		m_rechargeObj.SetActive (firstPage);
		m_vipDesObj.SetActive (!firstPage);
		m_vipItemListObj.SetActive (!firstPage);
		if (firstPage)
		{
			m_leftBtn.SetActive (false);
			m_rightBtn.SetActive (false);
		}
	}

	#region RechargePage

	public VipInfoResp M_VipResp;

	public UIScrollView m_sc;
	public UIGrid m_grid;
	public UIScrollBar m_sb;

	public GameObject m_rechargeItem;
	private List<GameObject> m_rechargeItemList = new List<GameObject> ();

	public GameObject m_nextVipObj;
	public UISprite m_vipBtn;
	public UILabel m_vipBtnLabel;
	public UISprite m_v1;
	public UIProgressBar m_expPb;
	public UILabel m_expLabel;
	public UILabel m_need;
	public UISprite m_v2;

	public GameObject m_maxVipObj;

	public UILabel m_titleLabel;

	public GameObject m_leftBtn;
	public GameObject m_rightBtn;

	public void InItRechargePage (VipInfoResp tempResp)
	{
		M_VipResp = tempResp;

		UIYindao.m_UIYindao.CloseUI ();

		m_titleLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.V_TEQUAN_JINGYAN_2);

		InItVipInfo ();

		m_rechargeItemList = QXComData.CreateGameObjectList (m_rechargeItem,m_grid,tempResp.infos.Count,m_rechargeItemList);

		for (int i = 0;i < tempResp.infos.Count - 1;i ++)
		{
			for (int j = 0;j < tempResp.infos.Count - i - 1;j ++)
			{
				int id1 = tempResp.infos[j].id;
				int id2 = tempResp.infos[j + 1].id;
				bool yueOrZhou1 = id1 == 1 || id1 == 2 ? true : false;
				bool yueOrZhou2 = id2 == 1 || id2 == 2 ? true : false;

				if (yueOrZhou1 && yueOrZhou2)
				{
					int rank1 = ChongZhiTemplate.GetChongZhiTempById (tempResp.infos[j].id).rank;
					int rank2 = ChongZhiTemplate.GetChongZhiTempById (tempResp.infos[j + 1].id).rank;
					
					if (rank1 > rank2)
					{
						ChongTimes tempChong = tempResp.infos[j];
						tempResp.infos[j] = tempResp.infos[j + 1];
						tempResp.infos[j + 1] = tempChong;
					}
				}
				else
				{
					if (!yueOrZhou1 && yueOrZhou2)
					{
						ChongTimes tempChong = tempResp.infos[j];
						tempResp.infos[j] = tempResp.infos[j + 1];
						tempResp.infos[j + 1] = tempChong;
					}
					else if (!yueOrZhou1 && !yueOrZhou2)
					{
						bool haveBuy1 = tempResp.infos[j].times > 0 ? true : false;
						bool haveBuy2 = tempResp.infos[j + 1].times > 0 ? true : false;

						int rank1 = ChongZhiTemplate.GetChongZhiTempById (tempResp.infos[i].id).rank;
						int rank2 = ChongZhiTemplate.GetChongZhiTempById (tempResp.infos[i + 1].id).rank;

						if (haveBuy1 && haveBuy2)
						{
							if (rank1 > rank2)
							{
								ChongTimes tempChong = tempResp.infos[j];
								tempResp.infos[j] = tempResp.infos[j + 1];
								tempResp.infos[j + 1] = tempChong;
							}
						}
						else
						{
							if (haveBuy1 && !haveBuy2)
							{
								ChongTimes tempChong = tempResp.infos[j];
								tempResp.infos[j] = tempResp.infos[j + 1];
								tempResp.infos[j + 1] = tempChong;
							}
							else if (!haveBuy1 && !haveBuy2)
							{
								if (rank1 > rank2)
								{
									ChongTimes tempChong = tempResp.infos[j];
									tempResp.infos[j] = tempResp.infos[j + 1];
									tempResp.infos[j + 1] = tempChong;
								}
							}
						}
					}
				}
			}
		}

		for (int i = 0;i < m_rechargeItemList.Count;i ++)
		{
			RechargeItem recharge = m_rechargeItemList[i].GetComponent<RechargeItem> ();
			recharge.InItRechargeItem (tempResp.infos[i]);
		}

		m_sc.UpdateScrollbars (true);
	}

	void InItVipInfo ()
	{
		m_nextVipObj.SetActive (M_VipResp.isMax ? false : true);
		m_v1.spriteName = "v" + M_VipResp.vipLevel;
		m_maxVipObj.SetActive (M_VipResp.isMax);
		if (M_VipResp.isMax)
		{
			m_expPb.value = 1;
			m_expLabel.text = "";
		}
		else
		{
			m_v2.spriteName = "v" + (M_VipResp.vipLevel + 1);
			m_need.text = (M_VipResp.needYb - M_VipResp.hasYb).ToString();
			m_expLabel.text = M_VipResp.hasYb.ToString() + "/" + M_VipResp.needYb.ToString();
			m_expPb.value = M_VipResp.hasYb / float.Parse (M_VipResp.needYb.ToString ());
		}
	}

	#endregion

	#region ShowUpVip
	public GameObject m_mainCameraObj;

	public void ShowVip (int m_curVip)
	{
		GeneralVip.m_instance.ShowVip (m_curVip,m_mainCameraObj,RechargeData.Instance.RechargeBack);
	}
	#endregion

	#region VipPage
	public TriggerHandler m_triggerHandler;

	private List<VipTemplate> m_vipTemplateList = new List<VipTemplate>();

	public GameObject m_rewardItem;
	private List<GameObject> m_rewardList = new List<GameObject>();

	public GameObject m_cameraObj;

	private int m_curLevel = 1;

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

		m_vipTemplateList = vipTemplates;

		m_rewardList = QXComData.CreateGameObjectList (m_rewardItem,vipTemplates.Count,m_rewardList);

		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			m_rewardList[i].transform.localPosition = new Vector3(i * 880,0,0);
			RechargeVipItem vipItem = m_rewardList[i].GetComponent<RechargeVipItem> ();
			vipItem.InItVipItem (vipTemplates[i]);
		}

		m_cameraObj.transform.localPosition = m_rewardList [(M_VipResp.vipLevel == 0 ? 1 : M_VipResp.vipLevel) - 1].transform.localPosition;
	}

	public void ChangeVipTitle (int tempVip)
	{
		m_leftBtn.SetActive (tempVip == 1 ? false : true);
		m_rightBtn.SetActive (tempVip == m_rewardList.Count ? false : true);

		SparkleEffectItem leftSpark = m_leftBtn.GetComponent<SparkleEffectItem> ();
		SparkleEffectItem rightSpark = m_rightBtn.GetComponent<SparkleEffectItem> ();

		leftSpark.enabled = false;
		rightSpark.enabled = false;
//		Debug.Log ("QXComData.JunZhuInfo ().vipLv:" + QXComData.JunZhuInfo ().vipLv);
		for (int i = 0;i < m_vipTemplateList.Count;i ++)
		{
			if (m_vipTemplateList[i].lv <= QXComData.JunZhuInfo ().vipLv)
			{
				if (m_vipTemplateList[i].lv < tempVip)
				{
					if (!IsRewardGet (m_vipTemplateList[i].lv))
					{
//						Debug.Log ("m_vipTemplateList[i].lv:" + m_vipTemplateList[i].lv);
						leftSpark.enabled = true;
					}
				}
				else if (m_vipTemplateList[i].lv > tempVip)
				{
					if (!IsRewardGet (m_vipTemplateList[i].lv))
					{
//						Debug.Log ("m_vipTemplateList[i].lv:" + m_vipTemplateList[i].lv);
						rightSpark.enabled = true;
					}
				}
			}
		}
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
				vipItem.SetBtn ();
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
		return M_VipResp.getRewardVipList.Contains (tempVip);
	}

	void OnTriggerEnterCallBack (Collider col)
	{
//		Debug.Log ("col.name:" + col.name);
		
		RechargeVipItem vipItem = col.GetComponent<RechargeVipItem> ();
		if (vipItem)
		{
			m_curLevel = vipItem.M_VipTemp.lv;
			ChangeVipTitle (vipItem.M_VipTemp.lv);
		}
	}

	public void MoveToCenter (Vector3 pos,float time = 0.8f)
	{
//		SpringPosition.Begin(m_cameraObj, pos, 13f);
//		TweenPosition.Begin (m_cameraObj,0.5f,pos + new Vector3(0,1000,0));

		SpringPosition sp = m_cameraObj.GetComponent<SpringPosition> ();
		if (sp != null)
		{
			sp.enabled = false;
		}

		Hashtable move = new Hashtable ();
		move.Add ("position",pos);
		move.Add ("time",time);
		move.Add ("easetype",iTween.EaseType.easeOutQuart);
		move.Add ("islocal",true);
		iTween.MoveTo (m_cameraObj,move);
	}

	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "VipBtn":
		
//			ShowVip ();
//			return;

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
			if (!IsRewardGet (vipItem.M_VipTemp.lv))
			{
				RechargeData.Instance.GetVipRewardReq (vipItem.M_VipTemp.lv,vipItem.M_RewardDataList);
			}

			break;

		case "LeftBtn":
		{
			Vector3 curPos = m_cameraObj.transform.localPosition;
			float disX = curPos.x - m_rewardList[m_curLevel - 1].transform.localPosition.x;
			if (m_curLevel > 1)
			{
				if (disX > 20)
				{
					if (m_curLevel == m_rewardList.Count)
					{
						MoveToCenter (m_rewardList[m_curLevel - 2].transform.localPosition);
					}
					else
					{
						MoveToCenter (m_rewardList[m_curLevel - 1].transform.localPosition);
					}
				}
				else
				{
					MoveToCenter (m_rewardList[m_curLevel - 2].transform.localPosition);
				}
			}
			else
			{
				MoveToCenter (m_rewardList[0].transform.localPosition);
			}

			break;
		}
		case "RightBtn":
		{
			Vector3 curPos = m_cameraObj.transform.localPosition;
			float disX = m_rewardList[m_curLevel - 1].transform.localPosition.x - curPos.x;
			if (m_curLevel < m_rewardList.Count)
			{
				if (disX > 20)
				{
					if (m_curLevel == 1)
					{
						MoveToCenter (m_rewardList[m_curLevel].transform.localPosition);
					}
					else
					{
						MoveToCenter (m_rewardList[m_curLevel - 1].transform.localPosition);
					}
				}
				else
				{
					MoveToCenter (m_rewardList[m_curLevel].transform.localPosition);
				}
			}
			else
			{
				MoveToCenter (m_rewardList[m_curLevel - 1].transform.localPosition);
			}

			break;
		}
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
				if (recharge.M_ChongTimes.id == (int)RechargeData.RechargeType.MONTH_CARD)//月卡
				{
//					string text = "当前月卡剩余" + MyColorData.getColorString (4,M_VipResp.yueKaLeftDays) + "天（充值可累计）\n是否购买？";
					if (M_VipResp.yueKaLeftDays > 0)
					{
						string text = LanguageTemplate.GetText (LanguageTemplate.Text.RECHARGE_TIPS_1).Replace ("X",MyColorData.getColorString (4,M_VipResp.yueKaLeftDays.ToString ()));
						QXComData.CreateBoxDiy (text,false,YueKaBuyCallBack);
					}
					else
					{
						YueKaBuyCallBack (2);
					}
				}
				else if (recharge.M_ChongTimes.id == (int)RechargeData.RechargeType.WEEK_CARD)//周卡
				{
					if (M_VipResp.zhouKaLeftDays > 0)
					{
						string text = LanguageTemplate.GetText (LanguageTemplate.Text.RECHARGE_TIPS_6).Replace ("X",MyColorData.getColorString (4,M_VipResp.zhouKaLeftDays.ToString ()));
						QXComData.CreateBoxDiy (text,false,ZhouKaBuyCallBack);
					}
					else
					{
						ZhouKaBuyCallBack (2);
					}
				}
				else
				{
					ChongZhiTemplate temp = ChongZhiTemplate.GetChongZhiTempById (recharge.M_ChongTimes.id);
					RechargeData.RechargeType type = (RechargeData.RechargeType)Enum.ToObject (typeof (RechargeData.RechargeType),recharge.M_ChongTimes.id);

					if (ThirdPlatform.IsMyAppAndroidPlatform())
					{
						RechargeData.Instance.m_rechargeType = type;
						RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,temp.id,temp.addNum);
						Bonjour.BuyYuanBao (temp.addNum);
					}
					else
					{
						RechargeData.Instance.RechargeReq (type,temp.needNum);
					}
				}
			}

			break;
		}
	}

	void YueKaBuyCallBack (int i)
	{
		if (i == 2)
		{
			ChongZhiTemplate temp = ChongZhiTemplate.GetChongZhiTempById ((int)RechargeData.RechargeType.MONTH_CARD);
			RechargeData.RechargeType type = (RechargeData.RechargeType)Enum.ToObject (typeof (RechargeData.RechargeType),temp.id);

			if (ThirdPlatform.IsMyAppAndroidPlatform())
			{
				RechargeData.Instance.m_rechargeType = type;
				RechargeData.Instance.m_times = M_VipResp.yueKaLeftDays;
				RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,temp.id,temp.addNum);
				Bonjour.BuyYuanBao (temp.addNum);
			}
			else
			{
				RechargeData.Instance.RechargeReq (type,temp.addNum,M_VipResp.yueKaLeftDays);
			}
		}
	}

	void ZhouKaBuyCallBack (int i)
	{
		if (i == 2)
		{
			ChongZhiTemplate temp = ChongZhiTemplate.GetChongZhiTempById ((int)RechargeData.RechargeType.WEEK_CARD);
			RechargeData.RechargeType type = (RechargeData.RechargeType)Enum.ToObject (typeof (RechargeData.RechargeType),temp.id);
		
			if (ThirdPlatform.IsMyAppAndroidPlatform())
			{
				RechargeData.Instance.m_rechargeType = type;
				RechargeData.Instance.m_times = M_VipResp.zhouKaLeftDays;
				RechargeData.Instance.RechargeReport (RechargeData.ReportState.BEFORE,temp.id,temp.addNum);
				Bonjour.BuyYuanBao (temp.addNum);
			}
			else
			{
				RechargeData.Instance.RechargeReq (type,temp.needNum,M_VipResp.zhouKaLeftDays);
			}
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
