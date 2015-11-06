using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceTips : MonoBehaviour {

	public UILabel levelLabel;

	public UILabel nameLabel;

	public UILabel memberLabel;

	private UIPanel tipsPanel;

	[HideInInspector]public bool isAlpha;

	void Start ()
	{
		tipsPanel = this.GetComponent<UIPanel> ();
	}

	//获得联盟信息
	public void GetAllianceInfo (LianMengInfo tempInfo)
	{
//		Debug.Log ("tempInfo:" + tempInfo.level);

		levelLabel.text = tempInfo.level.ToString ();

		nameLabel.text = tempInfo.mengName;

		memberLabel.text = tempInfo.member.ToString () + "/" + tempInfo.allMember.ToString ();
	}

	void Update ()
	{
		if (isAlpha)
		{
			tipsPanel.alpha = 1;
		}

		else
		{
			tipsPanel.alpha -= 0.05f;
		}
	}
}
