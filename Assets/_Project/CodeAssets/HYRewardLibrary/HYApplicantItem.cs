using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HYApplicantItem : MonoBehaviour {

	private ApplyerInfo applicantInfo;

	public UISprite icon;

	public UILabel nameLabel;

	public UILabel gongXianLabel;

	public GameObject divideBtnColObj;

	public void GetApplicantInfo (ApplyerInfo tempInfo)
	{
		applicantInfo = tempInfo;

		icon.spriteName = "PlayerIcon" + tempInfo.iconId;

		nameLabel.text = tempInfo.name;

		gongXianLabel.text = tempInfo.gongXian.ToString ();
	}

	void OnClick ()
	{
		if (HYRewardLibrary.hyReward.m_allianceResp.identity == 2)
		{
			if (this.transform.FindChild ("ApplayerBox") == null)
			{
				HYRewardLibrary.hyReward.CreateApplayerBox (this.gameObject);
				HYRewardBtnsCol.rewardBtnsCol.GetApplicantInfo (applicantInfo);
			}
		}
	}
}
