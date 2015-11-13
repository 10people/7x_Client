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

		if (opponentInfo.junZhuId < 0)
		{
			int nameId = int.Parse(opponentInfo.junZhuName);
			
			string name = NameIdTemplate.GetName_By_NameId (nameId);
			
			nameLabel.text = name;
		}
		else
		{
			nameLabel.text = opponentInfo.junZhuName;
		}
		
		zhanLiLabel.text = opponentInfo.zhanLi.ToString ();
		
		levelLabel.text = opponentInfo.level.ToString ();
		
		if (opponentInfo.rank < 4)
		{
			rankIcon.gameObject.SetActive (true);
			
			rankIcon.spriteName = "rank" + opponentInfo.rank;
		}
		rankLabel.text = opponentInfo.rank.ToString ();
		//		Debug.Log ("RoleId:" + opponentInfo.roleId);
		headIcon.spriteName = "PlayerIcon" + opponentInfo.roleId;
		
		country.spriteName = "nation_" + opponentInfo.guojia;
	}
}
