using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenSelectItem : MonoBehaviour {

	private Fuwen fuWenInfo;

	private int iconId;
	private int pinZhiId;
	private string nameStr;

	public UILabel nameLabel;
	public UILabel numLabel;
	public UILabel desLabel;
	public UILabel shuXingLabel;

	private GameObject iconSamplePrefab;

	public UISprite select;

	public GameObject fuWenSelectWindow;

	//获得符文信息
	public void GetFuWenInfo (Fuwen tempInfo)
	{
		fuWenInfo = tempInfo;

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

		this.gameObject.GetComponent<UIDragScrollView> ().enabled = QXComData.CheckYinDaoOpenState (100470) ? false : true;
	}

	//当前符石是否选择
	public void IsCurFuShi (int itemId)
	{
		select.gameObject.SetActive (fuWenInfo.itemId == itemId ? true : false);
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);

		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = this.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (-175,0,0);

		InItIconSample ();
	}

	void InItIconSample ()
	{
		string mdesc = DescIdTemplate.GetDescriptionById (fuWenInfo.itemId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
		fuShiIconSample.SetIconBasic(3,iconId.ToString (),"","pinzhi" + pinZhiId);
		fuShiIconSample.SetIconPopText(fuWenInfo.itemId, nameStr, mdesc, 1);
		
		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
	}

	void OnClick ()
	{
		if (QXComData.CheckYinDaoOpenState (100470))
		{
			UIYindao.m_UIYindao.CloseUI ();
		}

		FuWenSelect fuWenSelect = fuWenSelectWindow.GetComponent<FuWenSelect> ();
		if (!fuWenSelect.IsSelect)
		{
			fuWenSelect.IsSelect = true;
			fuWenSelect.RefreshSelectFuShiItem (fuWenInfo.itemId);
			fuWenSelect.GetFuWenInfo = fuWenInfo;
			fuWenSelect.CloseBtn (gameObject);
		}
	}
}
