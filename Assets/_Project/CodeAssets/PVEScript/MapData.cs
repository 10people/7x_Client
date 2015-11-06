using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class MapData : MonoBehaviour, SocketProcessor {

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
		SocketTool.RegisterMessageProcessor(this);
		mapinstance = this;
     }

	void Start ()
	{

		StartCoroutine (Changestatebtn());

	}

	IEnumerator Changestatebtn()
	{
		yield return new WaitForSeconds (0.5f);

		PveUImanager mPveUImanger = PveUIs1.GetComponent<PveUImanager>();

		mPveUImanger.ChangeStateBtn();
	}

    void Update()
	{
//		if(UIs.Count > 0)
//		{
//			if(UIYindao.m_UIYindao.m_isOpenYindao)
//			{
//				UIYindao.m_UIYindao.CloseUI();
//			}
//		}
		if(ShowYinDao)
		{
			CityGlobalData.m_isRightGuide = false;
			ShowPVEGuid();
		}
		if(CityGlobalData.PveLevel_UI_is_OPen)
		{
			if(ShowYinDaoBackToCity)
			{
				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
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
				
				UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,mPve_Level_Info.gameObject,
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
				UI3DEffectTool.Instance ().ClearUIFx (mPve_Level_Info.gameObject);
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
    public	void ShowPVEGuid()
	{
		ShowYinDaoBackToCity = false;
		if(FreshGuide.Instance().IsActive(100020)&& TaskData.Instance.m_TaskInfoDic[100020].progress >= 0&&CurrChapter == 1&&GuidLevel == 0)
		{
//			Debug.Log("进入PVE 第一个任务 点击右边按钮翻页");

			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100020)&& TaskData.Instance.m_TaskInfoDic[100020].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{
			//Debug.Log("指向2-1");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			ShowYinDao = false;
			return;
		}
//		Debug.Log();
		if(FreshGuide.Instance().IsActive(100030)&& TaskData.Instance.m_TaskInfoDic[100030].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{     
//			Debug.Log("指向2-2");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100030];
			tempTaskData.m_iCurIndex = 2;
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
		    ShowYinDao = false;

		    return; 
		}
		if(FreshGuide.Instance().IsActive(100040)&& TaskData.Instance.m_TaskInfoDic[100040].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{
//			Debug.Log("Back!");

			ShowYinDaoBackToCity = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100040];
			tempTaskData.m_iCurIndex = 0;
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
			ShowYinDao = false;
			return;
		}


		//Debug.Log("wancheng 2-3 fanhui");
		if(FreshGuide.Instance().IsActive(100060)&& TaskData.Instance.m_TaskInfoDic[100060].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{       
//			Debug.Log("wancheng 2-3 fanhui");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100060];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}

		//Debug.Log("wancheng  2-4 fanhui");
		if(FreshGuide.Instance().IsActive(100070)&& TaskData.Instance.m_TaskInfoDic[100070].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{      
//			Debug.Log("wancheng  2-4 fanhui");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100070];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100080)&& TaskData.Instance.m_TaskInfoDic[100080].progress >= 0&&GuidLevel == 0)
		{
//			Debug.Log("回成了");

			ShowYinDaoBackToCity = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100080];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}

		//Debug.Log("wancheng  2-5 fanhui");
		if(FreshGuide.Instance().IsActive(100090)&& TaskData.Instance.m_TaskInfoDic[100090].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{      
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100090];

//			Debug.Log( "task1:" + tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}

		if(FreshGuide.Instance().IsActive(100100)&& TaskData.Instance.m_TaskInfoDic[100100].progress >= 0&&GuidLevel == 0)
		{
//			Debug.Log("有一次返回城中");

			ShowYinDaoBackToCity = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100100];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}



		if(FreshGuide.Instance().IsActive(100120)&& TaskData.Instance.m_TaskInfoDic[100120].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{
			//			Debug.Log("2-6");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100120];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}

		if(FreshGuide.Instance().IsActive(100130)&& TaskData.Instance.m_TaskInfoDic[100130].progress >= 0&&CurrChapter == 2&&GuidLevel == 0)
		{
			//			Debug.Log("2-7");
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100130];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100140)&& TaskData.Instance.m_TaskInfoDic[100140].progress >= 0&&GuidLevel == 0)
		{
			//			Debug.Log("Back   ");
			ShowYinDaoBackToCity = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100140];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress >= 0&&CurrChapter == 3&&GuidLevel == 0)
		{
			//			Debug.Log("3-1   ");
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100180)&& TaskData.Instance.m_TaskInfoDic[100180].progress >= 0&&GuidLevel == 0)
		{
			//			Debug.Log("Back ");
			ShowYinDaoBackToCity = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
			ShowYinDao = false;
			return;
		}

		if(FreshGuide.Instance().IsActive(100230)&& TaskData.Instance.m_TaskInfoDic[100230].progress >= 0&&CurrChapter == 3&&GuidLevel == 0)
		{
			//			Debug.Log("3-2 ");
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100230];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			ShowYinDao = false;
			return;
		}
		if(FreshGuide.Instance().IsActive(100240)&& TaskData.Instance.m_TaskInfoDic[100240].progress >= 0&&CurrChapter == 3&&GuidLevel == 0)
		{
//			Debug.Log("3-3 ");

			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100240];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);

			return;
		}

		if(FreshGuide.Instance().IsActive(100290)&& TaskData.Instance.m_TaskInfoDic[100290].progress >= 0&&GuidLevel == 0)
		{
			//			Debug.Log("back ");
			ShowYinDaoBackToCity = true;
			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100290];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);

			return;
		}
//		if(FreshGuide.Instance().IsActive(100310)&& TaskData.Instance.m_TaskInfoDic[100310].progress >= 0&&GuidLevel == 0)
//		{
//			Debug.Log("3-7 ");
//			ShowYinDao = false;
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100310];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
//
//			return;
//		}
		//Debug.Log("传奇1 CityGlobalData.PT_Or_CQ =  "+CityGlobalData.PT_Or_CQ);
		if(FreshGuide.Instance().IsActive(100340)&& TaskData.Instance.m_TaskInfoDic[100340].progress >= 0&&GuidLevel == 0&&CityGlobalData.PT_Or_CQ&&EnterGuoGuanmap.Instance().ShouldOpen_id < 1)
		{
			Debug.Log("传奇 = "+EnterGuoGuanmap.Instance().ShouldOpen_id);
			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100340];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);

			return;
		}
		//Debug.Log("传奇 2 GuidLevel = " +GuidLevel);
		if(FreshGuide.Instance().IsActive(100340)&& TaskData.Instance.m_TaskInfoDic[100340].progress >= 0&&GuidLevel == 0&&!CityGlobalData.PT_Or_CQ)
		{
//			Debug.Log("传奇 2-3");

			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100340];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);

			return;
		}
		if(FreshGuide.Instance().IsActive(100350)&& TaskData.Instance.m_TaskInfoDic[100350].progress >= 0&&GuidLevel == 0&&!CityGlobalData.PT_Or_CQ)
		{
			//			Debug.Log("传奇back ");
			ShowYinDaoBackToCity = true;
			ShowYinDao = false;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100350];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);

			return;
		}

	}
	public bool IsChoosedMiBao;
	public void  startsendinfo(int Currchapter)
	{
		YinDaoId1_3 = 1;
		if(FreshGuide.Instance().IsActive(100020)&& TaskData.Instance.m_TaskInfoDic[100020].progress >= 0)
		{
//			Debug.Log("进入PVE 第一个任务 点击右边按钮翻页");
			sendmapMessage (1);
			return;
		}
		sendmapMessage( Currchapter );
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){

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

			     	myMapinfo = tempInfo;

				    CurrChapter = myMapinfo.s_section;
					if(UIYindao.m_UIYindao.m_isOpenYindao)
					{
						UIYindao.m_UIYindao.CloseUI();
					}
				    ShowYinDao = true;

				    CityGlobalData.m_temp_CQ_Section = tempInfo.maxCqPassId;

					if(tempInfo.s_section > CityGlobalData.m_LastSection)
					{
						CityGlobalData.m_LastSection = tempInfo.s_section;
					}
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
				   if(CityGlobalData.PT_Or_CQ){

						Initmapinfo();  
					}
					else{
						Init_Cqmapinfo();
					   
					}
				     	
					if(PveUIs1){
						PveUIs1.GetComponent<PveUImanager>().ShowChapterName(CurrChapter);
					}
				    
					return true;
				}

				default: return false;
			}
		
		}

		return false;
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

		//StartCoroutine(ShowLvs());
	}
	
	IEnumerator ShowLvs()
	{
		if(choosemap.UpAndDownbtn)
		{
			yield return new WaitForSeconds(0.1f);

		}else{
			yield return new WaitForSeconds(0.5f);
		}
	}

	public static void sendmapMessage(int a)
	{
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

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
