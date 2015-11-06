using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllMembersData : MonoBehaviour,SocketProcessor {

	public static AllMembersData membersData;

	public AllianceHaveResp m_allianceInfo;//联盟信息

	public LookMembersResp m_membersInfo;//成员信息
	
	public GameObject electAndVoteTips;//弹出参加竞选和投票窗口manager

	void Awake ()
	{
		membersData = this;

		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		m_allianceInfo = AllianceData.Instance.g_UnionInfo;

		AllianceMembersReq (m_allianceInfo.id);
	}

	//获得联盟信息
	public void GetAllianceInfo (AllianceHaveResp allianceResp)
	{
		m_allianceInfo = allianceResp;

		AllianceMembersReq (allianceResp.id);
	}

	//联盟成员信息请求
	public void AllianceMembersReq (int a_id)
	{
		LookMembers membersReq = new LookMembers ();
		
		membersReq.id = a_id;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,membersReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LOOK_MEMBERS,ref t_protof,"30116");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LOOK_MEMBERS_RESP://成员信息返回

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LookMembersResp membersResp = new LookMembersResp();
				
				t_qx.Deserialize(t_stream, membersResp, membersResp.GetType());
				
				if (membersResp != null)
				{	
					Debug.Log ("联盟成员信息返回");
					m_membersInfo = membersResp;
					
					ElectionAndVote elect_vote = electAndVoteTips.GetComponent<ElectionAndVote> ();
					elect_vote.GetAllianceInfo (m_allianceInfo,membersResp);

//					//刷新查看成员页面
//					GameObject checkMembers = GameObject.Find ("CheckMembers");
//					if (checkMembers != null)
//					{
//						AllianceMember member = checkMembers.GetComponent<AllianceMember> ();
//						member.GetMembersInfo (membersResp,m_allianceInfo);
//					}

//					//刷新入盟申请页面
//					GameObject applyAlliance = GameObject.Find ("ApplyAlliance");
//					if (applyAlliance != null)
//					{
//						AllianceApplicationData apply = applyAlliance.GetComponent<AllianceApplicationData> ();
//						apply.GetAllianceInfo (m_allianceInfo);
//					}

//					//刷新转让联盟页面
//					GameObject transAlliance = GameObject.Find ("TransAlliance");
//					if (transAlliance != null)
//					{
//						TransAlliance trans = transAlliance.GetComponent<TransAlliance> ();
//						trans.GetOwnLianMeng (m_allianceInfo,membersResp);
//					}

				}

				return true;
			}
		}

		return false;
	}

	//查看成员
	public void CheckUnionMember ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_CHECKMEMBERS ),
		                        CheckMembersLoadCallback );
	}

	//查看人员异步加载回调
	public void CheckMembersLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject memberObj = Instantiate( p_object ) as GameObject;
		
		memberObj.name = "CheckMembers";
		memberObj.transform.parent = this.transform.FindChild ("Camera").transform;
		memberObj.transform.localPosition = Vector3.zero;
		memberObj.transform.localScale = Vector3.one;
		
		AllianceMember member = memberObj.GetComponent<AllianceMember> ();
		member.GetMembersInfo (m_membersInfo,m_allianceInfo);
	}

	//入盟申请
	public void ApplyAlliance ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_APPLY ),
		                        AllianceApplyLoadCallback );
	}

	//入盟申请异步加载回调
	public void AllianceApplyLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject applyObj = Instantiate( p_object ) as GameObject;
		
		applyObj.name = "ApplyAlliance";
		applyObj.transform.parent = this.transform.FindChild ("Camera").transform;
		applyObj.transform.localPosition = Vector3.zero;
		applyObj.transform.localScale = Vector3.one;
		
		AllianceApplicationData apply = applyObj.GetComponent<AllianceApplicationData> ();
		apply.GetAllianceInfo (m_allianceInfo);
	}

	//盟主操作
	public void LeaderSetting ()
	{
		//g_UnionInfo//为联系信息 直接赋值即可
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_LEADER_SETTINGS ),
		                        LeaderSettingsLoadCallback );
	}

	//盟主操作异步加载回调
	public void LeaderSettingsLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject LederSet = Instantiate( p_object ) as GameObject;
		LederSet.transform.parent = this.transform;
		LederSet.transform.localScale = Vector3.one;
		LederSet.transform.localPosition = Vector3.zero;
		LeaderSetting mLeaderSetting = LederSet.GetComponent<LeaderSetting>();
		mLeaderSetting.m_UnionInfo = m_allianceInfo;
		mLeaderSetting.membersResp = m_membersInfo;
		mLeaderSetting.InItSetting ();
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
