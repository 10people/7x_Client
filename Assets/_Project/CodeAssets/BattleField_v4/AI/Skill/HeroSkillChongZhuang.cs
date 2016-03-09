using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

/*
*  冲撞
*/
public class HeroSkillChongZhuang : HeroSkill
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
		speedBuff = Buff.createBuff (node, AIdata.AttributeType.ATTRTYPE_moveSpeed, node.nodeData.GetAttribute( AIdata.AttributeType.ATTRTYPE_moveSpeed ) * 2, 0);

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

			node.mAnim.SetTrigger ("ChongZhuang");

			if(node.targetNode.nodeData.nodeType == NodeType.SOLDIER)
			{
				inRunState = 2;
			}
			else
			{
				inRunState = 0;
			}
		}
		else if(inRunState == 2)
		{
			if(Vector3.Distance(node.transform.position, BattleControlor.Instance().getKing().transform.position) < 7)
			{
				node.setTargetPosition(Vector3.zero);

				inRunState = 0;
			}
		}
	}

	public void activeSkillChongZhuang()
	{
		FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill (node, node.targetNode, template.value1, 0);

		float v = fbp.Float;

		node.attackHp(node.targetNode, v, fbp.Bool ,BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);
	}

	public void activeSkillChongZhuangDone()
	{
		if (inRunState != 2) return;

		if(node.targetNode.nodeData.nodeType == NodeType.SOLDIER)
		{
			node.setTargetPosition (BattleControlor.Instance().getKing().transform.position);
		}
	}

	public override void activeSkill(int state)
	{
		
	}
	
	public override void upData()
	{
		
	}
}
