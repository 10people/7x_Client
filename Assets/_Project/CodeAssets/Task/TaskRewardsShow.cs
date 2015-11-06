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
    public List<EventHandler> ehandleList;
    public GameObject m_ObjLayer;
    public GameObject m_ObjectGride;
    private int taskId = 0;
    public GameObject m_grid;

    public GameObject objCancel;
    public GameObject objOffset;
	private GameObject touch;

    public GameObject m_MainParent;
	private bool isgetData = false;

    public UILabel m_labelTitle;

    
    struct RewardInfo
    {
        public string type;
        public string count;
        public string icon;
    }
   // private RewardInfo rewardShowInfo;
    private List<RewardInfo> listRewardInfo = new List<RewardInfo>();
    private List<RewardInfo> listRewardInfo2 = new List<RewardInfo>();
    void Start()
    {
        ehandleList.ForEach(item => item.m_handler += GetAwards);

     
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
		isgetData = false;
        if (touch != null)
        touch.GetComponent<Collider>().enabled = true;
 
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
    
    public void Show(string reward, string title,int Id,GameObject obj)
    {
        index_Num = 0;
        index_Num2 = 0;
        taskId = Id;
        objEffect = obj;
        m_labelTitle.text = title;
        listRewardInfo.Clear();
        if (!string.IsNullOrEmpty(reward) && reward != "0")
        {
            if (reward.IndexOf('#') > -1)
            {
                string[] tempAwardList = reward.Split('#');
                size = tempAwardList.Length;
          
                for (int i = 0; i < tempAwardList.Length; i++)
                {
                    string[] tempAwardItemInfo = tempAwardList[i].Split(':');
                    RewardInfo rinfo = new RewardInfo();
                    rinfo.type = tempAwardItemInfo[0];
                    rinfo.icon = tempAwardItemInfo[1];
                    rinfo.count = tempAwardItemInfo[2];
                    listRewardInfo.Add(rinfo);
                }
            }
            else
            {
                string[] tempAwardItemInfo = reward.Split(':');
                RewardInfo rinfo = new RewardInfo();
                rinfo.type = tempAwardItemInfo[0];
                rinfo.icon = tempAwardItemInfo[1];
                rinfo.count = tempAwardItemInfo[2];
                listRewardInfo.Add(rinfo);
            }

          
            int sizeAll = listRewardInfo.Count;
            m_grid.transform.localPosition = new Vector3(ParentPosOffset(sizeAll,112), 0,0);
            for (int i = 0; i < sizeAll; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
             //   Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_AWARD_ITEM_BIG), ResourcesLoadCallBack);
            }
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
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path,Object p_object)
    {
        GameObject rewardShow = Instantiate(p_object) as GameObject;
        rewardShow.transform.parent = m_grid.transform;
      
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.GetComponent<TaskAwardItemAmend>().Show(listRewardInfo[index_Num].icon, listRewardInfo[index_Num].count, int.Parse(listRewardInfo[index_Num].type));
       // Debug.Log("listRewardInfo2[index_Num2].iconlistRewardInfo2[index_Num2].icon ::" + listRewardInfo2[index_Num2].icon);
        if (int.Parse(listRewardInfo[index_Num].icon) == 900006)
        {
			UI3DEffectTool.Instance().ShowTopLayerEffect( UI3DEffectTool.UIType.FunctionUI_1, rewardShow, EffectIdTemplate.GetPathByeffectId(100112), null);
        }
        if (index_Num < listRewardInfo.Count - 1)
        {
            index_Num++;
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

            iconSampleManager.SetIconByID(int.Parse(listRewardInfo[index_Num].icon), listRewardInfo[index_Num].count);

			iconSampleManager.SetIconPopText(int.Parse(listRewardInfo[index_Num].icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).descId));

            if (int.Parse(listRewardInfo[index_Num].icon) == 900006)
            {
                UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, iconSampleObject, EffectIdTemplate.GetPathByeffectId(100112), null);
            }

            if (index_Num < listRewardInfo.Count - 1)
            {
                index_Num++;
            }
            m_grid.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void CreateAppendReward()
    {
        string[] AwardItemInfo = RenWuTemplate.GetAllianceRewardById(taskId).Split('#');
        foreach (string tempAward in AwardItemInfo)
        {

            string[] tempAwardItemInfo = tempAward.Split(':');
            RewardInfo rinfo = new RewardInfo();
            rinfo.type = tempAwardItemInfo[0];
            rinfo.icon = tempAwardItemInfo[1];
            rinfo.count = tempAwardItemInfo[2];
            listRewardInfo2.Add(rinfo);
           
        }
        int sizeappend = listRewardInfo2.Count;
        for (int i = 0; i < sizeappend; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_AWARD_ITEM_BIG), ResourcesLoadCallBack2);
        }
    }

    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        startPos.x = pos_x + size * 130 + index_Num2 * 130; ;
        GameObject rewardShow = Instantiate(p_object) as GameObject;
    
        rewardShow.transform.parent = m_grid.transform;
       // Debug.Log(" startPos.x  startPos.x " + startPos.x);
        rewardShow.transform.localPosition = startPos;
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.GetComponent<TaskAwardItemAmend>().Show(listRewardInfo2[index_Num2].icon, listRewardInfo2[index_Num2].count, int.Parse(listRewardInfo2[index_Num2].type));
       
        if (int.Parse(listRewardInfo2[index_Num2].icon) == 900006)
        {
			UI3DEffectTool.Instance().ShowTopLayerEffect( UI3DEffectTool.UIType.FunctionUI_1, rewardShow, EffectIdTemplate.GetPathByeffectId(100112), null);
        }
        if (index_Num2 < listRewardInfo2.Count - 1)
        {
            index_Num2++;
        }
    }
    void GetAwards(GameObject obj)
    {
        if (obj.name == "Close")
        {
            m_ObjectGride.GetComponent<TaskGridManager>().m_SaveId = 0;
            for (int i = 0; i < m_grid.transform.childCount; i++)
            {
                Destroy(m_grid.transform.GetChild(i).gameObject);
            }
            if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_iCurMissionIndex == 100000 && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = 100000;
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 2;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

                }
                else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_iCurMissionIndex == 100010 && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = 100010;
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 3;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                }
                else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_iCurMissionIndex == 100020 && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = 100020;
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 9;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                }
            }
            this.gameObject.SetActive(false);
        }
        else
        {
            switch ((TaskType)CityGlobalData.m_TaskType)
            {
                case TaskType.MainQuest:
                    {
                        touch = obj;
                        //obj.collider.collider.enabled = false;
                        MemoryStream t_tream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        GetTaskReward tempRequest = new GetTaskReward();
                        tempRequest.taskId = taskId;
                        t_qx.Serialize(t_tream, tempRequest);

                        byte[] t_protof;
                        t_protof = t_tream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GetTaskReward, ref t_protof);
                    }
                    break;
                case TaskType.DailyQuest:
                    {
                        touch = obj;
                        //obj.collider.collider.enabled = false;
                        MemoryStream t_tream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        GetTaskReward tempRequest = new GetTaskReward();
                        tempRequest.taskId = taskId;
                        t_qx.Serialize(t_tream, tempRequest);

                        byte[] t_protof;
                        t_protof = t_tream.ToArray();

                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DAILY_TASK_GET_REWARD_REQ, ref t_protof);
                    }
                    break;
                case TaskType.AchievementQuest:
                    {

                    }
                    break;
                case TaskType.BiographicalQuest:
                    {

                    }
                    break;
            }

        }
     //ss   Destroy(m_MainParent);
    }

    int index_Index = 0;
    //public bool OnProcessSocketMessage(QXBuffer p_message)
    //{
    //    m_ObjectGride.GetComponent<TaskGridManager>().m_SaveId = 0;
    //    if (p_message != null)
    //    {
    //        switch (p_message.m_protocol_index)
    //        {
    //            case ProtoIndexes.S_GetTaskRwardResult: //主线任务领取
    //                {
    //                    MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

    //                    QiXiongSerializer t_qx = new QiXiongSerializer();

    //                    GetTaskRwardResult TaskSyncReponse = new GetTaskRwardResult();

    //                    t_qx.Deserialize(t_tream, TaskSyncReponse, TaskSyncReponse.GetType());
    //                    UI3DEffectTool.Instance().ClearUIFx(objEffect);
    //                    //if (UIYindao.m_UIYindao.m_isOpenYindao && !isgetData)
    //                    //{
    //                    //    isgetData = true;
    //                    //    if (FreshGuide.Instance().IsActive(TaskData.Instance.m_iCurMissionIndex) && TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex].yindaoId.Equals("0") && TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex].progress < 0)
    //                    //    {
    //                    //        UIYindao.m_UIYindao.CloseUI();
    //                    //    }
    //                    //    else if (FreshGuide.Instance().IsActive(200002) && TaskData.Instance.ShowId == 200002 && TaskData.Instance.m_TaskInfoDic[200002].progress < 0)
    //                    //    {
    //                    //        UIYindao.m_UIYindao.CloseUI();
    //                    //    }
    //                    //    else if (FreshGuide.Instance().IsActive(TaskData.Instance.m_iCurMissionIndex))
    //                    //    {
    //                    //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
    //                    //        if (TaskData.Instance.m_iCurMissionIndex == 100012)
    //                    //        {
    //                    //          //  Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
    //                    //            tempTaskData.m_iCurIndex = 10;
    //                    //        }
    //                    //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
    //                    //    }
    //                    //    else if (TaskData.Instance.m_iCurMissionIndex == 100010)
    //                    //    {
    //                    //        TaskData.Instance.m_iCurMissionIndex = 100012;
    //                    //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
    //                    //        tempTaskData.m_iCurIndex = 9;
    //                    //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
    //                    //    }
    //                    //    else if (FreshGuide.Instance().IsActive(100012) && TaskData.Instance.ShowId == 100012 && TaskData.Instance.m_TaskInfoDic[100012].progress < 0)
    //                    //    {
    //                    //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100012];
    //                    //        tempTaskData.m_iCurIndex = 10;
    //                    //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
    //                    //    }

    //                    //}

    //                    if (UIYindao.m_UIYindao.m_isOpenYindao)
    //                    {
    //                        if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
    //                        {
    //                            TaskData.Instance.m_iCurMissionIndex = 100000;
    //                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
    //                            //tempTaskData.m_iCurIndex = 4;
    //                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
    //                            //   UI3DEffectTool.Instance().ShowMidLayerEffect(objCancel, EffectIdTemplate.GetPathByeffectId(100000), objOffset);
    //                        }
    //                        else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
    //                        {
    //                            TaskData.Instance.m_iCurMissionIndex = 100010;
    //                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
    //                            //  tempTaskData.m_iCurIndex = 6;
    //                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
    //                        }
    //                        else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
    //                        {
    //                            TaskData.Instance.m_iCurMissionIndex = 100020;
    //                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
    //                            // tempTaskData.m_iCurIndex = 4;
    //                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
    //                        }
                            
    //                    }
    //                    //if (TaskData.Instance.m_iCurMissionIndex == 100015 && GameObject.Find("MainQuestInfo") != null)
    //                    //{
    //                    //    GameObject.Find("MainQuestInfo").transform.GetComponent<UILabel>().text = "";
    //                    //    //  TaskTitleLabEffertManagerment.m_Labeffert.CloneMove(GameObject.Find("MainQuestInfo").gameObject, "");
    //                    //}

    //                   /* if (!TaskSyncReponse.msg.Equals("fail"))
    //                    {
    //                        TaskData.Instance.m_TaskInfoDic.Remove(TaskSyncReponse.taskId);
    //                        foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
    //                        {
    //                            if (item.Value.type == 0)
    //                            {
    //                                TaskData.Instance.ShowId = item.Value.id;

    //                                TaskData.Instance.showTitleOn = true;
    //                            }

    //                            break;
    //                            //if(item.Value.yindaoId.Equals("7") || item.Value.yindaoId.Equals("1") && item.Value.progress >= 0 )
    //                            //{
    //                            // CityGlobalData.m_JunZhuTouXiangGuide = true;
    //                            //}
    //                        }



    //                        TaskData.Instance.isReload = true;

    //                        //this.gameObject.SetActive(false);

    //                    }

    //                    if (!string.IsNullOrEmpty(CityGlobalData.TaskTitleInfo))
    //                    {
    //                        CityGlobalData.TaskUpdata = true;
    //                        if (TaskData.Instance.m_TaskInfoDic.Count > 0)
    //                        {
    //                            foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
    //                            {
    //                                if (item.Value.progress >= 0)
    //                                {
    //                                    CityGlobalData.TaskTitleInfo += ";" + item.Value.title;
    //                                }
    //                                break;
    //                            }
    //                        }
    //                        else
    //                        {
    //                            CityGlobalData.TaskTitleInfo += ";" + " ";
    //                            TaskData.Instance.ShowCompleteTag(true);
    //                        }
    //                    }
    //                    */
    //                    CityGlobalData.TaskLingQu = true;
    //                  //  this.gameObject.SetActive(false);
    //                    TaskData.Instance.m_TagIsShow = true;
    //                    switch (TaskSyncReponse.msg)
    //                    { 
    //                        case "success":
    //                         {
                               
    //                             {
    //                                 FunctionOpenTemp.GetMissionDoneOpenFunction(TaskSyncReponse.taskId);
    //                                 TaskData.Instance.m_TaskInfoDic.Remove(TaskSyncReponse.taskId);
    //                                 foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
    //                                 {
    //                                     if (item.Value.type == 0)
    //                                     {
    //                                         TaskData.Instance.ShowId = item.Value.id;

    //                                         TaskData.Instance.showTitleOn = true;
    //                                     }

    //                                     break;
           
    //                                 }

    //                                 TaskData.Instance.isReload = true;

    //                             }

    //                             if (!string.IsNullOrEmpty(CityGlobalData.TaskTitleInfo))
    //                             {
    //                                 CityGlobalData.TaskUpdata = true;
    //                                 if (TaskData.Instance.m_TaskInfoDic.Count > 0)
    //                                 {
    //                                     foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
    //                                     {
    //                                         if (item.Value.progress >= 0)
    //                                         {
    //                                             CityGlobalData.TaskTitleInfo += ";" + item.Value.title;
    //                                         }
    //                                         break;
    //                                     }
    //                                 }
    //                                 else
    //                                 {
    //                                     CityGlobalData.TaskTitleInfo += ";" + " ";
    //                                     TaskData.Instance.SetTaskPopupType(true);
    //                                 }
    //                             }
    //                             this.gameObject.SetActive(false);
    //                             index_Index = 0;
                             
    //                         }
    //                          break;
    //                        case "fail":
    //                          {
    //                              index_Index = 1;
    //                              Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //                                 UIBoxLoadCallbackZero);
    //                          }
    //                          break;
    //                        case "hasGet":
    //                          {
    //                              this.gameObject.SetActive(false);
    //                              index_Index = 2;
    //                              Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //                                 UIBoxLoadCallbackZero);

    //                          }
    //                          break;
    //                        default:
    //                          break;
    //                    }
    //                    return true;
    //                }

    //            case ProtoIndexes.S_DAILY_TASK_GET_REWARD_RESP://日常任务领取
    //                {
    //                    MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
    //                    QiXiongSerializer t_qx = new QiXiongSerializer();
    //                    DailyTaskRewardResponse dailyTask = new DailyTaskRewardResponse();
    //                    t_qx.Deserialize(t_tream, dailyTask, dailyTask.GetType());
 
    //                    if (dailyTask.status)
    //                    {
    //                        if (TaskData.Instance.m_TaskDailyDic.ContainsKey(dailyTask.taskId))
    //                        {
    //                            TaskData.Instance.m_TaskDailyDic.Remove(dailyTask.taskId);
    //                            TaskData.Instance.m_DailyQuestGetReward = true;
    //                        }
    //                        m_ObjLayer.GetComponent<TaskLayerManager>().ShowDailyTanhao();
    //                        TaskData.Instance.m_TagIsShow = true;
    //                        switch (dailyTask.msg)
    //                        {
    //                            case "success":
    //                                {
    //                                    this.gameObject.SetActive(false);
    //                                    index_Index = 0;
                              
                                 
    //                                 //   Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //                                   //        UIBoxLoadCallbackZero);
    //                                }
    //                                break;
    //                            case "fail":
    //                                {
    //                                    index_Index = 1;
    //                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //                                       UIBoxLoadCallbackZero);
    //                                }
    //                                break;
    //                            case "hasGet":
    //                                {
    //                                    this.gameObject.SetActive(false);
    //                                    index_Index = 2;
    //                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
    //                                       UIBoxLoadCallbackZero);

    //                                }
    //                                break;
    //                            default:
    //                                this.gameObject.SetActive(false);
    //                                break;
    //                        }
    //                    }

    //                    return true;
    //                }
    //        }
    //    }
    //    return false;
    //}
    public void UIBoxLoadCallbackZero(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        boxObj.transform.localPosition = new Vector3(5000, 5000, 0);
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
          string str1 = "";
          if (index_Index == 0)
          {
              str1 = LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_GET_AWARD_SUCCESS_TITLE) + "!";
          }
          else if (index_Index == 1)
          {
              str1 = LanguageTemplate.GetText(LanguageTemplate.Text.TASK_GET_FAIL) + "!";
          }
          else
          {
              str1 = LanguageTemplate.GetText(LanguageTemplate.Text.TASK_HAS_GET) + "!";
          }
     
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, null, null);
    }
    void BuyJinBi(int i)
    { 
    
    }
 
    void OnDisable()
    {
        m_ObjectGride.GetComponent<TaskGridManager>().m_isTouched = false;
        int size_All = m_grid.transform.childCount;
        for (int i = 0; i < size_All; i++)
        {
            Destroy(m_grid.transform.GetChild(i).gameObject);
        }
      //  SocketTool.UnRegisterMessageProcessor(this);
    }

}
