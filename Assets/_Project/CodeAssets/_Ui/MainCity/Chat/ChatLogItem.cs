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
    public bool IsRobMode = false;

    private long RobKingID;

    #region Const Variables

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

    private const float DetailLabelRestrictWidth = 310f;
    private const float DetailLabelRestrictHeight = 25f;

    public static readonly Dictionary<ChatPct.Channel, string> ChannelToString = new Dictionary<ChatPct.Channel, string>()
    {
        {ChatPct.Channel.GUOJIA, "国家"}, {ChatPct.Channel.LIANMENG, "联盟"}, {ChatPct.Channel.SHIJIE, "世界"},
        {ChatPct.Channel.SILIAO, "私聊"}, {ChatPct.Channel.SYSTEM, "系统"}, {ChatPct.Channel.XiaoWu, "小屋"},{ChatPct.Channel.Broadcast, "广播"},
    };

    public static readonly Dictionary<int, string> NationToString = new Dictionary<int, string>()
    {
        {1, "齐"}, {2, "楚"}, {3, "燕"}, {4, "韩"}, {5, "赵"}, {6, "魏"}, {7, "秦"},
    };

    #endregion

    #region Components

    public GameObject AlertBtn;
    public GameObject CopyBtn;

    /// <summary>
    /// This button is not controlled by deactive method.
    /// </summary>
    public GameObject CarriageHelpBtn;

    /// <summary>
    /// This button is not controlled by deactive method.
    /// </summary>
    public GameObject RobBtn;

    public UISprite HeadSprite;
    public UISprite HeadBgSprite;

    public UILabel DetailLabel;
    public UISprite DetailBgSprite;
    public UILabel SenderLabel;

    public UISprite VipSprite;

    public FloatButtonsController AlertFloatButtonsController;
    public FloatButtonsController SenderFloatButtonsController;

    #endregion

    /// <summary>
    /// Set chat log item data.
    /// </summary>
    public void SetData()
    {
        //Set normal or simple mode.
        if (IsSimpleMode)
        {
            HeadSprite.gameObject.SetActive(false);
            HeadBgSprite.gameObject.SetActive(false);
        }
        else
        {
            HeadSprite.gameObject.SetActive(true);
            HeadBgSprite.gameObject.SetActive(true);
        }

        //Set mode.
        IsRobMode = m_ChatStruct.m_ChatPct.isLveDuoHelp;

        //Set rob button.
        RobBtn.SetActive(IsRobMode);

        //Set optional data.
        //TODO
        //HeadBgSprite.spriteName;
        VipSprite.spriteName = "vip" + m_ChatStruct.m_ChatPct.vipLevel;

        SenderLabel.gameObject.SetActive(true);
        if (m_ChatStruct.m_ChatPct.channel == ChatPct.Channel.SYSTEM)
        {
            SenderLabel.color = BasicRedColor;
            SenderLabel.text = m_ChatStruct.m_ChatPct.senderName;
        }
        else
        {
            SenderLabel.color = Color.white;
            SenderLabel.text = ColorTool.Color_Blue_016bc5 + "[" + ChannelToString[m_ChatStruct.m_ChatPct.channel] + "]" + "[-]" + ((m_ChatStruct.m_ChatPct.guoJia >= 1 && m_ChatStruct.m_ChatPct.guoJia <= 7) ? (ColorTool.Color_Gold_edc347 + "[" + NationToString[m_ChatStruct.m_ChatPct.guoJia] + "]" + "[-]") : "") + ColorTool.Color_Gold_ffb12a + m_ChatStruct.m_ChatPct.senderName + "[-]";
        }

        if (m_ChatStruct.m_ChatPct.senderName == JunZhuData.Instance().m_junzhuInfo.name)
        {
            DetailBgSprite.spriteName = DetailBGSpriteSelfName;
            DetailBgSprite.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            DetailBgSprite.spriteName = DetailBGSpriteOtherName;
            DetailBgSprite.transform.localScale = Vector3.one;
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

        CopyBtn.SetActive(false);
        CarriageHelpBtn.SetActive(false);
        AlertBtn.SetActive(!m_ChatStruct.isReceived);
    }

    /// <summary>
    /// Restrict detail bg width to fit chat window.
    /// </summary>
    public void AdaptDetailText()
    {
        //TODO: optimize calc.
        if (DetailLabel.height > DetailLabelRestrictHeight + 1)
        {
            DetailLabel.transform.localPosition = new Vector3(0, -(DetailLabel.height - DetailLabelRestrictHeight) / 2, 0);
        }
    }

    public void DeActiveAllButtonsExpectAlert()
    {
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

    public void OnCopyClick()
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

    public void OnRobClick()
    {
		PlunderData.Instance.PlunderOpponent (PlunderData.Entrance.CHAT,RobKingID);
    }

    [Obsolete("Do not use any more.")]
    public void OnCarriageHelpClick()
    {

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
        PoolManagerListController.Instance.ReturnItem("ChatDataItem", gameObject);
    }

    public void OnSenderClick()
    {
        if (IsSimpleMode) return;

        //disable when self or system message.
        if (m_ChatStruct.m_ChatPct.senderName == JunZhuData.Instance().m_junzhuInfo.name ||
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

        SenderFloatButtonsController.Initialize(tempList, true);

        TransformHelper.ActiveWithStandardize(transform, tempObject.transform);
        //adapt transform position.
        //TODO: complete adapt
        SenderFloatButtonsController.transform.position = HeadSprite.transform.position;
        SenderFloatButtonsController.transform.localPosition += new Vector3(HeadSprite.width, 0, 0);

        //adapt to scroll view.
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
        NGUIHelper.AdaptWidgetInScrollView(m_ChatBaseDataHandler.m_ScrollView, m_ChatBaseDataHandler.m_ScrollBar, SenderFloatButtonsController.m_BG.GetComponent<UIWidget>());
    }

    public void OnDetailClick()
    {
        if (IsSimpleMode) return;
    }

    public void OnAlertClick()
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

        AlertFloatButtonsController.Initialize(tempList, true);

        TransformHelper.ActiveWithStandardize(transform, tempObject.transform);
        //adapt transform position.
        AlertFloatButtonsController.transform.position = AlertBtn.transform.position;

        //adapt to scroll view.
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
        float widgetValue = scrollView.GetWidgetValueRelativeToScrollView(AlertFloatButtonsController.m_BG.GetComponent<UIWidget>()).y;
        if (widgetValue < 0 || widgetValue > 1)
        {
            scrollView.SetWidgetValueRelativeToScrollView(AlertFloatButtonsController.m_BG.GetComponent<UIWidget>(), 0);

            //clamp scroll bar value.
            //donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            float scrollValue = scrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) scrollBar.value = 0.99f;
            if (scrollValue <= 0) scrollBar.value = 0.01f;
        }
    }

    #region Mono

    #endregion
}