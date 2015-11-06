using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NationManagerment : MonoBehaviour, SocketProcessor
{
    public List<UILabel> m_ListInfo;
    public UIScrollView m_ScrollView;
    public List<EventIndexHandle> m_ListEvent;
    public UIGrid m_GridObject;
    public GameObject m_AllInfo;
    public GameObject m_Introduce;
    public GameObject m_ObjectForward;
    public GameObject m_ObjectNow;
    public GameObject m_ObjectReward;
    public UIGrid m_ObjectRewardParent;
    public GameObject m_Tanhao;

    public ScaleEffectController m_SEC;

    private List<GameObject> _listitem = new List<GameObject>();
    private List<GuojiaRankInfo> _listNationInfo = new List<GuojiaRankInfo>();
    private string _strTitle = "";
    private string _strContent1 = "";
    private string _strContent2 = "";
    private string _strNotice = "";
    private bool _isLocked = false;
    private bool _isEnough = false;
    private int _lastRank = 0;
    // private string _strReward = "";



    private bool _isForward = false;
    private GuoJiaMainInfoResp _GuojiaInfo = new GuoJiaMainInfoResp();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        NationData.Instance.m_DataGetComplete = false;
        m_SEC.OpenCompleteDelegate += RequestData;
        m_ListEvent.ForEach(p => p.m_Handle += ShowInfo);
    }

    public void RequestData()
    {
        NationData.Instance.m_DataRequest = true;
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.GUO_JIA_MAIN_INFO_REQ);
    }

    void Update()
    {
        if (NationData.Instance.m_DataGetComplete)
        {
            NationData.Instance.m_DataGetComplete = false;
            NationInfo(NationData.Instance.m_NationInfo);
        }
    }
    void ShowInfo(int index)
    {

        switch (index)
        {
            case 0:
                {
                    string _sss = LanguageTemplate.GetText(LanguageTemplate.Text.ZHU_BU_RULE_1) + "\n"
                        + LanguageTemplate.GetText(LanguageTemplate.Text.ZHU_BU_RULE_2) + "\n"
                        + LanguageTemplate.GetText(LanguageTemplate.Text.ZHU_BU_RULE_3) + "\n"
                        + LanguageTemplate.GetText(LanguageTemplate.Text.ZHU_BU_RULE_4) + "\n";

                    m_ListInfo[9].text = _sss;
                    m_AllInfo.SetActive(false);
                    m_Introduce.SetActive(true);
                }
                break;
            case 1:
                {
                    m_ListEvent[1].GetComponent<Collider>().enabled = false;
                    //    Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_JUANXIAN_DAYAWARD_REQ);
                }
                break;
            case 2:
                {
                    m_ListEvent[2].GetComponent<Collider>().enabled = false;
                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0 || !_isLocked || !_isEnough)
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content1);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                   UIBoxLoadCallbackOne);
                        }
                        else if (!_isLocked)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content2) + CanshuTemplate.GetStrValueByKey("OPENTIME_LUEDUO") + " - " + CanshuTemplate.GetStrValueByKey("CLOSETIME_LUEDUO");
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UIBoxLoadCallbackOne);
                        }
                        else
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content7);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UIBoxLoadCallbackOne);
                        }
                    }
                    else
                    {

                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_JUANXIAN_GONGJIN_REQ);
                    }

                }
                break;
            case 3:
                {
                    if (!_isForward)
                    {
                        m_ObjectForward.SetActive(false);
                        m_ObjectNow.SetActive(true);
                        TidyData(_GuojiaInfo.lastRank);
                    }
                    else
                    {
                        m_ObjectForward.SetActive(true);
                        m_ObjectNow.SetActive(false);
                        TidyData(_GuojiaInfo.nowRank);
                    }
                    _isForward = !_isForward;
                    //m_AllInfo.SetActive(true);
                    //m_Introduce.SetActive(false);
                }
                break;
            case 4:
                {
                    m_AllInfo.SetActive(true);
                    m_Introduce.SetActive(false);
                }
                break;
            case 7:
                {
                    m_ObjectReward.SetActive(false);
                }
                break;
            case 8:
                {
                    m_ObjectReward.SetActive(false);
                }
                break;
            default: break;
        }
    }

    void NationInfo(GuoJiaMainInfoResp temp)
    {
        _GuojiaInfo = temp;
        if (temp.nowRank != null)
        {
            for (int i = 0; i < temp.nowRank.Count; i++)
            {
                if (temp.nowRank[i].guojiaId == JunZhuData.Instance().m_junzhuInfo.guoJiaId)
                {
                    _lastRank = temp.nowRank[i].rank + 1;
                    m_ListInfo[3].text = (temp.nowRank[i].rank + 1).ToString();
                    m_ListInfo[2].text = temp.nowRank[i].shengwang.ToString();
                    break;
                }
            }

            //if (temp.lastRank == null)
            //{
            //    for (int i = 0; i < temp.lastRank.Count; i++)
            //    {
            //        if (temp.lastRank[i].guojiaId == JunZhuData.Instance().m_junzhuInfo.guoJiaId)
            //        {
            //            _lastRank = temp.lastRank[i].rank + 1;
            //            break;
            //        }
            //    }
            //}
        }


        m_AllInfo.SetActive(true);

        switch (JunZhuData.Instance().m_junzhuInfo.guoJiaId)
        {
            case 1:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_1);
                }
                break;
            case 2:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_4);
                }
                break;
            case 3:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_2);
                }
                break;
            case 4:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_6);
                }
                break;
            case 5:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_7);
                }
                break;
            case 6:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_5);
                }
                break;
            case 7:
                {
                    _strNotice = LanguageTemplate.GetText(LanguageTemplate.Text.COUNTRY_NOTICE_3);
                }
                break;
            default:
                break;
        }

        m_ListInfo[0].text = NameIdTemplate.GetName_By_NameId(temp.guojiaId);
        m_ListInfo[1].text = temp.kingName;

        m_ListInfo[4].text = temp.todayGive.ToString();
        m_ListInfo[5].text = temp.thisWeekGive.ToString();
        //_TodayGiveCount = temp.todayGive;
        //_WeekGiveCount = temp.thisWeekGive;
     
        //; jinjie
        m_ListInfo[6].text = temp.myGongJin.ToString() + "/" + temp.shouldGive.ToString();
        //_AllShengWang = temp.myGongJin;
        //_shouldGiveShengWang = temp.shouldGive;
        _isEnough = temp.myGongJin >= temp.shouldGive ? true : false;
        string diduiNation = "";
        if (temp.hate_guoId1 != 0)
        {
            diduiNation += NameIdTemplate.GetName_By_NameId(temp.hate_guoId1);
        }

        if (temp.hate_guoId2 != 0 && temp.hate_guoId1 != 0)
        {
            diduiNation += "、" + NameIdTemplate.GetName_By_NameId(temp.hate_guoId2);
        }
        else
        {
            diduiNation += NameIdTemplate.GetName_By_NameId(temp.hate_guoId2);
        }
        m_ListInfo[7].text = diduiNation;

        m_ListInfo[8].text = _strNotice;
      //   Debug.Log("isCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGive ::" + temp.isCanGive);
        _isLocked = temp.isCanGive;
 
        if (PushAndNotificationHelper.IsShowRedSpotNotification(500020) && _isLocked)
        {
            PushAndNotificationHelper.SetRedSpotNotification(500020, true);
        }
        else
        {
            PushAndNotificationHelper.SetRedSpotNotification(500020, false);
        }
        m_Tanhao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(500020) && _isLocked);
        //_lastRank = temp.lastGuoRank;
        if (temp.guojiaAward.Equals("0"))
        {
            m_ListEvent[1].GetComponent<Collider>().enabled = false;
            m_ListEvent[1].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        }
         ;
        TidyData(temp.nowRank);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //case ProtoIndexes.GUO_JIA_MAIN_INFO_RESP:// 获得国家信息
                //    {
                //        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        GuoJiaMainInfoResp ReponseInfo = new GuoJiaMainInfoResp();
                //        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                //        _GuojiaInfo = ReponseInfo;
                //        Debug.Log("isCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGiveisCanGive ::");
                //        //if (ReponseInfo.nowRank != null)
                //        //{
                //        //    for (int i = 0; i < ReponseInfo.nowRank.Count; i++)
                //        //    {
                //        //        if (ReponseInfo.nowRank[i].guojiaId == JunZhuData.Instance().m_junzhuInfo.guoJiaId)
                //        //        {
                //        //            _lastRank = ReponseInfo.nowRank[i].rank + 1;
                //        //            m_ListInfo[3].text = (ReponseInfo.nowRank[i].rank + 1).ToString();
                //        //            m_ListInfo[2].text = ReponseInfo.nowRank[i].shengwang.ToString();
                //        //            break;
                //        //        }
                //        //    }

                //      //    if (ReponseInfo.lastRank == null)
                //      //    {
                //      //        for (int i = 0; i < ReponseInfo.lastRank.Count; i++)
                //      //        {
                //      //            if (ReponseInfo.lastRank[i].guojiaId == JunZhuData.Instance().m_junzhuInfo.guoJiaId)
                //      //            {
                //      //                _lastRank = ReponseInfo.lastRank[i].rank + 1;
                //      //                break;
                //      //            }
                //      //        }
                //      //    }
                //      //}

                //      NationInfo(ReponseInfo);
                //        return true;
                //    }
                //    break;
                case ProtoIndexes.S_GET_JUANXIAN_GONGJIN_RESP:// 上缴结果
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        JuanXianGongJinResp ReponseInfo = new JuanXianGongJinResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());


                        m_ListEvent[2].GetComponent<Collider>().enabled = true;
                        if (ReponseInfo.result == 10)
                        {
                            for (int i = 0; i < ReponseInfo.gjmainInfo.nowRank.Count; i++)
                            {
                                if (ReponseInfo.gjmainInfo.nowRank[i].guojiaId == JunZhuData.Instance().m_junzhuInfo.guoJiaId)
                                {
                                    _lastRank = ReponseInfo.gjmainInfo.nowRank[i].rank + 1;
                                    m_ListInfo[3].text = (ReponseInfo.gjmainInfo.nowRank[i].rank + 1).ToString();
                                    m_ListInfo[2].text = ReponseInfo.gjmainInfo.nowRank[i].shengwang.ToString();
                                    break;
                                }
                            }
                            m_ListInfo[4].text = ReponseInfo.gjmainInfo.todayGive.ToString();
                            m_ListInfo[5].text = ReponseInfo.gjmainInfo.thisWeekGive.ToString();

                            m_ListInfo[6].text = ReponseInfo.gjmainInfo.myGongJin.ToString() + "/" + ReponseInfo.gjmainInfo.shouldGive;
                            _isEnough = ReponseInfo.gjmainInfo.myGongJin >= ReponseInfo.gjmainInfo.shouldGive ? true : false;

                            if (!_isEnough)
                            {
                                m_Tanhao.SetActive(_isEnough);
                                PushAndNotificationHelper.SetRedSpotNotification(500020, false);
                            }
                            else
                            {
                                m_Tanhao.SetActive(_isEnough);
                                PushAndNotificationHelper.SetRedSpotNotification(500020, true);
                            }
                            m_ListInfo[2].text = ReponseInfo.gjshengwang.ToString();
                            // _shouldGiveShengWang = ReponseInfo.nextNeedGongjin;
                            //   TidyData(ReponseInfo.guojiInfo);

                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content3) + _GuojiaInfo.shouldGive.ToString() + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content4) + ReponseInfo.gongxian.ToString()
                                  + "\n" + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content5) + ReponseInfo.lmshengwang.ToString() + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_Content6) + ReponseInfo.gjshengwang.ToString();
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                     UIBoxLoadCallbackOne);

                            _GuojiaInfo = ReponseInfo.gjmainInfo;

                        }
                        else if (ReponseInfo.result == 20)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        else if (ReponseInfo.result == 30)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        else if (ReponseInfo.result == 40)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GONGJIN_NO_ENOUGH);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        else if (ReponseInfo.result == 50)
                        {
                            m_Tanhao.SetActive(false);
                            PushAndNotificationHelper.SetRedSpotNotification(500020, false);
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GONGJIN_TIME_FULL);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        else if (ReponseInfo.result == 60)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.FUNCTION_NO_OPEN);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        else if (ReponseInfo.result == 70)
                        {
                            _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_SHANGJIAO_TITLE);
                            _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GONGJIN_NO_START);
                            _strContent2 = "";
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallbackOne);
                        }
                        return true;
                    }
                    break;
                case ProtoIndexes.S_GET_JUANXIAN_DAYAWARD_RESP:// 领奖
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        JuanXianDayAwardResp ReponseInfo = new JuanXianDayAwardResp();
                        t_qx.Deserialize(t_tream, ReponseInfo, ReponseInfo.GetType());

                        if (ReponseInfo.result == 10)
                        {
                            m_ListInfo[10].text = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_REWARD) + ReponseInfo.guojiRank.ToString()
                                   + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_REWARD_1) + ReponseInfo.lianMengRank.ToString()
                                   + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_REWARD_2);
                            MainCityUIRB.SetRedAlert(212, false);
                            m_ListEvent[1].GetComponent<Collider>().enabled = false;
                            m_ListEvent[1].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_ObjectReward.SetActive(true);
                            ShowReward(ReponseInfo.award);

                            //_strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                            //_strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_CONTENT1) + _lastRank.ToString()
                            //    + LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_CONTENT2) +"\n" + ShowRewardInfo(ReponseInfo.award);
                            //_strContent2 = "";
                            //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                            //                 UIBoxLoadCallbackOne);
                        }
                        //else if (ReponseInfo.result == 20)
                        //{
                        //    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                        //    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.FUNCTION_NO_OPEN);
                        //    _strContent2 = "";
                        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        //                     UIBoxLoadCallbackOne);
                        //}
                        //else if (ReponseInfo.result == 30)
                        //{
                        //    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                        //    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GEIVE_NOT_ENOUGH);
                        //    _strContent2 = "";
                        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        //                     UIBoxLoadCallbackOne);
                        //}
                        //else if (ReponseInfo.result == 40)
                        //{
                        //    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                        //    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GEIVE_NO_REWARD);
                        //    _strContent2 = "";
                        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        //                     UIBoxLoadCallbackOne);
                        //}
                        //else if (ReponseInfo.result == 50)
                        //{
                        //    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                        //    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE);
                        //    _strContent2 = "";
                        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        //                     UIBoxLoadCallbackOne);
                        //}
                        //else if (ReponseInfo.result == 60)
                        //{
                        //    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.NATION_LINGQUJIANGLI_TITLE);
                        //    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.GUO_JIA_TIPS);
                        //    _strContent2 = "";
                        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                        //                     UIBoxLoadCallbackOne);
                        //}


                        return true;
                    }
                    break;
            }
        }
        return false;
    }
    struct RewardInfo
    {
        public int type;
        public string count;
        public string icon;
    }
    private List<RewardInfo> _listAward = new List<RewardInfo>();
    void ShowReward(string s_reward)
    {
        index_num = 0;
        _listAward.Clear();
        if (s_reward.IndexOf('#') > -1)
        {
            string[] tempAwardList = s_reward.Split('#');
            for (int i = 0; i < tempAwardList.Length; i++)
            {
                string[] tempAwardItemInfo = tempAwardList[i].Split(':');
                RewardInfo rInfo = new RewardInfo();
                rInfo.type = int.Parse(tempAwardItemInfo[0]);
                rInfo.icon = tempAwardItemInfo[1];
                rInfo.count = tempAwardItemInfo[2];
                _listAward.Add(rInfo);
            }
        }
        else
        {
            string[] tempAwardItemInfo = s_reward.Split(':');
            RewardInfo rInfo = new RewardInfo();
            rInfo.type = int.Parse(tempAwardItemInfo[0]);
            rInfo.icon = tempAwardItemInfo[1];
            rInfo.count = tempAwardItemInfo[2];
            _listAward.Add(rInfo);
        }
        m_ObjectRewardParent.transform.localPosition = new Vector3(ParentPosOffset(_listAward.Count,112),-68,0);
        for (int i = 0; i < _listAward.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }
    private int ParentPosOffset(int count, int distance)
    {
        if (count % 2 == 0)
        {
            if (count / 2 > 1)
            {
                return -1 * distance / 2 * (count / 2 + 1);
            }
            else
            {
                return -1 * distance / 2 * count / 2;
            }
        }
        else
        {
            return -1 * distance * (count / 2);
        }
    }

    int index_num = 0;
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {

        if (m_ObjectRewardParent != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_ObjectRewardParent.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID(int.Parse(_listAward[index_num].icon), _listAward[index_num].count);
			iconSampleManager.SetIconPopText(int.Parse(_listAward[index_num].icon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(_listAward[index_num].icon)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(_listAward[index_num].icon)).descId));
            if (index_num < _listAward.Count - 1)
            {
                index_num++;
            }

            m_ObjectRewardParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    void TidyData(List<GuojiaRankInfo> data)
    {
        _listNationInfo.Clear();

        int size = data.Count;
        for (int i = 0; i < size; i++)
        {
            _listNationInfo.Add(data[i]);
        }

        if (_listitem.Count > 0)
        {
            for (int i = 0; i < _listNationInfo.Count; i++)
            {
                _listitem[i].name = _listNationInfo[i].rank.ToString();
                _listitem[i].GetComponent<NationItemManagerment>().ShowInfo(_listNationInfo[i]);
            }
            m_GridObject.repositionNow = true;
        }
        else
        {
            int size_ = _listNationInfo.Count;
            for (int k = 0; k < size_; k++)
            {
                CreateItem();
            }
        }
    }

    void CreateItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.NATION_RANK_ITEM),
                          ResLoaded);
    }
    private int _indexNum = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_GridObject != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _listNationInfo[_indexNum].rank.ToString();
            tempObject.transform.parent = m_GridObject.transform;
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.GetComponent<NationItemManagerment>().ShowInfo(_listNationInfo[_indexNum]);
            _listitem.Add(tempObject);
            if (_indexNum < _listNationInfo.Count - 1)
            {
                _indexNum++;
            }
        }
        else
        {
            p_object = null;
        }
        m_GridObject.repositionNow = true;
    }


    public void UIBoxLoadCallbackOne(ref WWW p_www, string p_path, Object p_object)
    {
        m_ListEvent[2].GetComponent<Collider>().enabled = true;
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        uibox.setBox(_strTitle, MyColorData.getColorString(1, _strContent1), MyColorData.getColorString(1, _strContent2), null, confirmStr, null, null);
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }

    private string ShowRewardInfo(string str)
    {
        string rewardInfo = "";
        if (str.IndexOf("#") > -1)
        {
            string[] str_All = str.Split('#');
            int _sizeA = str_All.Length;
            for (int i = 0; i < str_All.Length; i++)
            {
                string[] ss_item = str_All[i].Split(':');
                if (i == 0)
                {
                    rewardInfo = NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(ss_item[1])).nameId) + "x" + ss_item[2];
                }
                else
                {
                    rewardInfo += "、" + NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(ss_item[1])).nameId) + "x" + ss_item[2];
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                string[] ss_item = str.Split(':');
                rewardInfo = NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(ss_item[1])).nameId) + "x" + ss_item[2];
            }

        }
        return rewardInfo;
    }
}
