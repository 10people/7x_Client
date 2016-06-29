﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;
public class TaskSignalInfoShow : MonoBehaviour
{
    public static TaskSignalInfoShow m_TaskSignal;
    public List<EventHandler> m_ListEvent;
    private int taskId = 0;
    public GameObject m_grid;
    public GameObject m_ObjHidden;
    public UILabel m_labelTitle;
    public UILabel m_labObject;
    public static int m_TaskId = 0;
    public ScaleEffectController m_SEC;
    private string _rewardInfo = "";
    public GameObject m_ObjFinish;
    public UISprite m_SpriteIcon;
    private UICamera m_Camera = null;
    struct RewardInfo
    {
        public string type;
        public string count;
        public string icon;
    }
    private List<RewardInfo> listRewardInfo = new List<RewardInfo>();
    private  int _GuidanceId = -1;
    
    void Awake()
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
           _GuidanceId = UIYindao.m_UIYindao.m_iCurId;
 
            UIYindao.m_UIYindao.CloseUI();
        }
        if (MainCityUI.m_MainCityUI)
        {
            int size = MainCityUI.m_MainCityUI.m_WindowObjectList.Count;
            if (size > 0)
            {
                m_Camera = MainCityUI.m_MainCityUI.m_WindowObjectList[size - 1].GetComponentInChildren<UICamera>();
                if (m_Camera != null)
                    EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, true);
            }
            else
            {
                UI2DTool.Instance.AddTopUI(gameObject);
            }
        }

        //   UI2DTool.Instance.AddTopUI(gameObject);
    }
    void Start()
    {
        // UIYindao.m_UIYindao.CloseUI();
        _listObj.Clear();
        m_TaskSignal = this;
        m_ListEvent.ForEach(item => item.m_click_handler += GetAwards);
        CycleTween.StartCycleTween(m_labObject.gameObject, 1, 0.4f, 0.5f, sdssd);
        m_SEC.OpenCompleteDelegate += ChargeData;
    }
    void sdssd(float f)
    {
        m_labObject.color = new Color(m_labObject.color.r, m_labObject.color.g, m_labObject.color.b, f);
    }
    int index_Num = 0;

    private List<GameObject> _listObj = new List<GameObject>();
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_grid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_grid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            iconSampleObject.transform.localScale = Vector3.one * 0.8f;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(int.Parse(listRewardInfo[index_Num].icon), listRewardInfo[index_Num].count);
            iconSampleManager.SetIconPopText(int.Parse(listRewardInfo[index_Num].icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).descId));
            if (int.Parse(listRewardInfo[index_Num].icon) == 900006)
            {
                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, iconSampleObject, EffectIdTemplate.GetPathByeffectId(100112), null);
                _listObj.Add(iconSampleObject);
            }

            if (index_Num < listRewardInfo.Count - 1)
            {
                index_Num++;
            }
            else
            {
                m_ListEvent[0].GetComponent<Collider>().enabled = true;
            }
            m_grid.GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    void ChargeData()
    {
        if (TaskData.Instance.m_TaskInfoDic.ContainsKey(m_TaskId)
            && TaskData.Instance.m_TaskInfoDic[m_TaskId].type == 1)
        {
            m_SpriteIcon.spriteName = "side";
            ShowAwardInfo(TaskData.Instance.m_TaskInfoDic[m_TaskId]);
        }
        else if (TaskData.Instance.m_TaskDailyDic.ContainsKey(m_TaskId))
        {
            m_SpriteIcon.spriteName = "daily";
            ShowDailyAwardInfo(TaskData.Instance.m_TaskDailyDic[m_TaskId]);
        }
        else if (TaskData.Instance.m_TaskInfoDic.ContainsKey(m_TaskId))
        {
            m_SpriteIcon.spriteName = "main";
            ShowAwardInfo(TaskData.Instance.m_MainComplete);
        }
        else
        {
            UI3DEffectTool.ClearUIFx(m_ObjFinish);
            ClientMain.closePopUp();
            Destroy(gameObject);
        }
    }

    void ShowAwardInfo(ZhuXianTemp temp)
    {
        UI3DEffectTool.ClearUIFx(m_ObjFinish);
        m_ObjFinish.SetActive(false);
        m_ObjHidden.SetActive(true);
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_labelTitle.gameObject, EffectIdTemplate.GetPathByeffectId(620246), null);
        taskId = temp.id;
        _rewardInfo = temp.award;
        m_labelTitle.text = "完成任务：" + temp.doneTitle;
        listRewardInfo.Clear();
        if (!string.IsNullOrEmpty(_rewardInfo) && _rewardInfo != "0")
        {
            if (_rewardInfo.IndexOf('#') > -1)
            {
                string[] tempAwardList = _rewardInfo.Split('#');
                int size = tempAwardList.Length;
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
                string[] tempAwardItemInfo = _rewardInfo.Split(':');
                RewardInfo rinfo = new RewardInfo();
                rinfo.type = tempAwardItemInfo[0];
                rinfo.icon = tempAwardItemInfo[1];
                rinfo.count = tempAwardItemInfo[2];
                listRewardInfo.Add(rinfo);
            }
            int sizeAll = listRewardInfo.Count;
            m_grid.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(sizeAll, 112), 0, 0);
            for (int i = 0; i < sizeAll; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
        }
        else
        {
            UI3DEffectTool.ClearUIFx(m_ObjFinish);
            Destroy(gameObject);
        }
    }

    void ShowDailyAwardInfo(RenWuTemplate temp)
    {
        UI3DEffectTool.ClearUIFx(m_ObjFinish);
        m_ObjFinish.SetActive(false);
        m_ObjHidden.SetActive(true);
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_labelTitle.gameObject, EffectIdTemplate.GetPathByeffectId(620246), null);
        m_TaskSignal = this;
        taskId = temp.id;
        _rewardInfo = temp.jiangli;
        m_labelTitle.text = "完成任务：" + temp.doneTitle;
        listRewardInfo.Clear();
        if (!string.IsNullOrEmpty(_rewardInfo) && _rewardInfo != "0")
        {
            if (_rewardInfo.IndexOf('#') > -1)
            {
                string[] tempAwardList = _rewardInfo.Split('#');
                int size = tempAwardList.Length;

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
                string[] tempAwardItemInfo = _rewardInfo.Split(':');
                RewardInfo rinfo = new RewardInfo();
                rinfo.type = tempAwardItemInfo[0];
                rinfo.icon = tempAwardItemInfo[1];
                rinfo.count = tempAwardItemInfo[2];
                listRewardInfo.Add(rinfo);
            }
            int sizeAll = listRewardInfo.Count;
            m_grid.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(sizeAll, 112), 0, 0);
            for (int i = 0; i < sizeAll; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
        }
        else
        {
            UI3DEffectTool.ClearUIFx(m_ObjFinish);
            Destroy(gameObject);
        }
    }
    private bool _isCancelEffect = false;
    void GetAwards(GameObject obj)
    {
        if (obj.name.Equals("CancelEffect"))
        {
            _isCancelEffect = true;
            UI3DEffectTool.ClearUIFx(m_ObjFinish);
            ChargeData();
        }
        else
        {
            if (TaskData.Instance.m_TaskInfoDic.ContainsKey(m_TaskId))
            {
                TaskData.Instance.GetQuestAward(taskId);
            }
            else if (TaskData.Instance.m_TaskDailyDic.ContainsKey(m_TaskId))
            {
                TaskData.Instance.GetDailyQuestAward(taskId);
 
                if (TaskData.Instance.m_TaskDailyDic.ContainsKey(m_TaskId))
                {
                    TaskData.Instance.m_DailyLocalRefresh = true;
                    if (!string.IsNullOrEmpty(TaskData.Instance.m_TaskDailyDic[m_TaskId].award))
                    {
                        FunctionWindowsCreateManagerment.ShowRAwardInfo(TaskData.Instance.m_TaskDailyDic[m_TaskId].award);
                    }
                    TaskData.Instance.m_TaskDailyDic.Remove(m_TaskId);
                }
            }
            UI3DEffectTool.ClearUIFx(m_ObjFinish);
            Destroy(gameObject);
        }
    }
    void OnDisable()
    {
        if (m_Camera)
            EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, false);
        UI3DEffectTool.ClearUIFx(m_labelTitle.gameObject);
    }

    void OnDestroy()
    {
        if (TaskData.Instance.m_TaskRewardsGetID.ContainsKey(m_TaskId))
        {
            TaskData.Instance.m_TaskRewardsGetID.Remove(m_TaskId);
        }

        if (_GuidanceId > 0)
        {
            UIYindao.m_UIYindao.setOpenYindao(_GuidanceId);
            _GuidanceId = -1;
        }
        m_TaskSignal = null;
        if (TaskLayerManager.m_TaskLayerM)
            TaskLayerManager.m_TaskLayerM.m_isFinishCurrent = false;
    }
}
