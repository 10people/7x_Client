using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RechargeVipItem : MonoBehaviour {

	public VipTemplate M_VipTemp;

	public UILabel m_vipDesLabel;

	public UIScrollView m_vipSc;
	public UIScrollBar m_vipSb;
	public GameObject m_getBtn;
	public UILabel m_getLabel;
	private SparkleEffectItem m_sparkEffect;

	public UISprite m_desVip;
	public UISprite m_rewardVip;

	public UILabel m_totleRecharge;

	public BoxCollider m_labelDragBox;

	public GameObject m_rewardParent;
	private GameObject m_iconSample;
	private List<GameObject> m_rewardList = new List<GameObject> ();
	private List<string> m_rewardStrList = new List<string> ();

	private int m_maxHeigh = 300;

	[HideInInspector]public List<RewardData> M_RewardDataList = new List<RewardData> ();

	public void InItVipItem (VipTemplate template)
	{
		M_VipTemp = template;

		m_vipDesLabel.text = "";
		string[] desLen = DescIdTemplate.GetDescriptionById (template.desc).Split ('#');
		for (int i = 0;i < desLen.Length;i ++)
		{
			m_vipDesLabel.text += (i < desLen.Length - 1 ? desLen[i] + "\n" : desLen[i]);
		}

		m_desVip.spriteName = "v" + M_VipTemp.lv;
		m_rewardVip.spriteName = "v" + M_VipTemp.lv;
		m_totleRecharge.text = M_VipTemp.needNum.ToString ();

		int desLine = m_vipDesLabel.height; //限制330

		m_vipSc.UpdateScrollbars (true);
		m_vipSc.enabled = desLine > m_maxHeigh ? true : false;
		m_vipSb.gameObject.SetActive (desLine > m_maxHeigh ? true : false);
		m_labelDragBox.enabled = desLine > m_maxHeigh ? true : false;

		m_rewardStrList.Clear ();
		M_RewardDataList.Clear ();
		string[] reward = VipGiftTemplate.GetVipGiftTemplateByVip (template.lv).award.Split ('#');
		foreach (string str in reward)
		{
			m_rewardStrList.Add (str);
		}

		if (m_iconSample == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			CreateRewardList ();
		}

		m_sparkEffect = m_getBtn.GetComponent<SparkleEffectItem> ();

		if (M_VipTemp.lv <= QXComData.JunZhuInfo ().vipLv)
		{
			m_getBtn.SetActive (true);
			m_sparkEffect.enabled = !RechargePage.m_instance.IsRewardGet (M_VipTemp.lv);
			QXComData.SetBtnState (m_getBtn,!RechargePage.m_instance.IsRewardGet (M_VipTemp.lv));
			m_getLabel.text = RechargePage.m_instance.IsRewardGet (M_VipTemp.lv) ? "已 领" : "领 取";
		}
		else
		{
			m_sparkEffect.enabled = false;
			m_getBtn.SetActive (false);
		}
	}

	public void SetBtn ()
	{
		m_sparkEffect.enabled = false;
		QXComData.SetBtnState (m_getBtn,false);
		m_getLabel.text = "已 领";
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		m_iconSample = Instantiate (p_object) as GameObject;
		m_iconSample.SetActive (false);
		m_iconSample.transform.parent = m_rewardStrList.Count > 2 ? m_rewardParent.transform : m_rewardParent.transform.parent;
		CreateRewardList ();
	}

	void CreateRewardList ()
	{
		m_rewardList = QXComData.CreateGameObjectList (m_iconSample,m_rewardStrList.Count,m_rewardList);
		for (int i = 0;i < m_rewardList.Count;i ++)
		{
			if (m_rewardStrList.Count <= 2)
			{
				m_rewardList[i].transform.localPosition = new Vector3(i * 45 - (m_rewardStrList.Count - 1) * 22.5f,10,0);
			}

			string[] rewardStr = m_rewardStrList[i].Split (':');
			int rewardId = int.Parse (rewardStr[1]);
			string rewardNum = rewardStr[2];

			M_RewardDataList.Add (new RewardData (rewardId,int.Parse (rewardNum)));

			CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (rewardId);
			string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
			string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
			
			IconSampleManager iconSample = m_rewardList[i].GetComponent<IconSampleManager> ();
			iconSample.SetIconByID (rewardId,"x" + rewardNum,2);
			iconSample.SetIconPopText(int.Parse (rewardStr[1]), nameStr, mdesc, 1);
			
			m_rewardList[i].transform.localScale = Vector3.one * 0.75f;
		}
	}
}
