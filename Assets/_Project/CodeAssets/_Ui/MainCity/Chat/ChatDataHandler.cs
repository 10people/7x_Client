using System;
using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class ChatDataHandler : ChatBaseDataHandler
{
    #region ChatData

    public ChatWindow m_ChatWindow;

    /// <summary>
    /// Channel config, mention that base chat viewer may show kinds of channels.
    /// </summary>
    public ChatPct.Channel m_Channel;

    public GameObject AlertObject()
    {
        switch (m_Channel)
        {
            case ChatPct.Channel.SHIJIE:
                return m_ChatWindow.WorldAlert;
            case ChatPct.Channel.GUOJIA:
                Debug.LogError("Not correct channel num:" + m_Channel);
                return null;
            case ChatPct.Channel.LIANMENG:
                return m_ChatWindow.AllianceAlert;
            case ChatPct.Channel.XiaoWu:
                return m_ChatWindow.PrivateAlert;
            default:
                Debug.LogError("Not correct channel num:" + m_Channel);
                return null;
        }
    }

    public override void ClearUnUsedChatStructList()
    {
        if (m_Channel == ChatPct.Channel.LIANMENG && AllianceData.Instance.IsAllianceNotExist)
        {
            base.ClearUnUsedChatStructList();
            return;
        }

        if (m_Channel == ChatPct.Channel.XiaoWu && MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
        {
            base.ClearUnUsedChatStructList();
            return;
        }
    }

    #endregion

    public void OnDataReceived()
    {
        //Show alert if window closed.
        if (!m_ChatWindow.m_ChatOpenCloseController.isOpen)
        {
            if (Application.loadedLevelName == SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.CARRIAGE))
            {
                //CarriageMsgManager.Instance.m_RootManager.m_CarriageUi.m_ChatRedAlert.SetActive(true);

                if (m_Channel != ChatPct.Channel.SYSTEM)
                {
                    AlertObject().SetActive(true);
                }
            }
            else
            {
                //OpenCloseAlert.SetActive(true);
                if ((AllianceData.Instance.IsAllianceNotExist && m_Channel != ChatPct.Channel.SHIJIE) || (!AllianceData.Instance.IsAllianceNotExist && m_Channel != ChatPct.Channel.LIANMENG))
                {
                    MainCityUIL.SetRedAlert("chat", true);
                }

                if (m_Channel != ChatPct.Channel.SYSTEM)
                {
                    AlertObject().SetActive(true);
                }
            }
        }
        else
        {
            if (m_ChatWindow.CurrentChannel != m_Channel && m_Channel != ChatPct.Channel.SYSTEM)
            {
                AlertObject().SetActive(true);
            }
        }

        //Set sended data received.
        if (m_ChatWindow.m_ChatBaseSendController.m_SendedChatStruct != null
            && m_ChatWindow.m_ChatBaseSendController.m_SendedChatStruct.m_ChatPct.channel == m_Channel
            && m_ChatWindow.m_ChatBaseSendController.m_SendedChatStruct.m_ChatPct.senderName == JunZhuData.Instance().m_junzhuInfo.name)
        {
            m_ChatWindow.m_ChatBaseSendController.m_SendedChatStruct.isReceived = true;
            Refresh(2);
        }
        else
        {
            Refresh(1);
        }

        //Refresh broadcast in broadcast channel.
        if (m_Channel == ChatPct.Channel.Broadcast && storedChatStructList.Any() && !storedChatStructList.Last().isComeFromBroadCast)
        {
            HighestUI.Instance.m_BroadCast.ShowBroadCast(((storedChatStructList.Last().m_ChatPct.guoJia >= 1 && storedChatStructList.Last().m_ChatPct.guoJia <= 7) ? (ColorTool.Color_Gold_edc347 + "[" + ChatLogItem.NationToString[storedChatStructList.Last().m_ChatPct.guoJia] + "]" + "[-]") : "") + ColorTool.Color_Gold_ffb12a + storedChatStructList.Last().m_ChatPct.senderName + "[-]" + storedChatStructList.Last().m_ChatPct.content);
        }
    }

    new void OnDestroy()
    {
        m_DataReceivedEvent -= OnDataReceived;

        base.OnDestroy();
    }

    private bool IsAwaked = false;

    public new void Awake()
    {
        if (IsAwaked) return;

        IsAwaked = true;

        base.Awake();

        m_ChannelList.AddRange(new List<ChatPct.Channel>() { m_Channel, ChatPct.Channel.SYSTEM });
        m_DataReceivedEvent += OnDataReceived;
    }
}