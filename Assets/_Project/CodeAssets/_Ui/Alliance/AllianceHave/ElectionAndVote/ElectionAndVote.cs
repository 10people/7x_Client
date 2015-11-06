using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ElectionAndVote : MonoBehaviour,SocketProcessor {

	public GameObject zhezhao;
	private AllianceHaveResp allianceData;//联盟返回信息
	private LookMembersResp membersInfo;//成员信息
	public GameObject cameraObj;
	private int code;

	private string leaderName;//盟主名字

	private string confirmStr;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}

	//获得联盟信息
	public void GetAllianceInfo (AllianceHaveResp allianceInfo,LookMembersResp membersResp)
	{
		Debug.Log ("有通知吗？");
		allianceData = allianceInfo;
		membersInfo = membersResp;

		leaderName = allianceInfo.mengzhuName;

		GetState (allianceInfo);
	}

	void GetState (AllianceHaveResp tempInfo)
	{
		switch (tempInfo.status)
		{
		case 0://正常

			if (AllianceData.Instance.isNewLeader)
			{
				if (tempInfo.isVoteDialog == 1)
				{
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        NewLeaderLoadCallback );

					AllianceData.Instance.isNewLeader = false;
				}
			}

			Debug.Log ("风平浪静，毛线没有！");

			break;

		case 1://选举报名

			Debug.Log ("俺要选盟主！");

			if (tempInfo != null)
			{
				if (tempInfo.isBaoming != null)
				{
					if (tempInfo.isBaoming == 0)//0-未报名，1-已报名
					{
						ElectionTips ();
					}
					
					else if (tempInfo.isBaoming == 1)
					{
						Debug.Log ("已经报过名了，坐等盟主之位");
					}
				}
			}

			break;

		case 2://投票

			Debug.Log ("投票喽！");

			if (tempInfo != null)
			{
				if (tempInfo.isVoted != null)
				{
					Debug.Log ("投票状态?：" + tempInfo.isVoted);
					//0-未投票，1-已投票，2-弃权
					if (tempInfo.isVoted == 0)
					{
						VoteTips ();
					}
					
					else if (tempInfo.isVoted == 1)
					{
						Debug.Log ("已投票");
					}
					
					else if (tempInfo.isVoted == 2)
					{
						Debug.Log ("弃权");
					}
				}
			}

			break;
		}
	}

	public void NewLeaderLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NEW_LEADER_DES1);
		string str2 = "     " + leaderName + "     " + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NEW_LEADER_DES2);

		string newLeaderTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NEW_LEADER_TITLE);

		uibox.setBox(newLeaderTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,confirmStr,null,null);
	}

	//弹出参加选举公告
	void ElectionTips ()
	{
		zhezhao.SetActive (true);
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ElectLoadCallback );
	}

	public void ElectLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_LEADER_SELECT_DES1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_LEADER_SELECT_DES2);

		string leaderElectTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_LEADER_SELECT_TITLE);

		string notJoin = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NOT_JOIN);
		string join = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JOIN_CAMPAIGN);

		uibox.setBox(leaderElectTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,notJoin,join,ConfiremElect);
	}

	//取消或确定报名
	void ConfiremElect (int i)
	{
		if (i == 1)
		{
			zhezhao.SetActive (false);
		}

		else if (i == 2)
		{
			ElectionReq ();
		}
	}

	//弹出投票公告
	void VoteTips ()
	{
		zhezhao.SetActive (true);
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        VoteLoadCallback );
	}

	public void VoteLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_LEADER_ELECT_DES);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_NOW_ASK);

		string electTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_LEADER_ELECT_TITLE);

		string notVote = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_NOT_VOTE);
		string voteNow = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_NOW);

		uibox.setBox(electTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,notVote,voteNow,ConfiremVote);
	}

	//取消或确定去投票
	void ConfiremVote (int i)
	{
		zhezhao.SetActive (false);

		if (i == 2)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_ELECTION ),
			                        VoteSelectLoadCallback );
		}
	}

	public void VoteSelectLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject electAndVote = Instantiate( p_object ) as GameObject;
		
		electAndVote.transform.parent = cameraObj.transform;
		electAndVote.transform.localPosition = Vector3.zero;
		electAndVote.transform.localScale = Vector3.one;
		
		AllianceElectionData electAndVoteData = electAndVote.GetComponent<AllianceElectionData> ();
		electAndVoteData.GetOwnLianMeng (allianceData,membersInfo);
	}

	//参加选举请求
	void ElectionReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.MENGZHU_APPLY,"30140");
	}
	
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.MENGZHU_APPLY_RESP://参加投票返回

				MemoryStream elect_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer elect_qx = new QiXiongSerializer();
				
				MengZhuApplyResp electResp = new MengZhuApplyResp();
				
				elect_qx.Deserialize(elect_stream, electResp, electResp.GetType());

				if (electResp != null)
				{
					code = electResp.code;

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ElectRespLoadCallback );
				}

				return true;
			}
		}

		return false;
	}

	public void ElectRespLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;

		UIBox uibox = boxObj.GetComponent<UIBox> ();

		if (code == 0)//报名成功
		{
			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JOIN_SUCCESS);
			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_ELECT_TOMORROW);

			uibox.setBox(str1, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
			             null,confirmStr,null,CloseTipWindow);
		}
		
		if (code == 1)//报名失败
		{
			string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JOIN_FAIL);

			uibox.setBox(str, null, MyColorData.getColorString (1,str), 
			             null,confirmStr,null,CloseTipWindow);
		}
	}

	//关闭弹窗
	void CloseTipWindow (int i)
	{
		AllianceData.Instance.RequestData ();
		zhezhao.SetActive (false);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
