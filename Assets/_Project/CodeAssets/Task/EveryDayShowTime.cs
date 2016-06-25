using UnityEngine;
using System.Collections;

public class EveryDayShowTime : Singleton<EveryDayShowTime> {
	// Use this for initialization
	public GameObject m_objShow0;
	public GameObject m_objShow1;
	public GameObject m_objShow2;
	public GameObject m_objShow3;
	public bool m_isLoad0 = false;
	public bool m_isLoad1 = false;
	public bool m_isLoad2 = false;
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () {
		if(m_isLoad0 && m_isLoad1 && m_isLoad2)
		{
			if(!m_objShow0.activeSelf)
			{
				m_objShow0.SetActive(true);
				m_objShow1.SetActive(true);
				m_objShow2.SetActive(true);
				m_objShow3.SetActive(true);
			}
		}
	}
}
