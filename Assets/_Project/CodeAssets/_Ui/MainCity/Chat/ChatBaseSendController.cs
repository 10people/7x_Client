using System;
using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

public class ChatBaseSendController : MonoBehaviour
{
    public ChatBaseWindow m_ChatBaseWindow;

    /// <summary>
    /// Send message cd.
    /// </summary>
    [HideInInspector]
    public bool NotInSendMessageCD = true;

    [HideInInspector]
    public ChatBaseDataHandler.ChatStruct m_SendedChatStruct;

    public void SendMessageWithInputField()
    {
        //transfer and substring send string.
        string sendStr = m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value.Replace("\n", "");

        // notice console tool what were typed
        {
            if (ConsoleTool.Instance().OnChatContent(sendStr))
            {
                return;
            }
        }

        if (sendStr.Length > CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS))
        {
            sendStr = sendStr.Substring(0, (int)CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS));
        }

        ChatPct tempChatPct = new ChatPct
        {
            channel = m_ChatBaseWindow.CurrentChannel,
            content = sendStr,
            senderId = JunZhuData.Instance().m_junzhuInfo.id,
            senderName = JunZhuData.Instance().m_junzhuInfo.name
        };
        SendMessageWithChatPct(tempChatPct);
    }

    public void SendMessageWithChatPct(ChatPct chatPct)
    {
        SocketHelper.SendQXMessage(chatPct, ProtoIndexes.C_Send_Chat);

        NotInSendMessageCD = false;

        m_SendedChatStruct = new ChatBaseDataHandler.ChatStruct { isReceived = false, m_ChatPct = chatPct };

        TimeCalc.Instance.StopAllCoroutines();

        TimeCalc.Instance.StartCheckMsgSucceedCoroutine(ConfigTool.GetFloat(ConfigTool.CONST_NETOWRK_SOCKET_TIME_OUT) + checkMsgSecurityTimePlus, m_ChatBaseWindow, m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).m_ChatBaseDataHandler);
        TimeCalc.Instance.StartOpenSendMessageCD((float)CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_WORLD_INTERVAL_TIME), m_ChatBaseWindow);

        //Clean field after send chat message.
        m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value = null;
        m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditLabel.text = NullString;
    }

    //private void OnEditTextClick(GameObject go)
    //{
    //    //Cancel edit text if no alliance when alliance channel.
    //    if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.LIANMENG && AllianceData.Instance.IsAllianceNotExist == 1)
    //    {
    //        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //            m_ChatBaseWindow.m_ChatUiBoxManager.NoAllianceCallBack);
    //        return;
    //    }

    //    //Cancel edit text if no private found when private channel.
    //    if (m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.XiaoWu &&
    //        MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
    //    {
    //        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //            m_ChatBaseWindow.m_ChatUiBoxManager.NoTeneCallBack);
    //        return;
    //    }
    //}

    public void OnSubmitSendMessage()
    {
        //Check input num out of restrict.
        if (m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value.Length > CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS))
        {
            m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value = m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value.Substring(0, (int)CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS));
            m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditLabel.text = m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value;
        }

        if (m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).SendMessageButton.isEnabled)
        {
            OnMessageBTNClick(null);
        }
    }

    private void OnMessageBTNClick(GameObject go)
    {
        //Cancel send message if no text input.
        if (string.IsNullOrEmpty(m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value))
        {
            return;
        }

        if (!CheckChannelCanSendMessage()) return;

        //Cancel send message if in send message cd.
        if (!NotInSendMessageCD)
        {
            m_ChatBaseWindow.m_ChatUiBoxManager.SendMsgCD();
            return;
        }

        if ((m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.SHIJIE && m_ChatBaseWindow.m_RemainingFreeTimes <= 0) || m_ChatBaseWindow.CurrentChannel == ChatPct.Channel.Broadcast)
        {
            //Go to recharge info if not ingot enough.
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao < PurchaseTemplate.GetBuyWorldChat_Price(1))
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                    m_ChatBaseWindow.m_ChatUiBoxManager.ReChargeCallBack);
            }
            //Go to ingot pay info.
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                    m_ChatBaseWindow.m_ChatUiBoxManager.PayCallBack);
            }
        }
        else
        {
            SendMessageWithInputField();
        }
    }

    public virtual bool CheckChannelCanSendMessage()
    {
        Debug.LogError("Call CheckChannelCanSendMessage in base class.");
        return false;
    }

    private const float checkMsgSecurityTimePlus = 2;

    [HideInInspector]
    public readonly string NullString = "点击输入";

    private void OnEnable()
    {
        m_ChatBaseWindow.m_ChatChannelFrameList.ForEach(item => item.SendMessageListener.onClick = OnMessageBTNClick);
    }

    private void OnDisable()
    {
        m_ChatBaseWindow.m_ChatChannelFrameList.ForEach(item => item.SendMessageListener.onClick = null);
    }

    void Update()
    {
        //Check input is empty.
        m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).SendMessageButton.isEnabled = !string.IsNullOrEmpty(m_ChatBaseWindow.GetChannelFrame(m_ChatBaseWindow.CurrentChannel).EditInput.value);
    }
}
