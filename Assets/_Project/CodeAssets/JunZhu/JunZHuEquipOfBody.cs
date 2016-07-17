using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class JunZHuEquipOfBody : MonoBehaviour, SocketProcessor
{
    public static JunZHuEquipOfBody m_EquipOfBody;
    public List<JunZhuEquipInfoManagerment> m_listEquipEleInfo;
 


    private float _SaveValue = -1.0f;
    public static bool m_IsJiHuo = false;
    public struct SelfEquipInfo
    {
        public string _icon;
        public int _PinZhi;
        public int _quality;
        public string _Name;
        public bool _TanHao;
        public bool _isAdd;
        public bool _isAdvance;
        public string _Signal0;
        public string _Signal1;
        public bool _isParent;
        public int _NextItemIcon;
        public int _JinJieId;
    };

    private List<SelfEquipInfo> _listSelfEquipInfo = new List<SelfEquipInfo>();
    public List<UISprite> m_equipList;
    public List<UISprite> m_equipAdd;
    public List<UISprite> m_equipUpgrade;
    public List<UISprite> m_equipPinZhi;
    public JunZhuZhuangBeiInfo m_zhuanbeiInfo;
 
 
    public GameObject m_ObjEffectUse;
    public GameObject m_ObjEffectWenZi;
 
    public GameObject m_Mask;
 
    private bool _isShowEffect = false;
    private Vector3 vec_target = new Vector3(280, 180, 0);
    private List<Vector3> list_pos = new List<Vector3>();
    public List<GameObject> listMovObj;
    private FunctionWindowsCreateManagerment.EquipTaoJiHuo m_JiHuoTaozhuangInfo;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        m_JiHuoTaozhuangInfo = new FunctionWindowsCreateManagerment.EquipTaoJiHuo();
        m_EquipOfBody = this;
        Vector3[] vvv = { new Vector3(-47, 183, 0),new Vector3(- 47,93, 0),new Vector3(- 47,0, 0)
                ,new Vector3( - 47,- 96, 0),new Vector3(52,- 185, 0),new Vector3(220,-185, 0)
                ,new Vector3(310 - 93, 0),new Vector3(380,0, 0),new Vector3(415,90,0) };
        list_pos.AddRange(vvv);
      
        foreach (UISprite tempSprite in m_equipList)
        {
            tempSprite.GetComponent<EventHandler>().m_click_handler += ShowEquipOfBody;
        }
     

    }
    void TTeffect(int index)
    {
        EffectShowOne();
    }
    

    int indeValue = 1;
    TaoZhuangResp _TaoZhuang_Temp;

    void ShowActivaty()
    {
        _TaoZhuang_Temp = EquipsOfBody.Instance().m_Activatetemp;
       
    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
 
                case ProtoIndexes.activate_tao_zhuang_resp:
                    {
                        //MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        //QiXiongSerializer t_qx = new QiXiongSerializer();

                        //ActivateTaoZhResp temp = new ActivateTaoZhResp();

                        //t_qx.Deserialize(t_stream, temp, temp.GetType());
                        //if (temp.success == 0)
                        //{
                        //    _TaoZhuang_Temp.maxActiZhuang = temp.activatedId;
                        //    FunctionWindowsCreateManagerment.m_IsEquipJihuoShow = true;
                        //    MoveOnNow();
                        //    // EffectShowOne();
                          
                           
                        //}
                        //  MoveOnNow();
                        return true;
                    }
                default: return false;
            }
        }
        return false;
    }

    void EffectShowOne()
    {
        m_ObjEffectUse.SetActive(true);
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_ObjEffectUse, EffectIdTemplate.GetPathByeffectId(620214), null);
      //StartCoroutine(WaitForOne());
    }

 

    void EffectShowTwo()
    {

        UI3DEffectTool.ClearUIFx(m_ObjEffectUse);
        //UI3DEffectTool.ShowMidLayerEffect(UI3DEffectTool.UIType.FunctionUI_1,
        //                                            m_ObjEffectWenZi,
        //                                            EffectIdTemplate.GetPathByeffectId(620215));
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_ObjEffectUse, EffectIdTemplate.GetPathByeffectId(620215), null);
        m_ObjEffectWenZi.SetActive(true);
        StartCoroutine(WaitForTwo());
    }
    IEnumerator WaitForTwo()
    {
        yield return new WaitForSeconds(0.5f);
        m_ObjEffectUse.SetActive(false);
        m_ObjEffectWenZi.SetActive(false);
        UI3DEffectTool.ClearUIFx(m_ObjEffectWenZi);

    }

    void ResLoaded_JiHuo(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.GetComponent<EquipTaoZhuangLayerManagerment>().ShowInfo(m_JiHuoTaozhuangInfo);
    }
    void OnDisable()
    {

        ClearAdvanceEffect();
    }

   
    void JiHuoInfo(TaoZhuangResp temp)
    {
        if (TaoZhuangTemplate.GetNextTaoZhuangById(temp.maxActiZhuang) != null)
        {
            FunctionWindowsCreateManagerment.EquipTaoJiHuo taozhuangInfo = new FunctionWindowsCreateManagerment.EquipTaoJiHuo();
            taozhuangInfo._gong = JunZhuData.Instance().m_junzhuInfo.gongJi;
            taozhuangInfo._fang = JunZhuData.Instance().m_junzhuInfo.fangYu;
            taozhuangInfo._ming = JunZhuData.Instance().m_junzhuInfo.shengMing;
            taozhuangInfo._gongadd = TaoZhuangTemplate.GetNextTaoZhuangById(temp.maxActiZhuang).num1;
            taozhuangInfo._fanggadd = TaoZhuangTemplate.GetNextTaoZhuangById(temp.maxActiZhuang).num2;
            taozhuangInfo._minggadd = TaoZhuangTemplate.GetNextTaoZhuangById(temp.maxActiZhuang).num3;
            taozhuangInfo._quality = TaoZhuangTemplate.GetNextTaoZhuangById(temp.maxActiZhuang).color;
            FunctionWindowsCreateManagerment.m_JiHuoInfo = taozhuangInfo;

        }
    }
    private int GetColorNeed(int _pinzhi)
    {
        int colorNum = 31;
        switch (_pinzhi)
        {
            // case 2:
            // case 3:
            //  colorNum = 4;
            //break;
            case 2:
            case 3:
        
                colorNum = 33;
                break;
            case 4:
            case 5:
            case 6:
                colorNum = 32;
                break;
            case 7:
            case 8:
            case 9:
                colorNum = 31;
                break;
        }

        return colorNum;

    }
    private bool _isFresh = false;
    void Update()
    {
        if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_TaskInfoDic[100110].progress < 0)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        if (EquipsOfBody.Instance().m_isRefrsehEquips)
        {
            EquipsOfBody.Instance().m_isRefrsehEquips = false;

            if (EquipsOfBody.Instance().m_equipsOfBodyDic != null)
            {
 
                EquipDataTidy();
            }
        }

        if (EquipsOfBody.Instance().m_ActivateLoad)
        {
            EquipsOfBody.Instance().m_ActivateLoad = false;
            ShowActivaty();
            EquipDataTidy();
        }
    }
    void EquipDataTidy()
    {
        _indexNum = 0;
        _listSelfEquipInfo.Clear();
        _list_Num.Clear();
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        int size = m_listEquipEleInfo.Count;
        for (int j = 0; j < size; j++)
        {
            if (tempEquipsOfBodyDic.ContainsKey(j))
            {
                for (int i = 0; i < ZhuangBei.templates.Count; i++)
                {
                    if (ZhuangBei.templates[i].id == tempEquipsOfBodyDic[j].itemId)
                    {
                        SelfEquipInfo equip = new SelfEquipInfo();
                        equip._icon = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[j].itemId).icon;
                        equip._JinJieId = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[j].itemId).jiejieId;
                        equip._quality = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[j].itemId).pinZhi;
                        if (tempEquipsOfBodyDic[j].qiangHuaLv > 0)
                        {
                            equip._Name = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name)) + "+" + tempEquipsOfBodyDic[j].qiangHuaLv.ToString());
                        }
                        else
                        {
                            equip._Name = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name)));
                        }

                        equip._PinZhi = CommonItemTemplate.getCommonItemTemplateById(tempEquipsOfBodyDic[j].itemId).color;
                        equip._isAdd = false;
                        equip._isAdvance = ChangeEquip(j);
                        //equip._TanHao = (equip._isAdvance || equip._isAdd);
                        int MaterialCount = GetMaterialCountByID(int.Parse(ZhuangBei.templates[i].jinjieItem));
                        if (ZhuangBei.templates[i].jiejieId == 0)
                        {
                            equip._Signal0 = "";
                            equip._Signal1 = MyColorData.getColorString(34, LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_EQUIP_SIGNAL));
                     
                            equip._NextItemIcon = 0;
                        }
                        else if (ChangeEquip(j))
                        {
                            equip._Signal0 = "";
                            equip._Signal1 = MyColorData.getColorString(35, LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_EQUIP_SIGNAL1));
                       
                            equip._NextItemIcon = 0;
                        }
                        else
                        {
                            if (int.Parse(ZhuangBei.templates[i].jinjieNum) <= MaterialCount)
                            {
                            
                                equip._Signal0 = MyColorData.getColorString(35, LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_EQUIP_SIGNAL2));
                                equip._Signal1 = "";
 
                                equip._NextItemIcon = ZhuangBei.templates[i].jiejieId;
                            }
                            else if (TaoZhuangTemplate.GetNextTaoZhuangById(EquipsOfBody.Instance().m_Activatetemp.maxActiZhuang) != null
                                && TaoZhuangTemplate.GetNextTaoZhuangById(EquipsOfBody.Instance().m_Activatetemp.maxActiZhuang).condition > equip._quality
                                && EquipsOfBody.Instance().GetEquipCountByQuality(TaoZhuangTemplate.GetNextTaoZhuangById(EquipsOfBody.Instance().m_Activatetemp.maxActiZhuang).condition) >= (TaoZhuangTemplate.GetNextTaoZhuangById(EquipsOfBody.Instance().m_Activatetemp.maxActiZhuang).neededNum/2.0f))
                            {
                     
                                equip._Signal0 = MyColorData.getColorString(5, "急需进阶");
                                equip._Signal1 = "";
                          
                          
                                equip._NextItemIcon = ZhuangBei.templates[i].jiejieId;
                            }
                            else
                            {
                       
                                equip._Signal0 = MyColorData.getColorString(33, LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_EQUIP_SIGNAL3));
                                equip._Signal1 = "";
                            
                              
                                equip._NextItemIcon = ZhuangBei.templates[i].jiejieId;
                            }
                        }
                        equip._TanHao = (equip._isAdvance || equip._isAdd);
                        _listSelfEquipInfo.Add(equip);
                    }
                }
            }
            else
            {
                SelfEquipInfo equip = new SelfEquipInfo();
                equip._icon = "";
                equip._PinZhi = 999;
                equip._isAdd = EquipInBag(j);
                equip._isAdvance = false;
                equip._TanHao = (equip._isAdvance || equip._isAdd);
                equip._Name = MyColorData.getColorString(10, PosName(j));
                if (equip._isAdd)
                {
                    equip._Signal0 = "";
                    equip._Signal1 = MyColorData.getColorString(35, LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_EQUIP_SIGNAL1));
                    // equip._Signal1 = MyColorData.getColorString(13, "点击穿戴新装备!");
    
                }
                else
                {
                    equip._Signal0 = "";
                    equip._Signal1 = MyColorData.getColorString(34, ZBChushiDiaoluoTemp.GetTemplateById(EquipsOfBody.BuWeiRevert(j)));
                }
                _listSelfEquipInfo.Add(equip);
            }
        }
        ClearAdvanceEffect();
        for (int i = 0; i < size; i++)
        {
            m_listEquipEleInfo[i].m_SpriteIcon.GetComponent<Collider>().enabled = true;
         
            m_listEquipEleInfo[i].m_SpriteIcon.spriteName = _listSelfEquipInfo[i]._icon;
           
           

            if (FunctionWindowsCreateManagerment.SpecialSizeFit(_listSelfEquipInfo[i]._PinZhi))
            {
                m_listEquipEleInfo[i].m_SpritePinZhi.height = m_listEquipEleInfo[i].m_SpritePinZhi.width = 92;
            }
            else
            {
                m_listEquipEleInfo[i].m_SpritePinZhi.height = m_listEquipEleInfo[i].m_SpritePinZhi.width = 85;
            }
            if (!string.IsNullOrEmpty(QualityIconSelected.SelectQuality(_listSelfEquipInfo[i]._PinZhi)))
            {
                m_listEquipEleInfo[i].m_SpritePinZhi.gameObject.SetActive(true);
                m_listEquipEleInfo[i].m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(_listSelfEquipInfo[i]._PinZhi);
            }
            else
            {
                m_listEquipEleInfo[i].m_SpritePinZhi.gameObject.SetActive(false);
            }
//            m_listEquipEleInfo[i].m_LabName.text = _listSelfEquipInfo[i]._Name;
//            m_listEquipEleInfo[i].m_LabSignal0.text = _listSelfEquipInfo[i]._Signal0;
//            m_listEquipEleInfo[i].m_LabSignal1.text = _listSelfEquipInfo[i]._Signal1;
//            m_listEquipEleInfo[i].m_UpgradeProgress.gameObject.SetActive(_listSelfEquipInfo[i]._isProgress);

            if (_listSelfEquipInfo[i]._isAdvance)
            {
                ShowEffert(i);
            }
            m_listEquipEleInfo[i].m_Tanhao.SetActive(_listSelfEquipInfo[i]._TanHao);
            m_listEquipEleInfo[i].m_Add.SetActive(_listSelfEquipInfo[i]._isAdd);
            m_listEquipEleInfo[i].m_Advance.SetActive(_listSelfEquipInfo[i]._isAdvance);
            m_listEquipEleInfo[i].m_LabNameBox.gameObject.SetActive(!_listSelfEquipInfo[i]._isAdd && !_listSelfEquipInfo[i]._isAdvance && string.IsNullOrEmpty(_listSelfEquipInfo[i]._icon));
//            if (m_listEquipEleInfo[i].m_Parent.transform.childCount > 0)
//            {
//                Destroy(m_listEquipEleInfo[i].m_Parent.transform.GetChild(0).gameObject);
//            }

            if (_listSelfEquipInfo[i]._NextItemIcon != 0 && _listSelfEquipInfo[i]._JinJieId != 0)
            {
                _list_Num.Add(i);
            }
        }
        EquipCreate();
        if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_iCurMissionIndex == 100060 && TaskData.Instance.m_TaskInfoDic[100060].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100060;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 3;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_iCurMissionIndex == 100100 && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100100;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 3;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100405) && TaskData.Instance.m_iCurMissionIndex == 100405 && TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100405;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 3;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

        }
        if (FunctionWindowsCreateManagerment.m_BuWeiNum != -1)
        {
            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(FunctionWindowsCreateManagerment.m_BuWeiNum))
            {
                m_zhuanbeiInfo.GetEquipInfo(EquipsOfBody.Instance().m_equipsOfBodyDic[FunctionWindowsCreateManagerment.m_BuWeiNum].itemId, FunctionWindowsCreateManagerment.m_BuWeiNum);
                m_zhuanbeiInfo.gameObject.SetActive(true);
           

            }
            FunctionWindowsCreateManagerment.m_BuWeiNum = -1;
        }

   
    }
    private List<int> _list_Num = new List<int>();

    void EquipCreate()
    {
        for (int i = 0; i < _list_Num.Count; i++)
        {
            CreateItem();
        }
     }
   
    public void ClearAdvanceEffect()
    {
        int size = m_listEquipEleInfo.Count;
        for (int i = 0; i < size; i++)
        {
            m_listEquipEleInfo[i].m_Animation.SetActive(false);
        }
    }
    private void ShowEffert(int index)
    {
        m_listEquipEleInfo[index].m_Animation.SetActive(true);
    }
    //private void ShowEffert(GameObject obj)
    //{
    //    UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, obj, EffectIdTemplate.GetPathByeffectId(100186), null);
    //}
    void ShowEquipOfBody(GameObject tempObject) //显示玩家身上的装备信息
    {
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_iCurMissionIndex == 100100 && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100100;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 4;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100405) && TaskData.Instance.m_iCurMissionIndex == 100405 && TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100405;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 4;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;


        if (tempEquipsOfBodyDic.ContainsKey(int.Parse(tempObject.name)) && !_listSelfEquipInfo[int.Parse(tempObject.name)]._isAdvance)
        {
            SendZhuangBeiInfo(tempEquipsOfBodyDic[int.Parse(tempObject.name)].itemId, int.Parse(tempObject.name));
        }
        else
        {
            int tempBuwei = 0;
            switch (int.Parse(tempObject.name))
            {
                case 3:
                    tempBuwei = 1;
                    break;//刀
                case 4:
                    tempBuwei = 2;
                    break;//枪
                case 5:
                    tempBuwei = 3;
                    break;//弓
                case 0:
                    tempBuwei = 11;
                    break;//头盔
                case 8:
                    tempBuwei = 12;
                    break;//肩膀
                case 1:
                    tempBuwei = 13;
                    break;//铠甲
                case 7:
                    tempBuwei = 14;
                    break;//手套
                case 2:
                    tempBuwei = 15;
                    break;//裤子
                case 6:
                    tempBuwei = 16;
                    break;//鞋子
                default:
                    break;
            }

            if (_listSelfEquipInfo[int.Parse(tempObject.name)]._isAdd)//穿装备
            {
                m_listEquipEleInfo[int.Parse(tempObject.name)].m_SpriteIcon.GetComponent<Collider>().enabled = false;

 
                Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;

                foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
                {
                    if (item.Value.buWei == tempBuwei)
                    {

                        EquipsOfBody.Instance().EquipADD(item.Value.dbId);
                        MainCityUIRB.setDeletePropUse(item.Value.itemId);
                         
                        EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
                        break;
                    }
                }

          
            }
            else if (_listSelfEquipInfo[int.Parse(tempObject.name)]._isAdvance)//替换装备
            {
                m_listEquipEleInfo[int.Parse(tempObject.name)].m_SpriteIcon.GetComponent<Collider>().enabled = false;
   
                List<BagItem> _listEquip = new List<BagItem>();
                Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
                foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
                {
                    if (item.Value.buWei == tempBuwei)
                    {
                        //  tempAddReq.gridIndex = item.Value.bagIndex;
                        _listEquip.Add(item.Value);
                        EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
                    }
                }

                if (_listEquip.Count > 1)
                {
                    for (int j = 0; j < _listEquip.Count; j++)
                    {
                        for (int i = 0; i < _listEquip.Count - 1 - j; i++)
                        {
                            if (_listEquip[i].pinZhi < _listEquip[i + 1].pinZhi)
                            {
                                BagItem equip = new BagItem();
                                equip = _listEquip[i];
                                _listEquip[i] = _listEquip[i + 1];
                                _listEquip[i + 1] = equip;
                            }

                        }
                    }
					MainCityUIRB.setDeletePropUse(_listEquip[0].itemId);
                
                    EquipsOfBody.Instance().EquipADD(_listEquip[0].dbId);
                }
                else
                {
					MainCityUIRB.setDeletePropUse(_listEquip[0].itemId);
                    EquipsOfBody.Instance().EquipADD(_listEquip[0].dbId);
                }
            }
            else
            {
                ClientMain.m_UITextManager.createText(ZBChushiDiaoluoTemp.GetTemplateById(
                                                     EquipsOfBody.BuWeiRevert(int.Parse(tempObject.name))));
            }

        }
 
        
    }

 

    void OnEnable()
    {
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100100;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 3;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        //else
        //{
        //    UIYindao.m_UIYindao.CloseUI();
        //}

        if (EquipsOfBody.Instance().m_equipsOfBodyDic != null)
        {
            EquipDataTidy();
        }

        
    }

    void ShowEquipInfo()
    {
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;

        for (int i = 0; i < m_equipList.Count; i++) //初始化玩家背包scrollview的item
        {
            UI3DEffectTool.ClearUIFx(m_equipPinZhi[i].gameObject);
            if (tempEquipsOfBodyDic.ContainsKey(i))
            {
                m_equipList[i].gameObject.SetActive(true);

                m_equipList[i].GetComponent<UISprite>().enabled = true;
                m_equipList[i].spriteName = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId).icon;
                m_equipAdd[i].gameObject.SetActive(false);
                m_equipPinZhi[i].GetComponent<UISprite>().spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId).icon)));
                m_equipPinZhi[i].gameObject.SetActive(true);
                //				m_equipList[i].GetComponent<EquipDragDropItem>().SetData(tempEquipsOfBodyDic[i],i); // 345
                m_equipUpgrade[i].gameObject.SetActive(ChangeEquip(i));
                if (ChangeEquip(i))
                {
                    ShowEffert(i);
                }
            }
            else
            {
                m_equipPinZhi[i].gameObject.SetActive(false);
                m_equipUpgrade[i].gameObject.SetActive(false);
                m_equipAdd[i].gameObject.SetActive(EquipInBag(i));
                //	m_equipList[i].gameObject.SetActive(false);
            }
        }

    }

 
    private void SendZhuangBeiInfo(int id, int buwei)
    {
        m_zhuanbeiInfo.GetEquipInfo(id, buwei);
        m_zhuanbeiInfo.gameObject.SetActive(true);
 
    }
    void OnDestroy()
    {
        m_EquipOfBody = null;
        SocketTool.UnRegisterMessageProcessor(this);
    }

    private bool EquipInBag(int buwei)
    {
        int tempBuwei = 0;
        switch (buwei)
        {
            case 3:
                tempBuwei = 1;
                break;//刀
            case 4:
                tempBuwei = 2;
                break;//枪
            case 5:
                tempBuwei = 3;
                break;//弓
            case 0:
                tempBuwei = 11;
                break;//头盔
            case 8:
                tempBuwei = 12;
                break;//肩膀
            case 1:
                tempBuwei = 13;
                break;//铠甲
            case 7:
                tempBuwei = 14;
                break;//手套
            case 2:
                tempBuwei = 15;
                break;//裤子
            case 6:
                tempBuwei = 16;
                break;
            default:
                break;
        }
        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {
            if (item.Value.buWei == tempBuwei)
            {
                return true;
            }
        }
        return false;
    }


    private bool ChangeEquip(int buwei)
    {
        List<BagItem> listEquip = new List<BagItem>();

        foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
        {
            int tempBuwei = 0;
            switch (item.Value.buWei)
            {
                case 1: tempBuwei = 3; break;//重武器
                case 2: tempBuwei = 4; break;//轻武器
                case 3: tempBuwei = 5; break;//弓
                case 11: tempBuwei = 0; break;//头盔
                case 12: tempBuwei = 8; break;//肩膀
                case 13: tempBuwei = 1; break;//铠甲
                case 14: tempBuwei = 7; break;//手套
                case 15: tempBuwei = 2; break;//裤子
                case 16: tempBuwei = 6; break;//鞋子
                default: break;
            }
            if (tempBuwei == buwei && item.Value.pinZhi > EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].pinZhi)
            {
                return true;
            }
        }
        return false;
    }
    private int GetMaterialCountByID(int id)
    {
        foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].itemId == id)
                {
                    return item.Value[i].cnt;
                }
            }
        }

        return 0;
    }

    private string PosName(int buwei)
    {
        string str = "";
        switch (buwei)
        {
            case 0:
                {
                    str = "头盔";
                }
                break;
            case 1:
                {
                    str = "铠甲";
                }
                break;
            case 2:
                {
                    str = "护腿";
                }
                break;
            case 3:
                {
                    str = "长柄武器";
                }
                break;
            case 4:
                {
                    str = "双持武器";
                }
                break;
            case 5:
                {
                    str = "远程武器";
                }
                break;
            case 6:
                {
                    str = "战靴";
                }
                break;
            case 7:
                {
                    str = "手套";
                }
                break;
            case 8:
                {
                    str = "护肩";
                }
                break;

            default:
                break;
        }

        return str;
    }

    void CreateItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                          ResLoaded);
    }

    private int _indexNum = 0;
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
//        if (m_listEquipEleInfo[_list_Num[_indexNum]].m_Parent != null)
//        {
//            GameObject tempObject = (GameObject)Instantiate(p_object);
//            tempObject.transform.parent = m_listEquipEleInfo[_list_Num[_indexNum]].m_Parent.transform;
//            tempObject.transform.transform.localPosition = Vector3.zero;
//            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
//            iconSampleManager.SetIconByID(_listSelfEquipInfo[_list_Num[_indexNum]]._NextItemIcon, "", 20);
//            tempObject.transform.localScale = Vector3.one * 0.45f;
//            iconSampleManager.SetIconPopText(_listSelfEquipInfo[_list_Num[_indexNum]]._NextItemIcon, NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(_listSelfEquipInfo[_list_Num[_indexNum]]._NextItemIcon).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(_listSelfEquipInfo[_list_Num[_indexNum]]._NextItemIcon).descId));
//            if (_indexNum < _list_Num.Count -1)
//            {
//                _indexNum++;  
//            }
//        }
//        else
//        {
            p_object = null;
//        }
    }

 
    private GameObject _PiaoLingObject = null;
    int index_Move = 0;

    void MoveOnNow()
    {
        int size = listMovObj.Count;
        for (int i = 0; i < size; i++)
        {
            MoveActionAlong(listMovObj[i].transform);
        }
    }
    void MoveActionAlong(Transform trans)
    {
        
       UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, trans.gameObject, EffectIdTemplate.GetPathByeffectId(620216), null);
      // UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, trans.gameObject, EffectIdTemplate.GetPathByeffectId(620217), null);
        iTween.MoveTo(trans.transform.gameObject, iTween.Hash("position", vec_target, "islocal", true, "delay", 0, "easeType", "linear", "time", 0.5f, "oncomplete", "SendName", "oncompletetarget", gameObject));
    }
    void SendName()
    {
        index_Move++;
        if (index_Move == listMovObj.Count)
        {
            StartCoroutine(WaitForOne());
        }
        else if(index_Move > listMovObj.Count)
        {
            for (int i = 0; i < listMovObj.Count; i++)
            {
                UI3DEffectTool.ClearUIFx(listMovObj[i]);
                listMovObj[i].transform.localPosition = list_pos[i];
            }
            index_Move = 1;
           // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_TAO), ResLoaded_JiHuo);
        }
    }

    IEnumerator WaitForOne()
    {
        yield return new WaitForSeconds(0.25f);
        for (int i = 0; i < listMovObj.Count; i++)
        {
            UI3DEffectTool.ClearUIFx(listMovObj[i]);
            listMovObj[i].transform.localPosition = list_pos[i];
        }
        index_Move = 0;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_TAO), ResLoaded_JiHuo);
    }
}
