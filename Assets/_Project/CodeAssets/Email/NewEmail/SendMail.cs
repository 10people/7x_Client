using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SendMail : MonoBehaviour {

	public List<EventHandler> sendEmailBtnList = new List<EventHandler> ();
	private Dictionary<string,EventHandler> btnDic = new Dictionary<string, EventHandler> ();

	public UIInput nameLabel;
	public UIInput contentLabel;

	public UILabel m_content;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;
	
	public UILabel btnLabel;

	private string textStr;

	public GameObject selectFriendWindow;

	private bool m_isDefault = false;

	/// <summary>
	/// Gets the name of the reply.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempName">Temp name.</param>
	public void GetReplyName ()
	{
		nameLabel.value = "";
		contentLabel.value = "";

		RefreshBtnState (1);
		foreach (EventHandler handler in sendEmailBtnList)
		{
			handler.m_click_handler -= BtnHandlerCallBack;
			handler.m_click_handler += BtnHandlerCallBack;
			if (btnDic.Count < sendEmailBtnList.Count)
			{
				btnDic.Add (handler.name,handler);
			}
		}

		btnDic["Block"].GetComponent<UISprite> ().alpha = 0.1f;

		m_isDefault = NewEmailData.Instance().SendEmailType == NewEmailData.SendType.DEFAULT ? true : false;

		nameLabel.value = NewEmailData.Instance().SendName;
		contentLabel.value = m_isDefault ? NewEmailData.Instance().SendContent : "";
		SetContent ();

		nameLabel.GetComponent<BoxCollider> ().enabled = m_isDefault;
	}

	void SetContent ()
	{
		if (m_content.height <= 232)
		{
			m_sc.ResetPosition ();
		}

		m_sc.UpdateScrollbars (true);
		m_sc.enabled = m_content.height > 232 ? true : false;
		m_sb.gameObject.SetActive (m_content.height > 232 ? true : false);
	}

	void BtnHandlerCallBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "InputName":
			btnDic["Block"].gameObject.SetActive (true);
//			#if UNITY_EDITOR
//			Debug.Log ("Unity");
//			#else
//			Debug.Log ("Android or Ios");
//			btnDic["Block"].gameObject.SetActive (true);
//			#endif

			break;
		case "InputContent":
			btnDic["Block"].gameObject.SetActive (true);
			break;
		case "SendBtn":

			EmailSendReq ();

			break;
		case "Block":

			NameSubmit ();
			ContentSubmit ();

			break;
		case "SelectFriendBtn":

			selectFriendWindow.SetActive (true);
			EmailSelectFriend.selectFriend.InItSelectFriendPage (nameLabel.value);

			break;
		default:
			break;
		}
	}

	//提交名字
	public void NameSubmit ()
	{
		btnDic["Block"].gameObject.SetActive (false);
		nameLabel.value = QXComData.TextLengthLimit (QXComData.StrLimitType.CREATE_ROLE_NAME,nameLabel.value);
	}

	public void ContentSubmit ()
	{
		btnDic["Block"].gameObject.SetActive (false);
		SetContent ();
	}

	/// <summary>
	/// Emails the send req.
	/// </summary>
	void EmailSendReq ()
	{
		SendEmail sendReq = new SendEmail ();
		
		sendReq.receiverName = nameLabel.text;
		sendReq.content = contentLabel.text;
		
		QXComData.SendQxProtoMessage (sendReq,ProtoIndexes.C_SEND_EAMIL,ProtoIndexes.S_SEND_EAMIL.ToString ());
//		Debug.Log ("SendEmail:" + ProtoIndexes.C_SEND_EAMIL);

		RefreshBtnState (2);
	}

	/// <summary>
	/// Refreshs the state of the button.
	/// </summary>
	/// <param name="tempState">Temp state.</param>
	public void RefreshBtnState (int tempState)
	{
		string str = "";
		switch (tempState)
		{
		case 1:
			str = "发  送";
			break;
		case 2:
			str = "";
			break;
		case 3:
			str = "已发送";
			nameLabel.value = "";
			contentLabel.value = "";
			break;
		default:
			break;
		}
		btnLabel.text = str;
	}
}
