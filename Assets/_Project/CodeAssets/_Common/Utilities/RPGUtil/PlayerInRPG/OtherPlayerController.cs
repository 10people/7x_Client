using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    #region Move Controller

    private bool IsCanMove = true;
    private bool ReadyToCorrectPos = false;
    private bool IsInMove = false;

    private float possibleMinYPos = 18f;
    private float possibleMaxYPos = 24f;

    /// <summary>
    /// Is use character controller or lerp position to move.
    /// </summary>
    public bool IsUseCharacterController = true;

    public void DeactiveMove()
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
        {
            Debug.LogWarning("--------------other deactive move.");
        }

        PlayerStop();
        IsCanMove = false;
    }

    public void ActiveMove()
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
        {
            Debug.LogWarning("+++++++++++++++other active move.");
        }

        IsCanMove = true;
    }

    #endregion

    public Camera TrackCamera;

    public bool IsCarriage
    {
        get { return m_RoleID >= 50000 && m_RoleID < 100000; }
    }

    public bool IsHold
    {
        get { return m_RoleID >= 100000; }
    }

    [HideInInspector]
    public int m_UID;
    [HideInInspector]
    public int m_RoleID;
    [HideInInspector]
    public Transform m_transform;
    //[HideInInspector]
    //public NavMeshAgent m_Agent;
    [HideInInspector]
    public Animator m_Animator;
    [HideInInspector]
    public CharacterController m_CharacterController;

    public float m_CharacterLerpDuration;
    public float m_CharacterSpeedY = 10f;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        //m_Agent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        m_CharacterController.enabled = true;
        //m_Agent.enabled = false;
    }

    void Update()
    {
        //Always execute gravity.
        if (m_CharacterController.enabled && !m_CharacterController.isGrounded)
        {
            m_CharacterController.Move(new Vector3(0, -m_CharacterSpeedY * Time.deltaTime, 0));
        }

        var precentFromMove = (Time.realtimeSinceStartup - m_startMoveTime) / m_CharacterLerpDuration;

        //Close this for better percision.

        ////Server sync check.
        //if (Time.realtimeSinceStartup - PlayerManager.m_LatestServerSyncTime >
        //    NetworkHelper.GetPingSecWithMin(Console_SetNetwork.GetMinPingForPreRun()) * NetworkHelper.GetPreRunC())
        //{
        //    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
        //    {
        //        Debug.LogWarning("++++++++++++Limit other move" + Time.realtimeSinceStartup + "uid:" + m_UID);
        //    }

        //    return;
        //}

        ////Optimize character sync in client.
        //if (precentFromMove > 1 && IsInMove)
        //{
        //    if (m_Animator != null)
        //    {
        //        m_Animator.SetBool("Move", true);
        //    }

        //    var tempPosition = m_targetPosition * 2 - (isRealServerData ? m_lastTargetServerPosition : m_lastTargetPosition);
        //    var tempRotation = m_targetRotation * 2 - (isRealServerData ? m_lastTargetServerRotation : m_lastTargetRotation);

        //    if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
        //    {
        //        Debug.LogWarning("==========Forcast other move, time:" + Time.realtimeSinceStartup + "uid:" + m_UID);
        //    }

        //    StartPlayerTransformTurn(tempPosition, tempRotation, false);
        //}

        if (!IsCanMove)
        {
            ReadyToCorrectPos = true;
        }
        else if (precentFromMove > 1 && ReadyToCorrectPos)
        {
            ReadyToCorrectPos = false;

            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
            {
                Debug.Log("Correct after move recover: " + m_UID + ", move from: " + transform.localPosition + " to: " + m_targetPosition);
            }

            float modifiedPos;
            if (TransformHelper.RayCastXToFirstCollider(m_targetPosition, out modifiedPos))
            {
                if (modifiedPos > possibleMinYPos && modifiedPos < possibleMaxYPos)
                {
                    transform.localPosition = new Vector3(m_targetPosition.x, modifiedPos, m_targetPosition.z);
                }
            }
            else
            {
                transform.localPosition = m_targetPosition;
            }

            transform.localEulerAngles = m_targetRotation;
        }

        //Moving
        if (precentFromMove >= 0 && precentFromMove <= 1 && IsCanMove)
        {
            IsInMove = true;

            if (m_Animator != null)
            {
                m_Animator.SetBool("Move", true);
            }

            if (IsUseCharacterController)
            {
                m_CharacterController.enabled = true;
                //m_Agent.enabled = false;

                var tempTargetPosition = Vector3.Lerp(m_tryStartMovePosition, m_targetPosition, (float)precentFromMove);

                m_CharacterController.Move(tempTargetPosition - transform.localPosition);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(m_tryStartMovePosition, m_targetPosition, (float)precentFromMove);
            }

            if (Math.Abs(m_tryStartMoveRotation.y - m_targetRotation.y) > 180)
            {
                m_tryStartMoveRotation = new Vector3(m_tryStartMoveRotation.x, (m_tryStartMoveRotation.y < m_targetRotation.y ? (m_tryStartMoveRotation.y + 360) : (m_tryStartMoveRotation.y - 360)), m_tryStartMoveRotation.z);
            }

            transform.localEulerAngles = Vector3.Lerp(m_tryStartMoveRotation, m_targetRotation, (float)precentFromMove);
        }

        //Stop
        if (precentFromMove > 1 && IsInMove)
        {
            IsInMove = false;

            CheckStuckAndSkipMove();

            if (m_Animator != null)
            {
                m_Animator.SetBool("Move", false);
            }
        }
    }

    private Vector3 m_lastTargetPosition;
    private Vector3 m_lastTargetRotation;
    private Vector3 m_lastTargetServerPosition;
    private Vector3 m_lastTargetServerRotation;

    private Vector3 m_targetPosition;
    private Vector3 m_targetRotation;
    private Vector3 m_targetServerPosition;
    private Vector3 m_targetServerRotation;
    private Vector3 m_tryStartMovePosition;
    private Vector3 m_tryStartMoveRotation;

    private float m_startMoveTime;
    private bool isRealServerData = false;

    private int skipMoveCount = 0;

    private void CheckStuckAndSkipMove()
    {
        var totalDistance = Vector3.Distance(m_tryStartMovePosition, m_targetPosition);
        var moveDistance = Vector3.Distance(m_tryStartMovePosition, transform.position);

        if (totalDistance > 2f && moveDistance / totalDistance < 0.2f)
        {
            skipMoveCount++;
        }
        else
        {
            skipMoveCount = 0;
        }

        if (skipMoveCount >= 10)
        {
            DoSkipMove();
            skipMoveCount = 0;
        }
    }

    private void DoSkipMove()
    {
        if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
        {
            Debug.LogWarning("[REAL TIME]Stuck and skip move: " + m_UID + ", move from: " + transform.localPosition + " to: " + m_targetPosition);
        }

        float modifiedPos;
        if (TransformHelper.RayCastXToFirstCollider(m_targetPosition, out modifiedPos))
        {
            if (modifiedPos > possibleMinYPos && modifiedPos < possibleMaxYPos)
            {
                transform.position = new Vector3(m_targetPosition.x, modifiedPos, m_targetPosition.z);
            }
        }
        else
        {
            transform.position = m_targetPosition;
        }
        transform.eulerAngles = m_targetRotation;
    }

    public void StartPlayerTransformTurn(Vector3 p_targetPosition, Vector3 p_targetRotation, bool isRealServerData)
    {
        CheckStuckAndSkipMove();

        if (!IsCanMove)
        {
            if (ConfigTool.GetBool(ConfigTool.CONST_LOG_REALTIME_MOVE))
            {
                Debug.Log("Move other player in cannot move mode, network latency may occur.");
            }
        }

        this.isRealServerData = isRealServerData;

        //Record now transform
        m_tryStartMovePosition = transform.localPosition;
        m_tryStartMoveRotation = transform.localEulerAngles;

        //Set target transform and record last target transform.
        m_lastTargetPosition = m_targetPosition;
        m_targetPosition = new Vector3(p_targetPosition.x, m_tryStartMovePosition.y, p_targetPosition.z);
        if (isRealServerData)
        {
            m_lastTargetServerPosition = m_targetServerPosition;
            m_targetServerPosition = m_targetPosition;
        }
        if (IsCarriage)
        {
            var direction = p_targetPosition - m_tryStartMovePosition;
            m_targetRotation = new Vector3(0, (float)(Math.Atan2(direction.x, direction.z) / Math.PI * 180), 0);
        }
        else
        {
            m_lastTargetRotation = m_targetRotation;
            m_targetRotation = p_targetRotation;
            if (isRealServerData)
            {
                m_lastTargetServerRotation = m_targetServerRotation;
                m_targetServerRotation = p_targetRotation;
            }
        }

        if ((Vector3.Distance(m_lastTargetPosition, m_targetPosition) < SinglePlayerController.m_CharacterMoveDistance && Vector3.Distance(m_lastTargetRotation, m_targetRotation) < SinglePlayerController.m_CharacterMoveDistance))
        {

        }
        else
        {
            //Set now time.
            m_startMoveTime = Time.realtimeSinceStartup - Time.deltaTime;
        }
    }

    public void PlayerStop()
    {
        IsInMove = false;

        m_targetPosition = m_tryStartMovePosition = transform.localPosition;
        m_targetRotation = m_tryStartMoveRotation = transform.localEulerAngles;

        if (m_Animator != null)
        {
            m_Animator.SetBool("Move", false);
        }
    }

    public void DestoryObject()
    {
        Destroy(gameObject);
    }
}
