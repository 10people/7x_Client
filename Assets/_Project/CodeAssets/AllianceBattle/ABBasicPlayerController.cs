using UnityEngine;
using System.Collections;
using AllianceBattle;

public class ABBasicPlayerController : MonoBehaviour
{
    public ABPlayerController m_PlayerController;
    public BasicPlayerController m_BasicPlayerController;

    public void OnAttackFinish()
    {
        EnableMove();
    }

    public void OnBeenAttackFinish()
    {
        EnableMove();
    }

    private void EnableMove()
    {
        if (m_PlayerController == null && m_BasicPlayerController == null)
        {
            m_PlayerController = GetComponent<ABPlayerController>();
            m_BasicPlayerController = GetComponent<BasicPlayerController>();
        }

        if (m_PlayerController != null)
        {
            m_PlayerController.ActiveMove();
        }

        if (m_BasicPlayerController != null)
        {
            m_BasicPlayerController.ActiveMove();
        }
    }

    public Camera TrackCamera;

    public UIPanel NguiPanel;

    public UIProgressBar ProgressBar;
    public UISprite ProgressBarForeSprite;
    private const string redBarName = "progressred";
    private const string greenBarName = "progressford";

    public UILabel NameLabel;
    public UILabel AllianceLabel;

    public UILabel PopupDamageLabel;

    public string KingName;
    public string AllianceName;
    public float TotalBlood;
    public float RemainingBlood;
    public bool IsRed;

    public GameObject PlayerSelectedSign;

    public void SetThis()
    {
        NameLabel.text = string.IsNullOrEmpty(KingName) ? "" : KingName;
        AllianceName = string.IsNullOrEmpty(AllianceName) ? "无联盟" : AllianceName;
        NameLabel.color = AllianceLabel.color = IsRed ? Color.red : Color.green;
        ProgressBarForeSprite.spriteName = IsRed ? redBarName : greenBarName;
        if (TotalBlood > 0 || RemainingBlood <= TotalBlood)
        {
            UpdateBloodBar(RemainingBlood);
        }
    }

    public void OnSelected()
    {
        PlayerSelectedSign.SetActive(true);
    }

    public void OnDeSelected()
    {
        PlayerSelectedSign.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage">damage</param>
    public void OnDamage(float damage, float remaining)
    {
        StopAllCoroutines();
        DeactiveDamage();
        StartCoroutine(ShowDamage(damage));

        UpdateBloodBar(remaining);
    }

    private void UpdateBloodBar(float remaining)
    {
        RemainingBlood = remaining;
        ProgressBar.value = RemainingBlood / TotalBlood;
    }

    private IEnumerator ShowDamage(float damage)
    {
        PopupDamageLabel.text = ColorTool.Color_Red_c40000 + damage + "[-]";
        PopupDamageLabel.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        DeactiveDamage();
    }

    private void DeactiveDamage()
    {
        PopupDamageLabel.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (TrackCamera == null) return;

        NguiPanel.transform.eulerAngles = new Vector3(NguiPanel.transform.eulerAngles.x, TrackCamera.transform.eulerAngles.y, NguiPanel.transform.eulerAngles.z);
    }
}
