using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityPlayer : GeneralInstance<TreasureCityPlayer> {

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

	public GameObject m_tCityPlayerObj;

	new void Awake ()
	{
		base.Awake ();
	}

	void Start()
	{

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

		InitWithGlobalData ();
	}

	void InitWithGlobalData()
	{
		//		Debug.Log ("PlayerModelCol");
		LimitActivityData.Instance.RequestData();//request limit activity info.
		
		MiBaoGlobleData.Instance();
		
		GetAllianceData.Instance();
		//CheckXml.Instance();

		BagData.Instance();
		
		EquipsOfBody.Instance();
		
		SettingData.Instance();
		
		BlockedData.Instance();
		
		EquipSuoData.Instance();
		
		NewEmailData.Instance().LoadEmailPrefab();
		
		QXChatData.Instance.LoadChatPrefab ();
		
		TreasureCityData.Instance ();

		QXSelectRole.Instance ();
	}

	#region CreatePlayer Model

	public void CreatePlayerModel (Object p_object)
	{
		m_playerObj = Instantiate(p_object) as GameObject;
		m_playerObj.SetActive(true);

		Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
		                        ResourceLoadModelCallback);
	}

	public void ResourceLoadModelCallback (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject obj = Instantiate (p_object) as GameObject;
		obj.transform.parent = m_playerObj.transform;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;

		Vector3 pos = new Vector3 (m_tCityPlayerObj.transform.position.x,
		                           				   m_tCityPlayerObj.transform.position.y,
		                          			 	   m_tCityPlayerObj.transform.position.z);
		
//		Debug.Log ("pos:" + pos);
		m_playerObj.transform.position = pos;
		m_playerObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
		m_playerObj.transform.localScale = Vector3.one * 1.5f;
		
		m_character = m_playerObj.GetComponent<CharacterController>();
		m_animator = obj.GetComponent<Animator>();
		
		m_agent = m_playerObj.GetComponent<NavMeshAgent>();
		m_agent.enabled = true;
		
		m_transform = m_playerObj.transform;
		m_currenPos = m_transform.position;
		//		Debug.Log ("m_currenPos:" + m_currenPos);
		m_mainCamera.GetComponent<CameraRun>().target = m_playerObj.transform;
		m_mainCamera.GetComponent<CameraRun>().setCameraPos();
		
		//		m_playerObj.AddComponent<TCityPlayerTrigger> ();
		//		TCityPlayerAuto playerAuto = m_playerObj.AddComponent<TCityPlayerAuto> ();
		//		GameObject autoObj = m_playerObj.GetComponentInChildren<SkinnedMeshRenderer> ().gameObject;
		//		playerAuto.AddAutoObj (autoObj);
		
		SendPlayerData();
		
		StartCoroutine (ManualStart ());
	}

	void SendPlayerData()
	{
//		EnterScene tempEnterScene = new EnterScene();
//		
//		tempEnterScene.senderName = SystemInfo.deviceName;
//		tempEnterScene.uid = 0;
//		
//		tempEnterScene.posX = m_transform.position.x;
//		tempEnterScene.posY = m_transform.position.y;
//		tempEnterScene.posZ = m_transform.position.z;
//		
//		QXComData.SendQxProtoMessage (tempEnterScene,ProtoIndexes.Enter_TBBXScene);
//		Debug.Log("I_EnterScene :" + ProtoIndexes.Enter_TBBXScene);
		{
			//  Debug.Log( "--- Scene Tag --- PlayerModelController.SendPlayerData --- State_LEAGUEOFCITY" );
			
			PlayerState t_state = new PlayerState();
			
			t_state.s_state = State.State_LEAGUEOFCITY;

			QXComData.SendQxProtoMessage (t_state,ProtoIndexes.PLAYER_STATE_REPORT,null);
		}
		
//		Debug.Log("SendPlayerData. m_ObjHero.transform.position :" + m_playerObj.transform.position);
	}

	#endregion

	#region Update Player Info

	private float m_timeInterval = 0.0f;
	private float _time_StayAnimator = 0.0f;
	private float m_fPDistance = 0f;
	private bool _playerRandomAnimator = false;

	private Vector3 m_PlayerPostion = new Vector3(0, 0.4f, 0);
	
	private bool _PlaySet = false;

	#endregion

	#region Self Navigation
	private bool inTurning;

	private Vector3 savePos = Vector3.zero;
	private Vector3 selfTargetPos = new Vector3 ();

	public void SelfNavigation(Vector3 tempPosition) //自动导航
	{
		selfTargetPos = tempPosition;

		m_agent.enabled = true;

		PlayerSelfNameManagerment.AutoNav();
		
		if (!inTurning && Vector3.Distance (selfTargetPos, savePos) > 0)
		{
			savePos = selfTargetPos;
			StartCoroutine (TurnToDestination (tempPosition));
			Debug.Log ("SelfNavigation");
		}
	}

	public void StopPlayerNavigation() //停止自动导航
	{
		PlayerSelfNameManagerment.DestroyAutoNav();

		savePos = Vector3.zero;
		m_fPDistance = 0f;

		if (!CityGlobalData.m_selfNavigation) return;
		
		CityGlobalData.m_selfNavigation = false;
		
		isArrived = false;

		m_agent.Stop();
		m_agent.enabled = false;

		m_character.enabled = true;

		Debug.Log ("stop all");
	}
	
	public void StopPlayerNavigationMove() //停止自动导航但不停止动作
	{
		savePos = Vector3.zero;
		m_fPDistance = 0f;
		PlayerSelfNameManagerment.DestroyAutoNav();

		if (!CityGlobalData.m_selfNavigation) return;
		CityGlobalData.m_selfNavigation = false;

		m_agent.Stop();
		m_agent.enabled = false;

		m_character.enabled = true;
		Debug.Log ("stop");
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

		PlayerAnimation (AnimatorState.RUN);

		m_agent.speed = m_speed;
		m_agent.Resume();
		m_agent.SetDestination(targetPosition);
//		m_playerObj.transform.position = Vector3.Lerp (m_playerObj.transform.position,targetPosition,Time.deltaTime * 2);

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

			QXComData.SendQxProtoMessage (tempPositon,ProtoIndexes.Sprite_Move,null);

			m_currenPos = m_targetPos;
		}
	}

	#endregion

	#region Update Player Info
	public enum AnimatorState
	{
		NONE,
		IDLE,
		RUN,
		RELAX1,
		RELAX2,
	}
	private AnimatorState animatorState = AnimatorState.IDLE;

	private AnimatorState curAnimatorState;

	private AnimatorState moveType = AnimatorState.NONE;

	private Dictionary<AnimatorState,string> animatorDic = new Dictionary<AnimatorState, string>()
	{
		{AnimatorState.NONE,""},
		{AnimatorState.IDLE,"zhuchengidle"},
		{AnimatorState.RUN,"zhuchengrun"},
		{AnimatorState.RELAX1,"zhuchengrelax_1"},
		{AnimatorState.RELAX2,"zhuchengrelax_2"},
	};

	private bool randomAnimatorState = false;
	private bool timeContinue = false;
	private float stayTime = 0;
	private float refreshPosTime = 0;

	private bool isArrived = false;
	private Vector3 m_moveDir;//移动方向向量
	private bool isMoving = false;//是否在移动

	private bool m_isSendPos = true;//发送位置
	public bool M_isSendPos { set{m_isSendPos = value;} get{return m_isSendPos;} }

	void Update ()
	{
		if (m_playerObj == null || joyStick == null) return;
		
		if (randomAnimatorState && !timeContinue)
		{
//			Debug.Log ("randomAnimatorState" + randomAnimatorState);
//			Debug.Log ("timeContinue" + timeContinue);
			stayTime += Time.deltaTime;
			
			if (stayTime >= 5.0f)
			{
				timeContinue = true;
				curAnimatorState = animatorState;

				switch (animatorState)
				{
				case AnimatorState.IDLE:

					PlayerAnimation (AnimatorState.RELAX1);

					break;
				case AnimatorState.RELAX2:
					
					PlayerAnimation (AnimatorState.RELAX2);
					
					break;
				default:
					break;
				}

				animatorState = AnimatorState.RELAX1;
				stayTime = 0;
			}
		}
		else if (stayTime > 0)
		{
			stayTime = 0;
		}

		#region If Play Complete
		if ((IsPlayComplete (animatorDic[AnimatorState.RELAX1]) || IsPlayComplete (animatorDic[AnimatorState.RELAX2])) && animatorState == AnimatorState.RELAX1)
		{
			timeContinue = false;
			moveType = AnimatorState.IDLE;

			animatorState = curAnimatorState == AnimatorState.IDLE ? AnimatorState.RELAX2 : AnimatorState.IDLE;
		}
		#endregion

		#region Update Audio Listener Pos
		if (m_playerObj != null)
		{
			ClientMain.m_ClientMainObj.transform.position = new Vector3 (m_playerObj.transform.position.x, m_playerObj.transform.position.y, m_playerObj.transform.position.z - 1);
		}
		#endregion

		#region Refresh Player Pos
		refreshPosTime += Time.deltaTime;
		//0.24秒刷新一次玩家位置  提交数据        
		if (refreshPosTime >= 0.05f && m_playerObj != null && M_isSendPos)
		{
			refreshPosTime = 0.0f;
			m_targetPos = m_transform.position;
			
			UpdatePlayerPosition ();
		}
		#endregion

		//Navigation End Execute.
		m_moveDir = joyStick.m_uiOffset.normalized;
		
		if (CityGlobalData.m_selfNavigation)
		{
//			Debug.Log ("m_agent.remainingDistance:" + m_agent.remainingDistance);
			if (m_agent.remainingDistance <= 2.1f && m_fPDistance != 0f)
			{
				if (!isArrived)
				{
					isArrived = true ;
					//宝箱按钮变亮
					TreasureCityUI.m_instance.BottomUI (true);
//					Debug.Log ("Arrived!");
				}

				StopPlayerNavigation();
//				Debug.Log ("Arrived2!");
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
			
			if (randomAnimatorState)
			{
				PlayerAnimation (AnimatorState.RUN);
			}
			
			if (CityGlobalData.m_selfNavigation)
			{
				StopPlayerNavigationMove ();
			}
			
			if (!isMoving)
			{
				isMoving = true;//正在移动
			}
		}
		else
		{
			if (moveType == AnimatorState.NONE)
			{
				randomAnimatorState = true;
			}
			else if (moveType == AnimatorState.IDLE)
			{
				moveType = AnimatorState.RUN;
				PlayerAnimation (AnimatorState.IDLE);
				randomAnimatorState = true;
			}

			if (isMoving)
			{
				isMoving = false;//停止移动
			}
		}
		//character control vertical
		if (!m_character.isGrounded)
		{
			m_moveDir.y -= m_speedY;
		}
		m_PlayerPostion = m_playerObj.transform.position;
//		string value = m_playerObj.transform.position.x.ToString() + ":" + m_playerObj.transform.position.y.ToString() + ":" + m_playerObj.transform.position.z.ToString();
//		if (_IsSetPos)
//		{
//			PlayerPrefs.SetString("IsCurrentJunZhuPos", value);
//		}
	}

	/// <summary>
	/// Determines whether this instance is play complete the specified tempName.
	/// </summary>
	/// <returns><c>true</c> if this instance is play complete the specified tempName; otherwise, <c>false</c>.</returns>
	/// <param name="tempName">Temp name.</param>
	public bool IsPlayComplete (string tempName)
	{
		if (m_animator == null) return false;
		
		AnimatorClipInfo[] t_states = m_animator.GetCurrentAnimatorClipInfo(0);
		AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);

		float playing = Mathf.Clamp01 (info.normalizedTime);
		
		for (int i = 0; i < t_states.Length; /*i++*/ )
		{
			AnimatorClipInfo t_item = t_states[i];
			
			if (t_item.clip.name.Equals(tempName) && playing >= 1.0f)
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

	/// <summary>
	/// Players the animation.
	/// </summary>
	/// <param name="tempState">Temp state.</param>
	void PlayerAnimation (AnimatorState tempState)
	{
//		m_animator.SetTrigger (animatorDic[tempState]);
		switch (tempState)
		{
		case AnimatorState.IDLE:

			randomAnimatorState = true;

			break;
		case AnimatorState.RUN:

			timeContinue = false;
			randomAnimatorState = false;
			animatorState = AnimatorState.IDLE;
			moveType = AnimatorState.IDLE;

			break;
		default:
			break;
		}

		m_animator.Play (animatorDic[tempState]);
	}
	#endregion

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
