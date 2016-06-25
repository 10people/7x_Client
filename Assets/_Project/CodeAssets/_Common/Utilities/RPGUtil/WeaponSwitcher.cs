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

    public void SwitchRangeWeapon(int p_roleID)
    {
        switch (p_roleID)
        {
            case 1:
                {
                    SwitchWeapon(WeaponType.Range);
                    break;
                }
            case 2:
                {
                    SwitchWeapon(WeaponType.Range, Vector3.zero, new Vector3(0, 0, 90));
                    break;
                }
            case 3:
                {
                    SwitchWeapon(WeaponType.Range, Vector3.zero, new Vector3(0, 0, 90));
                    break;
                }
            case 4:
                {
                    SwitchWeapon(WeaponType.Range, Vector3.zero, new Vector3(0, 0, 180));
                    break;
                }
        }
    }

    public void SwitchWeapon(WeaponType p_type)
    {
        SwitchWeapon(p_type, Vector3.zero, Vector3.zero);
    }

    public void SwitchWeapon(WeaponType p_type, Vector3 p_offsetPos, Vector3 p_offsetRo)
    {
        if (m_Type == p_type)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME))
            {
                Debug.LogWarning("Cancel switch weapon cause trying to switch to same type.");
            }
            return;
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

        switch (p_type)
        {
            case WeaponType.Range:
                {
                    var temp2 = NGUITools.AddChild(m_WeaponNode2.gameObject, m_TypePrefabDic[p_type]);
                    temp2.transform.localPosition = p_offsetPos;
                    temp2.transform.localEulerAngles = p_offsetRo;

                    break;
                }
            case WeaponType.Heavy:
                {
                    var temp1 = NGUITools.AddChild(m_WeaponNode1.gameObject, m_TypePrefabDic[p_type]);
                    temp1.transform.localPosition = p_offsetPos;
                    temp1.transform.localEulerAngles = p_offsetRo;

                    break;
                }
            case WeaponType.Light:
                {
                    var temp1 = NGUITools.AddChild(m_WeaponNode1.gameObject, m_TypePrefabDic[p_type]);
                    temp1.transform.localPosition = p_offsetPos;
                    temp1.transform.localEulerAngles = p_offsetRo;

                    var temp2 = NGUITools.AddChild(m_WeaponNode2.gameObject, m_TypePrefabDic[p_type]);
                    temp2.transform.localPosition = p_offsetPos;
                    temp2.transform.localEulerAngles = p_offsetRo;

                    break;
                }
        }

        m_Type = p_type;
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
