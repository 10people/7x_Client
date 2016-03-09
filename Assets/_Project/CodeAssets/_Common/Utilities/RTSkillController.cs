using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RTSkillController : BaseSkillController
{
    public int m_Index;
    public RTSkillTemplate m_Template;

    public AllianceBattle.AllianceBattleUI m_AllianceBattleUI;
    public Carriage.CarriageMain m_CarriageMain;

    public new void OnClick()
    {
        base.OnClick();

        if (m_AllianceBattleUI != null)
        {
            m_AllianceBattleUI.OnSkillClick(m_Index);
        }

        if (m_CarriageMain != null)
        {
            m_CarriageMain.OnSkillClick(m_Index);
        }
    }

    public void LoadTemplate()
    {
        var temp = RTSkillTemplate.templates.Where(item => item.SkillId == m_Index).ToList();
        if (temp.Any())
        {
            m_Template = temp.First();
        }

        SelfCD = m_Template.BaseCD;
    }

    new void Awake()
    {
        base.Awake();

        LoadTemplate();
    }
}
