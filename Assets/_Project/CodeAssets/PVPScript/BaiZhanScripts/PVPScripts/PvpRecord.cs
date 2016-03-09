using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PvpRecord : MonoBehaviour {

	private ZhandouRecordResp m_recordResp;
	
	private List<GameObject> recordItemList = new List<GameObject> ();

	public UIScrollView recordSc;
	public UIScrollBar recordSb;
	public GameObject recordItemObj;

	public UILabel recordDes;

	public GameObject topRightObj;

	public List<EventHandler> btnHandlerList = new List<EventHandler> ();

	void Start ()
	{
		QXComData.LoadYuanBaoInfo (topRightObj);
	}

	public void InItRecordPage (ZhandouRecordResp tempRecordResp)
	{
		recordItemList = QXComData.CreateGameObjectList (recordItemObj,tempRecordResp.info.Count,recordItemList);

		for (int i = 0;i < recordItemList.Count;i ++)
		{
			recordItemList[i].transform.localPosition = new Vector3(0,-113 * i,0);
			recordSc.UpdateScrollbars (true);
			PvpRecordItem record = recordItemList[i].GetComponent<PvpRecordItem> ();
			record.InItRecordItemInfo (tempRecordResp.info[i]);
		}

		recordSc.enabled = recordItemList.Count < 5 ? false : true;
		recordSb.gameObject.SetActive (recordItemList.Count < 5 ? false : true);

		recordDes.text = recordItemList.Count == 0 ? "战斗记录为空！" : "";

		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_click_handler -= BtnHandlerCallBack;
			handler.m_click_handler += BtnHandlerCallBack;
		}
	}

	void BtnHandlerCallBack (GameObject obj)
	{
//		Debug.Log (obj.name);
		switch (obj.name)
		{
		case "CloseBtn":

//			PvpPage.pvpPage.CancelBtn ();
//			sEffectControl.OnCloseWindowClick ();
//			sEffectControl.CloseCompleteDelegate += DisActive;
			DisActive ();

			break;
		case "Backbtn":

			DisActive ();

			break;
		default:
			break;
		}
	}

	void DisActive ()
	{
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
