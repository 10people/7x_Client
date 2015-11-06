#define HOUSE_TEST
//#undef HOUSE_TEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TenementData : Singleton<TenementData>, SocketProcessor
{
    public HouseBasic m_HouseBasic;

    public Dictionary<int, HouseSimpleInfo> m_AllianceCityTenementDic = new Dictionary<int, HouseSimpleInfo>();
    public HouseExpInfo m_AllianceCityTenementExp = new HouseExpInfo();

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_LM_HOUSE_INFO:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        BatchSimpleInfo AllianceTenementInfo = new BatchSimpleInfo();
                        t_qx.Deserialize(t_tream, AllianceTenementInfo, AllianceTenementInfo.GetType());
                        m_AllianceCityTenementDic.Clear();

                        //Debug.Log("===============Receive house data.");
                        if (AllianceTenementInfo.infos != null)
                        {
                          //  Debug.Log("size :" + AllianceTenementInfo.infos.Count);
                            int size = AllianceTenementInfo.infos.Count;
                            for (int i = 0; i < size; i++)
                            {
                                m_AllianceCityTenementDic.Add(AllianceTenementInfo.infos[i].locationId, AllianceTenementInfo.infos[i]);
                            }
                        }
                        m_AllianceCityTenementExp = AllianceTenementInfo.expInfo;

                        if (m_HouseBasic != null)
                        {
                            m_HouseBasic.RefreshData();
                        }

#if HOUSE_TEST

                        if (MainCityUI.m_MainCityUI != null)
                        {
                            GameObject temp = UtilityTool.FindChild(MainCityUI.m_MainCityUI.transform, "HouseEnter").gameObject;
                            BTNTest temp2 = temp.GetComponent<BTNTest>();
                            temp2.DicKeyStr = string.Join(",", m_AllianceCityTenementDic.Keys.Select(item => item.ToString()).ToArray());
                        }

#endif

                        return true;
                    }
                case ProtoIndexes.S_LM_UPHOUSE_INFO:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        HouseUpdateInfo AllianceTenementInfo = new HouseUpdateInfo();
                        t_qx.Deserialize(t_tream, AllianceTenementInfo, AllianceTenementInfo.GetType());

                        Debug.Log("Receive house update data.");

                        //return if no data existed.
                        if (AllianceTenementInfo.infos == null || AllianceTenementInfo.infos.Count == 0)
                        {
                            return true;
                        }

                        switch (AllianceTenementInfo.code)
                        {
                            //Add to alliance.
                            case 100:
                                foreach (var item in AllianceTenementInfo.infos)
                                {
                                    if (m_AllianceCityTenementDic.ContainsKey(item.locationId))
                                    {
                                //        Debug.LogError("Cancel add house, key: " + item.locationId + " cause house exist.");
                                        continue;
                                    }

                                    m_AllianceCityTenementDic.Add(item.locationId, item);
                                }
                                break;
                            //Remove from alliance.
                            case 200:
                                foreach (var item in AllianceTenementInfo.infos)
                                {
                                    if (!m_AllianceCityTenementDic.ContainsKey(item.locationId))
                                    {
                                        Debug.LogError("Cancel remove house, key: " + item.locationId + " cause house not exist.");
                                        continue;
                                    }

                                    m_AllianceCityTenementDic.Remove(item.locationId);
                                }
                                break;
                            //Update alliance.
                            case 300:
                                foreach (var item in AllianceTenementInfo.infos)
                                {
                                    if (!m_AllianceCityTenementDic.ContainsKey(item.locationId))
                                    {
                                        Debug.LogError("Cancel update house, key: " + item.locationId + " cause house not exist.");
                                        continue;
                                    }

                                    {
                                        m_AllianceCityTenementDic[item.locationId] = item;
                                    }
                                }
                                break;
                            default:
                                Debug.LogError("Not defined code type in S_LM_UPHOUSE_INFO message, code: " + AllianceTenementInfo.code);
                                return false;
                        }

#if HOUSE_TEST

                        if (MainCityUI.m_MainCityUI != null)
                        {
                            GameObject temp = UtilityTool.FindChild(MainCityUI.m_MainCityUI.transform, "HouseEnter").gameObject;
                            BTNTest temp2 = temp.GetComponent<BTNTest>();
                            temp2.DicKeyStr = string.Join(",", m_AllianceCityTenementDic.Keys.Select(item => item.ToString()).ToArray());
                        }

#endif
                        Debug.Log("ChangePortColorChangePortColorChangePortColorChangePortColorChangePortColor");
                        NpcManager.m_NpcManager.ChangePortColor();
                        return true;
                    }
            }
        }
        return false;
    }

    public void RequestData()
    {
        //Debug.LogWarning("================Request house data.");
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LM_HOUSE_INFO);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
