using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YaBiaoTeamList : MonoBehaviour {

	public GameObject ybJunZhuItemObj;

	public GameObject teamGrid;

	private List<GameObject> teamItemList = new List<GameObject> ();

	public UILabel jieBiaoTime;//剩余劫镖次数

	/// <summary>
	/// 获得押镖房间的君主信息
	/// </summary>
	public void GetYaBiaoTeamInfo (YabiaoRoomInfo tempRoomInfo)
	{
		foreach (GameObject obj in teamItemList)
		{
			Destroy (obj);
		}
		teamItemList.Clear ();

		YunBiaoMainPage.yunBiaoMainData.MoveBackToTop (teamGrid,tempRoomInfo.ybjzList.Count,3);

		for (int i = 0;i < tempRoomInfo.ybjzList.Count;i ++)
		{
			GameObject ybJunZhuItem = (GameObject)Instantiate (ybJunZhuItemObj);

			ybJunZhuItem.SetActive (true);

			ybJunZhuItem.transform.parent = teamGrid.transform;

			ybJunZhuItem.transform.localPosition = new Vector3(0,-i * 125,0);

			ybJunZhuItem.transform.localScale = Vector3.one;

			teamItemList.Add (ybJunZhuItem);
		}

		for (int i = 0;i < tempRoomInfo.ybjzList.Count;i ++)
		{
			teamItemList[i].GetComponent<TeamItem> ().GetTeamItemInfo (tempRoomInfo.ybjzList[i]);
		}
//		Debug.Log ("次数：" + YunBiaoMainPage.yunBiaoMainData.yunBiaoMainInfoRes.jieBiaoCiShu);
		jieBiaoTime.text = "剩余[00ff00]" + YunBiaoData.Instance.yunBiaoRes.jieBiaoCiShu.ToString () + "[-]次";
	}
}
