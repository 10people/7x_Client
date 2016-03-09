using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KingWeapon : MonoBehaviour
{
	public enum ColliderType
	{
		BOX,
		SPHERE,
	}


	public ColliderType colliderType;

	public Vector3 colliderCenter;

	public Vector3 boxSize;

	public float sphereR;


	[HideInInspector] public int hand;

	[HideInInspector] public bool rightnow;

	[HideInInspector] public bool hitTarget;//是否打中了目标


	private BaseAI node;

	private List<BaseAI> a_nodes = new List<BaseAI>();

	private bool isSkill;

	private float shanghaixishu;

	private float gudingshanghai;


	public void initWeapon(BaseAI _node, int _hand, bool _rightnow = false)
	{
		node = _node;

		hand = _hand;

		rightnow = _rightnow;

		hitTarget = false;

		a_nodes.Clear();
	}

	public void setTriggerable(bool _triggerable)
	{
		hitTarget = false;

		if(_triggerable == true) a_nodes.Clear();

		else
		{
			if(rightnow == false) trigger();

			a_nodes.Clear();
		}

		isSkill = false;
	}

	public void setTriggerableSkill(bool _triggerable, float _shanghaixishu, float _gudingshanghai)
	{
		a_nodes.Clear();
		
		isSkill = true;

		shanghaixishu = _shanghaixishu;

		gudingshanghai = _gudingshanghai;
	}

	private void UpdateList()
	{
		a_nodes.Clear ();

		List<BaseAI> list = new List<BaseAI> ();

		foreach(BaseAI t_node in (node.stance == BaseAI.Stance.STANCE_ENEMY ? BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes))
		{
			if(t_node == null || t_node.isAlive == false || t_node.gameObject.activeSelf == false) continue;

			list.Add(t_node);
		}

		foreach(BaseAI t_node in BattleControlor.Instance().midNodes)
		{
			if(t_node == null || t_node.isAlive == false || t_node.gameObject.activeSelf == false) continue;
			
			list.Add(t_node);
		}

		Collider coll = gameObject.GetComponent<Collider>();

//		BoxCollider boxClo = coll as BoxCollider;
//
//		SphereCollider sphClo = coll as SphereCollider;

		if(colliderType == ColliderType.BOX)
		{
			foreach(BaseAI t_node in list)
			{
				updateBoxColliderList(t_node);
			}
		}
		else if(colliderType == ColliderType.SPHERE)
		{
			foreach(BaseAI t_node in list)
			{
				updateSphereColliderList(t_node);
			}
		}
	}

	private void updateBoxColliderList( BaseAI t_node)
	{
		Vector3 pos2 = t_node.transform.position;
		
		Vector3 p4 = node.transform.position + node.transform.forward * colliderCenter.z;
		
		pos2.y = 0;
		
		p4.y = 0;
	
		Vector3 m_VForWard = node.transform.forward;

		float m_iCollValue1 = boxSize.z;

		float m_iCollValue2 = boxSize.x;

		Vector3 p0 = p4 - ((m_VForWard * m_iCollValue1) / 2);

		Vector3 p1 = p4 + ((m_VForWard * m_iCollValue1) / 2);

		Vector3 p2 = p4 + (new Vector3(-m_VForWard.z, 0f, m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));

		Vector3 p3 = p4 + (new Vector3(m_VForWard.z, 0f, -m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));

		if(Global.getCollRect(pos2, p1, p0, p3, p2))
		{
			a_nodes.Add(t_node);
		}
	}

	private void updateSphereColliderList(BaseAI t_node)
	{
		Vector3 pos2 = t_node.transform.position;

		Vector3 p4 = node.transform.position + node.transform.forward * colliderCenter.z;

		pos2.y = 0;
		
		p4.y = 0;

		if(Vector3.Distance(pos2, p4) < sphereR)
		{
			a_nodes.Add(t_node);
		}
	}

	public void OnTriggerEnters(Collider other)
	{
		if (BattleControlor.Instance().inDrama) return;

		if (node == null) return;

		BaseAI t_node = (BaseAI)other.gameObject.GetComponent("BaseAI");
		
		if(t_node == null 
		   || !t_node.isAlive 
		   || t_node.stance == node.stance 
		   || t_node.nodeData.nodeType == qxmobile.protobuf.NodeType.NPC
		   || t_node.nodeData.nodeType == qxmobile.protobuf.NodeType.GOD)
		{
			return;
		}

		bool f = checkNodes(t_node.nodeId);
		
		if(f == false) return;

		a_nodes.Add(t_node);

		if(rightnow == true)
		{
			EnterAttack(t_node);
		}
	}

	private void trigger()
	{
		UpdateList ();

		hitTarget = a_nodes.Count > 0;

		if(hand != 1)
		{
			foreach(BaseAI t_node in a_nodes)
			{
				EnterAttack(t_node);
			}

			return;
		}

		BaseAI tar = null;

		float tempAlpha = 500;

		foreach(BaseAI t_node in a_nodes)
		{
			float alpha = Vector3.Angle(t_node.transform.position - node.transform.position, node.transform.forward);

			if(alpha < tempAlpha)
			{
				tempAlpha = alpha;

				tar = t_node;
			}
		}

		EnterAttack (tar);
	}

	private void EnterAttack(BaseAI t_node)
	{
		if(node == null)
		{
			NormalEnter(t_node);

			return;
		}

		if(t_node == null 
		   || !t_node.isAlive
		   || t_node.stance == node.stance 
		   || t_node.nodeData.nodeType == qxmobile.protobuf.NodeType.NPC
		   || t_node.nodeData.nodeType == qxmobile.protobuf.NodeType.GOD)
		{
			return;
		}

		float weaponRatio = 1;

		KingControllor kc = (KingControllor)node.GetComponent ("KingControllor");

		int hitCount = 0;

		if(kc != null)
		{
			hitCount = kc.hitCount - 1;

			hitCount = hitCount < 0 ? 3 : hitCount;
		}

		if(kc != null && kc.weaponType == KingControllor.WeaponType.W_Heavy) weaponRatio = kc.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_Heavy_weaponRatio_1 + hitCount );

		else if(kc != null && kc.weaponType == KingControllor.WeaponType.W_Light) weaponRatio = kc.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_Light_weaponRatio_1 + hitCount );

		else if(kc != null && kc.weaponType == KingControllor.WeaponType.W_Ranged) weaponRatio = kc.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_Range_weaponRatio_1 + hitCount );

		BattleControlor.AttackType at = BattleControlor.AttackType.BASE_ATTACK;
		
		if(node.isPlayingSkill() == true) at = BattleControlor.AttackType.SKILL_ATTACK;

		FloatBoolParam fbp;

		if(isSkill == true)
		{
			fbp = BattleControlor.Instance().getAttackValueSkill(node, t_node, shanghaixishu, gudingshanghai);

			foreach(Buff buff in t_node.buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
				{
					node.attackHp(node, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_REFLEX, BattleControlor.NuqiAddType.NULL);
					
					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

					t_node.showText(LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

					break;
				}
			}
		}
		else
		{
			fbp = BattleControlor.Instance().getAttackValue(node, t_node, weaponRatio);

			foreach(Buff buff in t_node.buffs)
			{
				if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
				{
					node.attackHp(node, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.BASE_REFLEX, BattleControlor.NuqiAddType.NULL);
					
					fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

					t_node.showText(LanguageTemplate.GetText( LanguageTemplate.Text.BATTLE_BASE_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

					break;
				}
			}
		}

		BattleControlor.NuqiAddType nuqiType = BattleControlor.NuqiAddType.NULL;

		if(kc != null)
		{
			if(kc.actionId == 200 || kc.actionId == 201 || kc.actionId == 202 || kc.actionId == 203)//重武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_BASE;
			}
			else if(kc.actionId == 250)//重武器技能1-八荒烈日
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_SKILL_1;
			}
			else if(kc.actionId == 252)//重武器技能2-乾坤斗转
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_SKILL_2;
			}
			else if(kc.actionId == 101 || kc.actionId == 103 || kc.actionId == 105 || kc.actionId == 106)//轻武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_BASE;
			}
			else if(kc.actionId >= 140 && kc.actionId <= 151)//轻武器技能1-绝影星光斩
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_SKILL_1;
			}
			else if(kc.actionId == 160)//轻武器技能2-血迹烙印
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_SKILL_2;
			}
			else if(kc.actionId == 262 || kc.actionId == 263)//弓武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_BASE;
			}
			else if(kc.actionId == 260)//弓武器节能1-追星箭
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_SKILL_1;
			}
			else if(kc.actionId == 261)//弓武器技能2-寒冰箭
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_SKILL_2;
			}
		}

		node.attackHp(t_node, fbp.Float, fbp.Bool, at, nuqiType);

		//StartCoroutine (attackHp(t_node, v));
	}

	private void NormalEnter(BaseAI t_node)
	{
		if (t_node == null) return;

		if (t_node.stance == BaseAI.Stance.STANCE_ENEMY) return;

		BaseAI ba = (BaseAI)transform.parent.GetComponent ("BaseAI");
		
		FloatBoolParam fbp = BattleControlor.Instance().getAttackValue(ba, t_node);

		foreach(Buff buff in t_node.buffs)
		{
			if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_WEAPON)
			{
				ba.attackHp(ba, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.BASE_REFLEX, BattleControlor.NuqiAddType.NULL);
				
				fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

				t_node.showText(LanguageTemplate.GetText( LanguageTemplate.Text.BATTLE_BASE_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

				break;
			}
		}

		BattleControlor.NuqiAddType nuqiType = BattleControlor.NuqiAddType.NULL;

		KingControllor kc = (KingControllor)node.GetComponent ("KingControllor");

		if(kc != null)
		{
			if(kc.actionId == 200 || kc.actionId == 201 || kc.actionId == 202 || kc.actionId == 203)//重武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_BASE;
			}
			else if(kc.actionId == 250)//重武器技能1-八荒烈日
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_SKILL_1;
			}
			else if(kc.actionId == 252)//重武器技能2-乾坤斗转
			{
				nuqiType = BattleControlor.NuqiAddType.HEAVY_SKILL_2;
			}
			else if(kc.actionId == 101 || kc.actionId == 103 || kc.actionId == 105 || kc.actionId == 106)//轻武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_BASE;
			}
			else if(kc.actionId >= 140 && kc.actionId <= 151)//轻武器技能1-绝影星光斩
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_SKILL_1;
			}
			else if(kc.actionId == 160)//轻武器技能2-血迹烙印
			{
				nuqiType = BattleControlor.NuqiAddType.LIGHT_SKILL_2;
			}
			else if(kc.actionId == 262 || kc.actionId == 263)//弓武器普攻
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_BASE;
			}
			else if(kc.actionId == 260)//弓武器节能1-追星箭
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_SKILL_1;
			}
			else if(kc.actionId == 261)//弓武器技能2-寒冰箭
			{
				nuqiType = BattleControlor.NuqiAddType.RANGE_SKILL_2;
			}
		}

		ba.attackHp(t_node, fbp.Float, fbp.Bool, BattleControlor.AttackType.BASE_ATTACK, nuqiType);
	}

	/*
	IEnumerator attackHp(BaseAI defender, float v)
	{
		yield return new WaitForSeconds (0.05f);

		node.attackHp(defender, v);
	}
	*/

	private bool checkNodes(int id)
	{
		if(node.nodeData.nodeType != qxmobile.protobuf.NodeType.PLAYER
		   && a_nodes.Count > 0) return false;

		foreach(BaseAI b in a_nodes)
		{
			if(b.nodeId == id)
			{
				return false;
			}
		}

		return true;
	}

}
