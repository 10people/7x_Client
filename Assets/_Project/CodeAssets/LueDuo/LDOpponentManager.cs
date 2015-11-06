using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LDOpponentManager : MonoBehaviour {

	public GameObject itemObj;
	
	public GameObject grid;

	public List<JunZhuInfo> junZhuInfoList = new List<JunZhuInfo> ();

	private List<JunZhuInfo> allJunZhuList = new List<JunZhuInfo> ();//所有君主
	private List<JunZhuInfo> unProtectList = new List<JunZhuInfo>();//未在保护期内的玩家
	private List<JunZhuInfo> protectList = new List<JunZhuInfo> ();//保护期内的玩家

	private List<GameObject> itemList = new List<GameObject> ();

	public UILabel desLabel;

	/// <summary>
	/// 获得君主list，创建足够覆盖scrollview的item
	/// </summary>
	public void GetJunZhuInfoList (List<JunZhuInfo> tempList)
	{
		junZhuInfoList = tempList;

		foreach (GameObject obj in itemList)
		{
			Destroy (obj);
		}
		itemList.Clear ();

		//创建item
		for (int i = 0;i < tempList.Count;i ++)
		{
			GameObject item = (GameObject)Instantiate (itemObj);
			
			item.SetActive (true);
			item.transform.parent = grid.transform;
			item.transform.localPosition = new Vector3 (0,-i * 100,0);
			item.transform.localScale = Vector3.one;
			
			itemList.Add (item);

			grid.transform.parent.GetComponent<UIScrollView> ().UpdateScrollbars(true);
		}

		if (tempList.Count > 0)
		{
			desLabel.text = "";
		}
		else
		{
			desLabel.text = "暂时还没有可掠夺的对手！";
		}

//		ItemTopCol itemTopCol = grid.GetComponent<ItemTopCol> ();
//		if (tempList.Count < 4)
//		{
//			itemTopCol.enabled = true;
//		}
//		else
//		{
//			itemTopCol.enabled = false;
//		}

		InItJunZhuInfoList ();
	}

	void InItJunZhuInfoList ()
	{
		allJunZhuList.Clear ();
		unProtectList.Clear ();
		protectList.Clear ();
		
		//区分是否处于保护期的玩家
		for (int i = 0;i < junZhuInfoList.Count;i ++)
		{
			if (junZhuInfoList[i].leftProtectTime > 0)
			{
				protectList.Add (junZhuInfoList[i]);
			}
			else
			{
				unProtectList.Add (junZhuInfoList[i]);
			}
		}
		
		//对未处于保护期的玩家做排序
		for (int i = 0;i < unProtectList.Count - 1;i ++)
		{
			for (int j = 0;j < unProtectList.Count - i - 1;j ++)
			{
				if (unProtectList[j].gongjin < unProtectList[j + 1].gongjin)
				{
					JunZhuInfo tempInfo = unProtectList[j];
					unProtectList[j] = unProtectList[j + 1];
					unProtectList[j + 1] = tempInfo;
				}
				else if (unProtectList[j].gongjin == unProtectList[j + 1].gongjin)
				{
					if (unProtectList[j].zhanli < unProtectList[j + 1].zhanli)
					{
						JunZhuInfo tempInfo = unProtectList[j];
						unProtectList[j] = unProtectList[j + 1];
						unProtectList[j + 1] = tempInfo;
					}
					else if (unProtectList[j].zhanli == unProtectList[j + 1].zhanli)
					{
						if (unProtectList[j].remainHp < unProtectList[j + 1].remainHp)
						{
							JunZhuInfo tempInfo = unProtectList[j];
							unProtectList[j] = unProtectList[j + 1];
							unProtectList[j + 1] = tempInfo;
						}
					}
				}
			}
		}
		for (int i = 0;i < unProtectList.Count;i ++)
		{
			allJunZhuList.Add (unProtectList[i]);
		}
		
		for (int i = 0;i < protectList.Count - 1;i ++)
		{
			for (int j = 0;j < protectList.Count - i - 1;j ++)
			{
				if (protectList[i].leftProtectTime > protectList[j + 1].leftProtectTime)
				{
					JunZhuInfo tempInfo = protectList[j];
					protectList[j] = protectList[j + 1];
					protectList[j + 1] = tempInfo;
				}
			}
		}
		for (int i = 0;i < protectList.Count;i ++)
		{
			allJunZhuList.Add (protectList[i]);
		}
		
		for (int i = 0;i < allJunZhuList.Count;i ++)
		{
			LDOpponentItem ldOpponent = itemList[i].GetComponent<LDOpponentItem> ();
			ldOpponent.GetLDOpponentInfo (allJunZhuList[i]);
		}
	}
}
