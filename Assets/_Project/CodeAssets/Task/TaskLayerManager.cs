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
    public List<EventIndexHandle> m_listEventVitality;
    public List<EventIndexHandle> m_listMainTaskEvent;
    public List<VitalityButtonEffectManangerment> m_listVitalityInfo;
    private Dictionary<int, GameObject> _QuestButtonDic = new Dictionary<int, GameObject>();
    public UIGrid m_RewardParent;
    public UIGrid m_ItemParent;
    public UIGrid m_ButtonParent;
    public GameObject m_Durable_UI;
    //public List<GameObject> m_listTanHao;
    public UILabel m_labTimeDown;
    public UILabel m_labVitalityCurrent;
    public UILabel m_labVitalityWeekly;
    public UIProgressBar m_UIProgressDaily;
    public ScaleEffectController m_SEC;
    public UILabel m_labMainQuestName;
    public UILabel m_labOtherQuestTitle;
    public UILabel m_labMainQuestDes;
    public UILabel m_labMainQuestTarget;
    public UISprite m_SpriteMainQuest;

    public GameObject m_MainQuestObj;
    public GameObject m_OtherQuestObj;
    public GameObject m_MainParent;
    public GameObject m_DailyParent;
    public GameObject m_ObjFinish;
    public GameObject m_RewardPanel;

    public GameObject m_VitalityRewardGet;
    public GameObject m_VitalityRewardShow;
    public UIGrid m_VitalityRewardGetParent;
    public UIGrid m_VitalityRewardShowParent;
    public UITexture m_TextureIcon;
    private GameObject TouchedAhead;
    private int _index_OtherNum = 0;
    public GameObject m_DailyQuestObj;

    public UILabel m_labVitalityRewardShow;
    public UILabel m_labVitalityRewardGet;
    public GameObject m_ObjTopLeft;
    public bool m_isDailyVitilityFresh = false;
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

    private List<FunctionWindowsCreateManagerment.RewardInfo> _listVitalityReward = new List<FunctionWindowsCreateManagerment.RewardInfo>();
   
    void Start()
    {
        m_TaskLayerM = this;
        _touchIndex = TaskData.Instance.m_ShowType;
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "任务", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_listMainTaskEvent.ForEach(p => p.m_Handle += MainTaskTouch);
        m_listEvent.ForEach(p => p.m_Handle += TouchInfo);
        m_SEC.OpenCompleteDelegate += ShowInfo;
        m_listEventVitality.ForEach(p => p.m_Handle += VitalityGet);
        // m_listVitalityInfo.ForEach(p => p.OnNormalPress += NormalTouch);
         m_listVitalityInfo.ForEach(p => p.m_TouchLQEvent.m_Handle += NormalTouch);
        m_listVitalityInfo.ForEach(p => p.OnLongPress += LongTouch);
        m_listVitalityInfo.ForEach(p => p.OnLongPressFinish += LongTouchFinish);
    }
    void VitalityGet(int index)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        ErrorMessage tempRequest = new ErrorMessage();
        tempRequest.errorCode = _Vitality_index + 1;

        //_Vitality_index
        t_qx.Serialize(t_tream, tempRequest);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.dailyTask_get_huoYue_award_req, ref t_protof);
        TaskData.Instance.m_VitalityShowInfo._listawardStatus[_Vitality_index] = 1;
        FreshVitality();
        m_VitalityRewardGet.SetActive(false);
    }
    void OnEnable()
    {
        TaskData.Instance.m_MainReload = false;
        TaskData.Instance.m_MainReload = false;
        TaskData.Instance.m_DailyQuestIsRefresh = false;
    }
    void Update()
    {
        //if(TaskData.Instance.m_isDailyTimeDown)
        //{
        //    m_labTimeDown.text = MyColorData.getColorString(4, TimeHelper.GetUniformedTimeString(TaskData.Instance.m_RemainTime)) + "后刷新";
        //}
        if (m_isDailyVitilityFresh)
        {
            m_isDailyVitilityFresh = false;
            FreshVitality();
        }

        if (TaskData.Instance.m_MainReload || TaskData.Instance.m_SideReload || TaskData.Instance.m_DailyQuestIsRefresh)
        {
            ShowLeftButton();
        }

        //if (_touchIndex == 0)
        //{
        //    TaskData.Instance.m_SideReload = false;
        //    TaskData.Instance.m_DailyQuestIsRefresh = false;
        //}
        //else if (_touchIndex == 1)
        //{
        //    TaskData.Instance.m_MainReload = false;
        //    TaskData.Instance.m_DailyQuestIsRefresh = false;
        //}
        //else if (_touchIndex == 2)
        //{
        //    TaskData.Instance.m_MainReload = false;
        //    TaskData.Instance.m_SideReload = false;
        //}
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
        if (TaskData.Instance.m_DailyQuestIsRefresh && _touchIndex == 2)
        {
            TaskData.Instance.m_DailyQuestIsRefresh = false;
            TidyDailyTaskInfo();
        }

        if (TaskData.Instance.m_VitaRefresh)
        {
            TaskData.Instance.m_VitaRefresh = false;
            FreshVitality();
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
           
            _QuestButtonDic[index].GetComponent<TaskButtonItemController>().m_TaskButtonEffect.ButtonState(true);
            _QuestButtonDic[index].GetComponent<TaskButtonItemController>().m_Guang.SetActive(true);
            _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_TaskButtonEffect.ButtonState(false);
            _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Guang.SetActive(false);
            _touchIndex = index;
            if (index == 0)
            {
                m_MainQuestObj.SetActive(true);
                m_OtherQuestObj.SetActive(false);
                m_DailyQuestObj.SetActive(false);
            }
            else if (index == 1)
            {
                m_MainQuestObj.SetActive(false);
                m_OtherQuestObj.SetActive(true);
                m_DailyQuestObj.SetActive(false);
            }
            else
            {
                m_MainQuestObj.SetActive(false);
                m_OtherQuestObj.SetActive(false);
                m_DailyQuestObj.SetActive(true);
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
                        m_labOtherQuestTitle.text = MyColorData.getColorString(1, "支线任务");
                    }
                    break;
                case 2:
                    {
                        TidyDailyTaskInfo();
                        m_labOtherQuestTitle.text = MyColorData.getColorString(1, "日常任务");
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
        for (int i = 0; i < 3; i++)
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
                        type._Icon = "side";
                    }
                    else
                    {
                        type._Icon = "daily";
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

        foreach (KeyValuePair<int, GameObject> item in _QuestButtonDic)
        {
            item.Value.GetComponent<TaskButtonItemController>().m_TanHao.SetActive(IsProgressDone(item.Key));
        }
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
        else
        {
            if (TaskData.Instance.m_TaskDailyDic.Count > 0 && FunctionOpenTemp.GetWhetherContainID(106))
            {
                return true;
            }
            else if(TaskData.Instance.m_TaskDailyDic.Count == 0 && FunctionOpenTemp.GetWhetherContainID(106))
            {
                return true;
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
                    m_labOtherQuestTitle.text = MyColorData.getColorString(1, "支线任务");
                }
              break;
            case 2:
                {
                    TidyDailyTaskInfo();
                    m_labOtherQuestTitle.text = MyColorData.getColorString(1, "日常任务");
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
                int reward_count = list.Count;
                for (int j = 0; j < reward_count; j++)
                {
                    for (int i = 0; i < reward_count - 1 - j; i++)
                    {
                        if (list[i].type > list[i + 1].type)
                        {
                            FunctionWindowsCreateManagerment.RewardInfo t = new FunctionWindowsCreateManagerment.RewardInfo();

                            t = list[i];

                            list[i] = list[i+1];

                            list[i+1] = t;
                        }
                    }
                }
                tni._listReward = list;

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
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YINDAO_DRAMA_AVATAR_PREFIX) + _listTaskInfo[0]._npcIcon,
                                ResourceLoadCallback);
        RewardItemCreate();
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
        if (_listTaskInfo.Count > 0)
        {
            m_LabelNullSignal.gameObject.SetActive(false);
        }
        FreshVitality();

        if (TaskData.Instance.m_VitalityShowInfo._todaylHuoYue > 0 && TaskData.Instance.m_VitalityShowInfo._todaylHuoYue <= 100)
        {
            m_MoveSprite.gameObject.SetActive(true);
            m_MoveSprite.transform.localPosition = new Vector3(-390 + GetLengthMove(float.Parse(TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString()) / HuoYueTempTemplate.GetHuoYueTempById(5).needNum), 164, 0);
        }
        else
        {
            m_MoveSprite.gameObject.SetActive(false);
        }

       
        m_UIProgressDaily.value = float.Parse(TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString()) / HuoYueTempTemplate.GetHuoYueTempById(5).needNum;
        m_labVitalityCurrent.text = TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString();
        m_labVitalityWeekly.text = TaskData.Instance.m_VitalityShowInfo._weekHuoYue.ToString();
        DailyTaskShow();
    }

    int index_daily = 0;
    void DailyTaskShow()
    {
        index_daily = 0;
        int size = m_DailyParent.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(m_DailyParent.transform.GetChild(i).gameObject);
        }

        int size_a = _listTaskInfo.Count;

        for (int i = 0; i < size_a; i++)
        {
            CreateDailyScrollViewItem();
        }
        if (size_a == 0)
        {
            m_DailyQuestObj.SetActive(true);
            m_LabelNullSignal.gameObject.SetActive(true);
        }
    }
    void CreateDailyScrollViewItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_SCROLL_VIEW_ITEM_AMEND), ResourcesDailyLoadCallBack);
    }

    public void ResourcesDailyLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {

        if (m_DailyParent != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_DailyParent.transform;
            tempObject.name = index_Num.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.GetComponent<TaskScrollViewItemAmend>().ShowTaskInfo(_listTaskInfo[index_daily], ShowReward);

            if (index_daily < _listTaskInfo.Count - 1)
            {
                index_daily++;
            }
            else
            {
                m_DailyQuestObj.SetActive(true);
                m_DailyParent.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
            }
            m_DailyParent.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
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
           
            if (_listTaskInfo[0]._listReward[index_Reward].type == 0)
            {
                iconSampleObject.transform.localScale = Vector3.one * 0.8f;
            }
            else
            {
                iconSampleObject.transform.localScale = Vector3.one;
            }

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
            else
            {
                m_OtherQuestObj.SetActive(true);
                m_ItemParent.transform.parent.GetComponent<UIScrollView>().verticalScrollBar.value = 0;
                m_ItemParent.transform.parent.GetComponent<UIScrollView>().UpdatePosition();
                m_ItemParent.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
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
            if (TaskData.Instance.m_TaskDailyDic.ContainsKey(taskInfo._TaskId))
            {
                CleartVitalityEffect();
            }
            overMission(taskInfo._TaskId);
            //m_ObjFinish.SetActive(true);
            //StartCoroutine(WaitSecond(taskInfo, obj));
        }
        else
        {
            if (RenWuTemplate.GetWetherContainId(taskInfo._TaskId) && !FunctionOpenTemp.GetWhetherContainID(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID) && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != -1 && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900002 && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900001)
            {
                ClientMain.m_UITextManager.createText(MyColorData.getColorString(1, FunctionOpenTemp.GetTemplateById(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID).m_sNotOpenTips));
              //  EquipSuoData.ShowSignal(null, FunctionOpenTemp.GetTemplateById(104).m_sNotOpenTips);
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
        TaskSignalInfoShow.m_TaskId = id;
        if(TaskSignalInfoShow.m_TaskSignal == null)
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_EFFECT), RewardCallback);
    }

    public void RewardCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        UIYindao.m_UIYindao.CloseUI();
    }
    IEnumerator WaitSecond(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);
        m_ObjFinish.SetActive(false);
        m_RewardPanel.SetActive(true);
        for (int i = 0; i < 7; i++)
        {
           UI3DEffectTool.ClearUIFx(m_listVitalityInfo[i].gameObject);
        }
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
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_TaskButtonEffect.ButtonState(true);
                _QuestButtonDic[_touchIndex].GetComponent<TaskButtonItemController>().m_Guang.SetActive(true);
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

    void Vitality_RewardGet()
    {
        index_Vitality_Reward = 0;
        int num =  _listVitalityReward.Count;
        m_VitalityRewardGetParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(num, 108),0,0);
        for (int i = 0; i < num; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadReward_GetCallBack);
        }
    }

    void Vitality_RewardShow()
    {
        index_Vitality_Reward = 0;
        int num = _listVitalityReward.Count;
 
        m_VitalityRewardShowParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(num, 108), 0, 0);
        for (int i = 0; i < num; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadReward_ShowCallBack);
        }
    }
    int index_Vitality_Reward = 0;
    private void OnIconSampleLoadReward_GetCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_VitalityRewardGetParent != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_VitalityRewardGetParent.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID(_listVitalityReward[index_Vitality_Reward].icon, _listVitalityReward[index_Vitality_Reward].count.ToString(), 4);
            iconSampleManager.SetIconPopText(_listVitalityReward[index_Vitality_Reward].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listVitalityReward[index_Vitality_Reward].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listVitalityReward[index_Vitality_Reward].icon).descId));

          
            iconSampleObject.transform.localScale = Vector3.one;

            if (index_Vitality_Reward < _listVitalityReward.Count - 1)
            {
                index_Vitality_Reward++;
            }
            else
            {
                m_VitalityRewardGet.SetActive(true);
            }
            m_VitalityRewardGetParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    private void OnIconSampleLoadReward_ShowCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_VitalityRewardShowParent != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_VitalityRewardShowParent.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID(_listVitalityReward[index_Vitality_Reward].icon, _listVitalityReward[index_Vitality_Reward].count.ToString(), 4);
            iconSampleManager.SetIconPopText(_listVitalityReward[index_Vitality_Reward].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listVitalityReward[index_Vitality_Reward].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listVitalityReward[index_Vitality_Reward].icon).descId));

            iconSampleObject.transform.localScale = Vector3.one;


            if (index_Vitality_Reward < _listVitalityReward.Count - 1)
            {
                index_Vitality_Reward++;
            }
            else
            {
                m_VitalityRewardShow.SetActive(true);
            }

            m_VitalityRewardShowParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    private int _Vitality_index = 0;


    public void FreshVitality()
    {
 
        for (int i = 0; i < 7; i++)
        {
            m_listVitalityInfo[i].m_LabTitle.text = HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum.ToString();
            if (TaskData.Instance.m_VitalityShowInfo._todaylHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum && TaskData.Instance.m_VitalityShowInfo._listawardStatus[i] != 1)
            {
                m_listVitalityInfo[i].m_TouchLQEvent.gameObject.SetActive(true);
                m_listVitalityInfo[i].m_FirstObj.SetActive(true);
                m_listVitalityInfo[i].m_SecondObj.SetActive(false);
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_listVitalityInfo[i].gameObject, EffectIdTemplate.GetPathByeffectId(600154), null);
            }
            else if (TaskData.Instance.m_VitalityShowInfo._listawardStatus[i] != 1)
            {
                m_listVitalityInfo[i].m_TouchLQEvent.gameObject.SetActive(false);
                m_listVitalityInfo[i].m_FirstObj.SetActive(true);
                m_listVitalityInfo[i].m_SecondObj.SetActive(false);
            }
            else
            {
                m_listVitalityInfo[i].m_TouchLQEvent.gameObject.SetActive(false);
                m_listVitalityInfo[i].m_FirstObj.SetActive(false);
                m_listVitalityInfo[i].m_SecondObj.SetActive(true);
                UI3DEffectTool.ClearUIFx(m_listVitalityInfo[i].gameObject);
            }
        }
    }
    void NormalTouch(int index)
    {
        _Vitality_index = index;
        int count0 = m_VitalityRewardGetParent.transform.childCount;
        if (count0 > 0)
        {
            for (int i = 0; i < count0; i++)
            {
                Destroy(m_VitalityRewardGetParent.transform.GetChild(i).gameObject);
            }
        }
        CleartVitalityEffect();

        if (TaskData.Instance.m_VitalityShowInfo._todaylHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum && TaskData.Instance.m_VitalityShowInfo._listawardStatus[index] != 1)
        {
            m_labVitalityRewardGet.text = "[今日]活跃度达到" + HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum.ToString();
            _listVitalityReward.Clear();
            _listVitalityReward = FunctionWindowsCreateManagerment.GetRewardInfo(HuoYueTempTemplate.GetHuoYueTempById(index + 1).award);
            Vitality_RewardGet();
        }
    }

    void CleartVitalityEffect()
    {
        for (int i = 0; i < 7; i++)
        {
            UI3DEffectTool.ClearUIFx(m_listVitalityInfo[i].gameObject);
        }
    }
    private int index_Touch_Save = 0;
    void LongTouch(GameObject obj)
    {
        for (int i = 0; i < 7; i++)
        {
            m_listVitalityInfo[i].m_LabTitle.text = HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum.ToString();
            int size = TaskData.Instance.m_VitalityShowInfo._listawardStatus.Count;
            if (TaskData.Instance.m_VitalityShowInfo._todaylHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum && TaskData.Instance.m_VitalityShowInfo._listawardStatus[i] != 1)
            {
                UI3DEffectTool.ClearUIFx(m_listVitalityInfo[i].gameObject);
            }

        }
        m_listVitalityInfo[int.Parse(obj.name)].GetComponent<ButtonScaleManagerment>().ButtonsControl(true, 0.9f);
        {
            m_labVitalityRewardShow.text = "活跃度" + HuoYueTempTemplate.GetHuoYueTempById(int.Parse(obj.name) + 1).needNum.ToString() + "奖励";

            _listVitalityReward.Clear();
            _listVitalityReward = FunctionWindowsCreateManagerment.GetRewardInfo(HuoYueTempTemplate.GetHuoYueTempById(int.Parse(obj.name) + 1).award);

            Vitality_RewardShow();
        }
    }

    void LongTouchFinish(GameObject obj)
    {
        FreshVitality();
        int count2 = m_VitalityRewardShowParent.transform.childCount;
 
        if (count2 > 0)
        {
            for (int i = 0; i < count2; i++)
            {
                Destroy(m_VitalityRewardShowParent.transform.GetChild(i).gameObject);
            }
        }
        m_listVitalityInfo[int.Parse(obj.name)].GetComponent<ButtonScaleManagerment>().ButtonsControl(false, 0.9f);
        m_VitalityRewardShow.SetActive(false);
    }


    private float GetLengthMove(float tt)
    {
        return m_ForeSprite.width*tt;
    }
}
