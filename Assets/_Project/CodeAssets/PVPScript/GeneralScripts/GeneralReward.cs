using UnityEngine;
using System;
using System.Collections;

public class GeneralReward : MonoBehaviour {

	public GameObject transObj;

	public UILabel numLabel;

	private GameObject iconSamplePrefab;

	private int itemId;
	private int itemCount;

	private float moveFirstTime;
	private float stopTime;
	private float moveSecondTime;
	private RewardData rewardData;

	private bool m_next = true;

	private float m_scale = 0.005f;

	/// <summary>
	/// Ins it reward item.
	/// </summary>
	/// <param name="tempData">Temp data.</param>
	public void InItRewardItem (RewardData tempData)
	{
		rewardData = tempData;

		if (tempData.moveTime1 == 0 || tempData.moveTime2 == 0)
		{
//			Debug.Log ("Do not moveTime set to 0");
			return;
		}

		moveFirstTime = tempData.moveTime1;
		stopTime = tempData.stopTime;
		moveSecondTime = tempData.moveTime2;

		itemId = tempData.itemId;
		numLabel.text = "+" + tempData.itemCount.ToString ();

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack(ref tempWww, null, iconSamplePrefab);
		}

		MoveFirst ();
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		if (iconSamplePrefab == null)
		{
			iconSamplePrefab = p_object as GameObject;
		}
		
		GameObject iconSample = (GameObject)Instantiate (iconSamplePrefab);
		
		iconSample.SetActive(true);
		iconSample.transform.parent = transObj.transform;
		iconSample.transform.localPosition = new Vector3 (10,0,0);

		IconSampleManager fuShiIconSample = iconSample.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (rewardData.itemId);
		iconSample.transform.localScale = Vector3.one * 0.4f;
	}

	void MoveFirst ()
	{
		Hashtable move = new Hashtable ();
		move.Add ("time",moveFirstTime * 4);
		move.Add ("position",new Vector3 (0,100,0));
		move.Add ("islocal",true);
		move.Add ("oncomplete","DestroyObj");
		move.Add ("oncompletetarget",this.gameObject);
		move.Add ("easetype",rewardData.itweenType1);
		iTween.MoveTo (transObj,move);
	}
	
	void DestroyObj ()
	{
		GeneralRewardManager.Instance().RefreshItemList (this.gameObject);
	}

	void Update ()
	{
		float rewardAlpha = transObj.GetComponent<UIWidget> ().alpha;
		if (transObj.transform.localPosition.y <= 0)
		{
//			rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y) / 100);//225
			rewardAlpha = 1;
		}
		else
		{
			rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y) / 225);
		}
		transObj.GetComponent<UIWidget> ().alpha = rewardAlpha;

		if (m_next && transObj.transform.localPosition.y > -80)
		{
			m_next = false;
			GeneralRewardManager.Instance().Reward_Index ++;
			GeneralRewardManager.Instance().CheckReward ();
		}

		if (transObj.transform.localPosition.y >= -100)
		{
			if (transObj.transform.localScale.x > 0.6f)
			{
				transObj.transform.localScale -= Vector3.one * m_scale;
				m_scale -= 0.0001f;
				if (transObj.transform.localScale.x <= 0.6f)
				{
					transObj.transform.localScale = Vector3.one * 0.6f;
				}
			}
		}
	}
}
