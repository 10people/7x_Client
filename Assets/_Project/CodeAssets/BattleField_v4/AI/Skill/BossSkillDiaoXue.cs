using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  BOSS掉血
*/
public class BossSkillDiaoXue : HeroSkill
{
	
	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("Skill_" + node.m_iUseSkillIndex);
		
		return true;
	}
	
	public override void activeSkill(int state)
	{
		BattleEffectControllor.Instance().PlayEffect (
			46,
			node.targetNode.gameObject);
		Buff.createBuff(node.targetNode, AIdata.AttributeType.ATTRTYPE_hp, template.value1, template.value2);
	}
	
	public override void upData()
	{
		
	}
}
