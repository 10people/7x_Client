using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AllianceBattle;
using qxmobile.protobuf;

public class PlayerManager : MonoBehaviour
{
    public GameObject MaleCharacterPrefab;
    public GameObject FemaleCharacterPrefab;
    public float BasicYPosition;

    /// <summary>
    /// Key for id, value for OtherPlayerController
    /// </summary>
    public Dictionary<long, OtherPlayerController> m_PlayerDic = new Dictionary<long, OtherPlayerController>();

    public virtual void AddTrackCamera(OtherPlayerController temp)
    {
        Debug.LogError("Call AddTrackCamera in base class.");
    }

    public void CreatePlayer(long l_ID, int l_roleID,int l_uID, Vector3 l_position)
    {
        if (m_PlayerDic.ContainsKey(l_ID))
        {
            return;
        }

        if (!m_PlayerDic.ContainsKey(l_ID))
        {
            var tempObject = Instantiate(l_roleID <= 2 ? MaleCharacterPrefab : FemaleCharacterPrefab) as GameObject;

            tempObject.transform.position = new Vector3(l_position.x, l_position.y, l_position.z);
            tempObject.transform.name = "CreatedOtherPlayer_" + l_ID;

            OtherPlayerController tempItem = tempObject.AddComponent<OtherPlayerController>();
            AddTrackCamera(tempItem);

            tempItem.m_PlayerID = l_ID;
            tempItem.m_RoleID = l_roleID;
            tempItem.m_UID = l_uID;

            m_PlayerDic.Add(l_ID, tempItem);
        }
    }

    public void UpdatePlayerPosition(long l_ID, Vector3 l_position)
    {
        if (!m_PlayerDic.ContainsKey(l_ID))
        {
            return;
        }

        OtherPlayerController tempPlayer = m_PlayerDic[l_ID];
        l_position.y = 0;

        Vector3 targetPosition = new Vector3(l_position.x, l_position.y, l_position.z);

        tempPlayer.PlayerRun(targetPosition);
    }

    private void UpdatePlayerPosition()
    {
        foreach (OtherPlayerController tempPlayer in m_PlayerDic.Values)
        {
            if (tempPlayer != null)
            {
                if (tempPlayer.m_Agent.remainingDistance < 0.1f)
                {
                    tempPlayer.PlayerStop();
                }
            }
        }
    }

    public void DestroyPlayer(long l_ID)
    {
        if (m_PlayerDic.ContainsKey(l_ID))
        {
            Destroy(m_PlayerDic[l_ID].gameObject);
            m_PlayerDic.Remove(l_ID);
        }
    }

    public void OnDestroy()
    {
        m_PlayerDic.Clear();
    }

    public void LateUpdate()
    {
        UpdatePlayerPosition();
    }
}
