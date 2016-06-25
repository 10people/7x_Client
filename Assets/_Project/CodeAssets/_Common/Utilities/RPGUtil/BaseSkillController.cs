using System;
using UnityEngine;
using System.Collections;

public class BaseSkillController : MonoBehaviour
{
    public bool IsShowCdEndEffect = false;

    public bool IsInCD = false;

    public static float SharedCD = 0.5f;
    public float SelfCD = -1;
    public float StartCDTime = -1;
    public float ActivedCD = -1;

    public UIButton m_SkillButton;
    public UISprite m_SkillSprite;
    public UISprite m_ShieldSprite;
    public UILabel m_CDLabel;

    public DelegateHelper.VoidDelegate BaseSkillClickDelegate;

    public bool IsMultiClickCheck = true;
    public float MultiClickDuration = 0.2f;
    private float lastClickTime;

    public void OnClick()
    {
        if (IsMultiClickCheck)
        {
            if (Time.realtimeSinceStartup - lastClickTime < MultiClickDuration)
            {
                return;
            }

            lastClickTime = Time.realtimeSinceStartup;
        }

        if (BaseSkillClickDelegate != null)
        {
            BaseSkillClickDelegate();
        }
    }

    public void TryStartSharedCD(bool isShowCDLabel = false)
    {
        if (SharedCD <= 0)
        {
            return;
        }

        if (SharedCD > GetRemainingCDTime)
        {
            ActivedCD = SharedCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            if (isShowCDLabel)
            {
                m_CDLabel.text = ActivedCD + "s";
                m_CDLabel.gameObject.SetActive(true);
            }
        }
    }

    public void TryStartSelfCD(bool isShowCDLabel = true)
    {
        if (SelfCD <= 0)
        {
            return;
        }

        if (SelfCD > GetRemainingCDTime)
        {
            ActivedCD = SelfCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            if (isShowCDLabel)
            {
                m_CDLabel.text = ActivedCD + "s";
                m_CDLabel.gameObject.SetActive(true);
            }
        }
    }

    public void TryStartAmountOfSelfCD(float amountTime, bool isShowCDLabel = true)
    {
        if (amountTime <= 0)
        {
            return;
        }

        if (amountTime > SelfCD)
        {
            Debug.LogError("Cannot show amount of self cd cause amount bigger than self cd.");
        }

        if (amountTime > GetRemainingCDTime)
        {
            ActivedCD = SelfCD;
            StartCDTime = Time.realtimeSinceStartup - (SelfCD - amountTime);

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            if (isShowCDLabel)
            {
                m_CDLabel.text = amountTime + "s";
                m_CDLabel.gameObject.SetActive(true);
            }
        }
    }


    public void StopCD()
    {
        //if cd ends.
        if (IsInCD && IsShowCdEndEffect && ActivedCD > 2)
        {
            if (UI3DEffectTool.HaveAnyFx(gameObject))
            {
                UI3DEffectTool.ClearUIFx(gameObject);
            }
            UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.FunctionUI_1, gameObject, EffectTemplate.getEffectTemplateByEffectId(110000).path);
        }

        IsInCD = false;
        m_ShieldSprite.gameObject.SetActive(false);
        m_CDLabel.gameObject.SetActive(false);
    }

    private float GetRemainingCDTime
    {
        get { return ActivedCD - (Time.realtimeSinceStartup - StartCDTime); }
    }

    public void Update()
    {
        var elapseTime = Time.realtimeSinceStartup - StartCDTime;
        if (elapseTime < ActivedCD)
        {
            m_ShieldSprite.fillAmount = (1 - elapseTime / ActivedCD);
            m_CDLabel.text = (int)Math.Ceiling(ActivedCD - elapseTime) + "s";
        }
        else
        {
            StopCD();
        }
    }

    public void Awake()
    {
        m_SkillButton = (m_SkillButton ?? GetComponent<UIButton>()) ?? gameObject.AddComponent<UIButton>();
        m_SkillSprite = (m_SkillSprite ?? GetComponent<UISprite>()) ?? gameObject.AddComponent<UISprite>();
    }
}
