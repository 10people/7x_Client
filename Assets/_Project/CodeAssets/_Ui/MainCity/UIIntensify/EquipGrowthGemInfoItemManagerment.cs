using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthGemInfoItemManagerment : MonoBehaviour
{
    public UILabel m_LabelName;
    public UILabel m_LabelLevel;
    public UILabel m_LabelProgress;
    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;
    public UILabel m_LabelSuccess;
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
    public EquipAttributeManagerment m_AttributeAdd;
    public GameObject m_objArrow;
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
    private
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
    public void ShowInfo(EquipGrowthInlayLayerManagerment.GemLayerInfo info)
    {
        m_LabelName.text = NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(info._id).nameId);
        if (FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel
           < FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).levelMax)
        {
            m_objArrow.SetActive(true);
            m_LabelLevel.text = FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel + "级                 "   
                + MyColorData.getColorString(4,(FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel+1).ToString() + "级");
        }
        else
        {
            m_LabelLevel.text = MyColorData.getColorString(5, "当前宝石已合成到最高品质");
            m_objArrow.SetActive(false);
        }
        m_SpriteIcon.spriteName = CommonItemTemplate.getCommonItemTemplateById(info._id).icon.ToString();
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(CommonItemTemplate.getCommonItemTemplateById(info._id).color))
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 115;
        }
        else
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 105;
        }

        m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(CommonItemTemplate.getCommonItemTemplateById(info._id).color);
        if (FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel
            < FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).levelMax)
        {
            m_PregressBar.value = float.Parse(info._exp.ToString()) / FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).lvlupExp;
            m_PregressBar.ForceUpdate();
            m_LabelProgress.text = info._exp.ToString() + "/" + FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).lvlupExp;
        }
        else
        {
            m_PregressBar.value = 1.0f;
            m_PregressBar.ForceUpdate();
            m_LabelProgress.text = info._exp.ToString()+ "/0";
        }
        ShowAttribute(info);
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

    void ShowAttribute(EquipGrowthInlayLayerManagerment.GemLayerInfo info)
    {
        if ( FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel
            < FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).levelMax)
        {
            m_AttributeAdd.m_LabName.text = GetName(FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).shuxing - 1);

            m_AttributeAdd.m_LabCount.text = FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).shuxingValue + " → " 
               + MyColorData.getColorString(4,FuWenTemplate.GetGemByAttrubiteAndLevel(FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).shuxing
               , FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).fuwenLevel+ 1).shuxingValue);
        }
        else
        {
            m_AttributeAdd.m_LabName.text = GetName(FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).shuxing - 1);
            m_AttributeAdd.m_LabCount.text = FuWenTemplate.GetFuWenTemplateByFuWenId(info._id).shuxingValue.ToString();
        }
    }
}
