using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthEquipInfoAdvanceItemManagerment : MonoBehaviour
{
    public UILabel m_LabelName;
    public UILabel m_LabelLevel;
    public UILabel m_LabelProgress;
    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;
    public GameObject m_ObjNext;
    public UILabel m_LabelName_Next;
    public UILabel m_LabelLevel_Next;
    public UISprite m_SpriteIcon_Next;
    public UISprite m_SpritePinZhi_Next;
    public UISprite m_SpriteArrow;

    public UIProgressBar m_PregressBar;
    public UIProgressBar m_EffectPregressBar;
    public List<int> m_listEffect;
    private float _ValueCount;
    public int m_LevelSave;
    private int _LevelChange = 0;
    private int _MaxExp = 0;
    private float _PerTimeAddExp = 0;
    private int _ExpId = 0;
    public UILabel m_LabEffect;
    public UILabel m_LabSignal;
    public List<EquipAttributeManagerment> m_listAttribute;
    public Dictionary<int, UILabel> m_DicInfo = new Dictionary<int, UILabel>();

    struct NextEquipInfo
    {
        public string _Nme;
        public string _Level;
        public string _Icon;
        public int _Quality;
    };
    private enum EffectType
    {
        EFFECT_NONE,
        EFFECT_RUNNING,
        EFFECT_STOP
    };
    private EffectType _TypeShow;
    private float _timeInterval = 0;
    private int _CircleCount = 0;
    private int _SaveNum = 0;
    private float _ValueTime = 0;
    private int _limitedNum = 3;
    struct AttributeInfo
    {
        public int _type;
        public int _count;
    };
    private List<AttributeInfo> _listsInfo = new List<AttributeInfo>();
    void Update()
    {
        if (m_listEffect.Count > 0 && _TypeShow == EffectType.EFFECT_NONE)
        {
            m_EffectPregressBar.value = 0;
            _ValueTime = 1.5f * Time.deltaTime;
            int tt = 0;

            for (int i = 0; i < m_listEffect.Count; i++)
            {
                tt += m_listEffect[i];
            }

            _LevelChange = m_LevelSave - tt;

            _MaxExp = ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange).needExp;

            m_LabEffect.text = "0/" + _MaxExp.ToString();
            if (m_listEffect[m_listEffect.Count - 1] < 8)
            {

                _ValueCount = 0.2f + (m_listEffect[m_listEffect.Count - 1] - 1) * 0.05f;
                _PerTimeAddExp = (_MaxExp * m_listEffect.Count) / (1 / _ValueCount);
            }
            else
            {
                _ValueCount = 0.44f;
                _PerTimeAddExp = (_MaxExp * m_listEffect.Count) / (1 / _ValueCount);
            }

            _SaveNum = m_listEffect[m_listEffect.Count - 1];
            _TypeShow = EffectType.EFFECT_RUNNING;
        }

        if (_TypeShow == EffectType.EFFECT_RUNNING && _timeInterval < _ValueTime)
        {

            _timeInterval += Time.deltaTime;
        }

        if (m_listEffect.Count > 0 && _timeInterval >= _ValueTime)
        {
            ShowEffectInfo();
        }
        else if (_TypeShow == EffectType.EFFECT_STOP)
        {
            ShowEffectInfo();
        }
    }
    private float _runningValueLab = 0;
    void ShowEffectInfo()
    {
        switch (_TypeShow)
        {
            case EffectType.EFFECT_RUNNING:
                {
                    m_PregressBar.gameObject.SetActive(false);
                    m_EffectPregressBar.gameObject.SetActive(true);
                    m_EffectPregressBar.value += _ValueCount;
                    _timeInterval = 0;
                    _runningValueLab += _PerTimeAddExp;
                    m_LabEffect.text = _runningValueLab + "/" + _MaxExp.ToString();

                    if (m_EffectPregressBar.value >= 1)
                    {
                        _CircleCount++;
                        if (_CircleCount >= _SaveNum)
                        {
                            _TypeShow = EffectType.EFFECT_STOP;
                        }
                        else
                        {
                            m_EffectPregressBar.value = 0;
                        }
                        _runningValueLab = 0;
                        _LevelChange++;
                        _MaxExp = ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange).needExp;
                    }
                }
                break;
            case EffectType.EFFECT_STOP:
                {
                    _ValueCount = 0;
                    _CircleCount = 0;
                    _SaveNum = 0;
                    _timeInterval = 0;
                    m_listEffect.RemoveAt(m_listEffect.Count - 1);
                    m_PregressBar.gameObject.SetActive(true);
                    m_EffectPregressBar.gameObject.SetActive(false);
                    _TypeShow = EffectType.EFFECT_NONE;
                }
                break;
            case EffectType.EFFECT_NONE:
                {

                }
                break;
        }
    }
    public void ShowInfo(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
 
        _ExpId = baseInfo._EquipId;
        m_SpriteIcon.spriteName = baseInfo._Icon.ToString();
        m_LabelName.text = NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(_ExpId).m_name)); 
        m_LabelLevel.text = "Lv." + baseInfo._Level;
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(CommonItemTemplate.getCommonItemTemplateById(baseInfo._EquipId).color))
        {
            m_SpritePinZhi.transform.localPosition = new Vector3(-393, 173, 0);
            m_SpritePinZhi.width = m_SpritePinZhi.height = 115;
        }
        else
        {
            m_SpritePinZhi.transform.localPosition = new Vector3(-394, 173, 0);
            m_SpritePinZhi.width = m_SpritePinZhi.height = 105;
        }
        m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(baseInfo._EquipId).color);
        if (baseInfo._MaxEXp > 0)
        {
          
            m_PregressBar.value = baseInfo._advanceExp / float.Parse(baseInfo._MaxEXp.ToString());
            m_LabelProgress.text = baseInfo._advanceExp + "/" +  baseInfo._MaxEXp.ToString();
        }
        else
        {
            m_PregressBar.value = 1.0f;
            m_LabelProgress.text = baseInfo._advanceExp.ToString() + "/0";
        }
        ShowAttribute(baseInfo);
    }

    private string GetName(int index)
    {
        switch (index)
        {
            case 0:
                {
                    return "攻击";
                }
                break;
            case 1:
                {
                    return "防御";
                }
                break;
            case 2:
                {
                    return "生命";
                }
                break;

        }
        return null;
    }

    private int _NextLevel = 0;
    void ShowAttribute(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
        _NextLevel = baseInfo._Level;
        _listsInfo.Clear();
        m_DicInfo.Clear();
        if (baseInfo._Gong > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 0;
            info._count = baseInfo._Gong;
            _listsInfo.Add(info);
        }

        if (baseInfo._Fang > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 1;
            info._count = baseInfo._Fang;
            _listsInfo.Add(info);
        }
        if (baseInfo._Xue > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 2;
            info._count = baseInfo._Xue;
            _listsInfo.Add(info);
        }

        int buwei = FunctionWindowsCreateManagerment.GetNeedLocation(ZhuangBei.getZhuangBeiById(baseInfo._EquipId).buWei);
        for (int i = 0; i < 3; i++)
        {
            if (i < _listsInfo.Count && _listsInfo[i]._count > 0)
            {
                m_listAttribute[i].m_LabName.text = GetName(_listsInfo[i]._type);
                if (ZhuangBei.getZhuangBeiById(baseInfo._EquipId).jiejieId > 0)
                {
                    switch (_listsInfo[i]._type)
                    {
                        case 0:
                            {
                                m_listAttribute[i].m_LabCount.text = _listsInfo[i]._count.ToString() + MyColorData.getColorString(4, " → "
                                    + EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei]
                                , EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].itemId)._gongJiAfter.ToString());
                            }
                            break;
                        case 1:
                            {
                                m_listAttribute[i].m_LabCount.text = _listsInfo[i]._count.ToString() + MyColorData.getColorString(4, " → "
                                    + EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei]
                                , EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].itemId)._fangYuAfter.ToString());
                            }
                            break;
                        case 2:
                            {
                                m_listAttribute[i].m_LabCount.text = _listsInfo[i]._count.ToString() + MyColorData.getColorString(4, " → "
                                    + EquipNextUpGradeCommonattribute.CommomAttribute(EquipsOfBody.Instance().m_equipsOfBodyDic[buwei]
                                , EquipsOfBody.Instance().m_equipsOfBodyDic[buwei].itemId)._shengMingAfter.ToString());
                            }
                            break;
                    }
                }
                else
                {
                    m_listAttribute[i].m_LabCount.text = _listsInfo[i]._count.ToString() ;
                }
             
                m_DicInfo.Add(_listsInfo[i]._type, m_listAttribute[i].m_LabCount);
            }
            else
            {
                m_listAttribute[i].m_LabName.text = "";
                m_listAttribute[i].m_LabCount.text = "";
            }
        }

        if (ZhuangBei.getZhuangBeiById(baseInfo._EquipId).jiejieId > 0)
        {
            int id = ZhuangBei.getZhuangBeiById(baseInfo._EquipId).jiejieId;
            NextEquipInfo info = new NextEquipInfo();
            info._Nme = NameIdTemplate.GetName_By_NameId(int.Parse(ZhuangBei.getZhuangBeiById(id).m_name));
            info._Icon = CommonItemTemplate.getCommonItemTemplateById(id).icon.ToString();
            info._Quality = CommonItemTemplate.getCommonItemTemplateById(id).color;
            info._Level = baseInfo._Level.ToString();
            ShowNextEquip(info);
            m_ObjNext.SetActive(true);
            m_LabSignal.gameObject.SetActive(false);
        }
        else
        {
            m_ObjNext.SetActive(false);
            m_LabSignal.text = MyColorData.getColorString(5, "当前装备已进阶到最高品质");
            m_LabSignal.gameObject.SetActive(true);
        }
    }

    void ShowNextEquip(NextEquipInfo info)
    {
        m_LabelName_Next.text = info._Nme;
        m_LabelLevel_Next.text = "Lv." + info._Level;
        m_SpriteIcon_Next.spriteName = info._Icon;
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(info._Quality))
        {
            m_SpritePinZhi.transform.localPosition = new Vector3(-393, 173, 0);
            m_SpritePinZhi_Next.width = m_SpritePinZhi_Next.height = 115;
        }
        else
        {
            m_SpritePinZhi.transform.localPosition = new Vector3(-394, 173, 0);
            m_SpritePinZhi_Next.width = m_SpritePinZhi_Next.height = 105;
        }

        m_SpritePinZhi_Next.spriteName = QualityIconSelected.SelectQuality(info._Quality);
    }
}
