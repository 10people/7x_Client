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
        public PlayerController m_PlayerController;

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
            if (UID == PlayerSceneSyncManager.Instance.m_MyselfUid)
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
                RootManager.Instance.m_CarriageItemSyncManager.DestroyPlayer(UID);
            }
        }

        private void EnableMove()
        {
            TryGetController();

            if (m_CarriagePlayerController != null)
            {
                m_CarriagePlayerController.ActiveMove();
            }

            if (m_PlayerController != null)
            {
                m_PlayerController.ActiveMove();
            }
        }

        private bool isGetController = false;

        private void TryGetController()
        {
            if (!isGetController)
            {
                m_CarriagePlayerController = GetComponent<CarriagePlayerController>();
                m_PlayerController = GetComponent<PlayerController>();
            }

            if (m_CarriagePlayerController != null || m_PlayerController != null)
            {
                isGetController = true;
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
        public UISprite VIPSprite;
        public UILabel LevelLabel;

        public UILabel PopupLabel
        {
            get
            {
                return JunZhuData.Instance().m_junzhuInfo.name == KingName ? PopupLabel_Important : PopupLabel_Basic;
            }
        }
        public UILabel PopupLabel_Basic;
        public UILabel PopupLabel_Important;

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
        public bool IsEnemy;
        public int Money;
        public int HorseLevel;

        public int UID;
        public int RoleID;

        public GameObject PlayerSelectedSign;

        public virtual void SetThis()
        {
            NameLabel.text = string.IsNullOrEmpty(KingName) ? "" : MyColorData.getColorString(9, "[b]" + KingName + "[/b]");
            AllianceLabel.text = (string.IsNullOrEmpty(AllianceName) || AllianceName == "***") ? MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT)) : (MyColorData.getColorString(12, "<" + AllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(AlliancePost));
            VIPSprite.spriteName = "vip" + Vip;
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : greenBarName;

            if (TotalBlood > 0 || RemainingBlood <= TotalBlood)
            {
                UpdateBloodBar(RemainingBlood);
            }
        }

        public void OnCarriageItemClick()
        {
            TryGetController();

            if (m_PlayerController != null)
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
        public void OnDamage(float damage, float remaining)
        {
            StopAllCoroutines();
            DeactivePopupLabel();
            StartCoroutine(ShowBloodChange(damage, true));

            UpdateBloodBar(remaining);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recover">recover</param>
        public void OnRecover(float recover, float remaining)
        {
            StopAllCoroutines();
            DeactivePopupLabel();
            StartCoroutine(ShowBloodChange(recover, false));

            UpdateBloodBar(remaining);
        }

        private void UpdateBloodBar(float remaining)
        {
            RemainingBlood = remaining;
            ProgressBar.value = RemainingBlood / TotalBlood;
        }

        private IEnumerator ShowBloodChange(float change, bool isSub)
        {
            PopupLabel.text = (isSub ? "-" : "+") + change;
            PopupLabel.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.0f);
            DeactivePopupLabel();
        }

        private void DeactivePopupLabel()
        {
            PopupLabel.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (TrackCamera == null) return;

            NguiPanel.transform.eulerAngles = new Vector3(TrackCamera.transform.eulerAngles.x, TrackCamera.transform.eulerAngles.y, 0);
        }
    }
}
