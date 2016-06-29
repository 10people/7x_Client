using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class MiBaoGlobleData : MonoBehaviour ,SocketProcessor
{
	private int mChapter;
	public QueryFuwenResp mueryFuwen;
	public Section m_yMapinfo;
	private static MiBaoGlobleData m_instance;
	public MibaoInfoResp G_MiBaoInfo;//返回秘宝信息
	public MibaoInfoResp G_MiBaoInfoCopy;
	public bool MiBaoISNull = true;//判断秘宝是否为空

	public bool MiBaoUp = false;//判断秘宝是否已经升级

	private bool CanLevelUp;
	private bool changeState;
	private int m_Money;

	private int JUNZHULevel;

	private int JunZhuJinBi;

	private int Point;

	public List<MibaoInfo> OldMiBaolist = new List<MibaoInfo>();//久的秘宝

	private int m_iMibaoNum;

	private GameObject m_ChaperAwardUI;//领取章节奖励界面

	[HideInInspector]public List<int > Sec_idlist = new List<int>();

	private bool mZhangJiedataback;

	public bool m_NeedOpenUI;

	public static MiBaoGlobleData Instance()
	{
		if (m_instance == null)
		{
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();;
			
			m_instance = t_gameObject.AddComponent<MiBaoGlobleData>();
		}
		
		return m_instance;
	}



	void Awake()
	{
		changeState = true;
		mZhangJiedataback = false;
		SocketTool.RegisterMessageProcessor(this);
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_NOT_GET_AWART_ZHANGJIE_REQ); 
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);

		m_instance = null;
	}

	void Start()
	{
	
		SendMiBaoIfoMessage();
	}

	//发送秘宝请求
	public void SendMiBaoIfoMessage(bool openUI = false)
	{
		m_NeedOpenUI = openUI;
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
	}
	public void ShowmiBaoPanle()//加载将魂主界面
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_SECRET),
		                        JiangHunLoadCallback);
	}
	public void JiangHunLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject Secrtgb = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(Secrtgb);
	}

	public void GetFuwenInit(int page = 1)
	{
		QueryFuwen  m_mQueryFuwen  = new QueryFuwen  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		m_mQueryFuwen.tab = page;
		MiBaoinfoer.Serialize (MiBaoinfoStream,m_mQueryFuwen);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_MAINPAGE_REQ,ref t_protof,ProtoIndexes.S_FUWEN_MAINPAGE_RES.ToString());	
	}

	public void ShowFuwenPanle()//加载符文主界面
	{
		if(mFuShi)
		{
			return;
		}
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.FUWEN),Load111ResourceCallback);
	}
	private GameObject mFuShi;
	void Load111ResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
	
		mFuShi = Instantiate(p_object )as GameObject;
		
		mFuShi.transform.localScale = Vector3.one;
		
		mFuShi.transform.localPosition = new Vector3 (100,100,0);
		MainCityUI.TryAddToObjectList(mFuShi);
	}

	void Update()
	{

		if(CityGlobalData.MibaoSatrUpCallback &&changeState)
		{
			changeState = false;
			StopCoroutine("ChangeMiBaoStarUp");
			StartCoroutine("ChangeMiBaoStarUp");
		}
		if(MiBaoDataBack)
		{
			if(JUNZHULevel != JunZhuData.Instance().m_junzhuInfo.level || JunZhuJinBi != JunZhuData.Instance().m_junzhuInfo.jinBi ||Point != G_MiBaoInfo.levelPoint)
			{
				JUNZHULevel = JunZhuData.Instance().m_junzhuInfo.level;
				
				JunZhuJinBi = JunZhuData.Instance().m_junzhuInfo.jinBi;
				if(G_MiBaoInfo.levelPoint != null)
				{
					Point = G_MiBaoInfo.levelPoint;
				}
				
				ShowMiBaoCanLevelUp ();
			}
		}
	}

	public void GetPveSectionAward()
	{
		Debug.Log ("Loda PVELevelPass");
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_REMIND_MI_BAO ),OpenLockLoadBack);
	}
	
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		mZhangJiedataback = false;
		if(m_ChaperAwardUI != null)
		{
			return;
		}
		m_ChaperAwardUI = ( GameObject )Instantiate( p_object );
		m_ChaperAwardUI.name = "PVELevelPass";
		
		m_ChaperAwardUI.transform.localPosition = new Vector3(100,100,0);
		
		m_ChaperAwardUI.transform.localScale  = Vector3.one;
		
		PassLevelAward mPassLevelAward = m_ChaperAwardUI.GetComponent<PassLevelAward>();
		mPassLevelAward.Init (Sec_idlist[0]);
		MainCityUI.TryAddToObjectList (m_ChaperAwardUI);
	}

	private GameObject mm_tempObject;


	IEnumerator ChangePveOpenDate()
	{
//		Debug.Log ("ChangePveOpenDate ");
		yield return new WaitForSeconds (2.0f);
		changeState = true;
		CityGlobalData.PveLevel_UI_is_OPen = false;
	}
	IEnumerator ChangeMiBaoStarUp()
	{
		//		Debug.Log ("ChangePveOpenDate ");
		yield return new WaitForSeconds (3.0f);
		changeState = true;
		CityGlobalData.MibaoSatrUpCallback = false;
	}
	bool MiBaoDataBack = false;
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_INFO_RESP://秘宝信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());
				
				G_MiBaoInfo = MiBaoInfo;

//				Debug.Log("G_MiBaoInfo.count = " +G_MiBaoInfo.mibaoGroup.Count);

		  		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (6);
		        CanLevelUp = false;
				MiBaoDataBack = true;
				int index = functionTemp.m_iID;
				int ActiveMiBaonum = 0;
			
                if(CityGlobalData.IsFistGetMiBaoData)
				{
					OldMiBaolist.Clear();
					foreach(MibaoInfo mibaoif in G_MiBaoInfo.miBaoList)
					{
						if(mibaoif.level > 0 )
						{
							OldMiBaolist.Add(mibaoif);
						}
					}
	
					CityGlobalData.IsFistGetMiBaoData = false;
				}
				if(G_MiBaoInfoCopy == null)
				{
					G_MiBaoInfoCopy = MiBaoInfo;
					for(int i = 0; i < G_MiBaoInfoCopy.miBaoList.Count; i ++)
					{
						if(G_MiBaoInfoCopy.miBaoList[i].level > 0)
						{
							m_iMibaoNum ++;
						}
					}
				}
				else
				{
					upMibaoShouji();
				}
				BoolSuperRedPoint();
				if(m_NeedOpenUI)
				{
					m_NeedOpenUI = false;
					ShowmiBaoPanle();
				}
				return true;
			}
			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      密保保存返回
			{
				return true;
			}
			case ProtoIndexes.MIBAO_DEAL_SKILL_RESP://m秘宝技能激活或者进阶返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MiBaoDealSkillResp mMiBaoDealSkillResp = new MiBaoDealSkillResp();
				
				t_qx.Deserialize(t_stream, mMiBaoDealSkillResp, mMiBaoDealSkillResp.GetType());

				//	UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillTemp,EffectIdTemplate.GetPathByeffectId(100178));
				return true;
			}
			case ProtoIndexes.S_NOT_GET_AWART_ZHANGJIE_RESP:// 请weilingqu章节奖励
			{
				
				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				GetNotGetAwardZhangJieResp mGetNotGetAwardZhangJieResp = new GetNotGetAwardZhangJieResp();
				
				t_qx.Deserialize(t_tream, mGetNotGetAwardZhangJieResp, mGetNotGetAwardZhangJieResp.GetType());
				
				Sec_idlist = mGetNotGetAwardZhangJieResp.zhangJiaId;
				if(Sec_idlist == null)
				{
//					Debug.Log("Sec_idlist == nul");
					mZhangJiedataback = false;
				}
				else
				{
					mZhangJiedataback = true;
//					Debug.Log("Sec_idlist.count =" +Sec_idlist.Count);
				}

				return true;
			}
			case ProtoIndexes.S_FUWEN_MAINPAGE_RES: //FW首页返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				QueryFuwenResp mQueryFuwenResp  = new QueryFuwenResp();
				
				t_qx.Deserialize(t_stream, mQueryFuwenResp, mQueryFuwenResp.GetType());
				
				mueryFuwen = mQueryFuwenResp;

				ShowFuwenPanle();
				return true;
			}
			case ProtoIndexes.S_LOAD_FUWEN_IN_BAG_RESP: //FWbag返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenInBagResp mFuwenInBagResp  = new FuwenInBagResp();
				
				t_qx.Deserialize(t_stream, mFuwenInBagResp, mFuwenInBagResp.GetType());
				
				return true;
			}
			case ProtoIndexes.S_FUWEN_OPERAT_RES: //符文镶嵌返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenResp mFuwenResp  = new FuwenResp();
				
				t_qx.Deserialize(t_stream, mFuwenResp, mFuwenResp.GetType());
	
				return true;
			}
			case ProtoIndexes.S_FUWEN_DUI_HUAN_RESP: //符文兑换请求返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenDuiHuanResp mFuwenDuiHuanResp  = new FuwenDuiHuanResp();
				
				t_qx.Deserialize(t_stream, mFuwenDuiHuanResp, mFuwenDuiHuanResp.GetType());

				return true;
			}
			case ProtoIndexes.S_FUWEN_EQUIP_ALL_RESP: //一键穿戴返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenEquipAllResp mFuwenEquipAllResp  = new FuwenEquipAllResp();
				
				t_qx.Deserialize(t_stream, mFuwenEquipAllResp, mFuwenEquipAllResp.GetType());
	
				return true;
			}
			case ProtoIndexes.S_FUWEN_UNLOAD_ALL_RESP: //一键拆卸返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenUnloadAllResp mFuwenUnloadAllResp  = new FuwenUnloadAllResp();
				
				t_qx.Deserialize(t_stream, mFuwenUnloadAllResp, mFuwenUnloadAllResp.GetType());
		
				return true;
			}
		
			default: return false;
			}
			
		}
		
		else
		{
			//Debug.Log("p_message == null");
		}
		
		return false;
	}
	public void BoolSuperRedPoint()
	{
		int activemibaonuber = 0; 
		if(G_MiBaoInfo.skillList == null|| G_MiBaoInfo.skillList.Count == 0 )
		{
			MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(1);
			string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);
			for(int i = 0 ; i < G_MiBaoInfo.miBaoList.Count; i++)
			{
				if(G_MiBaoInfo.miBaoList[i].level > 0)
				{
					activemibaonuber ++;
				}
			}
			if(MainCityUI.m_MainCityUI != null)
			{
				if(MainCityUI.m_MainCityUI.getButton(6) != null)
				{
					MainCityUI.m_MainCityUI.getButton(6).setSuperAlert(activemibaonuber >= mMiBaoskill.needNum);
				}
			}
		}
		else
		{
			int Maxid = 0;
			if(G_MiBaoInfo.skillList.Count >= 7)
			{
				if(MainCityUI.m_MainCityUI != null)
				{
					if(MainCityUI.m_MainCityUI.getButton(6) != null)
					{
						MainCityUI.m_MainCityUI.getButton(6).setSuperAlert(false);
					}
					else
					{
						Debug.Log("MainCityUI.m_MainCityUI.getButton(6) == null ");
					}
				}
			
			}
			else
			{
				for(int i = 0; i < G_MiBaoInfo.skillList.Count; i++)
				{
					//Debug.Log ("m_MiBaoInfo.skillList[i].activeZuheId = "+m_MiBaoInfo.skillList[i].activeZuheId);
					if(G_MiBaoInfo.skillList[i].activeZuheId > Maxid)
					{
						Maxid = G_MiBaoInfo.skillList[i].activeZuheId;//找到最大值
					}
				}
				Maxid  += 1  ;
				MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(Maxid);
				
				for(int i = 0 ; i < G_MiBaoInfo.miBaoList.Count; i++)
				{
					if(G_MiBaoInfo.miBaoList[i].level > 0)
					{
						activemibaonuber ++;
					}
				}
				if(MainCityUI.m_MainCityUI != null)
				{
					if(activemibaonuber >= mMiBaoskill.needNum)
					{
						MainCityUI.m_MainCityUI.getButton(6).setSuperAlert(true);
					}
					else
					{
						MainCityUI.m_MainCityUI.getButton(6).setSuperAlert(false);
					}
				}
			}
			
		}
		
	}

	public void ShowMiBaoCanLevelUp()
	{
//		CanLevelUp = false;
//
//		if( G_MiBaoInfo ==null || G_MiBaoInfo.mibaoGroup == null || G_MiBaoInfo.mibaoGroup.Count == 0)
//		{
//			return;
//		}
//		for (int i = 0; i < G_MiBaoInfo.mibaoGroup.Count; i ++) {
//		
//			for(int j = 0 ; j < G_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j ++)
//			{
//				int lv = G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level;
//				if(lv == 0)
//				{
//					lv = 1;
//				}
//				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].miBaoId);
//				
//				ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId, lv);
//
//				if(G_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
//				   lv < JunZhuData.Instance().m_junzhuInfo.level&&lv < 100&&G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level >=1 &&!G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
//				{
//					
//					CanLevelUp = true;// 有升级点数可以升级秘宝
//					break;
//				}
//			}
//		}
//
////		{
////			TimeHelper.LogDeltaTimeSinceSignet( "After 2 for." );
////			
////			TimeHelper.SignetTime();
////		}
//
//		CantUpMiBao ();
//
////		{
////			TimeHelper.LogDeltaTimeSinceSignet( "After CantUpMiBao." );
////			
////			TimeHelper.SignetTime();
////		}
	}
	void  CantUpMiBao()
	{
		// TODO
		// 秘宝升级全部完成后调用

	//Debug.Log ("CanLevelUp = " +CanLevelUp);

		if(!CanLevelUp)
		{
			if(PushAndNotificationHelper.IsShowRedSpotNotification(600))
			{
				PushAndNotificationHelper.SetRedSpotNotification (600, false);
			}
		}
		else
		{
			if(!PushAndNotificationHelper.IsShowRedSpotNotification(600))
			{
				PushAndNotificationHelper.SetRedSpotNotification (600, true);
			}
		}
	}
	public bool GetMiBaoskillOpen()
	{
	
		if(G_MiBaoInfo.skillList == null)
		{
			return false;
		}
		return true;
	}
	public bool GetEnterChangeMiBaoSkill_Oder()
	{

		if(!GetMiBaoskillOpen ())
		{
			//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),CantOpenSkillUI);
		
			return false;
		}
		return true;
	}
	void CantOpenSkillUI(ref WWW p_www,string p_path, Object p_object)
	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		
//		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
//		
//		string str = "\r\n"+"对不起，您当前没有可使用的秘技，"+"\r\n"+"激活同属一组的两个秘宝可以激活对应的秘技。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
//		
//		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
//		
//		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
//		
//		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,null,null,null);
	}

	public MibaoInfo getMibao(int id)
	{
//		for(int q = 0; q < MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup.Count; q ++)
//		{
//			for(int p = 0; p < MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo.Count; p++)
//			{
//				if(MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo[p].miBaoId == id)
//				{
//					return MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo[p];
//				}
//			}
//		}
		return null;
	}

	public string getStart(int id)
	{
		string returnStart;
		MibaoInfo temp = getMibao(id);
		if(temp == null)
		{
			return "";
		}
		switch(temp.star)
		{
		case 1:
			returnStart = "pinzhi6";
			break;
		case 2:
			returnStart = "pinzhi6";
			break;
		case 3:
			returnStart = "pinzhi6";
			break;
		case 4:
			returnStart = "pinzhi6";
			break;
		case 5:
			returnStart = "pinzhi6";
			break;
		default:
			returnStart = "";
			break;
		}
		return returnStart;
	}
	private int Type;

	private int Skill_id;

	private NewMiBaoSkill.mCloseMiBaoskillDo m_skillDo;

	public void OpenMiBaoSkillUI( int type , int id ,NewMiBaoSkill.mCloseMiBaoskillDo m_mCloseMiBaoskillDo = null )
	{
		Type = type;
		Skill_id = id;
		m_skillDo = m_mCloseMiBaoskillDo;
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);
		
	}
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		
		//mNewMiBaoSkill.COmeMiBaoUI = true;
		
		mNewMiBaoSkill.Init ( Type,Skill_id,m_skillDo );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}
	public void OpenAllianceUI()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT),
		                        AllianceHaveLoadCallback);
	}
	public void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
		
	}
	public void OpenHYMap_UI()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_MAP),
		                        HYMapLoadCallback);
	}
	public void HYMapLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
		HY_UIManager mWildnessManager = tempObject.GetComponent<HY_UIManager>();
		mWildnessManager.init();
	}

	public void upMibaoShouji()
	{
		for(int i = 0; i < G_MiBaoInfoCopy.miBaoList.Count; i ++)
		{
			if(G_MiBaoInfoCopy.miBaoList[i].suiPianNum < G_MiBaoInfo.miBaoList[i].suiPianNum)
			{
				if(G_MiBaoInfoCopy.miBaoList[i].level > 0)
				{
					if(G_MiBaoInfoCopy.miBaoList[i].star < 5)
					{
						MainCityUI.addShouji(G_MiBaoInfoCopy.miBaoList[i].miBaoId, 2, G_MiBaoInfo.miBaoList[i].suiPianNum, G_MiBaoInfo.miBaoList[i].needSuipianNum, "将魂升星");
					}
				}
				else
				{
					MiBaoSuipianXMltemp mmibaosuip = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(G_MiBaoInfo.miBaoList[i].tempId);
					MainCityUI.addShouji(mmibaosuip.icon, 3, G_MiBaoInfo.miBaoList[i].suiPianNum, mmibaosuip.hechengNum, "将魂合成");
				}
			}
		}
		G_MiBaoInfoCopy = G_MiBaoInfo;
		int tempNum = 0;
		for(int i = 0; i < G_MiBaoInfoCopy.miBaoList.Count; i ++)
		{
			if(G_MiBaoInfoCopy.miBaoList[i].level > 0)
			{
				tempNum ++;
			}
		}
		if(tempNum > m_iMibaoNum)
		{
			MiBaoSkillTemp mMiBaoskill;
			if(G_MiBaoInfo.skillList == null|| G_MiBaoInfo.skillList.Count == 0 )
			{
				mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(1);
			}
			else
			{
				mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(G_MiBaoInfo.skillList.Count + 1);
			}

			m_iMibaoNum = tempNum;
			MainCityUI.addShouji(int.Parse(mMiBaoskill.icon), 4, m_iMibaoNum, mMiBaoskill.needNum, "无双技");
		}
	}
	/// <summary>
	/// Tm_Type 1 c创建国家 2 为联盟转国
	/// </summary>
	int m_Type;

	public void LoadCountryUI(int mtype = 2)
	{
		Debug.Log ("mtype = "+mtype);
		m_Type = mtype;
		AllianceChangeCountry ();
	}
	void AllianceChangeCountry()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.COUNTRYUI ),
		                        LoadCountry );
	}
	GameObject mCountry;
	public void LoadCountry( ref WWW p_www, string p_path,  Object p_object )
	{
		
		if(mCountry == null)
		{
			mCountry = Instantiate( p_object ) as GameObject;
			
			mCountry.transform.localScale = Vector3.one;
			mCountry.transform.localPosition = new Vector3 (500,200,0);
			ChooseCountry mChooseCountry = mCountry.GetComponent<ChooseCountry>();
			//mLianmengMuBiaomanager.Lianmeng_Alliance = m_Alliance;
			//int x = (int)(mChooseCountry.ChooseType.Choose);
			mChooseCountry.Init(m_Type);
			
		}
	}
	 List<int >mdi = new List<int> ();
	List<int >mnumbers = new List<int> ();
	/// <summary>
	/// AwardID 物品Id m_numbers物品id对应的物品数量
	/// </summary>
	/// <param name="AwardID">Award I.</param>
	/// <param name="m_numbers">M_numbers.</param>
	private FuWenShow.CallBack m_rulesDelegate;

	private UICamera m_UICamera;

	public  void GetCommAwards(List<int > AwardID,List<int > m_numbers,FuWenShow.CallBack mm_CallBack = null,UICamera mUICamera = null)
	{
		m_rulesDelegate = mm_CallBack;
		mdi = AwardID;
		mnumbers = m_numbers;

		m_UICamera = mUICamera;

		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GETFUWEN),LoadShowFuwenCallback);
	}
	void LoadShowFuwenCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		GameObject ShowFuwen = Instantiate(p_object )as GameObject;
		
		ShowFuwen.transform.localScale = Vector3.one;
		
		ShowFuwen.transform.localPosition = new Vector3 (-100,-100,0);
		
		FuWenShow mFuWenShow = ShowFuwen.GetComponent<FuWenShow>();
	
		mFuWenShow.Init (mdi,mnumbers,m_rulesDelegate,m_UICamera);
	}

}
