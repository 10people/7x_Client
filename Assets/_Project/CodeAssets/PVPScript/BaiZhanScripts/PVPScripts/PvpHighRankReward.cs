using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpHighRankReward : MonoBehaviour {

	public static PvpHighRankReward highReward;

	public GameObject rewardBgObj;
	public GameObject rewardInfoObj;
	public UILabel desLabel3Obj;

	private GameObject iconSamplePrefab;
	private List<GameObject> iconSampleList = new List<GameObject> ();

	public EventHandler clickHandler;

	public enum AnimationState
	{
		BgScale1	= 0,
		Move1		= 1,
		ClickBox	= 2,
		Move2		= 3,
		BgScale2	= 4,
	}
	private AnimationState animationState = AnimationState.BgScale1;

	private float time1 = 0.3f;
	private float time2 = 0.5f;

	private List<RewardData> rewardDataList;

	void Awake ()
	{
		highReward = this;
	}

	void OnDestroy ()
	{
		highReward = null;
	}

	/// <summary>
	/// Ins it high rank reward.
	/// </summary>
	public void InItHighRankReward (List<RewardData> tempRewardList)
	{
		rewardDataList = tempRewardList;
		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}

		clickHandler.m_click_handler -= CloseHandler;

		animationState = AnimationState.BgScale1;
		setAnimation ();
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		iconSamplePrefab.SetActive(true);
		iconSamplePrefab.transform.parent = rewardInfoObj.transform;
		iconSamplePrefab.transform.localPosition = new Vector3(0,-25,0);
		
		InItIconSample ();
	}

	void InItIconSample ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardDataList[0].itemId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (rewardDataList[0].itemId);
		
		IconSampleManager fuShiIconSample = iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (rewardDataList[0].itemId,"x" + rewardDataList[0].itemCount,3);
		fuShiIconSample.SetIconPopText(rewardDataList[0].itemId, nameStr, mdesc, 1);
		iconSamplePrefab.transform.localScale = Vector3.one * 0.6f;
	}

	/// <summary>
	/// Sets the animation.
	/// </summary>
    void setAnimation ()
	{
		switch(animationState)
		{
		case AnimationState.BgScale1:

			iTween.ValueTo(gameObject, iTween.Hash("from", 0, 
			                                       "to", 1, 
			                                       "time", time1, 
			                                       "easetype",  "easeOutBack", 
			                                       "onupdate",  "UpValue", 
			                                       "oncomplete", "End"));

			break;
		case AnimationState.Move1:

			iTween.ValueTo(gameObject, iTween.Hash("from", -950, 
			                                       "to", 0, 
			                                       "time", time2, 
			                                       "easetype",  "easeOutBack", 
			                                       "onupdate",  "UpValue", 
			                                       "oncomplete", "End"));

			break;
		case AnimationState.ClickBox:


			clickHandler.m_click_handler += CloseHandler;

			break;
		case AnimationState.Move2:

			iTween.ValueTo(gameObject, iTween.Hash("from", 0, 
			                                       "to", 950, 
			                                       "time", time2, 
			                                       "easetype",  "easeInBack", 
			                                       "onupdate",  "UpValue", 
			                                       "oncomplete", "End"));

			break;
		case AnimationState.BgScale2:

			iTween.ValueTo(gameObject, iTween.Hash("from", 1, 
			                                       "to", 0, 
			                                       "time", time1, 
			                                       "easetype",  "easeInBack", 
			                                       "onupdate",  "UpValue", 
			                                       "oncomplete", "End"));

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Ups the value.
	/// </summary>
	/// <param name="value">Value.</param>
	void UpValue (float value)
	{
		switch(animationState)
		{
		case AnimationState.BgScale1:

			rewardBgObj.transform.localScale = new Vector3(1f, value, 1f);

			break;
		case AnimationState.Move1:

			rewardInfoObj.transform.localPosition = new Vector3(value, 0f, 0f);

			break;
		case AnimationState.ClickBox:
			break;
		case AnimationState.Move2:

			rewardInfoObj.transform.localPosition = new Vector3(value, 0f, 0f);

			break;
		case AnimationState.BgScale2:

			rewardBgObj.transform.localScale = new Vector3(1f, value, 1f);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// End this instance.
	/// </summary>
	void End ()
	{
		switch (animationState)
		{
		case AnimationState.BgScale1:

			animationState = AnimationState.Move1;
			setAnimation();

			break;
		case AnimationState.Move1:

			animationState = AnimationState.ClickBox;
			setAnimation ();

			break;
		case AnimationState.ClickBox:

			break;
		case AnimationState.Move2:

			animationState = AnimationState.BgScale2;
			setAnimation();

			break;
		case AnimationState.BgScale2:

			CloseReward ();
			
			break;
		default:
			break;
		}
	}

	void CloseHandler (GameObject obj)
	{
		clickHandler.m_click_handler -= CloseHandler;
		animationState = AnimationState.Move2;
		setAnimation ();
	}

	void CloseReward ()
	{
		BaiZhanPage.baiZhanPage.Yd_GetReward = false;
		BaiZhanPage.baiZhanPage.Yd_Store = false;
		GeneralRewardManager.Instance().CreateReward (rewardDataList);
		gameObject.SetActive (false);
	}
}
