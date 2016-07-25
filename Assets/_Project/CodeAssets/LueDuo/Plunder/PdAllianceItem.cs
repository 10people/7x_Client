using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PdAllianceItem : MonoBehaviour {

	private LianMengInfo allianceInfo;
	
	public UISprite rankSprite;
	public UILabel rankLabel;
	public UILabel nameLabel;
	public UILabel shengWangLabel;
	public UISprite m_bgSprite;
	
	public GameObject selectBox;

	/// <summary>
	/// Ins it alliance item.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItAllianceItem (LianMengInfo tempInfo)
	{
		//Debug.Log ("排名：" + tempInfo.rank + "，" + tempInfo.mengName);
		allianceInfo = tempInfo;
		selectBox.SetActive (false);
		rankSprite.spriteName = tempInfo.rank > 3 ? "" : "rank" + tempInfo.rank.ToString ();
		rankLabel.text = tempInfo.rank > 3 ? tempInfo.rank.ToString () : "";
	
		nameLabel.text = JunZhuData.Instance().m_junzhuInfo.lianMengId == tempInfo.mengId ? 
			"<" + tempInfo.mengName + ">\n我的联盟" : "<" + tempInfo.mengName + ">";
		
		shengWangLabel.text = tempInfo.shengWang.ToString ();
	}

	/// <summary>
	/// Shows the select.
	/// </summary>
	/// <param name="isShow">If set to <c>true</c> is show.</param>
	public void ShowSelect (bool isShow)
	{
//		selectBox.SetActive (isShow);
		QXComData.SetBgSprite2 (m_bgSprite,isShow);
	}
}
