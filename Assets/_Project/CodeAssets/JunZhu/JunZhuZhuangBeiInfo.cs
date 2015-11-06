using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class JunZhuZhuangBeiInfo : MonoBehaviour, SocketProcessor
{
    public List<EventHandler> ListEventHandler;
 
    public UILabel m_EquipName;
    public UILabel m_EquipDes;
    public GameObject m_MainPaent;
    public UILabel m_EquipGong;
    public UILabel m_EquipFang;
    public UILabel m_EquipXue;
    public GameObject m_ArmorAttr;
    public GameObject m_WeaponAttr;
    public GameObject m_Junzhu;
    public GameObject m_ArmorAttrInfo1;
    public GameObject m_WeapoSignal2;
    public GameObject m_RightObj;

    public GameObject m_TopRight;
    public GameObject m_MidLeft;

    public GameObject m_WuQi;
    public UILabel m_WuQiGong;
   
    public GameObject m_Equip;
    public UILabel m_EquipFang2;
    public UILabel m_EquipXue2;
    public GameObject m_NextAdvance;
    public UILabel m_NextGong;
    public UILabel m_NextFang;
    public UILabel m_NextXue;
    public UILabel m_HuoDeText;
    public GameObject m_Hidden;
    public GameObject m_MaxPinZhi;
    public GameObject m_ObjIcon;

    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;
    public UIProgressBar m_Progress;
    public UILabel m_labProgress;
    private int EquipSaveId;

    private int BuWeiSave;
    private bool WearIsOn = false;
    private int materialSendId = 0;
 
 
    //  private int SaveInType = 0;
    private bool isUpgrade = false;
    private int levelNeed = 0;
    public UIGrid m_ObjGrid;
    public UIGrid m_ObjGrid2;
    public List<GameObject> m_listSignal;
    public struct DiaoLuoGuanQia
    {
        public string GuanQiaName;
        public bool _isChuanQi;
        public bool _isOpen;
        public int _id;
        public int _SetionId;
    };
 
    long _dbid;
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

        public List<DiaoLuoGuanQia> _listGuanQia;
    };

    private ExibiteInfo exInfo = new ExibiteInfo();
    private GuanQiaMaxId MapCurrentInfo = null;
    void Start()
    {
        ListEventHandler.ForEach(item => item.m_handler += Touched);
    }
    void OnEnable()
    {
        m_RightObj.SetActive(false);
        SocketTool.RegisterMessageProcessor(this);
    }

    void OnDisable()
    {
        UI3DEffectTool.Instance().ClearUIFx(m_SpritePinZhi.gameObject);
 
        SocketTool.UnRegisterMessageProcessor(this);
    }

    void Update()
    {
        if (EquipsOfBody.Instance().m_RefrsehEquipsInfo)
        {
             
            EquipsOfBody.Instance().m_RefrsehEquipsInfo = false;
            isUpgrade = true;
            PushAndNotificationHelper.SetRedSpotNotification(1211, EquipSuoData.AllUpgrade());
            ShowEffert(m_SpritePinZhi.gameObject, 100166);
            ShowEffert(m_SpritePinZhi.gameObject, 600155);
            EquipSaveId = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId;
            //  EquipsInfoTidy(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave]);
            GetEquipInfo(EquipSaveId, BuWeiSave);

        }
    }

    public void GetEquipInfo(int Equipid,int buwei)//获得对应装备信息
    {
        BuWeiSave = buwei;
        EquipSaveId = Equipid;

        exInfo.PinZhi = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].pinZhi;
        exInfo._listGuanQia = new List<DiaoLuoGuanQia>();
        exInfo._listGuanQia.Clear();
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
        {
            if (ZhuangBei.templates[i].id == EquipSaveId)
            {
                exInfo.EquipIcon = ZhuangBei.templates[i].icon;
                if (EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].qiangHuaLv > 0)
                {
                    exInfo.Name = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name)) + "+" + EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].qiangHuaLv.ToString());
                }
                else
                {
                    exInfo.Name = MyColorData.getColorString(10, NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.templates[i].m_name)));
                }


                if (exInfo.PinZhi < 11)
                {
                    exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;
                    if (BuWeiSave == 3 || BuWeiSave == 4 || BuWeiSave == 5)
                    {
                        if (EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave], EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId)._gongJiAfter != 0)
                        {
                            exInfo.NextGong = EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave], EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId)._gongJiAfter.ToString();

                        }
                        else
                        {
                            exInfo.NextGong = "";
                        }
                          //  (int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(ZhuangBei.templates[i].jinjieIcon)).gongji) - int.Parse(ZhuangBei.getZhuangBeiById(EquipSaveId).gongji) + EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].gongJi).ToString();
                        exInfo.NextFang = "";
                        exInfo.NextMing = "";
                    }
                    else
                    {
                        exInfo.NextGong = "";
                        if (EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave], EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].itemId)._fangYuAfter != 0)
                        {
                            exInfo.NextFang = (int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(ZhuangBei.templates[i].jinjieIcon)).fangyu) - int.Parse(ZhuangBei.getZhuangBeiById(EquipSaveId).fangyu) + EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].fangYu).ToString();
                            exInfo.NextMing = (int.Parse(ZhuangBei.getZhuangBeiById(int.Parse(ZhuangBei.templates[i].jinjieIcon)).shengming) - int.Parse(ZhuangBei.getZhuangBeiById(EquipSaveId).shengming) + EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].shengMing).ToString();
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
            exInfo.Gong = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].gongJi.ToString();
            exInfo._isWuQi = true;
        }
        else
        {
            exInfo.Fang = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].fangYu.ToString();
            exInfo.Ming = EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].shengMing.ToString();
            exInfo._isWuQi = false;
        }

        if (exInfo.PinZhi < 11)
        {
            exInfo._isDiaoLuo = true;
            if (MapCurrentInfo == null)
            {
                RequestDiaoLuoLevel();
            }
            else
            {
                QuerySection();
            }
        }
        else
        {
            m_Hidden.SetActive(true);
            exInfo._isDiaoLuo = false;
            EquipsInfoTidy(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei]);
        }
    }
    public void RequestDiaoLuoLevel()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_MAX_ID_REQ);
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
                        MapCurrentInfo = tempInfo;
                        QuerySection();
                    }
                    return true;
                default: return false;
            }
        }
        return false;

    }

    private void QuerySection()//查询掉落关卡是否为当前章节关卡
    {
        m_Hidden.SetActive(true);
        for (int i = 0; i < DiaoLuoTemplate.templates.Count; i++)
        {
            if (DiaoLuoTemplate.templates[i].itemId == int.Parse(exInfo.EquipMaterialIcon))
            {
                if (DiaoLuoTemplate.templates[i].PveIds.IndexOf(":") > -1)
                {
                    if (DiaoLuoTemplate.templates[i].PveIds.IndexOf("|") > -1)
                    {
                        string[] data = DiaoLuoTemplate.templates[i].PveIds.Split('|');
                        for (int j = 0; j < data.Length; j++)
                        {
                            string[] sss = data[j].Split(':');
                            DiaoLuoGuanQia ddl = new DiaoLuoGuanQia();
                            ddl._id = int.Parse(sss[1]);
                            ddl._SetionId = PveTempTemplate.GetPveTemplateGuanQiaId(ddl._id).bigId;
                            ddl.GuanQiaName = NameIdTemplate.GetName_By_NameId(PveTempTemplate.GetPveTemplateGuanQiaId(ddl._id).smaName);
                            if (sss[0].Equals("1"))
                            {
                                ddl._isChuanQi = true;
                                ddl._isOpen = (MapCurrentInfo.chuanQiId >= ddl._id) ? true : false;
                            }
                            else
                            {
                                ddl._isChuanQi = false;
                                ddl._isOpen = (MapCurrentInfo.commonId >= ddl._id) ? true : false;
                            }
                            exInfo._listGuanQia.Add(ddl);
                        }
                        exInfo._DiaoLuoSigNal = "";
                    }
                    else
                    {
                        string[] sss = DiaoLuoTemplate.templates[i].PveIds.Split('：');
                        DiaoLuoGuanQia ddl = new DiaoLuoGuanQia();
                        ddl._id = int.Parse(sss[1]);
                        ddl._SetionId = PveTempTemplate.GetPveTemplateGuanQiaId(ddl._id).bigId;
                        ddl.GuanQiaName = NameIdTemplate.GetName_By_NameId(PveTempTemplate.GetPveTemplateGuanQiaId(ddl._id).smaName);
                        if (sss[0].Equals("1"))
                        {
                            ddl._isChuanQi = true;
                            ddl._isOpen = (MapCurrentInfo.chuanQiId >= ddl._id) ? true : false;
                        }
                        else
                        {
                            ddl._isChuanQi = false;
                            ddl._isOpen = (MapCurrentInfo.commonId >= ddl._id) ? true : false;
                        }
                        exInfo._listGuanQia.Add(ddl);
                    }
                    exInfo._DiaoLuoSigNal = "";
                }
                else
                {
                    exInfo._DiaoLuoSigNal = DiaoLuoTemplate.templates[i].PveIds;
                }
            }
        }
        EquipsInfoTidy(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave]);
    }
    private List<EquipSuoData.WashInfo> _listAttribute;
    private void EquipsInfoTidy(BagItem esr)//分离数据并赋值
    {
        _listAttribute = new List<EquipSuoData.WashInfo>();
        EquipSuoData.Instance().listIndexs.Clear();

        //for (int i = 0; i < ZhuangBei.templates.Count; i++)
        //{
        //    if (ZhuangBei.templates[i].id == EquipSaveId)
        //    {
        int[] attribute = { esr.wqSH, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };

        //    int[] attribute_Max = { esr.wqSH, esr.wqJM, esr.wqBJ, esr.wqRX, esr.jnSH, esr.jnJM, esr.jnBJ, esr.jnRX };

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


        //;
        //exInfo.EquipUpgradeIcon = ZhuangBei.templates[i].jinjieIcon;

        
        //exInfo.des = ZhuangBei.templates[i].funDesc;
        exInfo.level = esr.qiangHuaLv.ToString();
        //exInfo.Condition = ZhuangBei.templates[i].jinjieLv.ToString();
        //levelNeed = ZhuangBei.templates[i].jinjieLv;

        //exInfo.EquipMaterialCount = ZhuangBei.templates[i].jinjieNum;
        //exInfo.Gong = esr.gongJi.ToString();
        //exInfo.Fang = esr.fangYu.ToString();
        //exInfo.Ming = esr.shengMing.ToString();
        //exInfo.PinZhi = esr.pinZhi;

        //        break;
        //    }
        //}
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

      //  ShowWearEquipInfo();
    }

    public void ShowWearEquipInfo()
    {
       // materialSendId = int.Parse(exInfo.EquipMaterialId);
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            if (FreshGuide.Instance().IsActive(100110)&& TaskData.Instance.m_iCurMissionIndex == 100110 && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
            {
 
            }
            else if (FreshGuide.Instance().IsActive(100270) && TaskData.Instance.m_iCurMissionIndex == 100270 && TaskData.Instance.m_TaskInfoDic[100270].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100270;
                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 3;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            else
            {
                CityGlobalData.m_isRightGuide = true;
            }
        }
        EquipInfoShow();
    }
    int equipType = 1;
    void EquipInfoShow()
    {
        if (BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_HEAVY_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_LIGHT_WEAPONS || BuWeiSave == (int)JunzhuEquipPartEnum.E_EQUIP_BOW)
        {
            equipType = 1;
            m_EquipGong.text = MyColorData.getColorString(10, exInfo.Gong);
            m_ArmorAttr.SetActive(false);
            m_WeaponAttr.SetActive(true);
        }
        else
        {
            equipType = 0;
            m_ArmorAttr.SetActive(true);
            m_WeaponAttr.SetActive(false);
            m_EquipFang.text = MyColorData.getColorString(10, exInfo.Fang);
            m_EquipXue.text = MyColorData.getColorString(10, exInfo.Ming);
        }

 
        m_EquipName.text = MyColorData.getColorString(10,  exInfo.Name);// + "  " + MyColorData.getColorString(4, "+" + exInfo.level);
 
        m_EquipDes.text = exInfo.des;
        m_SpriteIcon.spriteName = exInfo.EquipIcon;
        m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(exInfo.PinZhi);
        //ShowEffert(m_SpritePinZhi.gameObject, 100166);
        //ShowEffert(m_SpritePinZhi.gameObject, 600155);
        m_WuQi.SetActive(exInfo._isWuQi);
        m_WuQiGong.text = exInfo.Gong;
        m_Equip.SetActive(!exInfo._isWuQi);
        m_EquipFang2.text = exInfo.Fang;
        m_EquipXue2.text = exInfo.Ming;
        m_NextAdvance.SetActive(exInfo.PinZhi < 11 ? true : true);
        m_NextGong.text = exInfo.NextGong;
        m_NextFang.text = exInfo.NextFang;
        m_NextXue.text = exInfo.NextMing;
        m_HuoDeText.text = exInfo._DiaoLuoSigNal;
        m_MaxPinZhi.SetActive(exInfo.PinZhi >= 11 ? true : false);
        m_ObjIcon.SetActive(true);
        m_Progress.gameObject.SetActive(exInfo._isProgress);
        m_Progress.value = exInfo._Value;
        m_labProgress.text= exInfo.EquipMaterialCount;
 
        if (exInfo.PinZhi > 10 || exInfo._Value < 1 || JunZhuData.Instance().m_junzhuInfo.level < int.Parse(exInfo.Condition))
        {
            ListEventHandler[1].transform.GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        }
        else
        {
            ListEventHandler[1].transform.GetComponent<ButtonColorManagerment>().ButtonsControl(true);
        }
        if (m_TopRight.transform.childCount > 0)
        {
            Destroy(m_TopRight.transform.GetChild(0).gameObject);
        }
        if (exInfo.PinZhi < 11)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                                    ResLoadedTopRight);
        }

        if (m_MidLeft.transform.childCount > 0)
        {
            Destroy(m_MidLeft.transform.GetChild(0).gameObject);
        }
        if (exInfo.PinZhi < 11)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                              ResLoadedMiddleLeft);
        }

        if (m_ObjGrid2.transform.childCount > 0)
        {
            int size_c = m_ObjGrid2.transform.childCount;
            for (int i = 0; i < size_c; i++)
            {
                Destroy(m_ObjGrid2.transform.GetChild(i).gameObject);
            }
        }
        if (exInfo.PinZhi < 11 && exInfo._listGuanQia.Count > 0)
        {
            int size_a = exInfo._listGuanQia.Count;

            if (FreshGuide.Instance().IsActive(100260) && TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
            {
                TaskData.Instance.m_iCurMissionIndex = 100260;

                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                tempTaskData.m_iCurIndex = 3;
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }
            index_DiaoLuoNum = 0;
            for (int i = 0; i < size_a; i++)
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_DROP_ITEM),
                                            ResourcesLoadCallBack);
            }
        }
    }

    private void ShowEffert(GameObject obj,int index)
    {
        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, obj, EffectIdTemplate.GetPathByeffectId(index), null);
    }
 
    private void Touched(GameObject obj)
    {

        if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_iCurMissionIndex == 100110 && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100110;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 3;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100110) && TaskData.Instance.m_iCurMissionIndex == 100270 && TaskData.Instance.m_TaskInfoDic[100270].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100270;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 4;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else
        {
            UIYindao.m_UIYindao.CloseUI();
        }

        if (UICamera.GetTouches().Count == 1 && (Application.platform == RuntimePlatform.IPhonePlayer 
            || Application.platform == RuntimePlatform.Android) 
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (obj.name.Equals("ButtondAdvance"))
            {
                obj.GetComponent<Collider>().enabled = false;
                if (!FunctionOpenTemp.GetWhetherContainID(1211))
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                              UIBoxLoadCallback);
                }
                else
                {
                    EquipsOfBody.Instance().EquipAdvace(EquipsOfBody.Instance().m_equipsOfBodyDic[BuWeiSave].dbId);
                }
            }
            else if (obj.name.Equals("ButtondWear"))
            {
                obj.GetComponent<Collider>().enabled = false;
                EquipsOfBody.Instance().m_WearEquip = true;

                if (UIYindao.m_UIYindao.m_isOpenYindao)
                {
                    CityGlobalData.m_isRightGuide = false;
                    if (TaskData.Instance.m_iCurMissionIndex == 100150 && TaskData.Instance.m_TaskInfoDic[100150].progress >= 0)
                    {
                        UIYindao.m_UIYindao.CloseUI();
                        //TaskData.Instance.m_iCurMissionIndex = 100115;
                        //ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        //UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                    else
                    {
                        CityGlobalData.m_isRightGuide = true;
                    }
                }
                EquipAddReq tempAddReq = new EquipAddReq(); //装备在背包中下标
                Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
                foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
                {
                    if (item.Value.itemId == EquipSaveId)
                    {
                        tempAddReq.gridIndex = item.Value.bagIndex;
                        EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
                        break;
                    }
                }

                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer t_qx = new QiXiongSerializer();
                t_qx.Serialize(tempStream, tempAddReq);

                byte[] t_protof;
                t_protof = tempStream.ToArray();
                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipAdd, ref t_protof);

            }
            else if (obj.name.Equals("ButtondChange"))//装备替换
            {
                obj.GetComponent<Collider>().enabled = false;
                EquipsOfBody.Instance().m_WearEquip = true;
                if (UIYindao.m_UIYindao.m_isOpenYindao)
                {
                    if (TaskData.Instance.m_iCurMissionIndex == 100090 && TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
                    {
                        TaskData.Instance.m_iCurMissionIndex = 100090;
                        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                    else if (TaskData.Instance.m_iCurMissionIndex == 100115 && TaskData.Instance.m_TaskInfoDic[100110].progress >= 0)
                    {
                        TaskData.Instance.m_iCurMissionIndex = 100115;
                        ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                        UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                    }
                    else
                    {
                        UIYindao.m_UIYindao.CloseUI();
                    }
                }



                EquipAddReq tempAddReq = new EquipAddReq(); //装备在背包中下标
                Dictionary<int, BagItem> tempBagEquipDic = BagData.Instance().m_playerEquipDic;
                foreach (KeyValuePair<int, BagItem> item in tempBagEquipDic)
                {
                    if (item.Value.itemId == EquipSaveId)
                    {
                        tempAddReq.gridIndex = item.Value.bagIndex;
                        EquipsOfBody.Instance().m_EquipBuWeiWearing = item.Value.buWei;
                        break;
                    }
                }

                MemoryStream tempStream = new MemoryStream();
                QiXiongSerializer t_qx = new QiXiongSerializer();
                t_qx.Serialize(tempStream, tempAddReq);

                byte[] t_protof;
                t_protof = tempStream.ToArray();
                //  if (BuWeiSave == 3 || BuWeiSave == 4 || BuWeiSave == 5)

                SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_EquipAdd, ref t_protof);


            }
            else if (obj.name.Equals("ButtondIntensify"))
            {
                NpcManager.m_NpcManager.setGoToNpc(1);
                MainCityUI.TryRemoveFromObjectList(m_Junzhu);
                Destroy(m_Junzhu);
            }
            else
            {
                m_RightObj.SetActive(true);
                gameObject.SetActive(false);
            }
        }

    }
    public void UIBoxLoadCallback(ref WWW p_www, string p_path, Object p_object)
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
        if (EquipsOfBody.Instance().m_equipsOfBodyDic.ContainsKey(buwei))
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
                if (tempBuwei == buwei && item.Value.pinZhi > EquipsOfBody.Instance().m_equipsOfBodyDic[tempBuwei].pinZhi)
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
    public void ResourcesLoadCallBack2(ref WWW p_www, string p_path, Object p_object)
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
    void ResLoadedTopRight(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_TopRight != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_TopRight.transform;
            tempObject.transform.localPosition = Vector3.zero;
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
             iconSampleManager.SetIconByID(int.Parse(exInfo.EquipUpgradeIcon),"",20);

            tempObject.transform.localScale = Vector3.one*0.85f;

            iconSampleManager.SetIconPopText(int.Parse(exInfo.EquipUpgradeIcon)
                , NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipUpgradeIcon)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipUpgradeIcon)).descId));
        }
        else
        {
            p_object = null;
        }

    }

    void ResLoadedMiddleLeft(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        if (m_MidLeft != null)
        {
            GameObject tempObject = (GameObject)Instantiate(p_object);
            tempObject.transform.parent = m_MidLeft.transform;
            tempObject.transform.localPosition = Vector3.zero;
            tempObject.GetComponent<UIWidget>().depth = 12;
            tempObject.GetComponent<UIWidget>().SetDimensions(82, 82);
            IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
            iconSampleManager.SetIconByID(int.Parse(exInfo.EquipMaterialId),"",20);

            tempObject.transform.localScale=Vector3.one*0.7f;

            iconSampleManager.SetIconPopText(int.Parse(exInfo.EquipMaterialId)
                , NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialId)).nameId)
                , DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(exInfo.EquipMaterialId)).descId));
        }
        else
        {
            p_object = null;
        }

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
    void GoTo(int id)
    {
        WindowBackShowController.m_SaveEquipBuWei = BuWeiSave;
        MainCityUI.TryRemoveFromObjectList(m_MainPaent);

        WindowBackShowController.SaveWindowInfo("JUN_ZHU_LAYER_AMEND", Res2DTemplate.Res.JUN_ZHU_LAYER_AMEND);
        EnterGuoGuanmap.Instance().ShouldOpen_id = id;

        Destroy(m_MainPaent);

    }
    int index_DiaoLuoNum = 0;
    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_ObjGrid2 != null)
        {
            GameObject DiaoluoShow = Instantiate(p_object) as GameObject;
            DiaoluoShow.transform.parent = m_ObjGrid2.transform;

            DiaoluoShow.transform.localPosition = Vector2.zero;

            DiaoluoShow.transform.localScale = Vector3.one;
            DiaoluoShow.transform.GetComponent<JunZhuDiaoLuoItem>().ShowInfo(exInfo._listGuanQia[index_DiaoLuoNum],GoTo);
            if (index_DiaoLuoNum < exInfo._listGuanQia.Count - 1)
            {
                index_DiaoLuoNum++;
            }
            m_ObjGrid2.repositionNow = true;
        }
        else
        {

        }
        //  DiaoluoShow.transform.GetComponent<JunZhuDiaoLuoItem>().ShowInfo(diaoluoInfo.isShow, diaoluoInfo.bigId.ToString(), diaoluoInfo.smaName.ToString(), diaoluoInfo.icon.ToString(), diaoluoInfo.stars);
    }
}
