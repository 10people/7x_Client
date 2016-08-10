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
    public static TaskLayerManager m_TaskLayerM;
    public List<EventIndexHandle> m_listEvent;
    public UISprite m_MoveSprite;
    public UILabel m_LabelNullSignal;
    public UISprite m_ForeSprite;
    public List<EventIndexHandle> m_listMainTaskEvent;
    private Dictionary<int, GameObject> _QuestButtonDic = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> _SideItemDic = new Dictionary<int, GameObject>();
    public UIGrid m_RewardParent;
    public UIGrid m_ItemParent;
 
    public UIScrollView m_ScrollView;
    public UIGrid m_ButtonParent;
    public GameObject m_Durable_UI;
    //public List<GameObject> m_listTanHao;
 
 
    public ScaleEffectController m_SEC;
    public UILabel m_labMainQuestName;
    public UILabel m_labOtherQuestTitle;
    public UILabel m_labMainQuestDes;
    public UILabel m_labMainQuestTarget;
    public UISprite m_SpriteMainQuest;

    public GameObject m_MainQuestObj;
    public GameObject m_OtherQuestObj;
    public GameObject m_MainParent;
 
    public GameObject m_ObjFinish;
    public GameObject m_RewardPanel;

    public UITexture m_TextureIcon;

    private GameObject TouchedAhead;
    private int _index_OtherNum = 0;
    public GameObject m_DailyQuestObj;

 
    public GameObject m_ObjTopLeft;
 
    public bool m_isFinishCurrent = false;
    public struct TaskType
    {
        public int _type;
        public string _Icon;
    };
  

    private List<TaskType> _listTaskButton = new List<TaskType>();
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
        m_TaskLayerM = this;
        _touchIndex = TaskData.Instance.m_ShowType;
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "任务", 59, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_listMainTaskEvent.ForEach(p => p.m_Handle += MainTaskTouch);

        m_listEvent.ForEach(p => p.m_Handle += TouchInfo);
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }
   
    void OnEnable()
    {
        TaskData.Instance.m_MainReload = false;
        TaskData.Instance.m_MainReload = false;
 
    }
    void Update()
    {
        if (TaskData.Instance.m_MainReload || TaskData.Instance.m_SideReload || TaskData.Instance.m_DailyQuestIsRefresh)
        {
            ShowLeftButton();
        }

        if (TaskData.Instance.m_MainReload && _touchIndex == 0)
        {
            TaskData.Instance.m_MainReload = false;
            TidyMainTaskInfo();
        }

       
        if (TaskData.Instance.m_SideReload && _touchIndex == 1)
        {
            TaskData.Instance.m_SideReload = false;
            TidySideTaskInfo();
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
            TaskData.Instance.GetQuestAward(_listTaskInfo[0]._TaskId);
        }
    }
    void TouchInfo(int index)
    {
        if (_touchIndex != index)
        {
            _QuestButtonDic[index].GetComponent<TaskButtonItemController>().m_Sprite_1.SetActive(true);
            _QuestButtonDic[index].GetComponent<TaskButtonItemController>().m_Sprite_0.SetActive(false);
            if (_QuestButtonDic.ContainsKey(_touchIndex))
            {
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Sprite_1.SetActive(false);
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Sprite_0.SetActive(true);
            }
            _touchIndex = index;
            if (index == 0)
            {
                m_MainQuestObj.SetActive(true);
                m_OtherQuestObj.SetActive(false);
            }
            else if (index == 1)
            {
                m_MainQuestObj.SetActive(false);
                m_OtherQuestObj.SetActive(true);
            }
            else
            {
                m_MainQuestObj.SetActive(false);
                m_OtherQuestObj.SetActive(false);
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
                        m_labOtherQuestTitle.text = MyColorData.getColorString(1, "盟务");
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void ShowLeftButton()
    {
        _listTaskButton.Clear();
        for (int i = 0; i < 2; i++)
        {
            if (!_QuestButtonDic.ContainsKey(i))
            {
                if (GetWetherContainQuest(i))
                {
                    TaskType type = new TaskType();
                    type._type = i;
                    if (i == 0)
                    {
                        type._Icon = "main";
                    }
                    else if (i == 1)
                    {
                        type._Icon = "mengwu";
                    }
                 
                   
                    _listTaskButton.Add(type);
                    if (_listTaskButton.Count == 1)
                    {
                        if (TaskData.Instance.m_ShowType == 0)
                        {
                            _touchIndex = i;
                        }
                        else
                        {
                            _touchIndex = TaskData.Instance.m_ShowType;
                        }
                    }
                }
            }
            else
            {
                if (!GetWetherContainQuest(i))
                {
                    Destroy(_QuestButtonDic[i]);
                    _QuestButtonDic.Remove(i);
                }
            }
        }

        //foreach (KeyValuePair<int, GameObject> item in _QuestButtonDic)
        //{
        //  item.Value.GetComponent<TaskButtonItemController>().m_TanHao.SetActive(IsProgressDone(item.Key));
        //}
        Createbutton();
    }

    void Createbutton()
    {
        index_button = 0;
        int size = _listTaskButton.Count;
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_BUTTON_ITEM), ResourcesButtonLoad_CallBack);
        }
    }

   private bool GetWetherContainQuest(int type)
    {
        if (type < 2)
        {
            foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
            {
                if (item.Value.type == type)
                {
                    if (FunctionOpenTemp.GetWhetherContainID(107) && type > 0)
                    {
                        return true;
                    }
                    else if (type == 0)
                    {
                        return true;
                    }
                }
            }
        }
       
        return false;
    }
    void ShowInfo()
    {
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
                    m_labOtherQuestTitle.text = MyColorData.getColorString(1, "盟务");
                }
              break;
     
            default:
                break;
        }

        ShowLeftButton();
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
                tni._Des = item.Value.brief;
                tni._Target = item.Value.desc;

                tni._Progress = item.Value.progress;
                tni._npcIcon = item.Value.icon;
                List<FunctionWindowsCreateManagerment.RewardInfo> list = new List<FunctionWindowsCreateManagerment.RewardInfo>();

                list = FunctionWindowsCreateManagerment.GetRewardInfo(item.Value.award);
                if (list != null)
                {
                    int reward_count = list.Count;
                    for (int j = 0; j < reward_count; j++)
                    {
                        for (int i = 0; i < reward_count - 1 - j; i++)
                        {
                            if (list[i].type > list[i + 1].type)
                            {
                                FunctionWindowsCreateManagerment.RewardInfo t = new FunctionWindowsCreateManagerment.RewardInfo();

                                t = list[i];

                                list[i] = list[i + 1];

                                list[i + 1] = t;
                            }
                        }
                    }
                    tni._listReward = list;
                }
                _listTaskInfo.Add(tni);
                
            }
        }

        if (_listTaskInfo.Count > 0)
        {
            showMainTaskInfo();
        }
    }

    void showMainTaskInfo()
    {
        m_listMainTaskEvent[0].gameObject.SetActive(_listTaskInfo[0]._Progress >= 0);
        m_listMainTaskEvent[1].gameObject.SetActive(_listTaskInfo[0]._Progress < 0);
        m_labMainQuestName.text = _listTaskInfo[0]._Name;
        m_labMainQuestDes.text = _listTaskInfo[0]._Des;
        m_labMainQuestTarget.text = _listTaskInfo[0]._Target;// + MyColorData.getColorString(4,
                   
        m_SpriteMainQuest.spriteName = _listTaskInfo[0]._npcIcon;
        int size_a = m_RewardParent.transform.childCount;
        if (size_a > 0)
        {
            for (int i = 0; i < size_a; i++)
            {
                Destroy(m_RewardParent.transform.GetChild(i).gameObject);
            }
        }
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YINDAO_DRAMA_AVATAR_PREFIX) + _listTaskInfo[0]._npcIcon
                               , ResourceLoadCallback);

        if (_listTaskInfo[0]._listReward != null)
        {
            RewardItemCreate();
        }
        else
        {
            m_MainQuestObj.SetActive(true);
        }
    }
    public void ResourceLoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        Texture temptext = (Texture)p_object;

        m_TextureIcon.mainTexture = temptext;
        m_TextureIcon.gameObject.SetActive(true);
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
    private bool GetContainSideTask()
    {
        foreach (KeyValuePair<int, ZhuXianTemp> item in TaskData.Instance.m_TaskInfoDic)
        {
            if (item.Value.type == 1)
            {
                return true;
            }
        }
        return false;
    }

    
    int index_daily = 0;
   
  
   
    void OtherTaskShow()
    {
        //_index_OtherNum = 0;
        //int size = m_ItemParent.transform.childCount;
        //for (int i = 0; i < size; i++)
        //{
        //    Destroy(m_ItemParent.transform.GetChild(i).gameObject);
        //}

        int size_a = _listTaskInfo.Count;

        if (_SideItemDic.Count > 0)
        {
            for (int i = 0; i < size_a; i++)
            {
                if (_SideItemDic.ContainsKey(_listTaskInfo[i]._TaskId))
                {
                    _SideItemDic[_listTaskInfo[i]._TaskId].transform.localPosition = new Vector3(0,-1*i*m_ItemParent.cellHeight,0);
                    _SideItemDic[_listTaskInfo[i]._TaskId].GetComponent<TaskScrollViewItemAmend>()
                                           .ShowTaskInfo(_listTaskInfo[_index_OtherNum], ShowReward);
                }
                else
                {
                    CreateScrollViewItem();
                }
            }
        }
        else
        {
            if (size_a > 0)
            {
                for (int i = 0; i < size_a; i++)
                {
                    CreateScrollViewItem();
                }
            }
            else
            {
                Destroy(_QuestButtonDic[_touchIndex]);
                TouchInfo(0);
                m_OtherQuestObj.SetActive(false);
                TidyMainTaskInfo();
            }
        }
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
            iconSampleObject.transform.localScale = Vector3.one * 0.8f;
            if (index_Reward < _listTaskInfo[0]._listReward.Count - 1)
            {
                index_Reward++;
            }
            else
            {
                m_MainQuestObj.SetActive(true);
                m_RewardParent.repositionNow = true;
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
            if (!_SideItemDic.ContainsKey(_listTaskInfo[_index_OtherNum]._TaskId))
            {
                _SideItemDic.Add(_listTaskInfo[_index_OtherNum]._TaskId, tempObject);
            }

            if (_index_OtherNum < _listTaskInfo.Count - 1)
            {
                _index_OtherNum++;
            }
            else
            {
                m_OtherQuestObj.SetActive(true);
                m_ItemParent.transform.parent.GetComponent<UIScrollView>().UpdatePosition();
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
            if (m_isFinishCurrent)
            {
                return;
            }
            m_isFinishCurrent = true;
            overMission(taskInfo._TaskId);
            //m_ObjFinish.SetActive(true);
            //StartCoroutine(WaitSecond(taskInfo, obj));
        }
        else 
        {
          
            if (RenWuTemplate.GetWetherContainId(taskInfo._TaskId) && !FunctionOpenTemp.GetWhetherContainID(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID) && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != -1 && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900002 && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900001)
            {
                ClientMain.m_UITextManager.createText(MyColorData.getColorString(1, FunctionOpenTemp.GetTemplateById(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID).m_sNotOpenTips));
            }
            else
            {
                MainCityUI.TryRemoveFromObjectList(m_MainParent);
                MainCityUI.m_MainCityUI.m_MainCityUILT.ClickTasID(taskInfo._TaskId);
                Destroy(m_MainParent);
                //m_MainParent.SetActive(false);

            }
        }
    }

    void overMission(int id)
    {
        FunctionWindowsCreateManagerment.ShowTaskAward(id); 
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
        }
        return false;
    }
    void OnDisable()
    {
        TaskData.Instance.m_ShowType = 0;
    }

    void OnDestroy()
    {
        m_TaskLayerM = null; ;
    }

    int index_button = 0;
    public void ResourcesButtonLoad_CallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_ButtonParent != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_ButtonParent.transform;
            tempObject.name = index_button.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.GetComponent<TaskButtonItemController>().m_TaskButton.ShowInfo(_listTaskButton[index_button],TouchInfo);
            _QuestButtonDic.Add(_listTaskButton[index_button]._type, tempObject);
            if (index_button < _listTaskButton.Count - 1)
            {
                index_button++;
            }
            else
            {
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Sprite_1.SetActive(true);
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Sprite_0.SetActive(false);
                foreach (KeyValuePair<int, GameObject> item in _QuestButtonDic)
                {
                   item.Value.GetComponent<TaskButtonItemController>().m_TanHao.SetActive(IsProgressDone(item.Key));
                }
            }
            m_ButtonParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }


    public void LocalSideFresh(int id)
    {
        if (_SideItemDic.ContainsKey(id))
        {
            Destroy(_SideItemDic[id]);
            _SideItemDic.Remove(id);
        }
        m_ItemParent.repositionNow = true;
        if (_SideItemDic.Count == 0)
        {
            TidySideTaskInfo();
        }
    }
   
}
