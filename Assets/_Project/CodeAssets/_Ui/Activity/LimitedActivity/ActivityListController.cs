using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class ActivityListController : MonoBehaviour, SocketListener
    {
        public RootController m_RootController;

        public UIScrollView m_ScrollView;
        public UIScrollBar m_ScrollBar;
        public UIGrid m_Grid;
        public GameObject m_ActivityItemPrefab;

        public List<ActivityItemController> m_ActivityItemControllerList = new List<ActivityItemController>();

        /// <summary>
        /// Activity list data.
        /// </summary>
        public static List<OpenXianShi> m_openXianShiList = new List<OpenXianShi>();

        public void Refresh()
        {
            LimitActivityData.Instance.RequestData();
        }

        public void DoRefresh(bool isLoadLimitActivityCache)
        {
            while (m_Grid.transform.childCount > 0)
            {
                var child = m_Grid.transform.GetChild(0);

                child.parent = null;
                Destroy(child.gameObject);
            }
            m_ActivityItemControllerList.Clear();

            m_openXianShiList.Sort((item, item2) => item.shunxu.CompareTo(item2.shunxu));
            //Additional safety check, this list cannot be null.
            if (m_openXianShiList != null)
            {
                for (int i = 0; i < m_openXianShiList.Count; i++)
                {
                    //Skip activity closed manually.
                    if (m_openXianShiList[i].state != 10)
                    {
                        continue;
                    }

                    var temp = Instantiate(m_ActivityItemPrefab) as GameObject;
                    UtilityTool.ActiveWithStandardize(m_Grid.transform, temp.transform);

                    var controller = temp.GetComponent<ActivityItemController>();
                    controller.m_ActivityListController = this;
                    controller.m_OpenXianShi = m_openXianShiList[i];

                    controller.Refresh();

                    m_ActivityItemControllerList.Add(controller);
                }
                m_Grid.Reposition();
                UtilityTool.SetScrollBarValue(m_ScrollView, m_ScrollBar, 0.01f);
            }

            if (m_RootController.IsClickFirstActivityItem && m_ActivityItemControllerList != null && m_ActivityItemControllerList.Count > 0)
            {
                m_RootController.IsClickFirstActivityItem = false;

                if (isLoadLimitActivityCache)
                {
                    //goto refresh manually if loading cache.
                    m_ActivityItemControllerList[0].SetSelected();
                    ProcessActivityDetail(RootController.CacheProtoActivityDetail);
                }
                else
                {
                    m_ActivityItemControllerList[0].SendMessage("OnClick", SendMessageOptions.RequireReceiver);
                }
            }
        }

        public bool OnSocketEvent(QXBuffer p_message)
        {
            object temp = new object();

            lock (temp)
            {
                if (p_message != null)
                {
                    switch (p_message.m_protocol_index)
                    {
                        //initialize activity detail with refresh activity item red alert.
                        case ProtoIndexes.S_XIANSHI_INFO_RESP:
                            {
                                MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                                QiXiongSerializer t_qx = new QiXiongSerializer();
                                XinShouXianShiInfo tempInfo = new XinShouXianShiInfo();
                                t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                                ProcessActivityDetail(tempInfo);

                                return true;
                            }

                        default:
                            return false;
                    }
                }
                return false;
            }
        }

        public void ProcessActivityDetail(XinShouXianShiInfo data)
        {
            //Store cache.
            RootController.CacheProtoActivityDetail = data;

            var controllers = m_ActivityItemControllerList.Where(item => item.m_OpenXianShi.typeId == data.typeId).ToList();

            if (controllers.Count == 1)
            {
                controllers[0].m_ShouXianShiInfo = data;
                controllers[0].Refresh();
                controllers[0].RefreshActivityDetail();
            }
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
}
