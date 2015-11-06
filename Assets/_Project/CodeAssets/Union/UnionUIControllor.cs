using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionUIControllor : MonoBehaviour, SocketProcessor
{
	public UnionNoticeControllor noticeControllor;

	public UnionDetailControllor detailControllor;

	public UnionFriendControllor friendControllor;

	public UnionInvitedControllor invitedControllor;

	public UnionCreateControllor createControllor;

	public UnionSignControllor signControllor;

	public UnionEditControllor editControllor;

	public UnionFriendDetailControllor friendDetailControllor;


	[HideInInspector] public List<UnionDate> unionList = new List<UnionDate>();

	[HideInInspector] public List<UnionFriendDate> friendList = new List<UnionFriendDate>();


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}

	void Start ()
	{
//		SocketTool.Instance().Connect();
		
		UnionListInitReq req = new UnionListInitReq();
		
		req.uid = (int)JunZhuData.Instance().m_junzhuInfo.id;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.GET_UNION_INFO_REQ, ref t_protof);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;

		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.GET_UNION_INFO_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionListInit bi = new UnionListInit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			initData(bi);
			
			return true;
		}
		}
		
		return false;
	}

	private void initData(UnionListInit da)
	{
		unionList.Clear();

		for(int i = 0; da.unions != null && i < da.unions.Count; i++)
		{
			UnionDate date = da.unions[i];

			date.apply = false;

			unionList.Add(date);
		}

		for(int i = 0; da.applyUnions != null && i < da.applyUnions.Count; i++)
		{
			int id = da.applyUnions[i];

			markUnionApply(id);
		}

		if(da.inviteUnionId == 0)
		{
			OnShowNoticeLayer();
		}
		else
		{
			foreach(UnionDate date in unionList)
			{
				if(date.unionId == da.inviteUnionId)
				{
					OnShowInvited(date);

					break;
				}
			}
		}
	}

	private void markUnionApply(int unionId)
	{
		foreach(UnionDate union in unionList)
		{
			if(union.unionId == unionId)
			{
				union.apply = true;

				return;
			}
		}
	}

	private void OnCloseLayer()
	{
		noticeControllor.gameObject.SetActive(false);

		detailControllor.gameObject.SetActive(false);

		friendControllor.gameObject.SetActive(false);

		invitedControllor.gameObject.SetActive(false);

		createControllor.gameObject.SetActive(false);

		signControllor.gameObject.SetActive(false);

		editControllor.gameObject.SetActive(false);

		friendDetailControllor.gameObject.SetActive(false);
	}

	public void OnShowNoticeLayer()
	{
		OnCloseLayer();

		noticeControllor.gameObject.SetActive(true);

		noticeControllor.refreshDate();
	}

	public void OnShowUnionDetail(UnionDate unionDate)
	{
		OnCloseLayer();

		detailControllor.gameObject.SetActive(true);

		detailControllor.refreshDate(unionDate);
	}

	public void OnShowUnionFriend(int position)
	{
		OnCloseLayer();

		friendControllor.gameObject.SetActive(true);

		friendControllor.refreshDate(position);
	}

	public void OnShowInvited(UnionDate date)
	{
		OnCloseLayer();

		invitedControllor.gameObject.SetActive(true);

		invitedControllor.refreshDate(date);
	}

	public void OnShowUnionCreate()
	{
		createControllor.gameObject.SetActive(true);
	}

	public void OnShowUnionSign()
	{
		createControllor.gameObject.SetActive(false);

		signControllor.gameObject.SetActive(true);
	}

	public void refreshSignControllor(UnionFriendDate date, int position, string _unionName, int _iconId)
	{
		if(date == null)
		{
			signControllor.refreshDate(_unionName, _iconId);
		}
		else
		{
			signControllor.setSignDate(position, date);
		}
	}

	public void OnShowUnionEdit()
	{
		editControllor.gameObject.SetActive(true);
	}

	public void OnShowUnionFriendDetail(UnionFriendDate friendDate, int position)
	{
		friendDetailControllor.gameObject.SetActive(true);

		friendDetailControllor.refreshDate(friendDate, position);
	}

}
