using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagLayerManager : MonoBehaviour
{
    public GameObject RootGameObject;

    public GameObject TopLeftAnchor;

    public ScaleEffectController m_ScaleEffectController;

    public void CloseBagModule()
    {
        m_ScaleEffectController.CloseCompleteDelegate = DoClose;
        m_ScaleEffectController.OnCloseWindowClick();
    }

    void DoClose()
    {
        MainCityUI.TryRemoveFromObjectList(RootGameObject);

        RootGameObject.SetActive(false);
    }

    void Awake()
    {
        MainCityUI.setGlobalBelongings(gameObject, 480 + ClientMain.m_iMoveX + 30, 320 + ClientMain.m_iMoveY);
        MainCityUI.setGlobalTitle(TopLeftAnchor, "背包", 0, 0);
    }
}
