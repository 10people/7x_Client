using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RoomItem : MonoBehaviour {

	private YabiaoRoomInfo roomInfo;

	public UISprite roomBg;

	public UILabel roomName;

	public UILabel yunBiaoNum;

	public GameObject yaBiaoTeamListObj;

	public void GetRoomItemInfo (YabiaoRoomInfo tempRoomInfo,int tempRoomId,int index)
	{
		roomInfo = tempRoomInfo;

		//YUN_BIAO_SCENE_:1097-1158
		string s = "YUN_BIAO_SCENE_" + (((index + 1) < 62)?(index + 1):(index % 62 + 1));
//		Debug.Log ("s:" + s);
		LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
//		Debug.Log ("t:" + t);
		roomName.text = LanguageTemplate.GetText (t) + (((index + 1) / 62) > 0?((index + 1) / 62).ToString ():"");

		yunBiaoNum.text = "当前运货马车" + tempRoomInfo.ybjzList.Count.ToString ();

		if (tempRoomInfo.roomId == tempRoomId)
		{
			SendRoomInfo ();

			JieBiaoMainPage.jieBiaoMain.InstantSelectBox (this.gameObject);
		}
	}

	void OnClick ()
	{
		JieBiaoMainPage.jieBiaoMain.InstantSelectBox (this.gameObject);

		SendRoomInfo ();

		JieBiaoMainPage.jieBiaoMain.GetRoomInfo (roomInfo);
	}

	void SendRoomInfo ()
	{
		yaBiaoTeamListObj.SetActive (true);
		YaBiaoTeamList yaBiaoTeam = yaBiaoTeamListObj.GetComponent<YaBiaoTeamList> ();
		yaBiaoTeam.GetYaBiaoTeamInfo (roomInfo);
	}
}
