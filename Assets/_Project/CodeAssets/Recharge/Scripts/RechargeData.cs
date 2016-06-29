using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RechargeData : Singleton<RechargeData>,SocketProcessor {

	public enum RechargeType
	{
		WEEK_CARD = 2,		//周卡
		MONTH_CARD = 1,	//月卡
		YB_60 = 3,				//60元宝
		YB_300 = 4,				//300元宝
		YB_500 = 5,				//500元宝
		YB_980 = 6,				//980元宝
		YB_1980 = 7,			//1980元宝
		YB_3280 = 8,			//3280元宝
		YB_6480 = 9,			//6480元宝
	}
	public RechargeType m_rechargeType;

	private VipInfoResp m_vipResp;

	private GameObject m_rechargeRoot;

	private bool m_isOpenRecharge = false;

	public bool M_IsRechargeEnd = false;

	private int m_lastVip;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	#region RechargeInfo Req
	public void RechargeDataReq ()
	{
		m_isOpenRecharge = true;
		Recharge ();
	}

	void Recharge ()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_VIPINFO_REQ,ProtoIndexes.S_VIPINFO_RESP.ToString ());
//		Debug.Log ("充值首页请求：" + ProtoIndexes.C_VIPINFO_REQ);
	}
	#endregion

	#region Recharge Req
	[HideInInspector]
	public int m_rechargeNum;
	public int m_times;

	public void RechargeReq (RechargeType tempType,int tempAmount,int tempTimes = 0)
	{
		m_rechargeType = tempType;
		m_rechargeNum = tempAmount;
		m_times = tempTimes;

		RechargeReq rechargeReq = new RechargeReq ();
		rechargeReq.type = (int)tempType;
		rechargeReq.amount = tempAmount;
		QXComData.SendQxProtoMessage (rechargeReq,ProtoIndexes.C_RECHARGE_REQ,ProtoIndexes.S_RECHARGE_RESP.ToString ());
//		Debug.Log ("充值请求：" + ProtoIndexes.S_RECHARGE_RESP);
	}
	#endregion

	#region Get Vip Reward
	private int m_getRewardId;
	private List<RewardData> m_rewardDataList = new List<RewardData> ();

	public void GetVipRewardReq (int tempLevel,List<RewardData> tempDataList)
	{
		m_getRewardId = tempLevel;
		m_rewardDataList = tempDataList;

		GetVipRewardReq rewardReq = new GetVipRewardReq ();
		rewardReq.vipLevel = tempLevel;
		QXComData.SendQxProtoMessage (rewardReq,ProtoIndexes.C_VIP_GET_GIFTBAG_REQ,ProtoIndexes.S_VIP_GET_GIFTBAG_RESP.ToString ());
//		Debug.Log ("领奖请求：" + ProtoIndexes.C_VIP_GET_GIFTBAG_REQ);
	}
	#endregion

	#region Recharge Report
	public enum ReportState
	{
		BEFORE = 20,		//弹sdk前
		AFTER_SUCCESS = 40,		//sdk充值结束后，success
		CANCEL = 50,		//取消
		FAIL = 60,		//失败
		DEFAULT = 404,
	}
	public ReportState M_ReportState;

	public ErrorMessage M_ErrorMsg;

	public void RechargeReport (ReportState tempState,int tempId,int tempNum)
	{
		M_ReportState = tempState;

		ErrorMessage errorMsg = new ErrorMessage ();
		errorMsg.cmd = (int)tempState;				
		errorMsg.errorDesc = tempId.ToString ();	// 404
		errorMsg.errorCode = tempNum;

		M_ErrorMsg = errorMsg;

//		QXComData.SendQxProtoMessage (errorMsg,ProtoIndexes.C_CHECK_CHARGE,tempState == ReportState.AFTER_SUCCESS ? ProtoIndexes.CHARGE_OK.ToString () : "");
		QXComData.SendQxProtoMessage (errorMsg,ProtoIndexes.C_CHECK_CHARGE,null);
//		Debug.Log ("充值状态汇报：" + ProtoIndexes.C_CHECK_CHARGE);
	}
	#endregion

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_VIPINFO_RESP:
			{
//				Debug.Log ("充值首页返回：" + ProtoIndexes.S_VIPINFO_RESP);
				VipInfoResp vipResp = new VipInfoResp();
				vipResp = QXComData.ReceiveQxProtoMessage (p_message,vipResp) as VipInfoResp;

				if (vipResp != null)
				{
					if (vipResp.infos == null)
					{
						vipResp.infos = new List<ChongTimes>();
					}
					if (vipResp.getRewardVipList == null)
					{
						vipResp.getRewardVipList = new List<int>();
					}
//					Debug.Log ("vip等级：" + vipResp.vipLevel);
//					Debug.Log ("需要元宝数：" + vipResp.needYb);
//					Debug.Log ("拥有元宝数：" + vipResp.hasYb);
//					Debug.Log ("是否达到最高级：" + vipResp.isMax);
//					Debug.Log ("zhouka：" + vipResp.zhouKaLeftDays);
//					Debug.Log ("yueka：" + vipResp.yueKaLeftDays);

					m_vipResp = vipResp;

					if (m_isOpenRecharge)
					{
						LoadRechargeRoot ();
						if (ThirdPlatform.IsMyAppAndroidPlatform () && M_IsRechargeEnd)
						{
							if (m_lastVip < m_vipResp.vipLevel)
							{
								m_lastVip = m_vipResp.vipLevel;
								RechargePage.m_instance.ShowVip (m_vipResp.vipLevel);
							}
							else
							{
								RechargeBack ();
							}
						}
					}
				}

				return true;
			}
			case ProtoIndexes.S_RECHARGE_RESP:
			{
//				Debug.Log ("返回充值信息：" + ProtoIndexes.S_RECHARGE_RESP);
				RechargeResp rechargeResp = new RechargeResp();
				rechargeResp = QXComData.ReceiveQxProtoMessage (p_message,rechargeResp) as RechargeResp;

				if (rechargeResp != null)
				{
//					Debug.Log ("充值结果：" + rechargeResp.isSuccess);
//					Debug.Log ("增加元宝数：" + rechargeResp.sumAoumnt);
//					Debug.Log ("vip等级：" + rechargeResp.vipLevel);
//					Debug.Log ("月卡剩余天数：" + rechargeResp.yueKaLeftDays);
//					Debug.Log ("msg：" + rechargeResp.msg);

					if (m_isOpenRecharge && !ThirdPlatform.IsMyAppAndroidPlatform ())
					{
						if (rechargeResp.vipLevel > m_vipResp.vipLevel)
						{
							RechargePage.m_instance.ShowVip (rechargeResp.vipLevel);
						}
						else
						{
							RechargeBack ();
						}
						Recharge ();
					}
				}

				return true;
			}
			case ProtoIndexes.S_VIP_GET_GIFTBAG_RESP:
			{
//				Debug.Log ("领奖返回：" + ProtoIndexes.S_VIP_GET_GIFTBAG_RESP);
				GetVipRewardResp rewardResp = new GetVipRewardResp();
				rewardResp = QXComData.ReceiveQxProtoMessage (p_message,rewardResp) as GetVipRewardResp;

				if (rewardResp != null)
				{
//					Debug.Log ("rewardResp.result:" + rewardResp.result);

					if (rewardResp.result == 0 || rewardResp.result == 1)
					{
						RechargePage.m_instance.RefreshVipReward (m_getRewardId);

						if (rewardResp.result == 0)
						{
							GeneralRewardManager.Instance ().CreateReward (m_rewardDataList);
						}
					}

					switch (rewardResp.result)
					{
					case 0://成功
						break;
					case 1://奖励已领取
						break;
					case 2://VIP等级不足
						break;
					default:
						break;
					}
				}

				return true;
			}
			case ProtoIndexes.CHARGE_OK:
			{
//				Debug.Log ("充值成功");
				if (m_isOpenRecharge)
				{
					m_lastVip = m_vipResp.vipLevel;
					M_IsRechargeEnd = true;
					Recharge ();
				}
				return true;
			}
			}
		}
		return false;
	}

	public void RechargeBack ()
	{
		string text = "";
		switch (m_rechargeType)
		{
		case RechargeType.MONTH_CARD://月卡
			text = LanguageTemplate.GetText (LanguageTemplate.Text.RECHARGE_TIPS_5).Replace ("a",MyColorData.getColorString (4,CanshuTemplate.GetStrValueByKey ("YUEKA_TIME")));
			text = text.Replace ("N",MyColorData.getColorString (4,m_times.ToString ()));
			break;
		case RechargeType.WEEK_CARD://周卡
			text = LanguageTemplate.GetText (LanguageTemplate.Text.RECHARGE_TIPS_7).Replace ("a",MyColorData.getColorString (4,CanshuTemplate.GetStrValueByKey ("ZHOUKA_TIME")));
			text = text.Replace ("N",MyColorData.getColorString (4,m_times.ToString ()));
			break;
		default:
			text = LanguageTemplate.GetText (LanguageTemplate.Text.RECHARGE_TIPS_4);
			break;
		}
		QXComData.CreateBoxDiy (text,true,RechargeCallBack);
	}

	void RechargeCallBack (int i)
	{
		M_IsRechargeEnd = false;
	}

	void LoadRechargeRoot ()
	{
		if (m_rechargeRoot == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.RECHARGE_ROOT ),LoadRechargeRootCallBack );
		}
		else
		{
			m_rechargeRoot.SetActive (true);
			InItRechargePage ();
		}
	}

	void LoadRechargeRootCallBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		m_rechargeRoot = Instantiate (p_object) as GameObject;
		InItRechargePage ();
	}

	void InItRechargePage ()
	{
		MainCityUI.TryAddToObjectList (m_rechargeRoot);
		TreasureCityUI.TryAddToObjectList (m_rechargeRoot);
		RechargePage.m_instance.ChangePage (true);
		RechargePage.m_instance.InItRechargePage (m_vipResp);
		RechargePage.m_instance.M_RechargeDelegate = RechargeDelegateCallBack;
	}

	void RechargeDelegateCallBack ()
	{
		m_isOpenRecharge = false;
		M_IsRechargeEnd = false;
		MainCityUI.TryRemoveFromObjectList (m_rechargeRoot);
		TreasureCityUI.TryRemoveFromObjectList (m_rechargeRoot);
		m_rechargeRoot.SetActive (false);
	}

	void OnDestroy ()
	{
		M_IsRechargeEnd = false;
		m_isOpenRecharge = false;
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
