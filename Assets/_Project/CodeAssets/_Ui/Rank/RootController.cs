﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace Rank
{
    public class RootController : MonoBehaviour, SocketListener
    {
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

        private void OnModulesClick(int index)
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

        private void Start()
        {
            //Initialize
            CurrentNation = 0;
            CurrentModule = 0;
            OnModulesClick(0);

            //Reset nation toggle.
            m_NationTogglesControl.OnToggleClick(0);
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

            CloseEventHandler.m_handler += OnCloseClick;

            //Load float button
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
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

            CloseEventHandler.m_handler -= OnCloseClick;
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
                    //Alliance member info request.
                    case ProtoIndexes.RANKING_ALLIANCE_MEMBER_RESP:
                        {
                            object playerRespObject = new AlliancePlayerResp();
                            if (SocketHelper.ReceiveQXMessage(ref playerRespObject, p_message, ProtoIndexes.RANKING_ALLIANCE_MEMBER_RESP))
                            {
                                m_AlliancePlayerResp = playerRespObject as AlliancePlayerResp;

                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_MEMBER_WINDOW), AllianceMemberWindowCallBack);

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

        public void AllianceMemberWindowCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            var tempObject = Instantiate(p_object) as GameObject;
            var controller = tempObject.GetComponent<AllianceMemberController>();

            controller.m_AllianceName = SelectedAllianceName;
            controller.m_AlliancePlayerResp = m_AlliancePlayerResp;
            controller.m_RootController = this;

            controller.SetThis();
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
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_JunzhuPlayerResp.junZhuId))
                {
                    tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加入屏蔽", m_ButtonClick = Shield });
                }
                if (m_JunzhuPlayerResp.lianMeng != "无")
                {
                    tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "掠夺", m_ButtonClick = Rob });
                }
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
            AddFriendName = m_JunzhuPlayerResp.name;

            FriendOperationLayerManagerment.AddFriends((int)m_JunzhuPlayerResp.junZhuId);
        }

        /// <summary>
        /// king detail info window button call back.
        /// </summary>
        private void Shield()
        {
            ShieldName = m_JunzhuPlayerResp.name;

            JoinToBlacklist tempMsg = new JoinToBlacklist
            {
                junzhuId = m_JunzhuPlayerResp.junZhuId
            };
            SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
        }

        /// <summary>
        /// king detail info window button call back.
        /// </summary>
        private void Rob()
        {
			PlunderData.Instance.PlunderOpponent (PlunderData.Entrance.RANKLIST,m_JunzhuPlayerResp.junZhuId);
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
