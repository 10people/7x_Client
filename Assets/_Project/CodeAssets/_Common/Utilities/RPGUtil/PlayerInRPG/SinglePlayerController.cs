using System;
using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

public class SinglePlayerController : MonoBehaviour
{
    #region Configs

    /// <summary>
    /// Is fixed or rotate main camera.
    /// </summary>
    public bool IsRotateCamera = false;

    /// <summary>
    /// Is upload player position or not.
    /// </summary>
    public bool IsUploadPlayerPosition = true;

    public float BaseGroundPosY;

    public static float m_CharacterSyncDuration = 0.2f;
    public static float m_CharacterMoveDistance = 0.1f;

    #endregion

    #region Move Controller

    public bool is_CanMove = true;

    public void DeactiveMove()
    {
        //Debug.LogWarning("--------------self deactive move.");

        StopPlayerNavigation();
        is_CanMove = false;
    }

    public void ActiveMove()
    {
        //Debug.LogWarning("+++++++++++++++++self active move.");

        is_CanMove = true;
    }

    #endregion

    public bool IsMainCityUIExist
    {
        get { return MainCityUI.m_MainCityUI != null; }
    }

    public virtual void OnPlayerRun()
    {
        Debug.LogError("OnPlayerRun not implemented in derived class");
    }

    public virtual void OnPlayerStop()
    {
        Debug.LogError("OnPlayerStop not implemented in derived class");
    }

    /// <summary>
    /// player uid used for server sync.
    /// </summary>
    public static int s_uid;

    /// <summary>
    /// Character stood and run animations.
    /// </summary>
    public Animator m_Animator;

    #region Move and navigation

    /// <summary>
    /// quit navigation when use character control
    /// </summary>
    public bool IsInNavigate;

    public delegate void VoidDelegate();
    public VoidDelegate m_CompleteNavDelegate;
    public VoidDelegate m_StartNavDelegate;
    public VoidDelegate m_EndNavDelegate;

    public Vector3 NavigationEndPosition;
    private float navigateDistance = 1f;

    public CharacterController m_CharacterController;
    public NavMeshAgent m_NavMeshAgent;

    public Joystick m_Joystick;

    public Vector3 m_RealJoystickOffset
    {
        get
        {
            return
#if UNITY_EDITOR || UNITY_STANDALONE
                new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
#else  
            m_Joystick.m_uiOffset;
#endif
        }
    }

    public static float m_CharacterSpeed = 6;
    public float m_CharacterSpeedY = 10f;
    public float m_NavigateSpeed = 6;

    /// <summary>
    /// how much degree does angle change per second.
    /// </summary>
    [HideInInspector]
    public const float angleSpeed = 1080;

    public Transform m_Transform;

    /// <summary>
    /// is moving means in navigation or character move.
    /// </summary>
    public bool m_IsMoving;

    /// <summary>
    /// Cannot start navi or use character control when turning.
    /// </summary>
    public bool m_IsTurning;

    public float m_LastNavigateTime;
    public Transform m_NavigationTransform;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tempPosition"></param>
    /// <param name="tempDistance">navigate distance, 1f for default.</param>
    public void StartNavigation(Vector3 tempPosition, float tempDistance = 1f)
    {
        if (m_RealJoystickOffset != Vector3.zero)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
            {
                Debug.LogWarning("Cancel navigation cause in character control");
            }
            return;
        }

        m_LastNavigateTime = Time.realtimeSinceStartup;

        navigateDistance = tempDistance;

        StopCoroutine("DoStartNavigation");
        StartCoroutine(DoStartNavigation(tempPosition));

        if (m_StartNavDelegate != null)
        {
            m_StartNavDelegate();
        }
    }

    private float navigationRotateSpeed = 1000f;

    public IEnumerator DoStartNavigation(Vector3 tempPosition)
    {
        m_IsTurning = true;

        while (true)
        {
            Vector3 oldAngle = transform.eulerAngles;

            //Get target angle.
            transform.forward = tempPosition - transform.position;
            float targetAngleY = transform.eulerAngles.y;

            //Get angle per frame.
            float maxDelta = navigationRotateSpeed * Time.deltaTime;
            float angle = Mathf.MoveTowardsAngle(oldAngle.y, targetAngleY, maxDelta);

            transform.eulerAngles = new Vector3(0, angle, 0);

            if (Mathf.Abs(targetAngleY - oldAngle.y) < 20)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        m_IsTurning = false;

        NavigationEndPosition = tempPosition;

        m_CharacterController.enabled = false;
        m_NavMeshAgent.enabled = true;

        OnPlayerRun();
        if (!m_IsMoving)
        {
            m_IsMoving = true;

            if (IsMainCityUIExist)
            {
                MainCityUIRB.IsCanClickButtons = false;
                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(false);
                UIYindao.m_UIYindao.setCloseUIEff();
            }
        }

        //        Debug.Log("Start navigate to position:" + tempPosition);

        m_NavMeshAgent.Resume();

        m_NavMeshAgent.SetDestination(tempPosition);

        IsInNavigate = true;
    }

    public void StopPlayerNavigation()
    {
        if (!IsInNavigate)
        {
            return;
        }

        OnPlayerStop();
        if (m_IsMoving)
        {
            m_IsMoving = false;

            if (IsMainCityUIExist)
            {
                MainCityUIRB.IsCanClickButtons = true;
                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(true);
                UIYindao.m_UIYindao.setOpenUIEff();
            }
        }

        m_NavMeshAgent.Stop();
        IsInNavigate = false;

        m_CharacterController.enabled = true;
        m_NavMeshAgent.enabled = false;

        if (m_EndNavDelegate != null)
        {
            m_EndNavDelegate();
        }
    }

    #endregion

    #region Character track camera

    public Camera TrackCamera;

    /// <summary>
    /// offset of positive axis y.
    /// </summary>
    [HideInInspector]
    [Obsolete("Only effective in camera rotate mode, this mode of camera has not exist in the game.")]
    public float TrackCameraOffsetPosUp;

    /// <summary>
    /// offset of negative axis z.
    /// </summary>
    [HideInInspector]
    [Obsolete("Only effective in camera rotate mode, this mode of camera has not exist in the game.")]
    public float TrackCameraOffsetPosBack;

    /// <summary>
    /// offset of up down rotation.
    /// </summary>
    [HideInInspector]
    [Obsolete("Only effective in camera rotate mode, this mode of camera has not exist in the game.")]
    public float TrackCameraOffsetUpDownRotation;

    public Vector3 TrackCameraPosition;
    public Vector3 TrackCameraRotation;

    public void LateUpdate()
    {
        if (TrackCamera == null)
        {
            return;
        }

        //Use camera back, up and updownrotate value.
        if (IsRotateCamera)
        {
            TrackCamera.transform.localPosition = transform.localPosition;
            TrackCamera.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            TrackCamera.transform.Translate(Vector3.up * TrackCameraOffsetPosUp);
            TrackCamera.transform.Translate(Vector3.back * TrackCameraOffsetPosBack);
            TrackCamera.transform.localEulerAngles = new Vector3(TrackCameraOffsetUpDownRotation, transform.localEulerAngles.y, 0);
        }
        //Use camera offset position and rotation.
        else
        {
            if (TrackCameraPosition == Vector3.zero || TrackCameraRotation == Vector3.zero)
            {
                TrackCameraPosition = TrackCamera.transform.localPosition;
                TrackCameraRotation = TrackCamera.transform.localEulerAngles;

                return;
            }

            TrackCamera.transform.localPosition = TrackCameraPosition + new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            TrackCamera.transform.localEulerAngles = TrackCameraRotation;
            //TrackCamera.transform.localPosition = transform.localPosition + new Vector3(0, TrackCameraOffsetPosUp, -TrackCameraOffsetPosBack);
        }
    }

    #endregion

    #region Character sync

    public float m_lastUploadCheckTime;
    private int m_sameTransformTimes;

    public Vector3 m_lastPosition;
    public Vector3 m_lastRotation;

    /// <summary>
    /// Commit this character's position to server.
    /// </summary>
    public void TryUploadPlayerPosition()
    {
        if (Vector3.Distance(m_lastPosition, transform.localPosition) < 0.1f && Vector3.Distance(m_lastRotation, transform.localEulerAngles) < 0.1f)
        {
            m_sameTransformTimes++;
        }
        else
        {
            m_sameTransformTimes = 0;
        }

        if (m_sameTransformTimes >= 2)
        {
            return;
        }

        SpriteMove tempPositon = new SpriteMove
        {
            posX = transform.localPosition.x,
            posY = transform.localPosition.y,
            posZ = transform.localPosition.z,
            dir = transform.localEulerAngles.y
        };

        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        t_qx.Serialize(t_tream, tempPositon);

        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.Sprite_Move, ref t_protof, false);

        m_lastPosition = transform.localPosition;
        m_lastRotation = transform.localEulerAngles;
    }

    #endregion

    public void OnDestroy()
    {
        StopPlayerNavigation();

        m_CharacterController = null;

        m_NavMeshAgent = null;

        m_Animator = null;
    }

    public void Update()
    {
        #region Character Sync

        if (IsUploadPlayerPosition && Time.realtimeSinceStartup - m_lastUploadCheckTime >= m_CharacterSyncDuration)
        {
            m_lastUploadCheckTime = Time.realtimeSinceStartup;

            TryUploadPlayerPosition();
        }

        #endregion

        ClientMain.m_ClientMainObj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);

        if (m_RealJoystickOffset != Vector3.zero && IsInNavigate)
        {
            StopPlayerNavigation();
        }

        #region Character Controller

        //Always execute gravity.
        if (m_CharacterController.enabled && !m_CharacterController.isGrounded)
        {
            m_CharacterController.Move(new Vector3(0, -m_CharacterSpeedY * Time.deltaTime, 0));
        }

        if (!is_CanMove)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
            {
                //Debug.Log("Cancel move cause controller set.");
            }
        }
        else
        {
            if (!m_IsTurning && !IsInNavigate)
            {
                Vector3 offset = m_RealJoystickOffset;

                if (offset != Vector3.zero)
                {
                    if (IsRotateCamera)
                    {
                        Vector3 normalizedOffset = offset.normalized;
                        bool isMoveForward = false;

                        double angleTemp = Math.Atan2(normalizedOffset.x, normalizedOffset.z) / Math.PI * 180;
                        double distance = Vector2.Distance(Vector2.zero, new Vector2(offset.x, offset.z));
                        if (60 < angleTemp && 180 > angleTemp)
                        {
                            m_Transform.localEulerAngles = new Vector3(0, (float)(m_Transform.localEulerAngles.y + angleSpeed * Time.deltaTime), 0);
                        }
                        else if (angleTemp > -180 && angleTemp < -60)
                        {
                            m_Transform.localEulerAngles = new Vector3(0, (float)(m_Transform.localEulerAngles.y - angleSpeed * Time.deltaTime), 0);
                        }
                        else if (angleTemp > -60 && angleTemp < 60)
                        {
                            isMoveForward = true;
                        }

                        OnPlayerRun();

                        if (!m_IsMoving)
                        {
                            m_IsMoving = true;

                            if (IsMainCityUIExist)
                            {
                                MainCityUIRB.IsCanClickButtons = false;
                                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(false);
                                UIYindao.m_UIYindao.setCloseUIEff();
                            }
                        }

                        Vector3 moveDirection = transform.forward;

                        //move if offset above half distance
                        if (distance > Joystick.MaxToggleDistance / 2.0f || isMoveForward)
                        {
                            m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        Vector3 moveDirection = offset.normalized;
                        double degree = -TrackCamera.transform.localEulerAngles.y * Math.PI / 180;
                        moveDirection = new Vector3((float)(Math.Cos(degree) * moveDirection.x - Math.Sin(degree) * moveDirection.z), 0, (float)(Math.Cos(degree) * moveDirection.z + Math.Sin(degree) * moveDirection.x));

                        OnPlayerRun();

                        if (!m_IsMoving)
                        {
                            m_IsMoving = true;

                            if (IsMainCityUIExist)
                            {
                                MainCityUIRB.IsCanClickButtons = false;
                                MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(false);
                                UIYindao.m_UIYindao.setCloseUIEff();
                            }
                        }

                        //rotate and move.
                        transform.forward = moveDirection.normalized;

                        m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    OnPlayerStop();

                    if (m_IsMoving)
                    {
                        m_IsMoving = false;

                        if (IsMainCityUIExist)
                        {
                            MainCityUIRB.IsCanClickButtons = true;
                            MainCityUI.m_MainCityUI.m_MainCityUIRB.SetPanel(true);
                            UIYindao.m_UIYindao.setOpenUIEff();
                        }
                    }
                }
            }
        }

        #endregion

        #region Navigation Controller

        if (IsInNavigate)
        {
            //Check navigation remaining destination
            if (Vector3.Distance(m_Transform.position, NavigationEndPosition) <= navigateDistance)
            {
                if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
                {
                    Debug.LogWarning("=======end navi with:" + m_CompleteNavDelegate);
                }

                StopPlayerNavigation();
                if (m_CompleteNavDelegate != null)
                {
                    var tempDelegate = m_CompleteNavDelegate;
                    m_CompleteNavDelegate = null;
                    tempDelegate();
                }
            }
        }

        #endregion
    }

    public void Start()
    {
        m_Transform = transform;
        m_CharacterController.enabled = true;
        m_NavMeshAgent.enabled = false;

        if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE))
        {
            m_CharacterSpeed = 20;
            m_NavigateSpeed = 20;
        }

        m_NavMeshAgent.speed = m_NavigateSpeed;
        m_NavMeshAgent.acceleration = 10000.0f;

        //TrackCameraOffsetUpDownRotation = TrackCamera.transform.localEulerAngles.x;
        //TrackCameraOffsetPosUp = TrackCamera.transform.localPosition.y - BaseGroundPosY;
        //TrackCameraOffsetPosBack = -TrackCamera.transform.localPosition.z;
    }

    public void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }
}
