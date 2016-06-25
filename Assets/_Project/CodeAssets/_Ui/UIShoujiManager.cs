using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShoujiManager : MonoBehaviour, IUIRootAutoActivator{
	public static UIShoujiManager m_UIShoujiManager;
	public int m_iIndex;
	public bool m_isPlayShouji = true;
	public List<UIShouji> m_listUIShouji;
	public bool m_isPlay = true;

	void Awake(){
		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
	}

	void OnDestroy(){
		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}
	}

	// Use this for initialization
	void Start () {
		m_UIShoujiManager = this;
		DontDestroyOnLoad(gameObject);
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

	public bool IsNGUIVisible()
	{
		return m_listUIShouji[0].gameObject.activeSelf || m_listUIShouji[1].gameObject.activeSelf;
	}

}
