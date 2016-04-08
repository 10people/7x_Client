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

        public static void CreateRankWindow(int startModuleIndex = 0)
        {
            m_StoredStartModuleIndex = startModuleIndex;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.RANK_WINDOW),
                        RankWindowLoadBack);
        }

        private static void RankWindowLoadBack(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject temp = (GameObject)Instantiate(p_object) as GameObject;
            RootController tempController = temp.GetComponent<RootController>();
            tempController.m_StartModuleIndex = m_StoredStartModuleIndex;

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
            CurrentNation = index;
            RequestAllInModule(1, index);
        }

        #endregion

        #region Module Toggles

        public List<ModuleController> m_ModuleControllerList = new List<ModuleController>();

        public TogglesControl m_ModuleTogglesControl;

        private readonly List<int> ModuleList = new List<int>() { 0, 1, 2, 3 };

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
            m_ModuleControllerList.ForEach(item => item.gameObject.SetActive(false));
            m_ModuleControllerList[index].gameObject.SetActive(true);
            currentModule = index;
            RequestAllInModule(1, 0);

            //Reset nation toggle.
            m_NationTogglesControl.OnToggleClick(0);
        }

        #endregion

        public GameObject TopLeftAnchor;

        [HideInInspector]
        public int m_StartModuleIndex = 0;

        private void Start()
        {
            //Initialize
            CurrentNation = 0;
            CurrentModule = 0;
            OnModulesClick(m_StartModuleIndex);
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
        public JunZhuInfo m_JunzhuPlayerResp;

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
                    case ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP:
                        {
                            {
                                object junzhuResp = new JunZhuInfo();
                                if (SocketHelper.ReceiveQXMessage(ref junzhuResp, p_message, ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP))
                                {
                                    m_JunzhuPlayerResp = junzhuResp as JunZhuInfo;

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);

                                    return true;
                                }
                                return false;
                            }
                        }
                }
            }
            return false;
        }

        /// <summary>
        /// king detail info window.
        /// </summary>
        /// <param name="p_www"></param>
        /// <param name="p_path"></param>
        /// <param name="p_object"></param>
        private void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            var temp = Instantiate(p_object) as GameObject;
            var info = temp.GetComponent<KingDetailInfo>();

            var tempKingInfo = new KingDetailInfo.KingDetailData()
            {
                RoleID = m_JunzhuPlayerResp.roleId,
                Attack = m_JunzhuPlayerResp.gongji,
                AllianceName = m_JunzhuPlayerResp.lianMeng,
                BattleValue = m_JunzhuPlayerResp.zhanli,
                Junxian = m_JunzhuPlayerResp.junxian,
                JunxianRank = m_JunzhuPlayerResp.junxianRank,
                KingName = m_JunzhuPlayerResp.name,
                Level = m_JunzhuPlayerResp.level,
                Money = m_JunzhuPlayerResp.gongjin,
                Life = m_JunzhuPlayerResp.remainHp,
                Protect = m_JunzhuPlayerResp.fangyu,
                Title = m_JunzhuPlayerResp.chenghao
            };

            var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>();
            if (m_JunzhuPlayerResp.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_JunzhuPlayerResp.junZhuId))
                {
                    tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加为好友", m_ButtonClick = AddFriend });
                }
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邮件", m_ButtonClick = Mail });
                if (m_JunzhuPlayerResp.lianMeng != "无")
                {
                    tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "掠夺", m_ButtonClick = Rob });
                }
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邀请入盟", m_ButtonClick = InviteToAlliance });
            }
            info.SetThis(tempKingInfo, tempConfigList);

            info.m_KingDetailEquipInfo.m_BagItemDic = m_JunzhuPlayerResp.equip.items.Where(item => item.buWei > 0).ToDictionary(item => KingDetailInfo.TransferBuwei(item.buWei));

            temp.SetActive(true);
        }

        /// <summary>
        /// king detail info window button call back.
        /// </summary>
        private void AddFriend()
        {
            if (FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_JunzhuPlayerResp.junZhuId))
            {
                ClientMain.m_UITextManager.createText("该玩家已经是您的好友！");
            }
            else
            {
                AddFriendName = m_JunzhuPlayerResp.name;

                FriendOperationData.Instance.AddFriends((FriendOperationData.AddFriendType)(-1), m_JunzhuPlayerResp.junZhuId, m_JunzhuPlayerResp.name);
            }
        }

        private void Mail()
        {
            NewEmailData.Instance().OpenEmail(NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE, m_JunzhuPlayerResp.name);
        }

        /// <summary>
        /// king detail info window button call back.
        /// </summary>
        private void Rob()
        {
            PlunderData.Instance.PlunderOpponent(PlunderData.Entrance.RANKLIST, m_JunzhuPlayerResp.junZhuId);
        }

        /// <summary>
        /// king detail info window button call back.
        /// </summary>
        private void InviteToAlliance()
        {
            AllianceInvite green = new AllianceInvite();
            green.junzhuId = m_JunzhuPlayerResp.junZhuId;

            MemoryStream t_stream = new MemoryStream();
            QiXiongSerializer q_serializer = new QiXiongSerializer();
            q_serializer.Serialize(t_stream, green);
            byte[] t_protof = t_stream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE, ref t_protof);
        }

        public string AddFriendName = "";

        public string ShieldName = "";

        public void ShieldSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 null, "您已成功添加" + ShieldName + "到屏蔽列表",
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void FindNoKingCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 null, "此玩家不存在！",
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }

        public void FindNoAllianceCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
            uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
            uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                 null, "此联盟不存在！",
                 null,
                 LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                 null);
        }
    }
}
