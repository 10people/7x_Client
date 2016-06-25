using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CWScoreItem : MonoBehaviour {

	public ScoreInfo m_scoreInfo;

	public List<UILabel> m_labelList = new List<UILabel>();

	public void InItScore (ScoreInfo tempInfo)
	{
		Debug.Log ("tempInfo.isEnemy:" + tempInfo.isEnemy);
		m_scoreInfo = tempInfo;
		m_labelList[0].text = tempInfo.rank.ToString ();
//		m_labelList[1].text = MyColorData.getColorString (tempInfo.isEnemy  == 1 ? (tempInfo.jzId == QXComData.JunZhuInfo ().id ? 4 : 12) : 5,tempInfo.jzName) + "\n" + MyColorData.getColorString (tempInfo.isEnemy == 1 ? 12 : 5,"<" + tempInfo.lmName + ">");
		m_labelList[1].text = (tempInfo.jzId == QXComData.JunZhuInfo ().id ? "[06de34]" : "[ffffff]") + tempInfo.jzName + "[-]\n<" + tempInfo.lmName + ">";
		m_labelList[2].text = tempInfo.score.ToString ();
		m_labelList[3].text = tempInfo.KillNum.ToString ();
		m_labelList[4].text = tempInfo.lianShaNum.ToString ();
		m_labelList[5].text = tempInfo.gongXun.ToString ();
	}
}
