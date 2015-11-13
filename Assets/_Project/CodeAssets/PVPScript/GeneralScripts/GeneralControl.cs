using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralControl : Singleton<GeneralControl>,SocketProcessor {

	public string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);//确定按钮
	public string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);//取消按钮

	#region
	//商店类型
	public enum StoreType
	{
		PVP,//百战
		HUANGYE,//荒野
		ALLANCE,//联盟
		ALLIANCE_FIGHT,//联盟战
	}
	private StoreType storeType = StoreType.PVP;
	
	//商铺请求类型
	public enum StoreReqType
	{
		FREE,//免费
		USE_MONEY,//使用货币
	}
	private StoreReqType storeReqType = StoreReqType.FREE;

	public enum BoxCallBackType
	{
		STORE_REQ,
		STORE_BUY,
	}
	private BoxCallBackType boxCallBackType = BoxCallBackType.STORE_REQ;//提示框回调类型

	private HyShopResp storeResp;//商铺返回信息
	private HyBuyGoodResp storeBuyResp;//商铺购买返回信息

	private string titleName;//商铺名字

	private int itemId;//兑换物品id
	private string itemName;//兑换物品名字
	private int needMoney;//需要的钱币数量
	
	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}
	
	/// <summary>
	/// 公用商铺信息请求
	/// </summary>
	/// <param name="tempType">商铺类型</param>
	/// <param name="buyType">购买类型</param>
	/// <param name="titleName">商铺名字</param>
	public void GeneralStoreReq (StoreType tempType,StoreReqType tempReqType,string tempTitleName)
	{
		storeType = tempType;
		storeReqType = tempReqType;
		titleName = tempTitleName;
		
		HyShopReq storeReq = new HyShopReq();
		int reqType = 0;
		switch (tempType)
		{
		case StoreType.PVP:
		{
			reqType = tempReqType == StoreReqType.FREE ? 40 : 41;
			break;
		}
		case StoreType.ALLANCE:
		{
			reqType = tempReqType == StoreReqType.FREE ? 20 : 21;
			break;
		}
		case StoreType.ALLIANCE_FIGHT:
		{
			reqType = tempReqType == StoreReqType.FREE ? 30 : 31;
			break;
		}
		case StoreType.HUANGYE:
		{
			reqType = tempReqType == StoreReqType.FREE ? 10 : 11;
			break;
		}
		default:
			break;
		}
		
		storeReq.type = reqType;
		
		QXComData.SendQxProtoMessage (storeReq,ProtoIndexes.HY_SHOP_REQ,"30391");

//		Debug.Log ("商铺信息请求:" + ProtoIndexes.HY_SHOP_REQ);
	}
	
	/// <summary>
	/// 商铺物品购买请求
	/// </summary>
	/// <param name="tempType">购买类型</param>
	/// <param name="tempItemId">物品id</param>
	/// <param name="tempItemName">物品名字</param>
	/// <param name="tempNeedMoney">花费钱币</param>
	public void StoreBuyReq (StoreType tempType,int tempItemId,string tempItemName,int tempNeedMoney)
	{
		storeType = tempType;
		itemId = tempItemId;
		itemName = tempItemName;
		needMoney = tempNeedMoney;

		HyBuyGoodReq storeBuyReq = new HyBuyGoodReq();
		int buyType = 0;
		switch (tempType)
		{
		case StoreType.PVP:
			buyType = 4;
			break;
			
		case StoreType.ALLANCE:
			buyType = 2;
			break;
			
		case StoreType.ALLIANCE_FIGHT:
			buyType = 3;
			break;
			
		case StoreType.HUANGYE:
			buyType = 1;
			break;
			
		default:
			break;
		}
		
		storeBuyReq.type = buyType;
		storeBuyReq.goodId = tempItemId;
		
		QXComData.SendQxProtoMessage (storeBuyReq,ProtoIndexes.HY_BUY_GOOD_REQ,"30393");
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

				object tempTargetObj = new HyShopResp();
				HyShopResp storeRes = QXComData.ReceiveQxProtoMessage (p_message,tempTargetObj) as HyShopResp;
				
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

				object tempTargetObj = new HyBuyGoodResp();
				HyBuyGoodResp storeBuyRes = QXComData.ReceiveQxProtoMessage (p_message,tempTargetObj) as HyBuyGoodResp;
				
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

						GameObject storePrefab = GameObject.Find ("GeneralStore");
						if (storePrefab != null)
						{
							GeneralStore generalStore = storePrefab.GetComponent<GeneralStore> ();
							generalStore.RefreshStoreItem (itemId,storeBuyRes.remianHyMoney);
						}
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
		GameObject storePrefab = GameObject.Find ("GeneralStore");
		if (storePrefab == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GENERAL_STORE ),
			                        GeneralStoreLoadBack );
		}
		else
		{
			GeneralStore generalStore = storePrefab.GetComponent<GeneralStore> ();
			generalStore.GetStoreInfo (storeType,titleName,storeResp.goodsInfos,storeResp.remianTime,storeResp.hyMoney,storeResp.nextRefreshNeedMoney);
		}
	}

	void GeneralStoreLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject storeObj = GameObject.Instantiate( p_object ) as GameObject;
		
		storeObj.name = "GeneralStore";

		GeneralStore generalStore = storeObj.GetComponent<GeneralStore> ();
		generalStore.GetStoreInfo (storeType,titleName,storeResp.goodsInfos,storeResp.remianTime,storeResp.hyMoney,storeResp.nextRefreshNeedMoney);
	}

	/// <summary>
	/// 提示框
	/// </summary>
	/// <param name="tempCallBack">T提示类型</param>
	void BoxLoad (BoxCallBackType tempCallBack)
	{
		boxCallBackType = tempCallBack;
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        BoxLoadCallBack );
	}

	private void BoxLoadCallBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate (p_object) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string titleStr = "提示";
		string textStr = "";

		string moneyType = "";
		switch (storeType)
		{
		case StoreType.PVP:
			moneyType = "威望";
			break;
		case StoreType.HUANGYE:
			moneyType = "荒野币";
			break;
		case StoreType.ALLANCE:
			moneyType = "贡献值";
			break;
		case StoreType.ALLIANCE_FIGHT:
			moneyType = "功勋";
			break;
		default:
			break;
		}

		switch (boxCallBackType)
		{
		case BoxCallBackType.STORE_REQ:
		{
			if (storeResp.msg == 11)//货币不足
			{
				textStr = "\n商铺刷新失败，" + moneyType + "不足";
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
				textStr = "\n恭喜使用" + needMoney + moneyType + "成功兑换物品\n" + itemName;
			}
			else//购买失败
			{
				textStr = "\n\n" + moneyType + "不足,购买失败";
			}
			break;
		}
		default:
			break;
		}

		uibox.setBox(titleStr, MyColorData.getColorString (1,textStr), null, 
		             null, confirmStr, null, null);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
	#endregion
	
	#region
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
