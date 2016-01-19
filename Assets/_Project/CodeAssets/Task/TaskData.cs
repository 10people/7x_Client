using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TaskData : Singleton<TaskData>, SocketProcessor
{
    public int ShowId = -1;
	public int m_iShowOtherId = -1;
    public int m_iCurMissionIndex;
    public bool showTitleOn;
    public bool m_MainReload = false;
    public bool m_SideReload = false;
    public bool m_DailyQuestIsRefresh = false;
    public bool m_VitaRefresh = false;
    public bool m_TaskGetAwardComplete = false;
    public bool m_DestroyMiBao = false;
    public bool isRefrsh = false;
    public ZhuXianTemp m_MainComplete;

    public struct VitalityInfo
    {
        public int _todaylHuoYue;
        public int _weekHuoYue;
        public List<int> _listawardStatus; //每个活跃度节点（箱子）的领奖情况（包括周活跃度）0-活跃度不够; 1-活跃度够，但没领; 2-已经领取
    }
    public int m_RemainTime = 0;
    public VitalityInfo m_VitalityShowInfo = new VitalityInfo();


    public int m_ShowType = 0;// 0:main 1:side 2:daily
    private bool _isMainComplete = false;

    public Dictionary<int, ZhuXianTemp> m_TaskInfoDic = new Dictionary<int, ZhuXianTemp>();

    public Dictionary<int, RenWuTemplate> m_TaskDailyDic = new Dictionary<int, RenWuTemplate>();

    public Dictionary<int, AcheInfo> m_acheInfoDic = new Dictionary<int, AcheInfo>();
    private Dictionary<int, int> DicTaskLX = new Dictionary<int, int>();

    public bool m_isFirst = false;
 
    public bool m_TagIsShow = false;

    private static TaskData m_instance = null;
  
    public bool m_MainTaskGetAwardComplete = false;
    public bool m_MainCityUIIsNull = false;

    public bool m_isDailyTimeDown = false;
    private bool _isNeedWait = false;

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {

    }

    void Update()
    {
        if (m_RemainTime > 0 && !_isNeedWait)
        {
            _isNeedWait = true;
            StartCoroutine(WaitTimeDown());
        }
        if (WindowBackShowController.m_isContainKey)
        {
            if (ClientMain.m_listPopUpData.Count == 0)
            {
                WindowBackShowController.m_isContainKey = false;
                WindowBackShowController.CreateSaveWindow(WindowBackShowController.m_SaveKey);
            }
        }
        if (_isMainComplete)
        {
            _isMainComplete = false;
			ClientMain.addPopUP(5, 1, "", null);
        }

        if (m_TagIsShow)
        {
            int Count_index = 0;

            foreach (KeyValuePair<int, ZhuXianTemp> item in m_TaskInfoDic)
            {
                if (item.Value.progress < 0)
                {
                    Count_index++;
                }
            }
            if (FunctionOpenTemp.GetWhetherContainID(106))
            {
                if (JunZhuData.Instance().m_CurrentLevel >= FunctionOpenTemp.GetTemplateById(106).Level)
                {
                    foreach (KeyValuePair<int, RenWuTemplate> item in m_TaskDailyDic)
                    {
                        if (item.Value.progress < 0)
                        {
                            Count_index++;
                        }
                    }
                }
            }

            if (Count_index == 0)
            {
                MainCityUI.SetRedAlert(5, false);
                if (MainCityUI.SetRedAlert(5, false))
                {
                    m_TagIsShow = false;
                }
            }
            else
            {
                MainCityUI.SetRedAlert(5, true);
                if (MainCityUI.SetRedAlert(5, true))
                {
                    m_TagIsShow = false;
                }

            }
        }
    }
    IEnumerator WaitTimeDown()
    {
        yield return new WaitForSeconds(1.0f);
        m_RemainTime--;
        _isNeedWait = false;
        if (m_RemainTime > 0)
        {
            m_isDailyTimeDown = true;
        }
        else
        {
            m_RemainTime = 24*3600;
        }

    }
    int indexNum = 0;
    public bool ShowMainTaskGet(string data)
    {
        if (JunZhuLevelUpManagerment.m_JunZhuLevelUp == null && TaskSignalInfoShow.m_TaskSignal == null
                && !CityGlobalData.m_isBattleField_V4_2D && EquipGrowthWearManagerment.m_EquipGrowth == null
                && TanBaoPage.tbPage == null && !m_DestroyMiBao)
        {
            if (PlayerModelController.m_playerModelController != null)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_EFFECT), OnTaskEffectLoadCallBack);
                return true;
            }
        }
        return false;
                
    }
    private void OnTaskEffectLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject  Object = Instantiate(p_object) as GameObject;
        Object.transform.localPosition = new Vector3(0, 10000, 0);

        Object.transform.localScale = Vector3.one;
    }
     
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_TaskList://返回任务列表
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        TaskList TaskListReponse = new TaskList();

                        t_qx.Deserialize(t_tream, TaskListReponse, TaskListReponse.GetType());

                       
                        if (TaskListReponse.list != null && TaskListReponse.list.Count > 0)
                        {
                            for (int i = 0; i < TaskListReponse.list.Count; i++)
                            {
                                if (m_TaskInfoDic.ContainsKey(TaskListReponse.list[i].id))
                                {
                                    if (m_TaskInfoDic[TaskListReponse.list[i].id].progress >= 0 && TaskListReponse.list[i].progress < 0)
                                    {
                                        if (TaskListReponse.list[i].id == 100290)
                                        {
											UIYindao.m_UIYindao.CloseUI();
                                        }

                                        //[WARNING]Added by liangxiao.
                                        if (TaskListReponse.list[i].id == 400000)
                                        {
                                            MainCityUI.IsShowFunctionOpenEffectInAllianceCity = true;
                                        }
                                        else if (TaskListReponse.list[i].id == 100060)
                                        {
											UIYindao.m_UIYindao.CloseUI();
                                        }
										else if (TaskListReponse.list[i].id == 100315)
										{
											UIYindao.m_UIYindao.CloseUI();
										}
										else if (TaskListReponse.list[i].id == 100370)
										{
											UIYindao.m_UIYindao.CloseUI();
										}
                                    }
                                }

                                m_isFirst = true;
                            }

                            m_TaskInfoDic.Clear();
                            if (TaskListReponse.list != null)
                            {
                                RefreshTaskInfo(TaskListReponse.list);
                                m_isFirst = false;
                            }
                        }
                        else
                        {
                            if (MainCityUI.m_MainCityUI != null)
                            {
//                                MainCityUI.m_MainCityUI.m_MainCityUIL.TaskDetailLabel.text = "";
                            }
                        }
                        m_TagIsShow = true;
                        return true;
                    }
                case ProtoIndexes.S_TaskSync://服务器向客户端发送任务进度
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        TaskSync TaskSyncReponse = new TaskSync();

                        t_qx.Deserialize(t_tream, TaskSyncReponse, TaskSyncReponse.GetType());

                        if (m_TaskInfoDic.ContainsKey(TaskSyncReponse.task.id))
                        {
                            if (ShowId == TaskSyncReponse.task.id)
                            {
                                showTitleOn = true;
                            }

                            if (m_TaskInfoDic[TaskSyncReponse.task.id].progress >= 0 && TaskSyncReponse.task.progress < 0)
                            {
                                Debug.Log(TaskSyncReponse.task.id);

                            }
                            m_TaskInfoDic[TaskSyncReponse.task.id].progress = TaskSyncReponse.task.progress;
                            if (ZhuXianTemp.getTemplateById(TaskSyncReponse.task.id).type == 0)
                            {
                                m_MainReload = true;
                            }
                            else
                            {
                                m_SideReload = true;
                            }
                          
                        }
                        else
                        {
                            List<TaskInfo> list = new List<TaskInfo>();
                            list.Add(TaskSyncReponse.task);
                            RefreshTaskInfo(list);

                            if (ZhuXianTemp.getTemplateById(TaskSyncReponse.task.id).type == 0)
                            {
                                m_MainReload = true;
                            }
                            else
                            {
                                m_SideReload = true;
                            }
                        }

                        m_TagIsShow = true;
                        return true;
                    }
                case ProtoIndexes.S_DAILY_TASK_LIST_RESP://返回日常任务列表
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        DailyTaskListResponse dailyTaskList = new DailyTaskListResponse();
                
                        t_qx.Deserialize(t_tream, dailyTaskList, dailyTaskList.GetType());
                        VitalityInfo vi = new VitalityInfo();

                        vi._todaylHuoYue = dailyTaskList.todaylHuoYue;
                        vi._weekHuoYue = dailyTaskList.weekHuoYue;
                        vi._listawardStatus = dailyTaskList.awardStatus;
                        m_RemainTime = dailyTaskList.remainTime/1000;
                        m_VitalityShowInfo = vi;
                        if (dailyTaskList.taskInfo != null)
                        {
                            m_TaskDailyDic.Clear();
                            listComplete.Clear();
                            listUnComplete.Clear();
                            RefreshDailyTaskInfo(dailyTaskList);
                            m_TagIsShow = true;
                            m_DailyQuestIsRefresh = true;
                        }        
                        return true;
                    }

                case ProtoIndexes.S_DAILY_TASK_FINISH_INFORM://返回日常任务更新
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        DailyTaskListResponse dailyTaskInfo = new DailyTaskListResponse();
                        t_qx.Deserialize(t_tream, dailyTaskInfo, dailyTaskInfo.GetType());
                        if (dailyTaskInfo.taskInfo != null)
                        {
                            for (int i = 0; i < dailyTaskInfo.taskInfo.Count; i++)
                            {
                                if (m_TaskDailyDic.ContainsKey(dailyTaskInfo.taskInfo[i].taskId))
                                {
                                    if (dailyTaskInfo.taskInfo[i].isFinish)
                                    {
                                        m_TaskDailyDic[dailyTaskInfo.taskInfo[i].taskId].progress = -1;
                                    }
                                    else
                                    {
                                        m_TaskDailyDic[dailyTaskInfo.taskInfo[i].taskId].progress = dailyTaskInfo.taskInfo[i].jindu;
                                    }
                                }
                            }
                            TaskTidy();
                            m_TagIsShow = true;
                        }
                        return true;
                    }
                case ProtoIndexes.S_GetTaskRwardResult: //主线任务领取

//				Debug.Log(ProtoIndexes.S_GetTaskRwardResult);

                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetTaskRwardResult TaskSyncReponse = new GetTaskRwardResult();

                        t_qx.Deserialize(t_tream, TaskSyncReponse, TaskSyncReponse.GetType());
                        CityGlobalData.TaskLingQu = true;
                        TaskData.Instance.m_TagIsShow = true;
                        switch (TaskSyncReponse.msg)
                        {
                            case "success":
                                {
                                    FunctionOpenTemp.GetMissionDoneOpenFunction(TaskSyncReponse.taskId);
                                  
                                    string award = "";
  
//                                    foreach (KeyValuePair<int, ZhuXianTemp> item in m_TaskInfoDic)
//                                    {
//                                        Debug.Log(item.Value.id);
//
//                                    }
                                    
                                    m_TaskInfoDic.Remove(TaskSyncReponse.taskId);
                                    foreach (KeyValuePair<int, ZhuXianTemp> item in m_TaskInfoDic)
                                    {
                                        if (item.Value.type == 0)
                                        {
                                            ShowId = item.Value.id;
                                        }

                                    }

                                    award = ZhuXianTemp.getTemplateById(TaskSyncReponse.taskId).award;
                                    if (ZhuXianTemp.getTemplateById(TaskSyncReponse.taskId).type == 0)
                                    {
                                        m_MainReload = true;
                                    }
                                    else
                                    {
                                        m_SideReload = true;
                                    }
                                    m_TaskGetAwardComplete = true;
                                    if (!string.IsNullOrEmpty(award))
                                    {
                                        FunctionWindowsCreateManagerment.ShowRAwardInfo(award);
                                    }
                                }
                                break;
                            case "fail":
                                {
                                    //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    //   UIBoxLoadCallbackZero);
                                }
                                break;
                            case "hasGet":
                                {
                                    //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    //   UIBoxLoadCallbackZero);

                                }
                                break;
                            default:
                                break;
                        }
                        return true;
                    }

                case ProtoIndexes.S_DAILY_TASK_GET_REWARD_RESP://日常任务领取
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        DailyTaskRewardResponse dailyTask = new DailyTaskRewardResponse();
                        t_qx.Deserialize(t_tream, dailyTask, dailyTask.GetType());

                        if (dailyTask.status)
                        {
                            m_VitalityShowInfo._todaylHuoYue = dailyTask.todaylHuoYue;
                            m_VitalityShowInfo._weekHuoYue = dailyTask.weekHuoYue;
                            string award = "";
                            if (TaskData.Instance.m_TaskDailyDic.ContainsKey(dailyTask.taskId))
                            {
                                award = m_TaskDailyDic[dailyTask.taskId].jiangli;
                                m_TaskDailyDic.Remove(dailyTask.taskId);
                                m_TaskGetAwardComplete = true;
                                m_DailyQuestIsRefresh = true;
                                if (!string.IsNullOrEmpty(award))
                                {
                                    FunctionWindowsCreateManagerment.ShowRAwardInfo(award);
                                }
                                MainCityUI.SetRedAlert(106, DailyWetherContainComplete(m_TaskDailyDic));
                            }
                       
                            switch (dailyTask.msg)
                            {
                                case "success":
                                    {
                                    
                                        //   Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        //        UIBoxLoadCallbackZero);
                                    }
                                    break;
                                case "fail":
                                    {
                                        //index_Index = 1;
                                        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        //   UIBoxLoadCallbackZero);
                                    }
                                    break;
                                case "hasGet":
                                    {
                                        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        //   UIBoxLoadCallbackZero);
                                    }
                                    break;
                            }
                        }
                        m_TagIsShow = true;
                        return true;
                    }
                 case ProtoIndexes.dailyTask_get_huoYue_award_resp:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ErrorMessage vitality = new ErrorMessage();
                        t_qx.Deserialize(t_tream, vitality, vitality.GetType());
                        if (vitality.errorCode == 0)
                        {
                            FunctionWindowsCreateManagerment.ShowRAwardInfo(HuoYueTempTemplate.GetHuoYueTempById(int.Parse(vitality.errorDesc) + 1).award);
                            m_VitaRefresh = true;
                        }
                        return true;
                    }
            }
        }
        return false;
    }


    void TaskTidy()
    {
        listDailyComplete.Clear();
        listDailyUnComplete.Clear();
        listDailyAll.Clear();
        foreach (KeyValuePair<int, RenWuTemplate> item in m_TaskDailyDic)
        {
            if (item.Value.progress == -1)
            {
                listDailyComplete.Add(item.Value);
            }
            else
            {
                listDailyUnComplete.Add(item.Value);
            }
        }
        m_TaskDailyDic.Clear();
        TaskInfoTidy();
    }
    List<RenWuTemplate> listDailyComplete = new List<RenWuTemplate>();
    List<RenWuTemplate> listDailyUnComplete = new List<RenWuTemplate>();
    List<RenWuTemplate> listDailyAll = new List<RenWuTemplate>();

    void TaskInfoTidy()
    {
        for (int i = 0; i < listDailyComplete.Count; i++)
        {
            listDailyAll.Add(listDailyComplete[i]);
        }

        for (int i = 0; i < listDailyUnComplete.Count; i++)
        {
            listDailyAll.Add(listDailyUnComplete[i]);
        }
        int size_Daily_All = listDailyAll.Count;
        for (int i = 0; i < size_Daily_All; i++)
        {
            m_TaskDailyDic.Add(listDailyAll[i].id, listDailyAll[i]);
        }
        m_DailyQuestIsRefresh = true;
        MainCityUI.SetRedAlert(106, DailyWetherContainComplete(m_TaskDailyDic));
    }

    public void RequestData()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TaskReq);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DAILY_TASK_LIST_REQ);
        //SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DAILY_TASK_LIST_REQ);
    }

    void RefreshTaskInfo(List<TaskInfo> templist)
    {
        List<ZhuXianTemp> listMain = new List<ZhuXianTemp>();
		List<ZhuXianTemp> listOther = new List<ZhuXianTemp>();
        List<ZhuXianTemp> listBranchComplete = new List<ZhuXianTemp>();
        List<ZhuXianTemp> listBranchUnComplete = new List<ZhuXianTemp>();
        int size_tem = templist.Count;
        for (int j = 0; j < size_tem; j++)
        {
            for (int i = 0; i < ZhuXianTemp.tempTasks.Count; i++)
            {
                if (ZhuXianTemp.tempTasks[i].id == templist[j].id)
                {
                    if (ZhuXianTemp.tempTasks[i].progress != templist[j].progress && ZhuXianTemp.tempTasks[i].type == 0)
                    {
                        if (templist[j].progress < 0)
                        {
                            FunctionOpenTemp.GetMissionAddOpenFunction(ZhuXianTemp.tempTasks[i].id);
                        }
                    }
                    ZhuXianTemp.tempTasks[i].progress = templist[j].progress;
                    if (ZhuXianTemp.tempTasks[i].type == 0)
                    {
                        listMain.Add(ZhuXianTemp.tempTasks[i]);
                    }
                    else
                    {
						listOther.Add(ZhuXianTemp.tempTasks[i]);
                        if (templist[j].progress < 0)
                        {
                            listBranchComplete.Add(ZhuXianTemp.tempTasks[i]);
                        }
                        else
                        {
                            listBranchUnComplete.Add(ZhuXianTemp.tempTasks[i]);
                        }
                    }
                    //  m_TaskInfoDic.Add(tempTaskInfo.id, ZhuXianTemp.tempTasks[i]);
                }
            }
        }
		if(listMain.Count == 0)
		{
			//MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityTaskManagerMain.closeShow();
			ShowId = -1;
		}
		else
		{
			ShowId = listMain[0].id;
		}
		if(listOther.Count == 0)
		{
			m_iShowOtherId = -1;
		}
		else
		{
			m_iShowOtherId = listOther[0].id;

		}

        int s_main = listMain.Count;
        for (int i = 0; i < listMain.Count; i++)
        {
            if (i == 0)
            {
                showTitleOn = true;
                ShowId = listMain[i].id;
            }
            if (listMain[0].progress < 0)
            {
                _isMainComplete = true;
                m_MainComplete = listMain[0];
            }
            ShowId = listMain[0].id;
            // Debug.Log("listMain[i].idlistMain[i].idlistMain[i].id ::" + listMain[i].id);
            m_TaskInfoDic.Add(listMain[i].id, listMain[i]);

        }

        for (int i = 0; i < listBranchComplete.Count; i++)
        {
            if (s_main == 0 && i == 0)
            {
                showTitleOn = true;
                ShowId = listBranchComplete[i].id;
            }
            m_TaskInfoDic.Add(listBranchComplete[i].id, listBranchComplete[i]);
        }

        for (int j = 0; j < listBranchUnComplete.Count; j++)
        {
            for (int i = 0; i < listBranchUnComplete.Count - 1 - j; i++)
            {
                if (listBranchUnComplete[i].rank < listBranchUnComplete[i + 1].rank)
                {
                    ZhuXianTemp t = new ZhuXianTemp();

                    t = listBranchUnComplete[i];

                    listBranchUnComplete[i] = listBranchUnComplete[i + 1];

                    listBranchUnComplete[i + 1] = t;
                }
            }
        }

        for (int i = 0; i < listBranchUnComplete.Count; i++)
        {
            if (s_main == 0 && i == 0)
            {
                showTitleOn = true;
                ShowId = listBranchUnComplete[i].id;
            }
            m_TaskInfoDic.Add(listBranchUnComplete[i].id, listBranchUnComplete[i]);
        }

		if(MainCityUI.m_MainCityUI != null)
		{
			MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityTaskManagerMain.setData(ShowId);
			MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityTaskManagerOther.setData(m_iShowOtherId);
			MainCityUI.m_MainCityUI.m_MainCityUILT.setLTPos(false);
		}

        m_TagIsShow = true;
    }
 

    List<DailyTaskInfo> listComplete = new List<DailyTaskInfo>();
    List<DailyTaskInfo> listUnComplete = new List<DailyTaskInfo>();
    List<DailyTaskInfo> listAll = new List<DailyTaskInfo>();
    void RefreshDailyTaskInfo(DailyTaskListResponse dailyTaskList)
    {
        listAll.Clear();
        for (int i = 0; i < dailyTaskList.taskInfo.Count; i++)
        {
            if (dailyTaskList.taskInfo[i].isFinish)
            {
                listComplete.Add(dailyTaskList.taskInfo[i]);
            }
            else
            {
                listUnComplete.Add(dailyTaskList.taskInfo[i]);
            }
        }

        for (int i = 0; i < listComplete.Count; i++)
        {
            listAll.Add(listComplete[i]);
        }


        for (int j = 0; j < listUnComplete.Count; j++)
        {
            for (int i = 0; i < listUnComplete.Count - 1 - j; i++)
            {
                if (listUnComplete[i].taskId > listUnComplete[i + 1].taskId)
                {
                    DailyTaskInfo t = new DailyTaskInfo();

                    t = listUnComplete[i];

                    listUnComplete[i] = listUnComplete[i + 1];

                    listUnComplete[i + 1] = t;
                }
            }
        }

        for (int i = 0; i < listUnComplete.Count; i++)
        {
            listAll.Add(listUnComplete[i]);
        }
        int size_all = listAll.Count;
        for (int i = 0; i < size_all; i++)
        {
            AddTaskInfo(listAll[i]);
        }

    }
    private bool DailyWetherContainComplete(Dictionary<int, RenWuTemplate> dailyTaskDic)
    {
        foreach (KeyValuePair<int, RenWuTemplate> item in dailyTaskDic)
        {
            if (item.Value.progress < 0)
            {
                return true;
            }
        }
        return false;
    }
    void AddTaskInfo(DailyTaskInfo tempTaskInfo)
    {
        for (int i = 0; i < RenWuTemplate.templates.Count; i++)
        {
            if (tempTaskInfo.taskId == RenWuTemplate.templates[i].id)
            {

                if (tempTaskInfo.isFinish)
                {
                    RenWuTemplate.templates[i].progress = -1;
                }
                else
                {
                    RenWuTemplate.templates[i].progress = tempTaskInfo.jindu;
                }
        
                m_TaskDailyDic.Add(RenWuTemplate.templates[i].id, RenWuTemplate.templates[i]);
            }
        }
        m_TagIsShow = true;

        MainCityUI.SetRedAlert(106, DailyWetherContainComplete(m_TaskDailyDic));
    }

    void OnDestroy(){
        SocketTool.UnRegisterMessageProcessor(this);

		base.OnDestroy();
    }

    public void SendData(int id, int progress)//客户端向服务器发送任务进度，用户对话任务，进度发1表示对话完毕（任务完成）
    {
        if (!FreshGuide.Instance().IsActive(id))
        {
            return;
        }
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        TaskProgress tempSend = new TaskProgress();
        tempSend.task = new TaskInfo();
        tempSend.task.id = id;
        tempSend.task.progress = progress;
        t_qx.Serialize(t_tream, tempSend);

        byte[] t_protof;
        t_protof = t_tream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TaskProgress, ref t_protof);
    }
    void LianXuLintQu(int index)
    {
        if (!UIYindao.m_UIYindao.m_isOpenYindao && DicTaskLX.ContainsKey(index))
        {
            if (FreshGuide.Instance().IsActive(DicTaskLX[index]))
            {
                if (m_TaskInfoDic[DicTaskLX[index]].progress < 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = DicTaskLX[index];
                    if (m_TaskInfoDic[ShowId].m_listYindaoShuju.Count != 0)
                    {
                        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                }
            }
        }
    }

    public bool WetherContainMainTask()
    {
        foreach (KeyValuePair<int, ZhuXianTemp> item in m_TaskInfoDic)
        {
            if (item.Value.type == 0)
            {
                return true;
            }
        }
        return false;
    }
}
