using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionEditControllor : MonoBehaviour, SocketProcessor
{
	public UIInput input;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

	public void OnSend()
	{
//		SocketTool.Instance().Connect();
		
		UnionListEditReq req = new UnionListEditReq();
		
//		req.edit = input.text;
		req.edit = input.value;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_EDIT_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_EDIT_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionListEdit bi = new UnionListEdit();

			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			OnClose();
			
			return true;
		}
		}
		
		return false;
	}

}
