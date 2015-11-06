using UnityEngine;
using System.Collections;

public class WorshipJoinHandsManagerment : MonoBehaviour 
{
    public GameObject m_SignalOne;
    public GameObject m_SignalTwo;
    public UILabel m_LabCount;
	void Start () 
    
    {
	
	}

    void OnPress(bool isDown)
    {
        if (isDown)
        {
            if (int.Parse(m_LabCount.text) < 100)
            {
                m_SignalOne.SetActive(true);
            }
            else
            {
                m_SignalTwo.SetActive(true);
            }

        }
        else
        { 
             m_SignalOne.SetActive(false);
             m_SignalTwo.SetActive(false);
        }
    }
}
