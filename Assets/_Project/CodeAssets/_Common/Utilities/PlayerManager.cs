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
    public GameObject CarriagePrefab;
    public float BasicYPosition;

    /// <summary>
    /// Key for id, value for OtherPlayerController
    /// </summary>
    public Dictionary<int, PlayerController> m_PlayerDic = new Dictionary<int, PlayerController>();

    public virtual void AddTrackCamera(PlayerController temp)
    {
        Debug.LogError("Call AddTrackCamera in base class.");
    }

    public bool CreatePlayer(int l_roleID, int l_uID, Vector3 l_position)
    {
        if (m_PlayerDic.ContainsKey(l_uID))
        {
            return false;
        }

        if (!m_PlayerDic.ContainsKey(l_uID))
        {
            var temp = Instantiate(l_roleID >= 50000 ? CarriagePrefab : (l_roleID <= 2 ? MaleCharacterPrefab : FemaleCharacterPrefab)) as GameObject;

            temp.GetComponent<CharacterController>().enabled = false;

            temp.transform.localPosition = l_position;
            temp.transform.name = "CreatedOtherPlayer_" + l_uID;

            PlayerController tempItem = temp.AddComponent<PlayerController>();
            AddTrackCamera(tempItem);

            tempItem.m_RoleID = l_roleID;
            tempItem.m_UID = l_uID;

            m_PlayerDic.Add(l_uID, tempItem);
        }

        return true;
    }

    public void UpdatePlayerPosition(int l_uID, Vector3 l_position)
    {
        if (!m_PlayerDic.ContainsKey(l_uID))
        {
            return;
        }

        PlayerController tempPlayer = m_PlayerDic[l_uID];
        l_position.y = 0;

        Vector3 targetPosition = new Vector3(l_position.x, l_position.y, l_position.z);

        tempPlayer.PlayerRun(targetPosition);
    }

    private void UpdatePlayerPosition()
    {
        foreach (PlayerController tempPlayer in m_PlayerDic.Values)
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

    public void DestroyPlayer(int l_uID)
    {
        if (m_PlayerDic.ContainsKey(l_uID))
        {
            Destroy(m_PlayerDic[l_uID].gameObject);
            m_PlayerDic.Remove(l_uID);
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
