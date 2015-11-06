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
            SetSelected();

            RequestData();
        }

        public void SetSelected()
        {
            m_ActivityListController.m_ActivityItemControllerList.ForEach(item => item.m_CheckOutSprite.gameObject.SetActive(false));
            m_CheckOutSprite.gameObject.SetActive(true);
        }

        public void RequestData()
        {
            XinShouXSActivity temp = new XinShouXSActivity() { typeId = m_OpenXianShi.typeId };
            UtilityTool.SendQXMessage(temp, ProtoIndexes.C_XIANSHI_INFO_REQ);
        }
    }
}
