using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerInCity : MonoBehaviour { //在主城中跑动的玩家

    private string m_playerName;
    
    private string m_zhanli;
   
    private Vector3 m_position;

    public Transform m_playerNameTransform;

    [HideInInspector]
    public int m_playerID;
    [HideInInspector]
    public Transform m_transform;
    [HideInInspector]
    public NavMeshAgent m_Agent;
    [HideInInspector]
    public Animator m_animation;
    private bool isStop = false;

    private CharacterController m_character;
    private Vector3 m_moveDir;
    private List<Vector3> _listPos = new List<Vector3>();
    private bool _isMovingOn = false;
    private bool _isMovingComntinue = false;
    void Awake()
    {
        m_animation = this.GetComponent<Animator>();

        m_Agent = this.GetComponent<NavMeshAgent>();

        m_transform = this.transform;
    }

    void Start()
    {
        m_character = transform.GetComponent<CharacterController>();
      //  m_character.enabled = false;
        //m_Agent.enabled = false;
    }


    public void PlayerRun(Vector3 targetPosition)
    {
        //_isRun = false;
        //if (_listPos.Count == 0)
        //{
        //    Debug.Log("_listPos.Count_listPos.Count_listPos.Count ::" + _listPos.Count);
        //    _listPos.Add(targetPosition);
        //    MovingOn(targetPosition);
        //}
        //else
        //{
        //    Debug.Log("_listPos.Count_listPos.Count_listPos.Count ::" + _listPos.Count);
        //    _listPos.Add(targetPosition);
        //}
        _isRun = true;
        MovingOn(targetPosition);
    }
     float _timeInterval = 0;
     private bool _isRun = false;
    void Update()
    {
    
      //  if (_listPos.Count > 0)
        {
            //  Debug.Log(" m_Agent.remainingDistance m_Agent.remainingDistance m_Agent.remainingDistance ::" + m_Agent.remainingDistance);
            if (_isRun)
            {

                if (m_Agent != null)
                {
                    if (Mathf.Abs(m_Agent.remainingDistance) < 0.01f)
                    {
                        _isRun = false;
                        PlayerStop();
                    }
                }
                else
                {
                    _isRun = false;
                }
            }
        }
    }

    void MovingOn(Vector3 targetPosition)
    {
        m_animation.SetBool("inRun", true);
        m_Agent.speed = PlayerModelController.m_playerModelController.m_speed;

        m_Agent.Resume();
        m_Agent.SetDestination(targetPosition);

    }
 
    protected bool inTurning;



    public void PlayerStop()
    {
        _isRun = false;
        m_Agent.Stop();
        
        if (m_animation != null)
        {
            m_animation.SetBool("inRun", false); 
        }
    }



    public void DestoryObject()
    {
        Destroy(this.gameObject);
    }
}
