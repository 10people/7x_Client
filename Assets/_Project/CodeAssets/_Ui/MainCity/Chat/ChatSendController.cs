using System;
using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

public class ChatSendController : ChatBaseSendController
{
    public ChatWindow m_ChatWindow;

    public override bool CheckChannelCanSendMessage()
    {
        //Cancel send message if no alliance when alliance channel.
        if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.LIANMENG && AllianceData.Instance.IsAllianceNotExist)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                m_ChatBaseWindow.m_ChatUiBoxManager.NoAllianceCallBack);

            //go to world channel
            m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject,
                SendMessageOptions.DontRequireReceiver);
            return false;
        }

        //Cancel send message if no private found when private channel.
        if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.XiaoWu &&
            MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                m_ChatBaseWindow.m_ChatUiBoxManager.NoTeneCallBack);

            //go to world channel
            m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject,
                SendMessageOptions.DontRequireReceiver);
            return false;
        }

        return true;
    }
}