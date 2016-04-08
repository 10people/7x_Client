using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class SwitchCountryManagerment : MonoBehaviour, SocketProcessor
{
    public ScaleEffectController m_ScaleEffectController;
 
    public List<EventIndexHandle> m_listChangeCountryEvent;
 
    public UISprite m_SpriteCountryCurrent;
 
    public GameObject m_ChangeCountryLayer;

    public GameObject m_Durable_UI;
    public UISprite m_SpriteCountry;
    public UIGrid m_ItemParent;
    public GameObject m_ObjTopLeft;
    private List<GameObject> _listCountryObj  = new List<GameObject>();
    private int SaveNum = 0;
 
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        m_ScaleEffectController.OpenCompleteDelegate += ShowInfo;
        m_listChangeCountryEvent.ForEach(p => p.m_Handle += ChangeCountryReception);
    }

    void ShowInfo()
    {
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "转国", 0, 0);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        TidyData();
        m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        m_SpriteCountryCurrent.spriteName = "nation_" + JunZhuData.Instance().m_junzhuInfo.guoJiaId.ToString();
    }

    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + VipFuncOpenTemplate.GetNeedLevelByKey(4).ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null);

    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_ChangeCountry_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ChangeGuojiaResp tempResponse = new ChangeGuojiaResp();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());

                        if (tempResponse.result == 0)
                        {
                            if (SettingUpLayerManangerment.m_SettingUp)
                            {
                                FunctionWindowsCreateManagerment.m_isSwitchCountry = true;
                            }
                            _isSaveKey = false;
                            RefreshCountryInfo(SaveNum);
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadZhuangGuoSuccess);
                        }
                        else if (tempResponse.result == 101)
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadHaveLianMeng);
                        }
                        else if (tempResponse.result == 102)
                        {

                        }
                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }
    void ChangeCountryReception(int index)
    {
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
                break;
        }
    }
    public void UIBoxLoadHaveLianMeng(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_1) +"<"+ AllianceData.Instance.g_UnionInfo.name+ ">" + LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_2);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_3);
        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, confirmStr, null, null);
    }
 
    public void UIBoxLoadNOZhuanGuoKa(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

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

   
    void RefreshCountryInfo(int index)
    {
        if (JunZhuData.Instance().m_junzhuInfo.guoJiaId != index)
        {
            JunZhuData.Instance().m_junzhuInfo.guoJiaId = index;
        }
        TidyData();
        m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        m_SpriteCountryCurrent.spriteName = "nation_" + index;
        m_SpriteCountry.spriteName = "nation_" + index;
    }
    private List<int> _listCountryNum = new List<int>();
    void TidyData()
    {
        _listCountryNum.Clear();
        int[] country = { 1, 2, 3, 4, 5, 6, 7 };
        m_ItemParent.transform.localPosition = new Vector3(FunctionWindowsCreateManagerment.ParentPosOffset(6, 70),0,0);
        for (int i = 0; i < country.Length; i++)
        {
            if (country[i] != JunZhuData.Instance().m_junzhuInfo.guoJiaId)
            {
                _listCountryNum.Add(country[i]);
            }
        }
        int childCount = m_ItemParent.transform.childCount;
        int size = _listCountryNum.Count;
        if (childCount == 0)
        {
            for (int i = 0; i < size;i++)
            {
                CreateCountryItem();
            }
        }
        else
        {
            for (int i = 0;i < size; i++)
            {
                _listCountryObj[i].GetComponent<SettingUpSelectCountryManagerment>().m_BackKuang.SetActive(false);
                _listCountryObj[i].GetComponent<SettingUpSelectCountryManagerment>().RefreshCounty(_listCountryNum[i]);
          
            }
            m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        }
    }

    void CreateCountryItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SWITCH_COUNTRY_ITEM), UICountryItem_Load);
    }

    int index_num = 0;
    public void UICountryItem_Load(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.transform.parent = m_ItemParent.transform;
        tempObject.name = _listCountryNum[index_num].ToString();
        tempObject.transform.localPosition = Vector3.zero;
        tempObject.transform.localScale = Vector3.one;
        tempObject.GetComponent<SettingUpSelectCountryManagerment>().ShowCounty(_listCountryNum[index_num], GuangShow);
        _listCountryObj.Add(tempObject);
        if (index_num < _listCountryNum.Count - 1)
        {
            index_num++;
        }
 
        m_ItemParent.repositionNow = true;
    }

    private int _TouchNum = 0;
    private bool _isSaveKey = false;
    void GuangShow(int index)
    {
        SaveNum = index;
        //if (_TouchNum != index)
        {
            int size = _listCountryObj.Count;
            for (int i = 0; i < size; i++)
            {
                if (_listCountryObj[i].GetComponent<SettingUpSelectCountryManagerment>().m_CountryNum == index)
                {
                    _listCountryObj[i].GetComponent<SettingUpSelectCountryManagerment>().m_BackKuang.SetActive(true);
                }
                else
                {
                    _listCountryObj[i].GetComponent<SettingUpSelectCountryManagerment>().m_BackKuang.SetActive(false);
                }

            }
            _TouchNum = index;
            if (!_isSaveKey)
            {
                _isSaveKey = true;
                m_listChangeCountryEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(true);
            }
        }
    }

    void OnDisable()
    {
      _isSaveKey = false;
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
