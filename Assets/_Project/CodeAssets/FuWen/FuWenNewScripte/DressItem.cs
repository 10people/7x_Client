using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class DressItem : MonoBehaviour {

	public FuwenInBag mFuwenInBag;

	public UILabel FuWenName;

	public UILabel FuwenNumber;

	public UILabel Exp;

	public UILabel Property;

	public UILabel PropertyNumber;

	public UISprite FuWenICon;

	public UISprite FuWenPinZhi;

	public UIEventListener mListener;

	public GameObject ButtonName;

	public bool XiangQian;

	public UILabel mButtonName;
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		if(XiangQian)
		{
			mButtonName.text = "穿戴";
		}
		else
		{
			mButtonName.text = "替换";
		}
		FuWenTemplate mTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (mFuwenInBag.itemId);
		FuWenPinZhi.spriteName = "pinzhi"+(mTemp.color -1).ToString();
		FuWenName.text = mTemp.fuwenLevel.ToString()+"级"+NameIdTemplate.GetName_By_NameId (mTemp.name);

		FuwenNumber.text = "x"+mFuwenInBag.cnt.ToString ();

		Exp.text = mFuwenInBag.exp + "/" + mTemp.lvlupExp.ToString ();

		FuWenICon.spriteName = mTemp.icon.ToString ();
		//DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId (mTemp.desc);
		//Debug.Log ("mFuwenInBag.itemId = "+mFuwenInBag.itemId);
		Property.text = GetFuWenProperty(mTemp.shuxing) ;

		PropertyNumber.text = "+" + mTemp.shuxingValue.ToString ();
	}

	string GetFuWenProperty(int index)
	{
		string mstr = "";
		switch(index)
		{
		case 1:
			mstr = "攻击";
			break;
		case 2:
			mstr = "防御";
			break;
		case 3:
			mstr = "生命";
			break;
		case 4:
			mstr = "武器伤害加深";
			break;
		case 5:
			mstr = "武器伤害抵抗";
			break;
		case 6:
			mstr = "武器暴击加深";
			break;
		case 7:
			mstr = "武器暴击抵抗";
			break;
		case 8:
			mstr = "技能伤害加深";
			break;
		case 9:
			mstr = "技能伤害抵抗";
			break;
		case 10:
			mstr = "技能暴击加深";
			break;
		case 11:
			mstr = "技能暴击抵抗";
			break;
		default:
			break;
		}
		return mstr;
	}
}
