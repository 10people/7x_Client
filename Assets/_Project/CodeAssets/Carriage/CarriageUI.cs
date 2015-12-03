using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageUI : MonoBehaviour
    {
        public RootManager m_RootManager;
        public UILabel PlayerNameLabel;
        public UILabel AllianceNameLabel;
        public UISprite TitleSprite;

        private void OnReturnClick(GameObject go)
        {
            CityGlobalData.m_isJieBiaoScene = false;
            PlayerSceneSyncManager.Instance.ExitCarriage();
        }

        #region Anchor Buttom Left

        public Joystick m_Joystick;

        #endregion

        #region OpenClose Controller

        public EventHandler m_OpenBtnHandler;
        public EventHandler m_CloseBtnHandler;
        public EventHandler m_ReturnBtnHandler;
        public EventHandler m_ChatBtnHandler;
        private const float OpenXPos = 499;
        private const float CloseXPos = -123.01f;
        private const float Duration = 0.5f;

        public UISprite BgSprite;
        public GameObject MainObject;

        public static bool IsOpened;

        private void OnOpenClick(GameObject go)
        {
            m_OpenBtnHandler.gameObject.SetActive(false);

            MainObject.transform.localPosition = new Vector3(CloseXPos, MainObject.transform.localPosition.y, MainObject.transform.localPosition.z);

            iTween.MoveTo(MainObject, iTween.Hash(
                "position", new Vector3(OpenXPos, MainObject.transform.localPosition.y, MainObject.transform.localPosition.z),
                "time", Duration,
                "easeType", "easeOutBack",
                "islocal", true,
                "oncomplete", "OnOpenComplete",
                "oncompletetarget", gameObject));
        }

        void OnOpenComplete()
        {
            m_CloseBtnHandler.gameObject.SetActive(true);
            IsOpened = true;

            //Refresh carriage ui item state ui when open complete.
            m_CarriageUiItemControllers.ForEach(item => item.RefreshStateUI());
        }

        private void OnCloseClick(GameObject go)
        {
            m_CloseBtnHandler.gameObject.SetActive(false);
            IsOpened = false;

            MainObject.transform.localPosition = new Vector3(OpenXPos, MainObject.transform.localPosition.y, MainObject.transform.localPosition.z);

            iTween.MoveTo(MainObject, iTween.Hash(
                "position", new Vector3(CloseXPos, MainObject.transform.localPosition.y, MainObject.transform.localPosition.z),
                "time", Duration,
                "easeType", "easeInBack",
                "islocal", true,
                "oncomplete", "OnCloseComplete",
                "oncompletetarget", gameObject));
        }

        void OnCloseComplete()
        {
            m_OpenBtnHandler.gameObject.SetActive(true);
        }

        #endregion

        #region Carriage List

        public UIGrid m_Grid;

        public List<CarriageUIItemController> m_CarriageUiItemControllers = new List<CarriageUIItemController>();

        /// <summary>
        /// init left top anchor, clear all and initialize.
        /// </summary>
        public void InitCarriageGrid()
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.CARRIAGE_UI_ITEM), OnInitCarriageItemLoadCallBack);
        }

        private YabiaoJunZhuInfo m_yabiaoJunZhuInfo;

        public void InitACarriage(YabiaoJunZhuInfo l_yabiaoJunZhuInfo)
        {
            m_yabiaoJunZhuInfo = l_yabiaoJunZhuInfo;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.CARRIAGE_UI_ITEM), OnAddACarriageItemLoadCallBack);
        }

        /// <summary>
        /// Refresh a item in left top anchor grid.
        /// </summary>
        public void RefreshCarriageUIItem(BiaoCheState l_biaoCheState)
        {
            List<CarriageUIItemController> controllers = m_CarriageUiItemControllers.Where(controller => controller.m_YabiaoJunZhuInfo.junZhuId == l_biaoCheState.junZhuId).ToList();
            if (controllers == null || controllers.Count != 1)
            {
                Debug.LogWarning("Fail when refresh a carriage, kingId:" + l_biaoCheState.junZhuId + ", this situation is normal when carriage destroyed.");
                return;
            }

            controllers[0].m_YabiaoJunZhuInfo.state = l_biaoCheState.state;
            controllers[0].m_YabiaoJunZhuInfo.hp = l_biaoCheState.hp;
            controllers[0].m_YabiaoJunZhuInfo.worth = l_biaoCheState.worth;
            controllers[0].m_YabiaoJunZhuInfo.usedTime = l_biaoCheState.usedTime;
            controllers[0].m_YabiaoJunZhuInfo.baohuCD = l_biaoCheState.baohuCD;
            controllers[0].SetThis();
        }

        private void OnAddACarriageItemLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            ExecuteACarriage(m_yabiaoJunZhuInfo, loadedObject);
            m_Grid.Reposition();
        }

        private void ExecuteACarriage(YabiaoJunZhuInfo l_yabiaoJunZhuInfo, Object loadedObject)
        {
            var tempObject = Instantiate(loadedObject) as GameObject;
            TransformHelper.ActiveWithStandardize(m_Grid.transform, tempObject.transform);

            var tempController = tempObject.GetComponent<CarriageUIItemController>();

            tempController.m_YabiaoJunZhuInfo = l_yabiaoJunZhuInfo;
            tempController.m_RootManager = m_RootManager;
            tempController.m_CarriageUi = this;
            tempController.SetThis();
            m_CarriageUiItemControllers.Add(tempController);
        }

        private void OnInitCarriageItemLoadCallBack(ref WWW www, string path, Object loadedObject)
        {
            //if (CarriageMsgManager.Instance.s_YabiaoJunZhuList == null || CarriageMsgManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList == null)
            //{
            //    return;
            //}

            //for (int i = 0; i < CarriageMsgManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList.Count; i++)
            //{
            //    YabiaoJunZhuInfo temp = CarriageMsgManager.Instance.s_YabiaoJunZhuList.yabiaoJunZhuList[i];

            //    ExecuteACarriage(temp, loadedObject);
            //}
            //m_Grid.Reposition();
        }

        #endregion

        #region Buttom Right/ Carriage Info

        public EventHandler m_SwitchSkillHandler;
        public EventHandler m_BattleHandler;

        ///// <summary>
        ///// carriage used for battle.
        ///// </summary>
        //private YabiaoJunZhuInfo m_selectedYabiaoJunZhuInfo
        //{
        //    get { return m_NavigationYabiaoJunZhuInfo ?? m_nearestYabiaoJunZhuInfo; }
        //}

        ///// <summary>
        ///// the carriage nearest to player.
        ///// </summary>
        //private YabiaoJunZhuInfo m_nearestYabiaoJunZhuInfo
        //{
        //    get
        //    {
        //        return
        //            m_RootManager.m_CarriageManager.m_CarriageControllers.Aggregate(
        //                (i, j) =>
        //                    Vector3.Distance(i.transform.position, m_RootManager.m_CarriagePlayerController.transform.position) > Vector3.Distance(j.transform.position, m_RootManager.m_CarriagePlayerController.transform.position)
        //                        ? j : i)
        //                        .m_YabiaoJunZhuInfo;
        //    }
        //}

        /// <summary>
        /// the carriage info navigate to.
        /// </summary>
        public YabiaoJunZhuInfo m_NavigationYabiaoJunZhuInfo
        {
            get { return m_navigationYabiaoJunZhuInfo; }
        }
        private YabiaoJunZhuInfo m_navigationYabiaoJunZhuInfo;

        private void OnSwitchSkillClick(GameObject go)
        {
            //Disable all effect.
            if (!MiBaoGlobleData.Instance().GetEnterChangeMiBaoSkill_Oder())
            {
                return;
            }
            ClearAllEffect();

            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadBack);
        }

        void LoadBack(ref WWW p_www, string p_path, Object p_object)
        {
            //GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
            //TransformHelper.ActiveWithStandardize(transform, mChoose_MiBao.transform);
            //mChoose_MiBao.SetActive(true);

            //ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();
            //mChangeMiBaoSkill.Init(7, CarriageMsgManager.Instance.s_YabiaoJunZhuList.gongjiZuHeId);
        }

        private bool CheckCanRob()
        {
            //if (m_selectedYabiaoJunZhuInfo == null)
            //{
            //    Debug.LogError("Cannot enter battle cause no carriage selected.");
            //    return false;
            //}

            ////Check.
            //List<CarriageCultureController> controllers = m_RootManager.m_CarriageManager.m_CarriageControllers.Where(item => item.m_YabiaoJunZhuInfo.junZhuId == m_selectedYabiaoJunZhuInfo.junZhuId).ToList();
            //if (controllers == null || controllers.Count != 1)
            //{
            //    Debug.LogError("Cannot find specific carriage:" + m_selectedYabiaoJunZhuInfo.junZhuId + ", cancel open battle.");
            //    return false;
            //}

            //if (m_BattleHandler.GetComponent<UISprite>().color == Color.grey)
            //{
            //    Debug.LogWarning("===========cancel open battle cause not close enough.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_57));
            //    return false;
            //}

            //if (m_selectedYabiaoJunZhuInfo.junZhuId == JunZhuData.Instance().m_junzhuInfo.id)
            //{
            //    Debug.LogWarning("===========cancel open battle cause carriage is mine.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_55));
            //    return false;
            //}

            //if (!AllianceData.Instance.IsAllianceNotExist && AllianceData.Instance.g_UnionInfo != null && m_selectedYabiaoJunZhuInfo.lianMengName == AllianceData.Instance.g_UnionInfo.name)
            //{
            //    Debug.LogWarning("===========cancel open battle cause carriage is alliance.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_56));
            //    return false;
            //}

            //if (m_selfRemainTimes <= 0)
            //{
            //    Debug.LogWarning("===========cancel open battle cause not enough times.");
            //    //ShowInfo("你的劫镖次数已用尽，不能劫镖");
            //    CarriageMsgManager.Instance.OnBuyRobTimesConfirm();
            //    return false;
            //}

            //if (m_selfProtectTime > 0)
            //{
            //    Debug.LogWarning("===========cancel open battle cause king in cd.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_52));
            //    return false;
            //}

            //if (m_selectedYabiaoJunZhuInfo.state == 30)
            //{
            //    Debug.LogWarning("===========cancel open battle cause carriage in protect.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_53));
            //    return false;
            //}

            //if (m_selectedYabiaoJunZhuInfo.state == 20)
            //{
            //    Debug.LogWarning("===========cancel open battle cause carriage in fight.");
            //    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_54));
            //    return false;
            //}

            return true;
        }

        private void OnBattleClick(GameObject go)
        {
            //if (!CheckCanRob())
            //{
            //    return;
            //}

            //EnterBattleField.EnterBattleCarriage(m_selectedYabiaoJunZhuInfo.junZhuId);
            //RootManager.m_PlayerLastLocalPosition = m_RootManager.m_CarriagePlayerController.transform.localPosition;
            //RootManager.KingInfoBattleWith = m_selectedYabiaoJunZhuInfo;
            //RootManager.KingInfoBattleWith.junZhuId = -RootManager.KingInfoBattleWith.junZhuId;
        }

        public UILabel m_CarriageInfoLabel;
        public GameObject m_CarriageInfoObject;
        private readonly Vector2 popLabelDistance = new Vector2(0, 25);
        private const float popLabelDuration = 2.0f;

        private void ShowInfo(string str)
        {
            m_CarriageInfoLabel.text = str;

            PopUpLabelTool.Instance().AddPopLabelWatcher(m_CarriageInfoObject, Vector3.zero, popLabelDistance, iTween.EaseType.easeOutBack, -1.0f, iTween.EaseType.linear, popLabelDuration);
        }

        public void SetCarriageInfo(YabiaoJunZhuInfo l_yabiaoJunZhuInfo)
        {
            m_navigationYabiaoJunZhuInfo = l_yabiaoJunZhuInfo;
        }

        private float m_robButtonsLastCalcTime;

        private void SetBattleColor(bool isEnable)
        {
            m_BattleHandler.GetComponent<UISprite>().color = isEnable ? Color.white : Color.grey;
        }

        /// <summary>
        /// Check switch skill, show effect if can select and no mibao selected.
        /// </summary>
        public void RefreshMibaoSkillEffect()
        {
            //List<MibaoGroup> mibaoGroup = MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup;
            //List<MibaoGroup> activeGroup = new List<MibaoGroup>();

            //for (int i = 0; i < mibaoGroup.Count; i++)
            //{
            //    List<MibaoInfo> miBaoInfoList = new List<MibaoInfo>();

            //    for (int j = 0; j < mibaoGroup[i].mibaoInfo.Count; j++)
            //    {
            //        if (mibaoGroup[i].mibaoInfo[j].level > 0 && !mibaoGroup[i].mibaoInfo[j].isLock)
            //        {
            //            miBaoInfoList.Add(mibaoGroup[i].mibaoInfo[j]);
            //        }
            //    }
            //    if (miBaoInfoList.Count >= 2)
            //    {
            //        activeGroup.Add(mibaoGroup[i]);
            //    }
            //}

            //if (activeGroup.Count > 0)
            //{
            //    List<int> zuHeIdList = new List<int>();
            //    for (int i = 0; i < activeGroup.Count; i++)
            //    {
            //        zuHeIdList.Add(activeGroup[i].zuheId);
            //    }

            //    if (zuHeIdList.Contains(CarriageMsgManager.Instance.s_YabiaoJunZhuList.gongjiZuHeId))
            //    {
            //        UI3DEffectTool.Instance().ClearUIFx(m_SwitchSkillHandler.gameObject);
            //    }
            //    else
            //    {
            //        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_SwitchSkillHandler.gameObject, EffectTemplate.getEffectTemplateByEffectId(100005).path);
            //    }
            //}
            //else
            //{
            //    UI3DEffectTool.Instance().ClearUIFx(m_SwitchSkillHandler.gameObject);
            //}
        }

        /// <summary>
        /// Refresh all ui effect in carriage ui.
        /// </summary>
        public void RefreshAllEffect()
        {
            RefreshMibaoSkillEffect();

            m_CarriageUiItemControllers.ForEach(item => item.RefreshStateUI());
        }

        public void ClearAllEffect()
        {
            UI3DEffectTool.Instance().ClearUIFx(m_SwitchSkillHandler.gameObject);
            m_CarriageUiItemControllers.ForEach(item => UI3DEffectTool.Instance().ClearUIFx(item.m_StateObject));
        }

        #endregion

        #region Top Left/ Carriage Info

        public UILabel m_SelfRemainTimesLabel;
        public UILabel m_SelfProtectTimeLabel;

        private int m_selfRemainTimes;

        /// <summary>
        /// use this to control king in cd or not.
        /// </summary>
        private float m_selfProtectTime;

        private float m_topLeftInfoLastCalcTime;

        /// <summary>
        /// Set king remain time and protect time.
        /// </summary>
        public void SetKingInfo(int l_remainTimes, float l_protectTime)
        {
            m_selfRemainTimes = l_remainTimes;
            m_selfProtectTime = l_protectTime;
            DoSetKingInfoLabel();
        }

        private void DoSetKingInfoLabel()
        {
            m_SelfRemainTimesLabel.text = ColorTool.Color_Gold_ffb12a + "剩余次数：" + "[-]" + (m_selfRemainTimes > 0 ? ColorTool.Color_Gold_ffb12a : ColorTool.Color_Red_c40000) + m_selfRemainTimes + "[-]";
            m_SelfProtectTimeLabel.text = ColorTool.Color_Gold_ffb12a + "打劫冷却：" + "[-]" + (m_selfProtectTime > 0 ? ColorTool.Color_Red_c40000 : ColorTool.Color_Gold_ffb12a) + m_selfProtectTime + "[-]" + ColorTool.Color_Gold_ffb12a + "秒" + "[-]";
        }

        void Update()
        {
            if (m_selfProtectTime > 0 && Time.realtimeSinceStartup - m_topLeftInfoLastCalcTime > 1.0f)
            {
                m_topLeftInfoLastCalcTime = Time.realtimeSinceStartup;

                m_selfProtectTime--;

                DoSetKingInfoLabel();
            }

            //if (Time.realtimeSinceStartup - m_robButtonsLastCalcTime > 1.0f)
            //{
            //    m_robButtonsLastCalcTime = Time.realtimeSinceStartup;

            //    var tempList =
            //        m_RootManager.m_CarriageManager.m_CarriageControllers.Where(
            //            item => item.m_YabiaoJunZhuInfo.junZhuId == m_selectedYabiaoJunZhuInfo.junZhuId).ToList();
            //    if (tempList == null || tempList.Count != 1)
            //    {
            //        return;
            //    }

            //    SetBattleColor(Vector3.Distance(m_RootManager.m_CarriagePlayerController.transform.position, tempList[0].transform.position) < 5);
            //}
        }

        #endregion

        #region Chat Click Control

        public GameObject m_ChatRedAlert;

        private GameObject chatUI;
        /// <summary>
        /// Can only be called when chatUI is not null and actived.
        /// </summary>
        private ChatWindow chatWindow
        {
            get
            {
                if (chatWindow_Value != null)
                {
                    return chatWindow_Value;
                }
                else
                {
                    return chatWindow_Value = chatUI.GetComponentInChildren<ChatWindow>();
                }
            }
        }
        private ChatWindow chatWindow_Value;

        private void OnChatClick(GameObject go)
        {
            ChatWindow.s_ChatWindow.m_ChatChannelFrameList.ForEach(item => item.m_ChatBaseDataHandler.ClearUnUsedChatStructList());

            chatUI.SetActive(true);
            chatWindow.m_RootChatOpenObject = m_ChatBtnHandler.gameObject;
            chatWindow.m_ChatOpenCloseController.OnOpenWindowClick(gameObject);
        }

        private void ChatWindowLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            chatUI = Instantiate(p_object) as GameObject;
            DontDestroyOnLoad(chatUI);
            chatWindow.m_RootChatOpenObject = m_ChatBtnHandler.gameObject;

            chatUI.SetActive(false);
        }

        private void ChatWindowOpenCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            chatUI = Instantiate(p_object) as GameObject;
            DontDestroyOnLoad(chatUI);
            chatWindow.m_RootChatOpenObject = m_ChatBtnHandler.gameObject;
            chatWindow.m_ChatOpenCloseController.OnOpenWindowClick(gameObject);
        }

        #endregion

        void Start()
        {
            if (chatUI == null)
            {
                if (ChatWindow.s_ChatWindow != null && ChatWindow.s_ChatWindow.gameObject != null)
                {
                    chatUI = ChatWindow.s_ChatWindow.ChatRoot.gameObject;
                }
                else
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHAT_WINDOW),
                        ChatWindowLoadCallBack);
                }
            }

            //init player name infos.

            if (JunZhuData.Instance().m_junzhuInfo.vipLv > 0)
            {
                PlayerNameLabel.text = MyColorData.getColorString(1, "V" + JunZhuData.Instance().m_junzhuInfo.vipLv) + " " + MyColorData.getColorString(9, "[b]" + JunZhuData.Instance().m_junzhuInfo.name + "[/b]");
            }
            else
            {
                PlayerNameLabel.text = MyColorData.getColorString(9, "[b]" + JunZhuData.Instance().m_junzhuInfo.name + "[/b]");
            }

            if (!AllianceData.Instance.IsAllianceNotExist && AllianceData.Instance.g_UnionInfo != null)
            {
                AllianceNameLabel.text = MyColorData.getColorString(12, "<" + AllianceData.Instance.g_UnionInfo.name + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AllianceData.Instance.g_UnionInfo.identity);
            }
            else
            {
                AllianceNameLabel.text = MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT));
            }

            if (JunZhuData.m_iChenghaoID != -1)
            {
                TitleSprite.gameObject.SetActive(true);
                TitleSprite.spriteName = JunZhuData.m_iChenghaoID.ToString();
            }
            else
            {
                TitleSprite.gameObject.SetActive(false);
            }
        }

        void Awake()
        {
            m_SwitchSkillHandler.m_handler += OnSwitchSkillClick;
            m_BattleHandler.m_handler += OnBattleClick;
            m_OpenBtnHandler.m_handler += OnOpenClick;
            m_CloseBtnHandler.m_handler += OnCloseClick;
            m_ReturnBtnHandler.m_handler += OnReturnClick;
            m_ChatBtnHandler.m_handler += OnChatClick;
        }

        void OnDestroy()
        {
            m_SwitchSkillHandler.m_handler -= OnSwitchSkillClick;
            m_BattleHandler.m_handler -= OnBattleClick;
            m_OpenBtnHandler.m_handler -= OnOpenClick;
            m_CloseBtnHandler.m_handler -= OnCloseClick;
            m_ReturnBtnHandler.m_handler -= OnReturnClick;
            m_ChatBtnHandler.m_handler += OnChatClick;
        }
    }
}