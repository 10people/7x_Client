using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorplateManagerment : MonoBehaviour
{

    public List<GameObject> m_ListDoorplate0;
    public List<GameObject> m_ListDoorplate1;
    public List<GameObject> m_ListDoorplate2;
    public List<GameObject> m_ListDoorplate3;
	void Start () 
    {
         
        ShowDoorplate();
	}
    void ShowDoorplate()
    {
        int size = 0;
        if (CityGlobalData.m_iAllianceTenentsSceneNum == 0)
        {
            size = m_ListDoorplate0.Count;
        }
        else if (CityGlobalData.m_iAllianceTenentsSceneNum == 1)
        {
            size = m_ListDoorplate1.Count;
        }
        else if (CityGlobalData.m_iAllianceTenentsSceneNum == 2)
        {
            size = m_ListDoorplate2.Count;
        }
        else if (CityGlobalData.m_iAllianceTenentsSceneNum == 3)
        {
            size = m_ListDoorplate3.Count;
        }

        for (int i = 0; i < size; i++)
        {
            if (CityGlobalData.m_iAllianceTenentsSceneNum == 0)
            {
                m_ListDoorplate0[i].SetActive(true);
            }
            else if (CityGlobalData.m_iAllianceTenentsSceneNum == 1)
            {
                m_ListDoorplate1[i].SetActive(true);
            }
            else if (CityGlobalData.m_iAllianceTenentsSceneNum == 2)
            {
                m_ListDoorplate2[i].SetActive(true);
            }
            else if (CityGlobalData.m_iAllianceTenentsSceneNum == 3)
            {
                m_ListDoorplate3[i].SetActive(true);
            }
        }

           
    }
 
}
