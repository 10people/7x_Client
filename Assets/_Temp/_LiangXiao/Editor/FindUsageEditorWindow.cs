using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

public class FindUsageEditorWindow : EditorWindow
{
    #region Private Fields

    private enum WhereToFind
    {
        Hierarchy,
        Project
    }

    private WhereToFind m_whereToFind;

    private Object m_specificComponent;

    private const string PrefabExt = ".prefab";

    private static readonly List<string> OriginalPathList = new List<string>
    {
        "_Project",
        "Resources",
    };

    private static string AddedPath = "";

    private string userManual = "only ergodic folder: _Project, Resources.";

    #endregion

    #region Private Methods

    /// <summary>
    /// Find the whole process.
    /// </summary>
    private void FindUsage()
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

        Debug.LogWarning("Find components begins.");
        List<Component> componentList = FindComponents.FindAllComponents(loadedObjects, m_specificComponent.GetType());
        Debug.LogWarning("Find components ends, count: " + componentList.Count);

        Debug.LogWarning("Display all labels count: " + componentList.Count);
        FindComponents.DisplayComponents<Component>(componentList);
        Debug.LogWarning("Display components ends, count: " + componentList.Count);

        //Debug.LogWarning("Restore active status begins.");
        //FindWidget.RestoreActiveStatus(activeStatus);
        //Debug.LogWarning("Restore active status ends.");

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }

    #endregion

    #region Mono

    private void OnGUI()
    {
        userManual = GUILayout.TextArea(userManual, "Label");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Specific path:");
        EditorGUILayout.Space();
        AddedPath = EditorGUILayout.TextField(AddedPath);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("PlaceType:");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        m_whereToFind = (WhereToFind)EditorGUILayout.EnumPopup(m_whereToFind);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Specific Component:");

        EditorGUILayout.Space();

        m_specificComponent = EditorGUILayout.ObjectField(m_specificComponent, typeof(Object), true) as Object;

        //try
        //{
        //    Component aa = m_specificComponent as Component;
        //    Debug.Log(aa.tag);
        //    string temp = aa.tag;
        //}
        //catch (Exception)
        //{
        //    Debug.Log("aaaaaaa");
        //    throw;
        //}
        EditorGUILayout.Space();

        if (GUILayout.Button("FindUsage"))
        {
            FindUsage();
        }
    }

    #endregion
}