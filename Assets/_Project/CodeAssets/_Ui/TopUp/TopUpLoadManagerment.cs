using UnityEngine;
using System.Collections;

public class TopUpLoadManagerment : MonoBehaviour
{
    public static TopUpLoadManagerment m_instance;
    private bool isNeed = false;
    private bool isSpecial = false;

    public bool m_isLoaded = false;
    public static TopUpLoadManagerment Instance()
    {
        if (m_instance == null)
        {
            GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

            m_instance = t_gameObject.AddComponent<TopUpLoadManagerment>();
        }

        return m_instance;
    }
 

    public void LoadPrefab(bool ison)//是否需要特殊处理
    {
        if (MainCityUI.IsWindowsExist())
        {
            return;
        }

      isNeed = ison;
        if (!m_isLoaded)
        {
            m_isLoaded = true;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_MAIN_LAYER), RB_LR_75_LoadCallback);
        }
    }
    public void LoadPrefabSpecial(bool ison,bool special)//是否需要特殊处理
    {
        if (MainCityUI.IsWindowsExist())
        {
            return;
        }

        isNeed = ison;
        isSpecial = special;
        if (!m_isLoaded)
        {
            m_isLoaded = true;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_MAIN_LAYER), RB_LR_75_LoadCallback);
        }
    }

    void RB_LR_75_LoadCallback(ref WWW p_www, string p_path, Object p_object)
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
