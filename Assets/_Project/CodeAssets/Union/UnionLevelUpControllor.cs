using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionLevelUpControllor : MonoBehaviour, SocketProcessor
{
	public UnionDetailUIControllor controllor;

	public UILabel labelLevelTittle;

	public UILabel labelLevel;

	public UISlider barExp;

	public UILabel labelExp;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void refreshDate()
	{
		labelLevelTittle.text = controllor.unionDate.level + "";

		labelLevel.text = controllor.unionDate.level + "";
	}

	public void OnCancel()
	{
		gameObject.SetActive(false);
	}

	public void OnOK()
	{
//		SocketTool.Instance().Connect();
		
		UnionLevelupReq req = new UnionLevelupReq();
		
		req.unionId = controllor.unionDate.unionId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_LEVELUP_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_LEVELUP_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionLevelup bi = new UnionLevelup();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			OnCancel();
			
			return true;
		}
		}
		
		return false;
	}

}
