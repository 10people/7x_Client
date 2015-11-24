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
    [HideInInspector]
    public List<MYNGUIPanel> m_MYNGUIPanel = new List<MYNGUIPanel>();

    /// <summary>
    /// access not allowed
    /// </summary>
    private List<GameObject> m_WindowObjectList = new List<GameObject>();

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

    public static void TryAddToObjectList(GameObject go)
    {
        if (m_MainCityUI != null)
        {
            m_MainCityUI.m_WindowObjectList.Add(go);
        }
    }

    public static void TryRemoveFromObjectList(GameObject go)
    {
        if (m_MainCityUI != null && m_MainCityUI.m_WindowObjectList.Contains(go))
        {
            m_MainCityUI.m_WindowObjectList.Remove(go);
        }
    }

#if UNITY_EDITOR

    void OnGUI()
    {
//        if (GUILayout.Button("DO"))
//        {
//            ClearObjectList();
//        }
    }

#endif

    /// <summary>
    /// Destroy all object from objectList and clear list.
    /// </summary>
    public static void ClearObjectList()
    {
        m_MainCityUI.m_WindowObjectList.ForEach(item => Destroy(item));
        m_MainCityUI.m_WindowObjectList.Clear();
    }

    public List<EventIndexHandle> m_listEvent = new List<EventIndexHandle>();

    void OnDestroy()
    {
        m_MainCityUI = null;

        SocketTool.UnRegisterSocketListener(this);
    }

    void Awake()
    {
        m_MainCityUI = this;
        m_MYNGUIPanel.Add(m_MainCityUILT);
        m_MYNGUIPanel.Add(null);
        m_MYNGUIPanel.Add(m_MainCityUILB);
        //m_MYNGUIPanel.Add(m_MainCityUIRB);

        SocketTool.RegisterSocketListener(this);

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

            ClientMain.addPopUP(40, 1, "" + 400000, null);
            ClientMain.addPopUP(50, 2, "" + 400000, null);
            ClientMain.addPopUP(40, 1, "" + 7, null);
            ClientMain.addPopUP(50, 2, "" + 7, null);
        }
    }

    void Start()
    {
        ClientMain.m_isNewOpenFunction = false;
        m_listEvent.ForEach(p => p.m_Handle += EventTouch);
        m_MainCityUIRB.Initialize();
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

        Global.ScendID(ProtoIndexes.C_ADD_TILI_INTERVAL, 1);

        //		Global.m_isZhanli = true;
        //		ClientMain.addPopUP(10, 0, "", null);
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
    private void EventTouch(int index)
    {
        switch (index)
        {
            //vip click
            case 0:
                TopUpLoadManagerment.m_instance.LoadPrefabSpecial(true, true);
                break;
            //buy money
            case 1:
                JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
                break;
            //recharge
            case 2:
                TopUpLoadManagerment.m_instance.LoadPrefab(true);
                break;
            //buy energy
            case 3:
                JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
                break;
            default:
                break;
        }

    }
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
                    MainCityUIRB.OutterButtonTrigger(Global.m_sMainCityWantOpenPanel);
                    Global.m_sMainCityWantOpenPanel = -1;
                }
                else if (Global.m_isShowBianqiang)
                {
                    Global.m_isShowBianqiang = false;
                    MainCityUILT.ShowMainTipWindow();
                }
            }
        }

    }

    public override void MYClick(GameObject ui)
    {
        if (!m_isClick)
        {
            return;
        }

        int tempIndex = 0;
        if (ui.name.IndexOf("LT_") != -1)
        {

            tempIndex = 0;
        }
        else if (ui.name.IndexOf("RB_") != -1)
        {
            tempIndex = 3;
        }
        m_MYNGUIPanel[tempIndex].MYClick(ui);
        if (ui.name.IndexOf("BattleValueContainer") != -1)
        {
            MainCityUILT.ShowMainTipWindow();
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
        MainCityUI.TryAddToObjectList(m_AddFunction);
        return true;
    }
}
