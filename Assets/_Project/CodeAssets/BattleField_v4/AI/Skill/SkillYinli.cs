using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
*  击退
*/
public class SkillYinli : SkillDataBead 
{
	private float m_fMove = 0;
	
	
	public SkillYinli(HeroSkill heroskill) : base(heroskill)
	{

	}
	
	public override void setData(ref string data)
	{
		m_fMove = float.Parse (Global.NextCutting(ref data));
		int temp = int.Parse(Global.NextCutting(ref data));
		float tempTime = float.Parse(Global.NextCutting(ref data));
		float time = float.Parse(Global.NextCutting(ref data));
		for(int i = 0; i < temp; i ++)
		{
			m_fTime.Add(time + (tempTime * (i + 1)));
		}
	}
	
	public override void activeSkill(int state, GameObject skillEff)
	{
		for(int i = 0; i < m_HeroSkill.m_listATTTarget.Count; i ++)
		{
			GameObject tempmubiao = new GameObject();
			tempmubiao.transform.position = m_HeroSkill.m_listATTTarget[i].m_Gameobj.transform.position;
			tempmubiao.transform.LookAt(skillEff.transform.position);
			Vector3 v3Move = tempmubiao.transform.forward.normalized;
			m_HeroSkill.m_listATTTarget[i].m_Gameobj.transform.position += v3Move;
		}
	}
}
