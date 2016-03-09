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
    public EventHandler m_HandCancelEffect;
    public GameObject m_HiddenObj;
    public GameObject m_EffectObj;
    public UISprite m_SpriteIcon;
    private UICamera m_Camera = null;
    void Awake()
    {
        if (MainCityUI.m_MainCityUI)
        {
            int size = MainCityUI.m_MainCityUI.m_WindowObjectList.Count;
            if (size > 0)
            {
                m_Camera = MainCityUI.m_MainCityUI.m_WindowObjectList[size - 1].GetComponentInChildren<UICamera>();
                if (m_Camera != null)
                    EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, true);
            }
            else
            {
                UI2DTool.Instance.AddTopUI(gameObject);
            }
        }
    }

    void Start()
    {
      //  UIYindao.m_UIYindao.CloseUI();
        m_JunZhuLevelUp = this;
        //m_Handler.m_handler += SelefDestroy;
        m_HandCancelEffect.m_click_handler += SelefDestroy;
        ShowInfo();
    }
   
    void SelefDestroy(GameObject obj)
    {
        ClientMain.closePopUp();
        Destroy(this.gameObject);
    }

    void ShowInfo()
    {
        m_SpriteIcon.spriteName = "Player_" + CityGlobalData.m_king_model_Id;
        m_HiddenObj.SetActive(true);
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_EffectObj, EffectIdTemplate.GetPathByeffectId(100020), null);
        m_LabTitle.text = NameIdTemplate.GetName_By_NameId(990020) +MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.level.ToString()) + NameIdTemplate.GetName_By_NameId(990019);
        m_LabGong.text = MyColorData.getColorString(9,(JunZhuData.Instance().m_junzhuSavedInfo.gongji - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).gongAdd)).ToString() + " (") + MyColorData.getColorString(4, (JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).gongAdd) + "↑") + MyColorData.getColorString(9, ")");
        m_LabFang.text = MyColorData.getColorString(9, (JunZhuData.Instance().m_junzhuSavedInfo.fangyu - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).fangAdd)).ToString() + " (") + MyColorData.getColorString(4, ( JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).fangAdd) + "↑") + MyColorData.getColorString(9, ")");
        m_LabMing.text = MyColorData.getColorString(9, (JunZhuData.Instance().m_junzhuSavedInfo.shengming - int.Parse(JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).xueAdd)).ToString() + " (") + MyColorData.getColorString(4, ( JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).xueAdd) + "↑") + MyColorData.getColorString(9, ")");
        JunZhuData.Instance().m_junzhuSavedInfo.gongji = JunZhuData.Instance().m_junzhuInfo.gongJi;
        JunZhuData.Instance().m_junzhuSavedInfo.fangyu = JunZhuData.Instance().m_junzhuInfo.fangYu;
        JunZhuData.Instance().m_junzhuSavedInfo.shengming = JunZhuData.Instance().m_junzhuInfo.shengMing;
  
        m_LabTiLi.text = MyColorData.getColorString(9, JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level -1).tiliCao.ToString() + "(")+ MyColorData.getColorString(4,  (JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level).tiliCao - JunzhuShengjiTemplate.GetJunZhuLevelUpInfo(JunZhuData.Instance().m_junzhuInfo.level - 1).tiliCao).ToString() + "↑") + MyColorData.getColorString(9, ")");
        m_LabHuoDeTiLi.text = MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_UP_SIGNAL)) + MyColorData.getColorString(4, JunzhuShengjiTemplate.GetAddTili(JunZhuData.Instance().m_junzhuInfo.level)) + MyColorData.getColorString(1, LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_UP_SIGNAL_1));
    }

    void OnDestroy()
    {
        if (m_Camera != null)
            EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, false);

        UI3DEffectTool.ClearUIFx(m_EffectObj);
        CityGlobalData.m_isWhetherOpenLevelUp = true;
    }
}
