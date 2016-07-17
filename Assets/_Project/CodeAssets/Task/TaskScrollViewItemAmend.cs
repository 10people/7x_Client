using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;

public class TaskScrollViewItemAmend : MonoBehaviour 
{
    public UISprite m_taskIcon;

    public GameObject m_Effect;
   // public UISprite m_taskIconTag;

    public UILabel m_nameLabel;

    public UILabel m_dicLabel;

    public UILabel m_progressLabel;

    public UIGrid m_awardIconGrid;

    public UISprite m_backGround;
    public UISprite m_LingQu;

    public GameObject m_flag_finish;

    public GameObject m_Kuang;


    private TaskLayerManager.TaskNeedInfo _SavetaskInfo;
    public delegate void OnClick_Touch(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj);
    OnClick_Touch CallBackTouch;
 
    public int m_itemId;
 
 
    private List<FunctionWindowsCreateManagerment.RewardInfo> listRewardInfo = new List<FunctionWindowsCreateManagerment.RewardInfo>();
 
    void Start()
    {
       
    }

  
 
    int index_Num = 0;

    public void ShowTaskInfo(TaskLayerManager.TaskNeedInfo taskInfo, OnClick_Touch callback)
    {
        _SavetaskInfo = taskInfo;
 
        CallBackTouch = callback;
        index_Num = 0;
        listRewardInfo.Clear();
    
        m_itemId = taskInfo._TaskId;
 
        listRewardInfo = taskInfo._listReward;
        int size_all = listRewardInfo.Count;
        index_num = 0;
        if (m_awardIconGrid.transform.childCount == 0)
        {
            for (int i = 0; i < size_all; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
        }

        if (taskInfo._Progress < 0)
        {
            m_LingQu.gameObject.SetActive(true);
            m_backGround.spriteName = "jianbianbgliang";
            if(taskInfo._Type == 2 && m_LingQu.GetComponent<Animator>()== null)
            {
              EffectTool.OpenMultiUIEffect_ById( m_LingQu.gameObject, 223, 224, 225);
            }
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);
            m_progressLabel.text = "";

        }
        else  
        {
            m_backGround.spriteName = "jianbianbgan";
            m_LingQu.gameObject.SetActive(false);
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);
            switch ((TaskType)taskInfo._Type)
            {
                case TaskType.DailyQuest:
                    {
                        m_progressLabel.text =  taskInfo._Progress + "/" +  RenWuTemplate.GetRenWuById(taskInfo._TaskId).condition.ToString();
                    }
                    break;
                case TaskType.SideQuest:
                    {
                        m_progressLabel.text = taskInfo._Progress + "/1"; //ZhuXianTemp.getTemplateById(taskInfo._TaskId).condition.ToString();
                    }
                    break;
            }

        }

      //  m_Kuang.SetActive(taskInfo._Progress < 0);
   

        m_nameLabel.text = taskInfo._Name;
        m_dicLabel.text = taskInfo._Des;
        m_taskIcon.spriteName = "Icon_RenWuType_" + taskInfo._npcIcon;

        m_awardIconGrid.Reposition();
    }
  
    private int index_num = 0;

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_awardIconGrid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_awardIconGrid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
          
            iconSampleManager.SetIconByID(listRewardInfo[index_num].icon, listRewardInfo[index_num].count.ToString(), 4);
            iconSampleManager.SetIconPopText(listRewardInfo[index_num].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listRewardInfo[index_num].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listRewardInfo[index_num].icon).descId));
            iconSampleObject.transform.localScale = Vector3.one * 0.4f;
            if (index_num < listRewardInfo.Count - 1)
            {
                index_num++;
            }
           
            m_awardIconGrid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    

   
    void OnClick()
    {
        if (CallBackTouch != null && Input.touchCount < 2)
        {
            CallBackTouch(_SavetaskInfo, gameObject);
        }
    }

    int index_Num2 = 0;
 
}
