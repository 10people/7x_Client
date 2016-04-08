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
        ClientMain.m_isNewOpenFunction = false;
        if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0
           || FreshGuide.Instance().IsActive(100705) && TaskData.Instance.m_TaskInfoDic[100705].progress < 0)
        {
            UIYindao.m_UIYindao.CloseUI();
        }

        MainCityUI.TryRemoveFromObjectList(m_gameObject);
         // Destroy(m_gameObject);
        m_gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EquipSuoData.Instance().m_listNoCurrentShow.Clear();
        CityGlobalData.m_isWashMaxSignalConfirm = false;
    }
}
