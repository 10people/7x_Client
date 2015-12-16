using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 召唤
 */
public class SkillSummon : SkillDataBead 
{
	private List<int> m_iID = new List<int>();
	
	public SkillSummon(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData( ref string data ){
		if (data == null) {
			Debug.LogError ("SkillSummon.setData data = null." + data);
		} else if (data == "") {
				Debug.LogError ("SkillSummon.setData data == \"\" " + data);
		} else {
//			Debug.Log( "SkillSummon: " + data );
		}

		string t_str =Global.NextCutting(ref data);

		if (t_str == null) {
			Debug.LogError ("t_str == null.");	
		} else  if( string.IsNullOrEmpty( t_str ) ){
			Debug.LogError( "IsNullOrEmpty: " + t_str );
		}

		int tempNum = int.Parse ( t_str );
//		m_HeroSkill.dis += "可以召唤" + tempNum + "个，分别为";
//		Debug.Log( "tempNum: " + tempNum );

		for(int i = 0; i < tempNum; i ++)
		{
			m_iID.Add(int.Parse(Global.NextCutting(ref data)));
//			m_HeroSkill.dis += (m_iID[i] + ",");
		}
//		m_HeroSkill.dis += ",";
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] + "，";
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
//		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
//		{
//			BaseAI defender = m_HeroSkill.m_listATTTarget[i];
//		}

		foreach(int id in m_iID)
		{
			BaseAI node = getNodeById(id);

			if(node == null) continue;

			if(node.isAlive == false)
			{
				node.relive();
			}
			else
			{
				node.fadeIn();
			}
			node.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
			m_HeroSkill.isFirse(node);
		}
	}

	private BaseAI getNodeById(int id)
	{
		foreach(BattleFlag flag in BattleControlor.Instance().flags.Values)
		{
			if(flag.flagId == id)
			{
				return flag.node;
			}
		}

		return null;
	}

}
