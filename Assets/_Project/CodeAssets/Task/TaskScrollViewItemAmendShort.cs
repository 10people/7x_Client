using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;

public class TaskScrollViewItemAmendShort : MonoBehaviour 
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
    public UISprite m_XiangZi;
    public GameObject m_flag_finish;

    public GameObject m_Kuang;


    private TaskLayerManager.TaskNeedInfo _SavetaskInfo;
    public delegate void OnClick_Touch(TaskLayerManager.TaskNeedInfo taskInfo, GameObject obj);
    OnClick_Touch CallBackTouch;
 
    public int m_itemId;
 
 
    private List<FunctionWindowsCreateManagerment.RewardInfo> listRewardInfo = new List<FunctionWindowsCreateManagerment.RewardInfo>();
 	public UISprite m_spriteJindu;
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
		int W = 0;
        if (taskInfo._Progress < 0)
        {
			m_LingQu.spriteName = "TaskIconWeiwancheng";
            if (m_XiangZi != null)
            {
                m_XiangZi.gameObject.SetActive(false);
            }
//            m_backGround.spriteName = "BlueItemBg";
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);
            m_progressLabel.text = "100%";
			W = 194;
        }
        else  
        {
            if (m_XiangZi != null)
            {
                m_XiangZi.gameObject.SetActive(true);
            }
//            m_backGround.spriteName = "BlueItemBg";
            m_LingQu.spriteName = "TaskIconWancheng";
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);

            switch ((TaskType)taskInfo._Type)
            {
	            case TaskType.DailyQuest:
					W = Global.getBili(194, taskInfo._Progress, RenWuTemplate.GetRenWuById(taskInfo._TaskId).condition);
				m_progressLabel.text = (int)((float)taskInfo._Progress / (float)RenWuTemplate.GetRenWuById(taskInfo._TaskId).condition * 100) + "%";
//	                m_progressLabel.text =  taskInfo._Progress + "/" +  RenWuTemplate.GetRenWuById(taskInfo._TaskId).condition.ToString();
	            break;
	            case TaskType.SideQuest:
				W = Global.getBili(194, taskInfo._Progress, 1);
	                m_progressLabel.text = taskInfo._Progress + "/1"; //ZhuXianTemp.getTemplateById(taskInfo._TaskId).condition.ToString();
				m_progressLabel.text = (int)((float)taskInfo._Progress / (float)1 * 100) + "%";
				break;
            }

        }
		m_spriteJindu.SetDimensions(W, 10);
		if(W == 0)
		{
			m_spriteJindu.gameObject.SetActive(false);
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
