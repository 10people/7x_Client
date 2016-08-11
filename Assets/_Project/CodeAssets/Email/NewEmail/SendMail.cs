using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SendMail : GeneralInstance<SendMail> {

	public UIInput nameLabel;
	public UIInput contentLabel;

	public UILabel m_content;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;
	
	public UILabel btnLabel;

	private string textStr;

	public GameObject m_block;

	public GameObject selectFriendWindow;

	private bool m_isDefault = false;

	new void Awake ()
	{
		base.Awake ();
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}

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

		m_block.GetComponent<UISprite> ().alpha = 0.1f;

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

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "InputName":
			m_block.SetActive (true);
//			#if UNITY_EDITOR
//			Debug.Log ("Unity");
//			#else
//			Debug.Log ("Android or Ios");
//			btnDic["Block"].gameObject.SetActive (true);
//			#endif

			break;
		case "InputContent":
			m_block.SetActive (true);
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
		m_block.SetActive (false);
		nameLabel.value = QXComData.TextLengthLimit (QXComData.StrLimitType.CREATE_ROLE_NAME,nameLabel.value);
	}

	public void ContentSubmit ()
	{
		m_block.SetActive (false);
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
