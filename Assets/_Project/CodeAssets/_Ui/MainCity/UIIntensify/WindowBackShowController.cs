using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowBackShowController : MonoBehaviour 
{
    public static Dictionary<string, Res2DTemplate.Res> m_BackWindowsDic = new Dictionary<string, Res2DTemplate.Res>();
    public static bool m_isContainKey = false;

    public static string m_SaveKey = "";
    public static int m_SaveEquipBuWei = 0;
    public static void SaveWindowInfo(string name, Res2DTemplate.Res patah)
    {
        if (!m_BackWindowsDic.ContainsKey(name))
        {
            m_BackWindowsDic.Add(name, patah);
        }
    }
 
    public static void CreateSaveWindow(string key)
    {
   
        if (m_BackWindowsDic.ContainsKey(key) && ClientMain.m_listPopUpData.Count > 0)
        {
            m_isContainKey = true;
            m_SaveKey = key;
        }
        else if (m_BackWindowsDic.ContainsKey(key))
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(m_BackWindowsDic[key]),
                                    UILoadCallback);
            m_BackWindowsDic.Remove(key);
        }
    }

    private static void UILoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject temObj = Instantiate(p_object) as GameObject;
 
        UIYindao.m_UIYindao.CloseUI();
        MainCityUI.TryAddToObjectList(temObj);
    }

}
