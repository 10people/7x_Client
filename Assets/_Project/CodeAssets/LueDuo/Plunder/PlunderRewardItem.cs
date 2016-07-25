using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderRewardItem : MonoBehaviour {

	public UISprite bgSprite;
	
	public UISprite rankSprite;

	public GameObject m_rankDiWen;
	
	public UILabel rankLabel;
	
	public UILabel rankDesLabel;
	
	private List<string[]> rewardList = new List<string[]> ();
	
	private GameObject iconSamplePrefab;
	private List<GameObject> rewardItemList = new List<GameObject> ();

	/// <summary>
	/// Ins it reward item.
	/// </summary>
	/// <param name="tempLate">Temp late.</param>
	/// <param name="isCurItem">If set to <c>true</c> is current item.</param>
	public void InItRewardItem (LueDuoPersonRankTemplate tempLate,int tempNextIndex,int tempIndex)
	{
		int maxRank = tempLate.max;
		int minRank = tempLate.min;

		rankSprite.spriteName = maxRank == minRank ? "rank" + maxRank : "";
		m_rankDiWen.SetActive (maxRank == minRank && minRank < 4? true : false);
		rankLabel.text = maxRank == minRank ? "" : "第" + minRank + "名-第" + maxRank + "名";
		
		rankDesLabel.text = tempIndex == tempNextIndex ? MyColorData.getColorString (5,"排名提升，可获得更多奖励") : "";
//		bgSprite.color = tempIndex == tempNextIndex ? new Color(1,0.75f,0.35f) : Color.white;
		QXComData.SetBgSprite2 (bgSprite,tempIndex == tempNextIndex ? true : false);

		rewardList.Clear ();
		string[] rewardLength = tempLate.award.Split ('#');
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

	/// <summary>
	/// Ins it reward item.
	/// </summary>
	/// <param name="tempLate">Temp late.</param>
	/// <param name="isCurItem">If set to <c>true</c> is current item.</param>
	public void InItRewardItem (LueDuoLianmengRankTemplate tempLate,int tempNextIndex,int tempIndex)
	{
		int maxRank = tempLate.max;
		int minRank = tempLate.min;

		rankSprite.spriteName = maxRank == minRank ? "rank" + maxRank : "";
		m_rankDiWen.SetActive (maxRank == minRank && minRank < 4 ? true : false);
		rankLabel.text = maxRank == minRank ? (maxRank > 3 ? "第" + maxRank + "名" : "") : "第" + minRank + "名-第" + maxRank + "名";
		
		rankDesLabel.text = tempIndex == tempNextIndex ? MyColorData.getColorString (5,"排名提升，可获得更多奖励") : "";
//		bgSprite.color = tempIndex == tempNextIndex ? new Color(1,0.75f,0.35f) : Color.white;
		QXComData.SetBgSprite2 (bgSprite,tempIndex == tempNextIndex ? true : false);

		rewardList.Clear ();
		rewardList.Add (new string[]{"0","900017",tempLate.award.ToString ()});
		
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
