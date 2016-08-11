using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailPage : GeneralInstance<EmailPage> {

	public enum EmailShowPage
	{
		EMAIL_MAINPGE = 0,//首页
		EMAIL_CHECK = 1,//查看邮件
		EMAIL_SEND = 2,//发送邮件
	}
	private EmailShowPage showPage = EmailShowPage.EMAIL_MAINPGE;
	public List<GameObject> emailPageList = new List<GameObject> ();//0-mainPage 1-checkPage 2-writePage
	private Dictionary<EmailShowPage,GameObject> pageDic = new Dictionary<EmailShowPage, GameObject>();

	private NewEmailData.EmailOpenType emailOpenType;

	public enum EmailShowType
	{
		SYSTEM,
		PRIVATE,
		WRITE,
	}
	private EmailShowType ShowType = EmailShowType.SYSTEM;
	private EmailShowType m_lastShowType = EmailShowType.SYSTEM;

	private List<EmailInfo> systemList = new List<EmailInfo> ();//系统邮件列表
	private List<EmailInfo> privateList = new List<EmailInfo> ();//私信列表

	public UIScrollView emailSc;
	public UIScrollBar emailSb;
	private float emailSbValue;

	public GameObject emailItemObj;
	private List<GameObject> emailItemList = new List<GameObject> ();

	public MyPageManager m_pageManager;

	public UILabel desLabel;

	public ScaleEffectController m_ScaleEffectController;

	public GameObject anchorTopRight;
	public GameObject m_anchorTL;

	new void Awake ()
	{
		base.Awake ();
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}

	void Start ()
	{
		QXComData.LoadTitleObj (m_anchorTL,"邮箱");
		QXComData.LoadYuanBaoInfo (anchorTopRight);
	}

	/// <summary>
	/// Gets the email resp.
	/// </summary>
	public void GetEmailResp (NewEmailData.EmailOpenType tempOpenType,List<EmailInfo> tempSystemList,List<EmailInfo> tempPrivateList)
	{
		m_ScaleEffectController.OnOpenWindowClick ();

		systemList = tempSystemList;
		privateList = tempPrivateList;

		emailOpenType = tempOpenType;

		emailSbValue = 0;

		if (pageDic.Count == 0)
		{
			for (int i = 0;i < emailPageList.Count;i ++)
			{
				pageDic.Add ((EmailShowPage)Enum.ToObject(typeof(EmailShowPage),i),emailPageList[i]);
			}
		}

		ShowEmailPage (tempOpenType == NewEmailData.EmailOpenType.EMAIL_MAIN_PAGE ? 
		               EmailShowPage.EMAIL_MAINPGE : EmailShowPage.EMAIL_SEND);

		if (tempOpenType == NewEmailData.EmailOpenType.EMAIL_MAIN_PAGE)
		{
			SwitchEmailType (ShowType);
			CheckUnSendEmail ();
		}
	}

	/// <summary>
	/// Shows the email page.
	/// </summary>
	/// <param name="tempPage">Temp page.</param>
	public void ShowEmailPage (EmailShowPage tempPage)
	{
		showPage = tempPage;
		foreach (KeyValuePair<EmailShowPage,GameObject> pair in pageDic)
		{
			pair.Value.SetActive (pair.Key == tempPage ? true : false);
		}

		if (tempPage == EmailShowPage.EMAIL_SEND)
		{
			SendMail send = emailPageList[2].GetComponent<SendMail> ();
			send.GetReplyName ();
		}
	}

	/// <summary>
	/// Switchs the type of the email.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void SwitchEmailType (EmailShowType tempType)
	{
		switch (tempType)
		{
		case EmailShowType.SYSTEM:

			desLabel.text = systemList.Count == 0 ? "您的邮箱暂时没有系统邮件！" : "";
			InItEmailList (systemList);
			m_pageManager.setAnimation (0);
			m_pageManager.setPageIndex (0);

			break;
		case EmailShowType.PRIVATE:

			desLabel.text = privateList.Count == 0 ? "暂时没有玩家给您发送邮件！" : "";
			InItEmailList (privateList);
			m_pageManager.setAnimation (1);
			m_pageManager.setPageIndex (1);

			break;
		case EmailShowType.WRITE:

			m_pageManager.setAnimation (2);
			m_pageManager.setPageIndex (2);

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Ins it email list.
	/// </summary>
	/// <param name="tempList">Temp list.</param>
	void InItEmailList (List<EmailInfo> tempList)
	{
		emailItemList = QXComData.CreateGameObjectList (emailItemObj,tempList.Count,emailItemList);

		for (int i = 0;i < tempList.Count - 1;i ++)
		{
			for (int j = 0;j < tempList.Count - i - 1;j ++)
			{
				if (tempList[j].isRead != tempList[j + 1].isRead)
				{
					if (tempList[j].isRead == 1)
					{
						EmailInfo tempEmailInfo = tempList[j];
						tempList[j] = tempList[j + 1];
						tempList[j + 1] = tempEmailInfo;
					}
				}
				else
				{
					if (tempList[j].time < tempList[j + 1].time)
					{
						EmailInfo tempEmailInfo = tempList[j];
						tempList[j] = tempList[j + 1];
						tempList[j + 1] = tempEmailInfo;
					}
				}
			}
		}

		emailSb.value = tempList.Count > 4 ? emailSbValue : 0;

		for (int i = 0;i < tempList.Count;i ++)
		{
			emailItemList[i].transform.localPosition = new Vector3(0,-120 * i,0);
			emailSc.UpdateScrollbars (true);
//			emailSc.UpdatePosition ();
			EmailItem email = emailItemList[i].GetComponent<EmailItem> ();
			email.GetEmailItemInfo (tempList[i]);
		}

		emailSc.enabled = tempList.Count > 4 ? true : false;
		emailSb.gameObject.SetActive (tempList.Count > 4 ? true : false);

//		UIYindao.m_UIYindao.CloseUI ();
	}

	public override void MYClick (GameObject ui)
	{
		MyPageButtonManager pageBtn = ui.GetComponentInParent<MyPageButtonManager> ();
		if (pageBtn != null)
		{
			int index = int.Parse (pageBtn.name.Substring (4,1));
			Debug.Log ("index:" + index);
			switch (index)
			{
			case 0:
				if (ShowType != EmailShowType.SYSTEM)
				{
					ShowType = EmailShowType.SYSTEM;
					SwitchEmailType (EmailShowType.SYSTEM);
					if (NewEmailData.Instance().SendEmailType == NewEmailData.SendType.DEFAULT)
					{
						GetSendEmailInfo ();
						ShowEmailPage (EmailShowPage.EMAIL_MAINPGE);
					}
				}
				break;
			case 1:
				if (ShowType != EmailShowType.PRIVATE)
				{
					ShowType = EmailShowType.PRIVATE;
					SwitchEmailType (EmailShowType.PRIVATE);
					if (NewEmailData.Instance().SendEmailType == NewEmailData.SendType.DEFAULT)
					{
						GetSendEmailInfo ();
						ShowEmailPage (EmailShowPage.EMAIL_MAINPGE);
					}
				}
				break;
			case 2:
				if (ShowType != EmailShowType.WRITE)
				{
					m_lastShowType = ShowType;
					ShowType = EmailShowType.WRITE;
					SwitchEmailType (EmailShowType.WRITE);
					NewEmailData.Instance().SendEmailType = NewEmailData.SendType.DEFAULT;
					ShowEmailPage (EmailShowPage.EMAIL_SEND);
				}
				break;
			default:
				break;
			}
		}

		EmailItem emailItem = ui.GetComponent <EmailItem> ();
		if (emailItem != null)
		{
			switch (emailItem.EmailItemInfo.isRead)
			{
			case 0:
				
				NewEmailData.Instance().ReadEmail (emailItem.EmailItemInfo.id,emailItem.EmailItemInfo.type == 80000 ? 
				                                   EmailShowType.PRIVATE : EmailShowType.SYSTEM);
				
				break;
			case 1:
				
				OpenCheckEmail (emailItem.EmailItemInfo);
				
				break;
			default:
				break;
			}
		}

		if (ui.name == "CloseBtn")
		{
			if (emailOpenType == NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE)
			{
				m_ScaleEffectController.OnCloseWindowClick ();
				m_ScaleEffectController.CloseCompleteDelegate += CloseEmail;
			}
			else
			{
				if (showPage == EmailShowPage.EMAIL_MAINPGE)
				{
					m_ScaleEffectController.OnCloseWindowClick ();
					m_ScaleEffectController.CloseCompleteDelegate += CloseEmail;
				}
				else
				{
					BackBtnCallBack ();
				}
			}
		}
	}

	public void BackBtnCallBack ()
	{
		if (NewEmailData.Instance().SendEmailType == NewEmailData.SendType.DEFAULT)
		{
			GetSendEmailInfo ();
			ShowEmailPage (EmailShowPage.EMAIL_MAINPGE);
			SwitchEmailType (m_lastShowType);
			CheckUnSendEmail ();
		}
		else
		{
			ShowEmailPage (EmailShowPage.EMAIL_CHECK);
			NewEmailData.Instance().SendEmailType = NewEmailData.SendType.DEFAULT;
		}
	}

	/// <summary>
	/// Opens the check email.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void OpenCheckEmail (EmailInfo tempInfo)
	{
//		emailSbValue = emailSb.value;
		//打开查看邮件
		ShowEmailPage (EmailShowPage.EMAIL_CHECK);

		EmailCheck check = emailPageList[1].GetComponent<EmailCheck> ();
		check.GetEmailInfo (tempInfo);
	}

	/// <summary>
	/// Refreshs the email list.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void RefreshEmailList (EmailShowType tempType,List<EmailInfo> tempList)
	{
		switch (tempType)
		{
		case EmailShowType.SYSTEM:

			systemList = tempList;
			desLabel.text = systemList.Count == 0 ? "您的邮箱暂时没有系统邮件！" : "";

			break;
		case EmailShowType.PRIVATE:

			privateList = tempList;
			desLabel.text = privateList.Count == 0 ? "暂时没有玩家给您发送邮件！" : "";

			break;
		default:
			break;
		}
		CheckUnSendEmail ();
//		Debug.Log ("showPage:" + showPage);
		if (showPage == EmailShowPage.EMAIL_MAINPGE)
		{
			int emailCount = ShowType == EmailShowType.PRIVATE ? privateList.Count : systemList.Count;

//			Debug.Log ("emailSbValue:" + emailSbValue);
//			Debug.Log ("emailSb.value:" + emailSb.value);
//			emailSbValue = emailCount > 4 ? emailSb.value : 0;

			InItEmailList (ShowType == EmailShowType.SYSTEM ? systemList : privateList);
		}
	}
	
	///刷新屏蔽按钮
	public enum RefreshType
	{
		ADD_FRIEND,
		SHIELD_FRIEND,
	}
	public void RefreshEmailCheck (RefreshType tempType)
	{
		EmailCheck check = emailPageList[1].GetComponent<EmailCheck> ();
		switch (tempType)
		{
		case RefreshType.ADD_FRIEND:
			check.IsAddFriend (true);
			break;
		case RefreshType.SHIELD_FRIEND:
			check.IsJoinBlackList (true);
			break;
		default:
			break;
		}
	}

	//刷新发送按钮状态
	public void RefreshEmailSendBtnState (int tempState)
	{
		SendMail send = emailPageList [2].GetComponent<SendMail> ();
		send.RefreshBtnState (tempState);
	}

	//获得发送邮件内容
	public void GetSendEmailInfo ()
	{
		SendMail send = emailPageList [2].GetComponent<SendMail> ();
		NewEmailData.Instance().SendName = send.nameLabel.value;
		NewEmailData.Instance().SendContent = send.contentLabel.value;
//		Debug.Log ("SendName:" + NewEmailData.Instance().SendName + "//SendContent:" + NewEmailData.Instance().SendContent);
	}

	public void CloseEmail ()
	{
		emailSbValue = 0;
		emailSb.value = emailSbValue;
		ShowType = EmailShowType.SYSTEM;
		GetSendEmailInfo ();
//		NewEmailData.Instance().ExistNewEmail ();
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Checks the un send email.
	/// </summary>
	public void CheckUnSendEmail ()
	{
		if (systemList.Count == 0)
		{
//			redObjList[0].SetActive (false);
			m_pageManager.m_listMyPageButtonManager[0].m_spriteRed.gameObject.SetActive (false);
			PushAndNotificationHelper.SetRedSpotNotification (41,false);
		}
		else
		{
			foreach (EmailInfo email in systemList)
			{
				if (email.isRead == 0)
				{
//					redObjList[0].SetActive (true);
					m_pageManager.m_listMyPageButtonManager[0].m_spriteRed.gameObject.SetActive (true);
					PushAndNotificationHelper.SetRedSpotNotification (41,true);
					break;
				}
				else
				{
//					redObjList[0].SetActive (false);
					m_pageManager.m_listMyPageButtonManager[0].m_spriteRed.gameObject.SetActive (false);
					PushAndNotificationHelper.SetRedSpotNotification (41,false);
				}
			}
		}

		if (privateList.Count == 0)
		{
//			redObjList[1].SetActive (false);
			m_pageManager.m_listMyPageButtonManager[1].m_spriteRed.gameObject.SetActive (false);
			PushAndNotificationHelper.SetRedSpotNotification (10,false);
		}
		else
		{
			foreach (EmailInfo email in privateList)
			{
				if (email.isRead == 0)
				{
//					redObjList[1].SetActive (true);
					m_pageManager.m_listMyPageButtonManager[1].m_spriteRed.gameObject.SetActive (true);
					PushAndNotificationHelper.SetRedSpotNotification (10,true);
					break;
				}
				else
				{
//					redObjList[1].SetActive (false);
					m_pageManager.m_listMyPageButtonManager[1].m_spriteRed.gameObject.SetActive (false);
					PushAndNotificationHelper.SetRedSpotNotification (10,false);
				}
			}
		}

		bool isUnSend = string.IsNullOrEmpty (NewEmailData.Instance ().SendName) && string.IsNullOrEmpty (NewEmailData.Instance ().SendContent) ? false : true;
//		redObjList[2].SetActive (isUnSend);
//		PushAndNotificationHelper.SetRedSpotNotification (20,isUnSend);
//		redObjList[2].SetActive (false);
		m_pageManager.m_listMyPageButtonManager[2].m_spriteRed.gameObject.SetActive (false);
		PushAndNotificationHelper.SetRedSpotNotification (20,false);
	}
}
