using UnityEngine;
using System.Collections;

public class GeneralReward : MonoBehaviour {

	public GameObject transObj;

	public UILabel numLabel;

	private GameObject iconSamplePrefab;

	private int itemId;
	private int itemCount;

	public UIAtlas atlasFuwen;
	public UIAtlas atlasEquip;

	private float moveFirstTime;
	private float stopTime;
	private float moveSecondTime;
	private RewardData rewardData;

	/// <summary>
	/// Ins it reward item.
	/// </summary>
	/// <param name="tempData">Temp data.</param>
	public void InItRewardItem (RewardData tempData)
	{
		rewardData = tempData;

		if (tempData.moveTime1 == 0 || tempData.moveTime2 == 0)
		{
			Debug.Log ("Do not moveTime set to 0");
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

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
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

		//0普通道具;3当铺材料;5秘宝碎片;6进阶材料;7基础宝石;8高级宝石;9强化材料
		CommonItemTemplate comTemplate = CommonItemTemplate.getCommonItemTemplateById (itemId);
		if(comTemplate.itemType == 7 || comTemplate.itemType == 8)//符文
		{
			fuShiIconSample.SetIconType(IconSampleManager.IconType.FuWen);
		}
		else
		{
			fuShiIconSample.SetIconType(IconSampleManager.IconType.equipment);;
		}

		fuShiIconSample.SetIconBasic(1,itemId.ToString ());

		iconSample.transform.localScale = Vector3.one * 0.4f;
	}

	void MoveFirst ()
	{
		Hashtable move = new Hashtable ();
		move.Add ("time",moveFirstTime);
		move.Add ("position",Vector3.zero);
		move.Add ("islocal",true);
		move.Add ("oncomplete","MoveStop");
		move.Add ("oncompletetarget",this.gameObject);
		move.Add ("easetype",rewardData.itweenType1);
		iTween.MoveTo (transObj,move);
		
		Hashtable scale = new Hashtable ();
		scale.Add ("time",moveFirstTime);
		scale.Add ("scale",Vector3.one);
		scale.Add ("easetype",rewardData.itweenType1);
		iTween.ScaleTo (transObj,scale);
	}
	
	void MoveStop ()
	{
		StartCoroutine ("MoveSecond");
	}
	
	IEnumerator MoveSecond ()
	{
		yield return new WaitForSeconds (stopTime);

		GeneralRewardManager.Instance ().Reward_Index ++;
		GeneralRewardManager.Instance ().CheckReward ();

		Hashtable move = new Hashtable ();
		move.Add ("time",moveSecondTime * 2);
		move.Add ("position",new Vector3 (0,100,0));
		move.Add ("islocal",true);
		move.Add ("oncomplete","DestroyObj");
		move.Add ("oncompletetarget",this.gameObject);
		move.Add ("easetype",rewardData.itweenType2);
		iTween.MoveTo (transObj,move);
	}
	
	void DestroyObj ()
	{
		GeneralRewardManager.Instance ().RefreshItemList (this.gameObject);
	}

	void Update ()
	{
		float rewardAlpha = transObj.GetComponent<UIWidget> ().alpha;
		if (transObj.transform.localPosition.y <= 0)
		{
			rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y) / 225);
		}
		else
		{
			rewardAlpha = 1 - (Mathf.Abs (transObj.transform.localPosition.y) / 100);
		}
		transObj.GetComponent<UIWidget> ().alpha = rewardAlpha;
	}
}
