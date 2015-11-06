using UnityEngine;
using System.Collections;
using qxmobile.protobuf;
using System.Collections.Generic;

public class ChatSimpleDataHandler : ChatBaseDataHandler
{
    public UISprite BgSprite;

    #region ChatData

    /// <summary>
    /// Channel config, mention that base chat viewer may show kinds of channels.
    /// </summary>
    public ChatPct.Channel m_Channel;

    #endregion

    public void OnSimpleDataReceived()
    {
        storedChatStructList.ForEach(item =>
        {
            item.m_ChatPct.isLveDuoHelp = false;
            item.m_ChatPct.isYBHelp = false;
        });

        Refresh(2, true);

        BgSprite.color = new Color(BgSprite.color.r, BgSprite.color.g, BgSprite.color.b, 1.0f);
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
        m_DataReceivedEvent -= OnSimpleDataReceived;

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

        var tempChannelList = AllianceData.Instance.IsAllianceNotExist ? new List<ChatPct.Channel>() { ChatPct.Channel.SHIJIE } : new List<ChatPct.Channel>() { ChatPct.Channel.SHIJIE, ChatPct.Channel.LIANMENG };
        m_ChannelList.AddRange(tempChannelList);
        m_ChannelList.Add(ChatPct.Channel.SYSTEM);

        m_DataReceivedEvent += OnSimpleDataReceived;
    }
}
