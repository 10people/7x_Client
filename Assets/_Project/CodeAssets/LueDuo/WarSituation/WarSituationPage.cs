using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarSituationPage : MonoBehaviour {

	public static WarSituationPage situationPage;

	private JunQingResp situationResp;
	private WarSituationData.SituationType situationType;

	public UIScrollView situationSc;
	public UIScrollBar situationSb;

	public GameObject situationObj;
	private List<GameObject> situationList = new List<GameObject>();

	public GameObject plunderBtnObj;
	public GameObject yunBiaoBtnObj;
	public List<EventHandler> situationHandlerList = new List<EventHandler>();

	public UILabel leftLabel;
	public UILabel rightLabel;

	public UILabel desLabel;

	public GameObject anchorTopRight;

	public GameObject m_anchorLT;

	private string textStr;

	private bool isEnterByPlunder = false;

	void Awake ()
	{
		situationPage = this;
	}

	void OnDestroy ()
	{
		situationPage = null;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (anchorTopRight);
		QXComData.LoadTitleObj (m_anchorLT,"联盟军情");
	}

	/// <summary>
	/// Ins it war situation page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempResp">Temp resp.</param>
	public void InItWarSituationPage (WarSituationData.SituationType tempType,JunQingResp tempResp,bool enterByPlunder)
	{
		situationType = tempType;
		situationResp = tempResp;

		isEnterByPlunder = enterByPlunder;

		yunBiaoBtnObj.SetActive (!enterByPlunder);
		plunderBtnObj.transform.localPosition = new Vector3 (enterByPlunder ? -360 : -175,230,0);

		situationList = QXComData.CreateGameObjectList (situationObj,tempResp.infos.Count,situationList);

		PushAndNotificationHelper.SetRedSpotNotification (tempType == WarSituationData.SituationType.PLUNDER ? 410012 : 410015,situationList.Count > 0 ? true : false);
		if (enterByPlunder)
		{
			PlunderPage.plunderPage.SetSituationRedPoint (situationList.Count > 0 ? true : false);
		}

		if (tempType == WarSituationData.SituationType.YUNBIAO)
		{
			for (int i = 0;i < tempResp.infos.Count - 1;i ++)
			{
				for (int j = 0;j < tempResp.infos.Count - i -1;j ++)
				{
					if (tempResp.infos[j].friendRemainHP > tempResp.infos[j + 1].friendRemainHP)
					{
						HistoryBattleInfo tempInfo = tempResp.infos[j];
						tempResp.infos[j] = tempResp.infos[j + 1];
						tempResp.infos[j + 1] = tempInfo;
					}
				}
			}
		}

		for (int i = 0;i < situationList.Count;i ++)
		{
			situationList[i].transform.localPosition = new Vector3(0,-130 * i,0);
			situationSc.UpdateScrollbars (true);

			WarSituationItem situation = situationList[i].GetComponent<WarSituationItem> ();
			situation.InItSituationItem (tempType,tempResp.infos[i]);
		}

		situationSc.enabled = situationList.Count > 3 ? true : false;
		situationSb.gameObject.SetActive (situationList.Count > 3 ? true : false);

		desLabel.text = situationList.Count > 0 ? "" : "军情记录为空";
		leftLabel.text = tempType == WarSituationData.SituationType.YUNBIAO ? "盟友运镖成功后联盟可获得建设值" : "驱逐可获得贡献度奖励\n驱逐成功联盟将不会损失建设值";

		StopCoroutine ("PlunderSituationCd");

		switch (tempType)
		{
		case WarSituationData.SituationType.PLUNDER:

			if (situationResp.todayRemainHelp > 0)
			{
				if (situationResp.cd > 0)
				{
					StartCoroutine ("PlunderSituationCd");
				}
				else
				{
					rightLabel.text = "今日剩余驱逐次数：" + situationResp.todayRemainHelp + "/" + situationResp.todayAllHelp;
				}
			}
			else
			{
				rightLabel.text = "今日剩余驱逐次数：" + situationResp.todayRemainHelp + "/" + situationResp.todayAllHelp;
			}

			break;
		case WarSituationData.SituationType.YUNBIAO:

			rightLabel.text = "";

			break;
		default:
			break;
		}

		QXComData.SetBtnState (plunderBtnObj,tempType == WarSituationData.SituationType.PLUNDER ? true : false);
		QXComData.SetBtnState (yunBiaoBtnObj,tempType == WarSituationData.SituationType.YUNBIAO ? true : false);

		foreach (EventHandler handler in situationHandlerList)
		{
			handler.m_click_handler -= SituationHandlerClickBack;
			handler.m_click_handler += SituationHandlerClickBack;
		}
	}

	void SituationHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "CloseBtn":

			CloseSituationPage ();

			break;
		case "YunBiaoBtn":

			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.YUNBIAO);

			break;
		case "LueDuoBtn":

			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.PLUNDER,isEnterByPlunder);

			break;
		default:
			break;
		}
	}

	IEnumerator PlunderSituationCd ()
	{
		while (situationResp.cd > 0)
		{
			situationResp.cd --;

			rightLabel.text = "今日剩余驱逐次数：" + situationResp.todayRemainHelp + "/" + situationResp.todayAllHelp +
			                                              "\n驱逐冷却时间：" + TimeHelper.GetUniformedTimeString (situationResp.cd);

			yield return new WaitForSeconds (1);

			if (situationResp.cd == 0)
			{
				WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.PLUNDER,isEnterByPlunder);
			}
		}
	}

	/// <summary>
	/// Refreshs the item list.
	/// </summary>
	/// <param name="dataID">Data I.</param>
	public void RefreshItemList (int dataID,bool isDestroy)
	{
		if (isDestroy)
		{
			for (int i = 0;i < situationResp.infos.Count;i ++)
			{
				if (situationResp.infos[i].itemId == dataID)
				{
					Destroy (situationList[i]);
					situationList.RemoveAt (i);
					situationResp.infos.RemoveAt (i);
					break;
				}
			}
		}
		else
		{
			for (int i = 0;i < situationResp.infos.Count;i ++)
			{
				if (situationResp.infos[i].itemId == dataID)
				{
					situationResp.infos[i].state = 1;
					break;
				}
			}
		}

		InItWarSituationPage (situationType,situationResp,isEnterByPlunder);
	}

	/// <summary>
	/// Fights the judgment.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void FightJudgment (HistoryBattleInfo tempInfo)
	{
		WarSituationData.Instance.Expel (tempInfo.itemId);
//		if (situationResp.todayRemainHelp <= 0)
//		{
//			textStr = "今日已无剩余驱逐次数！";
//			QXComData.CreateBox (1,textStr,true,null);
//		}
//		else if (situationResp.cd > 0)
//		{
//			textStr = "处于驱逐冷却期！";
//			QXComData.CreateBox (1,textStr,true,null);
//		}
//		else
//		{
//			//请求驱逐
//			WarSituationData.Instance.Expel (tempInfo.itemId);
//		}
	}

	public void CloseSituationPage ()
	{
		if (MainCityUI.m_MainCityUI != null)
		{
			MainCityUI.m_MainCityUI.setInit ();
		}

		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
