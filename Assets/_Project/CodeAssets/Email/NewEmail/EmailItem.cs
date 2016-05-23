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

	//1-玩家邮件 2-房屋 3-联盟 4-荒野 5-百战 6-国战 7-运镖 8-运营 9-掠夺
	private string[] iconLength = new string[]{"home","alliance","huangye","baizhan","guozhan","yunbiao","yunying","lueduo"};
	
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
			titleLabel.text = "[000000]" + emailInfo.title + "[-]";
			//			Debug.Log ("emailInfo.type" + emailInfo.type);
			EmailTemp emailTemp = EmailTemp.getEmailTempByType (emailInfo.type);
			int whichType = emailTemp.whichType;
//			Debug.Log ("whichType:" + whichType);
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
		
		senderLabel.text = "[000000]发件人:" + emailInfo.senderName + "[-]";
		
		timeLabel.text = MyColorData.getColorString (5,QXComData.UTCToTimeString (emailInfo.time,"yyyy-MM-dd hh:ss"));

		bgSprite.color = emailInfo.isRead == 0 ? QXComData.lightColor : Color.white;
	}
}
