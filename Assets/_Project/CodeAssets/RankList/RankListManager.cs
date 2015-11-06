using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RankListManager : MonoBehaviour {

	public RankingResp m_rankResp;

	public List<JunZhuInfo> junzhuInfoList = new List<JunZhuInfo> ();
	public List<LianMengInfo> allianceInfoList = new List<LianMengInfo> ();

	private List<GameObject> junzhuItemList = new List<GameObject> ();
	private List<GameObject> allianceItemList = new List<GameObject> ();

	public GameObject heroItem;
	public GameObject heroGrid;
	
	public GameObject allianceItem;
	public GameObject allianceGrid;

	public GameObject noHeroDesLabel;
	public GameObject noAllianceDesLabel;

	//获得排名榜返回信息
	public void GetRankResp (RankingResp tempResp)
	{
		m_rankResp = tempResp;

		if (tempResp.junList.Count > 0)
		{
			noHeroDesLabel.SetActive (false);
			junzhuInfoList = tempResp.junList;
			InItHeroList ();
		}
		else
		{
			noHeroDesLabel.SetActive (true);
		}

		if (tempResp.mengList.Count > 0)
		{
			noAllianceDesLabel.SetActive (false);
			allianceInfoList = tempResp.mengList;
			InItAllianceList ();
		}
		else
		{
			noAllianceDesLabel.SetActive (true);
		}
	}

	//创建英雄榜
	void InItHeroList ()
	{
		for (int i = 0;i < junzhuInfoList.Count - 1;i ++)
		{
			for (int j = 0;j < junzhuInfoList.Count - i - 1;j ++)
			{
				if (junzhuInfoList[j].rank > junzhuInfoList[j + 1].rank)
				{
					JunZhuInfo tempJunZhu = junzhuInfoList[j];

					junzhuInfoList[j] = junzhuInfoList[j + 1];

					junzhuInfoList[j + 1] = tempJunZhu;
				}
			}
		}

		ClearItems (junzhuItemList);

		StartCoroutine (CreateHeroItems ());
	}

	IEnumerator CreateHeroItems ()
	{
		for (int i = 0;i < junzhuInfoList.Count;i ++)
		{
			GameObject hero = (GameObject)Instantiate (heroItem);
			
			hero.SetActive (true);
			hero.name = "HeroItem" + junzhuInfoList[i].rank;
			
			hero.transform.parent = heroGrid.transform;
			hero.transform.localPosition = new Vector3(0,-i * 100,0);
			hero.transform.localScale = Vector3.one;
			
			HeroItemInfo heroInfo = hero.GetComponent<HeroItemInfo> ();
			heroInfo.GetHeroInfo (junzhuInfoList[i]);

			yield return new WaitForSeconds(0.02f);
		}
	}

	//创建联盟榜
	void InItAllianceList ()
	{
		for (int i = 0;i < allianceInfoList.Count - 1;i ++)
		{
			for (int j = 0;j < allianceInfoList.Count - i - 1;j ++)
			{
				if (allianceInfoList[j].rank > allianceInfoList[j + 1].rank)
				{
					LianMengInfo tempAlliance = allianceInfoList[j];
					
					allianceInfoList[j] = allianceInfoList[j + 1];
					
					allianceInfoList[j + 1] = tempAlliance;
				}
			}
		}

		ClearItems (allianceItemList);

		StartCoroutine (CreateAllianceItems ());
	}

	IEnumerator CreateAllianceItems ()
	{
		for (int i = 0;i < allianceInfoList.Count;i ++)
		{
			//			Debug.Log ("Rank:" + allianceInfoList[i].rank);
			GameObject alliance = (GameObject)Instantiate (allianceItem);
			
			alliance.SetActive (true);
			alliance.name = "AllianceItem" + allianceInfoList[i].rank;
			
			alliance.transform.parent = allianceGrid.transform;
			alliance.transform.localPosition = new Vector3(0,-i * 100,0);
			alliance.transform.localScale = Vector3.one;
			
			AllianceItemInfo allianceInfo = alliance.GetComponent<AllianceItemInfo> ();
			allianceInfo.GetAllianceInfo (allianceInfoList[i]);

			yield return new WaitForSeconds(0.02f);
		}
	}

	//清除gameObj
	void ClearItems (List<GameObject> tempList)
	{
		foreach (GameObject tempObj in tempList)
		{
			Destroy (tempObj);
		}

		tempList.Clear ();
	}

	void Update ()
	{
		if (junzhuInfoList.Count < 5)
		{
			heroGrid.GetComponent<ItemTopCol> ().enabled = true;
		}
		else
		{
			heroGrid.GetComponent<ItemTopCol> ().enabled = false;
		}

		if (allianceInfoList.Count < 5)
		{
			allianceGrid.GetComponent<ItemTopCol> ().enabled = true;
		}
		else
		{
			allianceGrid.GetComponent<ItemTopCol> ().enabled = false;
		}
	}

	public void TurnToCreateAllianceBtn ()
	{
		if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
			                        NoAllianceLoadCallback);
		}

		RankListData.Instance ().DestroyRoot ();
	}

	public void NoAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject Secrtgb = (GameObject)Instantiate(p_object);
		
		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}
	}
}
