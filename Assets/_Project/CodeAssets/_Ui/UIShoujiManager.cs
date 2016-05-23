using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShoujiManager : MonoBehaviour {
	public static UIShoujiManager m_UIShoujiManager;
	public int m_iIndex;
	public bool m_isPlayShouji = true;
	public List<UIShouji> m_listUIShouji;
	public bool m_isPlay = true;
	// Use this for initialization
	void Start () {
		m_UIShoujiManager = this;
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setData(ShoujiData data)
	{
		m_isPlay = false;
		m_listUIShouji[m_iIndex].setData(data);
		m_iIndex = ++m_iIndex % 2;
	}

	public void close()
	{

	}
}
