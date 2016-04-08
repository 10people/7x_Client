using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

namespace Rank
{
    public class AllianceMemberController : MonoBehaviour
    {
        [HideInInspector]
        public bool isOutterCall = false;

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

        public GameObject FloatButtonPrefab
        {
            get
            {
                return isOutterCall ? floatButtonPrefab : m_RootController.FloatButtonPrefab;
            }

            set
            {
                if (isOutterCall)
                {
                    floatButtonPrefab = value;
                }
                else
                {
                    Debug.LogError("Cannot set cause using prefab in RootController in rank sys mode.");
                }
            }
        }

        private GameObject floatButtonPrefab;

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
                    TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);

                    controller.m_JunZhuInfo = m_AlliancePlayerResp.player[i];
                    controller.SetThis();

                    m_AllianceMemberDetailControllerList.Add(controller);
                }

                m_Grid.Reposition();
            }
        }

        public UIEventListener DragAreaHandler;

        /// <summary>
        /// Close current window.
        /// </summary>
        /// <param name="go"></param>
        public void OnBackClick()
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
        [Obsolete("Close function not exist any more.")]
        public void OnCloseClick()
        {
            OnBackClick();

            if (!isOutterCall)
            {
                m_RootController.OnCloseClick(null);
            }
        }

        private void DoCloseWindow()
        {
            Destroy(gameObject);
        }

        public void ClampScrollView()
        {
            //clamp scroll bar value.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            StartCoroutine(DoClampScrollView());
        }

        IEnumerator DoClampScrollView()
        {
            yield return new WaitForEndOfFrame();

            m_ScrollView.UpdateScrollbars(true);
            float scrollValue = m_ScrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) m_ScrollBar.value = 0.99f;
            if (scrollValue <= 0) m_ScrollBar.value = 0.01f;
        }

        public GameObject TopLeftAnchor;

        void Awake()
        {
            DragAreaHandler.onPress += OnDragAreaPress;

            MainCityUI.setGlobalBelongings(gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);
            MainCityUI.setGlobalTitle(TopLeftAnchor, "成员信息", 0, 0);

            //Load float button prefab if outter call.
            if (isOutterCall)
            {
                CityGlobalData.m_isRightGuide = true;

                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
            }
        }

        void FloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
        {
            FloatButtonPrefab = p_object as GameObject;
        }

        void OnDestroy()
        {
            DragAreaHandler.onPress -= OnDragAreaPress;

            if (isOutterCall)
            {
                MainCityUI.TryRemoveFromObjectList(gameObject);
            }
        }
    }
}
