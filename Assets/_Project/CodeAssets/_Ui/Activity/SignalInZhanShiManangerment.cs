using UnityEngine;
using System.Collections;

public class SignalInZhanShiManangerment : MonoBehaviour {
    public GameObject m_ObjAllSignal;
    public EventIndexHandle m_Event;
    void Start()
    {
        if (m_Event != null)
        {
            m_Event.m_Handle += Show;
        }
    }

    void Show(int index)
    {
        SignalInManagerment.m_SignalIn.m_ButtonTouch = SignalInManagerment.TouchType.BUTTON_UP;
        m_ObjAllSignal.SetActive(true);
        gameObject.SetActive(false);
    }
	
    
}
