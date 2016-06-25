using UnityEngine;
using System.Collections;

public class TCitySimpleMove : MonoBehaviour {

	private NavMeshAgent m_agent;

	void Start ()
	{
		m_agent = GetComponent<NavMeshAgent> ();
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			LayerMask mask = 1 << 11;

			if (Physics.Raycast (ray,out hit,200,mask))
			{
				m_agent.destination = hit.point;
			}
		}
	}
}
