//#define StartTest
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChatPlayerItem : MonoBehaviour {

	private ChatMessage chatMsg;

	public UISprite headIcon;
	public UISprite vipIcon;
	public UILabel nameLabel;
	public UISprite textBg;//-60,15,0  -60,5,0
	public UILabel textLabel;

	public UILabel titleLabel;

	private const int minTextBgHeigh = 34;
	private const int dis = 20;

	private const int bgDis = 37;
	private const int textLabelMixWidth = 144;

	public EventHandler playerHandler;

	private bool isPlayer;

	private int itemHeigh;

	void Start ()
	{
		#if StartTest
		textLabel.text = "花费的烦得很地花费的烦得很地花费的烦得很地花费的烦得很地花费的烦得很地花费的烦得很地花费的烦得很地花费的烦得很地";
		
		int row = textLabel.height / 19;
		textBg.height = minTextBgHeigh + (row - 1) * dis;
		#endif
	}

	/// <summary>
	/// Ins it chat player.
	/// </summary>
	/// <param name="tempChatMsg">Temp chat message.</param>
	public void InItChatPlayer (ChatMessage tempChatMsg)
	{
		chatMsg = tempChatMsg;

		isPlayer = (tempChatMsg.chatPct.guoJia > 0 && tempChatMsg.chatPct.guoJia < 8) ? true : false;

		headIcon.transform.parent.gameObject.SetActive (isPlayer ? true : false);
		titleLabel.text = isPlayer ? "" : (tempChatMsg.chatPct.channel == ChatPct.Channel.Broadcast ? "[00e1c4]【广播】[-]" : MyColorData.getColorString (5,"【系统】"));
		textBg.transform.localPosition = new Vector3 (-60,isPlayer ? 6 : 15,0);

		textBg.color = isPlayer ? (tempChatMsg.chatPct.senderId == JunZhuData.Instance ().m_junzhuInfo.id ? new Color (0.7f,1,0.6f) : Color.white) : Color.white;

		textLabel.text = "[dbba8f]" + tempChatMsg.chatPct.content + "[-]";
		Debug.Log ("nationId:" + tempChatMsg.chatPct.guoJia);

		if (isPlayer)
		{
			headIcon.spriteName = "PlayerIcon" + 1;
			string channelStr = "[00e1c4][" + QXChatData.Instance.GetChannelTitleStr (tempChatMsg.chatPct.channel) + "][-]";
			string nationStr = "[e5e205][" + QXComData.GetNationName (tempChatMsg.chatPct.guoJia) + "][-]";
			nameLabel.text = channelStr + nationStr + "[f5aa29]" + tempChatMsg.chatPct.senderName + "[-]";
			vipIcon.spriteName = tempChatMsg.chatPct.vipLevel > 0 ? "vip" + tempChatMsg.chatPct.vipLevel : "";
		}

		int row = textLabel.height / 19;
		textBg.height = minTextBgHeigh + (row - 1) * dis;

		itemHeigh = textBg.height;

		playerHandler.m_handler -= PlayerHandlerClickBack;
		playerHandler.m_handler += PlayerHandlerClickBack;
	}

	void PlayerHandlerClickBack (GameObject obj)
	{
		if (JunZhuData.Instance ().m_junzhuInfo.id == chatMsg.chatPct.senderId)
		{
			QXChatPage.chatPage.SetChatItemInfoClose (true);
			return;	
		}

		List<QXChatItemInfo.ChatBtnInfo> chatBtnInfoList = new List<QXChatItemInfo.ChatBtnInfo> ();
		switch (chatMsg.sendState)
		{
		case ChatMessage.SendState.SEND_SUCCESS:
	
			chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "查看信息",chatBtnDelegate = CheckJunZhuInfo});
		
			if (!FriendOperationData.Instance.friendIdList.Contains (chatMsg.chatPct.senderId))
			{
				chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "加好友",chatBtnDelegate = AddFriend});
			}

			chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "邮件",chatBtnDelegate = SendEmail});

			if (!BlockedData.Instance ().m_BlockedInfoDic.ContainsKey (chatMsg.chatPct.senderId))
			{
				chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "屏蔽",chatBtnDelegate = Shield});
			}

			break;
		case ChatMessage.SendState.SEND_FAIL:

			chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "重发",chatBtnDelegate = SendAgain});
			chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "删除",chatBtnDelegate = Delate});

			break;
		default:
			return;
			break;
		}

		QXChatPage.chatPage.InItChatItemInfo (gameObject,chatBtnInfoList);
	}

	/// <summary>
	/// Gets the item heigh.
	/// </summary>
	/// <returns>The item heigh.</returns>
	public int GetItemHeigh ()
	{
		return itemHeigh;
	}

	private void CheckJunZhuInfo ()
	{	
		JunZhuInfoSpecifyReq temp = new JunZhuInfoSpecifyReq()
		{
			junzhuId = chatMsg.chatPct.senderId
		};
		
		//Set response to king detail info and request.
		QXChatData.Instance.SetOpenJunZhuInfo = true;
		QXComData.SendQxProtoMessage (temp,ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ);
	}

	private void AddFriend ()
	{
		FriendOperationLayerManagerment.AddFriends ((int)chatMsg.chatPct.senderId);
	}

	private void SendEmail ()
	{
		NewEmailData.Instance().OpenEmail (NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE,chatMsg.chatPct.senderName);
	}

	private void Shield ()
	{
		string textStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO) + "\n\n" + LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SHIELD).Replace("***", chatMsg.chatPct.senderName);
		QXComData.CreateBox (1,textStr,false,ShieldCallBack);
	}

	void ShieldCallBack (int i)
	{
		switch (i)
		{
		case 1:
			break;
		case 2:
			JoinToBlacklist tempMsg = new JoinToBlacklist
			{
				junzhuId = chatMsg.chatPct.senderId
			};
			QXComData.SendQxProtoMessage (tempMsg,ProtoIndexes.C_Join_BlackList);
			break;
		default:
			break;
		}
	}

	private void SendAgain ()
	{
		QXChatData.Instance.SendChatData (chatMsg);
	}

	private void Delate ()
	{
		QXChatData.Instance.DelateChatMsg (chatMsg);
	}
}
