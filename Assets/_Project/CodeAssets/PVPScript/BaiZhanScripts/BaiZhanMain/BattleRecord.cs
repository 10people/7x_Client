using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleRecord : MonoBehaviour,SocketProcessor {

	public static BattleRecord recordData;

	public ZhandouRecordResp recordResp;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);

		recordData = this;
	}

	void Start ()
	{
		RecordReq ();
	}

	void RecordReq ()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZhanDou_Notes_Req, 
		                                        ProtoIndexes.ZhanDou_Notes_Resq.ToString());
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{	
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ZhanDou_Notes_Resq: 
			{
				MemoryStream t_stream = new MemoryStream (p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ZhandouRecordResp recordInfo = new ZhandouRecordResp();
				
				t_qx.Deserialize(t_stream, recordInfo, recordInfo.GetType());
				
				if (recordInfo != null)
				{
					if (recordInfo.info == null)
					{
						recordInfo.info = new List<ZhandouItem>();
					}
					Debug.Log ("对战记录返回！");
					recordResp = recordInfo;
				}

				return true;
			}

			default:return false;
			}
		}
		
		return false;
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
