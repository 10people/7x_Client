using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

using ProtoBuf;
using ProtoBuf.Meta;
public class EquipGrowthWashManagerment : MonoBehaviour, SocketProcessor, UI2DEventListener
{
    public UILabel m_TotalGold;
 
    public GameObject m_EquipInfo;
    public GameObject m_SuoTagSignal;
 
    public GameObject m_HidenGameobject;
    public GameObject m_WashTanHao;
    public GameObject m_WashSeniorTanHao;
    public GameObject m_MainParent;
    public UIGrid m_GridParent;
    public UILabel m_LabPPPP;
    public UIGrid m_GridTop;
    public UIGrid m_GridTop2;
    public GameObject m_XLYuanBao;
    public GameObject m_XLStone;
    public UILabel m_WashConsume;
    public UILabel m_WashConsumeSignal;
    public GameObject m_WashSuccesse;
    public GameObject m_UnWashSuccesse;
    public List<EventIndexHandle> listEvent;
    public EventIndexHandle m_MaskEvent;
    public EventIndexHandle m_MaskTouch;
    XiLianRes EquipWashInfo = new XiLianRes();
    public EventPressIndexHandle m_EventPress;
    public EventPressIndexHandle m_EventPress_JinJie;
    public GameObject m_ObjNewAttribute;
    public UILabel m_labNewAttSignal;
    private int _WashType = 0;
    private long dbIdSave;
    public int buttonNum = 0;
    public int m_Sendbuwei = 0;
    private int savedId;
    private int _StoneWashTImes = 0;
    private bool _isFull = false;
    private List<int> listConnect = new List<int>();
    private List<int> listSuoAdded = new List<int>();
    //private bool washYuanBao = false;
    private int pinzhiSaved = 0;
    private int YBXiLianLimited = 0;
    private int _yuanbaoConSume = 0;
    private int _BuWeiSave = 0;
    public int m_EquipType = 0;
    public UIGrid m_GridAdd;
    public GameObject m_ObjNormal;
 
    public UILabel m_LabJinJie;
    public struct RangeInfo
    {
        public int _nameid;
        public int _min;
        public int _max;
    };
    private List<EquipSuoData.ShuXingInfo> _listRangeInfo = new List<EquipSuoData.ShuXingInfo>();
    public struct AddInfo
    {
        public int _nameid;
        public int _add;
  
    };
    private List<EquipSuoData.ShuXingInfo> _listAddInfo = new List<EquipSuoData.ShuXingInfo>();
    private int _WadhStoneCount = 0;
    void Start()
    {
        m_MaskTouch.m_Handle += TouchEvent;
        listEvent.ForEach(p => p.m_Handle += TouchEvent);
        m_EventPress.m_Handle += PressEvent;
        m_EventPress_JinJie.m_Handle += PressEvent_JinJie;
        m_MaskEvent.m_Handle += MaskEvent;
        if (FreshGuide.Instance().IsActive(100330) && TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100330;

            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
    }

    void MaskEvent(int index)
    {
        ClientMain.m_UITextManager.createText(LanguageTemplate.GetText(LanguageTemplate.Text.WASH_TAG));
    }
    public void OnUI2DShow()
    {
        _WadhStoneCount = BagData.Instance().GetCountByItemId(910002);
        FreshLab();
    }
    void OnEnable()
    {
        _WadhStoneCount = BagData.Instance().GetCountByItemId(910002);
        SocketTool.RegisterMessageProcessor(this);
    }

    void PressEvent_JinJie(int index)
    {
        m_EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().m_objSharePart.GetComponent<EquipGrowthWearManagerment>().ShowTypes(3);
    }
    void PressEvent(int index)
    {
		//if (DeviceHelper.IsSingleTouching() )
        {
            if (index != 99)
            {
                buttonNum = index;
            }
            //if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                CityGlobalData.m_isRightGuide = false;
                if (FreshGuide.Instance().IsActive(100330) && TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
                {
                    UIYindao.m_UIYindao.CloseUI();
                }
            }
            if (index == 1 || index == 2)
            {
                _WashType = index;

                if (!FunctionOpenTemp.GetWhetherContainID(1210))
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                              UIBoxLoadCallbackWash);
                    buttonNum = 0;
                }
                else
                {
                    if (FreshGuide.Instance().IsActive(100705) && TaskData.Instance.m_TaskInfoDic[100705].progress >= 0)
                    {

                        UIYindao.m_UIYindao.CloseUI();
                    }
                    else
                    {
                        UIYindao.m_UIYindao.CloseUI();
                    }

                    CreateMove(m_EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().m_EquipItenm.m_LabelSuccess.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.XILIAN_DESC_11));
                    if (index == 2 
                        && JunZhuData.Instance().m_junzhuInfo.yuanBao < EquipWashInfo.yuanBao 
                        && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES))
                        || _WadhStoneCount == 0))
                    {
                        Global.CreateFunctionIcon(101);
                       
                        buttonNum = 0;
                    }
                    else
                    {
                        if (YBXiLianLimited == 0 
                            && index == 2 
                            && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxLoadCallback);
                            buttonNum = 0;
                        }
                        else
                        {
                            m_EventPress.GetComponent<Collider>().enabled = false;
                            
                            EquipWashRequest(index);
                        }
                    }
                }
            }
        }
    }

    void TouchEvent(int index)//按钮事件控制
    {
        if (index != 99)
        {
            buttonNum = index;
        }

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(400120) && TaskData.Instance.m_TaskInfoDic[400120].progress >= 0)
            {
                CityGlobalData.m_isRightGuide = true;
            }
            else if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
            {
                
            }
        }

		//if (DeviceHelper.IsSingleTouching() )
        {
            if (index < 10)
            {
                if (index == 4 || index == 3)
                {
                    //listEvent[0].transform.GetComponent<Collider>().enabled = false;
                    //listEvent[1].transform.GetComponent<Collider>().enabled = false;
                    if (UIYindao.m_UIYindao.m_isOpenYindao)
                    {
                        if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress < 0)
                        {
                            UIYindao.m_UIYindao.CloseUI();
                        }
                    }
                    EquipWash(dbIdSave, savedId, index, 3, 3, 3, 3, 3, 3, 3, 3, pinzhiSaved);
                }
            }
            else
            {
                if (index == 11)
                {
                    CityGlobalData.m_isWashMaxSignal = true;
                    m_MaskTouch.gameObject.SetActive(false);
               
 

                }
                else if (index == 12)
                {
                    m_MaskTouch.gameObject.SetActive(false);
                    CityGlobalData.m_isWashMaxSignalConfirm = true;
                }
                else if (index == 10)
                {
                    EquipSuoData.TopUpLayerTip(m_MainParent,true);
                }
                else if (index == 99)
                {
                    CreateMove(m_LabPPPP.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.WASH_TAG));
                }
            }
        }

    } 
    public void EquipWash(long dbid, int equipid, int type, int wqSHLock, int wqJMLock, int wqBJLock, int wqRXLock, int jnSHLock, int jnJMLock, int jnBJLock, int jnRXLock,int pinzhi)
    {
        BagData.Instance().GetCountByItemId(910002).ToString();
        pinzhiSaved = pinzhi;
        dbIdSave = dbid;
        savedId = equipid;
       // EquipsOfBody.Instance().m_equipsOfBodyDic
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        XiLianReq equip = new XiLianReq();
        equip.equipId = dbid;
        equip.action = type;

        equip.lockTongli = 3;
        equip.lockMouli = 3;
        equip.lockWuli = 3;

        equip.wqSHLock = wqSHLock;
        equip.wqJMLock = wqJMLock;
        equip.wqBJLock = wqBJLock;
        equip.wqRXLock = wqRXLock;

        equip.jnSHLock = jnSHLock;
        equip.jnJMLock= jnJMLock;
        equip.jnBJLock = jnBJLock;
        equip.jnRXLock = jnRXLock;

        equip.type = 2;
        t_qx.Serialize(t_tream, equip);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_XiLian, ref t_protof,false,p_receiving_wait_proto_index: ProtoIndexes.S_EQUIP_XiLian);
    }
    private XiLianRes _WashInfoSave;
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_EQUIP_XiLian:// 需要洗练装备信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        XiLianRes WashInfo = new XiLianRes();
                        t_qx.Deserialize(t_tream, WashInfo, WashInfo.GetType());
                        YBXiLianLimited = WashInfo.yuanBaoTimes;
                        m_HidenGameobject.SetActive(true);
                        _StoneWashTImes = WashInfo.xilianshiTimes;
                        EquipWashInfo = WashInfo;

                        if (_WadhStoneCount == 0 || WashInfo.isAllXiMan)
                        {
                            PushAndNotificationHelper.SetRedSpotNotification(1210, false);
                        }
                        //else if(!AllQualityLowest() && _WadhStoneCount > 0 && !PushAndNotificationHelper.IsShowRedSpotNotification(1210))
                        //{
                        //    Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                        //    PushAndNotificationHelper.SetRedSpotNotification(1210, true);
                        //}
                        if (buttonNum == 0)
                        {
                            ShowRangeInfo(WashInfo);
                         
                            buttonNum = 0;
                        }
                        else if (buttonNum == 1 || buttonNum == 2)
                        {
                            if (buttonNum == 2 && (_WadhStoneCount == 0 || _StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES))))
                            {
                                if (_yuanbaoConSume == 0)
                                {
                                    _yuanbaoConSume = WashInfo.yuanBao;
                                }
                                else
                                {
                                    _yuanbaoConSume = _WashInfoSave.yuanBao;
                                }
                            }
                            else
                            {
                                _yuanbaoConSume = 0;
                            }

                            if (buttonNum == 2 && _WadhStoneCount > 0 && _StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)))
                            {
                                _WadhStoneCount--;
                            }
                        
                            AppendAttributeTidy(EquipWashInfo);
                            buttonNum = 0;
                        }
                        else if (buttonNum == 3 || buttonNum == 4)
                        {
                            ClientMain.m_UITextManager.createText("洗练成功");

                            FunctionWindowsCreateManagerment.FreshWashEquipInfo(WashInfo);
                            SaveOrCancel(buttonNum);
                            m_MaskTouch.gameObject.SetActive(false);
                            m_WashSuccesse.SetActive(false);
                            buttonNum = 0;
                        }
                        ShowWashInfo();
                        _WashInfoSave = WashInfo;
                        return true;
                    }
                case ProtoIndexes.S_EQUIP_XILIAN_ERROR:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        XilianError WashErr = new XilianError();
                        t_qx.Deserialize(t_tream, WashErr, WashErr.GetType());
                        FreshLab();
                        switch (WashErr.result)
                        {
                            case 0:
                                {
                                
                                }
                                break;
                            case 1:
                                {
                                
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

    public void UIBoxLoadCallbackWash(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str1 = NameIdTemplate.GetName_By_NameId(990043) + ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1210)) + NameIdTemplate.GetName_By_NameId(990044) + "!"; ;

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, null, null);
    }

    void ShowRangeInfo(XiLianRes xlres)
    {
        _listRangeInfo.Clear();
        _listAtt.Clear();
        float[] allNow = { float.Parse(xlres.wqSH.ToString()), float.Parse(xlres.wqJM.ToString()), float.Parse(xlres.wqBJ.ToString())
                , float.Parse(xlres.wqRX.ToString()), float.Parse(xlres.jnSH.ToString()), float.Parse(xlres.jnJM.ToString())
                , float.Parse(xlres.jnBJ.ToString()), float.Parse(xlres.jnRX.ToString())
                ,xlres.wqBJL, xlres.jnBJL, xlres.wqMBL, xlres.jnMBL };

        int[] allMax = { xlres.wqSHMax, xlres.wqJMMax, xlres.wqBJMax, xlres.wqRXMax, xlres.jnSHMax, xlres.jnJMMax, xlres.jnBJMax, xlres.jnRXMax };

        int attri_size = EquipSuoData.AllShuXin(XiLianShuXingTemp.GetTemplateById(savedId)).Count;

        for (int i = 0; i < attri_size; i++)
        {
            _listAtt.Add(EquipSuoData.AllShuXin(XiLianShuXingTemp.GetTemplateById(savedId))[i]);
        }

        for (int j = 0; j < _listAtt.Count; j++)
        {
            for (int i = 0; i < _listAtt.Count - 1 - j; i++)
            {
                if (_listAtt[i] > _listAtt[i + 1])
                {
                    int t = _listAtt[i];
                    _listAtt[i] = _listAtt[i + 1];
                    _listAtt[i + 1] = t;
                }
            }
        }

        if (WetherAllMax(xlres))
        {
            _isFull = true;
        }
        else
        {
            _isFull = false;
        }


        for (int i = 0; i < _listAtt.Count; i++)
        {
            if (allNow[_listAtt[i]] > 0)
            {
                EquipSuoData.ShuXingInfo add = new EquipSuoData.ShuXingInfo();
                add._nameid = EquipSuoData.GetNameIDByIndex(_listAtt[i]);
                if (_listAtt[i] > 3)
                {
                    add._type = 1;
                }
                else
                {
                    add._type = 0;
                }
                add._Max = allMax[_listAtt[i]];
                add._Count = allNow[_listAtt[i]];
                add._Max2 = 0;
                add._Count2 = 0;
                add._NeedUpgrade = allMax[_listAtt[i]] <= allNow[_listAtt[i]]; ;
                add._NoHave = false;
                add._IsAdd = false;
                add._IsAllMax = WetherAllMax(xlres);
                add._IsMaxQualliaty = ZhuangBei.getZhuangBeiById(savedId).jiejieId == 0;
                _listRangeInfo.Add(add);
            }
            else
            {
                EquipSuoData.ShuXingInfo add = new EquipSuoData.ShuXingInfo();
                add._nameid = EquipSuoData.GetNameIDByIndex(_listAtt[i]);
                if (_listAtt[i] > 3)
                {
                    add._type = 1;
                }
                else
                {
                    add._type = 0;
                }
                add._Max = 0;
                add._Count = 0;
                add._Max2 = 0;
                add._Count2 = 0;

                if (_listAtt[i] < allMax.Length)
                {
                    add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]];
                }
                add._NoHave = true;
                add._IsAdd = false;
                add._IsAllMax = WetherAllMax(xlres);
                add._IsMaxQualliaty = ZhuangBei.getZhuangBeiById(savedId).jiejieId == 0;
                _listRangeInfo.Add(add);
            }
        }

        int size = m_GridTop.transform.childCount;
        int sizeR = _listRangeInfo.Count;
        if (size > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                m_GridTop.transform.GetChild(i).GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listRangeInfo[i]);
            }
        }
        else
        {
            index_ShuXing = 0;
            for (int i = 0; i < _listRangeInfo.Count; i++)
            {
                if (i < 4)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM2), ResourcesLoadCallBack2);
                }
                
            }
        }
    }

    private bool WetherAllMax(XiLianRes xlres)
    {
        float[] allNow = { float.Parse(xlres.wqSH.ToString()), float.Parse(xlres.wqJM.ToString()), float.Parse(xlres.wqBJ.ToString())
                , float.Parse(xlres.wqRX.ToString()), float.Parse(xlres.jnSH.ToString()), float.Parse(xlres.jnJM.ToString())
                , float.Parse(xlres.jnBJ.ToString()), float.Parse(xlres.jnRX.ToString())
                ,xlres.wqBJL, xlres.jnBJL, xlres.wqMBL, xlres.jnMBL};

        int[] allMax = { xlres.wqSHMax, xlres.wqJMMax, xlres.wqBJMax, xlres.wqRXMax, xlres.jnSHMax, xlres.jnJMMax, xlres.jnBJMax, xlres.jnRXMax };

        for (int i = 0; i < _listAtt.Count; i++)
        {
            if (allMax[_listAtt[i]] > allNow[_listAtt[i]])
            {
                return false;
            }
        }
        return true;
    }

    void FreshLab()
    {
        m_MaskTouch.gameObject.SetActive(false);
        m_EventPress.GetComponent<Collider>().enabled = true;


        if (_StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WadhStoneCount > 0)
        {
            m_XLYuanBao.SetActive(false);
            m_XLStone.SetActive(true);
            //if (int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) >= _WadhStoneCount)
            {
                m_WashConsume.text = "1";
            }
            //else
            //{
            //    m_WashConsume.text = "1/" + CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES);
            //}
            m_WashConsumeSignal.text = "剩余洗练石：" + _WadhStoneCount + "个";
        }
        else
        {
            m_XLYuanBao.SetActive(true);
            m_XLStone.SetActive(false);
            m_WashConsumeSignal.text = LanguageTemplate.GetText(LanguageTemplate.Text.XILIAN_DESC_2) +YBXiLianLimited.ToString();
 
            //if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 10000)
            //{
            //    m_TotalGold.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000).ToString() + NameIdTemplate.GetName_By_NameId(990051);
            //}
            //else
            //{
            //    m_TotalGold.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
       
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao < EquipWashInfo.yuanBao)
            {
                m_WashConsume.text = MyColorData.getColorString(5, EquipWashInfo.yuanBao.ToString());
            }
            else
            {
                m_WashConsume.text = MyColorData.getColorString(1, EquipWashInfo.yuanBao.ToString());
            }
           
        }

        //if (FunctionOpenTemp.GetWhetherContainID(1210) 
        //    &&(_StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES))
        //    && _WadhStoneCount > 0)
        //      ) 
        //{
        //    m_WashTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(1210) && FunctionOpenTemp.GetWhetherContainID(1210));
        //   //
        //}
        //else
        //{
        //    m_WashTanHao.SetActive(false);
        //}
        m_WashSeniorTanHao.SetActive((_StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WadhStoneCount > 0));
 
        WashButtonsShowState();
    }

    void ShowWashInfo()
    {
        FreshLab();
       
        if (ShowWetherWash())
        {
            m_LabJinJie.gameObject.SetActive(true);
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
        }
        else
        {
            m_LabJinJie.gameObject.SetActive(false);
            m_WashSuccesse.SetActive(true);
            m_UnWashSuccesse.SetActive(false);
            SuccessedAttribute();
        }
        WashButtonsShowState();
    }

    private bool ShowWetherWash()
    {
        if (EquipWashInfo.wqSHAdd == 0 
            && EquipWashInfo.wqJMAdd == 0 
            && EquipWashInfo.wqBJAdd == 0 
            && EquipWashInfo.wqRXAdd == 0 
            && EquipWashInfo.jnSHAdd == 0 
            && EquipWashInfo.jnJMAdd == 0
            && EquipWashInfo.jnBJAdd == 0 
            && EquipWashInfo.jnRXAdd == 0)
        {
            return true;
        }
        return false;
    }
    void ShowWashSuccessInfo()
    {
        FreshLab();
       // m_WashSuccesse.SetActive(true);
        if (buttonNum == 3 || buttonNum == 4)
        {
            m_WashSuccesse.SetActive(false);
        }
        else if (buttonNum == 1 || buttonNum == 2)
        {
            //m_ButtonBack.SetActive(false);
        }
        WashButtonsShowState();
       // CurrentAttributeTidy();
        SuccessedAttribute();
    }
    private List<int> _listAtt = new List<int>();
    void SuccessedAttribute()//显示新生成属性
    {
        _listAddInfo.Clear();
        _listAtt.Clear();
        int attri_size = EquipSuoData.AllShuXin(XiLianShuXingTemp.GetTemplateById(savedId)).Count;
        for (int i = 0; i < attri_size; i++)
        {
            _listAtt.Add(EquipSuoData.AllShuXin(XiLianShuXingTemp.GetTemplateById(savedId))[i]);
        }
        for (int j = 0; j < _listAtt.Count; j++)
        {
            for (int i = 0; i < _listAtt.Count - 1 - j; i++)
            {
                if (_listAtt[i] > _listAtt[i + 1])
                {
                    int t = _listAtt[i];
                    _listAtt[i] = _listAtt[i + 1];
                    _listAtt[i + 1] = t;
                }
            }
        }
        float[] allNow = { float.Parse(EquipWashInfo.wqSH.ToString()), float.Parse(EquipWashInfo.wqJM.ToString()),
            float.Parse(EquipWashInfo.wqBJ.ToString())
                , float.Parse(EquipWashInfo.wqRX.ToString()), float.Parse(EquipWashInfo.jnSH.ToString()),
            float.Parse(EquipWashInfo.jnJM.ToString())
                , float.Parse(EquipWashInfo.jnBJ.ToString()), float.Parse(EquipWashInfo.jnRX.ToString())
                ,EquipWashInfo.wqBJL, EquipWashInfo.jnBJL, EquipWashInfo.wqMBL, EquipWashInfo.jnMBL};
        int[] attribute = { EquipWashInfo.wqSHAdd, EquipWashInfo.wqJMAdd, EquipWashInfo.wqBJAdd,
            EquipWashInfo.wqRXAdd, EquipWashInfo.jnSHAdd, EquipWashInfo.jnJMAdd, EquipWashInfo.jnBJAdd, EquipWashInfo.jnRXAdd };

        int[] allMax = { EquipWashInfo.wqSHMax, EquipWashInfo.wqJMMax, EquipWashInfo.wqBJMax
                , EquipWashInfo.wqRXMax, EquipWashInfo.jnSHMax, EquipWashInfo.jnJMMax, EquipWashInfo.jnBJMax, EquipWashInfo.jnRXMax };
        for (int i = 0; i < _listAtt.Count; i++)
        {

            if (allNow[_listAtt[i]] > 0)
            {
                EquipSuoData.ShuXingInfo add = new EquipSuoData.ShuXingInfo();
                add._nameid = EquipSuoData.GetNameIDByIndex(_listAtt[i]);
                if (_listAtt[i] > 3)
                {
                    add._type = 1;
                }
                else
                {
                    add._type = 0;
                }
                add._Max = allMax[_listAtt[i]];
                if (_listAtt[i] < attribute.Length && Mathf.Abs(attribute[_listAtt[i]]) > 0)
                {
                    if (attribute[_listAtt[i]] < 0)
                    {
                        add._IsAdd = false;
                    }
                    else
                    {
                        add._IsAdd = true;
                    }
                     
                    add._Max2 = allMax[_listAtt[i]];
                    if (attribute[_listAtt[i]] > 0)
                    {
                        add._Count2 = allNow[_listAtt[i]] + attribute[_listAtt[i]];
                    }
                    else
                    {
                        add._Count2 = allNow[_listAtt[i]];
                    }
                    add._Count = allNow[_listAtt[i]];
                }
                //else if(attribute[_listAtt[i]] < 0)
                //{
                //    add._IsAdd = false;
                //    add._Max2 = allMax[_listAtt[i]];
                //    add._Count2 = allNow[_listAtt[i]];
                //    add._Count = allNow[_listAtt[i]] + attribute[_listAtt[i]];
                //}
                else
                {
                    add._Max2 = 0;
                    add._Count = allNow[_listAtt[i]];
                }
                add._CountAdd = attribute[_listAtt[i]];
                add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]]; ;
                add._NoHave = false;
                add._IsMaxQualliaty = ZhuangBei.getZhuangBeiById(savedId).jiejieId == 0;
                _listAddInfo.Add(add);
            }
            else
            {
                EquipSuoData.ShuXingInfo add = new EquipSuoData.ShuXingInfo();
                add._nameid = EquipSuoData.GetNameIDByIndex(_listAtt[i]);
                if (_listAtt[i] > 3)
                {
                    add._type = 1;
                }
                else
                {
                    add._type = 0;
                }
                add._Max = 0;
                add._Count = 0;
                add._Max2 = 0;
                add._Count2 = 0;
                if (_listAtt[i] < allMax.Length)
                {
                    add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]];
                }
                add._NoHave = true;
                add._IsAdd = false;
                add._IsMaxQualliaty = ZhuangBei.getZhuangBeiById(savedId).jiejieId == 0;
                _listAddInfo.Add(add);
            }
        }
        int size = m_GridTop2.transform.childCount;
        int size_add = _listAddInfo.Count;
        if (size > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                m_GridTop2.transform.GetChild(i).GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listAddInfo[i]);
            }
 
        }
        else
        {
            index_add = 0;
            for (int i = 0; i < _listAddInfo.Count; i++)
            {
                if (i < 4)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM2), ResourcesLoadAddCallBack);
                }
               
            }
        }
    }
 
    private List<EquipSuoData.WashInfo> _listEquipWash;
    private void AppendAttributeTidy(XiLianRes xlres)
    {
        EquipSuoData.m_listNewAttribute.Clear();

        _listEquipWash = new List<EquipSuoData.WashInfo>();
        float[] allNow = { float.Parse(xlres.wqSH.ToString()), float.Parse(xlres.wqJM.ToString()), float.Parse(xlres.wqBJ.ToString())
                , float.Parse(xlres.wqRX.ToString()), float.Parse(xlres.jnSH.ToString()), float.Parse(xlres.jnJM.ToString())
                , float.Parse(xlres.jnBJ.ToString()), float.Parse(xlres.jnRX.ToString())
                ,xlres.wqBJL, xlres.jnBJL, xlres.wqMBL, xlres.jnMBL, xlres.jnMBL };

        int[] allAdd = { xlres.wqSHAdd, xlres.wqJMAdd, xlres.wqBJAdd, xlres.wqRXAdd, xlres.jnSHAdd, xlres.jnJMAdd, xlres.jnBJAdd, xlres.jnRXAdd };
 
        int[] allMax = { xlres.wqSHMax, xlres.wqJMMax, xlres.wqBJMax, xlres.wqRXMax, xlres.jnSHMax, xlres.jnJMMax, xlres.jnBJMax, xlres.jnRXMax };
        int size = allNow.Length;
        for (int i = 0; i < size; i++)
        {
            if (allNow[i] > 0)
            {
                int num = -1;
                for (int j = 0; j < EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID].Count; j++)
                {
                    if (i == EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID][j]._num)
                    {
                        num = EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID][j]._num;
                        break;
                    }
                }

                if (num < 0)
                {
                    EquipSuoData.NewAttribute na = new EquipSuoData.NewAttribute();
                    na._num = i;
                    na._isnew = true;
                    EquipSuoData.m_listNewAttribute.Add(i, na);
                }
            }
        }

        if (EquipSuoData.m_listEquipWash.ContainsKey(EquipWashInfo.zhuangbeiID))
        {
            EquipSuoData.m_listEquipWash.Remove(EquipWashInfo.zhuangbeiID);
        }

        for (int i = 0; i < allNow.Length; i++)
        {
            if (allNow[i] > 0)
            {
                EquipSuoData.WashInfo wss = new EquipSuoData.WashInfo();
                if (i > 3)
                {
                    wss._type = 1;
                }
                else
                {
                    wss._type = 0;
                }
                wss._num = i;
                wss._nameid = EquipSuoData.GetNameIDByIndex(i);
                if (i < allAdd.Length)
                {
                    wss._add = allAdd[i];
                }

                if (i < allMax.Length)
                {
                    wss._isMax = allNow[i] == allMax[i] ? true : false;
                }
                wss._count = allNow[i];

                if (EquipSuoData.m_listNewAttribute.ContainsKey(i))
                {
                    wss._isnew = EquipSuoData.m_listNewAttribute[i]._isnew;
                }
                _listEquipWash.Add(wss);
            }
        }
        EquipSuoData.m_listEquipWash.Add(EquipWashInfo.zhuangbeiID, _listEquipWash);
        m_ObjNewAttribute.SetActive(EquipSuoData.GetWetherContainNewAttribute(EquipWashInfo.zhuangbeiID));
      //  m_WashFreeTimesCount.text = timesCurrent.ToString() + "/" + VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit;
        if (EquipSuoData.GetWetherContainNewAttribute(EquipWashInfo.zhuangbeiID))
        {
            FreshLab();
            ShowRangeInfo(xlres);
            m_MaskTouch.gameObject.SetActive(false); 
         
            UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_ObjNewAttribute, EffectIdTemplate.GetPathByeffectId(100180), null);
           
            ShowRangeInfo(EquipWashInfo);
            StartCoroutine(WaitSecond());
        }
        else
        {
            m_UnWashSuccesse.SetActive(false);
            ShowWashSuccessInfo();
        }
    }
    IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(1.2f);
        m_ObjNewAttribute.SetActive(false);
        UI3DEffectTool.ClearUIFx(m_ObjNewAttribute);
        if (_StoneWashTImes == int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WashType == 2)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UICallback);
        }

    }
    void SaveOrCancel(int index)//3 替换和 4 还原界面刷新
    {
        if (_StoneWashTImes == int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WashType == 2)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                  UICallback);
        }
        if (index == 3)
        {
            int[] allNow = { EquipWashInfo.wqSH, EquipWashInfo.wqJM, EquipWashInfo.wqBJ, EquipWashInfo.wqRX, EquipWashInfo.jnSH, EquipWashInfo.jnJM, EquipWashInfo.jnBJ, EquipWashInfo.jnRX };
            int size = EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID].Count;
            for (int i = 0; i < size; i++)
            {
                EquipSuoData.WashInfo ww = new EquipSuoData.WashInfo();
                ww = EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID][i];
                ww._count = allNow[ww._num];
                EquipSuoData.m_listEquipWash[EquipWashInfo.zhuangbeiID][i] = ww;
            }
            ShowRangeInfo(EquipWashInfo);
            ShowRangeInfo(EquipWashInfo);
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
        }
        else
        {
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
        }
        FreshLab();
    }
     
    void WashBotton1(bool ison, int index)
    {
        if (ison)
        {
            m_EventPress.GetComponent<Collider>().enabled = ison;
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        }
        else
        {
            m_EventPress.transform.GetComponent<Collider>().enabled = ison;
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_EventPress.transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        }
    }

    void OnDisable()
    {
         //buttonNum = 0;
        //m_UnWashSuccesse.SetActive(false);
        //m_WashSuccesse.SetActive(false);
        SocketTool.UnRegisterMessageProcessor(this);
    }
  
    private string TimeInfo(int time)
    {
        int hour = time / 3600;
        int minute = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - minute * 60;
        return hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2") + NameIdTemplate.GetName_By_NameId(990021);
    }

    private bool XiLianIsOn()//装备是否可洗练
    {
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (savedId == ZhuangBei.templates[i].id)
            {
                if (ZhuangBei.templates[i].pinZhi > 1)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void WashButtonsShowState()//免费洗练和元宝洗练按钮的显示状态控制
    {
        if (pinzhiSaved > 1 && !_isFull)
        {
            if (EquipWashInfo.count4New > 0)
            {
                m_labNewAttSignal.text = LanguageTemplate.GetText(LanguageTemplate.Text.WASH_INFO)
                    + MyColorData.getColorString(4, EquipWashInfo.count4New.ToString())
                    + LanguageTemplate.GetText(LanguageTemplate.Text.WASH_INFO1);
            }
            else
            {
                m_labNewAttSignal.text = "";
            }
        }
        else
        {
            if (pinzhiSaved <= 1)
            {
                m_labNewAttSignal.text = "进阶装备后可洗练";
            }
            else if (ZhuangBei.getZhuangBeiById(savedId).jiejieId != 0 && _isFull)
            {
                m_LabJinJie.text = MyColorData.getColorString(1, "达到当前品质洗练上限，需进阶");
            }
            else if (_isFull)
            {
              m_labNewAttSignal.text = "所有属性已洗练到最大值";
            }
            else
            {
                m_labNewAttSignal.text = "";
            }
            //else if (_isFull && ZhuangBei.getZhuangBeiById(savedId).jiejieId == 0)
            //{
            //    m_labNewAttSignal.text = m_labNewAttSignal.text = MyColorData.getColorString(10, "进阶后可洗练");
            //}
        }

        if (pinzhiSaved > 1 && !_isFull)
        {
 
            m_EventPress.gameObject.SetActive(true);
            m_SuoTagSignal.SetActive(false);
            if (YBXiLianLimited == 0 && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
            {
                m_EventPress.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            }
            else
            {
                m_EventPress.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
            }
            m_labNewAttSignal.gameObject.SetActive(true);
            m_EventPress_JinJie.gameObject.SetActive(false);
            m_ObjNormal.SetActive(true);
           // m_ObjJinJie.SetActive(false);
            m_LabJinJie.text = MyColorData.getColorString(1, "洗练装备可以\n提升装备高级属性");
        }
        else
        {
 
            if (ZhuangBei.getZhuangBeiById(savedId).jiejieId != 0)
            {
                m_EventPress.gameObject.SetActive(false);
                m_ObjNormal.SetActive(false);
                m_EventPress_JinJie.gameObject.SetActive(true); 
            }
            else
            {
                m_EventPress_JinJie.gameObject.SetActive(false);
                m_ObjNormal.SetActive(true);
                m_EventPress.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            }
 
            if (pinzhiSaved == 1)
            {
 
                m_LabJinJie.text = MyColorData.getColorString(1, "绿色装备无法洗练\n需进阶");
            }
       
            else if (_isFull)
            {
                m_LabJinJie.text = MyColorData.getColorString(1, "所有属性已洗练到最大值\n无法继续洗练");
            }
        }
    }
    int index_s = 0;
    List<int> listIndeAddSuo = new List<int>();
    void EquipWashRequest(int index)//洗练请求判断
    {
        int index_Signal = 0;
        listIndeAddSuo.Clear();
        index_s = 0;
        listConnect.Clear();
        int[] connect = { 3, 3, 3, 3, 3, 3, 3, 3 };
        listConnect.AddRange(connect);
        m_MaskTouch.gameObject.SetActive(true);
 
        EquipWash(dbIdSave, savedId, index, listConnect[0], listConnect[1], listConnect[2], listConnect[3], listConnect[4], listConnect[5], listConnect[6], listConnect[7], pinzhiSaved);

    }

    public void UICallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.WASH_STONE_MAX_TIME);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, null, null);
    }
    public void UIBoxLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.YUAN_BAO_XILIAN_TIP);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, null, null);
    }

 
    void CreateMove(GameObject move, string content)
    {
        GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
        clone.transform.localPosition = move.transform.localPosition;
        clone.transform.localRotation = move.transform.localRotation;
        clone.transform.localScale = move.transform.localScale;
        clone.GetComponent<UILabel>().text = "";

        clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, content);


        clone.AddComponent(typeof(TweenPosition));
        clone.AddComponent(typeof(TweenAlpha));
        clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 80;
        clone.GetComponent<TweenPosition>().duration = 0.6f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.9f;
        StartCoroutine(WatiFor(clone));
    }
    IEnumerator WatiFor(GameObject obj)
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(obj);
    }
    int index_ShuXing = 0;
    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GridTop != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_GridTop.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.transform.GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listRangeInfo[index_ShuXing]);
            if (index_ShuXing < _listRangeInfo.Count - 1)
            {
                index_ShuXing++;
            }
            m_GridTop.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    public void ResourcesLoadCallBack3(ref WWW p_www, string p_path, Object p_object)
    {
       // if (m_GridBottom != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
          //  rewardShow.transform.parent = m_GridBottom.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
        //    rewardShow.transform.GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowXYAttribute(_listRangeInfo[index_ShuXing]);
            if (index_ShuXing < _listRangeInfo.Count - 1)
            {
                index_ShuXing++;
            }
          //  m_GridBottom.repositionNow = true;
        }
 
    }

    int index_add;
    public void ResourcesLoadAddCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GridTop2 != null)
        {
            //m_GridAdd
               GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_GridTop2.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.transform.GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listAddInfo[index_add]);
            m_GridAdd.repositionNow = true;
            if (index_add < _listAddInfo.Count - 1)
            {
                index_add ++;
            }
        }
        else
        {
            p_object = null;
        }
    }

    public void ResourcesLoadAddCallBack4(ref WWW p_www, string p_path, Object p_object)
    {
        //if (m_GridBottom2 != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
           // rewardShow.transform.parent = m_GridBottom2.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
           // rewardShow.transform.GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowXYAttribute(_listAddInfo[index_ShuXing]);
            if (index_add < _listAddInfo.Count - 1)
            {
                index_add++;
            }
         //   m_GridBottom2.repositionNow = true;
        }
        //else
        //{
        //    p_object = null;
        //}
    }


    private bool AllQualityLowest()
    {
        int sum = 0;
        int size =  EquipsOfBody.Instance().m_equipsOfBodyDic.Count;
        foreach (KeyValuePair<int, BagItem> item in EquipsOfBody.Instance().m_equipsOfBodyDic)
        {
            if (item.Value.pinZhi == 1)
            {
                sum++;
            }
        }
        if (size == sum)
        {
            return true;
        }
        return false;
    }
}
