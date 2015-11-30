using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    #region Move Controller

    private bool is_CanMove = true;

    public void DeactiveMove()
    {
        PlayerStop();
        is_CanMove = false;
    }

    public void ActiveMove()
    {
        is_CanMove = true;
    }

    #endregion

    public Camera TrackCamera;

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

    void Awake()
    {
        m_animation = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    private List<Vector3> m_positionList = new List<Vector3>();
    private Vector3 m_targetPosition;

    public void PlayerRun(Vector3 targetPosition)
    {
        m_targetPosition = targetPosition;

        if (!is_CanMove)
        {
            Debug.LogWarning("Cancel move cause controller set.");
            return;
        }
        MovingOn(m_targetPosition);
    }

    private void MovingOn(Vector3 targetPosition)
    {
        m_animation.SetBool("Move", true);

        m_Agent.Resume();

        m_Agent.SetDestination(targetPosition);
    }

    public void PlayerStop()
    {
        m_Agent.Stop();

        if (m_animation != null)
        {
            m_animation.SetBool("Move", false);
        }
    }

    public void DestoryObject()
    {
        Destroy(this.gameObject);
    }
}
