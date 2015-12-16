using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  加BUFF
*/
public class SkillDeleteBuff : SkillDataBead
{
	public int m_iBuffType = 0;//减少类型

	public SkillDeleteBuff(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData(ref string data)
	{
		m_iBuffType = int.Parse(Global.NextCutting(ref data));
//		m_HeroSkill.dis += "删除BUFF类型" + m_iBuffType + "，";
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] + "，";
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			m_HeroSkill.m_listATTTarget[i].deleteBuff(this);
			m_HeroSkill.m_listATTTarget[i].showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
		}
	}
}
