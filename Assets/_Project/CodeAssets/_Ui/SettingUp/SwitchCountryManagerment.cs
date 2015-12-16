using UnityEngine;
using System.Collections;
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
    public List<SettingUpSelectCountryManagerment> m_listCountrySelect;
    public UISprite m_SpriteCountryCurrent;
 
    public GameObject m_ChangeCountryLayer;

    public GameObject m_Durable_UI;
    public UISprite m_SpriteCountry;
    private int SaveNum = 0;
    private Dictionary<long, GameObject> BloakedEleDic = new Dictionary<long, GameObject>();
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
        MainCityUI.m_MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        ChangeCountryInfo(JunZhuData.Instance().m_junzhuInfo.guoJiaId);
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

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_1) + AllianceData.Instance.g_UnionInfo.name + LanguageTemplate.GetText(LanguageTemplate.Text.SETTINGUP_CHANGE_COUNTRY_ALLIANCE_2);
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
