using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MainCityUI : MYNGUIPanel, SocketListener
{
    public delegate void MainCityUIDelegate();

    /// <summary>
    /// Execute this after init if it is not null
    /// </summary>
    public static MainCityUIDelegate m_MainCityUiDelegate;
	public static GuanQiaMaxId  MapCurrentInfo = new GuanQiaMaxId();
    public static GameObject m_objBelongings;
	public static GameObject m_objTitle;
	public UISprite m_spriteBBG;
    public enum PlayerPlace
    {
        MainCity,
        HouseSelf,
        HouseOther
    }

    public static PlayerPlace m_PlayerPlace;

    public static bool m_isClick = true;
    public static MainCityUI m_MainCityUI;
    public MainCityUIL m_MainCityUIL;
    public MainCityUIRB m_MainCityUIRB;
    public MainCityUILT m_MainCityUILT;
    public MainCityUIRT m_MainCityUIRT;
    public Joystick m_MainCityUILB;
    public GameObject m_AddFunction;
    public GameObject m_ObjMainCityButtonS;

    public MainCityListButtonManager m_MainCityListButton_L;
	public MainCityListButtonManager m_MainCityListButton_LT;
    public MainCityListButtonManager m_MainCityListButton_RT;
    public MainCityListButtonManager m_MainCityListButton_RB;
    public MainCityListButtonManager m_MainCityListButton_R;
	public MainCityListButtonManager m_MainCityListButton_RTRT;
    public MainCityUITongzhi m_MainCityUITongzhi;
    public List<MainCityListButtonManager> m_listMainCityButtons = new List<MainCityListButtonManager>();
    public GameObject ButtonPrefab;
	public TPController m_TpController;
    private GameObject m_bagObject;
    private int BigHouseId = 0;
    private int SmallHouseId = 0;

	public static List<ShoujiData> m_listShoujiData = new List<ShoujiData>();
    [HideInInspector]
    public List<MYNGUIPanel> m_MYNGUIPanel = new List<MYNGUIPanel>();

    public GameObject equipObject;
    public int a = 0;
	public static bool m_isAdd = false;
	private bool m_isQiangzhiClick = false;
	public GameObject m_objRichangRed;
    /// <summary>
    /// 1.auto cleared when accessed;
    /// 2.After negotiation, all objects contained here are only Functions' Main UI, not sub UI popped.
    /// </summary>
    public List<GameObject> m_WindowObjectList
    {
        get
        {         
            //Clear object list.
            for (int i = 0; i < m_windowObjectList.Count; i++)
            {
                if (m_windowObjectList[i] == null || !m_windowObjectList[i].activeInHierarchy)
                {
                    m_windowObjectList.RemoveAt(i);
                }
            }

            return m_windowObjectList;
        }
    }

    private List<GameObject> m_windowObjectList = new List<GameObject>();

	public FuLiHuoDongResp m_FuLiHuoDongResp;

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
        
        m_MainCityUI = this;
        m_MYNGUIPanel.Add(m_MainCityUILT);
        m_MYNGUIPanel.Add(m_MainCityUIRT);
        m_MYNGUIPanel.Add(null);
        m_MYNGUIPanel.Add(m_MainCityUIRB);
        m_MYNGUIPanel.Add(m_MainCityUIL);
    }

    void Start()
    {
		for(int i = 0; i < ClientMain.m_listPopUpData.Count; i ++)
		{
			if(ClientMain.m_listPopUpData[i].iLevel == 40)
			{
				ClientMain.m_listPopUpData.RemoveAt(i);
				i--;
			}
		}

        ClientMain.m_isNewOpenFunction = false;

        if (Global.m_isOpenJiaoxue)
        {
			if(Global.m_isZhanli)
			{
				ClientMain.closePopUp();
			}
            if (m_WindowObjectList.Count != 0)
            {

            }

//            if (Global.m_isOpenBaiZhan)
//            {
////                PvpData.Instance.OpenPvp();
//				SportData.Instance.OpenSport ();
//            }

			if (Global.m_isOpenHuangYe && JunZhuData.Instance().m_junzhuInfo.lianMengId != 0)
			{
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_MAP),
                                        LoadHY_Map);
            }
        }

        m_isInited = true;
		Global.m_isAddZhanli = true;
		Global.m_isZhanli = false;
		Global.m_iPZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;

        setInit();
        setInitButtons();
		SetRedAlert(106, FunctionOpenTemp.GetTemplateById(106).m_show_red_alert);
		MishuGlobalData.Instance.scend();
        Global.ScendID(ProtoIndexes.C_ADD_TILI_INTERVAL, 1);
		Global.ScendNull(ProtoIndexes.C_FULIINFO_REQ);
		Global.ScendNull(ProtoIndexes.C_MengYouKuaiBao_Req);
		if(m_objBelongings == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_Belongings), OnLoadCallBack);
		}
		if(m_objTitle == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_Titile ), OnLoadCallBackTitle);
		}
        //		Global.m_isZhanli = true;
		if(!m_isAdd)
		{
			m_isAdd = true;
//			ClientMain.addPopUP(40, 2, "9", null);
		}
		Global.ScendNull(ProtoIndexes.PVE_MAX_ID_REQ);
//      ClientMain.addPopUP(40, 2, "140", null);
//		FunctionOpenTemp.m_EnableFuncIDList.Add(140);
//      ClientMain.addPopUP(40, 2, "139", null);
//		m_MainCityUIRB.setPropUse(103103,30);
//		m_MainCityUIRB.setPropUse(103104,20);
//		m_MainCityUIRB.setPropUse(103105,0);
//		MainCityUI.addShouji(100, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(101, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(102, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(103, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(104, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(105, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(106, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(107, 0, 1, 5, "装备进阶");
//		MainCityUI.addShouji(108, 0, 1, 5, "装备进阶");

//		MainCityUI.addShouji(101, 4, 100, 5, "无双技");
//		MainCityUI.addShouji(102, 4, 100, 5, "无双技");
//		MainCityUI.addShouji(103, 4, 100, 5, "无双技");
//		MainCityUI.addShouji(104, 4, 100, 5, "无双技");
//		MainCityUI.addShouji(105, 4, 100, 5, "无双技");
//		MainCityUI.addShouji(106, 4, 100, 5, "无双技");
    }

    public static bool IsWindowsExist()
    {
        if (MainCityUI.m_MainCityUI == null) return false;

        return MainCityUI.m_MainCityUI.m_WindowObjectList.Count > 0;
    }

    public static bool TryAddToObjectList(GameObject go, bool p_add_to_2d_tool = true)
    {
        if (m_MainCityUI != null)
        {
			for(int i = 0; i < m_MainCityUI.m_WindowObjectList.Count; i ++)
			{
				if(m_MainCityUI.m_WindowObjectList[i].name == go.name)
				{
					return false;
				}
			}
            m_MainCityUI.m_WindowObjectList.Add(go);

            // assume all param:go is functionUI's main page, if not will cause an error
            if (p_add_to_2d_tool)
            {
                UI2DTool.Instance.AddTopUI(go);
            }
        }
        else
        {
            Debug.LogWarning("Warning, MainCityUI not exist.");
        }
		return true;
    }

    public static void TryRemoveFromObjectList(GameObject go)
    {
        if (m_MainCityUI != null && m_MainCityUI.m_WindowObjectList.Contains(go))
        {
            m_MainCityUI.m_WindowObjectList.Remove(go);
        }
    }

    public static bool IsExitInObjectList(GameObject go)
    {
        if (m_MainCityUI != null && m_MainCityUI.m_WindowObjectList.Contains(go))
        {
            return true;
        }
        return false;
    }

    //#if UNITY_EDITOR

    void OnGUI()
    {
        GameObject temp;
        //if(GUI.Button(new Rect(200,200,100,30), "运镖"))
        //{
        //	temp = new GameObject();
        //	temp.name = "MainCityUIButton_310";
        //	MYClick(temp);
        //}
        //		if(GUI.Button(new Rect(300,200,100,30), "主线任务"))
        //		{
        //			temp = new GameObject();
        //			temp.name = "MainCityUIButton_5";
        //			MYClick(temp);
        //		}
        //		if(GUI.Button(new Rect(400,200,100,30), "出征"))
        //		{
        //			temp = new GameObject();
        //			temp.name = "MainCityUIButton_8";
        //			MYClick(temp);
        //		}
        //		if(GUI.Button(new Rect(500,200,100,30), "游侠"))
        //		{
        //			temp = new GameObject();
        //			temp.name = "MainCityUIButton_300";
        //			MYClick(temp);
        //		}
        //		if(GUI.Button(new Rect(600,200,100,30), "掠夺"))
        //		{
        //			temp = new GameObject();
        //			temp.name = "MainCityUIButton_211";
        //			MYClick(temp);
        //		}
        //		for(int i = 0; i < m_listTongzhiData.Count; i ++)
        //		{
        //			GUI.Box(new Rect(180, 230 + i * 30, 400, 30), DescIdTemplate.GetDescriptionById(m_listTongzhiData[i].m_ReportTemplate.m_iLanguageID));
        //			for(int q = 0; q < m_listTongzhiData[i].m_listState.Count; q ++)
        //			{
        //				string tempShowText = "";
        //				switch(m_listTongzhiData[i].m_listState[q])
        //				{
        //				case 0:
        //					tempShowText ="显示";
        //					break;
        //				case 1:
        //					tempShowText ="忽略";
        //					break;
        //				case 2:
        //					tempShowText ="前网";
        //					break;
        //				case 3:
        //					tempShowText ="祝福";
        //					break;
        //				case 4:
        //					tempShowText ="安慰";
        //					break;
        //				case 5:
        //					tempShowText ="领取";
        //					break;
        //				case 6:
        //					tempShowText ="知道了";
        //					break;
        //				}
        //				if(GUI.Button(new Rect(580 + q * 50, 230 + i * 30, 50, 30), tempShowText))
        //				{
        //					PromptActionReq req = new PromptActionReq();
        //					
        //					req.reqType = m_listTongzhiData[i].m_listState[q];
        //
        //					req.suBaoId = m_listTongzhiData[i].m_SuBaoMSG.subaoId;
        //					
        //					MemoryStream tempStream = new MemoryStream();
        //					
        //					QiXiongSerializer t_qx = new QiXiongSerializer();
        //					
        //					t_qx.Serialize(tempStream, req);
        //					
        //					byte[] t_protof;
        //					
        //					t_protof = tempStream.ToArray();
        //					
        //					SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Prompt_Action_Req, ref t_protof);
        //
        ////					Global.ScendID(ProtoIndexes.C_Prompt_Action_Req, m_listTongzhiData[i].m_listState[q]);
        //					m_listTongzhiData.RemoveAt(i);
        //					q--;
        //				}
        //			}
        //		}
        //        if (GUILayout.Button("DO"))
        //        {
        //            ClearObjectList();
        //        }
    }

    //#endif

    /// <summary>
    /// Destroy all object from objectList and clear list.
    /// </summary>
    public static void ClearObjectList()
    {
        m_MainCityUI.m_WindowObjectList.ForEach(item => Destroy(item));
        m_MainCityUI.m_WindowObjectList.Clear();
    }

    void OnDestroy()
    {
        m_MainCityUI = null;

        SocketTool.UnRegisterSocketListener(this);

		if(UIShoujiManager.m_UIShoujiManager != null)
		{
			UIShoujiManager.m_UIShoujiManager.close();
		}
    }


    public static bool IsShowFunctionOpenEffectInAllianceCity = false;

    void OnLevelWasLoaded()
    {
        if (IsShowFunctionOpenEffectInAllianceCity
            && (Application.loadedLevelName == SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.ALLIANCE_CITY)
                    || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY
                    || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN
            || Application.loadedLevelName == SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.ALLIANCE_CITY_YE_WAN)))
        {
            IsShowFunctionOpenEffectInAllianceCity = false;

            ClientMain.addPopUP(40, 2, "" + 400000, null);
            //            ClientMain.addPopUP(50, 2, "" + 400000, null);
            ClientMain.addPopUP(40, 2, "" + 7, null);
            //            ClientMain.addPopUP(50, 2, "" + 7, null);
        }
    }



    private void OnLoadCallBack(ref WWW www, string str, object obj)
    {
		m_objBelongings = obj as GameObject;
    }

	private void OnLoadCallBackTitle(ref WWW www, string str, object obj)
	{
//		Debug.Log("============1");

		m_objTitle = obj as GameObject;
	}
    /// <summary>
    ///  通用资源显示
    /// </summary>
    /// <param name="parent">绑定在哪个OBJ下面</param>
    /// <param name="x">坐标X</param>
    /// <param name="y">坐标Y</param>
	public static void setGlobalBelongings(GameObject parent, float x, float y, MYNGUIPanel panel = null, OnOPenGuiBtnClick m_OnOPenGuiBtnClick = null)
    {
        GameObject tempObj;
		tempObj = GameObject.Instantiate(m_objBelongings);

        tempObj.transform.parent = parent.transform;
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.localPosition = new Vector3(x, y, 0);

		MainCityBelongings tempMainCityBelongings = tempObj.GetComponent<MainCityBelongings>();
		for(int i = 0; i < 3; i ++)
		{
			tempMainCityBelongings.m_listButtonMessage[i].panel = tempMainCityBelongings;
		}
		onOPenGuiBtnClick = m_OnOPenGuiBtnClick;
    }
	
	public delegate void OnOPenGuiBtnClick ();
	
	public static OnOPenGuiBtnClick onOPenGuiBtnClick;

	public static void OpenGui()
	{
		if(onOPenGuiBtnClick != null)
		{
			onOPenGuiBtnClick();
		}
	}

	public static void setGlobalTitle(GameObject parent, string data, float x, float y, MYNGUIPanel panel = null)
	{
		GameObject tempObj;

		tempObj = GameObject.Instantiate(m_objTitle);
		
		tempObj.transform.parent = parent.transform;
		tempObj.transform.localScale = Vector3.one;
		tempObj.transform.localPosition = new Vector3(x, y, 0);

		MainCityLTTitle tempMainCityTitle = tempObj.GetComponent<MainCityLTTitle>();
		if(data.Length > 2)
		{
			tempMainCityTitle.m_labelTitle.spacingX = 2;
		}
		else if(data.Length == 2)
		{
			tempMainCityTitle.m_labelTitle.spacingX = 13;
		}
		data = "[b]" + data + "[-]";
		tempMainCityTitle.m_labelTitle.text = data;
//		tempMainCityTitle.m_labelType.setType(0);
	}

	private int[] m_mission = new int[]
    {
		100010,0,100020,0,100030,0,100040,1,100050,0,100055,0,100057,1,100060,1,100080,0,
		100090,0,100100,1,100110,0,100120,0,100130,0,100140,0,100145,1,
		100150,0,100160,1,100170,0,100173,0,100175,1,100177,0,100180,0,
		100190,0,100200,1,100210,0,100220,0,100230,1,100240,0,100250,1,
		100255,1,100257,0,100259,0,100260,0,100270,0,100280,1,100285,1,
		100290,0,100300,0,100305,1,100307,0,100310,0,100315,1,100320,0,
		100330,1,100340,0,100350,0,100360,0,100370,0,100404,2,100405,1,
		100407,0,100409,0,100410,0,100440,0,100460,0,100470,1,100705,0,
		200005,1,200010,1,200040,1,200020,1,200030,1,200060,1,200065,1,
    };
    public void setInit()
    {
        //		Debug.Log( "MainCityUI.SetInit()" );

        {
#if UNITY_IPHONE
            UtilityTool.UnloadUnusedAssets();
#endif
        }
        if (CityGlobalData.m_debugPve == true)
        {
            EnterBattleField.EnterBattlePveDebug();

            return;
        }
        if (Global.m_isOpenJiaoxue)
        {
            if (isFirstAnimation())
            {
                return;
            }
            if (!m_isClick)
            {
                return;
            }
            //Cancel fresh guide if not in main city.
            if (MainCityUI.m_PlayerPlace != PlayerPlace.MainCity)
            {
                Debug.LogWarning("cancel fresh guide cause not in main city.");
                return;
            }
            //			Debug.Log(FreshGuide.Instance().IsActive(100040));
            if (m_MainCityUI.m_WindowObjectList.Count == 0)
            {
                for (int i = 0; i < m_mission.Length; i += 2)
                {
                    if (FreshGuide.Instance().IsActive(m_mission[i]) && TaskData.Instance.m_TaskInfoDic[m_mission[i]].progress >= 0)
                    {
//						Debug.Log(m_mission[i]);
                        TaskData.Instance.m_iCurMissionIndex = m_mission[i];
                        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        tempTaskData.m_iCurIndex = m_mission[i + 1];
                        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                }
            }
			else
			{
//				for (int i = 0; i < m_MainCityUI.m_WindowObjectList.Count; i ++)
//				{
//					Debug.Log(m_MainCityUI.m_WindowObjectList[i].gameObject.name);
//				}
			}
		}
		
		if (m_MainCityUiDelegate != null)
		{
			m_MainCityUiDelegate();
			m_MainCityUiDelegate = null;
		}
	}
	
	public void setInitButtons()
    {
        m_MainCityListButton_L = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_L", 0);
        m_MainCityListButton_RT = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_RT", 1);
        m_MainCityListButton_RB = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_RB", 2);
        m_MainCityListButton_R = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_R", 3);
		m_MainCityListButton_LT = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_LT", 4);
		m_MainCityListButton_RTRT = new MainCityListButtonManager(m_ObjMainCityButtonS, "ButtonS_RTRT", 5);
        m_listMainCityButtons.Add(m_MainCityListButton_L);
        m_listMainCityButtons.Add(m_MainCityListButton_RT);
        m_listMainCityButtons.Add(m_MainCityListButton_RB);
        m_listMainCityButtons.Add(m_MainCityListButton_R);
		m_listMainCityButtons.Add(m_MainCityListButton_LT);
		m_listMainCityButtons.Add(m_MainCityListButton_RTRT);
        for (int i = 0; i < m_listMainCityButtons.Count; i++)
        {
            for (int q = 0; q < FunctionOpenTemp.m_EnableFuncIDList.Count; q++)
            {
                FunctionOpenTemp openTemp = FunctionOpenTemp.GetTemplateById(FunctionOpenTemp.m_EnableFuncIDList[q]);

				if( m_listMainCityButtons[i] == null ){
//					Debug.Log( "Error, button i is null: " + i );

					continue;
				}

				if( openTemp == null ){
//					Debug.Log( "Error, FunctionOpen is null: " + openTemp );

					continue;
				}

                if (m_listMainCityButtons[i].m_iState == openTemp.type && openTemp.m_iPlay == 1 && openTemp.m_parent_menu_id == -1)
                {
                    if (openTemp.m_iID == 17)
                    {
                        var temp = FunctionUnlock.templates.Where(item => !FunctionOpenTemp.m_EnableFuncIDList.Contains(item.id));
                        if (temp.Any())
                        {
                            m_listMainCityButtons[i].addButton(openTemp);
                        }
                    }
//                    else 
					if (openTemp.m_iID == 15)
                    {
                        if (LimitActivityData.Instance.IsOpenZaixianActivity)
                        {
                            m_listMainCityButtons[i].addButton(openTemp);
                        }
                    }
                    else if (openTemp.m_iID == 16)
                    {
                        if (LimitActivityData.Instance.IsOpenQiriActivity)
                        {
                            m_listMainCityButtons[i].addButton(openTemp);
                        }
                    }
                    else
                    {
                        m_listMainCityButtons[i].addButton(openTemp);
                    }
                }
            }
            m_listMainCityButtons[i].sortButtonS();
            m_listMainCityButtons[i].setPos();
            m_listMainCityButtons[i].setEndPos();
        }
		m_spriteBBG.SetDimensions(m_listMainCityButtons[2].m_listFunctionButtonManager.Count * 75 + 35, 52);
    }

    public void AddButton(int id)
    {
        if (getButton(id) == null)
        {
            FunctionOpenTemp openTemp = FunctionOpenTemp.GetTemplateById(id);
            for (int i = 0; i < m_listMainCityButtons.Count; i++)
            {
                if (m_listMainCityButtons[i].m_iState == openTemp.type && openTemp.m_iPlay == 1 && openTemp.m_parent_menu_id == -1)
                {
                    m_listMainCityButtons[i].addButton(openTemp);
                    m_listMainCityButtons[i].sortButtonS();
                    m_listMainCityButtons[i].setPos();
                    m_listMainCityButtons[i].setEndPos();
                    m_listMainCityButtons[i].m_listFunctionButtonManager[m_listMainCityButtons[i].m_listFunctionButtonManager.Count - 1].Teshu();
                    break;
                }
            }
			if(openTemp.m_parent_menu_id == -1)
			{
				m_spriteBBG.SetDimensions(m_listMainCityButtons[2].m_listFunctionButtonManager.Count * 75 + 35, 52);
			}
        }
    }

    public void AddButton(FunctionButtonManager functionButtonManager, FunctionOpenTemp functionOpenTemp)
    {
		if(getButton(functionButtonManager.m_index) != null)
		{
			return;
		}
        int tempID = FunctionOpenTemp.GetParent(functionOpenTemp);
        int tempType = 0;
        if (tempID == -1)
        {
            tempType = functionOpenTemp.type;
			Debug.Log(tempType);
        }
        else
        {
            tempType = FunctionOpenTemp.GetTemplateById(tempID).type;
        }
        m_listMainCityButtons[tempType].addButton(functionButtonManager);
        if (tempID == -1)
        {
            m_listMainCityButtons[tempType].sortButtonS();
            m_listMainCityButtons[tempType].setPos();
        }
        else
        {
			for(int i = 0; i < m_listMainCityButtons[tempType].m_listFunctionButtonManager.Count; i ++)
			{
				m_listMainCityButtons[tempType].m_listFunctionButtonManager[i].setMoveDis();
			}
			if(tempID == 106)
			{
				functionButtonManager.m_iWantToX = 960 + ClientMain.m_iMoveX * 2 - 40;
				functionButtonManager.m_iWantToY = -(640 + ClientMain.m_iMoveY * 2 - 40);
			}
			else
			{
				FunctionButtonManager tempButtonManager = m_listMainCityButtons[tempType].getButtonManagerByID(tempID);
				functionButtonManager.m_iWantToX = (int)tempButtonManager.gameObject.transform.localPosition.x;
				functionButtonManager.m_iWantToY = (int)tempButtonManager.gameObject.transform.localPosition.y;
			}
			
			functionButtonManager.setMoveDis();
        }
        m_listMainCityButtons[tempType].setMove(overAnim);
		if(functionOpenTemp.m_parent_menu_id == -1)
		{
			m_spriteBBG.SetDimensions(m_listMainCityButtons[2].m_listFunctionButtonManager.Count * 75 + 35, 52);
		}
    }

    public void deleteMaincityUIButton(int id)
    {
        //		Debug.Log(id);
        for (int i = 0; i < m_listMainCityButtons.Count; i++)
        {
            if (m_listMainCityButtons[i].getButtonManagerByID(id) != null)
            {
                m_listMainCityButtons[i].reMoveButton(id);
                m_listMainCityButtons[i].setPos();
                m_listMainCityButtons[i].setEndPos();
                break;
            }
        }
		
    }

    public FunctionButtonManager getButton(int id)
    {
        for (int i = 0; i < m_listMainCityButtons.Count; i++)
        {
            if (m_listMainCityButtons[i].getButtonManagerByID(id) != null)
            {
                return m_listMainCityButtons[i].getButtonManagerByID(id);
            }
        }
        return null;
    }

    public static bool SetRedAlert(int id, bool isShow, bool p_manual_set = true)
    {
//		Debug.Log(id);
		if(id == 9)
		{
//			Debug.Log("isShow="+isShow);
		}
		if(id == 140 && isShow)
		{
//			Debug.Log(FunctionOpenTemp.IsHaveID(140));

			if(FunctionOpenTemp.IsHaveID(140))
			{
				if(!ClientMain.m_isOpenQianDao)
				{
					ClientMain.m_isOpenQianDao = true;
					ClientMain.addPopUP(4, 2, "1", null);
				}
			}
		}
		if(id == 106)
		{
			if(MainCityUI.m_MainCityUI != null)
			{
				MainCityUI.m_MainCityUI.m_objRichangRed.SetActive(isShow);
				FunctionOpenTemp.GetTemplateById(106).m_show_red_alert = isShow;
			}
			return true;
			//			Debug.Log(FunctionOpenTemp.IsHaveID(140));
		}
        FunctionOpenTemp temp = FunctionOpenTemp.GetTemplateById(id);
        if (temp == null)
        {
            return false;
        }
        else
        {
            temp.m_show_red_alert = isShow;
			if(!FunctionOpenTemp.IsHaveID(id))
			{
				if(id == 9)
				{
//					Debug.Log("==========="+1);
				}
				return false;
			}
            if(temp.m_parent_menu_id != -1)
            {
                temp = FunctionOpenTemp.GetTemplateById(temp.m_parent_menu_id);
                if (isShow)
                {
                    temp.m_show_red_alert = isShow;
					SetRedAlert(temp.m_iID, isShow);
                }
                else
                {
                    FunctionOpenTemp temp1;
                    bool tempShowRed = false;
                    for (int i = 0; i < temp.m_listNextID.Count; i++)
                    {
                        temp1 = FunctionOpenTemp.GetTemplateById(temp.m_listNextID[i]);

						if (temp1.m_show_red_alert && FunctionOpenTemp.IsHaveID(temp.m_listNextID[i]))
                        {
                            tempShowRed = true;
                            break;
                        }
                    }
                    if (!tempShowRed)
                    {
                        temp.m_show_red_alert = false;
                    }
					SetRedAlert(temp.m_iID, tempShowRed);
                }
            }
            if (MainCityUI.m_MainCityUI != null)
            {
				if(id == 9)
				{
//					Debug.Log("==========="+2);
				}
                if (temp.type >= 0 && temp.type <= 6 && MainCityUI.m_MainCityUI.m_listMainCityButtons.Count == 6)
                {
					if(id == 9)
					{
//						Debug.Log("==========="+3);
					}
                    FunctionButtonManager tempButton = MainCityUI.m_MainCityUI.m_listMainCityButtons[temp.type].getButtonManagerByID(id);
                    if (tempButton != null)
                    {
						if(id == 9)
						{
//							Debug.Log("==========="+4);
//							Debug.Log(tempButton.gameObject.name);
//							Debug.Log(temp.m_show_red_alert);
						}
                        tempButton.setRed(temp.m_show_red_alert);
                    }
                    else
                    {
						if(id == 9)
						{
//							Debug.Log("==========="+5);
						}
                        return false;
                    }
                }
            }
        }
        return true;
    }


	public static bool SetButtonNum(int id, int Num)
	{
		FunctionOpenTemp temp = FunctionOpenTemp.GetTemplateById(id);
		if (temp == null)
		{
			return false;
		}
		else
		{
			temp.m_iNum = Num;
			if(temp.m_iNum > 99)
			{
				temp.m_iNum = 99;
			}
			if (MainCityUI.m_MainCityUI != null)
			{
				if (temp.type >= 0 && temp.type <= 4 && MainCityUI.m_MainCityUI.m_listMainCityButtons.Count == 4)
				{
					FunctionButtonManager tempButton = MainCityUI.m_MainCityUI.m_listMainCityButtons[temp.type].getButtonManagerByID(id);
					if (tempButton != null)
					{
						tempButton.setNum(temp.m_iNum);
					}
					else
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static bool SetSuperRed(int id, bool isShow)
	{
		FunctionOpenTemp temp = FunctionOpenTemp.GetTemplateById(id);
		if (temp == null)
		{
			return false;
		}
		else
		{
			if (MainCityUI.m_MainCityUI != null)
			{
				if (temp.type >= 0 && temp.type <= 4 && MainCityUI.m_MainCityUI.m_listMainCityButtons.Count == 4)
				{
					FunctionButtonManager tempButton = MainCityUI.m_MainCityUI.m_listMainCityButtons[temp.type].getButtonManagerByID(id);
					if (tempButton != null)
					{
						tempButton.setSuperAlert(isShow);
					}
					else
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	
	public void overAnim()
	{
		ClientMain.closePopUp();
	}
	
	private bool isZero = false;
	private bool m_isInited = false;
	private int m_iCur = 0;
	void Update()
	{
//		Debug.Log(Global.m_sMainCityWantOpenPanel);
		if (Global.m_sMainCityWantOpenPanel != -1)
		{
			GameObject tempObj = new GameObject();
			tempObj.name = "MainCityUIButton_" + Global.m_sMainCityWantOpenPanel;
			m_isQiangzhiClick = true;
			MYClick(tempObj);
			Global.m_sMainCityWantOpenPanel = -1;
		}
		//		m_iCur ++;
		//		if(m_iCur == 200)
		//		{
		//			m_iCur = 0;
		//			ClientMain.m_UIAddZhanliManager.createText(JunZhuData.Instance().m_junzhuInfo.zhanLi - Global.m_iZhanli);
		//		}
		//if has open function index, open it now.
		//        if (Global.m_iOpenFunctionIndex != -1)
		//        {
		//			if(MainCityUIRB.ButtonSpriteNameTransferDic.ContainsKey(Global.m_iOpenFunctionIndex))
		//			{
		//				if (MainCityUI.m_MainCityUI.m_WindowListCount == 0)
		//				{
		//					if(UIYindao.m_UIYindao.m_isOpenYindao)
        //					{
        //						UIYindao.m_UIYindao.CloseUI();
        //						MainCityUIRB.EnableButton(Global.m_iOpenFunctionIndex);
        //					}
        //				}
        ////				Global.m_iOpenFunctionIndex = -1;
        //			}
        //        }

        if (m_WindowObjectList.Count >= 1)
        {
            isZero = false;
            m_isInited = false;
        }

        if (!isZero)
        {
            if (m_WindowObjectList.Count == 0 && !m_isInited)
            {
                isZero = true;
                //				Debug.Log(isZero);
            }
        }
        isFirstAnimation();

        if (m_WindowObjectList.Count >= 1 && UIYindao.m_UIYindao.m_isOpenYindao && CityGlobalData.m_isRightGuide)
        {
            m_isInited = false;
            CityGlobalData.m_isRightGuide = false;
            UIYindao.m_UIYindao.CloseUI();
        }
        else if (m_WindowObjectList.Count == 0 && !ClientMain.m_isNewOpenFunction)
        {
            //			Debug.Log(m_WindowObjectList.Count);
            if (isZero)
            {
                isZero = false;
                m_isInited = true;
                setInit();
                if (Global.m_sMainCityWantOpenPanel != -1)
                {
                    GameObject tempObj = new GameObject();
                    tempObj.name = "MainCityUIButton_" + Global.m_sMainCityWantOpenPanel;
					m_isQiangzhiClick = true;
                    MYClick(tempObj);
                    Global.m_sMainCityWantOpenPanel = -1;
					m_isQiangzhiClick = true;
                }
                else if (Global.m_isShowBianqiang)
                {
                    Global.m_isShowBianqiang = false;
                    MainCityUILT.ShowMainTipWindow();
                }
            }
        }
        for (int i = 0; i < m_listMainCityButtons.Count; i++)
        {
            m_listMainCityButtons[i].UpData();
        }
//		for(int i = 0; i < m_listShoujiData.Count; i ++)
//		{
//			ClientMain.m_UITextManager.createText(m_listShoujiData[i].m_sDrawString + m_listShoujiData[i].m_iCurNum + "/" + m_listShoujiData[i].m_iMaxNum);
//		}
//		m_listShoujiData = new List<ShoujiData>();
		if(UIShoujiManager.m_UIShoujiManager != null && UIShoujiManager.m_UIShoujiManager.m_isPlayShouji)
		{
			if(m_listShoujiData.Count > 0)
			{
				if(UIShoujiManager.m_UIShoujiManager.m_isPlay)
				{
					UIShoujiManager.m_UIShoujiManager.setData(m_listShoujiData[0]);
					m_listShoujiData.RemoveAt(0);
//					Debug.Log(UIShoujiManager.m_UIShoujiManager.m_isPlay);
				}
			}
		}
    }

    public override void MYClick(GameObject ui)
    {
        if (!m_isClick)
        {
			Debug.Log("=================2");
            return;
        }
//		Debug.Log(ui.name);
        //		LimitActivityData.Instance.RequestData();
        //		if(1==1)
        //		{
        //			return;
        //		}
        int tempIndex = 0;

        if (ui.name.IndexOf("MainCityUIButton_") != -1)
        {
            int id = int.Parse(ui.name.Substring(17, ui.name.Length - 17));
//			Debug.Log(id);
            if (Input.touchCount <= 1 && MainCityUIRB.IsCanClickButtons)
            {
				FunctionOpenTemp tempFunction = FunctionOpenTemp.GetTemplateById(id);
				if(tempFunction != null)
				{
					if (tempFunction.m_iNpcID <= 0)
					{
						TaskData.Instance.SendData(FunctionOpenTemp.GetTemplateById(id).m_iMissionOpenID, 1);
					}
					if(id != 106)
					{
//						if(tempFunction.type < 5 && getButton(id) == null)
//						{
//							return;
//						}
					}
				}

				//return if window opened.
				if (MainCityUI.IsWindowsExist() && !m_isQiangzhiClick)
                {
					for(int i = 0; i < MainCityUI.m_MainCityUI.m_WindowObjectList.Count; i ++)
					{
						Debug.Log(MainCityUI.m_MainCityUI.m_WindowObjectList[i].gameObject.name);
					}
                    return;
                }
				m_isQiangzhiClick = false;
//				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_FULI),
//				                        AddUIPanel);
//				return;
				GameObject objPanel = FunctionOpenTemp.GetTemplateById(id).m_objPanel;
				if(objPanel != null)
				{
					MainCityUI.TryRemoveFromObjectList(objPanel);
					GameObject.Destroy(objPanel);
				}
                switch (id)
                {
                    //setting up
                    //                case 2:
                    //                    {
                    //                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
                    //                                         SettingUpLoadCallback);
                    //                    }
                    //                    break;
                    //goto rank
                    case 210:
                        //Add rank sys here.
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.RANK_WINDOW),
                                                RankWindowLoadBack);
                        break;
                    //bag sys
                    case 3:
                        if (m_bagObject == null)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_BAG),
                                                BagLoadCallback);
                        }
                        else
                        {
                            m_bagObject.SetActive(true);
                            m_bagObject.GetComponentInChildren<ScaleEffectController>().OnOpenWindowClick();
                            MainCityUI.TryAddToObjectList(m_bagObject);

                            // Manual show UI
                            UI2DTool.Instance.AddTopUI(m_bagObject);
                        }
                        break;
                    //friend sys
                    case 4:
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FRIEND_OPERATION),
                                                FriendLoadCallback);
                        break;
                    //task sys
                    case 5:
                        TaskData.Instance.m_ShowType = 0;
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
                                                TaskLoadCallback);
                        break;
                    //treasure sys
                    case 6:
                        if (FreshGuide.Instance().IsActive(300110) && TaskData.Instance.m_TaskInfoDic[300110].progress >= 0)
                        {
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300110];
                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
                        }
                        else if (FreshGuide.Instance().IsActive(300210) && TaskData.Instance.m_TaskInfoDic[300210].progress >= 0)
                        {
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300210];
                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
                        }
                        else if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            CityGlobalData.m_isRightGuide = true;
                        }
//                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_SECRET),
//                                                TreasureLoadCallback);
					    MiBaoGlobleData.Instance().SendMiBaoIfoMessage(true);
                        break;
                    //go home
                    case 7:
                        DoGoHome();
                        break;
                    //Battle
                    case 8:
                        //					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(8))
                        //					{
                        WarPage.m_instance.OpenWarSelectWindow();
                        //						原来的
                        //						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FIGHT_TYPE_SELECT),
                        //						                        Battle_LoadCallBack);
                        //					}
                        //					else
                        //					{
                        //						FunctionWindowsCreateManagerment.ShowUnopen(8);
                        //					}
                        break;
                    //Pawnshop
                    case 9:
                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(9))
                        {
                            ShopData.Instance.OpenShop(ShopData.ShopType.ROLL);
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(9);
                        }
                        break;
                    //serach treasure
                    case 11:
                        if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
                            {
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                            else if (FreshGuide.Instance().IsActive(100150) && TaskData.Instance.m_TaskInfoDic[100150].progress >= 0)
                            {
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                            else
                            {
                                CityGlobalData.m_isRightGuide = true;
                            }
                        }
                        //                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TANBAO),
                        //                                                SerachTreasureLoadCallback);
                        TanBaoData.Instance.TanBaoInfoReq();
                        break;
                    //Recharge
                    //Equip
                    case 12:
//						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MAIN_CITY_YUNYING ), AddUIPanel);
//						Debug.Log(UIShouji.m_UIShouji);
//						Debug.Log(UIShouji.m_isPlayShouji);
//						Debug.Log(m_listShoujiData.Count);
//						Debug.Log(UIShouji.m_UIShouji.m_isPlay);
//						Debug.Log(Time.realtimeSinceStartup - UIShouji.m_UIShouji.playTime);
//						Debug.Log(UIShouji.m_UIShouji.playTime);
//						Debug.Log(Time.realtimeSinceStartup);

                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(12))
                        {
                            //						if (equipObject == null)
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.INTENSIFY_EQUIP_GROWTH_AMEND),
                                                        Equip_LoadCallback);
                            }
                            //						else
                            //						{
                            //							equipObject.SetActive(true);
                            //
                            //							MainCityUI.TryAddToObjectList( equipObject, false );

                            // Manual show UI
                            // REMOVED, add will be execute in Function UI
                            //							{
                            //								UI2DTool.Instance.AddTopUI(equipObject);
                            //							}
                            //						}
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(12);
                        }
                        break;
                    case 13:
					case 1300:
//						TopUpLoadManagerment.LoadPrefab(false);
						RechargeData.Instance.RechargeDataReq ();
                        break;
                    //activity sys
                    case 14:
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER) + "1", OnActivityLoadCallBack);
                        break;
                    //新手在线礼包
                    case 15:
                        CityGlobalData.m_Limite_Activity_Type = 1542000;

                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_TIMES),
                                                RewardCallback);
                        break;
                    //新手七日礼包
                    case 16:
                        CityGlobalData.m_Limite_Activity_Type = 1543000;
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_DAY),
                                                RewardCallback);
                        break;
                    //comming soon
                    case 17:
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_JIESUO),
                                                AddUIPanel);
                        break;
					//email
					case 22:
						NewEmailData.Instance ().OpenEmail (NewEmailData.EmailOpenType.EMAIL_MAIN_PAGE);
						break;
                    //alliance sys
                    case 104:
                        //                        AllianceData.Instance.RequestData();
                        //                        Debug.Log("JunZhuData.Instance().m_junzhuInfo.lianMengIdJunZhuData.Instance().m_junzhuInfo.lianMengId ::" + JunZhuData.Instance().m_junzhuInfo.lianMengId);
                        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
                                                    NoAllianceLoadCallback);
                        }
                        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT),
                                                    AllianceHaveLoadCallback);
                        }
                        break;
                    case 106:
                        TaskData.Instance.m_ShowType = 2;
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_LAYER_AMEND_EBERYDAY),
                                                TaskLoadCallback);
                        break;
                    case 107:
                        TaskData.Instance.m_ShowType = 1;
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
                                                TaskLoadCallback);
                        break;
                    case 139://福利
                             //UI_PANEL_FULI正确的路径

                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_FULI),
                                                AddUIPanel);
                        break;
					case 140:
					case 1400:
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SIGNAL_LAYER),
						                        MainCityUI.m_MainCityUI.AddUIPanel);
						break;
					case 141:
						PlayerSceneSyncManager.Instance.EnterTreasureCity ();
						break;
					case 142:
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER) + "1", OnActivityLoadCallBack);
						break;
					case 143:
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MAIN_CITY_YUNYING ), AddUIPanel);
						break;
					case 144:
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER),
						                        MainCityUI.m_MainCityUI.AddUIPanel);
						break;
                    //king
                    case 200:
                        {
                            //Add king here.
                            CityGlobalData.m_JunZhuTouXiangGuide = false;

                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_LAYER_AMEND),
                                                    JunzhuLayerLoadCallback);
                            break;
                        }
                    //LueDuo
                    case 211:
                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(211))
                        {
                            //Add luo duo here.
                            //						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.LUEDUO), LueDuo_LoadCallBack);
                            PlunderData.Instance.OpenPlunder();
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(211);
                        }
                        break;
                    //Nation
                    case 212:
                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(212))
                        {
                            //Add carriage here.
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.NATION_ROOT), Nation_LoadCallback);
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(212);
                        }
                        break;
                    //YouXia
                    case 250:
                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(300))
                        {
                            //Add ranger here.
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YOUXIA), YouXia_LoadCallBack);
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(300);
                        }
                        break;
                    //Carriage
                    case 310:
                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(310))
                        {
                            PlayerSceneSyncManager.Instance.EnterCarriage();
                        }
                        else
                        {
                            FunctionWindowsCreateManagerment.ShowUnopen(310);
                        }
                        break;
					//运镖福利按钮
					case 311:
                        {
							if(!FunctionOpenTemp.IsHaveID(310))
							{
								ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(310).m_sNotOpenTips);
							}
                            else
							{
								PlayerSceneSyncManager.Instance.EnterCarriage();
							}
                            break;
                        }
                    //house operation
                    case 2000:
                        IHouseSelf temp = FindObjectOfType<SmallHouseSelf>();
                        temp = temp ?? FindObjectOfType<BigHouseSelf>();
                        if (temp != null)
                        {
                            temp.OnOperationClick();
                        }
                        break;
                    //house self exit
                    case 2001:
                        //house other exit
                        break;
                    case 2002:
                        HouseBasic temp1 = FindObjectOfType<SmallHouseSelf>();
                        temp1 = temp1 ?? FindObjectOfType<BigHouseSelf>();
                        temp1 = temp1 ?? FindObjectOfType<SmallHouseOther>();
                        temp1 = temp1 ?? FindObjectOfType<BigHouseOther>();
                        if (temp1 != null)
                        {
                            temp1.OnExitClick();
                            break;
                        }
                        Debug.LogError("No any house prefab found, exit house fail.");
                        break;
					case 1420:
						Global.CreateFunctionIcon(301);
						break;
					case 1421:
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
						                        SettingUpLoadCallback);
						break;
	                    //					Worship
	                    case 400000:
	                        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(400000))
	                        {
	                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
	                                                    Worship_LoadCallback);
	                        }
	                        else
	                        {
	                            FunctionWindowsCreateManagerment.ShowUnopen(400000);
	                        }
	                        break;
                    case 300000:
                        if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300000).Level)
                        {
                            EnterGuoGuanmap.EnterPveUI(-1);
                        }
                        break;
					case 600100:
						MainCityUILT.ShowMainTipWindow();
						break;
                    case 300100:
                        if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300100).Level)
                        {
//                            PvpData.Instance.OpenPvp();
							SportData.Instance.OpenSport ();
                        }
                        break;
			        case 900001: //买铜币 900001
					case 2101:
			            {
			                //Add king here.
			
			                JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
			
			                break;
			            }
			        case 900002: //买体力  900002
						JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
						break;
					case 2100:
			            {
			                //Add king here.
			                JunZhuData.Instance().BuyTiliAndTongBi(true, false, false, true);
			                break;
			            }
						case 500010: //符石
						{
							//Add king here.
							
					       MiBaoGlobleData.Instance().GetFuwenInit();
//					        Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.FUWEN),Load111ResourceCallback);
							break;
						}
                    default:
                        //Skip empty button.
                        if (id < 0)
                        {
                            return;
                        }
                        break;
                }
            }
        }
        else if (ui.name.IndexOf("TongzhiIcon") != -1)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TONGZHI),
                                    AddUIPanel);
        }

        else
        {
            if (ui.name.IndexOf("LT_") != -1)
            {
                tempIndex = 0;
            }
            else if (ui.name.IndexOf("RT_") != -1)
            {
                tempIndex = 1;
            }
            else if (ui.name.IndexOf("RB_") != -1)
            {
                tempIndex = 3;
            }
            else if (ui.name.IndexOf("L_") != -1)
            {
                tempIndex = 4;
            }
            m_MYNGUIPanel[tempIndex].MYClick(ui);
        }
    }

    private GameObject baizhanRoot;
    public void BaiZhanLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (baizhanRoot)
        {
            return;
        }
        baizhanRoot = Instantiate(p_object) as GameObject;

        baizhanRoot.SetActive(true);

        baizhanRoot.name = "BaiZhan";

        baizhanRoot.transform.localPosition = new Vector3(0, 800, 0);

        baizhanRoot.transform.localScale = Vector3.one;

        MainCityUI.TryAddToObjectList(baizhanRoot);
    }
    public override void MYMouseOver(GameObject ui)
    {
    }

    public override void MYMouseOut(GameObject ui)
    {
    }

    public override void MYPress(bool isPress, GameObject ui)
    {
		if(ui.name.IndexOf("Icon900003") != -1 && isPress)
		{
			ShowTip.showTip(900003);
		}
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

    public void LoadHY_Map(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject HY_Map = Instantiate(p_object) as GameObject;
        //WildnessManager mWildnessManager = mobg.GetComponent<WildnessManager>();
        HY_Map.transform.localPosition = new Vector3(-200, 200, 0);
        HY_Map.transform.localScale = Vector3.one;
        MainCityUI.TryAddToObjectList(HY_Map);
        HY_UIManager mWildnessManager = HY_Map.GetComponent<HY_UIManager>();
        mWildnessManager.init();
    }

    private void EnterCarriageScene(Vector2 p_position)
    {
        PlayerSceneSyncManager.Instance.EnterCarriage(p_position.x, p_position.y);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message == null)
        {
            return false;
        }
        switch (p_message.m_protocol_index)
        {
        case ProtoIndexes.S_TICHU_YBHELPXZ_RESP:
        {
            object tiChuXieZhuRsq = new TiChuXieZhuResp();
            if (SocketHelper.ReceiveQXMessage(ref tiChuXieZhuRsq, p_message, ProtoIndexes.S_TICHU_YBHELPXZ_RESP))
            {
                TiChuXieZhuResp temp = tiChuXieZhuRsq as TiChuXieZhuResp;

                Global.CreateBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                    null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_10).Replace("***", temp.name),
                    null,
                    LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                    null);
                return true;
            }
            return false;
        }
        case ProtoIndexes.S_MengYouKuaiBao_Resq:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			PromptMSGResp tempInfo = new PromptMSGResp();
			
			t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
			
			//			Debug.Log("=============2");
			List<TongzhiData> tempListTongzhiData = new List<TongzhiData>();
			if (tempInfo.msgList != null)
			{
				for (int i = 0; i < tempInfo.msgList.Count; i++)
				{
					TongzhiData tempTongzhiData = new TongzhiData(tempInfo.msgList[i]);
					tempListTongzhiData.Add(tempTongzhiData);
				}
			}
			
			//			SuBaoMSG tempsubao = new SuBaoMSG();
			//			tempsubao.subaoId = 10000;
			//			tempsubao.subao = "aaaaa";
			//			tempsubao.configId = 6;
			//			TongzhiData tempTongzhiData11 = new TongzhiData(tempsubao);
			//			tempListTongzhiData.Add(tempTongzhiData11);
			//			tempsubao = new SuBaoMSG();
			//			tempsubao.subaoId = 10001;
			//			tempsubao.subao = "bbbb";
			//			tempsubao.configId = 7;
			//			tempTongzhiData11 = new TongzhiData(tempsubao);
			//			tempListTongzhiData.Add(tempTongzhiData11);
			//			tempsubao = new SuBaoMSG();
			//			tempsubao.subaoId = 10002;
			//			tempsubao.subao = "ccccc";
			//			tempsubao.configId = 32;
			//			tempTongzhiData11 = new TongzhiData(tempsubao);
			//			tempListTongzhiData.Add(tempTongzhiData11);
			//			tempsubao = new SuBaoMSG();
			//			tempsubao.subaoId = 10003;
			//			tempsubao.subao = "ddddd";
			//			tempsubao.configId = 33;
			//			tempTongzhiData11 = new TongzhiData(tempsubao);
			//			tempListTongzhiData.Add(tempTongzhiData11);
			//			tempsubao = new SuBaoMSG();
			//			tempsubao.subaoId = 10004;
			//			tempsubao.subao = "eeeeee";
			//			tempsubao.configId = 2;
			//			tempTongzhiData11 = new TongzhiData(tempsubao);
			//			tempListTongzhiData.Add(tempTongzhiData11);
			//			Global.m_listMainCityData[i].m_SuBaoMSG
			//			Debug.Log("=============3");
			Global.upDataTongzhiData(tempListTongzhiData);
			m_MainCityUITongzhi.upDataShow();
			//			ReportTemplate temp = ReportTemplate.GetHeroSkillUpByID();
			//			Global.NextCutting();
			return true;
		}

        
        case ProtoIndexes.S_MengYouKuaiBao_PUSH:
		{
			MemoryStream t_stream1 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx1 = new QiXiongSerializer();
			
			SuBaoMSG tempInfo1 = new SuBaoMSG();
			
			t_qx1.Deserialize(t_stream1, tempInfo1, tempInfo1.GetType());
			
			TongzhiData tempTongzhiData1 = new TongzhiData(tempInfo1);
			
			List<TongzhiData> tempListTongzhiData1 = new List<TongzhiData>();
			
			tempListTongzhiData1.Add(tempTongzhiData1);
			
			Global.upDataTongzhiData(tempListTongzhiData1);
			m_MainCityUITongzhi.upDataShow();
			break;
		}
        case ProtoIndexes.S_Prompt_Action_Resp:
        {
            MemoryStream stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
            QiXiongSerializer xiongSerializer = new QiXiongSerializer();
            PromptActionResp msg = new PromptActionResp();
            xiongSerializer.Deserialize(stream, msg, msg.GetType());

                    if (msg.result != 10)
                    {
//                        ClientMain.m_UITextManager.createText("通知已过时");

                        return true;
                    }

                    switch (msg.subaoType)
            {
                //transport to position.
                case 101:
                case 102:
                case 104:
                case 105:
                {
                    m_TpController.m_ExecuteAfterTP = EnterCarriageScene;
                        m_TpController.TPToPosition(new Vector2(msg.posX, msg.posZ), float.Parse(YunBiaoTemplate.GetValueByKey("TP_duration")));
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
			if(msg.fujian == null || msg.fujian == "")
			{
				break;
			}

			string tempFujian = msg.fujian;
			List<string> funjianlist = new List<string>();
			
			while(tempFujian.IndexOf("#") != -1)
			{
				funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
			}
			funjianlist.Add(Global.NextCutting(ref tempFujian, "#"));
			List<RewardData> RewardDataList = new List<RewardData>();
			for(int i = 0; i < funjianlist.Count; i ++)
			{
				tempFujian = funjianlist[i];
				Global.NextCutting(ref tempFujian, ":");
				RewardData Rewarddata = new RewardData ( int.Parse(Global.NextCutting(ref tempFujian, ":")), int.Parse(Global.NextCutting(ref tempFujian, ":"))); 
				RewardDataList.Add(Rewarddata);
			}
			GeneralRewardManager.Instance().CreateReward (RewardDataList);
            break;
        }
        case ProtoIndexes.S_USE_ITEM:
        {
            ExploreResp tbGetRewardRes = new ExploreResp();
            tbGetRewardRes = QXComData.ReceiveQxProtoMessage(p_message, tbGetRewardRes) as ExploreResp;

            List<RewardData> dataList = new List<RewardData>();
			if(tbGetRewardRes.awardsList != null)
			{
				for (int m = 0; m < tbGetRewardRes.awardsList.Count; m++)
				{
					RewardData data = new RewardData(tbGetRewardRes.awardsList[m].itemId, tbGetRewardRes.awardsList[m].itemNumber);
					dataList.Add(data);
				}
				GeneralRewardManager.Instance().CreateReward(dataList);
			}
            break;
        }
		case ProtoIndexes.S_FULIINFO_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			QiXiongSerializer t_qx = new QiXiongSerializer();
			FuLiHuoDongResp tempInfo = new FuLiHuoDongResp();
			t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

			FunctionButtonManager tempButton = getButton(139);
			if(tempButton == null)
			{
				return false;
			}
			m_FuLiHuoDongResp = tempInfo;
			int time = 100000;
			for(int i = 0; i < m_FuLiHuoDongResp.xianshi.Count; i ++)
			{
				if(m_FuLiHuoDongResp.xianshi[i].isCanGet)
				{
					TimeLabelHelper.Instance.setTimeLabel(tempButton.m_LabelTime, ColorTool.Color_Green_00ff00 + "领取[-]", -1);
					return true;
				}
				else
				{
					if(m_FuLiHuoDongResp.xianshi[i].remainTime < time)
					{
						time = m_FuLiHuoDongResp.xianshi[i].remainTime;
					}
				}
			}
			TimeLabelHelper.Instance.setTimeLabel(tempButton.m_LabelTime, ColorTool.Color_Green_00ff00 + "领取[-]", time);
//			if(tempButton == null)
//			{
//				m_FuLiHuoDongResp = tempInfo;
//			}
//			else
//			{
//
//			}
			return true;
		}
		case ProtoIndexes.LOOK_APPLICANTS_RESP://查看申请入盟成员请求返回
		{
			//	Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
			MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer application_qx = new QiXiongSerializer();
			
			LookApplicantsResp applicateResp = new LookApplicantsResp();
			
			application_qx.Deserialize(application_stream, applicateResp, applicateResp.GetType());
			
			if (applicateResp != null)
			{
				if (applicateResp.applicanInfo == null || applicateResp.applicanInfo.Count == 0)
				{
					MainCityUI.SetRedAlert(410000, false);
				}
				else
				{
					MainCityUI.SetRedAlert(410000, true);
				}
			}
			return true;
		}
		case ProtoIndexes.PVE_MAX_ID_RESP:
		{
			MemoryStream t_stream = new MemoryStream (p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer ();
			
			GuanQiaMaxId tempInfo = new GuanQiaMaxId ();
			
			t_qx.Deserialize (t_stream, tempInfo, tempInfo.GetType ());
			
			MapCurrentInfo = tempInfo;

//			Debug.Log("================123");

			//				Debug.Log("MapCurrentInfo.chuanQiId  = " +MapCurrentInfo.chuanQiId );
			return true;
		}
        }
        return false;
    }

	private GameObject QCL_tempOjbect;
	void Load_QCL_ResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		if(QCL_tempOjbect == null)
		{
			QCL_tempOjbect = Instantiate(p_object )as GameObject;
			
			QCL_tempOjbect.transform.localScale = Vector3.one;
			
			QCL_tempOjbect.transform.localPosition = new Vector3 (100,100,0);
			
			MainCityUI.TryAddToObjectList(QCL_tempOjbect);
		}		
	}

    public bool isFirstAnimation()
    {

		if (CityGlobalData.QCLISOPen)
		{
		
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.QIANCHONGLOU), Load_QCL_ResourceCallback);
			//			return true;
		}

		if (Global.m_isOpenBaiZhan)
		{
			SportData.Instance.OpenSport ();
			Global.m_isOpenBaiZhan = false;
		}

		if (Global.m_isCityWarOpen)
		{
			Global.m_isCityWarOpen = false;
			CityWarData.Instance.OpenCityWar (CityWarData.CityWarType.NORMAL_CITY);
		}

        if (Global.m_isOpenPVP)
        {
            if (CityGlobalData.m_tempSection <= 0)
            {
                CityGlobalData.m_tempSection = -1;
            }
            EnterGuoGuanmap.EnterPveUI(CityGlobalData.m_tempSection);
            //			return true;
        }
        if (!ClientMain.m_isNewOpenFunction)
        {
            for (int i = 0; i < ClientMain.m_listPopUpData.Count; i++)
            {

                if (CityGlobalData.m_isBattleField_V4_2D)
                {
                    if (ClientMain.m_listPopUpData[i].iType != 0)
                    {
                        return true;
                    }
                }
                if (m_MainCityUI.m_WindowObjectList.Count != 0)
                {
                    if (ClientMain.m_listPopUpData[i].iType == 2)
                    {
                        continue;
                    }
                }
                switch (ClientMain.m_listPopUpData[i].iLevel)
                {
					case 1:
					{
						if(SignalInManagerment.m_SignalIn)
						{
							SignalInManagerment.m_SignalIn.VipEffect();
							ClientMain.m_isNewOpenFunction = true;
							ClientMain.m_listPopUpData.RemoveAt(i);
							return true;
						}
					}
						break;
                    case 2:
                        CityGlobalData.m_Limite_Activity_Type = 1543000;

                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_DAY),
                                                RewardCallback);
                        ClientMain.m_isNewOpenFunction = true;
                        ClientMain.m_listPopUpData.RemoveAt(i);
                        return true;

                    case 5:
                        //					Debug.Log(TaskData.Instance.ShowMainTaskGet(ClientMain.m_listPopUpData[i].sData));
                        if (TaskData.Instance.ShowMainTaskGet(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
                        break;
                   
                    case 10:
                        if (MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.setChange(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
                        break;
                    case 20:
                        if (JunZhuData.Instance().ShowLevelUp(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
                        break;
                    case 40:
						int tempFunctionID = int.Parse(ClientMain.m_listPopUpData[i].sData);
                        if (MainCityUI.m_MainCityUI.openAddFunction(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
							if(UIYindao.m_UIYindao.m_isOpenYindao)
							{
								UIYindao.m_UIYindao.CloseUI();
							}
                            return true;
                        }
                        break;
					case 4:
						ClientMain.m_isNewOpenFunction = true;
						ClientMain.m_listPopUpData.RemoveAt(i);
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SIGNAL_LAYER),
						                        MainCityUI.m_MainCityUI.AddUIPanel);
						break;
                    case 70:
                        {
							if(!Global.m_isZhanli)
							{
								Global.m_isZhanli = true;
								Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_ADDZHANLI),
								                        AddUIPanelNotCloseAndNotCloseYindao);
								ClientMain.m_isNewOpenFunction = true;
								ClientMain.m_listPopUpData.RemoveAt(i);
								return true;
							}
							else
							{
								return false;
							}
                        }
                        break;
					case 80:
					//Debug.Log(PassLevelAward.mStatePassLevelAward.ComLingQU());
						
					        PassLevelAward.mStatePassLevelAward.ComLingQU();
							ClientMain.m_isNewOpenFunction = true;
							ClientMain.m_listPopUpData.RemoveAt(i);
						break;
                }
            }
        }

        //		else if (Global.m_iOpenFunctionIndex != -1)
        //		{
        //			MainCityUIRB.EnableButton(Global.m_iOpenFunctionIndex);
        //			
        //			Global.m_iOpenFunctionIndex = -1;
        //			m_isNews = true;
        //			return true;
        //		}

        //		else
        //		{
        //			Debug.Log("==========1");
        //			Global.m_iZhanli = 6110;
        //			isFirstAnimation();
        //			m_isNews = true;
        //		}
        return false;
    }

    public bool openAddFunction(string data)
    {
        m_AddFunction = GameObject.Instantiate(Resources.Load("_UIs/MainCity/AddFunctionEff")) as GameObject;
        MainCityAddFunction m_Temp = m_AddFunction.GetComponentInChildren<MainCityAddFunction>();
        m_Temp.set(data);
        //        MainCityUI.TryAddToObjectList(m_AddFunction);
        if (!MainCityUIRB.isOpen)
        {
            GameObject ClickObj = new GameObject();
            ClickObj.name = "RB_OpenButton";
            MYClick(ClickObj);
        }
        return true;
    }

    public void AddButton(FunctionOpenTemp functionTemp)
    {

    }

    public void RankWindowLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject temp = (GameObject)Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(temp);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    public void BagLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        m_bagObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(m_bagObject);
        m_bagObject.transform.position = new Vector3(0, 500, 0);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }
    public void FriendLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
        UIYindao.m_UIYindao.CloseUI();
    }
    public void TaskLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }
    public void TreasureLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject Secrtgb = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(Secrtgb);
    }
    public void NoAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject Secrtgb = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(Secrtgb);
    }
	private GameObject expiectuiltempObject; // 联盟跳转到联盟做个特殊处理 直接删掉原来的UI才重新打开 fix by guronglin
    public void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
		if( expiectuiltempObject )
		{
			Destroy(expiectuiltempObject);
		}
		expiectuiltempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(expiectuiltempObject);

    }
    public void RewardCallback(ref WWW p_www, string p_path, Object p_object)
    {
        ClientMain.m_isOpenQIRI = true;
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject, false);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void JunzhuLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
		if(!MainCityUI.TryAddToObjectList(tempObject))
		{
			Destroy(tempObject);
		}
    }

    public void AddUIPanel(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void AddUIPanelNotCloseAndNotCloseYindao(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
    }

    public void AddUIPanelNotClose(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        UIYindao.m_UIYindao.CloseUI();
    }

    private void OnActivityLoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void DoGoHome()
    {
        //  Debug.Log("7777777777777777777777777777777777777");
        foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
        {
            if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId > 50)
            {
                //   Debug.Log("7777777777777777777777777777777777777");
                BigHouseId = item.Value.locationId;
                break;
            }
        }
        if (BigHouseId > 0)
        {
            //  Debug.Log("7777777777777777777777777777777777777");
            CityGlobalData.m_isNavToHouse = true;
            NpcManager.m_NpcManager.setGoToNpc(BigHouseId + 1000);
        }
        else
        {
            // Debug.Log("7777777777777777777777777777777777777");
            CityGlobalData.m_isNavToHome = true;
            foreach (KeyValuePair<int, HouseSimpleInfo> item in TenementData.Instance.m_AllianceCityTenementDic)
            {
                //    Debug.Log("7777777777777777777777777777777777777");
                if (item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id && item.Value.locationId <= 50)
                {
                    //     Debug.Log("7777777777777777777777777777777777777");
                    SmallHouseId = item.Value.locationId;
                    break;
                }
            }
            if (CityGlobalData.m_isAllianceTenentsScene)
            {
                //  Debug.Log("7777777777777777777777777777777777777");
                if (!NpcManager.m_NpcManager.m_npcObjectItemDic.ContainsKey(SmallHouseId + 1000))
                {
                    //        Debug.Log("7777777777777777777777777777777777777");
                    NpcManager.m_NpcManager.setGoToNpc(SmallHouseId + 1000);
                    CityGlobalData.m_isNavToAllianCityToTenement = true;
                }
                else
                {
                    //   Debug.Log("7777777777777777777777777777777777777");
                    //NpcManager.m_NpcManager.setGoToSelfTenement(SmallHouseId + 1000);
                }
            }
            else
            {
                //   Debug.Log("7777777777777777777777777777777777777");
                //NpcManager.m_NpcManager.setGoToTenementNpc(SmallHouseId + 1000);
            }
        }
    }

    public void LueDuo_LoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject LueDuoObj = GameObject.Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(LueDuoObj);
        LueDuoObj.name = "LueDuo";

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    public void Alliance_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);

        //  tempObject.transform.position = new Vector3(0, 500, 0);

        MainCityUI.TryAddToObjectList(tempObject);
    }

    public void Worship_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void Nation_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    private void YouXia_LoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    public void Battle_LoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject t_gb = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(t_gb);

        //		if (IsDoAdditionalOperation)
        //		{
        //			IsDoAdditionalOperation = false;
        //			m_Delegate(t_gb);
        //			m_Delegate = null;
        //		}
    }

    public void Equip_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        // add for new UI behaviour
        if (UI2DTool.ShowHidedUI(p_object))
        {
            // do init&operation here

            GameObject t_ui_gb = UI2DTool.GetCachedUIGameObject(p_object);

            {
                MainCityUI.TryAddToObjectList(t_ui_gb, false);
            }

            return;
        }

        equipObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(equipObject, false);

//		if (IsDoAdditionalOperation)
//		{
//			IsDoAdditionalOperation = false;
//			m_Delegate(equipObject);
//			m_Delegate = null;
//		}
    }

	public void SettingUpLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(tempObject);
		tempObject.transform.position = new Vector3(0, 500, 0);
		
		
		UIYindao.m_UIYindao.CloseUI();
	}

	public void addButtonTime(int id, int time)
	{
		if(getButton(id) == null)
		{
			AddButton(id);
			FunctionButtonManager temp = getButton(id);
			TimeLabelHelper.Instance.setTimeLabel(temp.m_LabelTime, "抢宝箱", time);
		}
	}

	public static void addShouji(int id, int type, int curNum, int maxNum, string drawString)
	{
		ShoujiData tempShoujiData = new ShoujiData(id, type, curNum, maxNum, drawString);
//		for(int i = 0; i < m_listShoujiData.Count; i ++)
//		{
//			if(m_listShoujiData[i].m_iID == id)
//			{
//				m_listShoujiData[i].setCurNum(curNum);
//				return;
//			}
//		}
		m_listShoujiData.Add(tempShoujiData);
	}
}
