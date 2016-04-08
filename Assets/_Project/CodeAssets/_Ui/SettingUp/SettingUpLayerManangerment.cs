using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SettingUpLayerManangerment : MonoBehaviour, SocketProcessor
{
    public static SettingUpLayerManangerment m_SettingUp;
    public ScaleEffectController m_ScaleEffectController;
    public GameObject m_Durable_UI; 
    public GameObject m_DestroyTarget;
    public GameObject m_AllianceHave;
    public GameObject m_MainParent;
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteCountry;
    public UISprite m_JunXian;
    public UIGrid m_GrideCDReward;
    public GameObject m_ObjReward;

    public List<SettingUpButtonController> listSettingButton;

    public List<EventIndexHandle> listEventMainLayer;
    public List<UILabel> listMainLab;

    public List<GameObject> listRenameObject;
    public List<EventIndexHandle> listRenameEvent;

    public List<GameObject> listCDKeyObject;
    public List<EventIndexHandle> listCDKeyEvent;


    public  GameObject m_SwitchAccountObject;
    public  EventIndexHandle m_listSwitchAccountEvent;

 
    public List<EventIndexHandle> m_listChangeCountryEvent;
    public List<SettingUpSelectCountryManagerment> m_listCountrySelect;
    public UISprite m_SpriteCountryCurrent;
    public GameObject m_SpriteCountryHuang;
    public GameObject m_SpriteCountryHuangBack;
    public GameObject m_ChangeCountryLayer;
    public GameObject m_ObjTopLeft;
    public UILabel m_LabelTopUp;

    public UILabel m_LabRenameSignal;
    public UIInput m_CDKeyInput;

    private string RenameInfo = "";
    private string CDkeyInfo = "";

    private string NameSave = "";

    private string _award = "";
    private Dictionary<long, GameObject> BloakedEleDic = new Dictionary<long, GameObject>();
    private List<FunctionWindowsCreateManagerment.RewardInfo> _listReward = new List<FunctionWindowsCreateManagerment.RewardInfo>();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

 
	void Start ()
    {
        m_SettingUp = this;
        m_LabRenameSignal.text = LanguageTemplate.GetText(1604) + MyColorData.getColorString(5, "100") + "元宝。";
        MainCityUI.setGlobalTitle(m_ObjTopLeft, LanguageTemplate.GetText(1528), 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
      //  m_LabelTopUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_SIGNAL);
        listEventMainLayer.ForEach(p => p.m_Handle += EventReception);
        listRenameEvent.ForEach(p => p.m_Handle += RenameReception);
        m_listSwitchAccountEvent.m_Handle += SwitchAccountController;
        listCDKeyEvent.ForEach(p => p.m_Handle += CDKeyReception);
        m_listChangeCountryEvent.ForEach(p => p.m_Handle += ChangeCountryReception);
        //listEventMainLayer[0].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
        //listEventMainLayer[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
        //listEventMainLayer[2].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
        SettingInfoShow();
	}
    public void OnInput()
    {
      
           UIInput input = listRenameObject[1].GetComponent<UILabel>().parent.GetComponent<UIInput>();
      
        RenameInfo = FunctionWindowsCreateManagerment.GetNeedString(input.value);

        listRenameObject[1].SetActive(true);
        listRenameObject[1].GetComponent<UILabel>().text = RenameInfo;
        input.value = RenameInfo;
        if (string.IsNullOrEmpty(RenameInfo))
        {
            listRenameObject[2].SetActive(true);
        }
        else
        {
            listRenameObject[2].SetActive(false);
        }
    }

    public void OnSetInfo()
    {
        m_CDKeyInput.value = UIInput.current.value;
        CDkeyInfo = m_CDKeyInput.value;
        listCDKeyObject[3].GetComponent<UILabel>().text = m_CDKeyInput.value;
        listCDKeyObject[4].SetActive(false);
    }
    void Update()
    {
        UIInput input = listRenameObject[1].GetComponent<UILabel>().parent.GetComponent<UIInput>();
 
        if (!string.IsNullOrEmpty(input.value))
        {
            OnInput();
        }
      CDkeyInfo = m_CDKeyInput.value;
      CreateNameControl();
      CDKeyInfoControl();

        if (FunctionWindowsCreateManagerment.m_isSwitchCountry)
        {
            FunctionWindowsCreateManagerment.m_isSwitchCountry = false;
            RefreshCountryInfo();
        }
    }
    bool createNameIsOn = false;
    bool createNameIsOff = false;
    void CreateNameControl()
    {
       int size = listRenameObject[1].transform.childCount;
       if (size > 0)
       {
           if (!createNameIsOff)
           {
               createNameIsOff = true;
               listRenameObject[2].SetActive(false);
           }

       }
       else
       {
           if (createNameIsOff && string.IsNullOrEmpty(RenameInfo))
           {
               createNameIsOff = false;
               listRenameObject[2].SetActive(true);
           }
       }
    
       if (!string.IsNullOrEmpty(RenameInfo))
       {
       
           if (createNameIsOn)
           {
               createNameIsOn = false;
               listRenameObject[2].SetActive(false);
               listRenameEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
              // ButtonsControl(listRenameEvent[0].gameObject, true);
           }
       }
       else
       {
           if (!createNameIsOn )
           {
               createNameIsOn = true;
               listRenameEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
              // ButtonsControl(listRenameEvent[0].gameObject, false);
           }
       }
    }
    bool CDKeyIsOn = false;
    bool CDKeyIsOff = false;
    void CDKeyInfoControl()
    {
        int size = listCDKeyObject[3].transform.childCount;
        if (size > 0)
        {
            if (!CDKeyIsOff)
            {
                CDKeyIsOff = true;
                listCDKeyObject[4].SetActive(false);
            }
        }
        else
        {
            if (CDKeyIsOff && string.IsNullOrEmpty(CDkeyInfo))
            {
                CDKeyIsOff = false;
                listCDKeyObject[4].SetActive(true);
            }
        }

        if (!string.IsNullOrEmpty(CDkeyInfo))
        {
            listCDKeyObject[4].SetActive(false);
        }

        if (!string.IsNullOrEmpty(CDkeyInfo) && CDkeyInfo.Length >= 8)
        {
            if (CDKeyIsOn)
            {
                CDKeyIsOn = false;
                listCDKeyEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
            
               // ButtonsControl(listCDKeyEvent[0].gameObject, true);
            }
        }
        else
        {
            if (!CDKeyIsOn)
            {
                CDKeyIsOn = true;
                listCDKeyEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                //ButtonsControl(listCDKeyEvent[0].gameObject, false);
               
            }
        }
    }
    GameObject SendObj;
   
    public void SettingInfoShow()
    {
        
        {
              m_JunXian.gameObject.SetActive(true);
              m_JunXian.spriteName = "junxian" + JunZhuData.Instance().m_junzhuInfo.junXian;
        }
     
        m_SpriteCountry.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
        if (CityGlobalData.m_AllianceIsHave)
        {
            listMainLab[0].text = "<" +AllianceData.Instance.g_UnionInfo.name + ">";
            m_AllianceHave.SetActive(true);
        }
        else
        {
            listMainLab[0].text = LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT);
            m_AllianceHave.SetActive(false);
        }
        listMainLab[1].text = "Lv" + JunZhuData.Instance().m_junzhuInfo.level.ToString() + "   " + JunZhuData.Instance().m_junzhuInfo.name;
        listMainLab[2].text = "";
        m_SpriteIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id; ;
        SettingButtonControl();
    }

    void SettingButtonControl()
    {

        for (int i = 0; i < SettingData.Instance().m_listSettingsInfo.Count; i++)
        {
            ShowButtonInfo(i, SettingData.Instance().m_listSettingsInfo[i]);
        }

        if (FunctionWindowsCreateManagerment.m_SettingUpTYpe != FunctionWindowsCreateManagerment.SettingType.NONE)
        {
            StartCoroutine(AutoShowNation());
        }
    }

    IEnumerator AutoShowNation()
    {
        yield return new WaitForSeconds(0.6f);
        switch (FunctionWindowsCreateManagerment.m_SettingUpTYpe)
        {
            case FunctionWindowsCreateManagerment.SettingType.NATION_CHANGE:
                {
                    m_ChangeCountryLayer.SetActive(true);
                    ChangeCountryInfo(JunZhuData.Instance().m_junzhuInfo.guoJiaId);
                    m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                    m_SpriteCountryCurrent.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
                }
                break;

            case FunctionWindowsCreateManagerment.SettingType.NAME_CHANGE:
                {


                }
                break;
            case FunctionWindowsCreateManagerment.SettingType.SWITCH_USER:
                {


                }
                break;
            default:
                break;
        }
        FunctionWindowsCreateManagerment.m_SettingUpTYpe = FunctionWindowsCreateManagerment.SettingType.NONE;
    }
    void ShowButtonInfo(int index, int state)
    {
        if (state == 1)
        {
            listSettingButton[index].m_ObjOn.SetActive(true);
            listSettingButton[index].m_ObjOff.SetActive(false);
            listSettingButton[index].m_IsTurnOn = true;
        }
        else
        {
            listSettingButton[index].m_ObjOn.SetActive(false);
            listSettingButton[index].m_ObjOff.SetActive(true);
            listSettingButton[index].m_IsTurnOn = false;
        }
    }
    private int _Index_Type_Save = 2;
    void EventReception(int index)
    {
        if (index < 3)
        {
           // if (_Index_Type_Save != index)
            {
                //listEventMainLayer[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                //listEventMainLayer[_Index_Type_Save].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                _Index_Type_Save = index;
            }
        }
        switch (index)
        {
            case 0:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(4))
                    {
                        listRenameObject[0].SetActive(true);
                    }
                    else
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad);
                    }
                }
                break;
            case 1:
                {
      
                   Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SWITCH_COUNTRY_ROOT),
                                                                                         SwitchCountry_LoadCallback);
                    // m_ChangeCountryLayer.SetActive(true);

                    //ChangeCountryInfo(JunZhuData.Instance().m_junzhuInfo.guoJiaId);
                    // m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                    // m_SpriteCountryCurrent.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
                }
                break;
            case 2:
                {
                    m_SwitchAccountObject.SetActive(true);
                }
                break;
            case 3:
                {
                    listCDKeyObject[0].SetActive(true);
                }
                break;
            case 4:
            {
                m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
                m_ScaleEffectController.OnCloseWindowClick();
            }
                break;
         
            default:
                break;
        }
    }
    public void SwitchCountry_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 1500, 0);
      //  UI2DTool.Instance.AddTopUI(tempObject);

        UIYindao.m_UIYindao.CloseUI();
    }
    void DoCloseWindow()
    {
        MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
        Destroy(m_DestroyTarget);

    }

    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(600) 
                    + VipFuncOpenTemplate.GetNeedLevelByKey(4).ToString()
                    + LanguageTemplate.GetText(601)
                    + "\n\n" + LanguageTemplate.GetText(700);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);

    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_change_name:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ChangeNameBack tempInfo = new ChangeNameBack();

                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
                        NameSave = tempInfo.name;
                     
                        listRenameEvent[1].GetComponent<Collider>().enabled = true;
                        if (tempInfo.code == 0)
                        {
                         //   listRenameObject[1].SetActive(false);
                           // RenameInfo = "";
                          //  listRenameEvent[2].GetComponent<UIInput>().value = "";
                           // listRenameObject[2].SetActive(true);
                            listMainLab[1].text = "Lv" + JunZhuData.Instance().m_junzhuInfo.level.ToString() + "   " + tempInfo.name;
                            JunZhuData.Instance().m_junzhuInfo.name = NameSave;
                            listRenameObject[0].SetActive(false);
                            //_content1 = LanguageTemplate.GetText(1506);
                            //_content2 = "\n" + NameSave;
                            //_SignalType = 2;
                            // EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                            //EquipSuoData.ShowSignal(null, _content1, _content2);//tanchukuang 

                            ClientMain.m_UITextManager.createText(  "改名成功！");

                        }
                        else if (tempInfo.code == -100)
                        {
                            //_content1 = LanguageTemplate.GetText(1507);
                            //_content2 = "";
                            //_SignalType = 2;
                            //EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                            ClientMain.m_UITextManager.createText("输入的名称过长！");
                        }
                        else if (tempInfo.code == -200)
                        {
                            //_content1 = LanguageTemplate.GetText(1507);
                            //_content2 = "";
                            //_SignalType = 2;
                            //EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                            ClientMain.m_UITextManager.createText("仅限使用中/英文以及数字！");
                        }
                        else if (tempInfo.code == -300)
                        {
                            //_content1 = LanguageTemplate.GetText(1508);
                            //_content2 = "";
                            //_SignalType = 2;
                            //EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);

                            ClientMain.m_UITextManager.createText("该名称已被其他玩家使用！");
                        }
                        else if (tempInfo.code == -400)
                        {
                            //_content1 = LanguageTemplate.GetText(1509);
                            //_content2 = "";
                            //_SignalType = 2;
                            //EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                            ClientMain.m_UITextManager.createText(  LanguageTemplate.GetText(1509));
                        }
                        else if (tempInfo.code == -500)
                        {
                            //_content1 = LanguageTemplate.GetText(1509);
                            //_content2 = "";
                            //_SignalType = 2;
                            //EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));
                            //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                  
                              ClientMain.m_UITextManager.createText("输入的名称包含敏感词！");
                        }
                        else if (tempInfo.code > 0)
                        {
                            EquipSuoData.TopUpLayerTip(m_MainParent);
                        }
                        if (tempInfo.code < 0 || tempInfo.code > 0)
                        {
                            listRenameObject[1].SetActive(true);
                        }
                        return true;
                    }
                case ProtoIndexes.S_CDKEY_RES:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GetCDKeyAwardResp tempResponse = new GetCDKeyAwardResp();
                     
                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());

                        if (tempResponse.result == 0)
                        {
                            listCDKeyObject[0].SetActive(false);
                            _award = "";
                          for (int i = 0; i < tempResponse.awards.Count;i++)
                          {
                                if (i < tempResponse.awards.Count - 1)
                                {
                                    _award += tempResponse.awards[i].awardType + ":" + tempResponse.awards[i].awardId + ":" + tempResponse.awards[i].awardNum+ "#";
                                }
                                else
                                {
                                    _award += tempResponse.awards[i].awardType + ":" + tempResponse.awards[i].awardId + ":" + tempResponse.awards[i].awardNum;
                                }

                          }
                             _listReward = FunctionWindowsCreateManagerment.GetRewardInfo(_award);
                            m_ObjReward.SetActive(true);
                            Create();
                         
                        }
                        else
                        {
                            ClientMain.m_UITextManager.createText(tempResponse.errorMsg);
                            //EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                            //                             , tempResponse.errorMsg
                            //                             , "");
                        }
                        
                        return true;
                    }


                //case ProtoIndexes.S_ChangeCountry_RESP:
                //    {
                //        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        ChangeGuojiaResp tempResponse = new ChangeGuojiaResp();

                //        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
 
                //        if (tempResponse.result == 0)
                //        {
                //            RefreshCountryInfo(SaveNum);
                //            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadZhuangGuoSuccess);
                            
                //        }
                //        else if (tempResponse.result == 101)
                //        {
                //          Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadHaveLianMeng);
                //        }
                //        else if (tempResponse.result == 102)
                //        {
                           
                //        }
                //        return true;
                //    }
                default:
                     return false;
            }
        }
        return false;
    }

    void RenameReception(int index)
    {
        switch (index)
        {
            case 0:
                {
                    //if (SysparaTemplate.CompareSyeParaWord(RenameInfo))
                    //{
                    //    listRenameObject[0].SetActive(false);
                    //    listRenameObject[8].SetActive(true);
                    //    _content1 = LanguageTemplate.GetText(1503);
                    //    _content2 = "";
                    //    _SignalType = 2;
                    //    EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));

                    //  //  Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);

                    //}
                    //else
                    {
                        //   //listRenameObject[0].SetActive(false);
                        //  // listRenameObject[1].SetActive(true);
                        //   _content1 = LanguageTemplate.GetText(1504) + "100" + LanguageTemplate.GetText(1505);
                        //   _content2 = "";
                        //  _SignalType = 1;
                        ////   EquipSuoData.ShowSignal(_title, MyColorData.getColorString(1, _content1), MyColorData.getColorString(1, _content2));

                        //   Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadRename);
                        Rename_Cost(2);
                    }   
                }
                break;

            case 12:
                {
                    OnInput();
                    listRenameEvent[1].gameObject.SetActive(false);
                }
                break;
            case 13:
                {
                   
                    listRenameObject[2].SetActive(false);
                    listRenameEvent[1].gameObject.SetActive(true);
                }
                break;
            case 1:
                {
                    listRenameObject[0].SetActive(false);
                }
                break;

            default:
                break;
        }
    }
  
    int index_NUm = 0;

    long JunZhuIDSaved = 0;
    void CallBack(long id)
    {
        JunZhuIDSaved = id;
    }
    void CDKeyReception(int index)
    {
        switch (index)
        {
            case 0:
                {
                   CDKeyController(CDkeyInfo);
                }
                break;
            case 1:
                {
                    int size = m_GrideCDReward.transform.childCount;
                    for (int i = 0; i < size; i++)
                    {
                        Destroy(m_GrideCDReward.transform.GetChild(i).gameObject);
                    }
                    m_ObjReward.SetActive(false);
                    FunctionWindowsCreateManagerment.ShowRAwardInfo(_award);
                    //listCDKeyObject[0].SetActive(true);
                    //listCDKeyObject[1].SetActive(false);
                }
                break;
            case 2:
                {
                    listCDKeyObject[2].SetActive(false);
                }
                break;
            default:
                break;
        }
    }
    void CDKeyController(string cdKey)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        GetCDKeyAwardReq cdkey = new GetCDKeyAwardReq();
        cdkey.cdkey = cdKey;
        t_qx.Serialize(t_tream, cdkey);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_CDKEY_REQ, ref t_protof);
    }

    public static void SwitchAccountController( int index = 0 )
    {
        WindowBackShowController.m_SaveEquipBuWei = 0;
        PlayerPrefs.DeleteKey("UserNameAndPassWord");
        FunctionWindowsCreateManagerment.SetSelectEquipDefault();
        SceneManager.m_isSequencer = false;
		if (UIYindao.m_UIYindao != null && UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        EquipSuoData.Instance().m_listNoShow.Clear();
        CityGlobalData.m_isAllianceTenentsScene = false;
        CityGlobalData.m_isWashMaxSignal = false;
        SceneManager.m_isSequencer = false;
        CityGlobalData.m_isAllianceTenentsScene = false;

		{
			SceneManager.RequestEnterLogin();
		}
    }

    private int SaveNum = 0;
    private int _index_Touch_num = 0;
    void ChangeCountryReception(int index)
    {
        if (_index_Touch_num != index)
        {
            _index_Touch_num = index;
            switch (index)
            {
                case 0:
                    {
                        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadHaveLianMeng);
                        }
                        else
                        {
                            if (BagData.Instance().GetCountItemShiYongId(910001) == 0)
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadNOZhuanGuoKa);
                            }
                            else
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadZhuanGuoKaUse);
                            }

                        }
                    }
                    break;
                default:
                    {
                        SaveNum = index;
                        ChangeCountryInfo(index);
                    }
                    break;
            }
        }
    }
    public void UIBoxLoadHaveLianMeng(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_1) + AllianceData.Instance.g_UnionInfo.name +  LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_2);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_3);
        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null);

    }


    public void UIBoxLoadNOZhuanGuoKa(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr =LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = "\n\n" + LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_CARD_1);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);

    }
    public void UIBoxLoadZhuanGuoKaUse(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str = "\n\n" + LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_CARD_USE);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, ZhuanGuo);

    }
    private string _title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
    private string _content1 = "";
    private string _content2 = "";
    private int _SignalType = 0;
    public void UIBoxLoadRename(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string  TitleStr = _title;
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str1 = _content1;

        string str2 = _content2;
        if (_SignalType == 1)
        {
            uibox.setBox(TitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, cancelStr, confirmStr, Rename_Cost);
        }
        else if (_SignalType == 2)
        {
            EquipSuoData.ShowSignal(TitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2));
           //ibox.setBox(TitleStr, MyColorData.getColorString(1, str1), MyColorData.getColorString(1, str2), null, null, confirmStr,null);
        }

    }
    void Rename_Cost(int index)
    {
        if (index == 2)
        {
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= 100)
            {
                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer t_serializer = new QiXiongSerializer();
                ChangeName temp = new ChangeName();
                temp.name = RenameInfo;
                t_serializer.Serialize(tempStream, temp);

                byte[] t_protof = tempStream.ToArray();

                t_protof = tempStream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_change_name, ref t_protof);
            }
            else
            {
                EquipSuoData.ShowSignal(null, "您的元宝不足！");
                //EquipSuoData.TopUpLayerTip(m_MainParent);
            }
        }

    }

    void ZhuanGuo(int index)
    {
        if (index == 2)
        {
            if (BagData.GetMaterialCountByID(910001) > 0)
            {
                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer t_serializer = new QiXiongSerializer();
                ChangeGuojiaReq temp = new ChangeGuojiaReq();
                temp.guojiaId = SaveNum;
                temp.useType = 0;
                t_serializer.Serialize(tempStream, temp);

                byte[] t_protof = tempStream.ToArray();

                t_protof = tempStream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ChangeCountry_REQ, ref t_protof);
            }
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadNOZhuanGuoKa);
            }
        }
    
    }

    public void UIBoxLoadZhuangGuoSuccess(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_SUCCESS);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);

    }


    void ChangeCountryInfo(int index)
    {
        m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(JunZhuData.Instance().m_junzhuInfo.guoJiaId == index ? false : true);
        int size = m_listCountrySelect.Count;
        for (int i = 0; i < size; i++)
        {
            if (i == index - 1)
            {
                m_listCountrySelect[i].SelectedShow(true);
            }
            else
            {
                m_listCountrySelect[i].SelectedShow(false);
            }
        }

    }
    void RefreshCountryInfo()
    {
        m_SpriteCountry.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
    }
    void OnDestroy()
    {
        m_SettingUp = null;
        SocketTool.UnRegisterMessageProcessor(this);
    }

    int index_Reward = 0;


    void Create()
    {
        index_Reward = 0;
        int size = _listReward.Count;
        m_GrideCDReward.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(size - 1, 108), 0, 0);
        for (int i = 0; i < size; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
 
    }
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GrideCDReward != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_GrideCDReward.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            iconSampleManager.SetIconByID( _listReward[index_Reward].icon,_listReward[index_Reward].count.ToString(), 4);
            iconSampleManager.SetIconPopText(_listReward[index_Reward].icon,
                NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listReward[index_Reward].icon).nameId),
                DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listReward[index_Reward].icon).descId));

          
                iconSampleObject.transform.localScale = Vector3.one;
           

            if (index_Reward < _listReward.Count - 1)
            {
                index_Reward++;
            }

            m_GrideCDReward.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

}
