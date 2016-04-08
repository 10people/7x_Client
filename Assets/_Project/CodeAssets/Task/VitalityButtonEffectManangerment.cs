using UnityEngine;
using System.Collections;

public class VitalityButtonEffectManangerment : MonoBehaviour
{
    public UILabel m_LabTitle;
    public delegate void TouchedSend(GameObject obj);
    public event TouchedSend m_PressLong;
    public event TouchedSend m_PressUp;
    public GameObject m_FirstObj;
    public GameObject m_SecondObj;
    public EventIndexHandle m_TouchLQEvent;


    void OnPress(bool isDown)
    {
        if (isDown)
        {
            m_PressLong(gameObject);
        }
        else
        {
            m_PressUp(gameObject);
        }


    }
 
}
