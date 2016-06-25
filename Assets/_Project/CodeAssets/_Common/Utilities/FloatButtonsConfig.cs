using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

public class FloatButtonsConfig : MonoBehaviour
{
    public static long m_KingId;
    public static string m_KingName;
    public static string m_AllianceName;
    public static List<GameObject> m_PrivateChatObjectList;
    public static DelegateHelper.VoidDelegate m_ExecuteAfterClose;

    public static List<FloatButtonsController.ButtonInfo> GetConfig(long kingID, string kingName, string allianceName, List<GameObject> privateChatObjectList, DelegateHelper.VoidDelegate ExecuteAfterClose = null, bool isGetInfo = true, bool isAddFriend = true, bool isShield = true, bool isRob = true, bool isCopyName = true, bool isPrivateChat = true)
    {
        m_KingId = kingID;
        m_KingName = kingName;
        m_AllianceName = allianceName;
        m_PrivateChatObjectList = privateChatObjectList;
        m_ExecuteAfterClose = ExecuteAfterClose;

        List<FloatButtonsController.ButtonInfo> tempList = new List<FloatButtonsController.ButtonInfo>();
        if (isGetInfo)
        {
            tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "查看信息", m_VoidDelegate = GetInfo });
        }
        if (kingID != JunZhuData.Instance().m_junzhuInfo.id)
        {
            if (isAddFriend)
            {
                if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(kingID))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
                }
            }

            if (isShield)
            {
                if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(kingID))
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "屏蔽玩家", m_VoidDelegate = Shield });
                }
            }

            if (isRob)
            {
                if (string.IsNullOrEmpty(allianceName) || allianceName != "无")
                {
                    tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "掠夺", m_VoidDelegate = Rob });
                }
            }

            if (isCopyName)
            {
                tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "复制名字", m_VoidDelegate = CopyName });
            }

            if (isPrivateChat)
            {
                tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "私聊", m_VoidDelegate = PrivateChat });
            }
        }

        return tempList;
    }

    private static void GetInfo()
    {
        KingDetailInfoController.Instance.ShowKingDetailWindow(m_KingId);

        if (m_ExecuteAfterClose != null)
        {
            m_ExecuteAfterClose();
        }
    }

    private static void AddFriend()
    {
        if (FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_KingId))
        {
            ClientMain.m_UITextManager.createText("该玩家已经是您的好友！");
        }
        else
        {
            FriendOperationLayerManagerment.AddFriends((int)m_KingId);
            if (m_ExecuteAfterClose != null)
            {
                m_ExecuteAfterClose();
            }
        }
    }

    private static void Shield()
    {
        JoinToBlacklist tempMsg = new JoinToBlacklist
        {
            junzhuId = m_KingId
        };
        SocketHelper.SendQXMessage(tempMsg, ProtoIndexes.C_Join_BlackList);
        if (m_ExecuteAfterClose != null)
        {
            m_ExecuteAfterClose();
        }
    }

    private static void Rob()
    {
        PlunderData.Instance.PlunderOpponent(PlunderData.Entrance.RANKLIST, m_KingId);
        if (m_ExecuteAfterClose != null)
        {
            m_ExecuteAfterClose();
        }
    }

    private static void CopyName()
    {
        DeviceHelper.SetClipBoard(m_KingName);
        if (m_ExecuteAfterClose != null)
        {
            m_ExecuteAfterClose();
        }
    }

    private static void PrivateChat()
    {
        if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_KingId))
        {
            QXChatPage.chatPage.setSiliaoList(m_KingId, m_KingName, m_PrivateChatObjectList);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CannotChatCallBack);
        }

        if (m_ExecuteAfterClose != null)
        {
            m_ExecuteAfterClose();
        }
    }

    private static void CannotChatCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
             LanguageTemplate.GetText(5004), null,
             null,
             LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
             null);
    }
}
