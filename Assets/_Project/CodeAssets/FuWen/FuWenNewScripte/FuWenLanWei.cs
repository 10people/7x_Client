using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class FuWenLanWei : MonoBehaviour {

	// 特别备注  没穿带符文时 红点表示有可以穿戴的符文， 已经穿戴符文了则表示有可以升级的符文 由后台发


	[HideInInspector]public FuwenLanwei mFuwenLanwei;

	public UILabel Property;

	public GameObject Arelt;

	public UISprite FuWenIcon;

	public UILabel FuwenName;

	public UISprite ChangeTips;

	public GameObject LevelUp;

	public bool showChangeTips;

//	public GameObject EffectRoot;

	public void init()
	{
//		Debug.Log ("mFuwenLanwei.itemId = "+mFuwenLanwei.itemId);
//		Debug.Log ("showChangeTips = "+showChangeTips);
		if(mFuwenLanwei.itemId == 0)
		{
			Property.gameObject.SetActive(true);
			FuWenIcon.spriteName = "";
			//List<int> mFuwenOpenList = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiIdList(mpage);
			FuWenOpenTemplate mFuwenOpen = FuWenOpenTemplate.GetFuWenOpenTemplateBy_By_Id(mFuwenLanwei.lanweiId);
			Property.text = mFuwenOpen.background;

			FuwenName.text = "";
			Arelt.SetActive(mFuwenLanwei.flag);
			LevelUp.SetActive(false);

		}else if(mFuwenLanwei.itemId > 0)
		{
			Property.gameObject.SetActive(false);
			FuWenTemplate mFuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenLanwei.itemId);
			FuWenIcon.spriteName = mFuwenTemp.icon.ToString();
			string m_Name = "Lv"+mFuwenTemp.fuwenLevel.ToString()+ NameIdTemplate.GetName_By_NameId(mFuwenTemp.name).Substring(4,2);//+mFuwenTemp.fuwenLevel.ToString()
			FuwenName.text = m_Name;
			if(showChangeTips)
			{
				Arelt.SetActive(false);
			}
			else{
				Arelt.SetActive(mFuwenLanwei.flag);
			}
			LevelUp.SetActive (showChangeTips);
		}
		ChangeTips.gameObject.SetActive (showChangeTips);
		if(showChangeTips)
		{
			Closeffect();
			OPeneffect();
		}
		else
		{
			Closeffect();
		}
	}
	public void OPeneffect()
	{
		int effectid = 620232;
		UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,FuWenIcon.gameObject,EffectIdTemplate.GetPathByeffectId(effectid));
	}
	public void Closeffect()
	{
		UI3DEffectTool.ClearUIFx (FuWenIcon.gameObject);
	}
}
