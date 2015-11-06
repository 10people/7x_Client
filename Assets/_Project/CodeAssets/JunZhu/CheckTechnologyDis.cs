using UnityEngine;
using System.Collections;

public class CheckTechnologyDis : MonoBehaviour {

    public delegate void CheckDis();

    public event CheckDis m_checkDis;

    void OnClick()
    {
        if (m_checkDis != null)
        {
            m_checkDis();
        }
    }
    
}
