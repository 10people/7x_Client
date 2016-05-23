using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGBaseCultureController : MonoBehaviour
{
    public GameObject m_LogicMain;

    public SinglePlayerController m_SinglePlayerController;
    public OtherPlayerController m_OtherPlayerController;

    public WeaponSwitcher m_WeaponSwitcher;

    public Animator m_Animator;
    public Renderer m_MainRenderer;
    public GameObject m_ShadowObject;

    public UIProgressBar ProgressBar;
    public UISprite ProgressBarForeSprite;

    public bool IsSelf
    {
        get { return JunZhuData.Instance().m_junzhuInfo.name == KingName; }
    }

    public bool IsEnemy
    {
        get { return !(((!string.IsNullOrEmpty(AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (AllianceName == AllianceData.Instance.g_UnionInfo.name))) || (KingName == JunZhuData.Instance().m_junzhuInfo.name)); }
    }

    public string KingName;
    public string AllianceName;
    public int AlliancePost;
    public int Vip;
    public int Title;
    public int Level;
    public int NationID;
    public int BattleValue;
    public float TotalBlood;
    public float RemainingBlood;

    public virtual void SetThis()
    {
        if (TotalBlood > 0 || RemainingBlood <= TotalBlood)
        {
            UpdateBloodBar(RemainingBlood);
        }
    }

    public DelegateHelper.VoidDelegate m_ExecuteAfterSkillFinish;

    public void OnSkillFinish()
    {
        EnableMove();

        TryRestoreWeapon();

        if (m_ExecuteAfterSkillFinish != null)
        {
            m_ExecuteAfterSkillFinish();
            m_ExecuteAfterSkillFinish = null;
        }
    }

    public void OnBeenSkillFinish()
    {
        EnableMove();
    }

    public void OnDeadFinish()
    {
        m_LogicMain.SendMessage("ExecuteDead", UID);

        //if (TimeHelper.Instance.IsTimeCalcKeyExist("RPGBaseDeadAnim" + UID))
        //{
        //    TimeHelper.Instance.RemoveFromTimeCalc("RPGBaseDeadAnim" + UID);
        //}
        //TimeHelper.Instance.AddOneDelegateToTimeCalc("RPGBaseDeadAnim" + UID, 1f, ExecuteAfterDeadFinish);
    }

    private void ExecuteAfterDeadFinish(string key)
    {
        m_LogicMain.SendMessage("ExecuteDead", int.Parse(key.Replace("RPGBaseDeadAnim", "")));
    }

    protected void EnableMove()
    {
        TryGetController();

        if (m_SinglePlayerController != null)
        {
            m_SinglePlayerController.ActiveMove();
        }

        if (m_OtherPlayerController != null)
        {
            m_OtherPlayerController.ActiveMove();
        }
    }

    private void TryRestoreWeapon()
    {
        if (m_WeaponSwitcher != null)
        {
            m_WeaponSwitcher.RestoreWeapon();
        }
    }

    private bool isGetController = false;

    private void TryGetController()
    {
        if (!isGetController)
        {
            m_SinglePlayerController = GetComponent<SinglePlayerController>();
            m_OtherPlayerController = GetComponent<OtherPlayerController>();
        }

        if (m_SinglePlayerController != null || m_OtherPlayerController != null)
        {
            isGetController = true;
        }
    }

    public Camera TrackCamera;

    public GameObject m_UIParentObject;

    public void SetUIParentObject(bool isActive)
    {
        if (isActive && !m_UIParentObject.activeInHierarchy)
        {
            ProgressBar.ForceUpdate();
        }

        m_UIParentObject.SetActive(isActive);
    }

    [Obsolete]
    public void SetActivedPopupLabel(bool isSub)
    {
        m_activedPopupLabel = isSub ? (IsSelf ? PopupLabel_Important : PopupLabel_Basic) : PopupLabel_Recover;
    }

    private UILabel m_activedPopupLabel;

    /// <summary>
    /// white color
    /// </summary>
    public UILabel PopupLabel_Basic;
    /// <summary>
    /// red color
    /// </summary>
    public UILabel PopupLabel_Important;
    /// <summary>
    /// green color
    /// </summary>
    public UILabel PopupLabel_Recover;

    public int UID;
    public int RoleID;
    public long JunzhuID;

    public GameObject PlayerSelectedSign;

    public void OnClick()
    {
        TryGetController();

        if (m_OtherPlayerController != null)
        {
            m_LogicMain.SendMessage("ActiveTarget", UID);
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
    /// <param name="remaining"></param>
    /// <param name="isSPDamage"></param>
    public void OnDamage(long damage, float remaining, bool isSPDamage = false)
    {
        StopAllCoroutines();
        DeactivePopupLabel();
        //StartCoroutine(ShowBloodChange(damage, true));
        ShowBloodChange(damage, true, isSPDamage);

        UpdateBloodBar(remaining);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="recover">recover</param>
    /// <param name="remaining"></param>
    public void OnRecover(float recover, float remaining)
    {
        StopAllCoroutines();
        DeactivePopupLabel();
        //StartCoroutine(ShowBloodChange(recover, false));
        ShowBloodChange((long)recover, false, false);

        UpdateBloodBar(remaining);
    }

    public void UpdateBloodBar(float remaining)
    {
        RemainingBlood = remaining;
        ProgressBar.value = RemainingBlood / TotalBlood;
    }

    private readonly Vector3 originalPos = new Vector3(0, 470, 0);
    private readonly Vector3 halfPos = new Vector3(0, 520, 0);
    private readonly Vector3 finalPos = new Vector3(0, 570, 0);
    private const float moveDuration = 0.5f;
    private const float stayDuration = 0.25f;

    private void ShowBloodChange(long change, bool isSub, bool isSPDamage)
    {
        string labelText = (isSub ? "-" : "+") + change;

        GlobalBloodLabelController.BloodLabelType type;
        if (!isSub)
        {
            type = GlobalBloodLabelController.BloodLabelType.Recover;
        }
        else if (IsSelf)
        {
            type = GlobalBloodLabelController.BloodLabelType.PlayerAttack;
        }
        else
        {
            type = GlobalBloodLabelController.BloodLabelType.EnemyAttack;
        }

        GlobalBloodLabelController.Instance.ShowBloodLabel(gameObject, labelText, type, isSPDamage);
    }

    [Obsolete]
    private IEnumerator ShowBloodChange(float change, bool isSub)
    {
        SetActivedPopupLabel(isSub);

        m_activedPopupLabel.text = (isSub ? "-" : "+") + change;

        //Move
        m_activedPopupLabel.transform.localPosition = originalPos;
        m_activedPopupLabel.color = new Color(m_activedPopupLabel.color.r, m_activedPopupLabel.color.g, m_activedPopupLabel.color.b, 1);
        m_activedPopupLabel.gameObject.SetActive(true);
        iTween.MoveTo(m_activedPopupLabel.gameObject, iTween.Hash("position", halfPos, "time", moveDuration, "easetype", "easeOutQuint", "islocal", true));

        yield return new WaitForSeconds(moveDuration);

        //Fade.
        m_activedPopupLabel.transform.localPosition = halfPos;
        iTween.MoveTo(m_activedPopupLabel.gameObject, iTween.Hash("position", finalPos, "time", stayDuration, "easetype", "easeOutQuint", "islocal", true));
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", stayDuration, "easetype", "linear", "onupdate", "OnUpdateBloodColor"));

        yield return new WaitForSeconds(stayDuration);

        DeactivePopupLabel();
    }

    private void OnUpdateBloodColor(float p_a)
    {
        m_activedPopupLabel.color = new Color(m_activedPopupLabel.color.r, m_activedPopupLabel.color.g, m_activedPopupLabel.color.b, p_a);
    }

    private void DeactivePopupLabel()
    {
        PopupLabel_Basic.gameObject.SetActive(false);
        PopupLabel_Important.gameObject.SetActive(false);
        PopupLabel_Recover.gameObject.SetActive(false);
    }

    private float checkTime;
    private readonly List<int> m_canMoveHashList = new List<int>()
        {
            Animator.StringToHash("Run"), Animator.StringToHash("BATC"), Animator.StringToHash("Stand")
        };

    void Update()
    {
        if (Time.realtimeSinceStartup - checkTime > 0.5f)
        {
            if (m_SinglePlayerController != null && m_canMoveHashList.Contains(AnimationHelper.GetAnimatorPlayingHash(m_Animator)))
            {
                m_SinglePlayerController.ActiveMove();
            }

            if (m_OtherPlayerController != null && m_canMoveHashList.Contains(AnimationHelper.GetAnimatorPlayingHash(m_Animator)))
            {
                m_OtherPlayerController.ActiveMove();
            }

            checkTime = Time.realtimeSinceStartup;
        }
    }

    void LateUpdate()
    {
        if (TrackCamera == null) return;

        m_UIParentObject.transform.eulerAngles = new Vector3(TrackCamera.transform.eulerAngles.x, TrackCamera.transform.eulerAngles.y, 0);
    }

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_ShadowObject.SetActive(Quality_Shadow.BattleField_ShowSimpleShadow());

        if (m_WeaponSwitcher == null)
        {
            m_WeaponSwitcher = gameObject.GetComponent<WeaponSwitcher>();
        }

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
