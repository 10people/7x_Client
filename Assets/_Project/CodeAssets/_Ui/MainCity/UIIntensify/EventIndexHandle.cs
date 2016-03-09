using UnityEngine;
using System.Collections;

public class EventIndexHandle : MonoBehaviour 
{
	public delegate void TouchedSend(int index);
	public event TouchedSend m_Handle;
	public int m_SendIndex;
    public bool m_isDrag = false;
	void Start () {
	
	}
 
	public void OnClick () 
	{
		if (m_Handle!= null)
		m_Handle (m_SendIndex);
	}

    void OnDrag(Vector2 delta)
    {
        if (m_Handle != null && !m_isDrag)
            m_Handle(m_SendIndex);
    }
}
