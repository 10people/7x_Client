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

	private List<string> rulesList = new List<string>();

	public ScaleEffectController sEffectController;
	
	private ShopData.ShopPageType spType = ShopData.ShopPageType.MAIN_PAGE;

	private bool isDuiHuanYinDao = false;
	private bool isByFuShiYinDao = false;

	public GameObject moneyObj;

	void Awake ()
	{
		shopPage = this;
	}
	
	void Start ()
	{
		QXComData.LoadYuanBaoInfo (moneyObj);

//		string ruleStr = LanguageTemplate.GetText (LanguageTemplate.Text.sh);
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
//			Debug.Log ("Open");
			ShopData.Instance.OpenShop (shopType);
		}
	}
	
	#region ShopMainPage
	private ShopResp shopResp;
	
	public GameObject moneyInfoObj;
	public UISprite moneyIcon;
	public UILabel moneyLabel;

	public GameObject refreshBtn;
	public GameObject refreshLabelObj;
	
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
	private int column;//列数
	private float size = 660;
	private float itemDis = 165;

	private bool buyFinished = false;
	public bool BuyFinished { set{buyFinished = value;} get{return buyFinished;} }

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
			handler0.m_click_handler -= ShopBtnClickBack;
			handler0.m_click_handler += ShopBtnClickBack;
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
				shopBtnHandler.m_click_handler -= ShopBtnClickBack;
				shopBtnHandler.m_click_handler += ShopBtnClickBack;
			}
		}

		int activeCount = 0;
		for (int i = 0;i < shopBtnList.Count;i ++)
		{
			ShopData.ShopType sType = (ShopData.ShopType)Enum.ToObject (typeof (ShopData.ShopType),i + 1);
			shopBtnList[i].gameObject.SetActive (ShopData.Instance.IsShopFunctionOpen (sType));

			shopBtnList[i].transform.localPosition = new Vector3(0,-75 * activeCount,0);
			activeCount += ShopData.Instance.IsShopFunctionOpen (sType) ? 1 : 0;
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

		bool isAllianceType = tempType == ShopData.ShopType.GONGXIAN || tempType == ShopData.ShopType.GONGXUN || tempType == ShopData.ShopType.HUANGYE ? true : false;
		refreshBtn.SetActive (!isAllianceType);
		refreshLabelObj.transform.localPosition = new Vector3 (isAllianceType ? 335 : 150,185,0);

		ShopBtnState (tempType);

		foreach (EventHandler handler in otherBtnList)
		{
			handler.m_click_handler -= OtherHandlerClickBack;
			handler.m_click_handler += OtherHandlerClickBack;
		}

		CreateGoodList ();

//		Debug.Log ("QXComData.CheckYinDaoOpenState (400040):" + QXComData.CheckYinDaoOpenState (400040));
		if (!BuyFinished)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO, 400040, 1);
		}
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
//			Debug.Log ("ShopType:" + shopType);
			for (int i = 0;i < shopBtnList.Count;i ++)
			{
				UIWidget[] btnWidgets = shopBtnList[i].GetComponentsInChildren<UIWidget> ();
				foreach (UIWidget widget in btnWidgets)
				{
					widget.color = (int)tempType == i + 1 ? Color.white : Color.gray;
				}
			}
		}
	}
	
	void ShopBtnClickBack (GameObject obj)
	{
//		Debug.Log (obj.name + "||" + ((int)shopType).ToString ());
		if (((int)shopType).ToString () != obj.name)
		{
			int btnIndex = int.Parse (obj.name);
			ShopData.ShopType type = (ShopData.ShopType)Enum.ToObject (typeof(ShopData.ShopType),btnIndex);
			
//			ShopBtnState (type);
			ShopData.Instance.OpenShop (type);
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100460,2);
		}
	}
	
	/// <summary>
	/// Creates the good list.
	/// </summary>
	void CreateGoodList ()
	{
		goodObjList = QXComData.CreateGameObjectList (goodItemObj,goodGrid.GetComponent<UIGrid> (),shopResp.goodsInfos.Count,goodObjList);

		for (int i = 0;i < shopResp.goodsInfos.Count;i ++)
		{
			goodGrid.GetComponent<UIGrid> ().Reposition ();
			goodScrollView.UpdateScrollbars (true);
			goodSb.value = goodSbValue;
			GoodItem good = goodObjList[i].GetComponent<GoodItem> ();
			good.GetDuiHuanInfo (shopType,shopResp.goodsInfos[i]);
		}

		column = goodObjList.Count % 2 > 0 ? goodObjList.Count / 2 + 1 : goodObjList.Count / 2;

		goodSb.gameObject.SetActive (goodObjList.Count > 8 ? true : false);
		goodScrollView.enabled = goodObjList.Count > 8 ? true : false;
	}
	
	/// <summary>
	/// Opens the shop buy window.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void OpenShopBuyWindow (ShopGoodInfo tempInfo)
	{
		shopBuyWindow.SetActive (true);
		ShopBuyWindow.shopBuyWindow.GetBuyGoodInfo (tempInfo,shopType);
	}
	
	#endregion
	
	#region SellPage
	
	private List<ShopSellGoodInfo> sellGoodList = new List<ShopSellGoodInfo> ();
	private List<GameObject> bagObjList = new List<GameObject> ();
	
	public UIScrollView bagSc;
	public UIScrollBar bagSb;
	public UIGrid bagGrid;
	
	public EventHandler sellGoodBtn;
	public UILabel desLabel;
	private readonly int[] posY = new int[]{155,90,-155};
	
	private Dictionary<long,SellGoodsInfo> sellDic = new Dictionary<long, SellGoodsInfo>();
	private int sellTongBi;
	
	public UILabel sellLabel;
	
	/// <summary>
	/// Ins it bag sell page.
	/// </summary>
	/// <param name="tempSellState">Temp sell state.0,1</param>
	public void InItBagSellPage (int tempSellState)
	{
		sellTongBi = 0;

		if (tempSellState == 0)
		{
			sellGoodList.Clear ();
			foreach (BagItem b in BagData.Instance().m_bagItemList)
			{
				if (b.itemType == 21)//玉玦
				{
					ShopSellGoodInfo sellGood = new ShopSellGoodInfo();
					sellGood.itemId = b.itemId;
					sellGood.itemType = (QXComData.XmlType)Enum.ToObject (typeof(QXComData.XmlType),(int)QXComData.XmlType.YUJUE);
					sellGood.dbId = b.dbId;
					sellGood.itemNum = b.cnt;
					sellGood.sellType = 1;
					sellGoodList.Add(sellGood);
				}
			}
			
			var miBaoInfo = MiBaoGlobleData.Instance().G_MiBaoInfo;
			foreach (MibaoInfo miBao in miBaoInfo.miBaoList)
			{
				if (miBao.star == 5 && miBao.suiPianNum > 0)
				{
					ShopSellGoodInfo sellGood = new ShopSellGoodInfo();
					sellGood.itemId = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (miBao.tempId).id;
					sellGood.itemType = (QXComData.XmlType)Enum.ToObject (typeof(QXComData.XmlType),(int)QXComData.XmlType.MIBAO_PIECE);
					sellGood.dbId = miBao.dbId;
					sellGood.itemNum = miBao.suiPianNum;
					sellGood.sellType = 2;
					sellGoodList.Add(sellGood);
				}
			}
		}
		else
		{
			for (int i = 0;i < sellGoodList.Count;i ++)
			{
				if (sellDic.ContainsKey (sellGoodList[i].dbId))
				{
					sellGoodList[i].itemNum -= sellDic[sellGoodList[i].dbId].count;

					if (sellGoodList[i].itemNum <= 0)
					{
						sellGoodList.Remove (sellGoodList[i]);
					}
				}
			}
		}

		sellDic.Clear ();

		bagSc.transform.parent.gameObject.SetActive (sellGoodList.Count > 0 ? true : false);
		
		int bagItemCount = sellGoodList.Count - bagObjList.Count;
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
			bagItem.GetBagItemInfo (sellGoodList[i]);
		}
		
		bagSc.enabled = sellGoodList.Count > 10 ? true : false;
		bagSb.gameObject.SetActive (sellGoodList.Count > 10 ? true : false);
		RefreshSellState (bagObjList.Count > 0 ? 1 : 2);
		
		switch (tempSellState)
		{
		case 0:
			sellLabel.text = sellGoodList.Count > 0 ? 
				LanguageTemplate.GetText ((LanguageTemplate.Text)226) : LanguageTemplate.GetText ((LanguageTemplate.Text)228);
			break;
		case 1:
			sellLabel.text = LanguageTemplate.GetText ((LanguageTemplate.Text)227);
			break;
		default:
			break;
		}
		
		sellGoodBtn.m_click_handler -= SellGoodBtnClickBack;
		sellGoodBtn.m_click_handler += SellGoodBtnClickBack;
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
	public void GetSellBagItem (ShopSellGoodInfo tempItem,int type)
	{
		int tongBi = 0;

		switch (tempItem.itemType)
		{
		case QXComData.XmlType.YUJUE:
			tongBi = ItemTemp.getItemTempById (tempItem.itemId).sellNum;
			break;
		case QXComData.XmlType.MIBAO_PIECE:
			tongBi = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (tempItem.itemId).recyclePrice;
			break;
		default:
			break;
		}

		switch (type)
		{
		case 1://增加
			
			if (!sellDic.ContainsKey (tempItem.dbId))
			{
				SellGoodsInfo sellGood = new SellGoodsInfo();
				sellGood.bagId = tempItem.dbId;
				sellGood.count = 1;
				sellGood.goodsType = tempItem.sellType;
				sellDic.Add (tempItem.dbId,sellGood);
			}
			else
			{
				sellDic[tempItem.dbId].count += 1;
			}

			sellTongBi += tongBi;
			
			break;
		case 2://减少
			
			if (sellDic.ContainsKey (tempItem.dbId))
			{
				sellDic[tempItem.dbId].count -= 1;
				if (sellDic[tempItem.dbId].count <= 0)
				{
					sellDic.Remove (tempItem.dbId);
				}
				
				sellTongBi -= tongBi;
			}
			
			break;
		default:
			break;
		}
		
		if (sellDic.Count <= 0)
		{
			RefreshSellState (1);
		}
		else
		{
			RefreshSellState (0,sellTongBi.ToString ());
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

			desLabel.text = "本次出售可获得" + tempSell + "铜币";

			break;
		case 1:
			
			desLabel.text = "请选择要出售的物品";
			
			break;
		case 2:
			
			desLabel.text = "没有可以出售的物品...\n\n参与百战千军可在每日奖励中领取玉珏";
			
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
		case "LeftBtn":

			StartCoroutine( StartMove (-1,column));

			break;
		case "RightBtn":

			StartCoroutine( StartMove (1,column));

			break;
		case "RulesBtn":

			GeneralControl.Instance.LoadRulesPrefab (rulesList);

			break;
		default:
			break;
		}
	}
	
	IEnumerator StartMove(int tempDirect,int tempColum)
	{
		UIPanel goodScPanel = goodScrollView.GetComponent<UIPanel> ();

		float moveDis = 0;
		if (tempDirect == 1)//向右移动
		{
			moveDis = tempColum * itemDis - size + (int)goodScPanel.cachedGameObject.transform.localPosition.x;
			
			if(moveDis > size)
			{
				SpringPanel.Begin (goodScPanel.cachedGameObject,
				                   new Vector3 (goodScPanel.cachedGameObject.transform.localPosition.x - size, 0f, 0f), 8f);
				yield return new WaitForSeconds (0.2f);
			}
			else
			{
				SpringPanel.Begin (goodScPanel.cachedGameObject,
				                   new Vector3(goodScPanel.cachedGameObject.transform.localPosition.x - moveDis, 0f, 0f), 8f);
				yield return new WaitForSeconds (0.2f);
			}
		}
		else
		{
			moveDis = (int)(-goodScPanel.cachedGameObject.transform.localPosition.x);
			if(moveDis > size)
			{
				SpringPanel.Begin (goodScPanel.cachedGameObject,
				                   new Vector3 (goodScPanel.cachedGameObject.transform.localPosition.x + size, 0f, 0f), 8f);
				yield return new WaitForSeconds (0.2f);
			}
			else
			{
				SpringPanel.Begin (goodScPanel.cachedGameObject,
				                   new Vector3 (goodScPanel.cachedGameObject.transform.localPosition.x + moveDis, 0f, 0f), 8f);
				yield return new WaitForSeconds (0.2f);
			}
		}
	}

	void Update ()
	{
		if (QXComData.CheckYinDaoOpenState (100220) && !isDuiHuanYinDao)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100220,2);
			isDuiHuanYinDao = true;
		}
		
		if (QXComData.CheckYinDaoOpenState (100460) && !isByFuShiYinDao)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100460,1);
			isByFuShiYinDao = true;
		}

		ScrollViewMove ();
	}

	void ScrollViewMove ()
	{
		UIPanel goodScPanel = goodScrollView.GetComponent<UIPanel> ();
		goodScPanel.clipOffset = new Vector2(-goodScPanel.gameObject.transform.localPosition.x, 0);
		
		float moveDis = column * itemDis - size + (int)goodScPanel.cachedGameObject.transform.localPosition.x;
		//3-left 4-right
		if (column <= 4)
		{
			otherBtnList[3].gameObject.SetActive (false);
			otherBtnList[4].gameObject.SetActive (false);
			return;
		}
		
		if (moveDis <= 5)
		{
			otherBtnList[4].gameObject.SetActive (false);
			otherBtnList[3].gameObject.SetActive (true);
		}
		
		else if (goodScPanel.cachedGameObject.transform.localPosition.x >= -10)
		{
			otherBtnList[4].gameObject.SetActive (true);
			otherBtnList[3].gameObject.SetActive (false);
		}
		
		else
		{
			otherBtnList[4].gameObject.SetActive (true);
			otherBtnList[3].gameObject.SetActive (true);
		}
	}

	void CloseShop ()
	{
		goodSbValue = 0;
		ShopData.Instance.IsOpenFirstTime = true;
		Global.m_isOpenShop = false;
		if(GameObject.Find("New_My_Union(Clone)"))
		{
			NewAlliancemanager.Instance ().ShowAllianceGuid ();
		}
		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}
}
