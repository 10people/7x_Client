using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TanBaoData : Singleton<TanBaoData>,SocketProcessor {

	private ExploreInfoResp tbInfoResp;

	public enum TanBaoType
	{
		TONGBI_SINGLE = 1,
		TONGBI_SPEND = 2,
		YUANBAO_SINGLE = 3,
		YUANBAO_SPEND = 4,
	}
	private TanBaoType tbType = TanBaoType.TONGBI_SINGLE;

	private GameObject tbObj;
	
	private string textStr;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// Tans the bao req.
	/// </summary>
	public void TanBaoInfoReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.EXPLORE_INFO_REQ,ProtoIndexes.EXPLORE_INFO_RESP.ToString ());
//		Debug.Log ("TanBaoInfoReq:" + ProtoIndexes.EXPLORE_INFO_REQ);
	}

	/// <summary>
	/// TBs the get reward req.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void TBGetRewardReq (TanBaoType tempType)
	{
		tbType = tempType;
//		Debug.Log ("tbType:" + tbType);
		ExploreReq tbGetRewardReq = new ExploreReq ();
		tbGetRewardReq.type = (int)tempType;
		QXComData.SendQxProtoMessage (tbGetRewardReq,ProtoIndexes.EXPLORE_REQ,ProtoIndexes.EXPLORE_RESP.ToString ());
//		Debug.Log ("TBGetRewardReq:" + ProtoIndexes.EXPLORE_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.EXPLORE_INFO_RESP://探宝返回
			{
//				Debug.Log ("TanBaoInfoResp:" + ProtoIndexes.EXPLORE_INFO_RESP);

				ExploreInfoResp tbInfoRes = new ExploreInfoResp();
				tbInfoRes = QXComData.ReceiveQxProtoMessage (p_message,tbInfoRes) as ExploreInfoResp;

				if (tbInfoRes != null)
				{
					tbInfoResp = tbInfoRes;

//					Debug.Log ("铜币：" + tbInfoRes.tongBi + 
//					           "\n免费铜币抽取次数：" + tbInfoRes.allFreeTongBiCount +
//					           "\n铜币还剩免费抽取次数：" + tbInfoRes.remainFreeTongBiCount + 
//					           "\n铜币抽取cd：" + tbInfoRes.tongBiCd +
//					           "\n元宝：" + tbInfoRes.yuanBao + 
//					           "\n元宝抽取cd：" + tbInfoRes.yuanBaoCd);

					LoadTanBaoPrefab ();
				}

				return true;
			}

			case ProtoIndexes.EXPLORE_RESP:
			{
//				Debug.Log ("TBGetRewardResp:" + ProtoIndexes.EXPLORE_RESP);
				ExploreResp tbGetRewardRes = new ExploreResp();
				tbGetRewardRes = QXComData.ReceiveQxProtoMessage (p_message,tbGetRewardRes) as ExploreResp;

				if (tbGetRewardRes != null)
				{
//					Debug.Log ("探宝结果：" + tbGetRewardRes.success + "//0-成功 1-铜币不足 2-元宝不足 3-数据错误");
					if (tbGetRewardRes.awardsList == null)
					{
						tbGetRewardRes.awardsList = new List<Award>();
					}

					if (tbGetRewardRes.success == 0)
					{
//						Debug.Log ("TypeInfo:" + tbGetRewardRes.info);
						if (tbGetRewardRes.info == null)
						{
							tbGetRewardRes.info = new TypeInfo();
						}
//						Debug.Log ("TypeInfo.money:" + tbGetRewardRes.info.money + 
//						           "\nCd:" + tbGetRewardRes.info.cd);

//						Debug.Log ("tbGetRewardRes.info.cd:" + tbGetRewardRes.info.cd);
						TanBaoPage.tbPage.RefreshTanBaoInfo (tbType,tbGetRewardRes);
						TanBaoPage.tbPage.CheckTBRed ();
						TBReward.tbReward.GetTBReward (tbType,tbGetRewardRes);
					}
					else
					{
						switch (tbGetRewardRes.success)
						{
						case 1:
//							textStr = "对不起，您的铜币不足！\n通过以下方式可以获取更多的铜币：\n元宝购买、【行镖】玩法、【试练-洗劫权贵】。";
//							textStr = LanguageTemplate.GetText (LanguageTemplate.Text.TAN_BAO_TIPS_2);//铜币不足
							Global.CreateFunctionIcon (501);
							TBReward.tbReward.BlockController (false);
							break;
						case 2:
//							textStr = LanguageTemplate.GetText (LanguageTemplate.Text.TAN_BAO_TIPS_1);//元宝不足
							Global.CreateFunctionIcon (101);
							TBReward.tbReward.BlockController (false);
							break;
						case 3:
							textStr = "数据错误...";
							QXComData.CreateBox (1,textStr,true,TanBaoRespCallBack,true);
							break;
						default:
							break;
						}
					}
				}

				return true;
			}
			}
		}
		return false;
	}

	public void TanBaoRespCallBack (int i)
	{
		UIShoujiManager.m_UIShoujiManager.m_isPlayShouji = true;
		TBReward.tbReward.BlockController (false);
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.FINISHED_TASK_YINDAO,100160,4);
	}

	void LoadTanBaoPrefab ()
	{
		if (tbObj == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TANBAO),
			                        TanBaoPrefabLoadCallBack);
		}
		else
		{
			tbObj.SetActive (true);
			InItTanBaoInfo ();
		}
	}
	public void TanBaoPrefabLoadCallBack (ref WWW p_www, string p_path, Object p_object)
	{
		tbObj = (GameObject)Instantiate(p_object);
		InItTanBaoInfo ();
	}

	void InItTanBaoInfo ()
	{
		if (!MainCityUI.IsExitInObjectList (tbObj))
		{
			MainCityUI.TryAddToObjectList(tbObj);
		}

		//十连副本场景
		TreasureCityUI.TryAddToObjectList (tbObj);

		TanBaoPage.tbPage.GetTBInfoResp (tbInfoResp);
	}

	void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
