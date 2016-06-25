using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipGrowthIntensifyManagerment : MonoBehaviour, UI2DEventListener
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
    public EquipGrowthEquipInfoManagerment m_EquipGrowthEquipInfoManagerment;
    public List<GameObject> m_listEquipTaHao;
    public UISprite m_SpriteV;
    public UILabel m_LabV;
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
    //public GameObject m_SignalTag2;
    private int Equip_BuWei = 0;
    private int Equip_PinZhi = 0;
    private int Equip_Level = 0;
    public enum TouchType
    {
        BUTTON_NONE ,
        BUTTON_UP
    }
    public TouchType m_YJQHTouch;
  
    void Start()
    {
        m_SpriteV.spriteName = "v" + VipFuncOpenTemplate.GetNeedLevelByKey(6);
        m_YJQHTouch = TouchType.BUTTON_UP;
        if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
        {
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100110;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        listEvent.ForEach(p => p.m_click_handler += IntensifyTouch);
      //  listTagEvent.ForEach(p => p.m_handler += TagConfirm);
      
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
 
        if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress >= 0)
        {
            m_EquipGrowthEquipInfoManagerment.m_ScrollView.enabled = false;
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100040;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 4;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        else if (FreshGuide.Instance().IsActive(100705) && TaskData.Instance.m_TaskInfoDic[100705].progress >= 0)
        {
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100705;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
        {
            FunctionWindowsCreateManagerment.SetSelectEquipInfo(1, 3);
            TaskData.Instance.m_iCurMissionIndex = 100110;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        
    }
    void Update()
    {
        if (EquipGrowthMaterialUseManagerment.materialItemTouched)
        {
            EquipGrowthMaterialUseManagerment.materialItemTouched = false;
            ProgressBarExhibition();
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
          //  m_SignalTag2.SetActive(false);
            EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);
        }
        else if (obj.name.Equals("ButtonCancel2"))
        {
        //    m_SignalTag2.SetActive(false);
        }
        else
        {
            m_IntensifyTag.SetActive(false);
        }
    }

    void IntensifyTouch(GameObject obj)//强化界面按钮控制
    {

        if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100040;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 7;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        else if (FreshGuide.Instance().IsActive(100125) && TaskData.Instance.m_TaskInfoDic[100125].progress >= 0)
        {

            UIYindao.m_UIYindao.CloseUI();
        }


        //  if (obj.name.Equals("ButtondQiangHua") && EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0)
        if (obj.name.Equals("ButtondQiangHua"))
        {
            if (Equip_Level >= ZhuangBei.getZhuangBeiById(EquipSavedId).qianghuaMaxLv)
            {
                ClientMain.m_UITextManager.createText("当前装备已强化至最大等级");
            }
            else if (Equip_Level >= JunZhuData.Instance().m_junzhuInfo.level)
            {
                ClientMain.m_UITextManager.createText("当前装备已强化至君主当前等级");
            }
            else if (EquipGrowthMaterialUseManagerment.listMaterials.Count == 0)
            {
                Global.CreateFunctionIcon(1301);
            }
            else if (CurrentEquipOverPlayerLeveMax(Equip_BuWei))
            {
                ClientMain.m_UITextManager.createText("当前装备已强化至君主当前等级");
            }
            else
            {

                if (EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0)
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
                if (EquipsAllLevelMax())
                {
                    ClientMain.m_UITextManager.createText("所有装备已强化至最大等级");
                }
                else if (!WetherContainMaterialsWuQi() && !WetherContainMaterialsFangJu())
                {
                    Global.CreateFunctionIcon(1301);
                }
               
                else if (!EquipLevelOverJunZhuWuQi() && !EquipLevelOverJunZhuFangJu())
                {
                    ClientMain.m_UITextManager.createText("所有装备已强化至君主当前等级");
                }
                else
                {
                    YiJianQiangHuaTag();
                    // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad_YiJianQiangHua);
                }
            }
            else
            {
                Global.CreateFunctionIcon(1901);
                //string str = "\n" + LanguageTemplate.GetText(600) + "\n" + VipFuncOpenTemplate.GetNeedLevelByKey(6).ToString()
                //+ LanguageTemplate.GetText(602);
                //// + "\n\n" + LanguageTemplate.GetText(700);
                //EquipSuoData.ShowSignal(null, str);

            }
        }
        else if (obj.name.Equals("ButtondGM"))
        {
            Global.CreateFunctionIcon(1301);
            //if (FunctionOpenTemp.GetWhetherContainID(9))
            {
                //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadGouMai);

            }
        }
    }

    public void RB_UB_Button3_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }
     
    public void OnUI2DShow()
    {
      

        MaterialsInfoTidy();
    }
    public void ShowInfo(EquipStrengthResp equipinfo)//界面信息显示
    {
        curr_residue = 0;
        curr_Max = 0;
        addCount = 0;
    
        Equip_Level = equipinfo.level;
        Material_Num = 0;
  
        Equip_PinZhi = ZhuangBei.getZhuangBeiById(equipinfo.zhuangbeiID).pinZhi;
        EquipGrowthMaterialUseManagerment.listCurrentAddExp.Clear();
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        EquipSavedId = equipinfo.zhuangbeiID;
        EquipGrowthMaterialUseManagerment.m_EuipId = equipinfo.zhuangbeiID;
        currSave = equipinfo.exp;
        maxSave = equipinfo.expMax;
        isMaxSave = equipinfo.exp < equipinfo.expMax ? true : false;
        savedbid = equipinfo.equipId;
        //  WashBotton(YiJianQiangHuaShow(), 1);
        //if (level >= JunZhuData.Instance().m_junzhuInfo.level)
        //{

        //    WashBotton(false, 0,1);
        //}
        //else if (max != -1 && level < JunZhuData.Instance().m_junzhuInfo.level)
        //{
        //    WashBotton(true, 0);
        //}
        //else 
        Equip_BuWei = FunctionWindowsCreateManagerment.GetNeedLocation(ZhuangBei.getZhuangBeiById(equipinfo.zhuangbeiID).buWei);
        if (equipinfo.expMax == -1)
        {
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = 1.0f;
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = equipinfo.exp + "/0";
            //  WashBotton(false, 0);
        }
        else
        {
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = float.Parse(equipinfo.exp.ToString()) / equipinfo.expMax;
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = equipinfo.exp +"/" + equipinfo.expMax;
        }
        if (Equip_BuWei == 3 || Equip_BuWei == 4 || Equip_BuWei == 5)
        {
            EquipType = 1;
        }
        else
        {
            EquipType = 0;
        }




        MaterialsInfoTidy();
      //  MaterialsInfoTidy();
    }
 
     void MaterialsInfoTidy()//材料信息整理
    {
        BagCaiLiao = BagData.Instance().m_playerCaiLiaoDic;
 
        if (Equip_BuWei == 3 || Equip_BuWei == 4 || Equip_BuWei == 5)
        {
            EquipType = 1;
        }
        else
        {
            EquipType = 0;
        }
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        CaiLiaoStrenth.Clear();
   
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

                }
            }
        }
        ShowEquipTanHao();
        QualitySort(CaiLiaoStrenth);
    }

    EquipGrowthMaterialUseManagerment.MaterialInfo mInfo = new EquipGrowthMaterialUseManagerment.MaterialInfo();

    void QualitySort(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//按品质排序
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
 
   

    void DestroyChild(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//新创建子物体
    {
        int sizeSend = list.Count;
   
 
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                EquipGrowthMaterialUseManagerment.listMaterials.Add(list[i]);
            }
           
            m_Material.SetActive(true);
            m_NoMaterial.SetActive(false);
        }
        else
        {
            m_listGameObject[0].SetActive(false);
            if (Equip_BuWei == 3 || Equip_BuWei == 4 || Equip_BuWei == 5)
            {
                m_listGameObject[2].SetActive(false);
            }
            else
            {
                m_listGameObject[1].SetActive(false);
            } 
            m_Material.SetActive(true);
            m_NoMaterial.SetActive(true);
  
        }

        ShowMaterialInfo();
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
            ShowMaterialInfos(EquipGrowthMaterialUseManagerment.listMaterials);
        }
    
    }
    struct MaterialInfo
    {
        public int materialid;
        public string texture;
        public string count;
        public int pinzhi;
        public int materialExp;
        public long dbid;
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
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            EquipGrowthMaterialItem equipGrowthMaterialItem = iconSampleObject.GetComponent<EquipGrowthMaterialItem>() 
                ?? iconSampleObject.AddComponent<EquipGrowthMaterialItem>();
            equipGrowthMaterialItem.IconSampleManager = iconSampleManager;
            if (listInfoo[index_Num2].materialid != 0)
            {
                iconSampleObject.GetComponent<IconSampleManager>().SubButton.SetActive(false);
            }
            EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
            minfo._itemid = listInfoo[index_Num2].materialid;
            minfo._dbid = listInfoo[index_Num2].dbid;
            minfo._icon = listInfoo[index_Num2].texture;
            minfo._count = listInfoo[index_Num2].count;
            minfo._pinzhi = listInfoo[index_Num2].pinzhi;
            equipGrowthMaterialItem.ShowMaterialInfo(minfo, isMaxSave, OverStepTip);
          
            if (index_Num2 < listInfoo.Count - 1)
            {
                index_Num2++;
            }
            else if (index_Num2 == listInfoo.Count - 1)
            {

                if (!IsShowOn)
                {
                    IsShowOn = true;

                    ShowMaterialInfos(EquipGrowthMaterialUseManagerment.listMaterials);
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

                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        minfo._dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        minfo._icon = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        minfo._count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        minfo._pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, OverStepTip);
                    }
                    else 
                    {
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = 0;
                        minfo._dbid = 0;
                        minfo._icon = "";
                        minfo._count = "";
                        minfo._pinzhi = 200;
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
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
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        minfo._dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        minfo._icon = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        minfo._count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        minfo._pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, OverStepTip);
                    }
                    else
                    {
                        MaterialInfo mInfo = new MaterialInfo();
                        mInfo.materialid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        mInfo.dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        mInfo.texture = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        mInfo.count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        mInfo.pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        mInfo.materialExp = EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP;
                     
                        listInfoo.Add(mInfo);
                    }
                }
                int size_add = listInfoo.Count;
                if (size_add - 1 > index_Num2)
                {
                    index_Num2++;
                    for (int i = index_Num2; i < size_add; i++)
                    {
                            CreateItems();
                    }
                }
            }
        }
        else
        {
            int s_child = IconSampleGrid.transform.childCount;
            if (s_child > 20)
            {
                for (int i = 0; i < s_child; i++)
                {
                    if (i < 20)
                    {
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = 0;
                        minfo._dbid = 0;
                        minfo._icon = "";
                        minfo._count = "";
                        minfo._pinzhi = 200;
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
                    }
                    else
                    {
                        Destroy(IconSampleGrid.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                    minfo._itemid = 0;
                    minfo._dbid = 0;
                    minfo._icon = "";
                    minfo._count = "";
                    minfo._pinzhi = 200;
                    IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
                }
            }
        }
    }

    void EquipUpGrade(long dbid, List<long> listUsedID)
    {
        m_EquipGrowthEquipInfoManagerment.m_WetherIsIntensify = true;
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        EquipStrengthReq equip = new EquipStrengthReq();
        equip.equipId = dbid;
        equip.usedEquipIds = listUsedID;
        equip.type = 2;
        t_qx.Serialize(t_tream, equip);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_UPGRADE, ref t_protof);
    }

    public void WashBotton(bool ison, int index,int type = 0)
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
            if (type == 0)
            {
                listButton[index].transform.GetComponent<Collider>().enabled = ison;
            }
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            listButton[index].transform.FindChild("Background").GetComponent<TweenColor>().enabled = true;
        }
    }

    void ProgressBarExhibition()
    {
        int current = 0;
        for (int j = 0; j < EquipGrowthMaterialUseManagerment.listTouchedId.Count; j++)
        {
            for (int i = 0; i < EquipGrowthMaterialUseManagerment.listMaterials.Count; i++)
            {
                if (EquipGrowthMaterialUseManagerment.listMaterials[i].materialId == EquipGrowthMaterialUseManagerment.m_MaterialId._itemid
                    && EquipGrowthMaterialUseManagerment.listMaterials[i].dbid == EquipGrowthMaterialUseManagerment.m_MaterialId._dbid)
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
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId)
                , EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) 
                >= JunZhuData.Instance().m_junzhuInfo.level)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;

            }
            else if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId)
                , EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved)
                < JunZhuData.Instance().m_junzhuInfo.level)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = true;
            }
     
            EquipGrowthMaterialUseManagerment.ReduceUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);
    
            ProcessReduce(content);
            CreateClone(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject, content * -1);
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
 
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId)
                , EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved) 
                >= JunZhuData.Instance().m_junzhuInfo.level)
            {
                EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = true;
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId)
                    , EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved)
                    >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
                {
 
                    CreateMove(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject
                        , "当前装备已强化至最大等级",1);
                }
            }

            _levelAdd = Equip_Level;

            CreateClone(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject, content);
            ProcessAddEffect();
        }
    }


     int _levelAdd = 0;
    void ProcessAddEffect()
    {
        _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
        
        _levelReduce = 0;
        ExpXxmlTemp.GetUpgradeMaxLevel_ByExpidLevel(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), 
            EquipGrowthMaterialUseManagerment.equipLevel, curr_residue, ZhuangBei.GetMaxLevelByEquipId(EquipSavedId));
        lastcontent = curr_residue;

        int size = ExpXxmlTemp.m_listNeedInfo.Count;
        int sum_All = 0;
        int judgeIndex = 0;
        int fewardLevel = 0;
        for (int i = 0; i < size; i++)
        {
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
      
            m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel
                , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel
                , _levelAdd).gongAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel
                , _levelAdd).fangAdd, QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel
                , _levelAdd).xueAdd);
            _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
        
        }
        else
        {
            _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
        }
        if (curr_Max < 0)
        {
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = 1;
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = curr_residue + "/0";

        }
        else
        {
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
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

                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId
                    , EquipGrowthMaterialUseManagerment.equipLevel + 1, _levelReduce).gongAdd * -1
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId
                    , EquipGrowthMaterialUseManagerment.equipLevel + 1, _levelReduce).fangAdd * -1
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, EquipGrowthMaterialUseManagerment.equipLevel + 1
                    , _levelReduce).xueAdd * -1);
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
        else
        {
            ExpXxmlTemp.GetReduceMaxLevel_ByExpidLevel(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId)
                , content, EquipGrowthMaterialUseManagerment.equipLevel, EquipGrowthMaterialUseManagerment.Levelsaved);

            int size = ExpXxmlTemp.m_listReduceInfo.Count;
            int reduceIndex = 0;
            int sum_Reduce = curr_residue;
            for (int i = 0; i < size; i++)
            {
                sum_Reduce += ExpXxmlTemp.m_listReduceInfo[i].needExp;
                if (sum_Reduce >= content)
                {
                    reduceIndex = i;
                    break;
                }
            }
            curr_residue = sum_Reduce - content;
            curr_Max = ExpXxmlTemp.m_listReduceInfo[reduceIndex].needExp;
            int level = EquipGrowthMaterialUseManagerment.equipLevel;
            EquipGrowthMaterialUseManagerment.equipLevel = ExpXxmlTemp.m_listReduceInfo[reduceIndex].level;
            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));

            if (EquipGrowthMaterialUseManagerment.equipLevel < _levelReduce)
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;

                m_EquipGrowthEquipInfoManagerment.ShowLevel(EquipGrowthMaterialUseManagerment.equipLevel
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).gongAdd * -1
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).fangAdd * -1
                    , QiangHuaTemplate.GetAppendAttributeAddInfo(QiangHuaId, level, _levelReduce).xueAdd * -1);
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.equipLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.equipLevel;
            }
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
    }

  

    void OnDisable()
    {
  
    }
    void OverStepTip(int index)
    {
        if (index > 0)
        {
            if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(EquipSavedId), 
                EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.Levelsaved)
                >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
            {

                CreateMove(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject
                          , "当前装备已强化至最大等级",1);
            }
            else if (EquipGrowthMaterialUseManagerment.Levelsaved >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
            {
                CreateMove(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject
                    , "当前装备已强化至最大等级", 1);
            }
            else
            {
                ClientMain.m_UITextManager.createText("当前装备已强化至君主当前等级");
            }
        }
        else
        {
            if (EquipGrowthMaterialUseManagerment.Levelsaved >= ZhuangBei.GetItemByID(EquipSavedId).qianghuaMaxLv)
            {
                ClientMain.m_UITextManager.createText("当前装备已强化至最大等级");
            }
            else
            {
                CreateMove(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject
                  , "当前装备已强化至君主当前等级", 1);
            }
           
        }
    }
 

  
    //public void UIBoxLoad_YiJianQiangHua(ref WWW p_www, string p_path, Object p_object)
    //{
    //    GameObject obj = Instantiate(p_object) as GameObject;
    //    UIBox uibox = obj.GetComponent<UIBox>();
    //    string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_TITLE);
    //    string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
    //    string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
    //    string str = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_CONTENT);
    //    string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.YIJIQIANGHUA_CONTENT2);
    //    uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), MyColorData.getColorString(1, str2), null, cancelStr, confirmStr, YiJianQiangHuaTag, null, null);

    //}

    void YiJianQiangHuaTag( )
    {
  
        {
            EquipSuoData.m_equipsLevelSave.Clear();
            Dictionary<int, BagItem> listtemp = EquipsOfBody.Instance().m_equipsOfBodyDic;
            foreach (KeyValuePair<int, BagItem> item in listtemp)
            {
                EquipSuoData.m_equipsLevelSave.Add(item.Value.buWei, item.Value.qiangHuaLv);
            }
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_UPALLGRADE);
        }
      
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
    private bool  YiJianQiangHuaShow()
    {
        int _size = m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent.Count;
        for (int i = 0; i < _size; i++)
        {
            if (AllIntensify(i) == i)
            {
             return true;
            }
        }
        return false;
    }
 
    private bool CurrentEquipOverPlayerLeveMax(int buwei)
    {
        if( EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(buwei))
        {
            if (EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].qiangHuaLv >= JunZhuData.Instance().m_junzhuInfo.level)
            {
                return true;
            }
        }
        return false;
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
                         }
                         else if (EquipType == 1 && item.Value[0].itemType == 1)
                         {
                             EquipExp += item.Value[0].cnt * ItemTemp.templates[i].effectId;
                         }
                     }
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
        string upLevelTitleStr = LanguageTemplate.GetText(2000);

        string confirmStr = LanguageTemplate.GetText(2003);
        string cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string str =  MyColorData.getColorString(35, LanguageTemplate.GetText(2005) + LanguageTemplate.GetText(2004))
             + "\n" + MyColorData.getColorString(35, LanguageTemplate.GetText(2006) + LanguageTemplate.GetText(2004))
             + "\n" + LanguageTemplate.GetText(2007) +"\n" +LanguageTemplate.GetText(2008) + LanguageTemplate.GetText(2009);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, GuoMai, null, null);
    }
    void CreateMove(GameObject move, string content,int type)
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
        if (type == 0)
        {
            clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        }
        else
        {
            clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 60;
        }
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
            if (FunctionOpenTemp.GetWhetherContainID(9))
            {
                ShopData.Instance.OpenShop(ShopData.ShopType.ROLL);
            }
            else
            {
                ClientMain.m_UITextManager.createText("商店未开启");
            }
        }

    }

    public struct MaterialsInfo
    {
        public long _dbId;
        public int _cnt;
        public int pinzhi;
        public int buwei;
        public int _exp;
    };
    private List<MaterialsInfo> _listOneLevelMaterial = new List<MaterialsInfo>();

    void Strengthen(int index)// 强化升一级ID统计
    {
        int EquipExp = 0;
        int equipLevel = 0;
        int pinzhi = 0;
 
        List<BagItem> _listMaterial = new List<BagItem>();
        List<BagItem> _listSuiPian = new List<BagItem>();
        List<BagItem> _listEquip = new List<BagItem>();

        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index))
        {
            equipLevel = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaLv;
            EquipExp = EquipsOfBody.Instance().m_equipsOfBodyDic[index].qiangHuaExp;
            pinzhi = EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi;

            int size_Material = EquipGrowthMaterialUseManagerment.listMaterials.Count;
            for (int i = 0; i < size_Material; i++)
            {
                for (int j = 0; j < int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count); j++)
                {
                    EquipExp += EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP;
                    EquipGrowthMaterialUseManagerment.listTouchedId.Add(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid);
                    if (NeedIsEnought(EquipExp, index, equipLevel))
                    {

                        EquipIntensy();
                        int ssz = EquipGrowthMaterialUseManagerment.listTouchedId.Count;
                        return;
                    }
                }
            }

            if (EquipExp < ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoadCallback);
            }

        }
    }

    private void TidyOneLevelMaterial()
    {


    }

    private void EquipIntensy()
    {
        EquipUpGrade(savedbid, EquipGrowthMaterialUseManagerment.listTouchedId);
    }
    private bool NeedIsEnought(int add_exp,int buwei,int equip_level)
    {
        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(buwei) && add_exp >= ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].itemId).expId, equip_level).needExp)
        {
            return true;
        }
        return false;
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



    List<MaterialsInfo> QualitySortOneLevel(List<MaterialsInfo> list)//按品质排序
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                if (list[j].pinzhi > list[j + 1].pinzhi)
                {
                    MaterialsInfo mm = new MaterialsInfo();
                    mm = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = mm;
                }
            }
        }
        return list;

    }



    List<MaterialsInfo> SortBuWeiOneLevel(List<MaterialsInfo> list)//按部位排序
    {
        List<MaterialsInfo> listMid = new List<MaterialsInfo>();
        int[] Sequence = { 1, 2, 3, 11, 12, 13, 14, 15, 16 };
        for (int i = 0; i < Sequence.Length; i++)
        {
            for (int j = 0; j < list.Count; j++)
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
                    listMid.Add(list[j]);
                }
            }
        }
        return listMid;
    }
    private bool WetherContainMaterialsWuQi()//材料信息整理
    {
        foreach (KeyValuePair<int, BagItem> equip in EquipsOfBody.Instance().m_equipsOfBodyDic)
        {
            foreach (KeyValuePair<long, List<BagItem>> item in BagCaiLiao)
            {
                for (int i = 0; i < ItemTemp.templates.Count; i++)
                {
                    if (item.Value[0].itemId == ItemTemp.templates[i].id/* && ItemTemp.templates[i].quality != 0 */)
                    {
                        if (item.Value[0].itemType == 1 && item.Value[0].cnt > 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool WetherContainMaterialsFangJu()//材料信息整理
    {
        foreach (KeyValuePair<int, BagItem> equip in EquipsOfBody.Instance().m_equipsOfBodyDic)
        {
            foreach (KeyValuePair<long, List<BagItem>> item in BagCaiLiao)
            {
                for (int i = 0; i < ItemTemp.templates.Count; i++)
                {
                    if (item.Value[0].itemId == ItemTemp.templates[i].id/* && ItemTemp.templates[i].quality != 0 */)
                    {
                        if (item.Value[0].itemType == 2 && item.Value[0].cnt > 0)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool EquipLevelOverJunZhuWuQi()
    {
        int[] WuQi = { 3, 4, 5 };
        int size = WuQi.Length;
        for (int i = 0;i < size;i++)
        {
            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(WuQi[i]))
            {
                if (EquipsOfBody.Instance().m_equipsOfBodyDic[WuQi[i]].qiangHuaLv < JunZhuData.Instance().m_junzhuInfo.level)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool EquipLevelOverJunZhuFangJu()
    {
        int[] fangju = { 0, 1, 2, 6, 7, 8};
        int size = fangju.Length;
        for (int i = 0; i < size; i++)
        {
            if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(fangju[i]))
            {
                if (EquipsOfBody.Instance().m_equipsOfBodyDic[fangju[i]].qiangHuaLv < JunZhuData.Instance().m_junzhuInfo.level)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool EquipsAllLevelMax()
    {
        int sum = 0;
       foreach(KeyValuePair<int, BagItem> item in EquipsOfBody.Instance().m_equipsOfBodyDic)
       { 
            
            if (item.Value.qiangHuaLv == item.Value.qianghuaHighestLv)
            {
                sum++;
            }
        }

        if (sum == EquipsOfBody.Instance().m_equipsOfBodyDic.Count)
        {
            return true;
        }
        return false;
    }
}