using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class KingControllor : HeroAI
{
	public enum WeaponType
	{
		W_Heavy,
		W_Light,
		W_Ranged,
	}
	
	public GameObject m_weapon_Heavy;
	
	public GameObject m_weapon_Light_left;
	
	public GameObject m_weapon_Light_right;

	public GameObject m_weapon_Ranged;
	
	public KingWeapon weaponBox_1;//轻武器

	public KingWeapon weaponBox_2;//重武器
	
	public KingWeapon weaponBox_3;//旋风斩
	
	public KingWeapon weaponBox_4;//冲锋
	
	public RuntimeAnimatorController lightControllor;
	
	public RuntimeAnimatorController heavyConrollor;
	
	public RuntimeAnimatorController rangedConrollor;


	[HideInInspector] public KingCamera gameCamera;
	
	[HideInInspector] public WeaponType weaponType;
	
	[HideInInspector] public PlayerWeapon weaponDateHeavy;
	
	[HideInInspector] public PlayerWeapon weaponDateLight;
	
	[HideInInspector] public PlayerWeapon weaponDateRanged;
	
	[HideInInspector] public int actionId;
	
	[HideInInspector] public HeroSkill kingSkillMibao;

	[HideInInspector] public HeroSkill kingSkillHeavy_2;

	[HideInInspector] public int maxLevel;

	[HideInInspector] public int hitCount;

	[HideInInspector] public SignShadow signShadow;

	public bool comboable;
	

	private bool moving;
	
	private float tempSpeed;
	
	private System.DateTime tempTime;
	
	private Vector3 prepareFoward;
	
	private Vector3 tempOffset;
	
	private bool moveableInAttack;

	private bool autoMove;

	private float autoWeaponCD;

	private float skillCD;

	private bool inAttackAuto;//攻击时攻击范围内没有敌人

	private Vector3 inAttackPos;

	private string str_swingState = "";

	private bool b_resetHit;


	public override void Start()
	{
		//base.Start ();

		prepareFoward = Vector3.zero;
		
		isBlind = false;
		
		isIdle = false;
		
		slowdownable = true;
		
		moveable = true;
		
		comboable = true;

		hitCount = 0;

		b_resetHit = false;

		moving = false;
		
		tempSpeed = 0;
		
		initWeapon();

		//changeWeapon(WeaponType.W_Light);

		attackTempTime = System.DateTime.Now;

		tempTime = System.DateTime.Now;

		tempOffset = Vector3.zero;

		moveableInAttack = false;

		autoMove = false;

		//initWeaponData ();

		autoWeaponCD = 0;

		skillCD = 0;

		inAttackAuto = false;

		inAttackPos = Vector3.zero;
	}

	public override void initStart()
	{
		base.initStart ();
		
		gameCamera = (KingCamera) Camera.main.GetComponent(typeof(KingCamera));

		gameCamera.init ();
	}

	private void initWeapon()
	{
		if(weaponBox_1 != null) weaponBox_1.initWeapon (this, 1);
		
		if(weaponBox_2 != null) weaponBox_2.initWeapon (this, 2);

		if(weaponBox_3 != null) weaponBox_3.initWeapon (this, 3);

		if(weaponBox_4 != null) weaponBox_4.initWeapon (this, 4);

		if(weaponBox_1 != null) weaponBox_1.gameObject.SetActive (false);
		
		if(weaponBox_2 != null) weaponBox_2.gameObject.SetActive (false);
		
		if(weaponBox_3 != null) weaponBox_3.gameObject.SetActive (false);
		
		if(weaponBox_4 != null) weaponBox_4.gameObject.SetActive (false);
	}

	public void changeWeapon(WeaponType _weapon)
	{
		weaponType = _weapon;
		
		//BattleReplayorWrite.Instance().addReplayKingWeapon(nodeId, weaponType);

		int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		int weaponId = 0;

		if(weaponType == WeaponType.W_Heavy) weaponId = 1;

		else if(weaponType == WeaponType.W_Light) weaponId = 2;

		else weaponId = 3;

		bool f = GuideTemplate.HaveId_type (level, 7, weaponId);

		if(f == true)
		{
			GuideTemplate template = GuideTemplate.getTemplateByLevelAndType (level, 7, weaponId);
			
			bool flag = BattleControlor.Instance ().havePlayedGuide (template);
			
			if (flag == false) 
			{
				BattleUIControlor.Instance ().showDaramControllor (level, template.eventId);
			}
		}

		ResetHitCount ();
		
		hitCount = 0;

		if(m_weapon_Heavy != null) m_weapon_Heavy.SetActive(false);
		
		if(m_weapon_Light_left != null) m_weapon_Light_left.SetActive(false);
		
		if(m_weapon_Light_right != null) m_weapon_Light_right.SetActive(false);
		
		if(m_weapon_Ranged != null) m_weapon_Ranged.SetActive(false);

		if(weaponType == WeaponType.W_Heavy && m_weapon_Heavy != null)
		{
			m_weapon_Heavy.SetActive(true);
			
			//mAnim.SetTrigger(getAnimationName(AniType.ANI_Stand0));
			
			mAnim.runtimeAnimatorController = heavyConrollor;
		}
		else if(weaponType == WeaponType.W_Light && m_weapon_Light_left != null && m_weapon_Light_right != null)
		{
			m_weapon_Light_left.SetActive(true);
			
			m_weapon_Light_right.SetActive(true);
			
			//mAnim.SetTrigger(getAnimationName(AniType.ANI_Stand0));
			
			mAnim.runtimeAnimatorController = lightControllor;
		}
		else if(weaponType == WeaponType.W_Ranged && m_weapon_Ranged != null)
		{
			m_weapon_Ranged.SetActive(true);
			
			//mAnim.SetTrigger(getAnimationName(AniType.ANI_Stand0));
			
			mAnim.runtimeAnimatorController = rangedConrollor;
		}
		
		updataAttackRange();

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.ReplaceWap ((int)weaponType);
	}
	
	public void initWeaponData()
	{
		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan
		   || CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)//过关
		{
			int level = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

			if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
			{
				level = 300000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
			}

			PveFunctionOpen pveFucTemplate = PveFunctionOpen.getPveFunctionOpenById (level);

			if(pveFucTemplate.defaultWeapon == 1)//heavy
			{
				if(weaponDateHeavy != null && BattleUIControlor.Instance().m_changeWeapon.btnHeavy.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Heavy);
				}
				else if(weaponDateLight != null && BattleUIControlor.Instance().m_changeWeapon.btnLight.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Light);
				}
				else if(weaponDateRanged != null && BattleUIControlor.Instance().m_changeWeapon.btnRange.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Ranged);
				}
			}
			else if(pveFucTemplate.defaultWeapon == 2)//light
			{
				if(weaponDateLight != null && BattleUIControlor.Instance().m_changeWeapon.btnLight.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Light);
				}
				else if(weaponDateHeavy != null && BattleUIControlor.Instance().m_changeWeapon.btnHeavy.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Heavy);
				}
				else if(weaponDateRanged != null && BattleUIControlor.Instance().m_changeWeapon.btnRange.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Ranged);
				}
			}
			else if(pveFucTemplate.defaultWeapon == 3)//range
			{
				if(weaponDateRanged != null && BattleUIControlor.Instance().m_changeWeapon.btnRange.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Ranged);
				}
				else if(weaponDateHeavy != null && BattleUIControlor.Instance().m_changeWeapon.btnHeavy.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Heavy);
				}
				else if(weaponDateLight != null && BattleUIControlor.Instance().m_changeWeapon.btnLight.gameObject.activeSelf == true)
				{
					initWeaponData(WeaponType.W_Light);
				}
			}
		}
		else
		{
			if(weaponDateHeavy != null && BattleUIControlor.Instance().m_changeWeapon.btnHeavy.gameObject.activeSelf == true)
			{
				initWeaponData(WeaponType.W_Heavy);
			}
			else if(weaponDateLight != null && BattleUIControlor.Instance().m_changeWeapon.btnLight.gameObject.activeSelf == true)
			{
				initWeaponData(WeaponType.W_Light);
			}
			else if(weaponDateRanged != null && BattleUIControlor.Instance().m_changeWeapon.btnRange.gameObject.activeSelf == true)
			{
				initWeaponData(WeaponType.W_Ranged);
			}
		}
		//refreshWeaponDate();
	}

	public void initWeaponDataEnemy()
	{
		if(weaponDateHeavy != null)
		{
			initWeaponData(WeaponType.W_Heavy);
		}
		else if(weaponDateLight != null)
		{
			initWeaponData(WeaponType.W_Light);
		}
		else if(weaponDateRanged != null)
		{
			initWeaponData(WeaponType.W_Ranged);
		}
	}

	private void initWeaponData(WeaponType weaponType)
	{
		if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().changeWeaponTo(weaponType, true);

		else BattleUIControlor.Instance().changeWeaponTo_enemy(weaponType, this);
	}

	public override void refreshWeaponDate()
	{
		if(weaponType == WeaponType.W_Heavy && weaponDateHeavy != null)
		{
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange, weaponDateHeavy.attackRange );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackSpeed, weaponDateHeavy.attackSpeed );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed, weaponDateHeavy.moveSpeed );
			
			refreshCRI(weaponDateHeavy.criX, weaponDateHeavy.criY, weaponDateHeavy.criSkillX, weaponDateHeavy.criSkillY);
		}
		else if(weaponType == WeaponType.W_Light && weaponDateLight != null)
		{
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange, weaponDateLight.attackRange );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackSpeed, weaponDateLight.attackSpeed );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed, weaponDateLight.moveSpeed );
			
			refreshCRI(weaponDateLight.criX, weaponDateLight.criY, weaponDateLight.criSkillX, weaponDateLight.criSkillY);
		}
		else if(weaponType == WeaponType.W_Ranged && weaponDateRanged != null)
		{
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange, weaponDateRanged.attackRange );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_attackSpeed, weaponDateRanged.attackSpeed );
			
			nodeData.SetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed, weaponDateRanged.moveSpeed );
			
			refreshCRI(weaponDateRanged.criX, weaponDateRanged.criY, weaponDateRanged.criSkillX, weaponDateRanged.criSkillY);
		}

		updataAttackRange ();

		foreach(Buff buff in buffs)
		{
			if(buff.buffType == AIdata.AttributeType.ATTRTYPE_moveSpeed)
			{
				addNavMeshSpeed(buff.getBuffValue());
			}
			else if(buff.buffType == AIdata.AttributeType.ATTRTYPE_attackSpeed
			        || buff.buffType == AIdata.AttributeType.ATTRTYPE_attackRange
			        || buff.buffType == AIdata.AttributeType.ATTRTYPE_Heavy_criX
			        || buff.buffType == AIdata.AttributeType.ATTRTYPE_Heavy_criY
			        || buff.buffType == AIdata.AttributeType.ATTRTYPE_Heavy_criSkillX
			        || buff.buffType == AIdata.AttributeType.ATTRTYPE_Heavy_criSkillY
			        )
			{
				float t_att = nodeData.GetAttribute( buff.buffType );

				nodeData.SetAttribute( buff.buffType, t_att + buff.getBuffValue() );
			}
		}

	}

	string playingAnimationName = "";

	public override void BaseUpdate()
	{
		try
		{
			string playing = IsPlaying();

			if(playingAnimationName.Equals(playing) == false)
			{
				playingAnimationName = playing;
			}

			if(autoMove == true)
			{
				character.Move(transform.forward * nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ) * Time.deltaTime);
			}

			bool auto = BattleControlor.Instance().autoFight == true;

			if(auto == true)
			{
				if(BattleUIControlor.Instance().pressedJoystick == true)
				{
					auto = false;
				}
			}

			if(auto == true || stance == Stance.STANCE_ENEMY)
			{
				updateWeapon();

				if(stance == Stance.STANCE_ENEMY) updateWeaponSkill ();

				base.BaseUpdate();

				return;
			}
			else
			{
				uptateTarget();

				updateAttack();

				for(int i = 0; i < skills.Count; i ++)
				{
					skills[i].upData();
				}
			}

			updateEnemysList();

			updataAttackRange();

			updateRevise();
		}
		catch(System.Exception e)
		{
			Debug.Log("ERROR! BUG ID: 31279");

			Debug.LogError("ERROR! BUG ID: 31279 \\n" + e.Message);
		}
	}

	private void updateAttack()
	{
		if (inAttackAuto == false) return;

		if(inTurning == false && Vector3.Distance(transform.position, inAttackPos) < nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange ) )
		{
			inAttackAuto = false;

			setNavMeshStop();

			StartCoroutine(nextFrameAttack());
		}
	}

	private IEnumerator nextFrameAttack()
	{
		yield return new WaitForSeconds (.1f);

		attack();
	}

	public void setAutoWeapon()
	{
		autoWeaponCD = 4f;
	}

	public void updateWeapon()
	{
		autoWeaponCD += Time.deltaTime;

		if (autoWeaponCD < 4.0f) return;

		if (isPlayingAttack () == true) return;

		if (isPlayingSkill () == true) return;

		autoWeaponCD = 0;

//		if(nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp ) < nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hpMax ) * .3f)
//		{
//			tryChangeWeapon(WeaponType.W_Ranged, weaponDateRanged, BattleUIControlor.Instance().m_changeWeapon.btnRange);
//
//			return;
//		}
		
		int count_1 = 0;

		int count_2 = 0;

		List<BaseAI> nodeList = stance == Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;
		
		foreach(BaseAI node in nodeList)
		{
			if(node == null || node.isAlive == false) continue;
			
			if(node.gameObject.activeSelf == false) continue;
			
			float length = Vector3.Distance(node.transform.position, transform.position);
			
			if(length < nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange ))
			{
				count_1 ++;
			}

			if(length < nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_Heavy_attackRange ))
			{
				count_2 ++;
			}
		}

		bool rangeSkillAble = false;

		if(stance == Stance.STANCE_SELF)
		{
			WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (maxLevel);

			if (template.b_skill_ranged_2 == false) 
			{
				rangeSkillAble = false;
			}
			else
			{
				rangeSkillAble = BattleUIControlor.Instance().cooldownRangeSkill_1.spriteCD.gameObject.activeSelf == false
					|| BattleUIControlor.Instance().cooldownRangeSkill_2.spriteCD.gameObject.activeSelf == false;
			}
		}
		else
		{
			WeaponSkillOpenTemplate template = WeaponSkillOpenTemplate.getWeaponSkillTemplate (maxLevel);

			if (template.b_skill_ranged_2 == false) 
			{
				rangeSkillAble = false;
			}
			else
			{
				rangeSkillAble = BattleUIControlor.Instance().cooldownRangeSkill_1_enemy.spriteCD.gameObject.activeSelf == false
					|| BattleUIControlor.Instance().cooldownRangeSkill_2_enemy.spriteCD.gameObject.activeSelf == false;
			}
		}

		if(count_2 == 0 && rangeSkillAble)
		{
			tryChangeWeapon(WeaponType.W_Ranged, weaponDateRanged, BattleUIControlor.Instance().m_changeWeapon.btnRange);
		}
		else
		{
			if(count_1 > 2)
			{
				tryChangeWeapon(WeaponType.W_Heavy, weaponDateHeavy, BattleUIControlor.Instance().m_changeWeapon.btnHeavy);
			}
			else
			{
				tryChangeWeapon(WeaponType.W_Light, weaponDateLight, BattleUIControlor.Instance().m_changeWeapon.btnLight);
			}
		}
	}

	private void tryChangeWeapon(WeaponType _weaponType, PlayerWeapon _playerWeapon, GameObject _weaponBtn)
	{
		if(weaponType != _weaponType && _playerWeapon != null)
		{
			if(stance == Stance.STANCE_SELF)
			{
				if(_weaponBtn.activeSelf == true)
				{
					BattleUIControlor.Instance().changeWeaponTo(_weaponType);
			
					skillCD = 0;
				}
			}
			else
			{
				BattleUIControlor.Instance().changeWeaponTo_enemy(_weaponType, this);

				skillCD = 0;
			}
		}
	}

	protected void updateWeaponSkill ()
	{
		skillCD += Time.deltaTime;

		if (skillCD < 1.5f) return;

		List<BaseAI> nodes = stance == Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;
		
		int count = 0;
		
		foreach(BaseAI node in nodes)
		{
			if(node == null || node.isAlive == false) continue;
			
			if(node.gameObject.activeSelf == false) continue;

			if(node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_hp) < 0) continue;

			if(node.nodeData.nodeType == NodeType.GEAR || node.nodeData.nodeType == NodeType.GOD || node.nodeData.nodeType == NodeType.NPC) continue;

			float length = Vector3.Distance(node.transform.position, transform.position);
			
			if(length < nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange ))
			{
				count ++;
			}
		}

		if (count == 0) return;

		bool flag = false;

		if(weaponType == WeaponType.W_Heavy)
		{
			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useDaoSkill_2();

				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useDaoSkill_2_Enemy(this);
				
				if(flag == true) skillCD = 1;
			}

			if(flag == true) return;

			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useDaoSkill_1();

				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useDaoSkill_1_Enemy(this);

				if(flag == true) skillCD = 1;
			}
		}
		else if(weaponType == WeaponType.W_Light)
		{
			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useQiangSkill_2();

				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useQiangSkill_2_Enemy(this);
				
				if(flag == true) skillCD = 1;
			}

			if(flag == true) return;

			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useQiangSkill_1();
				
				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useQiangSkill_1_Enemy(this);
				
				if(flag == true) skillCD = 1;
			}
		}
		else if(weaponType == WeaponType.W_Ranged)
		{
			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useGongSkill_2();
				
				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useGongSkill_2_Enemy(this);
				
				if(flag == true) skillCD = 1;
			}

			if(flag == true) return;

			if(stance == Stance.STANCE_SELF)
			{
				flag = BattleUIControlor.Instance().useGongSkill_1();
				
				if(flag == true) skillCD = 1;
			}
			else
			{
				flag = BattleUIControlor.Instance().useGongSkill_1_Enemy(this);
				
				if(flag == true) skillCD = 1;
			}
		}

		if (flag == true) return;

		if(stance == Stance.STANCE_SELF)
		{
			flag = BattleUIControlor.Instance().useMiBaoSkill();

			if(flag == true) skillCD = 1;
		}
		else
		{
			flag = BattleUIControlor.Instance().useMiBaoSkill_Enemy(this);
			
			if(flag == true) skillCD = 1;
		}
	}

	protected override void runaway(float time)
	{
		List<BaseAI> list = stance == Stance.STANCE_SELF ? BattleControlor.Instance ().selfNodes : BattleControlor.Instance ().enemyNodes;

		if (list.Count < 2) return;

		float length = 99999;

		BaseAI targetNode = null;

		foreach(BaseAI node in list)
		{
			float l = Vector3.Distance(node.transform.position, transform.position);

			if(l < length)
			{
				targetNode = node;

				length = l;
			}
		}

		if (targetNode == null) return;

		Vector3 tempFoward = targetNode.transform.forward;

		targetNode.transform.forward = transform.position - targetNode.transform.position;

		Vector3 targetPosition = targetNode.transform.position + targetNode.transform.forward * (-1f) * ((targetNode.radius + radius) * .6f);

		targetNode.transform.forward = tempFoward;

		inHover = true;
		
		setNavMeshDestination (targetPosition);
		
		iTween.ValueTo(gameObject, iTween.Hash(
			"from", Vector3.zero,
			"to", new Vector3(100, 0, 0),
			"delay", 0,
			"time", time,
			"easetype", iTween.EaseType.linear,
			"oncomplete", "onRunawayComplete"
			));
	}

	protected void updateRevise()
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		if(mill < 1000) return;
		
		//BattleReplayorWrite.Instance().addReplayKingRevise(nodeId, transform.position);
		
		//BattleReplayorWrite.Instance().addReplayKingForward(nodeId, transform.forward);
		
		tempTime = System.DateTime.Now;
	}

	public void move(Vector3 offset)
	{
		if (Vector3.Distance(offset, Vector3.zero) > .2f) DramaControllor.Instance ().closeYindao (20109);

		if (BattleControlor.Instance ().autoFight == true && BattleUIControlor.Instance().pressedJoystick == false) return;

		if(!isAlive || mAnim == null) 
		{
			mAnim.SetFloat("move_speed", 0);

			return;
		}

		string strPlaying = IsPlaying();

//		if(!inAttackAuto)
//		{
//			float l = Vector3.Distance(offset, Vector3.zero);
//
//			if(l < .2f)
//			{
//				mAnim.SetFloat("move_speed", 0);
//
//				return;
//			}
//		}

		bool flag = isPlayingSwing();

		if(flag == true && Vector3.Distance(offset, Vector3.zero) > .5f && moveable == true)
		{
			//mAnim.Play(getAnimationName(AniType.ANI_Stand0));

			swingEnd ();

			mAnim.Play("Run");

//			mAnim.ResetTrigger(getAnimationName(AniType.ANI_DODGE));
//
//			mAnim.ResetTrigger("skill_1");
//
//			mAnim.ResetTrigger("skill_2");
//
//			mAnim.ResetTrigger("skill_mibao");

			ResetHitCount();
		}
		else if(flag == false)
		{
			flag = isPlayingAttack () || isPlayingSkill ();
		}

		if(flag == true && moveableInAttack == false)
		{
			if (strPlaying.IndexOf ("XuanFengZhan") == -1)
			{
				return;
			}
		}

		if(!inAttackAuto)
		{
			mAnim.SetFloat("move_speed", Vector3.Distance(offset, Vector3.zero));
		}

		if(Vector3.Distance(offset, Vector3.zero) > 0.5f)
		{
			if(inAttackAuto == true)
			{
				inAttackAuto = false;

				setNavMeshStop();
			}
		}

		if(moveable == false)
		{
			if(Vector3.Distance(offset, Vector3.zero) > 0.5f)
			{
				transform.forward = offset;
			}

			if(offset == Vector3.zero && moving == true)
			{
				moving = false;
			}

			return;
		}

		//BattleReplayorWrite.Instance().addReplayPlayerMove(nodeId, offset);
		
		if(Vector3.Distance(Vector3.zero, offset) > 0)
		{
			transform.forward = offset;
		}
		
		if(character.enabled == true)
		{
			character.Move(offset * getNavMeshSpeed() * Time.deltaTime);
		}

		if (offset != Vector3.zero)
		{
			moving = true;
		}
		else if(moving)
		{
			moving = false;
		}
	}

	protected void attackForward()
	{
		float tempAlpha = 500;

		float tempAlphaDead = 500;

		BaseAI tempNode = null;

		BaseAI tempNodeDead = null;

		List<BaseAI> list = stance == Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;

		foreach(BaseAI t_node in list)
		{
			if(t_node == null
			   || t_node.gameObject.activeSelf == false
			   || Vector3.Distance(t_node.transform.position, transform.position) > nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange ) + 1.5f
			   || t_node.isAlive == false
			   || t_node.nodeData.nodeType == NodeType.NPC
			   || t_node.nodeData.nodeType == NodeType.GOD)
			{
				continue;
			}

			float alpha = Vector3.Angle(t_node.transform.position - transform.position, transform.forward);
			
			if(alpha < tempAlpha)
			{
				if(t_node.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp ) >= 0)
				{
					tempAlpha = alpha;

					tempNode = t_node;
				}
				else 
				{
					tempAlphaDead = alpha;

					tempNodeDead = t_node;
				}
			}
		}
		
		if(tempNode != null)
		{
			transform.forward = tempNode.transform.position - transform.position;
		}

		if(signShadow != null)
		{
			signShadow.LateUpdate();
		}
	}

	public void ResetHitCount()
	{
		comboable = true;

		b_resetHit = false;

		StartCoroutine (resetHitCountAction());

		//_ResetHitCount ();
	}

	IEnumerator resetHitCountAction()
	{
		float startTime = Time.realtimeSinceStartup;

		for(;;)
		{
			if(b_resetHit == true)
			{
				break;
			}

			float now = Time.realtimeSinceStartup;

			if(now - startTime > 1f)
			{
				break;
			}

			yield return new WaitForEndOfFrame();
		}

		_ResetHitCount ();
	}

	void _ResetHitCount()
	{
		if (b_resetHit == true)
		{
			return;
		}

		for(int i = 0; i < 4; i++)
		{
			mAnim.SetBool("attack_" + i, false);
		}

		hitCount = 0;
	}

	public void dodge(Vector3 forward)
	{
		swingEnd ();
		
		ResetHitCount ();

		if(Vector3.Distance(forward, Vector3.zero) > .1f) transform.forward = forward;

		mAnim.SetTrigger (getAnimationName(AniType.ANI_DODGE));
	}

	public void attack()
	{
		if (comboable == false) return;

		if (isIdle == true) return;

		string playing = IsPlaying ();

		if (playing.IndexOf(getAnimationName( AniType.ANI_BATC)) != -1) return;

		if (playing.IndexOf(getAnimationName( AniType.ANI_BATCDown)) != -1) return;

		//BattleReplayorWrite.Instance().addReplayKingAttack(nodeId, weaponFoward);

		bool checkWeapon = checkWeaponRange ();

		if(checkWeapon == false) return;

		setNavMeshStop ();

		{
			//comboable = false;

			attackTempTime = System.DateTime.Now;

			mAnim.SetBool(getAnimationName(AniType.ANI_Attack_0), true);

			attackForward ();

		}
	}

	//return: true-attack, false-move
	private bool checkWeaponRange()
	{
		if (moving == true) return true;

		BaseAI tempNode = null;

		float tempLength = 12;

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node == null 
			   || node.isAlive == false 
			   || node.gameObject.activeSelf == false
			   || node.nodeData.nodeType == NodeType.NPC
			   || node.nodeData.nodeType == NodeType.GOD) 
			{
				continue;
			}

			float length = Vector3.Distance(node.transform.position, transform.position);

			if(length > tempLength) continue;

			if(length < nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange ))
			{
				return true;
			}

			tempNode = node;

			tempLength = length;
		}

		if(tempNode != null)
		{
			Vector3 tempFoward = tempNode.transform.forward;

			tempNode.transform.forward = transform.position - tempNode.transform.position;

			inAttackPos = tempNode.transform.position + tempNode.transform.forward * (nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackRange) * .8f );

			setNavMeshDestination(inAttackPos);

			tempNode.transform.forward = tempFoward;

			inAttackAuto = true;

			mAnim.SetFloat("move_speed", 10);

			return false;
		}

		return true;
	}

	public void updateAttackSpeed()
	{
		mAnim.speed = 1f / nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_attackSpeed );
	}

	public void attackStart()
	{
		moveableInAttack = false;

//		if(hitCount < 3) 
//		{
			comboable = false;
//		}

		b_resetHit = true;

		mAnim.SetBool("attack_" + hitCount, false);

		if(hitCount < 3)
		{
			hitCount ++;
		}
		else
		{
			hitCount = 0;
			
			//BattleUIControlor.Instance().attackJoystick.reset();
		}
	}

	private void attackMove(int actionId)
	{
		KingMovementTemplate template = KingMovementTemplate.getKingMovementById (actionId);

		iTween.EaseType easeType = iTween.EaseType.linear;

		if(actionId == 401)
		{
			easeType = iTween.EaseType.easeOutSine;
		}

		moveAction (transform.forward * template.length + transform.position, easeType, template.time, false);

//		iTween.ValueTo(gameObject, iTween.Hash(
//			"name", "action_" + actionId,
//			"from", transform.position,
//			"to", transform.position + gameObject.transform.TransformDirection( new Vector3(0, 0, template.length)),
//			"delay", 0,
//			"time", template.time,
//			"easeType", iTween.EaseType.linear,
//			"onupdate", "PositonTween"
//			));

		if( AttackMoveWillPlayEffect( actionId ) ){
			BattleEffectControllor.Instance().PlayEffect( GetAttackMoveEffectId(), gameObject );
		}
	}

	public static int GetAttackMoveEffectId(){
		return 23;
	}

	public static bool AttackMoveWillPlayEffect( int p_action_id ){
		if( p_action_id == 115 ){
			return true;
		}

		return false;
	}

	public override void createArrow(int _actionId)
	{
		actionId = _actionId;

		if(_actionId == 260)//穿云箭
		{
			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200031);
			
			KingArrow arrow = KingArrow.createArrow(this, BattleControlor.AttackType.SKILL_ATTACK, 68);

			arrow.continueArrow = true;
			
			arrow.shanghaixishu = skillTemplate.value1;
			
			arrow.gudingshanghai = 0;

			arrow.aid = 260;

			return;
		}
		else if(_actionId == 261)//寒冰箭
		{
			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200032);

			Vector3 tempForward = transform.forward;

			for(int i = -4; i <= 4; i++)
			{
				transform.forward = tempForward;

				transform.localEulerAngles += new Vector3(0, 8f * i, 0);

				KingArrow arrow = KingArrow.createArrow(this, BattleControlor.AttackType.SKILL_ATTACK, 10310);

				arrow.shanghaixishu = skillTemplate.value1;

				arrow.gudingshanghai = 0;

				arrow.aid = 261;
			}

			transform.forward = tempForward;

			return;
		}

		bool attacking = isPlayingAttack ();
		
		if(attacking == true)
		{
			//if(hitCount < 3) 
			{
				comboable = true;
			}
		}

		base.createArrow (_actionId);
	}

	public override void arrowAttackCallback(int _aid, BaseAI defenderNode)
	{
		if(_aid == 260)//穿云箭
		{
//			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200031);
//
//			transform.forward = defenderNode.transform.position - transform.position;
//
//			Vector3 targetP = defenderNode.transform.position + transform.forward * skillTemplate.value2;
//
//			StartCoroutine(defenderNode.attackedMovement(0.05f, targetP, iTween.EaseType.easeOutExpo, 0.2f));
//
//			defenderNode.mAnim.SetTrigger(defenderNode.getAnimationName(AniType.ANI_BATCDown));
		}
		else if(_aid == 261)//寒冰箭
		{
			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200032);

			Buff.createBuff(defenderNode, AIdata.AttributeType.ATTRTYPE_moveSpeed, -defenderNode.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed ) * skillTemplate.value2, skillTemplate.value3);

			Buff.createBuff(defenderNode, AIdata.AttributeType.ATTRTYPE_Ice, 10, 1f);

			BattleEffectControllor.Instance ().PlayEffect (50000, defenderNode.gameObject);

			BattleEffectControllor.Instance ().PlayEffect (303, defenderNode.gameObject);
		}
	}

	public bool attackBaseAI(BaseAI node, float hpValue, bool cri)
	{
		if(node.body != null) {
			EffectTool.Instance.SetHittedEffect (node.body);
		}
		else {
			EffectTool.Instance.SetHittedEffect (node.gameObject); 
		}

		if (weaponType == WeaponType.W_Heavy)
		{

		}

		bool f = false;

		if(actionId == 103
		   || actionId == 106
		   || actionId == 107
		   || actionId == 109

		   )
		{
			Shake(KingCamera.ShakeType.Forward);
		}
		else if( actionId == 111
		        || actionId == 113
		        || actionId == 117)
		{
			Shake(KingCamera.ShakeType.Cri);
		}

		if(actionId == 250)//旋风斩
		{
			Shake(KingCamera.ShakeType.Cri);
		}

		if(cri == true)
		{
			if(actionId == 200 //大刀第一击
			   || actionId == 201//大刀第二击
			   || actionId == 202//大刀第三击
			   || actionId == 101//双刀第一击
			   || actionId == 103//双刀第二击
			   || actionId == 105//双刀第三击
			   || actionId == 106//双刀第四击
			   )
			{
				Shake(KingCamera.ShakeType.Cri);
			}
		}

		if(actionId >= 120 && actionId <= 129)
		{
			Shake(KingCamera.ShakeType.Cri);
		}
		
		if(actionId == 140 || actionId == 147)//无敌斩
		{
			Shake(KingCamera.ShakeType.Cri);
		}

		if (actionId == 203)//大刀最后一击
		{
			Shake(KingCamera.ShakeType.Vertical);
		}

		if(actionId == 260)//穿云箭
		{
			SkillTemplate st = SkillTemplate.getSkillTemplateById(200031);

			Vector3 tempForward = transform.forward;

			transform.forward = node.transform.position - transform.position;

			Vector3 targetP = node.transform.position + transform.forward * st.value2;

			StartCoroutine(node.attackedMovement(0.05f, targetP, iTween.EaseType.easeOutExpo, 0.2f));

			transform.forward = tempForward;

			f = node.beatDown(102);
		}

		if(node.IsPlaying(getAnimationName(AniType.ANI_BATCDown)) == false)
		{
			if(actionId == 117
			   || actionId == 116
			   || actionId == 130
			   || actionId == 148)//不会存在，已废弃
			{
				Debug.LogError("AAAAAAAAAAAAAAAA  " + actionId);

				node.beatDown(100);

				f = true;
			}
			else if(actionId == 203)//大刀最后一击
			{
				node.beatDown(100);

				Shake(KingCamera.ShakeType.Vertical);

				f = true;
			}
		}

		bool have = KingCrashTemplate.haveKingCrashById(actionId);

		if(have == true)
		{
			KingCrashTemplate template = KingCrashTemplate.getKingCrashById(actionId);
			
			float v = nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp ) > 0 ? template.length : 4f;
			
			float t = nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp ) > 0 ? template.time : 1f;
			
			float d = nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp ) > 0 ? template.delay : 0f;
			
			Vector3 e = node.transform.position + transform.forward * v;
			
			node.curCrashData = template;
			
			StartCoroutine(node.attackedMovement(d, e, iTween.EaseType.easeOutCubic, t));
		}

		if(QualityTool.GetBool(QualityTool.CONST_CHARACTER_HITTED_FX))
		{
			BattleEffectControllor.Instance ().PlayEffect (1010, node.transform.position + new Vector3(0, 1.2f, 0), node.transform.forward);
		}

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.AttackNum ((int)hpValue);

		return f;
	}

	public override bool isPlayingAttack()
	{
		if (isIdle == true) return true;

		string playing = IsPlaying ();

		if (playing.IndexOf ("attack_") != -1) return true;

		if (playing.IndexOf(getAnimationName( AniType.ANI_BATC)) != -1) return true;

		if (playing.IndexOf(getAnimationName( AniType.ANI_BATCDown)) != -1) return true;

		if (playing.IndexOf(getAnimationName(AniType.ANI_DODGE)) != -1) return true;

		string nextPlay = nextPlaying ();

		if (nextPlay.IndexOf ("attack_") != -1) return true;
		
		if (nextPlay.IndexOf(getAnimationName( AniType.ANI_BATC)) != -1) return true;
		
		if (nextPlay.IndexOf(getAnimationName( AniType.ANI_BATCDown)) != -1) return true;
		
		if (nextPlay.IndexOf(getAnimationName(AniType.ANI_DODGE)) != -1) return true;

		return false;
	}

	public void attackDone(int actionId)
	{
		//if (BattleControlor.Instance ().inDrama == true) return;

		mAnim.speed = 1f;

		//comboable = true;

		actionId = 0;

		if(actionId == 10)
		{
			setNavEnabled(true);

			character.enabled = true;
		}
	}

	public override bool isPlayingSkill()
	{
		//if (m_iUseSkillIndex != -1) return true;

		if (m_sPlaySkillAnimation.Length > 0) return true;

		string playing = IsPlaying ();

		if (playing.IndexOf ("skill") != -1) return true;

		if (playing.IndexOf ("XuanFengZhan") != -1) return true;

		string nextPlying = nextPlaying ();

		if (nextPlying.IndexOf ("skill") != -1) return true;

		if (nextPlying.IndexOf ("XuanFengZhan") != -1) return true;

		return false;
	}

	public void swingStart()
	{
		str_swingState = IsPlaying();
	}

	public void swingEnd()
	{
		str_swingState = "";
	}

	public override bool isPlayingSwing()
	{
		string curPlaying = IsPlaying ();

		string nextPlying = nextPlaying ();

		if (m_sPlaySkillAnimation.Length > 0) return false;

		if (nextPlying.IndexOf ("skill") != -1 && nextPlying.IndexOf ("_done") == -1) return false;

		if (nextPlying.IndexOf ("XuanFengZhan") != -1 && nextPlying.IndexOf ("_done") == -1) return false;

		if (nextPlying.IndexOf ("attack_") != -1 && nextPlying.IndexOf ("_done") == -1) return false;
		
		if (nextPlying.IndexOf(getAnimationName( AniType.ANI_BATC)) != -1 && nextPlying.IndexOf ("_done") == -1) return false;
		
		if (nextPlying.IndexOf(getAnimationName( AniType.ANI_BATCDown)) != -1 && nextPlying.IndexOf ("_done") == -1) return false;
		
		if (nextPlying.IndexOf(getAnimationName(AniType.ANI_DODGE)) != -1 && nextPlying.IndexOf ("_done") == -1) return false;

		if (str_swingState.Equals(curPlaying)) return true;

		return base.isPlayingSwing();
	}

	private static DevelopUtility.PlayAttackEffectReturn m_play_attack_effect = new DevelopUtility.PlayAttackEffectReturn();

	public static DevelopUtility.PlayAttackEffectReturn GetPlayAttackEffectParams( int attackId ){
		m_play_attack_effect.m_will_play_effect = false;

		if( attackId == 100 )
		{
			m_play_attack_effect.Set( 25, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 101)
		{
			m_play_attack_effect.Set( 26, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 102)
		{
			m_play_attack_effect.Set( 27, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 103)
		{
			m_play_attack_effect.Set( 28, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 104)
		{
			m_play_attack_effect.Set( 28, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 105)
		{
			m_play_attack_effect.Set( 28, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 106)
		{
			m_play_attack_effect.Set( 31, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 107)
		{
			m_play_attack_effect.Set( 33,
			                         DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT );
		}
		else if(attackId == 108)
		{
			m_play_attack_effect.Set( 26,
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 115)
		{
			m_play_attack_effect.Set( 34, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 116)
		{
			m_play_attack_effect.Set( 35,
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 120)
		{
			m_play_attack_effect.Set( 37,
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 121)
		{
			m_play_attack_effect.Set( 38, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 122)
		{
			m_play_attack_effect.Set( 39, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		///////////////
		/// 
		/// 
		/// 
		else if(attackId == 140)
		{
			//BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)252, transform.position + transform.forward, transform.forward);
			
			m_play_attack_effect.Set( 42, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRNASFORM_POSITION_PLUS_TRANSFORM_FORWARD,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 141)
		{
			Vector3 tempR = new Vector3(Random.value, Random.value, Random.value).normalized;
			
			//Vector3 tempR = new Vector3(0, 0, 0);
			
			m_play_attack_effect.Set( 253, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRNASFORM_POSITION_PLUS_TRANSFORM_FORWARD,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.CUSTOM,
			                         tempR );
		}
		else if(attackId == 200)
		{
			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200012);
			
			m_play_attack_effect.Set( 150, 
			                         DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT,
			                         skillTemplate.value1 );
		}
		else if(attackId == 201)
		{
			m_play_attack_effect.Set( 152,
			                         DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT );
		}
		else if(attackId == 202)
		{
			m_play_attack_effect.Set( 63, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 203)//穿云箭
		{
			m_play_attack_effect.Set( 302, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 204)//冰箭
		{
			m_play_attack_effect.Set( 44, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 205)//冲锋落地
		{
			m_play_attack_effect.Set( 151, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 300)
		{
			m_play_attack_effect.Set( 100, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 301)
		{
			m_play_attack_effect.Set( 101, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 302)
		{
			m_play_attack_effect.Set( 102, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 303)
		{
			m_play_attack_effect.Set( 103, 
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD );
		}
		else if(attackId == 304)//弓箭普通攻击-集气特效
		{
			m_play_attack_effect.Set( 300,
			                         DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT );
		}
		else if(attackId == 305)//弓箭普通攻击-集气特效
		{
			m_play_attack_effect.Set( 299,
			                         DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT );
		}
		else if(attackId == 500)
		{
			m_play_attack_effect.Set( 50071,
			                         DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION,
			                         DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD);
		
			BattleControlor.Instance().getKing().gameCamera.Shake(KingCamera.ShakeType.Vertical);
		}

		return m_play_attack_effect;
	}

	protected override void playAttackEffect(int attackId)
	{
		if (BattleControlor.Instance ().inDrama == true) return;

		m_play_attack_effect =  GetPlayAttackEffectParams( attackId );

		if( m_play_attack_effect.m_will_play_effect ){
			GameObject t_gb = null;

			switch( m_play_attack_effect.m_gb_type ){
			case DevelopUtility.PlayAttackEffectReturn.GameObjectType.NONE:
				t_gb = null;
				break;

			case DevelopUtility.PlayAttackEffectReturn.GameObjectType.GAMEOBJECT:
				t_gb = gameObject;
				break;

			case DevelopUtility.PlayAttackEffectReturn.GameObjectType.WEAPON_RANGE_TRANSFORM_PARENT_GAMEOBJECT:
				t_gb = m_weapon_Ranged.transform.parent.gameObject;
				break;
			}

			Vector3 t_position = Vector3.zero;;

			switch( m_play_attack_effect.m_position_type ){
			case DevelopUtility.PlayAttackEffectReturn.PositionType.ZERO:
				t_position = Vector3.zero;
				break;

			case DevelopUtility.PlayAttackEffectReturn.PositionType.TRANSFORM_POSITION:
				t_position = transform.position;
				break;

			case DevelopUtility.PlayAttackEffectReturn.PositionType.TRNASFORM_POSITION_PLUS_TRANSFORM_FORWARD:
				t_position = transform.position + transform.forward;
				break;
			}

			Vector3 t_forward = Vector3.zero;

			switch( m_play_attack_effect.m_forward_type ){
			case DevelopUtility.PlayAttackEffectReturn.ForwardType.ZERO:
				t_forward = Vector3.zero;
				break;

			case DevelopUtility.PlayAttackEffectReturn.ForwardType.TRANSFORM_FORWARD:
				t_forward = transform.forward;
				break;

			case DevelopUtility.PlayAttackEffectReturn.ForwardType.CUSTOM:
				t_forward = m_play_attack_effect.m_custom_forward;
				break;
			}

			// debug use
			if( Console_SetBattleFieldFx.IsEnableAttackFx() ){
				BattleEffectControllor.Instance ().PlayEffect(
					m_play_attack_effect.m_effect_id, 
					t_gb, 
					m_play_attack_effect.m_time,
					t_position,
					t_forward );
			}
//			else{
//				EffectIdTemplate t_et = EffectTemplate.getEffectTemplateByEffectId ( m_play_attack_effect.m_effect_id );
//
//				Debug.Log( "Should Play Effect: " + m_play_attack_effect.m_effect_id + " - " + t_et.path );
//			}

			return;
		}

		// debug use
		if( !Console_SetBattleFieldFx.IsEnableAttackFx() ){
			return;
		}

//		public void Set( int p_effect_id, POSITION_TYPE p_pos_type, FORWARD_TYPE p_forward_type ){
//			Set( p_effect_id, p_pos_type, p_forward_type, Vector3.zero, GAMEOBJECT_TYPE.none );
//		}
//		
//		public void Set( int p_effect_id, POSITION_TYPE p_pos_type, FORWARD_TYPE p_forward_type, Vector3 p_custom_forward ){
//			Set( p_effect_id, p_pos_type, p_forward_type, p_custom_forward, GAMEOBJECT_TYPE.none );
//		}



//		if(attackId == 100)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)25, transform.position, transform.forward);
//		}
//		else if(attackId == 101)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)26, transform.position, transform.forward);
//		}
//		else if(attackId == 102)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)27, transform.position, transform.forward);
//		}
//		else if(attackId == 103)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)28, transform.position, transform.forward);
//		}
//		else if(attackId == 104)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_4, transform.position, transform.forward);
//		}
//		else if(attackId == 105)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_4, transform.position, transform.forward);
//		}
//		else if(attackId == 106)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_SHIZI, transform.position, transform.forward);
//		}
//		else if(attackId == 107)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_XUANFENGZHAN, gameObject);
//		}
//		else if(attackId == 108)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_2, transform.position, transform.forward);
//		}
//		else if(attackId == 115)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_SHANGTIAO_ZUO, transform.position, transform.forward);
//		}
//		else if(attackId == 116)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_SHANGTIAO_YOU, transform.position, transform.forward);
//		}
//		else if(attackId == 120)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_LUANWU_1, transform.position, transform.forward);
//		}
//		else if(attackId == 121)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_LUANWU_2, transform.position, transform.forward);
//		}
//		else if(attackId == 122)
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_LIGHT_LUANWU_3, transform.position, transform.forward);
//		}
//		else if(attackId == 140)
//		{
//			//BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)252, transform.position + transform.forward, transform.forward);
//		
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_SKILL_HUAHEN, transform.position + transform.forward, transform.forward);
//		}
//		else if(attackId == 141)
//		{
//			Vector3 tempR = new Vector3(Random.value, Random.value, Random.value).normalized;
//
//			//Vector3 tempR = new Vector3(0, 0, 0);
//
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)253, transform.position + transform.forward, tempR);
//		}
//		else if(attackId == 200)
//		{
//			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200012);
//			
//			BattleEffectControllor.Instance ().PlayEffect ((BattleEffectControllor.EffectType)150, gameObject, skillTemplate.value1);
//		}
//		else if(attackId == 201)
//		{
//			BattleEffectControllor.Instance ().PlayEffect ((BattleEffectControllor.EffectType)152, gameObject);			
//		}
//		else if(attackId == 202)
//		{
//			BattleEffectControllor.Instance ().PlayEffect (BattleEffectControllor.EffectType.EFFECT_KING_JI_SHE, transform.position, transform.forward);			
//		}
//		else if(attackId == 203)//穿云箭
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)302, m_weapon_Ranged.transform.parent.gameObject);
//		}
//		else if(attackId == 204)//冰箭
//		{
//			BattleEffectControllor.Instance().PlayEffect(BattleEffectControllor.EffectType.EFFECT_KING_CESHI_SKILL_QIXUAN, gameObject);
//		}
//		else if(attackId == 205)//冲锋落地
//		{
//			BattleEffectControllor.Instance ().PlayEffect ((BattleEffectControllor.EffectType)151, transform.position, transform.forward);
//		}
//		else if(attackId == 300)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)100, transform.position, transform.forward);
//		}
//		else if(attackId == 301)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)101, transform.position, transform.forward);
//		}
//		else if(attackId == 302)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)102, transform.position, transform.forward);
//		}
//		else if(attackId == 303)
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)103, transform.position, transform.forward);
//		}
//		else if(attackId == 304)//弓箭普通攻击-集气特效
//		{
//			BattleEffectControllor.Instance().PlayEffect((BattleEffectControllor.EffectType)300, gameObject);
//		}



		if(attackId == 150)//轻武器二技能
		{
			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200022);

			float length = skillTemplate.value1;

			float time = skillTemplate.value2;

			List<BaseAI> nodeList = stance == Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;

			BaseAI focusNode = null;

			foreach(BaseAI node in nodeList)
			{
				if(node == null || node.isAlive == false || node.gameObject.activeSelf == false) continue;

				float _length = Vector3.Distance(transform.position, node.transform.position);

				if(_length > length) continue;

				if(focusNode == null)
				{
					focusNode = node;
				
					continue;
				}

				if((int)node.nodeData.nodeType > (int)focusNode.nodeData.nodeType)
				{
					focusNode = node;
					
					continue;
				}
				else if((int)node.nodeData.nodeType == (int)focusNode.nodeData.nodeType)
				{
					if(node.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp) > focusNode.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_hp))
					{
						focusNode = node;
						
						continue;
					}
				}
			}

			if(focusNode != null)
			{
				Buff buff = Buff.createBuff(focusNode, AIdata.AttributeType.ATTRTYPE_Focus, 5, time);

				buff.setEffect(BattleEffectControllor.Instance().PlayEffect(104, focusNode.gameObject, time));
			
				//Buff.createBuff(focusNode, AIdata.AttributeType.ATTRTYPE_weaponReduction_Light, skillTemplate.value3, time);

				FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(this, focusNode, skillTemplate.value3, 0, false);

				Buff.createBuff(focusNode, AIdata.AttributeType.ATTRTYPE_hpDelay, fbp.Float, time);
			}
		}

	}

	public void createGhost(int ghostId)
	{
		GameObject gc = (GameObject)Instantiate (gameObject);
		
		gc.transform.parent = transform.parent;
		
		gc.transform.position = transform.position;
		
		gc.transform.localScale = transform.localScale;
		
		Animator aa = (Animator)gc.GetComponent ("Animator");
		
		Destroy (aa);
		
		KingControllor kk = (KingControllor)gc.GetComponent ("KingControllor");
		
		Destroy (kk.shadowObject);
		
		//Destroy (kk.shadowObject_2);
		
		Destroy (kk.bloodbar.gameObject);
		
		Destroy (kk);
		
		Component[] ki = gc.GetComponentsInChildren (typeof(iTween));
		
		foreach(Component com in ki)
		{
			Destroy(com);
		}
		
		//ReplayNode rr = (ReplayNode)gc.GetComponent ("ReplayNode");
		
		//Destroy (rr);
		
		CharacterController cc = (CharacterController)gc.GetComponent("CharacterController");
		
		Destroy(cc);
		
		NavMeshAgent nn = (NavMeshAgent)gc.GetComponent ("NavMeshAgent");
		
		Destroy (nn);
		
		SphereCollider co = (SphereCollider)gc.GetComponent("SphereCollider");
		
		Destroy(co);
		
		Component[] ws = gc.GetComponentsInChildren (typeof(KingWeapon));
		
		foreach(Component ww in ws)
		{
			Destroy(ww);
		}
		
		KingGhost gg = (KingGhost)gc.AddComponent <KingGhost>();
		
		gg.init (ghostId, 0.3f);
	}

	public void winActionStart()
	{
		BattleUIControlor.Instance().layerFight.SetActive (false);
		
		gameCamera.dark_2 ();
		
		gameCamera.CameraChange (new Vector4(16, -transform.localEulerAngles.y, 3.5f, 1.3f));
	}

	public void winActionDone()
	{
		StartCoroutine (winActionDoneAction());
	}

	IEnumerator winActionDoneAction()
	{
		yield return new WaitForSeconds (1.8f);

		gameCamera.resetCamera ();

		yield return new WaitForSeconds (2.2f);

		BattleUIControlor.Instance().showResult(BattleControlor.Instance().result);
	}

	private void cameraPositonTween(Vector3 p_offset)
	{
		gameCamera.CameraChange (p_offset, gameCamera.getCamRotation());
	}

	public bool useDaoSkill_1()
	{
		swingEnd ();

		comboable = false;

		mAnim.SetTrigger ("skill_1");

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.UseSkill (200011, nodeId);

		checkSkillDrama (200011);

		return true;
	}

	public bool useDaoSkill_2()
	{
//		mAnim.SetTrigger ("skill_2");
//
//		if (BattleControlor.Instance ().achivement != null)
//			BattleControlor.Instance ().achivement.UseSkill (200012, nodeId);

		swingEnd ();

		comboable = false;

		kingSkillHeavy_2.template.zhudong = false;

		int tempTimePeriod = 0;

		tempTimePeriod = tempTimePeriod ^ kingSkillHeavy_2.template.timePeriod;
		kingSkillHeavy_2.template.timePeriod = tempTimePeriod ^ kingSkillHeavy_2.template.timePeriod;
		tempTimePeriod = tempTimePeriod ^ kingSkillHeavy_2.template.timePeriod;

		kingSkillHeavy_2.template.timePeriod = 0;

		kingSkillHeavy_2.m_isDeadOverSkill = 1;

		kingSkillHeavy_2.upData ();
		
		kingSkillHeavy_2.template.zhudong = true;

		kingSkillHeavy_2.template.timePeriod = tempTimePeriod;

		kingSkillHeavy_2.m_isDeadOverSkill = 0;

		return true;
	}

	public void useQiangSkill_1()
	{
		swingEnd ();

		comboable = false;

		mAnim.SetTrigger ("skill_1");

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.UseSkill (200021, nodeId);

		checkSkillDrama (200021);
	}

	public void useQiangSkill_2()
	{
		swingEnd ();

		//comboable = false;

		mAnim.SetTrigger ("skill_2");

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.UseSkill (200022, nodeId);

		checkSkillDrama (200022);
	}

	public bool useGongSkill_1()
	{
		swingEnd ();

		comboable = false;

		mAnim.SetTrigger ("skill_1");

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.UseSkill (200031, nodeId);

		checkSkillDrama (200031);

		return true;
	}
	
	public bool useGongSkill_2()
	{
		swingEnd ();

		comboable = false;

		mAnim.SetTrigger ("skill_2");

		if (BattleControlor.Instance ().achivement != null)
			BattleControlor.Instance ().achivement.UseSkill (200032, nodeId);

		checkSkillDrama (200032);

		return true;
	}

	public void useMiBaoSkill()
	{
		swingEnd ();

		if (kingSkillMibao == null) return;

		kingSkillMibao.template.zhudong = false;

		kingSkillHeavy_2.m_isDeadOverSkill = 1;

		kingSkillMibao.upData ();

		kingSkillMibao.template.zhudong = true;

		kingSkillHeavy_2.template.zhudong = true;
		
		kingSkillHeavy_2.m_isDeadOverSkill = 0;

		if (kingSkillMibao.m_isUseThisSkill) 
		{
			comboable = false;

			for(int i = 0; i < 4; i++)
			{
				mAnim.SetBool("attack_" + i, false);
			}
			
			hitCount = 0;
		}
	}

	public void refreshCDTime(int id)
	{
		if(id == 11)//八荒烈日
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownHeavySkill_1.refreshCDTime ();
		
			else BattleUIControlor.Instance().cooldownHeavySkill_1_enemy.refreshCDTime ();
		}
		else if(id == 12)//乾坤斗转
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownHeavySkill_2.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownHeavySkill_2_enemy.refreshCDTime ();
		}
		else if(id == 21)//绝影星光斩
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownLightSkill_1.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownLightSkill_1_enemy.refreshCDTime ();
		}
		else if(id == 22)//血祭烙印
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownLightSkill_2.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownLightSkill_2_enemy.refreshCDTime ();
		}
		else if(id == 31)//追星箭
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownRangeSkill_1.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownRangeSkill_1_enemy.refreshCDTime ();
		}
		else if(id == 32)//寒冰箭
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownRangeSkill_2.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownRangeSkill_2_enemy.refreshCDTime ();
		}
		else if(id == 100)//秘宝技能
		{
			if(stance == Stance.STANCE_SELF) BattleUIControlor.Instance().cooldownMibaoSkill.refreshCDTime ();
			
			else BattleUIControlor.Instance().cooldownMibaoSkill_enemy.refreshCDTime ();
		}
	}

	public void HeavySkill_1_Start()
	{
		if(chongfengControllor == null)
		{
			GameObject chongfengObject = new GameObject ();
		
			chongfengObject.transform.parent = transform.parent;

			chongfengObject.transform.position = transform.position;

			chongfengObject.transform.eulerAngles = transform.eulerAngles;

			chongfengObject.transform.localScale = new Vector3(1, 1, 1);

			chongfengControllor = chongfengObject.AddComponent<CharacterController>();

			chongfengControllor.radius = 0;

			chongfengControllor.center = new Vector3(0, 1.6f, 0);

			chongfengControllor.height = 3f;
		}

		StartCoroutine (chongFengAction());
	}

	private IEnumerator chongFengAction()
	{
		attackForward ();

		transform.localEulerAngles += new Vector3 (0, 2f, 0);

		SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById (200012);
		
		float t = skillTemplate.value1;

		float sp = skillTemplate.value3;
		
		chongfengControllor.transform.position = transform.position;

		foreach(BaseAI a in BattleControlor.Instance().enemyNodes)
		{
			a.setNavMeshRadiusTemp(0);

			a.character.radius = 0;
		}

		foreach(BaseAI a in BattleControlor.Instance().selfNodes)
		{
			a.setNavMeshRadiusTemp(0);
			
			a.character.radius = 0;
		}

		yield return new WaitForEndOfFrame ();

		chongfengControllor.Move (transform.forward * (t * sp));
		
		yield return new WaitForEndOfFrame ();

		foreach(BaseAI a in BattleControlor.Instance().enemyNodes)
		{
			a.setNavMeshRadiusTemp(a.radius);
			
			a.character.radius = a.radius;
		}
		
		foreach(BaseAI a in BattleControlor.Instance().selfNodes)
		{
			a.setNavMeshRadiusTemp(a.radius);
			
			a.character.radius = a.radius;
		}

		iTween.ValueTo(gameObject, iTween.Hash(
			"name", "action_" + actionId,
			"from", transform.position,
			"to", chongfengControllor.transform.position,
			"delay", 0,
			"time", t,
			"easeType", iTween.EaseType.easeOutQuad,
			"onupdate", "PositonTweenWithoutCharactor"
			));
		
		mAnim.speed = 0.4f / t;
	}

	public void HeavySkill_1_End()
	{
		mAnim.speed = 1f;
	}

	public void HeavySkill_2_Start()
	{
		autoMove = true;
	}

	public void HeavySkill_2_End()
	{
		autoMove = false;
	}

	public override void setWeaponTriggerTrue(int _aid)
	{
		if (BattleControlor.Instance ().inDrama == true) return;

		int hand = _aid / 1000;
		
		actionId = _aid % 1000;

		if(actionId >= 140 && actionId <= 151)
		{
			playAttackEffect(140);

			//playAttackEffect(141);

			actionId = _aid % 1000;

			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200021);

			KingSkillWuDiZhan wudizhan = (KingSkillWuDiZhan)gameObject.GetComponent("KingSkillWuDiZhan");

			int count = 0;

			if(wudizhan.curNode != null)
			{
				foreach(Buff buff in wudizhan.curNode.buffs)
				{
					if(buff.buffType == AIdata.AttributeType.ATTRTYPE_hp
					   && buff.getBuffValue() == 0)
					{
						count++;
					}
				}

				count = count > (int)skillTemplate.value5 ? (int)skillTemplate.value5 : count;

				FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(
					this, 
					wudizhan.curNode, 
					skillTemplate.value2 + count * skillTemplate.value4, 
					0
					);

				foreach(Buff buff in wudizhan.curNode.buffs)
				{
					if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
					{
						attackHp(this, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK);
						
						fbp.Float = buff.supplement.m_fValue1 * fbp.Float;
						
						break;
					}
				}

				attackHp(wudizhan.curNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK);

				Buff.createBuff(wudizhan.curNode, AIdata.AttributeType.ATTRTYPE_hp, 0, 5f);

				Shake(KingCamera.ShakeType.Cri);
			}
		}
		else if(_aid == 3250)//旋风斩
		{
			actionId = _aid % 1000;

			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200011);

			weaponBox_3.gameObject.SetActive(false);

			weaponBox_3.gameObject.SetActive(true);

			weaponBox_3.setTriggerableSkill(true, skillTemplate.value1, 0);
		}
		else if(_aid == 3260)//冲锋
		{
			actionId = _aid % 1000;

			SkillTemplate skillTemplate = SkillTemplate.getSkillTemplateById(200012);

			weaponBox_4.gameObject.SetActive(false);

			weaponBox_4.gameObject.SetActive(true);
			
			weaponBox_4.setTriggerableSkill(true, skillTemplate.value2, 0);
		}
		else if(hand == 1)
		{
			weaponBox_1.gameObject.SetActive(false);
			
			weaponBox_1.gameObject.SetActive(true);
			
			weaponBox_1.setTriggerable(true);
		}
		else if(hand == 2)
		{
			weaponBox_2.gameObject.SetActive(false);
			
			weaponBox_2.gameObject.SetActive(true);
			
			weaponBox_2.setTriggerable(true);
		}
		else if(hand == 3)
		{
			weaponBox_3.gameObject.SetActive(false);
			
			weaponBox_3.gameObject.SetActive(true);
			
			weaponBox_3.setTriggerable(true);
		}
		else if(hand == 4)
		{
			weaponBox_4.gameObject.SetActive(false);
			
			weaponBox_4.gameObject.SetActive(true);
			
			weaponBox_4.setTriggerable(true);
		}
	}

	public override void setWeaponTriggerFalse(int hand)
	{
		if (BattleControlor.Instance ().inDrama == true) return;

		if (actionId >= 140 && actionId <= 151) return;//无敌斩

		if (actionId == 3250)  return;//旋风斩

		if (actionId == 3260) return;//冲锋

		bool attacking = isPlayingAttack ();

		if(attacking == true)
		{
			//if(hitCount <= 3) 

			if(BattleUIControlor.Instance().layerAutoFight[0].activeSelf == false || hitCount != 0)
			{
				comboable = true;
			}
		}

		if(hand == 1)
		{
			weaponBox_1.setTriggerable(false);
			
			weaponBox_1.gameObject.SetActive(false);
		}
		else if(hand == 2)
		{
			weaponBox_2.setTriggerable(false);

			if(weaponBox_2.hitTarget == true)
			{
				int efid = 105;

				if(actionId == 200) efid = 105;

				else if(actionId == 201) efid = 106;

				else if(actionId == 202) efid = 107;

				if(Console_SetBattleFieldFx.IsEnableAttackFx())
				{
					BattleEffectControllor.Instance().PlayEffect(efid, transform.position, transform.forward);
				}
//				else{
//					EffectIdTemplate t_et = EffectTemplate.getEffectTemplateByEffectId ( efid );
//
//					Debug.Log( "Play Effect: " + efid + " - " + t_et.path );
//				}
			}

			weaponBox_2.gameObject.SetActive(false);
		}
		else if(hand == 3)
		{
			weaponBox_3.transform.localPosition = Vector3.zero;

			weaponBox_3.setTriggerable(false);
			
			weaponBox_3.gameObject.SetActive(false);
		}
		else if(hand == 4)
		{
			weaponBox_4.setTriggerable(false);
			
			weaponBox_4.gameObject.SetActive(false);
		}
	}

	public void Shake(KingCamera.ShakeType shakeType)
	{
		if(stance == Stance.STANCE_SELF)
		{
			gameCamera.Shake (shakeType);
		}
	}

	private void setMoveableInAttackTrue()
	{
		moveableInAttack = true;
	}
	
	private void setMoveableInAttackFalse()
	{
		moveableInAttack = false;
	}

	public void setSkill(List<NodeSkill> skilldata)
	{
		for(int i = 0; i < skilldata.Count; i ++)
		{
			skills.Add(gameObject.AddComponent<HeroSkill>());
			skills[skills.Count - 1].init(skilldata[i], skills.Count - 1);
		}
	}

	public override string getAnimationName(AniType aniType)
	{
		if(aniType == AniType.ANI_Attack_0)
		{
			return "attack_" + hitCount;
		}
		else if(aniType == AniType.ANI_Dead)
		{
			return "Dead";
		}
		
		return base.getAnimationName(aniType);
	}

}
