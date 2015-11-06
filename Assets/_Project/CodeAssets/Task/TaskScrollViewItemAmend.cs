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

    private string award = "";
    private string _title = "";


    public delegate void OnClick_Touch(int id,string award,string title,GameObject obj);
    OnClick_Touch CallBackTouch;
 
    public int m_itemId;
    struct RewardInfo
    {
        public int type;
        public string count;
        public string icon;
    }
  //  private RewardInfo rewardShowInfo;
    private List<RewardInfo> listRewardInfo = new List<RewardInfo>();
    private List<RewardInfo> listRewardInfo2 = new List<RewardInfo>();
    void Start()
    {
       
    }

  
    bool isFinish = false;
    int index_Num = 0;
    int taskType = 0;

    public void ShowTaskInfo( int type, int tempId, int tempJindu, bool tempComplete, string reward, string name, string des, int triggerCond, int doneCond, OnClick_Touch callback,string title)
    {
        taskType = type;
        CallBackTouch = callback;
        index_Num = 0;
        listRewardInfo.Clear();
        isFinish =  tempComplete;
        m_itemId = tempId;
        award = reward;
        _title = title;
        if (!string.IsNullOrEmpty(reward) && m_awardIconGrid.transform.childCount == 0)
        {
            string[] tempAwardList = reward.Split('#');
            for (int i = 0; i < tempAwardList.Length;i++ )
            {
                string[] tempAwardItemInfo = tempAwardList[i].Split(':');
                RewardInfo rInfo = new RewardInfo();
                rInfo.type = int.Parse(tempAwardItemInfo[0]);
                rInfo.icon = tempAwardItemInfo[1];
                rInfo.count = tempAwardItemInfo[2];
                listRewardInfo.Add(rInfo);
               
              
             
                m_awardIconGrid.Reposition();
            }
            int size_all = listRewardInfo.Count;
            index_num = 0;
            for (int i = 0; i < size_all; i++)
            {
               Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_AWARD_ITEM_SMALL), ResourcesLoadCallBack);
               // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
            }
                //if (CityGlobalData.m_AllianceIsHave && CityGlobalData.m_TaskType == (int)TaskType.DailyQuest)
                //{
                //    CreateAppendReward();
                //}
           
        }

        string tempString = "";

        if (tempComplete == true)
        {
            m_LingQu.gameObject.SetActive(true);
         //   tempString = "backGround_Common_ing";
            tempString = "bg2";
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);
          //  m_progressLabel.text = "完成";
            m_progressLabel.text = "";

        }
		else if(triggerCond == 3)
        {
            m_LingQu.gameObject.SetActive(false);
            tempString = "backGround_Common_big";
            m_flag_finish.SetActive(false);
            m_progressLabel.gameObject.SetActive(true);
            switch ((TaskType)CityGlobalData.m_TaskType)
            {
                case TaskType.MainQuest:
                    {
                        m_progressLabel.text = tempJindu + "/" + TaskData.Instance.m_TaskInfoDic[tempId].progress.ToString();
                    }
                    break;
                case TaskType.DailyQuest:
                    {
                        m_progressLabel.text = tempJindu + "/" + TaskData.Instance.m_TaskDailyDic[tempId].condition.ToString();
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
			
        }
		else 
		{
            m_LingQu.gameObject.SetActive(false);
            tempString = "backGround_Common_big";
			m_flag_finish.SetActive(false);
			m_progressLabel.gameObject.SetActive(true);
			m_progressLabel.text = tempJindu + "/1";

		}

        m_backGround.spriteName = tempString;

        m_nameLabel.text = name;
		m_dicLabel.text = des;
        m_taskIcon.spriteName = ZhuXianTemp.GetTaskIconById(tempId);
     
        m_awardIconGrid.Reposition();
    }
    private int index_num = 0;
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
  
        GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        iconSampleObject.SetActive(true);
        iconSampleObject.transform.parent = m_awardIconGrid.transform;
        iconSampleObject.transform.localPosition = Vector3.zero;
        iconSampleObject.transform.localScale = Vector3.one*0.45f;
        IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        //0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；9强化材料
       
        if (listRewardInfo[index_num].type == 6 || listRewardInfo[index_num].type == 0 || listRewardInfo[index_num].type == 2 || listRewardInfo[index_num].type == 3 || listRewardInfo[index_num].type == 9)
        {
            iconSampleManager.SetIconType((IconSampleManager.IconType)2);
        }
        else if (listRewardInfo[index_num].type == 5)
        {
            iconSampleManager.SetIconType((IconSampleManager.IconType)8);
        }
        else
        {
            iconSampleManager.SetIconType((IconSampleManager.IconType)listRewardInfo[index_num].type);
        }


        iconSampleManager.SetIconBasic(0, listRewardInfo[index_num].icon, listRewardInfo[index_num].count.ToString());
        //iconSampleManager.FgSprite.spriteName = rewardInfo.icon;
        //iconSampleManager.FgSprite.gameObject.SetActive(true);
        //iconSampleManager.RightButtomCornorLabel.text = rewardInfo.count.ToString();
		iconSampleManager.SetIconPopText(int.Parse(listRewardInfo[index_num].icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_num].icon)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listRewardInfo[index_num].icon)).descId), 0, Vector3.zero);
        if (index_num < listRewardInfo.Count - 1)
        {
            index_num++;
        }
    }

    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_awardIconGrid != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;

            rewardShow.transform.parent = m_awardIconGrid.transform;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.GetComponent<TaskAwardItemAmend>().Show(listRewardInfo[index_Num].icon, listRewardInfo[index_Num].count,  listRewardInfo[index_Num].type);
            if (index_Num < listRewardInfo.Count - 1)
            {
                index_Num++;
            }
            else
            {
                if (isFinish && taskType == 0)
                {
                    //UI3DEffectTool.Instance().ShowMidLayerEffect(
                    // UI3DEffectTool.UIType.FunctionUI_1,
                    // gameObject/*.GetComponent<TaskScrollViewItemAmend>().m_Effect*/, EffectIdTemplate.GetPathByeffectId(100011), null);
                    m_Kuang.SetActive(true);
                }
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
        if (isFinish && taskType == 0) return;

        if (isFinish)
        {
            CallBackTouch(m_itemId, award, _title, gameObject);
            UI3DEffectTool.Instance().ClearUIFx(gameObject);

        }
        else if (!isFinish)
        { 
             if (m_itemId == 10)
            {
                CallBackTouch(0, "", "", gameObject);
            }
            else
            {
                CallBackTouch(m_itemId, "", "", gameObject);
            }
        }
       
    }

    int index_Num2 = 0;
    void CreateAppendReward()
    {
        listRewardInfo2.Clear();
      string[] AwardItemInfo = RenWuTemplate.GetAllianceRewardById(m_itemId).Split('#');
      foreach (string tempAward in AwardItemInfo)
      {
          string[] tempAwardItemInfo = tempAward.Split(':');
          RewardInfo rinfo = new RewardInfo();
          rinfo.type = int.Parse(tempAwardItemInfo[0]);
          rinfo.icon = tempAwardItemInfo[1];
          rinfo.count = tempAwardItemInfo[2];
          listRewardInfo2.Add(rinfo);
          
      }
       int size_append = listRewardInfo2.Count;
       for (int i = 0; i < size_append; i++)
       {
           Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TASK_AWARD_ITEM_SMALL), ResourcesLoadCallBack2);
       }
    }
    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject rewardShow = Instantiate(p_object) as GameObject;

        rewardShow.transform.parent = m_awardIconGrid.transform;
        rewardShow.transform.localPosition = Vector3.zero;
        rewardShow.transform.localScale = Vector3.one;
        rewardShow.transform.GetComponent<TaskAwardItemAmend>().Show(listRewardInfo2[index_Num2].icon, listRewardInfo2[index_Num2].count, listRewardInfo2[index_Num2].type);
        if (index_Num2 < listRewardInfo2.Count - 1)
        {
            index_Num2++;
        }
        m_awardIconGrid.Reposition();
    }
    void OnDestroy()
    {

    }
}
