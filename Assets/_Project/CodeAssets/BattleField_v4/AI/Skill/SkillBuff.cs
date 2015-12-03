using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  加BUFF
*/
public class SkillBuff : SkillDataBead
{
	public int m_iBuffID;

	public int m_iXishuType;

	public float m_fXishu;

	public float m_fLastTime;

	public float m_fValue1;

	public float m_fValue2;

	public int m_iBuffType = 0;

	public int m_iEffID = 0;//BUFF绑定特效

	public int m_lv;//控制技能等级


	public SkillBuff(HeroSkill heroskill) : base(heroskill)
	{
		
	}

	public override void setData(ref string data)
	{
		m_iBuffID = int.Parse(Global.NextCutting(ref data));

		if(m_iBuffID == 100 || (m_iBuffID >= 102 && m_iBuffID <= 130))
		{
			m_fLastTime = float.Parse(Global.NextCutting(ref data));
		}
		else if(m_iBuffID == 101)
		{
			m_iXishuType = int.Parse(Global.NextCutting(ref data));

			m_fXishu = float.Parse(Global.NextCutting(ref data));

			m_fLastTime = float.Parse(Global.NextCutting(ref data));

			m_fValue1 = float.Parse(Global.NextCutting(ref data));
		}
		else
		{
			m_iXishuType = int.Parse(Global.NextCutting(ref data));

			m_fXishu = float.Parse(Global.NextCutting(ref data));

			m_fLastTime = float.Parse(Global.NextCutting(ref data));
		}

		m_iBuffType = int.Parse(Global.NextCutting(ref data));

		if(m_iBuffID == 100)
		{
			m_lv = int.Parse(Global.NextCutting(ref data));
		}
		else if(m_iBuffID == 108 || m_iBuffID == 109)
		{
			m_fValue1 = float.Parse(Global.NextCutting(ref data));

			m_fValue2 = float.Parse(Global.NextCutting(ref data));
		}

		m_iEffID = int.Parse(Global.NextCutting (ref data));

		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0];
	}

	public override void activeSkill(int state)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
			if(defender.gameObject.activeSelf && defender.isAlive)//是否可被攻击
			{
				float t_XiShutype = m_iXishuType;

				float va = m_fXishu;

				if(m_iBuffID < 100)
				{
					if(t_XiShutype == 1)
					{
						va = m_fXishu * defender.nodeData.GetAttribute( m_iBuffID );
					}
				}
				else if(m_iBuffID == 101)
				{
					if(t_XiShutype == 1)
					{
						va = BattleControlor.Instance().getAttackValueSkill(m_HeroSkill.node, defender, va, 0, false).Float;
					}
				}

				if(m_iBuffID == 100)
				{
					bool able = ControlOrderLvTemplate.getCantrolableById(m_lv, AIdata.AttributeType.ATTRTYPE_ReductionDisable, defender);

					if(able == true) Buff.createBuff(defender, (AIdata.AttributeType)m_iBuffID, va, m_fLastTime, this);
				}
				else if(m_iBuffID == 73)//增加的仇恨值，用于BUFF计算
				{
					Buff.createBuffThreat(defender, va, m_fLastTime, m_HeroSkill.node.nodeId);
				}
				else
				{
					Buff.createBuff(defender, (AIdata.AttributeType)m_iBuffID, va, m_fLastTime, this);
				}

				m_HeroSkill.isFirse(defender);
			}

			defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
		}
	}
}
