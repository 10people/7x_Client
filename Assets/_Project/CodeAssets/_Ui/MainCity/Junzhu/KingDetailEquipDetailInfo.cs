using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KingDetailEquipDetailInfo : MonoBehaviour
{
    public KingDetailEquipInfo m_KingDetailEquipInfo;

    public EventHandler m_EventHandler;

    public UILabel m_EquipName;
    public UILabel m_EquipDes;

    public UILabel m_EquipGong;
    public UILabel m_EquipFang;
    public UILabel m_EquipXue;

    public GameObject m_ArmorAttr;
    public GameObject m_WeaponAttr;

    public GameObject m_Junzhu;

    public GameObject m_ArmorAttrInfo1;
    public GameObject m_ArmorAttrInfo2;

    public GameObject m_ArmorSignal2;
    public GameObject m_WeapoSignal2;

    public List<UILabel> listArmorAttributeName = new List<UILabel>();
    public List<UILabel> listArmorAttributeCount = new List<UILabel>();

    public List<UILabel> listWeaponAttributeName = new List<UILabel>();
    public List<UILabel> listWeaponAttributeCount = new List<UILabel>();
    private struct AppendAttr
    {
        public string OneSend;
        public int _OneType;
        public string TwoSend;
        public int _TwoType;
        public string ThreeSend;
        public int _ThreeType;
        public string FourSend;
        public int _FourType;
    };
    private AppendAttr _AppendInfo = new AppendAttr();

    private List<int> listData = new List<int>();
    private List<int> listAppendName = new List<int>();
    private List<string> listNames = new List<string>();
    private int EquipSaveId;
    private int BuWeiSave;
    private bool WearIsOn = false;
    private int materialSendId = 0;
    private int equipType = 0;
    private long CurrentDbId;
    private bool isUpgrade = false;
    private int levelNeed = 0;

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
        public string One;
        public string Two;
        public string Three;
        public string Four;
        public string EquipMaterialCount;
        public string EquipMaterialId;
        public int PinZhi;
        public int MaxLevel;
    };
    private ExibiteInfo exInfo = new ExibiteInfo();

    void Start()
    {
        m_EventHandler.m_click_handler += Touched;
    }

    void Update()
    {
        if (CityGlobalData.m_RefreshEquipInfo)
        {
            CityGlobalData.m_RefreshEquipInfo = false;
        }
    }

    public void GetEquipInfo(int Equipid, long dbid, int buwei, bool iswear, int type)//获得对应装备信息
    {
        listAppendName.Clear();

        int[] allName = { 990011, 990012, 990013, 990014, 990015, 990016, 990017, 990018 };

        WearIsOn = iswear;

        BuWeiSave = buwei;
        listAppendName.AddRange(allName);
        if (EquipsUpContain(buwei) != null)
        {
            EquipSaveId = EquipsUpContain(buwei).itemId;
            if (iswear)
            {
                EquipsInfoTidy(EquipsUpContain(buwei));
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
        else
        {
            EquipSaveId = Equipid;

            if (iswear)
            {
                EquipsInfoTidy(m_KingDetailEquipInfo.m_BagItemDic[buwei]);
            }
            else
            {
                List<BagItem> listEquip = (from item in BagData.Instance().m_playerEquipDic where item.Value.itemId == Equipid select item.Value).ToList();

                for (int j = 0; j < listEquip.Count; j++)
                {
                    for (int i = 0; i < listEquip.Count - 1 - j; i++)
                    {
                        if (listEquip[i].pinZhi < listEquip[i + 1].pinZhi)
                        {
                            BagItem t = new BagItem();

                            t = listEquip[i];

                            listEquip[i] = listEquip[i + 1];

                            listEquip[i + 1] = t;
                        }
                    }
                }
                EquipsInfoTidy(listEquip[0]);
            }
        }
    }

    private void EquipsInfoTidy(BagItem esr)//分离数据并赋值
    {
        listData.Clear();
        listNames.Clear();
        EquipSuoData.Instance().listIndexs.Clear();

        _AppendInfo.OneSend = "";
        _AppendInfo.TwoSend = "";
        _AppendInfo.ThreeSend = "";
        _AppendInfo.FourSend = "";
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                int[] attribute = { esr.wqSH, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };
                listData.AddRange(attribute);
                for (int j = 0; j < listData.Count; j++)
                {
                    if (listData[j] > 0)
                    {
                        listNames.Add(NameIdTemplate.GetName_By_NameId(listAppendName[j]));

                        if (string.IsNullOrEmpty(_AppendInfo.OneSend))
                        {
                            if (j > 3)
                            {
                                _AppendInfo._OneType = 1;
                            }
                            else
                            {
                                _AppendInfo._OneType = 0;
                            }
                            _AppendInfo.OneSend = listData[j].ToString();
                        }
                        else if (string.IsNullOrEmpty(_AppendInfo.TwoSend))
                        {
                            if (j > 3)
                            {
                                _AppendInfo._TwoType = 1;
                            }
                            else
                            {
                                _AppendInfo._TwoType = 0;
                            }
                            _AppendInfo.TwoSend = listData[j].ToString();
                        }
                        else if (string.IsNullOrEmpty(_AppendInfo.ThreeSend))
                        {
                            if (j > 3)
                            {
                                _AppendInfo._ThreeType = 1;
                            }
                            else
                            {
                                _AppendInfo._ThreeType = 0;
                            }
                            _AppendInfo.ThreeSend = listData[j].ToString();
                        }
                        else if (string.IsNullOrEmpty(_AppendInfo.FourSend))
                        {
                            if (j > 3)
                            {
                                _AppendInfo._FourType = 1;
                            }
                            else
                            {
                                _AppendInfo._FourType = 0;
                            }
                            _AppendInfo.FourSend = listData[j].ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;

                exInfo.Name = ZhuangBei.templates[i].m_name;
                exInfo.des = ZhuangBei.templates[i].funDesc;
                exInfo.level = esr.qiangHuaLv.ToString();
                exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
                levelNeed = ZhuangBei.templates[i].jinjieLv;
                exInfo.EquipMaterialId = ZhuangBei.templates[i].jinjieItem;
                exInfo.EquipMaterialCount = ZhuangBei.templates[i].jinjieNum;
                exInfo.Gong = esr.gongJi.ToString();
                exInfo.Fang = esr.fangYu.ToString();
                exInfo.Ming = esr.shengMing.ToString();
                exInfo.One = _AppendInfo.OneSend;
                exInfo.Two = _AppendInfo.TwoSend;
                exInfo.Three = _AppendInfo.ThreeSend;
                exInfo.Four = _AppendInfo.FourSend;
                exInfo.PinZhi = esr.pinZhi;

                break;
            }
        }
        if (isUpgrade)
        {
            isUpgrade = false;

            RefreshEquipInfo();
        }
        else
        {
            ExhibitionEquip();
        }
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
        ShowWearEquipInfo();
    }

    public void ShowWearEquipInfo()
    {
        materialSendId = int.Parse(exInfo.EquipMaterialId);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (TaskData.Instance.m_iCurMissionIndex == 100090 && TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100090;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 2;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
            }
            else if (TaskData.Instance.m_iCurMissionIndex == 100115 && TaskData.Instance.m_TaskInfoDic[100115].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100115;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else
            {
                CityGlobalData.m_isRightGuide = true;
            }
        }

        EquipInfoShow();

        m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));

        m_EquipDes.text = DescIdTemplate.GetDescriptionById(int.Parse(exInfo.des));
    }

    void EquipInfoShow()
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            listWeaponAttributeCount[0].text = MyColorData.getColorString(ColorID(_AppendInfo._OneType), exInfo.One);
            listWeaponAttributeCount[1].text = MyColorData.getColorString(ColorID(_AppendInfo._TwoType), exInfo.Two);
            listWeaponAttributeCount[2].text = MyColorData.getColorString(ColorID(_AppendInfo._ThreeType), exInfo.Three);
            listWeaponAttributeCount[3].text = MyColorData.getColorString(ColorID(_AppendInfo._FourType), exInfo.Four);
            AttributeNameInfo(listWeaponAttributeName.Count / 2, "[016bc5]");
            m_EquipGong.text = MyColorData.getColorString(10, exInfo.Gong);
            if (listNames.Count == 0)
            {
                m_WeapoSignal2.SetActive(false);
            }
            else
            {
                m_WeapoSignal2.SetActive(true);
            }
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);

            //if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_WAIST || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOSOM || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HELMET)
            //{
            //    if (listNames.Count == 0)
            //    {
            //        m_ArmorSignal2.SetActive(false);
            //        m_ArmorAttrInfo1.SetActive(false);
            //        m_ArmorAttrInfo2.SetActive(false);
            //    }
            //    else if (listNames.Count > 2)
            //    {
            //        m_ArmorSignal2.SetActive(true);
            //        m_ArmorAttrInfo1.SetActive(true);
            //        m_ArmorAttrInfo2.SetActive(true);
            //    }
            //    else
            //    {
            //        m_ArmorSignal2.SetActive(true);
            //        m_ArmorAttrInfo1.SetActive(true);
            //        m_ArmorAttrInfo2.SetActive(false);
            //    }
            //    if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HELMET)
            //    {
            //        listArmorAttributeCount[0].text = MyColorData.getColorString(7, exInfo.One);
            //        listArmorAttributeCount[1].text = MyColorData.getColorString(7, exInfo.Two);
            //        listArmorAttributeCount[2].text = MyColorData.getColorString(7, exInfo.Three);
            //        listArmorAttributeCount[3].text = MyColorData.getColorString(7, exInfo.Four);
            //        AttributeNameInfo(listArmorAttributeName.Count, "[5E01BC]");
            //    }
            //    else
            //    {
            //        listArmorAttributeCount[0].text = MyColorData.getColorString(6, exInfo.One);
            //        listArmorAttributeCount[1].text = MyColorData.getColorString(6, exInfo.Two);
            //        listArmorAttributeCount[2].text = MyColorData.getColorString(6, exInfo.Three);
            //        listArmorAttributeCount[3].text = MyColorData.getColorString(6, exInfo.Four);
            //        AttributeNameInfo(listArmorAttributeName.Count, "[016bc5]");
            //    }
            //}
            //else
            {
                if (listNames.Count == 0)
                {
                    m_ArmorSignal2.SetActive(false);
                    m_ArmorAttrInfo1.SetActive(false);
                    m_ArmorAttrInfo2.SetActive(false);
                }
                else if (listNames.Count > 2)
                {
                    m_ArmorSignal2.SetActive(true);
                    m_ArmorAttrInfo1.SetActive(true);
                    m_ArmorAttrInfo2.SetActive(true);
                }
                else
                {
                    m_ArmorSignal2.SetActive(true);
                    m_ArmorAttrInfo1.SetActive(true);
                    m_ArmorAttrInfo2.SetActive(false);
                }

                listArmorAttributeCount[0].text = MyColorData.getColorString(ColorID(_AppendInfo._OneType), exInfo.One);
                listArmorAttributeCount[1].text = MyColorData.getColorString(ColorID(_AppendInfo._TwoType), exInfo.Two);
                listArmorAttributeCount[2].text = MyColorData.getColorString(ColorID(_AppendInfo._ThreeType), exInfo.Three);
                listArmorAttributeCount[3].text = MyColorData.getColorString(ColorID(_AppendInfo._FourType), exInfo.Four);
                AttributeNameInfo(listArmorAttributeName.Count / 2, "[016bc5]");
            }
            m_EquipFang.text = MyColorData.getColorString(10, exInfo.Fang);
            m_EquipXue.text = MyColorData.getColorString(10, exInfo.Ming);
        }
    }

    private int ColorID(int type)
    {
        if (type == 0)
        {
            return 6;
        }
        else
        {
            return 7;
        }
    }
    private int NameColorID(int index)
    {
        int colorIndex = 0;
        switch (index)
        {
            case 0:
                {
                    if (_AppendInfo._OneType == 0)
                    {
                        colorIndex = 6;
                    }
                    else
                    {
                        colorIndex = 7;
                    }
                }
                break;

            case 1:
                {
                    if (_AppendInfo._TwoType == 0)
                    {
                        colorIndex = 6;
                    }
                    else
                    {
                        colorIndex = 7;
                    }
                }
                break;
            case 2:
                {
                    if (_AppendInfo._ThreeType == 0)
                    {
                        colorIndex = 6;
                    }
                    else
                    {
                        colorIndex = 7;
                    }
                }
                break;
            case 3:
                {
                    if (_AppendInfo._FourType == 0)
                    {
                        colorIndex = 6;
                    }
                    else
                    {
                        colorIndex = 7;
                    }
                }
                break;
        }
        return colorIndex;
    }
    void AttributeNameInfo(int index, string color)
    {
        if (listNames.Count > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i < listNames.Count)
                {
                    if (equipType == 0)
                    {
                        EquipSuoData.Instance().listColor.Add(NameColorID(i));
                        listArmorAttributeName[i].text = MyColorData.getColorString(NameColorID(i), listNames[i]);
                        //if (i < index)
                        //{
                        //    //EquipSuoData.Instance().listColor.Add(color);
                        //    //if (color.Equals("[016bc5]"))
                        //    //{
                        //    //    listArmorAttributeName[i].text = MyColorData.getColorString(6, listNames[i]);
                        //    //}
                        //    //else if (color.Equals("[5E01BC]"))
                        //    //{
                        //    //    listArmorAttributeName[i].text = MyColorData.getColorString(7, listNames[i]);
                        //    //}
                        //}
                        //else
                        //{
                        //    EquipSuoData.Instance().listColor.Add("[5E01BC]");
                        //    listArmorAttributeName[i].text = MyColorData.getColorString(7, listNames[i]);
                        //}
                    }
                    else
                    {
                        EquipSuoData.Instance().listColor.Add(NameColorID(i));
                        listWeaponAttributeName[i].text = MyColorData.getColorString(NameColorID(i), listNames[i]);
                        //if (i < index)
                        //{
                        //    EquipSuoData.Instance().listColor.Add(color);
                        //    listWeaponAttributeName[i].text = color + listNames[i] + "[-]";
                        //}
                        //else
                        //{
                        //    EquipSuoData.Instance().listColor.Add("[5E01BC]");
                        //    listWeaponAttributeName[i].text = MyColorData.getColorString(7, listNames[i]);
                        //}
                    }
                }
                else
                {
                    if (equipType == 0)
                    {
                        listArmorAttributeName[i].text = "";
                    }
                    else
                    {
                        listWeaponAttributeName[i].text = "";
                    }
                }
            }
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                if (equipType == 0)
                {
                    listArmorAttributeName[j].text = "";
                }
                else
                {
                    listWeaponAttributeName[j].text = "";
                }
            }
        }
    }

    private void Touched(GameObject obj)
    {
        gameObject.SetActive(false);
    }

    private void RefreshEquipInfo()
    {
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId)
            {
                EquipSaveId = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId;

                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;
                exInfo.Name = ZhuangBei.templates[i].m_name;
                exInfo.des = ZhuangBei.templates[i].funDesc;
                exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
                exInfo.EquipMaterialId = ZhuangBei.templates[i].jinjieItem;
                exInfo.EquipMaterialCount = ZhuangBei.templates[i].jinjieNum;
                exInfo.PinZhi = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].pinZhi;
                exInfo.Gong = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].gongJi.ToString();
                exInfo.Fang = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].fangYu.ToString();
                exInfo.Ming = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].shengMing.ToString();
                exInfo.MaxLevel = 0;
                break;
            }
        }

        materialSendId = int.Parse(exInfo.EquipMaterialId);
        if (int.Parse(exInfo.level) > 0)
        {
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        else
        {
            m_EquipName.text = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(exInfo.Name)));
        }
        m_EquipDes.text = DescIdTemplate.GetDescriptionById(int.Parse(exInfo.des));
        int tttt = 0;

        int index = 0;

        foreach (KeyValuePair<long, List<BagItem>> item in BagData.Instance().m_playerCaiLiaoDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].itemId == int.Parse(exInfo.EquipMaterialId))
                {
                    index = item.Value[i].cnt;
                }
            }
        }

        m_EventHandler.gameObject.SetActive(false);

        RefreshInfo();
    }

    void RefreshInfo()
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            listWeaponAttributeCount[0].text = MyColorData.getColorString(ColorID(_AppendInfo._OneType), exInfo.One);
            listWeaponAttributeCount[1].text = MyColorData.getColorString(ColorID(_AppendInfo._TwoType), exInfo.Two);
            listWeaponAttributeCount[2].text = MyColorData.getColorString(ColorID(_AppendInfo._ThreeType), exInfo.Three);
            listWeaponAttributeCount[3].text = MyColorData.getColorString(ColorID(_AppendInfo._FourType), exInfo.Four);
            AttributeNameInfo(listWeaponAttributeName.Count / 2, "[016bc5]");

            if (listNames.Count == 0)
            {
                m_WeapoSignal2.SetActive(false);
            }
            else
            {
                m_WeapoSignal2.SetActive(true);
            }
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);

            //if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_WAIST || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOSOM || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HELMET)
            //{
            //    if (listNames.Count == 0)
            //    {
            //        m_ArmorSignal2.SetActive(false);
            //    }
            //    else if (listNames.Count > 2)
            //    {
            //        m_ArmorSignal2.SetActive(true);
            //        m_ArmorAttrInfo1.SetActive(false);
            //        m_ArmorAttrInfo2.SetActive(true);
            //    }
            //    else
            //    {
            //        m_ArmorSignal2.SetActive(true);
            //        m_ArmorAttrInfo1.SetActive(true);
            //        m_ArmorAttrInfo2.SetActive(false);
            //    }
            //    if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HELMET)
            //    {
            //        listArmorAttributeCount[0].text = MyColorData.getColorString(7, exInfo.One);
            //        listArmorAttributeCount[1].text = MyColorData.getColorString(7, exInfo.Two);
            //        listArmorAttributeCount[2].text = MyColorData.getColorString(7, exInfo.Three);
            //        listArmorAttributeCount[3].text = MyColorData.getColorString(7, exInfo.Four);
            //        AttributeNameInfo(listArmorAttributeName.Count, "[5E01BC]");
            //    }
            //    else
            //    {
            //        listArmorAttributeCount[0].text = MyColorData.getColorString(6, exInfo.One);
            //        listArmorAttributeCount[1].text = MyColorData.getColorString(6, exInfo.Two);
            //        listArmorAttributeCount[2].text = MyColorData.getColorString(6, exInfo.Three);
            //        listArmorAttributeCount[3].text = MyColorData.getColorString(6, exInfo.Four);
            //        AttributeNameInfo(listArmorAttributeName.Count, "[016bc5]");
            //    }
            //}
            //else
            {
                if (listNames.Count == 0)
                {
                    m_ArmorSignal2.SetActive(false);
                }
                else
                {
                    m_ArmorSignal2.SetActive(true);
                }
                m_ArmorAttrInfo1.SetActive(true);
                m_ArmorAttrInfo2.SetActive(false);
                listArmorAttributeCount[0].text = MyColorData.getColorString(6, exInfo.One);
                listArmorAttributeCount[1].text = MyColorData.getColorString(6, exInfo.Two);
                listArmorAttributeCount[2].text = MyColorData.getColorString(7, exInfo.Three);
                listArmorAttributeCount[3].text = MyColorData.getColorString(7, exInfo.Four);
                AttributeNameInfo(listArmorAttributeName.Count / 2, "[016bc5]");
            }
        }
    }

    private BagItem EquipsUpContain(int buwei)
    {
        if (m_KingDetailEquipInfo.m_BagItemDic.ContainsKey(buwei))
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
                if (tempBuwei == buwei && item.Value.pinZhi > m_KingDetailEquipInfo.m_BagItemDic[tempBuwei].pinZhi)
                {
                    listEquip.Add(item.Value);
                }
            }

            if (listEquip.Count > 1)
            {
                for (int j = 0; j < listEquip.Count; j++)
                {
                    for (int i = 0; i < listEquip.Count - 1 - j; i++)
                    {
                        if (listEquip[i].pinZhi < listEquip[i + 1].pinZhi)
                        {
                            BagItem t = new BagItem();

                            t = listEquip[i];
                            listEquip[i] = listEquip[i + 1];
                            listEquip[i + 1] = t;
                        }
                    }
                }
                return listEquip[0];
            }
            else if (listEquip.Count == 1)
            {
                return listEquip[0];
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}