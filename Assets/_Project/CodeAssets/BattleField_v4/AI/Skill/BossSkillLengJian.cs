using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  BOSS冷箭
*/
public class BossSkillLengJian : HeroSkill
{
	private BaseAI tempNode;
	
	
	public override void init()
	{

	}
	
	public override bool castSkill()
	{
		tempNode = null;
		
		List<BaseAI> nodes = node.stance == BaseAI.Stance.STANCE_SELF ? BattleControlor.Instance ().enemyNodes : BattleControlor.Instance ().selfNodes;

		foreach(BaseAI n in nodes)
		{
			if(Vector3.Distance(n.transform.position, node.transform.position) < 15)
			{
				if(n.nodeData.nodeType == NodeType.PLAYER
				   || n.nodeData.nodeType == NodeType.BOSS
				   || n.nodeData.nodeType == NodeType.HERO)
				{
					tempNode = n;
					
					break;
				}
			}
		}
		
		if (tempNode == null)
		{
			foreach(BaseAI n in nodes)
			{
				if(Vector3.Distance(n.transform.position, node.transform.position) < 15)
				{
					if(n.nodeData.nodeType == NodeType.HERO)
					{
						tempNode = n;
						
						break;
					}
				}
			}
		}

		if (tempNode == null) return false;
		
		node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);
		return true;
	}
	
	public override void activeSkill(int state)
	{
		FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(node, tempNode, template.value1, 0);
		
		KingArrow arrow = KingArrow.createArrow(node, tempNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, 68);
	
		arrow.setCallback (skillLengJianHit);
	}

	public void skillLengJianHit(BaseAI defender)
	{
		Buff.createBuff (defender, AIdata.AttributeType.ATTRTYPE_moveSpeed, template.value2, template.value3);
	}

	public override void upData()
	{
		
	}
}
