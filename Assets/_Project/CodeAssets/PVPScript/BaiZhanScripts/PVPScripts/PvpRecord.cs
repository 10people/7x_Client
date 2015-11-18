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

	public List<EventHandler> btnHandlerList = new List<EventHandler> ();

	public ScaleEffectController sEffectControl;

	public void InItRecordPage (ZhandouRecordResp tempRecordResp)
	{
		sEffectControl.gameObject.transform.localScale = Vector3.one;

		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler += BtnHandlerCallBack;
		}

		if (tempRecordResp.info.Count == 0)
		{
			foreach (GameObject obj in recordItemList)
			{
				Destroy (obj);
			}
			recordItemList.Clear ();

			recordDes.text = "战斗记录为空！";
			return;
		}

		int recordCount = tempRecordResp.info.Count - recordItemList.Count;

		if (recordCount > 0)
		{
			int exitCount = recordItemList.Count;

			for (int i = 0;i < recordCount;i ++)
			{
				GameObject recordItem = (GameObject)Instantiate (recordItemObj);
				
				recordItem.SetActive (true);
				recordItem.transform.parent = recordItemObj.transform.parent;
				recordItem.transform.localPosition = new Vector3(0,-130 * (i + exitCount),0);
				recordItem.transform.localScale = recordItemObj.transform.localScale;

				recordItemList.Add (recordItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (recordCount);i ++)
			{
				Destroy (recordItemList[recordItemList.Count - 1]);
				recordItemList.RemoveAt (recordItemList.Count - 1);
			}
		}

		recordSc.UpdateScrollbars (true);

		for (int i = 0;i < recordItemList.Count;i ++)
		{
			PvpRecordItem record = recordItemList[i].GetComponent<PvpRecordItem> ();
			record.InItRecordItemInfo (tempRecordResp.info[i]);
		}

		recordDes.text = "";

		recordSc.enabled = recordItemList.Count < 4 ? false : true;
	}

	void BtnHandlerCallBack (GameObject obj)
	{
		Debug.Log (obj.name);
		switch (obj.name)
		{
		case "CloseBtn":

			PvpPage.pvpPage.CancelBtn ();
			sEffectControl.OnCloseWindowClick ();
			sEffectControl.CloseCompleteDelegate += DisActive;

			break;
		case "Backbtn":

			PvpPage.pvpPage.PvpActiveState (true);
			DisActive ();

			break;
		default:
			break;
		}
	}

	void DisActive ()
	{
		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler -= BtnHandlerCallBack;
		}
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
