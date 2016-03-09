using UnityEngine;
using System.Collections;

public class TopUpLoadManagerment : MonoBehaviour
{
    public static TopUpLoadManagerment m_instance;
    private static bool isSpecial = false;
    public static bool m_isLoaded = false;
 

    public static void LoadPrefab(bool ison = false)//是否需要特殊处理
    {
//        Debug.Log("LoadPrefabLoadPrefabLoadPrefabLoadPrefabLoadPrefabLoadPrefab");
        //if (MainCityUI.IsWindowsExist())
        //{
        //    return;
        //}


        isSpecial = ison;
        if (!m_isLoaded)
        {
            m_isLoaded = true;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_MAIN_LAYER), RB_LR_75_LoadCallback);
        }
    }
    static void  RB_LR_75_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);

        if (isSpecial)
        {
            isSpecial = false;
            tempObject.GetComponent<TopUpSpecialChargeManagerment>().SpecialCharge();
        }
       // if (isNeed)
        {
            if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                CityGlobalData.m_isRightGuide = true;
            }
        }
       
    }
}
