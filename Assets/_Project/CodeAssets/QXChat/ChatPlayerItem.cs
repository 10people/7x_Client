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

public class ChatPlayerItem : MYNGUIPanel,SocketListener {

	private ChatMessage chatMsg;

	public UISprite headIcon;
	public UISprite vipIcon;
	public UISprite channelIcon;
	public UISprite nationIcon;
	public UILabel nameLabel;
	public UISprite textBg;
	public UILabel textLabel;

	public UISprite channelTitle;

	private const int minTextBgHeigh = 43;
	private const int dis = 24;

//	private const int bgDis = -37;
	private const int textLabelMixWidth = 295;

	private const int colorCharNum = 33;//发送的内容包含的颜色代码字符长度

	public EventHandler playerHandler;

	public EventHandler allianceInfoHandler;

	private bool isPlayer;

	private float itemHeigh;

	private string textStr;

	void Awake ()
	{
		SocketTool.RegisterSocketListener (this);
	}

	void Start ()
	{
		#if StartTest

		string s1 = "测，.1!！q#*[";
		string s = "测试啊啊啊啊啊啊啊a啊啊啊啊啊";
		Debug.Log ("s:" + s);
	
//		for (int i = 0;i < s.Length - 1;i ++)
//		{
//			Debug.Log ("str:" + s[i] + "|||||IsMatch汉字:" + Regex.IsMatch(s[i].ToString (), @"[\u4e00-\u9fbb]"));
//			Debug.Log ("str:" + s[i] + "|||||IsMatch数字:" + Regex.IsMatch(s[i].ToString (), @"[0-9]"));
//			Debug.Log ("str:" + s[i] + "|||||IsMatch标点符号:" + Regex.IsMatch(s[i].ToString (), @"[\,\.\?\!]"));
//		}

		BetterList<Vector3> mTempVerts = new BetterList<Vector3> ();
		BetterList<int> mTempIndices = new BetterList<int> ();
		string mProcessedText = "";
		bool fits = NGUIText.WrapText(s, out mProcessedText, false);
		NGUIText.PrintCharacterPositions (s, mTempVerts, mTempIndices);
		Vector3[] list = mTempVerts.ToArray();
		int[] indexs = mTempIndices.ToArray();

		Debug.Log ("list[list.Length - 1]:" + list[list.Length - 1]);

		#endif
	}

	/// <summary>
	/// Ins it chat player.
	/// </summary>
	/// <param name="tempChatMsg">Temp chat message.</param>
	public void InItChatPlayer (ChatMessage tempChatMsg)
	{
		chatMsg = tempChatMsg;

		isPlayer = (tempChatMsg.chatPct.guoJia >= 0 && tempChatMsg.chatPct.guoJia < 8) ? true : false;

		headIcon.gameObject.SetActive (isPlayer ? true : false);
		channelTitle.spriteName = isPlayer ? "" : (tempChatMsg.chatPct.channel == ChatPct.Channel.Broadcast ? "broadCast" : "system");
		textBg.transform.localPosition = new Vector3 (-105,isPlayer ? 1 : 20,0);

		textBg.spriteName = isPlayer ? (tempChatMsg.chatPct.senderId == JunZhuData.Instance().m_junzhuInfo.id ? "DialogSelf" : "DialogOther") : "DialogOther";
		textBg.color = isPlayer ? Color.white : new Color (0.02f, 0.02f, 0.02f);
		textBg.gameObject.name = "dialog" + tempChatMsg.chatPct.seq;

//		Debug.Log ("nationId:" + tempChatMsg.chatPct.guoJia);

		if (isPlayer)
		{
			headIcon.spriteName = "Player_" + tempChatMsg.chatPct.roleId;
			vipIcon.spriteName = tempChatMsg.chatPct.vipLevel > 0 ? "vip" + tempChatMsg.chatPct.vipLevel : "";
			channelIcon.spriteName = QXChatData.Instance.GetChannelTitleStr (tempChatMsg.chatPct.channel);
			nationIcon.spriteName = QXComData.GetNationSpriteName (tempChatMsg.chatPct.guoJia);
			nameLabel.text = MyColorData.getColorString (1,tempChatMsg.chatPct.senderName);
		}

//		byte[] bytes = System.Text.Encoding.Default.GetBytes (tempChatMsg.chatPct.content);

		textLabel.text = isPlayer ? "[000000]" + tempChatMsg.chatPct.content + "[-]" : tempChatMsg.chatPct.content;

		int row = textLabel.height / dis;

		if (NGUIHelper.GetTextWidth (textLabel,textLabel.text).x <= 280)
		{
			textBg.width = (int)(NGUIHelper.GetTextWidth (textLabel,textLabel.text).x + 28);
		}
		else
		{
			textBg.width = 308;
		}

//		BetterList<Vector3> mTempVerts = new BetterList<Vector3> ();
//		BetterList<int> mTempIndices = new BetterList<int> ();
//		NGUIText.PrintCharacterPositions (textLabel.text, mTempVerts, mTempIndices);
//		Vector3[] list = mTempVerts.ToArray();
//		int[] indexs = mTempIndices.ToArray();
//		
//		Debug.Log ("list:" + list[list.Length - 1]);
//		Debug.Log ("list.len:" + list.Length);
//		Debug.Log ("indexs:" + indexs.Length);
//		textBg.width = row > 1 ? 325 : (int)(list [list.Length - 1].x + 34);

//		Debug.Log ("tempChatMsg.chatPct.type:" + tempChatMsg.chatPct.type);
		#region AllianceInfo Btn
		allianceInfoHandler.gameObject.SetActive (tempChatMsg.chatPct.type == 2 ? true : false);

//		Debug.Log ("numnum:" + num);
		bool add = false;
		if (tempChatMsg.chatPct.type == 2)
		{
			add = true;
//			if (num - colorCharNum <= 29)
//			{
//				allianceInfoHandler.transform.localPosition = new Vector3(60 + (num - colorCharNum - 19) * 8,-39,0);
//			}
//			else
//			{
//				allianceInfoHandler.transform.localPosition = new Vector3(60 + ((num - colorCharNum - 38) > 0 ? (num - colorCharNum - 38) * 8 : 0),-59,0);
//				add = true;
//			}
		}
		#endregion

		textBg.height = minTextBgHeigh + (row - 1) * dis + (add ? 15 : 0);

		itemHeigh = isPlayer ? textBg.height + 14.5f : textBg.height;//*************消息的间隔

		playerHandler.m_click_handler -= PlayerHandlerClickBack;
		playerHandler.m_click_handler += PlayerHandlerClickBack;

		allianceInfoHandler.m_click_handler -= AllianceInfoHandlerClickBack;
		allianceInfoHandler.m_click_handler += AllianceInfoHandlerClickBack;

		BoxCollider tempBox = textBg.GetComponent<BoxCollider>();
		tempBox.center = new Vector3(textBg.width / 2, - textBg.height / 2, 0);
		tempBox.size = new Vector3(textBg.width, textBg.height, 0);
	}

	void PlayerHandlerClickBack (GameObject obj)
	{
		if (JunZhuData.Instance().m_junzhuInfo.id == chatMsg.chatPct.senderId)
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

			if (!BlockedData.Instance().m_BlockedInfoDic.ContainsKey (chatMsg.chatPct.senderId))
			{
				chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "屏蔽",chatBtnDelegate = Shield});
			}

			if (chatMsg.chatPct.lianmengId <= 0)
			{
				chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "邀请入盟",chatBtnDelegate = InvitedIntoAlliance});
			}

			chatBtnInfoList.Add (new QXChatItemInfo.ChatBtnInfo (){btnString = "私聊",chatBtnDelegate = GotoSiliao});
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

	void AllianceInfoHandlerClickBack (GameObject obj)
	{
//		Debug.Log ("chatMsg.chatPct.lianmengId:" + chatMsg.chatPct.lianmengId);
		if (chatMsg.chatPct.lianmengId <= 0)
		{
			return;
		}

		if (!FunctionOpenTemp.IsHaveID (104))
		{
			textStr = FunctionOpenTemp.GetTemplateById (104).m_sNotOpenTips;
			QXComData.CreateBox (1,textStr,true,null);
			return;
		}

		int m_allianceId = JunZhuData.Instance().m_junzhuInfo.lianMengId;
//		Debug.Log ("m_allianceId:" + m_allianceId);

		if (m_allianceId <= 0)
		{
			//跳转到加入联盟页面（无联盟）
			var noAllianceDic = AllianceData.Instance.m_AllianceInfoDic;
			if (noAllianceDic.ContainsKey (chatMsg.chatPct.lianmengId))
			{
				FunctionWindowsCreateManagerment.CreateAllianceLayer (chatMsg.chatPct.lianmengId);
			}
			else
			{
				textStr = "此联盟不存在！";
				QXComData.CreateBox (1,textStr,true,null);
				return;
			}
		}
		else
		{
			//跳转到查看联盟信息页面（有联盟）
			AllianceMemberWindowManager.Instance.OpenAllianceMemeberWindow (chatMsg.chatPct.lianmengId,chatMsg.chatPct.lianmengName);
		}

		QXChatPage.chatPage.ChatPageAnimation (false);
	}

	/// <summary>
	/// Gets the item heigh.
	/// </summary>
	/// <returns>The item heigh.</returns>
	public float GetItemHeigh ()
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
		string textStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SHIELD).Replace("***", chatMsg.chatPct.senderName);
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

	public void InvitedIntoAlliance ()
	{
		AllianceData.Instance.RequestAllianceInvite (chatMsg.chatPct.senderId);
	}

	private void SendAgain ()
	{
		QXChatData.Instance.SendChatData (chatMsg);
	}

	private void Delate ()
	{
		QXChatData.Instance.DelateChatMsg (chatMsg);
	}

	public void GotoSiliao()
	{
		QXChatPage.chatPage.SetSiliaoName(chatMsg.chatPct.senderName);
		QXChatPage.chatPage.m_iSiliaoID = chatMsg.chatPct.senderId;
		GameObject tempButton = new GameObject();
		tempButton.name = "Siliao";
		QXChatPage.chatPage.ChatBtnHandlerClickBack(tempButton);
	}

	public bool OnSocketEvent (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_Join_BlackList_Resp://返回加入黑名单信息 
			{	
				BlacklistResp joinResp = new BlacklistResp();
				joinResp = QXComData.ReceiveQxProtoMessage (p_message,joinResp) as BlacklistResp;

				if (joinResp != null)
				{
					if (joinResp.junzhuId != chatMsg.chatPct.senderId)
					{
						Debug.Log("joinResp.junzhuId:" + joinResp.junzhuId);
						return false;
					}
					switch (joinResp.result)
					{
					case 0:

						BlockedData.Instance().m_BlockedInfoDic.Add (joinResp.junzhuId, joinResp.junzhuInfo);

						break;
					case 103:

						string textStr = LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_2) + "\n" + LanguageTemplate.GetText(LanguageTemplate.Text.FRIEND_SIGNAL_TAG_3);
						QXComData.CreateBox (1,textStr,true,null);

						break;
					default:
						break;
					}
				}
				return true;
			}
			}
		}
		return false;
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("dialog") != -1)
		{
			Debug.Log(chatMsg.chatPct.soundLen);
		    MSCPlayer.Instance.PlaySound(chatMsg.chatPct.seq, chatMsg.chatPct.channel);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{

	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterSocketListener (this);
	}
}
