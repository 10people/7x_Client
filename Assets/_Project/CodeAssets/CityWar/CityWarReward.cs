using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarReward : GeneralInstance<CityWarReward> {

	private CityWarRewardResp m_rewardResp;

	public delegate void CityWarRewardDelegate ();
	public CityWarRewardDelegate M_CWRewardDelegate;

	private CityWarData.CW_RewardType m_rewardType;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;

	public GameObject m_rewardObj;
	private List<GameObject> m_rewardList = new List<GameObject>();

	public UILabel m_rulesLabel;
	public UILabel m_desLabe;

	public GameObject m_allianceBtn;
	public GameObject m_personalBtn;

	public GameObject m_aRed;
	public GameObject m_pRed;

	public readonly Dictionary<int,string[]> M_RewardDic = new Dictionary<int, string[]>
	{
		{0,new string[]{"[d80202]失败[-]","[00e1c4]镇守[-]","今天"}}, //string[] （0-result  1-wartype  2-time）
		{1,new string[]{"[10ff2b]成功[-]","[d80202]进攻[-]","昨天"}},
		{2,new string[]{"","[e5e205]未发生战斗[-]","前天"}},
	};

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItCityWarReward (CityWarData.CW_RewardType tempType,CityWarRewardResp tempResp)
	{
		m_rewardType = tempType;

		m_rewardResp = tempResp;

		m_sb.value = 0;
		m_sc.UpdateScrollbars (true);
		m_sc.ResetPosition ();

		m_rewardList = QXComData.CreateGameObjectList (m_rewardObj,tempResp.rewardList.Count,m_rewardList);	

		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			m_rewardList[i].transform.localPosition = new Vector3(0,-48 * i,0);
			m_sc.UpdateScrollbars (true);
			CWRewardItem cwRewardItem = m_rewardList[i].GetComponent<CWRewardItem> ();
			cwRewardItem.InItReward (tempType,tempResp.rewardList[i]);
		}

//		m_desLabe.text = m_rewardList.Count > 0 ? "" : 
//			(tempType == CityWarData.CW_RewardType.ALLIANCE ? "目前没有记录，根据相关城池每日战况，\n所有盟友均可获得相关奖励" : "暂无任何奖励可领取，\n进入战场积极参与郡城战可获得丰厚奖励");

		m_desLabe.text = m_rewardList.Count > 0 ? "" : 
			(tempType == CityWarData.CW_RewardType.ALLIANCE ? LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_41) : LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_42));

//		m_rulesLabel.text = QXComData.yellow + (tempType == CityWarData.CW_RewardType.ALLIANCE ? "每日于18:00（未被宣战）与21:00（战斗结算）发放奖励，最多存3天" : "每日21：00开始参加与任意郡城战的进攻/镇守，都可获得奖励，最多存3天") + "[-]";
		m_rulesLabel.text = QXComData.yellow + (tempType == CityWarData.CW_RewardType.ALLIANCE ? LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_7) : LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_8)) + "[-]";

		m_sc.enabled = m_rewardList.Count < 7 ? false : true;
		m_sb.gameObject.SetActive (m_rewardList.Count < 7 ? false : true);

		switch (tempType)
		{
		case CityWarData.CW_RewardType.ALLIANCE:
			
			QXComData.SetBtnState (m_allianceBtn,true);
			QXComData.SetBtnState (m_personalBtn,false);
			
			break;
		case CityWarData.CW_RewardType.PERSONAL:
			
			QXComData.SetBtnState (m_allianceBtn,false);
			QXComData.SetBtnState (m_personalBtn,true);
			
			break;
		default:
			break;
		}

		//310410  310420
		m_aRed.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (310410));
		m_pRed.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (310420));
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "AllianceBtn":

			if (m_rewardType != CityWarData.CW_RewardType.ALLIANCE)
			{
				CityWarData.Instance.RewardReq (CityWarData.CW_RewardType.ALLIANCE);
			}

			break;
		case "PersonalBtn":

			if (m_rewardType != CityWarData.CW_RewardType.PERSONAL)
			{
				CityWarData.Instance.RewardReq (CityWarData.CW_RewardType.PERSONAL);
			}

			break;
		case "ZheZhao":

			if (M_CWRewardDelegate != null)
			{
				M_CWRewardDelegate ();
			}

			break;
		case "GetBtn":

			CWRewardItem cwReward = ui.transform.parent.GetComponent<CWRewardItem> ();
			if (cwReward.M_RewardInfo != null)
			{
				//发送领取请求
				CityWarOperateReq operate = new CityWarOperateReq();
				operate.operateType = CityOperateType.GET_REWARD;
				operate.rewardId = (int)cwReward.M_RewardInfo.id;
				CityWarData.Instance.CityOperate (operate,cwReward.M_RewardInfo.rewardNum);
			}

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Refreshs the reward.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	public void RefreshReward (long tempId)
	{
		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			CWRewardItem cwRecord = m_rewardList[i].GetComponent<CWRewardItem> ();
			if (cwRecord.M_RewardInfo.id == tempId)
			{
				cwRecord.M_RewardInfo.getState = 1;
				m_rewardResp.rewardList[i].getState = 1;
				cwRecord.InItReward (m_rewardType,cwRecord.M_RewardInfo);
				break;
			}
		}

		bool isRed = false;
		for (int i = 0;i < m_rewardResp.rewardList.Count;i ++)
		{
			if (m_rewardResp.rewardList[i].getState != 1)
			{
				isRed = true;
				break;
			}
			else
			{
				isRed = false;
			}
		}

		if (m_rewardType == CityWarData.CW_RewardType.ALLIANCE)
		{
			m_aRed.SetActive (isRed);
			PushAndNotificationHelper.SetRedSpotNotification (310410,isRed);
		}
		else
		{
			m_pRed.SetActive (isRed);
			PushAndNotificationHelper.SetRedSpotNotification (310420,isRed);
		}

		CityWarPage.m_instance.SetRewardRed ();
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
