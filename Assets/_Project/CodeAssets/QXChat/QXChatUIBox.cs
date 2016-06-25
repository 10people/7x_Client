//#define CloneBox
#define ShowChatMsgInOneLabel
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXChatUIBox : MYNGUIPanel {

	public static QXChatUIBox chatUIBox;

	public GameObject chatUIBoxObj;
	public GameObject redObj;

	public UILabel chatLabel;
	private List<GameObject> chatLabelList = new List<GameObject> ();

	public EventHandler chatUIBoxHandler;

	public EventHandler situationHandler;
	public UISprite lightBox;

	public UISprite m_spriteLianmengSwitch;
	public GameObject m_objYuyinScend;

	private bool isShow = false;
	public enum ShowState
	{
		COUNT,
		ADD,
	}
	private ShowState showState = ShowState.COUNT;

	private List<string> chatMsgList = new List<string>();

	void Awake ()
	{
		chatUIBox = this;

		UIWidget widget = chatUIBoxHandler.GetComponent<UIWidget> ();
		widget.alpha = chatLabel.text != "" ? 1.0f : 0.5f;
	}

	void OnDestroy ()
	{
		chatUIBox = null;
	}

	void Start ()
	{
		SetRedAlert (false);
		chatUIBoxHandler.m_click_handler += ChatUIBoxClickBack;
		situationHandler.m_click_handler += SituationHandlerClickBack;
		if(Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_BATTLE)
		{
			m_spriteLianmengSwitch.gameObject.SetActive(true);
			m_objYuyinScend.SetActive(true);
			chatUIBoxObj.transform.localPosition = new Vector3(480 + ClientMain.m_iMoveX, 27, 0);
		}
		else
		{
			m_spriteLianmengSwitch.gameObject.SetActive(false);
			m_objYuyinScend.SetActive(false);
			chatUIBoxObj.transform.localPosition = new Vector3(240, 27, 0);
		}
//		PushAndNotificationHelper.SetRedSpotNotification (410012,true);
	}

	/// <summary>
	/// Ins it chat user interface box.
	/// </summary>
	/// <param name="tempChatMsg">Temp chat message.</param>
	public void InItChatUIBox (ChatMessage tempChatMsg)
	{
		SetRedAlert (!QXChatData.Instance.SetOpenChat);
//		Debug.Log ("chatPct.guoJia:" + tempChatMsg.chatPct.guoJia);
		#if ShowChatMsgInOneLabel
		string chatText = "";
		if (tempChatMsg.chatPct.guoJia == 100)
		{
			chatText = (tempChatMsg.chatPct.channel == ChatPct.Channel.Broadcast ? "[00e1c4][广播][-]" : MyColorData.getColorString (5,"[系统]")) + tempChatMsg.chatPct.content;
		}
		else
		{
			string channelStr = "[00e1c4][" + QXChatData.Instance.GetChannelNameStr (tempChatMsg.chatPct.channel) + "][-]";
			string nationStr = "[e5e205][" + QXComData.GetNationName (tempChatMsg.chatPct.guoJia) + "][-]";
			string nameStr = "[f5aa29]" + tempChatMsg.chatPct.senderName + "：[-]";
			chatText = channelStr + nationStr + nameStr + tempChatMsg.chatPct.content;
//			chatText = chatText.Replace (" ","");
			chatText = chatText.Replace ("\n","");
		}

		chatText = tempChatMsg.chatPct.type == 2 ? chatText + "[u][e15a00]快来看看吧[-][/u]" + "！" : chatText;

		chatMsgList.Add (chatText);

		for (int i = 0;i < chatMsgList.Count;i ++)
		{
			if (chatMsgList.Count > 3)
			{
				chatMsgList.RemoveAt (0);
			}
		}

		for (int i = 0;i < chatMsgList.Count;i ++)
		{
//			Debug.Log ("chatMsgList:" + chatMsgList[i]);
			if (chatMsgList.Count == 1)
			{
				chatLabel.text = chatMsgList[chatMsgList.Count - 1];
			}
			else if (chatMsgList.Count > 1)
			{
				chatLabel.text = chatMsgList[chatMsgList.Count - 1] + "\n" + chatMsgList[chatMsgList.Count - 2];
			}
		}

		UIWidget widget = chatUIBoxHandler.GetComponent<UIWidget> ();
		
		widget.alpha = chatLabel.text != "" ? 1.0f : 0.5f;

		#endif

		#if CloneBox
		GameObject chatLabelItem = (GameObject)Instantiate (chatLabel.gameObject);
		chatLabelItem.SetActive (true);
		chatLabelItem.transform.parent = chatLabel.transform.parent;
		chatLabelItem.transform.localPosition = new Vector3 (-180,40,0);
		chatLabelItem.transform.localScale = Vector3.one;

		UILabel mChatLabel = chatLabelItem.GetComponent<UILabel> ();
		string chatText = "";
//		Debug.Log ("tempChatMsg.chatPct.content:" + tempChatMsg.chatPct.content);
		if (tempChatMsg.chatPct.guoJia <= 0)
		{
			chatText = (tempChatMsg.chatPct.channel == ChatPct.Channel.Broadcast ? "[00e1c4][广播][-]" : MyColorData.getColorString (5,"[系统]")) + tempChatMsg.chatPct.content;
		}
		else
		{
			string channelStr = "[00e1c4][" + QXChatData.Instance.GetChannelNameStr (tempChatMsg.chatPct.channel) + "][-]";
			string nationStr = "[e5e205][" + QXComData.GetNationName (tempChatMsg.chatPct.guoJia) + "][-]";
			string nameStr = "[f5aa29]" + tempChatMsg.chatPct.senderName + "[-]";
			chatText = channelStr + nationStr + nameStr + "" + tempChatMsg.chatPct.content;
			chatText = chatText.Replace (" ","");
			chatText = chatText.Replace ("\n","");
		}
		mChatLabel.text = chatText;

		int chatLabelHeigh = mChatLabel.height;

		foreach (GameObject obj in chatLabelList)
		{
			obj.transform.localPosition -= new Vector3(0,chatLabelHeigh,0);
//			Hashtable move = new Hashtable();
//			move.Add ("time",0.1f);
//			move.Add ("position",obj.transform.localPosition - new Vector3(0,chatLabelHeigh,0));
//			move.Add ("islocal",true);
//			move.Add ("easetype",iTween.EaseType.easeOutQuad);
//			iTween.MoveTo (obj,move);
		}

		chatLabelList.Add (chatLabelItem);

		if (chatLabelList.Count > 4)
		{
			Destroy (chatLabelList[0]);
			chatLabelList.RemoveAt (0);
		}
		#endif
	}

	public void ChatUIBoxClickBack (GameObject obj)
	{
		if (!MainCityUI.IsWindowsExist ())
		{
			ChatPct.Channel tempChannel = ChatPct.Channel.SHIJIE;
			foreach (KeyValuePair<ChatPct.Channel,string[]> pair in QXChatData.Instance.GetChatInfoDic ())
			{
				if (pair.Value[2] == "0")
				{
					tempChannel = pair.Key;
					break;
				}
			}
			QXChatData.Instance.OpenChatPage (tempChannel);
			SetRedAlert (false);
		}
	}

	/// <summary>
	/// Clears the chat user interface box.
	/// </summary>
	public void ClearChatUIBox ()
	{
		foreach (GameObject obj in chatLabelList)
		{
			Destroy (obj);
		}
		chatLabelList.Clear ();
	}

	/// <summary>
	/// Sets the chat user interface box position.
	/// </summary>
	/// <param name="isUp">If set to <c>true</c> is up.</param>
	public void SetChatUIBoxPos (bool isUp)
	{
		chatUIBoxObj.transform.localPosition = new Vector3 (0,isUp ? 112 : 40,0);
	}

	/// <summary>
	/// Sets the red alert.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	public void SetRedAlert (bool isActive)
	{
		redObj.SetActive (isActive);
	}
	
	/// <summary>
	/// Sets the state of the situation.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	public void SetSituationState ()
	{
		if (MainCityUI.m_MainCityUI != null)
		{
			bool plunderCheck = FunctionOpenTemp.IsShowRedSpotNotification (410012) && FunctionOpenTemp.IsHaveID (410012);
			bool yunBiaoCheck = FunctionOpenTemp.IsShowRedSpotNotification (410015) && FunctionOpenTemp.IsHaveID (410015);
			isShow = (plunderCheck || yunBiaoCheck) && !AllianceData.Instance.IsAllianceNotExist;
			situationHandler.gameObject.SetActive (isShow);
		}
		else
		{
			isShow = false;
		}
	}

	void SituationHandlerClickBack (GameObject obj)
	{
		bool plunderCheck = FunctionOpenTemp.IsShowRedSpotNotification (410012) && FunctionOpenTemp.IsHaveID (410012);
		bool yunBiaoCheck = FunctionOpenTemp.IsShowRedSpotNotification (410015) && FunctionOpenTemp.IsHaveID (410015);
		if (plunderCheck)
		{
			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.PLUNDER);
			return;
		}
		else if (yunBiaoCheck)
		{
			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.YUNBIAO);
		}
	}

	void Update ()
	{
		if(QXChatPage.chatPage != null && m_spriteLianmengSwitch != null)
		{
			m_spriteLianmengSwitch.spriteName = "lianmeng" + (QXChatPage.chatPage.m_listKaiguan[1] == true ? "open" : "close");
		}
		if (isShow)
		{
			if (showState == ShowState.ADD)
			{
				if (lightBox.alpha < 1)
				{
					lightBox.alpha += 0.08f;
				}
				else if (lightBox.alpha >= 1)
				{
					showState = ShowState.COUNT;
				}
			}
			else if (showState == ShowState.COUNT)
			{
				if (lightBox.alpha > 0)
				{
					lightBox.alpha -= 0.08f;
				}
				else if (lightBox.alpha <= 0)
				{
					showState = ShowState.ADD;
				}
			}
		}

		if(!QXChatPage.chatPage.m_isPlaying && QXChatPage.chatPage.m_listKaiguan[3] && !SceneManager.IsInLoadingScene() && !ClientMain.m_ClientMain.m_SoundPlayEff.m_isPlay)
		{
			if(QXChatPage.chatPage.m_listChatMessage.Count > 0)
			{
				QXChatPage.chatPage.m_isPlaying = true;
				MSCPlayer.Instance.PlaySound(QXChatPage.chatPage.m_listChatMessage[0].chatPct.seq, QXChatPage.chatPage.m_listChatMessage[0].chatPct.channel);
				QXChatPage.chatPage.m_listChatMessage[0].isPlay = false;
				for(int i = 0; i < QXChatPage.chatPage.chatItemList.Count; i ++)
				{
					ChatPlayerItem tempItem = QXChatPage.chatPage.chatItemList[i].GetComponent<ChatPlayerItem>();
					
					if(QXChatPage.chatPage.m_listChatMessage[0].chatPct.seq == tempItem.chatMsg.chatPct.seq)
					{
						tempItem.chatMsg.isPlay = false;
						tempItem.m_spriteRed.gameObject.SetActive(false);
						break;
					}
				}
				QXChatPage.chatPage.m_listChatMessage.RemoveAt(0);
			}
		}
	}
	public static bool m_bFocused = true;
	public static bool m_bPause = false;
	void OnApplicationFocus( bool p_focused )
	{
		m_bFocused = p_focused;
	}
	
	void OnApplicationPause(bool p_pause)
	{
		m_bPause = p_pause;
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name == "LianmengSwitch")
		{
			QXChatPage.chatPage.m_listKaiguan[1] = !QXChatPage.chatPage.m_listKaiguan[1];
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
				QXChatData.Instance.SetChatChannel (ChatPct.Channel.LIANMENG);
			}
			QXChatPage.chatPage.MYPress(isPress, ui);
		}
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		QXChatPage.chatPage.MYondrag(delta);
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}
