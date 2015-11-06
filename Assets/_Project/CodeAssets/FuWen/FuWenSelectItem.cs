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
		nameLabel.text = NameIdTemplate.GetName_By_NameId (fuWenTemp.name);
		desLabel.text = NameIdTemplate.GetName_By_NameId (fuWenTemp.shuXingName);
		
		numLabel.text = "x" + tempInfo.cnt;
		shuXingLabel.text = "+" + fuWenTemp.shuxingValue;
		
		iconId = fuWenTemp.icon;
		
		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
			fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
			fuShiIconSample.SetIconBasic(3,iconId.ToString ());
			
			iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
		}

		if(FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
		{
			this.gameObject.GetComponent<UIDragScrollView> ().enabled = false;
		}
		else
		{
			this.gameObject.GetComponent<UIDragScrollView> ().enabled = true;
		}
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

		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
		fuShiIconSample.SetIconBasic(3,iconId.ToString ());

		iconSamplePrefab.transform.localScale = Vector3.one * 0.8f;
	}

	void OnClick ()
	{
		if(FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
		{
			UIYindao.m_UIYindao.CloseUI ();
		}
		FuWenSelect fuWenSelect = fuWenSelectWindow.GetComponent<FuWenSelect> ();
		if (!fuWenSelect.IsSelect)
		{
			fuWenSelect.IsSelect = true;
			fuWenSelect.RefreshSelectFuShiItem (fuWenInfo.itemId);
			fuWenSelect.GetFuWenInfo = fuWenInfo;
			fuWenSelect.CloseBtn ();
		}
	}
}
