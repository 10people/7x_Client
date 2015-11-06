using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailAwardItem : MonoBehaviour {

	public UISprite icon;

	public UILabel numLabel;

	private int commonId;

	public UIAtlas equipAtlas;

	public UIAtlas fuWenAtlas;

	//获得邮件奖励信息
	public void GetEmailAwardInfo (EmailGoods awardInfo)
	{
		Debug.Log ("物品类型：" + awardInfo.type);
		Debug.Log ("物品id：" + awardInfo.id);
//		Debug.Log ("物品数量：" + awardInfo.count);

//		CommonItemTemplate commonItemTemp = CommonItemTemplate.getCommonItemTemplateById (awardInfo.id);

		icon.atlas = awardInfo.type == 7 ? fuWenAtlas : equipAtlas;

		if (awardInfo.type == 7)
		{
			icon.SetDimensions (60,60);
		}
		else
		{
			icon.SetDimensions (40,40);
		}

		commonId = awardInfo.id;

		icon.spriteName = awardInfo.id.ToString ();

		numLabel.text = "x" + awardInfo.count.ToString ();

		this.GetComponent<NGUILongPress> ().OnLongPress += ActiveTips;
//		this.GetComponent<NGUILongPress> ().OnLongPressFinish += DoActiveTips;
	}

	void ActiveTips (GameObject go)
	{
		ShowTip.showTip(commonId);
	}
	
	void DoActiveTips (GameObject go)
	{
		ShowTip.close ();
	}
}
