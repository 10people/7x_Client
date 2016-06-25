using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class RootController : MonoBehaviour, SocketListener
    {
        #region Call Interface

        public static int m_StoredStartModuleIndex;

        public static void CreateRankWindow(int startModuleIndex = 4)
        {
            m_StoredStartModuleIndex = startModuleIndex;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.RANK_WINDOW),
                        RankWindowLoadBack);
        }

        private static void RankWindowLoadBack(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject temp = (GameObject)Instantiate(p_object) as GameObject;
            RootController tempController = temp.GetComponent<RootController>();
            tempController.m_startModuleIndex = m_StoredStartModuleIndex;

            MainCityUI.TryAddToObjectList(temp);
        }

        #endregion

        public GameObject FloatButtonPrefab;

        public void RequestAllInModule(int pageID, int nationID)
        {
            m_ModuleControllerList[CurrentModule].RequestAll(pageID, nationID);
        }

        public EventHandler CloseEventHandler;
        public ScaleEffectController m_ScaleEffectController;

        public void OnCloseClick(GameObject go)
        {
            m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        private void DoCloseWindow()
        {
            MainCityUI.TryRemoveFromObjectList(gameObject);
            Destroy(gameObject);
        }

        #region Nation Toggles

        public TogglesControl m_NationTogglesControl;

        private readonly List<int> NaionList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        public readonly List<string> NaionStringList = new List<string>() { "周 ", "齐", "楚", "燕", "韩", "赵", "魏", "秦" };

        public int CurrentNation
        {
            get { return currentNation; }
            set
            {
                if (!NaionList.Contains(value))
                {
                    Debug.LogError("Not correct nation set.");
                    return;
                }
                currentNation = value;
            }
        }

        private int currentNation;

        private void OnNationsClick(int index)
        {
            m_NationTogglesControl.OnToggleClick(index);
            m_NationTogglesControl.TogglesEvents.ForEach(item =>
            {
                var temp = item.GetComponentsInChildren<UILabelType>(true);
                if (temp.Any())
                {
                    temp.First().setType(11);
                }
            });

            var temp2 = m_NationTogglesControl.TogglesEvents[index].GetComponentsInChildren<UILabelType>(true);
            if (temp2.Any())
            {
                temp2.First().setType(10);
            }
            CurrentNation = index;
            RequestAllInModule(1, index);
        }

        #endregion

        #region Module Toggles

        public List<ModuleController> m_ModuleControllerList = new List<ModuleController>();

        public TogglesControl m_ModuleTogglesControl;

        private readonly List<int> ModuleList = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };

        public int CurrentModule
        {
            get { return currentModule; }
            set
            {
                if (!ModuleList.Contains(value))
                {
                    Debug.LogError("Not correct module set.");
                    return;
                }
                currentModule = value;
            }
        }

        private int currentModule;

        public void OnModulesClick(int index)
        {
            m_ModuleTogglesControl.OnToggleClick(index);
            m_ModuleTogglesControl.TogglesEvents.ForEach(item => item.GetComponentsInChildren<UILabelType>(true).First().setType(11));
            m_ModuleTogglesControl.TogglesEvents[index].GetComponentsInChildren<UILabelType>(true).First().setType(10);
            m_ModuleControllerList.ForEach(item => item.gameObject.SetActive(false));
            m_ModuleControllerList[index].gameObject.SetActive(true);
            currentModule = index;
            RequestAllInModule(1, 0);

            //Reset nation toggle.
            m_NationTogglesControl.OnToggleClick(0);
        }

        #endregion

        public GameObject TopLeftAnchor;

        private int m_startModuleIndex = 6;

        private void Start()
        {
            //Initialize
            CurrentNation = 0;
            CurrentModule = m_startModuleIndex;
            OnModulesClick(m_startModuleIndex);
        }

        private void Awake()
        {
            SocketTool.RegisterSocketListener(this);

            //Init nation toggles.
            m_NationTogglesControl.TogglesEvents.ForEach(item => item.m_Handle += OnNationsClick);

            //Init module toggles.
            if (m_ModuleControllerList.Count != m_ModuleTogglesControl.TogglesEvents.Count)
            {
                Debug.LogError("Error toggle num when initialize rank sys.");
                return;
            }
            m_ModuleTogglesControl.TogglesEvents.ForEach(item => item.m_Handle += OnModulesClick);

            CloseEventHandler.m_click_handler += OnCloseClick;

            //Load float button
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);

            MainCityUI.setGlobalBelongings(gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);
            MainCityUI.setGlobalTitle(TopLeftAnchor, "排行榜", 0, 0);
        }

        void FloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            FloatButtonPrefab = p_object as GameObject;
        }

        private void OnDestroy()
        {
            SocketTool.UnRegisterSocketListener(this);

            m_NationTogglesControl.TogglesEvents.ForEach(item => item.m_Handle -= OnNationsClick);
            m_ModuleTogglesControl.TogglesEvents.ForEach(item => item.m_Handle -= OnModulesClick);

            CloseEventHandler.m_click_handler -= OnCloseClick;
        }

        public string SelectedAllianceName;
        public AlliancePlayerResp m_AlliancePlayerResp;

        public bool OnSocketEvent(QXBuffer p_message)
        {
            if (p_message != null)
            {
                switch (p_message.m_protocol_index)
                {
                    //Black list.
                    case ProtoIndexes.S_Join_BlackList_Resp:
                        {
                            object joinToBlackListRespObject = new BlacklistResp();
                            if (SocketHelper.ReceiveQXMessage(ref joinToBlackListRespObject, p_message,
                                ProtoIndexes.S_Join_BlackList_Resp))
                            {
                                BlacklistResp tempJoinBlacklistResp = joinToBlackListRespObject as BlacklistResp;

                                //shield if return 0.
                                if (tempJoinBlacklistResp.result == 0)
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ShieldSucceedCallBack);

                                    //Refresh black list after shield succeed.
                                    BlockedData.Instance().RequestBlockedInfo();
                                }
                                else if (tempJoinBlacklistResp.result == 1)
                                {
                                    Debug.LogWarning("Trying to add to black list fail.");
                                }
                                return true;
                            }
                            return false;
                        }
                }
            }
            return false;
        }

        public string ShieldName = "";

        public void ShieldSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "您已成功添加" + ShieldName + "到屏蔽列表", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void PlayerNotExistCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "此玩家不存在！", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void PlayerNotInRankCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 LanguageTemplate.GetText(2720), null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void AllianceNotExistCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "此联盟不存在！", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void AllianceNotInRankCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 "该联盟暂未上榜！", null,
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }
    }
}
