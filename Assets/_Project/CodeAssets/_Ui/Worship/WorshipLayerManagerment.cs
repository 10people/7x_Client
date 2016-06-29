using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;
public class WorshipLayerManagerment : MonoBehaviour, SocketProcessor, UI2DEventListener
{
    public List<EventPressIndexHandle> m_ListEvent;
    public static int m_bulidingLevel = 0;
    public List<GameObject> m_ListGameobjectShow;
    public List<UILabel> m_ListCount;
    public List<GameObject> m_ListSignal;
    public List<GameObject> m_listParent;
    public UIGrid m_YuJueSuffParent;
    public List<WorshipStepAwardManangerment> m_listRewardEvent;
 
    public List<WorshipMoBaiItemManagerment> m_listMoBaiItem;
    public List<WorshipDonateItemManagerment> m_listDonateItem;

    public GameObject m_MainParent;
    public GameObject m_Durable_UI;
    public GameObject m_RewardObj;
    public GameObject m_YuJueParent;
    public UIGrid m_RewardParent;
    public List<EventIndexHandle> m_ListTypeEvent;
    public List<EventIndexHandle> m_ListCancelEvent;

    public UISprite m_SpriteV;
    public UILabel m_LabV;

    public UISprite m_SpriteV_DonateLeft;
    public UILabel m_LabV_DonateLeft;
    public UISprite m_SpriteV_DonateRight;
    public UILabel m_LabV_DonateRight;
    private MoBaiInfo worshipShow = null;
    private List<string> listInsufficientYuJue = new List<string>();

    public UISprite m_PopSprite;
    public UILabel m_PopLabel;
    public UILabel m_LabelTopUp;
    public UILabel m_LabelSignal;
    public GameObject m_TanHao;
    public GameObject m_YuanBaoTanHao;
    public GameObject m_YuJueTanHao;

    public ScaleEffectController m_SEC;
    public UIGrid m_Gride;
    public GameObject m_HidenInfo;
    public UILabel m_LabWorshipCount;
 
    public struct StepReward
    {
        public int stepState;//今日已经领了几个阶段的奖励，取值0/1/2，0表示不可领1表示可领2已经领取
        public int priviousCount;
    };
    private List<StepReward> _listStepInfo = new List<StepReward>();


    public GameObject m_MoBai;
    public GameObject m_FengChan;


    public GameObject m_DaDianTanHao;
    public GameObject m_ShengDianTanHao;

    public GameObject m_TanHaoMoBai;
    //public GameObject m_TanHaoFengShan;
    public UIProgressBar m_ProgressBar;

    public List<GameObject> m_listMoBaiPiao;
    public List<GameObject> m_listFengShanPiao;

    public List<UILabel> m_listLabObjMoBai;
    public List<UILabel> m_listLabObjFengShan;
    public GameObject m_ObjTopLeft;
    public UILabel m_LabDingLiSignal;
  

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
 
    private int touchIndex = -1;
    private int _mobaiCount = 0;
    private Vector3 vec_pos = new Vector3(332, 235, 0);
    public List<Vector3> _listDonatePos;
    public struct YuJueShow
    {
        public int icon;
        public int color;
        public int count;
    };
    private List<YuJueShow> _listYJ = new List<YuJueShow>();
    private bool _isShowHide = false;
    private bool _isGetReward = false;
    private int _TouchRewardNum = 0;
    private bool _isopen = false;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    public void OnUI2DShow()
    {

        if (_isopen)
        {
            if (_DonateTemple != null)
            {
                ShowLabInfo(_DonateTemple);
            }

            ShowWorshipInfo();
        }
    }
    void Start()
    {
        Vector3[] vec =  { new Vector3(-142,-240,0), new Vector3(260,-239,0)};
        _listDonatePos.AddRange(vec);
        m_SpriteV.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey(7);

        m_SpriteV_DonateLeft.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey(27);
        m_SpriteV_DonateRight.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey(26);
        if (FreshGuide.Instance().IsActive(400030) && TaskData.Instance.m_TaskInfoDic.ContainsKey(400030) 
            && TaskData.Instance.m_TaskInfoDic[400030].progress >= 0)
        {
            //CityGlobalData.m_isRightGuide = false;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400030];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        else
        {
            UIYindao.m_UIYindao.CloseUI();
            // UIYindao.m_UIYindao.CloseJiantou();
        }
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "联盟图腾", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_ListTypeEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
       // m_LabelSignal.text = LanguageTemplate.GetText(1520);
        m_ListEvent.ForEach(p => p.m_Handle += EventGet);
        m_listRewardEvent.ForEach(p => p.m_TouchLQEvent.m_Handle += RewardGet);
        m_listRewardEvent.ForEach(p => p.OnLongPress += ShowReward);
        m_listRewardEvent.ForEach(p => p.OnLongPressFinish += LongFinish);
        m_ListTypeEvent.ForEach(p => p.m_Handle += TypeInfo);
        //m_listRewardEvent.ForEach(p => p.m_TouchEvent.m_Handle += ShowReward);
        m_SEC.OpenCompleteDelegate += RequestData;
        m_ListCancelEvent.ForEach(p => p.m_Handle += HiddenReward);
        m_listDonateItem.ForEach(p => p.m_Event.m_Handle += Donate);
    }

 
    void Donate(int index)
    {
  
        if (_DonateTemple.fsInfo[index].usedTimes >= _DonateTemple.fsInfo[index].totalTimes
            || JunZhuData.Instance().m_junzhuInfo.yuanBao < _DonateTemple.fsInfo[index].needYuanBao
            || JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(27) && index == 0
            || JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(26) && index == 1)
        {
            switch (index)
            {
                case 0:
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(27))
                        {
         
                            Global.CreateFunctionIcon(1901);
                        }
                        else if (_DonateTemple.fsInfo[index].usedTimes >= _DonateTemple.fsInfo[index].totalTimes)
                        {
                         Global.CreateBox("提示", "今日捐献次数已用光，提升V特权可以增加捐献次数", "", null,"确定" , "前往充值", ToAccount);
                            //ClientMain.m_UITextManager.createText("没有次数！");
                        }
                        else
                        {
                            Global.CreateFunctionIcon(101);
                        }
                    }
                    break;
                case 1:
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(26))
                        {
                        
                            Global.CreateFunctionIcon(1901);
                        }
                        else if (_DonateTemple.fsInfo[index].usedTimes >= _DonateTemple.fsInfo[index].totalTimes)
                        {
                            Global.CreateBox("提示", "今日捐献次数已用光，提升V特权可以增加捐献次数", "", null,"确定" , "前往充值", ToAccount);
                         //   EquipSuoData.TopUpLayerTip(null,false,0,"今日捐献次数已用光，提升V特权可以增加捐献次数", true);
                          //  ClientMain.m_UITextManager.createText("没有次数！");
                        }
                        else
                        {
                            Global.CreateFunctionIcon(101);
                        }
                    }
                    break;
            }
          

        }
        else
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            FengShanReq fens = new FengShanReq();
            fens.confId = LianmengJuanxianTemplate.getJuanxianById(_DonateTemple.fsInfo[index].confId).id;
            t_qx.Serialize(t_tream, fens);
            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DO_ALLIANCE_FENGSHAN_REQ, ref t_protof,false, p_receiving_wait_proto_index: ProtoIndexes.S_ALLIANCE_FENGSHAN_RESP);
            //_DonateTemple.fsInfo[index].usedTimes++;
            m_listDonateItem[index].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false, 1);
            //if (_DonateTemple.fsInfo[index].totalTimes > 0)
            //{
            //    m_listDonateItem[index].m_LabTimes.text = MyColorData.getColorString(5, "今日剩余次数 : "
            //                                          + (_DonateTemple.fsInfo[index].totalTimes - _DonateTemple.fsInfo[index].usedTimes)
            //                                          + "/" + _DonateTemple.fsInfo[index].totalTimes);
            //}
            //JunZhuData.Instance().m_junzhuInfo.yuanBao -= _DonateTemple.fsInfo[index].needYuanBao;
            //m_Durable_UI.transform.GetChild(0).GetComponent<MainCityBelongings>().m_yuanbaoNuM.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
            //    _DonateTemple.fsInfo[index].needYuanBao = PurchaseTemplate.GetPurchaseTempByTypeAndTimes(LianmengJuanxianTemplate.getJuanxianById(_DonateTemple.fsInfo[index].confId).type
            //       , _DonateTemple.fsInfo[index].totalTimes).price;
            //MoveActionAlong(m_listFengShanPiao[index].transform);
            //FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengJuanxianTemplate.getJuanxianById(_DonateTemple.fsInfo[index].confId).award);
            //ShowLabInfo(_DonateTemple);
            //if (_DonateTemple.fsInfo[index].totalTimes - _DonateTemple.fsInfo[index].usedTimes == 0)
            //{
            //    m_listDonateItem[index].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
            //}
        }

    }

    void ToAccount(int index)
    {
        if (index == 2)
        {
            RechargeData.Instance.RechargeDataReq();
        }
    }
    void ShowInfo()
    {

        if (GoldWorship() && worshipShow.tongBiOpen)
        {
            m_listMoBaiItem[0].gameObject.SetActive(true);
            m_listMoBaiItem[0].m_LabDes.text = "膜拜";
        }
        else if (!worshipShow.tongBiOpen)
        {

            m_listMoBaiItem[0].m_ButtonBack.GetComponent<UIButton>().enabled = false;
            m_listMoBaiItem[0].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false,1);
            m_listMoBaiItem[0].m_LabDes.text = "膜拜";
        }
        //else
        //{
        //    m_ListEvent[0].gameObject.SetActive(false);
        //    m_listMoBaiItem[0].m_LabDes.text = LanguageTemplate.GetText(20011)
        //                              + LianmengMoBaiTemplate.GetShowInfoByType(1).tuTenglvNeeded.ToString()
        //                              + LanguageTemplate.GetText(20012);
        //}



        //if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(7) && worshipShow.yuanBaoOpen)
        //{

        //    m_listMoBaiItem[1].m_ButtonBack.GetComponent<UIButton>().enabled = false;
        //    m_listMoBaiItem[1].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
        //    m_listMoBaiItem[1].m_LabDes.text = "V"
        //                              + VipFuncOpenTemplate.GetNeedLevelByKey(7).ToString()
        //                              + "开启";
        //}
        //else 
        if (!worshipShow.yuanBaoOpen)
        {

            m_listMoBaiItem[1].m_ButtonBack.GetComponent<UIButton>().enabled = false;
            m_listMoBaiItem[1].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false,1);
            m_listMoBaiItem[1].m_LabDes.text = "膜拜";
        }

        else
        {

            m_listMoBaiItem[1].m_ButtonBack.GetComponent<UIButton>().enabled = true;
            m_listMoBaiItem[1].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
            m_listMoBaiItem[1].m_LabDes.text = "膜拜";

        }


        //if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(11) && worshipShow.yuOpen)
        //{

        //    m_listMoBaiItem[2].m_ButtonBack.GetComponent<UIButton>().enabled = false;
        //    m_listMoBaiItem[2].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);

        //    m_listMoBaiItem[2].m_LabDes.text = "V"
        //                                         + VipFuncOpenTemplate.GetNeedLevelByKey(11).ToString()
        //                                              + "开启";
        //}
        //else
        if (!worshipShow.yuOpen)
        {
            m_listMoBaiItem[2].m_ButtonBack.GetComponent<UIButton>().enabled = false;
            m_listMoBaiItem[2].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false,1);
            m_listMoBaiItem[2].m_LabDes.text = "膜拜";
        }
        else
        {
          m_listMoBaiItem[2].m_ButtonBack.GetComponent<UIButton>().enabled = true;
            m_listMoBaiItem[2].m_ButtonBack.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
            m_listMoBaiItem[2].m_LabDes.text = "膜拜";
        }
        int size_1 = m_listLabObjFengShan.Count;
        Vector3[] effect = { new Vector3(-265, -120, 0), new Vector3(28, -120, 0), new Vector3(350, -120, 0) };
        listEffectPos.AddRange(effect);

        Vector3[] moveT = { new Vector3(-228, -5, 0), new Vector3(66, -5, 0), new Vector3(350, -5, 0) };
        Vector3[] moveS = { new Vector3(-228, -150, 0), new Vector3(66, -150, 0), new Vector3(350, -150, 0) };
        lisMoveStart.AddRange(moveS);
        lisMoveTarget.AddRange(moveT);

        m_HidenInfo.SetActive(true);
        TidyInsufficientYuJueInfo();
    }
    int type_Touch_index = 0;
    void TypeInfo(int index)
    {
        touchIndex = 0;
        if (type_Touch_index != index)
        {
          //  m_LabelSignal.text = LanguageTemplate.GetText(1520);
            if (index == 0)
            {
              //  m_LabelSignal.text = LanguageTemplate.GetText(1520);
                m_MoBai.SetActive(true);
                m_FengChan.SetActive(false);
            }
            else
            {
                if (!FunctionOpenTemp.GetWhetherContainID(400017))
                {
                    Global.CreateBox("提示",FunctionOpenTemp.GetTemplateById(400017).m_sNotOpenTips,"",null,"确定",null,null,null);
                    EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                     , FunctionOpenTemp.GetTemplateById(400017).m_sNotOpenTips
                     , "");
                    return;
                }
            //    m_LabelSignal.text = LanguageTemplate.GetText(1603);
                m_ListTypeEvent[index - 1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                m_ListTypeEvent[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                m_MoBai.SetActive(false);
                m_FengChan.SetActive(true);
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_FENGSHAN_REQ);

            }
            m_ListTypeEvent[type_Touch_index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
            m_ListTypeEvent[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
            type_Touch_index = index;
        }

    }
    void LongFinish(GameObject obj)
    {
        m_RewardObj.SetActive(false);
        int size = m_RewardParent.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            Destroy(m_RewardParent.transform.GetChild(i).gameObject);
        }
    }

    void ShowReward(GameObject obj)
    {
        if (obj.name.IndexOf('_') > -1)
        {
            string[] ss = obj.name.Split('_');
            switch (int.Parse(ss[1]))
            {
                case 0:
                    {

                        {
                            TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award1);
                        }
                    }
                    break;
                case 1:
                    {

                        {
                            TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award2);
                        }
                    }
                    break;
                case 2:
                    {
                        {
                            TuTengRewardData(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award3);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void RewardGet(int index)
    {
  

        //  if (obj.name.IndexOf('_') > -1)
        {
 
            switch (index)
            {
                case 0:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes1 )
                        {
                            GetReward(index + 1);
                        }
                    }
                    break;
                case 1:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes2 )
                        {
                            GetReward(index + 1);
                        }
                    }
                    break;
                case 2:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3 )
                        {
                            GetReward(index + 1);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void GetReward(int index)
    {
        _TouchRewardNum = index;
        _isGetReward = true;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        MoBaiReq tempRequest = new MoBaiReq();
        tempRequest.cmd = index;
        t_qx.Serialize(t_tream, tempRequest);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_MOBAI_AWARD, ref t_protof,false, p_receiving_wait_proto_index: ProtoIndexes.S_MoBai_Info);
    }
    void TuTengRewardData(string _award)
    {
        int size_all = m_RewardParent.transform.childCount;
        for (int i = 0; i < size_all; i++)
        {
            Destroy(m_RewardParent.transform.GetChild(i).gameObject);
        }
        m_RewardParent.repositionNow = true;
        _listReward.Clear();

        if (_award.IndexOf("#") > -1)
        {
            string[] ss = _award.Split('#');
            for (int j = 0; j < ss.Length; j++)
            {
                string[] award = ss[j].Split(':');
                RewardYuJueInfo reward = new RewardYuJueInfo();

                reward.icon = award[1];

                reward.count = award[2];
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
        m_RewardParent.transform.localPosition = Vector3.zero;
    }
    void Update()
    {
        if (FreshGuide.Instance().IsActive(400030) && TaskData.Instance.m_TaskInfoDic.ContainsKey(400030)
            && TaskData.Instance.m_TaskInfoDic[400030].progress < 0)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        //if (JunZhuData.Instance().m_RefreshCopperInfo)
        //{
        //    JunZhuData.Instance().m_RefreshCopperInfo = false;
        //    if (GetCopperCoinWorshipEnable())
        //    {
        //        m_ListCount[0].text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
        //    }
        //    else
        //    {
        //        m_ListCount[0].text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
        //    }
        //}

        m_TanHaoMoBai.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(400010)
                                           || PushAndNotificationHelper.IsShowRedSpotNotification(400012)
                                           || PushAndNotificationHelper.IsShowRedSpotNotification(400015));

        //m_TanHaoFengShan.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(300300)
        //                         || PushAndNotificationHelper.IsShowRedSpotNotification(400017));
    }

    void EventGet(int index)
    {
        if (JunZhuData.Instance().m_junzhuInfo.tili >= 999 && !FreshGuide.Instance().IsActive(400030))
        {
            EquipSuoData.ShowSignal(null, LanguageTemplate.GetText(800));
            return;

        }
        touchIndex = index;
        switch ((WorshipButtonEnumManegernent)index)
        {
            case WorshipButtonEnumManegernent.E_WORSHIP_COMMON_WORSHIP:
                {
                    if (FreshGuide.Instance().IsActive(400030) && TaskData.Instance.m_TaskInfoDic[400030].progress >= 0)
                    {
 
                        ConnectServer(index);
                    }
                    else if (GetCopperCoinWorshipEnable() && worshipShow.tongBiOpen)
                    {
                        ConnectServer(index);
                    }
                    else
                    {
                        if (!worshipShow.tongBiOpen)
                        {
                            ClientMain.m_UITextManager.createText("今日已膜拜");
                        }
                        else if (!GetCopperCoinWorshipEnable())
                        {

                            Global.CreateFunctionIcon(501);
                        }
                    }
                }
                break;
            case WorshipButtonEnumManegernent.E_WORSHIP_PIOUS_WORSHIP:
                {
                    if (GetYuanBaoWorshipEnable() && worshipShow.yuanBaoOpen 
                        && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(7))
                    {
                       
                            ConnectServer(index);
                      
                        {
                         
                         //   Global.CreateFunctionIcon(1901);
                        }
                    }
                    else
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(7))
                        {
                            Global.CreateFunctionIcon(1901);
                        }
                        else if (!worshipShow.yuanBaoOpen)
                        {
                            ClientMain.m_UITextManager.createText("今日已膜拜");
                        }
                        else if(!GetYuanBaoWorshipEnable())
                        {
                            Global.CreateFunctionIcon(101);
                        }
                        
                        
                    }
                }
                break;
            case WorshipButtonEnumManegernent.E_WORSHIP_DINGLI_WORSHIP:
                {
                    if (GetYuJueWorshipEnable())
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(11))
                        {
                            if (worshipShow.yuOpen)
                            {
                                ConnectServer(index);
                            }
                            else
                            {
                                ClientMain.m_UITextManager.createText("今日已膜拜");
                            }
                            //else
                            //{
                            //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadDingLi);
                            //}
                        }
                        else
                        {
                            touchIndex = -1;

                            Global.CreateFunctionIcon(1901);
                        }

                    }
                    else
                    {
                        Global.CreateFunctionIcon(1601);
                        //InsufficientYuJueCreate();
                        //m_ListSignal[2].SetActive(true);
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
                        EquipSuoData.TopUpLayerTip(m_MainParent, true);
                    }
                }
                break;
            default:
                break;
        }
    }
    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str = "";
        if (touchIndex == 1)
        {
            str = LanguageTemplate.GetText(600)
                + VipFuncOpenTemplate.GetNeedLevelByKey(7).ToString()
                + LanguageTemplate.GetText(603) + "\n\n" + LanguageTemplate.GetText(700);
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
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MoBai_Info,false, p_receiving_wait_proto_index: ProtoIndexes.S_MoBai_Info);

    }

    void ConnectServer(int index)
    {
        //m_ListEvent[index].GetComponent<Collider>().enabled = false;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        MoBaiReq mobai = new MoBaiReq();
        mobai.cmd = index + 1;
        t_qx.Serialize(t_tream, mobai);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        //SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MoBai, ref t_protof);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MoBai, ref t_protof,false, p_receiving_wait_proto_index: ProtoIndexes.S_MoBai_Info);
    }
    FengShanInfoResp _DonateTemple = null;
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
                        _isopen = true;
                        t_qx.Deserialize(t_tream, worship, worship.GetType());
                        if (worshipShow == null)
                        {
                            worshipShow = worship;
                        }
                        else if(!_isGetReward)
                        {

                            if ((worshipShow.tongBiOpen == worship.tongBiOpen
                                && worshipShow.yuanBaoOpen == worship.yuanBaoOpen
                                && worshipShow.yuOpen == worship.yuOpen )|| worshipShow.yuOpen != worship.yuOpen)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengMoBaiTemplate.GetShowInfoByType(3).awardShow);
                                //WorshipMoveEffect(2);
                                RewardMoveEffect(2);
                            }
                            else if (worshipShow.tongBiOpen != worship.tongBiOpen)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengMoBaiTemplate.GetShowInfoByType(1).awardShow);
                             //   WorshipMoveEffect(0);
                                RewardMoveEffect(0);
                            }
                            else if (worshipShow.yuanBaoOpen != worship.yuanBaoOpen)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengMoBaiTemplate.GetShowInfoByType(2).awardShow);
                              //  WorshipMoveEffect(1);
                                RewardMoveEffect(1);
                            }
                        }

                        worshipShow = worship;
                        _listStepInfo.Clear();
                        PushAndNotificationHelper.SetRedSpotNotification(400010, worship.tongBiOpen && GetCopperCoinWorshipEnable());
                        PushAndNotificationHelper.SetRedSpotNotification(400012, (worship.yuanBaoOpen && YuanBaoWorship()
                                     && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(7) && GetYuanBaoWorshipEnable()));
                        PushAndNotificationHelper.SetRedSpotNotification(400015, (worship.yuOpen && YuJueWorship() && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(11) && YuJueIsEnough()));
                        MainCityUI.SetRedAlert(400010, worship.tongBiOpen && GetCopperCoinWorshipEnable());
                        MainCityUI.SetRedAlert(400012, (worship.yuanBaoOpen && YuanBaoWorship() && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(7) && GetYuanBaoWorshipEnable()));
                        MainCityUI.SetRedAlert(400015, (worship.yuOpen && YuJueWorship() && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(11) && YuJueIsEnough()));

                        m_YuanBaoTanHao.SetActive(worship.yuanBaoOpen && PushAndNotificationHelper.IsShowRedSpotNotification(400012) && YuanBaoWorship() && GetYuanBaoWorshipEnable());
                        m_YuJueTanHao.SetActive(worship.yuOpen && YuJueIsEnough() && PushAndNotificationHelper.IsShowRedSpotNotification(400015) && YuJueWorship());
                        m_TanHao.SetActive(worship.tongBiOpen && GetCopperCoinWorshipEnable());


                        StepReward sr0 = new StepReward();
                        sr0.stepState = worship.bigStep0;
                        sr0.priviousCount = worship.buffCount;
                        _listStepInfo.Add(sr0);

                        StepReward sr1 = new StepReward();
                        sr1.stepState = worship.bigStep1;
                        sr1.priviousCount = worship.buffCount;
                        _listStepInfo.Add(sr1);

                        StepReward sr2 = new StepReward();
                        sr2.stepState = worship.bigStep2;
                        sr2.priviousCount = worship.buffCount;
                        _listStepInfo.Add(sr2);
                        StepRewardShow();
                        if (_isGetReward)
                        {
                            _isGetReward = false;
                            if (_TouchRewardNum == 1)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award1);
                            }
                            else if (_TouchRewardNum == 2)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award2);
                            }
                            else if (_TouchRewardNum == 3)
                            {
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).award3);
                            }
                        }
                 
                        {

                            ShowInfo();
                        }

                       // 
                        ShowWorshipInfo();
                        return true;
                    }

                case ProtoIndexes.S_ALLIANCE_FENGSHAN_RESP:/** 封禅信息 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        FengShanInfoResp FengShan = new FengShanInfoResp();
                        t_qx.Deserialize(t_tream, FengShan, FengShan.GetType());

                        if (FengShan.fsInfo != null)
                        {
                          
                            int size = FengShan.fsInfo.Count;
                            if (size > 1)
                            {
                                _DonateTemple = FengShan;
                                for (int i = 0; i < size; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            {
                                                //m_listDonateItem[i].m_TanHao.SetActive(_DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes > 0
                                                //                                && JunZhuData.Instance().m_junzhuInfo.yuanBao >= FengShan.fsInfo[i].needYuanBao
                                                //                                && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(27));
                                                if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(27)
                                                    || _DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes <= 0
                                                   )
                                                {
                                                    //if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(27))
                                                    //{
                                                    //    m_listDonateItem[i].m_Event.GetComponent<UIButton>().enabled = false;
                                                    //    m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                                                    //    m_listDonateItem[i].m_LabDes.text = "V" + VipFuncOpenTemplate.GetNeedLevelByKey(27).ToString()
                                                    //       + "开启";
                                                         
                                                    //}
                                                    
                                                    //else
                                                    {
                                                    //    m_listDonateItem[i].m_Event.GetComponent<UIButton>().enabled = false;
                                                        m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                                                        m_listDonateItem[i].m_LabDes.text = "捐献";
                                                    }
                                                }
                                                else
                                                {
                                             //       m_listDonateItem[i].m_Event.GetComponent<UIButton>().enabled = true;
                                                    m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                                                    m_listDonateItem[i].m_LabDes.text = "捐献";
                                                }
                                            }
                                            break;
                                        case 1:
                                            {
                                                //m_listDonateItem[i].m_TanHao.SetActive(_DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes > 0
                                                //                              && JunZhuData.Instance().m_junzhuInfo.yuanBao >= FengShan.fsInfo[i].needYuanBao
                                                //                              && JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(26));
                                                if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(26)
                                                     || _DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes <= 0
                                                    )
                                                {
                                                    //if (JunZhuData.Instance().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey(26))
                                                    //{
                                                    //    m_listDonateItem[i].m_Event.GetComponent<UIButton>().enabled = false;
                                                    //    m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false, 1);
                                                    //    m_listDonateItem[i].m_LabDes.text = "V" + VipFuncOpenTemplate.GetNeedLevelByKey(26).ToString()
                                                    //       + "开启";
                                                       
                                                    //}
                                                    //else
                                                    {
                                            
                                                        m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                                                        m_listDonateItem[i].m_LabDes.text = "捐献";
                                                    }
                                                }

                                                else
                                                {
         
                                                    m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                                                    m_listDonateItem[i].m_LabDes.text = "捐献";
                                                }
                                            }
                                            break;
                                    }

                                    m_listDonateItem[i].m_LabSelf.text = "X" + LianmengJuanxianTemplate.getJuanxianById(FengShan.fsInfo[i].confId).awardShow1;
                                    m_listDonateItem[i].m_LabAlliance.text = "X" + LianmengJuanxianTemplate.getJuanxianById(FengShan.fsInfo[i].confId).awardShow2;
                                    ShowLabInfo(FengShan);
                                    if (FengShan.fsInfo[i].totalTimes > 0)
                                    {
                                        m_listDonateItem[i].m_LabTimes.text = MyColorData.getColorString(5, "今日剩余次数 : "
                                                                              + (FengShan.fsInfo[i].totalTimes - FengShan.fsInfo[i].usedTimes)
                                                                              + "/" + FengShan.fsInfo[i].totalTimes);
                                    }
                                    else
                                    {
                                        m_listDonateItem[i].m_LabTimes.text = MyColorData.getColorString(5, "今日剩余次数 : "
                                                                            + FengShan.fsInfo[i].usedTimes +"/0");
                                    }
                                    m_LabWorshipCount.text = FengShan.huoyuedu.ToString() + "/" +
                                               LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString() + "（每日凌晨4点重置）";


                                }
                            }
                            else
                            {
						       if(_DonateTemple !=null)
						      {
                                _DonateTemple.huoyuedu = FengShan.huoyuedu;
                                _mobaiCount = FengShan.huoyuedu;
                                m_LabWorshipCount.text = _DonateTemple.huoyuedu.ToString() + "/"
                                    + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString() + "（每日凌晨4点重置）";
                                m_ProgressBar.value = _DonateTemple.huoyuedu / float.Parse(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString());

                                for (int i = 0; i < 3; i++)
                                {
                                    StepReward sr0 = _listStepInfo[i];
                                    sr0.priviousCount = FengShan.huoyuedu;
                                    _listStepInfo[i] = sr0;
                                }

                                StepRewardShow();
                                int size_s = _DonateTemple.fsInfo.Count;
                                for (int i = 0; i < size_s; i++)
                                {
 
                                    if (_DonateTemple.fsInfo[i].confId == FengShan.fsInfo[0].confId)
                                    { 
                                      m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                                      _DonateTemple.fsInfo[i].usedTimes = FengShan.fsInfo[0].usedTimes;
                                      m_listDonateItem[i].m_LabTimes.text = MyColorData.getColorString(5, "今日剩余次数 : "
                                                                      + (_DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes)
                                                                      + "/" + _DonateTemple.fsInfo[i].totalTimes);
                                    }
                                }
                                ShowLabInfo(FengShan);


                                for (int i = 0; i < size_s; i++)
                                {
                                    if (_DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes <= 0)
                                    {
                                        if (_DonateTemple.fsInfo[i].totalTimes - _DonateTemple.fsInfo[i].usedTimes <= 0)
                                        {
                                            m_listDonateItem[i].m_Event.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                                        }
                                       
                                    }

                                }
                                FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengJuanxianTemplate.getJuanxianById(FengShan.fsInfo[0].confId).award);
                            }
					     }

                        }

                        return true;
                    }
                case ProtoIndexes.S_DO_ALLIANCE_FENGSHAN_RESP:/** 封禅领奖信息 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FengShanResp FengShan = new FengShanResp();

                        t_qx.Deserialize(t_tream, FengShan, FengShan.GetType());

                        // if (FengShan.result == 10)
                        {


                        }

                        return true;
                    }

            }
        }
        return false;
    }

    void ShowLabInfo(FengShanInfoResp Donate)
    {
        int size_s = _DonateTemple.fsInfo.Count;
        for (int i = 0; i < size_s; i++)
        {
            if (i < Donate.fsInfo.Count && _DonateTemple.fsInfo[i].confId == Donate.fsInfo[i].confId)
            {
                if (JunZhuData.Instance().m_junzhuInfo.yuanBao < Donate.fsInfo[0].needYuanBao)
                {
                    m_listDonateItem[i].m_LabPrice.text = MyColorData.getColorString(5, Donate.fsInfo[i].needYuanBao.ToString());
                }
                else
                {
                    m_listDonateItem[i].m_LabPrice.text = Donate.fsInfo[i].needYuanBao.ToString();
                }
            }
        }
    }
    private bool WetherCanFengShan()
    {
        int size = _DonateTemple.fsInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_DonateTemple.fsInfo[i].totalTimes > _DonateTemple.fsInfo[i].usedTimes
                  && JunZhuData.Instance().m_junzhuInfo.yuanBao >= _DonateTemple.fsInfo[i].needYuanBao)
            {
                return true;
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
        m_ProgressBar.value = worshipShow.buffCount / float.Parse(LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString());
        m_LabWorshipCount.text = worshipShow.buffCount.ToString() + "/" 
                          + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString() + "（每日凌晨4点重置）";
        if (GetCopperCoinWorshipEnable())
        {
           m_listMoBaiItem[0].m_LabPrice.text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());
        }
        else
        {
            m_listMoBaiItem[0].m_LabPrice.text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(1).needNum.ToString());

        }
        string awad0 = LianmengMoBaiTemplate.GetShowInfoByType(1).awardShow;
        string[] ss0 = LianmengMoBaiTemplate.GetShowInfoByType(1).awardShow.Split('#');
        for (int i = 0; i < ss0.Length; i++)
        {
            string[] info = ss0[i].Split(':');
            switch (i)
            {
                case 0:
                    {
                        m_listMoBaiItem[0].m_LabSelfRight.text = "X" + info[2];

                    }
                    break;
                case 1:
                    {
                        m_listMoBaiItem[0].m_LabSelfLeft.text = "X" + info[2];
                    }
                    break;
                case 2:
                    {
                        m_listMoBaiItem[0].m_LabAlliance.text = "X" + info[2];
                    }
                    break;
                default:
                    break;
            }
        }

        if (GetYuanBaoWorshipEnable())
        {
            m_listMoBaiItem[1].m_LabPrice.text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        else
        {
            m_listMoBaiItem[1].m_LabPrice.text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        string awad1 = LianmengMoBaiTemplate.GetShowInfoByType(2).awardShow;
        string[] ss1 = LianmengMoBaiTemplate.GetShowInfoByType(2).awardShow.Split('#');
        for (int i = 0; i < ss1.Length; i++)
        {
            string[] info = ss1[i].Split(':');
            switch (i)
            {
                case 0:
                    {
                        m_listMoBaiItem[1].m_LabSelfRight.text = "X" + info[2];

                    }
                    break;
                case 1:
                    {
                        m_listMoBaiItem[1].m_LabSelfLeft.text = "X" + info[2];
                    }
                    break;
                case 2:
                    {
                        m_listMoBaiItem[1].m_LabAlliance.text = "X" + info[2];
                    }
                    break;
                default:
                    break;
            }
        }
        //m_listMoBaiItem[1].m_LabSelfLeft.text = "X" + LianmengMoBaiTemplate.GetShowInfoByType(2).gongxian;
        //m_listMoBaiItem[1].m_LabSelfRight.text = "X" + LianmengMoBaiTemplate.GetShowInfoByType(2).tili;
        //m_listMoBaiItem[1].m_LabAlliance.text = "X" + LianmengMoBaiTemplate.GetShowInfoByType(1).jianshe;
        if (!worshipShow.yuanBaoOpen)
        {
            //m_ListGameobjectShow[2].SetActive(false);
          //  m_ListGameobjectShow[3].SetActive(true);
        }

        string awad = LianmengMoBaiTemplate.GetShowInfoByType(3).awardShow;
        string[] ss = LianmengMoBaiTemplate.GetShowInfoByType(3).awardShow.Split('#');
        for (int i = 0; i < ss.Length; i++)
        {
            string[] info = ss[i].Split(':');
            switch (i)
            {
                case 0:
                    {
                        m_listMoBaiItem[2].m_LabSelfRight.text = "X" + info[2];
                       
                    }
                    break;
                case 1:
                    {
                        m_listMoBaiItem[2].m_LabSelfLeft.text = "X" + info[2];
                    }
                    break;
                case 2:
                    {
                        m_listMoBaiItem[2].m_LabAlliance.text = "X" + info[2];
                    }
                    break;
                default:
                    break;
            }
        }
        YuJueInfo();
    }

    private string GetString(string award)
    {
        string info = "";
        if (award.IndexOf("#") > -1)
        {
            string[] ss = award.Split('#');
            string[] ss2 = ss[0].Split(':');
            info = ss2[2].ToString();
        }
        else
        {
            string[] ss = award.Split(':');
            info = ss[2].ToString();
        }

        return info;
    }



    void YuJueInfo()
    {
        _listYJ.Clear();
 
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
        }
        else
        {
            yj1.color = 5;
        }
        _listYJ.Add(yj1);
        YuJueShow yj2 = new YuJueShow();
        yj2.icon = 950001;
        yj2.count = BagData.Instance().GetCountByItemId(950001);
        if (BagData.Instance().GetCountByItemId(950001) >= 1)
    
        {
            yj2.color = 1;
         
        }
        else
        {
            yj2.color = 5;
        }
        _listYJ.Add(yj2);
        YuJueShow yj3 = new YuJueShow();
        yj3.icon = 950004;
        yj3.count = BagData.Instance().GetCountByItemId(950004);
        if (BagData.Instance().GetCountByItemId(950004) >= 1)
        {
            yj3.color = 1;
        }
        else
        {
            yj3.color = 5;
        }
        _listYJ.Add(yj3);
        YuJueShow yj4 = new YuJueShow();
        yj4.icon = 950003;
        yj4.count = BagData.Instance().GetCountByItemId(950003);
        if (BagData.Instance().GetCountByItemId(950003) >= 1)

        {
            yj4.color = 1;
        }
        else
        {
            yj4.color = 5;
       
        }
        _listYJ.Add(yj4);
        _indexNum = 0;


        if (m_Gride.transform.childCount > 0)
        {
            int childCount = m_Gride.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
              
               IconSampleManager iconSampleManager = m_Gride.transform.GetChild(i).GetComponent<IconSampleManager>();
               iconSampleManager.SetIconByID(_listYJ[i].icon, _listYJ[i].count.ToString(), 10);
               iconSampleManager.SetIconPopText(_listYJ[i].icon
                   ,NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listYJ[i].icon).nameId)
                    , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listYJ[i].icon).descId));
               iconSampleManager.transform.localScale = Vector3.one * 0.45f;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                           ResLoaded);
            }
        }
    }


    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
        AllianceData.Instance.RequestAllianceData();
        NewAlliancemanager.Instance().Refreshtification();
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
    }

    int row = 5;
    Vector3 Startpos;
    Vector3 Startpos2;
    int indexNum = 0;
    float pos_x = 0;
    void RewardYuJueCreate()
    {
        if (m_YuJueParent.transform.childCount > 0)
        {
            int childCount = m_YuJueParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Destroy(m_YuJueParent.transform.GetChild(i).gameObject);
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
   
        m_YuJueSuffParent.transform.localPosition = new Vector3(ParentPosOffset(listInsufficientYuJue.Count, 60), 0, 0);
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
        listInsufficientYuJue.Clear();
        if (BagData.Instance().GetCountByItemId(950001) < 1)
        {
            listInsufficientYuJue.Add("950001");
        }
        if (BagData.Instance().GetCountByItemId(950002) < 1)
        {
            listInsufficientYuJue.Add("950002");
        }
        if (BagData.Instance().GetCountByItemId(950003) < 1)
        {
            listInsufficientYuJue.Add("950003");
        }
        if (BagData.Instance().GetCountByItemId(950004) < 1)
        {
            listInsufficientYuJue.Add("950004");
        }
        if (BagData.Instance().GetCountByItemId(950005) < 1)
        {
            listInsufficientYuJue.Add("950005");
        }
    }

    void TidyRewardInfo()
    {
        
        RewardYuJueCreate();
    }
    int indexNum3 = 0;
    void RewardMoveEffect(int index)
    {
        indexNum3 = index;
        MoveActionAlong(m_listMoBaiPiao[index].transform);
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
        FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengMoBaiTemplate.GetShowInfoByType(index_Num4 + 1).awardShow);
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

            tempObject.transform.localScale = Vector3.one * 0.45f;

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
            iconSampleManager.SetIconByID(int.Parse(listInsufficientYuJue[indexNum2]), "1", 8);

            iconSampleManager.SetIconPopText(int.Parse(listInsufficientYuJue[indexNum2]), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listInsufficientYuJue[indexNum2])).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(listInsufficientYuJue[indexNum2])).descId));

            tempObject.transform.localScale = Vector3.one * 0.5f;
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
        if (m_YuJueParent != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_YuJueParent.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(listReWard[indexNum].itemId, listReWard[indexNum].itemNumber.ToString());

            iconSampleManager.SetIconPopText(listReWard[indexNum].itemId, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listReWard[indexNum].itemId).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listReWard[indexNum].itemId).descId));

            tempObject.transform.localScale = Vector3.one * 0.5f;

            if (indexNum < listReWard.Count - 1)
            {
                indexNum++;
            }
            m_YuJueParent.GetComponent<UIGrid>().repositionNow = true;
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
        m_RewardParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(size_ReWard, 82), 0, 0);
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
            tempObject.transform.localScale = Vector3.one * 0.8f;

            if (_indexNum22 < _listReward.Count - 1)
            {
                _indexNum22++;
            }
            else
            {
                m_RewardObj.SetActive(true);
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
        if (int.Parse(obj.name) == 0)
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
    Vector3 vec_target = new Vector3(0, 232, 0);
    private GameObject _PiaoLingObject;
    void MoveActionAlong(Transform trans)
    {
        _PiaoLingObject = trans.gameObject;
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, trans.gameObject, EffectIdTemplate.GetPathByeffectId(620216), null);
        iTween.MoveTo(trans.transform.gameObject, iTween.Hash("position", vec_target, "islocal", true, "delay", 0, "easeType", "linear", "time", 0.5f, "oncomplete", "SendName", "oncompletetarget", gameObject));
    }

    void SendName()
    {
        UI3DEffectTool.ClearUIFx(_PiaoLingObject.gameObject);
        if (touchIndex == 2)
        {
            m_listMoBaiPiao[touchIndex].transform.localPosition = new Vector3(332, -235, 0);
        }

        for (int i = 0; i < m_listMoBaiPiao.Count; i++)
        {
            UI3DEffectTool.ClearUIFx(m_listMoBaiPiao[i]);
        }
        for (int i = 0; i < m_listFengShanPiao.Count; i++)
        {
            m_listFengShanPiao[i].transform.localPosition = _listDonatePos[i];
            UI3DEffectTool.ClearUIFx(m_listFengShanPiao[i]);
        }
 
    }

    private bool YuJueIsEnough()
    {
        if (BagData.Instance().GetCountByItemId(950002) >= 1
            && BagData.Instance().GetCountByItemId(950005) >= 1
            && BagData.Instance().GetCountByItemId(950001) >= 1
            && BagData.Instance().GetCountByItemId(950004) >= 1
            && BagData.Instance().GetCountByItemId(950003) >= 1)
        {
            return true;
        }

        return false;
    }

  

    private bool GoldWorship()
    {

        return /*m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(1).tuTenglvNeeded && */ FunctionOpenTemp.GetWhetherContainID(400010);
    }
    private bool YuanBaoWorship()
    {
        return /*m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(2).tuTenglvNeeded && */FunctionOpenTemp.GetWhetherContainID(400012);
    }
    private bool YuJueWorship()
    {
        return FunctionOpenTemp.GetWhetherContainID(400015);// m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(3).tuTenglvNeeded 
    }

    void StepRewardShow()
    {
        int size = m_listRewardEvent.Count;
        for (int i = 0; i < size; i++)
        {
            if (_listStepInfo[i].stepState == 0)
            {
                switch (i)
                {
                    case 0:
                        {
                
                            if (_listStepInfo[i].priviousCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes1)
                            {
                                StepReward sr = _listStepInfo[i];
                                sr.stepState = 1;
                                _listStepInfo[i] = sr;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (_listStepInfo[i].priviousCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes2)
                            {
                                StepReward sr = _listStepInfo[i];
                                sr.stepState = 1;
                                _listStepInfo[i] = sr;
                            }
                        }
                        break;
                    case 2:
                        {
                            if (_listStepInfo[i].priviousCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3)
                            {
                                StepReward sr = _listStepInfo[i];
                                sr.stepState = 1;
                                _listStepInfo[i] = sr;
                            }
                        }
                        break;
                }
            }

            if (_listStepInfo[i].stepState != 2)
            {
                m_listRewardEvent[i].m_TouchLQEvent.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(_listStepInfo[i].stepState == 0 ? false : true,1);
                m_listRewardEvent[i].m_TouchEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(_listStepInfo[i].stepState == 0 ? false : true);
                m_listRewardEvent[i].m_TouchEvent.GetComponent<Collider>().enabled = true;
                m_listRewardEvent[i].m_ObjFirst.SetActive(true);
                m_listRewardEvent[i].m_ObjScecond.SetActive(false);
               
                switch (i)
                {
                    case 0:
                        {
                            m_listRewardEvent[i].m_LabTitle.text = "虔诚值" + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes1.ToString() + "点可领取";
                        }
                        break;
                    case 1:
                        {
                            m_listRewardEvent[i].m_LabTitle.text = "虔诚值" + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes2.ToString() + "点可领取";
                        }
                        break;
                    case 2:
                        {
                            m_listRewardEvent[i].m_LabTitle.text = "虔诚值" + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString() + "点可领取";
                        }
                        break;
                }

            }
            else
            {
                m_listRewardEvent[i].m_TouchLQEvent.GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false,1);
                m_listRewardEvent[i].m_TouchEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                m_listRewardEvent[i].m_TouchEvent.GetComponent<Collider>().enabled = true;
                m_listRewardEvent[i].m_LabLQ.text = "已领取";
          
                m_listRewardEvent[i].m_ObjFirst.SetActive(false);
                m_listRewardEvent[i].m_ObjScecond.SetActive(true);
                m_listRewardEvent[i].m_LabTitle.text = "";
            }

            m_listRewardEvent[i].GetComponent<ButtonColorManagerment>().ShakeEffectShow(_listStepInfo[i].stepState == 1 ? true : false);
        }
    }
}
