using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RTSkillController : BaseSkillController
{
    public int m_Index;
    public RTSkillTemplate m_Template;

    /// <summary>
    /// Must implement OnSkillClick method in m_LogicMain mono.
    /// </summary>
    public GameObject m_LogicMain;

    public new void OnClick()
    {
        base.OnClick();

        m_LogicMain.SendMessage("OnSkillClick", m_Index);
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

        //Set logic main.
        if (m_LogicMain == null)
        {
            if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
            {
                m_LogicMain = Carriage.RootManager.Instance.m_CarriageMain.gameObject;
            }
            else if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_BATTLE)
            {
                m_LogicMain = AllianceBattle.RootManager.Instance.m_AllianceBattleMain.gameObject;
            }
            else
            {
                Debug.LogError("Logic main is null, r u use this in a new scene?");
            }
        }
    }
}
