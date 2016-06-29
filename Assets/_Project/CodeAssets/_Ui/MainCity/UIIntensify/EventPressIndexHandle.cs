using UnityEngine;
using System.Collections;

public class EventPressIndexHandle : MonoBehaviour 
{

    public delegate void TouchedSend(int index);
    public event TouchedSend m_Handle;
    public int m_SendIndex;
    void Start()
    {

    }

    public void OnPress(bool isdown )
    {
        if (Input.touchCount > 1)
        {
            return;
        }
        if (!isdown)
        {
            if (m_Handle != null)
                m_Handle(m_SendIndex);
        }
    }   
}
