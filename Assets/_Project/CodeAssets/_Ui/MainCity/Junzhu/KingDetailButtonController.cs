using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingDetailButtonController : MonoBehaviour
{
    public struct KingDetailButtonConfig
    {
        public string m_ButtonStr;

        public delegate void ButtonClick();
        public ButtonClick m_ButtonClick;
    }

    public KingDetailButtonConfig m_Config;

    public UILabel m_ButtonLabel;

    public void SetThis(KingDetailButtonConfig config)
    {
        if (string.IsNullOrEmpty(config.m_ButtonStr) || config.m_ButtonClick == null)
        {
            Debug.LogError("Config null in KingDetailButton setting.");
            return;
        }

        m_Config = config;

        m_ButtonLabel.text = config.m_ButtonStr;
    }

    void OnClick()
    {
        if (m_Config.m_ButtonClick == null) return;
        m_Config.m_ButtonClick();
    }

    void Awake()
    {
        m_ButtonLabel = m_ButtonLabel ?? GetComponentInChildren<UILabel>();
    }
}
