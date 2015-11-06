using UnityEngine;
using System.Collections;

public class JunZhuLevelUpManagerment : MonoBehaviour
{
    public static JunZhuLevelUpManagerment m_JunZhuLevelUp;
    public UILabel m_LabTitle;
    public UILabel m_LabSignal;
    public UILabel m_LabGong;
    public UILabel m_LabFang;
    public UILabel m_LabMing;
    public UILabel m_LabTiLi;
    public UILabel m_LabHuoDeTiLi;
    public EventHandler m_Handler;
    public EventHandler m_HandCancelEffect;
    public GameObject m_HiddenObj;
    public GameObject m_EffectObj;

    public GameObject m_UpgradeObj;

    void Awake()
    {

    }

    void Start()
    {
        m_JunZhuLevelUp = this;
        m_Handler.m_handler += SelefDestroy;
        m_HandCancelEffect.m_handler += CancelEffect;
        StartCoroutine(WaitFor());
        StartCoroutine(WaitForUpgaade());
        UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EffectObj, EffectIdTemplate.GetPathByeffectId(100020), null);

    }
    private bool _isCancelEffect = false;
    void CancelEffect(GameObject obj)
    {
        _isCancelEffect = true;
        m_HandCancelEffect.gameObject.SetActive(false);
        UI3DEffectTool.Instance().ClearUIFx(m_EffectObj);
        m_UpgradeObj.SetActive(false);
        ShowInfo();
    }
    bool _isPlayingOn = false;
    //void Update()
    //{
    //    if (!m_UpgradeObj.GetComponent<Animator>().animation.isPlaying && !_isPlayingOn)
    //    {
    //        _isPlayingOn = true;
    //        m_UpgradeObj.SetActive(false);
    //    }
    //}

    IEnumerator WaitForUpgaade()
    {
        yield return new WaitForSeconds(0.4f);
        if (!_isCancelEffect)
        {
            m_UpgradeObj.SetActive(true);
        }

    }

    IEnumerator WaitFor()
    {
        yield return new WaitForSeconds(2.4f);
        if (!_isCancelEffect)
        {
            m_UpgradeObj.SetActive(false);
            m_HandCancelEffect.gameObject.SetActive(false);
            ShowInfo();
        }
    }
    void SelefDestroy(GameObject obj)
    {
        ClientMain.closePopUp();
        Destroy(this.gameObject);
    }

    void ShowInfo()
    {
        m_HiddenObj.SetActive(true);
        m_LabTitle.text = NameIdTemplate.GetName_By_NameId(990020) + JunZhuData.Instance().m_junzhuInfo.level.ToString() + NameIdTemplate.GetName_By_NameId(990019);
      //  m_LabSignal.text = FunctionOpenTemp.GetDesByLevel(JunZhuData.Instance().m_junzhuInfo.level);
        m_LabGong.text = (JunZhuData.Instance().m_junzhuSavedInfo.gongji - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).gongAdd)).ToString() + MyColorData.getColorString(4, (" +" + JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).gongAdd));
        m_LabFang.text = (JunZhuData.Instance().m_junzhuSavedInfo.fangyu - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).fangAdd)).ToString() + MyColorData.getColorString(4, (" +" + JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).fangAdd));
        m_LabMing.text = (JunZhuData.Instance().m_junzhuSavedInfo.shengming - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).xueAdd)).ToString() + MyColorData.getColorString(4, (" +" + JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).xueAdd));
        JunZhuData.Instance().m_junzhuSavedInfo.gongji = JunZhuData.Instance().m_junzhuInfo.gongJi;
        JunZhuData.Instance().m_junzhuSavedInfo.fangyu = JunZhuData.Instance().m_junzhuInfo.fangYu;
        JunZhuData.Instance().m_junzhuSavedInfo.shengming = JunZhuData.Instance().m_junzhuInfo.shengMing;
  
        m_LabTiLi.text = JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level -1).tiliCao.ToString()+ MyColorData.getColorString(4, "+" + (JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).tiliCao - JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level - 1).tiliCao).ToString());
        m_LabHuoDeTiLi.text = MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_UP_SIGNAL)) + MyColorData.getColorString(4, JunzhuShengjiTemplate.GetAddTili(JunZhuData.Instance().m_junzhuInfo.level)) + MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_UP_SIGNAL_1));
    }

    void OnDestroy()
    {
        CityGlobalData.m_isWhetherOpenLevelUp = true;
    }
}
