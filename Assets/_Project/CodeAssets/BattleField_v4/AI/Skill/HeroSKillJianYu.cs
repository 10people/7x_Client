using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  箭雨
*/
public class HeroSKillJianYu : HeroSkill
{
	private Vector3 tempPosition;


	public override void init()
	{

	}

	public override bool castSkill()
	{
		node.mAnim.SetTrigger ("Skill_2");

		tempPosition = node.targetNode.transform.position;

		return true;
	}

	public void activeSkill()
	{
		BattleEffectControllor.Instance().PlayEffect (49, tempPosition, node.transform.forward);

		StartCoroutine (hurt());
	}

	private IEnumerator hurt()
	{
		yield return new WaitForSeconds(0.2f);

		FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(node, node.targetNode, template.value2, 0);

		float av = fbp.Float;

		node.attackHp(node.targetNode, av, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);

		List<BaseAI> list = node.stance == BaseAI.Stance.STANCE_ENEMY ? BattleControlor.Instance().selfNodes : BattleControlor.Instance().enemyNodes;

		foreach(BaseAI n in list)
		{
			bool flag = node.targetNode == null;

			flag = flag == true ? true : n.nodeId != node.targetNode.nodeId;

			if(n != null && n.isAlive && flag == true && Vector3.Distance(n.transform.position, tempPosition) < 3.5f)
			{
				fbp = BattleControlor.Instance().getAttackValueSkill(node, n, template.value2, 0);

				node.attackHp(n, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK, BattleControlor.NuqiAddType.NULL);
			}
		}
	}
	
	public override void upData()
	{
		
	}
}
