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

	public bool m_isUpDateZaixian = false;

    public List<OpenXianShi> m_OpenXianShiList = new List<OpenXianShi>();

    public void RequestData()
    {
        SocketHelper.SendQXMessage(ProtoIndexes.C_XIANSHI_REQ);
    }

    private readonly List<int> MainCityUIActivityID = new List<int>() { 1542000, 1543000 };

    /// <summary>
    /// Include 2 parts, main city ui icon init and limit activity refresh.
    /// </summary>
    private void RefreshData(bool isLoadFromCache)
    {
        //set main city ui affairs if not load cache.
        if (!isLoadFromCache)
        {
            //Qiri, Zaixian icon init.
            var temp = m_OpenXianShiList.Select(item => item.typeId).Where(item => MainCityUIActivityID.Contains(item)).ToList();
            //Zaixian activiy
            IsOpenZaixianActivity = temp.Contains(1542000) && (m_OpenXianShiList.Where(item => item.typeId == 1542000).First().state == 10);
            //Qiri activity
            IsOpenQiriActivity = temp.Contains(1543000) && (m_OpenXianShiList.Where(item => item.typeId == 1543000).First().state == 10);

            //Set 2 activities red alert.
			if(m_isUpDateZaixian)
			{
				if (IsOpenZaixianActivity)
				{
					MainCityUI.m_MainCityUI.AddButton(15);
					MainCityUI.SetRedAlert(15, m_OpenXianShiList.Where(item => item.typeId == 1542000).First().isNewAward);
					MainCityUIRB.ShowTimeCalc(m_OpenXianShiList.Where(item => item.typeId == 1542000).First().shunxu);
				}
				else
				{
					if (MainCityUI.m_MainCityUI != null)
					{
						MainCityUI.m_MainCityUI.deleteMaincityUIButton(15);
					}
				}
			}
           
            if (IsOpenQiriActivity)
            {
                if (MainCityUI.m_MainCityUI != null)
                {
                    MainCityUI.m_MainCityUI.AddButton(16);
                    MainCityUI.SetRedAlert(16, true);
                    if (!ClientMain.m_isOpenQIRI)
                    {
                        ClientMain.addPopUP(2, 2, "1", null);
                        ClientMain.m_isOpenQIRI = true;
                    }
                }
            }
            else
            {
                if (MainCityUI.m_MainCityUI != null)
                {
                    MainCityUI.m_MainCityUI.deleteMaincityUIButton(16);
                }
            }

            //Set zaixian activity time calc.
        }

        //Refresh limit activity window if window showed.
        LimitActivity.ActivityListController.m_openXianShiList = m_OpenXianShiList.Where(item => !MainCityUIActivityID.Contains(item.typeId)).ToList();

        var tempController = FindObjectOfType<LimitActivity.RootController>();
        if (tempController != null)
        {
            tempController.m_ActivityListController.Refresh(isLoadFromCache);
        }
    }

    public void ProcessActivityListData(OpenXianShiResp data, bool isLoadFromCache = false)
    {
        if (!isLoadFromCache)
        {
            //Store to cache.
            RootController.CacheProtoActivityList = data;
        }
		m_isUpDateZaixian = false;
        if (data.xianshi != null)
        {
            for (int i = 0; i < data.xianshi.Count; i++)
            {
                //add data to activity list
                if (m_OpenXianShiList == null)
                {
                    m_OpenXianShiList = new List<OpenXianShi>() { data.xianshi[i] };
                }
                else if (!m_OpenXianShiList.Select(item => item.typeId).Contains(data.xianshi[i].typeId))
                {
                    m_OpenXianShiList.Add(data.xianshi[i]);
                }
                //refresh activity list
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
				if(data.xianshi[i].typeId == 1542000)
				{
					m_isUpDateZaixian = true;
				}
            }
        }
        RefreshData(isLoadFromCache);

        IsDataReceived = true;
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_XIANSHI_RESP:
                    {
//				Debug.Log("================1");
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

    #region Mono

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    new void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        base.OnDestroy();
    }

    #endregion
}
