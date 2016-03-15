using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanPage : MonoBehaviour {

	public static BaiZhanPage baiZhanPage;

	public BaiZhanInfoResp baiZhanResp;

	public GameObject topRightObj;

	public ScaleEffectController sEffectController;

	private bool yd_GetReward = true;
	public bool Yd_GetReward { set{yd_GetReward = value;} get{return yd_GetReward;}}

	private bool yd_Store = true;
	public bool Yd_Store { set{yd_Store = value;} get{return yd_Store;}}

	private bool isOpenFirst = false;

	private string textStr;

	void Awake ()
	{
		baiZhanPage = this;
	}

	void OnDestroy ()
	{
		baiZhanPage = null;
	}

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (topRightObj);
	}

	public void InItBaiZhanPage (BaiZhanInfoResp tempResp)
	{
		baiZhanResp = tempResp;

		PvpData.Instance.IsPvpPageOpen = true;

		if (!isOpenFirst)
		{
			isOpenFirst = true;
			sEffectController.OnOpenWindowClick ();
		}

		InItMyRank ();
		InItOpponent ();
		InItChallenge ();

		foreach (EventHandler handler in pvpHandlerList)
		{
			handler.m_click_handler -= BaiZhanHandlerListClickBack;
			handler.m_click_handler += BaiZhanHandlerListClickBack;
		}
		if (baiZhanResp.rankAward > 0)
		{
			List<RewardData> tempList = new List<RewardData>();
			tempList.Add (new RewardData(900002,baiZhanResp.rankAward));
			PvpData.Instance.RewardDataList = tempList;
			PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_GET_RANK_REWARD);
		}
		else
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);

			Yd_GetReward = false;
			Yd_Store = false;
			Global.m_isOpenBaiZhan = false;
		}
	}

	#region MyRankInfo

	public UISprite curJunXianIcon;
	public UISprite curJunXian;
	public UISprite nextJunXianIcon;
	public UISprite nextJunXian;
	public UISprite jianTouSprite;

	public UILabel rankLabel;
	public UILabel rankDesLabel;

	private int myRank;
	public int MyRank { set{myRank = value;} get{return myRank;}}

	public UILabel weiWangLabel;
	public UILabel getWeiWangLabel;
	public UILabel speedLabel;

	public GameObject getTipObj;//提示图标
	private bool showGetTipObj;

	public GameObject getRewardBtn;
	public UILabel getRewardLabel;
	public UILabel getRewardTimeLabel;

	public GameObject recordRed;

	public List<EventHandler> pvpHandlerList = new List<EventHandler>();

	public void InItMyRank ()
	{
		BaiZhanTemplate baiZhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (baiZhanResp.pvpInfo.baizhanXMLId);
		
		curJunXianIcon.spriteName = "junxian" + baiZhanTemp.jibie;
		curJunXian.spriteName = "JunXian_" + baiZhanTemp.jibie;

		int maxJiBie = BaiZhanTemplate.getBaiZhanTemplateById (11).jibie;
		bool maxLevel = baiZhanTemp.jibie < maxJiBie ? false : true;

		jianTouSprite.spriteName = maxLevel ? "" : "jiantou";

		curJunXianIcon.transform.localPosition = new Vector3 (-100,maxLevel ? -20 : 0,0);

		nextJunXianIcon.spriteName = maxLevel ? "" : "junxian" + BaiZhanTemplate.getBaiZhanTemplateById (baiZhanResp.pvpInfo.baizhanXMLId + 1).jibie;
		nextJunXian.spriteName = maxLevel ? "" : "JunXian_" + BaiZhanTemplate.getBaiZhanTemplateById (baiZhanResp.pvpInfo.baizhanXMLId + 1).jibie;

		MyRank = baiZhanResp.pvpInfo.historyHighRank;
		rankLabel.text = "排名：" + baiZhanResp.pvpInfo.rank.ToString ();
		rankDesLabel.transform.localPosition = maxLevel ? new Vector3(-250,-20) : new Vector3 (-290,-70,0);
		rankDesLabel.text = MyColorData.getColorString (3,(maxLevel ? "[d80202]已达到最高军衔[-]" : "排名达到[d80202]" + BaiZhanTemplate.getBaiZhanTemplateById (baiZhanResp.pvpInfo.baizhanXMLId + 1).minRank + "[-]进阶") + "\n军衔越高，威望产出越多");

		Debug.Log ("baiZhanResp.pvpInfo.rank:" + baiZhanResp.pvpInfo.rank);

		Debug.Log ("baiZhanResp.canGetweiWang:" + baiZhanResp.canGetweiWang);

		weiWangLabel.text = baiZhanResp.hasWeiWang.ToString ();
		getWeiWangLabel.text = MyColorData.getColorString (3,"可领：[016bc5]"+ baiZhanResp.canGetweiWang + "/" + (baiZhanResp.pvpInfo.rank > 5000 ? BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (5001).weiwangLimit : BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (baiZhanResp.pvpInfo.rank).weiwangLimit) + "[-]" + "威望");
		speedLabel.text = MyColorData.getColorString (3,"产出：[016bc5]" + baiZhanResp.weiWangHour + "[-]" + "威望/小时");;

		UIWidget[] btnSprites = getRewardBtn.GetComponentsInChildren <UIWidget> ();
		foreach (UIWidget sprite in btnSprites)
		{
			sprite.color = baiZhanResp.canGetweiWang > 0 ? Color.white : Color.grey;
		}

		showGetTipObj = baiZhanResp.canGetweiWang > 0 ? true : false;

		getRewardLabel.text = baiZhanResp.rankAward > 0 ? "可领取" : "";

		StopCoroutine ("GetReward");

		if (baiZhanResp.nextTimeTo21 <= 0)
		{
			getRewardTimeLabel.text = "";
		}
		else
		{
			StartCoroutine ("GetReward");
		}

		ShowRecordWarring (baiZhanResp.isShow);
	}

	IEnumerator GetReward ()
	{
		while (baiZhanResp.nextTimeTo21 > 0)
		{
			baiZhanResp.nextTimeTo21 --;

			if (baiZhanResp.nextTimeTo21 <= 0)
			{
				getRewardTimeLabel.text = "";
				baiZhanResp.nextTimeTo21 = 0;
				StopCoroutine ("GetReward");
			}
			else
			{
				getRewardTimeLabel.text = QXComData.TimeFormat (baiZhanResp.nextTimeTo21);
			}
			yield return new WaitForSeconds (1);
		}
	}
	#endregion

	/// <summary>
	/// Refreshs the wei wang.
	/// </summary>
	/// <param name="weiWang">Wei wang.</param>
	public void RefreshWeiWang (int weiWang)
	{
		baiZhanResp.hasWeiWang = weiWang;
		weiWangLabel.text = weiWang.ToString ();
	}

	#region OpponentInfo
	
	public GameObject opponentItemObj;

	private List<GameObject> opponentList = new List<GameObject>();

	public UILabel zhanLiLabel;

	public GameObject opponentWindow;

	private float itemDis = 170;

	public void InItOpponent ()
	{
		zhanLiLabel.text = "战力" + baiZhanResp.pvpInfo.zhanLi;

		opponentList = QXComData.CreateGameObjectList (opponentItemObj,baiZhanResp.oppoList.Count,opponentList);

		for (int i = 0;i < baiZhanResp.oppoList.Count;i ++)
		{
			opponentList[i].name = i.ToString ();
			opponentList[i].transform.localPosition = new Vector3(-340 + itemDis * i,-15,0);

			BaiZhanOpponent opponent = opponentList[i].GetComponent<BaiZhanOpponent> ();
			opponent.InItOpponent (baiZhanResp.oppoList[i]);

			EventHandler handler = opponentList[i].GetComponent<EventHandler> ();
			handler.m_click_handler -= OpponentHandlerClickBack;
			handler.m_click_handler += OpponentHandlerClickBack;
		}
	}

	void OpponentHandlerClickBack (GameObject obj)
	{
		//打开对手详情窗口
		opponentWindow.SetActive (true);
		PvpOpponentInfo opponentWin = opponentWindow.GetComponent<PvpOpponentInfo> ();
		opponentWin.InItOpponentWindow (baiZhanResp.oppoList[int.Parse (obj.name)]);
	}

	//刷新百战对手好友状态
	public void RefreshOpponentFriendState ()
	{
		PvpOpponentInfo opponentInfo = opponentWindow.GetComponent<PvpOpponentInfo> ();
		opponentInfo.ShowFriendState (false);
	}
	#endregion

	#region ChallengeInfo

	public UILabel label1;
	public UILabel label2;

	public GameObject resetBtn;
	public GameObject buyTimeBtn;

	public void InItChallenge ()
	{
		StopCoroutine ("ChallangeCd");
		label2.transform.localPosition = new Vector3 (155,0,0);

		if (baiZhanResp.pvpInfo.time <= 0 || QXComData.CheckYinDaoOpenState (100255))
		{
			label1.color = Color.white;
			if (baiZhanResp.pvpInfo.leftTimes > 0)
			{
				label2.transform.localPosition = new Vector3 (250,0,0);
				label1.text = "";
				label2.text = "今日剩余次数：" + baiZhanResp.pvpInfo.leftTimes + "/" + baiZhanResp.pvpInfo.totalTimes + "次";
				resetBtn.SetActive (false);
				buyTimeBtn.SetActive (false);
			}
			
			else
			{
				if (baiZhanResp.pvpInfo.totalTimes < baiZhanResp.nowMaxBattleCount)
				{
					label1.text = "";
					label2.text = "今日剩余次数：" + baiZhanResp.pvpInfo.leftTimes + "/" + baiZhanResp.pvpInfo.totalTimes + "次";
					resetBtn.SetActive (false);
					buyTimeBtn.SetActive (true);
				}
				else //挑战次数用完
				{
					resetBtn.SetActive (false);
					buyTimeBtn.SetActive (false);
					
					int vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;
					if (vipLevel < 10)
					{
						label1.text = "今日挑战次数已用尽";
						label2.text = "达到VIP" + (vipLevel + 1) + "每日可最多挑战" + baiZhanResp.nextMaxBattleCount + "次";
					}
					else
					{
						label1.text = "";
						label2.text = "今日挑战次数已用尽";
					}
				}
			}
		}
		else
		{
			StartCoroutine ("ChallangeCd");
			label1.color = Color.red;
			label2.text = "今日剩余次数：" + baiZhanResp.pvpInfo.leftTimes + "/" + baiZhanResp.pvpInfo.totalTimes;
			resetBtn.SetActive (true);
			buyTimeBtn.SetActive (false);
		}
	}

	IEnumerator ChallangeCd () 
	{	
		while (baiZhanResp.pvpInfo.time > 0) 
		{	
			baiZhanResp.pvpInfo.time --;
			
			int minute = (baiZhanResp.pvpInfo.time / 60) % 60;
			int second = baiZhanResp.pvpInfo.time % 60;
			
			label1.text = minute + "分" + second + "秒后可挑战";
			
			if (baiZhanResp.pvpInfo.time == 0) 
			{	
				baiZhanResp.pvpInfo.time = 0;
				InItChallenge ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	#endregion

	void BaiZhanHandlerListClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "ShopBtn":

			ShopData.Instance.OpenShop (ShopData.ShopType.WEIWANG);

			break;
		case "RecordBtn":

			PvpData.Instance.PvpRecordReq ();

			break;
		case "CanGetBtn":
//			List<RewardData> tempList = new List<RewardData>();
//			tempList.Add (new RewardData(900002,1000));
//			OpenHighRankRewardWindow (tempList);
			OpenRankRewardWindow ();
			PvpRankReward.rankReward.InItRankReward (baiZhanResp.pvpInfo.rank);

			break;
		case "RankBtn":

			Rank.RootController.CreateRankWindow ();

			break;
		case "RulesBtn":

			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_HELP_DESC));

			break;
		case "GetRewardBtn":

			if (baiZhanResp.nextTimeTo21 == 0)
			{
				BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (baiZhanResp.pvpInfo.baizhanXMLId);
				string[] rewardLength = baizhanTemp.dayAward.Split ('#');

				List<RewardData> rewardDataList = new List<RewardData>();

				for (int i = 0;i < rewardLength.Length;i ++)
				{
					string[] rewardLen = rewardLength[i].Split (':');
					RewardData data = new RewardData(int.Parse (rewardLen[1]),int.Parse (rewardLen[2]));
					rewardDataList.Add (data);
				}

				PvpData.Instance.RewardDataList = rewardDataList;
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_GET_DAY_REWARD);
			}
			else
			{
				OpenResultRewardWindow ();
			}

			break;
		case "GetWeiWangBtn":

			if (baiZhanResp.canGetweiWang > 0)
			{
				PvpData.Instance.CanGetWeiWang = baiZhanResp.canGetweiWang;
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_GET_REWARD);
			}

			break;
		case "PvpClose":

			CancelBtn ();

			break;
		case "RefreshBtn":

			textStr = baiZhanResp.huanYiPiYB == 0 ? "首次更换对手免费，确定更换一批对手？" : "确定使用" + baiZhanResp.huanYiPiYB + "元宝更换一批对手？";
			QXComData.CreateBox (1,textStr,false,RefreshPlayer);

			break;
		case "ResetBtn":

			int cleanCdVipLevel = baiZhanResp.canCleanCDvipLev;
			if (JunZhuData.Instance().m_junzhuInfo.vipLv < cleanCdVipLevel)
			{
				textStr = "V特权等级不足，V特权等级提升到" + cleanCdVipLevel + "级即可重置挑战冷却时间。\n\n是否前往充值？ ";
				QXComData.CreateBox (1,textStr,false,TurnToVipPage);
			}
			else
			{
				textStr = "是否使用" + baiZhanResp.cdYuanBao + "元宝清除挑战冷却？";
				QXComData.CreateBox (1,textStr,false,ClearCd);
			}

			break;
		case "BuyTimesBtn":

			if (baiZhanResp.leftCanBuyCount == 0)
			{
				if (JunZhuData.Instance().m_junzhuInfo.vipLv < QXComData.maxVipLevel)
				{
					string textDes1 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_7);
					string textDes2 = LanguageTemplate.GetText (LanguageTemplate.Text.V_PRIVILEGE_TIPS_8).Replace ('*',char.Parse ((JunZhuData.Instance().m_junzhuInfo.vipLv + 1).ToString ()));
					string textDes3 = LanguageTemplate.GetText (LanguageTemplate.Text.VIPDesc2);
//					textStr = "今日剩余购买次数已用完,V特权等级提升到" + (JunZhuData.Instance().m_junzhuInfo.vipLv + 1) + "级即可购买更多挑战次数。\n\n是否前往充值？";
					textStr = textDes1 + "\n" + textDes2 + "\n" + textDes3;
					QXComData.CreateBox (1,textStr,true,null);
				}
				else
				{
					textStr = "今日已无剩余购买次数...";
					QXComData.CreateBox (1,textStr,true,OpenSkillEffect);
				}
			}
			else
			{
				textStr = "是否使用" + baiZhanResp.buyNeedYB + "元宝购买" + baiZhanResp.buyNumber + "次挑战次数？\n\n今日还可购买" + baiZhanResp.leftCanBuyCount + "回";
				QXComData.CreateBox (1,textStr,false,BuyTimes);
			}

			break;
		default:
			break;
		}
	}

	//换一批对手
	void RefreshPlayer (int i)
	{
		if (i == 2)
		{
			if (baiZhanResp.huanYiPiYB == 0)
			{
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_REFRESH_PLARER);
			}
			else
			{
				if (baiZhanResp.huanYiPiYB > JunZhuData.Instance().m_junzhuInfo.yuanBao)
				{
					textStr = "元宝不足，是否前往充值？";
					QXComData.CreateBox (1,textStr,false,TurnToVipPage);
				}
				else
				{
					PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_REFRESH_PLARER);
				}
			}
		}
	}

	//清除冷却
	public void ClearCd (int i)
	{
		if (i == 2)
		{
			if (baiZhanResp.cdYuanBao > JunZhuData.Instance().m_junzhuInfo.yuanBao)
			{
				textStr = "元宝不足，是否前往充值？";
				QXComData.CreateBox (1,textStr,false,TurnToVipPage);
			}
			else
			{
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_CLEAR_CD);
			}
		}
		else
		{
			
		}
	}
	
	//购买挑战次数
	public void BuyTimes (int i)
	{
		if (i == 2)
		{
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < baiZhanResp.buyNeedYB)
			{
				textStr = "元宝不足，是否前往充值？";
				QXComData.CreateBox (1,textStr,false,TurnToVipPage);
			}
			else
			{
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_BUY_CISHU);
			}
		}
		else
		{
			
		}
	}

	//跳转到充值
	public void TurnToVipPage (int i)
	{
		if (i == 1)
		{
			
		}
		else
		{
			EquipSuoData.TopUpLayerTip();
		}
	}

	//打开按钮特效
	public void OpenSkillEffect (int i)
	{

	}

	void Update ()
	{
		if (showGetTipObj)
		{
			getTipObj.SetActive (true);
			Vector3 tempScale = getTipObj.transform.localScale;
			float addNum = 0.05f;
			if (tempScale == Vector3.one)
			{
				tempScale = Vector3.zero;
				addNum = 0.05f;
			}
			if (tempScale.x < 1)
			{
				if (tempScale.x >= 0.95f)
				{
					addNum = 0.001f;
				}
				
				tempScale.x += addNum;
				tempScale.y += addNum;
				tempScale.z += addNum;
			}
			
			getTipObj.transform.localScale = tempScale;
		}
		else
		{
			getTipObj.SetActive (false);
			getTipObj.transform.localScale = Vector3.zero;
		}
		
		if (QXComData.CheckYinDaoOpenState (100210) && !Yd_GetReward)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100210,1);
			
			Yd_GetReward = true;
		}
		
		if (QXComData.CheckYinDaoOpenState (100220) && !Yd_Store)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100220,1);
			
			Yd_Store = true;
		}
	}

	//是否有新的对战记录
	void ShowRecordWarring (bool isFlag)
	{
		recordRed.SetActive (isFlag);
		PushAndNotificationHelper.SetRedSpotNotification (300105,isFlag);
	}

	#region LoadRecordPrefab
	private GameObject recordObj;
	private ZhandouRecordResp pvpRecordResp;
	public void LoadPvpRecordPrefab (ZhandouRecordResp tempRecord)
	{
		pvpRecordResp = tempRecord;
//		Debug.Log ("baizhan.isred:" + baiZhanResp.isShow);
		if (baiZhanResp.isShow)
		{
			baiZhanResp.isShow = false;
			ShowRecordWarring (baiZhanResp.isShow);
		}
		if (recordObj == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.TIAOZHAN_RECORD ),
			                        RecordLoadBack);
		}
		else
		{
			recordObj.SetActive (true);
			MainCityUI.TryAddToObjectList (recordObj);
			PvpRecord pvpRecord = recordObj.GetComponent<PvpRecord> ();
			pvpRecord.InItRecordPage (pvpRecordResp);
		}
	}
	void RecordLoadBack(ref WWW p_www,string p_path, Object p_object)
	{	
		recordObj = (GameObject)Instantiate (p_object);
		MainCityUI.TryAddToObjectList (recordObj);
		PvpRecord pvpRecord = recordObj.GetComponent<PvpRecord> ();
		pvpRecord.InItRecordPage (pvpRecordResp);
	}
	#endregion

	#region RankRewardInfo
	public GameObject rankRewardWindow;
	void OpenRankRewardWindow ()
	{
		rankRewardWindow.SetActive (true);

	}
	#endregion

	#region RankResultRewardInfo
	public GameObject resultRewardWindow;

	/// <summary>
	/// Opens the result reward window.
	/// </summary>
	void OpenResultRewardWindow ()
	{
		resultRewardWindow.SetActive (true);
		PvpResultReward.resultReward.InItResultReward (baiZhanResp.pvpInfo.rank,baiZhanResp.pvpInfo.baizhanXMLId);
	}

	#endregion

	#region HighRankRewardInfo
	public GameObject highRankRewardObj;

	/// <summary>
	/// Opens the high rank reward window.
	/// </summary>
	/// <param name="tempRewardList">Temp reward list.</param>
	public void OpenHighRankRewardWindow (List<RewardData> tempRewardList)
	{
		highRankRewardObj.SetActive (true);
//		PvpHighRankReward.highReward.InItHighRankReward (tempRewardList);
		Debug.Log ("baiZhanResp:" + baiZhanResp.pvpInfo.rank + "||" + baiZhanResp.pvpInfo.historyHighRank);
		PvpHighRank.highRank.InItHighRankPage (baiZhanResp,tempRewardList);
	}

	#endregion

	public void CancelBtn ()
	{
		sEffectController.OnCloseWindowClick ();
		sEffectController.CloseCompleteDelegate += DisActiveObj;
	}
	
	public void DisActiveObj ()
	{
		//reset value
		{
			isOpenFirst = false;
			PvpData.Instance.IsPvpPageOpen = false;
		}

		//set pvp red point
		{
			PushAndNotificationHelper.SetRedSpotNotification (300103,baiZhanResp.pvpInfo.time > 0 ? false : (baiZhanResp.pvpInfo.leftTimes > 0 ? true : false));
		}

		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
