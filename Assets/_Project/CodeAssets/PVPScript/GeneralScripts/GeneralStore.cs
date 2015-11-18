using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralStore : MonoBehaviour {

	private GeneralControl.StoreType storeType;

	private List<DuiHuanInfo> duihuanList = new List<DuiHuanInfo> ();
	private List<GameObject> rewardObjList = new List<GameObject>();
	public List<EventHandler> btnHandlerList = new List<EventHandler> ();

	private int refreshTime;
	private int money;
	private int commonId;//钱币iconid
	private int refreshNeedMoney;

	public UILabel titleLabel;
	public UILabel timeLabel;
	public UILabel moneyLabel;
	public UISprite moneyIcon;

	public GameObject rewardGrid;
	public GameObject rewardObj;

	private string textStr = "";

	public ScaleEffectController m_ScaleEffectController;

	/// <summary>
	/// 获得兑换商店信息
	/// </summary>
	/// <param name="tempType">商铺类型</param>
	/// <param name="tempDuiHuanList">物品信息</param>
	/// <param name="tempTime">刷新时间</param>
	/// <param name="tempMoney">兑换钱币数量</param>
	/// <param name="refreshNeed">刷新货物需要的钱币数量</param>
	public void GetStoreInfo (GeneralControl.StoreType tempType,List<DuiHuanInfo> tempDuiHuanList,int tempTime,int tempMoney,int refreshNeed)
	{
		m_ScaleEffectController.transform.localScale = Vector3.one;

		storeType = tempType;
		titleLabel.text = GeneralControl.Instance.storeDic[tempType][2];

		refreshTime = tempTime;
		money = tempMoney;

		moneyLabel.text = tempMoney.ToString ();

		refreshNeedMoney = refreshNeed;
//		Debug.Log ("refreshNeedMoney:" + refreshNeedMoney);

		moneyIcon.spriteName = GeneralControl.Instance.storeDic[tempType][0];

		int instantCount = tempDuiHuanList.Count - rewardObjList.Count;
		//创建商铺物品
		if (instantCount > 0)
		{
			for (int i = 0;i < instantCount;i ++)
			{
				GameObject rewardItem = (GameObject)Instantiate (rewardObj);

				rewardItem.SetActive (true);
				rewardItem.transform.parent = rewardGrid.transform;
				rewardItem.transform.localPosition = Vector3.zero;
				rewardItem.transform.localScale = Vector3.one;
				
				rewardObjList.Add (rewardItem);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (instantCount);i ++)
			{
				Destroy (rewardObjList[rewardObjList.Count - 1]);
				rewardObjList.RemoveAt (rewardObjList.Count - 1);
			}
		}

		for (int i = 0;i < tempDuiHuanList.Count - 1;i ++)
		{
			for (int j = 0;j < tempDuiHuanList.Count - i - 1;j ++)
			{
				if (tempDuiHuanList[j].site > tempDuiHuanList[j + 1].site)
				{
					DuiHuanInfo tempInfo = tempDuiHuanList[j];
					tempDuiHuanList[j] = tempDuiHuanList[j + 1];
					tempDuiHuanList[j + 1] = tempInfo;
				}
			}
		}

		rewardGrid.GetComponent<UIGrid> ().repositionNow = true;

		duihuanList = tempDuiHuanList;

		for (int i = 0;i < tempDuiHuanList.Count;i ++)
		{
			GeneralGoods goodsInfo = rewardObjList[i].GetComponent<GeneralGoods> ();
			goodsInfo.InItGoodsInfo (tempType,tempDuiHuanList[i]);
		}

		rewardGrid.transform.parent.GetComponent <UIScrollView> ().enabled = tempDuiHuanList.Count <= 8 ? false : true;

		StopCoroutine ("RefreshStoreTime");
		StartCoroutine ("RefreshStoreTime");

		moneyIcon.GetComponent<NGUILongPress> ().OnLongPress += ActiveTips;
		moneyIcon.GetComponent<NGUILongPress> ().OnLongPressFinish += DoActiveTips;

		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler += BtnHandlerCallBack;
		}

		isDuiHuan = false;
		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);
	}

	void BtnHandlerCallBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "RefreshBtn":

			textStr = "\n\n立即刷新所有货物需要花费" + refreshNeedMoney + GeneralControl.Instance.MoneyTypeName + "\n确定刷新所有货物吗？";
			
			Global.CreateBox (QXComData.titleStr,
			                  MyColorData.getColorString (1,textStr) ,null, null,
			                  QXComData.cancelStr, QXComData.confirmStr,
			                  RefreshBack);

			break;
		case "BackBtn":

			switch (storeType)
			{
			case GeneralControl.StoreType.PVP:
			{
				PvpPage.pvpPage.PvpActiveState (true);
				PvpPage.pvpPage.pvpResp.hasWeiWang = money;
				PvpPage.pvpPage.InItMyRank ();
				QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,2);
				
				break;
			}
			case GeneralControl.StoreType.HUANGYE:
			{
				
				break;
			}
			case GeneralControl.StoreType.ALLANCE:
			{
				_MyAllianceManager.Instance().SHow_OR_Close_MyAlliance();
				break;
			}
			case GeneralControl.StoreType.ALLIANCE_FIGHT:
			{
				break;
			}
			default:
				break;
			}
			DoCloseWindow();

			break;
		case "CloseBtn":

			switch (storeType)
			{
			case GeneralControl.StoreType.PVP:
			{
				PvpPage.pvpPage.DisActiveObj ();
				break;
			}
			case GeneralControl.StoreType.HUANGYE:
			{
				HY_UIManager.Instance().CloseUI();
				break;
			}
			case GeneralControl.StoreType.ALLANCE:
			{
				_MyAllianceManager.Instance().DoCloseWindow();
				//_MyAllianceManager.Instance().Closed();
				break;
			}
			case GeneralControl.StoreType.ALLIANCE_FIGHT:
			{
				_MyAllianceManager.Instance().Closed();
				AllianceFightMainPage.fightMainPage.CloseBtn ();
				break;
			}
			default:
				break;
			}
			m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
			m_ScaleEffectController.OnCloseWindowClick();

			break;
		default:
			break;
		}
	}

	void RefreshBack (int i)
	{
		if (i == 2)
		{
			//判断钱币是否够
			if (money >= refreshNeedMoney)
			{
				Debug.Log ("确定刷新物品");
				
				GeneralControl.Instance.GeneralStoreReq (storeType,GeneralControl.StoreReqType.USE_MONEY);
			}
			else
			{
				//钱币不足
				BoxLoad (GeneralControl.BoxCallBackType.STORE_REQ);
			}
		}
	}
	//钱币不足提示弹框
	void BoxLoad (GeneralControl.BoxCallBackType tempCallBack)
	{
		switch (tempCallBack)
		{
		case GeneralControl.BoxCallBackType.STORE_REQ:
		{
			textStr = "\n\n商铺刷新失败，" + GeneralControl.Instance.MoneyTypeName + "不足";
			break;
		}
		case GeneralControl.BoxCallBackType.STORE_BUY:
		{
			textStr = "\n\n" + GeneralControl.Instance.MoneyTypeName + "不足,购买失败";
			break;
		}
		default:
			break;
		}
		
		Global.CreateBox (QXComData.titleStr, 
		                  MyColorData.getColorString (1,textStr), null, null, 
		                  QXComData.confirmStr, null, 
		                  BoxBack);
	}
	void BoxBack (int i)
	{
		isDuiHuan = false;
	}

	void DoCloseWindow()
	{
		foreach (EventHandler handler in btnHandlerList)
		{
			handler.m_handler -= BtnHandlerCallBack;
		}

		moneyIcon.GetComponent<NGUILongPress> ().OnLongPress -= ActiveTips;
		moneyIcon.GetComponent<NGUILongPress> ().OnLongPressFinish -= DoActiveTips;

		MainCityUI.TryRemoveFromObjectList (gameObject);
		gameObject.SetActive (false);
	}

	void ActiveTips (GameObject go)
	{
		ShowTip.showTip (commonId);
	}
	void DoActiveTips (GameObject go)
	{
		ShowTip.close ();
	}

	IEnumerator RefreshStoreTime ()
	{	
		while (refreshTime > 0) 
		{
			refreshTime --;
			
			timeLabel.text = QXComData.TimeFormat (refreshTime);
			
			if (refreshTime == 0) 
			{
				//刷新商铺时间
				GeneralControl.Instance.GeneralStoreReq (storeType,GeneralControl.StoreReqType.FREE);
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// 刷新商店状态
	/// </summary>
	/// <param name="tempItemId">物品id</param>
	/// <param name="tempMoney">剩余钱币</param>
	public void RefreshStoreItem (int tempItemId,int tempMoney)
	{
		moneyLabel.text = tempMoney.ToString ();

		for (int i = 0;i < duihuanList.Count;i ++)
		{
			if (duihuanList[i].id == tempItemId)
			{
				duihuanList[i].isChange = false;
			}
		}
		for (int i = 0;i < duihuanList.Count;i ++)
		{
			GeneralGoods goodsInfo = rewardObjList[i].GetComponent<GeneralGoods> ();
			goodsInfo.InItGoodsInfo (storeType,duihuanList[i]);
		}
	}

	#region
	private DuiHuanInfo getDuiHuanInfo;
	private int needNum;
	private string itemName;

	private bool isDuiHuan = false;//是否点击兑换物品
	public bool IsDuiHuan
	{
		set{isDuiHuan = value;}
		get{return isDuiHuan;}
	}

	public GameObject duiHuanWindowObj;//兑换窗口

	/// <summary>
	/// 兑换请求回调
	/// </summary>
	/// <param name="tempDuiHuanInfo">兑换物品信息</param>
	/// <param name="tempItemId">兑换物品id</param>
	/// <param name="tempNeedMoney">兑换需要钱币数量</param>
	/// <param name="tempItemName">想要兑换的物品名字</param>
	/// <param name="tempItemType">物品类型</param>
	/// <param name="tempItemNum">物品数量</param>
	public void DuiHuanReq (DuiHuanInfo tempDuiHuanInfo,int tempItemId,int tempNeedMoney,string tempItemName,int tempItemType,int tempItemNum)
	{
		getDuiHuanInfo = tempDuiHuanInfo;
		needNum = tempNeedMoney;
		itemName = tempItemName;

		duiHuanWindowObj.SetActive (true);

		GeneralBuyWindow generalBuy = duiHuanWindowObj.GetComponent<GeneralBuyWindow> ();
		generalBuy.GetGoodsInfo (storeType,tempItemType,tempItemId,tempItemNum,tempItemName,tempNeedMoney);
	}

	public void DuiHuanReqBack (int i)
	{
		isDuiHuan = false;
		if (i == 2)
		{
			//判断钱币是否够 needNum
			if (money >= needNum)
			{
				Debug.Log ("确定兑换");

				GeneralControl.Instance.StoreBuyReq (GeneralControl.StoreGoodInfo.CreateStoreGood(getDuiHuanInfo.id,itemName,needNum));
			}
			else
			{
				//钱币不足
				BoxLoad (GeneralControl.BoxCallBackType.STORE_BUY);
			}
		}
		else if (i == 1)
		{
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);
		}
	}
	#endregion
}
