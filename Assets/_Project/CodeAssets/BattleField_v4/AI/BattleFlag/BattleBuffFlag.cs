using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBuffFlag : MonoBehaviour 
{
	public int flagId;

	public int refreshTime;//从释放技能到刷新出技能所用时间，单位:秒


	private enum FlagState
	{
		state_null,		//什么都没做
		state_effect,	//刷出BUFF特效
	}

	private FlagState state;

	private BaseAI node;

	private int count;

	private float startTime;

	private List<HeroSkill> skillEffects = new List<HeroSkill>();

	private List<HeroSkill> skills = new List<HeroSkill> ();

	private HeroSkill skillDeleteBuff;

	private int curSkillIndex;


	void Start()
	{
		state = FlagState.state_null;

		count = 0;

		curSkillIndex = -1;
	}

	public void setNode(BaseAI _node)
	{
		if(_node.skills.Count > 1)
		{
			node = _node;

			skillDeleteBuff = _node.skills[0];

			skillEffects.Clear();

			skills.Clear();
			
			for(int i = 1; i < _node.skills.Count; i++)
			{
				if(i % 2 == 1)
				{
					skillEffects.Add(_node.skills[i]);
				}
				else
				{
					skills.Add(_node.skills[i]);
				}
			}
		}
	}

	void FixedUpdate ()
	{
		if (node == null) return;

		if (BattleControlor.Instance().completed == false) return;

		if(count == 0 && state == FlagState.state_null)
		{
			castEffect();
		}

		if(state == FlagState.state_effect)
		{
			float length = Vector3.Distance(BattleControlor.Instance().getKing().transform.position, transform.position);
		
			if(length < 2)
			{
				castSkill();

				deleteBuff();
			}
		}
	}

	private void castEffect()
	{
		curSkillIndex = ((int)(Random.value * 1000)) % skillEffects.Count;

		HeroSkill skillEffect = skillEffects[curSkillIndex];

		skillEffect.template.zhudong = false;
		
		int tempTimePeriod = skillEffect.template.timePeriod;

		skillEffect.template.timePeriod = 0;

		skillEffect.upData ();
		
		skillEffect.template.zhudong = true;
		
		skillEffect.template.timePeriod = tempTimePeriod;

		state = FlagState.state_effect;
	}

	private void castSkill()
	{
		HeroSkill skill = skills [curSkillIndex];

		skill.template.zhudong = false;
		
		int tempTimePeriod = skill.template.timePeriod;
		
		skill.template.timePeriod = 0;
		
		skill.upData ();
		
		skill.template.zhudong = true;
		
		skill.template.timePeriod = tempTimePeriod;

		state = FlagState.state_null;

		count ++;

		StartCoroutine (waitForTime());
	}

	private void deleteBuff()
	{
		skillDeleteBuff.template.zhudong = false;
		
		int tempTimePeriod = skillDeleteBuff.template.timePeriod;
		
		skillDeleteBuff.template.timePeriod = 0;
		
		skillDeleteBuff.upData ();
		
		skillDeleteBuff.template.zhudong = true;
		
		skillDeleteBuff.template.timePeriod = tempTimePeriod;
	}

	private IEnumerator waitForTime()
	{
		yield return new WaitForSeconds(refreshTime);

		castEffect ();
	}

}
