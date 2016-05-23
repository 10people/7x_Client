using UnityEngine;
using System.Collections;

public class EquipGrowthCompoundButtonEffectManangerment : MonoBehaviour {

    public GameObject m_ButtonXie;
    public GameObject m_Buttonback;
    void OnEnable()
    {
      
        m_ButtonXie.transform.localPosition = new Vector3(-29, -20, 0);
        m_Buttonback.transform.localPosition = new Vector3(-170, -20, 0);
    }

    void OnDisable()
    {
        m_ButtonXie.transform.localPosition = new Vector3(74, -20, 0);
        m_Buttonback.transform.localPosition = new Vector3(-139, -20, 0);
    }
}
