using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SelectCountryData : MonoBehaviour, SocketProcessor
{
    private static SelectCountryData m_instance = null;
    public static SelectCountryData Instance()
    {
        if (m_instance == null)
        {
            GameObject t_gameObject = UtilityTool.GetDontDestroyOnLoadGameObject();

            m_instance = t_gameObject.AddComponent<SelectCountryData>();
        }

        return m_instance;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
	// Use this for initialization
	void Start () 
    {
        RequestData();
	}

    public void RequestData()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.ALLIANCE_NON_RESP://返回联盟信息， 给没有联盟的玩家返回此条信息
                    {

                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        AllianceNonResp AllianceReponseInfo = new AllianceNonResp();
                        t_qx.Deserialize(t_tream, AllianceReponseInfo, AllianceReponseInfo.GetType());
               

                        return true;
                    }
            
            }
        }
        return false;
 
	}

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
