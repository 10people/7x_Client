using UnityEngine;
using System.Collections;

public class BiaoJuHorseItem : MonoBehaviour {

	private BiaoJuHorseInfo horseInfo;

	public UISprite horseIcon;
	public UISprite border;
	public UILabel nameLabel;

	public NGUILongPress horsePress;

	public void InItHorseItem (BiaoJuHorseInfo tempInfo)
	{
		horseInfo = tempInfo;

		horseIcon.spriteName = BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,1);
		border.spriteName = BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,3);

		nameLabel.text = BiaoJuPage.bjPage.HorseStringInfo (tempInfo.horseId,0);

		horsePress.OnLongPress -= ActiveTips;
		horsePress.OnLongPress += ActiveTips;
	}

	void ActiveTips (GameObject go)
	{
		ShowTip.showTip (horseInfo.horseItemId);
	}
}
