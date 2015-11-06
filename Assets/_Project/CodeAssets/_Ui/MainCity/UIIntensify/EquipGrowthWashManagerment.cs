using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

using ProtoBuf;
using ProtoBuf.Meta;
public class EquipGrowthWashManagerment : MonoBehaviour, SocketProcessor
{
    public UILabel m_TotalGold;
    public UILabel m_WashTimeDown;
    public GameObject m_EquipInfo;
    public GameObject m_SuoTagSignal;
    public GameObject m_TimesShow;
    public GameObject m_HidenGameobject;
    public GameObject m_WashTanHao;
    public GameObject m_WashSeniorTanHao;
    public GameObject m_WashTanHao2;
    public GameObject m_MainParent;
    public GameObject m_WashDisable;
    public UIGrid m_GridParent;
    public UILabel m_LabPPPP;
    public UILabel m_WashFreeTimesCount;
    public UILabel m_WashConsume;
    public UILabel m_WashCostTimesCount;
    public GameObject m_FreeTimesSingnalFinish;
    public GameObject m_FreeTimesSingnalTimesCount;
    public GameObject m_WashSignal;
    public GameObject m_LabSuoSignal;
    public List<UILabel> listAttrShow = new List<UILabel>();
    public GameObject m_WashSuccesse;
    public GameObject m_UnWashSuccesse;
    public List<EventIndexHandle> listEvent;
    XiLianRes EquipWashInfo = new XiLianRes();
    private int keepTime = 0;
    public List<GameObject> listButton = new List<GameObject>();
    public UILabel m_SuoSignalContent;
    //public GameObject m_YuanBaoSignal;
    public GameObject m_FreeWashComplete;
    public GameObject m_FreeWashIsOn;
    public GameObject m_NotEnoughYB;
    public GameObject m_MaskTouch;
    public List<EventPressIndexHandle> m_listPress;
    public UILabel m_labelStone;
    public GameObject m_ObjNewAttribute;
    public UILabel m_labNewAttSignal;
    private int _WashType = 0;
    private long dbIdSave;
    public int buttonNum = 0;
    public UICamera m_Camera;
    private int savedId;

    private bool unLocked = false;
    private int _StoneWashTImes = 0;
    private bool _isFull = false;

 
    private List<int> listConnect = new List<int>();
    private List<int> listSuoAdded = new List<int>();
    private int timesCurrent = 0;//
    private int _CurrentSecond = 0;
    private bool washYuanBao = false;
    private int pinzhiSaved = 0;
//    private int totalTime = 0;
    private int YBXiLianLimited = 0;
    private int _yuanbaoConSume = 0;

    public int m_EquipType = 0;
    public UIGrid m_GridAdd;
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
        _WadhStoneCount = BagData.Instance().GetCountByItemId(910002);
        listEvent.ForEach(p => p.m_Handle += TouchEvent);
        m_listPress.ForEach(p=> p.m_Handle += PressEvent);
        if (FreshGuide.Instance().IsActive(100330) && TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100330;

            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
    }
    void OnEnable()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
    void PressEvent(int index)
    {
        if (UICamera.GetTouches().Count == 1 && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) || Application.platform == RuntimePlatform.WindowsEditor)
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
                    if (index == 2)
                    {
                        washYuanBao = true;
                    }
                    else if (index == 1)
                    {

                        washYuanBao = false;
                    }

                    if (index == 2 && JunZhuData.Instance().m_junzhuInfo.yuanBao < EquipWashInfo.yuanBao && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
                    {
                        EquipSuoData.TopUpLayerTip(m_MainParent);
                        buttonNum = 0;
                    }
                    else
                    {
                        if (YBXiLianLimited == 0 && index == 2 && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
                        {
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxLoadCallback);
                            buttonNum = 0;
                        }
                        else
                        {
                            m_listPress[0].transform.GetComponent<Collider>().enabled = false;
                            m_listPress[1].transform.GetComponent<Collider>().enabled = false;
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
                //ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
                //UIYindao.m_UIYindao.CloseUI();
                //UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }

        if (UICamera.GetTouches().Count == 1 && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (index < 10)
            {
                if (index == 4 || index == 3)
                {
                    listEvent[0].transform.GetComponent<Collider>().enabled = false;
                    listEvent[1].transform.GetComponent<Collider>().enabled = false;
                    if (UIYindao.m_UIYindao.m_isOpenYindao)
                    {
                        if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_TaskInfoDic[100170].progress < 0)
                        {
                            // ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
                            //  UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
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
                    m_MaskTouch.SetActive(false);
                    unLocked = true;
                    m_WashSignal.SetActive(false);

                }
                else if (index == 12)
                {
                    m_MaskTouch.SetActive(false);
                    CityGlobalData.m_isWashMaxSignalConfirm = true;
                    unLocked = true;
                    m_WashSignal.SetActive(false);
                }
                //else if (index == 15)
                //{

                //    MainCityUI.TryRemoveFromObjectList(m_MainParent);
                //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                //                              UITopUp);
                //}
                //else if (index == 16)
                //{
                //    m_YuanBaoSignal.SetActive(false);
                //}
                else if (index == 10)
                {
                    EquipSuoData.TopUpLayerTip(m_MainParent);
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
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_XiLian, ref t_protof);
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
                        m_WashCostTimesCount.text = WashInfo.yuanBaoTimes.ToString(); //+ "/" + VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).YBxilianLimit.ToString();
                        timesCurrent = WashInfo.freeXilianTimes;
                        _CurrentSecond = WashInfo.time;
                        m_HidenGameobject.SetActive(true);
                     
                       
                        _StoneWashTImes = WashInfo.xilianshiTimes;
                        EquipWashInfo = WashInfo;

                        if (WashInfo.count4New > 0)
                        {
                            m_labNewAttSignal.text = LanguageTemplate.GetText(LanguageTemplate.Text.WASH_INFO) + WashInfo.count4New + LanguageTemplate.GetText(LanguageTemplate.Text.WASH_INFO1);
                        }
                        else
                        {
                            m_labNewAttSignal.text = "";
                        }
                        if (buttonNum == 0)
                        {
                            ShowRangeInfo(WashInfo);
                            ShowWashInfo();
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
                             //   Debug.Log("_yuanbaoConSume_yuanbaoConSume :::" + _yuanbaoConSume);
                            }
                            else
                            {

                                _yuanbaoConSume = 0;
                               // Debug.Log("_yuanbaoConSume_yuanbaoConSume :::" + _yuanbaoConSume);
                            }

                            if (buttonNum == 2 && _WadhStoneCount > 0 && _StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)))
                            {
                                _WadhStoneCount--;
                            }
                            AppendAttributeTidy(EquipWashInfo);
                            // m_UnWashSuccesse.SetActive(false);
                            // ShowWashSuccessInfo();
                            buttonNum = 0;
                        }
                        else if (buttonNum == 3 || buttonNum == 4)
                        {
                            SaveOrCancel(buttonNum);

                            if (timesCurrent > 0 && timesCurrent < VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && _CurrentSecond > 0 && !washYuanBao)
                            {
                                keepTime = _CurrentSecond;
                                if (IsInvoking("TimeDown") )
                                {
                                    CancelInvoke("TimeDown");
                                    InvokeRepeating("TimeDown", 1, 1);
                                }
                                else
                                {
                                    InvokeRepeating("TimeDown", 1, 1);
                                }
                            }
                            else if (timesCurrent == 0 && !washYuanBao)
                            {
                                WashBotton(false, 0);
                                keepTime = _CurrentSecond;
                                if (IsInvoking("TimeDown")){
                                    CancelInvoke("TimeDown");
                                   
									InvokeRepeating("TimeDown", 1, 1);
                                }
                                else
                                {
                                    InvokeRepeating("TimeDown", 1, 1);
                                }
                            }
                            else if (timesCurrent == VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && !washYuanBao)
                            {
                                CancelInvoke("TimeDown");
                                m_WashTimeDown.text = "";
                            }
                            m_MaskTouch.SetActive(false);
                            m_WashSuccesse.SetActive(false);
                            buttonNum = 0;
                        }
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
        List<int> _listAtt = new List<int>();

        int[] allNow = { xlres.wqSH, xlres.wqJM, xlres.wqBJ, xlres.wqRX, xlres.jnSH, xlres.jnJM, xlres.jnBJ, xlres.jnRX };
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

        for (int i = 0; i < _listAtt.Count; i++)
        {
            if (allNow[_listAtt[i]] == allMax[_listAtt[i]])
            {
                _isFull = true;
            }
            else
            {
                _isFull = false;
            }

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

                add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]]; ;
                add._NoHave = false;
                add._IsAdd = false;
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

                add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]]; ;
                add._NoHave = true;
                add._IsAdd = false;
                _listRangeInfo.Add(add);
            }
        }

        int size = m_GridParent.transform.childCount;
        int sizeR = _listRangeInfo.Count;
        if (size > 0)
        {
            //if (sizeR == size)
            //{
            //    for (int i = 0; i < size; i++)
            //    {
            //        m_GridParent.transform.GetChild(i).GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listRangeInfo[i]);
            //    }
            //}
            //else
            //{
            for (int i = 0; i < size; i++)
            {
                Destroy(m_GridParent.transform.GetChild(i).gameObject);
            }
            //}
        }

        index_ShuXing = 0;
        for (int i = 0; i < _listRangeInfo.Count; i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM2), ResourcesLoadCallBack2);
        }
    }
 
    void FreshLab()
    {
        m_MaskTouch.SetActive(false);
        m_listPress[0].transform.GetComponent<Collider>().enabled = true;
        m_listPress[1].transform.GetComponent<Collider>().enabled = true;
      
        if ((_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0) && _WashType == 2)
        {
            if (JunZhuData.Instance().m_junzhuInfo.yuanBao >= _yuanbaoConSume)
            {
                JunZhuData.Instance().m_junzhuInfo.yuanBao -= _yuanbaoConSume;
            }
        }

        if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 10000)
        {
            m_TotalGold.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000).ToString() + NameIdTemplate.GetName_By_NameId(990051);
        }
        else
        {
            m_TotalGold.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
        }
        m_WashFreeTimesCount.text = timesCurrent.ToString() + "/" + VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit;

        if (JunZhuData.Instance().m_junzhuInfo.yuanBao < EquipWashInfo.yuanBao)
        {
            m_WashConsume.text = MyColorData.getColorString(5, EquipWashInfo.yuanBao.ToString());
        }
        else
        {
            m_WashConsume.text = MyColorData.getColorString(1, EquipWashInfo.yuanBao.ToString());
        }
        m_labelStone.text = _WadhStoneCount.ToString();

        if (FunctionOpenTemp.GetWhetherContainID(1210) &&(timesCurrent > 0
            || (YBXiLianLimited > 0 && _StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WadhStoneCount > 0)))
        {
            m_WashTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(1210) && FunctionOpenTemp.GetWhetherContainID(1210));
            PushAndNotificationHelper.SetRedSpotNotification(1210, true);
        }
        else
        {
            m_WashTanHao.SetActive(false);
            PushAndNotificationHelper.SetRedSpotNotification(1210,  false);
        }
        m_WashSeniorTanHao.SetActive(_StoneWashTImes < int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) && _WadhStoneCount > 0 && !_isFull ? true : false);
        m_WashTanHao2.SetActive(timesCurrent > 0 && FunctionOpenTemp.GetWhetherContainID(1210) && !_isFull);

        WashButtonsShowState();
    }

    void ShowWashInfo()
    {

        FreshLab();
        keepTime = _CurrentSecond;
	
        if (JunZhuData.Instance().m_junzhuInfo.yuanBao < EquipWashInfo.yuanBao)
        {
            m_WashConsume.text = MyColorData.getColorString(5, EquipWashInfo.yuanBao.ToString());
        }
        else
        {
            m_WashConsume.text = MyColorData.getColorString(1, EquipWashInfo.yuanBao.ToString());
        }
		
     
        if ( EquipWashInfo.wqSHAdd == 0 && EquipWashInfo.wqJMAdd == 0 && EquipWashInfo.wqBJAdd == 0 && EquipWashInfo.wqRXAdd == 0 && EquipWashInfo.jnSHAdd == 0 && EquipWashInfo.jnJMAdd == 0 && EquipWashInfo.jnBJAdd == 0 && EquipWashInfo.jnRXAdd == 0 )
        {
            m_WashTimeDown.text = TimeInfo(_CurrentSecond);         
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
            if (timesCurrent < VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && keepTime > 0 && pinzhiSaved > 1)
            {
                if (IsInvoking("TimeDown") )
                {
                    CancelInvoke("TimeDown");

                    InvokeRepeating("TimeDown", 0, 1.0f);
                }
                else
                {
                    InvokeRepeating("TimeDown", 0, 1.0f);
                }
            }
            else if (timesCurrent < VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && keepTime > 0 && pinzhiSaved <= 1)
            {
                m_WashFreeTimesCount.text =   timesCurrent.ToString() + "/" +VipTemplate.GetVipInfoByLevel(0).xilianLimit;
                
				m_FreeWashIsOn.SetActive(true);
                
				m_FreeWashComplete.SetActive(false);
                
				m_WashTimeDown.text = "";
            }
            else if (timesCurrent < VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && keepTime <= 0 && pinzhiSaved > 1)
            {
                EquipWash(dbIdSave, savedId, 0, 3, 3, 3, 3, 3, 3, 3, 3, pinzhiSaved);
            }
            else if (timesCurrent == VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit && !washYuanBao)
            {
                m_WashTimeDown.text = "";
            }
        }
        else
        {
			if(timesCurrent == 0)
			{
				m_WashTimeDown.text = "";
			}
            m_WashSuccesse.SetActive(true);
          //  m_MaskTouch.SetActive(true);
            m_UnWashSuccesse.SetActive(false);
   
            SuccessedAttribute();
        }

       
        WashButtonsShowState();
        if (FreshGuide.Instance().IsActive(100330) && TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100330;

            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
    }
    void ShowWashSuccessInfo()
    {
        FreshLab();
        m_WashSuccesse.SetActive(true);
      // m_MaskTouch.SetActive(true);
 
        if (buttonNum == 3 || buttonNum == 4)
        {
      //      m_MaskTouch.SetActive(false);
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

    void SuccessedAttribute()//显示新生成属性
    {
        _listAddInfo.Clear();
        List<int> _listAtt = new List<int>();
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
        int[] allNow = { EquipWashInfo.wqSH, EquipWashInfo.wqJM, EquipWashInfo.wqBJ, EquipWashInfo.wqRX, EquipWashInfo.jnSH, EquipWashInfo.jnJM, EquipWashInfo.jnBJ, EquipWashInfo.jnRX };

        int[] attribute = { EquipWashInfo.wqSHAdd, EquipWashInfo.wqJMAdd, EquipWashInfo.wqBJAdd, EquipWashInfo.wqRXAdd, EquipWashInfo.jnSHAdd, EquipWashInfo.jnJMAdd, EquipWashInfo.jnBJAdd, EquipWashInfo.jnRXAdd };

        int[] allMax = { EquipWashInfo.wqSHMax, EquipWashInfo.wqJMMax, EquipWashInfo.wqBJMax, EquipWashInfo.wqRXMax, EquipWashInfo.jnSHMax, EquipWashInfo.jnJMMax, EquipWashInfo.jnBJMax, EquipWashInfo.jnRXMax };
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
                if (attribute[_listAtt[i]] > 0)
                {
                    add._IsAdd = true;
                    add._Max2 = allMax[_listAtt[i]];
                    add._Count2 = allNow[_listAtt[i]] + attribute[_listAtt[i]];
                    add._Count = allNow[_listAtt[i]];
                }
                else if (attribute[_listAtt[i]] < 0)
                {
                    add._IsAdd = false;
                    add._Max2 = allMax[_listAtt[i]];
                    add._Count2 = allNow[_listAtt[i]];
                    add._Count = allNow[_listAtt[i]] + attribute[_listAtt[i]];
                }
                else
                {
                    add._Max2 = 0;
                    add._Count = allNow[_listAtt[i]];
                }
                add._CountAdd = attribute[_listAtt[i]];
                add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]]; ;
                add._NoHave = false;
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
                add._NeedUpgrade = allMax[_listAtt[i]] == allNow[_listAtt[i]]; ;
                add._NoHave = true;
                add._IsAdd = false;
                _listAddInfo.Add(add);
            }
        }
        int size = m_GridAdd.transform.childCount;
        int size_add = _listAddInfo.Count;
        if (size > 0)
        {
            for (int i = 0; i < size_add; i++)
            {
                m_GridAdd.transform.GetChild(i).GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listAddInfo[i]);
            }
        }
        else
        {
            index_add = 0;
            for (int i = 0; i < _listAddInfo.Count; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM2), ResourcesLoadAddCallBack);
            }
        }
    }
 
    private List<EquipSuoData.WashInfo> _listEquipWash;
    private void AppendAttributeTidy(XiLianRes xlres)
    {
        EquipSuoData.m_listNewAttribute.Clear();

        _listEquipWash = new List<EquipSuoData.WashInfo>();
        int[] allNow = { xlres.wqSH, xlres.wqJM, xlres.wqBJ, xlres.wqRX, xlres.jnSH, xlres.jnJM, xlres.jnBJ, xlres.jnRX };

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
                wss._add = allAdd[i];
                wss._isMax = allNow[i] == allMax[i] ? true : false;
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
        m_WashFreeTimesCount.text = timesCurrent.ToString() + "/" + VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit;
        if (EquipSuoData.GetWetherContainNewAttribute(EquipWashInfo.zhuangbeiID))
        {
            FreshLab();
            ShowRangeInfo(xlres);
            m_MaskTouch.SetActive(false); 
            listEvent[0].transform.GetComponent<Collider>().enabled = true;
            listEvent[1].transform.GetComponent<Collider>().enabled = true;
            m_UnWashSuccesse.SetActive(true);
            //m_labelStone.text = _WadhStoneCount.ToString();
            //listEvent[0].gameObject.SetActive(false);
            UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_ObjNewAttribute, EffectIdTemplate.GetPathByeffectId(100180), null);
            m_EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().AppendAttributeUpdate(EquipWashInfo.zhuangbeiID);
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
        UI3DEffectTool.Instance().ClearUIFx(m_ObjNewAttribute);
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
        listEvent[0].transform.GetComponent<Collider>().enabled = true;
        listEvent[1].transform.GetComponent<Collider>().enabled = true;
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
            m_EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().AppendAttributeUpdate(EquipWashInfo.zhuangbeiID);
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
            /// listEvent[0].gameObject.SetActive(true);
  
        }
        else
        {
    
            m_UnWashSuccesse.SetActive(true);
            m_WashSuccesse.SetActive(false);
        }
        FreshLab();
    }
     
    void WashBotton(bool ison, int index)
    {
        if (ison)
        {
            listButton[index].transform.GetComponent<Collider>().enabled = ison;
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        }
        else
        {
            listButton[index].transform.GetComponent<Collider>().enabled = ison;
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        }
    }

    void OnDisable()
    {
        //buttonNum = 0;
        //m_UnWashSuccesse.SetActive(false);
        //m_WashSuccesse.SetActive(false);
        SocketTool.UnRegisterMessageProcessor(this);
    }

    void TimeDown()
    {
        if (keepTime >= 0)
        {
            m_WashTimeDown.text = TimeInfo(keepTime);
            keepTime--;
             if (keepTime > 0)
            {
                m_FreeWashIsOn.SetActive(true);
                m_FreeWashComplete.SetActive(false);
                m_WashTimeDown.text = TimeInfo(keepTime);
            }
            else if (keepTime < 0)
            {
                if (timesCurrent < VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit)
                {
                    CancelInvoke("TimeDown");
                    buttonNum = 0;
                    EquipWash(dbIdSave, savedId, 0, 3, 3, 3, 3, 3, 3, 3, 3, pinzhiSaved);
                  
                }
                else if (timesCurrent == VipTemplate.GetVipInfoByLevel(JunZhuData.Instance().m_junzhuInfo.vipLv).xilianLimit)
                {
                    CancelInvoke("TimeDown");
                    m_WashTimeDown.text = "";
                }
            }
        }
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
        //if (EquipSuoData.Instance().m_EquipSuoInfo.ContainsKey(savedId))
        //{
        //    m_WashTimeDown.gameObject.SetActive(false);
        //    if (!EquipSuoData.Instance().m_EquipSuoInfo[savedId].oneSuo && !EquipSuoData.Instance().m_EquipSuoInfo[savedId].twoSuo && !EquipSuoData.Instance().m_EquipSuoInfo[savedId].threeSuo && !EquipSuoData.Instance().m_EquipSuoInfo[savedId].fourSuo && EquipWashInfo.freeXilianTimes > 0 && XiLianIsOn() && pinzhiSaved > 1 && timesCurrent > 0)
        //    {
        //        WashBotton(true, 0);
        //        m_TimesShow.SetActive(true);
        //        m_LabSuoSignal.gameObject.SetActive(false);
                
        //    }
        //    else
        //    {
        //       m_LabSuoSignal.gameObject.SetActive(true);
        //       m_TimesShow.SetActive(false);
        //        WashBotton(false, 0);
          
        //    }
        //}
        //else
        {
            m_WashTimeDown.gameObject.SetActive(true);
            m_LabSuoSignal.gameObject.SetActive(false);

         
            if (pinzhiSaved > 1 && !_isFull)
            {
                m_WashDisable.SetActive(false);
                m_TimesShow.SetActive(true);
                m_SuoTagSignal.SetActive(false);
                WashBotton(timesCurrent > 0 ? true : false, 0);
           
                if (YBXiLianLimited == 0 && (_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
                {
                    m_WashDisable.SetActive(false);
                    WashBotton(false, 1);
                }
                else
                {
                    WashBotton(true, 1);
                }
            }
            else
            {
                //if (pinzhiSaved <= 1)
                //{
                    m_WashDisable.SetActive(true);
                    WashBotton(false, 1);
                //}
                //else if (YBXiLianLimited == 0 &&(_StoneWashTImes >= int.Parse(CanshuTemplate.GetStrValueByKey(CanshuTemplate.XILIANSHI_MAXTIMES)) || _WadhStoneCount == 0))
                //{
                //    m_WashDisable.SetActive(false);
                //    WashBotton(false, 1);
                //}
                //else
                //{
                //    m_WashDisable.SetActive(false);
                //    WashBotton(true, 1);
                //}
                WashBotton(false, 0);
            }
        }

        //if (EquipSuoData.Instance().m_EquipSuoInfo.ContainsKey(savedId))
        //{
        //    int SuoIndex = 0;
        //    for(int i = 0;i < listSuoAdded.Count;i++ )
        //    {  
        //      SuoIndex += listSuoAdded[i];
        //    }

        //    if (!XiLianIsOn() || SuoIndex == EquipSuoData.Instance().listIndexs.Count ? true : false)
        //    {
        //        WashBotton(false, 1);
        //    }
        //    else
        //    {
        //        if (YBXiLianLimited > 0)
        //        {
        //            WashBotton(true, 1);
        //        }
        //        else
        //        {
        //            WashBotton(false, 1);
        //        }
        //    }
        //}
        //else
        //{
        //    if (!EquipSuoData.Instance().m_EquipSuoInfo.ContainsKey(savedId))
        //    {
        //        listSuoAdded.Clear();
        //    }

        //    if (!XiLianIsOn() || YBXiLianLimited == 0)
        //    {
        //        WashBotton(false, 1);
        //    }
        //    else
        //    {
        //        WashBotton(true, 1);
        //    }
        //}
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
        m_MaskTouch.SetActive(true);
 
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
        if (m_GridParent != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_GridParent.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.transform.GetComponent<EquipGrowthAttributeManagerment>().ShowInfo(_listRangeInfo[index_ShuXing]);
            if (index_ShuXing < _listRangeInfo.Count - 1)
            {
                index_ShuXing++;
            }
            m_GridParent.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    int index_add;
    public void ResourcesLoadAddCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GridParent != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_GridAdd.transform;
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
}
