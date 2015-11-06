using UnityEngine;
using System.Collections;

public class AllianceClose : MonoBehaviour 
{
   public GameObject m_gameObject;
	void Start () 
    {
	
	}

    void OnClick()
    {
        if (m_gameObject != null)
        {

            Destroy(m_gameObject);
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
        }
    }
}
