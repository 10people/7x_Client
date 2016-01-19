using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleReplayControlor : MonoBehaviour
{
	public enum BattleResult
	{
		RESULT_BATTLING,
		RESULT_WIN,
		RESULT_LOSE,
	}

	public GameObject heroTemple_Dun;

	public GameObject heroTemple_Qiang;

	public GameObject heroTemple_Gong;

	public GameObject heroTemple_Che;

	public GameObject heroTemple_Qi;

	public GameObject heroTemple_Player;

	public GameObject heroTemple_King;

	public ReplayNode soldierTemple_Che;
	
	public ReplayNode soldierTemple_Dun;
	
	public ReplayNode soldierTemple_Qiang;
	
	public ReplayNode soldierTemple_Qi;
	
	public ReplayNode soldierTemple_Gong;

	public GameObject selfTeam;

	public GameObject enemyTeam;
	
	public GameObject shadowTemple;

	public BattleResultControllor resultControllor;

	public List<GameObject> selfPositionFlag;

	public List<GameObject> enemyPositionFlag;

	public List<GameObject> soldierPositionFlag;


	[HideInInspector] public bool autoFight;

	[HideInInspector] public ReplayNode king;

	[HideInInspector] public List<TroopReplay> selfTroops = new List<TroopReplay>();
	
	[HideInInspector] public List<TroopReplay> enemyTroops = new List<TroopReplay>();

	[HideInInspector] public BattleResult result;


	private static BattleReplayControlor _instance;

	private List<Vector3> selfPositions = new List<Vector3>();
	
	private List<Vector3> enemyPositions = new List<Vector3>();

	private int startCount;


	void Awake() { _instance = this; }

	public static BattleReplayControlor Instance() { return _instance; }

	void Start()
	{
		result = BattleResult.RESULT_BATTLING;

		startCount = 0;

		selfPositions.Clear();

		enemyPositions.Clear();

		foreach(GameObject flag in selfPositionFlag) { selfPositions.Add(flag.transform.localPosition); }

		foreach(GameObject flag in enemyPositionFlag) { enemyPositions.Add(flag.transform.localPosition); }

		foreach(TroopReplay troop in selfTroops) { Destroy(troop.gameObject); }

		foreach(TroopReplay troop in enemyTroops) { Destroy(troop.gameObject); }

		selfTroops.Clear();

		enemyTroops.Clear();
	}

	void OnDestroy(){
		_instance = null;
	}

	public void createTroopEnemy(Troop troopDate, int positionIndex)
	{
		GameObject heroTemple = null;

		if(troopDate.heroType == (int)HeroType.TYPE_CHE) heroTemple = heroTemple_Che;
		else if(troopDate.heroType == (int)HeroType.TYPE_DUN) heroTemple = heroTemple_Dun;
		else if(troopDate.heroType == (int)HeroType.TYPE_GONG) heroTemple = heroTemple_Gong;
		else if(troopDate.heroType == (int)HeroType.TYPE_QI) heroTemple = heroTemple_Qi;
		else if(troopDate.heroType == (int)HeroType.TYPE_QIANG) heroTemple = heroTemple_Qiang;
		else if(troopDate.heroType == (int)HeroType.TYPE_KING) heroTemple = heroTemple_King;

		createTroop(heroTemple, troopDate.troopId, TroopReplay.Stance.STANCE_ENEMY, enemyPositions[positionIndex], troopDate);
	}

	public void createTroopSelf(Troop troopDate, int positionIndex)
	{
		GameObject heroTemple = null;
		
		if(troopDate.heroType == (int)HeroType.TYPE_CHE) heroTemple = heroTemple_Che;
		else if(troopDate.heroType == (int)HeroType.TYPE_DUN) heroTemple = heroTemple_Dun;
		else if(troopDate.heroType == (int)HeroType.TYPE_GONG) heroTemple = heroTemple_Gong;
		else if(troopDate.heroType == (int)HeroType.TYPE_QI) heroTemple = heroTemple_Qi;
		else if(troopDate.heroType == (int)HeroType.TYPE_QIANG) heroTemple = heroTemple_Qiang;
		else if(troopDate.heroType == (int)HeroType.TYPE_KING) heroTemple = heroTemple_Player;

		createTroop(heroTemple, troopDate.troopId, TroopReplay.Stance.STANCE_SELF, selfPositions[positionIndex], troopDate);
	}

	private TroopReplay createTroop(GameObject heroTemple, int id, TroopReplay.Stance stance, Vector3 position, Troop troopDate)
	{
		GameObject troopObject = new GameObject();

		GameObject parent = stance == TroopReplay.Stance.STANCE_ENEMY ? enemyTeam : selfTeam;

		List<TroopReplay> list = stance == TroopReplay.Stance.STANCE_ENEMY ? enemyTroops : selfTroops;

		troopObject.name = "Troop_" + id;

		troopObject.transform.parent = parent.transform;

		troopObject.transform.localScale = new Vector3(1, 1, 1);

		troopObject.transform.localPosition = position;

		GameObject heroObject = (GameObject)Instantiate(heroTemple, heroTemple.transform.position, heroTemple.transform.rotation);

		heroObject.SetActive(true);

		heroObject.transform.parent = troopObject.transform;

		heroObject.transform.localScale = new Vector3(1, 1, 1);

		heroObject.transform.localPosition = Vector3.zero;

		ReplayNode hero = (ReplayNode)heroObject.GetComponent("ReplayNode");

		TroopReplay troop = (TroopReplay)troopObject.AddComponent<TroopReplay>();

		hero.troop = troop;

		troop.hero = hero;

		hero.nodeId = id * 100;

		hero.shadowTemple = shadowTemple;

		if(troopDate == null)
		{
			hero.initDate();
		}
		else
		{
			hero.initDate(troopDate.hero);
		}
		
		troop.soldierTemple_Che = soldierTemple_Che;
		
		troop.soldierTemple_Dun = soldierTemple_Dun;
		
		troop.soldierTemple_Gong = soldierTemple_Gong;
		
		troop.soldierTemple_Qi = soldierTemple_Qi;
		
		troop.soldierTemple_Qiang = soldierTemple_Qiang;
		
		troop.soldierPositionFlag = soldierPositionFlag;
		
		troop.shadowTemple = shadowTemple;

		troop.stance = stance;

		troop.heroType = hero.heroType;

		/*
		if(troopDate != null)
		{
			troop.soldierDate = troopDate.soldiers;
			
			troop.soldierNum = troopDate.soldierNum;
		}
		*/

		list.Add(troop);

		if(hero.heroType == TroopReplay.HeroType.TYPE_PLAYER)
		{
			king = (ReplayNode)hero;
		}

		return troop;
	}

	public void createNode(BattleNodeData nodeData)
	{
		if(nodeData.nodes == null) return;

		ReplayNode node = null;

		foreach(TroopReplay troop in selfTroops)
		{
			if(node != null) break;
			
			if(troop.hero.nodeId == nodeData.nodeId)
			{
				node = troop.hero;

				break;
			}

			foreach(ReplayNode soldier in troop.soldiers)
			{
				if(soldier.nodeId == nodeData.nodeId)
				{
					node = soldier;

					break;
				}
			}
		}
		
		foreach(TroopReplay troop in enemyTroops)
		{
			if(node != null) break;
			
			if(troop.hero.nodeId == nodeData.nodeId)
			{
				node = troop.hero;
				
				break;
			}
			
			foreach(ReplayNode soldier in troop.soldiers)
			{
				if(soldier.nodeId == nodeData.nodeId)
				{
					node = soldier;
					
					break;
				}
			}
		}

		if(node == null)
		{
			Debug.LogError("THERE IS NO NODE WHOSE NODEID IS " + nodeData.nodeId);

			return;
		}

		foreach(ReplayNodeData data in nodeData.nodes)
		{
			if(data.nodeType == ReplayorNodeTypeData.AUTO_FIGHT)
			{
				ReplayAutoFight raf = ReplayAutoFight.createReplayAutoFight(data.delayTime);

				node.replayList.Add(raf);
			}
			else if(data.nodeType == ReplayorNodeTypeData.ATTACK)
			{
				ReplayAttacked ra = ReplayAttacked.createReplayAttack(data.delayTime, data.hpValue);

				node.replayList.Add(ra);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_ATTACK)
			{
				ReplayKingAttack rka = ReplayKingAttack.createReplayKingAttack(data.delayTime, new Vector3(data.offsetX, data.offsetY, data.offsetZ));
		
				node.replayList.Add(rka);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_WEAPON)
			{
				ReplayKingWeapon rkw = ReplayKingWeapon.createReplayKingWeapon(data.delayTime, data.weapon);

				node.replayList.Add(rkw);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_INSPIRE)
			{
				ReplayKingInspire rki = ReplayKingInspire.createReplayKingInspire(data.delayTime);
			
				node.replayList.Add(rki);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_MOVE)
			{
				ReplayKingMove rkm = ReplayKingMove.createReplayKingMove(data.delayTime, new Vector3(data.offsetX, data.offsetY, data.offsetZ));
			
				node.replayList.Add(rkm);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_REVISE)
			{
				ReplayKingRevise rkr = ReplayKingRevise.createReplayKingRevise(data.delayTime, new Vector3(data.offsetX, data.offsetY, data.offsetZ));
			
				node.replayList.Add(rkr);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_FOWARD)
			{
				ReplayKingForward rkf = ReplayKingForward.createReplayKingForward(data.delayTime, new Vector3(data.offsetX, data.offsetY, data.offsetZ));
			
				node.replayList.Add(rkf);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_RELIF)
			{
				ReplayKingRelief rkr = ReplayKingRelief.createReplayKingRelief(data.delayTime);

				node.replayList.Add(rkr);
			}
			else if(data.nodeType == ReplayorNodeTypeData.KING_SKILL)
			{
				ReplayKingSkill rks = ReplayKingSkill.createReplayKingSkill(data.delayTime, data.skillType);

				node.replayList.Add(rks);
			}
		}

		node.startReplay();
	}

	public IEnumerator reliefTroop()
	{
		TroopReplay troop = createTroop(heroTemple_Dun, 30000, TroopReplay.Stance.STANCE_SELF, selfPositions[0], null);

		yield return new WaitForSeconds(.1f);

		troop.setTargetPosition(getKing().position);
	}

	public void troopAIStartComplete()
	{
		startCount ++;

		bool complete = startComplete();

		if(complete)
		{
			ReplayUIControllor.Instance().addAvatar();
		}
	}

	public bool startComplete()
	{
		if(startCount == 0) return false;

		if(startCount >= selfTroops.Count + enemyTroops.Count) return true;

		return false;
	}

	void FixedUpdate ()
	{
		if(startComplete() == false) return;

		if(result != BattleResult.RESULT_BATTLING) return;

		checkResult();

		if(result != BattleResult.RESULT_BATTLING)
		{
			foreach(TroopReplay troop in selfTroops)
			{
				ReplayNode hero = troop.hero;

				hero.isAlive = false;

				foreach(ReplayNode  soldier in troop.soldiers)
				{
					soldier.isAlive = false;
				}
			}

			foreach(TroopReplay troop in enemyTroops)
			{
				ReplayNode hero = troop.hero;
				
				hero.isAlive = false;
				
				foreach(ReplayNode soldier in troop.soldiers)
				{
					soldier.isAlive = false;
				}
			}

			resultControllor.gameObject.SetActive(true);
			
			if(result == BattleResult.RESULT_WIN)
			{
				//resultControllor.showWin();
			}
			else if(result == BattleResult.RESULT_LOSE)
			{
				//resultControllor.showLose();
			}
		}
	}

	private void checkResult()
	{
		if(enemyTroops.Count == 0) result = BattleResult.RESULT_WIN;

		if(!king.isAlive) result = BattleResult.RESULT_LOSE;
	}

	public float getAttackValue(ReplayNode attacker, ReplayNode defender)
	{
		//【基础伤害】=【A攻击】*(【A攻击】+【系数A】)/(【A攻击】+【B防御】*【系数B】+【系数C】)
		//【最终伤害】=【基础伤害】*(1+【伤害加深】-【伤害减免】)+【固定伤害】
		//【系数A】=2
		//【系数B】=2
		//【系数C】=5
		//【系数D】=5
		//【系数E】=50
		//【系数F】=1.2
		//【系数G】=0.8
		//【系数H】=1.05

		float a = 2f;

		float b = 2f;

		float c = 5f;

		//float d = 5f;

		//float e = 50f;

		//float f = 1.2f;

		//float g = 0.8f;

		//float  h = 1.05f;

		float baseValue = attacker.attackValue * (attacker.attackValue + a) / (attacker.attackValue + defender.defenceValue * b + c);

		return baseValue;
	}

	public TroopReplay.HeroType getAttackTargetType_1(TroopReplay.HeroType heroType)
	{
		if(heroType == TroopReplay.HeroType.TYPE_CHE)
		{
			return TroopReplay.HeroType.TYPE_DUN;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_DUN)
		{
			return TroopReplay.HeroType.TYPE_QI;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_GONG)
		{
			return TroopReplay.HeroType.TYPE_QIANG;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_QI)
		{
			return TroopReplay.HeroType.TYPE_CHE;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_QIANG)
		{
			return TroopReplay.HeroType.TYPE_DUN;
		}

		return TroopReplay.HeroType.TYPE_DUN;
	}

	public TroopReplay.HeroType getAttackTargetType_2(TroopReplay.HeroType heroType)
	{
		if(heroType == TroopReplay.HeroType.TYPE_CHE)
		{
			return TroopReplay.HeroType.TYPE_GONG;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_DUN)
		{
			return TroopReplay.HeroType.TYPE_GONG;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_GONG)
		{
			return TroopReplay.HeroType.TYPE_QI;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_QI)
		{
			return TroopReplay.HeroType.TYPE_QIANG;
		}
		else if(heroType == TroopReplay.HeroType.TYPE_QIANG)
		{
			return TroopReplay.HeroType.TYPE_CHE;
		}

		return TroopReplay.HeroType.TYPE_DUN;
	}

	public ReplayNode getKing()
	{
		return king;
	}
	
}