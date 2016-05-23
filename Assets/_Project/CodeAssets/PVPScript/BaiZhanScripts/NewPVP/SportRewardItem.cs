using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SportRewardItem : MonoBehaviour {

	private GameObject iconSampleObj;
	public UILabel m_num;
	private string m_rewardStr;

	public void InItSportRewardItem (string tempReward)
	{
		m_rewardStr = tempReward;

		if (iconSampleObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ICON_SAMPLE ),SportRewardItemLoadCallback );
		}
		else
		{
			InItSportRewardItem ();
		}
	}

	private void SportRewardItemLoadCallback ( ref WWW p_www, string p_path, Object p_object )
	{
		iconSampleObj = Instantiate (p_object) as GameObject;
		iconSampleObj.SetActive (true);
		iconSampleObj.transform.parent = this.transform;
		iconSampleObj.transform.localPosition = new Vector3 (-35,0,0);

		InItSportRewardItem ();
	}

	private void InItSportRewardItem ()
	{
		string[] m_reward = m_rewardStr.Split (':');

		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (m_reward[1]));
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);

		IconSampleManager iconSample = iconSampleObj.GetComponent<IconSampleManager> ();
		iconSample.SetIconByID (int.Parse (m_reward[1]),"",2);
		iconSample.SetIconPopText(int.Parse (m_reward[1]), nameStr, mdesc, 1);

		iconSampleObj.transform.localScale = Vector3.one * 0.3f;

		m_num.text = m_reward[2];
	}
}
