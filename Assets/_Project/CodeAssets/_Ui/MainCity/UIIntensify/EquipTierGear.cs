using UnityEngine;
using System.Collections;

public class EquipTierGear : MonoBehaviour 
{

    public GameObject m_TierGear;
	void Start () 
    {
	
	}

    //void OnPress(bool isDown)
    //{
    //    if (isDown)
    //    {
    //        m_TierGear.SetActive(true);
    //    }
    //    else
    //    {
    //        m_TierGear.SetActive(false);
    //    }
    //}
    void OnClick()
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        m_TierGear.SetActive(true);
    }
}
