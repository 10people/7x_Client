using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionLeaderSayControllor : MonoBehaviour, SocketProcessor
{
	public enum NoticeType
	{
		Leader_Say,
		Recruit,
	}

	public UnionDetailUIControllor controllor;

	public UIInput input;


	[HideInInspector] public NoticeType nType;


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
//		input.text = "";
		input.value = "";
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

	public void OnSure()
	{
//		if(nType == NoticeType.Leader_Say && input.text.Length > 0)//inner
		if(nType == NoticeType.Leader_Say && input.value.Length > 0)//inner
		{
//			SocketTool.Instance().Connect();
			
			UnionInnerEditReq req = new UnionInnerEditReq();
			
//			req.edit = input.text;
			req.edit = input.value;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_INNER_EDIT_REQ, ref t_protof);
		}
//		else if(nType == NoticeType.Recruit && input.text.Length > 0)//outer
		else if(nType == NoticeType.Recruit && input.value.Length > 0)//outer
		{
//			SocketTool.Instance().Connect();
			
			UnionOuterEditReq req = new UnionOuterEditReq();
			
//			req.edit = input.text;
			req.edit = input.value;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_OUTER_EDIT_REQ, ref t_protof);
		}
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_INNER_EDIT_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionInnerEdit bi = new UnionInnerEdit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			controllor.unionDate.unionSignInner = bi.edit;

			controllor.OnShowNoticeLayer();

			OnClose();

			return true;
		}
		case ProtoIndexes.UNION_OUTER_EDIT_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionOuterEdit bi = new UnionOuterEdit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			OnClose();
			
			return true;
		}
		}
		
		return false;
	}

}
