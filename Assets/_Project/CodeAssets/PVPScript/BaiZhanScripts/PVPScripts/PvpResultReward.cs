using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpResultReward : MonoBehaviour {

	public static PvpResultReward resultReward;

	public ScaleEffectController rewardWindow;

	public List<EventHandler> rewardHandlerList = new List<EventHandler>();

	public UIScrollView rewardSc;
	public UIScrollBar rewardSb;

	private List<GameObject> rankItemList = new List<GameObject> ();
	public GameObject rewardItemObj;

	public UISprite junXianIcon;
	public UISprite junXian;
	public UISprite rankSprite;
	public UILabel rankLabel;

	private GameObject iconSamplePrefab;
	private List<GameObject> rewardItemList = new List<GameObject> ();
	private List<string[]> rewardList = new List<string[]> ();

	void Awake ()
	{
		resultReward = this;
	}

	void OnDestroy ()
	{
		resultReward = null;
	}

	/// <summary>
	/// Ins it result reward.
	/// </summary>
	/// <param name="tempRank">Temp rank.</param>
	/// <param name="tempXmlId">Temp xml identifier.</param>
	public void InItResultReward (int tempRank,int tempXmlId)
	{
		rewardWindow.OnOpenWindowClick ();

		List<BaiZhanTemplate> baizhanTemps = BaiZhanTemplate.templates;

		rankItemList = QXComData.CreateGameObjectList (rewardItemObj,baizhanTemps.Count,rankItemList);

		for (int i = 0;i < baizhanTemps.Count - 1;i ++)
		{
			for (int j = 0;j < baizhanTemps.Count - i - 1;j ++)
			{
				if (baizhanTemps[j].id < baizhanTemps[j + 1].id)
				{
					BaiZhanTemplate temp = baizhanTemps[j];
					baizhanTemps[j] = baizhanTemps[j + 1];
					baizhanTemps[j + 1] = temp;
				}
			}
		}

		for (int i = 0;i < baizhanTemps.Count;i ++)
		{
			rankItemList[i].transform.localPosition = new Vector3(0,-80 * i,0);
			rewardSc.UpdateScrollbars (true);
			PvpRewardItem reward = rankItemList[i].GetComponent<PvpRewardItem> ();
			reward.InItRewardItem (baizhanTemps[i],tempXmlId);
		}
//		Debug.Log ("tempXmlId；" + tempXmlId);
		if (tempXmlId < baizhanTemps.Count)
		{
			QXComData.SetWidget (rewardSc,rewardSb,rankItemList[baizhanTemps.Count - 1 -tempXmlId]);
		}

		rewardList.Clear ();
		string[] rewardLength = BaiZhanTemplate.getBaiZhanTemplateById(tempXmlId).dayAward.Split ('#');
		for (int i = 0;i < rewardLength.Length;i ++)
		{
			rewardList.Add (rewardLength[i].Split (':'));
		}

		rankSprite.spriteName = tempRank <= 3 ? "rank" + tempRank : "";
		rankLabel.text = tempRank <= 3 ? "" : "第" + tempRank + "名";

		junXianIcon.spriteName = "junxian" + BaiZhanTemplate.getBaiZhanTemplateById (tempXmlId).jibie;
		junXian.spriteName = "JunXian_" + BaiZhanTemplate.getBaiZhanTemplateById (tempXmlId).jibie;

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

		foreach (EventHandler handler in rewardHandlerList)
		{
			handler.m_click_handler -= CloseWindow;
			handler.m_click_handler += CloseWindow;
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (iconSamplePrefab == null) 
		{
			iconSamplePrefab = p_object as GameObject;
		}
		
		int tempCount = rewardList.Count - rewardItemList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (iconSamplePrefab);
				
				obj.SetActive (true);
				obj.transform.parent = junXianIcon.transform.parent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				
				rewardItemList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				Destroy (rewardItemList[rewardItemList.Count - 1]);
				rewardItemList.RemoveAt (rewardItemList.Count - 1);
			}
		}
		
		for (int i = 0;i < rewardItemList.Count;i ++)
		{
			rewardItemList[i].transform.localPosition = new Vector3(60 + 55 * i,0,0);
			
			CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (rewardList[i][1]));
			string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
			string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
			
			IconSampleManager iconSample = rewardItemList[i].GetComponent<IconSampleManager> ();
			iconSample.SetIconByID (int.Parse (rewardList[i][1]),rewardList[i][2],1);
//			iconSample.SetIconBasicDelegate (true,true,null);
			iconSample.SetIconPopText(int.Parse (rewardList[i][1]), nameStr, mdesc, 1);
			
			rewardItemList[i].transform.localScale = Vector3.one * 0.5f;
		}
	}

	void CloseWindow (GameObject obj)
	{
		gameObject.SetActive (false);
	}
}
