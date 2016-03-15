using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
using Object = UnityEngine.Object;

public class QXChatData : Singleton<QXChatData>,SocketProcessor {

	private ChatPct.Channel chatChannel = ChatPct.Channel.SHIJIE;

	private Dictionary<ChatPct.Channel,List<ChatMessage>> chatDic = new Dictionary<ChatPct.Channel, List<ChatMessage>>()
	{
		{ChatPct.Channel.SHIJIE,new List<ChatMessage> ()},
		{ChatPct.Channel.LIANMENG,new List<ChatMessage> ()},
		{ChatPct.Channel.Broadcast,new List<ChatMessage> ()},
		//		{ChatPct.Channel.XiaoWu,new List<ChatMessage> ()},
	};

	private Dictionary<ChatPct.Channel,string[]> chatInfoDic = new Dictionary<ChatPct.Channel, string[]>()
	{
		{ChatPct.Channel.SHIJIE,new string[]{"世","0","-1"}},
		{ChatPct.Channel.LIANMENG,new string[]{"盟","1","-1"}},
		{ChatPct.Channel.Broadcast,new string[]{"播","2","-1"}},
		{ChatPct.Channel.XiaoWu,new string[]{"屋","3","-1"}},
	};

	private JunZhuInfo junZhuInfo;

	private GameObject chatPrefab;
	private bool isOpenChat = false;
	public bool SetOpenChat
	{
		set{isOpenChat = value;}
		get{return isOpenChat;}
	}

	private bool isOpenJunZhuInfo = false;
	public bool SetOpenJunZhuInfo
	{
		set{isOpenJunZhuInfo = value;}
		get{return isOpenJunZhuInfo;}
	}

	private int sendWaitTime;
	public int SendWaitTime {set{sendWaitTime = value;} get{return sendWaitTime;}}

	private int unReceiveWaitTime;

	private List<ChatMessage> receiveList = new List<ChatMessage>();

	private int freeTimes;
	public int FreeTimes {set{freeTimes = value;} get{return freeTimes;}}

	private ChatMessage willSendChatMsg;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// Loads the chat prefab.
	/// </summary>
	public void LoadChatPrefab ()
	{
//		Debug.Log ("LoadChatPrefab");

		if (chatPrefab == null)
		{
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHAT_WINDOW),
			                         LoadChatPrefabCallBack);
		}
		else
		{
			chatPrefab.SetActive (false);
		}

		ReqChatTimes ();
	}

	/// <summary>
	/// Resets the chat info.
	/// </summary>
	public void ResetChatInfo ()
	{
		SendWaitTime = 0;
		StopCoroutine ("SendWait");
		foreach (KeyValuePair<ChatPct.Channel,List<ChatMessage>> pair in chatDic)
		{
			pair.Value.Clear ();
		}
	}

	private void LoadChatPrefabCallBack (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		chatPrefab = Instantiate (p_object) as GameObject;
		chatPrefab.SetActive (false);
		DontDestroyOnLoad (chatPrefab);
	}

	/// <summary>
	/// Reqs the chat times.
	/// </summary>
	void ReqChatTimes ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_GET_CHAT_CONF);
	}

	/// <summary>
	/// Opens the chat page.
	/// </summary>
	public void OpenChatPage ()
	{
		SetOpenChat = true;
		chatPrefab.SetActive (true);
//		MainCityUI.TryAddToObjectList (chatPrefab);
		QXChatPage.chatPage.ChatPageAnimation (true);
		SetChatChannel (ChatPct.Channel.SHIJIE);

		UIYindao.m_UIYindao.CloseUI ();
	}

	/// <summary>
	/// Sets the chat channel.
	/// </summary>
	/// <param name="tempChannel">Temp channel.</param>
	public void SetChatChannel (ChatPct.Channel tempChannel)
	{
		chatChannel = tempChannel;

		QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);

		QXChatPage.chatPage.SetChatBtnRedPoint ();
	}

	/// <summary>
	/// Sends the chat data.
	/// </summary>
	/// <param name="tempChatMsg">Temp chat message.</param>
	public void SendChatData (ChatMessage tempChatMsg)
	{
		if (SendWaitTime > 0) 
		{
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5, "您发言太快！"));
			setOpen = false;
			return;
		}

		willSendChatMsg = tempChatMsg;

		switch (tempChatMsg.chatPct.channel)
		{
		case ChatPct.Channel.SHIJIE:
			
			if (FreeTimes <= 0 && int.Parse (CanshuTemplate.GetStrValueByKey (CanshuTemplate.WORLDCHAT_PRICE)) > 0)
			{
				QXComData.CreateBox (1,"是否花费" + MyColorData.getColorString (5,CanshuTemplate.GetStrValueByKey (CanshuTemplate.WORLDCHAT_PRICE)) + "元宝发送本条消息到世界频道？",false,ShiJieSendCallBack);
			}
			else
			{
				SendChatMessage (tempChatMsg);
			}

			break;
		case ChatPct.Channel.LIANMENG:
			SendChatMessage (tempChatMsg);
			break;
		case ChatPct.Channel.Broadcast:
			QXComData.CreateBox (1,"是否花费" + MyColorData.getColorString (5,CanshuTemplate.GetStrValueByKey (CanshuTemplate.BROADCAST_PRICE)) + "元宝发送本条消息到广播频道？",false,BroadCastSendCallBack);
			break;
		case ChatPct.Channel.XiaoWu:
			SendChatMessage (tempChatMsg);
			break;
		default:
			break;
		}
	}

	void ShiJieSendCallBack (int i)
	{
		if (i == 2)
		{
			SendChatMessage (willSendChatMsg);
		}
		else
		{
			setOpen = false;
		}
	}

	void BroadCastSendCallBack (int i)
	{
		if (i == 2)
		{
			SendChatMessage (willSendChatMsg);
		}
	}

	void SendChatMessage (ChatMessage tempChatMsg)
	{
		string sendStr = tempChatMsg.chatPct.content.Replace ("\n", "");
		sendStr = tempChatMsg.chatPct.content.Replace (" ", "");
		// notice console tool what were typed
		{
			if (ConsoleTool.Instance().OnChatContent (sendStr)) {
				setOpen = false;
				return;
			}
		}
//		Debug.Log ("tempChatMsg.chatPct.type:" + tempChatMsg.chatPct.type);
		if (sendStr.Length > CanshuTemplate.GetValueByKey (CanshuTemplate.CHAT_MAX_WORDS)) 
		{
			sendStr = sendStr.Substring (0, (int)CanshuTemplate.GetValueByKey (CanshuTemplate.CHAT_MAX_WORDS));
		}
		
		tempChatMsg.chatPct.content = sendStr;
		
		//add sendchat into chatlist and show in chatwinow
		tempChatMsg.chatPct.content = ReplaceSensitiveStr.Filter (tempChatMsg.chatPct.content);
		//		Debug.Log ("tempChatMsg.chatPct.content;" + tempChatMsg.chatPct.content);
		chatDic [tempChatMsg.chatPct.channel].Add (tempChatMsg);
		if (tempChatMsg.chatPct.channel == chatChannel)
		{
			QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
		}
		
		receiveList.Add (tempChatMsg);
		
		SendWaitTime = (int)CanshuTemplate.GetValueByKey (CanshuTemplate.CHAT_INTERVAL_TIME);
//		Debug.Log ("SendWaitTime:" + SendWaitTime);
		StartCoroutine ("SendWait");
		QXChatPage.chatPage.ClearInputText (int.Parse (chatInfoDic[chatChannel][1]));
		
		QXComData.SendQxProtoMessage (tempChatMsg.chatPct,ProtoIndexes.C_Send_Chat);
//		Debug.Log ("聊天发送：" + ProtoIndexes.C_Send_Chat);
		
		//计算发送时间，***秒后未收到判定为发送失败(断线时间 + 策划配)
		unReceiveWaitTime = (int)(ConfigTool.GetFloat(ConfigTool.CONST_NETOWRK_SOCKET_TIME_OUT));
//		Debug.Log ("unReceiveWaitTime:" + unReceiveWaitTime);
		StartCoroutine ("ReceiveWait");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_Send_Chat:
			{
//				Debug.Log ("聊天返回：" + ProtoIndexes.S_Send_Chat);
				ChatPct chatData = new ChatPct();
				chatData = QXComData.ReceiveQxProtoMessage (p_message,chatData) as ChatPct;

				if (chatData != null)
				{
//					Debug.Log ("chatData.type:" + chatData.type);
//					Debug.Log ("chatData.senderId:" + chatData.senderId);
//					Debug.Log ("chatData.senderName:" + chatData.senderName);
//					Debug.Log ("chatData.guoJia:" + chatData.guoJia);
//					Debug.Log ("chatData.content:" + chatData.content);
//					Debug.Log ("chatData.dateTime:" + chatData.dateTime);
//					Debug.Log ("chatData.seq:" + chatData.seq);
//					Debug.Log ("chatData.vipLevel:" + chatData.vipLevel);
					#region ShowIn HighestUI
					if (chatData.channel == ChatPct.Channel.Broadcast)
					{
						HighestUI.Instance.m_BroadCast.ShowBroadCast (((chatData.guoJia >= 1 && chatData.guoJia <= 7) ? 
						                                               (ColorTool.Color_Gold_edc347 + "[" + QXComData.GetNationName (chatData.guoJia) + "][-]") : "") + ColorTool.Color_Gold_ffb12a + chatData.senderName + "[-]" + chatData.content);
					}
					#endregion

					#region Set Btn And ChatUIBox RedState
					ChatPct.Channel tempChannel = ChatPct.Channel.SHIJIE;
					tempChannel = chatData.channel == ChatPct.Channel.SYSTEM ? ChatPct.Channel.Broadcast : chatData.channel;
					chatInfoDic[tempChannel][2] = "0";
//					Debug.Log ("tempChannel:" + chatInfoDic[tempChannel][2]);
					QXChatUIBox.chatUIBox.SetRedAlert (!SetOpenChat);
					#endregion

					ChatMessage chatMsg = new ChatMessage()
					{
						sendState = ChatMessage.SendState.SEND_SUCCESS,
						chatPct = chatData
					};

					if (chatMsg.chatPct.senderId == JunZhuData.Instance().m_junzhuInfo.id)
					{
						for (int i = 0;i < receiveList.Count;i ++)
						{
							if (receiveList[i].chatPct.content == chatMsg.chatPct.content)
							{
								if (chatDic[receiveList[i].chatPct.channel].Contains (receiveList[i]))
								{
									chatDic[receiveList[i].chatPct.channel].Remove (receiveList[i]);
								}
								receiveList.RemoveAt (i);

								StopCoroutine ("ReceiveWait");
								unReceiveWaitTime = 0;
								break;
							}
						}
					}

					if (QXChatUIBox.chatUIBox != null)
					{
						QXChatUIBox.chatUIBox.InItChatUIBox (chatMsg);
					}

					if (chatChannel == ChatPct.Channel.SHIJIE)
					{
						if (FreeTimes > 0)
						{
							FreeTimes --;
						}
					}

					if (chatData.channel != ChatPct.Channel.SYSTEM)
					{
						chatDic[chatData.channel].Add (chatMsg);
						if (SetOpenChat)
						{
							if (chatData.channel == chatChannel)
							{
								QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
							}
							QXChatPage.chatPage.SetChatBtnRedPoint ();
						}
					}
					else
					{
						chatDic[ChatPct.Channel.Broadcast].Add (chatMsg);

						if (SetOpenChat)
						{
							if (chatChannel == ChatPct.Channel.Broadcast)
							{
								QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
							}
							QXChatPage.chatPage.SetChatBtnRedPoint ();
						}
					}

					if (setOpen)
					{
						setOpen = false;
						QXChatUIBox.chatUIBox.ChatUIBoxClickBack (gameObject);
					}
				}

				return true;
			}
			case ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP:
			{
				if (!SetOpenJunZhuInfo)
				{
					return false;
				}
//				Debug.Log ("查看君主信息返回：" + ProtoIndexes.JUNZHU_INFO_SPECIFY_RESP);
				JunZhuInfo junZhuResp = new JunZhuInfo();
				junZhuResp = QXComData.ReceiveQxProtoMessage (p_message,junZhuResp) as JunZhuInfo;
				
				if (junZhuResp != null)
				{
					junZhuInfo = junZhuResp;
					SetOpenJunZhuInfo = false;
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);
				}
				return true;
			}
			case ProtoIndexes.S_GET_CHAT_CONF:
			{
				ErrorMessage errorMsg = new ErrorMessage();
				errorMsg = QXComData.ReceiveQxProtoMessage (p_message,errorMsg) as ErrorMessage;

				if (errorMsg != null)
				{
//					Debug.Log ("errorMsg.errorCode:" + errorMsg.errorCode);
					FreeTimes = errorMsg.errorCode;
				}

				return true;
			}
			}
		}
		return false;
	}

	/// <summary>
	/// Gets the channel title string.
	/// </summary>
	/// <returns>The channel title string.</returns>
	/// <param name="tempChannel">Temp channel.</param>
	public string GetChannelTitleStr (ChatPct.Channel tempChannel)
	{
		return chatInfoDic [tempChannel][0];
	}

	/// <summary>
	/// Gets the index of the channel.
	/// </summary>
	/// <returns>The channel index.</returns>
	/// <param name="tempChannel">Temp channel.</param>
	public Dictionary<ChatPct.Channel,string[]> GetChatInfoDic ()
	{
		return chatInfoDic;
	}

	/// <summary>
	/// Gets the index of the channel.
	/// </summary>
	/// <returns>The channel index.</returns>
	public int GetChannelCount ()
	{
		return chatInfoDic.Count;
	}

	/// <summary>
	/// Adds the broadcast message in to chat list.
	/// </summary>
	/// <param name="tempResp">Temp resp.</param>
	public void AddBroadcastMsgInToChatList (ErrorMessage tempResp)
	{
		ChatMessage chatMsg = new ChatMessage ()
		{
			sendState = ChatMessage.SendState.SEND_SUCCESS,
			chatPct = new ChatPct ()
			{
				guoJia = 0,
				channel = ChatPct.Channel.Broadcast,
				content = tempResp.errorDesc
			}
		};

		if (QXChatUIBox.chatUIBox != null)
		{
			QXChatUIBox.chatUIBox.InItChatUIBox (chatMsg);
		}

		chatDic [ChatPct.Channel.Broadcast].Add (chatMsg);
		chatInfoDic[ChatPct.Channel.Broadcast][2] = "0";

		if (SetOpenChat)
		{
			if (chatChannel == ChatPct.Channel.Broadcast)
			{
				QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
			}
			QXChatPage.chatPage.SetChatBtnRedPoint ();
		}
	}
	
	private void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		var temp = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList (temp);
		var info = temp.GetComponent<KingDetailInfo>();
		
		var tempKingInfo = new KingDetailInfo.KingDetailData()
		{
			RoleID = junZhuInfo.roleId,
			Attack = junZhuInfo.gongji,
			AllianceName = junZhuInfo.lianMeng,
			BattleValue = junZhuInfo.zhanli,
			Junxian = junZhuInfo.junxian,
			JunxianRank = junZhuInfo.junxianRank,
			KingName = junZhuInfo.name,
			Level = junZhuInfo.level,
			Money = junZhuInfo.gongjin,
			Life = junZhuInfo.remainHp,
			Protect = junZhuInfo.fangyu,
			Title = junZhuInfo.chenghao
		};
		
		var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>();
		if (junZhuInfo.junZhuId != JunZhuData.Instance().m_junzhuInfo.id)
		{
			if (FriendOperationData.Instance.m_FriendListInfo == null || FriendOperationData.Instance.m_FriendListInfo.friends == null || !FriendOperationData.Instance.m_FriendListInfo.friends.Select(item => item.ownerid).Contains(junZhuInfo.junZhuId))
			{
				tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "加好友", m_ButtonClick = OnAddFriendClick });
			}
			tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "邮件", m_ButtonClick = OnMailClick });
			
			if (BlockedData.Instance().m_BlockedInfoDic == null || BlockedData.Instance().m_BlockedInfoDic.Count == 0 || !BlockedData.Instance().m_BlockedInfoDic.Select(item => item.Value.junzhuId).Contains(junZhuInfo.junZhuId))
			{
				tempConfigList.Add(new KingDetailButtonController.KingDetailButtonConfig() { m_ButtonStr = "屏蔽", m_ButtonClick = OnShieldClick });
			}
		}
		info.SetThis(tempKingInfo, tempConfigList);
		
		info.m_KingDetailEquipInfo.m_BagItemDic = junZhuInfo.equip.items.Where(item => item.buWei > 0).ToDictionary(item => KingDetailInfo.TransferBuwei(item.buWei));
		
		temp.SetActive(true);
	}

	private void OnAddFriendClick()
	{
		FriendOperationLayerManagerment.AddFriends ((int)junZhuInfo.junZhuId);
	}

	private void OnMailClick()
	{
		NewEmailData.Instance().OpenEmail(NewEmailData.EmailOpenType.EMAIL_REPLY_PAGE, junZhuInfo.name);
	}

	private void OnShieldClick()
	{
		string textStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO) + "\n\n" + LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_SHIELD).Replace("***", junZhuInfo.name);
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
				junzhuId = junZhuInfo.junZhuId
			};
			QXComData.SendQxProtoMessage (tempMsg,ProtoIndexes.C_Join_BlackList);
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Delates the chat message.
	/// </summary>
	/// <param name="tempMsg">Temp message.</param>
	public void DelateChatMsg (ChatMessage tempMsg)
	{
		if (chatDic[tempMsg.chatPct.channel].Contains (tempMsg))
		{
			chatDic[tempMsg.chatPct.channel].Remove (tempMsg);
		}
		if (tempMsg.chatPct.channel == chatChannel)
		{
			if (SetOpenChat)
			{
				QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
			}
		}
	}

	/// <summary>
	/// Sends the wait.
	/// </summary>
	/// <returns>The wait.</returns>
	IEnumerator SendWait ()
	{
		while (SendWaitTime > 0)
		{
			yield return new WaitForSeconds (1);
			SendWaitTime --;

			if (SendWaitTime <= 0)
			{
				SendWaitTime = 0;
				StopCoroutine ("SendWait");
			}
		}
	}

	/// <summary>
	/// Receives the wait.
	/// </summary>
	/// <returns>The wait.</returns>
	IEnumerator ReceiveWait ()
	{
		while (unReceiveWaitTime > 0)
		{
			unReceiveWaitTime --;
			yield return new WaitForSeconds (1);
			if (unReceiveWaitTime <= 0)
			{
				unReceiveWaitTime = 0;
				StopCoroutine ("ReceiveWait");

				foreach (ChatMessage mChatMsg in receiveList)
				{
					foreach (ChatMessage targetChatMsg in chatDic[mChatMsg.chatPct.channel])
					{
						if (targetChatMsg == mChatMsg)
						{
							targetChatMsg.sendState = ChatMessage.SendState.SEND_FAIL;
						}
					}
				}

				if (SetOpenChat)
				{
					QXChatPage.chatPage.InItChatPage (chatChannel,chatDic[chatChannel]);
				}
			}
		}
	}

	#region SendAllianceInfo
	private bool setOpen = false;
	/// <summary>
	/// Sends the alliance info.
	/// </summary>
	/// <param name="tempText">Temp text.</param>
	/// <param name="tempLevel">Temp level.</param>
	public void SendAllianceInfo ()
	{
		setOpen = true;

		if (AllianceData.Instance.g_UnionInfo == null)
		{
			Debug.LogError ("AllianceInfo is Null!");
			return;
		}
		var allianceInfo = AllianceData.Instance.g_UnionInfo;
		var junZhuInfo = JunZhuData.Instance().m_junzhuInfo;

		ChatMessage chatMsg = new ChatMessage()
		{
			sendState = ChatMessage.SendState.SENDING,
			chatPct = new ChatPct()
			{
				senderName = junZhuInfo.name,
				senderId = junZhuInfo.id,
				channel = ChatPct.Channel.SHIJIE,
				content = "[ffffff]联盟：[-][06de34]" + allianceInfo.name + "(Lv." + allianceInfo.level + ")[-][ffffff]" + "正在招募人手[-]",
				guoJia = junZhuInfo.guoJiaId,
				vipLevel = junZhuInfo.vipLv,
				lianmengId = junZhuInfo.lianMengId,
				lianmengName = allianceInfo.name,
				type = 2,
			},
		};

		QXChatData.Instance.SendChatData (chatMsg);
	}
	#endregion

	new void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
		base.OnDestroy ();
	}
}
