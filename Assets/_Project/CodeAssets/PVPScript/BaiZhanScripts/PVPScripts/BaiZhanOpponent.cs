using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanOpponent : MonoBehaviour {

	private OpponentInfo opponentInfo;

	public UISprite headIcon;
	public UILabel levelLabel;
	public UISprite nation;
	public UILabel nameLabel;
	public UILabel rankLabel;
	public UILabel zhanLiLabel;
	public UISprite newRecordSprite;

	/// <summary>
	/// Ins it opponent.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItOpponent (OpponentInfo tempInfo)
	{
		opponentInfo = tempInfo;

		headIcon.spriteName = "PlayerIcon" + opponentInfo.roleId;

		nation.spriteName = "nation_" + opponentInfo.guojia;
		nameLabel.text = MyColorData.getColorString (1,opponentInfo.junZhuId < 0 ? NameIdTemplate.GetName_By_NameId (int.Parse(opponentInfo.junZhuName)) : opponentInfo.junZhuName);
		
		zhanLiLabel.text = "战力：" + MyColorData.getColorString (opponentInfo.zhanLi > BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.zhanLi ? 5 : 4,opponentInfo.zhanLi.ToString ());
		
		levelLabel.text = opponentInfo.level.ToString ();

		rankLabel.text = "排行：" + opponentInfo.rank.ToString ();

		newRecordSprite.gameObject.SetActive (tempInfo.rank < BaiZhanPage.baiZhanPage.baiZhanResp.pvpInfo.historyHighRank ? true : false);
	}
}
