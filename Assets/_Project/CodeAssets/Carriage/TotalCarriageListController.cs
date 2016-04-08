using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class TotalCarriageListController : MonoBehaviour
    {
        public GameObject TotalCarriageListWindow;
        public ScaleEffectController m_ScaleEffectController;

        public UIGrid m_Grid;

        public GameObject ItemPrefab;

        public List<TotalCarriageItemController> m_CarriageItemControllerList = new List<TotalCarriageItemController>();

        public List<CarriageCultureController> m_StoredCarriageControllerList = new List<CarriageCultureController>();

        public UISprite m_ArrowSprite;
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

            DoOpenWindow();
        }

        public void DoOpenWindow()
        {
            TotalCarriageListWindow.SetActive(true);
            m_ScaleEffectController.OpenCompleteDelegate = OnOpenWindowComplete;
            m_ScaleEffectController.OnOpenWindowClick();
        }

        private void NoAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
        {
            GameObject Secrtgb = (GameObject)Instantiate(p_object);
        }

        private void OnOpenWindowComplete()
        {
            SetWindow();
        }

        public void OnCloseWindowClick()
        {
            m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindowComplete;
            m_ScaleEffectController.OnCloseWindowClick();
        }

        private void OnCloseWindowComplete()
        {
            TotalCarriageListWindow.SetActive(false);
        }

        public void SetWindow()
        {
            while (m_Grid.transform.childCount > 0)
            {
                var child = m_Grid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
            m_CarriageItemControllerList.Clear();

            //Set recommanded one.
            m_StoredCarriageControllerList.ForEach(item => item.IsRecommandedOne = false);
            m_StoredCarriageControllerList.Where(item => item.KingName != JunZhuData.Instance().m_junzhuInfo.name && (AllianceData.Instance.IsAllianceNotExist || AllianceData.Instance.g_UnionInfo.name != item.AllianceName) && item.BattleValue < RootManager.Instance.m_CarriageMain.RecommandedScale * JunZhuData.Instance().m_junzhuInfo.zhanLi).OrderByDescending(item => item.Money).Take(RootManager.Instance.m_CarriageMain.RecommandedNum).ToList().ForEach(item => item.IsRecommandedOne = true);
            m_StoredCarriageControllerList = m_StoredCarriageControllerList.OrderByDescending(item => item.IsRecommandedOne).ThenByDescending(item => item.HorseLevel).ThenByDescending(item => CarriageValueCalctor.GetRealValueOfCarriage(item.Money, item.Level, item.BattleValue, item.HorseLevel, item.IsChouRen)).ToList();

            foreach (var item in m_StoredCarriageControllerList)
            {
                var temp = Instantiate(ItemPrefab);
                TransformHelper.ActiveWithStandardize(m_Grid.transform, temp.transform);

                var controller = temp.GetComponent<TotalCarriageItemController>();
                controller.SetThis(item);
                controller.m_TotalCarriageListController = this;
                m_CarriageItemControllerList.Add(controller);
            }

            m_Grid.Reposition();
        }
    }
}
