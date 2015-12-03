using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

namespace Carriage
{
    public class CarriageCultureController : MonoBehaviour
    {
        public CarriagePlayerController m_CarriagePlayerController;
        public PlayerController m_PlayerController;

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
            if (m_CarriagePlayerController == null && m_PlayerController == null)
            {
                m_CarriagePlayerController = GetComponent<CarriagePlayerController>();
                m_PlayerController = GetComponent<PlayerController>();
            }

            if (m_CarriagePlayerController != null)
            {
                m_CarriagePlayerController.ActiveMove();
            }

            if (m_PlayerController != null)
            {
                m_PlayerController.ActiveMove();
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

        public UILabel PopupLabel;

        public string KingName;
        public string AllianceName;
        public float TotalBlood;
        public float RemainingBlood;
        public bool IsRed;

        public GameObject PlayerSelectedSign;

        public void SetThis()
        {
            NameLabel.text = string.IsNullOrEmpty(KingName) ? "" : KingName;
            AllianceLabel.text = string.IsNullOrEmpty(AllianceName) ? "无联盟" : AllianceName;
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
            PopupLabel.text = (isSub ? (ColorTool.Color_Red_c40000 + "-") : (ColorTool.Color_Green_00ff00 + "+")) + change + "[-]";
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

            NguiPanel.transform.eulerAngles = new Vector3(TrackCamera.transform.eulerAngles.x, TrackCamera.transform.eulerAngles.y, NguiPanel.transform.eulerAngles.z);
        }
    }
}
