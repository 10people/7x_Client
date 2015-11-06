using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionCreateControllor : MonoBehaviour, SocketProcessor
{
	public UnionGeneralControllor generalControllor;

	public UnionUIControllor controllor;

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

	public void enterSign()
	{
		controllor.OnShowUnionSign();

//		controllor.refreshSignControllor(null, 0, input.text, 0);
		controllor.refreshSignControllor(null, 0, input.value, 0);
	}

	public void OnCreate()
	{
//		SocketTool.Instance().Connect();
		
		UnionListCreateReq req = new UnionListCreateReq();
		
//		req.unionName = input.text;
		req.unionName = input.value;

		req.iconId = 0;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.CREATE_UNION_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.CREATE_UNION_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionListCreate bi = new UnionListCreate();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());

			OnClose();

			generalControllor.OnShowUnion_2();

			return true;
		}
		}
		
		return false;
	}

}
