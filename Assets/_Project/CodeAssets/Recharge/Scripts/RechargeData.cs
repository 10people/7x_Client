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

	private VipInfoResp m_vipResp;

	private GameObject m_rechargeRoot;

	private int m_rechargeId;
	private int m_rechargeNum;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	#region RechargeInfo Req
	public void RechargeDataReq ()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_VIPINFO_REQ,ProtoIndexes.S_VIPINFO_RESP.ToString ());
		Debug.Log ("充值首页请求：" + ProtoIndexes.C_VIPINFO_REQ);
	}
	#endregion

	#region Recharge Req
	public void RechargeReq (int tempId,int tempAmount)
	{
		m_rechargeId = tempId;
		RechargeReq rechargeReq = new RechargeReq ();
		rechargeReq.type = tempId;
		rechargeReq.amount = tempAmount;
		QXComData.SendQxProtoMessage (rechargeReq,ProtoIndexes.C_RECHARGE_REQ,ProtoIndexes.S_RECHARGE_RESP.ToString ());
		Debug.Log ("充值请求：" + ProtoIndexes.S_RECHARGE_RESP);
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
		Debug.Log ("领奖请求：" + ProtoIndexes.C_VIP_GET_GIFTBAG_REQ);
	}
	#endregion

	#region Recharge Report
	public enum ReportState
	{
		BEFORE = 20,		//弹sdk前
		AFTER_SUCCESS = 40,		//sdk充值结束后，success
		CANCEL = 50,		//取消
		FAIL = 60,		//失败
	}
	public ReportState M_ReportState;

	public ErrorMessage M_ErrorMsg;

	public void RechargeReport (ReportState tempState,int tempId,int tempNum)
	{
		M_ReportState = tempState;

		ErrorMessage errorMsg = new ErrorMessage ();
		errorMsg.cmd = (int)tempState;
		errorMsg.errorDesc = tempId.ToString ();
		errorMsg.errorCode = tempNum;

		M_ErrorMsg = errorMsg;

		QXComData.SendQxProtoMessage (errorMsg,ProtoIndexes.C_CHECK_CHARGE);
		Debug.Log ("充值状态汇报：" + ProtoIndexes.C_CHECK_CHARGE);
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
				Debug.Log ("充值首页返回：" + ProtoIndexes.S_VIPINFO_RESP);
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
					Debug.Log ("vip等级：" + vipResp.vipLevel);
					Debug.Log ("需要元宝数：" + vipResp.needYb);
					Debug.Log ("拥有元宝数：" + vipResp.hasYb);
					Debug.Log ("是否达到最高级：" + vipResp.isMax);

					m_vipResp = vipResp;

					LoadRechargeRoot ();
				}

				return true;
			}
			case ProtoIndexes.S_RECHARGE_RESP:
			{
				Debug.Log ("返回充值信息：" + ProtoIndexes.S_RECHARGE_RESP);
				RechargeResp rechargeResp = new RechargeResp();
				rechargeResp = QXComData.ReceiveQxProtoMessage (p_message,rechargeResp) as RechargeResp;

				if (rechargeResp != null)
				{
					Debug.Log ("充值结果：" + rechargeResp.isSuccess);
					Debug.Log ("增加元宝数：" + rechargeResp.sumAoumnt);
					Debug.Log ("vip等级：" + rechargeResp.vipLevel);
//					Debug.Log ("月卡剩余天数：" + rechargeResp.yueKaLeftDays);
//					Debug.Log ("msg：" + rechargeResp.msg);

					RechargeDataReq ();
					if (m_rechargeId == 0)
					{
						QXComData.CreateBoxDiy ("购买月卡成功！",true,null);
					}
					else if (m_rechargeId == 1)
					{
						QXComData.CreateBoxDiy ("购买终生卡成功！",true,null);
					}
					else
					{
						QXComData.CreateBoxDiy ("充值成功！",true,null);
					}
				}

				return true;
			}
			case ProtoIndexes.S_VIP_GET_GIFTBAG_RESP:
			{
				Debug.Log ("领奖返回：" + ProtoIndexes.S_VIP_GET_GIFTBAG_RESP);
				GetVipRewardResp rewardResp = new GetVipRewardResp();
				rewardResp = QXComData.ReceiveQxProtoMessage (p_message,rewardResp) as GetVipRewardResp;

				if (rewardResp != null)
				{
					Debug.Log ("rewardResp.result:" + rewardResp.result);

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
			}
		}
		return false;
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
		RechargePage.m_instance.ChangePage (true);
		RechargePage.m_instance.InItRechargePage (m_vipResp);
		RechargePage.m_instance.M_RechargeDelegate = RechargeDelegateCallBack;
	}

	void RechargeDelegateCallBack ()
	{
		MainCityUI.TryRemoveFromObjectList (m_rechargeRoot);
		m_rechargeRoot.SetActive (false);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
