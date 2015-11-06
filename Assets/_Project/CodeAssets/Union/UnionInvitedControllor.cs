using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionInvitedControllor : MonoBehaviour, SocketProcessor
{
	public UnionGeneralControllor generalControllor;

	public UnionUIControllor controllor;

	public UILabel labelName_1;

	public UILabel labelName_2;

	public UILabel labelLeader;

	public UILabel labelShengWang;

	public UILabel labelMember;

	public UILabel labelDesc;


	[HideInInspector] public UnionDate date;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void refreshDate(UnionDate _date)
	{
		date = _date;

		labelName_1.text = date.unionName;

		labelName_2.text = date.unionName;

		labelLeader.text = date.leaderName;

		labelShengWang.text = date.shengwang + "";

		labelMember.text = date.member + "";

		labelDesc.text = date.unionSignOuter;
	}

	public void OnCancel()
	{
//		SocketTool.Instance().Connect();
		
		UnionRefuseInviteReq req = new UnionRefuseInviteReq();
		
		req.unionId = date.unionId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_INVITED_REFUSE_REQ, ref t_protof);
	}

	public void OnAgree()
	{
//		SocketTool.Instance().Connect();
		
		UnionAgreeInviteReq req = new UnionAgreeInviteReq();
		
		req.unionId = date.unionId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_INVITED_AGREE_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_INVITED_REFUSE_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionRefuseInvite bi = new UnionRefuseInvite();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());

			controllor.OnShowNoticeLayer();

			return true;
		}
		case ProtoIndexes.UNION_INVITED_AGREE_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionAgreeInvite bi = new UnionAgreeInvite();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			generalControllor.OnShowUnion_2();
			
			return true;
		}
		}
		
		return false;
	}

}
