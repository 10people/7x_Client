using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class FriendData : Singleton<FriendData>, SocketProcessor
{
    public List<long> FriendIDList = new List<long>();

	public static List<string> Friendsid = new List<string>();
    public void RequestData()
    {
        SocketHelper.SendQXMessage(ProtoIndexes.C_GET_FRIEND_IDS);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_GET_FRIEND_IDS:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FriendIds tempInfo = new FriendIds();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        FriendIDList = (tempInfo.ids == null) ? new List<long>() : tempInfo.ids.Select(item => long.Parse(item)).ToList();

						//Friendsid.Clear();
				        if(tempInfo.ids != null)
						{
							Friendsid = tempInfo.ids;
						}else
						{
					      if(Friendsid == null ){

							Friendsid.Add("");

							}
							
						}
						
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);

		base.OnDestroy();
    }
}
