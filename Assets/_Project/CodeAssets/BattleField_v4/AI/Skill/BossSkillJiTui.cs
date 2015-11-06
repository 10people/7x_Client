using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  BOSS击退
*/
public class BossSkillJiTui : HeroSkill 
{

	public override bool castSkill()
	{
		List<BaseAI> nodeList = node.stance == BaseAI.Stance.STANCE_ENEMY ? 
			BattleControlor.Instance ().selfNodes : BattleControlor.Instance ().enemyNodes; 

		foreach(BaseAI t_node in nodeList)
		{
			if(Vector3.Distance(t_node.transform.position, node.transform.position) < template.value2)
			{
				node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);

				return true;
			}
		}

		return false;
	}

	public override void activeSkill(int state)
	{
		List<BaseAI> nodeList = node.stance == BaseAI.Stance.STANCE_ENEMY ? 
			BattleControlor.Instance ().selfNodes : BattleControlor.Instance ().enemyNodes; 

		Vector3 tempFow = transform.forward;

		foreach(BaseAI t_node in nodeList)
		{
			float length = Vector3.Distance(t_node.transform.position, node.transform.position);

			if(length < template.value2)
			{
				transform.forward = t_node.transform.position - transform.position;
				
				Vector3 targetP = transform.position + transform.forward * (length + template.value1);
				
				StartCoroutine(t_node.attackedMovement(0.05f, targetP, iTween.EaseType.easeOutExpo, 0.2f));
				
				t_node.mAnim.SetTrigger("KnockDown");
			}
		}

		transform.forward = tempFow;

		BattleEffectControllor.Instance().PlayEffect(
			25, 
			gameObject);
	}
	
	public override void upData()
	{
		
	}
}
