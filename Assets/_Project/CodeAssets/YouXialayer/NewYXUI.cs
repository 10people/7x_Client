using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewYXUI : MYNGUIPanel,SocketProcessor {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public UILabel m_ShowTime;

	public GameObject EffectRoot;

	public GameObject NoMiBaoSkillMind;

	public GameObject NoMiBaoSkillLock;

	public static NewYXUI mNewYXUI;
	public int l_id;
	
	public int big_id;
	public static NewYXUI Instance()
	{
		if (!mNewYXUI)
		{
			mNewYXUI = (NewYXUI)GameObject.FindObjectOfType (typeof(NewYXUI));
		}
		
		return mNewYXUI;
	}

	public YouXiaGuanQiaInfoResp m_YouXiaGuanQiaInfoResp;

	public PveSaoDangRet saodinfo;

	public Transform EnemyRoot;
	
	public Transform AwardRoot;
	
	[HideInInspector]
	public GameObject IconSamplePrefab;
	
	List<int> GetPveEnemyId = new List<int>();
	
	List<int> soldires = new List<int>();
	List<int> heros = new List<int>();
	List<int> Bosses = new List<int>();
	List<int> Zhi_Ye = new List<int>();

	int EnemyNumBers = 0;//显示敌人数量
	
	private int awardNum = 4;//掉落物品个数
	
	int distance = 100;//敌人头像距离
	
	int countDistance = 250;//偏移量
	
	List <GameObject> mHerosIcon = new List<GameObject> (); 
	
	List <GameObject> m_AwardIcon = new List<GameObject> (); 

	private int NpcId;

//	public UILabel YouxiaName;

	public UILabel Best_Num;
	
	//public UILabel All_Num;
	
	public UILabel Instruction;
	
	public UILabel Difficult;

	public UILabel WanFa;

	public YouXiaInfo m_You_XiaInfo;

	public UILabel SaoDangTimes;
	public UILabel TuiJianZHanli;
	public UILabel My_ZHanli;
	public UILabel[] Conditions;

	public UISprite MiBaoIcon;

	public UISprite TuiJianMiBaoIcon;

	public GameObject ChengJiRoot;
	//public GameObject SaodangAndEnterBattleBtn;
	public MiBaoSkillTips mMiBaoSkillTips;
	public NGUILongPress EnergyDetailLongPress1;

	public GameObject m_CantSaoDang;

	//public UILabel mPassLevelMind;

	public SparkleEffectItem mSparkleEffectItem;

	public GameObject OnStrongBtn;

	void Awake()
	{
		EnergyDetailLongPress1.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress1.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress1.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress1.OnLongPress = OnEnergyDetailClick1;
		SocketTool.RegisterMessageProcessor(this);
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	public void OnEnergyDetailClick1(GameObject go)//显示体力恢复提示
	{
		int mibaoid = m_You_XiaInfo.zuheId;
		if(mibaoid<=0)
		{
			return;
		}
		ShowTip.showTip (mibaoid);
	}
	void OnDestroy()
	{
		mNewYXUI = null;
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	public void Init()
	{
		CreateEnemy ();
		
		CreateAward ();

		InitUIData ();
	}

	YouXiaTimesInfoResp M_tempInfo;
	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YOUXIA_SAO_DANG_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				PveSaoDangRet tempInfo = new PveSaoDangRet();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				saodinfo = tempInfo;
				Debug.Log("扫荡游侠saodinfo.result = "+saodinfo.result);
				switch(saodinfo.result)
				{
				case 0:
					if(m_You_XiaInfo.remainTimes >0)
					{
						m_You_XiaInfo.remainTimes -= 1;
					}
					getSaoDangData(tempInfo);
					break;
				case 1:
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ExitLoadCallback );
					break;
				default:
					break;
				}
				InitUIData();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);
				
				//	ChooseYouXiaUIManager.mChooseYouXiaUIManager.mYouXia_Info.remainTimes -= 1;
				//	ChooseYouXiaUIManager.mChooseYouXiaUIManager.Init();
				return true;
			}
			case ProtoIndexes.S_YOUXIA_GUANQIA_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaGuanQiaInfoResp tempInfo = new YouXiaGuanQiaInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				canBtn = true;
				m_YouXiaGuanQiaInfoResp = tempInfo;
				Debug.Log ("tempInfo:" + tempInfo.saoDang);
				showChengJi(tempInfo);
				return true;
			}
			case ProtoIndexes.S_YOUXIA_TIMES_INFO_RESP:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaTimesInfoResp tempInfo = new YouXiaTimesInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				M_tempInfo = tempInfo;
				
				BuyTimesInfoBack(tempInfo);
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);
				//Debug.Log("购买游侠信息次数返回");
				
				return true;
			}
			case ProtoIndexes.S_YOUXIA_TIMES_BUY_RESP:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaTimesBuyResp tempInfo = new YouXiaTimesBuyResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
			//	Debug.Log("确认购买游侠信息返回");
				
				if(tempInfo.result == 0)
				{
					ComfireBuyTimesInfoBack(tempInfo);
				}
				else
				{

					Debug.LogError("购买失败了"+tempInfo.result);
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ExitLoadCallback1 );
				}
				
				return true;
			}
			case ProtoIndexes.S_YOUXIA_CLEAR_COOLTIME_RESP:// 游侠清除冷却时间返回
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ClearCooltimeResp tempInfo = new ClearCooltimeResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				Debug.Log("游侠清除冷却时间返回");
				if(tempInfo.result == 0)
				{
					ClientMain.m_UITextManager.createText("清除成功！");
					SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);
					InitUIData();
				}
				else if(tempInfo.result == 1)
				{
					Global.CreateFunctionIcon (1901);
				}
				else if(tempInfo.result == 2)
				{
					Global.CreateFunctionIcon (101);
				}
				else  if(tempInfo.result == 3)
				{
					ClientMain.m_UITextManager.createText("不再冷却中，不需要清除！");
				}
				
				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}
	//购买失败提示框异步加载回调
	public void ExitLoadCallback1( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str2 = "";
		
		string str1 = "\r\n"+"购买失败！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, str1+str2, null,null,confirmStr,null,null,null,null);
	}
	//扫荡次数用完了提示框异步加载回调
	public void ExitLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str2 = "";
		
		string str1 = "\r\n"+"扫荡次数已经用完了！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, str1+str2, null,null,confirmStr,null,null,null,null);
	}
	private void ComfireBuyTimesInfoBack(YouXiaTimesBuyResp m_tempInfo)
	{
		int AllTimes = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (big_id).maxTimes;
		SaoDangTimes.text = m_tempInfo.ramainTimes.ToString ()+"/"+AllTimes.ToString();
		m_You_XiaInfo.remainTimes = m_tempInfo.ramainTimes;
		foreach(YXItem mXY in XYItemManager.initance().YXItemList)
		{
			if(mXY.mYouXiaInfo.id == big_id)
			{
				mXY.mYouXiaInfo.remainTimes = m_tempInfo.ramainTimes;
				
				mXY.Init();
			}
		}	
		InitUIData ();
	}
	private void BuyTimesInfoBack(YouXiaTimesInfoResp m_tempInfo)
	{
		Debug.Log ("m_tempInfo.remainBuyTimes = "+m_tempInfo.remainBuyTimes);
		Debug.Log ("m_tempInfo.type = "+m_tempInfo.type);
		if(m_tempInfo.remainBuyTimes > 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyTimesInfoBack);
		}
		else
		{
			Global.CreateFunctionIcon (1901);
			//Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), HaveNoTimesReMainBack);
		}
	}
	void LoadBuyTimesInfoBack(ref WWW p_www, string p_path, Object p_object)
	{
		string str1 =  LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str2 = "\n"+"您是否要用"+M_tempInfo.cost.ToString()+"元宝购买"+M_tempInfo.getTimes.ToString()+"次挑战？";
		string str3 = "您还可以购买"+M_tempInfo.remainBuyTimes.ToString()+"次。";
		string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		uibox.setBox(str1,MyColorData.getColorString (1,str2 + "\n" + str3) , null, null, strbtn1, strbtn2, BuYComfire, null, null, null);
	}
	void BuYComfire(int i)
	{
		if (i == 2)
		{
			// Debug.Log("发送购买金币的请求");
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < M_tempInfo.cost)
			{
				//Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
				EquipSuoData.TopUpLayerTip();
				return;
			}
			YouXiaTimesBuyReq mYouXiaTimesInfoReq = new YouXiaTimesBuyReq ();
			
			MemoryStream YouXiaTimesInfoReqStream = new MemoryStream ();
			
			QiXiongSerializer YouXiaTimesInfoReqer = new QiXiongSerializer ();
			
			mYouXiaTimesInfoReq.type = big_id;
			
			YouXiaTimesInfoReqer.Serialize (YouXiaTimesInfoReqStream,mYouXiaTimesInfoReq);
			
			byte[] t_protof;
			
			t_protof = YouXiaTimesInfoReqStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(
				ProtoIndexes.C_YOUXIA_TIMES_BUY_REQ, 
				ref t_protof,
				true,
				ProtoIndexes.S_YOUXIA_TIMES_BUY_RESP );
		}
	}
	void HaveNoTimesReMainBack(ref WWW p_www, string p_path, Object p_object)
	{
	//	Debug.Log ("无购买次数");
//		string title = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
//		
//		int vip = 3;
//		if (vip <= JunZhuData.Instance().m_junzhuInfo.vipLv) {
//			
//			vip = 7;
//		} 
//		string str2 = "";
//		if(JunZhuData.Instance().m_junzhuInfo.vipLv >= 7)
//		{
//			 str2 = "\r\n" + "今日购买次数已经用完了。";
//		}
//		else
//		{
//		     str2 = "V特权等级不足，V特权等级提升到"+(vip).ToString()+"级即可购买挑战次数。参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";
//		}
//
//		
//		//string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
//		
//		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
//		GameObject m = GameObject.Find ("1YouXiaBuyTime");
//		if(m == null)
//		{
//			UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//			uibox.gameObject.name = "1YouXiaBuyTime";
//			uibox.setBox(title,str2, null, null, strbtn2,  null, null, null, null);
//		}
	}
	public void SaodangBtn()  // w未定协议7 23
	{
//		Debug.Log ("m_YouXiaGuanQiaInfoResp.saoDang:" + m_YouXiaGuanQiaInfoResp.saoDang);
		if (!m_YouXiaGuanQiaInfoResp.saoDang) {
			string data = "通关一次才能扫荡！";
			ClientMain.m_UITextManager.createText( data);
			return;
		}
		if(m_You_XiaInfo.remainTimes <= 0)
		{
			string mstr = LanguageTemplate.GetText(LanguageTemplate.Text.NOXYTIME);
			ClientMain.m_UITextManager.createText(mstr);
			return;
		}
		if(ColdTimeIsNotNull())
		{
			string mstr = "冷却中，请稍后再来！";
			ClientMain.m_UITextManager.createText(mstr);
//			if(VipFuncOpenTemplate.GetNeedLevelByKey(28) <= JunZhuData.Instance().m_junzhuInfo.vipLv )
//			{
//				SetClearClodTime();
//			}
//			else
//			{
//				Global.CreateFunctionIcon(1901);
//			}
			return;
		}
		int num = 0;
		foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
		{
			if(m_YXItem.mYouXiaInfo.remainColdTime <= 0 && m_YXItem.mYouXiaInfo.remainTimes >= 0)
			{
				num += 1;
			}
		}
		if(num < 2)
		{
		//	Debug.Log("关闭游侠红点");
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
		Debug.Log("K开始扫荡！");
		YouXiaSaoDangReq saodanginfo = new YouXiaSaoDangReq ();
		
		MemoryStream saodangstream = new MemoryStream ();
		
		QiXiongSerializer saodangSer = new QiXiongSerializer ();
		
		int i = l_id;
		
		saodanginfo.guanQiaId = l_id;
		
		saodanginfo.times = 1;
		
		saodangSer.Serialize (saodangstream, saodanginfo);
		
		byte[] t_protof;
		
		t_protof = saodangstream.ToArray();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_YOUXIA_SAO_DANG_REQ,ref t_protof);
		
	}
	void InitUIData()
	{
		Sendmessege ();
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
		
		string difficult = NameIdTemplate.GetName_By_NameId (myouxia.smaName);

		Difficult.text = difficult;

		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);

		WanFa.text = mYouxiaPveTemplate.wanfaType;

//		YouxiaName.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);
		MainCityUI.setGlobalTitle(TopLeftManualAnchor,NameIdTemplate.GetName_By_NameId (myouxia.bigName), 0, 0);
		string mDesc = DescIdTemplate.GetDescriptionById (mYouxiaPveTemplate.smaDesc);
		
		Instruction.text = mDesc;

		TuiJianZHanli.text = mYouxiaPveTemplate.recZhanli.ToString();
		int desc = mYouxiaPveTemplate.vicConDescID;
		string mstr = LanguageTemplate.GetText (desc);

		string []s = mstr.Split('#');

		for(int i = 0; i < s.Length; i++)
		{
		//	Debug.Log("s[i] = "+s[i]);
		}
		if (s.Length <= 1) {
			for (int i = 0; i < Conditions.Length; i++) {
				Conditions [i].text = "";
			}
			Conditions [1].text = s [0];
		}
		else
		{
			for(int i = 0; i < Conditions.Length; i++)
			{
				Conditions[i].text = s[i];
			}
		}

		if(JunZhuData.Instance().m_junzhuInfo.zhanLi < mYouxiaPveTemplate.recZhanli )
		{
			OnStrongBtn.SetActive(true);
			My_ZHanli.text = MyColorData.getColorString(5, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		else
		{
			OnStrongBtn.SetActive(false);
			My_ZHanli.text = MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		string TuiJiAnMiBAo = mYouxiaPveTemplate.recMibaoSkill;
		string []m_str = TuiJiAnMiBAo.Split(',');
		for (int i = 0; i < m_str.Length; i++)
		{
			GameObject mobg = (GameObject)Instantiate(mMiBaoSkillTips.gameObject);	
			mobg.SetActive(true);
			mobg.transform.parent = mMiBaoSkillTips.gameObject.transform.parent;
			mobg.transform.localPosition = mMiBaoSkillTips.gameObject.transform.localPosition + new Vector3(i * 70 - (m_str.Length - 1) * 35, 0, 0);
			mobg.transform.localScale = Vector3.one;
			if(m_str[i] != ""&&m_str[i] != null)
			{
				mobg.GetComponent<MiBaoSkillTips>().Skillid = int.Parse(m_str[i]);
				mobg.GetComponent<MiBaoSkillTips>().mibao_name = m_str[i];
			}

			mobg.GetComponent<MiBaoSkillTips>().Init();
		}
		//TuiJianMiBaoIcon.spriteName = mYouxiaPveTemplate.recMibaoSkill;

		int AllTimes = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (big_id).maxTimes;

		SaoDangTimes.text = m_You_XiaInfo.remainTimes.ToString ()+"/"+AllTimes.ToString();

//		if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
//		{
//			//Debug.Log("进入试练二阶界面333");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
//			//mScorview.enabled = false;
//		}
		
		ShowMiBaoSkillIcon ();
	}
	public void ShowMiBaoSkillIcon()
	{
		if(m_You_XiaInfo.zuheId < 1)
		{

			mSparkleEffectItem.enabled = MiBaoGlobleData.Instance().GetMiBaoskillOpen() ;
			MiBaoIcon.spriteName = "";
			MiBaoIcon.gameObject.SetActive(false);
			NoMiBaoSkillMind.SetActive(MiBaoGlobleData.Instance().GetMiBaoskillOpen());
			if(MiBaoGlobleData.Instance().GetMiBaoskillOpen())
			{
				Closeffect();
				OPeneffect();
			}
		}
		else
		{
			NoMiBaoSkillMind.SetActive(false);
			MiBaoIcon.gameObject.SetActive(true);
			MiBaoSkillTemp mMiBAo = MiBaoSkillTemp.getMiBaoSkillTempBy_id(m_You_XiaInfo.zuheId);
			mSparkleEffectItem.enabled = false ;
			MiBaoIcon.spriteName = mMiBAo.icon.ToString();
			Closeffect();
		}
		NoMiBaoSkillLock.SetActive ( !MiBaoGlobleData.Instance().GetMiBaoskillOpen() );
	}
	public void OPeneffect()
	{
		int effectid = 620233;
		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,EffectRoot,EffectIdTemplate.GetPathByeffectId(effectid));
	}
	public void Closeffect()
	{
		UI3DEffectTool.ClearUIFx (EffectRoot);
	}
	private void Sendmessege()
	{
		YouXiaGuanQiaInfoReq mYouXiaGuanQiaInfoReq = new YouXiaGuanQiaInfoReq ();
		
		MemoryStream YouXiaGuanQiaInfoReqtream = new MemoryStream ();
		
		QiXiongSerializer YouXiaGuanQiaInfoReqSer = new QiXiongSerializer ();
		
		int i = l_id;
		
		mYouXiaGuanQiaInfoReq.guanQiaId = i;
		
		YouXiaGuanQiaInfoReqSer.Serialize (YouXiaGuanQiaInfoReqtream, mYouXiaGuanQiaInfoReq);
		
		byte[] t_protof;
		
		t_protof = YouXiaGuanQiaInfoReqtream.ToArray();
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_YOUXIA_GUANQIA_REQ,ref t_protof);
	}
	public GameObject obj_SaodangBtn;
	
	public GameObject obj_EnterBattleBtn1;
	public GameObject obj_EnterBattleBtn2;
	void showChengJi(YouXiaGuanQiaInfoResp mtempInfo)
	{
		ChengJiRoot.SetActive(false);
		if(big_id ==1 )
		{
			Best_Num.text = mtempInfo.bestScore.ToString ();
			
			YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
			
			//All_Num.text = myouxia.maxNum.ToString ();
			Best_Num.text = mtempInfo.bestScore.ToString ()+"/"+myouxia.maxNum.ToString ();
			ChengJiRoot.SetActive(true);
		}
		Debug.Log("mtempInfo.saoDang = "+mtempInfo.saoDang);
		if(!mtempInfo.saoDang)
		{
			obj_SaodangBtn.SetActive(false);
			m_CantSaoDang.SetActive(true);
			Debug.Log("mtempInfo.time = "+mtempInfo.time);
			if(mtempInfo.time > 0)
			{
				m_ShowTime.gameObject.SetActive(true);
				obj_EnterBattleBtn1.SetActive(false);
				obj_EnterBattleBtn2.SetActive(true);
				StopCoroutine("StartCountTime");
				StartCoroutine("StartCountTime");
			}
			else
			{
				obj_EnterBattleBtn1.SetActive(true);
				obj_EnterBattleBtn2.SetActive(false);
				m_ShowTime.gameObject.SetActive(false);
			}
		}
		else{
			if(m_You_XiaInfo.remainTimes <= 0)
			{
				obj_EnterBattleBtn1.SetActive(false);
				obj_EnterBattleBtn2.SetActive(true);

				obj_SaodangBtn.SetActive(false);
				m_CantSaoDang.SetActive(true);

				m_ShowTime.gameObject.SetActive(false);
			}
			else
			{
				obj_EnterBattleBtn1.SetActive(true);
				obj_EnterBattleBtn2.SetActive(false);
				if(mtempInfo.time > 0)
				{
				
					m_ShowTime.gameObject.SetActive(true);


					obj_SaodangBtn.SetActive(false);
					m_CantSaoDang.SetActive(true);

					obj_EnterBattleBtn1.SetActive(false);
					obj_EnterBattleBtn2.SetActive(true);

					StopCoroutine("StartCountTime");
					StartCoroutine("StartCountTime");
				}
				else
				{
					m_ShowTime.gameObject.SetActive(false);
	
					obj_SaodangBtn.SetActive(true);
					m_CantSaoDang.SetActive(false);
					
					obj_EnterBattleBtn1.SetActive(true);
					obj_EnterBattleBtn2.SetActive(false);

				}

			}

		}

//		Debug.Log ("saoDang = "+mtempInfo.saoDang);
//		Debug.Log ("remainTimes = "+m_You_XiaInfo.remainTimes);
//		Debug.Log ("time = "+mtempInfo.time);

		
	}
	IEnumerator StartCountTime()
	{
		int T = m_YouXiaGuanQiaInfoResp.time;
		while(T > 0)
		{
			T -= 1;
			
			int M = (int)(T/60);
			
			int S = (int)(T % 60);
			string m_s = "";
			if(S < 10)
			{
				m_s = "0"+S.ToString();
			}
			else
			{
				m_s = S.ToString();
			}
			m_ShowTime.text = "战斗冷却："+M.ToString()+":"+m_s;
			yield return new WaitForSeconds(1f);
		}
		//this.gameObject.GetComponent<BoxCollider>().enabled = true;
		
		obj_EnterBattleBtn1.SetActive(true);
		obj_EnterBattleBtn2.SetActive(false);
		obj_SaodangBtn.SetActive(true);
		m_CantSaoDang.SetActive(false);
		m_ShowTime.gameObject.SetActive(false);
	}
	void getSaoDangData(PveSaoDangRet mtempInfo)
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),LoadResourceCallback);
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object) as GameObject;
		
//		GameObject obj = GameObject.Find ("NewYouXiaEnemy(Clone)");
//		
//		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localPosition = new Vector3 (0,10000,0);
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		SaoDangManeger mSaoDangManeger = tempOjbect.GetComponent<SaoDangManeger>();
		
		mSaoDangManeger.m_PveSaoDangRet = saodinfo;
		
		mSaoDangManeger.SaodangType = 2;
		
		mSaoDangManeger.Init ();
	}
	bool canBtn = true;
	public void EnterBattleBtn()
	{
		if(canBtn)
		{
			canBtn = EnterBattleBtnComform ();
		}
		canBtn = true;
	}
	public void ClearCDTime()
	{
//		if(VipFuncOpenTemplate.GetNeedLevelByKey(28) <= JunZhuData.Instance().m_junzhuInfo.vipLv )
//		{
//			SetClearClodTime();
//		}
//		else
//		{
//			Global.CreateFunctionIcon(1901);
//		}
		SetClearClodTime();
	}
	public bool EnterBattleBtnComform()
	{
		if(m_You_XiaInfo.remainTimes <= 0)
		{
			//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),NoTimes);
			string mstr = LanguageTemplate.GetText(LanguageTemplate.Text.NOXYTIME);
			ClientMain.m_UITextManager.createText(mstr);
			return false;
		}
		//Debug.Log ("big_id = "+big_id +"l_id%10 = " +l_id%10);
		if(ColdTimeIsNotNull())
		{
			string mstr = "冷却中，请稍后再来！";
			ClientMain.m_UITextManager.createText(mstr);
			return false;
		}
		int num = 0;
		foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
		{
			if(m_YXItem.mYouXiaInfo.remainColdTime <= 0 && m_YXItem.mYouXiaInfo.remainTimes >= 0)
			{
				num += 1;
			}
		}
		if(num < 2)
		{
			//Debug.Log("关闭游侠红点");
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
		YouxiaPveTemplate template = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
		EnterBattleField.EnterBattleYouXia (big_id, template.smaId);
		return true;
	}
	private bool ColdTimeIsNotNull()
	{
		int mtime = 0;
		foreach(YXItem mYxinfo in XYItemManager.initance().YXItemList)
		{
			if(mYxinfo.mYouXiaInfo.id == m_You_XiaInfo.id)
			{
				mtime = mYxinfo.GetColdTime();
			}
		}
		return mtime > 0 ? true:false;
	}
	private void SetClearClodTime()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadCostYuanBaoUI);
	}
	void LoadCostYuanBaoUI(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str = "";
		string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		titleStr = "提示";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		//	Debug.Log ("次数不足");
		str ="是否使用"+CanshuTemplate.GetStrValueByKey("SHILIAN_CLEARCD_COST")+"元宝清除冷却？";
		int v = VipFuncOpenTemplate.GetNeedLevelByKey (28);
		
		uibox.setBox(titleStr,str, null,null,cancel,confirm,ClearTime,null ,null ,null ,false,true,true,false,100,0,v);
		
	}
	void ClearTime(int i)
	{
		if(i == 2)
		{
			ClearCooltime mClearCooltime = new ClearCooltime ();
			
			MemoryStream YouXiaTimesInfoReqStream = new MemoryStream ();
			
			QiXiongSerializer YouXiaTimesInfoReqer = new QiXiongSerializer ();
			
			mClearCooltime.type = big_id;
			
			YouXiaTimesInfoReqer.Serialize (YouXiaTimesInfoReqStream,mClearCooltime);
			
			byte[] t_protof;
			
			t_protof = YouXiaTimesInfoReqStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(
				ProtoIndexes.C_YOUXIA_CLEAR_COOLTIME, 
				ref t_protof,
				true,
				ProtoIndexes.S_YOUXIA_CLEAR_COOLTIME_RESP );
		}
		
	}
	void NoTimes(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str = "";
		string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		titleStr = "次数不足";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
	//	Debug.Log ("次数不足");
		str ="\r\n"+"今日挑战次数已用完，是否购买挑战次数？";
		
		uibox.setBox(titleStr,str, null,null,cancel,confirm,BuyTimes);
	
	}
	 void BuyTimes(int i)
	{
		if(i == 2)
		{
			YouXiaTimesInfoReq mYouXiaTimesInfoReq = new YouXiaTimesInfoReq ();
			
			MemoryStream YouXiaTimesInfoReqStream = new MemoryStream ();
			
			QiXiongSerializer YouXiaTimesInfoReqer = new QiXiongSerializer ();
			
			mYouXiaTimesInfoReq.type = big_id;
			
			YouXiaTimesInfoReqer.Serialize (YouXiaTimesInfoReqStream,mYouXiaTimesInfoReq);
			
			byte[] t_protof;
			
			t_protof = YouXiaTimesInfoReqStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(
				ProtoIndexes.C_YOUXIA_TIMES_INFO_REQ, 
				ref t_protof,
				true,
				ProtoIndexes.S_YOUXIA_TIMES_INFO_RESP );
		}
	
	}
	/// <summary>
	/// Changers the mi bao skill button.
	/// </summary>
	public void ChangerMiBaoSkillBtn()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeSkillLoadBack);
	}
	GameObject mChoose_MiBao;
	void ChangeSkillLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (mChoose_MiBao != null) 
			return;
		mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		switch(big_id)
		{
		case 1:
			mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.YX_JinBi),m_You_XiaInfo.zuheId );
			break;
		case 2:
			mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.YX_Cailiao),m_You_XiaInfo.zuheId );
			break;
		case 3:
			mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.YX_Jingpo),m_You_XiaInfo.zuheId );
			break;
		case 4:
			mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.YX_WanbiGuizhao),m_You_XiaInfo.zuheId );
			break;
		case 5:
			mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.YX_ZongHengLiuHe),m_You_XiaInfo.zuheId );
			break;
		default:
			break;
		}

		MainCityUI.TryAddToObjectList(mChoose_MiBao);
		
	}
	List<YouXiaNpcTemplate> mYouXiaNpcTemplateList = new List<YouXiaNpcTemplate>();
	private void CreateEnemy()
	{
		mYouXiaNpcTemplateList.Clear ();
		
		EnemyNumBers = 4;
		Debug.Log ("l_id = "+l_id);
		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById (l_id);
		Debug.Log ("mYouxiaPveTemplate.npcId = "+mYouxiaPveTemplate.npcId);
		mYouXiaNpcTemplateList = YouXiaNpcTemplate.GetYouXiaNpcTemplates_By_npcid(mYouxiaPveTemplate.npcId);

		for (int i = 0; i < mYouXiaNpcTemplateList.Count-1; i ++)
		{
			for(int j = i+1; j < mYouXiaNpcTemplateList.Count; )
			{
				if(mYouXiaNpcTemplateList[i].modelId == mYouXiaNpcTemplateList[j].modelId)
				{

					if(mYouXiaNpcTemplateList[i].type < mYouXiaNpcTemplateList[j].type)
					{
						mYouXiaNpcTemplateList[i] = mYouXiaNpcTemplateList[j];
					}
					mYouXiaNpcTemplateList.RemoveAt(j);
				}
				else{
					j ++;
				}
			}
		}
		for (int i = 0; i < mYouXiaNpcTemplateList.Count-1; i ++)
		{
			for(int j = i+1 ; j < mYouXiaNpcTemplateList.Count; j++)
			{
				if(mYouXiaNpcTemplateList[i].type < mYouXiaNpcTemplateList[j].type)
				{
					YouXiaNpcTemplate mLegendNpc = mYouXiaNpcTemplateList[i];
					mYouXiaNpcTemplateList[i] = mYouXiaNpcTemplateList[j];
					mYouXiaNpcTemplateList[j] = mLegendNpc ;
				}
			}
		}
		getEnemyData();
	}
	
	void getEnemyData()
	{
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateCQ_Enemys);
		}
		else
		{
			WWW temp = null;
			OnCreateCQ_Enemys(ref temp, null, IconSamplePrefab);
		}
	}
	private void OnCreateCQ_Enemys(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		int count = mYouXiaNpcTemplateList.Count;
		if(count > 4)
		{
			count = 4;
		}
		for (int n = 0; n < count; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = EnemyRoot.transform;
			
			int allenemy = mYouXiaNpcTemplateList.Count;
			if(allenemy > 4)
			{
				allenemy = 4;
			}
			
			iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			YouXiaNpcTemplate m_LegendNpcTemplate= YouXiaNpcTemplate.GetYouXiaNpcTemplate_By_id(mYouXiaNpcTemplateList[n].id);
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
			if(m_LegendNpcTemplate.type == 4)
			{
				rightButtomSpriteName = "boss";
			}
			if(m_LegendNpcTemplate.type == 5)
			{
				rightButtomSpriteName = "JunZhu";
			}
			
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(m_LegendNpcTemplate.Name);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + m_LegendNpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(m_LegendNpcTemplate.desc).description;
			
			
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(3, m_LegendNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
		}
	}

	private int numPara;
	private List<int> itemsPara;
	
	private void CreateAward()
	{
		foreach(GameObject maward in m_AwardIcon)
		{
			Destroy(maward);
			
		}
		m_AwardIcon.Clear ();
		
		List<int> t_items = new List<int>();
		
		YouxiaPveTemplate mYouxiaPveTemplate = YouxiaPveTemplate.getYouXiaPveTemplateById(l_id);
		
		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = mYouxiaPveTemplate.awardId.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[0] != "")
			{
				t_items.Add(int.Parse(t_finals[0]));
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
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		int pos = 0;
		
		//Debug.Log("numPara = " +numPara);
		
		for (int n = 0; n < numPara; n++)
		{
			//Debug.Log("itemsPara[n] = " +itemsPara[n]);
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);
			
			for (int i = 0; i < mAwardTemp.Count; i++)
			{
				if(mAwardTemp[i].weight != 0)
				{
					pos += 1;
					if(pos > 4)
					{
						return;
					}
					if (IconSamplePrefab == null)
					{
						IconSamplePrefab = p_object as GameObject;
					}
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
					m_AwardIcon.Add(iconSampleObject);
					iconSampleObject.SetActive(true);
					iconSampleObject.transform.parent = AwardRoot;
					iconSampleObject.transform.localPosition = new Vector3(-150 + (pos - 1) * 100, -20, 0);
					iconSampleObject.transform.localScale = Vector3.one;
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);
					iconSampleManager.SetIconByID(mAwardTemp[i].itemId, "", 3);
					iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, mNameIdTemplate.Name, mdesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
				}
			}
		}
	}
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
	public void OnStronger()
	{
		MainCityUILT.ShowMainTipWindow();
	
	}
	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Destroy (this.gameObject);
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
