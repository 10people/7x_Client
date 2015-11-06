using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceCurrentItem : MonoBehaviour {

	public UILabel fixtures;

	public UILabel id_1;

	public UILabel allianceName_1;

	public UILabel allianceName_2;

	public UILabel id_2;

	//获得本届战况对战条目信息
	public void GetCurrentInfo (FightHistoryInfo tempInfo)
	{
		fixtures.text = ShowFixtures (tempInfo.times);

		if (tempInfo.lm1Id == tempInfo.winLmId)
		{
			id_1.text = tempInfo.lm1Id.ToString ();
			allianceName_1.text = tempInfo.lm1Name;
			
			id_2.text = tempInfo.lm2Id.ToString ();
			allianceName_2.text = tempInfo.lm2Name;
		}
		else
		{
			id_1.text = tempInfo.lm2Id.ToString ();
			allianceName_1.text = tempInfo.lm2Name;
			
			id_2.text = tempInfo.lm1Id.ToString ();
			allianceName_2.text = tempInfo.lm1Name;
		}
	}

	// 赛程，1-32强，2-16强，3-8强，4-4强，5-半决赛，6-三四名比赛，7-决赛
	private string ShowFixtures (int fixturesId)
	{
		string fixturesText = "";
		switch (fixturesId)
		{
		case 1:
			fixturesText = "32强赛";
			break;
		case 2:
			fixturesText = "16强赛";
			break;
		case 3:
			fixturesText = "8强赛";
			break;
		case 4:
			fixturesText = "4强赛";
			break;
		case 5:
			fixturesText = "半决赛";
			break;
		case 6:
			fixturesText = "三四名决赛";
			break;
		case 7:
			fixturesText = "决赛";
			break;
		default:
			break;
		}

		return fixturesText;
	}
}
