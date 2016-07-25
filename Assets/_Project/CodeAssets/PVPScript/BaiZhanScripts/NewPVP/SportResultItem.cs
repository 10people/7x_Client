using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SportResultItem : MonoBehaviour {

	public UISprite m_bgSprite;
	
	public UISprite m_junXianIcon;
	public UISprite m_junXian;
	
	public UISprite m_rankSprite;
	
	public UILabel m_rank;
	
	public UILabel m_rankDes;
	
	private List<string[]> m_rewardStrList = new List<string[]> ();
	
	private GameObject m_iconSamplePrefab;
	private List<GameObject> m_rewardList = new List<GameObject> ();

	public void InItRewardItem (BaiZhanTemplate template,int tempXmlId)
	{
		m_junXianIcon.spriteName = "junxian" + template.jibie;
		m_junXian.spriteName = "JunXian_" + template.jibie;
		
		int maxRank = template.maxRank;
		int minRank = template.minRank;
		
		m_rankSprite.spriteName = maxRank == minRank ? "rank" + maxRank : "";
		m_rank.text = maxRank == minRank ? "" : minRank < 2001 ? "第" + minRank + "名-第" + maxRank + "名" : "2000名之后";
		
		m_rankDes.text = tempXmlId == template.id ? MyColorData.getColorString (104,"排名提升，可获得更多奖励") : "";
//		m_bgSprite.color = tempXmlId == template.id ? new Color(1,0.75f,0.35f) : Color.white;
//		m_bgSprite.spriteName = tempXmlId == template.id ? "jianbianbgliang" : "thirdBg";
		QXComData.SetBgSprite (m_bgSprite,tempXmlId == template.id ? true : false);
		
		m_rewardStrList.Clear ();
		string[] rewardLength = template.dayAward.Split ('#');
		for (int i = 0;i < rewardLength.Length;i ++)
		{
			m_rewardStrList.Add (rewardLength[i].Split (':'));
		}
		
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
				obj.transform.parent = this.transform;
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
}
