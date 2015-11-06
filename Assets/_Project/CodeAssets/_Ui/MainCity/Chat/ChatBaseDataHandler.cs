using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class ChatBaseDataHandler : MonoBehaviour, SocketListener
{
    public string ChatSelfLogItemStr;
    public string ChatOtherLogItemStr;

    #region Components

    public ChatBaseWindow m_ChatBaseWindow;

    public UIScrollView m_ScrollView;
    public UIScrollBar m_ScrollBar;
    public UITable m_Table;

    public UIEventListener m_DragAreaListener;

    public float ScrollViewWidth
    {
        get
        {
            if (scrollViewWidth == -1)
            {
                scrollViewWidth = m_ScrollView.GetComponent<UIPanel>().width;
            }
            return scrollViewWidth;
        }
    }

    private float scrollViewWidth = -1;

    #endregion

    #region ChatStruct

    public class ChatStruct
    {
        public bool isReceived;
        public ChatPct m_ChatPct = new ChatPct();
        public ChatLogItem m_ChatLogItem;
    }

    public List<ChatStruct> storedChatStructList = new List<ChatStruct>();

    public int ChatStructListRestrictNum = 50;

    /// <summary>
    /// Restrict chat struct list num.
    /// </summary>
    public void RestrictChatStructListNum()
    {
        while (storedChatStructList.Count > ChatStructListRestrictNum)
        {
            PoolManagerListController.Instance.ReturnItem(storedChatStructList[0].m_ChatLogItem.IsLeftMode ? "ChatDataOtherItem" : "ChatDataSelfItem", storedChatStructList[0].m_ChatLogItem.gameObject);
            storedChatStructList[0].m_ChatLogItem = null;
            storedChatStructList.RemoveAt(0);
        }
    }

    /// <summary>
    /// Clear all struct list, must call in derived class.
    /// </summary>
    public virtual void ClearUnUsedChatStructList()
    {
        storedChatStructList.ForEach(item =>
        {
            if (item.m_ChatLogItem != null)
            {
                PoolManagerListController.Instance.ReturnItem(item.m_ChatLogItem.IsLeftMode ? "ChatDataOtherItem" : "ChatDataSelfItem", item.m_ChatLogItem.gameObject);
            }
        });

        storedChatStructList.ForEach(item => item.m_ChatLogItem = null);
        storedChatStructList.Clear();
    }

    #endregion

    #region Config

    /// <summary>
    /// Show these kinds of channel in this chat view.
    /// </summary>
    public List<ChatPct.Channel> m_ChannelList = new List<ChatPct.Channel>();

    public delegate void VoidDelegate();
    public event VoidDelegate m_DataReceivedEvent;

    #endregion

    public void RemoveChatStruct(ChatLogItem item)
    {
        if (!storedChatStructList.Contains(item.m_ChatStruct))
        {
            Debug.LogWarning("[ERROR:]can not find chat struct, remove failed.");
            return;
        }

        item.m_ChatStruct.m_ChatLogItem = null;
        storedChatStructList.Remove(item.m_ChatStruct);
        PoolManagerListController.Instance.ReturnItem(item.IsLeftMode ? "ChatDataOtherItem" : "ChatDataSelfItem", item.gameObject);
    }

    /// <summary>
    /// Refresh chat struct table.
    /// </summary>
    /// <param name="index">0: do not go to end, 1: go to end if scroll bar value > 0.9, 2:always go to end</param>
    /// <param name="isSimpleMode">disable buttons click on simple mode</param>
    public void Refresh(int index, bool isSimpleMode = false)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cancel refresh in chat window cause chat data controller not active.");
            return;
        }

        StartCoroutine(DoRefresh(m_Table, storedChatStructList, index, isSimpleMode));
    }

    private IEnumerator DoRefresh(UITable table, List<ChatStruct> structList, int index, bool isSimpleMode)
    {
        object temp = new object();

        lock (temp)
        {
            //Sync scroll bar value and mShouldMove state.
            m_ScrollView.UpdateScrollbars(true);

            //Create table'children and add to objectList.
            foreach (var structItem in structList)
            {
                if (structItem.m_ChatLogItem == null)
                {
                    bool isLeft = structItem.m_ChatPct.senderName != JunZhuData.Instance().m_junzhuInfo.name;

                    var objectItem = PoolManagerListController.Instance.TakeItem(isLeft ? ChatOtherLogItemStr : ChatSelfLogItemStr);

                    var chatItemManager = objectItem.GetComponent<ChatLogItem>();
                    chatItemManager.IsSimpleMode = isSimpleMode;
                    chatItemManager.IsRefreshed = false;

                    chatItemManager.m_ChatBaseDataHandler = this;
                    chatItemManager.m_ChatStruct = structItem;

                    //Set data.
                    chatItemManager.SetData();
                    //Set label type.
                    chatItemManager.DetailLabel.overflowMethod = UILabel.Overflow.ResizeFreely;

                    objectItem.transform.parent = table.transform;
                    objectItem.transform.localPosition = Vector3.zero;
                    objectItem.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    objectItem.transform.localScale = Vector3.one;
                    objectItem.SetActive(true);

                    structItem.m_ChatLogItem = chatItemManager;
                }
            }

            if (structList.Count != 0 && structList.Where(item => item.m_ChatLogItem != null).Any())
            {
                var tempChatLogItemList = structList.Where(item => item.m_ChatLogItem != null).Select(item => item.m_ChatLogItem).ToList();

                //Wait for a frame for resize freely label auto set and anchor takes effect.
                yield return new WaitForEndOfFrame();

                tempChatLogItemList.ForEach(item => item.AdaptDetailText());
                //Wait for a frame for detail label width takes effect.
                yield return new WaitForEndOfFrame();

                tempChatLogItemList.ForEach(item => item.SetTransform());
                tempChatLogItemList.ForEach(item => item.AdaptButtons());

                for (int i = 0; i < tempChatLogItemList.Count; i++)
                {
                    if (!tempChatLogItemList[i].IsRefreshed)
                    {
                        if (i <= 0)
                        {
                            tempChatLogItemList[0].transform.localPosition = new Vector3(0, 0, tempChatLogItemList[0].transform.localPosition.z);
                        }
                        else
                        {
                            tempChatLogItemList[i].transform.localPosition = new Vector3(0, tempChatLogItemList[i - 1].transform.localPosition.y - tempChatLogItemList[i - 1].DetailBG.height - 10, tempChatLogItemList[i - 1].transform.localPosition.z);
                        }

                        //re-render to make sure NGUI render logs right, this is caused by idiot NGUI native bug.
                        tempChatLogItemList[i].gameObject.SetActive(false);
                        tempChatLogItemList[i].gameObject.SetActive(true);

                        tempChatLogItemList[i].IsRefreshed = true;
                    }
                }

                //Use pervious value cause refresh changed scroll value.
                if ((index == 1 && m_ScrollBar.value > 0.7f) || index == 2)
                {
                    //Sync scroll bar value and mShouldMove state.
                    m_ScrollView.UpdateScrollbars(true);

                    ResetScrollViewToEnd();
                }
            }
        }
    }

    /// <summary>
    /// To move scroll view to the end.
    /// </summary>
    public void ResetScrollViewToEnd()
    {
        if (m_ScrollView.shouldMoveVertically)
        {
            //Sync scroll bar value and mShouldMove state.
            m_ScrollView.UpdateScrollbars(true);

            m_ScrollBar.value = m_ScrollBar.value == 1.0f ? 0.99f : 1.0f;
        }
    }

    private const string sysSenderName = "【系统】";

    [HideInInspector]
    public bool IsResponseToKingDetailInfo = false;

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //recevie chat message.
                case ProtoIndexes.S_Send_Chat:
                    {
                        object targetObject = new ChatPct();
                        if (!UtilityTool.ReceiveQXMessage(ref targetObject, p_message, ProtoIndexes.S_Send_Chat))
                        {
                            return false;
                        }

                        ChatPct tempChatPct = targetObject as ChatPct;
                        OnChatMessageReceived(tempChatPct);
                        return true;
                    }
                case ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP:
                    {
                        if (!IsResponseToKingDetailInfo)
                        {
                            return false;
                        }
                        IsResponseToKingDetailInfo = false;

                        object junzhuResp = new JunZhuInfo();
                        if (UtilityTool.ReceiveQXMessage(ref junzhuResp, p_message, ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP))
                        {
                            m_JunzhuPlayerResp = junzhuResp as JunZhuInfo;

                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);

                            return true;
                        }
                        return false;
                    }
            }
        }
        return false;
    }

    public void OnChatMessageReceived(ChatPct tempChatPct)
    {
        //Check if in black list, abort execute message.
        if (BlockedData.Instance().m_BlockedInfoDic != null && BlockedData.Instance().m_BlockedInfoDic.Count != 0 &&
            BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.name).Contains(tempChatPct.senderName))
        {
            Debug.Log("Abort execute chat message cause black list shield, JunZhu name:" + tempChatPct.senderName);
            return;
        }

        ChatStruct tempChatStruct = new ChatStruct { m_ChatPct = tempChatPct, isReceived = true };

        //Stored data and execute restrict.
        if (m_ChannelList.Contains(tempChatPct.channel))
        {
            if (tempChatPct.channel == ChatPct.Channel.SYSTEM)
            {
                tempChatPct.senderName = sysSenderName;
            }

            storedChatStructList.Add(tempChatStruct);

            RestrictChatStructListNum();

            //Do message received delegate.
            if (m_DataReceivedEvent != null)
            {
                m_DataReceivedEvent();
            }
        }
    }

    #region King Detail Info

    private JunZhuInfo m_JunzhuPlayerResp;

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
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加好友", m_ButtonClick = OnAddFriendClick });
            }
            tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邮件", m_ButtonClick = OnMailClick });

            if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_JunzhuPlayerResp.junZhuId))
            {
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "屏蔽", m_ButtonClick = OnShieldClick });
            }
        }
        info.SetThis(tempKingInfo, tempConfigList);

        info.m_KingDetailEquipInfo.m_BagItemDic = m_JunzhuPlayerResp.equip.items.Where(item => item.buWei > 0).ToDictionary(item => KingDetailInfo.TransferBuwei(item.buWei));

        temp.SetActive(true);
    }

    private void OnAddFriendClick()
    {
        m_ChatBaseWindow.m_ChatUiBoxManager.AddFriendName = m_JunzhuPlayerResp.name;
        FriendOperationLayerManagerment.AddFriends((int)m_JunzhuPlayerResp.junZhuId);
    }

    private void OnMailClick()
    {
        //go to mail sys.
        EmailData.Instance().ReplyLetter(m_JunzhuPlayerResp.name);
    }

    private void OnShieldClick()
    {
        m_ChatBaseWindow.m_ChatUiBoxManager.JunZhuIDToBeShielded = m_JunzhuPlayerResp.junZhuId;
        m_ChatBaseWindow.m_ChatUiBoxManager.ShieldName = m_JunzhuPlayerResp.name;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), m_ChatBaseWindow.m_ChatUiBoxManager.ShieldCallBack);
    }

    #endregion

    /// <summary>
    /// Deactive all btns if touch drag area.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isPressed"></param>
    private void OnPressDragArea(GameObject go, bool isPressed)
    {
        if (isPressed)
        {
            storedChatStructList.Where(item => item.m_ChatLogItem != null).Select(item => item.m_ChatLogItem).ToList().ForEach(item => item.DeActiveAllButtonsExpectAlert());
        }
    }

    #region Mono

    public void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        if (m_DragAreaListener != null)
        {
            m_DragAreaListener.onPress -= OnPressDragArea;
        }
    }

    public void Awake()
    {
        SocketTool.RegisterSocketListener(this);

        if (m_DragAreaListener != null)
        {
            m_DragAreaListener.onPress += OnPressDragArea;
        }
    }

    #endregion
}
