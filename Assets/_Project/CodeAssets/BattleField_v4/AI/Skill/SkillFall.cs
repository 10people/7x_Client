using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * 倒地
 */
public class SkillFall : SkillDataBead 
{
	private float m_fDis;

	private int m_lv;

	
	public SkillFall(HeroSkill heroskill) : base(heroskill)
	{
		m_HeroSkill = heroskill;
	}
	
	public override void setData(ref string data)
	{
		m_fDis = float.Parse(Global.NextCutting(ref data));
//		m_HeroSkill.dis += "击退距离" + m_fDis + "，";
		m_lv = int.Parse (Global.NextCutting(ref data));
//		m_HeroSkill.dis += "击退等级" + beatdownId + "，";
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] + "，";
	}
	
	public override void activeSkill(int state)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			BaseAI defender = m_HeroSkill.m_listATTTarget[i];

			defender.beatDown(m_lv);

			if(m_HeroSkill.node.nodeData.nodeType == qxmobile.protobuf.NodeType.PLAYER)
			{
				BattleControlor.Instance().getKing().Shake(KingCamera.ShakeType.Vertical);
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
