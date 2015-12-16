using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDRecordManager : MonoBehaviour,SocketProcessor {

	public GameObject recordGrid;

	public GameObject recordItem;

	private List<GameObject> itemList = new List<GameObject> ();

	private LveBattleRecordResp recordRes;

	public UILabel desLabel;

	public ScaleEffectController sEffectControl;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		LdRecordReq ();
	}

	void LdRecordReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LVE_BATTLE_RECORD_REQ,"26065");
//		Debug.Log ("掠夺记录请求：" + ProtoIndexes.LVE_BATTLE_RECORD_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LVE_BATTLE_RECORD_RESP://掠夺记录返回

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LveBattleRecordResp lueDuoRecordRes = new LveBattleRecordResp();
				
				t_qx.Deserialize(t_stream, lueDuoRecordRes, lueDuoRecordRes.GetType());

				if (lueDuoRecordRes != null)
				{
					if (lueDuoRecordRes.info == null)
					{
						lueDuoRecordRes.info = new List<LveBattleItem>();
					}
//					Debug.Log ("记录条数：" + lueDuoRecordRes.info.Count);
					recordRes = lueDuoRecordRes;
					InItRecordItem ();
				}

				return true;
			}
		}

		return false;
	}

	void InItRecordItem ()
	{
		foreach (GameObject obj in itemList)
		{
			Destroy (obj);
		}
		itemList.Clear ();

		if (recordRes.info.Count > 0)
		{
			desLabel.text = "";

			for (int i = 0;i < recordRes.info.Count - 1;i++)
			{
				for (int j = 0;j < recordRes.info.Count - i - 1;j ++)
				{
					if (recordRes.info[j].time < recordRes.info[j + 1].time)
					{
						LveBattleItem tempBattle = recordRes.info[j];
						recordRes.info[j] = recordRes.info[j + 1];
						recordRes.info[j + 1] = tempBattle;
					}
				}
			}

			for (int i = 0;i < recordRes.info.Count;i ++)
			{
				GameObject record = (GameObject)Instantiate (recordItem);
				
				record.SetActive (true);
				record.transform.parent = recordGrid.transform;
				record.transform.localPosition = new Vector3(0,-i * 130,0);
				record.transform.localScale = Vector3.one;

				itemList.Add (record);

				LDRecordItem ldRecord = itemList[i].GetComponent<LDRecordItem> ();
				ldRecord.GetRecordInfo (recordRes.info[i]);
			}

			ItemTopCol top = recordGrid.GetComponent<ItemTopCol> ();
			if (recordRes.info.Count < 4)
			{
				top.enabled = true;
			}
			else
			{
				top.enabled = false;
			}
		}
		else
		{
			desLabel.text = "掠夺记录为空";
		}

		LueDuoData.Instance.IsStop = false;
	}
	
	public void CloseBtn ()
	{
		sEffectControl.OnCloseWindowClick();
		LueDuoManager.ldManager.DestroyRoot ();
	}

	public void BackBtn ()
	{
		Destroy (this.gameObject);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
