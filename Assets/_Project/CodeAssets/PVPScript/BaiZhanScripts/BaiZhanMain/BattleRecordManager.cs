using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleRecordManager : MonoBehaviour {

	private ZhandouRecordResp m_recordResp;

	private List<GameObject> recordItemList = new List<GameObject> ();

	public GameObject recordItemObj;

	public GameObject recordGrid;

	public GameObject recordDes;

	void Start ()
	{
		m_recordResp = BattleRecord.recordData.recordResp;

		InItRecordPage ();
	}

	void InItRecordPage ()
	{
		foreach (GameObject tempItem in recordItemList)
		{
			Destroy (tempItem);
		}
		recordItemList.Clear ();

		if (m_recordResp.info.Count > 0)
		{
			recordDes.SetActive (false);

			ItemTopCol topCol = recordGrid.GetComponent<ItemTopCol> ();
			if (m_recordResp.info.Count > 3)
			{
				topCol.enabled = false;
			}
			else
			{
				topCol.enabled = true;
			}

			for (int i = 0;i < m_recordResp.info.Count - 1;i ++)
			{
				for (int j = 0;j < m_recordResp.info.Count - i - 1;j ++)
				{
					if (m_recordResp.info[j].time > m_recordResp.info[j + 1].time)
					{
						ZhandouItem tempItem = m_recordResp.info[j];

						m_recordResp.info[j] = m_recordResp.info[j + 1];

						m_recordResp.info[j + 1] = tempItem;
					}
				}
			}

			StartCoroutine (CreateRecordItem ());
		}

		else
		{
			recordDes.SetActive (true);
		}
	}

	IEnumerator CreateRecordItem ()
	{
		for (int i = 0;i < m_recordResp.info.Count;i ++)
		{
			GameObject recordItem = (GameObject)Instantiate (recordItemObj);

			recordItem.SetActive (true);
			recordItem.name = "RecordItem" + (i + 1);

			recordItem.transform.parent = recordGrid.transform;

			recordItem.transform.localPosition = new Vector3(0,-130 * i,0);

			recordItem.transform.localScale = recordItemObj.transform.localScale;

			BattleRecordItem battleRecord = recordItem.GetComponent<BattleRecordItem> ();
			battleRecord.recordInfo = m_recordResp.info[i];
			battleRecord.InItRecordItemInfo ();

			yield return new WaitForSeconds (0.1f);
		}
	}

	//返回按钮
	public void BackBtn ()
	{
		BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
		BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
		Destroy (this.gameObject);
	}
	
	//关闭按钮
	public void DestroyBaiZhanRoot ()
	{
		BaiZhanData.Instance ().CloseBaiZhan ();
	}
}
