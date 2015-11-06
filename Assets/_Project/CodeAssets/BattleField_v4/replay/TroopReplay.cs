using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TroopReplay : MonoBehaviour
{
	public enum Stance
	{
		STANCE_SELF,
		STANCE_ENEMY,
		STANCE_MID,
	}
	
	public enum HeroType
	{
		TYPE_DUN,
		TYPE_QIANG,
		TYPE_GONG,
		TYPE_CHE,
		TYPE_QI,
		TYPE_KING,
		TYPE_PLAYER,
	}
	
	public ReplayNode hero;
	
	
	[HideInInspector] public ReplayNode soldierTemple_Che;
	
	[HideInInspector] public ReplayNode soldierTemple_Dun;
	
	[HideInInspector] public ReplayNode soldierTemple_Qiang;
	
	[HideInInspector] public ReplayNode soldierTemple_Qi;
	
	[HideInInspector] public ReplayNode soldierTemple_Gong;
	
	[HideInInspector] public List<GameObject> soldierPositionFlag;
	
	[HideInInspector] public GameObject shadowTemple;
	
	[HideInInspector] public Stance stance;
	
	[HideInInspector] public HeroType heroType;
	
	[HideInInspector] public Soldier soldierDate;
	
	[HideInInspector] public int soldierNum;
	
	[HideInInspector] public List<ReplayNode> soldiers = new List<ReplayNode>();
	
	public TroopReplay targetTroop;
	
	[HideInInspector] public Vector3 position;
	
	[HideInInspector] public bool isAlive;
	
	[HideInInspector] public float soldierHp;
	
	[HideInInspector] public Vector3 targetPosition;
	
	
	private List<Vector3> soldierPositions = new List<Vector3>();
	
	
	public virtual void Start()
	{
		isAlive = true;
		
		updatePosition();
		
		soldierNum = 5;
		
		addSoliders();
		
		hero.init();
		
		foreach(ReplayNode soldier in soldiers)
		{
			soldier.init();
		}
		
		targetPosition = new Vector3(0, 1000, 0);
		
		BattleReplayControlor.Instance().troopAIStartComplete();
	}
	
	private void addSoliders()
	{
		soldierPositions.Clear();
		
		foreach(GameObject flag in soldierPositionFlag) { soldierPositions.Add(flag.transform.localPosition); }
		
		foreach(ReplayNode so in soldiers) { Destroy(so.gameObject); }
		
		soldiers.Clear();
		
		for(int i = 0; i < 5/*(soldierNum < 5 ? soldierNum : 5)*/; i++)
		{
			GameObject soldierTemple = null;
			
			if(heroType == HeroType.TYPE_CHE) soldierTemple = soldierTemple_Che.gameObject;
			else if(heroType == HeroType.TYPE_DUN) soldierTemple = soldierTemple_Dun.gameObject;
			else if(heroType == HeroType.TYPE_GONG) soldierTemple = soldierTemple_Gong.gameObject;
			else if(heroType == HeroType.TYPE_QI) soldierTemple = soldierTemple_Qi.gameObject;
			else if(heroType == HeroType.TYPE_QIANG) soldierTemple = soldierTemple_Qiang.gameObject;
			
			if(soldierTemple == null) break;
			
			GameObject soldierObject = (GameObject)Instantiate(soldierTemple, soldierTemple.transform.position, soldierTemple.transform.rotation);
			
			soldierObject.SetActive(true);
			
			soldierObject.transform.parent = gameObject.transform;
			
			soldierObject.transform.localScale = new Vector3(1, 1, 1);
			
			soldierObject.transform.localPosition = soldierPositions[i];
			
			ReplayNode soldier = (ReplayNode)soldierObject.GetComponent("ReplayNode");
			
			soldier.troop = this;
			
			soldier.attackRange = hero.attackRange;
			
			soldier.shadowTemple = shadowTemple;
			
			soldier.nodeId = hero.nodeId * 100 + i;
			
			if(soldierDate == null)
			{
				soldier.initDate(i);
			}
			else
			{
				soldier.initDate(soldierDate, i);
			}
			
			soldiers.Add(soldier);
			
			soldierHp = soldier.hp;
		}
	}
	
	void FixedUpdate ()
	{
		if(!BattleReplayControlor.Instance().startComplete()) return;
		
		isAlive = checkHp();
		
		if(!isAlive)
		{
			if(stance == Stance.STANCE_ENEMY)
			{
				BattleReplayControlor.Instance().enemyTroops.Remove(this);
			}
			else if(stance == Stance.STANCE_SELF)
			{
				BattleReplayControlor.Instance().selfTroops.Remove(this);
			}
			
			return;
		}
		
		updatePosition();
		
		//if(heroType == HeroType.TYPE_PLAYER) return;
		
		if(targetPosition.y < 900)
		{
			targetTroop = null;
		}
		else
		{
			targetUpdate();
		}
	}
	
	private bool checkHp()
	{
		if(hero != null && hero.isAlive) return true;
		
		foreach(ReplayNode soldier in soldiers)
		{
			if(soldier != null && soldier.isAlive) return true;
		}
		
		if(hero == null)
		{
			foreach(ReplayNode soldier in soldiers)
			{
				if(soldier != null)
				{
					return false;
				}
			}
			
			Destroy(gameObject);
			
			return false;
		}
		
		return false;
	}
	
	public void updatePosition()
	{
		float px = 0;
		
		float py = 0;
		
		float pz = 0;
		
		int count = 0;
		
		if(hero != null && hero.isAlive)
		{
			px += hero.transform.localPosition.x;
			
			py += hero.transform.localPosition.y;
			
			pz += hero.transform.localPosition.z;
			
			count ++;
		}
		
		foreach(ReplayNode node in soldiers)
		{
			if(node != null && node.isAlive)
			{
				px += node.transform.localPosition.x;
				
				py += node.transform.localPosition.y;
				
				pz += node.transform.localPosition.z;
				
				count ++;
			}
		}
		
		px = px / count;
		
		py = py / count;
		
		pz = pz / count;
		
		position = new Vector3(px, py, pz) + gameObject.transform.localPosition;
	}
	
	private void targetUpdate()
	{
		if(targetTroop != null && targetTroop.isAlive) return;
		
		chooseTarget();
	}
	
	private void chooseTarget()
	{
		targetTroop = null;
		
		List<TroopReplay> list = null;
		
		if(stance == Stance.STANCE_SELF) list = BattleReplayControlor.Instance().enemyTroops;
		else if(stance == Stance.STANCE_ENEMY) list = BattleReplayControlor.Instance().selfTroops;
		else return;
		
		float tempLength = 1000000;
		
		foreach(TroopReplay tempTroop in list)
		{
			if(tempTroop == null || !tempTroop.isAlive) continue;
			
			float length = Vector3.Distance(position, tempTroop.position);
			
			if(length < tempLength)
			{
				tempLength = length;
				
				targetTroop = tempTroop;
			}
		}
	}
	
	public void setColor(Color color)
	{
		color = Color.black;
		
		hero.shadowObject.GetComponent<Renderer>().material.color = color;
		
		foreach(ReplayNode soldier in soldiers)
		{
			soldier.shadowObject.GetComponent<Renderer>().material.color = color;
		}
	}
	
	public void clearColor()
	{
		hero.shadowObject.GetComponent<Renderer>().material.color = hero.shadowColor;
		
		foreach(ReplayNode soldier in soldiers)
		{
			soldier.shadowObject.GetComponent<Renderer>().material.color = soldier.shadowColor;
		}
	}
	
	public void setTargetPosition(Vector3 _targetPosition)
	{
		targetPosition = _targetPosition;
		
		hero.setNavMeshDestination(targetPosition);
		
		foreach(ReplayNode soldier in soldiers)
		{
			soldier.setNavMeshDestination(targetPosition);
		}
	}
	
	public void clearTargetPosition()
	{
		targetPosition = new Vector3(0, 1000, 0);
	}

	public void minusHp(ReplayNode attacker, ReplayNode defender, float hpValue)
	{
		defender.hp -= hpValue;
		
		defender.attacked(attacker, hpValue);
		
		if(defender.hp < 0)
		{
			defender.die();
		}
	}

}
