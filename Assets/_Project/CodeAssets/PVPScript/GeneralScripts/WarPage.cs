using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarPage : MonoBehaviour, SocketProcessor {

	public static WarPage warPage;

	private MainSimpleInfoResp mainSimpleResp;

	public GameObject warItemObj;
	private List<GameObject> warList = new List<GameObject> ();

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		warPage = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void OnDestroy ()
	{
		warPage = null;
		SocketTool.UnRegisterMessageProcessor (this);
	}

	/// <summary>
	/// Ins it war page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void InItWarPage (MainSimpleInfoResp tempResp)
	{
		mainSimpleResp = tempResp;
		sEffectController.OnOpenWindowClick ();

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100370,1);

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
			switch (mainSimpleResp.info[i].functionId)
			{
			case 310:
				warList[i].transform.localPosition = new Vector3(100,0,0);
				break;
			case 211:
				warList[i].transform.localPosition = new Vector3(200,0,0);
				break;
			case 300100:
				warList[i].transform.localPosition = new Vector3(0,0,0);
				break;
			default:
				break;
			}

			warList[i].transform.localScale = Vector3.one;
			WarItem war = warList[i].GetComponent<WarItem> ();
			war.InItWarItem (mainSimpleResp.info[i]);
		}
	}

	public void CheckRedPoint ()
	{
		for (int i = 0;i < warList.Count;i ++)
		{
			WarItem war = warList[i].GetComponent<WarItem> ();
			war.CheckRedPoint ();
		}
	}

	/// <summary>
	/// Sets the plunder times.
	/// </summary>
	/// <param name="countNum">Count number.</param>
	/// <param name="totleNum">Totle number.</param>
	public void SetPlunderTimes (int countNum,int totleNum)
	{
		for (int i = 0;i < mainSimpleResp.info.Count;i ++)
		{
			if (mainSimpleResp.info[i].functionId == 211)
			{
				mainSimpleResp.info[i].num1 = countNum;
				mainSimpleResp.info[i].num2 = totleNum;
				
//				Debug.Log ("refresh:" + mainSimpleResp.info[i].num1 + "||" + mainSimpleResp.info[i].num2);
			}
		}

		for (int i = 0;i < warList.Count;i ++)
		{
			WarItem war = warList[i].GetComponent<WarItem> ();
			war.InItWarItem (mainSimpleResp.info[i]);
		}
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP:
			{
				Debug.Log ("Fight窗口信息返回" + ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP);
				
				MainSimpleInfoResp mainSimpleRes = new MainSimpleInfoResp();
				mainSimpleRes = QXComData.ReceiveQxProtoMessage (p_message,mainSimpleRes) as MainSimpleInfoResp;
				
				if (mainSimpleRes != null)
				{
					if (mainSimpleRes.info == null)
					{
						mainSimpleRes.info = new List<SimpleInfo>();
					}
					mainSimpleResp = mainSimpleRes;
					
					InItWarPage (mainSimpleResp);
				}
				return true;
			}
			}
		}
		
		return false;
	}
}
