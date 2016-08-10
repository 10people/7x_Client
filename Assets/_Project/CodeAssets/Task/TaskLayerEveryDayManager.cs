using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class TaskLayerEveryDayManager : MonoBehaviour, UI2DEventListener
{
    public static TaskLayerEveryDayManager m_TaskLayerM;
    public UISprite m_MoveSprite;
    public UILabel m_LabelNullSignal;
    public UISprite m_ForeSprite;
    public List<EventIndexHandle> m_listEventVitality;
    public List<VitalityButtonEffectManangerment> m_listVitalityInfo;
    private Dictionary<int, GameObject> _DailyObjInfo = new Dictionary<int, GameObject>();

    public GameObject m_Durable_UI;
    //public List<GameObject> m_listTanHao;
    public UILabel m_labTimeDown;
    public UILabel m_labVitalityCurrent;
    public UILabel m_labVitalityWeekly;
    public UIProgressBar m_UIProgressDaily;
    public ScaleEffectController m_SEC;

    public GameObject m_MainParent;
    public GameObject m_DailyParent;
    //    public GameObject m_ObjFinish;
    //    public GameObject m_RewardPanel;

    public GameObject m_VitalityRewardGet;
    public GameObject m_VitalityRewardShow;
    public UIGrid m_VitalityRewardGetParent;
    public UIGrid m_VitalityRewardShowParent;
    public UITexture m_TextureIcon;

    public EventIndexHandle m_EventHide;
    private GameObject TouchedAhead;
    private int _index_OtherNum = 0;
    public GameObject m_DailyQuestObj;

    public UILabel m_labVitalityRewardShow;
    public UILabel m_labVitalityRewardGet;
    public GameObject m_ObjTopLeft;
    public bool m_isDailyVitilityFresh = false;
    public GameObject m_panel;

    public struct TaskType
    {
        public int _type;
        public string _Icon;
    };

    private List<TaskLayerManager.TaskNeedInfo> _listTaskInfo = new List<TaskLayerManager.TaskNeedInfo>();
    private int _tempNum = 0;
    private int _touchIndex = 0;

    private List<FunctionWindowsCreateManagerment.RewardInfo> _listVitalityReward = new List<FunctionWindowsCreateManagerment.RewardInfo>();

    void Start()
    {
        m_TaskLayerM = this;
        _touchIndex = 2;
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "日常", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_EventHide.m_Handle += Hide;
        m_SEC.OpenCompleteDelegate += ShowInfo;
        m_listEventVitality.ForEach(p => p.m_Handle += VitalityGet);
        // m_listVitalityInfo.ForEach(p => p.OnNormalPress += NormalTouch);
        m_listVitalityInfo.ForEach(p => p.m_TouchLQEvent.m_Handle += NormalTouch);
        m_listVitalityInfo.ForEach(p => p.m_PressLong += LongTouch);
        m_listVitalityInfo.ForEach(p => p.m_PressUp += LongTouchFinish);

        MainSimpleInfoReq mainSimpleReq = new MainSimpleInfoReq();
        mainSimpleReq.functionType = 8;
        QXComData.SendQxProtoMessage(mainSimpleReq, ProtoIndexes.C_MAIN_SIMPLE_INFO_REQ, ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP.ToString());

        if (FreshGuide.Instance().IsActive(100200) && TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
        {
            //if(!UIYindao.m_UIYindao.m_isOpenYindao)
            {
                TaskData.Instance.m_iCurMissionIndex = 100200;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 2;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }
        else if (FreshGuide.Instance().IsActive(100220) && TaskData.Instance.m_TaskInfoDic[100220].progress >= 0)
        {
            //if(!UIYindao.m_UIYindao.m_isOpenYindao)
            {
                TaskData.Instance.m_iCurMissionIndex = 100220;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 1;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }
        else if (FreshGuide.Instance().IsActive(100315) && TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
        {
            //if(!UIYindao.m_UIYindao.m_isOpenYindao)
            {
                TaskData.Instance.m_iCurMissionIndex = 100315;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 2;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }
        FunctionOpenTemp.GetTemplateById(106).m_objPanel = m_panel;
    }
    void Hide(int index)
    {
        _LoneTouchOn = false;
        _Name = "";
        FreshVitality();
        int count2 = m_VitalityRewardShowParent.transform.childCount;

        if (count2 > 0)
        {
            for (int i = 0; i < count2; i++)
            {
                Destroy(m_VitalityRewardShowParent.transform.GetChild(i).gameObject);
            }
        }

        m_VitalityRewardShow.SetActive(false);
    }

    public void OnUI2DShow()
    {
        if (FreshGuide.Instance().IsActive(100200) && TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
        {

        }
        else if (FreshGuide.Instance().IsActive(100220) && TaskData.Instance.m_TaskInfoDic[100220].progress >= 0)
        {

        }
        else if (FreshGuide.Instance().IsActive(100315) && TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
        {

        }
		else if(FreshGuide.Instance().IsActive(200020)&& TaskData.Instance.m_TaskInfoDic[200020].progress >= 0)
		{

		}
        else
        {
            UIYindao.m_UIYindao.CloseUI();
        }
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
    }
    void Update()
    {
        if (m_isDailyVitilityFresh)
        {
            m_isDailyVitilityFresh = false;
            FreshVitality();
        }
 
        if (TaskData.Instance.m_DailyQuestIsRefresh)
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
            else if (TaskData.Instance.m_TaskDailyDic.Count == 0 && FunctionOpenTemp.GetWhetherContainID(106))
            {
                return true;
            }
        }
        return false;
    }
    void ShowInfo()
    {
        TidyDailyTaskInfo();
    }


    public void ResourceLoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        Texture temptext = (Texture)p_object;

        m_TextureIcon.mainTexture = temptext;
        m_TextureIcon.gameObject.SetActive(true);
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
            TaskLayerManager.TaskNeedInfo tni = new TaskLayerManager.TaskNeedInfo();
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
			m_MoveSprite.transform.localPosition = new Vector3(-379 + GetLengthMove(float.Parse(TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString()) / HuoYueTempTemplate.GetHuoYueTempById(5).needNum), -237, 0);
        }
        else
        {
            m_MoveSprite.gameObject.SetActive(false);
        }


        m_UIProgressDaily.value = float.Parse(TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString()) / HuoYueTempTemplate.GetHuoYueTempById(5).needNum;
        m_labVitalityCurrent.text = TaskData.Instance.m_VitalityShowInfo._todaylHuoYue.ToString();
        m_labVitalityWeekly.text = TaskData.Instance.m_VitalityShowInfo._weekHuoYue.ToString() + "/";
        DailyTaskShow();
    }

    int index_daily = 0;
    void DailyTaskShow()
    {
       // _DailyObjInfo.Clear();
        //index_daily = 0;

        //int size = m_DailyParent.transform.childCount;
        //for (int i = 0; i < size; i++)
        //{
        //    Destroy(m_DailyParent.transform.GetChild(i).gameObject);
        //}
        int size_a = _listTaskInfo.Count;
        if (_DailyObjInfo.Count > 0)
        {
            for (int i = 0; i < size_a; i++)
            {
                if (_DailyObjInfo.ContainsKey(_listTaskInfo[i]._TaskId))
                {
                    _DailyObjInfo[_listTaskInfo[i]._TaskId].transform.localPosition = new Vector3(0, -1*i*m_DailyParent.GetComponent<UIGrid>().cellHeight, 0);
                    _DailyObjInfo[_listTaskInfo[i]._TaskId].GetComponent<TaskScrollViewItemAmendShort>()
                                                           .ShowTaskInfo(_listTaskInfo[i], ShowReward);
                }
                else
                {
                    CreateDailyScrollViewItem();
                }
            }
        }
        else
        {

         

            for (int i = 0; i < size_a; i++)
            {
                CreateDailyScrollViewItem();
            }
            if (size_a == 0)
            {
                EveryDayShowTime.m_isLoad0 = true;
                m_DailyQuestObj.SetActive(true);
                m_LabelNullSignal.gameObject.SetActive(true);
            }
        }
    }
    void CreateDailyScrollViewItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_SCROLL_VIEW_ITEM_AMEND) + "Short", ResourcesDailyLoadCallBack);
    }

    public void ResourcesDailyLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_DailyParent != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_DailyParent.transform;
//            tempObject.name = index_Num.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.GetComponent<TaskScrollViewItemAmendShort>().ShowTaskInfo(_listTaskInfo[index_daily], ShowReward);
            if (!_DailyObjInfo.ContainsKey(_listTaskInfo[index_daily]._TaskId))
            {
                _DailyObjInfo.Add(_listTaskInfo[index_daily]._TaskId, tempObject);
            }

            //if (_listTaskInfo[index_daily]._Progress < 0)
            //{
            //    EffectTool.OpenMultiUIEffect_ById(tempObject.GetComponent<TaskScrollViewItemAmend>().m_LingQu.gameObject, 223, 224, 225);
            //}

            if (index_daily < _listTaskInfo.Count - 1)
            {
                index_daily++;
            }
            else
            {
                m_DailyQuestObj.SetActive(true);
                m_DailyParent.transform.parent.GetComponent<UIScrollView>().UpdateScrollbars(true);
                EveryDayShowTime.m_isLoad0 = true;
            }
            m_DailyParent.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
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
        //        if (m_RewardParent != null)
        //        {
        //            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        //            iconSampleObject.SetActive(true);
        //            iconSampleObject.transform.parent = m_RewardParent.transform;
        //            iconSampleObject.transform.localPosition = Vector3.zero;
        //            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        //            iconSampleManager.SetIconByID(_listTaskInfo[0]._listReward[index_Reward].icon, _listTaskInfo[0]._listReward[index_Reward].count.ToString(),4);
        //            iconSampleManager.SetIconPopText(_listTaskInfo[0]._listReward[index_Reward].icon,
        //                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listTaskInfo[0]._listReward[index_Reward].icon).nameId),
        //                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listTaskInfo[0]._listReward[index_Reward].icon).descId));
        //            iconSampleObject.transform.localScale = Vector3.one * 0.8f;
        //            if (index_Reward < _listTaskInfo[0]._listReward.Count - 1)
        //            {
        //                index_Reward++;
        //            }
        //            else
        //            {
        //                m_MainQuestObj.SetActive(true);
        //                m_RewardParent.repositionNow = true;
        //            }
        //        }
        //        else
        //        {
        //            p_object = null;
        //        }
    }

    void CreateScrollViewItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_SCROLL_VIEW_ITEM_AMEND), ResourcesLoadCallBack);
    }

    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        //        if (m_ItemParent != null)
        //        {
        //            GameObject tempObject = Instantiate(p_object) as GameObject;
        //            tempObject.transform.parent = m_ItemParent.transform;
        //            tempObject.name = index_Num.ToString();
        //            tempObject.transform.localPosition = Vector3.zero;
        //            tempObject.transform.localScale = Vector3.one;
        //            tempObject.GetComponent<TaskScrollViewItemAmend>().ShowTaskInfo(_listTaskInfo[_index_OtherNum], ShowReward);
        //
        //            if (_index_OtherNum < _listTaskInfo.Count - 1)
        //            {
        //                _index_OtherNum++;
        //            }
        //        }
        //        else
        //        {
        //            p_object = null;
        //        }
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
            if (RenWuTemplate.GetWetherContainId(taskInfo._TaskId) &&
                !FunctionOpenTemp.GetWhetherContainID(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID)
                && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != -1
                && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900002
                && RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID != 900001)
            {
                ClientMain.m_UITextManager.createText(MyColorData.getColorString(1, FunctionOpenTemp.GetTemplateById(RenWuTemplate.GetRenWuById(taskInfo._TaskId).funcID).m_sNotOpenTips));
                //  EquipSuoData.ShowSignal(null, FunctionOpenTemp.GetTemplateById(104).m_sNotOpenTips);
            }
            else
            {
                {
                    MainCityUI.TryRemoveFromObjectList(m_MainParent);
                    Destroy(m_MainParent);
                }
                MainCityUI.m_MainCityUI.m_MainCityUILT.ClickTasID(taskInfo._TaskId);

                //m_MainParent.SetActive(false);

            }
        }
    }

    void overMission(int id)
    {
        FunctionWindowsCreateManagerment.ShowTaskAward(id);
    }

    public void RewardCallback(ref WWW p_www, string p_path, Object p_object)
    {
        if (TaskSignalInfoShow.m_TaskSignal == null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            UIYindao.m_UIYindao.CloseUI();
        }
        else
        {
            p_object = null;
        }
    }
    IEnumerator WaitSecond(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj)
    {
        yield return new WaitForSeconds(0.4f);
        //        m_ObjFinish.SetActive(false);
        //        m_RewardPanel.SetActive(true);
        for (int i = 0; i < 7; i++)
        {
            UI3DEffectTool.ClearUIFx(m_listVitalityInfo[i].gameObject);
        }
        //        m_RewardPanel.GetComponent<TaskRewardsShow>().Show(taskInfo, obj);
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
    private bool _CreatedReward = false;
    void Vitality_RewardGet()
    {
        index_Vitality_Reward = 0;
        int num = _listVitalityReward.Count;
        m_VitalityRewardGetParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(num, 108), 0, 0);
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


            iconSampleObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

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
        if (m_VitalityRewardShowParent != null && !_LoneTouchOn)
        {
            _LoneTouchOn = true;
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
                _CreatedReward = true;
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
        int i = 0;
        for (i = 0; i < 7; i++)
        {
            m_listVitalityInfo[i].m_LabTitle.text = HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum.ToString();
            //            if (i < 5)
            //            {
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
            //            }
        }
        i = 5;
        if (TaskData.Instance.m_VitalityShowInfo._listawardStatus[5] != 1)
        {
            i = 5;
            m_listVitalityInfo[6].gameObject.SetActive(false);
        }
        else
        {
            i = 6;
            m_listVitalityInfo[5].gameObject.SetActive(false);
            m_listVitalityInfo[6].gameObject.SetActive(true);
        }
        m_labVitalityWeekly.gameObject.transform.parent = m_listVitalityInfo[i].gameObject.transform;
        if (TaskData.Instance.m_VitalityShowInfo._weekHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(i + 1).needNum && TaskData.Instance.m_VitalityShowInfo._listawardStatus[i] != 1)
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
    private int _indexSave = -1;
    void NormalTouch(int index)
    {
        if (_indexSave != index)
        {
            _indexSave = index;
            m_listVitalityInfo[index].m_TouchLQEvent.GetComponent<Collider>().enabled = false;
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

            if (index > 4
                && TaskData.Instance.m_VitalityShowInfo._weekHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum
                    && TaskData.Instance.m_VitalityShowInfo._listawardStatus[index] != 1)
            {
                m_labVitalityRewardGet.text = "[本周]活跃度达到" + HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum.ToString();
                _listVitalityReward.Clear();
                _listVitalityReward = FunctionWindowsCreateManagerment.GetRewardInfo(HuoYueTempTemplate.GetHuoYueTempById(index + 1).award);
                Vitality_RewardGet();
            }
            else if (TaskData.Instance.m_VitalityShowInfo._todaylHuoYue >= HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum && TaskData.Instance.m_VitalityShowInfo._listawardStatus[index] != 1)
            {
                m_labVitalityRewardGet.text = "[今日]活跃度达到" + HuoYueTempTemplate.GetHuoYueTempById(index + 1).needNum.ToString();
                _listVitalityReward.Clear();
                _listVitalityReward = FunctionWindowsCreateManagerment.GetRewardInfo(HuoYueTempTemplate.GetHuoYueTempById(index + 1).award);
                Vitality_RewardGet();
            }
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
    private string _Name = "";
    private bool _LoneTouchOn = false;

    void LongTouch(GameObject obj)
    {
        if (UICamera.selectedObject != null && !string.IsNullOrEmpty(_Name))
        {
            if (!UICamera.selectedObject.name.Equals(_Name))
            {
                LongTouchFinish(obj);
            }
        }

        if (Input.touchCount > 1)
        {
            return;
        }
        if (string.IsNullOrEmpty(_Name))
        {
            _Name = obj.name;
        }

        if (_Name == obj.name)
        {
            TouchOn(obj);
        }
    }
    void TouchOn(GameObject obj)
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

            {

                Vitality_RewardShow();
            }
        }

    }

    void LongTouchFinish(GameObject obj)
    {
        _LoneTouchOn = false;
        _Name = "";
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
        return m_ForeSprite.width * tt;
    }

    public void LocalFreshDaily(int id)
    {
        if (_DailyObjInfo.ContainsKey(id))
        {
          Destroy(_DailyObjInfo[id]);
          _DailyObjInfo.Remove(id);
          m_DailyParent.GetComponent<UIGrid>().repositionNow = true;
        }

        if (_DailyObjInfo.Count == 0)
        {
            TidyDailyTaskInfo();
        }
    }
}
