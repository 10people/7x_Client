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
    public bool m_WetherIsIntensify = false;
    public GameObject m_IntensifyTanHao;
    public GameObject m_AdvanceTanHao;
    public GameObject m_WashTanHao;
    public GameObject m_InlayTanHao;
    public UILabel m_labelIntensify;
    public GameObject m_SharePart;
    public GameObject m_EquipIntensify;
    public GameObject m_EquipWash;
    public GameObject m_EquipWashObject;

    public GameObject m_EquipInlayObject;
    public GameObject m_EquipCompoundObject;
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
    public  EquipStrengthResp EquipInfo;
    private List<float> listData = new List<float>();
    public GameObject m_YiJianQHEffect;
    public GameObject m_YiJianQHEffectTexture;
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
        public long _dbid;
        public int _EquipId;
        public string _Name;
        public int _Gong;
        public int _Fang;
        public int _Xue;
        public int _Level;
        public string _Progress;
        public string _Icon;
        public int _PinZhi;
        public float _PregressValue;
        public int _AttrCount;
        public int _MaxLevel;
        public int _currEXp;
        public int _MaxEXp;
        public int _gemGong;
        public int _gemFang;
        public int _gemMing;
        public int _advanceExp;
    };
    private EquipBaseInfo _EquipBInfo;
    private bool _isUsed = false;
    public EquipGrowthEquipInfoItemManagerment m_EquipItenm = null;
    private EquipGrowthInlayLayerManagerment.GemsMainInfo _GemiInfo;
    void Start()
    {
        _GemiInfo = new EquipGrowthInlayLayerManagerment.GemsMainInfo();
        _EquipBInfo = new EquipBaseInfo();
        _listObj.Clear();
        if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 10000)
        {
            m_TotalGold.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000).ToString() + NameIdTemplate.GetName_By_NameId(990051);
        }
        else
        {
            m_TotalGold.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
        }
        ShowAdvanceTanHao();
     //   m_LabelTopUp.text = LanguageTemplate.GetText(LanguageTemplate.Text.TOPUP_SIGNAL);
    }

    public static bool m_isEffect = false;
    private int _CurrenItemNum = 0;
    private float _timeInterval = 0;
    public void ShowAdvanceTanHao()
    {
        m_AdvanceTanHao.SetActive(CompoundContainEnable());
    }

    void Update()
    {
        _timeInterval += Time.deltaTime;
       if (_timeInterval >= 0.06f)
        {
            _timeInterval = 0;
            //int[] arrange = { 3, 4, 5, 0, 8, 1, 7, 2, 6 };
            if (m_isEffect)
            {
                if (_CurrenItemNum < EquipSuoData.m_listEffectInfo.Count)
                {
                    foreach (KeyValuePair<int, EquipSuoData.StrengthEffect> item in EquipSuoData.m_listEffectInfo)
                    {
                        _CurrenItemNum++;
                        m_objSharePart.GetComponent<EquipGrowthWearManagerment>().m_listItemEvent[item.Value._num].MoveLabel(item.Value._LevelAdd);
                    }
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
    private bool _IsOn = false;
    void OnEnable()
    {
        if (_isUsed)
        {
			ManualAddToTop();
        }
        if (_IsOn)
        {
            if (ShowType == 0)//0强化
            {
                m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ShowInfo(EquipInfo);
            }
            else if (ShowType == 1)//1 洗练
            {
                m_EquipWash.GetComponent<EquipGrowthWashManagerment>().m_Sendbuwei = BuWeiSave;
                m_EquipWash.GetComponent<EquipGrowthWashManagerment>().EquipWash(DBidSave, EquipSaveId, 0, 3, 3, 3, 3, 3, 3, 3, 3, _PinZhiSave);
            }
        }
        _IsOn = true;
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
    private int _CurrSave = 0;
    private int _MaxSave = 0;
    private int _PinZhiSave = 0;
    private int _LevelSave = 0;
    public void ShowEquipInfo(EquipStrengthResp esr)
    {
        _CurrSave = esr.exp;
        _PinZhiSave = ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).pinZhi;
        _LevelSave = esr.level;
        _MaxSave = esr.expMax;
  
        ei.gong = esr.gongJi.ToString();
        ei.fang = esr.fangYu.ToString();
        ei.xue = esr.shengMing.ToString();
        ei.level = esr.level;
        Equipname = ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).m_name;

        if (_isMaxLevel)
        {
            _isMaxLevel = false;
            if (m_EquipItenm != null)
            {
                CreateMove(m_EquipItenm.m_LabelProgress.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_MAX_LEVEL));
            }
        }
        _EquipBInfo._Name = NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).m_name));
        _EquipBInfo._Gong = esr.gongJi;
        _EquipBInfo._Fang = esr.fangYu;
        _EquipBInfo._Xue = esr.shengMing;
        if (esr.expMax != -1)
        {
            _EquipBInfo._Progress = esr.exp.ToString() + "/" + esr.expMax.ToString();
            _EquipBInfo._PregressValue = esr.exp / float.Parse(esr.expMax.ToString());
        }
        else
        {
            _EquipBInfo._Progress = "";
            _EquipBInfo._PregressValue = 1.0f;
        }
        _EquipBInfo._Level = esr.level;
        _EquipBInfo._EquipId = esr.zhuangbeiID;
        _EquipBInfo._Icon = ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).icon;
        _EquipBInfo._MaxLevel = ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).qianghuaMaxLv;
        _EquipBInfo._PinZhi = CommonItemTemplate.getCommonItemTemplateById(esr.zhuangbeiID).color;
        _EquipBInfo._currEXp = esr.exp;
        _EquipBInfo._MaxEXp = esr.expMax;
        _EquipBInfo._dbid = DBidSave;
        _EquipBInfo._advanceExp = esr.jinJieExp;
        if (EquipSuoData.m_listEquipWash.ContainsKey(EquipSaveId))
            _EquipBInfo._AttrCount = EquipSuoData.m_listEquipWash[EquipSaveId].Count;
        if (ShowType == 0)//0强化
        {
          //  m_labelIntensify.gameObject.SetActive(true);
          //  m_labelIntensify.text = MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.XILIAN_DESC_8));


            EquipSuoData.Instance().m_WashIson = false;
            m_EquipWashObject.SetActive(false);
            m_EquipInlayObject.SetActive(false);
            m_EquipCompoundObject.SetActive(false);
            m_EquipIntensify.SetActive(true);

            m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ShowInfo(esr);
        }
        else if (ShowType == 1)//1 洗练
        {
            EquipSuoData.Instance().m_EquipID = EquipSaveId;
            m_EquipIntensify.SetActive(false);
            m_EquipWashObject.SetActive(true);
            m_EquipInlayObject.SetActive(false);
            m_EquipCompoundObject.SetActive(false);
            m_EquipWash.SetActive(true);
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().m_EquipType = equipType;
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().buttonNum = 0;
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().m_Sendbuwei = BuWeiSave;
            m_EquipWash.GetComponent<EquipGrowthWashManagerment>().EquipWash(DBidSave, EquipSaveId, 0, 
                               3, 3, 3, 3, 3, 3, 3, 3, ZhuangBei.getZhuangBeiById(esr.zhuangbeiID).pinZhi);
        }
        else if (ShowType == 2)//进阶
        {
            m_EquipIntensify.SetActive(false);
            m_EquipWashObject.SetActive(false);
            m_EquipInlayObject.SetActive(false);
            m_EquipCompoundObject.SetActive(true);

           m_EquipCompoundObject.GetComponent<EquipGrowthEquipAdvanceManagerment>().ShowInfo(_EquipBInfo);
        }
        else//镶嵌
        {
            m_EquipIntensify.SetActive(false);
            m_EquipWashObject.SetActive(false);
            m_EquipInlayObject.SetActive(true);
            m_EquipCompoundObject.SetActive(false);
            _GemiInfo.baseInfo = _EquipBInfo;
            m_EquipInlayObject.GetComponent<EquipGrowthInlayLayerManagerment>().ShowInfo(_GemiInfo);
        }

        m_EquipItenm.ShowInfo(_EquipBInfo);
        if (ShowType >= 0 && ShowType < 2)
        {
            if (ShowType == 0)//0强化
            {
                m_EquipItenm.m_ObjAttributeManagerment.SetActive(true);
                IntensifyEquipInfoShow();
            }
            else if (ShowType == 1)//1 洗练
            {
                m_EquipItenm.m_ObjAttributeManagerment.SetActive(false);

                WashEquipInfoShow();
            }
            m_EquipItenm.gameObject.SetActive(true);
        }
        else
        {
            m_EquipItenm.gameObject.SetActive(false);
        }
        //LoadAttrbuite();
        ShowInfo();

    }

    void ShowInfo()
    {
        //if (m_EquipItenm)
        {
            if (EquipInfo.gongJiAdd > 0 || EquipInfo.fangYuAdd > 0 || EquipInfo.shengMingAdd > 0)
            {
                ShowLevel(ei.level, EquipInfo.gongJiAdd, EquipInfo.fangYuAdd, EquipInfo.shengMingAdd);
            }
            else
            {
                ShowFresh(ei.level);
            }
        }
        m_MianInfo.SetActive(true);
        m_SharePart.SetActive(true);

        if (!_isUsed)
        {
            _isUsed = true;

            ManualAddToTop();
        }
    }

	/// only runned once when Equip Window opened, if close then reset.
	void ManualAddToTop(){
//		Debug.Log( "Equip Window Opened." );
		
		UI2DTool.Instance.AddTopUI( GameObjectHelper.GetRootGameObject( gameObject ) );
	}
    private readonly Vector3 BasicIconPos = new Vector3(-110, 0, 0);
    private const int BasicIconDepth = 10;
    private List<int> _Show_Gong = new List<int>();
    private List<int> _Show_Xue = new List<int>();
    private List<int> _Show_Fang = new List<int>();
    public void ShowLevel(int index, int gong, int fang, int xue)
    {
        m_EquipItenm.m_LabelName.text = _EquipBInfo._Name;

        if (index < ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaMaxLv)
        {
            m_EquipItenm.m_SpriteArrow.gameObject.SetActive(true);
            m_EquipItenm.m_LabelLevel.text = "LV." + index.ToString()
                +  MyColorData.getColorString(45, "                     LV." + (index + 1).ToString());
        }
        else
        {
            m_EquipItenm.m_SpriteArrow.gameObject.SetActive(false);
            m_EquipItenm.m_LabelLevel.text = "LV." + index.ToString();
        }
        if (ShowType == 0)
        {
            if (Mathf.Abs(gong) > 0)
            {
                CreateClone(m_EquipItenm.m_DicInfo[0].gameObject, gong);
                int gg = int.Parse(ei.gong);
                if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , _EquipBInfo._Level + 1) != null)
                {
                    if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                    {
                        m_EquipItenm.m_DicInfo[0].text = gg.ToString() +
                            MyColorData.getColorString(4, " → "
                            + (gg + QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                            , index + 1).gongji - QiangHuaTemplate.GetTemplateByItemId(
                                int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId), index).gongji).ToString()
                            );
                    }
                    else
                    {
                        m_EquipItenm.m_DicInfo[0].text = gg.ToString();
                    }
                }
                else
                {
                    m_EquipItenm.m_DicInfo[0].text = gg.ToString();
                }
            }


            if (Mathf.Abs(fang) > 0)
            {
                int ff = int.Parse(ei.fang);
                CreateClone(m_EquipItenm.m_DicInfo[1].gameObject, fang);
                if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , _EquipBInfo._Level + 1) != null)
                {
                    if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                    {
                        m_EquipItenm.m_DicInfo[1].text = ff.ToString() +
                         MyColorData.getColorString(4, " → "
                        + (ff + QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                        , index + 1).fangyu - QiangHuaTemplate.GetTemplateByItemId(int.Parse(
                            ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                        , index).fangyu).ToString()
                       );

                    }
                    else
                    {
                        m_EquipItenm.m_DicInfo[1].text = ff.ToString();
                    }
                }
                else
                {
                    m_EquipItenm.m_DicInfo[1].text = ff.ToString();
                }
            }


            if (Mathf.Abs(xue) > 0)
            {
                CreateClone(m_EquipItenm.m_DicInfo[2].gameObject, xue);

                int xx = int.Parse(ei.xue);
                if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , _EquipBInfo._Level + 1) != null)
                {
                    if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                    {
                        m_EquipItenm.m_DicInfo[2].text = xx.ToString() +
                            MyColorData.getColorString(4, " → "
                        + (xx + QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                        , index + 1).shengming - QiangHuaTemplate.GetTemplateByItemId(
                            int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId), index).shengming).ToString()
                        );
                    }
                    else
                    {
                        m_EquipItenm.m_DicInfo[2].text = xx.ToString();
                    }

                }
                else
                {
                    m_EquipItenm.m_DicInfo[2].text = xx.ToString();
                }
            }

        }
    }


    void ShowFresh(int index)
    {
        m_EquipItenm.m_LabelName.text = _EquipBInfo._Name;
        if (index < ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaMaxLv)
        {
            m_EquipItenm.m_SpriteArrow.gameObject.SetActive(true);
            m_EquipItenm.m_LabelLevel.text = "LV." + index.ToString() 
                +  MyColorData.getColorString(45, "                     LV." + (index + 1).ToString());
        }
        else
        {
            m_EquipItenm.m_SpriteArrow.gameObject.SetActive(false);
            m_EquipItenm.m_LabelLevel.text = "LV." + index.ToString();
        }
        if (!string.IsNullOrEmpty(ei.gong) && int.Parse(ei.gong) > 0 )
        {
            if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                , _EquipBInfo._Level + 1) != null)
            {
                if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                {
                    m_EquipItenm.m_DicInfo[0].text = ei.gong +
                    MyColorData.getColorString(4, " → "
                    + (int.Parse(ei.gong)
                    + QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , index + 1).gongji - QiangHuaTemplate.GetTemplateByItemId(
                        int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId), index).gongji).ToString()
                    );
                }
                else
                {
                    m_EquipItenm.m_DicInfo[0].text = ei.gong;
                }
             
            }
            else
            {
                m_EquipItenm.m_DicInfo[0].text = ei.gong;
            }

        }

        if (!string.IsNullOrEmpty(ei.fang) && int.Parse(ei.fang) > 0)
        {
            if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId),
                _EquipBInfo._Level + 1) != null)
            { 
                if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                {
                    m_EquipItenm.m_DicInfo[1].text = ei.fang +
                     MyColorData.getColorString(4, " → "
                    + (int.Parse(ei.fang) + QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , index + 1).fangyu - QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                    , index).fangyu).ToString()
                    );
                }
                else
                {
                    m_EquipItenm.m_DicInfo[1].text = ei.fang;
                }
            }
            else
            {
                m_EquipItenm.m_DicInfo[1].text = ei.fang;
            }
        }

        if (!string.IsNullOrEmpty(ei.xue) && int.Parse(ei.xue) > 0)
        {
            if (QiangHuaTemplate.GetTemplateByItemId(int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                , _EquipBInfo._Level + 1) != null)
            {
                if (_EquipBInfo._Level < _EquipBInfo._MaxLevel)
                {
                    m_EquipItenm.m_DicInfo[2].text = ei.xue +
                    MyColorData.getColorString(4, " → "
                    + (int.Parse(ei.xue) + QiangHuaTemplate.GetTemplateByItemId(
                        int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId)
                        , index + 1).shengming - QiangHuaTemplate.GetTemplateByItemId(
                            int.Parse(ZhuangBei.getZhuangBeiById(_EquipBInfo._EquipId).qianghuaId), index).shengming).ToString()
                    );
                }
                else
                {
                    m_EquipItenm.m_DicInfo[2].text = ei.xue;
                }
            }
            else
            {
                m_EquipItenm.m_DicInfo[2].text = ei.xue;
            }
        }
    }
    void IntensifyEquipInfoShow()//强化装备信息显示
    {
        if (BuWeiSave == (int)EquipPositionEnum.QingWuQi 
            || BuWeiSave == (int)EquipPositionEnum.ZhongWuQi
            || BuWeiSave == (int)EquipPositionEnum.Gong)
        {
            equipType = 1;
            m_EquipIntensify.SetActive(true);
        }
        else
        {
            equipType = 0;
        }

        //if (Mathf.Abs(EquipInfo.gongJiAdd) > 0)
        //{
        //    m_EquipItenm.m_DicInfo[0].text = MyColorData.getColorString(10, ei.gong);
        //    CreateClone(m_EquipItenm.m_DicInfo[0].gameObject, EquipInfo.gongJiAdd);
        //}
      
        //if (Mathf.Abs(EquipInfo.fangYuAdd) > 0)
        //{
        //    m_EquipItenm.m_DicInfo[1].text = MyColorData.getColorString(10, ei.fang);

        //    CreateClone(m_EquipItenm.m_DicInfo[1].gameObject, EquipInfo.fangYuAdd);
        //}

        //if (Mathf.Abs(EquipInfo.shengMingAdd) > 0)
        //{

        //    m_EquipItenm.m_DicInfo[2].text = MyColorData.getColorString(10, ei.xue);

        //    CreateClone(m_EquipItenm.m_DicInfo[2].gameObject, EquipInfo.shengMingAdd);
        //}
    }


    void WashEquipInfoShow()//洗练装备信息显示
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS 
            || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS 
            || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            EquipSuoData.Instance().m_WashIson = true;
      
        }
        else
        {
            equipType = 0;
            EquipSuoData.Instance().m_WashIson = true;
 
            m_EquipWashObject.SetActive(true);
        }

        //if (Mathf.Abs(EquipInfo.gongJiAdd) > 0)
        //{
        //    m_EquipItenm.m_DicInfo[0].text = MyColorData.getColorString(10, ei.gong);
        //    CreateClone(m_EquipItenm.m_DicInfo[0].gameObject, EquipInfo.gongJiAdd);
        //}

 
 
        //if (Mathf.Abs(EquipInfo.fangYuAdd) > 0)
        //{
        //    m_EquipItenm.m_DicInfo[1].text = MyColorData.getColorString(10, ei.fang);

        //    CreateClone(m_EquipItenm.m_DicInfo[1].gameObject, EquipInfo.fangYuAdd);
        //}

        //if (Mathf.Abs(EquipInfo.shengMingAdd) > 0)
        //{

        //    m_EquipItenm.m_DicInfo[2].text = MyColorData.getColorString(10, ei.xue);

        //    CreateClone(m_EquipItenm.m_DicInfo[2].gameObject, EquipInfo.shengMingAdd);
        //}
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

                      
                        if (m_objSharePart.GetComponent<EquipGrowthWearManagerment>()._Index_Type_Save == 1)
                        {
                            m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ShowInfo(EquipInfo);
                            if (!m_WetherIsIntensify)
                            {
                                EquipInfoSave = Equip;
                            }
                            else
                            {
                                FunctionWindowsCreateManagerment.FreshEquipInfo(Equip);
                                EquipInfo.gongJiAdd = EquipInfo.gongJi - EquipInfoSave.gongJi;
                                EquipInfo.fangYuAdd = EquipInfo.fangYu - EquipInfoSave.fangYu;
                                EquipInfo.shengMingAdd = EquipInfo.shengMing - EquipInfoSave.shengMing;
                                if (Equip.level > _LevelSave)
                                {
                                    m_EquipItenm.m_LevelSave = Equip.level;
                                    m_EquipItenm.m_listEffect.Add(Equip.level - _LevelSave);

                                }
                                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2,
                                    m_EquipItenm.m_SpritePinZhi.gameObject, EffectIdTemplate.GetPathByeffectId(100182), null);
                                m_WetherIsIntensify = false;


                                if (EquipInfo.expMax == -1)
                                {
                                    _isMaxLevel = true;
                                }

                                CreateMove(m_EquipItenm.m_LabelSuccess.gameObject,
                                    LanguageTemplate.GetText(LanguageTemplate.Text.INTENSIFY_SUCCESS));
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
                        m_WashTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(1210) 
                            && FunctionOpenTemp.GetWhetherContainID(1210));
                        m_InlayTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(1213));
                        PushAndNotificationHelper.SetRedSpotNotification(1212, AllIntensify());
                        EquipsInfoTidy(EquipInfo);
                        StartCoroutine(WaitSecondIntensify());
                        return true;
                    }
                case ProtoIndexes.S_EQUIP_UPALLGRADE://一键强化返回信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipStrength4AllResp Equip = new EquipStrength4AllResp();
                        t_qx.Deserialize(t_tream, Equip, Equip.GetType());
                        EquipGrowthIntensifyManagerment intensy = m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>();
                        intensy.m_YJQHTouch = EquipGrowthIntensifyManagerment.TouchType.BUTTON_UP;
                        if (Equip.allResp != null)
                        {

                            UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_YiJianQHEffectTexture
                                , EffectIdTemplate.GetPathByeffectId(100180), null);
                            m_YiJianQHEffect.SetActive(true);
                            StartCoroutine(WaitSecond());

                            EquipSuoData.m_listEffectInfo.Clear();
                            for (int i = 0; i < Equip.allResp.Count; i++)
                            {

                                if (Equip.allResp[i].level 
                                    > EquipSuoData.m_equipsLevelSave[EquipSuoData.GetEquipInfactUseBuWei(
                                        ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei)])
                                {
                                    EquipSuoData.StrengthEffect sf = new EquipSuoData.StrengthEffect();
                                    sf._num = EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei);
                                    sf._isshow = true;
                                    sf._LevelAdd = Equip.allResp[i].level - EquipSuoData.m_equipsLevelSave[
                                        EquipSuoData.GetEquipInfactUseBuWei(ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei)];
                                    EquipSuoData.m_listEffectInfo.Add(EquipSuoData.GetEquipInfactUseBuWei(
                                        ZhuangBei.getZhuangBeiById(Equip.allResp[i].zhuangbeiID).buWei), sf);
                                }

                                if (Equip.allResp[i].zhuangbeiID == EquipSaveId)
                                {
                                    StrengthenIndex = 0;
                                    EquipInfo = Equip.allResp[i];
                                    if (Equip.allResp[i].level > _LevelSave)
                                    {
                                        m_EquipItenm.m_LevelSave = Equip.allResp[i].level;
                                        m_EquipItenm.m_listEffect.Add(Equip.allResp[i].level - _LevelSave);
                                    }
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
                            //m_EquipIntensify.GetComponent<EquipGrowthIntensifyManagerment>().ButtonShow();
                            m_isEffect = true;

                        }
                        return true;
                    }
            }
        }
        return false;
    }

    IEnumerator WaitSecond()
    {
        //   yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(1.0f);
        UI3DEffectTool.ClearUIFx(m_YiJianQHEffectTexture);
        m_YiJianQHEffect.SetActive(false);

    }
    IEnumerator WaitSecondIntensify()
    {
        //   yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(1.0f);
 
        UI3DEffectTool.ClearUIFx(m_EquipItenm.m_SpritePinZhi.gameObject);
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
    


        EquipGrowthMaterialUseManagerment.equipLevel = esr.level;
        EquipGrowthMaterialUseManagerment.Levelsaved = esr.level;
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                float[] attribute = { float.Parse(esr.wqSH.ToString()), float.Parse(esr.wqJM.ToString()), float.Parse(esr.wqBJ.ToString())
                , float.Parse(esr.wqRX.ToString()), float.Parse(esr.jnSH.ToString()), float.Parse(esr.jnJM.ToString())
                , float.Parse(esr.jnBJ.ToString()), float.Parse(esr.jnRX.ToString())
                ,esr.wqBJL, esr.jnBJL, esr.wqMBL, esr.jnMBL };
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
                        wss._num = j;
                        wss._nameid = EquipSuoData.GetNameIDByIndex(j);
                        wss._count = listData[j];
                        _listAttribute.Add(wss);
                    }
                }

                EquipSuoData.m_listEquipWash.Add(esr.zhuangbeiID, _listAttribute);
                // index_ShuXing = 0;



                ShowEquipInfo(esr);
               
            }
        }
    
    }
    void CreateClone(GameObject move, int content)
    {
        GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
        clone.transform.localPosition = new Vector3(-100,move.transform.localPosition.y,0);
        clone.transform.localRotation = move.transform.localRotation;

        clone.transform.localScale = move.transform.localScale;
        clone.GetComponent<UILabel>().text = "";
        clone.GetComponent<UILabel>().effectStyle = UILabel.Effect.Outline;
        clone.GetComponent<UILabel>().effectColor = Color.black;
       
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
        clone.GetComponent<TweenPosition>().from = clone.transform.localPosition;
        clone.GetComponent<TweenPosition>().to = clone.transform.localPosition + Vector3.up * 40;
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
        clone.GetComponent<UILabel>().effectStyle = UILabel.Effect.Outline;
        clone.GetComponent<UILabel>().effectColor = Color.black;
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
        //UI3DEffectTool.ClearUIFx(m_EquipItenm.m_LabelXue.gameObject);
        //UI3DEffectTool.ClearUIFx(m_EquipItenm.m_LabelFang.gameObject);
        //UI3DEffectTool.ClearUIFx(m_EquipItenm.m_LabelGong.gameObject);
        UI3DEffectTool.ClearUIFx(m_EquipItenm.m_SpritePinZhi.gameObject);
        Destroy(obj);
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
            if (EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 3 
                || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 4 
                || EquipsOfBody.Instance().m_equipsOfBodyDic[index].buWei == 5)
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
 
            if (ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(
                EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp > 0)
            {
                if (EquipExp >= ExpXxmlTemp.getExpXxmlTemp_By_expId(ZhuangBei.getZhuangBeiById(
                    EquipsOfBody.Instance().m_equipsOfBodyDic[index].itemId).expId, equipLevel).needExp)
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
}
