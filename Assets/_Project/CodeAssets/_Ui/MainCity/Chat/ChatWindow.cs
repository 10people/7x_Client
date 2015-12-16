using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using UnityEngine;

public class ChatWindow : ChatBaseWindow, SocketListener
{
    #region General

    public static ChatWindow s_ChatWindow;

    public override ChatChannelFrame GetChannelFrame(ChatPct.Channel channel)
    {
        switch (channel)
        {
            case ChatPct.Channel.SHIJIE:
                return m_ChatChannelFrameList[0];
            case ChatPct.Channel.LIANMENG:
                return m_ChatChannelFrameList[1];
            case ChatPct.Channel.XiaoWu:
                return m_ChatChannelFrameList[2];
            default:
                Debug.LogError("Not correct channel num:" + CurrentChannel);
                return null;
        }
    }

    public override bool IsChatWindowOpened()
    {
        return m_ChatOpenCloseController.m_Root.gameObject.activeInHierarchy;
    }

    //OpenCloseWindow
    public ChatOpenCloseController m_ChatOpenCloseController;

    [Obsolete("Do not use anymore, maybe useful in future.")]
    [HideInInspector]
    public GameObject m_RootChatOpenObject;

    #endregion

    #region Toggle Control

    [HideInInspector]
    public bool isEnterToggleByOpeningWindow;
    public TogglesControl TogglesControl;

    //Alert objects, OpenCloseAlert in MainCityUIL.
    public GameObject WorldAlert;
    public GameObject AllianceAlert;
    public GameObject PrivateAlert;

    public void OnToggleClick(int index)
    {
        switch (index)
        {
            //world
            case 0:
                {
                    CurrentChannel = ChatPct.Channel.SHIJIE;
                    WorldAlert.SetActive(false);
                    break;
                }
            //alliance
            case 1:
                {
                    if (AllianceData.Instance.IsAllianceNotExist)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), m_ChatUiBoxManager.NoAllianceCallBack);
                        return;
                    }
                    CurrentChannel = ChatPct.Channel.LIANMENG;
                    AllianceAlert.SetActive(false);
                    break;
                }
            //house
            case 2:
                {
                    if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.MainCity)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), m_ChatUiBoxManager.NoTeneCallBack);
                        return;
                    }
                    CurrentChannel = ChatPct.Channel.XiaoWu;
                    PrivateAlert.SetActive(false);
                    break;
                }
        }

        //Send go to toggle control.
        TogglesControl.OnToggleClick(index);

        isEnterToggleByOpeningWindow = false;

        m_ChatChannelFrameList.ForEach(item => item.gameObject.SetActive(false));
        GetChannelFrame(CurrentChannel).gameObject.SetActive(true);
        GetChannelFrame(CurrentChannel).m_ChatBaseDataHandler.Refresh(2);
    }

    #endregion

    #region Mono

    void OnEnable()
    {
        //Send carriage help times.
        SocketHelper.SendQXMessage(ProtoIndexes.C_YABIAO_XIEZHU_RSQ);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        TogglesControl.TogglesEvents[0].m_Handle -= OnToggleClick;
        TogglesControl.TogglesEvents[1].m_Handle -= OnToggleClick;
        TogglesControl.TogglesEvents[2].m_Handle -= OnToggleClick;
    }

    new void Awake()
    {
        base.Awake();

        s_ChatWindow = this;

        SocketTool.RegisterSocketListener(this);

        TogglesControl.TogglesEvents[0].m_Handle += OnToggleClick;
        TogglesControl.TogglesEvents[1].m_Handle += OnToggleClick;
        TogglesControl.TogglesEvents[2].m_Handle += OnToggleClick;
    }

    void OnLevelWasLoaded(int level)
    {
        if (ChatRoot.gameObject.activeInHierarchy)
        {
            ChatRoot.gameObject.SetActive(false);
        }

        //Init register.
        m_ChatChannelFrameList.ForEach(item => SocketTool.RegisterSocketListener(item.m_ChatBaseDataHandler));
    }

    #endregion

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message == null)
        {
            return false;
        }
        switch (p_message.m_protocol_index)
        {
            case ProtoIndexes.S_Join_BlackList_Resp:
                {
                    //Cancel process if chat window not opened.
                    if (!IsChatWindowOpened()) return false;

                    object joinToBlackListRespObject = new BlacklistResp();
                    if (SocketHelper.ReceiveQXMessage(ref joinToBlackListRespObject, p_message,
                        ProtoIndexes.S_Join_BlackList_Resp))
                    {
                        BlacklistResp tempJoinBlacklistResp = joinToBlackListRespObject as BlacklistResp;

                        //shield if return 0.
                        if (tempJoinBlacklistResp.result == 0)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                m_ChatUiBoxManager.ShieldSucceedCallBack);

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
            case ProtoIndexes.S_ANSWER_YBHELP_RESP:
                {
                    object answerCarriageHelp = new AnswerYaBiaoHelpResp();
                    if (SocketHelper.ReceiveQXMessage(ref answerCarriageHelp, p_message,
                        ProtoIndexes.S_ANSWER_YBHELP_RESP))
                    {
                        AnswerYaBiaoHelpResp tempAnswerCarriageHelp = answerCarriageHelp as AnswerYaBiaoHelpResp;

                        switch (tempAnswerCarriageHelp.code)
                        {
                            //succeed
                            case 10:
                                {
                                    ShowInfo("协助成功");
                                    break;
                                }
                            case 20:
                                {
                                    ShowInfo("协助失败（押镖人已押镖等情况）");
                                    break;
                                }
                            case 30:
                                {
                                    ShowInfo("你已离开联盟");
                                    break;
                                }
                            case 40:
                                {
                                    ShowInfo("信息已过时");
                                    break;
                                }
                            case 50:
                                {
                                    ShowInfo("已经同意过协助");
                                    break;
                                }
                            //times not enough
                            case 60:
                                {
                                    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_84));
                                    break;
                                }
                            case 70:
                                {
                                    ShowInfo("不能协助自己");
                                    break;
                                }
                            case 80:
                                {
                                    Global.CreateBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
                                        null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_11).Replace("***", tempAnswerCarriageHelp.name),
                                        null,
                                        LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
                                        null);
                                    break;
                                }
                            //info invalid
                            default:
                                {
                                    ShowInfo(LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_85));
                                    break;
                                }
                        }

                        return true;
                    }
                    return false;
                }
        }
        return false;
    }
}