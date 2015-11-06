using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipGrowthCloseManagerment : MonoBehaviour
{
    public ScaleEffectController m_ScaleEffectController;

    public GameObject m_gameObject;

    public bool m_isHideChildTransform;


    void OnClick()
    {
        if (m_gameObject != null)
        {
            m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
            m_ScaleEffectController.OnCloseWindowClick();
        }
    }

    void DoCloseWindow()
    {
        //if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_TaskInfoDic[100110].progress < 0)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
            else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress < 0)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
            else if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress < 0)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
            //else if (TaskData.Instance.m_iCurMissionIndex == 100270 && TaskData.Instance.m_TaskInfoDic[100270].progress >= 0)
            //{
            //    UIYindao.m_UIYindao.CloseUI();
            //}
            else if (FreshGuide.Instance().IsActive(100125) && TaskData.Instance.m_TaskInfoDic[100125].progress < 0)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
            //else if (TaskData.Instance.m_iCurMissionIndex == 100270 && TaskData.Instance.m_TaskInfoDic[100270].progress < 0)
            //{
            //    UIYindao.m_UIYindao.CloseUI();
            //}
        }

        MainCityUI.TryRemoveFromObjectList(m_gameObject);
        //Destroy(m_gameObject);
        m_gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EquipSuoData.Instance().m_listNoCurrentShow.Clear();
        CityGlobalData.m_isWashMaxSignalConfirm = false;
    }
}
