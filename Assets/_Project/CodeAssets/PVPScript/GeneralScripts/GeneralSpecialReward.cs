using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeneralSpecialReward : MonoBehaviour {

	private RewardData rewardData;

	private QXComData.XmlType xmlType;

	public GameObject miBaoObj;
	public GameObject equipObj;

	//秘宝信息
	public UITexture mbTexTure;//秘宝图标
	public UISprite border;
	public UILabel miBaoName;
	public GameObject starObj;
	private int effectId;

	//装备信息
	public UILabel cardLabel;

	public UILabel desLabel;
	private float time = 1f;
	private string desStr = "点击任意位置继续";

	public EventHandler rewardHandler;

	public void InItSpecialReward (RewardData tempData)
	{
		rewardData = tempData;
		desLabel.text = "";
		xmlType = (QXComData.XmlType)Enum.ToObject (typeof (QXComData.XmlType),CommonItemTemplate.GetCommonItemTemplateTypeById (tempData.itemId));
//		Debug.Log ("xmlType:" + xmlType);
		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			InItMibaoCardInfo ();
			desLabel.transform.localPosition = new Vector3(0,-270,0);

			break;
		case QXComData.XmlType.EQUIP:

			InItEquipReward ();
			desLabel.transform.localPosition = new Vector3(0,-175,0);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Scales the effect.
	/// </summary>
	/// <param name="obj">Object.</param>
	void ScaleEffect (GameObject obj,float time)
	{
		Hashtable scale = new Hashtable ();
		scale.Add ("time",time);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("scale",Vector3.one);
		scale.Add ("islocal",true);
		scale.Add ("oncomplete","ScaleEffectEnd");
		scale.Add ("oncompletetarget",this.gameObject);
		iTween.ScaleTo (obj,scale);
	}
	void ScaleEffectEnd ()
	{
		//特效
		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:
			
//			QXComData.InstanceEffect (QXComData.EffectPos.TOP,miBaoObj,100148);
			QXComData.InstanceEffect (QXComData.EffectPos.MID,miBaoObj,100157);

			break;
		case QXComData.XmlType.EQUIP:

			QXComData.InstanceEffect (QXComData.EffectPos.TOP,equipObj,100142 + effectId);
			QXComData.InstanceEffect (QXComData.EffectPos.MID,equipObj,100150 + effectId);

			break;
		default:
			break;
		}

		rewardHandler.m_handler += RewardHandlerClickBack;

		StartCoroutine ("ShowDesLabel1");
	}

	void RewardHandlerClickBack (GameObject obj)
	{
		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			UI3DEffectTool.Instance ().ClearUIFx (miBaoObj);

			break;
		case QXComData.XmlType.EQUIP:

			UI3DEffectTool.Instance ().ClearUIFx (equipObj);

			break;
		default:
			break;
		}

		if (rewardData.miBaoClick != null)
		{
			rewardData.miBaoClick ();
		}

		GeneralRewardManager.Instance ().SpecialReward_Index ++;
		GeneralRewardManager.Instance ().CheckSpecialReward ();
		GeneralRewardManager.Instance ().RefreshSpecialItemList (this.gameObject);
	}

	IEnumerator ShowDesLabel1 ()
	{
		desLabel.text = MyColorData.getColorString (4,desStr);
		UIWidget widget = desLabel.GetComponent<UIWidget> ();
		while (widget.alpha > 0)
		{
			yield return new WaitForSeconds (time);
			time = 0.07f;
			widget.alpha -= 0.1f;
			if (widget.alpha <= 0)
			{
				StopCoroutine ("ShowDesLabel1");
				StartCoroutine ("ShowDesLabel2");
			}
		}
	}

	IEnumerator ShowDesLabel2 ()
	{
		desLabel.text = MyColorData.getColorString (4,desStr);
		UIWidget widget = desLabel.GetComponent<UIWidget> ();
		while (widget.alpha < 1)
		{
			widget.alpha += 0.1f;
			yield return new WaitForSeconds (time);
			if (widget.alpha >= 1)
			{
				StopCoroutine ("ShowDesLabel2");
				StartCoroutine ("ShowDesLabel1");
			}
		}
	}

	/// <summary>
	/// Ins it mibao card info.
	/// </summary>
	void InItMibaoCardInfo ()
	{
		MiBaoXmlTemp miBaoTemp = MiBaoXmlTemp.getMiBaoXmlTempById (rewardData.itemId);
		int iconId = miBaoTemp.icon;
		int nameId = miBaoTemp.nameId;

		int defaultStar = rewardData.miBaoStar > 0 ? rewardData.miBaoStar : miBaoTemp.initialStar;

		mbTexTure.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON)
		        
		                                                 + iconId.ToString ());
		int colorId = CommonItemTemplate.getCommonItemTemplateById (rewardData.itemId).color;

		effectId = QXComData.GetEffectColorByXmlColorId (colorId);

		border.spriteName = "pinzhi" + (colorId - 1);

		border.width = QXComData.IsDifferent (colorId) ? 350 : 340;
		border.height = QXComData.IsDifferent (colorId) ? 470 : 460;

//		Debug.Log ("rewardData.xingJi:" + rewardData.xingJi);
		for (int i = 0;i < defaultStar;i ++)
		{
			GameObject star = (GameObject)Instantiate (starObj);

			star.SetActive (true);
			star.transform.parent = starObj.transform.parent;
			star.transform.localPosition = new Vector3(i * 35 - (defaultStar - 1) * 17.5f,0,0);
			star.transform.localScale = Vector3.one;
		}
		
		miBaoName.text = NameIdTemplate.GetName_By_NameId (nameId);
		
		miBaoObj.SetActive (true);
		ScaleEffect (miBaoObj,0.3f);
	}

	/// <summary>
	/// Ins it equip reward.
	/// </summary>
	void InItEquipReward ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
		                        IconSampleLoadCallBack);


	}
	void IconSampleLoadCallBack (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		GameObject iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive (true);
		iconSamplePrefab.transform.parent = equipObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3 (0, 20, 0);

		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardData.itemId);
		int nameId = commonTemp.nameId;
		effectId = QXComData.GetEffectColorByXmlColorId (commonTemp.color);

		cardLabel.text = NameIdTemplate.GetName_By_NameId (nameId);

		IconSampleManager iconSampleManager = iconSamplePrefab.GetComponent<IconSampleManager>();

		iconSampleManager.SetIconByID (rewardData.itemId);
		iconSampleManager.SetIconBasicDelegate (true,true,RewardHandlerClickBack);///////////////////////////////
		iconSampleManager.BgSprite.gameObject.SetActive (false);

		equipObj.SetActive (true);
		ScaleEffect (equipObj,0.3f);
	}
}
