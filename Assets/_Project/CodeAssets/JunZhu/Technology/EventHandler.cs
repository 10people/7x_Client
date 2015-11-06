using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour {

    public delegate void eventHandler(GameObject tempObject);

    public event eventHandler m_handler;

    void OnClick()
    {
        if (m_handler != null)
        {
            m_handler(this.gameObject);
        }
    }
}
