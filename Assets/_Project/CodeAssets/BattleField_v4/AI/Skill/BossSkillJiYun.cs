using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  BOSS击晕
*/
public class BossSkillJiYun : HeroSkill 
{
	private Vector3 targetPosition;
	
	
	public override void init()
	{
		
	}
	
	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);
		return true;
	}
	
	public override void activeSkill(int state)
	{
		if(node.targetNode == null || node.targetNode.isAlive == false)
		{
			targetPosition = node.transform.position;
		}
		else
		{
			targetPosition = node.targetNode.transform.position;
		}
		
		BattleEffectControllor.Instance().PlayEffect (
			60,
			targetPosition, node.transform.forward);

		StartCoroutine (callbackAction());
	}

	IEnumerator callbackAction()
	{
		yield return new WaitForSeconds (1.0f);

		skillJiYunCallback_2 ();
	}

	public void skillJiYunCallback_2()
	{
		List<BaseAI> nodeList = node.stance == BaseAI.Stance.STANCE_ENEMY ? 
			BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes; 
		
		foreach(BaseAI t_node in nodeList)
		{
			if(t_node == null || t_node.isAlive == false) continue;
			
			float length = Vector3.Distance(t_node.transform.position, targetPosition);
			
			if(length > template.value2) continue;
			
			FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(
				node, 
				t_node, 
				template.value1,
				0);
			
			node.attackHp(t_node, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);
		}
	}
	
	public override void upData()
	{
		
	}
}
