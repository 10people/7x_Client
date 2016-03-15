using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpData : Singleton<PvpData>,SocketProcessor {

	private bool isPvpPageOpen = false;//是否打开首页
	public bool IsPvpPageOpen {set{isPvpPageOpen = value;} get{return isPvpPageOpen;}}

	private string textStr = "";

	void Awake () 
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	//打开百战
	public void OpenPvp ()
	{
		PvpDataReq ();
	}

	#region PVP_DataReq
	private BaiZhanInfoResp pvpInfoResp;//百战info
	private GameObject pvpObj;

	/// <summary>
	/// Pvps the data req.
	/// </summary>
	public void PvpDataReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.BAIZHAN_INFO_REQ,"27002|27012|27019");

//		Debug.Log ("PvpReq:" + ProtoIndexes.BAIZHAN_INFO_REQ);
	}
	#endregion

	#region PlayerStateCheck Part
	public enum PlayerState
	{
		STATE_PVP_MAIN_PAGE,
		STATE_GENERAL_TIAOZHAN_PAGE,
	}
	private PlayerState playerState = PlayerState.STATE_PVP_MAIN_PAGE;

	private OpponentInfo opponentInfo;//对手详情
	public OpponentInfo PvpOpponentInfo
	{
		set{opponentInfo = value;}
		get{return opponentInfo;}
	}

	private ChallengeResp challengeRes;//挑战队列
	public ChallengeResp PvpChallengeResp
	{
		set{challengeRes = value;}
		get{return challengeRes;}
	}
	private int PlayerStateCheckFaillType = -1;//挑战验证失败返回类型
	private long enemyId;

	/// <summary>
	/// Players the state check.
	/// </summary>
	/// <param name="tempState">Temp state.</param>
	public void PlayerStateCheck (PlayerState tempState)
	{
		playerState = tempState;

		PlayerStateReq stateReq = new PlayerStateReq ();

		int enemyRank = 0;
		int myRank = 0;
		switch (tempState)
		{
		case PlayerState.STATE_PVP_MAIN_PAGE:
		{
			if (PvpOpponentInfo == null)
			{
				Debug.LogError ("PvpOpponentInfo is null!");
				return;
			}
			enemyId = PvpOpponentInfo.junZhuId;
			enemyRank = PvpOpponentInfo.rank;
			myRank = BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.rank;

			break;
		}
		case PlayerState.STATE_GENERAL_TIAOZHAN_PAGE:
		{
			if (PvpChallengeResp == null)
			{
				Debug.LogError ("PvpChallengeResp is null!");
				return;
			}
			enemyId = PvpChallengeResp.oppoId;
			enemyRank = PvpChallengeResp.oppoRank;
			myRank = PvpChallengeResp.myRank;

			break;
		}
		default:
			break;
		}

		stateReq.enemyId = enemyId;
		stateReq.junRank = myRank;
		stateReq.rank = enemyRank;

		QXComData.SendQxProtoMessage (stateReq,ProtoIndexes.PLAYER_STATE_REQ,ProtoIndexes.PLAYER_STATE_RESP.ToString ());

//		Debug.Log ("PlayerStateCheck:" + ProtoIndexes.PLAYER_STATE_REQ);
	}
	#endregion

	#region PVP_ChallengeReq
	private GameObject challengeObj;
	/// <summary>
	/// Challenges the req.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	void ChallengeReq (long tempId)
	{
		ChallengeReq challengeReq = new ChallengeReq ();
		challengeReq.oppoJunZhuId = tempId;
		QXComData.SendQxProtoMessage (challengeReq,ProtoIndexes.CHALLENGE_REQ,ProtoIndexes.CHALLENGE_RESP.ToString ());
//		Debug.Log ("ChallengeReq:" + ProtoIndexes.CHALLENGE_REQ);
	}
	#endregion

	#region PVP_ConfirmReq
	public enum PVP_CONFIRM_TYPE
	{
		PVP_BUY_CISHU = 0,
		PVP_CLEAR_CD = 1,
		PVP_GET_REWARD = 2,
		PVP_REFRESH_PLARER = 3,
		PVP_REFRESH_MY_RANK = 4,
		PVP_REFRESH_OPPO_RANK = 5,
		PVP_GET_RANK_REWARD = 6,
		PVP_GET_DAY_REWARD = 7,
	}
	private PVP_CONFIRM_TYPE pvpConfirmType = PVP_CONFIRM_TYPE.PVP_BUY_CISHU;

	private ConfirmExecuteResp confirmResp;//百战操作返回

	private int canGetWeiWang;//当前可领取的威望
	public int CanGetWeiWang { set{canGetWeiWang = value;} get{return canGetWeiWang;} }

	private List<RewardData> rewardDataList;//每日排名结算奖励
	public List<RewardData> RewardDataList { set{rewardDataList = value;} get{return rewardDataList;} }

	/// <summary>
	/// Confirms the req.
	// 0 元宝确定购买挑战次数
	// 1 元宝清除下次挑战的冷却时间，
	// 2 确定领取生产奖励
	// 3 确定花费元宝更新玩家对手
	// 4 君主名次变化，刷新玩家对手（刷新名次）
	// 5 对手名次变化，重新刷对手（只是把名次与玩家重新对应）
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void ConfirmReq (PVP_CONFIRM_TYPE tempType)
	{
		pvpConfirmType = tempType;
		ConfirmExecuteReq confirmReq = new ConfirmExecuteReq();
		confirmReq.type = (int)tempType;

		QXComData.SendQxProtoMessage (confirmReq,ProtoIndexes.CONFIRM_EXECUTE_REQ,ProtoIndexes.CONFIRM_EXECUTE_RESP.ToString ());
		Debug.Log ("ConfirmReq:" + ProtoIndexes.CONFIRM_EXECUTE_REQ);
	}
	
	#endregion

	#region PvpRecord Part
	public void PvpRecordReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.ZhanDou_Notes_Req,ProtoIndexes.ZhanDou_Notes_Resq.ToString());
//		Debug.Log ("PvpRecordReq:" + ProtoIndexes.ZhanDou_Notes_Req);
	}
	#endregion

	#region SocketMessageCallBack Part
	public bool OnProcessSocketMessage (QXBuffer p_message) 
	{	
		if (p_message != null) 
		{	
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.BAIZHAN_INFO_RESP://百战首页信息
			{
//				Debug.Log ("PvpnRes:" + ProtoIndexes.BAIZHAN_INFO_RESP);

				BaiZhanInfoResp pvpInfo = new BaiZhanInfoResp ();
				pvpInfo = QXComData.ReceiveQxProtoMessage (p_message,pvpInfo) as BaiZhanInfoResp;
				
				if (pvpInfo != null)
				{
					if (pvpInfo.oppoList == null)
					{
						pvpInfo.oppoList = new List<OpponentInfo>();
					}
					
					pvpInfoResp = pvpInfo;

//					Debug.Log ("pvpInfo.nextTimeTo21:" + pvpInfo.nextTimeTo21);
//					Debug.Log ("pvpInfo.baizhanXMLId:" + pvpInfo.pvpInfo.baizhanXMLId);
//					Debug.Log ("pvpInfo.rankAward:" + pvpInfo.rankAward);

					if (pvpObj == null)
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_PAGE ),
						                        PvpLoadCallback );
					}
					else
					{
						pvpObj.SetActive (true);
						
						InItPvpPage ();
					}
				}
				
				return true;
			}
			case ProtoIndexes.CONFIRM_EXECUTE_RESP://百战操作返回
			{
//				Debug.Log ("ConfirmResp:" + ProtoIndexes.CONFIRM_EXECUTE_RESP);

				ConfirmExecuteResp confirm = new ConfirmExecuteResp ();
				confirm = QXComData.ReceiveQxProtoMessage (p_message,confirm) as ConfirmExecuteResp;

				if (confirm != null)
				{
					confirmResp = confirm;

					switch (pvpConfirmType)
					{
					case PVP_CONFIRM_TYPE.PVP_BUY_CISHU:
					{
//						Debug.Log ("购买挑战次数成功");

						if (confirm.buyCiShuInfo != null)
						{
//							LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_ADDNUM_DES);
							switch (confirm.buyCiShuInfo.success)
							{
							case 0://元宝不足
								textStr = "元宝不足,是否前往充值？";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.TurnToVipPage);
								break;
							case 1://购买成功
//								Debug.Log ("剩余次数：" + confirm.buyCiShuInfo.leftTimes);
//								Debug.Log ("总次数：" + confirm.buyCiShuInfo.totalTimes);
								BaiZhanPage.baiZhanPage.OpenSkillEffect (2);
								PvpDataReq ();
//								textStr = "\n\n恭喜购买挑战次数成功！";
//								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
//								                 QXComData.confirmStr, null, 
//								                 BaiZhanPage.baiZhanPage.OpenSkillEffect);
								break;
							case 2://数据错误
								textStr = "购买失败，请重新购买！";
								QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
								break;
							case 3://今日购买次数用尽
								int vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;
								if (vipLevel < QXComData.maxVipLevel)
								{
									textStr = "今日购买次数已用完，V特权等级提升到" + (vipLevel + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
									QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.TurnToVipPage);
								}
								else
								{
									textStr = "今日购买次数已用完...";
									QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
								}

								break;
							default:
								break;
							}
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_CLEAR_CD:
					{
						if (confirm.cleanCDInfo != null)
						{
							switch (confirm.cleanCDInfo.success)
							{
							case 0://元宝不足
								textStr = "元宝不足,是否前往充值？";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.TurnToVipPage);
								break;
							case 1:
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_TITLE);
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_SUCCESS);
								textStr = "清除冷却成功！";

								BaiZhanPage.baiZhanPage.baiZhanResp.cdYuanBao = confirm.cleanCDInfo.nextCDYB;
								BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.time = 0;
								BaiZhanPage.baiZhanPage.InItChallenge ();
//								QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
								break;
							case 2:
								textStr = "清除冷却失败，请重新尝试！";
								QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
								break;
							case 3:
								textStr = "V特权等级不足，V特权等级提升到" + BaiZhanPage.baiZhanPage.baiZhanResp.canCleanCDvipLev + "级即可重置挑战冷却时间。\n\n是否前往充值？ ";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.TurnToVipPage);
								break;
							default:
								break;
							}
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_GET_REWARD:
					{
						if (confirmResp.getAwardInfo.success == 1)
						{
//							Debug.Log ("领取成功");

//							PvpPage.pvpPage.pvpResp.canGetweiWang = 0;
//							PvpPage.pvpPage.pvpResp.hasWeiWang += CanGetWeiWang;
//							PvpPage.pvpPage.InItMyRank ();

							BaiZhanPage.baiZhanPage.baiZhanResp.canGetweiWang = 0;
							BaiZhanPage.baiZhanPage.baiZhanResp.hasWeiWang += CanGetWeiWang;
							BaiZhanPage.baiZhanPage.InItMyRank ();

							//关闭按钮特效
							if (QXComData.CheckYinDaoOpenState (100210))
							{
								UIYindao.m_UIYindao.setCloseUIEff ();
							}

							RewardData data = new RewardData (900011,CanGetWeiWang);
							GeneralRewardManager.Instance().CreateReward (data);
						}
						else
						{
//							Debug.Log ("领取失败");

							textStr = "您已领取过该奖励！";
							QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
							//刷新百战奖励领取
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_REFRESH_MY_RANK:
					{
						if (playerState == PlayerState.STATE_PVP_MAIN_PAGE)
						{
							if (confirm.refreshMyInfo != null)
							{
								if (confirm.refreshMyInfo.oppoList == null)
								{
									confirm.refreshMyInfo.oppoList = new List<OpponentInfo>();
								}

								BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.rank = confirm.refreshMyInfo.junZhuRank;
								BaiZhanPage.baiZhanPage.baiZhanResp.oppoList = confirm.refreshMyInfo.oppoList;
								BaiZhanPage.baiZhanPage.InItMyRank ();
								BaiZhanPage.baiZhanPage.InItOpponent ();
							}
						}
						else
						{
							//关闭挑战阵容页面，刷新百战信息
							ReOpenPvp ();
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_REFRESH_OPPO_RANK:
					{
						if (playerState == PlayerState.STATE_PVP_MAIN_PAGE)
						{
							if (confirm.refreshOtherInfo != null)
							{
								if (confirm.refreshOtherInfo.oppoList == null)
								{
									confirm.refreshOtherInfo.oppoList = new List<OpponentInfo>();
								}

								BaiZhanPage.baiZhanPage.baiZhanResp.oppoList = confirm.refreshOtherInfo.oppoList;
								BaiZhanPage.baiZhanPage.InItOpponent ();
							}
						}
						else
						{
							//关闭挑战阵容页面，刷新百战信息
							ReOpenPvp ();
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_REFRESH_PLARER:
					{
						if (confirm.payChangeInfo.success == 1)
						{
							BaiZhanPage.baiZhanPage.baiZhanResp.huanYiPiYB = confirm.payChangeInfo.nextHuanYiPiYB;
							BaiZhanPage.baiZhanPage.baiZhanResp.oppoList = confirm.payChangeInfo.oppoList;
							BaiZhanPage.baiZhanPage.InItMyRank ();
							BaiZhanPage.baiZhanPage.InItOpponent ();

							BaiZhanPage.baiZhanPage.OpenSkillEffect (2);

//							textStr = "\n\n更换挑战对手成功!";
//							Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
//							                 QXComData.confirmStr, null, 
//							                 BaiZhanPage.baiZhanPage.OpenSkillEffect);
						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_GET_RANK_REWARD:
					{
						switch (confirm.getAwardInfo.success)
						{
						case 0://失败
							break;
						case 1://成功

							BaiZhanPage.baiZhanPage.baiZhanResp.rankAward = 0;
							BaiZhanPage.baiZhanPage.InItMyRank ();

							BaiZhanPage.baiZhanPage.OpenHighRankRewardWindow (rewardDataList);

							break;
						default:
							break;
						}
						break;
					}
					case PVP_CONFIRM_TYPE.PVP_GET_DAY_REWARD:
					{
						switch (confirm.getAwardInfo.success)
						{
						case 0://失败
							break;
						case 1://成功
							
							BaiZhanPage.baiZhanPage.baiZhanResp.nextTimeTo21 = -1;
							BaiZhanPage.baiZhanPage.InItMyRank ();

							GeneralRewardManager.Instance().CreateReward (rewardDataList);

							break;
						default:
							break;
						}
						break;
					}
					}
				}

				return true;
			}
			case ProtoIndexes.ZhanDou_Notes_Resq://百战战斗记录返回
			{
//				Debug.Log ("PvpRecordResp:" + ProtoIndexes.ZhanDou_Notes_Resq);
				
				ZhandouRecordResp pvpRecord = new ZhandouRecordResp ();
				pvpRecord = QXComData.ReceiveQxProtoMessage (p_message,pvpRecord) as ZhandouRecordResp;

				if (pvpRecord != null)
				{
					if (pvpRecord.info == null)
					{
						pvpRecord.info = new List<ZhandouItem>();
					}
					BaiZhanPage.baiZhanPage.LoadPvpRecordPrefab (pvpRecord);
				}

				return true;
			}
			case ProtoIndexes.PLAYER_STATE_RESP:
			{
//				Debug.Log ("PlayerStateCheck:" + ProtoIndexes.PLAYER_STATE_RESP);

				PlayerStateResp stateRes = new PlayerStateResp ();
				stateRes = QXComData.ReceiveQxProtoMessage (p_message,stateRes) as PlayerStateResp;

				if (stateRes != null)
				{
//					Debug.Log ("StatesType:" + stateRes.type);

					UIYindao.m_UIYindao.CloseUI ();
					if (stateRes.type == 1)
					{
//						Debug.Log ("可以挑战，发送挑战请求");

						switch (playerState)
						{
						case PlayerState.STATE_PVP_MAIN_PAGE:

							ChallengeReq (enemyId);//发送挑战请求

							break;
						case PlayerState.STATE_GENERAL_TIAOZHAN_PAGE:

							Global.m_isOpenBaiZhan = true;

							UIYindao.m_UIYindao.CloseUI ();
							EnterBattleField.EnterBattlePvp (enemyId, QXComData.CheckYinDaoOpenState (100200));

							break;
						default:
							break;
						}
					}
					
					else
					{
						PlayerStateCheckFaillType = stateRes.type;

						if (stateRes.type == 4)
						{
							//挑战机会用完
							//可否购买挑战机会check************
							if (BaiZhanPage.baiZhanPage.baiZhanResp.leftCanBuyCount == 0)
							{
								if (JunZhuData.Instance().m_junzhuInfo.vipLv < 10)
								{
									string textDes1 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_7);
									string textDes2 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_8).Replace ('*',char.Parse ((JunZhuData.Instance().m_junzhuInfo.vipLv + 1).ToString ()));
									string textDes3 = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
//									textStr = "今日挑战次数已用完，V特权等级提升到" + (JunZhuData.Instance().m_junzhuInfo.vipLv + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
									textStr = textDes1 + "\n" + textDes2 + "\n" + textDes3;
									QXComData.CreateBox (1,textStr,true,null);
								}
								else
								{
									textStr = "今日挑战次数已用完...";
									QXComData.CreateBox (1,textStr,true,BaiZhanPage.baiZhanPage.OpenSkillEffect);
								}
							}
							else
							{
								textStr = "今日挑战次数已用完\n\n是否使用" + BaiZhanPage.baiZhanPage.baiZhanResp.buyNeedYB + "元宝购买" + BaiZhanPage.baiZhanPage.baiZhanResp.buyNumber + "次挑战次数？\n\n今日还可购买" + BaiZhanPage.baiZhanPage.baiZhanResp.leftCanBuyCount + "回";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.BuyTimes);
							}
						}
						else if (stateRes.type == 5)
						{
							//冷却cd
							//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_DISCD);
							int cleanCdVipLevel = BaiZhanPage.baiZhanPage.baiZhanResp.canCleanCDvipLev;
							if (JunZhuData.Instance().m_junzhuInfo.vipLv < cleanCdVipLevel)
							{
								textStr = "V特权等级不足，V特权等级提升到" + cleanCdVipLevel + "级即可重置挑战冷却时间。\n\n是否前往充值？ ";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.TurnToVipPage);
							}
							else
							{
								textStr = "挑战冷却中,是否使用" + BaiZhanPage.baiZhanPage.baiZhanResp.cdYuanBao + "元宝清除挑战冷却？";
								QXComData.CreateBox (1,textStr,false,BaiZhanPage.baiZhanPage.ClearCd);
							}
						}
						else
						{
							switch (stateRes.type)
							{
							case 2://玩家正在被挑战
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_BGING_CHALLENGED);
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_WAIT_TRY);
								textStr = "这位玩家正在被挑战\n\n请稍后再做尝试...";
								break;
							case 3://君主等级不足
								textStr = "君主等级不足，无法挑战...";
								break;
							case 6://数据出错
								textStr = "挑战失败，请稍后再做尝试...";
								break;
							case 7://对手名次发生变化
								textStr = "对手名次发生变化，请重新选择挑战对手！";
								break;
							case 8://自己的名次已发生变化
								textStr = "您的名次已发生变化！";
								break;
							default:
								break;
							}

							QXComData.CreateBox (1,textStr,true,PlayerStateCheckCallBack);
						}
					}
				}

				return true;
			}
			case ProtoIndexes.CHALLENGE_RESP://请求挑战返回
			{
//				Debug.Log ("ChallengeResp:" + ProtoIndexes.CHALLENGE_RESP);

				ChallengeResp challenge = new ChallengeResp ();
				challenge = QXComData.ReceiveQxProtoMessage (p_message,challenge) as ChallengeResp;

				if (challenge != null)
				{
//					Debug.Log ("组合id：" + challenge.myZuheId);

					PvpChallengeResp = challenge;

					GeneralControl.Instance.OpenPvpChallengePage (PvpChallengeResp);

					if (IsPvpPageOpen)
					{
						BaiZhanPage.baiZhanPage.DisActiveObj ();
					}
				}
				
				return true;
			}
			}
		}
		
		return false;
	}

	public void PvpLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		pvpObj = Instantiate (p_object) as GameObject;

		InItPvpPage ();
	}

	void InItPvpPage ()
	{
		MainCityUI.TryAddToObjectList(pvpObj);
		{
			UI2DTool.Instance.AddTopUI( GameObjectHelper.GetRootGameObject( pvpObj ) );
		}

		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}

//		PvpPage.pvpPage.InItPvpPage (pvpInfoResp);
		BaiZhanPage.baiZhanPage.InItBaiZhanPage (pvpInfoResp);
	}

	//挑战验证失败回调
	private void PlayerStateCheckCallBack (int i)
	{
		if (playerState == PlayerState.STATE_PVP_MAIN_PAGE)
		{
			BaiZhanPage.baiZhanPage.OpenSkillEffect (2);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);

			switch (PlayerStateCheckFaillType)
			{
			case 2:
				//对手正在被挑战
				ConfirmReq (PVP_CONFIRM_TYPE.PVP_REFRESH_OPPO_RANK);//刷新对手名次
				break;
			case 3:
				//关闭百战
				UIYindao.m_UIYindao.CloseUI ();
				BaiZhanPage.baiZhanPage.CancelBtn ();
				break;
			case 6:
				ConfirmReq (PVP_CONFIRM_TYPE.PVP_REFRESH_OPPO_RANK);//刷新对手名次
				break;
			case 7:
				ConfirmReq (PVP_CONFIRM_TYPE.PVP_REFRESH_OPPO_RANK);//刷新对手名次
				break;
			case 8:
				ConfirmReq (PVP_CONFIRM_TYPE.PVP_REFRESH_MY_RANK);//刷新我的名次
				break;
			default:
				break;
			}
		}
		else
		{
			//关闭挑战阵容页面，刷新百战首页
			ReOpenPvp ();
		}
	}
	void ReOpenPvp ()
	{
		GeneralChallengePage.gcPage.PvpReset = true;
		GeneralChallengePage.gcPage.DisActiveObj ();

		OpenPvp ();
	}
	#endregion

	void OnDestroy ()
	{	
		SocketTool.UnRegisterMessageProcessor (this);
		base.OnDestroy();
	}
}
