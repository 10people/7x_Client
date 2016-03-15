using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;
public class WorshipLayerManagerment : MonoBehaviour, SocketProcessor
{
    public List<EventSuoHandle> m_ListEvent;
    public static int m_bulidingLevel = 0;
    public List<GameObject> m_ListGameobjectShow;
    public List<UILabel> m_ListCount;
    public List<GameObject> m_ListSignal;
    public List<GameObject> m_listParent;
    public UIGrid m_YuJueSuffParent;
    public List<WorshipStepAwardManangerment> m_listRewardEvent;
    public List<WorshipFengChanManagerment> m_listFengShanInfo;
   
    public GameObject m_MainParent;
    public GameObject m_Durable_UI;
    public GameObject m_RewardObj;
    public UIGrid m_RewardParent;
    public List<EventIndexHandle> m_ListTypeEvent;
    public List<EventIndexHandle> m_ListCancelEvent;
    private MoBaiInfo worshipShow = new MoBaiInfo();
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

    public GameObject m_MoBai;
    public GameObject m_FengChan;


    public GameObject m_DaDianTanHao;
    public GameObject m_ShengDianTanHao;

    public GameObject m_TanHaoMoBai;
    public GameObject m_TanHaoFengShan;
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

    private bool isWorship = false;
    private int touchIndex = -1;
    private int _mobaiCount = 0;
    private Vector3 vec_pos = new Vector3(332, 235, 0);
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
 
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }


    void Start()
    {
        if (FreshGuide.Instance().IsActive(400030) && TaskData.Instance.m_TaskInfoDic.ContainsKey(400030) && TaskData.Instance.m_TaskInfoDic[400030].progress >= 0)
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
        m_ListTypeEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
        m_LabelSignal.text = LanguageTemplate.GetText(1520);
        m_ListEvent.ForEach(p => p.m_Handle += EventGet);
        m_listRewardEvent.ForEach(p =>p.m_TouchLQEvent.m_Handle += RewardGet);
        m_listRewardEvent.ForEach(p => p.OnLongPress += ShowReward);
        m_listRewardEvent.ForEach(p => p.OnLongPressFinish += LongFinish);
        m_ListTypeEvent.ForEach(p => p.m_Handle += TypeInfo);
        //m_listRewardEvent.ForEach(p => p.m_TouchEvent.m_Handle += ShowReward);
        m_SEC.OpenCompleteDelegate += RequestData;
        m_ListCancelEvent.ForEach(p => p.m_Handle += HiddenReward);
        m_listFengShanInfo.ForEach(p =>p.m_Event.m_Handle += FengShan);
    }


    void FengShan(int index)
    {
        UIYindao.m_UIYindao.CloseUI();
        if (JunZhuData.Instance().m_junzhuInfo.tili >= 999)
        {
            EquipSuoData.ShowSignal(null, LanguageTemplate.GetText(800));
            return;

        }
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        FengShanReq fens = new FengShanReq();
        fens.confId = LianmengFengshanTemplate.GetFengshanById(index + 1).id;
        t_qx.Serialize(t_tream, fens);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_DO_ALLIANCE_FENGSHAN_REQ, ref t_protof);

    }
    void ShowInfo()
    {
        int size_0 = m_listLabObjMoBai.Count;
        for (int i = 0; i < size_0; i++)
        {
            if (i == 0 && m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(i + 1).tuTenglvNeeded && FunctionOpenTemp.GetWhetherContainID(400010))
            {
                m_ListEvent[i].gameObject.SetActive(true);
                m_listLabObjMoBai[i].text = "";
            }
            else if (i == 1 && m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(i + 1).tuTenglvNeeded && FunctionOpenTemp.GetWhetherContainID(400012))
            {
                m_ListEvent[i].gameObject.SetActive(true);
                m_listLabObjMoBai[i].text = "";
            }
            else if (i == 2 && m_bulidingLevel >= LianmengMoBaiTemplate.GetShowInfoByType(i + 1).tuTenglvNeeded && FunctionOpenTemp.GetWhetherContainID(400015))
            {
                m_ListEvent[i].gameObject.SetActive(true);
                m_listLabObjMoBai[i].text = "";
            }
            else
            {
                m_ListEvent[i].gameObject.SetActive(false);
                m_listLabObjMoBai[i].text = LanguageTemplate.GetText(20011) 
                    + LianmengMoBaiTemplate.GetShowInfoByType(i + 1).tuTenglvNeeded .ToString()
                    + LanguageTemplate.GetText(20012);
            }
        }

        int size_1 = m_listLabObjFengShan.Count;
        //for (int i = 0; i < size_1; i++)
        //{
        //    if (i == 0 && m_bulidingLevel >= LianmengFengshanTemplate.GetFengshanById(i + 1).tuTenglvNeeded && FunctionOpenTemp.GetWhetherContainID(400100))
        //    {
        //        m_listFengShanInfo[i].m_Event.gameObject.SetActive(true);
        //        m_listLabObjFengShan[i].text = "";
        //    }
        //    else if (i == 0 && m_bulidingLevel >= LianmengFengshanTemplate.GetFengshanById(i + 1).tuTenglvNeeded && FunctionOpenTemp.GetWhetherContainID(400110))
        //    {
        //        m_listFengShanInfo[i].m_Event.gameObject.SetActive(true);
        //        m_listLabObjFengShan[i].text = "";
        //    }
        //    else
        //    {
        //        m_listFengShanInfo[i].m_Event.gameObject.SetActive(false);
        //        m_listLabObjFengShan[i].text = LanguageTemplate.GetText(20011)
        //            + LianmengFengshanTemplate.GetFengshanById(i + 1).tuTenglvNeeded.ToString()
        //            + LanguageTemplate.GetText(20012);
        //    }
        //}
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
            type_Touch_index = index;
            if (index == 0)
            {
                m_LabelSignal.text = LanguageTemplate.GetText(1520);
                m_ListTypeEvent[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                m_ListTypeEvent[index + 1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                m_MoBai.SetActive(true);
                m_FengChan.SetActive(false);
            }
            else
            {
                m_LabelSignal.text = LanguageTemplate.GetText(1603);
                m_ListTypeEvent[index - 1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                m_ListTypeEvent[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                m_MoBai.SetActive(false);
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_FENGSHAN_REQ);
             
            }
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
    void  RewardGet(int index)
    {
      //  if (obj.name.IndexOf('_') > -1)
        {
          ///  string[] ss = obj.name.Split('_');
            switch (index)
            {
                case 0:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes1 && worshipShow.bigStep0 == 1)
                        {
                            GetReward(index + 1);
                        }
                    }
                    break;
                case 1:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes2 && worshipShow.bigStep1 == 1)
                        {
                            GetReward(index + 1);
                        }
                    }
                    break;
                case 2:
                    {
                        if (_mobaiCount >= LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3 && worshipShow.bigStep2 == 1)
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
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_MOBAI_AWARD, ref t_protof);
    }
    void TuTengRewardData(string _award)
    {
        int size_all = m_RewardParent.transform.childCount;
        for (int i = 0;i < size_all; i++)
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
        m_RewardParent.transform.localPosition = Vector3.zero;
    }
    void Update()
    {
        // m_TanHaoFengShan.SetActive();
    
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
        UIYindao.m_UIYindao.CloseUI();
        if (JunZhuData.Instance().m_junzhuInfo.tili >= 999)
        {
            EquipSuoData.ShowSignal(null, LanguageTemplate.GetText(800));
            return;

        }
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
                        EquipSuoData.TopUpLayerTip(m_MainParent,true);
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
                        EquipSuoData.TopUpLayerTip(m_MainParent,true);
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
    FengShanInfoResp _FengShanTemple;
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

                        m_TanHaoMoBai.SetActive((worship.tongBiOpen && FunctionOpenTemp.GetWhetherContainID(400010))
                                || (worship.yuanBaoOpen && FunctionOpenTemp.GetWhetherContainID(400012)) 
                                || (worship.yuOpen && YuJueIsEnough() && FunctionOpenTemp.GetWhetherContainID(400015)));

                        MainCityUI.SetRedAlert(400010, worship.tongBiOpen && FunctionOpenTemp.GetWhetherContainID(400010));
                        MainCityUI.SetRedAlert(400012, worship.yuanBaoOpen && FunctionOpenTemp.GetWhetherContainID(400012));
                        MainCityUI.SetRedAlert(400015, worship.yuOpen && YuJueIsEnough() && FunctionOpenTemp.GetWhetherContainID(400015));
 
                        m_YuanBaoTanHao.SetActive(worship.yuanBaoOpen && FunctionOpenTemp.GetWhetherContainID(400010));
                        m_YuJueTanHao.SetActive(worship.yuOpen && YuJueIsEnough() && FunctionOpenTemp.GetWhetherContainID(400015));
                        m_TanHao.SetActive(worship.tongBiOpen && FunctionOpenTemp.GetWhetherContainID(400010));
                            worshipShow = worship;
                        m_listRewardEvent[0].GetComponent<ButtonColorManagerment>().ShakeEffectShow(worship.bigStep0 == 1 ? true : false);
                        if (worship.bigStep0 != 2)
                        {
                            m_listRewardEvent[0].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(worship.bigStep0 == 0 ? false : true);
                            m_listRewardEvent[0].m_ObjFirst.SetActive(true);
                            m_listRewardEvent[0].m_ObjScecond.SetActive(false);
                            m_listRewardEvent[0].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_0) + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes1.ToString() + LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_1);
                        }
                        else
                        {
                            m_listRewardEvent[0].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_listRewardEvent[0].m_LabLQ.gameObject.SetActive(false);
                            m_listRewardEvent[0].m_LabYLQ.gameObject.SetActive(true);
                            m_listRewardEvent[0].m_ObjFirst.SetActive(false);
                            m_listRewardEvent[0].m_ObjScecond.SetActive(true);
                            //   m_listRewardEvent[0].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_2);
                            m_listRewardEvent[0].m_LabTitle.text = "";
                        }
                        //m_listRewardEvent[0].m_LabTitle.gameObject.SetActive(worship.bigStep0 == 2);
                        m_listRewardEvent[1].GetComponent<ButtonColorManagerment>().ShakeEffectShow(worship.bigStep1 == 1);

                        if (worship.bigStep1 != 2)
                        {
                            m_listRewardEvent[1].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(worship.bigStep1 == 0 ? false:true);
                            m_listRewardEvent[1].m_ObjFirst.SetActive(true);
                            m_listRewardEvent[1].m_ObjScecond.SetActive(false);
                            m_listRewardEvent[1].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_0) + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes2.ToString() + LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_1);
                        }
                        else
                        {
                            m_listRewardEvent[1].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_listRewardEvent[1].m_LabLQ.gameObject.SetActive(false);
                            m_listRewardEvent[1].m_LabYLQ.gameObject.SetActive(true);
                            m_listRewardEvent[1].m_ObjFirst.SetActive(false);
                            m_listRewardEvent[1].m_ObjScecond.SetActive(true);
                            //m_listRewardEvent[1].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_2);
                            m_listRewardEvent[1].m_LabTitle.text = "";
                        }

                        //   m_listRewardEvent[1].m_LabTitle.gameObject.SetActive(worship.bigStep1 == 2);
                        m_listRewardEvent[2].GetComponent<ButtonColorManagerment>().ShakeEffectShow(worship.bigStep2 == 1);
                        if (worship.bigStep2 != 2)
                        {
                            m_listRewardEvent[2].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(worship.bigStep2 == 0 ? false : true);
                            m_listRewardEvent[2].m_ObjFirst.SetActive(true);
                            m_listRewardEvent[2].m_ObjScecond.SetActive(false);
                            m_listRewardEvent[2].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_0) + LianMengTuTengTemplate.getTuTengAwardByLevel(m_bulidingLevel).moBaiTimes3.ToString() + LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_1);

                        }
                        else
                        { 
                            m_listRewardEvent[2].m_TouchLQEvent.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                            m_listRewardEvent[2].m_LabLQ.gameObject.SetActive(false);
                            m_listRewardEvent[2].m_LabYLQ.gameObject.SetActive(true);
                            m_listRewardEvent[2].m_ObjFirst.SetActive(false);
                            m_listRewardEvent[2].m_ObjScecond.SetActive(true);
                            //  m_listRewardEvent[2].m_LabTitle.text = LanguageTemplate.GetText(LanguageTemplate.Text.WORSHIP_TITI_2);
                            m_listRewardEvent[2].m_LabTitle.text = "";
                        }
                        // m_listRewardEvent[2].m_LabTitle.gameObject.SetActive(worship.bigStep2 == 2);

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
                        m_TanHaoFengShan.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(400100) || PushAndNotificationHelper.IsShowRedSpotNotification(400110));
                        NewAlliancemanager.Instance().Refreshtification();
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
                            _FengShanTemple = FengShan;
                            int size = FengShan.fsInfo.Count;
                            for (int i = 0; i < size; i++)
                            {
                                m_listFengShanInfo[i].m_TopTitle.text = LianmengFengshanTemplate.GetFengshanById(FengShan.fsInfo[i].confId).desc1;
                                m_listFengShanInfo[i].m_MidTitle.text = LianmengFengshanTemplate.GetFengshanById(FengShan.fsInfo[i].confId).desc2;

                                if (FengShan.fsInfo[i].state == 2 && i == 0 && FunctionOpenTemp.GetWhetherContainID(400100))
                                {
                                    m_listFengShanInfo[i].m_TanHao.SetActive(true);
                                    m_listFengShanInfo[i].m_Event.transform.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
                                }
                                else if (FengShan.fsInfo[i].state == 2 && i == 1 && FunctionOpenTemp.GetWhetherContainID(400110))
                                {
                                    m_listFengShanInfo[i].m_TanHao.SetActive(true);
                                    m_listFengShanInfo[i].m_Event.transform.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
                                }
                                else
                                {
                                    m_listFengShanInfo[i].m_TanHao.SetActive(false);
                                    m_listFengShanInfo[i].m_Event.transform.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                                    if (FengShan.fsInfo[i].state == 3)
                                    {
                                        m_listFengShanInfo[i].m_UnFengChan.SetActive(false);
                                        m_listFengShanInfo[i].m_FengChanSuccess.SetActive(true);
                                    }
                                }
                            }
                          //  m_TanHaoFengShan.SetActive(WetherCanFengShan());
                            m_FengChan.SetActive(true);
                        }

                            return true;
                    }
                case ProtoIndexes.S_DO_ALLIANCE_FENGSHAN_RESP:/** 封禅领奖信息 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FengShanResp FengShan = new FengShanResp();

                        t_qx.Deserialize(t_tream, FengShan, FengShan.GetType());
                        if (FengShan.result == 10)
                        {
                            NewAlliancemanager.Instance().Refreshtification();
                            worshipShow.buffCount++;
                            m_ListCount[13].text = worshipShow.buffCount.ToString() + "/" + LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3.ToString() + "（每日重置归零）";
                            int size = _FengShanTemple.fsInfo.Count;
                            for (int i = 0; i < size; i++)
                            {
                                if (FengShan.confId == _FengShanTemple.fsInfo[i].confId)
                                {
                                    if (FengShan.confId == 1)
                                    {
                                        MainCityUI.SetRedAlert(400100, false);
                                        PushAndNotificationHelper.SetRedSpotNotification(400100, false);
                                    }
                                    else
                                    {
                                        MainCityUI.SetRedAlert(40110, false);
                                        PushAndNotificationHelper.SetRedSpotNotification(400110, false);
                                    }
                                    MoveActionAlong(m_listFengShanPiao[i].transform);
                                    _FengShanTemple.fsInfo[i].state = 3;

                                    m_listFengShanInfo[i].m_UnFengChan.SetActive(false);
                                    m_listFengShanInfo[i].m_FengChanSuccess.SetActive(true);
                                }
                            }
                            NewAlliancemanager.Instance().Refreshtification();

                            m_TanHaoFengShan.SetActive(WetherCanFengShan());
                            m_listFengShanInfo[FengShan.confId - 1].m_TanHao.SetActive(false);
                            m_listFengShanInfo[FengShan.confId -1].m_Event.GetComponentInChildren<ButtonColorManagerment>().ButtonsControl(false);
                           FunctionWindowsCreateManagerment.ShowRAwardInfo(LianmengFengshanTemplate.GetFengshanById(FengShan.confId).award);

                        }

                        return true;
                    }

            }
        }
        return false;
    }

    private bool WetherCanFengShan()
    {
        int size = _FengShanTemple.fsInfo.Count;
        for (int i = 0; i < size; i++)
        {
            if (_FengShanTemple.fsInfo[i].state == 2)
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
		MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
      //  m_TanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(400010));
        m_ProgressBar.value = worshipShow.buffCount / float.Parse(LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3.ToString());
        m_ListCount[13].text = worshipShow.buffCount.ToString()+ "/" + LianMengTuTengTemplate.getTuTengAwardByLevel(1).moBaiTimes3.ToString() + "（每日重置归零）";
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

        m_ListCount[1].text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(1).tili + LanguageTemplate.GetText(1529)
                              + "\n           " + NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(1).gongxian + LanguageTemplate.GetText(1530));
        if (GetYuanBaoWorshipEnable())
        {
            m_ListCount[2].text = MyColorData.getColorString(1, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        else
        {
            m_ListCount[2].text = MyColorData.getColorString(5, LianmengMoBaiTemplate.GetShowInfoByType(2).needNum.ToString());
        }
        m_ListCount[3].text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(2).tili + LanguageTemplate.GetText(1529)
                                                            + "\n           " + NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(2).gongxian + LanguageTemplate.GetText(1530));


        if (!worshipShow.yuanBaoOpen)
        {
            m_ListGameobjectShow[2].SetActive(false);
            m_ListGameobjectShow[3].SetActive(true);
        }
        m_LabDingLiSignal.text = MyColorData.getColorString(1, NameIdTemplate.GetName_By_NameId(990035) + GetString(LianmengMoBaiTemplate.GetShowInfoByType(3).award) + LanguageTemplate.GetText(1531) +"\n" +NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(3).tili + LanguageTemplate.GetText(1529)
                                                            +"  "+ NameIdTemplate.GetName_By_NameId(990035) + LianmengMoBaiTemplate.GetShowInfoByType(3).gongxian + LanguageTemplate.GetText(1530)
                                                           

                                                            );
        ;
        ButtonEnable(1, worshipShow.yuanBaoOpen);

        YuJueInfo();
    }

    private string GetString(string award)
    {
        string info = "";
        string[] ss = award.Split(':');
        info = ss[2].ToString();
        return info;
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
        m_YuJueSuffParent.transform.localPosition = new Vector3(ParentPosOffset(listInsufficientYuJue.Count, 80), 20, 0);
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
        MoveActionAlong(m_listMoBaiPiao[touchIndex].transform);
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

            tempObject.transform.localScale = Vector3.one*0.45f;

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
    Vector3 vec_target = new Vector3(75, 232, 0);
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
            m_listMoBaiPiao[touchIndex].transform.localPosition = new Vector3(332,-235,0);
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
}
