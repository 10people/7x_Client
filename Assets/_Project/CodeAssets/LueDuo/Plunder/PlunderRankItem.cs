using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderRankItem : MonoBehaviour {

	public UISprite m_bg;
	public UISprite rankSprite;
	public UILabel rankLabel;
	public UILabel nameLabel;
	public UILabel numLabel;

	/// <summary>
	/// Ins it rank item.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItRankItem (PlunderData.PlunderRankType tempType,GongJinInfo tempInfo)
	{
		SetBg (false);
		rankSprite.spriteName = "rank" + tempInfo.rank;
		rankLabel.text = tempInfo.rank > 3 ? tempInfo.rank.ToString () : "";

		nameLabel.text = tempType == PlunderData.PlunderRankType.ALLIANCE_RANK ? "<" + tempInfo.name + ">" : tempInfo.name;

		numLabel.text = tempInfo.gongJin.ToString ();
	}

	public void SetBg (bool light)
	{
		QXComData.SetBgSprite (m_bg,light);
	}
}
