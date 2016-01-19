using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipWearShowTestMangerment : MonoBehaviour {

    public List<EventIndexHandle> m_listEvent;
    public GameObject m_ObjMain;
    public ScaleEffectController m_SEC;
    void Start ()
    {
        m_SEC.OpenCompleteDelegate +=  ShowInfo;
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
    }

    void ShowInfo()
    {
        gameObject.SetActive(true);
    }
    void OnEnable()
    {
        m_SEC.transform.localScale = Vector3.one;
    }
    void TouchEvent(int index)
    {
        PlayerModelController.m_playerModelController.m_ObjHero.GetComponent<PlayerWeaponManagerment>().ShowWeapon(index);
        foreach(KeyValuePair<int, GameObject> item in PlayerInCityManager.m_playrDic)
        {
            item.Value.GetComponent<PlayerWeaponManagerment>().ShowWeapon(index);
        }
       m_ObjMain.SetActive(false);
       
    }
 
}
