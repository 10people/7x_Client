using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpRewardItem : MonoBehaviour {

	public UISprite bgSprite;

	public UISprite junXianIcon;
	public UISprite junXian;

	public UISprite rankSprite;

	public UILabel rankLabel;

	public UILabel rankDesLabel;

	private List<string[]> rewardList = new List<string[]> ();

	private GameObject iconSamplePrefab;
	private List<GameObject> rewardItemList = new List<GameObject> ();

	public void InItRewardItem (BaiZhanTemplate template,int tempXmlId)
	{
		junXianIcon.spriteName = "junxian" + template.jibie;
		junXian.spriteName = "JunXian_" + template.jibie;

		int maxRank = template.maxRank;
		int minRank = template.minRank;

		rankSprite.spriteName = maxRank == minRank ? "rank" + maxRank : "";
		rankLabel.text = maxRank == minRank ? "" : MyColorData.getColorString (3,"第" + minRank + "名-第" + maxRank + "名");

		rankDesLabel.text = (tempXmlId + 1) == template.id ? MyColorData.getColorString (5,"排名提升，可获得更多奖励") : "";
		bgSprite.color = (tempXmlId + 1) == template.id ? new Color(1,0.75f,0.35f) : Color.white;

		rewardList.Clear ();
		string[] rewardLength = template.dayAward.Split ('#');
		for (int i = 0;i < rewardLength.Length;i ++)
		{
			rewardList.Add (rewardLength[i].Split (':'));
		}

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
				obj.transform.parent = this.transform;
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
}
