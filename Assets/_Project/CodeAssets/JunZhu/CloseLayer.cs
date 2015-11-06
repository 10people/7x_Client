using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CloseLayer : MonoBehaviour
{
    public GameObject m_gameObject;

    public bool m_isDestroy;
    public bool m_isHideChildTransform;

    public ScaleEffectController m_ScaleEffectController;

    void OnClick()
    {
        if (m_gameObject != null)
        {
            if (m_ScaleEffectController != null)
            {
                m_ScaleEffectController.CloseCompleteDelegate = DoClose;
                m_ScaleEffectController.OnCloseWindowClick();
            }
            else
            {
                DoClose();
            }
        }
    }

    void DoClose()
    {
        MainCityUI.TryRemoveFromObjectList(m_gameObject);

        if (m_isDestroy == true)
        {
            Destroy(m_gameObject);

            if (UIYindao.m_UIYindao.m_isOpenYindao)
            {

            }

            m_gameObject = null;
        }
        else
        {
            if (m_isHideChildTransform == true)
            {
                foreach (Transform tempTransform in m_gameObject.transform)
                {
                    tempTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                m_gameObject.SetActive(false);
            }
        }
    }
}
