using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

namespace Rank
{
    public class AllianceMemberController : MonoBehaviour
    {
        public RootController m_RootController;
        public ScaleEffectController m_ScaleEffectController;

        [HideInInspector]
        public string m_AllianceName;
        public AlliancePlayerResp m_AlliancePlayerResp;

        public UIScrollView m_ScrollView;
        public UIScrollBar m_ScrollBar;
        public UIGrid m_Grid;
        public UILabel m_AllianceLabel;

        public GameObject m_Prefab;

        [HideInInspector]
        public List<AllianceMemberDetailController> m_AllianceMemberDetailControllerList = new List<AllianceMemberDetailController>();

        public void SetThis()
        {
            if (m_AlliancePlayerResp == null || string.IsNullOrEmpty(m_AllianceName)) return;

            //Set alliance name label.
            m_AllianceLabel.text = m_AllianceName;

            //Clear all grid data.
            while (m_Grid.transform.childCount != 0)
            {
                var child = m_Grid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
            m_AllianceMemberDetailControllerList.Clear();

            //Add grid data.
            if (m_AlliancePlayerResp.player != null)
            {
                for (int i = 0; i < m_AlliancePlayerResp.player.Count; i++)
                {
                    var temp = Instantiate(m_Prefab) as GameObject;
                    var controller = temp.GetComponent<AllianceMemberDetailController>();
                    UtilityTool.ActiveWithStandardize(m_Grid.transform, temp.transform);

                    controller.m_JunZhuInfo = m_AlliancePlayerResp.player[i];
                    controller.SetThis();

                    m_AllianceMemberDetailControllerList.Add(controller);
                }

                m_Grid.Reposition();
            }
        }

        public EventHandler m_CloseHandler;
        public EventHandler m_BackHandler;
        public UIEventListener DragAreaHandler;

        /// <summary>
        /// Close current window.
        /// </summary>
        /// <param name="go"></param>
        private void OnBackClick(GameObject go)
        {
            m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        public void OnDragAreaPress(GameObject go, bool isPress)
        {
            if (isPress)
            {
                m_AllianceMemberDetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }
        }

        /// <summary>
        /// Close current and root window.
        /// </summary>
        /// <param name="go"></param>
        private void OnCloseClick(GameObject go)
        {
            OnBackClick(null);

            m_RootController.OnCloseClick(null);
        }

        private void DoCloseWindow()
        {
            Destroy(gameObject);
        }

        void Awake()
        {
            m_CloseHandler.m_handler += OnCloseClick;
            m_BackHandler.m_handler += OnBackClick;
            DragAreaHandler.onPress += OnDragAreaPress;
        }

        void OnDestroy()
        {
            m_CloseHandler.m_handler -= OnCloseClick;
            m_BackHandler.m_handler -= OnBackClick;
            DragAreaHandler.onPress -= OnDragAreaPress;
        }
    }
}
