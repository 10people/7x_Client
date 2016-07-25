using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SportResult : GeneralInstance<SportResult> {
	
	public GameObject m_resultPage;
	public GameObject m_rankRewardPage;

	public delegate void SportResultDelegate ();
	public SportResultDelegate M_SportResultDelegate;

	new void Awake ()
	{
		base.Awake ();
	}

	#region Result
	public UIScrollView m_sc;
	public UIScrollBar m_sb;
	
	public GameObject m_resultObj;
	private List<GameObject> m_resultList = new List<GameObject> ();
	
	public UISprite m_junXianIcon;
	public UISprite m_junxian;
	public UISprite m_rankSprite;
	public UILabel m_rank;
	
	private GameObject m_iconSamplePrefab;
	private List<GameObject> m_rewardList = new List<GameObject> ();
	private List<string[]> m_rewardStrList = new List<string[]> ();

	public void InItSportResult (int tempRank)
	{
		m_resultPage.SetActive (true);
		m_rankRewardPage.SetActive (false);

		List<BaiZhanTemplate> baizhanTemps = BaiZhanTemplate.templates;
		
		m_resultList = QXComData.CreateGameObjectList (m_resultObj,baizhanTemps.Count,m_resultList);
		
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

		int tempXmlIndex = -1;
		int tempId = 0;
		for (int i = 0;i < baizhanTemps.Count;i ++)
		{
			if (SportPage.m_instance.SportTemp.id == baizhanTemps[i].id)
			{
				if (i > 0)
				{
					tempXmlIndex = i - 1;
					tempId = baizhanTemps[i - 1].id;
					break;
				}
			}
		}

		for (int i = 0;i < baizhanTemps.Count;i ++)
		{
			m_resultList[i].transform.localPosition = new Vector3(0,-74 * i,0);
			m_sc.UpdateScrollbars (true);
			SportResultItem reward = m_resultList[i].GetComponent<SportResultItem> ();
			reward.InItRewardItem (baizhanTemps[i],tempId);
		}
//		Debug.Log ("tempXmlId；" + tempXmlIndex);
		if (tempXmlIndex >= 0)
		{
			QXComData.SetWidget (m_sc,m_sb,m_resultList[tempXmlIndex]);
		}

		m_rewardStrList.Clear ();
		string[] rewardLength = BaiZhanTemplate.getBaiZhanTemplateById(SportPage.m_instance.SportTemp.id).dayAward.Split ('#');
		for (int i = 0;i < rewardLength.Length;i ++)
		{
			m_rewardStrList.Add (rewardLength[i].Split (':'));
		}
		
		m_rankSprite.spriteName = tempRank <= 3 ? "rank" + tempRank : "";
		m_rank.text = tempRank <= 3 ? "" : "第" + tempRank + "名";
		
		m_junXianIcon.spriteName = "junxian" + BaiZhanTemplate.getBaiZhanTemplateById (SportPage.m_instance.SportTemp.id).jibie;
		m_junxian.spriteName = "JunXian_" + BaiZhanTemplate.getBaiZhanTemplateById (SportPage.m_instance.SportTemp.id).jibie;
		
		if (m_iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			WWW tempWww = null;
			IconSampleLoadCallBack(ref tempWww, null, m_iconSamplePrefab);
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (m_iconSamplePrefab == null) 
		{
			m_iconSamplePrefab = p_object as GameObject;
		}
		
		int tempCount = m_rewardStrList.Count - m_rewardList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (m_iconSamplePrefab);
				
				obj.SetActive (true);
				obj.transform.parent = m_junXianIcon.transform.parent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				
				m_rewardList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				Destroy (m_rewardList[m_rewardList.Count - 1]);
				m_rewardList.RemoveAt (m_rewardList.Count - 1);
			}
		}
		
		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			m_rewardList[i].transform.localPosition = new Vector3(60 + 55 * i,0,0);
			
			CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (m_rewardStrList[i][1]));
			string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
			string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
			
			IconSampleManager iconSample = m_rewardList[i].GetComponent<IconSampleManager> ();
			iconSample.SetIconByID (int.Parse (m_rewardStrList[i][1]),m_rewardStrList[i][2],2);
			//			iconSample.SetIconBasicDelegate (true,true,null);
			iconSample.SetIconPopText(int.Parse (m_rewardStrList[i][1]), nameStr, mdesc, 1);
			
			m_rewardList[i].transform.localScale = Vector3.one * 0.5f;
		}
	}
	#endregion

	#region RankReward
	public UILabel yuanBaoLabel;
	
	public void InItRankReward ()
	{
		m_resultPage.SetActive (false);
		m_rankRewardPage.SetActive (true);
		yuanBaoLabel.text = (BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (1).yuanbao  - (SportPage.m_instance.SportResp.historyHighRank > BaiZhanRankTemplate.GetTemplatesCount() ? 
		                                                                                      0 : BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (SportPage.m_instance.SportResp.historyHighRank).yuanbao)).ToString ();
	}
	#endregion

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			if (M_SportResultDelegate != null)
			{
				M_SportResultDelegate ();
			}

			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
