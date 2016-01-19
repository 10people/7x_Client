using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXChatPage : MonoBehaviour {

	public static QXChatPage chatPage;

	private ChatPct.Channel chatChannel;

	private bool isDragChatPage = false;

	void Awake ()
	{
		chatPage = this;
	}

	void OnDestroy ()
	{
		chatPage = null;
	}

	#region ShowChatInfo
	public UIScrollView chatSc;
	public UIScrollBar chatSb;
	private float chatSbValue = 1;
	public UIDragScrollView dragArea;
	private int lastScHeigh;
	
	private List<GameObject> chatItemList = new List<GameObject> ();
	public GameObject chatItemObj;
	
	public List<EventHandler> chatBtnHandlerList = new List<EventHandler> ();

	/// <summary>
	/// Ins it chat page.
	/// </summary>
	/// <param name="tempChatList">Temp chat list.</param>
	public void InItChatPage (ChatPct.Channel tempChannel,List<ChatMessage> tempChatList)
	{
		chatChannel = tempChannel;

		CreateChatList (tempChatList);

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
		default:
			break;
		}

		foreach (EventHandler handler in chatBtnHandlerList)
		{
			handler.m_handler -= ChatBtnHandlerClickBack;
			handler.m_handler += ChatBtnHandlerClickBack;
		}

		foreach (EventHandler handler in sendBtnHandlerList)
		{
			handler.m_handler -= SendBtnHandlerClickBack;
			handler.m_handler += SendBtnHandlerClickBack;
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

		int firstHeigh = 0;
		int lastHeigh = 0;
		int totleHeigh = 0;
		//s:s :h + 11  p:p : h + 38  p:s : h + 21 s:p : h + 26
		for (int i = 0;i < tempChatList.Count;i ++)
		{
			ChatPlayerItem chatPlayer = chatItemList[i].GetComponent<ChatPlayerItem> ();
			chatPlayer.InItChatPlayer (tempChatList[i]);

			if (i == 0)
			{
				firstHeigh = chatPlayer.GetItemHeigh ();
				bool isPlayer = (tempChatList[i].chatPct.guoJia > 0 && tempChatList[i].chatPct.guoJia < 8) ? true : false;
				totleHeigh += (isPlayer ? 32 : 15 ) + firstHeigh;
			}
			else
			{
				int heigh = chatItemList[i - 1].GetComponent<ChatPlayerItem> ().GetItemHeigh ();
				bool isPlayer1 = (tempChatList[i - 1].chatPct.guoJia > 0 && tempChatList[i - 1].chatPct.guoJia < 8) ? true : false;
				bool isPlayer2 = (tempChatList[i].chatPct.guoJia > 0 && tempChatList[i].chatPct.guoJia < 8) ? true : false;

				if (!isPlayer1)
				{
					chatItemList[i].transform.localPosition = chatItemList[i - 1].transform.localPosition + new Vector3(0,-(heigh + (!isPlayer2 ? 11 : 26)),0);//ss:sp
					totleHeigh += (heigh + (!isPlayer2 ? 11 : 26));
				}
				else
				{
					chatItemList[i].transform.localPosition = chatItemList[i - 1].transform.localPosition + new Vector3(0,-(heigh + (!isPlayer2 ? 21 : 38)),0);//ps:pp
					totleHeigh += (heigh + (!isPlayer2 ? 21 : 38));
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

		if (!canDrag)
		{
			chatSc.ResetPosition ();
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
				int countHeigh = lastScHeigh - totleHeigh - 10;
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
	}

	void ChatBtnHandlerClickBack (GameObject obj)
	{
		SetChatItemInfoClose (false);
		switch (obj.name)
		{
		case "WorldBtn":
			if (chatChannel != ChatPct.Channel.SHIJIE)
			{
				if (!isDragChatPage)
				{
					chatSbValue = 1;
				}
				PutChatInfoInToDic (0);
				QXChatData.Instance.SetChatChannel (ChatPct.Channel.SHIJIE);
			}
			break;
		case "AllianceBtn":
			if (JunZhuData.Instance ().m_junzhuInfo.lianMengId <= 0)
			{
				ClientMain.m_UITextManager.createText (MyColorData.getColorString (5, "请先加入一个联盟！"));
				return;
			}
			if (chatChannel != ChatPct.Channel.LIANMENG)
			{
				if (!isDragChatPage)
				{
					chatSbValue = 1;
				}
				PutChatInfoInToDic (1);
				QXChatData.Instance.SetChatChannel (ChatPct.Channel.LIANMENG);
			}
			break;
		case "BroadCastBtn":
			if (chatChannel != ChatPct.Channel.Broadcast)
			{
				if (!isDragChatPage)
				{
					chatSbValue = 1;
				}
				PutChatInfoInToDic (2);
				QXChatData.Instance.SetChatChannel (ChatPct.Channel.Broadcast);
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
	public UIInput chatInput;

	public UILabel costLabel;
	public UILabel freeLabel;

	public List<EventHandler> sendBtnHandlerList = new List<EventHandler>();

	private Dictionary<int,string> chatInputDic = new Dictionary<int, string>();

	/// <summary>
	/// Puts the chat info in to dic.
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	void PutChatInfoInToDic (int tempIndex)
	{
		if (!chatInputDic.ContainsKey (tempIndex))
		{
			chatInputDic.Add (tempIndex,chatInput.value);
		}
		else
		{
			chatInputDic[tempIndex] = chatInput.value;
		}
	}

	/// <summary>
	/// Switchs the chat input.
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	void SwitchChatInput (int tempIndex)
	{
		chatInput.value = "";
		if (!chatInputDic.ContainsKey (tempIndex))
		{
			chatInputDic.Add (tempIndex,"");
		}
		else
		{
			chatInput.value = chatInputDic [tempIndex];
		}

		freeLabel.transform.parent.gameObject.SetActive (tempIndex == 1 ? false : true);
		switch (tempIndex)
		{
		case 0:
			freeLabel.text = "";
			costLabel.gameObject.SetActive (true);
			costLabel.text = "10";
			break;
		case 2:
			break;
		default:
			break;
		}
	}

	void SendBtnHandlerClickBack (GameObject obj)
	{
		SetChatItemInfoClose (false);
		switch (obj.name)
		{
		case "SendBtn":
			
			if (string.IsNullOrEmpty (chatInput.value))
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
						senderName = JunZhuData.Instance ().m_junzhuInfo.name,
						senderId = JunZhuData.Instance ().m_junzhuInfo.id,
						channel = chatChannel,
						content = chatInput.value,
						guoJia = JunZhuData.Instance ().m_junzhuInfo.guoJiaId,
						vipLevel = JunZhuData.Instance ().m_junzhuInfo.vipLv,
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

	public GameObject chatPageObj;
	public GameObject chatWindowBtn;
	private bool isOpenChat;
	public void ChatPageAnimation (bool isOpen)
	{
		isOpenChat = isOpen;

		Hashtable move = new Hashtable ();
		move.Add ("time",0.4f);
		move.Add ("position",new Vector3 (isOpen ? -120 : -450,0,0));
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
			chatSbValue = 1;
			chatSb.value = 0;
//			MainCityUI.TryRemoveFromObjectList (gameObject);
			QXChatData.Instance.SetOpenChat = false;
			gameObject.SetActive (false);
		}
	}
}
