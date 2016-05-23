using UnityEngine;
using System.Collections;

public class TPController : MonoBehaviour
{
    public DelegateHelper.Vector2Delegate m_ExecuteAfterTP;

    public GameObject m_TpObject;
    public UIProgressBar m_TpBar;
    public UILabel m_TpTimeLabel;
    private float m_tpDuration;

    private Vector2 m_TpToPos;

    public void TPToPosition(Vector2 targetPos, float duration)
    {
        m_TpToPos = targetPos;
        m_tpDuration = duration;

        m_TpObject.SetActive(true);
        StartSetTpBar();
    }

    private void StartSetTpBar()
    {
        m_TpBar.value = 0;
        m_TpTimeLabel.text = "传送中" + 0.0 + "秒";
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", m_tpDuration, "easetype", "linear", "onupdate", "UpdateTpInfo", "oncomplete", "EndTpBar"));
    }

    private void UpdateTpInfo(float value)
    {
        m_TpBar.value = value;
        m_TpTimeLabel.text = "传送中" + float.Parse((m_tpDuration * value).ToString("0.0")) + "秒";
    }

    private void EndTpBar()
    {
        m_TpObject.SetActive(false);

        if (m_ExecuteAfterTP != null)
        {
            m_ExecuteAfterTP(m_TpToPos);
        }
    }
}
