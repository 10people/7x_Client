using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarSituationData : Singleton<WarSituationData>,SocketProcessor {

	public enum SituationType
	{
		PLUNDER = 1,
		YUNBIAO = 2,
	}
	private SituationType situationType;

	private JunQingResp situationResp;

	private GameObject situationObj;

	private bool isSituationPageOpen = false;
	public bool IsSituationPageOpen { set{isSituationPageOpen = value;} get{return isSituationPageOpen;} }

	private int itemId;

	private string textStr;

	private bool isEnterByPlunder = false;//是否是从掠夺进入

	private readonly Dictionary<int,string> resultDic = new Dictionary<int, string>()
	{
		{1,"对手已经不在原来的联盟！"},
		{2,"联盟变更，无法驱逐！"},
		{3,"驱逐时间已过！"},
		{4,"对手正在被其他玩家驱逐！\n请待战斗结束后再次尝试！"},
		{5,"驱逐次数不够！"},
		{6,"驱逐冷却中！"},
	};

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
		base.OnDestroy ();
	}

	/// <summary>
	/// Opens the war situation.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void OpenWarSituation (SituationType tempType = SituationType.PLUNDER,bool enterByPlunder = false)
	{
		situationType = tempType;

		isEnterByPlunder = enterByPlunder;

		if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			//无联盟
			ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"您还没有加入一个联盟！"));
			return;
		}

//		switch (situationType)
//		{
//		case SituationType.PLUNDER:
//
//			if (JunZhuData.Instance().m_junzhuInfo.level < FunctionOpenTemp.GetTemplateById (211).Level)
//			{
//				//未到掠夺开启等级
//				ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"[dc0600]" + FunctionOpenTemp.GetTemplateById (211).Level + "[-]级开启掠夺！"));
//				return;
//			}
//			break;
//		case SituationType.YUNBIAO:
//			
//			if (!FunctionOpenTemp.IsHaveID(310))
//			{
//				ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,FunctionOpenTemp.GetTemplateById (310).m_sNotOpenTips));
//				return;
//			}
//			
//			break;
//		default:
//			break;
//		}

		JunQingReq junQingReq = new JunQingReq();
		junQingReq.type = (int)tempType;
		QXComData.SendQxProtoMessage (junQingReq,ProtoIndexes.alliance_junQing_req);
		Debug.Log ("联盟军情请求：" + ProtoIndexes.alliance_junQing_req);
	}

	/// <summary>
	/// Expel the specified tempId.
	/// </summary>
	/// <param name="tempId">Temp identifier.</param>
	public void Expel (int tempId)
	{
		itemId = tempId;

		QuZhuReq quZhu = new QuZhuReq ();
		quZhu.itemId = tempId;
		QXComData.SendQxProtoMessage (quZhu,ProtoIndexes.go_qu_zhu_req);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.alliance_junQing_resq:
			{
				Debug.Log ("联盟军情返回：" + ProtoIndexes.alliance_junQing_resq);
				JunQingResp junQingData = new JunQingResp();
				junQingData = QXComData.ReceiveQxProtoMessage (p_message,junQingData) as JunQingResp;

				if (junQingData != null)
				{
					if (junQingData.infos == null)
					{
						junQingData.infos = new List<HistoryBattleInfo>();
					}
					Debug.Log ("junQingData.infos:" + junQingData.infos.Count);
					situationResp = junQingData;

					situationType = (SituationType)Enum.ToObject (typeof(SituationType),situationResp.type);

					if (situationObj == null)
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.WARSITUATION ),
						                        SituationObjLoadBack );
					}
					else
					{
						situationObj.SetActive (true);
						InItSituationPage ();
					}
				}

				return true;
			}
			case ProtoIndexes.go_qu_zhu_resp:
			{
				Debug.Log ("驅逐返回：" + ProtoIndexes.go_qu_zhu_resp);
				ErrorMessage errorMsg = new ErrorMessage();
				errorMsg = QXComData.ReceiveQxProtoMessage (p_message,errorMsg) as ErrorMessage;

				if (errorMsg != null)
				{
					Debug.Log ("errorMsg.errorCode:" + errorMsg.errorCode);
					if (errorMsg.errorCode == 0)
					{
						//可以驱逐
						EnterBattleField.EnterBattleYuanZhu (itemId);
						PushAndNotificationHelper.SetRedSpotNotification (410012,false);
					}
					else
					{
						textStr = resultDic[errorMsg.errorCode];

						if (errorMsg.errorCode == 1 || errorMsg.errorCode == 2 | errorMsg.errorCode == 3)
						{
							//删除item
							QXComData.CreateBox (1,textStr,true,DeleteItem);
						}
						else if (errorMsg.errorCode == 4)
						{
							QXComData.CreateBox (1,textStr,true,Expeling);
						}
						else
						{
							QXComData.CreateBox (1,textStr,true,null);
						}
					}
				}

				return true;
			}
			}
		}

		return false;
	}

	void Expeling (int i)
	{
		//刷新item驱逐状态
		WarSituationPage.situationPage.RefreshItemList (itemId,false);
	}                              

	void DeleteItem (int i)
	{
		//删除无法驱逐的item
		WarSituationPage.situationPage.RefreshItemList (itemId,true);
	}

	void SituationObjLoadBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		situationObj = GameObject.Instantiate( p_object ) as GameObject;
		
		InItSituationPage ();
	}

	void InItSituationPage ()
	{
		IsSituationPageOpen = true;
		MainCityUI.TryAddToObjectList (situationObj);
		UIYindao.m_UIYindao.CloseUI ();
		WarSituationPage.situationPage.InItWarSituationPage (situationType,situationResp,isEnterByPlunder);
	}

	/// <summary>
	/// Refreshs the situation page.
	/// </summary>
	public void RefreshSituationPage (int xmlId)
	{
		string textStr = "";
		QXComData.CreateBox (1,textStr,true,RefreshSituationPageCallBack,true);
	}

	void RefreshSituationPageCallBack (int cilckId)
	{
		for (int i = 0;i < situationResp.infos.Count;i ++)
		{

		}

		if (IsSituationPageOpen)
		{
			WarSituationPage.situationPage.InItWarSituationPage (situationType,situationResp,isEnterByPlunder);
		}
	}
}
