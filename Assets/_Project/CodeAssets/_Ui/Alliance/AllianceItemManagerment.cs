using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AllianceItemManagerment : MonoBehaviour {

    //public UISprite m_SpriteIcon;
    public GameObject m_ObjGou;
    public UILabel m_LabName;
    public UILabel m_LabLevel;
    public UILabel m_LabShengWang;
    public UILabel m_LabMengZhu;
    public List<EventIndexHandle> m_listEvent;
    private int ItemId = 0;

    public delegate void OnClick_Touch(int id);
    OnClick_Touch CallBackTouch;
    public delegate void OnClick_Application(int id);
    OnClick_Application CallBackAppliacetion;
	void Start () 
    {
        m_listEvent.ForEach(p => p.m_Handle += TouchIndex);
	}

    void TouchIndex(int index)
    {
        if (index == 0)
        {
            if (CallBackTouch != null)
            {
                CallBackTouch(ItemId);
            }
        }
        else
        {
            if (CallBackAppliacetion != null)
            {
                CallBackAppliacetion(ItemId);
            }
        
        }
    
    }
    void OnClick()
    {
     
    }
    public void ShowAllianceItem(int id, string name, int level, int shengwang, string mengzhu, bool gou, OnClick_Touch callback,OnClick_Application application)
    {
        ItemId = id;
        // m_SpriteIcon.spriteName = icon;
        m_ObjGou.SetActive(gou);
		m_listEvent [1].gameObject.SetActive (!gou);
        m_LabName.text = "<" + name + ">";
        m_LabLevel.text = level.ToString();
        m_LabShengWang.text = shengwang.ToString();
        m_LabMengZhu.text = mengzhu;
        if (callback != null)
        {
            CallBackTouch = callback;
        }
        CallBackAppliacetion = application;
    }
	
	 
}
