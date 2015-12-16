﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;
public class WorshipLayerManagerment : MonoBehaviour, SocketProcessor
{
    public List<EventSuoHandle> m_ListEvent;
    public List<GameObject> m_ListGameobjectShow;
    public List<UILabel> m_ListCount;
    public List<GameObject> m_ListSignal;
    public List<GameObject> m_listParent;
    public UIGrid m_YuJueSuffParent;
    public List<EventIndexHandle> m_listRewardEvent;
    public EventIndexHandle m_EventTouch;
    public GameObject m_MainParent;
    public GameObject m_Durable_UI;
    public GameObject m_RewardObj;
    public UIGrid m_RewardParent;

    public List<EventIndexHandle> m_ListCancelEvent;
    private MoBaiInfo worshipShow = new MoBaiInfo();
    private List<string> listInsufficientYuJue = new List<string>();

    public UISprite m_PopSprite;
    public UILabel m_PopLabel;
    public UILabel m_LabelTopUp;
 
    public GameObject m_TanHao;
    public ScaleEffectController m_SEC;
    public UIGrid m_Gride;
    public GameObject m_HidenInfo;

    public UIProgressBar m_ProgressBar;
    private int yuanBaoCost = 0;
    struct RewardYuJueInfo
    {
      public string icon;
      public string count;
    };
    private List<RewardYuJueInfo> _listReward = new List<RewardYuJueInfo>();
    private List<RewardYuJueInfo> listRewardYuJue = new List<RewardYuJueInfo>();
    private List<Vector3> listEffectPos = new List<Vector3>();
    private List<Vector3> lisMoveTarget = new List<Vector3>();
    private List<Vector3> lisMoveStart = new List<Vector3>();
    private List<Award> listReWard = new List<Award>();

    private bool isWorship = false;
    private int touchIndex = -1;
    private int _mobaiCount = 0;
    public struct YuJueShow
    {
        public int icon;
        public int color;
        public int count;

    };
    private List<YuJueShow> _listYJ = new List<YuJueShow>();
    private bool _isShowHide = false;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);

    }


    void Start()
    {
        m_ListEvent.ForEach(p => p.m_Handle += EventGet);
        
        m_listRewardEvent.ForEach(p => p.m_Handle += ShowReward);
        m_SEC.OpenCompleteDelegate += RequestData;
        m_ListCancelEvent.ForEach(p => p.m_Handle += HiddenReward);
    }

    void ShowInfo()
    {
        //m_LabelTopUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_SIGNAL);
        Vector3[] effect = { new Vector3(-265, -120, 0), new Vector3(28, -120, 0), new Vector3(350, -120, 0) };
        listEffectPos.AddRange(effect);

        Vector3[] moveT = { new Vector3(-228, -5, 0), new Vector3(66, -5, 0), new Vector3(350, -5, 0) };
        Vector3[] moveS = { new Vector3(-228, -150, 0), new Vector3(66, -150, 0), new Vector3(350, -150, 0) };
        lisMoveStart.AddRange(moveS);
        lisMoveTarget.AddRange(moveT);
   
        //ShowWorshipInfo();
        m_HidenInfo.SetActive(true);
         TidyInsufficientYuJueInfo();
    }
 
    void ShowReward(int index)
    {
        m_RewardObj.SetActive(true);
        switch (index)
        {
            case 0:
                {
                    if (_mobaiCount == LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes1)
                    {

                    }
                    else
                    {
                        TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(1).award1);
                    }
                }
                break;
            case 1:
                {
                    if (_mobaiCount == LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes2)
                    {

                    }
                    else
                    {
                        TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(1).award2);
                    }
                }
                break;
            case 2:
                {
                    if (_mobaiCount == LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3)
                    {

                    }
                    else
                    {
                        TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(1).award3);
                    }
                }
                break;
            default:
                break;
        }
    }

    void TuTengRewardData(string _award)
    {
        int size_all = m_RewardParent.transform.childCount;
        for (int i = 0;i < size_all; i++)
        {
            Destroy(m_RewardParent.transform.GetChild(i).gameObject);
        }

        _listReward.Clear();

        if (_award.IndexOf("#") > -1)
        {
            string[] ss = _award.Split('#');
            for (int j = 0; j < ss.Length; j++)
            {
                string[] award = ss[j].Split(':');
                RewardYuJueInfo reward = new RewardYuJueInfo();

                reward.icon =  award[1];

                reward.count =  award[2];
                _listReward.Add(reward);
            }
        }
        else
        {
            string[] award = _award.Split(':');
            RewardYuJueInfo reward = new RewardYuJueInfo();
            reward.icon = award[1];
            reward.count = award[2];
            _listReward.Add(reward);
        }
        CreateItem();
    }
    void HiddenReward(int index)
    {
        m_RewardObj.SetActive(false);
        int size = m_RewardParent.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(m_RewardParent.transform.GetChild(i).gameObject);
        }
    }
    void Update()
    {
        if (JunZhuData.Instance().m_RefreshCopperInfo)
        {
            JunZhuData.Instance().m_RefreshCopperInfo = false;
            if (GetCopperCoinWorshipEnable())
            {
                m_ListCount[0].text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
            }
            else
            {
                m_ListCount[0].text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
            }
        }
    }

    void EventGet(int index, GameObject gameobject)
    {
        touchIndex = index;
        switch ((WorshipButtonEnumManegernent)index)
        {
            case WorshipButtonEnumManegernent.E_WORSHIP_COMMON_WORSHIP:
                {
                    if (GetCopperCoinWorshipEnable())
                    {
                        ConnectServer(index);
                    }
                    else
                    {
                        JunZhuData.Instance().IsBuyTongBi = true;
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TIMES_REQ);
                    }
                }
                break;
            case WorshipButtonEnumManegernent.E_WORSHIP_PIOUS_WORSHIP:
                {
                    if (GetYuanBaoWorshipEnable())
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(7))
                        {
                            ConnectServer(index);
                        }
                        else
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad);
                        }
                    }
                    else
                    {
                        EquipSuoData.TopUpLayerTip(m_MainParent);
                    }
                }
                break;
            case WorshipButtonEnumManegernent.E_WORSHIP_DINGLI_WORSHIP:
                {
                    if (GetYuJueWorshipEnable())
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(11))
                        {
                            //Debug.Log("worshipShow.yuOpenworshipShow.yuOpen ::" + worshipShow.yuOpen);
                            if (worshipShow.yuOpen)
                            {
                                ConnectServer(index);
                            }
                            //else
                            //{
                            //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadDingLi);
                            //}
                        }
                        else
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad);
                        }
                    
                    }
                    else
                    {
                        InsufficientYuJueCreate();
                        m_ListSignal[2].SetActive(true);
                    }
                }
                break;
            case WorshipButtonEnumManegernent.E_WORSHIP_BUY_COPPER_COIN:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.yuanBao > yuanBaoCost)
                    {
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TongBi);
                    }
			        else 
                    {
                        m_ListSignal[0].SetActive(false);
                        EquipSuoData.TopUpLayerTip(m_MainParent);
                    }
                }
                break;
            default:
                break;
        }
    }

    //public void UIBoxLoadDingLi(ref WWW p_www, string p_path, Object p_object)
    //{
    //    GameObject boxObj = Instantiate(p_object) as GameObject;
    //    UIBox uibox = boxObj.GetComponent<UIBox>();
    //    string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
    //    string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
    //    string str = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_DINGLI_TIP);
    //    // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
    //    uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, titleFont, btn1Font);

    //}

   
    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str = "";
        if (touchIndex == 1)
        {
            str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + VipFuncOpenTemplate.GetNeedLevelByKey(7).ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
        }
        else
        {
            str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + VipFuncOpenTemplate.GetNeedLevelByKey(11).ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
        }
        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, null, null);

    }

    void RequestData()
    {
        _isShowHide = true;
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MoBai_Info);
    
    }

    void ConnectServer(int index)
    {
        m_ListEvent[index].GetComponent<Collider>().enabled = false;
        isWorship = true;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        MoBaiReq mobai = new MoBaiReq();
        mobai.cmd = index + 1;
        t_qx.Serialize(t_tream, mobai);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MoBai, ref t_protof);
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {

                case ProtoIndexes.S_MoBai_Info:/** 返回膜拜信息 **/
                    {
                        listReWard.Clear();
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        MoBaiInfo worship = new MoBaiInfo();

                        t_qx.Deserialize(t_tream, worship, worship.GetType());
                        worshipShow = worship;

                        if (touchIndex > 0)
                        {
                            m_ListEvent[touchIndex].GetComponent<Collider>().enabled = true;
                        }
                        if (touchIndex == 2 && worship.mobaiGain != null)
                        {
                            indexNum = 0;

                            for (int i = 0; i < worship.mobaiGain.awardsList.Count; i++)
                            {
                                listReWard.Add(worship.mobaiGain.awardsList[i]);
                            }
                            TidyRewardInfo();

                            m_ListSignal[3].SetActive(true);
                        }
                        else if (touchIndex >= 0)
                        {
                            isWorship = false;
                            if (touchIndex == 0)
                            {
                                m_TanHao.SetActive(false);
                                PushAndNotificationHelper.SetRedSpotNotification(400010, false);
                            }
                            WorshipMoveEffect(touchIndex);
                            RewardMoveEffect(touchIndex);
                        }
                        if (_isShowHide)
                        {
                            _isShowHide = false;
                            ShowInfo();
                        }
                        ShowWorshipInfo();
                        return true;
                    }


                case ProtoIndexes.IMMEDIATELY_JOIN_RESP:/** 返回领奖信息 **/
                    {
                        return true;
                    }
            }
        }
        return false;
    }

    public void GetRewardEffertMove()
    {
        WorshipMoveEffect(touchIndex);
        RewardMoveEffect(touchIndex);
    }
    void ShowWorshipInfo()
    {
        _mobaiCount = worshipShow.buffCount;
        MainCityUI.m_MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_TanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(400010));
        m_listRewardEvent[0].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes1 <= worshipShow.buffCount);
        m_listRewardEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes2 <= worshipShow.buffCount);
        m_listRewardEvent[2].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3 <= worshipShow.buffCount);
        m_ProgressBar.value = worshipShow.buffCount / float.Parse(LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3.ToString());
        m_ListCount[13].text = worshipShow.buffCount.ToString()+ "/" + LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3.ToString();
        m_ListGameobjectShow[6].GetComponent<UICamera>().enabled = true;
        if (GetCopperCoinWorshipEnable())
        {
            m_ListCount[0].text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
        }
        else
        {
            m_ListCount[0].text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());

        }
        ButtonEnable(0, worshipShow.tongBiOpen);
        if (!worshipShow.tongBiOpen)
        {
            m_ListGameobjectShow[0].SetActive(false);
            m_ListGameobjectShow[1].SetActive(true);
        }

        m_ListCount[1].text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(1).tili + NameIdTemplate.GetName_By_NameId(990036));
        if (GetYuanBaoWorshipEnable())
        {
            m_ListCount[2].text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        else
        {
            m_ListCount[2].text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        m_ListCount[3].text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(2).tili + NameIdTemplate.GetName_By_NameId(990036));


        if (!worshipShow.yuanBaoOpen)
        {
            m_ListGameobjectShow[2].SetActive(false);
            m_ListGameobjectShow[3].SetActive(true);
        }

        ButtonEnable(1, worshipShow.yuanBaoOpen);

        YuJueInfo();
    }

    void ButtonEnable(int index, bool ison)
    {
        if (ison)
        {
            if (m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>() == null)
            {
                m_ListEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>();
                m_ListEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>().enabled = false;
            }
            m_ListEvent[index].transform.GetComponent<Collider>().enabled = ison;
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().duration = 0.2f;
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
            m_ListEvent[index].transform.FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = NameIdTemplate.GetName_By_NameId(990037);
            m_ListEvent[index].GetComponent<Collider>().enabled = true;
            if (AllianceData.Instance.g_UnionInfo != null && AllianceData.Instance.g_UnionInfo.identity == 2)
            {
                m_ListEvent[index].transform.FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = NameIdTemplate.GetName_By_NameId(990049);
            }
        }
        else
        {
            if (m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>() == null)
            {
                m_ListEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>();
                m_ListEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>().enabled = false;
            }
            m_ListEvent[index].transform.GetComponent<Collider>().enabled = ison;
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().duration = 0.2f;
            m_ListEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
            //m_ListEvent[str].transform.FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = NameIdTemplate.GetName_By_NameId(990038);
            if (AllianceData.Instance.g_UnionInfo.identity == 2)
            {
                m_ListEvent[index].transform.FindChild("Background").FindChild("Label").GetComponent<UILabel>().text = NameIdTemplate.GetName_By_NameId(990049);
            }
            m_ListEvent[index].GetComponent<Collider>().enabled = false;
        }
    }

    void YuJueInfo()
    {
        ButtonEnable(2, worshipShow.yuOpen);
        _listYJ.Clear();
        if (!worshipShow.yuOpen)
        {
            m_ListGameobjectShow[4].SetActive(false);
            m_ListGameobjectShow[5].SetActive(true);
            m_ListCount[9].text = "VIP" + JunZhuData.Instance().m_junzhuInfo.vipLv.ToString() + NameIdTemplate.GetName_By_NameId(990054) + VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).yujueDuihuan.ToString() + NameIdTemplate.GetName_By_NameId(990040);
        }

        if (m_Gride.transform.childCount > 0)
        {
            int childCount = m_Gride.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(m_Gride.transform.GetChild(i).gameObject);
            }
        }
        YuJueShow yj0 = new YuJueShow();
        yj0.icon = 950002;
        yj0.count = BagData.Instance().GetCountByItemId(950002);
        if (BagData.Instance().GetCountByItemId(950002) >= 1)
        {
            yj0.color = 1;
           // m_ListCount[4].text = MyColorData.getColorString(1, "1");
        }
        else
        {
            yj0.color = 5;
         //   m_ListCount[4].text = MyColorData.getColorString(5, "1");
        }
        _listYJ.Add(yj0);

        YuJueShow yj1 = new YuJueShow();
        yj1.icon = 950005;
        yj1.count = BagData.Instance().GetCountByItemId(950005);
        if (BagData.Instance().GetCountByItemId(950005) >= 1)
        {
            yj1.color = 1;
           // m_ListCount[5].text = MyColorData.getColorString(1, "1");
        }
        else
        {
            yj1.color = 5;
          //  m_ListCount[5].text = MyColorData.getColorString(5, "1");
        }
        _listYJ.Add(yj1);
        YuJueShow yj2 = new YuJueShow();
        yj2.icon = 950001;
        yj2.count = BagData.Instance().GetCountByItemId(950001);
        if (BagData.Instance().GetCountByItemId(950001) >= 1)
        //{
        //    m_ListCount[6].text = MyColorData.getColorString(1, "1");
        //}
        //else
        //{
        //    m_ListCount[6].text = MyColorData.getColorString(5, "1");
        //}
        {
            yj2.color = 1;
            // m_ListCount[5].text = MyColorData.getColorString(1, "1");
        }
        else
        {
            yj2.color = 5;
            //  m_ListCount[5].text = MyColorData.getColorString(5, "1");
        }
        _listYJ.Add(yj2);
        YuJueShow yj3 = new YuJueShow();
        yj3.icon = 950004;
        yj3.count = BagData.Instance().GetCountByItemId(950004);
        if (BagData.Instance().GetCountByItemId(950004) >= 1)
        //{
        //    m_ListCount[7].text = MyColorData.getColorString(1, "1");
        //}
        //else
        //{
        //    m_ListCount[7].text = MyColorData.getColorString(5, "1");
        //}
        {
            yj3.color = 1;
            // m_ListCount[5].text = MyColorData.getColorString(1, "1");
        }
        else
        {
            yj3.color = 5;
            //  m_ListCount[5].text = MyColorData.getColorString(5, "1");
        }
        _listYJ.Add(yj3);
        YuJueShow yj4 = new YuJueShow();
        yj4.icon = 950003;
        yj4.count = BagData.Instance().GetCountByItemId(950003);
        if (BagData.Instance().GetCountByItemId(950003) >= 1)

        {
            yj4.color = 1;
            // m_ListCount[5].text = MyColorData.getColorString(1, "1");
        }
        else
        {
            yj4.color = 5;
            //  m_ListCount[5].text = MyColorData.getColorString(5, "1");
        }
        _listYJ.Add(yj4);
        _indexNum = 0;
        for (int i = 0; i < 5; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                       ResLoaded);
        }
    }


    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }

    private bool GetCopperCoinWorshipEnable()
    {
        return JunZhuData.Instance().m_junzhuInfo.jinBi >= LianmengMoBaiTemplate.GetShowInfoByType(1).needNum ? true : false;
    }

    private bool GetYuanBaoWorshipEnable()
    {
        return JunZhuData.Instance().m_junzhuInfo.yuanBao >= LianmengMoBaiTemplate.GetShowInfoByType(2).needNum ? true : false;
    }

    private bool GetYuJueWorshipEnable()
    {
        if (BagData.Instance().GetCountByItemId(950001) >= 1)
        {
            if (BagData.Instance().GetCountByItemId(950002) >= 1)
            {
                if (BagData.Instance().GetCountByItemId(950003) >= 1)
                {
                    if (BagData.Instance().GetCountByItemId(950004) >= 1)
                    {
                        if (BagData.Instance().GetCountByItemId(950005) >= 1)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
        
        //return BagData.Instance().GetCountByItemId(950001) >= 1 && BagData.Instance().GetCountByItemId(950002) >= 1 && BagData.Instance().GetCountByItemId(950003) >= 1 && BagData.Instance().GetCountByItemId(950004) >= 1 && BagData.Instance().GetCountByItemId(950005) >= 1;
    }

    int row = 5;
    Vector3 Startpos ;
    Vector3 Startpos2;
    int indexNum = 0;
    float pos_x = 0;
    void RewardYuJueCreate()
    {
        if (m_listParent[1].transform.childCount > 0)
        {
            int childCount = m_listParent[1].transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(m_listParent[1].transform.GetChild(i).gameObject);
            }
        }
        int size = listReWard.Count;
       // Startpos = new Vector3(-130 + (row - size)*70, 0, 0);
        //pos_x = Startpos.x; 
       
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
       ResLoaded3);
           
        }
    }
 

    int indexNum2 = 0;
    float pos_x2 = 0;
    void InsufficientYuJueCreate()
    {
        if (m_YuJueSuffParent.transform.childCount > 0)
        {
            int childCount = m_YuJueSuffParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(m_YuJueSuffParent.transform.GetChild(i).gameObject);
            }
        }
        int size = listInsufficientYuJue.Count;
        m_YuJueSuffParent.transform.localPosition = new Vector3(ParentPosOffset(listInsufficientYuJue.Count, 56), 20, 0);
        indexNum2 = 0;
        for (int i = 0; i < listInsufficientYuJue.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                   ResLoaded2);
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
    void TidyInsufficientYuJueInfo()
    {

        if (BagData.Instance().GetCountByItemId(950001) < 1)
        {
          listInsufficientYuJue.Add("950001");
        }
         if(BagData.Instance().GetCountByItemId(950002)< 1 )
        {
          listInsufficientYuJue.Add("950002");
        }
         if(BagData.Instance().GetCountByItemId(950003) <1 )
        {
          listInsufficientYuJue.Add("950003");
        }
         if(BagData.Instance().GetCountByItemId(950004) < 1 )
        {
          listInsufficientYuJue.Add("950004");
        }
         if(BagData.Instance().GetCountByItemId(950005) < 1 )
        {
          listInsufficientYuJue.Add("950005");
        }
    }

    void TidyRewardInfo()
    {
      //  listRewardYuJue.Clear();
      // string award =  LianmengMoBaiTemplate.GetShowInfoByType(3).award;
      // string[] yujueInfo ;
      
      // yujueInfo = award.Split('#');
   
      //for (int i = 0; i < yujueInfo.Length; i++)
      //{
      //    Debug.Log(yujueInfo[i]);
      //    string[] ss = yujueInfo[i].Split(':');
      //    RewardYuJueInfo ryji = new RewardYuJueInfo();
      //    ryji.icon = ss[1];
      //    ryji.count = ss[2];
      //    listRewardYuJue.Add(ryji);
      //}
      RewardYuJueCreate();
    }
    int indexNum3 = 0;
    void RewardMoveEffect(int index)
    {
        indexNum3 = index;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_EFFECT_ITEM), ResourcesLoadCallBack3);
    }

    public void ResourcesLoadCallBack3(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.transform.parent = m_listParent[2].transform;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localPosition = listEffectPos[indexNum3];
        tempObject.GetComponent<WorshipMoveItemManagerment>().ShowInfo(listEffectPos[indexNum3]);
    }

    int index_Num4 = 0;

    void WorshipMoveEffect(int index)
    {
        index_Num4 = index;
        //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_REWARD_MOVE_ITEM), ResourcesLoadCallBack4);
        FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengMoBaiTemplate.GetShowInfoByType(index_Num4 + 1).awardShow);
        //LianmengMoBaiTemplate.GetShowInfoByType(index_Num4 + 1)
    }

    public void ResourcesLoadCallBack4(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.transform.parent = m_listParent[2].transform;
        tempObject.transform.localScale = Vector3.one;

        tempObject.GetComponent<WorshipEffertMoveManagerment>().ShowInfo(LianmengMoBaiTemplate.GetShowInfoByType(index_Num4 + 1).tili.ToString(), LianmengMoBaiTemplate.GetShowInfoByType(index_Num4 + 1).gongxian.ToString(), lisMoveStart[index_Num4], lisMoveTarget[index_Num4]);
    }
    private int _indexNum = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_Gride != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_Gride.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            
            iconSampleManager.SetIconByID(_listYJ[_indexNum].icon, _listYJ[_indexNum].count.ToString(), 10);

            iconSampleManager.SetIconPopText(_listYJ[_indexNum].icon, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listYJ[_indexNum].icon).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listYJ[_indexNum].icon).descId));

            tempObject.transform.localScale = Vector3.one*0.5f;

            if (_indexNum < _listYJ.Count - 1)
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


    void ResLoaded2(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_YuJueSuffParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_YuJueSuffParent.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(int.Parse(listInsufficientYuJue[indexNum2]), "1",8);

            iconSampleManager.SetIconPopText(int.Parse(listInsufficientYuJue[indexNum2]), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listInsufficientYuJue[indexNum2])).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listInsufficientYuJue[indexNum2])).descId));

            tempObject.transform.localScale = Vector3.one*0.5f;
            if (indexNum2 < listInsufficientYuJue.Count - 1)
            {
                indexNum2++;
            }
            m_YuJueSuffParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }

    }

    void ResLoaded3(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_listParent[1] != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_listParent[1].transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(listReWard[indexNum].itemId, listReWard[indexNum].itemNumber.ToString());

            iconSampleManager.SetIconPopText(listReWard[indexNum].itemId, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listReWard[indexNum].itemId).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listReWard[indexNum].itemId).descId));

            tempObject.transform.localScale = Vector3.one*0.5f;

            if (indexNum < listReWard.Count - 1)
            {
                indexNum++;
            }
            m_listParent[1].GetComponent<UIGrid>().repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }


    void CreateItem()
    {
        int size_ReWard = _listReward.Count;
        _indexNum22 = 0;
        m_RewardParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(size_ReWard - 1, 108), 0, 0);
        for (int i = 0; i < size_ReWard; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                              ResLoaded22);
        }
    }
    private int _indexNum22 = 0;
    void ResLoaded22(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_RewardParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.name = _listReward[_indexNum22].icon;
            tempObject.transform.parent = m_RewardParent.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(int.Parse(_listReward[_indexNum22].icon), _listReward[_indexNum22].count.ToString(), 12);
            iconSampleManager.SetIconPopText(int.Parse(_listReward[_indexNum22].icon)
                , NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(_listReward[_indexNum22].icon)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(_listReward[_indexNum22].icon)).descId));
            tempObject.transform.localScale = Vector3.one * 0.9f;
        
            if (_indexNum22 < _listReward.Count - 1)
            {
                _indexNum22++;
            }
            m_RewardParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    private void Ward(GameObject obj)
    {
        if ( int.Parse(obj.name) == 0)
        {

        }

    }

    private int ShowMaskInfo(int index)
    {
        int index_num = 0;
        if (index / 99.0f < 0.5f)
        {
            index_num++;
        }

        return 0;
    }
}
