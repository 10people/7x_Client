using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceFightRecord : MonoBehaviour,SocketProcessor {

	public GameObject currentObj;
	public GameObject currentGrid;
	public GameObject currentItem;
	private List<GameObject> currentItemList = new List<GameObject> ();

	public GameObject lastObj;
	public GameObject lastGrid;
	public GameObject lastItem;
	private List<GameObject> lastItemList = new List<GameObject> ();

	public ScaleEffectController m_ScaleEffectController;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		CurrentReq ();
		NextReq ();
	}

	//本届战况请求
	void CurrentReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.ALLIANCE_FIGHT_HISTORY_REQ,"4210");
	}
	//上届排名请求
	void NextReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.ALLIANCE_FIGTH_LASTTIME_RANK,"4212");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ALLIANCE_FIGHT_HISTORY_RESP://联盟战历史战况结果返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FightHistoryResp currentFightResp = new FightHistoryResp();
				
				t_qx.Deserialize(t_stream, currentFightResp, currentFightResp.GetType());

				if (currentFightResp != null)
				{
					Debug.Log ("本届战况：" + currentFightResp.historyInfos.Count);
					InItCurrentPage (currentFightResp);
				}

				return true;
			}
			case ProtoIndexes.ALLIANCE_FIGTH_LASTTIME_RANK_RESP://联盟战上届排名返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FightLasttimeRankResp lastFightResp = new FightLasttimeRankResp();
				
				t_qx.Deserialize(t_stream, lastFightResp, lastFightResp.GetType());

				if (lastFightResp != null)
				{
					Debug.Log ("上届排名：" + lastFightResp.rankInfos.Count);
					InItLastPage (lastFightResp);
				}

				return true;
			}
			}
		}
		return false;
	}

	//本届战况显示
	void InItCurrentPage (FightHistoryResp tempResp)
	{
		if (tempResp.historyInfos.Count == 0)
		{
			//没有战况
			Debug.Log ("本届没有战况");
			return;
		}

		if (currentItemList.Count == 0)
		{
			for (int i = 0;i < tempResp.historyInfos.Count;i ++)
			{
				GameObject currentObj = (GameObject)Instantiate (currentItem);

				currentObj.SetActive (true);
				currentObj.transform.parent = currentGrid.transform;
				currentObj.transform.localPosition = new Vector3(0,-60 * i,0);
				currentObj.transform.localScale = Vector3.one;

				currentItemList.Add (currentObj);
			}
		}

		for (int i = 0;i < tempResp.historyInfos.Count;i ++)
		{
			AllianceCurrentItem allianceCurrent = currentItemList[i].GetComponent<AllianceCurrentItem> ();
			allianceCurrent.GetCurrentInfo (tempResp.historyInfos[i]);
		}
	}

	//上届战况显示
	void InItLastPage (FightLasttimeRankResp tempResp)
	{
		if (tempResp.rankInfos.Count == 0)
		{
			//没有战况
			Debug.Log ("上届没有战况");
			return;
		}
		
		if (lastItemList.Count == 0)
		{
			for (int i = 0;i < tempResp.rankInfos.Count;i ++)
			{
				GameObject lastObj = (GameObject)Instantiate (lastItem);
				
				lastObj.SetActive (true);
				lastObj.transform.parent = lastGrid.transform;
				lastObj.transform.localPosition = new Vector3(0,-60 * i,0);
				lastObj.transform.localScale = Vector3.one;
				
				lastItemList.Add (lastObj);
			}
		}
		
		for (int i = 0;i < tempResp.rankInfos.Count;i ++)
		{
			AllianceLastItem allianceLast = lastItemList[i].GetComponent<AllianceLastItem> ();
			allianceLast.GetLastItemInfo (tempResp.rankInfos[i]);
		}
	}

	//本届战况按钮
	public void CurrentBtn ()
	{
		currentObj.SetActive (true);
		lastObj.SetActive (false);
//		CurrentReq ();
	}

	//上街排名按钮
	public void LastBtn ()
	{
		currentObj.SetActive (false);
		lastObj.SetActive (true);
	}

	public void BackBtn ()
	{
		Destroy (this.gameObject);
	}

	public void ClostBtn ()
	{
		AllianceFightMainPage.fightMainPage.CloseBtn ();
		m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
		m_ScaleEffectController.OnCloseWindowClick();
	}
	void OnCloseWindow ()
	{
		Destroy (this.gameObject);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
