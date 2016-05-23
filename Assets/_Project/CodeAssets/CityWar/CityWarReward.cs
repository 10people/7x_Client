using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarReward : GeneralInstance<CityWarReward> {

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
		m_rewardList = QXComData.CreateGameObjectList (m_rewardObj,tempResp.rewardList.Count,m_rewardList);	

		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			m_rewardList[i].transform.localPosition = new Vector3(0,-65 * i,0);
			m_sc.UpdateScrollbars (true);
			CWRewardItem cwRewardItem = m_rewardList[i].GetComponent<CWRewardItem> ();
			cwRewardItem.InItReward (tempType,tempResp.rewardList[i]);
		}

		m_desLabe.text = m_rewardList.Count > 0 ? "" : 
			(tempType == CityWarData.CW_RewardType.ALLIANCE ? "目前没有记录，根据相关城池每日战况，\n所有盟友均可获得相关奖励" : "暂无任何奖励可领取，\n进入战场积极参与郡城战可获得丰厚奖励");

		m_rulesLabel.text = QXComData.yellow + (tempType == CityWarData.CW_RewardType.ALLIANCE ? "每日于18:00（未被宣战）与21:00（战斗结算）发放奖励，最多存3天" : "每日21：00开始参加与任意郡城战的进攻/镇守，都可获得奖励，最多存3天") + "[-]";

		m_sc.enabled = m_rewardList.Count < 8 ? false : true;
		m_sb.gameObject.SetActive (m_rewardList.Count < 8 ? false : true);

		switch (tempType)
		{
		case CityWarData.CW_RewardType.ALLIANCE:
			
			InItBtnState (m_allianceBtn,true);
			InItBtnState (m_personalBtn,false);
			
			break;
		case CityWarData.CW_RewardType.PERSONAL:
			
			InItBtnState (m_allianceBtn,false);
			InItBtnState (m_personalBtn,true);
			
			break;
		default:
			break;
		}
	}

	void InItBtnState (GameObject obj,bool light)
	{
		UIWidget[] widgets = obj.GetComponentsInChildren<UIWidget> ();
		foreach (UIWidget widget in widgets)
		{
			widget.color = light ? Color.white : Color.grey;
		}
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
				cwRecord.InItReward (m_rewardType,cwRecord.M_RewardInfo);
				break;
			}
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
