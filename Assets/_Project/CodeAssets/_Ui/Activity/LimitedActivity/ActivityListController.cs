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
        /// Activity list data, contains all activities that opened and closed.
        /// </summary>
        public static List<OpenXianShi> m_openXianShiList = new List<OpenXianShi>();

        public void Refresh(bool isLoadFromCache)
        {
            //Store select status.
            int storedSelectedId = 0;
            if (m_ActivityItemControllerList.Any())
            {
                var temp = m_ActivityItemControllerList.Where(item => item.IsSelected).Select(item => item.m_OpenXianShi.typeId).ToList();
                if (temp.Any())
                {
                    storedSelectedId = temp.First();
                }
            }

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
                    TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);

                    var controller = temp.GetComponent<ActivityItemController>();
                    controller.m_ActivityListController = this;
                    controller.m_OpenXianShi = m_openXianShiList[i];

                    controller.Refresh();

                    m_ActivityItemControllerList.Add(controller);
                }
                m_Grid.Reposition();

                NGUIHelper.SetScrollBarValue(m_ScrollView, m_ScrollBar, 0.01f);
            }

            //Restore select status.
            if (storedSelectedId != 0)
            {
                foreach (var item in m_ActivityItemControllerList)
                {
                    if (storedSelectedId == item.m_OpenXianShi.typeId)
                    {
                        item.SetSelected();
                        break;
                    }
                }
            }

            if (!isLoadFromCache)
            {
                m_ActivityItemControllerList.ForEach(item => item.RequestData());
            }

            if ((m_RootController.IsClickAndShowFirstActivityItemDetail || (m_RootController.m_ActivityDetailController.m_OpenXianShi != null && !m_openXianShiList.Where(item => item.state == 10).Select(item => item.typeId).Contains(m_RootController.m_ActivityDetailController.m_OpenXianShi.typeId))) && m_ActivityItemControllerList != null && m_ActivityItemControllerList.Count > 0)
            {
                m_RootController.IsClickAndShowFirstActivityItemDetail = false;

                if (isLoadFromCache)
                {
                    //goto refresh manually if loading cache.
                    var itemControllers = m_ActivityItemControllerList.Where(item => RootController.CacheProtoActivityDetailList.Select(item2 => item2.typeId).ToList().Contains(item.m_OpenXianShi.typeId)).ToList();

                    if (itemControllers.Any())
                    {
                        itemControllers.First().SetSelected();
                        ProcessActivityDetail(RootController.CacheProtoActivityDetailList.Where(item => item.typeId == itemControllers.First().m_OpenXianShi.typeId).First(), true);
                    }
                    else
                    {
                        m_ActivityItemControllerList[0].SendMessage("OnClick", SendMessageOptions.RequireReceiver);
                    }
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

        public void ProcessActivityDetail(XinShouXianShiInfo data, bool isLoadFromCache = false)
        {
            if (!isLoadFromCache)
            {
                //Store cache.
                var tempList = RootController.CacheProtoActivityDetailList.Where(item => item.typeId == data.typeId).ToList();

                if (tempList.Any())
                {
                    RootController.CacheProtoActivityDetailList.Remove(tempList.First());
                }
                RootController.CacheProtoActivityDetailList.Add(data);
            }

            var controllers = m_ActivityItemControllerList.Where(item => item.m_OpenXianShi.typeId == data.typeId).ToList();

            if (controllers.Count == 1)
            {
                controllers[0].m_ShouXianShiInfo = data;
                controllers[0].Refresh();

                if (controllers[0].IsSelected)
                {
                    controllers[0].RefreshActivityDetail();
                }
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
