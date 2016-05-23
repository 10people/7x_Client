//#define SHOP_INFO_REQ
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopData : Singleton<ShopData>, SocketProcessor
{

    public static ShopData shopData;

    public enum ShopType//数字代表商铺中位置顺序
    {
        //ORDINARY = 1,//普通
        //MYSTRERT = 2,//神秘
        ROLL = 1,
        WEIWANG = 2,//威望
        GONGXIAN = 3,//贡献
        GONGXUN = 4,//功勋
                    //HUANGYE = 6,//荒野
    }
    public ShopType shopType = ShopType.ROLL;

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

    private readonly Dictionary<ShopType, string[]> shopReqDic = new Dictionary<ShopType, string[]>()
    {
        //string[0]:商铺刷新类型（免费，花钱）
        //string[1]:商铺按钮名字 
        //string[2]:商铺购买类型 
        //string[3]:币种(QXCombData.cs:MoneyType)
        //string[4]:功能名称
        //string[5]:功能开启id(FunctionOpen.xml)
        //string[6]:商铺红点id
        //{ShopType.HUANGYE,new string[]{"10,11","荒野币","1","4","联盟","104","903"}},//"荒野求生","300200"
        {ShopType.ROLL, new string[] {"10,11", "武备坊", "-1", "-1", "", "900", "900"}}, //"荒野求生","300200"
        {ShopType.GONGXIAN, new string[] {"20,21", "贡献", "2", "2", "联盟", "104", "600700"}},
        {ShopType.GONGXUN, new string[] {"30,31", "功勋", "3", "5", "联盟战", "104", "902"}},
        {ShopType.WEIWANG, new string[] {"40,41", "威望", "4", "3", "百战千军", "300100", "300107"}},
        //{ShopType.ORDINARY,new string[]{"50,51","普通","5","1","商铺","9","-1"}},
        //{ShopType.MYSTRERT,new string[]{"60,61","神秘","6","1","商铺","9","901"}},
    };

    public Dictionary<ShopType, ShopResp> shopDic = new Dictionary<ShopType, ShopResp>();
    public WubeiFangInfoResp m_WubeiFangInfoResp;

    private ShopGoodInfo goodInfo;
    private float shopRefreshTime;

    private QXComData.MoneyType moneyType = QXComData.MoneyType.WEIWANG;

    private GameObject ShopPrefab;

    private string textStr;

    private bool isOpenFirstTime = true;
    public bool IsOpenFirstTime
    {
        get { return isOpenFirstTime; }
        set { isOpenFirstTime = value; }
    }

    /// <summary>
    /// The alliance level.
    /// </summary>
    private int allianceLevel;

    void Awake()
    {
        shopData = this;
        SocketTool.RegisterMessageProcessor(this);
    }

    /// <summary>
    /// Sets the type of the shop page.
    /// </summary>
    /// <param name="tempType">Temp type.</param>
    public void SetShopPageType(ShopPageType tempType)
    {
        spType = tempType;
    }

    /// <summary>
    /// Gets the alliance level.
    /// </summary>
    /// <returns>The alliance level.</returns>
    public int GetAllianceLevel()
    {
        return allianceLevel;
    }

    /// <summary>
    /// Determines whether this instance is shop open the specified tempType.
    /// </summary>
    /// <returns><c>true</c> if this instance is shop open the specified tempType; otherwise, <c>false</c>.</returns>
    /// <param name="tempType">Temp type.</param>
    public bool IsShopFunctionOpen(ShopType tempType)
    {
        if (tempType == ShopType.GONGXIAN || tempType == ShopType.GONGXUN)
        {
            return (FunctionOpenTemp.IsHaveID(int.Parse(shopReqDic[tempType][5])) && JunZhuData.Instance().m_junzhuInfo.lianMengId > 0);
        }
        else
        {
            return FunctionOpenTemp.IsHaveID(int.Parse(shopReqDic[tempType][5]));
        }
    }

    /// <summary>
    /// Determines whether this instance is button red the specified sType.
    /// </summary>
    /// <returns><c>true</c> if this instance is button red the specified sType; otherwise, <c>false</c>.</returns>
    /// <param name="sType">S type.</param>
    public bool IsBtnRed(ShopType sType)
    {
        int redId = ShopBtnRedId(sType);

        return redId == -1 ? false : (shopReqDic[sType][5] == "104" ? (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0 ? FunctionOpenTemp.IsShowRedSpotNotification(redId) : false) : FunctionOpenTemp.IsShowRedSpotNotification(redId));
    }

    /// <summary>
    /// Shops the button red identifier.
    /// </summary>
    /// <returns>The button red identifier.</returns>
    /// <param name="sType">S type.</param>
    public int ShopBtnRedId(ShopType sType)
    {
        return int.Parse(shopReqDic[sType][6]);
    }

    /// <summary>
    /// Opens the shop.
    /// </summary>
    /// <param name="tempType">Temp type.</param>
    public void OpenShop(ShopType tempType)
    {
        shopType = tempType;

        if (!Global.m_isOpenShop)
        {
            shopDic.Clear();
            Global.m_isOpenShop = true;
            SetShopPageType(ShopPageType.MAIN_PAGE);
        }

        ShopInfoReq(tempType, ShopReqType.FREE);
    }

    /// <summary>
    /// Refreshs the shop req.
    /// </summary>
    /// <param name="tempType">Temp type.</param>
    public void RefreshShopReq(ShopType tempType)
    {
        int m_vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;
        int refreshLimit = VipTemplate.GetVipInfoByLevel(m_vipLevel).dangpuRefreshLimit;
        Debug.Log("dangpuRefreshLimit:" + refreshLimit);

        shopType = tempType;
        textStr = "立刻刷新货物需要花费" + shopDic[shopType].nextRefreshNeedMoney + QXComData.MoneyName(MoneyType(shopType)) + "\n\n确定刷新货物吗？";
        QXComData.CreateBox(1, textStr, false, RefreshShopAskBack);
    }

    void RefreshShopAskBack(int i)
    {
        if (i == 2)
        {
            int moneyNum = 0;
            //if (shopType == ShopType.MYSTRERT)
            //{
            //    moneyNum = JunZhuData.Instance().m_junzhuInfo.yuanBao;
            //}
            //else
            {
                moneyNum = shopDic[shopType].money;
            }

            if (shopDic[shopType].nextRefreshNeedMoney > moneyNum)
            {
                //缺钱
                RefreshLackMoney();
            }
            else
            {
                ShopInfoReq(shopType, ShopReqType.MONEY);
            }
        }
    }

    /// <summary>
    /// 商铺信息请求
    /// </summary>
    /// <param name="tempType">Temp type.</param>
    /// <param name="tempReqType">Temp req type.</param>
    void ShopInfoReq(ShopType tempType, ShopReqType tempReqType)
    {
        if (tempType == ShopType.ROLL)
        {
            SocketHelper.SendQXMessage(ProtoIndexes.C_WUBEIFANG_INFO);
        }
        else
        {
            shopReqType = tempReqType;

            ShopReq shopReq = new ShopReq();

            string[] reqType = shopReqDic[tempType][0].Split(',');
            shopReq.type = tempReqType == ShopReqType.FREE ? int.Parse(reqType[0]) : int.Parse(reqType[1]);

            QXComData.SendQxProtoMessage(shopReq, ProtoIndexes.HY_SHOP_REQ, ProtoIndexes.HY_SHOP_RESP.ToString());
            Debug.Log("商铺信息请求:" + ProtoIndexes.HY_SHOP_REQ);
        }
    }

    /// <summary>
    /// Shops the goods buy req.
    /// </summary>
    /// <param name="tempInfo">Temp info.</param>
    public void ShopGoodsBuyReq(ShopGoodInfo tempInfo)
    {
        goodInfo = tempInfo;
        //		Debug.Log ("goodInfo.needMoney:" + goodInfo.needMoney);
        //		Debug.Log ("goodInfo.xmlId:" + goodInfo.xmlId);
        //		Debug.Log ("goodInfo.itemId:" + goodInfo.itemId);
        //		Debug.Log ("goodInfo.site:" + goodInfo.site);
        int moneyNum = 0;
        if (goodInfo.moneyType == QXComData.MoneyType.YUANBAO)
        {
            moneyNum = JunZhuData.Instance().m_junzhuInfo.yuanBao;
        }
        else if (goodInfo.moneyType == QXComData.MoneyType.TONGBI)
        {
            moneyNum = JunZhuData.Instance().m_junzhuInfo.jinBi;
        }
        else
        {
            moneyNum = shopDic[shopType].money;
        }

        BuyGoodReq storeBuyReq = new BuyGoodReq();

        storeBuyReq.type = int.Parse(shopReqDic[shopType][2]);
        storeBuyReq.goodId = tempInfo.xmlId;

        QXComData.SendQxProtoMessage(storeBuyReq, ProtoIndexes.HY_BUY_GOOD_REQ, ProtoIndexes.HY_BUY_GOOD_RESP.ToString());
    }

    /// <summary>
    /// Sells the shop goods.
    /// </summary>
    /// <param name="tempList">Temp list.</param>
    public void SellShopGoods(List<SellGoodsInfo> tempList)
    {
        PawnShopGoodsSell shopSellReq = new PawnShopGoodsSell();
        shopSellReq.sellGinfo = tempList;

        QXComData.SendQxProtoMessage(shopSellReq, ProtoIndexes.PAWN_SHOP_GOODS_SELL, ProtoIndexes.PAWN_SHOP_GOODS_SELL_OK.ToString());
        //		Debug.Log ("商铺出售物品请求:" + ProtoIndexes.PAWN_SHOP_GOODS_SELL);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.HY_SHOP_RESP://商铺返回信息
                    {
                        //				Debug.Log ("商铺信息返回:" + ProtoIndexes.HY_SHOP_RESP);
                        ShopResp shopRes = new ShopResp();
                        shopRes = QXComData.ReceiveQxProtoMessage(p_message, shopRes) as ShopResp;

                        if (shopRes != null)
                        {
                            if (shopRes.goodsInfos == null)
                            {
                                shopRes.goodsInfos = new List<DuiHuanInfo>();
                            }

                            //					Debug.Log ("shopRes.msg:" + shopRes.msg);
                            //					Debug.Log ("shopRes.goodsInfos:" + shopRes.goodsInfos.Count);
                            //					Debug.Log ("shopRes.money:" + shopRes.money);
                            //					Debug.Log ("shopRes.nextRefreshNeedMoney:" + shopRes.nextRefreshNeedMoney);

                            if (shopType == ShopType.GONGXIAN)
                            {
                                allianceLevel = shopRes.lmshopLv;
                                //						Debug.Log ("shopRes.lmshopLv:" + shopRes.lmshopLv);
                            }

                            switch (shopReqType)
                            {
                                case ShopReqType.FREE:

                                    shopRefreshTime = shopRes.remianTime;

                                    StopCoroutine("ShopRefresh");
                                    StartCoroutine("ShopRefresh");

                                    if (!shopDic.ContainsKey(shopType))
                                    {
                                        shopDic.Add(shopType, shopRes);
                                    }
                                    else
                                    {
                                        shopDic[shopType].msg = shopRes.msg;
                                        shopDic[shopType].goodsInfos = shopRes.goodsInfos;
                                        shopDic[shopType].nextRefreshNeedMoney = shopRes.nextRefreshNeedMoney;
                                        shopDic[shopType].money = shopRes.money;
                                    }

                                    RefreshMoney(shopType, shopRes.money);

                                    LoadShopPrefab();

                                    break;
                                case ShopReqType.MONEY:

                                    if (shopRes.msg == 11)
                                    {
                                        RefreshLackMoney();
                                    }
                                    else if (shopRes.msg == 12)
                                    {
                                        shopRefreshTime = shopRes.remianTime;
                                        StopCoroutine("ShopRefresh");
                                        StartCoroutine("ShopRefresh");

                                        shopDic[shopType].msg = shopRes.msg;
                                        shopDic[shopType].goodsInfos = shopRes.goodsInfos;
                                        shopDic[shopType].nextRefreshNeedMoney = shopRes.nextRefreshNeedMoney;
                                        shopDic[shopType].money = shopRes.money;

                                        RefreshMoney(shopType, shopRes.money);

                                        LoadShopPrefab();
                                    }
                                    else
                                    {
                                        textStr = "今日刷新次数已用完\n提升V特权等级可增加商铺刷新次数！";
                                        QXComData.CreateBox(1, textStr, true, null);
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
                        //				Debug.Log ("商铺物品购买返回:" + ProtoIndexes.HY_BUY_GOOD_RESP);

                        BuyGoodResp shopBuyRes = new BuyGoodResp();
                        shopBuyRes = QXComData.ReceiveQxProtoMessage(p_message, shopBuyRes) as BuyGoodResp;

                        if (shopBuyRes != null)
                        {
                            //					Debug.Log ("购买结果：" + shopBuyRes.msg + "//0:金钱不足 1:购买成功 2:已经售罄 3:购买商品不存在");
                            switch (shopBuyRes.msg)
                            {
                                case 0:

                                    BuyGoodLackMoney();

                                    break;
                                case 1:

                                    //if (shopType == ShopType.ORDINARY)
                                    //{
                                    //    foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
                                    //    {
                                    //        if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
                                    //        {
                                    //            duiHuan.remainCount -= 1;
                                    //            duiHuan.isChange = shopBuyRes.isChange;
                                    //            break;
                                    //        }
                                    //    }
                                    //    ShopPage.shopPage.InItShopPage(shopType, shopDic[shopType]);
                                    //}
                                    //else
                                    {
                                        //if (shopType == ShopType.MYSTRERT)
                                        //{
                                        //    QXComData.MoneyType type = goodInfo.moneyType;
                                        //    switch (type)
                                        //    {
                                        //        case QXComData.MoneyType.TONGBI:

                                        //            //刷新铜币

                                        //            break;
                                        //        case QXComData.MoneyType.YUANBAO:

                                        //            //刷新元宝

                                        //            break;
                                        //        default:
                                        //            break;
                                        //    }
                                        //}
                                        //else
                                        {
                                            shopDic[shopType].money = shopBuyRes.remianMoney;
                                            RefreshMoney(shopType, shopBuyRes.remianMoney);
                                        }
                                        //							Debug.Log ("shopBuyRes.isChange:" + shopBuyRes.isChange);
                                        foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
                                        {
                                            if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
                                            {
                                                duiHuan.isChange = shopBuyRes.isChange;
                                                break;
                                            }
                                        }
                                        ShopInfoReq(shopType, ShopReqType.FREE);
                                        //ShopPage.shopPage.InItShopPage(shopType);
                                    }

                                    RewardData data = new RewardData(goodInfo.itemId, goodInfo.itemNum);
                                    GeneralRewardManager.Instance().CreateReward(data);

                                    break;
                                case 2:

                                    foreach (DuiHuanInfo duiHuan in shopDic[shopType].goodsInfos)
                                    {
                                        if (goodInfo.xmlId == duiHuan.id && goodInfo.site == duiHuan.site)
                                        {
                                            //								Debug.Log ("goodInfo.xmlId:" + goodInfo.xmlId + "||goodInfo.site:" + goodInfo.site);
                                            //								Debug.Log ("duiHuan.id:" + duiHuan.id + "||duiHuan.site:" + duiHuan.site);
                                            duiHuan.isChange = shopBuyRes.isChange;
                                            break;
                                        }
                                    }
                                    ShopPage.shopPage.SetShopPage(shopType);
                                    textStr = "购买失败，该物品已售罄！";
                                    QXComData.CreateBox(1, textStr, true, null);

                                    break;
                                case 3:

                                    shopDic.Remove(shopType);
                                    ShopInfoReq(shopType, ShopReqType.FREE);
                                    textStr = "商品不存在，请重新尝试购买！";
                                    QXComData.CreateBox(1, textStr, true, null);

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
                        ShopPage.shopPage.InItBagSellPage(1);

                        return true;
                    }
                case ProtoIndexes.S_WUBEIFANG_INFO_RESP: //商铺返回信息
                    {
                        object shopInfo = new WubeiFangInfoResp();
                        SocketHelper.ReceiveQXMessage(ref shopInfo, p_message, ProtoIndexes.S_WUBEIFANG_INFO_RESP);
                        m_WubeiFangInfoResp = shopInfo as WubeiFangInfoResp;
                        LoadShopPrefab();

                        return true;
                    }
            }
        }
        return false;
    }

    /// <summary>
    /// Refreshs the money.
    /// </summary>
    /// <param name="tempType">Temp type.</param>
    /// <param name="tempMoney">Temp money.</param>
    public void RefreshMoney(ShopType tempType, int tempMoney)
    {
        switch (tempType)
        {
            case ShopType.WEIWANG:

                break;
            //case ShopType.HUANGYE:
            //    break;
            case ShopType.GONGXIAN:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Shops the refresh.
    /// </summary>
    /// <returns>The refresh.</returns>
    IEnumerator ShopRefresh()
    {
        while (shopRefreshTime > 0)
        {
            yield return new WaitForSeconds(1);

            shopRefreshTime--;

            if (shopRefreshTime <= 0)
            {
                shopDic.Clear();
                if (Global.m_isOpenShop)
                {
                    if (spType == ShopPageType.MAIN_PAGE)
                    {
                        //if (shopType != ShopType.ORDINARY)
                        //{
                        //刷新商铺
                        ShopInfoReq(shopType, ShopReqType.FREE);
                        //}
                    }
                }
            }
        }
    }

    /// <summary>
    /// Loads the shop prefab.
    /// </summary>
    void LoadShopPrefab()
    {
        if (Global.m_isOpenShop)
        {
            if (ShopPrefab == null)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.QXSHOP),
                                        ShopLoadBack);
            }
            else
            {
                ShopPrefab.SetActive(true);

                InItShopInfo();
            }
        }
    }
    void ShopLoadBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        ShopPrefab = GameObject.Instantiate(p_object) as GameObject;

        InItShopInfo();
    }
    void InItShopInfo()
    {
        MainCityUI.TryAddToObjectList(ShopPrefab);
        ShopPage.shopPage.SetShopPage(shopType);
    }

    /// <summary>
    /// Lacks the money.
    /// </summary>
    void RefreshLackMoney()
    {
        //对应货币不足
        //if (shopType == ShopType.MYSTRERT)
        //{
        //    textStr = "商铺刷新失败，" + QXComData.MoneyName(MoneyType(shopType)) + "不足，是否跳转到充值？";
        //    QXComData.CreateBox(1, textStr, false, TurnToVip);
        //}
        //else
        {
            textStr = "商铺刷新失败，" + QXComData.MoneyName(MoneyType(shopType)) + "不足！";
            QXComData.CreateBox(1, textStr, true, null);
        }
    }
    /// <summary>
    /// Buies the good lack money.
    /// </summary>
    void BuyGoodLackMoney()
    {
        switch (goodInfo.moneyType)
        {
            case QXComData.MoneyType.YUANBAO:
                {
                    Global.CreateFunctionIcon(101);
                    break;
                }
            case QXComData.MoneyType.GONGXIAN:
                {
                    Global.CreateFunctionIcon(701);
                    break;
                }
            case QXComData.MoneyType.GONGXUN:
                {
                    Global.CreateFunctionIcon(2201);
                    break;
                }
            case QXComData.MoneyType.WEIWANG:
                {
                    Global.CreateFunctionIcon(801);
                    break;
                }
        }

        //LanguageTemplate.Text languageTempText = (LanguageTemplate.Text)Enum.ToObject(typeof(LanguageTemplate.Text), int.Parse(QXComData.MoneyDicStr(goodInfo.moneyType, 3)));
        //textStr = LanguageTemplate.GetText(languageTempText);
        //if (goodInfo.moneyType == QXComData.MoneyType.YUANBAO)
        //{
        //    QXComData.CreateBox(1, textStr, true, TurnToVip);
        //}
        //else
        //{
        //    QXComData.CreateBox(1, textStr, true, null);
        //}
    }
    /// <summary>
    /// Turns to vip.
    /// </summary>
    /// <param name="i">The index.</param>
    void TurnToVip(int i)
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
    public string ShopBtnName(int tempIndex)
    {
        ShopType type = (ShopType)Enum.ToObject(typeof(ShopType), tempIndex);

        return shopReqDic[type][1];
    }

    /// <summary>
    /// Moneies the type.
    /// </summary>
    /// <returns>The type.</returns>
    /// <param name="tempType">Temp type.</param>
    public QXComData.MoneyType MoneyType(ShopType tempType)
    {
        return (QXComData.MoneyType)Enum.ToObject(typeof(QXComData.MoneyType), int.Parse(shopReqDic[tempType][3]));
    }

    new void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);

        base.OnDestroy();
    }
}
