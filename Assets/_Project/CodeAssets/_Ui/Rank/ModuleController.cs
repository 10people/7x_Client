using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

namespace Rank
{
    public abstract class ModuleController : MonoBehaviour
    {
        public List<DetailController> m_DetailControllerList = new List<DetailController>();

        public RootController m_RootController;

        public GameObject m_Prefab;

        public int ModuleIndex;

        [HideInInspector]
        public int CurrentPageIndex = -1;
        [HideInInspector]
        public int TotalPageIndex = -1;
        [HideInInspector]
        public int CurrentNationIndex = -1;

        [HideInInspector]
        public bool IsOneMode
        {
            get { return isOneMode; }
            set
            {
                isOneMode = value;

                //Set left cornor buttons.
                MyDataHandler.gameObject.SetActive(!isOneMode);
                AllDataHandler.gameObject.SetActive(isOneMode);
            }
        }
        private bool isOneMode;
        public string m_OneModeNameStr;

        public UIGrid m_Grid;
        public UIScrollView m_ScrollView;
        public UIScrollBar m_ScrollBar;

        public EventHandler MyDataHandler;
        public EventHandler AllDataHandler;
        public EventHandler OpenSearchHandler;
        public UILabel MiddleLabel;

        public GameObject SearchObject;
        public UILabel SearchLabel;
        public EventHandler SearchButtonHandler;
        public EventHandler SearchDimmerHandler;
        public UIEventListener DragAreaHandler;

        public UILabel NoDataLabel;

        public void RequestAll(int pageID, int nationID)
        {
            //Cancel request error page id, go ahead if request the first page.
            if (pageID != 1)
            {
                if (pageID < 1) return;
                if (TotalPageIndex >= 0 && pageID > TotalPageIndex) return;
            }

            IsOneMode = false;

            RankingReq temp = new RankingReq { rankType = ModuleIndex + 1, guojiaId = nationID, pageNo = pageID };

            SocketHelper.SendQXMessage(temp, ProtoIndexes.RANKING_REQ);

            //Set CurrentNationIndex.
            CurrentNationIndex = nationID;

            //Refresh my rank when request all rank.
            GetMyRank(ModuleIndex + 1, nationID);
        }

        public void RequestOne(string p_name)
        {
            IsOneMode = true;
            m_OneModeNameStr = p_name;

            RankingReq temp = new RankingReq()
            {
                rankType = ModuleIndex + 1,
                name = p_name,
            };
            SocketHelper.SendQXMessage(temp, ProtoIndexes.RANKING_REQ);
        }

        public abstract void GetMyRank(int type, int nationID);

        public abstract void OnMyDataClick(GameObject go);

        public void OnAllDataClick(GameObject go)
        {
            //Reset data to all.
            RequestAll(1, 0);
        }

        public void OnOpenSearchClick(GameObject go)
        {
            SearchObject.SetActive(true);
        }

        public void OnSearchButtonClick(GameObject go)
        {
            RequestOne(SearchLabel.text);
            OnSearchDimmerClick(null);
        }

        public void OnSearchDimmerClick(GameObject go)
        {
            SearchObject.SetActive(false);
        }

        public void OnDragAreaPress(GameObject go, bool isPress)
        {
            if (isPress)
            {
                m_DetailControllerList.ForEach(item => item.DestroyFloatButtons());
            }
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

        public void ScrollToEndIfClose()
        {
            if (m_ScrollView.shouldMoveVertically && m_ScrollBar.value > 0.9f)
            {
                StartCoroutine(DoScrollViewTo(1.0f));
            }
        }

        IEnumerator DoScrollViewTo(float value)
        {
            yield return new WaitForEndOfFrame();

            m_ScrollView.UpdateScrollbars(true);
            m_ScrollBar.value = value;
        }

        public bool IsRefreshToTop = true;

        public void OnEnable()
        {
            IsRefreshToTop = true;
        }

        public void Awake()
        {
            MyDataHandler.m_click_handler += OnMyDataClick;
            AllDataHandler.m_click_handler += OnAllDataClick;
            OpenSearchHandler.m_click_handler += OnOpenSearchClick;
            SearchButtonHandler.m_click_handler += OnSearchButtonClick;
            SearchDimmerHandler.m_click_handler += OnSearchDimmerClick;
            DragAreaHandler.onPress += OnDragAreaPress;
        }

        public void OnDestroy()
        {
            MyDataHandler.m_click_handler -= OnMyDataClick;
            AllDataHandler.m_click_handler -= OnAllDataClick;
            OpenSearchHandler.m_click_handler -= OnOpenSearchClick;
            SearchButtonHandler.m_click_handler -= OnSearchButtonClick;
            SearchDimmerHandler.m_click_handler -= OnSearchDimmerClick;
            DragAreaHandler.onPress -= OnDragAreaPress;
        }

        private bool IsCanSlideRequest = true;

        public void Update()
        {
            float temp = m_ScrollView.GetSingleScrollViewValue();
            if (temp == -100) return;

            //Reset can slide request.
            if (temp > -0.1f && temp < 1.1f)
            {
                IsCanSlideRequest = true;
            }

            if (IsOneMode || !IsCanSlideRequest) return;

            if (temp > 1.25f)
            {
                IsRefreshToTop = true;

                RequestAll(CurrentPageIndex + 1, m_RootController.CurrentNation);
                IsCanSlideRequest = false;
                return;
            }
            if (temp < -0.25f)
            {
                IsRefreshToTop = false;

                RequestAll(CurrentPageIndex - 1, m_RootController.CurrentNation);
                IsCanSlideRequest = false;
                return;
            }
        }
    }
}
