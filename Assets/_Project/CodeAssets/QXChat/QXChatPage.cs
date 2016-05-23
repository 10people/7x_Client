using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXChatPage : MYNGUIPanel , IMSCListener{

	public static QXChatPage chatPage;

	private ChatPct.Channel chatChannel;

	private bool openFirstChannel = true;//是否第一次打开聊天

	void Awake ()
	{
		chatPage = this;
		MSCController.RegisterListener(this);
	}

	void OnDestroy ()
	{
		chatPage = null;
		MSCController.UnregisterListener(this);
	}

	public void MSCResult(string key, string data, string fileBytes)
	{
		ChatPct chatPct1 = new ChatPct();
		
		ChatMessage chatMsg = new ChatMessage()
		{
			sendState = ChatMessage.SendState.SENDING,
			
			
			chatPct = new ChatPct()
			{
				roleId = CityGlobalData.m_king_model_Id,
				senderName = JunZhuData.Instance().m_junzhuInfo.name,
				senderId = JunZhuData.Instance().m_junzhuInfo.id,
				channel = chatChannel,
				content = data,
				guoJia = JunZhuData.Instance().m_junzhuInfo.guoJiaId,
				vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv,
				receiverId = m_iSiliaoID,
				soundData = fileBytes,
				soundLen = 5,
			},
		};
		
		QXChatData.Instance.SendChatData (chatMsg);
	}
	
	public void MSCStarted()
	{

	}
	
	public void MSCEnded()
	{

	}
	
	public void MSCError(string error)
	{

	}

	public void MSCVolume(int vol)
	{

	}

	#region ShowChatInfo
	public UIScrollView chatSc;
	public UIScrollBar chatSb;
	private float chatSbValue = 1;
	public UIDragScrollView dragArea;
	private float lastScHeigh;
	
	private List<GameObject> chatItemList = new List<GameObject> ();
	public GameObject chatItemObj;
	public GameObject m_objSiliaoObj;
	public UILabel m_labelSiLiaoName;
	public long m_iSiliaoID;

	public QXChatInputEnemt chatInputObj;
	private List<QXChatInputEnemt> inputList = new List<QXChatInputEnemt> ();
	
	public UILabel costLabel;
	public UILabel freeLabel;
	
	public List<EventHandler> sendBtnHandlerList = new List<EventHandler>();
	
	public int chatInputIndex;
	
	public UILabel sendCdLabel;
	
	public List<EventHandler> chatBtnHandlerList = new List<EventHandler> ();

	public UISprite m_spriteYuYinQiehuanButton;

	public UILabelType m_UILabelTypeYuyin;

	public UISprite m_spriteYuyinScendButton;

	private bool m_isYuyin = false;

	private int m_iVoiceIndex = 0;

	public List<int> m_listSoundPlayID = new List<int>();

	/// <summary>
	/// Ins it chat page.
	/// </summary>
	/// <param name="tempChatList">Temp chat list.</param>
	public void InItChatPage (ChatPct.Channel tempChannel,List<ChatMessage> tempChatList)
	{
		chatChannel = tempChannel;

		if (tempChatList.Count > 50)
		{
			tempChatList.RemoveAt (0);
		}

		CreateChatList (tempChatList);

		if (inputList.Count == 0)
		{
			for (int i = 0;i < QXChatData.Instance.GetChannelCount ();i ++)
			{
				QXChatData.Instance.m_listScendWaitTime.Add(0);
				GameObject chatInput = (GameObject)Instantiate (chatInputObj.gameObject);

				chatInput.transform.parent = chatInputObj.transform.parent;
				chatInput.transform.localPosition = Vector3.zero;
				chatInput.transform.localScale = Vector3.one;

				QXChatInputEnemt tempInputEnemt = chatInput.GetComponent<QXChatInputEnemt>();
				inputList.Add (tempInputEnemt);
				if(i == 3)
				{
					tempInputEnemt.m_spriteBG.SetDimensions(222, 40);
					tempInputEnemt.m_labelInputLabel.SetDimensions(196, 30);
					tempInputEnemt.m_labelInputLabel.text = "          点此输入";
				}
			}
		}

		switch (tempChannel)
		{
		case ChatPct.Channel.SHIJIE:
			SwitchChannel (0);
			break;
		case ChatPct.Channel.LIANMENG:
			SwitchChannel (1);
			break;
		case ChatPct.Channel.Broadcast:
			SwitchChannel (2);
			break;
		case ChatPct.Channel.SILIAO:
			SwitchChannel (3);
			break;
		default:
			break;
		}

		foreach (EventHandler handler in chatBtnHandlerList)
		{
			handler.m_click_handler -= ChatBtnHandlerClickBack;
			handler.m_click_handler += ChatBtnHandlerClickBack;
		}

		foreach (EventHandler handler in sendBtnHandlerList)
		{
			handler.m_click_handler -= SendBtnHandlerClickBack;
			handler.m_click_handler += SendBtnHandlerClickBack;
		}

		CityGlobalData.m_isRightGuide = true;
//		MainCityUI.TryAddToObjectList (gameObject);
	}
	
	void CreateChatList (List<ChatMessage> tempChatList)
	{
		int countItem = tempChatList.Count - chatItemList.Count;
		if (countItem > 0)
		{
			for (int i = 0;i < countItem;i ++)
			{
				GameObject chatItem = (GameObject)Instantiate (chatItemObj);
				
				chatItem.SetActive (true);
				chatItem.transform.parent = chatItemObj.transform.parent;
				chatItem.transform.localPosition = Vector3.zero;
				chatItem.transform.localScale = Vector3.one;
				
				chatItemList.Add (chatItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (countItem);i ++)
			{
				Destroy (chatItemList[chatItemList.Count - 1]);
				chatItemList.RemoveAt (chatItemList.Count - 1);
			}
		}

		float firstHeigh = 0;
		float lastHeigh = 0;
		float totleHeigh = 0;
		//s:s :h2 + 11  p:p : h1 + 52.5  p:s : h1 + 36.5 s:p : h2 + 42
		for (int i = 0;i < tempChatList.Count;i ++)
		{
			ChatPlayerItem chatPlayer = chatItemList[i].GetComponent<ChatPlayerItem> ();
			chatPlayer.InItChatPlayer (tempChatList[i]);

			if (i == 0)
			{
				firstHeigh = chatPlayer.GetItemHeigh ();
				bool isPlayer = (tempChatList[i].chatPct.guoJia >= 0 && tempChatList[i].chatPct.guoJia < 8) ? true : false;
				totleHeigh += (isPlayer ? 46.5f : 22.5f ) + firstHeigh;
			}
			else
			{
				float heigh = chatItemList[i - 1].GetComponent<ChatPlayerItem> ().GetItemHeigh ();
				bool isPlayer1 = (tempChatList[i - 1].chatPct.guoJia >= 0 && tempChatList[i - 1].chatPct.guoJia < 8) ? true : false;
				bool isPlayer2 = (tempChatList[i].chatPct.guoJia >= 0 && tempChatList[i].chatPct.guoJia < 8) ? true : false;

				if (!isPlayer1)
				{
					chatItemList[i].transform.localPosition = chatItemList[i - 1].transform.localPosition + new Vector3(0,-(heigh + (!isPlayer2 ? 11 : 42)),0);//ss:sp
					totleHeigh += (heigh + (!isPlayer2 ? 11 : 42));
				}
				else
				{
					chatItemList[i].transform.localPosition = chatItemList[i - 1].transform.localPosition + new Vector3(0,-(heigh + (!isPlayer2 ? 36.5f : 52.5f)),0);//ps:pp
					totleHeigh += (heigh + (!isPlayer2 ? 36.5f : 52.5f));
				}

				if (i == tempChatList.Count - 1)
				{
					lastHeigh = chatPlayer.GetItemHeigh ();
					totleHeigh += (lastHeigh - firstHeigh);
				}
			}
		}
//		Debug.Log ("totleHeigh:" + totleHeigh);

		#region SetChatScrollView

//		Debug.Log ("chatSb.value:" + chatSb.value);
//		Debug.Log ("chatSc.GetComponent<UIPanel> ().GetViewSize ().y:" + chatSc.GetComponent<UIPanel> ().GetViewSize ().y);

		bool canDrag = totleHeigh > chatSc.GetComponent<UIPanel> ().GetViewSize ().y ? true : false;
		bool isDragChatPage = false;

		if (!canDrag)
		{
			chatSc.ResetPosition ();
			isDragChatPage = false;
		}
		else
		{
			bool isMySelf = tempChatList [tempChatList.Count - 1].chatPct.senderId == JunZhuData.Instance ().m_junzhuInfo.id ? true : false;
			isDragChatPage = openFirstChannel ? false : (isMySelf ? false : (chatSb.value < 0.9f ? true : false));
		}

		foreach (GameObject obj in chatItemList)
		{
			SpringPosition.Begin(obj,obj.transform.localPosition, 30f).updateScrollView = true;
			chatSc.UpdateScrollbars (true);
		}

		chatSb.gameObject.SetActive (canDrag);
		dragArea.enabled = canDrag ? true : false;
		chatSb.value = isDragChatPage ? chatSb.value : (canDrag ? chatSbValue : 0);
//		Debug.Log ("lastScHeigh:" + lastScHeigh);
		if (canDrag)
		{
			if (lastScHeigh > totleHeigh)
			{
				float countHeigh = lastScHeigh - totleHeigh - 10;
				chatSc.GetComponent<UIPanel> ().clipOffset += new Vector2 (0,countHeigh);
				chatSc.gameObject.transform.localPosition -= new Vector3(0,countHeigh,0);
			}
		}

		SpringPanel sp = chatSc.GetComponent<SpringPanel>();
		if (sp == null)
		{
			sp = chatSc.gameObject.AddComponent<SpringPanel> ();
		}
		sp.target = chatSc.transform.localPosition;

		lastScHeigh = totleHeigh;

		#endregion

		openFirstChannel = false;
	}

	public void ChatBtnHandlerClickBack (GameObject obj)
	{
		SetChatItemInfoClose (false);
		switch (obj.name)
		{
		case "WorldBtn":
			if (chatChannel != ChatPct.Channel.SHIJIE)
			{
				chatSbValue = 1;
				openFirstChannel = true;

				QXChatData.Instance.SetChatChannel (ChatPct.Channel.SHIJIE);
			}
			break;
		case "AllianceBtn":
			if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
			{
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5, "请先加入一个联盟！"));
				return;
			}
			if (chatChannel != ChatPct.Channel.LIANMENG)
			{
				chatSbValue = 1;
				openFirstChannel = true;

				QXChatData.Instance.SetChatChannel (ChatPct.Channel.LIANMENG);
			}
			break;
		case "BroadCastBtn":
			if (chatChannel != ChatPct.Channel.Broadcast)
			{
				chatSbValue = 1;
				openFirstChannel = true;

				QXChatData.Instance.SetChatChannel (ChatPct.Channel.Broadcast);
			}
			break;
		case "Siliao":
			if (chatChannel != ChatPct.Channel.SILIAO)
			{
				chatSbValue = 1;
				openFirstChannel = true;
				
				QXChatData.Instance.SetChatChannel (ChatPct.Channel.SILIAO);
			}
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Switchs the channel.
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	void SwitchChannel (int tempIndex)
	{
		for (int i = 0;i < chatBtnHandlerList.Count;i ++)
		{
			UIWidget[] btnWidget = chatBtnHandlerList[i].GetComponentsInChildren<UIWidget> ();
			foreach (UIWidget widget in btnWidget)
			{
				widget.color = i == tempIndex ? Color.white : Color.grey;
			}
		}

		SwitchChatInput (tempIndex);
	}
	#endregion

	#region InputChatInfo


	/// <summary>
	/// Switchs the chat input.
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	void SwitchChatInput (int tempIndex)
	{
		freeLabel.transform.parent.gameObject.SetActive ((tempIndex == 1 ||  tempIndex == 3)? false : true);

		for (int i = 0;i < inputList.Count;i ++)
		{
			inputList[i].gameObject.SetActive (i == tempIndex ? true : false);
		}
		m_objSiliaoObj.SetActive(false);
		chatInputIndex = tempIndex;
		OnChatSubmit ();
		switch (tempIndex)
		{
		case 0:
			freeLabel.text = QXChatData.Instance.FreeTimes > 0 ? "免费\n（" + QXChatData.Instance.FreeTimes + "）" : "";
			costLabel.gameObject.SetActive (QXChatData.Instance.FreeTimes > 0 ? false : true);
			costLabel.text = CanshuTemplate.GetStrValueByKey (CanshuTemplate.WORLDCHAT_PRICE);
			break;
		case 2:
			freeLabel.text = "";
			costLabel.gameObject.SetActive (true);
			costLabel.text = CanshuTemplate.GetStrValueByKey (CanshuTemplate.BROADCAST_PRICE);
			break;
		case 3:
			m_objSiliaoObj.SetActive(true);
			break;
		default:
			break;
		}
	}

	public void OnChatSubmit ()
	{
		UIInput input = inputList[chatInputIndex].GetComponent<UIInput> ();
		if (input.value == "" || QXChatData.Instance.m_listScendWaitTime[chatInputIndex] > 0)
		{
			SendBtnActiveState (false);
		}
		else
		{
			SendBtnActiveState (true);
		}
	}

	void SendBtnActiveState (bool active)
	{
		if(m_isYuyin)
		{
			m_spriteYuyinScendButton.color = active ? Color.white : Color.grey;
			m_UILabelTypeYuyin.setType(active ? 10 : 11);
		}
		else
		{
			UIWidget[] widgets = sendBtnHandlerList[2].GetComponentsInChildren<UIWidget> ();
			foreach (UIWidget widget in widgets)
			{
				widget.color = active ? Color.white : Color.grey;
			}
		}
	}

	/// <summary>
	/// Clears the input text.
	/// </summary>
	/// <param name="index">Index.</param>
	public void ClearInputText (int index)
	{
		UIInput input = inputList[index].GetComponent<UIInput> ();
		input.value = "";
	}

	void SendBtnHandlerClickBack (GameObject obj)
	{
		SetChatItemInfoClose (false);
		switch (obj.name)
		{
		case "SendBtn":

			UIInput input = inputList[chatInputIndex].GetComponent<UIInput> ();
			if (string.IsNullOrEmpty (input.value))
			{
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5, "发送内容不能为空！"));
			}
			else
			{
				ChatMessage chatMsg = new ChatMessage()
				{
					sendState = ChatMessage.SendState.SENDING,
					chatPct = new ChatPct()
					{
						roleId = CityGlobalData.m_king_model_Id,
						senderName = JunZhuData.Instance().m_junzhuInfo.name,
						senderId = JunZhuData.Instance().m_junzhuInfo.id,
						channel = chatChannel,
						content = input.value,
						guoJia = JunZhuData.Instance().m_junzhuInfo.guoJiaId,
						vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv,
						receiverId = m_iSiliaoID,
					},
				};
				Debug.Log(m_iSiliaoID);
				Debug.Log(chatChannel);

				QXChatData.Instance.SendChatData (chatMsg);
			}
			
			break;
		case "BackBtn":
			ChatPageAnimation (false);
			break;
		case "TextBackGround":
			SetChatItemInfoClose (true);
			break;
		default:
			break;
		}
	}

	#endregion

	#region OpenChatItemInfo

	public GameObject chatItemInfoObj;
	private bool isChatItemInfoOpen = false;

	/// <summary>
	/// Ins it chat item info.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="tempList">Temp list.</param>
	public void InItChatItemInfo (GameObject target,List<QXChatItemInfo.ChatBtnInfo> tempList)
	{
		isChatItemInfoOpen = true;

		chatItemInfoObj.SetActive (true);
		chatItemInfoObj.transform.localPosition = target.transform.localPosition + new Vector3(-60,260,0);
		chatItemInfoObj.transform.localScale = Vector3.one;

		QXChatItemInfo chatItemInfo = chatItemInfoObj.GetComponent<QXChatItemInfo> ();
		chatItemInfo.InItChatBtn (tempList);

		UISprite widget = chatItemInfoObj.GetComponent<UISprite>();
		float widgetValue = chatSc.GetWidgetValueRelativeToScrollView (widget).y;
		if (widgetValue < 0 || widgetValue > 1)
		{
			chatSc.SetWidgetValueRelativeToScrollView(widget, 0);
			
			//clamp scroll bar value.
			//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
			//set 0.99 and 0.01 cause same bar value not taken in execute.
			float scrollValue = chatSc.GetSingleScrollViewValue();
			if (scrollValue >= 1) chatSb.value = 0.99f;
			if (scrollValue <= 0) chatSb.value = 0.01f;
		}
	}

	/// <summary>
	/// Sets the chat item info close.
	/// </summary>
	public void SetChatItemInfoClose (bool springPanel)
	{
		chatItemInfoObj.SetActive (false);
		isChatItemInfoOpen = false;

		if (springPanel && chatSb.value >= 0.99f)
		{
			SpringPanel sp = chatSc.GetComponent<SpringPanel>();
			if (sp != null && chatSc.transform.localPosition.y > sp.target.y ) sp.enabled = true;
			chatSc.UpdateScrollbars (true);
		}
	}

	#endregion
	
	private bool isCdEnd = false;

	void Update ()
	{
		UIYindao.m_UIYindao.CloseUI ();
		if (QXChatData.Instance.m_listScendWaitTime[chatInputIndex] > 0)
		{
			sendCdLabel.text = "冷却" + MyColorData.getColorString (5,QXChatData.Instance.m_listScendWaitTime[chatInputIndex] + "s");
		}
		else
		{
			sendCdLabel.text = "";
		}
	}

	#region ChatPageAnimation
	public GameObject chatPageObj;
	public GameObject chatWindowBtn;
	private bool isOpenChat;
	public void ChatPageAnimation (bool isOpen)
	{
		Debug.Log("=======1");
		isOpenChat = isOpen;

		Hashtable move = new Hashtable ();
		move.Add ("time",0.4f);
		move.Add ("position",new Vector3 (isOpen ? 0 : -555,0,0));
		move.Add ("islocal",true);
		move.Add ("oncomplete","ChatPageAnimationEnd");
		move.Add ("oncompletetarget",gameObject);
		move.Add ("easetype",isOpen ? iTween.EaseType.easeOutBack : iTween.EaseType.easeInBack);
		iTween.MoveTo (chatPageObj,move);
	}
	void ChatPageAnimationEnd ()
	{
		chatWindowBtn.transform.localRotation = Quaternion.Euler (0,0,isOpenChat ? 90 : -90);
		if (!isOpenChat)
		{
			//open yindao
			{
				if (MainCityUI.m_MainCityUI != null)
				{
					MainCityUI.m_MainCityUI.setInit ();
				}
			}
			openFirstChannel = true;
			chatSbValue = 1;
			chatSb.value = 0;
//			MainCityUI.TryRemoveFromObjectList (gameObject);
			QXChatData.Instance.SetOpenChat = false;
			gameObject.SetActive (false);
		}
	}

	public void SetSiliaoName(string name)
	{
		m_labelSiLiaoName.text = "@" + name + ":";
	}
	#endregion

	#region ChatPageRedPoint
	public List<GameObject> redList = new List<GameObject> ();

	/// <summary>
	/// Sets the chat button red point.
	/// </summary>
	/// <param name="tempChannel">Temp channel.</param>
	public void SetChatBtnRedPoint ()
	{
		foreach (KeyValuePair<ChatPct.Channel,string[]> pair in QXChatData.Instance.GetChatInfoDic ())
		{
//			Debug.Log ("pair.Key:" + pair.Key + "||chatChannel:" + chatChannel);
			pair.Value[2] = pair.Key == chatChannel ? "-1" : pair.Value[2];
			redList[int.Parse (pair.Value[1])].SetActive (pair.Value[2] == "-1" ? false : true);
		}
	}

	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("YunyinQiehuanBtn") != -1)
		{
			m_isYuyin = !m_isYuyin;
			if(m_isYuyin)
			{
				m_spriteYuYinQiehuanButton.spriteName = "jianpan";
				m_spriteYuyinScendButton.gameObject.SetActive(true);
				sendBtnHandlerList[2].gameObject.SetActive(false);
				inputList[chatInputIndex].gameObject.SetActive(false);
//				m_spriteYuyinScendButton.SetDimensions();
			}
			else
			{
				m_spriteYuYinQiehuanButton.spriteName = "yuyin";
				m_spriteYuyinScendButton.gameObject.SetActive(false);
				sendBtnHandlerList[2].gameObject.SetActive(true);
				inputList[chatInputIndex].gameObject.SetActive(true);
			}
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
		if(ui.name.IndexOf("YuyinScendBtn") != -1)
		{
			if(isPress)
			{
				MSCController.Instance.StartMSC(m_iVoiceIndex + "");
			}
			else
			{
				m_iVoiceIndex ++;
				MSCController.Instance.StopMSC();
			}
		}
		else if(ui.name.IndexOf("dialog") != -1)
		{
			int index = int.Parse(ui.name.Substring(6, ui.name.Length - 6));

		}
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
	#endregion
}
