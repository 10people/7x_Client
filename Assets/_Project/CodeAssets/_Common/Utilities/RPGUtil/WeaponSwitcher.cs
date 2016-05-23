//#define DEBUG_MODE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public enum WeaponType
    {
        Heavy,
        Light,
        Range
    }

    public WeaponType m_Type;
    private WeaponType m_initialType;
    private static Dictionary<WeaponType, GameObject> m_TypePrefabDic = new Dictionary<WeaponType, GameObject>();

    public Transform m_WeaponNode1;
    public Transform m_WeaponNode2;

    public void SwitchWeapon(WeaponType p_type)
    {
        if (m_Type == p_type)
        {
#if DEBUG_MODE
            Debug.LogWarning("Cancel switch weapon cause trying to switch to same type.");
            return;
#endif
        }

        while (m_WeaponNode1.childCount != 0)
        {
            var child = m_WeaponNode1.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        while (m_WeaponNode2.childCount != 0)
        {
            var child = m_WeaponNode2.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        NGUITools.AddChild(m_WeaponNode1.gameObject, m_TypePrefabDic[p_type]);
        if (p_type == WeaponType.Light)
        {
            NGUITools.AddChild(m_WeaponNode2.gameObject, m_TypePrefabDic[p_type]);
        }
    }

    public void RestoreWeapon()
    {
        SwitchWeapon(m_initialType);
    }

    void Awake()
    {
        if (m_TypePrefabDic.Count == 0)
        {
            m_TypePrefabDic.Add(WeaponType.Heavy, Resources.Load<GameObject>("_3D/Models/PlayerWeapon/Heavy/Weapon10101"));
            m_TypePrefabDic.Add(WeaponType.Light, Resources.Load<GameObject>("_3D/Models/PlayerWeapon/Light/Weapon10201"));
            m_TypePrefabDic.Add(WeaponType.Range, Resources.Load<GameObject>("_3D/Models/PlayerWeapon/Range/Weapon10301"));
        }

        m_initialType = m_Type;
    }
}
