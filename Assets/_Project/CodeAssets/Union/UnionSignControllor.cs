using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionSignControllor : MonoBehaviour, SocketProcessor
{
	public UnionSignAvatar avatar_1;

	public UnionSignAvatar avatar_2;

	public UnionSignAvatar avatar_3;


	private string unionName;

	private int iconId;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void refreshDate(string _unionName, int _iconId)
	{
		unionName = _unionName;

		iconId = _iconId;

		avatar_1.setFriendDate(null);

		avatar_2.setFriendDate(null);

		avatar_3.setFriendDate(null);
	}

	public void setSignDate(int position, UnionFriendDate date)
	{
		UnionSignAvatar curAvatar = null;

		if(position == 0)
		{
			curAvatar = avatar_1;
		}
		else if(position == 1)
		{
			curAvatar = avatar_2;
		}
		else if(position == 2)
		{
			curAvatar = avatar_3;
		}
		else
		{
			return;
		}

		curAvatar.setFriendDate(date);
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

	public void OnSend()
	{
		//List<int> ids = new List<int>();

		//if(avatar_1.friendDate == null || avatar_2.friendDate == null || avatar_3.friendDate == null) return;

		//ids.Add(avatar_1.friendDate.friendId);

		//ids.Add(avatar_2.friendDate.friendId);

		//ids.Add(avatar_3.friendDate.friendId);

//		SocketTool.Instance().Connect();
		
		UnionListCreateReq req = new UnionListCreateReq();
		
		req.unionName = unionName;

		req.iconId = iconId;

		//req.friendsId = ids;
		
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
			
			return true;
		}
		}
		
		return false;
	}

}
