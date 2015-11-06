using UnityEngine;
using System.Collections;
using AllianceBattle;
using qxmobile.protobuf;

public class ChatABWindow : ChatBaseWindow
{
    public AllianceBattleUI m_AllianceBattleUi;

    public override ChatChannelFrame GetChannelFrame(ChatPct.Channel channel)
    {
        switch (channel)
        {
            case ChatPct.Channel.LIANMENG:
                return m_ChatChannelFrameList[0];
            default:
                Debug.LogError("Not correct channel num:" + CurrentChannel);
                return null;
        }
    }

    void Start()
    {
        CurrentChannel = ChatPct.Channel.LIANMENG;
    }
}
