using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralRecord : GeneralInstance<GeneralRecord> {

	public delegate void RecordDelegate ();

	public RecordDelegate M_RecordDelegate;

	public enum RecordType
	{
		SPORT,//竞技
	}

	private RecordType m_recordType;

	public UIScrollView m_recordSc;
	public UIScrollBar m_recordSb;

	public UILabel m_desLabel;

	public GameObject m_recordItem;
	private List<GameObject> m_recordList = new List<GameObject>();

	private ZhandouRecordResp m_sportResp;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItRecordPage (RecordType tempType,object tempResp)
	{
		int count = 0;

		switch (tempType)
		{
		case RecordType.SPORT:
			m_sportResp = tempResp as ZhandouRecordResp;
			Debug.Log ("m_sportResp:" + m_sportResp);

			for (int i = 0;i < m_sportResp.info.Count;i ++)
			{
				for (int j = 0;j < m_sportResp.info.Count - 1 - i;j ++)
				{
					if (m_sportResp.info[j].time > m_sportResp.info[j + 1].time)
					{
						ZhandouItem tempItem = m_sportResp.info[j];
						m_sportResp.info[j] = m_sportResp.info[j + 1];
						m_sportResp.info[j + 1] = tempItem;
					}
				}
			}

			count = m_sportResp.info.Count;

			break;
		default:
			break;
		}

		CreateObjList (count);

		for (int i = 0;i < m_recordList.Count;i ++)
		{
			GeneralRecordItem recordItem = m_recordList[i].GetComponent<GeneralRecordItem> ();
			switch (tempType)
			{
			case RecordType.SPORT:
				recordItem.InItRecordItem (tempType,m_sportResp.info[i]);
				break;
			default:
				break;
			}
		}
	}

	void CreateObjList (int tempResCount)
	{
		m_recordList = QXComData.CreateGameObjectList (m_recordItem,tempResCount,m_recordList);

		m_desLabel.text = m_recordList.Count > 0 ? "" : "战斗记录为空！";

		for (int i = 0;i < m_recordList.Count;i ++)
		{
			m_recordList[i].transform.localPosition = new Vector3(0,-80 - 110 * i,0);
			m_recordSc.UpdateScrollbars (true);
		}

		m_recordSc.enabled = m_recordList.Count < 4 ? false : true;
		m_recordSb.gameObject.SetActive (m_recordList.Count < 4 ? false : true);
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			M_RecordDelegate ();

			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
