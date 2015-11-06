using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectCountryLayerManagerment : MonoBehaviour 
{
    public List<EventIndexHandle> m_listEvent = new List<EventIndexHandle>();
	void Start () 
    {
        m_listEvent.ForEach(p =>p.m_Handle += TouchEvent);
	}

    void TouchEvent(int index)
    { 
    
    }

    void TouchCallBack(int id)
    {


    }
    
}
