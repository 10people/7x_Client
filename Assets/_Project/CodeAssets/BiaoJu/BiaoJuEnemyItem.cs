using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BiaoJuEnemyItem : MonoBehaviour {

	private EnemiesInfo enemyInfo;

	public UISprite iconSprite;
	public UISprite border;
	public UISprite nation;
	public UILabel levelLabel;
	public UILabel nameLabel;
	public UILabel allianceLabel;

	public UIScrollBar hpBar;
	public UILabel hpLabel;

	public UIScrollBar jinDuBar;
	public UILabel jinDuLabel;

	public UILabel desLabel;
	public UILabel zhanLiLabel;

	public UILabel awardLabel;

	public GameObject processObj;

	public GameObject selectBoxObj;
	public UILabel addLabel;

	public void InItEnemyItem (EnemiesInfo tempInfo,long tempId)
	{
		enemyInfo = tempInfo;

		iconSprite.atlas = BiaoJuPage.bjPage.GetAtlas (tempInfo.state == 10 ? BiaoJuPage.AtlasType.YUNBIAO : BiaoJuPage.AtlasType.MAINCITYLAYER);
		iconSprite.spriteName = tempInfo.state == 10 ? "horseIcon" + tempInfo.horseType : "PlayerIcon" + tempInfo.roleId;

		border.spriteName = tempInfo.state == 10 ? "pinzhi" + (tempInfo.horseType - 1) : "";
		levelLabel.text = "Lv" + tempInfo.jzLevel.ToString ();

		nameLabel.text = tempInfo.junZhuName;
		nameLabel.transform.localPosition = new Vector3(-80,tempInfo.state == 10 ? 25 : 10,0);

		allianceLabel.text = tempInfo.lianMengName.Equals ("") ? "无联盟" : "<" + tempInfo.lianMengName + ">";
		allianceLabel.transform.localPosition = new Vector3(-80,tempInfo.state == 10 ? 0 : -15,0);

		zhanLiLabel.text = tempInfo.zhanLi.ToString ();
		zhanLiLabel.transform.parent.transform.localPosition = new Vector3(165,tempInfo.state == 10 ? 30 : 5,0);

		nation.spriteName = "nation_" + tempInfo.guojia;

		desLabel.text = tempInfo.state == 10 ? "" : "未在运镖";

		SetSelectBox(tempInfo.junZhuId == tempId ? true : false);

		processObj.SetActive (tempInfo.state == 10 ? true : false);

		if (tempInfo.state == 10)
		{
			addLabel.text = MyColorData.getColorString (6, "+" + tempInfo.hudun) + "%";

			int jinDu = (int)((tempInfo.usedTime / (float)tempInfo.totalTime) * 100);
			jinDuLabel.text = "进度" + jinDu.ToString () + "%";
			InItScrollBarValue (jinDuBar,jinDu);

			int hpNum = (int)((tempInfo.hp / (float)tempInfo.maxHp) * 100);
			hpLabel.text = tempInfo.hp.ToString () + "/" + tempInfo.maxHp.ToString ();
			InItScrollBarValue (hpBar,hpNum);

			awardLabel.text = BiaoJuPage.bjPage.GetHorseAwardNum (tempInfo.horseType).ToString ();
		}
	}

	/// <summary>
	/// Ins it scroll bar value.
	/// </summary>
	/// <param name="scrollBar">Scroll bar.</param>
	/// <param name="value">Value.</param>
	void InItScrollBarValue (UIScrollBar scrollBar,int value)
	{
		scrollBar.barSize = (float)value/100;
	}

	/// <summary>
	/// Gets the enemies info.
	/// </summary>
	/// <returns>The enemies info.</returns>
	public EnemiesInfo GetEnemiesInfo ()
	{
		return enemyInfo;
	}

	/// <summary>
	/// Sets the select box.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	public void SetSelectBox (bool isActive)
	{
		selectBoxObj.SetActive (isActive);
	}
}
