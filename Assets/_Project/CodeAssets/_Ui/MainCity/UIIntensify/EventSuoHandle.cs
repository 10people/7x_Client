using UnityEngine;
using System.Collections;

public class EventSuoHandle : MonoBehaviour {

    public delegate void TouchedSend(int index,GameObject obj);
    public event TouchedSend m_Handle;
    public int m_SendIndex;
    void Start()
    {

    }

    void OnClick()
    {
        if (m_Handle != null)
            m_Handle(m_SendIndex,this.gameObject);
    }   
}
