using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LueDuoData : Singleton<LueDuoData>,SocketProcessor {

	public LveDuoInfoResp lueDuoData;

	private LveConfirmResp confirmResp;

	private LveGoLveDuoResp ldOpponentRes;

	private int mStartPage = 1;//联盟初始页
	public int SetAllianceStartPage
	{
		set{mStartPage = value;}
	}
	public int GetAllianceStartPage
	{
		get{return mStartPage;}
	}

	public enum ReqType//请求类型
	{
		Alliance,//联盟
		JunZhu,//对手君主
	}
	private ReqType reqType = ReqType.Alliance;

	public enum Direction//滑动方向
	{
		Up,//向上滑
		Default,//不滑动
		Down,//向下滑
	}
	private Direction direct = Direction.Default;
	
	public enum BtnMakeType//按钮操作类型
	{
		ClearCd,//清除cd
		AddNum,//增加次数
	}
	private BtnMakeType btnType = BtnMakeType.ClearCd;
	public BtnMakeType SetBtnType
	{
		set{btnType = value;}
	}
	public BtnMakeType GetBtnType
	{
		get{return btnType;}
	}

	private int ldOpponentType;

	/// <summary>
	/// 当前国家id
	/// </summary>
	private int nationId = -1;
	public int SetNationId
	{
		set{nationId = value;}
	}
	public int GetNationId
	{
		get{return nationId;}
	}

	/// <summary>
	/// 要刷新的君主所在国家id
	/// </summary>
	private int junNationId = -1;
	public int JunNationId
	{
		set{junNationId = value;}
		get{return junNationId;}
	}

	/// <summary>
	/// 当前选择的联盟id
	/// </summary>
	private int allianceId = -1;
	public int SetAllianceId
	{
		set{allianceId = value;}
	}
	public int GetAllianceId
	{
		get{return allianceId;}
	}

	public enum WhichOpponent
	{
		LUE_DUO,//掠夺
		RANKLIST,//排行
		CHAT,//聊天
	}
	private WhichOpponent whichType = WhichOpponent.LUE_DUO;
	public WhichOpponent GetWhichType
	{
		get{return whichType;}
	}

	private bool isStop = false;//是否点击了某个按钮
	public bool IsStop
	{
		set{isStop = value;}
		get{return isStop;}
	}

	public string titleStr;
	public string textStr;
	public string cancelStr;
	public string confirmStr;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		titleStr = "提示";
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}

	/// <summary>
	/// 掠夺信息请求
	/// </summary>
	public void LueDuoInfoReq ()
	{	
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LVE_DUO_INFO_REQ,"26061");
//		Debug.Log ("掠夺信息请求:" + ProtoIndexes.LVE_DUO_INFO_REQ);
	}

	/// <summary>
	/// 掠夺下个页面请求
	/// </summary>
	public void LueDuoNextReq (ReqType type,int guoJiaId,int allianceId,int pageNumber,Direction direction)
	{
		LveNextItemReq lueDuoNextReq = new LveNextItemReq();

		reqType = type;
		switch (type)
		{
		case ReqType.Alliance:
			
			lueDuoNextReq.rankType = 1;

			lueDuoNextReq.guojiaId = guoJiaId;

			{
				direct = direction;
				switch (direction)
				{
				case Direction.Default://切换国家，默认联盟第一页
					
					lueDuoNextReq.pageNo = pageNumber; 
					
					break;
					
				case Direction.Up://不切换国家，向上滑，请求下一页数据
					
					lueDuoNextReq.pageNo = pageNumber + 1;
					
					break;
					
				case Direction.Down://不切换国家，向下滑，请求上一页数据
					
					lueDuoNextReq.pageNo = pageNumber - 1;
					
					break;
				}
				
//				mStartPage = lueDuoNextReq.pageNo;
			}

//			Debug.Log ("lueDuoNextReq.guojiaId:" + lueDuoNextReq.guojiaId);
//			Debug.Log ("lueDuoNextReq.pageNo:" + LueDuoData.Instance.GetAllianceStartPage);

			break;
			
		case ReqType.JunZhu:
			
			lueDuoNextReq.rankType = 2;

			lueDuoNextReq.mengId = allianceId;

//			Debug.Log ("lueDuoNextReq.mengId:" + lueDuoNextReq.mengId);

			break;
		}

//		Debug.Log ("lueDuoNextReq.rankType:" + lueDuoNextReq.rankType);

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,lueDuoNextReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LVE_NEXT_ITEM_REQ,ref t_protof,"26070");

//		Debug.Log ("下个掠夺页面请求:" + ProtoIndexes.LVE_NEXT_ITEM_REQ);
	}

	/// <summary>
	/// 清除冷却cd或增加次数请求
	/// </summary>
	/// <param name="reqType">Req type：1-清除cd 2-购买次数</param>
	public void LueDuoConfirmReq (int reqType)
	{
		LveConfirmReq confirmReq = new LveConfirmReq();

		confirmReq.doType = reqType;

		switch (reqType)
		{
		case 1:

			btnType = BtnMakeType.ClearCd;

			break;

		case 2:

			btnType = BtnMakeType.AddNum;

			break;
		default:
			break;
		}

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,confirmReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LVE_CONFIRM_REQ,ref t_protof,"26063");
//		Debug.Log ("清除cd或增加次数请求:" + ProtoIndexes.LVE_CONFIRM_REQ);
	}

	/// <summary>
	/// 掠夺对手请求
	/// </summary>
	/// <param name="tempId">对手君主id</param>
	public void LueDuoOpponentReq (long tempId,WhichOpponent type)
	{
		if (JunZhuData.Instance ().m_junzhuInfo.lianMengId <= 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
			                        HaveNoAllianceLoadCallBack);
			return;
		}

		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (211);

//		Debug.Log ("Level:" + functionTemp.Level);

		if (JunZhuData.Instance ().m_junzhuInfo.level >= functionTemp.Level)
		{
			whichType = type;
			
			LveGoLveDuoReq lueDuoOpponentReq = new LveGoLveDuoReq ();
			
			lueDuoOpponentReq.enemyId = tempId;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize (t_stream,lueDuoOpponentReq);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LVE_GO_LVE_DUO_REQ,ref t_protof,"26067");
					
//			Debug.Log ("掠夺对手请求:" + ProtoIndexes.LVE_GO_LVE_DUO_REQ);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
			                        OpenLueDuoCallBack);
		}
	}

	private void OpenLueDuoCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (211);

		textStr = "\n\n达到" + functionTemp.Level + "级可开启掠夺功能！";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,null);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LVE_DUO_INFO_RESP://掠夺信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LveDuoInfoResp lueDuoRes = new LveDuoInfoResp();
				
				t_qx.Deserialize(t_stream, lueDuoRes, lueDuoRes.GetType());

				if (lueDuoRes != null)
				{
//					Debug.Log ("已掠夺次数：" + lueDuoRes.used);
//					Debug.Log ("总掠夺次数：" + lueDuoRes.all);
//					Debug.Log ("是否有新的战斗记录：" + lueDuoRes.hasRecord );
//					Debug.Log ("掠夺cdTime：" + lueDuoRes.CdTime);
//					Debug.Log ("最大可掠夺次数：" + lueDuoRes.nowMaxBattleCount);
//					Debug.Log ("贡金：" + lueDuoRes.gongJin);
//					Debug.Log ("国家id：" + lueDuoRes.showGuoId);
//					Debug.Log ("可以清除冷却的vip等级：" + lueDuoRes.canClearCdVIP);
//					Debug.Log ("购买下次掠夺所花费的元宝数：" + lueDuoRes.buyNextBattleYB);
//					Debug.Log ("下次可购买的掠夺次数：" + lueDuoRes.buyNextBattleCount);
//					Debug.Log ("清除cd需要花费的元宝：" + lueDuoRes.clearCdYB);

					if (lueDuoRes.guoLianInfos == null)
					{
						lueDuoRes.guoLianInfos = new List<GuoInfo>();
					}
					else
					{
//						Debug.Log ("国家：" + lueDuoRes.guoLianInfos.Count);
					}

					if (lueDuoRes.junInfos == null)
					{
						lueDuoRes.junInfos = new List<JunZhuInfo>();
					}
					else
					{
//						Debug.Log ("君主：" + lueDuoRes.junInfos.Count);
					}

					if (lueDuoRes.mengInfos == null)
					{
						lueDuoRes.mengInfos = new List<LianMengInfo>();
					}
					else
					{
//						Debug.Log ("联盟个数：" + lueDuoRes.mengInfos.Count);
					}

					mStartPage = 1;

					lueDuoData = lueDuoRes;
					CheckNewRecord (lueDuoData.hasRecord);
					GameObject lueDuoObj = GameObject.Find ("LueDuo");

					if (lueDuoObj != null)
					{
						LueDuoManager ldManager = lueDuoObj.GetComponent<LueDuoManager> ();
						ldManager.GetLueDuoData (lueDuoRes);
					}
				}

				return true;
			}

			case ProtoIndexes.LVE_NEXT_ITEM_RESP://下个掠夺页面返回
			{
				Debug.Log ("下个掠夺页面返回:" + ProtoIndexes.LVE_NEXT_ITEM_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LveNextItemResp lueDuoNextRes = new LveNextItemResp();
				
				t_qx.Deserialize(t_stream, lueDuoNextRes, lueDuoNextRes.GetType());

				if (lueDuoNextRes != null)
				{
					if (lueDuoNextRes.junList == null)
					{
						lueDuoNextRes.junList = new List<JunZhuInfo>();
					}
					if (lueDuoNextRes.mengList == null)
					{
						lueDuoNextRes.mengList = new List<LianMengInfo>();
					}
					Debug.Log ("联盟个数：" + lueDuoNextRes.mengList.Count);
					Debug.Log ("君主个数：" + lueDuoNextRes.junList.Count);

					IsStop = false;

					GameObject lueDuoObj = GameObject.Find ("LueDuo");
					
					if (lueDuoObj != null)
					{
						LueDuoManager ldManager = lueDuoObj.GetComponent<LueDuoManager> ();
//						Debug.Log ("请求类型：" + reqType);
						switch (reqType)
						{
						case ReqType.Alliance:
						{
							{
								switch (direct)
								{
								case Direction.Default:
								{
									mStartPage = 1;
									ldManager.RefreshLueDuoInfo (ReqType.Alliance,
									                             lueDuoNextRes.mengList,
									                             lueDuoNextRes.junList);
									break;
								}
								case Direction.Up:
								{
									if (lueDuoNextRes.mengList.Count == 0)
									{
										ldManager.SetLdAllianceCanTap ();
									}
									else
									{
										mStartPage += 1;

										ldManager.RefreshLueDuoInfo (ReqType.Alliance,
										                             lueDuoNextRes.mengList,
										                             lueDuoNextRes.junList);
									}

									break;
								}
								case Direction.Down:
								{
									if (lueDuoNextRes.mengList.Count == 0)
									{
										ldManager.SetLdAllianceCanTap ();
									}
									else
									{
										mStartPage -= 1;

										ldManager.RefreshLueDuoInfo (ReqType.Alliance,
										                             lueDuoNextRes.mengList,
										                             lueDuoNextRes.junList);
									}

									break;
								}
								default:
									break;
								}
							}

							{
//								if (mStartPage > 1)
//								{
//									if (lueDuoNextRes.mengList.Count > 0)
//									{
//										LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.JunZhu,
//										                                   lueDuoNextRes.mengList[0].guoJiaId,
//										                                   lueDuoNextRes.mengList[0].mengId,
//										                                   mStartPage,
//										                                   LueDuoData.Direction.Default);
//									}
//								}
							}

							break;
						}
						case ReqType.JunZhu:
						{
							ldManager.RefreshLueDuoInfo (ReqType.JunZhu,null,
							                                           lueDuoNextRes.junList);

							break;
						}
						default:
							break;
						}
					}
				}

				return true;
			}
			case ProtoIndexes.LVE_CONFIRM_RESP://清除冷却或增加次数返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LveConfirmResp confirmRes = new LveConfirmResp();
				
				t_qx.Deserialize(t_stream, confirmRes, confirmRes.GetType());

				if (confirmRes != null)
				{
//					Debug.Log ("返回类型：" + confirmRes.isOk);
					confirmResp = confirmRes;

					if (confirmRes.isOk == 1)
					{
						GameObject lueDuoObj = GameObject.Find ("LueDuo");

						if (lueDuoObj != null)
						{
							LueDuoManager ldManager = lueDuoObj.GetComponent<LueDuoManager> ();
							lueDuoData.CdTime = confirmRes.leftCD;
							lueDuoData.clearCdYB = confirmRes.nextCDYuanBao;
							lueDuoData.all = confirmRes.all;
							lueDuoData.used = confirmRes.used;
							lueDuoData.gongJin = confirmRes.gongJin;
							lueDuoData.buyNextBattleCount = confirmRes.buyNextBattleCount;
							lueDuoData.buyNextBattleYB = confirmRes.buyNextBattleYB;
							lueDuoData.remainBuyHuiShi = confirmRes.remainBuyHuiShi;
							lueDuoData.nowMaxBattleCount = confirmRes.nowMaxBattleCount;
							ldManager.SetLueDuoInfoState (lueDuoData);

//							switch (btnType)
//							{
//							case BtnMakeType.ClearCd:
//							{
//								Debug.Log ("清除冷却成功");
//
//								break;
//							}
//							case BtnMakeType.AddNum:
//							{
//								Debug.Log ("增加次数成功");
//
//								lueDuoData.CdTime = confirmRes.leftCD;
//								lueDuoData.clearCdYB = confirmRes.nextCDYuanBao;
//								lueDuoData.all = confirmRes.all;
//								lueDuoData.used = confirmRes.used;
//								
//								ldManager.SetLueDuoInfoState (lueDuoData);
//								
//								break;
//							}
//							default:
//								break;
//							}
						}
					}

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
					                        BoxLoadCallBack);
				}

				return true;
			}
			case ProtoIndexes.LVE_GO_LVE_DUO_RESP://请求掠夺对手返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LveGoLveDuoResp lueDuoOpponentRes = new LveGoLveDuoResp();
				
				t_qx.Deserialize(t_stream, lueDuoOpponentRes, lueDuoOpponentRes.GetType());

				if (lueDuoOpponentRes != null)
				{
//					Debug.Log ("请求掠夺对手结果：" + lueDuoOpponentRes.isCanLveDuo);

					ldOpponentRes = lueDuoOpponentRes;
					ldOpponentType = lueDuoOpponentRes.isCanLveDuo;

					if (lueDuoOpponentRes.isCanLveDuo == 7)
					{
						//跳转到对战阵容页面
						GameObject challengeObj = GameObject.Find ("ChallengePage");
						if (challengeObj != null)
						{
							GeneralTiaoZhan tiaoZhan = challengeObj.GetComponent<GeneralTiaoZhan> ();
							
							tiaoZhan.InItLueDuoChallengePage (GeneralTiaoZhan.ZhenRongType.LUE_DUO,lueDuoOpponentRes);
						}
						else
						{
							CloneGeneralTiaoZhan ();
						}
					}
					else
					{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
						                        LueDuoOpponentCallBack);
					}
				}

				return true;
			}
			case ProtoIndexes.LVE_NOTICE_CAN_LVE_DUO:
			{
				//掠夺新纪录返回

				CheckNewRecord (true);
				GameObject lueDuoObj = GameObject.Find ("LueDuo");
				if (lueDuoObj != null)
				{
					LueDuoManager lueDuo = lueDuoObj.GetComponent<LueDuoManager> ();
					lueDuo.CheckNewRecord (true);
				}

				return true;
			}
			}
		}
		return false;
	}

	//实例化挑战页面
	void CloneGeneralTiaoZhan ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_CHALLENGE_PAGE ),
		                        ChallengeLoadCallback );
	}
	
	void ChallengeLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject challenge = Instantiate (p_object) as GameObject;
//		challenge.name = "GeneralChallengePage";
//
//		switch (whichType)
//		{
//		case WhichOpponent.LUE_DUO:
//		{
//			GameObject lueDuoObj = GameObject.Find ("LueDuo");
//			if (lueDuoObj != null)
//			{
//				challenge.transform.parent = lueDuoObj.transform;
//				challenge.transform.localPosition = Vector3.zero;
//			}
//			break;
//		}
//		case WhichOpponent.RANKLIST:
//		{
//			GameObject kingDetalInfo = GameObject.Find ("KingDetailInfoWindow(Clone)");
//			if (kingDetalInfo)
//			{
//				challenge.transform.parent = kingDetalInfo.transform;
//				challenge.transform.localPosition = new Vector3(0,0,-500);
//			}
//			else
//			{
//				GameObject allianceMember = GameObject.Find ("AllianceMemberWindow(Clone)");
//				if (allianceMember)
//				{
//					challenge.transform.parent = allianceMember.transform;
//					challenge.transform.localPosition = Vector3.zero;
//				}
//				else
//				{
//					GameObject rankObj = GameObject.Find ("RankWindow(Clone)");
//					if (rankObj != null)
//					{
//						challenge.transform.parent = rankObj.transform;
//						challenge.transform.localPosition = Vector3.zero;
//					}
//				}
//			}
//			break;
//		}
//		case WhichOpponent.CHAT:
//		{
//			GameObject chatObj = GameObject.Find ("UIChat(Clone)");
//			if (chatObj)
//			{
//				challenge.transform.parent = chatObj.transform;
//				challenge.transform.localPosition = Vector3.zero;
//			}
//			break;
//		}
//		default:
//			break;
//		}
//	
//		challenge.transform.localScale = Vector3.one;
		
		GeneralTiaoZhan tiaoZhan = challenge.GetComponent<GeneralTiaoZhan> ();
		
		tiaoZhan.InItLueDuoChallengePage (GeneralTiaoZhan.ZhenRongType.LUE_DUO,ldOpponentRes);
	}

	private void BoxLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		switch (confirmResp.isOk)
		{
		case 0:
		{
			textStr = "\n您的元宝不足！\n现在就去充值！";
//			TopUpLoadManagerment.m_instance.LoadPrefab(true);
			break;
		}
		case 1:
		{
			switch (btnType)
			{
			case BtnMakeType.ClearCd:
				
				textStr = "\n\n清除冷却成功！";
				
				break;
				
			case BtnMakeType.AddNum:

				textStr = "\n\n购买成功！";

				break;
			default:
				break;
			}
			break;
		}
		case 2:
		{
			textStr = "\n您的VIP等级不够，\n达到VIP" + lueDuoData.canClearCdVIP + 
				"级可清除冷却！";

			break;
		}
		case 3:
		{
			textStr = "\n\n购买失败！";

			break;
		}
		case 6:
		{
			textStr = "\n\n购买失败！\n今日掠夺次数已用尽！";

			break;
		}
		default:
			break;
		}

		uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,BoxLoadBack);
	}

	void BoxLoadBack (int i)
	{
		LueDuoManager.ldManager.ShowChangeSkillEffect (true);
	}

	private void LueDuoOpponentCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		var junZhuInfo = JunZhuData.Instance ().m_junzhuInfo;

		if (ldOpponentType == 2)
		{
			if (junZhuInfo.vipLv < lueDuoData.canClearCdVIP)
			{
				textStr = "\n掠夺冷却中！\n达到VIP" + lueDuoData.canClearCdVIP + "级可清除冷却！";
				
				uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,CantClearCd);
			}
			else
			{
				textStr = "\n掠夺冷却中！\n是否使用" + lueDuoData.clearCdYB + "元宝清除冷却时间？";
				uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,cancelStr,confirmStr,ClearCdCallBack);
			}
		}
		else if (ldOpponentType == 3)
		{
			if (lueDuoData.all < lueDuoData.nowMaxBattleCount)
			{
				textStr = "\n掠夺次数用尽！\n" + "是否使用" + lueDuoData.buyNextBattleYB + "元宝购买" +
					lueDuoData.buyNextBattleCount + "次掠夺机会？\n今日还可购买" + lueDuoData.remainBuyHuiShi + "次";
				
				uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,cancelStr,confirmStr,AddNumCallBack);
			}
			else
			{
				if (JunZhuData.Instance ().m_junzhuInfo.vipLv < 10)
				{
					textStr = "\n\n今日掠夺次数已用尽！\n" + "升级到VIP" + (JunZhuData.Instance ().m_junzhuInfo.vipLv + 1)
						+ "级可购买更多掠夺次数！";
				}
				else
				{
					textStr = "\n\n今日掠夺次数已用尽！";
				}
				
				uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,cancelStr,confirmStr,LackYBBack);
			}
		}
		else if (ldOpponentType == 4)
		{

			textStr = "\n这位玩家正在掠夺保护期！\n请稍后再做尝试...";

			uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,RefreshProtectJunZhu);
		}
		else
		{
			switch (ldOpponentType)
			{
			case -1:

				textStr = "\n\n无法掠夺自己！";

				break;

			case 0:

				string openStr = CanshuTemplate.GetStrValueByKey (CanshuTemplate.OPENTIME_LUEDUO);
				string closeStr = CanshuTemplate.GetStrValueByKey (CanshuTemplate.CLOSETIME_LUEDUO);

				textStr = "\n掠夺活动未开启，\n开启时间：" + openStr + "-" + closeStr;
				
				break;
				
			case 1:
				
				textStr = "\n不能掠夺自己的盟友！";
				
				break;
				
			case 5:
				
				textStr = "\n这位玩家正在被掠夺！\n请稍后再做尝试...";
				
				break;
				
			case 6:
				
				textStr = "\n所选玩家不存在...\n请选择其他玩家";
				
				break;

			case 8:

				textStr = "\n\n对手还没有开启掠夺！";

				break;

			default:
				break;
			}

			uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,null);
		}
	}

	//不能清除冷却cd
	void CantClearCd (int i)
	{
		IsStop = false;
		LueDuoManager.ldManager.ShowChangeSkillEffect (true);
	}

	/// <summary>
	/// 清除冷却回调
	/// </summary>
	public void ClearCdCallBack (int i)
	{
		IsStop = false;
		if (i == 2)
		{
			LueDuoManager.ldManager.ShowChangeSkillEffect (false);
			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao < lueDuoData.clearCdYB)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
				                        LackYBLoadCallBack);
			}
			else
			{
				LueDuoConfirmReq (1);
			}
		}
		else
		{
			LueDuoManager.ldManager.ShowChangeSkillEffect (true);
		}
	}

	private void LackYBLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		textStr = "\n您的元宝不足！\n现在就去充值！";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,cancelStr,confirmStr,LackYBBack);
	}
	
	void LackYBBack (int i)
	{
		IsStop = false;
		if (i == 2)
		{
			GameObject lueDuoObj = GameObject.Find ("LueDuo");
			if (lueDuoObj != null)
			{
				LueDuoManager lueDuo = lueDuoObj.GetComponent<LueDuoManager> ();
				lueDuo.IsOpenChongZhi = true;//打开充值
				lueDuo.DestroyRoot ();
			}
			else
			{
				//关闭排行榜，打开充值
				Debug.Log ("关闭排行榜，打开充值");
			}
		}
		else
		{
			LueDuoManager.ldManager.ShowChangeSkillEffect (true);
		}
	}

	/// <summary>
	/// 增加次数回调
	/// </summary>
	public void AddNumCallBack (int i)
	{
		IsStop = false;
		if (i == 2)
		{
			LueDuoManager.ldManager.ShowChangeSkillEffect (false);
			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao < lueDuoData.buyNextBattleYB)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
				                        LackYBLoadCallBack);
			}
			else
			{
				LueDuoConfirmReq (2);
			}
		}
		else
		{
			LueDuoManager.ldManager.ShowChangeSkillEffect (true);
		}
	}

	/// <summary>
	/// 刷新当前页面君主信息
	/// </summary>
	void RefreshProtectJunZhu (int i)
	{
		IsStop = false;
		if (junNationId == nationId)
		{
			LueDuoNextReq (LueDuoData.ReqType.JunZhu,
			               nationId,
			               allianceId,
			               LueDuoData.Instance.GetAllianceStartPage,
			               LueDuoData.Direction.Default);
		}
	}

	/// <summary>
	/// 显示时间格式
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="time">Time.</param>
	public string TimeStr (float time)
	{
		string minuteStr = "";
		string secondStr = "";

		int minute = (int)((time / 60) % 60);
		if (minute < 10)
		{
			minuteStr = "0" + minute;
		}
		else
		{
			minuteStr = minuteStr.ToString ();
		}

		int second = (int)(time % 60);
		if (second < 10) 
		{
			secondStr = "0" + second;
		} 
		else 
		{
			secondStr = second.ToString ();
		}
		
		return minute + "：" + secondStr;
	}

	/// <summary>
	/// 检测是否有新的掠夺记录
	/// </summary>
	public void CheckNewRecord (bool newRecord)
	{
		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (211);
		
		MainCityUIRB.SetRedAlert (functionTemp.m_iID,newRecord);
	}

	private void HaveNoAllianceLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		textStr = "\n\n您还没有联盟，请先加入一个联盟后再来掠夺对手！";
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,textStr),null,null,confirmStr,null,HaveNoAllianceCallBack);
	}

	void HaveNoAllianceCallBack (int i)
	{
		GameObject lueDuoObj = GameObject.Find ("LueDuo");
		
		if (lueDuoObj != null)
		{
			LueDuoManager ldManager = lueDuoObj.GetComponent<LueDuoManager> ();
			ldManager.ShowChangeSkillEffect (true);
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
