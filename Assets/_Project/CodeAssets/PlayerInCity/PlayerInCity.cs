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
        MOVE_TYPE_RUN,
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
      
        //Debug.Log("DistanceDistanceDistanceDistance ::" + Vector3.Distance(targetPosition, transform.position));
        if (Vector3.Distance(targetPosition, transform.position) < 0.35f)
        {
            return;
        }
        _isRun = true;
        _TargetPos = targetPosition;
        GameObject target = new GameObject();
        target.transform.position = new Vector3( targetPosition.x, transform.position.y, targetPosition.z);
//
        transform.LookAt(target.transform);
        Destroy(target);
       // transform.Rotate(0, Vector3.Angle(transform.forward, _TargetPos),0);
     //   m_Agent.stoppingDistance = m_Agent.remainingDistance;
       //    MovingOn(targetPosition);
       
    }
    void Update()
    {
        if (m_MoveType == MoveType.NONE)
        {
            _playerRandomAnimator = true;
        }

        if (_isRun && Mathf.Abs(Vector3.Distance(_TargetPos,transform.position)) > 0)
        {
            if (_TargetPos != transform.position)
            {
                Move(_TargetPos);
            }
            else
            {
                _isRun = false;
                PlayerStop();
            }
        }
        else if (Mathf.Abs(Vector3.Distance(_TargetPos, transform.position)) <= 0.01f)
        {
         
        }
        //else if(!_playerRandomAnimator)
        //{
        //    StartCoroutine(StoPMove());
        //}

        //if (m_Agent != null)
        //{
        //    if (Mathf.Abs(m_Agent.remainingDistance) < 0.01f)
        //    {
        //        PlayerStop();
        //    }
        //    else
        //    {
        //        Move(_TargetPos);
        //    }
        //}
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
            AnimationPlay(1);
        }
        transform.localPosition = Vector3.Lerp(transform.position, point, PlayerModelController.m_playerModelController.m_speed * Time.deltaTime * 0.3f);
        //CharacterController controller = GetComponent<CharacterController>();
        //Vector3 v = Vector3.ClampMagnitude(point - transform.position, PlayerModelController.m_playerModelController.m_speed * Time.deltaTime);

        //controller.Move(v);
    }
    void MovingOn(Vector3 targetPosition)
    {
        if (_playerRandomAnimator)
        {
            _playerRandomAnimator = false;
            AnimationPlay(1);
        }

		if (PlayerModelController.m_playerModelController != null)
		{
			m_Agent.speed = PlayerModelController.m_playerModelController.m_speed;
		}
		else if (TreasureCityPlayer.m_instance != null)
		{
			m_Agent.speed = TreasureCityPlayer.m_instance.m_speed;
		}
        m_Agent.Resume();
        m_Agent.SetDestination(targetPosition);
    }
 
    protected bool inTurning;



    public void PlayerStop()
    {
        m_Agent.Stop();
     
 
        if (m_MoveType == MoveType.MOVE_TYPE_IDLE)
        {
            m_MoveType = MoveType.MOVE_TYPE_RUN;
            AnimationPlay(0);
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
    void MoveTurnToDestination(Vector3 targetPosition)
    {
        if (_playerRandomAnimator)
        {
            _playerRandomAnimator = false;
            AnimationPlay(1);
        }
        if (offObject == null)
        {
            offObject = new GameObject();
        }

        offObject.transform.localPosition = targetPosition;

        offObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        offObject.transform.localScale = new Vector3(1, 1, 1);

        if (camObject == null)
        {
            camObject = new GameObject();
        }

        camObject.transform.localPosition = new Vector3(0, 0, 0);

        camObject.transform.localEulerAngles = Vector3.zero;

        camObject.transform.localScale = new Vector3(1, 1, 1);

        offObject.transform.parent = camObject.transform;

        camObject.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);

        targetPosition = offObject.transform.position;



        if (angleObject == null)
        {
            angleObject = new GameObject();
        }

        angleObject.transform.localScale = new Vector3(1, 1, 1);

        angleObject.transform.localPosition = Vector3.zero;

        angleObject.transform.eulerAngles = m_transform.transform.eulerAngles;

        Vector3 oldangle = angleObject.transform.eulerAngles;

        angleObject.transform.forward = targetPosition;

        float tar = angleObject.transform.eulerAngles.y;

        float sp = 1080 * Time.deltaTime;

        float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);

        angleObject.transform.eulerAngles = new Vector3(0, angle, 0);


        {

       
            Vector3 sp2 = targetPosition * PlayerModelController.m_playerModelController.m_speed * Time.deltaTime;

            m_transform.forward = sp2;
        
            m_character.Move(sp2);
        }

    }
    public void DestoryObject()
    {
        Destroy(this.gameObject);
    }
}
