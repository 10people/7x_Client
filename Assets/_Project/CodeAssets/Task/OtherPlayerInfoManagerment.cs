using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;
using System.Linq;
public class OtherPlayerInfoManagerment : MonoBehaviour, SocketProcessor
{
    public static OtherPlayerInfoManagerment m_OtherInfo;
    public Camera m_currentCamera;
    public UILabel m_LabName;
    public UILabel m_LabLevel;
    public string m_OtherPlayerId;
    public Vector3  m_NowPos;
    public EventIndexHandle m_ChaKan;
    public ScaleEffectController m_SEC;
    public GameObject m_ObjChaKan;
    public GameObject m_MainParent;
    private JunZhuInfo m_JunzhuPlayerResp;
    void Awake()
    {
        m_OtherInfo = this;
    }

	void Start ()
    {
        m_ObjChaKan.transform.position = m_currentCamera.ScreenToWorldPoint(m_NowPos);
        m_ChaKan.m_Handle += ChaKan;
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }
    void OnEnable()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
    void ShowInfo()
    {

        Request();
    }

    void Request()
    {
        string[] ss = m_OtherPlayerId.Split(':');
        JunZhuInfoSpecifyReq mJunZhuInfoSpecifyReq = new JunZhuInfoSpecifyReq();

        mJunZhuInfoSpecifyReq.junzhuId = long.Parse(ss[1]);

        MemoryStream t_stream = new MemoryStream();

        QiXiongSerializer q_serializer = new QiXiongSerializer();

        q_serializer.Serialize(t_stream, mJunZhuInfoSpecifyReq);

        byte[] t_protof = t_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ, ref t_protof);
    }
    void ChaKan (int index)
    {
        MainCityUI.TryRemoveFromObjectList(m_MainParent);
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);
        Destroy(m_MainParent);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
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
            
                            m_JunzhuPlayerResp = junzhuResp as JunZhuInfo;
                            m_LabName.text = m_JunzhuPlayerResp.name;
                            m_LabLevel.text = "LV." + m_JunzhuPlayerResp.level.ToString();
                            m_ObjChaKan.SetActive(true);
                            //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);
                        }
                      //  
                        return false;
                    }
            }
        }
        return false;
    }

    private void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var temp = Instantiate(p_object) as GameObject;
        var info = temp.GetComponent<KingDetailInfo>();

        var tempKingInfo = new KingDetailInfo.KingDetailData()
        {
            RoleID = m_JunzhuPlayerResp.roleId,
            Attack = m_JunzhuPlayerResp.gongji,
            AllianceName = m_JunzhuPlayerResp.lianMeng,
            BattleValue = m_JunzhuPlayerResp.zhanli,
            Junxian = m_JunzhuPlayerResp.junxian,
            JunxianRank = m_JunzhuPlayerResp.junxianRank,
            KingName = m_JunzhuPlayerResp.name,
            Level = m_JunzhuPlayerResp.level,
            Money = m_JunzhuPlayerResp.gongjin,
            Life = m_JunzhuPlayerResp.remainHp,
            Protect = m_JunzhuPlayerResp.fangyu,
            Title = m_JunzhuPlayerResp.chenghao
        };

       var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>();
        if (m_JunzhuPlayerResp.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
        {
            if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(m_JunzhuPlayerResp.junZhuId))
            {
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加好友", m_ButtonClick = OnAddFriendClick });
            }
            tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邮件", m_ButtonClick = OnMailClick });

            if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(m_JunzhuPlayerResp.junZhuId))
            {
                tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "屏蔽", m_ButtonClick = OnShieldClick });
            }
        }
        info.SetThis(tempKingInfo, tempConfigList);
        info.m_KingDetailEquipInfo.m_BagItemDic = m_JunzhuPlayerResp.equip.items.Where(item => item.buWei > 0).ToDictionary(item => KingDetailInfo.TransferBuwei(item.buWei));
        temp.SetActive(true);
        MainCityUI.TryAddToObjectList(temp);
    }
    private void OnAddFriendClick()
    {
        FriendOperationLayerManagerment.AddFriends((int)m_JunzhuPlayerResp.junZhuId);
    }

    private void OnMailClick()
    {
        //go to mail sys.
        //		EmailData.Instance.ReplyLetter(m_JunzhuPlayerResp.name);
        NewEmailData.Instance().OpenEmail(NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE, m_JunzhuPlayerResp.name);
    }

    private void OnShieldClick()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        JoinToBlacklist forbid = new JoinToBlacklist();
        forbid.junzhuId = m_JunzhuPlayerResp.junZhuId;
        t_qx.Serialize(t_tream, forbid);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Join_BlackList, ref t_protof);
    }


    void OnDisable()
    {
        SocketTool.UnRegisterMessageProcessor(this);
        m_OtherInfo = null;
    }
}
