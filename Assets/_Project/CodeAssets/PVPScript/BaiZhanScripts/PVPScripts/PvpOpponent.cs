using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpOpponent : MonoBehaviour {

	private OpponentInfo opponentInfo;
	public OpponentInfo OpponentInfo
	{
		get{return opponentInfo;}
	}

	public UISprite headIcon;//头像
	
	public UILabel nameLabel;//名字
	
	public UILabel zhanLiLabel;//战力
	
	public UILabel levelLabel;//等级
	
	public UISprite rankIcon;//排名icon
	public UILabel rankLabel;//排名
	
	public UISprite country;//国家

	public void GetOpponentInfo (OpponentInfo tempInfo)
	{
		opponentInfo = tempInfo;

		nameLabel.text = opponentInfo.junZhuId < 0 ? NameIdTemplate.GetName_By_NameId (int.Parse(opponentInfo.junZhuName)) : opponentInfo.junZhuName;
		
		zhanLiLabel.text = "战力" + opponentInfo.zhanLi.ToString ();
		
		levelLabel.text = opponentInfo.level.ToString ();

		rankIcon.spriteName = opponentInfo.rank < 4 ? "rank" + opponentInfo.rank : ""; 
		rankLabel.text = opponentInfo.rank < 4 ? "" : opponentInfo.rank.ToString ();

		//		Debug.Log ("RoleId:" + opponentInfo.roleId);
		headIcon.spriteName = "PlayerIcon" + opponentInfo.roleId;
		
		country.spriteName = "nation_" + opponentInfo.guojia;
	}
}
