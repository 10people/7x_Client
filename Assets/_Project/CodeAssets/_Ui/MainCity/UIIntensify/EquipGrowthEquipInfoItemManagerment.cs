using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthEquipInfoItemManagerment : MonoBehaviour
{
    public UILabel m_LabelName;
    public UILabel m_LabelLevel;
    public UILabel m_LabelProgress;
    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;
    public UISprite m_SpriteArrow;
    public UILabel m_LabelSuccess;
    public GameObject m_ObjAttributeManagerment;
    public UIProgressBar m_PregressBar;
    public UIProgressBar m_EffectPregressBar;
    public List<int> m_listEffect;

    public bool m_StopEffectNow = false;
    private float _ValueCount;
    public int m_LevelSave;
    private int _LevelChange = 0;
    private int _MaxExp = 0;
    private float _PerTimeAddExp = 0;
    private int _ExpId = 0;
    public UILabel m_LabEffect;
    public List<EquipAttributeManagerment> m_listAttribute;
    public Dictionary<int, UILabel> m_DicInfo = new Dictionary<int, UILabel>();
    public enum EffectType
    {
        EFFECT_NONE,
        EFFECT_RUNNING,
        EFFECT_STOP
    };
    public  EffectType m_TypeShow;
    private float _timeInterval = 0;
    private int _CircleCount = 0;
    private int _SaveNum = 0;
    private float _ValueTime = 0;
    private int _limitedNum = 3;
    struct AttributeInfo
    {
        public int _type;
        public string _count;
    };
    private List<AttributeInfo> _listsInfo = new List<AttributeInfo>();
    void Update()
    {
        if (m_listEffect.Count > 0 && m_TypeShow == EffectType.EFFECT_NONE)
        {
            m_EffectPregressBar.value = 0;
            m_EffectPregressBar.ForceUpdate();
            _ValueTime = 1.5f * Time.deltaTime;
            int tt = 0;

            for (int i = 0; i < m_listEffect.Count; i++)
            {
                tt += m_listEffect[i];
            }

            _LevelChange = m_LevelSave - tt;
            if (ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange) != null)
            {
                _MaxExp = ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange).needExp;
            }

            m_LabEffect.text = "0/" + _MaxExp.ToString();
            if (m_listEffect[m_listEffect.Count - 1] < 8)
            {

                _ValueCount = 0.2f + (m_listEffect[m_listEffect.Count - 1] - 1) * 0.02f;
                _PerTimeAddExp = (_MaxExp * m_listEffect.Count) / (1 / _ValueCount);
            }
            else
            {
                _ValueCount = 0.3f;
                _PerTimeAddExp = (_MaxExp * m_listEffect.Count) / (1 / _ValueCount);
            }

            _SaveNum = m_listEffect[m_listEffect.Count - 1];
            m_TypeShow = EffectType.EFFECT_RUNNING;
        }

        if (m_TypeShow == EffectType.EFFECT_RUNNING && _timeInterval < _ValueTime)
        {

            _timeInterval += Time.deltaTime;
        }

        if (m_listEffect.Count > 0 && _timeInterval >= _ValueTime)
        {
            ShowEffectInfo();
        }
        else if (m_TypeShow == EffectType.EFFECT_STOP)
        {
            ShowEffectInfo();
        }
    }
    private float _runningValueLab = 0;
    void ShowEffectInfo()
    {
        switch (m_TypeShow)
        {
            case EffectType.EFFECT_RUNNING:
                {
                    m_PregressBar.gameObject.SetActive(false);
                    m_EffectPregressBar.gameObject.SetActive(true);
                    m_EffectPregressBar.value += _ValueCount;
                    m_EffectPregressBar.ForceUpdate();
                    _timeInterval = 0;
                    _runningValueLab += _PerTimeAddExp;
                    m_LabEffect.text = _runningValueLab + "/" + _MaxExp.ToString();

                    if (m_EffectPregressBar.value >= 1)
                    {
                        m_EffectPregressBar.value = 1;
                        m_EffectPregressBar.ForceUpdate();
                        _CircleCount++;
                        if (_CircleCount >= _SaveNum)
                        {
                            m_TypeShow = EffectType.EFFECT_STOP;
                        }
                        else
                        {
                            m_EffectPregressBar.value = 0;
                            m_EffectPregressBar.ForceUpdate();
                        }
                        _runningValueLab = 0;
                        _LevelChange++;
                        if (ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange) != null)
                        {
                            _MaxExp = ExpXxmlTemp.getExpXxmlTemp_By_expId(_ExpId, _LevelChange).needExp;
                        }

                    }
                }
                break;
            case EffectType.EFFECT_STOP:
                {
                    _ValueCount = 0;
                    _CircleCount = 0;
                    _SaveNum = 0;
                    _timeInterval = 0;
                    if (m_listEffect.Count > 0)
                    {
                        m_listEffect.RemoveAt(m_listEffect.Count - 1);
                    }
                    if (m_StopEffectNow)
                    {
                        m_StopEffectNow = false;
                        m_listEffect.Clear();
                    }
                    m_PregressBar.gameObject.SetActive(true);
                    m_EffectPregressBar.gameObject.SetActive(false);
                    m_EffectPregressBar.ForceUpdate();
                    m_TypeShow = EffectType.EFFECT_NONE;
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
        _ExpId = ZhuangBei.getZhuangBeiById(baseInfo._EquipId).expId;
        m_LabelProgress.text = baseInfo._Progress;
        m_SpriteIcon.spriteName = baseInfo._Icon.ToString();

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
        m_PregressBar.value = baseInfo._PregressValue;
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

    void ShowAttribute(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
        _listsInfo.Clear();
        m_DicInfo.Clear();
        if (baseInfo._Gong > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 0;
            info._count = baseInfo._Gong.ToString();
            _listsInfo.Add(info);
        }

        if (baseInfo._Fang > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 1;
            info._count = baseInfo._Fang.ToString();
            _listsInfo.Add(info);
        }

        if (baseInfo._Xue > 0)
        {
            AttributeInfo info = new AttributeInfo();
            info._type = 2;
            info._count = baseInfo._Xue.ToString();
            _listsInfo.Add(info);
        }

        for (int i = 0; i < 3; i++)
        {
            if (i < _listsInfo.Count)
            {
                m_listAttribute[i].m_LabName.text = GetName(_listsInfo[i]._type);
                //m_listAttribute[i].m_LabCount.text =  _listsInfo[i]._count;
                m_DicInfo.Add(_listsInfo[i]._type, m_listAttribute[i].m_LabCount);
            }
            else
            {
                m_listAttribute[i].m_LabName.text = "";
                m_listAttribute[i].m_LabCount.text = "";
            }
        }
    }
}
