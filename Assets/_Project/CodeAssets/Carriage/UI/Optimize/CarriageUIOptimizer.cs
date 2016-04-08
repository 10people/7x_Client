
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class CarriageUIOptimizer : MonoBehaviour, IUIRootAutoActivator
{
    public GameObject m_window_dead;

    public GameObject m_window_help;


    void Awake()
    {
        {
            UIRootAutoActivator.RegisterAutoActivator(this);
        }
    }

    void OnDestroy()
    {
        {
            UIRootAutoActivator.UnregisterAutoActivator(this);
        }
    }


    public bool IsNGUIVisible() {
        if (m_window_dead.gameObject.activeInHierarchy ||
            m_window_help.gameObject.activeInHierarchy)
        {
            return true;
        }

        return false;
    }
}