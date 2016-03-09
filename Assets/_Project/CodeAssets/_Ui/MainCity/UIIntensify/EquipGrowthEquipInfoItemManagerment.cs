using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthEquipInfoItemManagerment : MonoBehaviour
{
    public UILabel m_LabelName;
    public UILabel m_LabelGong;
    public UILabel m_LabelFang;
    public UILabel m_LabelXue;
    public UILabel m_LabelLevel;
    public UILabel m_LabelProgress;
    public UISprite m_SpriteIcon;
    public UISprite m_SpritePinZhi;

    public UILabel m_LabelSuccess;
    public GameObject m_ObjEffectTop;
    public GameObject m_ObjEffectBottom;
    public GameObject m_ObjWuQi;
    public GameObject m_ObjFangJu;
    public UIProgressBar m_PregressBar;
    public UIProgressBar m_EffectPregressBar;
    public GameObject m_BottomTitle;
    public List<int> m_listEffect;
    private float _ValueCount;
    public int m_LevelSave;
    private int _LevelChange = 0;
    private int _MaxExp = 0;
    private float _PerTimeAddExp = 0;
    private int _ExpId = 0;
    public UILabel m_LabEffect;
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
                _PerTimeAddExp = _MaxExp / (1 / _ValueCount);
            }
            else
            {
                _ValueCount = 0.44f;
                _PerTimeAddExp = _MaxExp / (1 / _ValueCount);
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
        else  if ( _TypeShow == EffectType.EFFECT_STOP)
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
        _ExpId = ZhuangBei.getZhuangBeiById(baseInfo._EquipId).expId;
        m_LabelProgress.text = baseInfo._Progress;
        m_SpriteIcon.spriteName = baseInfo._Icon.ToString();

        if (FunctionWindowsCreateManagerment.SpecialSizeFit(baseInfo._PinZhi))
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 88;
        }
        else
        {
            m_SpritePinZhi.width = m_SpritePinZhi.height = 78;
        }
         
            m_SpritePinZhi.spriteName = QualityIconSelected.SelectQuality(baseInfo._PinZhi);
        m_PregressBar.value = baseInfo._PregressValue;
        if (baseInfo._Gong > 0)
        {
            m_ObjWuQi.SetActive(true);
            m_ObjFangJu.SetActive(false);
        }
        else
        {
            m_ObjWuQi.SetActive(false);
            m_ObjFangJu.SetActive(true);
        }

        if (baseInfo._AttrCount > 0)
        {
            m_BottomTitle.SetActive(true);
        }
        else
        {
            m_BottomTitle.SetActive(false);
        }
    }
}
