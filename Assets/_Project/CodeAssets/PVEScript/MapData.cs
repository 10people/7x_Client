using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class MapData : MonoBehaviour, SocketListener {

	public GameObject mMap;

	public UISprite MapBianKuang;

	[HideInInspector]public bool UI_exsit = false;
	public int GuoGuanNunbers;
    public  Section myMapinfo; 
	public static MapData mapinstance;
	public Dictionary<int,Level> Lv = new Dictionary<int,Level>();
	[HideInInspector]public List<Pve_Level_Info> Pve_Level_InfoList = new List<Pve_Level_Info> ();

	[HideInInspector]public List<Level> CQLv = new List<Level> ();

	[HideInInspector]public List<int> Save_MibaoId = new List<int> ();

	[HideInInspector]public int JYLvs;
	[HideInInspector]public int Starnums;
	[HideInInspector]public int CurrChapter;
	[HideInInspector]public int nowCurrChapter;

	[HideInInspector]public int CoM_Chapter;//普通关卡的当前章节
	[HideInInspector]public int Cq_Chapter;//传奇关卡的打到了的章节

	[HideInInspector]public int Cq_CurrChapter;//传奇关卡的当前的章节

	[HideInInspector]public int AllChapteres;
	//[HideInInspector]public bool Is_Com_Lv = true;//判断普通关卡和传奇关卡、默认显示普通关卡
	public GameObject Lev;
	private int YinDaoId1_3;
//	private int YinDaoId2_3;

	public  bool isnewplaye = true;

	bool IsFirstIn = true;

	public UISprite rightbtn;

	public GameObject PveUIs1;
	public GameObject PveUIs2;
	public int LingJiangStar ;
	public int LingJiangStared ;

	public bool ShowYinDao = false;

	public bool ShowYinDaoBackToCity = false;

	[HideInInspector ]public bool IsShowGuid;//是否显示或者刷新引导
	[HideInInspector]public bool IsCloseGuid = false;//判断是否关闭引导

	[HideInInspector]public int GuidLevel = 0;//用于做引导的关卡id
	[HideInInspector]public bool IsLingJIang = false;//判断是否领过奖励

	//List <GameObject> UIs = new List<GameObject>();

	void Awake()
	{ 
		SocketTool.RegisterSocketListener(this);
		mapinstance = this;
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, ShowPVEGuid );
		}
     }

	void Start ()
	{
		StartCoroutine (Changestatebtn());
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5 ,null ,null);
		GuidLevel = 0;
//		InitData ();
	}

	void OnDestroy(){

		SocketTool.UnRegisterSocketListener(this);

		mapinstance = null;
	}

	IEnumerator Changestatebtn()
	{
		yield return new WaitForSeconds (0.5f);

		PveUImanager mPveUImanger = PveUIs1.GetComponent<PveUImanager>();

		mPveUImanger.ChangeStateBtn();
	}

    void Update()
	{
//		Debug.Log ("a = "+CityGlobalData.PveLevel_UI_is_OPen);
		if(ShowYinDao && !CityGlobalData.PveLevel_UI_is_OPen && !GeneralRewardManager.Instance().IsExitReward())
		{
			CityGlobalData.m_isRightGuide = false;
			ShowPVEGuid();
		}
	}
	[HideInInspector]public int ShowEffectLevelid;

	public void OpenEffect()
	{
//		Debug.Log ("ShowEffectLevelid = "+ShowEffectLevelid);

		foreach(Pve_Level_Info mPve_Level_Info in Pve_Level_InfoList)
		{
			if(ShowEffectLevelid == mPve_Level_Info.litter_Lv.guanQiaId)
			{
				int effectId =  100171;
				
				UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,mPve_Level_Info.gameObject,
				                                               EffectIdTemplate.GetPathByeffectId(effectId));
			}
		}

	}

	public void CloseEffect()
	{
		foreach(Pve_Level_Info mPve_Level_Info in Pve_Level_InfoList)
		{
			if(ShowEffectLevelid == mPve_Level_Info.litter_Lv.guanQiaId)
			{
				UI3DEffectTool.ClearUIFx (mPve_Level_Info.gameObject);
			}
		}

	}
	public	void ClosewPVEGuid()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	public void ShowPve_PT_GuidSection1() // 第一章节引导
	{
		//CityGlobalData.m_isRightGuide = false;
		//Debug.Log("进入第一张引导");
		if(FreshGuide.Instance().IsActive(100020)&& TaskData.Instance.m_TaskInfoDic[100020].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-1");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100030)&& TaskData.Instance.m_TaskInfoDic[100030].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-2");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100030];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100040)&& TaskData.Instance.m_TaskInfoDic[100040].progress >= 0)
		{
//			Debug.Log("引导返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100040];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-3");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100055)&& TaskData.Instance.m_TaskInfoDic[100055].progress >= 0)
		{
//			Debug.Log("点击1-3领奖励");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100055];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100060)&& TaskData.Instance.m_TaskInfoDic[100060].progress >= 0)
		{
//			Debug.Log("引导返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100060];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100080)&& TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-4");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100080];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100090)&& TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-5");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100090];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100175)&& TaskData.Instance.m_TaskInfoDic[100175].progress >= 0)
		{
			//			Debug.Log("签到一次 1-5");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100175];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100100)&& TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
		{
			//Debug.Log("引导返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100100];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100120)&& TaskData.Instance.m_TaskInfoDic[100120].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-6");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100120];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100160)&& TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
		{
			//			Debug.Log("首次探宝");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100160];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100130)&& TaskData.Instance.m_TaskInfoDic[100130].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 1-7");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100130];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
			ShowYinDao = false;
			return;
		}
//		if(FreshGuide.Instance().IsActive(100140)&& TaskData.Instance.m_TaskInfoDic[100140].progress >= 0)
//		{
////			Debug.Log("指向宝箱");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100140];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
//			ShowYinDao = false;
//			return;
//		}

		if (FreshGuide.Instance().IsActive (100145) && TaskData.Instance.m_TaskInfoDic [100145].progress >= 0) {
			Debug.Log("领完奖励回程");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100145];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		ClosewPVEGuid ();
	}
	public void BackToCity()
	{
		if (FreshGuide.Instance().IsActive (100145) && TaskData.Instance.m_TaskInfoDic [100145].progress >= 0) {
			Debug.Log("领完奖励回程");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100145];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
	}
	public void ShowPve_PT_GuidSection2() // 第二章节引导
	{
		//Debug.Log("进入第2张引导");
//		if(FreshGuide.Instance().IsActive(100140)&& TaskData.Instance.m_TaskInfoDic[100140].progress >= 0)
//		{
////			Debug.Log("指向宝箱");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100140];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
//			ShowYinDao = false;
//			return;
//		}
		if (FreshGuide.Instance().IsActive (100145) && TaskData.Instance.m_TaskInfoDic [100145].progress >= 0) {
//			Debug.Log("领完奖励回程");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100145];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100200)&& TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
		{
//			Debug.Log("在第二张中完成2-2返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100200];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100230)&& TaskData.Instance.m_TaskInfoDic[100230].progress >= 0)
		{
			//			Debug.Log("在第二张中完成2-2返回主城");(新增)
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100230];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100250)&& TaskData.Instance.m_TaskInfoDic[100250].progress >= 0)
		{
//			Debug.Log("在第二张中完成2-3返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100250];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100255)&& TaskData.Instance.m_TaskInfoDic[100255].progress >= 0)
		{
			//			Debug.Log("在第二张中完成2-3返回主城");(新增)
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100255];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
//
//		if(FreshGuide.Instance().IsActive(100260)&& TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
//		{
//			//			Debug.Log("切换秘技)
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100260];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
//			ShowYinDao = false;
//			return;
//		}

		if(FreshGuide.Instance().IsActive(100280)&& TaskData.Instance.m_TaskInfoDic[100280].progress >= 0)
		{
//			Debug.Log("在第二张中完成2-5返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100280];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}

		if(FreshGuide.Instance().IsActive(100285)&& TaskData.Instance.m_TaskInfoDic[100285].progress >= 0)
		{
			//			Debug.Log("在第二张中完成2-5返回主城");(新增)
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100285];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}

		if(FreshGuide.Instance().IsActive(100305)&& TaskData.Instance.m_TaskInfoDic[100305].progress >= 0)
		{
//			Debug.Log("在第二张中完成2-7返回主城");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100305];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
	
		ClosewPVEGuid ();
	}
	public void GuidTO_CQ() // 引导第一次攻打传奇关卡
	{
		int _Dex = 1;
		//Debug.Log("攻打传奇关卡");
		if(!CityGlobalData.PT_Or_CQ)
		{
			_Dex = 2;
		}
		if(FreshGuide.Instance().IsActive(100320)&& TaskData.Instance.m_TaskInfoDic[100320].progress >= 0)
		{
//			Debug.Log("攻打传奇关卡");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100320];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[_Dex]);
			ShowYinDao = false;
			return;
		}
	}
    public	void ShowPVEGuid()
	{
		if (!ShowYinDao) {
			return;
		}
		ShowYinDaoBackToCity = false;
//		Debug.Log ("myMapinfo.s_section = "+myMapinfo.s_section);
		if(EnterGuoGuanmap.Instance().ShouldOpen_id > 1)
		{
			return;
		}
		switch(myMapinfo.s_section)
		{
		case 1:
			ShowPve_PT_GuidSection1();
			break;
		case 2:
			ShowPve_PT_GuidSection2();
			break;
		default:
			break;
		}
		GuidTO_CQ();
		if(FreshGuide.Instance().IsActive(100405)&& TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
		{
			//			Debug.Log("返回城中扫荡");(新增)
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100405];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(!UIYindao.m_UIYindao.m_isOpenYindao)
		{
			if(FreshGuide.Instance().IsActive(200005)&& TaskData.Instance.m_TaskInfoDic[200005].progress >= 0)
			{
                Debug.Log("返回城中激活新秘宝");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200005];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(200010)&& TaskData.Instance.m_TaskInfoDic[200010].progress >= 0)
			{
				Debug.Log("返回城中激活绿色秘术");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200010];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
			{
				Debug.Log("返回城中去试练");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(100280)&& TaskData.Instance.m_TaskInfoDic[100280].progress >= 0)
			{
				Debug.Log("返回城中有新功能开启");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100280];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(100405)&& TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
			{
				Debug.Log("返回城中激活蓝色秘宝");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100405];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(100057)&& TaskData.Instance.m_TaskInfoDic[100057].progress >= 0)
			{
				Debug.Log("返回城中激活两个绿品质的秘宝");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100057];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(200030)&& TaskData.Instance.m_TaskInfoDic[200030].progress >= 0)
			{
				Debug.Log("返回城中有宝石镶嵌");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200030];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
			if(FreshGuide.Instance().IsActive(200040)&& TaskData.Instance.m_TaskInfoDic[200040].progress >= 0)
			{
				Debug.Log("返回城中有装备坊抽奖");
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200040];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
				ShowYinDao = false;
				return;
			}
		}
	}
	public bool IsChoosedMiBao;
	public bool MapdataBack;
	public void  startsendinfo(int Currchapter)
	{
		ClosewPVEGuid ();
		MapdataBack = false;
		sendmapMessage( Currchapter );
	}

	public bool OnSocketEvent(QXBuffer p_message){

		if (p_message != null)
		{
			switch (p_message.m_protocol_index){
				case ProtoIndexes.PVE_PAGE_RET:
				{
//					Debug.Log( " PVE Message Return: " + ProtoIndexes.PVE_PAGE_RET );

					MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					Section tempInfo = new Section();
				  
					t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				// 临时添加判断条件  只能到13章节
			
				    MapdataBack = true;
					if(tempInfo.s_section > 13)
					{
						tempInfo.s_section =13;
					}
					if(tempInfo.sectionMax > 13)
					{
						tempInfo.sectionMax = 13;
					}
					if(tempInfo.maxCqPassId > 13)
					{
						tempInfo.maxCqPassId = 13;
					}
			     	myMapinfo = tempInfo;
				    
					if(!mMap.activeInHierarchy)
					{
						mMap.SetActive(true);
					}
					InitData();
					return true;
				}

				default: return false;
			}
		
		}

		return false;
	}
	public void InitData()
	{

		MapType ();
//	
//		myMapinfo = MiBaoGlobleData.Instance().m_yMapinfo;
//		
		Debug.Log("aaa" +myMapinfo.s_section);
		CurrChapter = myMapinfo.s_section;

		ShowYinDao = true;
		
		CityGlobalData.m_temp_CQ_Section = myMapinfo.maxCqPassId;

		if(myMapinfo.sectionMax < 0)
		{
			myMapinfo.sectionMax = 1;
		}
		CityGlobalData.m_LastSection = myMapinfo.sectionMax;

		AllChapteres = CityGlobalData.m_LastSection;
		//第一次进入pve场景时候获得当前的章节数
		if(IsFirstIn)
		{
			CoM_Chapter = myMapinfo.s_section;
			Cq_Chapter = CoM_Chapter - 1;
			Cq_CurrChapter = Cq_Chapter;
			
			nowCurrChapter = CurrChapter;
			IsFirstIn = false;
			
		}
//		Debug.Log ("CityGlobalData.PT_Or_CQ :"+CityGlobalData.PT_Or_CQ);
		if(CityGlobalData.PT_Or_CQ){
			Initmapinfo();  
			PassLevelBtn.Instance().InitData(CurrChapter);
			ShowLvs();
		}
		else{
			Init_Cqmapinfo();
			
		}
		
		if(PveUIs1){
			PveUIs1.GetComponent<PveUImanager>().ShowChapterName(CurrChapter);
		}
	}
	public void Init_Cqmapinfo()//初始化传奇关卡的地图信息
	{
		CQLv.Clear ();
		GuoGuanNunbers = 0;
		foreach(Level tempLevel in myMapinfo.s_allLevel)
		{
			//Debug.Log ("tempLevel.type  = " +tempLevel.type );
			if(tempLevel.type == 1)
			{
				CQLv.Add(tempLevel);
			}
		}

		foreach(Level tempLevel in CQLv)
		{
		
			if(tempLevel.chuanQiPass)
			{
				GuoGuanNunbers += 1;
			}
		}
		if(GuoGuanNunbers ==  CQLv.Count)
		{
			AllCQ_Lv_Passed = true;
		}
		else{
			AllCQ_Lv_Passed = false;
		}
		choosemap mchoosemap = PveUIs2.GetComponent<choosemap>();

		if(nowCurrChapter != CurrChapter)
		{
			mchoosemap.sortmap(CurrChapter);
		}
        else
        {
			mchoosemap.AddCurMap (CurrChapter);
		}

	}
	public bool AllPtLv_Passed = false; //判断是不是这一章所以关卡都过关了
	public bool AllCQ_Lv_Passed = false; //判断是不是这一章所以关卡都过关了
	public void Initmapinfo()
	{

		if(myMapinfo.s_allLevel == null)
		{
			//Debug.Log("关卡的个数为null。。。。。。。");
			return ;
		}
		GuoGuanNunbers = 0;
		foreach(Level tempLevel in myMapinfo.s_allLevel)
		{
			if(tempLevel.guanQiaId == 100205)
			{
//				Debug.Log("tempLevel.win_Level = " +tempLevel.win_Level);
			}

			if(!Lv.ContainsKey(tempLevel.guanQiaId))
				Lv.Add(tempLevel.guanQiaId,tempLevel);
			if(tempLevel.s_pass)
			{
				GuoGuanNunbers += 1;
			}
		}
		if(GuoGuanNunbers ==  myMapinfo.s_allLevel.Count)
		{
			AllPtLv_Passed = true;
		}
		else{
			AllPtLv_Passed = false;
		}
		choosemap mchoosemap = PveUIs2.GetComponent<choosemap>();
	
		if(nowCurrChapter != CurrChapter)
		{
			mchoosemap.sortmap(CurrChapter);
		}else{
			mchoosemap.AddCurMap (CurrChapter);
		}


	}
	
	void  ShowLvs()
	{
//		Debug.Log ("Global.m_sPanelWantRun ==== "+Global.m_sPanelWantRun);
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			if(Global.m_sPanelWantRun == "chuanqi")
			{
				PveUImanager.instances.CQ_Gamelv();

				Global.m_sPanelWantRun = "";
			}
		}
	}
    public  void MapType()
	{
	    if(CityGlobalData.PT_Or_CQ)
		{
			MapBianKuang.spriteName = "MapDikuang";
		}
		else
		{
			MapBianKuang.spriteName = "ChuanQiDiKuang";
		}
	}
	public static void sendmapMessage(int a)
	{
		CityGlobalData.PveLevel_UI_is_OPen = false;

//		Debug.Log (" a = "+ a);
		PvePageReq mapinfo = new PvePageReq ();

		MemoryStream mapStream = new MemoryStream ();

		QiXiongSerializer maper = new QiXiongSerializer ();

		mapinfo.s_section = a;

		maper.Serialize (mapStream,mapinfo);

		byte[] t_protof;

		t_protof = mapStream.ToArray();

		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.PVE_PAGE_REQ, 
			ref t_protof,
			true,
			ProtoIndexes.PVE_PAGE_RET );
	}
}
