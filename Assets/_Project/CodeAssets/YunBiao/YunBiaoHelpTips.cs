using UnityEngine;
using System.Collections;

public class YunBiaoHelpTips : MonoBehaviour {

	public UISprite headIcon;

	public UILabel nameLabel;

	public UILabel addNumLabel;

	public UILabel desLabel1;

	public UILabel desLabel2;

	public GameObject infoObj;

	void Start ()
	{
		desLabel1.text = DescIdTemplate.GetDescriptionById (2501);
		desLabel2.text = "协助规则：" + DescIdTemplate.GetDescriptionById (2502);
	}

	public void ShowHelpInfo (string name,int iconId,int addNum)
	{
		nameLabel.text = name;

		headIcon.spriteName = "PlayerIcon" + iconId;

		addNumLabel.text = MyColorData.getColorString (6,"+" + addNum) + "%";
	}
}
