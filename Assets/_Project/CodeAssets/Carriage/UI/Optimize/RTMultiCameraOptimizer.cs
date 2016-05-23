using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class RTMultiCameraOptimizer : MonoBehaviour, IUIRootAutoActivator
{
    public List<GameObject> m_ObjectList = new List<GameObject>();


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


    public bool IsNGUIVisible()
    {
        if (m_ObjectList.Any(item=>item.activeInHierarchy))
        {
            return true;
        }

        return false;
    }
}