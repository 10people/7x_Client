using UnityEngine;
using System.Collections;

public class WorshipStepAwardManangerment : MonoBehaviour
{
    public UILabel m_LabTitle;
    public UILabel m_LabLQ;
    public UILabel m_LabYLQ;
    public GameObject m_ObjFirst;
    public GameObject m_ObjScecond;
    public EventIndexHandle m_TouchEvent;
    public EventIndexHandle m_TouchLQEvent;
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
