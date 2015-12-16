using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipGrowthEquipInfoManagerment : MonoBehaviour, SocketProcessor
{
    public UILabel m_TotalGold;
    private static GameObject IconSamplePrefab;
    public static bool m_WetherIsIntensify = false;
    public GameObject m_IntensifyTanHao;
    public GameObject m_WashTanHao;
    public UILabel m_labelIntensify;


    public GameObject m_ParentInfo;
 
    public GameObject m_SharePart;

    public GameObject m_EquipUpgrade;

    public GameObject m_EquipIntensify;
    public GameObject m_EquipWash;
    public GameObject m_EquipWashObject;

    public GameObject m_MianInfo;

    public int m_AttributeCount;

  //  public UILabel m_LabelMove;

    public GameObject m_objSharePart;
    public UIScrollView m_ScrollView;
    private int EquipSaveId;
 
    private List<string> listNames = new List<string>();
    private int WuSave;
    private int YouSave;
    private int ZhiSave;
    public int BuWeiSave;
    private long DBidSave;

    private int ShowType = 0;
    private long EquipIntensifyId;
    private string Equipname = "";
    private int equipType = 0;
    private int MaxExp;
    private int CurrExp;
    private int StrengthenIndex = 0;
    private EquipStrengthResp EquipInfoSave;
    EquipStrengthResp EquipInfo;
    private List<float> listData = new List<float>();

    private struct EquipAtrrInfo
    {
        public string gong;
        public string fang;
        public string xue;
        public int max;
        public int curr;
        public int pinzhi;
        public int level;
    };
    EquipAtrrInfo ei = new EquipAtrrInfo();

    public struct EquipBaseInfo
    {
        public string _Name;
        public int _Gong;
        public int _Fang;
        public int _Xue;
        public int _Level;
        public string _Progress;
        public string _Icon;
        public string _PinZhi;
        public float _PregressValue;
        public int _AttrCount;
    };
    private EquipBaseInfo _EquipBInfo;
    public EquipGrowthEquipInfoItemManagerment m_EquipItenm = null;
    void Start()
    {
        _listObj.Clear();
        if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 10000)
        {
            m_TotalGold.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000).ToString() + NameIdTemplate.GetName_By_NameId(990051);
        }
        else
        {
            m_TotalGold.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
        }

     //   m_LabelTopUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_SIGNAL);
    }

    public static bool m_isEffect = false;
    private int _CurrenItemNum = 0;
    private float _timeInterval = 0;


    void Update()
    {
        _timeInterval += Time.deltaTime;
        if (_timeInterval >= 0.1f)
        {
            _timeInterval = 0;
            int[] arrange = { 3, 4, 5, 0, 8, 1, 7, 2, 6 };
            if (m_isEffect)
            {
                if (_CurrenItemNum < 9)
                {
                   
                    if (EquipSuoData.m_listEffectInfo.ContainsKey(arrange[_CurrenItemNum]))
                    {
                        m_isEffect = false;
                        m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[EquipSuoData.m_listEffectInfo[arrange[_CurrenItemNum]]._num].MoveLabel(EquipSuoData.m_listEffectInfo[arrange[_CurrenItemNum]]._LevelAdd);
                    }
                    _CurrenItemNum++;
                }
                else
                {
                    foreach (KeyValuePair<int, EquipSuoData.StrengthEffect> iten in EquipSuoData.m_listEffectInfo)
                    {
                        m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[iten.Value._num].m_ObjEffect.SetActive(false);
                    }
                    m_isEffect = false;
                    EquipsInfoTidy(EquipInfo);
                    _CurrenItemNum = 0;
                }
            }
        }
    }
    void OnEnable()
    {
        StrengthenIndex = 0;
        SocketTool.RegisterMessageProcessor(this);
    }

    public void GetEquipInfo(int Equipid, long dbid, int type, int buwei)//请求对应装备信息
    {
        m_AttributeCount = 0;
        DBidSave = dbid;
        BuWeiSave = buwei;
        EquipSaveId = Equipid;
        ShowType = type;
        StrengthenIndex = 0;
        times_index = 0;
        {
            MemoryStream t_tream = new MemoryStream();
            QiXiongSerializer t_qx = new QiXiongSerializer();
            EquipStrengthReq equip = new EquipStrengthReq();
            equip.equipId = dbid;
            equip.type = 2;
            t_qx.Serialize(t_tream, equip);
            byte[] t_protof;
            t_protof = t_tream.ToArray();
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EQUIP_UPGRADE, ref t_protof); 
        }
    }

    private ZhuangBei equipmentShowed = new ZhuangBei();
    public void ShowEquipInfo(ZhuangBei equipment, string gong, string fang, string xue, int max, int curr, int pinzhi, int level)
    {
        equipmentShowed = equipment;
        ei.gong = gong;
        ei.fang = fang;
        ei.xue = xue;
        ei.level = level;
        Equipname = equipment.m_name;

     
        {
            if (_isMaxLevel)
            {
                _isMaxLevel = false;
                if (m_EquipItenm != null)
                {
                    CreateMove(m_EquipItenm.m_LabelProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
                }
            }
          //  m_ProgressBar.value = 1.0f;

           // m_LabPrecess.text = "";
        }
        if (ShowType == 0)//0强化
        {
            m_labelIntensify.gameObject.SetActive(true);
            m_labelIntensify.text = MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.XILIAN_DESC_8));
            m_EquipUpgrade.SetActive(false);
          
            EquipSuoData.Instance().m_WashIson = false;
         //   IntensifyEquipInfoShow();

            m_EquipIntensify.SetActive(true);
            m_EquipWashObject.SetActive(false);
            m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ShowInfo(EquipSaveId, equipType, curr, max, DBidSave, level, BuWeiSave, pinzhi);
        }
        else if (ShowType == 1)//1 洗练
        {
            m_labelIntensify.gameObject.SetActive(false);

            int size_shuxing = m_ParentInfo.transform.childCount;
     
            m_EquipUpgrade.SetActive(false);
            

            EquipSuoData.Instance().m_EquipID = EquipSaveId;
 
        //    WashEquipInfoShow();
            m_EquipIntensify.SetActive(false);
            m_EquipWashObject.SetActive(true);
            m_EquipWash.SetActive(true);
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().m_EquipType = equipType;
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().buttonNum = 0;
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().EquipWash(DBidSave, EquipSaveId, 0, 3, 3, 3, 3, 3, 3, 3, 3, pinzhi);
        }
        //else
        //{
        //    m_EquipIntensify.SetActive(false);
        //    m_EquipWashObject.SetActive(false);
        //   // m_EquipWash.GetComponent<EquipGrowthWashManagerment>().EquipWash(DBidSave, EquipSaveId, 0, 3, 3, 3, 3, 3, 3, 3, 3, pinzhi);
   
        //    ShowUpgradeInfo();
        //}

 
        _EquipBInfo._Name = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(equipment.m_name))); ;
        _EquipBInfo._Gong = int.Parse(gong);
        _EquipBInfo._Fang = int.Parse(fang);
        _EquipBInfo._Xue = int.Parse(xue);
        if (max != -1)
        {
            _EquipBInfo._Progress = curr.ToString() + "/" + max.ToString();
            _EquipBInfo._PregressValue = curr / float.Parse(max.ToString());
        }
        else
        {
            _EquipBInfo._Progress = "";
            _EquipBInfo._PregressValue = 1.0f;
        }
        _EquipBInfo._Icon = equipment.id.ToString();
        _EquipBInfo._PinZhi = QualityIconSelected.SelectQuality(equipment.color);
        _EquipBInfo._AttrCount = EquipSuoData.m_listEquipWash[EquipSaveId].Count;
        if (m_EquipItenm == null)
        {
            LoadEquipItem();
        }
        else
        {
            m_EquipItenm.ShowInfo(_EquipBInfo);
            LoadAttrbuite();
        } 
        ShowLevel(ei.level, 0, 0, 0);
        m_MianInfo.SetActive(true);
        m_SharePart.SetActive(true);
    
    }
    void LoadEquipItem()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIPINFO_ITEM), ResourcesLoadEquipItemCallBack);
    }

    public void ResourcesLoadEquipItemCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_ParentInfo != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_ParentInfo.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.GetComponent<EquipGrowthEquipInfoItemManagerment>().ShowInfo(_EquipBInfo);
            m_EquipItenm = rewardShow.GetComponent<EquipGrowthEquipInfoItemManagerment>();
           // m_TableInfo.repositionNow = true;

            if (ShowType == 0)//0强化
            {
                IntensifyEquipInfoShow();
            }
            else if (ShowType == 1)//1 洗练
            {
                WashEquipInfoShow();
            }
        }
        else
        {
            p_object = null;
        }
       LoadAttrbuite();
    }

    void LoadAttrbuite()
    {
        int size_shuxing = EquipSuoData.m_listEquipWash[EquipSaveId].Count;
        int size = _listObj.Count;
        if (size > 0)
        {
            if (size_shuxing >= size)
            {
                for (int k = 0; k < size_shuxing; k++)
                {
                    if (k < size)
                    {
                        _listObj[k].GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(EquipSuoData.m_listEquipWash[EquipSaveId][k], false, false);
                    }
                    else
                    {
                        if (k == size)
                        {
                            index_ShuXing = k;
                        }
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM), ResourcesLoadCallBack2);
                    }
                }
            }
            else
            {
                for (int k = 0; k < size; k++)
                {
                    if (k < size_shuxing)
                    {
                        _listObj[k].GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(EquipSuoData.m_listEquipWash[EquipSaveId][k], false, false);
                    }
                    else
                    {
                        Destroy(_listObj[k]);
                    }
                }

                for (int k = 0; k < size - size_shuxing; k++)
                {
                    _listObj.RemoveAt(_listObj.Count - 1);
                }
            }
        }
        else
        {
            index_ShuXing = 0;
            for (int k = 0; k < size_shuxing; k++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM), ResourcesLoadCallBack2);
            }
        }
    }
    private readonly Vector3 BasicIconPos = new Vector3(-110, 0, 0);
    private const int BasicIconDepth = 10;
    private List<int> _Show_Gong = new List<int>();
    private List<int> _Show_Xue = new List<int>();
    private List<int> _Show_Fang = new List<int>();
    public void ShowLevel(int index, int gong, int fang, int xue)
    {

        m_EquipItenm.m_LabelLevel.text = MyColorData.getColorString(4, " +" + index.ToString());
        if (ShowType == 0)
        {
            if (gong > 0)
            {
                CreateClone(m_EquipItenm.m_LabelGong.gameObject, gong);
 
                int gg = int.Parse(ei.gong) + gong;
                ei.gong = gg.ToString();

                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, gg.ToString());
            }
            else
            {
                if (gong < 0)
                {
                    CreateClone(m_EquipItenm.m_LabelGong.gameObject, gong);
                }

                int gg = int.Parse(ei.gong) + gong;
                ei.gong = gg.ToString();
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, gg.ToString());
            }

            if (fang > 0)
            {
                int ff = int.Parse(ei.fang) + fang;
                ei.fang = ff.ToString();
                int xx = int.Parse(ei.xue) + xue;
        
                ei.xue = xx.ToString();
                CreateClone(m_EquipItenm.m_LabelXue.gameObject, xue);
                CreateClone(m_EquipItenm.m_LabelFang.gameObject, fang);
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ff.ToString());
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, xx.ToString());// +MyColorData.getColorString(4, "+" + xue);
            }
            else
            {
                if (xue < 0)
                {
                    CreateClone(m_EquipItenm.m_LabelXue.gameObject, xue);
                    CreateClone(m_EquipItenm.m_LabelFang.gameObject, fang);
                }
                int ff = int.Parse(ei.fang) + fang;
                ei.fang = ff.ToString();
                int xx = int.Parse(ei.xue) + xue;
                ei.xue = xx.ToString();

                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ff.ToString());
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, xx.ToString());
            }
        }
    }

    void IntensifyEquipInfoShow()//强化装备信息显示
    {
        if (BuWeiSave == (int)EquipPositionEnum.QingWuQi || BuWeiSave == (int)EquipPositionEnum.ZhongWuQi || BuWeiSave == (int)EquipPositionEnum.Gong)
        {
            equipType = 1;

            if (EquipInfo.gongJiAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, ei.gong);
            }
            else
            {
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, ei.gong);
                CreateClone(m_EquipItenm.m_LabelGong.gameObject, EquipInfo.gongJiAdd);
            }

            m_EquipIntensify.SetActive(true);
        }
        else
        {
            equipType = 0;
           

            if (EquipInfo.fangYuAdd == 0 && EquipInfo.shengMingAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);
            }
            else
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);

                CreateClone(m_EquipItenm.m_LabelFang.gameObject, EquipInfo.fangYuAdd);
                CreateClone(m_EquipItenm.m_LabelXue.gameObject, EquipInfo.shengMingAdd);
            }
        }
    }


    void WashEquipInfoShow()//洗练装备信息显示
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            EquipSuoData.Instance().m_WashIson = true;
            if (EquipInfo.gongJiAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, ei.gong);
            }
            else
            {
                CreateClone(m_EquipItenm.m_LabelGong.gameObject, EquipInfo.gongJiAdd);
            }
        }
        else
        {
            equipType = 0;
            
            EquipSuoData.Instance().m_WashIson = true;

            if (EquipInfo.fangYuAdd == 0 && EquipInfo.shengMingAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);
            }
            else
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);

                CreateClone(m_EquipItenm.m_LabelFang.gameObject, EquipInfo.shengMingAdd);
                CreateClone(m_EquipItenm.m_LabelXue.gameObject, EquipInfo.fangYuAdd);
            }

            m_EquipWashObject.SetActive(true);
        }
    }
 
    int times_index = 0;
    bool _isMaxLevel = false;
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {

                case ProtoIndexes.S_EQUIP_UPGRADE://返回需要强化的装备装备信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipStrengthResp Equip = new EquipStrengthResp();
                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());
                        EquipInfo = Equip;
 
                        EquipGrowthMaterialUseManagerment.listTouchedId.Clear();
                        EquipInfo.gongJiAdd = 0;
                        EquipInfo.fangYuAdd = 0;
                        EquipInfo.shengMingAdd = 0;
                        if (ShowType == 0)
                        {
                            m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ShowEquipTanHao();
                            if (!m_WetherIsIntensify)
                            {
                                EquipInfoSave = Equip;
                            }
                            else
                            {
                                UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EquipItenm.m_SpritePinZhi.gameObject, EffectIdTemplate.GetPathByeffectId(100182), null);
                                m_WetherIsIntensify = false;
                                CreateMove(m_EquipItenm.m_LabelSuccess.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_SUCCESS));
                                if (EquipInfo.expMax == -1)
                                {
                                    _isMaxLevel = true;
                                }
                                EquipInfo.gongJiAdd = EquipInfo.gongJi - EquipInfoSave.gongJi;
                                EquipInfo.fangYuAdd = EquipInfo.fangYu - EquipInfoSave.fangYu;
                                EquipInfo.shengMingAdd = EquipInfo.shengMing - EquipInfoSave.shengMing;
                                EquipInfoSave = Equip;
                            }
                        }
           
                        if (Equip.level < JunZhuData.Instance().m_junzhuInfo.level)
                        {
                            EquipGrowthMaterialUseManagerment.touchIsEnable = true;
                        }
                        else
                        {
                            EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                        }
                       m_IntensifyTanHao.SetActive(AllIntensify());
                       m_WashTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(1210)&& FunctionOpenTemp.GetWhetherContainID(1210));
                        PushAndNotificationHelper.SetRedSpotNotification(1212, AllIntensify());  
                        EquipsInfoTidy(EquipInfo);
                        return true;
                    }
                case ProtoIndexes.S_EQUIP_UPALLGRADE://一键强化返回信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipStrength4AllResp Equip = new EquipStrength4AllResp();
                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());
 
                        if (Equip.allResp != null)
                        {
                            EquipSuoData.m_listEffectInfo.Clear();
                            for (int i = 0; i < Equip.allResp.Count; i++)
                            {

                                if (Equip.allResp[i].level > EquipSuoData.m_equipsLevelSave[EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei)])
                                {
                                    EquipSuoData.StrengthEffect sf = new EquipSuoData.StrengthEffect();
                                    sf._num = EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei);
                                    sf._isshow = true;
                                    sf._LevelAdd = Equip.allResp[i].level - EquipSuoData.m_equipsLevelSave[EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei)];
                                    EquipSuoData.m_listEffectInfo.Add(EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei), sf);
                                }

                                if (Equip.allResp[i].zhuangbeiID == EquipSaveId)
                                {
                                    StrengthenIndex = 0;
                                    EquipInfo = Equip.allResp[i];
                                }
                            }
                            EquipInfo.gongJiAdd = EquipInfo.gongJi - EquipInfoSave.gongJi;
                            EquipInfo.fangYuAdd = EquipInfo.fangYu - EquipInfoSave.fangYu;
                            EquipInfo.shengMingAdd = EquipInfo.shengMing - EquipInfoSave.shengMing;
                            EquipInfoSave = EquipInfo;

                            m_IntensifyTanHao.SetActive(AllIntensify());
                   
                            if (EquipInfo.level < JunZhuData.Instance().m_junzhuInfo.level)
                            {
                                EquipGrowthMaterialUseManagerment.touchIsEnable = true;
                            }
                            else
                            {
                                EquipGrowthMaterialUseManagerment.touchIsEnable = false;
                            }

                            foreach (KeyValuePair<int, EquipSuoData.StrengthEffect> iten in EquipSuoData.m_listEffectInfo)
                            {
                                m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[iten.Value._num].m_ObjEffect.SetActive(true);
                            }
                            m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ButtonShow();
                            m_isEffect = true;
                        }
                        return true;
                    }
                    
            }
        }
        return false;
    }

    void OnDisable()
    {
        //m_LabName.gameObject.SetActive(false);
        //m_LabLevel.gameObject.SetActive(false);
        //m_EquipIntensify.SetActive(false);
        SocketTool.UnRegisterMessageProcessor(this);
    }
    private List<EquipSuoData.WashInfo> _listAttribute;
    private void EquipsInfoTidy(EquipStrengthResp esr)//分离数据并赋值
    {
        _listAttribute = new List<EquipSuoData.WashInfo>();
        listData.Clear();
 
        if (EquipSuoData.m_listEquipWash.ContainsKey(esr.zhuangbeiID))
        {
            EquipSuoData.m_listEquipWash.Remove(esr.zhuangbeiID);
        }
        EquipSuoData.Instance().listIndexs.Clear();
        Dictionary<int, BagItem> EquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;


        EquipGrowthMaterialUseManagerment.equipLevel = esr.level;
        EquipGrowthMaterialUseManagerment.Levelsaved = esr.level;
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                float[] attribute = { float.Parse(esr.wqSH.ToString()), float.Parse(esr.wqJM.ToString()), float.Parse(esr.wqBJ.ToString())
                , float.Parse(esr.wqRX.ToString()), float.Parse(esr.jnSH.ToString()), float.Parse(esr.jnJM.ToString())
                , float.Parse(esr.jnBJ.ToString()), float.Parse(esr.jnRX.ToString())
                ,esr.wqBJL, esr.jnBJL, esr.wqMBL, esr.jnMBL, esr.jnRX };
               // int[] attribute = { esr.wqSH, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };
               // int[] attribute_Max = { esr.wqSHMA, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };
                listData.AddRange(attribute);
                for (int j = 0; j < listData.Count; j++)
                {
                    if (listData[j] > 0)
                    {
                        EquipSuoData.WashInfo wss = new EquipSuoData.WashInfo();
                        if (j > 3)
                        {
                            wss._type = 1;
                        }
                        else
                        {
                            wss._type = 0;
                        }
                        //if (j < attribute_Max.Length)
                        //{
                        //    Debug.Log(" listData[j] listData[j] listData[j] ::" + listData[j]);
                        //    Debug.Log(" attribute_Max[j]attribute_Max[j]attribute_Max[j] ::" + attribute_Max[j] + "JJJJJJJJJJJJJJJJ  :" + j );
                        //    wss._isMax = listData[j] == attribute_Max[j] ? true : false;
                        //}
                        wss._num = j;
                        wss._nameid = EquipSuoData.GetNameIDByIndex(j);
                        wss._count = listData[j];
                        _listAttribute.Add(wss);
                    }
                }

                EquipSuoData.m_listEquipWash.Add(esr.zhuangbeiID, _listAttribute);
                // index_ShuXing = 0;
               
                foreach (KeyValuePair<int, BagItem> item in EquipsOfBodyDic)
                {
                    if (item.Value.itemId == EquipSaveId)
                    {
                        ShowEquipInfo(ZhuangBei.templates[i],
                                      esr.gongJi.ToString(),
                                      esr.fangYu.ToString(),
                                      esr.shengMing.ToString(),
                                      int.Parse(esr.expMax.ToString()),
                                      esr.exp,
                                      item.Value.pinZhi, esr.level);
                        break;
                    }
                }
                break;
            }
        }
    
    }
    public void AppendAttributeUpdate(int index)//更新属性及加成
    {
        if (EquipSuoData.m_listEquipWash.ContainsKey(index))
        {
            int size_sx = EquipSuoData.m_listEquipWash[index].Count;
            int size_obj = _listObj.Count;

            if (size_sx >= size_obj)
            {
                for (int k = 0; k < size_sx; k++)
                {
                    if (k < size_obj)
                    {
                        _listObj[k].GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(EquipSuoData.m_listEquipWash[EquipSaveId][k], true, EquipSuoData.GetWetherContainNewAttribute(index));
                    }
                    else
                    {
                        if (k == size_obj)
                        {
                            index_ShuXing = k;
                        }
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM), ResourcesLoadCallBack2);
                    }
                }
            }
            else
            {
                for (int k = 0; k < size_obj; k++)
                {
                    if (k < size_sx)
                    {
                        _listObj[k].GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(EquipSuoData.m_listEquipWash[EquipSaveId][k], true, EquipSuoData.GetWetherContainNewAttribute(index));
                    }
                    else
                    {
                        Destroy(_listObj[k]);
                    }
                }

                for (int k = 0; k < size_obj - size_sx; k++)
                {
                    _listObj.RemoveAt(_listObj.Count - 1);
                }
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

        clone.AddComponent<TweenPosition>();
        clone.AddComponent<TweenAlpha>();
        clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
        clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
        clone.GetComponent<TweenPosition>().duration = 0.5f;
        clone.GetComponent<TweenAlpha>().from = 1.0f;
        clone.GetComponent<TweenAlpha>().to = 0;
        clone.GetComponent<TweenPosition>().duration = 0.8f;
        StartCoroutine(WatiFor(clone));
    }

    void CreateMove(GameObject move, string content)
    {
        GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
        clone.transform.localPosition = move.transform.localPosition;
        clone.transform.localRotation = move.transform.localRotation;
        clone.transform.localScale = move.transform.localScale;
        clone.GetComponent<UILabel>().text = "";
      
        clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, content);
       

        clone.AddComponent< TweenPosition>();
        clone.AddComponent<TweenAlpha>();
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
        UI3DEffectTool.Instance().ClearUIFx(m_EquipItenm.m_SpritePinZhi.gameObject);
        Destroy(obj);
    }

    void ShowUpgradeInfo()
    {
        if (BuWeiSave == (int)EquipPositionEnum.QingWuQi || BuWeiSave == (int)EquipPositionEnum.ZhongWuQi || BuWeiSave == (int)EquipPositionEnum.Gong)
        {
            equipType = 1;

            if (EquipInfo.gongJiAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, ei.gong);
            }
            else
            {
                m_EquipItenm.m_LabelGong.text = MyColorData.getColorString(10, ei.gong);
                CreateClone(m_EquipItenm.m_LabelGong.gameObject, EquipInfo.gongJiAdd);
            }
        }
        else
        {
            equipType = 0;
            if (EquipInfo.fangYuAdd == 0 && EquipInfo.shengMingAdd == 0 || StrengthenIndex == 1)
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);
            }
            else
            {
                m_EquipItenm.m_LabelFang.text = MyColorData.getColorString(10, ei.fang);
                m_EquipItenm.m_LabelXue.text = MyColorData.getColorString(10, ei.xue);

                CreateClone(m_EquipItenm.m_LabelFang.gameObject, EquipInfo.fangYuAdd);
                CreateClone(m_EquipItenm.m_LabelXue.gameObject, EquipInfo.shengMingAdd);
            }
        }
         m_EquipUpgrade.SetActive(true);
         m_EquipUpgrade.GetComponent<EquipGrowthUpgradeManagerment>().GetEquipInfo(EquipSaveId, DBidSave, BuWeiSave, true);
    }

    int EquipType = 0;

    private bool AllIntensify()
    {
        for (int i = 0; i < 9; i++)
        {
            if (AllIntensify(i) != 99)
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

    int index_ShuXing = 0;
    public List<GameObject> _listObj = new List<GameObject>();
    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_ParentInfo != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            _listObj.Add(rewardShow);
            rewardShow.transform.parent = m_ParentInfo.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = new Vector3(-237, -20+index_ShuXing * (-28), 0);
            rewardShow.transform.GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(EquipSuoData.m_listEquipWash[EquipSaveId][index_ShuXing],false, EquipSuoData.GetWetherContainNewAttribute(EquipSaveId));
            //m_TableInfo.repositionNow = true;
            if (index_ShuXing < EquipSuoData.m_listEquipWash[EquipSaveId].Count - 1)
            {
                index_ShuXing++;
            }
        }
        else
        {
            p_object = null;
        }
    }

}
