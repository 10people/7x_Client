using UnityEngine;
using System.Collections;

public class SettingUpSelectCountryManagerment : MonoBehaviour 
{
    public GameObject m_BackKuang;
    private TaskLayerManager.TaskNeedInfo _SavetaskInfo;
    public UISprite m_SpriteIcon;
    public delegate void OnClick_Touch(int index);
    OnClick_Touch CallBackTouch;
    public int m_CountryNum = 0;
    void Start()
    {

    }

    void OnClick()
    {
        if (CallBackTouch != null)
        {
          CallBackTouch(m_CountryNum);
        }
    }

    public void ShowCounty(int country_Num,OnClick_Touch callback)
    {
        m_CountryNum = country_Num;
        CallBackTouch = callback;
        m_SpriteIcon.spriteName = "nation_" + country_Num.ToString();

    }

    public void RefreshCounty(int country_Num)
    {
        m_CountryNum = country_Num;
        m_SpriteIcon.spriteName = "nation_" + country_Num.ToString();
    }
    public void SelectedShow(bool show)
    {
        m_BackKuang.SetActive(show);
    }


}
