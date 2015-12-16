using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class SkillDataBead 
{
	protected HeroSkill m_HeroSkill;

	protected List<BaseAI> m_ListTargetBaseAI;

	public int m_iCurrentIndex = 0;

	public List<float> m_fTime = new List<float>();

	public bool isNextIndex = false;

	public SkillDataBead(HeroSkill HeroSkill)
	{
		m_HeroSkill = HeroSkill;
	}

	public virtual void setData(ref string data)
	{

	}

	public virtual IEnumerator StartTheTime()
	{
		yield return new WaitForSeconds (0.0f);
	}
	
	public virtual void activeSkill(int state, GameObject skillEff)
	{
		
	}

	public HeroSkill getHeroSkill()
	{
		return m_HeroSkill;
	}
}
