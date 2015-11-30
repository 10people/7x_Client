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
        public YabiaoJunZhuInfo m_YabiaoJunZhuInfo;
        public RootManager m_rootManager;

        [HideInInspector]
        public List<Vector3> m_3DPositionList = new List<Vector3>();

        public List<Vector2> m_2DPositionList
        {
            get
            {
                if (m_positionList == null || m_positionList.Count == 0)
                {
                    m_positionList = m_3DPositionList.Select(item => new Vector2(item.x, item.z)).ToList();
                }

                return m_positionList;
            }
        }
        private List<Vector2> m_positionList = new List<Vector2>();

		public List<MathHelper.SegmentInFoldLine> m_SegmentList
        {
            get
            {
                if (m_segmentList == null || m_segmentList.Count == 0)
                {
                    m_segmentList = MathHelper.GetSegmentListFromFoldLine(m_2DPositionList);
                }

                return m_segmentList;
            }
        }
		private List<MathHelper.SegmentInFoldLine> m_segmentList = new List<MathHelper.SegmentInFoldLine>();

        private float usedTime;
        private float realUsedTime;
        private float totalTime;

        [HideInInspector]
        public float periodPrecent;
        private float realPeriodPrecent;

        #region Panel Content Controller

        public GameObject PanelContentRootObject;
        public GameObject ParticleRootObject;
        public UISpriteAnimation AnimationSprite;

        public UIAtlas CanAttack_CarriageState_Atlas;
        public GameObject m_ProtectParticlePrefab;
        public GameObject m_InAttackParticlePrefab;

        public UILabel KingNameLabel;
        public UILabel AllianceNameLabel;
        public UILabel m_ProtectTimeLabel;

        private float bloodPercent
        {
            get { return m_YabiaoJunZhuInfo.hp / (float)m_YabiaoJunZhuInfo.maxHp; }
        }
        public UIProgressBar m_BloodBar;

        private float m_PanelContentRefreshTime;
        private const float m_PanelContentRefreshGap = 1.0f;

        public void SetPanelContent()
        {
            m_BloodBar.value = bloodPercent;

            KingNameLabel.text = m_YabiaoJunZhuInfo.junZhuName;

            if (string.IsNullOrEmpty(m_YabiaoJunZhuInfo.lianMengName))
            {
                AllianceNameLabel.text = "无联盟";
            }
            else
            {
                AllianceNameLabel.text = "<" + m_YabiaoJunZhuInfo.lianMengName + ">";
            }
        }

        #endregion

        void OnClick()
        {
            m_rootManager.m_CarriageUi.SetCarriageInfo(m_YabiaoJunZhuInfo);

            m_rootManager.m_CarriagePlayerController.m_NavigationTransform = transform;
            m_rootManager.m_CarriagePlayerController.StartNavigation(transform.position);
        }

        private int currentState = -1;

        /// <summary>
        /// Control state machine.
        /// </summary>
        public void SetStateMachine()
        {
            if (m_YabiaoJunZhuInfo == null)
            {
                Debug.LogError("Go to set state machine when carriage info is null");
                return;
            }

            usedTime = m_YabiaoJunZhuInfo.usedTime / 1000.0f;
            totalTime = m_YabiaoJunZhuInfo.totalTime / 1000.0f;
            periodPrecent = m_YabiaoJunZhuInfo.usedTime / (float)m_YabiaoJunZhuInfo.totalTime;

            Debug.Log("Set state machine:" + m_YabiaoJunZhuInfo.state + ", precent: " + periodPrecent);

            switch (m_YabiaoJunZhuInfo.state)
            {
                //Can attack
                case 10:
                    {
                        GoToMoving();
                        break;
                    }
                //In attack
                case 20:
                    {
                        GoToFighting();
                        break;
                    }
                //Protected
                case 30:
                    {
                        GoToMoving();
                        break;
                    }
                //End
                case 40:
                    {
                        GoToEnd();
                        break;
                    }
                //Carriage destroy.
                case 50:
                    {
                        GoToDestroy();
                        break;
                    }
            }

            if (currentState != m_YabiaoJunZhuInfo.state)
            {
                switch (m_YabiaoJunZhuInfo.state)
                {
                    //Can attack
                    case 10:
                        {
                            m_ProtectTimeLabel.gameObject.SetActive(false);
                            ParticleRootObject.SetActive(false);
                            //Donot show animation sprite.
                            AnimationSprite.gameObject.SetActive(false);
                            //AnimationSprite.GetComponent<UISprite>().atlas = CanAttack_CarriageState_Atlas;
                            //AnimationSprite.gameObject.SetActive(true);
                            break;
                        }
                    //In attack
                    case 20:
                        {
                            m_ProtectTimeLabel.gameObject.SetActive(false);
                            AnimationSprite.gameObject.SetActive(false);
                            while (ParticleRootObject.transform.childCount != 0)
                            {
                                var child = ParticleRootObject.transform.GetChild(0);
                                child.parent = null;
                                Destroy(child.gameObject);
                            }
                            var temp = Instantiate(m_InAttackParticlePrefab) as GameObject;
                            TransformHelper.ActiveWithStandardize(ParticleRootObject.transform, temp.transform);
                            ParticleRootObject.SetActive(true);
                            break;
                        }
                    //Protected
                    case 30:
                        {
                            AnimationSprite.gameObject.SetActive(false);
                            while (ParticleRootObject.transform.childCount != 0)
                            {
                                var child = ParticleRootObject.transform.GetChild(0);
                                child.parent = null;
                                Destroy(child.gameObject);
                            }
                            var temp = Instantiate(m_ProtectParticlePrefab) as GameObject;
                            TransformHelper.ActiveWithStandardize(ParticleRootObject.transform, temp.transform);
                            ParticleRootObject.SetActive(true);

                            //Set protect time label.
                            m_ProtectTimeLabel.gameObject.SetActive(true);
                            storedProtectTime = m_YabiaoJunZhuInfo.baohuCD;
                            if (TimeHelper.Instance.IsTimeCalcKeyExist("CarriageControllerProtectTime"))
                            {
                                TimeHelper.Instance.RemoveFromTimeCalc("CarriageControllerProtectTime");
                            }
                            TimeHelper.Instance.AddEveryDelegateToTimeCalc("CarriageControllerProtectTime",
                                m_YabiaoJunZhuInfo.baohuCD, RefreshProtectTimeLabel);

                            break;
                        }
                    //End
                    case 40:
                        {
                            m_ProtectTimeLabel.gameObject.SetActive(false);
                            AnimationSprite.gameObject.SetActive(false);
                            ParticleRootObject.SetActive(false);
                            break;
                        }
                    //Carriage destroy.
                    case 50:
                        {
                            m_ProtectTimeLabel.gameObject.SetActive(false);
                            AnimationSprite.gameObject.SetActive(false);
                            ParticleRootObject.SetActive(false);
                            break;
                        }
                }

                currentState = m_YabiaoJunZhuInfo.state;
            }
        }

        /// <summary>
        /// used for time calc.
        /// </summary>
        private int storedProtectTime;
        void RefreshProtectTimeLabel(int second)
        {
            if (storedProtectTime - second > 0)
            {
                m_ProtectTimeLabel.text = ColorTool.Color_Blue_016bc5 + (storedProtectTime - second) + "秒" + "[-]";
            }
            else
            {
                m_ProtectTimeLabel.gameObject.SetActive(false);
                TimeHelper.Instance.RemoveFromTimeCalc("CarriageControllerProtectTime");
            }
        }

        #region State Machine

        public enum CarriageState
        {
            Moving,
            Fighting,
            End,
            Destroy
        }
        [HideInInspector]
        public CarriageState m_CarriageState;

        public void GoToMoving()
        {
            m_CarriageState = CarriageState.Moving;
            m_IsMoving = true;
        }

        public void GoToFighting()
        {
            m_CarriageState = CarriageState.Fighting;
            m_IsMoving = false;
        }

        public void GoToEnd()
        {
            m_CarriageState = CarriageState.End;
            m_IsMoving = false;
            DestroyCarriage(false);
        }

        public void GoToDestroy()
        {
            m_CarriageState = CarriageState.Destroy;
            m_IsMoving = false;
            DestroyCarriage(true);
        }

        #endregion

        #region Move control

        public Animator m_Animator;

        private const float m_infoDuration = 3.0f;

        /// <summary>
        /// Destroy carriage object, then destroy ui object.
        /// </summary>
        /// <param name="isPlayingAnimation"></param>
        /// <returns></returns>
        void DestroyCarriage(bool isPlayingAnimation)
        {
            if (isPlayingAnimation)
            {
                m_Animator.SetTrigger("die");
            }
            else
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("Destroy carriage:" + m_YabiaoJunZhuInfo.junZhuId);

            //Clear carriage manager.
            m_IsMoving = false;
            transform.parent = null;
            m_rootManager.m_CarriageManager.m_CarriageControllers.Remove(this);

            //Clear carriage ui.
            List<CarriageUIItemController> temp = m_rootManager.m_CarriageUi.m_CarriageUiItemControllers.Where(item => item.m_YabiaoJunZhuInfo.junZhuId == m_YabiaoJunZhuInfo.junZhuId).ToList();
            if (temp == null || temp.Count != 1)
            {
                Debug.LogError("Got ui error when destroy a carriage");
                return;
            }

            CarriageUIItemController controller2 = temp[0];
            controller2.transform.parent = null;
            Destroy(controller2.gameObject);
            m_rootManager.m_CarriageUi.m_CarriageUiItemControllers.Remove(controller2);
            m_rootManager.m_CarriageUi.m_Grid.Reposition();

            //Destroy this carriage object finally.
            Destroy(gameObject);
        }

        void SetTransform(float precent)
        {
            if (precent < 0 || precent > 1)
            {
                return;
            }

			var point = MathHelper.GetPointFromSegmentLine(precent, m_SegmentList);
            transform.localPosition = new Vector3(point.x, m_rootManager.BasicYPosition, point.y);

			var direction = MathHelper.GetDirectionFromSegmentLine(precent, m_SegmentList).normalized;
            transform.forward = new Vector3(direction.x, 0, direction.y);

            //Bezier curve.
            //var bezierPoint = BezierUtility.GetBezierPoint(precent, m_2DPositionList);
            //var temp = precent > 0.95 ? BezierUtility.GetBezierPoint(precent - 0.01, m_2DPositionList) : BezierUtility.GetBezierPoint(precent + 0.01, m_2DPositionList);

            //transform.localPosition = new Vector3(bezierPoint.x, m_rootManager.BasicYPosition, bezierPoint.y);

            //transform.forward = (precent > 0.95 ? new Vector3(bezierPoint.x, m_rootManager.BasicYPosition, bezierPoint.y) - new Vector3(temp.x, m_rootManager.BasicYPosition, temp.y) : new Vector3(temp.x, m_rootManager.BasicYPosition, temp.y) - new Vector3(bezierPoint.x, m_rootManager.BasicYPosition, bezierPoint.y)).normalized;

            realPeriodPrecent = precent;
        }

        public void InitTransform()
        {
            SetTransform(periodPrecent);
        }

        public bool m_IsMoving;

        void Update()
        {
            //Set panel content.
            if (Time.realtimeSinceStartup > m_PanelContentRefreshTime + m_PanelContentRefreshGap)
            {
                PanelContentRootObject.transform.eulerAngles = new Vector3(m_rootManager.PlayerTrackCamera.transform.eulerAngles.x, 0, 0);

                m_PanelContentRefreshTime = Time.realtimeSinceStartup;
            }

            if (m_IsMoving)
            {
                m_Animator.SetBool("run", true);
                double precentGap = realPeriodPrecent - periodPrecent;
                if (precentGap > 0.05)
                {
                    SetTransform(realPeriodPrecent + Time.deltaTime / totalTime / 2);
                }
                else if (precentGap < -0.05)
                {
                    SetTransform(realPeriodPrecent + Time.deltaTime / totalTime * 2);
                }
                else
                {
                    SetTransform(realPeriodPrecent + Time.deltaTime / totalTime);
                }

                if (realPeriodPrecent > 1)
                {
                    GoToEnd();
                }
            }
            else
            {
                m_Animator.SetBool("run", false);
            }
        }

        #endregion

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }
    }
}
