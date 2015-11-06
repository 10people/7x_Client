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
    public static EquipGrowthWearManagerment m_EquipGrowth; 
    public List<EventIndexHandle> m_listTypeEvent;
    public List<EventIndexHandle> m_listEquipsEvent;
   // public GameObject m_UpgradeObj;

    public GameObject m_IntensifyOrWash;
    public List<GameObject> m_listSuoObj;
    public List<GameObject> m_ListPinZhi;


    public GameObject m_EquipTanHaoParent;
    public List<GameObject> m_listEquipTaHao;
    public List<GameObject> m_listAnimation;

    public ScaleEffectController m_SEC;
    public GameObject m_UpgradeTanHao;
	void Start () 
    {
        m_EquipGrowth = this;
        m_listTypeEvent.ForEach(p => p.m_Handle += ShowTypes);
        m_listEquipsEvent.ForEach(p => p.m_Handle += ShowEquips);
       // m_UpgradeTanHao.SetActive(EquipSuoData.AllUpgrade());
        m_SEC.OpenCompleteDelegate += ShowDefault;
	}
    private int materialSendId = 0;
    private int _Index_Type_Save = 0;//type
    private int _Index_Save = 3;//equip
    void ShowTypes(int index)
    {

        if (_Index_Type_Save != index)
        {
            switch (index)
            {
                case 0:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1211))
                        {
                            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                              , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_UPGRADE)
                              , "");
                            return;
                        }

                    }
                    break;
                case 1:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1212))
                        {
                            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                              , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_STRENGTH)
                              , "");
                            return;
                        }

                    }
                    break;
                case 2:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1210))
                        {
                            ShowTypes(1);
                            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                             , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_WASH)
                             , "");
                            return;
                        }
                    }
                    break;
                default: break;
            }

            FunctionWindowsCreateManagerment.SetSelectEquipInfo(index, _Index_Save);
            CityGlobalData.m_ShowSelectType = 0;
            if (index == 0)
            {
                ShowEquipTanHao();
            }

            if (index < 2)
            {
                m_EquipTanHaoParent.SetActive(true);
            }
            else
            {
                m_EquipTanHaoParent.SetActive(false);
            }

            //  if (_Index_Type_Save != index)
            {

                _Index_Type_Save = index;
                switch (index)
                {
                    case 0:
                        {
                            showAniMation();
                            m_listTypeEvent[0].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                            m_listTypeEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                            m_listTypeEvent[2].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                        }

                        break;
                    case 1:
                        {
                            m_listTypeEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                            m_listTypeEvent[0].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                            m_listTypeEvent[2].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                        }
                        break;
                    case 2:
                        {
                            showAniMation();
                            m_listTypeEvent[2].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(true);
                            m_listTypeEvent[0].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                            m_listTypeEvent[1].GetComponent<BbuttonColorChangeManegerment>().ButtonsControl(false);
                        }
                        break;
                    default:
                        break;
                }
                Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;

                if (index > 0)
                {
                    if (tempEquipsOfBodyDic.ContainsKey(_Index_Save))
                    {
                        m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().
                            GetEquipInfo(tempEquipsOfBodyDic[_Index_Save].itemId,
                            tempEquipsOfBodyDic[_Index_Save].dbId,
                            index - 1,
                            _Index_Save);
                    }
                }
                else
                {
                    if (tempEquipsOfBodyDic.ContainsKey(_Index_Save))
                    {
                        m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().
                            GetEquipInfo(tempEquipsOfBodyDic[_Index_Save].itemId,
                            tempEquipsOfBodyDic[_Index_Save].dbId,
                            2,
                            _Index_Save);
                    }
                }
            }
        }
      
    }


    void ShowEquips(int index)
    {
        FunctionWindowsCreateManagerment.SetSelectEquipInfo(_Index_Type_Save, index);
        if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100160;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        if (_Index_Save != index)
        {
            m_listEquipsEvent[_Index_Save].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(false);

            m_listEquipsEvent[index].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(true);

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
     //   if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = false;
            //if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
            //{
            //    m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().m_ScrollView.enabled = false;
            //    FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            //    TaskData.Instance.m_iCurMissionIndex = 100080;

            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //    tempTaskData.m_iCurIndex = 2;
            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //}

            //else if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
            //{
            //    m_IntensifyOrWash.GetComponent<EquipGrowthEquipInfoManagerment>().m_ScrollView.enabled = false;
            //    ////FunctionWindowsCreateManagerment.SetSelectEquipInfo(1,0);
            //    TaskData.Instance.m_iCurMissionIndex = 100160;
            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //    tempTaskData.m_iCurIndex = 1;
            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //}

            //else if (FreshGuide.Instance().IsActive(100330) && TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
            //{
            //    FunctionWindowsCreateManagerment.SetSelectEquipInfo(2, 3);
            //    TaskData.Instance.m_iCurMissionIndex = 100330;

            //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //    tempTaskData.m_iCurIndex = 2;
            //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //}
            //else
         
            {
                UIYindao.m_UIYindao.CloseUI();
            }
        }
   

        if (!string.IsNullOrEmpty(Global.m_sPanelWantRun))
        {

            int type = int.Parse(Global.NextCutting( ref Global.m_sPanelWantRun));
            int buwei = EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(int.Parse(Global.NextCutting(ref Global.m_sPanelWantRun))).buWei);
            _Index_Save = buwei;

            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(buwei))
            {

                m_listEquipsEvent[buwei].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
                m_listEquipsEvent[buwei].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
                Global.m_sPanelWantRun = null;

                switch (type)
                {
                    //case 0:
                    //    {
                    //        if (!FunctionOpenTemp.GetWhetherContainID(1211))
                    //        {
                    //            ShowTypes(1);
                    //            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                    //              , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_UPGRADE)
                    //              , "");
                    //        }
                    //        else
                    //        {
                    //            ShowTypes(type);
                    //        }
                    //    }
                    //    break;
                    case 1:
                        {
                            ShowTypes(type);
                        }
                        break;
                    case 2:
                        {
                            if (!FunctionOpenTemp.GetWhetherContainID(1210))
                            {
                                ShowTypes(1);
                                EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                                 , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_WASH)
                                 , "");
                            }
                            else
                            {
                                ShowTypes(type);
                            }
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                               , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_WEAR),"");
            }
        }
        else
        {
            string[] _info = FunctionWindowsCreateManagerment.m_EquipSaveInfo.Split(':');
            _Index_Save = int.Parse(_info[1]);
            m_listEquipsEvent[_Index_Save].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
            m_listEquipsEvent[_Index_Save].transform.parent.GetComponent<ButtonScaleManagerment>().ButtonsControl(true);
            switch (int.Parse(_info[0]))
            {
                //case 0:
                //    {
                //        if (!FunctionOpenTemp.GetWhetherContainID(1211))
                //        {
                //            ShowTypes(1);
                //            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                //              ,LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_UPGRADE)
                //              ,"");
                //        }
                //        else
                //        {
                //            ShowTypes(int.Parse(_info[0]));
                //        }
                //    }
                //    break;
                case 1:
                    {
                        ShowTypes(int.Parse(_info[0]));
                    }
                    break;
                case 2:
                    {
                        if (!FunctionOpenTemp.GetWhetherContainID(1210))
                        {
                            ShowTypes(1);
                            EquipSuoData.ShowSignal(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO)
                             , LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_WASH)
                             , "");
                        }
                        else
                        {
                            ShowTypes(int.Parse(_info[0]));
                        }
                    }
                    break;
                default:break;
            }
        }

       // ShowEquips(_Index_Save);
  }
    void showAniMation()
    {
        for (int i = 0; i < m_listAnimation.Count; i++)
        {
           m_listAnimation[i].SetActive(false);
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
 

    void OnEnable()
    {
        if (EquipsOfBody.Instance().m_equipsOfBodyDic != null)
        {
            ShowEquipInfo();
        }
    }

    void ShowEquipInfo()
    {
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;

        for (int i = 0; i < m_listEquipsEvent.Count; i++) //初始化玩家背包scrollview的item
        {
            if (tempEquipsOfBodyDic.ContainsKey(i))
            {
                m_listEquipsEvent[i].GetComponent<UISprite>().gameObject.SetActive(true);

                m_listEquipsEvent[i].GetComponent<UISprite>().enabled = true;
                m_listEquipsEvent[i].GetComponent<UISprite>().spriteName = ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId).icon;
                m_listSuoObj[i].gameObject.SetActive(false);
                m_ListPinZhi[i].GetComponent<UISprite>().spriteName =  QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(ZhuangBei.getZhuangBeiById(tempEquipsOfBodyDic[i].itemId).icon)));
                m_ListPinZhi[i].gameObject.SetActive(true);
               
            }
            else
            {
                m_ListPinZhi[i].gameObject.SetActive(false);
                m_listEquipsEvent[i].GetComponent<UISprite>().enabled = false;
                m_listSuoObj[i].gameObject.SetActive(true);
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

    private int EachUpgrade(int index)
    {
         if(EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
        {
            foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
            {
                if (int.Parse(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).jinjieItem) == item.Value[0].itemId)
                {
                    if (item.Value[0].cnt >= int.Parse(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).jinjieNum))
                    {
                        return EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei;
                    }
                }
            }
        }
        return 99;
    }

    public void ShowEquipTanHao()
    {
        int _size = m_listEquipTaHao.Count;
        for (int i = 0; i < _size; i++)
        {
            if (i == EachUpgrade(i))
            {
                if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(i))
                {
                    m_listEquipTaHao[i].SetActive(true);
                }
                else
                { 
                  m_listEquipTaHao[i].SetActive(false);
                }
            }
            else
            {
                m_listEquipTaHao[i].SetActive(false);
            }
        }
    }
}
