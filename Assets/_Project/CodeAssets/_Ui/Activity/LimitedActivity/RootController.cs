using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class RootController : MonoBehaviour
    {
        /// <summary>
        /// Used for auto click first activity item.
        /// </summary>
        [HideInInspector]
        public bool IsClickFirstActivityItem;

        public ActivityListController m_ActivityListController;
        public ActivityDetailController m_ActivityDetailController;

        public static OpenXianShiResp CacheProtoActivityList;
        public static XinShouXianShiInfo CacheProtoActivityDetail;

        private void Refresh()
        {
            m_ActivityListController.Refresh();
        }

        private void LoadFromCache()
        {
            LimitActivityData.Instance.ProcessActivityListData(CacheProtoActivityList);
        }

        void OnEnable()
        {
            IsClickFirstActivityItem = true;
            Refresh();
        }

        void Start()
        {
            if (CacheProtoActivityList != null && CacheProtoActivityDetail != null)
            {
                IsClickFirstActivityItem = true;
                LoadFromCache();
            }
        }
    }
}
