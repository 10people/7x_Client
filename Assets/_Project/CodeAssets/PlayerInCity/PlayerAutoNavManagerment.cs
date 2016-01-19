using UnityEngine;
using System.Collections;

public class PlayerAutoNavManagerment : MonoBehaviour
{
    public Animator m_NavAnim;

	void OnDestroy(){
		m_NavAnim = null;
	}

    public void AnimController()
    {
        m_NavAnim.enabled = false;
        if (m_NavAnim.name.Equals("Label7"))
        {
            PlayeratuoMoveManagerMent.m_AnimOver = true;
        }
    }
 
}
