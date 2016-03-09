using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailFriendItem : MonoBehaviour {

	private FriendJunzhuInfo friendInfo;

	public UISprite headIcon;
	public UISprite nation;
	public UILabel vipLabel;
	public UILabel nameLabel;

	public GameObject selectObj;
	public GameObject sendObj;

	/// <summary>
	/// Ins it friend item.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItFriendItem (FriendJunzhuInfo tempInfo)
	{
		friendInfo = tempInfo;

		headIcon.spriteName = "PlayerIcon" + tempInfo.iconId;
		nation.spriteName = "nation_" + tempInfo.guojia;
		vipLabel.text = "VIP  " + tempInfo.vipLv;
		nameLabel.text = tempInfo.name + (string.IsNullOrEmpty (tempInfo.lianMengName) ? "\n无联盟" : "\n<" + tempInfo.lianMengName + ">");

		EventHandler handler = this.GetComponent<EventHandler> ();
		handler.m_click_handler -= FriendItemHandlerClickBack;
		handler.m_click_handler += FriendItemHandlerClickBack;
	}

	void FriendItemHandlerClickBack (GameObject obj)
	{
		NewEmailData.Instance().SendName = friendInfo.name;
		SendMail send = sendObj.GetComponent<SendMail> ();
		send.GetReplyName ();

		EmailSelectFriend.selectFriend.CloseHandlerClickBack (gameObject);
	}

	/// <summary>
	/// Refreshs the state of the select.
	/// </summary>
	/// <param name="tempName">Temp name.</param>
	public void RefreshSelectState (string tempName)
	{
		selectObj.SetActive (friendInfo.name == tempName ? true : false);
	}
}
