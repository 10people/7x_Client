using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class AllianceLayerManagerment : MonoBehaviour, SocketProcessor
{
    public UILabel m_LabelTitle0;
    public UILabel m_LabelTitle1;

    public GameObject m_SignalLab;
    public UIScrollView m_ScrollViewItem;
    public UIScrollView m_ScrollView;
    public GameObject m_HidenObject;
    public List<EventIndexHandle> m_listEvent;
    public List<GameObject> m_ListObj;

    public List<EventIndexHandle> m_ListEventZhuanGuo;

    public Dictionary<int, GameObject> m_DictView = new Dictionary<int, GameObject>();
    public UILabel m_LabInputName;
    public UILabel m_LabInputAllianceID;
    private string InputAllianceID = "";
    private string InputName = "";
    public UIGrid m_GrideIcon;
    public UIGrid m_GrideAllianceList;

    public List<UISprite> m_ListAllianceIcon;

    public GameObject m_SwitchCountry;

    public UILabel m_LabsignalUp;
    public UILabel m_LabsignalDown;
    private int TouchedItemId = 0;

    public List<UILabel> m_ListApplicationInfo;
    public List<UILabel> m_ListApplicationName;
    public UILabel m_LabPrice;
    private int IconSave = 0;
    private int removeIndex = 0;
    private bool isUnInput = false;
    private bool isUnSlelectIcon = false;
    private bool isUnInputAllianecID = false;
    private string CreateNameSave = "";
 
    public GameObject m_MainLayer;
    private bool isShowOn = false;

    public struct AllianceItemInfo
    {
        public int id;
        public string name;
        public int level;
        public int shengwang;
        public string mengzhu;
        public int country;
        public bool isApply;
        public int ShenPiId;
    };

    private List<AllianceItemInfo> listItemNeedInfo = new List<AllianceItemInfo>();
    private List<NonAllianceInfo> listAllianceInfo = new List<NonAllianceInfo>();

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
 
        m_LabInputName.text = "";
        m_LabInputAllianceID.text = "";
        m_listEvent.ForEach(p => p.m_Handle += Touch);
        m_ListEventZhuanGuo.ForEach(p => p.m_Handle += SwitchCountry);
        m_LabelTitle0.text = LanguageTemplate.GetText(LanguageTemplate.Text.BUILD_UNION_1);
        m_LabelTitle1.text = LanguageTemplate.GetText(LanguageTemplate.Text.BUILD_UNION_2);
 
        AllianceData.Instance.RequestData();
    }
    public void OnSelect()
    {
        UIInput Input = m_LabInputName.transform.parent.GetComponent<UIInput>();
        if (!string.IsNullOrEmpty(FunctionWindowsCreateManagerment.GetNeedString(m_LabInputName.text)))
        {
            InputName = FunctionWindowsCreateManagerment.GetNeedString(Input.value);
            m_LabInputName.text = InputName;
        }
    }
    void Update()
    {
        if (AllianceData.Instance.m_InstantiateNoAlliance)
        {
           // AllianceData.Instance.m_InstantiateNoAlliance = false;
            m_HidenObject.SetActive(true);
           
            isShowOn = true;
        }
        
        if (isShowOn)
        {
            if (AllianceData.Instance.m_InstantiateNoAlliance)
            {
                isShowOn = false;
                AllianceData.Instance.m_InstantiateNoAlliance = false;
                AllianceInfoShow();
            }

           
        }
       

        if (!string.IsNullOrEmpty(InputName))
        {
//            Debug.Log("InputNameInputNameInputName :::" + InputName);
    
			CreateNameSave = InputName;
        }

        InputAllianceID = m_LabInputAllianceID.text;
        SerchButtonControl();
        CreateNameButtonControl();
    //    IconSelectButtonControl();
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

    void CreateNameButtonControl()
    {
        if (!InputName.Equals(LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_INPUT_SIGNAL)) && !string.IsNullOrEmpty(InputName) && isUnInput && IconSave != 0) 
        {
            isUnInput = false;
            
            ButtonsControl(6, true);
        }
        else if (!isUnInput && (InputName.Equals(LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_INPUT_SIGNAL)) || string.IsNullOrEmpty(InputName) || IconSave == 0))
        {
            isUnInput = true;
            ButtonsControl(6, false);
        }
    }

    void IconSelectButtonControl()
    {
        if (IconSave != 0 && isUnSlelectIcon)
        {
            isUnSlelectIcon = false;
            
            ButtonsControl(7, true);
        }
        else if (!isUnSlelectIcon && IconSave == 0)
        {
            isUnSlelectIcon = true;
 
            ButtonsControl(7, false);
            EventDelegate.Add(m_listEvent[7].transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);
        }
    }

    void AllianceInfoShow()
    {
       // Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        if (AllianceData.Instance.m_AllianceInfoDic.Count == 0)
        {
             
          //  ButtonsControl(1, false);
            m_SignalLab.SetActive(true);

        }
        else
        {
          //  ButtonsControl(1, true);

            m_SignalLab.SetActive(false);
            m_ListObj[13].SetActive(true);
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
        switch ((AllianceButtonNumEnum)index)
        {
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_ALLIANCE:
                {

                    m_LabInputName.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_INPUT_SIGNAL);
                    
                    IconSave = 0;
                    isUnInput = false;
                    isUnSlelectIcon = false;
                    m_ListObj[5].SetActive(true);
                    m_ListObj[6].SetActive(true);
                    m_ListObj[12].SetActive(true);
                    ShowAllianceIconInfo();
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_SEARCH_ALLIANCE:
                {
                    m_ListObj[0].SetActive(true);
                    m_ListObj[1].SetActive(true);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_SEARCH:
                {
                    ConnectServer(index);
                }
                break;

            case AllianceButtonNumEnum.E_ALLIANCE_APPLICATION:
                {
                        ConnectServer(index);
                 //   }
                    //if (AllianceData.Instance.m_AllianceAppliedCount < 3)
                   // {
                    //else
                    //{
                    //    m_ListObj[14].SetActive(true);
                    //}
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CANCEL_APPLICATION:
                {
                    ConnectServer(index);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_QUICK_APPLICATION:
                {
                    ConnectServer(index);
                }
                break;

            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_NAME:
                {
                    m_LabInputName.text = "";
                    if (SysparaTemplate.CompareSyeParaWord(CreateNameSave))
                    {
                        m_ListObj[16].SetActive(true);
                    }
                    else
                    {
                        ConnectServer(index);
                    }
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_SELECT_ALLIANCE_ICON:
                {
                   // m_ListObj[8].SetActive(false);
                    m_ListObj[12].SetActive(false);

                    m_ListObj[9].SetActive(true);
                    m_LabPrice.text = AllianceData.Instance.m_AllianceCreatePrice.ToString();
                    m_ListAllianceIcon[1].spriteName = IconSave.ToString();
                    m_ListApplicationName[1].text = "<" + CreateNameSave + ">";
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_CONSUME_CONFIRM:
                {
                    if (AllianceData.Instance.m_AllianceCreatePrice <= JunZhuData.Instance().m_junzhuInfo.yuanBao)
                    {
                        ConnectServer(index);
                    }
                    else
                    {
                        m_ListObj[9].SetActive(false);
                        m_ListObj[10].SetActive(true);
                    }
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_RECHARGE_CONFIRM:
                {
                    MainCityUI.TryRemoveFromObjectList(m_MainLayer);
                    TopUpLoadManagerment.m_instance.LoadPrefab(false);
                   // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TOPUP_MAIN_LAYER), RB_LR_75_LoadCallback);
                    Destroy(m_MainLayer);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_SEARCH_RESULT_CONFIRM:
                {
                    m_ListObj[4].SetActive(false);
                    m_ListObj[0].SetActive(false);
                  
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_SUCCESS_CONFIRM:
                {
                    //Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                    //PlayerModelController.m_playerModelController.m_isCanUpdatePosition = false;
                    //Vector3 vec_pos = new Vector3(-25.0f, 0, -132.0f);
                    //PlayerModelController.m_playerModelController.UploadPlayerPosition(vec_pos);
                    MainCityUI.TryRemoveFromObjectList(m_ListObj[17]);
                    // CityGlobalData.m_isAllianceScene = true;
                    JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                    //SceneManager.EnterAllianceCity();
                    Destroy(m_ListObj[17]);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_SUCCESS_ONSELECT_1:
                {
                    OnSelect();
                    m_listEvent[12].gameObject.SetActive(false);
                }
                break;
            case AllianceButtonNumEnum.E_ALLIANCE_CREATE_SUCCESS_ONSELECT_2:
                {
                    m_listEvent[12].gameObject.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

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

            //case AllianceButtonNumEnum.E_ALLIANCE_CREATE_NAME:
            //    {
            //       // Debug.Log("CreateNameSaveCreateNameSaveCreateNameSave :::" + CreateNameSave);
            //        if (!string.IsNullOrEmpty(CreateNameSave))
            //        {
            //            CheckAllianceName allianceName = new CheckAllianceName();
            //            allianceName.name = CreateNameSave;
            //            t_qx.Serialize(t_tream, allianceName);
            //            byte[] t_protof;
            //            t_protof = t_tream.ToArray();
            //            SocketTool.Instance().SendSocketMessage(ProtoIndexes.CHECK_ALLIANCE_NAME, ref t_protof);
            //        }
            //    }
            //    break;

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
                        Debug.Log("tempResponse.resulttempResponse.resulttempResponse.result ::" + tempResponse.result);
                        if (tempResponse.result == 0)
                        {
                            Debug.Log("TouchedItemIdTouchedItemIdTouchedItemId ::" + TouchedItemId);
                            for (int i = 0; i < listItemNeedInfo.Count; i++)
                            {
                                if (listItemNeedInfo[i].id == TouchedItemId)
                                {
                                    Debug.Log("listAllianceInfo[i].idlistAllianceInfo[i].id ::" + listItemNeedInfo[i].id);
                                    if (listItemNeedInfo[i].ShenPiId == 0)
                                    {
                                        ConnectServer(3);
                                        Debug.Log("333333333333333333333333333333333333333");
                                    }
                                    else
                                    {
                                        ConnectServer(5);
                                        Debug.Log("555555555555555555555555555555555555555555555");
                                    }
                                    return false;
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
                                   // IconSave = 0;
                                    CreateNameSave = "";
                                    m_ListObj[9].SetActive(false);
                                    m_ListObj[11].SetActive(true);
                                    m_ListAllianceIcon[2].spriteName = IconSave.ToString();
                                    m_ListApplicationName[2].text = "<" + AllianceCreate.allianceInfo.name + ">";
                                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);//刷新页面
                                    //    AllianceData.Instance.NoAlliance = true;
                                    //AllianceData.Instance.m_AllianceInfoDic.Add(AllianceCreate.allianceInfo.id, AllianceCreate.allianceInfo);
                                    // AllianceData.Instance.g_UnionInfo = AllianceCreate.allianceInfo;\
                                   // CityGlobalData.m_isAllianceScene = true;
                                    
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ListObj[9].SetActive(false);
                                    m_ListObj[7].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_TITLE);
                                    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_0);
                                    _strContent2 = "";
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UIBoxLoadCallbackOnea);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_TITLE);
                                    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_1);

                                    _strContent2 = "";
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                 UIBoxLoadCallbackOnea);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_TITLE);
                                    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_2);
                                    _strContent2 = "";
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UIBoxLoadCallbackOnea);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    _strTitle = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_TITLE);
                                    _strContent1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TSG_3);
                                    _strContent2 = "";
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UIBoxLoadCallbackOnea);
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
                                    m_ListObj[1].SetActive(false);
                                    m_ListObj[4].SetActive(true);
                                    ShowAllianceSearchInfo(AllianceFindInfo);
                                  
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ListObj[2].SetActive(true);
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

                                    AllianceData.Instance.NoAlliance = true;
                                    AllianceData.Instance.m_InstantiateNoAlliance = true;
                                    m_ListObj[0].SetActive(false);
                                    m_ListObj[4].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    UIBoxLoadCallbackZero);
                                  //  AllianceInfoShow();
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                      UIBoxLoadCallbackOne);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[2].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[15].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallbackFour);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                       UIBoxLoadCallbackFive);
                                   
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[14].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
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
                                    AllianceData.Instance.NoAlliance = true;
                                    AllianceData.Instance.m_InstantiateNoAlliance = true;
                                    
                                    m_ListObj[0].SetActive(false);
                                    m_ListObj[4].SetActive(false);
                                   // AllianceInfoShow();

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ListObj[15].SetActive(true);
                                }
                                break;
                         
                            default:
                                break;
                        }

                        //Refresh alliance

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
                                    MainCityUI.TryRemoveFromObjectList(m_ListObj[17]);
                                    JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                                    // SceneManager.EnterAllianceCity();
                                    Destroy(m_ListObj[17]);
                                   
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallback_ShenPi);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[2].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                   m_ListObj[4].SetActive(false);
                                   m_ListObj[14].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        UIBoxLoadCallback_WeiKaiQi);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[15].SetActive(true);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                       UIBoxLoadCallbackFive);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    m_ListObj[4].SetActive(false);
                                    m_ListObj[0].SetActive(false);
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
            }
        }
        return false;
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


        foreach (KeyValuePair<int, NonAllianceInfo> item in AllianceData.Instance.m_AllianceInfoDic)
        {

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

                if (listAllianceInfo[j].exp > listAllianceInfo[j + 1].exp)
                {

                    NonAllianceInfo aliance = new NonAllianceInfo();
                    aliance = listAllianceInfo[j];
                    listAllianceInfo[j] = listAllianceInfo[j + 1];
                    listAllianceInfo[j + 1] = aliance;
                }
            }
        }
        NeedInfTidy();
    }


    void NeedInfTidy()
    {
        foreach (NonAllianceInfo item in listAllianceInfo)
        {
            AllianceItemInfo aii = new AllianceItemInfo();
            aii.id = item.id;
            aii.name = item.creatorName;
            aii.level = item.level;
            aii.shengwang = item.reputation;
            aii.mengzhu = item.creatorName;
            aii.country = item.country;
            aii.isApply = item.isApplied;
            aii.ShenPiId = item.isShenPi;
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

       // Debug.Log("tempinfo.isAllow   tempinfo.isAllow ::" + tempinfo.isAllow);
        m_ListObj[18].GetComponent<UISprite>().spriteName = "nation_" + tempinfo.allianceInfo.country.ToString();
        if (tempinfo.isAllow == 0 || JunZhuData.Instance().m_junzhuInfo.guoJiaId != tempinfo.allianceInfo.country)
        {
            m_ListObj[0].SetActive(true);
            m_ListObj[4].SetActive(true);
            if (JunZhuData.Instance().m_junzhuInfo.guoJiaId != tempinfo.allianceInfo.country)
            {
                m_ListObj[19].SetActive(true);
            }
            m_listEvent[3].gameObject.SetActive(false);
            m_listEvent[4].gameObject.SetActive(false);
            m_listEvent[5].gameObject.SetActive(false);
            m_listEvent[10].gameObject.SetActive(true);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.level < tempinfo.allianceInfo.applyLevel
                    || JunZhuData.Instance().m_junzhuInfo.junXian < tempinfo.allianceInfo.junXian 
                    || tempinfo.allianceInfo.members >= tempinfo.allianceInfo.memberMax)
            {
                m_ListObj[0].SetActive(true);
                m_ListObj[4].SetActive(true);
                m_listEvent[3].gameObject.SetActive(false);
                m_listEvent[4].gameObject.SetActive(false);
                m_listEvent[5].gameObject.SetActive(false);
                m_listEvent[10].gameObject.SetActive(true);
            }
            else
            {

                if (tempinfo.allianceInfo.isApplied)
                {
                    m_ListObj[0].SetActive(true);
                    m_ListObj[4].SetActive(true);
                    m_ListObj[19].SetActive(false);
                    m_listEvent[3].gameObject.SetActive(false);

                    m_listEvent[4].gameObject.SetActive(true);
                    m_listEvent[5].gameObject.SetActive(false);
                    m_listEvent[10].gameObject.SetActive(false);
                }
                else
                {
                    m_ListObj[0].SetActive(true);
                    m_ListObj[4].SetActive(true);
                    m_ListObj[19].SetActive(false);

                    if (tempinfo.allianceInfo.isShenPi == 1)
                    {
                        m_listEvent[3].gameObject.SetActive(false);
                        m_listEvent[4].gameObject.SetActive(false);
                        m_listEvent[5].gameObject.SetActive(true);
                        m_listEvent[10].gameObject.SetActive(false);
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
        TouchedItemId = id;
        m_ListObj[18].GetComponent<UISprite>().spriteName = "nation_" + AllianceData.Instance.m_AllianceInfoDic[id].country.ToString();
        if (JunZhuData.Instance().m_junzhuInfo.guoJiaId != AllianceData.Instance.m_AllianceInfoDic[id].country)
        {
            m_ListObj[0].SetActive(true);
            m_ListObj[4].SetActive(true);

            m_ListObj[19].SetActive(true);
            m_listEvent[3].gameObject.SetActive(false);
            m_listEvent[4].gameObject.SetActive(false);
            m_listEvent[5].gameObject.SetActive(false);
            m_listEvent[10].gameObject.SetActive(true);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.level < AllianceData.Instance.m_AllianceInfoDic[id].applyLevel
                   || JunZhuData.Instance().m_junzhuInfo.junXian < AllianceData.Instance.m_AllianceInfoDic[id].junXian
                   || AllianceData.Instance.m_AllianceInfoDic[id].members >= AllianceData.Instance.m_AllianceInfoDic[id].memberMax)
            {
                m_ListObj[0].SetActive(true);
                m_ListObj[4].SetActive(true);
                m_listEvent[3].gameObject.SetActive(false);
                m_listEvent[4].gameObject.SetActive(false);
                m_listEvent[5].gameObject.SetActive(false);
                m_listEvent[10].gameObject.SetActive(true);
            }
            else
            {
                if (AllianceData.Instance.m_AllianceInfoDic[id].isApplied)
                {
                    m_ListObj[0].SetActive(true);
                    m_ListObj[4].SetActive(true);
                    m_ListObj[19].SetActive(false);
                    m_listEvent[3].gameObject.SetActive(false);

                    m_listEvent[4].gameObject.SetActive(true);
                    m_listEvent[5].gameObject.SetActive(false);
                    m_listEvent[10].gameObject.SetActive(false);
                }
                else
                {
                    m_ListObj[19].SetActive(false);
                    m_ListObj[0].SetActive(true);
                    m_ListObj[4].SetActive(true);

                    if (AllianceData.Instance.m_AllianceInfoDic[id].isShenPi == 1)
                    {
                        m_listEvent[3].gameObject.SetActive(false);
                        m_listEvent[4].gameObject.SetActive(false);
                        m_listEvent[5].gameObject.SetActive(true);
                        m_listEvent[10].gameObject.SetActive(false);
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
        //m_ListApplicationInfo[0].text = AllianceData.Instance.m_AllianceInfoDic[id].name;
        m_ListApplicationInfo[1].text = AllianceData.Instance.m_AllianceInfoDic[id].level.ToString() + NameIdTemplate.GetName_By_NameId(990019);
        m_ListApplicationInfo[4].text = AllianceData.Instance.m_AllianceInfoDic[id].reputation.ToString();
        m_ListApplicationInfo[3].text = AllianceData.Instance.m_AllianceInfoDic[id].members.ToString() + "/" + AllianceData.Instance.m_AllianceInfoDic[id].memberMax.ToString();
        m_ListApplicationInfo[2].text = AllianceData.Instance.m_AllianceInfoDic[id].creatorName;
        m_ListApplicationInfo[5].text = AllianceData.Instance.m_AllianceInfoDic[id].applyLevel.ToString();
        m_ListApplicationInfo[6].text = NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(AllianceData.Instance.m_AllianceInfoDic[id].junXian).templateName);


        if (!string.IsNullOrEmpty( AllianceData.Instance.m_AllianceInfoDic[id].attchCndition))
        {
            m_ListApplicationInfo[7].text =  AllianceData.Instance.m_AllianceInfoDic[id].attchCndition;
        }
        else
        {
            m_ListApplicationInfo[7].text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_NOTICE_DEFAULT);
        }
        
       // Debug.Log(" AllianceData.Instance.m_AllianceInfoDic[id].attchCndition ::" + AllianceData.Instance.m_AllianceInfoDic[id].attchCndition);
        m_ListAllianceIcon[0].spriteName = AllianceData.Instance.m_AllianceInfoDic[id].icon.ToString();
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
        SocketTool.UnRegisterMessageProcessor(this);
    }

    void RB_LR_75_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
    }

    private int _guojiaId;
    void TouchedAppLication(int country_id,int alliance_id)
    {
        TouchedItemId = alliance_id;
        if (country_id >= 0)
        {
            _guojiaId = country_id;
            if (BagData.GetMaterialCountByID(910001) > 0)
            {
                m_SwitchCountry.SetActive(true);

                m_LabsignalUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD);
                m_LabsignalDown.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_1)
                    + NameIdTemplate.GetName_By_NameId(country_id)
                + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_2); ;
            }
            else
            {
     
                m_LabsignalUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_3) 
                    + DangpuItemCommonTemplate.getDangpuItemCommonById(1003).needNum / DangpuItemCommonTemplate.getDangpuItemCommonById(1003).itemNum
                    + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_4);
                m_LabsignalDown.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_1)
                    + NameIdTemplate.GetName_By_NameId(country_id)
                + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TAG_ADD_2);
                m_SwitchCountry.SetActive(true);
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

    }

}
