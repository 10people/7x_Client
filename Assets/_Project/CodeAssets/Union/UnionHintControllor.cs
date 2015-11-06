using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionHintControllor : MonoBehaviour, SocketProcessor
{
	public enum HintType
	{
		Dismis,
		Quit,
		Transfer,
		Advance,
		Demotion,
		Remove,
	}


	public UnionDetailUIControllor controllor;

	public UILabel labelDismis;

	public UILabel labelQuit;

	public UILabel labelTransfer;

	public UILabel labelAdvance;

	public UILabel labelDemotion;

	public UILabel labelRemove;

	public TimerButton btn;


	[HideInInspector] public HintType hintType;


	private UnionMemberDate focusMember;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void refreshDate(HintType _hintType, UnionMemberDate _focusMember)
	{
		hintType = _hintType;

		focusMember = _focusMember;

		labelDismis.gameObject.SetActive(hintType == HintType.Dismis);

		labelQuit.gameObject.SetActive(hintType == HintType.Quit);

		labelTransfer.gameObject.SetActive(hintType == HintType.Transfer);

		labelAdvance.gameObject.SetActive(hintType == HintType.Advance);

		labelDemotion.gameObject.SetActive(hintType == HintType.Demotion);

		labelRemove.gameObject.SetActive(hintType == HintType.Remove);

		btn.Restart();
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

	public void OnSure()
	{
		if(hintType == HintType.Dismis)
		{
//			SocketTool.Instance().Connect();
			
			UnionDismissReq req = new UnionDismissReq();
			
			req.unionId = controllor.unionDate.unionId;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_DISMISS_REQ, ref t_protof);
		}
		else if(hintType == HintType.Quit)
		{
//			SocketTool.Instance().Connect();
			
			UnionQuitReq req = new UnionQuitReq();
			
			req.unionId = controllor.unionDate.unionId;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_QUIT_REQ, ref t_protof);
		}
		else if(hintType == HintType.Transfer)
		{
//			SocketTool.Instance().Connect();
			
			UnionTransferReq req = new UnionTransferReq();
			
			req.unionId = controllor.unionDate.unionId;

			req.userId = focusMember.memberId;

			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_TRANSFER_REQ, ref t_protof);
		}
		else if(hintType == HintType.Advance)
		{
//			SocketTool.Instance().Connect();
			
			UnionAdvanceReq req = new UnionAdvanceReq();
			
			req.unionId = controllor.unionDate.unionId;
			
			req.userId = focusMember.memberId;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_ADVANCE_REQ, ref t_protof);
		}
		else if(hintType == HintType.Demotion)
		{
//			SocketTool.Instance().Connect();
			
			UnionDemotionReq req = new UnionDemotionReq();
			
			req.unionId = controllor.unionDate.unionId;
			
			req.userId = focusMember.memberId;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_DEMOTION_REQ, ref t_protof);
		}
		else if(hintType == HintType.Remove)
		{
//			SocketTool.Instance().Connect();
			
			UnionRemoveReq req = new UnionRemoveReq();
			
			req.unionId = controllor.unionDate.unionId;
			
			req.userId = focusMember.memberId;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_REMOVE_REQ, ref t_protof);
		}
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_DISMISS_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionDismiss bi = new UnionDismiss();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());

			gameObject.SetActive(false);
			
			UnionGeneralControllor.Instance().OnShowUnion_1();

			return true;
		}
		case ProtoIndexes.UNION_QUIT_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionQuit bi = new UnionQuit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			gameObject.SetActive(false);
			
			UnionGeneralControllor.Instance().OnShowUnion_1();
			
			return true;
		}
		case ProtoIndexes.UNION_TRANSFER_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionTransferReq bi = new UnionTransferReq();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			gameObject.SetActive(false);
			
			return true;
		}
		case ProtoIndexes.UNION_ADVANCE_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionAdvance bi = new UnionAdvance();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			gameObject.SetActive(false);
			
			return true;
		}
		case ProtoIndexes.UNION_DEMOTION_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionDemotion bi = new UnionDemotion();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			gameObject.SetActive(false);
			
			return true;
		}
		case ProtoIndexes.UNION_REMOVE_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionRemove bi = new UnionRemove();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			gameObject.SetActive(false);
			
			return true;
		}
		}
		
		return false;
	}

}
