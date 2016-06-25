using UnityEngine;
using System.Collections;

public class EventIndexHandle : MonoBehaviour
{
    public delegate void TouchedSend(int index);
    public event TouchedSend m_Handle;
    public int m_SendIndex;
    public bool m_isDrag = false;

    public bool IsMultiClickCheck = true;
    public float MultiClickDuration = 0.2f;
    public bool m_NeedOnClick = false;
    private float lastClickTime;
    //这里有些东西有些地方要用不要彻底修改

    public void OnClick()
    {
        if (IsMultiClickCheck)
        {
            if (Time.realtimeSinceStartup - lastClickTime < MultiClickDuration)
            {
                return;
            }

            lastClickTime = Time.realtimeSinceStartup;
        }

        if (m_Handle != null)
            m_Handle(m_SendIndex);
    }
    
     

    //void OnDrag(Vector2 delta)
    //{
    //    if (m_Handle != null && !m_isDrag)
    //        m_Handle(m_SendIndex);
    //}
}
