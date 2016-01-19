using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;

public class TaskRewardsShow : MonoBehaviour
{   
    public List<EventIndexHandle> m_listEvent;
    private int taskId = 0;
    public UIGrid m_grid;
    public UILabel m_labelTitle;

    public TaskLayerManager m_TaskLA;
    private List<FunctionWindowsCreateManagerment.RewardInfo> listRewardInfo = new List<FunctionWindowsCreateManagerment.RewardInfo>();
  private int _TaskTYpe = 0;
    void Start()
    {
        m_listEvent.ForEach(item => item.m_Handle += GetAwards);
    }
    void Update()
    {
        if (TaskData.Instance.m_TaskGetAwardComplete)
        {
            TaskData.Instance.m_TaskGetAwardComplete = false;
            gameObject.SetActive(false);
        }
    }
	void OnEnable()
	{
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100000;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 3;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

            }
            else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100010;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
               
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100020;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 10;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }
	}
    int row = 5;
    private Vector3 startPos;

    int index_Num = 0;
    int index_Num2 = 0;
    int size = 0;
    float pos_x = 0;
    GameObject objEffect = null;

    public void Show(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
        index_Num = 0;
        index_Num2 = 0;
        taskId = taskInfo._TaskId;
        _TaskTYpe = taskInfo._Type;
        objEffect = obj;
        m_labelTitle.text = taskInfo._Name;
        listRewardInfo = taskInfo._listReward;
        int sizeAll = listRewardInfo.Count;
        m_grid.transform.localPosition = new Vector3(ParentPosOffset(sizeAll, 84), 0, 0);
        for (int i = 0; i < sizeAll; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }
    private int ParentPosOffset(int count, int distance)
    {
        if (count % 2 == 0)
        {
            if (count / 2 > 1)
            {
                return -1 * distance / 2 * (count / 2 + 1);
            }
            else
            {
                return -1 * distance/2 * count / 2;
            }
        }
        else
        {
            return -1 * distance * (count / 2);
        }
    }
   
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_grid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_grid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(listRewardInfo[index_Num].icon, listRewardInfo[index_Num].count.ToString(), 3);
            iconSampleManager.SetIconPopText(listRewardInfo[index_Num].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listRewardInfo[index_Num].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listRewardInfo[index_Num].icon).descId));
            iconSampleObject.transform.localScale = Vector3.one * 0.65f;
            if (index_Num < listRewardInfo.Count - 1)
            {
                index_Num++;
            }
            m_grid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void GetAwards(int index)
    {
         
        if (index == 0)
        {
            for (int i = 0; i < m_grid.transform.childCount; i++)
            {
                Destroy(m_grid.transform.GetChild(i).gameObject);
            }
            //if (UIYindao.m_UIYindao.m_isOpenYindao)
            //{
            //    if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_iCurMissionIndex == 100000 && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
            //    {
            //        TaskData.Instance.m_iCurMissionIndex = 100000;
            //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //        tempTaskData.m_iCurIndex = 2;
            //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

            //    }
            //    else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_iCurMissionIndex == 100010 && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
            //    {
            //        TaskData.Instance.m_iCurMissionIndex = 100010;
            //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //        tempTaskData.m_iCurIndex = 3;
            //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //    }
            //    else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_iCurMissionIndex == 100020 && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
            //    {
            //        TaskData.Instance.m_iCurMissionIndex = 100020;
            //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //        tempTaskData.m_iCurIndex = 9;
            //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //    }
            //}
            this.gameObject.SetActive(false);
        }
        else
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            if (_TaskTYpe == 1)
            {
            
                GetTaskReward tempRequest = new GetTaskReward();
                tempRequest.taskId = taskId;
                t_qx.Serialize(t_tream, tempRequest);

                byte[] t_protof;
                t_protof = t_tream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GetTaskReward, ref t_protof);
            }
            else
             {
                GetTaskReward tempRequest = new GetTaskReward();
                tempRequest.taskId = taskId;
                t_qx.Serialize(t_tream, tempRequest);
                byte[] t_protof;
                t_protof = t_tream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DAILY_TASK_GET_REWARD_REQ, ref t_protof);
            }
           
        }
    }
    void OnDisable()
    {
        m_TaskLA.FreshVitality();
        int size_All = m_grid.transform.childCount;
        for (int i = 0; i < size_All; i++)
        {
            Destroy(m_grid.transform.GetChild(i).gameObject);
        }
    }

}
