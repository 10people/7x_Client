using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ReplayNode : MonoBehaviour
{
	public enum AniType
	{
		ANI_WALK,
		ANI_ATTACK,
		ANI_DEAD,
		ANI_STAND,
	}

	public TroopReplay.HeroType heroType;

	public List<BaseReplay> replayList = new List<BaseReplay>();

	public TroopReplay troop;
	
	public GameObject trailTemple;
	
	public float attackRange;//鏀诲嚮鑼冨洿鍗婂緞
	
	public float attackValue;//鏀诲嚮鍔毶	
	public float attackSpeed;//鏀诲嚮閫熷害
	
	public float defenceValue;//闃插尽鍔毶	
	public float speed;//绉诲姩閫熷害
	
	public float hp;//琛閲幧
	public int heroOrSoldier;

	public int troopPosition;

	public int nodePosition;

	
	[HideInInspector] public ReplayNode targetNode;
	
	[HideInInspector] public GameObject shadowTemple;
	
	[HideInInspector] public GameObject shadowObject;
	
	[HideInInspector] public int nodeId;
	
	[HideInInspector] public float hpMax;
	
	[HideInInspector] public Vector3 position;
	
	[HideInInspector] public Animation mAnim;
	
	//[HideInInspector] public CapsuleCollider attackCollider;
	
	[HideInInspector] public List<GameObject> trails = new List<GameObject>();
	
	[HideInInspector] public List<Buff> buffs = new List<Buff>();
	
	[HideInInspector] public List<ReplayNode> enemysInRange = new List<ReplayNode>();
	
	[HideInInspector] public bool isAlive = true;
	
	[HideInInspector] public Color shadowColor;
	
	[HideInInspector] public bool isBlind;
	
	[HideInInspector] public bool isIdle;
	
	[HideInInspector] public bool slowdownable;


	private System.DateTime tempTime;

	private NavMeshAgent nav;


	public virtual void Start()
	{
		isBlind = false;
		
		isIdle = false;
		
		slowdownable = true;
		
		//attackCollider = (CapsuleCollider)gameObject.GetComponent("CapsuleCollider");
		
		nav = (NavMeshAgent)gameObject.GetComponent("NavMeshAgent");

		if(nav != null)
		{
			speed = nav.speed;

			updataAttackRange();
		}

		mAnim = GetComponentInChildren<Animation>();

		trails.Clear();

		tempTime = System.DateTime.Now;
	}

	public void initDate(BaseAI aiDate)
	{
		Debug.LogError( "Never use Since 2015.3.31, by YuGu." );

//		nodeId = aiDate.nodeId;
//
//		attackValue = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_attackValue];
//
//		attackRange = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_attackRange];
//
//		attackValue = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_attackValue];
//
//		attackSpeed = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_attackSpeed];
//
//		defenceValue = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_defenceValue];
//
//		speed = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_moveSpeed];
//
//		hp = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_hp];
//
//		hpMax = aiDate.nodeData.attributes[(int)AIdata.AttributeType.ATTRTYPE_hpMax];
	}

	public void initDate()
	{
		setNavMeshSpeed(troop.hero.getNavMeshSpeed());
		
		attackSpeed = troop.hero.attackSpeed;
		
		//attackRange = troop.hero.attackRange;
		
		attackValue = troop.hero.attackValue;
		
		defenceValue = troop.hero.defenceValue;
		
		hp = troop.hero.hpMax;
		
		hpMax = hp;
	}

	public void initDate(int soldierIndex)
	{
		initDate();

		nodeId = troop.hero.nodeId + 1 + soldierIndex;

		gameObject.name = "Soldier_" + nodeId;
	}

	public void initDate(Soldier soldierDate, int index)
	{
		setNavMeshSpeed(soldierDate.moveSpeed);

		attackSpeed = soldierDate.attackSpeed;

		attackValue = soldierDate.attackValue;

		defenceValue = soldierDate.defenceValue;

		hp = soldierDate.hpMax;

		hpMax = hp;

		nodeId = troop.hero.nodeId + 1 + index;

		gameObject.name = "Soldier_" + nodeId;
	}

	public void initDate(Hero heroDate)
	{
		setNavMeshSpeed(heroDate.moveSpeed);
		
		attackSpeed = heroDate.attackSpeed;
		
		attackValue = heroDate.attackValue;
		
		defenceValue = heroDate.defenceValue;
		
		hp = heroDate.hpMax;
		
		hpMax = hp;
		
		gameObject.name = "Hero_" + nodeId;
	}

	public void addAutoFight()
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		replayList.Add(ReplayAutoFight.createReplayAutoFight(mill));
		
		tempTime = System.DateTime.Now;
	}

	public void addAttackedNode(float hpValue)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;

		float mill = (float)span.TotalMilliseconds;

		replayList.Add(ReplayAttacked.createReplayAttack(mill, hpValue));

		tempTime = System.DateTime.Now;
	}

	public void addKingMoveNode(Vector3 offset)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;

		replayList.Add(ReplayKingMove.createReplayKingMove(mill, offset));

		tempTime = System.DateTime.Now;
	}

	public void addKingReviseNode(Vector3 position)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		replayList.Add(ReplayKingRevise.createReplayKingRevise(mill, position));

		tempTime = System.DateTime.Now;
	}

	public void addKingForwardNode(Vector3 forward)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		replayList.Add(ReplayKingForward.createReplayKingForward(mill, forward));
		
		tempTime = System.DateTime.Now;
	}

	public void addKingAttackNode(Vector3 offset)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;

		replayList.Add(ReplayKingAttack.createReplayKingAttack(mill, offset));

		tempTime = System.DateTime.Now;
	}

	public void addKingWeaponNode(KingControllor.WeaponType weaponType)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;

		int weapon = 0;

		if(weaponType == KingControllor.WeaponType.W_Heavy) weapon = 0;
		else if(weaponType == KingControllor.WeaponType.W_Ranged) weapon = 1;
		else if(weaponType == KingControllor.WeaponType.W_Light) weapon = 2;

		replayList.Add(ReplayKingWeapon.createReplayKingWeapon(mill, weapon));
		
		tempTime = System.DateTime.Now;
	}

	public void addKingInspireNode()
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;

		replayList.Add(ReplayKingInspire.createReplayKingInspire(mill));

		tempTime = System.DateTime.Now;
	}

	public void addKingReliefNode()
	{
		System.DateTime t = System.DateTime.Now;

		System.TimeSpan span = t - tempTime;

		float mill = (float)span.TotalMilliseconds;

		replayList.Add(ReplayKingRelief.createReplayKingRelief(mill));

		tempTime = System.DateTime.Now;
	}

	public void addKingSkillNode(int skillType)
	{
		System.DateTime t = System.DateTime.Now;
		
		System.TimeSpan span = t - tempTime;
		
		float mill = (float)span.TotalMilliseconds;
		
		replayList.Add(ReplayKingSkill.createReplayKingSkill(mill, skillType));

		tempTime = System.DateTime.Now;
	}

	public void init()
	{
		if(troop.heroType == TroopReplay.HeroType.TYPE_PLAYER)
		{
			shadowColor = Color.yellow;
		}
		else if(troop.heroType == TroopReplay.HeroType.TYPE_KING)
		{
			shadowColor = Color.green;
		}
		else
		{
			shadowColor = troop.stance == TroopReplay.Stance.STANCE_SELF ? Color.red : Color.blue;
		}
		
		shadowObject = (GameObject)Instantiate(shadowTemple);
		
		shadowObject.SetActive(true);
		
		shadowObject.transform.parent = gameObject.transform;
		
		shadowObject.transform.localPosition = Vector3.zero;
		
		shadowObject.transform.localScale = shadowTemple.transform.localScale;
		
		shadowObject.GetComponent<Renderer>().material.color = shadowColor;
		
		enemysInRange.Clear();
	}

	public virtual void FixedUpdate ()
	{
		if(BattleReplayorReader.Instance() == null) return;

		if(!isAlive) return;

		if(BattleReplayControlor.Instance().result != BattleReplayControlor.BattleResult.RESULT_BATTLING) return;

		updatePosition();
		
		uptateTarget();
		
		updateEnemysList();
		
		moveOrAttack();
	}

	public void startReplay()
	{
		StartCoroutine(replayAction());
	}

	public virtual IEnumerator replayAction()
	{
		yield return new WaitForSeconds(0.01f);

		foreach(BaseReplay br in replayList)
		{
			if(br.replayorType == BaseReplay.ReplayorNodeType.ATTACK)
			{
				ReplayAttacked ra = (ReplayAttacked)br;

				yield return new WaitForSeconds(ra.delayTime / 1000.0f);

				hp -= ra.hpValue;

				if(hp < 0)
				{
					die();
				}
			}
		}
	}

	public void updatePosition()
	{
		position = troop.gameObject.transform.localPosition + gameObject.transform.localPosition;
	}
	
	public void uptateTarget()
	{
		if(targetNode != null
		   && targetNode.isAlive
		   && Vector3.Distance(targetNode.position, position) < attackRange)
			return;
		
		targetNode = null;
		
		if(troop.targetTroop == null) return;
		
		float templ = 9999999;
		
		if(troop.targetTroop.hero != null && troop.targetTroop.hero.isAlive)
		{
			templ = Vector3.Distance(position, troop.targetTroop.hero.position);
			
			targetNode = troop.targetTroop.hero;
		}
		
		foreach(ReplayNode soldier in troop.targetTroop.soldiers)
		{
			if(soldier == null || !soldier.isAlive) continue;
			
			float l = Vector3.Distance(position, soldier.position);
			
			if(l < templ)
			{
				targetNode = soldier;
				
				templ = l;
			}
		}
		
		if(templ > attackRange)
		{
			foreach(ReplayNode node in enemysInRange)
			{
				float l = Vector3.Distance(position, node.position);
				
				if(l < attackRange)
				{
					targetNode = node;
					
					break;
				}
			}
		}
	}
	
	public virtual void moveOrAttack()
	{
		if(isIdle)
		{
			mAnim.Stop();
			
			setNavMeshStop();
			
			return;
		}
		else if(isBlind)
		{
			setNavMeshDestination(position + new Vector3(Random.value * 4 - 2, 0, Random.value * 4 - 2));
			
			return;
		}
		
		if(mAnim.IsPlaying(getAnimationName(AniType.ANI_ATTACK))) return;
		
		if(targetNode == null) 
		{
			if(troop.targetPosition.y < 100)
			{
				if(Vector3.Distance(troop.targetPosition, position) < 5)
				{
					troop.clearTargetPosition();
				}
			}

			return;
		}
		
		float tempL = Vector3.Distance(targetNode.position, position);
		
		if(tempL <= attackRange)
		{
			nav.Stop();
			
			attack(targetNode);
		}
		else
		{
			setNavMeshDestination(troop.targetTroop.position);
		}
	}
	
	public virtual bool attack(ReplayNode defender)
	{
		if(troop.heroType == TroopReplay.HeroType.TYPE_PLAYER)
		{
			ReplayNodeKing king = (ReplayNodeKing)this;
			
			king.attack(transform.forward);
			
			return true;
		}

		mAnim.Play(getAnimationName(AniType.ANI_ATTACK));
		
		transform.forward = defender.position - position;
		
		float av = BattleReplayControlor.Instance().getAttackValue(this, defender);

		if(attackRange < 6)
		{

		}
		else
		{
			StartCoroutine(minusTargetHp(defender, av, Color.blue));
		}
		
		return true;
	}
	
	public void updataAttackRange()
	{
		//attackCollider.radius = attackRange;
		
		nav.speed = speed;
	}
	
	public void updateEnemysList()
	{
		foreach(ReplayNode node in enemysInRange)
		{
			if(node == null || !node.isAlive)
			{
				enemysInRange.Remove(node);
				
				return;
			}
		}
	}
	
	public virtual void die()
	{
		if(!isAlive) return;
		
		if(mAnim.GetClip(getAnimationName(AniType.ANI_DEAD)) == null)
		{
			isAlive = false;
			
			Destroy(gameObject);
			
			return;
		}
		
		mAnim.Play(getAnimationName(AniType.ANI_DEAD));
		
		isAlive = false;
		
		Destroy(nav);
		
		CharacterController cc = (CharacterController)gameObject.GetComponent("CharacterController");
		
		Destroy(cc);
		
		CapsuleCollider co = (CapsuleCollider)gameObject.GetComponent("CapsuleCollider");
		
		Destroy(co);
		
		StartCoroutine( dieAction() );
	}
	
	public virtual IEnumerator dieAction()
	{
		yield return new WaitForSeconds(5.0f);
		
		TweenPosition.Begin(gameObject, 2.0f, gameObject.transform.localPosition + new Vector3(0, -2, 0));
		
		yield return new WaitForSeconds(2.0f);
		
		Destroy(gameObject);
	}
	
	public virtual void useFeaturesSkill()
	{
		
	}
	
	public IEnumerator minusTargetHp(ReplayNode defender, float _attackValue, Color trailColor)
	{
		GameObject tempTrailObject = (GameObject)Instantiate(trailTemple);
		
		tempTrailObject.SetActive(true);
		
		tempTrailObject.transform.parent = trailTemple.transform.parent;
		
		tempTrailObject.transform.localScale = trailTemple.transform.localScale;
		
		tempTrailObject.transform.localPosition = position + new Vector3(0, 1, 0);
		
		tempTrailObject.name = "Trail_" + nodeId;
		
		trails.Add(tempTrailObject);

		/*
		Trail trail = (Trail)tempTrailObject.GetComponentInChildren(typeof(Trail));
		
		trail.height = 0.5f;
		
		trail.SetTrailColor(trailColor);
		
		trail.time = 0.02f;
		*/
		
		TweenPosition.Begin(tempTrailObject, .5f, defender.position + new Vector3(0, 1, 0));

		yield return new WaitForSeconds(2.0f);
		
		trails.Remove(tempTrailObject);

		Destroy(tempTrailObject);
	}
	
	public virtual void attacked(ReplayNode attacker, float hpValue)
	{
		
	}
	
	public void clearTrails()
	{
		foreach(GameObject trailObject in trails)
		{
			Destroy(trailObject);
		}
		
		trails.Clear();
	}
	
	public List<Buff> getBuffs()
	{
		return buffs;
	}
	
	public void setNavMeshDestination(Vector3 targetPosition)
	{
		//if(Vector3.Distance(targetPosition, tempTargetPosition) < 2) return;

		mAnim.CrossFade(getAnimationName(AniType.ANI_WALK));
		
		nav.SetDestination(targetPosition);
		
		//tempTargetPosition = targetPosition;
	}
	
	public void addNavMeshSpeed(float tempSpeed)
	{
		speed += tempSpeed;
		
		updataAttackRange();
	}
	
	public void setNavMeshSpeed(float _speed)
	{
		speed = _speed;
		
		//updataAttackRange();
	}
	
	public void setNavMeshStop()
	{
		nav.Stop();
		
		//tempTargetPosition = Vector3.zero;
	}
	
	public float getNavMeshSpeed()
	{
		return speed;
	}
	
	public void setNavMeshRadius(float radius)
	{
		nav.radius = radius;
	}
	
	public float getNavMeshRadius()
	{
		return nav.radius;
	}
	
	public float getNavMeshRemainingDistance()
	{
		return nav.remainingDistance;
	}

	void OnTriggerEnter(Collider other)
	{
		ReplayNode node = (ReplayNode)other.gameObject.GetComponent("ReplayNode");
		
		if(node == null || !node.isAlive || node.troop == null ||node.troop.stance == troop.stance)
		{
			return;
		}
		
		bool flag = checkEnemysInRange(node);
		
		if(flag)
		{
			enemysInRange.Add(node);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		ReplayNode node = (ReplayNode)other.gameObject.GetComponent("ReplayNode");
		
		enemysInRange.Remove(node);
	}
	
	private bool checkEnemysInRange(ReplayNode node)
	{
		foreach(ReplayNode ai in enemysInRange)
		{
			if(ai.nodeId == node.nodeId)
			{
				return false;
			}
		}
		
		return true;
	}

	public virtual string getAnimationName(AniType aniType)
	{
		if(aniType == AniType.ANI_ATTACK)
		{
			return "Attack_1";
		}
		else if(aniType == AniType.ANI_DEAD)
		{
			return "Dead";
		}
		else if(aniType == AniType.ANI_WALK)
		{
			if(mAnim.GetClip("Run") != null)
			{
				return "Run";
			}
			else
			{
				return "Walk";
			}
		}
		
		return "";
	}

}
