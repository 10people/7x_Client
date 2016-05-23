using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RechargeItem : MonoBehaviour {

	public ChongTimes M_ChongTimes;

	public ChongZhiTemplate M_RechargeTemp;

	public UISprite m_icon;

	public UILabel m_numDes;
	public UILabel m_cost;
	public UILabel m_desLabel;

	public GameObject m_recObj;

	public void InItRechargeItem (ChongTimes tempTimes)
	{
		M_ChongTimes = tempTimes;

		M_RechargeTemp = ChongZhiTemplate.GetChongZhiTempById (tempTimes.id);
		m_numDes.text = "[b]" + M_RechargeTemp.name + "[/b]";
		m_cost.text = "[eac102][b]" + M_RechargeTemp.needNum.ToString () + "元[/b][-]";

		m_icon.spriteName = tempTimes.id.ToString ();

		m_recObj.SetActive(M_RechargeTemp.type == 1 && tempTimes.times == 0 ? true : false);

		m_desLabel.text = "[b]" + (tempTimes.times > 0 ? (M_RechargeTemp.extraYuanbao > 0 ? "[eac102]另赠" + M_RechargeTemp.extraYuanbao + "元宝[-]" : "") : "[e80000]" + M_RechargeTemp.desc + "[-]") + "[/b]";
	}
}
