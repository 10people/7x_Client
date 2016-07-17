using UnityEngine;
using System.Collections;

public class EffectNumController : Singleton<EffectNumController>
{
    private float m_limitDuration = 2.0f;
    private int m_limitNumber = 3;

    private float m_lastCheckTime = -1f;

    private int m_selfRelatedEffectCount;
    private int m_otherEffectCount;

    public bool IsCanPlayEffect(bool isSelfRelated)
    {
        return isSelfRelated ? m_selfRelatedEffectCount < m_limitNumber : m_otherEffectCount < m_limitNumber;
    }

    public void NotifyPlayingEffect(bool isSelfRelated, int plusNum = 1)
    {
        if (isSelfRelated)
        {
            m_selfRelatedEffectCount += plusNum;
        }
        else
        {
            m_otherEffectCount += plusNum;
        }
    }

    private void ResetPlayedEffectCount()
    {
        m_selfRelatedEffectCount = 0;
        m_otherEffectCount = 0;
    }

    void Update()
    {
        if (Time.realtimeSinceStartup - m_lastCheckTime > m_limitDuration)
        {
            ResetPlayedEffectCount();
            m_lastCheckTime = Time.realtimeSinceStartup;
        }
    }

    void Awake()
    {
        m_limitDuration = ConfigTool.GetFloat(ConfigTool.CONST_EFFECT_UPDATE_INTERVAL, 2.0f);
        m_limitNumber = ConfigTool.GetInt(ConfigTool.CONST_MAX_EFFECT_COUNT, 3);
    }
}
