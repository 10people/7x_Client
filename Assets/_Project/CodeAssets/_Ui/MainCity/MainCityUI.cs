using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

public class MainCityUI : MYNGUIPanel, SocketListener
{
    public delegate void MainCityUIDelegate();

    /// <summary>
    /// Execute this after init if it is not null
    /// </summary>
    public static MainCityUIDelegate m_MainCityUiDelegate;

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
	public GameObject m_MainGlobalBelongings;
	public GameObject m_ObjMainCityButtonS;

	public MainCityListButtonManager m_MainCityListButton_L;
	public MainCityListButtonManager m_MainCityListButton_RT;
	public MainCityListButtonManager m_MainCityListButton_RB;
	public MainCityListButtonManager m_MainCityListButton_R;
	public List<MainCityListButtonManager> m_listMainCityButtons = new List<MainCityListButtonManager>();
	public GameObject ButtonPrefab;
	private GameObject m_bagObject;
	private int BigHouseId = 0;
	private int SmallHouseId = 0;

    [HideInInspector]
    public List<MYNGUIPanel> m_MYNGUIPanel = new List<MYNGUIPanel>();

	public GameObject equipObject;
	public int a = 0;

    /// <summary>
	/// 1.access not allowed;
	/// 2.After negotiation, all objects contained here are only Functions' Main UI, not sub UI popped.
    /// </summary>
    private List<GameObject> m_WindowObjectList = new List<GameObject>();

	void Awake()
	{
		m_MainCityUI = this;
		m_MYNGUIPanel.Add(m_MainCityUILT);
		m_MYNGUIPanel.Add(m_MainCityUIRT);
		m_MYNGUIPanel.Add(null);
		m_MYNGUIPanel.Add(m_MainCityUIRB);
		m_MYNGUIPanel.Add(m_MainCityUIL);
		SocketTool.RegisterSocketListener(this);
	}

	void Start()
	{
		ClientMain.m_isNewOpenFunction = false;
		//        m_MainCityUIRB.Initialize();
		if (Global.m_isOpenJiaoxue)
		{
			//Debug.Log ("m_gameObjectList.Count = " + m_gameObjectList.Count);
			if (m_WindowObjectList.Count != 0)
			{
				return;
			}
			
			if (Global.m_isOpenBaiZhan)
			{
				//                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_PAGE),
				//                                        BaiZhanLoadCallback);
				PvpData.Instance.OpenPvp ();
				return;
			}
			
			if (Global.m_isOpenHuangYe)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_MAP),
				                        LoadHY_Map);
				return;
			}
		}
		
		
		
		
		
		m_isInited = true;
		setInit();
		setInitButtons();
		
		Global.ScendID(ProtoIndexes.C_ADD_TILI_INTERVAL, 1);
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_Belongings), OnLoadCallBack);


		//		Global.m_isZhanli = true;
//		ClientMain.addPopUP(40, 2, "6", null);
//		ClientMain.addPopUP(40, 2, "12", null);
//		ClientMain.addPopUP(40, 2, "1210", null);
//		m_MainCityUIRB.setPropUse(103103,30);
//		m_MainCityUIRB.setPropUse(103104,20);
//		m_MainCityUIRB.setPropUse(103105,0);
	}

    public static bool IsWindowsExist()
    {
        if (MainCityUI.m_MainCityUI == null) return false;

        //Clear object list.
        for (int i = 0; i < MainCityUI.m_MainCityUI.m_WindowObjectList.Count; i++)
        {
            if (MainCityUI.m_MainCityUI.m_WindowObjectList[i] == null || !MainCityUI.m_MainCityUI.m_WindowObjectList[i].activeInHierarchy)
            {
                MainCityUI.m_MainCityUI.m_WindowObjectList.RemoveAt(i);
            }
        }

        return MainCityUI.m_MainCityUI.m_WindowObjectList.Count > 0;
    }

    public static void TryAddToObjectList(GameObject go,bool p_add_to_2d_tool = true )
    {
        if (m_MainCityUI != null)
        {
            m_MainCityUI.m_WindowObjectList.Add(go);

			// assume all param:go is functionUI's main page, if not will cause an error
			if( p_add_to_2d_tool ){
				UI2DTool.Instance.AddTopUI( go );
			}
        }
    }

    public static void TryRemoveFromObjectList(GameObject go)
    {
        if (m_MainCityUI != null && m_MainCityUI.m_WindowObjectList.Contains(go))
        {
            m_MainCityUI.m_WindowObjectList.Remove(go);
        }
    }

	public static bool IsExitInObjectList (GameObject go)
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
		if(GUI.Button(new Rect(200,200,100,30), "运镖"))
		{
			temp = new GameObject();
			temp.name = "MainCityUIButton_310";
			MYClick(temp);
		}
		if(GUI.Button(new Rect(300,200,100,30), "主线任务"))
		{
			temp = new GameObject();
			temp.name = "MainCityUIButton_5";
			MYClick(temp);
		}
		if(GUI.Button(new Rect(400,200,100,30), "出征"))
		{
			temp = new GameObject();
			temp.name = "MainCityUIButton_8";
			MYClick(temp);
		}
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
    }

    
    public static bool IsShowFunctionOpenEffectInAllianceCity = false;

    void OnLevelWasLoaded()
    {
        if ( IsShowFunctionOpenEffectInAllianceCity 
		    && ( Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY ) 
		    		|| Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE 
		    		|| Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN 
		    || Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY_YE_WAN ) ) )
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
		m_MainGlobalBelongings = obj as GameObject;
	}
/// <summary>
///  通用资源显示
/// </summary>
/// <param name="parent">绑定在哪个OBJ下面</param>
/// <param name="x">坐标X</param>
/// <param name="y">坐标Y</param>
	public void setGlobalBelongings(GameObject parent, float x, float y)
	{
		GameObject tempObj = GameObject.Instantiate(m_MainGlobalBelongings);
		tempObj.transform.parent = parent.transform;
		tempObj.transform.localScale = Vector3.one;
		tempObj.transform.localPosition = new Vector3(x,y,0);
	}

    private int[] m_mission = new int[]
    {
        100010,0,100020,0,100030,0,100040,1,100050,0,100060,0,100070,0,
        100080,1,100090,0,100100,1,100110,0,100120,0,100130,0,100140,1,
        100150,0,100160,0,100170,0,100180,1,100190,0,100200,0,100210,0,
        100220,0,100230,0,100240,0,100250,1,100260,0,100270,0,100290,1,
        100300,0,100330,0,100340,0,100350,1,100360,0,100380,0,100400,0,
        100410,0
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
                        TaskData.Instance.m_iCurMissionIndex = m_mission[i];
                        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        tempTaskData.m_iCurIndex = m_mission[i + 1];
                        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                }
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
		m_listMainCityButtons.Add(m_MainCityListButton_L);
		m_listMainCityButtons.Add(m_MainCityListButton_RT);
		m_listMainCityButtons.Add(m_MainCityListButton_RB);
		m_listMainCityButtons.Add(m_MainCityListButton_R);
		for(int i = 0; i < m_listMainCityButtons.Count; i ++)
		{
			for(int q = 0; q < FunctionOpenTemp.m_EnableFuncIDList.Count; q ++)
			{
				FunctionOpenTemp openTemp = FunctionOpenTemp.GetTemplateById(FunctionOpenTemp.m_EnableFuncIDList[q]);

				if(m_listMainCityButtons[i].m_iState == openTemp.type && openTemp.m_iPlay == 1 && openTemp.m_parent_menu_id == -1)
				{
					m_listMainCityButtons[i].addButton(openTemp);
				}
			}
			m_listMainCityButtons[i].sortButtonS();
			m_listMainCityButtons[i].setPos();
			m_listMainCityButtons[i].setEndPos();
		}
	}

	public void AddButton(FunctionButtonManager functionButtonManager, FunctionOpenTemp functionOpenTemp)
	{
		int tempID = FunctionOpenTemp.GetParent(functionOpenTemp);
		int tempType = 0;
		if(tempID == -1)
		{
			tempType = functionOpenTemp.type;

		}
		else
		{
			tempType = FunctionOpenTemp.GetTemplateById(tempID).type;
		}
		m_listMainCityButtons[tempType].addButton(functionButtonManager);
		if(tempID == -1)
		{
			m_listMainCityButtons[tempType].sortButtonS();
			m_listMainCityButtons[tempType].setPos();
		}
		else
		{
			FunctionButtonManager tempButtonManager = m_listMainCityButtons[tempType].getButtonManagerByID(tempID);
			functionButtonManager.m_iWantToX = (int)tempButtonManager.gameObject.transform.localPosition.x;
			functionButtonManager.m_iWantToY = (int)tempButtonManager.gameObject.transform.localPosition.y;
			functionButtonManager.setMoveDis();
		}
		m_listMainCityButtons[tempType].setMove(overAnim);
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
            if (isZero)
            {
                isZero = false;
                m_isInited = true;
                setInit();
                if (Global.m_sMainCityWantOpenPanel != -1)
                {
					GameObject tempObj = new GameObject();
					tempObj.name = "MainCityUIButton_" + Global.m_sMainCityWantOpenPanel;
					MYClick(tempObj);
                    Global.m_sMainCityWantOpenPanel = -1;
                }
                else if (Global.m_isShowBianqiang)
                {
                    Global.m_isShowBianqiang = false;
                    MainCityUILT.ShowMainTipWindow();
                }
            }
        }
		for(int i = 0; i < m_listMainCityButtons.Count; i ++)
		{
			m_listMainCityButtons[i].UpData();
		}
    }

    public override void MYClick(GameObject ui)
    {
		Debug.Log(ui.name);

        if (!m_isClick)
        {
            return;
        }

        int tempIndex = 0;

		if(ui.name.IndexOf("MainCityUIButton_") != -1)
		{
			int id = int.Parse(ui.name.Substring(17, ui.name.Length - 17));
			if (Input.touchCount <= 1 && MainCityUIRB.IsCanClickButtons)
			{
				if (FunctionOpenTemp.GetTemplateById(id).m_iNpcID <= 0)
				{
					TaskData.Instance.SendData(FunctionOpenTemp.GetTemplateById(id).m_iMissionOpenID, 1);
				}
				
				//return if window opened.
				if (MainCityUI.IsWindowsExist())
				{
					return;
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
				{
					//Add rank sys here.
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.RANK_WINDOW),
					                        RankWindowLoadBack);
				}
					break;
					//Recharge
				case 13:
				{
					TopUpLoadManagerment.m_instance.LoadPrefab(true);
				}
					break;
					//bag sys
				case 3:
				{
					if (m_bagObject == null)
					{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_BAG),
						                        BagLoadCallback);
					}
					else
					{
						m_bagObject.SetActive(true);
						// Manual show UI
						{
							UI2DTool.Instance.AddTopUI( m_bagObject );
						}
					}
				}
					break;
					//friend sys
				case 4:
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FRIEND_OPERATION),
					                        FriendLoadCallback);
				}
					break;
					//serach treasure
				case 11:
				{
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
					TanBaoData.Instance.TanBaoInfoReq ();
				}
					break;
					//task sys
				case 5:
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TASK),
					                        TaskLoadCallback);
				}
					break;
					//treasure sys
				case 6:
				{
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
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_SECRET),
					                        TreasureLoadCallback);
				}
					break;
					//alliance sys
				case 104:
				{
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
				}
					break;
					//go home
				case 7:
				{
					DoGoHome();
					break;
				}
					//house operation
				case 2000:
				{
					IHouseSelf temp = FindObjectOfType<SmallHouseSelf>();
					temp = temp ?? FindObjectOfType<BigHouseSelf>();
					if (temp != null)
					{
						temp.OnOperationClick();
					}
					break;
				}
					//house self exit
				case 2001:
					//house other exit
				case 2002:
				{
					HouseBasic temp = FindObjectOfType<SmallHouseSelf>();
					temp = temp ?? FindObjectOfType<BigHouseSelf>();
					temp = temp ?? FindObjectOfType<SmallHouseOther>();
					temp = temp ?? FindObjectOfType<BigHouseOther>();
					if (temp != null)
					{
						temp.OnExitClick();
						break;
					}
					
					Debug.LogError("No any house prefab found, exit house fail.");
					break;
				}
					//activity sys
				case 14:
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER), OnActivityLoadCallBack);
				}
					break;
					//新手在线礼包
				case 15:
				{
					CityGlobalData.m_Limite_Activity_Type = 1542000;
					
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_ROOT),
					                        RewardCallback);
					
				}
					break;
					//新手七日礼包
				case 16:
				{
					CityGlobalData.m_Limite_Activity_Type = 1543000;
					
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_REWARD_ROOT),
					                        RewardCallback);
				}
					break;
					//comming soon
				case 17:
				{
					Instantiate(Resources.Load("_UIs/MainCity/WantOpenFunction"));
				}
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
					//Battle
				case 8:
				{
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(8))
					{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FIGHT_TYPE_SELECT),
						                        Battle_LoadCallBack);
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(8);
					}
				}
					break;
					//Equip
				case 12:
				{
					
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(12))
					{
						if (equipObject == null)
						{
							Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.INTENSIFY_EQUIP_GROWTH_AMEND),
							                        Equip_LoadCallback);
						}
						else
						{
							equipObject.SetActive(true);
							
							// Manual show UI
							{
								UI2DTool.Instance.AddTopUI(equipObject);
							}
						}
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(12);
					}
				}
					break;
					//Pawnshop
				case 9:
				{
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(9))
					{
						ShopData.Instance.OpenShop(ShopData.ShopType.ORDINARY);
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(9);
					}
				}
					break;
					//Worship
				case 400000:
				{
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(400000))
					{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
						                        Worship_LoadCallback);
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(400000);
					}
				}
					break;
					//YouXia
				case 300:
				{
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
				}
					//Carriage
				case 310:
				{
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(310))
					{
						PlayerSceneSyncManager.Instance.EnterCarriage();
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(310);
					}
					break;
				}
					//LueDuo
				case 211:
				{
					if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(211))
					{
						//Add luo duo here.
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.LUEDUO), LueDuo_LoadCallBack);
					}
					else
					{
						FunctionWindowsCreateManagerment.ShowUnopen(211);
					}
					break;
				}
					//Nation
				case 212:
				{
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
				}
				default:
				{
					//Skip empty button.
					if (id < 0)
					{
						return;
					}
					break;
				}
				}
			}
			
			switch (id)
			{
				//ironsmith
			case 12:
				//battle
			case 8:
				//store
			case 9:
				//worship
			case 400000:
				//ranger
			case 300:
				//carriage
			case 310:
				//luo duo
			case 211:
				//guo jia
			case 212:
			{
				if (FunctionOpenTemp.GetNpcIdByID(id) != -1)
				{
					NpcManager.m_NpcManager.setGoToNpc(FunctionOpenTemp.GetNpcIdByID(id));
				}
				break;
			}
			default:
			{
				//Skip empty button.
				if (id < 0)
				{
					return;
				}
				break;
			}
			}
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
        }
        return false;
    }

    public bool isFirstAnimation()
    {

        if (Global.m_isOpenPVP)
        {
            if (CityGlobalData.m_tempSection <= 0)
            {
                CityGlobalData.m_tempSection = -1;
            }
            EnterGuoGuanmap.EnterPveUI(CityGlobalData.m_tempSection);
            //			return true;
        }
        else if (Global.m_iZhanli != JunZhuData.Instance().m_junzhuInfo.zhanLi)
        {

            if (!Global.m_isZhanli)
            {
                //				Debug.Log(Global.m_iZhanli);
                //				Debug.Log(JunZhuData.Instance().m_junzhuInfo.zhanLi);

                if (Global.m_iZhanli == 0)
                {
                    Global.m_iZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
                }
                else
                {
                    Global.m_isZhanli = true;
                    ClientMain.addPopUP(70, 2, "", m_MainCityUILT.m_MainCityZhanliChange.setAnimation);
                    return true;
                }
            }
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
                    case 5:
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
                    case 30:

                        break;
                    case 40:
                        if (MainCityUI.m_MainCityUI.openAddFunction(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
                        break;
                    case 50:
                        if (MainCityUI.m_MainCityUI.m_MainCityUIRB.PlayAddButton(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
                        break;
                    case 70:
                        if (MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityZhanliChange.setAnimation(ClientMain.m_listPopUpData[i].sData))
                        {
                            ClientMain.m_isNewOpenFunction = true;
                            ClientMain.m_listPopUpData.RemoveAt(i);
                            return true;
                        }
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
		if(!MainCityUIRB.isOpen)
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
		UIYindao.m_UIYindao.CloseUI();
	}
	
	public void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
		
	}
	public void RewardCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(tempObject);
		UIYindao.m_UIYindao.CloseUI();
	}

	public void JunzhuLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
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
					NpcManager.m_NpcManager.setGoToSelfTenement(SmallHouseId + 1000);
				}
			}
			else
			{
				//   Debug.Log("7777777777777777777777777777777777777");
				NpcManager.m_NpcManager.setGoToTenementNpc(SmallHouseId + 1000);
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
		equipObject = (GameObject)Instantiate(p_object);
		MainCityUI.TryAddToObjectList(equipObject);
		
//		if (IsDoAdditionalOperation)
//		{
//			IsDoAdditionalOperation = false;
//			m_Delegate(equipObject);
//			m_Delegate = null;
//		}
	}
}
