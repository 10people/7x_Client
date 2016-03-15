using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXChatUIBox : MonoBehaviour {

	public static QXChatUIBox chatUIBox;

	public GameObject chatUIBoxObj;
	public GameObject redObj;

	public UILabel chatLabel;
	private List<GameObject> chatLabelList = new List<GameObject> ();

	public EventHandler chatUIBoxHandler;

	public EventHandler situationHandler;

	void Awake ()
	{
		chatUIBox = this;
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
	}

	/// <summary>
	/// Ins it chat user interface box.
	/// </summary>
	/// <param name="tempChatMsg">Temp chat message.</param>
	public void InItChatUIBox (ChatMessage tempChatMsg)
	{
		SetRedAlert (!QXChatData.Instance.SetOpenChat);
		GameObject chatLabelItem = (GameObject)Instantiate (chatLabel.gameObject);
		chatLabelItem.SetActive (true);
		chatLabelItem.transform.parent = chatLabel.transform.parent;
		chatLabelItem.transform.localPosition = new Vector3 (-180,40,0);
		chatLabelItem.transform.localScale = Vector3.one;

		UILabel mChatLabel = chatLabelItem.GetComponent<UILabel> ();
		string chatText = "";
		Debug.Log ("tempChatMsg.chatPct.content:" + tempChatMsg.chatPct.content);
		if (tempChatMsg.chatPct.guoJia <= 0)
		{
			chatText = (tempChatMsg.chatPct.channel == ChatPct.Channel.Broadcast ? "[00e1c4][广播][-]" : MyColorData.getColorString (5,"[系统]")) + tempChatMsg.chatPct.content;
		}
		else
		{
			string channelStr = "[00e1c4][" + QXChatData.Instance.GetChannelTitleStr (tempChatMsg.chatPct.channel) + "][-]";
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
			Hashtable move = new Hashtable();
			move.Add ("time",0.1f);
			move.Add ("position",obj.transform.localPosition - new Vector3(0,chatLabelHeigh,0));
			move.Add ("islocal",true);
			move.Add ("easetype",iTween.EaseType.easeOutQuad);
			iTween.MoveTo (obj,move);
		}

		chatLabelList.Add (chatLabelItem);

		if (chatLabelList.Count > 4)
		{
			Destroy (chatLabelList[0]);
			chatLabelList.RemoveAt (0);
		}
	}

	public void ChatUIBoxClickBack (GameObject obj)
	{
		QXChatData.Instance.OpenChatPage ();
		SetRedAlert (false);
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
			situationHandler.gameObject.SetActive (FunctionOpenTemp.IsShowRedSpotNotification (410012) || FunctionOpenTemp.IsShowRedSpotNotification (410015));
		}
	}

	void SituationHandlerClickBack (GameObject obj)
	{
		if (FunctionOpenTemp.IsShowRedSpotNotification (410012))
		{
			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.PLUNDER);
			return;
		}
		else if (FunctionOpenTemp.IsShowRedSpotNotification (410015))
		{
			WarSituationData.Instance.OpenWarSituation (WarSituationData.SituationType.YUNBIAO);
		}
	}
}
