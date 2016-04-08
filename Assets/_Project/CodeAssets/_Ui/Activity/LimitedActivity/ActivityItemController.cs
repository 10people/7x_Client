using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

namespace LimitActivity
{
    public class ActivityItemController : MonoBehaviour
    {
        public ActivityListController m_ActivityListController;

        public UILabel m_ActivityLabel;
        public UISprite m_CheckOutSprite;
        public UISprite m_RedAlertSprite;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                m_CheckOutSprite.gameObject.SetActive(value);
            }
        }

        private bool isSelected = false;

        /// <summary>
        /// Activity item data
        /// </summary>
        public OpenXianShi m_OpenXianShi;

        /// <summary>
        /// Activity detail data
        /// </summary>
        public XinShouXianShiInfo m_ShouXianShiInfo;

        public void Refresh()
        {
            m_ActivityLabel.text = m_OpenXianShi.name;

            m_RedAlertSprite.gameObject.SetActive(m_OpenXianShi.isNewAward);
        }

        public void RefreshActivityDetail()
        {
            m_ActivityListController.m_RootController.m_ActivityDetailController.m_OpenXianShi = m_OpenXianShi;
            m_ActivityListController.m_RootController.m_ActivityDetailController.m_ShouXianShiInfo = m_ShouXianShiInfo;
            m_ActivityListController.m_RootController.m_ActivityDetailController.Refresh();
        }

        public void OnClick()
        {
            //Add guide here.
            if (m_OpenXianShi != null && m_OpenXianShi.typeId == 1574000 && FreshGuide.Instance().IsActive(100173) && TaskData.Instance.m_TaskInfoDic[100173].progress >= 0)
            {
                UIYindao.m_UIYindao.setOpenYindao(TaskData.Instance.m_TaskInfoDic[100173].m_listYindaoShuju[1]);
            }

            SetSelected();
            RequestData();

            var tempList = RootController.CacheProtoActivityDetailList.Where(item => item.typeId == m_OpenXianShi.typeId).ToList();
            if (tempList.Any())
            {
                m_ActivityListController.ProcessActivityDetail(tempList.First(), true);
            }
        }

        public void SetSelected()
        {
            m_ActivityListController.m_ActivityItemControllerList.ForEach(item => item.IsSelected = false);
            IsSelected = true;
        }

        public void RequestData()
        {
            XinShouXSActivity temp = new XinShouXSActivity() { typeId = m_OpenXianShi.typeId };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.C_XIANSHI_INFO_REQ);
        }
    }
}
