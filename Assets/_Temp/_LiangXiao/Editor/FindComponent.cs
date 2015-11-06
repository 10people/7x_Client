using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindComponents
{
    public static List<string> FindAllPrefabs(string PrefabExt, List<string> anyPathList, string specificPath)
    {
        var result = new List<string>();
        var paths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in paths.Where(path => path.EndsWith(PrefabExt)))
        {
            var contained = anyPathList.Any(item => path.Contains(item));
            if (!contained)
            {
                continue;
            }
            if (!string.IsNullOrEmpty(specificPath))
            {
                if (!path.Contains(specificPath))
                {
                    continue;
                }
            }
            result.Add(path);
        }
        Debug.Log("All prefabs count: " + result.Count);

        return result;
    }

    public static List<GameObject> LoadAllPrefabs(List<string> prefabList, IDictionary<GameObject, bool> activeStatus)
    {
        List<GameObject> returns = new List<GameObject>();
        foreach (var path in prefabList)
        {
            var prefabObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (prefabObject == null)
            {
                continue;
            }
            activeStatus[prefabObject] = prefabObject.activeSelf;

            returns.Add(prefabObject);
        }

        return returns;
    }

    public static void ActiveAllPrefabs(IEnumerable<KeyValuePair<GameObject, bool>> activeStatus)
    {
        foreach (var pair in activeStatus.Where(pair => !pair.Value))
        {
            pair.Key.SetActive(true);
        }
    }

    public static List<T> FindAllComponents<T>(List<GameObject> sources) where T : Component
    {
        List<T> tempReturn = sources.Select(item => item.GetComponentsInChildren<T>(true)).SelectMany(item => item).ToList();
        return tempReturn;
    }

    public static List<Component> FindAllComponents(List<GameObject> sources, Type m_type)
    {
        List<Component> tempReturn = sources.Select(item => item.GetComponentsInChildren(m_type, true)).SelectMany(item => item).ToList();
        return tempReturn;
    }

    public static void DisplayComponents<T>(IEnumerable<T> componentList) where T : Component
    {
        foreach (var component in componentList)
        {
            var path = AssetDatabase.GetAssetPath(component);

            Transform tempTransform = component.transform;
            string namePath = tempTransform.name;
            while (tempTransform.parent != null)
            {
                tempTransform = tempTransform.parent;
                namePath = tempTransform.name + "/" + namePath;
            }

            if (component.GetType() == typeof(UILabel))
            {
                Debug.Log("Find label: " + namePath + ", path: " + path + ", text: " + (component as UILabel).text);
            }
            else
            {
                Debug.Log("Find component: " + namePath + ", path: " + path);
            }
        }
    }

    public static void RestoreActiveStatus(Dictionary<GameObject, bool> activeStatus)
    {
        foreach (var pair in activeStatus)
        {
            pair.Key.SetActive(pair.Value);
        }
    }
}
