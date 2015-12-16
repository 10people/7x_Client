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
	private int pinZhiId;

	//装备信息
	public UISprite cardBorder;
	public UILabel cardLabel;

	public EventHandler rewardHandler;

	public void InItSpecialReward (RewardData tempData)
	{
		rewardData = tempData;

		xmlType = (QXComData.XmlType)Enum.ToObject (typeof (QXComData.XmlType),CommonItemTemplate.GetCommonItemTemplateTypeById (tempData.itemId));

		switch (xmlType)
		{
		case QXComData.XmlType.MIBAO:

			InItMibaoCardInfo ();

			break;
		case QXComData.XmlType.EQUIP:

			InItEquipReward ();

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
			
			QXComData.InstanceEffect (QXComData.EffectPos.TOP,miBaoObj,100148);
			QXComData.InstanceEffect (QXComData.EffectPos.MID,miBaoObj,100157);

			break;
		case QXComData.XmlType.EQUIP:

			QXComData.InstanceEffect (QXComData.EffectPos.TOP,equipObj,100142 + QXComData.GetEffectColorByXmlColorId (pinZhiId));
			QXComData.InstanceEffect (QXComData.EffectPos.MID,equipObj,100150 + QXComData.GetEffectColorByXmlColorId (pinZhiId));
			
			break;
		default:
			break;
		}
		rewardHandler.m_handler += RewardHandlerClickBack;
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

		GeneralRewardManager.Instance ().SpecialReward_Index ++;
		GeneralRewardManager.Instance ().CheckSpecialReward ();
		GeneralRewardManager.Instance ().RefreshSpecialItemList (this.gameObject);
	}

	/// <summary>
	/// Ins it mibao card info.
	/// </summary>
	void InItMibaoCardInfo ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardData.itemId);
		int iconId = commonTemp.icon;
		int nameId = commonTemp.nameId;
		pinZhiId = commonTemp.color;

		mbTexTure.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON)
		        
		                                                 + iconId.ToString ());
		border.spriteName = "pinzhi" + (commonTemp.color - 1);
		Debug.Log ("rewardData.xingJi:" + rewardData.xingJi);
		for (int i = 0;i < rewardData.xingJi;i ++)
		{
			GameObject star = (GameObject)Instantiate (starObj);

			star.SetActive (true);
			star.transform.parent = starObj.transform.parent;
			star.transform.localPosition = new Vector3(i * 35 - (rewardData.xingJi - 1) * 17.5f,0,0);
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
		pinZhiId = commonTemp.color;

		cardLabel.text = NameIdTemplate.GetName_By_NameId (nameId);

		IconSampleManager iconSampleManager = iconSamplePrefab.GetComponent<IconSampleManager>();
		iconSampleManager.SetIconType(IconSampleManager.IconType.equipment);

		iconSampleManager.SetIconBasic(1,rewardData.itemId.ToString ());
		
		iconSampleManager.SetIconBasicDelegate (true,true,RewardHandlerClickBack);///////////////////////////////
		iconSampleManager.BgSprite.gameObject.SetActive (false);

		equipObj.SetActive (true);
		ScaleEffect (equipObj,0.3f);
	}
}
