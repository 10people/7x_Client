using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailItemInfo : MonoBehaviour {

	public EmailInfo m_emailInfo;
	
	public UISprite bgSprite;
	
	public UISprite systemIcon;
	public UISprite letterIcon;
	
	public UILabel titleLabel;

	public GameObject senderLabelObj;
	public UILabel sender;
	
	public UILabel timeLabel;
	
	public GameObject awardIcon;
	
	//获得emailitem信息
	public void GetEmailItemInfo (EmailInfo emailInfo)
	{
		m_emailInfo = emailInfo;
	
		if (emailInfo.type == 80000)//玩家邮件
		{
			titleLabel.gameObject.SetActive (false);

			systemIcon.gameObject.SetActive (false);
			letterIcon.gameObject.SetActive (true);

			letterIcon.spriteName = "PlayerIcon" + emailInfo.roleId;

			senderLabelObj.transform.localPosition = new Vector3(-270,15,0);
			timeLabel.transform.localPosition = new Vector3(-270,-15,0);
		}
	
		else
		{
			titleLabel.text = emailInfo.title;
//			Debug.Log ("emailInfo.type" + emailInfo.type);
			EmailTemp emailTemp = EmailTemp.getEmailTempByType (emailInfo.type);
			int whichType = emailTemp.whichType;

//			Debug.Log ("whichType:" + whichType);

			systemIcon.gameObject.SetActive (true);
			letterIcon.gameObject.SetActive (false);

			IconShow (whichType);

			if (emailInfo.goodsList != null)
			{
				awardIcon.SetActive (true);
			}
			else
			{
				awardIcon.SetActive (false);
			}
		}
		
		sender.text = emailInfo.senderName;

		timeLabel.text = UTCToTimeString (emailInfo.time,"yyyy-MM-dd");

//		Debug.Log ("ISREAD:" + emailInfo.isRead);
		if (emailInfo.isRead == 0)//未读
		{
			bgSprite.spriteName = "bg2";
		}

		else if (emailInfo.isRead == 1)//已读
		{
			bgSprite.spriteName = "backGround_Common_big";
		}
	}

	private string UTCToTimeString(long time, string format)
	{
		long lTime = time * 10000;
		
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

		TimeSpan toNow = new TimeSpan(lTime);

		DateTime dtResult = dtStart.Add(toNow);

		// "yyyy-MM-dd HH:mm:ss"
		return dtResult.ToString(format);
	}

	//点击查看邮件
	public void CheckEmail ()
	{
		Debug.Log ("EmailManager.emailMan.IsCheck:" + EmailManager.emailMan.IsCheck);
		if (EmailManager.emailMan.IsCheck)
		{
			return;
		}
		else
		{
			EmailManager.emailMan.IsCheck = true;
		}

		if (m_emailInfo.isRead == 0)//如果未读，点击发送读取状态
		{
			ReadEmail readReq = new ReadEmail ();
			
			readReq.id = m_emailInfo.id;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize (t_stream,readReq);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_READED_EAMIL,ref t_protof,"25014");
//			Debug.Log ("CheckEmail:" + ProtoIndexes.C_READED_EAMIL);
		}

		else if (m_emailInfo.isRead == 1)
		{
			EmailManager.emailMan.CreateCheckEmailObj (m_emailInfo);
		}
	}

	//1-玩家邮件 2-房屋 3-联盟 4-荒野 5-百战 6-国战 7-运镖 8-运营
	void IconShow (int type)
	{
		switch (type)
		{
		case 2:

			systemIcon.spriteName = "home";

			break;

		case 3:

			systemIcon.spriteName = "alliance";

			break;

		case 4:

			systemIcon.spriteName = "huangye";

			break;

		case 5:

			systemIcon.spriteName = "baizhan";

			break;

		case 6:

			systemIcon.spriteName = "guozhan";

			break;

		case 7:

			systemIcon.spriteName = "yunbiao";

			break;

		case 8:
			
			systemIcon.spriteName = "yunying";
			
			break;

		default:break;
		}
	}
}