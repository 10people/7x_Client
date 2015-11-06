using UnityEngine;
using System.Collections;

public class HorseItem : MonoBehaviour {

	public UILabel horseName;

	public UISprite horseIcon;

	public UISprite horseType;

	private int h_type;
	private int horseId;

	public GameObject tipsObj;

	public void GetHorseType (int type)
	{
		h_type = type;

		horseIcon.spriteName = "horse" + type;

		horseType.spriteName = "pinzhi" + (type - 1);

		horseId = type + 902000;

		string nameStr = "";

		switch (type)
		{
		case 1:
			nameStr = "骡马";
			break;

		case 2:
			nameStr = "驮马";
			break;

		case 3:
			nameStr = "膘马";
			break;

		case 4:
			nameStr = "骏马";
			break;

		case 5:
			nameStr = "神驹";
			break;
		}

		horseName.text = nameStr;

		this.GetComponent<NGUILongPress> ().OnLongPress += ActiveTips;
	}

	void ActiveTips (GameObject go)
	{
		ShowTip.showTip(horseId);
	}

//	void OnPress(bool isPressed)
//	{
//		if (isPressed)
//		{
//			tipsObj.transform.parent = this.transform;
//			tipsObj.transform.transform.localPosition = ShowPos ();
//			
//			YunBiaoTips ybTips = tipsObj.GetComponent<YunBiaoTips> ();
//			ybTips.SetTips (TipStr (),YunBiaoTips.ShowType.horse);
//			ybTips.SetAlpha = true;
//		}
//		else
//		{
//			YunBiaoTips ybTips = tipsObj.GetComponent<YunBiaoTips> ();
//			ybTips.SetAlpha = false;
//		}
//	}

	/// <summary>
	/// 显示内容
	/// </summary>
	private string TipStr ()
	{
		float shouYi = YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (h_type) - 
			YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (1);

		string str = "高等级的马匹可以提供更高收益的加成\n该品质马匹可获得([00ff00]" + shouYi +  "[-]铜币)收益的加成";

		return str;
	}

	/// <summary>
	/// 显示位置
	/// </summary>
	private Vector3 ShowPos ()
	{
		Vector3 pos = Vector3.zero;

		pos = new Vector3 (50 - (h_type - 1) * 102.5f,85f,0);

		return pos;
	}
}
