using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
 
public class BlockedData : MonoBehaviour, SocketProcessor
{
    public Dictionary<long, BlackJunzhuInfo> m_BlockedInfoDic = new Dictionary<long, BlackJunzhuInfo>();
    private static BlockedData m_BlockedData;
    public int m_ForbidFriendsMax = 0;
    public static BlockedData Instance()
    {
        if (m_BlockedData == null)
        {
            GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

            m_BlockedData = t_GameObject.AddComponent<BlockedData>();
        }

        return m_BlockedData;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        RequestBlockedInfo();
    }

	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor( this );

		m_BlockedData = null;
	}

    public void RequestBlockedInfo()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_BLACKLIST);
    }
   
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_GET_BLACKLIST :
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetBlacklistResp tempInfo = new GetBlacklistResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
                        m_BlockedInfoDic.Clear();
                        m_ForbidFriendsMax = tempInfo.blackMax;
                        if (tempInfo.junzhuInfo != null)
                        {
                            foreach (BlackJunzhuInfo item in tempInfo.junzhuInfo)
                            {
                                m_BlockedInfoDic.Add(item.junzhuId, item);
                            }
                        }
                        return true;
                    }

                default: return false;
            }
        }
        return false;
    }
}
