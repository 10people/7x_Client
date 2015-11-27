using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NationData : Singleton<NationData>,SocketProcessor
{
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
    public  GuoJiaMainInfoResp m_NationInfo;
    public bool m_DataGetComplete = false;
    public bool m_DataRequest = false;
    void Start()
    {
      //  RequestData();
    }
 
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {

            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.GUO_JIA_MAIN_INFO_RESP:// 获得国家信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GuoJiaMainInfoResp ReponseInfo = new GuoJiaMainInfoResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        m_NationInfo = ReponseInfo;
                        //if (ReponseInfo.guojiaAward.Equals("0"))
                        //{
                        //    MainCityUIRB.SetRedAlert(212, false);
                        //}
                        //else
                        //{
                        //    MainCityUIRB.SetRedAlert(212, true);
                        //}
                        if (m_DataRequest)
                        {
                            m_DataRequest = false;
                            m_DataGetComplete = true;
                        }

                       
                        return true;
                    }
            }
        }
        return false;
    }

}
