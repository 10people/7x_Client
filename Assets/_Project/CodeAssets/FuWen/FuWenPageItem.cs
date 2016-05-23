//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//
//using ProtoBuf;
//using qxmobile.protobuf;
//using ProtoBuf.Meta;
//
//public class FuWenPageItem : MonoBehaviour {
//	
//	public GameObject fuShiGrid;
//	public GameObject fuShiItemObj;//符石Item
//	private List<GameObject> fuShiItemList = new List<GameObject> ();
//
//	private List<int> existList = new List<int> ();//已镶嵌的符石属性 list
//
//	/// <summary>
//	/// 初始化符文页
//	/// </summary>
//	public void InItFuWenPageInfo (int pageIndex,List<FuwenLanwei> tempLanWeiList)
//	{
//		//获得此符文页的符文栏位list
//		int lanWeiCount = tempLanWeiList.Count / 3;
//		int index = (pageIndex - 1) * lanWeiCount;
////		Debug.Log ("index:" + index);
//
//		fuShiItemList = QXComData.CreateGameObjectList (fuShiItemObj,lanWeiCount,fuShiItemList);
//
//		existList.Clear ();
//
//		for (int i = index;i < lanWeiCount + index;i ++)
//		{
//			FuWenOpenTemplate fuWenOpenTemp = FuWenOpenTemplate.GetFuWenOpenTemplateByLanWeiId (tempLanWeiList[i].lanweiId);
//			float posX = fuWenOpenTemp.positionX;
//			float posY = fuWenOpenTemp.positionY;
//			float posZ = fuWenOpenTemp.positionZ;
//			
//			Vector3 pos = new Vector3(posX,posY,posZ);
//			fuShiItemList[i - index].transform.parent = fuShiGrid.transform;
//			fuShiItemList[i - index].transform.localPosition = pos;
//
//			if (tempLanWeiList[i].itemId > 0)
//			{
//				existList.Add (FuWenTemplate.GetFuWenTemplateByFuWenId (tempLanWeiList[i].itemId).shuxing);
//			}
//		}
//
//		foreach (int i in existList)
//		{
////			Debug.Log ("shuxing:" + i);
//		}
//
//		for (int i = index;i < lanWeiCount + index;i ++)
//		{
//			FuShiItem fuShi = fuShiItemList[i - index].GetComponent<FuShiItem> ();
//			fuShi.GetLanWeiInfo (tempLanWeiList[i],existList);
//		}
//	}
//}
