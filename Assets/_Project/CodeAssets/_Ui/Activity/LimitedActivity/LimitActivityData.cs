using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;
using System.Collections.Generic;
using System.Linq;
using LimitActivity;

public class LimitActivityData : Singleton<LimitActivityData>, SocketListener
{
    [HideInInspector]
    public bool IsDataReceived;

    [HideInInspector]
    public bool IsOpenZaixianActivity;
    [HideInInspector]
    public bool IsOpenQiriActivity;

    public List<OpenXianShi> m_OpenXianShiList = new List<OpenXianShi>();

    public void RequestData()
    {
        UtilityTool.SendQXMessage(ProtoIndexes.C_XIANSHI_REQ);
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_XIANSHI_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        OpenXianShiResp tempInfo = new OpenXianShiResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        ProcessActivityListData(tempInfo);

                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }

    public void ProcessActivityListData(OpenXianShiResp data, bool isProcessLimitActivityOnly = false)
    {
        //Store to cache.
        RootController.CacheProtoActivityList = data;

        if (data.xianshi != null)
        {
            for (int i = 0; i < data.xianshi.Count; i++)
            {
                if (m_OpenXianShiList == null)
                {
                    m_OpenXianShiList = new List<OpenXianShi>() { data.xianshi[i] };
                }
                else if (!m_OpenXianShiList.Select(item => item.typeId).Contains(data.xianshi[i].typeId))
                {
                    m_OpenXianShiList.Add(data.xianshi[i]);
                }
                else
                {
                    for (int j = 0; j < m_OpenXianShiList.Count; j++)
                    {
                        if (m_OpenXianShiList[j].typeId == data.xianshi[i].typeId)
                        {
                            m_OpenXianShiList[j] = data.xianshi[i];
                            break;
                        }
                    }
                }
            }
        }

        RefreshData(isProcessLimitActivityOnly);

        IsDataReceived = true;
    }

    readonly List<int> MainCityUIActivityID = new List<int>() { 1542000, 1543000 };

    /// <summary>
    /// Include 2 parts, main city ui icon init and limit activity refresh.
    /// </summary>
    void RefreshData(bool isLoadLimitActivityCache)
    {
        if (!isLoadLimitActivityCache)
        {
            //Qiri, Zaixian icon init.
            var temp = m_OpenXianShiList.Select(item => item.typeId).Where(item => MainCityUIActivityID.Contains(item)).ToList();
            //Zaixian activiy
            IsOpenZaixianActivity = temp.Contains(1542000) && (m_OpenXianShiList.Where(item => item.typeId == 1542000).First().state == 10);

            //Qiri activity
            IsOpenQiriActivity = temp.Contains(1543000) && (m_OpenXianShiList.Where(item => item.typeId == 1543000).First().state == 10);

            //Refresh main city ui icon.
            MainCityUIRB.AddOrRemoveButton(15, IsOpenZaixianActivity);
            MainCityUIRB.AddOrRemoveButton(16, IsOpenQiriActivity);

            //Set 2 activities red alert.
            if (IsOpenZaixianActivity)
            {
                MainCityUIRB.SetRedAlert(15, m_OpenXianShiList.Where(item => item.typeId == 1542000).First().isNewAward);
            }
            if (IsOpenQiriActivity)
            {
                MainCityUIRB.SetRedAlert(16, m_OpenXianShiList.Where(item => item.typeId == 1543000).First().isNewAward);
            }

            //Set zaixian activity time calc.
            if (IsOpenZaixianActivity)
            {
                //use shunxu field as time second.
                MainCityUIRB.ShowTimeCalc(m_OpenXianShiList.Where(item => item.typeId == 1542000).First().shunxu);
            }
        }

        //Refresh limit activity window if window showed.
        LimitActivity.ActivityListController.m_openXianShiList = m_OpenXianShiList.Where(item => !MainCityUIActivityID.Contains(item.typeId)).ToList();

        var tempController = FindObjectOfType<LimitActivity.RootController>();
        if (tempController != null)
        {
            tempController.m_ActivityListController.DoRefresh(isLoadLimitActivityCache);
        }

        //Set limit activity red alert.
//        MainCityUIRB.SetRedAlert(14, LimitActivity.ActivityListController.m_openXianShiList.Any(item => item.isNewAward));
       
		PushAndNotificationHelper.SetRedSpotNotification(144, ActivityListController.m_openXianShiList.Any(item => item.isNewAward));
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }
}
