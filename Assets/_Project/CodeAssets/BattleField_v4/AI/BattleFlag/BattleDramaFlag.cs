using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleDramaFlag : MonoBehaviour
{
	public int flagId;

	public int nodeId = 1;

	public GameObject triggerFlagRoot;

	public List<BattleFlag> triggerFlagList = new List<BattleFlag> ();


	[HideInInspector] public List<int> triggerFlagListInteger = new List<int> ();


	private BaseAI node;


	public void refreshTriggerFlags()
	{
		if (triggerFlagRoot == null) return;
		
		Component[] coms = triggerFlagRoot.GetComponentsInChildren(typeof(BattleFlag));
		
		foreach(Component co in coms)
		{
			BattleFlag bf = (BattleFlag)co;
			
			bool b = true;
			
			foreach(BattleFlag temp in triggerFlagList)
			{
				if(temp.flagId == bf.flagId) 
				{
					b = false;
					
					break;
				}
			}
			
			if(b == true) triggerFlagList.Add(bf);
		}
	}

	void FixedUpdate ()
	{
		if (BattleControlor.Instance() == null) return;

		if(node == null)
		{
			node = BattleControlor.Instance().getNodebyId(nodeId);
		}

		if (node == null) return;

		bool f = IsColliderWith (node.gameObject);

		if(f == false && nodeId == 1 && BattleControlor.Instance().getKing().copyObject.activeSelf == true)
		{
			f = IsColliderWith (BattleControlor.Instance().getKing().copyObject);
		}

		if(f == true)
		{
			trigger(node);
		}
	}

	private bool IsColliderWith(GameObject t_node)
	{
		Vector3 pos2 = t_node.transform.position;
		
		Vector3 p4 = transform.position;
		
		pos2.y = 0;
		
		p4.y = 0;
		
		Vector3 m_VForWard = transform.forward;
		
		float m_iCollValue1 = transform.localScale.z;//boxCol.size.z;
		
		float m_iCollValue2 = transform.localScale.x;//boxCol.size.x;
		
		Vector3 p0 = p4 - ((m_VForWard * m_iCollValue1) / 2);
		
		Vector3 p1 = p4 + ((m_VForWard * m_iCollValue1) / 2);
		
		Vector3 p2 = p4 + (new Vector3(-m_VForWard.z, 0f, m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));
		
		Vector3 p3 = p4 + (new Vector3(m_VForWard.z, 0f, -m_VForWard.x).normalized * (m_iCollValue2 / 2.0f));
		
		if(Global.getCollRect(pos2, p1, p0, p3, p2))
		{
			return true;
		}

		return false;
	}

	public void trigger(BaseAI node)
	{
		if(node == null || !node.isAlive)
		{
			return;
		}

		foreach(int _flagId in triggerFlagListInteger)
		{
			if(_flagId == 0) continue;

			bool have = BattleControlor.Instance().flags.ContainsKey(_flagId);

			if(have)
			{
				BattleFlag flag = BattleControlor.Instance().flags[_flagId];

				flag.trigger();
			}
			else
			{
				BattleDoorFlag[] doors = BattleControlor.Instance().GetComponentsInChildren<BattleDoorFlag>();
				
				foreach(BattleDoorFlag doorFlag in doors)
				{
					if(doorFlag.flagId == _flagId)
					{
						doorFlag.trigger();
					}
				}
			}
		}

		Destroy (gameObject);
	}

}
