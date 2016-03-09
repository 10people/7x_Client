using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 强制播放被击动作
 */
public class SkillByAtk : SkillDataBead 
{
	private float m_fDis;

	private int m_lv = 0;
	
	public SkillByAtk(HeroSkill heroskill) : base(heroskill)
	{
		m_HeroSkill = heroskill;
	}
	
	public override void setData(ref string data)
	{
		m_lv = int.Parse(Global.NextCutting(ref data));

		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] + "，";
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];

			bool able = ControlOrderLvTemplate.getCantrolableById(m_lv, AIdata.AttributeType.ATTRTYPE_ReductionBTAC, defender);

			defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);

			if(able == true) defender.rupt();
		}

		//		m_HeroSkill.m_listATTTarget
		
		//		BattleEffectControllor.Instance().PlayEffect (
		//			BattleEffectControllor.EffectType.EFFECT_KING_CESHI_SKILL_BAOQI,
		//			m_BaseAI.targetNode.gameObject);
		//		Buff.createBuff(m_BaseAI.targetNode, Buff.BuffType.BUFF_HP, m_NodeSkill.value1, m_NodeSkill.value2);
	}
}
