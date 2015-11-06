using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceFightItem : MonoBehaviour {

	public UILabel id1;

	public UILabel alliance1;

	public UILabel id2;

	public UILabel alliance2;

	public UISprite bgSprite;

	public void GetAlianceFightItemInfo (FightMatchInfo tempInfo,int m_allianceId)
	{
		bgSprite.spriteName = (m_allianceId == tempInfo.lm1Id || m_allianceId == tempInfo.lm2Id) ? "bg2" : "backGround_Common_big";
		Debug.Log ("lm1Name:" + tempInfo.lm1Name + "||" + "lm2Name:" + tempInfo.lm2Name);
		id1.text = tempInfo.lm1Id > 0 ? tempInfo.lm1Id.ToString () : "";
		alliance1.text = tempInfo.lm1Id > 0 ? tempInfo.lm1Name : "轮空";

		id2.text = tempInfo.lm2Id > 0 ? tempInfo.lm2Id.ToString () : "";
		alliance2.text = tempInfo.lm2Id > 0 ? tempInfo.lm2Name : "轮空";
	}
}
