using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CancelButtonManagerment : MonoBehaviour 
{

    public enum LayerType
   {
     SETTING_UP_ENUM,
     OTHER_ENUM,
   };

    public List<GameObject> m_Objectlist = new List<GameObject>();
    public LayerType ControllerTYpe;

    public bool m_isDestroy = false ;
	void Start ()
    {
	
	}

    void OnClick()
    {
        switch (ControllerTYpe)
        {
            case LayerType.SETTING_UP_ENUM:
                {
                    ObjectCharge();
                }
                break;
            default:
                break;

        }

    
    }

    void ObjectCharge()
    {
        int size = m_Objectlist.Count;
    
         for (int i = 0; i < size; i++)
         {
             if (m_isDestroy)
             {
               Destroy(m_Objectlist[i]);
             }
             else 
             {
              m_Objectlist[i].SetActive(false);
             }
         }
    }
 
}
