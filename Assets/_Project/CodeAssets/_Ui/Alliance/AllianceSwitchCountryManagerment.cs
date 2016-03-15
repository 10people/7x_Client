using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class AllianceSwitchCountryManagerment : MonoBehaviour, SocketProcessor
{

    public UILabel m_LabsignalUp;
    public UILabel m_LabsignalDown;
    public UILabel m_LabNoETopLeft;
    public UILabel m_LabNoEBottomLeft;
    public UILabel m_LabNoEBottomRight;
    public GameObject m_EnoughObj;
    public GameObject m_NoEnoughObj;
    public GameObject m_MainObj;
    public List<EventIndexHandle> m_listEvent;
    private int m_AllianceId = 0;
    private int _guojiaId = 0;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start ()
    {
        m_listEvent.ForEach(p => p.m_Handle += SwitchCountry);
    }

 
	public void ShowInfo(int c_id,int a_id)
    {
        m_AllianceId = a_id;
        _guojiaId = c_id;
        ShowSwitch(c_id);
    }
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
                    

                        if (tempResponse.result == 0)
                        {
                          ConnectServer();
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
                                    GameObject obj = new GameObject();
                                    obj.name = "MainCityUIButton_104";
                              
                                    Destroy(m_MainObj);
                                    return true;
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

                                    ShowSwitch(999);
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
    }

    void SwitchCountry(int index)
    {
        if (index == 0)
        {
            MainCityUI.TryRemoveFromObjectList(m_MainObj);
            Destroy(m_MainObj);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao > DangpuItemCommonTemplate.getDangpuItemCommonById(1003).needNum / DangpuItemCommonTemplate.getDangpuItemCommonById(1003).itemNum 
                || BagData.GetMaterialCountByID(910001) > 0)
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
                EquipSuoData.TopUpLayerTip(m_MainObj, true);
            }
        }
    }
    void ConnectServer()
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        CancelJoinAlliance allianceApplyCancel = new CancelJoinAlliance();
        allianceApplyCancel.id = m_AllianceId;
        t_qx.Serialize(t_tream, allianceApplyCancel);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.IMMEDIATELY_JOIN, ref t_protof);

    }
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
