using UnityEngine;
using System.Collections;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionDetailControllor : MonoBehaviour, SocketProcessor
{
	public UnionUIControllor controllor;

	public UILabel labelUnionName;

	public UILabel labelLeaderName;

	public UILabel labelShengWang;

	public UILabel labelMember;

	public UILabel labelSign;

	public GameObject btnApply;

	public GameObject btnCancel;


	private UnionDate unionDate;


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
		unionDate = _date;

		labelUnionName.text = unionDate.unionName;

		labelLeaderName.text = unionDate.leaderName;

		labelShengWang.text = unionDate.shengwang + "";

		labelMember.text = unionDate.member + "";

		labelSign.text = unionDate.unionSignOuter;

		btnApply.SetActive(!unionDate.apply);

		btnCancel.SetActive(unionDate.apply);
	}

	public void back()
	{
		controllor.OnShowNoticeLayer();
	}

	public void OnApply()
	{
//		SocketTool.Instance().Connect();
		
		UnionApllyJoinReq req = new UnionApllyJoinReq();
		
		req.unionId = unionDate.unionId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_APPLY_JION_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_APPLY_JION_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionApllyJoin bi = new UnionApllyJoin();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			back();
			
			return true;
		}
		}
		
		return false;
	}

}
