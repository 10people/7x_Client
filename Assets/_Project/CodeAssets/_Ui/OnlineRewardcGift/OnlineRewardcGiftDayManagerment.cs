using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class OnlineRewardcGiftDayManagerment : MonoBehaviour, SocketProcessor
{
    public ScaleEffectController m_SEC;
    public GameObject m_MainParent;
    public GameObject m_CenterParent;
    public UIGrid m_MidParent;
    public UIGrid m_BottomParent;
    public UILabel m_LabTopTitle;
    public UILabel m_LabMiddleTitle;
    public UILabel m_LabMiddleCount;
    public UILabel m_LabMiddleBottom;
   
    public UILabel m_LabBottomTitle;
    public UILabel m_LabButton;
    public UILabel m_LabTimeDown;

    public GameObject m_HiddenObject;
    public EventIndexHandle m_TouchEvent;
    private List<HuoDongInfo> _listOnlineInfo = new List<HuoDongInfo>();
    public struct SkillsInfo
    {
        public string icon;
        public string name;
        public string des;
    }
    

    private struct CurrentInfo
    {
        public int huodongType;
        public string topTitle;
        public string midTitle;
        public string midCount;
        public string bottomTitle;
        public string bottomTitle2;
        public List<SkillsInfo> listSkills;
        public string rewardInfo;
    };
    private CurrentInfo _MainInfo = new CurrentInfo();
    public struct RewardInfo
    {
        public int id;
        public int count;
    }
    private List<RewardInfo> _listReward = new List<RewardInfo>();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
    void Start ()
    {
        _MainInfo.listSkills = new List<SkillsInfo>();
        m_SEC.OpenCompleteDelegate += RequestRewardcGiftDay;
        m_TouchEvent.m_Handle += TouchEvent;
    }
    private bool _isTimeDown = false;
    private int _TimeCount = 0;
    void Update()
    {
        if (_isTimeDown)
        {
          _isTimeDown = false;
          _TimeCount--;
          StartCoroutine(TimeDown());
        }
    }

    void OnDisable()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    //void OnDestroy()
    //{
      
    //}
    IEnumerator TimeDown()
    {
        yield return new WaitForSeconds(1.0f);
        m_LabTimeDown.text = TimeHelper.GetUniformedTimeString(_TimeCount);
        if (_TimeCount > 0)
        {
            _isTimeDown = true;
        }
        else
        {
            m_LabTimeDown.gameObject.SetActive(false);
            m_LabButton.gameObject.SetActive(true);
            m_TouchEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
        }
    }
    void RequestRewardcGiftDay()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        XinShouXSActivity xinshou = new XinShouXSActivity();
        xinshou.typeId = 1543000;
        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_INFO_REQ, ref t_protof);
    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_XINSHOU_XIANSHI_INFO_RESP:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        XinShouXianShiInfo ReponseInfo = new XinShouXianShiInfo();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                        _TimeCount = ReponseInfo.beizhu/1000;
                        TidyData(ReponseInfo);
                        m_HiddenObject.SetActive(true);
                        return true;
                    }
                    break;
                case ProtoIndexes.S_XINSHOU_XIANSHI_AWARD_RESP:// 
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        ReturnAward ReponseInfo = new ReturnAward();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());
                   
                        for (int i = 0; i < _listOnlineInfo.Count; i++)
                        {
                            if (_listOnlineInfo[i].huodongId == ReponseInfo.huodongId)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(_listOnlineInfo[0].jiangli);
                                _listOnlineInfo.RemoveAt(i);
                                break;
                            }
                        }

 
                        if (_listOnlineInfo.Count > 0)
                        {
                            int size = m_CenterParent.transform.childCount;
                            for (int i = 0; i < size; i++)
                            {
                                Destroy(m_CenterParent.transform.GetChild(i).gameObject);
                            }

                            int size_ = m_BottomParent.transform.childCount;
                            for (int i = 0; i < size_; i++)
                            {
                                Destroy(m_BottomParent.transform.GetChild(i).gameObject);
                            }

                            int size_mid = m_MidParent.transform.childCount;
                            for (int i = 0; i < size_mid; i++)
                            {
                                Destroy(m_MidParent.transform.GetChild(i).gameObject);
                            }

                            if (_TimeCount == 0)
                            {
                                _TimeCount = 86400;
                            }
                       
                            ShowData(_listOnlineInfo[0]);
                        }
                        else
                        {
                            Destroy(m_MainParent);
                        }
                  
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    void TouchEvent(int index)
    {
        if (index == 0)
        {

        }
        else
        {
            GetAward();
        }
    }
    void TidyData(XinShouXianShiInfo data)
    {
        _listOnlineInfo.Clear();
        int size = data.huodong.Count;
        for (int i = 0; i < size; i++)
        {
            int id = 0;
            if (data.huodong[i].state == 10)
            {
                id = data.huodong[i].huodongId;
                _listOnlineInfo.Add(data.huodong[i]);
            }
            else if (data.huodong[i].state == 40)
            {
                id = data.huodong[i].huodongId;
                _listOnlineInfo.Add(data.huodong[i]);
            }
        }
        if (_listOnlineInfo.Count > 0)
        {
            ShowData(_listOnlineInfo[0]);
        }
    }

    void ShowData(HuoDongInfo hdinfo)
    {
        _listReward.Clear();
        if (hdinfo.state == 10) //奖励状态10：未领取 20：已领取 30：超时不能领取 40:不可领取
        {
            m_TouchEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
            m_LabButton.gameObject.SetActive(true);
            m_LabTimeDown.gameObject.SetActive(false);
        }
        else if (hdinfo.state == 40)
        {
            m_TouchEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            m_LabTimeDown.gameObject.SetActive(true);
            m_LabButton.gameObject.SetActive(false);
            m_LabTimeDown.text = TimeHelper.GetUniformedTimeString(_TimeCount);
            _isTimeDown = true;
        }
       // m_LabMiddleTitle.text = XianshiHuodongTemp.GetXianShiHuoDongById(hdinfo.huodongId).desc;

        int size = QiriQiandaoTemplate.templates.Count;
        for (int i = 0; i < size; i++)
        {
            if (QiriQiandaoTemplate.templates[i].id == hdinfo.huodongId)
            {
                _MainInfo.topTitle = QiriQiandaoTemplate.templates[i].bigTitle1;
                _MainInfo.midTitle = QiriQiandaoTemplate.templates[i].bigTitle2;
                _MainInfo.midCount = QiriQiandaoTemplate.templates[i].desc3;
                _MainInfo.huodongType = QiriQiandaoTemplate.templates[i].bigItemType;
                _MainInfo.bottomTitle = LanguageTemplate.GetText(LanguageTemplate.Text.QIRI_EVERDAYDESC8);
                _MainInfo.bottomTitle2 = QiriQiandaoTemplate.templates[i].tomorrowDesc;
                _MainInfo.listSkills = new List<SkillsInfo>();
                if (!string.IsNullOrEmpty(QiriQiandaoTemplate.templates[i].detail1))
                {
                    if (QiriQiandaoTemplate.templates[i].detail1.IndexOf("#") > -1)
                    {
                        string[] ss = QiriQiandaoTemplate.templates[i].detail1.Split('#');
                        for (int j = 0; j < ss.Length; j++)
                        {
                            string[] award = ss[j].Split(':');
                            SkillsInfo si = new SkillsInfo();
                            si.icon = award[0];
                            si.name = award[1];
                            si.des = award[2];
                            _MainInfo.listSkills.Add(si);
                        }
                    }
                    else
                    {
                        string[] award = QiriQiandaoTemplate.templates[i].detail1.Split(':');
                        SkillsInfo si = new SkillsInfo();
                        si.icon = award[0];
                        si.name = award[1];
                        si.des = award[2];
                        _MainInfo.listSkills.Add(si);
                    }
                }
                _MainInfo.rewardInfo = QiriQiandaoTemplate.templates[i].award;
         
                break;
            }
        }
        RewardData();

    }

    void RewardData()
    {
        switch (_MainInfo.huodongType)
        {
            case 1:
                {
                    m_LabTopTitle.text = "";
                }
                break;
            case 2:
                {
                    m_LabTopTitle.text = LanguageTemplate.GetText(int.Parse(_MainInfo.topTitle));
                }
                break;
            default:
                break;
        }
        m_LabMiddleTitle.text = LanguageTemplate.GetText(int.Parse(_MainInfo.midTitle));
        m_LabMiddleCount.text = LanguageTemplate.GetText(int.Parse(_MainInfo.midCount));
        m_LabMiddleBottom.text = _MainInfo.bottomTitle;
        m_LabBottomTitle.text = LanguageTemplate.GetText(int.Parse(_MainInfo.bottomTitle2));
        if (_MainInfo.rewardInfo.IndexOf("#") > -1)
        {
            string[] ss = _MainInfo.rewardInfo.Split('#');
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
            string[] award = _MainInfo.rewardInfo.Split(':');
            RewardInfo reward = new RewardInfo();
            reward.id = int.Parse(award[1]);
            reward.count = int.Parse(award[2]);
            _listReward.Add(reward);
        }
        CreateItem();
    }
    void CreateItem()
    {
        _indexNum = 0;
        int size = _listReward.Count;
        m_BottomParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(size - 1, 120), 0, 0);
    
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                              ResLoadedSimple);
        }
    }
    void GetAward()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GainAward xinshou = new GainAward();
        xinshou.typeId = 1542000;
        xinshou.huodongId = _listOnlineInfo[0].huodongId;
        t_qx.Serialize(t_tream, xinshou);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_XINSHOU_XIANSHI_AWARD_REQ, ref t_protof);
    }
    int _indexNum = 0;

    int index_Attr = 0;
    void ResLoaded_Attribute(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_MidParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
          
           tempObject.transform.parent = m_MidParent.transform;
        
            tempObject.name = index_Attr.ToString();
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.localPosition = Vector3.zero;
  
            tempObject.GetComponent<OnlineRewardGiftSkillItemManangerment>().ShowInfo(_MainInfo.listSkills[index_Attr]);
            
            if (index_Attr < _MainInfo.listSkills.Count - 1)
            {
                index_Attr++;
            }
            m_MidParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_CenterParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            if (_indexNum == 0)
            {
                tempObject.transform.parent = m_CenterParent.transform;
            }
            else
            {
                tempObject.transform.parent = m_BottomParent.transform;
            }
            tempObject.name = _listReward[_indexNum].id.ToString();
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            if (_indexNum == 0)
            {
                if (_listReward[_indexNum].count > 1)
                {
                    iconSampleManager.SetIconByID(_listReward[_indexNum].id, _listReward[_indexNum].count.ToString(), 10);
                }
                else
                {
                    iconSampleManager.SetIconByID(_listReward[_indexNum].id, "", 10);
                }
            }
            else
            {
                iconSampleManager.SetIconByID(_listReward[_indexNum].id, _listReward[_indexNum].count.ToString(), 10);
            }
           
            iconSampleManager.SetIconPopText(_listReward[_indexNum].id, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listReward[_indexNum].id).descId));

            if (_indexNum == 0)
            {
                tempObject.transform.localScale = Vector3.one * 0.8f;
            }
            else
            {
                tempObject.transform.localScale = Vector3.one * 0.55f;
                m_BottomParent.repositionNow = true;
            }

            if (_indexNum < _listReward.Count - 1)
            {
                _indexNum++;
            }
            else
            {
                if (_MainInfo.listSkills.Count > 0)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ONLINE_SKILL_ITEM),
                                   ResLoaded_Attribute);
                }
            }
        }
        else
        {
            p_object = null;
        }
    
    }
}
