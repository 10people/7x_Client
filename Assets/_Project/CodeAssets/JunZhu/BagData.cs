using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BagData : MonoBehaviour, SocketProcessor
{ //此类为全局类，接收背包数据


    public static BagData m_BatData;

    public List<BagItem> m_bagItemList = new List<BagItem>();

    public List<int> m_HighlightItemList = new List<int>();

    public Dictionary<int, BagItem> m_playerEquipDic = new Dictionary<int, BagItem>();

    public Dictionary<long, List<BagItem>> m_playerCaiLiaoDic = new Dictionary<long, List<BagItem>>();

    public int[] m_arrIYUNum = new int[5];

    [HideInInspector]
    public int m_cardBagCount;

    private const int BAG_ITEM_TYPE_EQUIP = 20000;


    public const int QUALITY_WHITE = 1;
    public const int QUALITY_GREEN = 2;
    public const int QUALITY_BLUE = 3;
    public const int QUALITY_PURPLE = 4;
    public const int QUALITY_ORANGE = 5;

    // 1刀2枪3弓 11头12肩膀13衣服14手套15裤子16鞋子

    public const int ITEM_BUWEI_MIN = 1;
    public const int ITEM_BUWEI_MAX = 16;


    public const int ITEM_TYPE_ENHANCE_WEAPON_MAT = 1;
    public const int ITEM_TYPE_ENHANCE_ARMOR_MAT = 2;


    public static BagData Instance()
    {
        if (m_BatData == null)
        {
            GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

            m_BatData = t_GameObject.AddComponent<BagData>();
        }

        return m_BatData;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        RefreshData();
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);

        m_BatData = null;
    }

    public static void RefreshData()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BagInfo);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_HighLight_item_ids);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_BagInfo:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        BagInfo tempBagInfo = new BagInfo();

                        t_qx.Deserialize(t_stream, tempBagInfo, tempBagInfo.GetType());

                        if (tempBagInfo.items != null)
                        {
                            int Index = 0;
                            for (int i = 0; i < tempBagInfo.items.Count; i++, Index++)
                            {
                                tempBagInfo.items[i].bagIndex = Index;
                                if (tempBagInfo.items[i].itemId == -1)
                                {
                                    tempBagInfo.items.RemoveAt(i);
                                    i--;
                                }
                            }
                            m_bagItemList = tempBagInfo.items;

                            SetPlayerEquipData();

                            RefreshPlayerCaiLiaoData();

                            GetCardBagCount();
                        }

                        // Removed By YuGu, red alert auto updated by PushHelper.
                        //Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                        //MainCityUI.SetRedAlert(12, AllIntensify() || PushAndNotificationHelper.IsShowRedSpotNotification(1210));
                        return true;
                    }
                case ProtoIndexes.S_GET_HighLight_item_ids:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        ErrorMessage tempErrorMsg = new ErrorMessage();
                        t_qx.Deserialize(t_stream, tempErrorMsg, tempErrorMsg.GetType());

                        m_HighlightItemList = tempErrorMsg.errorDesc.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();

                        return true;
                    }
                default: return false;
            }
        }
        return false;
    }

    public int getXmlType(BagItem bagItem)
    {
        int type = -1;
        if (bagItem.itemType != 20000)
        {
            var template = ItemTemp.getItemTempById(bagItem.itemId);
            if (template != null)
            {
                type = template.itemType;
            }
        }
        else
        {
            type = 20000;
        }
        return type;
    }

    public int getXmlFunDisID(BagItem bagItem)
    {
        int disID;
        if (bagItem.itemType != 20000)
        {
            disID = ItemTemp.getItemTempById(bagItem.itemId).funDesc;
        }
        else
        {
            disID = int.Parse(ZhuangBei.getZhuangBeiById(bagItem.itemId).funDesc);
        }
        return disID;
    }


    public List<BagItem> getProp(int[] arrtype)
    {
        for (int i = 0; i < 5; i++)
        {
            m_arrIYUNum[i] = 0;
        }
        List<BagItem> temp = new List<BagItem>();
        for (int i = 0; i < arrtype.Length; i++)
        {
            for (int q = 0; q < m_bagItemList.Count; q++)
            {
                int type = getXmlType(m_bagItemList[q]);
                if (arrtype[i] == type)
                {
                    temp.Add(m_bagItemList[q]);
                    if (type >= 21 && type <= 25)
                    {
                        m_arrIYUNum[type - 21] += m_bagItemList[q].cnt;
                    }
                }
            }
        }
        return temp;
    }

    /// <summary>
    /// set player equipment info and refresh junzhu data.
    /// </summary>
    void SetPlayerEquipData()
    {
        //        Debug.Log("刷新玩家装备信息");

        if (m_playerEquipDic != null)
        {
            m_playerEquipDic.Clear();
        }
        if (m_bagItemList != null)
        {
            for (int i = 0; i < m_bagItemList.Count; i++)
            {
                if (m_bagItemList[i].itemType == CityGlobalData.m_equipFalg)
                {
                    m_playerEquipDic.Add(i, m_bagItemList[i]);
                    //Debug.Log("装备在背包中数据：" + m_bagItemList[i].qiangHuaLv + " " + m_bagItemList[i].gongJi + " " + m_bagItemList[i].fangYu + " " + m_bagItemList[i].shengMing + " " +m_bagItemList[i].tongShuai + " " + m_bagItemList[i].wuYi + " " + m_bagItemList[i].mouLi);
                }

            }

            WetherHaveEquipsLowPinZhi();
            CityGlobalData.m_JunZhuTouJinJieTag = true;
        }

        MainCityUI.SetRedAlert(500005, EquipsOfBody.Instance().EquipUnWear() || EquipsOfBody.Instance().EquipReplace());
        MainCityUI.SetRedAlert(1211, BagData.AllUpgrade());


        //        if (!Global.m_isTianfuUpCan && !Global.m_isNewChenghao && !Global.m_isFuWen)
        //        {
        ////            Debug.Log("BagData.AllUpgrade()BagData.AllUpgrade() ::" + BagData.AllUpgrade());
        //
        //            MainCityUI.SetRedAlert(200, EquipsOfBody.Instance().EquipUnWear() || EquipsOfBody.Instance().EquipReplace() || BagData.AllUpgrade());
        //        }
        //Set junzhu data after setting equipment info.
        //		Debug.Log ("BagData");

        // Added 14436 By LiangXiao, update JunZhuInfo after equipped equip changes.
        // Removed by YuGu, 2015.9.21, server will push JunZhuInfo in that case.
        //JunZhuData.RequestJunZhuInfo();
    }

    void RefreshPlayerCaiLiaoData() //刷新玩家材料数据
    {
        if (m_playerCaiLiaoDic != null)
        {
            m_playerCaiLiaoDic.Clear();
        }
        if (m_bagItemList != null)
        {
            foreach (BagItem tempBagItem in m_bagItemList)
            {
                //if ((tempBagItem.itemType == 1) || (tempBagItem.itemType == 2))
                // if ((tempBagItem.itemType == 10000))

                {
                    if (!m_playerCaiLiaoDic.ContainsKey(tempBagItem.dbId))
                    {
                        m_playerCaiLiaoDic.Add(tempBagItem.dbId, new List<BagItem>() { tempBagItem });
                    }
                    else
                    {
                        m_playerCaiLiaoDic[tempBagItem.dbId].Add(tempBagItem);
                    }
                    if (tempBagItem.itemType >= 101 && tempBagItem.itemType <= 103)
                    {
						if( MainCityUI.m_MainCityUI != null ){
							if( MainCityUI.m_MainCityUI.m_MainCityUIRB != null ){
								MainCityUI.m_MainCityUI.m_MainCityUIRB.setPropUse(tempBagItem.itemId, tempBagItem.cnt);		
							}
						}
                    }
                }
            }

            //			Debug.Log("AllIntensifyAllIntensifyAllIntensifyAllIntensify ::" + AllIntensify());

            MainCityUI.SetRedAlert(1212, AllIntensify() || PushAndNotificationHelper.IsShowRedSpotNotification(1210));

            MainCityUI.SetRedAlert(500005, EquipsOfBody.Instance().EquipUnWear() || EquipsOfBody.Instance().EquipReplace());

            MainCityUI.SetRedAlert(1211, BagData.AllUpgrade());
        }

    }

    public void RefreshPlayerEquips(List<BagItem> tempBagItemList) //刷新玩家装备
    {
        m_playerEquipDic.Clear();

    }

    void GetCardBagCount() //卡包数量
    {
        m_cardBagCount = 0;
        foreach (BagItem tempItem in m_bagItemList)
        {
            if (tempItem.itemId == CityGlobalData.m_cardBag)
            {
                m_cardBagCount += tempItem.cnt;
            }
        }

        //        Debug.Log("卡箱数量：" + m_cardBagCount);
    }




    public BagItem GetBagEquip_With_InstId(long p_inst_id)
    {
        foreach (BagItem t_item in m_playerEquipDic.Values)
        {
            if (t_item.instId == p_inst_id)
            {
                return t_item;
            }
        }

        Debug.LogError("Error, Bag do not have equip with instId: " + p_inst_id);

        LogBagEquips();

        return null;
    }

    public BagItem GetBagEquip_With_DbId(long p_db_id)
    {
        foreach (BagItem t_item in m_playerEquipDic.Values)
        {
            if (t_item.dbId == p_db_id)
            {
                return t_item;
            }
        }

        Debug.LogError("Error, Bag do not have equip with dbId: " + p_db_id);

        LogBagEquips();

        return null;
    }


    public void WetherHaveEquipsLowPinZhi()
    {
        int tempBuwei = 0;
        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {

            switch (item.Value.buWei)
            {
                case 1: tempBuwei = 3; break;//重武器
                case 2: tempBuwei = 4; break;//轻武器
                case 3: tempBuwei = 5; break;//弓
                case 11: tempBuwei = 0; break;//头盔
                case 12: tempBuwei = 8; break;//肩膀
                case 13: tempBuwei = 1; break;//铠甲
                case 14: tempBuwei = 7; break;//手套
                case 15: tempBuwei = 2; break;//裤子
                case 16: tempBuwei = 6; break;//鞋子
                default: break;
            }

            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(tempBuwei) && item.Value.pinZhi > EquipsOfBody.Instance().m_equipsOfBodyDic[tempBuwei].pinZhi)
            {
                if (MainCityUI.m_MainCityUI == null)
                {
                    Debug.Log("MainCity Not Exist, Should Process Data Here.");
                }
                else
                {
                    MainCityUI.m_MainCityUI.m_MainCityUIRB.setPropUse(item.Value.itemId, 1);
                }
            }
            else if (!EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(tempBuwei))
            {
                if (MainCityUI.m_MainCityUI == null)
                {
					#if UNITY_EDITOR
                    Debug.Log("MainCity Not Exist, Should Process Data Here.");
					#endif
                }
                else
                {
                    MainCityUI.m_MainCityUI.m_MainCityUIRB.setPropUse(item.Value.itemId, 1);
                }
            }
        }

    }



    #region Utilities

    public static int GetBagItem_Feed_Exp(BagItem p_item)
    {
        if (BagData.IsEquip(p_item))
        {
            return ZhuangBei.GetItemByID(p_item.itemId).exp;
        }

        if (BagData.IsMaterial(p_item))
        {
            return ItemTemp.getItemTempById(p_item.itemId).effectIds[0];
        }

        Debug.LogError("Error,GetBagItem_Feed_Exp( " + p_item.itemId + " )");

        return 0;
    }

    public bool ContainBagItem_With_DbId(long p_db_id)
    {
        foreach (BagItem t_item in m_bagItemList)
        {
            if (t_item.dbId == p_db_id)
            {
                return true;
            }
        }

        return false;
    }

    public BagItem GetBagItem_With_DbId(long p_db_id)
    {
        foreach (BagItem t_item in m_bagItemList)
        {
            if (t_item.dbId == p_db_id)
            {
                return t_item;
            }
        }

        Debug.LogError("Error, Bag do not have item with dbId: " + p_db_id);

        LogBagEquips();

        return null;
    }

    public static int GetBagItem_Bu_Wei(BagItem p_item)
    {
        return ZhuangBei.GetItem_Bu_Wei(p_item);
    }

    public void LogBagEquips()
    {
        foreach (BagItem t_item in m_playerEquipDic.Values)
        {
            LogBagItem(t_item);
        }
    }

    public static void LogBagItem(BagItem p_item)
    {
        if (p_item == null)
        {
            Debug.LogError("p_item == null.");

            return;
        }

        Debug.Log("bag equip-  itemId: " + p_item.itemId +
                  " itemType: " + p_item.itemType +
                  " buWei: " + p_item.buWei +
                  " name: " + p_item.name +
                  " instId: " + p_item.instId +
                  " cnt: " + p_item.cnt +
                  " pinZhi: " + p_item.pinZhi +
                  " dbId: " + p_item.dbId);
    }

    public static bool IsMaterial(BagItem p_item)
    {
        return IsWeapon_Mat(p_item) || IsArmor_Mat(p_item);
    }

    public static bool IsEquip(BagItem p_item)
    {
        return p_item.itemType == BAG_ITEM_TYPE_EQUIP;
    }

    public static bool IsWeapon(BagItem p_item)
    {
        if (IsEquip(p_item))
        {
            return ZhuangBei.IsWeapon(p_item);
        }

        return false;
    }

    public static bool IsWeapon_Mat(BagItem p_item)
    {
        return p_item.itemType == ITEM_TYPE_ENHANCE_WEAPON_MAT;
    }

    public static bool IsArmor(BagItem p_item)
    {
        if (IsEquip(p_item))
        {
            return ZhuangBei.IsArmor(p_item);
        }

        return false;
    }

    public static bool IsArmor_Mat(BagItem p_item)
    {
        return p_item.itemType == ITEM_TYPE_ENHANCE_ARMOR_MAT;
    }
    public int GetCountByItemId(int id)
    {
        int sum = 0;
        foreach (KeyValuePair<long, List<BagItem>> item in m_playerCaiLiaoDic)
        {
            if (item.Value[0].itemId == id)
            {
                sum += item.Value[0].cnt;
            }
        }
        return sum;
    }

    public int GetCountItemShiYongId(int id)
    {
        int sum = 0;
        foreach (BagItem item in m_bagItemList)
        {
            if (item.itemId == id)
            {
                sum += item.cnt;
            }
        }
        return sum;
    }



    #endregion

    public static bool AllUpgrade()
    {
        foreach (KeyValuePair<int, BagItem> equip in EquipsOfBody.Instance().m_equipsOfBodyDic)
        {
            if (GetMaterialCountByID(int.Parse(ZhuangBei.getZhuangBeiById(equip.Value.itemId).jinjieItem)) >= int.Parse(ZhuangBei.getZhuangBeiById(equip.Value.itemId).jinjieNum))
            {
                return true;
            }
        }
        return false;
    }

    public static int GetMaterialCountByID(int id)
    {
        foreach (KeyValuePair<long, List<BagItem>> item in BagData.m_BatData.m_playerCaiLiaoDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].itemId == id)
                {
                    return item.Value[i].cnt;
                }
            }
        }

        return 0;
    }

    private bool AllIntensify()
    {
        for (int i = 0; i < 9; i++)
        {
            if (AllIntensify(i) != 99)
            {
                return true;
            }
        }
        return false;
    }
    private int AllIntensify(int index)
    {
        int EquipType = 0;
        int EquipExp = 0;
        int equipLevel = 0;
        int pinzhi = 0;
        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
        {
            equipLevel = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaLv;
            EquipExp = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaExp;
            pinzhi = EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi;
            if (EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 3 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 4 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 5)
            {
                EquipType = 1;
            }
            else
            {
                EquipType = 0;
            }

            foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
            {
                for (int i = 0; i < ItemTemp.templates.Count; i++)
                {
                    if (item.Value[0].itemId == ItemTemp.templates[i].id)
                    {
                        if (EquipType == 0 && item.Value[0].itemType == 2)
                        {
                            EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                        }
                        else if (EquipType == 1 && item.Value[0].itemType == 1)
                        {
                            EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                        }
                        else if (item.Value[0].itemType == 6 && pinzhi > item.Value[0].pinZhi)
                        {

                            int tempBuwei = 0;
                            switch (ZhuangBei.GetBuWeiByItemId(item.Value[0].itemId))
                            {
                                case 1: tempBuwei = 3; break;//重武器
                                case 2: tempBuwei = 4; break;//轻武器
                                case 3: tempBuwei = 5; break;//弓
                                case 11: tempBuwei = 0; break;//头盔
                                case 12: tempBuwei = 8; break;//肩膀
                                case 13: tempBuwei = 1; break;//铠甲
                                case 14: tempBuwei = 7; break;//手套
                                case 15: tempBuwei = 2; break;//裤子
                                case 16: tempBuwei = 6; break;//鞋子
                                default: break;
                            }
                            if (EquipType == 1)
                            {
                                if (tempBuwei == 3 || tempBuwei == 4 || tempBuwei == 5)
                                {
                                    if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                    {
                                        EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                    }
                                }
                            }
                            else
                            {
                                if (tempBuwei != 3 && tempBuwei != 4 && tempBuwei != 5)
                                {
                                    if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                    {
                                        EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                    }
                                }

                            }

                        }
                    }
                }
            }

            foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
            {
                int tempBuwei = 0;
                switch (item.Value.buWei)
                {
                    case 1: tempBuwei = 3; break;//重武器
                    case 2: tempBuwei = 4; break;//轻武器
                    case 3: tempBuwei = 5; break;//弓
                    case 11: tempBuwei = 0; break;//头盔
                    case 12: tempBuwei = 8; break;//肩膀
                    case 13: tempBuwei = 1; break;//铠甲
                    case 14: tempBuwei = 7; break;//手套
                    case 15: tempBuwei = 2; break;//裤子
                    case 16: tempBuwei = 6; break;//鞋子
                    default: break;
                }
                if (tempBuwei == EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei && item.Value.pinZhi <= EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi)
                {
                    //  Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                    EquipExp += ZhuangBei.GetItemByID(item.Value.itemId).exp;
                }
            }

            if (ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp > 0)
            {
                if (EquipExp >= ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp)
                {
                    if (equipLevel < JunZhuData.Instance().m_junzhuInfo.level)
                    {
                        return EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei;
                    }
                    else
                    {
                        return 99;
                    }
                }
            }
        }
        return 99;
    }
}
