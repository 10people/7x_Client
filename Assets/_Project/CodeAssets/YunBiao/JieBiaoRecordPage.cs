using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JieBiaoRecordPage : MonoBehaviour {
		
	public GameObject grid;
	public GameObject recordItemObj;

	private List<GameObject> recordItemList = new List<GameObject> ();

	public UILabel numLabel;//记录个数

	public UILabel noRecordDes;//没有对战记录描述

	public ScaleEffectController m_ScaleEffectController;

	void Start ()
	{
//		GetRecordInfo (YunBiaoMainPage.yunBiaoMainData.m_recordRes);
//		YunBiaoMainPage.yunBiaoMainData.JieBiaoRecordReq ();
	}

	/// <summary>
	/// 获得劫镖记录
	/// </summary>
	/// <param name="recordRes">Record res.</param>
	public void GetRecordInfo (YBHistoryResp recordRes)
	{
		if (recordRes.historyList.Count != 0)
		{
			for (int i = 0;i < recordRes.historyList.Count - 1;i ++)
			{
				for (int j = 0;j < recordRes.historyList.Count - i - 1;j ++)
				{
					if (recordRes.historyList[j].time < recordRes.historyList[j + 1].time)
					{
						YBHistory tempHis = recordRes.historyList[j];
						
						recordRes.historyList[j] = recordRes.historyList[j + 1];
						
						recordRes.historyList[j + 1] = tempHis;
					}
				}
			}

			for (int i = 0;i < recordRes.historyList.Count;i ++)
			{
				GameObject recordItem = (GameObject)Instantiate (recordItemObj);

				recordItem.SetActive (true);
				recordItem.transform.parent = grid.transform;
				recordItem.transform.localPosition = new Vector3(0,-130 * i,0);
				recordItem.transform.localScale = Vector3.one;

				recordItemList.Add (recordItem);

				JieBiaoRecord record = recordItem.GetComponent<JieBiaoRecord> ();
				record.GetRecordItemInfo (recordRes.historyList[i]);
			}

			noRecordDes.text = "";
		}

		else
		{
			noRecordDes.text = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_70);//无挑战记录
		}

		numLabel.text = "条数" + recordRes.historyList.Count + "/50";

		YunBiaoMainPage.yunBiaoMainData.MoveBackToTop (grid,recordRes.historyList.Count,4);
	}

	/// <summary>
	/// 初始化记录信息
	/// </summary>
	public void InItRecordItem (YBHistoryResp recordRes)
	{
		Debug.Log ("Refresh");
		if (recordItemList.Count < recordRes.historyList.Count)
		{
			for (int i = 0;i < recordRes.historyList.Count - recordItemList.Count;i ++)
			{
				GameObject recordItem = (GameObject)Instantiate (recordItemObj);
				
				recordItem.SetActive (true);
				recordItem.transform.parent = grid.transform;
				recordItem.transform.localPosition = Vector3.zero;
				recordItem.transform.localScale = Vector3.one;
				
				recordItemList.Add (recordItem);
			}
		}
		else if (recordItemList.Count > recordRes.historyList.Count)
		{
			for (int i = 0;i < recordItemList.Count - recordRes.historyList.Count;i ++)
			{
				Destroy (recordItemList[0]);
				recordItemList.Remove (recordItemList[0]);
			}
		}

		for (int i = 0;i < recordRes.historyList.Count - 1;i ++)
		{
			for (int j = 0;j < recordRes.historyList.Count - i - 1;j ++)
			{
				if (recordRes.historyList[i].time < recordRes.historyList[i + 1].time)
				{
					YBHistory tempHis = recordRes.historyList[i];

					recordRes.historyList[i] = recordRes.historyList[i + 1];

					recordRes.historyList[i + 1] = tempHis;
				}
			}
		}

		for (int i = 0;i < recordItemList.Count;i ++)
		{
			JieBiaoRecord record = recordItemList[i].GetComponent<JieBiaoRecord> ();
			record.GetRecordItemInfo (recordRes.historyList[i]);
		}

		numLabel.text = "条数" + recordRes.historyList.Count + "/50";

		if (recordRes.historyList.Count == 0)
		{
			noRecordDes.text = "当前还没有记录！";
		}
		else
		{
			noRecordDes.text = "";
		}
	}

	/// <summary>
	/// 关闭按钮
	/// </summary>
	public void CloseBtn ()
	{
//		Destroy (this.gameObject);
		m_ScaleEffectController.OnCloseWindowClick();
		YunBiaoMainPage.yunBiaoMainData.DestroyRoot ();
	}

	public void BackBtn ()
	{
		this.gameObject.SetActive (false);
	}
}
