using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class RootController : MonoBehaviour
    {
        /// <summary>
        /// Used for auto click first activity item.
        /// </summary>
        [HideInInspector]
        public bool IsClickAndShowFirstActivityItemDetail;

        public ActivityListController m_ActivityListController;
        public ActivityDetailController m_ActivityDetailController;
        public GameObject gcRoot;

        public static OpenXianShiResp CacheProtoActivityList;
        public static List<XinShouXianShiInfo> CacheProtoActivityDetailList = new List<XinShouXianShiInfo>();

        private void Refresh()
        {
            LimitActivityData.Instance.RequestData();
        }

        private void LoadFromCache()
        {
            LimitActivityData.Instance.ProcessActivityListData(CacheProtoActivityList, true);
        }

        void OnEnable()
        {
            IsClickAndShowFirstActivityItemDetail = true;
            Refresh();
        }

        void Start()
        {
            if (CacheProtoActivityList != null && CacheProtoActivityDetailList != null && CacheProtoActivityDetailList.Any())
            {
                LoadFromCache();
            }

            //Add guide here.
            if (FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
            {
                UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[100173].m_listYindaoShuju[1]);
            }
        }

        public void Close()
        {
            Destroy(gcRoot);
        }
    }
}
