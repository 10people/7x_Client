using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MemberItem : MonoBehaviour {

	public UISprite headIcon;//头像icon

	public UISprite zhiWei;//联盟中的职位

	public UILabel levelLabel;//等级

	public UILabel nameLabel;//名字

	public GameObject infoWindowObj;//显示信息的窗口

	private MemberInfo thisMemberInfo;//成员信息

	private AllianceHaveResp m_allianceInfo;//联盟信息

	//获得成员信息
	public void ShowMemberItemInfo (MemberInfo tempInfo,AllianceHaveResp allianceInfo)
	{
		thisMemberInfo = tempInfo;
		m_allianceInfo = allianceInfo;
//		Debug.Log ("名字：" + tempInfo.name + "/" + "身份：" + tempInfo.identity);
		switch (tempInfo.identity)
		{
		case 1:

			zhiWei.gameObject.SetActive (true);
			zhiWei.spriteName = "fumenzhu";

			break;

		case 2:

			zhiWei.gameObject.SetActive (true);
			zhiWei.spriteName = "mengzhu";

			break;
		}

		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;

		levelLabel.text = tempInfo.level.ToString ();

		nameLabel.text = tempInfo.name;
	}

	void OnClick ()
	{
		CheckMemberInfo ();
	}

	//弹出成员信息详情窗口
	void CheckMemberInfo ()
	{
		Debug.Log ("就不给你看");

		Vector3 localPos = new Vector3 (40f,-60f,0);
		AllianceMember.a_member.Select (this.gameObject,localPos);

		AllianceMember.a_member.MakeZheZhao (true);
		
		GameObject infoWin = (GameObject)Instantiate (infoWindowObj);
		
		infoWin.SetActive (true);
		infoWin.name = "MemberInfoWindow";
		
		infoWin.transform.parent = infoWindowObj.transform.parent;
		
		infoWin.transform.localPosition = infoWindowObj.transform.localPosition;
		
		infoWin.transform.localScale = infoWindowObj.transform.localScale;
		
		MemberInfoWindow m_info_win = infoWin.GetComponent<MemberInfoWindow> ();
		m_info_win.ShowCheckInfo (thisMemberInfo,m_allianceInfo);
		
		MemberInfoBtns m_info_btn = infoWin.GetComponent<MemberInfoBtns> ();
		m_info_btn.GetThisMemberInfo (thisMemberInfo,m_allianceInfo);
	}
}
