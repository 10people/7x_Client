using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SportHighRank : GeneralInstance<SportHighRank> {

	public delegate void SportHighRankDelegate ();
	public SportHighRankDelegate M_SportHighRankDelegate;

	public UISprite m_leftJunXian;
	public UISprite m_rightJunXian;

	public UILabel m_leftRank;
	public UILabel m_rightRank;

	public GameObject m_rewardBg;

	private GameObject m_iconSamplePrefab;

	private List<RewardData> m_rewardDataList;

	public GameObject m_effect;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItHighRank (List<RewardData> tempDataList)
	{
		m_rewardDataList = tempDataList;

		m_leftRank.text = SportPage.m_instance.SportResp.lasthistoryHighRank.ToString ();
		m_rightRank.text = SportPage.m_instance.SportResp.historyHighRank.ToString ();

		m_leftJunXian.spriteName = "junxian" + BaiZhanTemplate.getBaiZhanJiBieByRank (SportPage.m_instance.SportResp.lasthistoryHighRank);
		m_rightJunXian.spriteName = "junxian" + BaiZhanTemplate.getBaiZhanJiBieByRank (SportPage.m_instance.SportResp.historyHighRank);

		if (m_iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}

		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_effect, EffectIdTemplate.GetPathByeffectId(620214), null);
	}
	
	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		m_iconSamplePrefab = (GameObject)Instantiate (p_object);
		
		m_iconSamplePrefab.SetActive(true);
		m_iconSamplePrefab.transform.parent = m_rewardBg.transform;
		m_iconSamplePrefab.transform.localPosition = new Vector3(0,-15,0);
		
		InItIconSample ();
	}
	
	void InItIconSample ()
	{
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (m_rewardDataList[0].itemId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (m_rewardDataList[0].itemId);
		
		IconSampleManager fuShiIconSample = m_iconSamplePrefab.GetComponent<IconSampleManager>();
		fuShiIconSample.SetIconByID (m_rewardDataList[0].itemId,"x" + m_rewardDataList[0].itemCount,2);
		fuShiIconSample.SetIconPopText(m_rewardDataList[0].itemId, nameStr, mdesc, 1);
		//		iconSamplePrefab.transform.localScale = Vector3.one * 0.6f;
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			if (M_SportHighRankDelegate != null)
			{
				GeneralRewardManager.Instance().CreateReward (m_rewardDataList);
				M_SportHighRankDelegate ();
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
