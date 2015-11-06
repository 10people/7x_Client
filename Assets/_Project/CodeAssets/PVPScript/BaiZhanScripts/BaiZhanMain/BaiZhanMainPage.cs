using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanMainPage : MonoBehaviour {

	public static BaiZhanMainPage baiZhanMianPage;

	public BaiZhanInfoResp baiZhanResp;//百战返回

	/// <summary>
	/// 我的排名模块
	/// </summary>
	public UISprite junXianIcon;//军衔icon
	public UISprite junXian;//军衔称谓
	public UILabel rankLabel;//排名label

	public EventHandler getRewardBtn;//领取按钮
	
	public UILabel speedLabel;//威望产生速率
	public UILabel canGetLabel;//可领取威望
	public UILabel haveLabel;//拥有的威望
	
	public GameObject getTipObj;//提示图标
	private bool showGetTipObj = false;//是否显示领取

	public GameObject lqFlyNumObj;//弹出的数字

	/// <summary>
	/// 防守设置模块
	/// </summary>
	public UILabel zhanLiLabel;//战力label

	public UISprite miBaoSkillBg;

	public UISprite skillIcon;
	public GameObject lockObj;

	public UILabel notActiveLabel;
	public GameObject miBaoSkillInfoObj;
	public UISprite titleSprite;
	public UILabel skillName;
	public UILabel activeNum;

	public GameObject changeSkillBtn;

	public GameObject warringObj;

	/// <summary>
	/// 对手信息模块
	/// </summary>
	public GameObject opponentObj;//对手obj
	public GameObject changeBtn;//换对手btn

	private List<GameObject> opponentObjList = new List<GameObject> ();

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

	public int cdTime;//冷却时间

	private List<string> ruleList = new List<string> ();

	public ScaleEffectController sEffectControl;

	private bool canGetRewardYinDao = true;
	private bool canDuiHuanYinDao = true;

	private bool isYinDaoStop = false;//是否引导中，阻挡点击
	public bool IsYinDaoStop
	{
		get{return isYinDaoStop;}
	}

	private bool isOpenOpponent = false;//是否打开百战对手详情
	public bool IsOpenOpponent
	{
		set{isOpenOpponent = value;}
		get{return isOpenOpponent;}
	}

	void Awake ()
	{
		baiZhanMianPage = this;
	}

	void Start ()
	{
		getRewardBtn.m_handler += GetRewardBtn;

		InItMyRank ();
		DefensiveSetUp ();
		OpponentsInfo ();
		ChallangeRules ();

		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}

		//LanguageTemp.xml:349-357  BAIZHAN_RULE_(1-9)
		for (int i = 0;i < 9;i ++)
		{
			string s = "BAIZHAN_RULE_" + (i + 1);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			
			ruleList.Add (ruleStr);
//			Debug.Log ("rule:" + ruleList[i]);
		}
	}

	//我的排名
	public void InItMyRank ()
	{
		int junXianId = baiZhanResp.pvpInfo.junXianLevel;//军衔id

		junXianIcon.spriteName = "junxian" + junXianId;

		junXian.spriteName = "JunXian_" + junXianId;

		rankLabel.text = "排名：" + baiZhanResp.pvpInfo.rank.ToString ();

		getRewardBtn.gameObject.SetActive (baiZhanResp.canGetweiWang > 0 ? true : false);
		
		speedLabel.text = MyColorData.getColorString (3,"产出：[016bc5]" + baiZhanResp.weiWangHour + "[-]" + "威望/小时");
		canGetLabel.text = MyColorData.getColorString (3,"可领：[016bc5]"+ baiZhanResp.canGetweiWang + "[-]" + "威望");
		haveLabel.text = MyColorData.getColorString (3,"拥有：[016bc5]" + baiZhanResp.hasWeiWang + "[-]" + "威望");

		showGetTipObj = baiZhanResp.canGetweiWang > 0 ? true : false;
	}

	//防守设置
	public void DefensiveSetUp ()
	{
		zhanLiLabel.text = baiZhanResp.pvpInfo.zhanLi.ToString ();

		int miBaoCombId = baiZhanResp.pvpInfo.zuheId;

		int activeMiBaoCount = 0;

		List<MibaoGroup> mibaoGroup = MiBaoGlobleData.Instance ().G_MiBaoInfo.mibaoGroup;
		for (int i = 0;i < mibaoGroup.Count;i ++)
		{
			if (miBaoCombId == mibaoGroup[i].zuheId)
			{
				if (mibaoGroup[i].hasActive == 1)
				{
					for (int j = 0;j < mibaoGroup[i].mibaoInfo.Count;j ++)
					{
						if (mibaoGroup[i].mibaoInfo[j].level > 0 && !mibaoGroup[i].mibaoInfo[j].isLock)
						{
							activeMiBaoCount ++;
						}
					}

					lockObj.SetActive (false);
					skillIcon.gameObject.SetActive (true);
					miBaoSkillInfoObj.SetActive (true);
					notActiveLabel.text = "";
					MibaoSkillBgColor (miBaoCombId);
					titleSprite.spriteName = miBaoCombId.ToString ();
					
					MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHeId (miBaoCombId);

					skillIcon.spriteName = miBaoSkillTemp.skill.ToString ();
					
					SkillTemplate skillTemp = SkillTemplate.getSkillTemplateById (miBaoSkillTemp.skill);
					skillName.text = NameIdTemplate.GetName_By_NameId (skillTemp.skillName);
					
					activeNum.text = NameIdTemplate.GetName_By_NameId (miBaoSkillTemp.nameId) + "(" + activeMiBaoCount + "/3)";
						
					break;
				}
				else
				{
					lockObj.SetActive (true);
					skillIcon.gameObject.SetActive (false);
					miBaoSkillInfoObj.SetActive (false);
					miBaoSkillBg.spriteName = "greybg";
					notActiveLabel.text = "未选择可用的组合技能";
				}
			}
			else
			{
				lockObj.SetActive (true);
				skillIcon.gameObject.SetActive (false);
				miBaoSkillInfoObj.SetActive (false);
				miBaoSkillBg.spriteName = "greybg";
				notActiveLabel.text = "未选择可用的组合技能";
			}
		}

		ShowChangeSkillEffect (true);
		
		ShowRecordWarring (baiZhanResp.isShow);
	}

	/// <summary>
	/// 是否显示切换技能按钮特效
	/// </summary>
	public void ShowChangeSkillEffect (bool isOpen)
	{
		if (isOpen)
		{
			var mibaoGroupList = MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup;
			
			List<int> zuHeIdList = new List<int> ();
			
			for (int i = 0;i < mibaoGroupList.Count;i ++)
			{
				zuHeIdList.Add (mibaoGroupList[i].zuheId);
			}
			
			if (zuHeIdList.Contains (baiZhanResp.pvpInfo.zuheId))
			{
				Debug.Log ("contains");
				changeSkillBtn.SetActive (true);
				UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
			}
			else
			{
				Debug.Log ("MiBaoGlobleData.Instance ().GetMiBaoskillOpen ():" + MiBaoGlobleData.Instance ().GetMiBaoskillOpen ());
				if (MiBaoGlobleData.Instance ().GetMiBaoskillOpen ())
				{
					changeSkillBtn.SetActive (true);
					UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,changeSkillBtn,
					                                               EffectIdTemplate.GetPathByeffectId(110006));
				}
				else
				{
					UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
				}
			}
		}
		else
		{
			UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);
		}
	}

	/// <summary>
	/// 0-灰色 1-蓝色 2-黄色 3-红色
	/// </summary>
	/// <param name="id">Identifier.</param>
	void MibaoSkillBgColor (int id)
	{
		switch (id)
		{
		case 1:

			miBaoSkillBg.spriteName = "bulebg";

			break;

		case 2:

			miBaoSkillBg.spriteName = "redbg";

			break;

		case 3:

			miBaoSkillBg.spriteName = "yellowbg";

			break;

		case 4:

			miBaoSkillBg.spriteName = "greybg";

			break;
		}
	}

	//是否有新的对战记录
	void ShowRecordWarring (bool isFlag)
	{
		if (isFlag)
		{
			warringObj.SetActive (true);
		}

		else
		{
			warringObj.SetActive (false);
			PushAndNotificationHelper.SetRedSpotNotification (300100,false);
		}
	}

	//清除items
	void ClearItems (List<GameObject> tempObjList)
	{
		foreach (GameObject tempObj in tempObjList)
		{
			Destroy (tempObj);
		}
		tempObjList.Clear ();
	}

	//对手信息
	public void OpponentsInfo ()
	{
		foreach (GameObject opponent in opponentObjList)
		{
			Destroy (opponent);
		}
		opponentObjList.Clear ();

		//按排名排序
		for (int i = 0;i < baiZhanResp.oppoList.Count - 1;i ++)
		{
			for (int j = 0;j < baiZhanResp.oppoList.Count - i - 1;j ++)
			{
				if (baiZhanResp.oppoList[j].rank > baiZhanResp.oppoList[j + 1].rank)
				{
					OpponentInfo tempInfo = baiZhanResp.oppoList[j];
					
					baiZhanResp.oppoList[j] = baiZhanResp.oppoList[j + 1];
					
					baiZhanResp.oppoList[j + 1] = tempInfo;
				}
			}
		}
		
		for (int i = 0;i < baiZhanResp.oppoList.Count;i ++)
		{
			GameObject opponent = (GameObject)Instantiate (opponentObj);
			
			opponent.SetActive (true);
			opponent.name = "Opponent" + (i + 1);
			
			opponent.transform.parent = opponentObj.transform.parent;
			
			opponent.transform.localPosition = new Vector3(0,-i * 105,0);
			
			opponent.transform.localScale = opponentObj.transform.localScale;
			
			opponentObjList.Add (opponent);
			
			BaiZhanOpponent baiZhanOpponent = opponent.GetComponent<BaiZhanOpponent> ();
			baiZhanOpponent.opponentInfo = baiZhanResp.oppoList[i];

			if (baiZhanResp.oppoList[i].junZhuId < 0 || FriendData.Instance.FriendIDList.Contains (baiZhanResp.oppoList[i].junZhuId))
			{
				baiZhanOpponent.SetFriend = true;
			}
			else
			{
				baiZhanOpponent.SetFriend = false;
			}
			baiZhanOpponent.InItOpponentInfo ();
		}

		changeBtn.SetActive (true);
		changeBtn.transform.localPosition = new Vector3 (0,-baiZhanResp.oppoList.Count * 105,0);
	}


	//挑战规则相关
	public void ChallangeRules ()
	{
		if (baiZhanResp.pvpInfo.time > 0)
		{
			haveNoTimeObj.SetActive (false);
			resetBtn.SetActive (true);
			buyNumBtn.SetActive (false);

			if (cdTime == 0)
			{
				cdTime = baiZhanResp.pvpInfo.time;

				StartCoroutine (ChallangeCd ());
			}
		}

		else if (baiZhanResp.pvpInfo.leftTimes > 0)
		{
			conditionsLabel.text = "今日剩余次数：" + baiZhanResp.pvpInfo.leftTimes + "/" + baiZhanResp.pvpInfo.totalTimes + "次";
			haveNoTimeObj.SetActive (false);
			resetBtn.SetActive (false);
			buyNumBtn.SetActive (false);
		}

		else
		{
			if (baiZhanResp.pvpInfo.totalTimes < baiZhanResp.nowMaxBattleCount)
			{
				conditionsLabel.text = "今日剩余次数：" + baiZhanResp.pvpInfo.leftTimes + "/" + baiZhanResp.pvpInfo.totalTimes + "次";
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
					haveNoTimeLabel2.text = "达到VIP" + (vipLevel + 1) + "每日可最大挑战" + baiZhanResp.nextMaxBattleCount + "次";
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
		while (cdTime > 0) {
			
			cdTime --;
			
			int minute = (cdTime/60)%60;
			
			int second = cdTime%60;

			conditionsLabel.text = minute + "分" + second + "秒后可挑战";
			
			if (cdTime == 0) {
				
				baiZhanResp.pvpInfo.time = 0;
				ChallangeRules ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	//更换技能按钮
	public void ChangeMibaoSkill ()
	{
		if (IsYinDaoStop)
		{
			return;
		}
		if (!isOpenOpponent)
		{
			isOpenOpponent = true;
			if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
			{
				return;
			}
			
			MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ();
			
			ShowChangeSkillEffect (false);
			
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadBack);
		}
	}
	void LoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.SetActive(true);
		
		mChoose_MiBao.transform.parent = this.transform.parent;
		
		mChoose_MiBao.transform.localPosition = Vector3.zero;
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();

		mChangeMiBaoSkill.GetRootName ("BaiZhan");

		mChangeMiBaoSkill.Init ((int)CityGlobalData.MibaoSkillType.PVP_Fangshou, baiZhanResp.pvpInfo.zuheId);
	}


	//换一批按钮
	public void ChangeOppoentBtn ()
	{
		if (IsYinDaoStop)
		{
			return;
		}
		if (!isOpenOpponent)
		{
			isOpenOpponent = true;
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ChangeOppoentCallback );
		}
	}
	void ChangeOppoentCallback( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		string titleStr = "提示";
		string str;
		if (baiZhanResp.huanYiPiYB == 0)
		{
			str = "首次更换对手免费，是否确定更换一批对手？";
		}

		else
		{
			str = "确定使用" + baiZhanResp.huanYiPiYB + "元宝更换一批对手？";
		}
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
		             null, cancelStr, confirmStr,ChangeOpponent);
	}
	void ChangeOpponent (int i)
	{
		isOpenOpponent = false;
		if (i == 2)
		{
			ConfirmManager.confirm.ConfirmReq (5,null,0);
		}
	}

	//挑战记录按钮
	public void RecordBtn()
	{
		if (IsYinDaoStop)
		{
			return;
		}
		if (!isOpenOpponent)
		{
			isOpenOpponent = true;
			ShowChangeSkillEffect (false);
			if (baiZhanResp.isShow)
			{
				baiZhanResp.isShow = false;
				ShowRecordWarring (baiZhanResp.isShow);
			}
			
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.TIAOZHAN_RECORD ),
			                        RecordLoadBack);
		}
	}

	void RecordLoadBack(ref WWW p_www,string p_path, Object p_object)
	{	
		GameObject recordObj = (GameObject)Instantiate (p_object);

		recordObj.transform.parent = this.transform.parent;
		recordObj.transform.localPosition = Vector3.zero;
		recordObj.transform.localScale = Vector3.one;
	}

	//领取威望请求
	void GetRewardBtn (GameObject obj)
	{
		if (IsYinDaoStop)
		{
			return;
		}
		Debug.Log ("确定领取");
		ConfirmManager.confirm.ConfirmReq (4,null,baiZhanResp.canGetweiWang);
	}

	/// <summary>
	/// 领取奖励后飘出的数字
	/// </summary>
	public void GetAwardNumFly (int weiWang)
	{
//		GameObject flyNum = (GameObject)Instantiate (lqFlyNumObj);
//		
//		flyNum.SetActive (true);
//		flyNum.transform.parent = lqFlyNumObj.transform.parent;
//		flyNum.transform.localPosition = lqFlyNumObj.transform.localPosition;
//		flyNum.transform.localScale = lqFlyNumObj.transform.localScale;
//
//		GetRewardSuccessObj getSuccess = flyNum.GetComponent<GetRewardSuccessObj> ();
//		getSuccess.RewardFly (weiWang);

		RewardData data = new RewardData (900011,weiWang);
		GeneralRewardManager.Instance ().CreateReward (data);
	}

	//规则说明按钮
	public void RulesBtn ()
	{
		if (IsYinDaoStop)
		{
			return;
		}
		if (!isOpenOpponent)
		{
			isOpenOpponent = true;
			ShowChangeSkillEffect (false);
			GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.PVP,ruleList);
		}
	}

	public void DestroyBaiZhanRoot ()
	{
		ShowChangeSkillEffect (false);
		sEffectControl.CloseCompleteDelegate = CloseEffectControl;
		sEffectControl.OnCloseWindowClick ();
	}

	void CloseEffectControl ()
	{
		BaiZhanData.Instance ().CloseBaiZhan ();
	}

	//刷新百战对手好友状态
	public void RefreshOpponentList (long id)
	{
		for (int i = 0;i < opponentObjList.Count;i ++)
		{
			BaiZhanOpponent baiZhanOpponent = opponentObjList[i].GetComponent<BaiZhanOpponent> ();
			if (baiZhanOpponent.opponentInfo.junZhuId == id)
			{
				baiZhanOpponent.SetFriend = true;

				GameObject window = GameObject.Find ("BaiZhanOpponentInfo");
				BaiZhanOpponentInfo opponentWindow = window.GetComponent<BaiZhanOpponentInfo> ();
				opponentWindow.IsFriend = true;
				opponentWindow.InItOpponentWindow ();

				break;
			}
		}
	}

	void Update ()
	{
		if (showGetTipObj)
		{
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
			getTipObj.transform.localScale = Vector3.zero;
		}

		if (canGetRewardYinDao)
		{
			if(FreshGuide.Instance().IsActive(100190) && TaskData.Instance.m_TaskInfoDic[100190].progress >= 0)
			{
				isYinDaoStop = false;
				canGetRewardYinDao = false;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100190];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			}
		}

		if (canDuiHuanYinDao)
		{
			if(FreshGuide.Instance().IsActive(100200) && TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
			{
				canDuiHuanYinDao = false;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100200];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			}
		}

		if (FreshGuide.Instance ().IsActive (100180) && TaskData.Instance.m_TaskInfoDic[100180].progress < 0)
		{
			isYinDaoStop = true;
		}
	}
}
