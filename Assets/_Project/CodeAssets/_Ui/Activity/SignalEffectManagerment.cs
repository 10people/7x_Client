using UnityEngine;
using System.Collections;

public class SignalEffectManagerment : MonoBehaviour
{
    public static SignalEffectManagerment m_SignalEffect;

    void Start ()
    {
        m_SignalEffect = this;
    }
	
 
	void OnEnable ()
    {
        m_SignalEffect = this;
    }

    void OnDisable()
    {
        m_SignalEffect = null;
    }
}
