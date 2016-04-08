using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AnnounceItemManagerment : MonoBehaviour 
{

    public delegate void _delegateTouch(int index);
     _delegateTouch TouchCallBack;
    public UILabel m_UIlabelTitle;
    public UISprite m_SpriteGuang;
    public EventHandler m_TouchEvent;
    public UISprite m_SpriteBack;
    public GameObject m_ObjNew;
    public UILabel m_labNew;
    void Start () 
    {
        m_TouchEvent.m_click_handler += ShowEvent;
	}



    void ShowEvent(GameObject obj)
    {
        if (TouchCallBack != null)
        {
            TouchCallBack(int.Parse(transform.name));
        }
    
    }
    public void ContentShow(NoticeManager.AnnounceInfo announce_info, _delegateTouch callback)
    {
        TouchCallBack = callback;

        m_UIlabelTitle.text = announce_info.title;
        m_ObjNew.SetActive(announce_info._New == 1);
    }
	
	 
}
