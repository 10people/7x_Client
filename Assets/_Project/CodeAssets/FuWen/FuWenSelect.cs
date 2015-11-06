using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenSelect : MonoBehaviour {

	private int curItemId;//符石itemid

	public UIScrollView selectSc;
	public UIScrollBar selectSb;
	public GameObject fuShiGrid;
	public GameObject fuShiItemObj;
	private List<GameObject> fuShiItemList = new List<GameObject> ();

	public UILabel titleLabel;

	public UILabel desLabel;

	public enum SelectType
	{
		XIANGQIAN,
		HECHENG,
	}
	private SelectType selectType = SelectType.XIANGQIAN;//符石选择入口

	/// <summary>
	/// 镶嵌符石的时候需要符石信息，符石镶嵌栏位id
	/// </summary>
	private Fuwen fuWenInfo;
	public Fuwen GetFuWenInfo
	{
		set{fuWenInfo = value;}
	}
	private int lanWeiId;
	public int GetXiangQianLanWeiId
	{
		set{lanWeiId = value;}
	}

	private bool isSelect = false;//是否已选择符石
	public bool IsSelect
	{
		set{isSelect = value;}
		get{return isSelect;}
	}

	private float selectSbValue;

	//获得选择符石列表
	public void GetSelectFuWenInfo (SelectType tempType,List<Fuwen> tempList)
	{
		selectType = tempType;

		switch (tempType)
		{
		case SelectType.XIANGQIAN:

			titleLabel.text = "镶嵌";

			desLabel.text = tempList.Count > 0 ? "" : "没有可镶嵌的符石";

			break;

		case SelectType.HECHENG:
			
			titleLabel.text = "选择符石";

			desLabel.text = tempList.Count > 0 ? "" : "没有可合成的符石";
			
			break;

		default:
			break;
		}
		int selectCount = tempList.Count - fuShiItemList.Count;
		if (selectCount > 0)
		{
			for (int i = 0;i < selectCount;i ++)
			{
				GameObject fuShiItem = (GameObject)Instantiate (fuShiItemObj);
				
				fuShiItem.SetActive (true);
				fuShiItem.transform.parent = fuShiGrid.transform;
				fuShiItem.transform.localPosition = Vector3.zero;
				fuShiItem.transform.localScale = Vector3.one;
				fuShiItemList.Add (fuShiItem);
				selectSc.UpdateScrollbars (true);
				fuShiGrid.GetComponent<UIGrid> ().repositionNow = true;
			}
		}
		else if (selectCount < 0)
		{
			for (int i = 0;i < Mathf.Abs (selectCount);i ++)
			{
				Destroy (fuShiItemList[0]);
				fuShiItemList.RemoveAt (0);
				selectSc.UpdateScrollbars (true);
				fuShiGrid.GetComponent<UIGrid> ().repositionNow = true;
			}
		}
		selectSb.value = selectSbValue;
		for (int i = 0;i < tempList.Count;i ++)
		{
			FuWenSelectItem fuWenSelect = fuShiItemList[i].GetComponent<FuWenSelectItem> ();
			fuWenSelect.GetFuWenInfo (tempList[i]);
		}

		fuShiGrid.transform.parent.GetComponent<UIScrollView> ().enabled = tempList.Count < 4 ? false : true;

		if(FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100300];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}

		isSelect = false;
	}

	//刷新选择的符石
	public void RefreshSelectFuShiItem (int itemId)
	{
		curItemId = itemId;

		for (int i = 0;i < fuShiItemList.Count;i ++)
		{
			FuWenSelectItem fuWenItem = fuShiItemList[i].GetComponent<FuWenSelectItem> ();
			fuWenItem.IsCurFuShi (itemId);
		}
	}

	public void CloseBtn ()
	{
		if (curItemId != 0)
		{
			switch (selectType)
			{
			case SelectType.XIANGQIAN:

				FuWenMainPage.fuWenMainPage.CurXiangQianId = curItemId;
				//发送镶嵌符石请求
				FuWenData.Instance.FuWenOperateReq (FuWenData.FuWenOperateType.EQUIP_FUWEN,fuWenInfo.itemId,lanWeiId);
				
				break;
				
			case SelectType.HECHENG:
				
				FuWenMainPage.fuWenMainPage.CurHeChengItemId = curItemId;
				FuWenMainPage.fuWenMainPage.ShowMixBtns ();
				FuWenMainPage.fuWenMainPage.FxController (FuWenMixBtn.FxType.OPEN);
				FuWenMainPage.fuWenMainPage.EffectPanel (true);

				break;
				
			default:
				break;
			}
		}
		else
		{
			FuWenMainPage.fuWenMainPage.IsBtnClick = false;
		}
		selectSbValue = selectSb.value;
		gameObject.SetActive (false);
	}
}
