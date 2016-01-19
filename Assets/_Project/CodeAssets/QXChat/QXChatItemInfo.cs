using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXChatItemInfo : MonoBehaviour {

	private List<ChatBtnInfo> chatBtnInfoList;

	public delegate void ChatBtnDelegate ();
	public struct ChatBtnInfo
	{
		public string btnString;
		public ChatBtnDelegate chatBtnDelegate;
	}

	public UISprite chatBtnBg;

	public GameObject chatBtnObj;
	private List<GameObject> chatBtnList = new List<GameObject>();

	private int dis = 55;

	public void InItChatBtn (List<ChatBtnInfo> tempInfoList)
	{
		chatBtnInfoList = tempInfoList;

		chatBtnBg.height = 70 + (tempInfoList.Count - 1) * dis;

		foreach (GameObject obj in chatBtnList)
		{
			Destroy (obj);
		}
		chatBtnList.Clear ();

		for (int i = 0;i < tempInfoList.Count;i ++)
		{
			GameObject chatBtn = (GameObject)Instantiate (chatBtnObj);

			chatBtn.name = i.ToString ();
			chatBtn.SetActive (true);

			chatBtn.transform.parent = chatBtnObj.transform.parent;
			chatBtn.transform.localPosition = new Vector3(95,-35 - dis * i,0);
			chatBtn.transform.localScale = Vector3.one;

			chatBtnList.Add (chatBtn);

			UILabel btnLabel = chatBtn.GetComponentInChildren<UILabel> ();
			btnLabel.text = tempInfoList[i].btnString;

			EventHandler handler = chatBtn.GetComponent<EventHandler> ();
			handler.m_handler -= ChatBtnHandlerClickBack;
			handler.m_handler += ChatBtnHandlerClickBack;
		}
	}

	void ChatBtnHandlerClickBack (GameObject obj)
	{
		chatBtnInfoList [int.Parse (obj.name)].chatBtnDelegate ();
		QXChatPage.chatPage.SetChatItemInfoClose (true);
	}
}
