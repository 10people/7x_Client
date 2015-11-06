using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CloseHidenLayer : MonoBehaviour {

    public List<GameObject> m_ListHide = new List<GameObject>();

	void Start ()
    {
	
	}

    void OnClick()
    {
        foreach (GameObject obj in m_ListHide)
        {
            obj.SetActive(false);
        }
    }
 
}
