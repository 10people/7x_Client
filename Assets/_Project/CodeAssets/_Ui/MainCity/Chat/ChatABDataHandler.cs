using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.Linq;

public class ChatABDataHandler : ChatBaseDataHandler
{
    public ChatABWindow m_ChatAbWindow;

    #region ChatData

    /// <summary>
    /// Channel config, mention that base chat viewer may show kinds of channels.
    /// </summary>
    public ChatPct.Channel m_Channel;

    #endregion

    public void OnABDataReceived()
    {
        m_ChatAbWindow.m_ChatBaseSendController.m_SendedChatStruct.isReceived = true;
        Refresh(2);

        if (storedChatStructList != null && storedChatStructList.Count > 0)
        {
            m_ChatAbWindow.m_AllianceBattleUi.SetLastChat(storedChatStructList.Last().m_ChatPct.content);
        }
    }

    private void SelfItemLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        PoolManagerListController.Instance.ItemDic.Add("ChatDataSelfItem", (GameObject)p_object);
        PoolManagerListController.Instance.Initialize();
    }

    private void OtherItemLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        PoolManagerListController.Instance.ItemDic.Add("ChatDataOtherItem", (GameObject)p_object);
        PoolManagerListController.Instance.Initialize();
    }

    private void OnDragAreaClick(GameObject go)
    {

    }

    new void OnDestroy()
    {
        m_DataReceivedEvent -= OnABDataReceived;

        base.OnDestroy();
    }

    new void Awake()
    {
        base.Awake();

        if (!PoolManagerListController.Instance.ItemDic.ContainsKey("ChatDataSelfItem"))
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHATITEM_SELF),
                SelfItemLoadCallback);
        }

        if (!PoolManagerListController.Instance.ItemDic.ContainsKey("ChatDataOtherItem"))
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHATITEM_OTHER),
                OtherItemLoadCallback);
        }

        ChatSelfLogItemStr = "ChatDataSelfItem";
        ChatOtherLogItemStr = "ChatDataOtherItem";

        m_Channel = ChatPct.Channel.LIANMENG;
        m_ChannelList.AddRange(new List<ChatPct.Channel>() { m_Channel, ChatPct.Channel.SYSTEM });

        m_DataReceivedEvent += OnABDataReceived;
    }
}
