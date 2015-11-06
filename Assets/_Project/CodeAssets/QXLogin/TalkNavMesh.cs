using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TalkNavMesh : MonoBehaviour
{
    public int showid;
    private static int storedInt;

    void OnClick()
    {
        UI3DEffectTool.Instance().ClearUIFx(gameObject);
        showid = TaskData.Instance.ShowId;
       // TaskData.Instance.OtherSceneShow();
        FindNpc(showid);
    }

    static void FindNpc(int showId)
    {
        if (MainCityUI.m_PlayerPlace != MainCityUI.PlayerPlace.MainCity)
        {
            CityGlobalData.m_TaskNavID = showId;
            NpcManager.m_NpcManager.IsTaskNavMesh = true;

            //DoExitHouse
            HouseRootManager.m_HouseRootManager.m_houseBasic.OnExitClick();

            return;
        }

        if (TaskData.Instance.m_TaskInfoDic.ContainsKey(showId))
        {
            if (!TaskData.Instance.WetherContainMainTask())
            {
                if (UIYindao.m_UIYindao.m_isOpenYindao)
                {
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        tempTaskData.m_iCurIndex++;
                }
            }
            else  if (TaskData.Instance.m_TaskInfoDic[showId].progress >= 0 /*&& TaskData.Instance.m_TaskInfoDic[showId].doneType == 4*/)
            {
                NpcManager.m_NpcManager.setGoToNpc(TaskData.Instance.m_TaskInfoDic[showId].NpcId);
            }
            else if (TaskData.Instance.m_TaskInfoDic[showId].progress < 0)
            {
                if (UIYindao.m_UIYindao.m_isOpenYindao)
                {
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        tempTaskData.m_iCurIndex++;
                }
            }
        }
    }

    static void DoFindNpcWithStoredId()
    {
        FindNpc(storedInt);
    }
}
