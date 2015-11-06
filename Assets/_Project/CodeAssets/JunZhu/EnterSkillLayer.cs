using UnityEngine;
using System.Collections;

public class EnterSkillLayer : MonoBehaviour{

    public GameObject m_layer;

    public GameObject m_beforeLayer;

    void OnClick()
    {
        m_layer.SetActive(true);

        m_beforeLayer.SetActive(false);
    }
}
