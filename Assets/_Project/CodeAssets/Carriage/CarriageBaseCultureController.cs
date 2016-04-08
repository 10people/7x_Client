using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageBaseCultureController : MonoBehaviour
    {
        public CarriagePlayerController m_CarriagePlayerController;
        public OtherPlayerController m_OtherPlayerController;

        public Animator m_Animator;
        public Renderer m_MainRenderer;
        public GameObject m_ShadowObject;

        public void OnSkillFinish()
        {
            EnableMove();
        }

        public void OnBeenSkillFinish()
        {
            EnableMove();
        }

        public void OnDeadFinish()
        {
            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageBaseDeadAnim" + UID))
            {
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageBaseDeadAnim" + UID);
            }
            TimeHelper.Instance.AddOneDelegateToTimeCalc("CarriageBaseDeadAnim" + UID, 1f, ExecuteAfterDeadFinish);
        }

        private void ExecuteAfterDeadFinish(string key)
        {
            int l_uId = int.Parse(key.Replace("CarriageBaseDeadAnim", ""));

            if (l_uId == PlayerSceneSyncManager.Instance.m_MyselfUid)
            {
                Destroy(RootManager.Instance.m_SelfPlayerController.gameObject);

                RootManager.Instance.m_SelfPlayerCultureController = null;
                RootManager.Instance.m_SelfPlayerController = null;
                RootManager.Instance.m_CarriageMain.DeactiveTarget();

                //Show dead dimmer.
                RootManager.Instance.m_CarriageMain.ShowDeadWindow(RootManager.Instance.m_CarriageItemSyncManager.m_StoredPlayerDeadNotify.killerUid, RootManager.Instance.m_CarriageItemSyncManager.m_StoredPlayerDeadNotify.remainAllLifeTimes, RootManager.Instance.m_CarriageItemSyncManager.m_StoredPlayerDeadNotify.autoReviveRemainTime, RootManager.Instance.m_CarriageItemSyncManager.m_StoredPlayerDeadNotify.onSiteReviveCost);
            }
            else
            {
                //Remove from mesh controller.
                if (RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic.ContainsKey(l_uId) && !RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic[l_uId].IsCarriage)
                {
                    ModelAutoActivator.UnregisterAutoActivator(RootManager.Instance.m_CarriageItemSyncManager.m_PlayerDic[l_uId].gameObject);
                }

                RootManager.Instance.m_CarriageItemSyncManager.DestroyPlayer(l_uId);
            }
        }

        private void EnableMove()
        {
            TryGetController();

            if (m_CarriagePlayerController != null)
            {
                m_CarriagePlayerController.ActiveMove();
            }

            if (m_OtherPlayerController != null)
            {
                m_OtherPlayerController.ActiveMove();
            }
        }

        private bool isGetController = false;

        private void TryGetController()
        {
            if (!isGetController)
            {
                m_CarriagePlayerController = GetComponent<CarriagePlayerController>();
                m_OtherPlayerController = GetComponent<OtherPlayerController>();
            }

            if (m_CarriagePlayerController != null || m_OtherPlayerController != null)
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

        public UIProgressBar ProgressBar;
        public UISprite ProgressBarForeSprite;
        private const string redBarName = "progressred";
        private const string greenBarName = "progressford";

        public UILabel NameLabel;
        public UILabel AllianceLabel;
        public UISprite VIPSprite;
        public UILabel LevelLabel;

        public bool IsSelf
        {
            get { return JunZhuData.Instance().m_junzhuInfo.name == KingName; }
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

        public bool IsEnemy
        {
            get { return !(((!string.IsNullOrEmpty(AllianceName) && !AllianceData.Instance.IsAllianceNotExist && (AllianceName == AllianceData.Instance.g_UnionInfo.name))) || (KingName == JunZhuData.Instance().m_junzhuInfo.name)); }
        }

        public int Money;
        public int HorseLevel;
        public bool IsChouRen;

        public int UID;
        public int RoleID;
        public long JunzhuID;

        public bool IsCarriage
        {
            get { return RoleID >= 50000; }
        }

        public GameObject PlayerSelectedSign;

        public virtual void SetThis()
        {
            AllianceLabel.text = (string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)) : (MyColorData.getColorString(12, "<" + AllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost));
            VIPSprite.spriteName = "vip" + Vip;
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : greenBarName;

            if (TotalBlood > 0 || RemainingBlood <= TotalBlood)
            {
                UpdateBloodBar(RemainingBlood);
            }
        }

        public void OnClick()
        {
            TryGetController();

            if (m_OtherPlayerController != null)
            {
                RootManager.Instance.m_CarriageMain.ActiveTarget(UID);
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

        private void UpdateBloodBar(float remaining)
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

            CarriageBloodLabelController.BloodLabelType type;
            if (!isSub)
            {
                type = CarriageBloodLabelController.BloodLabelType.Recover;
            }
            else if (IsSelf)
            {
                type = CarriageBloodLabelController.BloodLabelType.PlayerAttack;
            }
            else
            {
                type = CarriageBloodLabelController.BloodLabelType.EnemyAttack;
            }

            RootManager.Instance.m_CarriageBloodLabelController.ShowBloodLabel(gameObject, labelText, type, isSPDamage);
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
                if (m_CarriagePlayerController != null && m_canMoveHashList.Contains(AnimationHelper.GetAnimatorPlayingHash(m_Animator)))
                {
                    m_CarriagePlayerController.ActiveMove();
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
        }
    }
}
