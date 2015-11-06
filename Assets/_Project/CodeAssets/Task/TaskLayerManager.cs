using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskLayerManager : MonoBehaviour {

	public List<EventHandler> m_eventList = new List<EventHandler>();
    public GameObject itemsParent;
    public GameObject objScrollView;
    public List<GameObject>  m_listTanHao;

    public ScaleEffectController m_SEC;
 
    private GameObject TouchedAhead;
    int tempNum = 0;
	void Start()
	{
        TaskData.Instance.isReload = false;
        TaskData.Instance.m_DailyQuestIsRefresh = false;
        m_SEC.OpenCompleteDelegate += ShowInfo;
	}


    void ShowInfo()
    {
        objScrollView.SetActive(true);
        m_eventList[0].transform.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        m_eventList.ForEach(item => item.m_handler += ChangeLayer);
        TaskShow(m_eventList[0].gameObject);
        TaskButtonShow();
        ShowLayer(tempNum);
    }

    void TaskButtonShow()
    {
        m_eventList[1].gameObject.SetActive(FunctionOpenTemp.GetWhetherContainID(106));
        if (FunctionOpenTemp.GetWhetherContainID(106))
        {
            ShowDailyTanhao();
        }
        m_eventList[2].gameObject.SetActive(FunctionOpenTemp.GetWhetherContainID(107));
        m_eventList[3].gameObject.SetActive(FunctionOpenTemp.GetWhetherContainID(108));
    }

    public void ChangeLayer(GameObject tempObject)
    {
        switch (tempObject.name)
        {
            case "ButtonMainQuest":
                {
                    tempNum = 0;
                    ShowLastMainQuest();
                }
                break;
            case "ButtonDailyQuest":
                {
                    tempNum = 1;
                    //UIYindao.m_UIYindao.CloseUI();
                }

                break;
            case "ButtonAchievementQuest":
                {
                    tempNum = 2;
                  //  UIYindao.m_UIYindao.CloseUI();
                }
                break;
            case "ButtonBiographicalQuest":
                {
                    tempNum = 3;
               // UIYindao.m_UIYindao.CloseUI();
                }
                break;
        }
  
        foreach (EventHandler tempHand in m_eventList)
        {
            tempObject.GetComponent<Collider>().enabled = false;
            if (tempHand.GetComponent<TweenScale>() != null)
            {
                tempHand.gameObject.GetComponent<Collider>().enabled = true;
                tempHand.gameObject.GetComponent<TweenScale>().from = new Vector3(1.1f, 1.1f, 1);
                tempHand.gameObject.GetComponent<TweenScale>().to = new Vector3(1, 1, 1);
                tempHand.gameObject.GetComponent<TweenScale>().duration = 0.1f;
                tempHand.gameObject.GetComponent<TweenScale>().enabled = true;
                TouchedAhead = tempHand.gameObject;
                EventDelegate.Add(tempHand.gameObject.GetComponent<TweenScale>().onFinished, ScaleDestroy);
                break;
            }
        }

        if (tempObject.GetComponent<TweenScale>() == null)
        {
            tempObject.AddComponent<TweenScale>();
            tempObject.GetComponent<TweenScale>().enabled = false;
        }
        tempObject.GetComponent<TweenScale>().from = new Vector3(1, 1, 1);
        tempObject.GetComponent<TweenScale>().to = new Vector3(1.1f, 1.1f, 1.1f);
        tempObject.GetComponent<TweenScale>().duration = 0.1f;
        tempObject.GetComponent<TweenScale>().enabled = true;
        ShowLayer(tempNum);
    }
	void ShowLayer(int tempNum)
	{
        itemsParent.transform.GetComponent<TaskGridManager>().CrateItem(tempNum);
	}
    void ScaleDestroy()
    {
        EventDelegate.Remove(TouchedAhead.gameObject.GetComponent<TweenScale>().onFinished, ScaleDestroy);

        Destroy(TouchedAhead.GetComponent<TweenScale>());
    }

    void TaskShow(GameObject tempObject)
    {
        tempObject.GetComponent<Collider>().enabled = false;
        if (tempObject.GetComponent<TweenScale>() == null)
        {
            tempObject.AddComponent<TweenScale>();
            tempObject.GetComponent<TweenScale>().enabled = false;
        }
        tempObject.GetComponent<TweenScale>().from = new Vector3(1, 1, 1);
        tempObject.GetComponent<TweenScale>().to = new Vector3(1.1f, 1.1f, 1.1f);
        tempObject.GetComponent<TweenScale>().duration = 0.1f;
        tempObject.GetComponent<TweenScale>().enabled = true;

    }
    void ShowLastMainQuest()
    {
        //if (UIYindao.m_UIYindao.m_isOpenYindao)
        //{
        //    if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
        //    {
        //       // Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100000;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 3;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

        //    }
        //    else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
        //    {
        //       // Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100010;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 2;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
        //    {
        //      //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100020;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 3;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100030) && TaskData.Instance.m_TaskInfoDic[100030].progress < 0)
        //    {
        //        //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100030;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 3;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0)
        //    {
        //        //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100040;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 3;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100050) && TaskData.Instance.m_TaskInfoDic[100050].progress < 0)
        //    {
        //        //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //        TaskData.Instance.m_iCurMissionIndex = 100050;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 3;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_TaskInfoDic[100060].progress < 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100060;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        // tempTaskData.m_iCurIndex = 4;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress < 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100080;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        // tempTaskData.m_iCurIndex = 4;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100090) && TaskData.Instance.m_TaskInfoDic[100090].progress < 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100090;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        // tempTaskData.m_iCurIndex = 4;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100120) && TaskData.Instance.m_TaskInfoDic[100120].progress < 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100120;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        // tempTaskData.m_iCurIndex = 4;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else if (FreshGuide.Instance().IsActive(100150) && TaskData.Instance.m_TaskInfoDic[100150].progress < 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100150;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        // tempTaskData.m_iCurIndex = 4;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    //else if (FreshGuide.Instance().IsActive(TaskData.Instance.m_iCurMissionIndex) && TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex].yindaoId.Equals("0") && TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex].progress < 0)
        //    //{
        //    //    UIYindao.m_UIYindao.CloseUI();
        //    //}
        //}
    }

    public  void ShowDailyTanhao()
    {
        m_listTanHao[0].SetActive(DailyTaskComplete());
    }

    private  bool DailyTaskComplete()
    {
        foreach (KeyValuePair<int, RenWuTemplate> item in TaskData.Instance.m_TaskDailyDic)
        {
            if (item.Value.progress == -1)
            {
                return true;
            }
        }
        return false;
    }
}
