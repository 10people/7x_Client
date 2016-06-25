using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipGrowthEquipAdvanceManagerment : MonoBehaviour, UI2DEventListener, SocketProcessor
{
    public List<EventIndexHandle> listEvent;
    public UIGrid IconSampleGrid;
    public List<UISprite> listSprite;
    public int SelectType = 0;
    public EquipGrowthEquipInfoManagerment m_EquipGrowthEquipInfoManagerment;
    public EquipGrowthEquipInfoManagerment.EquipBaseInfo _baseInfo;
    public List<GameObject> listButton = new List<GameObject>();
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoEquips = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    public EquipGrowthEquipInfoAdvanceItemManagerment m_EquipAdvanceInfo;

    private int Equip_BuWei = 0;
    private int Equip_PinZhi = 0;
    private Dictionary<long, GameObject> _DicMaterial = new Dictionary<long, GameObject>();
    private bool isMaxSave;
    private long savedbid = 0;
    private int currSave;
    private int maxSave;
    private int EquipSavedId = 0;
    private long EquipSavedDBId = 0;
    void Start()
    {
        SocketTool.RegisterMessageProcessor(this);
        listEvent.ForEach(p => p.m_Handle += CompoundTouch);
    }

    void OnEnable()
    {
      
    }
    void Update()
    {
        if (EquipGrowthMaterialUseManagerment.materialItemTouched)
        {
            EquipGrowthMaterialUseManagerment.materialItemTouched = false;
            ProgressBarExhibition();
        }
    }
    void CompoundTouch(int index)
    {
 
        switch (index)
        {
            case 0:
                {
                    if (ZhuangBei.getZhuangBeiById(_baseInfo._EquipId).jiejieId == 0)
                    {
                        ClientMain.m_UITextManager.createText("装备已进阶至最高品质");
                        return;
                    }
                    if (EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0)
                    {
                        MemoryStream tempStream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipJinJie tempAdvanceReq = new EquipJinJie();
                        tempAdvanceReq.equipId = EquipSavedDBId;
                        tempAdvanceReq.cailiaoList = EquipGrowthMaterialUseManagerment.listTouchedId;
                        t_qx.Serialize(tempStream, tempAdvanceReq);

                        byte[] t_protof;
                        t_protof = tempStream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_JINJIE, ref t_protof);
                    }
                    else
                    {
                        ClientMain.m_UITextManager.createText("未放入材料，无法合成");
                    }
                }
                break;
            case 1:
                {
                    if (ZhuangBei.getZhuangBeiById(_baseInfo._EquipId).jiejieId == 0)
                    {
                        ClientMain.m_UITextManager.createText("装备已进阶至最高品质");
                        return;
                    }

                    if (!isMaxSave)
                    {
                        ClientMain.m_UITextManager.createText("装备已进阶至最高品质");
                    }
                    else if (EquipGrowthMaterialUseManagerment.listMaterials.Count == 0)
                    {
                        Global.CreateFunctionIcon(601);
                    }
                    else
                    {
                        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
                        {
                            TaskData.Instance.m_iCurMissionIndex = 100100;
                            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                            tempTaskData.m_iCurIndex++;
                            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
                        }
                        AdvaceOneLevel();
                    }
                   
                }
                break;
            case 2:
                {
                    Global.CreateFunctionIcon(2001);
                }
                break;
        }
    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_EQUIP_JINJIE:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipJinJieResp Equip = new EquipJinJieResp();
                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());

                       
                        if (EquipSavedId != Equip.zbItemId)
                        {
                            if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
                            {
                                TaskData.Instance.m_iCurMissionIndex = 100100;
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                tempTaskData.m_iCurIndex++;
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
                            }
                            FunctionWindowsCreateManagerment.FreshAdvanceEquipInfo(Equip);
                            FunctionWindowsCreateManagerment.EquipAdvanceInfo ainfo = new FunctionWindowsCreateManagerment.EquipAdvanceInfo();
                            ainfo._equipid = EquipSavedId;
                            ainfo._nextid = ZhuangBei.GetItemByID(EquipSavedId).jiejieId;
                            ainfo._gong = _baseInfo._Gong;
                            ainfo._fang = _baseInfo._Fang;
                            ainfo._ming = _baseInfo._Xue;
                            ainfo._gongadd = Equip.gongJi - _baseInfo._Gong;
                            ainfo._fanggadd = Equip.fangYu - _baseInfo._Fang;
                            ainfo._minggadd = Equip.shengMing - _baseInfo._Xue;

                            FunctionWindowsCreateManagerment.m_AdvanceInfo = ainfo;
                            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_ADVANCE),
                                ResLoadedSimple);
                        }

                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());
                        _baseInfo._EquipId = Equip.zbItemId;
                        _baseInfo._Gong = Equip.gongJi;
                        _baseInfo._Fang = Equip.fangYu;
                        _baseInfo._Xue = Equip.shengMing;
                        _baseInfo._dbid = Equip.equipId;
                        _baseInfo._Icon = ZhuangBei.getZhuangBeiById(Equip.zbItemId).icon;
                        _baseInfo._PinZhi = ZhuangBei.getZhuangBeiById(Equip.zbItemId).pinZhi;
                        _baseInfo._advanceExp = Equip.exp;
                        ShowInfo(_baseInfo);
                        return true;
                    }
            }
        }
        return false;
    }
    void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.GetComponent<EquipAdvanceLayerManagerment>().ShowInfo(FunctionWindowsCreateManagerment.m_AdvanceInfo);
    }
    public void RB_UB_Button3_LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        MainCityUI.TryAddToObjectList(tempObject);
        tempObject.transform.position = new Vector3(0, 500, 0);
    }

    public void OnUI2DShow()
    {
        listM.Clear();
        CaiLiaoEquips.Clear();
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        MaterialsInfoTidy();
    }
    public void ShowInfo(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)//界面信息显示
    {
  
        _baseInfo = baseInfo;
        EquipSavedDBId = baseInfo._dbid;
        baseInfo._MaxEXp = ZhuangBei.getZhuangBeiById(baseInfo._EquipId).lvlupExp;
        curr_residue = 0;
        curr_Max = 0;
        addCount = 0;
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
        ShowEquipTanHao();
        currSave = baseInfo._advanceExp;
        Material_Num = 0;
        listM.Clear();
        Equip_BuWei = FunctionWindowsCreateManagerment.GetNeedLocation(ZhuangBei.getZhuangBeiById(baseInfo._EquipId).buWei);
        Equip_PinZhi = baseInfo._PinZhi;
        EquipGrowthMaterialUseManagerment.listCurrentAddExp.Clear();
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        EquipSavedId = baseInfo._EquipId;
        EquipGrowthMaterialUseManagerment.m_EuipId = baseInfo._EquipId;
        EquipGrowthMaterialUseManagerment.equipAdvanceid = baseInfo._EquipId;
        curr_residue = baseInfo._advanceExp;
        maxSave = baseInfo._MaxEXp;
        isMaxSave = baseInfo._advanceExp < baseInfo._MaxEXp ? true : false;
        EquipGrowthMaterialUseManagerment.touchIsEnable = isMaxSave;
        savedbid = baseInfo._dbid;
        CaiLiaoEquips.Clear();
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        m_EquipAdvanceInfo.ShowInfo(baseInfo);
        EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = false;
        ZhuangBei.m_listNeedInfo.Clear();
        ZhuangBei.m_listReduceInfo.Clear();
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        ShowEquipTanHao();
        MaterialsInfoTidy();
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100100;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex++;
            if (tempTaskData.m_iCurIndex < tempTaskData.m_listYindaoShuju.Count)
            {
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
            }
        }

    }

    void MaterialsInfoTidy()//材料信息整理
    {

        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
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
                material.materialEXP = ZhuangBei.GetItemByID(item.Value.itemId).exp;
                CaiLiaoEquips.Add(material);
            }
        }
        QualitySort2(CaiLiaoEquips);
    }
    EquipGrowthMaterialUseManagerment.MaterialInfo mInfo = new EquipGrowthMaterialUseManagerment.MaterialInfo();
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

    List<EquipGrowthMaterialUseManagerment.MaterialInfo> listM = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    void DestroyChild(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//新创建子物体
    {
        int sizeSend = list.Count;
        for (int i = 0; i < sizeSend; i++)
        {
            listM.Add(list[i]);
        }
        for (int i = 0; i < listM.Count; i++)
        {
            EquipGrowthMaterialUseManagerment.listMaterials.Add(listM[i]);
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
            ShowMaterialInfos(listM);
        }

    }
    struct MaterialInfo
    {
        public int materialid;
        public long materialdbid;
        public string texture;
        public string count;
        public int pinzhi;
        public int materialExp;
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
            //ShowMaterialInfo(int id, string icon, string count, bool isMaxExp, Over_JunZhu_Level callback)
            minfo._itemid = listInfoo[index_Num2].materialid;
            minfo._dbid = listInfoo[index_Num2].materialdbid;
            minfo._icon = listInfoo[index_Num2].texture;
            minfo._count = listInfoo[index_Num2].count;
            minfo._pinzhi = listInfoo[index_Num2].pinzhi;

          equipGrowthMaterialItem.ShowMaterialInfo(minfo,  isMaxSave,OverStepTip);

            if (listInfoo[index_Num2].materialdbid != 0)
            {
                _DicMaterial.Add(listInfoo[index_Num2].materialdbid, iconSampleObject);
            }
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
                        if (_DicMaterial.ContainsKey(list[i].dbid))
                        {
                            _DicMaterial[list[i].dbid] = IconSampleGrid.transform.GetChild(i).gameObject;
                        }
                        else
                        {
                            _DicMaterial.Add(list[i].dbid, IconSampleGrid.transform.GetChild(i).gameObject);
                        }
                        IconSampleGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().m_IntenseShow = true;
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
    
                        minfo._itemid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        minfo._dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        minfo._icon = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        minfo._count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        minfo._pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        IconSampleGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave , OverStepTip);
                        
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
                        if (_DicMaterial.ContainsKey(list[i].dbid))
                        {
                            _DicMaterial[list[i].dbid] = IconSampleGrid.transform.GetChild(i).gameObject;
                        }
                        else
                        {
                            _DicMaterial.Add(list[i].dbid, IconSampleGrid.transform.GetChild(i).gameObject);
                        }
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
                        mInfo.materialid = list[i].materialId;
                        mInfo.materialdbid = list[i].dbid;
                        mInfo.texture = list[i].icon;
                        mInfo.count = list[i].count;
                        mInfo.pinzhi = list[i].quality;
                        mInfo.materialExp = list[i].materialEXP;
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
            _DicMaterial.Clear();
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


    int curr_residue = 0;
    int curr_Max = 0;
    int addCount = 0;



    int lastcontent = 0;
    int lastExpAll = 0;


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
            if (ZhuangBei.getZhuangBeiById(ZhuangBei.GetMaxAdvanceIdByAddExp(EquipSavedId
                , EquipGrowthMaterialUseManagerment.m_TotalAddExp
                , ZhuangBei.getZhuangBeiById(EquipSavedId).buWei)).jiejieId == 0)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
            }
            else
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = true;
            }

            EquipGrowthMaterialUseManagerment.ReduceUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);

            ProcessReduce(content);
            CreateClone(m_EquipGrowthEquipInfoManagerment.m_EquipItenm.m_LabelProgress.gameObject, content * -1);
        }
        else
        {
            if (curr_Max == 0)
            {
                curr_Max = maxSave;
            }
            curr_residue += content;
            EquipGrowthMaterialUseManagerment.m_TotalAddExp = curr_residue;


            if (ZhuangBei.getZhuangBeiById(ZhuangBei.GetMaxAdvanceIdByAddExp(EquipGrowthMaterialUseManagerment.equipAdvanceid
                , EquipGrowthMaterialUseManagerment.m_TotalAddExp
                , ZhuangBei.getZhuangBeiById(EquipSavedId).buWei)).jiejieId == 0)
            {
                EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = true;
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                CreateMove(m_EquipAdvanceInfo.m_LabelProgress.gameObject
                     , "当前装备已进阶到最高品质");
            }

            _EquipIdAdd = EquipSavedId;

            CreateClone(m_EquipAdvanceInfo.m_LabelProgress.gameObject, content);
            ProcessAddEffect();
        }
    }

    int _EquipIdAdd = 0;
    void ProcessAddEffect()
    {
        _EquipidReduce = 0;
        _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
        ZhuangBei.GetUpgradeMaxLevel_ById(EquipGrowthMaterialUseManagerment.equipAdvanceid
           , ZhuangBei.getZhuangBeiById(EquipGrowthMaterialUseManagerment.equipAdvanceid).buWei, curr_residue);
        lastcontent = curr_residue;

        int size = ZhuangBei.m_listNeedInfo.Count;
        int sum_All = 0;
        int judgeIndex = 0;

        for (int i = 0; i < size; i++)
        {
            if (i < size - 1 && ZhuangBei.m_listNeedInfo[i].needExp > 0)
            {
                sum_All += ZhuangBei.m_listNeedInfo[i].needExp;
            }
            else
            {
                judgeIndex = i;
            }

            if (i == size - 1)
            {
                EquipGrowthMaterialUseManagerment.equipAdvanceid = ZhuangBei.m_listNeedInfo[i].id;
            }
        }
        lastExpAll = sum_All;
        if (curr_residue - sum_All >= 0)
        {
            curr_residue -= sum_All;
        }


        curr_Max = ZhuangBei.m_listNeedInfo[size - 1].needExp;

        if (EquipGrowthMaterialUseManagerment.equipAdvanceid > _EquipIdAdd)
        {
            _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
        }
        else
        {
            _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
        }

        if (curr_Max < 0)
        {
            m_EquipAdvanceInfo.m_PregressBar.value = 1;
            if (curr_residue > 0)
            {
                m_EquipAdvanceInfo.m_LabelProgress.text = curr_residue.ToString() + "/0";
            }
            else
            {
                m_EquipAdvanceInfo.m_LabelProgress.text = "0/0";
            }

        }
        else
        {
            m_EquipAdvanceInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_EquipAdvanceInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
    }

    int _EquipidReduce = 0;
    void ProcessReduce(int content)
    {
        if (curr_residue >= content)
        {
            curr_residue -= content;

            if (EquipGrowthMaterialUseManagerment.equipAdvanceid < _EquipidReduce)
            {
                _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
                _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            }
            else
            {
                _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
                _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            }

            if (curr_Max < 0)
            {
                m_EquipAdvanceInfo.m_PregressBar.value = 1.0f;
                if (curr_residue > 0)
                {
                    m_EquipAdvanceInfo.m_LabelProgress.text = curr_residue.ToString()+ "/0";
                }
                else
                {
                    m_EquipAdvanceInfo.m_LabelProgress.text = "0/0";
                }
            }
            else
            {
                m_EquipAdvanceInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
                m_EquipAdvanceInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
            }

        }
        else
        {
            ZhuangBei.GetReduceMaxLevel_Byid(EquipGrowthMaterialUseManagerment.equipAdvanceid
                , content, ZhuangBei.getZhuangBeiById(EquipGrowthMaterialUseManagerment.equipAdvanceid).buWei);

            int size = ZhuangBei.m_listReduceInfo.Count;
            int reduceIndex = 0;
            int sum_Reduce = curr_residue;
            for (int i = 0; i < size; i++)
            {
                if (ZhuangBei.m_listReduceInfo[i].needExp > 0)
                {
                    sum_Reduce += ZhuangBei.m_listReduceInfo[i].needExp;
                }
                if (sum_Reduce >= content)
                {
                    reduceIndex = i;
                    break;
                }
            }
            curr_residue = sum_Reduce - content;
            curr_Max = ZhuangBei.m_listReduceInfo[reduceIndex].needExp;
            EquipGrowthMaterialUseManagerment.equipAdvanceid = ZhuangBei.m_listReduceInfo[reduceIndex].id;
            int QiangHuaId = int.Parse(ZhuangBei.GetQiangHuaIdByEquipID(EquipSavedId));

            if (EquipGrowthMaterialUseManagerment.equipAdvanceid < _EquipidReduce)
            {
                _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
                _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            }
            else
            {
                _EquipIdAdd = EquipGrowthMaterialUseManagerment.equipAdvanceid;
                _EquipidReduce = EquipGrowthMaterialUseManagerment.equipAdvanceid;
            }
            m_EquipAdvanceInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_EquipAdvanceInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
    }

    void OnDisable()
    {
 
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    void OverStepTip(int index)
    {
        if (index > 0)
        {
            if (ZhuangBei.getZhuangBeiById(EquipGrowthMaterialUseManagerment.equipAdvanceid).jiejieId == 0)
            {
                ClientMain.m_UITextManager.createText("装备已进阶至最高品质");
            }
        }
        else
        {
            ClientMain.m_UITextManager.createText("装备已进阶至最高品质");
          //  CreateMove(m_EquipAdvanceInfo.m_LabelProgress.gameObject, "已达到最高阶");
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

    public void ShowEquipTanHao()
    {
        m_EquipGrowthEquipInfoManagerment.ShowAdvanceTanHao();
        int _size = m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent.Count;
        for (int i = 0; i < _size; i++)
        {
            if (CompoundEnable(i))
            {
                m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(true);
            }
            else
            {
                m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(false);
            }
        }
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
        string str = MyColorData.getColorString(35, LanguageTemplate.GetText(2005) + LanguageTemplate.GetText(2004))
             + "\n" + MyColorData.getColorString(35, LanguageTemplate.GetText(2006) + LanguageTemplate.GetText(2004))
             + "\n" + LanguageTemplate.GetText(2007) + "\n" + LanguageTemplate.GetText(2008) + LanguageTemplate.GetText(2009);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, cancelStr, confirmStr, GuoMai, null, null);
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
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 60;
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
    void GuoMai(int index)
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

 
    private bool NeedIsEnought(int add_exp, int buwei)
    {
        if (add_exp >= ZhuangBei.getZhuangBeiById(EquipSavedId).lvlupExp)
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

    private void ConfirmStrength(int index)
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
    private bool EquipLevelOverJunZhuWuQi()
    {
        int[] WuQi = { 3, 4, 5 };
        int size = WuQi.Length;
        for (int i = 0; i < size; i++)
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
        int[] fangju = { 0, 1, 2, 6, 7, 8 };
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

    void AdvaceOneLevel()// 合成升一级ID统计
    {
        int GemExpAdd = 0;
        int GemExp = currSave;
 
        int size = EquipGrowthMaterialUseManagerment.listMaterials.Count;

        if (size == 0)
        {
            ClientMain.m_UITextManager.createText("材料不足！");
            return;
        }
        int size_add = EquipGrowthMaterialUseManagerment.listTouchedId.Count;
 
        for (int i = 0; i < size_add; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (EquipGrowthMaterialUseManagerment.listMaterials[j].dbid
                    == EquipGrowthMaterialUseManagerment.listTouchedId[i])
                {
                    GemExp += EquipGrowthMaterialUseManagerment.listMaterials[j].materialEXP;
                    break;
                }
            }
        }
        GemExpAdd = GemExp;
 
        if (!NeedIsEnought(GemExp, EquipSavedId))
        {
            for (int i = 0; i < size; i++)
            {
                if (GetNowMaterialAddCount(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid)
                    < int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count))
                {
                    for (int j = GetNowMaterialAddCount(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid); 
                        j < int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count); j++)
                    {
                        GemExp += EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP;
                        EquipGrowthMaterialUseManagerment.listTouchedId.Add(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid);
                        if (NeedIsEnought(GemExp, EquipSavedId))
                        {
                            if (GemExp - GemExpAdd > 0)
                            {
                                CreateClone(GemExp - GemExpAdd);
                                ShowAddNum();
                            }
 
                         //   ClientMain.m_UITextManager.createText("所需经验已满");
                            return;
                        }
                    }
                }
            }
            if (GetAllMaterialAddCount() == EquipGrowthMaterialUseManagerment.listTouchedId.Count)
            {
                if (GemExp - GemExpAdd > 0)
                {
                    CreateClone(GemExp - GemExpAdd);
                    ShowAddNum();
                }
               // ClientMain.m_UITextManager.createText("已添加所有装备");
                return;
            }
        }
        else
        {
            //if (GetAllMaterialAddCount() == EquipGrowthMaterialUseManagerment.listTouchedId.Count)
            //{
            //    ClientMain.m_UITextManager.createText("已添加所有装备");
            //}
            //else
            //{
            //    ClientMain.m_UITextManager.createText("所需经验已满");
            //}
        }
        if (GemExp - GemExpAdd > 0)
        {
            CreateClone(GemExp - GemExpAdd);
            ShowAddNum();
        }

    }

    void ShowAddNum()
    {
        foreach (KeyValuePair<long, GameObject> item in _DicMaterial)
        {
            if (GetNowMaterialAddCount(item.Key) > 0)
            {
                item.Value.GetComponent<EquipGrowthMaterialItem>().showLabInfo(GetNowMaterialAddCount(item.Key));
            }
        }
    }

    private int GetNowMaterialAddCount(long dbid)
    {
        int sum = 0;
        int size = EquipGrowthMaterialUseManagerment.listTouchedId.Count;
        for (int i = 0; i < size; i++)
        {
            if (EquipGrowthMaterialUseManagerment.listTouchedId[i] == dbid)
            {
                sum++;
            }
        }
        return sum;
    }

    private int GetAllMaterialAddCount()
    {
        int sum = 0;
        int size = EquipGrowthMaterialUseManagerment.listMaterials.Count;
        for (int i = 0; i < size; i++)
        {
            sum += int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count);
        }
        return sum;
    }

    private bool CompoundEnable(int index)
    {
        int EquipExp = 0;
        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(index)
            && ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).lvlupExp > 0)
        {
            EquipExp = EquipsOfBody.Instance().m_equipsOfBodyDic[index].jinJieExp;
            int tempBuwei = 0;
            switch (index)
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
            foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
            {
                if (tempBuwei == item.Value.buWei && item.Value.pinZhi <= EquipsOfBody.Instance().m_equipsOfBodyDic[index].pinZhi)
                {
                    EquipExp += ZhuangBei.GetItemByID(item.Value.itemId).exp;
                    if (EquipExp >= ZhuangBei.getZhuangBeiById(EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).lvlupExp)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
