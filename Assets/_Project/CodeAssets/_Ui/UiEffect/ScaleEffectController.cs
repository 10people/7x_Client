using UnityEngine;
using System.Collections;

[AddComponentMenu("Tools/WindowScaleEffect")]
public class ScaleEffectController : MonoBehaviour
{
    /// <summary>
    /// Is execute open effect when enable object.
    /// </summary>
    public bool IsAutoExecuteOpenEffect = true;

    public delegate void TweenEndDelegate();

    /// <summary>
    /// Clear any "OnOpenComplete" method on this gameobject before using this delegate
    /// </summary>
    public TweenEndDelegate OpenCompleteDelegate;
    /// <summary>
    /// Clear any "OnCloseComplete" method on this gameobject before using this delegate
    /// </summary>
    public TweenEndDelegate CloseCompleteDelegate;

    public void OnOpenWindowClick()
    {
        transform.localScale = UIEffectParas.WindowMinScale;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", UIEffectParas.WindowsMaxScale,
            "time", UIEffectParas.WindowsScaleDuration,
            "easeType", "easeOutBack",
            "islocal", true,
            "oncomplete", "OnOpenComplete",
            "oncompletetarget", gameObject));
    }

    private void OnOpenComplete()
    {
        if (OpenCompleteDelegate != null)
        {
            OpenCompleteDelegate();
        }
    }

    public void OnCloseWindowClick()
    {
        transform.localScale = UIEffectParas.WindowsMaxScale;

        iTween.ScaleTo(gameObject, iTween.Hash(
            "scale", UIEffectParas.WindowMinScale,
            "time", UIEffectParas.WindowsScaleDuration,
            "easeType", "easeInBack",
            "islocal", true,
            "oncomplete", "OnCloseComplete",
            "oncompletetarget", gameObject));
    }

    private void OnCloseComplete()
    {
        if (CloseCompleteDelegate != null)
        {
            CloseCompleteDelegate();
        }
    }

    void OnEnable()
    {
        if (IsAutoExecuteOpenEffect)
        {
            OnOpenWindowClick();
        }
    }
}
