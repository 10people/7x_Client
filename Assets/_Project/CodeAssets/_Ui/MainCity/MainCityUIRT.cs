using UnityEngine;
using System.Collections;

/// <summary>
/// UI controller in main city ui right top.
/// </summary>
public class MainCityUIRT : MonoBehaviour
{
    public GameObject PopupObject;

    public GameObject PopupBGObject;

    /// <summary>
    /// Task pop up label info
    /// </summary>
    public UILabel PopupLabel;

    public static void OutterShowPopupDetail(string text, float duration)
    {
        MainCityUI.m_MainCityUI.m_MainCityUIRT.ShowPopupDetail(text, duration);
    }

    private void ShowPopupDetail(string text, float duration)
    {
        StartCoroutine(DoShowPopupDetail(text, duration));
    }

    private IEnumerator DoShowPopupDetail(string text, float duration)
    {
        PopupObject.SetActive(false);

        PopupLabel.text = text;

        PopupObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        PopupObject.SetActive(false);
    }
}
