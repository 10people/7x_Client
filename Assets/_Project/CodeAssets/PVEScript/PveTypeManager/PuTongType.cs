using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PuTongType : MonoBehaviour {

	public UILabel desLabel;
	public UILabel Person_name;//描述label
	public GameObject enemyListObj;
	public GameObject awardListObj;

	public void GetNeedInfo (Level tempLevelInfo,GuanQiaInfo m_GuanQia)
	{
		int guanqiaId = tempLevelInfo.guanQiaId;
		
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (guanqiaId);
		
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		char[] separator = new char[] { '#' };
		string[] s = descStr.Split (separator);
		
		desLabel.text = s[0];
		
		Person_name.text = s [1];

		UICreateEnemy enemyCreate = enemyListObj.GetComponent<UICreateEnemy> ();
		enemyCreate.InItEnemyList (tempLevelInfo.type);

		UICreateDropthings awardCreate = awardListObj.GetComponent<UICreateDropthings> ();

		awardCreate.mLevl = tempLevelInfo;

		awardCreate.GetAward (tempLevelInfo.type);
	}
}
