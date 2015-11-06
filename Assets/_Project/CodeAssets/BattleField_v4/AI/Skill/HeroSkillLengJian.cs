using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  冷箭
*/
public class HeroSkillLengJian : HeroSkill 
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

		node.mAnim.SetTrigger ("LengJian");

		return true;
	}
	
	public void activeSkillLengJian()
	{
		FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(node, tempNode, template.value1, 0);
		
		KingArrow.createArrow(node, tempNode, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK);
	}

	public override void activeSkill(int state)
	{
		
	}
	
	public override void upData()
	{
		
	}
}
