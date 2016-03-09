using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class NewAlliancemanager : MonoBehaviour , SocketListener{

	//public GameObject Buttons;

	private string titleStr;
	private string str2;
	private string jieSanTitleStr;
	private string closeTitleStr;
	private string cancelStr;
	private string confirmStr;

	public	AllianceHaveResp m_allianceHaveRes; 

	public JianZhuList m_JianZhu;


	public UILabel AllianceName;

	public UILabel OnlineNum;

	public UILabel Builds;

	public UILabel Label_LeaderBtn;

	public GameObject[] mBUILDS;

	public GameObject DeiInfoMation;

	public GameObject JuanXianUI;

	public GameObject Appbtn;
	public GameObject AppbtnAlert;
	public GameObject NoticeBox;

	ExitAllianceResp mexitResp;

	public GameObject All_KeZhan;
	public GameObject All_FirstUI;
	public GameObject All_ReadRoom;
	public GameObject All_Temples;
	public GameObject All_Apply;

	public GameObject NeedScoleUI;

	public List<AllBuildsTmp> AllBuildsTmp_List = new List<AllBuildsTmp>();

	public static NewAlliancemanager m_NewAlliancemanager;

	[HideInInspector]public int KejiLev;
	[HideInInspector]public int KezhanLev;
	[HideInInspector]public int TutengLev;
	[HideInInspector]public int ZongmiaoLev;
	[HideInInspector]public int ShangPuLev;
	public MyAllianceInfo mMyAllianceInfo;
	public int Up_id;

	public GameObject KeZhan;
	public GameObject Temples;
	public GameObject Apply;
	public GameObject XiaoWu;
	public GameObject HyQs;

	public GameObject BuildUpVecotry;
	public static NewAlliancemanager Instance()
	{
		if (!m_NewAlliancemanager)
		{
			m_NewAlliancemanager = (NewAlliancemanager)GameObject.FindObjectOfType (typeof(NewAlliancemanager));
		}
		return m_NewAlliancemanager;
	}
	
	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
	}
	void Start()
	{
		AllianceData.Instance.RequestData ();
		jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		closeTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_TITLE);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		GetAllianceBuildsMessege ();
		//Init ();
	}

	void OnDestroy(){
		SocketTool.UnRegisterSocketListener(this);

		m_NewAlliancemanager = null;
	}

	void Update()
	{
		//Shownotice ();
	}
	/// <summary>
	/// Shows the alliance GUID.联盟引导
	/// </summary>
	void ShowAllianceGuid()
	{
		UIYindao.m_UIYindao.CloseUI();
		//Debug.Log("联盟引导");
		if(FreshGuide.Instance().IsActive(400010)&& TaskData.Instance.m_TaskInfoDic[400010].progress >= 0)
		{
			//			Debug.Log("去小屋领经验");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400010];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);

			return;
		}
		if(FreshGuide.Instance().IsActive(400020)&& TaskData.Instance.m_TaskInfoDic[400020].progress >= 0)
		{
			//			Debug.Log("去寺庙祭拜");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			
			return;
		}
		if(FreshGuide.Instance().IsActive(400030)&& TaskData.Instance.m_TaskInfoDic[400030].progress >= 0)
		{
			//			Debug.Log("去图腾祭拜");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400030];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			
			return;
		}
		if(FreshGuide.Instance().IsActive(400040)&& TaskData.Instance.m_TaskInfoDic[400040].progress >= 0)
		{
			//			Debug.Log("去商店购买东西");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400040];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			
			return;
		}
	}
	public void GetAllianceBuildsMessege()
	{
		//CityGlobalData.m_isRightGuide = true;
		ShowAllianceGuid ();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_JIAN_ZHU_INFO);
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		// Ui等比放缩
//		UIRoot root = GameObject.FindObjectOfType<UIRoot>();
//		if (root != null) {
//			float s = (float)root.activeHeight / Screen.height;
//			int height = Mathf.CeilToInt (Screen.height * s);
//			int width = Mathf.CeilToInt (Screen.width * s);
//			float v1 = (float)height / (float)640;
//			float v2 = (float)width / (float)960;
//			NeedScoleUI.transform.localScale = new Vector3(v2,v1,1);
//			//Debug.Log ("NeedScoleUI.localScale = " + NeedScoleUI.transform.localScale);
//		} else {
//			//Debug.Log ("root == null");
//		}
	}

	public bool OnSocketEvent(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				
			case ProtoIndexes.ALLIANCE_HAVE_RESP://返回联盟信息， 给有联盟的玩家返回此条信息
			{
				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				AllianceHaveResp allianceHaveRes = new AllianceHaveResp();
				
				t_qx.Deserialize(t_tream, allianceHaveRes, allianceHaveRes.GetType());
				
    			//Debug.Log ("监听到联盟信息返回了");
				
				m_allianceHaveRes = allianceHaveRes;

				InitAlliance();
		
				return true;
			}
			case ProtoIndexes.S_JIAN_ZHU_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				JianZhuList mJianZhuList = new JianZhuList();
				
				t_qx.Deserialize(t_stream, mJianZhuList, mJianZhuList.GetType());

				m_JianZhu = mJianZhuList;

//				Debug.Log("请求建筑返回");

				InitBuilds(mJianZhuList);
				if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
				{
					if(Global.m_sPanelWantRun == "AllianceTuteng")
					{
						WorshipLayerManagerment.m_bulidingLevel = TutengLev;
						Enter_MoBai();
						
						Global.m_sPanelWantRun = "";
					}
					if(Global.m_sPanelWantRun == "AllianceHuangye")
					{
						ENterHY();
						
						Global.m_sPanelWantRun = "";
					}
				}
				return true;
			}
			case ProtoIndexes.S_JIAN_ZHU_UP  :
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ErrorMessage BuildUpback = new ErrorMessage();
				
				t_qx.Deserialize(t_stream, BuildUpback, BuildUpback.GetType());

				//Debug.Log("BuildUpback   ");

				if(BuildUpback.errorCode == 0)
				{
					for(int i = 0; i < AllBuildsTmp_List.Count; i++)
					{
						if(Up_id == AllBuildsTmp_List[i].id){

							AllBuildsTmp_List[i].lv += 1;
							
							AllBuildsTmp_List[i].Init();

							int effectid = 100180;
							BuildUpVecotry.SetActive(true);
							BuildUpVecotry.transform.localPosition = AllBuildsTmp_List[i].gameObject.transform.localPosition;
							UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,BuildUpVecotry,EffectIdTemplate.GetPathByeffectId(effectid));
							StartCoroutine( closeEffect());
						}
					}

				}
				else
				{
					//Debug.Log("升级失败");
				}
				return true;
			}
			case ProtoIndexes.ALLIANCE_FIRE_NOTIFY://被联盟开除通知
			{
				MainCityUI.TryRemoveFromObjectList(this.gameObject);
				Destroy(this.gameObject);
				return true;
			}
			case ProtoIndexes.ALLIANCE_DISMISS_NOTIFY://联盟被解散成功
			{
				MainCityUI.TryRemoveFromObjectList(this.gameObject);
				Destroy(this.gameObject);
				return true;
			
			}
			case ProtoIndexes.EXIT_ALLIANCE_RESP://退出联盟返回
			{
				MemoryStream exit_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer exit_qx = new QiXiongSerializer();
				
				ExitAllianceResp exitResp = new ExitAllianceResp();
				
				exit_qx.Deserialize(exit_stream, exitResp, exitResp.GetType());
				
				if(exitResp != null)
				{
					mexitResp = exitResp;
					
					if (exitResp.code == 0)
					{
						CityGlobalData.m_isMainScene = true;
					}
					
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ExitAllianceLoadCallback );
					
				}
				return true;
			}
			default: return false;
			}
		}
		return false;
	}
	IEnumerator closeEffect()
	{
		yield return new WaitForSeconds (1f);
		BuildUpVecotry.SetActive(false);
	}
	public void InitAlliance()
	{
		//int Online = 0;
		if(m_allianceHaveRes.memberInfo != null)
		{
//			for(int i = 0 ; i < m_allianceHaveRes.memberInfo.Count; i++)
//			{
//				if(m_allianceHaveRes.memberInfo[i].offlineTime < 0)//之前为显示在线人数 后改为显示目前人数和最多能容纳人数
//				{
//					Online += 1 ;
//				}
//			}
			OnlineNum.text = "目前人数/最高容纳人数："+m_allianceHaveRes.memberInfo.Count.ToString()+"/"+m_allianceHaveRes.memberMax.ToString();
		}
		else{
			OnlineNum.text = "目前人数/最高容纳人数： 0/"+m_allianceHaveRes.memberMax.ToString();
		}
	
		AllianceName.text = m_allianceHaveRes.name+"(LV."+m_allianceHaveRes.level.ToString()+")";
		ShowJianSheZhi ();
		ShowBtn ();
		//Noitice.text = m_allianceHaveRes.notice;
	}
	public void ShowJianSheZhi()
	{
		Builds.text = m_allianceHaveRes.build.ToString();
	}
	public void InitBuilds(JianZhuList mJianZhu)
	{
		AllBuildsTmp_List.Clear ();

		for(int i = 0; i < mJianZhu.list.Count; i++)
		{
//			Debug.Log("mJianZhu.list.lv = "+mJianZhu.list[i].lv);

			AllBuildsTmp mAllBuildsTmp = mBUILDS[i].GetComponent<AllBuildsTmp>();

			mAllBuildsTmp.lv = mJianZhu.list[i].lv;

			mAllBuildsTmp.id = i+1;

			AllBuildsTmp_List.Add(mAllBuildsTmp);

			mAllBuildsTmp.Init();
		}
		for(int i = 0; i < 2; i++)
		{
			AllBuildsTmp mAllBuildsTmp = mBUILDS[i+5].GetComponent<AllBuildsTmp>();
		
			mAllBuildsTmp.id = i+6;
	
			mAllBuildsTmp.Init();
		}
		Refreshtification ();
	}
	public void Refreshtification()
	{
		HYInterface mHYInterface1 = KeZhan.GetComponent<HYInterface>();
		mHYInterface1.Init ();
		HYInterface mHYInterface4 = Temples.GetComponent<HYInterface>();
		mHYInterface4.Init ();
		HYInterface mHYInterface5 = Apply.GetComponent<HYInterface>();
		mHYInterface5.Init ();
		HYInterface mHYInterface6 = XiaoWu.GetComponent<HYInterface>();
		mHYInterface6.Init ();
		HYInterface mHYInterface7 = HyQs.GetComponent<HYInterface>();
		mHYInterface7.Init ();
	}
	public void ExitAllianceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string exitTitleStr = "退出联盟";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);
		
		if(mexitResp.code == 0)
		{
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);
			string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES2);
			
			uibox.setBox(exitTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,DeletUI_i);
		}
		else
		{
			//Debug.Log("退出失败");
			
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_FAIL_REASON);
			uibox.setBox(exitTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,null);
		}
	}
	public void DeletUI_i(int i)//Quite。
	{
		JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
		AllianceData.Instance.RequestData();
		//SceneManager.EnterMainCity();
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy(this.gameObject);
	}
	void ShowBtn()
	{
		//Debug.Log ("m_allianceHaveRes.status = "+m_allianceHaveRes.status );
		JuanXianUI.SetActive(false);
		if(m_allianceHaveRes.identity == 2)
		{
			Label_LeaderBtn.text = "解散联盟";

//			JuanXianUI.SetActive(true);

			Appbtn.SetActive(true);
			int mEvent_id = 410000; // 联盟申请
			if(PushAndNotificationHelper.IsShowRedSpotNotification(mEvent_id))
			{
				AppbtnAlert.SetActive(true);
			}
			else
			{
				AppbtnAlert.SetActive(false);
			}
			NoticeBox.GetComponent<BoxCollider>().enabled = true;
		}
		else if(m_allianceHaveRes.identity == 1)
		{
			Label_LeaderBtn.text = "退出联盟";
			//JuanXianUI.SetActive(false);
			Appbtn.SetActive(true);
			int mEvent_id = 410000;
			if(PushAndNotificationHelper.IsShowRedSpotNotification(mEvent_id))
			{
				AppbtnAlert.SetActive(true);
			}
			else
			{
				AppbtnAlert.SetActive(false);
			}
			NoticeBox.GetComponent<BoxCollider>().enabled = true;

		}
		else
		{
//			JuanXianUI.SetActive(false);
//
//			JuanXianUI.transform.localPosition = new Vector3(0,-290,0);

			Appbtn.SetActive(false);
			Label_LeaderBtn.text = "退出联盟";
			NoticeBox.GetComponent<BoxCollider>().enabled = false;
		}

		mMyAllianceInfo.m_Alliance = m_allianceHaveRes;

		mMyAllianceInfo.Init ();
	}
	public void BuyTiLi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
	}
	public void BuyTongBi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
	}
	public void BuyYuanBao()
	{
		MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
    }

	public void Help()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCEINSTRCDUTION));
	}
	public void CheckInfo()
	{
		DeiInfoMation.transform.localScale = Vector3.one;
	}
	public void CloseCheckInfo()
	{
		DeiInfoMation.transform.localScale = new Vector3(0,0,1);
	}

	public void ApplyInfo() //打开申请列表
	{
		All_Apply.SetActive (true);
		All_FirstUI.SetActive (false);
		//Buttons.transform.localPosition = new Vector3(50,298,0);
		ApplyManager mApplyManager = All_Apply.GetComponent<ApplyManager>();
		
		mApplyManager.m_tempInfo = m_allianceHaveRes;
		
		mApplyManager.Init ();

		//AppbtnAlert.SetActive(false);
	}


	public void QuitAlliance()
	{
		if(m_allianceHaveRes.identity == 2) // 盟主
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        AllianceTransTipsLoadCallback1 );
		}
		else
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ExitLoadCallback );
		}
	}
	//解散联盟提示异步加载回调
	public void AllianceTransTipsLoadCallback1 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR2);
		
		uibox.setBox(jieSanTitleStr,MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,cancelStr,confirmStr,DisAlliance);
	}

	//退出联盟提示框异步加载回调
	public void ExitLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR1);
		string str2 = "\n\r"+str1+"\n\r"+LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR2);
		
		string exitTitle = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_TITLE);
		
		uibox.setBox(exitTitle, MyColorData.getColorString (1,str2), null,
		             null,cancelStr,confirmStr,ExitAllianceReq);
	}
	//发送退出联盟的请求
	void ExitAllianceReq (int i)
	{
		if (i == 2)
		{
			ExitAlliance exitReq = new ExitAlliance ();
			
			exitReq.id = m_allianceHaveRes.id;
			
			MemoryStream exitStream = new MemoryStream ();
			
			QiXiongSerializer exitQx = new QiXiongSerializer ();
			
			exitQx.Serialize (exitStream, exitReq);
			
			byte[] t_protof = exitStream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.EXIT_ALLIANCE, ref t_protof, "30114");
		}
	}

	void DisAlliance (int i)
	{
		if(i == 2)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        AllianceTransTipsLoadCallback2 );
		}
	}
	//解散联盟再次提示异步加载回调
	public void AllianceTransTipsLoadCallback2 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR3);
		string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR4);
		
		string sanSiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SANSI);
		string jieSanStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN);
		
		uibox.setBox(jieSanTitleStr,MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,sanSiStr,jieSanStr,DisAllianceReq);
	}
	//发送解散联盟请求
	void DisAllianceReq (int i)
	{
		if(i == 2)
		{
			DismissAlliance disAllianceReq = new DismissAlliance ();
			
			disAllianceReq.id = m_allianceHaveRes.id;
			
			MemoryStream dis_stream = new MemoryStream ();
			
			QiXiongSerializer disQx = new QiXiongSerializer ();
			
			disQx.Serialize (dis_stream,disAllianceReq);
			
			byte[] t_protof = dis_stream.ToArray();;
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.DISMISS_ALLIANCE,ref t_protof,"30132");
			//Debug.Log ("jiesanReq:" + ProtoIndexes.DISMISS_ALLIANCE);
		}
	}
	public void ENterOtherUI(int m_id)
	{
		//Debug.Log ("m_id = " +m_id );
		//Buttons.transform.localPosition = new Vector3(50,298,0);
		switch(m_id)
		{
		case 1:
			All_KeZhan.SetActive (true);
			AllianceKeZhanManager mAllianceKeZhanManager = All_KeZhan.GetComponent<AllianceKeZhanManager>();
			mAllianceKeZhanManager.m_AllianceHaveRes = m_allianceHaveRes;
			mAllianceKeZhanManager.Init();
			break;
		case 2:
			All_ReadRoom.SetActive (true);
			TechnologyManager mTechnologyManager = All_ReadRoom.GetComponent<TechnologyManager>();
			mTechnologyManager.Init();
			break;
		case 3:
			// 膜拜
			break;
		case 4:
			//商铺
			break;
		case 5:
			All_Temples.SetActive (true);
			AllianceTemples mAllianceTemples = All_Temples.GetComponent<AllianceTemples>();
			mAllianceTemples.Init();
			break;
		default:
			break;
		}
		All_FirstUI.SetActive (false);
	}
	public void ENterHY()
	{
		if(m_allianceHaveRes.level >= 2)
		{
			MiBaoGlobleData.Instance().OpenHYMap_UI ();
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),CantEnterOpenLockLoadBack);
		}
	}

	void CantEnterOpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str2 = "";
		
		string str1 = "\r\n"+"联盟等级到达2级才能进入荒野求生！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1+str2), null,null,confirmStr,null,null,null,null);
	}
	[HideInInspector]
	public OldBookWindow m_OldBookWindow;
	public void ENterAllianceHorse()
	{
		//Buttons.transform.localPosition = new Vector3(50,298,0);
		All_FirstUI.SetActive (false);
		if (m_OldBookWindow != null)
		{
			m_OldBookWindow.gameObject.SetActive(true);
			m_OldBookWindow.OldBookMode = OldBookWindow.Mode.ExchangeBoxOther;
			m_OldBookWindow.RefreshUI();
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.OLD_BOOK_WINDOW), OnBookLoadCallBack);
		}
	}
	void OnBookLoadCallBack(ref WWW www, string path, object loadedObject)
	{
		var tempObject = Instantiate(loadedObject as GameObject) as GameObject;
		m_OldBookWindow = tempObject.GetComponent<OldBookWindow>();
		m_OldBookWindow.IsSelfHouse = true;
		//m_OldBookWindow.m_House = this;
		
		TransformHelper.ActiveWithStandardize(transform, m_OldBookWindow.transform);

		ENterAllianceHorse();
	}
	public void EnterShop()
	{
		ShopData.Instance.OpenShop (ShopData.ShopType.GONGXIAN);
	}

	public void Enter_MoBai() // 进入膜拜界面  直接复制TutengLev膜拜等级
	{
        WorshipLayerManagerment.m_bulidingLevel = TutengLev;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
                                     WorshipLayerLoadCallback);
    
    }
    private static void WorshipLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }
    public void BackToThis(GameObject m_game)
	{
		//Buttons.transform.localPosition = new Vector3(0,298,0);
	    m_game.SetActive (false);
		All_FirstUI.SetActive (true);

	}

	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy(this.gameObject);
	}
}
