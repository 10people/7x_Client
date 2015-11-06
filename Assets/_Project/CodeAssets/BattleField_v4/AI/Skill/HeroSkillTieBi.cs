using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  铁壁
*/
public class HeroSkillTieBi : HeroSkill
{
	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("TieBi");

		return true;
	}
	
	public void activeSkillTieBi()
	{
		Buff.createBuff (node, AIdata.AttributeType.ATTRTYPE_attackReduction, template.value1, 0);
	}

	public override void activeSkill(int state)
	{
		
	}
	
	public override void upData()
	{
		
	}
}
