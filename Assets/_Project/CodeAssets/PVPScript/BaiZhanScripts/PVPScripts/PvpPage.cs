using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpPage : MonoBehaviour {

	public static PvpPage pvpPage;

	public BaiZhanInfoResp pvpResp;

	public List<EventHandler> pvpBtnHandlerList = new List<EventHandler>();
	private Dictionary<string,EventHandler> pvpBtnHandlerDic = new Dictionary<string, EventHandler> ();

	private string[] mibaoSkillBgStr = new string[]{"redbg","greybg"};

	public ScaleEffectController sEffectControl;

	public GameObject pvpObj;
	public GameObject topRightObj;

	//variable
	private bool yd_GetReward = false;
	private bool yd_Store = false;
	private bool isOpenFirst = false;
	private bool isTurnToVipPage = false;//是否跳转到vip页面

	void Awake ()
	{
		pvpPage = this;
	}

	void OnDestroy ()
	{
		pvpPage = null;
	}

	void Start ()
	{
		QXComData.LoadMoneyInfoPrefab (topRightObj,true);
	}

	/// <summary>
	/// Ins it pvp page.
	/// </summary>
	/// <param name="tempPvpResp">Temp pvp resp.</param>
	public void InItPvpPage (BaiZhanInfoResp tempPvpResp)
	{
		if (!isOpenFirst)
		{
			isOpenFirst = true;
			sEffectControl.OnOpenWindowClick ();
		}

		pvpResp = tempPvpResp;
		pvpObj.SetActive (true);
		pvpBtnHandlerDic.Clear ();

		isTurnToVipPage = false;

		foreach (EventHandler handler in pvpBtnHandlerList)
		{
//			Debug.Log ("handler.name:" + handler.name);
			pvpBtnHandlerDic.Add (handler.name,handler);
		}

		InItMyRank ();
		DefensiveSetUp ();
		OpponentsInfo ();
		ChallangeRules ();

		foreach (EventHandler handler in pvpBtnHandlerList)
		{
			handler.m_handler -= BtnHandlerCallBack;
			handler.m_handler += BtnHandlerCallBack;
		}

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);
	}

	#region MyRankInfo Part
	public UISprite junXianIcon;//军衔icon
	public UISprite junXian;//军衔称谓
	public UILabel rankLabel;//排名label
	
	public UILabel speedLabel;//威望产生速率
	public UILabel canGetLabel;//可领取威望
	public UILabel haveLabel;//拥有的威望
	
	public GameObject getTipObj;//提示图标
	private bool showGetTipObj = false;//是否显示领取

	//我的排名
	public void InItMyRank ()
	{
		int junXianId = pvpResp.pvpInfo.junXianLevel;//军衔id
		
		junXianIcon.spriteName = "junxian" + junXianId;
		
		junXian.spriteName = "JunXian_" + junXianId;
		
		rankLabel.text = "排名：" + pvpResp.pvpInfo.rank.ToString ();

		UISprite rewardSprite = pvpBtnHandlerDic ["GetRewardBtn"].GetComponent<UISprite> ();
		rewardSprite.color = pvpResp.canGetweiWang > 0 ? Color.white : Color.black;
		BoxCollider rewardBox = pvpBtnHandlerDic ["GetRewardBtn"].GetComponent<BoxCollider> ();
		rewardBox.enabled = pvpResp.canGetweiWang > 0 ? true : false;

		speedLabel.text = MyColorData.getColorString (3,"产出：[016bc5]" + pvpResp.weiWangHour + "[-]" + "威望/小时");
		canGetLabel.text = MyColorData.getColorString (3,"可领：[016bc5]"+ pvpResp.canGetweiWang + "[-]" + "威望");
		haveLabel.text = MyColorData.getColorString (3,"拥有：[016bc5]" + pvpResp.hasWeiWang + "[-]" + "威望");
		
		showGetTipObj = pvpResp.canGetweiWang > 0 ? true : false;
	}
	#endregion	

	#region DefensiveSetUp Part
	public UILabel zhanLiLabel;//战力label
	
	public UISprite miBaoSkillBg;
	
	public UISprite skillIcon;
	public GameObject lockObj;
	
	public UILabel notActiveLabel;
	public GameObject miBaoSkillInfoObj;
	public UISprite skillName;
	public UILabel desLabel;
	public UILabel noSkillLabel;
	
	public GameObject warringObj;

	//防守设置
	public void DefensiveSetUp ()
	{
		zhanLiLabel.text = "战力" + pvpResp.pvpInfo.zhanLi.ToString ();

		lockObj.SetActive (pvpResp.pvpInfo.zuheId > 0 ? false : true);
		noSkillLabel.text = pvpResp.pvpInfo.zuheId <= 0 ? "未配置秘技" : "";
		miBaoSkillInfoObj.SetActive (pvpResp.pvpInfo.zuheId > 0 ? true : false);

		if (pvpResp.pvpInfo.zuheId > 0)
		{
			MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (pvpResp.pvpInfo.zuheId);
			skillIcon.spriteName = miBaoSkillTemp.icon.ToString ();
			skillName.spriteName = miBaoSkillTemp.skill.ToString ();

			MiBaoSkillLvTempLate miBaoSkillLvTemp = MiBaoSkillLvTempLate.GetMiBaoSkillLvTemplateByIdAndLevel (pvpResp.pvpInfo.zuheId,QXComData.GetMiBaoSkillLevel (pvpResp.pvpInfo.zuheId));
			desLabel.text = DescIdTemplate.GetDescriptionById (miBaoSkillLvTemp.skillDesc);
		}
		else
		{
			skillIcon.spriteName = "";
			skillName.spriteName = "";
			desLabel.text = "";
		}

		ShowRecordWarring (pvpResp.isShow);
	}

	//是否有新的对战记录
	void ShowRecordWarring (bool isFlag)
	{
		warringObj.SetActive (isFlag);
//		PushAndNotificationHelper.SetRedSpotNotification (300100,isFlag);
	}

	/// <summary>
	/// 0-灰色 1-蓝色 2-黄色 3-红色
	/// </summary>
	/// <param name="id">Identifier.</param>
	public string MibaoSkillBgColor (int id)
	{
		string bgSpriteName = id > 0 ? mibaoSkillBgStr[id - 1] : mibaoSkillBgStr[3];
		return bgSpriteName;
	}
	#endregion

	#region Opponent Part
	public UIScrollView opponentSc;
	public UIScrollBar opponentSb;
	public GameObject opponentObj;//对手obj
	public GameObject changeBtn;//换对手btn
	
	private List<GameObject> opponentObjList = new List<GameObject> ();

	public GameObject opponentWindow;

	//对手信息
	public void OpponentsInfo ()
	{
		int tempCount = pvpResp.oppoList.Count - opponentObjList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (opponentObj);
				
				obj.SetActive (true);
				obj.transform.parent = opponentObj.transform.parent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				
				opponentObjList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				GameObject.Destroy (opponentObjList[opponentObjList.Count - 1]);
				opponentObjList.RemoveAt (opponentObjList.Count - 1);
			}
		}

		//按排名排序
//		for (int i = 0;i < pvpResp.oppoList.Count - 1;i ++)
//		{
//			for (int j = 0;j < pvpResp.oppoList.Count - i - 1;j ++)
//			{
//				if (pvpResp.oppoList[j].rank > pvpResp.oppoList[j + 1].rank)
//				{
//					OpponentInfo tempInfo = pvpResp.oppoList[j];
//					pvpResp.oppoList[j] = pvpResp.oppoList[j + 1];
//					pvpResp.oppoList[j + 1] = tempInfo;
//				}
//			}
//		}

		for (int i = 0;i < pvpResp.oppoList.Count;i ++)
		{
//			opponentObjList[i].transform.localPosition = new Vector3(0,-100 * i);

			PvpOpponent opponentInfo = opponentObjList[i].GetComponent<PvpOpponent> (); 
			opponentInfo.GetOpponentInfo (pvpResp.oppoList[i]);

			SpringPosition.Begin(opponentObjList[i].gameObject, new Vector3(0,-100 * i), 30f).updateScrollView = true;

			opponentSc.UpdateScrollbars (true);
			EventHandler handler = opponentObjList[i].GetComponent<EventHandler> ();
			handler.m_handler -= OpponentHandlerCallBack;
			handler.m_handler += OpponentHandlerCallBack;
		}
		changeBtn.transform.localPosition = new Vector3 (0,-pvpResp.oppoList.Count * 100 + 145,0);
		opponentSc.enabled = !QXComData.CheckYinDaoOpenState (100200);
	}

	void OpponentHandlerCallBack (GameObject obj)
	{
		PvpOpponent opponent= obj.GetComponent<PvpOpponent> ();
		//打开对手详情窗口
		opponentWindow.SetActive (true);
		PvpOpponentInfo opponentInfo = opponentWindow.GetComponent<PvpOpponentInfo> ();
		opponentInfo.ScaleEffect ();
		opponentInfo.InItOpponentWindow (opponent.OpponentInfo);
		opponentInfo.ShowFriendState (true);
	}

	//刷新百战对手好友状态
	public void RefreshOpponentFriendState ()
	{
		PvpOpponentInfo opponentInfo = opponentWindow.GetComponent<PvpOpponentInfo> ();
		opponentInfo.ShowFriendState (false);
	}

	#endregion

	#region Challenge Rules Part
	/// <summary>
	/// 规则及挑战条件模块
	/// </summary>
	public UILabel numDesLabel;//挑战次数

	public UILabel conditionsLabel;//显示时间

	private int cdTime;//冷却时间
	public int CdTime
	{
		set{cdTime = value;}
		get{return cdTime;}
	}

	//挑战规则相关
	public void ChallangeRules ()
	{
		cdTime = pvpResp.pvpInfo.time;
		StopCoroutine ("ChallangeCd");
		StartCoroutine ("ChallangeCd");

		if (pvpResp.pvpInfo.time > 0)
		{
			numDesLabel.text = "";
			pvpBtnHandlerDic ["ResetBtn"].gameObject.SetActive (true);
			pvpBtnHandlerDic ["BuyTimesBtn"].gameObject.SetActive (false);
		}
		
		else if (pvpResp.pvpInfo.leftTimes > 0)
		{
			numDesLabel.color = Color.white;
			numDesLabel.text = "今日剩余次数：" + pvpResp.pvpInfo.leftTimes + "/" + pvpResp.pvpInfo.totalTimes + "次";
			conditionsLabel.text = "";
			pvpBtnHandlerDic ["ResetBtn"].gameObject.SetActive (false);
			pvpBtnHandlerDic ["BuyTimesBtn"].gameObject.SetActive (false);
		}
		
		else
		{
			if (pvpResp.pvpInfo.totalTimes < pvpResp.nowMaxBattleCount)
			{
				conditionsLabel.text = "今日剩余次数：" + pvpResp.pvpInfo.leftTimes + "/" + pvpResp.pvpInfo.totalTimes + "次";
				numDesLabel.text = "";
				pvpBtnHandlerDic ["ResetBtn"].gameObject.SetActive (false);
				pvpBtnHandlerDic ["BuyTimesBtn"].gameObject.SetActive (true);
			}
			else //挑战次数用完
			{
				conditionsLabel.text = "";
				numDesLabel.color = Color.red;
				pvpBtnHandlerDic ["ResetBtn"].gameObject.SetActive (false);
				pvpBtnHandlerDic ["BuyTimesBtn"].gameObject.SetActive (false);
				
				int vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;
				if (vipLevel < 10)
				{
					numDesLabel.text = "今日挑战次数已用尽\n\n达到VIP" + (vipLevel + 1) + "每日可最大挑战" + pvpResp.nextMaxBattleCount + "次";
				}
				else
				{
					numDesLabel.text = "今日挑战次数已用尽";
				}
			}
		}
	}
	
	IEnumerator ChallangeCd () 
	{	
		while (cdTime > 0) 
		{	
			cdTime --;
			
			int minute = (cdTime/60)%60;
			int second = cdTime%60;

			conditionsLabel.text = minute + "分" + second + "秒后可挑战";
			
			if (cdTime == 0) 
			{	
				pvpResp.pvpInfo.time = 0;
				ChallangeRules ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}
	#endregion

	#region BtnHandlerCallBack Part
	void BtnHandlerCallBack (GameObject obj)
	{
		string textStr = "";
		switch (obj.name)
		{
		case "GetRewardBtn":

//			if (QXComData.CheckYinDaoOpenState (100210))
//			{
//				UIYindao.m_UIYindao.setCloseUIEff ();
//			}
			PvpData.Instance.CanGetWeiWang = pvpResp.canGetweiWang;
			PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_GET_REWARD);

			break;
		case "DuiHuanBtn":

			ShopData.Instance.OpenShop (ShopData.ShopType.WEIWANG);

			break;
		case "RecordBtn":

			PvpData.Instance.PvpRecordReq ();

			break;
		case "ChangePlayerBtn":

			textStr = pvpResp.huanYiPiYB == 0 ? "\n\n首次更换对手免费，确定更换一批对手？" : "\n\n确定使用" + pvpResp.huanYiPiYB + "元宝更换一批对手？";
			Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
			                 QXComData.cancelStr, QXComData.confirmStr, 
			                 ChangeOpponent);

			break;
		case "BuyTimesBtn":

			if (pvpResp.leftCanBuyCount == 0)
			{
				if (JunZhuData.Instance ().m_junzhuInfo.vipLv < 10)
				{
					textStr = "\n今日剩余购买回数为0，充值到vip" + (JunZhuData.Instance ().m_junzhuInfo.vipLv + 1) + "级可购买更多挑战次数\n\n是否跳转到充值？";
					Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
					                 QXComData.cancelStr, QXComData.confirmStr, 
					                 TurnToVipPage);
				}
				else
				{
					textStr = "\n\n今日已无剩余购买回数...";
					Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
					                 QXComData.confirmStr, null, 
					                 OpenSkillEffect);
				}
			}
			else
			{
				textStr = "\n是否使用" + pvpResp.buyNeedYB + "元宝购买" + pvpResp.buyNumber + "次挑战次数？\n\n今日还可购买" + pvpResp.leftCanBuyCount + "回";
				Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                 QXComData.cancelStr, QXComData.confirmStr, 
				                 BuyTimes);
			}

			break;
		case "ResetBtn":

			int cleanCdVipLevel = pvpResp.canCleanCDvipLev;
			if (JunZhuData.Instance ().m_junzhuInfo.vipLv < cleanCdVipLevel)
			{
				textStr = "\n达到vip" + cleanCdVipLevel + "级可清除冷却\n\n是否跳转到充值？";
				Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                 QXComData.cancelStr, QXComData.confirmStr, 
				                 TurnToVipPage);
			}
			else
			{
				textStr = "\n\n是否使用" + pvpResp.cdYuanBao + "元宝清除挑战冷却？";
				Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                 QXComData.cancelStr, QXComData.confirmStr, 
				                 ClearCd);
			}

			break;
		case "RulesBtn":

//			GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.PVP,ruleList);

			GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_HELP_DESC));

			break;
		case "PvpClose":

			CancelBtn ();

			break;
		default:
			break;
		}
	}

	//换一批对手
	void ChangeOpponent (int i)
	{
		if (i == 2)
		{
			if (pvpResp.huanYiPiYB == 0)
			{
				PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_REFRESH_PLARER);
			}
			else
			{
				if (pvpResp.huanYiPiYB > JunZhuData.Instance ().m_junzhuInfo.yuanBao)
				{
					string textStr = "\n\n元宝不足，是否跳转到充值？";
					Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                     QXComData.cancelStr, QXComData.confirmStr, 
					                 TurnToVipPage);
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
			if (pvpResp.cdYuanBao > JunZhuData.Instance ().m_junzhuInfo.yuanBao)
			{
				string textStr = "\n\n元宝不足，是否跳转到充值？";
				Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                 QXComData.cancelStr, QXComData.confirmStr, 
				                 TurnToVipPage);
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
			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao < pvpResp.buyNeedYB)
			{
				string textStr = "\n\n元宝不足，是否跳转到充值？";
				Global.CreateBox(QXComData.titleStr,MyColorData.getColorString (1,textStr), null, null, 
				                 QXComData.cancelStr, QXComData.confirmStr, 
				                 TurnToVipPage);
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
	
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.PVP_Fangshou ),pvpResp.pvpInfo.zuheId );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}
	//打开按钮特效
	public void OpenSkillEffect (int i)
	{

	}
	
	//跳转到充值
	public void TurnToVipPage (int i)
	{
		if (i == 1)
		{

		}
		else
		{
			isTurnToVipPage = true;
			CancelBtn ();
		}
	}

	#endregion

	#region LoadRecordPrefab
	private GameObject recordObj;
	private ZhandouRecordResp pvpRecordResp;
	public void LoadPvpRecordPrefab (ZhandouRecordResp tempRecord)
	{
		pvpRecordResp = tempRecord;
//		PvpActiveState (false);
		if (pvpResp.isShow)
		{
			pvpResp.isShow = false;
			ShowRecordWarring (pvpResp.isShow);
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

	//pvp首页激活状态
	public void PvpActiveState (bool setActive)
	{
		pvpObj.SetActive (setActive);
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

		if (QXComData.CheckYinDaoOpenState (100210) && !yd_GetReward)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100210,1);

			yd_GetReward = true;
		}

		if (QXComData.CheckYinDaoOpenState (100220) && !yd_Store)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100220,1);

			yd_Store = true;
		}
	}

	public void CancelBtn ()
	{
		sEffectControl.OnCloseWindowClick ();
		sEffectControl.CloseCompleteDelegate += DisActiveObj;
	}

	public void DisActiveObj ()
	{
		opponentSb.value = 0;
		isOpenFirst = false;
		Global.m_isOpenBaiZhan = false;
		PvpData.Instance.IsOpenPvpByBtn = false;
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
		if (isTurnToVipPage)
		{
            EquipSuoData.TopUpLayerTip();
        }
	}
}
