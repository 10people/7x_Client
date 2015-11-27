using UnityEngine;
using System.Collections;

public class Threat 
{
	public BaseAI node
	{
		get
		{
			return m_node;
		}
		set
		{
			m_node = value;

			m_nodeId = value.nodeId;
		}
	}

	public int nodeId
	{
		get
		{
			return m_nodeId;
		}
	}

	public float lengthThreat;

	public float skillThreat;

	public float actionThreat;

	public float totalThreat
	{
		get
		{
			return lengthThreat + skillThreat + actionThreat;
		}
	}


	private BaseAI m_node;

	private int m_nodeId;

}
