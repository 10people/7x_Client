using UnityEngine;
using System.Collections;

public class PlayerEnterCollider : MonoBehaviour {

	public int m_index;

	public delegate void EnterCollider(int tempIndex,bool tempEnterState,Transform tempCollider,Transform tempTransform);

	public event EnterCollider m_colliser;

	void OnTriggerEnter(Collider tempCollider)
	{
		if(m_colliser != null)
		{
			m_colliser(m_index,true,tempCollider.transform,this.transform.parent);
		}
	}

	void OnTriggerExit(Collider tempCollider)
	{
		if(m_colliser != null)
		{
			m_colliser(m_index,false,tempCollider.transform,this.transform.parent);
		}
	}
}
