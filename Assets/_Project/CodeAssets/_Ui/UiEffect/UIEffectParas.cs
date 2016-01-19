using UnityEngine;
using System.Collections;

public class UIEffectParas
{
    #region Button Scale

    public static Vector3 ButtonHoverScale = Vector3.one * 1.0f;
    public static Vector3 ButtonPressedScale = Vector3.one * 0.9f;
    public static float ButtonScaleDuration = 0.1f;

    #endregion

    #region Button Color

    public static readonly Color ButtonNormalColor = new Color(1, 1, 1, 1);
    public static readonly Color ButtonHoverColor = new Color(225 / 255.0f, 200 / 255.0f, 150 / 255.0f, 1);
    public static readonly Color ButtonPressedColor = new Color(183 / 255.0f, 163 / 255.0f, 123 / 255.0f, 1);
    public static readonly Color ButtonDisabledColor = new Color(128 / 255.0f, 128 / 255.0f, 128 / 255.0f, 1);
    public static bool IsButtonDragEqualToPressColor = false;

    #endregion

    #region Window Scale

    public static Vector3 WindowMinScale = Vector3.one;
    public static Vector3 WindowsMaxScale = Vector3.one;
    public static float WindowsScaleDuration = 0f;

    #endregion
}
