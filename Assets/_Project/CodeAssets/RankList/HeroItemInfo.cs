using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HeroItemInfo : MonoBehaviour {

	private JunZhuInfo junzhuInfo;

	public UISprite rankIcon;

	public UISprite headIcon;

	public UISprite countryIcon;

	public UILabel rankLabel;

	public UILabel levelLabel;

	public UILabel nameLabel;

	public GameObject heroTipsObj;

	public NGUILongPress NguiLongPress;

	void Start ()
	{

	}

	//获得君主信息
	public void GetHeroInfo (JunZhuInfo tempInfo)
	{
		junzhuInfo = tempInfo;

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

		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;
//		Debug.Log ("Rank:" + tempInfo.rank + "||" + "nation_:" + tempInfo.guojiaId);
		countryIcon.spriteName = "nation_" + tempInfo.guojiaId;

		levelLabel.text = tempInfo.level.ToString ();

		nameLabel.text = tempInfo.name;

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
		HeroTips tips = heroTipsObj.GetComponent<HeroTips> ();
		tips.GetHeroInfo (junzhuInfo);
		tips.isAlpha = true;
	}

	void DisActiveDesObj (GameObject tempObj)
	{
		HeroTips tips = heroTipsObj.GetComponent<HeroTips> ();
		tips.isAlpha = false;
	}
}
