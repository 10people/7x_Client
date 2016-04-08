using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class MyHelperListController : MonoBehaviour
    {
        public GameObject MyHelperListWindow;
        public ScaleEffectController m_ScaleEffectController;

        public UIGrid m_LeftGrid;
        public UIGrid m_CenterGrid;

        public GameObject LeftItemPrefab;
        public GameObject CenterItemPrefab;

        public List<MyHelperItemController> m_MyHelperLeftItemControllerList = new List<MyHelperItemController>();
        public List<MyHelperItemController> m_MyHelperCenterItemControllerList = new List<MyHelperItemController>();

        public XieZhuJunZhuResp m_StoredXieZhuJunZhuResp;

        public UISprite m_ArrowSprite;
        public UILabel m_MoreLabel;
        private bool IsListOpen = true;

        public void OnOpenCloseListClick()
        {
            if (IsListOpen)
            {
                OnCloseListClick();
            }
            else
            {
                OnOpenListClick();
            }
        }

        public void OnOpenListClick()
        {
            transform.localPosition = new Vector3(-160, -320, 0);
            iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(0, -320, 0), "time", 0.5f, "easetype", "easeOutBack", "islocal", true, "oncomplete", "OnOpenListComplete"));
        }

        void OnOpenListComplete()
        {
            IsListOpen = true;
            m_ArrowSprite.transform.localScale = new Vector3(1, 1, 1);
        }

        public void OnCloseListClick()
        {
            transform.localPosition = new Vector3(0, -320, 0);
            iTween.MoveTo(gameObject, iTween.Hash("position", new Vector3(-160, -320, 0), "time", 0.5f, "easetype", "easeInBack", "islocal", true, "oncomplete", "OnCloseListComplete"));
        }

        void OnCloseListComplete()
        {
            IsListOpen = false;
            m_ArrowSprite.transform.localScale = new Vector3(1, -1, 1);
        }

        public void OnRequestOthersHelpClick()
        {
            if (AllianceData.Instance.IsAllianceNotExist)
            {
                if (FunctionOpenTemp.m_EnableFuncIDList.Contains(104))
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
                                            NoAllianceLoadCallback);
                }
                else
                {
                    ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(104).m_sNotOpenTips);
                }

                return;
            }

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YABIAO_HELP_RSQ);
        }

        public void OnOpenWindowClick()
        {
            if (AllianceData.Instance.IsAllianceNotExist)
            {
                if (FunctionOpenTemp.m_EnableFuncIDList.Contains(104))
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE),
                                            NoAllianceLoadCallback);
                }
                else
                {
                    ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(104).m_sNotOpenTips);
                }

                return;
            }

            //Request data.
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YABIAO_XIEZHU_LIST_REQ);

            MyHelperListWindow.SetActive(true);
            m_ScaleEffectController.OpenCompleteDelegate = OnOpenWindowComplete;
            m_ScaleEffectController.OnOpenWindowClick();
        }

        private void NoAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject Secrtgb = (GameObject)Instantiate(p_object);
        }

        private void OnOpenWindowComplete()
        {
            UpdateGrids();
        }

        public void OnCloseWindowClick()
        {
            m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindowComplete;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        private void OnCloseWindowComplete()
        {
            MyHelperListWindow.SetActive(false);
        }

        public void SetThis(XieZhuJunZhuResp tempInfo)
        {
            m_StoredXieZhuJunZhuResp = tempInfo;

            UpdateGrids();
        }

        public void UpdateGrids()
        {
            if (gameObject.activeInHierarchy)
            {
                m_MoreLabel.gameObject.SetActive(m_StoredXieZhuJunZhuResp != null && m_StoredXieZhuJunZhuResp.xiezhuJz != null && m_StoredXieZhuJunZhuResp.xiezhuJz.Count >= 3);
                UpdateGrid(m_LeftGrid, LeftItemPrefab, m_MyHelperLeftItemControllerList);

                if (MyHelperListWindow.activeInHierarchy)
                {
                    UpdateGrid(m_CenterGrid, CenterItemPrefab, m_MyHelperCenterItemControllerList);
                }
            }
        }

        private void UpdateGrid(UIGrid targetGrid, GameObject prefab, List<MyHelperItemController> storedControllers)
        {
            var toDestroyList = new List<Transform>();
            for (int i = 0; i < targetGrid.transform.childCount; i++)
            {
                var child = targetGrid.transform.GetChild(i);
                if (TransformHelper.SpecificGridItemName.Any(item => child.gameObject.name.Contains(item)))
                {
                    continue;
                }
                toDestroyList.Add(child);
            }
            toDestroyList.ForEach(item =>
            {
                item.parent = null;
                Destroy(item.gameObject);
            });

            storedControllers.Clear();

            if (m_StoredXieZhuJunZhuResp != null && m_StoredXieZhuJunZhuResp.xiezhuJz != null)
            {
                foreach (var item in m_StoredXieZhuJunZhuResp.xiezhuJz)
                {
                    var temp = Instantiate(prefab);
                    TransformHelper.ActiveWithStandardize(targetGrid.transform, temp.transform);

                    var controller = temp.GetComponent<MyHelperItemController>();
                    controller.SetThis(item);
                    storedControllers.Add(controller);
                }

                targetGrid.Reposition();
            }
        }

        void OnEnable()
        {
            //Load cache data and init/refresh ui.
            UpdateGrids();

            //Request data to update data.
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YABIAO_XIEZHU_LIST_REQ);
        }
    }
}
