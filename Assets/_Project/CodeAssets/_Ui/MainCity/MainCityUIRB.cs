using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

/// <summary>
/// 主城UI界面右下角功能开启按钮管理类
/// </summary>
public class MainCityUIRB : MonoBehaviour
{
    public enum FunctionState
    {
        MainCity,
        AllianceCity,
        House,
        Other
    }

    public FunctionState m_FunctionState
    {
        get { return m_functionState; }
        set
        {
            m_functionState = value;
            switch (m_functionState)
            {
                case FunctionState.MainCity:
                case FunctionState.AllianceCity:
                    {
                        m_FastButtonParent.SetActive(true);
                        m_NavigationButtonParent.SetActive(true);
                        m_HouseButtonParent.SetActive(false);
                        m_MiddleStickButtonParent.SetActive(true);
                        m_RightStickButtonParent.SetActive(true);
                        break;
                    }
                case FunctionState.House:
                    {
                        m_FastButtonParent.SetActive(true);
                        m_NavigationButtonParent.SetActive(false);
                        m_HouseButtonParent.SetActive(true);
                        m_MiddleStickButtonParent.SetActive(true);
                        m_RightStickButtonParent.SetActive(true);
                        break;
                    }
                case FunctionState.Other:
                    {
                        m_FastButtonParent.SetActive(false);
                        m_NavigationButtonParent.SetActive(false);
                        m_HouseButtonParent.SetActive(false);
                        m_MiddleStickButtonParent.SetActive(true);
                        m_RightStickButtonParent.SetActive(true);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Unknown state:" + m_functionState);
                        break;
                    }
            }
        }
    }
    private FunctionState m_functionState;

    public UILabel m_FuncNotOpenInfoLabel;
    public GameObject m_FuncNotOpenInfoObject;

    #region Buttons Initialize and Enable

    public enum ButtonType
    {
        Fast,
        Navigation,
        Stick
    }

    public static readonly Dictionary<int, string> ButtonSpriteNameTransferDic = new Dictionary<int, string>()
    {
        {14,"IconActivityCircle"},
        {104,"IconAlliacneSqaure"},
        {3,"IconBagSqaure"},
        {310,"IconCarriageCircle"},
        {4,"IconFriendSquare"},
        {2001,"IconGetOutCircle"},
        {2002,"IconGetOutCircle"},
        {7,"IconGoHomeCircle"},
        {12,"IconIronsMithCircle"},
        {200,"IconKingSquare"},
        {2000,"IconOperationCircle"},
        {8,"IconRaidCircle"},
        {300,"IconRangerCircle"},
        {210,"IconRankSquare"},
        {13,"IconRechargeCircle"},
        {11,"IconSerarchTreasureSquare"},
        {2,"IconSettingCircle"},
        {9,"IconStoreCircle"},
        {5,"IconTaskSquare"},
        {6,"IconTreasureSquare"},
        {400000,"IconWorshipCircle"},
        {15,"IconZaixianCircle"},
        {16,"IconQiriCircle"},
        {211,"IconRobCircle"},
        {212,"IconNationCircle"},
        {17,"NULLCircle"},
    };

    private List<FunctionButtonManager> m_totalButtonManagerlist
    {
        get { return m_fastButtonManagerlist.Concat(m_navigationButtonManagerlist).Concat(m_houseButtonManagerlist).Concat(m_middleStickButtonManagerlist).Concat(m_rightStickButtonManagerlist).ToList(); }
    }

    private List<FunctionButtonManager> m_fastButtonManagerlist = new List<FunctionButtonManager>();
    private List<FunctionButtonManager> m_navigationButtonManagerlist = new List<FunctionButtonManager>();
    //sticked buttons
    private List<FunctionButtonManager> m_houseButtonManagerlist = new List<FunctionButtonManager>();
    private List<FunctionButtonManager> m_middleStickButtonManagerlist = new List<FunctionButtonManager>();
    private List<FunctionButtonManager> m_rightStickButtonManagerlist = new List<FunctionButtonManager>();

    public GameObject m_FastButtonParent;
    public GameObject m_NavigationButtonParent;
    public GameObject m_HouseButtonParent;
    public GameObject m_MiddleStickButtonParent;
    public GameObject m_RightStickButtonParent;

    private GameObject GetButtonParentByType(int type)
    {
        switch (type)
        {
            case 0:
                return m_FastButtonParent;
            case 1:
                return m_NavigationButtonParent;
            case 2:
                return m_HouseButtonParent;
            case 3:
                return m_MiddleStickButtonParent;
            case 4:
                return m_RightStickButtonParent;
            default:
                Debug.LogError("Error type when get button parent, type:" + type);
                return null;
        }
    }

    private List<FunctionButtonManager> GetButtonListByType(int type)
    {
        switch (type)
        {
            case 0:
                return m_fastButtonManagerlist;
            case 1:
                return m_navigationButtonManagerlist;
            case 2:
                return m_houseButtonManagerlist;
            case 3:
                return m_middleStickButtonManagerlist;
            case 4:
                return m_rightStickButtonManagerlist;
            default:
                Debug.LogError("Error type when get button list, type:" + type);
                return null;
        }
    }

    public GameObject ButtonPrefab;

    private bool isShowBtnEffect;

    public bool IsInitialized = false;

    /// <summary>
    /// Initialize main city uirb.
    /// </summary>
    public void Initialize()
    {
        //Set state
        if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAIN_CITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAIN_CITY_YEWAN)
        {
            m_FunctionState = FunctionState.MainCity;
        }
        else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_YE_WAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN)
        {
            m_FunctionState = FunctionState.AllianceCity;
        }
        else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_HOUSE)
        {
            m_FunctionState = FunctionState.House;
        }
        else
        {
            m_FunctionState = FunctionState.Other;
        }

        //Add buttons.
        foreach (var template in FunctionOpenTemp.templates)
        {
            if (template.type <= 0)
            {
                //Debug.LogWarning("Skip initialize button:" + template.m_iID);
                continue;
            }

            //Set alliance related btns as specially.
            if (template.m_iID == 7 || template.m_iID == 400000)
            {
                continue;
            }

            //Set Zaixian, Qiri acitivity specifically.
            if (template.m_iID == 15 || template.m_iID == 16)
            {
                continue;
            }

            //Set comming soon specifically.
            if (template.m_iID == 17)
            {
                continue;
            }

            //Skip mail, task guide and chat btns.
            if (template.type == 6 || template.type == 7) continue;

            //Skip obsolete btns.
            if (template.m_iID == 2002)
            {
                continue;
            }

            AddButton(template.m_iID);
        }

        //Set alliance related btns as specially.
        if (m_FunctionState == FunctionState.AllianceCity)
        {
            AddButton(7);
            AddButton(400000);
        }

        //Set Zaixian, Qiri acitivity specifically.
        if (!LimitActivityData.Instance.IsDataReceived)
        {
            Debug.LogWarning("Cannot init Zaixian, Qiri acitivty when data not received");
        }
        else
        {
            if (LimitActivityData.Instance.IsOpenZaixianActivity)
            {
                AddButton(15);
            }

            if (LimitActivityData.Instance.IsOpenQiriActivity)
            {
                AddButton(16);
            }
        }

        RankButtons();
        InitializeButtonEnable();

        //execute openclose state.
        InitializeOpenCloseState();

        //Refresh comming soon button.
        RefreshCommingSoonButton();

        //Init red alert.
        PushAndNotificationHelper.UpdateMainMenusNewRedSpot();

        //execute after init.
        if (m_MainCityUIRBDelegate != null)
        {
            m_MainCityUIRBDelegate();
        }

        IsInitialized = true;
    }

    /// <summary>
    /// Add/Remove a button to/from button list.
    /// </summary>
    /// <param name="index"></param>
    private void AddButton(int index, bool isAdd = true)
    {
        FunctionOpenTemp template = FunctionOpenTemp.GetTemplateById(index);
        if (isAdd)
        {
            float scale = (template.type == 4 || template.type == 5) ? 0.75f : 1.0f;

            var tempObject = Instantiate(ButtonPrefab) as GameObject;
            tempObject.transform.name = "button" + index;
            var tempButtonManager = tempObject.GetComponent<FunctionButtonManager>();
            tempButtonManager.SetData(template);
            tempButtonManager.m_OnFuncBtnClick = InnerButtonTrigger;

            GetButtonListByType(template.type - 1).Add(tempButtonManager);

            UtilityTool.ActiveWithStandardize(GetButtonParentByType(template.type - 1).transform, tempObject.transform);
            tempObject.transform.localScale = Vector3.one * scale;
        }
        else
        {
            var tempList = m_totalButtonManagerlist.Where(item => item.m_index == index).ToList();

            if (tempList != null && tempList.Count() == 1)
            {
                GetButtonListByType(template.type - 1).Remove(tempList[0]);
                Destroy(tempList[0].gameObject);
            }
        }
    }

    private void InitializeButtonEnable()
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION))
        {
            FunctionOpenTemp.templates.Where(item => item.type > 0).ToList().ForEach(item => EnableButton(item.m_iID));
        }
        else
        {
            m_totalButtonManagerlist.ForEach(item => item.DenableButton());

            FunctionOpenTemp.m_EnableFuncIDList.ForEach(item => EnableButton(item));

            //Set alliance related btns as normal.
            //if (m_FunctionState == FunctionState.MainCity)
            //{
            //    EnableButton(7, false);
            //    EnableButton(400000, false);
            //}
            //else if (m_FunctionState == FunctionState.AllianceCity)
            //{
            //    EnableButton(7, true);
            //    EnableButton(400000, true);
            //}

            //Set Zaixian, Qiri acitivity specifically.
            EnableButton(15, true);
            EnableButton(16, true);
        }
    }

    /// <summary>
    /// lock or unlock button, default state is unlock
    /// </summary>
    /// <param name="id">fun id</param>
    /// <param name="isEnable">is unlock</param>
    public static void EnableButton(int id, bool isEnable = true)
    {
        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
            .Concat(tempUIRB.m_houseButtonManagerlist)
            .Concat(tempUIRB.m_middleStickButtonManagerlist)
            .Concat(tempUIRB.m_rightStickButtonManagerlist)
            .Where(item => item.m_index == id).ToList();

        if (tempList != null && tempList.Count() == 1)
        {
            if (isEnable)
            {
                tempList[0].EnableButton();
            }
            else
            {
                tempList[0].DenableButton();
            }
        }
    }

    private bool isButtonAdded(int id)
    {
        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
            .Concat(tempUIRB.m_houseButtonManagerlist)
            .Concat(tempUIRB.m_middleStickButtonManagerlist)
            .Concat(tempUIRB.m_rightStickButtonManagerlist)
            .Where(item => item.m_index == id).ToList();

        return tempList != null && tempList.Count() == 1;
    }

    public static bool isButtonEnabled(int id)
    {
        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
            .Concat(tempUIRB.m_houseButtonManagerlist)
            .Concat(tempUIRB.m_middleStickButtonManagerlist)
            .Concat(tempUIRB.m_rightStickButtonManagerlist)
            .Where(item => item.m_index == id).ToList();

        if (tempList != null && tempList.Count() == 1)
        {
            return tempList[0].m_UiButton.isEnabled;
        }
        else
        {
            Debug.LogError("Cannot find button:" + id + " in get enable state.");
            return false;
        }
    }

    /// <summary>
    /// Outter call for add/remove button with enable and rank.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isAdd"></param>
    public static void AddOrRemoveButton(int id, bool isAdd = true)
    {
        if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIRB == null)
        {
            m_AddOrRemoveButtonCommandList.Add(new AddOrRemoveButtonCommand { IsAdd = isAdd, index = id });
            IsDoDelegateAfterInit = true;
            m_MainCityUIRBDelegate += ExecuteStoredAddOrRemoveButtonCommand;
            return;
        }

        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        if (isAdd)
        {
            if (!tempUIRB.isButtonAdded(id))
            {
                tempUIRB.AddButton(id);
            }

            EnableButton(id);
        }
        else
        {
            if (tempUIRB.isButtonAdded(id))
            {
                tempUIRB.AddButton(id, false);
            }
        }

        tempUIRB.RankButtons();
    }

    public class AddOrRemoveButtonCommand
    {
        public int index;
        public bool IsAdd;
    }
    public static List<AddOrRemoveButtonCommand> m_AddOrRemoveButtonCommandList = new List<AddOrRemoveButtonCommand>();

    private static void ExecuteStoredAddOrRemoveButtonCommand()
    {
        IsDoDelegateAfterInit = false;
        m_MainCityUIRBDelegate -= ExecuteStoredAddOrRemoveButtonCommand;

        foreach (var item in m_AddOrRemoveButtonCommandList)
        {
            AddOrRemoveButton(item.index, item.IsAdd);
        }
        m_AddOrRemoveButtonCommandList.Clear();
    }

    private const string CommingSoonSpriteNamePrefix = "commingSoon_";

    public static void RefreshCommingSoonButton()
    {
        if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIRB == null)
        {
            m_RefreshCommingSoonButtonCommandList.Add(new RefreshCommingSoonButtonCommand());
            IsDoDelegateAfterInit = true;
            m_MainCityUIRBDelegate += ExecuteStoredRefreshCommingSoonButtonCommand;
            return;
        }

        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        var temp = FunctionUnlock.templates.Where(item => !FunctionOpenTemp.m_EnableFuncIDList.Contains(item.id));
        if (temp.Any())
        {
            AddOrRemoveButton(17);
            tempUIRB.m_totalButtonManagerlist.Where(item => item.m_index == 17).First().m_ButtonSprite.spriteName = CommingSoonSpriteNamePrefix + temp.First().spriteName;
        }
        else
        {
            AddOrRemoveButton(17, false);
        }
    }

    public class RefreshCommingSoonButtonCommand
    {

    }
    public static List<RefreshCommingSoonButtonCommand> m_RefreshCommingSoonButtonCommandList = new List<RefreshCommingSoonButtonCommand>();

    private static void ExecuteStoredRefreshCommingSoonButtonCommand()
    {
        IsDoDelegateAfterInit = false;
        m_MainCityUIRBDelegate -= ExecuteStoredRefreshCommingSoonButtonCommand;

        if (m_RefreshCommingSoonButtonCommandList.Any())
        {
            RefreshCommingSoonButton();
        }
        m_RefreshCommingSoonButtonCommandList.Clear();
    }

    #endregion

    #region Buttons rank controller

    private const float rankGap = 90;

    private const int fastColumnNum = 4;
    private const int navigationColumnNum = 2;

    public void RankButtons()
    {
        m_fastButtonManagerlist.Sort();
        m_navigationButtonManagerlist.Sort();
        m_houseButtonManagerlist.Sort();
        m_middleStickButtonManagerlist.Sort();
        m_rightStickButtonManagerlist.Sort();

        for (int i = 0; i < m_fastButtonManagerlist.Count; i++)
        {
            m_fastButtonManagerlist[i].transform.localPosition = Vector3.zero + new Vector3(0, -rankGap * (i % fastColumnNum), 0) + new Vector3(-rankGap * (i / fastColumnNum), 0, 0);
        }

        for (int i = 0; i < m_navigationButtonManagerlist.Count; i++)
        {
            m_navigationButtonManagerlist[i].transform.localPosition = Vector3.zero + new Vector3(0, -rankGap * (i % navigationColumnNum), 0) + new Vector3(-rankGap * (i / navigationColumnNum), 0, 0);
        }

        for (int i = 0; i < m_houseButtonManagerlist.Count; i++)
        {
            m_houseButtonManagerlist[i].transform.localPosition = Vector3.zero + new Vector3(0, -rankGap * i, 0);
        }

        for (int i = 0; i < m_middleStickButtonManagerlist.Count; i++)
        {
            m_middleStickButtonManagerlist[i].transform.localPosition = Vector3.zero + new Vector3(rankGap * 0.75f * i, 0, 0);
        }

        for (int i = 0; i < m_rightStickButtonManagerlist.Count; i++)
        {
            m_rightStickButtonManagerlist[i].transform.localPosition = Vector3.zero;
        }
    }

    #endregion

    #region Buttons Click Trigger

    public static void OutterButtonTrigger(int index)
    {
        if (MainCityUI.m_MainCityUI != null)
        {
            MainCityUI.m_MainCityUI.m_MainCityUIRB.InnerButtonTrigger(index);
        }
    }

    private GameObject m_bagObject;

    private void InnerButtonTrigger(int id)
    {
        //trigger when one finger touch, not in character control and all windows closed.
        if (Input.touchCount <= 1 && IsCanClickButtons)
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
                case 2:
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SETTINGS_UP_LAYER),
                                         SettingUpLoadCallback);
                    }
                    break;
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
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_TANBAO),
                                                SerachTreasureLoadCallback);
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

    int BigHouseId = 0;
    int SmallHouseId = 0;

    /// <summary>
    /// Execute on gohome btn click
    /// </summary>
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

    private void LoadYouXiaBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }

    }

    public void RewardCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void TaskLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

    public void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

    }

    public void SerachTreasureLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.name = "QXTanBao";
    }

    public void SettingUpLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);


        UIYindao.m_UIYindao.CloseUI();

    }

    public void FriendLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
        UIYindao.m_UIYindao.CloseUI();
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

    public void RankWindowLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject temp = (GameObject)Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(temp);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    #endregion

    #region Red Alert and Panel Control

    public static bool IsDoDelegateAfterInit = false;
    public delegate void MainCityUIRBDelegate();
    public static MainCityUIRBDelegate m_MainCityUIRBDelegate;

    [HideInInspector]
    public UIPanel m_Panel;
    [HideInInspector]
    public UIPanel m_YinDaoPanel;
    public static bool IsCanClickButtons = true;

    public static bool IsFastButtonOpen
    {
        get
        {
            if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIRB == null)
            {
                Debug.LogWarning("Cannot get fast button open state cause main city ui not exist.");
                return false;
            }
            return MainCityUI.m_MainCityUI.m_MainCityUIRB.m_fastButtonSet.m_IsButtonOpen;
        }
    }

    public static bool IsNavigationButtonOpen
    {
        get
        {
            if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIRB == null)
            {
                Debug.LogWarning("Cannot get navigation button open state cause main city ui not exist.");
                return false;
            }
            return MainCityUI.m_MainCityUI.m_MainCityUIRB.m_navigationButtonSet.m_IsButtonOpen;
        }
    }

    public class RedAlertCommand
    {
        public int index;
        public bool IsShow;
        public bool m_manual_set;
    }
    public static List<RedAlertCommand> m_RedAlertCommandList = new List<RedAlertCommand>();

    private const float FadeInOutDuration = 1.0f;
    private bool isYinDaoShow;
    private float panelYinDaoFadeSpeed;

    /// <summary>
    /// Set MainCity UI panel.
    /// </summary>
    /// <param name="isShow"></param>
    public void SetPanel(bool isShow)
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", m_Panel.alpha,
                    "to", isShow ? 1.0f : 0.0f,
                    "time", FadeInOutDuration,
                    "easetype", "linear",
                    "onupdate", "UpdatePanelA"));

        StopCoroutine("UpdateYinDaoPanelA");

        if (UIYindao.m_UIYindao == null)
        {
            return;
        }
        m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();

        isYinDaoShow = isShow;
        panelYinDaoFadeSpeed = Math.Abs(m_YinDaoPanel.alpha - (isShow ? 1.0f : 0.0f)) * (1 / FadeInOutDuration);
        StartCoroutine("UpdateYinDaoPanelA");
    }

    private void UpdatePanelA(float a)
    {
        m_Panel.alpha = a;
    }

    private IEnumerator UpdateYinDaoPanelA()
    {
        while (m_YinDaoPanel.alpha >= 0 && m_YinDaoPanel.alpha <= 1)
        {
            if (isYinDaoShow)
            {
                m_YinDaoPanel.alpha += panelYinDaoFadeSpeed * Time.deltaTime;
            }
            else
            {
                m_YinDaoPanel.alpha -= panelYinDaoFadeSpeed * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private static void ExecuteStoredRedAlertCommand()
    {
        IsDoDelegateAfterInit = false;
        m_MainCityUIRBDelegate -= ExecuteStoredRedAlertCommand;

        foreach (var item in m_RedAlertCommandList)
        {
            SetRedAlert(item.index, item.IsShow, item.m_manual_set);
        }
        m_RedAlertCommandList.Clear();
    }

    /// <summary>
    /// Set main city ui RB button's red alert active or deactive.
    /// [WARNING] Note that calling of zaixian activity and xianshi activity's red alert will show particle effect.
    /// </summary>
    /// <param name="id">function id, reference in FunctionOpen.xml</param>
    /// <param name="isShow">active or deactive, true: active, false: deactive</param>
    /// <returns>set successfully or not, true: succeed, false: fail</returns>
    /// p_manual_set: if it's called by old red spot sys.
    public static bool SetRedAlert(int id, bool isShow, bool p_manual_set = true)
    {
        // if red spot is manual setted, then check if it could be manually changed.
        if (p_manual_set)
        {
            if (!PushAndNotificationHelper.IsManualSetRedNotificationOK(id, isShow))
            {
                return true;
            }
        }

        try
        {
            //Add to command list if UI not inited, then return true.
            if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIRB == null)
            {
                m_RedAlertCommandList.Add(new RedAlertCommand { IsShow = isShow, index = id, m_manual_set = p_manual_set });
                IsDoDelegateAfterInit = true;
                m_MainCityUIRBDelegate += ExecuteStoredRedAlertCommand;
                return true;
            }

            MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

            var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
                .Concat(tempUIRB.m_houseButtonManagerlist)
                .Concat(tempUIRB.m_middleStickButtonManagerlist)
                .Concat(tempUIRB.m_rightStickButtonManagerlist)
                .Where(item => item.m_index == id).ToList();

            if (tempList != null && tempList.Count == 1)
            {
                if (isShow)
                {
                    if (tempList[0].m_index == 15 || tempList[0].m_index == 16)
                    {
                        tempList[0].IsAlertShowed = true;
                        UI3DEffectTool.Instance().ClearUIFx(tempList[0].gameObject);
                        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, tempList[0].gameObject, EffectTemplate.getEffectTemplateByEffectId(100185).path);
                    }
                    else
                    {
                        tempList[0].ShowRedAlert();
                    }
                    tempUIRB.CheckActiveSlideRedAlert();
                }
                else
                {
                    if (tempList[0].m_index == 15 || tempList[0].m_index == 16)
                    {
                        tempList[0].IsAlertShowed = false;
                        UI3DEffectTool.Instance().ClearUIFx(tempList[0].gameObject);
                    }
                    else
                    {
                        tempList[0].HideRedAlert();
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Got exception in setting alert, ex:" + ex.Message + ", \nstackTrace:" + ex.StackTrace);
            return false;
        }
    }

    public static bool LockRedAlert(int id, bool isRemove = false)
    {
        try
        {
            //Lock red alert state that cannot be setted.
            if (isRemove)
            {
                if (FunctionButtonManager.s_LockedList.Contains(id)) FunctionButtonManager.s_LockedList.Remove(id);
            }
            else
            {
                if (!FunctionButtonManager.s_LockedList.Contains(id)) FunctionButtonManager.s_LockedList.Add(id);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Got exception in LockRedAlert, ex:" + ex.Message + ", \nstackTrace:" + ex.StackTrace);
            return false;
        }
    }

    #endregion

    #region OpenClose Controller

    public EventIndexHandle m_FastOpenHandler;
    public EventIndexHandle m_FastCloseHandler;
    public EventIndexHandle m_NavigationOpenHandler;
    public EventIndexHandle m_NavigationCloseHandler;

    public GameObject m_FastMovingObject;
    public GameObject m_NavigationMovingObject;

    public GameObject m_FastRedAlertObject;
    public GameObject m_NavigationRedAlertObject;

    private const float MovingDuration = 0.5f;

    private bool isNoiTween = false;

    /// <summary>
    /// button set is used for control a whole series of button, containing different itween mode, open close buttons, open close position, columns, moving object.
    /// </summary>
    private class ButtonSet
    {
        public iTweenMode m_iTweenMode;

        public bool m_IsButtonOpen;
        public List<FunctionButtonManager> m_buttonManagerList = new List<FunctionButtonManager>();
        public EventIndexHandle m_openHandle;
        public EventIndexHandle m_closeHandle;
        public GameObject m_redAlertObject;
        public GameObject m_MovingObject;

        public delegate void VoidDelegate();

        public VoidDelegate m_CloseCompleteDelegate;

        //moving object's move position.

        public Vector3 m_openPos
        {
            get { return m_closePos - new Vector3(rankGap * ((m_buttonManagerList.Count / m_columnNum) + (m_buttonManagerList.Count % m_columnNum == 0 ? -1 : 0)), 0, 0); }
        }

        public Vector3 m_closePos;

        /// <summary>
        /// column num is used for active and deactive specific buttons.
        /// </summary>
        public int m_columnNum;

        public void Open()
        {
            m_iTweenMode.Open(this);
        }

        public void Close()
        {
            m_iTweenMode.Close(this);
        }
    }

    /// <summary>
    /// fast button set in main city ui.
    /// </summary>
    private ButtonSet m_fastButtonSet = new ButtonSet();

    /// <summary>
    /// navigation button set in main city ui.
    /// </summary>
    private ButtonSet m_navigationButtonSet = new ButtonSet();

    /// <summary>
    /// house button in house ui.
    /// </summary>
    private ButtonSet m_houseButtonSet = new ButtonSet();

    /// <summary>
    /// different itween mode in button set control
    /// </summary>
    private abstract class iTweenMode
    {
        public virtual void Open(ButtonSet buttonSet)
        {
            buttonSet.m_IsButtonOpen = true;

            buttonSet.m_openHandle.gameObject.SetActive(false);
            buttonSet.m_redAlertObject.SetActive(false);
        }

        public virtual void Close(ButtonSet buttonSet)
        {
            buttonSet.m_buttonManagerList.Where(item => buttonSet.m_buttonManagerList.IndexOf(item) >= buttonSet.m_columnNum).ToList().ForEach(item => item.gameObject.SetActive(false));

            buttonSet.m_closeHandle.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// do open/close buttons with itween animation.
    /// </summary>
    private class WithiTween : iTweenMode
    {
        public static List<ButtonSet> m_OpenButtonSetList = new List<ButtonSet>();
        public static List<ButtonSet> m_CloseButtonSetList = new List<ButtonSet>();

        public override void Open(ButtonSet buttonSet)
        {
            m_OpenButtonSetList.Add(buttonSet);

            base.Open(buttonSet);

            iTween.MoveTo(buttonSet.m_MovingObject,
                iTween.Hash("time", MovingDuration, "position", buttonSet.m_openPos, "easetype", "easeOutBack", "islocal", true, "oncomplete", "OnOpenComplete", "oncompletetarget", MainCityUI.m_MainCityUI.m_MainCityUIRB.gameObject));
        }

        public override void Close(ButtonSet buttonSet)
        {
            m_CloseButtonSetList.Add(buttonSet);

            base.Close(buttonSet);

            iTween.MoveTo(buttonSet.m_MovingObject,
                iTween.Hash("time", MovingDuration, "position", buttonSet.m_closePos, "easetype", "easeInBack", "islocal", true, "oncomplete", "OnCloseComplete", "oncompletetarget", MainCityUI.m_MainCityUI.m_MainCityUIRB.gameObject));
        }
    }

    public delegate void VoidDelegate();

    public VoidDelegate ButtonOpenDelegate;

    public void OnOpenComplete()
    {
        object temp = new object();

        lock (temp)
        {
            for (int i = 0; i < WithiTween.m_OpenButtonSetList.Count; i++)
            {
                var m_buttonSet = WithiTween.m_OpenButtonSetList[i];
                m_buttonSet.m_buttonManagerList.Where(
                    item => m_buttonSet.m_buttonManagerList.IndexOf(item) >= m_buttonSet.m_columnNum)
                    .ToList()
                    .ForEach(item => item.gameObject.SetActive(true));
                m_buttonSet.m_closeHandle.gameObject.SetActive(true);

                WithiTween.m_OpenButtonSetList.Remove(m_buttonSet);
            }

            if (ButtonOpenDelegate != null)
            {
                ButtonOpenDelegate();
                ButtonOpenDelegate = null;
            }
        }
    }

    public void OnCloseComplete()
    {
        object temp = new object();

        lock (temp)
        {

            for (int i = 0; i < WithiTween.m_CloseButtonSetList.Count; i++)
            {
                var m_buttonSet = WithiTween.m_CloseButtonSetList[i];

                m_buttonSet.m_IsButtonOpen = false;

                m_buttonSet.m_openHandle.gameObject.SetActive(true);

                WithiTween.m_CloseButtonSetList.Remove(m_buttonSet);

                if (m_buttonSet.m_CloseCompleteDelegate != null)
                {
                    m_buttonSet.m_CloseCompleteDelegate();
                    m_buttonSet.m_CloseCompleteDelegate = null;
                }
            }

            CheckActiveSlideRedAlert();
        }
    }

    /// <summary>
    /// do open/close button without itween animation.
    /// </summary>
    private class WithoutiTween : iTweenMode
    {
        public override void Open(ButtonSet buttonSet)
        {
            base.Open(buttonSet);

            buttonSet.m_MovingObject.transform.localPosition = buttonSet.m_openPos;
            buttonSet.m_buttonManagerList.Where(item => buttonSet.m_buttonManagerList.IndexOf(item) >= buttonSet.m_columnNum).ToList().ForEach(item => item.gameObject.SetActive(true));
            buttonSet.m_closeHandle.gameObject.SetActive(true);
        }

        public override void Close(ButtonSet buttonSet)
        {
            base.Close(buttonSet);

            buttonSet.m_IsButtonOpen = false;

            buttonSet.m_MovingObject.transform.localPosition = buttonSet.m_closePos;
            buttonSet.m_openHandle.gameObject.SetActive(true);

            MainCityUI.m_MainCityUI.m_MainCityUIRB.CheckActiveSlideRedAlert();

            if (buttonSet.m_CloseCompleteDelegate != null)
            {
                buttonSet.m_CloseCompleteDelegate();
                buttonSet.m_CloseCompleteDelegate = null;
            }
        }
    }

    /// <summary>
    /// main open/close button set control.
    /// </summary>
    /// <param name="index"></param>
    private void OnOpenCloseClick(int index)
    {
        ButtonSet tempButtonSet;

        //		Debug.Log(index);

        switch (index)
        {
            //fast buttons open
            case 1:
            //fast buttons close
            case 2:
                {
                    tempButtonSet = m_fastButtonSet;

                    //with no itween
                    if (isNoiTween)
                    {
                        tempButtonSet.m_iTweenMode = new WithoutiTween();
                    }
                    //with itween
                    else
                    {
                        tempButtonSet.m_iTweenMode = new WithiTween();
                    }

                    //fast buttons open
                    if (index == 1)
                    {
                        tempButtonSet.Open();

                        if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            if (UIYindao.m_UIYindao.m_iCurId == 604 || UIYindao.m_UIYindao.m_iCurId == 6040)
                            {
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                        }
                    }
                    //fast buttons close
                    else if (index == 2)
                    {
                        if (!isNoiTween)
                        {
                            tempButtonSet.m_CloseCompleteDelegate += OnCloseEndDelegate;
                        }

                        tempButtonSet.Close();
                    }
                    break;
                }
            //navigation buttons open
            case 3:
            //navigation buttons close
            case 4:
                {
                    //Set to navigation button set
                    if (m_FunctionState == FunctionState.MainCity || m_FunctionState == FunctionState.AllianceCity)
                    {
                        tempButtonSet = m_navigationButtonSet;
                    }
                    //Set to house button set
                    else if (m_FunctionState == FunctionState.House)
                    {
                        tempButtonSet = m_houseButtonSet;
                    }
                    else
                    {
                        Debug.LogError("Not correct function state:" + m_FunctionState);
                        return;
                    }

                    //with out itween
                    if (isNoiTween)
                    {
                        tempButtonSet.m_iTweenMode = new WithoutiTween();
                    }
                    //with itween
                    else
                    {
                        tempButtonSet.m_iTweenMode = new WithiTween();
                    }

                    //navigation buttons open
                    if (index == 3)
                    {
                        tempButtonSet.Open();

                        if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            if (FreshGuide.Instance().IsActive(TaskData.Instance.m_iCurMissionIndex))
                            {
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                if (UIYindao.m_UIYindao.m_iCurId == 603 || UIYindao.m_UIYindao.m_iCurId == 6030)
                                {
                                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                                }
                            }
                        }
                    }
                    //navigation buttons close
                    else if (index == 4)
                    {
                        if (!isNoiTween)
                        {
                            tempButtonSet.m_CloseCompleteDelegate += OnCloseEndDelegate;
                        }

                        tempButtonSet.Close();
                    }
                    break;
                }
        }
    }

    private void OnCloseEndDelegate()
    {
        MainCityUI.m_MainCityUI.setInit();
    }

    private void CheckActiveSlideRedAlert()
    {
        if (!IsFastButtonOpen)
        {
            m_FastRedAlertObject.SetActive(m_fastButtonManagerlist.Any(item => item.IsAlertShowed && item.m_UiButton.isEnabled && m_fastButtonManagerlist.IndexOf(item) >= fastColumnNum));
        }

        if (!IsNavigationButtonOpen)
        {
            m_NavigationRedAlertObject.SetActive(m_navigationButtonManagerlist.Any(item => item.IsAlertShowed && item.m_UiButton.isEnabled && m_navigationButtonManagerlist.IndexOf(item) >= navigationColumnNum));
        }
    }

    /// <summary>
    /// Initialize open close buttons state
    /// </summary>
    /// <param name="isClose">isclose, default state is closed</param>
    private void InitializeOpenCloseState(bool isClose = true)
    {
        isNoiTween = true;
        OnOpenCloseClick(2);
        OnOpenCloseClick(4);
        isNoiTween = false;
    }

    #endregion

    #region Time Calc Controller

    public GameObject TimeCalcRoot;
    public UILabel TimeCalcLabel;

    /// <summary>
    /// Show time calculation in online activity.
    /// </summary>
    /// <param name="second">time measured by seconds</param>
    public static void ShowTimeCalc(int second)
    {
        if (MainCityUI.m_MainCityUI != null)
        {
            MainCityUI.m_MainCityUI.m_MainCityUIRB.DoShowTimeCalc(second);
        }
    }

    private int TotalTimeCalcSecond;

    void DoShowTimeCalc(int second)
    {
        TotalTimeCalcSecond = second;

        var tempList = m_middleStickButtonManagerlist.Where(item => item.m_index == 15).ToList();
        if (tempList == null || tempList.Count != 1)
        {
            return;
        }

        TimeCalcRoot.transform.position = tempList[0].transform.position;
        TimeCalcRoot.transform.localScale = tempList[0].transform.localScale;
        TimeCalcRoot.gameObject.SetActive(true);

        if (TimeHelper.Instance.IsTimeCalcKeyExist("MainCityUIRBTimeCalc"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");
        }
        TimeHelper.Instance.AddEveryDelegateToTimeCalc("MainCityUIRBTimeCalc", second, RefreshTimeCalc);

        //hide red alert in zaixian activity
        SetRedAlert(15, false);
    }

    void RefreshTimeCalc(int second)
    {
        if (TotalTimeCalcSecond - second > 0)
        {
            TimeCalcLabel.text = ColorTool.Color_Green_00ff00 + TimeHelper.SecondToClockTime(TotalTimeCalcSecond - second) + "[-]";
        }
        else
        {
            TimeCalcRoot.gameObject.SetActive(false);

            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");

            //Show red alert in zaixian activity
            SetRedAlert(15, true);
        }
    }

    #endregion

    #region Play Add Button Effect

    public bool PlayAddButton(string id)
    {
        StartCoroutine(DoPlayAddButton(int.Parse(id)));
        return true;
    }

    private int PlayAddButtonID;
    private GameObject PlayAddButtonObject;

    private IEnumerator DoPlayAddButton(int id)
    {
        //Wait for initialize finished.
        while (!IsInitialized)
        {
            yield return new WaitForEndOfFrame();
        }

        var temp = m_totalButtonManagerlist.Where(item => item.m_index == id);
        if (temp != null && temp.Count() == 1)
        {
            PlayAddButtonID = id;
            PlayAddButtonObject = temp.First().gameObject;

            if (m_fastButtonManagerlist.Select(item => item.m_index).Contains(id))
            {
                ButtonOpenDelegate = PlayAddButtonEffect;
                OnOpenCloseClick(1);
                yield break;
            }
            else if (m_navigationButtonManagerlist.Select(item => item.m_index).Contains(id))
            {
                ButtonOpenDelegate = PlayAddButtonEffect;
                OnOpenCloseClick(3);
                yield break;
            }

            PlayAddButtonEffect();
        }
    }

    private void PlayAddButtonEffect()
    {
        StartCoroutine(DoPlayAddButtonEffect());
    }

    private IEnumerator DoPlayAddButtonEffect()
    {
        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, PlayAddButtonObject, EffectTemplate.getEffectTemplateByEffectId(100166).path);
        yield return new WaitForSeconds(0.5f);

        EnableButton(PlayAddButtonID);
        yield return new WaitForSeconds(0.5f);

        UI3DEffectTool.Instance().ClearUIFx(PlayAddButtonObject);

        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, PlayAddButtonObject, EffectTemplate.getEffectTemplateByEffectId(ButtonSpriteNameTransferDic[PlayAddButtonID].Contains("Square") ? 100167 : 100168).path);
        yield return new WaitForSeconds(1.0f);
        UI3DEffectTool.Instance().ClearUIFx(PlayAddButtonObject);

        Global.m_iOpenFunctionIndex = -1;
        ClientMain.closePopUp();
    }

    #endregion

    #region Mono

    void OnDisable()
    {
        if (UtilityTool.m_ApplicationIsQuitting) return;

        if (TimeHelper.Instance.IsTimeCalcKeyExist("MainCityUIRBTimeCalc"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");
        }
    }

    void Destroy()
    {
        m_FastOpenHandler.m_Handle -= OnOpenCloseClick;
        m_FastCloseHandler.m_Handle -= OnOpenCloseClick;
        m_NavigationOpenHandler.m_Handle -= OnOpenCloseClick;
        m_NavigationCloseHandler.m_Handle -= OnOpenCloseClick;
    }

    void Awake()
    {
        m_FastOpenHandler.m_Handle += OnOpenCloseClick;
        m_FastCloseHandler.m_Handle += OnOpenCloseClick;
        m_NavigationOpenHandler.m_Handle += OnOpenCloseClick;
        m_NavigationCloseHandler.m_Handle += OnOpenCloseClick;

        m_Panel = GetComponent<UIPanel>();
        if (UIYindao.m_UIYindao != null)
        {
            m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();
        }

        //Initialize buttonset.
        m_fastButtonSet.m_buttonManagerList = m_fastButtonManagerlist;
        m_fastButtonSet.m_openHandle = m_FastOpenHandler;
        m_fastButtonSet.m_closeHandle = m_FastCloseHandler;
        m_fastButtonSet.m_redAlertObject = m_FastRedAlertObject;
        m_fastButtonSet.m_MovingObject = m_FastMovingObject;
        m_fastButtonSet.m_closePos = Vector3.zero;
        m_fastButtonSet.m_columnNum = fastColumnNum;

        m_navigationButtonSet.m_buttonManagerList = m_navigationButtonManagerlist;
        m_navigationButtonSet.m_openHandle = m_NavigationOpenHandler;
        m_navigationButtonSet.m_closeHandle = m_NavigationCloseHandler;
        m_navigationButtonSet.m_redAlertObject = m_NavigationRedAlertObject;
        m_navigationButtonSet.m_MovingObject = m_NavigationMovingObject;
        m_navigationButtonSet.m_closePos = Vector3.zero;
        m_navigationButtonSet.m_columnNum = navigationColumnNum;

        m_houseButtonSet.m_buttonManagerList = m_houseButtonManagerlist;
        m_houseButtonSet.m_openHandle = m_NavigationOpenHandler;
        m_houseButtonSet.m_closeHandle = m_NavigationCloseHandler;
        m_houseButtonSet.m_redAlertObject = m_NavigationRedAlertObject;
        m_houseButtonSet.m_MovingObject = m_NavigationMovingObject;
        m_houseButtonSet.m_closePos = Vector3.zero;
        m_houseButtonSet.m_columnNum = navigationColumnNum;
    }

    #endregion
}