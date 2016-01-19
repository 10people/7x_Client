using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarPage : MonoBehaviour {

	public static WarPage warPage;

	private MainSimpleInfoResp mainSimpleResp;

	public GameObject warItemObj;
	private List<GameObject> warList = new List<GameObject> ();
	
	public List<EventHandler> closeHandlerList = new List<EventHandler>();

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		warPage = this;
	}

	void OnDestroy ()
	{
		warPage = null;
	}

	/// <summary>
	/// Ins it war page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void InItWarPage (MainSimpleInfoResp tempResp)
	{
		mainSimpleResp = tempResp;
		sEffectController.OnOpenWindowClick ();

		int warCount = mainSimpleResp.info.Count - warList.Count;
		if (warCount > 0)
		{
			for (int i = 0;i < warCount;i ++)
			{
				GameObject warItem = (GameObject)Instantiate (warItemObj);
				warItem.SetActive (true);
				warItem.transform.parent = warItemObj.transform.parent;
				warItem.transform.localPosition = Vector3.zero;
				warItem.transform.localScale = Vector3.one;
				
				warList.Add (warItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (warCount);i ++)
			{
				Destroy (warList[warList.Count - 1]);
				warList.RemoveAt (warList.Count - 1);
			}
		}

		for (int i = 0;i < mainSimpleResp.info.Count;i ++)
		{
			warList[i].transform.localPosition = new Vector3(i * 180 - (mainSimpleResp.info.Count - 1) * 90,0,0);
			warList[i].transform.localScale = Vector3.one;
			WarItem war = warList[i].GetComponent<WarItem> ();
			war.InItWarItem (mainSimpleResp.info[i]);
		}

		foreach (EventHandler handler in closeHandlerList)
		{
			handler.m_handler -= WarCloseHandlerClickBack;
			handler.m_handler += WarCloseHandlerClickBack;
		}
	}

	public void WarCloseHandlerClickBack (GameObject obj)
	{
		CheckRedPoint ();
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Shows the war red.
	/// </summary>
	public void CheckRedPoint ()
	{
		foreach (GameObject obj in warList)
		{
			WarItem war = obj.GetComponent<WarItem> ();
			if (war.GetShowRed)
			{
				PushAndNotificationHelper.SetRedSpotNotification (220,true);//or 313
				break;
			}
			else
			{
				PushAndNotificationHelper.SetRedSpotNotification (220,false);
			}
		}
	}
}
