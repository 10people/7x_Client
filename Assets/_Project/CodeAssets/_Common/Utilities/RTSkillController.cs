using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RTSkillController : MonoBehaviour
{
    public int m_Index;
    public RTSkillTemplate m_Template;

    public bool IsInCD = false;

    public static float SharedCD = 0.5f;
    public float SelfCD
    {
        get { return m_Template.BaseCD / 1000f; }
    }
    public float StartCDTime = -1;
    public float ActivedCD = -1;

    public UIButton m_SkillButton;
    public UISprite m_SkillSprite;
    public UISprite m_ShieldSprite;
    public UILabel m_CDLabel;

    public AllianceBattle.AllianceBattleUI m_AllianceBattleUI;
    public Carriage.CarriageMain m_CarriageMain;

    public void TryStartSharedCD()
    {
        if (SharedCD > GetRemainingCDTime)
        {
            ActivedCD = SharedCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            m_CDLabel.text = ActivedCD + "s";
            m_CDLabel.gameObject.SetActive(true);
        }
    }

    public void TryStartSelfCD()
    {
        if (SelfCD > GetRemainingCDTime)
        {
            ActivedCD = SelfCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            m_CDLabel.text = ActivedCD + "s";
            m_CDLabel.gameObject.SetActive(true);
        }
    }

    public void StopCD()
    {
        IsInCD = false;
        m_ShieldSprite.gameObject.SetActive(false);
        m_CDLabel.gameObject.SetActive(false);
    }

    private float GetRemainingCDTime
    {
        get { return ActivedCD - (Time.realtimeSinceStartup - StartCDTime); }
    }

    public void OnClick()
    {
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
    }

    void Update()
    {
        var elapseTime = Time.realtimeSinceStartup - StartCDTime;
        if (elapseTime <= ActivedCD)
        {
            m_ShieldSprite.fillAmount = (1 - elapseTime / ActivedCD);
            m_CDLabel.text = (int)(ActivedCD - elapseTime) + "s";
        }
        else
        {
            StopCD();
        }
    }

    void Awake()
    {
        m_SkillButton = (m_SkillButton ?? GetComponent<UIButton>()) ?? gameObject.AddComponent<UIButton>();
        m_SkillSprite = (m_SkillSprite ?? GetComponent<UISprite>()) ?? gameObject.AddComponent<UISprite>();

        LoadTemplate();
    }
}
