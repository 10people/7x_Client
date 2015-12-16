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
public class MainCityUIRB : MYNGUIPanel
{
    public UILabel m_FuncNotOpenInfoLabel;
    public GameObject m_FuncNotOpenInfoObject;
	public static bool isOpen = true;
	public UISprite m_SpriteOpen;
	public MainCityListButtonManager m_MainCityListButtonManager_RB;
	public MainCityListButtonManager m_MainCityListButtonManager_R;
	public GameObject m_ObjPropUse;
	public IconSampleManager m_IconSampleManager;
	public UILabel m_UILabelPropName;
	public UILabel m_UILabelPropUse;
	public List<int> m_listPropUseID = new List<int>();
	public List<int> m_listPropUseNum = new List<int>();
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
		{1210,"IconQiriCircle"},
		{250,"IconQiriCircle"},
    };

    private bool isShowBtnEffect;

    public bool IsInitialized = false;

	void Awake()
	{
		m_Panel = GetComponent<UIPanel>();
		if (UIYindao.m_UIYindao != null)
		{
			m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();
		}
	}

    /// <summary>
    /// Initialize main city uirb.
    /// </summary>
	public void Initialize(MainCityListButtonManager rb, MainCityListButtonManager r)
    {
		m_MainCityListButtonManager_RB = rb;
		m_MainCityListButtonManager_R = r;
        //Add buttons.
//        foreach (var template in FunctionOpenTemp.templates)
//        {
//            if (template.type <= 0)
//            {
//                //Debug.LogWarning("Skip initialize button:" + template.m_iID);
//                continue;
//            }
//
//            //Set alliance related btns as specially.
//            if (template.m_iID == 7 || template.m_iID == 400000)
//            {
//                continue;
//            }
//
//            //Set Zaixian, Qiri acitivity specifically.
//            if (template.m_iID == 15 || template.m_iID == 16)
//            {
//                continue;
//            }
//
//            //Set comming soon specifically.
//            if (template.m_iID == 17)
//            {
//                continue;
//            }
//
//            //Skip mail, task guide and chat btns.
//            if (template.type == 6 || template.type == 7) continue;
//
//            //Skip obsolete btns.
//            if (template.m_iID == 2002)
//            {
//                continue;
//            }
//
//            AddButton(template.m_iID);
//        }

        //Set alliance related btns as specially.
//        if (m_FunctionState == FunctionState.AllianceCity)
//        {
//            AddButton(7);
//            AddButton(400000);
//        }

        //Set Zaixian, Qiri acitivity specifically.
//        if (!LimitActivityData.Instance.IsDataReceived)
//        {
//            Debug.LogWarning("Cannot init Zaixian, Qiri acitivty when data not received");
//        }
//        else
//        {
//            if (LimitActivityData.Instance.IsOpenZaixianActivity)
//            {
//                AddButton(15);
//            }
//
//            if (LimitActivityData.Instance.IsOpenQiriActivity)
//            {
//                AddButton(16);
//            }
//        }
//
//        //Refresh comming soon button.
//        RefreshCommingSoonButton();
//
//        //Init red alert.
//        PushAndNotificationHelper.UpdateMainMenusNewRedSpot();
//
//        //execute after init.
//        if (m_MainCityUIRBDelegate != null)
//        {
//            m_MainCityUIRBDelegate();
//        }
//
//        IsInitialized = true;
    }

    /// <summary>
    /// Add/Remove a button to/from button list.
    /// </summary>
    /// <param name="index"></param>
    private void AddButton(int index, bool isAdd = true)
    {
//        FunctionOpenTemp template = FunctionOpenTemp.GetTemplateById(index);
//        if (isAdd)
//        {
//            float scale = (template.type == 4 || template.type == 5) ? 0.75f : 1.0f;
//
//            var tempObject = Instantiate(ButtonPrefab) as GameObject;
//            tempObject.transform.name = "button" + index;
//            var tempButtonManager = tempObject.GetComponent<FunctionButtonManager>();
//            tempButtonManager.SetData(template);
////            tempButtonManager.m_OnFuncBtnClick = InnerButtonTrigger;
//
//            GetButtonListByType(template.type - 1).Add(tempButtonManager);
//
//            TransformHelper.ActiveWithStandardize(GetButtonParentByType(template.type - 1).transform, tempObject.transform);
//            tempObject.transform.localScale = Vector3.one * scale;
//        }
//        else
//        {
//            var tempList = m_totalButtonManagerlist.Where(item => item.m_index == index).ToList();
//
//            if (tempList != null && tempList.Count() == 1)
//            {
//                GetButtonListByType(template.type - 1).Remove(tempList[0]);
//                Destroy(tempList[0].gameObject);
//            }
//        }
    }

    private bool isButtonAdded(int id)
    {
        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

//		var tempList;

//        return tempList != null && tempList.Count() == 1;
		return false;
    }

    public static bool isButtonEnabled(int id)
    {
        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

//        var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
//            .Concat(tempUIRB.m_houseButtonManagerlist)
//            .Concat(tempUIRB.m_middleStickButtonManagerlist)
//            .Concat(tempUIRB.m_rightStickButtonManagerlist)
//            .Where(item => item.m_index == id).ToList();

//        if (tempList != null && tempList.Count() == 1)
//        {
//            return tempList[0].m_UiButton.isEnabled;
//        }
//        else
//        {
//            Debug.LogError("Cannot find button:" + id + " in get enable state.");
            return false;
//        }
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
        }
        else
        {
            if (tempUIRB.isButtonAdded(id))
            {
                tempUIRB.AddButton(id, false);
            }
        }

//        tempUIRB.RankButtons();
    }

    public class AddOrRemoveButtonCommand
    {
        public int index;
        public bool IsAdd;
    }
    public static List<AddOrRemoveButtonCommand> m_AddOrRemoveButtonCommandList = new List<AddOrRemoveButtonCommand>();

    private static void ExecuteStoredAddOrRemoveButtonCommand()
    {
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
            m_MainCityUIRBDelegate += ExecuteStoredRefreshCommingSoonButtonCommand;
            return;
        }

        MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

        var temp = FunctionUnlock.templates.Where(item => !FunctionOpenTemp.m_EnableFuncIDList.Contains(item.id));
        if (temp.Any())
        {
            AddOrRemoveButton(17);
//            tempUIRB.m_totalButtonManagerlist.Where(item => item.m_index == 17).First().m_ButtonSprite.spriteName = CommingSoonSpriteNamePrefix + temp.First().spriteName;
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

    #endregion

    #region Buttons Click Trigger



    /// <summary>
    /// Execute on gohome btn click
    /// </summary>
    



    private void LoadYouXiaBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }

    }
    #endregion

    #region Red Alert and Panel Control

    public delegate void MainCityUIRBDelegate();
    public static MainCityUIRBDelegate m_MainCityUIRBDelegate;

    [HideInInspector]
    public UIPanel m_Panel;
    [HideInInspector]
    public UIPanel m_YinDaoPanel;
    public static bool IsCanClickButtons = true;

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
//        iTween.ValueTo(gameObject, iTween.Hash(
//            "from", m_Panel.alpha,
//                    "to", isShow ? 1.0f : 0.0f,
//                    "time", FadeInOutDuration,
//                    "easetype", "linear",
//                    "onupdate", "UpdatePanelA"));
//
//        StopCoroutine("UpdateYinDaoPanelA");
//
//        if (UIYindao.m_UIYindao == null)
//        {
//            return;
//        }
//        m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();
//
//        isYinDaoShow = isShow;
//        panelYinDaoFadeSpeed = Math.Abs(m_YinDaoPanel.alpha - (isShow ? 1.0f : 0.0f)) * (1 / FadeInOutDuration);
//        StartCoroutine("UpdateYinDaoPanelA");
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
                m_MainCityUIRBDelegate += ExecuteStoredRedAlertCommand;
                return true;
            }

            MainCityUIRB tempUIRB = MainCityUI.m_MainCityUI.m_MainCityUIRB;

//            var tempList = tempUIRB.m_fastButtonManagerlist.Concat(tempUIRB.m_navigationButtonManagerlist)
//                .Concat(tempUIRB.m_houseButtonManagerlist)
//                .Concat(tempUIRB.m_middleStickButtonManagerlist)
//                .Concat(tempUIRB.m_rightStickButtonManagerlist)
//                .Where(item => item.m_index == id).ToList();

//            if (tempList != null && tempList.Count == 1)
//            {
//                if (isShow)
//                {
//                    if (tempList[0].m_index == 15 || tempList[0].m_index == 16)
//                    {
//                        tempList[0].IsAlertShowed = true;
//                        UI3DEffectTool.Instance().ClearUIFx(tempList[0].gameObject);
//                        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, tempList[0].gameObject, EffectTemplate.getEffectTemplateByEffectId(100185).path);
//                    }
//                    else
//                    {
//                        tempList[0].ShowRedAlert();
//                    }
//                    tempUIRB.CheckActiveSlideRedAlert();
//                }
//                else
//                {
//                    if (tempList[0].m_index == 15 || tempList[0].m_index == 16)
//                    {
//                        tempList[0].IsAlertShowed = false;
//                        UI3DEffectTool.Instance().ClearUIFx(tempList[0].gameObject);
//                    }
//                    else
//                    {
//                        tempList[0].HideRedAlert();
//                    }
//                }
//            }

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

    private const float MovingDuration = 0.5f;

    private bool isNoiTween = false;

    private void OnCloseEndDelegate()
    {
        MainCityUI.m_MainCityUI.setInit();
    }

    private void CheckActiveSlideRedAlert()
    {

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

//        var tempList = m_middleStickButtonManagerlist.Where(item => item.m_index == 15).ToList();
//        if (tempList == null || tempList.Count != 1)
//        {
//            return;
//        }

//        TimeCalcRoot.transform.position = tempList[0].transform.position;
//        TimeCalcRoot.transform.localScale = tempList[0].transform.localScale;
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

//        var temp = m_totalButtonManagerlist.Where(item => item.m_index == id);
//        if (temp != null && temp.Count() == 1)
//        {
//            PlayAddButtonID = id;
//            PlayAddButtonObject = temp.First().gameObject;
//
//            if (m_fastButtonManagerlist.Select(item => item.m_index).Contains(id))
//            {
//                ButtonOpenDelegate = PlayAddButtonEffect;
//                OnOpenCloseClick(1);
//                yield break;
//            }
//            else if (m_navigationButtonManagerlist.Select(item => item.m_index).Contains(id))
//            {
//                ButtonOpenDelegate = PlayAddButtonEffect;
//                OnOpenCloseClick(3);
//                yield break;
//            }
//
//            PlayAddButtonEffect();
//        }
    }

    private void PlayAddButtonEffect()
    {
        StartCoroutine(DoPlayAddButtonEffect());
    }

    private IEnumerator DoPlayAddButtonEffect()
    {
        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, PlayAddButtonObject, EffectTemplate.getEffectTemplateByEffectId(100166).path);
        yield return new WaitForSeconds(0.5f);

//        EnableButton(PlayAddButtonID);
//        yield return new WaitForSeconds(0.5f);

        UI3DEffectTool.Instance().ClearUIFx(PlayAddButtonObject);

        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, PlayAddButtonObject, EffectTemplate.getEffectTemplateByEffectId(ButtonSpriteNameTransferDic[PlayAddButtonID].Contains("Square") ? 100167 : 100168).path);
        yield return new WaitForSeconds(1.0f);
        UI3DEffectTool.Instance().ClearUIFx(PlayAddButtonObject);

        Global.m_iOpenFunctionIndex = -1;
        ClientMain.closePopUp();
    }

	public void setPropUse(int id, int num = 0)
	{
		bool tempAdd = true;
		for(int i = 0; i < m_listPropUseID.Count; i ++)
		{
			if(m_listPropUseID.Contains(id))
			{
				tempAdd = false;
				break;
			}
		}
		if(tempAdd)
		{
			m_listPropUseID.Add(id);
			m_listPropUseNum.Add(num);
		}
		if(!m_ObjPropUse.activeSelf)
		{
			m_ObjPropUse.SetActive(true);
//			if(MainCityUIRB.isOpen)
//			{
				m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);
//			}
//			else
//			{
//				m_ObjPropUse.transform.localPosition = new Vector3(-190, 100, 0);
//			}
			if(num == 0)
			{
				m_IconSampleManager.SetIconByID(id);
			}
			else
			{
				m_IconSampleManager.SetIconByID(id, "x"+num);
			}
			m_IconSampleManager.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			CommonItemTemplate tempCommon = CommonItemTemplate.getCommonItemTemplateById(id);
			m_UILabelPropName.text = NameIdTemplate.GetName_By_NameId (tempCommon.nameId);
			if(tempCommon.itemType == 2)
			{
				m_UILabelPropUse.text = "点击穿戴";
			}
			else
			{
				m_UILabelPropUse.text = "点击使用";
			}
		}
//
//		int typeID = .itemType;
//
//		NameIdTemplate.GetName_By_NameId (commonItemTemplate.nameId) + "x" + _item.num;
	}

    #endregion


    void OnDisable()
    {
        if (UtilityTool.m_ApplicationIsQuitting) return;

        if (TimeHelper.Instance.IsTimeCalcKeyExist("MainCityUIRBTimeCalc"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");
        }
    }

	public void EndRB()
	{
//		if(!isOpen)
//		{
//			ClientMain.addPopUP(40, 2, "6", null);
//			ClientMain.addPopUP(40, 2, "12", null);
//			ClientMain.addPopUP(40, 2, "1210", null);
//			ClientMain.addPopUP(40, 2, "250", null);
//		}
		if(m_ObjPropUse.activeSelf)
		{
			if(!MainCityUIRB.isOpen)
			{
				m_ObjPropUse.transform.localPosition = new Vector3(-190, 50, 0);
			}
//			else
//			{
//				m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);
//			}
		}

//		if(Global.m_iOpenFunctionIndex != -1)
//		{
//
//		}
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("RB_OpenButton") != -1)
		{
			isOpen = !isOpen;
			if(isOpen)
			{
				m_SpriteOpen.spriteName = "Open";
				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 150, -(640 + ClientMain.m_iMoveY * 2 - 50), -100, 0);
				if(m_ObjPropUse.activeSelf)
				{
					m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);
//					if(MainCityUIRB.isOpen)
//					{
//						m_ObjPropUse.transform.localPosition = new Vector3(-190, 50, 0);
//					}
//					else
//					{
//
//					}
				}
			}
			else
			{
				m_SpriteOpen.spriteName = "Close";
//				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 50 + 150, -(640 + ClientMain.m_iMoveY * 2 - 50), 100, 0);
				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 50 + (MainCityUI.m_MainCityUI.m_MainCityListButton_RB.m_listFunctionButtonManager.Count * 100), -(640 + ClientMain.m_iMoveY * 2 - 50), -100, 0);
			}
			MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setPos();
			MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setMove(EndRB);
		}
		else if(ui.name.IndexOf("RB_UseProp") != -1)
		{
			m_ObjPropUse.SetActive(false);
			m_listPropUseID.RemoveAt(0);
			m_listPropUseNum.RemoveAt(0);
			if(m_listPropUseID.Count > 0)
			{
				setPropUse(m_listPropUseID[0], m_listPropUseNum[0]);
			}
		}
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
}