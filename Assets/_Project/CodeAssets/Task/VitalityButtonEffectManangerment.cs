using UnityEngine;
using System.Collections;

public class VitalityButtonEffectManangerment : MonoBehaviour
{
    public UILabel m_LabTitle;
    public NGUILongPress NguiLongPress;
    public GameObject m_FirstObj;
    public GameObject m_SecondObj;
    public UIEventListener.VoidDelegate OnLongPressFinish
    {
        get { return NguiLongPress.OnLongPressFinish; }
        set { NguiLongPress.OnLongPressFinish = value; }
    }

    public UIEventListener.VoidDelegate OnLongPress
    {
        get { return NguiLongPress.OnLongPress; }
        set { NguiLongPress.OnLongPress = value; }
    }
    public UIEventListener.VoidDelegate OnNormalPress
    {
        get { return NguiLongPress.OnNormalPress; }
        set { NguiLongPress.OnNormalPress = value; }
    }
}
