using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ElectorInfoItem : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UILabel voteNumLabe;//得票数
	
	public UILabel nameLabel;//名字
	
	public GameObject infoWindowObj;//显示信息的窗口

	private MemberInfo thisMemberInfo;//成员信息
	
	private AllianceHaveResp m_allianceInfo;//联盟信息
	
	public GameObject selectObj;//标记所投君主的对号
	private long junZhuId;//君主id
	
	//获得选举人信息和联盟信息
	public void GetElectorInfo (MemberInfo m_info,AllianceHaveResp allianceInfo)
	{
		thisMemberInfo = m_info;
		m_allianceInfo = allianceInfo;

		voteNumLabe.text = m_info.voteNum.ToString () + "票";

		headIcon.spriteName = "PlayerIcon" + m_info.roleId;

		nameLabel.text = m_info.name;

		CloneSelectObj (m_info.junzhuId);
	}

	void OnClick ()
	{
		CheckElectorInfo ();
	}
	
	//弹出选举人信息详情窗口
	void CheckElectorInfo ()
	{
		Debug.Log ("就不给你看");
		
		GameObject infoWin = (GameObject)Instantiate (infoWindowObj);
		
		infoWin.SetActive (true);
		infoWin.name = "ElectorInfoWindow";
		
		infoWin.transform.parent = infoWindowObj.transform.parent;
		
		infoWin.transform.localPosition = infoWindowObj.transform.localPosition;
		
		infoWin.transform.localScale = infoWindowObj.transform.localScale;
		
		CheckElectorInfo check = infoWin.GetComponent<CheckElectorInfo> ();
		check.ShowCheckInfo (thisMemberInfo,m_allianceInfo);
	}

	void CloneSelectObj (long id)
	{
		if (id == m_allianceInfo.voteJunzhuId)
		{
			GameObject select = (GameObject)Instantiate (selectObj);
			
			select.SetActive (true);
			
			select.transform.parent = this.transform;
			
			select.transform.localPosition = new Vector3 (40,-60,0);
			
			select.transform.localScale = Vector3.one;
		}
	}
}
