using UnityEngine;
using System.Collections;

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

    public GameObject m_ObjWuQi;
    public GameObject m_ObjFangJu;
    public UIProgressBar m_PregressBar;

    public GameObject m_BottomTitle;

    public void ShowInfo(EquipGrowthEquipInfoManagerment.EquipBaseInfo baseInfo)
    {
        m_LabelName.text = baseInfo._Name;
        m_LabelGong.text = baseInfo._Gong.ToString();
        m_LabelFang.text = baseInfo._Gong.ToString();
        m_LabelXue.text = baseInfo._Gong.ToString();
        m_LabelProgress.text = baseInfo._Progress;
     //   m_LabelLevel.text = baseInfo._Level.ToString();
        m_SpriteIcon.spriteName = baseInfo._Icon.ToString();
        m_SpritePinZhi.spriteName = baseInfo._PinZhi;
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
