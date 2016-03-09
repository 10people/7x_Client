using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenBagItem : MonoBehaviour {

	private Fuwen fuWenInfo;

	public UISprite itemBg;
	public UILabel nameLabel;
	public UILabel numLabel;
	public UILabel desLabel;
	public UILabel shuXingLabel;

	private int iconId;
	private int pinZhiId;
	private string nameStr;

	private bool isCanMix = false;

	private GameObject iconSamplePrefab;

	public void GetFuWenInfo (Fuwen tempInfo)
	{
		fuWenInfo = tempInfo;
//		Debug.Log ("tempInfo:" + tempInfo);

		isCanMix = tempInfo.cnt >= 4 ? true : false;

//		itemBg.color = tempInfo.cnt >= 4 ? Color.white : new Color (0.7f,0.7f,0.7f,1);

		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (tempInfo.itemId);

		nameStr = NameIdTemplate.GetName_By_NameId (fuWenTemp.name);
		nameLabel.text = MyColorData.getColorString (3,nameStr);
		desLabel.text = FuWenData.Instance.colorCode + NameIdTemplate.GetName_By_NameId (fuWenTemp.shuXingName) + "[-]";

		numLabel.text = MyColorData.getColorString (3,"x" + tempInfo.cnt);
		shuXingLabel.text = FuWenData.Instance.colorCode + "+" + fuWenTemp.shuxingValue + "[-]";

		iconId = fuWenTemp.icon;
		pinZhiId = CommonItemTemplate.getCommonItemTemplateById (tempInfo.itemId).color - 1;

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);

		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = this.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (-145,0,0);

		InItIconSample ();
	}

	void InItIconSample ()
	{
		string mdesc = DescIdTemplate.GetDescriptionById (fuWenInfo.itemId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
		fuShiIconSample.SetIconBasic(5,iconId.ToString (),"","pinzhi" + pinZhiId);
		fuShiIconSample.SetIconPopText(fuWenInfo.itemId, nameStr, mdesc, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;

		UIWidget[] sprites = this.GetComponentsInChildren<UIWidget> ();
		foreach (UIWidget sp in sprites)
		{
			sp.color = isCanMix ? Color.white : new Color (0.6f,0.6f,0.6f,1);
		}
	}

	void OnClick ()
	{
		if (isCanMix)
		{
			if (!FuWenMainPage.fuWenMainPage.IsBtnClick)
			{
				FuWenMainPage.fuWenMainPage.IsBtnClick = true;
				
				FuWenMainPage.fuWenMainPage.CurHeChengItemId = fuWenInfo.itemId;
				FuWenMainPage.fuWenMainPage.ShowMixBtns ();
				FuWenMainPage.fuWenMainPage.FxController (FuWenMixBtn.FxType.OPEN);
				FuWenMainPage.fuWenMainPage.EffectPanel (true);

//				FuWenMainPage.fuWenMainPage.FxController (FuWenMixBtn.FxType.CLEAR);
//				FuWenMainPage.fuWenMainPage.OperateFuWen (FuShiOperate.OperateType.HECHENG,fuWenInfo.itemId,0);
			}
		}
	}
}
