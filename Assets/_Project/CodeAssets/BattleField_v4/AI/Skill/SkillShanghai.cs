using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillShanghai : SkillDataBead 
{
	private List<int> m_iType = new List<int>();
	private List<float> m_iXishu = new List<float>();

	public SkillShanghai(HeroSkill heroskill) : base(heroskill)
	{

	}

	public override void setData(ref string data)
	{
		int tempNum = int.Parse(Global.NextCutting(ref data));
		float time;
		if(tempNum == -1)
		{
			tempNum = int.Parse(Global.NextCutting(ref data));
			time = float.Parse(Global.NextCutting(ref data));
			int tempType = int.Parse(Global.NextCutting(ref data));
			float tempXishu = float.Parse(Global.NextCutting(ref data));
			float tempTime = float.Parse(Global.NextCutting(ref data));
			for(int i = 0; i < tempNum; i ++)
			{
				m_iType.Add(tempType);
				m_iXishu.Add(tempXishu);
				m_fTime.Add(time + (tempTime * (i + 1)));
			}
		}
		else
		{
			for(int i = 0; i < tempNum; i ++)
			{
				m_iType.Add(int.Parse(Global.NextCutting(ref data)));
				m_iXishu.Add(float.Parse(Global.NextCutting(ref data)));
				m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
			}
		}
	}

	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
			if(defender.gameObject.activeSelf && defender.isAlive)//是否可被攻击
			{
				float xishu = m_iType[m_iCurrentIndex] == 1 ? m_iXishu[m_iCurrentIndex] : 0;

				float guding = m_iType[m_iCurrentIndex] == 0 ? m_iXishu[m_iCurrentIndex] : 0;

				FloatBoolParam fbp = BattleControlor.Instance().getAttackValueSkill(m_HeroSkill.node, defender, xishu, guding);

				foreach(Buff buff in defender.buffs)
				{
					if(buff.buffType == AIdata.AttributeType.ATTRTYPE_ECHO_SKILL)
					{
						m_HeroSkill.node.attackHp(m_HeroSkill.node, fbp.Float * buff.supplement.m_fValue2, fbp.Bool, BattleControlor.AttackType.SKILL_REFLEX);
						
						fbp.Float = buff.supplement.m_fValue1 * fbp.Float;

						defender.showText(LanguageTemplate.GetText(LanguageTemplate.Text.BATTLE_SKILL_REFLEX_NAME), buff.supplement.getHeroSkill().template.id);

						break;
					}
				}

				m_HeroSkill.node.attackHp(defender, fbp.Float, fbp.Bool, BattleControlor.AttackType.SKILL_ATTACK);
			
				m_HeroSkill.isFirse(defender);

				defender.m_listBySkill.Add(m_HeroSkill.node);
				m_HeroSkill.node.m_listSkill.Add(defender);

				defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
			}
		}
//		m_HeroSkill.m_listATTTarget

//		BattleEffectControllor.Instance ().PlayEffect (
//			BattleEffectControllor.EffectType.EFFECT_KING_CESHI_SKILL_BAOQI,
//			m_BaseAI.targetNode.gameObject);
//		Buff.createBuff(m_BaseAI.targetNode, Buff.BuffType.BUFF_HP, m_NodeSkill.value1, m_NodeSkill.value2);
	}
}
