using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  加血
*/
public class SkillAddHP : SkillDataBead 
{
	private int m_iAddType = 0;

	private float m_iAddXishu = 0;


	public SkillAddHP(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData(ref string data)
	{
//		Debug.Log(data);

		m_iAddType = int.Parse(Global.NextCutting(ref data));
		m_iAddXishu = (float.Parse(Global.NextCutting(ref data)));
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0].ToString();
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		//Debug.Log("调试成功");
		
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
			if(defender.gameObject.activeSelf && defender.isAlive)//是否可被攻击
			{
				float va = m_iAddXishu;
				
				if(m_iAddType == 1)
				{
					va = m_iAddXishu * defender.nodeData.GetAttribute( (int)AIdata.AttributeType.ATTRTYPE_hpMax );
				}
				
				defender.addHp(va);
				m_HeroSkill.isFirse(defender);
				defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
			}
		}
	}

}
