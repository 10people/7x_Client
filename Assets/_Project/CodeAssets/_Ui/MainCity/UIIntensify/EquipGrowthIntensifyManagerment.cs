using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipGrowthIntensifyManagerment : MonoBehaviour
{
    public GameObject m_MainParent;
    public GameObject m_NoMaterial;
    public GameObject m_Material;
    public GameObject m_IntensifyTag;

    public UIGrid IconSampleGrid;
    public List<EventHandler> listEvent;
    public List<EventHandler> listTagEvent;
    public List<UISprite> listSprite;
    public int SelectType = 0;
    public UIProgressBar m_EquipExp;
    public UILabel m_LabProgress;
    public EquipGrowthEquipInfoManagerment m_EquipGrowthEquipInfoManagerment;
    public List<GameObject> m_listEquipTaHao;

    public List<GameObject> m_listGameObject;

    private bool isMaxSave;
    private long savedbid = 0;
    private int currSave;
    private int maxSave;

    private int EquipSavedId = 0;
    public List<GameObject> listButton = new List<GameObject>();

    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoStrenth = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoAdvance = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoEquips = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    Dictionary<long, List<BagItem>> BagCaiLiao = new Dictionary<long, List<BagItem>>();
    private int EquipType = 0;
    public GameObject m_SignalTag2;
    private int Equip_BuWei = 0;
    private int Equip_PinZhi = 0;
    private int Equip_Level = 0;
    private bool WetherHaveMaterial = false;
    void Start()
    {
        listEvent.ForEach(p => p.m_handler += IntensifyTouch);
        listTagEvent.ForEach(p => p.m_handler += TagConfirm);
      
    }

    void OnEnable()
    {
        index_Num = 0;
        Addindex = 0;

        curr_residue = 0;
        curr_Max = 0;
        addCount = 0;
        IsResidueOn = false;
        ReduceIndex_Now = 0;
        lastcontent = 0;
        currentLevel = 0;
        EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = false;
        ExpXxmlTemp.m_listNeedInfo.Clear();
        ExpXxmlTemp.m_listReduceInfo.Clear();
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        BagCaiLiao = BagData.Instance().m_playerCaiLiaoDic;
        if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
        {
            m_EquipGrowthEquipInfoManagerment.m_ScrollView.enabled = false;
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100080;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
        {
            m_EquipGrowthEquipInfoManagerment.m_ScrollView.enabled = false;
            TaskData.Instance.m_iCurMissionIndex = 100160;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
    }
    void Update()
    {
        //if (EquipGrowthMaterialUseManagerment.listTouchedId.Count == 0 && !EquipGrowthMaterialUseManagerment.strengthenIsOn)//按钮状态显示控制
        //{
        //    EquipGrowthMaterialUseManagerment.strengthenIsOn = true;
        //    for (int i = 0; i < 1; i++)
        //    {
        //        WashBotton(false, i);
        //    }
        //}
        //else if (EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0 && EquipGrowthMaterialUseManagerment.strengthenIsOn)
        //{
 
        //    EquipGrowthMaterialUseManagerment.strengthenIsOn = false;
        //    for (int i = 0; i < 1; i++)
        //    {
        //        WashBotton(true, i);
        //    }
        //}

        if (EquipGrowthMaterialUseManagerment.materialItemTouched)
        {
            EquipGrowthMaterialUseManagerment.materialItemTouched = false;
            ProgressBarExhibition();
        }

        if (WetherHaveMaterial)
        {
            WetherHaveMaterial = false;
            ShowMaterialInfo();
        }
    }

    void TagConfirm(GameObject obj)//强化Signal界面按钮控制
    {
        if (obj.name.Equals("ButtoncConfirm"))
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            EquipStrengthReq equip = new EquipStrengthReq();
         
            equip.usedEquipIds = new List<long>();
            equip.usedEquipIds.Clear();
     
            equip.type = 2;

            t_qx.Serialize(t_tream, equip);
            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.S_EQUIP_UPALLGRADE, ref t_protof);

        }
        else if (obj.name.Equals("ButtonConfirm"))
        {
            m_SignalTag2.SetActive(false);
            EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);
        }
        else if (obj.name.Equals("ButtonCancel2"))
        {
            m_SignalTag2.SetActive(false);
        }
        else
        {
            m_IntensifyTag.SetActive(false);
        }
    }

    void IntensifyTouch(GameObject obj)//强化界面按钮控制
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = false;
            if (FreshGuide.Instance().IsActive(100160) && TaskData.Instance.m_TaskInfoDic[100160].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100160;

                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 4;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100080) && TaskData.Instance.m_TaskInfoDic[100080].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100080;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 4;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else if (FreshGuide.Instance().IsActive(100125) && TaskData.Instance.m_TaskInfoDic[100125].progress >= 0)
            {
                //ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100125];
                //UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
        }

        if (obj.name.Equals("ButtondQiangHua") && EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0)
        {
            if (curr_residue > curr_Max)
            {
                m_SignalTag2.SetActive(true);
            }
            else
            {
                EquipGrowthEquipInfoManagerment.m_WetherIsIntensify = true;
                if (EquipGrowthMaterialUseManagerment.listTouchedId.Count == 0)
                {
                    EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);
                }
                else
                {
                    Strengthen(Equip_BuWei);
                }
            }
        }
        else if (obj.name.Equals("ButtondYJQiangHua"))
        {
            if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(6))
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad_YiJianQiangHua);
            }
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad);
            }
        }
        else if (obj.name.Equals("ButtondGM"))
        {

            //if (FunctionOpenTemp.GetWhetherContainID(9))
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadGouMai);

            }
 
        }
    }

    public void RB_UB_Button3_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }
    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + VipFuncOpenTemplate.GetNeedLevelByKey(6).ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, null, null);
    }

    public void ShowInfo(int equipid, int type, int curr, int max, long dbid, int level, int buwei, int pinzhi)//界面信息显示
    {
        curr_residue = 0;
        curr_Max = 0;
        addCount = 0;
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        ShowEquipTanHao();

        Equip_Level = level;
        Material_Num = 0;
        listM.Clear();

        Equip_BuWei = buwei;

        Equip_PinZhi = pinzhi;
        EquipGrowthMaterialUseManagerment.listCurrentAddExp.Clear();
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        EquipSavedId = equipid;
        EquipGrowthMaterialUseManagerment.m_EuipId = equipid;
        currSave = curr;
        maxSave = max;
        isMaxSave = curr < max ? true : false;
        savedbid = dbid;

        if (level >= JunZhuData.Instance().m_junzhuInfo.level)
        {
            WashBotton(false, 0);
            WashBotton(false, 1);
        }
        else if (max != -1 && level < JunZhuData.Instance().m_junzhuInfo.level)
        {

            m_EquipExp.value = curr / float.Parse(max.ToString());
            m_LabProgress.text = curr.ToString() + "/" + max.ToString();
             WashBotton(true, 0);
             WashBotton(true, 1);
        }
        else if (max == -1)
        {
            m_EquipExp.value = 1.0f;
            m_LabProgress.text = "";
            WashBotton(false, 0);
            WashBotton(false, 1);

        }
        if (buwei == 3 || buwei == 4 || buwei == 5)
        {
            EquipType = 1;
        }
        else
        {
            EquipType = 0;
        }
        BagCaiLiao = BagData.Instance().m_playerCaiLiaoDic;

        CaiLiaoStrenth.Clear();
        CaiLiaoAdvance.Clear();
        CaiLiaoEquips.Clear();
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        MaterialsInfoTidy();
    }

    public void ButtonShow()
    {
        WashBotton(false, 0);
        WashBotton(false, 1);
    }

    void MaterialsInfoTidy()//材料信息整理
    {
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        foreach (KeyValuePair<long, List<BagItem>> item in BagCaiLiao)
        {
            for (int i = 0; i < ItemTemp.templates.Count; i++)
            {
                if (item.Value[0].itemId == ItemTemp.templates[i].id/* && ItemTemp.templates[i].quality != 0 */)
                {
                        if (EquipType == 0 && item.Value[0].itemType == 2)
                        {

                            EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                            material.dbid = item.Value[0].dbId;
                            material.materialId = item.Value[0].itemId;
                            material.icon = ItemTemp.templates[i].icon;
                            material.count = item.Value[0].cnt.ToString();
                            material.isSelected = false;
                            material.quality = item.Value[0].pinZhi;
                            material.isTouchControl = isMaxSave;
                            material.materialEXP = ItemTemp.templates[i].effectId;
                            CaiLiaoStrenth.Add(material);
                        }
                        else if (EquipType == 1 && item.Value[0].itemType == 1)
                        {
                            EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                            material.dbid = item.Value[0].dbId;
                            material.materialId = item.Value[0].itemId;
                            material.icon = ItemTemp.templates[i].icon;
                            material.count = item.Value[0].cnt.ToString();
                            material.isSelected = false;
                            material.quality = item.Value[0].pinZhi;
                            material.isTouchControl = isMaxSave;
                            material.materialEXP = ItemTemp.templates[i].effectId;
                            CaiLiaoStrenth.Add(material);
                        }
                        else if (item.Value[0].itemType == 6)
                        {
                            foreach (KeyValuePair<int, BagItem> equip  in tempEquipsOfBodyDic)
                            {
                                int tempBuwei = 0;
                                switch (ZhuangBei.GetBuWeiByItemId(item.Value[0].itemId))
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
                                if (EquipType == 1)
                                {
                                    if (tempBuwei == 3 || tempBuwei == 4 || tempBuwei == 5)
                                    {
                                        if (tempBuwei == equip.Value.buWei && equip.Value.pinZhi > item.Value[0].pinZhi)
                                        {
                                            EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                                            material.dbid = item.Value[0].dbId;
                                            material.materialId = item.Value[0].itemId;
                                            material.icon = ItemTemp.templates[i].icon;
                                            material.count = item.Value[0].cnt.ToString();
                                            material.isSelected = false;
                                            material.quality = item.Value[0].pinZhi;
                                            material.isTouchControl = isMaxSave;
                                            material.materialEXP = ItemTemp.templates[i].effectId;
                                            CaiLiaoAdvance.Add(material);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tempBuwei != 3 && tempBuwei != 4 && tempBuwei != 5)
                                    {
                                        if (tempBuwei == equip.Value.buWei && equip.Value.pinZhi > item.Value[0].pinZhi)
                                        {
                                            EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                                            material.dbid = item.Value[0].dbId;
                                            material.materialId = item.Value[0].itemId;
                                            material.icon = ItemTemp.templates[i].icon;
                                            material.count = item.Value[0].cnt.ToString();
                                            material.isSelected = false;
                                            material.quality = item.Value[0].pinZhi;
                                            material.isTouchControl = isMaxSave;
                                            material.materialEXP = ItemTemp.templates[i].effectId;
                                            CaiLiaoAdvance.Add(material);
                                            break;
                                        }
                                    }
                                
                                }
                            }
                        }
                }
            }
        }

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
            if (tempBuwei == Equip_BuWei && item.Value.pinZhi <= Equip_PinZhi)
            {
                EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                material.dbid = item.Value.dbId;
                material.materialId = item.Value.itemId;
                material.icon = ZhuangBei.GetIcon_ByEquipId(item.Value.itemId);
                material.count = item.Value.cnt.ToString();
                material.isSelected = false;
                material.quality = item.Value.pinZhi;
                material.materialEXP =  ZhuangBei.GetItemByID(item.Value.itemId).exp;
                CaiLiaoEquips.Add(material);

            }
        }
        QualitySort(CaiLiaoStrenth, true);
        QualitySort(CaiLiaoAdvance, false);
        QualitySort2(CaiLiaoEquips);
    }

    EquipGrowthMaterialUseManagerment.MaterialInfo mInfo = new EquipGrowthMaterialUseManagerment.MaterialInfo();

    void QualitySort(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list, bool isStrength)//按品质排序
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (list[j].quality > list[j + 1].quality)
                {
                    mInfo = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = mInfo;
                }
            }
        }

        if (isStrength)
        {
            //for (int i = 0; i < list.Count; i++)
            //{
            //    EquipGrowthMaterialUseManagerment.listMaterials.Add(list[i]);
            //}
            DestroyChild(list);
        }
        else
        {
            AssignmentBuWei();
        }
    }

    void QualitySort2(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//按品质排序
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (list[j].quality > list[j + 1].quality)
                {
                    mInfo = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = mInfo;
                }
            }
        }
       
        DestroyChild(list);
    }

    void AssignmentBuWei()//查询部位
    {
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            for (int j = 0; j < CaiLiaoAdvance.Count; j++)
            {
                if (int.Parse(ZhuangBei.templates[i].jinjieItem) == CaiLiaoAdvance[j].materialId)
                {
                    EquipGrowthMaterialUseManagerment.MaterialInfo materialBuwei = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                    materialBuwei = CaiLiaoAdvance[j];
                    materialBuwei.buwei = ZhuangBei.templates[i].buWei;
                    CaiLiaoAdvance[j] = materialBuwei;
                }
            }
        }
        SortBuWei();
    }

    void SortBuWei()//按部位排序
    {
        List<EquipGrowthMaterialUseManagerment.MaterialInfo> list = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
        int[] Sequence = { 1, 2, 3, 11, 12, 13, 14, 15, 16 };
        for (int i = 0; i < Sequence.Length; i++)
        {
            for (int j = 0; j < CaiLiaoAdvance.Count; j++)
            {
                if (CaiLiaoAdvance[j].buwei == Sequence[i])
                {
                    int tempBuwei = 0;
                    switch (Sequence[i])
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
                    list.Add(CaiLiaoAdvance[j]);
                }
            }
        }
        DestroyChild(list);
    }
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> listM = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    int CreateCount = 0;
    void DestroyChild(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//新创建子物体
    {
        CreateCount++;
        int sizeSend = list.Count;
        for (int i = 0; i < sizeSend; i++)
        {
            listM.Add(list[i]);
        }
 
        if (CreateCount == 3)
        {

            CreateCount = 0;
            if (listM.Count > 0)
            {
                for (int i = 0; i < listM.Count; i++)
                {
                    EquipGrowthMaterialUseManagerment.listMaterials.Add(listM[i]);
                }
 
                m_listGameObject[0].SetActive(true);
                m_listGameObject[1].SetActive(false);
                m_listGameObject[2].SetActive(false);
                WetherHaveMaterial = true;
                m_Material.SetActive(true);
                m_NoMaterial.SetActive(false);
                listEvent[0].gameObject.SetActive(true);
                listEvent[1].gameObject.SetActive(true);
                listEvent[2].gameObject.SetActive(false);
            }
            else if (!WetherHaveMaterial)
            {
                m_listGameObject[0].SetActive(false);
                if (Equip_BuWei == 3 || Equip_BuWei == 4 || Equip_BuWei == 5)
                {
                    m_listGameObject[1].SetActive(true);
                    m_listGameObject[2].SetActive(false);
                }
                else
                {
                    m_listGameObject[1].SetActive(false);
                    m_listGameObject[2].SetActive(true);
                }
                WetherHaveMaterial = true;
         
                listEvent[0].gameObject.SetActive(false);
                listEvent[2].gameObject.SetActive(true);
                
                m_Material.SetActive(true);
                m_NoMaterial.SetActive(true);
                for (int i = 0; i < 2; i++)
                {
                    listEvent[i].transform.GetComponent<Collider>().enabled = false;
                    listSprite[i].GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
                    listSprite[i].GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
                    listSprite[i].GetComponent<TweenColor>().enabled = true;
                }
            }
        }
    }

    void ShowMaterialInfo()
    {
        if (IconSampleGrid.transform.childCount == 0)
        {
 
            listInfoo.Clear();
            index_Num2 = 0;
            IsShowOn = false;
            for (int i = 0; i < 20; i++)
            {
                MaterialInfo mInfo = new MaterialInfo();
                mInfo.materialid = 0;
                mInfo.texture = "";
                mInfo.count = "";
                mInfo.pinzhi = 200;
                listInfoo.Add(mInfo);

            }
            int sizeall = listInfoo.Count;
            for (int i = 0; i < sizeall; i++)
            {
                CreateItems();
            }
        }
        else
        {
            ShowMaterialInfos(listM);
        }
    
    }
    struct MaterialInfo
    {
        public int materialid;
        public string texture;
        public string count;
        public int pinzhi;
    }
    MaterialInfo matInfo;
    private List<MaterialInfo> listInfoo = new List<MaterialInfo>();

    void CreateItems()
    {
          Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), IconSampleLoadCallBack);
    }
    int index_Num2 = 0;
    bool IsShowOn = false;
    public void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSampleGrid != null)
        {
             GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = IconSampleGrid.transform;

            iconSampleObject.transform.localPosition = Vector3.zero;
            iconSampleObject.transform.localScale = Vector3.one;

            var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            var equipGrowthMaterialItem = iconSampleObject.GetComponent<EquipGrowthMaterialItem>() ?? iconSampleObject.AddComponent<EquipGrowthMaterialItem>();
            equipGrowthMaterialItem.IconSampleManager = iconSampleManager;
            if (listInfoo[index_Num2].materialid != 0)
            {
                iconSampleObject.GetComponent<IconSampleManager>().SubButton.SetActive(false);
            }
            equipGrowthMaterialItem.ShowMaterialInfo(listInfoo[index_Num2].materialid, listInfoo[index_Num2].texture, listInfoo[index_Num2].count, isMaxSave, listInfoo[index_Num2].pinzhi, OverStepTip);

            if (index_Num2 < listInfoo.Count - 1)
            {
                index_Num2++;
            }
            else if (index_Num2 == listInfoo.Count - 1)
            {

                if (!IsShowOn)
                {
                    IsShowOn = true;

                    ShowMaterialInfos(listM);
                }
            }
            IconSampleGrid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }
    int Material_Num = 0;
    void ShowMaterialInfos(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)
    {
        int size = IconSampleGrid.transform.childCount;
        if (list.Count > 0)
        {
            if (size > list.Count)
            {
 
                for (int i = Material_Num; i < size; i++)
                {
                    if (i < list.Count)
                    {
                        Material_Num++;
                        IconSampleGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().m_IntenseShow = true;
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(EquipGrowthMaterialUseManagerment.listMaterials[i].materialId, EquipGrowthMaterialUseManagerment.listMaterials[i].icon, EquipGrowthMaterialUseManagerment.listMaterials[i].count, isMaxSave, EquipGrowthMaterialUseManagerment.listMaterials[i].quality, OverStepTip);
                    }
                    else 
                    {
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(0, "", "", isMaxSave, 200,null);
                    }
                }
            }
            else
            {
                for (int i = Material_Num; i < list.Count; i++)
                {
                    Material_Num++;
                    if (i < size)
                    {
                     
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().m_IntenseShow = true;
                        IconSampleGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(EquipGrowthMaterialUseManagerment.listMaterials[i].materialId, EquipGrowthMaterialUseManagerment.listMaterials[i].icon, EquipGrowthMaterialUseManagerment.listMaterials[i].count, isMaxSave, EquipGrowthMaterialUseManagerment.listMaterials[i].quality, OverStepTip);
                    }
                    else
                    {
  
                        MaterialInfo mInfo = new MaterialInfo();
                        mInfo.materialid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        mInfo.texture = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        mInfo.count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        mInfo.pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        listInfoo.Add(mInfo);
                    }
                }
                int size_add = listInfoo.Count;
                if (size_add - 1 > index_Num2)
                {
                    index_Num2++;
                    for (int i = index_Num2; i < size_add; i++)
                    {
                        if (i < 50)
                        {
                            CreateItems();
                        }
                    }
                }
            }
        }
        else
        {
        
            for (int i = 0; i < 20; i++)
            {
              IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(0, "", "", isMaxSave, 200, null);
            }
        }
    }

    void EquipUpGrade(long dbid, List<long> listUsedID)
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        EquipStrengthReq equip = new EquipStrengthReq();
        equip.equipId = dbid;
        equip.usedEquipIds = new List<long>();
        equip.usedEquipIds.Clear();
        for (int i = 0; i < listUsedID.Count; i++)
        {
            equip.usedEquipIds.Add(listUsedID[i]);
        }
        equip.type = 2;

        t_qx.Serialize(t_tream, equip);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_UPGRADE, ref t_protof);
    }

    public void WashBotton(bool ison, int index)
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

    void ProgressBarExhibition()
    {
        //int maxExpSum = currSave;
        int current = 0;
        for (int j = 0; j < EquipGrowthMaterialUseManagerment.listTouchedId.Count; j++)
        {
            for (int i = 0; i < EquipGrowthMaterialUseManagerment.listMaterials.Count; i++)
            {
                if (EquipGrowthMaterialUseManagerment.listMaterials[i].materialId == EquipGrowthMaterialUseManagerment.m_MaterialId)
                {
                    current = EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP;
                }
            }
        }
        CreateClone(current);
    }
    private int index_Num = 0;
    private int Addindex = 0;

    int curr_residue = 0;
    int curr_Max = 0;
    int addCount = 0;
    int ReduceIndex_Now = 0;

    bool IsResidueOn = false;
    int lastcontent = 0;
    int lastExpAll = 0;
    int currentLevel = 0;

    void CreateClone(int content)
    {
     
        int summ = 0;
        foreach (KeyValuePair<int, int> item in EquipGrowthMaterialUseManagerment.listCurrentAddExp)
        {
            summ += item.Value;
        }
 

        if (EquipGrowthMaterialUseManagerment.materialItemReduce)
        {
 
            EquipGrowthMaterialUseManagerment.materialItemReduce = false;
  
            EquipGrowthMaterialUseManagerment.m_TotalAddExp -= content;
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) >= JunZhuData.Instance().m_junzhuInfo.level)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;

            }
            else if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) < JunZhuData.Instance().m_junzhuInfo.level)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = true;
            }
     
            EquipGrowthMaterialUseManagerment.ReduceUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);
    
            ProcessReduce(content);
            CreateClone(m_LabProgress.gameObject, content * -1);
        }
        else
        {
            if (addCount == 0)
            {
                addCount++;
                curr_residue = currSave + content;
                curr_Max = maxSave;
                EquipGrowthMaterialUseManagerment.m_TotalAddExp = curr_residue;
            }
            else
            {
                EquipGrowthMaterialUseManagerment.m_TotalAddExp += content;
                curr_residue += content;
            }
 
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) >= JunZhuData.Instance().m_junzhuInfo.level)
            {
              
                EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = true;
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
                {
 
                    CreateMove(m_LabProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
                }
            }

            _levelAdd = Equip_Level;

            CreateClone(m_LabProgress.gameObject, content);
            ProcessAddEffect();
        }
    }


     int _levelAdd = 0;
    void ProcessAddEffect()
    {
        _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
        
        _levelReduce = 0;
        ExpXxmlTemp.GetUpgradeMaxLevel_ByExpidLevel(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.equipLevel, curr_residue, ZhuangBei.GetMaxLevelByEquipId(EquipSavedId));
       
        //if (EquipGrowthMaterialUseManagerment.m_IsSurpassLimited)
        //{
        //    curr_residue = EquipGrowthMaterialUseManagerment.listCurrentAddExp[EquipGrowthMaterialUseManagerment.CurrentExpIndex];
        //    lastcontent = curr_residue;
        //}
        //else
        //{
        //    lastcontent = curr_residue;
        //}
        lastcontent = curr_residue;

        int size = ExpXxmlTemp.m_listNeedInfo.Count;
        int sum_All = 0;
        int judgeIndex = 0;
        int fewardLevel = 0;
    
        for (int i = 0; i < size; i++)
        {
            //Debug.Log("LEVELLEVELLEVELLEVEL" + ExpXxmlTemp.m_listNeedInfo[size - 1].level);
            if (i < size - 1 && ExpXxmlTemp.m_listNeedInfo[i].needExp > 0)
            {
                sum_All += ExpXxmlTemp.m_listNeedInfo[i].needExp;

            }
            else
            {
                judgeIndex = i;
            }
   
            if (i == size - 1)
            {
                EquipGrowthMaterialUseManagerment.equipLevel = ExpXxmlTemp.m_listNeedInfo[i].level;
            }
        }
        lastExpAll = sum_All;
        if (curr_residue - sum_All >= 0)
        {
            curr_residue -= sum_All;
        }

        curr_Max = ExpXxmlTemp.m_listNeedInfo[size - 1].needExp;
        int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        if (EquipGrowthMaterialUseManagerment.equipLevel >_levelAdd)
        {
      
            m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel, _levelAdd).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel, _levelAdd).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel, _levelAdd).xueAdd);
            _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            //  Debug.Log("QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1).gongAdd :::" + QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1).gongAdd * -1);
        
        }
        else
        {
            _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
        }
        if (curr_Max < 0)
        {
            m_EquipExp.value = 1;
            m_LabProgress.text = "";

        }
        else
        {
            m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }


        //if (ExpXxmlTemp.m_listNeedInfo[judgeIndex].needExp < 0)
        //{
        //    curr_residue = lastcontent - lastExpAll;
        //    curr_Max = ExpXxmlTemp.m_listNeedInfo[ExpXxmlTemp.m_listNeedInfo.Count - 2].needExp;

        //    if (IsInvoking("ProgressBarController"))
        //    {
        //        CancelInvoke("ProgressBarController");
        //    }
        //    currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //    int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //    m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).xueAdd);


        //    m_EquipExp.value = 1;
        //    m_LabProgress.text = "";
        //}
        //else if (EquipGrowthMaterialUseManagerment.m_IsSurpassLimited)
        //{
        //    curr_residue = ExpXxmlTemp.m_listNeedInfo[ExpXxmlTemp.m_listNeedInfo.Count - 1].needExp;
        //    curr_Max = ExpXxmlTemp.m_listNeedInfo[ExpXxmlTemp.m_listNeedInfo.Count - 1].needExp;

        //    if (size <= 1)
        //    {
        //        currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //        int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //        m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //        m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //        m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //    }
        //    else
        //    {
        //        if (IsInvoking("ProgressBarController"))
        //        {
        //            CancelInvoke("ProgressBarController");
        //            index_Num = 0;
        //            Addindex = 0;
        //        }
        //            currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //            m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //            m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //        InvokeRepeating("ProgressBarController", 0.02f, 0.02f);
        //    }

        //}
        //else
        //{
        //    curr_residue -= sum_All;
        //    curr_Max = ExpXxmlTemp.m_listNeedInfo[ExpXxmlTemp.m_listNeedInfo.Count - 1].needExp;
        //    if (size <= 1)
        //    {
        //        currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //        int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //        m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //        m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //        m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //    }
        //    else
        //    {
        //        if (IsInvoking("ProgressBarController"))
        //        {

        //            CancelInvoke("ProgressBarController");
        //            currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //            m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).xueAdd);

        //            m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //            index_Num = 0;
        //            Addindex = 0;
        //        }
        //        InvokeRepeating("ProgressBarController", 0.02f, 0.02f);
        //    }

        //}
    }

    int _levelReduce = 0;
    void ProcessReduce(int content)
    {
        if (curr_residue >= content)
        {
            curr_residue -= content;
            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
            if (EquipGrowthMaterialUseManagerment.equipLevel < _levelReduce)
            {

                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1, _levelReduce).gongAdd * -1, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1, _levelReduce).fangAdd * -1, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1, _levelReduce).xueAdd * -1);
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
                m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
        else
        {
            ExpXxmlTemp.GetReduceMaxLevel_ByExpidLevel(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), content, EquipGrowthMaterialUseManagerment.equipLevel, EquipGrowthMaterialUseManagerment.Levelsaved);

            int size = ExpXxmlTemp.m_listReduceInfo.Count;

           // Debug.Log("sizesizesizesize :: " + ExpXxmlTemp.m_listReduceInfo.Count);
            int reduceIndex = 0;
            int sum_Reduce = curr_residue;
            for (int i = 0; i < size; i++)
            {
              //  Debug.Log("LVLVLVLVLVLVLV :: " + ExpXxmlTemp.m_listReduceInfo[i].needExp);
             //   Debug.Log("ExpXxmlTemp.m_listReduceInfo[i].needExp :: " + ExpXxmlTemp.m_listReduceInfo[i].needExp);
                sum_Reduce += ExpXxmlTemp.m_listReduceInfo[i].needExp;
                if (sum_Reduce >= content)
                {
                    reduceIndex = i;
                    break;
                }
            }
           // Debug.Log("sum_Reducesum_Reducesum_Reduce :: " + sum_Reduce);
            curr_residue = sum_Reduce - content;
         //   Debug.Log("reduceIndexreduceIndexreduceIndex :: " + reduceIndex);
            curr_Max = ExpXxmlTemp.m_listReduceInfo[reduceIndex].needExp;
            int level = EquipGrowthMaterialUseManagerment.equipLevel;
            EquipGrowthMaterialUseManagerment.equipLevel = ExpXxmlTemp.m_listReduceInfo[reduceIndex].level;
            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));

            if (EquipGrowthMaterialUseManagerment.equipLevel < _levelReduce)
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;

                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).gongAdd * -1, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).fangAdd * -1, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).xueAdd * -1);
             
                //Debug.Log("QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1).gongAdd * -1" + QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1).gongAdd * -1);
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
            m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }

       


        //if (curr_Max < 0)
        //{
        //    m_EquipExp.value = 1;
        //    m_LabProgress.text = "";

        //}
        //else



        //    if (size > 0)
        //    {
        //        EquipGrowthMaterialUseManagerment.equipLevel = ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Just].level;
        //        curr_Max = ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Just].needExp;
        //    }

        //    if (EquipGrowthMaterialUseManagerment.m_InfectAddExp - content < 0)
        //    {
        //        curr_residue = sum_Reduce - EquipGrowthMaterialUseManagerment.listCurrentAddExp[EquipGrowthMaterialUseManagerment.CurrentExpIndex];
        //    }
        //    else
        //    {
        //        curr_residue = sum_Reduce - content; 
        //    }

        //    if (EquipGrowthMaterialUseManagerment.listCurrentAddExp.ContainsKey(EquipGrowthMaterialUseManagerment.CurrentExpIndex))
        //    {
        //        EquipGrowthMaterialUseManagerment.listCurrentAddExp.Remove(EquipGrowthMaterialUseManagerment.CurrentExpIndex);
        //        EquipGrowthMaterialUseManagerment.CurrentExpIndex--;
        //    }

        //    if (ReduceIndex_Just - ReduceIndex_Now > 0)
        //    {

        //        IsResidueOn = true;
        //        if (IsInvoking("ProgressBarReduce"))
        //        {
        //            CancelInvoke("ProgressBarReduce");

        //            m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //            m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //        }

        //        Addindex = ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp;
        //        InvokeRepeating("ProgressBarReduce", 0.1f, 0.1f);
        //    }
        //    else
        //    {
        //        currentLevel = EquipGrowthMaterialUseManagerment.equipLevel;
        //        int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //        m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //        m_EquipExp.value = curr_residue / float.Parse(curr_Max.ToString());
        //        m_LabProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        //    }
        // }
    }

    //IEnumerator WatiFor(GameObject obj)
    //{
    //    yield return new WaitForSeconds(0.8f);
    //    Destroy(obj);
    //}
    // float processValue = 0;

    //void ProgressBarController()
    //{
    //    m_EquipExp.value += 0.1f;

    //    Addindex += ExpXxmlTemp.m_listNeedInfo[index_Num].needExp / 10;
    //    if (m_EquipExp.value == 1)
    //    {

    //        m_LabProgress.text = Addindex.ToString() + "/" + ExpXxmlTemp.m_listNeedInfo[index_Num].needExp.ToString();
    //        Addindex = 0;
    //        CancelInvoke("ProgressBarController");

    //        if (index_Num < ExpXxmlTemp.m_listNeedInfo.Count - 1)
    //        {
    //            currentLevel = ExpXxmlTemp.m_listNeedInfo[index_Num].level;
    //            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
    //            if (currentLevel > _levelAdd)
    //            {
    //                _levelAdd = currentLevel;
    //                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).xueAdd);
    //            }

    //            StartCoroutine(WaitFor());
    //        }
    //        else
    //        {
    //            if (ExpXxmlTemp.m_listNeedInfo[index_Num].needExp == -1)
    //            {
    //                curr_residue = lastcontent - lastExpAll;
    //                curr_Max = ExpXxmlTemp.m_listNeedInfo[ExpXxmlTemp.m_listNeedInfo.Count - 2].needExp;
    //                if (IsInvoking("ProgressBarController"))
    //                {
    //                    CancelInvoke("ProgressBarController");
    //                }
    //                currentLevel = ExpXxmlTemp.m_listNeedInfo[index_Num].level;
    //                int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
    //                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).xueAdd);

    //                m_EquipExp.value = 1.0f;
    //                m_LabProgress.text = "";
    //            }
    //            else
    //            {
    //                currentLevel = ExpXxmlTemp.m_listNeedInfo[index_Num].level;

    //                int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
    //                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, currentLevel).xueAdd);

    //                m_EquipExp.value = curr_residue / float.Parse(ExpXxmlTemp.m_listNeedInfo[index_Num].needExp.ToString());
    //                m_LabProgress.text = curr_residue.ToString() + "/" + ExpXxmlTemp.m_listNeedInfo[index_Num].needExp.ToString();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        m_LabProgress.text = Addindex.ToString() + "/" + ExpXxmlTemp.m_listNeedInfo[index_Num].needExp.ToString();
    //    }
    //}

    //IEnumerator WaitFor()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    if (IsResidueOn)
    //    {
    //        Back_Continue();
    //    }
    //    else
    //    {
    //        Continue();
    //    }
    //}

    //void Continue()
    //{
    //    index_Num++;
    //    m_EquipExp.value = 0;
    //    InvokeRepeating("ProgressBarController", 0.02f, 0.02f);
    //}




    void ProgressBarReduce()
    {
        //m_EquipExp.value -= 0.1f;

        //Addindex -= ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp / 10;
        //if (m_EquipExp.value == 0)
        //{
        //    m_LabProgress.text = Addindex.ToString() + "/" + ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp;
        //    CancelInvoke("ProgressBarReduce");
        //    currentLevel--;
        //    int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //    m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //    if (ReduceIndex_Now < ReduceIndex_Just)
        //    {
        //        StartCoroutine(WaitFor());
        //    }
        //    else
        //    {
        //        EquipGrowthMaterialUseManagerment.touchIsEnable = true;
        //        IsResidueOn = false;
        //        CancelInvoke("ProgressBarReduce");
        //        m_EquipExp.value = curr_residue / float.Parse(ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp.ToString());
        //        m_LabProgress.text = curr_residue.ToString() + "/" + ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp.ToString();

        //        int QiangHuaId2 = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));
        //        m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId2,currentLevel).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId,currentLevel).xueAdd);

        //    }
        //}
        ////else
        //{
        //    m_LabProgress.text = Addindex.ToString() + "/" + ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp.ToString();
        //}
    }

    void Back_Continue()
    {
        ReduceIndex_Now++;
        Addindex = ExpXxmlTemp.m_listReduceInfo[ReduceIndex_Now].needExp;
        m_EquipExp.value = 1;
        InvokeRepeating("ProgressBarReduce", 0.02f, 0.02f);
    }

    void OnDisable()
    {
        if (IsInvoking("ProgressBarController"))
        {
            CancelInvoke("ProgressBarController");
        }

        if (IsInvoking("ProgressBarReduce"))
        {
            CancelInvoke("ProgressBarReduce");
        }
    }
    void OverStepTip(int index)
    {
        if (index > 0)
        {
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
            {

                CreateMove(m_LabProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
            }
            else if (EquipGrowthMaterialUseManagerment.Levelsaved >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
            {
                //Debug.Log("EquipGrowthMaterialUseManagerment.Levelsaved ::" + EquipGrowthMaterialUseManagerment.Levelsaved);
                //Debug.Log("ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv ::" + ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv);

                CreateMove(m_LabProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
            }
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.LABEL_EFFECT), UIBoxLoadCallback_OverStep);
            }
        }
        else
        {
          //  Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
            CreateMove(m_LabProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
        }
    }

    public void UIBoxLoadCallback_OverStep(ref WWW p_www, string p_path, Object p_object)
    {
        if (!CityGlobalData.m_isCreatedMoveLab)
        {
            CityGlobalData.m_isCreatedMoveLab = true;
            GameObject boxObj = Instantiate(p_object) as GameObject;
            boxObj.GetComponent<MoveCloneManangement>().CreateMove(null, LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_OVERSTEP_LEVEL));
        }
        else
        {
            p_object = null;
        }
       
    }

    public void UIBoxLoad_YiJianQiangHua(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_CONTENT);
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_CONTENT2);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, cancelStr, confirmStr, YiJianQiangHuaTag, null, null);

    }

    void YiJianQiangHuaTag(int index)
    {
        if (index == 2)
        {
            //if (Equip_Level < JunZhuData.Instance().m_junzhuInfo.level)
            //{
            //    if (Equip_BuWei == 3 || Equip_BuWei == 4 || Equip_BuWei == 5)
            //    {
            //        EquipGrowthMaterialUseManagerment.QuicklyStrengthen(EquipSavedId,currSave, maxSave, 1);
            //    }
            //    else
            //    {
            //        EquipGrowthMaterialUseManagerment.QuicklyStrengthen(EquipSavedId,currSave, maxSave, 0);

            //    }
            //    //Debug.Log("EquipGrowthMaterialUseManagerment.listTouchedId.Count" + EquipGrowthMaterialUseManagerment.listTouchedId.Count);
            //    //for (int i = 0; i < EquipGrowthMaterialUseManagerment.listTouchedId.Count; i++)
            //    //{
            //    //    Debug.Log(" EquipGrowthMaterialUseManagerment.listTouchedId :::" + EquipGrowthMaterialUseManagerment.listTouchedId[i]);
            //    //}
            //    EquipGrowthEquipInfoManagerment.m_WetherIsIntensify = true;
            //        EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);

            //        m_IntensifyTag.SetActive(false);
            //}
            //else
            //{


            //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallback_OverStep);
            //}
            EquipSuoData.m_equipsLevelSave.Clear();
            Dictionary<int, BagItem> listtemp = EquipsOfBody.Instance().m_equipsOfBodyDic;
            foreach (KeyValuePair<int, BagItem> item in listtemp)
            {
                EquipSuoData.m_equipsLevelSave.Add(item.Value.buWei, item.Value.qiangHuaLv);
            }
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_UPALLGRADE);
        }
    }

    public void UIBoxLoad_ExpOverStep(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str = "\n" + LanguageTemplate.GetText(LanguageTemplate.Text.EQUIPEXP_OVERSTEP);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, EXPOverStep, null, null);
    }

    void EXPOverStep(int index)
    {
        
    }

    public  void ShowEquipTanHao()
    {
        int _size = m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent.Count;
        for (int i = 0; i < _size; i++)
        {
 
            if (AllIntensify(i) == i)
            {
                m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(true);
            }
            else
            {
                m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(false);
            }
        }
    }


     private int AllIntensify(int index)
     {

         int EquipExp = 0;
         int equipLevel = 0;
         int pinzhi = 0;
         if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
         {
             equipLevel = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaLv;
             EquipExp = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaExp;
             pinzhi = EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi;
             if (EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 3 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 4 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 5)
             {
                 EquipType = 1;
             }
             else
             {
                 EquipType = 0;
             }

             foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
             {
                 for (int i = 0; i < ItemTemp.templates.Count; i++)
                 {
                     if (item.Value[0].itemId == ItemTemp.templates[i].id)
                     {
                         if (EquipType == 0 && item.Value[0].itemType == 2)
                         {
                             EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                             //   Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                         }
                         else if (EquipType == 1 && item.Value[0].itemType == 1)
                         {
                             EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                             //   Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                         }
                         else if (item.Value[0].itemType == 6 && pinzhi > item.Value[0].pinZhi)
                         {

                             int tempBuwei = 0;
                             switch (ZhuangBei.GetBuWeiByItemId(item.Value[0].itemId))
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
                             if (EquipType == 1)
                             {
                                 if (tempBuwei == 3 || tempBuwei == 4 || tempBuwei == 5)
                                 {
                                     if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                     {
                                         EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                         //   Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                                     }
                                 }
                             }
                             else
                             {
                                 if (tempBuwei != 3 && tempBuwei != 4 && tempBuwei != 5)
                                 {
                                     if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                     {
                                         EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                         // Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                                     }
                                 }

                             }

                         }
                     }
                 }
             }

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
                 if (tempBuwei == EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei && item.Value.pinZhi <= EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi)
                 {
                     //  Debug.Log("EquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExpEquipExp ::" + EquipExp);
                     EquipExp += ZhuangBei.GetItemByID(item.Value.itemId).exp;
                 }
             }

             if (ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp > 0)
             {
                 if (EquipExp >= ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp)
                 {
                    if (equipLevel < JunZhuData.Instance().m_junzhuInfo.level)
                    {
                        return EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei;
                    }
                    else
                    {
                        return 99;
                    }
                }
             }
         }
         return 99;
     }

    void CreateClone(GameObject move, int content)
    {
        GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
        clone.transform.localPosition = move.transform.localPosition;
        clone.transform.localRotation = move.transform.localRotation;
        clone.transform.localScale = move.transform.localScale;
        clone.GetComponent<UILabel>().text = "";
        if (content < 0)
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(5, content.ToString());
        }
        else
        {
            clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+" + content.ToString());
        }

        clone.AddComponent(typeof(TweenPosition));
        clone.AddComponent(typeof(TweenAlpha));
        clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiForee(clone));
    }

    IEnumerator WatiForee(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }


    public void UIBoxLoadGouMai(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        if (FunctionOpenTemp.GetWhetherContainID(9))
        {
            string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
            string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
            string str = "\n" + LanguageTemplate.GetText(LanguageTemplate.Text.BUY_MATERIAL);

            // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
            uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, GuoMai, null, null);
        }
        else
        {
            string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
            string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
            string str ="商店未开启";
            uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, null, null, null);
        }

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
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiFor(clone));
    }
    IEnumerator WatiFor(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
    }
    void  GuoMai(int index)
    {
        if (index == 2)
        {
            MainCityUI.TryRemoveFromObjectList(m_MainParent);

            //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_PAWNSHOP),
            //                         RB_UB_Button3_LoadCallback);
            NpcManager.m_NpcManager.setGoToNpc(6);
            Destroy(m_MainParent);
        }

    }

   void Strengthen(int index)// 强化升一级ID统计
    {
        int EquipExp = 0;
        int equipLevel = 0;
        int pinzhi = 0;
        int EquipType = 0;
        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
        {
            equipLevel = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaLv;
            EquipExp = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaExp;
            pinzhi = EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi;
            if (EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 3 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 4 || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 5)
            {
                EquipType = 1;
            }
            else
            {
                EquipType = 0;
            }

            foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
            {
                for (int i = 0; i < ItemTemp.templates.Count; i++)
                {
                    if (item.Value[0].itemId == ItemTemp.templates[i].id)
                    {
                        if (EquipType == 0 && item.Value[0].itemType == 2)
                        {
                            EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                            EquipGrowthMaterialUseManagerment.listTouchedId.Add(item.Value[0].dbId);
                        }
                        else if (EquipType == 1 && item.Value[0].itemType == 1)
                        {
                            EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                            EquipGrowthMaterialUseManagerment.listTouchedId.Add(item.Value[0].dbId);
                        }
                        else if (item.Value[0].itemType == 6 && pinzhi > item.Value[0].pinZhi)
                        {

                            int tempBuwei = 0;
                            switch (ZhuangBei.GetBuWeiByItemId(item.Value[0].itemId))
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
                            if (EquipType == 1)
                            {
                                if (tempBuwei == 3 || tempBuwei == 4 || tempBuwei == 5)
                                {
                                    if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                    {
                                        EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                        EquipGrowthMaterialUseManagerment.listTouchedId.Add(item.Value[0].dbId);
                                    }
                                }
                            }
                            else
                            {
                                if (tempBuwei != 3 && tempBuwei != 4 && tempBuwei != 5)
                                {
                                    if (tempBuwei == index && EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi > item.Value[0].pinZhi)
                                    {
                                        EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                                        EquipGrowthMaterialUseManagerment.listTouchedId.Add(item.Value[0].dbId);
                                    }
                                }

                            }

                        }
                    }
                }
            }

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
                if (tempBuwei == EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei && item.Value.pinZhi <= EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi)
                {
                    EquipExp += ZhuangBei.GetItemByID(item.Value.itemId).exp;
                    EquipGrowthMaterialUseManagerment.listTouchedId.Add(item.Value.dbId);
                }
            }

            if (ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp > 0)
            {
                if (EquipExp < ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp)
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallback);
                }
            }
        }

    }
    void UIBoxLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
 
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.LACK_OF_QIANGHUACL);
        string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox("", MyColorData.getColorString(1, str), null, null, concelr, confirmStr, ConfirmStrength, null, null, null);
    }

    private  void ConfirmStrength(int index)
    {
        if (index == 2)
        {
            EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);
        }
        else
        {
            EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        }
    }
}