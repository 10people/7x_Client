using UnityEngine;
using System.Collections;
using AllianceBattle;

public class RTSkillController : MonoBehaviour
{
    public int m_Index;
    public RTSkillTemplate m_Template;

    public UIButton m_SkillButton;
    public UISprite m_SkillSprite;

    public AllianceBattleUI m_AllianceBattleUi;

    public void OnClick()
    {
        m_AllianceBattleUi.OnSkillClick(m_Index);
    }

    void Awake()
    {
        m_SkillButton = m_SkillButton ?? GetComponent<UIButton>();
        m_SkillSprite = m_SkillSprite ?? GetComponent<UISprite>();
    }
}
