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

	public List<ChatMessage> m_listChatMessage = new List<ChatMessage>();//要播放的语音列表
	public List<UISprite> m_listSpriteKaiguan = new List<UISprite>();
	public bool[] m_listKaiguan = new bool[]{true, true, true, true};
	public bool m_isPlaying = false;//是否播放语音中
	public GameObject m_objSetting;

	public bool m_isLuyining = false;//是否录音中
	public float m_fNum = 0;
	public float m_fBTime;
	public int m_iLuzhiTime = 0;
	public UILabel m_labelLuyin;
	public List<UISprite> m_listYinLiang = new List<UISprite>();
	public GameObject m_objLuying;
	public GameObject m_objLuyinTime;
	public GameObject m_objGotoOBj = null;
	public List<GameObject> m_listGotoObj;

	public UISprite m_spriteVipGuangbo;
	public UILabel m_labelVipGuangbo;
	public BoxCollider m_BoxCollider;

	public struct NewSiliaoData
	{
		public int id;
		public string name;
	}

	public List<NewSiliaoData> m_listNewSiliaoData = new List<NewSiliaoData>();
	public GameObject m_objNotFirend;

	void Awake ()
	{
		chatPage = this;
		MSCController.RegisterListener(this);
		MSCPlayer.Instance.ExecuteAfterPlayEnds = PlayeringEnd;
		m_spriteVipGuangbo.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey(31);


		string saveString;
		saveString = PlayerPrefs.GetString( "DialogSetting" );
		if(saveString != null && saveString != "")
		{
			for(int i = 0; i < 4; i ++)
			{
				m_listKaiguan[i] = int.Parse(Global.NextCutting(ref saveString, ",")) == 1 ? true : false;
			}
		}

		if (m_floatBtnObj == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
		}
	}

	void OnDestroy ()
	{
		chatPage = null;
		MSCController.UnregisterListener(this);
	}

	public void MSCResult(string key, string data, string fileBytes)
	{
		Debug.Log("MSCResult");
		Debug.Log(QXChatUIBox.m_bFocused);
		Debug.Log(QXChatUIBox.m_bPause);
		Debug.Log(fileBytes.Length);
		if(QXChatUIBox.m_bFocused && !QXChatUIBox.m_bPause)
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
					receiverName = m_sScendName,
					soundData = fileBytes,
					soundLen = m_iLuzhiTime,
				},
			};
			
			QXChatData.Instance.SendChatData (chatMsg);
		}

	}
	
	public void MSCStarted()
	{

	}
	
	public void MSCEnded()
	{

	}

	public void PlayeringEnd()
	{
//		Debug.Log("邸松接收到梁霄的播放接收");
		m_isPlaying = false;
	}

	public void stopYuyin()
	{
		m_isPlaying = false;
		MSCPlayer.Instance.stopSound();
	}
	
	public void MSCError(string error)
	{
		ClientMain.m_UITextManager.createText(error);
	}

	public void MSCVolume(int vol)
	{
		int num = vol / 6;
		for(int i = 0; i < 5; i ++)
		{
			m_listYinLiang[i].gameObject.SetActive(i < num);
		}
	}

	#region ShowChatInfo
	public UIScrollView chatSc;
	public UIScrollBar chatSb;
	private float chatSbValue = 1;
	public UIDragScrollView dragArea;
	private float lastScHeigh;
	
	public List<GameObject> chatItemList = new List<GameObject> ();
	public GameObject chatItemObj;
	public GameObject m_objSiliaoObj;
	public string m_sScendName;
	public UILabel m_labelSiLiaoName;
	public long m_iSiliaoID;
	public GameObject m_objFriendButton;
	public GameObject m_objFirendPanel;
	public List<QXFriendData> m_listFriendData;

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
					tempInputEnemt.m_spriteBG.SetDimensions(127, 40);
					tempInputEnemt.m_labelInputLabel.SetDimensions(107, 30);
					tempInputEnemt.m_labelInputLabel.text = "  点此输入";
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

	void FloatButtonLoadCallBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		m_floatBtnObj = p_object as GameObject;
	}

	public void qiangzhiPlay(ChatMessage data)
	{
		for(int i = 0; i < m_listChatMessage.Count; i ++)
		{
			if(m_listChatMessage[i].chatPct.seq == data.chatPct.seq)
			{
				m_listChatMessage.RemoveAt(i);
//				int q = i;
//				for(int p = 0; p < q; p ++)
//				{
//					m_listChatMessage.RemoveAt(0);
//				}
				break;
			}
		}
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
		m_objFriendButton.SetActive(false);

		m_spriteVipGuangbo.gameObject.SetActive(false);
		m_labelVipGuangbo.gameObject.SetActive(false);

		chatInputIndex = tempIndex;
		OnChatSubmit ();
		switch (tempIndex)
		{
		case 0:
			freeLabel.text = QXChatData.Instance.FreeTimes > 0 ? "免费\n（" + QXChatData.Instance.FreeTimes + "）" : "";
			costLabel.gameObject.SetActive (QXChatData.Instance.FreeTimes > 0 ? false : true);
			costLabel.text = CanshuTemplate.GetStrValueByKey (CanshuTemplate.WORLDCHAT_PRICE);
			m_objFirendPanel.SetActive(false);
			break;
		case 1:
			m_objFirendPanel.SetActive(false);
			break;
		case 2:
			freeLabel.text = "";
			costLabel.gameObject.SetActive (true);
			costLabel.text = CanshuTemplate.GetStrValueByKey (CanshuTemplate.BROADCAST_PRICE);
			m_objFirendPanel.SetActive(false);
			m_spriteVipGuangbo.gameObject.SetActive(true);
			m_labelVipGuangbo.gameObject.SetActive(true);
			break;
		case 3:
			m_objSiliaoObj.SetActive(true);
			m_objFriendButton.SetActive(true);
			break;
		default:
			break;
		}
		if(m_isYuyin && chatInputIndex != 2)
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
		if(chatInputIndex == 2)
		{
			m_spriteYuYinQiehuanButton.gameObject.SetActive(false);
		}
		else
		{
			m_spriteYuYinQiehuanButton.gameObject.SetActive(true);
		}
		switch(chatInputIndex)
		{
		case 0:
		case 1:
		case 2:
			m_spriteYuyinScendButton.gameObject.transform.localPosition = new Vector3(0, 0, 0);
			m_spriteYuyinScendButton.SetDimensions(257, m_spriteYuyinScendButton.height);
			m_spriteYuyinScendButton.GetComponent<BoxCollider>().size = new Vector3(257, 60, 0);
			break;
		case 3:
			m_spriteYuyinScendButton.gameObject.transform.localPosition = new Vector3(28, 0, 0);
			m_spriteYuyinScendButton.SetDimensions(195, m_spriteYuyinScendButton.height);
			m_spriteYuyinScendButton.GetComponent<BoxCollider>().size = new Vector3(195, 60, 0);
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
//			m_spriteYuyinScendButton.color = active ? Color.white : Color.grey;
//			m_UILabelTypeYuyin.setType(active ? 10 : 11);
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
			else if(chatChannel == ChatPct.Channel.SILIAO && JunZhuData.Instance().m_junzhuInfo.level < 2)
			{
				ClientMain.m_UITextManager.createText("等级达到2级后才可私聊");
			}
			else if(chatChannel == ChatPct.Channel.SILIAO && (m_sScendName == null || m_sScendName == ""))
			{
				ClientMain.m_UITextManager.createText("请选择私聊对象");
			}
			else if(chatChannel == ChatPct.Channel.Broadcast && JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(31))
			{
				Global.CreateFunctionIcon(1901);
			}
			else if(chatChannel == ChatPct.Channel.Broadcast && JunZhuData.Instance().m_junzhuInfo.yuanBao < int.Parse(CanshuTemplate.GetStrValueByKey (CanshuTemplate.BROADCAST_PRICE)))
			{
				Global.CreateFunctionIcon(101);
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
						receiverName = m_sScendName,
					},
				};

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

	private bool isChatItemInfoOpen = false;

	private GameObject m_tempObject;
	private GameObject m_floatBtnObj;

	/// <summary>
	/// Ins it chat item info.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="tempList">Temp list.</param>
	public void InItChatItemInfo (GameObject target,ChatMessage tempMsg)
	{
		isChatItemInfoOpen = true;

		m_tempObject = (GameObject)Instantiate(m_floatBtnObj);
		m_tempObject.SetActive (true);
		m_tempObject.transform.parent = target.transform.parent;
		m_tempObject.transform.localPosition = target.transform.localPosition + new Vector3(25,0,0);
		m_tempObject.transform.localScale = Vector3.one * 0.8f;
		FloatButtonsController floatBtn = m_tempObject.GetComponent<FloatButtonsController>();
		floatBtn.Initialize(FloatButtonsConfig.GetConfig(tempMsg.chatPct.senderId, tempMsg.chatPct.senderName, tempMsg.chatPct.lianmengName, new List<GameObject> (){}, null), true);
		Debug.Log ("m_tempObject:" + m_tempObject);
		Debug.Log ("floatBtn:" + floatBtn);
		UISprite widget = m_tempObject.GetComponentInChildren<UISprite>();
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
		Destroy (m_tempObject);
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

	void Update()
	{
		if(isOpenChat)
		{
			UIYindao.m_UIYindao.CloseUI ();
			if (QXChatData.Instance.m_listScendWaitTime[chatInputIndex] > 0)
			{
				if(m_isYuyin)
				{
					m_spriteYuyinScendButton.color = Color.gray;
					m_spriteYuyinScendButton.GetComponent<BoxCollider>().enabled = false;
					
					sendCdLabel.fontSize = 24;
					sendCdLabel.gameObject.transform.localPosition = new Vector3(0, 2, 0);
					sendCdLabel.text = MyColorData.getColorString (5,QXChatData.Instance.m_listScendWaitTime[chatInputIndex] + "s");
				}
				else
				{
					sendCdLabel.fontSize = 12;
					sendCdLabel.gameObject.transform.localPosition = new Vector3(90, 22, 0);
					sendCdLabel.text = "冷却" + MyColorData.getColorString (5,QXChatData.Instance.m_listScendWaitTime[chatInputIndex] + "s");
				}
			}
			else
			{
				sendCdLabel.text = "";
				if(m_isYuyin)
				{
					m_spriteYuyinScendButton.color = Color.white;
					m_spriteYuyinScendButton.GetComponent<BoxCollider>().enabled = true;
				}
			}
		}

		if(m_isLuyining)
		{
//			Debug.Log(Time.time);
			m_iLuzhiTime = (int)(Time.time - m_fBTime);
			if(m_iLuzhiTime >= 15)
			{
				m_objLuyinTime.gameObject.SetActive(true);
				m_objLuying.gameObject.SetActive(false);
				m_labelLuyin.text = "0" + (20 - m_iLuzhiTime);
			}
			if(m_iLuzhiTime >= 20)
			{
				m_objLuying.SetActive(false);
				m_objLuyinTime.SetActive(false);
				m_isLuyining = false;
				m_iVoiceIndex ++;
				MSCController.Instance.StopMSC();
			}
		}
	}

	#region ChatPageAnimation
	public GameObject chatPageObj;
	public GameObject chatWindowBtn;
	private bool isOpenChat;
	public void ChatPageAnimation (bool isOpen)
	{
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
		Debug.Log("==========1");
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
//			gameObject.SetActive (false);
			if(m_objGotoOBj != null)
			{
				m_objGotoOBj.SetActive(true);
				m_objGotoOBj = null;
			}
			if(m_listGotoObj != null)
			{
				for(int i = 0; i < m_listGotoObj.Count; i ++)
				{
					m_listGotoObj[i].SetActive(true);
				}
			}
		}
		m_BoxCollider.enabled = isOpenChat;
	}

	public void SetSiliaoName(string name)
	{
		m_sScendName = name;
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

	public void DeleteDialogType(ChatPct.Channel channel)
	{
		for(int i = 0; i < m_listChatMessage.Count; i ++)
		{
			if(m_listChatMessage[i].chatPct.channel == channel)
			{
				m_listChatMessage.RemoveAt(i);
				i --;
			}
		}
	}

	public void AddFriend(int id0, int id1, string name0, string name1)
	{
		NewSiliaoData temp;
		int id;
		string name;
		if(id0 == JunZhuData.Instance().m_junzhuInfo.id)
		{
			id = id1;
			name = name1;
		}
		else
		{
			id = id0;
			name = name0;
		}

		temp.id = id;
		temp.name = name;
		for(int i = 0; i < m_listNewSiliaoData.Count; i ++)
		{
			if(m_listNewSiliaoData[i].id == id)
			{
				m_listNewSiliaoData.RemoveAt(i);
				break;
			}
		}
		m_listNewSiliaoData.Add(temp);
		setUpFriendList(m_listNewSiliaoData);
	}

	public void setUpFriendList(List<NewSiliaoData> listNewSiliaoData)
	{
		m_objNotFirend.SetActive(false);
		for(int i = 0; i < m_listNewSiliaoData.Count; i ++)
		{
			m_listFriendData[i].gameObject.SetActive(true);
			m_listFriendData[i].m_labelName.text = listNewSiliaoData[i].name;
		}
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("YunyinQiehuanBtn") != -1)
		{
			if(JunZhuData.Instance().m_junzhuInfo.level < 20 && chatInputIndex == 0)
			{
				ClientMain.m_UITextManager.createText("世界频道语音聊天功能在20级后开启！");
				return;
			}
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
			switch(chatInputIndex)
			{
			case 0:
			case 1:
			case 2:
				m_spriteYuyinScendButton.gameObject.transform.localPosition = new Vector3(0, 0, 0);
				m_spriteYuyinScendButton.SetDimensions(257, m_spriteYuyinScendButton.height);
				break;
			case 3:
				m_spriteYuyinScendButton.gameObject.transform.localPosition = new Vector3(28, 0, 0);
				m_spriteYuyinScendButton.SetDimensions(195, m_spriteYuyinScendButton.height);
				break;
			}
		}
		else if(ui.name.IndexOf("Kaiguan") != -1)
		{
			int index = int.Parse(ui.name.Substring(7, 1));
			m_listKaiguan[index] = !m_listKaiguan[index];
			if(m_listKaiguan[index])
			{
				m_listSpriteKaiguan[index].gameObject.transform.localPosition = new Vector3(-22, 0, 0);
			}
			else
			{
				m_listSpriteKaiguan[index].gameObject.transform.localPosition = new Vector3(22, 0, 0);
			}
			if(!m_listKaiguan[index])
			{
				switch(index)
				{
				case 0:
					DeleteDialogType(ChatPct.Channel.SHIJIE);
					break;
				case 1:
					DeleteDialogType(ChatPct.Channel.LIANMENG);
					break;
				case 2:
					DeleteDialogType(ChatPct.Channel.SILIAO);
					break;
				case 3:
					m_listChatMessage = new List<ChatMessage>();
					break;
				}
			}
			string saveString = "";
			for(int i = 0; i < m_listKaiguan.Length; i ++)
			{
				saveString += m_listKaiguan[i] ? "1" : "0";
				saveString += ",";
			}
			PlayerPrefs.SetString( "DialogSetting", saveString );
			PlayerPrefs.Save();
		}
		else if(ui.name.IndexOf("FirendElenemt") != -1)
		{
			int index = int.Parse(ui.name.Substring(13, 1));
			if(m_listNewSiliaoData.Count > index)
			{
				SetSiliaoName(m_listNewSiliaoData[index].name);
				m_iSiliaoID = m_listNewSiliaoData[index].id;
				m_objFirendPanel.SetActive(false);
			}
		}
		else if(ui.name.IndexOf("FirendButton") != -1)
		{
			m_objFirendPanel.gameObject.SetActive(!m_objFirendPanel.gameObject.activeSelf);
		}
		else if(ui.name.IndexOf("OpenSetting") != -1)
		{
			m_objSetting.SetActive(true);
			for(int i = 0; i < 4; i ++)
			{
				if(m_listKaiguan[i])
				{
					m_listSpriteKaiguan[i].gameObject.transform.localPosition = new Vector3(-22, 0, 0);
				}
				else
				{
					m_listSpriteKaiguan[i].gameObject.transform.localPosition = new Vector3(22, 0, 0);
				}
			}
		}
		else if(ui.name.IndexOf("CloseSetting") != -1)
		{
			m_objSetting.SetActive(false);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	public float m_fMaxVolume = 1.0f;
	
	public float m_fMaxEffVolume = 1.0f;
	public override void MYPress(bool isPress, GameObject ui)
	{
		if(ui.name.IndexOf("YuyinScendBtn") != -1)
		{
			if(isPress)
			{
				m_fBTime = Time.time;
				m_fNum = 0;
				m_isLuyining = true;
				m_objLuying.SetActive(true);
				for(int i = 0; i < 5; i ++)
				{
					m_listYinLiang[i].gameObject.SetActive(false);
				}
				MSCController.Instance.StartMSC(m_iVoiceIndex + "");
				m_fMaxVolume = ClientMain.m_sound_manager.m_fMaxEffVolume;
				m_fMaxEffVolume = ClientMain.m_sound_manager.m_fMaxEffVolume;
				ClientMain.m_sound_manager.setMaxVolume(0);
				ClientMain.m_sound_manager.setMaxEffVolume(0);
			}
			else
			{
				m_objLuying.SetActive(false);
				m_objLuyinTime.SetActive(false);
				m_isLuyining = false;
				m_iLuzhiTime = (int)(Time.time - m_fBTime);
				Debug.Log("press");
				Debug.Log(QXChatUIBox.m_bFocused);
				Debug.Log(QXChatUIBox.m_bPause);
				if(QXChatUIBox.m_bFocused && !QXChatUIBox.m_bPause)
				{
					if(m_iLuzhiTime < 1)
					{
						MSCController.Instance.CancelMSC();
						ClientMain.m_UITextManager.createText(LanguageTemplate.GetText(5102));
					}
					else
					{
						m_iVoiceIndex ++;
						MSCController.Instance.StopMSC();
						ClientMain.m_sound_manager.setMaxVolume(m_fMaxVolume);
						ClientMain.m_sound_manager.setMaxEffVolume(m_fMaxEffVolume);
					}
				}
				else
				{
					MSCController.Instance.CancelMSC();
				}
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
		m_fNum += delta.y;
		if(m_fNum >= 200 && m_isLuyining)
		{
			m_isLuyining = false;
			m_objLuying.SetActive(false);
			m_objLuyinTime.SetActive(false);
			MSCController.Instance.CancelMSC();
		}
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}

	public void setSiliao(long id, string name, GameObject obj)
	{
		m_objGotoOBj = obj;
		m_objGotoOBj.SetActive(false);

		SetSiliaoName(name);
		m_iSiliaoID = id;

		if(!QXChatData.Instance.SetOpenChat)
		{
			QXChatData.Instance.OpenChatPage(ChatPct.Channel.SILIAO);
		}
	}

	public void setSiliaoList(long id, string name, List<GameObject> obj)
	{
		m_listGotoObj = obj;
		for(int i = 0; i < m_listGotoObj.Count; i ++)
		{
			m_listGotoObj[i].SetActive(false);
		}
		
		SetSiliaoName(name);
		m_iSiliaoID = id;
		
		if(!QXChatData.Instance.SetOpenChat)
		{
			QXChatData.Instance.OpenChatPage(ChatPct.Channel.SILIAO);
		}
	}
	#endregion
}
