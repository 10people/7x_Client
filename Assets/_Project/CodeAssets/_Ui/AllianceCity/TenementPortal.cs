using UnityEngine;
using System.Collections;

public class TenementPortal : MonoBehaviour
{
	void Start () 
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("otherotherotherotherotherotherother :: " + other.name);
        if (CityGlobalData.m_isAllianceTenentsScene && other.name.Equals("AllianceCityPortal"))
        {
            CityGlobalData.m_isAllianceTenentsScene = false;

            CityGlobalData.m_isAllianceScene = true;
            SceneManager.EnterAllianceCity();
        }
        else if (other.name.Equals("EffectBigHouse") || other.name.Equals("EffectPortal"))
        {
            if (!PlayerModelController.m_isNavMesh)
            {
                TenementManagerment.Instance.NavgationToTenement(other.GetComponent<TenementEnterPortal>().m_indexNum - 1000);
            }
        }
        else if (other.name.Equals("TransferPortal"))
        {
            CityGlobalData.m_isAllianceScene = false;
            CityGlobalData.m_iAllianceTenentsSceneNum = other.GetComponent<TenementEnterPortal>().m_indexNum;
            // Debug.Log(" CityGlobalData.m_iAllianceTenentsSceneNum CityGlobalData.m_iAllianceTenentsSceneNum CityGlobalData.m_iAllianceTenentsSceneNum ::" + CityGlobalData.m_iAllianceTenentsSceneNum);
            CityGlobalData.m_isAllianceTenentsScene = true;
            SceneManager.EnterAllianceCityTenentsCityOne();
        }
        else if (other.name.Equals("RangCollider"))
        {
            other.transform.parent.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(true);
            //other.transform.parent.GetComponent<TenementEnterPortal>().m_ObjRang.SetActive(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("otherotherotherotherotherotherother ::  " + other.name);
        if (other.name.Equals("TransferPortal"))
        {
            other.GetComponent<TenementEnterPortal>().m_ObjRang.SetActive(true);
        }
        else if (other.name.Equals("RangCollider"))
        {
            other.transform.parent.GetComponent<TenementEnterPortal>().m_labName.gameObject.SetActive(false);
        }
    }
}
