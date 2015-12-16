using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 复活
 */
public class SkillResurrection : SkillDataBead 
{
	public List<int> m_iID = new List<int>();
	
	public SkillResurrection(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData(ref string data)
	{
		int tempNum = int.Parse(Global.NextCutting(ref data));
//		m_HeroSkill.dis += ("可以复活" + tempNum + "个,");
		for(int i = 0; i < tempNum; i ++)
		{
			m_iID.Add(int.Parse(Global.NextCutting(ref data)));
//			m_HeroSkill.dis += (m_iID[i] + "|");
		}
//		m_HeroSkill.dis += ",";
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] +",";
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
			if(!defender.isAlive)
			{
				defender.relive();
				m_HeroSkill.isFirse(defender);
				defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
			}
		}
	}
}
