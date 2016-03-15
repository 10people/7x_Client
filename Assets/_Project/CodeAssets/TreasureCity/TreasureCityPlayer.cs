using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityPlayer : TreasureCitySingleton<TreasureCityPlayer> {

	public Joystick joyStick;

	public float m_speed = 10.0f;
	public float m_speedY;

	public Camera m_mainCamera;
	private UIRoot m_root;

	private Vector3 m_currenPos;
	private Vector3 m_targetPos;

	private CharacterController m_character;
	public GameObject m_playerObj;
	private Transform m_transform;
	public Animator m_animator;
	public NavMeshAgent m_agent;

	private Vector3 m_moveDir;

	void Awake ()
	{
		base.Awake ();
	}

	void Start()
	{
		playerType = AnimationType.INIDLE;
	}

	IEnumerator ManualStart()
	{
		while (m_playerObj == null)
		{
			yield return new WaitForEndOfFrame();
		}
		
		while (TreasureCityRoot.m_instance.treasureCityUI == null)
		{
			yield return new WaitForEndOfFrame();
		}

		joyStick = TreasureCityRoot.m_instance.treasureCityUI.GetComponentInChildren<Joystick> ();
		m_root = TreasureCityRoot.m_instance.treasureCityUI.GetComponent<UIRoot>();
	}

	#region CreatePlayer Model

	public void CreatePlayerModel (Object p_object)
	{
		m_playerObj = Instantiate(p_object) as GameObject;
		m_playerObj.SetActive(true);

		m_playerObj.transform.localPosition = new Vector3(-15,0,-30);
		m_playerObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
		m_playerObj.transform.localScale = Vector3.one * 1.5f;

		m_character = m_playerObj.GetComponent<CharacterController>();
		m_animator = m_playerObj.GetComponent<Animator>();

		m_agent = m_playerObj.GetComponent<NavMeshAgent>();
		m_agent.enabled = true;
		
		m_transform = m_playerObj.transform;
		m_currenPos = m_transform.position;
		
		m_mainCamera.GetComponent<CameraRun>().target = m_playerObj.transform;
		m_mainCamera.GetComponent<CameraRun>().setCameraPos();
		
		SendPlayerData();

		StartCoroutine (ManualStart ());
	}

	void SendPlayerData()
	{
		EnterScene tempEnterScene = new EnterScene();
		
		tempEnterScene.senderName = SystemInfo.deviceName;
		tempEnterScene.uid = 0;
		
		tempEnterScene.posX = m_transform.position.x;
		tempEnterScene.posY = m_transform.position.y;
		tempEnterScene.posZ = m_transform.position.z;
		
		QXComData.SendQxProtoMessage (tempEnterScene,ProtoIndexes.Enter_TBBXScene);
		Debug.Log("I_EnterScene :" + ProtoIndexes.Enter_TBBXScene);
		{
			//  Debug.Log( "--- Scene Tag --- PlayerModelController.SendPlayerData --- State_LEAGUEOFCITY" );
			
			PlayerState t_state = new PlayerState();
			
			t_state.s_state = State.State_LEAGUEOFCITY;

			QXComData.SendQxProtoMessage (t_state,ProtoIndexes.PLAYER_STATE_REPORT);
		}
		
		Debug.Log("SendPlayerData. m_ObjHero.transform.position :" + m_playerObj.transform.position);
	}

	#endregion

	#region Update Player Info

	private float m_timeInterval = 0.0f;
	private float _time_StayAnimator = 0.0f;
	private float m_fPDistance = 0f;
	private bool _playerRandomAnimator = false;

	private Vector3 m_PlayerPostion = new Vector3(0, 0.4f, 0);

	private bool _isArrived = false;
	private bool _PlaySet = false;

	private bool m_isSendPos = true;
	public bool M_isSendPos { set{m_isSendPos = value;} get{return m_isSendPos;} }

	private int targetBoxUID;
	public int TargetBoxUID { set{targetBoxUID = value;} get{return targetBoxUID;} }

	private enum AnimationType
	{
		INIDLE,
		RELAX_1,
		RELAX_2,
	}
	private AnimationType playerType = new AnimationType();
	private AnimationType playerTypeMid = new AnimationType();
	private int _VecZeroNum = -1;

	void Update()
	{
		if (m_playerObj == null || joyStick == null)
		{
			return;
		}

		if (_playerRandomAnimator)
		{
			_time_StayAnimator += Time.deltaTime;

			if (_time_StayAnimator >= 3)
			{
				switch (playerType)
				{
				case AnimationType.INIDLE:

					playerTypeMid = playerType;
					AnimationPlay (2);
					playerType = AnimationType.RELAX_1;

					break;
				case AnimationType.RELAX_1:

					AnimationPlay (0);
					if (playerTypeMid == AnimationType.INIDLE)
					{
						playerType = AnimationType.RELAX_2;
					}
					else  
					{
						playerType = AnimationType.INIDLE;
					}
					
					break;
				case AnimationType.RELAX_2:

					playerTypeMid = playerType;
					AnimationPlay(3);
					playerType = AnimationType.RELAX_1;
					
					break;
				default:
					break;
				}
				_time_StayAnimator = 0;
			}
		}
		else if (_time_StayAnimator > 0)
		{
			_time_StayAnimator = 0;
		}

		//update audio listener positon
		if (m_playerObj != null)
		{
			ClientMain.m_ClientMainObj.transform.position = new Vector3 (m_playerObj.transform.position.x, m_playerObj.transform.position.y, m_playerObj.transform.position.z - 1);
		}
		m_timeInterval += Time.deltaTime;
		_time_StayAnimator += Time.deltaTime;

		//0.24秒刷新一次玩家位置  提交数据        
		if (m_timeInterval >= 0.05f && m_playerObj != null && M_isSendPos)
		{
			m_timeInterval = 0.0f;
			m_targetPos = m_transform.position;

			UpdatePlayerPosition ();
		}

		//navigation end execute.
		m_moveDir = joyStick.m_uiOffset.normalized;

		if (CityGlobalData.m_selfNavigation == true)
		{
			if (m_agent.remainingDistance <= 2.1f && m_fPDistance != 0f)
			{
				if (!TreasureCityUI.IsWindowsExist() && !_isArrived)
				{
					_isArrived = true ;
					//宝箱按钮变亮
					TreasureCityUI.m_instance.BottomUI (true);
				}

				StopPlayerNavigation();
			}
			else
			{
				m_fPDistance = m_agent.remainingDistance;
			}
			if (m_moveDir == Vector3.zero)
			{
				return;
			}
		}
		
		//character control horizontal
		if (m_moveDir != Vector3.zero)
		{
			MoveTurnToDestination (m_moveDir);
			// m_transform.forward = m_moveDir;
//			m_animator.SetBool("inRun", true);

			if (_playerRandomAnimator)
			{
				AnimationPlay(1);
			}

			if (CityGlobalData.m_selfNavigation)
			{
//				Global.m_sPanelWantRun = "";
//				Global.m_isShowBianqiang = false;
//				Global.m_sBianqiangClick = "";
				StopPlayerNavigationMove ();
			}
			
			if (!isMoving)
			{
				isMoving = true;

//				MainCityUIRB.IsCanClickButtons = false;
//				UIYindao.m_UIYindao.setCloseUIEff();
			}
		}
		else
		{
			//stop character, enable right buttom btns.
//			m_animator.SetBool("inRun", false);
			if (_VecZeroNum <= 1 && _VecZeroNum != -1)
			{
				_VecZeroNum++;
			}
			else if (_VecZeroNum == -1)
			{
				_playerRandomAnimator = true;
			}

			if (_VecZeroNum == 1)
			{
				AnimationPlay(0);
				_playerRandomAnimator = true;
			}
			
			if (isMoving)
			{
				isMoving = false;
				
//				MainCityUIRB.IsCanClickButtons = true;
//				UIYindao.m_UIYindao.setOpenUIEff();
			}
		}
		//character control vertical
		if (!m_character.isGrounded)
		{
			m_moveDir.y -= m_speedY;
		}
		m_PlayerPostion = m_playerObj.transform.position;
		string value = m_playerObj.transform.position.x.ToString() + ":" + m_playerObj.transform.position.y.ToString() + ":" + m_playerObj.transform.position.z.ToString();
		//if (_IsSetPos)
		{
//			PlayerPrefs.SetString("IsCurrentJunZhuPos", value);
		}
	}

	public void AnimationPlay(int index)
	{
		switch (index)
		{
		case 0:
		{
			_playerRandomAnimator = true;
			m_animator.SetTrigger("iniDle");
		}
			break;
		case 1:
		{
			_playerRandomAnimator = false;
			playerType = AnimationType.INIDLE;
			_VecZeroNum = 0;
			m_animator.SetTrigger("inRun");
		}
			break;
		case 2:
		{
			m_animator.SetTrigger("inRelax_1");
		}
			break;
		case 3:
		{
			m_animator.SetTrigger("inRelax_2");
		}
			break;
		default:
			break;
		}
	}

	#endregion

	#region Self Navigation

	private Vector3 savePos = Vector3.zero;
	private bool inTurning;

	private bool isNavMesh = false;
	private bool isMoving = false;

	private Vector3 selfTargetPos = new Vector3 ();

	public void SelfNavigation(Vector3 tempPosition) //自动导航
	{
		isNavMesh = true;
		m_agent.enabled = true;
		PlayerSelfNameManagerment.AutoNav();
		selfTargetPos = tempPosition;
		
		m_agent.enabled = true;
		
		//move character
		if (!isMoving)
		{
			//isMoving = true;
		}
		
		if (!inTurning && Vector3.Distance (selfTargetPos, savePos) > 0)
		{
			savePos = selfTargetPos;
			StartCoroutine (TurnToDestination (tempPosition));
		}
	}

	public void StopPlayerNavigation() //停止自动导航
	{
		PlayerSelfNameManagerment.DestroyAutoNav();

		savePos = Vector3.zero;
		m_fPDistance = 0f;

		if (!CityGlobalData.m_selfNavigation) return;
		
		CityGlobalData.m_selfNavigation = false;
		
		_isArrived = false;
		//stay character, disable right buttom btns.
//		m_animator.SetBool("inRun", false);
		AnimationPlay(0);

		m_agent.Stop();
		isNavMesh = false;

		m_character.enabled = true;
		m_agent.enabled = false;
//		m_agent.enabled = true;

//		if (m_showLayer != null)
//		{
//			m_showLayer = null;
//		}
	}
	
	public void StopPlayerNavigationMove() //停止自动导航但不停止动作
	{
		savePos = Vector3.zero;
		m_fPDistance = 0f;
		PlayerSelfNameManagerment.DestroyAutoNav();

		if (!CityGlobalData.m_selfNavigation) return;
		CityGlobalData.m_selfNavigation = false;

		m_agent.Stop();
		isNavMesh = false;

		m_character.enabled = true;
		m_agent.enabled = false;
		
		m_agent.enabled = true;
//		if (m_showLayer != null)
//		{
//			m_showLayer = null;
//		}
	}

	IEnumerator TurnToDestination (Vector3 targetPosition)
	{
		m_agent.Stop();
		m_agent.acceleration = 10000;
		inTurning = true;
		for (; inTurning == true;)
		{
			Vector3 oldangle = m_playerObj.transform.eulerAngles;
			
			m_playerObj.transform.forward = targetPosition - m_playerObj.transform.position;
			
			float tar = m_playerObj.transform.eulerAngles.y;
			
			//float sp = 120 * Time.deltaTime; 
			
			float sp = 1080 * Time.deltaTime;
			
			float angle = Mathf.MoveTowardsAngle(oldangle.y, tar, sp);
			
			m_playerObj.transform.eulerAngles = new Vector3(0, angle, 0);
			
			if (Mathf.Abs(tar - oldangle.y) < 20)
			{
				break;
			}
			
			yield return new WaitForEndOfFrame();
			
		}
		
		yield return new WaitForSeconds(0.01f);

		inTurning = false;
//		m_animator.SetBool("inRun", true);

		AnimationPlay(1);

		m_agent.speed = m_speed;
		m_agent.Resume();
		m_agent.SetDestination(targetPosition);
		
		m_currenPos = Vector3.zero;
		
		CityGlobalData.m_selfNavigation = true;
		
		if (Vector3.Distance(targetPosition, m_playerObj.transform.position) < 0.1f)
		{
			m_fPDistance = 10;
		}
	}

	#endregion

	#region Move Turn To Destination

	private GameObject camObject;
	private GameObject offObject;
	private GameObject angleObject;

	void MoveTurnToDestination (Vector3 targetPosition)
	{
		m_agent.enabled = true;
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
		
		Vector3 oldAngle = angleObject.transform.eulerAngles;
		
		angleObject.transform.forward = targetPosition;
		
		float tar = angleObject.transform.eulerAngles.y;
		float sp = 1080 * Time.deltaTime;
		float angle = Mathf.MoveTowardsAngle(oldAngle.y, tar, sp);
		
		angleObject.transform.eulerAngles = new Vector3(0, angle, 0);
		
		{
			Vector3 sp2 = targetPosition * m_speed * Time.deltaTime;
			
			m_transform.forward = sp2;
			
			m_character.Move(sp2);
		}
	}

	#endregion

	#region Update Player Pos

	void UpdatePlayerPosition ()
	{
		if (m_currenPos != m_targetPos) //玩家有位移 发送数据
		{
			SpriteMove tempPositon = new SpriteMove();

			tempPositon.posX = m_targetPos.x;
			tempPositon.posY = m_targetPos.y;
			tempPositon.posZ = m_targetPos.z;

			QXComData.SendQxProtoMessage (tempPositon,ProtoIndexes.Sprite_Move);

			m_currenPos = m_targetPos;
		}
	}

	#endregion

	void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
