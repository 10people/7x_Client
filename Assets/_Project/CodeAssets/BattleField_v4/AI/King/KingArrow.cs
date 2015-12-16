using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class KingArrow : MonoBehaviour {

	public delegate void Callback(BaseAI defender);

	public Callback m_callback;


	[HideInInspector] public bool continueArrow;

	[HideInInspector] public float shanghaixishu;

	[HideInInspector] public float gudingshanghai;

	[HideInInspector] public int aid;


	//private static Dictionary<string, GameObject> arrowTemples = new Dictionary<string, GameObject> ();

	private BaseAI attackerNode;

	private List<GameObject> boomObjects = new List<GameObject>();

	private Vector3 targetPosition;

	private List<Vector3> boomPosition = new List<Vector3>();

	private System.DateTime startTime;

	private List<BaseAI> tempNodes = new List<BaseAI>();

	private BaseAI targetNode;

	private float hurtValue;

	private bool cri;

	private BattleControlor.AttackType attackType;


	void Start()
	{
		if (attackerNode == null) return;

		boomObjects.Clear();

		boomPosition.Clear();

		tempNodes.Clear();
	}

//	public static void LoadRes( int effectId )
//	{
//		Global.ResourcesDotLoad ( EffectTemplate.getEffectTemplateByEffectId( effectId ).path, 
//		                         LoadResCallback );
//	}
//
//	public static void LoadResCallback(ref WWW p_www, string p_path, Object p_object)
//	{
//		LoadingHelper.ItemLoaded( StaticLoading.CONST_BATTLE_LOADING_FX, p_path );
//
//		if( arrowTemples.ContainsKey ( p_path ) ) return;
//
//		arrowTemples.Add (p_path, p_object as GameObject);
//	}

	public static KingArrow createArrow(BaseAI node, BaseAI _targetNode, float _hurtValue, bool _cri, BattleControlor.AttackType attackType, int effectId = 50)
	{
		node.transform.forward = _targetNode.transform.position - node.transform.position;

		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (effectId);

		GameObject temple = BattleEffectControllor.Instance().getEffect(effectId);

		GameObject bulletObject = new GameObject();

		bulletObject.transform.localScale = new Vector3(1, 1, 1);

		bulletObject.transform.position = node.transform.position + new Vector3(0, 1.5f, 0);

		bulletObject.transform.forward = -node.transform.forward;

		bulletObject.SetActive(true);


		BattleEffectControllor.Instance ().PlayEffect (effectId, bulletObject);

		KingArrow arrow = (KingArrow)bulletObject.AddComponent<KingArrow>();

		arrow.targetPosition = _targetNode.transform.position;

		arrow.startTime = System.DateTime.Now;

		arrow.attackerNode = node;

		arrow.hurtValue = _hurtValue;

		arrow.cri = _cri;

		arrow.attackType = attackType;

		arrow.targetNode = _targetNode;

		arrow.continueArrow = false;

		arrow.aid = 0;

		arrow.m_callback = null;

		TweenPosition.Begin(bulletObject, 0.5f, arrow.targetPosition);
		
		return arrow;
	}

	public static KingArrow createArrow(BaseAI node, BattleControlor.AttackType attackType, int effectId = 50)
	{
		EffectIdTemplate et = EffectTemplate.getEffectTemplateByEffectId (50);
		
		GameObject temple = BattleEffectControllor.Instance().getEffect(effectId);
		
		//arrowTemples.TryGetValue (et.path, out temple);
		
		GameObject bulletObject = new GameObject();

		bulletObject.transform.localScale = new Vector3(1, 1, 1);

		bulletObject.transform.position = node.transform.position + new Vector3(0, 1.5f, 0);

		bulletObject.transform.forward = node.transform.forward;

		bulletObject.SetActive(true);
		
		BattleEffectControllor.Instance ().PlayEffect (effectId, bulletObject);

		KingArrow arrow = (KingArrow)bulletObject.AddComponent<KingArrow>();

		arrow.targetPosition = bulletObject.transform.position + node.transform.forward * node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange );// + new Vector3(0, 1.5f, 0);

		arrow.startTime = System.DateTime.Now;

		arrow.attackerNode = node;

		arrow.hurtValue = 0;

		arrow.attackType = attackType;

		arrow.targetNode = null;

		arrow.continueArrow = false;

		arrow.aid = 0;

		arrow.m_callback = null;

		TweenPosition.Begin(bulletObject, 0.5f, arrow.targetPosition);

		return arrow;
	}

	public static KingArrow createArrow(ReplayNode node)
	{
		return null;
	}

	public void setCallback(Callback callback)
	{
		m_callback = callback;
	}

	void FixedUpdate()
	{
		if (attackerNode == null) return;

		updateArrow ();

		updataBoom();

		System.DateTime tempTime = System.DateTime.Now;
		
		System.TimeSpan ts = tempTime - startTime;
		
		int num = (int)ts.TotalMilliseconds;

		if(num > 500)
		{
			Destroy(gameObject);
		}
	}

	private void updateArrow()
	{
		float r1 = 2f;

		float r2 = 1.2f;

		if(aid == 260)
		{
			r1 = 3f;

			r2 = 3f;
		}

		if (targetNode != null)
		{
			if(Vector3.Distance(targetNode.transform.position, attackerNode.transform.position) < r1)
			{
				attackerNode.attackHp(targetNode, hurtValue, cri, attackType);
			}

			playBoom(targetNode);

			return;	
		}

		List<BaseAI> nodeList = attackerNode.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance().enemyNodes : BattleControlor.Instance().selfNodes;

		foreach(BaseAI node in nodeList)
		{
			if(node != null && node.isAlive && node.gameObject.activeSelf == true &&
			   Vector3.Distance(node.transform.position + new Vector3(0, -node.transform.position.y, 0), transform.position + new Vector3(0, -transform.position.y, 0)) < r2
			   && node.nodeData.nodeType != NodeType.GOD
			   && node.nodeData.nodeType != NodeType.NPC)
			{
				bool flag = checkTempNodes(node);

				if(flag == false)
				{
					if(attackerNode.nodeData.nodeType == NodeType.PLAYER)
					{
						if(attackType == BattleControlor.AttackType.BASE_ATTACK)
						{
							FloatBoolParam fbp = BattleControlor.Instance().getAttackValue(attackerNode, node, BattleControlor.Instance().getKing().weaponDateRanged.weaponRatio[BattleControlor.Instance().getKing().hitCount]);

							foreach(Buff buff in node.buffs)
							{
								if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
								{
									attackerNode.attackHp(attackerNode, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, attackType);
									
									fbp.Float = buff.supplement.m_fValue1 * fbp.Float;
									
									break;
								}
							}

							attackerNode.attackHp(node, fbp.Float, fbp.Bool, attackType);
						}
						else if(attackType == BattleControlor.AttackType.SKILL_ATTACK)
						{
							FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(attackerNode, node, shanghaixishu, gudingshanghai);
							
							attackerNode.attackHp(node, fbp.Float, fbp.Bool, attackType);
						}
					}
					else
					{
						FloatBoolParam fbp = BattleControlor.Instance().getAttackValue(attackerNode, node);

						foreach(Buff buff in node.buffs)
						{
							if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
							{
								attackerNode.attackHp(attackerNode, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, attackType);
								
								fbp.Float = buff.supplement.m_fValue1 * fbp.Float;
								
								break;
							}
						}

						attackerNode.attackHp(node, fbp.Float, fbp.Bool, attackType);
					}

					playBoom(node);

					return;
				}
			}
		}
	}

	private void updataBoom()
	{
		for(int i = 0; i < boomObjects.Count; i++)
		{
			GameObject gc = boomObjects[i];

			Vector3 v = boomPosition[i];

			gc.transform.position = v + new Vector3(0, 1.5f, 0);
		}
	}

	private void playBoom(BaseAI node)
	{
		if(continueArrow)
		{
			tempNodes.Add(node);
		}

		boomPosition.Add(node.transform.position);

		attackerNode.arrowAttackCallback (aid, node);

		if(m_callback != null)
		{
			m_callback(node);
		}

		gameObject.SetActive(continueArrow);
	}

	private bool checkTempNodes(BaseAI node)
	{
		foreach(BaseAI ai in tempNodes)
		{
			if(ai.nodeId == node.nodeId)
			{
				return true;
			}
		}

		if(aid == 261)
		{
			if(node.nodeData.GetAttribute(AIdata.AttributeType.ATTRTYPE_Ice) > 1)
			{
				return true;
			}
		}

		return false;
	}

}
