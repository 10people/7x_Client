using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  BOSS减免
*/
public class BossSkillJianMian : HeroSkill
{

	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);

		return true;
	}
	
	public override void activeSkill(int state)
	{
//		BattleEffectControllor.Instance().PlayEffect (
//			BattleEffectControllor.EffectType.EFFECT_KING_CESHI_SKILL_BAOQI,
//			node.targetNode.gameObject);
//		Buff.createBuff(node.targetNode, Buff.BuffType.BUFF_HP, template.value1, template.value2);

		BattleEffectControllor.Instance().PlayEffect (
			69,
			gameObject);

		foreach(BaseAI node in BattleControlor.Instance().enemyNodes)
		{
			if(node.isAlive == false) continue;
			
			Buff.createBuff(node, AIdata.AttributeType.ATTRTYPE_attackReduction, template.value1, 0);
		}
	}

	public override void upData()
	{
		
	}
}
