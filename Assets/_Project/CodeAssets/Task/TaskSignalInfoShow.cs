using UnityEngine;
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
    public static int m_TaskId = 0;
    public ScaleEffectController m_SEC;
    private string _rewardInfo = "";
    public GameObject m_ObjFinish;

    struct RewardInfo
    {
        public string type;
        public string count;
        public string icon;
    }
    private List<RewardInfo> listRewardInfo = new List<RewardInfo>();
    void Start()
    {
        _listObj.Clear();
        m_TaskSignal = this;
        m_ListEvent.ForEach(item => item.m_handler += GetAwards);
        m_SEC.OpenCompleteDelegate += ShowInfo;
    }

    public void Update()
    {
        if (TaskData.Instance.m_TaskGetAwardComplete)
        {
            TaskData.Instance.m_TaskGetAwardComplete = false;
            Destroy(gameObject);
        }

    }
 
    int index_Num = 0;
    void ShowInfo()
    {
        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_ObjFinish, EffectIdTemplate.GetPathByeffectId(100180), null);
        m_ObjFinish.SetActive(true);
        StartCoroutine(WaitSecond());
    }

    private List<GameObject> _listObj = new List<GameObject>();
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_grid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_grid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            iconSampleObject.transform.localScale = Vector3.one*0.8f;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(int.Parse(listRewardInfo[index_Num].icon), listRewardInfo[index_Num].count);

			iconSampleManager.SetIconPopText(int.Parse(listRewardInfo[index_Num].icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_Num].icon)).descId));
            if (int.Parse(listRewardInfo[index_Num].icon) == 900006)
            {
                UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, iconSampleObject, EffectIdTemplate.GetPathByeffectId(100112), null);
                _listObj.Add(iconSampleObject);
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
    IEnumerator WaitSecond()
    {
        //   yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(0.8f);

        if (!_isCancelEffect)
        {
            // m_ListEvent[1].gameObject.SetActive(false);
            ChargeData();
        }
    }

    void ChargeData()
    {
        if (TaskData.Instance.m_TaskInfoDic.ContainsKey(m_TaskId) && TaskData.Instance.m_TaskInfoDic[m_TaskId].type == 1)
        {
            ShowAwardInfo(TaskData.Instance.m_TaskInfoDic[m_TaskId]);
        }
        else
        {
            ShowAwardInfo(TaskData.Instance.m_MainComplete);
        }
    }

    void ShowAwardInfo(ZhuXianTemp temp)
    {
        UI3DEffectTool.Instance().ClearUIFx(m_ObjFinish);
        m_ObjFinish.SetActive(false);
        m_ObjHidden.SetActive(true);
        m_TaskSignal = this;
        taskId = temp.id;
        _rewardInfo = temp.award;
        m_labelTitle.text = temp.title;
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
    }
    private bool _isCancelEffect = false;
    void GetAwards(GameObject obj)
    {
        if (obj.name.Equals("CancelEffect"))
        {
            _isCancelEffect = true;
        //    m_ListEvent[1].gameObject.SetActive(false);
            UI3DEffectTool.Instance().ClearUIFx(m_ObjFinish);
            ChargeData();
        }
        else
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            GetTaskReward tempRequest = new GetTaskReward();
            tempRequest.taskId = taskId;
            t_qx.Serialize(t_tream, tempRequest);

            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GetTaskReward, ref t_protof);
        }
        //Debug.Log("ProtoIndexes.C_GetTaskRewardProtoIndexes.C_GetTaskReward ::" + ProtoIndexes.C_GetTaskReward + "  taskIdtaskIdtaskIdtaskId ::" + taskId);

    }

    int index_Index = 0;

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


    void OnDisable()
    {
        int size = _listObj.Count;
        for (int i = 0; i < size; i++)
        {
            UI3DEffectTool.Instance().ClearUIFx(_listObj[i]);
        }
    }
}
