using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_ANDROID

using System.Reflection;

#endif

#if UNITY_ANDROID

public class ClipboardHelper
{
    private static PropertyInfo m_systemCopyBufferProperty = null;

    private static PropertyInfo GetSystemCopyBufferProperty()
    {
        if (m_systemCopyBufferProperty == null)
        {
            Type T = typeof(GUIUtility);
            m_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
            if (m_systemCopyBufferProperty == null)
                throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
        }
        return m_systemCopyBufferProperty;
    }

    public static string clipBoard
    {
        get
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            return (string)P.GetValue(null, null);
        }
        set
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            P.SetValue(null, value, null);
        }
    }
}

#endif

/// <summary>
/// Base class in chat sys.
/// </summary>
public class ChatLogItem : MonoBehaviour
{
    [HideInInspector]
    public bool IsRefreshed = false;

    [HideInInspector]
    public bool IsSimpleMode = false;

    public ChatBaseDataHandler m_ChatBaseDataHandler;
    [HideInInspector]
    public ChatBaseDataHandler.ChatStruct m_ChatStruct = new ChatBaseDataHandler.ChatStruct();

    [HideInInspector]
    public bool IsLeftMode = true;
    [HideInInspector]
    public bool IsCarriageHelpMode = false;
    [HideInInspector]
    public bool IsRobMode = false;

    private long RobKingID;

    #region Const Variables

    private const int GapBetweenDetailLabelAndBG = 20;
    private const int GapBetweenMaxSenderLabelAndDetailBG = 20;

    /// <summary>
    /// Used as the right border in grid panel.
    /// </summary>
    private const int GapTidy = 20;
    private static readonly Color BasicLightYellow = new Color(1, 0.973f, 0.851f, 1);
    private static readonly Color BasicOrangeColor = new Color(1, 0.694f, 0.165f, 1);
    private static readonly Color BasicRedColor = new Color(0.769f, 0, 0, 1);

    private const string PlayerNamePrefixText = "<pn>";
    private const string PlayerNameSubfixText = "</pn>";
    //BasicOrangeColor
    private const string PlayerNamePrefixColor = "[ffb12a]";
    private const string PlayerNameSubfixColor = "[-]";

    private const string DetailBGSpriteSelfName = "DialogSelf";
    private const string DetailBGSpriteOtherName = "DialogOther";

    #endregion

    #region Components

    [HideInInspector]
    public GameObject AlertBtn;
    [HideInInspector]
    public UIEventListener AlertListener;
    [HideInInspector]
    public GameObject CopyContainer;
    [HideInInspector]
    public GameObject CopyBtn;
    [HideInInspector]
    public UISprite CopyContainerBG;
    [HideInInspector]
    public UIEventListener CopyListener;

    /// <summary>
    /// This button is not controlled by deactive method.
    /// </summary>
    [HideInInspector]
    public GameObject CarriageHelpBtn;
    [HideInInspector]
    public UIEventListener CarriageHelpListener;

    /// <summary>
    /// This button is not controlled by deactive method.
    /// </summary>
    [HideInInspector]
    public GameObject RobBtn;
    [HideInInspector]
    public UIEventListener RobListener;

    [HideInInspector]
    public UISprite DetailBG;
    [HideInInspector]
    public UILabel DetailLabel;
    [HideInInspector]
    public UIEventListener DetailListener;
    [HideInInspector]
    public UILabel SenderLabel;

    public UIWidget SenderWidget;
    public UISprite VipSprite;
    public UISprite NationSprite;
    public GameObject SenderContainer;

    [HideInInspector]
    public UIEventListener SenderListener;

    public FloatButtonsController AlertFloatButtonsController;
    public FloatButtonsController SenderFloatButtonsController;

    #endregion

    /// <summary>
    /// Set sender label and detail label transform, set left mode sender container's left border to 0 as base, set single line detail label's posY to 0 as base.
    /// </summary>
    public void SetTransform()
    {
        if (IsLeftMode)
        {
            SenderContainer.transform.localPosition = new Vector3(0 + SenderWidget.width / 2.0f - SenderWidget.transform.localPosition.x, 0, 0);

            DetailLabel.transform.localPosition = new Vector3(0 + SenderWidget.width + DetailLabel.width / 2.0f + GapBetweenMaxSenderLabelAndDetailBG + GapBetweenDetailLabelAndBG,
                0 - (DetailLabel.height - DetailLabel.fontSize) / 2.0f, 0);
        }
        else
        {
            if (IsCarriageHelpMode)
            {
                SenderContainer.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width / 2.0f - SenderWidget.transform.localPosition.x - CarriageHelpBtn.GetComponent<UIWidget>().width, 0, 0);

                DetailLabel.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - GapBetweenDetailLabelAndBG - DetailLabel.width / 2.0f - CarriageHelpBtn.GetComponent<UIWidget>().width,
                    0 - (DetailLabel.height - DetailLabel.fontSize) / 2.0f, 0);
            }
            else if (IsRobMode)
            {
                SenderContainer.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width / 2.0f - SenderWidget.transform.localPosition.x - RobBtn.GetComponent<UIWidget>().width, 0, 0);

                DetailLabel.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - GapBetweenDetailLabelAndBG - DetailLabel.width / 2.0f - RobBtn.GetComponent<UIWidget>().width,
                    0 - (DetailLabel.height - DetailLabel.fontSize) / 2.0f, 0);
            }
            else
            {
                SenderContainer.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width / 2.0f - SenderWidget.transform.localPosition.x, 0, 0);

                DetailLabel.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - GapBetweenDetailLabelAndBG - DetailLabel.width / 2.0f,
                    0 - (DetailLabel.height - DetailLabel.fontSize) / 2.0f, 0);
            }
        }
    }

    /// <summary>
    /// Set chat log item data.
    /// </summary>
    public void SetData()
    {
        //Set mode.
        IsLeftMode = m_ChatStruct.m_ChatPct.senderName != JunZhuData.Instance().m_junzhuInfo.name;
        IsCarriageHelpMode = m_ChatStruct.m_ChatPct.isYBHelp;
        IsRobMode = m_ChatStruct.m_ChatPct.isLveDuoHelp;

        //Disable help btn if it is self send.
        if (IsCarriageHelpMode && m_ChatStruct.m_ChatPct.senderId == JunZhuData.Instance().m_junzhuInfo.id)
        {
            CarriageHelpBtn.GetComponent<UIButton>().isEnabled = false;
        }

        //Disable help btn if it is self send.
        if (IsRobMode && m_ChatStruct.m_ChatPct.senderId == JunZhuData.Instance().m_junzhuInfo.id)
        {
            CarriageHelpBtn.GetComponent<UIButton>().isEnabled = false;
        }

        //Set carriage help button.
        CarriageHelpBtn.SetActive(IsCarriageHelpMode);
        //Set rob button.
        RobBtn.SetActive(IsRobMode);

        //Set optional data.
        VipSprite.spriteName = "vip" + m_ChatStruct.m_ChatPct.vipLevel;
        VipSprite.gameObject.SetActive(m_ChatStruct.m_ChatPct.vipLevel > 0);
        NationSprite.spriteName = "nation_" + m_ChatStruct.m_ChatPct.guoJia;

        SenderLabel.gameObject.SetActive(true);
        SenderLabel.text = m_ChatStruct.m_ChatPct.senderName;
        if (m_ChatStruct.m_ChatPct.channel == ChatPct.Channel.SYSTEM)
        {
            SenderLabel.color = BasicRedColor;
        }
        else if (SenderLabel.text == JunZhuData.Instance().m_junzhuInfo.name)
        {
            SenderLabel.color = BasicOrangeColor;
        }
        else
        {
            //[Add new color]Set label color to different color.
            SenderLabel.color = BasicOrangeColor;
        }

        DetailLabel.gameObject.SetActive(true);

        var oriStr = m_ChatStruct.m_ChatPct.content;

        if (IsRobMode)
        {
            var splited = oriStr.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (splited.Length != 2)
            {
                Debug.LogError("splited original string length is not correct.");
                return;
            }

            oriStr = splited[0];
            RobKingID = long.Parse(splited[1]);
        }

        //Set player name basic orange color.
        DetailLabel.text = oriStr.Replace(PlayerNamePrefixText, PlayerNamePrefixColor).Replace(PlayerNameSubfixText, PlayerNameSubfixColor);

        CopyContainer.SetActive(false);
        AlertBtn.SetActive(!m_ChatStruct.isReceived);
    }

    /// <summary>
    /// Restrict detail bg width to fit chat window.
    /// </summary>
    public void AdaptDetailText()
    {
        //same restrict detail bg width in 2 modes.
        //consider carriage help button.
        float restrictDetailBGWidth;
        if (IsCarriageHelpMode)
        {
            restrictDetailBGWidth = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - AlertBtn.GetComponent<UISprite>().width - CarriageHelpBtn.GetComponent<UIWidget>().width;
        }
        else if (IsRobMode)
        {
            restrictDetailBGWidth = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - AlertBtn.GetComponent<UISprite>().width - RobBtn.GetComponent<UIWidget>().width;
        }
        else
        {
            restrictDetailBGWidth = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - AlertBtn.GetComponent<UISprite>().width;
        }

        if (DetailBG.width > restrictDetailBGWidth)
        {
            DetailLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
            DetailLabel.width = (int)(restrictDetailBGWidth - 2 * GapBetweenDetailLabelAndBG);
        }
    }

    /// <summary>
    /// Set btns pos.
    /// </summary>
    public void AdaptButtons()
    {
        if (IsLeftMode)
        {
            //restrict btn pos with max value.
            float maxPosXCopy = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - CopyContainerBG.width / 2.0f;
            //Set copy btn pos
            float standradPosXCopy = SenderWidget.width + GapBetweenMaxSenderLabelAndDetailBG +
                                 DetailBG.width + CopyContainerBG.width / 2.0f;

            CopyBtn.transform.localPosition = new Vector3(standradPosXCopy < maxPosXCopy ? standradPosXCopy : maxPosXCopy, CopyBtn.transform.localPosition.y, 0);
        }
        else
        {
            //restrict btn pos with min value.
            float minPosXCopy = 0 + CopyContainerBG.width / 2.0f;
            //Set copy btn pos
            float standradPosXCopy = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - SenderWidget.width - GapBetweenMaxSenderLabelAndDetailBG - DetailBG.width - CopyContainerBG.width / 2.0f;

            CopyBtn.transform.localPosition = new Vector3(standradPosXCopy > minPosXCopy ? standradPosXCopy : minPosXCopy, CopyBtn.transform.localPosition.y, 0);
        }

        //Set carriage help button position.
        CarriageHelpBtn.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - CarriageHelpBtn.GetComponent<UIWidget>().width / 2.0f, 0, 0);

        //Set rob button position.
        RobBtn.transform.localPosition = new Vector3(m_ChatBaseDataHandler.ScrollViewWidth - RobBtn.GetComponent<UIWidget>().width / 2.0f, 0, 0);
    }

    public void DeActiveAllButtonsExpectAlert()
    {
        CopyContainer.SetActive(false);

        DestroyFloatButtons();
    }

    public void DestroyFloatButtons()
    {
        if (AlertFloatButtonsController != null)
        {
            Destroy(AlertFloatButtonsController.gameObject);
            AlertFloatButtonsController = null;
        }

        if (SenderFloatButtonsController != null)
        {
            Destroy(SenderFloatButtonsController.gameObject);
            SenderFloatButtonsController = null;
        }
    }

    private void GetInfo()
    {
        DeActiveAllButtonsExpectAlert();

        JunZhuInfoSpecifyReq temp = new JunZhuInfoSpecifyReq()
        {
            junzhuId = m_ChatStruct.m_ChatPct.senderId
        };

        //Set response to king detail info and request.
        m_ChatBaseDataHandler.IsResponseToKingDetailInfo = true;
        SocketHelper.SendQXMessage(temp, ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ);
    }

    private void OnAddFriendClick()
    {
        DeActiveAllButtonsExpectAlert();

        m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatUiBoxManager.AddFriendName = m_ChatStruct.m_ChatPct.senderName;
        FriendOperationLayerManagerment.AddFriends((int)m_ChatStruct.m_ChatPct.senderId);
    }

    private void OnMailClick()
    {
        DeActiveAllButtonsExpectAlert();

        //go to mail sys.
        //		EmailData.Instance.ReplyLetter(m_ChatStruct.m_ChatPct.senderName);
        NewEmailData.Instance().OpenEmail(NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE, m_ChatStruct.m_ChatPct.senderName);
    }

    private void OnShieldClick()
    {
        DeActiveAllButtonsExpectAlert();

        m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatUiBoxManager.JunZhuIDToBeShielded = m_ChatStruct.m_ChatPct.senderId;
        m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatUiBoxManager.ShieldName = m_ChatStruct.m_ChatPct.senderName;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatUiBoxManager.ShieldCallBack);
    }

    private void OnCopyClick(GameObject go)
    {
        //        CopyContainer.SetActive(false);

        //#if UNITY_EDITOR||UNITY_STANDALONE
        //        var te = new TextEditor
        //        {
        //            content = new GUIContent(DetailLabel.text.Replace("\n", ""))
        //        };

        //        if (te.content.text.Length > CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS))
        //        {
        //            te.content.text = te.content.text.Substring(0, (int)CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS));
        //        }

        //        te.SelectAll();
        //        te.Copy();
        //#endif

        //#if UNITY_ANDROID

        //        string str = DetailLabel.text.Replace("\n", "");
        //        if (str.Length > CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS))
        //        {
        //            str = str.Substring(0, (int)CanshuTemplate.GetValueByKey(CanshuTemplate.CHAT_MAX_WORDS));
        //        }

        //        ClipboardHelper.clipBoard = str;

        //#endif

        //        //Add this later.
        //#if UNITY_IPHONE

        //#endif

    }

    private void OnRobClick(GameObject go)
    {
        LueDuoData.Instance.LueDuoOpponentReq(RobKingID, LueDuoData.WhichOpponent.CHAT);
    }

    private void OnCarriageHelpClick(GameObject go)
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), OnCarriageHelpCallBack);
    }

    private void OnCarriageHelpCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, LanguageTemplate.GetText(LanguageTemplate.Text.YUN_BIAO_12).Replace("***", m_ChatStruct.m_ChatPct.senderName).Replace("X", CanshuTemplate.GetValueByKey(CanshuTemplate.YUNBIAOASSISTANCE_GAIN_SUCCEED).ToString()),
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL), LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM),
            OnBoxCarriageHelp);
    }

    private void OnBoxCarriageHelp(int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                CarriageHelpBtn.GetComponent<UIButton>().isEnabled = false;

                AnswerYaBiaoHelpReq temp = new AnswerYaBiaoHelpReq() { code = 10, jzId = m_ChatStruct.m_ChatPct.senderId };

                SocketHelper.SendQXMessage(temp, ProtoIndexes.C_ANSWER_YBHELP_RSQ);

                break;
            default:
                Debug.LogError("UIBox callback para:" + i + " is not correct.");
                break;
        }
    }

    private void OnResendClick()
    {
        //Cancel send message if in send message cd.
        if (!m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatBaseSendController.NotInSendMessageCD)
        {
            m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatUiBoxManager.SendMsgCD();
            return;
        }

        DeActiveAllButtonsExpectAlert();

        OnDeleteClick();

        var tempPct = new ChatPct
        {
            channel = m_ChatStruct.m_ChatPct.channel,
            content = m_ChatStruct.m_ChatPct.content,
            senderId = JunZhuData.Instance().m_junzhuInfo.id,
            senderName = JunZhuData.Instance().m_junzhuInfo.name
        };
        m_ChatBaseDataHandler.m_ChatBaseWindow.m_ChatBaseSendController.SendMessageWithChatPct(tempPct);
    }

    private void OnDeleteClick()
    {
        DeActiveAllButtonsExpectAlert();

        m_ChatBaseDataHandler.RemoveChatStruct(this);
        m_ChatBaseDataHandler.Refresh(0);
        m_ChatStruct.m_ChatLogItem = null;
        m_ChatBaseDataHandler.storedChatStructList.Remove(m_ChatStruct);
        PoolManagerListController.Instance.ReturnItem(IsLeftMode ? "ChatDataOtherItem" : "ChatDataSelfItem", gameObject);
    }

    private void OnSenderClick(GameObject go)
    {
        if (IsSimpleMode) return;

        //disable when left mode or system message.
        if (SenderLabel.text == JunZhuData.Instance().m_junzhuInfo.name ||
            m_ChatStruct.m_ChatPct.channel == ChatPct.Channel.SYSTEM)
        {
            return;
        }

        //Deactive all buttons
        var chatLogList = m_ChatBaseDataHandler.storedChatStructList.Where(item => item.m_ChatLogItem != null).Select(item => item.m_ChatLogItem).ToList();
        chatLogList.ForEach(item => item.DeActiveAllButtonsExpectAlert());

        if (m_ChatBaseDataHandler.m_ChatBaseWindow.FloatButtonPrefab == null)
        {
            return;
        }

        //Create object and set.
        GameObject tempObject = Instantiate(m_ChatBaseDataHandler.m_ChatBaseWindow.FloatButtonPrefab);
        SenderFloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

        List<FloatButtonsController.ButtonInfo> tempList = new List<FloatButtonsController.ButtonInfo>();

        tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "查看信息", m_VoidDelegate = GetInfo });

        if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_ChatStruct.m_ChatPct.senderId))
        {
            tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加好友", m_VoidDelegate = OnAddFriendClick });
        }
        tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "邮件", m_VoidDelegate = OnMailClick });

        if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_ChatStruct.m_ChatPct.senderId))
        {
            tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽", m_VoidDelegate = OnShieldClick });
        }

        SenderFloatButtonsController.Initialize(tempList, IsLeftMode);

        TransformHelper.ActiveWithStandardize(transform, tempObject.transform);
        //adapt transform position.
        if (IsLeftMode)
        {
            SenderFloatButtonsController.transform.localPosition = new Vector3(VipSprite.width + SenderLabel.width + SenderFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - SenderFloatButtonsController.m_BGLeft.transform.localPosition.x, SenderFloatButtonsController.transform.localPosition.y, SenderFloatButtonsController.transform.localPosition.z);
        }
        else
        {
            SenderFloatButtonsController.transform.localPosition = new Vector3(SenderLabel.transform.localPosition.x - SenderLabel.width / 2.0f - SenderFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - SenderFloatButtonsController.m_BGLeft.transform.localPosition.x, SenderFloatButtonsController.transform.localPosition.y, SenderFloatButtonsController.transform.localPosition.z);
        }

        StartCoroutine(AdjustSenderFloatButton());
    }

    IEnumerator AdjustSenderFloatButton()
    {
        yield return new WaitForEndOfFrame();

        //Cancel adjust cause multi touch may destroy this float buttons gameobject.
        if (SenderFloatButtonsController == null || SenderFloatButtonsController.gameObject == null)
        {
            yield break;
        }

        //adapt pop up buttons to scroll view.
        NGUIHelper.AdaptWidgetInScrollView(m_ChatBaseDataHandler.m_ScrollView, m_ChatBaseDataHandler.m_ScrollBar, SenderFloatButtonsController.m_BGLeft.GetComponent<UIWidget>());
    }

    private void OnDetailClick(GameObject go)
    {
        if (IsSimpleMode) return;

        ////Deactive all other buttons active copy.
        //if (!CopyBtn.activeInHierarchy)
        //{
        //    var chatLogList = s_ChatWindow.m_ChatDataHandlerList.showingChatMsgList;
        //    chatLogList.ForEach(item => item.DeActiveAllButtonObjectsExpectAlert());
        //}

        //CopyContainer.SetActive(!CopyContainer.activeSelf);
    }

    private void OnAlertClick(GameObject go)
    {
        if (IsSimpleMode) return;

        //Deactive all buttons
        var chatLogList = m_ChatBaseDataHandler.storedChatStructList.Where(item => item.m_ChatLogItem != null).Select(item => item.m_ChatLogItem).ToList();
        chatLogList.ForEach(item => item.DeActiveAllButtonsExpectAlert());

        if (m_ChatBaseDataHandler.m_ChatBaseWindow.FloatButtonPrefab == null)
        {
            return;
        }

        //Create object and set.
        GameObject tempObject = (GameObject)Instantiate(m_ChatBaseDataHandler.m_ChatBaseWindow.FloatButtonPrefab);
        AlertFloatButtonsController = tempObject.GetComponent<FloatButtonsController>();

        List<FloatButtonsController.ButtonInfo> tempList = new List<FloatButtonsController.ButtonInfo>();
        tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "重发", m_VoidDelegate = OnResendClick });
        tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "删除", m_VoidDelegate = OnDeleteClick });

        AlertFloatButtonsController.Initialize(tempList, IsLeftMode);

        TransformHelper.ActiveWithStandardize(transform, tempObject.transform);
        //adapt transform position.
        if (IsLeftMode)
        {
            float maxPosXAlert = m_ChatBaseDataHandler.ScrollViewWidth - GapTidy - AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - AlertFloatButtonsController.m_BGLeft.transform.localPosition.x;
            float standardPosXContainer = AlertBtn.transform.localPosition.x + AlertBtn.GetComponent<UISprite>().width / 2.0f + AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - AlertFloatButtonsController.m_BGLeft.transform.localPosition.x;
            AlertFloatButtonsController.transform.localPosition = new Vector3(standardPosXContainer < maxPosXAlert ? standardPosXContainer : maxPosXAlert, AlertFloatButtonsController.transform.localPosition.y, 0);
        }
        else
        {
            float minPosXAlert = 0 + AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - AlertFloatButtonsController.m_BGLeft.transform.localPosition.x;
            float standardPosXContainer = AlertBtn.transform.localPosition.x - AlertBtn.GetComponent<UISprite>().width / 2.0f - AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>().width / 2.0f - AlertFloatButtonsController.m_BGLeft.transform.localPosition.x;
            AlertFloatButtonsController.transform.localPosition = new Vector3(standardPosXContainer > minPosXAlert ? standardPosXContainer : minPosXAlert, AlertFloatButtonsController.transform.localPosition.y, 0);
        }

        StartCoroutine(AdjustAlertFloatButton());
    }

    IEnumerator AdjustAlertFloatButton()
    {
        yield return new WaitForEndOfFrame();

        //Cancel adjust cause multi touch may destroy this float buttons gameobject.
        if (AlertFloatButtonsController == null || AlertFloatButtonsController.gameObject == null)
        {
            yield break;
        }

        //adapt pop up buttons to scroll view.
        UIScrollView scrollView = m_ChatBaseDataHandler.m_ScrollView;
        UIScrollBar scrollBar = m_ChatBaseDataHandler.m_ScrollBar;
        float widgetValue = scrollView.GetWidgetValueRelativeToScrollView(AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>()).y;
        if (widgetValue < 0 || widgetValue > 1)
        {
            scrollView.SetWidgetValueRelativeToScrollView(AlertFloatButtonsController.m_BGLeft.GetComponent<UIWidget>(), 0);

            //clamp scroll bar value.
            //donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            float scrollValue = scrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) scrollBar.value = 0.99f;
            if (scrollValue <= 0) scrollBar.value = 0.01f;
        }
    }

    #region Mono

    private void OnEnable()
    {
        SenderListener.onClick = OnSenderClick;
        DetailListener.onClick = OnDetailClick;
        AlertListener.onClick = OnAlertClick;
        CopyListener.onClick = OnCopyClick;
        CarriageHelpListener.onClick = OnCarriageHelpClick;
        RobListener.onClick = OnRobClick;
    }

    private void OnDisable()
    {
        SenderListener.onClick = null;
        DetailListener.onClick = null;
        AlertListener.onClick = null;
        CopyListener.onClick = null;
        CarriageHelpListener.onClick = null;
        RobListener.onClick = null;
    }

    private void Awake()
    {
        SenderLabel = TransformHelper.FindChild(transform, "SenderText").GetComponent<UILabel>();
        DetailLabel = TransformHelper.FindChild(transform, "DetailText").GetComponent<UILabel>();
        CopyBtn = TransformHelper.FindChild(transform, "Copy").gameObject;
        CarriageHelpBtn = TransformHelper.FindChild(transform, "CarriageHelp").gameObject;
        RobBtn = TransformHelper.FindChild(transform, "Rob").gameObject;
        CopyContainer = TransformHelper.FindChild(transform, "CopyContainer").gameObject;
        CopyContainerBG = TransformHelper.FindChild(transform, "CopyContainerBG").GetComponent<UISprite>();
        AlertBtn = TransformHelper.FindChild(transform, "Alert").gameObject;
        DetailBG = TransformHelper.FindChild(transform, "DetailBG").GetComponent<UISprite>();
        SenderListener = UIEventListener.Get(SenderLabel.gameObject);
        DetailListener = UIEventListener.Get(DetailBG.gameObject);
        CopyListener = UIEventListener.Get(CopyBtn.gameObject);
        CarriageHelpListener = UIEventListener.Get(CarriageHelpBtn.gameObject);
        RobListener = UIEventListener.Get(RobBtn.gameObject);
        AlertListener = UIEventListener.Get(AlertBtn.gameObject);
    }

    #endregion
}