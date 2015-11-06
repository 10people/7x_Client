using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionApplysControllor : MonoBehaviour, SocketProcessor
{
	public UnionDetailUIControllor controllor;

	public GameObject msgAvatarTemple;

	public GameObject applyAvatarTemple;


	private List<GameObject> msgList = new List<GameObject>();

	private List<GameObject> applyList = new List<GameObject>();

	private UnionMemberDate tempDate;


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
		msgAvatarTemple.SetActive(false);

		applyAvatarTemple.SetActive(false);
		
		foreach(GameObject gc in msgList)
		{
			Destroy(gc);
		}

		foreach(GameObject gc in applyList)
		{
			Destroy(gc);
		}

		msgList.Clear();
		
		applyList.Clear();

		for(int i = 0; controllor.unionDate.msgs != null && i < controllor.unionDate.msgs.Count; i++)
		{
			GameObject avatarObject = (GameObject)Instantiate(msgAvatarTemple);

			avatarObject.SetActive(true);

			avatarObject.transform.parent = msgAvatarTemple.transform.parent;

			avatarObject.transform.localScale = msgAvatarTemple.transform.localScale;

			avatarObject.transform.localPosition = msgAvatarTemple.transform.localPosition + new Vector3(0, -100 * i, 0);

			UnionApplyAvatar avatar = (UnionApplyAvatar)avatarObject.GetComponent("UnionApplyAvatar");

			avatar.refreshDate(controllor.unionDate.msgs[i]);

			msgList.Add(avatarObject);
		}
		
		for(int i = 0; controllor.unionDate.applys != null && i < controllor.unionDate.applys.Count; i++)
		{
			GameObject avatarObject = (GameObject)Instantiate(applyAvatarTemple);
			
			avatarObject.SetActive(true);
			
			avatarObject.transform.parent = applyAvatarTemple.transform.parent;
			
			avatarObject.transform.localScale = applyAvatarTemple.transform.localScale;
			
			avatarObject.transform.localPosition = applyAvatarTemple.transform.localPosition + new Vector3(0, -100 * i, 0);
			
			UnionApplyAvatar avatar = (UnionApplyAvatar)avatarObject.GetComponent("UnionApplyAvatar");
			
			avatar.refreshDate(controllor.unionDate.applys[i]);
			
			applyList.Add(avatarObject);
		}
	}

	public void OnAgree(UnionMemberDate date)
	{
		tempDate = date;

//		SocketTool.Instance().Connect();
		
		UnionListApplyReq req = new UnionListApplyReq();
		
		req.userId = date.memberId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_APPLY_REQ, ref t_protof);
	}

	public void OnInvite(UnionMemberDate date)
	{
		tempDate = date;
		
//		SocketTool.Instance().Connect();
		
		UnionListInviteReq req = new UnionListInviteReq();
		
		req.userId = date.memberId;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_INVITE_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_APPLY_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionListApply bi = new UnionListApply();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());

			controllor.unionDate.applys.Remove(tempDate);

			refreshDate();

			return true;
		}
		case ProtoIndexes.UNION_INVITE_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionListInvite bi = new UnionListInvite();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			controllor.unionDate.msgs.Remove(tempDate);
			
			refreshDate();
			
			return true;
		}
		}
		
		return false;
	}

}
