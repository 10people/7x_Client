using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TaskGridManager : MonoBehaviour
{

    private Transform m_transform;
    public GameObject m_RewardPanel;
    public GameObject m_MainObject;
    public bool m_isTouched = false;
  //  private UIGrid m_grid;
  //  public UIScrollView m_ScrollView;
 //   public UIScrollBar m_ScrollBar;
    public List<UIScrollBar> m_listScrollBar = new List<UIScrollBar>();
    public List<UIScrollView> m_listScrollView = new List<UIScrollView>();
    public List<UIGrid> m_listGrid = new List<UIGrid>();
    public GameObject m_ObjFinish;
    private float barValue = 0;

    struct TaskShowInfo
    {
        public int tempItemId;
        public int tempJindu;
        public bool tempComplete;
        public string reward;
        public string name;
        public string des;
        public int triggerCond;
        public int doneCond;
        public int Type;
        public string icon;
        public int guoji1;
        public int guoji2;
        public int gongJinTimes;
        public string title;

    }
  //  private TaskShowInfo m_taskShowinfo;
    private List<TaskShowInfo> listTaskShowInfo = new List<TaskShowInfo>();
    private int CurrentTaskTYpe = 0;
    public GameObject m_ObjLayer;

    Dictionary<int, GameObject> m_gameObjectDic = new Dictionary<int, GameObject>(); //item 集合
    void Start()
    {
        m_transform = this.transform;
    }

    //void Update()
    //{
    //    if (TaskData.Instance.isReload)
    //    {
    //        TaskData.Instance.isReload = false;
    //        if (CityGlobalData.m_TaskType == (int)TaskType.MainQuest)
    //        {
    //            CrateItem(CityGlobalData.m_TaskType);
    //        }
    //    }

    //    if (TaskData.Instance.m_DailyQuestIsRefresh)
    //    {
    //        TaskData.Instance.m_DailyQuestIsRefresh = false;
    //        if (FunctionOpenTemp.GetWhetherContainID(106))
    //        {
    //            m_ObjLayer.GetComponent<TaskLayerManager>().ShowDailyTanhao();
    //        }
 
    //        if (CityGlobalData.m_TaskType == (int)TaskType.DailyQuest)
    //        {
    //            CrateItem(CityGlobalData.m_TaskType);
    //        }
    //    }
    //}

    public void CrateItem(int indexType)//创建任务 item
    {
        CityGlobalData.m_TaskType = indexType;
        foreach (KeyValuePair<int, GameObject> key in m_gameObjectDic)
        {
            Destroy(key.Value);
        }
        m_gameObjectDic.Clear();
        int size = m_listScrollBar.Count;
        for (int i = 0; i < size; i++)
        {
            if (i == CityGlobalData.m_TaskType)
            {
                m_listScrollBar[i].gameObject.SetActive(true);
            }
            else
            {
                m_listScrollBar[i].gameObject.SetActive(false);
            }
        }


        m_listScrollView[CityGlobalData.m_TaskType].verticalScrollBar = m_listScrollBar[CityGlobalData.m_TaskType];

        // m_listScrollView[CityGlobalData.m_TaskType].verticalScrollBar.alpha = 0;

        m_listScrollView[CityGlobalData.m_TaskType].transform.localPosition = new Vector3(-365, -31, 0);
        m_listScrollView[CityGlobalData.m_TaskType].GetComponent<UIPanel>().clipOffset = new Vector2(405, 1);

        switch ((TaskType)indexType)
        {
            case TaskType.MainQuest:
                {
                    listTaskShowInfo.Clear();
                    int index = 0;
                    index_Num = 0;
                    foreach (KeyValuePair<int, ZhuXianTemp> key in TaskData.Instance.m_TaskInfoDic)
                    {
                       // Debug.Log("key.Value.typekey.Value.typekey.Value.type ::" + key.Value.type);
                        if (key.Value.type == 0 && index == 0)
                        {
                            TaskShowInfo tinfo = new TaskShowInfo();
                            tinfo.tempItemId = key.Value.id;
                            tinfo.tempJindu = key.Value.progress;
                            tinfo.tempComplete = key.Value.progress < 0 ? true : false;
                            tinfo.reward = key.Value.award;
                            tinfo.name = key.Value.title;
                            tinfo.des = key.Value.desc;
                            tinfo.triggerCond = key.Value.triggerType;
                            tinfo.doneCond = 0;
                            tinfo.Type = key.Value.type;
                            tinfo.icon = key.Value.icon;
                            tinfo.title = key.Value.title;
                            listTaskShowInfo.Add(tinfo);
                         //   CreateScrollViewItem();
                            //CreateScrollViewItem(key.Value.id, key.Value.progress, key.Value.progress < 0 ? true : false, key.Value.award, key.Value.title, key.Value.desc, key.Value.triggerCond, key.Value.doneCond, key.Value.icon);
                            index++;
                        }
                        else  if (key.Value.type == 1)
                        {
                            TaskShowInfo tinfo = new TaskShowInfo();
                            tinfo.tempItemId = key.Value.id;
                            tinfo.tempJindu = key.Value.progress;
                            tinfo.tempComplete = key.Value.progress < 0 ? true : false;
                            tinfo.reward = key.Value.award;
                            tinfo.name = key.Value.title;
                            tinfo.des = key.Value.desc;
                            tinfo.triggerCond = key.Value.triggerType;
                            tinfo.doneCond = 0;
                            tinfo.Type = key.Value.type;
                            tinfo.icon = key.Value.icon;
                            tinfo.title = key.Value.title;
                            listTaskShowInfo.Add(tinfo);
                           // CreateScrollViewItem();
                        }
                    }

                      int size2 = listTaskShowInfo.Count;

                      if (size2 <= 4)
                      {
                          m_listScrollView[CityGlobalData.m_TaskType].enabled = false;
                      }
                      else
                      {
                          m_listScrollView[CityGlobalData.m_TaskType].enabled = true;
                      }
                      for (int i = 0; i < size2; i++)
                      {
                          CreateScrollViewItem();
                      }
                }
                break;
            case TaskType.DailyQuest:
                {
                    //if (_SaveObject != null)
                    //{
                    //    UI3DEffectTool.ClearUIFx(_SaveObject);
                    //}
                    index_Num = 0;
                    listTaskShowInfo.Clear();
                    foreach (KeyValuePair<int, RenWuTemplate> key in TaskData.Instance.m_TaskDailyDic)
                    {
                        TaskShowInfo tinfo = new TaskShowInfo();
                        tinfo.tempItemId = key.Value.id;
                        tinfo.tempJindu = key.Value.progress;
                        tinfo.tempComplete = key.Value.progress < 0 ? true : false;
                    
                        tinfo.name = NameIdTemplate.GetName_By_NameId(key.Value.m_name);
                       // if (key.Value.id == 19)
                       //  {
                       //   string [] sst = DescIdTemplate.GetDescriptionById(key.Value.funDesc).Split('#');
                       //   tinfo.des = sst[0] +  key.Value.guojia1 + sst[1] ;
                       //   tinfo.reward = key.Value.jiangli;
                       //  }
                       //else
                        {
                            tinfo.reward = key.Value.jiangli;
                            tinfo.des = DescIdTemplate.GetDescriptionById(key.Value.funDesc);
                        }
                        
                        tinfo.triggerCond = key.Value.triggerType;
                        tinfo.doneCond = 0;
                        tinfo.Type = 2;
                        tinfo.icon = key.Value.icon;
                        tinfo.title = NameIdTemplate.GetName_By_NameId(key.Value.funDesc);
                        listTaskShowInfo.Add(tinfo);
                      
                       // CreateScrollViewItem(key.Value.id, key.Value.progress, key.Value.progress < 0 ? true : false, key.Value.jiangli, NameIdTemplate.GetName_By_NameId(key.Value.m_name), DescIdTemplate.GetDescriptionById(key.Value.funDesc), key.Value.triggerType, 0, key.Value.icon);
                    }
                    int sizze =listTaskShowInfo.Count;
                    if (sizze <= 4)
                    {
                        m_listScrollView[CityGlobalData.m_TaskType].enabled = false;
                    }
                    else
                    {
                        m_listScrollView[CityGlobalData.m_TaskType].enabled = true;
                    }
                    for (int i = 0; i < sizze;i++ )
                    {
                        CreateScrollViewItem();
                    }
               
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
        m_listScrollBar[CityGlobalData.m_TaskType].value = barValue;
    }

    GameObject objMid;
    int index_Num = 0;
    void CreateScrollViewItem()
    {
       Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_SCROLL_VIEW_ITEM_AMEND), ResourcesLoadCallBack);
    }
    //private GameObject _SaveObject = null;
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
      
        if (m_listGrid[CityGlobalData.m_TaskType] != null)
        {
            if(!m_gameObjectDic.ContainsKey(listTaskShowInfo[index_Num].tempItemId))
            {
                    GameObject tempObject = Instantiate(p_object) as GameObject;
                    if (UIYindao.m_UIYindao.m_isOpenYindao)
                    {
                        Destroy(tempObject.GetComponent<UIDragScrollView>());
                    }


                    tempObject.transform.parent = m_listGrid[CityGlobalData.m_TaskType].transform;

                    tempObject.name = index_Num.ToString();//listTaskShowInfo[index_Num].tempItemId.ToString();
                    if (CityGlobalData.TaskLingQu)
                    {
                        CityGlobalData.TaskLingQu = false;
                        objMid = tempObject;
                        if (tempObject.GetComponent<TweenScale>() == null)
                        {
                            tempObject.AddComponent<TweenScale>();
                            tempObject.GetComponent<TweenScale>().enabled = false;
                        }
                        tempObject.transform.localScale = Vector3.one;
                        tempObject.GetComponent<TweenScale>().from = new Vector3(0.2f, 0.2f, 1);
                        tempObject.GetComponent<TweenScale>().to = Vector3.one;
                        tempObject.GetComponent<TweenScale>().duration = 0.6f;
                        tempObject.GetComponent<TweenScale>().enabled = true;
                        EventDelegate.Add(tempObject.GetComponent<TweenScale>().onFinished, Remove);
                    }
                    else
                    {
                        tempObject.transform.localScale = Vector3.one;
                    }
                  
                    tempObject.transform.localPosition = Vector3.zero;
                    //if (listTaskShowInfo[index_Num].Type == 0)
                    //{
                    //    _SaveObject = tempObject;
                    //}
               
                   tempObject.GetComponent<TaskScrollViewItemAmend>().m_Kuang.SetActive(listTaskShowInfo[index_Num].tempComplete);
              
                    //tempObject.GetComponent<TaskScrollViewItemAmend>().ShowTaskInfo(listTaskShowInfo[index_Num].Type,
                    //    listTaskShowInfo[index_Num].tempItemId, listTaskShowInfo[index_Num].tempJindu, listTaskShowInfo[index_Num].tempComplete,
                    //    listTaskShowInfo[index_Num].reward, listTaskShowInfo[index_Num].name, listTaskShowInfo[index_Num].des,
                    //    listTaskShowInfo[index_Num].triggerCond, listTaskShowInfo[index_Num].doneCond, ShowReward, listTaskShowInfo[index_Num].title);

                    tempObject.GetComponent<TaskScrollViewItemAmend>().m_taskIcon.spriteName = "Icon_RenWuType_" + listTaskShowInfo[index_Num].icon;

                    m_gameObjectDic.Add(listTaskShowInfo[index_Num].tempItemId, tempObject);

                    //  m_ScrollView.verticalScrollBar = m_ScrollBar;
                    m_listGrid[CityGlobalData.m_TaskType].repositionNow = true;
                    // m_listScrollView[CityGlobalData.m_TaskType].v
                    m_listScrollBar[CityGlobalData.m_TaskType].gameObject.SetActive(true);
                    if (index_Num < listTaskShowInfo.Count - 1)
                    {
                        index_Num++;
                    }
                    else
                    {
                        if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            if (FreshGuide.Instance().IsActive(100000) && TaskData.Instance.m_TaskInfoDic[100000].progress < 0)
                            {
                                TaskData.Instance.m_iCurMissionIndex = 100000;
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                tempTaskData.m_iCurIndex = 2;

                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

                            }
                            else if (FreshGuide.Instance().IsActive(100010) && TaskData.Instance.m_TaskInfoDic[100010].progress < 0)
                            {
                                TaskData.Instance.m_iCurMissionIndex = 100010;
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                tempTaskData.m_iCurIndex = 3;
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                            else if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_TaskInfoDic[100020].progress < 0)
                            {
                                TaskData.Instance.m_iCurMissionIndex = 100020;
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                tempTaskData.m_iCurIndex = 9;
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                            //else if (FreshGuide.Instance().IsActive(100030) && TaskData.Instance.m_TaskInfoDic[100030].progress < 0)
                            //{
                            //    //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                            //    TaskData.Instance.m_iCurMissionIndex = 100030;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    tempTaskData.m_iCurIndex = 3;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0)
                            //{
                            //    //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                            //    TaskData.Instance.m_iCurMissionIndex = 100040;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    tempTaskData.m_iCurIndex = 3;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100050) && TaskData.Instance.m_TaskInfoDic[100050].progress < 0)
                            //{
                            //    //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                            //    TaskData.Instance.m_iCurMissionIndex = 100050;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    tempTaskData.m_iCurIndex = 3;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_TaskInfoDic[100060].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100060;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100080;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100090) && TaskData.Instance.m_TaskInfoDic[100090].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100090;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100120) && TaskData.Instance.m_TaskInfoDic[100120].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100120;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100150) && TaskData.Instance.m_TaskInfoDic[100150].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100150;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100155) && TaskData.Instance.m_TaskInfoDic[100155].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100155;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100170;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            //else if (FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress < 0)
                            //{
                            //    TaskData.Instance.m_iCurMissionIndex = 100180;
                            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            //    // tempTaskData.m_iCurIndex = 4;
                            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            //}
                            else
                            {
                                if (TaskData.Instance.m_iCurMissionIndex > 100020)
                                {
                                    UIYindao.m_UIYindao.CloseUI();
                                }
                            }
                        }
                    }
                }
                else 
                {
                  p_object = null;
                }
            }
        else
        {
            p_object = null;
        }
    }
    void OnDestroy()
    {
    }
    public int m_SaveId = 0;
    void ShowReward(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
 
        //if (id == 0 && )
        //{
 
        //    MainCityUI.TryRemoveFromObjectList(m_MainObject);
        //}
        //else 
        //if (!string.IsNullOrEmpty(award))
        //{
        //    if (!m_isTouched)
        //    {
        //        m_isTouched = true;
        //        m_SaveId = id;
        //        m_ObjFinish.SetActive(true);
        //        StartCoroutine(WaitSecond(award, title, id, obj));
        //    }
        //}
        //else 
        //{
        //    if (TaskData.Instance.m_TaskInfoDic.ContainsKey(id))
        //    {
        //        MainCityUI.TryRemoveFromObjectList(m_MainObject);
        //        if (TaskData.Instance.m_TaskInfoDic[id].LinkNpcId != -1 && TaskData.Instance.m_TaskInfoDic[id].FunctionId == -1)
        //        {
        //            NpcManager.m_NpcManager.setGoToNpc(TaskData.Instance.m_TaskInfoDic[id].LinkNpcId);
        //        }
        //        else if (TaskData.Instance.m_TaskInfoDic[id].LinkNpcId == -1 && TaskData.Instance.m_TaskInfoDic[id].FunctionId != -1)
        //        {
        //            FunctionWindowsCreateManagerment.FunctionWindowCreate(TaskData.Instance.m_TaskInfoDic[id].FunctionId);
        //        }
        //    }
        //    else if(TaskData.Instance.m_TaskDailyDic.ContainsKey(id))
        //    {
        //        MainCityUI.TryRemoveFromObjectList(m_MainObject);
        //        if (TaskData.Instance.m_TaskDailyDic[id].LinkNpcId != -1 && TaskData.Instance.m_TaskDailyDic[id].FunctionId == -1)
        //        {
        //            NpcManager.m_NpcManager.setGoToNpc(TaskData.Instance.m_TaskDailyDic[id].LinkNpcId);
        //        }
        //        else if (TaskData.Instance.m_TaskDailyDic[id].LinkNpcId == -1 && TaskData.Instance.m_TaskDailyDic[id].FunctionId != -1)
        //        {
        //            FunctionWindowsCreateManagerment.FunctionWindowCreate(TaskData.Instance.m_TaskDailyDic[id].FunctionId);
        //        }
        //    }
        //    Destroy(m_MainObject);
        //}
      
    }
    IEnumerator WaitSecond(string award, string title, int id, GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);
        m_ObjFinish.SetActive(false);
        m_RewardPanel.SetActive(true);
       //m_RewardPanel.GetComponent<TaskRewardsShow>().Show(award, title, id, obj);
    }
        void Remove()
    {
        EventDelegate.Remove(objMid.GetComponent<TweenScale>().onFinished, Remove);
        Destroy(objMid.GetComponent<TweenScale>());
    }


}
