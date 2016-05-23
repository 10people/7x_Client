using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class OtherCityItem : MonoBehaviour {
	
	public CityInfo M_CityInfo;
	[HideInInspector]
	public bool M_canJoin = false;

	public UILabel m_title;
	public UISprite m_LevelIcon;
	public UILabel m_enterNum;

	public GameObject m_passObj;

	public GameObject m_enterBtn;
	public UILabel m_btnLabel;

	public JCZCityTemplate M_Template;

	public GameObject m_iconSampleParent;
	private GameObject m_iconSamplePrefab;

	public void InItOtherCity (CityInfo tempInfo,int tempCurId)
	{
		M_CityInfo = tempInfo;

		M_Template = JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId);
		m_title.text = NameIdTemplate.GetName_By_NameId (M_Template.name);
		m_LevelIcon.spriteName = "1-" + M_Template.smaID;

		m_passObj.SetActive (tempInfo.cityState == 0 ? false : true);

		if (tempCurId > 0)
		{
			JCZCityTemplate curJczCityTemp = JCZCityTemplate.GetJCZCityTemplateById (tempCurId);
			if (M_Template.smaID > curJczCityTemp.smaID)
			{
				m_enterBtn.SetActive (false);
				m_enterNum.transform.localPosition = new Vector3(0,-115,0);
				m_enterNum.text = MyColorData.getColorString (5,"通过上一难度解锁");
				M_canJoin = false;
			}
			else
			{
				m_enterBtn.SetActive (true);
				m_enterNum.transform.localPosition = new Vector3(0,-90,0);
				if (M_Template.smaID == curJczCityTemp.smaID)
				{
					if (QXComData.AllianceInfo ().level >= M_Template.allianceLv)
					{
						M_canJoin = true;
						if (tempInfo.cityState2 == 0)
						{
							m_enterNum.text = "";
							m_btnLabel.text = "宣 战";
						}
						else
						{
							m_enterNum.text = MyColorData.getColorString (4,tempInfo.lmNum > 0 ? tempInfo.lmNum + "个盟友正在作战" : "");
							m_btnLabel.text = CityWarPage.m_instance.CityResp.interval == 1 ? "宣战成功" : "进入战场";
						}
					}
					else
					{
						m_enterNum.text = MyColorData.getColorString (5,"联盟" + M_Template.allianceLv + "级可宣战");
						m_btnLabel.text = "宣 战";
						M_canJoin = false;
					}
				}
				else
				{
					M_canJoin = true;
					if (tempInfo.cityState2 == 0)
					{
						m_enterNum.text = "";
						m_btnLabel.text = "宣  战";
					}
					else
					{
						m_enterNum.text = MyColorData.getColorString (4,tempInfo.lmNum > 0 ? tempInfo.lmNum + "个盟友正在作战" : "");
						m_btnLabel.text = "进入战场";
					}
				}
			}
		}
		else
		{
			m_enterBtn.SetActive (true);
			m_enterNum.transform.localPosition = new Vector3(0,-90,0);
			M_canJoin = true;
			if (tempInfo.cityState2 == 0)
			{
				m_enterNum.text = "";
				m_btnLabel.text = "宣  战";
			}
			else
			{
				m_enterNum.text = MyColorData.getColorString (4,tempInfo.lmNum > 0 ? tempInfo.lmNum + "个盟友正在作战" : "");
				m_btnLabel.text = "进入战场";
			}
		}

		if (m_iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
			                        IconSampleLoadCallBack);
		}
		else
		{
			InItIconSample ();
		}
	}

	private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		m_iconSamplePrefab = Instantiate (p_object) as GameObject;
		m_iconSamplePrefab.transform.parent = m_iconSampleParent.transform;

		InItIconSample ();
	}

	void InItIconSample ()
	{
		string[] rewardStr = M_Template.award.Split (':');
		
		m_iconSamplePrefab.transform.localPosition = new Vector3(0,-13,0);
		
		CommonItemTemplate commonTemp = CommonItemTemplate.getCommonItemTemplateById (int.Parse (rewardStr[1]));
		string mdesc = DescIdTemplate.GetDescriptionById (commonTemp.descId);
		string nameStr = NameIdTemplate.GetName_By_NameId (commonTemp.nameId);
		
		IconSampleManager iconSample = m_iconSamplePrefab.GetComponent<IconSampleManager> ();
		iconSample.SetIconByID (int.Parse (rewardStr[1]),"x" + rewardStr[2],2);
		//			iconSample.SetIconBasicDelegate (true,true,null);
		iconSample.SetIconPopText(int.Parse (rewardStr[1]), nameStr, mdesc, 1);
		
		m_iconSamplePrefab.transform.localScale = Vector3.one * 0.4f;
	}
}
