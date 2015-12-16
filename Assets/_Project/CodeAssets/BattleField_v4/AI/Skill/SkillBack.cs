using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  击退
*/
public class SkillBack : SkillDataBead 
{
	private int m_iDis = 0;

	private int m_iType = 0;

	private int m_lv = 0;

	private float m_moveTime;


	public SkillBack(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData(ref string data)
	{
		m_iDis = int.Parse(Global.NextCutting(ref data));
		m_iType = int.Parse(Global.NextCutting(ref data));
		m_lv = int.Parse(Global.NextCutting(ref data));
		m_moveTime = float.Parse (Global.NextCutting(ref data));
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0];
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
			if(defender.gameObject.activeSelf && defender.isAlive)//是否可被攻击
			{
				Vector3 targetPos = defender.transform.position;

				if(m_iType == 0)//平推
				{
					targetPos = defender.transform.position + m_HeroSkill.node.transform.forward * m_iDis;
				}
				else if(m_iType == 1)//发散
				{
					defender.transform.forward = m_HeroSkill.node.transform.position - defender.transform.position;

					if(m_iDis > 0)
					{
						targetPos = defender.transform.position + defender.transform.forward * (-m_iDis);
					}
					else
					{
						float distance = Vector3.Distance(defender.transform.position, m_HeroSkill.node.transform.position);

						if(distance < Mathf.Abs(m_iDis))
						{
							targetPos = defender.transform.position + defender.transform.forward * (distance - defender.radius - m_HeroSkill.node.radius);
						}
						else
						{
							targetPos = defender.transform.position + defender.transform.forward * (-m_iDis);
						}
					}
				}

				defender.gameObject.transform.LookAt(m_HeroSkill.node.gameObject.transform.localPosition);

				if (defender.IsPlaying ().IndexOf (defender.getAnimationName (BaseAI.AniType.ANI_DODGE)) == -1) 
				{
					bool able = ControlOrderLvTemplate.getCantrolableById(m_lv, AIdata.AttributeType.ATTRTYPE_ReductionBTACMove, defender);

					if(able == true) defender.movement(0, targetPos, iTween.EaseType.easeOutExpo, m_moveTime);
				
					if(BattleControlor.Instance().achivement != null && defender.nodeId == 1)
					{
						BattleControlor.Instance().achivement.ByJiTui();
					}
				}
				defender.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
				m_HeroSkill.isFirse(defender);
			}
		}
	}
}
