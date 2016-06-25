using UnityEngine;
using System.Collections;

namespace AllianceBattle
{
    public class ABHoldCultureController : RPGBaseCultureController
    {
        private const string redBarName = "BloodRed";
        private const string greenBarName = "BloodGreen";
        private const string blueBarName = "BloodBlue";

        public bool m_IsAlive = true;

        public GameObject FlagRodObject;
        public GameObject AttackEffect;
        public GameObject RedAreaEffect;
        public GameObject BlueAreaEffect;
        public GameObject RecoverEffect;
        public BoxCollider BoxCollider;

        private GameObject AreaEffect
        {
            get { return m_OccupyPoint.Side == 1 ? BlueAreaEffect : RedAreaEffect; }
        }

        public override void UpdateBloodBar(float remaining)
        {
            if (remaining > RemainingBlood)
            {
                RecoverEffect.SetActive(true);
            }
            else if (remaining < RemainingBlood)
            {
                RecoverEffect.SetActive(false);
            }

            base.UpdateBloodBar(remaining);

            if ((ProgressBar.value*100).ToString("n0") == "100")
            {
                RecoverEffect.SetActive(false);
            }
        }

        public new void OnClick()
        {
            if (m_IsAlive)
            {
                base.OnClick();
            }
        }

        public void SetHoldPointState(bool isAlive)
        {
            m_IsAlive = isAlive;

            FlagRodObject.SetActive(m_IsAlive);
            m_UIParentObject.SetActive(m_IsAlive);

            if (AttackEffect != null && !isAlive)
            {
                AttackEffect.SetActive(false);
            }
            if (RecoverEffect != null && !isAlive)
            {
                RecoverEffect.SetActive(false);
            }
            if (AreaEffect != null)
            {
                AreaEffect.SetActive(m_IsAlive);
            }

            if (!m_IsAlive && SelectEffect != null)
            {
                SelectEffect.SetActive(false);
            }
        }

        public override bool IsEnemy
        {
            get
            {
                return (m_OccupyPoint.IsProtecter && RootManager.Instance.MyPart == 1) || (!m_OccupyPoint.IsProtecter && RootManager.Instance.MyPart == 2);
            }
        }

        public override bool IsSelf
        {
            get
            {
                return false;
            }
        }

        [HideInInspector]
        public ABHoldPointManager.OccupyPoint m_OccupyPoint;

        public override void SetThis()
        {
            ProgressBarForeSprite.spriteName = IsEnemy ? redBarName : greenBarName;

            base.SetThis();
        }

        new void Awake()
        {
            base.Awake();
        }
    }
}