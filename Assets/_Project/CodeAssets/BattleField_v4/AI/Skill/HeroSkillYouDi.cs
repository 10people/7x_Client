using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  诱敌
*/
public class HeroSkillYouDi : HeroSkill
{

	public override bool castSkill()
	{
		float templ = Vector3.Distance (node.targetNode.transform.position, node.transform.position);
		
		if (templ < template.value1)
		{
			node.mAnim.SetTrigger("YouDi");

			node.transform.forward = node.targetNode.transform.position - node.transform.position;
			
			iTween.ValueTo (node.gameObject, iTween.Hash (
				"name", "skill_1",
				"from", node.transform.position,
				"to", node.transform.position + node.transform.forward * (templ - 1.5f),
				"delay", 0,
				"time", 0.1f,
				"easeType", iTween.EaseType.easeOutExpo,
				"onupdate", "PositonTween"
				));

			return true;
		}

		return false;
	}

	public void activeSkillYouDi()
	{
		List<BaseAI> list = node.stance == BaseAI.Stance.STANCE_ENEMY ? BattleControlor.Instance ().selfNodes : BattleControlor.Instance ().enemyNodes;
		
		foreach(BaseAI n in list)
		{
			if(n.isAlive && n.nodeId != node.targetNode.nodeId && Vector3.Distance(n.transform.position, node.transform.position) < template.value2)
			{
				//Buff.createBuff (node.targetNode, Buff.BuffType.BUFF_BITEME, 0, template.value3);

				node.targetNode.targetNode = node;
			}
		}
	}

	public override void activeSkill(int state)
	{
		
	}
	
	public override void upData()
	{
		
	}
}
