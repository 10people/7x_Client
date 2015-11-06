using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionFriendControllor : MonoBehaviour, SocketProcessor
{
	public UnionUIControllor controllor;

	public GameObject friendAvatarTemplate;


	private int position;

	private List<GameObject> avatars = new List<GameObject>();


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void refreshDate(int _position)
	{
		position = _position;

		friendAvatarTemplate.SetActive(false);

		foreach(GameObject gc in avatars)
		{
			Destroy(gc);
		}

		avatars.Clear();

		sendRequest();
	}

	private void sendRequest()
	{
//		SocketTool.Instance().Connect();
		
		FriendListInitReq req = new FriendListInitReq();
		
		req.uid = (int)JunZhuData.Instance().m_junzhuInfo.id;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.GET_UNION_FRIEND_INFO_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.GET_UNION_FRIEND_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			FriendListInit bi = new FriendListInit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			initData(bi);
			
			return true;
		}
		}
		
		return false;
	}

	private void initData(FriendListInit fi)
	{
		controllor.friendList.Clear();

		for(int i = 0; i < fi.friends.Count; i++)
		{
			controllor.friendList.Add(fi.friends[i]);
		}

		for(int i = 0; i < controllor.friendList.Count; i++)
		{
			int row = i / 4;
			
			int col = i % 4;
			
			GameObject avatarObject = (GameObject)Instantiate(friendAvatarTemplate);
			
			avatarObject.SetActive(true);
			
			avatarObject.transform.parent = friendAvatarTemplate.transform.parent;
			
			avatarObject.transform.localScale = friendAvatarTemplate.transform.localScale;
			
			avatarObject.transform.localPosition = friendAvatarTemplate.transform.localPosition
				+ new Vector3(col * 200, - row * 210, 0);
			
			UnionFriendAvatar avatar = (UnionFriendAvatar)avatarObject.GetComponent("UnionFriendAvatar");
			
			avatar.refreshDate(controllor.friendList[i]);
		}
	}

	public void selectedAvatar(UnionFriendDate friendDate)
	{
		controllor.OnShowUnionFriend(position);

		controllor.OnShowUnionFriendDetail(friendDate, position);
	}

}
