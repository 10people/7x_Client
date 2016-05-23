using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class KingDetailInfoController : Singleton<KingDetailInfoController>, SocketListener
{
    private JunZhuInfo m_junzhuPlayerResp;

    public void ShowKingDetailWindow(long id)
    {
        RequestInfo(id);
    }

    private void RequestInfo(long id)
    {
        JunZhuInfoSpecifyReq temp = new JunZhuInfoSpecifyReq()
        {
            junzhuId = id
        };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP:
                    {
                        object junzhuResp = new JunZhuInfo();
                        if (SocketHelper.ReceiveQXMessage(ref junzhuResp, p_message, ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP))
                        {
                            m_junzhuPlayerResp = junzhuResp as JunZhuInfo;

                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);

                            return true;
                        }
                        return false;
                    }
            }
        }
        return false;
    }

    /// <summary>
    /// king detail info window.
    /// </summary>
    /// <param name="p_www"></param>
    /// <param name="p_path"></param>
    /// <param name="p_object"></param>
    private void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var temp = Instantiate(p_object) as GameObject;
        var info = temp.GetComponent<KingDetailInfo>();

        var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>();
        if (m_junzhuPlayerResp.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
        {
            if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_junzhuPlayerResp.junZhuId))
            {
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加为好友", m_ButtonClick = AddFriend });
            }
            tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邮件", m_ButtonClick = Mail });
            if (m_junzhuPlayerResp.lianMeng != "无")
            {
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "掠夺", m_ButtonClick = Rob });
            }
            tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邀请入盟", m_ButtonClick = InviteToAlliance });
        }

        info.SetThis(m_junzhuPlayerResp, tempConfigList);

        temp.SetActive(true);
    }

    /// <summary>
    /// king detail info window button call back.
    /// </summary>
    public void AddFriend()
    {
        if (FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_junzhuPlayerResp.junZhuId))
        {
            ClientMain.m_UITextManager.createText("该玩家已经是您的好友！");
        }
        else
        {
            FriendOperationData.Instance.AddFriends((FriendOperationData.AddFriendType)(-1), m_junzhuPlayerResp.junZhuId, m_junzhuPlayerResp.name);
        }
    }

    public void Mail()
    {
        NewEmailData.Instance().OpenEmail(NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE, m_junzhuPlayerResp.name);
    }

    /// <summary>
    /// king detail info window button call back.
    /// </summary>
    public void Rob()
    {
        PlunderData.Instance.PlunderOpponent(PlunderData.Entrance.RANKLIST, m_junzhuPlayerResp.junZhuId);
    }

    /// <summary>
    /// king detail info window button call back.
    /// </summary>
    public void InviteToAlliance()
    {
        AllianceInvite green = new AllianceInvite();
        green.junzhuId = m_junzhuPlayerResp.junZhuId;

        MemoryStream t_stream = new MemoryStream();
        QiXiongSerializer q_serializer = new QiXiongSerializer();
        q_serializer.Serialize(t_stream, green);
        byte[] t_protof = t_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE, ref t_protof);
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    new void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        base.OnDestroy();
    }
}
