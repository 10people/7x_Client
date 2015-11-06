using UnityEngine;
using System.Collections;

public class Eventitem : MonoBehaviour {

	public string EventData;
	public UILabel m_Time;
	public UILabel m_Event;

	void Start () {
	
	}

	void Update () {
	
	}
	public void init()
	{

		char[] w = {'#'};

		//Debug.Log ("EventData = " +EventData);

		string[] s = EventData.Split (w);
		for (int i = 0; i < s.Length; i++) 
		{
			m_Time.text = s[0];
			
			m_Event.text = s[1];
		}

	}
}
