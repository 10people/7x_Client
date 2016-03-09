using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
 
public class OnlineRewardcGiftBagsItemManagerment :  MonoBehaviour 
{
    public EventHandler m_EventTouch;
    public GameObject m_LingQuObj;
    public delegate void OnClick_Touch(int id);
    OnClick_Touch CallBackButtonEvent;
    public int id = 0;
    public int type = 0;
    public UILabel m_labelButton;
    public UILabel m_labelTitle;
    
    public UIGrid m_Gride;

    public struct RewardInfo
    {
        public int id;

        public int count;
    }

 
   public List<RewardInfo> _listReward = new List<RewardInfo>();
 
    void Start()
    {
        m_EventTouch.m_click_handler += Touch;
    }
    public void ShowInfo(HuoDongInfo info,OnClick_Touch callback)
    {
        id = info.huodongId;
        CallBackButtonEvent = callback;
        _listReward.Clear();
        if (info.state == 10) //奖励状态10：未领取 20：已领取 30：超时不能领取 40:不可领取
        {
            m_EventTouch.transform.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
        }
        else if (info.state == 20) 
        {
            m_LingQuObj.SetActive(true);
            m_EventTouch.gameObject.SetActive(false);
           // m_labelButton.text = DescIdTemplate.GetDescriptionById();
        }
        else if (info.state == 30 || info.state == 40) 
        {
            m_EventTouch.transform.GetComponent<Collider>().enabled = false;
            m_EventTouch.transform.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
        }
  
        //if (int.Parse(XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).doneCondition) > 10)
        //{
        //    string [] title = XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).desc.Split('*');

        //    m_labelTitle.text = title[0] + int.Parse(XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).doneCondition) / 60 + title[2];
        //}
        //else 
        //{
          //  string[] title = XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).desc.Split('*');

        m_labelTitle.text = XianshiHuodongTemp.GetXianShiHuoDongById(info.huodongId).desc;
      //  }
  
		
 
        if (info.jiangli.IndexOf("#") > -1)
        {
            string[] ss = info.jiangli.Split('#');
            for (int j = 0; j < ss.Length; j++)
            {
                string[] award = ss[j].Split(':');
                RewardInfo reward = new RewardInfo();

                reward.id = int.Parse(award[1]);
 
                reward.count = int.Parse(award[2]);
                _listReward.Add(reward);
            }
        }
        else
        {
            string[] award = info.jiangli.Split(':');
            RewardInfo reward = new RewardInfo();
            reward.id = int.Parse(award[1]);
            reward.count = int.Parse(award[2]);
            _listReward.Add(reward);
        }

        int ss_size = _listReward.Count;
        for (int i = 0; i < ss_size; i++)
        {
            CreateItem();
        }

    }
    void Touch(GameObject obj)
    {
        if (CallBackButtonEvent != null)
        {
            CallBackButtonEvent(id);
        }
    
    }
    void CreateItem()
    {

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                          ResLoaded);
    }
    private int _indexNum = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_Gride != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _listReward[_indexNum].id.ToString();
            tempObject.transform.parent = m_Gride.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(_listReward[_indexNum].id, _listReward[_indexNum].count.ToString(), 10);
            
            iconSampleManager.SetIconPopText(_listReward[_indexNum].id, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).descId));

            tempObject.transform.localScale = Vector3.one*0.75f;

            if (_indexNum < _listReward.Count - 1)
            {
                _indexNum++;
            }
            m_Gride.repositionNow = true;
        }
        else
        {
            p_object = null;
        }

    }
}
