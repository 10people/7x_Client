using UnityEngine;
using System.Collections;

public class ChangeLayer : MonoBehaviour{

    public GameObject m_currentLayer;

    public GameObject m_nextLayer;
	public bool IsOn = false;

    void OnClick()
    {
		if(m_nextLayer.name.Equals("Technology"))
		IsOn = true;

        Change();
    }

    public void Change()
    {
        if (m_currentLayer != null)
        {
            m_currentLayer.SetActive(false);
        }
        if (m_nextLayer != null)
        {
            m_nextLayer.SetActive(true);
        }
    }
}
