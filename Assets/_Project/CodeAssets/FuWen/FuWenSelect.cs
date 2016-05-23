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
//public class FuWenSelect : MonoBehaviour {
//
//	public static FuWenSelect fuWenSelect;
//
//	public UIScrollView selectSc;
//	public UIScrollBar selectSb;
//	public GameObject fuShiGrid;
//	public GameObject fuShiItemObj;
//	private List<GameObject> fuShiItemList = new List<GameObject> ();
//
//	public UILabel titleLabel;
//
//	public UILabel desLabel;
//
//	public enum SelectType
//	{
//		XIANGQIAN,
//		HECHENG,
//	}
//	private SelectType selectType = SelectType.XIANGQIAN;//符石选择入口
//
//	public List<EventHandler> selectHandlerList = new List<EventHandler> ();
//
//	private FuwenLanwei lanWeiInfo;
//	private int curItemId;//符石itemid
//
//	public ScaleEffectController sEffectController;
//
//	void Awake ()
//	{
//		fuWenSelect = this;
//	}
//
//	void OnDestroy ()
//	{
//		fuWenSelect = null;
//	}
//
//	/// <summary>
//	/// Gets the select fu wen info.
//	/// </summary>
//	/// <param name="tempType">Temp type.</param>
//	/// <param name="tempList">Temp list.</param>
//	public void GetSelectFuWenInfo (SelectType tempType,List<Fuwen> tempList,FuwenLanwei tempLanWeiInfo)
//	{
//		sEffectController.OnOpenWindowClick ();
//
//		selectType = tempType;
//		lanWeiInfo = tempLanWeiInfo;
//
//		titleLabel.text = tempType == SelectType.HECHENG ? "选择符石" : "镶嵌";
//		titleLabel.GetComponent<UILabelType> ().init ();
//
//		desLabel.text = tempList.Count > 0 ? "" : LanguageTemplate.GetText (LanguageTemplate.Text.SET_1);
//
//		fuShiItemList = QXComData.CreateGameObjectList (fuShiItemObj,tempList.Count,fuShiItemList);
//
//		List<Fuwen> sortList = new List<Fuwen> ();
//		for (int i = 0;i < tempList.Count;i ++)
//		{
//			if (tempList[i].itemId == tempLanWeiInfo.itemId)
//			{
//				sortList.Add (tempList[i]);
//			}
//		}
//
//		for (int i = 0;i < tempList.Count - 1;i ++)
//		{
//			for (int j = 0;j < tempList.Count - i - 1;j ++)
//			{
//				if (FuWenTemplate.GetFuWenTemplateByFuWenId (tempList[j].itemId).fuwenLevel < FuWenTemplate.GetFuWenTemplateByFuWenId (tempList[j + 1].itemId).fuwenLevel)
//				{
//					Fuwen tempFuWen = tempList[j];
//					tempList[j] = tempList[j + 1];
//					tempList[j + 1] = tempFuWen;
//				}
//			}
//		}
//
//		for (int i = 0;i < tempList.Count;i ++)
//		{
//			if (tempList[i].itemId != tempLanWeiInfo.itemId)
//			{
//				sortList.Add (tempList[i]);
//			}
//		}
//
//		for (int i = 0;i < sortList.Count;i ++)
//		{
//			fuShiItemList[i].transform.localPosition = new Vector3(0,-107 * i,0);
//			selectSc.UpdateScrollbars (true);
//
//			FuWenSelectItem fuWenSelect = fuShiItemList[i].GetComponent<FuWenSelectItem> ();
//			fuWenSelect.GetFuWenInfo (tempType,sortList[i],tempLanWeiInfo);
//		}
//
//		selectSc.enabled = tempList.Count <= 4 ? false : true;
//		selectSb.gameObject.SetActive (tempList.Count <= 4 ? false : true);
//
//		foreach (EventHandler handler in selectHandlerList)
//		{
//			handler.m_click_handler -= CloseBtn;
//			handler.m_click_handler += CloseBtn;
//		}
//
//		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100470,4);
//	}
//
//	public void CloseBtn (GameObject obj)
//	{
//		FuWenMainPage.fuWenMainPage.IsBtnClick = false;
//		gameObject.SetActive (false);
//	}
//}
