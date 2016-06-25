using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CWRewardItem : MonoBehaviour {

	[HideInInspector]public CityWarRewardInfo M_RewardInfo;

	public UILabel m_cityName;
	public UILabel m_cityLevel;
	public UILabel m_state;
	public UILabel m_result;
	public UILabel m_gongXun;
	public UILabel m_time;
	public UILabel m_haveGet;

	public GameObject m_getBtn;

	public void InItReward (CityWarData.CW_RewardType tempType,CityWarRewardInfo tempInfo)
	{
		M_RewardInfo = tempInfo;

		JCZCityTemplate jczCityTemp = JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId);

		m_cityName.text = NameIdTemplate.GetName_By_NameId (jczCityTemp.name);

		m_cityLevel.text = "Lv." + jczCityTemp.allianceLv;

		m_state.text = CityWarReward.m_instance.M_RewardDic[tempInfo.warType][1];
//		Debug.Log ("tempInfo.warType:" + tempInfo.warType);
//		Debug.Log ("tempInfo.result:" + tempInfo.result);
		m_result.text = tempInfo.warType == 2 ? "" : (tempType == CityWarData.CW_RewardType.ALLIANCE ? CityWarReward.m_instance.M_RewardDic [tempInfo.result][0] : ("[d80202]杀敌" + tempInfo.result + "[-]"));

		m_gongXun.text = "x" + tempInfo.rewardNum;
//		Debug.Log ("tempInfo.time:" + tempInfo.time);
		m_time.text = CityWarReward.m_instance.M_RewardDic[tempInfo.time][2];

		m_getBtn.SetActive (tempInfo.getState == 0 ? true : false);
		m_haveGet.text = tempInfo.getState == 0 ? "" : "[10ff2b]已领[-]";
	}
}
