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

	public UILabel nameLabel;
	public UILabel numLabel;
	public UILabel desLabel;
	public UILabel shuXingLabel;

	public UISprite lockIcon;

	private int iconId;
	private int pinZhiId;
	private string nameStr;

	private GameObject iconSamplePrefab;

	public void GetFuWenInfo (Fuwen tempInfo)
	{
		fuWenInfo = tempInfo;
//		Debug.Log ("tempInfo:" + tempInfo);
//		Debug.Log ("isLock:" + tempInfo.isLock);
		lockIcon.gameObject.SetActive (tempInfo.isLock == 1 ? true : false);

		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (tempInfo.itemId);

		nameStr = NameIdTemplate.GetName_By_NameId (fuWenTemp.name);
		nameLabel.text = nameStr;
		desLabel.text = NameIdTemplate.GetName_By_NameId (fuWenTemp.shuXingName);

		numLabel.text = "x" + tempInfo.cnt;
		shuXingLabel.text = "+" + fuWenTemp.shuxingValue;

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
	}

	void OnClick ()
	{
		if (!FuWenMainPage.fuWenMainPage.IsBtnClick)
		{
			FuWenMainPage.fuWenMainPage.IsBtnClick = true;
			FuWenMainPage.fuWenMainPage.FxController (FuWenMixBtn.FxType.CLEAR);
			FuWenMainPage.fuWenMainPage.OperateFuWen (FuShiOperate.OperateType.HECHENG,fuWenInfo.itemId,0);
		}
	}
}
