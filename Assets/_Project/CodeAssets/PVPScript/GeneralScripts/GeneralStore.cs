using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralStore : MonoBehaviour {

	public UILabel titleLabel;

	private GeneralControl.StoreType storeType;

	public GameObject rewardGrid;
	public GameObject rewardObj;
	private List<GameObject> rewardObjList = new List<GameObject>();

	private List<DuiHuanInfo> duihuanList = new List<DuiHuanInfo> ();

	private int refreshTime;
	public UILabel timeLabel;
	private int money;
	public UILabel moneyLabel;
	public UISprite moneyIcon;
	private int commonId;//钱币iconid

	private int refreshNeedMoney;

	private string confirmStr;
	private string cancelStr;
	private string titleStr = "提示";
	private string moneyTypeStr;//钱币种类
	private string textStr = "";

	public ScaleEffectController m_ScaleEffectController;

	void Start ()
	{
		confirmStr = GeneralControl.Instance.confirmStr;
		cancelStr = GeneralControl.Instance.cancelStr;
	}

	/// <summary>
	/// 获得兑换商店信息
	/// </summary>
	/// <param name="tempType">商铺类型</param>
	/// <param name="titleStr">商铺名字</param>
	/// <param name="tempDuiHuanList">物品信息</param>
	/// <param name="tempTime">刷新时间</param>
	/// <param name="tempMoney">兑换钱币数量</param>
	/// <param name="refreshNeed">刷新货物需要的钱币数量</param>
	public void GetStoreInfo (GeneralControl.StoreType tempType,string titleStr,List<DuiHuanInfo> tempDuiHuanList,int tempTime,int tempMoney,int refreshNeed)
	{
		storeType = tempType;
		titleLabel.text = titleStr;

		refreshTime = tempTime;
		money = tempMoney;

		moneyLabel.text = tempMoney.ToString ();
		moneyTypeStr = MoneyStr (tempType);

		refreshNeedMoney = refreshNeed;
//		Debug.Log ("refreshNeedMoney:" + refreshNeedMoney);

		moneyIcon.spriteName = IconName (tempType);

		//创建商铺物品
		if (rewardObjList.Count == 0)
		{
			for (int i = 0;i < tempDuiHuanList.Count;i ++)
			{
				GameObject rewardItem = (GameObject)Instantiate (rewardObj);

				rewardItem.SetActive (true);
				rewardItem.transform.parent = rewardGrid.transform;
				rewardItem.transform.localPosition = Vector3.zero;
				rewardItem.transform.localScale = Vector3.one;
				
				rewardObjList.Add (rewardItem);
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

		duihuanList = tempDuiHuanList;

		for (int i = 0;i < tempDuiHuanList.Count;i ++)
		{
			GeneralGoods goodsInfo = rewardObjList[i].GetComponent<GeneralGoods> ();
			goodsInfo.InItGoodsInfo (tempType,tempDuiHuanList[i]);
		}

		rewardGrid.GetComponent<UIGrid> ().repositionNow = true;

		rewardGrid.transform.parent.GetComponent <UIScrollView> ().enabled = tempDuiHuanList.Count <= 8 ? false : true;

		StopCoroutine ("RefreshStoreTime");
		StartCoroutine ("RefreshStoreTime");

		moneyIcon.GetComponent<NGUILongPress> ().OnLongPress += ActiveTips;
		moneyIcon.GetComponent<NGUILongPress> ().OnLongPressFinish += DoActiveTips;

		isDuiHuan = false;

		QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,3);
	}

	//iconName
	public string IconName (GeneralControl.StoreType tempStoreType)
	{
		string iconName = "";
		switch (tempStoreType)
		{
		case GeneralControl.StoreType.PVP:
			
			iconName = "weiwangIcon";
			
			break;
			
		case GeneralControl.StoreType.HUANGYE:
			
			iconName = "HuangYe";
			
			break;
			
		case GeneralControl.StoreType.ALLIANCE_FIGHT:
			
			iconName = "GongXun";
			
			break;
			
		case GeneralControl.StoreType.ALLANCE:
			
			iconName = "GongXian";
			
			break;

		default:
			break;
		}

		return iconName;
	}

	public string MoneyStr (GeneralControl.StoreType tempStoreType)
	{
		string money = "";
		switch (tempStoreType)
		{
		case GeneralControl.StoreType.PVP:
			
			money = "威望";
			
			break;
			
		case GeneralControl.StoreType.HUANGYE:
			
			money = "荒野币";
			
			break;
			
		case GeneralControl.StoreType.ALLIANCE_FIGHT:
			
			money = "功勋";
			
			break;
			
		case GeneralControl.StoreType.ALLANCE:
			
			money = "贡献";
			
			break;
			
		default:
			break;
		}
		return money;
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
		string hourStr = "";
		string minuteStr = "";
		string secondStr = "";
		
		while (refreshTime > 0) 
		{
			refreshTime --;

			int hour = refreshTime / 3600;
			int minute = (refreshTime / 60) % 60;
			int second = refreshTime % 60;
//			Debug.Log (hour + ":" + minute + ":" + second);
			if (hour < 10)
			{
				hourStr = "0" + hour;
			}
			else
			{
				hourStr = hour.ToString ();
			}

			if (minute < 10)
			{
				minuteStr = "0" + minute;
			}
			else
			{
				minuteStr = minute.ToString ();
			}
			
			if (second < 10) 
			{
				secondStr = "0" + second;
			} 
			else 
			{
				secondStr = second.ToString ();
			}
			
			timeLabel.text = hourStr + " : " + minuteStr + "：" + secondStr;
			
			if (refreshTime == 0) 
			{
				//刷新商铺时间
				GeneralControl.Instance.GeneralStoreReq (storeType,GeneralControl.StoreReqType.FREE,titleLabel.text);
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

		GameObject dhWindow = (GameObject)Instantiate (duiHuanWindowObj);
		dhWindow.SetActive (true);
		dhWindow.transform.parent = duiHuanWindowObj.transform.parent;
		dhWindow.transform.localPosition = duiHuanWindowObj.transform.localPosition;
		dhWindow.transform.localScale = duiHuanWindowObj.transform.localScale;

		GeneralBuyWindow generalBuy = dhWindow.GetComponent<GeneralBuyWindow> ();
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
				
				GeneralControl.Instance.StoreBuyReq (storeType,getDuiHuanInfo.id,itemName,needNum);
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

	//刷新按钮
	public void RefreshBtn ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        RefreshLoadCallback );
	}
	
	private void RefreshLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = "\n\n立即刷新所有货物需要花费";
		string str2 = "\n确定刷新所有货物吗？";

		textStr = str1 + refreshNeedMoney + moneyTypeStr + str2;

		uibox.setBox(titleStr,MyColorData.getColorString (1,textStr) ,null, null, cancelStr, confirmStr,RefreshBack);
	}
	
	void RefreshBack (int i)
	{
		if (i == 2)
		{
			//判断钱币是否够
			if (money >= refreshNeedMoney)
			{
				Debug.Log ("确定刷新物品");
				
				GeneralControl.Instance.GeneralStoreReq (storeType,GeneralControl.StoreReqType.USE_MONEY,titleLabel.text);
			}
			else
			{
				//钱币不足
				BoxLoad (GeneralControl.BoxCallBackType.STORE_REQ);
			}
		}
	}

	//钱币不足提示弹框
	private GeneralControl.BoxCallBackType boxCallBackType;
	void BoxLoad (GeneralControl.BoxCallBackType tempCallBack)
	{
		boxCallBackType = tempCallBack;
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        BoxLoadCallBack );
	}
	
	private void BoxLoadCallBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate (p_object) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		switch (boxCallBackType)
		{
		case GeneralControl.BoxCallBackType.STORE_REQ:
		{
			textStr = "\n商铺刷新失败，" + moneyTypeStr + "不足";
			break;
		}
		case GeneralControl.BoxCallBackType.STORE_BUY:
		{
			textStr = "\n\n" + moneyTypeStr + "不足,购买失败";
			break;
		}
		default:
			break;
		}
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,textStr), null, 
		             null, confirmStr, null, BoxBack);
	}

	void BoxBack (int i)
	{
		isDuiHuan = false;
	}

	//返回按钮
	public void BackBtn ()
	{
		switch (storeType)
		{
		case GeneralControl.StoreType.PVP:
		{
			PvpPage.pvpPage.PvpActiveState (true);
			PvpPage.pvpPage.pvpResp.hasWeiWang = money;
			PvpPage.pvpPage.InItMyRank ();
//			BaiZhanMainPage.baiZhanMianPage.baiZhanResp.hasWeiWang = money;
//			BaiZhanMainPage.baiZhanMianPage.InItMyRank ();
//			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
//			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
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

		Destroy (this.gameObject);
	}

	//关闭按钮
	public void CloseBtn ()
	{
		m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
		m_ScaleEffectController.OnCloseWindowClick();

		switch (storeType)
		{
		case GeneralControl.StoreType.PVP:
		{
			PvpPage.pvpPage.DisActiveObj ();
//			BaiZhanData.Instance ().CloseBaiZhan ();
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
	}

	void DoCloseWindow()
	{
		Destroy(gameObject);
	}
}
