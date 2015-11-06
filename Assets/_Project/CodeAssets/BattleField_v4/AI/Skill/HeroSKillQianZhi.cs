using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  牵制
*/
public class HeroSKillQianZhi : HeroSkill 
{
	
	private Buff speedBuff;
	
	private int inRunState;//0:stop 1:runOn 2:runOff
	
	
	public override void init()
	{
		speedBuff = null;
		
		inRunState = 0;
	}
	
	public override bool castSkill()
	{
		speedBuff = Buff.createBuff (node, AIdata.AttributeType.ATTRTYPE_moveSpeed, node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_moveSpeed ) * 2, 0);
		
		node.setTargetPosition (node.targetNode.transform.position);
		
		inRunState = 1;
		
		return true;
	}
	
	public void FixedUpdate ()
	{
		if(speedBuff != null
		   && inRunState == 1
		   && Vector3.Distance(node.transform.position, node.targetNode.transform.position) < node.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_attackRange ) )
		{
			node.setTargetPosition(Vector3.zero);
			
			speedBuff.end();
			
			//node.mAnim.SetTrigger ("QianZhi");
			
			inRunState = 0;
		}
	}
	
	public void activeSkillQianZhi()
	{
		Buff.createBuff (node.targetNode, AIdata.AttributeType.ATTRTYPE_attackAmplify, template.value1, template.value4);
	}

	public override void activeSkill(int state)
	{
		
	}
	
	public override void upData()
	{
		
	}
}
