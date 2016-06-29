﻿using UnityEngine;
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

    public bool m_DailyLocalRefresh = false;
    public bool m_VitaRefresh = false;
  //  public bool m_TaskGetAwardComplete = false;
    public bool m_DestroyMiBao = false;
    public bool isRefrsh = false;
    public ZhuXianTemp m_MainComplete;
    public bool m_IsSaoDangNow = false;
    public bool IsCanShowComplete
    {
        get { return _IsCanShow; }
        set
        {
            _IsCanShow = value;
        }
    }

    private bool _IsCanShow = false;
    public struct VitalityInfo
    {
        public int _todaylHuoYue;
        public int _weekHuoYue;
        public List<int> _listawardStatus; 
		//每个活跃度节点（箱子）的领奖情况（包括周活跃度）0-活跃度不够; 1-已经领取
    }
    public int m_RemainTime = 0;
    public VitalityInfo m_VitalityShowInfo = new VitalityInfo();


    public int m_ShowType = 0;// 0:main 1:side 2:daily
    private bool _isMainComplete = false;

    public Dictionary<int, int> m_TaskRewardsGetID = new Dictionary<int, int>();
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
        if (CityGlobalData.m_isBattleField_V4_2D && Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_MAINCITY))
        {
            CityGlobalData.m_isBattleField_V4_2D = false;
        }
        //if (m_RemainTime > 0 && !_isNeedWait)
        //{
        //    _isNeedWait = true;
        //    StartCoroutine(WaitTimeDown());
        //}
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
        if ( !SceneManager.IsInLoadingScene() 
                && TaskSignalInfoShow.m_TaskSignal == null
                && !FunctionWindowsCreateManagerment.m_IsSaoDangNow
                && !CityGlobalData.m_isBattleField_V4_2D
                && !Global.m_isOpenPVP
                && EquipGrowthWearManagerment.m_EquipGrowth == null
                && TanBaoPage.tbPage == null
                && !m_DestroyMiBao
                && Global.m_isSportDataInItEnd
                )
        {
            if (PlayerModelController.m_playerModelController != null)
            {
                ShowMainTaskAward(m_MainComplete.id);
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
										if (TaskListReponse.list[i].id == 200005)
										{
											ZhuXianTemp tempTaskData = ZhuXianTemp.getTemplateById(200010);
											tempTaskData.m_iCurIndex = 2;
											UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
										}
										if (TaskListReponse.list[i].id == 100173)
										{
											UIYindao.m_UIYindao.CloseUI();
										}
								        if (TaskListReponse.list[i].id == 100470)//镶嵌符文
										{
											UIYindao.m_UIYindao.CloseUI();
										}
                                        //[WARNING]Added by liangxiao.
                                        if (TaskListReponse.list[i].id == 400000)
                                        {
                                            MainCityUI.IsShowFunctionOpenEffectInAllianceCity = true;
                                        }
                                        else if (new List<int>() {100060, 100315, 100370, 100173, 400010, 200040, 100220}.Contains(TaskListReponse.list[i].id))
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


                            m_TaskInfoDic[TaskSyncReponse.task.id].progress = TaskSyncReponse.task.progress;
                            if (TaskLayerManager.m_TaskLayerM)
                            {
                                if (ZhuXianTemp.getTemplateById(TaskSyncReponse.task.id).type == 0)
                                {
                                    m_MainReload = true;
                                }
                                else
                                {
                                    m_SideReload = true;
                                }
                            }

                        }
                        else
                        {
                            List<TaskInfo> list = new List<TaskInfo>();
                            list.Add(TaskSyncReponse.task);
                            RefreshTaskInfo(list);

                            if (TaskLayerManager.m_TaskLayerM)
                            {
                                if (ZhuXianTemp.getTemplateById(TaskSyncReponse.task.id).type == 0)
                                {
                                    m_MainReload = true;
                                }
                                else
                                {
                                    m_SideReload = true;
                                }
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
           
                        if (dailyTaskList != null)
                        {
                            vi._todaylHuoYue = dailyTaskList.todaylHuoYue;
                            vi._weekHuoYue = dailyTaskList.weekHuoYue;
                            if (dailyTaskList.awardStatus != null)
                            {
                                vi._listawardStatus = dailyTaskList.awardStatus;
                            }
                            m_RemainTime = dailyTaskList.remainTime / 1000;
                            m_VitalityShowInfo = vi;
                            if (dailyTaskList.taskInfo != null)
                            {
                                m_TaskDailyDic.Clear();
                                listComplete.Clear();
                                listUnComplete.Clear();
                                RefreshDailyTaskInfo(dailyTaskList);
                                m_TagIsShow = true;
                                if (TaskLayerEveryDayManager.m_TaskLayerM)
                                {
                                    m_DailyQuestIsRefresh = true;
                                }
                            }
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
                                    m_TaskInfoDic.Remove(TaskSyncReponse.taskId);
                                    m_TaskRewardsGetID.Remove(TaskSyncReponse.taskId);
                                    foreach (KeyValuePair<int, ZhuXianTemp> item in m_TaskInfoDic)
                                    {
                                        if (item.Value.type == 0)
                                        {
                                            ShowId = item.Value.id;
                                        }

                                    }

                                    award = ZhuXianTemp.getTemplateById(TaskSyncReponse.taskId).award;
                                    if (TaskLayerManager.m_TaskLayerM)
                                    {
                                        if (ZhuXianTemp.getTemplateById(TaskSyncReponse.taskId).type == 0)
                                        {
                                            m_MainReload = true;
                                        }
                                        else
                                        {
                                            m_SideReload = true;
                                        }
                                    }

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
                            if (TaskLayerEveryDayManager.m_TaskLayerM)
                            {
                                m_DailyQuestIsRefresh = true;
                            }
                            MainCityUI.SetRedAlert(251, DailyWetherContainComplete(m_TaskDailyDic));
 
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
                        if (TaskLayerManager.m_TaskLayerM)
                        {
                            if (vitality.errorCode == 0)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(HuoYueTempTemplate.GetHuoYueTempById(int.Parse(vitality.errorDesc)).award);
                                m_VitaRefresh = true;
                            }
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
        listDailyComplete = TidyDailyInfo(listDailyComplete);
        listDailyUnComplete = TidyDailyInfo(listDailyUnComplete);
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
		if (TaskLayerEveryDayManager.m_TaskLayerM)
        {
            m_DailyQuestIsRefresh = true;
        }
        MainCityUI.SetRedAlert(251, DailyWetherContainComplete(m_TaskDailyDic));
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
        List<ZhuXianTemp> listBranchComplete = new List<ZhuXianTemp>();
        List<ZhuXianTemp> listBranchUnComplete = new List<ZhuXianTemp>();
        int size_tem = templist.Count;
        for (int j = 0; j < size_tem; j++)
        {
			for (int i = 0; i < ZhuXianTemp.GetTemplatesCount(); i++)
            {
				if (ZhuXianTemp.GetTemplateByIndex( i ).id == templist[j].id)
                {
					if (ZhuXianTemp.GetTemplateByIndex( i ).progress != templist[j].progress && ZhuXianTemp.GetTemplateByIndex( i ).type == 0)
                    {
                        if (templist[j].progress < 0)
                        {
							FunctionOpenTemp.GetMissionAddOpenFunction(ZhuXianTemp.GetTemplateByIndex( i ).id);
                        }
                    }
					ZhuXianTemp.GetTemplateByIndex( i ).progress = templist[j].progress;
					if (ZhuXianTemp.GetTemplateByIndex( i ).type == 0)
                    {
						listMain.Add(ZhuXianTemp.GetTemplateByIndex( i ) );
                    }
                    else
                    {
                        if (templist[j].progress < 0)
                        {
							listBranchComplete.Add(ZhuXianTemp.GetTemplateByIndex( i ) );
                        }
                        else
                        {
							listBranchUnComplete.Add(ZhuXianTemp.GetTemplateByIndex( i ) );
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
		 

        for (int i = 0; i < listMain.Count; i++)
        {
            if (i == 0)
            {
                showTitleOn = true;
                ShowId = listMain[i].id;
            }
 
            m_TaskInfoDic.Add(listMain[i].id, listMain[i]);
        }

        if (listMain[0].progress < 0)
        {
            if (!string.IsNullOrEmpty(listMain[0].award))
            {
                m_MainComplete = listMain[0];
                _isMainComplete = true;
            }
            else
            {
                GetQuestAward(listMain[0].id);
            }
        }

        ShowId = listMain[0].id;

        for (int i = 0; i < listBranchComplete.Count; i++)
        {
            m_TaskInfoDic.Add(listBranchComplete[i].id, listBranchComplete[i]);
        }

        for (int j = 0; j < listBranchUnComplete.Count; j++)
        {
            for (int i = 0; i < listBranchUnComplete.Count - 1 - j; i++)
            {
                if (listBranchUnComplete[i].rank > listBranchUnComplete[i + 1].rank)
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
            m_TaskInfoDic.Add(listBranchUnComplete[i].id, listBranchUnComplete[i]);
        }

        if (listBranchComplete.Count != 0)
        {
            if (string.IsNullOrEmpty(listBranchComplete[0].award))
            {
                 GetQuestAward(listBranchComplete[0].id);
            }
            m_iShowOtherId = listBranchComplete[0].id;
        }
        else if (listBranchUnComplete.Count != 0)
        {
            m_iShowOtherId = listBranchUnComplete[0].id;
        }
        else
        {
//            Debug.Log("SSSSSSSSSS");
            m_iShowOtherId = -1;
         
        }

		if(MainCityUI.m_MainCityUI != null)
		{
			MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityTaskManagerMain.setData(ShowId);
			MainCityUI.m_MainCityUI.m_MainCityUILT.m_MainCityTaskManagerOther.setData(m_iShowOtherId);
			MainCityUI.m_MainCityUI.m_MainCityUILT.setLTPos(false);
		}

        if (FreshGuide.Instance().IsActive(100177) && TaskData.Instance.m_TaskInfoDic[100177].progress >= 0 && SignalInManagerment.m_SignalIn)
        {
            TaskData.Instance.m_iCurMissionIndex = 100177;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        m_TagIsShow = true;
    }
 

    List<RenWuTemplate> listComplete = new List<RenWuTemplate>();
    List<RenWuTemplate> listUnComplete = new List<RenWuTemplate>();
    List<RenWuTemplate> listAll = new List<RenWuTemplate>();
    void RefreshDailyTaskInfo(DailyTaskListResponse dailyTaskList)
    {
        listAll.Clear();
        listComplete.Clear();
        listUnComplete.Clear();
        for (int j = 0; j < dailyTaskList.taskInfo.Count; j++)
        {
            for (int i = 0; i < RenWuTemplate.templates.Count; i++)
            {
                if (dailyTaskList.taskInfo[j].taskId == RenWuTemplate.templates[i].id)
                {
                    if (dailyTaskList.taskInfo[j].isFinish)
                    {
                        RenWuTemplate.templates[i].progress = -1;
                        listComplete.Add(RenWuTemplate.templates[i]);
                    }
                    else
                    {
                        RenWuTemplate.templates[i].progress = dailyTaskList.taskInfo[j].jindu;
                        listUnComplete.Add(RenWuTemplate.templates[i]);
                    }
                }
            }
        }
        listComplete = TidyDailyInfo(listComplete);
        listUnComplete = TidyDailyInfo(listUnComplete);
        for (int i = 0; i < listComplete.Count; i++)
        {
            listAll.Add(listComplete[i]);
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
    void AddTaskInfo(RenWuTemplate tempTaskInfo)
    {
       
        m_TagIsShow = true;
        m_TaskDailyDic.Add(tempTaskInfo.id, tempTaskInfo);
        MainCityUI.SetRedAlert(251, DailyWetherContainComplete(m_TaskDailyDic));
    }

    private List<RenWuTemplate> TidyDailyInfo(List<RenWuTemplate>  TaskInfo)
    {
        for (int j = 0; j < TaskInfo.Count; j++)
        {
            for (int i = 0; i < TaskInfo.Count - 1 - j; i++)
            {
                if (TaskInfo[i].site >TaskInfo[i + 1].site)
                {
                    RenWuTemplate t = TaskInfo[i];
                    TaskInfo[i] = TaskInfo[i + 1];
                    TaskInfo[i + 1] = t;
                }
            }
        }
        return TaskInfo;
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

    public void GetQuestAward(int quest_Id)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetTaskReward tempRequest = new GetTaskReward();
        tempRequest.taskId = quest_Id;
        t_qx.Serialize(t_tream, tempRequest);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GetTaskReward, ref t_protof);
        
    }

    public void GetDailyQuestAward(int quest_Id)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetTaskReward tempRequest = new GetTaskReward();
        tempRequest.taskId = quest_Id;
        t_qx.Serialize(t_tream, tempRequest);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DAILY_TASK_GET_REWARD_REQ, ref t_protof);
       
    }

    public static void ShowMainTaskAward(int id)
    {
        TaskSignalInfoShow.m_TaskId = id;
        TaskCompleteEffect();
    }
    private static void TaskCompleteEffect()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_EFFECT), AddUIPanel);
    }
    private static void AddUIPanel(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        UIYindao.m_UIYindao.CloseUI();
    }

}
