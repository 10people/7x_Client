using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarData : Singleton<CityWarData>,SocketProcessor {

	private CityFightInfoResp m_cityResp;

	private GameObject m_cityObj;

	private string m_text;

	private bool m_isOpenCityWar = false;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	#region CityWarMainInfo
	public enum CityWarType
	{
		NORMAL_CITY = 1,
		OTHER_CITY = 2,
	}

	public void OpenCityWar (CityWarType tempType)
	{
		CityFightInfoReq cityReq = new CityFightInfoReq ();
		cityReq.type = (int)tempType;
		QXComData.SendQxProtoMessage (cityReq,ProtoIndexes.C_ALLIANCE_CITYFIGHTINFO_REQ,ProtoIndexes.S_ALLIANCE_CITYFIGHTINFO_RESP.ToString ());
//		Debug.Log ("郡城战请求：" + ProtoIndexes.C_ALLIANCE_CITYFIGHTINFO_REQ);
	}
	#endregion

	#region CityWarReward
	public enum CW_RewardType
	{
		ALLIANCE = 0,
		PERSONAL = 1,
	}
	private CW_RewardType m_rewardType;

	public void RewardReq (CW_RewardType tempType)
	{
		m_rewardType = tempType;

		CityWarRewardReq cwRewardReq = new CityWarRewardReq ();
		cwRewardReq.rewardType = (int)tempType;
		QXComData.SendQxProtoMessage (cwRewardReq,ProtoIndexes.C_CITYWAR_REWARD_REQ,ProtoIndexes.S_CITYWAR_REWARD_RESP.ToString ());
//		Debug.Log ("郡城战奖励信息请求：" + ProtoIndexes.C_CITYWAR_REWARD_REQ);
	}
	#endregion

	#region CityWarBrand
	public int M_BrandPage;
	public void BrandReq (int tempPage)
	{
		M_BrandPage = tempPage;
		CityWarRewardReq brandReq = new CityWarRewardReq ();
		brandReq.rewardType = tempPage;
		QXComData.SendQxProtoMessage (brandReq,ProtoIndexes.C_CITYWAR_GRAND_REQ,ProtoIndexes.S_CITYWAR_GRAND_RESP.ToString ());
//		Debug.Log ("郡城战报信息请求：" + ProtoIndexes.C_CITYWAR_GRAND_REQ);
	}

	#endregion

	#region CityWarBid

	private CityInfo m_targetCityInfo;

	public void BidReq (CityInfo tempInfo)
	{
		m_targetCityInfo = tempInfo;
		CityWarBidReq bidReq = new CityWarBidReq ();
		bidReq.cityId = tempInfo.cityId;
		QXComData.SendQxProtoMessage (bidReq,ProtoIndexes.C_CITYWAR_BID_REQ,ProtoIndexes.S_CITYWAR_BID_RESP.ToString ());
//		Debug.Log ("竞拍页面请求：" + ProtoIndexes.C_CITYWAR_BID_REQ);
	}

	#endregion

	#region CityWarOperate

	private CityWarOperateReq m_operateInfo;
	private int m_getNum;//领取的奖励数

	public void CityOperate (CityWarOperateReq tempReq,int tempRewardNum = 0)
	{
		m_operateInfo = tempReq;
		m_getNum = tempRewardNum;
//		Debug.Log ("m_operateInfo.cityId:" + m_operateInfo.cityId);
		QXComData.SendQxProtoMessage (tempReq,ProtoIndexes.C_CITYWAR_OPERATE_REQ,ProtoIndexes.S_CITYWAR_OPERATE_RESP.ToString ());
//		Debug.Log ("操作请求：" + ProtoIndexes.C_CITYWAR_OPERATE_REQ);
	}

	#endregion

	#region CityWarJiFen
	private OtherCity.PageType m_pageType;

	public void CityWarJiFenReq (OtherCity.PageType tempType,int tempCityId)
	{
		m_pageType = tempType;
		CityWarScoreResultReq scoreReq = new CityWarScoreResultReq ();
		scoreReq.cityId = tempCityId;
		QXComData.SendQxProtoMessage (scoreReq,ProtoIndexes.C_CITYWAR_SCORE_RESULT_REQ,ProtoIndexes.S_CITYWAR_SCORE_RESULT_RESP.ToString ());
//		Debug.Log ("郡城战积分请求：" + ProtoIndexes.C_CITYWAR_SCORE_RESULT_REQ);
	}

	#endregion

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ALLIANCE_CITYFIGHTINFO_RESP:
			{
				CityFightInfoResp cityResp = new CityFightInfoResp();
				cityResp = QXComData.ReceiveQxProtoMessage (p_message,cityResp) as CityFightInfoResp;
//				Debug.Log ("ProtoIndexes.S_ALLIANCE_CITYFIGHTINFO_RESP:" + ProtoIndexes.S_ALLIANCE_CITYFIGHTINFO_RESP);
				if (cityResp != null)
				{
					if (cityResp.cityList == null)
					{
						cityResp.cityList = new List<CityInfo>();
					}
					if (cityResp.bidList == null)
					{
						cityResp.bidList = new List<BidMsgInfo> ();
					}
//					Debug.Log ("CityResp.cityList:" + cityResp.cityList.Count);
//					Debug.Log ("CityResp.bidList:" + cityResp.bidList.Count);

					m_cityResp = cityResp;

					if (QXComData.JunZhuInfo ().lianMengId <= 0)
					{
						//无联盟
						return false;
					}
					CityWarType cityWarType = (CityWarType)Enum.ToObject (typeof(CityWarType),m_cityResp.type);
					switch (cityWarType)
					{
					case CityWarType.NORMAL_CITY:
//						Debug.Log ("CityResp.interval:" + cityResp.interval);
//						Debug.Log ("CityResp.myCityCount:" + cityResp.myCityCount);
						LoadCityWarObj ();
						break;
					case CityWarType.OTHER_CITY:
						CityWarPage.m_instance.OpenOtherCity (m_cityResp);
						break;
					default:
						break;
					}
				}

				return true;
			}
			case ProtoIndexes.S_CITYWAR_REWARD_RESP:
			{
				CityWarRewardResp cityRewardResp = new CityWarRewardResp();
				cityRewardResp = QXComData.ReceiveQxProtoMessage (p_message,cityRewardResp) as CityWarRewardResp;
//				Debug.Log ("奖励页面返回:" + ProtoIndexes.S_CITYWAR_REWARD_RESP);
				if (cityRewardResp != null)
				{
					if (cityRewardResp.rewardList == null)
					{
						cityRewardResp.rewardList = new List<CityWarRewardInfo>();
					}

					CityWarPage.m_instance.OpenReward (m_rewardType,cityRewardResp);
				}
				
				return true;
			}
			case ProtoIndexes.S_CITYWAR_GRAND_RESP :
			{
				CityWarGrandResp cityBrandResp = new CityWarGrandResp();
				cityBrandResp = QXComData.ReceiveQxProtoMessage (p_message,cityBrandResp) as CityWarGrandResp;
//				Debug.Log ("战报返回:" + ProtoIndexes.S_CITYWAR_GRAND_RESP);
				if (cityBrandResp != null)
				{
					if (cityBrandResp.grandList == null)
					{
						cityBrandResp.grandList = new List<CityWarGrandInfo> ();
					}
					if (M_BrandPage > 1)
					{
						M_BrandPage = cityBrandResp.grandList.Count > 0 ? M_BrandPage : M_BrandPage - 1;
					}
					CityWarPage.m_instance.OpenBrand (cityBrandResp);
				}
				
				return true;
			}
			case ProtoIndexes.S_CITYWAR_BID_RESP :
			{
				CityWarBidResp cityBidResp = new CityWarBidResp();
				cityBidResp = QXComData.ReceiveQxProtoMessage (p_message,cityBidResp) as CityWarBidResp;
//				Debug.Log ("竞拍页面返回:" + ProtoIndexes.S_CITYWAR_BID_RESP);
				if (cityBidResp != null)
				{
					if (cityBidResp.recordList == null)
					{
						cityBidResp.recordList = new List<BidRecord>();
					}

					CityWarPage.m_instance.OpenBid (cityBidResp,m_targetCityInfo);
				}
				
				return true;
			}
			case ProtoIndexes.S_CITYWAR_OPERATE_RESP :
			{
				CityWarOperateResp operateResp = new CityWarOperateResp();
				operateResp = QXComData.ReceiveQxProtoMessage (p_message,operateResp) as CityWarOperateResp;
//				Debug.Log ("操作返回:" + ProtoIndexes.S_CITYWAR_OPERATE_RESP);
				if (operateResp != null)
				{
//					Debug.Log ("operateType:" + m_operateInfo.operateType);
//					Debug.Log ("result:" + operateResp.result);
					switch (m_operateInfo.operateType)
					{
					case CityOperateType.GET_REWARD://领奖
					{
						if (operateResp.result == 0)
						{
//							Debug.Log ("领取成功");
							RewardData data = new RewardData(900027,m_getNum);
							GeneralRewardManager.Instance ().CreateReward (data);
						}
						else
						{
//							Debug.Log ("领取失败");
							m_text = "领取失败！";
							ClientMain.m_UITextManager.createText (m_text);
						}
						CityWarReward.m_instance.RefreshReward (m_operateInfo.rewardId);

						break;
					}
					case CityOperateType.BID://竞拍
					{
						if (operateResp.result == 0)//成功
						{
							if (operateResp.bidRecord == null)
							{
								CityWarPage.m_instance.RefreshRecState (m_operateInfo.cityId);
							}
							else
							{
//								Debug.Log ("allianceName:" + operateResp.bidRecord.allianceName);
//								Debug.Log ("huFuNum:" + operateResp.bidRecord.huFuNum);
//								Debug.Log ("time:" + operateResp.bidRecord.time);
								CityWarPage.m_instance.RefreshRecState (m_operateInfo.cityId,operateResp.bidRecord);
							}
						}
						else
						{
							int cityType = JCZCityTemplate.GetJCZCityTemplateById (m_operateInfo.cityId).type;
//							Debug.Log ("citytype:" + cityType);
							if (operateResp.result == 5)
							{
//								m_text = "虎符不足！";
//								int cost = JCZCityTemplate.GetJCZCityTemplateById (m_operateInfo.cityId).cost;
								Global.CreateFunctionIcon (2202);
							}
							else
							{
								switch (operateResp.result)
								{
								case 1://不是盟主或副盟主
									//								m_text = "盟主/副盟主才可宣战！";
									if (cityType == 1)
									{
										m_text = LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_30);
									}
									else
									{
										m_text = LanguageTemplate.GetText(LanguageTemplate.Text.JUN_CHENG_ZHAN_10);
									}

									break;
								case 2://非宣战时段
									//									m_text = MyColorData.getColorString (5,JCZTemplate.GetJCZTemplateByKey(CityWarPage.m_instance.M_TimeLabelDic[0][0]).value + "~" + JCZTemplate.GetJCZTemplateByKey(CityWarPage.m_instance.M_TimeLabelDic[0][1]).value) + "为可宣战时段！";
									if (cityType == 1)
									{
										m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_31);	
									}
									else
									{
										m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_12);	
									}
									break;
								case 3://联盟等级不足
									//								m_text = "本城只有" + MyColorData.getColorString (5,JCZCityTemplate.GetJCZCityTemplateById (m_targetCityInfo.cityId).allianceLv) + "级联盟才可宣战！";
									m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_32).Replace ("N",MyColorData.getColorString (5,JCZCityTemplate.GetJCZCityTemplateById (m_targetCityInfo.cityId).allianceLv.ToString ()));
									break;
								case 4://不能对自己领土宣战
									if (cityType == 1)
									{
										//									m_text = "您不能对自己的领土宣战！";
										m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_34);
									}
									else
									{
										//									m_text = "野城只能宣战一个难度！";
										m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_11);	
									}
									break;
								case 6:
									m_text = "联盟已对该难度宣战！";
									CityWarPage.m_instance.RefreshRecState (m_operateInfo.cityId);
									break;
								default:
									break;
								}
								ClientMain.m_UITextManager.createText (m_text);
							}
						}

						break;
					}
					case CityOperateType.ENTER_FIGHT://进入战场
					{
						if (operateResp.result == 0)//成功
						{
							//进入战场
							m_isOpenCityWar = false;

							//set redPoint

							PushAndNotificationHelper.SetRedSpotNotification (300500,false);
//							NewAlliancemanager.Instance ().ShowJunChengZhanAlert ();
							SetAllianceRed ();

							PlayerSceneSyncManager.Instance.EnterAB (m_operateInfo.cityId);
						}
						else
						{
							int cityType = JCZCityTemplate.GetJCZCityTemplateById (m_operateInfo.cityId).type;
							switch (operateResp.result)
							{
							case 1://非攻方或守方盟员
//								m_text = "非攻守方禁止进入战场！";
								m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_37);	
								break;
							case 2://非战斗时段
//								m_text = "非战斗时段禁止进入战场！请于" + MyColorData.getColorString (5,JCZTemplate.GetJCZTemplateByKey(CityWarPage.m_instance.M_TimeLabelDic[2][0]).value + "~" + JCZTemplate.GetJCZTemplateByKey(CityWarPage.m_instance.M_TimeLabelDic[2][1]).value) + "期间再来！";
								m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_35);
								break;
							case 3://不能频繁进出
								Debug.Log ("cd:" + operateResp.cdTime);
//								m_text = "禁止频繁进入战场，进入战场冷却还有" + MyColorData.getColorString (5,operateResp.cdTime.ToString ()) + "秒！";
								if (cityType == 1)
								{
									m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_39).Replace ("XX",MyColorData.getColorString (5,operateResp.cdTime.ToString ()));	
								}
								else
								{
									m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_16).Replace ("XX",MyColorData.getColorString (5,operateResp.cdTime.ToString ()));	
								}
								break;
							case 4://今日战斗已结束
//								m_text = "今日战斗已结束！";
								m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_40);	
								break;
							case 5://没有敌人宣战
//								m_text = "没有敌人对您的该领土宣战，去开疆辟土吧主人！";
								m_text = LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_36);	
								break;
							}
							ClientMain.m_UITextManager.createText (m_text);
						}
						break;
					}
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.S_CITYWAR_BID_MSG_RESP :
			{
				BidMsgInfo bidMsgResp = new BidMsgInfo();
				bidMsgResp = QXComData.ReceiveQxProtoMessage (p_message,bidMsgResp) as BidMsgInfo;
//				Debug.Log ("竞宣战情报返回:" + ProtoIndexes.S_CITYWAR_BID_MSG_RESP);
				if (bidMsgResp != null)
				{
//					Debug.Log ("allianceName:" + bidMsgResp.allianceName);
//					Debug.Log ("cityId:" + bidMsgResp.cityId);
//					Debug.Log ("bidTime:" + bidMsgResp.bidTime);
					if (m_isOpenCityWar)
					{
						CityWarPage.m_instance.InItInfoLabel (bidMsgResp);
					}
				}
				
				return true;
			}
			case ProtoIndexes.S_CITYWAR_SCORE_RESULT_RESP:
			{
				CityWarScoreResultResp scoreResp = new CityWarScoreResultResp();
				scoreResp = QXComData.ReceiveQxProtoMessage (p_message,scoreResp) as CityWarScoreResultResp;
				Debug.Log ("积分返回：" + ProtoIndexes.S_CITYWAR_SCORE_RESULT_RESP);
				if (scoreResp != null)
				{
					if (scoreResp.scoreList == null)
					{
						scoreResp.scoreList = new List<ScoreInfo>();
					}
//					Debug.Log ("积分list:" + scoreResp.scoreList.Count);
//					Debug.Log ("城池名字:" + scoreResp.cityName);
//					Debug.Log ("时间:" + scoreResp.date);
					Debug.Log ("npc:" + scoreResp.isNpc);
					CityWarPage.m_instance.OpenJiFenPage (m_pageType,scoreResp);
				}

				return true;
			}
			}
		}
		return false;
	}

	void LoadCityWarObj ()
	{
		if (m_cityObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.CITY_WAR_ROOT ),CityWarRootLoadCallback );
		}
		else
		{
			m_cityObj.SetActive (true);
			InItCityWarPage ();
		}
	}

	private void CityWarRootLoadCallback ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		m_cityObj = (GameObject)Instantiate (p_object);

		InItCityWarPage ();
	}

	void InItCityWarPage ()
	{
		if( !MainCityUI.TryAddToObjectList (m_cityObj) ){
			UI2DTool.Instance.AddTopUI( m_cityObj );
		}

		CityWarPage.m_instance.M_CityWarDelegate = CityWarDelegateCallBack;
		CityWarPage.m_instance.InItCityWarPage (m_cityResp);
		m_isOpenCityWar = true;
	}

	void CityWarDelegateCallBack ()
	{
		MainCityUI.TryRemoveFromObjectList (m_cityObj);
		m_cityObj.SetActive (false);
		m_isOpenCityWar = false;
	}

	public void SetAllianceRed ()
	{
		GameObject allianceObj = GameObject.Find ("New_My_Union(Clone)");
		if (allianceObj != null)
		{
			NewAlliancemanager.Instance ().ShowJunChengZhanAlert ();
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
		base.OnDestroy ();
	}
}
