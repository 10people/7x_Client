using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SetLabelFontWindow : EditorWindow
{
    #region Private Fields
    private enum FontType
    {
        Unity,
        NGUI
    }

    private FontType fontType;

    private UIFont nguiFont;
    private Font unityFont;
    private TextAsset fontTextAsset;

    #endregion

    #region Private Methods

    /// <summary>
    /// Apply font the whole process.
    /// </summary>
    private void SetFont()
    {
        List<UILabel> labelList = new List<UILabel>();

        foreach (Transform root in Selection.transforms)
        {
            labelList.AddRange(root.GetComponentsInChildren<UILabel>(true));
        }

        if (fontType == FontType.Unity)
        {
            labelList.ForEach(item =>
            {
                item.trueTypeFont = unityFont;
                item.bitmapFont = null;
            });
        }
        else if (fontType == FontType.NGUI)
        {
            labelList.ForEach(item =>
            {
                item.bitmapFont = nguiFont;
                item.trueTypeFont = null;
            });
        }
        else
        {
            Debug.LogError("Not correct font type.");
            return;
        }

        FindComponents.DisplayComponents<UILabel>(labelList);
        Debug.LogWarning("Set specific labels ends, count: " + labelList.Count);
    }

    private bool Validate()
    {
        if (fontType == FontType.Unity && unityFont == null)
        {
            return false;
        }

        if (fontType == FontType.NGUI && nguiFont == null)
        {
            return false;
        }
        return true;
    }

    #endregion

    #region Mono

    private void OnGUI()
    {
        EditorGUILayout.LabelField("FontType:");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        fontType = (FontType)EditorGUILayout.EnumPopup(fontType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (fontType == FontType.Unity)
        {
            unityFont = (Font)EditorGUILayout.ObjectField("UnityFont", unityFont, typeof(Font), false);
        }

        if (fontType == FontType.NGUI)
        {

            var fontObject = EditorGUILayout.ObjectField("NGUIFont", nguiFont, typeof(GameObject), false) as GameObject;
            if (fontObject != null)
            {
                nguiFont = fontObject.GetComponent<UIFont>();
            }
        }

        EditorGUILayout.Space();

        var validated = Validate();
        GUI.enabled = validated;
        if (GUILayout.Button("Set Font"))
        {
            SetFont();
        }
        GUI.enabled = true;
    }

    #endregion
}

