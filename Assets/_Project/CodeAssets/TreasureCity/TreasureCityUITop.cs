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

public class TreasureCityUITop : MonoBehaviour {

	void Start ()
	{
		InItTopUI (TreasureCityData.Instance ().GetTCityData ());
		InItTopUIYB (TreasureCityData.Instance ().GetTCityBoxData ());
		boxChatObj.alpha = 0;
		chatMsgPanel.alpha = 0;
	}

	#region Box And YuanBao Info
	private ErrorMessage boxMsg;
	private ErrorMessage ybMsg;

	public UILabel boxBoShu;
	public UILabel boxNum;

	public UILabel canGetBoxNum;
	public UILabel haveYBNum;

	public UILabel nextBox;
	
	public UILabel backTimeLabel;

	private int backCdTime;

	private string backStr = MyColorData.getColorString (1,"后自动回城");

	private bool canGetBox = true;

	/// <summary>
	/// Ins it top U.
	/// </summary>
	/// <param name="tempMsg">Temp message.</param>
	public void InItTopUI (ErrorMessage tempMsg)
	{
		boxMsg = tempMsg;

		boxBoShu.text = "宝箱波数：" + tempMsg.cmd;

		boxNum.text = "剩余宝箱：" + tempMsg.errorCode;

		nextBox.text = TreasureCityData.Instance ().nextBxSec == 0 ? "" : "下一波宝箱：" + (TreasureCityData.Instance ().nextBxSec > 10 ? TimeHelper.GetUniformedTimeString (TreasureCityData.Instance ().nextBxSec) : TreasureCityData.Instance ().nextBxSec + "秒");
//		Debug.Log ("backCdTime:" + backCdTime);

		if (canGetBox)
		{
			if (backCdTime == 0)
			{
				canGetBox = false;

				backCdTime = TreasureCityData.Instance ().staySeconds;
				StartCoroutine ("BackToCity");
//				Debug.Log ("Start");
			}
		}
	}

	IEnumerator BackToCity ()
	{
		while (backCdTime > 0)
		{
			backCdTime --;

			backTimeLabel.text = MyColorData.getColorString (backCdTime <= 30 ? 5 : 4,TimeHelper.GetUniformedTimeString (backCdTime))  +  backStr;

			yield return new WaitForSeconds (1);

			if (backCdTime == 0)
			{
				Debug.Log ("turn");
				//回主城
				CityGlobalData.m_selfNavigation = false;
				PlayerSceneSyncManager.Instance.ExitTreasureCity ();
			}
		}
	}

	/// <summary>
	/// Ins it top UIY.
	/// </summary>
	/// <param name="tempMsg">Temp message.</param>
	public void InItTopUIYB (ErrorMessage tempMsg)
	{
		ybMsg = tempMsg;

		canGetBoxNum.text = "可抢宝箱：" + tempMsg.cmd;

		haveYBNum.text = "已获元宝：" + tempMsg.errorCode;
	}

	#endregion

	#region Get Box Chat

	private List<GameObject> msgObjList = new List<GameObject> ();

	public GameObject chatLabelObj;

	public UISprite boxChatObj;
	public UIPanel chatMsgPanel;
	private float disTime = 30;

	public void GetChatMsg (string tempMsg)
	{
		boxChatObj.alpha = 1;
		chatMsgPanel.alpha = 1;
		disTime = 0;
		if (msgObjList.Count < 6)
		{
			GameObject chatMsgObj = (GameObject)Instantiate (chatLabelObj);
			chatMsgObj.SetActive (true);
			chatMsgObj.transform.parent = chatLabelObj.transform.parent;
			chatMsgObj.transform.localPosition = new Vector3(-110,msgObjList.Count == 0 ? 50 : 28,0);
			chatMsgObj.transform.localScale = Vector3.one;
			msgObjList.Add (chatMsgObj);
			UILabel label = chatMsgObj.GetComponent<UILabel> ();
			label.text = tempMsg;
		}
		else
		{
			GameObject targetObj = msgObjList[0];
			targetObj.transform.localPosition = msgObjList[5].transform.localPosition - new Vector3(0,22,0);
//			Debug.Log ("msgObjList.count1:" + msgObjList.Count);
			msgObjList.Remove (msgObjList[0]);
//			Debug.Log ("msgObjList.count2:" + msgObjList.Count);
			msgObjList.Add (targetObj);
//			Debug.Log ("msgObjList.count3:" + msgObjList.Count);
			UILabel label = targetObj.GetComponent<UILabel> ();
			label.text = tempMsg;
		}

		if (msgObjList.Count > 0)
		{
			for (int i = 0;i < msgObjList.Count;i ++)
			{
				MsgObjMove (msgObjList[i],new Vector3(-110,msgObjList.Count == 6 ? 160 - i * 22 : 160 - 22 * (6 - msgObjList.Count) - i * 22,0));
			}
		}
	}

	void MsgObjMove (GameObject obj,Vector3 pos)
	{
		Hashtable move = new Hashtable ();
		move.Add ("position",pos);
		move.Add ("easetype",iTween.EaseType.linear);
		move.Add ("time",0.1f);
		move.Add ("islocal",true);
		iTween.MoveTo (obj,move);
	}

	#endregion

	void Update ()
	{
		if (disTime < 30)
		{
			disTime += Time.deltaTime;
		}
		else
		{
			if (chatMsgPanel.alpha > 0)
			{
				boxChatObj.alpha -= 0.05f;
				chatMsgPanel.alpha -= 0.05f;
			}
		}
	}
}
