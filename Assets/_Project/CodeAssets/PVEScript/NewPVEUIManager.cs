using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewPVEUIManager : MYNGUIPanel ,SocketProcessor {

	public static NewPVEUIManager mNewPVEUIManager;

	[HideInInspector]public Level mLevel;

	public  GuanQiaInfo GuanqiaReq;

	public UILabel mTiLi;
	
	public UILabel mTongBi;
	
	public UILabel mYuanBao;

	public PveSaoDangRet saodinfo;

	public NGUILongPress EnergyDetailLongPress1;
	public NGUILongPress EnergyDetailLongPress2;
	public NGUILongPress EnergyDetailLongPress3;
	public static NewPVEUIManager Instance ()
	{
		if (!mNewPVEUIManager)
		{
			mNewPVEUIManager = (NewPVEUIManager)GameObject.FindObjectOfType (typeof(NewPVEUIManager));
		}
		
		return mNewPVEUIManager;
	}
	void Awake()
	{ 
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;

		EnergyDetailLongPress2.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress2.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress2.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress2.OnLongPress = OnEnergyDetailClick2;

		EnergyDetailLongPress3.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress3.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress3.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress3.OnLongPress = OnEnergyDetailClick3;
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		mNewPVEUIManager = null;
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{

		int Star_Id = mLevel.starInfo [0].starId;
		string[] Awardlist = PveStarTemplate.GetAwardInfo (Star_Id);
		int awardid = int.Parse(Awardlist[1]);

		ShowTip.showTip (awardid);
	}
	public void OnEnergyDetailClick2(GameObject go)//显示体力恢复提示
	{
		int Star_Id = mLevel.starInfo [1].starId;
		string[] Awardlist = PveStarTemplate.GetAwardInfo (Star_Id);
		int awardid = int.Parse(Awardlist[1]);
		
		ShowTip.showTip (awardid);
	}
	public void OnEnergyDetailClick3(GameObject go)//显示体力恢复提示
	{
		int Star_Id = mLevel.starInfo [2].starId;
		string[] Awardlist = PveStarTemplate.GetAwardInfo (Star_Id);
		int awardid = int.Parse(Awardlist[1]);
		
		ShowTip.showTip (awardid);
	}
	void Update () {
	
		mTiLi.text = JunZhuData.Instance ().m_junzhuInfo.tili.ToString();
		
		mTongBi.text = JunZhuData.Instance ().m_junzhuInfo.jinBi.ToString();
		
		mYuanBao.text = JunZhuData.Instance ().m_junzhuInfo.yuanBao.ToString();
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	
	#endregion
	public void Init()
	{
		InitStarUI ();
		sendLevelDrop ();
		InItEnemyList (mLevel.type);
		GetAward (mLevel.type);

		if( ConfigTool.GetBool(ConfigTool.CONST_QUICK_CHOOSE_LEVEL))
		{
			TestBtn.SetActive(true);
		}
		else
		{
			TestBtn.SetActive(false);
		}
	}
	public void  sendLevelDrop()
	{
		GuanQiaInfoRequest EnemyandDropInfo = new GuanQiaInfoRequest ();
		
		MemoryStream EnemyStream = new MemoryStream ();
		
		QiXiongSerializer Enemyer = new QiXiongSerializer ();
		
		EnemyandDropInfo.type = mLevel.type;

		EnemyandDropInfo.guanQiaId = mLevel.guanQiaId;
		
		Enemyer.Serialize (EnemyStream,EnemyandDropInfo);
		
		byte[] t_protof;
		
		t_protof = EnemyStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_GuanQia_Request, ref t_protof,ProtoIndexes.PVE_GuanQia_Info.ToString());
	}
	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.PVE_GuanQia_Info: 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();	
				
				GuanQiaInfo tempInfo = new GuanQiaInfo();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				GuanqiaReq = tempInfo;

				InitData();

				return true;
			}
			case ProtoIndexes.S_PVE_SAO_DANG:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				PveSaoDangRet tempInfo = new PveSaoDangRet();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				saodinfo = tempInfo;
				
				Debug.Log("请求扫荡是数据返回了。。。");
		
				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
				sendLevelDrop();
				
				popRewardUI();
				
				return true;
			}
			case ProtoIndexes.S_PVE_Reset_CQ:
			{
				sendLevelDrop();
				Debug.Log("重置成功了。。。");
				return true;
			}
			case ProtoIndexes.PVE_STAR_REWARD_GET_RET: 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				PveStarGetSuccess tempInfo = new PveStarGetSuccess();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				if (tempInfo != null)
				{
					foreach(StarInfo  m_item in mLevel.starInfo )
					{
						if(m_item.starId == tempInfo.s_starNum)
						{
							string[] Awardlist = PveStarTemplate.GetAwardInfo (tempInfo.s_starNum);
							
							PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (tempInfo.s_starNum);
						
							CommonItemTemplate mCom = CommonItemTemplate.getCommonItemTemplateById (int.Parse(Awardlist[1]));
							
							int Awardsnum = int.Parse(Awardlist[2]);

							int Award_id = mCom.icon;

							RewardData data = new RewardData ( Award_id, Awardsnum); 

							GeneralRewardManager.Instance ().CreateReward (data); 
							
							break;
						}
					}
					foreach(Pve_Level_Info m_Lv in MapData.mapinstance.Pve_Level_InfoList )
					{
						if(m_Lv.litter_Lv.guanQiaId == tempInfo.guanQiaId)
						{
							for(int j = 0 ; j < m_Lv.litter_Lv.starInfo.Count; j++)
							{
								if(m_Lv.litter_Lv.starInfo[j].starId == tempInfo.s_starNum)
								{
									m_Lv.litter_Lv.starInfo[j].getRewardState = true;
									
									m_Lv.ShowBox();
									
									break;
								}
							}
							break;
						}
					}
		
				
				}
				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}
	void popRewardUI()
	{
		ColsePVEGuid ();
	
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),LoadResourceCallback);
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
		GameObject obj = GameObject.Find ("NewPVEUI(Clone)");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localPosition = new Vector3 (0,0,0);
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		SaoDangManeger mSaoDangManeger = tempOjbect.GetComponent<SaoDangManeger>();
		
		mSaoDangManeger.m_PveSaoDangRet = saodinfo;
		
		mSaoDangManeger.SaodangType = 1;
		
		mSaoDangManeger.Init ();
	}
	private int tiaoZhanNum;
	int sdTotleTime = 0;
	int sdCountTime = 0;
	public void InitData()
	{
		TiliCost.text = "x"+GuanqiaReq.tili.ToString();
		 sdTotleTime = 0;
		 sdCountTime = 0;
		switch(mLevel.type)
		{
		case 0:
			break;
		case 1:
			ToDayTime.gameObject.SetActive(false);

			 sdTotleTime =  GuanqiaReq.jySaoDangDayTimes;//扫荡总次数
			 sdCountTime = sdTotleTime -GuanqiaReq.jySaoDangUsedTimes;

			TodayAllTime.text = "今日扫荡次数："+sdCountTime.ToString () + "/" + sdTotleTime.ToString ();
			break;
		case 2:
			tiaoZhanNum = GuanqiaReq.cqDayTimes- GuanqiaReq.cqPassTimes;
			sdTotleTime = GuanqiaReq.cqSaoDangDayTimes;
			sdCountTime = sdTotleTime - GuanqiaReq.cqSaoDangUsedTimes;
			ToDayTime.text = "今日剩余挑战"+(GuanqiaReq.cqDayTimes-GuanqiaReq.cqPassTimes).ToString ()+"/"+GuanqiaReq.cqDayTimes.ToString()+"次";
			TodayAllTime.text = sdCountTime.ToString () + "/" + sdTotleTime.ToString ();
			break;
		default:
			break;
		}

		ShowMiBaoIcon ();
	}
	public void ShowMiBaoIcon()
	{
		if(GuanqiaReq.zuheId <= 0)
		{
			MiBaoIcon.spriteName = "";
		}
		else
		{
			MiBaoSkillTemp mMiBaoSkill  = MiBaoSkillTemp.getMiBaoSkillTempBy_id(GuanqiaReq.zuheId);
			
			MiBaoIcon.spriteName = mMiBaoSkill.icon.ToString();
		}
	}

	/// <summary>
	/// 
	public UISprite MiBaoIcon;
	public UILabel LevelName;
	public UILabel desLabel;
	public UILabel Person_name;//描述label
	public UILabel LevelWanFa;
	public UILabel LevelTypeLabel;
	public UILabel IntroZHanLi;
	public UILabel my_Zhanli;
	public UILabel[] Conditions;
	public UISprite recMibaoSkill;
	public GameObject[] DisAble_Boxs;
	public GameObject[] Active_Boxs;
	public UISprite WinType;

	public GameObject notCrossIcon;
	public UISprite[] starSpriteList;

	public UILabel TiliCost;

	public UILabel ToDayTime;// 挑战次数

	public UILabel TodayAllTime;

	public GameObject BtnRoot;

	public GameObject Saodang1;

	public GameObject  Saodang10;

	public GameObject Saodang;
	/// </summary>

	public void InitStarUI()
	{
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (mLevel.guanQiaId);
		
		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);

		LevelWanFa.text = m_item.wanfaType;
		IntroZHanLi.text = m_item.recZhanli.ToString ();
		my_Zhanli.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString ();

		string []s = M_Name.Split(' ');
		if(s.Length > 1)
		{
			LevelName.text = "[b]"+s[1];
		}
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
	
		desLabel.text = descStr;
	
		recMibaoSkill.spriteName = m_item.recMibaoSkill.ToString ();

		switch (mLevel.type)
		{
		case 0:
			
			Putong();
			break;
		case 1:
			JingYIng();
			break;
		case 2:
			Chuanqi();
			
			break;
			
		default:break;
		}
		ShowPVEGuid ();
	}
	private void Putong()
	{
		notCrossIcon.SetActive (false);
		BtnRoot.SetActive (false);
		LevelTypeLabel.text = "[b]普通关卡";
		int count = 3;
		for(int i = 0; i< count; i++)
		{
			Conditions[i].text = "无星级条件";
			DisAble_Boxs[i].SetActive(false);
			Active_Boxs[i].SetActive(false);
		}
	}
	private void JingYIng()
	{
		BtnRoot.SetActive (true);
		Saodang1.SetActive (true);
		Saodang10.SetActive (true);
		Saodang.SetActive (false);
	
		LevelTypeLabel.text = "[b]精英关卡";
		int count = mLevel.starInfo.Count;
		if(count > 3 )
		{
			count = 3;
		}
		for(int i = 0; i< count; i++)
		{
			int Star_Id = mLevel.starInfo [i].starId;
			
			PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (Star_Id);
			
			string mDes = DescIdTemplate.GetDescriptionById (mPveStarTemplate.desc);
			
			Conditions[i].text = mDes;
			if(mLevel.starInfo [i].finished)
			{
				starSpriteList[i].spriteName = "BigStar";
				if(!mLevel.starInfo [i].getRewardState)
				{
					Active_Boxs[i].SetActive(true);
					BoxBtn mBoxBtn = Active_Boxs[i].GetComponent<BoxBtn>();
					mBoxBtn.m_Lev = mLevel;
					mBoxBtn.Star_Id = mLevel.starInfo [i].starId;
				}
				DisAble_Boxs[i].SetActive(true);
			}

		}
		if(mLevel.s_pass)
		{
			notCrossIcon.SetActive (true);
			
			if(mLevel.win_Level == 1)
			{
				WinType.spriteName = "X_Victoyr";
			}
			if(mLevel.win_Level == 2)
			{
				WinType.spriteName = "L_Victoyr";
			}
			if(mLevel.win_Level == 3)
			{
				WinType.spriteName = "Prefect_Victor";
			}
		}
		else
		{
			notCrossIcon.SetActive (false);
		}
	}
	private void Chuanqi()
	{
		BtnRoot.SetActive (true);
		Saodang1.SetActive (false);
		Saodang10.SetActive (false);
		Saodang.SetActive (true);

		LevelTypeLabel.text = "[b]传奇关卡";
		int count = 3;
		for(int i = 0; i< count; i++)
		{
			Conditions[i].text = "无星级条件";
			DisAble_Boxs[i].SetActive(false);
			Active_Boxs[i].SetActive(false);
		}
		if(mLevel.chuanQiPass)
		{
			notCrossIcon.SetActive (true);
			
			if(mLevel.pingJia == 1)
			{
				WinType.spriteName = "X_Victoyr";
			}
			if(mLevel.pingJia == 2)
			{
				WinType.spriteName = "L_Victoyr";
			}
			if(mLevel.pingJia == 3)
			{
				WinType.spriteName = "Prefect_Victor";
			}
		}
		else
		{
			notCrossIcon.SetActive (false);
		}
	}

	/// <summary>
	/// Changes the mi bao button.
	/// </summary>
	public void ChangeMiBaoBtn() 
	{
	    ColsePVEGuid ();
		
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);
	}
	
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		
		mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.PveSend ), GuanqiaReq.zuheId );
		
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}
	/// <summary>
	/// Enters the battle button.
	/// </summary>
	public void EnterBattleBtn()
	{

		if(IsTest)
		{
			CityGlobalData.m_tempSection = Test_Section;
			
			CityGlobalData.m_tempLevel = Test_Level;
			Global.m_isOpenPVP = true;
			EnterBattleField.EnterBattlePve(Test_Section, Test_Level, LevelType.LEVEL_NORMAL);
			
			return;
		}

		if(JunZhuData.Instance().m_junzhuInfo.tili < GuanqiaReq.tili)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTiLiLoadBack);
			
			return ;
		}
		if(mLevel.type == 2)
		{
			if(tiaoZhanNum <= 0)
			{
				ReSettingLv();
				
				return;
			}
		}
		CityGlobalData.PveLevel_UI_is_OPen = false;
		int  my_tempSection = MapData.mapinstance.myMapinfo.s_section;
		
		int a = mLevel.guanQiaId;
		
		int my_tempLevel = a%10;
		
		CityGlobalData.m_tempSection = my_tempSection;
		
		CityGlobalData.m_tempLevel = my_tempLevel;
		Global.m_isOpenPVP = true;

//		Debug.Log ("my_tempSection = "+my_tempSection);
//		Debug.Log ("my_tempLevel = "+my_tempLevel);
//		Debug.Log ("mLevel.type = "+mLevel.type);

		if(CityGlobalData.PT_Or_CQ )
		{
			if(mLevel.type == 1)
			{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_ELITE);
			}
			else{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_NORMAL);
			}
		}
		else{
			EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_TALE);
		}

	}
	public  void ReSettingLv(){
		
		int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(10);
		if(JunZhuData.Instance().m_junzhuInfo.vipLv < viplv)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loadback1);
			return;
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockResttingBox);
	}
	public void loadback1(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_DESC);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null);
	}
	public void LockResttingBox(ref WWW p_www,string p_path, Object p_object)
	{
		if(GuanqiaReq.cqResetLeft <= 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockResttingtimesload);
			
			return;
		}
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.RESTTING_CQ_TITLE);
		string str1 = "\r\n"+LanguageTemplate.GetText (LanguageTemplate.Text.USE_YUANBAO)+GuanqiaReq.cqResetPay.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.YUANBAO_RESTTING);//;LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.TODAY_RESETTING)+GuanqiaReq.cqResetLeft.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR3);
		string btnStr1 = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		string btnStr2 = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		//Debug.Log ("cqResetPay = "+GuanqiaReq.cqResetPay);
		//.Log ("cqResetLeft = "+GuanqiaReq.cqResetLeft);
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1),MyColorData.getColorString (1,str2),null,btnStr1,btnStr2,ShowResettingLv);
	}
	void ShowResettingLv(int i)
	{
		if(i == 1)
		{
			return;
		}
		if(!mLevel.chuanQiPass)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadCQNo_Pass);
			return;
		}
		if( GuanqiaReq.cqResetLeft > 0)
		{
			if(JunZhuData.Instance().m_junzhuInfo.yuanBao < GuanqiaReq.cqResetPay)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loadback2);
				
			}
			else
			{
				ResetCQTimesReq mResetCQTimesReq = new ResetCQTimesReq();
				MemoryStream mResetCQTimesReqStream = new MemoryStream ();
				QiXiongSerializer mResetCQTimesReqer = new QiXiongSerializer ();
				mResetCQTimesReq.guanQiaId = mLevel.guanQiaId;
				mResetCQTimesReqer.Serialize (mResetCQTimesReqStream,mResetCQTimesReq);
				byte[] t_protof;
				t_protof = mResetCQTimesReqStream.ToArray();
				//Debug.Log("Lv_Info.guanQiaId = "+Lv_Info.guanQiaId);
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_PVE_Reset_CQ, ref t_protof,ProtoIndexes.S_PVE_Reset_CQ.ToString());
				
			}
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockResttingtimesload);
		}
	
	}
	public void loadback2(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.YUANBAO_LACK_TITLE);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_LACK_YUANBAO);
		string btnStr1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		uibox.setBox(titleStr, null, MyColorData.getColorString(1, str),null, btnStr1, btnStr, TopUpShow);
	}
	void TopUpShow(int index)
	{
		EquipSuoData.TopUpLayerTip();
	}
	public void LoadCQNo_Pass(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = "还未通关";//LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_DESC);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null);
	}
	public void LockResttingtimesload(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.RESETTING_FINSHED);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null,null,null);
	}
	void LockTiLiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.TITITLE);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTili);
	}
	void getTili(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true,false,false);
		}
	}
	public void ColsePVEGuid()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	public	void ShowPVEGuid()
	{
		if(FreshGuide.Instance().IsActive(100020)&& TaskData.Instance.m_TaskInfoDic[100020].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-1");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100030)&& TaskData.Instance.m_TaskInfoDic[100030].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-2");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100030];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		
		if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-3");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		
		if(FreshGuide.Instance().IsActive(100080)&& TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-4");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100080];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100090)&& TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-5");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100090];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		
		if(FreshGuide.Instance().IsActive(100120)&& TaskData.Instance.m_TaskInfoDic[100120].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-6");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100120];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100130)&& TaskData.Instance.m_TaskInfoDic[100130].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-7");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100130];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100320)&& TaskData.Instance.m_TaskInfoDic[100320].progress >= 0)
		{
			//			Debug.Log("攻打传奇关卡");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100320];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		
			return;
		}
	}
	/// <summary>
	/// Ins it enemy list.
	/// </summary>
	/// <param name="leveType">Leve type.</param>
	/// 
	[HideInInspector]
	public GameObject IconSamplePrefab;
	
	List<int> GetPveEnemyId = new List<int>();
	
	List<int> soldires = new List<int>();
	List<int> heros = new List<int>();
	List<int> Bosses = new List<int>();
	List<int> Zhi_Ye = new List<int>();
	
	int EnemyNumBers = 0;//显示敌人数量
	int distance = 100;//敌人头像距离
	int countDistance = 250;//偏移量
	List <GameObject> mHerosIcon = new List<GameObject> (); 

	public GameObject EnemyRoot;
	public GameObject AwardRoot;
	public void InItEnemyList(int leveType)
	{
		foreach(GameObject con in mHerosIcon)
		{
			Destroy(con);
			
		}
		mHerosIcon.Clear ();
		
		soldires.Clear();
		heros.Clear();
		Bosses.Clear();
		Zhi_Ye.Clear();
//		if (leveType == 0)
//		{
//			EnemyNumBers = 6;
//		}
//		
//		else if (leveType == 1 || leveType == 2)
//		{
//			EnemyNumBers = 4;
//		}
		EnemyNumBers = 4;
		//Debug.Log ("GetPveTempID.CurLev = "+GetPveTempID.CurLev);
		if(!CityGlobalData.PT_Or_CQ)
		{
			LegendPveTemplate L_pvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(Pve_Level_Info.CurLev);
			
			int npcid = L_pvetemp.npcId;
			
			List<LegendNpcTemplate> mLegendNpcTemplateList = LegendNpcTemplate.GetLegendNpcTemplates_By_npcid(npcid);
			//Debug.Log("npcid t = " +npcid);
			foreach(LegendNpcTemplate mLegendNpcTemplate in mLegendNpcTemplateList)
			{
				int icn = int.Parse(mLegendNpcTemplate.icon);
				
				if(icn != 0 && mLegendNpcTemplate.type == 4&& !Bosses.Contains(mLegendNpcTemplate.EnemyId)) // boss
				{
					Bosses.Add(mLegendNpcTemplate.id);
				}
				if(icn != 0 && mLegendNpcTemplate.type == 3&& !heros.Contains(mLegendNpcTemplate.id)) // hero
				{				
					heros.Add(mLegendNpcTemplate.id);
				}
				if(icn != 0 && mLegendNpcTemplate.type == 2&& !soldires.Contains(mLegendNpcTemplate.id)) // Solider
				{
					soldires.Add(mLegendNpcTemplate.id);
				}
			}
			
			for (int i = 0; i < soldires.Count-1; i ++)
			{
				
				LegendNpcTemplate m_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (soldires [i]);
				
				for(int j = i+1; j < soldires.Count; )
				{
					LegendNpcTemplate j_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (soldires [j]);
					if(m_LegendNpcTemplate.profession == j_LegendNpcTemplate.profession)
					{
						
						soldires.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			
			for (int i = 0; i < heros.Count-1; i ++)
			{
				//Debug.Log("heros[i] = "+heros[i]);
				LegendNpcTemplate m_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (heros [i]);
				
				for(int j = i+1; j < heros.Count; )
				{
					LegendNpcTemplate j_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (heros [j]);
					if(m_LegendNpcTemplate.profession == j_LegendNpcTemplate.profession)
					{
						
						heros.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			//GetPveEnemyId = LegendNpcTemplate.GetEnemyId_By_npcid(npcid);
		}
		else
		{
			PveTempTemplate pvetemp = PveTempTemplate.GetPveTemplate_By_id(Pve_Level_Info.CurLev);
			
			int npcid = pvetemp.npcId;
			//Debug.Log("npcid  = " +npcid);
			//Debug.Log("npcid = " +npcid);
			List<NpcTemplate> mNpcTemplateList = NpcTemplate.GetNpcTemplates_By_npcid(npcid);
			
			//Debug.Log("mNpcTemplateList.count = "+mNpcTemplateList.Count);
			
			foreach(NpcTemplate mNpcTemplate in mNpcTemplateList)
			{
				int icn = int.Parse(mNpcTemplate.icon);
				if(icn != 0&&mNpcTemplate.type == 4&& !Bosses.Contains(mNpcTemplate.id)) // boss
				{
					Bosses.Add(mNpcTemplate.id);
				}	
				if(icn != 0 && mNpcTemplate.type == 3&& !heros.Contains(mNpcTemplate.id)) // hero
				{
					heros.Add(mNpcTemplate.id);
				}
				if(icn != 0 && mNpcTemplate.type == 2&& !soldires.Contains(mNpcTemplate.id)) // Solider
				{
					soldires.Add(mNpcTemplate.id);
					
				}
			}
			for (int i = 0; i < soldires.Count-1; i ++)
			{
				
				NpcTemplate m_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (soldires [i]);
				
				for(int j = i+1; j < soldires.Count; )
				{
					NpcTemplate j_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (soldires [j]);
					if(m_NpcTemplate.profession == j_NpcTemplate.profession)
					{
						
						soldires.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			
			
			for (int i = 0; i < heros.Count-1; i ++)
			{
				//Debug.Log("heros[i] = "+heros[i]);
				NpcTemplate m_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (heros [i]);
				
				for(int j = i+1; j < heros.Count; )
				{
					NpcTemplate j_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (heros [j]);
					if(m_NpcTemplate.profession == j_NpcTemplate.profession)
					{
						heros.Remove(heros [j]);
					}else
					{
						j ++;
					}
				}
				
			}
			
		}
		
		
		getEnemyData();
	}
	
	void getEnemyData()
	{
		//List<string> EyName = new List<string>(GetPveTempID.NewEnemy.Keys);
		
		int bossnum = Bosses.Count;
		int heronum = heros.Count;
		int solder = soldires.Count;
		
		//Debug.Log ("boss个数：" + bossnum);
		//Debug.Log ("hero个数：" + heronum);
		//Debug.Log ("soldier个数：" + solder);
		
		if (bossnum > 0)//BOSS个数不为0
		{
			if (bossnum < EnemyNumBers)//boss不大于大于6个
			{
				if (heronum > 0)//w武将个数大于0
				{
					if (heronum + bossnum < EnemyNumBers)//w武将和bpss的总个数小于6
					{
						if (solder > 0)
						{
							if (heronum + bossnum + solder > EnemyNumBers)
							{
								createBoss(bossnum);
								createHeros(heronum);
								createSoliders(EnemyNumBers - (bossnum + heronum));
							}
							else
							{
								createBoss(bossnum);
								createHeros(heronum);
								createSoliders(solder);
							}
						}
						else
						{
							createBoss(bossnum);
							createHeros(heronum);
						}
					}
					else
					{//boss和武将的和大于6了 只创建6个
						createBoss(bossnum);
						createHeros(EnemyNumBers - bossnum);
					}
				}
				else
				{
					//ww武将为0
					if (solder > 0)
					{
						if (solder + bossnum > EnemyNumBers)
						{
							createBoss(bossnum);
							createSoliders(EnemyNumBers - bossnum);
						}
						else
						{
							createBoss(bossnum);
							createSoliders(solder);
						}
					}
					else
					{
						createBoss(bossnum);
					}
				}
			}
			else
			{
				//boss的个数大于6
				createBoss(EnemyNumBers);
			}
		}
		else
		{
			//假如boss的个数为0000000000
			if (heronum > 0)//w武将个数大于0
			{
				if (heronum < EnemyNumBers)//w武将和bpss的总个数小于6
				{
					if (solder > 0)
					{
						if (heronum + solder <= EnemyNumBers)
						{
							createHeros(heronum);
							createSoliders(solder);
						}
						else
						{
							createHeros(heronum);
							createSoliders(EnemyNumBers - heronum);
						}
					}
					else
					{
						createHeros(heronum);
					}
				}
				else
				{
					createHeros(EnemyNumBers);
				}
			}
			else
			{
				if (solder > EnemyNumBers)
				{
					createSoliders(EnemyNumBers);
				}
				else
				{
					createSoliders(solder);
				}
			}
		}
		//this.gameObject.GetComponent<UIGrid>().repositionNow = true;
	}
	
	private void OnCreateBossCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		
		for (int n = 0; n < createBossPara; n++)
		{
			
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
			}
			
			if(!CityGlobalData.PT_Or_CQ)
			{
				//EnemyNameid = LegendNpcTemplate.GetEnemyNameId_By_EnemyId(Bosses[n]);
				
				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(Bosses[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;
				
				string leftTopSpriteName = null;
				
				var rightButtomSpriteName = "boss";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{
				
				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(Bosses[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;
				
				var rightButtomSpriteName = "boss";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
				
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createBossPara;
	
	private int allenemy
	{
		get { return Bosses.Count + heros.Count + soldires.Count; }
	}
	
	void createBoss(int i)
	{
		createBossPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateBossCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateBossCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private void OnCreateHeroCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		
		for (int n = 0; n < createHeroPara; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - Bosses.Count - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - Bosses.Count - n) * distance - countDistance, -20, 0);
			}
			
			if(!CityGlobalData.PT_Or_CQ)
			{
				
				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(heros[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{
				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(heros[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
				
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createHeroPara;
	
	void createHeros(int i)
	{
		createHeroPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateHeroCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateHeroCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private void OnCreateSoldierCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		
		for (int n = 0; n < createSoldierPara; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			if (allenemy >= EnemyNumBers)
			{
				iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, -20, 0);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - (Bosses.Count + heros.Count)
				                                                        - n) * distance - countDistance, -20, 0);
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
			
			int EnemyNameid = 0;
			
			if(!CityGlobalData.PT_Or_CQ)
			{
				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(soldires[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{
				
				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(soldires[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();;
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(3, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
				
			}
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}
	
	private int createSoldierPara;
	
	void createSoliders(int i)
	{
		createSoldierPara = i;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateSoldierCallBack);
		}
		else
		{
			WWW temp = null;
			OnCreateSoldierCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	/// <summary>
	/// CreateAward
	/// </summary>
	private int awardNum;//掉落物品个数
	
	private int m_Leveltype;
	
	List <GameObject> m_AwardIcon = new List<GameObject> (); 
	
	List<int> First_t_items = new List<int>();
	
	private int FirstDropthin_Icon;


	public void GetAward(int levelType)
	{
		m_Leveltype = levelType;
		
		foreach(GameObject maward in m_AwardIcon)
		{
			Destroy(maward);
		}
		m_AwardIcon.Clear ();
		
		if (levelType == 0)
		{
			awardNum = 4;
		}
		else if (levelType == 1 || levelType == 2)
		{
			awardNum = 4;
		}
		
		List<int> t_items = new List<int>();
		First_t_items.Clear ();
		if(!CityGlobalData.PT_Or_CQ)
		{
			LegendPveTemplate Legendpvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(Pve_Level_Info.CurLev);
			char[] t_items_delimiter = { ',' };
			
			char[] t_item_id_delimiter = { '=' };
			
			string[] t_item_strings = Legendpvetemp.awardId.Split(t_items_delimiter);
			
			for (int i = 0; i < t_item_strings.Length; i++)
			{
				string t_item = t_item_strings[i];
				
				string[] t_finals = t_item.Split(t_item_id_delimiter);
				
				if(t_finals[0] != "" && !t_items.Contains(int.Parse(t_finals[0])))
				{
					t_items.Add(int.Parse(t_finals[0]));
				}
			}
			if(!mLevel.chuanQiPass)
			{
				if(Legendpvetemp.firstAwardId != null && Legendpvetemp.firstAwardId !="")
				{
					string[] First_t_item_strings = Legendpvetemp.firstAwardId.Split(t_item_id_delimiter);
					
					FirstDropthin_Icon = int.Parse(First_t_item_strings[0]);
					
					First_t_items.Add(FirstDropthin_Icon);
				}
			}
		}
		else
		{
			PveTempTemplate pvetemp = PveTempTemplate.GetPveTemplate_By_id(Pve_Level_Info.CurLev);
			//		Debug.Log ("pvetemp.awardId ：" +pvetemp.awardId);
			
			char[] t_items_delimiter = { ',' };
			
			char[] t_item_id_delimiter = { '=' };
			
			string[] t_item_strings = pvetemp.awardId.Split(t_items_delimiter);
			
			for (int i = 0; i < t_item_strings.Length; i++)
			{
				string t_item = t_item_strings[i];
				
				string[] t_finals = t_item.Split(t_item_id_delimiter);
				if(t_finals[0] != ""&&!t_items.Contains(int.Parse(t_finals[0])))
				{
					t_items.Add(int.Parse(t_finals[0]));
				}
				
			}
			if(!mLevel.s_pass)
			{
				if(pvetemp.firstAwardId != null && pvetemp.firstAwardId !="")
				{
					string[] First_t_item_strings = pvetemp.firstAwardId.Split(t_item_id_delimiter);
					
					FirstDropthin_Icon = int.Parse(First_t_item_strings[0]);
					
					First_t_items.Add(FirstDropthin_Icon);
				}
			}
		}
		int initNum;
		if (awardNum >= t_items.Count)
		{
			initNum = t_items.Count;
		}
		else
		{
			initNum = awardNum;
		}
		
		numPara = initNum;
		itemsPara = t_items;
		
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}
	
	private int numPara;
	private List<int> itemsPara;
	Vector3 FirstAwardPos = new Vector3 (0,0,0);
	
	float x = -150;
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		int pos = 0;
		
		int MaxPuTong_pos = 4;
		
		int  MaxJingYing_pos = 4;
		
		if(First_t_items.Count > 0)
		{
			MaxPuTong_pos = MaxPuTong_pos - First_t_items.Count;
			
			MaxJingYing_pos = MaxJingYing_pos - First_t_items.Count;
		}	
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		switch(mLevel.type)
		{
		case 0:
			if(!mLevel.s_pass)
			{
				if(First_t_items.Count > 0)
				{
					x = -50;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		case 1:
			if(!mLevel.s_pass)
			{
				if(First_t_items.Count > 0)
				{
					x = -50;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		case 2:
			if(!mLevel.chuanQiPass)
			{
				if(First_t_items.Count > 0)
				{
					x = -50;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		default:
			break;
		}
		
		//		Debug.Log("numPara = "+numPara );
		
		for (int n = 0; n < numPara; n++)
		{
			//			Debug.Log("itemsPara[n] = "+itemsPara[n]);
			
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
			
			//			foreach(AwardTemp mmAwardTemp in mAwardTemp)
			//			{
			//				Debug.Log("mmAwardTemp.itemId = "+mmAwardTemp.itemId);
			//			}
			
			for (int i = 0; i < mAwardTemp.Count; i++)
			{
				if(mAwardTemp[i].weight != 0)
				{
					pos += 1;
					
					if (m_Leveltype == 1 || m_Leveltype == 2)
					{
						if(pos > MaxJingYing_pos)
						{
							return;
						}
					}
					else
					{
						if(pos > MaxPuTong_pos)
						{
							return;
						}
					}
					
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
					
					m_AwardIcon.Add(iconSampleObject);
					
					iconSampleObject.SetActive(true);
					
					iconSampleObject.transform.parent = AwardRoot.transform;
					
					iconSampleObject.transform.localPosition = new Vector3(x + (pos-1) * 100, -20, 0);
					
					//FirstAwardPos = iconSampleObject.transform.localPosition;
					
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					
					var iconSpriteName = "";
					
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[i].itemId);
					
					iconSpriteName = mItemTemp.icon.ToString();
					
					iconSampleManager.SetIconType(IconSampleManager.IconType.item);
					
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
					
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);
					
					var popTitle = mNameIdTemplate.Name;
					
					var popDesc = mdesc;
					
					iconSampleManager.SetIconByID(mItemTemp.id, "", 3);
					iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, popTitle, popDesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
				}

			}
		}
		
	}
	public void CreateFirsrAward()
	{			
//		Debug.Log ("Create FirstAward now !");
		
		GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
		
		m_AwardIcon.Add(iconSampleObject);
		
		iconSampleObject.SetActive(true);
		
		iconSampleObject.transform.parent = AwardRoot.transform;
		
		iconSampleObject.transform.localPosition = new Vector3(-150, -20, 0);
		
		var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
		var iconSpriteName = "";
		
		List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(FirstDropthin_Icon);
		
		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[0].itemId);
		
		iconSpriteName = mItemTemp.icon.ToString();
		
		iconSampleManager.SetIconType(IconSampleManager.IconType.item);
		
		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[0].itemId);
		
		string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[0].itemId);
		
		var popTitle = mNameIdTemplate.Name;
		
		var popDesc = mdesc;
		
		iconSampleManager.SetIconByID(mItemTemp.id, "", 3);
		iconSampleManager.SetIconPopText(mAwardTemp[0].itemId, popTitle, popDesc, 1);
		iconSampleManager.FirstWinSpr.gameObject.SetActive(true);
		iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
	}
	/// <summary>
	/// Saos the dang1 button.
	/// </summary>
	public void SaoDangBtn(int Times)
	{
		if(mLevel.type == 1)
		{
			if(!mLevel.s_pass||mLevel.win_Level !=3)
			{
				string data = "完胜通关后，才能扫荡！";
				ClientMain.m_UITextManager.createText( data);
				return;
			}
			else
			{
				if(Times > sdCountTime)
				{
					string data = "扫荡次数不足！";
					ClientMain.m_UITextManager.createText( data);
					return;
				}
			}
		}else
		{
			if(!mLevel.chuanQiPass||mLevel.pingJia !=3)
			{
				string data = "完胜通关后，才能扫荡！";
				ClientMain.m_UITextManager.createText( data);
				return;
			}
		}

		Vipgrade = JunZhuData.Instance ().m_junzhuInfo.vipLv;
		junZhuLevel = JunZhuData.Instance ().m_junzhuInfo.level;
		ExistingPower = JunZhuData.Instance ().m_junzhuInfo.tili;
		if(!FunctionOpenTemp.GetWhetherContainID(3000010))
		{
			string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT);
			string Contain2 = Contain1+ZhuXianTemp.GeTaskTitleById (FunctionOpenTemp.GetTemplateById(3000010).m_iDoneMissionID)+"后开启该功能";
			//string data = "完胜通关后，才能扫荡！";
			ClientMain.m_UITextManager.createText( Contain2);
			return;
		}
		if (junZhuLevel >= FunctionOpenTemp.GetTemplateById(3000010).Level)
		{

			needpower = GuanqiaReq.tili*Times;
			//Debug.Log ("needpower" +needpower);
			if(needpower > ExistingPower)//体力不够
			{
				string data = "体力不足！";
				ClientMain.m_UITextManager.createText( data);
			}
			else
			{
				int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(13);
				
				if(Times>1&&Vipgrade<viplv)
				{
					//ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(500000).m_sNotOpenTips);
					string data = "Vip等级不够！"+viplv.ToString()+"级Vip开启连续扫荡！";
					ClientMain.m_UITextManager.createText( data);
				}else{
					//Debug.Log ("发送扫荡请求。。。");
					
					SendSaoDangInfo (mLevel.guanQiaId,Times);
				}
			}
		}
	}
	void SendSaoDangInfo(int id,int howTimes)
	{
		
		PveSaoDangReq saodanginfo = new PveSaoDangReq ();
		
		MemoryStream saodangstream = new MemoryStream ();
		
		QiXiongSerializer saodangSer = new QiXiongSerializer ();
		
		int i = 1;
		
		if(mLevel.type == 2)
		{
			i = -1;
		}
		saodanginfo.guanQiaId = id*i;
		
		saodanginfo.times = howTimes;
		
		//Debug.Log ("saodanginfo.times = " +saodanginfo.times);
		
		saodangSer.Serialize (saodangstream, saodanginfo);
		
		byte[] t_protof;
		
		t_protof = saodangstream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_PVE_SAO_DANG,ref t_protof);
		
	}
	/// <summary>
	/// The existing power.
	/// </summary>
	int needpower;
	int ExistingPower = 0;
	private int junZhuLevel;//君主等级
	[HideInInspector]public int Vipgrade = 0;
	public void SaoDangBtn1()
	{
		SaoDangBtn (1);
	}
	public void SaoDangBtn10()
	{
		SaoDangBtn (10);
	}
	/// <summary>
	/// Buies the ti li.
	/// </summary>
	public void BuyTiLi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
	}
	/// <summary>
	/// Buy_s the money.
	/// </summary>
	public void Buy_Money()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
	}
	/// <summary>
	/// Buy_s the yuan bao.
	/// </summary>
	public void Buy_YuanBao()
	{
		MainCityUI.ClearObjectList();
		EquipSuoData.TopUpLayerTip();
		//		QXTanBaoData.Instance().CheckFreeTanBao();
	}
//	/// <summary>
//	/// Helps the button.
//	/// 临时联盟代替  有新说明后更换
//	/// </summary>
//	public void HelpBtn()
//	{
//		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCEINSTRCDUTION)); // 
//	}
	public void CloseUI()
	{
		CityGlobalData.PveLevel_UI_is_OPen = false;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
	}
	/// <summary>
	/// Test
	/// </summary>
	public UILabel testLable;
	public GameObject inputbox;
	public bool IsTest = false;
	public GameObject TestBtn;
	public int Test_Section = 1;
	public int Test_Level = 1;
	//攻打按钮
	public void ComfireTest_btn()
	{

		IsTest = true;
		char[] aprcation = {','};
		//Debug.Log ("text =  "+testLable.text);
		string[] s = testLable.text.Split(aprcation);
		
		if(s.Length > 0)
		{
			Test_Section = int.Parse(s[0]);
			
			Test_Level = int.Parse(s[1]);
		}
		//Debug.Log ("Test_Section =  "+Test_Section);
		//Debug.Log ("Test_Level = "+Test_Level);
		inputbox.SetActive(false);
	}
	public void ShowInputBox()
	{
		if(inputbox.activeInHierarchy)
		{
			inputbox.SetActive(false);
		}
		else{
			inputbox.SetActive(true);
		}
	}

}
