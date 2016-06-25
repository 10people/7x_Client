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

    float _timeInterval = 0;
    private bool _isRun = false;
    private bool _PlaySet = false;
    private bool _isTimeGo = false;
    private float _time_StayAnimator = 0.0f;
    private bool _playerRandomAnimator = false;
    private Vector3 _TargetPos;
    private enum AnimationType
    {
        INIDLE,
        RELAX_1,
        RELAX_2
    }
    private AnimationType _PlayerTyoe = new AnimationType();
    private AnimationType _PlayerTyoeMiddle = new AnimationType();
    private int _time_limit = 0;
    private enum MoveType
    {
        NONE,
        MOVE_TYPE_STOPRUN,
        MOVE_TYPE_IDLE
    }
    MoveType m_MoveType = new MoveType();
    void Awake()
    {
        m_Agent = this.GetComponent<NavMeshAgent>();

        m_transform = this.transform;
    }

    void Start()
    {
        _TargetPos = Vector3.zero;
        _time_limit = Random.Range(5, 15);
        m_MoveType = MoveType.NONE;
        _PlayerTyoe = AnimationType.INIDLE;
        m_character = transform.GetComponent<CharacterController>();
        //  m_character.enabled = false;
        //m_Agent.enabled = false;
    }
 
    public void PlayerRun(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, transform.position) < 0.4f)
        {
            return;
        }
        _isRun = true;
        _isTarget = false;
        _TargetPos = targetPosition;
        GameObject target = new GameObject();
        target.transform.position = new Vector3( targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(target.transform);
        Destroy(target);
    }
    bool _isTarget = false;
    void Update()
    {
        if (m_MoveType == MoveType.NONE)
        {
            _playerRandomAnimator = true;
        }
        else if (m_MoveType == MoveType.MOVE_TYPE_IDLE
            && Mathf.Abs(Vector3.Distance(_TargetPos, transform.position)) <= 0.4f && !_isTarget)
        {
            _isTarget = true;
            m_MoveType = MoveType.MOVE_TYPE_IDLE;
            _isRun = false;
            PlayerStop();
        }

        if (_isRun && Mathf.Abs(Vector3.Distance(_TargetPos,transform.position)) > 0)
        {
            if (Mathf.Abs(Vector3.Distance(_TargetPos, transform.position)) > 0.4f)
            {
                Move(_TargetPos);
            }
            else
            {
                m_MoveType = MoveType.MOVE_TYPE_IDLE;
                _isRun = false;
                PlayerStop();
            }
        }
       
       
        if (_playerRandomAnimator && !_isTimeGo)
        {
            _time_StayAnimator += Time.deltaTime;
            if (_time_StayAnimator >= _time_limit)
            {
                _time_limit = Random.RandomRange(5, 15);
                _isTimeGo = true;
                switch (_PlayerTyoe)
                {
                    case AnimationType.INIDLE:
                        {
                            _PlayerTyoeMiddle = _PlayerTyoe;
                            AnimationPlay(2);
                            _PlayerTyoe = AnimationType.RELAX_1;
                        }
                        break;
                    case AnimationType.RELAX_2:
                        {
                            _PlayerTyoeMiddle = _PlayerTyoe;
                            AnimationPlay(3);
                            _PlayerTyoe = AnimationType.RELAX_1;
                        }

                        break;

                }

                _time_StayAnimator = 0;
            }
        }
        else if (_time_StayAnimator > 0)
        {
            _time_StayAnimator = 0;

        }


        if ((IsPlayComplete("inRelax_1") || IsPlayComplete("inRelax_2")) && _PlayerTyoe == AnimationType.RELAX_1)
        {
            _isTimeGo = false;
            m_MoveType = MoveType.MOVE_TYPE_IDLE;
            PlayerStop();
            if (_PlayerTyoeMiddle == AnimationType.INIDLE)
            {
                _PlayerTyoe = AnimationType.RELAX_2;
            }
            else
            {
                _PlayerTyoe = AnimationType.INIDLE;
            }
        }
    }

    void Move(Vector3 point)
    {
        if (_playerRandomAnimator)
        {
            _playerRandomAnimator = false;
   
        }
        AnimationPlay(1);
        transform.localPosition = Vector3.Lerp(transform.position, point, PlayerModelController.m_playerModelController.m_speed * Time.deltaTime * 0.3f);
        //CharacterController controller = GetComponent<CharacterController>();
        //Vector3 v = Vector3.ClampMagnitude(point - transform.position, PlayerModelController.m_playerModelController.m_speed * Time.deltaTime);

        //controller.Move(v);
    }
    
    protected bool inTurning;
 
    public void PlayerStop()
    {
       // m_Agent.Stop();

        if (m_MoveType == MoveType.MOVE_TYPE_IDLE)
        {
            AnimationPlay(0);
            m_MoveType = MoveType.MOVE_TYPE_STOPRUN;
            _playerRandomAnimator = true;
        }
    }

    public void AnimationPlay(int index)
    {
        if (m_animation)
        {
            switch (index)
            {
                case 0:
                    {
                        _playerRandomAnimator = true;
                        //m_animation.SetTrigger("iniDle");
                        m_animation.Play("zhuchengidle");
                    }
                    break;
                case 1:
                    {
                        _isTimeGo = false;
                        _playerRandomAnimator = false;
                        _PlayerTyoe = AnimationType.INIDLE;
                        m_MoveType = MoveType.MOVE_TYPE_IDLE;
                        //m_animation.SetTrigger("inRun");
                        m_animation.Play("zhuchengrun");
                    }
                    break;
                case 2:
                    {
                        //m_animation.SetTrigger("inRelax_1");
                        m_animation.Play("zhuchengrelax_1");
                    }
                    break;
                case 3:
                    {
                        //m_animation.SetTrigger("inRelax_2");
                        m_animation.Play("zhuchengrelax_2");
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public bool IsPlayComplete(string _name)
    {
        if (m_animation == null) return false;

        AnimatorClipInfo[] t_states = m_animation.GetCurrentAnimatorClipInfo(0);
        AnimatorStateInfo info = m_animation.GetCurrentAnimatorStateInfo(0);
        float playing = Mathf.Clamp01(info.normalizedTime);

        for (int i = 0; i < t_states.Length; /*i++*/ )
        {
            AnimatorClipInfo t_item = t_states[i];

            if (t_item.clip.name.Equals(_name) && playing >= 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    private GameObject camObject;
    private GameObject offObject;
    private GameObject angleObject;
    
    public void DestoryObject()
    {
        Destroy(this.gameObject);
    }
}
