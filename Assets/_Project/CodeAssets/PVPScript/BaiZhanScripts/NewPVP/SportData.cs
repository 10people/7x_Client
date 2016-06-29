using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportData : Singleton<SportData>,SocketProcessor {

	#region MainPageReq

	private BaiZhanInfoResp m_sportResp;

	private GameObject m_sportRoot;

	public bool m_isOpenSport = false;

	private string m_textStr;

	public void OpenSport ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.BAIZHAN_INFO_REQ,ProtoIndexes.BAIZHAN_INFO_RESP.ToString ());
		Debug.Log ("竞技首页请求:" + ProtoIndexes.BAIZHAN_INFO_REQ);
	}

	#endregion

	#region SportOperate

	public enum SportOperate
	{
		BUY_SPORT_TIME = 0,		//元宝购买挑战次数
		CLEAR_CD = 1,			//清除cd
		REFRESH_ENEMYLIST = 3,	//刷新对手列表
		GET_RANK_REWARD = 6,	//领取排名奖励
		GET_DAILY_REWARD = 7,	//领取每日奖励
		SPORT_ENEMY_PAGE = 8,	//指定军衔敌人列表
	}
	private SportOperate m_sportOperate;

	public bool M_ClearByBtn = false;	//按钮清除cd

	public void SportOperateReq (SportOperate tempType,int tempJunXianId = 0)
	{
		m_sportOperate = tempType;

		ConfirmExecuteReq confirmReq = new ConfirmExecuteReq();
		confirmReq.type = (int)tempType;

		if (tempType == SportOperate.SPORT_ENEMY_PAGE || tempType == SportOperate.REFRESH_ENEMYLIST)
		{
			confirmReq.junxianid = tempJunXianId;
//			Debug.Log ("tempJunXianId:" + tempJunXianId);
		}

		QXComData.SendQxProtoMessage (confirmReq,ProtoIndexes.CONFIRM_EXECUTE_REQ,ProtoIndexes.CONFIRM_EXECUTE_RESP.ToString ());
//		Debug.Log ("ConfirmReq:" + ProtoIndexes.CONFIRM_EXECUTE_REQ);
	}

	#endregion

	#region Challenge Enemy

	public enum ChallengeType
	{
		ENTER_FIGHT = 1,		//可以挑战
		FIGHTING = 2,			//正在被挑战
		LEVEL_NOT_ENOUGH = 3,	//君主等级不足
		HAVE_NO_CHANCE = 4,		//今日挑战机会用完
		CD = 5,					//挑战cd中
		WORROR_DATA = 6,		//数据错误
		OPPO_RANK_CHANGE = 7,	//对手名次变更
		SELF_RANK_CHANGE = 8,	//自己名次变更
		RANK_NOT_ENOUGH = 9,	//排名不足以挑战
	}

	private ChallengeType challengeType;

	public enum EnterPlace
	{
		PLAYER = 1,
		ENTER_BTN = 2,
	}

	private EnterPlace m_enterPlace;

	private long m_enemyId;//挑战的君主id

	public void SportChallenge (long tempJunZhuId,int tempOppoRank,EnterPlace tempEnterPlace)
	{
		m_enterPlace = tempEnterPlace;
		m_enemyId = tempJunZhuId;

		ChallengeReq challenge = new ChallengeReq ();

		challenge.oppoJunZhuId = tempJunZhuId;
		challenge.oppoJunzhuRank = tempOppoRank;
		challenge.type = (int)tempEnterPlace;

		QXComData.SendQxProtoMessage (challenge,ProtoIndexes.CHALLENGE_REQ,ProtoIndexes.CHALLENGE_RESP.ToString ());
//		Debug.Log ("挑战请求：" + ProtoIndexes.CHALLENGE_REQ);
	}

	#endregion

	#region SportRecord
	public void SportRecordReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.ZhanDou_Notes_Req,ProtoIndexes.ZhanDou_Notes_Resq.ToString());
//		Debug.Log ("PvpRecordReq:" + ProtoIndexes.ZhanDou_Notes_Req);
	}
	#endregion

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.BAIZHAN_INFO_RESP:
			{
				Debug.Log ("竞技首页返回：" + ProtoIndexes.BAIZHAN_INFO_RESP);

				BaiZhanInfoResp sportResp = new BaiZhanInfoResp ();
				sportResp = QXComData.ReceiveQxProtoMessage (p_message,sportResp) as BaiZhanInfoResp;

				if (sportResp != null)
				{
					m_sportResp = sportResp;
					UIYindao.m_UIYindao.CloseUI ();
					LoadSportRoot ();
				}

				return true;
			}
			case ProtoIndexes.CONFIRM_EXECUTE_RESP:
			{
//				Debug.Log ("竞技操作返回:" + ProtoIndexes.CONFIRM_EXECUTE_RESP);
				
				ConfirmExecuteResp confirm = new ConfirmExecuteResp ();
				confirm = QXComData.ReceiveQxProtoMessage (p_message,confirm) as ConfirmExecuteResp;

				if (confirm != null)
				{
					switch (m_sportOperate)
					{
					case SportOperate.BUY_SPORT_TIME:
					{
						switch (confirm.buyCiShuInfo.success)
						{
						case 0://元宝不足

							Global.CreateFunctionIcon (101);
//							m_textStr = "元宝不足,是否前往充值？";
//							QXComData.CreateBoxDiy (m_textStr,false,TurnToVip);
							break;
						case 1://成功

							SportPage.m_instance.RefreshPlayerInfo (confirm.buyCiShuInfo);

							break;
						case 2://数据错误
							m_textStr = "购买失败，请重新购买！";
							QXComData.CreateBoxDiy (m_textStr,true,null);
							break;
						case 3://购买次数用尽

							if (QXComData.JunZhuInfo ().vipLv < QXComData.maxVipLevel)
							{
//								m_textStr = "今日购买次数已用完，V特权等级提升到" + (QXComData.JunZhuInfo ().vipLv + 1) + "级即可购买更多挑战次数。是否前往充值？";
//								QXComData.CreateBox (1,m_textStr,false,TurnToVip);
								Global.CreateFunctionIcon (1901);
							}
							else
							{
								m_textStr = "今日购买次数已用完...";
								QXComData.CreateBoxDiy (m_textStr,true,null);
							}
							break;
						default:
							break;
						}
						break;
					}
					case SportOperate.CLEAR_CD:
					{
						switch (confirm.cleanCDInfo.success)
						{
						case 0://元宝不足
							Global.CreateFunctionIcon (101);
//							m_textStr = "元宝不足,是否前往充值？";
//							QXComData.CreateBoxDiy (m_textStr,false,TurnToVip);
							break;
						case 1://成功
//							LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_TITLE);
							//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_SUCCESS);
//							m_textStr = "清除冷却成功！";

							SportPage.m_instance.RefreshPlayerInfo (confirm.cleanCDInfo);

							//turn to challengepage
							{
								if (!M_ClearByBtn)
								{
									SportPage.m_instance.SportEnemyInfoClickCallBack (1);
								}
							}
							break;
						case 2://数据错误
							m_textStr = "清除冷却失败，请重新尝试！";
							QXComData.CreateBoxDiy (m_textStr,true,null);
							break;
						case 3://vip等级不足
//							m_textStr = "V特权等级不足，V特权等级提升到" + QXComData.SportClearCdVipLevel + "级即可重置挑战冷却时间。是否前往充值？ ";
//							QXComData.CreateBoxDiy (m_textStr,false,TurnToVip);
							Global.CreateFunctionIcon (1901);
							break;
						default:
							break;
						}
						break;
					}
					case SportOperate.REFRESH_ENEMYLIST:
					{
						switch (confirm.payChangeInfo.success)
						{
						case 1://成功
							SportPage.m_instance.RefreshSportEnemyList (confirm.payChangeInfo);
							break;
						default:
							
							break;
						}
						break;
					}
					case SportOperate.GET_RANK_REWARD:
					{
						switch (confirm.getAwardInfo.success)
						{
						case 0://失败
							m_textStr = "领取失败！";
							QXComData.CreateBoxDiy (m_textStr,true,null);
							break;
						case 1://成功
							SportPage.m_instance.GetHighRankReward ();
							break;
						default:
							break;
						}
						break;
					}
					case SportOperate.GET_DAILY_REWARD:
					{
						switch (confirm.getAwardInfo.success)
						{
						case 0://失败
							m_textStr = "领取失败！";
							QXComData.CreateBoxDiy (m_textStr,true,null);
							break;
						case 1://成功
							SportPage.m_instance.GetDailyRankReward ();
							break;
						default:
							break;
						}
						break;
					}
					case SportOperate.SPORT_ENEMY_PAGE:
					{
						if (m_isOpenSport)
						{
							if (confirm.refreshOtherInfo.oppoList == null)
							{
								confirm.refreshOtherInfo.oppoList = new List<OpponentInfo>();
							}
//							Debug.Log ("confirm.refreshOtherInfo.oppoList:" + confirm.refreshOtherInfo.oppoList.Count);
//							foreach (OpponentInfo oppo in confirm.refreshOtherInfo.oppoList)
//							{
//								Debug.Log ("oppo.junZhuId:" + oppo.junZhuId);
//							}
							SportPage.m_instance.OpenJunXianRoom (confirm.refreshOtherInfo);
						}
						break;
					}
					}
				}

				return true;
			}
			case ProtoIndexes.CHALLENGE_RESP:
			{
//				Debug.Log ("请求挑战返回:" + ProtoIndexes.CHALLENGE_RESP);
				
				ChallengeResp challenge = new ChallengeResp ();
				challenge = QXComData.ReceiveQxProtoMessage (p_message,challenge) as ChallengeResp;
				
				if (challenge != null)
				{
//					Debug.Log ("challenge.type:" + challenge.type);
					ChallengeType challengeType = (ChallengeType)Enum.ToObject (typeof(ChallengeType),challenge.type);
					if (challengeType == ChallengeType.ENTER_FIGHT)
					{
						if (m_enterPlace == EnterPlace.PLAYER)
						{
							//弹出对战阵容
							GeneralControl.Instance.OpenChallengePage (GeneralChallengePage.ChallengeType.SPORT,challenge);
						}
						else
						{
							//进入战斗
							UIYindao.m_UIYindao.CloseUI ();
							SportPage.m_instance.SetSportEnemyObj (false);
							Global.m_isSportDataInItEnd = false;
//							Global.m_isOpenBaiZhan = true;
							EnterBattleField.EnterBattlePvp (m_enemyId, QXComData.CheckYinDaoOpenState (100200));
						}
					}
					else
					{
						if (challengeType == ChallengeType.CD)//冷却cd
						{
							M_ClearByBtn = false;
							m_textStr = "挑战冷却中,\n是否使用" + SportPage.m_instance.SportResp.cdYuanBao + "元宝清除挑战冷却？";
							QXComData.CreateBoxDiy (m_textStr,false,ClearCd,true,0,QXComData.SportClearCdVipLevel);
//							if (QXComData.JunZhuInfo ().vipLv < QXComData.SportClearCdVipLevel)
//							{
//								m_textStr = "V特权等级不足，V特权等级提升到" + QXComData.SportClearCdVipLevel + "级即可重置挑战冷却时间。是否前往充值？ ";
//								QXComData.CreateBoxDiy (m_textStr,false,TurnToVip);
////								Global.CreateFunctionIcon (1901);
//							}
//							else
//							{
//								m_textStr = "挑战冷却中,是否使用" + SportPage.m_instance.SportResp.cdYuanBao + "元宝清除挑战冷却？";
//								QXComData.CreateBoxDiy (m_textStr,false,ClearCd);
//							}
						}
						else if (challengeType == ChallengeType.HAVE_NO_CHANCE)//挑战机会用完
						{
							BuyChallengeTimes ();
						}
						else
						{
							switch (challengeType)
							{
							case ChallengeType.FIGHTING://玩家正在被挑战
							{
								m_textStr = "对手正在被挑战...";
								QXComData.CreateBoxDiy (m_textStr,true,null);
								break;
							}
							case ChallengeType.LEVEL_NOT_ENOUGH://君主等级不足
							{
								m_textStr = "等级不足，无法挑战...";
								QXComData.CreateBoxDiy (m_textStr,true,null);
								break;
							}
							case ChallengeType.WORROR_DATA://数据出错
							{
								m_textStr = "挑战失败，请稍后再做尝试...";
								QXComData.CreateBoxDiy (m_textStr,true,null);
								break;
							}
							case ChallengeType.OPPO_RANK_CHANGE://对手名次发生变化
							{
								//更新对手列表

								SportPage.m_instance.SetSportEnemyObj (false);
								GeneralControl.Instance.SetChallengeObjState (false);

								RefreshOtherInfo otherInfo = new RefreshOtherInfo();
								otherInfo.oppoList = challenge.oppoList;
								SportPage.m_instance.OpenJunXianRoom (otherInfo);
								m_textStr = "对手名次发生变化，请重新选择对手...";
								QXComData.CreateBoxDiy (m_textStr,true,RecheckPlayer,true);

								break;
							}
							case ChallengeType.SELF_RANK_CHANGE://自己的名次已发生变化
							{
								SportPage.m_instance.SetSportEnemyObj (false);
								GeneralControl.Instance.SetChallengeObjState (false);
								//更新主界面军衔
								SportPage.m_instance.RefreshPlayerInfo (challenge.myRank);
								m_textStr = "您的排名已经发生变化...";
								QXComData.CreateBoxDiy (m_textStr,true,RecheckPlayer,true);

								break;
							}
							case ChallengeType.RANK_NOT_ENOUGH://玩家排名不足以挑战目标
							{
								m_textStr = "军衔不足...";
								QXComData.CreateBoxDiy (m_textStr,true,null);
								break;
							}
							default:
								break;
							}
						}
					}
				}

				return true;
			}
			case ProtoIndexes.ZhanDou_Notes_Resq://百战战斗记录返回
			{
				Debug.Log ("PvpRecordResp:" + ProtoIndexes.ZhanDou_Notes_Resq);
				
				ZhandouRecordResp sportRecord = new ZhandouRecordResp ();
				sportRecord = QXComData.ReceiveQxProtoMessage (p_message,sportRecord) as ZhandouRecordResp;
				
				if (sportRecord != null)
				{
					if (sportRecord.info == null)
					{
						sportRecord.info = new List<ZhandouItem>();
					}
					GeneralControl.Instance.LoadRecordPage (GeneralRecord.RecordType.SPORT,sportRecord,InstanceEffect);
				}
				
				return true;
			}
			}
		}

		return false;
	}

	void RecheckPlayer (int i)
	{
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,4);
	}

	void InstanceEffect ()
	{
		SportPage.m_instance.DailyRewardBtnEffect ();
	}

	#region Challenge State Check
	/// <summary>
	/// Clears the cd.
	/// </summary>
	/// <param name="i">The index.</param>
	public void ClearCd (int i)
	{
		switch (i)
		{
		case 2:
			SportOperateReq (SportOperate.CLEAR_CD);
			break;
		default:
			break;
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
			SportOperateReq (SportOperate.BUY_SPORT_TIME);
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Turns to vip.
	/// </summary>
	/// <param name="i">The index.</param>
	public void TurnToVip (int i)
	{
		switch (i)
		{
		case 2:
			RechargeData.Instance.RechargeDataReq ();
			break;
		default:
			break;
		}
	}

	#endregion

	#region LoadSportRoot And InItSportPage
	void LoadSportRoot ()
	{
		if (m_sportRoot != null)
		{
			m_sportRoot.SetActive (true);

			InItSportPage ();
		}
		else
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_PAGE ),SportRootLoadCallback );
		}
	}

	private void SportRootLoadCallback ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		m_sportRoot = Instantiate (p_object) as GameObject;
		
		InItSportPage ();
	}

	private void InItSportPage ()
	{
		MainCityUI.TryAddToObjectList (m_sportRoot);
		{
			UI2DTool.Instance.AddTopUI( GameObjectHelper.GetRootGameObject( m_sportRoot ) );
		}

		SportPage.m_instance.InItSportPage (m_sportResp,SportPageClickCallBack);
		m_isOpenSport = true;
	}

	void SportPageClickCallBack ()
	{
		MainCityUI.TryRemoveFromObjectList (m_sportRoot);
		m_isOpenSport = false;
		m_sportRoot.SetActive (false);
	}

	#endregion

	public void BuyChallengeTimes ()
	{
		if (SportPage.m_instance.SportResp.leftCanBuyCount <= 0)
		{
			if (QXComData.JunZhuInfo ().vipLv < QXComData.maxVipLevel)
			{
				if (QXComData.JunZhuInfo ().vipLv >= 1)
				{
					int curVipBuyTime = VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv).bugBaizhanTime;//当前vip可购买回数
					Debug.Log ("curVipBuyTime:" + curVipBuyTime);
					int totleBuyNum = 0;//当前vip可购买的额外挑战总次数
					for (int i = 0;i < curVipBuyTime;i ++)
					{
						totleBuyNum += (int)PurchaseTemplate.GetPurchaseTempByTypeAndTime (3,i + 1).number;
					}
					Debug.Log ("totleBuyNum:" + totleBuyNum);
					if (totleBuyNum > SportPage.m_instance.SportResp.totalTimes - int.Parse (CanshuTemplate.GetStrValueByKey (CanshuTemplate.BAIZHAN_FREE_TIMES)))
					{
						int lastVipBuyTime = QXComData.JunZhuInfo ().vipLv - 1 >= 0 ? VipTemplate.GetVipInfoByLevel (QXComData.JunZhuInfo ().vipLv - 1).bugBaizhanTime : 0;//上个vip可购买回数
						
						int startBuyTime = lastVipBuyTime + 1;//当前vip第几回购买
						
						PurchaseTemplate purchas = PurchaseTemplate.GetPurchaseTempByTypeAndTime (3,startBuyTime);
						int costYb = purchas.price;
						int buyNum = (int)purchas.number;
						int countBuyNum = curVipBuyTime - lastVipBuyTime;
						
						m_textStr = "今日挑战次数已用完\n是否使用" + costYb + "元宝购买" + buyNum + "次挑战次数？\n今日还可购买" + countBuyNum + "回";
						QXComData.CreateBoxDiy (m_textStr,false,BuyTimes);
					}
					else
					{
						string textDes1 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_7);
						string textDes2 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_8).Replace ('*',char.Parse (VipTemplate.GetNextBaiZhanBuyTimeVipLevel (QXComData.JunZhuInfo ().vipLv).ToString ()));
						string textDes3 = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
						//									textStr = "今日挑战次数已用完，V特权等级提升到" + (JunZhuData.Instance().m_junzhuInfo.vipLv + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
						m_textStr = textDes1 + "\n" + textDes2 + textDes3;
						QXComData.CreateBoxDiy (m_textStr,false,TurnToVip,true);
					}
				}
				else
				{
					string textDes1 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_7);
					string textDes2 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_8).Replace ('*',char.Parse (VipTemplate.GetNextBaiZhanBuyTimeVipLevel (QXComData.JunZhuInfo ().vipLv).ToString ()));
					string textDes3 = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
					//									textStr = "今日挑战次数已用完，V特权等级提升到" + (JunZhuData.Instance().m_junzhuInfo.vipLv + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
					m_textStr = textDes1 + "\n" + textDes2 + textDes3;
					QXComData.CreateBoxDiy (m_textStr,false,TurnToVip,true);
				}
			}
			else
			{
				m_textStr = "今日挑战次数已用完...";
				QXComData.CreateBoxDiy (m_textStr,true,null);
			}
		}
		else
		{
			m_textStr = "今日挑战次数已用完\n是否使用" + SportPage.m_instance.SportResp.buyNeedYB + "元宝购买" + SportPage.m_instance.SportResp.buyNumber + "次挑战次数？\n今日还可购买" + SportPage.m_instance.SportResp.leftCanBuyCount + "回";
			QXComData.CreateBoxDiy (m_textStr,false,BuyTimes);
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
		base.OnDestroy ();
	}
}
