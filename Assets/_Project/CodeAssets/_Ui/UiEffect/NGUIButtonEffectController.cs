using UnityEngine;
using System.Collections;

[AddComponentMenu("Tools/UIButtonEffect")]
public class NGUIButtonEffectController : MonoBehaviour
{
    public bool IsOpenColorEffect = true;
    public bool IsOpenScaleEffect = true;

    private UIButton m_uiButton;
    private UIButtonScale m_uiButtonScale;

    void Start()
    {
        if (m_uiButton != null)
        {
            //m_uiButton.tweenTarget = gameObject;
            m_uiButton.defaultColor = UIEffectParas.ButtonNormalColor;
            m_uiButton.hover = UIEffectParas.ButtonHoverColor;
            m_uiButton.pressed = UIEffectParas.ButtonPressedColor;
            m_uiButton.disabledColor = UIEffectParas.ButtonDisabledColor;
            m_uiButton.dragHighlight = UIEffectParas.IsButtonDragEqualToPressColor;
        }

        if (m_uiButtonScale != null)
        {
            //m_uiButtonScale.tweenTarget = transform;
            m_uiButtonScale.hover = UIEffectParas.ButtonHoverScale;
            m_uiButtonScale.pressed = UIEffectParas.ButtonPressedScale;
            m_uiButtonScale.duration = UIEffectParas.ButtonScaleDuration;
        }
    }

    void Awake()
    {
        if (IsOpenColorEffect)
        {
            m_uiButton = gameObject.GetComponent<UIButton>() ?? gameObject.AddComponent<UIButton>();
        }

        if (IsOpenScaleEffect)
        {
            m_uiButtonScale = gameObject.GetComponent<UIButtonScale>() ?? gameObject.AddComponent<UIButtonScale>();
        }
    }
}
