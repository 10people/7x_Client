using UnityEngine;
using System.Collections;

public class TopUpSpecialChargeManagerment : MonoBehaviour
{
    public GameObject m_TopUpObject;
	void Start () {
	
	}

    public void SpecialCharge()
    {
        m_TopUpObject.GetComponent<TopUpLayerManagerment>().isSpecial = true;
    }
}
