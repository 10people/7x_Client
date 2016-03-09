using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class VoteManager : MonoBehaviour,SocketProcessor {

	private long junZhuId;//君主id
	private string junZhuName;//君主name

//	private int m_allianceId;//联盟id

	public GameObject voteBtn;
	public GameObject allianceElectionObj;

	private int code;

	private string confirmStr;
	private string cancelStr;

	private string confirmVoteTitleStr;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		confirmVoteTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_CONFIRM_TITLE);
	}

	//获得投票目标君主的id和名字
	public void GetJunZhuInfo (long id,string name,int allianceId)
	{
		junZhuId = id;

		junZhuName = name;

//		m_allianceId = allianceId;
	}

	//弃权
	public void Waiver ()
	{
		AllianceElectionData.electionData.MakeZheZhao (true);

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        WaiverLoadCallback );
	}

	public void WaiverLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_ASKSTR2);

		uibox.setBox(confirmVoteTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,cancelStr,confirmStr,ConfiremWaiver);
	}

	void ConfiremWaiver (int i)
	{
		if (i == 1)
		{
			AllianceElectionData.electionData.MakeZheZhao (false);
		}

		else if (i == 2)
		{
			WaiverReq ();
		}
	}

	//弃权请求
	void WaiverReq ()
	{
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.GIVEUP_VOTE,"30144");
	}

	//确定投票
	public void SureVote ()
	{
		AllianceElectionData.electionData.MakeZheZhao (true);

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        SureVoteLoadCallback );
	}

	public void SureVoteLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_TO_SOMEONE_DES1);

		uibox.setBox(confirmVoteTitleStr, MyColorData.getColorString (1,str), MyColorData.getColorString (1,junZhuName), 
		             null,cancelStr,confirmStr,ConfiremVote);
	}

	void ConfiremVote (int i)
	{
		if (i == 1)
		{
			AllianceElectionData.electionData.DestroySelect ();
			AllianceElectionData.electionData.MakeZheZhao (false);
		}
		
		else if (i == 2)
		{
			VoteReq ();
		}
	}

	//投票请求
	void VoteReq ()
	{
		MengZhuVote vote = new MengZhuVote ();
		
		vote.junzhuId = junZhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,vote);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.MENGZHU_VOTE,ref t_protof,"30142");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.MENGZHU_VOTE_RESP://确认投票返回

				MemoryStream vote_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);

				QiXiongSerializer vote_qx = new QiXiongSerializer();

				MengZhuVoteResp voteResp = new MengZhuVoteResp();

				vote_qx.Deserialize (vote_stream,voteResp,voteResp.GetType ());

				if (voteResp != null)
				{
//					long vote_junZhuId = voteResp.junzhuId;

//					PlayerPrefs.SetInt("ElectorId",(int)junZhuId);

//					int voteNum = voteResp.voteNum;

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        VoteSuccessLoadCallback );
				}

				return true;

			case ProtoIndexes.GIVEUP_VOTE_RESP://确认弃权返回

				MemoryStream waiver_stream = new MemoryStream(p_message.m_protocol_message,0,p_message.position);
				
				QiXiongSerializer waiver_qx = new QiXiongSerializer();
				
				GiveUpVoteResp waiverResp = new GiveUpVoteResp();
				
				waiver_qx.Deserialize (waiver_stream,waiverResp,waiverResp.GetType ());

				if (waiverResp != null)
				{
					code = waiverResp.code;//0-放弃成功 1-放弃失败

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        GiveUpLoadCallback );
				}

				return true;
			}
		}

		return false;
	}

	public void VoteSuccessLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_TO_SOMEONE_DES2);

		string voteSuccessTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_SUCCESS_TITLE);

		uibox.setBox(voteSuccessTitleStr, MyColorData.getColorString (1,str), MyColorData.getColorString (1,junZhuName), 
		             null,confirmStr,null,SureClose);
	}

	public void GiveUpLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		switch (code)
		{
		case 0:

			string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_DES);

			string voteGiveUpTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_TITLE);

			uibox.setBox(voteGiveUpTitleStr, null, MyColorData.getColorString (1,str1), 
			             null,confirmStr,null,SureClose);
			
			break;
			
		case 1:

			string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_FAILDES);

			string voteGiveUpFailTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_VOTE_GIVEUP_FAIL);

			uibox.setBox(voteGiveUpFailTitleStr, null, MyColorData.getColorString (1,str2), 
			             null,confirmStr,null,SureClose);
			
			break;
		}
	}

	void SureClose (int i)
	{
		AllianceData.Instance.RequestData ();
		AllianceElectionData.electionData.MakeZheZhao (false);
		Destroy (allianceElectionObj);
	}

	//控制voteBtn的显隐
	public void ShowVoteBtn (int i)
	{
		if (i == 1)
		{
			voteBtn.SetActive (true);
		}

		else if (i == 2)
		{
			voteBtn.SetActive (false);
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
