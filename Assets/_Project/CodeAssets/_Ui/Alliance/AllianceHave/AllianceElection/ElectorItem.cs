using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ElectorItem : MonoBehaviour {

	public UISprite headIcon;//头像

	public UILabel voteNumLabel;//票数

	public UILabel nameLabel;//名字

	public GameObject infoWindow;//选举人投票窗口

	private MemberInfo electorInfo;//参选人信息
	private AllianceHaveResp allianceInfo;//联盟信息

	public GameObject voteBtnsObj;

	//获得选举人信息
	public void GetElectorItemInfo (MemberInfo e_electorInfo,AllianceHaveResp a_allianceInfo)
	{
		electorInfo = e_electorInfo;
		allianceInfo = a_allianceInfo;

		voteNumLabel.text = e_electorInfo.voteNum.ToString () + "票";

		headIcon.spriteName = "PlayerIcon" + e_electorInfo.roleId;

		nameLabel.text = e_electorInfo.name;
	}

	void OnClick ()
	{
		Debug.Log ("你来投我啊");

		VoteManager voteMan = voteBtnsObj.GetComponent<VoteManager> ();
		voteMan.ShowVoteBtn (1);

		if (GameObject.Find ("SelectIt") != null)
		{
			Destroy (GameObject.Find ("SelectIt"));
		}

		AllianceElectionData.electionData.Select (this.gameObject,new Vector3(40f,-60f,0));

		ShowInfoWindow (electorInfo,allianceInfo);
	}

	//弹出参选人详情窗口
	void ShowInfoWindow (MemberInfo e_electorInfo,AllianceHaveResp a_allianceInfo)
	{
		AllianceElectionData.electionData.MakeZheZhao (true);

		GameObject infoWin = (GameObject)Instantiate (infoWindow);

		infoWin.SetActive (true);
		infoWin.name = "ElectorInfoWindow";

		infoWin.transform.parent = infoWindow.transform.parent;

		infoWin.transform.localPosition = infoWindow.transform.localPosition;

		infoWin.transform.localScale = infoWindow.transform.localScale;

		ElectorInfoWindow e_infoWin = infoWin.GetComponent<ElectorInfoWindow> ();
		e_infoWin.GetElectorInfo (e_electorInfo,a_allianceInfo);
	}
}
