using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ApplicateItem : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UILabel levelLabel;//等级
	
	public UILabel nameLabel;//名字

	public GameObject infoWindowObj;//显示信息的窗口
	
	private ApplicantInfo applicantInfo;//入盟申请者信息
	
	private AllianceHaveResp m_allianceInfo;//联盟信息

	//获得入盟申请成员信息
	public void GetApplicateItemInfo (ApplicantInfo tempInfo,AllianceHaveResp allianceInfo)
	{
		applicantInfo = tempInfo;
		m_allianceInfo = allianceInfo;

		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;

		levelLabel.text = tempInfo.level.ToString ();
		
		nameLabel.text = tempInfo.name;
	}
	
	void OnClick ()
	{
		CheckApplicateInfo ();
	}

	//查看申请入盟成员信息
	void CheckApplicateInfo ()
	{
		Debug.Log ("就不给你看");

		Vector3 localPos = new Vector3 (40f,-60f,0);
		AllianceApplicationData.Instance().Select (this.gameObject,localPos);

		AllianceApplicationData.Instance().MakeZheZhao (true);
		
		GameObject infoWin = (GameObject)Instantiate (infoWindowObj);
		
		infoWin.SetActive (true);
		infoWin.name = "ApplicationWindow";
		
		infoWin.transform.parent = infoWindowObj.transform.parent;
		
		infoWin.transform.localPosition = infoWindowObj.transform.localPosition;
		
		infoWin.transform.localScale = infoWindowObj.transform.localScale;
		
		ApplicationInfoWindow applicate_info_win = infoWin.GetComponent<ApplicationInfoWindow> ();
		applicate_info_win.GetApplicationInfo (applicantInfo,m_allianceInfo);
	}
}
