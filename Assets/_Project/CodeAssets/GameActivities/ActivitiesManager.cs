using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivitiesManager : MonoBehaviour {

//	List<CheckActivities> m_activities = new List<CheckActivities>();

	public List<GameObject> m_activitiesLayer = new List<GameObject>();

	void Start()
	{
//		foreach(Transform tempTransform in this.transform)
//		{
//			m_activities.Add(tempTransform.GetComponent<CheckActivities>());
//		}
	}

	public void ShowLayer(int indexLayer)
	{
		foreach(GameObject tempObject in m_activitiesLayer)
		{
			tempObject.SetActive(false);
		}

//		NGUIDebug.Log ("indexLayer: " + indexLayer.ToString());

		m_activitiesLayer [indexLayer - 1].gameObject.SetActive (true);
	}
	 
}
