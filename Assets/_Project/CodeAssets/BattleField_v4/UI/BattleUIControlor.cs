using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleUIControlor : MonoBehaviour, SocketProcessor
{
	public BattlePauseControllor pauseControllor;
	
	public DramaControllor dramaControllor;

	public BattleCenterLabelControllor centerLabelControllor;

	public UIProgressBar barSelf;

	public EnemyBar barEnemy;

	public GameObject layerAPCBar;

	public APCBar barAPC;

	public List<GameObject> layerAutoFight;
	
	public GameObject autoFight_1;
	
	public GameObject autoFight_2;
	
	public List<GameObject> attackIconList;
	
	public GameObject btnDaoSkill_1;
	
	public GameObject btnDaoSkill_2;
	
	public GameObject btnGongSkill_1;
	
	public GameObject btnGongSkill_2;
	
	public GameObject btnQiangSkill_1;
	
	public GameObject btnQiangSkill_2;
	
	public GameObject btnMibaoSkill;

	public UISprite btnMibaoSkillIcon;

	public UILabel labelTime;

	public UILabel labelLeave;

	public UILabel labelName;

	public UILabel winDesc;

	public UILabel winDescNum;

	public GameObject m_gc_move;
	
	public GameObject m_gc_attack;
	
	public List<GameObject> m_gc_skill_1;
	
	public List<GameObject> m_gc_skill_2;
	
	public BattleWeapon m_changeWeapon;
	
	public GameObject m_gc_autoFight;

	public GameObject m_gc_pause;

	public GameObject m_gc_dodge;

	public UITexture textureBlack;

	public GameObject layerFight;

	public CoolDown cooldownLightSkill_1;

	public CoolDown cooldownLightSkill_2;

	public CoolDown cooldownHeavySkill_1;

	public CoolDown cooldownHeavySkill_2;

	public CoolDown cooldownRangeSkill_1;

	public CoolDown cooldownRangeSkill_2;

	public CoolDown cooldownLightSkill_1_enemy;
	
	public CoolDown cooldownLightSkill_2_enemy;
	
	public CoolDown cooldownHeavySkill_1_enemy;
	
	public CoolDown cooldownHeavySkill_2_enemy;
	
	public CoolDown cooldownRangeSkill_1_enemy;
	
	public CoolDown cooldownRangeSkill_2_enemy;

	public CoolDown cooldownMibaoSkill;

	public CoolDown cooldownMibaoSkill_enemy;

	public UIAnchor anchorTop;

	public UIAnchor anchorTopLeft;

	public UIAnchor anchorTopRight;

	public UIAnchor anchorBottom;

	public UIAnchor anchorBottomLeft;

	public UIAnchor anchorBottomRight;

	public UISprite spriteAvatar;

	public GameObject btnDebug;

	public GameObject btnWin;

	public GameObject btnEasy;

	public GameObject btnLose;

	public UILabel labelDebug;

	public AttackJoystick attackJoystick;

	public GameObject layerAutoFightHint;

	public UISprite spriteBarMibao;

	public UILabel labelBarMibao;

	public GameObject spriteMibaoFrame;

	public BattleDroppenLayer droppenLayerBox;

	public BattleDroppenLayer droppenLayerCoin;

	public AchivementHintControllor achivementHintControllor;

	public UILabel labelLevelSkill_1;

	public UILabel labelLevelSkill_2;


	[HideInInspector] public bool b_joystick;
	
	[HideInInspector] public bool b_attack;
	
	[HideInInspector] public bool b_skill_light_1;
	
	[HideInInspector] public bool b_skill_light_2;
	
	[HideInInspector] public bool b_skill_heavy_1;
	
	[HideInInspector] public bool b_skill_heavy_2;
	
	[HideInInspector] public bool b_skill_ranged_1;
	
	[HideInInspector] public bool b_skill_ranged_2;
	
	[HideInInspector] public bool b_weapon_heavy;

	[HideInInspector] public bool b_weapon_light;

	[HideInInspector] public bool b_weapon_range;

	[HideInInspector] public bool b_skill_miBao;
	
	[HideInInspector] public bool b_autoFight;
	
	[HideInInspector] public bool b_pause;

	[HideInInspector] public bool b_dodge;

	[HideInInspector] public BattleResultControllor resultControllor;

	[HideInInspector] public BattleWinTemplate winDescTemplate;

	[HideInInspector] public bool pressedJoystick;


	private static BattleUIControlor _instance;

	private KingCamera m_camera;

	private bool keyDownW;

	private bool keyDownS;

	private bool keyDownA;
	
	private bool keyDownD;
	
	private GameObject offObject;
	
	private GameObject camObject;
	
	private GameObject angleObject;

	private BattleResult res;

	private Vector3 lastOffset;


	public static BattleUIControlor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }
	
	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor( this );

		_instance = null;
	}

	void Start()
	{
		SocketTool.RegisterMessageProcessor( this );
		
		m_camera = (KingCamera) Camera.main.gameObject.GetComponent ("KingCamera");

		cooldownLightSkill_1.init(200021);

		cooldownLightSkill_2.init(200022);

		cooldownHeavySkill_1.init (200011);

		cooldownHeavySkill_2.init (200012);

		cooldownRangeSkill_1.init (200031);

		cooldownRangeSkill_2.init (200032);
		
		cooldownLightSkill_1_enemy.init(200021);
		
		cooldownLightSkill_2_enemy.init(200022);
		
		cooldownHeavySkill_1_enemy.init (200011);
		
		cooldownHeavySkill_2_enemy.init (200012);
		
		cooldownRangeSkill_1_enemy.init (200031);
		
		cooldownRangeSkill_2_enemy.init (200032);

		BattleControlor.Instance().autoFight = false;
		
		dramaControllor.gameObject.SetActive (false);

		layerAutoFightHint.SetActive (false);

		spriteBarMibao.fillAmount = 0;

		labelBarMibao.text = "";

		droppenLayerBox.init ();

		droppenLayerCoin.init ();

		foreach(GameObject gc in layerAutoFight)
		{
			gc.transform.localPosition = new Vector3 (0, -10000, 0);
		}
		
		autoFight_1.gameObject.SetActive (false);
		
		autoFight_2.gameObject.SetActive (true);
		
		keyDownW = false;
		
		keyDownS = false;
		
		keyDownA = false;
		
		keyDownD = false;

		pressedJoystick = false;

		//if(CityGlobalData.m_enterPvp == false) checkDrama (true);
	}      
	
	public void checkDrama(bool first)
	{
		//bool f = CityGlobalData.getDramable ();
		
		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			level += 200000;
		}

		if(first)
		{
			if(CityGlobalData.m_battleType != EnterBattleField.BattleType.Type_GuoGuan
			   && CityGlobalData.m_battleType != EnterBattleField.BattleType.Type_YouXia)
			{
				WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate(CityGlobalData.m_pve_max_level);

				b_joystick = true;
				
				b_attack = true;
				
				b_skill_light_1 = template.b_skill_light_1;
				
				b_skill_light_2 = template.b_skill_light_2;
				
				b_skill_heavy_1 = template.b_skill_heavy_1;
				
				b_skill_heavy_2 = template.b_skill_heavy_2;
				
				b_skill_ranged_1 = template.b_skill_ranged_1;
				
				b_skill_ranged_2 = template.b_skill_ranged_2;

				b_dodge = template.b_dodge;
				
				b_weapon_heavy = true;
				
				b_weapon_light = true;
				
				b_weapon_range = true;
				
				b_skill_miBao = true;
				
				b_autoFight = true;
				
				b_pause = false;
			}
			else
			{
				PveFunctionOpen template = PveFunctionOpen.getPveFunctionOpenById (level);

				b_joystick = template.joystick;

				b_attack = template.attack;
				
				b_skill_light_1 = template.skill_light_1;
				
				b_skill_light_2 = template.skill_light_2;
				
				b_skill_heavy_1 = template.skill_heavy_1;
				
				b_skill_heavy_2 = template.skill_heavy_2;
				
				b_skill_ranged_1 = template.skill_ranged_1;
					
				b_skill_ranged_2 = template.skill_ranged_2;
				
				b_weapon_heavy = template.weaponHeavy;

				b_weapon_light = template.weaponLight;

				b_weapon_range = template.weaponRange;

				b_skill_miBao = template.skill_miBao;
				
				b_autoFight = template.autoFight;
				
				b_pause = template.pause;

				b_dodge = template.dodge;
			}
		}

		m_gc_move.SetActive (b_joystick);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.Attack, b_attack);
		
//		if(btnQiangSkill_1.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.LightSkill_1, b_skill_light_1);

//		if(btnQiangSkill_2.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.LightSkill_2, b_skill_light_2);
		
//		if(btnDaoSkill_1.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.HeavySkill_1, b_skill_heavy_1);

//		if(btnDaoSkill_2.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.HeavySkill_2, b_skill_heavy_2);

//		if(btnGongSkill_1.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.RangeSkill_1, b_skill_ranged_1);

//		if(btnGongSkill_2.activeSelf == true) 
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.RangeSkill_2, b_skill_ranged_2);

//		if(btnMibaoSkill.activeSelf == true) 

		//if(LockControllor.Instance().lockMiBaoSkill.activeSelf == false)
			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.MiBaoSkill,
		                                        b_skill_miBao 
		                                        && BattleControlor.Instance().getKing().kingSkillMibao != null
		                                        && BattleControlor.Instance().getKing().kingSkillMibao.Count > 0);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.WeaponHeavy, b_weapon_heavy && BattleControlor.Instance().getKing().weaponDateHeavy != null);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.WeaponLight, b_weapon_light && BattleControlor.Instance().getKing().weaponDateLight != null);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.WeaponRange, b_weapon_range && BattleControlor.Instance().getKing().weaponDateRanged != null);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.AutoFight, b_autoFight);

		LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.Pause, b_pause);

		LockControllor.Instance ().refreshLock (LockControllor.LOCK_TYPE.Dodge, b_dodge);
	}

	public void refreshWeaponIcon()
	{
		//if (m_changeWeapon.gameObject.activeSelf == false) return;

		m_changeWeapon.refreshIcons();
	}

	public void setMiBaoSkillIcon(HeroSkill mibaoSkill, bool isEnemy)
	{
		//btnMibaoSkill.SetActive (mibaoSkill != null);

		if(isEnemy == false)
		{
			spriteMibaoFrame.SetActive(mibaoSkill != null);

			LockControllor.Instance ().refreshLock ( LockControllor.LOCK_TYPE.MiBaoSkill, mibaoSkill != null);
		}

		if(mibaoSkill != null)
		{
			if(isEnemy == true)
			{
				cooldownMibaoSkill_enemy.init(mibaoSkill.template.timePeriod * 1f);
			}
			else
			{
				cooldownMibaoSkill.init(mibaoSkill.template.timePeriod * 1f);

				//int mibaoIconId = (mibaoSkill.template.id / 1000) * 1000 + 100 + (mibaoSkill.template.id % 100);//250204 - 250104

				btnMibaoSkillIcon.spriteName = mibaoSkill.template.name;
			}
		}
	}
	
	void FixedUpdate()
	{
		refreshWinDesc ();

		cooldownLightSkill_1.CoolDownUpdate();
		
		cooldownLightSkill_2.CoolDownUpdate();
		
		cooldownHeavySkill_1.CoolDownUpdate();
		
		cooldownHeavySkill_2.CoolDownUpdate();
		
		cooldownRangeSkill_1.CoolDownUpdate();
		
		cooldownRangeSkill_2.CoolDownUpdate();
		
		cooldownLightSkill_1_enemy.CoolDownUpdate();
		
		cooldownLightSkill_2_enemy.CoolDownUpdate();
		
		cooldownHeavySkill_1_enemy.CoolDownUpdate();
		
		cooldownHeavySkill_2_enemy.CoolDownUpdate();
		
		cooldownRangeSkill_1_enemy.CoolDownUpdate();
		
		cooldownRangeSkill_2_enemy.CoolDownUpdate();
		
		cooldownMibaoSkill.CoolDownUpdate();
		
		cooldownMibaoSkill_enemy.CoolDownUpdate();
	}
	
	void Update()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE
		{
			keyboardListen();
		}
		#endif

		CheckQuickFight();
	}

	private bool m_show_quick_fight = true;

	private void CheckQuickFight()
	{
//		if(labelDebug.gameObject.activeSelf == true)
//		{
//			string text = "";
//			
//			foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
//			{
//				text += node.nodeId + ": " + node.isAlive + ", " + node.flag.accountable + ", " + node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) + "\\n";
//			}
//
//			if(text.Length == 0)
//			{
//				text = "ALL KILL";
//			}
//
//			text += "" + BattleControlor.Instance().result;
//
//			//labelDebug.text = text;
//		}

		if( m_show_quick_fight == ConfigTool.GetBool( ConfigTool.CONST_QUICK_FIGHT ) )
		{
			return;
		}

		m_show_quick_fight = ConfigTool.GetBool( ConfigTool.CONST_QUICK_FIGHT );

		btnWin.SetActive( m_show_quick_fight );

		btnLose.SetActive( m_show_quick_fight );

		btnEasy.SetActive( m_show_quick_fight );

		btnDebug.SetActive ( m_show_quick_fight );

//		labelDebug.gameObject.SetActive (false);
	}

	public void addDebugText(string str)
	{
		//labelDebug.text += str + "\\n";
	}

	private void keyboardListen()
	{
		//if(dramaControllor.gameObject.activeSelf == true) return;
		
		if(Input.GetKey(KeyCode.Space))
		{
			keyDownW = false;
			
			keyDownS = false;
			
			keyDownA = false;
			
			keyDownD = false;
		}

		{
			if(Input.GetKeyDown(KeyCode.W)) keyDownW = true;
			
			if(Input.GetKeyDown(KeyCode.S)) keyDownS = true;
			
			if(Input.GetKeyDown(KeyCode.A)) keyDownA = true;
			
			if(Input.GetKeyDown(KeyCode.D)) keyDownD = true;
		}
		
		{
			if(Input.GetKeyUp(KeyCode.W)) keyDownW = false;
			
			if(Input.GetKeyUp(KeyCode.S)) keyDownS = false;
			
			if(Input.GetKeyUp(KeyCode.A)) keyDownA = false;
			
			if(Input.GetKeyUp(KeyCode.D)) keyDownD = false;
		}

		{
			Vector3 offset = Vector3.zero;
			
			if(keyDownW) offset += new Vector3(0, 0, 1);
			
			else if(keyDownS) offset += new Vector3(0, 0, -1);
			
			if(keyDownA) offset += new Vector3(-1, 0, 0);
			
			else if(keyDownD) offset += new Vector3(1, 0, 0);

			//if(Vector3.Distance(offset, Vector3.zero) > .2f) 

			moveKing(offset.normalized);
		}
	}

	public void resetKeyBoard()
	{
		keyDownW = false;
		
		keyDownS = false;
		
		keyDownA = false;
		
		keyDownD = false;
	}

	public void changeAutoFight()
	{
		foreach(GameObject gc in layerAutoFight)
		{
			gc.transform.localPosition = BattleControlor.Instance ().autoFight ? new Vector3 (0, -10000, 0) : Vector3.zero;
		}

		float alpha = BattleControlor.Instance ().autoFight ? 1 : .3f;

		//changeAlphaTo (alpha);
		
		BattleControlor.Instance().autoFight = !BattleControlor.Instance().autoFight;

		autoFight_1.gameObject.SetActive(BattleControlor.Instance().autoFight);
		
		autoFight_2.gameObject.SetActive(!BattleControlor.Instance().autoFight);

		layerAutoFightHint.SetActive (BattleControlor.Instance().autoFight);

		if(BattleControlor.Instance().autoFight == false)
		{
			BattleControlor.Instance().getKing().setNavMeshStop();
			
			BattleControlor.Instance().getKing().mAnim.SetTrigger(BattleControlor.Instance().getKing().getAnimationName(BaseAI.AniType.ANI_Stand0));
		}
		else
		{
			BattleControlor.Instance().getKing().setAutoWeapon();
		}
		
		//BattleReplayorWrite.Instance().addReplayNodeAutoFight(BattleControlor.Instance().getKing().nodeId);
	}

	private void changeAlphaTo(float alpha)
	{
		UISprite[] sprites = anchorBottom.GetComponentsInChildren<UISprite> ();
	
		foreach(UISprite sprite in sprites)
		{
			sprite.alpha = alpha;
		}

		sprites = anchorBottomRight.GetComponentsInChildren<UISprite> ();

		foreach(UISprite sprite in sprites)
		{
			if(sprite.transform.parent.localPosition.y > 300) continue;

			sprite.alpha = alpha;
		}

		UITexture[] texs = anchorBottomLeft.GetComponentsInChildren<UITexture>();

		foreach(UITexture tex in texs)
		{
			tex.alpha = alpha;
		}
	}
	
	public void moveKing(Vector3 offset)
	{
		if (dramaControllor.gameObject.activeSelf == true && dramaControllor.stopAction == true)
		{
			BattleControlor.Instance().getKing().move(Vector3.zero);
			
			return;
		}

		float offsetSize = Vector3.Distance (offset, Vector3.zero);

		if(BattleControlor.Instance().getKing() != null)
		{
			if( offsetSize != 0
			   		&& m_camera != null 
			  		&& m_camera.transform.localEulerAngles.y != 0) //摄像机角度不同时的修正
			{
				if(offObject == null)
				{
					offObject = new GameObject();
				}
				
				offObject.transform.localPosition = offset;
				
				offObject.transform.localEulerAngles = new Vector3(0, 0, 0);
				
				offObject.transform.localScale = new Vector3(1, 1, 1);
				
				if(camObject == null)
				{
					camObject = new GameObject();
				}
				
				camObject.transform.localPosition = new Vector3(0, 0, 0);
				
				camObject.transform.localEulerAngles = Vector3.zero;
				
				camObject.transform.localScale = new Vector3(1, 1, 1);
				
				offObject.transform.parent = camObject.transform;
				
				camObject.transform.localEulerAngles = new Vector3(0, m_camera.transform.localEulerAngles.y, 0);
				
				offset = offObject.transform.position;
			}

			lastOffset = offset;

			bool playingAttack = BattleControlor.Instance().getKing().isPlayingAttack() == true 
				&& BattleControlor.Instance().getKing().isPlayingSwing() == false;

			if(offsetSize != 0 && playingAttack == false) //转向时的角速度
			{
				if(angleObject == null)
				{
					angleObject = new GameObject();
				}

				angleObject.transform.localScale = new Vector3(1, 1, 1);

				angleObject.transform.localPosition = Vector3.zero;

				angleObject.transform.eulerAngles = BattleControlor.Instance().getKing().transform.eulerAngles;

				Vector3 oldangle = angleObject.transform.eulerAngles;
				
				angleObject.transform.forward = offset;
				
				float tar = angleObject.transform.eulerAngles.y; 
				
				float sp = 1080 * Time.deltaTime;
				
				float angle = Mathf.MoveTowardsAngle (oldangle.y, tar, sp);
				
				angleObject.transform.eulerAngles = new Vector3(0, angle, 0);
				
				offset = angleObject.transform.forward;
			}

			pressedJoystick = offsetSize > .1f;

			if(pressedJoystick == true && BattleControlor.Instance().autoFight == true)
			{
				BattleControlor.Instance().getKing().setNavMeshStop();
			}

			if(BattleControlor.Instance().getKing().isPlayingAttack() == true 
			   && BattleControlor.Instance().getKing().isPlayingSwing() == false)
			{
				BattleControlor.Instance().getKing().move(Vector3.zero);

				return;
			}

			if(offset.x != 0) offset += new Vector3(offset.x * 0.1f, 0, 0);

			BattleControlor.Instance().getKing().move(offset);
		}
	}
	
	public void kingStopAttack(){
		//BattleControlor.Instance().getKing().setAttacking(false);
	}
	
	public void kingAttack()
	{
		DramaControllor.Instance ().closeYindao (2);

		if (dramaControllor.gameObject.activeSelf == true && dramaControllor.stopAction == true) return;
		
		if(BattleControlor.Instance().getKing().isAlive)
		{
			BattleControlor.Instance().getKing().attack();
		}
	}

	public void kingDodge()
	{
		//if (BattleControlor.Instance ().getKing ().isPlayingAttack () == true) return;

		DramaControllor.Instance ().closeYindao (10);

		string playing = BattleControlor.Instance ().getKing ().IsPlaying ();

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName(BaseAI.AniType.ANI_DODGE))) return;

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCDown))) return;

		if (playing.Equals (BattleControlor.Instance ().getKing ().getAnimationName(BaseAI.AniType.ANI_BATCUp))) return;

		if (BattleControlor.Instance ().getKing ().isPlayingSkill () == true) return;

		if (BattleControlor.Instance ().getKing ().isIdle == true) return;

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return;

		BattleControlor.Instance ().getKing ().dodge (lastOffset);
	}

	public void kingChangeWeapon()
	{
		if(BattleControlor.Instance().getKing().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ) == 0) return;
		
		//if(BattleControlor.Instance().getKing().isPlayingAttack() == true) return;
		
		bool flag = false;
		
		if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Heavy)
		{
			flag = changeWeaponTo(KingControllor.WeaponType.W_Light);
			
			if(flag == false)
			{
				flag = changeWeaponTo(KingControllor.WeaponType.W_Ranged);
			}
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Light)
		{
			flag = changeWeaponTo(KingControllor.WeaponType.W_Ranged);
			
			if(flag == false)
			{
				flag = changeWeaponTo(KingControllor.WeaponType.W_Heavy);
			}
		}
		else if(BattleControlor.Instance().getKing().weaponType == KingControllor.WeaponType.W_Ranged)
		{
			flag = changeWeaponTo(KingControllor.WeaponType.W_Heavy);
			
			if(flag == false)
			{
				flag = changeWeaponTo(KingControllor.WeaponType.W_Light);
			}
		}
		
		if(flag == true)
		{
			BattleEffectControllor.Instance().PlayEffect(61, BattleControlor.Instance().getKing().gameObject);
		
			SoundPlayEff eff = (SoundPlayEff)BattleControlor.Instance().getKing().gameObject.GetComponent("SoundPlayEff");
		
			if(BattleControlor.Instance().completed) eff.PlaySound("2004");
		}
	}

	public bool changeWeaponTo(KingControllor.WeaponType weapon, bool first = false)// return: 换武器成功
	{
		KingControllor.WeaponType tempWeapon = weapon;

		if(tempWeapon == KingControllor.WeaponType.W_Ranged)
		{
			DramaControllor.Instance().closeYindao(7);
		}
		else if(tempWeapon == KingControllor.WeaponType.W_Light)
		{
			DramaControllor.Instance ().closeYindao (6);
		}

		if(weapon == KingControllor.WeaponType.W_Heavy && BattleControlor.Instance().getKing().m_weapon_Heavy == null)
		{
			return false;
		}
		else if(weapon == KingControllor.WeaponType.W_Light && BattleControlor.Instance().getKing().m_weapon_Light_left == null)
		{
			return false;
		}
		else if(weapon == KingControllor.WeaponType.W_Ranged && BattleControlor.Instance().getKing().m_weapon_Ranged == null)
		{
			return false;
		}

		if(first == false)
		{
			if(weapon == KingControllor.WeaponType.W_Heavy && BattleControlor.Instance().getKing().weaponDateHeavy == null)
			{
				return false;
			}
			else if(weapon == KingControllor.WeaponType.W_Light && BattleControlor.Instance().getKing().weaponDateLight == null)
			{
				return false;
			}
			else if(weapon == KingControllor.WeaponType.W_Ranged && BattleControlor.Instance().getKing().weaponDateRanged == null)
			{
				return false;
			}

			if(weapon == BattleControlor.Instance().getKing().weaponType)
			{
				return false;
			}
		}

		//BattleEffectControllor.Instance().PlayEffect( BattleEffectControllor.EffectType.EFFECT_KING_QIE_HUAN_WU_QI, BattleControlor.Instance().getKing().gameObject);
		
		SoundPlayEff eff = (SoundPlayEff)BattleControlor.Instance().getKing().gameObject.GetComponent("SoundPlayEff");
		
		if(BattleControlor.Instance().completed) eff.PlaySound("2004");

		foreach(GameObject go in attackIconList)
		{
			go.SetActive(false);
		}

		labelLevelSkill_1.text = "";

		labelLevelSkill_2.text = "";

		btnDaoSkill_1.SetActive(false);
		
		btnDaoSkill_2.SetActive(false);
		
		btnQiangSkill_1.SetActive(false);
		
		btnQiangSkill_2.SetActive(false);
		
		btnGongSkill_1.SetActive(false);
		
		btnGongSkill_2.SetActive(false);
		
		if(tempWeapon == KingControllor.WeaponType.W_Ranged)
		{
			btnGongSkill_1.SetActive(true);
			
			btnGongSkill_2.SetActive(true);

			if(b_skill_ranged_1 == true && first == false) 
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnGongSkill_1, EffectIdTemplate.GetPathByeffectId(100170));
			
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnGongSkill_1, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(b_skill_ranged_2 == true && first == false) 
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnGongSkill_2, EffectIdTemplate.GetPathByeffectId(100170));
				
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnGongSkill_2, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.zhuixingjian] == 0)
			{
				labelLevelSkill_1.text = HeroSkillUpTemplate.GetHeroSkillUpByID(3200).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}
			
			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.hanbingjian] == 0)
			{
				labelLevelSkill_2.text = HeroSkillUpTemplate.GetHeroSkillUpByID(3300).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}
		}
		else if(tempWeapon == KingControllor.WeaponType.W_Light)
		{
			btnQiangSkill_1.SetActive(true);
			
			btnQiangSkill_2.SetActive(true);
			
			if(b_skill_light_1 == true && first == false) 
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnQiangSkill_1, EffectIdTemplate.GetPathByeffectId(100170));
			
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnQiangSkill_1, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(b_skill_light_2 == true && first == false)
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnQiangSkill_2, EffectIdTemplate.GetPathByeffectId(100170));
				
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnQiangSkill_2, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.jueyingxingguangzhan] == 0)
			{
				labelLevelSkill_1.text = HeroSkillUpTemplate.GetHeroSkillUpByID(2200).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}
			
			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.xuejilaoyin] == 0)
			{
				labelLevelSkill_2.text = HeroSkillUpTemplate.GetHeroSkillUpByID(2300).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}
		}
		else if(tempWeapon == KingControllor.WeaponType.W_Heavy)
		{
			btnDaoSkill_1.SetActive(true);
			
			btnDaoSkill_2.SetActive(true);

			if(b_skill_heavy_1 == true && first == false)
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnDaoSkill_1, EffectIdTemplate.GetPathByeffectId(100170));
				
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnDaoSkill_1, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(b_skill_heavy_2 == true && first == false)
			{
				UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnDaoSkill_2, EffectIdTemplate.GetPathByeffectId(100170));
				
				UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, btnDaoSkill_2, EffectIdTemplate.GetPathByeffectId(100009));
			}

			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.bahuanglieri] == 0)
			{
				labelLevelSkill_1.text = HeroSkillUpTemplate.GetHeroSkillUpByID(1200).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}

			if(BattleControlor.Instance().getKing().skillLevel[(int)CityGlobalData.skillLevelId.qiankundouzhuan] == 0)
			{
				labelLevelSkill_2.text = HeroSkillUpTemplate.GetHeroSkillUpByID(1300).m_iNeedLV + LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_UNLOCK);
			}
		}

		BattleControlor.Instance().getKing().changeWeapon(tempWeapon);

		BattleControlor.Instance().getKing().refreshWeaponDate();

		attackIconList[(int)BattleControlor.Instance().getKing().weaponType].SetActive(true);

		if(first == false)
		{
			UI3DEffectTool.Instance().ShowBottomLayerEffect(UI3DEffectTool.UIType.MainUI_0, attackIconList[(int)BattleControlor.Instance().getKing().weaponType], EffectIdTemplate.GetPathByeffectId(100170));
		
			UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, attackIconList[(int)BattleControlor.Instance().getKing().weaponType], EffectIdTemplate.GetPathByeffectId(100009));

			BattleControlor.Instance().showUnlockEffByNet();
		}

		if (first || BattleControlor.Instance().autoFight == true) refreshWeaponIcon ();

		//if(CityGlobalData.m_enterPvp == false)
			checkDrama (first);

		return true;
	}

	public bool changeWeaponTo_enemy(KingControllor.WeaponType weapon, KingControllor node)
	{
		if(weapon == KingControllor.WeaponType.W_Heavy && node.m_weapon_Heavy == null)
		{
			return false;
		}
		else if(weapon == KingControllor.WeaponType.W_Light && node.m_weapon_Light_left == null)
		{
			return false;
		}
		else if(weapon == KingControllor.WeaponType.W_Ranged && node.m_weapon_Ranged == null)
		{
			return false;
		}
		
		node.changeWeapon (weapon);

		node.refreshWeaponDate();

		return true;
	}
	
	public bool useDaoSkill_1()
	{
		DramaControllor.Instance ().closeYindao (3);

		DramaControllor.Instance ().closeYindao (11);

		if (btnDaoSkill_1.activeSelf == false) return false;

		if (b_skill_heavy_1 == false) return false;

		if (cooldownHeavySkill_1.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;

			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		BattleControlor.Instance().getKing().useDaoSkill_1();

		//cooldownHeavySkill_1.refreshCDTime ();

		return true;
	}

	public bool useDaoSkill_2()
	{
		DramaControllor.Instance ().closeYindao (4);

		DramaControllor.Instance ().closeYindao (12);

		if (btnDaoSkill_2.activeSelf == false) return false;

		if (b_skill_heavy_2 == false) return false;

		if (cooldownHeavySkill_2.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;

			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		if (BattleControlor.Instance ().getKing ().kingSkillHeavy_2 == null) return false;

		BattleControlor.Instance().getKing().useDaoSkill_2();

		//cooldownHeavySkill_2.refreshCDTime ();

		return true;
	}
	
	public bool useQiangSkill_1()
	{
		DramaControllor.Instance ().closeYindao  (3);

		DramaControllor.Instance ().closeYindao  (13);

		if (btnQiangSkill_1.activeSelf == false) return false;

		if (b_skill_light_1 == false) return false;

		if (cooldownLightSkill_1.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;
			
			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}
		
		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		BattleControlor.Instance().getKing().useQiangSkill_1();
		
		//cooldownLightSkill_1.refreshCDTime ();

		return true;
	}
	
	public bool useQiangSkill_2()
	{
		DramaControllor.Instance ().closeYindao (4);

		DramaControllor.Instance ().closeYindao (14);

		if (btnQiangSkill_2.activeSelf == false) return false;

		if (b_skill_light_2 == false) return false;

		if (cooldownLightSkill_2.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;
			
			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		BattleControlor.Instance().getKing().useQiangSkill_2();
		
		//cooldownLightSkill_2.refreshCDTime ();

		return true;
	}
	
	public bool useGongSkill_1()
	{
		DramaControllor.Instance ().closeYindao (3);

		DramaControllor.Instance ().closeYindao (15);

		if (btnGongSkill_1.activeSelf == false) return false;

		if (b_skill_ranged_1 == false) return false;

		if (cooldownRangeSkill_1.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;
			
			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		BattleControlor.Instance().getKing().useGongSkill_1();
		
		//cooldownRangeSkill_1.refreshCDTime ();

		return true;
	}
	
	public bool useGongSkill_2()
	{
		DramaControllor.Instance ().closeYindao (4);

		DramaControllor.Instance ().closeYindao (16);

		if (btnGongSkill_2.activeSelf == false) return false;

		if (b_skill_ranged_2 == false) return false;

		if (cooldownRangeSkill_2.spriteCD.gameObject.activeSelf == true) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;
			
			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		if (BattleControlor.Instance ().getKing ().nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		BattleControlor.Instance().getKing().useGongSkill_2();

		//cooldownRangeSkill_2.refreshCDTime ();

		return true;
	}

	public bool useDaoSkill_1_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);

		if (template.b_skill_heavy_1 == false) return false;

		if (cooldownHeavySkill_1_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack() == true) return false;
		
		if (node.isPlayingSkill() == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useDaoSkill_1();
		
		//cooldownHeavySkill_1_enemy.refreshCDTime ();

		return true;
	}
	
	public bool useDaoSkill_2_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);
		
		if (template.b_skill_heavy_2 == false) return false;
		
		if (cooldownHeavySkill_2_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack() == true) return false;
		
		if (node.isPlayingSkill() == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useDaoSkill_2();

		//cooldownHeavySkill_2_enemy.refreshCDTime ();

		return true;
	}
	
	public bool useQiangSkill_1_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);
		
		if (template.b_skill_light_1 == false) return false;
		
		if (cooldownLightSkill_1_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack () == true) return false;
		
		if (node.isPlayingSkill () == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useQiangSkill_1();
		
		//cooldownLightSkill_1_enemy.refreshCDTime ();

		return true;
	}
	
	public bool useQiangSkill_2_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);
		
		if (template.b_skill_light_2 == false) return false;
		
		if (cooldownLightSkill_2_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack () == true) return false;
		
		if (node.isPlayingSkill () == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useQiangSkill_2();
		
		//cooldownLightSkill_2_enemy.refreshCDTime ();

		return true;
	}
	
	public bool useGongSkill_1_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);
		
		if (template.b_skill_ranged_1 == false) return false;
		
		if (cooldownRangeSkill_1_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack () == true) return false;
		
		if (node.isPlayingSkill () == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useGongSkill_1();

		//cooldownRangeSkill_1_enemy.refreshCDTime ();

		return true;
	}

	public bool useGongSkill_2_Enemy(KingControllor node)
	{
		WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (node.maxLevel);

		if (template.b_skill_ranged_2 == false) return false;
		
		if (cooldownRangeSkill_2_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.isPlayingAttack () == true) return false;

		if (node.isPlayingSkill () == true) return false;

		if (node.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp) < 0) return false;

		node.useGongSkill_2();
		
		//cooldownRangeSkill_2_enemy.refreshCDTime ();

		return true;
	}

	public bool useMiBaoSkill()
	{
		DramaControllor.Instance ().closeYindao (17);

		if (btnMibaoSkill.activeSelf == false) return false;
		
		if (b_skill_miBao == false) return false;
		
		if (cooldownMibaoSkill.spriteCD.gameObject.activeSelf == true) return false;

		if (BattleControlor.Instance().getKing().kingSkillMibao == null || BattleControlor.Instance().getKing().kingSkillMibao.Count == 0) return false;

		if(BattleControlor.Instance().getKing().isPlayingSwing() == false)
		{
			//if (BattleControlor.Instance().getKing().isPlayingAttack() == true) return false;
			
			if (BattleControlor.Instance ().getKing ().isPlayingSkill() == true) return false;
		}

		BattleControlor.Instance().getKing().useMiBaoSkill();

		//cooldownMibaoSkill.refreshCDTime ();

		return true;
	}

	public bool useMiBaoSkill_Enemy(KingControllor node)
	{
		if (cooldownMibaoSkill_enemy.spriteCD.gameObject.activeSelf == true) return false;

		if (node.kingSkillMibao == null || node.kingSkillMibao.Count == 0) return false;

		if (node.isPlayingAttack () == true) return false;

		if (node.isPlayingSkill () == true) return false;

		node.useMiBaoSkill();

		//cooldownMibaoSkill_enemy.refreshCDTime ();

		return true;
	}

	public void enterPause()
	{
		if (BattleControlor.Instance ().result != BattleControlor.BattleResult.RESULT_BATTLING) return;

		pauseControllor.refreshData ();

		pauseControllor.gameObject.SetActive(true);
		
		Time.timeScale = 0f;
	}
	
	void LateUpdate()
	{
		if(BattleControlor.Instance().getKing() != null)
		{
			barSelf.value = BattleControlor.Instance().getKing().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ) / BattleControlor.Instance().getKing().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );
		}
	}
	
	public void devolopmentDebug()
	{
		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.gameObject.activeSelf == true)
			{
				node.addHp(node.nodeData.GetAttribute((int)AIdata.AttributeType.ATTRTYPE_hpMaxReal) * .45f);
			}
		}

//		BattleControlor.Instance ().battleTime = 235;
//
//		CityGlobalData.autoFightDebug = !CityGlobalData.autoFightDebug;
//
//		if (BattleControlor.Instance ().autoFight == false) changeAutoFight ();
//
//		UILabel label = btnDebug.GetComponentInChildren<UILabel>();
//
//		if(CityGlobalData.autoFightDebug == true)
//		{
//			label.text = "AUTO";
//		}
//		else
//		{
//			label.text = "测";
//		}
	}

	public void devolopmentWin()
	{
		foreach(BaseAI ba in BattleControlor.Instance().selfNodes)
		{
			ba.setNavMeshStop();
		}

		foreach(BaseAI ba in BattleControlor.Instance().enemyNodes)
		{
			ba.setNavMeshStop();
		}

		BattleControlor.Instance ().result = BattleControlor.BattleResult.RESULT_WIN;

		BattleControlor.Instance ().showResult ();
	}

	public void devolopmentEasy()
	{
//		BattleControlor.Instance ().getKing ().addHp (2 - BattleControlor.Instance ().getKing ().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ));

//		BattleControlor.Instance ().getKing ().nodeData.SetAttribute ( AIdata.AttributeType.ATTRTYPE_hpMax, 100000000);

		BattleControlor.Instance ().getKing ().addHp (BattleControlor.Instance ().getKing ().nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ));

		//BattleControlor.Instance ().getKing ().nodeData.SetAttribute (AIdata.AttributeType.ATTRTYPE_attackValue, 100000f);

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.gameObject.activeSelf == true)
			{
//				BattleControlor.Instance ().getKing ().nodeData.SetAttribute ( AIdata.AttributeType.ATTRTYPE_hpMax, 100);

//				node.addHp(0);

				node.addHp(node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax ) * .05f - node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp ));

//				node.nodeData.SetAttribute(AIdata.AttributeType.ATTRTYPE_hpMax, 100000000);
//
//				node.addHp(node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hpMax));
			}
		}
	}

	public void devolopmentLose()
	{
		foreach(BaseAI ba in BattleControlor.Instance().selfNodes)
		{
			ba.setNavMeshStop();
		}
		
		foreach(BaseAI ba in BattleControlor.Instance().enemyNodes)
		{
			ba.setNavMeshStop();
		}

		BattleControlor.Instance ().result = BattleControlor.BattleResult.RESULT_LOSE;
		
		BattleControlor.Instance ().showResult ();
	}

	public void exitBattleWithAutoFightDebug()
	{
		StartCoroutine (exitBattleWithAutoFightDebugAction());
	}

	IEnumerator exitBattleWithAutoFightDebugAction()
	{
		yield return new WaitForSeconds (.5f);

		EnterBattleField.EnterBattlePve (CityGlobalData.m_tempSection, CityGlobalData.m_tempLevel, LevelType.LEVEL_NORMAL);
	}

	public void showResult(BattleControlor.BattleResult result)
	{
		short requestId = 0;

		sendEndSecret ();

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			sendResultPve(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan)
		{
			sendResultPvp(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			sendResultHYPve(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp)
		{
			sendResultHYPvp(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
			sendResultYaBiao(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			sendResultYouXia(result);
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
		{
			sendResultLueDuo(result);
		}
	}

	public void LoadResultRes()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.BATTLE_RESULT), LoadResultResCallback);
	}

	private void LoadResultResCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject resultObject = (GameObject)Instantiate(p_object);

		resultObject.transform.parent = transform.parent.parent;

		resultObject.transform.localPosition = new Vector3 (5000, 0, 0);

		resultObject.transform.localScale = new Vector3 (1, 1, 1);

		resultObject.transform.localEulerAngles = Vector3.zero;

		resultControllor = resultObject.GetComponent<BattleResultControllor>();

		resultObject.SetActive (false);
	}

	private void sendEndSecret()
	{
		BattleNet.Instance ().sendEndSecret();
	}

	private void sendResultPve(BattleControlor.BattleResult result)
	{
		resultControllor.bList_achivment = new List<int> ();
		
		if (BattleControlor.Instance ().achivement != null)
		{
			resultControllor.bList_achivment = BattleControlor.Instance().achivement.EndBattle();	
		}
		else
		{
			resultControllor.bList_achivment.Add(0);
			
			resultControllor.bList_achivment.Add(0);
			
			resultControllor.bList_achivment.Add(0);
		}
		
		resultControllor.winLevel = resultControllor.getStarCount();
		
		int iAchiv = 0;
		
		if (resultControllor.bList_achivment [0] == 1) iAchiv += 100;
		
		if (resultControllor.bList_achivment [1] == 1) iAchiv += 10;

		if (resultControllor.bList_achivment [2] == 1) iAchiv += 1;
		
		PveBattleOver tempbat = new PveBattleOver();
		
		tempbat.s_section = 100000 + 100 * CityGlobalData.m_tempSection + CityGlobalData.m_tempLevel;
		
		tempbat.s_pass = (result == BattleControlor.BattleResult.RESULT_WIN);
		
		tempbat.star = resultControllor.winLevel;
		
		tempbat.achievement = iAchiv;

		tempbat.dropeenItemNpcs = BattleControlor.Instance ().droppenList;

		MemoryStream t_tream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(t_tream, tempbat);
		
		byte[] t_protof;
		
		t_protof = t_tream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_BATTLE_OVER_REPORT, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.BattlePve_Result);
	}

	private void sendResultPvp(BattleControlor.BattleResult result)
	{
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();

		BaiZhanResult req = new BaiZhanResult();
		
		req.enemyId = CityGlobalData.m_tempEnemy;
		
		req.winId = result == BattleControlor.BattleResult.RESULT_LOSE ? 
			CityGlobalData.m_tempEnemy : JunZhuData.Instance().m_junzhuInfo.id;
		
		req.zhandouId = BattleControlor.Instance().battleId;

		t_qx.Serialize(tempStream, req);

		byte[] t_protof;
		
		t_protof = tempStream.ToArray();

		int waitIndex = ProtoIndexes.BAIZHAN_RESULT_RESP;

//		if(result == BattleControlor.BattleResult.RESULT_LOSE)
//		{
//			resultControllor.gameObject.SetActive(true);
//
//			resultControllor.showResult();
//
//			waitIndex = -1;
//		}

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.BAIZHAN_RESULT, ref t_protof, p_receiving_wait_proto_index:waitIndex);
	}

	private void sendResultHYPve(BattleControlor.BattleResult result)
	{
		int damageValue = 0;

		for(int i = 0; i < 10; i++)
		{
			int damage = 0;
			
			BattleControlor.Instance ().hurts.TryGetValue (i, out damage);

			damageValue += damage;
		}

		HuangYePveOver req = new HuangYePveOver ();

		req.id = (int)CityGlobalData.m_tempEnemy;

		req.isPass = 1;

		req.damageValue = damageValue;

		req.npcInfos = new List<HYPveNpcInfo> ();

		req.costTime = (int)BattleControlor.Instance ().battleTime;

		foreach(int id in BattleControlor.Instance().deadNodes)
		{
			HYPveNpcInfo info = new HYPveNpcInfo();
			
			info.npcId = id;

			info.remainHP = 0;

			req.npcInfos.Add(info);

			Debug.Log("dead Node id: " + id);
		}

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			HYPveNpcInfo info = new HYPveNpcInfo();

			info.npcId = node.nodeId;

			info.remainHP = (int)node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );

			if(info.remainHP > 0) req.isPass = 0;

			req.npcInfos.Add(info);

//			Debug.Log("alive Node id: " + info.npcId + ", hp: " + info.remainHP);
		}

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();

		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_HUANGYE_PVE_OVER, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.BattlePve_Result);
	}

	private void sendResultHYPvp(BattleControlor.BattleResult result)
	{
		HuangYePvpOver req = new HuangYePvpOver ();

		req.id = CityGlobalData.m_tempPoint;

		req.bossId = (int)CityGlobalData.m_tempEnemy;

		req.isPass = result == BattleControlor.BattleResult.RESULT_WIN ? 1 : 0;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_HUANGYE_PVP_OVER, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.S_HUANGYE_PVP_OVER_RESP);
	}

	private void sendResultYaBiao(BattleControlor.BattleResult result)
	{
		JieBiaoResult req = new JieBiaoResult ();

		req.enemyId = CityGlobalData.m_tempEnemy;
		
		req.winId = result == BattleControlor.BattleResult.RESULT_LOSE ? 
			CityGlobalData.m_tempEnemy : JunZhuData.Instance().m_junzhuInfo.id;

		req.npcInfos = new List<YBPveNpcInfo> ();
		
		foreach(int id in BattleControlor.Instance().deadNodes)
		{
			YBPveNpcInfo info = new YBPveNpcInfo();

			info.npcId = id;
			
			info.remainHP = 0;
			
			req.npcInfos.Add(info);
			
			Debug.Log("dead Node id: " + id);
		}
		
		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			YBPveNpcInfo info = new YBPveNpcInfo();

			info.npcId = node.nodeId;
			
			info.remainHP = (int)node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );
			
			req.npcInfos.Add(info);
			
			Debug.Log("alive Node id: " + info.npcId + ", hp: " + info.remainHP);
		}

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YABIAO_RESULT, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.BattlePve_Result);
	}

	private void sendResultYouXia(BattleControlor.BattleResult result)
	{
		BattleYouXiaResultReq req = new BattleYouXiaResultReq ();
		
		req.id = 300000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
		
		req.result = result == BattleControlor.BattleResult.RESULT_LOSE ? 0 : 1;

//		req.dropeenItems = new List<DroppenItemResult> ();
//
//		foreach(int id in BattleControlor.Instance().droppenDict.Keys)
//		{
//			int num = BattleControlor.Instance().droppenDict[id];
//
//			if(num == 0) continue;
//
//			DroppenItemResult item = new DroppenItemResult();
//
//			item.id = id;
//
//			item.num = num;
//
//			req.dropeenItems.Add(item);
//		}

		req.dropeenItemNpcs = BattleControlor.Instance ().droppenList;

		req.score = BattleControlor.Instance ().battleCheck.bossKilled + BattleControlor.Instance ().battleCheck.heroKilled + BattleControlor.Instance ().battleCheck.soldierKilled + BattleControlor.Instance ().battleCheck.gearKilled;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_BATTLE_OVER_REQ, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.BattlePve_Result);
	}

	private void sendResultLueDuo(BattleControlor.BattleResult result)
	{
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		LveBattleEndReq req = new LveBattleEndReq ();
		
		req.enemyId = CityGlobalData.m_tempEnemy;
		
		req.winId = result == BattleControlor.BattleResult.RESULT_LOSE ? 
			CityGlobalData.m_tempEnemy : JunZhuData.Instance().m_junzhuInfo.id;
		
		req.zhandouId = BattleControlor.Instance().battleId;

		req.bings = new List<Bing> ();

		foreach(int id in BattleControlor.Instance().deadNodes)
		{
			if(id < 100) continue;

			if(id == 101)
			{
				req.enemyHp = 0;

				continue;
			}

			Bing info = new Bing();
			
			info.id = id;
			
			info.hp = 0;

			req.bings.Add(info);
		}
		
		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.nodeId == 101)
			{
				req.enemyHp = (int)node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );

				continue;
			}

			Bing info = new Bing();
			
			info.id = node.nodeId;
			
			info.hp = (int)node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hp );
			
			req.bings.Add(info);
		}

		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		int waitIndex = ProtoIndexes.LVE_BATTLE_END_RESP;
	
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.LVE_BATTLE_END_REQ, ref t_protof, p_receiving_wait_proto_index:waitIndex);
	}

	public void sendHelp()
	{
		LveHelpReq req = new LveHelpReq ();

		req.enemyId = CityGlobalData.m_tempEnemy;

		BaseAI enemyNode = BattleControlor.Instance ().getNodebyId (101);

		if(enemyNode == null)
		{
			req.remainHp = 0;
		}
		else
		{
			req.remainHp = (int)enemyNode.nodeData.GetAttribute (AIdata.AttributeType.ATTRTYPE_hp);
		}

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.LVE_HELP_REQ, ref t_protof, p_receiving_wait_proto_index:ProtoIndexes.LVE_HELP_RESP);
	}

	private void OnSendExitBattle()
	{
		LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_NETWORK, "SendEnterBattle" );
		
		PlayerState t_state = new PlayerState();
		
		t_state.s_state = State.State_LEAGUEOFCITY;
		
		SocketHelper.SendQXMessage( t_state, ProtoIndexes.PLAYER_STATE_REPORT );
	}

	private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		resultControllor.itemTemple = (GameObject)p_object;

		resultControllor.itemTemple.SetActive (false);

		resultControllor.gameObject.SetActive(true);

		resultControllor.showResult();

		showAward (res);
	}

	public void ExecQuit()
	{
		UIYindao.m_UIYindao.CloseUI ();
		
		AudioListener al = BattleControlor.Instance().getKing().gameCamera.target.GetComponent<AudioListener>();
		
		Destroy ( al );
		
		if (CityGlobalData.m_isWhetherOpenLevelUp == false) return;
		
		ClientMain.m_ClientMainObj.AddComponent<AudioListener> ();
		
		GameObject root3d = GameObject.Find("BattleField_V4_3D");
		
		GameObject root2d = GameObject.Find("BattleField_V4_2D");
		
		foreach( BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			node.setNavEnabled(false);
		}
		
		foreach( BaseAI node in BattleControlor.Instance().selfNodes)
		{
			node.setNavEnabled(false);
		}
		
		foreach( BaseAI node in BattleControlor.Instance().midNodes)
		{
			node.setNavEnabled(false);
		}
		
		//Destroy(root3d);
		
		//Destroy(root2d);
		
		SceneManager.EnterCreateRole();
		
		if (!string.IsNullOrEmpty(PlayerPrefs.GetString("JunZhu")))
		{
			CityGlobalData.m_JunZhuCreate = true;
		}
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;

		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.BattlePve_Result:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			BattleResult res = new BattleResult();

			t_qx.Deserialize(t_stream, res, res.GetType());

			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan && CityGlobalData.m_tempSection == 0 && CityGlobalData.m_tempLevel == 1)
			{
				ExecQuit();
			}
			else
			{
				loadIconSample(res);
			}
			//showAward(res);
			
			return true;
		}
		case ProtoIndexes.BAIZHAN_RESULT_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			BaiZhanResultResp res = new BaiZhanResultResp();

			t_qx.Deserialize(t_stream, res, res.GetType());

			resultControllor.setPVPData(res);

			resultControllor.gameObject.SetActive(true);
			
			resultControllor.showResult();

			return true;
		}
		case ProtoIndexes.S_HUANGYE_PVP_OVER_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			BattleResultHYPvp res = new BattleResultHYPvp();
			
			t_qx.Deserialize(t_stream, res, res.GetType());

			resultControllor.setHYPvpData(res);

			resultControllor.gameObject.SetActive(true);
			
			resultControllor.showResult();

			return true;
		}
		case ProtoIndexes.LVE_BATTLE_END_RESP:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			LveBattleResult res = new LveBattleResult();
			
			t_qx.Deserialize(t_stream, res, res.GetType());
			
			resultControllor.setLueDuoData(res);
			
			resultControllor.gameObject.SetActive(true);

			resultControllor.showResult();
			
			return true;
		}
		case ProtoIndexes.LVE_HELP_RESP:
		{
			resultControllor.OnResultHelpCallback();

			return true;
		}
		}
		
		return false;
	}

	private void loadIconSample(BattleResult _res)
	{
		res = _res;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
	}

	private void showAward(BattleResult res)
	{
		for(int i = 0; res.awardItems != null && i < res.awardItems.Count; i++)
		{
			qxmobile.protobuf.AwardItem award = res.awardItems[i];
			
			resultControllor.addAward(award, i, res.awardItems.Count);
		}

		resultControllor.SetExpMoney(res.exp, res.money);

		if(res.lmAwardItems != null) resultControllor.lmAwards = res.lmAwardItems;

		else resultControllor.lmAwards.Clear();
	}
	
	public void showDaramControllor(int level, int eventId, DramaControllor.Callback _callBack = null)
	{
		dramaControllor.showEvent (level, eventId, _callBack);
	}

	public void setShowWinDesc(BattleWinTemplate _template)
	{
		winDescTemplate = _template;

		winDesc.text = "";

		winDescNum.text = "";
	}

	public void refreshWinDesc()
	{
		if (winDescTemplate == null) return;

		int descLanguageId = 0;

		if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_All)
		{
			descLanguageId = 1082;

			winDescNum.text = "";
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Boss)
		{
			descLanguageId = 1083;

			winDescNum.text = BattleControlor.Instance().battleCheck.bossKilled + "/" + winDescTemplate.killNum;
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Hero)
		{
			descLanguageId = 1092;

			winDescNum.text = BattleControlor.Instance().battleCheck.heroKilled + "/" + winDescTemplate.killNum;
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Soldier)
		{
			descLanguageId = 1090;

			winDescNum.text = BattleControlor.Instance().battleCheck.soldierKilled + "/" + winDescTemplate.killNum;
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Kill_Gear)
		{
			descLanguageId = 1088;

			winDescNum.text = BattleControlor.Instance().battleCheck.gearKilled + "/" + winDescTemplate.killNum;
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Reach_Destination)
		{
			descLanguageId = 1085;

			winDescNum.text = ((int)Vector3.Distance(BattleControlor.Instance().getKing().transform.position, winDescTemplate.destination) - winDescTemplate.destinationRadius) + "m";
		}
		else if(winDescTemplate.winType == BattleWinFlag.EndType.Reach_Time)
		{
			descLanguageId = 1086;

			winDescNum.text = BattleControlor.Instance().timeLast + "s";
		}

		winDesc.text = LanguageTemplate.GetText (descLanguageId) + " " + winDescNum.text;
	}

}