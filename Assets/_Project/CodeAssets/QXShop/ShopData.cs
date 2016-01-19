//#define TestGeneralReward

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopData : Singleton<ShopData>,SocketProcessor {

	public static ShopData shopData;

	public enum ShopType//数字代表商铺中位置顺序
	{
		ORDINARY = 1,//普通
		MYSTRERT = 2,//神秘
		WEIWANG = 3,//威望
		GONGXIAN = 4,//贡献
		GONGXUN = 5,//功勋
		HUANGYE = 6,//荒野
	}
	private ShopType shopType = ShopType.ORDINARY;

	public enum ShopReqType
	{
		FREE,
		MONEY,
	}
	private ShopReqType shopReqType = ShopReqType.FREE;

	public enum ShopPageType
	{
		MAIN_PAGE,
		SELL_PAGE,
	}
	private ShopPageType spType = ShopPageType.MAIN_PAGE;

	private readonly Dictionary<ShopType,string[]> shopReqDic = new Dictionary<ShopType,string[]>()
	{
		//string[0]:商铺刷新类型（免费，花钱）
		//string[1]:商铺按钮名字 
		//string[2]:商铺购买类型 
		//string[3]:币种(QXCombData.cs:MoneyType)
		//string[4]:功能名称
		//string[5]:功能开启id(FunctionOpen.xml)
		{ShopType.HUANGYE,new string[]{"10,11","荒野币","1","4","联盟","104"}},//"荒野求生","300200"
		{ShopType.GONGXIAN,new string[]{"20,21","贡献","2","2","联盟","104"}},
		{ShopType.GONGXUN,new string[]{"30,31","功勋","3","5","联盟","104"}},
		{ShopType.WEIWANG,new string[]{"40,41","威望","4","3","百战千军","300100"}},
		{ShopType.ORDINARY,new string[]{"50,51","普通","5","1","商铺","9"}},
		{ShopType.MYSTRERT,new string[]{"60,61","神秘","6","1","商铺","9"}},
	};

	private Dictionary<ShopType,ShopResp> shopDic = new Dictionary<ShopType, ShopResp>();

	private ShopGoodInfo goodInfo;
	private float shopRefreshTime;

	private QXComData.MoneyType moneyType = QXComData.MoneyType.WEIWANG;

	private GameObject ShopPrefab;

	private string textStr;

	private bool isOpenFirstTime = true;
	public bool IsOpenFirstTime
	{
		get{return isOpenFirstTime;}
		set{isOpenFirstTime = value;}
	}

	/// <summary>
	/// The alliance level.
	/// </summary>
	private int allianceLevel;

	void Awake ()
	{
		shopData = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// Sets the type of the shop page.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void SetShopPageType (ShopPageType tempType)
	{
		spType = tempType;
	}

	/// <summary>
	/// Gets the alliance level.
	/// </summary>
	/// <returns>The alliance level.</returns>
	public int GetAllianceLevel ()
	{
		return allianceLevel;
	}

	/// <summary>
	/// Determines whether this instance is shop open the specified tempType.
	/// </summary>
	/// <returns><c>true</c> if this instance is shop open the specified tempType; otherwise, <c>false</c>.</returns>
	/// <param name="tempType">Temp type.</param>
	public bool IsShopFunctionOpen (ShopType tempType)
	{
		return FunctionOpenTemp.IsHaveID(int.Parse (shopReqDic[tempType][5]));
	}

	/// <summary>
	/// Opens the shop.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void OpenShop (ShopType tempType)
	{
		#if TestGeneralReward
		List<RewardData> rewardDataList = new List<RewardData> ();
		for (int i = 0;i < 3;i ++)
		{
			RewardData data = new RewardData (301013,1);//108105,920001,301013
			rewardDataList.Add (data);
		}
		
		//		GeneralRewardManager.Instance ().CreateReward (rewardDataList);
		GeneralRewardManager.Instance ().CreateSpecialReward (rewardDataList);

		return;
		#endif

//		if (!FunctionOpenTemp.IsHaveID (9))
//		{
//			Debug.Log ("商铺未开启");
//			ClientMain.m_UITextManager.createText(MyColorData.getColorString (5,"商铺未开启"));
//			return;
//		}
//
//		if (!IsShopFunctionOpen (tempType)) 
//		{
//			Debug.Log (shopReqDic[tempType][4] + "未开启");
//			ClientMain.m_UITextManager.createText(MyColorData.getColorString (5,shopReqDic[tempType][4] + "功能未开启"));
//			return;
//		}

		if (!Global.m_isOpenShop)
		{
			shopDic.Clear ();
			Global.m_isOpenShop = true;
			SetShopPageType (ShopPageType.MAIN_PAGE);
		}
		Debug.Log ("shopDic.Count:" + shopDic.Count);
		shopType = tempType;

		if (tempType == ShopType.ORDINARY)
		{
			if (!shopDic.ContainsKey (tempType))
			{
				ShopInfoReq (tempType,ShopReqType.FREE);

//				ShopResp ordinaryShop = new ShopResp();
//				ordinaryShop.goodsInfos = new List<DuiHuanInfo>();
//
//				int tempLateCount = DangpuItemCommonTemplate.templates.Count;
//				foreach (DangpuItemCommonTemplate tempLate in DangpuItemCommonTemplate.dangpuItemTemplateList ())
//				{
//					DuiHuanInfo duiHuanInfo = new DuiHuanInfo();
//					duiHuanInfo.id = tempLate.id;
//					duiHuanInfo.isChange = true;
//					duiHuanInfo.site = tempLate.site;
//					ordinaryShop.goodsInfos.Add (duiHuanInfo);
//				}
//
//				shopDic.Add (tempType,ordinaryShop);
			}
			else
			{
				//打开商铺
				LoadShopPrefab ();
			}
		}
		else
		{
			if (shopRefreshTime <= 0)
			{
				ShopInfoReq (tempType,ShopReqType.FREE);
			}
			else
			{
				if (!shopDic.ContainsKey (tempType))
				{
					ShopInfoReq (tempType,ShopReqType.FREE);
				}
				else
				{
					//打开商铺
					LoadShopPrefab ();
				}
			}
		}
	}

	/// <summary>
	/// Refreshs the shop req.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void RefreshShopReq (ShopType tempType)
	{
		shopType = tempType;
		textStr = "立刻刷新货物需要花费" + shopDic[shopType].nextRefreshNeedMoney + QXComData.MoneyName (MoneyType (shopType)) + "\n\n确定刷新货物吗？";
		QXComData.CreateBox (1,textStr,false,RefreshShopAskBack);
	}
	void RefreshShopAskBack (int i)
	{
		if (i == 2)
		{
//			ShopInfoReq (shopType,ShopReqType.MONEY);
//			确定刷新
			int moneyNum = 0;
			if (shopType == ShopType.MYSTRERT)
			{
				moneyNum = JunZhuData.Instance ().m_junzhuInfo.yuanBao;
			}
			else
			{
				moneyNum = shopDic[shopType].money;
			}

			if (shopDic[shopType].nextRefreshNeedMoney > moneyNum)
			{
				//缺钱
			    RefreshLackMoney ();
			}
			else
			{
				ShopInfoReq (shopType,ShopReqType.MONEY);
			}
		}
	}

	/// <summary>
	/// 商铺信息请求
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempReqType">Temp req type.</param>
	void ShopInfoReq (ShopType tempType,ShopReqType tempReqType)
	{
		shopReqType = tempReqType;
		
		ShopReq shopReq = new ShopReq();
		
		string[] reqType = shopReqDic [tempType] [0].Split (',');
		shopReq.type = tempReqType == ShopReqType.FREE ? int.Parse (reqType[0]) : int.Parse (reqType[1]);
		
		QXComData.SendQxProtoMessage (shopReq,ProtoIndexes.HY_SHOP_REQ,ProtoIndexes.HY_SHOP_RESP.ToString ());
		Debug.Log ("商铺信息请求:" + ProtoIndexes.HY_SHOP_REQ);
	}

	/// <summary>
	/// Shops the goods buy req.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void ShopGoodsBuyReq (ShopGoodInfo tempInfo)
	{
		goodInfo = tempInfo;
		Debug.Log ("goodInfo.needMoney:" + goodInfo.needMoney);
		Debug.Log ("goodInfo.xmlId:" + goodInfo.xmlId);
		Debug.Log ("goodInfo.itemId:" + goodInfo.itemId);
		Debug.Log ("goodInfo.site:" + goodInfo.site);
		int moneyNum = 0;
		if (goodInfo.moneyType == QXComData.MoneyType.YUANBAO)
		{
			moneyNum = JunZhuData.Instance ().m_junzhuInfo.yuanBao;
		}
		else if (goodInfo.moneyType == QXComData.MoneyType.TONGBI)
		{
			moneyNum = JunZhuData.Instance ().m_junzhuInfo.jinBi;
		}
		else
		{
			moneyNum = shopDic[shopType].money;
		}
		Debug.Log ("moneyNum:" + moneyNum);
		if (goodInfo.needMoney > moneyNum)
		{
			//缺钱
			BuyGoodLackMoney ();
		}
		else
		{
			BuyGoodReq storeBuyReq = new BuyGoodReq();
			
			storeBuyReq.type = int.Parse (shopReqDic[shopType][2]);
			storeBuyReq.goodId = tempInfo.xmlId;
			
			QXComData.SendQxProtoMessage (storeBuyReq,ProtoIndexes.HY_BUY_GOOD_REQ,ProtoIndexes.HY_BUY_GOOD_RESP.ToString ());
			Debug.Log ("购买物品：" + ProtoIndexes.HY_BUY_GOOD_REQ);
		}
	}

	/// <summary>
	/// Sells the shop goods.
	/// </summary>
	/// <param name="tempList">Temp list.</param>
	public void SellShopGoods (List<SellGoodsInfo> tempList)
	{
		PawnShopGoodsSell shopSellReq = new PawnShopGoodsSell();
		shopSellReq.sellGinfo = tempList;

		foreach (SellGoodsInfo s in tempList)
		{
			Debug.Log ("s" + s.bagId + "|||" + s.count);
		}
		QXComData.SendQxProtoMessage (shopSellReq,ProtoIndexes.PAWN_SHOP_GOODS_SELL,ProtoIndexes.PAWN_SHOP_GOODS_SELL_OK.ToString ());
		Debug.Log ("商铺出售物品请求:" + ProtoIndexes.PAWN_SHOP_GOODS_SELL);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.HY_SHOP_RESP://商铺返回信息
			{
				Debug.Log ("商铺信息返回:" + ProtoIndexes.HY_SHOP_RESP);
				ShopResp shopRes = new ShopResp();
				shopRes = QXComData.ReceiveQxProtoMessage (p_message,shopRes) as ShopResp;
				
				if (shopRes != null)
				{
					if (shopRes.goodsInfos == null)
					{
						shopRes.goodsInfos = new List<DuiHuanInfo>();
					}

					Debug.Log ("shopRes.msg:" + shopRes.msg);
					Debug.Log ("shopRes.goodsInfos:" + shopRes.goodsInfos.Count);
					Debug.Log ("shopRes.money:" + shopRes.money);
					Debug.Log ("shopRes.nextRefreshNeedMoney:" + shopRes.nextRefreshNeedMoney);

					if (shopType == ShopType.GONGXIAN)
					{
						allianceLevel = shopRes.lmshopLv;
					}

					switch (shopReqType)
					{
					case ShopReqType.FREE:

						shopRefreshTime = shopRes.remianTime;

						StopCoroutine ("ShopRefresh");
						StartCoroutine ("ShopRefresh");

						if (!shopDic.ContainsKey(shopType))
						{
							shopDic.Add (shopType,shopRes);
						}
						else
						{
							shopDic[shopType].msg = shopRes.msg;
							shopDic[shopType].goodsInfos = shopRes.goodsInfos;
							shopDic[shopType].nextRefreshNeedMoney = shopRes.nextRefreshNeedMoney;
							shopDic[shopType].money = shopRes.money;
						}

						LoadShopPrefab ();

						break;
					case ShopReqType.MONEY:

						if (shopRes.msg == 11)
						{
							RefreshLackMoney ();
						}
						else
						{
							shopRefreshTime = shopRes.remianTime;
							StopCoroutine ("ShopRefresh");
							StartCoroutine ("ShopRefresh");

							shopDic[shopType].msg = shopRes.msg;
							shopDic[shopType].goodsInfos = shopRes.goodsInfos;
							shopDic[shopType].nextRefreshNeedMoney = shopRes.nextRefreshNeedMoney;
							shopDic[shopType].money = shopRes.money;

							LoadShopPrefab ();
						}

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.HY_BUY_GOOD_RESP://购买物品返回
			{
				Debug.Log ("商铺物品购买返回:" + ProtoIndexes.HY_BUY_GOOD_RESP);
				
				BuyGoodResp shopBuyRes = new BuyGoodResp();
				shopBuyRes = QXComData.ReceiveQxProtoMessage (p_message,shopBuyRes) as BuyGoodResp;
				
				if (shopBuyRes != null)
				{
					Debug.Log ("购买结果：" + shopBuyRes.msg + "//0:金钱不足 1:购买成功 2:已经售罄 3:购买商品不存在");
					switch (shopBuyRes.msg)
					{
					case 0:

						BuyGoodLackMoney ();

						break;
					case 1:

						if (shopType == ShopType.ORDINARY)
						{
							foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
							{
								if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
								{
									duiHuan.remainCount -= 1;
									break;
								}
							}
							ShopPage.shopPage.InItShopPage (shopType,shopDic[shopType]);
						}
						else
						{
							if (shopType == ShopType.MYSTRERT)
							{
								QXComData.MoneyType type = goodInfo.moneyType;
								switch (type)
								{
								case QXComData.MoneyType.TONGBI:

									//刷新铜币

									break;
								case QXComData.MoneyType.YUANBAO:

									//刷新元宝

									break;
								default:
									break;
								}
							}
							else
							{
								shopDic[shopType].money = shopBuyRes.remianMoney;
							}
							foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
							{
								if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
								{
									duiHuan.isChange = false;
									break;
								}
							}
							ShopPage.shopPage.InItShopPage (shopType,shopDic[shopType]);
						}

						RewardData data = new RewardData(goodInfo.itemId,goodInfo.itemNum);
						GeneralRewardManager.Instance ().CreateReward (data);

						break;
					case 2:

						foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
						{
							if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
							{
								Debug.Log ("goodInfo.xmlId:" + goodInfo.xmlId + "||goodInfo.site:" + goodInfo.site);
								Debug.Log ("duiHuan.id:" + duiHuan.id + "||duiHuan.site:" + duiHuan.site);
								duiHuan.isChange = false;
								break;
							}
						}
						ShopPage.shopPage.InItShopPage (shopType,shopDic[shopType]);
						textStr = "购买失败，该物品已售罄！";
						QXComData.CreateBox (1,textStr,true,null);
						
						break;
					case 3:

						shopDic.Remove (shopType);
						ShopInfoReq (shopType,ShopReqType.FREE);
						textStr = "商品不存在，请重新尝试购买！";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.PAWN_SHOP_GOODS_SELL_OK:
			{
				//刷新商铺出售
				ShopPage.shopPage.InItBagSellPage (1);
				//播放数字动画
//				labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)227);
				
				return true;
			}
			}
		}
		return false;
	}

	/// <summary>
	/// Shops the refresh.
	/// </summary>
	/// <returns>The refresh.</returns>
	IEnumerator ShopRefresh ()
	{
		while (shopRefreshTime > 0)
		{
			yield return new WaitForSeconds (1);

			shopRefreshTime --;

			if (shopRefreshTime <= 0)
			{
				shopDic.Clear ();
				if (Global.m_isOpenShop)
				{
					if (spType == ShopPageType.MAIN_PAGE)
					{
						if (shopType != ShopType.ORDINARY)
						{
							//刷新商铺
							ShopInfoReq (shopType,ShopReqType.FREE);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Loads the shop prefab.
	/// </summary>
	void LoadShopPrefab ()
	{
		if (Global.m_isOpenShop)
		{
			if (ShopPrefab == null)
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.QXSHOP ),
				                        ShopLoadBack );
			}
			else
			{
				ShopPrefab.SetActive (true);

				InItShopInfo ();
			}
		}
	}
	void ShopLoadBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		ShopPrefab = GameObject.Instantiate( p_object ) as GameObject;

		InItShopInfo ();
	}
	void InItShopInfo ()
	{
		MainCityUI.TryAddToObjectList (ShopPrefab);
		ShopPage.shopPage.InItShopPage (shopType,shopDic[shopType]);
	}

	/// <summary>
	/// Lacks the money.
	/// </summary>
	void RefreshLackMoney ()
	{
		//对应货币不足
		if (shopType == ShopType.MYSTRERT)
		{
			textStr = "商铺刷新失败，" + QXComData.MoneyName (MoneyType (shopType)) + "不足，是否跳转到充值？";
			QXComData.CreateBox (1,textStr,false,TurnToVip);
		}
		else
		{
			textStr = "商铺刷新失败，" + QXComData.MoneyName (MoneyType (shopType)) + "不足！";
			QXComData.CreateBox (1,textStr,true,null);
		}
	}
	/// <summary>
	/// Buies the good lack money.
	/// </summary>
	void BuyGoodLackMoney ()
	{
		string desStr = "购买失败，";
		if (goodInfo.moneyType == QXComData.MoneyType.YUANBAO)
		{
			textStr = QXComData.MoneyName (goodInfo.moneyType) + "不足\n\n是否跳转到充值？";
			QXComData.CreateBox (1,desStr + textStr,false,TurnToVip);
		}
		else if (goodInfo.moneyType == QXComData.MoneyType.TONGBI)
		{
			textStr = QXComData.MoneyName (goodInfo.moneyType) + "不足\n\n是否使用" + "xxx元宝兑换 xxx铜币？";
			QXComData.CreateBox (1,desStr + textStr,false,null);
		}
		else
		{
			textStr = QXComData.MoneyName (goodInfo.moneyType) + "不足";
			QXComData.CreateBox (1,desStr + textStr,true,null);
		}
	}
	/// <summary>
	/// Turns to vip.
	/// </summary>
	/// <param name="i">The index.</param>
	void TurnToVip (int i)
	{
		if (i == 2)
		{

		}
	}

	/// <summary>
	/// Shops the name of the button.
	/// </summary>
	/// <returns>The button name.</returns>
	/// <param name="tempIndex">Temp index.</param>
	public string ShopBtnName (int tempIndex)
	{
		ShopType type = (ShopType)Enum.ToObject (typeof (ShopType),tempIndex);

		return shopReqDic [type] [1];
	}

	/// <summary>
	/// Moneies the type.
	/// </summary>
	/// <returns>The type.</returns>
	/// <param name="tempType">Temp type.</param>
	public QXComData.MoneyType MoneyType (ShopType tempType)
	{
		return (QXComData.MoneyType)Enum.ToObject (typeof(QXComData.MoneyType),int.Parse (shopReqDic [tempType] [3]));
	}

	void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
