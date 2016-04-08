using UnityEngine;
using System.Collections;

public class EquipTaoZhuangLayerManagerment : MonoBehaviour {

    public UILabel m_LabTitle;
    public UILabel m_LabGong;
    public UILabel m_LabFang;
    public UILabel m_LabMing;
    public GameObject m_HiddenObj;
    public UISprite m_SpriteQuality;
    private UICamera m_Camera = null;
    public EventHandler m_HandCancelEffect;
    void Start ()
    {
        m_HandCancelEffect.m_click_handler += SelefDestroy;
        if (MainCityUI.m_MainCityUI)
        {
            int size = MainCityUI.m_MainCityUI.m_WindowObjectList.Count;
            if (size > 0)
            {
                m_Camera = MainCityUI.m_MainCityUI.m_WindowObjectList[size - 1].GetComponentInChildren<UICamera>();
                if (m_Camera != null)
                    EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, true);
            }
        }
    //  ShowInfo(FunctionWindowsCreateManagerment.m_JiHuoInfo);
    }

    void SelefDestroy(GameObject obj)
    {
        Destroy(this.gameObject);
    }

    public void ShowInfo(FunctionWindowsCreateManagerment.EquipTaoJiHuo info)
    {
        if (FunctionWindowsCreateManagerment.SpecialSizeFit(info._quality))
        {
            m_SpriteQuality.width = 120;
            m_SpriteQuality.height = 120;
        }
        else
        {
            m_SpriteQuality.width = 106;
            m_SpriteQuality.height = 106;
        }
        m_SpriteQuality.spriteName = QualityIconSelected.SelectQuality(info._quality);
        m_HiddenObj.SetActive(true);
        UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_LabTitle.gameObject, EffectIdTemplate.GetPathByeffectId(620214), null);
     
        m_LabGong.text = MyColorData.getColorString(9, (info._gong + info._gongadd).ToString() + " (") + MyColorData.getColorString(4,  info._gongadd.ToString() + "↑") + MyColorData.getColorString(9, ")");
        m_LabFang.text = MyColorData.getColorString(9, (info._fang + info._fanggadd).ToString() + " (") + MyColorData.getColorString(4, info._fanggadd + "↑") + MyColorData.getColorString(9, ")");
        m_LabMing.text = MyColorData.getColorString(9, (info._ming + info._minggadd).ToString() + " (") + MyColorData.getColorString(4, info._minggadd + "↑") + MyColorData.getColorString(9, ")");
    }

    void OnDestroy()
    {
        JunZHuEquipOfBody.m_IsJiHuo = false;
        if (m_Camera != null)
            EffectTool.SetUIBackgroundEffect(m_Camera.gameObject, false);

        Global.m_isZhanli = false;
        FunctionWindowsCreateManagerment.m_IsEquipJihuoShow = false;
    }
}
