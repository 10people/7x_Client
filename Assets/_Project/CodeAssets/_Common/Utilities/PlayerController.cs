//#define DEBUG_MOVE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Carriage;

public class PlayerController : MonoBehaviour
{
    #region Move Controller

    public bool IsCanMove = true;
    public bool IsInMove = false;

    public void DeactiveMove()
    {
        Debug.LogWarning("--------------other deactive move.");

        PlayerStop();
        IsCanMove = false;
    }

    public void ActiveMove()
    {
        Debug.LogWarning("+++++++++++++++other active move.");

        IsCanMove = true;
    }

    #endregion

    public Camera TrackCamera;

    public bool IsCarriage
    {
        get { return m_RoleID >= 50000; }
    }

    [HideInInspector]
    public int m_UID;
    [HideInInspector]
    public int m_RoleID;
    [HideInInspector]
    public Transform m_transform;
    [HideInInspector]
    public NavMeshAgent m_Agent;
    [HideInInspector]
    public Animator m_animation;
    [HideInInspector]
    public CharacterController m_CharacterController;

    public float m_CharacterLerpDuration;

    void Awake()
    {
        m_animation = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
    }

	void Update()
    {
	    var precentFromMove = (Time.realtimeSinceStartup - m_startMoveTime) / m_CharacterLerpDuration;

	    //Server sync check.
		if (Time.realtimeSinceStartup - CarriageItemSyncManager.m_LatestServerSyncTime > 
		    NetworkHelper.GetPingSecWithMin( Console_SetNetwork.GetMinPingForPreRun() ) * NetworkHelper.GetPreRunC() )
		{
#if DEBUG_MOVE
            Debug.LogWarning("++++++++++++Limit other move");
#endif
            return;
        }

        //Optimize character sync in client.
        if (precentFromMove > 1 && IsInMove)
        {
            if (m_animation != null)
            {
                m_animation.SetBool("Move", true);
            }

            var tempPosition = m_targetPosition * 2 - m_lastTargetPosition;
            var tempRotation = m_targetRotation * 2 - m_lastTargetRotation;

#if DEBUG_MOVE
            Debug.LogWarning("==========Forcast other move");
#endif

            StartPlayerTransformTurn(tempPosition, tempRotation);
        }

        if (precentFromMove >= 0 && precentFromMove <= 1 && IsCanMove)
        {
            IsInMove = true;

            if (m_animation != null)
            {
                m_animation.SetBool("Move", true);
            }

			{
				transform.localPosition = Vector3.Lerp(m_tryStartMovePosition, m_targetPosition, (float)precentFromMove);
			}

			if (Math.Abs(m_tryStartMoveRotation.y - m_targetRotation.y) > 180)
            {
                m_tryStartMoveRotation = new Vector3(m_tryStartMoveRotation.x, (m_tryStartMoveRotation.y < m_targetRotation.y ? (m_tryStartMoveRotation.y + 360) : (m_tryStartMoveRotation.y - 360)), m_tryStartMoveRotation.z);
            }

            transform.localEulerAngles = Vector3.Lerp(m_tryStartMoveRotation, m_targetRotation, (float)precentFromMove);
        }
    }

    private Vector3 m_lastTargetPosition;
    private Vector3 m_lastTargetRotation;

    private Vector3 m_targetPosition;
    private Vector3 m_targetRotation;
    private Vector3 m_tryStartMovePosition;
    private Vector3 m_tryStartMoveRotation;

    private float m_startMoveTime;

    public void StartPlayerTransformTurn(Vector3 p_targetPosition, Vector3 p_targetRotation)
    {
        if (!IsCanMove)
        {
            Debug.Log("Cancel move cause controller set.");
            return;
        }

        //Record now transform
        m_tryStartMovePosition = transform.localPosition;
        m_tryStartMoveRotation = transform.localEulerAngles;

        //Set target transform and record last target transform.
        m_lastTargetPosition = m_targetPosition;
        m_targetPosition = new Vector3(p_targetPosition.x, m_tryStartMovePosition.y, p_targetPosition.z);
        if (IsCarriage)
        {
            var direction = p_targetPosition - m_tryStartMovePosition;
            m_targetRotation = new Vector3(0, (float)(Math.Atan2(direction.x, direction.z) / Math.PI * 180), 0);
        }
        else
        {
            m_lastTargetRotation = m_targetRotation;
            m_targetRotation = p_targetRotation;
        }

        //Set now time.
        if ((Vector3.Distance(m_tryStartMovePosition, m_targetPosition) > SinglePlayerController.m_CharacterMoveDistance || Vector3.Distance(m_tryStartMoveRotation, m_targetRotation) > SinglePlayerController.m_CharacterMoveDistance))
        {
            m_startMoveTime = Time.realtimeSinceStartup - Time.deltaTime;
        }

        //Stop player.
        if ((Vector3.Distance(m_tryStartMovePosition, m_targetPosition) < SinglePlayerController.m_CharacterMoveDistance && Vector3.Distance(m_tryStartMoveRotation, m_targetRotation) < SinglePlayerController.m_CharacterMoveDistance))
        {
            PlayerStop();
        }
    }

    public void PlayerStop()
    {
        IsInMove = false;

        m_targetPosition = m_tryStartMovePosition = transform.localPosition;
        m_targetRotation = m_tryStartMoveRotation = transform.localEulerAngles;

        if (m_animation != null)
        {
            m_animation.SetBool("Move", false);
        }
    }

    public void DestoryObject()
    {
        Destroy(gameObject);
    }
}
