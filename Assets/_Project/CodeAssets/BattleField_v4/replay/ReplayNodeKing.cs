using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayNodeKing : ReplayNode {
	
	public enum WeaponType
	{
		Dao,
		Gong,
		Qiang,
	}
	
	public KingArrow bulletTemple;

	public GameObject m_weapon_dao;
	
	public GameObject m_weapon_dun;
	
	public GameObject m_weapon_chang_mao;

	public GameObject m_weapon_gong;

	public Transform m_pos_dao;
	
	public Transform m_pos_dun;
	
	public Transform m_pos_chang_mao;

	public Transform m_pos_gong;

	
	[HideInInspector] public WeaponType weaponType;
	
	[HideInInspector] public bool prepareQiangSkill;
	

	private CharacterController character;
	
	private bool moving;

	private bool attacking;
	
	private List<ReplayNode> attackedNodes;
	
//	private System.DateTime attackCdtime;

	private GameObject[] m_cur_weapons = new GameObject[ 2 ];

	private int hitCount;

	private float tempSpeed;


	public override void Start()
	{
		hitCount = 0;

		moving = false;

		attacking = false;

		prepareQiangSkill = false;

		tempSpeed = 0;

		base.Start();

		character = this.GetComponent<CharacterController>();

		attackedNodes = new List<ReplayNode>();

		m_cur_weapons[0] = null;

		m_cur_weapons[1] = null;

		changeWeapon(WeaponType.Dao);

//		attackCdtime = System.DateTime.Now.AddSeconds(-100.0);
	}

	public override IEnumerator replayAction()
	{
		yield return new WaitForSeconds(0.01f);

		foreach(BaseReplay br in replayList)
		{
			if(br.replayorType == BaseReplay.ReplayorNodeType.AUTO_FIGHT)
			{
				ReplayAutoFight raf = (ReplayAutoFight)br;

				yield return new WaitForSeconds(raf.delayTime / 1000.0f);

				BattleReplayControlor.Instance().autoFight = !BattleReplayControlor.Instance().autoFight;

				if(BattleReplayControlor.Instance().autoFight == false)
				{
					BattleReplayControlor.Instance().king.setNavMeshStop();

					BattleReplayControlor.Instance().king.mAnim.CrossFade(BattleReplayControlor.Instance().king.getAnimationName(AniType.ANI_STAND));
				}
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_MOVE)
			{
				ReplayKingMove rkm = (ReplayKingMove)br;

				yield return new WaitForSeconds(rkm.delayTime / 1000.0f);

				move(rkm.offset);
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_REVISE)
			{
				ReplayKingRevise rkr = (ReplayKingRevise)br;

				yield return new WaitForSeconds(rkr.delayTime / 1000.0f);

				revise(rkr.position);
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_FORWARD)
			{
				ReplayKingForward rkf = (ReplayKingForward)br;

				reviseForward(rkf.forward);
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.ATTACK)
			{
				ReplayAttacked ra = (ReplayAttacked)br;

				yield return new WaitForSeconds(ra.delayTime / 1000.0f);

				hp -= ra.hpValue;

				if(hp < 0)
				{
					die();
				}
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_ATTACK)
			{
				ReplayKingAttack rka = (ReplayKingAttack)br;

				yield return new WaitForSeconds(rka.delayTime / 1000.0f);

				if(rka.offset.x == -100 && rka.offset.y == -100 && rka.offset.z == -100)
				{
					setAttacking(false);
				}
				else
				{
					attack(rka.offset);
				}
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_WEAPON)
			{
				ReplayKingWeapon rkw = (ReplayKingWeapon)br;

				yield return new WaitForSeconds(rkw.delayTime / 1000.0f);

				WeaponType wt = WeaponType.Dao;

				if(rkw.weapon == 0) wt = WeaponType.Dao;
				else if(rkw.weapon == 1) wt = WeaponType.Gong;
				else if(rkw.weapon == 2) wt = WeaponType.Qiang;

				changeWeapon(wt);
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_INSPIRE)
			{
				ReplayKingInspire rki = (ReplayKingInspire)br;

				yield return new WaitForSeconds(rki.delayTime / 1000.0f);

				StartCoroutine(inspire());
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_RELIF)
			{
				ReplayKingRelief rkr = (ReplayKingRelief)br;

				yield return new WaitForSeconds(rkr.delayTime / 1000.0f);

				StartCoroutine(BattleReplayControlor.Instance().reliefTroop());
			}
			else if(br.replayorType == BaseReplay.ReplayorNodeType.KING_SKILL)
			{
				ReplayKingSkill rks = (ReplayKingSkill)br;

				yield return new WaitForSeconds(rks.delayTime / 1000.0f);

				//1:dao_1  2:dao_2  3:gong_1  4:gong_2  5:qiang_1_prepare  6:qiang_1_cacel  7:qiang_1_use  8:qiang_2
				if(rks.skillType == 1)
				{
					useDaoSkill_1();
				}
				else if(rks.skillType == 2)
				{
					useDaoSkill_2();
				}
				else if(rks.skillType == 3)
				{
					useGongSkill_1();
				}
				else if(rks.skillType == 4)
				{
					useGongSkill_2();
				}
				else if(rks.skillType == 5)
				{
					prepareQiangSkill_1();
				}
				else if(rks.skillType == 6)
				{
					cancelQiangSkill_1();
				}
				else if(rks.skillType == 7)
				{
					checkQiangSkill_1();
				}
				else if(rks.skillType == 8)
				{
					useQiangSkill_2();
				}
			}
		}
	}

	public void changeWeapon(WeaponType _weapon)
	{
		weaponType = _weapon;

		hitCount = 0;

		if(weaponType == WeaponType.Dao)
		{
			ChangeWeapon_DaoDun();

			attackRange = 4.5f;
		}
		else if(weaponType == WeaponType.Qiang)
		{
			ChangeWeapon_ChangMao();

			attackRange = 5f;
		}
		else if(weaponType == WeaponType.Gong)
		{
			ChangeWeapon_GongJian();
		
			attackRange = 12f;
		}

		updataAttackRange();

		if(weaponType == WeaponType.Qiang)
		{
			mAnim.CrossFade("idle_chang_mao");
		}
		else if(weaponType == WeaponType.Dao)
		{
			mAnim.CrossFade("idle_dao_dun");
		}
		else if(weaponType == WeaponType.Gong)
		{
			mAnim.CrossFade("idle_gong_jian");
		}
	}

	private void ClearWeapon(){
		if( m_cur_weapons[ 0 ] != null ){
			Destroy( m_cur_weapons[ 0 ] );
			
			m_cur_weapons[ 0 ] = null;
		}

		if( m_cur_weapons[ 1 ] != null ){
			Destroy( m_cur_weapons[ 1 ] );

			m_cur_weapons[ 1 ] = null;
		}
	}

	private void ChangeWeapon_DaoDun()
	{
		ClearWeapon();

		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_dao, m_pos_dao.transform.position, m_pos_dao.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_dao.parent;
		GameObjectHelper.SetGameObjectLayer( m_cur_weapons[ 0 ], gameObject.layer );

		m_cur_weapons[ 1 ] = (GameObject)Instantiate( m_weapon_dun, m_pos_dun.transform.position, m_pos_dun.transform.rotation  );
		m_cur_weapons[ 1 ].transform.parent = m_pos_dun.parent;
		GameObjectHelper.SetGameObjectLayer( m_cur_weapons[ 1 ], gameObject.layer );
	}

	private void ChangeWeapon_ChangMao()
	{
		ClearWeapon();

		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_chang_mao, m_pos_chang_mao.transform.position, m_pos_chang_mao.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_chang_mao.parent;
		GameObjectHelper.SetGameObjectLayer( m_cur_weapons[ 0 ], gameObject.layer );
	}

	private void ChangeWeapon_GongJian()
	{
		ClearWeapon();

		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_gong, m_pos_gong.transform.position, m_pos_gong.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_gong.parent;
		GameObjectHelper.SetGameObjectLayer( m_cur_weapons[ 0 ], gameObject.layer );
	}


	public override void FixedUpdate()
	{
		if(BattleReplayControlor.Instance().autoFight == true)
		{
			base.FixedUpdate();

			return;
		}

		updatePosition();
		
		updateEnemysList();
		
		updataAttackRange();
		
		checkQiangSkill_1();
	}
	
	void LateUpdate()
	{
		if(!isAlive) return;
		
		updateAnim();
		
		updateCamera();
	}

	private void updateAnim()
	{
		if(!moving && !attacking && !mAnim.isPlaying)
		{
			if(weaponType == WeaponType.Qiang)
			{
				mAnim.CrossFade("idle_chang_mao");
			}
			else if(weaponType == WeaponType.Dao)
			{
				mAnim.CrossFade("idle_dao_dun");
			}
			else if(weaponType == WeaponType.Gong)
			{
				mAnim.CrossFade("idle_gong_jian");
			}
		}
	}
	
	private void updateCamera()
	{
		Camera.main.transform.position = transform.position + new Vector3(0, 9.6f, -17.25f);
	}

	public void move(Vector3 offset)
	{
		if(!isAlive) return;

		if(getIsPlayingAttack())
		{
			if(offset == Vector3.zero && moving == true)
			{
				moving = false;
			}

			return;
		}

		if(Vector3.Distance(Vector3.zero, offset) > 0)
		{
			gameObject.transform.forward = offset;
		}
		
		character.Move(offset * getNavMeshSpeed() * Time.deltaTime);
		
		if (offset != Vector3.zero)
		{
			transform.forward = offset;

			if(weaponType == WeaponType.Qiang)
			{
				mAnim.CrossFade("run_chang_mao");
			}
			else if(weaponType == WeaponType.Dao)
			{
				mAnim.CrossFade("run_dao_dun");
			}
			else if(weaponType == WeaponType.Gong)
			{
				mAnim.CrossFade("run_gong_jian");
			}

			moving = true;
		}
		else if(moving)
		{
			if(weaponType == WeaponType.Qiang)
			{
				mAnim.CrossFade("idle_chang_mao");
			}
			else if(weaponType == WeaponType.Dao)
			{
				mAnim.CrossFade("idle_dao_dun");
			}
			else if(weaponType == WeaponType.Gong)
			{
				mAnim.CrossFade("idle_gong_jian");
			}
			
			moving = false;
		}
	}

	private void revise(Vector3 position)
	{
		transform.position = position;
	}

	private void reviseForward(Vector3 forward)
	{
		transform.forward = forward;
	}

	public override void die()
	{
		character.enabled = false;
		
		base.die();
	}

	private void setAttacking(bool _attacking)
	{
		attacking = _attacking;

		if(attacking == false)
		{
			hitCount = 0;
		}
	}

	public void attack(Vector3 weaponFoward)
	{
		attacking = true;

		if(weaponType == WeaponType.Dao)
		{
			attackDao();
		}
		else if(weaponType == WeaponType.Gong)
		{
			attackGong(weaponFoward);
		}
		else if(weaponType == WeaponType.Qiang)
		{
			attackQiang();
		}
	}

	private bool getIsPlayingAttack()
	{
		if(mAnim.IsPlaying("attack_chang_mao")
		   || mAnim.IsPlaying("attack_0_dao_dun")
		   || mAnim.IsPlaying("attack_1_dao_dun")
		   || mAnim.IsPlaying("attack_0_gong_jian")
		   || mAnim.IsPlaying("attack_1_gong_jian")
		   || mAnim.IsPlaying("attack_2_gong_jian"))
		{
			return true;
		}

		return false;
	}

	private bool getIsAttacking()
	{
		if(getIsPlayingAttack()) return true;

		return false;

		/*
		System.DateTime time = System.DateTime.Now;

		System.TimeSpan ts = time - attackCdtime;
		
		int num = (int)ts.TotalMilliseconds;
		
		if(num > 1000)
		{
			return false;
		}
		
		return true;
		*/
	}
	
	public void attackGong(Vector3 weaponFoward)
	{
		if(getIsAttacking()) return;
		
		if(weaponFoward != Vector3.zero)
		{
			gameObject.transform.forward = weaponFoward;
		}

		adjustFowardGong();

//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_" + hitCount + "_gong_jian");

		hitCount ++;

		hitCount = hitCount > 2 ? 0 : hitCount;

		KingArrow.createArrow(this);
	}

	public void adjustFowardGong()
	{
		ReplayNode tempNode = null;

		float tempAngle = 360;

		foreach(ReplayNode node in enemysInRange)
		{
			if(node == null || !node.isAlive) continue;

			float l = Vector3.Distance(node.position, position);

			float alpha = Vector3.Angle(node.transform.position - transform.position, transform.forward);

			if(l < attackRange && alpha < tempAngle && alpha < 30)
			{
				tempNode = node;

				tempAngle = alpha;
			}
		}

		if(tempNode != null)
		{
			gameObject.transform.forward = tempNode.position - transform.position;
		}
	}

	public void attackDao()
	{
		if(getIsAttacking()) return;
		
//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_" + hitCount + "_dao_dun");

		hitCount ++;
		
		hitCount = hitCount > 1 ? 0 : hitCount;

		attackActionDao();

		attackedNodes.Clear();
	}

	public void attackQiang()
	{
		if(getIsAttacking()) return;
		
//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_chang_mao");
		
		attackActionQiang();
	}

	private void attackActionDao()
	{
		foreach(TroopReplay troop in BattleReplayControlor.Instance().enemyTroops)
		{
			if(troop == null) continue;

			ReplayNode hero = troop.hero;

			if(hero != null && hero.isAlive)
			{
				bool flag = nodeInAttacked(hero.nodeId);

				if(flag) continue;

				bool inRule = checkAttackRuleDao(hero);

				if(inRule)
				{
					//troop.minusHp(this, hero, BattleReplayControlor.Instance().getAttackValue(this, hero));

					//attackedNodes.Add(hero);
				}
			}

			foreach(ReplayNode soldier in troop.soldiers)
			{
				if(soldier == null || !soldier.isAlive) continue;

				bool flagSoldier = nodeInAttacked(soldier.nodeId);

				if(flagSoldier) continue;

				bool inRuleSoldier = checkAttackRuleDao(soldier);

				if(inRuleSoldier)
				{
					//troop.minusHp(this, soldier, BattleReplayControlor.Instance().getAttackValue(this, soldier));

					//attackedNodes.Add(soldier);
				}
			}
		}
	}

	private void attackActionQiang()
	{
		foreach(TroopReplay troop in BattleReplayControlor.Instance().enemyTroops)
		{
			if(troop == null) continue;
			
			ReplayNode hero = troop.hero;
			
			if(hero != null && hero.isAlive)
			{
				bool flag = nodeInAttacked(hero.nodeId);
				
				if(flag) continue;
				
				bool inRule = checkAttackRuleQiang(hero);
				
				if(inRule)
				{
					//troop.minusHp(this, hero, BattleReplayControlor.Instance().getAttackValue(this, hero));
					
					//attackedNodes.Add(hero);
				}
			}
			
			foreach(ReplayNode soldier in troop.soldiers)
			{
				if(soldier == null || !soldier.isAlive) continue;
				
				bool flagSoldier = nodeInAttacked(soldier.nodeId);
				
				if(flagSoldier) continue;
				
				bool inRuleSoldier = checkAttackRuleQiang(soldier);
				
				if(inRuleSoldier)
				{
					//troop.minusHp(this, soldier, BattleReplayControlor.Instance().getAttackValue(this, soldier));
					
					//attackedNodes.Add(soldier);
				}
			}
		}
		
		attackedNodes.Clear();
	}

	private bool checkAttackRuleDao(ReplayNode node)
	{
		if(Vector3.Distance(node.position, transform.position) > attackRange)
		{
			return false;
		}

		float alpha = Vector3.Angle(node.transform.position - transform.position, transform.forward);

		if(alpha < 90)
		{
			return true;
		}

		return false;
	}

	private bool checkAttackRuleQiang(ReplayNode node)
	{
		if(node == null || node.isAlive == false) return false;

		if(Vector3.Distance(node.position, transform.position) > attackRange)
		{
			return false;
		}

		float alpha = Vector3.Angle(node.transform.position - transform.position, transform.forward);
		
		if(alpha < 40)
		{
			return true;
		}
		
		return false;
	}

	private bool nodeInAttacked(int nodeId)
	{
		foreach(ReplayNode node in attackedNodes)
		{
			if(nodeId == node.nodeId)
			{
				return true;
			}
		}

		return false;
	}

	public IEnumerator inspire()
	{
		float length = 10;

		mAnim.CrossFade("gather");

		TweenScale.Begin(shadowObject, .5f, new Vector3(250 * length, 250 * length, 250 * length));

		TroopReplay tempTroop = null;
		
		float tempLength = 1000;

		foreach(TroopReplay troop in BattleReplayControlor.Instance().selfTroops)
		{
			float l = Vector3.Distance(troop.hero.position, position);
			
			if(l < length && l < tempLength)
			{
				tempTroop = troop;
				
				tempLength = l;
			}
		}

		if(tempTroop != null)
		{
			if(troop.hero != null && troop.hero.isAlive)
			{
				troop.hero.useFeaturesSkill();
			}
			
			foreach(ReplayNode soldier in troop.soldiers)
			{
				if(soldier != null && soldier.isAlive)
				{
					soldier.useFeaturesSkill();
				}
			}
		}

		yield return new WaitForSeconds(1.0f);

		TweenScale.Begin(shadowObject, .3f, shadowTemple.transform.localScale);
	}

	public bool useDaoSkill_1()
	{
		mAnim.CrossFade("gather");

		bool flag = false;

		foreach(ReplayNode node in enemysInRange)
		{
			if(node == null || !node.isAlive) continue;
			
			float l = Vector3.Distance(node.position, position);
			
			if(l < attackRange)
			{
				float av = BattleReplayControlor.Instance().getAttackValue(this, node);
				
				node.troop.minusHp(this, node, av * .5f);
				
				flag = true;
			}
		}

		if(flag == false)
		{
			return false;
		}

		return true;
	}
	
	public bool useDaoSkill_2()
	{
		//if(getIsAttacking()) return false;

		attackActionDao();

//		foreach(ReplayNode node in attackedNodes)
//		{
			//Buff.createBuff(node, Buff.BuffType.BUFF_HP, 10, 10.0f);
//		}

		if(attackedNodes.Count == 0){
			//return false;
		}

//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_" + hitCount + "_dao_dun");

		attackedNodes.Clear();

		return true;
	}

	public void prepareQiangSkill_1()
	{
		prepareQiangSkill = true;

		tempSpeed = speed;

		setNavMeshSpeed(tempSpeed + 5.0f);
	}

	public void cancelQiangSkill_1()
	{
		prepareQiangSkill = false;
		
		setNavMeshSpeed(tempSpeed);
	}

	public void checkQiangSkill_1()
	{
		if(prepareQiangSkill == false) return;

		foreach(ReplayNode node in enemysInRange)
		{
			bool flag = checkAttackRuleQiang(node);

			if(flag == true)
			{
				useQiangSkill_1(node);
			}
		}
	}

	public void useQiangSkill_1(ReplayNode node)
	{
//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_chang_mao");

		float av = BattleReplayControlor.Instance().getAttackValue(this, node);
		
		node.troop.minusHp(this, node, av * 1.5f);
	}

	public bool useQiangSkill_2()
	{
		mAnim.CrossFade("gather");
		
		ReplayNode tempNode = null;
		
		float tempL = 99999;

		foreach(ReplayNode node in enemysInRange)
		{
			if(node == null || !node.isAlive) continue;
			
			float l = Vector3.Distance(node.position, position);
			
			if(l < attackRange && l < tempL)
			{
				tempNode = node;
				
				tempL = l;
			}
		}

		if(tempNode == null)
		{
			return false;
		}

		return true;
	}
	
	public bool useGongSkill_1()
	{
		adjustFowardGong();

//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_" + 2 + "_gong_jian");
		
		KingArrow.createArrow(this).continueArrow = true;

		return true;
	}
	
	public bool useGongSkill_2()
	{
//		attackCdtime = System.DateTime.Now;
		
		mAnim.CrossFade("attack_" + 2 + "_gong_jian");

		Vector3 v = gameObject.transform.forward;

		for(int i = -2; i < 3; i ++)
		{
			gameObject.transform.forward = v;

			transform.eulerAngles += new Vector3(0, 20 * i, 0);

			KingArrow.createArrow(this);
		}

		gameObject.transform.forward = v;

		return true;
	}

	public override string getAnimationName(AniType aniType)
	{
		if(aniType == AniType.ANI_ATTACK)
		{
			if(weaponType == WeaponType.Qiang)
			{
				return "attack_chang_mao";
			}
			else if(weaponType == WeaponType.Dao)
			{
				return "attack_" + hitCount + "_dao_dun";
			}
			else if(weaponType == WeaponType.Gong)
			{
				return "attack_" + hitCount + "_gong_jian";
			}
		}
		else if(aniType == AniType.ANI_DEAD)
		{
			return "Dead";
		}
		else if(aniType == AniType.ANI_WALK)
		{
			if(weaponType == WeaponType.Qiang)
			{
				return "run_chang_mao";
			}
			else if(weaponType == WeaponType.Dao)
			{
				return "run_dao_dun";
			}
			else if(weaponType == WeaponType.Gong)
			{
				return "run_gong_jian";
			}
		}
		else if(aniType == AniType.ANI_STAND)
		{
			if(weaponType == WeaponType.Qiang)
			{
				return "idle_chang_mao";
			}
			else if(weaponType == WeaponType.Dao)
			{
				return "idle_dao_dun";
			}
			else if(weaponType == WeaponType.Gong)
			{
				return "idle_gong_jian";
			}
		}

		return "";
	}

}
