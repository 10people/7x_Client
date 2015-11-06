using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanUnExpected : MonoBehaviour,SocketProcessor {

	public static BaiZhanUnExpected unExpected;

	private PlayerStateResp playerStateResp;

	public int enterPlace;//1-对手详情  2-对战阵容

	private OpponentInfo un_opponentInfo;//挑战对手信息
	private ChallengeResp tiaoZhanResp;//挑战阵容信息

	public GameObject baiZhanObj;

	private string titleStr;
	private string str;
	private string confirmStr;
	private string cancelStr;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);

		unExpected = this;
	}

	void Start ()
	{
		enterPlace = 0;

		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}

	//判断对手当前可挑战状态
	public void TiaoZhanStateReq (OpponentInfo tempInfo,int tempPlace,ChallengeResp tempResp)
	{
		enterPlace = tempPlace;

		PlayerStateReq stateReq = new PlayerStateReq ();

		if (tempInfo != null)
		{
			un_opponentInfo = tempInfo;
			stateReq.enemyId = tempInfo.junZhuId;
			stateReq.rank = tempInfo.rank;
		}
		else if (tempResp != null)
		{
			Debug.Log ("tempResp.oppoId:" + tempResp.oppoId);
			tiaoZhanResp = tempResp;
			stateReq.enemyId = tempResp.oppoId;
			stateReq.rank = tempResp.oppoRank;
		}

		if (tempPlace == 1)
		{
			stateReq.junRank = BaiZhanData.Instance ().m_baiZhanInfo.pvpInfo.rank;
		}
		else if (tempPlace == 2)
		{
			stateReq.junRank = tempResp.myRank;
		}

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,stateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.PLAYER_STATE_REQ,ref t_protof,"27019");
		Debug.Log ("挑战验证请求:" + ProtoIndexes.PLAYER_STATE_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.PLAYER_STATE_RESP:
				Debug.Log ("挑战验证请求返回:" + ProtoIndexes.PLAYER_STATE_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				PlayerStateResp stateRes = new PlayerStateResp ();
				
				t_qx.Deserialize(t_stream, stateRes, stateRes.GetType());
				
				if (stateRes != null)
				{
					playerStateResp = stateRes;
					Debug.Log ("StatesType:" + stateRes.type);
					if (stateRes.type == 1)
					{
						Debug.Log ("可以挑战，发送挑战请求");

						if (enterPlace == 1)
						{
							TiaoZhanReq (un_opponentInfo.junZhuId,un_opponentInfo.receiveWeiWang);
						}
						else if (enterPlace == 2)
						{
							UIYindao.m_UIYindao.CloseUI ();

							if (stateRes.isWeiWangChange != null)
							{
								if (stateRes.isWeiWangChange)
								{
									Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
									                        WeiWangChangeCallback );
								}

								else
								{
									EnterBattle ();
								}
							}
							else
							{
								EnterBattle ();
							}
						}
					}

					else
					{
						if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
						{
							ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
							UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
						}

						UIYindao.m_UIYindao.CloseUI ();

						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        StateResCallback );
					}
				}
				
				return true;
			}
		}
		
		return false;
	}

	public void StateResCallback ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_FAIL);

		if (playerStateResp.type == 0)
		{
//			Debug.Log ("玩家不存在！");

			str = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_NONE);

			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,StateRes0);
		}

		else if (playerStateResp.type == 2)
		{
//			Debug.Log ("玩家正在被挑战！");

			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_BGING_CHALLENGED);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_WAIT_TRY);

			str = str1 + "\n" +str2;
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,StateRes0);
		}

		else if (playerStateResp.type == 4)
		{
//			Debug.Log ("挑战机会用完");

			str = "挑战机会已用完";

			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,null);
		}

		else if (playerStateResp.type == 5)
		{
			Debug.Log ("冷却cd");
			
			str = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_DISCD);
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,ClearCdCallBack);
		}

		else if (playerStateResp.type == 7)
		{
//			Debug.Log ("对手名次发生变化！");

			str = "对手名次发生变化，请重新选择挑战对手！";
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,RankChangeRefresh1);
		}

		else if (playerStateResp.type == 8)
		{
//			Debug.Log ("自己的名次已发生变化！");
			
			str = "您的名次已发生变化！";
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
			             null, confirmStr, null,RankChangeRefresh2);
		}
	}

	void StateRes0 (int i)
	{
//		Debug.Log ("刷新玩家列表");

		GameObject tiaoZhanObj = GameObject.Find ("New_ZhengRong(Clone)");

		if (tiaoZhanObj)
		{
			Destroy (tiaoZhanObj);
		}

		BaiZhanData.Instance ().BaiZhanReq ();
	}

	void ClearCdCallBack (int i)
	{

	}

	void WeiWangChangeCallback ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		titleStr = "提示";

		if (playerStateResp.receiveWeiWang != null)
		{
			str = "当前战斗胜利可得到威望值" + playerStateResp.receiveWeiWang + "\n" + "是否继续进行挑战？";
		}

		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null, 
		             null, cancelStr, confirmStr,WeiWangChangeRes);
	}

	void WeiWangChangeRes (int i)
	{
//		Debug.Log ("刷新玩家列表");

		if (i == 1)
		{
			GameObject tiaoZhanObj = GameObject.Find ("GeneralChallengePage");
			
			if (tiaoZhanObj)
			{
				Destroy (tiaoZhanObj);
			}
			
			BaiZhanData.Instance ().BaiZhanReq ();
		}

		else
		{
			EnterBattle ();
		}
	}

	void EnterBattle ()
	{
//		EnterBattlefieldReq  mRecordChallenger = new EnterBattlefieldReq ();
//		
//		MemoryStream TiaoZhanStream = new MemoryStream();
//		
//		QiXiongSerializer TiaoZhan_er = new QiXiongSerializer();
//		
//		mRecordChallenger.enemyId = tiaoZhanResp.oppoId;
//
////		mRecordChallenger.rank = Enemy_Ifon.rank;
//		TiaoZhan_er.Serialize(TiaoZhanStream, mRecordChallenger);
//		
//		byte[] t_protof;
//		
//		t_protof = TiaoZhanStream.ToArray();
//		
//		SocketTool.Instance().SendSocketMessage(
//			ProtoIndexes.RECORD_CHALLENGER,
//			ref t_protof);
		
		EnterBattleField.EnterBattlePvp(tiaoZhanResp.oppoId);
		Debug.Log ("oppoId:" + tiaoZhanResp.oppoId);
	}

	//对手名次发生变化
	void RankChangeRefresh1 (int i)
	{
		ConfirmManager.confirm.ConfirmReq (7,null,0);
	}

	//自己名次发生变化
	void RankChangeRefresh2 (int i)
	{
		ConfirmManager.confirm.ConfirmReq (6,null,0);
	}

	//请求挑战
	void TiaoZhanReq (long junZhuId,int weiWang)
	{
		ChallengeReq tiaoZhanReq = new ChallengeReq ();
		
		tiaoZhanReq.oppoJunZhuId = junZhuId;
		tiaoZhanReq.showWeiWang = weiWang;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,tiaoZhanReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.CHALLENGE_REQ,ref t_protof,"27012");
//		Debug.Log ("TiaoZhanReq:" + ProtoIndexes.CHALLENGE_REQ);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
