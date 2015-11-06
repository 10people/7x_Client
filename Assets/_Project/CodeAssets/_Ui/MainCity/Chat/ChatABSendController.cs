using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class ChatABSendController : ChatBaseSendController
{
    public override bool CheckChannelCanSendMessage()
    {
        //Cancel send message if no alliance when alliance channel.
        if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.LIANMENG && AllianceData.Instance.IsAllianceNotExist)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                m_ChatBaseWindow.m_ChatUiBoxManager.NoAllianceCallBack);
            return false;
        }

        //Cancel send message if no private found when private channel.
        if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.XiaoWu &&
            MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                m_ChatBaseWindow.m_ChatUiBoxManager.NoTeneCallBack);
            return false;
        }

        return true;
    }
}
