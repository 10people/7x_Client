using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailReward : MonoBehaviour {

	private EmailGoods goodInfo;

	private GameObject iconSamplePrefab;

	public UILabel numLabel;

	public void GetRewardInfo (EmailGoods tempInfo)
	{
		goodInfo = tempInfo;

		numLabel.text = MyColorData.getColorString (1,"x" + tempInfo.count.ToString ());

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
		iconSamplePrefab.transform.localPosition = Vector3.zero;
		
		InItIconSample ();
	}
	
	void InItIconSample ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (goodInfo.id);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (goodInfo.id);

		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (goodInfo.id);
		fuShiIconSample.SetIconPopText(goodInfo.id, nameStr, mdesc, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.5f;
	}
}
