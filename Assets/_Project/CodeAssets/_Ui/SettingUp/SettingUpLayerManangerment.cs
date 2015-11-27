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
    public ScaleEffectController m_ScaleEffectController;

    public GameObject m_DestroyTarget;
    public GameObject m_AllianceHave;
    public GameObject m_MainParent;
    public UISprite m_SpriteIcon;
    public UISprite m_SpriteCountry;
    public UISprite m_JunXian;

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

    public UILabel m_LabelTopUp;
    private string RenameInfo = "";
    private string CDkeyInfo = "";

    private string NameSave = "";
    public UIFont titleFont;//标题字体
    public UIFont btn1Font;//按钮1字体
    public UIFont btn2Font;//按钮2字体

    private Dictionary<long, GameObject> BloakedEleDic = new Dictionary<long, GameObject>();
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

 
	void Start ()
    {
        m_LabelTopUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_SIGNAL);
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
        UIInput input = listRenameObject[6].GetComponent<UILabel>().parent.GetComponent<UIInput>();
        RenameInfo = FunctionWindowsCreateManagerment.GetNeedString(input.value);
 
        listRenameObject[6].GetComponent<UILabel>().text = RenameInfo;
        if (string.IsNullOrEmpty(RenameInfo))
        {
            listRenameObject[9].SetActive(true);
        }
        else
        {
            listRenameObject[9].SetActive(false);
        }
    }
    void Update()
    {
        UIInput input = listRenameObject[6].GetComponent<UILabel>().parent.GetComponent<UIInput>();
        if (!string.IsNullOrEmpty(input.value))
        {
            OnInput();
        }
        CDkeyInfo = listCDKeyObject[3].GetComponent<UILabel>().text;
      CreateNameControl();
      CDKeyInfoControl();
    }
    bool createNameIsOn = false;
    bool createNameIsOff = false;
    void CreateNameControl()
    {
       int size = listRenameObject[6].transform.childCount;
       if (size > 0)
       {
           if (!createNameIsOff)
           {
               createNameIsOff = true;
               listRenameObject[9].SetActive(false);
           }

       }
       else
       {
           if (createNameIsOff && string.IsNullOrEmpty(RenameInfo))
           {
               createNameIsOff = false;
               listRenameObject[9].SetActive(true);
           }
       }
    
       if (!string.IsNullOrEmpty(RenameInfo))
       {
       
           if (createNameIsOn)
           {
               createNameIsOn = false;
               listRenameObject[9].SetActive(false);
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
            if (CDKeyIsOn)
            {
                CDKeyIsOn = false;
                listCDKeyObject[4].SetActive(false);
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
    //void ButtonsControl(GameObject obj, bool colliderEnable)
    //{
    //    if (obj.transform.FindChild("Background").GetComponent<TweenColor>() == null)
    //    {
    //        obj.transform.FindChild("Background").gameObject.AddComponent<TweenColor>();
    //        obj.transform.FindChild("Background").gameObject.AddComponent<TweenColor>().enabled = false;
    //    }
    //    if (colliderEnable)
    //    {
    //        obj.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
    //        obj.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
    //    }
    //    else
    //    {
    //        obj.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
    //        obj.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
    //    }

    //    obj.transform.FindChild("Background").GetComponent<TweenColor>().duration = 0.2f;
    //    obj.transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
    //    obj.collider.enabled = colliderEnable;
    //    SendObj = obj;
    //    EventDelegate.Add(obj.transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);

    //}
    //void TweenColorDestroy()
    //{
    //    if (SendObj.transform.FindChild("Background").GetComponent<TweenColor>() != null)
    //    {
    //        EventDelegate.Remove(SendObj.transform.FindChild("Background").GetComponent<TweenColor>().onFinished, TweenColorDestroy);
    //        Destroy(SendObj.transform.FindChild("Background").GetComponent<TweenColor>());
    //    }
    //}
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

        Debug.Log("FunctionWindowsCreateManagerment.m_SettingUpTYpe ::" + FunctionWindowsCreateManagerment.m_SettingUpTYpe);
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
                  //  listBlockedChatObject[0].SetActive(true);
                m_ChangeCountryLayer.SetActive(true);
                   //m_SpriteCountryHuang.SetActive(true);
                   //m_SpriteCountryHuangBack.SetActive(true);
                   ChangeCountryInfo(JunZhuData.Instance().m_junzhuInfo.guoJiaId);
                    m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                    m_SpriteCountryCurrent.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
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

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + VipFuncOpenTemplate.GetNeedLevelByKey(4).ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, titleFont, btn1Font);

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
                        listRenameObject[1].SetActive(false);
                        listRenameEvent[1].GetComponent<Collider>().enabled = true;
                        if (tempInfo.code == 0)
                        {
                            listMainLab[1].text = "Lv" + JunZhuData.Instance().m_junzhuInfo.level.ToString() + "   " + tempInfo.name;
                            JunZhuData.Instance().m_junzhuInfo.name = NameSave;
                            listRenameObject[7].GetComponent<UILabel>().text = NameSave;
                            listRenameObject[4].SetActive(true);
                        }
                        else if (tempInfo.code == -200)
                        {
                            listRenameObject[0].SetActive(false);
                            listRenameObject[3].SetActive(true);
                        }
                        else if (tempInfo.code == -300)
                        {
                            listRenameObject[0].SetActive(false);
                            listRenameObject[2].SetActive(true);
                        }
                        else if (tempInfo.code > 0)
                        {
                            listRenameObject[5].SetActive(true);
                        }

                        return true;
                    }
                //case ProtoIndexes.S_CANCEL_BLACK:
                //    {
                //        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        CancelBlackResp tempResponse = new CancelBlackResp();

                //        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
             
                //        if (tempResponse.result == 0)
                //        {
                //            BlockedData.Instance().m_BlockedInfoDic.Remove(tempResponse.junzhuId);
                //            listBlockedChatObject[0].SetActive(true);
                //            listBlockedChatObject[1].SetActive(false);
                //            Destroy(BloakedEleDic[tempResponse.junzhuId]);
                //            BloakedEleDic.Remove(tempResponse.junzhuId);
                //            listBlockedChatObject[2].GetComponent<UIGrid>().Reposition();
                //        }
                      
                //        return true;
                //    }


                case ProtoIndexes.S_ChangeCountry_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ChangeGuojiaResp tempResponse = new ChangeGuojiaResp();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());
 
                        if (tempResponse.result == 0)
                        {
                            RefreshCountryInfo(SaveNum);
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadZhuangGuoSuccess);
                            
                        }
                        else if (tempResponse.result == 101)
                        {
                          Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadHaveLianMeng);
                        }
                        else if (tempResponse.result == 102)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadNOZhuanGuoKa);
                        }
                        return true;
                    }
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
                    if (SysparaTemplate.CompareSyeParaWord(RenameInfo))
                    {
                        listRenameObject[0].SetActive(false);
                        listRenameObject[8].SetActive(true);
                    }
                    else
                    {
                        listRenameObject[0].SetActive(false);
                        listRenameObject[1].SetActive(true);
                    }   
                }
                break;
            case 1:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= 100)
                    {
                        listRenameEvent[1].GetComponent<Collider>().enabled = false;
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
                        listRenameObject[1].SetActive(false);
                        listRenameObject[5].SetActive(true);
                    }
                }
                break;
            case 2:
                {
                    MainCityUI.TryRemoveFromObjectList(m_MainParent);
                    TopUpLoadManagerment.m_instance.LoadPrefab(false);
                 
                    Destroy(m_MainParent);
                }
                break;
            case 3:
                {
                    listRenameObject[3].SetActive(false);
                    listRenameObject[0].SetActive(true);
                }
                break;
            case 4:
                {
                    listRenameObject[2].SetActive(false);
                    listRenameObject[0].SetActive(true);
                }
                break;
            case 5:
                {
                    listRenameObject[1].SetActive(false);
                    listRenameObject[0].SetActive(true);
                }
                break;
            case 6:
                {
                    listRenameObject[8].SetActive(false);
                    listRenameObject[0].SetActive(true);
                }
                break;
            case 12:
                {
                    OnInput();
                    listRenameEvent[7].gameObject.SetActive(false);
                }
                break;
            case 13:
                {
                    listRenameObject[9].SetActive(false);
                    listRenameEvent[7].gameObject.SetActive(true);
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
                    listCDKeyObject[0].SetActive(false);
                    listCDKeyObject[1].SetActive(true);
                }
                break;
            case 1:
                {
                    listCDKeyObject[0].SetActive(true);
                    listCDKeyObject[1].SetActive(false);
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
    void CDKeyController()
    { 
    
    }

    void SwitchAccountController(int index)
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
    void ChangeCountryReception(int index)
    {
        switch (index)
        {
            case 0:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId  > 0)
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
    public void UIBoxLoadHaveLianMeng(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_1) + AllianceData.Instance.g_UnionInfo.name +  LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_2);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_3);
        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null, titleFont, btn1Font);

    }


    public void UIBoxLoadNOZhuanGuoKa(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr =LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = "\n\n" + LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_CARD_1);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, titleFont, btn1Font);

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
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, ZhuanGuo, titleFont, btn1Font);

    }

    void ZhuanGuo(int index)
    {
        if (index == 2)
        {

            MemoryStream tempStream = new MemoryStream();
            QiXiongSerializer t_serializer = new QiXiongSerializer();
            ChangeGuojiaReq temp = new ChangeGuojiaReq();
            temp.guojiaId = SaveNum;
            t_serializer.Serialize(tempStream, temp);

            byte[] t_protof = tempStream.ToArray();

            t_protof = tempStream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ChangeCountry_REQ, ref t_protof);
        }
    
    }

    public void UIBoxLoadZhuangGuoSuccess(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_SUCCESS);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, titleFont, btn1Font);

    }


    void ChangeCountryInfo(int index)
    {
        m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(JunZhuData.Instance().m_junzhuInfo.guoJiaId == index ? false : true);
        int size  = m_listCountrySelect.Count;
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
    void RefreshCountryInfo(int index)
    {
        ChangeCountryInfo(index);
        m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        m_SpriteCountryCurrent.spriteName = "nation_" + index;
        m_SpriteCountry.spriteName = "nation_" + index;
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
