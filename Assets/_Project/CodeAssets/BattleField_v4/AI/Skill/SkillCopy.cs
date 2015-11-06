using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

/*
*  分身
*/
public class SkillCopy : SkillDataBead 
{
	private int m_iCopyNum = 0;
	private List<int> m_listArrType = new List<int>();
	private List<int> m_iXishuType = new List<int>();
	private List<float> m_iValue = new List<float>();
	private List<BaseAI> m_listCopyBaseAI = new List<BaseAI>();
	
	public SkillCopy(HeroSkill heroskill) : base(heroskill)
	{
		
	}
	
	public override void setData(ref string data)
	{
		m_iCopyNum = int.Parse(Global.NextCutting(ref data));
		int tempNum = int.Parse(Global.NextCutting(ref data));
		for(int i = 0; i < tempNum; i ++)
		{
			m_listArrType.Add(int.Parse(Global.NextCutting(ref data)));
			m_iXishuType.Add(int.Parse(Global.NextCutting(ref data)));
			m_iValue.Add(float.Parse(Global.NextCutting(ref data)));
		}
		m_fTime.Add(float.Parse(Global.NextCutting(ref data)));
//		m_HeroSkill.dis += "生效时间" + m_fTime[0] + "，";
	}
	
	public override void activeSkill(int state)
	{
		for(int i = 0; i < m_listCopyBaseAI.Count; i ++)
		{
			if(m_listCopyBaseAI[i] == null)
			{
				m_listCopyBaseAI.RemoveAt(i);
				i--;
				continue;
			}
		}
		Debug.Log("生效");
		int tempNum = m_iCopyNum - m_listCopyBaseAI.Count;
		float angle = 360 / m_iCopyNum;
		for(int i = 0; i < tempNum; i ++)
		{
			Node node = m_HeroSkill.node.nodeData.nodeData;

			BaseAI new_Node = BattleControlor.Instance().CreateNode(
				node, 
				BaseAI.Stance.STANCE_ENEMY, 
				m_HeroSkill.node.flag.flagId, 
				m_HeroSkill.node.m_Gameobj,
				i + 1);

			new_Node.gameObject.transform.localPosition = m_HeroSkill.node.gameObject.transform.localPosition;
			new_Node.gameObject.transform.localRotation = m_HeroSkill.node.gameObject.transform.localRotation;
			new_Node.gameObject.transform.localScale = m_HeroSkill.node.gameObject.transform.localScale;
			for(int q = 0; q < new_Node.nodeData.GetAttributeCount(); q ++)
			{
				new_Node.nodeData.SetAttribute(q, m_HeroSkill.node.nodeData.GetAttribute( q ) );
			}
			for(int j = 0; j < m_listArrType.Count; j++)
			{
				int arrType = m_listArrType[j];

				int xiShuType = m_iXishuType[j];

				float xishuValue = m_iValue[j];

				float va = new_Node.nodeData.GetAttribute( arrType );

				if(xiShuType == 0)
				{
					va = xishuValue;
				}
				else
				{
					va = va * xishuValue;
				}

				va += new_Node.nodeData.GetAttribute( arrType );

				new_Node.nodeData.SetAttribute( arrType, va );
			}

			new_Node.skills = new List<HeroSkill>();

			HeroSkill[] skills = new_Node.gameObject.GetComponents<HeroSkill> ();
			
			foreach(HeroSkill sk in skills)
			{
				GameObject.Destroy(sk);
			}

			//new_Node.gameObject.name = new_Node.gameObject.name + "Copy" + i;

			GameObject tempObj = new GameObject();
			tempObj.transform.localPosition = Vector3.zero;
			tempObj.transform.localRotation = Quaternion.Euler(new Vector3(0, angle * i + 90, 0));
			tempObj.transform.localScale = new Vector3(1, 1, 1);

			//new_Node.gameObject.transform.localPosition += (tempObj.transform.forward.normalized * 3);
			new_Node.gameObject.SetActive(true);
			m_listCopyBaseAI.Add(new_Node);
			m_HeroSkill.isFirse(new_Node);

			new_Node.showText(m_HeroSkill.m_sDec, m_HeroSkill.template.id);
		}
	}
}
