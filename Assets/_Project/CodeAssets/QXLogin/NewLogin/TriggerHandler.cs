using UnityEngine;
using System.Collections;

public class TriggerHandler : MonoBehaviour {

	public delegate void triggerHandler (Collider item);
	
	public event triggerHandler m_handler;
	
	void OnTriggerEnter (Collider item)
	{
		if (m_handler != null)
		{
			m_handler (item);
		}
	}
}
