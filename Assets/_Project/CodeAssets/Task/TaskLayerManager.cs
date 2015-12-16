using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class TaskLayerManager : MonoBehaviour
{

    public List<EventIndexHandle> m_listEvent;
    public List<EventIndexHandle> m_listMainTaskEvent;
    public UIGrid m_RewardParent;
    public UIGrid m_ItemParent;
    public List<GameObject> m_listTanHao;

    public ScaleEffectController m_SEC;
    public UILabel m_labMainQuestName;
    public UILabel m_labMainQuestDes;
    public UILabel m_labMainQuestTarget;
    public UISprite m_SpriteMainQuest;

    public GameObject m_MainQuestObj;
    public GameObject m_OtherQuestObj;
    public GameObject m_MainParent;

    public GameObject m_ObjFinish;
    public GameObject m_RewardPanel;

    private GameObject TouchedAhead;
    private int _index_OtherNum = 0;

    public struct TaskNeedInfo
    {
        public int _TaskId;
        public int _Type;
        public string _npcIcon;
        public string _Name;
        public string _Des;
        public string _Target;
        public int _Progress;
        public int _TriggerType;
        public List<FunctionWindowsCreateManagerment.RewardInfo> _listReward;
    };
    private List<TaskNeedInfo> _listTaskInfo = new List<TaskNeedInfo>();
    private int _tempNum = 0;
    private int _touchIndex = 0;
   
    void Start()
    {
        m_listMainTaskEvent.ForEach(p => p.m_Handle += MainTaskTouch);
        m_listEvent.ForEach(p => p.m_Handle += TouchInfo);
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }

    void OnEnable()
    {
        TaskData.Instance.m_MainReload = false;
        TaskData.Instance.m_MainReload = false;
        TaskData.Instance.m_DailyQuestIsRefresh = false;
    }
    void Update()
    {
        if (TaskData.Instance.m_MainReload)
        {
            TaskData.Instance.m_MainReload = false;
            TidyMainTaskInfo();
            TanHaoShow();
        }


        if (TaskData.Instance.m_MainReload)
        {
            TaskData.Instance.m_MainReload = false;
            TidySideTaskInfo();
            TanHaoShow();

        }
        if (TaskData.Instance.m_DailyQuestIsRefresh)
        {
            TaskData.Instance.m_DailyQuestIsRefresh = false;
            TidyDailyTaskInfo();
            TanHaoShow();
        }
    }

    void MainTaskTouch(int index)
    {
        if (index == 0)
        {
            m_MainParent.SetActive(false);
            MainCityUI.m_MainCityUI.m_MainCityUILT.ClickTasID(_listTaskInfo[0]._TaskId);
        }
        else
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            GetTaskReward tempRequest = new GetTaskReward();
            tempRequest.taskId = _listTaskInfo[0]._TaskId;
            t_qx.Serialize(t_tream, tempRequest);

            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GetTaskReward, ref t_protof);
        }


    }
    void TouchInfo(int index)
    {
        if (_touchIndex != index)
        {
            m_listEvent[index].GetComponent<TaskButtonEffectManagerment>().ButtonState(true);
            m_listEvent[_touchIndex].GetComponent<TaskButtonEffectManagerment>().ButtonState(false);
            _touchIndex = index;
            if (index == 0)
            {
                m_MainQuestObj.SetActive(true);
                m_OtherQuestObj.SetActive(false);
            }
            else
            {
                m_MainQuestObj.SetActive(false);
                m_OtherQuestObj.SetActive(true);
            }

            switch (index)
            {
                case 0:
                    {
                        TidyMainTaskInfo();
                    }
                    break;
                case 1:
                    {
                        TidySideTaskInfo();
                    }
                    break;
                case 2:
                    {
                        TidyDailyTaskInfo();
                    }
                    break;
                default:
                    break;

            }
        }
    }
    void ShowInfo()
    {
     
        m_listEvent[TaskData.Instance.m_ShowType].GetComponent<TaskButtonEffectManagerment>().ButtonState(true);
 
        m_listEvent[1].gameObject.SetActive(FunctionOpenTemp.GetWhetherContainID(107));
        m_listEvent[2].gameObject.SetActive(FunctionOpenTemp.GetWhetherContainID(106));
        switch (TaskData.Instance.m_ShowType)
        {
            case 0:
                {
                    TidyMainTaskInfo();
                }
                break;
            case 1:
                {
                    TidySideTaskInfo();
                }
              break;
            case 2:
                {
                    TidyDailyTaskInfo();
                }
              break;
            default:
                break;
        }
        TaskData.Instance.m_ShowType = 0;
        TanHaoShow();
    }

    void TanHaoShow()
    {
        int ss = m_listTanHao.Count;
        for (int i = 0; i < ss; i++)
        {
            m_listTanHao[i].SetActive(IsProgressDone(i));
        }
    }

    void TidyMainTaskInfo()
    {
        _listTaskInfo.Clear();
        foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
        {
           if (ZhuXianTemp.getTemplateById(item.Value.id).type == 0)
            {
                TaskNeedInfo tni = new TaskNeedInfo();
                tni._TaskId = item.Value.id;
                tni._Name = item.Value.title;
                tni._Des = item.Value.desc; ;
                tni._Target = item.Value.title;
                tni._Progress = item.Value.progress;
                tni._npcIcon = "Icon_RenWuType_" + item.Value.icon;
                tni._listReward = FunctionWindowsCreateManagerment.GetRewardInfo(item.Value.award);
                _listTaskInfo.Add(tni);
            }
        }

        showMainTaskInfo();
    }

    void showMainTaskInfo()
    {
        m_listMainTaskEvent[0].gameObject.SetActive(_listTaskInfo[0]._Progress >= 0);
        m_listMainTaskEvent[1].gameObject.SetActive(_listTaskInfo[0]._Progress < 0);
        m_labMainQuestName.text = _listTaskInfo[0]._Name;
        m_labMainQuestDes.text = _listTaskInfo[0]._Des;
        m_labMainQuestTarget.text = _listTaskInfo[0]._Name;// + MyColorData.getColorString(4,
                     //_listTaskInfo[0]._Progress.ToString() + "/" + ZhuXianTemp.getTemplateById(_listTaskInfo[0]._TaskId).progress.ToString());
        m_SpriteMainQuest.spriteName = _listTaskInfo[0]._npcIcon;
        int size_a = m_RewardParent.transform.childCount;
        if (size_a > 0)
        {
            for (int i = 0; i < size_a; i++)
            {
                Destroy(m_RewardParent.transform.GetChild(i).gameObject);
            }
        }
        RewardItemCreate();
    }

    void TidySideTaskInfo()
    {
        _listTaskInfo.Clear();
        foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
        {
            if ( item.Value.type == 1)
            {
                TaskNeedInfo tni = new TaskNeedInfo();
                tni._TaskId = item.Value.id;
                tni._Type = 1;
                tni._Name = item.Value.title;
                tni._Des = item.Value.desc; ;
                tni._Target = item.Value.title;
                tni._Progress = item.Value.progress;
                tni._npcIcon = item.Value.icon;
                tni._listReward = FunctionWindowsCreateManagerment.GetRewardInfo(item.Value.award);
                _listTaskInfo.Add(tni);
            }
        }
        OtherTaskShow();
    }

    void TidyDailyTaskInfo()
    {
        _listTaskInfo.Clear();
        foreach (KeyValuePair<int, RenWuTemplate> item in TaskData.Instance.m_TaskDailyDic)
        {
            TaskNeedInfo tni = new TaskNeedInfo();
            tni._TaskId = item.Value.id;
            tni._Type = 2;
            tni._Name = NameIdTemplate.GetName_By_NameId(item.Value.m_name);
            tni._Des = DescIdTemplate.GetDescriptionById(item.Value.funDesc);
            tni._Target = NameIdTemplate.GetName_By_NameId(item.Value.m_name);
            tni._Progress = item.Value.progress;
            tni._npcIcon = item.Value.icon;
            tni._listReward = FunctionWindowsCreateManagerment.GetRewardInfo(item.Value.jiangli);
            _listTaskInfo.Add(tni);
        }
        OtherTaskShow();
    }
    void OtherTaskShow()
    {
        _index_OtherNum = 0;
        int size = m_ItemParent.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(m_ItemParent.transform.GetChild(i).gameObject);
        }

        int size_a = _listTaskInfo.Count;

        for (int i = 0; i < size_a; i++)
        {
            CreateScrollViewItem();
        }
    }
 
 
   
    public void ShowDailyTanhao()
    {
        m_listTanHao[0].SetActive(DailyTaskComplete());
    }

    private bool DailyTaskComplete()
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



    int index_Num = 0;
    void RewardItemCreate()
    {
        index_Reward = 0;
        int num = _listTaskInfo[0]._listReward.Count;
        for (int i = 0; i < num; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }
    int index_Reward = 0;
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_RewardParent != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_RewardParent.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID(_listTaskInfo[0]._listReward[index_Reward].icon, _listTaskInfo[0]._listReward[index_Reward].count.ToString(),4);
            iconSampleManager.SetIconPopText(_listTaskInfo[0]._listReward[index_Reward].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listTaskInfo[0]._listReward[index_Reward].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listTaskInfo[0]._listReward[index_Reward].icon).descId));
            if (index_Reward < _listTaskInfo[0]._listReward.Count - 1)
            {
                index_Reward++;
            }
            else
            {
                m_MainQuestObj.SetActive(true);
            }

            m_RewardParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    void CreateScrollViewItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_SCROLL_VIEW_ITEM_AMEND), ResourcesLoadCallBack);
    }
  
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {

        if (m_ItemParent != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_ItemParent.transform;
            tempObject.name = index_Num.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.GetComponent<TaskScrollViewItemAmend>().ShowTaskInfo(_listTaskInfo[_index_OtherNum], ShowReward);

            if (_index_OtherNum < _listTaskInfo.Count - 1)
            {
                _index_OtherNum++;
            }
            m_ItemParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void ShowReward(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
        if (taskInfo._Progress < 0)
        {
            m_ObjFinish.SetActive(true);
            StartCoroutine(WaitSecond(taskInfo, obj));
        }
        else
        {
            m_MainParent.SetActive(false);
            MainCityUI.m_MainCityUI.m_MainCityUILT.ClickTasID(taskInfo._TaskId);
        }
    }

    IEnumerator WaitSecond(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);
        m_ObjFinish.SetActive(false);
        m_RewardPanel.SetActive(true);
        m_RewardPanel.GetComponent<TaskRewardsShow>().Show(taskInfo, obj);
    }
    private bool IsProgressDone(int type)
    {
        if (type < 2)
        {
            foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
            {
                if (item.Value.type == type)
                {
                    if (type == 1)
                    {
                        if (item.Value.progress < 0 && FunctionOpenTemp.GetWhetherContainID(107))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (item.Value.progress < 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        else
        {
            foreach (KeyValuePair<int, RenWuTemplate> item in TaskData.Instance.m_TaskDailyDic)
            {
                if (item.Value.progress < 0 && FunctionOpenTemp.GetWhetherContainID(106))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
