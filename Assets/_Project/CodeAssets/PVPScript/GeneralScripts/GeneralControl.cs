using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralControl : Singleton<GeneralControl>,SocketProcessor {

	#region GeneralStore
	public class StoreGoodInfo
	{
		public int itemId;//兑换物品id
		public string itemName;//兑换物品名字
		public int needMoney;//需要的钱币数量

		public static StoreGoodInfo CreateStoreGood (int tempId,string tempName,int tempMoney)
		{
			StoreGoodInfo tempGood = new StoreGoodInfo ();
			tempGood.itemId = tempId;
			tempGood.itemName = tempName;
			tempGood.needMoney = tempMoney;

			return tempGood;
		}
	}
	
	//商店类型
	public enum StoreType
	{
		PVP = 4,//百战
		HUANGYE = 1,//荒野
		ALLANCE = 2,//联盟
		ALLIANCE_FIGHT = 3,//联盟战
	}
	private StoreType storeType = StoreType.PVP;
	
	//商铺请求类型
	public enum StoreReqType
	{
		FREE,//免费
		USE_MONEY,//使用货币
	}

	public enum BoxCallBackType
	{
		STORE_REQ,
		STORE_BUY,
	}

	private HyShopResp storeResp;//商铺返回信息
	private HyBuyGoodResp storeBuyResp;//商铺购买返回信息

	private StoreGoodInfo goodInfo;

	private List<StoreType> storeTypeList = new List<StoreType>();
	public Dictionary<StoreType,string[]> storeDic = new Dictionary<StoreType, string[]>();
	private string[][] storeInfo = new string[][]{new string[]{"weiwangIcon","威望","威望商店"},
		new string[]{"HuangYe","荒野币","荒野商店"},
		new string[]{"GongXun","功勋","功勋商店"},
		new string[]{"GongXian","贡献","贡献商店"}};
	
	private Dictionary<StoreType,int[]> storeReqDic = new Dictionary<StoreType, int[]>();
	private int[][] storeReqType = new int[][]{new int[]{40,41},
		new int[]{20,21},
		new int[]{30,31},
		new int[]{10,11}};

	private string moneyTypeName;
	public string MoneyTypeName
	{
		set{moneyTypeName = value;}
		get{return moneyTypeName;}
	}

	private GameObject generalStoreObj;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// 公用商铺信息请求
	/// </summary>
	/// <param name="tempType">商铺类型</param>
	/// <param name="buyType">购买类型</param>
	public void GeneralStoreReq (StoreType tempType,StoreReqType tempReqType)
	{
		if (storeTypeList.Count == 0)
		{
			storeTypeList.Add (StoreType.PVP);
			storeTypeList.Add (StoreType.HUANGYE);
			storeTypeList.Add (StoreType.ALLIANCE_FIGHT);
			storeTypeList.Add (StoreType.ALLANCE);
			
			for (int i = 0;i < storeTypeList.Count;i ++)
			{
				storeDic.Add (storeTypeList[i],storeInfo[i]);
				storeReqDic.Add (storeTypeList[i],storeReqType[i]);
			}
		}

		storeType = tempType;

		MoneyTypeName = storeDic [tempType][1];

		HyShopReq storeReq = new HyShopReq();
		
		storeReq.type = tempReqType == StoreReqType.FREE ? storeReqDic[tempType][0] : storeReqDic[tempType][1];
		
		QXComData.SendQxProtoMessage (storeReq,ProtoIndexes.HY_SHOP_REQ,ProtoIndexes.HY_SHOP_RESP.ToString ());

//		Debug.Log ("商铺信息请求:" + ProtoIndexes.HY_SHOP_REQ);
	}
	
	/// <summary>
	/// 商铺物品购买请求
	/// </summary>
	public void StoreBuyReq (StoreGoodInfo tempGoodInfo)
	{
		goodInfo = tempGoodInfo;

		HyBuyGoodReq storeBuyReq = new HyBuyGoodReq();

		storeBuyReq.type = (int)storeType;
		storeBuyReq.goodId = tempGoodInfo.itemId;
		
		QXComData.SendQxProtoMessage (storeBuyReq,ProtoIndexes.HY_BUY_GOOD_REQ,ProtoIndexes.HY_BUY_GOOD_RESP.ToString ());
//		Debug.Log ("商铺物品购买请求:" + ProtoIndexes.HY_BUY_GOOD_REQ);
	}
	
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.HY_SHOP_RESP://商铺返回信息
			{
//				Debug.Log ("商铺信息返回:" + ProtoIndexes.HY_SHOP_RESP);

				HyShopResp storeRes = new HyShopResp();
				storeRes = QXComData.ReceiveQxProtoMessage (p_message,storeRes) as HyShopResp;
				
				if (storeRes != null)
				{
					if (storeRes.goodsInfos == null)
					{
						storeRes.goodsInfos = new List<DuiHuanInfo>();
					}
					
					storeResp = storeRes;

					switch (storeType)
					{
					case StoreType.PVP:
						PvpPage.pvpPage.PvpActiveState (false);
						break;
					case StoreType.HUANGYE:
						HY_UIManager.Instance ().CanOpenShop = true;
						break;
					case StoreType.ALLANCE:
						MyAllianceInfo.m_MyAllianceInfo.CanOpenShop = true;
						break;
					case StoreType.ALLIANCE_FIGHT:

						break;
					default:
						break;
					}

//					Debug.Log ("storeRes.msg:" + storeResp.msg);
//					Debug.Log ("sstoreRes.goodsInfos:" + storeResp.goodsInfos.Count);
//					Debug.Log ("storeRes.hyMoney:" + storeResp.hyMoney);
//					Debug.Log ("sstoreRes.nextRefreshNeedMoney:" + storeResp.nextRefreshNeedMoney);

					if (storeRes.msg == 11)
					{
						//货币不足
						BoxLoad (BoxCallBackType.STORE_REQ);
					}
					else
					{
						LoadGeneralStorePrefab ();
						if (storeRes.msg == 12)
						{
							//商铺刷新成功
							BoxLoad (BoxCallBackType.STORE_REQ);
						}
					}
				}
				
				return true;
			}
			case ProtoIndexes.HY_BUY_GOOD_RESP://购买物品返回
			{
				Debug.Log ("商铺物品购买返回:" + ProtoIndexes.HY_BUY_GOOD_RESP);

				HyBuyGoodResp storeBuyRes = new HyBuyGoodResp();
				storeBuyRes = QXComData.ReceiveQxProtoMessage (p_message,storeBuyRes) as HyBuyGoodResp;
				
				if (storeBuyRes != null)
				{
					Debug.Log ("购买结果：" + storeBuyRes.msg + "//1: 购买成功");
					storeBuyResp = storeBuyRes;
					if (storeBuyRes.msg == 1)
					{
						switch (storeType)
						{
						case StoreType.PVP:
							UIYindao.m_UIYindao.CloseUI ();
							break;
						case StoreType.HUANGYE:
							HY_UIManager.HuangYeData.ShowHuangyeBi (storeBuyRes.remianHyMoney);
							break;
						case StoreType.ALLANCE:
							MyAllianceInfo.m_MyAllianceInfo.ShowMyGongXianZhi (storeBuyRes.remianHyMoney);
							break;
						case StoreType.ALLIANCE_FIGHT:
							
							break;
						default:
							break;
						}

						GeneralStore generalStore = generalStoreObj.GetComponent<GeneralStore> ();
						generalStore.RefreshStoreItem (goodInfo.itemId,storeBuyRes.remianHyMoney);
					}
					BoxLoad (BoxCallBackType.STORE_BUY);
				}
				
				return true;
			}
			}
		}
		return false;
	}

	/// <summary>
	/// Loads the general store prefab.
	/// </summary>
	void LoadGeneralStorePrefab ()
	{
		if (generalStoreObj == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_STORE ),
			                        GeneralStoreLoadBack );
		}
		else
		{
			generalStoreObj.SetActive (true);
			MainCityUI.TryAddToObjectList (generalStoreObj);
			GeneralStore generalStore = generalStoreObj.GetComponent<GeneralStore> ();
			generalStore.GetStoreInfo (storeType,storeResp.goodsInfos,storeResp.remianTime,storeResp.hyMoney,storeResp.nextRefreshNeedMoney);
		}
	}

	void GeneralStoreLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		generalStoreObj = GameObject.Instantiate( p_object ) as GameObject;
		MainCityUI.TryAddToObjectList (generalStoreObj);
		GeneralStore generalStore = generalStoreObj.GetComponent<GeneralStore> ();
		generalStore.GetStoreInfo (storeType,storeResp.goodsInfos,storeResp.remianTime,storeResp.hyMoney,storeResp.nextRefreshNeedMoney);
	}

	/// <summary>
	/// 提示框
	/// </summary>
	/// <param name="tempCallBack">T提示类型</param>
	void BoxLoad (BoxCallBackType tempCallBack)
	{
		string textStr = "";

		switch (tempCallBack)
		{
		case BoxCallBackType.STORE_REQ:
		{
			if (storeResp.msg == 11)//货币不足
			{
				textStr = "\n\n商铺刷新失败，" + MoneyTypeName + "不足";
			}
			else if (storeResp.msg == 12)//刷新成功
			{
				textStr = "\n\n商铺刷新成功";
			}
			break;
		}
		case BoxCallBackType.STORE_BUY:
		{
			if (storeBuyResp.msg == 1)//购买成功
			{
				textStr = "\n\n恭喜使用" + goodInfo.needMoney + MoneyTypeName + "成功兑换物品\n" + goodInfo.itemName;
			}
			else//购买失败
			{
				textStr = "\n\n" + MoneyTypeName + "不足,购买失败";
			}
			break;
		}
		default:
			break;
		}

		Global.CreateBox (QXComData.titleStr, 
		                  MyColorData.getColorString (1,textStr), null, null, 
		                  QXComData.confirmStr, null, 
		                  null);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
	#endregion
	
	#region GeneralRule
	//规则类型
	public enum RuleType
	{
		PVP,
		LUE_DUO,
		HUANGYE,
		ALLIANCE_FIGHT,
	}
	private RuleType ruleType = RuleType.PVP;
	
	private List<string> ruleList;
	
	/// <summary>
	/// 规则
	/// </summary>
	/// <param name="type">挑战规则类型（需要自己添加，特殊处理需求）</param>
	/// <param name="textList">规则list</param>
	/// <param name="rootObjName">根prefab名字</param>
	public void LoadRulesPrefab (RuleType type,List<string> textList)
	{
		ruleType = type;
		ruleList = textList;
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_RULES ),
		                        RulesLoadBack );
	}
	
	void RulesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject rulesObj = GameObject.Instantiate( p_object ) as GameObject;
		
		GeneralRules rules = rulesObj.GetComponent<GeneralRules> ();
		rules.ShowRules (ruleType,ruleList);
	}
	#endregion
}
