using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopPage : MonoBehaviour {

	public static ShopPage shopPage;

	public GameObject shopMainPage;
	public GameObject sellPage;

	public UILabel titleLabel;

	public List<EventHandler> otherBtnList = new List<EventHandler>();

	public ScaleEffectController sEffectController;

	private ShopData.ShopPageType spType = ShopData.ShopPageType.MAIN_PAGE;

	void Awake ()
	{
		shopPage = this;
	}

	void Start ()
	{
//		if (shopBtnList.Count == 0)
//		{
//			EventHandler handler0 = shopBtnObj.GetComponent<EventHandler> ();
//			handler0.name = "1";
//			UILabel btn0Label = handler0.GetComponentInChildren<UILabel> ();
//			btn0Label.text = ShopData.Instance ().ShopBtnName (1);
//			shopBtnList.Add (handler0);
//			for (int i = 0;i < 5;i ++)
//			{
//				GameObject shopBtn = (GameObject)Instantiate (shopBtnObj);
//				
//				shopBtn.transform.parent = shopBtnObj.transform.parent;
//				shopBtn.transform.localPosition = new Vector3(0,-75 * (i + 1),0);
//				shopBtn.transform.localScale = Vector3.one;
//				shopBtn.gameObject.name = (i + 2).ToString ();
//				UILabel btnLabel = shopBtn.GetComponentInChildren<UILabel> ();
//				btnLabel.text = ShopData.Instance ().ShopBtnName (i + 2);
//				
//				EventHandler shopBtnHandler = shopBtn.GetComponent<EventHandler> ();
//				shopBtnList.Add (shopBtnHandler);
//			}
//		}
		QXComData.LoadYuanBaoInfo (shopMainPage.transform.parent.gameObject);
		foreach (EventHandler handler in otherBtnList)
		{
			handler.m_handler += OtherHandlerClickBack;
		}
	}

	/// <summary>
	/// Switchs the page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	void SwitchPage (ShopData.ShopPageType tempType)
	{
		spType = tempType;
		ShopData.Instance.SetShopPageType (tempType);

		titleLabel.text = spType == ShopData.ShopPageType.MAIN_PAGE ? "商铺" : "出售";
		shopMainPage.SetActive (tempType == ShopData.ShopPageType.MAIN_PAGE ? true : false);
		sellPage.SetActive (tempType == ShopData.ShopPageType.SELL_PAGE ? true : false);

		if (spType == ShopData.ShopPageType.MAIN_PAGE)
		{
			Debug.Log ("Open");
			ShopData.Instance.OpenShop (shopType);
		}
	}

	#region ShopMainPage
	private ShopResp shopResp;

	public GameObject moneyInfoObj;
	public UISprite moneyIcon;
	public UILabel moneyLabel;

	private List<EventHandler> shopBtnList = new List<EventHandler>();
	public GameObject shopBtnObj;

	private ShopData.ShopType shopType;

	public UIScrollView goodScrollView;
	public UIScrollBar goodSb;
	public GameObject goodGrid;
	public GameObject goodItemObj;
	private List<GameObject> goodObjList = new List<GameObject> ();

	public GameObject shopBuyWindow;

	private float goodSbValue;

	/// <summary>
	/// Ins it shop page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempShopRes">Temp shop res.</param>
	public void InItShopPage (ShopData.ShopType tempType,ShopResp tempShopRes)
	{
		shopResp = tempShopRes;

		if (ShopData.Instance.IsOpenFirstTime)
		{
			sEffectController.OnOpenWindowClick ();
			ShopData.Instance.IsOpenFirstTime = false;
		}

		if (shopBtnList.Count == 0)
		{
			EventHandler handler0 = shopBtnObj.GetComponent<EventHandler> ();
			handler0.name = "1";
			handler0.m_handler += ShopBtnClickBack;
			shopBtnList.Add (handler0);

			UILabel btn0Label = handler0.GetComponentInChildren<UILabel> ();
			btn0Label.text = ShopData.Instance.ShopBtnName (1);


			for (int i = 0;i < 5;i ++)
			{
				GameObject shopBtn = (GameObject)Instantiate (shopBtnObj);

				shopBtn.transform.parent = shopBtnObj.transform.parent;
				shopBtn.transform.localPosition = new Vector3(0,-75 * (i + 1),0);
				shopBtn.transform.localScale = Vector3.one;
				shopBtn.gameObject.name = (i + 2).ToString ();

				UILabel btnLabel = shopBtn.GetComponentInChildren<UILabel> ();
				btnLabel.text = ShopData.Instance.ShopBtnName (i + 2);

				EventHandler shopBtnHandler = shopBtn.GetComponent<EventHandler> ();
				shopBtnList.Add (shopBtnHandler);
				shopBtnHandler.m_handler += ShopBtnClickBack;
			}
		}

		moneyInfoObj.SetActive (tempType == ShopData.ShopType.ORDINARY ? false : true);
		if (tempType == ShopData.ShopType.MYSTRERT)
		{
			moneyIcon.transform.parent.gameObject.SetActive (false);
		}
		else
		{
			moneyIcon.transform.parent.gameObject.SetActive (true);
			moneyIcon = QXComData.MoneySprite (ShopData.Instance.MoneyType (tempType),moneyIcon);
			moneyLabel.text = tempShopRes.money.ToString ();
		}

		ShopBtnState (tempType);

		CreateGoodList ();
	}

	/// <summary>
	/// Shops the state of the button.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	void ShopBtnState (ShopData.ShopType tempType)
	{
		if (shopType != tempType)
		{
			goodSbValue = 0;

			shopType = tempType;
			Debug.Log ("ShopType:" + shopType);
			for (int i = 0;i < shopBtnList.Count;i ++)
			{
				UISprite btnSprite = shopBtnList[i].GetComponent<UISprite> ();
				btnSprite.color = (int)tempType == i + 1 ? Color.white : Color.gray;
				
				UILabel btnLabel = shopBtnList[i].GetComponentInChildren<UILabel> ();
				btnLabel.color = (int)tempType == i + 1 ? Color.white : Color.gray;
			}
		}
	}

	void ShopBtnClickBack (GameObject obj)
	{
		Debug.Log (obj.name + "||" + ((int)shopType).ToString ());
		if (((int)shopType).ToString () != obj.name)
		{
			int btnIndex = int.Parse (obj.name);
			ShopData.ShopType type = (ShopData.ShopType)Enum.ToObject (typeof(ShopData.ShopType),btnIndex);
			
			ShopBtnState (type);
			ShopData.Instance.OpenShop (type);
		}
	}

	/// <summary>
	/// Creates the good list.
	/// </summary>
	void CreateGoodList ()
	{
		int goodsCount = shopResp.goodsInfos.Count - goodObjList.Count;
		Debug.Log ("shopResp.goodsInfos.Count:" + shopResp.goodsInfos.Count + "\ngoodObjList.Count:" + goodObjList.Count
		           + "\ngoodsCount:" + goodsCount + "\nMathf.Abs (goodsCount):" + Mathf.Abs (goodsCount));
		if (goodsCount > 0)
		{
			for (int i = 0;i < goodsCount;i ++)
			{
				GameObject goodItem = (GameObject)Instantiate (goodItemObj);
				goodItem.SetActive (true);
				goodItem.transform.parent = goodGrid.transform;
				goodItem.transform.localPosition = Vector3.zero;
				goodItem.transform.localScale = Vector3.one;
				goodObjList.Add (goodItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (goodsCount);i ++)
			{
				Destroy (goodObjList[goodObjList.Count - 1]);
				goodObjList.Remove (goodObjList[goodObjList.Count - 1]);
			}
		}
		goodGrid.GetComponent<UIGrid> ().repositionNow = true;

		for (int i = 0;i < shopResp.goodsInfos.Count;i ++)
		{
			goodSb.value = goodSbValue;
			goodScrollView.UpdateScrollbars (true);
			GoodItem good = goodObjList[i].GetComponent<GoodItem> ();
			good.GetDuiHuanInfo (shopType,shopResp.goodsInfos[i]);
		}
	}

	/// <summary>
	/// Opens the shop buy window.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void OpenShopBuyWindow (ShopGoodInfo tempInfo)
	{
		shopBuyWindow.SetActive (true);
		ShopBuyWindow.shopBuyWindow.GetBuyGoodInfo (tempInfo);
	}

	#endregion

	#region SellPage

	private List<BagItem> bagItemList = new List<BagItem> ();
	private List<GameObject> bagObjList = new List<GameObject> ();

	public UIScrollView bagSc;
	public UIScrollBar bagSb;
	public UIGrid bagGrid;

	public EventHandler sellGoodBtn;
	public UILabel desLabel;
	private readonly int[] posY = new int[]{155,90,-155};

	private Dictionary<long,SellGoodsInfo> sellDic = new Dictionary<long, SellGoodsInfo>();
	private int sellMoney;

	public UILabel sellLabel;

	/// <summary>
	/// Ins it bag sell page.
	/// </summary>
	/// <param name="tempSellState">Temp sell state.0,1</param>
	public void InItBagSellPage (int tempSellState)
	{
		bagItemList.Clear ();
		sellDic.Clear ();
		sellMoney = 0;

		foreach (BagItem b in BagData.Instance().m_bagItemList)
		{
			if (b.itemType == 21)//玉玦
			{
				bagItemList.Add(b);
			}
		}

		bagSc.transform.parent.gameObject.SetActive (bagItemList.Count > 0 ? true : false);

		int bagItemCount = bagItemList.Count - bagObjList.Count;
		if (bagItemCount > 0)
		{
			for (int i = 0;i < bagItemCount;i ++)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
				                        LoadIconSamplePrefab);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (bagItemCount);i ++)
			{
				Destroy (bagObjList[bagObjList.Count - 1]);
				bagObjList.RemoveAt (bagObjList.Count - 1);
			}
		}

		for (int i = 0;i < bagObjList.Count;i ++)
		{
			bagObjList[i].SetActive (true);
			bagGrid.repositionNow = true;

			ShopBagItem bagItem = bagObjList[i].GetComponent<ShopBagItem>() ?? bagObjList[i].AddComponent<ShopBagItem>();
			bagItem.GetBagItemInfo (bagItemList[i]);
		}

		bagSc.enabled = bagItemList.Count > 10 ? true : false;
		bagSb.enabled = bagItemList.Count > 10 ? true : false;
		RefreshSellState (bagObjList.Count > 0 ? 1 : 2);

		switch (tempSellState)
		{
		case 0:
			sellLabel.text = bagItemList.Count > 0 ? 
				LanguageTemplate.GetText ((LanguageTemplate.Text)226) : LanguageTemplate.GetText ((LanguageTemplate.Text)228);
			break;
		case 1:
			sellLabel.text = LanguageTemplate.GetText ((LanguageTemplate.Text)227);
			break;
		default:
			break;
		}

		sellGoodBtn.m_handler -= SellGoodBtnClickBack;
		sellGoodBtn.m_handler += SellGoodBtnClickBack;
	}
	
	void LoadIconSamplePrefab (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		GameObject bagIconSample = (GameObject)Instantiate (p_object);

		bagIconSample.transform.parent = bagGrid.transform;
		bagIconSample.transform.localPosition = Vector3.zero;

		bagObjList.Add (bagIconSample);
	}

	/// <summary>
	/// Gets the sell bag item.
	/// </summary>
	/// <param name="tempItem">Temp item.</param>
	/// <param name="type">Type.</param>
	public void GetSellBagItem (BagItem tempItem,int type)
	{
		ItemTemp itemTemp = ItemTemp.getItemTempById (tempItem.itemId);

		switch (type)
		{
		case 1://增加

			if (!sellDic.ContainsKey (tempItem.dbId))
			{
				SellGoodsInfo sellGood = new SellGoodsInfo();
				sellGood.bagId = tempItem.dbId;
				sellGood.count = 1;
				sellDic.Add (tempItem.dbId,sellGood);
			}
			else
			{
				sellDic[tempItem.dbId].count += 1;
			}

			sellMoney += itemTemp.sellNum;

			break;
		case 2://减少

			if (sellDic.ContainsKey (tempItem.dbId))
			{
				sellDic[tempItem.dbId].count -= 1;
				if (sellDic[tempItem.dbId].count <= 0)
				{
					sellDic.Remove (tempItem.dbId);
				}

				sellMoney -= itemTemp.sellNum;
			}

			break;
		default:
			break;
		}

		if (sellDic.Count == 0)
		{
			RefreshSellState (1);
		}
		else
		{
			RefreshSellState (0,sellMoney.ToString ());
		}
	}

	void SellGoodBtnClickBack (GameObject obj)
	{
		List<SellGoodsInfo> tempList = new List<SellGoodsInfo> ();
		foreach (KeyValuePair<long,SellGoodsInfo> pair in sellDic)
		{
			tempList.Add (pair.Value);
		}
		ShopData.Instance.SellShopGoods (tempList);
	}

	/// <summary>
	/// Refreshs the state of the sell.//0 - 正在出售 1-未选择物品 2-无可选物品
	/// </summary>
	/// <param name="tempIndex">Temp index.</param>
	void RefreshSellState (int tempIndex,string tempSell = null)
	{
		desLabel.transform.localPosition = new Vector3 (140,posY[tempIndex],0);
		sellGoodBtn.gameObject.SetActive (tempIndex == 0 ? true : false);

		switch (tempIndex)
		{
		case 0:

			desLabel.text = "本次出售可获得" + tempSell + "元宝";

			break;
		case 1:

			desLabel.text = "请选择要出售的物品";

			break;
		case 2:

			desLabel.text = "没有可以出售的物品...\n\n\n参与百战千军可在每日奖励中领取玉珏";

			break;
		default:
			break;
		}
	}

	#endregion
	
	void OtherHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "SellBtn":

			SwitchPage (ShopData.ShopPageType.SELL_PAGE);
			InItBagSellPage (0);

			break;
		case "RefreshBtn":

			ShopData.Instance.RefreshShopReq (shopType);

			break;
		case "CloseBtn":

			switch (spType)
			{
			case ShopData.ShopPageType.MAIN_PAGE:

				sEffectController.CloseCompleteDelegate += CloseShop;
				sEffectController.OnCloseWindowClick ();

				break;
			case ShopData.ShopPageType.SELL_PAGE:

				SwitchPage (ShopData.ShopPageType.MAIN_PAGE);

				break;
			default:
				break;
			}

			break;
		default:
			break;
		}
	}

	void CloseShop ()
	{
		ShopData.Instance.IsOpenFirstTime = true;
		Global.m_isOpenShop = false;
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
