using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class AllianceLayerManagerment : MonoBehaviour, SocketProcessor
{
    public static AllianceLayerManagerment m_No_AllianceLayer;
    public GameObject m_MainObj;
    public UILabel m_LabMainSignal;
    public UILabel m_LabelTitle0;
    public UILabel m_LabelTitle1;
    public GameObject m_Durable_UI;
    public GameObject m_SignalLab;
    public UIScrollView m_ScrollViewItem;
    public UIScrollView m_ScrollView;
    public GameObject m_HidenObject;
    public List<EventIndexHandle> m_listEvent;
    //public List<GameObject> m_ListObj;
    public GameObject m_ObjInputName;
    public GameObject m_ObjCreateSucess;
    public GameObject m_ObjSearchAlliance;
    public GameObject m_ObjAllianceInfo;
    public UISprite m_SpriteCountry;

    public GameObject m_ObjAllianceHave;
    public GameObject m_EnoughObj;
    public GameObject m_NoEnoughObj;
    public UILabel m_LabNoETopLeft;
    public UILabel m_LabNoEBottomLeft;
    public UILabel m_LabNoEBottomRight;

    public GameObject m_ObjTitleInfo;
    public GameObject m_ObjTitleMain;
    public GameObject m_ObjCloseInfo;
    public GameObject m_ObjButtonParent;
    public List<EventIndexHandle> m_ListEventZhuanGuo;

    public Dictionary<int, GameObject> m_DictView = new Dictionary<int, GameObject>();
    public UILabel m_LabInputName;
    public UILabel m_LabInputAllianceID;

    public Dictionary<int, GameObject> _YaoQingDict = new Dictionary<int, GameObject>();
    private string InputAllianceID = "";
    private string InputName = "";
    public UIGrid m_GrideIcon;
    public UIGrid m_GrideAllianceList;

    public List<UISprite> m_ListAllianceIcon;

    public GameObject m_SwitchCountry;

    public UILabel m_LabsignalUp;
    public UILabel m_LabsignalDown;
    public ScaleEffectController m_SEC;

    public GameObject m_ObjTopLeft;
    private int TouchedItemId = 0;

    public List<UILabel> m_ListApplicationInfo;
    public List<UILabel> m_ListApplicationName;
    public UILabel m_LabPrice;
    public UILabel m_LabInputSignal;

    public GameObject m_YaoQingObj;
    public GameObject m_YaoQingObjCancel;
    public GameObject m_MainCancel;
    public UIGrid m_GridParent;
    public GameObject m_ObjSig;
    public UILabel m_LabObjSig;
    public UIInput m_InputName;
    private int IconSave = 0;
    private int removeIndex = 0;
    private bool isUnInput = false;
    private bool isUnSlelectIcon = false;
    private bool isUnInputAllianecID = false;
    private string CreateNameSave = "";
 
    public GameObject m_MainLayer;
 
    private bool isShowOn = false;
    private int _AllianceCountry = 0;
    private int _AppliedCount = 0;
    public struct AllianceItemInfo
    {
        public int id;
        public string name;
        public int level;
        public int applyLevel;
        public int shengwang;
        public string mengzhu;
        public int country;
        public bool isApply;
        public int ShenPiId;
        public int Exp;
        public int Ren_Now;
        public int Ren_Max;
        public bool isCanApply;
    };

    private List<AllianceItemInfo> listItemNeedInfo = new List<AllianceItemInfo>();
    private List<NonAllianceInfo> listAllianceInfo = new List<NonAllianceInfo>();
    public struct AllianceYaoQingInfo
    {
        public string time;
        public int level;
        public string name;
        public int id;
        public int guojia;
    };
    private List<AllianceYaoQingInfo> _listYaoQingInfo = new List<AllianceYaoQingInfo>();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        m_No_AllianceLayer = this;
        UIYindao.m_UIYindao.CloseUI();
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "联盟", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        m_LabInputSignal.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_INPUT_SIGNAL);
        m_LabInputName.text = "";
        m_LabInputAllianceID.text = "";
        m_listEvent.ForEach(p => p.m_Handle += Touch);
        m_ListEventZhuanGuo.ForEach(p => p.m_Handle += SwitchCountry);
        m_LabelTitle0.text = LanguageTemplate.GetText(LanguageTemplate.Text.BUILD_UNION_1);
        m_LabelTitle1.text = LanguageTemplate.GetText(LanguageTemplate.Text.BUILD_UNION_2);
        m_SEC.OpenCompleteDelegate += RequestData;
    }
    void RequestData()
    {
        AllianceData.Instance.RequestData();
        AllianceData.Instance.RequestYaoQing();
    }
 
    public void OnSelect()
    {
        
        if (!string.IsNullOrEmpty(FunctionWindowsCreateManagerment.GetNeedString(m_InputName.value)))
        {
            InputName = FunctionWindowsCreateManagerment.GetNeedString(m_InputName.value);
        
            m_LabInputName.text = InputName;
            m_InputName.value = InputName;
            m_LabInputSignal.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        InputName = FunctionWindowsCreateManagerment.GetNeedString(m_InputName.value);
        if (!string.IsNullOrEmpty(InputName))
        {
            OnSelect();
        }
        if (AllianceData.Instance.m_InstantiateNoAlliance)
        {
            m_HidenObject.SetActive(true);
            isShowOn = true;
        }
        InputName = m_InputName.value;
        if (isShowOn)
        {
            if (AllianceData.Instance.m_InstantiateNoAlliance)
            {
                isShowOn = false;
                AllianceData.Instance.m_InstantiateNoAlliance = false;
                AllianceInfoShow();
                if (FunctionWindowsCreateManagerment.m_AllianceID > 0)
                {
                    ShowAllianceApplicationInfo(FunctionWindowsCreateManagerment.m_AllianceID);
                    FunctionWindowsCreateManagerment.m_AllianceID = -1;
                }
            }
        }


        if (!string.IsNullOrEmpty(InputName))
        {
            CreateNameSave = InputName;
        }
        else
        {
          ///  m_LabInputSignal.gameObject.SetActive(true);
        }

        if (AllianceData.Instance.m_Invite_List_Load)
        {
            AllianceData.Instance.m_Invite_List_Load = false;
            ShowInviteInfo();
        }
        InputAllianceID = m_LabInputAllianceID.text;
        SerchButtonControl();
        CreateNameButtonControl();
    }

    void SerchButtonControl()
    {
        if (!string.IsNullOrEmpty(m_LabInputAllianceID.text) && isUnInputAllianecID)
        {
            isUnInputAllianecID = false;

          
            ButtonsControl(2, true);
        }
        else if (!isUnInputAllianecID && string.IsNullOrEmpty(m_LabInputAllianceID.text))
        {
            isUnInputAllianecID = true;
            
            ButtonsControl(2, false);
        }
    }

    void ShowInviteInfo()
    {
        if (AllianceData.Instance.m_InviteInfo.inviteInfo != null)
        {
            _listYaoQingInfo.Clear();
            for (int i = 0; i < AllianceData.Instance.m_InviteInfo.inviteInfo.Count; i++)
            {
                AllianceYaoQingInfo ayqInfo = new AllianceYaoQingInfo();
                ayqInfo.time = AllianceData.Instance.m_InviteInfo.inviteInfo[i].date;
                ayqInfo.id = AllianceData.Instance.m_InviteInfo.inviteInfo[i].id;
                ayqInfo.name = AllianceData.Instance.m_InviteInfo.inviteInfo[i].name;
                ayqInfo.level = AllianceData.Instance.m_InviteInfo.inviteInfo[i].level;
                _listYaoQingInfo.Add(ayqInfo);

            }
            m_ObjSig.SetActive(true);
            m_LabObjSig.text = AllianceData.Instance.m_InviteInfo.inviteInfo.Count.ToString();
            if (index_YQRequest > 0)
            {
                m_MainCancel.SetActive(false);
                m_YaoQingObjCancel.SetActive(true);
                m_YaoQingObj.SetActive(true);
                CreateYaoQing();
            }

        }
        else if (index_YQRequest > 0)
        {
            m_ObjSig.SetActive(false);
            m_MainCancel.SetActive(false);
            m_YaoQingObjCancel.SetActive(true);
            m_YaoQingObj.SetActive(true);
        }

    }
    private bool _CDKeyIsOff = false;
    void CreateNameButtonControl()
    {
        if (_CDKeyIsOff && !string.IsNullOrEmpty(InputName))
        {
            _CDKeyIsOff = false;
            m_LabInputSignal.gameObject.SetActive(false);
        }
        else if (!_CDKeyIsOff && string.IsNullOrEmpty(InputName))
        {
            _CDKeyIsOff = true;
            m_LabInputSignal.gameObject.SetActive(true);
        }
        //     if (!string.IsNullOrEmpty(InputName))ui
        //{
        //    m_LabInputSignal.gameObject.SetActive(false);
        //}
 
        if ( !string.IsNullOrEmpty(InputName) && isUnInput && IconSave != 0) 
        {
            isUnInput = false;
            ButtonsControl(6, true);
            m_LabInputSignal.gameObject.SetActive(false);
        }
        else if (!isUnInput && (string.IsNullOrEmpty(InputName) || IconSave == 0))
        {
            isUnInput = true;
          //  m_LabInputSignal.gameObject.SetActive(true);
            ButtonsControl(6, false);
        }
    }

    void AllianceInfoShow()
    {
        if (AllianceData.Instance.m_AllianceInfoDic.Count == 0)
        {
            m_SignalLab.SetActive(true);
        }
        else
        {
            m_SignalLab.SetActive(false);
            m_ObjAllianceHave.SetActive(true);
            ShowAllAlliancesInfo();
        }
    }
    void TweenColorDestroy()
    {
        if (m_listEvent[removeIndex].transform.FindChild("Background").GetComponent<TweenColor>() != null)
        {
            EventDelegate.Remove(m_listEvent[removeIndex].transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);
            Destroy(m_listEvent[removeIndex].transform.FindChild("Background").GetComponent<TweenColor>());
        }
    }
    void ButtonsControl(int index, bool colliderEnable)
    {
        if (m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>() == null)
        {
            m_listEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>();
            m_listEvent[index].transform.FindChild("Background").gameObject.AddComponent<TweenColor>().enabled = false;
        }
        if (colliderEnable)
        {
            m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
        }
        //m_listEvent[str].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
        //m_listEvent[str].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
        m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().duration = 0.2f;
        m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        m_listEvent[index].GetComponent<Collider>().enabled = colliderEnable;
        removeIndex = index;
        EventDelegate.Add(m_listEvent[index].transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);

    }
    void Touch(int index)
    {
        switch (index)
        {
            case 0:
                {
                    //m_LabInputSignal.gameObject.SetActive(true);
                  
                    //   m_LabInputName.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_INPUT_SIGNAL);
                    IconSave = 0;
                    isUnInput = false;
                    isUnSlelectIcon = false;
                    m_ObjInputName.SetActive(true);
                    ShowAllianceIconInfo();
                }
                break;
            case 1:
                {

                    m_ObjSearchAlliance.SetActive(true);
                }
                break;
            case 2:
                {
                    ConnectServer(index);
                }
                break;

            case 3:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.guoJiaId != _AllianceCountry)
                    {
                        _guojiaId = _AllianceCountry;
                        ShowSwitch(_AllianceCountry);
                    }
                    else
                    {
                        ConnectServer(index);
                    }
                }
                break;
            case 4:
                {
                    ConnectServer(index);
                }
                break;
            case 5:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.guoJiaId != _AllianceCountry)
                    {
                        _guojiaId = _AllianceCountry;
                        ShowSwitch(_AllianceCountry);
                    }
                    else
                    {
                        ConnectServer(index);
                    }
                }
                break;

            case 6:
                {
                    m_LabInputName.text = "";
                    //if (SysparaTemplate.CompareSyeParaWord(CreateNameSave))
                    //{
                    //    _content1 = "有奇怪的文字混进来了...\n再推敲一下吧！"; ;
                    //    _content2 = "";
                    //    _SignalIndex = 2;
                    //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                    //}
                    //else
                    {
                        ConnectServer(index);
                    }
                }
                break;
            case 7:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= AllianceData.Instance.m_AllianceCreatePrice)
                    {
                        //_content1 = "确定花费" + AllianceData.Instance.m_AllianceCreatePrice.ToString() + "元宝创建一个名字叫" + "\n\n\n<" + CreateNameSave + ">"; ;
                        //_content2 = "的联盟？";
                        //_SignalIndex = 8;
                        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                        ConFirm(2);
                    }
                    else
                    {
                        EquipSuoData.ShowSignal(null,"您的元宝不足！");
                    }
                   
                }
                break;
            case 8:
                {
                    if (AllianceData.Instance.m_AllianceCreatePrice <= JunZhuData.Instance().m_junzhuInfo.yuanBao)
                    {
                        ConnectServer(index);
                    }
                    else
                    {
                      
                      //  m_ListObj[10].SetActive(true);
                    }
                }
                break;
            case 9:
                {
                    MainCityUI.TryRemoveFromObjectList(m_MainLayer);
                    EquipSuoData.TopUpLayerTip(null, false, 1);
                     // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_MAIN_LAYER), RB_LR_75_LoadCallback);
                     Destroy(m_MainLayer);
                }
                break;
            case 10:
                {
                    m_ObjAllianceInfo.SetActive(false);
                    m_ObjButtonParent.SetActive(true);
                    m_ObjTitleMain.SetActive(true);
                    ShowInfo();


                    m_ObjCloseInfo.SetActive(false);
                }
                break;
            case 11:
                {
                    MainCityUI.TryRemoveFromObjectList(m_MainObj);
                    JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                    AllianceData.Instance.IsAllianceNotExist = false;
                    if (Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_MAINCITY))
                    {
                        AllianceData.Instance.IsAllianceNotExist = false;
                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                        GameObject obj = new GameObject();
                        obj.name = "MainCityUIButton_104";
                        MainCityUI.m_MainCityUI.MYClick(obj);
                    }
                    else
                    {
                        ClientMain.m_UITextManager.createText("联盟加入成功！");
                    }
                    Destroy(m_MainObj);
                }
                break;
            case 12:
                {
                    OnSelect();
                    m_LabInputSignal.gameObject.SetActive(false);
                  //  m_listEvent[9].gameObject.SetActive(false);
                }
                break;
            case 13:
                {
                    m_LabInputSignal.gameObject.SetActive(false);
                    m_listEvent[9].gameObject.SetActive(true);
                }
                break;
            case 20:
                {
                    m_ObjAllianceInfo.SetActive(false);
                    m_ObjButtonParent.SetActive(true);
                    m_ObjTitleMain.SetActive(true);
                    ShowInfo();
                    m_ObjCloseInfo.SetActive(false);
                }
                break;
            case 21:
                {
                    m_ObjButtonParent.SetActive(false);
                    index_YQRequest = 1;
                    AllianceData.Instance.RequestYaoQing();
                }
                break;
            case 22:
                {
                    m_ObjButtonParent.SetActive(true);
                    m_MainCancel.SetActive(true);
                    m_YaoQingObj.SetActive(false);
                    m_YaoQingObjCancel.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
    int index_YQRequest = 0;
    void ConnectServer(int index)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        switch ((AllianceButtonNumEnum)index)
        {
            case AllianceButtonNumEnum.E_ALLIANCE_SEARCH:
                {
                    FindAlliance allianceSearch = new FindAlliance();
                    allianceSearch.id = int.Parse(InputAllianceID);
                    t_qx.Serialize(t_tream, allianceSearch);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.FIND_ALLIANCE, ref t_protof);
                }
                break;

            case AllianceButtonNumEnum.E_ALLIANCE_APPLICATION:
                {
                    ApplyAlliance allianceApplication = new ApplyAlliance();
                    allianceApplication.id = TouchedItemId;
                    t_qx.Serialize(t_tream, allianceApplication);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.APPLY_ALLIANCE, ref t_protof);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_CONSUME_CONFIRM:
                {
                    CreateAlliance allianceCreate = new CreateAlliance();
                    allianceCreate.name = CreateNameSave;
                    allianceCreate.icon = IconSave;
                    t_qx.Serialize(t_tream, allianceCreate);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.CREATE_ALLIANCE, ref t_protof); ;
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CANCEL_APPLICATION:
                {
                    CancelJoinAlliance allianceApplyCancel = new CancelJoinAlliance();
                    allianceApplyCancel.id = TouchedItemId;

                    t_qx.Serialize(t_tream, allianceApplyCancel);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.CANCEL_JOIN_ALLIANCE, ref t_protof); ;
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_QUICK_APPLICATION:
                {
                    CancelJoinAlliance allianceApplyCancel = new CancelJoinAlliance();
                    allianceApplyCancel.id = TouchedItemId;
                    t_qx.Serialize(t_tream, allianceApplyCancel);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.IMMEDIATELY_JOIN, ref t_protof);
                }
                break;
            default:
                break;
        }
    }
   string  _strTitle = "";
   string _strContent1 = "";
   string _strContent2 = "";
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_ChangeCountry_RESP://转国返回
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ChangeGuojiaResp tempResponse = new ChangeGuojiaResp();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
                        m_SwitchCountry.SetActive(false);

                        if (tempResponse.result == 0)
                        {
                            if (_IndexId != 0)
                            {
                                _IndexId = 0;
                                ConnectServer(5);
                            }
                            else
                            {
                                for (int i = 0; i < listItemNeedInfo.Count; i++)
                                {
                                    if (listItemNeedInfo[i].id == TouchedItemId)
                                    {
                                        if (listItemNeedInfo[i].ShenPiId == 0)
                                        {
                                            ConnectServer(3);
                                        }
                                        else
                                        {
                                            ConnectServer(5);
                                        }
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }
                case ProtoIndexes.CREATE_ALLIANCE_RESP:/** 返回创建联盟结果 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        CreateAllianceResp AllianceCreate = new CreateAllianceResp();
                        t_qx.Deserialize(t_tream, AllianceCreate, AllianceCreate.GetType());

                        switch ((AllianceConnectRespEnum)AllianceCreate.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    CreateNameSave = "";
                                    m_ObjInputName.SetActive(false);
                                    m_ObjCreateSucess.SetActive(true);
                                    m_ListAllianceIcon[2].spriteName = IconSave.ToString();
                                    m_ListApplicationName[2].text = "<" + AllianceCreate.allianceInfo.name + ">";
                                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);//刷新页面
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    ClientMain.m_UITextManager.createText("该名称已被其他联盟使用！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    ClientMain.m_UITextManager.createText("失败，元宝不足！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    ClientMain.m_UITextManager.createText("仅限使用中/英文以及数字！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    ClientMain.m_UITextManager.createText("输入的名称过长！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    ClientMain.m_UITextManager.createText("失败找不到所选的Icon！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    ClientMain.m_UITextManager.createText("输入的名称包含敏感词！");
                                }
                                break;

                            default:
                                break;
                        }
                        return true;
                    }
                case ProtoIndexes.FIND_ALLIANCE_RESP:/** 返回查找联盟结果 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        FindAllianceResp AllianceFindInfo = new FindAllianceResp();

                        t_qx.Deserialize(t_tream, AllianceFindInfo, AllianceFindInfo.GetType());

                        switch ((AllianceConnectRespEnum)AllianceFindInfo.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    m_LabInputAllianceID.text = "";
                                    m_ObjSearchAlliance.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjAllianceInfo.SetActive(true);
                                    m_ObjButtonParent.SetActive(false);
                                    m_ObjTitleMain.SetActive(false);
                                    ShowInfo(1);
                                    m_ObjCloseInfo.SetActive(true);
                                    ShowAllianceSearchInfo(AllianceFindInfo);

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    _title = "查找失败";
                                    _content1 = "很遗憾，找不到这个联盟";
                                    _content2 = "";
                                    _SignalIndex = 2;
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                                }
                                break;
                            default:
                                break;
                        }
                        return true;
                    }
                case ProtoIndexes.APPLY_ALLIANCE_RESP: /** 返回申请联盟结果 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ApplyAllianceResp AllianceApply = new ApplyAllianceResp();
                        t_qx.Deserialize(t_tream, AllianceApply, AllianceApply.GetType());
                        switch ((AllianceConnectRespEnum)AllianceApply.code)
                        {

                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    AllianceData.Instance.m_AllianceInfoDic[AllianceApply.id].isApplied = true;
                                    _AppliedCount = 0;
                                    foreach (KeyValuePair<int, NonAllianceInfo> item in AllianceData.Instance.m_AllianceInfoDic)
                                    {
                                        if (item.Value.isApplied)
                                        {
                                            _AppliedCount++;
                                        }
                                    }
                                    AllianceData.Instance.NoAlliance = true;
                                    AllianceData.Instance.m_InstantiateNoAlliance = true;


                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    UIBoxLoadCallbackZero);
                                    //  AllianceInfoShow();
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                      UIBoxLoadCallbackOne);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    _title = "申请失败";
                                    _content1 = "很遗憾，找不到这个联盟...";
                                    _content2 = "";
                                    _SignalIndex = 2;
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    _title = "申请失败";
                                    _content1 = "联盟人数已满...";
                                    _content2 = "";
                                    _SignalIndex = 2;
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallbackFour);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                       UIBoxLoadCallbackFive);

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    _title = "申请失败";
                                    _content1 = " 您同时只能提出3份入盟申请！";
                                    _content2 = "";
                                    _SignalIndex = 2;
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallbackFour);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_NINE:
                                {

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallback_Time);
                                }
                                break;
                            default:
                                break;
                        }

                        //Refresh alliance


                        return true;
                    }
                case ProtoIndexes.CANCEL_JOIN_ALLIANCE_RESP: /** 返回取消申请联盟结果 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        CancelJoinAllianceResp AllianceCancelApply = new CancelJoinAllianceResp();
                        t_qx.Deserialize(t_tream, AllianceCancelApply, AllianceCancelApply.GetType());

                        switch ((AllianceConnectRespEnum)AllianceCancelApply.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    AllianceData.Instance.m_AllianceInfoDic[AllianceCancelApply.id].isApplied = false;
                                    _AppliedCount = 0;
                                    foreach (KeyValuePair<int, NonAllianceInfo> item in AllianceData.Instance.m_AllianceInfoDic)
                                    {
                                        if (item.Value.isApplied)
                                        {
                                            _AppliedCount++;
                                        }
                                    }
                                    AllianceData.Instance.NoAlliance = true;
                                    AllianceData.Instance.m_InstantiateNoAlliance = true;


                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    //  m_ListObj[15].SetActive(true);
                                }
                                break;

                            default:
                                break;
                        }
                        return true;
                    }
                case ProtoIndexes.IMMEDIATELY_JOIN_RESP: /** 立刻加入联盟返回 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        immediatelyJoinResp AllianceQuickApply = new immediatelyJoinResp();
                        t_qx.Deserialize(t_tream, AllianceQuickApply, AllianceQuickApply.GetType());
                        switch ((AllianceConnectRespEnum)AllianceQuickApply.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    MainCityUI.TryRemoveFromObjectList(m_MainObj);
                                    JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                                    AllianceData.Instance.IsAllianceNotExist = false;
                                    if (Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_MAINCITY))
                                    {
                                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                                        GameObject obj = new GameObject();
                                        obj.name = "MainCityUIButton_104";
                                        MainCityUI.m_MainCityUI.MYClick(obj);
                                    }
                                    else
                                    {
                                        ClientMain.m_UITextManager.createText("联盟加入成功！");
                                    }

                                    Destroy(m_MainObj);

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallback_ShenPi);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    _title = "查找失败";
                                    _content1 = "很遗憾，找不到这个联盟...";
                                    _content2 = "";
                                    _SignalIndex = 2;
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    // m_ListObj[14].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallback_WeiKaiQi);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);
                                    //  m_ListObj[15].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                       UIBoxLoadCallbackFive);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    m_ObjAllianceInfo.SetActive(false);
                                    m_ObjButtonParent.SetActive(true);
                                    m_ObjTitleMain.SetActive(true);
                                    ShowInfo();
                                    m_ObjCloseInfo.SetActive(false);

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallbackFour);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_NINE:
                                {

                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                             UIBoxLoadCallback_Time);
                                }
                                break;


                            default:
                                break;
                        }
                        return true;
                    }
                //case ProtoIndexes.S_ALLIANCE_INVITE_LIST: /** 邀请的联盟列表 **/
                //    {
                //        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        InviteList invitetem = new InviteList();
                //        t_qx.Deserialize(t_tream, invitetem, invitetem.GetType());
                //        _listYaoQingInfo.Clear();
                //        if (invitetem.inviteInfo != null)
                //        {
                //            for (int i = 0; i < invitetem.inviteInfo.Count; i++)
                //            {
                //                AllianceYaoQingInfo ayqInfo = new AllianceYaoQingInfo();
                //                ayqInfo.time = invitetem.inviteInfo[i].date;
                //                Debug.Log("datedatedatedate ::::" + invitetem.inviteInfo[i].date);
                //                ayqInfo.id = invitetem.inviteInfo[i].id;
                //                ayqInfo.name = invitetem.inviteInfo[i].name;
                //                ayqInfo.level = invitetem.inviteInfo[i].level;
                //                _listYaoQingInfo.Add(ayqInfo);
                                
                //            }
                //            m_ObjSig.SetActive(true);
                //            m_LabObjSig.text = invitetem.inviteInfo.Count.ToString();
                //            if (index_YQRequest > 0)
                //            {
                //                m_MainCancel.SetActive(false);
                //                m_YaoQingObjCancel.SetActive(true);
                //                m_YaoQingObj.SetActive(true);
                //                CreateYaoQing();
                //            }

                //        }
                //        else if (index_YQRequest > 0)
                //        {
                //            m_ObjSig.SetActive(false);
                //            m_MainCancel.SetActive(false);
                //            m_YaoQingObjCancel.SetActive(true);
                //            m_YaoQingObj.SetActive(true);
                //        }
                //        return true;
                //    }
                case ProtoIndexes.S_ALLIANCE_INVITE_REFUSE: /** 拒绝联盟邀请返回 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        RefuseInviteResp invitetem = new RefuseInviteResp();
                        t_qx.Deserialize(t_tream, invitetem, invitetem.GetType());
                        if (_YaoQingDict.ContainsKey(invitetem.lianMengId))
                        {
                            Destroy(_YaoQingDict[invitetem.lianMengId]);
                        }
                        MainCityUI.SetButtonNum(104, _YaoQingDict.Count);
                        _YaoQingDict.Remove(invitetem.lianMengId);
                        if (_YaoQingDict.Count == 0)
                        {
                           
                            m_ObjSig.SetActive(false);
                        }
                        else
                        {
                            m_LabObjSig.text = _YaoQingDict.Count.ToString();
                        }
                        m_GrideIcon.repositionNow = true;
                        return true;
                    }
                case ProtoIndexes.S_ALLIANCE_INVITE_AGREE: /** 同意联盟邀请 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        immediatelyJoinResp AllianceQuickApply = new immediatelyJoinResp();
                        t_qx.Deserialize(t_tream, AllianceQuickApply, AllianceQuickApply.GetType());
                        switch ((AllianceConnectRespEnum)AllianceQuickApply.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    MainCityUI.TryRemoveFromObjectList(m_MainObj);
                                    JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;

                                    GameObject obj = new GameObject();
                                    obj.name = "MainCityUIButton_104";
                                    MainCityUI.m_MainCityUI.MYClick(obj);
                                    Destroy(m_MainObj);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    ClientMain.m_UITextManager.createText("失败：联盟需要审批！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {

                                    ClientMain.m_UITextManager.createText("很遗憾，找不到这个联盟！");

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    ClientMain.m_UITextManager.createText("玩家申请的联盟数量已经满了！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    ClientMain.m_UITextManager.createText("失败，联盟未开启招募！");

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    ClientMain.m_UITextManager.createText("失败，该联盟人数已经满员！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    ClientMain.m_UITextManager.createText("失败君主等级不满足！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    ClientMain.m_UITextManager.createText("失败，军衔等级不满足！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_EIGHT:
                                {
                                    int size_yaoQing = _listYaoQingInfo.Count;
                                    for (int i = 0; i < 99; i++)
                                    {
                                        if (_listYaoQingInfo[i].id == _IndexId)
                                        {
                                            ShowSwitch(_listYaoQingInfo[i].guojia);
                                            return true;
                                        }
                                    }

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_NINE:
                                {
                                    ClientMain.m_UITextManager.createText("间隔时间不到！");
                                }
                                break;


                            default:
                                break;
                        }
                        return true;
                    }
            }
        }
        return false;
    }
    public void ShowInfo(int index= 0)
    {
        if (index == 1)
        {
            m_ObjTopLeft.GetComponentInChildren<MainCityLTTitle>().m_labelTitle.spacingX = 0;
            m_ObjTopLeft.GetComponentInChildren<MainCityLTTitle>().m_labelTitle.text = "[b]联盟信息[-]";
        }
        else
        {
            m_ObjTopLeft.GetComponentInChildren<MainCityLTTitle>().m_labelTitle.spacingX = 13;
            m_ObjTopLeft.GetComponentInChildren<MainCityLTTitle>().m_labelTitle.text = "[b]联盟[-]";
        }
    }
    public void UIBoxLoadCallbackJoined(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();

        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);


        uibox.setBox(_strTitle, MyColorData.getColorString(1, _strContent1), MyColorData.getColorString(1, _strContent2), null, confirmStr, null, null);
    }

    public void UIBoxLoadCallbackOnea(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);


        uibox.setBox(_strTitle, MyColorData.getColorString(1, _strContent1), MyColorData.getColorString(1, _strContent2), null, confirmStr, null, null);
    }
    public void UIBoxLoadCallback_WeiKaiQi(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_APPLICATION_FAILURE_NO_OPEN);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    public void UIBoxLoadCallback_ShenPi(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_APPLICATION_FAILURE_NEED_APPROVAL);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }
    public void UIBoxLoadCallbackZero(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_SUCCESS_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_SUCCESS_TAG_1);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_SUCCESS_TAG_2);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, confirmStr, null, null);
    }

    public void UIBoxLoadCallbackOne(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_APPLICATIONED);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    public void UIBoxLoadCallbackFour(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TAG_MILLITARYRANK_INSUFFICIENT);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    public void UIBoxLoadCallbackFive(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TAG_LEVEL_INSUFFICIENT);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    
    int index_num2 = 0;
    void ShowAllianceIconInfo()
    {
        index_num2 = 0;
        int size = m_GrideIcon.transform.childCount;
        if (size == 0)
        {
            for (int i = 0; i < AllianceIconTemplate.templates.Count; i++)
            {
                //index_num2 = i;
                Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE_ICON_ITEM), 
				                        ResourcesLoadCallBack2 );
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
            {
                m_GrideIcon.transform.GetChild(i).GetComponent<AllianceSelectIcon>().m_SpriteGou.gameObject.SetActive(false);
            }
        }
    
    }
    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.name = index_num2.ToString();
        tempObject.transform.parent = m_GrideIcon.transform;
        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.GetComponent<AllianceSelectIcon>().ShowIcon(AllianceIconTemplate.templates[index_num2].icon, SelectedObj);
        m_GrideAllianceList.repositionNow = true;
        if (index_num2 < AllianceIconTemplate.templates.Count - 1)
        {
            index_num2++;
        }
        else
        {
          
        }
        m_GrideIcon.Reposition();
    }

    void ShowAllAlliancesInfo()
    {
        listAllianceInfo.Clear();


        
        int size = m_GrideAllianceList.transform.childCount;

        for (int i = 0; i < size; i++)
        {
            Destroy(m_GrideAllianceList.transform.GetChild(i).gameObject);
        }
        _AppliedCount = 0;
        foreach (KeyValuePair<int, NonAllianceInfo> item in AllianceData.Instance.m_AllianceInfoDic)
        {
            if (item.Value.isApplied)
            {
                _AppliedCount++;
            }
            listAllianceInfo.Add(item.Value);
        }
        AlliancesInfoTidy();
    }
    void AlliancesInfoTidy()
    {
        for (int i = 0; i < listAllianceInfo.Count; i++)
        {
            for (int j = 0; j < listAllianceInfo.Count - 1 - i; j++)
            {
                if (listAllianceInfo[j].level < listAllianceInfo[j + 1].level)
                {
                    NonAllianceInfo aliance = new NonAllianceInfo();
                    aliance = listAllianceInfo[j];
                    listAllianceInfo[j] = listAllianceInfo[j + 1];
                    listAllianceInfo[j + 1] = aliance;
                }
                else if (listAllianceInfo[j].exp  == listAllianceInfo[j + 1].exp)
                {
                    if (listAllianceInfo[j].exp < listAllianceInfo[j + 1].exp)
                    {
                        NonAllianceInfo aliance = new NonAllianceInfo();
                        aliance = listAllianceInfo[j];
                        listAllianceInfo[j] = listAllianceInfo[j + 1];
                        listAllianceInfo[j + 1] = aliance;
                    }
                    else if (listAllianceInfo[j].exp == listAllianceInfo[j + 1].exp)
                    {
                        if (listAllianceInfo[j].members < listAllianceInfo[j + 1].members)
                        {
                            NonAllianceInfo aliance = new NonAllianceInfo();
                            aliance = listAllianceInfo[j];
                            listAllianceInfo[j] = listAllianceInfo[j + 1];
                            listAllianceInfo[j + 1] = aliance;
                        }
                    }
                }
            }
        }
        NeedInfTidy();
    }


    void NeedInfTidy()
    {
        listItemNeedInfo.Clear();
        foreach (NonAllianceInfo item in listAllianceInfo)
        {
            AllianceItemInfo aii = new AllianceItemInfo();
            aii.id = item.id;
            aii.name = item.name;
            aii.level = item.level;
            aii.applyLevel = item.applyLevel;
            aii.shengwang = item.reputation;
            aii.mengzhu = item.creatorName;
            aii.country = item.country;
            aii.isApply = item.isApplied;
            aii.ShenPiId = item.isShenPi;
            aii.isCanApply = item.isCanApply;
            aii.Ren_Now = item.members;
            aii.Ren_Max = item.memberMax;
           aii.Exp = item.exp;
            listItemNeedInfo.Add(aii);
        }
        listAllianceInfo.Clear();
       
            CreateAllianceItem();
    }
    int index_Nu = 0;
    void CreateAllianceItem()
    {
        int size = listItemNeedInfo.Count;
        //Debug.Log("listAllianceInfo.CountlistAllianceInfo.CountlistAllianceInfo.Count ::" + listAllianceInfo.Count);
        index_Nu = 0;
        if (size < 5)
        {
            m_ScrollViewItem.enabled = false;
        }
        else
        {
            m_ScrollViewItem.enabled = true;
        }

        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_NO_SELF_ALLIANCE_ITEM), 
			                        ResourcesLoadCallBack);
        }
    }
    public struct AllianceInfo
    {
        public int _id;
        public string _name;
        public int _reputation;
        public string _creatorName;
        public bool _isApplied;
    };
    public void ResourcesLoadCallBack(ref WWW p_www,string p_path,Object p_object)
    {
        if (m_GrideAllianceList != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_GrideAllianceList.transform;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.GetComponent<AllianceItemManagerment>().ShowAllianceItem(listItemNeedInfo[index_Nu], ShowAllianceApplicationInfo, TouchedAppLication);
            m_GrideAllianceList.repositionNow = true;
            if (index_Nu < listItemNeedInfo.Count - 1)
            {
                index_Nu++;
            }
        }
        else
        {
            p_object = null;
        }
    }
    void ShowAllianceSearchInfo(FindAllianceResp tempinfo)
    {
        TouchedItemId = tempinfo.allianceInfo.id;
        _AllianceCountry = tempinfo.allianceInfo.country;

        m_SpriteCountry.spriteName = "nation_" + tempinfo.allianceInfo.country.ToString();
        if (tempinfo.isAllow == 0)
        {
      
            m_ObjAllianceInfo.SetActive(true);
            m_ObjTitleMain.SetActive(false);
            m_ObjButtonParent.SetActive(false);
            ShowInfo(1);
            m_ObjCloseInfo.SetActive(true);
            m_listEvent[3].gameObject.SetActive(false);
            m_listEvent[4].gameObject.SetActive(false);
            m_listEvent[5].gameObject.SetActive(false);
            m_listEvent[7].gameObject.SetActive(true);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.level < tempinfo.allianceInfo.applyLevel
                    || JunZhuData.Instance().m_junzhuInfo.junXian < tempinfo.allianceInfo.junXian 
                    || tempinfo.allianceInfo.members >= tempinfo.allianceInfo.memberMax)
            {

                m_ObjAllianceInfo.SetActive(true);
                m_ObjButtonParent.SetActive(false);
                m_ObjTitleMain.SetActive(false);
                ShowInfo(1);
                m_ObjCloseInfo.SetActive(true);
                m_listEvent[3].gameObject.SetActive(false);
                m_listEvent[4].gameObject.SetActive(false);
                m_listEvent[5].gameObject.SetActive(false);
                m_listEvent[7].gameObject.SetActive(true);
            }
            else
            {
                if (tempinfo.allianceInfo.isApplied)
                {

                    m_ObjAllianceInfo.SetActive(true);
                    m_ObjButtonParent.SetActive(false);
                    m_ObjTitleMain.SetActive(false);
                    ShowInfo(1);
                    m_ObjCloseInfo.SetActive(true);
                    m_listEvent[3].gameObject.SetActive(false);
                    m_listEvent[4].gameObject.SetActive(true);
                    m_listEvent[5].gameObject.SetActive(false);
                    m_listEvent[7].gameObject.SetActive(false);
                }
                else
                {

                    m_ObjAllianceInfo.SetActive(true);
                    m_ObjButtonParent.SetActive(false);
                    m_ObjTitleMain.SetActive(false);
                    ShowInfo(1);
                    m_ObjCloseInfo.SetActive(true);

                    if (tempinfo.allianceInfo.isShenPi == 1)
                    {
                        m_listEvent[3].gameObject.SetActive(false);
                        m_listEvent[4].gameObject.SetActive(false);
                        m_listEvent[5].gameObject.SetActive(true);
                        m_listEvent[7].gameObject.SetActive(false);
                    }
                    else
                    {
                        m_listEvent[3].gameObject.SetActive(true);
                        m_listEvent[4].gameObject.SetActive(false);
                        m_listEvent[5].gameObject.SetActive(false);
                        m_listEvent[7].gameObject.SetActive(false);
                    }
                }
            }
        }

        m_ListApplicationInfo[0].text = "<" + tempinfo.allianceInfo.name + ">\n" + "(ID:" + tempinfo.allianceInfo.id + ")";
        m_ListApplicationInfo[1].text = tempinfo.allianceInfo.level.ToString() + NameIdTemplate.GetName_By_NameId(990019);
        m_ListApplicationInfo[4].text = tempinfo.allianceInfo.reputation.ToString();
        m_ListApplicationInfo[3].text = tempinfo.allianceInfo.members.ToString() + "/" + tempinfo.allianceInfo.memberMax.ToString();
        m_ListApplicationInfo[2].text = tempinfo.allianceInfo.creatorName;
        m_ListApplicationInfo[5].text = tempinfo.allianceInfo.applyLevel.ToString();
        m_ListApplicationInfo[6].text = NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(tempinfo.allianceInfo.junXian).templateName);
        if (!string.IsNullOrEmpty(tempinfo.allianceInfo.attchCndition))
        {
            m_ListApplicationInfo[7].text = tempinfo.allianceInfo.attchCndition;
        }
        else
        {
            m_ListApplicationInfo[7].text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_NOTICE_DEFAULT);
        }
        m_ListAllianceIcon[0].spriteName = tempinfo.allianceInfo.icon.ToString();

    }
    void ShowAllianceApplicationInfo(int id)
    {
        if (AllianceData.Instance.m_AllianceInfoDic.ContainsKey(id))
        {
            TouchedItemId = id;
            _AllianceCountry = AllianceData.Instance.m_AllianceInfoDic[id].country;
            m_SpriteCountry.spriteName = "nation_" + AllianceData.Instance.m_AllianceInfoDic[id].country.ToString();
            {
                if (JunZhuData.Instance().m_junzhuInfo.level < AllianceData.Instance.m_AllianceInfoDic[id].applyLevel
                       || JunZhuData.Instance().m_junzhuInfo.junXian < AllianceData.Instance.m_AllianceInfoDic[id].junXian
                       || AllianceData.Instance.m_AllianceInfoDic[id].members >= AllianceData.Instance.m_AllianceInfoDic[id].memberMax)
                {
                    m_ObjAllianceInfo.SetActive(true);
                    m_ObjButtonParent.SetActive(false);
                    m_ObjTitleMain.SetActive(false);
                    ShowInfo(1);
                    m_ObjCloseInfo.SetActive(true);
                    m_listEvent[3].gameObject.SetActive(false);
                    m_listEvent[4].gameObject.SetActive(false);
                    m_listEvent[5].gameObject.SetActive(false);
                    m_listEvent[7].gameObject.SetActive(true);
                }
                else
                {
                    if (AllianceData.Instance.m_AllianceInfoDic[id].isApplied)
                    {

                        m_ObjAllianceInfo.SetActive(true);
                        m_ObjButtonParent.SetActive(false);
                        m_ObjTitleMain.SetActive(false);
                        ShowInfo(1);
                        m_ObjCloseInfo.SetActive(true);

                        m_listEvent[3].gameObject.SetActive(false);

                        m_listEvent[4].gameObject.SetActive(true);
                        m_listEvent[5].gameObject.SetActive(false);
                        m_listEvent[7].gameObject.SetActive(false);
                    }
                    else
                    {

                        m_ObjAllianceInfo.SetActive(true);
                        m_ObjButtonParent.SetActive(false);
                        m_ObjTitleMain.SetActive(false);
                        ShowInfo(1);
                        m_ObjCloseInfo.SetActive(true);

                        if (AllianceData.Instance.m_AllianceInfoDic[id].isShenPi == 1)
                        {
                            m_listEvent[3].gameObject.SetActive(false);
                            m_listEvent[4].gameObject.SetActive(false);
                            m_listEvent[5].gameObject.SetActive(true);
                            m_listEvent[7].gameObject.SetActive(false);
                        }
                        else
                        {
                            m_listEvent[3].gameObject.SetActive(true);
                            m_listEvent[4].gameObject.SetActive(false);
                            m_listEvent[5].gameObject.SetActive(false);
                            m_listEvent[10].gameObject.SetActive(false);
                        }
                    }
                }
            }
            m_ListApplicationInfo[0].text = "<" + AllianceData.Instance.m_AllianceInfoDic[id].name + ">\n" + "(ID:" + AllianceData.Instance.m_AllianceInfoDic[id].id + ")";
            m_ListApplicationInfo[1].text = AllianceData.Instance.m_AllianceInfoDic[id].level.ToString() + NameIdTemplate.GetName_By_NameId(990019);
            m_ListApplicationInfo[4].text = AllianceData.Instance.m_AllianceInfoDic[id].reputation.ToString();
            m_ListApplicationInfo[3].text = AllianceData.Instance.m_AllianceInfoDic[id].members.ToString() + "/" + AllianceData.Instance.m_AllianceInfoDic[id].memberMax.ToString();
            m_ListApplicationInfo[2].text = AllianceData.Instance.m_AllianceInfoDic[id].creatorName;
            m_ListApplicationInfo[5].text = AllianceData.Instance.m_AllianceInfoDic[id].applyLevel.ToString();
            m_ListApplicationInfo[6].text = NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(AllianceData.Instance.m_AllianceInfoDic[id].junXian).templateName);


            if (!string.IsNullOrEmpty(AllianceData.Instance.m_AllianceInfoDic[id].attchCndition))
            {
                m_ListApplicationInfo[7].text = AllianceData.Instance.m_AllianceInfoDic[id].attchCndition;
            }
            else
            {
                m_ListApplicationInfo[7].text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_NOTICE_DEFAULT);
            }

            m_ListAllianceIcon[0].spriteName = AllianceData.Instance.m_AllianceInfoDic[id].icon.ToString();
        }
    }

    void SelectedObj(GameObject obj, int id)
    {
        IconSave = id;

        int size = m_GrideIcon.transform.childCount;


        for (int i = 0; i < size; i++)
        {
            if (m_GrideIcon.transform.GetChild(i).name.Equals(obj.name))
            {
                m_GrideIcon.transform.GetChild(i).GetComponent<AllianceSelectIcon>().m_SpriteGou.gameObject.SetActive(true);
            }
            else
            {
                m_GrideIcon.transform.GetChild(i).GetComponent<AllianceSelectIcon>().m_SpriteGou.gameObject.SetActive(false);
            }
        }

    }

    void OnDestroy()
    {
        m_No_AllianceLayer = null;
        SocketTool.UnRegisterMessageProcessor(this);
    }

    private int _guojiaId;
    void TouchedAppLication(int country_id,int alliance_id)
    {
        TouchedItemId = alliance_id;
        if (_AppliedCount < 3)
        {
            if (country_id >= 0)
            {
                _guojiaId = country_id;
                for (int i = 0; i < listItemNeedInfo.Count; i++)
                {
                    if (listItemNeedInfo[i].id == alliance_id)
                    {
                        if (listItemNeedInfo[i].applyLevel <= JunZhuData.Instance().m_junzhuInfo.level)
                        {
                            ShowSwitch(_guojiaId);
                        }
                        else
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                               UIBoxLoadCallbackFive);
                        }
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listItemNeedInfo.Count; i++)
                {
                    if (listItemNeedInfo[i].id == alliance_id)
                    {
                        if (listItemNeedInfo[i].ShenPiId == 0)
                        {
                            ConnectServer(3);
                        }
                        else
                        {
                            ConnectServer(5);
                        }
                        return;
                    }
                }
            }
        }
        else
        {
            _title = "申请失败";
            _content1 = " 您同时只能提出3份入盟申请！";
            _content2 = "";
            _SignalIndex = 2;
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
        }
    }
    private void ShowSwitch(int _country_id)
    {
        if (BagData.GetMaterialCountByID(910001) > 0)
        {
            m_LabNoETopLeft.text = LanguageTemplate.GetText(20008);
            m_LabNoEBottomLeft.text = LanguageTemplate.GetText(20009);
            m_LabNoEBottomRight.text = "X" + BagData.GetMaterialCountByID(910001);
            m_EnoughObj.SetActive(false);
            m_NoEnoughObj.SetActive(true);
        }
        else
        {
            m_LabsignalUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_3)
               + DangpuItemCommonTemplate.getDangpuItemCommonById(1003).needNum / DangpuItemCommonTemplate.getDangpuItemCommonById(1003).itemNum
               + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_4);
          m_LabsignalDown.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_1)
               + NameIdTemplate.GetName_By_NameId(_country_id)
               + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_2);
            m_NoEnoughObj.SetActive(false);
            m_EnoughObj.SetActive(true);
        }
        m_SwitchCountry.SetActive(true);
        //if (BagData.GetMaterialCountByID(910001) > 0)
        {
            //  m_LabsignalUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD);
           
            //m_LabsignalDown.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_1)
            //    + NameIdTemplate.GetName_By_NameId(_country_id)
            //+ LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_2); ;
           
        }
        //else
        //{

        //    m_LabsignalUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_3)
        //        + DangpuItemCommonTemplate.getDangpuItemCommonById(1003).needNum / DangpuItemCommonTemplate.getDangpuItemCommonById(1003).itemNum
        //        + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_4);
        //    m_LabsignalDown.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_1)
        //        + NameIdTemplate.GetName_By_NameId(_country_id)
        //    + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_2);
        //}
    }
    public void UIBoxLoadCallback_Time(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_APPLY_HOUR);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }

    void SwitchCountry(int index)
    {
        if (index == 0)
        {
          m_SwitchCountry.SetActive(false);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao > DangpuItemCommonTemplate.getDangpuItemCommonById(1003).needNum / DangpuItemCommonTemplate.getDangpuItemCommonById(1003).itemNum || BagData.GetMaterialCountByID(910001) > 0)
            {
                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer t_serializer = new QiXiongSerializer();
                ChangeGuojiaReq temp = new ChangeGuojiaReq();
                temp.guojiaId = _guojiaId;
                if (BagData.GetMaterialCountByID(910001) > 0)
                {
                    temp.useType = 0;
                }
                else
                {
                    temp.useType = 1;
                }
                t_serializer.Serialize(tempStream, temp);

                byte[] t_protof = tempStream.ToArray();

                t_protof = tempStream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ChangeCountry_REQ, ref t_protof);
            }
            else
            {
                EquipSuoData.TopUpLayerTip(m_MainObj,true);
            }
        }
    }
    public void UIBoxLoadCallback_YuanBao(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.AllIANCE_APPLICATION_FAILURE_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_APPLY_HOUR);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null);
    }


    private string _title = "";
    private string _content1 = "";
    private string _content2 = "";
    private int _SignalIndex = 0;
    public void UIBoxLoadRename(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        if (string.IsNullOrEmpty(_title))
        {
            _title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        }
        string TitleStr = _title;

        
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str1 = _content1;

        string str2 = _content2;
        if (_SignalIndex == 8)
        {
            uibox.setBox(TitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, cancelStr, confirmStr, ConFirm);
        }
        else if (_SignalIndex == 2)
        {
            uibox.setBox(TitleStr, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2), null, confirmStr, null, null);
            //uibox.setBox(TitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, null, confirmStr,null);
        }

    }

    void ConFirm(int index)
    {
        if (index == 2)
        {
            if (AllianceData.Instance.m_AllianceCreatePrice <= JunZhuData.Instance().m_junzhuInfo.yuanBao)
            {
                ConnectServer(8);
            }
            else
            {
                EquipSuoData.TopUpLayerTip();
            }
        }
    }
    private int _IndexId = 0;
    void YaoQingChuLi(int index,int id)
    {
        _IndexId = id;
        if (index == 0)
        {
            AllianceData.Instance.RequestAgreeInvite(id);
        }
        else
        {
            AllianceData.Instance.RequestRefuseInvite(id);
        }

    }
    void CreateYaoQing()
    {
        _YaoQingDict.Clear();
        int size_Child = m_GridParent.transform.childCount;
        for (int i = 0; i < size_Child; i++)
        {
            Destroy(m_GridParent.transform.GetChild(i).gameObject);
        }
        int size = _listYaoQingInfo.Count;
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_YAOQING_ITEM), UIBoxLoad_YaoQing);
        }
    }

    int index_YaoQing = 0;
    public void UIBoxLoad_YaoQing(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GridParent != null)
        {
            GameObject tempObject = Instantiate(p_object) as GameObject;
            tempObject.transform.parent = m_GridParent.transform;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.transform.localScale = Vector3.one;
            tempObject.transform.GetComponent<AllianceYQItemManagerment>().ShowInfo(_listYaoQingInfo[index_YaoQing], YaoQingChuLi);
            m_GrideAllianceList.repositionNow = true;
            _YaoQingDict.Add(_listYaoQingInfo[index_YaoQing].id, tempObject);
            if (index_YaoQing < _listYaoQingInfo.Count - 1)
            {
                index_YaoQing++;
            }
       
            m_GridParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }

    }
}
