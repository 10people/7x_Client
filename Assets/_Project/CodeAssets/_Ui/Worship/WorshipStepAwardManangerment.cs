using UnityEngine;
using System.Collections;

public class WorshipStepAwardManangerment : MonoBehaviour
{
    public UILabel m_LabTitle;
    public EventIndexHandle m_TouchEvent;
    public NGUILongPress NguiLongPress;
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
