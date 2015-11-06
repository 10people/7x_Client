using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FindFontUsageEditorWindow : EditorWindow
{
    #region Private Fields
    private enum FontType
    {
        Unity,
        NGUI
    }

    private const string PrefabExt = ".prefab";

    private static readonly List<string> OriginalPathList = new List<string>
    {
        "_Project",
        "Resources",
    };

    private static string AddedPath = "";

    private string userManual = "Please select the font, only ergodic folder: _Project, Resources.";

    private FontType fontType;

    private UIFont nguiFont;
    private Font unityFont;
    private TextAsset fontTextAsset;

    #endregion

    #region Private Methods

    /// <summary>
    /// Apply font the whole process.
    /// </summary>
    private void FindFontUsage()
    {
        Debug.LogWarning("Find all prefabs begins.");
        var prefabList = FindComponents.FindAllPrefabs(PrefabExt, OriginalPathList, AddedPath);
        Debug.LogWarning("Find all prefabs ends, count: " + prefabList.Count);

        Debug.LogWarning("Load all prefabs begins.");
        Dictionary<GameObject, bool> activeStatus = new Dictionary<GameObject, bool>();
        List<GameObject> loadedObjects = FindComponents.LoadAllPrefabs(prefabList, activeStatus);
        Debug.LogWarning("Load all prefabs ends, count: " + loadedObjects.Count);

        Debug.LogWarning("Active all prefabs begins.");
        FindComponents.ActiveAllPrefabs(activeStatus);
        Debug.LogWarning("Active all prefabs ends.");

        Debug.LogWarning("Find specific labels begins.");
        List<UILabel> labelList = FindComponents.FindAllComponents<UILabel>(loadedObjects);
        Debug.LogWarning("Find all labels count: " + labelList.Count);
        if (fontType == FontType.Unity)
        {
            labelList = labelList.Where(item => item.trueTypeFont == unityFont).ToList();
        }
        else if (fontType == FontType.NGUI)
        {

            labelList = labelList.Where(item => item.bitmapFont == nguiFont).ToList();
        }
        else
        {
            Debug.LogError("Not correct font type.");
            return;
        }
        FindComponents.DisplayComponents<UILabel>(labelList);
        Debug.LogWarning("Find specific labels ends, count: " + labelList.Count);

        //Display not contained words.
        if (fontType == FontType.Unity)
        {
            Debug.Log("Don't analyse unity font.");
        }
        else if (fontType == FontType.NGUI)
        {
            //Acutally, this is oridiary ASCII transfer, UTF-8 and GB2312 not used.

            List<string> tempFontString = fontTextAsset.text.Split(new string[] { "char id=" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            tempFontString.RemoveAt(0);
            tempFontString = tempFontString.Select(item => item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).First()).ToList();
            List<int> tempFontID = tempFontString.Select(item => Convert.ToInt32(item)).Distinct().ToList();
            List<char> tempLabelChar = labelList.Select(item => item.text).SelectMany(item => item).Distinct().ToList();
            //List<byte[]> tempLabelStr = tempLabelChar.Select(item => Encoding.GetEncoding("UTF-8").GetBytes(item.ToString())).ToList();
            //List<int> tempLabelID = tempLabelStr.Select(item => (int)((item[0] << 16 | item[1] << 8 | item[2]))).ToList();
            List<int> tempLabelID = tempLabelChar.Select(item => (int)(item)).ToList();
            List<int> notContainedId = tempLabelID.Where(item => !tempFontID.Contains(item)).ToList();
            //List<string> notContainedStrings = notContainedId.Select(item =>
            //{
            //    byte[] temp = BitConverter.GetBytes(item);
            //    Array.Reverse(temp);
            //    return Encoding.GetEncoding("UTF-8").GetString(temp);
            //}
            //).ToList();            
            char[] notContainedStrings = notContainedId.Select(item => (char)item).ToArray();
            Debug.Log("not contained string: " + new string(notContainedStrings));
        }
        else
        {
            Debug.LogError("Not correct font type.");
            return;
        }

        //Debug.LogWarning("Restore active status begins.");
        //FindWidget.RestoreActiveStatus(activeStatus);
        //Debug.LogWarning("Restore active status ends.");

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
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
        userManual = GUILayout.TextArea(userManual, "Label");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("ngui font text:");
        EditorGUILayout.Space();
        fontTextAsset = (TextAsset)EditorGUILayout.ObjectField("fontText", fontTextAsset, typeof(TextAsset), false);

        EditorGUILayout.Space();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Specific path:");
        EditorGUILayout.Space();
        AddedPath = EditorGUILayout.TextField(AddedPath);

        EditorGUILayout.Space();

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
        if (GUILayout.Button("FindUsage"))
        {
            FindFontUsage();
        }
        GUI.enabled = true;
    }

    #endregion
}

