using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageFieldPoolManager : Singleton<MessageFieldPoolManager>
{
    public List<GameObject> MessageFieldsList = new List<GameObject>();

    private List<GameObject> parentObjects = new List<GameObject>();

    private List<MyPoolManager> messageFieldsPoolList = new List<MyPoolManager>();

    private bool isInitialized;

    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;

        if (messageFieldsPoolList == null || messageFieldsPoolList.Count == 0)
        {
            foreach (var effectPrefab in MessageFieldsList)
            {
                GenerateMessageFieldPoolManager(effectPrefab);
            }
        }
    }

    public void Reset()
    {
        if (!isInitialized)
        {
            return;
        }

        isInitialized = false;
    }

    public void Cleanup()
    {
        if (!isInitialized)
        {
            return;
        }

        Reset();

        messageFieldsPoolList.ForEach(pool => pool.Cleanup());
        messageFieldsPoolList.ForEach(pool => pool.SpawnObject = null);
        messageFieldsPoolList.Clear();

        parentObjects.ForEach(item => Destroy(item));
        parentObjects.Clear();
    }

    /// <summary>
    /// Take pool item from pool index.
    /// </summary>
    /// <param name="index">The index of pool</param>
    public GameObject TakeMessageField(int index)
    {
        if (index < 0 || index >= messageFieldsPoolList.Count)
        {
            Debug.LogError("m_Index is out of range [0, " + (messageFieldsPoolList.Count - 1) + "]" + ", but index is: " + index);
            return null;
        }
        var pool = messageFieldsPoolList[index];
        if (pool.SpawnObject == null)
        {
            // resouce file are 1 based, but our index is 0 based.
            pool.SpawnObject = MessageFieldsList[index];
        }
        return pool.Take();
    }

    /// <summary>
    /// Return game object to pool of index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="go"></param>
    public void ReturnMessageField(int index, GameObject go)
    {
        if (index < 0 || index >= messageFieldsPoolList.Count)
        {
            Debug.LogError("m_Index is out of range [0, " + messageFieldsPoolList.Count + ")" + ", but index is: " + index);
            return;
        }
        var pool = messageFieldsPoolList[index];
        pool.Return(go);
    }

    private void GenerateMessageFieldPoolManager(GameObject spawnObject)
    {
        var go = new GameObject(spawnObject.name);
        var pool = go.AddComponent<MyPoolManager>();
        pool.SpawnObject = spawnObject;
        pool.Capacity = 0;
        pool.Initialize();
        go.transform.parent = transform;
        parentObjects.Add(go);

        messageFieldsPoolList.Add(pool);
    }

    void OnDestroy()
    {
        Cleanup();

		base.OnDestroy();
	}
}
