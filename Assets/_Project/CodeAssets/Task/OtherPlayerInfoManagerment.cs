using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;
using System.Linq;
public class OtherPlayerInfoManagerment : MonoBehaviour 
{
    public static OtherPlayerInfoManagerment m_OtherInfo;
    public Camera m_currentCamera;
    public UILabel m_LabName;
    public UILabel m_LabLevel;
    public string m_OtherPlayerId;
    public Vector3  m_NowPos;
    public List<EventIndexHandle> m_listEvent;
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
        m_listEvent.ForEach(p=>p.m_Handle += ChaKan);
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }
    void Update()
    {
        if (FriendOperationData.Instance.m_GreetSucess || AllianceData.Instance. m_InviteGetData)
        {
            FriendOperationData.Instance.m_GreetSucess = false;
            AllianceData.Instance.m_InviteGetData = false;
            MainCityUI.TryRemoveFromObjectList(m_MainParent);
            Destroy(m_MainParent);
        }
    }
    void ShowInfo()
    {
        m_ObjChaKan.SetActive(true);
        for (int i = 0; i < m_listEvent.Count; i++)
        {
            m_listEvent[i].GetComponent<Collider>().enabled = true;
        }

    }
    void ChaKan (int index)
    {
        switch (index)
        {
            case 0:
                {
                    string[] ss = m_OtherPlayerId.Split(':');
                    KingDetailInfoController.Instance.ShowKingDetailWindow(long.Parse(ss[1]));
                    Destroy(m_MainParent);
                }
                break;
            case 1:
                {
                    string[] ss = m_OtherPlayerId.Split(':');
                    GreetReq green = new GreetReq();

                    green.jzId = long.Parse(ss[1]);

                    MemoryStream t_stream = new MemoryStream();

                    QiXiongSerializer q_serializer = new QiXiongSerializer();

                    q_serializer.Serialize(t_stream, green);

                    byte[] t_protof = t_stream.ToArray();

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GREET_REQ, ref t_protof);
                }
                break;
            case 2:
                {
                    if (AllianceData.Instance.g_UnionInfo == null || AllianceData.Instance.g_UnionInfo.identity == 0)
                    {
                        MainCityUI.TryRemoveFromObjectList(m_MainParent);
                        Destroy(m_MainParent);
                        ClientMain.m_UITextManager.createText("只有盟主/副盟主可以邀请玩家加入联盟！");
                      
                    }
                    else if (AllianceData.Instance.g_UnionInfo.identity > 0)
                    {
                        string[] ss = m_OtherPlayerId.Split(':');
             
                        AllianceData.Instance.RequestAllianceInvite(long.Parse(ss[1]));
                        Destroy(m_MainParent);
                    }
                }
                break;
            case 3:
                {
                    string[] ss = m_OtherPlayerId.Split(':');
                    // long.Parse(ss[1])   ss[2];
                    QXChatPage.chatPage.setSiliao(long.Parse(ss[1]), ss[2], m_MainParent);
                    Destroy(m_MainParent);
                }
                break;
        }
    }
    void OnDisable()
    {
        m_OtherInfo = null;
    }
}
