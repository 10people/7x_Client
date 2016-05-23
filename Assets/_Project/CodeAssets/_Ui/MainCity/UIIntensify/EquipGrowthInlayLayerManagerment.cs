using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipGrowthInlayLayerManagerment : MonoBehaviour, UI2DEventListener, SocketProcessor
{
   
    public GameObject m_ObjInlayMain;
    public GameObject m_ObjGemInfo;
    public GameObject m_ObjGemlist;
    public List<EquipGrowthInlayGemItemManagerment> m_listGemItem;
    public List<EventIndexHandle> m_listEvent;
    public UIGrid m_GemsGrid;
    public UIGrid m_GemsNeedGrid;
    public UIGrid m_GemsNeedlist;
    public UILabel m_LabNoMaterial;
    public EquipGrowthEquipInfoManagerment m_EquipGrowthEquipInfoManagerment;
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoStrenth = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> CaiLiaoInfo = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    private Dictionary<long, GameObject> _DicMaterial = new Dictionary<long, GameObject>();
    private int index_Num = 0;
    private int Addindex = 0;
    Dictionary<long, List<BagItem>> BagCaiLiao = new Dictionary<long, List<BagItem>>();
    int curr_residue = 0;
    int curr_Max = 0;
    int addCount = 0;
    int ReduceIndex_Now = 0;
    public EquipGrowthGemEquipItemManagerment m_GemsEquip;
    bool IsResidueOn = false;
    int lastcontent = 0;
    int lastExpAll = 0;
    int currentLevel = 0;
    private int EquipType = 0;

    private int Equip_Level = 0;
    int Material_Num = 0;
    int Material_Num_2 = 0;
    private bool isMaxSave;
    private long savedbid = 0;
    private int currSave;
    private int maxSave;
    public EquipGrowthGemInfoItemManagerment m_GemDetailInfo;
    private GemsMainInfo geninfo = new GemsMainInfo();
    private int _GemSelectId = 0;
    private int _MaxGems = 0;
    public struct AddAttribute
    {
        public int _Gong;
        public int _Fang;
        public int _Ming;
        public int _GongAdd;
        public int _FangAdd;
        public int _MingAdd;
        public bool _IsChange;

    };
    private AddAttribute m_SaveInfo;
    struct MaterialInfo
    {
        public int materialid;
        public long materialdbid;
        public string texture;
        public string count;
        public int pinzhi;
    }
    MaterialInfo matInfo;
    private List<MaterialInfo> listInfoo = new List<MaterialInfo>();
    private List<MaterialInfo> listShowGems = new List<MaterialInfo>();

    public void OnUI2DShow()
    {
    
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        MaterialsInfoTidy();
    }
    public struct GemLayerInfo
    {
        public int _posNum;
        public bool _isopen;
        public string _icon;
        public string _Des;
        public int _id;
        public int _exp;
        public int _expMax;
        public string _Di;
        public string _kuang;
        public string _Lvinfo;
    }
    public struct GemsMainInfo
    {
        public List<GemLayerInfo> _ListGems;
        public EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo;
    }
    private EquipGrowthEquipInfoManagerment.EquipBaseInfo _baseInfo;
    private GemLayerInfo _gemsCurrTouchInfo ;
    public struct GemsEquipInfo
    {
        public int _EquipId;
        public int _Gong;
        public int _Fang;
        public int _Blood;
        public int _PinZhi;
    }

    public struct GemSelectInfo
    {
        public long _dbid;
        public int _Gemid;
        public string _Count;
        public string _Attribute;
        public int _Exp;
    }

  
    void Start()
    {
        m_listGemItem.ForEach(p => p.m_Add.m_Handle += TouchAdd);
        m_listGemItem.ForEach(p => p.m_GemInfo.m_Handle += TouchInfo);
        m_listEvent.ForEach(p => p.m_Handle += TouchEvent);
    }
    void OnEnable()
    {
        SocketTool.RegisterMessageProcessor(this);
    }
    void OnDisable()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
    void Update()
    {
        if (EquipGrowthMaterialUseManagerment.materialItemTouched)
        {
            EquipGrowthMaterialUseManagerment.materialItemTouched = false;
            ProgressBarExhibition();
        }

        if (EquipGrowthMaterialUseManagerment.listTouchedId.Count > 0)
        {
            m_listEvent[4].gameObject.SetActive(true);
        }
        else
        {
            m_listEvent[4].gameObject.SetActive(false);
        }
    }
    void TouchInfo(int index)
    {
        curr_Max = 0;
        addCount = 0;
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();

        m_listEvent[3].transform.localPosition = new Vector3(74, -20, 0);
        m_listEvent[4].gameObject.SetActive(false);
        m_listEvent[5].transform.localPosition = new Vector3(-139, -20, 0);
        _GemSelectId = geninfo._ListGems[index]._id;
        _gemsCurrTouchInfo = geninfo._ListGems[index];
     
        curr_residue = 0;
        currSave = geninfo._ListGems[index]._exp;
        EquipGrowthMaterialUseManagerment.m_GemLevel = FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[index]._id).fuwenLevel;
        EquipGrowthMaterialUseManagerment.m_GemLevelsaved = FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[index]._id).fuwenLevel;
        maxSave = geninfo._ListGems[index]._expMax;
        EquipGrowthMaterialUseManagerment.m_TotalAddExp = 0;
        isMaxSave = geninfo._ListGems[index]._exp < geninfo._ListGems[index]._expMax ? true : false;
      
        EquipGrowthMaterialUseManagerment.touchIsEnable = isMaxSave;

        m_GemDetailInfo.ShowInfo(geninfo._ListGems[index]);
        FreshMaterials(FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[index]._id).shuxing);
        m_ObjInlayMain.SetActive(false);
        m_ObjGemInfo.SetActive(true);
    }

    void FreshMaterials(int gem_type)
    {
        CaiLiaoInfo.Clear();
        for (int i = 0; i < CaiLiaoStrenth.Count; i++)
        {
            if (gem_type == CaiLiaoStrenth[i].attribute)
            {
                EquipGrowthMaterialUseManagerment.listMaterials.Add(CaiLiaoStrenth[i]);
                CaiLiaoInfo.Add(CaiLiaoStrenth[i]);
            }
        }
        Material_Num_2 = 0;
  
        ShowNeedMaterialInfo();
    }
    private int _posNum = 0;
    void TouchAdd(int index)
    {
        if (FreshGuide.Instance().IsActive(200030) && TaskData.Instance.m_TaskInfoDic[200030].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 200030;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex++;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        m_ObjGemlist.SetActive(true);
        _posNum = index;
        index_list = 0;
        int inlayerid = 1;
        CaiLiaoInfo.Clear();
        for (int i = 0; i < CaiLiaoStrenth.Count; i++)
        {
            if (ZhuangBei.getZhuangBeiById(_equipIdSave).inlayColor  == CaiLiaoStrenth[i].attribute)
            {
                EquipGrowthMaterialUseManagerment.listMaterials.Add(CaiLiaoStrenth[i]);
                CaiLiaoInfo.Add(CaiLiaoStrenth[i]);
            }
        }

        if (CaiLiaoInfo.Count == 0)
        {
            m_LabNoMaterial.gameObject.SetActive(true);
        }
        else
        {
            m_LabNoMaterial.gameObject.SetActive(false);
        }
        int size = CaiLiaoInfo.Count;
        int child_size = m_GemsNeedlist.transform.childCount;

        if (child_size > 0)
        {
            for (int i = 0; i < child_size; i++)
            {
                Destroy(m_GemsNeedlist.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0;i < size;i++)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_GROWTH_GEM_ITEM), Select_item_LoadCallBack);
        }
    }
 
    void TouchEvent(int index)
    {
        switch (index)
        {
            case 0:
                {
                    Global.CreateFunctionIcon(1801);
                }
                break;
            case 1://请求一键拆解 
                {
                    if (GetEquipContainGemsCount() == 0)
                    {
                        ClientMain.m_UITextManager.createText("装备上没有镶嵌宝石");
                    }
                    else
                    {
                        MemoryStream t_tream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipOperationReq equip = new EquipOperationReq();
                        equip.equlpId = _equipDBIDSave;
                        equip.type = 3;
                        t_qx.Serialize(t_tream, equip);
                        byte[] t_protof;
                        t_protof = t_tream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
                    }
                }
                break;
            case 2://请求一键镶嵌
                {
                   if (_MaxGems == GetEquipContainGemsCount())
                    {
                        ClientMain.m_UITextManager.createText("宝石孔已镶满");
                    }
                    else
                    {
                        MemoryStream t_tream = new MemoryStream();
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipOperationReq equip = new EquipOperationReq();
                        equip.equlpId = _equipDBIDSave;
                        equip.type = 2;
                        t_qx.Serialize(t_tream, equip);
                        byte[] t_protof;
                        t_protof = t_tream.ToArray();
                        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
                    }
                }
                break;
            case 3:	//请求拆解一颗宝石
                {
                    MemoryStream t_tream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    EquipOperationReq equip = new EquipOperationReq();
                    equip.equlpId = _equipDBIDSave;
                    equip.possionId = _gemsCurrTouchInfo._posNum;
                    equip.type = 5;
                    t_qx.Serialize(t_tream, equip);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
                }
                break;
            case 4://	type = 6：请求宝石合成
                {
                    MemoryStream t_tream = new MemoryStream();
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    EquipOperationReq equip = new EquipOperationReq();
                    equip.equlpId = _equipDBIDSave;
                    equip.possionId = _gemsCurrTouchInfo._posNum;
                    equip.cailiaoList = EquipGrowthMaterialUseManagerment.listTouchedId;
                    equip.type = 6;
                    t_qx.Serialize(t_tream, equip);
                    byte[] t_protof;
                    t_protof = t_tream.ToArray();
                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
                }
                break;
            case 5:
                {
                    _gemsCurrTouchInfo._id = 0;
                    m_ObjGemInfo.SetActive(false);
                    m_ObjInlayMain.SetActive(true);
                    ShowChangeInfo(m_SaveInfo);
                }
                break;
            case 6:
                {
                    if (!isMaxSave)
                    {
                        ClientMain.m_UITextManager.createText("已达到最高阶");
                    }
                    else
                    {
                        Strengthen();
                    }
                }
                break;
            case 7:
                {
                    m_ObjGemlist.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
    private List<GemLayerInfo> listGems = new List<GemLayerInfo>();
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.S_EQUIP_BAOSHI_RESP://装备相关宝石镶嵌返回
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipOperationResp Equip = new EquipOperationResp();
                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());
                        if (!Equip.Succes && (Equip.type == 2 || Equip.type == 3))
                        {
                            return false;
                        }
                        if (Equip.JewelList != null)
                        {
                            if (Equip.type == 1 || Equip.type == 2 || Equip.type == 3)
                            {
                                listGems.Clear();
                                for (int i = 0; i < 5; i++)
                                {
                                    GemLayerInfo gem = new GemLayerInfo();
                                    gem._posNum = i;
                                    _MaxGems = Equip.JewelList.jewelNum;
                                    if (i < Equip.JewelList.jewelNum)
                                    {
                                        gem._isopen = true;
                                        gem._id = 0;
                                        gem._Des = "";
                                    }
                                    else if (i >= Equip.JewelList.jewelNum)
                                    {
                                        gem._isopen = false;
                                        gem._id = 0;
                                        gem._Des = BaoShiOpenTemplate.GetTemplatesBuweiAndNum(ZhuangBei.getZhuangBeiById(_equipIdSave).buWei
                                                   , i);
                                    }
                                    else
                                    {
                                        gem._isopen = false;
                                        gem._id = 0;
                                        gem._Des = "";
                                    }

                                    if (Equip.JewelList.list != null)
                                    {
                                        for (int j = 0; j < Equip.JewelList.list.Count; j++)
                                        {
                                            if (Equip.JewelList.list[j].possionId == i)
                                            {
                                                gem._isopen = true;
                                                gem._id = Equip.JewelList.list[j].itemId;
                                                gem._exp = Equip.JewelList.list[j].exp;
                                                gem._expMax = FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[j].itemId).lvlupExp;
                                                gem._Lvinfo = "Lv." + FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[j].itemId).fuwenLevel 
                                                          +  AddType(FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[j].itemId).shuxing);
                                                gem._Des = "";
                                                break;
                                            }
                                        }
                                    }
                                    gem._Di = "inlayColor" + ZhuangBei.getZhuangBeiById(_equipIdSave).inlayColor;
                                    gem._kuang = "inlaRound" + ZhuangBei.getZhuangBeiById(_equipIdSave).inlayColor;
                                    listGems.Add(gem);
                                }

                                m_ObjInlayMain.SetActive(true);
                                m_ObjGemInfo.SetActive(false);
                            }
                            //else if (Equip.type == 5)
                            //{
                            //    GemLayerInfo gem = new GemLayerInfo();
                            //    gem._isopen = true;
                            //    gem._id = 0;
                            //    gem._Des = "";
                            //    geninfo._ListGems[Equip.OneJewel.possionId] = gem;

                            //    m_ObjGemInfo.SetActive(false);
                            //}
                            //else if (Equip.type == 4)
                            //{
                            //    GemLayerInfo gem = new GemLayerInfo();
                            //    gem._isopen = true;
                            //    gem._id = Equip.OneJewel.itemId;
                            //    gem._exp = Equip.OneJewel.exp;
                            //    gem._Des = "";

                            //    geninfo._ListGems[Equip.OneJewel.possionId] = gem;
                            //    m_ObjGemlist.SetActive(false);
                            //}
                            //else if (Equip.type == 6)
                            //{
                            //    GemLayerInfo gem = new GemLayerInfo();
                            //    gem._isopen = true;
                            //    gem._id = Equip.OneJewel.itemId;
                            //    gem._Des = "";
                            //    gem._exp = Equip.OneJewel.exp;
                            //    curr_residue = Equip.OneJewel.exp;
                            //    geninfo._ListGems[Equip.OneJewel.possionId] = gem;
                            //    TouchInfo(Equip.OneJewel.possionId);

                            //}
                          
                            geninfo._ListGems = listGems;

                            geninfo.baseInfo = _baseInfo;
                        
                        }
                        else
                        {
                            GemLayerInfo gem = new GemLayerInfo();
                  
                            if (Equip.type == 5)
                            {
                                gem._isopen = true;
                                gem._id = 0;
                                gem._Des = "";
                                m_ObjGemInfo.SetActive(false);
                                m_ObjInlayMain.SetActive(true);
                            }
                            else if (Equip.type == 4)
                            {
                                gem._isopen = true;
                                gem._id = Equip.OneJewel.itemId;
                                gem._exp = Equip.OneJewel.exp;
                                gem._Lvinfo = "Lv." + FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).fuwenLevel
                                              + AddType(FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).shuxing);
                                gem._Des = "";
                                gem._exp = Equip.OneJewel.exp;
                                gem._expMax = FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).lvlupExp;
                                m_ObjGemlist.SetActive(false);
                            }
                            else if (Equip.type == 6)
                            {
                                gem._isopen = true;
                                gem._id = Equip.OneJewel.itemId;
                                gem._Des = "";
                                gem._exp = Equip.OneJewel.exp;
                                gem._Lvinfo = "Lv." + FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).fuwenLevel
                                               + AddType(FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).shuxing);
                                gem._expMax = FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.OneJewel.itemId).lvlupExp;
                                //TouchInfo(Equip.OneJewel.possionId);
                            }
                            gem._Di = "inlayColor" + ZhuangBei.getZhuangBeiById(_equipIdSave).inlayColor;
                            gem._kuang = "inlaRound" + ZhuangBei.getZhuangBeiById(_equipIdSave).inlayColor;
                            gem._posNum = Equip.OneJewel.possionId;
                            geninfo._ListGems[Equip.OneJewel.possionId] = gem;

                            if (Equip.type == 6)
                            {
                                TouchInfo(Equip.OneJewel.possionId);
                            }
                            if (Equip.JewelList != null)
                            {
                                if (Equip.JewelList.list != null)
                                {
                                    _baseInfo._gemGong = 0;
                                    _baseInfo._gemFang = 0;
                                    _baseInfo._gemMing = 0;

                                    for (int i = 0; i < Equip.JewelList.list.Count; i++)
                                    {

                                        switch (FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[i].itemId).shuxing)
                                        {
                                            case 1:
                                                {
                                                    _baseInfo._gemGong += FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[i].itemId).shuxingValue;
                                                }
                                                break;
                                            case 2:
                                                {
                                                    _baseInfo._gemFang += FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[i].itemId).shuxingValue;
                                                }
                                                break;
                                            case 3:
                                                {
                                                    _baseInfo._gemMing += FuWenTemplate.GetFuWenTemplateByFuWenId(Equip.JewelList.list[i].itemId).shuxingValue;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                          
                            geninfo._ListGems = listGems;

                            geninfo.baseInfo = _baseInfo;
                          
                        }

                        if (geninfo._ListGems != null)
                        {
                            _baseInfo._gemGong = 0;
                            _baseInfo._gemFang = 0;
                            _baseInfo._gemMing = 0;

                            for (int i = 0; i < geninfo._ListGems.Count; i++)
                            {
                                if (geninfo._ListGems[i]._id > 0)
                                {
                                   
                                    switch (FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[i]._id).shuxing)
                                    {
                                        case 1:
                                            {
                                                _baseInfo._gemGong += FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[i]._id).shuxingValue;
                                            }
                                            break;
                                        case 2:
                                            {
                                                _baseInfo._gemFang += FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[i]._id).shuxingValue;
                                            }
                                            break;
                                        case 3:
                                            {
                                                _baseInfo._gemMing += FuWenTemplate.GetFuWenTemplateByFuWenId(geninfo._ListGems[i]._id).shuxingValue;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        AddAttribute att = new AddAttribute();
                        if (Equip.type == 2 || Equip.type == 3 || Equip.type == 4 || Equip.type == 5 || Equip.type == 6)
                        {
                            att._Gong = _baseInfo._gemGong;
                            att._Fang = _baseInfo._gemFang;
                            att._Ming = _baseInfo._gemMing;
                            att._GongAdd = _baseInfo._gemGong - m_SaveInfo._Gong;
                            att._FangAdd = _baseInfo._gemFang - m_SaveInfo._Fang;
                            att._MingAdd = _baseInfo._gemMing - m_SaveInfo._Ming;
                            att._IsChange = att._GongAdd > 0 || att._FangAdd > 0 || att._MingAdd > 0;
                           
                            m_SaveInfo = att;
                            if (Equip.type == 3 || Equip.type == 5)
                            {
                                ShowChangeInfo(m_SaveInfo);
                            }

                            if (Equip.type == 6)
                            {
                                ShowChangeInfo(m_SaveInfo);
                            }
                        }
                        else if (Equip.type == 1)
                        {
                            att._Gong = _baseInfo._gemGong;
                            att._Fang = _baseInfo._gemFang;
                            att._Ming = _baseInfo._gemMing;
                            att._IsChange = false;
                            m_SaveInfo = att;
                        }
                        else  
                        {
                            att._Gong = _baseInfo._gemGong;
                            att._Fang = _baseInfo._gemFang;
                            att._Ming = _baseInfo._gemMing;
                            att._GongAdd = _baseInfo._gemGong - m_SaveInfo._Gong;
                            att._FangAdd = _baseInfo._gemFang - m_SaveInfo._Fang;
                            att._MingAdd = _baseInfo._gemMing - m_SaveInfo._Ming;
                      
                            m_SaveInfo = att;
                            if (m_SaveInfo._GongAdd == 0 && m_SaveInfo._FangAdd == 0 && m_SaveInfo._MingAdd == 0)
                            {
                                m_SaveInfo._IsChange = false;
                            }
                            else
                            {
                                m_SaveInfo._IsChange = true;
                            }
                        }

                    
                        MaterialsInfoTidy();
                        GemsRedPot(geninfo);
                        m_GemsEquip.ShowInfo(_baseInfo);
                        if (Equip.type == 2 || Equip.type == 4)
                        {
                            ShowChangeInfo(m_SaveInfo);
                        }
                        ShowGemInfo(geninfo);
                        if (Equip.RedPoint!= null)
                        {
 
                            List<int> list = new List<int>();
                            for (int i = 0; i < Equip.RedPoint.Count; i++)
                            {
                                foreach(KeyValuePair<int, BagItem> item in EquipsOfBody.Instance().m_equipsOfBodyDic)
                                {
                                    if (item.Value.dbId == Equip.RedPoint[i])
                                    {
                                        list.Add(item.Key);
                                    }
                                }

                            }
                            ShowEquipTanHao(list);
                        }
                        else
                        {
                            ShowEquipTanHaoFalse();
                        }
                            return true;
                    }
 
            }
        }
        return false;
    }

    void ShowChangeInfo(AddAttribute att_change)
    {
        if (Mathf.Abs(att_change._GongAdd) > 0 && m_GemsEquip.m_DicInfo.ContainsKey(0))
        {
            CreateClone(m_GemsEquip.m_DicInfo[0].gameObject, att_change._GongAdd);
        }

        if (Mathf.Abs(att_change._FangAdd) > 0 && m_GemsEquip.m_DicInfo.ContainsKey(1))
        {
            CreateClone(m_GemsEquip.m_DicInfo[1].gameObject, att_change._FangAdd);

        }

        if (Mathf.Abs(att_change._MingAdd) > 0 && m_GemsEquip.m_DicInfo.ContainsKey(2))
        {
            CreateClone(m_GemsEquip.m_DicInfo[2].gameObject, att_change._MingAdd);
        }
    }
    private void ShowGemInfo(GemsMainInfo geninfo)
    {
        for (int i = 0; i < 5; i++)
        {
            if (geninfo._ListGems[i]._id > 0)
            {
                m_listGemItem[i].m_Add.GetComponent<Collider>().enabled = false;
                m_listGemItem[i].m_GemSprite.gameObject.SetActive(true);
                m_listGemItem[i].m_GemSprite.spriteName = CommonItemTemplate.getCommonItemTemplateById(geninfo._ListGems[i]._id).icon.ToString();
            }
            else if (geninfo._ListGems[i]._isopen)
            {
                m_listGemItem[i].m_Add.GetComponent<Collider>().enabled = true;
                m_listGemItem[i].m_GemSprite.gameObject.SetActive(false);
            }
            else
            {
                m_listGemItem[i].m_Add.GetComponent<Collider>().enabled = false;
                m_listGemItem[i].m_GemSprite.gameObject.SetActive(false);
            }
            if (geninfo._ListGems[i]._id > 0)
            {
                m_listGemItem[i].m_Level.text = geninfo._ListGems[i]._Lvinfo;
            }
            else
            {
                m_listGemItem[i].m_Level.text = "";
            }



            m_listGemItem[i].m_ObjLock.SetActive(!geninfo._ListGems[i]._isopen);
            if (!string.IsNullOrEmpty(geninfo._ListGems[i]._Des))
            {
                m_listGemItem[i].m_labDes.text = geninfo._ListGems[i]._Des;
            }
            else
            {

                m_listGemItem[i].m_labDes.text = "";
            }
            m_listGemItem[i].m_GemDi.spriteName = geninfo._ListGems[i]._Di;
            m_listGemItem[i].m_GemDiKuang.spriteName = geninfo._ListGems[i]._kuang;
        }
    }

    private int _equipIdSave = 0;
    private long _equipDBIDSave = 0;
    public void ShowInfo(GemsMainInfo _Geminfo)
    {
        if (FreshGuide.Instance().IsActive(200030) && TaskData.Instance.m_TaskInfoDic[200030].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 200030;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex++;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
        }
        _equipIdSave = _Geminfo.baseInfo._EquipId;
        _equipDBIDSave = _Geminfo.baseInfo._dbid;
        _baseInfo = _Geminfo.baseInfo;

        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        EquipOperationReq equip = new EquipOperationReq();
        equip.equlpId = _Geminfo.baseInfo._dbid;
        equip.type = 1;
       t_qx.Serialize(t_tream, equip);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
        ShowDetailInfo(_Geminfo.baseInfo);
    }

    public void ShowDetailInfo(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)//界面信息显示
    {
        Equip_Level = baseInfo._Level;
        EquipGrowthMaterialUseManagerment.listCurrentAddExp.Clear();
        EquipGrowthMaterialUseManagerment.m_EuipId = baseInfo._EquipId;
    }
    void MaterialsInfoTidy()//材料信息整理
    {
        Material_Num = 0;
        Material_Num_2 = 0;
        BagCaiLiao = BagData.Instance().m_playerCaiLiaoDic;
        CaiLiaoStrenth.Clear();
        listM.Clear();
        EquipGrowthMaterialUseManagerment.listMaterials.Clear();
        Dictionary<int, BagItem> tempEquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;
        foreach (KeyValuePair<long, List<BagItem>> item in BagCaiLiao)
        {
            for (int i = 0; i < ItemTemp.templates.Count; i++)
            {
                if (item.Value[0].itemId == ItemTemp.templates[i].id/* && ItemTemp.templates[i].quality != 0 */)
                {
                    if (item.Value[0].itemType == 7)
                    {
                        EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                        material.dbid = item.Value[0].dbId;
                        material.materialId = item.Value[0].itemId;
                        material.icon = ItemTemp.templates[i].icon;
                        material.count = item.Value[0].cnt.ToString();
                        material.isSelected = false;
                        material.quality = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).color;
                        material.isTouchControl = isMaxSave;
                        material.inlayColor = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).inlayColor;
                        material.expCurr = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).exp;
                        material.expNeed = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).lvlupExp;
                        material.attribute = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).shuxing;
                        material.materialEXP = item.Value[0].qiangHuaExp;
                        CaiLiaoStrenth.Add(material);
                    }
                }
            }
        }
        QualitySort(CaiLiaoStrenth);
    }
    void QualitySort(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//按品质排序
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1 - i; j++)
            {
                EquipGrowthMaterialUseManagerment.MaterialInfo mInfo;
                if (list[j].inlayColor > list[j + 1].inlayColor)
                {
                    mInfo = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = mInfo;
                }
                else if (list[j].inlayColor == list[j + 1].inlayColor)
                {
                    if (list[j].expCurr > list[j + 1].expCurr)
                    {
                        mInfo = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = mInfo;
                    }
                }
            }
        }
        DestroyChild(list);


        if (_gemsCurrTouchInfo._id > 0)
        {
            FreshMaterials(FuWenTemplate.GetFuWenTemplateByFuWenId(_gemsCurrTouchInfo._id).shuxing);
        }
    }
 
    List<EquipGrowthMaterialUseManagerment.MaterialInfo> listM = new List<EquipGrowthMaterialUseManagerment.MaterialInfo>();
    int CreateCount = 0;
    void DestroyChild(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)//新创建子物体
    {
        int sizeSend = list.Count;
        for (int i = 0; i < sizeSend; i++)
        {
            listM.Add(list[i]);
        }
        ShowMaterialInfo();
    }

    void ShowMaterialInfo()
    {
        if (m_GemsGrid.transform.childCount == 0)
        {
            listInfoo.Clear();
            index_Num2 = 0;
            IsShowOn = false;
            for (int i = 0; i < 20; i++)
            {
                MaterialInfo mInfo = new MaterialInfo();
                mInfo.materialid = 0;
                mInfo.materialdbid = 0;
                mInfo.texture = "";
                mInfo.count = "";
                mInfo.pinzhi = 200;
                listInfoo.Add(mInfo);
            }
            int sizeall = listInfoo.Count;
            for (int i = 0; i < sizeall; i++)
            {
                CreateIcon();
            }
        }
        else
        {
            ShowMaterialInfos(listM);
        }
    }

   
    void ShowMaterialInfos(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)
    {
        int size = m_GemsGrid.transform.childCount;
        if (list.Count > 0)
        {
            if (size >= list.Count)
            {

                for (int i = Material_Num; i < size; i++)
                {
                    if (i < list.Count)
                    {
                        Material_Num++;
                        Transform tran = m_GemsGrid.transform.GetChild(i);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconByID(list[i].materialId, list[i].count.ToString(), 4);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconPopText(list[i].materialId,
                           NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(list[i].materialId).nameId),
                           DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(list[i].materialId).descId));
                        tran.localScale = Vector3.one * 0.85f;

                    }
                    else
                    {
                        {
                            Transform tran = m_GemsGrid.transform.GetChild(i);
                            m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconByID(-1);
                            tran.localScale = Vector3.one * 0.85f;
                        }
                        //m_GemsGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(0, "", "", isMaxSave, 200, null);
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
                        Transform tran = m_GemsGrid.transform.GetChild(i);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconByID(list[i].materialId, list[i].count.ToString(), 4);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconPopText(list[i].materialId,
                           NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(list[i].materialId).nameId),
                           DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(list[i].materialId).descId));
                        tran.localScale = Vector3.one * 0.85f;
                    }
                    else
                    {
                        MaterialInfo mInfo = new MaterialInfo();
                        mInfo.materialid = list[i].materialId;
                        mInfo.materialdbid = list[i].dbid;
                        mInfo.texture = list[i].icon;
                        mInfo.count = list[i].count;
                        mInfo.pinzhi = list[i].quality;
                        listInfoo.Add(mInfo);
                    }
                }
                int size_add = listInfoo.Count;
                if (size_add - 1 > index_Num)
                {
                    index_Num++;
                    for (int i = index_Num; i < size_add; i++)
                    {
                      CreateIcon();
                    }
                }
            }
        }
        else
        {
            _DicMaterial.Clear();
            int s_child = m_GemsGrid.transform.childCount;
            if (s_child > 20)
            {
                for (int i = 0; i < s_child; i++)
                {
                    if (i < 20)
                    {
                        Transform tran = m_GemsGrid.transform.GetChild(i);
                        m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconByID(-1);
                        tran.localScale = Vector3.one * 0.85f;
                    }
                    else
                    {
                        Destroy(m_GemsGrid.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    Transform tran = m_GemsGrid.transform.GetChild(i);
                    m_GemsGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SetIconByID(-1);
                    tran.localScale = Vector3.one * 0.85f;
                }
            }
        }
    }

    void OverStepTip(int index)
    {
        if (index > 0)
        {
            if (FuWenTemplate.GetMaxLevelByAddExp(_GemSelectId, 
                EquipGrowthMaterialUseManagerment.m_TotalAddExp, 
                EquipGrowthMaterialUseManagerment.m_GemLevelsaved) 
                >= FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
            {
                CreateMove(m_GemDetailInfo.m_LabelProgress.gameObject, "已达到最高阶");
            }
            else if (EquipGrowthMaterialUseManagerment.m_GemLevelsaved >= FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
            {
                CreateMove(m_GemDetailInfo.m_LabelProgress.gameObject, "已达到最高阶");
            }
            else
            {
                ClientMain.m_UITextManager.createText(LanguageTemplate.GetText(LanguageTemplate.Text.EQUIP_OVERSTEP_LEVEL));
            }
        }
        else
        {
            CreateMove(m_GemDetailInfo.m_LabelProgress.gameObject, "已达到最高阶");
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
                   
                    current = EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP 
                            + EquipGrowthMaterialUseManagerment.listMaterials[i].expCurr;
                }
            }
        }
        CreateCurrent(current);
    }

    void CreateCurrent(int content)
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
            if (FuWenTemplate.GetMaxLevelByAddExp(_GemSelectId, 
                EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.m_GemLevelsaved) 
                >= FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;

            }
            else if (FuWenTemplate.GetMaxLevelByAddExp(_GemSelectId, 
                EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.m_GemLevelsaved) 
                < FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
            {
                EquipGrowthMaterialUseManagerment.touchIsEnable = true;
            }

           EquipGrowthMaterialUseManagerment.ReduceUseMaterials(EquipGrowthMaterialUseManagerment.m_MaterialId);

            ProcessReduce(content);
            CreateClone(m_GemDetailInfo.m_LabelProgress.gameObject, content * -1);
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

            if (FuWenTemplate.GetMaxLevelByAddExp(_GemSelectId,
                EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.m_GemLevelsaved)
                >= FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
            {
                EquipGrowthMaterialUseManagerment.m_IsSurpassLimited = true;
                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                if (FuWenTemplate.GetMaxLevelByAddExp(_GemSelectId, 
                    EquipGrowthMaterialUseManagerment.m_TotalAddExp, EquipGrowthMaterialUseManagerment.m_GemLevelsaved) 
                    >= FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
                {
                    CreateMove(m_GemDetailInfo.m_LabelProgress.gameObject, "已达到最高阶");
                }
            }
            _levelAdd = Equip_Level;

            CreateClone(m_GemDetailInfo.m_LabelProgress.gameObject, content);
            ProcessAddEffect();
        }
    }


    int _levelAdd = 0;
    void ProcessAddEffect()
    {
        _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;

        _levelReduce = 0;
        FuWenTemplate.GetUpgradeMaxLevel_ByExpidLevel(_GemSelectId,
            EquipGrowthMaterialUseManagerment.m_GemLevel, curr_residue);
        lastcontent = curr_residue;

        int size = FuWenTemplate.m_listNeedInfo.Count;
        int sum_All = 0;
        int judgeIndex = 0;
        int fewardLevel = 0;
        for (int i = 0; i < size; i++)
        {
            if (i < size - 1 && FuWenTemplate.m_listNeedInfo[i].needExp > 0)
            {
                sum_All += FuWenTemplate.m_listNeedInfo[i].needExp;

            }
            else
            {
                judgeIndex = i;
            }

            if (i == size - 1)
            {
                EquipGrowthMaterialUseManagerment.m_GemLevel = FuWenTemplate.m_listNeedInfo[i].level;
            }
        }
        lastExpAll = sum_All;
        if (curr_residue - sum_All >= 0)
        {
            curr_residue -= sum_All;
        }

        curr_Max = FuWenTemplate.m_listNeedInfo[size - 1].needExp;

        if (EquipGrowthMaterialUseManagerment.m_GemLevel > _levelAdd && EquipGrowthMaterialUseManagerment.m_GemLevel < FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
        {
            m_GemDetailInfo.m_objArrow.SetActive(true);
            m_GemDetailInfo.m_LabelLevel.text = EquipGrowthMaterialUseManagerment.m_GemLevel.ToString() + "阶                 "
                                            + MyColorData.getColorString(4, (EquipGrowthMaterialUseManagerment.m_GemLevel + 1).ToString() + "阶");
            _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
        }
        else if (EquipGrowthMaterialUseManagerment.m_GemLevel == FuWenTemplate.GetFuWenTemplateByFuWenId(_GemSelectId).levelMax)
        {
            m_GemDetailInfo.m_LabelLevel.text = EquipGrowthMaterialUseManagerment.m_GemLevel.ToString() + "阶" ;
            _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
            m_GemDetailInfo.m_objArrow.SetActive(false);
        }
        else
        {
            _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
            _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
        }

        if (curr_Max < 0)
        {
            m_GemDetailInfo.m_PregressBar.value = 1;
            m_GemDetailInfo.m_LabelProgress.text = "";

        }
        else
        {
            m_GemDetailInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_GemDetailInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
    }

    int _levelReduce = 0;
    void ProcessReduce(int content)
    {
        if (curr_residue >= content)
        {
            curr_residue -= content;
 
            if (EquipGrowthMaterialUseManagerment.m_GemLevel < _levelReduce)
            {
                m_GemDetailInfo.m_objArrow.SetActive(true);
                m_GemDetailInfo.m_LabelLevel.text = EquipGrowthMaterialUseManagerment.m_GemLevel.ToString() + "阶                 "
                                        + MyColorData.getColorString(4, (EquipGrowthMaterialUseManagerment.m_GemLevel + 1).ToString() + "阶");
                _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
            }
            m_GemDetailInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_GemDetailInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
        }
        else
        {
            FuWenTemplate.GetReduceMaxLevel_ByExpidLevel(_GemSelectId, content,
                EquipGrowthMaterialUseManagerment.m_GemLevel, EquipGrowthMaterialUseManagerment.Levelsaved);

            int size = FuWenTemplate.m_listReduceInfo.Count;
            int reduceIndex = 0;
            int sum_Reduce = curr_residue;
            for (int i = 0; i < size; i++)
            {
                sum_Reduce += FuWenTemplate.m_listReduceInfo[i].needExp;
                if (sum_Reduce >= content)
                {
                    reduceIndex = i;
                    break;
                }
            }
            curr_residue = sum_Reduce - content;
            curr_Max = FuWenTemplate.m_listReduceInfo[reduceIndex].needExp;
            int level = EquipGrowthMaterialUseManagerment.m_GemLevel;
            EquipGrowthMaterialUseManagerment.m_GemLevel = FuWenTemplate.m_listReduceInfo[reduceIndex].level;

            if (EquipGrowthMaterialUseManagerment.m_GemLevel < _levelReduce)
            {
                m_GemDetailInfo.m_objArrow.SetActive(true);
                _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
                m_GemDetailInfo.m_LabelLevel.text = EquipGrowthMaterialUseManagerment.m_GemLevel.ToString() + "阶                 "
                                           + MyColorData.getColorString(4, (EquipGrowthMaterialUseManagerment.m_GemLevel + 1).ToString() + "阶");
            }
            else
            {
                _levelAdd = EquipGrowthMaterialUseManagerment.m_GemLevel;
                _levelReduce = EquipGrowthMaterialUseManagerment.m_GemLevel;
            }
            m_GemDetailInfo.m_PregressBar.value = curr_residue / float.Parse(curr_Max.ToString());
            m_GemDetailInfo.m_LabelProgress.text = curr_residue.ToString() + "/" + curr_Max.ToString();
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
    int index_num = 0;
    void CreateIcon()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
    }
    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GemsGrid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_GemsGrid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            if (listInfoo[index_num].materialid != 0)
            {
                iconSampleManager.SetIconByID(listInfoo[index_num].materialid, listInfoo[index_num].count.ToString(), 4);
                iconSampleManager.SetIconPopText(listInfoo[index_num].materialid,
                   NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(listInfoo[index_num].materialid).nameId),
                   DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(listInfoo[index_num].materialid).descId));
                iconSampleManager.transform.localScale = Vector3.one * 0.85f;
            }
            iconSampleManager.transform.localScale = Vector3.one * 0.85f;
            if (index_num < listInfoo.Count - 1)
            {
                index_num++;
            }
            else
            {
                if (!IsShowOn)
                {
                    IsShowOn = true;

                    ShowMaterialInfos(listM);
                }
            }
            m_GemsGrid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    void ShowNeedMaterialInfo()
    {
        if (m_GemsNeedGrid.transform.childCount == 0)
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
                listShowGems.Add(mInfo);
            }
            int sizeall = listShowGems.Count;
            for (int i = 0; i < sizeall; i++)
            {
                CreateItems();
            }
        }
        else
        {
            ShowNeedMaterialInfos(CaiLiaoInfo);
        }
    }
    void ShowNeedMaterialInfos(List<EquipGrowthMaterialUseManagerment.MaterialInfo> list)
    {
        int size = m_GemsNeedGrid.transform.childCount;
        if (list.Count > 0)
        {
            if (size > list.Count)
            {
                for (int i = Material_Num_2; i < size; i++)
                {
                    if (i < list.Count)
                    {
                        Material_Num_2++;
                        if (_DicMaterial.ContainsKey(list[i].dbid))
                        {
                            _DicMaterial[list[i].dbid] = m_GemsNeedGrid.transform.GetChild(i).gameObject;
                        }
                        else
                        {
                            _DicMaterial.Add(list[i].dbid, m_GemsNeedGrid.transform.GetChild(i).gameObject);
                        }
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().m_IntenseShow = true;
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        minfo._dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        minfo._icon = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        minfo._count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        minfo._pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, OverStepTip);
                    }
                    else
                    {
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = 0;
                        minfo._dbid = 0;
                        minfo._icon = "";
                        minfo._count = "";
                        minfo._pinzhi = 200;
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
                    }
                }
            }
            else
            {
                for (int i = Material_Num_2; i < list.Count; i++)
                {
                    Material_Num_2++;
                    if (i < size)
                    {
                        if (_DicMaterial.ContainsKey(list[i].dbid))
                        {
                            _DicMaterial[list[i].dbid] = m_GemsNeedGrid.transform.GetChild(i).gameObject;
                        }
                        else
                        {
                            _DicMaterial.Add(list[i].dbid, m_GemsNeedGrid.transform.GetChild(i).gameObject);
                        }

                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().m_IntenseShow = true;
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<IconSampleManager>().SubButton.SetActive(false);
                        EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
                        minfo._itemid = EquipGrowthMaterialUseManagerment.listMaterials[i].materialId;
                        minfo._dbid = EquipGrowthMaterialUseManagerment.listMaterials[i].dbid;
                        minfo._icon = EquipGrowthMaterialUseManagerment.listMaterials[i].icon;
                        minfo._count = EquipGrowthMaterialUseManagerment.listMaterials[i].count;
                        minfo._pinzhi = EquipGrowthMaterialUseManagerment.listMaterials[i].quality;
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, OverStepTip);
                    }
                    else
                    {
                        MaterialInfo mInfo = new MaterialInfo();
                        mInfo.materialid = list[i].materialId;
                        mInfo.texture = list[i].icon;
                        mInfo.count = list[i].count;
                        mInfo.pinzhi = list[i].quality;
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
            int s_child = m_GemsNeedGrid.transform.childCount;
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
                        m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
                    }
                    else
                    {
                        Destroy(m_GemsNeedGrid.transform.GetChild(i).gameObject);
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
                       m_GemsNeedGrid.transform.GetChild(i).GetComponent<EquipGrowthMaterialItem>().ShowMaterialInfo(minfo, isMaxSave, null);
                }
            }

        }
    }

    void CreateItems()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), IconSampleLoadCallBack);
    }
    int index_Num2 = 0;
    bool IsShowOn = false;
    public void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GemsNeedGrid != null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_GemsNeedGrid.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            EquipGrowthMaterialItem equipGrowthMaterialItem = iconSampleObject.GetComponent<EquipGrowthMaterialItem>() ?? iconSampleObject.AddComponent<EquipGrowthMaterialItem>();
            equipGrowthMaterialItem.IconSampleManager = iconSampleManager;
            if (listShowGems[index_Num2].materialid != 0)
            {
                iconSampleObject.GetComponent<IconSampleManager>().SubButton.SetActive(false);
            }
            EquipGrowthMaterialItem.MaterialNeed minfo = new EquipGrowthMaterialItem.MaterialNeed();
            minfo._itemid = listShowGems[index_Num2].materialid;
            minfo._dbid = listShowGems[index_Num2].materialdbid;
            minfo._icon = listShowGems[index_Num2].texture;
            minfo._count = listShowGems[index_Num2].count;
            minfo._pinzhi = listShowGems[index_Num2].pinzhi;
            equipGrowthMaterialItem.ShowMaterialInfo(minfo, isMaxSave,OverStepTip);
            if (listShowGems[index_Num2].materialdbid != 0)
            {
                _DicMaterial.Add(listShowGems[index_Num2].materialdbid, iconSampleObject);
            }
            if (index_Num2 < listShowGems.Count - 1)
            {
                index_Num2++;
            }
            else if (index_Num2 == listShowGems.Count - 1)
            {
               ShowNeedMaterialInfos(CaiLiaoInfo);
            }
            m_GemsNeedGrid.repositionNow = true;
        }
        else
        {
            p_object = null;
        }
    }

    int index_list = 0;
    public void Select_item_LoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_GemsNeedlist!= null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.SetActive(true);
            iconSampleObject.transform.parent = m_GemsNeedlist.transform;
            iconSampleObject.transform.localPosition = Vector3.zero;
            EquipGrowthInlayLayerManagerment.GemSelectInfo gemInfo = new EquipGrowthInlayLayerManagerment.GemSelectInfo();
            gemInfo._Gemid = CaiLiaoInfo[index_list].materialId;
            gemInfo._dbid = CaiLiaoInfo[index_list].dbid;
            gemInfo._Count = CaiLiaoInfo[index_list].count;
            gemInfo._Exp = CaiLiaoInfo[index_list].expCurr;
            iconSampleObject.GetComponent<EquipGrowthGemSelectItemManagerment>().ShowInfo(gemInfo, Inlay);
            iconSampleObject.transform.localScale = Vector3.one;
            if (index_list < CaiLiaoInfo.Count - 1)
            {
                index_list++;
            }
            else
            {
                m_GemsNeedlist.repositionNow = true;
         
            }
        }
        else
        {
            p_object = null;
        }
    }
    /*

       message EquipOperationReq{
   //	type = 1：请求装备上的宝石信息
   //	type = 2：请求一键镶嵌
   //	type = 3：请求一键拆解 
   //	type = 4：请求镶嵌一颗宝石
   //	type = 5：请求拆解一颗宝石
   //	type = 6：请求宝石合成
   required int32 type = 1 ;		//请求类型
   required int64 equlpId = 2 ;	//所请求的装备dbid
   optional int64 jewelId = 3 ;	//所请求的宝石dbid，请求镶嵌时需要发送
   optional int32 possionId = 4 ;	//所请求的宝石孔的id，请求镶嵌、拆下、合成时需要发送
   repeated int64 cailiaoList = 5;	//所请求的材料列表，宝石合成的时候发送
}
   */
    void Inlay(long inlay_id)
    {
        if (FreshGuide.Instance().IsActive(200030) && TaskData.Instance.m_TaskInfoDic[200030].progress >= 0)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        m_ObjGemlist.SetActive(false);
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        EquipOperationReq equip = new EquipOperationReq();
        equip.equlpId = _equipDBIDSave;
        equip.jewelId = inlay_id;
        equip.possionId = _posNum;
        equip.type = 4;
        t_qx.Serialize(t_tream, equip);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_BAOSHI_REQ, ref t_protof);
    }

    void Strengthen()// 合成升一级ID统计
    {
        int GemExpAdd = 0;
        int GemExp = currSave;
        int sumAll = 0;
        int size = EquipGrowthMaterialUseManagerment.listMaterials.Count;
        int size_add = EquipGrowthMaterialUseManagerment.listTouchedId.Count;
        for (int i = 0; i < size_add; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (EquipGrowthMaterialUseManagerment.listMaterials[j].dbid
                    == EquipGrowthMaterialUseManagerment.listTouchedId[i])
                {
                    GemExp += EquipGrowthMaterialUseManagerment.listMaterials[j].materialEXP 
                           + EquipGrowthMaterialUseManagerment.listMaterials[j].expCurr;
                    break;
                }
            }
        }
        GemExpAdd = GemExp;
        if (!NeedIsEnought(GemExp, _GemSelectId))
        {
            for (int i = 0; i < size; i++)
            {
                if (GetNowMaterialAddCount(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid)
                    < int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count))
                {
                    for (int j = GetNowMaterialAddCount(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid);
                         j < int.Parse(EquipGrowthMaterialUseManagerment.listMaterials[i].count); j++)
                    {
                        GemExp += EquipGrowthMaterialUseManagerment.listMaterials[i].materialEXP
                               + EquipGrowthMaterialUseManagerment.listMaterials[i].expCurr;
                        EquipGrowthMaterialUseManagerment.listTouchedId.Add(EquipGrowthMaterialUseManagerment.listMaterials[i].dbid);

                        if (NeedIsEnought(GemExp, _GemSelectId))
                        {
                            if (GemExp - GemExpAdd > 0)
                            {
                                CreateCurrent(GemExp - GemExpAdd);
                                ShowAddNum();
                            }
                            ClientMain.m_UITextManager.createText("所需经验已满");
                            return;
                        }
                    }
                }
               
            }

            if (GetAllMaterialAddCount() == EquipGrowthMaterialUseManagerment.listTouchedId.Count)
            {
                if (GemExp - GemExpAdd > 0)
                {
                    CreateCurrent(GemExp - GemExpAdd);
                    ShowAddNum();
                }
                ClientMain.m_UITextManager.createText("已添加所有宝石");
                return;
                
            }
        }
        else
        {
            if (GetAllMaterialAddCount() == EquipGrowthMaterialUseManagerment.listTouchedId.Count)
            {
                ClientMain.m_UITextManager.createText("已添加所有宝石");
            }
            else
            {
                ClientMain.m_UITextManager.createText("所需经验已满");
            }


        }
        if (GemExp - GemExpAdd > 0)
        {
            CreateCurrent(GemExp - GemExpAdd);
            ShowAddNum();
        }
    
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


    private bool NeedIsEnought(int add_exp, int gemid)
    {
        if ( add_exp >= FuWenTemplate.GetFuWenTemplateByFuWenId(gemid).lvlupExp)
        {
            return true;
        }
        return false;
    }
    void GemsRedPot(GemsMainInfo geninfo)
    {
        for (int i = 0; i < 5; i++)
        {
            if(geninfo._ListGems[i]._isopen && geninfo._ListGems[i]._id == 0)
            {
                m_listGemItem[i].m_ObjRedPot.SetActive(WetherContainSuitedGem(ZhuangBei.getZhuangBeiById(geninfo.baseInfo._EquipId).inlayColor));
            }
            else
            {
                m_listGemItem[i].m_ObjRedPot.SetActive(false);
            }
        }
    }

    private bool WetherContainSuitedGem(int attribute_id)
    {
        for (int i = 0; i < CaiLiaoStrenth.Count; i++)
        {
            if (attribute_id == CaiLiaoStrenth[i].attribute)
            {
                return true;
            }
        }
        return false;
    }

    private int GetEquipContainGemsCount()
    {
        int sum = 0;
        int size = geninfo._ListGems.Count;
        for (int i = 0; i < size; i++)
        {
            if (geninfo._ListGems[i]._id > 0)
            {
                sum++;
            }
        }
        return sum;
    }

    private string AddType(int index)
    {
        switch (index)
        {
            case 1:
                {
                    return "攻击";
                }
                break;
            case 2:
                {
                    return "防御";
                }
                break;
            case 3:
                {
                    return "生命";
                }
                break;
        }
        return "";
    }

    public void ShowEquipTanHao(List<int> list_index)
    {
        int _size = m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent.Count;
 
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < list_index.Count; j++)
            {
                if (list_index[j] == i)
                {
                    m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(true);
                }
                else
                {
                    m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(false);
                } 
            }
            
        }
    }
    public void ShowEquipTanHaoFalse( )
    {
        int _size = m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent.Count;
        for (int i = 0; i < _size; i++)
        {
          m_EquipGrowthEquipInfoManagerment.m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[i].m_ObjTanHao.SetActive(false);
        }
    }

    void SwitchGem()
    {
        foreach (KeyValuePair<long, List<BagItem>> item in BagCaiLiao)
        {
            for (int i = 0; i < ItemTemp.templates.Count; i++)
            {
                if (item.Value[0].itemId == ItemTemp.templates[i].id/* && ItemTemp.templates[i].quality != 0 */)
                {
                    if (item.Value[0].itemType == 7)
                    {
                        EquipGrowthMaterialUseManagerment.MaterialInfo material = new EquipGrowthMaterialUseManagerment.MaterialInfo();
                        material.dbid = item.Value[0].dbId;
                        material.materialId = item.Value[0].itemId;
                        material.icon = ItemTemp.templates[i].icon;
                        material.count = item.Value[0].cnt.ToString();
                        material.isSelected = false;
                        material.quality = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).color;
                        material.isTouchControl = isMaxSave;
                        material.inlayColor = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).inlayColor;
                        material.expCurr = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).exp;
                        material.expNeed = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).lvlupExp;
                        material.attribute = FuWenTemplate.GetFuWenTemplateByFuWenId(item.Value[0].itemId).shuxing;
                        material.materialEXP = item.Value[0].qiangHuaExp;
                        CaiLiaoStrenth.Add(material);
                    }
                }
            }
        }
    }
}
