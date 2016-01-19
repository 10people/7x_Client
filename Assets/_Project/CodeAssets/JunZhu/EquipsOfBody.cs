using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipsOfBody : MonoBehaviour, SocketProcessor
{ //全局君主身上的装备

    private static EquipsOfBody m_equipsOfBody;

    public Dictionary<int, BagItem> m_equipsOfBodyDic = new Dictionary<int, BagItem>();

    // public Dictionary<int, EquipStrengthResp> m_EquipNeedInfo = new Dictionary<int, EquipStrengthResp>();


    public bool[] m_weapon = new bool[3];

    public bool m_isRefrsehEquips = false;

    public bool m_RefrsehEquipsInfo = false;
    public bool m_WearEquip = false;
    public int m_EquipBuWeiWearing = 0;
    public bool m_Advance = false;
    public EquipJinJieResp m_EquipUpgradeInfo = new EquipJinJieResp();

    public static EquipsOfBody Instance()
    {
        if (m_equipsOfBody == null)
        {
            GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

            m_equipsOfBody = t_gameObject.AddComponent<EquipsOfBody>();
        }

        return m_equipsOfBody;
    }

    #region Mono

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor(this);

		m_equipsOfBody = null;
	}

    /// 请求君主身上装备信息
    void OnEnable()
    {
        RequestEquipInfo();
    }

    /// Obtain JunZhu Data
    public static void RequestEquipInfo()
    {
        EquipInfo tempInfo = new EquipInfo(); //S_EquipInfo 
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qixiong = new QiXiongSerializer();
        t_qixiong.Serialize(t_tream, tempInfo);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipInfo);
    }

    void Update()
    {
        //if (CityGlobalData.m_JunZhuTouJinJieTag)
        //{
        //  CityGlobalData.m_JunZhuTouJinJieTag = false;

        // ShowTishi();
        //}

    }

    #endregion



    #region Network

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_EquipInfo:
                    {
                        ProcessEquipInfo(p_message);
                    } return true;

                case ProtoIndexes.S_EQUIP_JINJIE:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        EquipJinJieResp tempEquipInfo = new EquipJinJieResp();

                        t_qx.Deserialize(t_stream, tempEquipInfo, tempEquipInfo.GetType());

                        m_EquipUpgradeInfo = tempEquipInfo;
                        foreach (KeyValuePair<int, BagItem> item in m_equipsOfBodyDic)
                        {
                            
                                for (int i = 0; i < ZhuangBei.templates.Count; i++)
                                {
                                    if (ZhuangBei.templates[i].id == item.Value.itemId)
                                    {
                                    item.Value.itemId = tempEquipInfo.zbItemId;
                                    item.Value.qiangHuaLv = tempEquipInfo.level;
                                    item.Value.gongJi = tempEquipInfo.gongJi;
                                    item.Value.shengMing = tempEquipInfo.shengMing;
                                    item.Value.fangYu = tempEquipInfo.fangYu;
                                    item.Value.name = ZhuangBei.templates[i].m_name;
                                    item.Value.pinZhi = ZhuangBei.templates[i].pinZhi;
                                        break;
                                    }
                                }
                           
                        }
                        if (UIJunZhu.m_UIJunzhu != null)
                        {
                            m_RefrsehEquipsInfo = true;

                            m_isRefrsehEquips = true;
                        }
                    } return true;
                default: return false;
            }
        }
        return false;
    }

    #endregion



    #region Network Processors

    /// Processes the equip info.
    public void ProcessEquipInfo(QXBuffer p_message)
    {
        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

        QiXiongSerializer t_qx = new QiXiongSerializer();

        EquipInfo tempInfo = new EquipInfo();

        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

        RefreshEquipsDic(tempInfo);
    }

    #endregion

    public void EquipAdvace(long equipID)
    {
        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        EquipJinJie tempAdvanceReq = new EquipJinJie();
        tempAdvanceReq.equipId = equipID;
        t_qx.Serialize(tempStream, tempAdvanceReq);

        byte[] t_protof;
        t_protof = tempStream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_JINJIE, ref t_protof);
    }

    /// 刷新君主装备信息
    private void RefreshEquipsDic(EquipInfo tempInfo)
    {
        EquipsOfBody.Instance().m_equipsOfBodyDic.Clear();

        int tempBuwei = 0;

        for (int i = 0; i < tempInfo.items.Count; i++)
        {
            //穿身上的
            if (tempInfo.items[i].itemId >= 0)
            {
                switch (tempInfo.items[i].buWei)
                {
                    case 1: tempBuwei = 3; break;//刀
                    case 2: tempBuwei = 4; break;//枪
                    case 3: tempBuwei = 5; break;//弓
                    case 11: tempBuwei = 0; break;//头盔
                    case 12: tempBuwei = 8; break;//肩膀
                    case 13: tempBuwei = 1; break;//铠甲
                    case 14: tempBuwei = 7; break;//手套
                    case 15: tempBuwei = 2; break;//裤子
                    case 16: tempBuwei = 6; break;//鞋子
                    default: break;
                }

                tempInfo.items[i].buWei = tempBuwei;

                if (!m_equipsOfBodyDic.ContainsKey(tempBuwei))
                {
                    m_equipsOfBodyDic.Add(tempBuwei, tempInfo.items[i]);
                }
            }
        }
        CityGlobalData.m_RefreshEquipInfo = true;
        EquipsOfBody.Instance().WeaponInfo();
        EquipsOfBody.Instance().m_isRefrsehEquips = true;
        if (m_WearEquip && m_EquipBuWeiWearing != 0)
        {
            EquipsOfBody.Instance().m_WearEquip = false;
            if (GameObject.Find("JunZhuLayerAmend(Clone)") != null)
            {
                switch (m_EquipBuWeiWearing)
                {
                    case 1:
                        {
                            UIJunZhu.m_UIJunzhu.setOpenWeapon(0);
                        }
                        break;
                    case 2:
                        {
                            UIJunZhu.m_UIJunzhu.setOpenWeapon(1);
                        }
                        break;
                    case 3:
                        {
                            UIJunZhu.m_UIJunzhu.setOpenWeapon(2);
                        }
                        break;
                    default:
                        break;
                }
            }

        }
       
    }
    //	private void MapMessage()//获取关卡当前完成信息
    //	{
    // 		PvePageReq mapinfo = new PvePageReq ();
    //		MemoryStream mapStream = new MemoryStream ();
    //		QiXiongSerializer maper = new QiXiongSerializer ();
    //		mapinfo.s_section = -1;
    //		maper.Serialize (mapStream,mapinfo);
    //		
    //		byte[] t_protof;
    //		t_protof = mapStream.ToArray();
    //		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_PAGE_REQ, ref t_protof);
    //	}
    private void ShowTishi()
    {
        if (JunZhuData.Instance().m_junzhuInfo != null &&
            ZhuangBei.templates.Count > 0 &&
            BagData.Instance().m_playerEquipDic.Count > 0)
        {
            if ((m_equipsOfBodyDic.Count != 9 && EquipUnWear()) || UpgRadeIsOn(JunZhuData.Instance().m_junzhuInfo.level))
            {
                CityGlobalData.m_JunZhuTouJinJieTag = true;
            }
            else if (m_equipsOfBodyDic.Count == 9 && !UpgRadeIsOn(JunZhuData.Instance().m_junzhuInfo.level))
            {
                CityGlobalData.m_JunZhuTouJinJieTag = false;
            }
        }
    }

    private bool UpgRadeIsOn(int level)//是否可进阶提示
    {
        foreach (KeyValuePair<int, BagItem> item in m_equipsOfBodyDic)
        {
            for (int i = 0; i < ZhuangBei.templates.Count; i++)
            {
                if (ZhuangBei.templates[i].id == item.Value.itemId && level >= ZhuangBei.templates[i].jinjieLv)
                {
                    foreach (KeyValuePair<long, List<BagItem>> item2 in BagData.Instance().m_playerCaiLiaoDic)
                    {
                        for (int j = 0; j < item2.Value.Count; j++)
                        {
                            if (item2.Value[j].itemId == int.Parse(ZhuangBei.templates[i].jinjieItem))
                            {
                                return item2.Value[j].cnt > int.Parse(ZhuangBei.templates[i].jinjieNum);
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool EquipUnWear()//检测是否有可穿戴的装备
    {
        int tempBuwei = 0;

        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {
            switch (item.Value.buWei)
            {
                case 1:
                    tempBuwei = 3;
                    break;//刀
                case 2:
                    tempBuwei = 4;
                    break;//枪
                case 3:
                    tempBuwei = 5;
                    break;//弓
                case 11:
                    tempBuwei = 0;
                    break;//头盔
                case 12:
                    tempBuwei = 8;
                    break;//肩膀
                case 13:
                    tempBuwei = 1;
                    break;//铠甲
                case 14:
                    tempBuwei = 7;
                    break;//手套
                case 15:
                    tempBuwei = 2;
                    break;//裤子
                case 16:
                    tempBuwei = 6;
                    break;//鞋子
                default:
                    break;
            }
            if (!m_equipsOfBodyDic.ContainsKey(tempBuwei))
            {
                return true;
            }
        }
        return false;
    }
    public bool EquipReplace()//检测是否有可替换的装备
    {
        int tempBuwei = 0;

        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {
            switch (item.Value.buWei)
            {
                case 1:
                    tempBuwei = 3;
                    break;//刀
                case 2:
                    tempBuwei = 4;
                    break;//枪
                case 3:
                    tempBuwei = 5;
                    break;//弓
                case 11:
                    tempBuwei = 0;
                    break;//头盔
                case 12:
                    tempBuwei = 8;
                    break;//肩膀
                case 13:
                    tempBuwei = 1;
                    break;//铠甲
                case 14:
                    tempBuwei = 7;
                    break;//手套
                case 15:
                    tempBuwei = 2;
                    break;//裤子
                case 16:
                    tempBuwei = 6;
                    break;//鞋子
                default:
                    break;
            }
            if (m_equipsOfBodyDic.ContainsKey(tempBuwei) && item.Value.pinZhi > m_equipsOfBodyDic[tempBuwei].pinZhi)
            {

                return true;
            }
        }
        return false;
    }

    #region Utilities

    public bool ContainBagItem_With_DbId(long p_db_id)
    {
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            if (t_item.dbId == p_db_id)
            {
                return true;
            }
        }

        return false;
    }

    public bool GetDesignatedEquipIsStrengthen()
    {
        foreach (KeyValuePair<int, BagItem> item in m_equipsOfBodyDic)
        {
            if (item.Value.itemId == 101002 && item.Value.qiangHuaLv > 0)
            {
                // Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                return true;
            }
        }
        return false;
    }
    public BagItem GetBagItem_With_DbId(long p_db_id)
    {
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            if (t_item.dbId == p_db_id)
            {
                return t_item;
            }
        }

        Debug.LogError("Error, Equips of Body do not have item with dbId: " + p_db_id);

        LogEquips();

        return null;
    }

    public void LogEquips()
    {
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            BagData.LogBagItem(t_item);
        }
    }
    private void WeaponInfo()
    {
        if (m_equipsOfBodyDic.ContainsKey(3))
        {
            m_weapon[0] = true;
        }
        else
        {
            m_weapon[0] = false;
        }

        if (m_equipsOfBodyDic.ContainsKey(4))
        {
            m_weapon[1] = true;
        }
        else
        {
            m_weapon[1] = false;
        }

        if (m_equipsOfBodyDic.ContainsKey(5))
        {
            m_weapon[2] = true;
        }
        else
        {
            m_weapon[2] = false;
        }
    }
    public static int BuWeiRevert(int buwei)
    {
        int tempBuwei = 0;
        switch (buwei)
        {
            case 3: tempBuwei = 1; break;//刀
            case 4: tempBuwei = 2; break;//枪
            case 5: tempBuwei = 3; break;//弓
            case 0: tempBuwei = 11; break;//头盔
            case 8: tempBuwei = 12; break;//肩膀
            case 1: tempBuwei = 13; break;//铠甲
            case 7: tempBuwei = 14; break;//手套
            case 2: tempBuwei = 15; break;//裤子
            case 6: tempBuwei = 16; break;//鞋子
            default: break;
        }
        return tempBuwei;
    }

    public int GetEquipCountByQuality(int quality)
    {
        int count = 0;
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            if (t_item.pinZhi >= quality)
            {
                count++;
            }
        }
        return count;
    }
    public int GetEquipCountByStrengthLevel(int level)
    {
        int count = 0;
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            if (t_item.qiangHuaLv >= level)
            {
                count++;
            }
        }
        return count;
    }
    public bool GetEquipWashQuality()
    {
        int count = 0;
        foreach (BagItem t_item in m_equipsOfBodyDic.Values)
        {
            if (t_item.pinZhi > 1)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}
