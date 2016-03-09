using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

/// <summary>
/// Manage multiple house players.
/// </summary>
public class HousePlayerManager : Singleton<HousePlayerManager>
{
    /// <summary>
    /// Key for uid, value for PlayerInCity
    /// </summary>
    public static Dictionary<int, PlayerInCity> m_playrDic = new Dictionary<int, PlayerInCity>();

    public void CreatePlayer(EnterScene p_enter_scene_player)
    {
        if (m_playrDic.ContainsKey(p_enter_scene_player.uid))
        {
            return;
        }

        storedCreateObject = p_enter_scene_player;
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + p_enter_scene_player.roleId), ResourceLoadCallback);
    }

    private EnterScene storedCreateObject;

    private void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (!m_playrDic.ContainsKey(storedCreateObject.uid))
        {
            GameObject t_gb = Instantiate(p_object) as GameObject;

            t_gb.transform.position = new Vector3(storedCreateObject.posX, storedCreateObject.posY, storedCreateObject.posZ);
            t_gb.transform.name = "CreatedHousePlayer_" + storedCreateObject.uid;

            PlayerInCity tempItem = t_gb.AddComponent<PlayerInCity>();

            tempItem.m_playerID = storedCreateObject.uid;

            m_playrDic.Add(storedCreateObject.uid, tempItem);
        }
    }

    public void UpdatePlayerPosition(SpriteMove tempMove)
    {
        if (!m_playrDic.ContainsKey(tempMove.uid))
        {
            return;
        }

        PlayerInCity tempPlayer = m_playrDic[tempMove.uid];

        tempMove.posY = 0;

        Vector3 targetPosition = new Vector3(tempMove.posX, tempMove.posY, tempMove.posZ);

        tempPlayer.PlayerRun(targetPosition);
    }

    private void UpdatePlayerPosition()
    {
        //foreach (PlayerInCity tempPlayer in m_playrDic.Values)
        //{
        //    if (tempPlayer != null)
        //    {
        //        if (tempPlayer.m_Agent.remainingDistance <0.1f)
        //        {
        //            tempPlayer.PlayerStop();
        //        }
        //    }
        //}
    }

    public void DestroyPlayer(ExitScene tempPlayer)
    {
        if (m_playrDic.ContainsKey(tempPlayer.uid))
        {
            Destroy(m_playrDic[tempPlayer.uid].gameObject);
        }

        m_playrDic.Remove(tempPlayer.uid);
    }

    void OnDestroy(){
        m_playrDic.Clear();

		base.OnDestroy();
	}

    void LateUpdate()
    {
        UpdatePlayerPosition();
    }
}

