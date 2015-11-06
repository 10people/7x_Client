using UnityEngine;
using System.Collections;

public class EquipGrowthWashBack : MonoBehaviour 
{ 
    public GameObject m_HideWashNoSuccess;
    public GameObject m_HideWashSuccess;
    public GameObject m_HideMainInfo;
    public GameObject m_HideWashEquipInfo;
    public GameObject m_HideAttribuiteArmor;
    public GameObject m_HideAttribuiteWeapon;
    public GameObject m_HideEquipInfo;


    public GameObject m_ShowEquipofbody;
   
	void Start ()
    {
	  
	}

    void OnClick()
    { 
      if(m_HideWashNoSuccess != null)
      {
          m_HideWashNoSuccess.SetActive(false);
      }

      if(m_HideWashSuccess != null)
      {
          m_HideWashSuccess.SetActive(false);
      }

      if(m_HideWashEquipInfo != null)
      {
         m_HideWashEquipInfo.SetActive(false);
      }

      if (m_HideAttribuiteArmor != null)
      {
         m_HideAttribuiteArmor.SetActive(false);
      }


      if (m_HideAttribuiteWeapon != null)
      {
         m_HideAttribuiteWeapon.SetActive(false);
      }

      if (m_HideEquipInfo != null)
      {
         m_HideEquipInfo.SetActive(false);
      }

      if (m_ShowEquipofbody != null)
      {
         m_ShowEquipofbody.SetActive(true);
      }
    }
}
