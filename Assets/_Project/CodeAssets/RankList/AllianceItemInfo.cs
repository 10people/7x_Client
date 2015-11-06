using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceItemInfo : MonoBehaviour {

	private LianMengInfo allianceInfo;
	
	public UISprite rankIcon;
	
	public UISprite allianceIcon;

	public UISprite countryIcon;
	
	public UILabel rankLabel;
	
	public UILabel levelLabel;
	
	public UILabel nameLabel;

	public GameObject allianceTipsObj;

	public NGUILongPress NguiLongPress;
	
	//获得联盟信息
	public void GetAllianceInfo (LianMengInfo tempInfo)
	{
		allianceInfo = tempInfo;
		
		if (tempInfo.rank < 4)
		{
			rankIcon.gameObject.SetActive (true);
			
			rankIcon.spriteName = "rank" + tempInfo.rank;
		}

		else if (tempInfo.rank < 10000)
		{
			rankIcon.gameObject.SetActive (false);
			
			rankLabel.text = tempInfo.rank.ToString ();
		}

		else
		{
			rankIcon.gameObject.SetActive (false);
			
			rankLabel.text = ">" + tempInfo.rank.ToString ();
		}

		allianceIcon.spriteName = "rank" + tempInfo.icon.ToString ();

		countryIcon.spriteName = "nation_" + tempInfo.guoJiaId;

		levelLabel.text = tempInfo.level.ToString ();
		
		nameLabel.text = tempInfo.mengName;

		NguiLongPress.OnLongPress += SetActiveDesObj;
		NguiLongPress.OnLongPressFinish += DisActiveDesObj;
	}

	void SetActiveDesObj (GameObject tempObj)
	{
		StartCoroutine (WaitForShow ());
	}
	
	IEnumerator WaitForShow ()
	{
		yield return new WaitForEndOfFrame ();
		AllianceTips tips = allianceTipsObj.GetComponent<AllianceTips> ();
		tips.GetAllianceInfo (allianceInfo);
		tips.isAlpha = true;

	}
	
	void DisActiveDesObj (GameObject tempObj)
	{
		AllianceTips tips = allianceTipsObj.GetComponent<AllianceTips> ();
		tips.isAlpha = false;
	}
}
