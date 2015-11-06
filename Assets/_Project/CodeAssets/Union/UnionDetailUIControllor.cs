using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UnionDetailUIControllor : MonoBehaviour, SocketProcessor
{
	public UnionDetailNoticeControllor noticeControllor;

	public UnionMemberControllor memberControllor;

	public UnionApplysControllor applysControllor;

	public UnionHintControllor hintControllor;

	public UnionLeaderSayControllor leaderSayControllor;

	public UnionMemberDetailControllor memberDetailControllor;

	public UnionLevelUpControllor levelupControllor;


	[HideInInspector] public UnionDate unionDate;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	void Start ()
	{
		OnSend();
	}

	public void OnSend()
	{
//		SocketTool.Instance().Connect();
		
		UnionDetailInitReq req = new UnionDetailInitReq();
		
		req.uid = (int)JunZhuData.Instance().m_junzhuInfo.id;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.UNION_DETAIL_INFO_REQ, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;

		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.UNION_DETAIL_INFO:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			UnionDetailtInit bi = new UnionDetailtInit();
			
			t_qx.Deserialize(t_stream, bi, bi.GetType());
			
			unionDate = bi.union;

			OnShowNoticeLayer();
			
			return true;
		}
		}
		
		return false;
	}

	/*
	private void initDate()
	{
		{
			unionDate = new UnionDate();

			unionDate.unionSignInner = "欢迎来到迪菲亚兄弟会，每周五晚上9点半活动。这周通BT，买大药。";
			
			unionDate.member = 248;
			
			unionDate.leaderId = 0;
			
			unionDate.apply = false;
			
			unionDate.shengwang = 4862003;
			
			unionDate.unionName = "迪菲亚兄弟会";
			
			unionDate.unionId = 1;
			
			unionDate.leaderName = "墩饽饽";
			
			unionDate.unionIcon = 0;

			unionDate.renkou = 850;

			unionDate.level = 28;

			unionDate.exp = 15;

			unionDate.members = new List<UnionMemberDate>();

			for(int i = 0; i < 51; i++)
			{
				UnionMemberDate mDate = new UnionMemberDate();

				mDate.memberId = i;

				mDate.memberName = "第" + i + "个成员";

				mDate.level = 5;

				mDate.grade = "东厂大太监";

				mDate.junXian = 1;

				mDate.shengWang = i * 10 + 2150;

				unionDate.members.Add(mDate);
			}

			unionDate.applys = new List<UnionMemberDate>();

			for(int i = 0; i < 15; i++)
			{
				UnionMemberDate mDate = new UnionMemberDate();

				mDate.memberId = i;

				mDate.memberName = "第" + i + "个申请人";
				
				mDate.level = 5 + i;
				
				mDate.junXian = 1;
				
				unionDate.applys.Add(mDate);
			}

			unionDate.msgs = new List<UnionMemberDate>();

			for(int i = 0; i < 25; i++)
			{
				UnionMemberDate mDate = new UnionMemberDate();
				
				mDate.memberId = i;
				
				mDate.memberName = "第" + i + "个消息人";
				
				mDate.level = 10 + i * 2;
				
				mDate.junXian = 1;
				
				unionDate.msgs.Add(mDate);
			}
		}

		OnShowNoticeLayer();
	}
	*/

	private void OnCloseLayer()
	{
		noticeControllor.gameObject.SetActive(false);

		memberControllor.gameObject.SetActive(false);

		applysControllor.gameObject.SetActive(false);

		hintControllor.gameObject.SetActive(false);

		leaderSayControllor.gameObject.SetActive(false);

		memberDetailControllor.gameObject.SetActive(false);

		levelupControllor.gameObject.SetActive(false);
	}
	
	public void OnShowNoticeLayer()
	{
		OnCloseLayer();
		
		noticeControllor.gameObject.SetActive(true);
		
		noticeControllor.refreshDate(unionDate);
	}

	public void OnShowMemberList()
	{
		OnCloseLayer();

		memberControllor.gameObject.SetActive(true);

		memberControllor.refreshDate();
	}

	public void OnShowApplys()
	{
		OnCloseLayer();

		applysControllor.gameObject.SetActive(true);

		applysControllor.refreshDate();
	}

	public void OnShowHint(UnionHintControllor.HintType _hintType, UnionMemberDate focusMember)
	{
		hintControllor.gameObject.SetActive(true);

		hintControllor.refreshDate(_hintType, focusMember);
	}

	public void OnShowLeaderSay()
	{
		leaderSayControllor.gameObject.SetActive(true);

		leaderSayControllor.nType = UnionLeaderSayControllor.NoticeType.Leader_Say;

		leaderSayControllor.refreshDate();
	}

	public void OnShowRecruitNotice()
	{
		leaderSayControllor.gameObject.SetActive(true);
		
		leaderSayControllor.nType = UnionLeaderSayControllor.NoticeType.Recruit;

		leaderSayControllor.refreshDate();
	}

	public void OnShowMemberDetail(UnionMemberDate memberDate)
	{
		memberDetailControllor.gameObject.SetActive(true);

		memberDetailControllor.refreshDate(memberDate);
	}

	public void OnShowLevelup()
	{
		levelupControllor.gameObject.SetActive(true);

		levelupControllor.refreshDate();
	}

}
