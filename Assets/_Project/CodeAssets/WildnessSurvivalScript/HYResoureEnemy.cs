using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HYResoureEnemy : MonoBehaviour , SocketProcessor {

	public UILabel ResourseName;

	//public HuangYeResource mHuangYeResource;

	public UILabel AllianceName;//占领的联盟的名字

	public UILabel Num_PreHour;//每小时产量

	public UILabel BattleTimes;//挑战次数和剩余次数

	public GameObject ChangeBtn; //更换按钮

	public GameObject ChangeBtnColider; //按钮遮罩 

	public UILabel[] ZhanLi;//战力1
	//public UILabel ZhanLi2;//战力2
	//public UILabel ZhanLi3;//战力3

	public UILabel[] Bat_Look;//挑战或者查看

	//public UILabel Bat_Look2;//挑战或者查看
	//public UILabel Bat_Look3;//挑战或者查看

	public GameObject[] BattleFinshedColider; //按钮遮罩 

	public UISprite[] FinshTips;//

	public UISprite[] EnemyIcom;

	public UILabel ResourseInstrction; //资源点描述

	private static HYResoureEnemy g_HYResoureEnemy;

	public UILabel[] BattleName;

	public GameObject[] Battle_Name;

	public GameObject[] TiaoZhanBtn;

	public UISprite[] Btnbg;

	private bool m_isEnterBattle = false;
	private int m_iEnterIndex = 0;

	public static HYResoureEnemy Instance ()
	{
		if (!g_HYResoureEnemy)
		{
			g_HYResoureEnemy = (HYResoureEnemy)GameObject.FindObjectOfType (typeof(HYResoureEnemy));
		}
		
		return g_HYResoureEnemy;
	}

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);

	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	

	}
	
//	public void SendMessage_HYResoures()
//	{
////		BattleResouceReq mHYResouresBattle = new BattleResouceReq ();
////		
////		MemoryStream HYResouresBattleStream = new MemoryStream ();
////		
////		QiXiongSerializer HYResouresBattleBattleper = new QiXiongSerializer ();
////
////		mHYResouresBattle.id = mHuangYeResource.id;
////
////		Debug.Log ("mHuangYeResource.id = "+mHuangYeResource.id);
////
////		HYResouresBattleBattleper.Serialize (HYResouresBattleStream,mHYResouresBattle);
////		
////		byte[] t_protof;
////		
////		t_protof = HYResouresBattleStream.ToArray();
////		
////		SocketTool.Instance().SendSocketMessage(
////			ProtoIndexes.C_HYRESOURCE_BATTLE, 
////			ref t_protof,
////			true,
////			ProtoIndexes.S_HYRESOURCE_BATTLE_RESP );
//	}
//
	public bool OnProcessSocketMessage(QXBuffer p_message){
//		
//		if (p_message != null)
//		{
//			switch (p_message.m_protocol_index){
//
////			case ProtoIndexes.S_HYRESOURCE_BATTLE_RESP:// 查看资源点返回
////			{
////				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
////				
////				QiXiongSerializer t_qx = new QiXiongSerializer();
////				
////				//BattleResouceResp mHYResourseBattleResp = new BattleResouceResp();
////				
////				t_qx.Deserialize(t_stream, mHYResourseBattleResp, mHYResourseBattleResp.GetType());
////
////				if(mHYResourseBattleResp != null)
////				{
////					Debug.Log("Data Back now !");
////
////					m_HYResourseBattleResp = mHYResourseBattleResp;
////
////					InitUi(mHYResourseBattleResp);
////				}
////
////				if(m_isEnterBattle)
////				{
////					m_isEnterBattle = false;
////					if(mHYResourseBattleResp.resNpcInfo[m_iEnterIndex].battleName != "")
////					{
////						return true;
////					}
////					if( mHuangYeResource.status == 1) // 我方资源点
////					{
////						if(mHYResourseBattleResp.resNpcInfo[m_iEnterIndex].battleSuccess == 1)
////						{
////							return true;
////						}
////					}
////					if( mHuangYeResource.status == 0||mHuangYeResource.status == 2) 
////					{
////						if(mHYResourseBattleResp.resNpcInfo[m_iEnterIndex].battleSuccess == 1)
////						{
////							return true;
////						}
////					}
////					mResourceNpcInfo = mresNpcInfo [m_iEnterIndex];
////					en_zhanli = m_zhanli[m_iEnterIndex];
////					my_zhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
////					EnemyPos = m_iEnterIndex;
////					OpenEnemyUI ();
////				}
////				return true;
////			}
////			case ProtoIndexes.S_HYRESOURCE_CHANGE_RESP: // 更换资源点返回
////			{
////				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
////				
////				QiXiongSerializer t_qx = new QiXiongSerializer();
////				
////				ResourceChangeResp mResourceChangeResp = new ResourceChangeResp();
////				
////				t_qx.Deserialize(t_stream, mResourceChangeResp, mResourceChangeResp.GetType());
////				Debug.Log("mResourceChangeResp.result== "+mResourceChangeResp.result);
////				if(mResourceChangeResp.result == 0)
////				{
////					Debug.Log("mResourceChangeResp Back now !");
////					m_HYResourseBattleResp = mResourceChangeResp.resResp;
////					InitUi(m_HYResourseBattleResp);
////					foreach(HuangyeResource m_HuangyeResource in WildnessManager.Instance().mHuangYe_RES_List)
////					{
////						if(m_HuangyeResource.mHuangYeResource.id == m_HYResourseBattleResp.resourceId)  // 要更换为资源点Id
////						{
////							//m_HuangyeResource.mHuangYeResource = m_HYResourseBattleResp; //有错误  数据
////							m_HuangyeResource.init();
////						}
////					}
////				}
////
////				
////				return true;
////			}	
//			default: return false;
//			}
//		}
	return false;
	}
//	private int[] mbossId = new int[3];
//
//	private int[] m_zhanli  = new int[3];
//
//	private int My_Time;// 更换时间
//
//	public GameObject ShowCountTime;
//
////	void InitUi(BattleResouceResp mBattleResouceResp)
////	{
////		if(mHuangYeResource.name == "")
////		{
////			AllianceName.text = "无";
////		}
////		else
////		{
////			AllianceName.text = mHuangYeResource.name;//占领联盟的名字
////		}
////
////		if( mHuangYeResource.status == 1) // 我方资源点
////		{
////			ShowCountTime.SetActive(false);
////		}
////		HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid (mHuangYeResource.fileId);//m描述
////
////		string mDescIdTemplate = DescIdTemplate.GetDescriptionById (mHuangyeTemplate.descId);
////
////		string mName = NameIdTemplate.GetName_By_NameId (mHuangyeTemplate.nameId);
////
////		ResourseName.text = mName;
////
////		ResourseInstrction.text = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_68);;
////
////		Num_PreHour.text = mBattleResouceResp.produce.ToString()+"/"+"小时"; 
////
////		BattleTimes.text = mBattleResouceResp.timesOfDay.ToString () + "/" + mBattleResouceResp.totalTimes.ToString ();//总次数和剩余次数
////
////		mresNpcInfo.Clear ();
////
////		Debug.Log("mBattleResouceResp.remainTime = "+mBattleResouceResp.remainTime);
////
////		if( mHuangYeResource.status == 2) 
////		{
////			ChangeBtn.SetActive(true);
////
////			ChangeBtnColider.SetActive(true);
////
////			ChangeBtn.SetActive(m_HYResourseBattleResp.isChange == 1);
////		}
////		if(mBattleResouceResp == null )
////		{
////			Debug.Log("mBattleResouceResp == null");
////		}
////		if(mBattleResouceResp.resNpcInfo == null)
////		{
////			Debug.Log("mBattleResouceResp.resNpcInfo == null");
////		}
////
////		for(int i = 0; i < mBattleResouceResp.resNpcInfo.Count; i ++)
////		{
////			HuangyePvpNpcTemplate mHuangyePvpNpcTemplate = HuangyePvpNpcTemplate.getHuangyepvpNPCTemplate_by_Npcid(mBattleResouceResp.resNpcInfo[i].bossId);
////
////			mbossId[i] = mBattleResouceResp.resNpcInfo[i].bossId;
////
//////			Debug.Log("	mbossId[i] = " +	mbossId[i]);
////
////			mresNpcInfo.Add(mBattleResouceResp.resNpcInfo[i]);
////
////			EnemyIcom[i].spriteName = mHuangyePvpNpcTemplate.icon.ToString();//mHuangyePvpNpcTemplate.icon;
////			//mBattleResouceResp.timesOfDay = 0;
////			if(mBattleResouceResp.timesOfDay <= 0)
////			{
////				Btnbg[i].gameObject.SetActive(true);
////			}
////			if(mBattleResouceResp.resNpcInfo[i].battleName != "")
////			{
////			    TiaoZhanBtn[i].SetActive(false);
////
////			    Battle_Name[i].gameObject.SetActive(true);
////
////				Debug.Log("	 mBattleResouceResp.resNpcInfo[i].battleName = " +	mBattleResouceResp.resNpcInfo[i].battleName);
////
////			    BattleName[i].text = mBattleResouceResp.resNpcInfo[i].battleName;
////			}
////			else
////			{
////				TiaoZhanBtn[i].SetActive(true);
////				
////				Battle_Name[i].gameObject.SetActive(false);
////			}
////			if( mHuangYeResource.status == 1) // 我方资源点
////			{
////				Bat_Look[i].text = "查看";
////				if(mBattleResouceResp.resNpcInfo[i].battleSuccess == 1)
////				{
////					TiaoZhanBtn[i].SetActive(false);
////
////					BattleFinshedColider[i].SetActive(true);
////
////					FinshTips[i].spriteName = "Lost";
////				}
////			}
////			if( mHuangYeResource.status == 0||mHuangYeResource.status == 2) 
////			{
////				Bat_Look[i].text = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_27);
////
////				if(mBattleResouceResp.resNpcInfo[i].battleSuccess == 1)
////				{
////					TiaoZhanBtn[i].SetActive(false);
////
////					BattleFinshedColider[i].SetActive(true);
////
////					FinshTips[i].spriteName = "Win";
////				}
////			}
////			m_zhanli[i] = 0;
////			for(int j = 0; j < mBattleResouceResp.resNpcInfo[i].yongBingId.Count; j++)
////			{
////				HY_GuYongBingTempTemplate mGuYongBingTempTemplate = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(mBattleResouceResp.resNpcInfo[i].yongBingId[j]);
////
////				m_zhanli[i] += mGuYongBingTempTemplate.power;
////
////			}
////			m_zhanli[i] += mHuangyePvpNpcTemplate.power;
////
////			ZhanLi[i].text = m_zhanli[i].ToString();
////		}
////
////		GameObject EnemyZhengrong = GameObject.Find ("New_ZhengRong(Clone)");
////
////		if(EnemyZhengrong != null)
////		{
////
////			TiaoZhan tiaoZhan = EnemyZhengrong.GetComponent<TiaoZhan> ();
////
////			tiaoZhan.BattleType = 1;
////
////			tiaoZhan.m_Zl = JunZhuData.Instance().m_junzhuInfo.zhanLi;
////
////			tiaoZhan.E_Zl = m_zhanli[EnemyPos];
////
////			tiaoZhan .m_ResInfo = mBattleResouceResp.resNpcInfo[EnemyPos];
////
////			tiaoZhan .my_HYResourseBattleResp = m_HYResourseBattleResp;
////			
////
////			tiaoZhan.ResType = mHuangYeResource.status;//状态  //状态：0-NPC占领，1-我方占领，2-表示被其他联盟占领
////			// tiaoZhan.ResType = 1;//
////			tiaoZhan.my_HuangYeResource = mHuangYeResource;
////
////			tiaoZhan.Init_HYUI (m_HYResourseBattleResp.zuheId,0);
////		}
////	}
////	
////	List<ResourceNpcInfo> mresNpcInfo = new List<ResourceNpcInfo>();
////	
////	public BattleResouceResp m_HYResourseBattleResp;
////
////	ResourceNpcInfo mResourceNpcInfo;
////
////	int en_zhanli;
////
////	int my_zhanli;
////
////	int EnemyPos;
////
////	public void BattleBtn1()
////	{
////		m_iEnterIndex = 0;
////		m_isEnterBattle = true;
////		SendMessage_HYResoures();
////	}
////	public void BattleBtn2()
////	{
////		m_iEnterIndex = 1;
////		m_isEnterBattle = true;
////		SendMessage_HYResoures();
////	}
////	public void BattleBtn3()
////	{
////		m_iEnterIndex = 2;
////		m_isEnterBattle = true;
////		SendMessage_HYResoures();
////	}
////
////	void OpenEnemyUI()
////	{
////
////		if (mHuangYeResource.status != 1) {
////
////			if(m_HYResourseBattleResp.timesOfDay <= 0)
////			{
////				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadNoTimeBoxBack);
////				return;
////			}
////		}
////		Debug.Log (" 111显示 荒野资源点的敌人窗口");
////
////		TiaoZhanWindow ();
////	}
////
////	void LoadNoTimeBoxBack(ref WWW p_www,string p_path, Object p_object)
////	{
////		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
////
////		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_19);
////		
////		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_23)+"0";
////	
////		string ComfireStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
////
////		uibox.setBox(titleStr,null, str1,null,ComfireStr,null,null,null,null);
////		
////	}
////
////
////	int cost = 0;
////	void LoadBoxBack(ref WWW p_www,string p_path, Object p_object)
////	{
////		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
////
////		HuangYePVPTemplate mHuangYePVPTemplate = HuangYePVPTemplate.getHuangYePVPTemplate_byid (mHuangYeResource.fileId);
////
////	    cost = mHuangYePVPTemplate.refreshCost;
////
////		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_36);
////		
////		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_37);
////
////		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_38);
////
////		string str3 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_39);
////
////		string str4 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_40);
////
////		string ComfireStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
////
////		string CancleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
////		uibox.setBox(titleStr,null, str1+cost.ToString()+str2+str3+str4,null,CancleStr,ComfireStr,ComChange,null,null,null);
////		
////	}
////	void ComChange(int i)
////	{
////		if(i == 2)
////		{
////			if(cost <= AllianceData.Instance.g_UnionInfo.build)
////			{
////
////				if(AllianceData.Instance.g_UnionInfo.identity != 0)
////				{
////					ResourceChange mResourceChange = new ResourceChange ();
////					
////					MemoryStream mResourceChangeStream = new MemoryStream ();
////					
//					QiXiongSerializer mResourceChangeer = new QiXiongSerializer ();
//					
//					mResourceChange.id = mHuangYeResource.id;
//					Debug.Log ("mHuangYeResource.id = "+mHuangYeResource.id);
//					mResourceChangeer.Serialize (mResourceChangeStream,mResourceChange);
//					
//					byte[] t_protof;
//					
//					t_protof = mResourceChangeStream.ToArray();
//					
//					SocketTool.Instance().SendSocketMessage(
//						ProtoIndexes.C_HYRESOURCE_CHANGE, 
//						ref t_protof,
//						true,
//						ProtoIndexes.S_HYRESOURCE_CHANGE_RESP );
//
//				}
//				else
//				{
//					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LocadIdentyBack);
//				}
//			
//			}
//			else
//			{
//				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LocadLockBuildBack);
//			}
//		}
//	}
//
//	void LocadIdentyBack(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//
//		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_41);
//
//		string Str = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_44);
//
//		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
//
//		uibox.setBox(titleStr,null, Str,null,confirmStr,null,null,null,null);
//		
//	}
//
//	void LocadLockBuildBack(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//	
//		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_41);
//		
//		string Str = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_42);
//
//		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
//
//		uibox.setBox(titleStr,null, Str,null,confirmStr,null,null,null,null);
//		
//	}
//
//    public void ChangeResoures()
//	{
//		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadBoxBack);
//
//	}
//	void TiaoZhanWindow ()
//	{
//		Debug.Log ("22222显示 荒野资源点的敌人窗口");
//		//mHuangYeResource.status = 1;
//		if(mHuangYeResource.status == 1)
//		{
//			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_ZHENG_RONG ),
//			                        DownLoad_Mresourse );
//		}
//		else{
//			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.NEW_ZHENGRONG ),
//			                        TiaoZhanLoadCallback );
//		}
//
//	}
//
//	void DownLoad_Mresourse( ref WWW p_www, string p_path,  Object p_object )
//	{
//		GameObject challengeWin = Instantiate (p_object) as GameObject;
//		
//		challengeWin.SetActive (true);
//		
//		challengeWin.transform.parent = this.gameObject.transform.parent;
//		
//		challengeWin.transform.localPosition = Vector3.zero;
//		
//		challengeWin.name = "mResourse";
//		
//		challengeWin.transform.localScale = Vector3.one;
//
//		Debug.Log (" 33333333显示我的资源点的防守阵容");
//
//		MyResourse M_MyResourse = challengeWin.GetComponent<MyResourse> ();
//
//		M_MyResourse.ZhaliNum = en_zhanli;
//
//		M_MyResourse .m_ResInfo = mResourceNpcInfo;
//
//		M_MyResourse .mHYResourseBattleResp = m_HYResourseBattleResp;
//
//		M_MyResourse.my_HuangYeResource = mHuangYeResource;
//
//		M_MyResourse.Init ();
//	}
//
//	public void TiaoZhanLoadCallback( ref WWW p_www, string p_path,  Object p_object )
//	{
//		GameObject challengeWin = Instantiate (p_object) as GameObject;
//		
//		challengeWin.SetActive (true);
//		
//		challengeWin.transform.parent = this.gameObject.transform.parent;
//		
//		challengeWin.transform.localPosition = Vector3.zero;
//
//		challengeWin.name = "NEW_ZHENGRONG";
//
//		challengeWin.transform.localScale = Vector3.one;
//		Debug.Log (" 33333333显示 荒野资源点的敌人窗口");
//		TiaoZhan tiaoZhan = challengeWin.GetComponent<TiaoZhan> ();
//		tiaoZhan.BattleType = 1;
//		tiaoZhan.m_Zl = my_zhanli;
//		tiaoZhan.E_Zl = en_zhanli;
//		tiaoZhan .m_ResInfo = mResourceNpcInfo;
//		tiaoZhan .my_HYResourseBattleResp = m_HYResourseBattleResp;
//
//		Debug.Log (" tiaoZhan.ResType = "+tiaoZhan.ResType);
//
//		tiaoZhan.ResType = mHuangYeResource.status;//状态  //状态：0-NPC占领，1-我方占领，2-表示被其他联盟占领
//		// tiaoZhan.ResType = 1;//
//		tiaoZhan.my_HuangYeResource = mHuangYeResource;
//		tiaoZhan.NumberOfEnemyPosition = EnemyPos;
//
//
//		tiaoZhan.Init_HYUI (m_HYResourseBattleResp.zuheId,0);
//	}
//
//	public void BackBtn()
//	{
//
//		Destroy (this.gameObject);
//
//	}


}

























