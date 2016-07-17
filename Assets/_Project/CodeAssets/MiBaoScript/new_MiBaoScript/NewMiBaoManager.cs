using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewMiBaoManager : MYNGUIPanel ,SocketListener {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public bool isOpenMiBaoTempInfo = false;

	public UILabel JunZhuZhanLi;

	public mFixUniform mmFixUniform;

	public GameObject First_MiBao_UI; //将魂UI
	
	public GameObject MiBao_TempInfo; //将魂信息
	
	//public GameObject MiBao_ZhanLiInfo; //将魂战力信息界面

	public MibaoInfoResp m_MiBaoInfo;

	public UILabel SkillName;

	public UILabel SkillInstruction;

	public UILabel Num_of_ActiveMiBao;

	public UILabel Remaind;

	public UILabel DisactiveLabel;

	public UISprite SkillIcon;

	public UISprite ActiveMibaoBackGroud;

	public UISprite DisActiveMibaoBackGroud;

	public GameObject UIsilder;

	//public GameObject AllMiBaoActive;

	public List<MibaoInfo> ActiveMiBaoList = new List<MibaoInfo> (); //已经激活将魂

	public List<MibaoInfo> DisActiveMiBaoList = new List<MibaoInfo> ();//尚未激活将魂

	public GameObject new_MiBaoTemp; //将魂战力信息界面

	public GameObject Art; //可以合成技能的红点

	public static NewMiBaoManager mMiBaoData;

	public List<MBTemp> mMBTempList = new List<MBTemp>();

	public UILabel MiBaoSkillNum;

	public UILabel LockofMiBaonum;

	public  GameObject OpenLockBtn ;

	public  GameObject NuqiObg ;

	public UILabel MiBaonunber;
	public UILabel NuqiZhi;

	public  GameObject mLock ;

	public NGUILongPress EnergyDetailLongPress1;

	public GameObject OPenSkillEffect;
	public UISprite OPenSkillICon1;
	public UISprite OPenSkillICon2;
	public UIScrollView mUIscrolview;
	
	public static NewMiBaoManager Instance()
	{
		if (!mMiBaoData)
		{
			mMiBaoData = (NewMiBaoManager)GameObject.FindObjectOfType (typeof(NewMiBaoManager));
		}
		
		return mMiBaoData;
	}

	void Awake()
	{
		SocketTool.RegisterSocketListener(this);
		{
			UIWindowEventTrigger.SetOnTopDelegate( gameObject, ShowMiBaoYInDao );
		}
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;

		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "将魂", 0, 0);
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)
	{
		int MiBaoSkillId = MaxId;
		ShowTip.showTip (MiBaoSkillId);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		mMiBaoData = null;
	}

	void Start () {
	
		isOpenMiBaoTempInfo = false;
		//TaskData.Instance.m_DestroyMiBao = true;
        TaskData.Instance.IsCanShowComplete = false;
        Init ();
		//ShowMiBaoYInDao ();
	}

	void OnEnable()
	{

	}

	void Update () {
	
		JunZhuZhanLi.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString ();
	}
	public void Init()
	{
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			if(Global.m_sPanelWantRun == "Mibaoskill")
			{
				ShowAllSkillS_Btn();
				
				Global.m_sPanelWantRun = "";
			}
		}
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);

		m_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
		mtime = m_MiBaoInfo.remainTime;
		InitData();
		StopCoroutine("showTime");
		StartCoroutine("showTime");
		//SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ,ProtoIndexes.S_MIBAO_INFO_RESP.ToString());
	}
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());

				m_MiBaoInfo = MiBaoInfo;
//				Debug.Log("========1");
				mtime = m_MiBaoInfo.remainTime;

//				Debug.Log("将魂信息返回！");

				InitData();
				StopCoroutine("showTime");
				StartCoroutine("showTime");

				return true;
			}
			case ProtoIndexes.S_BUY_MiBaoPoint: //请求tongbi购买信息 BuyTongbiResp
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				BuyMibaoPointResp mBuyMibaoPointResp = new BuyMibaoPointResp();
				
				t_qx.Deserialize(t_stream, mBuyMibaoPointResp, mBuyMibaoPointResp.GetType());
				if (mBuyMibaoPointResp.result == 0)
				{
					int viplv = JunZhuData.Instance().m_junzhuInfo.vipLv;
					VipTemplate mVip = VipTemplate.GetVipInfoByLevel (viplv);
					
					int MaxPoint = mVip.MiBaoLimit;

					m_MiBaoInfo.levelPoint = MaxPoint;
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().Init();
				}
				return true;
			}
			case ProtoIndexes.MIBAO_DEAL_SKILL_RESP://m将魂技能激活或者进阶返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MiBaoDealSkillResp mMiBaoDealSkillResp = new MiBaoDealSkillResp();
				
				t_qx.Deserialize(t_stream, mMiBaoDealSkillResp, mMiBaoDealSkillResp.GetType());
				
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
		
				if(mMiBaoDealSkillResp.message == 0)
				{
					Debug.Log ("激活技能成功");
//					OPenSkillEffect.SetActive(true);
					if(OPenSkillEffect == null)
					{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_SECRET ),Load_OPenSkillEffect);
					}
					UIYindao.m_UIYindao.CloseUI ();

					Closeffect();
				}
				else{
//					Debug.Log ("碎片不足");
				}
				//UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillIcon.gameObject,EffectIdTemplate.GetPathByeffectId(100166));
				return true;
			}
			default: return false;
			}
			
		}else
		{
//			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	void Load_OPenSkillEffect(ref WWW p_www,string p_path, Object p_object)//将魂等级不足回调函数
	{
		OPenSkillEffect = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(OPenSkillEffect);
		int x   =  MaxId < 1?1:MaxId ;
		
		//100108
		//620215
		MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(x);

		OpenMiBaioskillManger mOpenMiBaioskillManger = OPenSkillEffect .GetComponent<OpenMiBaioskillManger>();

		mOpenMiBaioskillManger.m_SkillName.spriteName = mMiBaoskill.id.ToString();
		switch(x)
		{
		case 1:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC1);
			break;
		case 2:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC2);
			break;
		case 3:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC3);
			break;
		case 4:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC4);
			break;
		case 5:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC5);
			break;
		case 6:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC6);
			break;
		case 7:
			mOpenMiBaioskillManger.mSkillDesc.text = LanguageTemplate.GetText(LanguageTemplate.Text.JINHUNSKILL_DESC7);
			break;
		default:
			break;
			
		}
		// mMiBaoskill.id.ToString();
		mOpenMiBaioskillManger.m_skillIcon1.spriteName = mMiBaoskill.icon;
		mOpenMiBaioskillManger.m_skillIcon2.spriteName = mMiBaoskill.icon;
		mOpenMiBaioskillManger.Init ();
	}
	public void CloseOPenEffect()
	{
		UI3DEffectTool.ClearUIFx (OPenSkillICon1.gameObject);
//		OPenSkillEffect.SetActive(false);
	}
	int mtime;
	IEnumerator showTime()
	{
		int viplv = JunZhuData.Instance().m_junzhuInfo.vipLv;
		if(viplv > 7)
		{
			viplv = 7;
		}
		VipTemplate mVip = VipTemplate.GetVipInfoByLevel (viplv);
//		Debug.Log ("JunZhuData.Instance().m_junzhuInfo.vipLv = "+JunZhuData.Instance().m_junzhuInfo.vipLv);
//		Debug.Log ("mVip = "+mVip.MiBaoLimit);
		int MaxPoint = mVip.MiBaoLimit;

		while (mtime > 0) {
			yield return new WaitForSeconds (1.0f);
			mtime -= 1;
			m_MiBaoInfo.remainTime = mtime;
		}
		if(MaxPoint <= m_MiBaoInfo.levelPoint)
		{
			StopCoroutine("showTime");
			MiBao_TempInfo.GetComponent<MiBaoDesInfo>().PointNum.text = NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint.ToString ();
		}
		else
		{
			m_MiBaoInfo.levelPoint += 1;
			if(mtime <=0)
			{
				mtime = (int )CanshuTemplate.GetValueByKey("ADD_MIBAODIANSHU_INTERVAL_TIME");
			}

			StopCoroutine("showTime");
			
			StartCoroutine("showTime");

			MiBao_TempInfo.GetComponent<MiBaoDesInfo>().PointNum.text = NewMiBaoManager.Instance().m_MiBaoInfo.levelPoint.ToString ();
		}
	}
	int MaxId;
	public void InitUI()
	{
		UISlider mSlider = UIsilder.GetComponent<UISlider>();

		if(m_MiBaoInfo.skillList == null|| m_MiBaoInfo.skillList.Count == 0 )
		{
			MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(1);
			string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);

			SkillName.text = mName;
			MiBaoSkillNum.text = "0/7";
			DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoskill.briefDesc);

			string []s = mDesc.description.Split('#');
			string mRul = "";
			for (int i = 0; i < s.Length; i++)
			{
				mRul += s[i]+"\r\n";	
			}

			SkillInstruction.text = "[f64b00]"+mRul;

			Num_of_ActiveMiBao.text = ActiveMiBaoList.Count.ToString()+"/"+ mMiBaoskill.needNum.ToString();
			if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
			{
				string mstr = "当前无双技可解锁";
				LockofMiBaonum.text = mstr;
				OpenLockBtn.SetActive(true);
				NuqiObg.SetActive(false);
				Closeffect();
				OPeneffect();
			}
			else
			{
				OpenLockBtn.SetActive(false);
				NuqiObg.SetActive(true);

				MiBaonunber.text = ActiveMiBaoList.Count.ToString();

				ChuShiNuQiTemplate mNiqi = ChuShiNuQiTemplate.getChuShiNuQiTemplate_by_Num(ActiveMiBaoList.Count);

				NuqiZhi.text = mNiqi.nuqiRatioc.ToString();
				string mstr1 = (mMiBaoskill.needNum -ActiveMiBaoList.Count).ToString();
				Closeffect();
				string mstr2 = "再激活 ";
				string mstr3 = " 个将魂可解锁该技能";
				PushAndNotificationHelper.SetRedSpotNotification (610, false);
				LockofMiBaonum.text = mstr2+MyColorData.getColorString(5,mstr1)+mstr3;
			}
		
			if(!MiBao_TempInfo.activeInHierarchy)
			{
				mSlider.value = (float)(ActiveMiBaoList.Count) / (float)(mMiBaoskill.needNum);
			}

			SkillIcon.spriteName = mMiBaoskill.icon;

			//AllMiBaoActive.SetActive(false);
			mLock.SetActive(true);
			if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
			{
				Art.SetActive(true);
			}
			else
			{
				Art.SetActive(false);
			}
			MaxId = 1;
		}
		else
		{
			MiBaoSkillNum.text = m_MiBaoInfo.skillList.Count.ToString()+"/7";
			 MaxId = 0;
//			Debug.Log("m_MiBaoInfo.skillList.Count = "+m_MiBaoInfo.skillList.Count);
			if(m_MiBaoInfo.skillList.Count >= 7)
			{
				Art.SetActive(false);
				OpenLockBtn.SetActive(false);
				NuqiObg.SetActive(true);
				MiBaonunber.text = ActiveMiBaoList.Count.ToString();
				
				ChuShiNuQiTemplate mNiqi = ChuShiNuQiTemplate.getChuShiNuQiTemplate_by_Num(ActiveMiBaoList.Count);
				
				NuqiZhi.text = mNiqi.nuqiRatioc.ToString();
				UIsilder.SetActive(false);
				MaxId = 7;
				//AllMiBaoActive.SetActive(true);
				string mstr ="无双技已全部解锁";
				LockofMiBaonum.text = mstr;
				mLock.SetActive(false);

				MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(MaxId);
				
				string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);
				
				SkillName.text = mName;
				
				DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoskill.briefDesc);
				
				SkillIcon.spriteName = mMiBaoskill.icon;
				string []s = mDesc.description.Split('#');
				string mRul = "";
				for (int i = 0; i < s.Length; i++)
				{
					mRul += s[i]+"\r\n";	
				}
				Closeffect();
				SkillInstruction.text = mRul;;
			}
			else
			{
				mLock.SetActive(true);
				UIsilder.SetActive(true);

				//AllMiBaoActive.SetActive(false);

				for(int i = 0; i < m_MiBaoInfo.skillList.Count; i++)
				{
					//Debug.Log ("m_MiBaoInfo.skillList[i].activeZuheId = "+m_MiBaoInfo.skillList[i].activeZuheId);
					if(m_MiBaoInfo.skillList[i].activeZuheId > MaxId)
					{
						MaxId = m_MiBaoInfo.skillList[i].activeZuheId;//找到最大值
					}
				}
				MaxId  += 1  ;
				MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(MaxId);
				
				string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);
				
				SkillName.text = mName;
				
				DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoskill.briefDesc);
				
				SkillIcon.spriteName = mMiBaoskill.icon;
				string []s = mDesc.description.Split('#');
				string mRul = "";
				for (int i = 0; i < s.Length; i++)
				{
					mRul += s[i]+"\r\n";	
				}
				
				SkillInstruction.text = mRul;
				
				Num_of_ActiveMiBao.text = ActiveMiBaoList.Count.ToString()+"/"+ mMiBaoskill.needNum.ToString();
				if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
				{
					OpenLockBtn.SetActive(true);
					NuqiObg.SetActive(false);
					string mstr = "当前无双技可解锁";
					Closeffect();
					OPeneffect();
					LockofMiBaonum.text = mstr;
				}
				else
				{
					OpenLockBtn.SetActive(false);
					NuqiObg.SetActive(true);
					MiBaonunber.text = ActiveMiBaoList.Count.ToString();
					
					ChuShiNuQiTemplate mNiqi = ChuShiNuQiTemplate.getChuShiNuQiTemplate_by_Num(ActiveMiBaoList.Count);
					
					NuqiZhi.text = mNiqi.nuqiRatioc.ToString();
					PushAndNotificationHelper.SetRedSpotNotification (610, false);
					string mstr2 = "再激活 ";
					string mstr3 = " 个将魂可解锁该技能";
					Closeffect();
					string mstr1 = (mMiBaoskill.needNum -ActiveMiBaoList.Count).ToString();
					LockofMiBaonum.text = mstr2+MyColorData.getColorString(5,mstr1)+mstr3;
				}
				if(!MiBao_TempInfo.activeInHierarchy)
				{
					mSlider.value = (float)(ActiveMiBaoList.Count) / (float)(mMiBaoskill.needNum);
				}

				if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
				{
					Art.SetActive(true);
				}
				else
				{
					Art.SetActive(false);
				}
			}

		}

	}

	public void OPeneffect()
	{
//		if(!OPenSkillEffect.activeInHierarchy)
//		{
			int effectid = 620224;
			UI3DEffectTool.ShowBottomLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillIcon.gameObject,EffectIdTemplate.GetPathByeffectId(620224));
//		}
	}

	
	public void Closeffect()
	{
		UI3DEffectTool.ClearUIFx (SkillIcon.gameObject);
	}
	/// <summary>
	/// IShow yindao
	/// </summary>
	/// 
	bool yindaois_open = false;
	void ShowMiBaoYInDao()
	{
		Debug.Log ("ShowMiBaoYInDao First");
//		Debug.Log (TaskData.Instance.m_TaskInfoDic[100330].progress);
		if(MiBao_TempInfo.activeInHierarchy)
		{
			return;
		}
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = false;
			UIYindao.m_UIYindao.CloseUI();
			//Debug.Log ("UIYindao.m_UIYindao.CloseUI();");
		}
		yindaois_open = false;
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
		{
			//Debug.Log("choose one mibao ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			mUIscrolview.enabled = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100330)&& TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
		{
//			Debug.Log("Make one mibao ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100330];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			mUIscrolview.enabled = false;
			yindaois_open = true;
			return;
		}
		if(FreshGuide.Instance().IsActive(100360)&& TaskData.Instance.m_TaskInfoDic[100360].progress >= 0)
		{
			//Debug.Log("将魂升星 ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100360];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			//mUIscrolview.enabled = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100259)&& TaskData.Instance.m_TaskInfoDic[100259].progress >= 0)
		{
			//Debug.Log("将魂技能激活");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100259];

			//Debug.Log ("tempTaskData.m_listYindaoShuju[2] = "+tempTaskData.m_listYindaoShuju[2]);

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			//mUIscrolview.enabled = false;
			return;
		}
	
		mUIscrolview.enabled = true;

	}
	public void InitData()
	{
		if(m_MiBaoInfo == null)
		{
			m_MiBaoInfo = MiBaoGlobleData.Instance().G_MiBaoInfo;
		}
		ActiveMiBaoList.Clear ();
		DisActiveMiBaoList.Clear ();
		bool isHeCheng = false;
		bool isupStar = false;
		bool isLevel = false;
		for(int i = 0 ; i < m_MiBaoInfo.miBaoList.Count; i++)
		{
			if(m_MiBaoInfo.miBaoList[i].level > 0)
			{
				ActiveMiBaoList.Add(m_MiBaoInfo.miBaoList[i]);
				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_MiBaoInfo.miBaoList[i].miBaoId);
				ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,m_MiBaoInfo.miBaoList[i].level);
				MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (m_MiBaoInfo.miBaoList[i].star);
				if(m_MiBaoInfo.miBaoList[i].suiPianNum >= m_MiBaoInfo.miBaoList[i].needSuipianNum && m_MiBaoInfo.miBaoList[i].star < 5 
				   &&mMiBaoStarTemp.needMoney <= JunZhuData.Instance().m_junzhuInfo.jinBi)
				{
					isupStar = true;
				}
//				Debug.Log ("level = "+m_MiBaoInfo.miBaoList[i].level);
//				Debug.Log ("levelPoint = "+ m_MiBaoInfo.levelPoint);
//				Debug.Log ("jinBi = "+JunZhuData.Instance().m_junzhuInfo.jinBi);
//				Debug.Log ("mExpXxmlTemp.needExp = "+mExpXxmlTemp.needExp);
				if(m_MiBaoInfo.miBaoList[i].level <  JunZhuData.Instance().m_junzhuInfo.level && 
				   m_MiBaoInfo.levelPoint > 0&&JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp&&m_MiBaoInfo.miBaoList[i].level < 100)
				{
					isLevel = true;
				}
			}
			else
			{
				DisActiveMiBaoList.Add(m_MiBaoInfo.miBaoList[i]);
				MiBaoSuipianXMltemp mMiBaosuipian1 = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(m_MiBaoInfo.miBaoList[i].tempId);
			    int Mony = mMiBaosuipian1.money;
				if(m_MiBaoInfo.miBaoList[i].suiPianNum >= mMiBaosuipian1.hechengNum  && Mony <= JunZhuData.Instance().m_junzhuInfo.jinBi)
				{
					isHeCheng = true;
				}
			}
		}

//		Debug.Log ("isHeCheng = "+isHeCheng);
//		Debug.Log ("isLevel = "+isLevel);
//		Debug.Log ("isupStar = "+isupStar);

		if(!isHeCheng)
		{
			PushAndNotificationHelper.SetRedSpotNotification (605, false);
		}
		if(!isupStar)
		{
			PushAndNotificationHelper.SetRedSpotNotification (602, false);
		}
		if(!isLevel)
		{
			PushAndNotificationHelper.SetRedSpotNotification (600, false);
		}
		for(int i = 0 ; i < ActiveMiBaoList.Count; i++)
		{
			for(int j = i+1 ; j < ActiveMiBaoList.Count; j++)
			{
				float suipian1 = (float)ActiveMiBaoList[i].suiPianNum / (float)ActiveMiBaoList[i].needSuipianNum;
				float suipian2 = (float)ActiveMiBaoList[j].suiPianNum / (float)ActiveMiBaoList[j].needSuipianNum;

				if(suipian1 < suipian2 )
				{
					MibaoInfo mbTemp = ActiveMiBaoList[i];
					
					ActiveMiBaoList[i] = ActiveMiBaoList[j];
					
					ActiveMiBaoList[j] = mbTemp;

				}
				else if(suipian1 == suipian2 )
				{
					if(ActiveMiBaoList[i].star < ActiveMiBaoList[j].star)
					{
						MibaoInfo mbTemp = ActiveMiBaoList[i];
						
						ActiveMiBaoList[i] = ActiveMiBaoList[j];
						
						ActiveMiBaoList[j] = mbTemp;
					}
					else if(ActiveMiBaoList[i].star == ActiveMiBaoList[j].star)
					{
						if(ActiveMiBaoList[i].level < ActiveMiBaoList[j].level)
						{
							MibaoInfo mbTemp = ActiveMiBaoList[i];
							
							ActiveMiBaoList[i] = ActiveMiBaoList[j];
							
							ActiveMiBaoList[j] = mbTemp;
						}
					}
				}
			}
		}
	
		for(int i = 0 ; i < DisActiveMiBaoList.Count; i++)
		{
			MiBaoSuipianXMltemp mMiBaosuipian1 = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(DisActiveMiBaoList[i].tempId);

			float a = (float)DisActiveMiBaoList[i].suiPianNum/(float)mMiBaosuipian1.hechengNum;

			for(int j = i+1 ; j < DisActiveMiBaoList.Count; j++)
			{
				MiBaoSuipianXMltemp mMiBaosuipian2 = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(DisActiveMiBaoList[j].tempId);
		
				float b = (float)DisActiveMiBaoList[j].suiPianNum/(float)mMiBaosuipian2.hechengNum;
		
				if(a < b && a < 1.0f)
				{
					MibaoInfo mbTemp = DisActiveMiBaoList[i];
					
					DisActiveMiBaoList[i] = DisActiveMiBaoList[j];
					
					DisActiveMiBaoList[j] = mbTemp;
				}
	
			}
		}

		InitUI ();

		InistanceMiBaoItem ();
		SaodangBack ();
	}
	void SaodangBack()
	{
		if(MiBao_TempInfo.activeInHierarchy)
		{
			foreach(MibaoInfo minfo in m_MiBaoInfo.miBaoList )
			{
				if(minfo.miBaoId == MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowmMiBaoinfo.miBaoId)
				{
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowmMiBaoinfo = minfo;
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowSuipian();
					//Debug.Log("RefreshMiBao");
				}
			}
		}
	}
	public float dis_x = 120f;

	public float dis_y = 120f;

	public void InistanceMiBaoItem()
	{
		//Debug.Log ("ActiveMiBaoList.Count = "+ActiveMiBaoList.Count);

		//Debug.Log ("ActiveMiBaoList.Count = "+DisActiveMiBaoList.Count);
		ActiveMibaoBackGroud.gameObject.SetActive (true);

		foreach(MBTemp m in mMBTempList)
		{
			Destroy(m.gameObject);
		}
		mMBTempList.Clear ();
//		Debug.Log("MiBaoGlobleData.Instance().OldMiBaolist.Count = "+MiBaoGlobleData.Instance().OldMiBaolist.Count);
		for(int i = 0 ; i < ActiveMiBaoList.Count; i++)
		{
			GameObject mMiBaotep = Instantiate(new_MiBaoTemp) as GameObject;
			
			mMiBaotep.SetActive(true);

			mMiBaotep.transform.parent = new_MiBaoTemp.transform.parent;

			int m_x = (i%4);

			int m_y = (int)(i/4);

			mMiBaotep.transform.localPosition = new Vector3(-180+m_x*dis_x,-m_y*(dis_y+10)+130+50,0);

			mMiBaotep.transform.localScale = Vector3.one;

			MBTemp mMBTemp = mMiBaotep.GetComponent<MBTemp>();

			mMBTemp.mMiBaoinfo = ActiveMiBaoList[i];
			mMBTempList.Add(mMBTemp);

			if(MiBaoGlobleData.Instance().OldMiBaolist.Count == 0)
			{
				mMBTemp.IsNew = true;
			}
			else
			{
				foreach(MibaoInfo mmibaoodl in MiBaoGlobleData.Instance().OldMiBaolist)
				{
					if(ActiveMiBaoList[i].miBaoId == mmibaoodl.miBaoId)
					{
						mMBTemp.IsNew = false;
						break;
					}
					mMBTemp.IsNew = true;
				}
			}

			mMBTemp.Init();
		}

		if(ActiveMiBaoList.Count >= 21)
		{
			ActiveMibaoBackGroud.transform.localPosition = new Vector3(0,130-(6-1)*70+50,0);
			
			ActiveMibaoBackGroud.SetDimensions(486,140*6);

			DisactiveLabel.gameObject.SetActive(false);

			DisActiveMibaoBackGroud.gameObject.SetActive(false);
		}
		else
		{
			DisactiveLabel.gameObject.SetActive(true);

			DisActiveMibaoBackGroud.gameObject.SetActive(true);

			if(ActiveMiBaoList.Count == 0)
			{
				ActiveMibaoBackGroud.gameObject.SetActive(false);
				DisactiveLabel.gameObject.transform.localPosition = new Vector3(-150,-50+50,0);
			}
			else
			{
				int n = (int)((ActiveMiBaoList.Count-1)/4)+1;

				DisactiveLabel.gameObject.transform.localPosition = new Vector3(-150,-60 -(dis_y+20)*n+50,0);

				ActiveMibaoBackGroud.transform.localPosition = new Vector3(0,130-(n-1)*70+50,0);

				ActiveMibaoBackGroud.SetDimensions(486,140*n);
			}
		}

		float Sprite_y = 0;
		bool HaveCouldMake = false;
	
		for(int i = 0 ; i < DisActiveMiBaoList.Count; i++)
		{
			MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (DisActiveMiBaoList[i].tempId);
			if(DisActiveMiBaoList[i].suiPianNum >= mMiBaoSuipianXMltemp.hechengNum)
			{
				HaveCouldMake = true;
			}
			GameObject mMiBaotep = Instantiate(new_MiBaoTemp) as GameObject;
			
			mMiBaotep.SetActive(true);
			
			mMiBaotep.transform.parent = new_MiBaoTemp.transform.parent;
			
			int m_x = (i%4);
			
			int m_y = (int)(i/4);
			int n = 0;
			if(ActiveMiBaoList.Count > 0)
			{
				n = (int)((ActiveMiBaoList.Count-1)/4)+1;
			}
			mMiBaotep.transform.localPosition = new Vector3(-180+m_x*dis_x,-m_y*(dis_y+10)+130+50 -(n*140+60),0);
			if( i == 0)
			{
				Sprite_y = -m_y*dis_y+130+50 -n*140+60;
			}
			mMiBaotep.transform.localScale = Vector3.one;
			
			MBTemp mMBTemp = mMiBaotep.GetComponent<MBTemp>();
			
			mMBTemp.mMiBaoinfo = DisActiveMiBaoList[i];
			mMBTempList.Add(mMBTemp);
			mMBTemp.Init();
		}
		if(DisActiveMiBaoList.Count > 0)
		{
			int n = (int)((DisActiveMiBaoList.Count-1)/4)+1;

			DisActiveMibaoBackGroud.transform.localPosition = new Vector3(0,Sprite_y-(n-1)*70 - 120,0);
			
			DisActiveMibaoBackGroud.SetDimensions(486,140*n);
		}
		else
		{
			DisactiveLabel.gameObject.SetActive(false);
			DisActiveMibaoBackGroud.gameObject.SetActive(false);
		}
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				if(ActiveMiBaoList.Count > 1)
				{
					if(ActiveMiBaoList[0].miBaoId != 301011)
					{
						MibaoInfo mMiBaoIf = ActiveMiBaoList[0];
						
						ActiveMiBaoList[0] = ActiveMiBaoList[1];
						
						ActiveMiBaoList[1] = mMiBaoIf;
					}
				}
			}

		}
		//Debug.Log ("HaveCouldMake = "+HaveCouldMake);
		if(HaveCouldMake && !yindaois_open)
		{
			int S_y = 130+(ActiveMiBaoList.Count/4)*120;
			//Debug.Log ("S_y = "+S_y);
			mmFixUniform.offset = new Vector3(-9.2f,S_y,0);
			mmFixUniform.enabled = true;
			Invoke("ClosemmFixUniform",1f);
		}
		else
		{
			mmFixUniform.enabled = false;
		}
	}
	void ClosemmFixUniform()
	{
		mmFixUniform.enabled = false;
	}
	public void Buy_Money()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
	}
	public void Buy_YuanBao()
	{
		MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
        //		QXTanBaoData.Instance().CheckFreeTanBao();
    }
	public void GoPropertyShow()
	{
		First_MiBao_UI.SetActive (false);
		//MiBao_ZhanLiInfo.SetActive (true);
	}
	public void BackToFirstPage(GameObject Nextgme) // 返回首页
	{
		First_MiBao_UI.SetActive (true);

		Nextgme.SetActive (false);

		InitUI ();
	}
	public void ShowAllSkillS_Btn()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);

	}
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;

		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);

		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();

		mNewMiBaoSkill.COmeMiBaoUI = true;

		mNewMiBaoSkill.Init ( 0,0 );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}

	public void ShowMiBaoDeilInf( MibaoInfo mMiBaoifon)
	{

		MiBao_TempInfo.SetActive (true);
		MiBaoDesInfo mMiBaodes = MiBao_TempInfo.GetComponent<MiBaoDesInfo>();
		mMiBaodes.ShowmMiBaoinfo = mMiBaoifon;
		First_MiBao_UI.SetActive (false);
		mMiBaodes.InitLevel ();
		mMiBaodes.Init();

	}

	public void HelpBtn()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.MIBAOINSTRCDUTION));
	}
	public void CloseBtn()
	{
		//TaskData.Instance.m_DestroyMiBao = false;
        TaskData.Instance.IsCanShowComplete = true;
        MainCityUI.TryRemoveFromObjectList(this.gameObject);

		Destroy (this.gameObject);
	}
	public void JIeSuo()
	{
		MiBaoDealSkillReq mMiBaoDealSkillReq = new MiBaoDealSkillReq ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
		
		//Debug.Log("MaxId  = "+MaxId);
		
		mMiBaoDealSkillReq.zuheId =  MaxId;

		MiBaoSer.Serialize (miBaoStream,mMiBaoDealSkillReq);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray();
		UIYindao.m_UIYindao.CloseUI ();
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.MIBAO_DEAL_SKILL_REQ,ref t_protof);
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
}
