using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CWBidRecordItem : MonoBehaviour {

	public UILabel m_allianceName;
	public UILabel m_huFuNum;
	public UILabel m_time;
	public GameObject m_attack;

	public void InItRecordItem (BidRecord tempRecord,string tempName)
	{
		string colorStr = QXComData.AllianceInfo ().name == tempRecord.allianceName ? "[10ff2b]" : "[ffffff]";

		m_allianceName.text = colorStr + "<" + tempRecord.allianceName + ">[-]";
		m_huFuNum.text = colorStr + tempRecord.huFuNum.ToString () + "[-]";
		m_time.text = colorStr + QXComData.UTCToTimeString (tempRecord.time * 1000,"HH:mm") + "[-]";

		m_attack.SetActive (CityWarPage.m_instance.CityResp.interval != 0 ? (tempRecord.allianceName == tempName ? true : false) : false);
	}
}
