using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceLastItem : MonoBehaviour {

	public UILabel rankLabel;

	public UILabel nameLabel;

	public UILabel idLabel;

	//获得上届对战记录条目信息
	public void GetLastItemInfo (FightRankInfo tempInfo)
	{
		rankLabel.text = tempInfo.rank.ToString ();

		nameLabel.text = tempInfo.lmName;

		idLabel.text = tempInfo.lmId.ToString ();
	}
}
