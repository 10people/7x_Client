using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CWBrandItem : MonoBehaviour {

	public UISprite m_nation1;
	public UISprite m_nation2;

	public UILabel m_allianceName1;
	public UILabel m_allianceName2;

	public UILabel m_city;
	public UILabel m_time;

	public void InItBrandItem (CityWarGrandInfo tempInfo)
	{
		m_nation1.spriteName = QXComData.GetNationSpriteName (tempInfo.nationId1);
		m_nation2.spriteName = QXComData.GetNationSpriteName (tempInfo.nationId2);



		m_allianceName1.text = (tempInfo.allianceName1 == QXComData.AllianceInfo ().name ? "[10ff2b]" : "[00e1c4]") + tempInfo.allianceName1 + "[-]";
		m_allianceName2.text = (tempInfo.allianceName2 == QXComData.AllianceInfo ().name ? "[10ff2b]" : "[00e1c4]") + tempInfo.allianceName2 + "[-]的";

		m_city.text = NameIdTemplate.GetName_By_NameId (JCZCityTemplate.GetJCZCityTemplateById (tempInfo.cityId).name);

		int countTime = QXComData.ConvertDateTimeInt (System.DateTime.Now) - (int)tempInfo.time;
		int threeDay = 259200;
		int twoDay = 172800;
		int oneDay = 86400;

		if (countTime < oneDay)
		{
			m_time.text = "今天";
		}
		else if (countTime < twoDay)
		{
			m_time.text = "昨天";
		}
		else if (countTime < threeDay)
		{
			m_time.text = "前天";
		}
		else
		{
			m_time.text = QXComData.UTCToTimeString (tempInfo.time * 1000,"MM-dd");
		}
	}
}
