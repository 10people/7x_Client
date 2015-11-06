using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDAllianceManager : MonoBehaviour {

	public UIScrollView sc;
	public UIScrollBar sb;

	public GameObject itemObj;

	public GameObject grid;

	private List<LianMengInfo> allianceList = new List<LianMengInfo>();

	private List<GameObject> itemList = new List<GameObject> ();

	public UILabel desLabel;

	private bool canTap = false;
	public bool SetCanTap
	{
		set{canTap = value;}
	}

	public static bool isRefreshToTop = true;

	/// <summary>
	/// 获得联盟list，创建足够覆盖scrollview的item
	/// </summary>
	public void GetAllianceList (List<LianMengInfo> tempList)
	{
		allianceList = tempList;
//		Debug.Log ("allianceList.count:" + allianceList.Count);

		foreach (GameObject obj in itemList)
		{
			Destroy (obj);
		}
		itemList.Clear ();

		for (int i = 0;i < tempList.Count;i ++)
		{
			GameObject item = (GameObject)Instantiate (itemObj);
			
			item.SetActive (true);
			item.transform.parent = grid.transform;
			item.transform.localPosition = new Vector3(0,-i * 75,0);
			item.transform.localScale = Vector3.one;
			
			itemList.Add (item);

//			grid.GetComponent<UIGrid> ().Reposition ();
			sc.UpdateScrollbars(true);
			sb.value = isRefreshToTop ? 0.0f : 1.0f;

			LDAllianceItem ldAlliance = item.GetComponent<LDAllianceItem> ();
			ldAlliance.GetLdAllianceInfo (tempList[i]);
		}

//		Debug.Log ("sb.value" + sb.value);

		if (tempList.Count > 0)
		{
			desLabel.text = "";
			ShowSelectBox (tempList[0].mengId);
		}
		else
		{
			desLabel.text = "这个国家暂时还没有联盟！";
		}

//		ItemTopCol itemTopCol = grid.GetComponent<ItemTopCol> ();
//		if (tempList.Count < 5)
//		{
//			itemTopCol.enabled = true;
//		}
//		else
//		{
//			itemTopCol.enabled = false;
//		}
		canTap = true;
	}

	/// <summary>
	/// 显示点选框
	/// </summary>
	/// <param name="obj">某个gameobject对象</param>
	public void ShowSelectBox (int allianceId)
	{
		LueDuoData.Instance.SetAllianceId = allianceId;
		for (int i = 0;i < itemList.Count;i ++)
		{
			LDAllianceItem ldAlliance = itemList[i].GetComponent<LDAllianceItem> ();

			if (allianceId == allianceList[i].mengId)
			{
				ldAlliance.SetBoxShow = true;
				ldAlliance.selectBox.SetActive (true);
			}
			else
			{
				ldAlliance.SetBoxShow = false;
				ldAlliance.selectBox.SetActive (false);
			}
		}
	}

	void Update ()
	{
//		Debug.Log ("sc.GetSingleScrollViewValue():" + sc.GetSingleScrollViewValue());
		float temp = sc.GetSingleScrollViewValue();
		if (temp == -100) return;

		if (LueDuoData.Instance.GetAllianceStartPage == 1)
		{
			if (allianceList.Count >= 20)
			{
				if (temp > 1.25f && canTap)
				{
//					Debug.Log ("temp11111 + UP + LueDuoData.Instance.GetAllianceStartPage:" + temp + LueDuoData.Instance.GetAllianceStartPage);
					isRefreshToTop = true;
					LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.Alliance,LueDuoData.Instance.GetNationId,
					                                   LueDuoData.Instance.GetAllianceId,
					                                   LueDuoData.Instance.GetAllianceStartPage,
					                                   LueDuoData.Direction.Up);
					canTap = false;
				}
			}
		}
		else if (LueDuoData.Instance.GetAllianceStartPage > 1)
		{
			if (allianceList.Count >= 20)
			{
				if (temp > 1.25f && canTap)
				{
//					Debug.Log ("temp22222 + UP + LueDuoData.Instance.GetAllianceStartPage:" + temp + LueDuoData.Instance.GetAllianceStartPage);
					isRefreshToTop = true;
					LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.Alliance,LueDuoData.Instance.GetNationId,
					                                   LueDuoData.Instance.GetAllianceId,
					                                   LueDuoData.Instance.GetAllianceStartPage,
					                                   LueDuoData.Direction.Up);
					canTap = false;
				}
				else if (temp < -0.25f && canTap)
				{
//					Debug.Log ("temp33333 + Down + LueDuoData.Instance.GetAllianceStartPage:" + temp + LueDuoData.Instance.GetAllianceStartPage);
					isRefreshToTop = false;
					LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.Alliance,LueDuoData.Instance.GetNationId,
					                                   LueDuoData.Instance.GetAllianceId,
					                                   LueDuoData.Instance.GetAllianceStartPage,
					                                   LueDuoData.Direction.Down);
					canTap = false;
				}
			}
			else
			{
				if (temp < -0.25f && canTap)
				{
//					Debug.Log ("temp4444 + Down + LueDuoData.Instance.GetAllianceStartPage:" + temp + LueDuoData.Instance.GetAllianceStartPage);
					isRefreshToTop = false;
					LueDuoData.Instance.LueDuoNextReq (LueDuoData.ReqType.Alliance,LueDuoData.Instance.GetNationId,
					                                   LueDuoData.Instance.GetAllianceId,
					                                   LueDuoData.Instance.GetAllianceStartPage,
					                                   LueDuoData.Direction.Down);
					canTap = false;
				}
			}
		}
	}
}
