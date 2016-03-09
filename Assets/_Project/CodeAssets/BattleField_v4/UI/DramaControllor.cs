using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DramaControllor : MonoBehaviour
{
	public delegate void Callback ();


	public UILabel label;

	public UILabel labelName;

	public UISprite spriteBoard;

	public UITexture avatar;

	public GameObject bg;

	public GameObject guideDrama;

	public KingCamera gameCamera;

	public GameObject spriteVedioBG;

	public GameObject spriteTemple;

	public GameObject labelTemple;

	public UIAnchor anchorTop;
	
	public UIAnchor anchorTopLeft;
	
	public UIAnchor anchorTopRight;
	
	public UIAnchor anchorLeft;
	
	public UIAnchor anchorRight;
	
	public UIAnchor anchorBottom;
	
	public UIAnchor anchorBottomLeft;
	
	public UIAnchor anchorBottomRight;
	
	public UIAnchor anchorCenter;


	[HideInInspector] public bool stopAction;


	private int yindaoId = 0;

	private static DramaControllor _instance;

	private DramaStoryControllor storyControllor;
	
	private GameObject selfRoot;
	
	private GameObject enemyRoot;

	private Callback mCallback;

	private int tempEventId;

	private int tempLevel;

	private bool inGuide;

	private bool inDrama;

	private GameObject gc_moving;

	private Vector3 movingTargetPos;

	private Vector3 movingStartPos;

	private float movingCount;

	private GameObject uiGc;

	private bool tempAutoFight = false;

	private List<int> nextPlayList = new List<int>();

	private List<Callback> nextCallbackList = new List<Callback>();

	private int m_levelCallback;

	private int m_eventIdCallback;

	private Callback m_callbackCallback;

	private GuideTemplate m_templateCallback;


	void Awake()
	{
//		Debug.Log( "DramaControllor.Awake()" );

		_instance = this;
	}

	public static DramaControllor Instance() { return _instance; }

	void OnDestroy(){
		_instance = null;
	}

	void init(GuideTemplate template)
	{
		if(template.actionType == 3)
		{
			if(template.pause == 1)
			{
				GameObject Gobj = GameObject.Find ("JoyStick");
				
				if(Gobj != null) Gobj.SendMessage ("release");
			}
		}
		else if(template.actionType != 2 && template.actionType != 4 && template.actionType != 5 && template.actionType != 6 && template.actionType != 7 && template.actionType != 8)
		{
			GameObject Gobj = GameObject.Find ("JoyStick");
			
			if(Gobj != null) Gobj.SendMessage ("release");
		}

		gameCamera = (KingCamera)GameObject.Find ("Main Camera").GetComponent ("KingCamera");

		selfRoot = GameObject.Find ("Self");

		enemyRoot = GameObject.Find ("Enemy");

		tempAutoFight = BattleControlor.Instance().autoFight || tempAutoFight;

		bg.SetActive (true);

		if (storyControllor == null)
		{
			storyControllor = (DramaStoryControllor)DramaStoryReador.Instance().GetComponentInChildren (typeof(DramaStoryControllor));
			
			storyControllor.init ();
		}
	}

	public void showEventDelay()
	{
		showEvent (m_levelCallback, m_eventIdCallback, m_callbackCallback);
	}

	public void showEvent(int level, int eventId, Callback _callBack)
	{
		if (inDrama == true) {
			//nextPlayList.Add(eventId);

			//nextCallbackList.Add(_callBack);

			return;
		}

		GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (level, eventId);

		bool played = BattleControlor.Instance().havePlayedGuide (template);

		if (played == true) 
		{
			init (template);

			mCallback = _callBack;

			dramaOver ();

			return;
		}

		BattleControlor.Instance().guidePlayed.Add (template);

		m_levelCallback = level;

		m_eventIdCallback = eventId;

		m_callbackCallback = _callBack;

		m_templateCallback = template;

		if (template.delay > 0 && template.triggerType != 4) 
		{
			//yield return new WaitForSeconds (template.delay);

			BattleControlor.Instance().clockWithCallback (template.delay, showEventCallback);
		}
		else
		{
			showEventCallback();
		}

	}

	private void showEventCallback()
	{
		int level = m_levelCallback;

		int eventId = m_eventIdCallback;

		Callback _callBack = m_callbackCallback;

		GuideTemplate template = m_templateCallback;

		if(template.actionType == 2)
		{
			tempLevel = level;
			
			tempEventId = eventId;
			
			mCallback = _callBack;

			playmusic(template.ap1);

			dramaOver(false);

			return;
		}

		gameObject.SetActive (true);

		bg.SetActive (false);

		GetComponent<BoxCollider> ().enabled = false;

		//StartCoroutine (showEventAction(template, level, eventId, _callBack));

		showEventAction (template, level, eventId, _callBack);
	}

	void showEventAction(GuideTemplate template, int level, int eventId, Callback _callBack)
	{
//		if(template.delay > 0)
//		{
//			yield return new WaitForSeconds (template.delay);
//		}

		GetComponent<BoxCollider> ().enabled = true;

		init (template);

		inGuide = false;

		inDrama = true;

		stopAction = false;

		tempLevel = level;

		tempEventId = eventId;

		mCallback = _callBack;

		bool returnFlag = true;

		if(template.triggerType == 6)
		{
			dramaOver();

			returnFlag = false;
		}
		else if(template.triggerType == 7)
		{
			dramaOver();
			
			returnFlag = false;
		}
		else if(template.actionType == 0)//对话
		{
			showDrama(template);

			stopAction = true;
		}
		else if(template.actionType == 1)//小电影
		{
			stopAction = true;

			foreach(BaseAI node in BattleControlor.Instance().selfNodes)
			{
				node.setNavMeshStop();

				node.targetNode = null;

				if(node.flag.hideInDrama == true)
				{
					node.setLayer(2);
				}
			}

			foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
			{
				node.setNavMeshStop();
				
				node.targetNode = null;

				if(node.flag.hideInDrama == true)
				{
					node.setLayer(2);
				}
			}

			foreach(BaseAI node in BattleControlor.Instance().midNodes)
			{
				node.setNavMeshStop();
				
				node.targetNode = null;
				
				if(node.flag.hideInDrama == true)
				{
					node.setLayer(2);
				}
			}

			if (storyControllor == null)
			{
				storyControllor = (DramaStoryControllor)DramaStoryReador.Instance().GetComponentInChildren (typeof(DramaStoryControllor));
				
				storyControllor.init ();
			}

			showStoryBoard(template.ap1);
		}
		else if(template.actionType == 2)//背景音乐
		{
			playmusic(template.ap1);

			dramaOver(false);

			returnFlag = false;
		}
		else if(template.actionType == 3)//气泡引导
		{
			bool yindaoable = false;

			if(template.ap2 == 0)//无限制
			{
				yindaoable = true;
			}
			else if(template.ap2 == 1)//是重武器
			{
				if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
				{
					yindaoable = true;
				}
			}
			else if(template.ap2 == 2)//是轻武器
			{
				if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
				{
					yindaoable = true;
				}
			}
			else if(template.ap2 == 3)//是弓武器
			{
				if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
				{
					yindaoable = true;
				}
			}
			else if(template.ap2 == -1)//不是重武器
			{
				if(BattleControlor.Instance().getKing().weaponType != KingControllor.WeaponType.W_Heavy)
				{
					yindaoable = true;
				}
			}
			else if(template.ap2 == -2)//不是轻武器
			{
				if(BattleControlor.Instance().getKing().weaponType != KingControllor.WeaponType.W_Light)
				{
					yindaoable = true;
				}
			}
			else if(template.ap2 == -3)//不是弓武器
			{
				if(BattleControlor.Instance().getKing().weaponType != KingControllor.WeaponType.W_Ranged)
				{
					yindaoable = true;
				}
			}

			if(yindaoable == true)
			{
				yindaoId = int.Parse(template.ap3);

				UIYindao.m_UIYindao.setOpenYindao(template.ap1);
			}

			dramaOver (yindaoable == true && template.pause == 1);

			returnFlag = yindaoable == true && template.pause == 1;

			if(yindaoable == true && template.pause == 1)
			{
				SceneGuideManager.Instance().OnSceneTipsHide();

				Time.timeScale = 0;
			}
		}
		else if(template.actionType == 4)//解锁界面功能
		{
			showGuide(template);

			returnFlag = false;
		}
		else if(template.actionType == 5)//清除技能CD
		{
			refreshCD(template);

			dramaOver ();

			returnFlag = false;
		}
		else if(template.actionType == 6)//音效
		{
			playSound(template.ap1);
			
			dramaOver(false);
			
			returnFlag = false;
		}
		else if(template.actionType == 7)//横幅提示
		{
			SceneGuideManager.Instance().ShowSceneGuide (template.ap1);

			dramaOver(false);

			returnFlag = false;
		}
		else if(template.actionType == 8)//语音
		{
			playVoice(template.ap1);
			
			dramaOver(false);
			
			returnFlag = false;
		}
		else if(template.actionType == 9)//消除目标
		{
			GameObject t_node = getNodeByName(template.ap3);

			if(t_node != null)
			{
				t_node.SetActive(false);
			}

			dramaOver(false);
			
			returnFlag = false;
		}
		else if(template.actionType == 10)//播放视频
		{
			playVideo(template.ap1);
		}
		else if(template.actionType == 11)//强制死亡
		{
			nodeDie(template.ap1);

			dramaOver(false);

			returnFlag = false;
		}

		if(returnFlag == true)
		{
			BattleUIControlor.Instance().attackJoystick.reset();
		}

		if(returnFlag == true && BattleControlor.Instance().autoFight == true)
		{
			BattleUIControlor.Instance().changeAutoFight();
		}
	}

	private void showStoryBoard(int index)
	{
		BubblePopControllor.Instance().closeAllBubble ();

		UIYindao.m_UIYindao.CloseUI ();

		UI3DEffectTool.ClearUIFx(BattleUIControlor.Instance().autoFight_1);

		inGuide = true;

		bg.SetActive (false);

		guideDrama.SetActive(false);

		BattleUIControlor.Instance().layerFight.SetActive (false);

		storyControllor.init ();


		storyControllor.recreateModel (index);

		//toryControllor.playNext (gameObject, index);

		storyControllor.playAction (gameObject, index);

		BattleControlor.Instance().inDrama = true;
	}

	private void playmusic(int musicId)
	{
		ClientMain.m_sound_manager.chagneBGSound (musicId);
	}

	private void playSound(int soundId)
	{
		SoundPlayEff spe = BattleControlor.Instance().getKing().GetComponent<SoundPlayEff>();

		if(spe != null)
		{
			spe.PlaySound(soundId + "");
		}
	}

	private void playVoice(int soundId)
	{
		ClientMain.m_ClientMain.m_SoundPlayEff.PlaySound (soundId + "");
	}

	private void playVideo(int videoId)
	{
		EffectIdTemplate template = EffectIdTemplate.getEffectTemplateByEffectId (videoId);

		VideoHelper.PlayDramaVideo (template.path, onPressInDrama);

		spriteVedioBG.SetActive (true);
	}

	private void nodeDie(int nodeId)
	{
		BaseAI node = BattleControlor.Instance().getNodebyId (nodeId);
		
		if (node == null) return;

		node._dieActionDone (true);
	}

	public void dramaStoryDone()
	{
		foreach(BaseAI node in BattleControlor.Instance().selfNodes)
		{
			node.setLayer(8);//3D Layer
		}

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			node.setLayer(8);//3D Layer
		}

		foreach(BaseAI node in BattleControlor.Instance().midNodes)
		{
			node.setLayer(8);//3D Layer
		}

		onPressInDrama ();
	}

	private void showDrama(GuideTemplate template)//对话
	{
		BubblePopControllor.Instance().closeAllBubble ();

		bg.SetActive (false);

		guideDrama.SetActive(false);
		
		BattleUIControlor.Instance().layerFight.SetActive (false);

//		if (template.para2 == 1)
//		{
//			GameObject focus = getNodeByName(chatTemplate.cameraTarget);
//
//			if(focus != null)
//			{
//				StartCoroutine(hideActor(focus));
//
//				//focus.SetActive(false);
//			}
//
//			DialogCallback();
//
//			return;
//		}

		UIYindao.m_UIYindao.CloseUI ();

		UI3DEffectTool.ClearUIFx(BattleUIControlor.Instance().autoFight_1);

		List<DialogData.dialogData> dialoglistdata = new List<DialogData.dialogData>();

		PlotChatTemplate chatTemplate = PlotChatTemplate.getPlotChatTemplateById (template.ap1);

		DialogData.dialogData dialogdata = new DialogData.dialogData();
		
		string text = chatTemplate.pText;
		
		string textName = chatTemplate.pName;

//		Debug.Log( "BattleControlor.Instance(): " + BattleControlor.Instance() );

//		Debug.Log( "BattleControlor.Instance().getKing(): " + BattleControlor.Instance().getKing() );

		text = text.Replace ("xxxxx", BattleControlor.Instance().getKing().nodeData.nodeName);

		textName = textName.Replace ("xxxxx", BattleControlor.Instance().getKing().nodeData.nodeName);

		label.text = text;

		labelName.text = textName;

		dialogdata.iHeadID = chatTemplate.icon;

		if(dialogdata.iHeadID == 0)//第0关缺少君主信息，将1003翻译为102
		{
			dialogdata.iHeadID = BattleControlor.Instance().getKing().modelId - 901;
		}

		dialogdata.sDialogData = text;

		dialogdata.sName = textName;

		dialogdata.isLeft = (chatTemplate.position == 0);

		dialoglistdata.Add(dialogdata);

		float autoTime = 99999999;

		if(tempAutoFight == true) autoTime = 10;

		ClientMain.m_ClientMain.m_UIDialogSystem.setOpenDialog(dialoglistdata, autoTime, DialogCallback);

		if(chatTemplate.cameraTarget.Equals("") == false)
		{
			GameObject focus = getNodeByName(chatTemplate.cameraTarget);

			if(focus != null)
			{
				if(chatTemplate.forwardFlagId != 0)
				{
					BaseAI forwardNode = BattleControlor.Instance().getNodebyId(chatTemplate.forwardFlagId);

					if(forwardNode != null)
					{
						focus.transform.forward = forwardNode.transform.position - focus.transform.position;
					}
					else
					{
						BattleFlag bf = null;

						BattleControlor.Instance().flags.TryGetValue(chatTemplate.forwardFlagId, out bf);

						if(bf != null)
						{
							focus.transform.forward = bf.transform.position - focus.transform.position;
						}
					}
				}

				Vector3 cameraPosition = new Vector3(chatTemplate.cameraPx, chatTemplate.cameraPy, chatTemplate.cameraPz);

				Vector3 cameraRotation = new Vector3(chatTemplate.cameraRx, chatTemplate.cameraRy, 0);

				if(chatTemplate.isLocal == true)
				{
					Vector3 tempFocusFowrad = focus.transform.forward;

					GameObject tempObject = new GameObject();
					
					tempObject.transform.parent = focus.transform;

					tempObject.transform.localScale = new Vector3(1, 1, 1);
	
					tempObject.transform.localPosition = cameraPosition;
	
					tempObject.transform.localEulerAngles = cameraRotation;

					tempObject.transform.parent = focus.transform.parent;

					focus.transform.eulerAngles = Vector3.zero;

					tempObject.transform.parent = focus.transform;

					focus.transform.forward = tempFocusFowrad;

					//tempObject.transform.parent = focus.transform.parent;

					cameraPosition = tempObject.transform.localPosition;

					cameraRotation = tempObject.transform.localEulerAngles;

					Destroy(tempObject);
				}

//				GameObject tempObject = new GameObject();
//
//				tempObject.transform.parent = gameCamera.transform.parent;
//
//				tempObject.transform.localScale = new Vector3(1, 1, 1);
//
//				tempObject.transform.position = Vector3.zero;
//
//				tempObject.transform.eulerAngles = focus.transform.localEulerAngles;
//
//				GameObject tempInner = new GameObject();
//
//				tempInner.transform.parent = tempObject.transform;
//
//				tempInner.transform.localScale = new Vector3(1, 1, 1);
//
//				tempInner.transform.localPosition = new Vector3(chatTemplate.cameraPx, chatTemplate.cameraPy, chatTemplate.cameraPz);
//
//				tempInner.transform.localEulerAngles = new Vector3(chatTemplate.cameraRx, chatTemplate.cameraRy, 0); 
//
//				tempInner.transform.parent = tempObject.transform.parent;

				gameCamera.targetChang(focus);

				gameCamera.CameraChange(cameraPosition, cameraRotation);

//				Destroy(tempObject);
//
//				Destroy(tempInner);
			}
			else
			{
				bg.SetActive(true);
			}
		}
		else
		{
			gameCamera.resetCamera();
		}

		if(template.pause == 1) 
		{
			SceneGuideManager.Instance().OnSceneTipsHide();

			Time.timeScale = 0;
		}

		BattleControlor.Instance().inDrama = true;
	}

	private void refreshCD(GuideTemplate template)//清除CD
	{
		if(template.ap1 == 11)
		{
			BattleUIControlor.Instance().cooldownHeavySkill_1.CoolDownComplate();
		}
		else if(template.ap1 == 12)
		{
			BattleUIControlor.Instance().cooldownHeavySkill_2.CoolDownComplate();
		}
		else if(template.ap1 == 21)
		{
			BattleUIControlor.Instance().cooldownLightSkill_1.CoolDownComplate();
		}
		else if(template.ap1 == 22)
		{
			BattleUIControlor.Instance().cooldownLightSkill_2.CoolDownComplate();
		}
		else if(template.ap1 == 31)
		{
			BattleUIControlor.Instance().cooldownRangeSkill_1.CoolDownComplate();
		}
		else if(template.ap1 == 32)
		{
			BattleUIControlor.Instance().cooldownRangeSkill_2.CoolDownComplate();
		}
		else if(template.ap1 == 41)
		{
			BattleUIControlor.Instance().cooldownMibaoSkill.CoolDownComplate();
		}
	}

	private void showGuide(GuideTemplate template)//解锁界面功能
	{
		//inGuide = true;

		//BattleControlor.Instance().inDrama = true;

		bg.SetActive (false);

		guideDrama.SetActive(false);

		GameObject gc = null;

		GameObject uigc = null;

		Vector3 offset = Vector3.zero;

		LockControllor.LOCK_TYPE lockType = LockControllor.LOCK_TYPE.Attack;

		if (template.ap1 == 1)
		{
			uigc = BattleUIControlor.Instance().m_gc_move;

			BattleUIControlor.Instance().b_joystick = true;

			//offset = BattleUIControlor.Instance().anchorBottomLeft.transform.localPosition;
		}
		else if(template.ap1 == 2)
		{
			uigc = BattleUIControlor.Instance().m_gc_attack;

			lockType = LockControllor.LOCK_TYPE.Attack;

			BattleUIControlor.Instance().b_attack = true;

			//offset = BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
		}
		else if(template.ap1 == 3)
		{
			if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_1[0];

				lockType = LockControllor.LOCK_TYPE.HeavySkill_1;

				BattleUIControlor.Instance().b_skill_heavy_1 = true;
			}
			else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_1[2];

				lockType = LockControllor.LOCK_TYPE.LightSkill_1;

				BattleUIControlor.Instance().b_skill_light_1 = true;
			}
			else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_1[1];

				lockType = LockControllor.LOCK_TYPE.RangeSkill_1;

				BattleUIControlor.Instance().b_skill_ranged_1 = true;
			}

			offset = BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
		}
		else if(template.ap1 == 4)
		{
			if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_2[0];

				lockType = LockControllor.LOCK_TYPE.HeavySkill_2;

				BattleUIControlor.Instance().b_skill_heavy_2 = true;
			}
			else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_2[2];

				lockType = LockControllor.LOCK_TYPE.LightSkill_2;

				BattleUIControlor.Instance().b_skill_light_2 = true;
			}
			else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
			{
				uigc = BattleUIControlor.Instance().m_gc_skill_2[1];

				lockType = LockControllor.LOCK_TYPE.RangeSkill_2;

				BattleUIControlor.Instance().b_skill_ranged_2 = true;
			}

			offset = BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
		}
		else if(template.ap1 == 5)
		{
			uigc = BattleUIControlor.Instance().m_changeWeapon.btnHeavy.gameObject;

			lockType = LockControllor.LOCK_TYPE.WeaponHeavy;

			BattleUIControlor.Instance().b_weapon_heavy = true;

			offset = BattleUIControlor.Instance().anchorBottom.transform.localPosition;
		}
		else if(template.ap1 == 6)
		{
			uigc = BattleUIControlor.Instance().m_changeWeapon.btnLight.gameObject;

			lockType = LockControllor.LOCK_TYPE.WeaponLight;

			BattleUIControlor.Instance().b_weapon_light = true;

			offset = BattleUIControlor.Instance().anchorBottom.transform.localPosition;
		}
		else if(template.ap1 == 7)
		{
			uigc = BattleUIControlor.Instance().m_changeWeapon.btnRange.gameObject;

			lockType = LockControllor.LOCK_TYPE.WeaponRange;

			BattleUIControlor.Instance().b_weapon_range = true;

			offset = BattleUIControlor.Instance().anchorBottom.transform.localPosition;
		}
		else if(template.ap1 == 8)//暂停
		{
			uigc = BattleUIControlor.Instance().m_gc_pause;

			lockType = LockControllor.LOCK_TYPE.Pause;

			BattleUIControlor.Instance().b_pause = true;

			//offset = BattleUIControlor.Instance().anchorTopLeft.transform.localPosition;
		}
		else if(template.ap1 == 9)//自动
		{
			uigc = BattleUIControlor.Instance().m_gc_autoFight;

			lockType = LockControllor.LOCK_TYPE.AutoFight;

			BattleUIControlor.Instance().b_autoFight = true;

			//offset = BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
		}
		else if(template.ap1 == 10)//翻滚
		{
			uigc = BattleUIControlor.Instance().m_gc_dodge;

			lockType = LockControllor.LOCK_TYPE.Dodge;

			BattleUIControlor.Instance().b_dodge = true;

			//offset = BattleUIControlor.Instance().anchorBottomRight.transform.localPosition;
		}
		else if(template.ap1 == 11)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Heavy);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToHeavy();

			uigc = BattleUIControlor.Instance().m_gc_skill_1[0];
			
			lockType = LockControllor.LOCK_TYPE.HeavySkill_1;
			
			BattleUIControlor.Instance().b_skill_heavy_1 = true;
		}
		else if(template.ap1 == 12)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Heavy);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToHeavy();

			uigc = BattleUIControlor.Instance().m_gc_skill_2[0];
			
			lockType = LockControllor.LOCK_TYPE.HeavySkill_2;
			
			BattleUIControlor.Instance().b_skill_heavy_2 = true;
		}
		else if(template.ap1 == 13)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Light);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToLight();

			uigc = BattleUIControlor.Instance().m_gc_skill_1[2];
			
			lockType = LockControllor.LOCK_TYPE.LightSkill_1;
			
			BattleUIControlor.Instance().b_skill_light_1 = true;
		}
		else if(template.ap1 == 14)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Light);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToLight();

			uigc = BattleUIControlor.Instance().m_gc_skill_2[2];
			
			lockType = LockControllor.LOCK_TYPE.LightSkill_2;
			
			BattleUIControlor.Instance().b_skill_light_2 = true;
		}
		else if(template.ap1 == 15)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Ranged);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToRange();

			uigc = BattleUIControlor.Instance().m_gc_skill_1[1];
			
			lockType = LockControllor.LOCK_TYPE.RangeSkill_1;
			
			BattleUIControlor.Instance().b_skill_ranged_1 = true;
		}
		else if(template.ap1 == 16)
		{
			//BattleUIControlor.Instance().changeWeaponTo(KingControllor.WeaponType.W_Ranged);

			BattleUIControlor.Instance().m_changeWeapon.changeWeaponToRange();

			uigc = BattleUIControlor.Instance().m_gc_skill_2[1];
			
			lockType = LockControllor.LOCK_TYPE.RangeSkill_2;
			
			BattleUIControlor.Instance().b_skill_ranged_2 = true;
		}
		else if(template.ap1 == 17)
		{
			uigc = BattleUIControlor.Instance().btnMibaoSkill;

			lockType = LockControllor.LOCK_TYPE.MiBaoSkill;

			BattleUIControlor.Instance().b_skill_miBao = true;
		}

		if(uigc != null)
		{
			//LockControllor.Instance().refreshLock(lockType, true);

			GetComponent<BoxCollider> ().enabled = false;

			StartCoroutine(_showGuide(lockType));

			UI3DEffectTool.ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, uigc, EffectIdTemplate.GetPathByeffectId(100169));

			UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, uigc, EffectIdTemplate.GetPathByeffectId(100009));
		}
		else
		{
			dramaOver ();
		}
	}

	IEnumerator _showGuide(LockControllor.LOCK_TYPE lockType)
	{
		//yield return new WaitForSeconds (.2f);

		LockControllor.Instance().refreshLock(lockType, true);

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection <= 1 && CityGlobalData.m_tempLevel <= 1)
		{
			float x = 0;
			
			if(BattleUIControlor.Instance().m_changeWeapon.getUnlockCount() <= 1)
			{
				x = -100;
			}

			if(x == 0)
			{
				iTween.ValueTo(gameObject, iTween.Hash(
					"from", -100,
					"to", 0,
					"time", .5f,
					"onupdate", "setAnchorLeftOffsetX",
					"easetype", iTween.EaseType.linear
					));

				yield return new WaitForSeconds (2f);
			}

			//BattleUIControlor.Instance().anchorLeft.pixelOffset.x = x;
		}

		if(lockType == LockControllor.LOCK_TYPE.WeaponHeavy || lockType == LockControllor.LOCK_TYPE.WeaponLight || lockType == LockControllor.LOCK_TYPE.WeaponRange)
		{
			if(BattleUIControlor.Instance().m_changeWeapon.labelHeavy.activeSelf == true)
			{
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.btnHeavy.GetComponentInChildren<UISprite>().gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.focusHeavy.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			}

			if(BattleUIControlor.Instance().m_changeWeapon.labelLight.activeSelf == true)
			{
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.btnLight.GetComponentInChildren<UISprite>().gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.focusLight.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			}

			if(BattleUIControlor.Instance().m_changeWeapon.labelRange.activeSelf == true)
			{
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.btnRange.GetComponentInChildren<UISprite>().gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			
				SparkleEffectItem.OpenSparkle(BattleUIControlor.Instance().m_changeWeapon.focusRange.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon, 1);
			}
		}

		yield return new WaitForSeconds (1f);

		dramaOver (false);
	}

	public void setAnchorLeftOffsetX(float x)
	{
		BattleUIControlor.Instance ().m_changeWeapon.transform.localPosition += new Vector3 (-BattleUIControlor.Instance ().m_changeWeapon.transform.localPosition.x, 0, 0);
	}

	public void DialogCallback()
	{
		OnPress (true);
	}

	public void OnPress(bool pressed)
	{
		if (pressed == false) return;

		if (inGuide == true) return;

		if (m_templateCallback.actionType == 10) return;

		//if (inDrama == true) return;

		dramaOver ();
	}

	public void onPressInGuide()
	{
		dramaOver ();
	}

	public void onPressInDrama()
	{
		dramaOver ();
	}

	private void clearDelTarget()
	{
		if (m_templateCallback.delTarget.Count == 0) return;

		foreach(int delTarget in m_templateCallback.delTarget)
		{
			BaseAI delNode = BattleControlor.Instance().getNodebyId (delTarget);

			if (delNode == null) continue;

			if(delNode.stance == BaseAI.Stance.STANCE_ENEMY)
			{
				BattleControlor.Instance().enemyNodes.Remove(delNode);
			}
			else if(delNode.stance == BaseAI.Stance.STANCE_SELF)
			{
				BattleControlor.Instance().selfNodes.Remove(delNode);
			}
			else if(delNode.stance == BaseAI.Stance.STANCE_MID)
			{
				BattleControlor.Instance().midNodes.Remove(delNode);
			}

			DestroyObject (delNode.gameObject);
		}
	}

	private void dramaOver(bool _stopAction = true)
	{
		inDrama = false;

		inGuide = false;

		openEye ();

		clearDelTarget ();

		int e = tempEventId + 1;

		bool guideFlag = GuideTemplate.HaveId (tempLevel, e);
		
		Time.timeScale = 1;

		spriteVedioBG.SetActive (false);

		if(_stopAction == true)
		{
			BattleUIControlor.Instance().m_gc_attack.SendMessage ("Start");

			BattleUIControlor.Instance().m_gc_move.SendMessage ("release");

			BattleUIControlor.Instance().resetKeyBoard ();
		}

		if(guideFlag == true)
		{
			bool f = GuideTemplate.HaveId(tempLevel, e);

			if(f == true) 
			{
				GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (tempLevel, e);

				bool ff = BattleControlor.Instance().havePlayedGuide(template);

				if(ff == false && template.triggerType == 4)
				{
					if(template.delay > 0)
					{
						m_levelCallback = tempLevel;

						m_templateCallback = template;

						m_eventIdCallback = e;

						m_callbackCallback = mCallback;

						BattleControlor.Instance().clockWithCallback(template.delay, showEventDelay);
					}
					else
					{
						showEvent(tempLevel, e, mCallback);

						return;
					}
				}
			}
		}

//		if(nextPlayList.Count > 0)
//		{
//			int nextId = nextPlayList[0];
//
//			Callback callback = nextCallbackList[0];
//
//			nextPlayList.RemoveAt(0);
//			
//			nextCallbackList.RemoveAt(0);
//
//			showEvent(tempLevel, nextId, callback);
//
//			return;
//		}

		gameObject.SetActive(false);

		if(gameCamera != null)
		{
			gameCamera.resetCamera();
		
			gameCamera.updateCameraFlags();

			gameCamera.updateCamera();
		}

		BattleControlor.Instance().inDrama = false;

		if(mCallback != null) mCallback();

		if(tempAutoFight == true && _stopAction == true)
		{
			BattleUIControlor.Instance().changeAutoFight();

			tempAutoFight = false;
		}
	}

	public void closeYindao(int _yindaoId)
	{
		if(DramaControllor.Instance().yindaoId == _yindaoId)
		{
			UIYindao.m_UIYindao.CloseUI();

			DramaControllor.Instance().yindaoId = 0;

			dramaOver(false);
		}
	}

	void Update()
	{
		if (gc_moving == null) return;

		float count = 30f;

		if(movingCount >= count)
		{
			gc_moving.SetActive(false);

			uiGc.SetActive(true);

			onPressInGuide ();

			gc_moving = null;

			Time.timeScale = 1;

			return;
		}

		Vector3 pos = (movingTargetPos - movingStartPos) * movingCount / count;

		gc_moving.transform.localPosition = pos;

		movingCount += 1f;
	}

	private void openEye()
	{
		if( BattleUIControlor.Instance() == null ) return;

		if( BattleUIControlor.Instance().layerFight == null ) return;

		BattleUIControlor.Instance().layerFight.SetActive (true);

		bool have = GuideTemplate.HaveId (tempLevel, tempEventId);

		if (have == false) return;

		GuideTemplate template = GuideTemplate.getTemplateByLevelAndEvent (tempLevel, tempEventId);

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			foreach(int flagId in template.flagId)
			{
				if(node.flag.flagId == flagId)
				{
					node.nodeData.SetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange, 100 );

					SphereCollider sc = (SphereCollider)node.GetComponent("SphereCollider");
					
					if(sc != null)
					{
						sc.radius = node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_eyeRange );
					}

					return;
				}
			}
		}
	}

	public bool getInDrama()
	{
		return inGuide;
	}

	private GameObject getNodeByName(string nodeName)
	{
		if(nodeName.Equals("xxxxx"))
		{
			return BattleControlor.Instance().getKing().gameObject;
		}

		Component[] selfComps = selfRoot.GetComponentsInChildren (typeof(BaseAI));

		foreach(Component com in selfComps)
		{
			BaseAI node = (BaseAI)com;

			if(node.nodeData.nodeName.Equals(nodeName) && node.nodeId != 1)
			{
				return node.gameObject;
			}
		}

		BaseAI[] enemyComs = enemyRoot.GetComponentsInChildren<BaseAI> ();

		foreach(BaseAI node in enemyComs)
		{
			if(node != null && node.nodeData.nodeName != null && node.nodeData.nodeName.Equals(nodeName))
			{
				return node.gameObject;
			}
		}

		if (storyControllor == null) return null;

		Component[] actorComs = storyControllor.GetComponentsInChildren (typeof(DramaActorControllor));

		foreach(Component com in actorComs)
		{
			DramaActorControllor actor = (DramaActorControllor)com;

			if(actor.gameObject.activeSelf == true && actor.actorName.Equals(nodeName))
			{
				return actor.gameObject;
			}
		}

		return null;
	}

	IEnumerator hideActor(GameObject actor)
	{
		Renderer[] meshs = actor.GetComponentsInChildren<Renderer>();

		float alpha = 1;

		for(;alpha > 0;)
		{
			alpha -= .05f;

			foreach(Renderer mesh in meshs)
			{
				foreach(Material mater in mesh.materials)
				{
					if(mater.shader.name.Equals("Custom/Characters/Main Texture High Light"))
					{
						mater.color = new Color(mater.color.r, mater.color.g, mater.color.b, alpha);
					}
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public int getYindaoId()
	{
		return yindaoId;
	}

}
