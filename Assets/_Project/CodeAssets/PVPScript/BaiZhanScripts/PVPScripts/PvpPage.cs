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

	private string[] mibaoSkillBgStr = new string[4]{"bulebg","redbg","yellowbg","greybg"};

	private List<string> ruleList = new List<string> ();

	public ScaleEffectController sEffectControl;

	public GameObject pvpObj;

	private bool yd_GetReward = false;
	private bool yd_Store = false;

	void Awake ()
	{
		pvpPage = this;
	}

	void Start ()
	{
		//LanguageTemp.xml:349-357  BAIZHAN_RULE_(1-9)
		for (int i = 0;i < 9;i ++)
		{
			string s = "BAIZHAN_RULE_" + (i + 1);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			
			ruleList.Add (ruleStr);
			//Debug.Log ("rule:" + ruleList[i]);
		}
	}

	/// <summary>
	/// Ins it pvp page.
	/// </summary>
	/// <param name="tempPvpResp">Temp pvp resp.</param>
	public void InItPvpPage (BaiZhanInfoResp tempPvpResp)
	{
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
			handler.m_handler += BtnHandlerCallBack;
		}

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,3);
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
	public UISprite titleSprite;
	public UILabel skillName;
	public UILabel activeNum;
	
	public GameObject warringObj;

	private bool isShowSkillBtnEffect;

	//防守设置
	public void DefensiveSetUp ()
	{
		zhanLiLabel.text = pvpResp.pvpInfo.zhanLi.ToString ();
		
		int miBaoCombId = pvpResp.pvpInfo.zuheId;

		Dictionary<int,MibaoGroup> miBaoGroupDic = new Dictionary<int, MibaoGroup> ();
		foreach (MibaoGroup group in MiBaoGlobleData.Instance ().G_MiBaoInfo.mibaoGroup)
		{
			if (!miBaoGroupDic.ContainsKey (group.zuheId))
			{
				miBaoGroupDic.Add (group.zuheId,group);
			}
		}

		if (miBaoGroupDic.ContainsKey (miBaoCombId))
		{
			MibaoGroup miBaoGroup = miBaoGroupDic[miBaoCombId];
			if (miBaoGroup.hasActive == 1)//激活
			{
				int activeCount = 0;
				foreach (MibaoInfo mibaoInfo in miBaoGroup.mibaoInfo)
				{
					if (mibaoInfo.level > 0 && !mibaoInfo.isLock)
					{
						activeCount ++;
					}
				}

				lockObj.SetActive (false);
				skillIcon.gameObject.SetActive (true);
				miBaoSkillInfoObj.SetActive (true);

				BoxCollider skillBtnBox = pvpBtnHandlerDic["ChangeSkillBtn"].gameObject.GetComponent<BoxCollider> ();
				skillBtnBox.enabled = true;
				UISprite skillBtnSprite = pvpBtnHandlerDic["ChangeSkillBtn"].gameObject.GetComponent<UISprite> ();
				skillBtnSprite.color = Color.white;

				notActiveLabel.text = "";
				miBaoSkillBg.spriteName = MibaoSkillBgColor (miBaoCombId);
				titleSprite.spriteName = miBaoCombId.ToString ();
				
				MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (miBaoCombId);
				skillIcon.spriteName = miBaoSkillTemp.skill.ToString ();
				
				SkillTemplate skillTemp = SkillTemplate.getSkillTemplateById (miBaoSkillTemp.skill);
				skillName.text = NameIdTemplate.GetName_By_NameId (skillTemp.skillName);
				activeNum.text = NameIdTemplate.GetName_By_NameId (miBaoSkillTemp.nameId) + "(" + activeCount + "/3)";

				isShowSkillBtnEffect = false;
			}
			else
			{
				lockObj.SetActive (true);
				skillIcon.gameObject.SetActive (false);
				miBaoSkillInfoObj.SetActive (false);
				miBaoSkillBg.spriteName = MibaoSkillBgColor (4);
				notActiveLabel.text = "未选择可用的组合技能";

				isShowSkillBtnEffect = true;
			}
		}
		else
		{
			lockObj.SetActive (true);
			skillIcon.gameObject.SetActive (false);
			miBaoSkillInfoObj.SetActive (false);
			miBaoSkillBg.spriteName = MibaoSkillBgColor (4);
			notActiveLabel.text = "未选择可用的组合技能";

			isShowSkillBtnEffect = true;
		}
		SkillEffect (true);
		ShowRecordWarring (pvpResp.isShow);
	}

	//是否开启按钮特效
	public void SkillEffect (bool isShow)
	{
		if (isShowSkillBtnEffect)
		{
			QXComData.ShowChangeSkillEffect (isShow,pvpBtnHandlerDic["ChangeSkillBtn"].gameObject,110006);
		}
	}

	//是否有新的对战记录
	void ShowRecordWarring (bool isFlag)
	{
		warringObj.SetActive (isFlag);
		PushAndNotificationHelper.SetRedSpotNotification (300100,isFlag);
	}

	/// <summary>
	/// 0-灰色 1-蓝色 2-黄色 3-红色
	/// </summary>
	/// <param name="id">Identifier.</param>
	public string MibaoSkillBgColor (int id)
	{
		string bgSpriteName = "";
		if (id <= 0 || id > MiBaoGlobleData.Instance ().G_MiBaoInfo.mibaoGroup.Count)
		{
			bgSpriteName = mibaoSkillBgStr[3];
		}
		else
		{
			bgSpriteName = mibaoSkillBgStr[id - 1];
		}
		return bgSpriteName;
	}
	#endregion

	#region Opponent Part
	public UIScrollView opponentSc;
	public UIScrollBar opponentSb;
	public GameObject opponentObj;//对手obj
	public GameObject changeBtn;//换对手btn
	
	private List<GameObject> opponentObjList = new List<GameObject> ();
	private float opponentSbValue;

	private List<EventHandler> opponentHandlerList = new List<EventHandler> ();

	public GameObject opponentWindow;

	//对手信息
	public void OpponentsInfo ()
	{
		if (opponentHandlerList.Count != 0)
		{
			foreach (EventHandler handler in opponentHandlerList)
			{
				handler.m_handler -= OpponentHandlerCallBack;
			}

			opponentHandlerList.Clear ();
		}
	
		//按排名排序
		for (int i = 0;i < pvpResp.oppoList.Count - 1;i ++)
		{
			for (int j = 0;j < pvpResp.oppoList.Count - i - 1;j ++)
			{
				if (pvpResp.oppoList[j].rank > pvpResp.oppoList[j + 1].rank)
				{
					OpponentInfo tempInfo = pvpResp.oppoList[j];
					pvpResp.oppoList[j] = pvpResp.oppoList[j + 1];
					pvpResp.oppoList[j + 1] = tempInfo;
				}
			}
		}

		int opponentCount = pvpResp.oppoList.Count - opponentObjList.Count;
		int exitCount = opponentObjList.Count;
		if (opponentCount > 0)
		{
			for (int i = 0;i < opponentCount;i ++)
			{
				GameObject opponent = (GameObject)Instantiate (opponentObj);
				opponent.SetActive (true);
				opponent.transform.parent = opponentObj.transform.parent;
				opponent.transform.localPosition = new Vector3(0,-105 * (i + exitCount),0);
				opponent.transform.localScale = opponentObj.transform.localScale;
				
				opponentObjList.Add (opponent);

				opponentSc.UpdateScrollbars (true);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs(opponentCount);i ++)
			{
				Destroy (opponentObjList[opponentObjList.Count - 1]);
				opponentObjList.RemoveAt (opponentObjList.Count - 1);

				opponentSc.UpdateScrollbars (true);
			}
		}
		opponentSb.value = opponentSbValue;

		for (int i = 0;i < pvpResp.oppoList.Count;i ++)
		{
			PvpOpponent opponentInfo = opponentObjList[i].GetComponent<PvpOpponent> (); 
			opponentInfo.GetOpponentInfo (pvpResp.oppoList[i]);

			EventHandler opponentHandler = opponentObjList[i].GetComponent<EventHandler> ();
			opponentHandlerList.Add (opponentHandler);
		}

		foreach (EventHandler handler in opponentHandlerList)
		{
			handler.m_handler += OpponentHandlerCallBack;
		}

		opponentSc.enabled = !QXComData.CheckYinDaoOpenState (100180);

		changeBtn.SetActive (true);
		changeBtn.transform.localPosition = new Vector3 (0,-pvpResp.oppoList.Count * 105,0);
	}

	void OpponentHandlerCallBack (GameObject obj)
	{
		SkillEffect (false);
		PvpOpponent opponent= obj.GetComponent<PvpOpponent> ();
		//打开对手详情窗口
		opponentWindow.SetActive (true);
		PvpOpponentInfo opponentInfo = opponentWindow.GetComponent<PvpOpponentInfo> ();
		opponentInfo.ScaleEffect ();
		opponentInfo.InItOpponentWindow (opponent.OpponentInfo);
		opponentInfo.ShowFriendState (true);

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,4);
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
	public GameObject haveNoTimeObj;
	public GameObject haveNoTimeLabel1;
	public UILabel haveNoTimeLabel2;//显示无挑战次数的label
	
	public GameObject timesObj;
	public UILabel conditionsLabel;//当前挑战条件
	public GameObject buyNumBtn;
	public GameObject resetBtn;
	
	private int cdTime;//冷却时间
	public int CdTime
	{
		set{cdTime = value;}
		get{return cdTime;}
	}

	//挑战规则相关
	public void ChallangeRules ()
	{
		if (pvpResp.pvpInfo.time > 0)
		{
			haveNoTimeObj.SetActive (false);
			resetBtn.SetActive (true);
			buyNumBtn.SetActive (false);
			
			if (cdTime == 0)
			{
				cdTime = pvpResp.pvpInfo.time;
				
				StartCoroutine (ChallangeCd ());
			}
		}
		
		else if (pvpResp.pvpInfo.leftTimes > 0)
		{
			conditionsLabel.text = "今日剩余次数：" + pvpResp.pvpInfo.leftTimes + "/" + pvpResp.pvpInfo.totalTimes + "次";
			haveNoTimeObj.SetActive (false);
			resetBtn.SetActive (false);
			buyNumBtn.SetActive (false);
		}
		
		else
		{
			if (pvpResp.pvpInfo.totalTimes < pvpResp.nowMaxBattleCount)
			{
				conditionsLabel.text = "今日剩余次数：" + pvpResp.pvpInfo.leftTimes + "/" + pvpResp.pvpInfo.totalTimes + "次";
				haveNoTimeObj.SetActive (false);
				resetBtn.SetActive (false);
				buyNumBtn.SetActive (true);
			}
			else //挑战次数用完
			{
				conditionsLabel.text = "";
				haveNoTimeObj.SetActive (true);
				resetBtn.SetActive (false);
				buyNumBtn.SetActive (false);
				
				int vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;
				if (vipLevel < 10)
				{
					haveNoTimeLabel2.text = "达到VIP" + (vipLevel + 1) + "每日可最大挑战" + pvpResp.nextMaxBattleCount + "次";
				}
				else
				{
					haveNoTimeLabel1.transform.localPosition = new Vector3(70,0,0);
					haveNoTimeLabel2.text = "";
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

			PvpData.Instance.CanGetWeiWang = pvpResp.canGetweiWang;
			PvpData.Instance.ConfirmReq (PvpData.PVP_CONFIRM_TYPE.PVP_GET_REWARD);

			break;
		case "DuiHuanBtn":

			GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.PVP,GeneralControl.StoreReqType.FREE);

			break;
		case "RecordBtn":

			PvpData.Instance.PvpRecordReq ();

			break;
		case "ChangeSkillBtn":

			PvpActiveState (false);
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);

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

			PvpActiveState (false);
			GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.PVP,ruleList);

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
			SkillEffect (true);
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
			SkillEffect (true);
		}
	}

	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.SetActive(true);
		mChoose_MiBao.transform.parent = pvpObj.transform.parent;
		mChoose_MiBao.transform.localPosition = Vector3.zero;
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();
		mChangeMiBaoSkill.Init ((int)CityGlobalData.MibaoSkillType.PVP_Fangshou, pvpResp.pvpInfo.zuheId);
	}

	//打开按钮特效
	public void OpenSkillEffect (int i)
	{
		SkillEffect (true);
	}

	private bool isTurnToVipPage = false;
	//跳转到充值
	public void TurnToVipPage (int i)
	{
		if (i == 1)
		{
			SkillEffect (true);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100180,4);
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
		PvpActiveState (false);
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
		SkillEffect (setActive);
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

		if (QXComData.CheckYinDaoOpenState (100190) && !yd_GetReward)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100190,2);

			yd_GetReward = true;
		}

		if (QXComData.CheckYinDaoOpenState (100200) && !yd_Store)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);

			yd_Store = true;
		}
	}

	public void CancelBtn ()
	{
		SkillEffect (false);
		sEffectControl.OnCloseWindowClick ();
		sEffectControl.CloseCompleteDelegate += DisActiveObj;
	}

	public void DisActiveObj ()
	{
		foreach (EventHandler handler in pvpBtnHandlerList)
		{
			handler.m_handler -= BtnHandlerCallBack;
		}
		Global.m_isOpenBaiZhan = false;
		PvpData.Instance.IsOpenPvpByBtn = false;
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
		if (isTurnToVipPage)
		{
			TopUpLoadManagerment.m_instance.LoadPrefab(false);
		}
	}
}
