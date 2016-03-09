using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderReward : MonoBehaviour {

	public static PlunderReward plunderReward;

	#region Plunder Personal Reward
	public UIScrollView plunderPSc;
	public UIScrollBar plunderPSb;

	private List<GameObject> pRewardItemList = new List<GameObject>();
	public GameObject pRewardItem;

	private int curPRewardIndex;

	public UISprite pRankSprite;
	public UILabel pRankLabel;
	
	private GameObject iconSamplePrefab;
	private List<GameObject> pRewardSampleList = new List<GameObject> ();
	private List<string[]> pRewardList = new List<string[]> ();
	#endregion

	#region Plunder Alliance Reward
	public UIScrollView plunderASc;
	public UIScrollBar plunderASb;
	
	private List<GameObject> aRewardItemList = new List<GameObject>();
	public GameObject aRewardItem;

	private int curARewardIndex;

	public UISprite aRankSprite;
	public UILabel aRankLabel;
	
//	private GameObject iconSamplePrefab;
	private List<GameObject> aRewardSampleList = new List<GameObject> ();
	private List<string[]> aRewardList = new List<string[]> ();
	#endregion

	void Awake ()
	{
		plunderReward = this;
	}

	void OnDestroy ()
	{
		plunderReward = null;
	}

	/// <summary>
	/// Ins it plunder P reward page.
	/// </summary>
	/// <param name="tempPRank">Temp P rank.</param>
	public void InItPlunderPRewardPage (int tempPRank)
	{
		pRewardItemList = QXComData.CreateGameObjectList (pRewardItem,LueDuoPersonRankTemplate.m_templates.Count,pRewardItemList);

		LueDuoPersonRankTemplate tempLate = LueDuoPersonRankTemplate.GetLueDuoPersonRankTemplateByRank (tempPRank);

		for (int i = 0;i < LueDuoPersonRankTemplate.m_templates.Count;i ++)
		{
			if (tempLate == LueDuoPersonRankTemplate.m_templates[i])
			{
				curPRewardIndex = i;
			}
		}
//		Debug.Log ("curPRewardIndex：" + curPRewardIndex);
		for (int i = 0;i < pRewardItemList.Count;i ++)
		{
			pRewardItemList[i].transform.localPosition = new Vector3(0,-80 * i,0);
			
			PlunderRewardItem reward = pRewardItemList[i].GetComponent<PlunderRewardItem> ();
			reward.InItRewardItem (LueDuoPersonRankTemplate.m_templates[i],curPRewardIndex - 1,i);
		}
		
		if (curPRewardIndex > 0 && curPRewardIndex < LueDuoPersonRankTemplate.m_templates.Count - 1)
		{
			QXComData.SetWidget (plunderPSc,plunderPSb,pRewardItemList[curPRewardIndex - 1]);
		}
		
		pRankSprite.spriteName = tempPRank <= 3 ? "rank" + tempPRank : "";
		pRankLabel.text = tempPRank <= 3 ? "" : "第" + tempPRank + "名";

		pRewardList.Clear ();
		string[] rewardLength = tempLate.award.Split ('#');
		for (int i = 0;i < rewardLength.Length;i ++)
		{
			pRewardList.Add (rewardLength[i].Split (':'));
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
		
		int tempCount = pRewardList.Count - pRewardSampleList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (iconSamplePrefab);
				
				obj.SetActive (true);
				obj.transform.parent = pRankSprite.transform.parent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				
				pRewardSampleList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				Destroy (pRewardSampleList[pRewardSampleList.Count - 1]);
				pRewardSampleList.RemoveAt (pRewardSampleList.Count - 1);
			}
		}
		
		for (int i = 0;i < pRewardSampleList.Count;i ++)
		{
			pRewardSampleList[i].transform.localPosition = new Vector3(60 + 55 * i,0,0);
			
			CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (pRewardList[i][1]));
			string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
			string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
			
			IconSampleManager iconSample = pRewardSampleList[i].GetComponent<IconSampleManager> ();
			iconSample.SetIconByID (int.Parse (pRewardList[i][1]),pRewardList[i][2],1);
			//			iconSample.SetIconBasicDelegate (true,true,null);
			iconSample.SetIconPopText(int.Parse (pRewardList[i][1]), nameStr, mdesc, 1);
			
			pRewardSampleList[i].transform.localScale = Vector3.one * 0.5f;
		}
	}

	/// <summary>
	/// Ins it plunder A reward page.
	/// </summary>
	/// <param name="tempARank">Temp A rank.</param>
	public void InItPlunderARewardPage (int tempARank)
	{
		aRewardItemList = QXComData.CreateGameObjectList (aRewardItem,LueDuoLianmengRankTemplate.m_templates.Count,aRewardItemList);
		
		LueDuoLianmengRankTemplate tempLate = LueDuoLianmengRankTemplate.GetLueDuoLianmengRankTemplateByRank (tempARank);
		
		for (int i = 0;i < LueDuoLianmengRankTemplate.m_templates.Count;i ++)
		{
			if (tempLate == LueDuoLianmengRankTemplate.m_templates[i])
			{
				curARewardIndex = i;
			}
		}
//		Debug.Log ("curARewardIndex：" + curARewardIndex);
		for (int i = 0;i < aRewardItemList.Count;i ++)
		{
			aRewardItemList[i].transform.localPosition = new Vector3(0,-80 * i,0);
			
			PlunderRewardItem reward = aRewardItemList[i].GetComponent<PlunderRewardItem> ();
			reward.InItRewardItem (LueDuoLianmengRankTemplate.m_templates[i],curARewardIndex - 1,i);
		}
		
		if (curARewardIndex > 0 && curARewardIndex < LueDuoLianmengRankTemplate.m_templates.Count - 1)
		{
			QXComData.SetWidget (plunderASc,plunderASb,aRewardItemList[curARewardIndex - 1]);
		}
		
		aRankSprite.spriteName = tempARank <= 3 ? "rank" + tempARank : "";
		aRankLabel.text = tempARank <= 3 ? "" : "第" + tempARank + "名";

		aRewardList.Clear ();
		aRewardList.Add (new string[]{"0","900017",tempLate.award.ToString ()});

		if (iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack2);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack2(ref tempWww, null, iconSamplePrefab);
		}
	}

	private void IconSampleLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
	{
		if (iconSamplePrefab == null) 
		{
			iconSamplePrefab = p_object as GameObject;
		}
		
		int tempCount = aRewardList.Count - aRewardSampleList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (iconSamplePrefab);
				
				obj.SetActive (true);
				obj.transform.parent = aRankSprite.transform.parent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				
				aRewardSampleList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				Destroy (aRewardSampleList[aRewardSampleList.Count - 1]);
				aRewardSampleList.RemoveAt (aRewardSampleList.Count - 1);
			}
		}
		
		for (int i = 0;i < aRewardSampleList.Count;i ++)
		{
			aRewardSampleList[i].transform.localPosition = new Vector3(60 + 55 * i,0,0);
			
			CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (aRewardList[i][1]));
			string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
			string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
			
			IconSampleManager iconSample = aRewardSampleList[i].GetComponent<IconSampleManager> ();
			iconSample.SetIconByID (int.Parse (aRewardList[i][1]),aRewardList[i][2],1);
			//			iconSample.SetIconBasicDelegate (true,true,null);
			iconSample.SetIconPopText(int.Parse (aRewardList[i][1]), nameStr, mdesc, 1);
			
			aRewardSampleList[i].transform.localScale = Vector3.one * 0.5f;
		}
	}
}
