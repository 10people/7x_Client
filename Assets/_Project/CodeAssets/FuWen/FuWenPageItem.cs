using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenPageItem : MonoBehaviour {

	private List<FuwenLanwei> fuWenLanWeiList = new List<FuwenLanwei> ();//当前页的符文栏位
	private List<FuwenLanwei> bigFuWenList = new List<FuwenLanwei> ();//大符文栏位
	private List<FuwenLanwei> smallFuWenList = new List<FuwenLanwei> ();//小符文栏位

	private List<int> bigItemIdList = new List<int> ();//镶嵌的大符文itemIdList
	private List<int> smallItemIdList = new List<int> ();//镶嵌的小符文itemIdList

	public GameObject fuShiGrid;
	public GameObject fuShiItemObj;//符石Item
	private List<GameObject> fuShiItemList = new List<GameObject> ();

	public UILabel effectLabel_1;
	public UILabel effectLabel_2;

	/// <summary>
	/// 初始化符文页
	/// </summary>
	public void InItFuWenPageInfo (int pageIndex,List<FuwenLanwei> tempLanWeiList)
	{
		//获得此符文页的符文栏位list
		int lanWeiCount = tempLanWeiList.Count / 3;
		int index = (pageIndex - 1) * lanWeiCount;
		Debug.Log ("index:" + index);

		if (fuShiItemList.Count == 0)
		{
			for (int i = index;i < lanWeiCount + index;i ++)
			{
				FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWeiList[i].lanweiId);
				float posX = fuWenOpenTemp.positionX;
				float posY = fuWenOpenTemp.positionY;
				float posZ = fuWenOpenTemp.positionZ;
				
				Vector3 pos = new Vector3(posX,posY,posZ);
				
				GameObject fuShiItem = (GameObject)Instantiate (fuShiItemObj);
				
				fuShiItem.SetActive (true);
				fuShiItem.transform.parent = fuShiGrid.transform;
				fuShiItem.transform.localPosition = pos;
				fuShiItem.transform.localScale = Vector3.one;
				
				fuShiItemList.Add (fuShiItem);
			}
		}

		fuWenLanWeiList.Clear ();
		bigFuWenList.Clear ();
		smallFuWenList.Clear ();

		bigItemIdList.Clear ();
		smallItemIdList.Clear ();

		for (int i = index;i < lanWeiCount + index;i ++)
		{
			fuWenLanWeiList.Add (tempLanWeiList[i]);
		}

		//对符文分类
		for (int i = 0;i < fuWenLanWeiList.Count;i ++)
		{
			FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (fuWenLanWeiList[i].lanweiId);
			if (fuWenOpenTemp.lanweiType < 10)
			{
				bigFuWenList.Add (fuWenLanWeiList[i]);
				if (fuWenLanWeiList[i].itemId > 0)
				{
					bigItemIdList.Add (fuWenLanWeiList[i].itemId);
				}
			}
			else
			{
				smallFuWenList.Add (fuWenLanWeiList[i]);
				if (fuWenLanWeiList[i].itemId > 0)
				{
					smallItemIdList.Add (fuWenLanWeiList[i].itemId);
				}
			}
		}
//		Debug.Log ("BigFuWenList:" + bigFuWenList.Count);
//		Debug.Log ("SmallFuWenList:" + smallFuWenList.Count);

		List<int> m_ExceptList = new List<int> ();//已镶嵌的主属性符石类型list
		List<int> g_ExceptList = new List<int> ();//已镶嵌的高级属性符石类型list
		
		//筛选主属性中已镶嵌符石的符文栏位上符石itemidlist
		for (int i = 0;i < bigItemIdList.Count;i ++)
		{
			FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (bigItemIdList[i]);
			int fuShiTypeId = fuWenTemp.shuxing;
			if (!m_ExceptList.Contains (fuShiTypeId))
			{
				m_ExceptList.Add (fuShiTypeId);
			}
//			Debug.Log ("m_fuShiTypeId:" + fuShiTypeId);
		}
		
		//筛选高级属性中已镶嵌符石的符文栏位上符石itemidlist
		for (int i = 0;i < smallItemIdList.Count;i ++)
		{
			FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (smallItemIdList[i]);
			int fuShiTypeId = fuWenTemp.shuxing;
			if (!g_ExceptList.Contains (fuShiTypeId))
			{
				g_ExceptList.Add (fuShiTypeId);
			}
//			Debug.Log ("g_fuShiTypeId:" + fuShiTypeId);
		}

		for (int i = index;i < lanWeiCount + index;i ++)
		{
			FuShiItem fuShi = fuShiItemList[i - index].GetComponent<FuShiItem> ();
			fuShi.GetLanWeiInfo (tempLanWeiList[i],m_ExceptList,g_ExceptList);
		}

		ShowFuShiEffect (bigItemIdList,bigFuWenList.Count,effectLabel_1,"宝石","主");
		ShowFuShiEffect (smallItemIdList,smallFuWenList.Count,effectLabel_2,"符文","高级");
	}

	/// <summary>
	/// 符石套装效果显示
	/// </summary>
	void ShowFuShiEffect (List<int> tempItemList,int totleNum,UILabel tempLabel,string typeStr1,string typeStr2)
	{
		int minLevel = 0;//最低等级
		string des = "";
		
		if (tempItemList.Count == totleNum)
		{
			int minItemId = 0;

			minItemId = tempItemList[0];
			for (int i = 0;i < tempItemList.Count;i ++)
			{
				if (tempItemList[i] <= minItemId)
				{
					minItemId = tempItemList[i];
				}
			}
//			Debug.Log ("minItemId:" + minItemId);

			FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (minItemId);
			minLevel = fuWenTemp.fuwenLevel;
//			Debug.Log ("minLevel:" + minLevel);

			FuWenJiaChengTemplate fuWenJiaChengTemp = FuWenJiaChengTemplate.GetFuWenJiaChengTemplateById (minLevel);
			float xiShu = fuWenJiaChengTemp.addition / 100;

			des = "(" + minLevel + "级)" + "+" + xiShu + "%" + typeStr2 + "属性";

			tempLabel.GetComponent<LabelColorTool> ().m_ColorID = 4;
		}
		else
		{
			des = "  嵌满可获得属性加成";
			tempLabel.GetComponent<LabelColorTool> ().m_ColorID = 8;
		}

		tempLabel.text = typeStr1 + "套装效果：" + tempItemList.Count + "/" + totleNum + des;
	}
}
