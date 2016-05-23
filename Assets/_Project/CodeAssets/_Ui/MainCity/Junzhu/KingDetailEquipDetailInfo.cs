using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KingDetailEquipDetailInfo : MonoBehaviour, SocketProcessor, UI2DEventListener
{
    public KingDetailEquipInfo m_KingDetailEquipInfo;

    public List<EventHandler> ListEventHandler;
    public GameObject m_ButtonClose;
    public UILabel m_EquipName;
    public UILabel m_EquipDes;
    public UILabel m_EquipGong;
    public UILabel m_EquipFang;
    public UILabel m_EquipXue;
    public GameObject m_ArmorAttr;
    public GameObject m_WeaponAttr;
    public GameObject m_ArmorAttrInfo1;
    public GameObject m_WeapoSignal2;

    public GameObject m_Hidden;

    public GameObject m_ObjIcon;

    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;

    public GameObject m_BottomButton;
    private int EquipSaveId;

    private int BuWeiSave;

    private bool isUpgrade = false;
    public UIGrid m_ObjGrid;

    public List<GameObject> m_listSignal;
    private float _ValueSave = 0;
    public static bool m_isJinJie = false;

    long _dbid;

    private struct ExibiteInfo
    {
        public string EquipIcon;
        public string EquipMaterialIcon;
        public string Name;
        public string des;
        public string level;
        public string Condition;
        public string Gong;
        public string Fang;
        public string Ming;
        public string NextGong;
        public string NextFang;
        public string NextMing;
        public string EquipMaterialCount;
        public string EquipMaterialId;
        public int PinZhi;
        public float _Value;
        public bool _isCanAdvance;
        public bool _isWuQi;
        public bool _isProgress;
        public bool _isDiaoLuo;
        public string _DiaoLuoSigNal;
        public int _JinjieId;
    };

    private ExibiteInfo exInfo = new ExibiteInfo();

    void Start()
    {
        ListEventHandler.ForEach(item => item.m_click_handler += Touched);
    }

    void OnEnable()
    {
        m_ButtonClose.SetActive(true);
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        {

        }
        else if (FreshGuide.Instance().IsActive(100405) && TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
        {

        }
        else
        {
            UIYindao.m_UIYindao.CloseUI();
        }

        SocketTool.RegisterMessageProcessor(this);
    }

    void OnDisable()
    {
        if (m_ButtonClose != null)
        {
            m_ButtonClose.SetActive(false);
        }

        UI3DEffectTool.ClearUIFx(m_SpritePinZhi.gameObject);

        SocketTool.UnRegisterMessageProcessor(this);
    }

    public void OnUI2DShow()
    {
        GetEquipInfo(EquipSaveId, BuWeiSave);
    }

    void Update()
    {
        //if (EquipsOfBody.Instance().m_RefrsehEquipsInfo)
        //{
        //    EquipsOfBody.Instance().m_RefrsehEquipsInfo = false;

        //    if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress >= 0)
        //    {
        //        TaskData.Instance.m_iCurMissionIndex = 100100;
        //        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
        //        tempTaskData.m_iCurIndex = 5;
        //        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        //    }
        //    else
        //    {
        //        UIYindao.m_UIYindao.CloseUI();
        //    }

        //    isUpgrade = true;
        //    PushAndNotificationHelper.SetRedSpotNotification(1211, EquipSuoData.AllUpgrade());
        //    ShowEffert(m_SpritePinZhi.gameObject, 100166);
        //    ShowEffert(m_SpritePinZhi.gameObject, 600155);
        //    DataInfo();
        //    EquipSaveId = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId;
        //    GetEquipInfo(EquipSaveId, BuWeiSave);
        //    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_ADVANCE),
        //                    ResLoadedSimple);
        //}
    }

    void DataInfo()
    {
        if (ZhuangBei.GetItemByID(EquipSaveId).jiejieId != 0)
        {
            FunctionWindowsCreateManagerment.EquipAdvanceInfo ainfo = new FunctionWindowsCreateManagerment.EquipAdvanceInfo();
            ainfo._equipid = EquipSaveId;
            ainfo._nextid = ZhuangBei.GetItemByID(EquipSaveId).jiejieId;


            if (!string.IsNullOrEmpty(exInfo.NextGong))
            {
                ainfo._gong = int.Parse(exInfo.Gong);
                ainfo._fang = 0;
                ainfo._ming = 0;
                ainfo._gongadd = int.Parse(exInfo.NextGong) - int.Parse(exInfo.Gong);
                ainfo._fanggadd = 0;
                ainfo._minggadd = 0;
            }
            else
            {
                ainfo._gong = 0;
                ainfo._fang = int.Parse(exInfo.Fang);
                ainfo._ming = int.Parse(exInfo.Ming);
                ainfo._gongadd = 0;
                ainfo._fanggadd = int.Parse(exInfo.NextFang) - int.Parse(exInfo.Fang);
                ainfo._minggadd = int.Parse(exInfo.NextMing) - int.Parse(exInfo.Ming);
            }
            FunctionWindowsCreateManagerment.m_AdvanceInfo = ainfo;
        }
    }
    void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
    }

    public void GetEquipInfo(int Equipid, int buwei)//获得对应装备信息
    {
        BuWeiSave = buwei;
        EquipSaveId = Equipid;
        exInfo.PinZhi = CommonItemTemplate.getCommonItemTemplateById(Equipid).color;
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                exInfo._JinjieId = ZhuangBei.templates[i].jiejieId;
                if (m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].qiangHuaLv > 0)
                {
                    exInfo.Name = NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name)) + "+" + MyColorData.getColorString(4, m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].qiangHuaLv.ToString());
                }
                else
                {
                    exInfo.Name = NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name));
                }


                if (exInfo._JinjieId != 0)
                {
                    if (BuWeiSave == 3 || BuWeiSave == 4 || BuWeiSave == 5)
                    {
                        if (EquipNextUpGradeCommonattribute.CommomAttribute(m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave], m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId)._gongJiAfter != 0)
                        {
                            exInfo.NextGong = EquipNextUpGradeCommonattribute.CommomAttribute(m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave]
                                , m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId)._gongJiAfter.ToString();

                        }
                        else
                        {
                            exInfo.NextGong = "";
                        }
                        exInfo.NextFang = "";
                        exInfo.NextMing = "";
                    }
                    else
                    {
                        exInfo.NextGong = "";
                        if (EquipNextUpGradeCommonattribute.CommomAttribute(m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave], m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].itemId)._fangYuAfter != 0)
                        {
                            exInfo.NextFang = (int.Parse(ZhuangBei.getZhuangBeiById(ZhuangBei.templates[i].jiejieId).fangyu) - int.Parse(ZhuangBei.getZhuangBeiById(EquipSaveId).fangyu) + m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].fangYu).ToString();
                            exInfo.NextMing = (int.Parse(ZhuangBei.getZhuangBeiById(ZhuangBei.templates[i].jiejieId).shengming) - int.Parse(ZhuangBei.getZhuangBeiById(EquipSaveId).shengming) + m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].shengMing).ToString();
                        }
                        else
                        {
                            exInfo.NextFang = "";
                            exInfo.NextMing = "";
                        }
                    }

                    exInfo.EquipMaterialId = ZhuangBei.templates[i].jinjieItem;

                    int MaterialCount = GetMaterialCountByID(int.Parse(ZhuangBei.templates[i].jinjieItem));
                    exInfo.EquipMaterialIcon = ZhuangBei.templates[i].jinjieItem;
                    exInfo.EquipMaterialCount = MaterialCount.ToString() + "/" + ZhuangBei.templates[i].jinjieNum;
                    exInfo._Value = MaterialCount / float.Parse(ZhuangBei.templates[i].jinjieNum);

                    exInfo._isProgress = true;
                }
                else
                {
                    exInfo._isProgress = false;
                }
                exInfo.des = DescIdTemplate.GetDescriptionById(int.Parse(ZhuangBei.templates[i].funDesc));
                exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
                break;
            }
        }

        if (BuWeiSave == 3 || BuWeiSave == 4 || BuWeiSave == 5)
        {
            exInfo.Gong = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].gongJi.ToString();
            exInfo._isWuQi = true;
        }
        else
        {
            exInfo.Fang = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].fangYu.ToString();
            exInfo.Ming = m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].shengMing.ToString();
            exInfo._isWuQi = false;
        }


        if (exInfo._JinjieId != 0)
        {
            exInfo._isDiaoLuo = true;

            EquipsInfoTidy(m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave]);
        }
        else
        {

            exInfo._isDiaoLuo = false;
            EquipsInfoTidy(m_KingDetailEquipInfo.m_BagItemDic[buwei]);
        }
    }


    public void EquipAllInfo()//对应装备信息
    {
        exInfo.EquipIcon = EquipSaveId.ToString();

    }
    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.PVE_MAX_ID_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        GuanQiaMaxId tempInfo = new GuanQiaMaxId();

                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                    }
                    return true;
                default: return false;
            }
        }
        return false;

    }

    private List<EquipSuoData.WashInfo> _listAttribute;

    private void EquipsInfoTidy(BagItem esr)//分离数据并赋值
    {
        _listAttribute = new List<EquipSuoData.WashInfo>();
        EquipSuoData.Instance().listIndexs.Clear();

        int[] attribute = { esr.wqSH, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };

        for (int j = 0; j < attribute.Length; j++)
        {
            if (attribute[j] > 0)
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
                wss._count = attribute[j];
                _listAttribute.Add(wss);
            }
        }

        int size_shuxing = _listAttribute.Count;
        int size = m_ObjGrid.transform.childCount;
        if (size > 0)
        {
            if (size_shuxing >= size)
            {
                for (int k = 0; k < size_shuxing; k++)
                {
                    if (k < size)
                    {
                        m_ObjGrid.transform.GetChild(k).GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(_listAttribute[k], false, false);
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
                        m_ObjGrid.transform.GetChild(k).GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowInfo(_listAttribute[k], false, false);
                    }
                    else
                    {
                        Destroy(m_ObjGrid.transform.GetChild(k).gameObject);
                    }
                }
            }
        }
        else
        {
            for (int k = 0; k < size_shuxing; k++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_SPECIAL_ITEM), ResourcesLoadCallBack2);
            }
        }
        exInfo.level = esr.qiangHuaLv.ToString();
        int size_s = m_listSignal.Count;
        for (int i = 0; i < size_s; i++)
        {
            if (i < _listAttribute.Count)
            {
                m_listSignal[i].SetActive(false);
            }
            else
            {
                m_listSignal[i].SetActive(true);
            }
        }
        ShowWearEquipInfo();
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
    }

    public void ShowWearEquipInfo()
    {
        EquipInfoShow();
    }

    int equipType = 1;

    void EquipInfoShow()
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            m_EquipGong.text = exInfo.Gong;
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);
            m_EquipFang.text = exInfo.Fang;
            m_EquipXue.text = exInfo.Ming;
        }


        m_EquipName.text = exInfo.Name;

        m_EquipDes.text = exInfo.des;
        m_SpriteIcon.spriteName = exInfo.EquipIcon;
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(exInfo.PinZhi))
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 100;
        }
        else
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 89;
        }
        m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(exInfo.PinZhi);

        m_ObjIcon.SetActive(true);

        m_Hidden.SetActive(true);
    }

    private void ShowEffert(GameObject obj, int index)
    {
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, obj, EffectIdTemplate.GetPathByeffectId(index), null);
    }

    private void Touched(GameObject obj)
    {
        {
            //if (obj.name.Equals("ButtondAdvance"))
            //{
            //    obj.GetComponent<Collider>().enabled = false;
            //    if (!FunctionOpenTemp.GetWhetherContainID(1211))
            //    {
            //        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
            //                                  UIBoxLoadCallback);
            //    }
            //    else
            //    {
            //        if (m_isJinJie)
            //        {
            //            return;
            //        }
            //        m_isJinJie = true;
            //        EquipsOfBody.Instance().EquipAdvace(m_KingDetailEquipInfo.m_BagItemDic[BuWeiSave].dbId);
            //    }
            //}
            //else if (obj.name.Equals("ButtondWear"))
            //{
            //    obj.GetComponent<Collider>().enabled = false;
            //    EquipsOfBody.Instance().m_WearEquip = true;

            //    if (UIYindao.m_UIYindao.m_isOpenYindao)
            //    {
            //        CityGlobalData.m_isRightGuide = false;
            //        if (TaskData.Instance.m_iCurMissionIndex == 100150 && TaskData.Instance.m_TaskInfoDic[100150].progress >= 0)
            //        {
            //            UIYindao.m_UIYindao.CloseUI();
            //        }
            //        else
            //        {
            //            CityGlobalData.m_isRightGuide = true;
            //        }
            //    }
            //    EquipAddReq tempAddReq = new EquipAddReq(); //装备在背包中下标
            //    Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
            //    foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
            //    {
            //        if (item.Value.itemId == EquipSaveId)
            //        {
            //            tempAddReq.gridIndex = item.Value.bagIndex;
            //            EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
            //            break;
            //        }
            //    }

            //    MemoryStream tempStream = new MemoryStream();
            //    QiXiongSerializer t_qx = new QiXiongSerializer();
            //    t_qx.Serialize(tempStream, tempAddReq);

            //    byte[] t_protof;
            //    t_protof = tempStream.ToArray();
            //    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipAdd, ref t_protof);

            //}
            //else if (obj.name.Equals("ButtondChange"))//装备替换
            //{
            //    obj.GetComponent<Collider>().enabled = false;
            //    EquipsOfBody.Instance().m_WearEquip = true;
            //    if (UIYindao.m_UIYindao.m_isOpenYindao)
            //    {
            //        if (TaskData.Instance.m_iCurMissionIndex == 100090 && TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
            //        {
            //            TaskData.Instance.m_iCurMissionIndex = 100090;
            //            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //        }
            //        else if (TaskData.Instance.m_iCurMissionIndex == 100115 && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
            //        {
            //            TaskData.Instance.m_iCurMissionIndex = 100115;
            //            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            //            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            //        }
            //        else
            //        {
            //            UIYindao.m_UIYindao.CloseUI();
            //        }
            //    }
            //    EquipAddReq tempAddReq = new EquipAddReq(); //装备在背包中下标
            //    Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
            //    foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
            //    {
            //        if (item.Value.itemId == EquipSaveId)
            //        {
            //            tempAddReq.gridIndex = item.Value.bagIndex;
            //            EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
            //            break;
            //        }
            //    }

            //    MemoryStream tempStream = new MemoryStream();
            //    QiXiongSerializer t_qx = new QiXiongSerializer();
            //    t_qx.Serialize(tempStream, tempAddReq);

            //    byte[] t_protof;
            //    t_protof = tempStream.ToArray();

            //    SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipAdd, ref t_protof);
            //}
            //else
            {
                m_BottomButton.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }

    public void UIBoxLoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.HUANGYE_19);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        string str1 = NameIdTemplate.GetName_By_NameId(990043) + ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1211)) + NameIdTemplate.GetName_By_NameId(990044) + "!";

        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str1), "", null, confirmStr, null, null, null, null);
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

    int index_ShuXing = 0;

    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_ObjGrid != null)
        {
            GameObject rewardShow = Instantiate(p_object) as GameObject;
            rewardShow.transform.parent = m_ObjGrid.transform;
            rewardShow.transform.localScale = Vector3.one;
            rewardShow.transform.localPosition = Vector3.zero;
            rewardShow.transform.GetComponent<EquipGrowthSpecialAttributeManagerment>().ShowEquipInfo(_listAttribute[index_ShuXing]);
            m_ObjGrid.repositionNow = true;
            if (index_ShuXing < _listAttribute.Count - 1)
            {
                index_ShuXing++;
            }
        }
        else
        {
            p_object = null;
        }
    }

    private int _indexNum = 0;

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

    void GoTo(int id)
    {
        WindowBackShowController.m_SaveEquipBuWei = BuWeiSave;
        EnterGuoGuanmap.Instance().ShouldOpen_id = id;
    }
}