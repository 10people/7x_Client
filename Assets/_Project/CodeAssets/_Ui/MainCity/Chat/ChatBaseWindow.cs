using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

public class ChatBaseWindow : MonoBehaviour
{
    #region General

    public GameObject FloatButtonPrefab;

    public List<ChatChannelFrame> m_ChatChannelFrameList = new List<ChatChannelFrame>();

    public virtual ChatChannelFrame GetChannelFrame(ChatPct.Channel channel)
    {
        Debug.LogError("Call GetChannelFrame in base class.");
        return null;
    }

    public virtual bool IsChatWindowOpened()
    {
        Debug.LogError("Call IsChatWindowOpened in base class.");
        return false;
    }

    //SendChatMessage
    public ChatBaseSendController m_ChatBaseSendController;

    //UIBoxCallBack
    public ChatUIBoxManager m_ChatUiBoxManager;

    public UIRoot ChatRoot;

    [HideInInspector]
    public ChatPct.Channel CurrentChannel = ChatPct.Channel.SHIJIE;

    [HideInInspector]
    public int m_RemainingFreeTimes;

    public void ClearObject()
    {
        //Remove grid's children and objectList.
        m_ChatChannelFrameList.ForEach(item => item.m_ChatBaseDataHandler.storedChatStructList.ForEach(item2 =>
        {
            if (item2.m_ChatLogItem != null)
            {
                PoolManagerListController.Instance.ReturnItem("ChatDataItem", item2.m_ChatLogItem.gameObject);
                item2.m_ChatLogItem = null;
            }
        }));
    }

    #endregion

    #region Carriage Help

    public UILabel InfoLabel;
    public GameObject InfoObject;

    private readonly Vector2 PopLabelDistance = new Vector2(0, 35);
    private const float PopLabelDuration = 2.0f;

    public void ShowInfo(string str)
    {
        InfoLabel.text = str;

        PopUpLabelTool.Instance().AddPopLabelWatcher(InfoObject, Vector3.zero, PopLabelDistance, iTween.EaseType.easeOutBack, -1.0f, iTween.EaseType.linear, PopLabelDuration);
    }

    #endregion

    #region Mono

    public void Awake()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
    }

    public void FloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        FloatButtonPrefab = p_object as GameObject;
    }

    #endregion
}
