using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailItem : MonoBehaviour {

	private EmailInfo m_emailInfo;
	public EmailInfo EmailItemInfo
	{
		get{return m_emailInfo;}
	}

	public UISprite bgSprite;
	public UISprite emailIcon;

	public UIAtlas playerAtlas;
	public UIAtlas emailAtals;

	public UILabel titleLabel;
	public UILabel senderLabel;
	public UILabel timeLabel;
	
	public GameObject awardIcon;

	//1-玩家邮件 2-房屋 3-联盟 4-荒野 5-百战 6-国战 7-运镖 8-运营
	private string[] iconLength = new string[]{"home","alliance","huangye","baizhan","guozhan","yunbiao","yunying"};
	
	//获得emailitem信息
	public void GetEmailItemInfo (EmailInfo emailInfo)
	{
		m_emailInfo = emailInfo;

		emailIcon.atlas = emailInfo.type == 80000 ? playerAtlas : emailAtals;

		if (emailInfo.type == 80000)//玩家邮件
		{
			titleLabel.gameObject.SetActive (false);
			
			emailIcon.spriteName = "PlayerIcon" + emailInfo.roleId;
			
			senderLabel.transform.localPosition = new Vector3(-270,15,0);
			timeLabel.transform.localPosition = new Vector3(-270,-15,0);

			awardIcon.SetActive (false);
		}
		
		else
		{
			titleLabel.text = emailInfo.title;
			//			Debug.Log ("emailInfo.type" + emailInfo.type);
			EmailTemp emailTemp = EmailTemp.getEmailTempByType (emailInfo.type);
			int whichType = emailTemp.whichType;
			
			emailIcon.spriteName = iconLength[whichType - 2];
			
			if (emailInfo.goodsList != null)
			{
				awardIcon.SetActive (true);
			}
			else
			{
				awardIcon.SetActive (false);
			}
		}
		
		senderLabel.text = "发件人:" + emailInfo.senderName;
		
		timeLabel.text = QXComData.UTCToTimeString (emailInfo.time,"yyyy-MM-dd");
		
		//		Debug.Log ("ISREAD:" + emailInfo.isRead);
		bgSprite.spriteName = emailInfo.isRead == 0 ? "bg2" : "backGround_Common_big";
	}
}
