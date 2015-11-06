using UnityEngine;
using System.Collections;

public class PlayerShadowManagerment : MonoBehaviour
{
    public GameObject m_Shadow;
	void Start () 
    {
        m_Shadow.SetActive( QualityTool.Instance.InCity_ShowSimpleShadow() );
	}

 
	// Update is called once per frame

}
