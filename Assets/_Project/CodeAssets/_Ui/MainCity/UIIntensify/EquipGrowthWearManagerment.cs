using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipGrowthWearManagerment : MonoBehaviour
{
   // public static EquipGrowthWearManagerment m_EquipGrowth; 
    public List<EventIndexHandle> m_listTypeEvent;
    public EventIndexHandle m_EventIllustrate;
    public GameObject m_Durable_UI;
    public List<EquipGrowthEquipItemManagerMent> m_listItemEvent;
    public List<UILabel> m_listLevel;
    public UILabel m_LabToSignal;
    public GameObject m_IntensifyOrWash;
    public GameObject m_illustrateObj;
    public UILabel m_LabContent;
    public ScaleEffectController m_SEC;
    public GameObject m_ObjTopLeft;
    private bool _IsNotNull = true;
    private int[] ss = { 3, 4, 5, 0, 8, 1,7, 2, 6 };
    private int materialSendId = 0;
    [HideInInspector]
    public int _Index_Type_Save = -1;//type
    [HideInInspector]
    public int _Index_Save = 4;//equip
    void Awake()
    {
    }
    void Start () 
    {
        m_LabToSignal.text = MyColorData.getColorString(2, LanguageTemplate.GetText(1513));
        m_LabContent.text = LanguageTemplate.GetText(1355);
        MainCityUI.setGlobalBelongings(m_Durable_UI, 0, 0);
        MainCityUI.setGlobalTitle(m_ObjTopLeft, "装备", 0, 0);
      //  MainCityUI.setGlobalTitle(m_ObjTopLeft, "装备打造", 0, 0);
       // m_EquipGrowth = this;
        m_listTypeEvent.ForEach(p => p.m_Handle += ShowTypes);
        m_SEC.OpenCompleteDelegate += ShowDefault;
        m_EventIllustrate.m_Handle += Showillusrate;
        m_listItemEvent.ForEach(p => p.m_Event.m_Handle += ShowEquips);
    }

    void Showillusrate(int index)
    {
        GeneralControl.Instance.LoadRulesPrefab(LanguageTemplate.GetText(1355));
    }
    void OnEnable()
    {
        TaskData.Instance.IsCanShowComplete = false;
        UIYindao.m_UIYindao.CloseUI();
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100100;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

            if (_Index_Type_Save != 3 && _Index_Save != 4)
            {
                tempTaskData.m_iCurIndex = 3;
            }
            else if (_Index_Type_Save == 3 && _Index_Save == 4)
            {
                tempTaskData.m_iCurIndex = 4;
            }
            else if (_Index_Type_Save != 3 && _Index_Save == 4)
            {
                tempTaskData.m_iCurIndex = 3;
            }
            else
            {
                tempTaskData.m_iCurIndex = 4;
            }
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        else if (FreshGuide.Instance().IsActive(200030) && TaskData.Instance.m_TaskInfoDic[200030].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 200030;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

            if (_Index_Type_Save == 3 && _Index_Save == 4)
            {
                tempTaskData.m_iCurIndex = 3;
            }
            else
            {
                tempTaskData.m_iCurIndex = 2;
            }

            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }

        m_SEC.transform.localScale = Vector3.one;
        if (EquipsOfBody.Instance().m_equipsOfBodyDic != null)
        {
            ShowEquipInfo();


            if (Global.m_sPanelWantRun != null
                && !string.IsNullOrEmpty(Global.m_sPanelWantRun)
                && Global.m_sPanelWantRun != "null")
            {
                ShowDefault();

            }
            else if (_Index_Type_Save != -1)
            {
                if (!EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(_Index_Save))
                {
                    _Index_Save = ContainEquip();
                }
                Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
                if (tempEquipsOfBodyDic.ContainsKey(_Index_Save))
                {
                    m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().
                        GetEquipInfo(tempEquipsOfBodyDic[_Index_Save].itemId,
                        tempEquipsOfBodyDic[_Index_Save].dbId,
                        _Index_Type_Save - 1,
                        _Index_Save);
                }
            }
        }
    }
 
  
   public void ShowTypes(int index)
    {
        if (_Index_Type_Save != index)
        {
            switch (index)
            {
                case 2:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1210))
                        {
                            ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(1210).m_sNotOpenTips);
                            return;
                        }
                    }
                    break;
                case 0:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1213))
                        {
                            ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(1213).m_sNotOpenTips);
                            return;
                        }
                    }
                    break;
                case 3:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1211))
                        {
                            ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(1211).m_sNotOpenTips);
                         
                            return;
                        }
                    }
                    break;
                default: break;
            }
            if (_Index_Type_Save != -1)
            {
                m_listTypeEvent[_Index_Type_Save].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
            }
            m_listTypeEvent[index].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);

            FunctionWindowsCreateManagerment.SetSelectEquipInfo(index, _Index_Save);
            CityGlobalData.m_ShowSelectType = 0;

            if (index == 2)
            {
                StrengthTanHao();
            }
          
            _Index_Type_Save = index;
          
            switch (index)
            {
                case 2:
                    {
                        if (FreshGuide.Instance().IsActive(100705) && TaskData.Instance.m_TaskInfoDic[100705].progress >= 0)
                        {
                            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
                            TaskData.Instance.m_iCurMissionIndex = 100705;
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            tempTaskData.m_iCurIndex = 3;
                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                        }
                        showAniMation();
                       

                    }
                    break;
                default:
                    break;
            }
            Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
            if (tempEquipsOfBodyDic.ContainsKey(_Index_Save))
            {
                m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().
                    GetEquipInfo(tempEquipsOfBodyDic[_Index_Save].itemId,
                    tempEquipsOfBodyDic[_Index_Save].dbId,
                    index - 1,
                    _Index_Save);
            }

        }
    }
  

    void ShowEquips(int index)
    {
        if (FreshGuide.Instance().IsActive(100705) && TaskData.Instance.m_TaskInfoDic[100705].progress >= 0 && index == 4)
        {
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100705;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 4;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        FunctionWindowsCreateManagerment.SetSelectEquipInfo(_Index_Type_Save, index);
        if (_Index_Save != index)
        {
            m_listItemEvent[_Index_Save].GetComponent<ButtonScaleManagerment>().ButtonsControl(false);

            m_listItemEvent[index].GetComponent<ButtonScaleManagerment>().ButtonsControl(true);

            _Index_Save = index;
            Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
            if (tempEquipsOfBodyDic.ContainsKey(index))
            {
                m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().
                    GetEquipInfo(tempEquipsOfBodyDic[index].itemId,
                    tempEquipsOfBodyDic[index].dbId,
                    _Index_Type_Save - 1,
                    index);
            }
        }

    }

    public void ShowDefault()
    {
        if (Global.m_sPanelWantRun != null
            && !string.IsNullOrEmpty(Global.m_sPanelWantRun)
            && Global.m_sPanelWantRun != "null")
        {
            int type = int.Parse(Global.NextCutting(ref Global.m_sPanelWantRun));
            int buwei = -1;

            string equip_id = Global.NextCutting(ref Global.m_sPanelWantRun);
            if (!string.IsNullOrEmpty(equip_id))
            {
                int tt = 0;
                if (int.TryParse(equip_id, out tt))
                {
                    buwei = EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(int.Parse(equip_id)).buWei);
                }
                else
                {
                    string[] _info = FunctionWindowsCreateManagerment.m_EquipSaveInfo.Split(':');
                    buwei = int.Parse(_info[1]);
                }
            }
            else
            {
                string[] _info = FunctionWindowsCreateManagerment.m_EquipSaveInfo.Split(':');
                buwei = int.Parse(_info[1]);
            }

            if (buwei < 0)
            {
                buwei = _Index_Save;
            }

            Global.m_sPanelWantRun = null;

            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(buwei))
            {

                m_listItemEvent[buwei].GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
                m_listItemEvent[_Index_Save].GetComponent<ButtonScaleManagerment>().ButtonsControl(false);
                _Index_Save = buwei;
                _IsNotNull = false;
                switch (type)
                {
                    case 0:
                        {
                            ShowTypes(type);

                        }
                        break;
                    case 1:
                        {

                            ShowTypes(type);
                        }
                        break;
                    case 2:
                        {
                            ShowTypes(type);
                        }
                        break;
                    case 3:
                        {
                            ShowTypes(type);
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                ClientMain.m_UITextManager.createText(LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_WEAR));

            }
        }
        else
        {
            string[] _info = FunctionWindowsCreateManagerment.m_EquipSaveInfo.Split(':');

            m_listItemEvent[_Index_Save].GetComponent<ButtonScaleManagerment>().ButtonsControl(false);
            _Index_Save = int.Parse(_info[1]);

            int[] arrange = { 3, 4, 5, 0, 8, 1, 7, 2, 6 };
            if (!EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(_Index_Save))
            {
                _Index_Save = ContainEquip();
            }
    
            m_listItemEvent[_Index_Save].GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
            switch (int.Parse(_info[0]))
            {
                case 0:
                    {
                        ShowTypes(int.Parse(_info[0]));
                    }
                    break;
                case 1:
                    {
                        ShowTypes(int.Parse(_info[0]));
                    }
                    break;
                case 2:
                    {
                        ShowTypes(int.Parse(_info[0]));
                    }
                    break;
                case 3:
                    {
                        ShowTypes(int.Parse(_info[0]));
                    }
                    break;
                default: break;
            }
        }
    }
    void showAniMation()
    {
        for (int i = 0; i < m_listItemEvent.Count; i++)
        {
            m_listItemEvent[i].m_ObjEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (EquipsOfBody.Instance().m_isRefrsehEquips)
        {
            EquipsOfBody.Instance().m_isRefrsehEquips = false;
            if (EquipsOfBody.Instance().m_equipsOfBodyDic != null)
            {
                ShowEquipInfo();
            }
        }

        if (!_IsNotNull && !string.IsNullOrEmpty(Global.m_sPanelWantRun))
        {
            _IsNotNull = true;
           ShowDefault();
        }
    }

    void ShowEquipOfBody(GameObject tempObject) //显示玩家身上的装备信息
    {

        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        {
            int tempBuwei = 0;
            switch (int.Parse(tempObject.name))
            {
                case 3: tempBuwei = 1;
                    break;//刀
                case 4: tempBuwei = 2;
                    break;//枪
                case 5: tempBuwei = 3;
                    break;//弓
                case 0: tempBuwei = 11;
                    break;//头盔
                case 8: tempBuwei = 12;
                    break;//肩膀
                case 1: tempBuwei = 13;
                    break;//铠甲
                case 7: tempBuwei = 14;
                    break;//手套
                case 2: tempBuwei = 15;
                    break;//裤子
                case 6: tempBuwei = 16;
                    break;//鞋子
                default:
                    break;
            }
            Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
        }
    }
 



    void ShowEquipInfo()
    {
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;

        for (int i = 0; i < m_listItemEvent.Count; i++) //初始化玩家背包scrollview的item
        {
            if (tempEquipsOfBodyDic.ContainsKey(i))
            {
                if (tempEquipsOfBodyDic[i].qiangHuaLv > 0)
                {
                    m_listItemEvent[i].m_Level.text = "Lv." + tempEquipsOfBodyDic[i].qiangHuaLv.ToString();
                }
               else
               {
                    m_listItemEvent[i].m_Level.text = "";
               }
                m_listItemEvent[i].m_Event.GetComponent<Collider>().enabled = true;
                m_listItemEvent[i].m_SpriteIcon.enabled = true;
                m_listItemEvent[i].m_SpriteIcon.spriteName = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId).icon;
                m_listItemEvent[i].m_Suo.gameObject.SetActive(false);
                if (FunctionWindowsCreateManagerment.SpecialSizeFit(CommonItemTemplate.getCommonItemTemplateById(tempEquipsOfBodyDic[i].itemId).color))
                {
                    m_listItemEvent[i].m_SpritePinZhi.transform.localPosition = new Vector3(1, 4, 0);
                    m_listItemEvent[i].m_SpritePinZhi.width = m_listItemEvent[i].m_SpritePinZhi.height = 84;
                }
                else
                {
                    m_listItemEvent[i].m_SpritePinZhi.transform.localPosition = new Vector3(0, -1, 0);
                    m_listItemEvent[i].m_SpritePinZhi.width = m_listItemEvent[i].m_SpritePinZhi.height = 74;
                }
                    m_listItemEvent[i].m_SpritePinZhi.spriteName =  QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(tempEquipsOfBodyDic[i].itemId).color);
                m_listItemEvent[i].m_SpritePinZhi.gameObject.SetActive(true);
               
            }
            else
            {
                m_listItemEvent[i].m_Level.text = "";
                m_listItemEvent[i].m_Event.GetComponent<Collider>().enabled = false;
                m_listItemEvent[i].m_SpriteIcon.enabled = false;
                m_listItemEvent[i].m_Suo.SetActive(true);
                m_listItemEvent[i].m_SpritePinZhi.gameObject.SetActive(false);
            }
        }
    }

    private bool UpgRadeIsOn(int id, int level)
    {
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {

            if (ZhuangBei.templates[i].id == id && level >= ZhuangBei.templates[i].jinjieLv)
            {
                foreach (KeyValuePair<long, List<BagItem>> item2 in BagData.Instance().m_playerCaiLiaoDic)
                {
                    for (int j = 0; j < item2.Value.Count; j++)
                    {
                        if (item2.Value[j].itemId == int.Parse(ZhuangBei.templates[i].jinjieItem))
                        {
                            return item2.Value[j].cnt > int.Parse(ZhuangBei.templates[i].jinjieNum);
                        }
                    }
                }
            }

        }
        return false;
    }

    void OnDisable()
    {
        _IsNotNull = false;
        //m_EquipGrowth = null;
        TaskData.Instance.IsCanShowComplete = true;
    }
    void OnDestroy()
    {
        //SocketTool.UnRegisterSocketListener(this);
    }

    private bool EquipInBag(int buwei)
    {
        int tempBuwei = 0;
        switch (buwei)
        {
            case 3: tempBuwei = 1;
                break;//刀
            case 4: tempBuwei = 2;
                break;//枪
            case 5: tempBuwei = 3;
                break;//弓
            case 0: tempBuwei = 11;
                break;//头盔
            case 8: tempBuwei = 12;
                break;//肩膀
            case 1: tempBuwei = 13;
                break;//铠甲
            case 7: tempBuwei = 14;
                break;//手套
            case 2: tempBuwei = 15;
                break;//裤子
            case 6: tempBuwei = 16;
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

    public void StrengthTanHao()
    {
        int _size = m_listItemEvent.Count;
        for (int i = 0; i < _size; i++)
        {
          m_listItemEvent[i].m_ObjTanHao.SetActive(false);
        }
    }

    private bool CompoundContainEnable()
    {
        int EquipExp = 0;
        foreach (KeyValuePair<int, BagItem> equip in EquipsOfBody.Instance().m_equipsOfBodyDic)
        {
            EquipExp = equip.Value.jinJieExp;
            if (ZhuangBei.getZhuangBeiById(equip.Value.itemId).jiejieId > 0)
            {
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
                    if (tempBuwei == equip.Value.buWei
                        && item.Value.pinZhi <= equip.Value.pinZhi)
                    {
                        EquipExp += ZhuangBei.GetItemByID(item.Value.itemId).exp;
                        if (EquipExp >= ZhuangBei.getZhuangBeiById(equip.Value.itemId).lvlupExp)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private int ContainEquip()
    {
        int[] arrange = { 3, 4, 5, 0, 8, 1, 7, 2, 6 };

        for (int i = 0; i < arrange.Length; i++)
        {
            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(arrange[i]))
            {
                return arrange[i];
            }
        }

        return arrange[0];
    }
}
