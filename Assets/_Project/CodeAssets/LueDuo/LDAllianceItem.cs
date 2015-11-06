using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDAllianceItem : MonoBehaviour {

	private LianMengInfo allianceInfo;

	public UISprite rankSprite;

	public UILabel rankLabel;

	public UILabel nameLabel;
	public UILabel m_alliance;

	public UILabel shengWangLabel;

	public GameObject ldAllianceManObj;

	public GameObject selectBox;

	private bool isShow;
	public bool SetBoxShow
	{
		set{isShow = value;}
	}

	public void GetLdAllianceInfo (LianMengInfo tempInfo)
	{
//		Debug.Log ("排名：" + tempInfo.rank + "，" + tempInfo.mengName);

		allianceInfo = tempInfo;

		if (tempInfo.rank > 3)
		{
			rankSprite.spriteName = "";
			rankLabel.text = tempInfo.rank.ToString ();
		}
		else
		{
			rankSprite.spriteName = "rank" + tempInfo.rank.ToString ();
		}
//		rankLabel.text = tempInfo.rank.ToString ();

		nameLabel.transform.localPosition = JunZhuData.Instance().m_junzhuInfo.lianMengId == tempInfo.mengId ? new Vector3(-20,10,0) : new Vector3(-20,0,0);
		m_alliance.text = JunZhuData.Instance().m_junzhuInfo.lianMengId == tempInfo.mengId ? "我的联盟" : "";

		nameLabel.text = "<" + tempInfo.mengName + ">";

		shengWangLabel.text = tempInfo.shengWang.ToString ();
	}

	void OnClick ()
	{
		if (!isShow)
		{
			if (!LueDuoData.Instance.IsStop)
			{
				LueDuoData.Instance.IsStop = true;
				
				LDAllianceManager ldAlliance = ldAllianceManObj.GetComponent<LDAllianceManager> ();
				ldAlliance.ShowSelectBox (allianceInfo.mengId);
				
				LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.JunZhu,
				                                   allianceInfo.guoJiaId,
				                                   allianceInfo.mengId,
				                                   LueDuoData.Instance.GetAllianceStartPage,
				                                   LueDuoData.Direction.Default);
			}
		}
	}
}
