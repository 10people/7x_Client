using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    public Camera TrackCamera;

    [HideInInspector]
    public long m_PlayerID;
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

    public void PlayerRun(Vector3 targetPosition)
    {
        //m_positionList.Add(targetPosition);

        MovingOn(targetPosition);
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
