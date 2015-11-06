using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  BOSS加深
*/
public class BossSkillJiaShen : HeroSkill 
{

	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);

		return true;
	}

	public override void activeSkill(int state)
	{
		BattleEffectControllor.Instance ().PlayEffect (
			70,
			gameObject);

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.isAlive == false) continue;

			Buff.createBuff(node, AIdata.AttributeType.ATTRTYPE_attackAmplify, template.value1, 0);
		}
	}
	
	public override void upData()
	{
		
	}
}
