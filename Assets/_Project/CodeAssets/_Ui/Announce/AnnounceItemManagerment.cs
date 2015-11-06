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

	void Start () 
    {
        m_TouchEvent.m_handler += ShowEvent;
	}



    void ShowEvent(GameObject obj)
    {
        if (TouchCallBack != null)
        {
            TouchCallBack(int.Parse(transform.name));
        }
    
    }
    public void ContentShow(string title,_delegateTouch callback)
    {
        TouchCallBack = callback;

        m_UIlabelTitle.text = title;
 
    }
	
	 
}
