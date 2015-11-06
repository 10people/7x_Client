using UnityEngine;
using System.Collections;

public class ButtonDepthManagerment : MonoBehaviour {

    public GameObject m_BackGround;
	void Start () 
    {
	
	}

    public void ObjectDepthController(bool isUp)
    {
        if (isUp)
        {
            m_BackGround.GetComponent<UIWidget>().depth = 6;
        }
        else
        {
            m_BackGround.GetComponent<UIWidget>().depth = 4;
        }
    }
   
}
