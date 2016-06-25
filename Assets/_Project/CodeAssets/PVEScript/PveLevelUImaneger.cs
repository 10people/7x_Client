using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PveLevelUImaneger: MonoBehaviour,SocketProcessor {

	public static Dictionary<int,RewardItem> Rewards = new Dictionary<int,RewardItem>();
	[HideInInspector]public bool Create_No_Disdroy;//判断是攻打按钮还是close按钮

	[HideInInspector]public Level Lv_Info;

	public static bool RewardsDataIsBack = false;
	public static bool EnemysDataIsBack = false;

	public GameObject ShowStargb;

	public GameObject Child;
	//public GameObject GetStarJiangli;
	Transform mtrans;
	float mScale;
	Vector3 scale1;
	Vector3 scale2;
	[HideInInspector]public int Vipgrade = 0;
//	float time = 0.5f;

	public static GuanQiaInfo GuanqiaReq;

	public static PveLevelUImaneger mPveLevelUImaneger;

	[HideInInspector]public int m_guidnotes; //记录引导步骤的数据

	public GameObject zheZhao;

	public GameObject TestBtn;

	public UILabel LevelName;

	private bool IsShowEffect = false ;

	public GameObject ChangeMiBaobtn;//更换秘宝的按钮

	public bool IsSaodang = false;

	public UISprite MiBaoSkillIcon;

	public bool ShowYinDao ;
	public bool IsChooseMiBao = false ;
	public UICreateEnemy mUICreateEnemy;
	public UICreateDropthings mUICreateAward;
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
		mPveLevelUImaneger = this;
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
		ISCanSaodang = true;
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
			ShowYinDao = false;
			return;
		}
	}
	public void mTest(ref WWW p_www,string p_path, Object p_object)
	{
//		 GameObject Eff = Instantiate(p_object)as GameObject;
//		Eff.transform.parent = TestBtn.transform.parent;
//		Eff.transform.localScale = Vector3.one;
//		Eff.transform.localPosition = TestBtn.transform.localPosition;
//		m_TestEffect mtestEffect = Eff.GetComponent<m_TestEffect>();
//		mtestEffect.Init (TestBtn.transform.localPosition);

	}

	public void ShowEffect()
	{
		if (IsSaodang)
						return;
		MibaoInfoResp my_MiBaoInfo = MiBaoGlobleData.Instance().G_MiBaoInfo;
		//IsShowEffect = true;
//		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
//		{
//			int Activenum = 0;
//			for(int j = 0 ; j < my_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j++)
//			{
//				if(my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0)
//				{
//					Activenum +=1;
//					if(Activenum >= 2)
//					{
//						IsShowEffect = true;
//						break;
//					}
//				}
//			}
//			
//		}
		//GuanQia.zuheId = 0;
		if(GuanqiaReq.zuheId <= 0 && IsShowEffect)
		{
			int effectId =  100006;
			
			UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,ChangeMiBaobtn,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));		
		}
	}
	public void CloseEffect()
	{
		UI3DEffectTool.ClearUIFx (ChangeMiBaobtn);
	}
	public void init(){

		ShowYinDao = true; 

		//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_SECRET ),mTest);
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (Lv_Info.guanQiaId);
		
		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);

		string []s = M_Name.Split(' ');
		if(s.Length > 1)
		{
			LevelName.text = s[1];
		}
		sendLevelDrop (Lv_Info.guanQiaId);
		ShowPVEGuid ();
		//SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ,ProtoIndexes.S_MIBAO_INFO_RESP.ToString());

		mUICreateEnemy.InItEnemyList (Lv_Info.type);
		mUICreateAward.mLevl = Lv_Info;
		mUICreateAward.GetAward (Lv_Info.type);
	}
	public void loadback1(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_DESC);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null);
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
	public void LoadCQNo_Pass(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = "还未通关";//LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_DESC);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null);
	}
    void TopUpShow(int index)
    {

        EquipSuoData.TopUpLayerTip();
    }

	void ShowResettingLv(int i)
	{
		if(i == 1)
		{
			return;
		}
		if(!Lv_Info.chuanQiPass)
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
					mResetCQTimesReq.guanQiaId = Lv_Info.guanQiaId;
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

   public  void ReSettingLv(){
	
		int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(10);
		if(JunZhuData.Instance().m_junzhuInfo.vipLv < viplv)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),loadback1);
			return;
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockResttingBox);
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

	public void LockResttingtimesload(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.RESETTING_FINSHED);
		string btnStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,btnStr,null,null,null,null);
	}
	public void  sendLevelDrop(int Curr_Id)
	{
		GuanQiaInfoRequest EnemyandDropInfo = new GuanQiaInfoRequest ();

		MemoryStream EnemyStream = new MemoryStream ();

		QiXiongSerializer Enemyer = new QiXiongSerializer ();

		EnemyandDropInfo.type = Lv_Info.type;

//		Debug.Log ("Lv_Info.guanQiaId = "+Lv_Info.guanQiaId);

		EnemyandDropInfo.guanQiaId = Curr_Id;
		
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

//				Debug.Log("level信息返回");

				bool IsChoosedMibao = false;

//				foreach(int mb_dbid in tempInfo.mibaoIds)
//				{
				if(GuanqiaReq.zuheId > 0)
					{
						IsChoosedMibao = true;
					}
//				}

//				Debug.Log("PveGuoguanlevel信IsChoosedMibao = "+IsChoosedMibao);

				if(IsChoosedMibao)
				{
					MapData.mapinstance.IsChoosedMiBao = true;
				}
				else{
					MapData.mapinstance.IsChoosedMiBao = false;
				}

				Rewards.Clear();
//
//				GameObject tempOjbect = GameObject.Find("PveStarsMissionLayer(Clone)");
//
//				if(tempOjbect)
//				{
//				//	tempOjbect.GetComponent<PveStarsMissionManagerment>().DataTidy(tempInfo.guanQiaId,tempInfo.acheive,tempInfo.acheiveRewardState);
//				}
				PveLeveType mLeveLtype = ShowStargb.GetComponent<PveLeveType>();
			
				mLeveLtype.levelInfo = Lv_Info;

				mLeveLtype.GuanQia = tempInfo;

				mLeveLtype.InItCheckPoint();

				ShowMiBaoSkillIcon();
			
				ShowEffect ();
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
				
				pveW_item.Clear();

				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			    sendLevelDrop(SaodangId);
				
				popRewardUI();

				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}

	public void ShowMiBaoSkillIcon()
	{

		//GuanqiaReq.zuheId = 1;

		if(GuanqiaReq.zuheId <= 0)
		{
			MiBaoSkillIcon.spriteName = "";
		}
		else
		{
			MiBaoSkillTemp mMiBaoSkill  = MiBaoSkillTemp.getMiBaoSkillTempBy_id(GuanqiaReq.zuheId);

			MiBaoSkillIcon.spriteName = mMiBaoSkill.icon.ToString();
		}

	}

//扫荡公告

	public Dictionary<string,PveStarAwardItem> pverewardItem = new Dictionary<string,PveStarAwardItem>();
	public List<PveStarAwardItem> pveW_item = new List<PveStarAwardItem>();
	public PveSaoDangRet saodinfo;
	private int junZhuLevel;//君主等级
	int ExistingPower = 0;
	public bool ISCanSaodang = true;
	private int SaodangId;
	public int SaoDangTimes;
	int needpower;
	public void SaodangBtn1()
	{
		SaoDangTimes = 1;
		SaodangBtn ();
	}
	public void SaodangBtn10()
	{
		SaoDangTimes = 10;
		SaodangBtn ();
	}
	public void SaodangBtn3()
	{
		SaoDangTimes = 3;
		SaodangBtn ();
	}
    void SaodangBtn()
	{
		if(!ISCanSaodang)
		{
			return;
		}
		ISCanSaodang = false;
		Vipgrade = JunZhuData.Instance().m_junzhuInfo.vipLv;
		junZhuLevel = JunZhuData.Instance().m_junzhuInfo.level;
		ExistingPower = JunZhuData.Instance().m_junzhuInfo.tili;
		
		
		//Debug.Log ("FunctionOpenTemp.GetWhetherContainID(3000010) = " +FunctionOpenTemp.GetWhetherContainID(3000010));
		
		if(!FunctionOpenTemp.GetWhetherContainID(3000010))
		{
			
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
			ISCanSaodang = true;
			return;
		}
		
		if (junZhuLevel >= FunctionOpenTemp.GetTemplateById(3000010).Level)
		{
			SaodangId = Pve_Level_Info.CurLev;
			//Debug.Log("SaoDangTimes = " +SaoDangTimes);
			needpower = GuanqiaReq.tili*SaoDangTimes;
			//Debug.Log ("needpower" +needpower);
			if(needpower > ExistingPower)//体力不够
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTiLiLoadBack);
				ISCanSaodang = true;
				//Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_NO_TI_LI ),LoadResourceCallback1);
			}
			else
			{
				int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(13);
				
				if(SaoDangTimes>1&&Vipgrade<viplv)
				{
					Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_CANT_SAO_DANG_REMAIN ),LoadResourceCallback1);
					ISCanSaodang = true;
				}else{
					//Debug.Log ("发送扫荡请求。。。");
					
					SendSaoDangInfo (SaodangId,SaoDangTimes);
				}
			}
		}
		
		else
		{
			//zheZhao.SetActive (true);
			
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LevelTips);
			ISCanSaodang = true;
		}
	}
	public void LevelTips(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string Tilte = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.JUNZHU_LV)+FunctionOpenTemp.GetTemplateById(3000010).Level.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.SAODANGGUANQIA);
		
		string Btn = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(Tilte,null,MyColorData.getColorString (1,str),null,Btn,null,FalseZheZhao);
	}
	
	void FalseZheZhao (int i)
	{
	    // zheZhao.SetActive (false);
	}
	void SendSaoDangInfo(int id,int howTimes)
	{
		
		PveSaoDangReq saodanginfo = new PveSaoDangReq ();
		
		MemoryStream saodangstream = new MemoryStream ();
		
		QiXiongSerializer saodangSer = new QiXiongSerializer ();
		
		int i = 1;
		
		if(Lv_Info.type == 2)
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
	public void LoadResourceCallback1(ref WWW p_www,string p_path, Object p_object)
	{
		//Debug.Log ("needpower33333" +needpower);
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
		GameObject obj = GameObject.Find ("Mapss");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localPosition = Vector3.zero;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
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
	void LoadRenWuBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		
		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT);
		
		string Contain2 = Contain1+"\r\n"+"\r\n"+ZhuXianTemp.GeTaskTitleById (FunctionOpenTemp.GetTemplateById(3000010).m_iDoneMissionID)+"\r\n"+"\r\n"+"后开启该功能";
		
		//string Contain3 = LanguageTemplate.GetText(LanguageTemplate.Text.FINGHT_CONDITON);
		
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(title,Contain2, null,null,Comfirm,null,null,null,null);
	}
	void popRewardUI()
	{
		CloseEffect ();
		ColsePVEGuid ();
		IsSaodang = true;
		
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),LoadResourceCallback);
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
//		GameObject obj = GameObject.Find ("Mapss");
//		
//		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localPosition = new Vector3 (0,10000,0);
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		SaoDangManeger mSaoDangManeger = tempOjbect.GetComponent<SaoDangManeger>();
		
		mSaoDangManeger.m_PveSaoDangRet = saodinfo;
		
		mSaoDangManeger.SaodangType = 1;
		
		mSaoDangManeger.Init ();
	}
	void getTili(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true,false,false);
		}
	}

	public void SHow_OrClose()
	{
		//Debug.Log ("Close = "+Child.activeInHierarchy);
		if(Child.activeInHierarchy)
		{
			Child.SetActive(false);
		}
		else
		{
			Child.SetActive(true);
		}
	}
}
