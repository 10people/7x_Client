using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewPVEUIManager : MYNGUIPanel ,SocketProcessor {

	public GameObject NoMiBaoSkill;

	public GameObject mChangemibaobtn;

	public UILabel SaodangConditions;

	public GameObject SaodangBtns;

	public static NewPVEUIManager mNewPVEUIManager;

	[HideInInspector]public Level mLevel;

	public  GuanQiaInfo GuanqiaReq;

	public PveSaoDangRet saodinfo;
	public List<MiBaoSkillTips> mMibaoTips = new List<MiBaoSkillTips> ();
	public MiBaoSkillTips mMiBaoSkillTips;
	public NGUILongPress EnergyDetailLongPress1;

	public GameObject LingQuBtn;
	[HideInInspector]public GameObject OpenBxo;
	public GameObject XingJiJianglIUI;
	public SparkleEffectItem mSparkleEffectItem;
	public GameObject OnStrongBtn;
	[HideInInspector]public bool YinDaoOpen = false;
	public static NewPVEUIManager Instance()
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
		int mibaoid = GuanqiaReq.zuheId;
		if(mibaoid<=0)
		{
			return;
		}
		ShowTip.showTip (mibaoid);
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
	public void OPenBox()
	{
		if(OpenBxo == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAO_DANG ),OpenLockLoadBack);
		}

	}
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		OpenBxo = ( GameObject )Instantiate( p_object );
		
		OpenBxo.transform.parent = XingJiJianglIUI.transform;
		
		OpenBxo.transform.localPosition = Vector3.zero;
		
		OpenBxo.transform.localScale  = Vector3.one;

		PveStarAward mPveStarAward = OpenBxo.GetComponent<PveStarAward> ();
		mPveStarAward.mLevel = mLevel;
		mPveStarAward.Init ();
	}

	public void Init()
	{
		InitStarUI ();
		sendLevelDrop ();
		InItEnemyList ();
		GetAward (mLevel.type);

		if( ConfigTool.GetBool(ConfigTool.CONST_QUICK_CHOOSE_LEVEL))
		{
			TestBtn.SetActive(true);
		}
		else
		{
			TestBtn.SetActive(false);
		}
//		Debug.Log ("FunctionOpenTemp.GetWhetherContainID(3000010) = "+FunctionOpenTemp.GetWhetherContainID(3000010));
		if(!FunctionOpenTemp.GetWhetherContainID(3000010))
		{
			SaodangBtns.SetActive(false);
		}
		else
		{
			SaodangBtns.SetActive(true);
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
				
			//	Debug.Log("请求扫荡是数据返回了。。。");
		
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
			
				return true;
			}
			case ProtoIndexes.PVE_STAR_REWARD_GET_RET: 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				PveStarGetSuccess tempInfo = new PveStarGetSuccess();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
					Debug.Log("领奖返回。。。");
				LingQuBtn.SetActive(false);
				if (tempInfo != null)
				{
					if(CityGlobalData.PT_Or_CQ)
					{
						foreach(StarInfo  m_item in mLevel.starInfo )
						{
							if(m_item.starId == tempInfo.s_starNum)
							{
								string[] Awardlist = PveStarTemplate.GetAwardInfo (tempInfo.s_starNum);
								
								PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (tempInfo.s_starNum);
								
								CommonItemTemplate mCom = CommonItemTemplate.getCommonItemTemplateById (int.Parse(Awardlist[1]));
								
								int Awardsnum = int.Parse(Awardlist[2]);
								
								int Award_id = mCom.id;
								
								RewardData data = new RewardData ( Award_id, Awardsnum); 
								
								GeneralRewardManager.Instance().CreateReward (data); 
								
								break;
							}
						}
					}
					else
					{
						foreach(StarInfo  m_item in mLevel.cqStarInfo )
						{
							if(m_item.starId == tempInfo.s_starNum)
							{
								string[] Awardlist = PveStarTemplate.GetAwardInfo (tempInfo.s_starNum);
								
								PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (tempInfo.s_starNum);
								
								CommonItemTemplate mCom = CommonItemTemplate.getCommonItemTemplateById (int.Parse(Awardlist[1]));
								
								int Awardsnum = int.Parse(Awardlist[2]);
								
								int Award_id = mCom.id;
								
								RewardData data = new RewardData ( Award_id, Awardsnum); 
								
								GeneralRewardManager.Instance().CreateReward (data); 
								
								break;
							}
						}
					}

					foreach(BoxBtn m_Box in mBoxBtnList)
					{
						if(m_Box.Star_Id == tempInfo.s_starNum)
						{
							m_Box.IsLingQu = 3;
							m_Box.Init();
						}
					}
					//sendLevelDrop();
					foreach(Pve_Level_Info m_Lv in MapData.mapinstance.Pve_Level_InfoList )
					{
						if(m_Lv.litter_Lv.guanQiaId == tempInfo.guanQiaId)
						{
							for(int j = 0 ; j < m_Lv.litter_Lv.starInfo.Count; j++)
							{
								if(m_Lv.litter_Lv.starInfo[j].starId == tempInfo.s_starNum)
								{
									m_Lv.litter_Lv.starInfo[j].getRewardState = true;
									bool jingying = false ;
									if(CityGlobalData.PT_Or_CQ)
									{
										jingying = true;
									}
									m_Lv.ShowBox(jingying);
									
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
			if(!MiBaoGlobleData.Instance().GetMiBaoskillOpen())
			{
				mSparkleEffectItem.enabled = false ;
				NoMiBaoSkill.SetActive(true);
				mChangemibaobtn.SetActive(false);
			}
			else
			{
				NoMiBaoSkill.SetActive(false);
				mSparkleEffectItem.enabled = true ;
				mChangemibaobtn.SetActive(true);
			}
			MiBaoIcon.gameObject.SetActive(false);
			MiBaoIcon.spriteName = "";
		}
		else
		{
			MiBaoIcon.gameObject.SetActive(true);
			MiBaoSkillTemp mMiBaoSkill  = MiBaoSkillTemp.getMiBaoSkillTempBy_id(GuanqiaReq.zuheId);
			mSparkleEffectItem.enabled = false ;
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
	//public GameObject[] DisAble_Boxs;
	public GameObject[] Active_Boxs;
	public UISprite WinType;

	public GameObject notCrossIcon;
	public UISprite[] starSpriteList;

	public UILabel TiliCost;

	public UILabel ToDayTime;// 挑战次数

	public UILabel TodayAllTime;

	public GameObject BtnRoot;

	public UISprite saodang1sprite;
	public UISprite saodang10sprite;
	public UISprite saodangsprite;

	public UILabel saodang1UILabel;
	public UILabel saodang10UILabel;
	public UILabel saodangUILabel;

	public GameObject Saodang1;

	public GameObject  Saodang10;

	public GameObject Saodang;
	/// </summary>

	public void InitStarUI()
	{
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);

		switch (mLevel.type)
		{
		case 0:
			LingQuBtn.SetActive(false);
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
		ColsePVEGuid ();
		ShowPVEGuid ();
	}
	private void Putong()
	{
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (mLevel.guanQiaId);
		
		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);
		
		LevelWanFa.text = m_item.wanfaType;
		IntroZHanLi.text = m_item.recZhanli.ToString ();
		if(JunZhuData.Instance().m_junzhuInfo.zhanLi < m_item.recZhanli )
		{
			OnStrongBtn.SetActive(true);
			my_Zhanli.text = MyColorData.getColorString(5, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		else
		{
			OnStrongBtn.SetActive(false);
			my_Zhanli.text = MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		
		//		Debug.Log ("M_Name = "+M_Name);
		
		string []s = M_Name.Split(' ');
		if(s.Length > 1)
		{
			LevelName.text = "[b]"+s[1];
		}
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		
		desLabel.text = descStr;
		string TuiJiAnMiBAo = m_item.recMibaoSkill;
		string []mstr = TuiJiAnMiBAo.Split(',');
		for (int i = 0; i < mstr.Length; i++)
		{
//			Debug.Log("mstr [i] = "+mstr[i]);
			GameObject mobg = (GameObject)Instantiate(mMiBaoSkillTips.gameObject);	
			mobg.SetActive(true);
			mobg.transform.parent = mMiBaoSkillTips.gameObject.transform.parent;
			mobg.transform.localPosition = mMiBaoSkillTips.gameObject.transform.localPosition + new Vector3(i * 70 - (mstr.Length - 1) * 35, 0, 0);
			mobg.transform.localScale = Vector3.one;
			if(mstr[i] != ""&&mstr[i] != null)
			{
				mobg.GetComponent<MiBaoSkillTips>().Skillid = int.Parse(mstr[i]);
				mobg.GetComponent<MiBaoSkillTips>().mibao_name = mstr[i];

			}
			mobg.GetComponent<MiBaoSkillTips>().Init();
		}

		notCrossIcon.SetActive (false);
		BtnRoot.SetActive (false);
		LevelTypeLabel.text = "[b]普通关卡";
		int count = 3;
		for(int i = 0; i< count; i++)
		{
			starSpriteList[i].enabled = false;
			if(i == 1)
			{
				Conditions[i].text = "普通关无星级奖励";
			}
			else{
				Conditions[i].text = "";
			}
			//DisAble_Boxs[i].SetActive(false);
			Active_Boxs[i].SetActive(false);
		}
	}
	private void JingYIng()
	{
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (mLevel.guanQiaId);
		
		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);
		
		LevelWanFa.text = m_item.wanfaType;
		IntroZHanLi.text = m_item.recZhanli.ToString ();
		if(JunZhuData.Instance().m_junzhuInfo.zhanLi < m_item.recZhanli )
		{
			OnStrongBtn.SetActive(true);
			my_Zhanli.text = MyColorData.getColorString(5, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		else
		{
			OnStrongBtn.SetActive(false);
			my_Zhanli.text = MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		
		//		Debug.Log ("M_Name = "+M_Name);
		
		string []s = M_Name.Split(' ');
		if(s.Length > 1)
		{
			LevelName.text = "[b]"+s[1];
		}
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		
		desLabel.text = descStr;
		string TuiJiAnMiBAo = m_item.recMibaoSkill;
		string []mstr = TuiJiAnMiBAo.Split(',');
		for (int i = 0; i < mstr.Length; i++)
		{
			GameObject mobg = (GameObject)Instantiate(mMiBaoSkillTips.gameObject);	
			mobg.SetActive(true);
			mobg.transform.parent = mMiBaoSkillTips.gameObject.transform.parent;
			mobg.transform.localPosition = mMiBaoSkillTips.gameObject.transform.localPosition + new Vector3(i * 70 - (mstr.Length - 1) * 35, 0, 0);
			mobg.transform.localScale = Vector3.one;
			if(mstr[i] != ""&&mstr[i] != null)
			{
				mobg.GetComponent<MiBaoSkillTips>().Skillid = int.Parse(mstr[i]);
				mobg.GetComponent<MiBaoSkillTips>().mibao_name = mstr[i];
				
			}
			mobg.GetComponent<MiBaoSkillTips>().Init();
		}

		BtnRoot.SetActive (true);
		Saodang1.SetActive (true);
		Saodang10.SetActive (true);
		Saodang.SetActive (false);
	
		LevelTypeLabel.text = "[b]精英关卡";
		RefreshStar ();
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
	List<BoxBtn> mBoxBtnList = new List<BoxBtn>();
	private void RefreshStar()
	{
		int count = mLevel.starInfo.Count;
		if(count > 3 )
		{
			count = 3;
		}
		mBoxBtnList.Clear ();
		LingQuBtn.SetActive(false);
		int starsnum = 0;
		for(int i = 0; i< count; i++)
		{
			int Star_Id = mLevel.starInfo [i].starId;
			
			PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (Star_Id);
			
			string mDes = DescIdTemplate.GetDescriptionById (mPveStarTemplate.desc);
			
			//Conditions[i].text = mDes;
			BoxBtn mBoxBtn = Active_Boxs[i].GetComponent<BoxBtn>();
			mBoxBtn.m_Lev = mLevel;
			mBoxBtn.Star_Id = mLevel.starInfo [i].starId;
			if(mLevel.starInfo [i].finished)
			{
				starsnum += 1;
				Conditions[i].text = MyColorData.getColorString(4,mDes);
				starSpriteList[i].spriteName = "BigStar";
				
				if(!mLevel.starInfo [i].getRewardState)
				{
					mBoxBtn.IsLingQu = 2;
					LingQuBtn.SetActive(true);
				}
				else
				{
					mBoxBtn.IsLingQu = 3;
				}
			}
			else
			{
				Conditions[i].text = MyColorData.getColorString(8,mDes);
				mBoxBtn.IsLingQu = 1;
			}
			mBoxBtn.Init();
			mBoxBtnList.Add(mBoxBtn);
		}
		if (starsnum >= 3) {
			saodang1sprite.color = new Color(255,255,255,255);
			saodang10sprite.color = new Color(255,255,255,255);
			Saodang1.GetComponent<UIButton>().enabled = true;
			Saodang10.GetComponent<UIButton>().enabled = true;
			SaodangConditions.gameObject.SetActive (false);
		} else {

			Saodang1.GetComponent<UIButton>().enabled = false;
			Saodang10.GetComponent<UIButton>().enabled = false;
			SaodangConditions.gameObject.SetActive (true);
			saodang1sprite.color = new Color(0,0,0,255);
			saodang10sprite.color = new Color(0,0,0,255);
			saodang1UILabel.GetComponent<UILabelType>().m_iType = 100;
			saodang1UILabel.GetComponent<UILabelType>().init();
			saodang10UILabel.GetComponent<UILabelType>().m_iType = 100;
			saodang10UILabel.GetComponent<UILabelType>().init();
		}
	}

	private void Chuanqi()
	{
		BtnRoot.SetActive (true);
		Saodang1.SetActive (false);
		Saodang10.SetActive (false);
		Saodang.SetActive (true);
		ChuanQi_RefreshStar ();
		LevelTypeLabel.text = "[b]传奇关卡";
//		int count = 3;
//		for(int i = 0; i< count; i++)
//		{
//			starSpriteList[i].enabled = false;
//			if(i == 1)
//			{
//				Conditions[i].text = "传奇关无星级奖励";
//			}
//			else{
//				Conditions[i].text = "";
//			}
//			//DisAble_Boxs[i].SetActive(false);
//			Active_Boxs[i].SetActive(false);
//		}
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
		LegendPveTemplate m_item = LegendPveTemplate.GetlegendPveTemplate_By_id (mLevel.guanQiaId);
		
		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);
		
		LevelWanFa.text = m_item.wanfaType;
		IntroZHanLi.text = m_item.recZhanli.ToString ();
		if(JunZhuData.Instance().m_junzhuInfo.zhanLi < m_item.recZhanli )
		{
			OnStrongBtn.SetActive(true);
			my_Zhanli.text = MyColorData.getColorString(5, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		else
		{
			OnStrongBtn.SetActive(false);
			my_Zhanli.text = MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		
		//		Debug.Log ("M_Name = "+M_Name);
		
		string []s = M_Name.Split(' ');
		if(s.Length > 1)
		{
			LevelName.text = "[b]"+s[1];
		}
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		
		desLabel.text = descStr;
		string TuiJiAnMiBAo = m_item.recMibaoSkill;
		string []mstr = TuiJiAnMiBAo.Split(',');
		for (int i = 0; i < mstr.Length; i++)
		{
			GameObject mobg = (GameObject)Instantiate(mMiBaoSkillTips.gameObject);	
			mobg.SetActive(true);
			mobg.transform.parent = mMiBaoSkillTips.gameObject.transform.parent;
			mobg.transform.localPosition = mMiBaoSkillTips.gameObject.transform.localPosition + new Vector3(i * 70 - (mstr.Length - 1) * 35, 0, 0);
			mobg.transform.localScale = Vector3.one;
			if(mstr[i] != ""&&mstr[i] != null)
			{
				mobg.GetComponent<MiBaoSkillTips>().Skillid = int.Parse(mstr[i]);
				mobg.GetComponent<MiBaoSkillTips>().mibao_name = mstr[i];
				
			}
			mobg.GetComponent<MiBaoSkillTips>().Init();
		}
		
		recMibaoSkill.spriteName = m_item.recMibaoSkill;
	}
	/// <summary>
	/// Chuans the qi_ refresh star.
	/// </summary>
	private void ChuanQi_RefreshStar()
	{
		int count = mLevel.cqStarInfo.Count;
	//	Debug.Log("mmLevel.cqStarInfo.Count = "+mLevel.cqStarInfo.Count);
		if(count > 3 )
		{
			count = 3;
		}
		mBoxBtnList.Clear ();
		LingQuBtn.SetActive(false);
		int starsnum = 0;
		for(int i = 0; i< count; i++)
		{
			int Star_Id = mLevel.cqStarInfo [i].starId;
			
			PveStarTemplate mPveStarTemplate = PveStarTemplate.getPveStarTemplateByStarId (Star_Id);
			
			string mDes = DescIdTemplate.GetDescriptionById (mPveStarTemplate.desc);
			
			//Conditions[i].text = mDes;
			BoxBtn mBoxBtn = Active_Boxs[i].GetComponent<BoxBtn>();
			mBoxBtn.m_Lev = mLevel;
			mBoxBtn.Star_Id = mLevel.cqStarInfo [i].starId;
		//	Debug.Log("mLevel.cqStarInfo [i].finished = "+mLevel.cqStarInfo [i].finished);
			if(mLevel.cqStarInfo [i].finished)
			{
				starsnum += 1;
				Conditions[i].text = MyColorData.getColorString(4,mDes);
				starSpriteList[i].spriteName = "BigStar";
				//Debug.Log("mLevel.cqStarInfo [i].getRewardState = "+mLevel.cqStarInfo [i].getRewardState);
				if(!mLevel.cqStarInfo [i].getRewardState)
				{
					mBoxBtn.IsLingQu = 2;
					LingQuBtn.SetActive(true);
				}
				else
				{
					mBoxBtn.IsLingQu = 3;
				}
			}
			else
			{
				Conditions[i].text = MyColorData.getColorString(8,mDes);
				mBoxBtn.IsLingQu = 1;
			}
			mBoxBtn.Init();
			mBoxBtnList.Add(mBoxBtn);
		}
		//Debug.Log("starsnum = "+starsnum);
		if (starsnum >= 3) {
			saodangsprite.color = new Color(255,255,255,255);
			Saodang.GetComponent<UIButton>().enabled = true;
			SaodangConditions.gameObject.SetActive (false);
		} else {
		//	Debug.Log("starsnum = "+starsnum);
			Saodang.GetComponent<UIButton>().enabled = false;
			SaodangConditions.gameObject.SetActive (true);
			saodangsprite.color = new Color(0,0,0,255);

			saodangUILabel.GetComponent<UILabelType>().m_iType = 100;
			saodangUILabel.GetComponent<UILabelType>().init();
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
		string str = "V特权等级不足，V特权等级提升到4级即可重置关卡挑战次数。\n\r参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";//LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_DESC);
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
			YinDaoOpen = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100030)&& TaskData.Instance.m_TaskInfoDic[100030].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-2");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100030];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		
		if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-3");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100055)&& TaskData.Instance.m_TaskInfoDic[100055].progress >= 0)
		{
//			Debug.Log("点击box领奖励");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100055];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100080)&& TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-4");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100080];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100090)&& TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-5");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100090];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		
		if(FreshGuide.Instance().IsActive(100120)&& TaskData.Instance.m_TaskInfoDic[100120].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-6");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100120];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			YinDaoOpen = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100130)&& TaskData.Instance.m_TaskInfoDic[100130].progress >= 0)
		{
			//			Debug.Log("进入PVE 第一个任务 1-7");
			YinDaoOpen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100130];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100320)&& TaskData.Instance.m_TaskInfoDic[100320].progress >= 0)
		{
			//			Debug.Log("攻打传奇关卡");
			YinDaoOpen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100320];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		
			return;
		}
		if(FreshGuide.Instance().IsActive(100260)&& TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
		{
			//	Debug.Log("切换秘技)
			YinDaoOpen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100260];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);

			return;
		}
		if(FreshGuide.Instance().IsActive(100405)&& TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
		{
			//	Debug.Log("扫荡引导)
			YinDaoOpen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100405];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			
			return;
		}
		YinDaoOpen = false;
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
	public void InItEnemyList()
	{

		UICreateEnemy mUIcreate = EnemyRoot.GetComponent<UICreateEnemy>();
		
		mUIcreate.InItEnemyList(mLevel.guanQiaId);
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
				//血量剩余≥60%可完胜
				string data = "完胜[c40000](血量剩余≥60%可完胜)[-]通关后，才能扫荡！";
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
			if(tiaoZhanNum <= 0)
			{
				ReSettingLv();
				
				return;
			}
			if(!mLevel.chuanQiPass||mLevel.pingJia !=3)
			{
				string data = "完胜[c40000](血量剩余≥60%可完胜)[-]通关后，才能扫荡！";
				ClientMain.m_UITextManager.createText( data);
				return;
			}
		}

		Vipgrade = JunZhuData.Instance().m_junzhuInfo.vipLv;
		junZhuLevel = JunZhuData.Instance().m_junzhuInfo.level;
		ExistingPower = JunZhuData.Instance().m_junzhuInfo.tili;
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
					string data = LanguageTemplate.GetText(LanguageTemplate.Text.VIPDesc3);
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
		FunctionWindowsCreateManagerment.m_IsSaoDangNow = true;
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
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_PVE_SAO_DANG,ref t_protof);
		
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
	public void OnStronger()
	{

		 MainCityUILT.ShowMainTipWindow();
		
		Global.m_isOpenPVP = false;

	}
	public void CloseUI()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		MapData.mapinstance.ShowPVEGuid();
		CityGlobalData.PveLevel_UI_is_OPen = false;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		if(EnterGuoGuanmap.Instance().ShouldOpen_id == 1)
		{
			EnterGuoGuanmap.Instance().ShouldOpen_id = 0;

			GameObject map = GameObject.Find("Map(Clone)");
			if(map)
			{
				MainCityUI.TryRemoveFromObjectList (map);
				Destroy(map);
			}
		}
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
