using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpData : Singleton<PvpData>,SocketProcessor {

	private bool isOpenPvpByBtn = false;
	public bool IsOpenPvpByBtn
	{
		set{isOpenPvpByBtn = value;}
		get{return isOpenPvpByBtn;}
	}
	private string textStr = "";

	void Awake () 
	{
		SocketTool.RegisterMessageProcessor (this);
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
			myRank = PvpPage.pvpPage.pvpResp.pvpInfo.rank;

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
	public void ChallengeReq (long tempId)
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
	}
	private PVP_CONFIRM_TYPE pvpConfirmType = PVP_CONFIRM_TYPE.PVP_BUY_CISHU;

	private ConfirmExecuteResp confirmResp;//百战操作返回

	private int canGetWeiWang;//当前可领取的威望
	public int CanGetWeiWang
	{
		set{canGetWeiWang = value;}
		get{return canGetWeiWang;}
	}

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
//		Debug.Log ("ConfirmReq:" + ProtoIndexes.CONFIRM_EXECUTE_REQ);
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

					Global.m_isOpenBaiZhan = true;

					if (IsOpenPvpByBtn)
					{
						if (pvpObj == null)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_PAGE ),
							                        PvpLoadCallback );
						}
						else
						{
							pvpObj.SetActive (true);
//							PvpPage.pvpPage.sEffectControl.OnOpenWindowClick ();
							PvpPage.pvpPage.InItPvpPage (pvpInfo);

							// ui 2d tool
							{
								UI2DTool.Instance.AddTopUI( GameObjectHelper.GetRootGameObject( pvpObj ) );
							}
						}
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
								textStr = "\n\n元宝不足,是否跳转到充值？";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.TurnToVipPage);
								break;
							case 1://购买成功
								Debug.Log ("剩余次数：" + confirm.buyCiShuInfo.leftTimes);
								Debug.Log ("总次数：" + confirm.buyCiShuInfo.totalTimes);
								PvpDataReq ();
								textStr = "\n\n恭喜购买挑战次数成功！";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.confirmStr, null, 
								                 PvpPage.pvpPage.OpenSkillEffect);
								break;
							case 2://数据错误
								textStr = "\n\n购买失败，请重新购买！";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.confirmStr, null, 
								                 PvpPage.pvpPage.OpenSkillEffect);
								break;
							case 3://今日购买次数用尽
								int vipLevel = JunZhuData.Instance ().m_junzhuInfo.vipLv;
								if (vipLevel < 10)
								{
									textStr = "\n今日购买次数已用尽，充值到vip" + (vipLevel + 1) + "级可购买更多挑战次数\n\n是否跳转到充值？";
									Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
									                 QXComData.cancelStr, QXComData.confirmStr, 
									                 PvpPage.pvpPage.TurnToVipPage);
								}
								else
								{
									textStr = "\n今日购买次数已用尽...";
									Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
									                 QXComData.confirmStr, null, 
									                 PvpPage.pvpPage.OpenSkillEffect);
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
								textStr = "\n\n元宝不足,是否跳转到充值？";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.TurnToVipPage);
								break;
							case 1:
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_TITLE);
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_SUCCESS);
								textStr = "\n\n清除冷却成功！";
								PvpPage.pvpPage.pvpResp.cdYuanBao = confirm.cleanCDInfo.nextCDYB;
								PvpPage.pvpPage.pvpResp.pvpInfo.time = 0;
								PvpPage.pvpPage.CdTime = 0;
								PvpPage.pvpPage.ChallangeRules ();
								
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.confirmStr, null, 
								                 PvpPage.pvpPage.OpenSkillEffect);
								break;
							case 2:
								textStr = "\n\n清除冷却失败，请重新尝试！";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.confirmStr, null, 
								                 PvpPage.pvpPage.OpenSkillEffect);
								break;
							case 3:
								textStr = "\n达到vip" + PvpPage.pvpPage.pvpResp.canCleanCDvipLev + "级可清楚冷却\n\n是否跳转到充值？";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.TurnToVipPage);
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

							PvpPage.pvpPage.pvpResp.canGetweiWang = 0;
							PvpPage.pvpPage.pvpResp.hasWeiWang += CanGetWeiWang;
							PvpPage.pvpPage.InItMyRank ();
							//关闭按钮特效
							UIYindao.m_UIYindao.setCloseUIEff ();
							RewardData data = new RewardData (900011,CanGetWeiWang);
							GeneralRewardManager.Instance ().CreateReward (data);
						}
						else
						{
//							Debug.Log ("领取失败");

							textStr = "\n\n您已领取过该奖励！";
							Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
							                 QXComData.confirmStr, null, 
							                 PvpPage.pvpPage.OpenSkillEffect);//刷新百战奖励领取
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
								PvpPage.pvpPage.pvpResp.pvpInfo.rank = confirm.refreshMyInfo.junZhuRank;
								PvpPage.pvpPage.pvpResp.oppoList = confirm.refreshMyInfo.oppoList;
								PvpPage.pvpPage.InItMyRank ();
								PvpPage.pvpPage.OpponentsInfo ();
							}
						}
						else
						{
							//关闭挑战阵容页面，刷新百战信息

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

								PvpPage.pvpPage.pvpResp.oppoList = confirm.refreshOtherInfo.oppoList;
								PvpPage.pvpPage.OpponentsInfo ();
							}
						}
						else
						{
							//关闭挑战阵容页面，刷新百战信息

						}

						break;
					}
					case PVP_CONFIRM_TYPE.PVP_REFRESH_PLARER:
					{
						if (confirm.payChangeInfo.success == 1)
						{
							PvpPage.pvpPage.pvpResp.huanYiPiYB = confirm.payChangeInfo.nextHuanYiPiYB;
							PvpPage.pvpPage.pvpResp.oppoList = confirm.payChangeInfo.oppoList;
							PvpPage.pvpPage.OpponentsInfo ();
							textStr = "\n\n更换挑战对手成功!";
							Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
							                 QXComData.confirmStr, null, 
							                 PvpPage.pvpPage.OpenSkillEffect);
						}

						break;
					}
					}
				}

				return true;
			}
			case ProtoIndexes.ZhanDou_Notes_Resq://百战战斗记录返回
			{
				Debug.Log ("PvpRecordResp:" + ProtoIndexes.ZhanDou_Notes_Resq);
				
				ZhandouRecordResp pvpRecord = new ZhandouRecordResp ();
				pvpRecord = QXComData.ReceiveQxProtoMessage (p_message,pvpRecord) as ZhandouRecordResp;

				if (pvpRecord != null)
				{
					if (pvpRecord.info == null)
					{
						pvpRecord.info = new List<ZhandouItem>();
					}
					PvpPage.pvpPage.LoadPvpRecordPrefab (pvpRecord);
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

							EnterBattleField.EnterBattlePvp (enemyId);

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
							if (PvpPage.pvpPage.pvpResp.leftCanBuyCount == 0)
							{
								if (JunZhuData.Instance ().m_junzhuInfo.vipLv < 10)
								{
									textStr = "\n今日挑战机会已用完,充值到vip" + (JunZhuData.Instance ().m_junzhuInfo.vipLv + 1) + "级可购买更多挑战次数\n\n是否跳转到充值？";
									Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
									                 QXComData.cancelStr, QXComData.confirmStr, 
									                 PvpPage.pvpPage.TurnToVipPage);
								}
								else
								{
									textStr = "\n\n今日挑战机会已用完...";
									Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
									                 QXComData.confirmStr, null, 
									                 PvpPage.pvpPage.OpenSkillEffect);
								}
							}
							else
							{
								textStr = "\n今日挑战机会已用完\n\n是否使用" + PvpPage.pvpPage.pvpResp.buyNeedYB + "元宝购买" + PvpPage.pvpPage.pvpResp.buyNumber + "次挑战次数？\n\n今日还可购买" + PvpPage.pvpPage.pvpResp.leftCanBuyCount + "回";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.BuyTimes);
							}
						}
						else if (stateRes.type == 5)
						{
							//冷却cd
							//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_DISCD);
							int cleanCdVipLevel = PvpPage.pvpPage.pvpResp.canCleanCDvipLev;
							if (JunZhuData.Instance ().m_junzhuInfo.vipLv < cleanCdVipLevel)
							{
								textStr = "\n挑战冷却中,达到vip" + cleanCdVipLevel + "级可清除冷却\n\n是否跳转到充值？";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.TurnToVipPage);
							}
							else
							{
								textStr = "\n\n挑战冷却中,是否使用" + PvpPage.pvpPage.pvpResp.cdYuanBao + "元宝清除挑战冷却？";
								Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
								                 QXComData.cancelStr, QXComData.confirmStr, 
								                 PvpPage.pvpPage.ClearCd);
							}
						}
						else
						{
							switch (stateRes.type)
							{
							case 2://玩家正在被挑战
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_BGING_CHALLENGED);
								//LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_WAIT_TRY);
								textStr = "\n\n这位玩家正在被挑战\n\n请稍后再做尝试...";
								break;
							case 3://君主等级不足
								textStr = "\n\n君主等级不足，无法挑战...";
								break;
							case 6://数据出错
								textStr = "\n\n挑战失败，请稍后再做尝试...";
								break;
							case 7://对手名次发生变化
								textStr = "\n\n对手名次发生变化，请重新选择挑战对手！";
								break;
							case 8://自己的名次已发生变化
								textStr = "\n\n您的名次已发生变化！";
								break;
							default:
								break;
							}
							
							Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
							                 QXComData.confirmStr, null, 
							                 PlayerStateCheckCallBack);
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

					Global.m_isOpenBaiZhan = true;

					if (pvpObj != null)
					{
						PvpPage.pvpPage.CancelBtn ();
					}

					if (IsOpenPvpByBtn)
					{
						if (challengeObj == null)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_CHALLENGE_PAGE ),
							                        ChallengeLoadCallback );
						}
						else
						{
							challengeObj.SetActive (true);
							
							GeneralTiaoZhan tiaoZhan = challengeObj.GetComponent<GeneralTiaoZhan> ();
							tiaoZhan.InItPvpChallengePage (GeneralTiaoZhan.ZhenRongType.PVP,PvpChallengeResp);
						}
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

		PvpPage.pvpPage.InItPvpPage (pvpInfoResp);

		MainCityUI.TryAddToObjectList(pvpObj);
	}

	//挑战验证失败回调
	private void PlayerStateCheckCallBack (int i)
	{
		if (playerState == PlayerState.STATE_PVP_MAIN_PAGE)
		{
			PvpPage.pvpPage.OpenSkillEffect (2);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,3);
			switch (PlayerStateCheckFaillType)
			{
			case 2:
				//对手正在被挑战
				ConfirmReq (PVP_CONFIRM_TYPE.PVP_REFRESH_OPPO_RANK);//刷新对手名次
				break;
			case 3:
				//关闭百战
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
			GeneralTiaoZhan tiaoZhan = challengeObj.GetComponent<GeneralTiaoZhan> ();
			tiaoZhan.PvpReset = true;
			tiaoZhan.DestroyRoot ();
		}
	}
	
	void ChallengeLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		challengeObj = Instantiate (p_object) as GameObject;

		MainCityUI.TryAddToObjectList (challengeObj);

		GeneralTiaoZhan tiaoZhan = challengeObj.GetComponent<GeneralTiaoZhan> ();
		
		if (PvpChallengeResp.myZuheId > 0)
		{
			tiaoZhan.YinDaoState = 2;
		}
		else
		{
//			List<MibaoGroup> mibaoGroup = MiBaoGlobleData.Instance ().G_MiBaoInfo.mibaoGroup;
//			for(int i = 0;i < mibaoGroup.Count;i ++)
//			{
//				if (mibaoGroup[i].hasActive == 1)
//				{
//					tiaoZhan.YinDaoState = 1;
//					break;
//				}
//				else
//				{
//					tiaoZhan.YinDaoState = 2;
//				}
//			}
		}
		tiaoZhan.InItPvpChallengePage (GeneralTiaoZhan.ZhenRongType.PVP,PvpChallengeResp);
	}
	#endregion

	//打开百战
	public void OpenPvp ()
	{
		IsOpenPvpByBtn = true;
		PvpDataReq ();
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
