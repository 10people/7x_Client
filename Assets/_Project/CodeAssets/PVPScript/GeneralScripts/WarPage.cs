using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarPage : GeneralInstance<WarPage>, SocketProcessor {

	private MainSimpleInfoResp mainSimpleResp;

	public GameObject warItemObj;
	private List<GameObject> warList = new List<GameObject> ();

	private Dictionary<string,int> m_warItemDic = new Dictionary<string, int>()
	{
		{"War_Sport",300100},{"War_YunBiao",310},{"War_Plunder",211},
	};

	new void Awake ()
	{
		base.Awake ();
		SocketTool.RegisterMessageProcessor (this);
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
		SocketTool.UnRegisterMessageProcessor (this);
	}

	public void OpenWarSelectWindow ()
	{
		MainSimpleInfoReq mainSimpleReq = new MainSimpleInfoReq ();
		mainSimpleReq.functionType = 8;
		QXComData.SendQxProtoMessage (mainSimpleReq,ProtoIndexes.C_MAIN_SIMPLE_INFO_REQ,ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP.ToString ());
		
		//		Debug.Log ("Fight窗口信息请求" + ProtoIndexes.C_MAIN_SIMPLE_INFO_REQ);
	}

	/// <summary>
	/// Ins it war page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	void InItWarPage (MainSimpleInfoResp tempResp)
	{
		mainSimpleResp = tempResp;

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
			warList[i].name = mainSimpleResp.info[i].functionId.ToString ();
			warList[i].transform.localScale = Vector3.one;
			WarItem war = warList[i].GetComponent<WarItem> ();
			war.InItWarItem (mainSimpleResp.info[i]);
		}
		Debug.Log ("Global.m_sPanelWantRun:" + Global.m_sPanelWantRun);
		if(!string.IsNullOrEmpty (Global.m_sPanelWantRun))
		{
			if (!m_warItemDic.ContainsKey (Global.m_sPanelWantRun)) return;

			int funcId = m_warItemDic[Global.m_sPanelWantRun];

			for (int i = 0;i < warList.Count;i ++)
			{
				WarItem warItem = warList[i].GetComponent<WarItem> ();
				if (warItem.simpleInfo.functionId == funcId)
				{
					warItem.WarItemHandlerClickBack ();
				}
			}

			Global.m_sPanelWantRun = "";
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

				EveryDayShowTime.m_isLoad2 = true;
				return true;
			}
			}
		}
		
		return false;
	}

	public override void MYClick (GameObject ui)
	{
		WarItem warItem = ui.transform.parent.GetComponent<WarItem> ();
		if (warItem.simpleInfo.functionId == int.Parse (ui.transform.parent.name))
		{
			warItem.WarItemHandlerClickBack ();
		}
	}
}
