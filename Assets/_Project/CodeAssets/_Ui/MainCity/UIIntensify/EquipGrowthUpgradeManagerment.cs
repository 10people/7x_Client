using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipGrowthUpgradeManagerment : MonoBehaviour
{
    public EventHandler m_EventUpgrade;
    public Dictionary<int, GameObject> DicSpriteInfo = new Dictionary<int, GameObject>();
    public UISprite m_EquipSprite;

    public UILabel m_LabelMove;
    public GameObject m_ShareObj;

    public GameObject m_PrentLeft;
    public GameObject m_PrentMiddle;
    public GameObject m_PrentRight;

    public UILabel m_EquipName;
    public UILabel m_EquipLevel;
    public UILabel m_EquipUpgradeCondition;
    public UILabel m_EquipUpgradeTag;

    public UILabel m_EquipMaterialCount;
    public GameObject m_ArmorAttr;
    public GameObject m_WeaponAttr;

    public GameObject m_ZhuangBeiWenHao;
    public GameObject m_MaterialWenHao;
    public GameObject m_MaxLevelTag;
    public GameObject m_NotMaxLevelTag;
    public List<UILabel> listArmorTopCount;
    public UILabel WeaponGongCount;
 

    public List<UISprite> listPinZhi;

    public List<GameObject> listEffect;


    public GameObject m_EffectObj;

    public List<GameObject> m_ListtArmorTitleLine;
    public GameObject m_WeaponTitleLineBottom;

    public   GameObject m_UpgradeTanHao;
    public GameObject m_MaterialDiaoLuo;

    private List<int> listData = new List<int>();
    private List<int> listAppendName = new List<int>();
    private List<string> listNames = new List<string>();
    private int EquipSaveId;
    private int BuWeiSave;
    private bool WearIsOn = false;
    private int materialSendId = 0;
    private int equipType = 0;
    private long CurrentDbId;
    //  private int SaveInType = 0;
    private bool isUpgrade = false;

    private int levelNeed = 0;
 

    private bool _isMaterialPassed = false;
    private bool _isLevelPassed = false;

    public UIFont titleFont;//标题字体
    public UIFont btn1Font;//按钮1字体
    public UIFont btn2Font;//按钮2字体
    private struct ExibiteInfo
    {
        public string EquipIcon;
        public string EquipUpgradeIcon;
        public string EquipMaterialIcon;
        public string Name;
        public string des;
        public string level;
        public string Condition;
        public string Gong;
        public string Fang;
        public string Ming;
        public string EquipMaterialCount;
        public string EquipMaterialId;
        public int PinZhi;
        public int MaxLevel;
        public int gongAdd;
        public int fangAdd;
        public int xueAdd;
    };

 
    private ExibiteInfo exInfo ;

    void Start()
    {
 
 
        m_EventUpgrade.m_click_handler += Touched;
        //  UI3DEffectTool.ShowTopLayerEffect(m_EffectObj, EffectIdTemplate.GetPathByeffectId(100021), null);
        // UI3DEffectTool.ClearUIFx(m_EffectObj);    
    }

    void Update()
    {
        //if (EquipsOfBody.Instance().m_RefrsehEquipsInfo)
        //{
        //    Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        //    EquipsOfBody.Instance().m_RefrsehEquipsInfo = false;
        //    isUpgrade = true;
        //    PushAndNotificationHelper.SetRedSpotNotification(1211, EquipSuoData.AllUpgrade());
        //    m_EventUpgrade.gameObject.GetComponent<Collider>().enabled = true;
        //    m_UpgradeTanHao.SetActive(EquipSuoData.AllUpgrade());
        //    m_ShareObj.GetComponent<EquipGrowthWearManagerment>().ShowEquipTanHao();
        //    EquipSaveId = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId;
        //    EquipsInfoTidy(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave]);

        //}
    }

    void OnEnable()
    {

        // DicSpriteInfo.Clear();
    }
   
    public void GetEquipInfo(int Equipid, long dbid, int buwei, bool iswear)//获得对应装备信息
    {
        listAppendName.Clear();
        foreach (KeyValuePair<int, GameObject> item in DicSpriteInfo)
        {
            Destroy(item.Value);
        }
        DicSpriteInfo.Clear();
        EquipSaveId = Equipid;
        //      SaveInType = type;

        WearIsOn = iswear;

        BuWeiSave = buwei;
 
        int[] allName = { 990011, 990012, 990013, 990014, 990015, 990016, 990017, 990018 };


        listAppendName.AddRange(allName);

        if (iswear)
        {
            EquipsInfoTidy(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei]);
        }
        else
        {
            foreach (KeyValuePair<int, BagItem> item in BagData.Instance().m_playerEquipDic)
            {
                if (item.Value.itemId == Equipid)
                {
                    EquipsInfoTidy(item.Value);

                    break;
                }
            }
        }
    }

    private void EquipsInfoTidy(BagItem esr)//分离数据并赋值
    {
        listData.Clear();
        listNames.Clear();
    
        EquipSuoData.Instance().listIndexs.Clear();

        //      Dictionary<int, BagItem> EquipsOfBodyDic = EquipsOfBody.Instance().m_equipsOfBodyDic;


   
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;

                exInfo.Name = ZhuangBei.templates[i].m_name;
                exInfo.des = ZhuangBei.templates[i].funDesc;
                exInfo.level = esr.qiangHuaLv.ToString();
                exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
                levelNeed = ZhuangBei.templates[i].jinjieLv;
                exInfo.EquipMaterialId = ZhuangBei.templates[i].jinjieItem;
                exInfo.EquipMaterialCount = ZhuangBei.templates[i].jinjieNum;
                if (!string.IsNullOrEmpty(exInfo.Gong))
                {
                    exInfo.gongAdd = esr.gongJi - int.Parse(exInfo.Gong);

                    exInfo.fangAdd = esr.fangYu - int.Parse(exInfo.Fang);
                    exInfo.xueAdd = esr.shengMing - int.Parse(exInfo.Ming);
                }
                exInfo.Gong = esr.gongJi.ToString();
                exInfo.Fang = esr.fangYu.ToString();
                exInfo.Ming = esr.shengMing.ToString();
                exInfo.PinZhi = esr.pinZhi;
                break;
            }
        }

        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
        }
        else
        {
            equipType = 0;
        }

        if (isUpgrade)
        {
           
            // StartCoroutine(WaitFor());
            ExhibitionEquip();
            isUpgrade = false;
            RefreshEquipInfo();
        }
        else
        {
            ExhibitionEquip();
        }

    }

    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(0.2f);
        RefreshEquipInfo();
    }
    void ExhibitionEquip()
    {
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                for (int j = 0; j < ItemTemp.templates.Count; j++)
                {
                    if (ItemTemp.templates[j].id == int.Parse(ZhuangBei.templates[i].jinjieItem))
                    {
                        exInfo.EquipMaterialIcon = ItemTemp.templates[j].icon;
                        break;
                    }
                }
                break;
            }
        }
        if (!isUpgrade)
        {
            ShowWearEquipInfo();
        }
    }

    public void ShowWearEquipInfo()
    {
        materialSendId = int.Parse(exInfo.EquipMaterialId);
 
        m_EquipSprite.gameObject.SetActive(true);
        SelectEquipLeftShow();
  
        if (EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].pinZhi == 11)
        {
           m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
           m_MaxLevelTag.SetActive(true);
           m_NotMaxLevelTag.SetActive(false);

            m_ZhuangBeiWenHao.SetActive(true);
            m_MaterialWenHao.SetActive(true);
        }
        else
        {
            m_EquipUpgradeTag.text = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).m_name)) + "  +" + exInfo.level);
            m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
            m_MaxLevelTag.SetActive(false);
            m_NotMaxLevelTag.SetActive(true);
            SelectEquipTopShow();
            SelectMaterialShow();
        }
        m_EquipSprite.spriteName = exInfo.EquipIcon;

        if (int.Parse(exInfo.level) > 0)
        {
            m_EquipLevel.text = MyColorData.getColorString(4, "+" + exInfo.level);
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        else
        {
            m_EquipLevel.text = "";
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        int ttt = 0;
        if (int.TryParse(exInfo.Condition, out ttt) && JunZhuData.Instance().m_junzhuInfo.level >= int.Parse(exInfo.Condition))
        {
            m_EquipUpgradeCondition.text = MyColorData.getColorString(10,  MyColorData.getColorString(4, exInfo.Condition) + MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(990019)));
        }
        else
        {
            m_EquipUpgradeCondition.text = MyColorData.getColorString(10, MyColorData.getColorString(5, exInfo.Condition) + MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(990019)));
         }
        listPinZhi[0].spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipIcon)));

        int index = 0;

        if (exInfo.PinZhi == 11)
        {
            _isMaterialPassed = false;
            _isLevelPassed = false;
            m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            m_EquipMaterialCount.text = "";
        }
        else
        {
            index = GetMaterialCountByID(int.Parse(exInfo.EquipMaterialId));
            if (index < int.Parse(exInfo.EquipMaterialCount) || JunZhuData.Instance().m_junzhuInfo.level < int.Parse(exInfo.Condition))
            {
                if (JunZhuData.Instance().m_junzhuInfo.level < int.Parse(exInfo.Condition))
                {
                    _isLevelPassed = false;
                }
                else
                {
                    _isMaterialPassed = false;
                    _isLevelPassed = true;
                }
                m_EquipMaterialCount.text = MyColorData.getColorString(5, index.ToString()) + "/" + exInfo.EquipMaterialCount;
            }
            else
            {
                _isMaterialPassed = true;
                _isLevelPassed = true;
                m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
                m_EquipMaterialCount.text = index.ToString() + "/" + exInfo.EquipMaterialCount;
            }
        }

      //  ButtonShow();
    }
    void EquipInfoShow()
    {

        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            WeaponGongCount.text = MyColorData.getColorString(10, exInfo.Gong);
            //if (listNames.Count == 0)
            //{
            //    m_WeaponTitleLineBottom.SetActive(false);
            //}
            //else
            //{
            //    m_WeaponTitleLineBottom.SetActive(true);
            //}
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);
            
            listArmorTopCount[0].text = MyColorData.getColorString(10, exInfo.Fang);
            listArmorTopCount[1].text = MyColorData.getColorString(10, exInfo.Ming);
        }
    }
    
    private void Touched(GameObject obj)
    {
         if (obj.name.Equals("ButtondAdvance"))//装备进阶
        {
            if (UIYindao.m_UIYindao.m_isOpenYindao)
            {
                if (TaskData.Instance.m_iCurMissionIndex == 100110 && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
                {
                    TaskData.Instance.m_iCurMissionIndex = 100110;
                    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                    tempTaskData.m_iCurIndex = 3;
                    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                }
                //else if (TaskData.Instance.m_iCurMissionIndex == 100270 && TaskData.Instance.m_TaskInfoDic[100270].progress >= 0)
                //{
                //    TaskData.Instance.m_iCurMissionIndex = 100270;
                //    ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                //    tempTaskData.m_iCurIndex = 4;
                //    UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                //}
            }

            if (!FunctionOpenTemp.GetWhetherContainID(1211))
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxLoadCallback);
            }
            else
            {
                if (_isMaterialPassed && _isLevelPassed)
                {
                    m_EventUpgrade.gameObject.GetComponent<Collider>().enabled = false;
                    EquipsOfBody.Instance().EquipAdvace(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].dbId);
                }
                else
                {
                    if (!_isLevelPassed)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                          UIBoxLoadCallLevel);
                    }
                    else
                    {
                        m_MaterialDiaoLuo.SetActive(true);
                        m_MaterialDiaoLuo.GetComponent<JunZhuDiaoLuoManager>().ShowDiaoLuoLevel(materialSendId);
                    }
                }   
            }
        }
        
    }
    public void UIBoxLoadCallbackLevel(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = NameIdTemplate.GetName_By_NameId(10000) + NameIdTemplate.GetName_By_NameId(990053) + "!";

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, titleFont, btn1Font);
    }

    public void UIBoxLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = NameIdTemplate.GetName_By_NameId(990043) + ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1211)) + NameIdTemplate.GetName_By_NameId(990044) + "!";

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, titleFont, btn1Font);
    }

    public void UIBoxLoadCallLevel(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_LEVEUP);

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, titleFont, btn1Font);
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
                            return item2.Value[j].cnt >= int.Parse(ZhuangBei.templates[i].jinjieNum);
                        }
                    }
                }
            }

        }
        return false;
    }

    private void ShowDiaoLuo(GameObject obj)
    {

        m_MaterialDiaoLuo.SetActive(true);
        m_MaterialDiaoLuo.GetComponent<JunZhuDiaoLuoManager>().ShowDiaoLuoLevel(materialSendId);
    }

    private void RefreshEquipInfo()
    {
        string itemIcon = "";
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId)
            {
                EquipSaveId = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId;
                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;
                exInfo.Name = ZhuangBei.templates[i].m_name;
                exInfo.des = ZhuangBei.templates[i].funDesc;
                exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
                exInfo.EquipMaterialId = ZhuangBei.templates[i].jinjieItem;
                exInfo.EquipMaterialCount = ZhuangBei.templates[i].jinjieNum;
                exInfo.PinZhi = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].pinZhi;
             
                exInfo.Gong = EquipsOfBody.Instance().m_EquipUpgradeInfo.gongJi.ToString();
                exInfo.Fang = EquipsOfBody.Instance().m_EquipUpgradeInfo.fangYu.ToString();
                exInfo.Ming = EquipsOfBody.Instance().m_EquipUpgradeInfo.shengMing.ToString();
                exInfo.MaxLevel = 0;
                break;
            }
        }


        materialSendId = int.Parse(exInfo.EquipMaterialId);
        if (int.Parse(exInfo.level) > 0)
        {
            m_EquipLevel.text = MyColorData.getColorString(4, "+" + exInfo.level);
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        else
        {
            m_EquipLevel.text = "";
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        int tttt = 0;
        if (int.TryParse(exInfo.Condition, out tttt) && JunZhuData.Instance().m_junzhuInfo.level >= int.Parse(exInfo.Condition))
        {
            m_EquipUpgradeCondition.text = MyColorData.getColorString(10, MyColorData.getColorString(4, exInfo.Condition) + MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(990019)));
        }
        else
        {
            m_EquipUpgradeCondition.text = MyColorData.getColorString(10, MyColorData.getColorString(5, exInfo.Condition) + MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(990019)));
        }

        if (exInfo.PinZhi == 11)
        {
            m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            m_MaxLevelTag.SetActive(true);
            m_NotMaxLevelTag.SetActive(false);

            m_ZhuangBeiWenHao.SetActive(true);
            m_MaterialWenHao.SetActive(true);

            m_EquipSprite.spriteName = exInfo.EquipIcon;

            DicSpriteInfo[0].GetComponent<IconSampleManager>().FgSprite.spriteName = exInfo.EquipIcon;
            DicSpriteInfo[0].GetComponent<IconSampleManager>().QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipIcon)).color);

            DicSpriteInfo[1].SetActive(false);

            DicSpriteInfo[2].SetActive(false);

            listPinZhi[0].spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipIcon)));


        }
        else
        {
            m_EquipUpgradeTag.text = MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).m_name)) + "  +" + exInfo.level);
            m_MaxLevelTag.SetActive(false);
            m_NotMaxLevelTag.SetActive(true);
            m_EquipSprite.spriteName = exInfo.EquipIcon;

          //  DicSpriteInfo[0].GetComponent<IconSampleManager>().FgSprite.spriteName = exInfo.EquipIcon;
            IconSampleManager iconSampleManager0 = DicSpriteInfo[0].GetComponent<IconSampleManager>();
   
            iconSampleManager0.SetIconBasic(0, exInfo.EquipIcon, "");

            iconSampleManager0.QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipIcon)));
            iconSampleManager0.SetIconPopText(int.Parse(exInfo.EquipIcon), NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipIcon)).m_name)), DescIdTemplate.GetDescriptionById(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipIcon)).funDesc)), 0, Vector3.zero);
            DicSpriteInfo[0].GetComponent<IconSampleManager>().QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipIcon)).color);


            //  DicSpriteInfo[1].GetComponent<IconSampleManager>().FgSprite.spriteName = exInfo.EquipUpgradeIcon;
            IconSampleManager iconSampleManager1 = DicSpriteInfo[1].GetComponent<IconSampleManager>();
  
            iconSampleManager1.SetIconBasic(0, exInfo.EquipUpgradeIcon, "");
     
            iconSampleManager1.QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipUpgradeIcon)));
            iconSampleManager1.SetIconPopText(int.Parse(exInfo.EquipUpgradeIcon), NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).m_name)), DescIdTemplate.GetDescriptionById(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).funDesc)), 0, Vector3.zero);
            DicSpriteInfo[1].GetComponent<IconSampleManager>().QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipIcon)).color);

            // DicSpriteInfo[2].GetComponent<IconSampleManager>().FgSprite.spriteName = exInfo.EquipMaterialIcon;
            IconSampleManager iconSampleManager2 = DicSpriteInfo[2].GetComponent<IconSampleManager>();
            iconSampleManager2.SetIconBasic(0, exInfo.EquipMaterialIcon, "");
            iconSampleManager2.QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).color);
            iconSampleManager2.SetIconPopText(int.Parse(exInfo.EquipMaterialIcon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).descId), 1, Vector3.zero);
            DicSpriteInfo[2].GetComponent<IconSampleManager>().QualityFrameSprite.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipIcon)).color);

            listPinZhi[0].spriteName = QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipIcon)));

        }
       
        if (exInfo.PinZhi == 11)
        {
            m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
            m_EquipMaterialCount.text = "";
        }
        else
        {
            int index_count = GetMaterialCountByID(int.Parse(exInfo.EquipMaterialId));

            if (index_count < int.Parse(exInfo.EquipMaterialCount) || JunZhuData.Instance().m_junzhuInfo.level < int.Parse(exInfo.Condition))
            {
                if (JunZhuData.Instance().m_junzhuInfo.level < int.Parse(exInfo.Condition))
                {
                    _isLevelPassed = false;
                }
                else
                {
                    _isMaterialPassed = false;
                    _isLevelPassed = true;
                }
                m_EquipMaterialCount.text = MyColorData.getColorString(5, index_count.ToString()) + "/" + exInfo.EquipMaterialCount;
            }
            else
            {
                _isMaterialPassed = true;
                _isLevelPassed = true;
                m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
                m_EquipMaterialCount.text = index_count.ToString() + "/" + exInfo.EquipMaterialCount;
            }
            m_EventUpgrade.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
           // m_EquipMaterialCount.text = GetMaterialCountByID(int.Parse(exInfo.EquipMaterialId)).ToString() + "/" + exInfo.EquipMaterialCount;

        }
        CreateMove(m_LabelMove.gameObject, LanguageTemplate.GetText(LanguageTemplate.Text.UPGRADE_SUCCESS));
        if (equipType == 1)
        {
            CreateClone(WeaponGongCount.gameObject, exInfo.gongAdd);
            WeaponGongCount.text = MyColorData.getColorString(10, exInfo.Gong); //+ MyColorData.getColorString(4, " +" + EquipsOfBody.Instance().m_EquipUpgradeInfo.gongJiAdd.ToString());
        }
        else
        {
            CreateClone(listArmorTopCount[0].gameObject, exInfo.fangAdd);
            listArmorTopCount[0].text = MyColorData.getColorString(10, exInfo.Fang); //+ MyColorData.getColorString(4, " +" + EquipsOfBody.Instance().m_EquipUpgradeInfo.fangYuAdd.ToString());
            CreateClone(listArmorTopCount[1].gameObject, exInfo.xueAdd);
            listArmorTopCount[1].text = MyColorData.getColorString(10, exInfo.Ming);// + MyColorData.getColorString(4, "+" + EquipsOfBody.Instance().m_EquipUpgradeInfo.shengMingAdd.ToString());
        }

        RefreshInfo();

    }

    void RefreshInfo()
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            //if (listNames.Count == 0)
            //{
            //    m_WeaponTitleLineBottom.SetActive(false);
            //}
            //else
            //{
            //    m_WeaponTitleLineBottom.SetActive(true);
            //}
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);
            {
            //    if (listNames.Count == 0)
            //    {
            //        m_ListtArmorTitleLine[0].SetActive(false);
            //    }
            //    else if (listNames.Count > 2)
            //    {
            //        m_ListtArmorTitleLine[0].SetActive(true);

            //        m_ListtArmorTitleLine[1].SetActive(true);
            //    }
            //    else
            //    {
            //        m_ListtArmorTitleLine[0].SetActive(true);
            //        m_ListtArmorTitleLine[1].SetActive(false);
            //    }
            }

        }
    }
    void OnDisable()
    {
 
    }

    void CreateClone()
    {
        for (int i = 0; i < listEffect.Count; i++)
        {
            GameObject clone = NGUITools.AddChild(listEffect[i].transform.parent.gameObject, listEffect[i]);
            clone.transform.localPosition = transform.localPosition;
            clone.transform.localRotation = transform.localRotation;
            clone.transform.localScale = transform.localScale;
            clone.AddComponent(typeof(TweenPosition));
            clone.GetComponent<TweenPosition>().enabled = false;
            clone.AddComponent(typeof(JunZhuAdvanceEffective));
            clone.GetComponent<JunZhuAdvanceEffective>().index = i;
        }
    }

    private int SelectQualityNum(int index)
    {
        if (index == 1)
        {
            return 0;
        }
        else if (index == 2)
        {
            return 1;
        }
        else if (index > 2 && index <= 5)
        {
            return 2;
        }
        else if (index > 5 && index <= 8)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    void SelectEquipLeftShow()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
    }

    void SelectEquipTopShow()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack1);
    }
    void SelectMaterialShow()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack2);
    }

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        DicSpriteInfo.Add(0, iconSampleObject);
        iconSampleObject.GetComponent<UIWidget>().depth = 10;
        iconSampleObject.SetActive(true);
        iconSampleObject.transform.parent = m_PrentLeft.transform;
        iconSampleObject.transform.localPosition = Vector3.zero;
        iconSampleObject.transform.localScale = Vector3.one;
        IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        iconSampleManager.SetIconType((IconSampleManager.IconType)2);//1,2
        iconSampleManager.SetIconBasic(20, exInfo.EquipIcon, "", QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipIcon))));

		iconSampleManager.SetIconPopText(int.Parse(exInfo.EquipIcon), NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipIcon)).m_name)), DescIdTemplate.GetDescriptionById(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipIcon)).funDesc)), 0, Vector3.zero);
    }

    private void OnIconSampleLoadCallBack1(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        DicSpriteInfo.Add(1, iconSampleObject);
        iconSampleObject.GetComponent<UIWidget>().depth = 10;
        iconSampleObject.SetActive(true);
        iconSampleObject.transform.parent = m_PrentMiddle.transform;
        iconSampleObject.transform.localPosition = Vector3.zero;
        iconSampleObject.transform.localScale = Vector3.one;
        IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        iconSampleManager.SetIconType((IconSampleManager.IconType)2);//1,2
        iconSampleManager.SetIconBasic(20, exInfo.EquipUpgradeIcon, "", QualityIconSelected.SelectQuality(ZhuangBei.GetColorByEquipID(int.Parse(exInfo.EquipUpgradeIcon))));

		iconSampleManager.SetIconPopText(int.Parse(exInfo.EquipUpgradeIcon), NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).m_name)), DescIdTemplate.GetDescriptionById(int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(exInfo.EquipUpgradeIcon)).funDesc)));
    }

    private void OnIconSampleLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject iconSampleObject = Instantiate(p_object) as GameObject;
        iconSampleObject.GetComponent<UIWidget>().depth = 10;
        DicSpriteInfo.Add(2, iconSampleObject);
        iconSampleObject.SetActive(true);
        iconSampleObject.transform.parent = m_PrentRight.transform;
        iconSampleObject.transform.localPosition = Vector3.zero;
        iconSampleObject.transform.localScale = Vector3.one;
        IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        iconSampleManager.SetIconType((IconSampleManager.IconType)1);//1,2
        iconSampleManager.SetIconBasic(20, exInfo.EquipMaterialIcon, "", QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).color));

		iconSampleManager.SetIconPopText(int.Parse(exInfo.EquipMaterialIcon), NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialIcon)).descId));
        iconSampleManager.NguiLongPress.OnNormalPress = ShowDiaoLuo;
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
        StartCoroutine(WatiFore(clone));
    }
    IEnumerator WatiFore(GameObject obj)
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(obj);
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
}
