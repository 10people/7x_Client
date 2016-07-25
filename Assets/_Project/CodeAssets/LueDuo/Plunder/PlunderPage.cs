using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderPage : MonoBehaviour {

	public static PlunderPage plunderPage;

	private LveDuoInfoResp plunderResp;

	public enum SwitchPageType
	{
		MAIN_PAGE = 0,
		RANK_PAGE = 1,
		RECORD_PAGE = 2,
		REWARD_PAGE = 3,
	}
	private SwitchPageType pageType;

	public List<GameObject> pageObjList = new List<GameObject>();

	public UILabel titleLabel;

	public ScaleEffectController sEffectController;

	private bool isOpenFirst = true;

	private string textStr;

	public GameObject m_rankRewardBtn;

	public GameObject anchorTopRight;
	public GameObject m_anchorTL;

	void Awake ()
	{
		plunderPage = this;
	}

	void OnDestroy ()
	{
		plunderPage = null;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (anchorTopRight);
		QXComData.LoadTitleObj (m_anchorTL,"掠夺");
	}

	/// <summary>
	/// Switchs the plunder page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void SwitchPlunderPage (SwitchPageType tempType)
	{
		pageType = tempType;

		pageObjList [(int)SwitchPageType.MAIN_PAGE].SetActive (tempType == SwitchPageType.MAIN_PAGE ? true : false);
		pageObjList [(int)SwitchPageType.RANK_PAGE].SetActive (tempType == SwitchPageType.RANK_PAGE ? true : false);
		pageObjList [(int)SwitchPageType.RECORD_PAGE].SetActive (tempType == SwitchPageType.RECORD_PAGE ? true : false);
		pageObjList [(int)SwitchPageType.REWARD_PAGE].SetActive (tempType == SwitchPageType.REWARD_PAGE ? true : false);

		switch (tempType)
		{
		case SwitchPageType.MAIN_PAGE:
			titleLabel.text = "掠夺";
			break;
		case SwitchPageType.RANK_PAGE:
			titleLabel.text = "掠夺排行";
				break;
		case SwitchPageType.RECORD_PAGE:
			titleLabel.text = "掠夺记录";
			break;
		case SwitchPageType.REWARD_PAGE:
			titleLabel.text = "结算奖励";
			break;
		default:
			break;
		}

		titleLabel.GetComponent<UILabelType> ().init ();
	}

	#region MainPage
	//国家按钮
	private List<GameObject> nationBtnsList = new List<GameObject> ();
	public GameObject nationBtnObj;

	//联盟
	public UIScrollView allianceSc;
	public UIScrollBar allianceSb;
	private List<GameObject> allianceItemList = new List<GameObject> ();
	public GameObject allianceItemObj;
	public GameObject allianceGrid;
	public UILabel allianceDesLabel;

	public int M_CurAllianceId;

	//对手
	public UIScrollView opponentSc;
	public UIScrollBar opponentSb;
	private List<GameObject> opponentItemList = new List<GameObject> ();
	public GameObject opponentItemObj;
	public GameObject opponentGrid;
	public UILabel opponentDesLabel;

	private bool canTap = false;
	
	private bool isRefreshToTop = true;

	public GameObject btnsObj;
	public GameObject labelsObj;

	public UILabel numsLabel;
	public UILabel gongJinLabel;
	public UILabel timeLabel;
	private int cdTime;//掠夺cd
	public UILabel m_shengWangDes;

	public List<EventHandler> plunderBtnList = new List<EventHandler> ();
	private Dictionary<string,EventHandler> plunderBtnDic = new Dictionary<string, EventHandler> ();

	private bool isFirstOpen = true;
	private int startNationIndex = 0;
	private int startAllianceIndex = 0;

	private bool isOpponentToTop;//对手列表是否回到顶端
	public bool IsOpponentToTop { get{return isOpponentToTop;} set{isOpponentToTop = value;} }

	public UILabel getRewardTimeLabel;
	private int getRewardCdTime;//领取每日奖励cd

	public GameObject recordRedObj;
	public GameObject situationRedObj;

	/// <summary>
	/// Ins it plunder page.
	/// </summary>
	/// <param name="tempRes">Temp res.</param>
	public void InItPlunderPage (LveDuoInfoResp tempRes)
	{
		plunderResp = tempRes;
		isRefreshToTop = true;
		IsOpponentToTop = true;
		if (isOpenFirst)
		{
			sEffectController.OnOpenWindowClick ();
			isOpenFirst = false;
		}

		SwitchPlunderPage (SwitchPageType.MAIN_PAGE);

		if (nationBtnsList.Count == 0)
		{
			for (int i = 0;i < tempRes.guoLianInfos.Count;i ++)
			{
				GameObject nationBtn = (GameObject)Instantiate (nationBtnObj);
				
				nationBtn.SetActive (true);
				nationBtn.name = i.ToString ();
				nationBtn.transform.parent = nationBtnObj.transform.parent;
				nationBtn.transform.localPosition = new Vector3(0,120 - 48 * i,0);
				nationBtn.transform.localScale = Vector3.one;
				
				nationBtnsList.Add (nationBtn);
			}
		}
		
		for (int i = 0;i < tempRes.guoLianInfos.Count;i ++)
		{
			PlunderNationBtn ldNationBtn = nationBtnsList[i].GetComponent<PlunderNationBtn> ();
			ldNationBtn.InItNationBtn (tempRes.guoLianInfos[i],i);
			
			EventHandler handler = nationBtnsList[i].GetComponent<EventHandler> ();
			handler.m_click_handler -= NationBtnHandlerClickBack;
			handler.m_click_handler += NationBtnHandlerClickBack;
		}

		plunderBtnDic.Clear ();
		foreach (EventHandler handler in plunderBtnList)
		{
			plunderBtnDic.Add (handler.name,handler);
			handler.m_click_handler -= PlunderHandlerClickBack;
			handler.m_click_handler += PlunderHandlerClickBack;
		}

		InItPlunderAlliance ();

		SetPlunderState ();
		SetGetRewardState ();
		SetRedPoint (tempRes.hasRecord);
		SetSituationRedPoint (FunctionOpenTemp.IsShowRedSpotNotification (410012));

		if (isFirstOpen)
		{
			startNationIndex = 0;
			NationBtnHandlerClickBack (nationBtnsList[0]);
		}
	}

	void NationBtnHandlerClickBack (GameObject obj)
	{
		for (int i = 0;i < nationBtnsList.Count;i ++)
		{
			PlunderNationBtn plundernation = nationBtnsList[i].GetComponent<PlunderNationBtn> ();
			plundernation.BtnAnimation (obj.name == nationBtnsList[i].name ? true : false);
		}

		if (int.Parse (obj.name) != startNationIndex)
		{
			//国家联盟信息请求
			isRefreshToTop = true;
			IsOpponentToTop = true;
			startNationIndex = int.Parse (obj.name);
			PlunderData.Instance.PdAlliancePage = 1;
			PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.ALLIANCE,plunderResp.guoLianInfos[startNationIndex].guojiaId);
		}
	}

	/// <summary>
	/// Gets the current nation identifier.
	/// </summary>
	/// <returns>The current nation identifier.</returns>
	public int GetCurNationId ()
	{
		return plunderResp.guoLianInfos [startNationIndex].guojiaId;
	}

	/// <summary>
	/// Ins it plunder alliance.
	/// </summary>
	void InItPlunderAlliance ()
	{
		allianceItemList = QXComData.CreateGameObjectList (allianceItemObj, plunderResp.mengInfos.Count,allianceItemList);

		if (allianceItemList.Count <= 5)
		{
			allianceSc.ResetPosition ();
		}

		allianceGrid.GetComponent<ItemTopCol> ().enabled = allianceItemList.Count < 5 ? true : false;

		for (int i = 0;i < allianceItemList.Count;i ++)
		{
			allianceItemList[i].transform.localPosition = new Vector3(0,-72 * i,0);
			allianceItemList[i].name = i.ToString ();
			PdAllianceItem pdAlliance = allianceItemList[i].GetComponent<PdAllianceItem> ();
			pdAlliance.InItAllianceItem (plunderResp.mengInfos[i]);
			M_CurAllianceId = plunderResp.mengInfos[0].mengId;

			EventHandler handler = allianceItemList[i].GetComponent<EventHandler> ();
			handler.m_click_handler -= AllianceItemClickBack;
			handler.m_click_handler += AllianceItemClickBack;
		}

		//Reset scroll view.
		allianceSc.UpdateScrollbars(true);
		allianceSb.value = isRefreshToTop ? 0.0f : 1.0f;
		allianceSb.ForceUpdate();
		allianceSc.UpdatePosition();

		allianceSb.gameObject.SetActive (allianceItemList.Count > 5 ? true : false);

		allianceDesLabel.text = allianceItemList.Count > 0 ? "" : "这个国家暂时还没有联盟！";

		startAllianceIndex = 0;

		canTap = true;

		if (allianceItemList.Count > 0)
		{
			AllianceItemClickBack (allianceItemList[0]);
		}

		InItPlunderOpponent ();
	}

	void AllianceItemClickBack (GameObject obj)
	{
		for (int i = 0;i < allianceItemList.Count;i ++)
		{
			PdAllianceItem pdAlliance = allianceItemList[i].GetComponent<PdAllianceItem> ();
			pdAlliance.ShowSelect (obj.name == allianceItemList[i].name ? true : false);
		}
		if (int.Parse (obj.name) != startAllianceIndex)
		{
			//联盟成员信息请求
			M_CurAllianceId = plunderResp.mengInfos[int.Parse (obj.name)].mengId;
			startAllianceIndex = int.Parse (obj.name);
			PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.JUNZHU,plunderResp.mengInfos[startAllianceIndex].mengId);
		}
	}

	/// <summary>
	/// Gets the current alliance identifier.
	/// </summary>
	/// <returns>The current alliance identifier.</returns>
	public int GetCurAllianceId ()
	{
		return plunderResp.mengInfos [startAllianceIndex].mengId;
	}

	/// <summary>
	/// Ins it plunder opponent.
	/// </summary>
	void InItPlunderOpponent ()
	{
		opponentItemList = QXComData.CreateGameObjectList (opponentItemObj,plunderResp.junInfos.Count, opponentItemList);

		opponentSc.ResetPosition ();

		opponentGrid.GetComponent<ItemTopCol> ().enabled = opponentItemList.Count < 4 ? true : false;

		opponentDesLabel.text = opponentItemList.Count > 0 ? "" : "暂时还没有可掠夺的对手！\n（[c40000]29[-]级以上的玩家可被掠夺）";

		//cd==0 在前 贡金多的在前
		for (int i = 0;i < plunderResp.junInfos.Count - 1;i ++)
		{
			for (int j = 0;j < plunderResp.junInfos.Count - i - 1;j ++)
			{
				if (plunderResp.junInfos[j].leftProtectTime == 0)
				{
					if (plunderResp.junInfos[j + 1].leftProtectTime == 0)
					{
						if (plunderResp.junInfos[j].gongjin < plunderResp.junInfos[j + 1].gongjin)
						{
							JunZhuInfo tempInfo = plunderResp.junInfos[j];
							plunderResp.junInfos[j] = plunderResp.junInfos[j + 1];
							plunderResp.junInfos[j + 1] = tempInfo;
						}
						else if (plunderResp.junInfos[j].gongjin == plunderResp.junInfos[j + 1].gongjin)
						{
							if (plunderResp.junInfos[j].zhanli < plunderResp.junInfos[j + 1].zhanli)
							{
								JunZhuInfo tempInfo = plunderResp.junInfos[j];
								plunderResp.junInfos[j] = plunderResp.junInfos[j + 1];
								plunderResp.junInfos[j + 1] = tempInfo;
							}
							else if (plunderResp.junInfos[j].zhanli == plunderResp.junInfos[j + 1].zhanli)
							{
								if (plunderResp.junInfos[j].remainHp < plunderResp.junInfos[j + 1].remainHp)
								{
									JunZhuInfo tempInfo = plunderResp.junInfos[j];
									plunderResp.junInfos[j] = plunderResp.junInfos[j + 1];
									plunderResp.junInfos[j + 1] = tempInfo;
								}
							}
						}
					}
				}
				else
				{
					if (plunderResp.junInfos[j + 1].leftProtectTime == 0)
					{
						JunZhuInfo tempInfo = plunderResp.junInfos[j];
						plunderResp.junInfos[j] = plunderResp.junInfos[j + 1];
						plunderResp.junInfos[j + 1] = tempInfo;
					}
					else
					{
						if (plunderResp.junInfos[j].leftProtectTime > plunderResp.junInfos[j + 1].leftProtectTime)
						{
							JunZhuInfo tempInfo = plunderResp.junInfos[j];
							plunderResp.junInfos[j] = plunderResp.junInfos[j + 1];
							plunderResp.junInfos[j + 1] = tempInfo;
						}
					}
				}
			}
		}

		for (int i = 0;i < opponentItemList.Count;i ++)
		{
			opponentItemList[i].transform.localPosition = new Vector3(0,-190 * i,0);
			opponentSc.UpdateScrollbars (true);
			opponentSb.value = IsOpponentToTop ? 0.0f : opponentSb.value;
			PdOpponentItem pdOpponent = opponentItemList[i].GetComponent<PdOpponentItem> ();
			pdOpponent.InItOpponentItem (plunderResp.junInfos[i]);
		}

		opponentSb.gameObject.SetActive (opponentItemList.Count > 4 ? true : false);
	}

	void PlunderHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "RecordBtn":

			SetRedPoint (false);
			PlunderData.Instance.PlunderRecordReq ();

			break;
		case "RankBtn":

			PlunderData.Instance.PRankPage = 1;
			PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);

			PlunderData.Instance.ARankPage = 1;
			PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);

			break;
		case "ResetBtn":
			Debug.Log ("plunderResp.clearCdYB:" + plunderResp.clearCdYB);
			Debug.Log ("PurchaseTemplate.GetPurchaseTempById (65047).price:" + PurchaseTemplate.GetPurchaseTempById (65047).price);
			textStr = "是否使用" + (plunderResp.clearCdYB == 0 ? PurchaseTemplate.GetPurchaseTempById (65047).price : plunderResp.clearCdYB) + "元宝清除冷却时间？";
			
			QXComData.CreateBoxDiy (textStr,false,PlunderData.Instance.ClearCdReqBack,true,0,plunderResp.canClearCdVIP);

			break;
		case "AddNumBtn":

			PlunderData.Instance.BuyPlunderTimes ();

			break;
		case "RulesBtn":

			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.LUEDUO_HELP_DESC));

			break;
		case "CloseBtn":

			switch (pageType)
			{
			case SwitchPageType.MAIN_PAGE:

				sEffectController.OnCloseWindowClick ();
				sEffectController.CloseCompleteDelegate += ClosePlunderPage;

				break;
			case SwitchPageType.RANK_PAGE:

				aRankSb.value = 0;
				pRankSb.value = 0;
				isArankToTop = true;
				isPRankToTop = true;
				aRankSc.ResetPosition ();
				pRankSc.ResetPosition ();
				SwitchPlunderPage (SwitchPageType.MAIN_PAGE);

				break;
			case SwitchPageType.RECORD_PAGE:

				recordSb.value = 0;
				SwitchPlunderPage (SwitchPageType.MAIN_PAGE);

				break;
			case SwitchPageType.REWARD_PAGE:

				SwitchPlunderPage (SwitchPageType.MAIN_PAGE);

				break;
			default:
				break;
			}

			break;
		case "GetAwardBtn":
//			Debug.Log ("getRewardCdTime:" + getRewardCdTime);
			if (getRewardCdTime ==0)
			{
				LueDuoPersonRankTemplate lueDuoRankTemp = LueDuoPersonRankTemplate.GetLueDuoPersonRankTemplateByRank (plunderResp.gongJinRank);
				string[] rewardLength = lueDuoRankTemp.award.Split ('#');
				
				List<RewardData> rewardDataList = new List<RewardData>();
				foreach (string reward in rewardLength)
				{
					string[] rewardStr = reward.Split(':');
					RewardData data = new RewardData(int.Parse (rewardStr[1]),int.Parse (rewardStr[2]));
					rewardDataList.Add (data);
				}

				LueDuoLianmengRankTemplate tempLate = LueDuoLianmengRankTemplate.GetLueDuoLianmengRankTemplateByRank (plunderResp.gongJinMengRank);
				RewardData data1 = new RewardData(900017,tempLate.award);
				rewardDataList.Add (data1);

				PlunderData.Instance.RewardDataList = rewardDataList;
				PlunderData.Instance.PlunderOperate (PlunderData.OperateType.GET_REWARD);
			}
			else
			{
				//open getRewardWindow
//				Debug.Log ("back");
				SwitchPlunderPage (SwitchPageType.REWARD_PAGE);
				PlunderReward.plunderReward.InItPlunderPRewardPage (plunderResp.gongJinRank);
				PlunderReward.plunderReward.InItPlunderARewardPage (plunderResp.gongJinMengRank);
			}

			break;
		case "SituationBtn":

			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.PLUNDER,true);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Refreshs the plunder page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempJunZhuList">Temp jun zhu list.</param>
	/// <param name="tempAllianceList">Temp alliance list.</param>
	public void RefreshPlunderPage (PlunderData.NextPageReqType tempType,List<JunZhuInfo> tempJunZhuList,List<LianMengInfo> tempAllianceList = null)
	{
		switch (tempType)
		{
		case PlunderData.NextPageReqType.ALLIANCE:

			plunderResp.mengInfos = tempAllianceList;
			plunderResp.junInfos = tempJunZhuList;

			InItPlunderAlliance ();
			InItPlunderOpponent ();

			break;
		case PlunderData.NextPageReqType.JUNZHU:

			plunderResp.junInfos = tempJunZhuList;
			InItPlunderOpponent ();

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Sets the state of the plunder.
	/// </summary>
	void SetPlunderState ()
	{
		StopCoroutine ("PlunderCdTime");

		gongJinLabel.text = QXComData.MoneyName (QXComData.MoneyType.JIFEN) + "：" + plunderResp.gongJin;
		 
		if (plunderResp.all >= plunderResp.nowMaxBattleCount)
		{
			plunderBtnDic["AddNumBtn"].gameObject.SetActive (false);
			
			if (plunderResp.used >= plunderResp.all)
			{
				timeLabel.text = "今日掠夺次数已用尽";
				plunderBtnDic["ResetBtn"].gameObject.SetActive (false);
			
				numsLabel.text = "";
			}
			else
			{
				if (plunderResp.CdTime > 0)//掠夺冷却期
				{
					plunderBtnDic["ResetBtn"].gameObject.SetActive (true);

					numsLabel.text = "今日剩余次数：" + (plunderResp.all - plunderResp.used) + "/" + plunderResp.all;

					cdTime = plunderResp.CdTime;
					StartCoroutine ("PlunderCdTime");
				}
				else
				{
					plunderBtnDic["ResetBtn"].gameObject.SetActive (false);

					numsLabel.text = "";
					timeLabel.text = "今日剩余次数：" + (plunderResp.all - plunderResp.used) + "/" + plunderResp.all;
				}
			}
		}
		else
		{
			if (plunderResp.used >= plunderResp.all)
			{	
				plunderBtnDic["ResetBtn"].gameObject.SetActive (false);
				plunderBtnDic["AddNumBtn"].gameObject.SetActive (true);

				timeLabel.text = "今日剩余次数：" + (plunderResp.all - plunderResp.used) + "/" + plunderResp.all;
				numsLabel.text = "";
			}
			else
			{
				plunderBtnDic["AddNumBtn"].gameObject.SetActive (false);
				
				if (plunderResp.CdTime > 0)//掠夺冷却期
				{
					plunderBtnDic["ResetBtn"].gameObject.SetActive (true);

					numsLabel.text = "今日剩余次数：" + (plunderResp.all - plunderResp.used) + "/" + plunderResp.all;

					cdTime = plunderResp.CdTime;
					StartCoroutine ("PlunderCdTime");
				}
				else
				{
					plunderBtnDic["ResetBtn"].gameObject.SetActive (false);

					timeLabel.text = "今日剩余次数：" + (plunderResp.all - plunderResp.used) + "/" + plunderResp.all;

					numsLabel.text = "";
				}
			}
		}

		m_shengWangDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.LVE_DUO_WINDOW_TIPS_1);
	}

	IEnumerator PlunderCdTime ()
	{
		string minuteStr = "";
		string secondStr = "";
		
		while (cdTime > 0) 
		{
			cdTime --;
			
			timeLabel.text = "掠夺冷却：" + MyColorData.getColorString (104,TimeHelper.GetUniformedTimeString (cdTime));
			
			if (cdTime == 0) 
			{
				plunderResp.CdTime = 0;
				SetPlunderState ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// Sets the state of the get reward.
	/// </summary>
	public void SetGetRewardState ()
	{
//		Debug.Log ("plunderResp.timeToNight：" + plunderResp.timeToNight);
		StopCoroutine ("GetRewardCd");

		if (plunderResp.timeToNight <= 0)
		{
			getRewardTimeLabel.text = "";
			if (plunderResp.timeToNight == 0)
			{
				QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_rankRewardBtn,600154);
			}
			else
			{
				QXComData.ClearEffect (m_rankRewardBtn);
			}
		}
		else
		{
//			QXComData.InstanceEffect (QXComData.EffectPos.TOP,m_rankRewardBtn,600154);
			QXComData.ClearEffect (m_rankRewardBtn);
			getRewardCdTime = plunderResp.timeToNight;
			StartCoroutine ("GetRewardCd");
		}
	}

	IEnumerator GetRewardCd ()
	{
		while (getRewardCdTime > 0)
		{
			getRewardCdTime --;

			getRewardTimeLabel.text = MyColorData.getColorString (104,TimeHelper.GetUniformedTimeString (getRewardCdTime));

			if (getRewardCdTime <= 0)
			{
				plunderResp.timeToNight = 0;
				SetGetRewardState ();
			}

			yield return new WaitForSeconds (1);
		}
	}

	/// <summary>
	/// Refreshs the state of the plunder.
	/// </summary>
	/// <param name="tempRes">Temp res.</param>
	public void RefreshPlunderState (LveConfirmResp tempRes)
	{
		switch (tempRes.isOk)
		{
		case 1:

			plunderResp.CdTime = tempRes.leftCD;
			plunderResp.clearCdYB = tempRes.nextCDYuanBao;
			plunderResp.all = tempRes.all;
			plunderResp.used = tempRes.used;
			plunderResp.gongJin = tempRes.gongJin;
			plunderResp.buyNextBattleCount = tempRes.buyNextBattleCount;
			plunderResp.buyNextBattleYB = tempRes.buyNextBattleYB;
			plunderResp.remainBuyHuiShi = tempRes.remainBuyHuiShi;
			plunderResp.nowMaxBattleCount = tempRes.nowMaxBattleCount;
			
			SetPlunderState ();

			//set warpage plunder times
			{
				PushAndNotificationHelper.SetRedSpotNotification (215, tempRes.all - tempRes.used > 0 ? true : false);
				if (WarPage.m_instance != null)
				{
					WarPage.m_instance.SetPlunderTimes (tempRes.all - tempRes.used,tempRes.all);
				}
			}

			break;
		case 8:

			plunderResp.timeToNight = -1;
			SetGetRewardState ();

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Sets the red point.
	/// </summary>
	/// <param name="isRed">If set to <c>true</c> is red.</param>
	void SetRedPoint (bool isRed)
	{
		plunderResp.hasRecord = isRed;
		recordRedObj.SetActive (isRed);
		PushAndNotificationHelper.SetRedSpotNotification (220, plunderResp.hasRecord);
		if (WarPage.m_instance != null)
		{
			WarPage.m_instance.CheckRedPoint ();
		}
	}

	/// <summary>
	/// Sets the situation red point.
	/// </summary>
	/// <param name="isRed">If set to <c>true</c> is red.</param>
	public void SetSituationRedPoint (bool isRed)
	{
		situationRedObj.SetActive (isRed);
		PushAndNotificationHelper.SetRedSpotNotification (410012, isRed);
//		WarPage.warPage.CheckRedPoint ();
	}

	/// <summary>
	/// Closes the plunder page.
	/// </summary>
	public void ClosePlunderPage ()
	{
		allianceSb.value = 0;
		opponentSb.value = 0;
		isOpenFirst = true;
		PlunderData.Instance.isOpenLueDuo = false;
		gameObject.SetActive (false);
	}

	#endregion
	
	void Update ()
	{
		//掠夺联盟
		float temp = allianceSc.GetSingleScrollViewValue();
		if (temp != -100) 
		{
			//Reset can slide request.
			if (temp > -0.1f && temp < 1.1f)
			{
				canTap = true;
			}
			
			if (canTap) 
			{
				if (PlunderData.Instance.PdAlliancePage == 1)
				{
					if (allianceItemList.Count >= 20)
					{
						if (temp > 1.25f)
						{
							isRefreshToTop = true;
							canTap = false;
							PlunderData.Instance.PdAlliancePage += 1;//向上滑
							PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.ALLIANCE,GetCurNationId ());
						}
					}
				}
				else if (PlunderData.Instance.PdAlliancePage > 1)
				{
					if (allianceItemList.Count >= 20)
					{
						if (temp > 1.25f)
						{
							isRefreshToTop = true;
							canTap = false;
							PlunderData.Instance.PdAlliancePage += 1;//向上滑
							PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.ALLIANCE,GetCurNationId ());
						}
						else if (temp < -0.25f)
						{
							isRefreshToTop = false;
							canTap = false;
							PlunderData.Instance.PdAlliancePage -= 1;//向下滑
							PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.ALLIANCE,GetCurNationId ());
						}
					}
					else
					{
						if (temp < -0.25f)
						{
							isRefreshToTop = false;
							canTap = false;
							PlunderData.Instance.PdAlliancePage -= 1;//向下滑
							PlunderData.Instance.NextPageReq (PlunderData.NextPageReqType.ALLIANCE,GetCurNationId ());
						}
					}
				}
			}
		}

		//掠夺个人排行
		float pScValue = pRankSc.GetSingleScrollViewValue();
		if (pScValue != -100) 
		{
			//Reset can slide request.
			if (pScValue > -0.1f && pScValue < 1.1f)
			{
				isPCanTap = true;
			}

			if (isPCanTap)
			{
				if (PlunderData.Instance.PRankPage == 1)
				{
					if (pRankItemList.Count >= 20)
					{
						if (pScValue > 1.25f)
						{
							isPRankToTop = true;
							isPCanTap = false;
							PlunderData.Instance.PRankPage += 1;//向上滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
				}
				else if (PlunderData.Instance.PRankPage > 1)
				{
					if (pRankItemList.Count >= 20)
					{
						if (pScValue > 1.25f)
						{
							isPRankToTop = true;
							isPCanTap = false;
							PlunderData.Instance.PRankPage += 1;//向上滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);
						}
						else if (pScValue < -0.25f)
						{
							isPRankToTop = false;
							isPCanTap = false;
							PlunderData.Instance.PRankPage -= 1;//向下滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
					else
					{
						if (pScValue < -0.25f)
						{
							isPRankToTop = false;
							isPCanTap = false;
							PlunderData.Instance.PRankPage -= 1;//向下滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
				}
			}
		}

		//掠夺联盟排行
		float aScValue = aRankSc.GetSingleScrollViewValue();
//		Debug.Log ("aRankSc.ResetPosition ():" + aRankSc.GetSingleScrollViewValue ());
		if (aScValue != -100)
		{	
			//Reset can slide request.
			if (aScValue > -0.1f && aScValue < 1.1f)
			{
				isACanTap = true;
			}

			if (isACanTap)
			{
				if (PlunderData.Instance.ARankPage == 1)
				{
					if (aRankItemList.Count >= 20)
					{
						if (aScValue > 1.25f)
						{
							isArankToTop = true;
							isACanTap = false;
							PlunderData.Instance.ARankPage += 1;//向上滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
				}
				else if (PlunderData.Instance.ARankPage > 1)
				{
					if (aRankItemList.Count >= 20)
					{
						if (aScValue > 1.25f)
						{
							isArankToTop = true;
							isACanTap = false;
							PlunderData.Instance.ARankPage += 1;//向上滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);
						}
						else if (aScValue < -0.25f)
						{
							isArankToTop = false;
							isACanTap = false;
							PlunderData.Instance.ARankPage -= 1;//向下滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
					else
					{
						if (aScValue < -0.25f)
						{
							isArankToTop = false;
							isACanTap = false;
							PlunderData.Instance.ARankPage -= 1;//向下滑
							PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);
						}
					}
				}
			}

		}
	}

	#region RankPage

	//个人排行
	public GameObject pRankItemObj;
	public UIScrollView pRankSc;
	public UIScrollBar pRankSb;
	public GameObject pRankGrid;
	private List<GameObject> pRankItemList = new List<GameObject> ();
	public UILabel pRankLabel;
	public UILabel pRankDesLabel;

	private bool isPRankToTop = true;
	public bool IsPRankToTop { set{isPRankToTop = value;} get{return isPRankToTop;} }

	private bool isPCanTap = false;

	private int personalPage;
	public UILabel personalBtnLabel;
	private PlunderData.RankSerchType pSerchType;

	//联盟排行
	public RankingResp allianceResp;
	public GameObject rankItemObj;
	public UIScrollView aRankSc;
	public UIScrollBar aRankSb;
	public GameObject aRankGrid;
	private List<GameObject> aRankItemList = new List<GameObject> ();
	public UILabel aRankLabel;
	public UILabel aRankDesLabel;

	private bool isArankToTop = true;
	public bool IsARankToTop { set{isArankToTop = value;} get{return isArankToTop;} }
	private bool isACanTap = false;

	private int alliancePage;

	public UILabel allianceBtnLabel;
	private PlunderData.RankSerchType aSerchType;

	public List<EventHandler> rankHandlerList = new List<EventHandler> ();


	/// <summary>
	/// Ins it rank page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempResp">Temp resp.</param>
	public void InItRankPage (PlunderData.PlunderRankType tempType,PlunderData.RankSerchType tempSerchType,RankingResp tempResp)
	{
		if (pageType != SwitchPageType.RANK_PAGE)
		{
			SwitchPlunderPage (SwitchPageType.RANK_PAGE);
		}

		switch (tempType)
		{
		case PlunderData.PlunderRankType.PERSONAL_RANK:

			pSerchType = tempSerchType;

			int personalRank = 0;
			int pRankCount = 0;

			for (int i = 0;i < tempResp.gongInfoList.Count;i ++)
			{
				if (JunZhuData.Instance().m_junzhuInfo.id == tempResp.gongInfoList[i].id)
				{
					personalRank = tempResp.gongInfoList[i].rank;
					break;
				}
			}
//			Debug.Log ("personalRank:" + personalRank);
			personalPage = personalRank >= 20 ? (personalRank % 20 > 0 ? personalRank / 20 + 1 : personalRank / 20) : 1;
//			Debug.Log ("personalPage:" + personalPage);
			if (personalPage == CurPage (tempResp.gongInfoList[0].rank))
			{
				pRankCount = tempResp.gongInfoList.Count;
			}
			else
			{
				pRankCount = tempResp.gongInfoList.Count - 1; 
			}

			//reset panel
			if (pRankCount <= 6)
			{
				//				aRankSc.restrictWithinPanel = true;
				aRankSc.ResetPosition ();
			}

			pRankLabel.text = personalRank.ToString ();

			pRankDesLabel.text = pRankCount > 0 ? "" : "暂时没有掠夺个人排行！";
//			pRankItemList = QXComData.CreateGameObjectList (rankItemObj,pRankGrid,pRankCount,pRankItemList);
			pRankItemList = QXComData.CreateGameObjectList (pRankItemObj,pRankCount,pRankItemList);
			pRankGrid.GetComponent<ItemTopCol> ().enabled = pRankItemList.Count < 5 ? true : false;

			for (int i = 0;i < pRankCount;i ++)
			{
				pRankItemList[i].transform.localPosition = new Vector3(0,-65 * i,0);
				PlunderRankItem plunderRank = pRankItemList[i].GetComponent<PlunderRankItem> ();
				plunderRank.InItRankItem (PlunderData.PlunderRankType.PERSONAL_RANK,tempResp.gongInfoList[i]);
			}

			//Reset scroll view.
			pRankSc.UpdateScrollbars(true);
			pRankSb.value = isPRankToTop ? 0.0f : 1.0f;
			pRankSb.ForceUpdate();
			pRankSc.UpdatePosition();

			pRankSb.gameObject.SetActive (pRankItemList.Count > 5 ? true : false);

			if (tempSerchType == PlunderData.RankSerchType.SELF_POS)
			{
				int index = personalRank > 20 ? (personalRank - 1) % 20 : personalRank - 1;
				UISprite widget = pRankItemList[index].GetComponent<UISprite>();
				pRankItemList[index].GetComponent<PlunderRankItem> ().SetBg (true);

				float widgetValue = pRankSc.GetWidgetValueRelativeToScrollView(widget).y;
				if (widgetValue < 0 || widgetValue > 1)
				{
					pRankSc.SetWidgetValueRelativeToScrollView(widget, 0);
					
					//clamp scroll bar value.
					//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
					//set 0.99 and 0.01 cause same bar value not taken in execute.
					float scrollValue = pRankSc.GetSingleScrollViewValue();
					if (scrollValue >= 1) pRankSb.value = 0.99f;
					if (scrollValue <= 0) pRankSb.value = 0.01f;
				}
			}

			personalBtnLabel.text = tempSerchType == PlunderData.RankSerchType.DEFAULT ? "我的名次" : "全部名次";

			isPCanTap = true;

			break;
		case PlunderData.PlunderRankType.ALLIANCE_RANK:

			allianceResp = tempResp;
			aSerchType = tempSerchType;
			int allianceRank = 0;
			int aRankCount = 0;
//			Debug.Log ("lianMengId:" + JunZhuData.Instance().m_junzhuInfo.lianMengId);
			for (int i = 0;i < tempResp.gongInfoList.Count;i ++)
			{
				if ((long)AllianceData.Instance.g_UnionInfo.id == tempResp.gongInfoList[i].id)
				{
					allianceRank = tempResp.gongInfoList[i].rank;
					
					break;
				}
			}
//			Debug.Log ("allianceRank:" + allianceRank);
			alliancePage = allianceRank >= 20 ? (allianceRank % 20 > 0 ? allianceRank / 20 + 1 : allianceRank / 20) : 1;

			if (alliancePage == CurPage (tempResp.gongInfoList[0].rank))
			{
				aRankCount = tempResp.gongInfoList.Count;
			}
			else
			{
				aRankCount = tempResp.gongInfoList.Count - 1; 
			}

			//reset panel
			if (aRankCount <= 6)
			{
//				aRankSc.restrictWithinPanel = true;
				aRankSc.ResetPosition ();
			}

			aRankLabel.text = allianceRank.ToString ();

			aRankDesLabel.text = aRankCount > 0 ? "" : "暂时没有掠夺联盟排行！";
//			aRankItemList = QXComData.CreateGameObjectList (rankItemObj,aRankGrid,aRankCount,aRankItemList);
			aRankItemList = QXComData.CreateGameObjectList (rankItemObj,aRankCount,aRankItemList);

			aRankGrid.GetComponent<ItemTopCol> ().enabled = aRankItemList.Count < 5 ? true : false;

			for (int i = 0;i < aRankCount;i ++)
			{
				aRankItemList[i].transform.localPosition = new Vector3(0,-65 * i,0);
				PlunderRankItem plunderRank = aRankItemList[i].GetComponent<PlunderRankItem> ();
				plunderRank.InItRankItem (PlunderData.PlunderRankType.ALLIANCE_RANK,tempResp.gongInfoList[i]);
			}

			//Reset scroll view.
			aRankSc.UpdateScrollbars(true);
			aRankSb.value = isArankToTop ? 0.0f : 1.0f;
			aRankSb.ForceUpdate();
			aRankSc.UpdatePosition();

			aRankSb.gameObject.SetActive (aRankItemList.Count > 5 ? true : false);

			if (tempSerchType == PlunderData.RankSerchType.SELF_POS)
			{
				int index = allianceRank > 20 ? (allianceRank - 1) % 20 : allianceRank - 1;
				UISprite widget = aRankItemList[index].GetComponent<UISprite>();
				aRankItemList[index].GetComponent<PlunderRankItem> ().SetBg (true);

				float widgetValue = aRankSc.GetWidgetValueRelativeToScrollView(widget).y;
				if (widgetValue < 0 || widgetValue > 1)
				{
					aRankSc.SetWidgetValueRelativeToScrollView(widget, 0);
					
					//clamp scroll bar value.
					//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
					//set 0.99 and 0.01 cause same bar value not taken in execute.
					float scrollValue = aRankSc.GetSingleScrollViewValue();
					if (scrollValue >= 1) aRankSb.value = 0.99f;
					if (scrollValue <= 0) aRankSb.value = 0.01f;
				}
			}

			allianceBtnLabel.text = tempSerchType == PlunderData.RankSerchType.DEFAULT ? "联盟名次" : "全部名次";

			isACanTap = true;

			break;
		default:
			break;
		}

		foreach (EventHandler handler in rankHandlerList)
		{
			handler.m_click_handler -= RankHandlerClickBack;
			handler.m_click_handler += RankHandlerClickBack;
		}
	}

	//当前页
	private int CurPage (int minRank)
	{
		if (minRank > 20)
		{
			return minRank % 20 > 0 ? minRank / 20 + 1 : minRank / 20;
		}
		else
		{
			return 1;
		}
	}

	void RankHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "PersonalBtn":

			if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
			{
				break;
			}
			switch (pSerchType)
			{
			case PlunderData.RankSerchType.DEFAULT:
				PlunderData.Instance.PRankPage = personalPage;
				PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.SELF_POS);
				break;
			case PlunderData.RankSerchType.SELF_POS:
				PlunderData.Instance.PRankPage = 1;
				PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.PERSONAL_RANK,PlunderData.RankSerchType.DEFAULT);
				break;
			default:
				break;
			}
			
			break;
		case "AllianceBtn":

			if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
			{
				break;
			}
			switch (aSerchType)
			{
			case PlunderData.RankSerchType.DEFAULT:
				PlunderData.Instance.ARankPage = alliancePage;
				PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.SELF_POS);
				break;
			case PlunderData.RankSerchType.SELF_POS:
				PlunderData.Instance.ARankPage = 1;
				PlunderData.Instance.PlunderRankListReq (PlunderData.PlunderRankType.ALLIANCE_RANK,PlunderData.RankSerchType.DEFAULT);
				break;
			default:
				break;
			}

			break;
		default:
			break;
		}
	}

	#endregion

	#region RecordPage;

	public UIScrollView recordSc;
	public UIScrollBar recordSb;
	
	public GameObject recordItemObj;
	private List<GameObject> recordItemList = new List<GameObject> ();
	
	private LveBattleRecordResp recordRes;
	
	public UILabel desLabel;

	/// <summary>
	/// Ins it record page.
	/// </summary>
	/// <param name="tempRes">Temp res.</param>
	public void InItRecordPage (LveBattleRecordResp tempRes)
	{
		SwitchPlunderPage (SwitchPageType.RECORD_PAGE);

		desLabel.text = tempRes.info.Count > 0 ? "" : "掠夺记录为空！";

		recordItemList = QXComData.CreateGameObjectList (recordItemObj,tempRes.info.Count,recordItemList);

		for (int i = 0;i < tempRes.info.Count - 1;i++)
		{
			for (int j = 0;j < tempRes.info.Count - i - 1;j ++)
			{
				if (tempRes.info[j].time < tempRes.info[j + 1].time)
				{
					LveBattleItem tempBattle = tempRes.info[j];
					tempRes.info[j] = tempRes.info[j + 1];
					tempRes.info[j + 1] = tempBattle;
				}
			}
		}

		for (int i = 0;i < tempRes.info.Count;i ++)
		{
			recordItemList[i].transform.localPosition = new Vector3(0,-i * 130,0);
			LDRecordItem ldRecord = recordItemList[i].GetComponent<LDRecordItem> ();
			ldRecord.GetRecordInfo (tempRes.info[i]);
		}

		recordSc.UpdateScrollbars (true);

		recordSb.gameObject.SetActive (tempRes.info.Count > 4 ? true : false);
		recordSc.enabled = tempRes.info.Count > 4 ? true : false;
	}

	#endregion
}
