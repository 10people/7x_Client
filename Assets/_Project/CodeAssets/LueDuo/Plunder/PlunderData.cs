using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderData : Singleton<PlunderData>,SocketProcessor {

	public enum NextPageReqType
	{
		ALLIANCE = 1,
		JUNZHU = 2,
	}
	private NextPageReqType nextPageType;//下个页面请求类型

	public enum OperateType
	{
		CLEAR_CD = 1,
		ADD_NUM = 2,
		GET_REWARD = 3,
	}
	private OperateType operateType;//掠夺操作类型

	public enum Entrance
	{
		PLUNDER,//掠夺
		RANKLIST,//排行
		CHAT,//聊天
	}
	private Entrance entrance;

	public enum PlunderRankType
	{
		PERSONAL_RANK = 5,
		ALLIANCE_RANK = 6,
	}
	private PlunderRankType rankType;//掠夺排行类型
	public enum RankSerchType
	{
		DEFAULT,
		SELF_POS,
	}
	private RankSerchType serchType;//掠夺排行查找类型

	private LveDuoInfoResp plunderResp;

	private int pdAlliancePage;//联盟页
	public int PdAlliancePage { get{return pdAlliancePage;} set{pdAlliancePage = value;} }

	private int pRankPage;//个人排行页
	public int PRankPage { get{return pRankPage;} set{pRankPage = value;} }

	private int aRankPage;//联盟排行页
	public int ARankPage { get{return aRankPage;} set{aRankPage = value;} }

	private List<RewardData> rewardDataList;//每日排名结算奖励
	public List<RewardData> RewardDataList { set{rewardDataList = value;} get{return rewardDataList;} }

	private GameObject plunderObj;

	private LveGoLveDuoResp plunderOpResp;

	private string textStr;

	public bool isOpenLueDuo = false;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	public LveDuoInfoResp PlunderDataResp ()
	{
		return plunderResp;
	}

	/// <summary>
	/// Opens the plunder.
	/// </summary>
	public void OpenPlunder ()
	{
		if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			//无联盟
			ClientMain.m_UITextManager.createText("去加入一个联盟再来掠夺对手吧！");
			return;
		}
		else if (JunZhuData.Instance().m_junzhuInfo.level < FunctionOpenTemp.GetTemplateById (211).Level)
		{
			//未到掠夺开启等级
			ClientMain.m_UITextManager.createText("[dc0600]" + FunctionOpenTemp.GetTemplateById (211).Level + "[-]级开启掠夺！");
			return;
		}

		isOpenLueDuo = true;
		PlunderDataReq ();
	}

	/// <summary>
	/// Plunders the data req.
	/// </summary>
	public void PlunderDataReq ()
	{
		if (JunZhuData.Instance ().m_junzhuInfo.lianMengId > 0 && JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById (211).Level) 
		{
			QXComData.SendQxProtoMessage (ProtoIndexes.LVE_DUO_INFO_REQ,ProtoIndexes.LVE_DUO_INFO_RESP.ToString ());
			//		Debug.Log ("掠夺信息请求:" + ProtoIndexes.LVE_DUO_INFO_REQ);
		}
	}

	/// <summary>
	/// Nexts the page req.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempId">Temp identifier.</param>
	public void NextPageReq (NextPageReqType tempType,int tempId)
	{
		nextPageType = tempType;

		LveNextItemReq nextPageReq = new LveNextItemReq();
		nextPageReq.rankType = (int)tempType;
		switch (tempType)
		{
		case NextPageReqType.ALLIANCE:
			nextPageReq.guojiaId = tempId;
			nextPageReq.pageNo = pdAlliancePage;
			break;
		case NextPageReqType.JUNZHU:
			nextPageReq.mengId = tempId;
			break;
		default:
			break;
		}
		QXComData.SendQxProtoMessage (nextPageReq,ProtoIndexes.LVE_NEXT_ITEM_REQ,ProtoIndexes.LVE_NEXT_ITEM_RESP.ToString ());
//		Debug.Log ("下个掠夺页面请求:" + ProtoIndexes.LVE_NEXT_ITEM_REQ);
	}

	/// <summary>
	/// Plunders the operate.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void PlunderOperate (OperateType tempType)
	{
		operateType = tempType;

		LveConfirmReq confirmReq = new LveConfirmReq();
		confirmReq.doType = (int)tempType;
		
		QXComData.SendQxProtoMessage (confirmReq,ProtoIndexes.LVE_CONFIRM_REQ,ProtoIndexes.LVE_CONFIRM_RESP.ToString ());
//		Debug.Log ("清除cd或增加次数请求:" + ProtoIndexes.LVE_CONFIRM_REQ);
	}

	/// <summary>
	/// Plunders the opponent.
	/// </summary>
	/// <param name="enterance">Enterance.</param>
	/// <param name="tempJunZhuId">Temp jun zhu identifier.</param>
	public void PlunderOpponent (Entrance tempEntrance,long tempJunZhuId)
	{
		tempEntrance = entrance;

		LveGoLveDuoReq plunderReq = new LveGoLveDuoReq ();
		plunderReq.enemyId = tempJunZhuId;
		QXComData.SendQxProtoMessage (plunderReq,ProtoIndexes.LVE_GO_LVE_DUO_REQ,ProtoIndexes.LVE_GO_LVE_DUO_RESP.ToString ());
		//			Debug.Log ("掠夺对手请求:" + ProtoIndexes.LVE_GO_LVE_DUO_REQ);
	}

	/// <summary>
	/// Plunders the record req.
	/// </summary>
	public void PlunderRecordReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.LVE_BATTLE_RECORD_REQ,ProtoIndexes.LVE_BATTLE_RECORD_RESP.ToString ());
//		Debug.Log ("掠夺记录请求：" + ProtoIndexes.LVE_BATTLE_RECORD_REQ);
	}

	/// <summary>
	/// Plunders the rank list req.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempPage">Temp page.</param>
	public void PlunderRankListReq (PlunderRankType tempType,RankSerchType tempSerchType)
	{
		rankType = tempType;
		serchType = tempSerchType;
		Debug.Log (PRankPage + ":" + ARankPage);
		RankingReq rank = new RankingReq ();
		rank.rankType = (int)tempType;
		rank.pageNo = tempType == PlunderRankType.PERSONAL_RANK ? PRankPage : ARankPage;
		QXComData.SendQxProtoMessage (rank,ProtoIndexes.RANKING_REQ,ProtoIndexes.RANKING_RESP.ToString ());
		Debug.Log ("掠夺排行请求：" + ProtoIndexes.RANKING_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LVE_DUO_INFO_RESP:
			{
//				Debug.Log ("掠夺信息返回:" + ProtoIndexes.LVE_DUO_INFO_RESP);
				LveDuoInfoResp plunderDataRes = new LveDuoInfoResp();
				plunderDataRes = QXComData.ReceiveQxProtoMessage (p_message,plunderDataRes) as LveDuoInfoResp;
				
				if (plunderDataRes != null)
				{
//					Debug.Log ("已掠夺次数：" + plunderDataRes.used);
//					Debug.Log ("总掠夺次数：" + plunderDataRes.all);
//					Debug.Log ("是否有新的战斗记录：" + plunderDataRes.hasRecord );
//					Debug.Log ("掠夺cdTime：" + plunderDataRes.CdTime);
//					Debug.Log ("最大可掠夺次数：" + plunderDataRes.nowMaxBattleCount);
//					Debug.Log ("贡金：" + plunderDataRes.gongJin);
//					Debug.Log ("国家id：" + plunderDataRes.showGuoId);
//					Debug.Log ("可以清除冷却的vip等级：" + plunderDataRes.canClearCdVIP);
//					Debug.Log ("购买下次掠夺所花费的元宝数：" + plunderDataRes.buyNextBattleYB);
//					Debug.Log ("下次可购买的掠夺次数：" + plunderDataRes.buyNextBattleCount);
//					Debug.Log ("清除cd需要花费的元宝：" + plunderDataRes.clearCdYB);
					
					if (plunderDataRes.guoLianInfos == null)
					{
						plunderDataRes.guoLianInfos = new List<GuoInfo>();
					}
					if (plunderDataRes.junInfos == null)
					{
						plunderDataRes.junInfos = new List<JunZhuInfo>();
					}
					if (plunderDataRes.mengInfos == null)
					{
						plunderDataRes.mengInfos = new List<LianMengInfo>();
					}

					plunderResp = plunderDataRes;
					if (isOpenLueDuo)
					{
						LoadPlunderObj ();
					}
				}
				
				return true;
			}
			case ProtoIndexes.LVE_NEXT_ITEM_RESP:
			{
//				Debug.Log ("下个掠夺页面返回:" + ProtoIndexes.LVE_NEXT_ITEM_RESP);
				LveNextItemResp pNextPageRes = new LveNextItemResp();
				pNextPageRes = QXComData.ReceiveQxProtoMessage (p_message,pNextPageRes) as LveNextItemResp;
				
				if (pNextPageRes != null)
				{
					if (pNextPageRes.junList == null)
					{
						pNextPageRes.junList = new List<JunZhuInfo>();
					}
					if (pNextPageRes.mengList == null)
					{
						pNextPageRes.mengList = new List<LianMengInfo>();
					}
//					Debug.Log ("联盟个数：" + pNextPageRes.mengList.Count);
//					Debug.Log ("君主个数：" + pNextPageRes.junList.Count);

					switch (nextPageType)
					{
					case NextPageReqType.ALLIANCE:

						if (PdAlliancePage == 1)
						{
							PlunderPage.plunderPage.RefreshPlunderPage (nextPageType,pNextPageRes.junList,pNextPageRes.mengList);
						}
						else
						{
							if (pNextPageRes.mengList.Count > 0)
							{
								PlunderPage.plunderPage.RefreshPlunderPage (nextPageType,pNextPageRes.junList,pNextPageRes.mengList);
							}
							else
							{
								pdAlliancePage -= 1;
							}
						}

						break;
					case NextPageReqType.JUNZHU:

						PlunderPage.plunderPage.RefreshPlunderPage (nextPageType,pNextPageRes.junList);

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.LVE_CONFIRM_RESP:
			{
//				Debug.Log ("清除冷却或增加次数返回:" + ProtoIndexes.LVE_CONFIRM_RESP);
				LveConfirmResp confirmRes = new LveConfirmResp();
				confirmRes = QXComData.ReceiveQxProtoMessage (p_message,confirmRes) as LveConfirmResp;
				
				if (confirmRes != null)
				{
					switch (confirmRes.isOk)
					{
					case 0:

//						textStr = "元宝不足！\n是否跳转到充值？";
//						QXComData.CreateBoxDiy (textStr,false,TurnToVip);
						Global.CreateFunctionIcon (101);

						break;
					case 1:

						if (isOpenLueDuo)
						{
							PlunderPage.plunderPage.RefreshPlunderState (confirmRes);
						}

						break;
					case 2:

//						textStr = "您的VIP等级不够，\n达到VIP" + plunderResp.canClearCdVIP + "级可清除冷却！";
//						QXComData.CreateBoxDiy (textStr,true,null);
						Global.CreateFunctionIcon (1901);

						break;
					case 3:

						textStr = "购买失败！";
						QXComData.CreateBoxDiy (textStr,true,null);

						break;
					case 6:

						textStr = "购买失败！\n今日掠夺次数已用尽！";
						QXComData.CreateBoxDiy (textStr,true,null);
						
						break;
					case 7:

						textStr = "今天已领取过奖励！";

						QXComData.CreateBoxDiy (textStr,true,null);

						break;
					case 8:

//						Debug.Log ("领奖成功！");
						GeneralRewardManager.Instance().CreateReward (RewardDataList);
						if (isOpenLueDuo)
						{
							PlunderPage.plunderPage.RefreshPlunderState (confirmRes);
						}

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.LVE_GO_LVE_DUO_RESP:
			{
//				Debug.Log ("请求掠夺对手返回:" + ProtoIndexes.LVE_GO_LVE_DUO_RESP);
				LveGoLveDuoResp plunderOpRes = new LveGoLveDuoResp();
				plunderOpRes = QXComData.ReceiveQxProtoMessage (p_message,plunderOpRes) as LveGoLveDuoResp;
				
				if (plunderOpRes != null)
				{
//					Debug.Log ("请求掠夺对手结果：" + plunderOpRes.isCanLveDuo);
					plunderOpResp = plunderOpRes;
					if (plunderOpRes.isCanLveDuo == 7)
					{
						//跳转到对战阵容页面
						GeneralControl.Instance.OpenChallengePage (GeneralChallengePage.ChallengeType.PLUNDER,plunderOpRes);
					}
					else
					{
						switch (plunderOpRes.isCanLveDuo)
						{
						case -1:
							textStr = "该玩家是您本人，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 0:
							string openStr = CanshuTemplate.GetStrValueByKey (CanshuTemplate.OPENTIME_LUEDUO);
							string closeStr = CanshuTemplate.GetStrValueByKey (CanshuTemplate.CLOSETIME_LUEDUO);
							textStr = MyColorData.getColorString (1,"不在开启时段，无法掠夺！\n掠夺玩法开启时段：") 
									+ MyColorData.getColorString (5,openStr) 
									+ MyColorData.getColorString (1,"-") 
									+ MyColorData.getColorString (5,closeStr);
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 1:
							textStr = "该玩家是您的盟友，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 2:
							Debug.Log ("plunderResp.clearCdYB:" + plunderResp.clearCdYB);
							Debug.Log ("PurchaseTemplate.GetPurchaseTempById (65047).price:" + PurchaseTemplate.GetPurchaseTempById (65047).price);
							textStr = "掠夺冷却中！\n是否使用" + (plunderResp.clearCdYB == 0 ? PurchaseTemplate.GetPurchaseTempById (65047).price : plunderResp.clearCdYB) + "元宝清除冷却时间？";
							QXComData.CreateBoxDiy (textStr,false,ClearCdReqBack,true,0,plunderResp.canClearCdVIP);
//							if (JunZhuData.Instance().m_junzhuInfo.vipLv < plunderResp.canClearCdVIP)
//							{
//								textStr = "掠夺冷却中！\n达到VIP" + plunderResp.canClearCdVIP + "级可清除冷却！";
//								QXComData.CreateBoxDiy (textStr,true,null);
//							}
//							else
//							{
//								textStr = "掠夺冷却中！\n是否使用" + plunderResp.clearCdYB + "元宝清除冷却时间？";
//								QXComData.CreateBoxDiy (textStr,false,ClearCdReqBack);
//							}
							break;
						case 3:

							BuyPlunderTimes ();

							break;
						case 4:
							textStr = "这位玩家正处于保护期内，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 5:
							textStr = "该玩家正在被其他玩家掠夺，请更换目标！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 6:
							textStr = "该玩家为系统机器人，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 8:
							textStr = "该玩家暂未开启掠夺玩法，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 9:
							textStr = "您暂时没有联盟，请加入联盟后再试！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 10:
							textStr = "该玩家暂时没有联盟，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 11:
							textStr = "该玩家暂未开启联盟功能，无法掠夺！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						case 12:
							textStr = "该玩家正在掠夺他人，请更换目标！";
							QXComData.CreateBoxDiy (textStr,true,null);
							break;
						default:
							break;
						}
					}
				}
				
				return true;
			}
			case ProtoIndexes.LVE_NOTICE_CAN_LVE_DUO:
			{
//				Debug.Log ("新的战斗记录:" + ProtoIndexes.LVE_GO_LVE_DUO_RESP);
				
				return true;
			}
			case ProtoIndexes.LVE_BATTLE_RECORD_RESP:
			{
//				Debug.Log ("掠夺记录返回:" + ProtoIndexes.LVE_GO_LVE_DUO_RESP);
				LveBattleRecordResp recordRes = new LveBattleRecordResp();
				recordRes = QXComData.ReceiveQxProtoMessage (p_message,recordRes) as LveBattleRecordResp;
				
				if (recordRes != null)
				{
					if (recordRes.info == null)
					{
						recordRes.info = new List<LveBattleItem>();
					}
//					Debug.Log ("记录条数：" + recordRes.info.Count);

					PlunderPage.plunderPage.InItRecordPage (recordRes);
				}
				
				return true;
			}
			case ProtoIndexes.RANKING_RESP:
			{
				if (!isOpenLueDuo)
				{
					return false;
				}
//				Debug.Log ("请求掠夺排行返回:" + ProtoIndexes.RANKING_RESP);
				RankingResp rankResp = new RankingResp();
				rankResp = QXComData.ReceiveQxProtoMessage (p_message,rankResp) as RankingResp;

				if (rankResp != null)
				{
					if (rankResp.gongInfoList == null)
					{
						rankResp.gongInfoList = new List<GongJinInfo>();
					}
					Debug.Log (rankResp.gongInfoList.Count);
					PlunderRankType tempType = (PlunderRankType)Enum.ToObject (typeof (PlunderRankType),rankResp.rankType);
//					Debug.Log ("rankResp.gongInfoList.Count:" + rankResp.gongInfoList.Count);
//					for (int i = 0;i < rankResp.gongInfoList.Count;i ++)
//					{
//						Debug.Log (rankResp.gongInfoList[i].rank);
//					}
					switch (tempType)
					{
					case PlunderRankType.PERSONAL_RANK:

						if (PRankPage > 1)
						{
							if (rankResp.gongInfoList.Count == 0)
							{
								PRankPage -= 1;
							}
							else
							{
								PlunderPage.plunderPage.InItRankPage (tempType,serchType,rankResp);
							}
						}
						else
						{
							PlunderPage.plunderPage.InItRankPage (tempType,serchType,rankResp);
						}

						break;
					case PlunderRankType.ALLIANCE_RANK:

						if (ARankPage > 1)
						{
							if (rankResp.gongInfoList.Count == 0)
							{
								ARankPage -= 1;
							}
							else
							{
								PlunderPage.plunderPage.InItRankPage (tempType,serchType,rankResp);
							}
						}
						else
						{
							PlunderPage.plunderPage.InItRankPage (tempType,serchType,rankResp);
						}

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
	
	void LoadPlunderObj ()
	{
		if (plunderObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LUEDUO ),
			                        PlunderLoadBack );
		}
		else
		{
			plunderObj.SetActive (true);
			
			InItPlunderInfo ();
		}
	}
	void PlunderLoadBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		plunderObj = GameObject.Instantiate( p_object ) as GameObject;
		
		InItPlunderInfo ();
	}
	void InItPlunderInfo ()
	{
		if (!MainCityUI.IsExitInObjectList (plunderObj))
		{
			MainCityUI.TryAddToObjectList (plunderObj);
		}
		PlunderPage.plunderPage.InItPlunderPage (plunderResp);
	}

	//购买次数
	public void BuyPlunderNumReqBack (int i)
	{
		if (i == 2)
		{
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < plunderResp.buyNextBattleYB)
			{
//				textStr = "元宝不足！\n\n是否跳转到充值？";
//				QXComData.CreateBoxDiy (textStr,false,TurnToVip);
				Global.CreateFunctionIcon (101);
			}
			else
			{
				PlunderOperate (PlunderData.OperateType.ADD_NUM);
			}
		}
	}

	//清除冷却
	public void ClearCdReqBack (int i)
	{
		if (i == 2)
		{
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < plunderResp.clearCdYB)
			{
//				textStr = "元宝不足！\n是否跳转到充值？";
//				QXComData.CreateBoxDiy (textStr,false,TurnToVip);
				Global.CreateFunctionIcon (101);
			}
			else
			{
				PlunderOperate (PlunderData.OperateType.CLEAR_CD);
			}
		}
	}

	/// <summary>
	/// Turns to vip.
	/// </summary>
	/// <param name="i">The index.</param>
	public void TurnToVip (int i)
	{
		if (i == 2)
		{
			//跳转到充值
			RechargeData.Instance.RechargeDataReq ();
		}
	}

	/// <summary>
	/// Buies the plunder times.
	/// </summary>
	public void BuyPlunderTimes ()
	{
		if (plunderResp.remainBuyHuiShi <= 0)
		{
			if (QXComData.JunZhuInfo ().vipLv < QXComData.maxVipLevel)
			{
				if (QXComData.JunZhuInfo ().vipLv >= 1)
				{
					int curVipBuyTime = VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv).LveduoTimes;//当前vip可购买回数
					Debug.Log ("curVipBuyTime:" + curVipBuyTime);
					int totleBuyNum = 0;//当前vip可购买的额外挑战总次数
					for (int i = 0;i < curVipBuyTime;i ++)
					{
						totleBuyNum += (int)PurchaseTemplate.GetPurchaseTempByTypeAndTime (20,i + 1).number;
					}
					Debug.Log ("totleBuyNum:" + totleBuyNum);
					if (totleBuyNum > plunderResp.nowMaxBattleCount - int.Parse (CanshuTemplate.GetStrValueByKey (CanshuTemplate.LUEDUO_MAXNUM)))
					{
						int lastVipBuyTime = QXComData.JunZhuInfo ().vipLv - 1 >= 0 ? VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv - 1).LveduoTimes : 0;//上个vip可购买回数
						
						int startBuyTime = lastVipBuyTime + 1;//当前vip第几回购买
						
						PurchaseTemplate purchas = PurchaseTemplate.GetPurchaseTempByTypeAndTime (20,startBuyTime);
						int costYb = purchas.price;
						int buyNum = (int)purchas.number;
						int countBuyNum = curVipBuyTime - lastVipBuyTime;
						
						textStr = "掠夺次数用尽\n是否使用" + costYb + "元宝购买" + buyNum + "次掠夺机会？\n今日还可购买" + countBuyNum + "回";
						QXComData.CreateBoxDiy (textStr,false,BuyTimes);
					}
					else
					{
						textStr = "今日掠夺次数已用尽！\nV特权等级提升到" + (VipTemplate.GetNextLueDuoBuyTimeVipLevel (QXComData.JunZhuInfo ().vipLv))
							+ "级可购买更多掠夺次数！是否跳转到充值？";
						QXComData.CreateBoxDiy (textStr,false,TurnToVip,true);
					}
				}
				else
				{
					textStr = "今日掠夺次数已用尽！\nV特权等级提升到" + (VipTemplate.GetNextLueDuoBuyTimeVipLevel (QXComData.JunZhuInfo ().vipLv))
						+ "级可购买更多掠夺次数！是否跳转到充值？";
					QXComData.CreateBoxDiy (textStr,false,TurnToVip,true);
				}
			}
			else
			{
				textStr = "今日掠夺次数已用完...";
				QXComData.CreateBoxDiy (textStr,true,null);
			}
		}
		else
		{
			textStr = "今日掠夺次数已用完\n是否使用" + plunderResp.buyNextBattleYB + "元宝购买" + plunderResp.buyNextBattleCount + "次掠夺次数？\n今日还可购买" + plunderResp.remainBuyHuiShi + "回";
			QXComData.CreateBoxDiy (textStr,false,BuyTimes);
		}
	}

	/// <summary>
	/// Buies the times.
	/// </summary>
	/// <param name="i">The index.</param>
	public void BuyTimes (int i)
	{
		switch (i)
		{
		case 2:
			PlunderOperate (OperateType.ADD_NUM);
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Checks the plunder times.
	/// </summary>
	public void CheckPlunderTimes ()
	{
		//		Debug.Log ("plunderResp.all - plunderResp.used:" + (plunderResp.all - plunderResp.used));
		if (plunderResp.all - plunderResp.used == 1)
		{
			PushAndNotificationHelper.SetRedSpotNotification (215, false);
			if (WarPage.m_instance != null)
			{
				WarPage.m_instance.CheckRedPoint ();
			}
		}
	}

	void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
