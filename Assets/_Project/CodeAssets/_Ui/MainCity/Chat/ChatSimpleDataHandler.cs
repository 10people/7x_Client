//#define TestChat
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

    #region Chat Click Control

    private GameObject m_chatRootObject;

    public void OnChatClick()
    {
		#if TestChat
        ChatWindow.s_ChatWindow.m_ChatChannelFrameList.ForEach(item => item.m_ChatBaseDataHandler.ClearUnUsedChatStructList());

        m_chatRootObject.SetActive(true);
        MainCityUI.TryAddToObjectList(m_chatRootObject);

        ChatWindow.s_ChatWindow.m_ChatOpenCloseController.OnOpenWindowClick();
		#endif
    }

    void Start()
    {
		#if TestChat
        if (ChatWindow.s_ChatWindow != null && ChatWindow.s_ChatWindow.gameObject != null)
        {
            m_chatRootObject = ChatWindow.s_ChatWindow.ChatRoot.gameObject;
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHAT_WINDOW),
                ChatWindowLoadCallBack);
        }
		#endif
    }

    private void ChatWindowLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        m_chatRootObject = Instantiate(p_object) as GameObject;
        DontDestroyOnLoad(m_chatRootObject);

        //Awake each frame manually.
        m_chatRootObject.GetComponentInChildren<ChatWindow>().m_ChatChannelFrameList.ForEach(item =>
        {
            var temp = item.m_ChatBaseDataHandler.GetComponent<ChatDataHandler>();
            if (temp.m_Channel == ChatPct.Channel.SHIJIE || temp.m_Channel == ChatPct.Channel.Broadcast || (temp.m_Channel == ChatPct.Channel.LIANMENG
                    && !AllianceData.Instance.IsAllianceNotExist) || (temp.m_Channel == ChatPct.Channel.XiaoWu
                      && Application.loadedLevelName == SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.HOUSE)))
            {
                item.m_ChatBaseDataHandler.GetComponent<ChatDataHandler>().Awake();
            }
        });

        m_chatRootObject.SetActive(false);
    }

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

        var tempChannelList = AllianceData.Instance.IsAllianceNotExist ? new List<ChatPct.Channel>() { ChatPct.Channel.SHIJIE } : new List<ChatPct.Channel>() { ChatPct.Channel.SHIJIE, ChatPct.Channel.LIANMENG };
        m_ChannelList.AddRange(tempChannelList);
        m_ChannelList.Add(ChatPct.Channel.SYSTEM);

        m_DataReceivedEvent += OnSimpleDataReceived;
    }
}
